// Exercises TextLayout.Repair over the homoglyph cases, which are the one part of the OCR work
// that cannot be verified end to end on this machine: Windows ships no Greek or Cyrillic
// recognizer here, so no probe can make one return a Greek omicron. The real TextLayout.cs is
// compiled into this program rather than copied, so what passes here is what ships.
//
// Every literal is built from code points. Typing the characters would defeat the purpose: the
// whole class of bug is that the wrong one is indistinguishable from the right one on screen.

using System;
using System.Collections.Generic;
using System.Text;
using Glyfo.Services;

internal static class Program
{
    private static int _failures;

    private static void Main()
    {
        // Greek
        const int Alpha = 0x0391, Beta = 0x0392, Iota = 0x0399, Omicron = 0x039F;
        // Cyrillic
        const int CyA = 0x0410, CyEn = 0x041D, CyO = 0x041E, CyTe = 0x0422, CyU = 0x0423;
        // Greek prose, none of which has an ASCII twin
        const int OmicronTonos = 0x038C, Lambda = 0x03BB, AlphaSmall = 0x03B1;

        Check("greek omicron between digits", S('2', Omicron, '2', '6'), "2026");
        Check("cyrillic O between digits", S('2', CyO, '2', '6'), "2026");
        Check("greek iota as a version 1", S('v', Iota, '.', '6', '.', '5'), "v1.6.5");

        Check("cyrillic model number", S(CyTe, CyU, '-', '1', '5', '4'), S(CyTe, CyU, '-', '1', '5', '4'));
        Check("cyrillic aircraft number", S(CyA, CyEn, '-', '2', '4'), S(CyA, CyEn, '-', '2', '4'));
        Check("greek prose", S(OmicronTonos, Lambda, AlphaSmall), S(OmicronTonos, Lambda, AlphaSmall));

        // A look-alike that is not in a numeric position stays put even when the run holds digits.
        Check("greek letters beside a folded one",
            S(Alpha, Beta, '2', Omicron, '2', '6'),
            S(Alpha, Beta, '2', '0', '2', '6'));

        // The rules that were already here, unchanged.
        Check("ascii O between digits", "2O26", "2026");
        Check("ascii l as a version 1", "vl.6.5", "v1.6.5");
        Check("html5", "html5", "html5");
        Check("IPv6", "IPv6", "IPv6");
        Check("COVID-19", "COVID-19", "COVID-19");
        Check("O2", "O2", "O2");
        Check("H2O", "H2O", "H2O");

        // The whole thing hangs off the user's switch.
        Check("greek omicron with repair off", S('2', Omicron, '2', '6'), S('2', Omicron, '2', '6'), repair: false);
        Check("greek iota with repair off", S('v', Iota, '.', '6', '.', '5'), S('v', Iota, '.', '6', '.', '5'), repair: false);

        Console.WriteLine(_failures == 0 ? "all passed" : _failures + " failed");
        Environment.Exit(_failures == 0 ? 0 : 1);
    }

    private static string S(params int[] codePoints)
    {
        var builder = new StringBuilder(codePoints.Length);
        foreach (var point in codePoints)
        {
            builder.Append((char)point);
        }

        return builder.ToString();
    }

    private static void Check(string name, string input, string expected, bool repair = true)
    {
        // One token carrying the whole line: JoinLine then does nothing but Repair, which is the
        // part under test. Fullwidth normalization is off because no Greek or Cyrillic recognizer
        // emits fullwidth forms, so that is the profile these cases arrive under.
        var tokens = new List<OcrToken> { new(input, 0, 100, 20) };
        var options = new TextLayoutOptions(
            TrustWordBoundaries: true,
            RepairNumbers: repair,
            NormalizeFullwidth: false,
            RightToLeft: false,
            SpaceGapRatio: 0.30);

        var actual = TextLayout.JoinLine(tokens, options);
        var ok = actual == expected;
        if (!ok)
        {
            _failures++;
        }

        Console.WriteLine("{0,-4} {1,-34} {2}  ->  {3}   want {4}",
            ok ? "ok" : "FAIL", name, Show(input), Show(actual), Show(expected));
    }

    private static string Show(string text)
    {
        var builder = new StringBuilder();
        foreach (var c in text)
        {
            builder.Append(c < 0x80 ? c.ToString() : "<U+" + ((int)c).ToString("X4") + ">");
        }

        return builder.ToString();
    }
}
