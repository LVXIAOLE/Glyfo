using Glyfo.Services;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Glyfo;

/// <summary>
/// The two dialogs that talk about the app rather than about an image: "About", and the release
/// notes shown the first time a new version is opened.
/// </summary>
/// <remarks>
/// Built in code rather than declared in XAML, for the same reason the window carries no display
/// text: every string comes from <see cref="Loc"/>, and a dialog constructed at the moment it is
/// opened is always in the language chosen by then. A XAML dialog would have to be re-read by
/// <c>ApplyLanguage</c> like the rest of the window, for something that is on screen for ten
/// seconds a month.
/// </remarks>
internal static class AppDialogs
{
    /// <summary>The app glyph, the same one the title bar uses.</summary>
    private const string AppGlyph = "\uE721";

    /// <summary>
    /// Whether one of these is already up. WinUI throws if a second <see cref="ContentDialog"/> is
    /// shown while one is open, and the release notes can collide with a fast click on About.
    /// </summary>
    private static bool _isOpen;

    /// <summary>
    /// Shows a dialog declared elsewhere — the settings page — under the same single-open guard as
    /// the ones built here, and with the two properties a dialog cannot inherit.
    /// </summary>
    /// <remarks>
    /// The guard has to be shared rather than duplicated: the release notes open by themselves a
    /// few seconds after launch, which is exactly when someone is likely to be reaching for the
    /// settings button. Returns <see cref="ContentDialogResult.None"/> without showing anything
    /// when another dialog already has the screen.
    /// </remarks>
    public static async Task<ContentDialogResult> ShowGuardedAsync(ContentDialog dialog, XamlRoot root)
    {
        if (_isOpen)
        {
            return ContentDialogResult.None;
        }

        dialog.XamlRoot = root;
        dialog.FlowDirection = Loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        _isOpen = true;
        try
        {
            return await dialog.ShowAsync();
        }
        finally
        {
            _isOpen = false;
        }
    }

    /// <summary>Shows "About Glyfo", with the version and the four links worth having.</summary>
    public static Task ShowAboutAsync(XamlRoot root, IntPtr hwnd) =>
        ShowAsync(root, hwnd, whatsNew: null);

    /// <summary>Shows what changed in <paramref name="releases"/>, newest first.</summary>
    public static Task ShowWhatsNewAsync(XamlRoot root, IntPtr hwnd, IReadOnlyList<Release> releases) =>
        ShowAsync(root, hwnd, releases);

    /// <summary>
    /// Reads several images in one go, and offers the result as one file, as one file per item, or
    /// on the clipboard. Returns what was read, which is empty when the run was stopped before the
    /// first source finished.
    /// </summary>
    /// <remarks>
    /// The dialog owns the run rather than being handed a finished list, because the run is the
    /// part that needs a place to show progress and a way to be stopped. The entries rather than
    /// the merged text come back, so the window can say how many there were as well as show them.
    /// </remarks>
    public static async Task<IReadOnlyList<BatchEntry>> ShowBatchAsync(
        XamlRoot root,
        IntPtr hwnd,
        IReadOnlyList<BatchSource> sources,
        IOcrEngine engine,
        string? languageTag)
    {
        if (_isOpen || sources.Count == 0)
        {
            return Array.Empty<BatchEntry>();
        }

        var progressText = new TextBlock { Text = Loc.Get("Batch_Progress", 0, sources.Count) };
        var progressBar = new ProgressBar { Minimum = 0, Maximum = sources.Count, Value = 0 };
        var lines = new StackPanel { Spacing = 2 };

        var separate = new CheckBox
        {
            Content = Loc.Get("Batch_Separate"),
            Visibility = Visibility.Collapsed,
        };

        // Built in code, so there is no x:Name for the UI tests to find it by.
        Microsoft.UI.Xaml.Automation.AutomationProperties.SetAutomationId(separate, "BatchSeparate");

        var note = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Visibility = Visibility.Collapsed,
            FontSize = 12,
            Opacity = 0.7,
        };

