# Fills the language columns of a Partner Center "Store listings" export from docs\store-listing.md.
#
# The three listings written by hand in Partner Center (en-us, zh-hant, zh-hans) are left untouched —
# whatever is in the export for them wins, because that is what the dashboard actually shows. That
# held while store-listing.md was a transcription of the dashboard. It stopped holding at 1.3.0,
# when the copy here was rewritten for a release the dashboard has never seen: leaving those three
# out now would ship thirty listings describing PDFs, batches and a searchable history alongside
# three that still describe 1.1.0. -IncludeAuthored fills them too, from this same file.
#
# The thirty remaining columns all have translated copy in store-listing.md and get it. The fallback
# to English further down is therefore dead code now; it stays because it is what should happen if a
# language is ever added to the package before its copy is written, and a silently English listing
# beats a silently empty one.
#
# Two files come out, because one question cannot be settled from here:
#
#   *-filled.csv          text plus the image cells, each pointing at the asset URL en-us already
#                         uses. The column type is "relative path (or URL to a Partner Center file)",
#                         so a URL is a legal value, but the URL carries the en-us listing's own id
#                         and Partner Center may refuse to hand that asset to a different listing.
#   *-filled-textonly.csv text only. Every image cell keeps the asset it already had, though not
#                         necessarily in the slot it had it in — see the screenshot-slot section.
#                         Captions past the number of images a listing actually has are left blank,
#                         since a caption on an empty slot is untested against the importer.
#
# Import the first. If it is rejected over the screenshots or logos, import the second, upload the
# seven PNGs and three logos per language in the dashboard in file-name order, then re-export and run
# again to pick up the captions this file held back.

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path,
    [string]$Listing,
    [string]$OutDir,
    # Fill only these language columns; every other column keeps whatever the export had. Use this
    # when the dashboard stalls on a whole-file import — a batch is a no-op for the columns it does
    # not name, so batches can be imported one at a time and re-imported safely.
    [string[]]$Only,
    # Same thing, cut automatically: -BatchSize 8 writes ceil(30/8) pairs of files instead of one.
    [int]$BatchSize,
    # Also fill en-us, zh-hant and zh-hans, the three that were typed into the dashboard by hand.
    [switch]$IncludeAuthored,
    # Which shot sits in which of en-us's screenshot slots, as shot numbers in slot order:
    # "2,5,3,4,1,6,7" means slot 1 shows 02-any-language.png and slot 5 shows 01-text-from-a-page.png.
    # Read it off the dashboard. Everything else is placed relative to en-us, so this is the one fact
    # the derivation below cannot always recover on its own — see the screenshot-slot section.
    [string]$EnSlotOrder,
    # Added to the output file names, so two runs over the same export do not overwrite each other.
    [string]$Suffix
)

$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\ListingCsv.ps1"

# $PSScriptRoot is empty while the param block is being evaluated under powershell -File, so the
# default for -Listing has to be filled in here instead.
if (-not $Listing) { $Listing = "$PSScriptRoot\..\docs\store-listing.md" }
if (-not $OutDir) { $OutDir = Split-Path -Parent (Resolve-Path $Path) }

