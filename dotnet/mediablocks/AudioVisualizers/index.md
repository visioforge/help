---
title: .Net Audio Visualizer Blocks
description: Explore a comprehensive set of .NET audio visualizer blocks for building powerful audio-reactive applications. Includes Spacescope, Spectrascope, Synaescope, and Wavescope.
sidebar_label: Audio Visualizers

---

# Audio visualizer blocks

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

VisioForge Media Blocks SDK .Net includes a set of audio visualizer blocks that allow you to create audio-reactive visualizations for your applications. These blocks take audio input and produce video output representing the audio characteristics.

The blocks can be connected to other audio and video processing blocks to create complex media pipelines.

Most of the blocks are available for all platforms, including Windows, Linux, MacOS, Android, and iOS.

## Spacescope

The Spacescope block is a simple audio visualization element that maps the left and right audio channels to X and Y coordinates, respectively, creating a Lissajous-like pattern. This visualizes the phase relationship between the channels. The appearance, such as using dots or lines and colors, can be customized via `SpacescopeSettings`.

#### Block info

Name: SpacescopeBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed audio | 1
Output | Video | 1

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->SpacescopeBlock;
    SpacescopeBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp3"; // Or any audio source
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

// Settings can be customized, e.g., for shader, line thickness, etc.
// The style (dots, lines, color-dots, color-lines) can be set in SpacescopeSettings.
var spacescopeSettings = new SpacescopeSettings(); 
var spacescope = new SpacescopeBlock(spacescopeSettings);
pipeline.Connect(fileSource.AudioOutput, spacescope.Input);

// Assuming you have a VideoRendererBlock or a way to display video output
var videoRenderer = new VideoRendererBlock(IntPtr.Zero); // Example for Windows
pipeline.Connect(spacescope.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

## Spectrascope

The Spectrascope block is a simple spectrum visualization element. It renders the frequency spectrum of the audio input as a series of bars.

#### Block info

Name: SpectrascopeBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed audio | 1
Output | Video | 1

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->SpectrascopeBlock;
    SpectrascopeBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp3"; // Or any audio source
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var spectrascope = new SpectrascopeBlock();
pipeline.Connect(fileSource.AudioOutput, spectrascope.Input);

// Assuming you have a VideoRendererBlock or a way to display video output
var videoRenderer = new VideoRendererBlock(IntPtr.Zero); // Example for Windows
pipeline.Connect(spectrascope.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

## Synaescope

The Synaescope block is an audio visualization element that analyzes frequencies and out-of-phase properties of the audio. It draws this analysis as dynamic clouds of stars, creating colorful and abstract patterns.

#### Block info

Name: SynaescopeBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed audio | 1
Output | Video | 1

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->SynaescopeBlock;
    SynaescopeBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp3"; // Or any audio source
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

// Settings can be customized for Synaescope.
// For example, to set a specific shader effect (if available in SynaescopeSettings):
// var synaescopeSettings = new SynaescopeSettings() { Shader = SynaescopeShader.LibVisualCurrent };
// var synaescope = new SynaescopeBlock(synaescopeSettings);
var synaescope = new SynaescopeBlock(); // Default settings
pipeline.Connect(fileSource.AudioOutput, synaescope.Input);

// Assuming you have a VideoRendererBlock or a way to display video output
var videoRenderer = new VideoRendererBlock(IntPtr.Zero); // Example for Windows
pipeline.Connect(synaescope.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

## Wavescope

The Wavescope block is a simple audio visualization element that renders the audio waveforms, similar to an oscilloscope display. The drawing style (dots, lines, colors) can be configured using `WavescopeSettings`.

#### Block info

Name: WavescopeBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed audio | 1
Output | Video | 1

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->WavescopeBlock;
    WavescopeBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp3"; // Or any audio source
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

// Settings can be customized, e.g., for style, mono/stereo mode, etc.
// The style (dots, lines, color-dots, color-lines) can be set in WavescopeSettings.
var wavescopeSettings = new WavescopeSettings(); 
var wavescope = new WavescopeBlock(wavescopeSettings);
pipeline.Connect(fileSource.AudioOutput, wavescope.Input);

// Assuming you have a VideoRendererBlock or a way to display video output
var videoRenderer = new VideoRendererBlock(IntPtr.Zero); // Example for Windows
pipeline.Connect(wavescope.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.
