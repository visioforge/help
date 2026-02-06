---
title: DirectShow Encoding Filters - Codecs Reference
description: DirectShow codec reference with H.264/H.265/VP8/VP9 video, AAC/MP3/Opus audio, and hardware acceleration (NVENC, QuickSync, AMF).
---

# Encoding Filters Pack - Codecs Reference

## Overview

This document provides a comprehensive reference for all video and audio codecs supported by the DirectShow Encoding Filters Pack. The pack includes both software and hardware-accelerated encoders for professional media encoding.

---
## Video Codecs
### H.264/AVC (MPEG-4 Part 10)
The most widely used video codec for streaming, broadcasting, and file storage.
#### Encoder Options
| Encoder Type | Description | Hardware Support | Performance | Quality |
|--------------|-------------|------------------|-------------|---------|
| **Software (x264)** | CPU-based H.264 encoder | None | Moderate | Excellent |
| **NVENC** | NVIDIA GPU encoder | NVIDIA GPUs (Kepler+) | Very Fast | Good-Excellent |
| **QuickSync** | Intel integrated graphics | Intel CPUs (2nd gen+) | Fast | Good |
| **AMD AMF** | AMD GPU encoder | AMD GPUs (GCN+) | Fast | Good |
| **Media Foundation** | Windows MF encoder | Various (OS-dependent) | Moderate | Good |
#### Profiles and Levels
**Profiles**:
- **Baseline** - Basic features, mobile devices
- **Main** - Standard features, most applications
- **High** - Advanced features, HD content, Blu-ray
**Common Levels**:
- **Level 3.0** - SD (720x480 @ 30 fps)
- **Level 3.1** - 720p (1280x720 @ 30 fps)
- **Level 4.0** - 1080p (1920x1080 @ 30 fps)
- **Level 4.1** - 1080p @ 60 fps
- **Level 5.0** - 4K (3840x2160 @ 30 fps)
- **Level 5.1** - 4K @ 60 fps
#### Rate Control Modes
| Mode | Description | Use Case | Bitrate Behavior |
|------|-------------|----------|------------------|
| **CBR** | Constant Bitrate | Streaming, broadcasting | Fixed bitrate |
| **VBR** | Variable Bitrate | File storage | Varies based on complexity |
| **CQP** | Constant Quantization | High quality archival | Varies significantly |
#### Recommended Settings
**Streaming (1080p @ 30fps)**:
- Bitrate: 4-6 Mbps
- Profile: High
- Level: 4.0
- GOP Size: 60 (2 seconds)
- B-frames: 2
**Recording (1080p @ 60fps)**:
- Bitrate: 8-12 Mbps
- Profile: High
- Level: 4.1
- GOP Size: 120 (2 seconds)
- B-frames: 3
**Low Latency Streaming**:
- Bitrate: 2-4 Mbps
- Profile: Main
- Level: 3.1
- GOP Size: 30 (1 second)
- B-frames: 0
---

### H.265/HEVC (High Efficiency Video Coding)

Next-generation codec offering 40-50% better compression than H.264.

#### Encoder Options

| Encoder Type | Description | Hardware Support | Performance | Quality |
|--------------|-------------|------------------|-------------|---------|
| **Software (x265)** | CPU-based HEVC encoder | None | Slow | Excellent |
| **NVENC** | NVIDIA GPU encoder | NVIDIA GPUs (Maxwell+) | Fast | Good-Excellent |
| **QuickSync** | Intel integrated graphics | Intel CPUs (6th gen+) | Fast | Good |
| **AMD AMF** | AMD GPU encoder | AMD GPUs (Fiji+) | Fast | Good |

#### Profiles and Tiers

**Profiles**:
- **Main** - 8-bit, 4:2:0, standard use
- **Main 10** - 10-bit, HDR support
- **Main Still Picture** - Single images

**Tiers**:
- **Main Tier** - Standard applications
- **High Tier** - Professional/broadcast

**Common Levels**:
- **Level 3.1** - 720p @ 30 fps
- **Level 4.0** - 1080p @ 30 fps
- **Level 4.1** - 1080p @ 60 fps
- **Level 5.0** - 4K @ 30 fps
- **Level 5.1** - 4K @ 60 fps

