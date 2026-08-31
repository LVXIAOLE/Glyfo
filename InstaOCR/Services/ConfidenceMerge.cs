using System;
using System.Collections.Generic;
using System.Linq;

namespace InstaOCR.Services;

/// <summary>A recognized word with its position and how sure the recognizer was about it.</summary>
/// <remarks>
/// Deliberately free of WinRT types. The arbitration rules below are the part most likely to be
/// wrong, and this machine has no NPU to exercise them on, so they have to be checkable without one.
/// </remarks>
internal readonly record struct ScoredWord(
    string Text, double Left, double Top, double Right, double Bottom, float Confidence);

/// <summary>
/// Picks between two recognition passes of the same image using the confidence the recognizer
/// reported for each word.
/// </summary>
/// <remarks>
/// Only the Copilot+ recognizer can use this — <c>Windows.Media.Ocr</c> reports no confidence at
/// all, which is why <see cref="WindowsMediaOcrEngine"/> is stuck comparing covered word area and
/// word counts instead. Given real confidences there are two things worth doing, and this class
/// does both:
///
/// 1. <see cref="Score"/> decides which whole pass to keep.
/// 2. <see cref="Upgrade"/> then repairs individual words in the winner from the loser, which is
///    strictly better than either pass on its own: an upscaled pass usually reads small glyphs
///    better while occasionally splitting a line the original got right.
/// </remarks>
internal static class ConfidenceMerge
{
    /// <summary>Fraction of overlap at which two boxes are taken to be the same word.</summary>
    private const double SameWordOverlap = 0.5;

    /// <summary>How much more confident the challenger must be before its reading replaces the winner's.</summary>
    private const float UpgradeMargin = 0.15f;

    /// <summary>Above this the winner's reading is left alone, however confident the challenger is.</summary>
    private const float SettledConfidence = 0.95f;

    /// <summary>
    /// How good a pass is, as the expected number of correctly read words.
    /// </summary>
    /// <remarks>
    /// The sum, emphatically not the mean. Mean confidence rewards giving up: a pass that finds two
    /// words at 0.99 would beat one that finds forty at 0.85, and the two-word answer is useless.
    /// Summing makes the score rise with both how much was found and how sure the recognizer was,
    /// which is the trade-off actually being made.
    /// </remarks>
    public static double Score(IEnumerable<ScoredWord> words) =>
        words.Sum(word => (double)word.Confidence);

    /// <summary>Mean confidence over all words, for display. Null when nothing was recognized.</summary>
    public static float? MeanConfidence(IReadOnlyList<ScoredWord> words) =>
        words.Count == 0 ? null : words.Average(word => word.Confidence);

    /// <summary>
    /// Replaces low-confidence words in <paramref name="winner"/> with better-read equivalents from
    /// <paramref name="challenger"/>, matched by position.
    /// </summary>
    /// <param name="winner">Lines of the pass that scored highest; its structure is preserved.</param>
    /// <param name="challenger">Words from the other pass, in that pass's own pixel space.</param>
    /// <param name="challengerScale">
    /// Size of the challenger's image relative to the winner's, used to bring the boxes into a
    /// common space. 2.0 when the challenger was recognized at double size.
    /// </param>
    /// <remarks>
    /// Only word text is taken across; line breaks and word order stay the winner's. Merging the
    /// two line structures would risk producing text neither pass ever produced, which is a far
    /// worse failure than leaving one word misread.
    /// </remarks>
    public static IReadOnlyList<IReadOnlyList<ScoredWord>> Upgrade(
        IReadOnlyList<IReadOnlyList<ScoredWord>> winner,
        IReadOnlyList<ScoredWord> challenger,
        double challengerScale)
    {
        if (challenger.Count == 0 || challengerScale <= 0)
        {
            return winner;
        }

        var normalized = challengerScale == 1
            ? challenger
            : challenger.Select(word => Deflate(word, challengerScale)).ToList();

        var result = new List<IReadOnlyList<ScoredWord>>(winner.Count);

        foreach (var line in winner)
        {
            var upgraded = new List<ScoredWord>(line.Count);

            foreach (var word in line)
            {
                upgraded.Add(word.Confidence >= SettledConfidence ? word : BestReading(word, normalized));
            }

            result.Add(upgraded);
        }

        return result;
    }

    private static ScoredWord BestReading(ScoredWord word, IReadOnlyList<ScoredWord> candidates)
    {
        var best = word;

        foreach (var candidate in candidates)
        {
            if (candidate.Confidence <= best.Confidence + UpgradeMargin)
            {
                continue;
            }

            if (string.Equals(candidate.Text, word.Text, StringComparison.Ordinal))
            {
                continue;
            }

            if (Overlap(word, candidate) >= SameWordOverlap)
            {
                // Keep the winner's geometry: TextLayout measures gaps against neighbouring words
                // from the same pass, and a box from a differently-scaled pass would not line up.
                best = word with { Text = candidate.Text, Confidence = candidate.Confidence };
            }
        }

        return best;
    }

    private static ScoredWord Deflate(ScoredWord word, double scale) => word with
    {
        Left = word.Left / scale,
        Top = word.Top / scale,
        Right = word.Right / scale,
        Bottom = word.Bottom / scale
    };

    /// <summary>Intersection over union of two boxes, 0 when they do not touch.</summary>
    private static double Overlap(ScoredWord a, ScoredWord b)
    {
        var width = Math.Min(a.Right, b.Right) - Math.Max(a.Left, b.Left);
        var height = Math.Min(a.Bottom, b.Bottom) - Math.Max(a.Top, b.Top);

        if (width <= 0 || height <= 0)
        {
            return 0;
        }

        var intersection = width * height;
        var union = Area(a) + Area(b) - intersection;

        return union <= 0 ? 0 : intersection / union;
    }

    private static double Area(ScoredWord word) =>
        Math.Max(0, word.Right - word.Left) * Math.Max(0, word.Bottom - word.Top);
}
