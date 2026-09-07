# Measures every installed Windows OCR recognizer against every writing system in ocr-corpus.txt,
# at four type sizes. The numbers it prints are the ones that belong in ScriptProfile.cs -- this
# repo annotates every constant in the recognition pipeline with how it was measured, and the
# per-script table has no reason to hold itself to a lower standard than the global constants it
# replaces.
#
# Read the output two ways. Down the diagonal (recognizer matches the corpus script) the CER column
# says where accuracy collapses as type gets smaller, which is what RescaleThreshold and
# TargetWordHeight have to bracket. Off the diagonal it says how a recognizer behaves on a script it
# was not trained for, which is what the auto mode's scoring has to be able to tell apart -- so the
# coverage, agreement and mean-token-length columns matter most in exactly the cells the CER column
# is meaningless in.
#
# Two things the CER column will not tell you. It is only comparable to itself across type sizes,
# never across scripts: a right-to-left recognizer returns its words in visual order while the corpus
# is written in logical order, so Arabic carries a large constant reordering penalty (measured at a
# flat 0.4286 across three sizes, which is the offset rather than the error). And it says nothing
# about where the recognizer put the words, which is why the gap columns exist -- those are the
# quantity TextLayout.SpaceGapRatio thresholds, in the same units.
#
# This script is ASCII on purpose. Windows PowerShell 5.1 reads a BOM-less .ps1 in the ANSI code
# page, which would silently mangle every non-Latin line in here; the corpus lives in a separate
# UTF-8 payload read back with -Encoding UTF8, the same split tools\Insert-Strings.ps1 uses.
#
# Usage:  powershell -NoProfile -ExecutionPolicy Bypass -File tools\Measure-Ocr.ps1
#         powershell ... -File tools\Measure-Ocr.ps1 -Out measured.tsv -KeepImages

param(
    [string]$Corpus = (Join-Path $PSScriptRoot 'ocr-corpus.txt'),
    [string]$Out    = (Join-Path $PSScriptRoot 'ocr-measurements.tsv'),
    [int[]] $Sizes  = @(10, 12, 16, 24),
    [switch]$KeepImages)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Runtime.WindowsRuntime

# ---------------------------------------------------------------------------------------------
# Metrics, in C# because PowerShell would take minutes over the edit distance and because the
# script classification below has to agree character-for-character with Glyfo\Services\ScriptProfile.cs.
# Unicode ranges are written as code points rather than character literals for the same encoding
# reason the header explains. /unsafe is for the one pixel loop, which is why this goes through
# CompilerParameters rather than the -ReferencedAssemblies shorthand (they are different parameter
# sets and Add-Type refuses both at once).
# ---------------------------------------------------------------------------------------------
$compiler = New-Object System.CodeDom.Compiler.CompilerParameters
$compiler.CompilerOptions = '/unsafe'
$compiler.GenerateInMemory = $true
# ReferencedAssemblies is a read-only StringCollection, so it has to be filled rather than assigned.
[void]$compiler.ReferencedAssemblies.Add('System.dll')
[void]$compiler.ReferencedAssemblies.Add('System.Drawing.dll')

Add-Type -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;

public static class OcrMetrics
{
    public static int Levenshtein(string a, string b)
    {
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;

        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];
        for (var j = 0; j <= b.Length; j++) previous[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            for (var j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                current[j] = Math.Min(Math.Min(current[j - 1] + 1, previous[j] + 1), previous[j - 1] + cost);
            }
            var swap = previous; previous = current; current = swap;
        }

