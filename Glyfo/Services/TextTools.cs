using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Glyfo.Services;

/// <summary>The kinds of thing in recognized text that are worth a button.</summary>
public enum TextActionKind
{
    Url,
    Email,
    Phone,
}

/// <summary>One such thing, with the exact text it was found as.</summary>
/// <remarks>
/// <paramref name="Value"/> is what was on the page, not a URI: turning it into something
/// launchable is the window's job, because that is where the difference between
/// <c>mailto:</c> and <c>tel:</c> belongs.
/// </remarks>
public sealed record TextAction(TextActionKind Kind, string Value);

/// <summary>The two cleanup passes OCR output almost always needs.</summary>
public static partial class TextTools
{
    /// <summary>
    /// How many actions are offered at most.
    /// </summary>
    /// <remarks>
    /// The row of buttons is half the window wide and scrolls sideways. A contacts page can hold
    /// thirty addresses, and a scroll bar through thirty of them is not a feature — the first few
    /// are the ones anyone came for.
    /// </remarks>
    private const int MaxActions = 6;

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

    /// <summary>
    /// The addresses, mailboxes and phone numbers a piece of recognized text contains.
    /// </summary>
    /// <remarks>
    /// In the order they appear, without repeats, and capped: this feeds a single row of buttons,
    /// and a page of links would push the useful ones off the end of it.
    /// </remarks>
    public static IReadOnlyList<TextAction> FindActions(string text)
    {
        var found = new List<TextAction>();

        if (string.IsNullOrWhiteSpace(text))
        {
            return found;
        }

        var matches = new List<(int Index, TextAction Action)>();
        var urls = new List<(int Start, int End)>();

        foreach (Match m in UrlPattern().Matches(text))
        {
            urls.Add((m.Index, m.Index + m.Length));
            matches.Add((m.Index, new TextAction(TextActionKind.Url, TrimTrailingPunctuation(m.Value))));
        }

        // An address inside a URL is part of the URL, and a run of digits inside one is a
        // parameter, not a number to dial. Both would otherwise be offered a second time on
        // their own, which is never what the user meant by that line of text.
        bool InsideUrl(int index) => urls.Any(u => index >= u.Start && index < u.End);

        foreach (Match m in EmailPattern().Matches(text))
        {
            if (!InsideUrl(m.Index))
            {
                matches.Add((m.Index, new TextAction(TextActionKind.Email, m.Value)));
            }
        }

        foreach (Match m in PhonePattern().Matches(text))
        {
            var candidate = m.Value.Trim();
            if (!InsideUrl(m.Index) && LooksLikePhoneNumber(candidate))
            {
                matches.Add((m.Index, new TextAction(TextActionKind.Phone, candidate)));
            }
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (_, action) in matches.OrderBy(m => m.Index))
        {
            if (action.Value.Length == 0 || !seen.Add($"{action.Kind}:{action.Value}"))
            {
                continue;
            }

            found.Add(action);
            if (found.Count == MaxActions)
            {
                break;
            }
        }

        return found;
    }

    /// <summary>
    /// Whether a run of digits and separators is plausibly a phone number rather than an identifier.
    /// </summary>
    /// <remarks>
    /// This is the whole defence against a page of invoices sprouting phone buttons. A bare run of
    /// digits is an order number, a part number or a date far more often than it is a number anyone
    /// would dial, so something has to say "this was written to be read aloud": either the country
    /// prefix, or the grouping people put in by hand.
    /// </remarks>
    private static bool LooksLikePhoneNumber(string candidate)
    {
        var digits = 0;
        var separators = 0;

        foreach (var c in candidate)
        {
            if (char.IsAsciiDigit(c))
            {
                digits++;
            }
            else if (c is ' ' or '-' or '(' or ')' or '.')
            {
                separators++;
            }
        }

        if (digits is < 7 or > 15)
        {
            return false;
        }

        return candidate.StartsWith('+') || separators > 0;
    }

    /// <summary>
    /// Drops the sentence that a recognized address is usually stuck to.
    /// </summary>
    /// <remarks>
    /// A URL at the end of a sentence comes back with the full stop attached, and a bracketed one
    /// comes back with the closing bracket. Both are the page's punctuation, not the address.
    /// </remarks>
    private static string TrimTrailingPunctuation(string value) =>
        value.TrimEnd('.', ',', ';', ':', '!', '?', ')', ']', '}', '\'', '"', '。', '，', '、', '）', '】', '」');

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

    /// <remarks>
    /// Bare <c>www.</c> counts, because that is how addresses are printed on the posters and
    /// business cards this app is pointed at. The excluded characters are the ones that end an
    /// address in running text rather than belong to it — brackets in both alphabets, and the
    /// quotes that wrap a link in prose.
    /// </remarks>
    [GeneratedRegex(@"(?:https?://|www\.)[^\s<>""'（）【】]+", RegexOptions.IgnoreCase)]
    private static partial Regex UrlPattern();

    [GeneratedRegex(@"[\w.+-]+@[\w-]+(?:\.[\w-]+)+")]
    private static partial Regex EmailPattern();

    /// <remarks>
    /// Deliberately loose, because the shapes phone numbers are written in vary by country more
    /// than any pattern can usefully capture. What keeps it from matching every number on the
    /// page is <see cref="LooksLikePhoneNumber"/>, not this.
    /// </remarks>
    [GeneratedRegex(@"\+?\d[\d \-().]{6,18}\d")]
    private static partial Regex PhonePattern();
}
