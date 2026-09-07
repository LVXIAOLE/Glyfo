# Fills the language columns of a Partner Center "Store listings" export from docs\store-listing.md.
#
# The three listings written by hand in Partner Center (en-us, zh-hant, zh-hans) are left untouched —
# whatever is in the export for them wins, because that is what the dashboard actually shows. That
# held while store-listing.md was a transcription of the dashboard. It stopped holding at 1.3.0,
# when the copy here was rewritten for a release the dashboard has never seen: leaving those three
# out now would ship thirty listings describing PDFs, batches and a searchable history alongside
# three that still describe 1.1.0. -IncludeAuthored fills them too, from this same file.
#
# The thirty remaining columns all have translated copy in store-listing.md and get it. The fallback
# to English further down is therefore dead code now; it stays because it is what should happen if a
# language is ever added to the package before its copy is written, and a silently English listing
# beats a silently empty one.
#
# Two files come out, because one question cannot be settled from here:
#
#   *-filled.csv          text plus the image cells, each pointing at the asset URL en-us already
#                         uses. The column type is "relative path (or URL to a Partner Center file)",
#                         so a URL is a legal value, but the URL carries the en-us listing's own id
#                         and Partner Center may refuse to hand that asset to a different listing.
#   *-filled-textonly.csv text only. Every image cell keeps the asset it already had, though not
#                         necessarily in the slot it had it in — see the screenshot-slot section.
#
# Import the first. If it is rejected over the screenshots or logos, import the second and upload the
# five PNGs and three logos per language in the dashboard.

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path,
    [string]$Listing,
    [string]$OutDir,
    # Fill only these language columns; every other column keeps whatever the export had. Use this
    # when the dashboard stalls on a whole-file import — a batch is a no-op for the columns it does
    # not name, so batches can be imported one at a time and re-imported safely.
    [string[]]$Only,
    # Same thing, cut automatically: -BatchSize 8 writes ceil(30/8) pairs of files instead of one.
    [int]$BatchSize,
    # Also fill en-us, zh-hant and zh-hans, the three that were typed into the dashboard by hand.
    [switch]$IncludeAuthored,
    # Added to the output file names, so two runs over the same export do not overwrite each other.
    [string]$Suffix
)

$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\ListingCsv.ps1"

# $PSScriptRoot is empty while the param block is being evaluated under powershell -File, so the
# default for -Listing has to be filled in here instead.
if (-not $Listing) { $Listing = "$PSScriptRoot\..\docs\store-listing.md" }
if (-not $OutDir) { $OutDir = Split-Path -Parent (Resolve-Path $Path) }

# The heading of every language section in store-listing.md ends with an English name after an em
# dash; the English section itself is just "English (default)". Matching on that keeps this file
# pure ASCII — PowerShell 5.1 reads a .ps1 with no BOM as ANSI, and a CJK heading literal in here
# would arrive mangled.
$Column = @{
    'English (default)'      = 'en-us'
    'Chinese (Simplified)'   = 'zh-hans'
    'Chinese (Traditional)'  = 'zh-hant'
    'Japanese'               = 'ja'
    'Korean'                 = 'ko'
    'German'                 = 'de'
    'French'                 = 'fr'
    'Spanish'                = 'es'
    'Portuguese (Brazil)'    = 'pt'
    'Russian'                = 'ru'
    'Italian'                = 'it'
    'Polish'                 = 'pl'
    'Dutch'                  = 'nl'
    'Czech'                  = 'cs'
    'Turkish'                = 'tr'
    'Swedish'                = 'sv'
    'Danish'                 = 'da'
    'Norwegian'              = 'nb'
    'Finnish'                = 'fi'
    'Greek'                  = 'el'
    'Hungarian'              = 'hu'
    'Romanian'               = 'ro'
    'Ukrainian'              = 'uk'
    'Vietnamese'             = 'vi'
    'Thai'                   = 'th'
    'Indonesian'             = 'id'
    'Malay'                  = 'ms'
    'Filipino'               = 'fil'
    'Hindi'                  = 'hi'
    'Bengali'                = 'bn'
    'Arabic'                 = 'ar'
    'Hebrew'                 = 'he'
    'Persian'                = 'fa'
}

