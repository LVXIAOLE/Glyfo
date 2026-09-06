using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Glyfo.Services;

/// <summary>Writing recognized text to disk, the same way from every place that offers it.</summary>
internal static class TextFile
{
    /// <summary>
    /// Writes <paramref name="text"/> as UTF-8, with a BOM for <c>.txt</c> and without one for
    /// everything else.
    /// </summary>
    /// <remarks>
    /// The BOM is not decoration. A loose <c>.txt</c> on Windows is opened by tools that guess the
    /// encoding from the first bytes, and without one they fall back to the system code page and
    /// mangle anything outside it. Markdown is the opposite case: it is read by toolchains that
    /// assume UTF-8 already, and a BOM there tends to surface inside the first heading.
    ///
    /// Line breaks are normalised to CRLF first. A <c>TextBox</c> hands back bare <c>\r</c>, which
    /// several Windows editors render as one very long line.
    /// </remarks>
    public static async Task WriteAsync(StorageFile file, string text)
    {
        var normalized = text.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "\r\n");

        var withBom = Path.GetExtension(file.Name).Equals(".txt", StringComparison.OrdinalIgnoreCase);
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: withBom);

        await FileIO.WriteBytesAsync(file, encoding.GetPreamble().Concat(encoding.GetBytes(normalized)).ToArray());
    }

    /// <summary>
    /// Turns a source name into something a file system will accept, without an extension.
    /// </summary>
    /// <remarks>
    /// Batch sources are named after files and PDF pages, so most already are legal; the ones that
    /// are not arrived from a share or a clipboard label. An empty result would produce a file
    /// called ".txt", so it falls back to a caller-supplied ordinal.
    /// </remarks>
    public static string SafeBaseName(string name, int ordinal)
    {
        var trimmed = Path.GetFileNameWithoutExtension(name);
        var builder = new StringBuilder(trimmed.Length);

        foreach (var character in trimmed)
        {
            builder.Append(Path.GetInvalidFileNameChars().Contains(character) ? '_' : character);
        }

        var result = builder.ToString().Trim().TrimEnd('.');
        return result.Length > 0 ? result : ordinal.ToString();
    }
}
