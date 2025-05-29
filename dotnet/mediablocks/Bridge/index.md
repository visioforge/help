---
title: Link Media Pipelines - Bridge Blocks Guide
description: Learn to use Bridge blocks for linking and dynamically switching media pipelines for audio, video, and subtitles in .Net applications.
sidebar_label: Video and Audio Bridges
---

# Bridge blocks

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

Bridges can be used to link two pipelines and dynamically switch between them. For example, you can switch between different files or cameras in the first Pipeline without interrupting streaming in the second Pipeline.

To link source and sink, give them the same name. Each bridge pair has a unique channel name.

## Bridge audio sink and source

Bridges can be used to connect different media pipelines and use them independently. `BridgeAudioSourceBlock` is used to connect to `BridgeAudioSinkBlock` and supports uncompressed audio.

### Block info

#### BridgeAudioSourceBlock information

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output audio | uncompressed audio | 1 |

#### BridgeAudioSinkBlock information

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input audio | uncompressed audio | 1 |

### Sample pipelines

#### First pipeline with an audio source and a bridge audio sink

```mermaid
graph LR;
    VirtualAudioSourceBlock-->BridgeAudioSinkBlock;
```

#### Second pipeline with a bridge audio source and an audio renderer

```mermaid
graph LR;
    BridgeAudioSourceBlock-->AudioRendererBlock;
```

### Sample code

The source pipeline with virtual audio source and bridge audio sink.

```csharp
// create source pipeline
var sourcePipeline = new MediaBlocksPipeline();

// create virtual audio source and bridge audio sink
var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());
var bridgeAudioSink = new BridgeAudioSinkBlock(new BridgeAudioSinkSettings());

// connect source and sink
sourcePipeline.Connect(audioSourceBlock.Output, bridgeAudioSink.Input);

// start pipeline
await sourcePipeline.StartAsync();
```

The sink pipeline with bridge audio source and audio renderer.

```csharp
// create sink pipeline
var sinkPipeline = new MediaBlocksPipeline();

// create bridge audio source and audio renderer
var bridgeAudioSource = new BridgeAudioSourceBlock(new BridgeAudioSourceSettings());
var audioRenderer = new AudioRendererBlock();

// connect source and sink
sinkPipeline.Connect(bridgeAudioSource.Output, audioRenderer.Input);

// start pipeline
await sinkPipeline.StartAsync();
```

## Bridge video sink and source

Bridges can be used to connect different media pipelines and use them independently. `BridgeVideoSinkBlock` is used to connect to the `BridgeVideoSourceBlock` and supports uncompressed video.

### Blocks info

#### BridgeVideoSinkBlock information

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | uncompressed video | 1 |

#### BridgeVideoSourceBlock information

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | uncompressed video | 1 |

### Sample pipelines

#### First pipeline with a video source and a bridge video sink

```mermaid
graph LR;
    VirtualVideoSourceBlock-->BridgeVideoSinkBlock;
```

#### Second pipeline with a bridge video source and a video renderer

```mermaid
graph LR;
    BridgeVideoSourceBlock-->VideoRendererBlock;
```

### Sample code

Source pipeline with a virtual video source and bridge video sink.

```csharp
// create source pipeline
var sourcePipeline = new MediaBlocksPipeline();

// create virtual video source and bridge video sink
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var bridgeVideoSink = new BridgeVideoSinkBlock(new BridgeVideoSinkSettings());

// connect source and sink
sourcePipeline.Connect(videoSourceBlock.Output, bridgeVideoSink.Input);

// start pipeline
await sourcePipeline.StartAsync();
```

Sink pipeline with a bridge video source and video renderer.

```csharp
// create sink pipeline
var sinkPipeline = new MediaBlocksPipeline();

// create bridge video source and video renderer
var bridgeVideoSource = new BridgeVideoSourceBlock(new BridgeVideoSourceSettings());
var videoRenderer = new VideoRendererBlock(sinkPipeline, VideoView1);

// connect source and sink
sinkPipeline.Connect(bridgeVideoSource.Output, videoRenderer.Input);

// start pipeline
await sinkPipeline.StartAsync();
```

## Bridge subtitle sink and source

Bridges can be used to connect different media pipelines and use them independently. `BridgeSubtitleSourceBlock` is used to connect to the `BridgeSubtitleSinkBlock`and supports text media type.

### Block info

#### BridgeSubtitleSourceBlock information

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | text | 1 |

#### BridgeSubtitleSinkBlock information

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | text | 1 |

## Proxy source

Proxy source/proxy sink pair of blocks can be used to connect different media pipelines and use them independently.

### Block info

Name: ProxySourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output | Any uncompressed | 1 |

### Sample pipelines

#### First pipeline with a video source and a proxy video sink

```mermaid
graph LR;
    VirtualVideoSourceBlock-->ProxySinkBlock;
```

#### Second pipeline with a proxy video source and a video renderer

```mermaid
graph LR;
    ProxySourceBlock-->VideoRendererBlock;
```

### Sample code

```csharp
// source pipeline with virtual video source and proxy sink
var sourcePipeline = new MediaBlocksPipeline();
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var proxyVideoSink = new ProxySinkBlock();
sourcePipeline.Connect(videoSourceBlock.Output, proxyVideoSink.Input);

// sink pipeline with proxy video source and video renderer
var sinkPipeline = new MediaBlocksPipeline();
var proxyVideoSource = new ProxySourceBlock(proxyVideoSink);
var videoRenderer = new VideoRendererBlock(sinkPipeline, VideoView1);
sinkPipeline.Connect(proxyVideoSource.Output, videoRenderer.Input);

// start pipelines
await sourcePipeline.StartAsync();
await sinkPipeline.StartAsync();
```

## Platforms

All bridge blocks are supported on Windows, macOS, Linux, iOS, and Android.
