namespace Glyfo.Services;

/// <summary>
/// Outcome of a single recognition pass.
/// <paramref name="MeanConfidence"/> is null when the engine does not report one — the built-in
/// <c>Windows.Media.Ocr</c> has no confidence data at all, only the Windows AI recognizer does.
/// <paramref name="RecognizerName"/> is the language pack that answered, in its own language, and
/// is null when the engine cannot say: the Windows AI recognizer detects the script internally and
/// never reports what it decided.
/// </summary>
/// <remarks>
/// Which recognizer ran only became worth showing once automatic mode started choosing one for
/// itself. When the user picks a language the answer is on screen already; when the machine picks,
/// this is the only place the choice is visible, and a wrong pick otherwise looks like a bad read.
/// </remarks>
public sealed record OcrResult(string Text, float? MeanConfidence, string EngineName, string? RecognizerName = null);

/// <summary>A language the current engine can actually recognize right now.</summary>
/// <param name="Tag">BCP-47 tag passed back to the engine, or an empty string for automatic detection.</param>
public sealed record OcrLanguage(string Tag, string DisplayName);
