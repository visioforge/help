---
title: Audio Effects Reference
description: Complete guide to audio effects in VisioForge SDKs. Includes volume, EQ, echo, reverb, compression, and more effects for cross-platform audio processing.
sidebar_label: Audio Effects Reference
---

# Audio Effects Reference

This document provides a comprehensive reference for all audio effects available in VisioForge SDKs.

## Effect Types and Availability

VisioForge provides **two types** of audio effects with different platform support:

### Cross-Platform Effects
- **Platform Support**: Windows, macOS, Linux, iOS, Android
- **SDKs**: Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **Implementation**: Cross-platform audio processing
- **Location**: `VisioForge.Core.Types.X.AudioEffects` namespace
- **Usage**: Via `AudioEffectsBlock` in audio pipelines (Media Blocks SDK) or `Audio_Effects_*` methods (VideoCaptureCoreX/MediaPlayerCoreX)

### Classic DSP Effects (VideoCaptureCore/MediaPlayerCore/VideoEditCore)
- **Platform Support**: Windows only
- **SDKs**: Video Capture SDK, Media Player SDK, Video Edit SDK
- **Implementation**: Native DSP processing
- **Location**: `VisioForge.Core.DSP` namespace
- **Usage**: Via `Audio_Effects_*` methods on SDK cores

### DirectSound Effects (Windows Classic)
- **Platform Support**: Windows only
- **SDKs**: Video Capture SDK, Media Player SDK, Video Edit SDK
- **Implementation**: DirectSound/DirectX-based
- **Location**: `VisioForge.Core.Types.X._Windows.AudioEffects` namespace
- **Usage**: Via `Audio_Effects_DS_*` methods on SDK cores

## Volume and Level Control

### Volume
**Availability:** ✅ Cross-platform (Media Blocks, VideoCaptureCoreX, MediaPlayerCoreX) | ✅ Classic SDKs (Windows)  
**Audio Element:** `volume`

Controls the volume level of an audio stream.

**Properties:**
- **Level** (double): Volume level (0.0 = silence, 1.0 = normal, >1.0 = amplification)
- **Mute** (bool): Mute flag

**Typical Usage (Cross-Platform SDKs):**
```csharp
var effect = new VolumeAudioEffect(0.8); // 80% volume
effect.Mute = false;

// Media Blocks SDK
audioEffectsBlock.AddOrUpdate(effect);

// VideoCaptureCoreX / MediaPlayerCoreX
videoCaptureX.Audio_Effects_AddOrUpdate(effect);
```

**Typical Usage (Classic SDKs):**
```csharp
// Handled through SDK audio volume properties
```

### Amplify
**Availability:** ✅ Cross-platform (Media Blocks, VideoCaptureCoreX, MediaPlayerCoreX) | ✅ Classic SDKs (Windows)  
**Audio Element:** `audioamplify` (Cross-platform)  
**DSP Class:** `Amplify` (Classic SDKs)

Amplifies audio with configurable clipping methods.

**Properties:**
- **Level** (double): Amplification factor (1.0 - 10.0)
- **ClippingMethod**: None, Normal, or Soft (Media Blocks only)

**Typical Usage (Cross-Platform SDKs):**
```csharp
var effect = new AmplifyAudioEffect(2.0); // Double amplification
effect.ClippingMethod = AmplifyClippingMethod.Normal;

// Media Blocks SDK
audioEffectsBlock.AddOrUpdate(effect);

// VideoCaptureCoreX / MediaPlayerCoreX
videoCaptureX.Audio_Effects_AddOrUpdate(effect);
```

**Typical Usage (Classic SDKs):**
```csharp
// Add via Audio_Effects_Add, then configure
videoCaptureCore.Audio_Effects_Add(0, AudioEffectType.Amplify, "amp", true, 
    TimeSpan.Zero, TimeSpan.MaxValue);
videoCaptureCore.Audio_Effects_Amplify(0, "amp", 15000, false);
```

## Stereo Processing

### Balance
**Audio Element:** `audiopanorama`

Controls stereo balance (left-right panning).

**Properties:**
- **Level** (double): Balance position (-1.0 = full left, 0.0 = center, +1.0 = full right)

**Typical Usage:**
```csharp
var effect = new BalanceAudioEffect(0.3); // Slightly right
```

### Wide Stereo
**Audio Element:** `stereo`

Enhances stereo width.

**Properties:**
- **Level** (float): Widening amount (0.01 - 1.0)

