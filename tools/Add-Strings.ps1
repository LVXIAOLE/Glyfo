# Splices a block of new entries into every Services\Strings.*.cs table.
#
# The blocks live in a separate UTF-8 file rather than in this script, because PowerShell 5.1 reads
# a BOM-less .ps1 as ANSI on this machine and would turn every non-Latin translation into mojibake
# before the file was even parsed. This script itself stays pure ASCII.
#
# The input is one "@@<TableName>" marker per language, followed by the lines to insert. TableName is
# the suffix of the file: En, ZhHans, Ar, and so on. Every block is inserted immediately before the
# closing "    };" of that table, so the entries land at the end of the dictionary.
#
# Re-running is safe: a table that already contains the first key in its block is left alone.

[CmdletBinding()]
param(
    [string]$Blocks,
    [string]$Services
)

$ErrorActionPreference = 'Stop'

if (-not $Blocks) { $Blocks = "$PSScriptRoot\news-strings.txt" }
if (-not $Services) { $Services = "$PSScriptRoot\..\Glyfo\Services" }

$text = [IO.File]::ReadAllText((Resolve-Path $Blocks), [Text.Encoding]::UTF8).TrimStart([char]0xFEFF)
$lines = $text -split "`r?`n"

$blockFor = @{}
$name = $null
$buffer = $null

foreach ($line in $lines) {
    if ($line -like '@@*') {
        if ($name) { $blockFor[$name] = $buffer }
        $name = $line.Substring(2).Trim()
        $buffer = New-Object Collections.Generic.List[string]
        continue
    }
    if ($null -ne $buffer -and $line.Trim()) { [void]$buffer.Add($line) }
}
if ($name) { $blockFor[$name] = $buffer }

foreach ($key in ($blockFor.Keys | Sort-Object)) {
    $path = Join-Path $Services "Strings.$key.cs"
    if (-not (Test-Path $path)) { throw "no table file for $key at $path" }

    $body = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)

    # The first entry of the block doubles as the marker for "already applied". Found by scanning
    # rather than taken from line 0, because a block may open with a comment.
    $firstKey = ([regex]::Match(($blockFor[$key] -join "`n"), '\["([^"]+)"\]')).Groups[1].Value
    if ($body -match [regex]::Escape("[""$firstKey""]")) {
        "skip  $key (already has $firstKey)"
        continue
    }

    # LF and no BOM, matching what is already on disk. Rewriting 33 files with CRLF would bury the
    # seventeen real lines in a whole-file diff.
    $insert = ($blockFor[$key] -join "`n")
    $marker = "`n    };"
    $at = $body.LastIndexOf($marker)
    if ($at -lt 0) { throw "no closing brace found in $path" }

    $body = $body.Substring(0, $at) + "`n" + $insert + $body.Substring($at)
    [IO.File]::WriteAllText($path, $body, (New-Object Text.UTF8Encoding $false))
    "wrote $key ($($blockFor[$key].Count) lines)"
}