        Microsoft.UI.Xaml.Automation.AutomationProperties.SetAutomationId(note, "BatchNote");

        var panel = new StackPanel { Spacing = 12, Width = 380 };
        panel.Children.Add(progressText);
        panel.Children.Add(progressBar);
        panel.Children.Add(new ScrollViewer
        {
            // Tall enough to see a run take shape, short enough that the buttons stay on screen
            // on a laptop display.
            MaxHeight = 220,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollMode = ScrollMode.Disabled,
            Content = lines,
        });
        panel.Children.Add(separate);
        panel.Children.Add(note);

        var dialog = new ContentDialog
        {
            XamlRoot = root,
            FlowDirection = Loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
            Title = Loc.Get("Batch_Title"),
            Content = panel,

            // While the run is going the only button is the one that stops it; the rest appear
            // when there is something to act on.
            CloseButtonText = Loc.Get("Common_Cancel"),
        };

        using var cancellation = new CancellationTokenSource();
        var entries = new List<BatchEntry>();
        var running = true;

        // Cancel stops the run but keeps the dialog: the pages that did finish are worth saving,
        // and closing on the user would throw them away.
        dialog.Closing += (_, args) =>
        {
            if (running)
            {
                args.Cancel = true;
                cancellation.Cancel();
            }
        };

        dialog.PrimaryButtonClick += async (_, args) =>
        {
            // Neither button is a "done" button — they are things to do with the result, and the
            // user may well want both. The dialog stays until Close.
            args.Cancel = true;
            var deferral = args.GetDeferral();
            try
            {
                await SaveAsync();
            }
            finally
            {
                deferral.Complete();
            }
        };

        dialog.SecondaryButtonClick += (_, args) =>
        {
            args.Cancel = true;
            CopyAll();
        };

        dialog.Opened += async (_, _) =>
        {
            // Counted here rather than read off `entries`: that list is only assigned when the run
            // returns, so during the run it is still the empty one declared above.
            var done = 0;

            var progress = new Progress<BatchEntry>(entry =>
            {
                done++;
                progressBar.Value = done;
                progressText.Text = Loc.Get("Batch_Progress", done, sources.Count);
                lines.Children.Add(Line(entry));
            });

            try
            {
                entries = await BatchRunner.RunAsync(sources, engine, languageTag, progress, cancellation.Token);
            }
            catch (Exception ex)
            {
                Trace.Write("ShowBatchAsync", ex);
                Say(Loc.Get("Status_RecognizeFailed", ex.Message));
            }

            running = false;
            progressBar.Value = entries.Count;

            // A short list means the run was stopped: the runner records a failure rather than
            // dropping a source, so a complete run always comes back with one entry per source.
            progressText.Text = entries.Count < sources.Count
                ? Loc.Get("Batch_Cancelled", entries.Count, sources.Count)
                : Loc.Get("Batch_DoneAll", entries.Count);

            dialog.CloseButtonText = Loc.Get("Common_Close");

            if (entries.Count > 0)
            {
                dialog.PrimaryButtonText = Loc.Get("Batch_Save");
                dialog.SecondaryButtonText = Loc.Get("Batch_CopyAll");
                dialog.DefaultButton = ContentDialogButton.Primary;
                separate.Visibility = Visibility.Visible;
            }
        };

        _isOpen = true;
        try
        {
            await dialog.ShowAsync();
        }
        finally
        {
            _isOpen = false;
        }

        return entries;

        // The progress list: one row per source, with the reason underneath when it failed.
        static FrameworkElement Line(BatchEntry entry)
        {
            var group = new StackPanel();
            group.Children.Add(new TextBlock { Text = entry.Name, TextTrimming = TextTrimming.CharacterEllipsis });

            if (entry.Error is not null)
            {
                group.Children.Add(new TextBlock
                {
                    Text = Loc.Get("Batch_ItemFailed", entry.Error),
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 12,
                    Opacity = 0.7,
                });
            }

            return group;
        }

        void Say(string message)
        {
            note.Text = message;
            note.Visibility = Visibility.Visible;
        }

