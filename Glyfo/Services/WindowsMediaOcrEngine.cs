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
    /// <remarks>
    /// Raised from three once the candidates were deduplicated by script rather than by tag. On a
    /// machine with Traditional Chinese and Simplified Chinese listed as user languages, three slots
    /// went to Hant, Hans and Latin — two of which are the same writing system — and the installed
    /// Arabic recognizer could never be reached. Four distinct scripts costs one more pass in the
    /// worst case and, with the early exit below, usually fewer than three.
    /// </remarks>
    private const int MaxAutoCandidates = 4;

    /// <summary>
    /// How much of a candidate's score comes from coverage regardless of what script it found.
    /// </summary>
    /// <remarks>
    /// Chosen by replaying tools/ocr-measurements.tsv through the scoring below, for this machine's
    /// candidate set, rather than picked to taste. Scoring on coverage alone — which is what a floor
    /// of 1 means, and what this did before — reaches the right recognizer in 16 of 28 measured
    /// pages, failing every Latin, Cyrillic and Greek one. A floor of 0.5 reaches 27, and is the
    /// lowest value that still rescues Traditional Chinese at 10pt while leaving en-US a 1.68x margin
    /// over zh-Hant-HK on the hardest Latin page. Below 0.25 the agreement term starts overpowering
    /// coverage on pages where coverage was the signal that mattered.
    /// </remarks>
    private const double AgreementFloor = 0.5;

    /// <summary>
    /// Agreement above which the auto mode stops trying further recognizers.
    /// </summary>
    private const double EarlyExitAgreement = 0.90;

    /// <summary>
    /// Mean word length above which a high-agreement result is believed rather than merely noted.
    /// </summary>
    /// <remarks>
    /// The second half of the early exit, and it is not optional. Agreement alone would let a Latin
    /// recognizer exit early on a Chinese page: it returns Latin whatever it is shown, so it scores
    /// 1.0 there too. What gives it away is the shape of what it returns — reading ideographs it
    /// produces a drift of one- and two-character fragments. Measured at 1.31-2.0 characters on Han,
    /// Japanese and Korean pages against 3.47-3.94 on real Latin ones.
    ///
    /// Which is also why Latin is excluded from the early exit entirely (see
    /// <see cref="CanExitEarly"/>): those two ranges do not separate. A Latin recognizer on a Latin
    /// page and a Latin recognizer on a Han page both fall inside 3.0-4.12, so no threshold can
    /// distinguish them and Latin has to earn its win by running the other candidates.
    /// </remarks>
    private const double MinTokenLength = 2.5;

    /// <summary>
    /// Characters of the recognizer's own script that have to come back before a high agreement is
    /// treated as evidence of anything.
    /// </summary>
    /// <remarks>
    /// The third half of the early exit, added because the first two let the search end on the wrong
    /// recognizer. <see cref="ScriptProfiles.Agreement"/> scores text with no script-bearing
    /// characters at 1.0 — deliberately, because bare numbers give nobody grounds to disagree — but
    /// that is exactly the answer a recognizer gives when it cannot read the page at all. On an
    /// Arabic page zh-Hant-TW returns "08-09-2026 1.6.5": agreement 1.0 by vacuity, mean token
    /// length 3.4, and with only those two tests it ended the search as the first candidate, before
    /// ar-SA was ever tried.
    ///
    /// Counting the recognizer's own script instead separates the two cases cleanly. Measured over
    /// the four probe pages against all seven installed recognizers: a recognizer reading its own
    /// script returns 25-94 such characters, while every vacuous or mistaken result returns 0-6.
    /// Twelve is twice the largest wrong answer and a third of the smallest right one.
    /// </remarks>
    private const int MinScriptEvidence = 12;

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
    /// languages first, then English as a Latin fallback, then whatever other writing systems are
    /// installed. One recognizer per writing system.
    /// </summary>
    /// <remarks>
    /// Deduplicating by script rather than by language tag is the point. Tag deduplication only
    /// catches the case where two tags name the same recognizer ("zh-CN" and "zh-Hans"); it does not
    /// catch two different recognizers for one writing system, and those are exactly what a user's
    /// language list tends to be full of. Someone listing Traditional and Simplified Chinese spent
    /// two of three slots on Han, so a page in any third script had no candidate that could read it
    /// — including, on this machine, an Arabic recognizer that was installed and never once tried.
    ///
    /// The third pass is new as well. Running out of user languages used to end the search, which
    /// left installed recognizers unused on exactly the pages they exist for.
    /// </remarks>
    public static IReadOnlyList<OcrLanguage> GetAutoCandidates()
    {
        List<Language> installed;
        try
        {
            installed = WinOcrEngine.AvailableRecognizerLanguages.ToList();
        }
        catch (Exception)
        {
            return Array.Empty<OcrLanguage>();
        }

        var candidates = new List<OcrLanguage>();
        var claimed = new HashSet<WritingScript>();

        void TryAdd(string? tag)
        {
            if (tag is null || candidates.Count >= MaxAutoCandidates)
            {
                return;
            }

            var language = installed.FirstOrDefault(
                l => string.Equals(l.LanguageTag, tag, StringComparison.OrdinalIgnoreCase));

            // A script already spoken for is not worth a second pass: two recognizers for one
            // writing system disagree about characters, not about whether they can read the page,
            // and the auto mode is only choosing between pages it can and cannot read.
            if (language is not null && claimed.Add(ScriptOf(language)))
            {
                candidates.Add(new OcrLanguage(language.LanguageTag, language.NativeName));
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

        // Whatever writing systems are left. These rank below the user's own languages because a
        // recognizer they never asked for is a worse first guess than one they did — but an
        // installed recognizer is still evidence that its script turns up on this machine.
        foreach (var language in installed)
        {
            TryAdd(language.LanguageTag);
        }

        return candidates;
    }

    private static WritingScript ScriptOf(Language language) =>
        ScriptProfiles.ForScriptCode(language.Script, rightToLeft: false).Script;

    public IReadOnlyList<OcrLanguage> GetAvailableLanguages()
    {
        try
        {
            // NativeName, not DisplayName: DisplayName renders every entry in whatever language
            // Windows itself is set to, so someone running a Chinese Windows and switching the
            // interface to English got a recognition list reading "英文 (美國)". A language is
            // named in itself here, the way Windows' own language pickers do it, which also means
            // the list reads the same whatever the interface language is.
            return WinOcrEngine.AvailableRecognizerLanguages
                .Select(language => new OcrLanguage(language.LanguageTag, language.NativeName))
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
        // and an unavailable tag silently falls back to the profile engine. Named the same way the
        // language picker names it, and for the same reason (see GetAvailableLanguages): in its own
        // language, so the status bar reads the same whatever the interface is set to.
        return new OcrResult(
            BuildText(result.Lines, ProfileFor(engine)),
            null,
            DisplayName,
            engine.RecognizerLanguage.NativeName);
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
    /// Runs the candidate recognizers and keeps the one whose answer looks most like the page.
    /// </summary>
    /// <remarks>
    /// Windows.Media.Ocr reports no confidence, so the pick has to be inferred. Covered word area is
    /// the obvious signal — a recognizer pointed at the wrong script does not merely misread the
    /// text, it fails to find whole regions of it — but on its own it is not enough, and it fails in
    /// a specific direction. Han glyphs fill a square em box while Latin boxes cling to the ink, so
    /// area systematically favours the Chinese recognizers whatever is on the page. Measured on an
    /// English page at 16pt: en-US covers 0.1803 of the image, zh-Hant-TW covers 0.1768. At 12pt
    /// zh-Hant-HK covers <em>more</em> than en-US does. That is the bug this addresses, and it was
    /// visible as auto mode handing back the Chinese recognizer's reading of an English page.
    ///
    /// So coverage is weighted by how much of what came back is in the recognizer's own script (see
    /// <see cref="ScriptProfiles.Agreement"/>), which is a signal precisely where coverage is blind.
    /// Replayed over tools/ocr-measurements.tsv the pick goes from 16 of 28 pages right to 27.
    ///
    /// Costs one pass per candidate, which is why it only runs for the auto entry — and why it stops
    /// as soon as a candidate is convincing enough that the rest cannot matter.
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
            var profile = ProfileFor(engine);
            var text = RawText(result);
            var agreement = ScriptProfiles.Agreement(text, profile);
            var score = ScoreCoverage(result, bitmap) *
                        (AgreementFloor + ((1 - AgreementFloor) * agreement));

            if (bestEngine is null || score > bestScore * AutoWinMargin)
            {
                bestEngine = engine;
                bestResult = result;
                bestScore = score;

                // A recognizer that found enough of its own script, and nearly nothing else, in
                // words of a plausible length, has already answered the only question the remaining
                // passes could. Stopping here is what turns the usual Chinese or Arabic page from
                // three recognitions into one.
                if (CanExitEarly(profile) &&
                    agreement >= EarlyExitAgreement &&
                    ScriptProfiles.ScriptEvidence(text, profile) >= MinScriptEvidence &&
                    MeanTokenLength(result) >= MinTokenLength)
                {
                    break;
                }
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

    /// <summary>
    /// Whether a convincing-looking result from this script can be trusted without running the rest.
    /// </summary>
    /// <remarks>
    /// Everything but Latin. A Latin recognizer reports Latin no matter what it is shown, so its
    /// agreement is always 1 and cannot vouch for anything; the token-length test is what normally
    /// catches that, and for Latin it does not separate — 3.47-3.94 characters on real Latin pages
    /// against 3.0-4.12 on Han, Japanese and Korean ones. With neither test able to speak, Latin
    /// runs the other candidates and wins on the comparison instead.
    /// </remarks>
    private static bool CanExitEarly(ScriptProfile profile) =>
        profile.Script != WritingScript.Latin && profile.Script != WritingScript.Unknown;

    /// <summary>Covered word area as a fraction of the image, so the figure compares across images.</summary>
    private static double ScoreCoverage(WinOcrResult result, SoftwareBitmap bitmap)
    {
        var area = (double)bitmap.PixelWidth * bitmap.PixelHeight;
        if (area <= 0)
        {
            return 0;
        }

        return result.Lines
            .SelectMany(line => line.Words)
            .Sum(word => word.BoundingRect.Width * word.BoundingRect.Height) / area;
    }

    /// <summary>The recognized words run together, for judging the text rather than laying it out.</summary>
    private static string RawText(WinOcrResult result)
    {
        var builder = new StringBuilder();
        foreach (var line in result.Lines)
        {
            foreach (var word in line.Words)
            {
                builder.Append(word.Text);
            }
        }

        return builder.ToString();
    }

    private static double MeanTokenLength(WinOcrResult result)
    {
        var words = result.Lines.SelectMany(line => line.Words).ToList();
        return words.Count == 0 ? 0 : words.Average(word => (double)word.Text.Length);
    }

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
        var profile = ProfileFor(engine);
        var scale = SuggestRescale(bitmap, first, profile);
        if (scale <= 1)
        {
            return first;
        }

        try
        {
            using var enlarged = await ImageLoader.ScaleAsync(bitmap, scale);
            var second = await engine.RecognizeAsync(enlarged);

            // Windows OCR exposes no confidence, so "found more" is the only signal available —
            // but what "more" means depends on the writing system. Where words are separated by
            // spaces, the recognizer is reporting boundaries that are really there and counting
            // them is meaningful. Where they are not, the split points are the recognizer's own
            // invention: an enlarged pass can carve one run of Chinese into more pieces without
            // having read a single extra character, and counting words would call that an
            // improvement. Characters are what actually got read.
            //
            // Ties still go to the enlarged pass, which has more detail to work from and in
            // practice gets the characters within a word right more often.
            var better = profile.SpacelessWords
                ? CharCount(second) >= CharCount(first)
                : WordCount(second) >= WordCount(first);

            return better ? second : first;
        }
        catch (Exception)
        {
            return first;
        }
    }

    private static double SuggestRescale(SoftwareBitmap bitmap, WinOcrResult result, ScriptProfile profile) =>
        ImageLoader.SuggestUpscale(
            bitmap.PixelWidth,
            bitmap.PixelHeight,
            result.Lines.SelectMany(line => line.Words).Select(word => word.BoundingRect.Height).ToList(),
            profile.RescaleThreshold,
            profile.TargetWordHeight);

    private static int WordCount(WinOcrResult result) => result.Lines.Sum(line => line.Words.Count);

    private static int CharCount(WinOcrResult result) =>
        result.Lines.Sum(line => line.Words.Sum(word => word.Text.Length));

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
    /// The profile selects the ruleset, not the spacing of individual boundaries — an earlier
    /// version used it the second way ("Chinese recognizer, so join everything tight") and got mixed
    /// Chinese-and-English lines wrong in both directions. A CJK recognizer still goes through
    /// <see cref="TextLayout"/>'s gap measuring, which handles those mixed lines correctly. What the
    /// profile settles is whether that machinery should run at all: for a Latin recognizer it has
    /// nothing to add and measurably subtracts.
    /// </remarks>
    private static string BuildText(IReadOnlyList<WinOcrLine> lines, ScriptProfile profile)
    {
        var builder = new StringBuilder();

        // Read once per result, not once per line: the user cannot change it mid-recognition, and
        // a whole result built under two different rules would be incoherent.
        var options = profile.ToLayoutOptions(AppSettings.Current.RepairVersionNumbers);

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
    /// The profile for whichever recognizer actually ran.
    /// </summary>
    /// <remarks>
    /// Windows answers both halves of this itself. Splitting the language tag at its first hyphen,
    /// which is what this replaced, could not tell "sr-Cyrl" from "sr-Latn" and carried a branch for
    /// Thai that no installable recognizer can ever reach.
    /// </remarks>
    private static ScriptProfile ProfileFor(WinOcrEngine engine)
    {
        var language = engine.RecognizerLanguage;
        return ScriptProfiles.ForScriptCode(
            language.Script,
            language.LayoutDirection == LanguageLayoutDirection.Rtl);
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
