# Takes the Store screenshots off the real, packaged app rather than a mock-up: it activates Glyfo
# through the file association, picks the interface and recognition languages through the actual
# controls, waits for recognition to settle, drives the flyouts with real mouse clicks at the
# coordinates UI Automation reports, grabs the window off the screen and drops it onto a 1920x1080
# canvas.
#
# Run it with nothing else on screen and don't touch the mouse or keyboard while it runs — it moves
# the real cursor, and a stray click lands wherever the pointer happens to be.
#
# It changes two pieces of app state: the interface language and the recognition language. Both are
# read at the start and put back at the end.
[CmdletBinding()]
param(
    # Which shots to take. Handy while iterating on one of them.
    [string[]]$Only
)

$ErrorActionPreference = 'Stop'

# This has to happen before anything measures a window. Windows PowerShell declares no DPI
# awareness, so on this 125% display user32 hands a script 1536x864 and quietly multiplies every
# coordinate it is given by 1.25 — while DwmGetWindowAttribute and the screen DC keep reporting real
# pixels. Asking for a 1600x900 window that way produces a 2000x1125 one hanging off the screen.
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class DpiSetup {
    [DllImport("user32.dll")] static extern bool SetProcessDpiAwarenessContext(IntPtr value);
    [DllImport("user32.dll")] static extern bool SetProcessDPIAware();
    public static string Apply() {
        try { if (SetProcessDpiAwarenessContext(new IntPtr(-4))) return "per-monitor-v2"; } catch {}
        return SetProcessDPIAware() ? "system" : "already set";
    }
}
'@
"dpi awareness  : $([DpiSetup]::Apply())"

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$root = $PSScriptRoot
$src = Join-Path $root 'src'
$out = Join-Path $root 'out'
New-Item -ItemType Directory -Force -Path $out | Out-Null

# The window is placed at a fixed physical size so every shot is framed identically. 1600x900 on a
# 1920x1080 screen leaves an even margin and clears the taskbar. The vertical offset sits the window
# above centre, because the Store may lay its own text over the bottom third of a screenshot.
$WinW = 1600; $WinH = 900
$CanvasW = 1920; $CanvasH = 1080
$WinX = [int](($CanvasW - $WinW) / 2)
$WinY = 70

Add-Type @'
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public static class Win {
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }

    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int cmd);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extra);
    [DllImport("user32.dll")] public static extern void keybd_event(byte vk, byte scan, uint flags, UIntPtr extra);
    [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr h, int attr, out RECT val, int size);

    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002, MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint KEYEVENTF_KEYUP = 0x0002;
    public const int SW_HIDE = 0, SW_SHOWNA = 8, SW_RESTORE = 9;

    // DWMWA_EXTENDED_FRAME_BOUNDS. GetWindowRect includes the invisible resize border DWM keeps
    // around every window; using it would put several pixels of desktop inside every screenshot.
    public const int EXTENDED_FRAME_BOUNDS = 9;

    delegate bool EnumProc(IntPtr h, IntPtr p);
    [DllImport("user32.dll")] static extern bool EnumWindows(EnumProc cb, IntPtr p);
    [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);

    // Every visible top-level window belonging to one of the given processes. Used to get the input
    // method's floating bar out of the frame: it is topmost, so it lands in the middle of the shot.
    public static List<IntPtr> VisibleWindowsOf(int[] pids) {
        var found = new List<IntPtr>();
        var wanted = new HashSet<uint>();
        foreach (var p in pids) wanted.Add((uint)p);
        EnumWindows((h, _) => {
            uint pid; GetWindowThreadProcessId(h, out pid);
            if (wanted.Contains(pid) && IsWindowVisible(h)) found.Add(h);
            return true;
        }, IntPtr.Zero);
        return found;
    }

    [StructLayout(LayoutKind.Sequential)] public struct SHELLEXECUTEINFO {
        public int cbSize; public uint fMask; public IntPtr hwnd;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpVerb;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpFile;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpParameters;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpDirectory;
        public int nShow; public IntPtr hInstApp; public IntPtr lpIDList;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpClass;
        public IntPtr hkeyClass; public uint dwHotKey; public IntPtr hIcon; public IntPtr hProcess;
    }
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ShellExecuteExW(ref SHELLEXECUTEINFO info);
}
'@

