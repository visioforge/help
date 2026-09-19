---
title: Live Subtitles and Speech-to-Text in C# .NET (Whisper)
description: Transcribe audio and video to text or live subtitles in C# with the Media Blocks SDK .NET using a local Whisper model and Silero VAD — no cloud, fully offline.
sidebar_label: Speech-to-Text (Whisper)
tags:
  - Media Blocks SDK
  - .NET
  - Whisper
  - Speech-to-Text
  - Subtitles
  - VAD
  - C#
primary_api_classes:
  - SpeechToTextBlock
  - SpeechToTextSettings
  - SileroVadSettings
  - SubtitleRenderer
  - SubtitleStyle
  - CaptionTimeline
  - SubtitleWriter
  - SubtitleFormat
  - SpeechRecognizedEventArgs
---

# Live Subtitles and Speech-to-Text in C# .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

`SpeechToTextBlock` adds **local, offline speech recognition** to any Media Blocks pipeline. It runs the
[Whisper](https://github.com/openai/whisper) ASR model (through [Whisper.net](https://github.com/sandrohanea/whisper.net),
the whisper.cpp / GGML backend) on the CPU or an NVIDIA GPU (CUDA), with optional
[Silero VAD](https://github.com/snakers4/silero-vad) voice-activity detection to split speech into clean
segments. Nothing is sent to the cloud.

The block sits **inline** in the audio path — audio passes through unchanged — and raises an
`OnSpeechRecognized` event with timed text segments. Use it to:

1. **Transcribe a media file** to text, SRT, or VTT (lossless, paced to the transcriber).
2. **Caption a live source** (microphone, capture card, RTSP camera) in real time.

```mermaid
graph LR;
    Source-->SpeechToTextBlock;
    SpeechToTextBlock-->AudioRendererBlock;
    SpeechToTextBlock-. OnSpeechRecognized .->App[Your app];
```

The block lives in the `VisioForge.Core.MediaBlocks.AI` namespace and ships in the **VisioForge AI Whisper**
add-on — NuGet package `VisioForge.DotNet.Core.AI.Whisper` (assembly `VisioForge.Core.AI.Whisper`),
built on `Whisper.net`. It needs the usual platform runtime package (for example
`VisioForge.CrossPlatform.Core.Windows.x64`) and works on Windows, Linux, and macOS.

## Models

The Whisper GGML weights and the Silero VAD model are **downloaded at runtime** — neither is shipped inside
the NuGet packages. Cache them once and reuse the local files:

- **Whisper GGML model** (`ggml-*.bin`): download with Whisper.net's `WhisperGgmlDownloader`, or fetch a
  `ggml-*.bin` from the whisper.cpp model repository.
- **Silero VAD model** (`silero_vad.onnx`, MIT): from the
  [silero-vad](https://github.com/snakers4/silero-vad) repository.

```csharp
using Whisper.net.Ggml;

// Download the "base" Whisper model to a local cache the first time, then reuse it.
var modelsDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "VisioForge", "models");
Directory.CreateDirectory(modelsDir);

var whisperModelPath = Path.Combine(modelsDir, "ggml-base.bin");
if (!File.Exists(whisperModelPath))
{
    using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.Base);
    using var fileStream = File.Create(whisperModelPath);
    await modelStream.CopyToAsync(fileStream);
}

// Silero VAD model — download silero_vad.onnx into the same cache (see "Models" above).
var sileroModelPath = Path.Combine(modelsDir, "silero_vad.onnx");
```

Pick the model size by the accuracy/speed/RAM trade-off you need. `SpeechToTextSettings.ModelSize` is
informational (it lets your app label or choose a download); the file actually loaded is always
`WhisperModelPath`.

| `WhisperModelSize` | Notes |
| --- | --- |
| `Tiny` / `TinyQuantized` | Fastest, lowest accuracy. |
| `Base` | Good real-time CPU default. |
| `Small` / `Medium` | Higher accuracy, heavier. |
| `LargeV3` / `LargeV3Turbo` | Highest accuracy; GPU recommended. |

## Two ways to run it

The sink that terminates the audio branch decides how the whole pipeline is paced. Pick the recipe
that matches the job — both use the same `SpeechToTextBlock`:

| Recipe | How it is wired | Use it for |
| --- | --- | --- |
| **Maximum speed** | The audio branch ends in a `NullRendererBlock` with `IsSync = false`, so no clock caps the run. | Offline transcription, SRT/VTT generation, batch jobs: finish a file as fast as Whisper can. |
| **Real-time playback** | An audible `AudioRendererBlock` paces the pipeline at 1x, and the transcriber runs on its own decoupled branch. | Watching a file or a live source with the captions appearing on the words. |

The sections below cover the maximum-speed recipe first, then the real-time one.

## Transcribe a media file

Transcription is lossless: the block paces the source to exactly the transcription throughput, so nothing is
dropped and the pipeline runs as fast as Whisper can keep up. Pair it with a non-synced sink so no real-time
clock caps the speed.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special; // NullRendererBlock
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AI;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var settings = new SpeechToTextSettings(whisperModelPath)
{
    Language = "auto",                          // ISO 639-1 code ("en", "es", "fr") or "auto"
    Provider = OnnxExecutionProvider.Auto,      // CUDA when available, else CPU
    EnableVad = true,                           // segment speech with Silero VAD
    OutputSrtPath = "subtitles.srt",            // optional side-car SRT (VTT via OutputVttPath)
};
settings.Vad.ModelPath = sileroModelPath;       // path to silero_vad.onnx

// Audio-only source from a file.
var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync("input.mp4", renderVideo: false, renderAudio: true));

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += (s, e) =>
{
    foreach (var seg in e.Segments)
    {
        if (!string.IsNullOrWhiteSpace(seg.Text))
        {
            Console.WriteLine($"[{seg.StartTime:hh\\:mm\\:ss}] {seg.Text.Trim()}");
        }
    }
};

// Non-synced null sink: no real-time clock, so the run is bounded only by transcription speed.
var sink = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = false };