**Typical Usage:**
```csharp
var effect = new WideStereoAudioEffect();
effect.Level = 0.05f; // Subtle widening
```

### Karaoke
**Audio Element:** `audiokaraoke`

Removes or reduces center-panned vocals.

**Properties:**
- **FilterBand** (float): Center frequency (Hz)
- **FilterWidth** (float): Frequency width (Hz)
- **Level** (float): Effect strength (0.0 - 1.0)
- **MonoLevel** (float): Center channel suppression (0.0 - 1.0)

**Typical Usage:**
```csharp
var effect = new KaraokeAudioEffect();
effect.FilterBand = 220;
effect.FilterWidth = 100;
```

## Delay and Modulation Effects

### Echo
**Audio Element:** `audioecho`

Creates echo/delay effects.

**Properties:**
- **Delay** (TimeSpan): Echo delay time
- **MaxDelay** (TimeSpan): Maximum delay buffer size
- **Intensity** (float): Echo volume (0.0 - 1.0)
- **Feedback** (float): Echo repetitions (0.0 - 1.0)

**Typical Usage:**
```csharp
var effect = new EchoAudioEffect(TimeSpan.FromMilliseconds(250));
effect.Intensity = 0.6f;
effect.Feedback = 0.3f;
```

### Reverberation
**Audio Element:** `freeverb`

Adds room acoustics and spatial depth.

**Properties:**
- **RoomSize** (float): Size of simulated room (0.0 - 1.0)
- **Damping** (float): High frequency damping (0.0 - 1.0)
- **Level** (float): Wet/dry mix (0.0 - 1.0)
- **Width** (float): Stereo width (0.0 - 1.0)

**Typical Usage:**
```csharp
var effect = new ReverberationAudioEffect();
effect.RoomSize = 0.7f; // Large room
effect.Level = 0.4f;
```

### Flanger
Creates sweeping "jet plane" effect.

**Properties:**
- **Delay** (TimeSpan): Base delay (1-10 ms typical)
- **Frequency** (float): LFO rate (0.1-2 Hz)
- **PhaseInvert** (bool): Invert phase of delayed signal

**Typical Usage:**
```csharp
var effect = new FlangerAudioEffect(
    TimeSpan.FromMilliseconds(3), // Delay
    0.5f,                         // Frequency
    false);                       // Phase invert
```

### Phaser
Creates phase-shifting modulation effect.

**Properties:**
- **Depth** (byte): Effect depth (0-255)
- **Frequency** (float): LFO frequency
- **Stages** (byte): Number of stages (2-24)
- **Feedback** (byte): Feedback amount
- **DryWetRatio** (byte): Mix ratio (0-255)
- **StartPhase** (float): LFO start phase (radians)

## Equalization and Filtering

### 10-Band Equalizer
**Audio Element:** `equalizer-10bands`

Graphic EQ with fixed frequencies.

**Band Frequencies:**
- Band 0: 29 Hz (Sub-bass)
- Band 1: 59 Hz (Bass)
- Band 2: 119 Hz (Upper bass)
- Band 3: 237 Hz (Low midrange)
- Band 4: 474 Hz (Midrange)
- Band 5: 947 Hz (Upper midrange)
- Band 6: 1889 Hz (Presence)
- Band 7: 3770 Hz (Upper presence)
- Band 8: 7523 Hz (Brilliance)
- Band 9: 15011 Hz (Air)

**Gain Range:** -24 dB to +12 dB per band

**Typical Usage:**
```csharp
double[] levels = new double[10] { 0, 3, 2, 0, -1, 0, 2, 1, 0, -1 };
var effect = new Equalizer10AudioEffect(levels);
```

### Parametric Equalizer
Adjustable frequency, Q, and gain per band.

**Properties:**
- **Bands**: Array of ParametricEqualizerBand (1-64 bands)
  - Frequency (Hz)
  - Q (bandwidth)
  - Gain (dB)

**Typical Usage:**
```csharp
var effect = new EqualizerParametricAudioEffect(3);
effect.Bands[0].Frequency = 200;
effect.Bands[0].Q = 1.5;
effect.Bands[0].Gain = -3;
effect.Update();
```

### High-Pass Filter
**Audio Element:** `audiocheblimit` (Chebyshev) or simple filter

Attenuates frequencies below cutoff.

**Properties:**
- **CutOff** (uint): Cutoff frequency in Hz

