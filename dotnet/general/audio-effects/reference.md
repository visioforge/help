---
title: Audio Effects API Reference for .NET - Parameters & Examples
description: API reference for 30+ audio effects in VisioForge .NET SDKs. Volume, EQ, compressor, reverb, echo, filters, pitch shift, and noise reduction with C# examples.
sidebar_label: Audio Effects Reference
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Streaming
  - Editing
  - Effects
  - C#
primary_api_classes:
  - VolumeAudioEffect
  - Equalizer10AudioEffect
  - BandPassAudioEffect
  - BalanceAudioEffect
  - WideStereoAudioEffect

---

# Audio Effects API Reference

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Complete parameter reference for all audio effects available in VisioForge .NET SDKs. Each cross-platform effect wraps a GStreamer element and supports real-time parameter changes from C# code during playback. Cross-platform effects work on Windows, macOS, Linux, iOS, and Android.

For an overview of effect categories and usage patterns, see [Audio Effects](index.md).

## Volume and Dynamics Effects

### VolumeAudioEffect

**GStreamer Element**: `volume`

**Purpose**: Control audio volume level with optional mute.

**Parameters**:

- `Level` (double): Volume multiplier
    - Range: 0.0 to unlimited
    - Default: 1.0 (100%)
    - Examples: 0.5 = 50%, 2.0 = 200%
- `Mute` (bool): Mute audio output
    - Default: false

**Usage**:

```csharp
var effect = new VolumeAudioEffect(1.5); // 150% volume
effect.Mute = true; // Temporarily mute
```

---

### AmplifyAudioEffect

**GStreamer Element**: `audioamplify`

**Purpose**: Amplify audio with clipping control.

**Parameters**:

- `Level` (double): Amplification level
    - Range: 1.0 to 10.0
    - Default: 1.0
- `ClippingMethod` (AmplifyClippingMethod): How to handle peaks
    - Options: Normal, WrapNegative, WrapPositive, NoClip
    - Default: Normal

**Usage**:

```csharp
var effect = new AmplifyAudioEffect(2.0);
effect.ClippingMethod = AmplifyClippingMethod.NoClip;
```

---

### CompressorExpanderAudioEffect

**GStreamer Element**: `audiodynamic`

**Purpose**: Dynamic range compression or expansion.

**Parameters**:

- `Threshold` (double): Activation threshold
    - Range: 0.0 to 1.0
    - Default: 0.0
- `Ratio` (double): Compression/expansion ratio
    - Range: 1.0+
    - Default: 1.0
    - Typical: 2:1 to 10:1 for compression
- `Mode` (AudioCompressorMode): Compressor or Expander
    - Default: Compressor
- `Characteristics` (AudioDynamicCharacteristics): HardKnee or SoftKnee
    - Default: SoftKnee

**Usage**:

```csharp
var effect = new CompressorExpanderAudioEffect();
effect.Threshold = 0.5;
effect.Ratio = 4.0; // 4:1 compression
effect.Characteristics = AudioDynamicCharacteristics.SoftKnee;
```

---

### DynamicAmplifyAudioEffect

**Purpose**: Adaptive gain control.

**Parameters**:

- `AttackTime` (uint): Response time in ms
    - Typical: 10-100 ms
- `MaxAmplification` (uint): Maximum gain
    - 10000 = 1x (no change)
    - 20000 = 2x amplification
    - Default: 10000
- `ReleaseTime` (TimeSpan): Time before resuming amplification
    - Typical: 100-1000 ms

**Usage**:

```csharp
var effect = new DynamicAmplifyAudioEffect(50, 20000, TimeSpan.FromMilliseconds(500));
```

---

## Equalization Effects

### Equalizer10AudioEffect

**GStreamer Element**: `equalizer-10bands`

**Purpose**: 10-band graphic equalizer with fixed frequencies.

**Frequency Bands**:

