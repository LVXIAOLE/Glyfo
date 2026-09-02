# Reports characters that appear in one language column of a Store-listings CSV and in almost no
# other, plus a per-column inventory of everything outside plain letters and digits.
#
# Written to answer one question: when a single listing is rejected while its neighbours import
# cleanly, is there something in its text that the others do not have?

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path,
    # Report the full inventory for these columns rather than only the rare characters.
    [string[]]$Column
)

$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\ListingCsv.ps1"

$rows = Read-ListingCsv $Path
$header = $rows[0]
$width = $header.Count
if ($Column) { $Column = @($Column | ForEach-Object { $_ -split '[,\s]+' } | Where-Object { $_ }) }

# char -> set of columns it occurs in. Letters and digits are skipped: every language brings its own
# and they say nothing about why an import failed.
$where = @{}
$inventory = @{}
for ($i = 4; $i -lt $width; $i++) {
    $col = $header[$i]
    $inventory[$col] = @{}
    for ($r = 1; $r -lt $rows.Count; $r++) {
        if ($i -ge $rows[$r].Count) { continue }
        foreach ($ch in $rows[$r][$i].ToCharArray()) {
            if ([char]::IsLetterOrDigit($ch) -or $ch -eq ' ') { continue }
            $k = '{0:X4}' -f [int]$ch
            if (-not $where[$k]) { $where[$k] = @{} }
            $where[$k][$col] = 1
            $inventory[$col][$k] = 1 + $inventory[$col][$k]
        }
    }
}

function Show-Char([string]$k) {
    $n = [Convert]::ToInt32($k, 16)
    $ch = [char]$n
    $shown = if ($n -lt 0x20) { '.' } else { $ch }
    '{0} U+{1} {2}' -f $shown, $k, [char]::GetUnicodeCategory($ch)
}

Write-Host 'characters confined to two columns or fewer:'
$rare = 0
foreach ($k in $where.Keys | Sort-Object) {
    $cols = @($where[$k].Keys | Sort-Object)
    if ($cols.Count -le 2) {
        $rare++
        "  {0,-24} {1}" -f (Show-Char $k), ($cols -join ' ')
    }
}
if (-not $rare) { Write-Host '  none' -ForegroundColor Green }

foreach ($c in $Column) {
    Write-Host ''
    Write-Host "inventory for $c :"
    foreach ($k in $inventory[$c].Keys | Sort-Object) {
        "  {0,-24} x{1}" -f (Show-Char $k), $inventory[$c][$k]
    }
}
