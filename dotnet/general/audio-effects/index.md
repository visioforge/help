---
title: Audio Effects in C# and .NET - EQ, Reverb, Filters, and More
description: Apply real-time audio effects in C# and .NET with VisioForge SDKs. Equalizer, reverb, echo, noise reduction, pitch shift, and 30+ effects.
sidebar_label: Audio Effects
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - Media Player SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Streaming
  - Editing
primary_api_classes:
  - VolumeAudioEffect
  - Equalizer10AudioEffect
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - CompressorExpanderAudioEffect

video:
  id: VsFSrYD80XE
  upload_date: 2026-11-20T15:10:00Z
  duration: PT11M57S
---

# Real-Time Audio Effects for .NET Applications

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

VisioForge Media Framework provides over 30 audio effects for real-time audio processing in C# and .NET applications. Built on GStreamer, the cross-platform effects include equalizers, reverb, echo, dynamic compression, filters, pitch shifting, AI-based noise reduction, and more — all running on Windows, macOS, Linux, iOS, and Android.

## Video tutorial

The video builds the same player in Visual Studio: a WPF window with a ten-band equalizer, echo, reverb and pitch, each changed while the file plays.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/VsFSrYD80XE?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

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

- **PitchAudioEffect** — Pitch shifting in semitones without tempo change
- **ScaleTempoAudioEffect** — Keeps the pitch when the playback rate changes

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
- **DS Reverb (I3DL2)** — Professional reverb with environmental modeling (may be missing on Windows 11; DS Waves Reverb is the alternative)
- **DS Waves Reverb** — Simplified reverb

## GStreamer Elements

All cross-platform audio effects run in a GStreamer pipeline. `ChannelOrder`, `DownMix`, `DynamicAmplify`, `Fade`, `Flanger`, `PhaseInvert`, `Phaser`, `Sound3D`, `TrebleEnhancer` and `TrueBass` process the samples in managed code. The others wrap one or more GStreamer elements:

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

## Video Transcript

### What we build

Let's add audio effects to a C# WPF player with the VisioForge Media Player SDK, and change them while the file plays. Here's what we build. A WPF player for video and audio files, with four effects: a ten-band equalizer, echo, reverb and pitch. A checkbox turns each effect on or off, and its sliders change it, all while the file keeps playing. The code behind it is about two hundred and forty lines of C#. This is the finished player. The effects panel sits under the video.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project targets .NET 8 or later. And a music or video file to play. The player part of the app is the same as in our video player tutorial, so this video goes through it quickly.

### Create a WPF project

In Visual Studio, create a new project. Search for WPF, and pick WPF Application for C#. Name the project AudioEffectsWpf. Choose .NET 10 and click Create.

### Add the NuGet packages

Now add four NuGet packages. The Media Player SDK itself. CrossPlatform Core, the GStreamer runtime the X engine runs on. CrossPlatform Core Adds, which carries more plugins, the reverb among them. And CrossPlatform Libav, the FFmpeg codecs that many formats decode through. Or use the NuGet Package Manager: right-click the project, Manage NuGet Packages, and install VisioForge.DotNet.MediaPlayer from the Browse tab. Then install the Core, Core Adds and Libav packages for Windows x64. The Installed tab lists all four. These are the x64 packages, so set PlatformTarget to x64 in the project file.

### The player window

The window is written in XAML, and the designer draws it right away. First the player part. Map the VisioForge.Core.UI.WPF namespace to a prefix, here vf. The grid has two columns, the video and a 260-pixel settings column, and four rows: the video, the timeline, the buttons, and the effects. The VideoView fills the first cell. Under the video goes the timeline: a slider for the position, with the time beside it. Then Play, Pause, Resume and Stop. They all start disabled: Play until the SDK has loaded, and the other three until a file plays. The settings column holds the file name with a browse button, and a combo box for the audio output. Then a volume slider, and a read-only text box for the error log.

### The effects panel