1. 29 Hz (Sub-bass)
2. 59 Hz (Bass)
3. 119 Hz (Bass)
4. 237 Hz (Low midrange)
5. 474 Hz (Midrange)
6. 947 Hz (Midrange)
7. 1889 Hz (Upper midrange)
8. 3770 Hz (Presence)
9. 7523 Hz (Brilliance)
10. 15011 Hz (Air)

**Parameters**:

- `Levels` (double[]): Gain for each band in dB
    - Range per band: -24 to +12 dB
    - Array must contain exactly 10 values

**Usage**:

```csharp
var levels = new double[] {
    3.0,   // 29 Hz: +3 dB
    2.0,   // 59 Hz: +2 dB
    0.0,   // 119 Hz: 0 dB (no change)
    -2.0,  // 237 Hz: -2 dB
    0.0,   // 474 Hz
    0.0,   // 947 Hz
    1.0,   // 1889 Hz: +1 dB
    2.0,   // 3770 Hz: +2 dB
    3.0,   // 7523 Hz: +3 dB
    4.0    // 15011 Hz: +4 dB
};
var effect = new Equalizer10AudioEffect(levels);
```

---

### EqualizerParametricAudioEffect

**GStreamer Element**: `equalizer-nbands`

**Purpose**: Parametric equalizer with configurable bands.

**Parameters**:

- `Bands` (ParametricEqualizerBand[]): Array of bands
    - Count: 1 to 64 bands
    - Each band: Frequency, Gain, Width (bandwidth in Hz)

**Usage**:

```csharp
var effect = new EqualizerParametricAudioEffect(3);
effect.Bands[0].Frequency = 100;  // Hz
effect.Bands[0].Gain = -6;        // dB
effect.Bands[0].Width = 1.0f;     // bandwidth
// Configure other bands...
effect.Update(); // Apply changes
```

---

### TrebleEnhancerAudioEffect

**Purpose**: Boost high frequencies.

**Parameters**:

- `Frequency` (int): Starting frequency in Hz
    - Typical: 4000-8000 Hz
    - Frequencies above this are boosted
- `Volume` (ushort): Boost amount
    - Range: 0 to 10000
    - 0 = no effect

**Usage**:

```csharp
var effect = new TrebleEnhancerAudioEffect(6000, 5000);
```

---

### TrueBassAudioEffect

**Purpose**: Boost low frequencies.

**Parameters**:

- `Frequency` (int): Upper frequency limit in Hz
    - Typical: 100-300 Hz
    - Frequencies below this are boosted
- `Volume` (ushort): Boost amount
    - Range: 0 to 10000
    - 0 = no effect

**Usage**:

```csharp
var effect = new TrueBassAudioEffect(150, 5000);
```

---

## Filter Effects

### HighPassAudioEffect

**Implementation**: Custom DSP (IIR high-pass filter)

**Purpose**: Remove low frequencies.

**Parameters**:

- `CutOff` (uint): Cutoff frequency in Hz
    - Frequencies below are attenuated
    - Typical: 80-200 Hz for voice, 40 Hz for music

**Common Frequencies**:

- 20-40 Hz: Sub-sonic removal
- 60-80 Hz: Rumble removal
- 100-150 Hz: Clarity improvement

**Usage**:

```csharp
var effect = new HighPassAudioEffect(100); // Remove frequencies below 100 Hz
```

---

### LowPassAudioEffect

**Implementation**: Custom DSP (IIR low-pass filter)

**Purpose**: Remove high frequencies.

**Parameters**:

- `CutOff` (uint): Cutoff frequency in Hz
    - Frequencies above are attenuated
    - Typical: 8000-12000 Hz for hiss removal

**Common Frequencies**:

- 15000-20000 Hz: Gentle air reduction
- 8000-10000 Hz: Warmth
- 3000-5000 Hz: Telephone effect

**Usage**:

```csharp
var effect = new LowPassAudioEffect(10000); // Remove frequencies above 10 kHz
```

---

### BandPassAudioEffect

**Implementation**: Custom DSP (state-variable filter)

**Purpose**: Allow only specific frequency range.

**Parameters**:

- `CutOffHigh` (float): Upper frequency boundary in Hz
- `CutOffLow` (float): Lower frequency boundary in Hz

