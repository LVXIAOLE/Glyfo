using Microsoft.Windows.AI;
using Microsoft.Windows.AI.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstaOCR.Services;

public sealed record TranslationLanguage(string Tag, string DisplayName, string PromptName);

/// <summary>
/// On-device translation through Phi Silica. Windows AI ships no dedicated translator API, so this
/// prompts the local language model — no network call and no API key, but Copilot+ hardware only.
/// </summary>
public sealed class TranslationService : IDisposable
{
    /// <summary>Chunk size in characters. Long OCR dumps otherwise overflow the model context.</summary>
    private const int MaxChunkLength = 1200;

    private readonly LanguageModel _model;
    private bool _disposed;

    private TranslationService(LanguageModel model) => _model = model;

    public static IReadOnlyList<TranslationLanguage> Targets { get; } = new[]
    {
        new TranslationLanguage("zh-Hans", "中文（简体）", "Simplified Chinese"),
        new TranslationLanguage("zh-Hant", "中文（繁體）", "Traditional Chinese"),
        new TranslationLanguage("en", "English", "English"),
        new TranslationLanguage("ja", "日本語", "Japanese"),
        new TranslationLanguage("ko", "한국어", "Korean"),
        new TranslationLanguage("fr", "Français", "French"),
        new TranslationLanguage("de", "Deutsch", "German"),
        new TranslationLanguage("es", "Español", "Spanish"),
        new TranslationLanguage("ru", "Русский", "Russian"),
    };

    /// <summary>Returns null whenever local translation is not possible; the caller hides the button.</summary>
    public static async Task<TranslationService?> TryCreateAsync()
    {
        try
        {
            var state = LanguageModel.GetReadyState();

            if (state == AIFeatureReadyState.NotReady)
            {
                await LanguageModel.EnsureReadyAsync();
                state = LanguageModel.GetReadyState();
            }

            if (state != AIFeatureReadyState.Ready)
            {
                return null;
            }

            return new TranslationService(await LanguageModel.CreateAsync());
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string> TranslateAsync(string text, TranslationLanguage target)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(target);

        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var chunk in SplitIntoChunks(text))
        {
            var prompt =
                $"Translate the text between the markers into {target.PromptName}. " +
                "Output only the translation: no notes, no explanation, no markers, no quotes.\n" +
                $"<<<TEXT\n{chunk}\nTEXT>>>";

            var response = await _model.GenerateResponseAsync(prompt);

            if (response.Status != LanguageModelResponseStatus.Complete)
            {
                throw new InvalidOperationException(DescribeFailure(response.Status));
            }

            if (builder.Length > 0)
            {
                builder.Append("\n\n");
            }

            builder.Append(response.Text.Trim());
        }

        return builder.ToString();
    }

    private static string DescribeFailure(LanguageModelResponseStatus status) => status switch
    {
        LanguageModelResponseStatus.PromptLargerThanContext => Loc.Get("Err_TextTooLong"),
        LanguageModelResponseStatus.BlockedByPolicy or
        LanguageModelResponseStatus.PromptBlockedByContentModeration or
        LanguageModelResponseStatus.ResponseBlockedByContentModeration => Loc.Get("Err_ModelDeclined"),
        _ => Loc.Get("Err_TranslationFailed")
    };

    /// <summary>Splits on paragraph boundaries first, and only cuts mid-paragraph when forced to.</summary>
    private static IEnumerable<string> SplitIntoChunks(string text)
    {
        var chunk = new StringBuilder();

        foreach (var paragraph in text.Replace("\r\n", "\n").Split('\n'))
        {
            if (paragraph.Length > MaxChunkLength)
            {
                if (chunk.Length > 0)
                {
                    yield return chunk.ToString();
                    chunk.Clear();
                }

                for (var offset = 0; offset < paragraph.Length; offset += MaxChunkLength)
                {
                    yield return paragraph.Substring(offset, Math.Min(MaxChunkLength, paragraph.Length - offset));
                }

                continue;
            }

            if (chunk.Length + paragraph.Length + 1 > MaxChunkLength && chunk.Length > 0)
            {
                yield return chunk.ToString();
                chunk.Clear();
            }

            if (chunk.Length > 0)
            {
                chunk.Append('\n');
            }

            chunk.Append(paragraph);
        }

        if (chunk.Length > 0)
        {
            yield return chunk.ToString();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        (_model as IDisposable)?.Dispose();
    }
}
