using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using ZXing;
using ZXing.Common;

namespace InstaOCR.Services;

public sealed record BarcodeHit(string Format, string Text);

/// <summary>QR / barcode decoding straight off the same Bgra8 bitmap the OCR pass uses.</summary>
public static class BarcodeService
{
    public static Task<IReadOnlyList<BarcodeHit>> DecodeAsync(SoftwareBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        var width = bitmap.PixelWidth;
        var height = bitmap.PixelHeight;
        var pixels = CopyBgraPixels(bitmap);

        return Task.Run<IReadOnlyList<BarcodeHit>>(() =>
        {
            var reader = new BarcodeReaderGeneric
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    TryInverted = true,
                    // Left unrestricted: users paste anything from a QR code to a product barcode.
                    PossibleFormats = null
                }
            };

            var results = reader.DecodeMultiple(pixels, width, height, RGBLuminanceSource.BitmapFormat.BGRA32);
            if (results is null || results.Length == 0)
            {
                return Array.Empty<BarcodeHit>();
            }

            var hits = new List<BarcodeHit>(results.Length);
            foreach (var result in results)
            {
                if (!string.IsNullOrEmpty(result?.Text))
                {
                    hits.Add(new BarcodeHit(result.BarcodeFormat.ToString(), result.Text));
                }
            }

            return hits;
        });
    }

    /// <summary>
    /// Repacks the bitmap into tightly packed BGRA rows. The source stride can be padded, and
    /// handing ZXing a padded buffer shifts every row and silently kills the decode.
    /// </summary>
    private static byte[] CopyBgraPixels(SoftwareBitmap bitmap)
    {
        using var bitmapBuffer = bitmap.LockBuffer(BitmapBufferAccessMode.Read);
        var plane = bitmapBuffer.GetPlaneDescription(0);

        var required = (uint)(plane.StartIndex + plane.Stride * bitmap.PixelHeight);
        var raw = new Windows.Storage.Streams.Buffer(required);
        bitmap.CopyToBuffer(raw);
        var source = raw.ToArray();

        var rowBytes = bitmap.PixelWidth * 4;
        if (plane.StartIndex == 0 && plane.Stride == rowBytes)
        {
            return source;
        }

        var packed = new byte[rowBytes * bitmap.PixelHeight];
        for (var row = 0; row < bitmap.PixelHeight; row++)
        {
            Array.Copy(source, plane.StartIndex + row * plane.Stride, packed, row * rowBytes, rowBytes);
        }

        return packed;
    }
}
