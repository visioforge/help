---
title: Speaker Diarization SDK for .NET — SpeakerDiarizationBlock
description: On-device speaker diarization for .NET — find "who spoke when" in recorded audio with pyannote segmentation, WeSpeaker embeddings and diarized transcripts.
sidebar_label: Speaker Diarization
tags:
  - .NET
  - AI
  - Speaker Diarization
  - Diarization SDK
  - Who Spoke When
  - Speaker Segmentation
  - Speaker Embedding
  - VideoCaptureCoreX
  - MediaPlayerCoreX
primary_api_classes:
  - SpeakerDiarizationBlock
  - SpeakerDiarizationSettings
  - SpeakerSegment
  - SpeakerSegmentEventArgs
  - DiarizedTranscriptBuilder
  - DiarizedTranscriptSegment
---

# Speaker Diarization — SpeakerDiarizationBlock

`SpeakerDiarizationBlock` is an audio-only Media Block that lives in the `VisioForge.Core.MediaBlocks.AI`
namespace (assembly `VisioForge.Core.AI`, NuGet package `VisioForge.DotNet.Core.AI`). It answers
"who spoke when": it taps the audio stream, finds speech turns with a pyannote segmentation ONNX model,
turns each turn into a voiceprint with a WeSpeaker/3D-Speaker embedding ONNX model, and — once the
recording has finished — groups the voiceprints into speakers and publishes the timeline. Audio passes
through the block: the tap forces a 32-bit float interleaved sample format, but the sample rate and channel
count are left unchanged. The block implements `IAudioProcessingBlock`, so it can be inserted into a manual
pipeline or registered directly on `VideoCaptureCoreX`/`MediaPlayerCoreX`.

The implementation follows the reference [sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx) offline
speaker-diarization pipeline, which uses these same two models.

```csharp
using VisioForge.Core.MediaBlocks.AI;
using VisioForge.Core.Types.X.AI;
```

## Diarization is offline — the answer arrives at end-of-stream

**The block reports nothing while the audio is still playing.** This is not a limitation of the
implementation, it is what the problem allows:

- The segmentation model separates the voices **inside one 10-second window** and numbers them locally.
  Window 5's "speaker 1" and window 6's "speaker 1" are unrelated — the model never claims otherwise.
- The only thing that can tie a voice heard at minute 1 to the same voice at minute 40 is **clustering every
  voiceprint of the recording together**, and that cannot be done until every voiceprint exists.

So the block analyses continuously as audio flows (that work is real, and it is why the answer appears almost
immediately after the last sample), but it raises `OnSpeakerSegment` — once per turn, in start-time order —
only after end-of-stream. **It suits files and finite streams.** An endless live source never reaches the
moment at which an answer exists.

## Basic block setup

```csharp
var settings = new SpeakerDiarizationSettings(segmentationModelPath, embeddingModelPath)
{
    Provider = OnnxExecutionProvider.CPU,
};

var diarization = new SpeakerDiarizationBlock(settings);

// Raised for every turn, in order, after the stream ends.
diarization.OnSpeakerSegment += (sender, e) =>
{
    var turn = e.Segment;
    Console.WriteLine($"speaker_{turn.SpeakerId:D2}: {turn.Start:c} - {turn.End:c} (conf {turn.Confidence:F2})");
};
```

The same timeline is available in one piece from `GetTimeline()`. Check `IsTimelineComplete` first — an empty
timeline means "we never got to work out who spoke", not "nobody spoke":

```csharp
if (!diarization.IsTimelineComplete)
{
    // The run never reached the clustering step: the pipeline was stopped before end-of-stream,
    // or the analysis worker faulted. The timeline is empty, and that emptiness is not an answer.
    Console.WriteLine("No diarization result — the recording was not heard to the end.");
    return;
}

Console.WriteLine($"{diarization.SpeakerCount} speaker(s).");

if (diarization.UnattributedTurns > 0)
{
    // Speech the models found but could not voiceprint: those turns carry no speaker id and
    // never reach the timeline, even though their audio WAS analysed.
    Console.WriteLine($"{diarization.UnattributedTurns} turn(s) could not be attributed to a speaker.");
}

foreach (var turn in diarization.GetTimeline())
{
    Console.WriteLine(turn); // speaker_00 [00:00:01.583 --> 00:00:03.405] (conf=0.98)
}
```