**Usage**:

```csharp
// Constructor: BandPassAudioEffect(cutOffHigh, cutOffLow)
var effect = new BandPassAudioEffect(5000, 300); // Allow 300-5000 Hz
```

---

### NotchAudioEffect

**Implementation**: Custom DSP (band-reject filter)

**Purpose**: Remove specific frequency.

**Parameters**:

- `CutOff` (uint): Center frequency to remove in Hz
    - Typical: 50/60 Hz for hum removal

**Usage**:

```csharp
var effect = new NotchAudioEffect(60); // Remove 60 Hz hum
```

---

### ChebyshevLimitAudioEffect

**GStreamer Element**: `audiocheblimit`

**Purpose**: Sharp low/high-pass filtering with ripple control.

**Parameters**:

- `CutOffFrequency` (float): Cutoff frequency in Hz
- `Mode` (ChebyshevLimitAudioEffectMode): LowPass or HighPass
- `Poles` (int): Filter order (2-8 typical)
    - Default: 4
    - More poles = steeper rolloff
- `Ripple` (float): Passband ripple in dB
    - Default: 0.25
- `Type` (int): Chebyshev type (1 or 2)
    - Default: 1

**Usage**:

```csharp
var effect = new ChebyshevLimitAudioEffect();
effect.CutOffFrequency = 100;
effect.Mode = ChebyshevLimitAudioEffectMode.HighPass;
effect.Poles = 6;
```

---

### ChebyshevBandPassRejectAudioEffect

**GStreamer Element**: `audiochebband`

**Purpose**: Sharp band-pass or band-reject filtering.

**Parameters**:

- `LowerFrequency` (float): Lower band boundary in Hz
- `UpperFrequency` (float): Upper band boundary in Hz
- `Mode` (ChebyshevBandPassRejectAudioEffectMode): BandPass or BandReject
- `Poles` (int): Filter order (2-8 typical)
    - Default: 4
- `Ripple` (float): Passband ripple in dB
    - Default: 0.25
- `Type` (int): Chebyshev type (1 or 2)
    - Default: 1

**Usage**:

```csharp
var effect = new ChebyshevBandPassRejectAudioEffect();
effect.LowerFrequency = 300;
effect.UpperFrequency = 3000;
effect.Mode = ChebyshevBandPassRejectAudioEffectMode.BandPass;
```

---

## Spatial and Stereo Effects

### BalanceAudioEffect

**GStreamer Element**: `audiopanorama`

**Purpose**: Stereo balance (pan) control.

**Parameters**:

- `Level` (double): Balance position
    - Range: -1.0 to 1.0
    - -1.0 = full left
    - 0.0 = center
    - 1.0 = full right
    - Default: 0.0

**Usage**:

```csharp
var effect = new BalanceAudioEffect(-0.5); // Pan 50% left
```

---

### WideStereoAudioEffect

**GStreamer Element**: `stereo`

**Purpose**: Enhance stereo width.

**Parameters**:

- `Level` (float): Widening intensity
    - Range: 0.0+
    - Default: 0.01
    - Typical: 0.01 to 1.0
    - Higher values = wider stereo field

**Usage**:

```csharp
var effect = new WideStereoAudioEffect();
effect.Level = 0.5f;
```

---

### Sound3DAudioEffect

**Purpose**: 3D spatial audio simulation.

**Parameters**:

- `Value` (uint): Spatial amplification
    - Range: 1 to 20000
    - 1000 = neutral (disabled)
    - < 1000 = narrower stereo
    - > 1000 = wider stereo
    - > 10000 may distort

**Usage**:

```csharp
var effect = new Sound3DAudioEffect(2000); // 2x spatial width
```

---

### PhaseInvertAudioEffect

**Purpose**: Invert audio phase by 180 degrees.

**Parameters**: None.

**Usage**:

```csharp
var effect = new PhaseInvertAudioEffect();
```

---

### HRTFRenderAudioEffect

**GStreamer Element**: `hrtfrender` (rsaudiofx)

