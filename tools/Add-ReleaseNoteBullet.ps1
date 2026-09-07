# Adds one bullet to every language section of docs/store-release-notes.md from a payload file.
#
# The same reasoning as Set-SearchTerms.ps1: thirty-three hand edits across Greek, Thai, Bengali and
# Arabic is thirty-three chances to corrupt copy nobody in the room can proofread, and PowerShell 5.1
# defaults to the ANSI code page on both read and write unless the encoding is spelled out.
#
# The bullet goes in as the *first* one of its section rather than appended. The list runs 1.3.0's
# features first and the field is read top-down by someone deciding whether to care about the update,
# so the item about recognition accuracy -- the thing the app is for -- belongs where it will be read.
# Inserting at the top is also a pure insertion: no existing bullet moves relative to another, so the
# diff shows thirty-three added lines and nothing else.

[CmdletBinding()]
param(
    [string]$Notes,
    [string]$Payload,
    [switch]$WhatIfOnly
)

$ErrorActionPreference = 'Stop'

# Resolved in the body: $PSScriptRoot is not populated while the param block binds under PS 5.1.
if (-not $Notes) { $Notes = Join-Path $PSScriptRoot '..\docs\store-release-notes.md' }
if (-not $Payload) { $Payload = Join-Path $PSScriptRoot 'release-note-ocr.txt' }

$utf8 = New-Object System.Text.UTF8Encoding($false)

function Read-Lines([string]$path) {
    if (-not (Test-Path $path)) { throw "Missing $path" }
    [System.IO.File]::ReadAllText($path, $utf8) -replace "`r`n", "`n" -split "`n"
}

$bullets = [ordered]@{}
$heading = $null
foreach ($line in (Read-Lines $Payload)) {
    if ($line -like '## *') { $heading = $line; continue }
    if ($heading -and $line.Trim()) { $bullets[$heading] = $line.TrimEnd(); $heading = $null }
}
Write-Output "payload: $($bullets.Count) languages"

$lines = Read-Lines $Notes
$output = New-Object System.Collections.Generic.List[string]

$pending = $null
$applied = 0

foreach ($line in $lines) {
    if ($line -like '## *') {
        if (-not $bullets.Contains($line)) { throw "No payload for heading: $line" }
        $pending = $bullets[$line]
        $bullets[$line] = $null   # marks it used, for the completeness check below
    }

    # The first bullet of the section is where the new one goes, immediately above it.
    if ($pending -and $line.StartsWith('- ')) {
        $output.Add($pending)
        $pending = $null
        $applied++
    }

    $output.Add($line)
}

if ($pending) { throw "Section had no bullets to insert before" }

$unused = @($bullets.Keys | Where-Object { $null -ne $bullets[$_] })
if ($unused.Count) { throw "Payload headings not found in notes:`n  " + ($unused -join "`n  ") }

Write-Output "inserted: $applied bullets"

if ($WhatIfOnly) { return }

$text = ($output -join "`n")
if (-not $text.EndsWith("`n")) { $text += "`n" }
[System.IO.File]::WriteAllText($Notes, $text, $utf8)
Write-Output "wrote: $Notes"
