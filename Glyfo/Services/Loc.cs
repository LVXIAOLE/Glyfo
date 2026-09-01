using System;
using System.Collections.Generic;
using System.Globalization;

namespace Glyfo.Services;

/// <summary>A language the app's own interface can be shown in.</summary>
/// <param name="Tag">BCP-47 tag, or an empty string for "follow the system".</param>
/// <param name="DisplayName">
/// Always written in the language itself. A picker that labelled 日本語 as "Japanese" would be
/// unreadable to the very person looking for it.
/// </param>
public sealed record UiLanguage(string Tag, string DisplayName);

/// <summary>
/// The app's own interface strings, and the language they are shown in.
/// </summary>
/// <remarks>
/// Plain C# tables rather than <c>.resw</c> + <c>x:Uid</c>, for one reason: the language has to be
/// switchable while the app is running. XAML resolves <c>x:Uid</c> once at load time, so the
/// resource route would mean either restarting the app or rebuilding the window on every change.
/// A table lookup plus one <c>ApplyLanguage</c> pass over the named controls does it instantly,
/// costs nothing at startup, survives trimming, and works with or without package identity.
///
/// Missing keys fall back to English rather than throwing, so a table that falls behind degrades
/// one string at a time instead of taking the window down.
/// </remarks>
public static class Loc
{
    private const string FallbackTag = "en";