**Purpose**: HRTF-based 3D spatial audio.

**Parameters**:

- `HrirFile` (string): Path to HRIR data file
- `InterpolationSteps` (ulong): Interpolation quality
    - Default: 8
- `BlockLength` (ulong): Processing block size
    - Default: 512
- `DistanceGain` (float): Distance attenuation factor
    - Default: 1.0

**Usage**:

```csharp
var effect = new HRTFRenderAudioEffect("/path/to/hrir.dat");
effect.InterpolationSteps = 16; // Higher quality
```

---

## Time-Based Effects

### EchoAudioEffect

**GStreamer Element**: `audioecho`

**Purpose**: Echo and delay effects.

**Parameters**:

- `Delay` (TimeSpan): Echo delay time
    - Must not exceed MaxDelay
- `MaxDelay` (TimeSpan): Maximum delay buffer
    - Must be >= Delay
    - Set before starting playback
- `Intensity` (float): Echo volume
    - Range: 0.0 to 1.0
    - Default: 1.0
- `Feedback` (float): Echo repetition amount
    - Range: 0.0 to 1.0
    - Default: 0.0
    - Higher = more echoes

**Usage**:

```csharp
var delay = TimeSpan.FromMilliseconds(500);
var effect = new EchoAudioEffect(delay);
effect.Intensity = 0.7f;
effect.Feedback = 0.4f;
```

---

### RSAudioEchoAudioEffect

**GStreamer Element**: `rsaudioecho` (rsaudiofx)

**Purpose**: Enhanced echo with advanced controls.

**Parameters**:

- `Delay` (TimeSpan): Echo delay time
- `MaxDelay` (TimeSpan): Maximum delay buffer
- `Intensity` (double): Echo intensity
    - Range: 0.0 to 1.0
- `Feedback` (double): Feedback amount
    - Range: 0.0 to 1.0

**Usage**:

```csharp
var effect = new RSAudioEchoAudioEffect();
effect.Delay = TimeSpan.FromMilliseconds(750);
effect.Intensity = 0.6;
effect.Feedback = 0.3;
```

---

### ReverberationAudioEffect

**GStreamer Element**: `freeverb`

**Purpose**: Room reverberation simulation.

**Parameters**:

- `RoomSize` (float): Virtual room size
    - Range: 0.0 to 1.0
    - Default: 0.5
    - Larger = longer reverb tail
- `Damping` (float): High-frequency absorption
    - Range: 0.0 to 1.0
    - Default: 0.2
    - Higher = darker reverb
- `Level` (float): Wet/dry mix
    - Range: 0.0 to 1.0
    - Default: 0.5
    - 0 = dry, 1 = wet
- `Width` (float): Stereo width
    - Range: 0.0 to 1.0
    - Default: 1.0
    - 0 = mono, 1 = full stereo

**Usage**:

```csharp
var effect = new ReverberationAudioEffect();
effect.RoomSize = 0.8f; // Large room
effect.Damping = 0.3f;
effect.Level = 0.4f;
```

---

### FadeAudioEffect

**Purpose**: Volume fade in/out automation.

**Parameters**:

- `StartVolume` (uint): Volume at start time
- `StopVolume` (uint): Volume at stop time
- `StartTime` (TimeSpan): When fade begins
- `StopTime` (TimeSpan): When fade completes

**Usage**:

```csharp
// Fade out over 3 seconds starting at 10 seconds
var effect = new FadeAudioEffect(
    100, 0,
    TimeSpan.FromSeconds(10),
    TimeSpan.FromSeconds(13)
);
```

---

## Modulation Effects

### PhaserAudioEffect

**Purpose**: Phaser effect with LFO modulation.

**Parameters**:

- `Depth` (byte): Sweep depth
    - Range: 0 to 255
- `DryWetRatio` (byte): Mix ratio
    - Range: 0 to 255
    - 0 = dry, 255 = wet
- `Feedback` (byte): Resonance
    - Range: -100 to 100
- `Frequency` (float): LFO speed in Hz
    - Typical: 0.1 to 5 Hz
