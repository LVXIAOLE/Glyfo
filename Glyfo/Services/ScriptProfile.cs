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
    // The values the pipeline used globally, before there were profiles to vary them. They are still
    // the right answer for a script nobody has measured: 20 and 28 came from English and Chinese
    // screenshots, so they are at worst a wasted second pass on a script that did not need one, where
    // guessing lower risks skipping a pass that was needed.
    private const double DefaultRescaleThreshold = 20;
    private const double DefaultTargetWordHeight = 28;

    // Corroborated rather than replaced. tools/ocr-measurements.tsv separates Han cleanly into
    // within-word gaps of 0.03-0.17 line heights and real spaces of 0.30-0.53, so 0.30 sits in the
    // valley between them — the same conclusion the original CJK-at-30px measurement reached. Every
    // other measured script trusts its word boundaries and never consults this at all.
    private const double DefaultSpaceGapRatio = 0.30;

    // Never measured on anything: there is no NPU on the machine this was developed on, so the AI
    // engine's confidence distribution is unknown for every script including this one.
    private const float DefaultRetryConfidence = 0.80f;

    private static readonly ScriptProfile[] Table =
    {
        // Latin stops improving early. Across three recognizers, CER runs 0.073-0.101 at 11px and
        // 0.044-0.058 at 13px, then flat: 17px and 27px are no better than 13px. The global
        // threshold of 20 therefore bought a second recognition pass on English that had already
        // reached its ceiling, which is what ImageLoader's own comment found from the other side.
        // The target clears the plateau with margin rather than sitting on its edge.
        Make(WritingScript.Latin, spaceless: false, fullwidth: false, rtl: false,
            rescaleThreshold: 13, targetWordHeight: 20),

        // Unmeasured — no recognizer installed here, so these keep the conservative global values.
        // Both are alphabetic scripts set with spaces and would very likely behave like Latin, but
        // "very likely" is not what the rest of this file is built on.
        Make(WritingScript.Cyrillic, spaceless: false, fullwidth: false, rtl: false),
        Make(WritingScript.Greek, spaceless: false, fullwidth: false, rtl: false),

        // Arabic fails differently from the others: it does not degrade towards small type, it stops
        // dead. At 10pt the recognizer returns no words at all, and from 13px upward CER is flat.
        // So the threshold is about clearing that cliff, and the target is set generously because
        // the cost of landing just above it is another empty result.
        Make(WritingScript.Arabic, spaceless: false, fullwidth: false, rtl: true,
            rescaleThreshold: 14, targetWordHeight: 24),

        // Unmeasured, and unmeasurable here — Windows has no Hebrew OCR capability to install.
        Make(WritingScript.Hebrew, spaceless: false, fullwidth: false, rtl: true),

        // Han keeps improving where Latin has plateaued, which is why the two cannot share a
        // threshold: Traditional runs 0.426 at 11px, 0.148 at 15px, 0.093 at 19px, and Simplified
        // 0.407 at 12px through 0.167 at 30px. So the threshold stays where it was. The target is
        // pulled back from 28 to 24 because past 19px the two disagree — Simplified is still
        // gaining at 30px while Traditional is slightly worse at 29px than at 19px.
        Make(WritingScript.Han, spaceless: true, fullwidth: true, rtl: false,
            rescaleThreshold: 20, targetWordHeight: 24),

        // Unmeasured (no ja-JP recognizer installed). Kana are simpler shapes than Han and may well
        // tolerate smaller type, but the global values are the safe direction to be wrong in.
        Make(WritingScript.Japanese, spaceless: true, fullwidth: true, rtl: false),

        // Korean is written with spaces, so its boundaries are trustworthy — and it still returns
        // fullwidth punctuation, which before these two flags were separated it had no way to say.
        // It was the case that motivated splitting them, and this is the line that acts on it: a
        // Korean page's "v1．6．5" now gets repaired the way a Chinese one already did, without its
        // word spacing being second-guessed. The numbers are unmeasured (no ko-KR recognizer here).
        Make(WritingScript.Korean, spaceless: false, fullwidth: true, rtl: false),

        // Unmeasured, and unreachable through the built-in engine — Windows ships no Thai OCR
        // capability. Only the AI engine, which detects the script itself, can arrive here.
        Make(WritingScript.Thai, spaceless: true, fullwidth: false, rtl: false),

        // A script nobody anticipated keeps the gap measuring and the normalizer, which is correct
        // everywhere and merely unnecessary in places. Falling the other way would silently join
        // words together.
        Make(WritingScript.Unknown, spaceless: true, fullwidth: true, rtl: false)
    };

    private static ScriptProfile Make(
        WritingScript script,
        bool spaceless,
        bool fullwidth,
        bool rtl,
        double rescaleThreshold = DefaultRescaleThreshold,
        double targetWordHeight = DefaultTargetWordHeight) =>
        new(script, spaceless, fullwidth, rtl,
            rescaleThreshold, targetWordHeight, DefaultSpaceGapRatio, DefaultRetryConfidence);

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
    /// How much of the recognized text is in the script the recognizer was built for, from 0 to 1.
    /// Characters that carry no script — digits, spaces, ASCII punctuation — are not counted, and
    /// text made of nothing else scores 1.
    /// </summary>
    /// <remarks>
    /// The signal the auto mode needs, and it is deliberately an asymmetric one. A Latin recognizer
    /// returns Latin whatever you show it, so its own agreement is always 1 and says nothing. But a
    /// Chinese or Arabic recognizer can read Latin perfectly well, so when one of those hands back a
    /// page of ASCII it is reporting something real: that the text it found is not in its script.
    /// That is the case coverage alone cannot see, because a Chinese recognizer misreading English
    /// still draws boxes over all of it — measured on an English page at 16pt, en-US covers 0.1803
    /// of the image and zh-Hant-TW covers 0.1768, which is a difference of nothing.
    ///
    /// A page with no script-bearing characters scores 1 rather than 0. Nothing about a page of bare
    /// numbers argues against any recognizer, and scoring it 0 would let the margin rule decide by
    /// arrival order alone.
    /// </remarks>
    public static double Agreement(string? text, ScriptProfile profile)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 1;
        }

        var total = 0;
        var belonging = 0;

        foreach (var c in text)
        {
            var script = ScriptOf(c);
            if (script is null)
            {
                continue;
            }

            total++;
            if (Belongs(script.Value, profile.Script))
            {
                belonging++;
            }
        }

        return total == 0 ? 1 : belonging / (double)total;
    }

    /// <summary>
    /// How many characters of the recognizer's own script came back — the absolute count behind
    /// <see cref="Agreement"/>'s ratio.
    /// </summary>
    /// <remarks>
    /// Needed because a ratio cannot distinguish "read a page of my own script" from "read nothing
    /// I recognize". Agreement scores both at 1.0: the first because everything belonged, the second
    /// because nothing was there to belong. That is the right answer for weighting a score, where a
    /// page of bare digits should count against nobody, and the wrong one for deciding whether a
    /// result is convincing enough to stop looking at the alternatives.
    /// </remarks>
    public static int ScriptEvidence(string? text, ScriptProfile profile)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        var belonging = 0;

        foreach (var c in text)
        {
            var script = ScriptOf(c);
            if (script is not null && Belongs(script.Value, profile.Script))
            {
                belonging++;
            }
        }

        return belonging;
    }

    /// <summary>
    /// Whether a character's script counts as belonging to a recognizer's. Mostly an equality test,
    /// except where one recognizer legitimately covers several scripts: Japanese is set in kana and
    /// Han together, and Korean prose carries hanja among the hangul.
    /// </summary>
    private static bool Belongs(WritingScript character, WritingScript recognizer) =>
        character == recognizer ||
        (recognizer == WritingScript.Japanese && character == WritingScript.Han) ||
        (recognizer == WritingScript.Korean && character == WritingScript.Han);

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