        return previous[b.Length];
    }

    /// <summary>
    /// The script a character belongs to, or null for digits, punctuation and whitespace. The
    /// ranges are the ones in ScriptProfiles.ScriptOf and have to stay in step with them; they are
    /// written as code points rather than character literals so this file stays ASCII.
    /// </summary>
    public static string ScriptOf(char c)
    {
        if (c >= 'a' && c <= 'z') return "Latn";
        if (c >= 'A' && c <= 'Z') return "Latn";
        if (c < 0x00C0) return null;                     // digits, ASCII punctuation, Latin-1 symbols
        if (c == 0x00D7 || c == 0x00F7) return null;     // the two maths signs among the Latin-1 letters
        if (c <= 0x024F) return "Latn";
        if (c >= 0x0370 && c <= 0x03FF) return "Grek";
        if (c >= 0x0400 && c <= 0x052F) return "Cyrl";
        if (c >= 0x0590 && c <= 0x05FF) return "Hebr";
        if (c >= 0x0600 && c <= 0x06FF) return "Arab";
        if (c >= 0x0750 && c <= 0x077F) return "Arab";
        if (c >= 0x0E00 && c <= 0x0E7F) return "Thai";
        if (c >= 0x1F00 && c <= 0x1FFF) return "Grek";   // polytonic
        if (c >= 0x1100 && c <= 0x11FF) return "Hang";   // jamo
        if (c >= 0x3040 && c <= 0x30FF) return "Kana";   // hiragana + katakana
        if (c >= 0x3130 && c <= 0x318F) return "Hang";   // compatibility jamo
        if (c >= 0x31F0 && c <= 0x31FF) return "Kana";   // katakana extensions
        if (c >= 0x3400 && c <= 0x4DBF) return "Hani";   // extension A
        if (c >= 0x4E00 && c <= 0x9FFF) return "Hani";
        if (c >= 0xAC00 && c <= 0xD7AF) return "Hang";   // hangul syllables
        if (c >= 0xF900 && c <= 0xFAFF) return "Hani";   // compatibility ideographs
        if (c >= 0xFB50 && c <= 0xFDFF) return "Arab";
        if (c >= 0xFE70 && c <= 0xFEFC) return "Arab";   // stops short of FEFF, a zero-width space
        return null;
    }

    /// <summary>
    /// The share of script-bearing characters that belong to the recognizer's own script. Digits,
    /// ASCII punctuation and whitespace do not count, and text with none of either scores 1.0 --
    /// a page of nothing but numbers gives nobody grounds to disagree.
    /// </summary>
    public static double Agreement(string text, string iso)
    {
        var total = 0;
        var mine = 0;

        foreach (var c in text)
        {
            var script = ScriptOf(c);
            if (script == null) continue;
            total++;
            if (Belongs(script, iso)) mine++;
        }

        return total == 0 ? 1.0 : (double)mine / total;
    }

    private static bool Belongs(string script, string iso)
    {
        switch (iso)
        {
            case "Hans":
            case "Hant":
            case "Hani": return script == "Hani";
            // Japanese is Han with kana threaded through it; Korean mixes hanja into hangul.
            case "Jpan": return script == "Hani" || script == "Kana";
            case "Kore":
            case "Hang": return script == "Hang" || script == "Hani";
            default:     return script == iso;
        }
    }

    /// <summary>
    /// Fraction of pixels darker than mid grey. A family that is installed but has no glyph for the
    /// text still draws something -- a row of tofu boxes -- so this only catches a blank render.
    /// The recognizer output is the real backstop: boxes come back as a CER near 1.0.
    /// </summary>
    public static double DarkFraction(Bitmap bitmap)
    {
        var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var dark = 0L;

        try
        {
            unsafe
            {
                for (var y = 0; y < data.Height; y++)
                {
                    var row = (byte*)data.Scan0 + (long)y * data.Stride;
                    for (var x = 0; x < data.Width; x++)
                    {
                        var p = row + x * 4;
                        if ((p[0] + p[1] + p[2]) < 384) dark++;
                    }
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return (double)dark / ((long)bitmap.Width * bitmap.Height);
    }
}
'@ -CompilerParameters $compiler

# ---------------------------------------------------------------------------------------------
# WinRT plumbing. Every projected type has to be named with its contract once before PowerShell
# will resolve it, and every async call has to be pumped by hand.
# ---------------------------------------------------------------------------------------------
$null = [Windows.Media.Ocr.OcrEngine, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime]
$null = [Windows.Globalization.Language, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime]
$null = [Windows.Graphics.Imaging.BitmapDecoder, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime]
$null = [Windows.Storage.Streams.FileRandomAccessStream, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime]

$asTask = ([System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object {
    $_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and
    $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1' })[0]

function Await($operation, [Type]$resultType) {
    $task = $asTask.MakeGenericMethod($resultType).Invoke($null, @($operation))
    [void]$task.Wait(-1)
    $task.Result
}

function Read-SoftwareBitmap([string]$path) {
    $stream = Await ([Windows.Storage.Streams.FileRandomAccessStream]::OpenAsync(
        $path, [Windows.Storage.FileAccessMode]::Read)) ([Windows.Storage.Streams.IRandomAccessStream])
    try {
        $decoder = Await ([Windows.Graphics.Imaging.BitmapDecoder]::CreateAsync($stream)) `
            ([Windows.Graphics.Imaging.BitmapDecoder])
        # Bgra8 premultiplied, because that is the one format both engines accept.
        Await ($decoder.GetSoftwareBitmapAsync(
            [Windows.Graphics.Imaging.BitmapPixelFormat]::Bgra8,
            [Windows.Graphics.Imaging.BitmapAlphaMode]::Premultiplied)) `
            ([Windows.Graphics.Imaging.SoftwareBitmap])
    }
    finally { $stream.Dispose() }
}

# ---------------------------------------------------------------------------------------------
# Corpus
# ---------------------------------------------------------------------------------------------
if (-not (Test-Path $Corpus)) { throw "corpus not found: $Corpus" }

$cases = @()
$current = $null
foreach ($line in (Get-Content -LiteralPath $Corpus -Encoding UTF8)) {
    if ($line -match '^\[(?<name>[^\]]+)\]\s*(?<iso>\S+)\s*\|\s*(?<font>.+?)\s*$') {
        if ($current) { $cases += $current }
        $current = @{ Name = $Matches.name; Iso = $Matches.iso; Font = $Matches.font; Lines = @() }
        continue
    }
    if ($line.Trim().Length -eq 0) { continue }
    if ($current) { $current.Lines += $line }
}
if ($current) { $cases += $current }

Write-Output "corpus: $($cases.Count) writing systems x $($Sizes.Count) sizes"

$installedFonts = @{}
foreach ($f in (New-Object System.Drawing.Text.InstalledFontCollection).Families) {
    $installedFonts[$f.Name] = $true
}

# ---------------------------------------------------------------------------------------------
# Recognizers
# ---------------------------------------------------------------------------------------------
$recognizers = @()
foreach ($language in [Windows.Media.Ocr.OcrEngine]::AvailableRecognizerLanguages) {
    $recognizers += [pscustomobject]@{
        Tag    = $language.LanguageTag
        Iso    = $language.Script
        Rtl    = ($language.LayoutDirection -eq [Windows.Globalization.LanguageLayoutDirection]::Rtl)
        Native = $language.NativeName
        Engine = [Windows.Media.Ocr.OcrEngine]::TryCreateFromLanguage($language)
    }
}
Write-Output "recognizers: $(($recognizers | ForEach-Object { $_.Tag }) -join ', ')"
Write-Output ''

# ---------------------------------------------------------------------------------------------
# Render, recognize, measure
# ---------------------------------------------------------------------------------------------
$imageDir = Join-Path ([IO.Path]::GetTempPath()) 'glyfo-measure'
if (Test-Path $imageDir) { Remove-Item $imageDir -Recurse -Force }
$null = New-Item -ItemType Directory -Path $imageDir

function New-CorpusImage($lines, [string]$family, [int]$points, [string]$path) {
    $font = New-Object System.Drawing.Font $family, ([float]$points)
    $probe = New-Object System.Drawing.Bitmap 1, 1
    $pg = [System.Drawing.Graphics]::FromImage($probe)

    $width = 0.0
    foreach ($line in $lines) { $width = [Math]::Max($width, $pg.MeasureString($line, $font).Width) }
    $lineHeight = $font.GetHeight($pg) * 1.4
    $pg.Dispose(); $probe.Dispose()

    $w = [int][Math]::Ceiling($width) + 48
    $h = [int][Math]::Ceiling($lineHeight * $lines.Count) + 48

    $bmp = New-Object System.Drawing.Bitmap $w, $h, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear([System.Drawing.Color]::White)
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit

    $y = 24.0
    foreach ($line in $lines) {
        $g.DrawString($line, $font, [System.Drawing.Brushes]::Black, 24, $y)
        $y += $lineHeight
    }
    $g.Dispose()
    $font.Dispose()

    $dark = [OcrMetrics]::DarkFraction($bmp)
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()

    [pscustomobject]@{ Width = $w; Height = $h; Dark = $dark }
}

# Nearest-rank percentile. The index has to truncate the way C# integer division does -- PowerShell's
# [int] cast rounds, which for a single-element list turns index 0 into index 1 and hands back $null.
function Get-Percentile($values, [double]$fraction) {
    if ($values.Count -eq 0) { return 0.0 }
    $sorted = @($values | Sort-Object)
    $i = [int][Math]::Floor($sorted.Count * $fraction)
    if ($i -ge $sorted.Count) { $i = $sorted.Count - 1 }
    [double]$sorted[$i]
}

$rows = @()
$warnings = @()

foreach ($case in $cases) {
    if (-not $installedFonts.ContainsKey($case.Font)) {
        # GDI+ substitutes silently for a missing family, which would file a whole writing system's
        # numbers under a font that never drew it. Skip loudly instead.
        $warnings += "$($case.Name): font '$($case.Font)' is not installed -- skipped"
        continue
    }

    $reference = ($case.Lines -join '') -replace '\s', ''

    foreach ($points in $Sizes) {
        $path = Join-Path $imageDir "$($case.Name)-$points.png"
        $image = New-CorpusImage $case.Lines $case.Font $points $path

        if ($image.Dark -lt 0.005) {
            $warnings += "$($case.Name) @ ${points}pt: blank render (dark fraction $([Math]::Round($image.Dark, 5)))"
        }

        $bitmap = Read-SoftwareBitmap $path
        $area = [double]$image.Width * $image.Height

        foreach ($recognizer in $recognizers) {
            $result = Await ($recognizer.Engine.RecognizeAsync($bitmap)) ([Windows.Media.Ocr.OcrResult])

            $heights = New-Object System.Collections.Generic.List[double]
            $lengths = New-Object System.Collections.Generic.List[double]
            $gaps    = New-Object System.Collections.Generic.List[double]
            $covered = 0.0
            $text = New-Object System.Text.StringBuilder

            foreach ($line in $result.Lines) {
                $lineHeight = 0.0
                foreach ($word in $line.Words) {
                    $lineHeight = [Math]::Max($lineHeight, [double]$word.BoundingRect.Height)
                }

                $previous = $null
                foreach ($word in $line.Words) {
                    $box = $word.BoundingRect
                    if ($box.Height -gt 0) { $heights.Add($box.Height) }
                    $covered += $box.Width * $box.Height
                    $lengths.Add($word.Text.Length)
                    [void]$text.Append($word.Text)

                    # The quantity TextLayout.NeedsSpace thresholds: the horizontal separation
                    # between two adjacent boxes as a fraction of the line height. Measured without
                    # assuming a direction, because in a right-to-left line the second word sits to
                    # the left of the first and the subtraction runs the other way.
                    if ($null -ne $previous -and $lineHeight -gt 0) {
                        $gap = if ($box.Left -ge $previous.Right) { [double]($box.Left - $previous.Right) }
                               elseif ($previous.Left -ge $box.Right) { [double]($previous.Left - $box.Right) }
                               else { 0.0 }
                        $gaps.Add($gap / $lineHeight)
                    }
                    $previous = $box
                }
            }

            $recognized = $text.ToString() -replace '\s', ''
            $cer = if ($reference.Length -eq 0) { 0.0 }
                   else { [OcrMetrics]::Levenshtein($recognized, $reference) / [double]$reference.Length }

            # Upper quartile, the same statistic ImageLoader.SuggestUpscale decides on.
            $quartile = Get-Percentile $heights 0.75

            $rows += [pscustomobject]@{
                Corpus     = $case.Name
                CorpusIso  = $case.Iso
                Points     = $points
                Recognizer = $recognizer.Tag
                Iso        = $recognizer.Iso
                Words      = $lengths.Count
                Cer        = [Math]::Round([Math]::Min([double]$cer, 9.999), 4)
                WordQ3     = [Math]::Round([double]$quartile, 1)
                Coverage   = [Math]::Round([double]($covered / $area), 4)
                Agreement  = [Math]::Round([OcrMetrics]::Agreement($recognized, $recognizer.Iso), 4)
                TokenLen   = if ($lengths.Count -eq 0) { 0.0 }
                             else { [Math]::Round([double](($lengths | Measure-Object -Sum).Sum / $lengths.Count), 2) }
                # Gap distribution, for SpaceGapRatio. A spaceless script should show two populations
                # -- the near-zero gaps the recognizer leaves where it split a word, and the wider ones
                # at real spaces -- so the ratio belongs in the valley between p25 and p75.
                GapP25     = [Math]::Round((Get-Percentile $gaps 0.25), 3)
                GapP50     = [Math]::Round((Get-Percentile $gaps 0.50), 3)
                GapP75     = [Math]::Round((Get-Percentile $gaps 0.75), 3)
                GapMax     = [Math]::Round((Get-Percentile $gaps 1.00), 3)
            }
        }

        $bitmap.Dispose()
        Write-Output ("{0,-10} {1,2}pt  {2}x{3}px  dark {4:P1}" -f `
            $case.Name, $points, $image.Width, $image.Height, $image.Dark)
    }
}

# ---------------------------------------------------------------------------------------------
# Output
# ---------------------------------------------------------------------------------------------
$rows | Export-Csv -LiteralPath $Out -NoTypeInformation -Delimiter "`t" -Encoding UTF8
Write-Output ''
Write-Output "wrote $Out  ($($rows.Count) rows)"

Write-Output ''
Write-Output '=== diagonal: recognizer script matches corpus script ==='
Write-Output ('{0,-10} {1,-12} {2,4} {3,7} {4,7} {5,8} {6,8} {7,8} {8,8}' -f `
    'corpus', 'recognizer', 'pt', 'CER', 'wordQ3', 'gapP25', 'gapP50', 'gapP75', 'tokenLen')
foreach ($row in $rows) {
    if ($row.Iso -eq $row.CorpusIso -or
        ($row.CorpusIso -eq 'Jpan' -and $row.Iso -eq 'Jpan') -or
        ($row.CorpusIso -eq 'Kore' -and $row.Iso -eq 'Kore')) {
        Write-Output ('{0,-10} {1,-12} {2,4} {3,7} {4,7} {5,8} {6,8} {7,8} {8,8}' -f `
            $row.Corpus, $row.Recognizer, $row.Points, $row.Cer, $row.WordQ3,
            $row.GapP25, $row.GapP50, $row.GapP75, $row.TokenLen)
    }
}

Write-Output ''
Write-Output '=== off-diagonal at 16pt: what the auto mode has to tell apart ==='
Write-Output ('{0,-10} {1,-12} {2,7} {3,8} {4,9} {5,8}' -f `
    'corpus', 'recognizer', 'CER', 'coverage', 'agreement', 'tokenLen')
foreach ($row in ($rows | Where-Object { $_.Points -eq 16 })) {
    Write-Output ('{0,-10} {1,-12} {2,7} {3,8} {4,9} {5,8}' -f `
        $row.Corpus, $row.Recognizer, $row.Cer, $row.Coverage, $row.Agreement, $row.TokenLen)
}

if ($warnings.Count -gt 0) {
    Write-Output ''
    Write-Output '=== warnings ==='
    foreach ($w in $warnings) { Write-Output $w }
}

if ($KeepImages) { Write-Output ''; Write-Output "images kept in $imageDir" }
else { Remove-Item $imageDir -Recurse -Force }
