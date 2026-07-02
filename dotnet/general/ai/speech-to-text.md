---
title: Speech-to-Text SDK for .NET — SpeechToTextBlock (Whisper)
description: Speech-to-text SDK for .NET — transcribe live or file audio on-device with Whisper ASR and Silero VAD, plus SRT/VTT and live subtitle helpers.
sidebar_label: Speech-to-Text
tags:
  - .NET
  - AI
  - Whisper
  - Speech-to-Text
  - Speech-to-Text SDK
  - Speech Recognition
  - Live Subtitles
  - Transcription
  - VideoCaptureCoreX
  - MediaPlayerCoreX
primary_api_classes:
  - SpeechToTextBlock
  - SpeechToTextSettings
  - SileroVadSettings
  - SpeechSegment
  - SubtitleWriter
  - SubtitleRenderer
  - SubtitleStyle
---

# Speech-to-Text and Live Subtitles — SpeechToTextBlock

`SpeechToTextBlock` is an audio-only Media Block from `VisioForge.DotNet.Core.AI.Whisper`. It taps the
audio stream, segments speech with Silero VAD, transcribes it with Whisper (Whisper.net / GGML), and
raises `OnSpeechRecognized`. Audio passes through unchanged. The block implements
`IAudioProcessingBlock`, so it can be inserted into a manual pipeline or registered directly on
`VideoCaptureCoreX`/`MediaPlayerCoreX`.

```csharp
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.Types.X.AI;
```

## Basic block setup

```csharp
var settings = new SpeechToTextSettings(whisperModelPath)
{
    Language = "auto",
    Task = SpeechToTextTask.Transcribe,
    EnableVad = true,
    EmitInterim = false,
};

settings.Vad.ModelPath = sileroVadModelPath;

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += (sender, e) =>
{
    foreach (var segment in e.Segments)
    {
        Console.WriteLine($"{segment.StartTime:c} - {segment.EndTime:c}: {segment.Text}");
    }
};
```

`SpeechToTextTask.Transcribe` keeps the source language. `SpeechToTextTask.Translate` translates
supported source speech to English text.

## Key settings

`SpeechToTextSettings(whisperModelPath)`. Unlike the vision AI settings, this type does **not**
derive from `OnnxInferenceSettings` — Whisper runs through Whisper.net (whisper.cpp / GGML), not ONNX
Runtime, so the ONNX-specific input-size/normalization knobs don't apply.

| Property | Default | Description |
| --- | --- | --- |
| `WhisperModelPath` | — | Absolute path to the Whisper GGML model file (`ggml-*.bin`). Required. |
| `ModelSize` | `WhisperModelSize.Base` | Informational label for the model variant at `WhisperModelPath` (see below). |
| `Language` | `"auto"` | ISO 639-1 code (`"en"`, `"es"`, `"fr"`, ...), or `"auto"` to let Whisper detect it. |
| `Task` | `Transcribe` | `Transcribe` (source language) or `Translate` (to English). |
| `Provider` | `Auto` | Only `CPU` and `CUDA` are meaningful for the GGML backend (no DirectML); `Auto` picks CUDA when present, else CPU. |
| `DeviceId` | `0` | Hardware device id when a GPU provider is selected. |
| `Threads` | `0` | CPU threads Whisper uses. `0` lets Whisper.net choose from the available processor count. |
| `EnableVad` | `true` | Segment speech with Silero VAD before transcription. When `false`, audio is transcribed in fixed windows, which is prone to hallucinating text during silence. |
| `Vad` | new `SileroVadSettings` | VAD settings used when `EnableVad` is `true`. |
| `FixedWindowSeconds` | `5` | Fixed transcription window length when `EnableVad` is `false`. Clamped to 1–30 s. |
| `EmitInterim` | `false` | Reserved for a future interim-hypothesis capability; currently has no effect — only final segments are emitted. |
| `OutputSrtPath` | `null` | Optional `.srt` side-car path the block writes as final segments are recognized. |
| `OutputVttPath` | `null` | Optional `.vtt` (WebVTT) side-car path the block writes as final segments are recognized. |

## VAD settings

