using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;

namespace Glyfo.Services;

/// <summary>Reads the recognized text aloud with the voices already installed on the system.</summary>
public sealed class SpeechService : IDisposable
{
    /// <summary>SynthesizeTextToStreamAsync rejects very long input; this keeps it comfortably under.</summary>
    private const int MaxCharacters = 10_000;

    private readonly SpeechSynthesizer _synthesizer = new();
    private readonly MediaPlayer _player = new();
    private bool _disposed;

    public SpeechService()
    {
        _player.AutoPlay = false;
        _player.MediaEnded += (_, _) => PlaybackEnded?.Invoke(this, EventArgs.Empty);
        _player.MediaFailed += (_, _) => PlaybackEnded?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Raised on a media-player thread — marshal to the UI queue before touching controls.</summary>
    public event EventHandler? PlaybackEnded;

    public static IReadOnlyList<VoiceInformation> GetVoices()
    {
        try
        {
            return SpeechSynthesizer.AllVoices
                .OrderBy(voice => voice.Language, StringComparer.OrdinalIgnoreCase)
                .ThenBy(voice => voice.DisplayName, StringComparer.CurrentCulture)
                .ToList();
        }
        catch (Exception)
        {
            return Array.Empty<VoiceInformation>();
        }
    }

    public static VoiceInformation? DefaultVoice
    {
        get
        {
            try
            {
                return SpeechSynthesizer.DefaultVoice;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public async Task SpeakAsync(string text, VoiceInformation? voice)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        if (text.Length > MaxCharacters)
        {
            text = text[..MaxCharacters];
        }

        if (voice is not null)
        {
            _synthesizer.Voice = voice;
        }

        var stream = await _synthesizer.SynthesizeTextToStreamAsync(text);
        _player.Source = MediaSource.CreateFromStream(stream, stream.ContentType);
        _player.Play();
    }

    public void Stop()
    {
        if (_disposed)
        {
            return;
        }

        _player.Pause();
        if (_player.PlaybackSession is not null)
        {
            _player.PlaybackSession.Position = TimeSpan.Zero;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _player.Dispose();
        _synthesizer.Dispose();
    }
}
