using Glyfo.Services;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    /// <summary>Shows "About Glyfo", with the version and the four links worth having.</summary>
    public static Task ShowAboutAsync(XamlRoot root, IntPtr hwnd) =>
        ShowAsync(root, hwnd, whatsNew: null);

    /// <summary>Shows what changed in <paramref name="releases"/>, newest first.</summary>
    public static Task ShowWhatsNewAsync(XamlRoot root, IntPtr hwnd, IReadOnlyList<Release> releases) =>
        ShowAsync(root, hwnd, releases);

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