**Common Frequencies:**
- 20-40 Hz: Sub-sonic removal
- 60-80 Hz: Rumble removal
- 100-150 Hz: Clarity improvement

### Low-Pass Filter
Attenuates frequencies above cutoff.

**Properties:**
- **CutOff** (uint): Cutoff frequency in Hz

**Common Frequencies:**
- 15000-20000 Hz: Gentle air reduction
- 8000-10000 Hz: Warmth
- 3000-5000 Hz: Telephone effect

### Band-Pass Filter
Passes frequencies within a range.

**Properties:**
- **CutOffLow** (float): Lower frequency limit
- **CutOffHigh** (float): Upper frequency limit

### Notch Filter
Attenuates a narrow frequency band.

**Properties:**
- **CutOff** (uint): Center frequency to attenuate

**Typical Usage:** Remove hum at 50/60 Hz or feedback frequencies.

### Chebyshev Band Pass/Reject
**Audio Element:** `audiochebband`

Steep filter with configurable characteristics.

**Properties:**
- **Mode**: Band-pass or Band-reject
- **LowerFrequency** (float): Lower bound (Hz)
- **UpperFrequency** (float): Upper bound (Hz)
- **Poles** (int): Filter order (2-12)
- **Type** (int): Type I or Type II (1 or 2)
- **Ripple** (float): Ripple amount (dB)

### Chebyshev Limit (Low-Pass/High-Pass)
**Audio Element:** `audiocheblimit`

Steep low-pass or high-pass filter.

**Properties:**
- **Mode**: Low-pass or High-pass
- **CutOffFrequency** (float): Cutoff (Hz)
- **Poles** (int): Filter order (2-12)
- **Type** (int): Type I or Type II (1 or 2)
- **Ripple** (float): Ripple amount (dB)

## Dynamic Processing

### Compressor/Expander
**Audio Element:** `audiodynamic`

Dynamic range control.

**Properties:**
- **Mode**: Compressor or Expander
- **Ratio** (double): Compression/expansion ratio
- **Threshold** (double): Activation level (0.0 - 1.0)
- **Characteristics**: Hard knee or Soft knee

**Typical Usage:**
```csharp
var effect = new CompressorExpanderAudioEffect();
effect.Mode = AudioCompressorMode.Compressor;
effect.Ratio = 4.0;  // 4:1 compression
effect.Threshold = 0.6;
effect.Characteristics = AudioDynamicCharacteristics.SoftKnee;
```

## Pitch and Tempo

### Pitch Shift
Uses SoundTouch algorithm to change pitch without affecting tempo.

**Properties:**
- **Pitch** (float): Pitch multiplier (0.5 = octave down, 2.0 = octave up)

**Musical Intervals:**
- 0.5 = -12 semitones
- 1.059 = +1 semitone
- 1.122 = +2 semitones
- 2.0 = +12 semitones

### Scale Tempo
**Audio Element:** `scaletempo`

Changes tempo without affecting pitch (WSOLA algorithm).

**Properties:**
- **Rate** (double): Speed multiplier (0.5 = half speed, 2.0 = double speed)
- **Stride** (TimeSpan): Segment length (default 30ms)
- **Overlap** (double): Overlap percentage (default 0.2)
- **Search** (TimeSpan): Search window (default 14ms)

**Typical Usage:**
```csharp
var effect = new ScaleTempoAudioEffect(1.5); // 50% faster
effect.Stride = TimeSpan.FromMilliseconds(25);
```

### Tempo
Simple tempo change (may affect pitch).

## Frequency Enhancement

### TrueBass
Enhances low frequencies.

**Properties:**
- **Frequency** (int): Upper frequency limit (Hz)
- **Volume** (ushort): Amplification amount (0-10000)

**Typical Usage:**
```csharp
var effect = new TrueBassAudioEffect(100, 5000); // Boost up to 100 Hz
```

### Treble Enhancer
Enhances high frequencies.

**Properties:**
- **Frequency** (int): Lower frequency limit (Hz)
- **Volume** (ushort): Amplification amount (0-10000)

## Advanced Effects

### Dynamic Amplify
Automatic level adjustment.

### Phase Invert
Inverts audio phase.

**Usage:** Useful for phase cancellation effects or correcting phase issues.

### Sound 3D
Creates 3D spatial effect.

**Properties:**
- **Value** (uint): 3D amplification (1-20000, 1000 = bypass)

