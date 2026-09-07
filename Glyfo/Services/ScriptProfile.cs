using System;

namespace Glyfo.Services;

/// <summary>The writing systems the recognizers can be pointed at, plus one for "no idea".</summary>
internal enum WritingScript
{
    Latin,
    Cyrillic,
    Greek,
    Arabic,
    Hebrew,
    Han,
    Japanese,
    Korean,
    Thai,
    Unknown
}

/// <summary>
/// Everything the recognition pipeline needs to know about one writing system.
/// </summary>
/// <param name="Script">Which writing system this describes.</param>
/// <param name="SpacelessWords">
/// True when the script is set without spaces between words, so the recognizer splits a single run
/// of text into several "words" and the gaps have to be measured to tell layout from a real space.
/// False for the scripts that are written with spaces, where every boundary the recognizer reports
/// simply is one.
/// </param>
/// <param name="NormalizeFullwidth">
/// True for the scripts whose recognizers have fullwidth punctuation in their repertoire, and so can
/// return "v1．6．5" for "v1.6.5". Deliberately separate from <paramref name="SpacelessWords"/>:
/// Korean is written with spaces yet emits fullwidth forms, so one flag could not serve both.
/// </param>
/// <param name="RightToLeft">Which direction words run in, for the gap arithmetic.</param>
/// <param name="RescaleThreshold">Word height below which a second, enlarged pass pays for itself.</param>
/// <param name="TargetWordHeight">Word height the enlarged pass aims for.</param>
/// <param name="SpaceGapRatio">Gap, as a fraction of line height, above which a boundary is a space.</param>
/// <param name="RetryConfidence">Mean confidence below which the AI engine retries.</param>
internal sealed record ScriptProfile(
    WritingScript Script,
    bool SpacelessWords,
    bool NormalizeFullwidth,
    bool RightToLeft,
    double RescaleThreshold,
    double TargetWordHeight,
    double SpaceGapRatio,
    float RetryConfidence)
{
    /// <summary>The layout rules this script implies, plus the one setting the user controls.</summary>
    public TextLayoutOptions ToLayoutOptions(bool repairNumbers) => new(
        TrustWordBoundaries: !SpacelessWords,
        RepairNumbers: repairNumbers,
        NormalizeFullwidth: NormalizeFullwidth,
        RightToLeft: RightToLeft,
        SpaceGapRatio: SpaceGapRatio);
}

/// <summary>
/// The per-script settings, and the two ways of arriving at one: from what the recognizer says it
/// is, or from what came back out of it.
/// </summary>
/// <remarks>
/// Written without any WinRT types, for the same reason as <see cref="ConfidenceMerge"/>: these
/// rules are the part most likely to be wrong, and a rule that can only be exercised on a machine
/// with the right language packs installed is a rule nobody checks.
///
/// The scripts themselves come from Windows rather than from a hand-kept table of language tags.
/// <c>Windows.Globalization.Language</c> exposes <c>Script</c> as an ISO 15924 code and
/// <c>LayoutDirection</c> alongside it, both since Windows 10 1709 — far below this app's 19041
/// floor. Splitting a BCP-47 tag at its first hyphen, which is what this replaced, cannot tell
/// "sr-Cyrl" from "sr-Latn" and had a branch for Thai that no installable recognizer can reach.
/// </remarks>
internal static class ScriptProfiles
{
    // Every profile below carries the same four numbers, which are the ones the pipeline used
    // globally before there were profiles at all. They are placeholders with a provenance: 20 and 28
    // were measured on English and Chinese screenshots, 0.30 on rendered CJK at 30px, and 0.80 was
    // never measured on anything (no NPU here). tools/Measure-Ocr.ps1 exists to replace them
    // per script; until it has run, "per script" is a shape and not yet a claim.
    private const double DefaultRescaleThreshold = 20;
    private const double DefaultTargetWordHeight = 28;
    private const double DefaultSpaceGapRatio = 0.30;
    private const float DefaultRetryConfidence = 0.80f;

    private static readonly ScriptProfile[] Table =
    {
        Make(WritingScript.Latin, spaceless: false, fullwidth: false, rtl: false),
        Make(WritingScript.Cyrillic, spaceless: false, fullwidth: false, rtl: false),
        Make(WritingScript.Greek, spaceless: false, fullwidth: false, rtl: false),
        Make(WritingScript.Arabic, spaceless: false, fullwidth: false, rtl: true),
        Make(WritingScript.Hebrew, spaceless: false, fullwidth: false, rtl: true),
        Make(WritingScript.Han, spaceless: true, fullwidth: true, rtl: false),
        Make(WritingScript.Japanese, spaceless: true, fullwidth: true, rtl: false),

        // Korean is written with spaces, so its boundaries are trustworthy — but its recognizer
        // still returns fullwidth punctuation, which until these two flags were separated it had no
        // way of saying. Left as it was for now so that introducing profiles changes no output;
        // turning fullwidth on here is a behaviour change and lands with the measured numbers.
        Make(WritingScript.Korean, spaceless: false, fullwidth: false, rtl: false),

        Make(WritingScript.Thai, spaceless: true, fullwidth: false, rtl: false),

        // A script nobody anticipated keeps the gap measuring and the normalizer, which is correct
        // everywhere and merely unnecessary in places. Falling the other way would silently join
        // words together.
        Make(WritingScript.Unknown, spaceless: true, fullwidth: true, rtl: false)
    };

    private static ScriptProfile Make(WritingScript script, bool spaceless, bool fullwidth, bool rtl) =>
        new(script, spaceless, fullwidth, rtl,
            DefaultRescaleThreshold, DefaultTargetWordHeight, DefaultSpaceGapRatio, DefaultRetryConfidence);

