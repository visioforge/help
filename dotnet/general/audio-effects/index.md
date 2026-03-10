---
title: Audio Effects in C# and .NET - EQ, Reverb, Filters, and More
description: Apply real-time audio effects in C# and .NET with VisioForge SDKs. Equalizer, reverb, echo, noise reduction, pitch shift, and 30+ effects.
sidebar_label: Audio Effects
---

# Real-Time Audio Effects for .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge Media Framework provides over 30 audio effects for real-time audio processing in C# and .NET applications. Built on GStreamer, the cross-platform effects include equalizers, reverb, echo, dynamic compression, filters, pitch shifting, AI-based noise reduction, and more — all running on Windows, macOS, Linux, iOS, and Android.

## SDKs and Platforms

### Cross-Platform Effects

- **SDKs**: Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **Platforms**: Windows, macOS, Linux, iOS, Android
- **Namespace**: `VisioForge.Core.Types.X.AudioEffects`

### Classic DSP Effects

- **SDKs**: Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore)
- **Platforms**: Windows only
- **Namespace**: `VisioForge.Core.DSP`

### DirectSound Effects

- **SDKs**: Video Capture SDK, Media Player SDK, Video Edit SDK (Classic cores)
- **Platforms**: Windows only
- **Namespace**: `VisioForge.Core.Types.X._Windows.AudioEffects`

For detailed parameters and properties of each effect, see [Audio Effects Reference](reference.md).

## Effect Categories

### Volume and Dynamics

- **VolumeAudioEffect** — Basic volume control with mute functionality
- **AmplifyAudioEffect** — Audio signal amplification with clipping control
- **CompressorExpanderAudioEffect** — Dynamic range compression and expansion
- **DynamicAmplifyAudioEffect** — Adaptive gain control with attack/release times

### Equalization

- **Equalizer10AudioEffect** — 10-band graphic equalizer with fixed frequencies
- **EqualizerParametricAudioEffect** — Parametric equalizer with configurable bands
- **TrebleEnhancerAudioEffect** — High-frequency boost
- **TrueBassAudioEffect** — Low-frequency boost

### Filters

- **HighPassAudioEffect** — High-pass filter for removing low frequencies
- **LowPassAudioEffect** — Low-pass filter for removing high frequencies
- **BandPassAudioEffect** — Band-pass filter for specific frequency ranges
- **NotchAudioEffect** — Notch filter for removing specific frequencies
- **ChebyshevLimitAudioEffect** — Chebyshev low/high-pass filters with sharp cutoffs
- **ChebyshevBandPassRejectAudioEffect** — Chebyshev band-pass/reject filters

### Spatial and Stereo

- **BalanceAudioEffect** — Stereo balance control (pan left/right)
- **WideStereoAudioEffect** — Stereo width enhancement
- **Sound3DAudioEffect** — 3D spatial audio effects
- **HRTFRenderAudioEffect** — Head-Related Transfer Function spatial audio
- **PhaseInvertAudioEffect** — Phase inversion for polarity correction

### Time-Based Effects

- **EchoAudioEffect** — Echo and delay effects
- **RSAudioEchoAudioEffect** — Enhanced echo with advanced controls
- **ReverberationAudioEffect** — Room reverberation (Freeverb algorithm)
- **FadeAudioEffect** — Fade in/out volume automation

### Modulation Effects

- **PhaserAudioEffect** — Phaser effect with LFO modulation
- **FlangerAudioEffect** — Flanging effect with delay modulation

### Pitch and Tempo

- **PitchShiftAudioEffect** — Pitch shifting without tempo change
- **ScaleTempoAudioEffect** — Tempo change without pitch shift

### Special Effects

- **KaraokeAudioEffect** — Vocal removal for karaoke
- **RemoveSilenceAudioEffect** — Automatic silence detection and removal
- **CsoundAudioEffect** — Advanced Csound-based audio processing

### Noise Reduction and Measurement

- **AudioRNNoiseAudioEffect** — AI-based noise reduction using RNN
- **AudioLoudNormAudioEffect** — EBU R128 loudness normalization
- **EbuR128LevelAudioEffect** — EBU R128 loudness measurement

### Channel Management

- **ChannelOrderAudioEffect** — Channel remapping and routing
- **DownMixAudioEffect** — Multi-channel to stereo/mono downmixing

### DirectSound Effects (Windows Only, Classic SDKs)

- **DS Chorus** — Multiple delayed and modulated copies
- **DS Distortion** — Audio distortion/overdrive
- **DS Gargle** — Gargling/tremolo modulation
- **DS Reverb (I3DL2)** — Professional reverb with environmental modeling
- **DS Waves Reverb** — Simplified reverb

