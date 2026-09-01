using System;
using System.Collections.Generic;
using System.Text;

namespace Glyfo.Services;

/// <summary>A recognized word plus the geometry needed to tell a real space from a tight boundary.</summary>
internal readonly record struct OcrToken(string Text, double Left, double Right, double Height);

/// <summary>How much of the recognizer's output <see cref="TextLayout"/> is allowed to second-guess.</summary>
/// <param name="TrustWordBoundaries">
/// True when the recognizer splits at word boundaries, which every Latin-script recognizer does.
/// Then each boundary between two words simply is a space, and none of the gap measuring or
/// punctuation binding below applies — those exist purely to undo CJK recognizer artifacts.
/// </param>
/// <param name="RepairNumbers">
/// Whether to rewrite letters that are really digits inside a number. User-switchable, because it
/// is the one step that substitutes a character the recognizer actually identified.
/// </param>
internal readonly record struct TextLayoutOptions(bool TrustWordBoundaries, bool RepairNumbers);

/// <summary>
/// Rebuilds a line of text from the words a recognizer produced.
/// </summary>
/// <remarks>
/// Neither engine tells us where the spaces were. A line's own <c>Text</c> property puts one
/// between every word, which spreads CJK out into "你 好 世 界"; joining everything tight instead
/// turns "使用 Windows OCR 识别" into "使用WindowsOCR识别". Both are guesses made without looking
/// at the image.
///
/// The word bounding boxes settle it. Measured on rendered CJK samples at 30px: gaps inside a token
/// ("v1" · "." · "6") run 3-8px against a ~29px line height, while real spaces run 10-16px. A
/// threshold at 30% of the line height separates the two cleanly.
///
/// None of which applies to a Latin-script recognizer, and applying it anyway made English worse
/// than a naive join. Measured on rendered English samples, mean CER over 16 cases: 19.45% with the
/// gap rule against 17.20% for joining the recognizer's own words with spaces. Two reasons. The
/// boxes are tight around the ink, so "line height" swings by 2.6x within one line of English
/// ("over" measures 8px where "jumps" measures 21px), and real word gaps are only 4-9px at 22px
/// type — below the threshold, so "fox jumps" came back as "foxjumps". And there is nothing to
/// decide in the first place: a Latin recognizer already splits at word boundaries, so every
/// boundary it reports is a space. Hence <see cref="TextLayoutOptions.TrustWordBoundaries"/>.
/// </remarks>
internal static class TextLayout
{
    /// <summary>Gap, as a fraction of line height, above which a boundary counts as a space.</summary>
    private const double SpaceGapRatio = 0.30;

    public static string JoinLine(IReadOnlyList<OcrToken> tokens, TextLayoutOptions options)
    {
        var lineHeight = 0.0;
        foreach (var token in tokens)
        {
            lineHeight = Math.Max(lineHeight, token.Height);
        }

        var builder = new StringBuilder();
        var previous = default(OcrToken);
        var havePrevious = false;

        foreach (var token in tokens)
        {
            if (token.Text.Length == 0)
            {
                continue;
            }

            if (havePrevious && NeedsSpace(previous, token, lineHeight, options.TrustWordBoundaries))
            {
                builder.Append(' ');
            }

            builder.Append(token.Text);
            previous = token;
            havePrevious = true;
        }

        return Repair(builder.ToString(), options);
    }

    private static bool NeedsSpace(OcrToken previous, OcrToken next, double lineHeight, bool trustWordBoundaries)
    {
        var left = previous.Text[^1];
        var right = next.Text[0];

        // The recognizer split these two apart, and a Latin-script recognizer only does that at a
        // space. Guessing again from the geometry can only lose information it already has.
        if (trustWordBoundaries)
        {
            return true;
        }

        // CJK is set without inter-word spaces, so a gap between two ideographs is layout.
        // Checked before the geometry so that generously letter-spaced headings do not come back
        // with a space between every character.
        if (IsCjk(left) && IsCjk(right))
        {
            return false;
        }

        // Punctuation binds to the token beside it regardless of how the boxes measure — the ink
        // of a period sits low and narrow, which inflates the apparent gap in front of it.
        if (IsClosingPunctuation(right) || IsOpeningPunctuation(left))
        {
            return false;
        }

        // No usable geometry (a recognizer that reports zero-height boxes): fall back to spacing.
        if (lineHeight <= 0)
        {
            return true;
        }

        return next.Left - previous.Right >= SpaceGapRatio * lineHeight;
    }

    /// <summary>
    /// Repairs the two systematic misreads that the CJK recognizers make on embedded Latin.
    /// </summary>
    /// <remarks>
    /// Both are heuristics that change recognized content, so both are deliberately narrow — they
    /// only fire inside a run that is already alphanumeric, never on prose. Only the second one is
    /// user-switchable: swapping a fullwidth period for a halfwidth one between two ASCII
    /// alphanumerics changes the form of a character the recognizer already identified, whereas
    /// reading "l" as "1" substitutes a different character entirely.
    /// </remarks>
    private static string Repair(string text, TextLayoutOptions options)
    {
        if (text.Length == 0)
        {
            return text;
        }

        var chars = text.ToCharArray();

        // A Latin recognizer has no fullwidth forms to emit, so running the normalizer over its
        // output could only do harm — it would rewrite a legitimate em dash in "pre—post" as a
        // hyphen.
        if (!options.TrustWordBoundaries)
        {
            NormalizePunctuation(chars);
        }

        if (options.RepairNumbers)
        {
            RepairDigitRuns(chars);
        }

        return new string(chars);
    }

