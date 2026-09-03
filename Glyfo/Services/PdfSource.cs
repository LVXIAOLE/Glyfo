using System;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Glyfo.Services;

/// <summary>
/// A PDF opened for reading, one page at a time.
/// </summary>
/// <remarks>
/// Built on <c>Windows.Data.Pdf</c>, which ships with Windows: no package to carry, nothing to
/// update, and — the part that matters for this app — no network. A PDF renderer is otherwise a
/// large dependency to take on for what is, from here, just another way of producing a picture.
///
/// Not disposable, deliberately. <c>PdfDocument</c> has nothing to close; only the individual pages
/// do, and <see cref="RenderAsync"/> closes each one before it returns.
/// </remarks>
internal sealed class PdfSource
{
    /// <summary>
    /// How much bigger than its natural size a page is drawn before it is read.
    /// </summary>
    /// <remarks>
    /// <c>PdfPage.Size</c> is in device-independent pixels, so this is a multiplier on 96 DPI —
    /// 2.5 puts a rendered page at about 240 DPI. Below roughly 200 the recognizers start losing
    /// footnotes and subscripts on ordinary body text, and above it the picture grows faster than
    /// the accuracy does.
    /// </remarks>
    private const double RenderScale = 2.5;

    /// <summary>Rendered width is clamped into this range whatever the page size says.</summary>
    /// <remarks>
    /// The floor catches slide-sized and cropped pages, which come out too small to read at their
    /// natural size. The ceiling is well under <see cref="ImageLoader.MaxDimension"/> so that a
    /// poster-sized page does not have to be shrunk straight back down before recognition.
    /// </remarks>
    private const double MinRenderWidth = 1000;
    private const double MaxRenderWidth = 4000;

    private readonly PdfDocument _document;

    private PdfSource(PdfDocument document, string name)
    {
        _document = document;
        Name = name;
    }

    /// <summary>The file name, for the label above the preview.</summary>
    public string Name { get; }

    public uint PageCount => _document.PageCount;

    /// <summary>
    /// Opens a PDF. Throws for anything that is not one, and for the encrypted ones — there is no
    /// password prompt, and a clear failure is better than a dialog this app has nowhere to put.
    /// </summary>
    public static async Task<PdfSource> OpenAsync(StorageFile file)
    {
        var document = await PdfDocument.LoadFromFileAsync(file);
        return new PdfSource(document, file.Name);
    }

    /// <summary>Draws one page, zero-based, as a PNG.</summary>
    public async Task<InMemoryRandomAccessStream> RenderAsync(uint index)
    {
        using var page = _document.GetPage(index);

        var stream = new InMemoryRandomAccessStream();
        var options = new PdfPageRenderOptions
        {
            DestinationWidth = (uint)Math.Clamp(page.Size.Width * RenderScale, MinRenderWidth, MaxRenderWidth),
        };

        // PNG is the default encoder, and the right one: the text on a page is exactly the kind of
        // hard-edged content JPEG smears.
        await page.RenderToStreamAsync(stream, options);
        stream.Seek(0);
        return stream;
    }
}
