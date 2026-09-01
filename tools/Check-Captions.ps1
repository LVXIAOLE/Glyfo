# Counts the screenshot captions in docs\store-listing.md and measures each one against the Store's
# 200-character limit. The file name at the start of every line is an annotation for us, not part of
# the caption, so only the text after the em dash is measured.
$path = Join-Path (Split-Path -Parent $PSScriptRoot) 'docs\store-listing.md'
$lines = Get-Content -LiteralPath $path -Encoding UTF8

$section = ''
$n = 0
$worst = 0
foreach ($line in $lines) {
    if ($line -match '^#{2,3} (.+)$') { $heading = $Matches[1] }
    if ($line -match '^\d\. `0\d-[a-z-]+\.png` — (.+)$') {
        $text = $Matches[1]
        $n++
        if ($text.Length -gt $worst) { $worst = $text.Length }
        $flag = if ($text.Length -gt 200) { 'OVER' } else { 'ok' }
        "{0,-40} {1,4}  {2}" -f $heading, $text.Length, $flag
    }
}
''
"captions: $n   longest: $worst"
