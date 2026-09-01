using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Glyfo.Services;

/// <summary>
/// The notifications that appear in the corner of the screen.
/// </summary>
/// <remarks>
/// Deliberately narrow: a toast is only raised when the window cannot show the same thing itself.
/// While the window is on screen the status bar already says everything these would, and duplicating
/// it in the corner would train the user to ignore both.
///
/// Every call is swallowed on failure. Notifications can be switched off per-app in Windows
/// Settings, or by a policy on a managed machine, and none of that is a reason to interrupt
/// recognition.
/// </remarks>
internal static class Toasts
{
    /// <summary>Argument name the buttons and bodies carry; read back in <see cref="Invoked"/>.</summary>
    private const string ActionKey = "action";

    public const string ActionOpen = "open";
    public const string ActionExit = "exit";
    public const string ActionMuteStartup = "mute-startup";

    /// <summary>How much recognized text a toast shows before trailing off.</summary>
    private const int PreviewLength = 120;

    private const string Group = "glyfo";
    private const string TagStartup = "startup";
    private const string TagTray = "tray";
    private const string TagResult = "result";
    private const string TagHotkey = "hotkey";

    private static bool _registered;

    /// <summary>Raised on a background thread with the action of whatever the user clicked.</summary>
    public static event EventHandler<string>? Invoked;

    public static void Register()
    {
        try
        {
            var manager = AppNotificationManager.Default;
            manager.NotificationInvoked += (_, args) => RaiseInvoked(args.Arguments);
            manager.Register();
            _registered = true;
            Trace.Write($"Toasts.Register ok, setting={manager.Setting}");
        }
        catch (Exception exception)
        {
            // No package identity, or the platform refused. The app works, just quietly.
            Trace.Write("Toasts.Register", exception);
        }
    }

    public static void Unregister()
    {
        if (!_registered)
        {
            return;
        }

        _registered = false;

        try
        {
            AppNotificationManager.Default.Unregister();
        }
        catch (Exception)
        {
        }
    }

    /// <summary>Also called for the redirected-activation path, where the event never fires here.</summary>
    public static void RaiseInvoked(IDictionary<string, string> arguments)
    {
        var action = arguments.TryGetValue(ActionKey, out var value) ? value : ActionOpen;
        Invoked?.Invoke(null, action);
    }

    /// <summary>Tells a first-time user that the capture shortcut exists at all.</summary>
    public static void ShowStartupHint(string hotkey) => Show(
        TagStartup,
        new AppNotificationBuilder()
            .AddArgument(ActionKey, ActionOpen)
            .AddText(Loc.Get("Toast_StartupTitle"))
            .AddText(Loc.Get("Toast_StartupBody", hotkey))
            .AddButton(new AppNotificationButton(Loc.Get("Toast_DontRemind"))
                .AddArgument(ActionKey, ActionMuteStartup)));

    /// <summary>Explains where the window went, and how to get out of the app for real.</summary>
    public static void ShowMinimizedToTray(string hotkey) => Show(
        TagTray,
        new AppNotificationBuilder()
            .AddArgument(ActionKey, ActionOpen)
            .AddText(Loc.Get("Toast_MinimizedTitle"))
            .AddText(Loc.Get("Toast_MinimizedBody", hotkey))
            .AddButton(new AppNotificationButton(Loc.Get("Toast_ExitApp"))
                .AddArgument(ActionKey, ActionExit)));

    /// <summary>The result of a capture taken while the window was hidden.</summary>
    public static void ShowRecognized(string text) => Show(
        TagResult,
        new AppNotificationBuilder()
            .AddArgument(ActionKey, ActionOpen)
            .AddText(Loc.Get("Toast_ResultTitle"))
            .AddText(Preview(text))
            .AddText(Loc.Get("Toast_ResultCopied"))
            .AddButton(new AppNotificationButton(Loc.Get("Toast_OpenWindow"))
                .AddArgument(ActionKey, ActionOpen)));

    public static void ShowNoText() => Show(
        TagResult,
        new AppNotificationBuilder()
            .AddArgument(ActionKey, ActionOpen)
            .AddText(Loc.Get("Toast_NoTextTitle"))
            .AddText(Loc.Get("Toast_NoTextBody"))
            .AddButton(new AppNotificationButton(Loc.Get("Toast_OpenWindow"))
                .AddArgument(ActionKey, ActionOpen)));

    public static void ShowFailed(string message) => Show(
        TagResult,
        new AppNotificationBuilder()
            .AddArgument(ActionKey, ActionOpen)
            .AddText(Loc.Get("Toast_FailedTitle"))
            .AddText(Preview(message))
            .AddButton(new AppNotificationButton(Loc.Get("Toast_OpenWindow"))
                .AddArgument(ActionKey, ActionOpen)));

    /// <summary>
    /// Raised at startup when another app already owns both capture shortcuts. Without this the
    /// only sign is a tooltip nobody reads until they wonder why nothing happens.
    /// </summary>
    public static void ShowHotkeyUnavailable() => Show(
        TagHotkey,
        new AppNotificationBuilder()
            .AddArgument(ActionKey, ActionOpen)
            .AddText(Loc.Get("Toast_HotkeyUnavailableTitle"))
            .AddText(Loc.Get("Toast_HotkeyUnavailableBody")));

    private static void Show(string tag, AppNotificationBuilder builder)
    {
        if (!_registered)
        {
            Trace.Write($"Toasts.Show({tag}) skipped: not registered");
            return;
        }

        try
        {
            var notification = builder.BuildNotification();

            // Same tag and group means the shell replaces the previous one rather than stacking
            // them, so a run of captures leaves a single entry in the notification centre.
            notification.Tag = tag;
            notification.Group = Group;

            AppNotificationManager.Default.Show(notification);
            Trace.Write($"Toasts.Show({tag}) id={notification.Id}");
        }
        catch (Exception exception)
        {
            Trace.Write($"Toasts.Show({tag})", exception);
        }
    }

    /// <summary>Flattens the text onto one line and cuts it to something a toast can show.</summary>
    private static string Preview(string text)
    {
        var builder = new StringBuilder(PreviewLength + 1);
        var lastWasSpace = false;

        foreach (var character in text)
        {
            var isSpace = char.IsWhiteSpace(character);
            if (isSpace && (lastWasSpace || builder.Length == 0))
            {
                continue;
            }

            builder.Append(isSpace ? ' ' : character);
            lastWasSpace = isSpace;

            if (builder.Length >= PreviewLength)
            {
                return builder.ToString().TrimEnd() + "…";
            }
        }

        return builder.ToString().TrimEnd();
    }
}
