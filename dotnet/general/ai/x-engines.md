---
title: AI Blocks with VideoCaptureCoreX and MediaPlayerCoreX
description: Insert AI Media Blocks — OCR, object detection, face recognition, ANPR, background removal, speech-to-text — into VideoCaptureCoreX and MediaPlayerCoreX.
sidebar_label: X Engines
tags:
  - .NET
  - AI
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - Media Blocks
primary_api_classes:
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - IVideoProcessingBlock
  - IAudioProcessingBlock
  - NullRendererBlock
---

# AI Blocks with VideoCaptureCoreX and MediaPlayerCoreX

`VideoCaptureCoreX` and `MediaPlayerCoreX` can host user-supplied Media Blocks inside their
high-level pipelines. Use this path when you want the convenience of the X engines and only need to
add AI processing to the video or audio chain — no manual `MediaBlocksPipeline` topology required.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.AI;
```

## Public API

Both engines expose the same processing-block API:

| API | Purpose |
| --- | --- |
| `Video_Processing_AddBlock(IVideoProcessingBlock block)` | Add a video processing block for the next session. |
| `Video_Processing_RemoveBlock(IVideoProcessingBlock block)` | Remove a registered video block before the session starts. |
| `Video_Processing_Clear()` | Clear registered video blocks before the session starts. |
| `Video_Processing_Blocks` | Snapshot of registered video blocks. |
| `Audio_Processing_AddBlock(IAudioProcessingBlock block)` | Add an audio processing block for the next session. |
| `Audio_Processing_RemoveBlock(IAudioProcessingBlock block)` | Remove a registered audio block before the session starts. |
| `Audio_Processing_Clear()` | Clear registered audio blocks before the session starts. |
| `Audio_Processing_Blocks` | Snapshot of registered audio blocks. |
| `Audio_OutputBlock` | Replace the default audio sink with a custom `MediaBlock`, commonly a non-synced `NullRendererBlock` for speech-to-text. |

Every video AI block (`OcrBlock`, `YOLOObjectDetectorBlock`, `ObjectAnalyticsBlock`,
`FaceRecognitionBlock`, `LicensePlateRecognizerBlock`, `BackgroundRemovalBlock`,
`OnnxInferenceBlock`) implements `IVideoProcessingBlock`. `SpeechToTextBlock` implements
`IAudioProcessingBlock`.

## Lifecycle rules

Register blocks before the engine builds its pipeline: before `StartAsync` on `VideoCaptureCoreX`,
before `OpenAsync`/`PlayAsync` on `MediaPlayerCoreX`. Adding, removing, or clearing processing blocks
after the pipeline build has started is ignored and logged as a `Warning`:

- `VideoCaptureCoreX`: *"Cannot add a processing block while capture is running. Add it before
  Start()."*
- `MediaPlayerCoreX`: *"Cannot add a processing block while playback is running. Add it before
  Play()."*

(The log text uses the classic `Start()`/`Play()` names even though the public API you call is
`StartAsync()`/`PlayAsync()` — the warning strings predate the async wrappers and are quoted here
verbatim so you can grep for them.)

If you see one of these messages in the log, the block call happened after the session was already
started (or starting) — move it earlier.

Once the session starts, the engine owns wired processing blocks and disposes them when the session
stops. Create a fresh block instance before each new session; block events are raised on pipeline or
block worker threads — keep handlers non-blocking and marshal UI changes to the UI dispatcher or main
thread. If a chain is inactive, its registered blocks don't run — for example, an audio processing
block requires an audio source and a built audio chain.

## Pipeline insertion order

- **`VideoCaptureCoreX`**: video AI blocks are inserted after video effects and before the sample
  grabber, overlay, tee, renderer, and outputs — so overlays the AI block draws reach both the
  preview and every recording/streaming output.
- **`MediaPlayerCoreX`**: video AI blocks are inserted after video effects and before the sample
  grabber, renderer, and custom video outputs.

## VideoCaptureCoreX video AI

```csharp
var detector = new YOLOObjectDetectorBlock(new YoloDetectorSettings(modelPath)
{
    Model = ObjectDetectorModel.YOLOv8,
    DrawDetections = true,
});

detector.OnObjectsDetected += (sender, e) =>
{
    // Raised from the streaming thread.
    Console.WriteLine($"Detected {e.Objects.Length} objects.");
};

core.Video_Processing_AddBlock(detector); // before StartAsync
await core.StartAsync();
```

The same pattern applies to `OcrBlock`, `ObjectAnalyticsBlock`, `FaceRecognitionBlock`,
`LicensePlateRecognizerBlock`, `BackgroundRemovalBlock`, and `OnnxInferenceBlock` — see each block's
own page for its settings and event payload.

## VideoCaptureCoreX speech-to-text

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var sttSettings = new SpeechToTextSettings(whisperModelPath)
{
    EnableVad = false, // or keep EnableVad = true and set sttSettings.Vad.ModelPath = sileroVadModelPath;
};
var stt = new SpeechToTextBlock(sttSettings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

core.Audio_Processing_AddBlock(stt); // before StartAsync
await core.StartAsync();
```

`EnableVad` defaults to `true`, which requires `SileroVadSettings.ModelPath` — the pipeline build
fails if VAD is enabled but no model path is set. Either disable VAD or set `sttSettings.Vad.ModelPath`
before registering the block.

For capture, an audio source is required. `Audio_OutputBlock` can activate and terminate the audio
chain without `Audio_Play`, `Audio_Record`, speaker monitoring, or a recording output.

