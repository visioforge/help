---
title: DirectShow Encoding Filters - Muxers Reference
description: DirectShow container formats reference for MP4, MKV, WebM, MPEG-TS, and AVI muxers with supported codecs and streaming options.
---

# Encoding Filters Pack - Muxers Reference

## Overview

This document provides comprehensive information about all container formats (muxers) supported by the DirectShow Encoding Filters Pack. Muxers combine video and audio streams into container files for storage and streaming.

---
## MP4 Container
### Overview
MPEG-4 Part 14 (MP4) is the most widely used container format for video distribution.
**File Extensions**: `.mp4`, `.m4v`, `.m4a` (audio only)
**MIME Type**: `video/mp4`, `audio/mp4`
### Supported Codecs
#### Video Codecs
- H.264/AVC ✓ (Primary)
- H.265/HEVC ✓
- MPEG-4 Part 2 ✓
- MPEG-2 ✗ (use MPEG-TS instead)
- VP8/VP9 ✗ (use WebM instead)
#### Audio Codecs
- AAC ✓ (Primary)
- MP3 ✓
- Opus ✗
- Vorbis ✗
- FLAC ✓
- PCM ✓
### Features
**Streaming Support**:
- **Progressive Download**: ✓ (with proper moov atom placement)
- **Adaptive Streaming**: ✓ (DASH, HLS with fragmented MP4)
- **Live Streaming**: ✓ (fragmented MP4)
**Metadata Support**:
- **Basic Tags**: Title, artist, album, year
- **Cover Art**: ✓
- **Chapters**: ✓
- **Subtitles**: ✓ (VTT, SRT, various text formats)
**Technical Features**:
- **Multiple Audio Tracks**: ✓
- **Multiple Subtitle Tracks**: ✓
- **Fast Start**: ✓ (moov atom at beginning)
- **Fragmented MP4**: ✓ (for streaming)
### Best Practices
**For Progressive Download (Web)**:
```
- Place moov atom at beginning (fast start)
- Use H.264 Baseline/Main profile
- AAC-LC audio
- Keyframe interval: 2-4 seconds
```
**For Local Playback**:
```
- H.264 High profile or H.265
- AAC-LC or HE-AAC
- Any keyframe interval
```
**For Streaming (DASH/HLS)**:
```
- Fragmented MP4
- H.264 Main/High profile
- AAC-LC audio
- Short fragments (2-6 seconds)
```
### Compatibility
| Platform/Device | Compatibility |
|----------------|---------------|
| **Windows Media Player** | ✓ |
| **VLC** | ✓ |
| **Web Browsers** | ✓ |
| **iOS/iPhone** | ✓ |
| **Android** | ✓ |
| **Smart TVs** | ✓ |
| **Game Consoles** | ✓ |
### Common Issues
**Issue**: Video not seekable on web
- **Solution**: Enable fast start (moov at beginning)
**Issue**: Audio sync problems
- **Solution**: Use constant frame rate, check audio sample rate
---

## MKV (Matroska) Container

### Overview

Matroska is an open-standard, feature-rich container format.

**File Extensions**: `.mkv` (video), `.mka` (audio), `.mks` (subtitles)

**MIME Type**: `video/x-matroska`, `audio/x-matroska`

### Supported Codecs

#### Video Codecs
- H.264/AVC ✓
- H.265/HEVC ✓
- VP8 ✓
- VP9 ✓
- MPEG-4 Part 2 ✓
- MPEG-2 ✓
- AV1 ✓

#### Audio Codecs
- AAC ✓
- MP3 ✓
- Opus ✓
- Vorbis ✓
- FLAC ✓
- DTS ✓
- AC-3 ✓
- PCM ✓

### Features

**Advanced Features**:
- **Multiple Video Tracks**: ✓
- **Multiple Audio Tracks**: ✓ (unlimited)
- **Multiple Subtitle Tracks**: ✓ (unlimited)
- **Attachments**: ✓ (fonts, cover art)
- **Chapters**: ✓ (with nesting)
- **Tags/Metadata**: ✓ (extensive)
- **Segmenting**: ✓ (linked segments)

