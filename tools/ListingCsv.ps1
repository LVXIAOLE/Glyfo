# Reader and writer for a Partner Center "Store listings" CSV export.
#
# Dot-source this; it defines Read-ListingCsv and Write-ListingCsv.
#
# Import-Csv cannot be used on these files. One of the columns is the Indonesian listing, whose
# BCP-47 tag is "id", and PowerShell matches property names case-insensitively — so it collides with
# the export's own "ID" column and the import fails outright with "the member id is already
# present". Everything here works on plain string arrays instead, which also keeps the column order
# exactly as Partner Center wrote it. Partner Center is strict about that on the way back in.

function Read-ListingCsv([string]$Path) {
    $text = [IO.File]::ReadAllText($Path, [Text.Encoding]::UTF8).TrimStart([char]0xFEFF)

    $rows = New-Object Collections.Generic.List[string[]]
    $field = New-Object Text.StringBuilder
    $row = New-Object Collections.Generic.List[string]
    $quoted = $false

    for ($i = 0; $i -lt $text.Length; $i++) {
        $c = $text[$i]
        if ($quoted) {
            if ($c -eq '"') {
                if ($i + 1 -lt $text.Length -and $text[$i + 1] -eq '"') { [void]$field.Append('"'); $i++ }
                else { $quoted = $false }
            }
            else { [void]$field.Append($c) }
            continue
        }

        switch ($c) {
            '"' { $quoted = $true }
            ',' { $row.Add($field.ToString()); [void]$field.Clear() }
            "`r" { }
            "`n" {
                $row.Add($field.ToString()); [void]$field.Clear()
                $rows.Add($row.ToArray()); $row.Clear()
            }
            default { [void]$field.Append($c) }
        }
    }
    if ($field.Length -or $row.Count) {
        $row.Add($field.ToString())
        $rows.Add($row.ToArray())
    }

    , $rows
}

function Write-ListingCsv([string]$Path, $Rows) {
    $sb = New-Object Text.StringBuilder
    $first = $true
    foreach ($row in $Rows) {
        # CRLF between records, none after the last one. Partner Center's own export ends without a
        # terminator, and a trailing one is an empty final record to a strict parser. Nothing else in
        # a round trip of an untouched export differs from the export by even one byte.
        if (-not $first) { [void]$sb.Append("`r`n") }
        $first = $false
        $cells = foreach ($cell in $row) {
            if ($cell -match '[",\r\n]') { '"' + $cell.Replace('"', '""') + '"' } else { $cell }
        }
        [void]$sb.Append(($cells -join ','))
    }
    # With the BOM: Partner Center reads these as UTF-8 only when it is there, and without it every
    # non-Latin listing comes back as mojibake.
    [IO.File]::WriteAllText($Path, $sb.ToString(), (New-Object Text.UTF8Encoding $true))
}
