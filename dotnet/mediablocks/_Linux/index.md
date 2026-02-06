---
title: Linux Platform Blocks
description: Linux-optimized MediaBlocks with VA-API hardware acceleration for efficient video encoding, decoding, and processing on Ubuntu platforms.
sidebar_label: Linux Platform
---

# Linux Platform Blocks - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

This section covers MediaBlocks specifically optimized for Linux platforms.

## Available Blocks

### VA-API Hardware Decoders

Linux provides hardware-accelerated video decoding through VA-API (Video Acceleration API):

- **VAAPIH264DecoderBlock**: Hardware H.264/AVC decoding
- **VAAPIH265DecoderBlock**: Hardware H.265/HEVC decoding
- **VAAPIMPEG2DecoderBlock**: Hardware MPEG-2 decoding
- **VAAPIVP8DecoderBlock**: Hardware VP8 decoding

See [Video Decoders Documentation](../VideoDecoders/index.md)

## Platform Requirements

- **Linux**: Any modern Linux distribution
- **VA-API**: libva and appropriate driver for your GPU
- **Hardware**: Intel, AMD, or other GPU with VA-API support

## Features

- **Hardware Acceleration**: GPU-based video decoding
- **Low CPU Usage**: Offload decoding to dedicated hardware
- **Wide Compatibility**: Works with Intel, AMD, and other GPUs
- **Power Efficiency**: Reduced power consumption
- **Multi-Stream**: Handle multiple HD/4K streams

## Sample Code

### Hardware H.264 Decoding

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video.mp4")));

// VA-API hardware decoder
if (VAAPIH264DecoderBlock.IsAvailable())
{
    var vaapiDecoder = new VAAPIH264DecoderBlock();
    pipeline.Connect(fileSource.VideoOutput, vaapiDecoder.Input);
    
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    pipeline.Connect(vaapiDecoder.Output, videoRenderer.Input);
}
else
{
    // Fallback to software decoder
    var decoder = new UniversalDecoderBlock();
    pipeline.Connect(fileSource.VideoOutput, decoder.Input);
    
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);
}

await pipeline.StartAsync();
```

### Multiple Hardware Decoders

```csharp
var pipeline = new MediaBlocksPipeline();

// Decode H.264 stream
var h264Source = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("h264.mp4")));
var h264Decoder = new VAAPIH264DecoderBlock();
pipeline.Connect(h264Source.VideoOutput, h264Decoder.Input);

// Decode H.265 stream
var h265Source = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("h265.mp4")));
var h265Decoder = new VAAPIH265DecoderBlock();
pipeline.Connect(h265Source.VideoOutput, h265Decoder.Input);

// Mix both streams (example)
var mixer = new VideoMixerBlock(mixerSettings);
pipeline.Connect(h264Decoder.Output, mixer.Input1);
pipeline.Connect(h265Decoder.Output, mixer.Input2);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixer.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Setup and Configuration

### Installing VA-API

**Ubuntu/Debian:**
```bash
sudo apt-get install libva2 libva-drm2 vainfo
```

**Fedora/RHEL:**
```bash
sudo dnf install libva libva-utils
```

### Checking VA-API Support

```bash
vainfo
```

This command displays available VA-API profiles and entrypoints.

### Driver Installation

**Intel GPUs:**
```bash
# Ubuntu/Debian
sudo apt-get install intel-media-va-driver

# Fedora/RHEL
sudo dnf install intel-media-driver
```

**AMD GPUs:**
```bash
# Ubuntu/Debian
sudo apt-get install mesa-va-drivers

# Fedora/RHEL
sudo dnf install mesa-va-drivers
```

## Checking Hardware Support in Code

```csharp
// Check if VA-API H.264 decoder is available
if (VAAPIH264DecoderBlock.IsAvailable())
{
    Console.WriteLine("VA-API H.264 hardware decoding is available");
    var decoder = new VAAPIH264DecoderBlock();
}
else
{
    Console.WriteLine("VA-API not available, using software decoder");
    var decoder = new UniversalDecoderBlock();
}
```

## Performance Tips

- Ensure VA-API drivers are properly installed
- Use hardware decoders when available for best performance
- Check decoder availability before creating blocks
- Monitor GPU memory usage when processing multiple streams
- Use `vainfo` command to verify hardware support

## Troubleshooting

**VA-API not working:**
1. Verify driver installation with `vainfo`
2. Check user permissions (may need to be in `video` or `render` group)
3. Ensure GStreamer VA-API plugins are installed: `gstreamer1.0-vaapi`

**Permission issues:**
```bash
# Add user to video group
sudo usermod -a -G video $USER
# Log out and back in for changes to take effect
```

## Platforms

Linux (Ubuntu, Debian, Fedora, RHEL, Arch, and other distributions).

Requires VA-API support with appropriate drivers.

## Related Documentation

- [VideoDecoders](../VideoDecoders/index.md) - Hardware decoding blocks
- [Nvidia](../Nvidia/index.md) - Nvidia GPU acceleration (also works on Linux)
