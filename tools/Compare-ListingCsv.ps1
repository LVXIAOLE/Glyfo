# Reports which language columns differ between two Store-listings CSVs, and by how many cells.
#
# The point is to prove a batch file touches only the columns it claims to: everything else must be
# byte-identical to the export, or importing the batch would quietly rewrite a listing that was
# already correct.

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Left,
    [Parameter(Mandatory)][string]$Right
)

$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\ListingCsv.ps1"

$a = Read-ListingCsv $Left
$b = Read-ListingCsv $Right

if (($a[0] -join ',') -ne ($b[0] -join ',')) { throw 'headers differ' }
if ($a.Count -ne $b.Count) { throw "record count differs: $($a.Count) vs $($b.Count)" }

$header = $a[0]
$width = $header.Count

# Cell-by-cell, tolerating the short rows the export contains and the padded ones the filler writes.
$diff = @{}
for ($r = 1; $r -lt $a.Count; $r++) {
    for ($i = 0; $i -lt $width; $i++) {
        $l = if ($i -lt $a[$r].Count) { $a[$r][$i] } else { '' }
        $x = if ($i -lt $b[$r].Count) { $b[$r][$i] } else { '' }
        if ($l -ne $x) { $diff[$header[$i]] = 1 + $diff[$header[$i]] }
    }
}

if (-not $diff.Count) { Write-Host 'identical' -ForegroundColor Green; return }
Write-Host "columns changed: $($diff.Count)"
foreach ($k in $diff.Keys | Sort-Object) { "  {0,-8} {1,3} cells" -f $k, $diff[$k] }
