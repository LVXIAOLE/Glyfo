using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using WinOcrEngine = Windows.Media.Ocr.OcrEngine;
using WinOcrLine = Windows.Media.Ocr.OcrLine;
using WinOcrResult = Windows.Media.Ocr.OcrResult;

namespace Glyfo.Services;

/// <summary>
/// The OCR engine built into Windows since 10 1809. No model files ship with the app — the
/// recognizable languages are exactly the language packs the user has installed.
/// </summary>
public sealed class WindowsMediaOcrEngine : IOcrEngine
{
    /// <summary>
    /// Sentinel <c>languageTag</c> that runs several recognizers and keeps the best answer.
    /// </summary>
    public const string AutoLanguageTag = "*";

    /// <summary>Recognizers the auto mode is willing to try, before it gets too slow to be useful.</summary>
    private const int MaxAutoCandidates = 3;

    /// <summary>
    /// How much better a lower-priority recognizer has to be before it displaces a higher-priority
    /// one. Candidates arrive in the user's own language order, and that order is the better guess
    /// whenever the scores are close.
    /// </summary>
    private const double AutoWinMargin = 1.15;

    private static string? _preferredLanguageTag;
    private static bool _preferredLanguageResolved;

    private readonly Dictionary<string, WinOcrEngine> _engines = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _disposed;

    public string DisplayName => "Windows OCR";

    /// <summary>
    /// The recognizer language that best matches the user's own language list, or null when none
    /// of their languages has a pack installed. Used to pick the default entry in the language
    /// picker — alphabetical order is meaningless there, and landing on the wrong recognizer looks
    /// exactly like an inaccurate engine.
    /// </summary>
    public static string? GetPreferredLanguageTag()
    {
        if (_preferredLanguageResolved)
        {
            return _preferredLanguageTag;
        }

        _preferredLanguageResolved = true;

        try
        {
            // GlobalizationPreferences.Languages is in the user's own priority order, and
            // TryCreateFromLanguage does the tag matching for us ("zh-CN" -> "zh-Hans-CN").
            foreach (var tag in Windows.System.UserProfile.GlobalizationPreferences.Languages)
            {
                try
                {
                    var engine = WinOcrEngine.TryCreateFromLanguage(new Language(tag));
                    if (engine is not null)
                    {
                        _preferredLanguageTag = engine.RecognizerLanguage.LanguageTag;
                        return _preferredLanguageTag;
                    }
                }
                catch (ArgumentException)
                {
                    // Malformed entry in the profile list; try the next one.
                }
            }

            _preferredLanguageTag = WinOcrEngine.TryCreateFromUserProfileLanguages()?.RecognizerLanguage.LanguageTag;
        }
        catch (Exception)
        {
            _preferredLanguageTag = null;
        }

        return _preferredLanguageTag;
    }

    /// <summary>
    /// The recognizers the auto mode will try, in the order it will try them: the user's own
    /// languages first, then English as a Latin fallback. Deduplicated, because "zh-CN" and
    /// "zh-Hans" both resolve to the same recognizer and running it twice buys nothing.
    /// </summary>
    public static IReadOnlyList<OcrLanguage> GetAutoCandidates()
    {
        var installed = new Dictionary<string, OcrLanguage>(StringComparer.OrdinalIgnoreCase);
        try
        {
            foreach (var language in WinOcrEngine.AvailableRecognizerLanguages)
            {
                installed[language.LanguageTag] = new OcrLanguage(language.LanguageTag, language.DisplayName);
            }
        }
        catch (Exception)
        {
            return Array.Empty<OcrLanguage>();
        }

        var candidates = new List<OcrLanguage>();

        void TryAdd(string? tag)
        {
            if (tag is null || candidates.Count >= MaxAutoCandidates)
            {
                return;
            }

            if (installed.TryGetValue(tag, out var language) &&
                !candidates.Any(c => string.Equals(c.Tag, tag, StringComparison.OrdinalIgnoreCase)))
            {
                candidates.Add(language);
            }
        }

        try
        {
            foreach (var tag in Windows.System.UserProfile.GlobalizationPreferences.Languages)
            {
                try
                {
                    TryAdd(WinOcrEngine.TryCreateFromLanguage(new Language(tag))?.RecognizerLanguage.LanguageTag);
                }
                catch (ArgumentException)
                {
                    // Malformed entry in the profile list; try the next one.
                }
            }
        }
        catch (Exception)
        {
            // Fall through to whatever was collected.
        }

        // The CJK recognizers read plain Latin words well but mangle punctuation inside
        // alphanumeric tokens, so an English recognizer is worth having in the running even when
        // the user never listed English as one of their languages.
        TryAdd("en-US");

        return candidates;
    }