pipeline.Connect(source.AudioOutput, stt.Input);
pipeline.Connect(stt.Output, sink.Input);

await pipeline.StartAsync();
```

Setting `OutputSrtPath` (or `OutputVttPath`) makes the block write a subtitle file directly as final segments
are recognized — no extra code needed.

## Caption a live source

The same block captions a live capture device — connect a microphone source instead of a file. The block
transcribes inline and never drops audio: it paces the source to Whisper. Whisper Base runs well above real
time, so a typical microphone is not throttled; if the model is slower than real time the source is held back
to transcription speed rather than losing samples.

```csharp
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sources;

// Pick the first system microphone.
var audioDevices = await SystemAudioSourceBlock.GetDevicesAsync();
var mic = new SystemAudioSourceBlock(audioDevices[0].CreateSourceSettings());

var settings = new SpeechToTextSettings(whisperModelPath)
{
    Language = "en",
    Provider = OnnxExecutionProvider.Auto,
    EnableVad = true,
};
settings.Vad.ModelPath = sileroModelPath;

var stt = new SpeechToTextBlock(settings);
stt.OnSpeechRecognized += (s, e) =>
{
    // Raised on the GStreamer streaming thread — marshal to the UI thread before touching UI.
    foreach (var seg in e.Segments)
    {
        Console.WriteLine(seg.Text);
    }
};

var audioRenderer = new AudioRendererBlock();

