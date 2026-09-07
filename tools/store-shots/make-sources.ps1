# Builds the images that appear inside the Store screenshots. They are generated rather than
# photographed so the text in them is ours: a Store listing is public, and a screenshot of someone
# else's page or document is their copyright, not ours.
#
# Each one stands for a real reason people reach for OCR — a scanned page, a slide, a label with
# codes on it — and the English page deliberately contains "v1.6.5", which is the exact string the
# "fix l and I inside numbers" option exists for.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$out = Join-Path $PSScriptRoot 'src'
New-Item -ItemType Directory -Force -Path $out | Out-Null

function New-Page {
    param([int]$W, [int]$H, [string]$Paper = '#FFFFFF')
    $bmp = New-Object Drawing.Bitmap $W, $H
    $g = [Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'AntiAlias'
    $g.TextRenderingHint = 'ClearTypeGridFit'
    $g.Clear([Drawing.ColorTranslator]::FromHtml($Paper))
    # Two separate outputs, not one array: "$bmp, $g = New-Page" only destructures if the function
    # emits two objects.
    @($bmp, $g)
}

function Write-Line {
    param($g, [string]$Text, [string]$Font, [single]$Size, [string]$Style, [single]$X, [single]$Y, [string]$Color = '#1A1A1A')
    $f = New-Object Drawing.Font $Font, $Size, ([Drawing.FontStyle]$Style), ([Drawing.GraphicsUnit]::Pixel)
    $b = New-Object Drawing.SolidBrush ([Drawing.ColorTranslator]::FromHtml($Color))
    $g.DrawString($Text, $f, $b, $X, $Y)
    $h = $g.MeasureString($Text, $f).Height
    $f.Dispose(); $b.Dispose()
    return $h
}

# ---------------------------------------------------------------- 1. a scanned page, English
$bmp, $g = New-Page 1000 1020 '#FCFBF8'
$y = 90.0
$y += Write-Line $g 'Bench notes — humidity logger' 'Georgia' 40 'Bold' 90 $y
$y += 18
$y += Write-Line $g 'Rev C board, firmware v1.6.5, 12 March' 'Georgia' 24 'Italic' 90 $y '#5A5A5A'
$y += 44
$g.DrawLine((New-Object Drawing.Pen ([Drawing.ColorTranslator]::FromHtml('#CFCAC0')), 1), 90, $y, 910, $y)
$y += 40

$body = @(
    'The sensor settles about four minutes after power-up. Readings taken',
    'before that run two to three points high, which is enough to matter for',
    'the calibration table but not enough to look obviously wrong, so the',
    'logger now discards the first 240 seconds outright.',
    '',
    'Three units were left running over the weekend beside a reference',
    'meter. Drift over 62 hours came out at 0.4 %RH for two of them and',
    '1.1 %RH for the third, which had been the one reflowed by hand.',
    '',
    'Open questions',
    '',
    '  1. Does the enclosure vent need a filter, or is the dust load low',
    '     enough that the mesh alone will hold for a season?',
    '  2. The 3.3 V rail sags to 3.11 V while the radio transmits. It has not',
    '     caused a reset yet, but the margin is thinner than it looks on paper.',
    '  3. Nothing in the log distinguishes a genuine flat line from a stuck',
    '     bus. Worth a heartbeat counter in the next revision.',
    '',
    'Next: rerun the soak with the vent filter fitted, and record the boot',
    'time separately so the discard window can be tightened later.'
)
foreach ($line in $body) {
    if ($line -eq '') { $y += 16; continue }
    $y += Write-Line $g $line 'Georgia' 26 'Regular' 90 $y
    $y += 6
}
$g.Dispose()
$bmp.Save((Join-Path $out 'notes-en.png'), [Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

# ---------------------------------------------------------------- 2. a slide, Simplified Chinese
# 1240x820 rather than something wider: the preview pane is roughly four by three, and a letterbox
# slide sat in it with a band of empty space above and below. The body text is 36px because at 30
# the recognizer took 浊 apart into 冫 and 虫, and 位 into 亻 and 立 — CJK glyphs carry far more
# strokes in the same box than Latin ones and need the pixels.
$bmp, $g = New-Page 1240 820 '#F7F4EE'
$bar = New-Object Drawing.SolidBrush ([Drawing.ColorTranslator]::FromHtml('#0F5A8F'))
$g.FillRectangle($bar, 0, 0, 1240, 16)
$bar.Dispose()
$y = 100.0
$y += Write-Line $g '第三季度水质抽样结果' 'Microsoft YaHei' 58 'Bold' 90 $y '#12446B'
$y += 32
$y += Write-Line $g '采样点 24 个，覆盖上游、库区与三条支流' 'Microsoft YaHei' 30 'Regular' 90 $y '#5A5A5A'
$y += 56
foreach ($line in @(
    '· 浊度中位数 3.2 NTU，较上季度下降 0.6',
    '· 溶解氧全部高于 6 mg/L，最低值出现在 8 月末的库尾',
    '· 两个支流的总磷仍高于目标值，与降雨后的农田径流一致',
    '· 全部 24 个点位的粪大肠菌群均低于检出限',
    '',
    '下一步：把 8 月末的连续监测数据单独取出来，',
    '核对与放水的时间关系。'
)) {
    if ($line -eq '') { $y += 26; continue }
    $y += Write-Line $g $line 'Microsoft YaHei' 36 'Regular' 90 $y
    $y += 18
}
$g.Dispose()
$bmp.Save((Join-Path $out 'slide-zh.png'), [Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

# ---------------------------------------------------------------- 3. a label carrying codes
# zxing is loaded from the NuGet cache rather than the app's own copy: the published copy is a
# .NET 8 assembly and Windows PowerShell cannot load it. Only the BitMatrix is used, drawn here,
# so no renderer assembly is needed either.
Add-Type -Path "$env:USERPROFILE\.nuget\packages\zxing.net\0.16.10\lib\net48\zxing.dll"

function Draw-Code {
    param($g, [string]$Content, $Format, [int]$X, [int]$Y, [int]$W, [int]$H)
    $writer = New-Object ZXing.MultiFormatWriter
    $hints = New-Object 'System.Collections.Generic.Dictionary[ZXing.EncodeHintType,object]'
    $hints[[ZXing.EncodeHintType]::MARGIN] = 1
    # 0x0 for QR so the matrix comes back one cell per module and the scaling below lands on whole
    # pixels; the linear writers want the real pixel size instead and produce a full-height matrix.
    if ($Format -eq [ZXing.BarcodeFormat]::QR_CODE) {
        $m = $writer.encode($Content, $Format, 0, 0, $hints)
    } else {
        $m = $writer.encode($Content, $Format, $W, $H, $hints)
    }
    $cw = $W / $m.Width
    $ch = $H / $m.Height
    $black = New-Object Drawing.SolidBrush ([Drawing.Color]::Black)
    for ($r = 0; $r -lt $m.Height; $r++) {
        for ($c = 0; $c -lt $m.Width; $c++) {
            if ($m[$c, $r]) {
                $g.FillRectangle($black, [single]($X + $c * $cw), [single]($Y + $r * $ch),
                                 [single][Math]::Ceiling($cw), [single][Math]::Ceiling($ch))
            }
        }
    }
    $black.Dispose()
}

$bmp, $g = New-Page 1100 760 '#FFFFFF'
$g.DrawRectangle((New-Object Drawing.Pen ([Drawing.ColorTranslator]::FromHtml('#222222')), 4), 30, 30, 1040, 700)
$y = 70.0
$y += Write-Line $g 'CRATE 4471-B' 'Consolas' 54 'Bold' 70 $y
$y += 10
$y += Write-Line $g 'Humidity loggers, rev C — 40 units' 'Segoe UI' 30 'Regular' 70 $y '#444444'

Draw-Code $g 'https://lvxiaole.github.io/glyfo-site/' ([ZXing.BarcodeFormat]::QR_CODE) 700 90 320 320
Draw-Code $g '4471000012847' ([ZXing.BarcodeFormat]::EAN_13) 70 470 520 170
[void](Write-Line $g '4 471000 012847' 'Consolas' 34 'Regular' 130 650 '#111111')
[void](Write-Line $g 'Scan for the packing list' 'Segoe UI' 24 'Regular' 700 425 '#444444')
$g.Dispose()
$bmp.Save((Join-Path $out 'label-codes.png'), [Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

# ---------------------------------------------------------------- 4. a three-page PDF
# Written out byte by byte rather than produced by a library, because there is no PDF writer on this
# machine that is not a whole install: no Word, no LaTeX, and printing to "Microsoft Print to PDF"
# cannot be driven headlessly. A PDF with nothing in it but Helvetica text is small enough to build
# by hand -- ten objects and an offset table -- and the format has not moved since 1.4.
#
# Two shots need it. The PDF one shows the page bar, which is the only visible proof the app reads
# PDFs at all; the batch one is taken by pressing "read every page", which is the one route to the
# batch dialog that starts from a single file and so needs no file picker driven from a script.
#
# Everything below is ASCII on purpose. The xref offsets are byte offsets, so one multi-byte
# character anywhere above them would move every entry and the file would not open -- and the check
# at the end of this section is there because a broken xref fails silently in some readers and
# loudly in Windows' own, which is the one that matters here.

function ConvertTo-PdfText([string]$Text) {
    if ($Text -cmatch '[^\x20-\x7E]') { throw "Non-ASCII in PDF text: $Text" }
    $Text -replace '\\', '\\' -replace '\(', '\(' -replace '\)', '\)'
}

function New-PdfContent([string]$Title, [string[]]$Body) {
    # 612x792 is US Letter in points, and the app renders a page at 2.5x, so 13pt body text arrives
    # at the recognizer about 32 px tall -- comfortably above where accuracy starts to fall off.
    $lines = @("BT /F2 22 Tf 72 720 Td ($(ConvertTo-PdfText $Title)) Tj ET")
    $y = 674
    foreach ($line in $Body) {
        if ($line -eq '') { $y -= 12; continue }
        $lines += "BT /F1 13 Tf 72 $y Td ($(ConvertTo-PdfText $line)) Tj ET"
        $y -= 22
    }
    $lines -join "`n"
}

$pages = @(
    @{ Title = 'Acceptance report - crate 4471-B'
       Body = @(
        'Forty humidity loggers, rev C, received 14 March. This report covers the',
        'incoming check: enclosure, power rail, and a four-hour soak against the',
        'reference meter in bay 2.',
        '',
        'Packing was intact and the desiccant sachets were still blue. Two units',
        'had scuffed lids, cosmetic only, and are noted here so that they are not',
        'raised again at the outgoing check.',
        '',
        'One unit, serial 4471-B-17, would not enumerate over USB until the cable',
        'was reseated. It has been kept in the batch but is flagged on page 3.',
        '',
        'All firmware reported v1.6.5. No unit needed reflashing.')
    },
    @{ Title = 'Measurements'
       Body = @(
        'Soak conditions: 22.4 C, 45 %RH nominal, four hours, chamber door shut',
        'throughout. Readings sampled every 30 seconds and averaged per minute.',
        '',
        'Drift over the soak, worst unit    1.1 %RH',
        'Drift over the soak, median        0.4 %RH',
        'Settling time after power-up       3 min 50 s',
        'Rail under radio transmit          3.11 V',
        'Units outside the 2 %RH band       0 of 40',
        '',
        'The settling figure is the reason the logger discards its first 240',
        'seconds. Nothing in this batch settled slower than that, so the discard',
        'window can stay where it is for now.')
    },
    @{ Title = 'Findings and next steps'
       Body = @(
        '1. Serial 4471-B-17 enumerated only after the cable was reseated. Retest',
        '   with a known good cable before it ships. If it repeats, the connector',
        '   is the suspect, not the board.',
        '',
        '2. The 3.3 V rail sags to 3.11 V while the radio transmits. No reset was',
        '   seen in four hours, but the margin is thinner than the schematic',
        '   suggests and should be measured again at low temperature.',
        '',
        '3. Nothing in the log tells a genuine flat line from a stuck bus. A',
        '   heartbeat counter is proposed for the next revision.',
        '',
        'Batch accepted. Recheck 4471-B-17 before dispatch.')
    }
)

# Objects 1 and 2 are the catalogue and the page tree; then a page and its content stream per page;
# then the two fonts. The order is fixed because the numbers are written into the tree above.
$objects = @()
$pageIds = @()
for ($i = 0; $i -lt $pages.Count; $i++) { $pageIds += 3 + $i * 2 }
$fontRegular = 3 + $pages.Count * 2
$fontBold = $fontRegular + 1

$objects += "<< /Type /Catalog /Pages 2 0 R >>"
$objects += "<< /Type /Pages /Kids [$(($pageIds | ForEach-Object { "$_ 0 R" }) -join ' ')] /Count $($pages.Count) >>"
for ($i = 0; $i -lt $pages.Count; $i++) {
    $page = $pages[$i]
    $id = $pageIds[$i]
    $objects += "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] " +
        "/Resources << /Font << /F1 $fontRegular 0 R /F2 $fontBold 0 R >> >> /Contents $($id + 1) 0 R >>"
    $stream = New-PdfContent $page.Title $page.Body
    $objects += "<< /Length $($stream.Length) >>`nstream`n$stream`nendstream"
}
$objects += "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>"
$objects += "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>"

$header = "%PDF-1.4`n"
$body = ''
$offsets = @()
$at = $header.Length
foreach ($i in 0..($objects.Count - 1)) {
    $offsets += $at
    $chunk = "$($i + 1) 0 obj`n$($objects[$i])`nendobj`n"
    $body += $chunk
    $at += $chunk.Length
}

# Every xref entry is exactly twenty bytes, free list entry included. Readers seek by multiplying,
# so a line one byte short breaks every object after it rather than just itself.
$xref = "xref`n0 $($objects.Count + 1)`n0000000000 65535 f `n"
foreach ($offset in $offsets) { $xref += ('{0:D10} 00000 n ' -f $offset) + "`n" }
$pdfText = $header + $body + $xref +
    "trailer`n<< /Size $($objects.Count + 1) /Root 1 0 R >>`nstartxref`n$at`n%%EOF`n"

$pdfPath = Join-Path $out 'report-4471.pdf'
[IO.File]::WriteAllBytes($pdfPath, [Text.Encoding]::ASCII.GetBytes($pdfText))

# Opened with the same component the app opens it with, so "it renders here" and "it renders in
# Glyfo" are the same claim. A hand-built file that no reader accepts is worse than no file at all,
# because the failure would otherwise show up halfway through a screenshot run.
Add-Type -AssemblyName System.Runtime.WindowsRuntime
$null = [Windows.Data.Pdf.PdfDocument, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime]
$null = [Windows.Storage.StorageFile, Windows.Foundation.UniversalApiContract, ContentType=WindowsRuntime]
$asTask = ([System.WindowsRuntimeSystemExtensions].GetMethods() | Where-Object {
    $_.Name -eq 'AsTask' -and $_.GetParameters().Count -eq 1 -and
    $_.GetParameters()[0].ParameterType.Name -eq 'IAsyncOperation`1' })[0]
function Await($operation, [Type]$resultType) {
    $task = $asTask.MakeGenericMethod($resultType).Invoke($null, @($operation))
    [void]$task.Wait(-1)
    $task.Result
}
$file = Await ([Windows.Storage.StorageFile]::GetFileFromPathAsync($pdfPath)) ([Windows.Storage.StorageFile])
$doc = Await ([Windows.Data.Pdf.PdfDocument]::LoadFromFileAsync($file)) ([Windows.Data.Pdf.PdfDocument])
if ($doc.PageCount -ne $pages.Count) { throw "PDF reports $($doc.PageCount) pages, expected $($pages.Count)" }
"report-4471.pdf opens: $($doc.PageCount) pages, first page $($doc.GetPage(0).Size.Width)x$($doc.GetPage(0).Size.Height)"

Get-ChildItem $out | Select-Object Name, Length | Format-Table -AutoSize
