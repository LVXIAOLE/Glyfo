# Prints the shape of a Partner Center "Store listings" export: which languages it carries columns
# for, and for every row which of them already have something written in.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Path)

. "$PSScriptRoot\ListingCsv.ps1"

$rows = Read-ListingCsv $Path
$header = $rows[0]
"columns ($($header.Count)): $($header -join ' ')"
"rows: $($rows.Count - 1)"
''

$langStart = 4   # Field, ID, Type, default, then one column per language
foreach ($row in $rows[1..($rows.Count - 1)]) {
    $filled = @()
    for ($i = $langStart; $i -lt $header.Count; $i++) {
        if ($i -lt $row.Count -and $row[$i]) { $filled += $header[$i] }
    }
    "{0,-26} {1,-22} len(default)={2,-5} filled({3}): {4}" -f `
        $row[0], $row[1], $row[3].Length, $filled.Count, ($filled -join ' ')
}