# The heading of every language section in store-listing.md ends with an English name after an em
# dash; the English section itself is just "English (default)". Matching on that keeps this file
# pure ASCII — PowerShell 5.1 reads a .ps1 with no BOM as ANSI, and a CJK heading literal in here
# would arrive mangled.
$Column = @{
    'English (default)'      = 'en-us'
    'Chinese (Simplified)'   = 'zh-hans'
    'Chinese (Traditional)'  = 'zh-hant'
    'Japanese'               = 'ja'
    'Korean'                 = 'ko'
    'German'                 = 'de'
    'French'                 = 'fr'
    'Spanish'                = 'es'
    'Portuguese (Brazil)'    = 'pt'
    'Russian'                = 'ru'
    'Italian'                = 'it'
    'Polish'                 = 'pl'
    'Dutch'                  = 'nl'
    'Czech'                  = 'cs'
    'Turkish'                = 'tr'
    'Swedish'                = 'sv'
    'Danish'                 = 'da'
    'Norwegian'              = 'nb'
    'Finnish'                = 'fi'
    'Greek'                  = 'el'
    'Hungarian'              = 'hu'
    'Romanian'               = 'ro'
    'Ukrainian'              = 'uk'
    'Vietnamese'             = 'vi'
    'Thai'                   = 'th'
    'Indonesian'             = 'id'
    'Malay'                  = 'ms'
    'Filipino'               = 'fil'
    'Hindi'                  = 'hi'
    'Bengali'                = 'bn'
    'Arabic'                 = 'ar'
    'Hebrew'                 = 'he'
    'Persian'                = 'fa'
}

# Already written by hand in the dashboard.
$Keep = if ($IncludeAuthored) { @() } else { 'en-us', 'zh-hant', 'zh-hans' }
$Authored = 'en-us', 'zh-hant', 'zh-hans'

# Partner Center's own limits, checked below rather than trusted.
$Limit = @{ ShortDescription = 1000; Description = 10000; Feature = 200; Caption = 200; SearchTerm = 30 }

# How many PNGs the listing has. Raising this means uploading the new one to en-us first: the export
# does carry thirty screenshot slots, but a cell holds a dashboard URL, so an empty slot has nothing
# that can be written into it.
$Shots = 7

# ---------------------------------------------------------------- read the copy

$md = [IO.File]::ReadAllText((Resolve-Path $Listing), [Text.Encoding]::UTF8) -split "`r?`n"

$copy = @{}          # column -> @{ Short; Description; Features; Captions; Terms }
$lang = $null
$blocks = $null      # the ### sections of the current language, in file order
$buffer = $null

function Close-Block {
    if ($null -ne $buffer -and $null -ne $blocks) {
        $blocks.Add(($buffer -join "`n").Trim())
    }
}

foreach ($line in $md) {
    if ($line -like '## *') {
        Close-Block
        if ($lang) { $copy[$lang] = $blocks }
        $buffer = $null

        $heading = $line.Substring(3).Trim()
        $name = $Column.Keys | Where-Object { $heading.EndsWith($_) } | Select-Object -First 1
        $lang = $Column[$name]
        $blocks = if ($lang) { New-Object Collections.Generic.List[string] } else { $null }
        continue
    }
    if ($line -like '### *') {
        Close-Block
        # -ne $null, not a plain truth test: an empty List is falsy in PowerShell, and $blocks is
        # empty at exactly the moment the first ### of a language arrives.
        $buffer = if ($null -ne $blocks) { New-Object Collections.Generic.List[string] } else { $null }
        continue
    }
    if ($null -ne $buffer) { [void]$buffer.Add($line) }
}
Close-Block
if ($lang) { $copy[$lang] = $blocks }

foreach ($key in @($copy.Keys)) {
    $b = $copy[$key]
    if ($b.Count -ne 5) { throw "$key : expected 5 sections in store-listing.md, found $($b.Count)" }

    # Sections are keyed by position, not by name — the headings are translated, the order is not.
    $features = @($b[2] -split "`n" | Where-Object { $_ -like '- *' } | ForEach-Object { $_.Substring(2).Trim() })
    $captions = @($b[3] -split "`n" | ForEach-Object {
        # \p{Pd} is the dash that separates the file name from the caption. Spelled as a category
        # rather than as the character so that nothing in this file depends on how it is decoded.
        if ($_ -match '^\d+\.\s+`[^`]+`\s*\p{Pd}\s*(.+)$') { $Matches[1].Trim() }
    })
    $terms = @([regex]::Matches($b[4], '`([^`]+)`') | ForEach-Object { $_.Groups[1].Value.Trim() })

    if ($features.Count -ne 10)      { throw "$key : expected 10 features, found $($features.Count)" }
    if ($captions.Count -ne $Shots)  { throw "$key : expected $Shots captions, found $($captions.Count)" }
    if ($terms.Count -ne 7)          { throw "$key : expected 7 search terms, found $($terms.Count)" }

    $copy[$key] = @{
        ShortDescription = $b[0]
        Description      = $b[1]
        Features         = $features
        Captions         = $captions
        SearchTerms      = $terms
    }
}