# Already written by hand in the dashboard.
$Keep = if ($IncludeAuthored) { @() } else { 'en-us', 'zh-hant', 'zh-hans' }
$Authored = 'en-us', 'zh-hant', 'zh-hans'

# Partner Center's own limits, checked below rather than trusted.
$Limit = @{ ShortDescription = 1000; Description = 10000; Feature = 200; Caption = 200; SearchTerm = 30 }

# ---------------------------------------------------------------- read the copy

$md = [IO.File]::ReadAllText((Resolve-Path $Listing), [Text.Encoding]::UTF8) -split "`r?`n"

$copy = @{}          # column -> @{ Short; Description; Features; Captions; Terms }
$lang = $null
$blocks = $null      # the ### sections of the current language, in file order
$buffer = $null

function Close-Block {
    if ($null -ne $buffer -and $null -ne $blocks) {
        $blocks.Add(($buffer -join "`n").Trim())
    }
}

foreach ($line in $md) {
    if ($line -like '## *') {
        Close-Block
        if ($lang) { $copy[$lang] = $blocks }
        $buffer = $null

        $heading = $line.Substring(3).Trim()
        $name = $Column.Keys | Where-Object { $heading.EndsWith($_) } | Select-Object -First 1
        $lang = $Column[$name]
        $blocks = if ($lang) { New-Object Collections.Generic.List[string] } else { $null }
        continue
    }
    if ($line -like '### *') {
        Close-Block
        # -ne $null, not a plain truth test: an empty List is falsy in PowerShell, and $blocks is
        # empty at exactly the moment the first ### of a language arrives.
        $buffer = if ($null -ne $blocks) { New-Object Collections.Generic.List[string] } else { $null }
        continue
    }
    if ($null -ne $buffer) { [void]$buffer.Add($line) }
}
Close-Block
if ($lang) { $copy[$lang] = $blocks }

foreach ($key in @($copy.Keys)) {
    $b = $copy[$key]
    if ($b.Count -ne 5) { throw "$key : expected 5 sections in store-listing.md, found $($b.Count)" }

    # Sections are keyed by position, not by name — the headings are translated, the order is not.
    $features = @($b[2] -split "`n" | Where-Object { $_ -like '- *' } | ForEach-Object { $_.Substring(2).Trim() })
    $captions = @($b[3] -split "`n" | ForEach-Object {
        # \p{Pd} is the dash that separates the file name from the caption. Spelled as a category
        # rather than as the character so that nothing in this file depends on how it is decoded.
        if ($_ -match '^\d+\.\s+`[^`]+`\s*\p{Pd}\s*(.+)$') { $Matches[1].Trim() }
    })
    $terms = @([regex]::Matches($b[4], '`([^`]+)`') | ForEach-Object { $_.Groups[1].Value.Trim() })

    if ($features.Count -ne 10) { throw "$key : expected 10 features, found $($features.Count)" }
    if ($captions.Count -ne 5)  { throw "$key : expected 5 captions, found $($captions.Count)" }
    if ($terms.Count -ne 7)     { throw "$key : expected 7 search terms, found $($terms.Count)" }

    $copy[$key] = @{
        ShortDescription = $b[0]
        Description      = $b[1]
        Features         = $features
        Captions         = $captions
        SearchTerms      = $terms
    }
}

if ($copy.Count -ne $Column.Count) {
    throw "parsed $($copy.Count) languages out of $($Column.Count) from $Listing"
}
"parsed store-listing.md: $($copy.Count) languages ($(($copy.Keys | Sort-Object) -join ' '))"