        void CopyAll()
        {
            try
            {
                var package = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
                package.SetText(BatchRunner.Merge(entries, markdown: false));
                Clipboard.SetContent(package);
                Say(Loc.Get("Status_TextCopied"));
            }
            catch (Exception ex)
            {
                Trace.Write("ShowBatchAsync copy", ex);
                Say(Loc.Get("Status_CopyFailed", ex.Message));
            }
        }

        async Task SaveAsync()
        {
            try
            {
                if (separate.IsChecked == true)
                {
                    await SaveEachAsync();
                }
                else
                {
                    await SaveOneAsync();
                }
            }
            catch (Exception ex)
            {
                Trace.Write("ShowBatchAsync save", ex);
                Say(Loc.Get("Status_TextSaveFailed", ex.Message));
            }
        }

        async Task SaveOneAsync()
        {
            var picker = new FileSavePicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeChoices.Add(Loc.Get("FileType_Text"), new[] { ".txt" });
            picker.FileTypeChoices.Add(Loc.Get("FileType_Markdown"), new[] { ".md" });
            picker.SuggestedFileName = "Glyfo";
            InitializeWithWindow.Initialize(picker, hwnd);

            var target = await picker.PickSaveFileAsync();
            if (target is null)
            {
                return;
            }

            var markdown = Path.GetExtension(target.Name).Equals(".md", StringComparison.OrdinalIgnoreCase);
            await TextFile.WriteAsync(target, BatchRunner.Merge(entries, markdown));
            Say(Loc.Get("Status_TextSaved", target.Name));
        }

        async Task SaveEachAsync()
        {
            var picker = new FolderPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };

            // A FolderPicker with no filter returns nothing at all on some builds, and the entry it
            // takes is never read — this is the documented incantation, not a real filter.
            picker.FileTypeFilter.Add("*");
            InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();
            if (folder is null)
            {
                return;
            }

            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var written = 0;

            for (var index = 0; index < entries.Count; index++)
            {
                var entry = entries[index];
                var name = TextFile.SafeBaseName(entry.Name, index + 1);

                // Two pages of different PDFs, or a .png and a .jpg of the same name, would
                // otherwise overwrite each other and leave fewer files than items.
                var candidate = name;
                var suffix = 2;
                while (!used.Add($"{candidate}.txt"))
                {
                    candidate = $"{name} ({suffix++})";
                }

                var file = await folder.CreateFileAsync($"{candidate}.txt", CreationCollisionOption.ReplaceExisting);
                await TextFile.WriteAsync(file, BatchRunner.Merge(new[] { entry }, markdown: false));
                written++;
            }