$UIA = [System.Windows.Automation.AutomationElement]

# ------------------------------------------------------------------ activation
function Get-GlyfoProgId([string]$Extension) {
    # The progid is generated from the package identity, so it is looked up rather than written in:
    # it changes if the identity ever does. Looked up per extension, because the shell registers a
    # separate progid for each one the manifest claims.
    $key = Get-ItemProperty "HKCU:\Software\Classes\$Extension\OpenWithProgids" -ErrorAction Stop
    foreach ($name in $key.PSObject.Properties.Name) {
        if ($name -notlike 'AppX*') { continue }
        $aumid = (Get-ItemProperty "HKCU:\Software\Classes\$name\Application" -ErrorAction SilentlyContinue).AppUserModelID
        if ($aumid -like 'LVLE.Glyfo*') { return $name }
    }
    throw "No Glyfo progid registered for $Extension — is the package installed?"
}
$script:ProgIds = @{}

function Open-InGlyfo([string]$Path) {
    # Full-trust packaged apps are activated for files through the shell's DelegateExecute handler;
    # ShellExecuteEx with the progid is the only route that reaches it from a script.
    $full = (Resolve-Path $Path).Path
    $ext = [IO.Path]::GetExtension($full).ToLowerInvariant()
    if (-not $script:ProgIds.ContainsKey($ext)) { $script:ProgIds[$ext] = Get-GlyfoProgId $ext }

    $info = New-Object Win+SHELLEXECUTEINFO
    $info.cbSize = [Runtime.InteropServices.Marshal]::SizeOf($info)
    $info.fMask = 0x00000001   # SEE_MASK_CLASSNAME
    $info.lpFile = $full
    $info.lpClass = $script:ProgIds[$ext]
    $info.nShow = 5
    if (-not [Win]::ShellExecuteExW([ref]$info)) { throw "ShellExecuteEx failed for $Path" }
    # The launched instance asks the activating process for its arguments over RPC, so this one has
    # to still be alive when it does. Exiting early gets it 0x800706BA and no file at all.
    Start-Sleep -Seconds 6
}

function Get-GlyfoWindow {
    for ($i = 0; $i -lt 40; $i++) {
        $p = Get-Process Glyfo -ErrorAction SilentlyContinue |
             Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
        if ($p) { return $p.MainWindowHandle }
        Start-Sleep -Milliseconds 500
    }
    throw 'Glyfo has no visible window'
}

function Get-Bounds([IntPtr]$h) {
    $r = New-Object Win+RECT
    [void][Win]::DwmGetWindowAttribute($h, [Win]::EXTENDED_FRAME_BOUNDS, [ref]$r, 16)
    return $r
}

function Set-Frame([IntPtr]$h) {
    # A window restored from the tray, or one the user last left maximised, ignores a plain
    # SetWindowPos; it has to come out of that state first. The result is then checked rather than
    # assumed, because the first framing after a cold start lands while the window is still
    # animating and silently ends up the wrong size.
    for ($try = 0; $try -lt 6; $try++) {
        [void][Win]::ShowWindow($h, [Win]::SW_RESTORE)
        [void][Win]::SetWindowPos($h, [IntPtr]::Zero, $WinX, $WinY, $WinW, $WinH, 0x0040)
        [void][Win]::SetForegroundWindow($h)
        Start-Sleep -Milliseconds 700
        $r = Get-Bounds $h
        if ([Math]::Abs(($r.R - $r.L) - $WinW) -le 24 -and [Math]::Abs(($r.B - $r.T) - $WinH) -le 24) {
            "  frame $($r.L),$($r.T) $($r.R - $r.L)x$($r.B - $r.T)"
            return
        }
    }
    $r = Get-Bounds $h
    Write-Warning "window is $($r.R - $r.L)x$($r.B - $r.T) at $($r.L),$($r.T), wanted ${WinW}x${WinH}"
}