When `EnableVad` is `true`, `SpeechToTextSettings.Vad` controls Silero speech segmentation. Silero
VAD is a tiny (~2 MB, MIT) ONNX model that classifies short audio windows as speech or non-speech,
used as a real-time pre-filter so the (much heavier) Whisper model only runs on actual speech.

```csharp
settings.Vad = new SileroVadSettings
{
    ModelPath = sileroVadModelPath,
    SpeechThreshold = 0.5f,
    MinSilenceMs = 100,
    MinSpeechMs = 250,
    SpeechPadMs = 30,
    MaxSpeechMs = 15000,
    Provider = OnnxExecutionProvider.CPU,
};
```

| Property | Default | Description |
| --- | --- | --- |
| `ModelPath` | — | Absolute path to `silero_vad.onnx`. |
| `SpeechThreshold` | `0.5` | Speech-probability threshold (0..1). Raise it in noisy environments to reduce false triggers. |
| `MinSilenceMs` | `100` | Minimum trailing silence, in ms, that ends a speech segment. |
| `MinSpeechMs` | `250` | Minimum speech-run duration, in ms, to be emitted (discards spurious blips). |
| `SpeechPadMs` | `30` | Onset padding, in ms, prepended to each detected segment. |
| `MaxSpeechMs` | `15000` | Maximum segment length, in ms, before the segmenter force-cuts an ongoing run. |
| `Provider` | `CPU` | Execution provider for the VAD session — the model is tiny (~1 ms/window on CPU), so GPU adds latency without benefit. |
| `DeviceId` | `0` | Hardware device id when a GPU provider is selected. |

The Whisper GGML weights and the Silero VAD model are downloaded at runtime; neither is shipped in
the SDK NuGet packages.

### Whisper model sizes

`WhisperModelSize` is informational — it names well-known Whisper GGML weight files so an application
can pick one to download. The file actually loaded is always `SpeechToTextSettings.WhisperModelPath`.

| Value | Approx. size | Notes |
| --- | --- | --- |
| `Tiny` | ~75 MB | Fastest, lowest accuracy. Good for real-time CPU transcription. |
| `Base` (default) | ~142 MB | A good real-time CPU default. |
| `Small` | ~466 MB | Noticeably more accurate; real-time with a GPU or a fast CPU. |
| `Medium` | ~1.5 GB | High accuracy; typically needs a GPU for real-time. |
| `LargeV3` | ~3 GB | Highest accuracy; GPU strongly recommended. |
| `LargeV3Turbo` | ~1.6 GB | Near-large accuracy at a fraction of the cost. |
| `TinyQuantized` | ~31 MB | `Tiny`, Q5_1-quantized. |
| `BaseQuantized` | ~57 MB | `Base`, Q5_1-quantized. |
| `SmallQuantized` | ~181 MB | `Small`, Q5_1-quantized. |
| `MediumQuantized` | ~514 MB | `Medium`, Q5_0-quantized. |
| `LargeV3TurboQuantized` | ~547 MB | `LargeV3Turbo`, Q5_0-quantized. Recommended quantized accuracy/speed balance. |

English-only (`*.en`) variants aren't enumerated; supply their path directly. Weights are
MIT-licensed.

## Recognized segments

Each `SpeechSegment` in `SpeechRecognizedEventArgs.Segments`:

| Property | Description |
| --- | --- |
| `Text` | The recognized text for the segment. |
| `StartTime` / `EndTime` | `TimeSpan`, relative to the start of the stream — ready to use for SRT/VTT or on-screen scheduling. |
| `IsFinal` | Reserved to distinguish interim vs. final hypotheses once interim results exist; segments are currently always final (`true`). |
| `Language` | Detected/used ISO 639-1 code, or `null` if unknown. |
| `Confidence` | Average token confidence (0..1), or `0` when the model doesn't report token probabilities. |

## Subtitle helpers

### SubtitleWriter — SRT/VTT files

`SubtitleWriter` writes recognized speech segments to a SubRip (`.srt`) or WebVTT (`.vtt`) side-car
file, appending one cue per final segment. It is thread-safe; interim (non-final) segments are
ignored.

