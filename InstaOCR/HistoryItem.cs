using System;
using InstaOCR.Services;

namespace InstaOCR;

public sealed class HistoryItem
{
    public required DateTimeOffset Timestamp { get; init; }
    public required string Source { get; init; }
    public required string Text { get; init; }

    /// <summary>Null when the engine reports no confidence — the built-in Windows OCR never does.</summary>
    public float? Confidence { get; init; }

    public string Header => Confidence is null
        ? $"{Timestamp:HH:mm:ss}  {Source}"
        : $"{Timestamp:HH:mm:ss}  {Source}  {Math.Round(Confidence.Value * 100)}%";

    public string Preview => string.IsNullOrWhiteSpace(Text)
        ? Loc.Get("History_EmptyPreview")
        : Text.Trim().Replace("\r\n", " ").Replace('\n', ' ');
}
