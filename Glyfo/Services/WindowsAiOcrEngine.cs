using Microsoft.Graphics.Imaging;
using Microsoft.Windows.AI;
using Microsoft.Windows.AI.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace Glyfo.Services;

/// <summary>
/// Windows AI Foundry text recognition. Only exists on Copilot+ PCs, so every entry point is
/// guarded — <c>TargetPlatformMinVersion</c> is 17763, and on older systems merely touching these
/// types throws at activation time.
/// </summary>
public sealed class WindowsAiOcrEngine : IOcrEngine
{
    private readonly TextRecognizer _recognizer;
    private bool _disposed;

    private WindowsAiOcrEngine(TextRecognizer recognizer) => _recognizer = recognizer;

    public string DisplayName => "Windows AI (Copilot+)";

    /// <summary>
    /// Returns null on any machine that cannot run the recognizer. When the model is present but
    /// not yet downloaded this blocks on <c>EnsureReadyAsync</c>, so call it off the UI thread.
    /// </summary>
    public static async Task<WindowsAiOcrEngine?> TryCreateAsync()
    {
        try
        {
            var state = TextRecognizer.GetReadyState();

            if (state == AIFeatureReadyState.NotReady)
            {
                await TextRecognizer.EnsureReadyAsync();
                state = TextRecognizer.GetReadyState();
            }

            if (state != AIFeatureReadyState.Ready)
            {
                return null;
            }

            return new WindowsAiOcrEngine(await TextRecognizer.CreateAsync());
        }
        catch (Exception)
        {
            // Unsupported OS, missing projection, model download failure — all mean "use the
            // built-in engine instead", and none of them should surface to the user.
            return null;
        }
    }

    /// <summary>The AI recognizer detects the script itself; there is nothing to choose.</summary>
    public IReadOnlyList<OcrLanguage> GetAvailableLanguages() =>
        new[] { new OcrLanguage(string.Empty, "Automatic") };

    /// <summary>
    /// Runs the recognizer, and runs it again on an enlarged copy when the first pass was either
    /// unsure of itself or looking at glyphs too small to read. The better pass wins, and words the
    /// loser read more confidently are folded back into it.
    /// </summary>
    /// <remarks>
    /// This is the payoff of <c>RecognizedWord.MatchConfidence</c>: unlike the built-in engine,
    /// which has to guess from covered area which of two answers is better, here the recognizer
    /// says so directly. The second pass costs roughly one more inference, so it only runs when the
    /// first pass gives a reason to — see <see cref="RetryConfidence"/>.
    /// </remarks>
    public async Task<OcrResult> RecognizeAsync(SoftwareBitmap bitmap, string? languageTag)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(bitmap);

        var first = await RecognizeOnceAsync(bitmap);
        var best = first;
        var challenger = Array.Empty<ScoredWord>();
        var challengerScale = 1.0;

        if (ShouldRetry(first))
        {
            var scale = ImageLoader.SuggestUpscale(
                bitmap.PixelWidth, bitmap.PixelHeight, first.Words.Select(WordHeight).ToList());

            if (scale > 1)
            {
                try
                {
                    using var enlarged = await ImageLoader.ScaleAsync(bitmap, scale);
                    var second = await RecognizeOnceAsync(enlarged);

                    if (second.Score > first.Score)
                    {
                        best = second;
                        challenger = first.Words;
                        challengerScale = 1 / scale;
                    }
                    else
                    {
                        challenger = second.Words;
                        challengerScale = scale;
                    }
                }
                catch (Exception)
                {
                    // A failed second pass must never cost us the first one.
                }
            }
        }

        var lines = ConfidenceMerge.Upgrade(best.Lines, challenger, challengerScale);
        var text = string.Join('\n', lines.Select(BuildLine));

