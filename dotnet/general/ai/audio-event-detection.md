---
title: Audio Event Detection SDK for .NET — YAMNet AudioSet
description: Detect real-world sounds — siren, dog bark, glass break, alarm, music, speech — in live or file audio on-device with a YAMNet AudioSet classifier.
sidebar_label: Audio Event Detection
tags:
  - .NET
  - AI
  - YAMNet
  - AudioSet
  - Audio Event Detection
  - Sound Classification
  - Sound Recognition
  - Surveillance
  - VideoCaptureCoreX
  - MediaPlayerCoreX
primary_api_classes:
  - AudioEventDetectorBlock
  - AudioEventDetectorSettings
  - AudioEventArgs
  - AudioEventScore
  - AudioScoresEventArgs
---

# Audio Event Detection — AudioEventDetectorBlock

`AudioEventDetectorBlock` is an audio-only Media Block from `VisioForge.DotNet.Core.AI`. It taps the
audio stream, resamples it to 16&nbsp;kHz mono, and runs a **YAMNet** (Google, AudioSet) ONNX
classifier over short windows to recognize real-world sounds — siren, dog bark, glass break, alarm,
music, speech and hundreds more (521 AudioSet classes). Audio passes through unchanged. The block
implements `IAudioProcessingBlock`, so it can be inserted into a manual pipeline or registered on
`VideoCaptureCoreX`/`MediaPlayerCoreX`. This is a natural companion to the object-analytics and PTZ
features for surveillance and monitoring.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.AI;
using VisioForge.Core.Types.X.Sources;
```

## Basic block setup

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    Threshold = 0.5f,        // confidence a class must reach to fire
    SmoothingWindows = 3,    // moving average that suppresses one-window blips
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += (sender, e) =>
{
    // Raised on a background worker thread — marshal to the UI thread before touching UI.
    Console.WriteLine($"{e.Label} ({e.Confidence:F2}) [{e.Start:mm\\:ss} - {e.End:mm\\:ss}]");
};
```

Wire it between an audio source and a sink (or any downstream audio block):

```csharp
var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(mediaFile, renderVideo: false, renderAudio: true));

// Unsynced: an offline file is analyzed as fast as the model allows.
// Keep IsSync = true (the default) for a live source you also want to monitor.
var sink = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = false };

pipeline.Connect(source.AudioOutput, detector.Input);
pipeline.Connect(detector.Output, sink.Input);

await pipeline.StartAsync();
```

## Using it with VideoCaptureCoreX and MediaPlayerCoreX

`AudioEventDetectorBlock` implements `IAudioProcessingBlock`, so instead of building a manual pipeline
you can register it on an X engine. The block **must be added before the session starts** — the
processing block list is consumed while the pipeline is built, and a block added later is ignored.

For capture, terminate the audio chain with a non-synced null renderer if you want analysis without
speaker monitoring or recording:

```csharp
core.Audio_Source = microphoneSettings;
core.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += Detector_OnAudioEvent;

core.Audio_Processing_AddBlock(detector); // before StartAsync
await core.StartAsync();
```

For playback, `Audio_Play` must be `true` for the audio chain to be built:

```csharp
player.Audio_Play = true;
player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio)
{
    IsSync = false,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnAudioEvent += Detector_OnAudioEvent;

player.Audio_Processing_AddBlock(detector); // before OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

See [Using AI blocks with VideoCaptureCoreX and MediaPlayerCoreX](x-engines.md) for the full
`Audio_Processing_*`/`Audio_OutputBlock` API and lifecycle rules.

## How events are raised

The block classifies each `WindowSeconds` window, averages the model's per-frame scores, and smooths
them with a moving average over `SmoothingWindows` windows. A per-class **hysteresis** (Schmitt
trigger) then collapses one continuous sound into a single event: a class enters at `Threshold` and
leaves at `Threshold * ReleaseRatio` (0.7 by default). `OnAudioEvent` fires **once per sound**, when
it ends (drops below the release threshold) or at end-of-stream, carrying the full `Start`..`End` span
and the peak `Confidence`. This means one continuous siren yields a single event with a duration, not
a stream of duplicates.

`Start` and `End` are measured over the audio the block has actually analysed, counting from zero when
the pipeline starts — they are not media positions. For straight-through playback or capture from the
beginning the two are the same. They part company once audio is skipped or replayed underneath the
block: seeking a `MediaPlayerCoreX` does not reset this clock, so after a seek the reported times no
longer line up with positions in the file, and a gap in a live source leaves every later time early by
the missing duration.

Inference runs on a dedicated background worker fed by an **unbounded** sample queue, so the streaming
thread is never blocked and **the queue never discards samples to bound its own growth**: a source that
outruns the worker (a live source on a slow provider, or a file decoded at full speed by an unsynced
renderer) simply grows the backlog — 64&nbsp;KB per buffered second at 16&nbsp;kHz mono — which drains
as soon as the worker catches up. Two designed exits stop the analysis instead of quietly dropping a
fraction of it; both are described in the notes below. YAMNet is tiny — on the CPU, roughly 0.6&nbsp;ms
for the default 0.975&nbsp;s window and 1.8&nbsp;ms for the 5&nbsp;s maximum (the cost follows the
window's frame count), over 1000× real time either way, so the CPU provider is usually sufficient.

## Key settings

`AudioEventDetectorSettings(yamnetModelPath)`. Unlike the vision AI settings, this type does **not**
derive from `OnnxInferenceSettings` — YAMNet consumes an audio waveform, not an image. Every property
below is read **once, when the block is built** during `StartAsync` — set them before you start the
pipeline; mutating the settings object afterwards has no effect.

| Property | Default | Description |
| --- | --- | --- |
| `ModelPath` | — | Absolute path to the YAMNet ONNX model. Required. |
| `Threshold` | 0.5 | Confidence a class must reach to start an event, clamped to 0.01–1 (a threshold of 0 could never be released and would latch every class). |
| `ReleaseRatio` | 0.7 | An active event ends below `Threshold * ReleaseRatio` (hysteresis), clamped to 0.01–1 for the same reason. |
| `SmoothingWindows` | 3 | Windows the scores are averaged over (1 disables smoothing). |
| `WindowSeconds` | 0.975 | Inference window length — one 0.96&nbsp;s YAMNet patch plus a small margin; clamped 0.96–5&nbsp;s. |
| `ClassFilter` | null | Optional allowlist of AudioSet class names (e.g. `"Siren"`, `"Dog"`). |
| `Provider` | `Auto` | ONNX execution provider (CPU is usually enough). |
| `EnableScoresEvent` | false | Raise `OnScores` (top-K) for every window, for a live meter. |
| `TopK` | 5 | Number of classes reported in the scores event. |

## Filtering to specific sounds

Pass a `ClassFilter` allowlist to report only the sounds you care about — for example an alarm
watchdog that ignores everything else:

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    ClassFilter = new[] { "Siren", "Emergency vehicle", "Police car (siren)", "Alarm", "Glass", "Gunshot, gunfire" },
};
```