# ------------------------------------------------------------------ driving the UI
function Find-Element([IntPtr]$h, [string]$AutomationId) {
    $window = $UIA::FromHandle($h)
    $cond = New-Object Windows.Automation.PropertyCondition (
        [Windows.Automation.AutomationElement]::AutomationIdProperty, $AutomationId)
    $window.FindFirst([Windows.Automation.TreeScope]::Descendants, $cond)
}

function Click-Point([int]$X, [int]$Y) {
    [void][Win]::SetCursorPos($X, $Y)
    Start-Sleep -Milliseconds 250
    [Win]::mouse_event([Win]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 80
    [Win]::mouse_event([Win]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 900
}

function Click-Element([IntPtr]$h, [string]$AutomationId) {
    $el = Find-Element $h $AutomationId
    if (-not $el) { throw "No element with AutomationId '$AutomationId'" }
    $r = $el.Current.BoundingRectangle
    # A real click rather than InvokePattern: a Button that owns a Flyout opens it from the click,
    # and this exercises the same path a person would.
    Click-Point ([int]($r.X + $r.Width / 2)) ([int]($r.Y + $r.Height / 2))
}

function Send-Esc {
    [Win]::keybd_event(0x1B, 0, 0, [UIntPtr]::Zero)
    [Win]::keybd_event(0x1B, 0, [Win]::KEYEVENTF_KEYUP, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 600
}

function Send-Digits([string]$Digits) {
    # Digits only, and typed rather than pushed in through ValuePattern: this goes through the same
    # TextChanged the user's keystrokes would, and it leaves the caret where a person's would be.
    # The virtual-key codes for 0-9 are the ASCII codes, which is why nothing wider is handled here.
    foreach ($c in $Digits.ToCharArray()) {
        if ($c -lt '0' -or $c -gt '9') { throw "Send-Digits takes digits only, got '$c'" }
        $vk = [byte][char]$c
        [Win]::keybd_event($vk, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 40
        [Win]::keybd_event($vk, 0, [Win]::KEYEVENTF_KEYUP, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 90
    }
    Start-Sleep -Milliseconds 800
}

function Get-DialogCloseButton([IntPtr]$h) {
    # "CloseButton" is the ContentDialog template's name for its close button -- and also the name
    # the status bar's InfoBar gives to its dismiss "x", which is on screen most of the run. So the
    # matches are filtered rather than taking the first one.
    $window = $UIA::FromHandle($h)
    $cond = New-Object Windows.Automation.PropertyCondition (
        [Windows.Automation.AutomationElement]::AutomationIdProperty, 'CloseButton')
    $walker = [Windows.Automation.TreeWalker]::ControlViewWalker
    foreach ($el in $window.FindAll([Windows.Automation.TreeScope]::Descendants, $cond)) {
        $parent = $walker.GetParent($el)
        if ($parent -and $parent.Current.AutomationId -eq 'StatusBar') { continue }
        return $el
    }
    return $null
}

function Test-DialogOpen([IntPtr]$h) {
    # An open ContentDialog puts a Popup the size of the window into the tree -- the layer that dims
    # everything behind it. The dialog's own content sits in a second Popup that reports IsOffscreen
    # even while it is plainly on screen, so the dimming layer is the one to ask. Flyouts and combo
    # drop-downs are Popups too, hence the width test: none of them come close to filling the window.
    $window = $UIA::FromHandle($h)
    $cond = New-Object Windows.Automation.PropertyCondition (
        [Windows.Automation.AutomationElement]::ControlTypeProperty, [Windows.Automation.ControlType]::Window)
    $bounds = Get-Bounds $h
    $wide = ($bounds.R - $bounds.L) * 0.8
    foreach ($popup in $window.FindAll([Windows.Automation.TreeScope]::Descendants, $cond)) {
        if (-not $popup.Current.IsOffscreen -and $popup.Current.BoundingRectangle.Width -ge $wide) {
            return $true
        }
    }
    return $false
}

function Close-Dialog([IntPtr]$h) {
    # Esc is what a person would press, but it is delivered to whatever holds focus, and changing
    # the interface language rebuilds the whole dialog underneath it -- after which the key went
    # nowhere and the dialog stayed up for the rest of the run. Invoking the button is addressed at
    # the dialog itself, and the result is checked rather than assumed.
    for ($i = 0; $i -lt 6; $i++) {
        if (-not (Test-DialogOpen $h)) { return }
        $btn = Get-DialogCloseButton $h
        $ip = $null
        if (-not $btn) { Send-Esc; Start-Sleep -Milliseconds 900; continue }
        if ($btn.TryGetCurrentPattern([Windows.Automation.InvokePattern]::Pattern, [ref]$ip)) {
            $ip.Invoke()
        } else {
            Send-Esc
        }
        Start-Sleep -Milliseconds 900
    }
    throw 'A dialog would not close'
}

function Wait-Element([IntPtr]$h, [string]$AutomationId, [int]$Seconds = 90) {
    # A collapsed element is absent from the UI Automation tree, so "this one has appeared" is a
    # real signal rather than a guess at how long something takes.
    for ($i = 0; $i -lt ($Seconds * 2); $i++) {
        if (Find-Element $h $AutomationId) { return }
        Start-Sleep -Milliseconds 500
    }
    throw "Timed out waiting for '$AutomationId'"
}

function Get-ComboValue([IntPtr]$h, [string]$AutomationId) {
    $el = Find-Element $h $AutomationId
    if (-not $el) { return $null }
    $vp = $null
    # A non-editable ComboBox still offers ValuePattern, and it answers with an empty string -- so
    # the value has to be non-empty to be believed, or the selection below is never consulted.
    if ($el.TryGetCurrentPattern([Windows.Automation.ValuePattern]::Pattern, [ref]$vp) -and $vp.Current.Value) {
        return $vp.Current.Value
    }
    $sel = $null
    if ($el.TryGetCurrentPattern([Windows.Automation.SelectionPattern]::Pattern, [ref]$sel)) {
        $items = $sel.Current.GetSelection()
        if ($items.Count -gt 0) { return $items[0].Current.Name }
    }
    return $null
}

function Select-Combo([IntPtr]$h, [string]$AutomationId, [string]$Pattern) {
    # ComboBox items are realised only once the popup is open, so it is expanded first and the
    # matching row is then clicked where it actually sits on screen.
    $el = Find-Element $h $AutomationId
    if (-not $el) { throw "No combo '$AutomationId'" }
    $ec = $null
    if (-not $el.TryGetCurrentPattern([Windows.Automation.ExpandCollapsePattern]::Pattern, [ref]$ec)) {
        throw "'$AutomationId' cannot be expanded"
    }
    $ec.Expand()
    Start-Sleep -Milliseconds 700

    $cond = New-Object Windows.Automation.PropertyCondition (
        [Windows.Automation.AutomationElement]::ControlTypeProperty,
        [Windows.Automation.ControlType]::ListItem)
    $items = $el.FindAll([Windows.Automation.TreeScope]::Descendants, $cond)
    $names = @()
    $target = $null
    foreach ($item in $items) {
        $names += $item.Current.Name
        if (-not $target -and $item.Current.Name -match $Pattern) { $target = $item }
    }
    if (-not $target) {
        $ec.Collapse()
        throw "No item matching '$Pattern' in '$AutomationId'. Options: $($names -join ' | ')"
    }
    $si = $null
    if ($target.TryGetCurrentPattern([Windows.Automation.SelectionItemPattern]::Pattern, [ref]$si)) {
        $si.Select()
    } else {
        $r = $target.Current.BoundingRectangle
        Click-Point ([int]($r.X + $r.Width / 2)) ([int]($r.Y + $r.Height / 2))
    }
    Start-Sleep -Milliseconds 900
    if ($ec.Current.ExpandCollapseState -eq [Windows.Automation.ExpandCollapseState]::Expanded) { $ec.Collapse() }
    Start-Sleep -Milliseconds 400
    "  $AutomationId -> $($target.Current.Name)"
}

function Get-ComboItems([IntPtr]$h, [string]$AutomationId) {
    # Rows are realised only while the popup is open, so both reading and writing a ComboBox has to
    # expand it first. The caller gets the element back too, to collapse it when it is done.
    $el = Find-Element $h $AutomationId
    if (-not $el) { throw "No combo '$AutomationId'" }
    $ec = $null
    if (-not $el.TryGetCurrentPattern([Windows.Automation.ExpandCollapsePattern]::Pattern, [ref]$ec)) {
        throw "'$AutomationId' cannot be expanded"
    }
    $ec.Expand()
    Start-Sleep -Milliseconds 700
    $cond = New-Object Windows.Automation.PropertyCondition (
        [Windows.Automation.AutomationElement]::ControlTypeProperty,
        [Windows.Automation.ControlType]::ListItem)
    @{ Expand = $ec; Items = $el.FindAll([Windows.Automation.TreeScope]::Descendants, $cond) }
}

function Close-Combo($Expand) {
    if ($Expand.Current.ExpandCollapseState -eq [Windows.Automation.ExpandCollapseState]::Expanded) {
        $Expand.Collapse()
    }
    Start-Sleep -Milliseconds 400
}

function Get-ComboIndex([IntPtr]$h, [string]$AutomationId) {
    # The interface language is restored by position rather than by name: the list is rebuilt in the
    # new language every time it changes, so "System default" read at the start no longer matches
    # anything by the end of the run. The order is the same list every time, so the index survives.
    $c = Get-ComboItems $h $AutomationId
    $found = -1
    for ($i = 0; $i -lt $c.Items.Count; $i++) {
        $si = $null
        if ($c.Items[$i].TryGetCurrentPattern([Windows.Automation.SelectionItemPattern]::Pattern, [ref]$si) -and
            $si.Current.IsSelected) {
            $found = $i
            break
        }
    }
    Close-Combo $c.Expand
    # Write-Host rather than an ordinary string: this function's return value is the index, and a
    # second object on the pipeline would come back to the caller as part of it.
    Write-Host "  $AutomationId is at index $found ($(if ($found -ge 0) { $c.Items[$found].Current.Name }))"
    $found
}

function Select-ComboIndex([IntPtr]$h, [string]$AutomationId, [int]$Index) {
    $c = Get-ComboItems $h $AutomationId
    if ($Index -lt 0 -or $Index -ge $c.Items.Count) {
        Close-Combo $c.Expand
        throw "Index $Index is outside '$AutomationId' ($($c.Items.Count) items)"
    }
    $target = $c.Items[$Index]
    $target.GetCurrentPattern([Windows.Automation.SelectionItemPattern]::Pattern).Select()
    Start-Sleep -Milliseconds 900
    Close-Combo $c.Expand
    "  $AutomationId -> $($target.Current.Name)"
}

function Move-CursorAway([IntPtr]$h) {
    # Otherwise whatever was clicked keeps its hover styling, and the tooltip it opened is still
    # fading in when the screen is grabbed.
    #
    # Jumping straight to the corner is not enough on its own: a tooltip left over from a click the
    # pointer never physically made stays up, because its owner never sees the pointer leave. An
    # "Options" box sat over the status bar for that reason, and outwaiting it did not help either.
    # So the pointer is walked across an empty part of the window first — that gives every element a
    # real enter/leave to react to — and only then parked off in the corner.
    $r = Get-Bounds $h
    $x = $r.L + [int](($r.R - $r.L) * 0.75)
    foreach ($t in 0.55, 0.65, 0.75) {
        [void][Win]::SetCursorPos($x, ($r.T + [int](($r.B - $r.T) * $t)))
        Start-Sleep -Milliseconds 250
    }
    Start-Sleep -Milliseconds 700
    [void][Win]::SetCursorPos(4, 4)
    Start-Sleep -Milliseconds 300
    [void][Win]::SetCursorPos(8, 8)
    Start-Sleep -Milliseconds 1800
}

function Wait-ForIdle([IntPtr]$h, [int]$Seconds = 30) {
    # Recognition is asynchronous and offers no completion signal from outside the process, so this
    # watches the status bar and the result text settle rather than sleeping a fixed guess.
    $last = $null; $stable = 0
    for ($i = 0; $i -lt ($Seconds * 4); $i++) {
        Start-Sleep -Milliseconds 250
        $bar = Find-Element $h 'StatusBar'
        $box = Find-Element $h 'ResultTextBox'
        $text = ''
        if ($box) {
            $vp = $null
            if ($box.TryGetCurrentPattern([Windows.Automation.ValuePattern]::Pattern, [ref]$vp)) {
                $text = $vp.Current.Value
            }
        }
        $now = "$(if ($bar) { $bar.Current.Name })|$($text.Length)"
        if ($now -eq $last) { $stable++ } else { $stable = 0; $last = $now }
        if ($stable -ge 8) { return }
    }
}

# ------------------------------------------------------------------ capture
function New-RoundedPath([int]$X, [int]$Y, [int]$W, [int]$H, [int]$R) {
    $p = New-Object Drawing.Drawing2D.GraphicsPath
    $d = $R * 2
    $p.AddArc($X, $Y, $d, $d, 180, 90)
    $p.AddArc($X + $W - $d, $Y, $d, $d, 270, 90)
    $p.AddArc($X + $W - $d, $Y + $H - $d, $d, $d, 0, 90)
    $p.AddArc($X, $Y + $H - $d, $d, $d, 90, 90)
    $p.CloseFigure()
    return $p
}

function Save-Shot([IntPtr]$h, [string]$Name, [switch]$WithDialog) {
    # A ContentDialog left open silently ruins every later shot: it dims the window and covers the
    # middle of it, and the run carries on because nothing throws. That is exactly what happened
    # once -- four shots taken through the settings dialog -- so the two shots that are meant to
    # show a dialog say so, and any other one that finds one on screen stops the run.
    if (-not $WithDialog -and (Test-DialogOpen $h)) {
        throw "A dialog is on screen; '$Name' would be taken over it"
    }

    $r = Get-Bounds $h
    $w = $r.R - $r.L; $ht = $r.B - $r.T

    $shot = New-Object Drawing.Bitmap $w, $ht
    $sg = [Drawing.Graphics]::FromImage($shot)
    $sg.CopyFromScreen($r.L, $r.T, 0, 0, (New-Object Drawing.Size $w, $ht))
    $sg.Dispose()

    $canvas = New-Object Drawing.Bitmap $CanvasW, $CanvasH
    $g = [Drawing.Graphics]::FromImage($canvas)
    $g.SmoothingMode = 'AntiAlias'
    $g.InterpolationMode = 'HighQualityBicubic'

    # A quiet mid-tone backdrop: the Store may draw its own text over the bottom third, and its
    # guidance asks for nothing extreme in either direction behind it.
    $brush = New-Object Drawing.Drawing2D.LinearGradientBrush(
        (New-Object Drawing.Rectangle 0, 0, $CanvasW, $CanvasH),
        [Drawing.ColorTranslator]::FromHtml('#DCE4EC'),
        [Drawing.ColorTranslator]::FromHtml('#B4C3D4'),
        [Drawing.Drawing2D.LinearGradientMode]::Vertical)
    $g.FillRectangle($brush, 0, 0, $CanvasW, $CanvasH)
    $brush.Dispose()

    $x = [int](($CanvasW - $w) / 2)
    $y = $WinY

    for ($i = 14; $i -ge 1; $i--) {
        $a = [int](3 + (14 - $i) * 0.7)
        $sb = New-Object Drawing.SolidBrush ([Drawing.Color]::FromArgb($a, 40, 55, 75))
        $g.FillPath($sb, (New-RoundedPath ($x - $i) ($y - [int]($i / 3) + 6) ($w + $i * 2) ($ht + $i * 2) (10 + $i)))
        $sb.Dispose()
    }

    $path = New-RoundedPath $x $y $w $ht 10
    $g.SetClip($path)
    $g.DrawImage($shot, $x, $y, $w, $ht)
    $g.ResetClip()
    $path.Dispose()

    $g.Dispose(); $shot.Dispose()
    $canvas.Save((Join-Path $out "$Name.png"), [Drawing.Imaging.ImageFormat]::Png)
    $canvas.Dispose()
    "  saved $Name.png"
}

function Should([string]$Name) { -not $Only -or ($Only -contains $Name) }

# ------------------------------------------------------------------ run
$imeWindows = @()
try {
    $imePids = Get-Process -ErrorAction SilentlyContinue |
               Where-Object { $_.ProcessName -match '^(Sogou|QQPinyin|ChsIME|TextInputHost)' } |
               ForEach-Object { $_.Id }
    if ($imePids) {
        $imeWindows = [Win]::VisibleWindowsOf([int[]]$imePids)
        foreach ($w in $imeWindows) { [void][Win]::ShowWindow($w, [Win]::SW_HIDE) }
        "hid $($imeWindows.Count) input-method window(s)"
    }

    # The app starts empty rather than on a file, because the language has to be set before anything
    # is recognised: opening a file runs recognition immediately, and a pass made with the wrong
    # recogniser leaves a line of garbage in the history for the rest of the session — which is
    # exactly what the history screenshot would then show.
    Get-Process Glyfo -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Seconds 1
    $package = Get-AppxPackage -Name LVLE.Glyfo
    $appId = (Get-AppxPackageManifest $package).Package.Applications.Application.Id
    Start-Process "shell:AppsFolder\$($package.PackageFamilyName)!$appId"
    $h = Get-GlyfoWindow
    Set-Frame $h
    # The startup hint toast overlaps the bottom-right of the window; it goes away on its own.
    Start-Sleep -Seconds 10
    Set-Frame $h

    $originalOcr = Get-ComboValue $h 'LanguageComboBox'
    $originalVoice = Get-ComboValue $h 'VoiceComboBox'
    "recognition was '$originalOcr', voice was '$originalVoice'"

    # The listing's default language is English and these images are reused for every listing, so
    # the interface goes into English for the run and is put back at the end. The interface picker
    # lives inside the settings dialog, so it can only be read with the dialog open -- reading it
    # from outside quietly returned nothing, which is why the restore at the end used to be skipped.
    Click-Element $h 'SettingsButton'
    $originalUi = Get-ComboIndex $h 'UiLanguageComboBox'
    Select-Combo $h 'UiLanguageComboBox' '^English'
    Close-Dialog $h
    Set-Frame $h

    function Set-Voice([string]$Pattern) {
        # Not essential, but a Chinese voice sitting above an English result reads as an oversight.
        try { Select-Combo $h 'VoiceComboBox' $Pattern | Out-Null }
        catch { Write-Warning "voice '$Pattern': $($_.Exception.Message)" }
    }

    Select-Combo $h 'LanguageComboBox' 'English'
    Set-Voice 'Zira|David|Mark|English'
    "opening notes-en.png"
    Open-InGlyfo (Join-Path $src 'notes-en.png')
    Set-Frame $h
    Wait-ForIdle $h
    Move-CursorAway $h
    if (Should '01-text-from-a-page') { Save-Shot $h '01-text-from-a-page' }

    # The recognisers are listed by their own native names, so the Simplified Chinese one is
    # "中文(中华人民共和国)" — matched on the region, since "中文" alone also hits the two Traditional
    # entries and the first match wins.
    Select-Combo $h 'LanguageComboBox' '中华人民共和国'
    Set-Voice 'Huihui|Yaoyao|Kangkang|Hanhan'
    "opening slide-zh.png"
    Open-InGlyfo (Join-Path $src 'slide-zh.png')
    Set-Frame $h
    Wait-ForIdle $h
    Move-CursorAway $h
    if (Should '02-any-language') { Save-Shot $h '02-any-language' }

    Select-Combo $h 'LanguageComboBox' 'English'
    Set-Voice 'Zira|David|Mark|English'
    "opening label-codes.png"
    Open-InGlyfo (Join-Path $src 'label-codes.png')
    Set-Frame $h
    Wait-ForIdle $h
    Click-Element $h 'BarcodeButton'
    Wait-ForIdle $h
    Move-CursorAway $h
    if (Should '03-qr-and-barcodes') { Save-Shot $h '03-qr-and-barcodes' }

    # The PDF comes after the three images and before the history shot, so that by the time the
    # history is opened it holds a mix of pictures and PDF pages — which is what makes searching it
    # worth showing at all.
    "opening report-4471.pdf"
    Open-InGlyfo (Join-Path $src 'report-4471.pdf')
    Set-Frame $h
    Wait-ForIdle $h
    Move-CursorAway $h
    if (Should '06-pdf-pages') { Save-Shot $h '06-pdf-pages' }

    if (Should '07-batch') {
        # "Read every page" is the one route into the batch dialog that starts from a file already
        # open, so it needs no file picker driven from a script. It is also a real batch: three
        # pages, read one after another, into the same merged result.
        Click-Element $h 'PdfAllButton'
        # "Save each one as its own file" is hidden while the run is going and shown the moment
        # there is something to save, so its arrival in the tree is the run finishing.
        Wait-Element $h 'BatchSeparate'
        Start-Sleep -Seconds 1
        Move-CursorAway $h
        Save-Shot $h '07-batch' -WithDialog
        Close-Dialog $h
        Start-Sleep -Seconds 2
    }

    if (Should '04-history') {
        Click-Element $h 'HistoryButton'
        # A part number that is on the crate label and all through the PDF, so the filtered list
        # shows the same string found in a photograph and in a document — which is the point of
        # keeping the history in the first place.
        Click-Element $h 'HistorySearchBox'
        Send-Digits '4471'
        Move-CursorAway $h
        Save-Shot $h '04-history'
        Send-Esc
    }

    if (Should '05-settings') {
        Click-Element $h 'SettingsButton'
        Move-CursorAway $h
        Save-Shot $h '05-settings' -WithDialog
        Close-Dialog $h
    }

    # Put the app back the way it was found.
    if ($originalVoice) { Set-Voice ([Regex]::Escape($originalVoice)) }
    if ($originalOcr) { Select-Combo $h 'LanguageComboBox' ([Regex]::Escape($originalOcr)) }
    if ($originalUi -ge 0) {
        Click-Element $h 'SettingsButton'
        Select-ComboIndex $h 'UiLanguageComboBox' $originalUi
        Close-Dialog $h
    }
}
finally {
    foreach ($w in $imeWindows) { [void][Win]::ShowWindow($w, [Win]::SW_SHOWNA) }
}

Get-ChildItem $out -Filter *.png | Select-Object Name, Length | Format-Table -AutoSize
