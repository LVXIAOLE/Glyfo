using System;
using System.IO;

namespace Glyfo.Services;

/// <summary>
/// A one-file activity log, for the parts of the app that fail where nobody can see them.
/// </summary>
/// <remarks>
/// The notification and tray paths run with no window on screen and swallow their exceptions on
/// purpose — a toast that cannot be shown must never interrupt recognition. That leaves no way to
/// tell "the platform refused" from "the code never ran", which is exactly the question support
/// asks. A handful of lines per session in the temp folder answers it.
///
/// Writes are best-effort and never throw: this is diagnostics, not a feature.
/// </remarks>
internal static class Trace
{
    private static readonly object Gate = new();
    private static string? _path;

    /// <summary>Where the log ended up, for showing the user when something needs reporting.</summary>
    public static string Path => _path ??= System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Glyfo.log");

    public static void Write(string message)
    {
        try
        {
            lock (Gate)
            {
                // Truncate rather than roll: nothing here is worth keeping across a day of use, and
                // an unbounded log in a Store app is a bug waiting to be filed.
                if (File.Exists(Path) && new FileInfo(Path).Length > 64 * 1024)
                {
                    File.Delete(Path);
                }

                File.AppendAllText(Path, $"{DateTime.Now:HH:mm:ss.fff}  {message}{Environment.NewLine}");
            }
        }
        catch (Exception)
        {
            // A log that throws is worse than no log.
        }
    }

    /// <summary>Records an exception with the operation that raised it.</summary>
    public static void Write(string operation, Exception exception) =>
        Write($"{operation} FAILED  {exception.GetType().Name}: {exception.Message} (0x{exception.HResult:X8}){Environment.NewLine}{exception}");
}