    public IReadOnlyList<OcrLanguage> GetAvailableLanguages()
    {
        try
        {
            return WinOcrEngine.AvailableRecognizerLanguages
                .Select(language => new OcrLanguage(language.LanguageTag, language.DisplayName))
                .OrderBy(language => language.DisplayName, StringComparer.CurrentCulture)
                .ToList();
        }
        catch (Exception)
        {
            // A broken language-pack registration should degrade to "no languages", not a crash.
            return Array.Empty<OcrLanguage>();
        }
    }

    public async Task<OcrResult> RecognizeAsync(SoftwareBitmap bitmap, string? languageTag)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(bitmap);

        WinOcrEngine engine;
        WinOcrResult result;

        if (string.Equals(languageTag, AutoLanguageTag, StringComparison.Ordinal))
        {
            (engine, result) = await RecognizeWithBestEngineAsync(bitmap);
        }
        else
        {
            engine = await GetEngineAsync(languageTag);
            result = await engine.RecognizeAsync(bitmap);
        }

        result = await TryRescaledPassAsync(engine, bitmap, result);

        // The recognizer that actually ran, not the tag that was asked for: auto mode picks its own
        // and an unavailable tag silently falls back to the profile engine.
        return new OcrResult(BuildText(result.Lines, engine.RecognizerLanguage.LanguageTag), null, DisplayName);
    }

    private async Task<WinOcrEngine> GetEngineAsync(string? languageTag)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            return GetOrCreateEngine(languageTag);
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    /// Runs each candidate recognizer and keeps the one that claimed the most of the image.
    /// </summary>
    /// <remarks>
    /// Windows.Media.Ocr reports no confidence, so the pick has to come from the geometry. Covered
    /// word area is the strongest signal available: a recognizer pointed at the wrong script does
    /// not merely misread the text, it fails to find whole regions of it. Character count is a
    /// much weaker discriminator — a Chinese recognizer reading English still emits roughly the
    /// right number of characters.
    ///
    /// Costs one full pass per candidate, which is why this only runs when the user picks the
    /// auto entry rather than a specific language.
    /// </remarks>
    private async Task<(WinOcrEngine Engine, WinOcrResult Result)> RecognizeWithBestEngineAsync(SoftwareBitmap bitmap)
    {
        WinOcrEngine? bestEngine = null;
        WinOcrResult? bestResult = null;
        var bestScore = 0.0;

        foreach (var candidate in GetAutoCandidates())
        {
            WinOcrEngine engine;
            try
            {
                engine = await GetEngineAsync(candidate.Tag);
            }
            catch (InvalidOperationException)
            {
                continue;
            }

            var result = await engine.RecognizeAsync(bitmap);
            var score = ScoreCoverage(result);

            if (bestEngine is null || score > bestScore * AutoWinMargin)
            {
                bestEngine = engine;
                bestResult = result;
                bestScore = score;
            }
        }

        if (bestEngine is null || bestResult is null)
        {
            // No candidate could be created — fall back to whatever the profile gives us and let
            // GetOrCreateEngine raise the "install a language pack" message if even that fails.
            var fallback = await GetEngineAsync(null);
            return (fallback, await fallback.RecognizeAsync(bitmap));
        }

        return (bestEngine, bestResult);
    }

    private static double ScoreCoverage(WinOcrResult result) =>
        result.Lines
            .SelectMany(line => line.Words)
            .Sum(word => word.BoundingRect.Width * word.BoundingRect.Height);

    /// <summary>
    /// Runs the recognizer a second time on an enlarged copy when the first pass found only small
    /// glyphs — or nothing at all.
    /// </summary>
    /// <remarks>
    /// Windows OCR degrades sharply below ~20px word height, which is exactly what a 100%-scaled
    /// screenshot of body text gives it. Upscaling before the first pass would be wasteful (most
    /// photos and scans are already large enough) and blind, so instead the first pass measures
    /// the actual glyph size and only then decides. Falls back to the original result on any
    /// failure — a worse second pass must never lose the first one.
    /// </remarks>
    private static async Task<WinOcrResult> TryRescaledPassAsync(
        WinOcrEngine engine, SoftwareBitmap bitmap, WinOcrResult first)
    {
        var scale = SuggestRescale(bitmap, first);
        if (scale <= 1)
        {
            return first;
        }

        try
        {
            using var enlarged = await ImageLoader.ScaleAsync(bitmap, scale);
            var second = await engine.RecognizeAsync(enlarged);

            // Windows OCR exposes no confidence, so "found more words" is the only signal
            // available. Ties go to the enlarged pass, which is the one with more detail to work
            // from and in practice gets the characters within a word right more often.
            return WordCount(second) >= WordCount(first) ? second : first;
        }
        catch (Exception)
        {
            return first;
        }
    }

    private static double SuggestRescale(SoftwareBitmap bitmap, WinOcrResult result) =>
        ImageLoader.SuggestUpscale(
            bitmap.PixelWidth,
            bitmap.PixelHeight,
            result.Lines.SelectMany(line => line.Words).Select(word => word.BoundingRect.Height).ToList());

    private static int WordCount(WinOcrResult result) => result.Lines.Sum(line => line.Words.Count);

    private WinOcrEngine GetOrCreateEngine(string? languageTag)
    {
        var key = languageTag ?? string.Empty;
        if (_engines.TryGetValue(key, out var cached))
        {
            return cached;
        }

        WinOcrEngine? engine = null;
        if (!string.IsNullOrWhiteSpace(languageTag))
        {
            try
            {
                engine = WinOcrEngine.TryCreateFromLanguage(new Language(languageTag));
            }
            catch (ArgumentException)
            {
                // Malformed tag — fall through to the profile-based engine below.
            }
        }

        engine ??= WinOcrEngine.TryCreateFromUserProfileLanguages();

        if (engine is null)
        {
            throw new InvalidOperationException(Loc.Get("Err_NoLanguagePack"));
        }

        _engines[key] = engine;
        return engine;
    }

    /// <summary>
    /// Joins recognized lines, using rules appropriate to the script the recognizer was built for.
    /// </summary>
    /// <remarks>
    /// The tag selects the ruleset, not the spacing of individual boundaries — an earlier version
    /// used it the second way ("Chinese recognizer, so join everything tight") and got mixed
    /// Chinese-and-English lines wrong in both directions. A CJK recognizer still goes through
    /// <see cref="TextLayout"/>'s gap measuring, which handles those mixed lines correctly. What
    /// the tag settles is whether that machinery should run at all: for a Latin recognizer it has
    /// nothing to add and measurably subtracts.
    /// </remarks>
    private static string BuildText(IReadOnlyList<WinOcrLine> lines, string recognizerTag)
    {
        var builder = new StringBuilder();

        // Read once per result, not once per line: the user cannot change it mid-recognition, and
        // a whole result built under two different rules would be incoherent.
        var options = new TextLayoutOptions(
            TrustWordBoundaries: !IsSpacelessScript(recognizerTag),
            RepairNumbers: AppSettings.Current.RepairVersionNumbers);

        foreach (var line in lines)
        {
            if (builder.Length > 0)
            {
                builder.Append('\n');
            }

            var tokens = line.Words
                .Select(word => new OcrToken(
                    word.Text,
                    word.BoundingRect.X,
                    word.BoundingRect.X + word.BoundingRect.Width,
                    word.BoundingRect.Height))
                .ToList();

            builder.Append(TextLayout.JoinLine(tokens, options));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Whether the recognizer's language is written without spaces between words, and so returns
    /// several words where the text has one run. Chinese, Japanese and Thai; Korean is written with
    /// spaces and behaves like the Latin recognizers.
    /// </summary>
    private static bool IsSpacelessScript(string? recognizerTag)
    {
        if (string.IsNullOrEmpty(recognizerTag))
        {
            // Unknown recognizer: keep the gap measuring, which is correct for every script even
            // where it is unnecessary.
            return true;
        }

        var primary = recognizerTag.Split('-')[0];
        return primary.Equals("zh", StringComparison.OrdinalIgnoreCase)
            || primary.Equals("ja", StringComparison.OrdinalIgnoreCase)
            || primary.Equals("th", StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _gate.Wait();
        try
        {
            // WinOcrEngine is not IDisposable; just drop the references.
            _engines.Clear();
        }
        finally
        {
            _gate.Release();
            _gate.Dispose();
        }
    }
}
