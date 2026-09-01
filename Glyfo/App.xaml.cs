using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.Threading;

namespace Glyfo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Identifies the one instance allowed to run. Any value works as long as it is stable; it
        /// is scoped to the package and the user, not global.
        /// </summary>
        private const string InstanceKey = "Glyfo";

        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // A XAML app that faults on a background thread dies as a stowed exception (0xc000027b)
            // with nothing in the event log but a module name. These three handlers are the only
            // way to learn what actually went wrong on a machine without a debugger attached.
            UnhandledException += (_, e) => Services.Trace.Write("Application.UnhandledException", e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception exception)
                {
                    Services.Trace.Write("AppDomain.UnhandledException", exception);
                }
            };
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (_, e) =>
                Services.Trace.Write("UnobservedTaskException", e.Exception);

            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // The args XAML hands us describe a plain launch and nothing else — a file opened from
            // Explorer, a share, or a sign-in start all arrive as Kind=Launch here. The real
            // activation only comes back from AppInstance.
            var activation = TryGetActivation();

            // Single instance, because the app now outlives its window. Launching it again while it
            // sits in the notification area would otherwise start a second process that adds a
            // second tray icon and fails to register the capture shortcuts the first one holds.
            Services.Trace.Write(
                $"OnLaunched pid={Environment.ProcessId} aumid={TryGetAumid()} " +
                $"kind={(activation is null ? "(unavailable)" : activation.Kind.ToString())}");

            var instance = AppInstance.FindOrRegisterForKey(InstanceKey);
            if (!instance.IsCurrent)
            {
                Services.Trace.Write("redirecting to the running instance");
                RedirectAndExit(instance, activation);
                return;
            }

            instance.Activated += OnRedirected;

            // Before any window exists: this also sets PrimaryLanguageOverride, which decides the
            // language of the strings Windows itself hands back — recognizer and voice display
            // names, and the file pickers' own chrome.
            Services.Loc.Initialize();
            Services.Toasts.Register();

            var window = new MainWindow();
            _window = window;
            Services.Trace.Write("MainWindow constructed");

            if (activation?.Kind == ExtendedActivationKind.StartupTask)
            {
                // Started at sign-in. Deliberately never activated: the shortcut, the tray icon and
                // the message loop are all in place after the constructor, and skipping Activate is
                // what keeps a window from flashing across the screen on every boot.
                window.MarkStartedHidden();
                Services.Trace.Write("started hidden from the startup task");
                return;
            }

            window.Activate();
            Services.Trace.Write("MainWindow activated");

            HandleActivation(activation);
        }

        /// <summary>
        /// The activation that started this process, or <c>null</c> if the platform will not say.
        /// </summary>
        /// <remarks>
        /// This call reaches out to the activating host over RPC, and that host is free to have
        /// gone away in the meantime — opening a file from a program that exits the instant it has
        /// handed the file over fails here with RPC_S_SERVER_UNAVAILABLE. Unhandled, that kills the
        /// app before it has drawn anything; treating it as a plain launch costs the user only the
        /// file they asked for, and they still get a working window.
        /// </remarks>
        private static AppActivationArguments? TryGetActivation()
        {
            try
            {
                return AppInstance.GetCurrent().GetActivatedEventArgs();
            }
            catch (Exception ex)
            {
                Services.Trace.Write("GetActivatedEventArgs", ex);
                return null;
            }
        }

        /// <summary>
        /// Routes a file or share activation into the window, whether it arrived with this launch
        /// or was redirected here from a second process.
        /// </summary>
        private void HandleActivation(AppActivationArguments? activation)
        {
            if (activation is null || _window is not MainWindow window)
            {
                return;
            }

            switch (activation.Kind)
            {
                case ExtendedActivationKind.File
                    when activation.Data is Windows.ApplicationModel.Activation.IFileActivatedEventArgs file:
                    Services.Trace.Write($"file activation, {file.Files.Count} item(s)");
                    _ = window.OpenActivatedFilesAsync(file.Files);
                    break;

                case ExtendedActivationKind.ShareTarget
                    when activation.Data is Windows.ApplicationModel.Activation.ShareTargetActivatedEventArgs share:
                    Services.Trace.Write("share activation");
                    _ = window.HandleShareAsync(share.ShareOperation);
                    break;
            }
        }

        /// <summary>
        /// The identity Windows files this process's notifications under, or why it has none.
        /// </summary>
        /// <remarks>
        /// Toasts are addressed by AppUserModelId. An unpackaged launch has no identity to address,
        /// which is the single most likely reason for a notification to be accepted and never shown.
        /// </remarks>
        private static string TryGetAumid()
        {
            try
            {
                uint length = 0;
                _ = GetCurrentApplicationUserModelId(ref length, null);
                if (length == 0)
                {
                    return "(none)";
                }

                var buffer = new char[length];
                return GetCurrentApplicationUserModelId(ref length, buffer) == 0
                    ? new string(buffer, 0, (int)length - 1)
                    : "(unpackaged)";
            }
            catch (Exception)
            {
                return "(unknown)";
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int GetCurrentApplicationUserModelId(ref uint length, char[]? id);

        /// <summary>
        /// Hands this launch to the instance already running and ends this process.
        /// </summary>
        /// <remarks>
        /// The redirect is awaited on a worker rather than inline: this is the UI thread, and
        /// <c>RedirectActivationToAsync</c> needs it to keep pumping to complete, so awaiting it
        /// here deadlocks. The process is then killed rather than returned from — the XAML message
        /// loop is already running and would sit there forever with no window to close.
        /// </remarks>
        private static void RedirectAndExit(AppInstance target, AppActivationArguments? activation)
        {
            if (activation is not null)
            {
                using var redirected = new AutoResetEvent(false);

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        target.RedirectActivationToAsync(activation).AsTask().GetAwaiter().GetResult();
                    }
                    catch (Exception)
                    {
                        // The other instance died between the lookup and the redirect. Nothing left
                        // to hand the activation to, and nothing useful to do about it.
                    }

                    redirected.Set();
                });

                redirected.WaitOne(TimeSpan.FromSeconds(5));
            }

            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// A second launch, or a notification click, that was handed to this instance instead.
        /// </summary>
        private void OnRedirected(object? sender, AppActivationArguments args)
        {
            if (_window is not MainWindow window)
            {
                return;
            }

            // Raised on a background thread by the platform.
            window.DispatcherQueue.TryEnqueue(() =>
            {
                if (args.Kind == ExtendedActivationKind.AppNotification &&
                    args.Data is Microsoft.Windows.AppNotifications.AppNotificationActivatedEventArgs notification)
                {
                    Services.Toasts.RaiseInvoked(notification.Arguments);
                    return;
                }

                if (args.Kind is ExtendedActivationKind.File or ExtendedActivationKind.ShareTarget)
                {
                    HandleActivation(args);
                    return;
                }

                if (args.Kind == ExtendedActivationKind.StartupTask)
                {
                    // Windows started the task while the app was already running. There is nothing
                    // to start, and nothing the user did that would justify raising a window.
                    return;
                }

                // Clicking the app again is the plainest way of asking for its window back.
                window.ShowFromTray();
            });
        }
    }
}