### Channel Order
Reorders audio channels.

### Down Mix
Converts multi-channel to fewer channels.

### Fade
Fade-in/fade-out effects.

## Noise Reduction and Cleanup

### Remove Silence
**Audio Element:** Custom implementation

Automatically removes silent sections.

**Properties:**
- **Threshold** (double): Silence detection level (0.0 - 1.0)
- **Squash** (bool): Remove completely (true) or reduce level (false)

**Typical Usage:**
```csharp
var effect = new RemoveSilenceAudioEffect("remove-silence");
effect.Threshold = 0.05;
effect.Squash = true;
```

### Audio RNNoise
**Audio Element:** `audiornnoise` (rsaudiofx plugin)

RNN-based noise reduction.

**Properties:**
- **VadThreshold** (float): Voice activity detection threshold (0.0 - 1.0)

**Typical Usage:**
```csharp
var effect = new AudioRNNoiseAudioEffect(0.5f);
```

### Audio Loud Norm
**Audio Element:** `audioloudnorm` (rsaudiofx plugin)

EBU R128 loudness normalization.

**Properties:**
- **LoudnessTarget** (double): Target in LUFS (-70.0 to -5.0, default -24.0)
- **LoudnessRangeTarget** (double): Range in LU (1.0 to 20.0, default 7.0)
- **MaxTruePeak** (double): Max peak in dbTP (-9.0 to 0.0, default -2.0)
- **Offset** (double): Offset gain in LU (-99.0 to 99.0, default 0.0)

## Analysis and Measurement

### EBU R128 Level
**Audio Element:** `ebur128level` (rsaudiofx plugin)

Measures loudness according to EBU R128 standard.

**Output:**
- Momentary loudness (LUFS)
- Short-term loudness (LUFS)
- Integrated loudness (LUFS)

### HRTF Render
**Audio Element:** `hrtfrender` (rsaudiofx plugin)

Head-Related Transfer Function spatial audio.

**Properties:**
- **Azimuth** (double): Horizontal direction (degrees)
- **Elevation** (double): Vertical direction (degrees)

## Specialized Effects

### RS Audio Echo
**Audio Element:** `rsaudioecho` (rsaudiofx plugin)

High-quality echo from rsaudiofx plugin.

**Properties:**
- **Delay** (int): Delay in milliseconds
- **Intensity** (float): Echo intensity (0-1)
- **Feedback** (float): Feedback amount (0-1)

### Csound Filter
**Audio Element:** `csoundfilter`

Advanced audio synthesis using Csound.

**Properties:**
- **CsdPath** (string): Path to Csound script file (.csd)

**Platform:** Requires Csound installation.

## DirectSound Effects (Classic SDKs - Windows Only)

The following effects are available only in Video Capture SDK, Media Player SDK, and Video Edit SDK on Windows. They use DirectSound/DirectX technology.

### DS Chorus
**Availability:** ❌ Media Blocks | ✅ Classic SDKs (Windows only)  
**Implementation:** DirectSound DMO (Dynamic Media Object)

Creates a chorus effect with multiple delayed and modulated copies.

**Properties:**
- **WetDryMix** (float): Dry/wet mix (0-100)
- **Depth** (float): Modulation depth (0-100)
- **Feedback** (float): Feedback amount (0-100)
- **Frequency** (float): LFO frequency (0-10 Hz)
- **Waveform**: Sine or Triangle
- **Delay** (float): Base delay (0-20 ms)
- **Phase**: Phase relationship for stereo (-180 to 180 degrees)

**Typical Usage:**
```csharp
videoCaptureCore.Audio_Effects_DS_Chorus(0, "chorus", true,
    wetDryMix: 50, depth: 25, feedback: 25, frequency: 1.1f,
    waveform: DSChorusWaveForm.Sine, delay: 16, phase: DSChorusPhase.Phase90);
```

### DS Distortion
**Availability:** ❌ Media Blocks | ✅ Classic SDKs (Windows only)  
**Implementation:** DirectSound DMO

Adds distortion/overdrive to audio signal.

**Properties:**
- **Gain** (float): Pre-distortion gain (-60 to 0 dB)
- **Edge** (float): Distortion amount (0-100%)
- **PostEQCenterFrequency** (float): Post-EQ center (100-8000 Hz)
- **PostEQBandwidth** (float): Post-EQ bandwidth (100-8000 Hz)
- **PreLowpassCutoff** (float): Pre-distortion lowpass (100-8000 Hz)