- `Stages` (byte): Number of stages
    - Range: 2 to 24 recommended
    - More = stronger effect
- `StartPhase` (float): LFO start phase in radians

**Usage**:

```csharp
var effect = new PhaserAudioEffect(
    150,      // depth
    128,      // 50% mix
    50,       // feedback
    0.5f,     // 0.5 Hz LFO
    6,        // 6 stages
    0f        // start phase
);
```

---

### FlangerAudioEffect

**Purpose**: Flanging effect with delay modulation.

**Parameters**:

- `Delay` (TimeSpan): Base delay time
    - Typical: 1-15 ms
- `Frequency` (float): LFO speed in Hz
    - Typical: 0.1 to 5 Hz
- `PhaseInvert` (bool): Invert delayed signal phase
    - Default: false

**Usage**:

```csharp
var effect = new FlangerAudioEffect(
    TimeSpan.FromMilliseconds(5),
    1.0f,
    false
);
```

---

## Pitch and Tempo Effects

### PitchShiftAudioEffect

**Purpose**: Change pitch without changing speed.

**Parameters**:

- `Pitch` (float): Pitch shift ratio
    - 1.0 = no change
    - 2.0 = one octave up
    - 0.5 = one octave down
    - Typical range: 0.5 to 2.0

**Musical Intervals**:

- 0.5 = -12 semitones
- 1.059 = +1 semitone
- 1.122 = +2 semitones
- 2.0 = +12 semitones

**Usage**:

```csharp
var effect = new PitchShiftAudioEffect(1.5f); // Up by a fifth
```

---

### ScaleTempoAudioEffect

**GStreamer Element**: `scaletempo`

**Purpose**: Change tempo without changing pitch (WSOLA algorithm).

**Parameters**:

- `Rate` (double): Playback speed
    - 1.0 = normal
    - 2.0 = double speed
    - 0.5 = half speed
    - Default: 1.0
- `Stride` (TimeSpan): Processing stride
    - Default: 30 ms
- `Overlap` (double): Overlap percentage
    - Range: 0.0 to 1.0
    - Default: 0.2
- `Search` (TimeSpan): Search window
    - Default: 14 ms

**Usage**:

```csharp
var effect = new ScaleTempoAudioEffect(1.5); // 1.5x speed
```

---

## Special Effects

### KaraokeAudioEffect

**GStreamer Element**: `audiokaraoke`

**Purpose**: Remove center-panned vocals.

**Parameters**:

- `FilterBand` (float): Center frequency in Hz
    - Default: 220 Hz
    - Typical: 80-400 Hz
- `FilterWidth` (float): Filter bandwidth in Hz
    - Default: 100 Hz
- `Level` (float): Effect intensity
    - Range: 0.0 to 1.0
    - Default: 1.0
- `MonoLevel` (float): Mono channel level
    - Range: 0.0 to 1.0
    - Default: 1.0

**Usage**:

```csharp
var effect = new KaraokeAudioEffect();
effect.FilterBand = 250f;
effect.Level = 1.0f;
```

---

### RemoveSilenceAudioEffect

**Purpose**: Automatically remove silent sections.

**Parameters**:

- `Threshold` (double): Silence detection threshold
    - Range: 0.0 to 1.0
    - Default: 0.05
    - Lower = more sensitive
- `Squash` (bool): Remove vs. reduce silence
    - true = remove completely
    - false = reduce level
    - Default: true

**Usage**:

```csharp
var effect = new RemoveSilenceAudioEffect("silence-remover");
effect.Threshold = 0.02;
effect.Squash = true;
```

---

### CsoundAudioEffect

**GStreamer Element**: `csoundfilter`

**Purpose**: Csound-based audio programming.

**Platforms**: Windows, macOS, Linux (requires Csound installation).

**Parameters**:

- `CsdText` (string): Csound CSD document as text
- `Location` (string): Path to CSD file
- `Loop` (bool): Loop score continuously
    - Default: false
- `ScoreOffset` (double): Start time in seconds
    - Default: 0.0