## GStreamer Elements

All cross-platform audio effects are built on top of GStreamer multimedia framework. Each effect wraps one or more GStreamer elements:

| Category | GStreamer Elements |
|----------|-------------------|
| Volume/Dynamics | volume, audioamplify, audiodynamic |
| Equalization | equalizer-10bands, equalizer-nbands |
| Filters | audiocheblimit, audiochebband, audioiirfilter |
| Spatial | audiopanorama, stereo, hrtfrender |
| Time-Based | audioecho, rsaudioecho, freeverb |
| Modulation | Custom phaser/flanger implementations |
| Pitch/Tempo | scaletempo, pitch (SoundTouch) |
| Special | audiokaraoke, csoundfilter, removesilence |
| Noise Reduction | audiornnoise, audioloudnorm, ebur128level |
| Channels | audioconvert, custom routing |

## Common Usage Patterns

### Adding Effects (Cross-Platform SDKs)

```csharp
// Create an audio effect
var volumeEffect = new VolumeAudioEffect(1.5); // 150% volume

// VideoCaptureCoreX / MediaPlayerCoreX
core.Audio_Effects_AddOrUpdate(volumeEffect);
```

### Combining Multiple Effects

Effects are processed in the order they are added:

```csharp
// Create a processing chain
core.Audio_Effects_AddOrUpdate(new HighPassAudioEffect(80));           // Remove rumble
core.Audio_Effects_AddOrUpdate(new CompressorExpanderAudioEffect());   // Compress dynamics
core.Audio_Effects_AddOrUpdate(new Equalizer10AudioEffect(levels));    // EQ adjustments
core.Audio_Effects_AddOrUpdate(new ReverberationAudioEffect());        // Add reverb
```

### Real-Time Parameter Updates

Most effects support real-time parameter changes:

```csharp
var volumeEffect = new VolumeAudioEffect(1.0);
core.Audio_Effects_AddOrUpdate(volumeEffect);

// Later, during playback:
volumeEffect.Level = 0.5; // Reduce volume to 50%
volumeEffect.Mute = true; // Mute audio
core.Audio_Effects_AddOrUpdate(volumeEffect); // Apply changes
```

### Media Blocks SDK Usage

For Media Blocks SDK pipeline-based usage with dedicated audio effect blocks, see [Audio Processing and Effect Blocks](../../mediablocks/AudioProcessing/index.md).

## Performance Considerations

- **CPU Usage**: Complex effects like reverberation, Csound, and multiple equalizers can be CPU-intensive
- **Effect Order**: Place computationally expensive effects after simpler ones to reduce processing load
- **Real-Time Processing**: All effects are designed for real-time audio streaming

## Frequently Asked Questions

???+ "What audio effects are available in VisioForge .NET SDKs?"
    VisioForge provides 30+ audio effects including volume control, 10-band and parametric equalizers, reverb, echo, compressor/expander, high/low/band-pass filters, pitch shift, noise reduction (RNN-based), karaoke vocal removal, 3D spatial audio, and more. All cross-platform effects work on Windows, macOS, Linux, iOS, and Android.

???+ "How do I add audio effects to my C# application?"
    Create an effect instance and add it using `Audio_Effects_AddOrUpdate()`. For example:

    ```csharp
    // 10-band EQ: all bands at 0 dB (neutral)
    var eq = new Equalizer10AudioEffect(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
    core.Audio_Effects_AddOrUpdate(eq);
    ```

    Effects can be added, updated, and removed during playback. For Media Blocks SDK, use the `AudioEffectsBlock`.

???+ "Can I chain multiple audio effects together?"
    Yes. Effects are processed in the order they are added. You can create complex processing chains combining filters, EQ, compression, reverb, and other effects. Each effect processes the audio output of the previous one.

???+ "Do audio effects work in real-time during playback?"
    Yes. All VisioForge audio effects support real-time parameter changes. You can adjust volume, EQ bands, reverb levels, and other parameters while audio is playing without interrupting the stream.

???+ "What is the difference between cross-platform and DirectSound effects?"
    Cross-platform effects (namespace `VisioForge.Core.Types.X.AudioEffects`) work on all platforms using GStreamer. DirectSound effects are Windows-only and available in the classic SDK cores. Cross-platform effects cover the same functionality and more.

## See Also

- [Audio Effects Reference](reference.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Audio Encoders](../audio-encoders/index.md)
- [Audio Processing Blocks (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
