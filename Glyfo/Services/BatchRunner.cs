using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace Glyfo.Services;

/// <summary>One thing to read: a file, or a page of one.</summary>
/// <remarks>
/// A factory rather than a bitmap, so a hundred pages are not all decoded and held at once. The
/// runner opens each one only when its turn comes and disposes it before moving on.
/// </remarks>
internal sealed record BatchSource(string Name, Func<Task<SoftwareBitmap>> OpenAsync);

/// <summary>What came back for one source. Exactly one of <c>Text</c> and <c>Error</c> is meaningful.</summary>
internal sealed record BatchEntry(string Name, string Text, string? Error);

/// <summary>
/// Reads a list of images one after another.
/// </summary>
/// <remarks>
/// Sequential on purpose. Both OCR engines are already using every core they can on a single
/// image, so running several at once buys nothing and multiplies peak memory by the degree of
/// parallelism — on a long PDF that is the difference between working and being killed.
/// </remarks>
internal static class BatchRunner
{
    /// <summary>
    /// Reads every source in order, reporting each result as it lands. Stops early when
    /// <paramref name="token"/> is signalled, returning what was finished by then.
    /// </summary>
    /// <remarks>
    /// A failure on one source is recorded and the run continues. A 58-page contract should not
    /// come back empty because page 12 is a scan the engine could not open, and the caller cannot
    /// retry a single page from here anyway.
    ///
    /// Cancelling returns rather than throwing, for the same reason: the pages that did finish are
    /// the whole point of letting someone stop a long run, and an exception would throw them away
    /// on the way out. A short list is how the caller tells the two endings apart.
    /// </remarks>
    public static async Task<List<BatchEntry>> RunAsync(
        IReadOnlyList<BatchSource> sources,
        IOcrEngine engine,
        string? languageTag,
        IProgress<BatchEntry>? progress,
        CancellationToken token)
    {
        var entries = new List<BatchEntry>(sources.Count);

        foreach (var source in sources)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            BatchEntry entry;
            try
            {
                using var bitmap = await source.OpenAsync();
                var result = await engine.RecognizeAsync(bitmap, languageTag);
                entry = new BatchEntry(source.Name, result.Text ?? string.Empty, null);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Trace.Write($"BatchRunner '{source.Name}'", ex);

                // WinRT failures often carry an empty Message — a corrupt PNG comes back as a bare
                // COMException — and "Could not be read:" trailing off into nothing tells the user
                // less than the exception's own name does.
                entry = new BatchEntry(
                    source.Name,
                    string.Empty,
                    string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message);
            }

            entries.Add(entry);
            progress?.Report(entry);
        }

        return entries;
    }

    /// <summary>
    /// Joins the results into one document, each preceded by the name it came from.
    /// </summary>
    /// <remarks>
    /// Failures get a line of their own rather than being dropped. A merged file that is silently
    /// missing a page is the worst outcome available here: it looks complete.
    ///
    /// CRLF throughout, matching what the save path writes, so the text handed to the clipboard and
    /// the text written to disk are the same string.
    /// </remarks>
    public static string Merge(IReadOnlyList<BatchEntry> entries, bool markdown)
    {
        var builder = new StringBuilder();

        foreach (var entry in entries)
        {
            if (builder.Length > 0)
            {
                builder.Append("\r\n");
            }

            builder.Append(markdown ? "## " : string.Empty).Append(entry.Name).Append("\r\n\r\n");

            var body = entry.Error is null
                ? Normalize(entry.Text)
                : Loc.Get("Batch_ItemFailed", entry.Error);

            builder.Append(body.Length == 0 ? Loc.Get("Batch_ItemEmpty") : body).Append("\r\n");
        }

        return builder.ToString();
    }

    /// <summary>Puts every flavour of line break onto CRLF.</summary>
    private static string Normalize(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "\r\n").TrimEnd();
}
