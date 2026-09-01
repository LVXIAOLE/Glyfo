using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using WinRT.Interop;

namespace Glyfo;

/// <summary>Selection in physical pixels, relative to the captured screenshot's top-left corner.</summary>
public sealed record RegionSelection(int X, int Y, int Width, int Height);

public sealed partial class RegionCaptureWindow : Window
{
    private readonly TaskCompletionSource<RegionSelection?> _tcs = new();
    private readonly string _backdropPath;
    private readonly RectInt32 _bounds;
    private AppWindow? _appWindow;
    private bool _isDragging;
    private Point _startPoint;

    /// <param name="backdropPath">Screenshot already taken by the caller, shown 1:1 behind the overlay.</param>
    /// <param name="bounds">Screen-absolute physical bounds the screenshot covers.</param>
    public RegionCaptureWindow(string backdropPath, RectInt32 bounds)
    {
        _backdropPath = backdropPath;
        _bounds = bounds;

        InitializeComponent();
        HintText.Text = Services.Loc.Get("Region_Hint");

        // Only the hint is mirrored. The selection rectangle is positioned by pointer coordinates
        // straight onto the canvas, so flipping the overlay itself would put it under the cursor's
        // mirror image instead of under the cursor.
        HintText.FlowDirection = Services.Loc.IsRightToLeft
            ? Microsoft.UI.Xaml.FlowDirection.RightToLeft
            : Microsoft.UI.Xaml.FlowDirection.LeftToRight;

        BackdropImage.Source = new BitmapImage(new Uri(_backdropPath, UriKind.Absolute));
        Closed += (_, _) => _tcs.TrySetResult(null);

        // Shape the window before it is ever shown. Doing this from Activated meant the overlay
        // was briefly presented at the default size, and any failure in there left an always-on-top
        // window covering the desktop at the wrong size with nothing painted in it.
        ConfigureOverlayWindow();
    }

    public Task<RegionSelection?> CaptureAsync()
    {
        Activate();
        RootGrid.Focus(FocusState.Programmatic);
        return _tcs.Task;
    }

    private void ConfigureOverlayWindow()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        try
        {
            // A borderless window spanning the whole virtual desktop, so the overlay also covers
            // secondary monitors (AppWindowPresenterKind.FullScreen only covers one).
            var presenter = OverlappedPresenter.Create();
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.IsAlwaysOnTop = true;
            presenter.SetBorderAndTitleBar(false, false);
            _appWindow.SetPresenter(presenter);
            _appWindow.MoveAndResize(_bounds);
        }
        catch (Exception)
        {
            // Borderless spanning is a nicety; fall back to single-monitor full screen.
            _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
    }

    /// <summary>
    /// Escape hatch. The overlay is full-screen, borderless and always-on-top, so if keyboard focus
    /// ever fails to land on RootGrid the user would have no way out of it.
    /// </summary>
    private void RootGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        _tcs.TrySetResult(null);
        Close();
    }

    private void RootGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _isDragging = true;
        _startPoint = e.GetCurrentPoint(RootGrid).Position;
        SelectionBorder.Visibility = Visibility.Visible;
        UpdateSelection(_startPoint, _startPoint);
        RootGrid.CapturePointer(e.Pointer);
    }

    private void RootGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        var current = e.GetCurrentPoint(RootGrid).Position;
        UpdateSelection(_startPoint, current);
    }

    private void RootGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        RootGrid.ReleasePointerCapture(e.Pointer);
        var current = e.GetCurrentPoint(RootGrid).Position;
        var rect = Normalize(_startPoint, current);

        _tcs.TrySetResult(rect.Width < 8 || rect.Height < 8 ? null : ToImagePixels(rect));
        Close();
    }

    private void RootGrid_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Escape)
        {
            _tcs.TrySetResult(null);
            Close();
        }
    }

    /// <summary>
    /// Converts a selection expressed in this window's DIPs into pixels of the backdrop screenshot.
    /// The window is sized to the screenshot, so the ratio is the only conversion needed.
    /// </summary>
    private RegionSelection ToImagePixels(Rect rect)
    {
        var scaleX = RootGrid.ActualWidth > 0 ? _bounds.Width / RootGrid.ActualWidth : 1.0;
        var scaleY = RootGrid.ActualHeight > 0 ? _bounds.Height / RootGrid.ActualHeight : 1.0;

        var x = (int)Math.Round(rect.X * scaleX);
        var y = (int)Math.Round(rect.Y * scaleY);
        var width = (int)Math.Round(rect.Width * scaleX);
        var height = (int)Math.Round(rect.Height * scaleY);

        x = Math.Clamp(x, 0, Math.Max(0, _bounds.Width - 1));
        y = Math.Clamp(y, 0, Math.Max(0, _bounds.Height - 1));
        width = Math.Clamp(width, 1, _bounds.Width - x);
        height = Math.Clamp(height, 1, _bounds.Height - y);

        return new RegionSelection(x, y, width, height);
    }

    private void UpdateSelection(Point start, Point current)
    {
        var rect = Normalize(start, current);
        Canvas.SetLeft(SelectionBorder, rect.X);
        Canvas.SetTop(SelectionBorder, rect.Y);
        SelectionBorder.Width = rect.Width;
        SelectionBorder.Height = rect.Height;
    }

    private static Rect Normalize(Point start, Point current)
    {
        var x = Math.Min(start.X, current.X);
        var y = Math.Min(start.Y, current.Y);
        var width = Math.Abs(current.X - start.X);
        var height = Math.Abs(current.Y - start.Y);
        return new Rect(x, y, width, height);
    }
}