**Typical Usage:**
```csharp
videoCaptureCore.Audio_Effects_DS_Distortion(0, "distortion", true,
    gain: -18, edge: 50, postEQCenterFrequency: 2400,
    postEQBandwidth: 2400, preLowpassCutoff: 8000);
```

### DS Gargle
**Availability:** ❌ Media Blocks | ✅ Classic SDKs (Windows only)  
**Implementation:** DirectSound DMO

Creates a gargling/tremolo modulation effect.

**Properties:**
- **RateHz** (uint): Modulation rate (1-1000 Hz)
- **WaveShape**: Triangle or Square wave

**Typical Usage:**
```csharp
videoCaptureCore.Audio_Effects_DS_Gargle(0, "gargle", true,
    rateHz: 20, waveShape: DSGargleWaveForm.Triangle);
```

### DS Reverb (I3DL2)
**Availability:** ❌ Media Blocks | ✅ Classic SDKs (Windows only)  
**Implementation:** DirectSound I3DL2 Reverb DMO

Professional reverb with environmental modeling.

**Properties:**
- **Room** (int): Room effect level (-10000 to 0 mB)
- **RoomHF** (int): High-frequency room effect (-10000 to 0 mB)
- **RoomRolloffFactor** (float): Rolloff factor (0 to 10)
- **DecayTime** (float): Decay time (0.1 to 20 seconds)
- **DecayHFRatio** (float): HF decay ratio (0.1 to 2.0)
- **Reflections** (int): Early reflections (-10000 to 1000 mB)
- **ReflectionsDelay** (float): Reflections delay (0 to 0.3 seconds)
- **Reverb** (int): Late reverb level (-10000 to 2000 mB)
- **ReverbDelay** (float): Reverb delay (0 to 0.1 seconds)
- **Diffusion** (float): Diffusion (0 to 100%)
- **Density** (float): Density (0 to 100%)
- **HFReference** (float): Reference HF (20 to 20000 Hz)

### DS Waves Reverb
**Availability:** ❌ Media Blocks | ✅ Classic SDKs (Windows only)  
**Implementation:** DirectSound Waves Reverb DMO

Simplified reverb with basic parameters.

**Properties:**
- **InGain** (float): Input gain (0 to 96 dB)
- **ReverbMix** (float): Reverb mix (0 to 96 dB)
- **ReverbTime** (float): Reverb time (0.001 to 3000 ms)
- **HighFreqRTRatio** (float): HF reverb time ratio (0.001 to 0.999)

**Typical Usage:**
```csharp
videoCaptureCore.Audio_Effects_DS_WavesReverb(0, "reverb", true,
    inGain: 0, reverbMix: -10, reverbTime: 1000, highFreqRTRatio: 0.001f);
```

## Platform Availability

Most audio effects are available on:
- Windows
- macOS
- Linux
- iOS
- Android

However, there are important differences between SDK types:

### Media Blocks SDK / VideoCaptureCoreX / MediaPlayerCoreX (Cross-Platform)
All cross-platform effects are available on all platforms listed above.

### Classic SDKs (Windows Only)
Video Capture SDK, Media Player SDK, and Video Edit SDK use Windows-only DSP effects.

**Platform-specific effects:**
- Csound Filter: Windows, macOS, Linux only (requires Csound)
- RS Audio effects: Requires rsaudiofx plugin
- DirectSound effects (Chorus, Distortion, Gargle, Reverb, Waves Reverb): Windows only, Classic SDKs only

## Effects Availability Matrix

