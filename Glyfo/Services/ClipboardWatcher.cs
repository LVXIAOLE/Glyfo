using System;
using System.Runtime.InteropServices;

namespace Glyfo.Services;

/// <summary>
/// Tells the window when something new lands on the clipboard.
/// </summary>
/// <remarks>
/// Built on <c>AddClipboardFormatListener</c> for the same reason <see cref="TrayIcon"/> is built on
/// <c>Shell_NotifyIcon</c>: the window is already subclassed for the global hotkeys, so a listener
/// costs one more branch in the existing window procedure. The owner must forward unhandled messages
/// to <see cref="HandleMessage"/> from that procedure.
///
/// This class deliberately knows nothing about what is on the clipboard. It owns the Win32 plumbing
/// and the three reasons an update should be ignored; deciding whether the content is worth reading
/// is the window's job, because only it can await the WinRT clipboard APIs.
/// </remarks>
internal sealed class ClipboardWatcher : IDisposable
{
    private const uint WmClipboardUpdate = 0x031D;

    /// <summary>
    /// How long after a report the next one is ignored. Several applications write a single copy in
    /// two or three formats, one message each, and reading the same picture three times would run
    /// recognition three times.
    /// </summary>
    private const int DebounceMs = 500;

    /// <summary>
    /// The clipboard sequence number as it stood after Glyfo last wrote to it.
    /// </summary>
    /// <remarks>
    /// Static because the copy helpers that have to report their writes are static, and because
    /// there is only ever one clipboard.
    ///
    /// A sequence number rather than a "we are writing" flag: <c>Clipboard.Flush</c> can bump the
    /// number a second time, so a flag set before the write and cleared on the first message back
    /// would leave the second message looking like somebody else's. Recording the final number
    /// afterwards covers both, and any genuinely foreign write pushes the number past it.
    /// </remarks>
    private static uint _ownSequence;

    private readonly IntPtr _hwnd;
    private bool _listening;
    private long _lastReportTicks;

    public ClipboardWatcher(IntPtr hwnd)
    {
        _hwnd = hwnd;
        _listening = AddClipboardFormatListener(hwnd);

        Trace.Write(_listening
            ? "clipboard listener registered"
            : $"clipboard listener NOT registered, error {Marshal.GetLastWin32Error()}");
    }

    /// <summary>Raised on the UI thread's message pump when foreign content arrives.</summary>
    public event EventHandler? ContentChanged;

    /// <summary>Whether updates are reported at all. Mirrors the user's setting.</summary>
    /// <remarks>
    /// The listener stays registered either way and this only gates the report. Adding and removing
    /// the listener as the switch is flipped would be one more piece of state that can disagree with
    /// the window, and registering it costs nothing while it is off.
    /// </remarks>
    public bool IsEnabled { get; set; }

    /// <summary>Records that the write which just completed was ours, so it is not read back.</summary>
    /// <remarks>Call after the content is on the clipboard, flush included — not before.</remarks>
    public static void NoteOwnWrite() => _ownSequence = GetClipboardSequenceNumber();

    /// <summary>
    /// Handles a window message. Returns true when it was a clipboard update, whether or not it was
    /// reported: the message is ours either way and the default procedure has nothing to do with it.
    /// </summary>
    public bool HandleMessage(uint msg)
    {
        if (msg != WmClipboardUpdate)
        {
            return false;
        }

        // Our own copy coming back. Checked before the switch so that turning the feature off does
        // not leave a stale sequence number behind to be mistaken for a foreign write later.
        var sequence = GetClipboardSequenceNumber();
        if (sequence == _ownSequence)
        {
            return true;
        }

        if (!IsEnabled)
        {
            return true;
        }

        var now = Environment.TickCount64;
        if (now - _lastReportTicks < DebounceMs)
        {
            return true;
        }

        _lastReportTicks = now;
        ContentChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void Dispose()
    {
        if (!_listening)
        {
            return;
        }

        RemoveClipboardFormatListener(_hwnd);
        _listening = false;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AddClipboardFormatListener(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetClipboardSequenceNumber();
}
