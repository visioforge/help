---
title: Zavio IP Camera RTSP URL Guide for C# .NET Integration
description: Zavio bullet, dome, and PTZ camera RTSP URL patterns for C# .NET. ONVIF-compatible integration with VisioForge SDK code for all Zavio models.
---

# How to Connect to Zavio IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Zavio** (Zavio Inc.) is a Taiwanese IP camera manufacturer headquartered in Hsinchu, Taiwan. Zavio is known for professional-grade network cameras with distinctive URL patterns that include both direct stream paths and profile-based paths. The company targets SMB and professional security markets with a range of bullet, dome, fixed, mini, and PTZ camera models.

**Key facts:**

- **Product lines:** B (bullet), D (dome), F (fixed/box), M (mini), P (PTZ/pan-tilt), V (vandal-proof)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI, MJPEG
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (most models)
- **Video codecs:** H.264, MPEG-4, MJPEG
- **Dual URL patterns:** Some models use `/video.mp4`, others use `/video.proN` (profile-based)

!!! tip "Profile-Based URLs"
    Zavio cameras support profile-based URLs. Use `/video.pro1` for the primary profile and `/video.pro2` for the secondary profile. The available profiles depend on your camera's configuration.

## RTSP URL Patterns

### Standard URL Format

Zavio cameras support two primary RTSP URL patterns:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video.mp4
```

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//video.pro1
```

| URL Path | Description |
|----------|-------------|
| `/video.mp4` | MP4 main stream (most common format) |
| `//video.pro1` | Profile 1 / primary stream (double-slash prefix) |
| `//video.pro2` | Profile 2 / sub stream (double-slash prefix) |
| `//video.h264` | Direct H.264 stream (some models) |

### Camera Models