```csharp
using VisioForge.Core.AI.Whisper.Subtitles;

using var writer = new SubtitleWriter("captions.srt", SubtitleFormat.Srt);
stt.OnSpeechRecognized += (sender, e) =>
{
    foreach (var segment in e.Segments)
    {
        writer.Add(segment);
    }
};
```

If you only need files, it's simpler to set `OutputSrtPath` and/or `OutputVttPath` on
`SpeechToTextSettings` — the block creates and drives its own `SubtitleWriter` instance(s) internally
as final segments are recognized, and disposes them for you.

### SubtitleRenderer — on-screen captions

`SubtitleRenderer` drives a single text overlay on an `OverlayManagerBlock`: it shows the latest
caption and auto-hides it after the segment's display duration.

```csharp
using SkiaSharp;
using VisioForge.Core.AI.Whisper.Subtitles;

var style = new SubtitleStyle
{
    FontName = "Arial",
    FontSize = 32,
    Color = SKColors.White,
    X = 50,
    Y = 50,
    MinDisplay = TimeSpan.FromSeconds(1.5),
    MaxDisplay = TimeSpan.FromSeconds(6),
};

var subtitleRenderer = new SubtitleRenderer(overlayManagerBlock, style);
stt.OnSpeechRecognized += subtitleRenderer.OnSpeechRecognized;

// ... later, when tearing down:
subtitleRenderer.Dispose(); // removes the overlay and stops the auto-hide timer
```

Wire `SubtitleRenderer.OnSpeechRecognized` directly as the block's event handler. It clamps the
on-screen time into `[MinDisplay, MaxDisplay]` based on the segment's duration and calls
`OverlayManagerBlock.Video_Overlay_Update` from whatever thread invokes it — marshal the call if your
UI framework requires it. `SubtitleStyle` defaults: `FontName = "Arial"`, `FontSize = 32`,
`Color = White`, `X = 50`, `Y = 50`, `MinDisplay = 1.5 s`, `MaxDisplay = 6 s`.

!!! note "No shipping demo yet"
    `SubtitleRenderer` exists in the SDK and is documented from its source, but no bundled demo
    currently uses it — the Live Subtitles X demos update a UI label directly from
    `OnSpeechRecognized` instead. Use `SubtitleWriter` or `OutputSrtPath`/`OutputVttPath` if you only
    need a subtitle file.

## Manual Media Blocks pipeline

Place `SpeechToTextBlock` in the audio chain before the audio renderer or output:

```csharp
var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

pipeline.Connect(audioSource.Output, stt.Input);
pipeline.Connect(stt.Output, audioRenderer.Input);
```

## VideoCaptureCoreX live microphone transcription

For capture, an audio source is required. If you want analysis without speaker monitoring or
recording, terminate the audio chain with a non-synced null renderer:

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