    private static readonly Dictionary<string, Dictionary<string, string>> Tables =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = Strings.En,
            ["zh-Hans"] = Strings.ZhHans,
            ["zh-Hant"] = Strings.ZhHant,
            ["ja"] = Strings.Ja,
            ["ko"] = Strings.Ko,
            ["fr"] = Strings.Fr,
            ["de"] = Strings.De,
            ["es"] = Strings.Es,
            ["ru"] = Strings.Ru,
            ["pt"] = Strings.Pt,
            ["it"] = Strings.It,
            ["nl"] = Strings.Nl,
            ["pl"] = Strings.Pl,
            ["tr"] = Strings.Tr,
            ["uk"] = Strings.Uk,
            ["cs"] = Strings.Cs,
            ["ro"] = Strings.Ro,
            ["hu"] = Strings.Hu,
            ["el"] = Strings.El,
            ["sv"] = Strings.Sv,
            ["da"] = Strings.Da,
            ["nb"] = Strings.Nb,
            ["fi"] = Strings.Fi,
            ["id"] = Strings.Id,
            ["ms"] = Strings.Ms,
            ["fil"] = Strings.Fil,
            ["vi"] = Strings.Vi,
            ["th"] = Strings.Th,
            ["hi"] = Strings.Hi,
            ["bn"] = Strings.Bn,
            ["ar"] = Strings.Ar,
            ["he"] = Strings.He,
            ["fa"] = Strings.Fa,
        };

    /// <summary>
    /// Windows tags whose primary subtag is not the one we file the table under.
    /// </summary>
    /// <remarks>
    /// "no" is the macrolanguage Norwegian, which Windows still hands out; "nn" is Nynorsk, close
    /// enough to Bokmål to be better than English. "tl" is Tagalog, which Filipino is built on.
    /// "in" and "iw" are the obsolete ISO codes for Indonesian and Hebrew — old, but still turn up
    /// in language lists carried across upgrades.
    /// </remarks>
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["no"] = "nb",
        ["nn"] = "nb",
        ["tl"] = "fil",
        ["in"] = "id",
        ["iw"] = "he",
    };

    /// <summary>Tags written right to left. The window's FlowDirection follows this.</summary>
    private static readonly HashSet<string> RightToLeftTags =
        new(StringComparer.OrdinalIgnoreCase) { "ar", "he", "fa" };

    private static Dictionary<string, string> _current = Strings.En;

    /// <summary>Raised after the language changes, so open windows can re-apply their strings.</summary>
    public static event EventHandler? Changed;

    /// <summary>The offered languages, each labelled in itself. Excludes the "follow system" entry.</summary>
    /// <remarks>
    /// Ordered the way Windows orders its own language list: alphabetically by endonym within the
    /// Latin script, then the other scripts. Sorting these by English name would scatter them.
    /// </remarks>
    public static IReadOnlyList<UiLanguage> Languages { get; } = new[]
    {
        new UiLanguage("id", "Bahasa Indonesia"),
        new UiLanguage("ms", "Bahasa Melayu"),
        new UiLanguage("cs", "Čeština"),
        new UiLanguage("da", "Dansk"),
        new UiLanguage("de", "Deutsch"),
        new UiLanguage("en", "English"),
        new UiLanguage("es", "Español"),
        new UiLanguage("fil", "Filipino"),
        new UiLanguage("fr", "Français"),
        new UiLanguage("it", "Italiano"),
        new UiLanguage("hu", "Magyar"),
        new UiLanguage("nl", "Nederlands"),
        new UiLanguage("nb", "Norsk bokmål"),
        new UiLanguage("pl", "Polski"),
        new UiLanguage("pt", "Português"),
        new UiLanguage("ro", "Română"),
        new UiLanguage("fi", "Suomi"),
        new UiLanguage("sv", "Svenska"),
        new UiLanguage("vi", "Tiếng Việt"),
        new UiLanguage("tr", "Türkçe"),
        new UiLanguage("el", "Ελληνικά"),
        new UiLanguage("ru", "Русский"),
        new UiLanguage("uk", "Українська"),
        new UiLanguage("he", "עברית"),
        new UiLanguage("ar", "العربية"),
        new UiLanguage("fa", "فارسی"),
        new UiLanguage("hi", "हिन्दी"),
        new UiLanguage("bn", "বাংলা"),
        new UiLanguage("th", "ไทย"),
        new UiLanguage("ko", "한국어"),
        new UiLanguage("ja", "日本語"),
        new UiLanguage("zh-Hans", "简体中文"),
        new UiLanguage("zh-Hant", "繁體中文"),
    };

    /// <summary>What the user picked: a tag from <see cref="Languages"/>, or "" to follow the system.</summary>
    public static string SelectedTag { get; private set; } = string.Empty;

    /// <summary>The language actually in use. Never empty.</summary>
    public static string CurrentTag { get; private set; } = FallbackTag;

    /// <summary>
    /// Whether the language in use runs right to left, so windows can mirror their layout.
    /// </summary>
    /// <remarks>
    /// Translating the strings without mirroring the layout would leave Arabic, Hebrew and Persian
    /// readers with the toolbar on the wrong side of every panel — worse than not translating.
    /// </remarks>
    public static bool IsRightToLeft => RightToLeftTags.Contains(CurrentTag);

    /// <summary>
    /// Resolves the startup language: the stored preference, else the first of the user's own
    /// Windows languages this app has a table for, else English.
    /// </summary>
    public static void Initialize()
    {
        Apply(AppSettings.Current.UiLanguage, persist: false);
    }

    /// <summary>Switches language. Pass an empty tag to go back to following the system.</summary>
    public static void Select(string tag)
    {
        if (string.Equals(tag, SelectedTag, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Apply(tag, persist: true);
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static string Get(string key) =>
        _current.TryGetValue(key, out var value) ? value
        : Strings.En.TryGetValue(key, out var english) ? english
        : key;

    public static string Get(string key, params object?[] args) =>
        string.Format(CultureInfo.CurrentCulture, Get(key), args);

    private static void Apply(string tag, bool persist)
    {
        SelectedTag = Tables.ContainsKey(tag) ? tag : string.Empty;
        CurrentTag = SelectedTag.Length > 0 ? SelectedTag : DetectSystemLanguage();
        _current = Tables.TryGetValue(CurrentTag, out var table) ? table : Strings.En;

        if (persist)
        {
            AppSettings.Current.UiLanguage = SelectedTag;
        }

        // Keeps the strings Windows itself hands us — recognizer language names, voice names, the
        // file picker's own chrome — in step with the app. Ignored when it is not available.
        try
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride =
                SelectedTag.Length > 0 ? SelectedTag : string.Empty;
        }
        catch (Exception)
        {
            // Unpackaged, or the tag is not one the system will accept. The app's own strings are
            // already switched, which is the part that matters.
        }
    }

    /// <summary>
    /// The best table for the user's Windows language list, in their own priority order.
    /// </summary>
    /// <remarks>
    /// Reads <c>GlobalizationPreferences.Languages</c> — the OS-level list, which is unaffected by
    /// <c>PrimaryLanguageOverride</c>. Reading the app's own language list here instead would make
    /// "follow the system" mean "follow whatever I was last set to".
    /// </remarks>
    private static string DetectSystemLanguage()
    {
        foreach (var tag in SystemLanguages())
        {
            var match = MatchTable(tag);
            if (match is not null)
            {
                return match;
            }
        }

        return FallbackTag;
    }

    private static IEnumerable<string> SystemLanguages()
    {
        IReadOnlyList<string>? preferences = null;
        try
        {
            preferences = Windows.System.UserProfile.GlobalizationPreferences.Languages;
        }
        catch (Exception)
        {
            // Falls through to the CLR's view below.
        }

        if (preferences is { Count: > 0 })
        {
            foreach (var tag in preferences)
            {
                yield return tag;
            }
        }

        yield return CultureInfo.CurrentUICulture.Name;
    }

    /// <summary>Maps one Windows language tag onto a table, or null when nothing fits.</summary>
    private static string? MatchTable(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return null;
        }

        if (Tables.ContainsKey(tag))
        {
            return tag;
        }

        // Chinese splits by script, not by region, and the region is what the tag usually carries.
        // "zh-CN" and "zh-SG" are Simplified; "zh-TW", "zh-HK" and "zh-MO" are Traditional.
        if (tag.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return tag.Contains("Hant", StringComparison.OrdinalIgnoreCase) ||
                   tag.Contains("TW", StringComparison.OrdinalIgnoreCase) ||
                   tag.Contains("HK", StringComparison.OrdinalIgnoreCase) ||
                   tag.Contains("MO", StringComparison.OrdinalIgnoreCase)
                ? "zh-Hant"
                : "zh-Hans";
        }

        var primary = tag.Split('-')[0];
        if (Tables.ContainsKey(primary))
        {
            return primary;
        }

        return Aliases.TryGetValue(primary, out var alias) ? alias : null;
    }
}