#### Recommended Settings

**4K Streaming (2160p @ 30fps)**:
- Bitrate: 15-20 Mbps
- Profile: Main
- Level: 5.0
- GOP Size: 60
- Tile Encoding: Enabled

**1080p High Quality**:
- Bitrate: 3-5 Mbps
- Profile: Main or Main 10
- Level: 4.0
- GOP Size: 90

---
### VP8
Google's open-source video codec, primarily for WebM containers.
#### Features
- **License**: Royalty-free, open source
- **Container**: WebM (preferred), MKV
- **Hardware Support**: Limited
- **Quality**: Good at medium-high bitrates
- **Complexity**: Moderate encoding time
#### Recommended Settings
**WebM Streaming (720p)**:
- Bitrate: 1-2 Mbps
- GOP Size: 120
- Quality: Good (0-63 scale, lower is better)
- Threads: Auto
---

### VP9

Successor to VP8 with significantly improved compression efficiency.

#### Features

- **License**: Royalty-free, open source
- **Container**: WebM (primary), MKV
- **Hardware Support**: Recent GPUs (Intel, NVIDIA, AMD)
- **Quality**: Comparable to H.265
- **Complexity**: Very high (software), moderate (hardware)

#### Profiles

- **Profile 0** - 8-bit, 4:2:0 (most common)
- **Profile 1** - 8-bit, 4:2:2/4:4:4
- **Profile 2** - 10/12-bit, 4:2:0
- **Profile 3** - 10/12-bit, 4:2:2/4:4:4

#### Recommended Settings

**YouTube/WebM (1080p @ 30fps)**:
- Bitrate: 2-4 Mbps
- Quality/Speed: 1 (fastest), 0 (slowest/best)
- GOP Size: 60
- Tile Columns: 2

---
### MPEG-2
Legacy codec still used for DVDs and broadcasting.
#### Features
- **License**: Requires license
- **Container**: MPEG-PS, MPEG-TS, VOB
- **Hardware Support**: Universal
- **Quality**: Good but requires higher bitrates
- **Use Cases**: DVD authoring, broadcasting
#### Common Variants
- **MPEG-2 DVD** - 4-8 Mbps, 720x480/720x576
- **MPEG-2 SVCD** - 2.5 Mbps, 480x480/480x576
- **MPEG-2 HD** - 15-25 Mbps, 1920x1080
#### Recommended Settings
**DVD Video (NTSC)**:
- Resolution: 720x480
- Bitrate: 6 Mbps
- GOP Size: 15 (NTSC) or 12 (PAL)
- Aspect Ratio: 16:9 or 4:3
---

### MPEG-4 Part 2

Older MPEG-4 Visual codec (DivX/Xvid era).

#### Features

- **License**: Requires license
- **Container**: AVI, MP4, MKV
- **Quality**: Moderate
- **Use Cases**: Legacy content, low-power devices

#### Recommended Settings

**Standard Definition**:
- Resolution: 640x480
- Bitrate: 1-2 Mbps
- GOP Size: 250

---
## Audio Codecs
### AAC (Advanced Audio Coding)
Industry-standard audio codec for most applications.
#### Encoder Options
| Encoder Type | Description | Quality | Performance |
|--------------|-------------|---------|-------------|
| **FFmpeg AAC** | Software encoder | Good | Fast |
| **Media Foundation AAC** | Windows built-in | Good | Fast |
| **FAAC** | Open-source encoder | Moderate | Fast |
#### Profiles
- **AAC-LC (Low Complexity)** - Standard, most compatible
- **HE-AAC (High Efficiency)** - Better at low bitrates
- **HE-AAC v2** - Even better for very low bitrates
#### Recommended Bitrates
| Quality | Stereo | 5.1 Surround |
|---------|--------|--------------|
| **Low** | 64-96 kbps | 192 kbps |
| **Medium** | 128 kbps | 256 kbps |
| **High** | 192 kbps | 384 kbps |
| **Very High** | 256-320 kbps | 448-640 kbps |
#### Sample Rates
- **44.1 kHz** - CD quality, music
- **48 kHz** - Professional audio, video
- **32 kHz** - Lower quality (voice)
---