            Say(Loc.Get("Batch_SavedFolder", written, folder.Name));
        }
    }

    /// <summary>
    /// One dialog for both, because About offers a way into the notes and swapping its content is
    /// the only way to get there: a second <see cref="ContentDialog"/> cannot open over the first.
    /// </summary>
    private static async Task ShowAsync(XamlRoot root, IntPtr hwnd, IReadOnlyList<Release>? whatsNew)
    {
        if (_isOpen)
        {
            return;
        }

        var dialog = new ContentDialog
        {
            // Without this the dialog has no tree to attach to and the show throws. A desktop
            // window can host several XAML islands, so WinUI will not guess which one.
            XamlRoot = root,

            // Dialog content lives in a popup, outside RootGrid, so it inherits nothing from the
            // window — including the mirroring that Arabic, Hebrew and Persian need.
            FlowDirection = Loc.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
            CloseButtonText = Loc.Get("Common_Close"),
            DefaultButton = ContentDialogButton.Close,
        };

        // Set by the About page's rating link, which has to close the dialog before the Store's own
        // sheet can take its place.
        var rateRequested = false;

        if (whatsNew is not null)
        {
            ShowNotes(whatsNew);
        }
        else
        {
            dialog.Title = Loc.Get("About_Title");
            dialog.Content = BuildAbout();
        }

        _isOpen = true;
        ContentDialogResult result;
        try
        {
            result = await dialog.ShowAsync();
        }
        finally
        {
            _isOpen = false;
        }

        if (rateRequested || result == ContentDialogResult.Primary)
        {
            await ProductInfo.RateAsync(hwnd);
        }

        void ShowNotes(IReadOnlyList<Release> releases)
        {
            dialog.Title = Loc.Get("News_Title");
            dialog.Content = BuildNotes(releases);

            // Having just read what was added is the second-best moment to be asked for a rating,
            // after a recognition that worked.
            dialog.PrimaryButtonText = Loc.Get("About_Rate");
        }

        FrameworkElement BuildAbout()
        {
            var heading = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 14 };
            heading.Children.Add(new FontIcon
            {
                Glyph = AppGlyph,
                FontSize = 32,
                VerticalAlignment = VerticalAlignment.Top,
            });

            var titles = new StackPanel { Spacing = 2 };
            titles.Children.Add(new TextBlock
            {
                Text = "Glyfo",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
            });
            titles.Children.Add(Caption(Loc.Get("About_Version", ProductInfo.Display)));
            heading.Children.Add(titles);

            var panel = new StackPanel { Spacing = 14, Width = 340 };
            panel.Children.Add(heading);
            panel.Children.Add(new TextBlock
            {
                Text = Loc.Get("About_Tagline"),
                TextWrapping = TextWrapping.Wrap,
            });

            var links = new StackPanel { Margin = new Thickness(-12, 0, -12, 0) };
            links.Children.Add(Action(Loc.Get("About_Rate"), () =>
            {
                rateRequested = true;
                dialog.Hide();
            }));
            links.Children.Add(Action(Loc.Get("About_WhatsNew"), () =>
            {
                // Everything ever released, not merely what is unread: reached from About this is
                // someone going looking, and an empty page would be the wrong answer.
                ShowNotes(Changelog.Releases);
            }));
            links.Children.Add(new HyperlinkButton
            {
                Content = Loc.Get("About_Source"),
                NavigateUri = ProductInfo.SourceUri,
            });
            links.Children.Add(new HyperlinkButton
            {
                Content = Loc.Get("About_Privacy"),
                NavigateUri = ProductInfo.PrivacyUri,
            });
            panel.Children.Add(links);

            return panel;
        }

        static FrameworkElement BuildNotes(IReadOnlyList<Release> releases)
        {
            var panel = new StackPanel { Spacing = 16, Width = 340 };

            foreach (var release in releases)
            {
                var group = new StackPanel { Spacing = 6 };
                group.Children.Add(new TextBlock
                {
                    Text = $"{release.Version.Major}.{release.Version.Minor}.{release.Version.Build}",
                    FontWeight = FontWeights.SemiBold,
                });

                foreach (var key in release.Keys)
                {
                    var line = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };

                    // A literal bullet rather than a list control: three items do not need one, and
                    // the glyph mirrors with the panel where a bulleted style would not.
                    line.Children.Add(new TextBlock { Text = "•" });
                    line.Children.Add(new TextBlock
                    {
                        Text = Loc.Get(key),
                        TextWrapping = TextWrapping.Wrap,
                        Width = 306,
                    });
                    group.Children.Add(line);
                }

                panel.Children.Add(group);
            }

            return new ScrollViewer
            {
                // Deep enough for two releases; a user who skipped several versions scrolls.
                MaxHeight = 380,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollMode = ScrollMode.Disabled,
                Content = panel,
            };
        }

        // Sized and dimmed by hand rather than through CaptionTextBlockStyle and
        // TextFillColorSecondaryBrush: those are theme resources, and looking one up from
        // Application.Current.Resources returns null as often as not depending on which dictionary
        // the current theme merged. Two literals beat a silent null Style.
        static TextBlock Caption(string text) => new()
        {
            Text = text,
            FontSize = 12,
            Opacity = 0.7,
        };

        static HyperlinkButton Action(string text, Action onClick)
        {
            var button = new HyperlinkButton { Content = text };
            button.Click += (_, _) => onClick();
            return button;
        }
    }
}