**Usage**:

```csharp
var csd = @"<CsoundSynthesizer>
<CsInstruments>
; Your Csound code here
</CsInstruments>
<CsScore>
; Your score here
</CsScore>
</CsoundSynthesizer>";

var effect = new CsoundAudioEffect("my-csound", csd);
effect.Loop = false;
```

---

## Noise Reduction and Measurement

### AudioRNNoiseAudioEffect

**GStreamer Element**: `audiornnoise` (rsaudiofx)

**Purpose**: AI-based noise reduction.

**Parameters**:

- `VadThreshold` (float): Voice activity detection threshold
    - Range: 0.0 to 1.0
    - Default: 0.0
    - Higher = more sensitive to voice

**Usage**:

```csharp
var effect = new AudioRNNoiseAudioEffect();
effect.VadThreshold = 0.5f;
```

---

### AudioLoudNormAudioEffect

**GStreamer Element**: `audioloudnorm` (rsaudiofx)

**Purpose**: EBU R128 loudness normalization.

**Parameters**:

- `LoudnessTarget` (double): Target loudness in LUFS
    - Range: -70.0 to -5.0
    - Default: -24.0
- `LoudnessRangeTarget` (double): Target range in LU
    - Range: 1.0 to 20.0
    - Default: 7.0
- `MaxTruePeak` (double): Max true peak in dbTP
    - Range: -9.0 to 0.0
    - Default: -2.0
- `Offset` (double): Offset gain in LU
    - Range: -99.0 to 99.0
    - Default: 0.0

**Usage**:

```csharp
var effect = new AudioLoudNormAudioEffect();
effect.LoudnessTarget = -16.0; // Streaming standard
effect.MaxTruePeak = -1.0;
```

---

### EbuR128LevelAudioEffect

**GStreamer Element**: `ebur128level` (rsaudiofx)

**Purpose**: EBU R128 loudness measurement.

**Parameters**:

- `Mode` (EbuR128Mode): Measurement types to calculate
    - Options: Momentary, ShortTerm, Global, LoudnessRange, SamplePeak, TruePeak, All
    - Default: All
- `PostMessages` (bool): Post measurement messages
    - Default: true
- `Interval` (TimeSpan): Measurement update interval
    - Default: 1 second

**Usage**:

```csharp
var effect = new EbuR128LevelAudioEffect();
effect.Mode = EbuR128Mode.All;
effect.Interval = TimeSpan.FromSeconds(0.5);
```

---

## Channel Management

### ChannelOrderAudioEffect

**Purpose**: Remap audio channels.

**Parameters**:

- `Orders` (byte[,]): 2D array of [target, source] pairs
    - Format: [[target0, source0], [target1, source1], ...]
    - Channels are zero-indexed

**Usage**:

```csharp
// Swap left and right channels
var orders = new byte[2, 2] {
    { 0, 1 },  // Output channel 0 gets input channel 1 (right)
    { 1, 0 }   // Output channel 1 gets input channel 0 (left)
};
var effect = new ChannelOrderAudioEffect(orders);
```

---

### DownMixAudioEffect

**Implementation**: Custom DSP (channel averaging)

**Purpose**: Reduce channel count (e.g., 5.1 to stereo).

**Parameters**: None (automatic downmixing).

**Usage**:

```csharp
var effect = new DownMixAudioEffect();
```

---

## DirectSound Effects (Classic SDKs, Windows Only)

The following effects are available only in Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), and Video Edit SDK (VideoEditCore) on Windows. They use DirectSound/DirectX technology.

### DS Chorus

Creates a chorus effect with multiple delayed and modulated copies.

**Properties**:

- **WetDryMix** (float): Dry/wet mix (0-100)
- **Depth** (float): Modulation depth (0-100)
- **Feedback** (float): Feedback amount (0-100)
- **Frequency** (float): LFO frequency (0-10 Hz)
- **Waveform**: Sine or Triangle
- **Delay** (float): Base delay (0-20 ms)
- **Phase**: Phase relationship for stereo (-180 to 180 degrees)