### MP3 (MPEG-1/2 Audio Layer III)

Legacy but still widely used audio codec.

#### Encoder Options

- **LAME** - Excellent quality open-source encoder
- **FFmpeg MP3** - Built-in encoder

#### Bitrate Modes

| Mode | Description | File Size | Quality |
|------|-------------|-----------|---------|
| **CBR** | Constant Bitrate | Predictable | Consistent |
| **VBR** | Variable Bitrate | Smaller | Better |
| **ABR** | Average Bitrate | Balanced | Good |

#### Recommended Settings

**Music (High Quality)**:
- Mode: VBR
- Quality: V0-V2 (LAME scale)
- Approximate Bitrate: 190-245 kbps
- Sample Rate: 44.1 kHz

**Podcast/Speech**:
- Mode: CBR
- Bitrate: 96-128 kbps
- Sample Rate: 44.1 kHz

**Low Bandwidth**:
- Mode: VBR
- Quality: V5-V6
- Approximate Bitrate: 120-150 kbps

---
### Vorbis
Open-source alternative to MP3 and AAC.
#### Features
- **License**: Completely free, no patents
- **Container**: OGG (primary), WebM, MKV
- **Quality**: Excellent, especially at low-mid bitrates
- **Compatibility**: Good but not universal
#### Recommended Settings
**Music (High Quality)**:
- Quality: 6-8 (0-10 scale)
- Approximate Bitrate: 192-256 kbps
- Sample Rate: 44.1 kHz or 48 kHz
**Streaming**:
- Quality: 4-5
- Approximate Bitrate: 128-160 kbps
---

### Opus

Modern, highly efficient codec for both speech and music.

#### Features

- **License**: Royalty-free, standardized (RFC 6716)
- **Container**: WebM, MKV, OGG
- **Latency**: Extremely low (5-66.5 ms)
- **Bitrate Range**: 6-510 kbps
- **Quality**: Superior to MP3, AAC, Vorbis

#### Applications

- **VoIP/Voice Chat**: 8-24 kbps
- **Music Streaming**: 64-128 kbps
- **High Fidelity**: 128-256 kbps

#### Recommended Settings

**Voice Chat**:
- Bitrate: 16-24 kbps
- Sample Rate: 16 kHz or 48 kHz
- Application: VoIP

**Music**:
- Bitrate: 96-160 kbps
- Sample Rate: 48 kHz
- Application: Audio

---
### FLAC (Free Lossless Audio Codec)
Lossless audio compression.
#### Features
- **License**: Open source, royalty-free
- **Compression**: Typically 40-60% of original
- **Quality**: Bit-perfect lossless
- **Compatibility**: Good and improving
#### Compression Levels
- **Level 0** - Fastest, ~50% compression
- **Level 5** - Default, ~55% compression
- **Level 8** - Slowest, ~60% compression
#### Recommended Settings
**Archival**:
- Compression Level: 5-8
- Sample Rate: Original (typically 44.1 or 48 kHz)
- Bit Depth: Original (16 or 24-bit)
**Streaming**:
- Compression Level: 0-3
- Sample Rate: 44.1 or 48 kHz
---

### Speex

Specialized codec for voice compression.

#### Features

- **License**: Open source (BSD)
- **Purpose**: Speech compression (not music)
- **Bitrate**: 2-44 kbps
- **Quality**: Optimized for voice

#### Modes

- **Narrowband** (8 kHz) - Phone quality, 2.15-24.6 kbps
- **Wideband** (16 kHz) - Better clarity, 4-44 kbps
- **Ultra-wideband** (32 kHz) - Full speech spectrum

#### Recommended Settings

**VoIP**:
- Mode: Wideband
- Quality: 6-8 (0-10 scale)
- Bitrate: ~15-20 kbps

