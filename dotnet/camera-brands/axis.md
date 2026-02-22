---
title: Axis IP Camera RTSP URL Format and C# .NET Integration
description: Connect to Axis Communications cameras in C# .NET with RTSP URL patterns, VAPIX API, and code samples for M, P, Q, and F series models.
---

# How to Connect to Axis IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Axis Communications** is a Swedish manufacturer widely regarded as the pioneer of network cameras, having created the world's first IP camera in 1996. Headquartered in Lund, Sweden and now a subsidiary of Canon, Axis produces premium IP cameras, encoders, and network audio products primarily for the professional and enterprise surveillance market.

**Key facts:**

- **Product lines:** M-series (compact/mini), P-series (fixed), Q-series (professional), F-series (modular), V-series (vandal-resistant), PTZ cameras
- **Protocol support:** ONVIF Profile S/G/T, RTSP, VAPIX (Axis proprietary HTTP API), HTTP/MJPEG
- **Default RTSP port:** 554
- **Default credentials:** root / (set during initial setup; older firmware: root / pass)
- **ONVIF support:** Full -- Axis was a founding member of ONVIF
- **Video codecs:** H.264, H.265 (newer models), MJPEG
- **Unique features:** VAPIX HTTP API for comprehensive camera control, ACAP (Axis Camera Application Platform)

## RTSP URL Patterns

Axis cameras use the `axis-media/media.amp` RTSP path with optional parameters for resolution and codec control.

### URL Format

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:[PORT]/axis-media/media.amp
```

### Primary RTSP URLs

| Model Series | RTSP URL | Codec | Audio |
|-------------|----------|-------|-------|
| All modern models | `rtsp://IP:554/axis-media/media.amp` | H.264 (default) | Possible |
| All modern models | `rtsp://IP:554/axis-media/media.amp?videocodec=h264` | H.264 (explicit) | Possible |
| All modern models | `rtsp://IP:554/axis-media/media.amp?videocodec=h265` | H.265 | Possible |
| ONVIF profile | `rtsp://IP:554/onvif-media/media.amp` | H.264 | Yes |
| Legacy models | `rtsp://IP:554/mpeg4/media.amp` | MPEG-4 | Possible |

### Stream Profile Selection

Axis cameras support named stream profiles that can be selected via URL parameter:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Quality` | High quality profile |
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Balanced` | Balanced profile |
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Bandwidth` | Low bandwidth profile |
| `rtsp://IP:554/axis-media/media.amp?resolution=1920x1080` | Explicit resolution |
| `rtsp://IP:554/axis-media/media.amp?resolution=640x480` | Lower resolution |
| `rtsp://IP:554/axis-media/media.amp?fps=15` | Frame rate limit |

### Multi-Channel Models (Encoders, Multi-Sensor)

For multi-channel devices like video encoders (M7001, P7214) and multi-sensor cameras:

| Device | RTSP URL | Channel |
|--------|----------|---------|
| Channel 1 | `rtsp://IP:554/axis-media/media.amp?camera=1` | 1 |
| Channel 2 | `rtsp://IP:554/axis-media/media.amp?camera=2` | 2 |
| Channel 3 | `rtsp://IP:554/axis-media/media.amp?camera=3` | 3 |
| Channel 4 | `rtsp://IP:554/axis-media/media.amp?camera=4` | 4 |

### Legacy URL Formats

Older Axis cameras (200-series, early 1000-series) may require these formats:

| URL Pattern | Models | Notes |
|-------------|--------|-------|
| `rtsp://IP:554/mpeg4/media.amp` | 200, 205, 206, 207 | MPEG-4 stream |
| `http://IP/axis-cgi/mjpg/video.cgi` | All models | MJPEG over HTTP |
| `http://IP/mjpg/video.mjpg` | 200-series | Direct MJPEG stream |
| `http://IP/axis-cgi/mjpg/video.cgi?camera=1` | Multi-channel | Specific channel |
| `http://IP/axis-cgi/mjpg/video.cgi?resolution=640x480` | All models | Resolution-specific |

## Connecting with VisioForge SDK

