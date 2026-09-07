# Replaces the "search terms" subsection of every language in docs/store-listing.md from a payload
# file, so the 33 blocks are rewritten in one auditable pass instead of 33 hand edits.
#
# The contract this leans on is the one Fill-ListingCsv.ps1 already enforces: inside a "## <language>"
# block the five "###" subsections are keyed by position, and the fifth is the search terms. So a
# language is located by its exact "## " heading line and the terms are whatever backtick lines sit
# under its fifth "###". English spills its seven terms over two lines today, which is why the whole
# run of backtick lines is replaced rather than one line rewritten in place.
#
# Written and read as UTF-8 without BOM. The file is full of Greek, Thai, Bengali and Arabic; letting
# PowerShell 5.1 default to the ANSI code page on either side would quietly destroy all of it, and
# Set-Content -Encoding UTF8 emits a BOM that git then reports as a change on every byte of line 1.

[CmdletBinding()]
param(
    [string]$Listing,
    [string]$Payload,
    [switch]$WhatIfOnly
)

$ErrorActionPreference = 'Stop'

# Resolved in the body, not as parameter defaults: $PSScriptRoot is not populated while the param
# block is being bound under Windows PowerShell 5.1, so a default referencing it comes out relative.
if (-not $Listing) { $Listing = Join-Path $PSScriptRoot '..\docs\store-listing.md' }
if (-not $Payload) { $Payload = Join-Path $PSScriptRoot 'search-terms.txt' }

$utf8 = New-Object System.Text.UTF8Encoding($false)

function Read-Lines([string]$path) {
    if (-not (Test-Path $path)) { throw "Missing $path" }
    # ReadAllText plus a manual split rather than ReadAllLines, so a file with no trailing newline
    # round-trips to exactly what it was instead of gaining one.
    [System.IO.File]::ReadAllText($path, $utf8) -replace "`r`n", "`n" -split "`n"
}

# The payload is blocks of "## heading" then one line of terms, separated by blank lines.
$terms = [ordered]@{}
$heading = $null
foreach ($line in (Read-Lines $Payload)) {
    if ($line -like '## *') { $heading = $line; continue }
    if ($heading -and $line.Trim()) { $terms[$heading] = $line.Trim(); $heading = $null }
}
Write-Output "payload: $($terms.Count) languages"

$lines = Read-Lines $Listing
$output = New-Object System.Collections.Generic.List[string]

$currentHeading = $null   # the "## " block we are inside
$subsection = 0           # how many "###" we have passed inside it
$replacing = $false       # true while dropping the old terms lines
$pending = $null          # the replacement, held until the old lines are gone
$applied = 0

foreach ($line in $lines) {
    if ($replacing) {
        # Inside the old terms block. Blank lines and backtick lines are part of it (English spills
        # onto a second line, and there is a blank between the heading and the first one). The first
        # line that is neither ends it, so prose after the terms and the next heading both survive.
        if ($line.Trim() -eq '' -or $line.StartsWith('`')) { continue }

        $output.Add('')
        $output.Add($pending)
        $output.Add('')
        $replacing = $false
        $pending = $null
    }

    if ($line -like '## *' -and $line -notlike '### *') {
        $currentHeading = $line
        $subsection = 0
    }
    elseif ($line -like '### *') {
        $subsection++
    }

    $output.Add($line)

    if ($subsection -eq 5 -and $line -like '### *' -and $currentHeading) {
        if (-not $terms.Contains($currentHeading)) {
            throw "No payload for heading: $currentHeading"
        }
        $pending = $terms[$currentHeading]
        $replacing = $true
        $applied++
        $terms[$currentHeading] = $null   # marks it used, for the completeness check below
    }
}

# The last language's terms run to the end of the file, so nothing terminates them.
if ($replacing) {
    $output.Add('')
    $output.Add($pending)
}

# Every heading in the payload must have matched a heading in the listing. A typo in either file
# would otherwise show up as a language silently keeping its old terms.
$unused = @($terms.Keys | Where-Object { $null -ne $terms[$_] })
if ($unused.Count) { throw "Payload headings not found in listing:`n  " + ($unused -join "`n  ") }

Write-Output "rewrote: $applied languages"

if ($WhatIfOnly) { return }

$text = ($output -join "`n")
if (-not $text.EndsWith("`n")) { $text += "`n" }
[System.IO.File]::WriteAllText($Listing, $text, $utf8)
Write-Output "wrote: $Listing"