if ($copy.Count -ne $Column.Count) {
    throw "parsed $($copy.Count) languages out of $($Column.Count) from $Listing"
}
"parsed store-listing.md: $($copy.Count) languages ($(($copy.Keys | Sort-Object) -join ' '))"

# ---------------------------------------------------------------- work out the target columns

$probe = Read-ListingCsv $Path
$header = $probe[0]
$width = $header.Count

$targets = @($header[4..($width - 1)] | Where-Object { $_ -notin $Keep })
if ($Only) {
    # Split on commas and spaces. Under powershell -File every argument arrives as one literal
    # string, so -Only es,pt,it is a single element here, not three.
    $Only = @($Only | ForEach-Object { $_ -split '[,\s]+' } | Where-Object { $_ })
    $unknown = @($Only | Where-Object { $_ -notin $header })
    if ($unknown) { throw "-Only names columns that are not in the export: $($unknown -join ' ')" }
    $targets = @($targets | Where-Object { $_ -in $Only })
    if (-not $targets) { throw "-Only selected no fillable column (en-us, zh-hant and zh-hans need -IncludeAuthored)" }
}

"columns to fill: $($targets.Count)"
"  translated : $(@($targets | Where-Object { $copy.ContainsKey($_) }) -join ' ')"
"  English    : $(@($targets | Where-Object { -not $copy.ContainsKey($_) }) -join ' ')"
''

$imageFields = @(1..$Shots | ForEach-Object { "DesktopScreenshot$_" }) +
               @('StoreLogo300x300', 'StoreLogoOverride150x150', 'StoreLogoOverride71x71')

$base = [IO.Path]::GetFileNameWithoutExtension($Path)

# ---------------------------------------------------------------- screenshot slots

# The same PNGs go into every listing, but the slot each one sits in is not the same, and writing
# captions from store-listing.md into a listing whose images are in a different order puts the
# QR-code caption under the settings dialog. So the slot order is worked out first and the image
# cells are moved to match, rather than the captions being bent to fit.
#
# Nothing in the export names a file. A screenshot cell is a dashboard URL ending in an asset id,
# and the column type is "relative path (or URL to a Partner Center file)" — no name, no dimensions,
# nothing. So which shot is in a slot can only ever be answered relative to something else, and there
# are exactly two somethings.
#
# The captions, for a listing whose captions were typed next to its images by hand. That is en-us,
# zh-hant and zh-hans, and only while their captions have not gone stale: the caption belongs to the
# slot, not to the image, so replacing the PNGs leaves the old captions sitting where they were and
# the route silently keeps answering with the old order. It is also all-or-nothing — five live
# captions cannot place seven slots.
#
# The asset ids, for the thirty that were machine-filled from en-us and so carry en-us's own ids.
# This one is inference-free while it works, and it stops working the moment en-us re-uploads: every
# id is new, and the thirty still point at assets that match nothing.
#
# Their captions prove nothing, and believing them is how this went wrong once already. Earlier runs
# wrote captions in file order while copying en-us's image cells slot for slot, so a listing's
# captions read 01..05 whatever its images actually were; a caption comparison returned the identity
# and read as confirmation. Twelve listings were mispaired in the dashboard on that basis.
#
# Which leaves -EnSlotOrder: one fact, read off the dashboard, for the one listing everything else is
# placed relative to. It is not a fallback for the caption route, it wins over it — a caption route
# that has gone stale is worse than no route, because it is confident.
function Get-SlotOrderByCaption($byField, $index, [string]$lang) {
    if (-not $copy.ContainsKey($lang)) { return $null }
    $wanted = $copy[$lang].Captions
    $order = New-Object int[] $Shots
    for ($s = 0; $s -lt $Shots; $s++) {
        $live = $byField["DesktopScreenshotCaption$($s + 1)"][$index[$lang]]
        $k = [Array]::IndexOf($wanted, $live)
        if ($k -lt 0) { return $null }
        $order[$s] = $k
    }
    if ((@($order | Sort-Object) -join ',') -ne ((0..($Shots - 1)) -join ',')) { return $null }
    , $order
}

