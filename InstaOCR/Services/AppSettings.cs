using System;
using System.Collections.Generic;

namespace InstaOCR.Services;

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
