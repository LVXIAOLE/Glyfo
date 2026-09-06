using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Glyfo.Services;

/// <summary>
/// The pixel work between a file on disk and the preview: turning a picture, and working out how
/// far it was turned to begin with.
/// </summary>
/// <remarks>
/// <para>
/// <c>System.Drawing</c> rather than a WinRT encoder, because the screen capture and the region
/// crop already use it — this adds no assembly and no second way of doing the same thing.
/// </para>
/// <para>
/// The destination path is passed in rather than invented here. Every temporary file the app makes
/// has to end up in one list so it can be deleted on exit, and the window is what owns that list;
/// letting this class name its own files would put the naming rule in two places and the cleanup
/// rule in neither.
/// </para>
/// </remarks>
internal static class ImageOps
{
    /// <summary>
    /// Writes <paramref name="sourcePath"/> turned clockwise by <paramref name="degrees"/> to
    /// <paramref name="destinationPath"/>, as a PNG.
    /// </summary>
    /// <remarks>
    /// The result is the bounding rectangle of the turned picture, with the corners it exposes
    /// filled white: a page is white, and grey or transparent corners are something the recognizer
    /// then has to decide about.
    /// </remarks>
    public static void Rotate(string sourcePath, double degrees, string destinationPath)
    {
        using var source = new Bitmap(sourcePath);

        if (QuarterTurn(degrees) is { } quarter)
        {
            // A quarter turn only moves pixels; it never mixes them. Taking this path means four
            // presses of "rotate right" land back on the original bit for bit, and it is why the
            // window always re-renders from the untouched file instead of turning the last result.
            source.RotateFlip(quarter);
            source.Save(destinationPath, ImageFormat.Png);
            return;
        }

        var radians = degrees * Math.PI / 180.0;
        var cos = Math.Abs(Math.Cos(radians));
        var sin = Math.Abs(Math.Sin(radians));
        var width = (int)Math.Ceiling(source.Width * cos + source.Height * sin);
        var height = (int)Math.Ceiling(source.Width * sin + source.Height * cos);

        using var target = new Bitmap(Math.Max(1, width), Math.Max(1, height));
        target.SetResolution(source.HorizontalResolution, source.VerticalResolution);

        using (var graphics = Graphics.FromImage(target))
        {
            graphics.Clear(Color.White);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Turn about the middle of the canvas, then bring the source's own middle under it.
            graphics.TranslateTransform(target.Width / 2f, target.Height / 2f);
            graphics.RotateTransform((float)degrees);
            graphics.TranslateTransform(-source.Width / 2f, -source.Height / 2f);
            graphics.DrawImage(source, 0, 0, source.Width, source.Height);
        }

        target.Save(destinationPath, ImageFormat.Png);
    }

    /// <summary>
    /// How far the text on the page leans, in degrees clockwise. Rotate by the negative of this to
    /// straighten it. Zero when there is nothing to correct, or nothing that looks like text.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Projection-profile method, which needs no dependency and no training data: rotate the dark
    /// pixels by a candidate angle, histogram them by row, and keep the angle whose histogram has
    /// the largest variance. At the right angle every line of text falls into a few rows and the
    /// gaps between lines are empty, so the histogram is a row of spikes; at the wrong angle the
    /// lines smear across each other and it flattens out.
    /// </para>
    /// <para>
    /// The picture is scaled to a thousand pixels wide first and the dark pixels are collected once
    /// into two flat arrays, so the search costs about fifty passes over a few hundred thousand
    /// points — tens of milliseconds — rather than fifty decodes of the original.
    /// </para>
    /// </remarks>
    public static double EstimateSkew(string sourcePath)
    {
        var (xs, ys, height) = CollectDarkPixels(sourcePath);
        if (xs.Length == 0)
        {
            return 0;
        }

        // Ten degrees each way. Past that it is not a crooked scan, it is a picture taken sideways,
        // and guessing at one would be a worse answer than admitting there is none.
        var best = Search(xs, ys, height, -10.0, 10.0, 0.5);
        best = Search(xs, ys, height, best - 0.5, best + 0.5, 0.1);

        // Below this the estimate is inside its own noise, and turning the image would resample it
        // for nothing.
        return Math.Abs(best) < 0.3 ? 0 : Math.Round(best, 1);
    }