**Usage**:

```csharp
// Signature: (int streamIndex, string name, float delay, float depth,
//             float feedback, float frequency, DSChorusPhase phase,
//             DSChorusWaveForm waveformTriangle, float wetDryMix)
videoCaptureCore.Audio_Effects_DS_Chorus(0, "chorus",
    delay: 16, depth: 25, feedback: 25, frequency: 1.1f,
    phase: DSChorusPhase.Phase90, waveformTriangle: DSChorusWaveForm.Sine,
    wetDryMix: 50);
```

---

### DS Distortion

Adds distortion/overdrive to audio signal.

**Properties**:

- **Gain** (float): Pre-distortion gain (-60 to 0 dB)
- **Edge** (float): Distortion amount (0-100%)
- **PostEQCenterFrequency** (float): Post-EQ center (100-8000 Hz)
- **PostEQBandwidth** (float): Post-EQ bandwidth (100-8000 Hz)
- **PreLowpassCutoff** (float): Pre-distortion lowpass (100-8000 Hz)

**Usage**:

```csharp
// Signature: (int streamIndex, string name, float edge, float gain,
//             float postEQBandwidth, float postEQCenterFrequency,
//             float preLowpassCutOff)
videoCaptureCore.Audio_Effects_DS_Distortion(0, "distortion",
    edge: 50, gain: -18, postEQBandwidth: 2400,
    postEQCenterFrequency: 2400, preLowpassCutOff: 8000);
```

---

### DS Gargle

Creates a gargling/tremolo modulation effect.

**Properties**:

- **RateHz** (int): Modulation rate (1-1000 Hz)
- **WaveForm**: Triangle or Square wave

**Usage**:

```csharp
// Signature: (int streamIndex, string name, int rateHz, DSGargleWaveForm waveForm)
videoCaptureCore.Audio_Effects_DS_Gargle(0, "gargle",
    rateHz: 20, waveForm: DSGargleWaveForm.Triangle);
```

---

### DS Reverb (I3DL2)

Professional reverb with environmental modeling.

**Properties**:

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

---

### DS Waves Reverb

Simplified reverb with basic parameters.

**Properties**:

- **InGain** (float): Input gain (0 to 96 dB)
- **ReverbMix** (float): Reverb mix (0 to 96 dB)
- **ReverbTime** (float): Reverb time (0.001 to 3000 ms)
- **HighFreqRTRatio** (float): HF reverb time ratio (0.001 to 0.999)

**Usage**:

```csharp
// Signature: (int streamIndex, string name, float highFreqRTRatio,
//             float inGain, float reverbMix, float reverbTime)
videoCaptureCore.Audio_Effects_DS_WavesReverb(0, "reverb",
    highFreqRTRatio: 0.001f, inGain: 0, reverbMix: -10, reverbTime: 1000);
```

---

## Effects Availability Matrix