pipeline.Connect(mic.Output, stt.Input);          // audio passes through the block unchanged
pipeline.Connect(stt.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

The transcriber is inline here, so it paces the source: audible output stays smooth only while the model
keeps up. Put it on its own tee branch instead when the audio must be heard whatever the model does —
see "Real-time playback with live captions" below.

## Render live subtitles on video

`SpeechToTextBlock` is audio-only, so it does not draw captions itself. For on-screen subtitles, add
an `OverlayManagerBlock` to the video branch and connect `SpeechToTextBlock.OnSpeechRecognized` to
`SubtitleRenderer.OnSpeechRecognized`.

```csharp
using SkiaSharp;
using VisioForge.Core.AI.Whisper.Subtitles;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.MediaBlocks.VideoRendering;

var overlay = new OverlayManagerBlock();
var videoRenderer = new VideoRendererBlock(pipeline, videoView) { IsSync = false }; // maximum-speed recipe

var subtitleRenderer = new SubtitleRenderer(
    overlay,
    new SubtitleStyle
    {
        X = 40,
        Y = 380,
        FontName = "Arial",
        FontSize = 30,
        Color = SKColors.White,
        MinDisplay = TimeSpan.FromSeconds(1.5),
        MaxDisplay = TimeSpan.FromSeconds(6),
    });

stt.OnSpeechRecognized += subtitleRenderer.OnSpeechRecognized;

pipeline.Connect(source.VideoOutput, overlay.Input);
pipeline.Connect(overlay.Output, videoRenderer.Input);
```

`SubtitleRenderer` drives a single text overlay and gates the captions on the video clock: it buffers
the recognized segments into a `CaptionTimeline`, and the overlay picks the caption for each frame from
that frame's own timestamp. A caption therefore appears when the picture reaches the words — whether
recognition runs ahead of playback or behind it — and no timer is involved. `OnSpeechRecognized` is
raised on the GStreamer streaming thread, and the renderer only buffers there, so the overlay needs no
marshalling to the UI thread (a transcript list in your own UI still does). Dispose the renderer when
stopping the pipeline so the overlay is removed.

Each segment becomes its own caption, shown from its `StartTime` for the segment duration clamped to
`MinDisplay..MaxDisplay`. A caption that arrives after playback has already passed its window is not
dropped: it starts at the current position and queues behind any other late caption, which is what keeps
captions on screen in the maximum-speed recipe.

`IsSync = false` on the video renderer above belongs to that recipe — the picture runs as fast as the
transcriber. For real-time playback set `IsSync = true`, as in the next section.

| `SubtitleStyle` property | Default | Description |
| --- | --- | --- |
| `FontName` / `FontSize` | `Arial` / `32` | Text font. |
| `Color` | `White` | Text color. |
| `X` / `Y` | `50` / `50` | Overlay position in pixels. |
| `MinDisplay` / `MaxDisplay` | `1.5 s` / `6 s` | Minimum and maximum on-screen time for each caption. |

## Real-time playback with live captions

Playback and transcription want separate branches, so split the audio with a `TeeBlock`: one leg plays
through an audible `AudioRendererBlock` — that is what holds the pipeline at 1x — and the other feeds
Whisper through a buffer deep enough to absorb an inference burst. Both legs stay on the clock; the note
after the code says why the transcriber's must too.

```mermaid
graph LR;
    Source-- audio -->TeeBlock;
    TeeBlock-->AudioRendererBlock;
    TeeBlock-->SpeechToTextBlock;
    SpeechToTextBlock-->NullRendererBlock;
    Source-- video -->OverlayManagerBlock;
    OverlayManagerBlock-->VideoRendererBlock;
    SpeechToTextBlock-. OnSpeechRecognized .->SubtitleRenderer;
    SubtitleRenderer-. captions .->OverlayManagerBlock;
```

```csharp
using VisioForge.Core.AI.Whisper.Subtitles;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Special;      // TeeBlock, NullRendererBlock
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Special;          // TeeQueueSettings

// A 10-second buffer on the transcriber's leg absorbs a Whisper inference burst so it does not stall the
// renderer. Do NOT make it leaky: segment times come from a sample counter, not from buffer timestamps, so
// a dropped buffer shifts every later caption and SRT line earlier, for the rest of the file.
var queueSettings = new TeeQueueSettings
{
    MaxSizeBuffers = 0,
    MaxSizeBytes = 0,
    MaxSizeTime = (ulong)TimeSpan.FromSeconds(10).TotalMilliseconds * 1000000,   // 10 s, in nanoseconds
    Leaky = TeeQueueLeaky.No,
};

var audioTee = new TeeBlock(2, MediaBlockPadMediaType.Audio, queueSettings);
var audioRenderer = new AudioRendererBlock();                                        // audible: paces the run at 1x
var sttSink = new NullRendererBlock(MediaBlockPadMediaType.Audio);   // on the clock: see the note below

var overlay = new OverlayManagerBlock();
var videoRenderer = new VideoRendererBlock(pipeline, videoView) { IsSync = true };    // picture on the clock

var subtitleRenderer = new SubtitleRenderer(overlay, new SubtitleStyle { X = 40, Y = 380 });
stt.OnSpeechRecognized += subtitleRenderer.OnSpeechRecognized;

pipeline.Connect(source.AudioOutput, audioTee.Input);
pipeline.Connect(audioTee.Outputs[0], audioRenderer.Input);   // speakers
pipeline.Connect(audioTee.Outputs[1], stt.Input);             // transcriber
pipeline.Connect(stt.Output, sttSink.Input);

pipeline.Connect(source.VideoOutput, overlay.Input);
pipeline.Connect(overlay.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

`TeeBlock.Outputs[i]` are the src pads of the tee's own per-output queues, so each branch is already
decoupled — there is no separate queue block to add. Note that **every sink in this mode is clock-synced,
the transcriber's included**: a pipeline position query is answered by the furthest-advanced sink, so leaving
that one free-running would report the transcriber's front instead of the playback position - seconds ahead
of the audio, which is exactly what makes captions appear early.
 Ten seconds of buffering absorbs the bursts a
per-segment transcriber produces; a machine that cannot keep up with real time over a longer stretch fills
that queue and the playback stutters. That is the honest failure here, and the reason not to reach for
`TeeQueueLeaky.Downstream`: dropping buffers would keep the picture smooth while silently shifting every
later caption out of step with it.

### With MediaPlayerCoreX and VideoCaptureCoreX

In the X engines the block joins the engine's own audio chain through `Audio_Processing_AddBlock`, so it
sits **serially** between the decoder and the audio output — a real speaker output would underrun on a
long inference. Terminate the chain with a null renderer instead, and let `IsSync` pick the mode:

```csharp
// Synced null renderer: 1x pacing, silent. IsSync = false transcribes at maximum speed instead.
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = true };
player.Audio_Processing_AddBlock(stt);
```

There is no overlay block in that path, so drive a UI caption label from a `CaptionTimeline` of your own
and ask it what is due at the current playback position:

```csharp
var captions = new CaptionTimeline();
stt.OnSpeechRecognized += (s, e) => captions.Add(e);   // safe from the streaming thread

