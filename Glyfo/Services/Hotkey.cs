using System.Text;

namespace Glyfo.Services;

/// <summary>
/// A global shortcut, in the shape <c>RegisterHotKey</c> wants it: a modifier mask and a
/// virtual-key code.
/// </summary>
/// <remarks>
/// Carried and stored as the pair rather than as its display text. Text would have to be parsed
/// back before it could be registered, and a parser is one more thing that can disagree with the
/// formatter — over a keyboard layout, over a translated key name, over a value written by an
/// older build. The text is derived from the pair and never the other way round.
/// </remarks>
public readonly record struct Hotkey(uint Modifiers, uint Key)
{
    public const uint ModAlt = 0x0001;
    public const uint ModControl = 0x0002;
    public const uint ModShift = 0x0004;

    /// <summary>Alt+Z: what users coming from other capture tools reach for first.</summary>
    public static readonly Hotkey RegionDefault = new(ModAlt, 0x5A);

    /// <summary>Ctrl+Shift+G, for when Alt+Z is taken — NVIDIA's overlay owns it by default.</summary>
    public static readonly Hotkey RegionFallback = new(ModControl | ModShift, 0x47);

    /// <summary>Ctrl+Shift+R.</summary>
    public static readonly Hotkey FullDefault = new(ModControl | ModShift, 0x52);

    /// <summary>Nothing chosen, or nothing registered — the two cases the callers care about.</summary>
    public bool IsEmpty => Key == 0;

    /// <summary>
    /// Whether this is a combination the app is willing to take.
    /// </summary>
    /// <remarks>
    /// Ctrl or Alt is required because a bare letter, or Shift and a letter, is what the user types
    /// into the result box; registering one would take it away from every other application on the
    /// desktop for as long as Glyfo is running. The main key is limited to A–Z, 0–9 and F1–F12 so
    /// that <see cref="ToString"/> stays a four-line table instead of a question for the keyboard
    /// layout, whose answer differs per machine and per language.
    /// </remarks>
    public bool IsUsable => IsMainKey(Key) && (Modifiers & (ModControl | ModAlt)) != 0;

    public static bool IsMainKey(uint vk) =>
        (vk >= 0x41 && vk <= 0x5A) || (vk >= 0x30 && vk <= 0x39) || (vk >= 0x70 && vk <= 0x7B);

    /// <summary>
    /// Shift, Ctrl, Alt and the Windows keys, in both their generic and their sided forms.
    /// </summary>
    /// <remarks>
    /// The recorder sees these on the way to the real key and has to sit through them rather than
    /// treat them as an answer: every combination starts with one.
    /// </remarks>
    public static bool IsModifierKey(uint vk) =>
        vk is 0x10 or 0x11 or 0x12 or 0x5B or 0x5C or 0xA0 or 0xA1 or 0xA2 or 0xA3 or 0xA4 or 0xA5;

    /// <summary>
    /// The combination as it is shown everywhere: the capture tooltip, the capture menu, the tray
    /// menu, the settings row. Empty when nothing is registered.
    /// </summary>
    /// <remarks>
    /// Not translated, deliberately. All thirty-three string tables spell Ctrl+Shift+R the same
    /// way, because that is what the key caps say whatever the interface language is.
    /// </remarks>
    public override string ToString()
    {
        if (IsEmpty)
        {
            return string.Empty;
        }

        var text = new StringBuilder();
        if ((Modifiers & ModControl) != 0)
        {
            text.Append("Ctrl+");
        }

        if ((Modifiers & ModAlt) != 0)
        {
            text.Append("Alt+");
        }

        if ((Modifiers & ModShift) != 0)
        {
            text.Append("Shift+");
        }

        text.Append(KeyName(Key));
        return text.ToString();
    }

    private static string KeyName(uint vk) => vk switch
    {
        >= 0x41 and <= 0x5A => ((char)vk).ToString(),
        >= 0x30 and <= 0x39 => ((char)vk).ToString(),
        >= 0x70 and <= 0x7B => "F" + (vk - 0x6F),
        _ => "?",
    };
}