---
## Hardware Acceleration Overview
### NVIDIA NVENC
**Supported Codecs**:
- H.264/AVC (all NVIDIA GPUs from Kepler generation)
- H.265/HEVC (Maxwell generation and newer)
**Generations**:
- **Kepler** (GTX 600/700) - 1st gen, basic H.264
- **Maxwell** (GTX 900) - 2nd gen, HEVC support
- **Pascal** (GTX 10XX) - 3rd gen, improved quality
- **Turing/Ampere** (RTX 20XX/30XX) - 7th/8th gen, excellent quality
**Performance**: Up to 8K @ 30 fps (GPU dependent)
**Quality Settings**:
- Preset: P1 (fastest) to P7 (slowest, best quality)
- Recommended: P4-P6 for balanced quality/speed
---

### Intel QuickSync

**Supported Codecs**:
- H.264/AVC (2nd gen Core and newer)
- H.265/HEVC (6th gen Core and newer)
- VP9 (9th gen Core and newer)

**Generations**:
- **Sandy Bridge** (2nd gen) - H.264 support
- **Skylake** (6th gen) - HEVC support
- **Ice Lake** (10th gen mobile) - Improved quality
- **Rocket Lake** (11th gen) - Enhanced features

**Performance**: Up to 4K @ 60 fps

**Quality**: Good, improving with each generation

---
### AMD AMF (Advanced Media Framework)
**Supported Codecs**:
- H.264/AVC (GCN 1.0 and newer)
- H.265/HEVC (Fiji/Polaris and newer)
**Generations**:
- **GCN 1-4** (R7/R9, RX 400/500) - H.264 only
- **Vega** (RX Vega) - HEVC support
- **RDNA** (RX 5000/6000) - Improved quality
**Performance**: Up to 4K @ 60 fps
**Quality**: Good, competitive with QuickSync
---

## Codec Selection Guide

### For Streaming (Live)

**Recommended**: H.264 (NVENC/QuickSync)
- **Reason**: Universal compatibility, low latency, hardware acceleration
- **Fallback**: H.264 (software)

**Settings**:
- 1080p @ 30fps: 4-6 Mbps
- 720p @ 30fps: 2.5-4 Mbps
- Low latency: Disable B-frames

---
### For Recording (High Quality)
**Recommended**: H.265 (NVENC/QuickSync) or H.264 (software)
- **Reason**: Best quality-to-size ratio
- **Alternative**: HEVC software for maximum quality
**Settings**:
- 4K @ 30fps: 15-25 Mbps (HEVC) or 35-50 Mbps (H.264)
- 1080p @ 60fps: 8-15 Mbps (HEVC) or 15-25 Mbps (H.264)
---

### For Web Delivery

**Recommended**: VP9 or H.264
- **Reason**: Browser compatibility, royalty-free (VP9)

**Settings**:
- VP9: 1080p @ 2-4 Mbps
- H.264: 1080p @ 4-6 Mbps

---
### For Audio
**Music**: AAC (128-192 kbps) or Opus (96-160 kbps)
**Voice**: Opus (16-32 kbps) or Speex (15-20 kbps)
**Archival**: FLAC (lossless)
**Podcast**: MP3 VBR (V4-V2, ~130-190 kbps) or AAC (128 kbps)
---

## Compatibility Matrix

| Codec | MP4 | MKV | AVI | WebM | OGG | MPEG-TS |
|-------|-----|-----|-----|------|-----|---------|
| **H.264** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **H.265** | ✓ | ✓ | ✗ | ✗ | ✗ | ✓ |
| **VP8** | ✗ | ✓ | ✗ | ✓ | ✗ | ✗ |
| **VP9** | ✗ | ✓ | ✗ | ✓ | ✗ | ✗ |
| **MPEG-2** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **AAC** | ✓ | ✓ | ✗ | ✗ | ✗ | ✓ |
| **MP3** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **Vorbis** | ✗ | ✓ | ✗ | ✓ | ✓ | ✗ |
| **Opus** | ✗ | ✓ | ✗ | ✓ | ✓ | ✗ |
| **FLAC** | ✓ | ✓ | ✗ | ✓ | ✓ | ✗ |

---
## See Also
- [Encoding Filters Pack Overview](index.md)
- [Muxers Reference](muxers-reference.md)
- [NVENC Interface Reference](interfaces/nvenc.md)
- [Code Examples](examples.md)