    public static ScriptProfile ForScript(WritingScript script) => Table[(int)script];

    /// <summary>
    /// The profile for a recognizer, given the ISO 15924 code and layout direction Windows reports
    /// for it.
    /// </summary>
    /// <remarks>
    /// The direction is taken from the caller rather than from the table because Windows knows it
    /// for scripts this table does not, and an unrecognized right-to-left script should still have
    /// its gaps measured the right way round.
    /// </remarks>
    public static ScriptProfile ForScriptCode(string? iso15924, bool rightToLeft)
    {
        var profile = ForScript(FromScriptCode(iso15924));
        return profile.RightToLeft == rightToLeft ? profile : profile with { RightToLeft = rightToLeft };
    }

    private static WritingScript FromScriptCode(string? iso15924) => iso15924 switch
    {
        "Latn" => WritingScript.Latin,
        "Cyrl" => WritingScript.Cyrillic,
        "Grek" => WritingScript.Greek,
        "Arab" => WritingScript.Arabic,
        "Hebr" => WritingScript.Hebrew,

        // Hans and Hant differ in which characters they use, not in how they are set, so they share
        // a profile. Hani is the script-neutral code and turns up on a few tags.
        "Hans" or "Hant" or "Hani" => WritingScript.Han,

        // Japanese is written in three scripts at once; whichever code a tag carries, the text will
        // be a mixture and has to be treated as Japanese.
        "Jpan" or "Hira" or "Kana" or "Hrkt" => WritingScript.Japanese,

        "Kore" or "Hang" => WritingScript.Korean,
        "Thai" => WritingScript.Thai,
        _ => WritingScript.Unknown
    };

    /// <summary>
    /// The profile for a piece of recognized text, by counting which script most of it belongs to.
    /// </summary>
    /// <remarks>
    /// For the Copilot+ recognizer, which detects the script itself and then does not say what it
    /// found. This is also the only route by which Thai and Hebrew reach a profile at all: neither
    /// is an installable Windows OCR recognizer, so the built-in engine can never ask for them.
    ///
    /// Latin is the tie-break rather than Unknown. Digits, spaces and ASCII punctuation carry no
    /// script, and a line that is nothing but those is far more likely to have come off a Latin page
    /// than off one in a script the table has never heard of.
    /// </remarks>
    public static ScriptProfile ForText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return ForScript(WritingScript.Latin);
        }

        var counts = new int[Table.Length];
        var kana = 0;

        foreach (var c in text)
        {
            var script = ScriptOf(c);
            if (script is null)
            {
                continue;
            }

            counts[(int)script.Value]++;

            if (script == WritingScript.Japanese)
            {
                kana++;
            }
        }

        // Kana is decisive rather than merely counted. Japanese prose is mostly Han characters with
        // kana threaded through it, so counting scripts would file every ordinary Japanese page as
        // Chinese; but no Chinese text contains kana at all, so a single one settles it.
        if (kana > 0)
        {
            return ForScript(WritingScript.Japanese);
        }

        var best = WritingScript.Latin;
        var bestCount = 0;
        for (var i = 0; i < counts.Length; i++)
        {
            if (counts[i] > bestCount)
            {
                bestCount = counts[i];
                best = (WritingScript)i;
            }
        }

        return ForScript(best);
    }

    /// <summary>
    /// Which script a character belongs to, or null when it carries none — digits, spaces, ASCII
    /// punctuation and anything else that would appear on a page in any language.
    /// </summary>
    private static WritingScript? ScriptOf(char c)
    {
        if (char.IsAsciiLetter(c))
        {
            return WritingScript.Latin;
        }

        if (c < 0x00C0)
        {
            // Digits, ASCII punctuation, whitespace and the Latin-1 symbols: no script.
            return null;
        }

        return c switch
        {
            '×' or '÷' => null,                    // the two maths signs sitting among the Latin-1 letters
            <= 'ɏ' => WritingScript.Latin,          // Latin-1 letters and Latin Extended A/B
            >= 'Ͱ' and <= 'Ͽ' => WritingScript.Greek,
            >= 'Ѐ' and <= 'ԯ' => WritingScript.Cyrillic,
            >= '֐' and <= '׿' => WritingScript.Hebrew,
            >= '؀' and <= 'ۿ' => WritingScript.Arabic,
            >= 'ݐ' and <= 'ݿ' => WritingScript.Arabic,
            >= '฀' and <= '๿' => WritingScript.Thai,
            >= 'ἀ' and <= '῿' => WritingScript.Greek,   // polytonic
            >= 'ᄀ' and <= 'ᇿ' => WritingScript.Korean,  // jamo
            >= '぀' and <= 'ヿ' => WritingScript.Japanese, // hiragana + katakana
            >= '㄰' and <= '㆏' => WritingScript.Korean,  // compatibility jamo
            >= 'ㇰ' and <= 'ㇿ' => WritingScript.Japanese, // katakana extensions
            >= '㐀' and <= '䶿' => WritingScript.Han,     // extension A
            >= '一' and <= '鿿' => WritingScript.Han,
            >= '가' and <= '힯' => WritingScript.Korean,  // hangul syllables
            >= '豈' and <= '﫿' => WritingScript.Han,     // compatibility ideographs
            >= 'ﭐ' and <= '﷿' => WritingScript.Arabic,
            >= 'ﹰ' and <= 'ﻼ' => WritingScript.Arabic,   // stops short of U+FEFF, which is a zero-width space
            _ => null
        };
    }
}
