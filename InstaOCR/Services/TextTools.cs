using System.Text;
using System.Text.RegularExpressions;

namespace InstaOCR.Services;

/// <summary>The two cleanup passes OCR output almost always needs.</summary>
public static partial class TextTools
{
    /// <summary>
    /// Unwraps hard-wrapped lines into paragraphs. A blank line still separates paragraphs — that
    /// is the structure worth keeping. Between CJK characters no space is inserted.
    /// </summary>
    public static string RemoveLineBreaks(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
        var paragraphs = ParagraphBreak().Split(normalized);
        var builder = new StringBuilder();

        foreach (var paragraph in paragraphs)
        {
            if (builder.Length > 0)
            {
                builder.Append("\n\n");
            }

            var lines = paragraph.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                if (builder.Length > 0 && NeedsSpace(builder[^1], line[0]))
                {
                    builder.Append(' ');
                }

                builder.Append(line);
            }
        }

        return CollapseSpaces().Replace(builder.ToString(), " ").Trim();
    }

    /// <summary>Strips every space and tab. Mainly for CJK text, where OCR sprays spaces between glyphs.</summary>
    public static string RemoveSpaces(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (c is ' ' or '\t' or '　' or ' ')
            {
                continue;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }

    /// <summary>
    /// The language a piece of text is written in, as far as its script alone can tell — "ja",
    /// "ko", "zh", "ru", or null for anything written in the Latin alphabet.
    /// </summary>
    /// <remarks>
    /// Used to point the speech voice at the right language, which is a job the script does answer:
    /// a Japanese voice reading Cyrillic, or an English one reading Han, is unintelligible no matter
    /// which particular language it turned out to be. Latin gets a null rather than a guess, because
    /// there the script genuinely does not distinguish English from French from Spanish, and every
    /// Latin voice at least produces recognizable words.
    ///
    /// Counting rather than first-match: OCR output routinely carries a stray Han glyph in English
    /// text, and one bad character should not redirect the whole thing.
    /// </remarks>
    public static string? DetectScriptLanguage(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        int kana = 0, hangul = 0, han = 0, cyrillic = 0;

        foreach (var c in text)
        {
            if (c is >= '぀' and <= 'ヿ' or >= 'ㇰ' and <= 'ㇿ')
            {
                kana++;
            }
            else if (c is >= '가' and <= '힯' or >= 'ᄀ' and <= 'ᇿ' or >= '㄰' and <= '㆏')
            {
                hangul++;
            }
            else if (c is >= '一' and <= '鿿' or >= '㐀' and <= '䶿')
            {
                han++;
            }
            else if (c is >= 'Ѐ' and <= 'ӿ')
            {
                cyrillic++;
            }
        }

        // Kana are unique to Japanese, and Japanese text is mostly Han: a handful of them settles
        // the whole passage. Hangul likewise outranks the Hanja that may sit beside it.
        if (kana > 2)
        {
            return "ja";
        }

        if (hangul > kana && hangul > 0)
        {
            return "ko";
        }

        if (kana > 0)
        {
            return "ja";
        }

        if (han > 0)
        {
            return "zh";
        }

        return cyrillic > 0 ? "ru" : null;
    }

    /// <summary>A joined-up line break only needs a space when neither side is a CJK glyph.</summary>
    private static bool NeedsSpace(char left, char right)
    {
        if (left is ' ' or '\n')
        {
            return false;
        }

        return !IsCjk(left) && !IsCjk(right);
    }

    private static bool IsCjk(char c) =>
        c is >= '　' and <= '〿'   // CJK punctuation
            or >= '぀' and <= 'ヿ' // kana
            or >= '㐀' and <= '䶿' // CJK ext A
            or >= '一' and <= '鿿' // CJK unified
            or >= '가' and <= '힯' // hangul
            or >= '＀' and <= '￯'; // fullwidth forms

    [GeneratedRegex(@"\n[ \t]*\n[ \t\n]*")]
    private static partial Regex ParagraphBreak();

    [GeneratedRegex(@"[ \t]{2,}")]
    private static partial Regex CollapseSpaces();
}