# ---------------------------------------------------------------- work out the target columns

$probe = Read-ListingCsv $Path
$header = $probe[0]
$width = $header.Count

$targets = @($header[4..($width - 1)] | Where-Object { $_ -notin $Keep })
if ($Only) {
    # Split on commas and spaces. Under powershell -File every argument arrives as one literal
    # string, so -Only es,pt,it is a single element here, not three.
    $Only = @($Only | ForEach-Object { $_ -split '[,\s]+' } | Where-Object { $_ })
    $unknown = @($Only | Where-Object { $_ -notin $header })
    if ($unknown) { throw "-Only names columns that are not in the export: $($unknown -join ' ')" }
    $targets = @($targets | Where-Object { $_ -in $Only })
    if (-not $targets) { throw "-Only selected no fillable column (en-us, zh-hant and zh-hans need -IncludeAuthored)" }
}

"columns to fill: $($targets.Count)"
"  translated : $(@($targets | Where-Object { $copy.ContainsKey($_) }) -join ' ')"
"  English    : $(@($targets | Where-Object { -not $copy.ContainsKey($_) }) -join ' ')"
''

$imageFields = @('DesktopScreenshot1', 'DesktopScreenshot2', 'DesktopScreenshot3', 'DesktopScreenshot4',
                 'DesktopScreenshot5', 'StoreLogo300x300', 'StoreLogoOverride150x150', 'StoreLogoOverride71x71')

$base = [IO.Path]::GetFileNameWithoutExtension($Path)

# ---------------------------------------------------------------- screenshot slots

# The five PNGs are the same five in every listing, but the slot each one sits in is not, and writing
# captions from store-listing.md into a listing whose images are in a different order puts the
# QR-code caption under the settings dialog. So the slot order is worked out first and the image
# cells are moved to match, rather than the captions being bent to fit.
#
# Which slot holds which shot has to be answered two different ways, because the three hand-made
# listings and the thirty machine-filled ones carry different evidence.
#
# en-us, zh-hant and zh-hans each own five uploaded PNGs and had their captions typed next to them in
# the dashboard, so their captions are the record of the pairing: matching each live caption against
# store-listing.md gives slot -> shot outright. Today that reads en-us as 02, 05, 03, 04, 01;
# zh-hant as 01, 04, 02, 03, 05; zh-hans as 02, 03, 01, 04, 05.
#
# For the other thirty the captions prove nothing at all, and believing them is how this went wrong.
# Earlier runs wrote captions in file order 01..05 while copying en-us's image cells slot for slot —
# so a listing's captions say "01..05" whatever its images actually are, and en-us's own order
# drifted between runs. The export bears this out: reading the asset ids rather than the captions,
# de/ru/fr/ja/ar/bn/cs/da/el/fa/fi/fil/he/hi/hu are 1 2 3 4 5, id is 1 3 4 5 2, it/ko/ms are
# 2 5 3 4 1, and nb/pl/sv/th/tr/uk/vi are 5 1 3 4 2. Twelve of those are mispaired in the dashboard
# right now — caption 1 sitting under shot 5 for seven of them — and no caption comparison can see
# it, because every one of those captions is exactly where the file said to put it.
#
# What those thirty do carry is en-us's own asset ids, so the asset id says which shot a cell holds
# with no inference at all. That is the derivation used for them. It degrades to the caption route
# when a listing has assets en-us does not (pt, es and ro were typed in by hand after the imports
# kept failing, and uploaded their own copies), and anything neither route can settle is left alone
# and reported.
function Get-SlotOrderByCaption($byField, $index, [string]$lang) {
    if (-not $copy.ContainsKey($lang)) { return $null }
    $wanted = $copy[$lang].Captions
    $order = New-Object int[] 5
    for ($s = 0; $s -lt 5; $s++) {
        $live = $byField["DesktopScreenshotCaption$($s + 1)"][$index[$lang]]
        $k = [Array]::IndexOf($wanted, $live)
        if ($k -lt 0) { return $null }
        $order[$s] = $k
    }
    if ((@($order | Sort-Object) -join ',') -ne '0,1,2,3,4') { return $null }
    , $order
}