| Model | Type | Main Stream URL | Notes |
|-------|------|----------------|-------|
| B5110 (bullet) | Bullet | `rtsp://IP//video.pro1` | Profile-based, also supports `//video.h264` |
| B5210 (bullet) | Bullet | `rtsp://IP//video.pro1` | Profile-based |
| B7110 (bullet) | Bullet | `rtsp://IP:554/video.mp4` | MP4 main stream |
| B7210 (bullet) | Bullet | `rtsp://IP:554/video.mp4` | MP4 main stream |
| D3100 (dome) | Dome | `rtsp://IP//video.pro1` | Profile-based |
| D3200 (dome) | Dome | `rtsp://IP:554/video.mp4` | MP4 main stream |
| D4210 (dome) | Dome | `rtsp://IP:554/video.mp4` | MP4 main stream |
| D50E (dome) | Dome | `rtsp://IP:554/video.mp4` | MP4 main stream |
| D510E (dome) | Dome | `rtsp://IP:554/video.mp4` | MP4 main stream |
| D520E (dome) | Dome | `rtsp://IP:554/video.mp4` | MP4 main stream |
| D7111 (dome) | Dome | `rtsp://IP:554//video.pro2` | Profile 2 sub stream |
| D7210 (dome) | Dome | `rtsp://IP:554//video.pro2` | Profile 2 sub stream |
| F1100 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F1105 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F1150 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F210A (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F3100 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F3102 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F3110 (fixed) | Fixed | `rtsp://IP:554//video.pro2` | Profile 2 sub stream |
| F3115 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F312A (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F3201 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F3206 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F3210 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F3215 (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F511E (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F520IE (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F521E (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| F731E (fixed) | Fixed | `rtsp://IP:554/video.mp4` | MP4 main stream |
| M510W (mini) | Mini | `rtsp://IP:554/video.mp4` | Wireless mini camera |
| M511E (mini) | Mini | `rtsp://IP:554/video.mp4` | Mini camera |
| P5110 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |
| P5115 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |
| P5210 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |

### Alternative URL Formats

Some Zavio models support these alternative URLs:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/video.mp4` | MP4 stream (recommended for most models) |
| `rtsp://IP//video.pro1` | Profile 1, primary stream |
| `rtsp://IP:554//video.pro2` | Profile 2, sub stream |
| `rtsp://IP//video.h264` | Direct H.264 stream (B5110 and similar) |

## Connecting with VisioForge SDK

Use your Zavio camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Zavio B7110, MP4 main stream
var uri = new Uri("rtsp://192.168.1.90:554/video.mp4");
var username = "admin";
var password = "admin";
```

For profile-based cameras, use `//video.pro1` for the primary stream or `//video.pro2` for the sub stream.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| Profile Snapshot | `http://IP/cgi-bin/view/image?pro_CHANNEL` | Snapshot by profile number |
| JPEG Snapshot | `http://IP/jpg/image.jpg` | Standard JPEG snapshot |
| Sized JPEG | `http://IP/jpg/image.jpg?size=3` | JPEG with size parameter |
| CGI JPEG | `http://IP/cgi-bin/jpg/image` | CGI-based JPEG snapshot |
| MJPEG Stream | `http://IP/video.mjpg` | Continuous MJPEG stream |
| MJPEG (quality/FPS) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | MJPEG with quality and FPS control |
| Profile HTTP Stream | `http://IP/stream?uri=video.proN` | Profile-based HTTP stream |

## Troubleshooting

### "401 Unauthorized" error

Zavio cameras ship with default credentials of `admin` / `admin`. If the camera has been configured with different credentials:

1. Access the camera at `http://CAMERA_IP` in a browser
2. Log in and check **Network > RTSP** settings
3. Verify that RTSP authentication is enabled and your credentials are correct

### Choosing between /video.mp4 and /video.proN

Zavio cameras have two URL families. The correct choice depends on your model:

- **Most models** (B7110, F210A, F312A, F520IE, F521E, F731E, etc.): Use `/video.mp4`
- **Older or profile-based models** (B5110, B5210, D3100): Use `//video.pro1`
- If one format fails, try the other

### Double-slash in profile URLs

Profile-based Zavio URLs require a double-slash (`//`) before `video.proN`. This is intentional:

- Correct: `rtsp://IP//video.pro1`
- Incorrect: `rtsp://IP/video.pro1`

If you omit the double-slash on a profile-based model, the connection may fail.

### No video with MPEG-4 codec

Some older Zavio models default to MPEG-4 encoding. If you experience codec issues:

- Log in to the camera web interface
- Change the video codec to **H.264** under the stream configuration
- Use the `/video.mp4` or `//video.pro1` URL after changing the setting

### Port 554 connection refused

Verify that RTSP is enabled on the camera:

- Web interface: Check **Network > RTSP** settings
- Confirm port 554 is not blocked by a firewall
- Default RTSP port is 554

## FAQ

**What is the default RTSP URL for Zavio cameras?**

The most common URL is `rtsp://admin:admin@CAMERA_IP:554/video.mp4` for the MP4 main stream. For profile-based models, use `rtsp://admin:admin@CAMERA_IP//video.pro1` instead.

**Do Zavio cameras support ONVIF?**

Yes. Most Zavio models support ONVIF, which provides a standardized method for camera discovery and streaming without needing brand-specific URL patterns.

**What is the difference between /video.mp4 and /video.pro1?**

`/video.mp4` is a direct stream path used by most newer Zavio models. `//video.pro1` and `//video.pro2` are profile-based paths that reference stream profiles configured in the camera's web interface. Profile 1 is typically the main (high-resolution) stream, and Profile 2 is typically the sub (lower-resolution) stream.

**What are the default login credentials for Zavio cameras?**

The default username is `admin` and the default password is `admin`. It is strongly recommended to change these credentials after initial setup.

**Can I control MJPEG quality and frame rate?**

Yes. Zavio cameras support MJPEG parameters in the URL. Use `http://IP/video.mjpg?q=30&fps=33&id=0.5` to specify quality (`q`), frames per second (`fps`), and stream identifier (`id`).

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Edimax Connection Guide](edimax.md) — Taiwanese SMB cameras
- [ONVIF Capture with Postprocessing](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Zavio ONVIF capture pipeline
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
