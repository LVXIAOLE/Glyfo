using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Glyfo.Services;

/// <summary>
/// What the app knows about itself: which version is running, and where it lives on the web.
/// </summary>
/// <remarks>
/// One place rather than a literal at each call site, because the version is compared against a
/// stored one to decide whether to show the release notes. Two readings that disagree — say a
/// manifest version against an assembly version — would either show the notes twice or never.
/// </remarks>
internal static class ProductInfo
{
    /// <summary>The product's Store id, from Partner Center. Part of the review deep link.</summary>
    private const string StoreProductId = "9PMC03HX113W";

    /// <summary>Opens the Store straight on this product's review sheet.</summary>
    private static readonly Uri ReviewUri = new($"ms-windows-store://review/?ProductId={StoreProductId}");

    public static readonly Uri SourceUri = new("https://github.com/LVXIAOLE/Glyfo");
    public static readonly Uri PrivacyUri = new("https://lvxiaole.github.io/glyfo-site/privacy.html");

    private static readonly Version Version = ReadVersion();

    /// <summary>The running version, as four parts.</summary>
    public static Version Current => Version;

    /// <summary>
    /// The version as a user should see it: three parts, because the fourth is always zero and a
    /// trailing ".0" reads like a mistake.
    /// </summary>
    public static string Display => $"{Version.Major}.{Version.Minor}.{Version.Build}";

    /// <summary>
    /// Asks for a rating, in the app if the platform allows it and in the Store if it does not.
    /// </summary>
    /// <remarks>
    /// <c>RequestRateAndReviewAppAsync</c> shows the Store's own rating sheet over the window, which
    /// is worth reaching for: sending someone out to the Store app loses most of them on the way.
    /// It only works for a copy actually installed from the Store, so a sideloaded build always
    /// takes the fallback — that is the expected path while developing, not a failure.
    /// </remarks>
    public static async Task RateAsync(IntPtr hwnd)
    {
        try
        {
            var context = Windows.Services.Store.StoreContext.GetDefault();

            // A desktop app has no implicit window for the sheet to sit over, and the call throws
            // without this rather than picking one.
            WinRT.Interop.InitializeWithWindow.Initialize(context, hwnd);

            var result = await context.RequestRateAndReviewAppAsync();
            Trace.Write($"RequestRateAndReviewAppAsync status={result.Status}");

            if (result.Status == Windows.Services.Store.StoreRateAndReviewStatus.Succeeded ||
                result.Status == Windows.Services.Store.StoreRateAndReviewStatus.CanceledByUser)
            {
                return;
            }
        }
        catch (Exception exception)
        {
            Trace.Write("RequestRateAndReviewAppAsync", exception);
        }

        try
        {
            await Windows.System.Launcher.LaunchUriAsync(ReviewUri);
        }
        catch (Exception exception)
        {
            // No Store app, or a policy blocking it. Nothing useful is left to try, and failing to
            // ask for a rating is not worth interrupting anyone over.
            Trace.Write("LaunchUriAsync(review)", exception);
        }
    }

    private static Version ReadVersion()
    {
        try
        {
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            return new Version(version.Major, version.Minor, version.Build, version.Revision);
        }
        catch (Exception)
        {
            // Unpackaged. The assembly version is close enough for a window that only displays it.
            return Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);
        }
    }
}