**Technical Capabilities**:
- **Variable Frame Rate**: ✓
- **Lossless Audio**: ✓
- **3D/Stereoscopic**: ✓
- **HDR Metadata**: ✓

### Best Practices

**For Archival**:
```
- Use FLAC or PCM for lossless audio
- Include all audio/subtitle tracks
- Add chapter markers
- Include metadata tags
```

**For Distribution**:
```
- H.264/H.265 video
- AAC audio (most compatible)
- Embedded soft subtitles
- Reasonable file size
```

**For Streaming**:
```
- Not ideal for streaming
- Consider MP4 or WebM instead
- If used: disable complex features
```

### Compatibility

| Platform/Device | Compatibility |
|----------------|---------------|
| **Windows Media Player** | ✗ (codec pack required) |
| **VLC** | ✓ |
| **Web Browsers** | ✗ (no native support) |
| **iOS/iPhone** | ✗ (3rd party apps only) |
| **Android** | Limited (app-dependent) |
| **Smart TVs** | Limited (model-dependent) |
| **Media Players** | ✓ (Kodi, Plex, etc.) |

### Common Issues

**Issue**: Seeking is slow
- **Solution**: Enable cues (index) during muxing

**Issue**: Playback stuttering with high-quality audio
- **Solution**: Check decoder performance, consider AAC instead of lossless

---
## WebM Container
### Overview
WebM is an open, royalty-free format designed for web use.
**File Extensions**: `.webm`
**MIME Type**: `video/webm`, `audio/webm`
### Supported Codecs
#### Video Codecs
- VP8 ✓ (WebM 1.0)
- VP9 ✓ (WebM 2.0)
- AV1 ✓ (experimental)
- H.264 ✗
- H.265 ✗
#### Audio Codecs
- Vorbis ✓ (Primary)
- Opus ✓ (Recommended)
- AAC ✗
- MP3 ✗
### Features
**Web Optimized**:
- **HTML5 Video**: ✓ (native browser support)
- **Streaming**: ✓
- **Adaptive Streaming**: ✓ (DASH)
- **Low Latency**: ✓
**Metadata Support**:
- **Basic Tags**: ✓
- **Chapters**: ✓
- **Subtitles**: ✓ (WebVTT)
### Best Practices
**For YouTube/Web**:
```
- VP9 video codec
- Opus audio codec (96-160 kbps)
- Keyframe interval: 2-4 seconds
- Two-pass encoding for best quality
```
**For Live Streaming**:
```
- VP8 for better encoder performance
- Opus audio (low latency mode)
- CBR bitrate
- Short GOP
```
**For High Quality**:
```
- VP9 with high bitrate
- Opus 128-256 kbps
- Two-pass encoding
- Quality-based rate control
```
### Compatibility
| Platform/Device | Compatibility |
|----------------|---------------|
| **Chrome** | ✓ |
| **Firefox** | ✓ |
| **Edge** | ✓ |
| **Safari** | Limited (VP8 only) |
| **Android** | ✓ |
| **iOS** | Limited |
### Common Issues
**Issue**: Safari won't play WebM
- **Solution**: Provide MP4 fallback with H.264
**Issue**: Encoding too slow
- **Solution**: Use VP8 instead of VP9, or hardware-accelerated VP9 if available
---

## MPEG-TS (Transport Stream)

### Overview

MPEG Transport Stream is designed for broadcast and streaming, especially where error resilience is important.

**File Extensions**: `.ts`, `.mts`, `.m2ts`

**MIME Type**: `video/mp2t`

### Supported Codecs

#### Video Codecs
- H.264/AVC ✓
- H.265/HEVC ✓
- MPEG-2 ✓
- VP8/VP9 ✗

#### Audio Codecs
- AAC ✓
- MP3 ✓
- AC-3 ✓
- PCM ✓

### Features

