using Glyfo.Services;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Graphics;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using Size = System.Drawing.Size;

namespace Glyfo;

public sealed partial class MainWindow : Window
{
    private const int FullHotkeyId = 9001;
    private const int RegionHotkeyId = 9002;
    private const uint WmHotkey = 0x0312;
    private const uint ModAlt = 0x0001;
    private const uint ModControl = 0x0002;
    private const uint ModShift = 0x0004;
    private const uint VkG = 0x47;
    private const uint VkR = 0x52;
    private const uint VkZ = 0x5A;
    private const int SmCxscreen = 0;
    private const int SmCyscreen = 1;
    private const int SmXvirtualscreen = 76;
    private const int SmYvirtualscreen = 77;
    private const int SmCxvirtualscreen = 78;
    private const int SmCyvirtualscreen = 79;
    private const int SmCxicon = 11;
    private const int GwlWndproc = -4;

    /// <summary>Resource id the compiler gives the icon named by <c>ApplicationIcon</c>.</summary>
    private const int IdiApplication = 32512;
    private const uint ImageIcon = 1;
    private const int MaxHistoryItems = 20;

    /// <summary>
    /// How many times the "it is still running, in the tray" notice is worth showing. Three covers
    /// the user who was not looking the first time without nagging the one who was.
    /// </summary>
    private const int MaxTrayHints = 3;

    /// <summary>
    /// How long the startup hint waits. Long enough that it does not land while the user is still
    /// taking in the window it is talking about.
    /// </summary>
    private const int StartupHintDelayMs = 1500;

    /// <summary>
    /// Must match the TaskId of the windows.startupTask extension in Package.appxmanifest. The two
    /// together are one identifier; changing either side alone means the app asks Windows about a
    /// task that does not exist.
    /// </summary>
    private const string StartupTaskId = "GlyfoStartup";

    private static readonly string TempRoot = Path.Combine(Path.GetTempPath(), "Glyfo");

    private readonly OcrEngineFactory _engines = new();
    private readonly SpeechService _speech = new();
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly IntPtr _hwnd;
    private readonly WndProcDelegate _wndProcDelegate;
    private readonly ObservableCollection<HistoryItem> _history = new();
    private readonly List<string> _sessionTempFiles = new();

    private TranslationService? _translation;
    private TrayIcon? _tray;
    private AppWindow? _appWindow;
    private StartupTask? _startupTask;
    private IntPtr _oldWndProc;
    private string _currentImagePath = string.Empty;
    private string _currentImageName = string.Empty;

    /// <summary>Which shortcut region capture ended up on; folded into the button's tooltip.</summary>
    private string _captureHotkeyText = string.Empty;

    /// <summary>False when every candidate shortcut was already taken by another app.</summary>
    private bool _hotkeyAvailable;

    private bool _autoFit = true;
    private bool _isBusy;
    private bool _isClosed;

    /// <summary>Whether the window is parked in the notification area rather than merely covered.</summary>
    private bool _isHidden;

    /// <summary>Lets the close through instead of hiding it; only the tray's Exit item sets it.</summary>
    private bool _exitRequested;

    // Guards against the handlers of controls this code is itself repopulating: rebuilding the
    // language pickers raises SelectionChanged, and acting on that would persist a value the user
    // never chose.
    private bool _suppressUiLanguageChange;
    private bool _suppressOcrLanguageChange;
    private bool _suppressVoiceChange;
    private bool _suppressStartupToggle;

    /// <summary>Set once the user picks a voice by hand, after which recognition stops choosing one.</summary>
    private bool _voicePinned;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Set before the Toggled handler can matter — assigning IsOn raises it, and writing the
        // stored value straight back is harmless.
        RepairVersionsToggle.IsOn = AppSettings.Current.RepairVersionNumbers;
        CloseToTrayToggle.IsOn = AppSettings.Current.CloseToTray;

        HistoryListView.ItemsSource = _history;
        _history.CollectionChanged += (_, _) =>
            HistoryEmptyText.Visibility = _history.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _hwnd = WindowNative.GetWindowHandle(this);
        _wndProcDelegate = WndProc;
        _oldWndProc = SetWindowLongPtr(_hwnd, GwlWndproc, Marshal.GetFunctionPointerForDelegate(_wndProcDelegate));
        RegisterHotkeys();
        InitializeTray();

        var windowId = Win32Interop.GetWindowIdFromWindow(_hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        _appWindow = appWindow;
        appWindow.Closing += AppWindowClosing;
        ApplyWindowIcon(appWindow);

        // AppWindow.Resize takes raw pixels while the XAML inside is laid out in DIPs. Passing a
        // fixed 1280 on a 150% display produced an 853 DIP window, which is narrower than the two
        // toolbars need and clipped their trailing buttons.
        var scale = GetDpiForWindow(_hwnd) / 96.0;
        appWindow.Resize(new SizeInt32((int)(1280 * scale), (int)(820 * scale)));

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            // Below this the action bar and the text-pane toolbar start clipping.
            presenter.PreferredMinimumWidth = 1060;
            presenter.PreferredMinimumHeight = 640;
        }

        _speech.PlaybackEnded += (_, _) => _dispatcherQueue.TryEnqueue(() =>
        {
            if (!_isClosed)
            {
                SpeakButton.IsEnabled = true;
            }
        });

        CleanupOldTempFiles();
        InitializeVoices();
        InitializeStartupToggle();
        ApplyLanguage();

        _engines.OptionsChanged += (_, _) => _dispatcherQueue.TryEnqueue(() =>
        {
            if (!_isClosed)
            {
                RefreshLanguageOptions();
            }
        });

        Loc.Changed += LanguageChanged;
        Toasts.Invoked += ToastInvoked;
        Closed += OnClosed;

