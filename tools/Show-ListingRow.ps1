# Prints one row of a Partner Center listing export in full: every language column that has
# something in it, one per line. The inspector only says which columns are filled; this says with
# what, which is what you need before copying a cell's shape into the other languages.
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path,
    # Remaining arguments, so that `-Field A B C` works when this is run through powershell -File,
    # which hands every argument over as its own string and never builds an array from a comma list.
    [Parameter(ValueFromRemainingArguments)][string[]]$Field
)

. "$PSScriptRoot\ListingCsv.ps1"

$rows = Read-ListingCsv $Path
$header = $rows[0]

foreach ($name in $Field) {
    $row = $rows | Where-Object { $_[0] -eq $name } | Select-Object -First 1
    if (-not $row) { "== $name : not found"; continue }
    "== $name (ID $($row[1]), type $($row[2]))"
    for ($i = 3; $i -lt $header.Count; $i++) {
        if ($i -lt $row.Count -and $row[$i]) { "   {0,-8} {1}" -f $header[$i], $row[$i] }
    }
    ''
}
