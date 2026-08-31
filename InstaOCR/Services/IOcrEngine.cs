using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace InstaOCR.Services;

public interface IOcrEngine : IDisposable
{
    /// <summary>Name shown in the status bar, e.g. "Windows OCR".</summary>
    string DisplayName { get; }

    /// <summary>
    /// Languages the engine can run right now. A single entry with an empty
    /// <see cref="OcrLanguage.Tag"/> means the engine detects the language itself.
    /// </summary>
    IReadOnlyList<OcrLanguage> GetAvailableLanguages();

    /// <param name="bitmap">Bgra8 bitmap, already normalized by <see cref="ImageLoader"/>.</param>
    /// <param name="languageTag">A tag from <see cref="GetAvailableLanguages"/>, or null for the engine default.</param>
    Task<OcrResult> RecognizeAsync(SoftwareBitmap bitmap, string? languageTag);
}
