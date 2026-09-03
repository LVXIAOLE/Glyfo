# Prints one or more listings from docs\store-listing.md as labelled fields, in the order the
# Partner Center "Store listing" page asks for them.
#
# For typing a listing in by hand. The CSV import is the normal route; this is what you use when a
# listing cannot be imported at all — a language whose listing does not exist yet has no Title, and
# Partner Center will not accept a Title from a CSV, so the first copy of it has to go in by hand.

[CmdletBinding()]
param(
    # Language column names, e.g. -Language "ro pt es". Omit for every language in the file.
    [string[]]$Language,
    [string]$Listing,
    # Write here instead of to the console.
    [string]$OutFile
)

$ErrorActionPreference = 'Stop'

if (-not $Listing) { $Listing = "$PSScriptRoot\..\docs\store-listing.md" }
if ($Language) { $Language = @($Language | ForEach-Object { $_ -split '[,\s]+' } | Where-Object { $_ }) }

# Same map as Fill-ListingCsv.ps1, and for the same reason: the headings in store-listing.md end with
# an English language name, and keeping this file pure ASCII stops PowerShell 5.1 from mangling it.
$Column = @{
    'English (default)' = 'en-us'; 'Chinese (Simplified)' = 'zh-hans'; 'Chinese (Traditional)' = 'zh-hant'
    'Japanese' = 'ja'; 'Korean' = 'ko'; 'German' = 'de'; 'French' = 'fr'; 'Spanish' = 'es'
    'Portuguese (Brazil)' = 'pt'; 'Russian' = 'ru'; 'Italian' = 'it'; 'Polish' = 'pl'; 'Dutch' = 'nl'
    'Czech' = 'cs'; 'Turkish' = 'tr'; 'Swedish' = 'sv'; 'Danish' = 'da'; 'Norwegian' = 'nb'
    'Finnish' = 'fi'; 'Greek' = 'el'; 'Hungarian' = 'hu'; 'Romanian' = 'ro'; 'Ukrainian' = 'uk'
    'Vietnamese' = 'vi'; 'Thai' = 'th'; 'Indonesian' = 'id'; 'Malay' = 'ms'; 'Filipino' = 'fil'
    'Hindi' = 'hi'; 'Bengali' = 'bn'; 'Arabic' = 'ar'; 'Hebrew' = 'he'; 'Persian' = 'fa'
}

$md = [IO.File]::ReadAllText((Resolve-Path $Listing), [Text.Encoding]::UTF8) -split "`r?`n"

$copy = @{}; $head = @{}
$lang = $null; $blocks = $null; $buffer = $null

function Close-Block {
    if ($null -ne $buffer -and $null -ne $blocks) { $blocks.Add(($buffer -join "`n").Trim()) }
}

foreach ($line in $md) {
    if ($line -like '## *') {
        Close-Block
        if ($lang) { $copy[$lang] = $blocks }
        $buffer = $null
        $heading = $line.Substring(3).Trim()
        $name = $Column.Keys | Where-Object { $heading.EndsWith($_) } | Select-Object -First 1
        $lang = $Column[$name]
        if ($lang) { $head[$lang] = $heading }
        $blocks = if ($lang) { New-Object Collections.Generic.List[string] } else { $null }
        continue
    }
    if ($line -like '### *') {
        Close-Block
        $buffer = if ($null -ne $blocks) { New-Object Collections.Generic.List[string] } else { $null }
        continue
    }
    if ($null -ne $buffer) { [void]$buffer.Add($line) }
}
Close-Block
if ($lang) { $copy[$lang] = $blocks }

if (-not $Language) { $Language = @($copy.Keys | Sort-Object) }

$out = New-Object Collections.Generic.List[string]
foreach ($l in $Language) {
    $b = $copy[$l]
    if (-not $b) { throw "$l is not a language in $Listing" }

    # Positional, like the filler: the headings are translated, the order is not.
    $features = @($b[2] -split "`n" | Where-Object { $_ -like '- *' } | ForEach-Object { $_.Substring(2).Trim() })
    $captions = @($b[3] -split "`n" | ForEach-Object {
        if ($_ -match '^\d+\.\s+`[^`]+`\s*\p{Pd}\s*(.+)$') { $Matches[1].Trim() }
    })
    $terms = @([regex]::Matches($b[4], '`([^`]+)`') | ForEach-Object { $_.Groups[1].Value.Trim() })

    $out.Add('=' * 78)
    $out.Add("$l  --  $($head[$l])")
    $out.Add('=' * 78)
    $out.Add('')
    $out.Add('---- Short description --------------------------------------------------------')
    $out.Add($b[0]); $out.Add('')
    $out.Add('---- Description --------------------------------------------------------------')
    $out.Add($b[1]); $out.Add('')
    $out.Add('---- Product features (one box each) ------------------------------------------')
    for ($i = 0; $i -lt $features.Count; $i++) { $out.Add("[$($i + 1)] $($features[$i])") }
    $out.Add('')
    $out.Add('---- Screenshot captions (in upload order) ------------------------------------')
    $names = '01-text-from-a-page.png', '02-any-language.png', '03-qr-and-barcodes.png',
             '04-history.png', '05-settings.png'
    for ($i = 0; $i -lt $captions.Count; $i++) { $out.Add("[$($names[$i])] $($captions[$i])") }
    $out.Add('')
    $out.Add('---- Search terms (one box each) ----------------------------------------------')
    for ($i = 0; $i -lt $terms.Count; $i++) { $out.Add("[$($i + 1)] $($terms[$i])") }
    $out.Add(''); $out.Add('')
}

if ($OutFile) {
    [IO.File]::WriteAllText($OutFile, ($out -join "`r`n"), (New-Object Text.UTF8Encoding $true))
    "wrote $OutFile"
}
else { $out | ForEach-Object { $_ } }