        _ = ProbeOnDeviceAiAsync();
        _ = AnnounceStartupAsync();
    }

    // ---------------------------------------------------------------- localization

    private void LanguageChanged(object? sender, EventArgs e) => ApplyLanguage();

    /// <summary>
    /// Writes every user-visible string in the window from the current string table.
    /// </summary>
    /// <remarks>
    /// The XAML deliberately carries no display text, so this runs once at startup and again on
    /// every language change. That is what makes switching language take effect immediately instead
    /// of on the next launch: XAML resolves <c>x:Uid</c> resources once when the tree is loaded, and
    /// nothing short of rebuilding the window would revisit them.
    /// </remarks>
    private void ApplyLanguage()
    {
        ApplyFlowDirection();
        BuildUiLanguagePicker();

        PreviewPlaceholderText.Text = Loc.Get("Preview_Placeholder");

        CopyImageLabel.Text = Loc.Get("Btn_CopyImage");
        SetTip(CopyImageButton, Loc.Get("Tip_CopyImage"));
        SaveImageLabel.Text = Loc.Get("Btn_SaveImage");
        SetTip(SaveImageButton, Loc.Get("Tip_SaveImage"));
        FitToWindowLabel.Text = Loc.Get("Btn_FitToWindow");
        SetTip(FitToWindowButton, Loc.Get("Tip_FitToWindow"));
        SetTip(ActualSizeButton, Loc.Get("Tip_ActualSize"));

        VoiceComboBox.PlaceholderText = Loc.Get("Voice_Placeholder");
        SetTip(VoiceComboBox, Loc.Get("Tip_Voice"));
        SetTip(SpeakButton, Loc.Get("Tip_Speak"));
        SetTip(StopSpeakButton, Loc.Get("Tip_StopSpeak"));
        SetTip(CopyTextButton, Loc.Get("Tip_CopyText"));
        RemoveLineBreaksButton.Content = Loc.Get("Btn_RemoveLineBreaks");
        SetTip(RemoveLineBreaksButton, Loc.Get("Tip_RemoveLineBreaks"));
        RemoveSpacesButton.Content = Loc.Get("Btn_RemoveSpaces");
        SetTip(RemoveSpacesButton, Loc.Get("Tip_RemoveSpaces"));
        ResultTextBox.PlaceholderText = Loc.Get("Result_Placeholder");

        OpenLabel.Text = Loc.Get("Btn_OpenFile");
        SetTip(OpenButton, Loc.Get("Tip_OpenFile"));
        CaptureLabel.Text = Loc.Get("Btn_Capture");
        SetTip(CaptureButton, Loc.Get("Tip_Capture", _captureHotkeyText));
        PasteLabel.Text = Loc.Get("Btn_Paste");
        SetTip(PasteButton, Loc.Get("Tip_Paste"));
        HistoryLabel.Text = Loc.Get("Btn_History");
        SetTip(HistoryButton, Loc.Get("Tip_History"));
        HistoryEmptyText.Text = Loc.Get("History_Empty");
        SetTip(SettingsButton, Loc.Get("Tip_Settings"));

        SettingsHeaderText.Text = Loc.Get("Settings_Header");
        StartupToggle.Header = Loc.Get("Setting_Startup");
        StartupToggle.OnContent = Loc.Get("Common_On");
        StartupToggle.OffContent = Loc.Get("Common_Off");
        StartupDescription.Text = Loc.Get("Setting_Startup_Desc");
        CloseToTrayToggle.Header = Loc.Get("Setting_CloseToTray");
        CloseToTrayToggle.OnContent = Loc.Get("Common_On");
        CloseToTrayToggle.OffContent = Loc.Get("Common_Off");
        CloseToTrayDescription.Text = Loc.Get("Setting_CloseToTray_Desc");
        RepairVersionsToggle.Header = Loc.Get("Setting_RepairNumbers");
        RepairVersionsToggle.OnContent = Loc.Get("Common_On");
        RepairVersionsToggle.OffContent = Loc.Get("Common_Off");
        RepairVersionsDescription.Text = Loc.Get("Setting_RepairNumbers_Desc");

        LanguageComboBox.PlaceholderText = Loc.Get("Lang_Placeholder");
        SetTip(LanguageComboBox, Loc.Get("Tip_OcrLanguage"));
        TranslateLabel.Text = Loc.Get("Btn_Translate");
        SetTip(TranslateButton, Loc.Get("Tip_Translate"));
        BarcodeLabel.Text = Loc.Get("Btn_Barcode");
        SetTip(BarcodeButton, Loc.Get("Tip_Barcode"));
        RecognizeLabel.Text = Loc.Get("Btn_Recognize");
        SetTip(RecognizeButton, Loc.Get("Tip_Recognize"));

        // The engine builds its own option names, so it has to be asked again in the new language.
        RefreshLanguageOptions();

        // The menu is built fresh on every right-click and so needs nothing; the tooltip is state
        // held by the shell.
        _tray?.Refresh();
    }

    /// <summary>
    /// Mirrors the layout for Arabic, Hebrew and Persian.
    /// </summary>
    /// <remarks>
    /// The flyouts are set separately because their content lives in a popup rather than under
    /// <c>RootGrid</c>, so it never inherits the change. The title bar is deliberately left running
    /// left to right: the caption buttons stay on the window's right whatever the content does, and
    /// mirroring that row would slide the app name underneath them.
    /// </remarks>
    private void ApplyFlowDirection()
    {
        var flow = Loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        RootGrid.FlowDirection = flow;
        AppTitleBar.FlowDirection = FlowDirection.LeftToRight;
        HistoryFlyoutRoot.FlowDirection = flow;
        SettingsFlyoutRoot.FlowDirection = flow;

        foreach (var item in TranslateMenu.Items)
        {
            item.FlowDirection = flow;
        }
    }

    /// <summary>
    /// Sets the tooltip and, for anything without a visible label of its own, the name a screen
    /// reader announces. An icon-only button with no name is read out as just "button".
    /// </summary>
    private static void SetTip(FrameworkElement element, string text)
    {
        ToolTipService.SetToolTip(element, text);
        AutomationProperties.SetName(element, text);
    }

    private void BuildUiLanguagePicker()
    {
        // "System default" is itself a translated string, so the list is rebuilt rather than
        // reordered — there is no item whose text survives a language change unchanged.
        var items = new List<UiLanguage> { new(string.Empty, Loc.Get("Lang_SystemDefault")) };
        items.AddRange(Loc.Languages);

        var selected = items.FindIndex(item =>
            string.Equals(item.Tag, Loc.SelectedTag, StringComparison.OrdinalIgnoreCase));

        _suppressUiLanguageChange = true;
        try
        {
            UiLanguageComboBox.ItemsSource = items;
            UiLanguageComboBox.SelectedIndex = selected >= 0 ? selected : 0;
        }
        finally
        {
            _suppressUiLanguageChange = false;
        }

        SetTip(UiLanguageComboBox, Loc.Get("Setting_UiLanguage"));
    }

    private void UiLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressUiLanguageChange || UiLanguageComboBox.SelectedItem is not UiLanguage language)
        {
            return;
        }

        // Raises Loc.Changed, which re-enters ApplyLanguage; the suppression flag above keeps the
        // rebuild from looping back through here.
        Loc.Select(language.Tag);
    }

    // ---------------------------------------------------------------- startup probes

    /// <summary>
    /// Copilot+ features are probed off the UI thread: the first call can block on a model
    /// download, and the app has to stay usable with the built-in engine while that happens.
    /// </summary>
    private async Task ProbeOnDeviceAiAsync()
    {
        await Task.Run(() => _engines.ProbeAdvancedEngineAsync()).ConfigureAwait(false);

        var translation = await Task.Run(TranslationService.TryCreateAsync).ConfigureAwait(false);
        if (translation is null)
        {
            return;
        }

        _dispatcherQueue.TryEnqueue(() =>
        {
            // The probe outlives the window on a quick close, and OnClosed has already disposed
            // everything it knew about — hand this one straight back rather than leaking the model.
            if (_isClosed)
            {
                translation.Dispose();
                return;
            }

            _translation = translation;
            BuildTranslateMenu();
            TranslateButton.Visibility = Visibility.Visible;
        });
    }

    private void BuildTranslateMenu()
    {
        TranslateMenu.Items.Clear();
        foreach (var target in TranslationService.Targets)
        {
            var item = new MenuFlyoutItem
            {
                Text = target.DisplayName,
                Tag = target,
                FlowDirection = Loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
            };
            item.Click += TranslateMenuItemClick;
            TranslateMenu.Items.Add(item);
        }
    }

    private void InitializeVoices()
    {
        var voices = SpeechService.GetVoices();

        _suppressVoiceChange = true;
        try
        {
            VoiceComboBox.ItemsSource = voices;

            if (voices.Count == 0)
            {
                VoiceComboBox.IsEnabled = false;
                SpeakButton.IsEnabled = false;
                return;
            }

            var preferred = SpeechService.DefaultVoice;
            var index = preferred is null ? -1 : voices.ToList().FindIndex(v => v.Id == preferred.Id);
            VoiceComboBox.SelectedIndex = index >= 0 ? index : 0;
        }
        finally
        {
            _suppressVoiceChange = false;
        }
    }

    private void RefreshLanguageOptions()
    {
        var previous = (LanguageComboBox.SelectedItem as OcrOption)?.PreferenceKey;
        var options = _engines.GetOptions();

        _suppressOcrLanguageChange = true;
        try
        {
            LanguageComboBox.ItemsSource = options;
            LanguageComboBox.IsEnabled = options.Count > 0;
            EngineStatusText.Text = "· " + _engines.StatusDescription;

            if (options.Count == 0)
            {
                ShowLanguagePackGuidance();
                return;
            }

            var restored = previous is null
                ? -1
                : options.ToList().FindIndex(o =>
                    string.Equals(o.PreferenceKey, previous, StringComparison.OrdinalIgnoreCase));

            LanguageComboBox.SelectedIndex = restored >= 0 ? restored : _engines.GetDefaultOptionIndex(options);
        }
        finally
        {
            _suppressOcrLanguageChange = false;
        }
    }

    /// <summary>
    /// Remembers an explicit choice so the next launch starts on the same recognizer. Skipped while
    /// the list is being rebuilt, when the "selection" is only this code restoring what was there.
    /// </summary>
    private void OcrLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressOcrLanguageChange || LanguageComboBox.SelectedItem is not OcrOption option)
        {
            return;
        }

        AppSettings.Current.OcrLanguage = option.PreferenceKey;
    }

    private void ShowLanguagePackGuidance()
    {
        SetStatus(Loc.Get("Status_LanguagePackMissing"), InfoBarSeverity.Warning);

        var open = new Button { Content = Loc.Get("Btn_OpenLanguageSettings") };
        open.Click += async (_, _) =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:regionlanguage"));

        // After SetStatus, which clears whatever button the previous message left behind.
        StatusBar.ActionButton = open;
    }

    // ---------------------------------------------------------------- input sources

    private async void OpenClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker();
            foreach (var extension in SupportedExtensions)
            {
                picker.FileTypeFilter.Add(extension);
            }

            InitializeWithWindow.Initialize(picker, _hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file is null)
            {
                return;
            }

            await LoadImageAsync(file);
            SetStatus(Loc.Get("Status_Loaded", file.Name), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_OpenFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    private async void CaptureRegionClick(object sender, RoutedEventArgs e)
    {
        await CaptureRegionAndRunAsync();
    }

    private async void PasteClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var clipboard = Clipboard.GetContent();

            if (clipboard.Contains(StandardDataFormats.StorageItems))
            {
                var items = await clipboard.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file && IsSupportedImage(file.Name))
                    {
                        await LoadImageAsync(file);
                        SetStatus(Loc.Get("Status_Loaded", file.Name), InfoBarSeverity.Success);
                        return;
                    }
                }
            }

            if (clipboard.Contains(StandardDataFormats.Bitmap))
            {
                var streamRef = await clipboard.GetBitmapAsync();
                if (streamRef is not null)
                {
                    var name = Loc.Get("Source_Clipboard");
                    await LoadImageAsync(streamRef, name);
                    SetStatus(Loc.Get("Status_Loaded", name), InfoBarSeverity.Success);
                    return;
                }
            }

            if (clipboard.Contains(StandardDataFormats.Text))
            {
                ResultTextBox.Text = await clipboard.GetTextAsync();
                SetStatus(Loc.Get("Status_ClipboardText"), InfoBarSeverity.Informational);
                return;
            }

            SetStatus(Loc.Get("Status_ClipboardEmpty"), InfoBarSeverity.Warning);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_ClipboardFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    /// <summary>
    /// Only claims the drop when the payload is something the app can actually open. Accepting
    /// everything made the cursor promise a copy that <see cref="PreviewDrop"/> would then silently
    /// discard.
    /// </summary>
    private void PreviewDragOver(object sender, DragEventArgs e)
    {
        var accepted = e.DataView.Contains(StandardDataFormats.StorageItems)
                    || e.DataView.Contains(StandardDataFormats.Bitmap);

        e.AcceptedOperation = accepted ? DataPackageOperation.Copy : DataPackageOperation.None;

        if (accepted)
        {
            e.DragUIOverride.Caption = Loc.Get("Btn_Recognize");
            e.DragUIOverride.IsCaptionVisible = true;
        }

        e.Handled = true;
    }

    private async void PreviewDrop(object sender, DragEventArgs e)
    {
        try
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file && IsSupportedImage(file.Name))
                    {
                        await LoadImageAsync(file);
                        SetStatus(Loc.Get("Status_Loaded", file.Name), InfoBarSeverity.Success);
                        return;
                    }
                }
            }

            if (e.DataView.Contains(StandardDataFormats.Bitmap))
            {
                var streamRef = await e.DataView.GetBitmapAsync();
                if (streamRef is not null)
                {
                    var name = Loc.Get("Source_Dropped");
                    await LoadImageAsync(streamRef, name);
                    SetStatus(Loc.Get("Status_Loaded", name), InfoBarSeverity.Success);
                }
            }
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_DropFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    // ---------------------------------------------------------------- activation entry points

    /// <summary>
    /// Opens an image handed over by Explorer's "Open with", and reads it.
    /// </summary>
    /// <remarks>
    /// Only the first image is taken. The window shows one image beside its text; queueing a
    /// multi-selection would need a batch mode that does not exist, and picking one silently is
    /// less confusing than opening several windows.
    /// </remarks>
    public async Task OpenActivatedFilesAsync(IReadOnlyList<IStorageItem> items)
    {
        foreach (var item in items)
        {
            if (item is StorageFile file && IsSupportedImage(file.Name))
            {
                await OpenExternalAsync(() => LoadImageAsync(file), file.Name, file.Name);
                return;
            }
        }

        ShowFromTray();
        SetStatus(Loc.Get("Status_OpenFailed", Loc.Get("Status_NeedImage")), InfoBarSeverity.Warning);
    }

    /// <summary>
    /// Receives an image from the Windows share sheet.
    /// </summary>
    /// <remarks>
    /// The three ShareOperation reports are not optional bookkeeping: the share flyout stays on
    /// screen, spinning, until one of them arrives. ReportCompleted has to come after the bytes
    /// have been copied to the temp folder — the source app is free to drop the data once the
    /// operation ends — but before recognition, which can take seconds the flyout should not wait
    /// for. <see cref="LoadImageAsync(StorageFile)"/> does that copy, so the seam sits between it
    /// and <see cref="RunOcrAsync"/>.
    /// </remarks>
    public async Task HandleShareAsync(ShareOperation operation)
    {
        try
        {
            operation.ReportStarted();

            var data = operation.Data;
            string displayName;

            // Both shapes turn up in practice: Explorer and Photos share a file, while Snipping
            // Tool and browsers share a bitmap with nothing on disk behind it.
            if (data.Contains(StandardDataFormats.StorageItems) &&
                await FirstSupportedImageAsync(data) is { } file)
            {
                displayName = file.Name;
                ShowFromTray();
                await LoadImageAsync(file);
            }
            else if (data.Contains(StandardDataFormats.Bitmap))
            {
                displayName = Loc.Get("Source_Shared");
                ShowFromTray();
                await LoadImageAsync(await data.GetBitmapAsync(), displayName);
            }
            else
            {
                operation.ReportError(Loc.Get("Status_NeedImage"));
                return;
            }

            operation.ReportCompleted();

            SetStatus(Loc.Get("Status_Loaded", displayName), InfoBarSeverity.Success);
            await RunOcrAsync(displayName);
        }
        catch (Exception ex)
        {
            Trace.Write("HandleShareAsync", ex);
            SetStatus(Loc.Get("Status_OpenFailed", ex.Message), InfoBarSeverity.Error);

            try
            {
                operation.ReportError(ex.Message);
            }
            catch (Exception)
            {
                // Already completed, or the source app has gone. Nobody left to report to.
            }
        }
    }

    /// <summary>Started at sign-in: everything is running, only the window was never shown.</summary>
    public void MarkStartedHidden() => _isHidden = true;

    /// <summary>
    /// Brings the window up, loads the image and reads it. Opening a file through Glyfo has
    /// exactly one meaning, so making the user press Recognize afterwards would be a wasted step.
    /// </summary>
    private async Task OpenExternalAsync(Func<Task> load, string displayName, string sourceLabel)
    {
        ShowFromTray();

        try
        {
            await load();
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_OpenFailed", ex.Message), InfoBarSeverity.Error);
            return;
        }

        SetStatus(Loc.Get("Status_Loaded", displayName), InfoBarSeverity.Success);
        await RunOcrAsync(sourceLabel);
    }

    private static async Task<StorageFile?> FirstSupportedImageAsync(DataPackageView data)
    {
        foreach (var item in await data.GetStorageItemsAsync())
        {
            if (item is StorageFile file && IsSupportedImage(file.Name))
            {
                return file;
            }
        }

        return null;
    }

    // ---------------------------------------------------------------- image preview

    private async Task LoadImageAsync(StorageFile file)
    {
        using var source = (await file.OpenAsync(FileAccessMode.Read)).AsStreamForRead();
        await LoadImageFromStreamAsync(source, file.Name);
    }

    private async Task LoadImageAsync(RandomAccessStreamReference streamRef, string displayName)
    {
        using var source = (await streamRef.OpenReadAsync()).AsStreamForRead();
        await LoadImageFromStreamAsync(source, displayName);
    }

    private async Task LoadImageFromStreamAsync(Stream source, string displayName)
    {
        var tempPath = CreateTempImagePath(displayName);
        await using (var destination = File.Create(tempPath))
        {
            await source.CopyToAsync(destination);
        }

        _sessionTempFiles.Add(tempPath);
        LoadPreview(tempPath, displayName);
    }

    private void LoadPreview(string imagePath, string displayName)
    {
        var bitmap = new BitmapImage();
        bitmap.ImageOpened += (_, _) =>
        {
            _autoFit = true;
            FitToWindow();
        };
        bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);

        PreviewImage.Source = bitmap;
        PreviewImage.Visibility = Visibility.Visible;
        PreviewPlaceholder.Visibility = Visibility.Collapsed;

        _currentImagePath = imagePath;
        _currentImageName = displayName;
        ResultTextBox.Text = string.Empty;
    }

    private void PreviewViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (ZoomText is null)
        {
            return;
        }

        ZoomText.Text = $"{Math.Round(PreviewScrollViewer.ZoomFactor * 100)}%";

        if (!e.IsIntermediate && _autoFit)
        {
            // A manual pinch/ctrl-scroll takes the view off the fit factor; stop re-fitting on resize.
            var fit = CalculateFitFactor();
            if (fit is not null && Math.Abs(fit.Value - PreviewScrollViewer.ZoomFactor) > 0.005f)
            {
                _autoFit = false;
            }
        }
    }

    private void PreviewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_autoFit)
        {
            FitToWindow();
        }
    }

    private void FitToWindowClick(object sender, RoutedEventArgs e)
    {
        _autoFit = true;
        FitToWindow();
    }

    private void ActualSizeClick(object sender, RoutedEventArgs e)
    {
        _autoFit = false;
        PreviewScrollViewer.ChangeView(null, null, 1f);
    }

    private void FitToWindow()
    {
        var factor = CalculateFitFactor();
        if (factor is not null)
        {
            PreviewScrollViewer.ChangeView(null, null, factor.Value);
        }
    }

    /// <summary>
    /// Uses the ScrollViewer's own size rather than ViewportWidth/Height: the viewport properties
    /// are expressed in content coordinates and therefore already scale with the zoom factor.
    /// </summary>
    private float? CalculateFitFactor()
    {
        var imageWidth = PreviewImage.ActualWidth;
        var imageHeight = PreviewImage.ActualHeight;
        var available = new Windows.Foundation.Size(
            PreviewScrollViewer.ActualWidth - 16,
            PreviewScrollViewer.ActualHeight - 16);

        if (imageWidth <= 0 || imageHeight <= 0 || available.Width <= 0 || available.Height <= 0)
        {
            return null;
        }

        var factor = Math.Min(available.Width / imageWidth, available.Height / imageHeight);

        // Never blow a small crop up past its native size — that only adds blur.
        factor = Math.Min(factor, 1.0);
        factor = Math.Clamp(factor, PreviewScrollViewer.MinZoomFactor, PreviewScrollViewer.MaxZoomFactor);
        return (float)factor;
    }

    private async void CopyImageClick(object sender, RoutedEventArgs e)
    {
        if (!HasImage())
        {
            return;
        }

        try
        {
            var file = await StorageFile.GetFileFromPathAsync(_currentImagePath);
            var data = new DataPackage();
            data.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
            Clipboard.SetContent(data);
            Clipboard.Flush();
            SetStatus(Loc.Get("Status_ImageCopied"), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_ImageCopyFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    private async void SaveImageClick(object sender, RoutedEventArgs e)
    {
        if (!HasImage())
        {
            return;
        }

        try
        {
            var extension = Path.GetExtension(_currentImagePath);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".png";
            }

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = Path.GetFileNameWithoutExtension(_currentImageName) is { Length: > 0 } name
                    ? name
                    : "Glyfo"
            };
            picker.FileTypeChoices.Add(
                Loc.Get("FileType_Image", extension.TrimStart('.').ToUpperInvariant()),
                new[] { extension });
            InitializeWithWindow.Initialize(picker, _hwnd);

            var target = await picker.PickSaveFileAsync();
            if (target is null)
            {
                return;
            }

            var source = await StorageFile.GetFileFromPathAsync(_currentImagePath);
            await source.CopyAndReplaceAsync(target);
            SetStatus(Loc.Get("Status_ImageSaved", target.Name), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_ImageSaveFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    // ---------------------------------------------------------------- recognition

    private async void RunClick(object sender, RoutedEventArgs e)
    {
        await RunOcrAsync();
    }

    private async Task RunOcrAsync(string? sourceLabel = null)
    {
        if (_isBusy)
        {
            return;
        }

        if (!HasImage())
        {
            return;
        }

        if (LanguageComboBox.SelectedItem is not OcrOption option)
        {
            // No toast for this one: the answer is a paragraph of guidance and a button, so the
            // window is the only place it fits. Bring it back and let it say so.
            if (_isHidden)
            {
                ShowFromTray();
            }

            ShowLanguagePackGuidance();
            return;
        }

        SetBusy(true);
        try
        {
            SetStatus(Loc.Get("Status_Recognizing"), InfoBarSeverity.Informational);

            using var bitmap = await ImageLoader.LoadAsync(_currentImagePath);
            var result = await option.Engine.RecognizeAsync(bitmap, NullIfEmpty(option.LanguageTag));

            ResultTextBox.Text = result.Text;

            if (string.IsNullOrWhiteSpace(result.Text))
            {
                SetStatus(Loc.Get("Status_NoText", result.EngineName), InfoBarSeverity.Warning);

                if (_isHidden)
                {
                    Toasts.ShowNoText();
                }
            }
            else
            {
                MatchVoiceToText(result.Text);

                SetStatus(
                    result.MeanConfidence is null
                        ? Loc.Get("Status_Done", result.EngineName)
                        : Loc.Get(
                            "Status_DoneConfidence",
                            result.EngineName,
                            Math.Round(result.MeanConfidence.Value * 100)),
                    InfoBarSeverity.Success);

                AddHistoryItem(sourceLabel ?? _currentImageName, result.Text, result.MeanConfidence);

                // Captured from the tray, so the status bar just written is on a window nobody can
                // see. Put the text where it is useful without making the user open anything.
                if (_isHidden)
                {
                    CopyTextToClipboard(result.Text);
                    Toasts.ShowRecognized(result.Text);
                }
            }
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_RecognizeFailed", ex.Message), InfoBarSeverity.Error);

            if (_isHidden)
            {
                Toasts.ShowFailed(ex.Message);
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void ScanBarcodeClick(object sender, RoutedEventArgs e)
    {
        if (_isBusy || !HasImage())
        {
            return;
        }

        SetBusy(true);
        try
        {
            SetStatus(Loc.Get("Status_Scanning"), InfoBarSeverity.Informational);

            using var bitmap = await ImageLoader.LoadAsync(_currentImagePath);
            var hits = await BarcodeService.DecodeAsync(bitmap);

            if (hits.Count == 0)
            {
                SetStatus(Loc.Get("Status_NoBarcode"), InfoBarSeverity.Warning);
                return;
            }

            ResultTextBox.Text = string.Join('\n', hits.Select(hit => hit.Text));
            SetStatus(
                hits.Count == 1
                    ? Loc.Get("Status_BarcodeOne", hits[0].Format)
                    : Loc.Get("Status_BarcodeMany", hits.Count),
                InfoBarSeverity.Success);

            AddHistoryItem(Loc.Get("Source_Codes", _currentImageName), ResultTextBox.Text, null);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_ScanFailed", ex.Message), InfoBarSeverity.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void TranslateMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (_isBusy || _translation is null || sender is not MenuFlyoutItem { Tag: TranslationLanguage target })
        {
            return;
        }

        var text = ResultTextBox.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            SetStatus(Loc.Get("Status_NothingToTranslate"), InfoBarSeverity.Warning);
            return;
        }

        SetBusy(true);
        try
        {
            SetStatus(Loc.Get("Status_Translating", target.DisplayName), InfoBarSeverity.Informational);
            var translated = await _translation.TranslateAsync(text, target);
            ResultTextBox.Text = translated;
            MatchVoiceToText(translated);
            SetStatus(Loc.Get("Status_Translated", target.DisplayName), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_TranslateFailed", ex.Message), InfoBarSeverity.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    // ---------------------------------------------------------------- text pane

    private void CopyTextClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ResultTextBox.Text))
        {
            SetStatus(Loc.Get("Status_NothingToCopy"), InfoBarSeverity.Warning);
            return;
        }

        if (CopyTextToClipboard(ResultTextBox.Text, out var error))
        {
            SetStatus(Loc.Get("Status_TextCopied"), InfoBarSeverity.Success);
        }
        else
        {
            SetStatus(Loc.Get("Status_CopyFailed", error), InfoBarSeverity.Error);
        }
    }

    private static bool CopyTextToClipboard(string text) => CopyTextToClipboard(text, out _);

    /// <summary>
    /// Puts text on the clipboard, reporting failure rather than throwing.
    /// </summary>
    /// <remarks>
    /// The clipboard is a shared resource and can be held open by another process, so this fails in
    /// normal use often enough that the tray path cannot afford to let it throw.
    /// </remarks>
    private static bool CopyTextToClipboard(string text, out string error)
    {
        try
        {
            var data = new DataPackage();
            data.SetText(text);
            Clipboard.SetContent(data);

            // Flush so the text outlives this process, which the tray path depends on: the user's
            // next action is a paste into some other app, possibly after quitting this one.
            Clipboard.Flush();
            error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private void RemoveLineBreaksClick(object sender, RoutedEventArgs e)
    {
        ResultTextBox.Text = TextTools.RemoveLineBreaks(ResultTextBox.Text);
    }

    private void RemoveSpacesClick(object sender, RoutedEventArgs e)
    {
        ResultTextBox.Text = TextTools.RemoveSpaces(ResultTextBox.Text);
    }

    /// <summary>
    /// Takes effect on the next recognition. Rerunning here would be surprising — the user may
    /// have edited the text in the meantime, and re-recognizing would silently discard those edits.
    /// </summary>
    private void RepairVersionsToggled(object sender, RoutedEventArgs e)
    {
        AppSettings.Current.RepairVersionNumbers = RepairVersionsToggle.IsOn;
    }

    /// <summary>
    /// Whether the close button hides the window or ends the app. The tray icon stays either way —
    /// it is what makes the capture shortcut reachable while the window is gone.
    /// </summary>
    private void CloseToTrayToggled(object sender, RoutedEventArgs e)
    {
        AppSettings.Current.CloseToTray = CloseToTrayToggle.IsOn;
    }

    /// <summary>
    /// Reads the sign-in task's current state from Windows and shows it on the switch.
    /// </summary>
    /// <remarks>
    /// Deliberately not mirrored into <see cref="AppSettings"/>. Task Manager and Settings can both
    /// turn this off behind the app's back, and a cached copy would keep claiming it was on. The
    /// platform is the only source of truth, so it is read on every launch.
    /// </remarks>
    private async void InitializeStartupToggle()
    {
        try
        {
            var task = await StartupTask.GetAsync(StartupTaskId);
            _startupTask = task;

            _suppressStartupToggle = true;
            StartupToggle.IsOn = task.State is StartupTaskState.Enabled or StartupTaskState.EnabledByPolicy;
            _suppressStartupToggle = false;

            // Policy decides it in both directions, and the app cannot overrule either.
            StartupToggle.IsEnabled =
                task.State is not (StartupTaskState.DisabledByPolicy or StartupTaskState.EnabledByPolicy);
        }
        catch (Exception ex)
        {
            // No package identity — running the .exe straight out of bin. There is no task to
            // toggle, so hide the row rather than leave a switch that does nothing when pressed.
            Trace.Write("StartupTask.GetAsync", ex);
            StartupToggle.Visibility = Visibility.Collapsed;
            StartupDescription.Visibility = Visibility.Collapsed;
            StartupSeparator.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Asks Windows to start the app at sign-in, and explains it when Windows says no.
    /// </summary>
    /// <remarks>
    /// Once the user has disabled the entry in Task Manager, <c>RequestEnableAsync</c> returns
    /// <c>DisabledByUser</c> and the app has no way to override it — by design. Without the message
    /// the switch simply flips back on its own and looks broken.
    /// </remarks>
    private async void StartupToggled(object sender, RoutedEventArgs e)
    {
        if (_suppressStartupToggle || _startupTask is not { } task)
        {
            return;
        }

        try
        {
            if (!StartupToggle.IsOn)
            {
                task.Disable();
                return;
            }

            var state = await task.RequestEnableAsync();
            if (state is StartupTaskState.Enabled or StartupTaskState.EnabledByPolicy)
            {
                return;
            }

            _suppressStartupToggle = true;
            StartupToggle.IsOn = false;
            _suppressStartupToggle = false;

            if (state == StartupTaskState.DisabledByPolicy)
            {
                StartupToggle.IsEnabled = false;
                SetStatus(Loc.Get("Status_StartupBlockedByPolicy"), InfoBarSeverity.Warning);
            }
            else
            {
                SetStatus(Loc.Get("Status_StartupBlockedByUser"), InfoBarSeverity.Warning);
            }
        }
        catch (Exception ex)
        {
            Trace.Write("StartupToggled", ex);

            _suppressStartupToggle = true;
            StartupToggle.IsOn = false;
            _suppressStartupToggle = false;

            SetStatus(Loc.Get("Status_StartupBlockedByUser"), InfoBarSeverity.Warning);
        }
    }

    /// <summary>
    /// A hand-picked voice is final. Recognition picks one only until the user overrules it.
    /// </summary>
    private void VoiceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_suppressVoiceChange)
        {
            _voicePinned = true;
        }
    }

    /// <summary>
    /// Moves the voice picker onto the script that was just recognized, so Japanese text is not
    /// read out by an English voice one syllable at a time.
    /// </summary>
    private void MatchVoiceToText(string text)
    {
        if (_voicePinned || VoiceComboBox.ItemsSource is not IReadOnlyList<VoiceInformation> voices)
        {
            return;
        }

        var tag = SpokenLanguageFor(text);
        if (tag is null)
        {
            return;
        }

        var index = -1;
        for (var i = 0; i < voices.Count; i++)
        {
            var language = voices[i].Language;
            if (language.Equals(tag, StringComparison.OrdinalIgnoreCase) ||
                language.StartsWith(tag + "-", StringComparison.OrdinalIgnoreCase))
            {
                index = i;
                break;
            }
        }

        if (index < 0 || index == VoiceComboBox.SelectedIndex)
        {
            return;
        }

        _suppressVoiceChange = true;
        try
        {
            VoiceComboBox.SelectedIndex = index;
        }
        finally
        {
            _suppressVoiceChange = false;
        }
    }

    /// <summary>
    /// Which language the text should be read in. Non-Latin scripts answer this on their own; for
    /// Latin text the script says nothing, so the recognizer that produced it is asked instead, and
    /// the interface language after that.
    /// </summary>
    private string? SpokenLanguageFor(string text)
    {
        var detected = TextTools.DetectScriptLanguage(text);
        if (detected is not null)
        {
            return detected;
        }

        var fallback = (LanguageComboBox.SelectedItem as OcrOption)?.LanguageTag;
        if (string.IsNullOrEmpty(fallback) || fallback == WindowsMediaOcrEngine.AutoLanguageTag)
        {
            // The AI recognizer and the multi-language mode report no language of their own.
            fallback = Loc.CurrentTag;
        }

        var primary = fallback.Split('-')[0];

        // The text is written in the Latin alphabet, so a recognizer or interface set to a
        // non-Latin language tells us nothing useful about how to pronounce it.
        return primary is "zh" or "ja" or "ko" or "ru" ? "en" : primary;
    }

    private async void SpeakClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ResultTextBox.Text))
        {
            SetStatus(Loc.Get("Status_NothingToSpeak"), InfoBarSeverity.Warning);
            return;
        }

        try
        {
            SpeakButton.IsEnabled = false;
            await _speech.SpeakAsync(ResultTextBox.Text, VoiceComboBox.SelectedItem as VoiceInformation);
        }
        catch (Exception ex)
        {
            SpeakButton.IsEnabled = true;
            SetStatus(Loc.Get("Status_SpeakFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    private void StopSpeakClick(object sender, RoutedEventArgs e)
    {
        _speech.Stop();
        SpeakButton.IsEnabled = true;
    }

    private void HistorySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HistoryListView.SelectedItem is not HistoryItem item)
        {
            return;
        }

        ResultTextBox.Text = item.Text;
        SetStatus(Loc.Get("Status_HistoryLoaded", item.Source), InfoBarSeverity.Informational);

        // Leaving the row selected made picking the same entry twice do nothing at all: the second
        // click is not a selection change. Clearing it re-enters this handler once, harmlessly.
        HistoryListView.SelectedItem = null;
    }

    private void AddHistoryItem(string source, string text, float? confidence)
    {
        _history.Insert(0, new HistoryItem
        {
            Timestamp = DateTimeOffset.Now,
            Source = source,
            Text = text,
            Confidence = confidence
        });

        while (_history.Count > MaxHistoryItems)
        {
            _history.RemoveAt(_history.Count - 1);
        }
    }

    // ---------------------------------------------------------------- screen capture

    private async Task CaptureFullScreenAndRunAsync()
    {
        try
        {
            var (path, _) = await Task.Run(CaptureVirtualScreen);
            _sessionTempFiles.Add(path);

            var label = Loc.Get("Source_FullScreen");
            LoadPreview(path, label);
            await RunOcrAsync(label);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_CaptureFailed", ex.Message), InfoBarSeverity.Error);

            if (_isHidden)
            {
                Toasts.ShowFailed(ex.Message);
            }
        }
    }

    private async Task CaptureRegionAndRunAsync()
    {
        string? screenshotPath = null;
        try
        {
            // Freeze the screen first: the overlay then draws on top of a still image instead of
            // racing with the live desktop, so it can never end up inside the captured pixels.
            var (path, bounds) = await Task.Run(CaptureVirtualScreen);
            screenshotPath = path;

            var captureWindow = new RegionCaptureWindow(screenshotPath, bounds);
            var selection = await captureWindow.CaptureAsync();
            if (selection is null)
            {
                return;
            }

            var source = screenshotPath;
            var regionPath = await Task.Run(() => CropToTempFile(source, selection));
            _sessionTempFiles.Add(regionPath);

            var label = Loc.Get("Source_Region");
            LoadPreview(regionPath, label);
            await RunOcrAsync(label);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_CaptureFailed", ex.Message), InfoBarSeverity.Error);

            if (_isHidden)
            {
                Toasts.ShowFailed(ex.Message);
            }
        }
        finally
        {
            if (screenshotPath is not null && !TryDeleteFile(screenshotPath))
            {
                // Still referenced by the overlay's BitmapImage; the next startup sweep gets it.
                _sessionTempFiles.Add(screenshotPath);
            }
        }
    }

    /// <summary>
    /// Captures every monitor. Returns the file and the screen-absolute bounds it covers.
    /// Must NOT run on the UI thread: CopyFromScreen BitBlts from the screen DC, and DWM sends
    /// synchronous messages to top-level windows while that happens — calling it from the thread
    /// that pumps those messages deadlocks, leaving the app painted white and unresponsive.
    /// </summary>
    private static (string Path, RectInt32 Bounds) CaptureVirtualScreen()
    {
        var x = GetSystemMetrics(SmXvirtualscreen);
        var y = GetSystemMetrics(SmYvirtualscreen);
        var width = GetSystemMetrics(SmCxvirtualscreen);
        var height = GetSystemMetrics(SmCyvirtualscreen);

        if (width <= 0 || height <= 0)
        {
            x = 0;
            y = 0;
            width = GetSystemMetrics(SmCxscreen);
            height = GetSystemMetrics(SmCyscreen);
        }

        var tempPath = CreateTempImagePath("screen.png");
        using var bitmap = new Bitmap(width, height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(x, y, 0, 0, new Size(width, height));
        }

        bitmap.Save(tempPath, ImageFormat.Png);
        return (tempPath, new RectInt32(x, y, width, height));
    }

    private static string CropToTempFile(string sourcePath, RegionSelection selection)
    {
        var tempPath = CreateTempImagePath("region.png");

        using var source = new Bitmap(sourcePath);
        var x = Math.Clamp(selection.X, 0, Math.Max(0, source.Width - 1));
        var y = Math.Clamp(selection.Y, 0, Math.Max(0, source.Height - 1));
        var width = Math.Clamp(selection.Width, 1, source.Width - x);
        var height = Math.Clamp(selection.Height, 1, source.Height - y);

        using (var region = new Bitmap(width, height))
        {
            using (var graphics = Graphics.FromImage(region))
            {
                graphics.DrawImage(
                    source,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(x, y, width, height),
                    GraphicsUnit.Pixel);
            }

            region.Save(tempPath, ImageFormat.Png);
        }

        return tempPath;
    }

    // ---------------------------------------------------------------- tray and notifications

    /// <summary>
    /// Puts the app in the notification area, so it can keep the capture shortcuts registered after
    /// its window is gone.
    /// </summary>
    /// <remarks>
    /// The handlers go through the dispatcher queue because the tray menu is a Win32 menu: it runs
    /// its own message loop inside the window procedure, and doing the work directly would start
    /// recognition — including opening the capture overlay — while that loop is still on the stack.
    /// </remarks>
    private void InitializeTray()
    {
        _tray = new TrayIcon(_hwnd) { HotkeyText = _captureHotkeyText };

        _tray.OpenRequested += (_, _) => _dispatcherQueue.TryEnqueue(ShowFromTray);
        _tray.CaptureRegionRequested += (_, _) =>
            _dispatcherQueue.TryEnqueue(async () => await CaptureRegionAndRunAsync());
        _tray.CaptureFullScreenRequested += (_, _) =>
            _dispatcherQueue.TryEnqueue(async () => await CaptureFullScreenAndRunAsync());
        _tray.ExitRequested += (_, _) => _dispatcherQueue.TryEnqueue(ExitApp);
    }

    /// <summary>
    /// Gives the window the executable's own icon, for the title bar, the taskbar and Alt+Tab.
    /// </summary>
    /// <remarks>
    /// A WinUI window starts with the generic application icon; only the packaged tile assets are
    /// picked up automatically. The icon is taken from the module's resource rather than a file so
    /// there is nothing extra to deploy, and it stays right when the app runs unpackaged.
    /// Failure here is cosmetic, so it is swallowed.
    /// </remarks>
    private static void ApplyWindowIcon(AppWindow window)
    {
        var icon = IntPtr.Zero;
        try
        {
            var size = GetSystemMetrics(SmCxicon);
            icon = LoadImage(GetModuleHandle(null), new IntPtr(IdiApplication), ImageIcon, size, size, 0);
            if (icon != IntPtr.Zero)
            {
                window.SetIcon(Win32Interop.GetIconIdFromIcon(icon));
            }
        }
        catch (Exception)
        {
            // No icon resource, or a shell that refused the handle. The default icon stands.
        }
        finally
        {
            // SetIcon copies the handle, and LoadImage without LR_SHARED hands ownership over.
            if (icon != IntPtr.Zero)
            {
                DestroyIcon(icon);
            }
        }
    }

    /// <summary>Brings the window back from the notification area and puts it in front.</summary>
    public void ShowFromTray()
    {
        if (_isClosed || _appWindow is null)
        {
            return;
        }

        _appWindow.Show();

        // Show alone leaves a window that was minimized before it was hidden still minimized, so
        // it comes back as nothing but a taskbar button.
        if (_appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.Restore();
        }

        // Started from the sign-in task, this window has never been activated at all, and Show on
        // its own yields an empty frame. Activating one that is already up costs nothing.
        Activate();

        SetForegroundWindow(_hwnd);
        _isHidden = false;
    }

    private void HideToTray()
    {
        _appWindow?.Hide();
        _isHidden = true;
    }

    private void ExitApp()
    {
        _exitRequested = true;
        Close();
    }

    /// <summary>
    /// Turns the close button into "hide", unless the user has turned that off or asked to exit.
    /// </summary>
    /// <remarks>
    /// Hooked on <see cref="AppWindow"/> rather than by intercepting WM_CLOSE, because this one
    /// event covers the caption button, Alt+F4 and the taskbar's own close command alike —
    /// <see cref="Window.Closed"/> cannot be cancelled at all.
    /// </remarks>
    private void AppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_exitRequested || !AppSettings.Current.CloseToTray)
        {
            return;
        }

        args.Cancel = true;
        HideToTray();

        // A window that vanishes without ending its process is worth explaining, but only until
        // the user has seen the explanation. After that the tray icon speaks for itself.
        //
        // Queued rather than raised inline: this handler runs inside the window's close message,
        // which is input-synchronous, and COM refuses outgoing calls for as long as one is being
        // dispatched. Calling Show here fails with RPC_E_CANTCALLOUT_ININPUTSYNCCALL (0x8001010D)
        // and the user simply never sees the notification.
        DispatcherQueue.TryEnqueue(() =>
        {
            var shown = AppSettings.Current.TrayHintCount;
            if (shown < MaxTrayHints)
            {
                AppSettings.Current.TrayHintCount = shown + 1;
                Toasts.ShowMinimizedToTray(_captureHotkeyText);
            }
        });
    }

    /// <summary>
    /// The one notification raised while the window is on screen, and only for a user who has not
    /// yet used the shortcut it is describing.
    /// </summary>
    /// <remarks>
    /// Delayed rather than immediate: a toast that arrives with the window is competing with the
    /// window for attention, and loses.
    /// </remarks>
    private async Task AnnounceStartupAsync()
    {
        if (!_hotkeyAvailable)
        {
            await Task.Delay(StartupHintDelayMs);
            if (!_isClosed)
            {
                Toasts.ShowHotkeyUnavailable();
            }

            return;
        }

        if (AppSettings.Current.HotkeyUsed || AppSettings.Current.StartupHintMuted)
        {
            Trace.Write($"startup hint suppressed: hotkeyUsed={AppSettings.Current.HotkeyUsed} muted={AppSettings.Current.StartupHintMuted}");
            return;
        }

        await Task.Delay(StartupHintDelayMs);

        if (!_isClosed)
        {
            Toasts.ShowStartupHint(_captureHotkeyText);
        }
    }

    private void ToastInvoked(object? sender, string action)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_isClosed)
            {
                return;
            }

            switch (action)
            {
                case Toasts.ActionExit:
                    ExitApp();
                    break;
                case Toasts.ActionMuteStartup:
                    AppSettings.Current.StartupHintMuted = true;
                    break;
                default:
                    ShowFromTray();
                    break;
            }
        });
    }

    // ---------------------------------------------------------------- hotkeys

    private void RegisterHotkeys()
    {
        RegisterHotKey(_hwnd, FullHotkeyId, ModControl | ModShift, VkR);

        // Alt+Z is the shortcut users coming from other capture tools reach for first; if another
        // app already owns it, fall back rather than leaving region capture without a hotkey at all.
        _hotkeyAvailable = true;
        _captureHotkeyText =
            RegisterHotKey(_hwnd, RegionHotkeyId, ModAlt, VkZ) ? "Alt+Z"
            : RegisterHotKey(_hwnd, RegionHotkeyId, ModControl | ModShift, VkG) ? "Ctrl+Shift+G"
            : Unavailable();

        string Unavailable()
        {
            _hotkeyAvailable = false;
            return Loc.Get("Hotkey_Unavailable");
        }
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WmHotkey)
        {
            var hotkeyId = wParam.ToInt32();
            if (hotkeyId == FullHotkeyId)
            {
                AppSettings.Current.HotkeyUsed = true;
                _dispatcherQueue.TryEnqueue(async () => await CaptureFullScreenAndRunAsync());
                return IntPtr.Zero;
            }

            if (hotkeyId == RegionHotkeyId)
            {
                AppSettings.Current.HotkeyUsed = true;
                _dispatcherQueue.TryEnqueue(async () => await CaptureRegionAndRunAsync());
                return IntPtr.Zero;
            }
        }

        if (_tray?.HandleMessage(msg, wParam, lParam) == true)
        {
            return IntPtr.Zero;
        }

        return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
    }

    // ---------------------------------------------------------------- helpers

    private bool HasImage()
    {
        if (!string.IsNullOrWhiteSpace(_currentImagePath) && File.Exists(_currentImagePath))
        {
            return true;
        }

        SetStatus(Loc.Get("Status_NeedImage"), InfoBarSeverity.Warning);
        return false;
    }

    private void SetBusy(bool busy)
    {
        _isBusy = busy;
        BusyRing.IsActive = busy;
        BusyRing.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        RecognizeIcon.Visibility = busy ? Visibility.Collapsed : Visibility.Visible;
        RecognizeButton.IsEnabled = !busy;
    }

    private void SetStatus(string message, InfoBarSeverity severity)
    {
        // Unconditionally: the action button belongs to the message that put it there. Keeping it
        // for the next warning too left "Open language settings" attached to, say, a failed scan.
        StatusBar.ActionButton = null;

        StatusBar.Severity = severity;
        StatusBar.Message = message;
        StatusBar.IsOpen = true;
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private static readonly string[] SupportedExtensions =
        { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff", ".webp" };

    private static bool IsSupportedImage(string fileName) =>
        SupportedExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant());

    private static string CreateTempImagePath(string displayName)
    {
        Directory.CreateDirectory(TempRoot);

        var extension = Path.GetExtension(displayName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".png";
        }

        return Path.Combine(TempRoot, $"{Guid.NewGuid():N}{extension}");
    }

    /// <summary>Removes leftovers from previous runs so the temp folder does not grow without bound.</summary>
    private static void CleanupOldTempFiles()
    {
        try
        {
            if (!Directory.Exists(TempRoot))
            {
                return;
            }

            var cutoff = DateTime.UtcNow.AddHours(-2);
            foreach (var file in Directory.EnumerateFiles(TempRoot))
            {
                if (File.GetLastWriteTimeUtc(file) < cutoff)
                {
                    TryDeleteFile(file);
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
        }
    }

    private static bool TryDeleteFile(string path)
    {
        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        _isClosed = true;

        // Loc.Changed is static, so an unsubscribed window would be kept alive by it for the rest
        // of the process — and would then try to write to controls that no longer have a tree.
        Loc.Changed -= LanguageChanged;
        Toasts.Invoked -= ToastInvoked;
        Toasts.Unregister();

        UnregisterHotKey(_hwnd, FullHotkeyId);
        UnregisterHotKey(_hwnd, RegionHotkeyId);

        // Before the window procedure is put back: the shell is told to drop the icon through this
        // very window, and a half-restored subclass is not the state to do that in.
        _tray?.Dispose();
        _tray = null;

        if (_oldWndProc != IntPtr.Zero)
        {
            SetWindowLongPtr(_hwnd, GwlWndproc, _oldWndProc);
            _oldWndProc = IntPtr.Zero;
        }

        _speech.Dispose();
        _translation?.Dispose();
        _engines.Dispose();

        PreviewImage.Source = null;
        foreach (var path in _sessionTempFiles)
        {
            TryDeleteFile(path);
        }

        _sessionTempFiles.Clear();
    }

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadImage(IntPtr instance, IntPtr name, uint type, int cx, int cy, uint load);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr icon);

    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string? moduleName);

    /// <summary>
    /// Subclasses the window. Two imports because 32-bit user32.dll exports no
    /// <c>SetWindowLongPtrW</c> at all — importing only that one threw
    /// <see cref="EntryPointNotFoundException"/> straight out of the constructor on the x86 build
    /// this project also produces.
    /// </summary>
    private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr newValue) =>
        IntPtr.Size == 8
            ? SetWindowLongPtrW(hWnd, nIndex, newValue)
            : new IntPtr(SetWindowLongW(hWnd, nIndex, newValue.ToInt32()));

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtrW(IntPtr hWnd, int nIndex, IntPtr newValue);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
    private static extern int SetWindowLongW(IntPtr hWnd, int nIndex, int newValue);

    [DllImport("user32.dll", EntryPoint = "CallWindowProcW")]
    private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