    /// <summary>
    /// A Chinese recognizer reading "v1.6.5" returns the separators as fullwidth "．" or "·",
    /// because that is the period it was trained on. Between two ASCII alphanumerics the
    /// halfwidth form is the only one that can be correct.
    /// </summary>
    private static void NormalizePunctuation(char[] chars)
    {
        for (var i = 1; i < chars.Length - 1; i++)
        {
            var replacement = chars[i] switch
            {
                '．' or '。' or '·' or '・' => '.',
                '，' or '、' => ',',
                '：' => ':',
                '；' => ';',
                '－' or '—' => '-',
                '／' => '/',
                _ => '\0'
            };

            if (replacement != '\0' && IsAsciiAlphanumeric(chars[i - 1]) && IsAsciiAlphanumeric(chars[i + 1]))
            {
                chars[i] = replacement;
            }
        }
    }

    /// <summary>
    /// Rewrites letters that are really digits, inside runs that are already numbers.
    /// </summary>
    /// <remarks>
    /// Two shapes, both measured rather than guessed:
    ///
    /// The digit 1 comes back as "l" or "I" in every font and at every size tested — "v1.6.5" reads
    /// as "vl.6.5". That one fires when the letter sits next to a separator that has a digit on its
    /// other side, which is the shape of a version, date or dotted number and nothing else. It
    /// deliberately leaves "html5" and "Model3" alone (no separator) and "IPv6" and "COVID-19"
    /// alone (letter neighbours).
    ///
    /// The English recognizer additionally confuses O with 0 and vice versa, which corrupts order
    /// numbers, years and prices. That one only fires for a single character with a digit on both
    /// sides — "2O26" becomes "2026". A run of them ("1OO" for "100") is left alone: resolving it
    /// means deciding how far the number extends, and getting that wrong silently corrupts data.
    /// The narrow rule leaves "O2", "H2O" and "3D" untouched, which a greedier one would not.
    /// </remarks>
    private static void RepairDigitRuns(char[] chars)
    {
        var start = 0;
        while (start < chars.Length)
        {
            if (!IsRunCharacter(chars[start]))
            {
                start++;
                continue;
            }

            var end = start;
            var hasDigit = false;
            while (end < chars.Length && IsRunCharacter(chars[end]))
            {
                hasDigit |= char.IsAsciiDigit(chars[end]);
                end++;
            }

            if (hasDigit)
            {
                for (var i = start; i < end; i++)
                {
                    // Between two digits, any of these is a misread digit.
                    if (chars[i] is 'l' or 'I' or 'O' or 'o' &&
                        i - 1 >= start && char.IsAsciiDigit(chars[i - 1]) &&
                        i + 1 < end && char.IsAsciiDigit(chars[i + 1]))
                    {
                        chars[i] = chars[i] is 'O' or 'o' ? '0' : '1';
                        continue;
                    }

                    if (chars[i] is not ('l' or 'I'))
                    {
                        continue;
                    }

                    var beforeIsNumeric = i - 2 >= start && IsSeparator(chars[i - 1]) && char.IsAsciiDigit(chars[i - 2]);
                    var afterIsNumeric = i + 2 < end && IsSeparator(chars[i + 1]) && char.IsAsciiDigit(chars[i + 2]);

                    if (beforeIsNumeric || afterIsNumeric)
                    {
                        chars[i] = '1';
                    }
                }
            }

            start = end;
        }
    }

    private static bool IsRunCharacter(char c) => char.IsAsciiLetterOrDigit(c) || IsSeparator(c);

    private static bool IsSeparator(char c) => c is '.' or '-' or '_';

    private static bool IsAsciiAlphanumeric(char c) => char.IsAsciiLetterOrDigit(c);

    private static bool IsClosingPunctuation(char c) =>
        c is '.' or ',' or ';' or ':' or '!' or '?' or '%' or ')' or ']' or '}'
          or '．' or '，' or '。' or '、' or '；' or '：' or '！' or '？' or '）' or '】' or '｝' or '·';

    private static bool IsOpeningPunctuation(char c) =>
        c is '(' or '[' or '{' or '（' or '【' or '｛';

    /// <summary>
    /// Han, kana, CJK punctuation and the fullwidth forms — the ranges where a rendered gap
    /// between two glyphs is layout, not a word separator.
    /// </summary>
    public static bool IsCjk(char c) =>
        (c >= '⺀' && c <= '〿') ||   // CJK radicals, symbols and punctuation
        (c >= '぀' && c <= 'ヿ') ||   // hiragana + katakana
        (c >= '㐀' && c <= '䶿') ||   // unified ideographs extension A
        (c >= '一' && c <= '鿿') ||   // unified ideographs
        (c >= '豈' && c <= '﫿') ||   // compatibility ideographs
        (c >= '＀' && c <= '｠') ||   // fullwidth forms
        (c >= '￠' && c <= '￦');
}