core.Audio_Processing_AddBlock(stt); // before StartAsync
await core.StartAsync();
```

`Audio_OutputBlock` builds and terminates the audio chain without enabling speaker playback or file
recording. The engine owns the assigned output block after start.

## MediaPlayerCoreX file transcription

For playback, `Audio_Play` must be `true` for the audio chain to be built. A non-synced null renderer
lets the source run without real-time speaker output:

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

player.Audio_Processing_AddBlock(stt); // before OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

See [Using AI blocks with VideoCaptureCoreX and MediaPlayerCoreX](x-engines.md) for the full
`Audio_Processing_*`/`Audio_OutputBlock` API and lifecycle rules.

## Threading, pacing, and lifetime

VAD and Whisper run **synchronously on the GStreamer streaming thread**: audio is segmented and
transcribed in place, and the trailing segment is flushed at end-of-stream. Nothing is dropped — the
whole input is transcribed losslessly, and the pipeline position tracks the transcription frontier
(useful for a progress bar). Because transcription runs inline, the source is paced to Whisper: if
Whisper is slower than real time, the upstream (including a live capture device) is throttled rather
than losing audio. In practice Whisper Base runs well above real time, so it is not the bottleneck for
a typical source.

`OnSpeechRecognized` is raised on that same streaming thread — never touch UI directly from the
handler; marshal to the UI dispatcher or main thread.

After a capture or playback session starts, the engine owns wired `SpeechToTextBlock` instances and
disposes them when the session stops. Create a new block for the next session.

## Use cases

- **Live captioning** — real-time subtitles for a live stream, webinar, or accessibility overlay.
- **Meeting and call transcription** — transcribe a microphone feed alongside `VideoCaptureCoreX`
  capture.
- **Media indexing and search** — batch-transcribe recorded video/audio files to make their content
  searchable.
- **Subtitle authoring** — generate `.srt`/`.vtt` side-car files from source video without a
  third-party transcription service.
- **Translation captions** — set `Task = SpeechToTextTask.Translate` to caption non-English speech in
  English.

## Troubleshooting

| Symptom | Likely cause | Fix |
| --- | --- | --- |
| No segments are ever recognized | `WhisperModelPath` invalid, or the audio chain isn't built | Confirm the GGML model file exists at that path; for capture/playback, confirm the audio chain is active (see `Audio_OutputBlock` below). |
| Whisper "hallucinates" text during silence | `EnableVad` is `false` | Enable VAD (`EnableVad = true`, the default) so Whisper only runs on detected speech, not fixed windows that may be silent. |
| Transcription lags behind live audio | Whisper is slower than real time on the current hardware/model size | Choose a smaller `WhisperModelSize`/model file, or use `Provider = CUDA` on a machine with an NVIDIA GPU. |
| Segments are cut off mid-sentence | `SileroVadSettings.MaxSpeechMs` force-cuts a long continuous utterance | This is a deliberate bound (default 15 s) so one in-flight transcription can't grow unbounded; raise `MaxSpeechMs` if your scenario needs longer uninterrupted segments and can tolerate the larger bound. |
| No audio reaches the block on `VideoCaptureCoreX`/`MediaPlayerCoreX` | Audio chain not built (missing `Audio_Source`/`Audio_Play`) | See [VideoCaptureCoreX live microphone transcription](#videocapturecorex-live-microphone-transcription) and [MediaPlayerCoreX file transcription](#mediaplayercorex-file-transcription) above. |
| `.srt`/`.vtt` file is empty | Segments never finalized, or wrong path | Confirm `OutputSrtPath`/`OutputVttPath` point to a writable path and that speech was actually detected; only final segments are written. |

## Frequently Asked Questions

### Does SpeechToTextBlock require an internet connection?

No — transcription runs fully on-device through Whisper.net/GGML; the model files are downloaded once
by your application (or bundled), not called per-request over the network.

### Which languages does it support?

Whisper is multilingual — set `Language` to an ISO 639-1 code, or leave it `"auto"` to let Whisper
detect the spoken language automatically.

### Can I translate speech to English captions instead of transcribing the source language?

Yes — set `SpeechToTextSettings.Task = SpeechToTextTask.Translate`.

### How do I get live on-screen subtitles instead of just an event?

Wire `SubtitleRenderer.OnSpeechRecognized` as the block's event handler against an
`OverlayManagerBlock` — see [SubtitleRenderer — on-screen captions](#subtitlerenderer-on-screen-captions).
If you only need SRT/VTT files, set `OutputSrtPath`/`OutputVttPath` instead.

### Which Whisper model size should I use?

Start with `Base` (the default) for real-time CPU transcription. Move to `Small`/`Medium`/`LargeV3` (or
their quantized variants) for higher accuracy if you have a GPU or can tolerate slower-than-real-time
processing; see the [Whisper model sizes](#whisper-model-sizes) table for the full trade-off.

## Demos

- **[Live Subtitles Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Subtitles%20Demo)** — WPF Media Blocks pipeline demo.
- **[Live Subtitles MB](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/Live%20Subtitles%20MB)** — the same Media Blocks demo for MAUI.
- **[Live Subtitles](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/Live%20Subtitles)** — headless console demo (downloads models on first run).

Dedicated `VideoCaptureCoreX`/`MediaPlayerCoreX` live-subtitles demos (`Capture Live Subtitles X`,
`Capture Live Subtitles X WPF`, `Player Live Subtitles X`, `Player Live Subtitles X WPF`) are in the
SDK's demo set and will be linked here once published to the public samples repository.
