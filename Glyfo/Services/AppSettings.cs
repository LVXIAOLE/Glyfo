using System;
using System.Collections.Generic;

namespace Glyfo.Services;

/// <summary>
/// User preferences, persisted per-user in the package's local settings.
/// </summary>
/// <remarks>
/// Backed by <c>ApplicationData.Current.LocalSettings</c>, which only exists when the app runs from
/// its package identity. Every access is guarded so that an unpackaged run (or a corrupted store)
/// degrades to in-memory defaults rather than taking the window down at startup.
/// </remarks>
public sealed class AppSettings
{
    private const string RepairVersionNumbersKey = "RepairVersionNumbers";
    private const string UiLanguageKey = "UiLanguage";
    private const string OcrLanguageKey = "OcrLanguage";
    private const string CloseToTrayKey = "CloseToTray";
    private const string HotkeyUsedKey = "HotkeyUsed";
    private const string StartupHintMutedKey = "StartupHintMuted";
    private const string TrayHintCountKey = "TrayHintCount";

    private readonly Dictionary<string, object> _fallback = new(StringComparer.Ordinal);
    private readonly Windows.Storage.ApplicationDataContainer? _store;

    public static AppSettings Current { get; } = new();

    private AppSettings()
    {
        try
        {
            _store = Windows.Storage.ApplicationData.Current.LocalSettings;
        }
        catch (Exception)
        {
            // No package identity — keep the settings for this run only.
            _store = null;
        }
    }

    /// <summary>
    /// Whether to rewrite "l" and "I" as the digit 1 inside version-like runs — "vl.6.5" to
    /// "v1.6.5". On by default: the CJK recognizers make this misread constantly, and the rule only
    /// fires next to a separator that has a digit on its other side.
    /// </summary>
    /// <remarks>
    /// Exposed as a setting because it is the one piece of post-processing that changes recognized
    /// content rather than just its layout. Anyone reading text where a bare "l" legitimately sits
    /// between digits — some serial numbers and licence keys do — needs to be able to turn it off.
    /// </remarks>
    public bool RepairVersionNumbers
    {
        get => GetBool(RepairVersionNumbersKey, true);
        set => SetBool(RepairVersionNumbersKey, value);
    }

    /// <summary>
    /// The language the interface is shown in, or an empty string to follow Windows.
    /// </summary>
    /// <remarks>
    /// Empty is the default and is a real value, not a missing one: it means "keep following the
    /// system", so a user who moves their machine to another language gets the app to follow.
    /// </remarks>
    public string UiLanguage
    {
        get => GetString(UiLanguageKey, string.Empty);
        set => SetString(UiLanguageKey, value);
    }

    /// <summary>
    /// The recognition language last chosen, so the choice survives a restart.
    /// </summary>
    /// <remarks>
    /// Stores the tag rather than the list index: the list is built from whatever language packs
    /// Windows has installed, and an index would silently point at a different language as soon as
    /// one is added or removed.
    /// </remarks>
    public string OcrLanguage
    {
        get => GetString(OcrLanguageKey, string.Empty);
        set => SetString(OcrLanguageKey, value);
    }

    /// <summary>
    /// Whether closing the window hides it to the notification area instead of ending the app.
    /// </summary>
    /// <remarks>
    /// On by default, because the capture shortcut is the point of the app and it dies with the
    /// process. Off is a real preference though: anyone who reaches for the app once a week would
    /// rather not have it sitting in the tray in between.
    /// </remarks>
    public bool CloseToTray
    {
        get => GetBool(CloseToTrayKey, true);
        set => SetBool(CloseToTrayKey, value);
    }

    /// <summary>
    /// Set the first time a capture shortcut is actually pressed, which retires the startup hint.
    /// </summary>
    /// <remarks>
    /// The hint exists to teach one fact. Counting launches would either stop before a distracted
    /// user noticed it or keep going long after they knew; using the fact itself as the condition
    /// ends it at exactly the right moment.
    /// </remarks>
    public bool HotkeyUsed
    {
        get => GetBool(HotkeyUsedKey, false);
        set => SetBool(HotkeyUsedKey, value);
    }

    /// <summary>Set when the user dismisses the startup hint by hand, which is final.</summary>
    public bool StartupHintMuted
    {
        get => GetBool(StartupHintMutedKey, false);
        set => SetBool(StartupHintMutedKey, value);
    }

    /// <summary>How many times the "still running in the tray" notice has been shown.</summary>
    public int TrayHintCount
    {
        get => GetInt(TrayHintCountKey, 0);
        set => SetInt(TrayHintCountKey, value);
    }

    private bool GetBool(string key, bool fallback)
    {
        try
        {
            if (_store is not null && _store.Values.TryGetValue(key, out var stored) && stored is bool value)
            {
                return value;
            }
        }
        catch (Exception)
        {
            // Fall through to the in-memory copy.
        }

        return _fallback.TryGetValue(key, out var cached) && cached is bool cachedValue ? cachedValue : fallback;
    }

    private void SetBool(string key, bool value)
    {
        _fallback[key] = value;

        try
        {
            if (_store is not null)
            {
                _store.Values[key] = value;
            }
        }
        catch (Exception)
        {
            // Preference is still honoured for this run; losing it on restart is not worth a crash.
        }
    }

    private int GetInt(string key, int fallback)
    {
        try
        {
            if (_store is not null && _store.Values.TryGetValue(key, out var stored) && stored is int value)
            {
                return value;
            }
        }
        catch (Exception)
        {
            // Fall through to the in-memory copy.
        }

        return _fallback.TryGetValue(key, out var cached) && cached is int cachedValue ? cachedValue : fallback;
    }

    private void SetInt(string key, int value)
    {
        _fallback[key] = value;

        try
        {
            if (_store is not null)
            {
                _store.Values[key] = value;
            }
        }
        catch (Exception)
        {
            // Preference is still honoured for this run; losing it on restart is not worth a crash.
        }
    }

    private string GetString(string key, string fallback)
    {
        try
        {
            if (_store is not null && _store.Values.TryGetValue(key, out var stored) && stored is string value)
            {
                return value;
            }
        }
        catch (Exception)
        {
            // Fall through to the in-memory copy.
        }

        return _fallback.TryGetValue(key, out var cached) && cached is string cachedValue ? cachedValue : fallback;
    }

    private void SetString(string key, string value)
    {
        _fallback[key] = value;

        try
        {
            if (_store is not null)
            {
                _store.Values[key] = value;
            }
        }
        catch (Exception)
        {
            // Preference is still honoured for this run; losing it on restart is not worth a crash.
        }
    }
}