# The cell is a full dashboard URL and it embeds the listing's own id, so two listings showing the
# same PNG do not have the same string here. Only the last segment, the asset id, is shared.
function Get-AssetId([string]$url) {
    if (-not $url) { return '' }
    $url.Substring($url.LastIndexOf('/') + 1)
}

function Get-SlotOrderByAsset($byField, $index, [string]$lang, $shotOfAsset) {
    $order = New-Object int[] $Shots
    for ($s = 0; $s -lt $Shots; $s++) {
        $id = Get-AssetId $byField["DesktopScreenshot$($s + 1)"][$index[$lang]]
        if (-not $shotOfAsset.ContainsKey($id)) { return $null }
        $order[$s] = $shotOfAsset[$id]
    }
    if ((@($order | Sort-Object) -join ',') -ne ((0..($Shots - 1)) -join ',')) { return $null }
    , $order
}

# "2,5,3,4,1,6,7" -> 0-based shot per slot. Rejected unless it is a permutation of 1..$Shots, since a
# typo here is otherwise indistinguishable from a deliberate reordering.
$givenOrder = $null
if ($EnSlotOrder) {
    $n = @($EnSlotOrder -split '[,\s]+' | Where-Object { $_ } | ForEach-Object { [int]$_ })
    if ((@($n | Sort-Object) -join ',') -ne ((1..$Shots) -join ',')) {
        throw "-EnSlotOrder must be a permutation of 1..$Shots, got '$EnSlotOrder'"
    }
    $givenOrder = New-Object int[] $Shots
    for ($s = 0; $s -lt $Shots; $s++) { $givenOrder[$s] = $n[$s] - 1 }
}

# ---------------------------------------------------------------- fill

