using System;
using System.Text.Json.Serialization;
using Glyfo.Services;

namespace Glyfo;

public sealed class HistoryItem
{
    public required DateTimeOffset Timestamp { get; init; }
    public required string Source { get; init; }
    public required string Text { get; init; }

    /// <summary>Null when the engine reports no confidence — the built-in Windows OCR never does.</summary>
    public float? Confidence { get; init; }

    /// <summary>
    /// The language pack that read this entry, in its own language. Null for the rows that never
    /// had one: barcodes, batch runs, the AI recognizer, and every entry saved before 1.3.0.
    /// </summary>
    public string? Recognizer { get; init; }

    /// <summary>
    /// The row's first line: when it was read, where it came from, and how sure the engine was.
    /// </summary>
    /// <remarks>
    /// The date appears only once the entry is no longer from today. While history lived and died
    /// with the process a bare clock time was unambiguous; now that it survives restarts, "09:14:22"
    /// on its own could be this morning or last week. Dating every row instead would put a redundant
    /// today's date on the entries the user is most likely looking at.
    ///
    /// Numeric, and read in local time rather than the offset the entry was recorded at: the month
    /// name would come out in the machine's culture, which is not necessarily the language the user
    /// picked in <see cref="Loc"/>, and a row saved in another time zone should still line up with
    /// the clock the user is reading it by.
    /// </remarks>
    [JsonIgnore]
    public string Header
    {
        get
        {
            var local = Timestamp.LocalDateTime;
            var when = local.Date == DateTime.Today
                ? local.ToString("HH:mm:ss")
                : local.ToString("MM-dd  HH:mm");

            // The recognizer rides with the source rather than at the end, because it says where
            // the text came from just as much as the file name does — and in automatic mode it is
            // the half the user did not choose.
            var from = Recognizer is null ? Source : $"{Source}  ·  {Recognizer}";

            return Confidence is null
                ? $"{when}  {from}"
                : $"{when}  {from}  {Math.Round(Confidence.Value * 100)}%";
        }
    }

    [JsonIgnore]
    public string Preview => string.IsNullOrWhiteSpace(Text)
        ? Loc.Get("History_EmptyPreview")
        : Text.Trim().Replace("\r\n", " ").Replace('\n', ' ');
}