Class names are the AudioSet display names and must match one exactly (case is ignored, but a typo, a
singular/plural difference or an abbreviation is not). Unknown names are ignored with a warning **as
long as at least one name resolves**. A non-empty filter in which **no** name resolves fails the
block's build — `StartAsync` returns `false` and an error naming the unresolved entries is reported —
rather than silently falling back to detecting all 521 classes.

## Live scores meter

Enable `OnScores` to receive the top-K class scores for every processed window (useful for a live
level meter or a debugging view):

```csharp
var settings = new AudioEventDetectorSettings(yamnetModelPath)
{
    EnableScoresEvent = true, // read once, when the pipeline starts — set it before StartAsync
    TopK = 5,
};

var detector = new AudioEventDetectorBlock(settings);
detector.OnScores += (sender, e) =>
{
    foreach (var s in e.Scores)
    {
        Console.WriteLine($"  {s.Label}: {s.Confidence:F2}");
    }
};
```

## Model

The block uses **YAMNet**, Google's MobileNet-v1 audio classifier trained on
[AudioSet](https://research.google.com/audioset/), predicting 521 classes from the AudioSet ontology.
The model consumes a 16&nbsp;kHz mono waveform (the block handles resampling and down-mixing) and
returns per-frame class scores. The weights are Apache-2.0. Your application supplies the model file and
points `ModelPath` at it: the SDK NuGet packages ship neither the weights nor a downloader, and the block
never fetches anything itself. The demo below has a **Download** button that fetches the model once and
caches it; nothing is downloaded unless you click it.

**For production, convert the official Google TF-Hub model to ONNX yourself** rather than relying on a
third-party ONNX export. The conversion is a format change only and does not alter the Apache-2.0 weights
license:

```bash
pip install tensorflow tensorflow-hub tf2onnx onnx
python -c "import tensorflow_hub as hub, tensorflow as tf; m=hub.load('https://tfhub.dev/google/yamnet/1'); tf.saved_model.save(m, 'yamnet_tf')"
python -m tf2onnx.convert --saved-model yamnet_tf --output yamnet.onnx --opset 13
```

If you use a community ONNX export instead, pin it to an immutable revision and verify its SHA-256 — the
WPF [Audio Event Detection Demo](#demos) downloads a pinned export of the Apache-2.0 weights and
verifies the hash after download.

> Sound event recognition uses the AudioSet ontology and the YAMNet model by Google, released under
> the Apache-2.0 license.

## Demos

The `Audio Event Detection Demo` (WPF Media Blocks — model download with SHA-256 verification, class
filter, live event log) is in the SDK's demo set and will be linked here once published to the public
samples repository.

## Notes

- `OnAudioEvent` and `OnScores` are raised on the inference worker thread; marshal to the UI thread
  before updating UI. A throwing handler is caught and logged, never propagated onto the worker.
- At end-of-stream the trailing window and any still-active events are flushed before teardown.
- Under normal operation the block never drops audio — neither from the *pass-through* path nor from
  the analysis path. The internal analysis queue is unbounded: a source that outruns the worker grows
  the backlog instead of discarding samples, and the audio still passes downstream untouched.
- Two designed exits end the analysis rather than lose audio silently, and neither ever touches the
  *pass-through* path: if the backlog no longer fits in memory, the block reports an error once and
  stops analyzing that stream (use a GPU execution provider or faster hardware); and an explicit stop
  abandons the backlog the worker has not reached yet — only a natural end-of-stream drains it in full.
