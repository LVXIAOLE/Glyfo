using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Glyfo.Services;

/// <summary>The file's outermost shape, so that a later format change has a hinge to turn on.</summary>
internal sealed class HistoryFile
{
    public int Version { get; set; } = HistoryStore.FormatVersion;

    public List<HistoryItem> Items { get; set; } = new();
}

/// <summary>
/// Serializer for <see cref="HistoryFile"/>, generated at compile time rather than by reflection.
/// </summary>
/// <remarks>
/// Not a style preference. The app publishes with <c>PublishTrimmed</c>, and reflection-based
/// <c>JsonSerializer</c> overloads are exactly what the trimmer cannot follow: they build converters
/// from type metadata it has already removed. The failure would be a packaged Release build that
/// loses its history with a clean build log — the same shape of bug as the one
/// <c>ILLink.Descriptors.xml</c> exists to prevent.
/// </remarks>
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(HistoryFile))]
internal sealed partial class HistoryJsonContext : JsonSerializerContext
{
}

/// <summary>
/// Keeps the recognized-text history across restarts.
/// </summary>
/// <remarks>
/// Only the text is written, never the picture it came from. That is the whole privacy story of this
/// feature and the reason it can be on by default: what lands on disk is what the user already had
/// selected in the result box, in a file inside the app's own package data, and the switch that
/// stops it also deletes it.
///
/// Every operation is best-effort and never throws. History is a convenience; losing it must not be
/// able to take down a recognition, and a corrupt file must not be able to stop the app starting.
/// </remarks>
internal static class HistoryStore
{
    public const int FormatVersion = 1;

    /// <summary>
    /// How many entries are kept. Twenty was the in-memory limit and is far too few once the list
    /// survives restarts and can be searched; two hundred is a few weeks of ordinary use.
    /// </summary>
    public const int MaxItems = 200;

    /// <summary>
    /// Second ceiling, on characters rather than entries.
    /// </summary>
    /// <remarks>
    /// A batch job can put a whole document into one entry, so a count on its own does not bound the
    /// file: two hundred of those would be tens of megabytes. A million characters is roughly three
    /// megabytes of UTF-8 even in Chinese, and an unbounded file in a Store app is a bug waiting to
    /// be filed.
    /// </remarks>
    private const int MaxTotalChars = 1_000_000;

    private const string FileName = "history.json";

    private static readonly object Gate = new();

    /// <summary>The file's path, or null when the app runs without package identity.</summary>
    private static string? Path
    {
        get
        {
            try
            {
                return System.IO.Path.Combine(
                    Windows.Storage.ApplicationData.Current.LocalFolder.Path, FileName);
            }
            catch (Exception)
            {
                // Unpackaged run. History lives for this session only, like the settings do.
                return null;
            }
        }
    }

    /// <summary>Reads the stored history, newest first. Returns nothing at all on any failure.</summary>
    public static IReadOnlyList<HistoryItem> Load()
    {
        try
        {
            if (Path is not { } path || !File.Exists(path))
            {
                return Array.Empty<HistoryItem>();
            }

            string json;
            lock (Gate)
            {
                json = File.ReadAllText(path);
            }

            var file = JsonSerializer.Deserialize(json, HistoryJsonContext.Default.HistoryFile);

            // A file from a future version is not readable here, and guessing at it would be worse
            // than starting over: the entries are a convenience, not the user's document.
            if (file is null || file.Version != FormatVersion)
            {
                return Array.Empty<HistoryItem>();
            }

            return file.Items;
        }
        catch (Exception ex)
        {
            Trace.Write("HistoryStore.Load", ex);
            return Array.Empty<HistoryItem>();
        }
    }

    /// <summary>
    /// Writes the list off the UI thread, after trimming it on the caller's.
    /// </summary>
    /// <remarks>
    /// The trim happens here, synchronously, because <paramref name="items"/> is owned by the UI
    /// thread and must not be walked from a background one. Only the immutable snapshot crosses.
    /// </remarks>
    public static void SaveInBackground(IReadOnlyList<HistoryItem> items)
    {
        var snapshot = Trim(items);
        _ = Task.Run(() => Write(snapshot));
    }

    /// <summary>Removes the file. Called when the user clears the list or turns the setting off.</summary>
    public static void Delete()
    {
        try
        {
            if (Path is { } path)
            {
                lock (Gate)
                {
                    File.Delete(path);
                }
            }
        }
        catch (Exception ex)
        {
            Trace.Write("HistoryStore.Delete", ex);
        }
    }

    /// <summary>
    /// Cuts the list down to both ceilings, newest kept.
    /// </summary>
    /// <remarks>
    /// One pass rather than "serialize, measure, drop one, serialize again": with entries that can
    /// each be a whole document, that loop would re-encode megabytes several times over for a single
    /// recognition.
    /// </remarks>
    private static List<HistoryItem> Trim(IReadOnlyList<HistoryItem> items)
    {
        var kept = new List<HistoryItem>(Math.Min(items.Count, MaxItems));
        var chars = 0;

        foreach (var item in items)
        {
            if (kept.Count == MaxItems)
            {
                break;
            }

            chars += item.Text.Length;

            // Checked after adding the length, so a single entry larger than the whole budget is
            // still kept: it is the newest thing the user did, and dropping it would look like the
            // recognition failed.
            kept.Add(item);

            if (chars >= MaxTotalChars)
            {
                break;
            }
        }

        return kept;
    }

    private static void Write(List<HistoryItem> items)
    {
        try
        {
            if (Path is not { } path)
            {
                return;
            }

            var json = JsonSerializer.Serialize(
                new HistoryFile { Items = items }, HistoryJsonContext.Default.HistoryFile);

            lock (Gate)
            {
                // Through a temporary file so that a crash or a full disk mid-write leaves the
                // previous history intact rather than a half-written one that Load then discards.
                var temp = path + ".tmp";
                File.WriteAllText(temp, json);
                File.Move(temp, path, overwrite: true);
            }
        }
        catch (Exception ex)
        {
            Trace.Write("HistoryStore.Write", ex);
        }
    }
}
