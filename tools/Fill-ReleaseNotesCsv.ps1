# Fills the ReleaseNotes row of a Partner Center "Store listings" CSV export from
# docs/store-release-notes.md, so the 33 sections are imported in one file instead of pasted into
# the dashboard's "What's new in this version" box 33 times.
#
#   powershell -NoProfile -ExecutionPolicy Bypass -File tools\Fill-ReleaseNotesCsv.ps1 `
#       -Path "C:\Users\you\Downloads\listingData-....csv"
#
# ReleaseNotes IS a row in the export (type "text", empty on a fresh export) -- the same round trip
# that carries the descriptions and search terms carries this. The listing copy is untouched: every
# other row is written back byte for byte as it was read.
#
# This is a separate script from Fill-ListingCsv.ps1 rather than a switch on it, because the two
# source files disagree on almost everything that matters here. store-listing.md is keyed by
# position (five subsections per language, in order) and throws if the count is off;
# store-release-notes.md is a flat list of bullets whose count is allowed to change from release to
# release. Their headings differ too -- see $Column below. Folding them together would mean one
# parser with two modes, and the listing side is the one that must not break.

[CmdletBinding()]
param(
    # The CSV exported from Partner Center > Store listings.
    [Parameter(Mandatory)] [string]$Path,
    # docs/store-release-notes.md. Defaults next to this script.
    [string]$Notes,
    # Where the filled copy goes. Defaults beside $Path.
    [string]$OutDir,
    # Fill only these language columns, e.g. -Only en-us,ja. Everything else keeps its exported
    # value. Useful when an import stalls partway and only some languages need a retry.
    [string[]]$Only
)

$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\ListingCsv.ps1"

# $PSScriptRoot is empty while the param block is being evaluated under powershell -File.
if (-not $Notes)  { $Notes = "$PSScriptRoot\..\docs\store-release-notes.md" }
if (-not $OutDir) { $OutDir = Split-Path -Parent (Resolve-Path $Path) }

# Partner Center's limit for this field. Characters, not bytes -- a Japanese section counts the same
# as an English one of the same length.
$MaxLength = 1500

# store-release-notes.md heads each section with the language's own name followed by its English
# name after an em dash, so matching on the English tail keeps this file pure ASCII (PowerShell 5.1
# reads a .ps1 with no BOM as ANSI, and a CJK literal in here would arrive mangled).
#
# These are NOT the same headings as store-listing.md's, so $Column cannot be shared with
# Fill-ListingCsv.ps1: there it is "Portuguese (Brazil)" and plain "Norwegian", here the tail is
# just "Portuguese" and "Norwegian" after "Norsk bokmal". Sharing one table would silently drop
# whichever file disagreed.
$Column = @{
    'English (default)' = 'en-us'
    'Arabic'            = 'ar'
    'Bengali'           = 'bn'
    'Czech'             = 'cs'
    'Danish'            = 'da'
    'German'            = 'de'
    'Greek'             = 'el'
    'Spanish'           = 'es'
    'Persian'           = 'fa'
    'Finnish'           = 'fi'
    'Filipino'          = 'fil'
    'French'            = 'fr'
    'Hebrew'            = 'he'
    'Hindi'             = 'hi'
    'Hungarian'         = 'hu'
    'Indonesian'        = 'id'
    'Italian'           = 'it'
    'Japanese'          = 'ja'
    'Korean'            = 'ko'
    'Malay'             = 'ms'
    'Norwegian'         = 'nb'
    'Dutch'             = 'nl'
    'Polish'            = 'pl'
    'Portuguese'        = 'pt'
    'Romanian'          = 'ro'
    'Russian'           = 'ru'
    'Swedish'           = 'sv'
    'Thai'              = 'th'
    'Turkish'           = 'tr'
    'Ukrainian'         = 'uk'
    'Vietnamese'        = 'vi'
}

# The two Chinese sections are the exception: their headings are written in Chinese with no English
# name at all, so there is nothing ASCII to match. Distinguish them by the one character that
# differs, built from its code point to keep this file ASCII.
$HanSimplified  = [char]0x7B80   # the first character of "jian ti"
$HanTraditional = [char]0x7E41   # the first character of "fan ti"

function Resolve-Column([string]$heading) {
    if ($heading.IndexOf($HanSimplified) -ge 0)  { return 'zh-hans' }
    if ($heading.IndexOf($HanTraditional) -ge 0) { return 'zh-hant' }
    foreach ($name in $Column.Keys) {
        if ($heading.EndsWith($name)) { return $Column[$name] }
    }
    return $null
}

# --- read the source ---------------------------------------------------------------------------

$md = [IO.File]::ReadAllText((Resolve-Path $Notes), [Text.Encoding]::UTF8)

$notesFor = @{}
foreach ($section in @($md -split '(?m)^## ')) {
    $lines = @($section -split "`r?`n")
    $code = Resolve-Column $lines[0].Trim()
    if (-not $code) { continue }
    if ($notesFor.ContainsKey($code)) { throw "$code : two sections map to the same column" }

    # Every bullet, verbatim including the "- ". The dashboard box is plain text, so the marker is
    # what puts a bullet in front of each line for the customer reading it; dropping it would run
    # the twelve features together as one paragraph.
    $bullets = @($lines | Where-Object { $_ -match '^\s*-\s' } | ForEach-Object { $_.Trim() })
    if ($bullets.Count -eq 0) { throw "$code : section has no bullets" }

    # Bare LF, which is what Partner Center's own export uses inside a multi-line cell. Write-
    # ListingCsv quotes the cell for us.
    $notesFor[$code] = ($bullets -join "`n")
}

$missing = @($Column.Values + @('zh-hans', 'zh-hant') | Where-Object { -not $notesFor.ContainsKey($_) })
if ($missing.Count) { throw "no section in $Notes for: $($missing -join ', ')" }

# --- check before writing anything ---------------------------------------------------------------

# Checked here rather than at write time so a file Partner Center would reject is never produced.
# An over-long cell does not fail the language it is in and continue -- the import stops, and every
# language after it is silently left unwritten.
foreach ($code in $notesFor.Keys) {
    $len = $notesFor[$code].Length
    if ($len -gt $MaxLength) {
        throw "$code : release notes are $len characters, Partner Center allows $MaxLength"
    }
}

# --- write ---------------------------------------------------------------------------------------

$rows = Read-ListingCsv (Resolve-Path $Path)
$hdr = $rows[0]

# Do not pipe $rows: the pipeline unrolls a List[string[]] into rows and then each row into its
# individual characters.
$notesRow = $null
foreach ($row in $rows) { if ($row[0] -eq 'ReleaseNotes') { $notesRow = $row } }
if (-not $notesRow) { throw "no ReleaseNotes row in $Path" }

$written = @()
$skipped = @()
foreach ($code in $notesFor.Keys) {
    $c = [Array]::IndexOf($hdr, $code)
    if ($c -lt 0) { $skipped += $code; continue }
    if ($Only -and $Only -notcontains $code) { $skipped += $code; continue }
    $notesRow[$c] = $notesFor[$code]
    $written += $code
}

$dst = Join-Path $OutDir ((Split-Path -Leaf $Path) -replace '\.csv$', '-notes.csv')
Write-ListingCsv $dst $rows

"wrote $($written.Count) languages: $(($written | Sort-Object) -join ' ')"
if ($skipped.Count) { "skipped $($skipped.Count): $(($skipped | Sort-Object) -join ' ')" }
"-> $dst"