    private static double Search(float[] xs, float[] ys, int height, double from, double to, double step)
    {
        var best = 0.0;
        var bestScore = double.NegativeInfinity;

        for (var angle = from; angle <= to + step / 2; angle += step)
        {
            var score = Score(xs, ys, height, angle);
            if (score > bestScore)
            {
                bestScore = score;
                best = angle;
            }
        }

        return best;
    }

    private static double Score(float[] xs, float[] ys, int height, double angle)
    {
        var radians = angle * Math.PI / 180.0;

        // The points are turned by minus the candidate: the candidate is how far the page leans,
        // so undoing it is what should line the rows up.
        var sin = -Math.Sin(radians);
        var cos = Math.Cos(radians);
        var offset = height / 2.0;

        var rows = new int[height + 1];
        for (var i = 0; i < xs.Length; i++)
        {
            var row = (int)(xs[i] * sin + ys[i] * cos + offset);
            if (row >= 0 && row < rows.Length)
            {
                rows[row]++;
            }
        }

        var mean = (double)xs.Length / rows.Length;
        var variance = 0.0;
        foreach (var count in rows)
        {
            var difference = count - mean;
            variance += difference * difference;
        }

        return variance;
    }

    /// <summary>
    /// The ink, as coordinates measured from the middle of the scaled-down picture.
    /// </summary>
    /// <remarks>
    /// Centred here rather than in the scoring loop, which runs fifty-odd times over the same
    /// points. The threshold is the average brightness: on a page that is mostly paper it lands
    /// just under the paper, which is exactly where it belongs.
    /// </remarks>
    private static (float[] Xs, float[] Ys, int Height) CollectDarkPixels(string sourcePath)
    {
        using var source = new Bitmap(sourcePath);

        const int maxWidth = 1000;
        var scale = source.Width > maxWidth ? (double)maxWidth / source.Width : 1.0;
        var width = Math.Max(1, (int)(source.Width * scale));
        var height = Math.Max(1, (int)(source.Height * scale));

        using var small = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(small))
        {
            // White, so that a picture with transparency is measured against a page and not against
            // black, which would make the whole background count as ink.
            graphics.Clear(Color.White);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(source, 0, 0, width, height);
        }

        byte[] bytes;
        int stride;
        var data = small.LockBits(
            new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            stride = Math.Abs(data.Stride);
            bytes = new byte[stride * height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
        }
        finally
        {
            small.UnlockBits(data);
        }

        var luma = new byte[width * height];
        long total = 0;
        for (var y = 0; y < height; y++)
        {
            var row = y * stride;
            for (var x = 0; x < width; x++)
            {
                var i = row + (x * 4);
                var value = (byte)(((bytes[i + 2] * 299) + (bytes[i + 1] * 587) + (bytes[i] * 114)) / 1000);
                luma[(y * width) + x] = value;
                total += value;
            }
        }

        var threshold = (int)(total / luma.Length);
        var dark = 0;
        foreach (var value in luma)
        {
            if (value < threshold)
            {
                dark++;
            }
        }

        // Too few and there is nothing to line up; too many and it is a photograph rather than a
        // page, where the brightest half of the pixels is not paper and the profile means nothing.
        if (dark < 200 || dark > luma.Length * 2 / 5)
        {
            return (Array.Empty<float>(), Array.Empty<float>(), height);
        }

        var xs = new float[dark];
        var ys = new float[dark];
        var centreX = width / 2f;
        var centreY = height / 2f;
        var next = 0;
        for (var y = 0; y < height && next < dark; y++)
        {
            for (var x = 0; x < width && next < dark; x++)
            {
                if (luma[(y * width) + x] < threshold)
                {
                    xs[next] = x - centreX;
                    ys[next] = y - centreY;
                    next++;
                }
            }
        }

        return (xs, ys, height);
    }

    /// <summary>The exact turn for a multiple of ninety degrees, or null for anything else.</summary>
    private static RotateFlipType? QuarterTurn(double degrees)
    {
        var normalized = ((degrees % 360) + 360) % 360;
        var quarters = Math.Round(normalized / 90);
        if (Math.Abs(normalized - (quarters * 90)) > 0.001)
        {
            return null;
        }

        return ((int)quarters % 4) switch
        {
            0 => RotateFlipType.RotateNoneFlipNone,
            1 => RotateFlipType.Rotate90FlipNone,
            2 => RotateFlipType.Rotate180FlipNone,
            _ => RotateFlipType.Rotate270FlipNone,
        };
    }
}
