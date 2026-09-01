using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Glyfo.Services;

/// <summary>
/// The notification-area icon and its context menu.
/// </summary>
/// <remarks>
/// Built on <c>Shell_NotifyIcon</c> rather than a package, because the window is already subclassed
/// for the global hotkeys: the callback message costs one more branch in the existing window
/// procedure, where a library would cost a dependency and its own hidden window.
///
/// The owner must forward unhandled messages to <see cref="HandleMessage"/> from that procedure.
/// </remarks>
internal sealed class TrayIcon : IDisposable
{
    /// <summary>Icon's callback message. WM_APP+1, which is ours to pick and never collides.</summary>
    private const uint CallbackMessage = 0x8000 + 1;

    private const uint WmLButtonUp = 0x0202;
    private const uint WmLButtonDblClk = 0x0203;
    private const uint WmRButtonUp = 0x0205;
    private const uint WmContextMenu = 0x007B;
    private const uint WmNull = 0x0000;

    private const int NimAdd = 0;
    private const int NimModify = 1;
    private const int NimDelete = 2;

    private const uint NifMessage = 0x01;
    private const uint NifIcon = 0x02;
    private const uint NifTip = 0x04;

    private const uint MfString = 0x0000;
    private const uint MfSeparator = 0x0800;

    private const uint TpmRightButton = 0x0002;
    private const uint TpmReturnCmd = 0x0100;
    private const uint TpmNoNotify = 0x0080;
    private const uint TpmLayoutRtl = 0x8000;

    private const int SmCxsmicon = 49;

    /// <summary>Resource id the compiler gives the icon named by <c>ApplicationIcon</c>.</summary>
    private const int IdiApplication = 32512;

    private const uint ImageIcon = 1;
    private const uint LrDefaultColor = 0x0000;

    private const uint CmdOpen = 0x9101;
    private const uint CmdRegion = 0x9102;
    private const uint CmdFullScreen = 0x9103;
    private const uint CmdExit = 0x9104;

    /// <summary>Tooltips are truncated by the shell at 128 characters including the terminator.</summary>
    private const int MaxTipLength = 127;

    private readonly IntPtr _hwnd;
    private readonly uint _taskbarCreated;

    private IntPtr _icon;
    private bool _added;
    private bool _disposed;

    public TrayIcon(IntPtr hwnd)
    {
        _hwnd = hwnd;

        // Explorer broadcasts this after it restarts, and every icon that was in the tray has to
        // add itself again. Without it, one Explorer crash loses the only way back into a hidden app.
        _taskbarCreated = RegisterWindowMessage("TaskbarCreated");

        _icon = CreateIconHandle();
        Add();
    }

    /// <summary>Folded into the tooltip, so the shortcut is readable without opening anything.</summary>
    public string HotkeyText { get; set; } = string.Empty;

    public event EventHandler? OpenRequested;
    public event EventHandler? CaptureRegionRequested;
    public event EventHandler? CaptureFullScreenRequested;
    public event EventHandler? ExitRequested;

    /// <summary>Re-reads the tooltip from the string table, for after a language change.</summary>
    public void Refresh()
    {
        if (_disposed || !_added)
        {
            return;
        }

        var data = CreateData(NifTip);
        Shell_NotifyIcon(NimModify, ref data);
    }

    /// <summary>
    /// Handles the messages this icon owns. Returns false for everything else, which the caller
    /// must then pass on to the original window procedure.
    /// </summary>
    public bool HandleMessage(uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (_disposed)
        {
            return false;
        }

        if (msg == _taskbarCreated && _taskbarCreated != 0)
        {
            _added = false;
            Add();
            return true;
        }

        if (msg != CallbackMessage)
        {
            return false;
        }

        // Version 4 of the notify-icon protocol is deliberately not requested: in the default
        // version the mouse message arrives whole in lParam, which is all this needs.
        switch ((uint)(lParam.ToInt64() & 0xFFFF))
        {
            case WmLButtonUp:
            case WmLButtonDblClk:
                OpenRequested?.Invoke(this, EventArgs.Empty);
                return true;

            case WmRButtonUp:
            case WmContextMenu:
                ShowMenu();
                return true;

            default:
                return true;
        }
    }

    private void Add()
    {
        var data = CreateData(NifMessage | NifIcon | NifTip);
        _added = Shell_NotifyIcon(NimAdd, ref data);
    }

    private NotifyIconData CreateData(uint flags) => new()
    {
        cbSize = Marshal.SizeOf<NotifyIconData>(),
        hWnd = _hwnd,
        uID = 1,
        uFlags = flags,
        uCallbackMessage = CallbackMessage,
        hIcon = _icon,
        szTip = Tooltip(),
        szInfo = string.Empty,
        szInfoTitle = string.Empty,
    };

    private string Tooltip()
    {
        var text = HotkeyText.Length > 0 ? Loc.Get("Tray_Tooltip", HotkeyText) : "Glyfo";
        return text.Length > MaxTipLength ? text[..MaxTipLength] : text;
    }