**Broadcast Features**:
- **Error Resilience**: ✓ (built-in error recovery)
- **Time-shifting**: ✓
- **Program Multiplexing**: ✓ (multiple programs in one stream)
- **Encryption**: ✓ (conditional access)

**Streaming Features**:
- **HLS Streaming**: ✓ (Apple HTTP Live Streaming)
- **DVB Broadcasting**: ✓
- **IPTV**: ✓

### Best Practices

**For HLS Streaming**:
```
- H.264 video
- AAC audio
- Segment duration: 6-10 seconds
- CBR encoding
- Closed GOP
```

**For Broadcasting**:
```
- MPEG-2 or H.264
- AC-3 or AAC audio
- Constant bitrate
- Fixed packet size (188 bytes)
```

### Compatibility

| Platform/Device | Compatibility |
|----------------|---------------|
| **HLS Players** | ✓ |
| **Set-top Boxes** | ✓ |
| **Smart TVs** | ✓ |
| **VLC** | ✓ |
| **Web Browsers** | Via HLS support |

---
## FLV (Flash Video)
### Overview
Legacy format formerly used for web video (YouTube, Flash players).
**File Extensions**: `.flv`, `.f4v`
**MIME Type**: `video/x-flv`
**Status**: ⚠️ Deprecated - Use MP4 or WebM instead
### Supported Codecs
#### Video Codecs
- H.264 ✓
- VP6 ✓ (legacy)
- Sorenson Spark ✓ (legacy)
#### Audio Codecs
- AAC ✓
- MP3 ✓
- Speex ✓
### Features
- **Streaming**: ✓ (RTMP)
- **Metadata**: Basic (onMetaData)
- **Cue Points**: ✓
**Not Recommended**: Flash Player end-of-life (2020) makes FLV obsolete
---

## OGG Container

### Overview

Open-source container primarily for Vorbis audio.

**File Extensions**: `.ogg`, `.oga` (audio), `.ogv` (video)

**MIME Type**: `audio/ogg`, `video/ogg`

### Supported Codecs

#### Video Codecs
- Theora ✓ (legacy quality)
- VP8 ✗ (use WebM)

#### Audio Codecs
- Vorbis ✓ (Primary)
- Opus ✓
- FLAC ✓
- Speex ✓

### Features

- **Streaming**: ✓
- **Chaining**: ✓ (multiple files in sequence)
- **Metadata**: ✓ (Vorbis comments)

### Best Practices

**For Audio**:
```
- Vorbis or Opus codec
- Quality-based encoding
- Vorbis comments for metadata
```

**For Video**:
```
- Not recommended
- Use WebM (VP8/VP9) instead
```

### Compatibility

| Platform/Device | Compatibility |
|----------------|---------------|
| **Firefox** | ✓ |
| **Chrome** | ✓ |
| **VLC** | ✓ |
| **Most mobile devices** | Limited |

---
## AVI (Audio Video Interleave)
### Overview
Legacy Microsoft container format.
**File Extensions**: `.avi`
**MIME Type**: `video/x-msvideo`
**Status**: ⚠️ Legacy - Use MP4 or MKV for new projects
### Supported Codecs
#### Video Codecs
- H.264 ✓ (limited support)
- MPEG-4 Part 2 ✓
- MPEG-2 ✓
- Various legacy codecs ✓
#### Audio Codecs
- MP3 ✓
- PCM ✓
- AC-3 ✓
- AAC Limited
### Limitations
- **Max File Size**: 2 GB (without OpenDML)
- **Limited Metadata**: Very basic
- **No Streaming**: Not designed for streaming
- **No Chapters**: Not supported
### When to Use
- Legacy system compatibility
- Capture from old hardware
- Specific software requirements
**Recommendation**: Use MP4 or MKV for new projects
---

## WAV Container

### Overview

Uncompressed audio container.

**File Extensions**: `.wav`

**MIME Type**: `audio/wav`, `audio/x-wav`

### Features

- **Lossless**: ✓ (PCM)
- **Compressed**: ✓ (MP3, AAC in WAV wrapper)
- **Metadata**: Limited (RIFF tags)

