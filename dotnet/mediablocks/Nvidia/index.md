---
title: .Net Media Nvidia Blocks Guide
description: Explore a complete guide to .Net Media SDK Nvidia blocks. Learn about Nvidia-specific blocks for your media processing pipelines.
sidebar_label: Nvidia
---

# Nvidia Blocks - VisioForge Media Blocks SDK .Net

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

Nvidia blocks leverage Nvidia GPU capabilities for accelerated media processing tasks such as data transfer, video conversion, and resizing.

## NVDataDownloadBlock

Nvidia data download block. Downloads data from Nvidia GPU to system memory.

#### Block info

Name: NVDataDownloadBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | Video (GPU memory) | 1 |
| Output video | Video (system memory) | 1 |

#### The sample pipeline

```mermaid
graph LR;
    NVCUDAConverterBlock-->NVDataDownloadBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// create a source that outputs to GPU memory (e.g., a decoder or another Nvidia block)
// For example, NVDataUploadBlock or an NV-accelerated decoder
var upstreamNvidiaBlock = new NVDataUploadBlock(); // Conceptual: assume this block is properly configured

// create Nvidia data download block
var nvDataDownload = new NVDataDownloadBlock();

// create video renderer block
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1 is your display control

// connect blocks
// pipeline.Connect(upstreamNvidiaBlock.Output, nvDataDownload.Input); // Connect GPU source to download block
// pipeline.Connect(nvDataDownload.Output, videoRenderer.Input); // Connect download block (system memory) to renderer

// start pipeline
// await pipeline.StartAsync();
```

#### Remarks

This block is used to transfer video data from the Nvidia GPU's memory to the main system memory. This is typically needed when a GPU-processed video stream needs to be accessed by a component that operates on system memory, like a CPU-based encoder or a standard video renderer.
Ensure that the correct Nvidia drivers and CUDA toolkit are installed for this block to function.
Use `NVDataDownloadBlock.IsAvailable()` to check if the block can be used.

#### Platforms

Windows, Linux (Requires Nvidia GPU and appropriate drivers/SDK).

## NVDataUploadBlock

Nvidia data upload block. Uploads data to Nvidia GPU from system memory.

#### Block info

Name: NVDataUploadBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | Video (system memory) | 1 |
| Output video | Video (GPU memory) | 1 |

#### The sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->NVDataUploadBlock-->NVH264EncoderBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// create a video source (e.g., SystemVideoSourceBlock or UniversalSourceBlock)
var videoSource = new UniversalSourceBlock(); // Conceptual: assume this block is properly configured
// videoSource.Filename = "input.mp4";

// create Nvidia data upload block
var nvDataUpload = new NVDataUploadBlock();

// create an Nvidia accelerated encoder (e.g., NVH264EncoderBlock)
// var nvEncoder = new NVH264EncoderBlock(new NVH264EncoderSettings()); // Conceptual

// connect blocks
// pipeline.Connect(videoSource.VideoOutput, nvDataUpload.Input); // Connect system memory source to upload block
// pipeline.Connect(nvDataUpload.Output, nvEncoder.Input); // Connect upload block (GPU memory) to NV encoder

// start pipeline
// await pipeline.StartAsync();
```

#### Remarks

This block is used to transfer video data from main system memory to the Nvidia GPU's memory. This is typically a prerequisite for using Nvidia-accelerated processing blocks like encoders, decoders, or filters that operate on GPU memory.
Ensure that the correct Nvidia drivers and CUDA toolkit are installed for this block to function.
Use `NVDataUploadBlock.IsAvailable()` to check if the block can be used.

#### Platforms

Windows, Linux (Requires Nvidia GPU and appropriate drivers/SDK).

## NVVideoConverterBlock

Nvidia video converter block. Performs color space conversions and other video format conversions using the Nvidia GPU.

#### Block info

Name: NVVideoConverterBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | Video (GPU memory) | 1 |
| Output video | Video (GPU memory, possibly different format) | 1 |

#### The sample pipeline

```mermaid
graph LR;
    NVDataUploadBlock-->NVVideoConverterBlock-->NVDataDownloadBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// Assume video data is already in GPU memory via NVDataUploadBlock or an NV-decoder
// var nvUploadedSource = new NVDataUploadBlock(); // Conceptual
// pipeline.Connect(systemMemorySource.Output, nvUploadedSource.Input);


// create Nvidia video converter block
var nvVideoConverter = new NVVideoConverterBlock();
// Specific conversion settings might be applied here if the block has properties for them.

// Assume we want to download the converted video back to system memory
// var nvDataDownload = new NVDataDownloadBlock(); // Conceptual

// connect blocks
// pipeline.Connect(nvUploadedSource.Output, nvVideoConverter.Input);
// pipeline.Connect(nvVideoConverter.Output, nvDataDownload.Input);
// pipeline.Connect(nvDataDownload.Output, videoRenderer.Input); // Or to another system memory component

// start pipeline
// await pipeline.StartAsync();
```

#### Remarks

The `NVVideoConverterBlock` is used for efficient video format conversions (e.g., color space, pixel format) leveraging the Nvidia GPU. This is often faster than CPU-based conversions, especially for high-resolution video. It typically operates on video data already present in GPU memory.
Ensure that the correct Nvidia drivers and CUDA toolkit are installed.
Use `NVVideoConverterBlock.IsAvailable()` to check if the block can be used.

#### Platforms

Windows, Linux (Requires Nvidia GPU and appropriate drivers/SDK).

## NVVideoResizeBlock

Nvidia video resize block. Resizes video frames using the Nvidia GPU.

#### Block info

Name: NVVideoResizeBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | Video (GPU memory) | 1 |
| Output video | Video (GPU memory, resized) | 1 |

#### Settings

The `NVVideoResizeBlock` is configured using a `VisioForge.Core.Types.Size` object passed to its constructor.

- `Resolution` (`VisioForge.Core.Types.Size`): Specifies the target output resolution (Width, Height) for the video.

#### The sample pipeline

```mermaid
graph LR;
    NVDataUploadBlock-->NVVideoResizeBlock-->NVH264EncoderBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// Target resolution for resizing
var targetResolution = new VisioForge.Core.Types.Size(1280, 720);

// Assume video data is already in GPU memory via NVDataUploadBlock or an NV-decoder
// var nvUploadedSource = new NVDataUploadBlock(); // Conceptual
// pipeline.Connect(systemMemorySource.Output, nvUploadedSource.Input);

// create Nvidia video resize block
var nvVideoResize = new NVVideoResizeBlock(targetResolution);

// Assume the resized video will be encoded by an NV-encoder
// var nvEncoder = new NVH264EncoderBlock(new NVH264EncoderSettings()); // Conceptual

// connect blocks
// pipeline.Connect(nvUploadedSource.Output, nvVideoResize.Input);
// pipeline.Connect(nvVideoResize.Output, nvEncoder.Input);

// start pipeline
// await pipeline.StartAsync();
```

#### Remarks

The `NVVideoResizeBlock` performs video scaling operations efficiently using the Nvidia GPU. This is useful for adapting video streams to different display resolutions or encoding requirements. It typically operates on video data already present in GPU memory.
Ensure that the correct Nvidia drivers and CUDA toolkit are installed.
Use `NVVideoResizeBlock.IsAvailable()` to check if the block can be used.

#### Platforms

Windows, Linux (Requires Nvidia GPU and appropriate drivers/SDK).