## MediaPlayerCoreX video AI

```csharp
var ocr = new OcrBlock(new OcrSettings(
    detectionModelPath,
    recognitionModelPath,
    characterDictionaryPath,
    classificationModelPath)
{
    DrawResults = true,
});

ocr.OnTextDetected += Ocr_OnTextDetected;

player.Video_Processing_AddBlock(ocr); // before OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

## MediaPlayerCoreX speech-to-text

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var sttSettings = new SpeechToTextSettings(whisperModelPath)
{
    EnableVad = false, // or keep EnableVad = true and set sttSettings.Vad.ModelPath = sileroVadModelPath;
};
var stt = new SpeechToTextBlock(sttSettings);
stt.OnSpeechRecognized += SpeechToText_OnSpeechRecognized;

player.Audio_Processing_AddBlock(stt); // before OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

For playback, `Audio_Play` must be `true` for the audio chain to be built. Replacing the sink with a
non-synced null renderer avoids speaker output and does not pace transcription to real-time playback.

## Troubleshooting

| Symptom | Likely cause | Fix |
| --- | --- | --- |
| Block's events never fire, no warning logged | Block registered after the session was already running | Register with `Video_Processing_AddBlock`/`Audio_Processing_AddBlock` before `StartAsync` (`VideoCaptureCoreX`) or before `OpenAsync`/`PlayAsync` (`MediaPlayerCoreX`). |
| Log shows *"Cannot add a processing block while capture/playback is running..."* | Same as above — the call happened after the pipeline started building | Move the `Video_Processing_AddBlock`/`Audio_Processing_AddBlock` call earlier in your startup sequence. |
| `SpeechToTextBlock` never receives audio on `VideoCaptureCoreX` | No `Audio_Source` configured | An audio processing block requires a built audio chain — set `Audio_Source` (and, if you don't want speaker output, `Audio_OutputBlock` to a non-synced `NullRendererBlock`). |
| `SpeechToTextBlock` never receives audio on `MediaPlayerCoreX` | `Audio_Play` left at its default | Set `player.Audio_Play = true` before `OpenAsync`; the audio chain isn't built otherwise, regardless of registered audio blocks. |
| AI block's overlay doesn't appear in the recorded/streamed output, only the preview | Block registered on the wrong chain, or inserted after the point the engine builds outputs | Video AI blocks are inserted before the sample grabber/overlay/tee/renderer/outputs — see [Pipeline insertion order](#pipeline-insertion-order) — so this is usually a registration-timing issue, not a topology one. |
| Block instance reused across a second `StartAsync`/`PlayAsync` throws or behaves oddly | The engine already disposed the previous session's block instance | Create a fresh block instance (and re-subscribe to its events) for every new capture/playback session. |

## Frequently Asked Questions

### Do I need to rebuild a manual MediaBlocksPipeline to use AI blocks with VideoCaptureCoreX/MediaPlayerCoreX?

No — that's the point of this integration. `Video_Processing_AddBlock`/`Audio_Processing_AddBlock`
insert the same AI block instances into the engine's existing internal pipeline; you don't construct
or manage any `MediaBlocksPipeline` yourself.

### Can I add or remove AI blocks while capture/playback is running?

No — additions/removals/clears are ignored (and logged as a warning) once the pipeline build has
started. Register everything you need before `StartAsync` / `OpenAsync`+`PlayAsync`, and start a new
session with fresh block instances if the set of blocks needs to change.

### Do video AI blocks slow down recording or streaming outputs?

They add their own inference cost to the video chain, since they run in-line before the renderer and
outputs. Use `FramesToSkip` (available on every video AI block's settings) and, where available, a GPU
execution provider to keep that cost bounded.

### Can I combine multiple AI blocks — for example object detection and speech-to-text — in one session?

Yes — register one or more `IVideoProcessingBlock`s with `Video_Processing_AddBlock` and, separately,
an `IAudioProcessingBlock` (like `SpeechToTextBlock`) with `Audio_Processing_AddBlock`, all before the
session starts. Video and audio processing chains are independent.

## Demos

Every block covered on this page also has focused Media Blocks pipeline demos — see the **Demos**
section on each block's own page ([OCR](ocr.md#demos), [Object detection](object-detection.md#demos),
[Object analytics](object-analytics.md#demos), [Face recognition](face-recognition.md#demos),
[License plate recognition](license-plate-recognition.md#demos),
[Background removal](background-removal.md#demo), [Speech-to-text](speech-to-text.md#demos)).

The SDK's demo set additionally ships 16 demos built directly on `VideoCaptureCoreX` /
`MediaPlayerCoreX` using the API on this page — Object Detection, OCR, Face Recognition, and Live
Subtitles, each for both WPF and MAUI, on both the capture and playback engines
(`Capture Object Detection X`, `Capture Object Detection X WPF`, `Capture OCR X`,
`Capture OCR X WPF`, `Capture Face Recognition X`, `Capture Face Recognition X WPF`,
`Capture Live Subtitles X`, `Capture Live Subtitles X WPF`, `Player Object Detection X`,
`Player Object Detection X WPF`, `Player OCR X`, `Player OCR X WPF`, `Player Face Recognition X`,
`Player Face Recognition X WPF`, `Player Live Subtitles X`, `Player Live Subtitles X WPF`). GitHub
links will be added here once these demos are published to the public samples repository.
