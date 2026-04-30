---
title: How to Connect to Grandstream IP Camera in C# .NET
description: Integrate Grandstream GXV and GSC cameras into C# .NET apps via RTSP. URL patterns for GXV3500, GXV3610, GSC3610, plus VisioForge SDK examples.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# How to Connect to Grandstream IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Grandstream Networks** is an American company headquartered in Boston, Massachusetts, USA, known for VoIP phones and IP surveillance products. Grandstream offers IP cameras and video encoders under the **GXV** (legacy) and **GSC** (current generation) product lines, targeting SMB and professional markets. Their cameras are often deployed alongside Grandstream VoIP and UCM (Unified Communications Manager) systems.

**Key facts:**

- **Product lines:** GXV (IP cameras and video encoders, legacy), GSC (current generation smart cameras)
- **Protocol support:** RTSP, ONVIF (GXV36xx and newer GSC series), HTTP/CGI, SIP (video calling)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (GXV36xx and newer GSC models)
- **Video codecs:** H.264, H.265 (current GSC models), MPEG-4 (legacy GXV models)

## RTSP URL Patterns

### Current GSC-Series Cameras

Current generation Grandstream GSC cameras use a channel-based URL format:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Primary stream | `rtsp://IP:554/live/ch00_0` | Main stream, channel 0 |
| Secondary stream | `rtsp://IP:554/live/ch00_1` | Sub stream |

### GXV-Series (Legacy)

Older GXV cameras support multiple URL formats depending on the model:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Primary stream | `rtsp://IP:554//0` | Main stream (channel 0) |
| Secondary stream | `rtsp://IP:554//4` | Sub stream (channel 4) |
| H.264 SDP | `rtsp://IP:554/ipcam_h264.sdp` | SDP file-based access |
| Live H.264 | `rtsp://IP:554/live/h264` | Named stream |
| Channel-based | `rtsp://IP:554/[CHANNEL]` | Direct channel number |
| Auth stream | `rtsp://IP:554//0/888888:888888/main` | With embedded credentials |
| MPEG-4 (legacy) | `rtsp://IP:554/cam1/mpeg4?user=USER&pwd=PASS` | Legacy MPEG-4 stream |

!!! info "Unusual channel numbering"
    Grandstream uses a non-standard channel numbering scheme. For single-channel cameras, channel **0** is the primary stream and channel **4** is the secondary stream. This differs from most other brands that use sequential numbering.

### Model-Specific URLs

| Model | Primary Stream | Secondary Stream | Type |
|-------|---------------|-----------------|------|
| GXV3500 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Video encoder |
| GXV3504 (ch 1) | `rtsp://IP:554/0` | `rtsp://IP:554/4` | 4-channel encoder |
| GXV3504 (ch 2) | `rtsp://IP:554/1` | `rtsp://IP:554/5` | 4-channel encoder |
| GXV3504 (ch 3) | `rtsp://IP:554/2` | `rtsp://IP:554/6` | 4-channel encoder |
| GXV3504 (ch 4) | `rtsp://IP:554/3` | `rtsp://IP:554/7` | 4-channel encoder |
| GXV3601 / GXV3611 | `rtsp://IP:554//4` | -- | Dome camera |
| GXV3601 (alt) | `rtsp://IP:554/ipcam_h264.sdp` | -- | SDP-based |
| GXV3610 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Dome HD |
| GXV3651 / GXV3661 / GXV3662 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | FHD cameras |
| GXV3672 | `rtsp://IP:554//0` | `rtsp://IP:554/live/ch00_0` | HD/FHD outdoor |
| GSC3610 / GSC3615 | `rtsp://IP:554/live/ch00_0` | `rtsp://IP:554/live/ch00_1` | Current dome |
| GSC3620 | `rtsp://IP:554/live/ch00_0` | `rtsp://IP:554/live/ch00_1` | Current outdoor |

### Multi-Channel Encoder (GXV3504) Channel Map

The GXV3504 is a 4-channel video encoder with the following channel numbering:

| Input | Primary Channel | Secondary Channel |
|-------|----------------|-------------------|
| Input 1 | 0 | 4 |
| Input 2 | 1 | 5 |
| Input 3 | 2 | 6 |
| Input 4 | 3 | 7 |

## Connecting with VisioForge SDK

Use your Grandstream camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Grandstream GSC-series camera, main stream
var uri = new Uri("rtsp://192.168.1.60:554/live/ch00_0");
var username = "admin";
var password = "YourPassword";
```

For legacy GXV models, use `rtsp://IP:554//0` for the primary stream or `rtsp://IP:554//4` for the secondary stream.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/snapshot/view0.jpg` | Channel 0 snapshot |
| JPEG Snapshot (ch 1) | `http://IP/snapshot/view1.jpg` | Channel 1 (multi-channel) |
| HTTP Stream | `http://IP/goform/stream?cmd=get&channel=0` | Channel-based HTTP stream |

## Troubleshooting

### Channel numbering confusion

Grandstream channel numbering is unconventional:

- **Single-channel cameras:** Channel 0 = primary stream, Channel 4 = secondary stream
- **GXV3504 (4-channel encoder):** Channels 0-3 are primary streams for inputs 1-4; Channels 4-7 are secondary streams for inputs 1-4

If you get a blank stream or error, double-check you are using the correct channel number for your desired stream quality.

### Factory default credential `888888`

Some older Grandstream GXV models use `888888` as the default password (or embedded in the RTSP URL as `888888:888888`). If `admin` / `admin` does not work, try `888888` as the password.

### RTSP not enabled

On some older GXV models, RTSP streaming must be explicitly enabled in the camera's web interface. Navigate to the streaming or media settings page and confirm that RTSP is turned on and set to port 554.

### Multiple URL formats per model

Many GXV cameras support several RTSP URL formats simultaneously. If one format does not work, try the alternatives:

1. `rtsp://IP:554//0` (channel number with double slash)
2. `rtsp://IP:554/live/ch00_0` (named channel)
3. `rtsp://IP:554/ipcam_h264.sdp` (SDP file)
4. `rtsp://IP:554/live/h264` (named stream)

### Codec compatibility

Current GSC-series cameras support H.265 and H.264. Legacy GXV models may default to MPEG-4. If you experience decoding issues with a legacy model, check the camera's web interface and switch the codec to H.264 if available.

## FAQ

**What is the default RTSP URL for Grandstream cameras?**

For current GSC-series cameras, use `rtsp://admin:password@CAMERA_IP:554/live/ch00_0`. For older GXV-series cameras, try `rtsp://admin:password@CAMERA_IP:554//0` for the primary stream.

**Do Grandstream cameras support ONVIF?**

Yes, the GXV36xx series and current GSC-series cameras support ONVIF. Older GXV35xx models and video encoders generally do not support ONVIF.

**What is the difference between channel 0 and channel 4?**

On single-channel Grandstream cameras, channel 0 is the primary (high quality) stream and channel 4 is the secondary (lower quality) stream. This is a Grandstream-specific convention that differs from most other camera brands.

**Can I use Grandstream cameras with a UCM system?**

Yes. Grandstream cameras integrate natively with Grandstream UCM (Unified Communications Manager) systems. However, RTSP access works independently of the UCM and can be used with any third-party software including VisioForge SDKs.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Milesight Connection Guide](milesight.md) — SMB / professional camera segment
- [ONVIF IP Camera Integration](../videocapture/video-sources/ip-cameras/onvif.md) — Grandstream ONVIF device setup
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
