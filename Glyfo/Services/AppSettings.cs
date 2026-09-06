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
    private const string LastSeenVersionKey = "LastSeenVersion";
    private const string RecognizeCountKey = "RecognizeCount";
    private const string RatingPromptDoneKey = "RatingPromptDone";
    private const string WatchClipboardKey = "WatchClipboard";
    private const string KeepHistoryKey = "KeepHistory";
    private const string WindowXKey = "WindowX";
    private const string WindowYKey = "WindowY";
    private const string WindowWidthKey = "WindowWidth";
    private const string WindowHeightKey = "WindowHeight";
    private const string WindowMaximizedKey = "WindowMaximized";
    private const string RegionHotkeyModsKey = "RegionHotkeyMods";
    private const string RegionHotkeyVkKey = "RegionHotkeyVk";
    private const string FullHotkeyModsKey = "FullHotkeyMods";
    private const string FullHotkeyVkKey = "FullHotkeyVk";

    /// <summary>
    /// Keys that already existed in 1.0.x. Their presence is what tells an upgrade apart from a
    /// first-ever install; see <see cref="HasEarlierState"/>.
    /// </summary>
    private static readonly string[] LegacyKeys =
    {
        RepairVersionNumbersKey, UiLanguageKey, OcrLanguageKey,
        CloseToTrayKey, HotkeyUsedKey, StartupHintMutedKey, TrayHintCountKey,
    };

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

    /// <summary>
    /// The version whose release notes have already been shown, empty until the first time.
    /// </summary>
    /// <remarks>
    /// Stored as the full four-part string so it can be parsed straight back into a
    /// <see cref="Version"/> and compared. Writing it is what closes the notes for good, so it is
    /// only written once they have actually been on screen.
    /// </remarks>
    public string LastSeenVersion
    {
        get => GetString(LastSeenVersionKey, string.Empty);
        set => SetString(LastSeenVersionKey, value);
    }

    /// <summary>How many images have been read successfully, ever. Paces the rating prompt.</summary>
    /// <remarks>
    /// Counting successes rather than launches: someone who has opened the app ten times without
    /// getting a usable result out of it has nothing to rate, and asking them would be asking for
    /// the wrong answer.
    /// </remarks>
    public int RecognizeCount
    {
        get => GetInt(RecognizeCountKey, 0);
        set => SetInt(RecognizeCountKey, value);
    }

    /// <summary>Set once the rating prompt has been shown, whatever the user did with it.</summary>
    /// <remarks>
    /// Deliberately not "did they rate": the Store will not say, and asking a second time is worse
    /// than never hearing back from someone who declined once.
    /// </remarks>
    public bool RatingPromptDone
    {
        get => GetBool(RatingPromptDoneKey, false);
        set => SetBool(RatingPromptDoneKey, value);
    }

    /// <summary>
    /// Whether a picture copied to the clipboard is read automatically.
    /// </summary>
    /// <remarks>
    /// Off by default, and that is not a matter of taste. An application that watches the clipboard
    /// is reading everything the user copies, and nobody should discover that by accident — least of
    /// all in a Store app whose whole claim is that pictures stay on the machine. The switch has to
    /// be turned on deliberately, and the text beside it says what it does and where the picture
    /// goes.
    /// </remarks>
    public bool WatchClipboard
    {
        get => GetBool(WatchClipboardKey, false);
        set => SetBool(WatchClipboardKey, value);
    }

    /// <summary>
    /// Whether the history list is written to disk so it survives restarts.
    /// </summary>
    /// <remarks>
    /// On by default, which is the opposite of <see cref="WatchClipboard"/> and for a reason worth
    /// stating. What gets written is only what the user has already read on screen and asked for —
    /// the text of their own recognitions, never the pictures — into a file in the app's own package
    /// data. Nothing is collected that was not already deliberately produced, and nothing leaves the
    /// machine.
    ///
    /// The switch is still real: turning it off deletes the file and empties the list on the spot
    /// rather than merely stopping new writes, because someone reaching for it is asking for what is
    /// already there to be gone, not for the leak to be capped.
    /// </remarks>
    public bool KeepHistory
    {
        get => GetBool(KeepHistoryKey, true);
        set => SetBool(KeepHistoryKey, value);
    }

    /// <summary>
    /// Where the window was when it was last closed, in raw pixels — <see cref="WindowWidth"/> is
    /// zero until something has been stored.
    /// </summary>
    /// <remarks>
    /// The rectangle is the restored one even when the window was maximized, so that un-maximizing
    /// on the next run lands on the size the user chose rather than on the size of their screen.
    /// </remarks>
    public int WindowX
    {
        get => GetInt(WindowXKey, 0);
        set => SetInt(WindowXKey, value);
    }

    public int WindowY
    {
        get => GetInt(WindowYKey, 0);
        set => SetInt(WindowYKey, value);
    }

    public int WindowWidth
    {
        get => GetInt(WindowWidthKey, 0);
        set => SetInt(WindowWidthKey, value);
    }

    public int WindowHeight
    {
        get => GetInt(WindowHeightKey, 0);
        set => SetInt(WindowHeightKey, value);
    }

    /// <summary>Whether the window was maximized when it was last closed.</summary>
    public bool WindowMaximized
    {
        get => GetBool(WindowMaximizedKey, false);
        set => SetBool(WindowMaximizedKey, value);
    }

    /// <summary>
    /// The shortcut chosen for region capture, empty when the user has never chosen one.
    /// </summary>
    /// <remarks>
    /// Empty rather than the built-in default, because "never chosen" has to stay distinguishable
    /// from "chose Alt+Z". The Alt+Z to Ctrl+Shift+G fallback is the right answer for a default
    /// nobody picked and the wrong one for a key somebody typed in on purpose: silently moving a
    /// user's own shortcut is worse than telling them it did not take.
    /// </remarks>
    public Hotkey RegionHotkey
    {
        get => new((uint)GetInt(RegionHotkeyModsKey, 0), (uint)GetInt(RegionHotkeyVkKey, 0));
        set
        {
            SetInt(RegionHotkeyModsKey, (int)value.Modifiers);
            SetInt(RegionHotkeyVkKey, (int)value.Key);
        }
    }

    /// <summary>
    /// The shortcut chosen for full-screen capture, empty when the user has never chosen one.
    /// </summary>
    public Hotkey FullHotkey
    {
        get => new((uint)GetInt(FullHotkeyModsKey, 0), (uint)GetInt(FullHotkeyVkKey, 0));
        set
        {
            SetInt(FullHotkeyModsKey, (int)value.Modifiers);
            SetInt(FullHotkeyVkKey, (int)value.Key);
        }
    }

    /// <summary>
    /// Whether this install has been used before the release-notes feature existed.
    /// </summary>
    /// <remarks>
    /// <see cref="LastSeenVersion"/> is empty in two very different situations: a fresh install,
    /// which has nothing new to be told about, and an upgrade from 1.0.x, which is exactly the
    /// audience the notes are for. Nothing distinguishes them except the settings 1.0.x already
    /// wrote, so those are what gets asked.
    /// </remarks>
    public bool HasEarlierState
    {
        get
        {
            try
            {
                if (_store is not null)
                {
                    foreach (var key in LegacyKeys)
                    {
                        if (_store.Values.ContainsKey(key))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Unreadable store. Treating it as a fresh install only costs one set of notes.
            }

            return false;
        }
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