    /// <summary>
    /// A Win32 menu rather than a XAML flyout: the window is normally hidden when this is wanted,
    /// and a flyout needs a visible XAML root to be placed against.
    /// </summary>
    private void ShowMenu()
    {
        var menu = CreatePopupMenu();
        if (menu == IntPtr.Zero)
        {
            return;
        }

        try
        {
            AppendMenu(menu, MfString, CmdOpen, Loc.Get("Tray_Open"));
            AppendMenu(menu, MfSeparator, 0, null);
            AppendMenu(menu, MfString, CmdRegion, Loc.Get("Tray_CaptureRegion"));
            AppendMenu(menu, MfString, CmdFullScreen, Loc.Get("Tray_CaptureFullScreen"));
            AppendMenu(menu, MfSeparator, 0, null);
            AppendMenu(menu, MfString, CmdExit, Loc.Get("Tray_Exit"));

            // Both of these are load-bearing. Without the foreground call the menu never receives
            // the click that should dismiss it, and without the posted message it stays on screen
            // after the user clicks elsewhere.
            SetForegroundWindow(_hwnd);
            GetCursorPos(out var cursor);

            var flags = TpmRightButton | TpmReturnCmd | TpmNoNotify;
            if (Loc.IsRightToLeft)
            {
                flags |= TpmLayoutRtl;
            }

            var command = TrackPopupMenuEx(menu, flags, cursor.X, cursor.Y, _hwnd, IntPtr.Zero);
            PostMessage(_hwnd, WmNull, IntPtr.Zero, IntPtr.Zero);

            switch ((uint)command)
            {
                case CmdOpen:
                    OpenRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case CmdRegion:
                    CaptureRegionRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case CmdFullScreen:
                    CaptureFullScreenRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case CmdExit:
                    ExitRequested?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
        finally
        {
            DestroyMenu(menu);
        }
    }

    /// <summary>
    /// The application icon, at exactly the size the notification area asks for.
    /// </summary>
    /// <remarks>
    /// Taken from the executable's own icon resource, so the tray, the taskbar, Explorer and the
    /// Store listing all show the same mark. <c>LoadImage</c> picks the entry matching
    /// <c>SM_CXSMICON</c> — the .ico carries 16/20/24/32/40/48, which covers every scaling the shell
    /// asks for without anything being resampled.
    /// </remarks>
    private static IntPtr CreateIconHandle()
    {
        var size = GetSystemMetrics(SmCxsmicon);
        if (size < 16)
        {
            size = 16;
        }

        var handle = LoadImage(GetModuleHandle(null), MakeIntResource(IdiApplication), ImageIcon, size, size, LrDefaultColor);
        return handle != IntPtr.Zero ? handle : DrawIconHandle(size);
    }

    /// <summary>
    /// Redraws the mark, for the case where the icon resource cannot be loaded.
    /// </summary>
    /// <remarks>
    /// Only reachable if the executable was built without its icon resource. It mirrors the
    /// simplified small-size drawing from <c>tools\IconArt.ps1</c> — the tile and the two text bars,
    /// no capture brackets, because below 32 px the brackets smear into the tile.
    /// </remarks>
    private static IntPtr DrawIconHandle(int size)
    {
        var s = (float)size;

        using var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent);

            using (var path = RoundedRect(0f, 0f, s, s, s * 0.225f))
            using (var brush = new LinearGradientBrush(
                new RectangleF(-1f, -1f, s + 2f, s + 2f),
                Color.FromArgb(255, 0x4C, 0xC2, 0xFF),
                Color.FromArgb(255, 0x00, 0x67, 0xC0),
                45f))
            {
                graphics.FillPath(brush, path);
            }

            using var white = new SolidBrush(Color.White);
            foreach (var bar in new[] { new[] { 0.235f, 0.330f, 0.530f, 0.115f }, new[] { 0.235f, 0.545f, 0.360f, 0.115f } })
            {
                var height = bar[3] * s;
                using var capsule = RoundedRect(bar[0] * s, bar[1] * s, bar[2] * s, height, height / 2f);
                graphics.FillPath(white, capsule);
            }
        }

        return bitmap.GetHicon();
    }

    private static GraphicsPath RoundedRect(float x, float y, float width, float height, float radius)
    {
        var diameter = radius * 2f;
        var path = new GraphicsPath();

        path.AddArc(x, y, diameter, diameter, 180, 90);
        path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
        path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
        path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_added)
        {
            var data = CreateData(0);
            Shell_NotifyIcon(NimDelete, ref data);
            _added = false;
        }

        if (_icon != IntPtr.Zero)
        {
            DestroyIcon(_icon);
            _icon = IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NotifyIconData
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szTip;
        public uint dwState;
        public uint dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string szInfo;
        public uint uVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string szInfoTitle;
        public uint dwInfoFlags;
        public Guid guidItem;
        public IntPtr hBalloonIcon;
    }

    [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW", CharSet = CharSet.Unicode)]
    private static extern bool Shell_NotifyIcon(int message, ref NotifyIconData data);

    [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW", CharSet = CharSet.Unicode)]
    private static extern uint RegisterWindowMessage(string message);

    [DllImport("user32.dll")]
    private static extern IntPtr CreatePopupMenu();

    [DllImport("user32.dll", EntryPoint = "AppendMenuW", CharSet = CharSet.Unicode)]
    private static extern bool AppendMenu(IntPtr menu, uint flags, uint id, string? item);

    [DllImport("user32.dll")]
    private static extern bool DestroyMenu(IntPtr menu);

    [DllImport("user32.dll")]
    private static extern int TrackPopupMenuEx(IntPtr menu, uint flags, int x, int y, IntPtr hWnd, IntPtr parameters);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out Point point);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "PostMessageW")]
    private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr icon);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    [DllImport("user32.dll", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadImage(IntPtr instance, IntPtr name, uint type, int cx, int cy, uint load);

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string? moduleName);

    /// <summary>The MAKEINTRESOURCE macro: an id passed where a name pointer is expected.</summary>
    private static IntPtr MakeIntResource(int id) => new(id);
}