| Effect | Cross-Platform SDKs | Classic SDKs | Platforms |
|--------|---------------------|--------------|-----------|
| **Volume/Level Control** | | | |
| Volume | Yes | Yes | Cross-platform / Windows |
| Amplify | Yes | Yes | Cross-platform / Windows |
| **Stereo Processing** | | | |
| Balance | Yes | No | Cross-platform |
| Wide Stereo | Yes | No | Cross-platform |
| Karaoke | Yes | No | Cross-platform |
| **Delay and Modulation** | | | |
| Echo | Yes | Yes | Cross-platform / Windows |
| Reverberation (Freeverb) | Yes | No | Cross-platform |
| Flanger | Yes | Yes | Cross-platform / Windows |
| Phaser | Yes | Yes | Cross-platform / Windows |
| **Pitch and Tempo** | | | |
| Pitch Shift | Yes | Yes | Cross-platform / Windows |
| Scale Tempo | Yes | No | Cross-platform |
| Tempo | Yes | Yes | Cross-platform / Windows |
| **Equalization** | | | |
| Equalizer 10-band | Yes | No | Cross-platform |
| Equalizer Parametric | Yes | Yes | Cross-platform / Windows |
| **Filtering** | | | |
| High-Pass | Yes | Yes | Cross-platform / Windows |
| Low-Pass | Yes | Yes | Cross-platform / Windows |
| Band-Pass | Yes | Yes | Cross-platform / Windows |
| Notch | Yes | Yes | Cross-platform / Windows |
| Chebyshev Band Pass/Reject | Yes | No | Cross-platform |
| Chebyshev Limit | Yes | No | Cross-platform |
| **Dynamic Processing** | | | |
| Compressor/Expander | Yes | No | Cross-platform |
| Dynamic Amplify | Yes | Yes | Cross-platform / Windows |
| **Frequency Enhancement** | | | |
| TrueBass | Yes | Yes | Cross-platform / Windows |
| Treble Enhancer | Yes | Yes | Cross-platform / Windows |
| **Advanced Effects** | | | |
| Phase Invert | Yes | Yes | Cross-platform / Windows |
| Sound 3D | Yes | Yes | Cross-platform / Windows |
| Channel Order | Yes | Yes | Cross-platform / Windows |
| Down Mix | Yes | Yes | Cross-platform / Windows |
| Fade | Yes | Yes | Cross-platform / Windows |
| **Noise Reduction** | | | |
| Remove Silence | Yes | No | Cross-platform |
| Audio RNNoise | Yes | No | Cross-platform (requires plugin) |
| Audio Loud Norm | Yes | No | Cross-platform (requires plugin) |
| **Analysis** | | | |
| EBU R128 Level | Yes | No | Cross-platform (requires plugin) |
| **Spatial Audio** | | | |
| HRTF Render | Yes | No | Cross-platform (requires plugin) |
| **Specialized** | | | |
| RS Audio Echo | Yes | No | Cross-platform (requires plugin) |
| Csound Filter | Yes | No | Windows/macOS/Linux (requires Csound) |
| **DirectSound Effects (Windows Classic Only)** | | | |
| DS Chorus | No | Yes | Windows only |
| DS Distortion | No | Yes | Windows only |
| DS Gargle | No | Yes | Windows only |
| DS Reverb (I3DL2) | No | Yes | Windows only |
| DS Waves Reverb | No | Yes | Windows only |

**Legend**:

- **Cross-platform SDKs** = Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **Classic SDKs** = Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore) — Windows only

## Audio Elements Reference

| Effect | Audio Element | Plugin |
|--------|---------------|--------|
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

## Frequently Asked Questions

???+ "What is the default value for audio effect parameters?"
    Each effect has documented defaults that represent neutral/bypass behavior. For example, `VolumeAudioEffect` defaults to level 1.0 (100%), `Equalizer10AudioEffect` defaults all bands to 0 dB, and `ReverberationAudioEffect` defaults to a moderate room simulation. See each effect's parameter table above for specific defaults.

???+ "How do I reset an audio effect to its default settings?"
    Create a new instance of the effect with default constructor parameters and call `Audio_Effects_AddOrUpdate()` to replace the current settings. Each effect's default constructor initializes all parameters to their documented defaults.

???+ "Can I use multiple instances of the same effect type?"
    Yes. Effect **names** are used as unique identifiers, not effect types. To run multiple instances of the same type simultaneously, give each instance a distinct name. Calling `Audio_Effects_AddOrUpdate()` with an effect whose name matches an existing one replaces that instance; a new name adds a new instance to the chain.

???+ "What sample rates and channel configurations are supported?"
    All cross-platform audio effects support standard sample rates (8 kHz to 192 kHz) and channel configurations (mono, stereo, multi-channel). The effects automatically adapt to the audio format of the input stream. Some effects like `BalanceAudioEffect` and `WideStereoAudioEffect` require stereo input.

???+ "How do I remove an audio effect during playback?"
    Use the `Audio_Effects_Remove()` method with the effect type to remove it from the processing chain during playback. The change takes effect immediately without interrupting the audio stream.

## See Also

- [Audio Effects Overview](index.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Audio Processing Blocks (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
