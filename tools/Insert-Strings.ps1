# Splices new entries into the Services\Strings.*.cs tables named by the payload, each entry next to
# a named neighbour. Strings.En.cs is written by hand and is not in the payload: it is the source the
# translations were made from, so having a script able to overwrite it is a way to lose the original.
#
# Add-Strings.ps1 appends at the end of the table, which is right for a block that belongs together
# (the About page, the release notes). This script exists for the other case: a handful of unrelated
# keys that each belong beside an existing one, so that a translator reading the table finds the new
# tooltip among the tooltips and the new status line among the status lines. Appending the whole
# batch to the bottom would have been less work and a worse file.
#
# Same reason as Add-Strings.ps1 for the split into two files: PowerShell 5.1 reads a BOM-less .ps1
# as ANSI on this machine, so every non-Latin translation would be mojibake before the script was
# even parsed. This file stays pure ASCII; the translations live in the UTF-8 payload.
#
# The payload is one "lang|key|value" per line -- lang being the file suffix (En, ZhHans, Ar), and
# value being the C# string literal without its quotes. Values therefore must not contain a double
# quote, a backslash or a pipe; there is no escaping, because a translation needing one is a sign
# the English needed rewording first.
#
# Where each key goes is decided here, not in the payload: the anchors are a property of the table's
# layout, which is the same in all thirty-three files, and repeating them on every payload line
# would be one chance per line to disagree.
#
# Re-running is NOT safe. A key that is already present is inserted a second time and the table stops
# compiling with a duplicate-key error, which is the loud failure this would rather have than a
# silent skip that hides a payload typo.

[CmdletBinding()]
param(
    [string]$Payload,
    [string]$Services
)

$ErrorActionPreference = 'Stop'

if (-not $Payload) { $Payload = "$PSScriptRoot\strings-130.txt" }
if (-not $Services) { $Services = "$PSScriptRoot\..\Glyfo\Services" }

# Anchor -> the keys that follow it. Insertion is after the anchor line, except for $BeforeAnchor
# below: the release notes are listed newest first, so a new version's notes go above the old ones.
$After = [ordered]@{
    'Tip_PdfNext'                = @('Tip_PdfAll')
    'Tip_CopyText'               = @('Tip_SaveText')
    'History_EmptyPreview'       = @('History_SearchPlaceholder', 'History_NoMatch', 'History_Off', 'History_Clear', 'History_CopyItem', 'History_DeleteItem')
    'Setting_WatchClipboard_Desc' = @('Setting_KeepHistory', 'Setting_KeepHistory_Desc')
    'Status_NothingToCopy'       = @('Status_NothingToSave', 'Status_TextSaved', 'Status_TextSaveFailed')
    'Status_HistoryLoaded'       = @('Status_HistoryCleared')
    'Source_Codes'               = @('Source_Batch', 'Source_PdfAll')
    'FileType_Image'             = @('FileType_Text', 'FileType_Markdown', 'Batch_Title', 'Batch_Progress', 'Batch_DoneAll', 'Batch_Cancelled', 'Batch_ItemFailed', 'Batch_ItemEmpty', 'Batch_Save', 'Batch_CopyAll', 'Batch_Separate', 'Batch_SavedFolder', 'Batch_TooLongForBox')
    'Common_Close'               = @('Common_Cancel')
}
$BeforeAnchor = 'News_120_1'
$BeforeKeys = @('News_130_1', 'News_130_2', 'News_130_3')

$text = [IO.File]::ReadAllText((Resolve-Path $Payload), [Text.Encoding]::UTF8).TrimStart([char]0xFEFF)

$byLang = @{}
foreach ($line in ($text -split "`r?`n")) {
    if (-not $line.Trim()) { continue }
    $parts = $line.Split('|', 3)
    if ($parts.Count -ne 3) { throw "malformed payload line: $line" }
    if (-not $byLang.ContainsKey($parts[0])) { $byLang[$parts[0]] = @{} }
    $byLang[$parts[0]][$parts[1]] = $parts[2]
}

function Find-Anchor($lines, [string]$key) {
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i].Contains("[""$key""]")) { return $i }
    }
    throw "anchor $key not found"
}

foreach ($lang in ($byLang.Keys | Sort-Object)) {
    $path = Join-Path $Services "Strings.$lang.cs"
    if (-not (Test-Path $path)) { throw "no table file for $lang at $path" }

    $body = [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)
    $lines = New-Object Collections.Generic.List[string]
    foreach ($l in ($body -split "`n")) { [void]$lines.Add($l) }

    $table = $byLang[$lang]
    $plan = @()

    foreach ($anchor in $After.Keys) {
        $block = @()
        foreach ($k in $After[$anchor]) {
            if (-not $table.ContainsKey($k)) { throw "$lang is missing $k" }
            $block += ('        ["' + $k + '"] = "' + $table[$k] + '",')
        }
        $plan += @{ Index = (Find-Anchor $lines $anchor) + 1; Lines = $block }
    }

    $block = @()
    foreach ($k in $BeforeKeys) {
        if (-not $table.ContainsKey($k)) { throw "$lang is missing $k" }
        $block += ('        ["' + $k + '"] = "' + $table[$k] + '",')
    }
    $plan += @{ Index = (Find-Anchor $lines $BeforeAnchor); Lines = $block }

    # Bottom up, so that an insertion never moves the line an earlier anchor was found at.
    foreach ($p in ($plan | Sort-Object { $_.Index } -Descending)) {
        $lines.InsertRange($p.Index, [string[]]$p.Lines)
    }

    # LF and no BOM, matching what is already on disk. Splitting on "`n" leaves any CR attached to
    # the end of the line, so a file that did use CRLF would come back out of here unchanged too.
    [IO.File]::WriteAllText($path, ($lines -join "`n"), (New-Object Text.UTF8Encoding $false))
    "wrote $lang ($((($plan | ForEach-Object { $_.Lines.Count }) | Measure-Object -Sum).Sum) lines)"
}
