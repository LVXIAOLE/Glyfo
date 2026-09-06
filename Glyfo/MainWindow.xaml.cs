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
using System.Text;
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

    /// <summary>Stands in for a shortcut that is not registered, on the buttons that show one.</summary>
    private const string NoHotkeyText = "—";

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
    /// How long the release notes wait for the window to finish coming up. A ContentDialog needs a
    /// <c>XamlRoot</c>, which does not exist until the tree is loaded, and the notes are the first
    /// thing this version does — arriving before the window has drawn would be a dialog over
    /// nothing.
    /// </summary>
    private const int WhatsNewDelayMs = 700;

    /// <summary>
    /// How long the desktop is given to repaint after the window is taken off it for a capture.
    /// Long enough to outlast the DWM fade at the default animation speed, short enough that the
    /// button still feels like it acted on the click.
    /// </summary>
    private const int CaptureHideDelayMs = 220;

    /// <summary>
    /// How many images have to be read successfully before the app asks for a rating. Ten is past
    /// the point where someone is still deciding whether they like it.
    /// </summary>
    private const int RatingPromptAfter = 10;

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
    /// <summary>
    /// Every entry there is, newest first. The list that is actually stored and searched.
    /// </summary>
    private readonly List<HistoryItem> _history = new();

    /// <summary>
    /// What the list is showing: all of <see cref="_history"/>, or the part matching the search box.
    /// </summary>
    /// <remarks>
    /// A second collection rather than a filter over the first, because <c>ListView</c> takes an
    /// <c>ObservableCollection</c> and the alternative — removing non-matching rows from the one
    /// list — would make the search destructive.
    /// </remarks>
    private readonly ObservableCollection<HistoryItem> _historyView = new();

    private readonly List<string> _sessionTempFiles = new();

    private TranslationService? _translation;
    private TrayIcon? _tray;
    private ClipboardWatcher? _clipboard;
    private AppWindow? _appWindow;
    private StartupTask? _startupTask;
    private IntPtr _oldWndProc;
    private string _currentImagePath = string.Empty;
    private string _currentImageName = string.Empty;

    /// <summary>
    /// The picture as it arrived, before any turning, and how far it has been turned since.
    /// </summary>
    /// <remarks>
    /// Every rotation re-renders from the original rather than turning the last result. Turning a
    /// turned picture resamples it again each time, so a few presses of "rotate left" would visibly
    /// soften a screenshot; it also makes "reset" a single assignment rather than an inverse the
    /// code would have to get right.
    /// </remarks>
    private string _originalImagePath = string.Empty;
    private double _rotationDegrees;

    /// <summary>The open PDF, or null when the preview came from anywhere else.</summary>
    /// <remarks>
    /// Cleared by <see cref="LoadPreview"/> and set back afterwards by the PDF path, so that a
    /// screenshot taken while a PDF is open takes the page bar down with it. Every other source
    /// gets that for free rather than having to remember.
    /// </remarks>
    private PdfSource? _pdf;
    private uint _pdfPageIndex;

    /// <summary>
    /// The two shortcuts as actually registered — empty when the combination was already taken.
    /// </summary>
    /// <remarks>
    /// What is registered, not what is stored: the two differ exactly when another app owns the
    /// user's choice, and every label in the app names these so that nothing ever advertises a key
    /// that does nothing.
    /// </remarks>
    private Hotkey _regionHotkey;
    private Hotkey _fullHotkey;

    /// <summary>The settings button currently listening for a combination, if any.</summary>
    private Button? _recordingButton;
    private bool _recordingRegion;

    private bool _autoFit = true;
    private bool _isBusy;
    private bool _isClosed;

    /// <summary>Whether the window is parked in the notification area rather than merely covered.</summary>
    private bool _isHidden;

    /// <summary>Lets the close through instead of hiding it; only the tray's Exit item sets it.</summary>
    private bool _exitRequested;

    /// <summary>
    /// Set by the About link in settings, which has to close that dialog before its own can open.
    /// </summary>
    private bool _aboutRequested;

    // Guards against the handlers of controls this code is itself repopulating: rebuilding the
    // language pickers raises SelectionChanged, and acting on that would persist a value the user
    // never chose.
    private bool _suppressUiLanguageChange;
    private bool _suppressOcrLanguageChange;
    private bool _suppressVoiceChange;
    private bool _suppressStartupToggle;
    private bool _suppressKeepHistoryToggle;

    /// <summary>Set once the user picks a voice by hand, after which recognition stops choosing one.</summary>
    private bool _voicePinned;

    /// <summary>
    /// Whether this run has already put something in the user's way that they did not ask for.
    /// </summary>
    /// <remarks>
    /// One per launch, at most. Reading the release notes and then being asked for a rating in the
    /// same sitting is the combination that makes an app feel like it wants something.
    /// </remarks>
    private bool _interrupted;

    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Set before the Toggled handler can matter — assigning IsOn raises it, and writing the
        // stored value straight back is harmless.
        RepairVersionsToggle.IsOn = AppSettings.Current.RepairVersionNumbers;
        CloseToTrayToggle.IsOn = AppSettings.Current.CloseToTray;
        WatchClipboardToggle.IsOn = AppSettings.Current.WatchClipboard;

        // Not harmless for this one, which is why it is guarded. Its handler writes the history out
        // or deletes it, and the write is asynchronous: left ungagged, restoring the switch here
        // raced the Load below with an empty list and wiped the file it was about to read.
        _suppressKeepHistoryToggle = true;
        KeepHistoryToggle.IsOn = AppSettings.Current.KeepHistory;
        _suppressKeepHistoryToggle = false;

        HistoryListView.ItemsSource = _historyView;

        // Read straight through rather than in the background: it is one small file, it has to be
        // there before the first recognition can append to it, and a history that appears a moment
        // after the window would be worse than one that costs a few milliseconds of startup.
        if (AppSettings.Current.KeepHistory)
        {
            _history.AddRange(HistoryStore.Load());
        }

        RefreshHistoryView();

        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _hwnd = WindowNative.GetWindowHandle(this);
        _wndProcDelegate = WndProc;
        _oldWndProc = SetWindowLongPtr(_hwnd, GwlWndproc, Marshal.GetFunctionPointerForDelegate(_wndProcDelegate));
        RegisterHotkeys();
        InitializeTray();
        InitializeClipboardWatcher();

        var windowId = Win32Interop.GetWindowIdFromWindow(_hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        _appWindow = appWindow;
        appWindow.Closing += AppWindowClosing;
        ApplyWindowIcon(appWindow);

        RestoreWindowGeometry(appWindow, _hwnd);

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
        _ = ShowWhatsNewIfUpdatedAsync();
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
        // The split button carries no label of its own, so the tooltip is also what a screen reader
        // announces for it — hence SetTip rather than the plain tooltip.
        SetTip(RotateButton, Loc.Get("Rotate_Right"));
        RotateLeftItem.Text = Loc.Get("Rotate_Left");
        RotateHalfItem.Text = Loc.Get("Rotate_180");
        DeskewItem.Text = Loc.Get("Rotate_Deskew");
        RotateResetItem.Text = Loc.Get("Rotate_Reset");

        FitToWindowLabel.Text = Loc.Get("Btn_FitToWindow");
        SetTip(FitToWindowButton, Loc.Get("Tip_FitToWindow"));
        SetTip(ActualSizeButton, Loc.Get("Tip_ActualSize"));

        VoiceComboBox.PlaceholderText = Loc.Get("Voice_Placeholder");
        SetTip(VoiceComboBox, Loc.Get("Tip_Voice"));
        SetTip(SpeakButton, Loc.Get("Tip_Speak"));
        SetTip(StopSpeakButton, Loc.Get("Tip_StopSpeak"));
        SetTip(CopyTextButton, Loc.Get("Tip_CopyText"));
        SetTip(SaveTextButton, Loc.Get("Tip_SaveText"));
        RemoveLineBreaksButton.Content = Loc.Get("Btn_RemoveLineBreaks");
        SetTip(RemoveLineBreaksButton, Loc.Get("Tip_RemoveLineBreaks"));
        RemoveSpacesButton.Content = Loc.Get("Btn_RemoveSpaces");
        SetTip(RemoveSpacesButton, Loc.Get("Tip_RemoveSpaces"));
        ResultTextBox.PlaceholderText = Loc.Get("Result_Placeholder");

        OpenLabel.Text = Loc.Get("Btn_OpenFile");
        SetTip(OpenButton, Loc.Get("Tip_OpenFile"));
        CaptureLabel.Text = Loc.Get("Btn_Capture");

        // The same two entries the tray menu offers, under the same names: they do the same thing,
        // and a second wording for it would only invite the reader to look for a difference.
        CaptureRegionItem.Text = Loc.Get("Tray_CaptureRegion");
        CaptureFullScreenItem.Text = Loc.Get("Tray_CaptureFullScreen");
        PasteLabel.Text = Loc.Get("Btn_Paste");
        SetTip(PasteButton, Loc.Get("Tip_Paste"));
        HistoryLabel.Text = Loc.Get("Btn_History");
        SetTip(HistoryButton, Loc.Get("Tip_History"));
        HistorySearchBox.PlaceholderText = Loc.Get("History_SearchPlaceholder");
        SetTip(HistoryClearButton, Loc.Get("History_Clear"));
        UpdateHistoryEmptyText();
        SetTip(SettingsButton, Loc.Get("Tip_Settings"));

        SettingsDialog.Title = Loc.Get("Settings_Header");
        SettingsDialog.CloseButtonText = Loc.Get("Common_Close");
        SettingsGroupGeneral.Text = Loc.Get("Setting_Group_General");
        SettingsGroupHotkeys.Text = Loc.Get("Setting_Group_Hotkeys");
        SettingsGroupRecognition.Text = Loc.Get("Setting_Group_Recognition");
        SettingsGroupPrivacy.Text = Loc.Get("Setting_Group_Privacy");
        HotkeyRegionLabel.Text = Loc.Get("Setting_Hotkey_Region");
        HotkeyFullLabel.Text = Loc.Get("Setting_Hotkey_Full");
        HotkeyHint.Text = Loc.Get("Hotkey_RecordHint");
        ResetHotkeysButton.Content = Loc.Get("Btn_ResetHotkeys");
        // The tooltip only, not SetTip: these two buttons carry the combination as their content,
        // and an automation name would hide it from a screen reader behind the instruction.
        ToolTipService.SetToolTip(HotkeyRegionButton, Loc.Get("Btn_ChangeHotkey"));
        ToolTipService.SetToolTip(HotkeyFullButton, Loc.Get("Btn_ChangeHotkey"));

        // Everything that names a combination goes through one place: the capture tooltip, both
        // menu accelerators, the tray and the two settings rows all depend on the language and on
        // what actually registered at once, and either can change without the other.
        RefreshHotkeyLabels();
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

        WatchClipboardToggle.Header = Loc.Get("Setting_WatchClipboard");
        WatchClipboardToggle.OnContent = Loc.Get("Common_On");
        WatchClipboardToggle.OffContent = Loc.Get("Common_Off");
        WatchClipboardDescription.Text = Loc.Get("Setting_WatchClipboard_Desc");

        KeepHistoryToggle.Header = Loc.Get("Setting_KeepHistory");
        KeepHistoryToggle.OnContent = Loc.Get("Common_On");
        KeepHistoryToggle.OffContent = Loc.Get("Common_Off");
        KeepHistoryDescription.Text = Loc.Get("Setting_KeepHistory_Desc");

        AboutButton.Content = Loc.Get("Btn_About");

        RatingTip.Title = Loc.Get("Rate_Title");
        RatingTip.Subtitle = Loc.Get("Rate_Body");
        RatingTip.ActionButtonContent = Loc.Get("Rate_Action");
        RatingTip.CloseButtonContent = Loc.Get("Rate_Later");

        LanguageComboBox.PlaceholderText = Loc.Get("Lang_Placeholder");
        SetTip(LanguageComboBox, Loc.Get("Tip_OcrLanguage"));
        TranslateLabel.Text = Loc.Get("Btn_Translate");
        SetTip(TranslateButton, Loc.Get("Tip_Translate"));
        BarcodeLabel.Text = Loc.Get("Btn_Barcode");
        SetTip(BarcodeButton, Loc.Get("Tip_Barcode"));
        RecognizeLabel.Text = Loc.Get("Btn_Recognize");
        SetTip(RecognizeButton, Loc.Get("Tip_Recognize"));

        SetTip(PdfPrevButton, Loc.Get("Tip_PdfPrev"));
        SetTip(PdfNextButton, Loc.Get("Tip_PdfNext"));
        SetTip(PdfAllButton, Loc.Get("Tip_PdfAll"));

        // The chips are built in code and carry their verb in the tooltip, so there is nothing to
        // reassign — they have to be made again. Clearing the cache is what forces that.
        _actions = Array.Empty<TextAction>();
        RefreshSmartActions();

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
    /// The flyouts and the settings dialog are set separately because their content lives in a popup
    /// rather than under <c>RootGrid</c>, so it never inherits the change. The title bar is
    /// deliberately left running left to right: the caption buttons stay on the window's right
    /// whatever the content does, and
    /// mirroring that row would slide the app name underneath them.
    /// </remarks>
    private void ApplyFlowDirection()
    {
        var flow = Loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        RootGrid.FlowDirection = flow;
        AppTitleBar.FlowDirection = FlowDirection.LeftToRight;
        HistoryFlyoutRoot.FlowDirection = flow;
        SettingsDialog.FlowDirection = flow;
        RatingTip.FlowDirection = flow;

        // The chips are laid out left to right by a StackPanel, so this is what puts the first
        // one on the right in Arabic. The page pill is mirrored for the same reason, which also
        // swaps which side the back arrow sits on — correct, since it points backwards in reading
        // order rather than in a fixed direction.
        ActionsPanel.FlowDirection = flow;
        PdfNavBar.FlowDirection = flow;

        foreach (var item in TranslateMenu.Items)
        {
            item.FlowDirection = flow;
        }

        foreach (var item in CaptureMenu.Items)
        {
            item.FlowDirection = flow;
        }

        foreach (var item in RotateMenu.Items)
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

        // Every other control in the flyout carries its own label; without this one the picker was
        // a bare box reading "English" under a heading that says only "Options", which tells nobody
        // what it selects. Set here rather than in the markup because it has to change with the
        // language it is choosing.
        UiLanguageComboBox.Header = Loc.Get("Setting_UiLanguage");
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

            picker.FileTypeFilter.Add(".pdf");

            InitializeWithWindow.Initialize(picker, _hwnd);

            // Multiple, not single, so the one rule holds everywhere: one file opens, several read
            // in a batch. Adding a second button for it would have made picking three files a
            // different gesture from dropping three files, for no gain.
            var files = await picker.PickMultipleFilesAsync();
            if (files is null || files.Count == 0)
            {
                return;
            }

            if (files.Count > 1)
            {
                await RunBatchOverFilesAsync(files);
                return;
            }

            await OpenFileAsync(files[0]);
            SetStatus(Loc.Get("Status_Loaded", files[0].Name), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_OpenFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    /// <summary>The button body, which is region capture — the arrow beside it is the only way to
    /// reach anything else. A separate handler because SplitButton.Click carries its own event
    /// args rather than the RoutedEventArgs a menu item raises.</summary>
    private async void CaptureSplitClick(SplitButton sender, SplitButtonClickEventArgs args)
    {
        await CaptureRegionAndRunAsync();
    }

    private async void CaptureRegionClick(object sender, RoutedEventArgs e)
    {
        await CaptureRegionAndRunAsync();
    }

    private async void CaptureFullScreenClick(object sender, RoutedEventArgs e)
    {
        await CaptureFullScreenAndRunAsync();
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
                    if (item is StorageFile file && IsSupportedFile(file.Name))
                    {
                        await OpenFileAsync(file);
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
                var files = SupportedFiles(items);

                if (files.Count > 1)
                {
                    await RunBatchOverFilesAsync(files);
                    return;
                }

                if (files.Count == 1)
                {
                    await OpenFileAsync(files[0]);
                    SetStatus(Loc.Get("Status_Loaded", files[0].Name), InfoBarSeverity.Success);
                    return;
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
    /// A multi-selection reads as a batch, the same as dropping the same files on the window: the
    /// window shows one image beside its text, so several of them have nowhere to go except through
    /// the batch dialog.
    /// </remarks>
    public async Task OpenActivatedFilesAsync(IReadOnlyList<IStorageItem> items)
    {
        var files = SupportedFiles(items);

        if (files.Count > 1)
        {
            ShowFromTray();
            await RunBatchOverFilesAsync(files);
            return;
        }

        if (files.Count == 1)
        {
            await OpenExternalAsync(() => OpenFileAsync(files[0]), files[0].Name, files[0].Name);
            return;
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
                await FirstSupportedFileAsync(data) is { } file)
            {
                displayName = file.Name;
                ShowFromTray();
                await OpenFileAsync(file);
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

    private static async Task<StorageFile?> FirstSupportedFileAsync(DataPackageView data)
    {
        foreach (var item in await data.GetStorageItemsAsync())
        {
            if (item is StorageFile file && IsSupportedFile(file.Name))
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
        ReplacePreview(imagePath);

        _currentImageName = displayName;
        _originalImagePath = imagePath;
        _rotationDegrees = 0;
        ResultTextBox.Text = string.Empty;

        // Anything reaching here is a new source, so the PDF stops being open. The PDF path
        // sets it straight back after calling this — see ShowPdfPageAsync.
        _pdf = null;
        RefreshPdfNav();
    }

    /// <summary>
    /// Puts a different picture under the preview, and changes nothing else.
    /// </summary>
    /// <remarks>
    /// What <see cref="LoadPreview"/> does on top of this — clearing the result and closing the PDF
    /// — is right for a new source and wrong for a rotation, which is the same document seen
    /// straight. Turning the page you are reading should not throw away what was read from it, and
    /// it should not take the page bar down with it.
    /// </remarks>
    private void ReplacePreview(string imagePath)
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
    }

    // ---------------------------------------------------------------- rotation

    private void RotateRightClick(SplitButton sender, SplitButtonClickEventArgs args) => TurnBy(90);

    private void RotateLeftClick(object sender, RoutedEventArgs e) => TurnBy(-90);

    private void RotateHalfClick(object sender, RoutedEventArgs e) => TurnBy(180);

    private void RotateResetClick(object sender, RoutedEventArgs e) => TurnTo(0);

    private void TurnBy(double degrees) => TurnTo(_rotationDegrees + degrees);

    /// <summary>
    /// Shows the original picture turned by <paramref name="degrees"/> from where it started.
    /// </summary>
    /// <remarks>
    /// Straight back to the untouched file at zero: rendering it would be a re-encode of something
    /// the app already has, and the whole point of keeping the original is that "reset" costs
    /// nothing.
    /// </remarks>
    private void TurnTo(double degrees)
    {
        if (!HasImage())
        {
            return;
        }

        degrees = ((degrees % 360) + 360) % 360;
        if (Math.Abs(degrees) < 0.05)
        {
            _rotationDegrees = 0;
            ReplacePreview(_originalImagePath);
            return;
        }

        try
        {
            var tempPath = CreateTempImagePath("rotated.png");
            ImageOps.Rotate(_originalImagePath, degrees, tempPath);
            _sessionTempFiles.Add(tempPath);

            _rotationDegrees = degrees;
            ReplacePreview(tempPath);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_RotateFailed", ex.Message), InfoBarSeverity.Error);
        }
    }

    /// <summary>
    /// Measures how far the page leans and turns it back.
    /// </summary>
    /// <remarks>
    /// Measured on the picture as it is shown, not on the original, so that straightening after a
    /// quarter turn measures the thing the user is looking at. The measurement is the slow part —
    /// tens of milliseconds on a large scan — so it goes to a worker under the same busy state the
    /// recognizer uses; the turn itself then goes through the ordinary path.
    /// </remarks>
    private async void DeskewClick(object sender, RoutedEventArgs e)
    {
        if (_isBusy || !HasImage())
        {
            return;
        }

        var source = _currentImagePath;
        SetBusy(true);
        try
        {
            var skew = await Task.Run(() => ImageOps.EstimateSkew(source));
            if (Math.Abs(skew) < 0.05)
            {
                SetStatus(Loc.Get("Status_DeskewNone"), InfoBarSeverity.Informational);
                return;
            }

            // The estimate is how far it leans, so the correction is the other way. The message
            // reports the size of the correction and not its sign: "straightened by -5°" reads as
            // an error, and which way it went is on the screen already.
            TurnBy(-skew);
            SetStatus(Loc.Get("Status_Deskewed", Math.Abs(skew).ToString("0.0")), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_RotateFailed", ex.Message), InfoBarSeverity.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    // ---------------------------------------------------------------- pdf

    /// <summary>
    /// Opens a PDF and shows its first page.
    /// </summary>
    /// <remarks>
    /// Throws on failure, like the image path does, so that the entry point that started this is
    /// the one that reports it — every caller already has a catch. What is replaced is the message:
    /// a password-protected file fails with an HRESULT nobody can act on, and there is no password
    /// prompt to offer, so saying plainly what is wrong is the whole of the remedy.
    /// </remarks>
    private async Task OpenPdfAsync(StorageFile file)
    {
        // Before the load, not after: a failure leaves nothing half-open behind.
        _pdf = null;
        RefreshPdfNav();

        PdfSource pdf;
        try
        {
            pdf = await PdfSource.OpenAsync(file);
            if (pdf.PageCount == 0)
            {
                throw new InvalidOperationException("no pages");
            }
        }
        catch (Exception ex)
        {
            Trace.Write("OpenPdfAsync", ex);
            throw new InvalidOperationException(Loc.Get("Status_PdfFailed"), ex);
        }

        _pdfPageIndex = 0;
        await ShowPdfPageAsync(pdf, 0);
    }

    /// <summary>
    /// Draws one page into the preview.
    /// </summary>
    /// <remarks>
    /// The temp file name has to end in <c>.png</c> because <see cref="CreateTempImagePath"/>
    /// takes the extension from the display name, and a PNG called <c>.pdf</c> would confuse
    /// everything downstream that reopens the path. The name shown on screen is a separate,
    /// translated string.
    /// </remarks>
    private async Task ShowPdfPageAsync(PdfSource pdf, uint index)
    {
        using var stream = await pdf.RenderAsync(index);
        var baseName = Path.GetFileNameWithoutExtension(pdf.Name);

        await LoadImageFromStreamAsync(stream.AsStreamForRead(), $"{baseName} p{index + 1}.png");

        // After LoadImageFromStreamAsync, which goes through LoadPreview and clears both. The
        // display name it left behind ("Contract p3.png") is deliberately not replaced with a
        // translated sentence: it is what Save As suggests and what the history lists, and both
        // want a file name.
        _pdf = pdf;
        _pdfPageIndex = index;
        RefreshPdfNav();
    }

    /// <summary>Puts the floating page pill in step with the open document, or hides it.</summary>
    private void RefreshPdfNav()
    {
        if (PdfNavBar is null)
        {
            return;
        }

        if (_pdf is null)
        {
            PdfNavBar.Visibility = Visibility.Collapsed;
            return;
        }

        PdfNavBar.Visibility = Visibility.Visible;
        PdfPageText.Text = $"{_pdfPageIndex + 1} / {_pdf.PageCount}";
        PdfPrevButton.IsEnabled = _pdfPageIndex > 0;
        PdfNextButton.IsEnabled = _pdfPageIndex + 1 < _pdf.PageCount;
    }

    private async void PdfPrevClick(object sender, RoutedEventArgs e) => await TurnPdfPageAsync(-1);

    private async void PdfNextClick(object sender, RoutedEventArgs e) => await TurnPdfPageAsync(1);

    /// <summary>
    /// Moves one page and redraws. Does not recognize the new page: rendering is fast enough to
    /// page through a document looking for the right one, and reading every page on the way there
    /// would make that unusable.
    /// </summary>
    private async Task TurnPdfPageAsync(int delta)
    {
        if (_pdf is not { } pdf || _isBusy)
        {
            return;
        }

        var target = (long)_pdfPageIndex + delta;
        if (target < 0 || target >= pdf.PageCount)
        {
            return;
        }

        try
        {
            await ShowPdfPageAsync(pdf, (uint)target);
        }
        catch (Exception ex)
        {
            Trace.Write("TurnPdfPageAsync", ex);
            SetStatus(Loc.Get("Status_PdfFailed"), InfoBarSeverity.Error);
        }
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

            // Or the clipboard watch reads back the picture we just put there and recognizes it
            // again, which recognizes it again.
            ClipboardWatcher.NoteOwnWrite();
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

    /// <summary>
    /// Reads the image currently in the preview.
    /// </summary>
    /// <param name="sourceLabel">What to call this image in the history list.</param>
    /// <param name="alwaysCopy">
    /// Puts the text on the clipboard even if the window turns out to be in the foreground. Only the
    /// clipboard watch passes this, and it is the promise that feature makes: copy a picture, paste
    /// text. Leaving it to <see cref="UserIsElsewhere"/> would mean the promise quietly held only
    /// while some other window happened to have focus, which is not something a user can predict.
    /// </param>
    private async Task RunOcrAsync(string? sourceLabel = null, bool alwaysCopy = false)
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

                if (UserIsElsewhere)
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
                NoteSuccessfulRecognition();

                // Two separate questions, and they only look like one. Where the text goes is asked
                // first: the clipboard watch always answers yes, everything else answers it only
                // when the user is looking elsewhere and the status bar above is therefore no use
                // to them. Whether to interrupt with a toast is asked second, and the answer to
                // that is never yes while the window is in front of the user — it would be saying
                // out loud what it has already written on screen.
                var elsewhere = UserIsElsewhere;

                if (alwaysCopy || elsewhere)
                {
                    CopyTextToClipboard(result.Text);
                }

                if (elsewhere)
                {
                    Toasts.ShowRecognized(result.Text);
                }
            }
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_RecognizeFailed", ex.Message), InfoBarSeverity.Error);

            if (UserIsElsewhere)
            {
                Toasts.ShowFailed(ex.Message);
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    // ---------------------------------------------------------------- batch

    /// <summary>
    /// Longest merged text that still goes into the result box and the history.
    /// </summary>
    /// <remarks>
    /// One threshold covering two separate hazards, because they arrive together. A <c>TextBox</c>
    /// holding two hundred pages of text makes the window unusable long before it runs out of
    /// memory; and a single history entry that large would eat most of the store's budget and push
    /// out everything else. Past this the text lives in the file that was just saved, which is
    /// where someone reading two hundred pages wanted it anyway.
    /// </remarks>
    private const int MaxInlineTextLength = 100_000;

    /// <summary>The files in a drop or an activation this app can actually read, in the order given.</summary>
    private static List<StorageFile> SupportedFiles(IReadOnlyList<IStorageItem> items)
    {
        var files = new List<StorageFile>();

        foreach (var item in items)
        {
            if (item is StorageFile file && IsSupportedFile(file.Name))
            {
                files.Add(file);
            }
        }

        return files;
    }

    /// <summary>
    /// Reads a set of files, expanding any PDF among them into its pages.
    /// </summary>
    /// <remarks>
    /// Every source is opened as a stream rather than by path. Files arriving from a drop or the
    /// share sheet need not have a path this process can reopen, and the stream route works for all
    /// of them — including PDF pages, which never touch the disk at all.
    /// </remarks>
    private async Task RunBatchOverFilesAsync(IReadOnlyList<StorageFile> files)
    {
        var sources = new List<BatchSource>();

        foreach (var file in files)
        {
            if (!IsPdf(file.Name))
            {
                sources.Add(new BatchSource(file.Name, async () =>
                {
                    using var stream = await file.OpenReadAsync();
                    return await ImageLoader.LoadAsync(stream);
                }));

                continue;
            }

            PdfSource pdf;
            try
            {
                pdf = await PdfSource.OpenAsync(file);
            }
            catch (Exception ex)
            {
                // Encrypted or malformed. One unreadable document should not cancel the other
                // nineteen files, so it becomes a failed entry like any other.
                Trace.Write($"batch PDF '{file.Name}'", ex);
                sources.Add(new BatchSource(file.Name, () => throw new InvalidOperationException(Loc.Get("Status_PdfFailed"), ex)));
                continue;
            }

            var baseName = Path.GetFileNameWithoutExtension(pdf.Name);
            for (uint index = 0; index < pdf.PageCount; index++)
            {
                var page = index;
                sources.Add(new BatchSource($"{baseName} p{page + 1}", async () =>
                {
                    using var stream = await pdf.RenderAsync(page);
                    return await ImageLoader.LoadAsync(stream);
                }));
            }
        }

        await RunBatchAsync(sources, Loc.Get("Source_Batch", files.Count));
    }

    /// <summary>Reads every page of the open document, without disturbing the page on screen.</summary>
    private async void PdfAllClick(object sender, RoutedEventArgs e)
    {
        if (_pdf is null)
        {
            return;
        }

        var pdf = _pdf;
        var baseName = Path.GetFileNameWithoutExtension(pdf.Name);
        var sources = new List<BatchSource>((int)pdf.PageCount);

        for (uint index = 0; index < pdf.PageCount; index++)
        {
            var page = index;
            sources.Add(new BatchSource($"{baseName} p{page + 1}", async () =>
            {
                using var stream = await pdf.RenderAsync(page);
                return await ImageLoader.LoadAsync(stream);
            }));
        }

        await RunBatchAsync(sources, Loc.Get("Source_PdfAll", pdf.Name));
    }

    /// <summary>
    /// Runs the batch dialog and decides what to do with what it produced.
    /// </summary>
    private async Task RunBatchAsync(IReadOnlyList<BatchSource> sources, string sourceLabel)
    {
        if (_isBusy || sources.Count == 0)
        {
            return;
        }

        if (LanguageComboBox.SelectedItem is not OcrOption option)
        {
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
            var entries = await AppDialogs.ShowBatchAsync(
                RootGrid.XamlRoot, _hwnd, sources, option.Engine, NullIfEmpty(option.LanguageTag));

            if (entries.Count == 0)
            {
                return;
            }

            var merged = BatchRunner.Merge(entries, markdown: false);

            if (merged.Length > MaxInlineTextLength)
            {
                SetStatus(Loc.Get("Batch_TooLongForBox", entries.Count), InfoBarSeverity.Informational);
                return;
            }

            ResultTextBox.Text = merged;
            MatchVoiceToText(merged);
            AddHistoryItem(sourceLabel, merged, null);
            NoteSuccessfulRecognition();

            SetStatus(Loc.Get("Batch_DoneAll", entries.Count), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_RecognizeFailed", ex.Message), InfoBarSeverity.Error);
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

    /// <summary>
    /// Writes the result box to a file the user picks.
    /// </summary>
    /// <remarks>
    /// Until now the only thing that could be saved was the picture, which is the input. Anything
    /// longer than a paste — a scanned page, a whole PDF — had to be copied out through the
    /// clipboard one bufferful at a time.
    /// </remarks>
    private async void SaveTextClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ResultTextBox.Text))
        {
            SetStatus(Loc.Get("Status_NothingToSave"), InfoBarSeverity.Warning);
            return;
        }

        try
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = Path.GetFileNameWithoutExtension(_currentImageName) is { Length: > 0 } name
                    ? name
                    : "Glyfo"
            };
            picker.FileTypeChoices.Add(Loc.Get("FileType_Text"), new[] { ".txt" });
            picker.FileTypeChoices.Add(Loc.Get("FileType_Markdown"), new[] { ".md" });
            InitializeWithWindow.Initialize(picker, _hwnd);

            var target = await picker.PickSaveFileAsync();
            if (target is null)
            {
                return;
            }

            await TextFile.WriteAsync(target, ResultTextBox.Text);
            SetStatus(Loc.Get("Status_TextSaved", target.Name), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            SetStatus(Loc.Get("Status_TextSaveFailed", ex.Message), InfoBarSeverity.Error);
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

            // After the flush, not before: it can bump the sequence number a second time, and the
            // number recorded here is the one the update message will report back.
            ClipboardWatcher.NoteOwnWrite();
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

    // ---------------------------------------------------------------- smart actions

    /// <summary>The actions the bar is currently showing, so an unchanged list is not rebuilt.</summary>
    private IReadOnlyList<TextAction> _actions = Array.Empty<TextAction>();

    /// <summary>
    /// Hangs the whole feature off the text box.
    /// </summary>
    /// <remarks>
    /// Recognition, barcode scanning, translation, unwrapping and the user's own typing all end
    /// up assigning to <c>ResultTextBox.Text</c>, so listening here covers every one of them
    /// instead of six separate call sites that could each be forgotten.
    /// </remarks>
    private void ResultTextChanged(object sender, TextChangedEventArgs e) => RefreshSmartActions();

    /// <summary>The Search button only exists while something is selected, so it tracks selection.</summary>
    private void ResultSelectionChanged(object sender, RoutedEventArgs e) => RefreshSelectionAction();

    /// <summary>
    /// Rebuilds the row of buttons under the text.
    /// </summary>
    /// <remarks>
    /// Compares against the previous list first. This runs on every keystroke, and tearing down
    /// and rebuilding buttons while somebody is typing makes the row flicker under their hands
    /// for no gain — the addresses in a page of text do not change character by character.
    /// </remarks>
    private void RefreshSmartActions()
    {
        if (ActionsPanel is null)
        {
            return;
        }

        var actions = TextTools.FindActions(ResultTextBox.Text);
        if (actions.SequenceEqual(_actions))
        {
            RefreshSelectionAction();
            return;
        }

        _actions = actions;
        ActionsPanel.Children.Clear();

        foreach (var action in actions)
        {
            ActionsPanel.Children.Add(BuildActionChip(action));
        }

        RefreshSelectionAction();
    }

    /// <summary>
    /// Adds or removes the Search button, which is the only action that comes from the selection
    /// rather than from the text.
    /// </summary>
    /// <remarks>
    /// Kept last in the row and rebuilt separately so that changing the selection does not disturb
    /// the buttons beside it. Also the reason the bar can be useful on a page with no addresses at
    /// all: select a phrase, look it up.
    /// </remarks>
    private void RefreshSelectionAction()
    {
        if (ActionsPanel is null)
        {
            return;
        }

        var last = ActionsPanel.Children.Count > 0 ? ActionsPanel.Children[^1] : null;
        if (last is FrameworkElement { Tag: string tag } && tag == SearchChipTag)
        {
            ActionsPanel.Children.Remove(last);
        }

        var selection = ResultTextBox.SelectedText.Trim();
        if (selection.Length is > 0 and <= MaxSearchLength)
        {
            var chip = BuildChip("\uE721", Loc.Get("Action_Search"), selection);
            chip.Tag = SearchChipTag;
            chip.Click += async (_, _) => await LaunchAsync(
                "https://www.bing.com/search?q=" + Uri.EscapeDataString(selection));

            ActionsPanel.Children.Add(chip);
        }

        ActionsBar.Visibility = ActionsPanel.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private Button BuildActionChip(TextAction action)
    {
        var (glyph, label, uri) = action.Kind switch
        {
            TextActionKind.Email => ("\uE715", Loc.Get("Action_Mail"), "mailto:" + action.Value),
            TextActionKind.Phone => ("\uE717", Loc.Get("Action_Call"), "tel:" + DiallableForm(action.Value)),

            // A bare "www.something" is a valid address to a human and not a URI to Launcher,
            // which would take it for a relative path and refuse it.
            _ => ("\uE71B", Loc.Get("Action_Open"),
                  action.Value.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
                      ? "https://" + action.Value
                      : action.Value),
        };

        var chip = BuildChip(glyph, label, action.Value);
        chip.Click += async (_, _) => await LaunchAsync(uri);
        return chip;
    }

    /// <summary>
    /// One button: an icon, a shortened value, and the whole value in the tooltip.
    /// </summary>
    /// <remarks>
    /// The value rather than the verb, because on a row of six the verbs are all the same and the
    /// addresses are what tells them apart. The verb is in the icon and in the tooltip's first line.
    /// </remarks>
    private static Button BuildChip(string glyph, string verb, string value)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
        panel.Children.Add(new FontIcon { Glyph = glyph, FontSize = 13 });
        panel.Children.Add(new TextBlock
        {
            Text = Shorten(value),
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
        });

        var chip = new Button
        {
            Content = panel,
            Padding = new Thickness(10, 4, 10, 4),
            CornerRadius = new CornerRadius(14),
        };

        // Both, and not one or the other. The tooltip breaks the two apart because it has the room;
        // the accessible name has to be a single phrase, and without it a screen reader announces
        // a chip as "button" — the same reason SetTip exists for the icon-only buttons.
        ToolTipService.SetToolTip(chip, $"{verb}\n{value}");
        AutomationProperties.SetName(chip, $"{verb} {value}");
        return chip;
    }

    /// <summary>
    /// A phone number with the human formatting taken out.
    /// </summary>
    /// <remarks>
    /// Brackets and dashes are how a number is written down, not how it is dialled, and a
    /// <c>tel:</c> URI carrying them is refused by some handlers before the call is ever placed.
    /// The leading <c>+</c> is the one piece of punctuation that means something.
    /// </remarks>
    private static string DiallableForm(string value)
    {
        var digits = new string(value.Where(char.IsAsciiDigit).ToArray());
        return value.TrimStart().StartsWith('+') ? "+" + digits : digits;
    }

    private static string Shorten(string value) =>
        value.Length <= MaxChipLength ? value : value[..(MaxChipLength - 1)] + "…";

    /// <summary>
    /// Opens one of the chips. A failure here is the shell's refusal, not the app's, and there is
    /// nothing to do about it beyond saying so.
    /// </summary>
    private async Task LaunchAsync(string uri)
    {
        try
        {
            if (!await Windows.System.Launcher.LaunchUriAsync(new Uri(uri)))
            {
                SetStatus(Loc.Get("Status_LaunchFailed"), InfoBarSeverity.Warning);
            }
        }
        catch (Exception ex)
        {
            Trace.Write("LaunchAsync", ex);
            SetStatus(Loc.Get("Status_LaunchFailed"), InfoBarSeverity.Warning);
        }
    }

    /// <summary>Marks the Search button so it can be replaced without touching the others.</summary>
    private const string SearchChipTag = "search";

    /// <summary>Longest value shown on a chip before it is cut; the tooltip still has all of it.</summary>
    private const int MaxChipLength = 28;

    /// <summary>
    /// Longest selection that gets a Search button. Past this the user has selected a paragraph,
    /// not a term, and a search box is not what they were reaching for.
    /// </summary>
    private const int MaxSearchLength = 120;

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
    /// Whether a picture copied anywhere on the machine is read automatically.
    /// </summary>
    /// <remarks>
    /// The listener is registered for the life of the window either way; this only decides whether
    /// it reports. Nothing is read while the switch is off, and turning it back on does not go
    /// looking at whatever happens to be on the clipboard already — only the next copy counts.
    /// </remarks>
    private void WatchClipboardToggled(object sender, RoutedEventArgs e)
    {
        var on = WatchClipboardToggle.IsOn;
        AppSettings.Current.WatchClipboard = on;

        if (_clipboard is not null)
        {
            _clipboard.IsEnabled = on;
        }
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

        while (_history.Count > HistoryStore.MaxItems)
        {
            _history.RemoveAt(_history.Count - 1);
        }

        RefreshHistoryView();
        SaveHistory();
    }

    /// <summary>
    /// Rebuilds the bound list from <see cref="_history"/> and the search box.
    /// </summary>
    /// <remarks>
    /// Clear-and-refill rather than a computed diff. Two hundred rows is nothing to rebuild, the
    /// flyout is usually closed while this runs, and a diff would be code to maintain in exchange
    /// for no difference anyone could see.
    /// </remarks>
    private void RefreshHistoryView()
    {
        var query = HistorySearchBox.Text.Trim();

        _historyView.Clear();

        foreach (var item in _history)
        {
            if (query.Length == 0 || Matches(item, query))
            {
                _historyView.Add(item);
            }
        }

        UpdateHistoryEmptyText();
    }

    /// <summary>
    /// Whether an entry answers the search. Matches the source label as well as the text, so
    /// "PDF" or "screenshot" finds a whole kind of entry and not just words that were read.
    /// </summary>
    private static bool Matches(HistoryItem item, string query) =>
        item.Text.Contains(query, StringComparison.CurrentCultureIgnoreCase)
        || item.Source.Contains(query, StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// Shows the right one of three "nothing here" messages, or none at all.
    /// </summary>
    /// <remarks>
    /// The three are genuinely different situations and telling them apart is the whole point:
    /// history turned off is a setting the user can undo, a search with no hits is a query they can
    /// retype, and an empty list is neither.
    /// </remarks>
    private void UpdateHistoryEmptyText()
    {
        if (_historyView.Count > 0)
        {
            HistoryEmptyText.Visibility = Visibility.Collapsed;
            return;
        }

        HistoryEmptyText.Text =
            !AppSettings.Current.KeepHistory && _history.Count == 0 ? Loc.Get("History_Off")
            : _history.Count > 0 ? Loc.Get("History_NoMatch")
            : Loc.Get("History_Empty");

        HistoryEmptyText.Visibility = Visibility.Visible;
    }

    /// <summary>Writes the list out, unless the user has asked for it not to be kept.</summary>
    private void SaveHistory()
    {
        if (AppSettings.Current.KeepHistory)
        {
            HistoryStore.SaveInBackground(_history);
        }
    }

    private void HistorySearchChanged(object sender, TextChangedEventArgs e) => RefreshHistoryView();

    private void HistoryClearClick(object sender, RoutedEventArgs e)
    {
        if (_history.Count == 0)
        {
            return;
        }

        _history.Clear();
        HistorySearchBox.Text = string.Empty;

        // Not merely "stop writing": the file has to go now, or clearing the list would leave
        // everything it contained on disk to reappear at the next start.
        HistoryStore.Delete();

        RefreshHistoryView();
        SetStatus(Loc.Get("Status_HistoryCleared"), InfoBarSeverity.Success);
    }

    /// <summary>
    /// Labels the row's menu as it opens.
    /// </summary>
    /// <remarks>
    /// Set here rather than in the markup because the items live inside a <c>DataTemplate</c>, whose
    /// instances <c>ApplyLanguage</c> cannot reach by name. Reading the strings at open time also
    /// means a row created before a language switch still opens a menu in the new language.
    /// </remarks>
    private void HistoryItemMenuOpening(object sender, object e)
    {
        if (sender is not MenuFlyout flyout || flyout.Items.Count < 2)
        {
            return;
        }

        if (flyout.Items[0] is MenuFlyoutItem copy)
        {
            copy.Text = Loc.Get("History_CopyItem");
        }

        if (flyout.Items[1] is MenuFlyoutItem delete)
        {
            delete.Text = Loc.Get("History_DeleteItem");
        }
    }

    private void HistoryCopyItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: HistoryItem item })
        {
            return;
        }

        if (CopyTextToClipboard(item.Text, out var error))
        {
            SetStatus(Loc.Get("Status_TextCopied"), InfoBarSeverity.Success);
        }
        else
        {
            SetStatus(Loc.Get("Status_CopyFailed", error), InfoBarSeverity.Error);
        }
    }

    private void HistoryDeleteItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: HistoryItem item })
        {
            return;
        }

        _history.Remove(item);
        RefreshHistoryView();

        // Rewritten rather than left for the next recognition: someone deleting one entry means it
        // should be gone from the disk too, not merely from the list until the app next saves.
        if (AppSettings.Current.KeepHistory)
        {
            if (_history.Count == 0)
            {
                HistoryStore.Delete();
            }
            else
            {
                HistoryStore.SaveInBackground(_history);
            }
        }
    }

    /// <summary>
    /// Whether the history survives a restart.
    /// </summary>
    /// <remarks>
    /// Turning it off deletes what is already stored and empties the list on the spot. Anything less
    /// would leave the user with a switch that reads as "forget my history" and behaves as "stop
    /// adding to it", which is the kind of gap that belongs in a privacy complaint rather than in a
    /// settings panel.
    /// </remarks>
    private void KeepHistoryToggled(object sender, RoutedEventArgs e)
    {
        if (_suppressKeepHistoryToggle)
        {
            return;
        }

        var on = KeepHistoryToggle.IsOn;
        AppSettings.Current.KeepHistory = on;

        if (on)
        {
            // Whatever this session has already read is worth keeping; there is nothing on disk to
            // merge with, because turning it off deleted it.
            SaveHistory();
        }
        else
        {
            _history.Clear();
            HistorySearchBox.Text = string.Empty;
            HistoryStore.Delete();
        }

        RefreshHistoryView();
    }

    // ---------------------------------------------------------------- screen capture

    private async Task CaptureFullScreenAndRunAsync()
    {
        var hidden = await HideForCaptureAsync();
        try
        {
            var (path, _) = await Task.Run(CaptureVirtualScreen);

            // Back before the reading starts, not after: recognition takes a second or two, and
            // the window is where its progress and its result are shown.
            if (hidden)
            {
                ShowAfterCapture();
                hidden = false;
            }

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
        finally
        {
            if (hidden)
            {
                ShowAfterCapture();
            }
        }
    }

    private async Task CaptureRegionAndRunAsync()
    {
        var hidden = await HideForCaptureAsync();
        string? screenshotPath = null;
        try
        {
            // Freeze the screen first: the overlay then draws on top of a still image instead of
            // racing with the live desktop, so it can never end up inside the captured pixels.
            var (path, bounds) = await Task.Run(CaptureVirtualScreen);
            screenshotPath = path;

            var captureWindow = new RegionCaptureWindow(screenshotPath, bounds);
            var selection = await captureWindow.CaptureAsync();

            // Only now, and not a moment earlier: the overlay covers every monitor, so a window
            // brought back underneath it would be invisible anyway, and taking the foreground off
            // the overlay would leave its Escape key going nowhere.
            if (hidden)
            {
                ShowAfterCapture();
                hidden = false;
            }

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
            if (hidden)
            {
                ShowAfterCapture();
            }

            if (screenshotPath is not null && !TryDeleteFile(screenshotPath))
            {
                // Still referenced by the overlay's BitmapImage; the next startup sweep gets it.
                _sessionTempFiles.Add(screenshotPath);
            }
        }
    }

    /// <summary>
    /// Takes the window off the screen so that it cannot end up inside the capture, and reports
    /// whether it now has to be put back.
    /// </summary>
    /// <remarks>
    /// Unconditional, with no setting behind it: nobody reaches for a capture tool in order to
    /// read Glyfo's own window, and every other one on Windows does the same thing.
    ///
    /// Deliberately does not touch <see cref="_isHidden"/>. That flag means "living in the tray",
    /// which is what decides whether a result is copied to the clipboard and announced by toast;
    /// a window that vanishes for a fifth of a second is not that.
    /// </remarks>
    private async Task<bool> HideForCaptureAsync()
    {
        // Already off the screen — captured from the tray, or from the shortcut while minimized.
        // Showing it afterwards would be putting up a window the user never had open.
        if (_isClosed || _appWindow is null || _isHidden || !_appWindow.IsVisible)
        {
            return false;
        }

        if (_appWindow.Presenter is OverlappedPresenter { State: OverlappedPresenterState.Minimized })
        {
            return false;
        }

        _appWindow.Hide();

        // The capture reads pixels off the screen, and Hide only starts the process of clearing
        // them: DWM animates the window out and repaints what was behind it a frame or two later.
        // Without the wait the image contains a half-faded copy of the window.
        await Task.Delay(CaptureHideDelayMs);
        return true;
    }

    /// <summary>Puts the window back after <see cref="HideForCaptureAsync"/> hid it.</summary>
    private void ShowAfterCapture()
    {
        if (_isClosed || _appWindow is null)
        {
            return;
        }

        _appWindow.Show();
        SetForegroundWindow(_hwnd);
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
        _tray = new TrayIcon(_hwnd) { HotkeyText = RegionHotkeyText };

        _tray.OpenRequested += (_, _) => _dispatcherQueue.TryEnqueue(ShowFromTray);
        _tray.CaptureRegionRequested += (_, _) =>
            _dispatcherQueue.TryEnqueue(async () => await CaptureRegionAndRunAsync());
        _tray.CaptureFullScreenRequested += (_, _) =>
            _dispatcherQueue.TryEnqueue(async () => await CaptureFullScreenAndRunAsync());
        _tray.ExitRequested += (_, _) => _dispatcherQueue.TryEnqueue(ExitApp);
    }

    // ------------------------------------------------------------- clipboard watch

    /// <summary>
    /// Starts watching the clipboard, if the user has asked for it.
    /// </summary>
    /// <remarks>
    /// The listener is registered either way and only reports while the setting is on, so flipping
    /// the switch never has to create or destroy anything. See <see cref="ClipboardWatcher"/>.
    /// </remarks>
    private void InitializeClipboardWatcher()
    {
        _clipboard = new ClipboardWatcher(_hwnd) { IsEnabled = AppSettings.Current.WatchClipboard };

        // Through the dispatcher for the same reason the tray handlers are: this runs inside the
        // window procedure, and reading the clipboard there would re-enter it.
        _clipboard.ContentChanged += (_, _) =>
            _dispatcherQueue.TryEnqueue(async () => await ReadClipboardImageAsync());
    }

    /// <summary>
    /// Reads a picture that has just been copied, if that is what it is.
    /// </summary>
    /// <remarks>
    /// Only <see cref="StandardDataFormats.Bitmap"/> counts. Copied text has to pass in complete
    /// silence, and a picture <em>file</em> copied in Explorer is not a screenshot either — that is
    /// someone moving a file around, and reading it would be startling. What remains is exactly the
    /// case this exists for: Win+Shift+S, "Copy image" in a browser, a screenshot from a chat app.
    ///
    /// Deliberately does not bring the window up. Not having to switch windows is the whole point,
    /// and the text goes on the clipboard either way — see the <c>alwaysCopy</c> argument to
    /// <see cref="RunOcrAsync"/>. Whether that is also announced with a toast is a separate
    /// question, answered by <see cref="UserIsElsewhere"/>.
    /// </remarks>
    private async Task ReadClipboardImageAsync()
    {
        if (_isClosed || _isBusy)
        {
            return;
        }

        try
        {
            var clipboard = Clipboard.GetContent();
            if (!clipboard.Contains(StandardDataFormats.Bitmap))
            {
                return;
            }

            var streamRef = await clipboard.GetBitmapAsync();
            if (streamRef is null)
            {
                return;
            }

            // One line per picture actually read, and nothing for the copies that are ignored:
            // enough to answer "why did it not read my screenshot", without keeping a record of
            // everything the user copies.
            Trace.Write("clipboard watch: reading copied picture");

            var label = Loc.Get("Source_Clipboard");
            await LoadImageAsync(streamRef, label);
            await RunOcrAsync(label, alwaysCopy: true);
        }
        catch (Exception ex)
        {
            // Never a toast and never a dialog: the user did not ask for anything here, they copied
            // something. A line in the log is what this is worth, and it is also the only way to
            // tell "the platform refused to hand over the clipboard" from "nothing happened".
            Trace.Write("clipboard watch", ex);
        }
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
    /// Puts the window back where it was left, or sizes it for this display on a first run.
    /// </summary>
    /// <remarks>
    /// The stored rectangle is clamped against the work area of whichever display it lands nearest
    /// to before it is used. Restoring a saved position as-is is how this feature usually breaks:
    /// a window last closed on a second monitor comes back entirely off-screen once that monitor is
    /// unplugged, with no title bar left to drag it home by.
    /// </remarks>
    private static void RestoreWindowGeometry(AppWindow appWindow, IntPtr hwnd)
    {
        const int minWidth = 1060;
        const int minHeight = 640;

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            // Below this the action bar and the text-pane toolbar start clipping.
            presenter.PreferredMinimumWidth = minWidth;
            presenter.PreferredMinimumHeight = minHeight;
        }

        var width = AppSettings.Current.WindowWidth;
        var height = AppSettings.Current.WindowHeight;
        if (width <= 0 || height <= 0)
        {
            // AppWindow.Resize takes raw pixels while the XAML inside is laid out in DIPs. Passing
            // a fixed 1280 on a 150% display produced an 853 DIP window, which is narrower than the
            // two toolbars need and clipped their trailing buttons.
            var scale = GetDpiForWindow(hwnd) / 96.0;
            appWindow.Resize(new SizeInt32((int)(1280 * scale), (int)(820 * scale)));
            return;
        }

        var stored = new RectInt32(AppSettings.Current.WindowX, AppSettings.Current.WindowY, width, height);
        var work = DisplayArea.GetFromRect(stored, DisplayAreaFallback.Nearest).WorkArea;

        stored.Width = Math.Min(Math.Max(width, minWidth), work.Width);
        stored.Height = Math.Min(Math.Max(height, minHeight), work.Height);

        // Reachable, not fully contained. A window left a little taller than the work area, with
        // its bottom edge behind the taskbar, is an ordinary thing to find; nudging it up every
        // launch would be the opposite of remembering where it was. What has to hold is that the
        // title bar can still be grabbed, which is exactly what stops holding when the display it
        // was closed on is no longer there.
        const int reachable = 120;
        stored.X = Math.Min(Math.Max(stored.X, work.X - stored.Width + reachable), work.X + work.Width - reachable);
        stored.Y = Math.Min(Math.Max(stored.Y, work.Y), work.Y + work.Height - reachable);
        appWindow.MoveAndResize(stored);

        // After the move rather than before it: the rectangle a maximized window is restored to is
        // the one it had when it was maximized, so moving it afterwards would set the wrong one.
        if (AppSettings.Current.WindowMaximized && appWindow.Presenter is OverlappedPresenter overlapped)
        {
            overlapped.Maximize();
        }
    }

    /// <summary>
    /// Records where the window is, so the next launch can put it back.
    /// </summary>
    /// <remarks>
    /// Two states are deliberately not recorded. Minimized, because its rectangle is not where the
    /// window lives and writing it would lose the real one. Hidden to the tray, because by then the
    /// geometry worth keeping is the one saved on the way in, and the tray's Exit comes back
    /// through the same handler afterwards. Maximized records the flag but leaves the rectangle
    /// alone, so un-maximizing on the next run lands on the size the user actually chose.
    /// </remarks>
    private void SaveWindowGeometry()
    {
        if (_appWindow is null || _isHidden)
        {
            return;
        }

        var maximized = false;
        if (_appWindow.Presenter is OverlappedPresenter presenter)
        {
            if (presenter.State == OverlappedPresenterState.Minimized)
            {
                return;
            }

            maximized = presenter.State == OverlappedPresenterState.Maximized;
        }

        AppSettings.Current.WindowMaximized = maximized;
        if (maximized)
        {
            return;
        }

        var position = _appWindow.Position;
        var size = _appWindow.Size;
        AppSettings.Current.WindowX = position.X;
        AppSettings.Current.WindowY = position.Y;
        AppSettings.Current.WindowWidth = size.Width;
        AppSettings.Current.WindowHeight = size.Height;
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
        // Before the branches, not inside them: this one line then covers the caption button,
        // Alt+F4, the taskbar's close command, hiding to the tray and the tray's own Exit, which
        // arrives here a second time with the window already hidden.
        SaveWindowGeometry();

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
                Toasts.ShowMinimizedToTray(RegionHotkeyText);
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
        if (_regionHotkey.IsEmpty)
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
            Toasts.ShowStartupHint(RegionHotkeyText);
        }
    }

    // ------------------------------------------------------- settings, about, notes, rating

    /// <summary>
    /// Opens the settings dialog, and then About if that is what was asked for on the way out.
    /// </summary>
    /// <remarks>
    /// About cannot simply be opened from inside: WinUI refuses to show a second
    /// <see cref="ContentDialog"/> over an open one. The link therefore closes this dialog and
    /// leaves a note, which is answered here — the same shape as the rating link inside About.
    /// </remarks>
    private async void SettingsClick(object sender, RoutedEventArgs e)
    {
        _aboutRequested = false;
        await AppDialogs.ShowGuardedAsync(SettingsDialog, RootGrid.XamlRoot);

        // Losing focus normally ends a recording, and dismissing the dialog does move focus. This
        // is the backstop for the paths where it does not, because the cost of missing one is that
        // the app runs on with no capture shortcut registered at all until it is restarted.
        if (_recordingButton is not null)
        {
            EndRecording();
        }

        if (_aboutRequested)
        {
            _aboutRequested = false;
            await AppDialogs.ShowAboutAsync(RootGrid.XamlRoot, _hwnd);
        }
    }

    private void AboutClick(object sender, RoutedEventArgs e)
    {
        _aboutRequested = true;
        SettingsDialog.Hide();
    }

    /// <summary>
    /// Shows what changed, once, the first time a version the user has not read about is opened.
    /// </summary>
    /// <remarks>
    /// Skipped entirely when the window is hidden. Started with Windows, the app goes straight to
    /// the notification area and never activates the window (see <c>App.OnLaunched</c>), and a modal
    /// dialog on a window nobody can see is a dialog nobody can dismiss. Nothing is written in that
    /// case either, so the notes are still waiting the next time the window is actually opened.
    /// </remarks>
    private async Task ShowWhatsNewIfUpdatedAsync()
    {
        // Before anything is decided, because App.OnLaunched calls MarkStartedHidden() after this
        // window's constructor has already started the task: reading _isHidden any earlier reads it
        // as false on exactly the launch that must not show a dialog.
        await Task.Delay(WhatsNewDelayMs);

        if (_isClosed || _isHidden || RootGrid.XamlRoot is null)
        {
            Trace.Write($"release notes skipped: closed={_isClosed} hidden={_isHidden} root={RootGrid.XamlRoot is not null}");
            return;
        }

        var stored = AppSettings.Current.LastSeenVersion;
        Version? lastSeen = Version.TryParse(stored, out var parsed) ? parsed : null;

        // Nothing stored and nothing else stored either: this is a first-ever install, which has no
        // "what changed" to be told about. Record where it came in so the next update does.
        if (lastSeen is null && !AppSettings.Current.HasEarlierState)
        {
            Trace.Write($"release notes skipped: first install, marking {ProductInfo.Current}");
            AppSettings.Current.LastSeenVersion = ProductInfo.Current.ToString();
            return;
        }

        var releases = Changelog.Since(lastSeen);
        Trace.Write($"release notes: lastSeen={stored} current={ProductInfo.Current} unread={releases.Count}");

        if (releases.Count == 0)
        {
            // Still worth writing: it collapses "upgraded from 1.0.x" to an ordinary up-to-date
            // install, so HasEarlierState is only consulted the once.
            AppSettings.Current.LastSeenVersion = ProductInfo.Current.ToString();
            return;
        }

        _interrupted = true;
        AppSettings.Current.LastSeenVersion = ProductInfo.Current.ToString();

        await AppDialogs.ShowWhatsNewAsync(RootGrid.XamlRoot, _hwnd, releases);
    }

    /// <summary>
    /// Counts a recognition that worked, and asks for a rating once enough of them have.
    /// </summary>
    /// <remarks>
    /// The count is of successes rather than launches because the question only makes sense to
    /// someone the app has already worked for. Hidden windows are skipped for the obvious reason
    /// that the tip would be pointing at a button that is not on screen.
    /// </remarks>
    private void NoteSuccessfulRecognition()
    {
        var count = AppSettings.Current.RecognizeCount + 1;
        AppSettings.Current.RecognizeCount = count;

        if (count < RatingPromptAfter ||
            AppSettings.Current.RatingPromptDone ||
            _isHidden ||
            _interrupted)
        {
            return;
        }

        // Set before it is shown, not in the handlers: light dismiss raises neither of them, and a
        // tip that came back because the user clicked past it is worse than one they never saw.
        AppSettings.Current.RatingPromptDone = true;
        _interrupted = true;
        RatingTip.IsOpen = true;
    }

    private async void RatingTipAction(TeachingTip sender, object args)
    {
        RatingTip.IsOpen = false;
        await ProductInfo.RateAsync(_hwnd);
    }

    private void RatingTipClose(TeachingTip sender, object args) => RatingTip.IsOpen = false;

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

    /// <summary>The region shortcut for prose — the sentence that says so when there is none.</summary>
    private string RegionHotkeyText =>
        _regionHotkey.IsEmpty ? Loc.Get("Hotkey_Unavailable") : _regionHotkey.ToString();

    /// <summary>
    /// Takes both capture shortcuts, from settings where the user has chosen one and from the
    /// built-in defaults where they have not.
    /// </summary>
    /// <remarks>
    /// Safe to call again at any time — it releases both ids first — which is what lets a shortcut
    /// be changed without restarting, and what puts them back after the recorder has borrowed them.
    /// Both registrations are kept this time: a combination another app already owns comes back
    /// empty, and the labels are built from these two fields rather than from the request, so
    /// nothing can end up advertising a key that does nothing.
    /// </remarks>
    private void RegisterHotkeys()
    {
        UnregisterHotKey(_hwnd, FullHotkeyId);
        UnregisterHotKey(_hwnd, RegionHotkeyId);

        var chosenFull = AppSettings.Current.FullHotkey;
        _fullHotkey = Take(FullHotkeyId, chosenFull.IsUsable ? chosenFull : Hotkey.FullDefault);

        // Alt+Z is what users coming from other capture tools reach for first; if another app
        // already owns it, fall back rather than leave region capture without a shortcut at all.
        // The fallback is for the default only: a combination the user typed in on purpose is not
        // something to quietly swap out from under them.
        var chosenRegion = AppSettings.Current.RegionHotkey;
        if (chosenRegion.IsUsable)
        {
            _regionHotkey = Take(RegionHotkeyId, chosenRegion);
        }
        else
        {
            _regionHotkey = Take(RegionHotkeyId, Hotkey.RegionDefault);
            if (_regionHotkey.IsEmpty)
            {
                _regionHotkey = Take(RegionHotkeyId, Hotkey.RegionFallback);
            }
        }

        Hotkey Take(int id, Hotkey key) =>
            RegisterHotKey(_hwnd, id, key.Modifiers, key.Key) ? key : default;
    }

    /// <summary>Writes the two live combinations into every place that names one.</summary>
    private void RefreshHotkeyLabels()
    {
        SetTip(CaptureButton, _fullHotkey.IsEmpty
            ? Loc.Get("Tip_CaptureRegionOnly", RegionHotkeyText)
            : Loc.Get("Tip_CaptureKeys", RegionHotkeyText, _fullHotkey.ToString()));

        // Blank when the combination is taken, rather than the sentence that says so: this slot is
        // for a key name, and the tooltip already explains the situation in full.
        CaptureRegionItem.KeyboardAcceleratorTextOverride = _regionHotkey.ToString();
        CaptureFullScreenItem.KeyboardAcceleratorTextOverride = _fullHotkey.ToString();

        HotkeyRegionButton.Content = _regionHotkey.IsEmpty ? NoHotkeyText : _regionHotkey.ToString();
        HotkeyFullButton.Content = _fullHotkey.IsEmpty ? NoHotkeyText : _fullHotkey.ToString();

        // Null on the first pass: the language is applied before the tray icon exists.
        if (_tray is not null)
        {
            _tray.HotkeyText = RegionHotkeyText;
            _tray.Refresh();
        }
    }

    private void HotkeyRegionClick(object sender, RoutedEventArgs e) => BeginRecording(HotkeyRegionButton, region: true);

    private void HotkeyFullClick(object sender, RoutedEventArgs e) => BeginRecording(HotkeyFullButton, region: false);

    /// <summary>
    /// Turns one of the two buttons into a listener for the next combination.
    /// </summary>
    /// <remarks>
    /// Both registrations come down for the duration, and that is not tidiness. The system swallows
    /// a combination it has registered — it never reaches the focused window's input queue — so
    /// with Alt+Z still held, pressing Alt+Z here would open the capture overlay instead of being
    /// recorded, and the one shortcut the user most wants to change would be the one they could
    /// never type.
    /// </remarks>
    private void BeginRecording(Button button, bool region)
    {
        UnregisterHotKey(_hwnd, FullHotkeyId);
        UnregisterHotKey(_hwnd, RegionHotkeyId);

        _recordingButton = button;
        _recordingRegion = region;
        button.Content = Loc.Get("Btn_RecordingHotkey");
        button.Focus(FocusState.Programmatic);
    }

    /// <summary>Stops listening and puts the pair back, from whatever settings now hold.</summary>
    private void EndRecording()
    {
        _recordingButton = null;
        RegisterHotkeys();
        RefreshHotkeyLabels();
    }

    /// <summary>
    /// Reads the combination off the keyboard while a button is listening.
    /// </summary>
    /// <remarks>
    /// The modifiers are asked for rather than taken from the event, because the event only ever
    /// carries one key. The non-modifier key is the one that ends the recording, and it is
    /// guaranteed to arrive: it is the modifiers that route oddly, not it. Every key is marked
    /// handled so that Space and Enter do not reach the button underneath and Enter does not close
    /// the dialog.
    /// </remarks>
    private void HotkeyRecordKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (_recordingButton is null)
        {
            return;
        }

        e.Handled = true;
        var vk = (uint)e.Key;

        if (e.Key == Windows.System.VirtualKey.Escape)
        {
            EndRecording();
            return;
        }

        if (Hotkey.IsModifierKey(vk))
        {
            return;
        }

        var candidate = new Hotkey(CurrentModifiers(), vk);
        if (!candidate.IsUsable)
        {
            // Still listening: a rejected key is almost always a slip, and dropping out of
            // recording would make the user click the button again to try the one they meant.
            SetStatus(Loc.Get("Hotkey_NeedModifier"), InfoBarSeverity.Warning);
            return;
        }

        ApplyRecordedHotkey(candidate);
    }

    private void HotkeyRecordLostFocus(object sender, RoutedEventArgs e)
    {
        if (ReferenceEquals(_recordingButton, sender))
        {
            EndRecording();
        }
    }

    /// <summary>
    /// Stores the recorded combination, then keeps it only if it could actually be registered.
    /// </summary>
    /// <remarks>
    /// Trying to take it is the only test there is: nothing will say whether another application
    /// already owns a combination without attempting to take it away. So the value is written,
    /// registered through the ordinary path, and rolled back when what came out is not what went
    /// in — leaving the settings showing a shortcut that is not live would be the worse failure.
    /// </remarks>
    private void ApplyRecordedHotkey(Hotkey key)
    {
        var region = _recordingRegion;
        var previous = region ? AppSettings.Current.RegionHotkey : AppSettings.Current.FullHotkey;

        if (region)
        {
            AppSettings.Current.RegionHotkey = key;
        }
        else
        {
            AppSettings.Current.FullHotkey = key;
        }

        EndRecording();

        if ((region ? _regionHotkey : _fullHotkey) == key)
        {
            return;
        }

        if (region)
        {
            AppSettings.Current.RegionHotkey = previous;
        }
        else
        {
            AppSettings.Current.FullHotkey = previous;
        }

        EndRecording();
        SetStatus(Loc.Get("Hotkey_Taken", key.ToString()), InfoBarSeverity.Warning);
    }

    private void ResetHotkeysClick(object sender, RoutedEventArgs e)
    {
        AppSettings.Current.RegionHotkey = default;
        AppSettings.Current.FullHotkey = default;
        EndRecording();
    }

    /// <summary>
    /// Which of Ctrl, Alt and Shift are down at this instant.
    /// </summary>
    /// <remarks>
    /// Asked of the input system rather than read off the key event, which reports the one key it
    /// is about and nothing else.
    /// </remarks>
    private static uint CurrentModifiers()
    {
        uint modifiers = 0;
        if (IsDown(Windows.System.VirtualKey.Control))
        {
            modifiers |= Hotkey.ModControl;
        }

        if (IsDown(Windows.System.VirtualKey.Menu))
        {
            modifiers |= Hotkey.ModAlt;
        }

        if (IsDown(Windows.System.VirtualKey.Shift))
        {
            modifiers |= Hotkey.ModShift;
        }

        return modifiers;

        static bool IsDown(Windows.System.VirtualKey key) =>
            (Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(key)
                & Windows.UI.Core.CoreVirtualKeyStates.Down) != 0;
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

        if (_clipboard?.HandleMessage(msg) == true)
        {
            return IntPtr.Zero;
        }

        if (_tray?.HandleMessage(msg, wParam, lParam) == true)
        {
            return IntPtr.Zero;
        }

        return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
    }

    // ---------------------------------------------------------------- helpers

    /// <summary>
    /// Whether a result has to announce itself, because the user is not looking at this window.
    /// </summary>
    /// <remarks>
    /// Broader than <see cref="_isHidden"/> on purpose. Living in the tray is one way of not being
    /// looked at; sitting behind the browser the user is actually reading is another, and the
    /// clipboard watch makes the second one the common case. Both want the same thing: the text on
    /// the clipboard, and a toast to say it is there.
    /// </remarks>
    private bool UserIsElsewhere => _isHidden || GetForegroundWindow() != _hwnd;

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

    private static bool IsPdf(string fileName) =>
        Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);

    /// <summary>Everything the window can open, whatever it arrived through.</summary>
    private static bool IsSupportedFile(string fileName) => IsSupportedImage(fileName) || IsPdf(fileName);

    /// <summary>
    /// Opens a file the right way for what it is.
    /// </summary>
    /// <remarks>
    /// Every entry point — the picker, paste, drop, "Open with", share — goes through here rather
    /// than deciding for itself, so that adding a third kind of file later is one change and not
    /// five.
    /// </remarks>
    private Task OpenFileAsync(StorageFile file) =>
        IsPdf(file.Name) ? OpenPdfAsync(file) : LoadImageAsync(file);

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

        _clipboard?.Dispose();
        _clipboard = null;

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

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

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