// A 200 ms UI timer is enough; the timeline decides which caption belongs to that position.
timer.Tick += async (s, e) => subtitleLabel.Text = captions.TextAt(await player.Position_GetAsync());
```

Call `CaptionTimeline.Clear()` before starting a new file and after a seek: positions are read as points
on the current stream's timeline. `SubtitleRenderer` exposes the same `Clear()`.

## Recognition results

`OnSpeechRecognized` is raised on the **GStreamer streaming thread** and carries a `SpeechRecognizedEventArgs`:

- `Segments` — a `SpeechSegment[]` (one event may carry several segments).
- `Timestamp` — the media time the segments belong to.

Each `SpeechSegment` has:

| Property | Description |
| --- | --- |
| `Text` | The recognized text. |
| `StartTime` / `EndTime` | Span on the media timeline (ready for SRT/VTT or an overlay schedule). |
| `Language` | Detected/used language (ISO 639-1), or `null`. |
| `Confidence` | Average token confidence (0..1), or 0 when the model does not report it. |
| `IsFinal` | Always `true` today (reserved for future interim hypotheses). |

## Key settings

| Property | Default | Description |
| --- | --- | --- |
| `WhisperModelPath` | — | Absolute path to the Whisper GGML model (`ggml-*.bin`). Required. |
| `Language` | `"auto"` | ISO 639-1 code or `"auto"` for detection. |
| `Task` | `Transcribe` | `Transcribe` (source language) or `Translate` (to English). |
| `Provider` | `Auto` | `CPU` or `CUDA` are meaningful (GGML has no DirectML); `Auto` picks CUDA when present, else CPU. |
| `DeviceId` | `0` | GPU device id when a GPU provider is used. |
| `Threads` | `0` | CPU threads; `0` lets Whisper.net choose. |
| `EnableVad` | `true` | Use Silero VAD to segment speech. Disable for fixed-window chunking. |
| `Vad` | (defaults) | `SileroVadSettings` — set `Vad.ModelPath` to `silero_vad.onnx`. |
| `FixedWindowSeconds` | `5` | Window length when `EnableVad = false` (clamped to 1–30 s). |
| `OutputSrtPath` | `null` | Optional side-car `.srt` written as segments finalize. |
| `OutputVttPath` | `null` | Optional side-car `.vtt` (WebVTT). |

`SileroVadSettings` exposes `SpeechThreshold` (0.5), `MinSilenceMs` (100), `MinSpeechMs` (250),
`SpeechPadMs` (30), and `MaxSpeechMs` (15000) to tune segmentation, plus its own `Provider`/`DeviceId`.

Call the static `SpeechToTextBlock.IsAvailable()` to verify the AI Whisper redistributable is present before
building a pipeline.

## Subtitle files

The easiest way to create side-car subtitles is to set `OutputSrtPath` or `OutputVttPath` in
`SpeechToTextSettings`. The block creates a `SubtitleWriter` internally and writes final segments as
they are recognized.

Use `SubtitleWriter` directly when you want to route recognized text yourself:

```csharp
using VisioForge.Core.AI.Whisper.Subtitles;

// Keep the writer alive for the whole pipeline run; dispose it when you stop the pipeline.
var writer = new SubtitleWriter("captions.vtt", SubtitleFormat.Vtt);

stt.OnSpeechRecognized += (sender, e) =>
{
    foreach (var segment in e.Segments)
    {
        writer.Add(segment);
    }
};
```

`SubtitleFormat.Srt` writes numbered SubRip cues with `HH:MM:SS,mmm` timestamps. `SubtitleFormat.Vtt`
writes a `WEBVTT` header and `HH:MM:SS.mmm` timestamps. `SubtitleWriter.Add()` ignores empty and
non-final segments. `FormatSrtTimestamp()` and `FormatVttTimestamp()` are public helpers for custom
writers.

## Demos

- **Live Subtitles** (Console) — [Live Subtitles](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/Live%20Subtitles) — lossless file transcription with progress reporting.
- **Live Subtitles Demo** (WPF) — [Live Subtitles Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Subtitles%20Demo) — live microphone/camera captioning with an on-screen overlay.
- **Live Subtitles MB** (MAUI) — [Live Subtitles MB](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/Live%20Subtitles%20MB).

Each demo carries a **Real-time playback** switch that toggles between the two recipes above.

## See also

- [AI in VisioForge .NET SDK](../../general/ai/index.md)
- [ElevenLabs Text-to-Speech and Voice Cloning](../ElevenLabs/index.md)
