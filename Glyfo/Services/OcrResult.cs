namespace Glyfo.Services;

/// <summary>
/// Outcome of a single recognition pass.
/// <paramref name="MeanConfidence"/> is null when the engine does not report one — the built-in
/// <c>Windows.Media.Ocr</c> has no confidence data at all, only the Windows AI recognizer does.
/// </summary>
public sealed record OcrResult(string Text, float? MeanConfidence, string EngineName);

/// <summary>A language the current engine can actually recognize right now.</summary>
/// <param name="Tag">BCP-47 tag passed back to the engine, or an empty string for automatic detection.</param>
public sealed record OcrLanguage(string Tag, string DisplayName);