| Effect | Cross-Platform SDKs | Classic SDKs | Platforms |
|--------|---------------------|--------------|-----------|
| **Volume/Level Control** |
| Volume | ✅ | ✅ | Cross-platform / Windows |
| Amplify | ✅ | ✅ | Cross-platform / Windows |
| **Stereo Processing** |
| Balance | ✅ | ❌ | Cross-platform |
| Wide Stereo | ✅ | ❌ | Cross-platform |
| Karaoke | ✅ | ❌ | Cross-platform |
| **Delay and Modulation** |
| Echo | ✅ | ✅ | Cross-platform / Windows |
| Reverberation (Freeverb) | ✅ | ❌ | Cross-platform |
| Flanger | ✅ | ✅ | Cross-platform / Windows |
| Phaser | ✅ | ✅ | Cross-platform / Windows |
| **Pitch and Tempo** |
| Pitch Shift | ✅ | ✅ | Cross-platform / Windows |
| Scale Tempo | ✅ | ❌ | Cross-platform |
| Tempo | ✅ | ✅ | Cross-platform / Windows |
| **Equalization** |
| Equalizer 10-band | ✅ | ❌ | Cross-platform |
| Equalizer Parametric | ✅ | ✅ | Cross-platform / Windows |
| **Filtering** |
| High-Pass | ✅ | ✅ | Cross-platform / Windows |
| Low-Pass | ✅ | ✅ | Cross-platform / Windows |
| Band-Pass | ✅ | ✅ | Cross-platform / Windows |
| Notch | ✅ | ✅ | Cross-platform / Windows |
| Chebyshev Band Pass/Reject | ✅ | ❌ | Cross-platform |
| Chebyshev Limit | ✅ | ❌ | Cross-platform |
| **Dynamic Processing** |
| Compressor/Expander | ✅ | ❌ | Cross-platform |
| Dynamic Amplify | ✅ | ✅ | Cross-platform / Windows |
| **Frequency Enhancement** |
| TrueBass | ✅ | ✅ | Cross-platform / Windows |
| Treble Enhancer | ✅ | ✅ | Cross-platform / Windows |
| **Advanced Effects** |
| Phase Invert | ✅ | ✅ | Cross-platform / Windows |
| Sound 3D | ✅ | ✅ | Cross-platform / Windows |
| Channel Order | ✅ | ✅ | Cross-platform / Windows |
| Down Mix | ✅ | ✅ | Cross-platform / Windows |
| Fade | ✅ | ✅ | Cross-platform / Windows |
| **Noise Reduction** |
| Remove Silence | ✅ | ❌ | Cross-platform |
| Audio RNNoise | ✅ | ❌ | Cross-platform (requires plugin) |
| Audio Loud Norm | ✅ | ❌ | Cross-platform (requires plugin) |
| **Analysis** |
| EBU R128 Level | ✅ | ❌ | Cross-platform (requires plugin) |
| **Spatial Audio** |
| HRTF Render | ✅ | ❌ | Cross-platform (requires plugin) |
| **Specialized** |
| RS Audio Echo | ✅ | ❌ | Cross-platform (requires plugin) |
| Csound Filter | ✅ | ❌ | Windows/macOS/Linux (requires Csound) |
| **DirectSound Effects (Windows Classic Only)** |
| DS Chorus | ❌ | ✅ | Windows only |
| DS Distortion | ❌ | ✅ | Windows only |
| DS Gargle | ❌ | ✅ | Windows only |
| DS Reverb (I3DL2) | ❌ | ✅ | Windows only |
| DS Waves Reverb | ❌ | ✅ | Windows only |

**Legend:**
- ✅ = Available
- ❌ = Not available
- **Cross-platform SDKs** = Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **Classic SDKs** = Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore) - Windows only

## Audio Elements Reference

| Effect | Audio Element | Plugin |
|--------|------------------|---------|
| Volume | volume | coreelements |
| Amplify | audioamplify | audiofx |
| Balance | audiopanorama | audiofx |
| Echo | audioecho | audiofx |
| Karaoke | audiokaraoke | audiofx |
| Wide Stereo | stereo | audiofx |
| Reverberation | freeverb | freeverb |
| Equalizer 10-band | equalizer-10bands | audiofx |
| High-Pass | audiocheblimit | audiofx |
| Low-Pass | audiocheblimit | audiofx |
| Chebyshev Band | audiochebband | audiofx |
| Chebyshev Limit | audiocheblimit | audiofx |
| Compressor | audiodynamic | audiofx |
| Scale Tempo | scaletempo | audiofx |
| Pitch Shift | pitch | soundtouch |
| Audio RNNoise | audiornnoise | rsaudiofx |
| Audio Loud Norm | audioloudnorm | rsaudiofx |
| EBU R128 Level | ebur128level | rsaudiofx |
| RS Audio Echo | rsaudioecho | rsaudiofx |
| HRTF Render | hrtfrender | rsaudiofx |
| Csound Filter | csoundfilter | csound |

## See Also

- [Audio Processing and Effects](index.md) - Block usage examples
- [Audio Elements Documentation](https://gstreamer.freedesktop.org/documentation/plugins_doc.html)
- [rsaudiofx Plugin](https://github.com/sdroege/gst-plugin-rs/tree/main/audio/audiofx)