# The export is re-read for every batch rather than reused. A batch has to leave the columns it does
# not name byte-identical to the export, and the cheapest way to guarantee that is to start from the
# export each time instead of trying to undo the previous batch's writes.
function Write-Filled([string[]]$langs, [string]$suffix) {
    $rows = Read-ListingCsv $Path

    $index = @{}
    for ($i = 0; $i -lt $width; $i++) { $index[$header[$i]] = $i }

    $byField = @{}
    for ($r = 1; $r -lt $rows.Count; $r++) {
        # Pad short rows once, here, so every write below can address any column.
        if ($rows[$r].Count -lt $width) {
            $padded = New-Object string[] $width
            [Array]::Copy($rows[$r], $padded, $rows[$r].Count)
            for ($c = $rows[$r].Count; $c -lt $width; $c++) { $padded[$c] = '' }
            $rows[$r] = $padded
        }
        $byField[$rows[$r][0]] = $rows[$r]
    }

    $warnings = New-Object Collections.Generic.List[string]
    $set = {
        param([string]$field, [string]$lang, [string]$value, [int]$limit)
        $row = $byField[$field]
        if (-not $row) { throw "no row named $field" }
        if ($limit -and $value.Length -gt $limit) {
            $warnings.Add("$lang $field is $($value.Length) characters, limit is $limit")
        }
        $row[$index[$lang]] = $value
    }

    $enTitle = $byField['Title'][$index['en-us']]

    # en-us first and unconditionally: it is the source every other listing's images are copied from.
    # -EnSlotOrder wins over the captions rather than backing them up. A caption that has gone stale
    # still matches store-listing.md and still returns an order, just the order from before the
    # re-upload -- so a stale caption route is worse than none, and being told the answer has to
    # override being able to guess one. Either way it has to happen before the captions below
    # overwrite the evidence.
    $enOrder = if ($givenOrder) { $givenOrder } else { Get-SlotOrderByCaption $byField $index 'en-us' }
    if (-not $enOrder) {
        throw ("en-us : cannot work out which screenshot is in which slot. Its captions do not match " +
               "store-listing.md -- which is what happens once the PNGs are re-uploaded, since the " +
               "captions stay in their old slots and the export names no files. Read the slot order " +
               "off the dashboard and pass it as -EnSlotOrder, e.g. -EnSlotOrder '1,2,3,4,5,6,7'.")
    }
    if ($givenOrder) { "  en-us : slot order taken from -EnSlotOrder" }

    # Reordered into a local array whether or not en-us is itself being filled: without this, a
    # re-run would push en-us's own slot order onto the other listings as if it were shot order.
    # $shotOfAsset is the same mapping keyed the other way, for the listings that copy from it.
    $enShots = New-Object string[] $Shots
    $shotOfAsset = @{}
    for ($s = 0; $s -lt $Shots; $s++) {
        $cell = $byField["DesktopScreenshot$($s + 1)"][$index['en-us']]
        $enShots[$enOrder[$s]] = $cell
        $shotOfAsset[(Get-AssetId $cell)] = $enOrder[$s]
    }

    $unplaced = New-Object Collections.Generic.List[string]
    $slotOrder = @{ 'en-us' = $enOrder }
    foreach ($lang in $langs) {
        if ($lang -eq 'en-us') { continue }
        $order = if ($lang -in $Authored) { Get-SlotOrderByCaption $byField $index $lang }
                 else { Get-SlotOrderByAsset $byField $index $lang $shotOfAsset }
        # pt, es and ro uploaded their own copies, so the asset ids are no help; their captions were
        # typed in by hand next to them, which puts them in the same position as the three above.
        if (-not $order -and $lang -notin $Authored) { $order = Get-SlotOrderByCaption $byField $index $lang }
        if ($order) { $slotOrder[$lang] = $order } else { $unplaced.Add($lang) }
    }

    # One line, not one per listing. After en-us re-uploads, every listing that was filled from it
    # fails both routes at once -- its asset ids are en-us's old ones and its captions are a run of
    # this script's own output -- so a per-listing warning is thirty-two lines saying one thing. It
    # costs nothing: the full file replaces their image cells with en-us's below, and the text-only
    # file's captions come out in file order, which is the order a hand upload goes in.
    if ($unplaced.Count) {
        $warnings.Add("$($unplaced.Count) listings' screenshot cells left as they are -- nothing in the " +
                      "export places them: $($unplaced -join ' ')")
    }

    foreach ($lang in $langs) {
        if (-not $slotOrder.ContainsKey($lang)) { continue }
        $old = @(1..$Shots | ForEach-Object { $byField["DesktopScreenshot$_"][$index[$lang]] })
        if (@($old | Where-Object { $_ }).Count -ne $Shots) { continue }   # not all of them uploaded yet
        for ($s = 0; $s -lt $Shots; $s++) {
            $byField["DesktopScreenshot$($slotOrder[$lang][$s] + 1)"][$index[$lang]] = $old[$s]
        }
        $moved = ($slotOrder[$lang] -join '') -ne ((0..($Shots - 1)) -join '')
        if ($moved) { "  $lang : screenshots reordered to 01..$('{0:00}' -f $Shots)" }
    }

    foreach ($lang in $langs) {
        $c = if ($copy.ContainsKey($lang)) { $copy[$lang] } else { $copy['en-us'] }

        & $set 'Title'            $lang $enTitle 0
        & $set 'ShortDescription' $lang $c.ShortDescription $Limit.ShortDescription
        & $set 'Description'      $lang $c.Description      $Limit.Description

        for ($i = 0; $i -lt 10; $i++) { & $set "Feature$($i + 1)"                  $lang $c.Features[$i]    $Limit.Feature }
        # A caption belongs to a slot, and a slot with no image in it is no place to put text. Only
        # en-us carries all $Shots right now; the rest are at whatever they were last uploaded with,
        # and it is not worth finding out the hard way whether Partner Center rejects a caption
        # pointing at nothing — this file's whole history is imports failing late and silently. The
        # full file below raises every listing to $Shots images and writes the rest of the captions
        # there; the text-only file leaves them blank, so that path is import, upload the PNGs by
        # hand, re-export, run again.
        $have = [Math]::Min($Shots, @(1..$Shots | Where-Object { $byField["DesktopScreenshot$_"][$index[$lang]] }).Count)
        for ($i = 0; $i -lt $have;   $i++) { & $set "DesktopScreenshotCaption$($i + 1)" $lang $c.Captions[$i] $Limit.Caption }
        for ($i = $have; $i -lt $Shots; $i++) { & $set "DesktopScreenshotCaption$($i + 1)" $lang '' 0 }
        for ($i = 0; $i -lt 7;  $i++) { & $set "SearchTerm$($i + 1)"               $lang $c.SearchTerms[$i] $Limit.SearchTerm }
    }

    $textOnly = Join-Path $OutDir "$base-filled$suffix-textonly.csv"
    Write-ListingCsv $textOnly $rows
    "wrote $textOnly"

    # And again, with the images pointed at the assets en-us already uses — in shot order, not in
    # the order en-us happens to show them.
    #
    # This used to skip the three hand-made listings, on the grounds that their own uploads were
    # certain to be accepted and en-us's assets were only probably accepted. That reasoning expired
    # in both directions: an import has since put en-us's assets into fourteen listings without
    # complaint, and those three are still carrying five PNGs while store-listing.md now writes
    # seven captions — a caption on a slot with no image in it. So they are filled like the rest.
    # en-us is skipped because it is the source.
    foreach ($lang in $langs) {
        if ($lang -eq 'en-us') { continue }
        for ($s = 0; $s -lt $Shots; $s++) {
            $byField["DesktopScreenshot$($s + 1)"][$index[$lang]] = $enShots[$s]
        }
        # Now that every slot holds an image, the captions held back above can go in.
        $c = if ($copy.ContainsKey($lang)) { $copy[$lang] } else { $copy['en-us'] }
        for ($i = 0; $i -lt $Shots; $i++) { & $set "DesktopScreenshotCaption$($i + 1)" $lang $c.Captions[$i] $Limit.Caption }
        foreach ($field in $imageFields | Where-Object { $_ -notlike 'DesktopScreenshot*' }) {
            $byField[$field][$index[$lang]] = $byField[$field][$index['en-us']]
        }
        # The three logo overrides only take effect when this is on, and en-us has it on.
        $byField['OverrideLogosForWin10'][$index[$lang]] = $byField['OverrideLogosForWin10'][$index['en-us']]
    }

    $full = Join-Path $OutDir "$base-filled$suffix.csv"
    Write-ListingCsv $full $rows
    "wrote $full"

    # Last, so that the over-limit checks on the captions only the full file writes are included.
    # Deduplicated because that pass rewrites captions the first pass already checked.
    foreach ($w in ($warnings | Select-Object -Unique)) { Write-Warning $w }
}

if ($BatchSize -gt 0) {
    $n = [Math]::Ceiling($targets.Count / $BatchSize)
    for ($b = 0; $b -lt $n; $b++) {
        $slice = @($targets | Select-Object -Skip ($b * $BatchSize) -First $BatchSize)
        "batch $($b + 1)/$n : $($slice -join ' ')"
        Write-Filled $slice "$Suffix-b$($b + 1)"
        ''
    }
}
else {
    Write-Filled $targets $Suffix
}