### Common Formats

- **PCM 44.1 kHz 16-bit**: CD quality
- **PCM 48 kHz 24-bit**: Professional audio
- **PCM 96 kHz 24-bit**: High-resolution audio

### Best Practices

**For Audio Production**:
```
- 48 kHz, 24-bit PCM
- Mono or stereo
- Avoid compression
```

**For Distribution**:
```
- Use FLAC or AAC instead
- WAV files are large
```

---
## Container Selection Guide
### For Web Delivery
**Primary**: MP4 (H.264 + AAC)
- **Reason**: Universal compatibility
- **Fallback**: WebM (VP9 + Opus) for modern browsers
### For Professional Archival
**Primary**: MKV (H.265 + FLAC)
- **Reason**: Feature-rich, lossless audio support
- **Alternative**: MP4 (H.265 + AAC) for better compatibility
### For Broadcast/IPTV
**Primary**: MPEG-TS (H.264 + AAC)
- **Reason**: Error resilience, industry standard
- **Alternative**: MPEG-TS (MPEG-2 + AC-3) for legacy systems
### For Live Streaming
**HLS**: MPEG-TS segments (H.264 + AAC)
**DASH**: Fragmented MP4 (H.264 + AAC)
**WebRTC**: Opus audio, VP8/H.264 video
### For Audio-Only
**High Quality**: FLAC (.flac) or MP3 VBR (.mp3)
**Streaming**: AAC in MP4 (.m4a) or Opus in WebM
**Voice**: Opus in OGG or Speex
---

## Format Comparison Table

| Feature | MP4 | MKV | WebM | MPEG-TS | FLV | OGG |
|---------|-----|-----|------|---------|-----|-----|
| **Web Compatibility** | ★★★★★ | ★☆☆☆☆ | ★★★★☆ | ★★☆☆☆ | ☆☆☆☆☆ | ★★☆☆☆ |
| **Mobile Compatibility** | ★★★★★ | ★★☆☆☆ | ★★★☆☆ | ★★★★☆ | ☆☆☆☆☆ | ★☆☆☆☆ |
| **Streaming Support** | ★★★★★ | ★★☆☆☆ | ★★★★★ | ★★★★★ | ★★★☆☆ | ★★★☆☆ |
| **Feature Richness** | ★★★★☆ | ★★★★★ | ★★★☆☆ | ★★★☆☆ | ★★☆☆☆ | ★★☆☆☆ |
| **Codec Support** | ★★★★☆ | ★★★★★ | ★★☆☆☆ | ★★★☆☆ | ★★☆☆☆ | ★★★☆☆ |
| **File Size Efficiency** | ★★★★☆ | ★★★★☆ | ★★★★★ | ★★★☆☆ | ★★★☆☆ | ★★★★☆ |
| **Error Resilience** | ★★☆☆☆ | ★★☆☆☆ | ★★☆☆☆ | ★★★★★ | ★★★☆☆ | ★★☆☆☆ |

---
## Technical Specifications
### MP4 Structure
```
ftyp (file type)
moov (metadata - place at beginning for fast start)
  ├── mvhd (movie header)
  ├── trak (video track)
  ├── trak (audio track)
  └── udta (user data/metadata)
mdat (media data)
```
### Fragmented MP4 (for streaming)
```
ftyp
moov
  └── mvex (movie extends)
moof (movie fragment)
  └── traf (track fragment)
mdat (fragment data)
[repeat moof/mdat for each fragment]
```
### MKV Structure
```
EBML Header
Segment
  ├── SeekHead (index)
  ├── Info (segment information)
  ├── Tracks (track definitions)
  ├── Chapters (optional)
  ├── Attachments (optional)
  ├── Tags (metadata)
  └── Cluster (media data)
```
---

## See Also

- [Encoding Filters Pack Overview](index.md)
- [Codecs Reference](codecs-reference.md)
- [Code Examples](examples.md)
- [NVENC Interface Reference](interfaces/nvenc.md)