# The cell is a full dashboard URL and it embeds the listing's own id, so two listings showing the
# same PNG do not have the same string here. Only the last segment, the asset id, is shared.
function Get-AssetId([string]$url) {
    if (-not $url) { return '' }
    $url.Substring($url.LastIndexOf('/') + 1)
}

function Get-SlotOrderByAsset($byField, $index, [string]$lang, $shotOfAsset) {
    $order = New-Object int[] 5
    for ($s = 0; $s -lt 5; $s++) {
        $id = Get-AssetId $byField["DesktopScreenshot$($s + 1)"][$index[$lang]]
        if (-not $shotOfAsset.ContainsKey($id)) { return $null }
        $order[$s] = $shotOfAsset[$id]
    }
    if ((@($order | Sort-Object) -join ',') -ne '0,1,2,3,4') { return $null }
    , $order
}

# ---------------------------------------------------------------- fill

# The export is re-read for every batch rather than reused. A batch has to leave the columns it does
# not name byte-identical to the export, and the cheapest way to guarantee that is to start from the
# export each time instead of trying to undo the previous batch's writes.
function Write-Filled([string[]]$langs, [string]$suffix) {
    $rows = Read-ListingCsv $Path

    $index = @{}
    for ($i = 0; $i -lt $width; $i++) { $index[$header[$i]] = $i }

    $byField = @{}
    for ($r = 1; $r -lt $rows.Count; $r++) {
        # Pad short rows once, here, so every write below can address any column.
        if ($rows[$r].Count -lt $width) {
            $padded = New-Object string[] $width
            [Array]::Copy($rows[$r], $padded, $rows[$r].Count)
            for ($c = $rows[$r].Count; $c -lt $width; $c++) { $padded[$c] = '' }
            $rows[$r] = $padded
        }
        $byField[$rows[$r][0]] = $rows[$r]
    }

    $warnings = New-Object Collections.Generic.List[string]
    $set = {
        param([string]$field, [string]$lang, [string]$value, [int]$limit)
        $row = $byField[$field]
        if (-not $row) { throw "no row named $field" }
        if ($limit -and $value.Length -gt $limit) {
            $warnings.Add("$lang $field is $($value.Length) characters, limit is $limit")
        }
        $row[$index[$lang]] = $value
    }

    $enTitle = $byField['Title'][$index['en-us']]

    # en-us first and unconditionally: it is both the one listing whose captions are trustworthy
    # evidence and the source every other listing's images are copied from. Read off the captions
    # still in the export, so it has to happen before those captions are overwritten below.
    $enOrder = Get-SlotOrderByCaption $byField $index 'en-us'
    if (-not $enOrder) { throw "en-us : cannot work out which screenshot is in which slot, so the other listings cannot be pointed at them" }

    # Reordered into a local array whether or not en-us is itself being filled: without this, a
    # re-run would push en-us's own hand-made slot order onto thirty listings as if it were shot
    # order. $shotOfAsset is the same mapping keyed the other way, for the thirty.
    $enShots = New-Object string[] 5
    $shotOfAsset = @{}
    for ($s = 0; $s -lt 5; $s++) {
        $cell = $byField["DesktopScreenshot$($s + 1)"][$index['en-us']]
        $enShots[$enOrder[$s]] = $cell
        $shotOfAsset[(Get-AssetId $cell)] = $enOrder[$s]
    }

    $slotOrder = @{ 'en-us' = $enOrder }
    foreach ($lang in $langs) {
        if ($lang -eq 'en-us') { continue }
        $order = if ($lang -in $Authored) { Get-SlotOrderByCaption $byField $index $lang }
                 else { Get-SlotOrderByAsset $byField $index $lang $shotOfAsset }
        # pt, es and ro uploaded their own copies, so the asset ids are no help; their captions were
        # typed in by hand next to them, which puts them in the same position as the three above.
        if (-not $order -and $lang -notin $Authored) { $order = Get-SlotOrderByCaption $byField $index $lang }
        if ($order) { $slotOrder[$lang] = $order }
        else { $warnings.Add("$lang : screenshot slots left as they are, neither its asset ids nor its captions place them") }
    }

    foreach ($lang in $langs) {
        if (-not $slotOrder.ContainsKey($lang)) { continue }
        $old = @(1..5 | ForEach-Object { $byField["DesktopScreenshot$_"][$index[$lang]] })
        if (@($old | Where-Object { $_ }).Count -ne 5) { continue }   # nothing uploaded yet
        for ($s = 0; $s -lt 5; $s++) {
            $byField["DesktopScreenshot$($slotOrder[$lang][$s] + 1)"][$index[$lang]] = $old[$s]
        }
        $moved = ($slotOrder[$lang] -join '') -ne '01234'
        if ($moved) { "  $lang : screenshots reordered to 01..05" }
    }

    foreach ($lang in $langs) {
        $c = if ($copy.ContainsKey($lang)) { $copy[$lang] } else { $copy['en-us'] }

        & $set 'Title'            $lang $enTitle 0
        & $set 'ShortDescription' $lang $c.ShortDescription $Limit.ShortDescription
        & $set 'Description'      $lang $c.Description      $Limit.Description

        for ($i = 0; $i -lt 10; $i++) { & $set "Feature$($i + 1)"                  $lang $c.Features[$i]    $Limit.Feature }
        for ($i = 0; $i -lt 5;  $i++) { & $set "DesktopScreenshotCaption$($i + 1)" $lang $c.Captions[$i]    $Limit.Caption }
        for ($i = 0; $i -lt 7;  $i++) { & $set "SearchTerm$($i + 1)"               $lang $c.SearchTerms[$i] $Limit.SearchTerm }
    }

    foreach ($w in $warnings) { Write-Warning $w }

    $textOnly = Join-Path $OutDir "$base-filled$suffix-textonly.csv"
    Write-ListingCsv $textOnly $rows
    "wrote $textOnly"

    # And again, with the images pointed at the assets en-us already uses — in shot order, not in
    # the order en-us happens to show them.
    #
    # The three hand-made listings are skipped here even when they are being filled: they already
    # own five uploaded PNGs and three logos apiece, and swapping those for en-us's assets would
    # trade something Partner Center is certain to accept for something it may not.
    foreach ($lang in $langs) {
        if ($lang -in $Authored) { continue }
        for ($s = 0; $s -lt 5; $s++) {
            $byField["DesktopScreenshot$($s + 1)"][$index[$lang]] = $enShots[$s]
        }
        foreach ($field in $imageFields | Where-Object { $_ -notlike 'DesktopScreenshot*' }) {
            $byField[$field][$index[$lang]] = $byField[$field][$index['en-us']]
        }
        # The three logo overrides only take effect when this is on, and en-us has it on.
        $byField['OverrideLogosForWin10'][$index[$lang]] = $byField['OverrideLogosForWin10'][$index['en-us']]
    }

    $full = Join-Path $OutDir "$base-filled$suffix.csv"
    Write-ListingCsv $full $rows
    "wrote $full"
}

if ($BatchSize -gt 0) {
    $n = [Math]::Ceiling($targets.Count / $BatchSize)
    for ($b = 0; $b -lt $n; $b++) {
        $slice = @($targets | Select-Object -Skip ($b * $BatchSize) -First $BatchSize)
        "batch $($b + 1)/$n : $($slice -join ' ')"
        Write-Filled $slice "$Suffix-b$($b + 1)"
        ''
    }
}
else {
    Write-Filled $targets $Suffix
}