## Models — download at runtime

Two ONNX models are required, both downloaded at runtime (neither is shipped in the SDK NuGet
packages). Both are exported by the [sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx) project:

| Model | Role | License | Source |
| --- | --- | --- | --- |
| pyannote `segmentation-3.0` | Local speech-turn segmentation (10 s window, up to 3 local speakers, powerset activity) | MIT | Extract `model.onnx` from the [`sherpa-onnx-pyannote-segmentation-3-0.tar.bz2`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-segmentation-models/sherpa-onnx-pyannote-segmentation-3-0.tar.bz2) release asset. |
| WeSpeaker `voxceleb_resnet34_LM` (English, **default**) | Speaker embedding (80-bin fbank in, 256-d voiceprint out) | Apache-2.0 | [`wespeaker_en_voxceleb_resnet34_LM.onnx`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-recongition-models/wespeaker_en_voxceleb_resnet34_LM.onnx) release asset. |
| 3D-Speaker `eres2netv2_sv_zh-cn_16k-common` (**multilingual**) | Speaker embedding (80-bin fbank in, 192-d voiceprint out); much better speaker separation on non-English speech | Apache-2.0 | [`3dspeaker_speech_eres2netv2_sv_zh-cn_16k-common.onnx`](https://github.com/k2-fsa/sherpa-onnx/releases/download/speaker-recongition-models/3dspeaker_speech_eres2netv2_sv_zh-cn_16k-common.onnx) release asset. |

The segmentation model is **language-independent** — only the embedding model needs to match your
language. The default `SegmentationModelUrl` / `EmbeddingModelUrl` on `SpeakerDiarizationSettings`
point at the pyannote and WeSpeaker English release assets. They are informational — the SDK never
downloads anything itself; the files actually loaded are `SegmentationModelPath` and
`EmbeddingModelPath`. Any 80-bin-fbank WeSpeaker or 3D-Speaker embedding model with commercially usable
weights can be substituted; the fbank feature front-end configures itself (sample scaling and
feature normalization) from the model's ONNX metadata, so no manual tuning is needed when you swap it.

## Multilingual diarization

The default WeSpeaker embedding is trained on English (VoxCeleb) and under-detects speakers on other
languages. For non-English or mixed-language content, use the Apache-2.0 **3D-Speaker ERes2NetV2**
multilingual embedding model instead: point `EmbeddingModelPath` at its ONNX file and set
`EmbeddingModel = SpeakerEmbeddingModel.ERes2NetV2Multilingual`. The segmentation model is unchanged.

```csharp
var settings = new SpeakerDiarizationSettings(segmentationModelPath, eres2netv2ModelPath)
{
    EmbeddingModel = SpeakerEmbeddingModel.ERes2NetV2Multilingual,

    // Retune the threshold for this embedding — see below. The default (0.5) over-splits it.
    ClusterThreshold = 0.8f,
    Provider = OnnxExecutionProvider.CPU,
};

var diarization = new SpeakerDiarizationBlock(settings);
```

!!! warning "Retune `ClusterThreshold` when you switch embedding models"

    Each embedding model has its own cosine geometry, and the default `ClusterThreshold` of `0.5` is the
    right distance for **WeSpeaker**. The ERes2NetV2 voiceprints are spread further apart, so that same
    distance splits one speaker into several — on a four-speaker reference clip it yields six. A value
    around **`0.8`** is the starting point for ERes2NetV2. If you know the speaker count, setting
    `NumSpeakers` sidesteps the question entirely.

`EmbeddingModel` is a declarative hint: it selects the recommended download URL, while the file actually
loaded is always `EmbeddingModelPath`. Because the front-end reads its feature configuration from the model's
ONNX metadata, switching between the two models needs no other code change.

Download URLs are also exposed as constants: `SpeakerDiarizationSettings.WeSpeakerEnglishModelUrl` and
`SpeakerDiarizationSettings.ERes2NetV2MultilingualModelUrl`.

## Key settings

`SpeakerDiarizationSettings(segmentationModelPath, embeddingModelPath)`. Unlike the vision AI settings,
this type does **not** derive from `OnnxInferenceSettings` — those describe video-frame preprocessing,
which does not apply to an audio model pair.

The defaults are the reference implementation's own and are a sound starting point for arbitrary content.

| Property | Default | Description |
| --- | --- | --- |
| `SegmentationModelPath` | — | Absolute path to the pyannote `segmentation-3.0` ONNX model. Required. |
| `EmbeddingModelPath` | — | Absolute path to the speaker-embedding ONNX model (for example WeSpeaker `voxceleb_resnet34_LM` or 3D-Speaker `eres2netv2`). Required. |
| `EmbeddingModel` | `WeSpeakerEnglish` | Which embedding model the settings are configured for (`WeSpeakerEnglish` or `ERes2NetV2Multilingual`). A declarative hint that picks the default URL; the file loaded is always `EmbeddingModelPath`, and feature extraction is auto-configured from its ONNX metadata. |
| `SegmentationModelUrl` | sherpa-onnx pyannote release | Informational download URL for the segmentation model. |
| `EmbeddingModelUrl` | sherpa-onnx WeSpeaker release | Informational download URL for the embedding model. |
| `NumSpeakers` | `-1` | The **exact** number of speakers, when you know it. A positive value pins the result to that many speakers and `ClusterThreshold` is ignored. `-1` means "work it out from the voices" — the right setting whenever the count is not certain. |
| `ClusterThreshold` | `0.5` | The cosine **DISTANCE** at which two voiceprints stop being the same person. Note the direction: this is a distance (0 = identical voices), **not** a similarity — a **smaller** value is stricter and yields **more** speakers. Ignored when `NumSpeakers` is positive. |
| `MinDurationOn` | `0.3` | The shortest turn, in seconds, that is reported. Shorter ones are dropped as noise. |
| `MinDurationOff` | `0.5` | The longest pause, in seconds, that still counts as the same turn. Two turns of one speaker closer than this are joined, so a speaker pausing for breath stays one turn. |
| `Provider` | `Auto` | Execution provider for both ONNX sessions. `Auto` uses the fastest provider in the loaded ONNX Runtime native build and falls back to CPU. |
| `DeviceId` | `0` | Hardware device id when a GPU provider is selected. |

## Speaker turns

Each `SpeakerSegment` (from `SpeakerSegmentEventArgs.Segment` and `GetTimeline()`):

| Property | Description |
| --- | --- |
| `SpeakerId` | Stable, zero-based speaker identifier for this run. |
| `Start` / `End` | `TimeSpan` on the media timeline. |
| `Duration` | `End - Start`. |
| `Confidence` | How strongly the overlapping analysis windows agreed on this speaker for this turn (0..1). `1` means every window that saw the moment named the same speaker. |

Speaker ids are stable **within a run**, not across separate pipeline runs (there is no cross-run
speaker re-identification in this version).

## Diarized transcript — "who said what"

`DiarizedTranscriptBuilder` merges a speech-to-text transcript with the diarization timeline, labeling
each transcript segment with the speaker who overlaps it the most (ties go to the lower speaker id). It
is pure data processing — no model or media dependency — so it pairs a
[`SpeechToTextBlock`](speech-to-text.md) with a `SpeakerDiarizationBlock`. Build it after end-of-stream,
when the diarization timeline exists.

```csharp
using VisioForge.Core.Types.X.AI;

// transcript: (start, end, text) tuples from SpeechToTextBlock.OnSpeechRecognized
// timeline: diarization.GetTimeline()
var labeled = DiarizedTranscriptBuilder.Build(transcript, timeline);
foreach (DiarizedTranscriptSegment seg in labeled)
{
    // "[00:00:01.000 --> 00:00:03.000] speaker_00: A pencil with black lead writes best."
    Console.WriteLine(seg);
}
```

Each `DiarizedTranscriptSegment` carries `Start`, `End`, `Text`, and the attributed `SpeakerId`
(`-1` when no diarization turn overlaps the segment — for example a silent gap).

## Manual Media Blocks pipeline

Place `SpeakerDiarizationBlock` in the audio chain before the audio renderer or output:

```csharp
var diarization = new SpeakerDiarizationBlock(settings);
diarization.OnSpeakerSegment += Diarization_OnSpeakerSegment;

pipeline.Connect(audioSource.Output, diarization.Input);
pipeline.Connect(diarization.Output, audioRenderer.Input);
```

## MediaPlayerCoreX file diarization

For playback, `Audio_Play` must be `true` for the audio chain to be built:

```csharp
player.Audio_Play = true;

var diarization = new SpeakerDiarizationBlock(settings);
diarization.OnSpeakerSegment += Diarization_OnSpeakerSegment;

player.Audio_Processing_AddBlock(diarization); // before OpenAsync / PlayAsync
await player.OpenAsync(source);
await player.PlayAsync();
```

Let the file play to its end: stopping playback early skips the clustering step and leaves the timeline
empty. Diarization reads only the audio, so to index a file as fast as the hardware allows rather than in
real time, give the player an unsynced audio sink — `player.Audio_OutputBlock = new NullRendererBlock(MediaBlockPadMediaType.Audio) { IsSync = false };` —
so the audio is not paced by the clock. (`Video_Renderer_IsSync = false` only unpaces a video renderer, so
it has no effect on an audio-only file.)

See [Using AI blocks with VideoCaptureCoreX and MediaPlayerCoreX](x-engines.md) for the full
`Audio_Processing_*` API and lifecycle rules. The engine owns the wired block after start and disposes
it when the session stops — create a new block for the next session.

## How it works

Understanding these three stages explains every setting above, and why the result cannot arrive earlier.

1. **Segmentation, as the audio flows.** The audio is resampled to mono 16 kHz and cut into 10-second
   windows that advance 1 second at a time — so the windows overlap by 90%, and every instant of audio is
   seen by about ten of them. The pyannote model labels, for each of its ~59 output frames per second, which
   of up to three *window-local* speakers are talking.
2. **Voiceprints, as the audio flows.** For each window and each speaker active in it, the audio of that
   speaker is collected — **minus every frame where somebody else is talking too** — and turned into a
   voiceprint. Overlapped speech is excluded deliberately: a voiceprint taken from two voices at once sits
   between them and looks similar to both, which welds two speakers into one. Turns with less than ~0.17 s of
   clean speech are skipped for the same reason.
3. **Speakers and the timeline, at end-of-stream.** All the voiceprints of the recording are clustered
   together (agglomerative, complete linkage, cosine distance — `ClusterThreshold` / `NumSpeakers`). The
   window-local speaker numbers are rewritten into global ones, all the overlapping windows are laid on one
   frame grid, and **every frame is decided by a vote among the ~10 windows covering it**: how many people
   are talking is the average those windows report, and who they are is the most-voted. A window that
   mislabels a moment is outvoted by its neighbours. Finally the frames become turns, short pauses are
   bridged (`MinDurationOff`) and fragments are dropped (`MinDurationOn`).

## Threading, performance, and lifetime

Analysis runs on a **dedicated background worker**, not the streaming thread: the tap resamples the audio to
mono 16 kHz and pushes it into an **unbounded queue**, and the worker consumes it in overlapping windows.
Because analysis is off the streaming thread, audio is never paced to the models — a source that runs faster
than they do (a file decoded at full speed, or a live source on a slow execution provider) simply makes the
worker fall behind.

**A backlog is never dropped.** The queue grows instead — at 16 kHz mono that is 64 KB per buffered second —
and it drains as soon as the worker catches up. If a machine is permanently slower than its source, the
backlog keeps growing, and that is the honest signal: the answer is faster hardware or a GPU execution
provider, never analysing a fraction of the audio. At end-of-stream the pipeline waits for the worker to
drain the backlog and publish the timeline before tearing down.

`OnSpeakerSegment` is raised on the worker thread — never touch UI directly from the handler; marshal
to the UI dispatcher or main thread.

Memory scales with the recording: the voiceprints are kept until the end (they are the clustering's input),
and clustering builds a pairwise distance matrix over them.

## Result completeness

| Member | Description |
| --- | --- |
| `IsTimelineComplete` | `true` when the run actually produced an answer: the stream reached end-of-stream, the worker drained, and the speakers were clustered and published. **Check it before reading `GetTimeline()`** — when it is `false` the timeline is EMPTY, and that emptiness means "we never got to work out who spoke", not "nobody spoke". |
| `SpeakerCount` | How many distinct speakers were found. `0` until the timeline is published. |
| `UnattributedTurns` | Speech turns the segmentation model **found** but the embedding model could not voiceprint. Their audio *was* analysed, but they carry no speaker id and are absent from the timeline. |
| `ActiveProvider` | The execution provider the ONNX sessions actually engaged. Stays valid after the run, so it can be logged at the end. |

**A timeline is fully trustworthy when `IsTimelineComplete` is `true` AND `UnattributedTurns` is `0`.**

## Use cases

- **Meeting and interview transcription** — label a diarized transcript with per-speaker turns.
- **Call-center analytics** — separate agent and customer speech for talk-time and turn-taking metrics.
- **Media indexing** — index recorded video/audio by speaker for search and navigation.
- **Podcast and broadcast tooling** — auto-generate speaker-labeled show notes with speech-to-text.

## Troubleshooting

| Symptom | Likely cause | Fix |
| --- | --- | --- |
| No turns are ever raised, and `IsTimelineComplete` is `false` | The stream never ended — the pipeline was stopped early, or the source is an endless live stream | Let the source reach end-of-stream. Diarization needs the complete recording; there is no partial answer to give. |
| No turns are raised although the file played to the end | A model path is invalid, or the audio chain isn't built | Confirm both ONNX files exist at their paths; for playback confirm `Audio_Play = true`. |
| Too many speakers detected | `ClusterThreshold` too small for this audio/model | **Raise** `ClusterThreshold` (it is a distance — a larger value merges more), or set `NumSpeakers` to the known count. |
| Speakers merged into one | `ClusterThreshold` too large | **Lower** `ClusterThreshold` (a smaller distance splits more eagerly), or set `NumSpeakers`. |
| Analysis lags a fast file source, and memory grows | The overlapping windows are compute-heavy, so the worker falls behind and the (unbounded) backlog grows — no audio is lost, but it is buffered | Use `Provider = OnnxExecutionProvider.CUDA` / `DirectML` on a GPU. A buffered second costs 64 KB. |
| Speech is missing although `IsTimelineComplete` is `true` | The embedding model failed to voiceprint some detected turns, so they got no speaker id | Check `UnattributedTurns`; look for the `SpeakerEmbeddingEngine` error in the log for the cause (typically a GPU out-of-memory or provider fallback). |

## Frequently Asked Questions

### Why do I get no speaker turns until the file ends?

Because until then there is no such thing as "speaker 2" to report. The segmentation model separates voices
only inside a single 10-second window and numbers them locally; tying a voice at minute 1 to the same voice at
minute 40 requires clustering every voiceprint of the recording at once. The block does all the work it can as
the audio flows, and publishes the moment the last sample is in.

### Can I diarize an endless live stream?

No. A live stream has no end-of-stream, so the clustering step never runs. Diarize finite recordings — or
segment a live stream into finite chunks and diarize each (speaker ids are not comparable between chunks).

### Does SpeakerDiarizationBlock require an internet connection?

No — diarization runs fully on-device through ONNX Runtime. Your application downloads the two model
files once (or bundles them); nothing is called per-request over the network.

### Can I combine it with speech-to-text to get a speaker-labeled transcript?

Yes — run a [`SpeechToTextBlock`](speech-to-text.md) and a `SpeakerDiarizationBlock` on the same audio,
then merge the transcript with `diarization.GetTimeline()` using `DiarizedTranscriptBuilder.Build` after
end-of-stream.

### Are speaker ids the same across two separate runs?

No — ids are stable within a single run only. This version does not re-identify a speaker across runs.

### Which languages does it work with?

The segmentation model is language-independent, so it works with any language out of the box. The
default WeSpeaker embedding model is trained on English (VoxCeleb) and under-detects speakers on other
languages; for non-English or mixed-language audio, switch `EmbeddingModelPath` to the Apache-2.0
3D-Speaker ERes2NetV2 multilingual model and set `EmbeddingModel =
SpeakerEmbeddingModel.ERes2NetV2Multilingual` (see [Multilingual diarization](#multilingual-diarization)).
The front-end adapts its feature extraction to the chosen model automatically from the ONNX metadata.