Now the effects panel, in the last row, under the buttons. A horizontal stack panel spans both columns. Each effect gets a group box with a checkbox in its header. The equalizer comes first, and its checkbox names one handler for both Checked and Unchecked. A uniform grid of two rows and ten columns holds the bands: ten sliders, and their frequencies under them, from 30 hertz to 15 kilohertz. Each slider's Tag is the index of its band, so all ten can share one handler. One style sets up all ten sliders: vertical, from minus 12 to plus 12 decibels, and wired to the same ValueChanged handler. A second style centers the labels. The echo has a delay slider, from 50 to 1000 milliseconds, and an intensity slider from 0 to 1. The reverb has a room size, and a level that mixes the reverberated sound with the dry one. Both go from 0 to 1. And the pitch has one slider, from 12 semitones down to 12 up, that snaps to whole semitones.

### Initialize the SDK

MainWindow.xaml.cs starts with these usings. VisioForge.Core.Types.X.AudioEffects holds the effect classes. Each effect is one object, created once and set to the sliders' starting values. Equalizer10AudioEffect takes the ten band levels in decibels, all zero here. EchoAudioEffect takes the delay, then its intensity and feedback. ReverberationAudioEffect gets a large room, and PitchAudioEffect is four semitones up. When the window loads, await VisioForgeX.InitSDKAsync once. Then create the MediaPlayerCoreX engine and pass it the VideoView. Errors arrive in the OnError event on another thread, so the handler goes back to the UI thread and adds them to the log. Audio_OutputDevicesAsync lists the speakers and headphones, and their names fill the combo box. Play is enabled once the SDK has loaded. The browse button shows an OpenFileDialog for video and audio files.

### Play with the effects

The Play handler disables the buttons and clears the log. It sends the sound to the selected output, at the slider's volume. OpenAsync opens the file. Then Audio_Effects_AddOrUpdate adds each effect whose checkbox is checked, and PlayAsync starts playback with them. Effects last for one playback: after Stop or the end of the file they are gone, so every Play adds them again. PlaybackState.Free means the file did not open or play, and the window goes back to stopped. Otherwise the buttons switch to playing, and the timer for the timeline starts.

### Change the effects while it plays

ApplyEffect serves all four checkboxes. While a file plays, checking a box adds its effect with Audio_Effects_AddOrUpdate, and unchecking it calls Audio_Effects_Remove. When nothing plays, it does nothing: Play adds the checked effects. The equalizer's checkbox calls ApplyEffect. The band sliders share one handler: it reads the band from the slider's Tag, and SetLevel sets that band's level in decibels. The playing equalizer takes it at once. The echo sliders set Delay and Intensity on the effect object. A property set on an effect that plays applies at once, so there is no need to add it again. The reverb works the same way, with RoomSize and Level. And the pitch slider sets PitchSemitones. The tempo stays the same: only the pitch moves.

### The rest of the player

The rest is the player from the video player tutorial. Pause and Resume are one call each, and Stop awaits StopAsync and resets the window. The effects keep playing after a pause. ShowBusy disables all four buttons, and ShowPlaying enables either Play or the other three. Stopped puts the buttons, the time label and the timeline back. OnStop fires after StopAsync and at the end of the file. The handler acts only for the end of the file, while Stop is enabled and State is Free. A DispatcherTimer ticks twice a second and moves the timeline to the position, with a flag set, so that this move does not seek. A click on the timeline seeks at once, and a drag seeks once, when the thumb is released. The volume follows its slider. When the window closes, Dispose stops playback and releases the player, and DestroySDK releases the SDK.

### Run it

Run the app. Pick a file, click Play, and click the timeline to jump ahead. First, listen to it with no effects. Check Equalizer, then pull the low bands down and the high bands up. Turn the equalizer off and Echo on. Then make the delay longer. Echo off, Reverb on: a large room. Then raise the level for more of the room. Reverb off, Pitch on: four semitones up. Then move it down. Uncheck Pitch, and the sound is back as it was. Then Stop, and close the window.

### Common mistakes

A few common mistakes. Leaving out the Core Adds package, so the reverb is not created. Adding the effects only once: Stop or the end of the file drops them, so add them again on every Play. Writing one item of the equalizer's Levels array during playback, which does not reach the playing effect: use SetLevel. PitchShiftAudioEffect is obsolete, so use PitchAudioEffect. And mixing x86 and x64 packages.

### Next steps

The written guide lists more effects, such as a compressor, a flanger and noise suppression. The same effects work while capturing video, and Media Blocks SDK has one block per effect. And the audio sample grabber hands you the samples in your own code. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Audio Effects Reference](reference.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Audio Encoders](../audio-encoders/index.md)
- [Audio Processing Blocks (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
