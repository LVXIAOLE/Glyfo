using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glyfo.Services;

/// <summary>One entry in the language picker: which engine to run, and in which language.</summary>
public sealed record OcrOption(string DisplayName, IOcrEngine Engine, string? LanguageTag)
{
    /// <summary>
    /// How this choice is remembered between launches.
    /// </summary>
    /// <remarks>
    /// The tag, not the list position. The list is built from the language packs Windows currently
    /// has installed, so a stored index would quietly come back pointing at a different language
    /// the first time one is added or removed. The AI recognizer carries no tag of its own — it
    /// detects the script itself — so it gets a name that no BCP-47 tag can collide with.
    /// </remarks>
    public string PreferenceKey => LanguageTag ?? "auto:ai";
}

/// <summary>
/// Owns both OCR backends. The built-in Windows engine is always there; the Copilot+ recognizer is
/// probed in the background and, when it shows up, becomes the preferred option without forcing
/// the user to give up explicit per-language recognition.
/// </summary>
public sealed class OcrEngineFactory : IDisposable
{
    private readonly WindowsMediaOcrEngine _builtIn = new();
    private WindowsAiOcrEngine? _ai;
    private bool _disposed;

    /// <summary>Raised on a background thread once the Copilot+ probe finishes successfully.</summary>
    public event EventHandler? OptionsChanged;

    public string StatusDescription
    {
        get
        {
            if (_ai is not null)
            {
                return Loc.Get("Engine_OnDevice", _ai.DisplayName);
            }

            var count = _builtIn.GetAvailableLanguages().Count;
            return count switch
            {
                0 => Loc.Get("Engine_NoLanguagePack"),
                1 => Loc.Get("Engine_OneLanguage"),
                _ => Loc.Get("Engine_ManyLanguages", count)
            };
        }
    }

    public IReadOnlyList<OcrOption> GetOptions()
    {
        var options = new List<OcrOption>();

        if (_ai is not null)
        {
            options.Add(new OcrOption(Loc.Get("Option_AutoAi"), _ai, null));
        }

        // Only worth offering when there is actually something to choose between: with one
        // recognizer installed it would just be a slower alias for that recognizer.
        if (WindowsMediaOcrEngine.GetAutoCandidates().Count >= 2)
        {
            options.Add(new OcrOption(
                Loc.Get("Option_AutoMulti"), _builtIn, WindowsMediaOcrEngine.AutoLanguageTag));
        }

        options.AddRange(_builtIn
            .GetAvailableLanguages()
            .Select(language => new OcrOption(language.DisplayName, _builtIn, language.Tag)));

        return options;
    }

    /// <summary>
    /// Which entry the picker should land on: what the user last chose, else a sensible default.
    /// </summary>
    /// <remarks>
    /// The default is emphatically not index 0. <see cref="GetOptions"/> sorts by display name, so
    /// on a machine with several language packs installed the first entry is whatever sorts first —
    /// "Arabic (Saudi Arabia)" on a Chinese user's PC, for instance. Recognizing Chinese with the
    /// Arabic recognizer produces garbage that reads as a bad OCR engine rather than a bad
    /// default, so follow the user's own language priority list instead.
    /// </remarks>
    public int GetDefaultOptionIndex(IReadOnlyList<OcrOption> options)
    {
        if (options.Count == 0)
        {
            return -1;
        }

        // An explicit past choice outranks every guess below it. Skipped when the language pack it
        // named is no longer installed, in which case the defaults take over again.
        var remembered = AppSettings.Current.OcrLanguage;
        if (remembered.Length > 0)
        {
            for (var i = 0; i < options.Count; i++)
            {
                if (string.Equals(options[i].PreferenceKey, remembered, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
        }

        // The AI recognizer auto-detects the script and is always first when present.
        if (_ai is not null)
        {
            return 0;
        }

        var preferred = WindowsMediaOcrEngine.GetPreferredLanguageTag();
        if (preferred is not null)
        {
            for (var i = 0; i < options.Count; i++)
            {
                if (string.Equals(options[i].LanguageTag, preferred, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
        }

        return 0;
    }

    public async Task ProbeAdvancedEngineAsync()
    {
        if (_disposed || _ai is not null)
        {
            return;
        }

        var engine = await WindowsAiOcrEngine.TryCreateAsync().ConfigureAwait(false);
        if (engine is null)
        {
            return;
        }

        if (_disposed)
        {
            engine.Dispose();
            return;
        }

        _ai = engine;
        OptionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _ai?.Dispose();
        _builtIn.Dispose();
    }
}