        return new OcrResult(text, ConfidenceMerge.MeanConfidence(best.Words), DisplayName);
    }

    /// <summary>Mean confidence below which the recognizer is unsure enough to be worth a retry.</summary>
    private const float RetryConfidence = 0.80f;

    private static bool ShouldRetry(Pass pass) =>
        pass.Words.Length == 0 || ConfidenceMerge.MeanConfidence(pass.Words) < RetryConfidence;

    /// <summary>One recognition pass, flattened into shapes the arbitration logic can compare.</summary>
    private readonly record struct Pass(IReadOnlyList<IReadOnlyList<ScoredWord>> Lines, ScoredWord[] Words)
    {
        public double Score => ConfidenceMerge.Score(Words);
    }

    private async Task<Pass> RecognizeOnceAsync(SoftwareBitmap bitmap)
    {
        var buffer = ImageBuffer.CreateForSoftwareBitmap(bitmap);
        var recognized = await _recognizer.RecognizeTextFromImageAsync(buffer);

        var lines = (recognized.Lines ?? Array.Empty<RecognizedLine>())
            .Select(ToScoredWords)
            .ToList();

        return new Pass(lines, lines.SelectMany(line => line).ToArray());
    }

    /// <summary>
    /// Flattens a line's words, turning each bounding quad into an axis-aligned box. The quad
    /// carries rotation, which this engine reports separately as <c>TextAngle</c>; for deciding
    /// whether two words touch or refer to the same place, the enclosing box is enough and degrades
    /// gracefully on skewed text.
    /// </summary>
    private static IReadOnlyList<ScoredWord> ToScoredWords(RecognizedLine line)
    {
        var words = line.Words;
        if (words is null || words.Length == 0)
        {
            // No word geometry to work from. Keep the line's own text as a single unpositioned
            // token so it still reaches the output; it simply cannot take part in arbitration.
            return new[] { new ScoredWord(line.Text, 0, 0, 0, 0, 0) };
        }

        return words.Select(ToScoredWord).ToArray();
    }

    private static ScoredWord ToScoredWord(RecognizedWord word)
    {
        var box = word.BoundingBox;

        var left = Math.Min(Math.Min(box.TopLeft.X, box.BottomLeft.X), Math.Min(box.TopRight.X, box.BottomRight.X));
        var right = Math.Max(Math.Max(box.TopLeft.X, box.BottomLeft.X), Math.Max(box.TopRight.X, box.BottomRight.X));
        var top = Math.Min(Math.Min(box.TopLeft.Y, box.TopRight.Y), Math.Min(box.BottomLeft.Y, box.BottomRight.Y));
        var bottom = Math.Max(Math.Max(box.TopLeft.Y, box.TopRight.Y), Math.Max(box.BottomLeft.Y, box.BottomRight.Y));

        return new ScoredWord(word.Text, left, top, right, bottom, word.MatchConfidence);
    }

    private static double WordHeight(ScoredWord word) => word.Bottom - word.Top;

    /// <summary>
    /// Rebuilds a line from its words rather than trusting <c>RecognizedLine.Text</c>, which
    /// space-separates every token and so spreads CJK out into "你 好 世 界". This engine detects
    /// the script by itself and reports no language, so the decision has to come from the glyphs
    /// and the measured gaps between them.
    /// </summary>
    /// <remarks>
    /// Unlike the built-in engine there is no recognizer language to consult — this one detects the
    /// script itself and does not report what it found. So the decision is made per line from the
    /// characters that came back: a line with no CJK in it, and no fullwidth punctuation, cannot
    /// have been split anywhere but at a space.
    /// </remarks>
    private static string BuildLine(IReadOnlyList<ScoredWord> words)
    {
        var options = new TextLayoutOptions(
            TrustWordBoundaries: !words.Any(word => word.Text.Any(TextLayout.IsCjk)),
            RepairNumbers: AppSettings.Current.RepairVersionNumbers);

        return TextLayout.JoinLine(
            words.Select(word => new OcrToken(word.Text, word.Left, word.Right, word.Bottom - word.Top)).ToList(),
            options);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _recognizer.Dispose();
    }
}
