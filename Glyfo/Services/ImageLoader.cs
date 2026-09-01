using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Glyfo.Services;

/// <summary>
/// Single decode path for the whole app. OCR and barcode scanning share the resulting bitmap
/// instead of each decoding the file again.
/// </summary>
public static class ImageLoader
{
    /// <summary>
    /// Windows.Media.Ocr silently returns nothing for very small images. Screenshot crops of a
    /// single word land here often enough that it is worth upscaling instead of failing.
    /// </summary>
    private const int MinDimension = 64;

    /// <summary>
    /// Word height, in pixels, that the recognizers are comfortable with. Body text in a
    /// 100%-scaled screenshot is roughly half of this, which is where most misreads come from.
    /// </summary>
    private const double TargetWordHeight = 28;

    /// <summary>Below this measured word height, a second pass at a larger size pays for itself.</summary>
    private const double RescaleThreshold = 20;

    private const double MaxRescale = 4.0;

    /// <summary>A smaller enlargement than this does not change the outcome; skip the extra pass.</summary>
    private const double MinWorthwhileRescale = 1.25;

    /// <summary>Ceiling on the enlarged image, so a big low-DPI scan cannot blow up memory.</summary>
    private const long MaxRescaledPixels = 30_000_000;

    /// <summary>Longest side both engines accept. 10000 on current Windows builds.</summary>
    public static int MaxDimension => (int)Windows.Media.Ocr.OcrEngine.MaxImageDimension;

    public static async Task<SoftwareBitmap> LoadAsync(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The image file no longer exists.", path);
        }

        using var stream = await FileRandomAccessStream.OpenAsync(path, FileAccessMode.Read);
        var decoder = await BitmapDecoder.CreateAsync(stream);

        var (width, height) = FitToEngineLimits((int)decoder.PixelWidth, (int)decoder.PixelHeight);

        var transform = new BitmapTransform
        {
            ScaledWidth = (uint)width,
            ScaledHeight = (uint)height,
            InterpolationMode = BitmapInterpolationMode.Fant
        };

        return await decoder.GetSoftwareBitmapAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            transform,
            ExifOrientationMode.RespectExifOrientation,
            ColorManagementMode.DoNotColorManage);
    }

    /// <summary>
    /// Returns a resampled copy. Used by the OCR engine to enlarge images whose glyphs are too
    /// small for the recognizer; the caller owns the result.
    /// </summary>
    /// <remarks>
    /// SoftwareBitmap has no resize API, so this goes through an in-memory BMP round trip and
    /// lets the decoder do the resampling. BMP because it is lossless and has no entropy-coding
    /// cost — the encode is a memcpy in practice, and JPEG artifacts would defeat the purpose.
    /// </remarks>
    public static async Task<SoftwareBitmap> ScaleAsync(SoftwareBitmap source, double scale)
    {
        var width = (uint)Math.Clamp((int)Math.Round(source.PixelWidth * scale), 1, MaxDimension);
        var height = (uint)Math.Clamp((int)Math.Round(source.PixelHeight * scale), 1, MaxDimension);

        using var stream = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
        encoder.SetSoftwareBitmap(source);
        await encoder.FlushAsync();

        stream.Seek(0);
        var decoder = await BitmapDecoder.CreateAsync(stream);

        var transform = new BitmapTransform
        {
            ScaledWidth = width,
            ScaledHeight = height,
            InterpolationMode = BitmapInterpolationMode.Fant
        };

        return await decoder.GetSoftwareBitmapAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            transform,
            ExifOrientationMode.IgnoreExifOrientation,
            ColorManagementMode.DoNotColorManage);
    }

    /// <summary>
    /// How much to enlarge an image before giving a recognizer a second look at it, judged from the
    /// glyph heights the first pass measured. Returns 1 when a second pass is not worth running.
    /// </summary>
    /// <remarks>
    /// Both engines degrade sharply below roughly 20px of word height, which is exactly what a
    /// 100%-scaled screenshot of body text gives them. Upscaling before the first pass would be
    /// both wasteful (most photos and scans are already large enough) and blind, so the first pass
    /// measures the real glyph size and only then decides.
    /// </remarks>
    public static double SuggestUpscale(int pixelWidth, int pixelHeight, IReadOnlyList<double> wordHeights)
    {
        var usable = wordHeights.Where(height => height > 0).OrderBy(height => height).ToList();

        double scale;
        if (usable.Count == 0)
        {
            // Nothing recognized. Often that is a small crop rather than a blank image, and a
            // straight 2x is the cheapest thing that rescues it.
            scale = 2.0;
        }
        else
        {
            // Upper quartile, not the mean and not the median.
            //
            // Not the mean, because one oversized heading would mask body text that is too small.
            //
            // Not the median, because word boxes are tight around the ink and Latin words vary
            // enormously in how much of the em they fill: in one line of 22px English, "over"
            // measures 11px where "jumps" measures 21px. The median lands near the x-height, so a
            // median threshold rescaled text that was already large enough — 15 of 16 measured
            // English samples took a second pass they did not need. The words with both an
            // ascender and a descender do span the em, and the upper quartile finds them.
            //
            // Safe for CJK too: those glyphs all fill the em box, so the quartile sits where the
            // median did, and it is the more robust of the two against stray punctuation boxes
            // (a recognized "." measures 4-8px and drags a median down).
            var tall = usable[(usable.Count * 3) / 4];
            if (tall >= RescaleThreshold)
            {
                return 1;
            }

            scale = TargetWordHeight / tall;
        }

        scale = Math.Min(scale, MaxRescale);

        // Do not exceed what the engine accepts, or what is reasonable to hold in memory.
        var longest = Math.Max(pixelWidth, pixelHeight);
        if (longest > 0)
        {
            scale = Math.Min(scale, (double)MaxDimension / longest);
        }

        var pixels = (long)pixelWidth * pixelHeight;
        if (pixels > 0)
        {
            scale = Math.Min(scale, Math.Sqrt((double)MaxRescaledPixels / pixels));
        }

        // Not worth a whole extra pass for a marginal enlargement.
        return scale >= MinWorthwhileRescale ? scale : 1;
    }

    /// <summary>Clamps a size into the range both OCR engines can handle, preserving aspect ratio.</summary>
    private static (int Width, int Height) FitToEngineLimits(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new InvalidOperationException("The image has no pixels.");
        }

        var max = MaxDimension;

        var longest = Math.Max(width, height);
        if (longest > max)
        {
            var scale = (double)max / longest;
            width = Math.Max(1, (int)Math.Round(width * scale));
            height = Math.Max(1, (int)Math.Round(height * scale));
        }

        var shortest = Math.Min(width, height);
        if (shortest < MinDimension)
        {
            var scale = (double)MinDimension / shortest;

            // Do not let the upscale push the other side past the engine ceiling.
            var cap = (double)max / Math.Max(width, height);
            scale = Math.Min(scale, cap);

            if (scale > 1)
            {
                width = Math.Max(1, (int)Math.Round(width * scale));
                height = Math.Max(1, (int)Math.Round(height * scale));
            }
        }

        return (width, height);
    }
}
