# Fills the language columns of a Partner Center "Store listings" export from docs\store-listing.md.
#
# The three listings written by hand in Partner Center (en-us, zh-hant, zh-hans) are left untouched —
# whatever is in the export for them wins, because that is what the dashboard actually shows.
#
# Of the thirty remaining columns, seven have translated copy in store-listing.md (de, fr, es, pt, ru,
# ja, ko) and get it. The other twenty-three get the English copy: Partner Center already carries a
# Title for every one of them, so those listings exist, and a listing with a title and no description
# is worse than a listing in English. What a reader sees is the same either way — the Store would have
# fallen back to the English listing anyway — but this way nothing is half-written.
#
# Two files come out, because one question cannot be settled from here:
#
#   *-filled.csv          text plus the image cells, each pointing at the asset URL en-us already
#                         uses. The column type is "relative path (or URL to a Partner Center file)",
#                         so a URL is a legal value, but the URL carries the en-us listing's own id
#                         and Partner Center may refuse to hand that asset to a different listing.
#   *-filled-textonly.csv text only. Every image cell is left exactly as the export had it.
#
# Import the first. If it is rejected over the screenshots or logos, import the second and upload the
# five PNGs and three logos per language in the dashboard.

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path,
    [string]$Listing,
    [string]$OutDir
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
}

# Already written by hand in the dashboard.
$Keep = 'en-us', 'zh-hant', 'zh-hans'

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

# ---------------------------------------------------------------- read the export

$rows = Read-ListingCsv $Path
$header = $rows[0]
$width = $header.Count

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

$targets = @($header[4..($width - 1)] | Where-Object { $_ -notin $Keep })
$translated = @($targets | Where-Object { $copy.ContainsKey($_) })
$english = @($targets | Where-Object { -not $copy.ContainsKey($_) })

"columns to fill: $($targets.Count)"
"  translated : $($translated -join ' ')"
"  English    : $($english -join ' ')"
''

# ---------------------------------------------------------------- fill

$warnings = New-Object Collections.Generic.List[string]

function Set-Cell([string]$field, [string]$lang, [string]$value, [int]$limit) {
    $row = $byField[$field]
    if (-not $row) { throw "no row named $field" }
    if ($limit -and $value.Length -gt $limit) {
        $warnings.Add("$lang $field is $($value.Length) characters, limit is $limit")
    }
    $row[$index[$lang]] = $value
}

$enTitle = $byField['Title'][$index['en-us']]

foreach ($lang in $targets) {
    $c = if ($copy.ContainsKey($lang)) { $copy[$lang] } else { $copy['en-us'] }

    Set-Cell 'Title'            $lang $enTitle 0
    Set-Cell 'ShortDescription' $lang $c.ShortDescription $Limit.ShortDescription
    Set-Cell 'Description'      $lang $c.Description      $Limit.Description

    for ($i = 0; $i -lt 10; $i++) { Set-Cell "Feature$($i + 1)"                  $lang $c.Features[$i]    $Limit.Feature }
    for ($i = 0; $i -lt 5;  $i++) { Set-Cell "DesktopScreenshotCaption$($i + 1)" $lang $c.Captions[$i]    $Limit.Caption }
    for ($i = 0; $i -lt 7;  $i++) { Set-Cell "SearchTerm$($i + 1)"               $lang $c.SearchTerms[$i] $Limit.SearchTerm }
}

foreach ($w in $warnings) { Write-Warning $w }

$base = [IO.Path]::GetFileNameWithoutExtension($Path)
$textOnly = Join-Path $OutDir "$base-filled-textonly.csv"
Write-ListingCsv $textOnly $rows
"wrote $textOnly"

# ---------------------------------------------------------------- and again, with the images

$imageFields = @('DesktopScreenshot1', 'DesktopScreenshot2', 'DesktopScreenshot3', 'DesktopScreenshot4',
                 'DesktopScreenshot5', 'StoreLogo300x300', 'StoreLogoOverride150x150', 'StoreLogoOverride71x71')

foreach ($lang in $targets) {
    foreach ($field in $imageFields) {
        $byField[$field][$index[$lang]] = $byField[$field][$index['en-us']]
    }
    # The three logo overrides only take effect when this is on, and en-us has it on.
    $byField['OverrideLogosForWin10'][$index[$lang]] = $byField['OverrideLogosForWin10'][$index['en-us']]
}

$full = Join-Path $OutDir "$base-filled.csv"
Write-ListingCsv $full $rows
"wrote $full"