Use your Axis camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Axis camera, H.264 main stream
var uri = new Uri("rtsp://192.168.1.50:554/axis-media/media.amp");
var username = "root";
var password = "YourPassword";
```

For sub-stream access, add `?resolution=640x480` parameter.

### ONVIF Discovery

Axis was a founding member of ONVIF and has industry-leading ONVIF compliance. See the [ONVIF integration guide](../mediablocks/Sources/index.md) for discovery code examples.

## Snapshot and MJPEG URLs (VAPIX API)

Axis cameras provide the VAPIX HTTP API, which is more feature-rich than most other brands:

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/axis-cgi/jpg/image.cgi` | Current frame |
| Snapshot (sized) | `http://IP/axis-cgi/jpg/image.cgi?resolution=1920x1080` | Specific resolution |
| Snapshot (with overlay) | `http://IP/axis-cgi/jpg/image.cgi?date=1&clock=1` | Date/time overlay |
| Snapshot (camera select) | `http://IP/axis-cgi/jpg/image.cgi?camera=1` | Multi-channel device |
| Simple snapshot | `http://IP/jpg/image.jpg` | Basic JPEG capture |
| Sized snapshot | `http://IP/jpg/image.jpg?size=3` | Predefined size (1-5) |
| MJPEG stream | `http://IP/axis-cgi/mjpg/video.cgi` | Continuous MJPEG |
| MJPEG (resolution) | `http://IP/axis-cgi/mjpg/video.cgi?resolution=640x480` | Sized MJPEG |
| MJPEG (direct) | `http://IP/mjpg/video.mjpg` | Direct MJPEG (legacy) |

## Troubleshooting

### Audio "Possible" vs "Yes"

Axis marks audio support as "Possible" on many RTSP streams because audio availability depends on the camera model having a built-in microphone or external audio input. The RTSP URL is the same whether audio is present or not -- the SDK will automatically detect and use audio if available.

### "401 Unauthorized" errors

- Axis cameras default to digest authentication for RTSP
- Ensure you're using the correct credentials (default username is `root`, not `admin`)
- On newer firmware, password must meet complexity requirements (minimum 8 characters)

### MPEG-4 stream not available on newer models

Modern Axis cameras (firmware 5.x+) have dropped MPEG-4 support. Use `/axis-media/media.amp` (H.264) instead of `/mpeg4/media.amp`.

### Resolution not matching expected output

Axis cameras negotiate resolution dynamically. To force a specific resolution, add the `resolution` parameter:
`rtsp://IP:554/axis-media/media.amp?resolution=1920x1080`

### Multi-channel encoder connections

When connecting to an Axis encoder (M7001, P7214, etc.), you must specify the camera/channel parameter. Without it, you get channel 1 by default.

## FAQ

**What is the default RTSP URL for Axis cameras?**

The standard URL is `rtsp://root:password@CAMERA_IP:554/axis-media/media.amp`. This works for virtually all modern Axis cameras (M, P, Q, F, V series). The default username is `root` (not `admin` like most other brands).

**How do I switch between H.264 and H.265 on Axis cameras?**

Add the `videocodec` parameter to the RTSP URL: `rtsp://IP:554/axis-media/media.amp?videocodec=h265` for H.265, or `videocodec=h264` for H.264. Note that H.265 is only available on newer Axis models with Artpec-7 or newer chipsets.

**Can I control stream quality via the RTSP URL?**

Yes. Axis supports several URL parameters: `resolution` (e.g., `1920x1080`), `fps` (frame rate), `compression` (0-100), and `streamprofile` (named profiles configured in the camera). Example: `rtsp://IP:554/axis-media/media.amp?resolution=1280x720&fps=15`.

**Why does Axis use "root" as the default username instead of "admin"?**

Axis cameras run embedded Linux, and following Unix conventions, the administrative user is named `root`. This is different from most other camera brands that use `admin`.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Bosch Connection Guide](bosch.md) — Enterprise surveillance peer
- [Hanwha Vision Connection Guide](hanwha.md) — Enterprise surveillance peer
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
