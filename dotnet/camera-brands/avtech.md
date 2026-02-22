---
title: AVTech IP Camera RTSP URL and C# .NET Connection Guide
description: AVTech IP camera integration guide for C# .NET with RTSP URL patterns, DVR/NVR channel URLs, and code samples for AVM, AVN, AVC, and AVI models.
---

# How to Connect to AVTech IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**AVTech** (AVTech Corporation) is a Taiwanese surveillance equipment manufacturer based in Taipei, Taiwan, founded in 1996. AVTech is one of the largest DVR/NVR manufacturers globally, with a strong presence in Asia-Pacific, Middle East, and Latin American markets. The company produces a wide range of IP cameras, DVRs, NVRs, and the EagleEyes mobile viewing platform. AVTech is known for offering cost-effective surveillance solutions with broad model compatibility.

**Key facts:**

- **Product lines:** AVM (IP cameras), AVN (network cameras), AVC (DVRs), AVI (specialty cameras), EagleEyes (mobile app)
- **Protocol support:** RTSP, ONVIF (newer models), HTTP/CGI, MJPEG
- **Default RTSP port:** 554 (some models use port 88)
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (newer models)
- **Video codecs:** H.264, MPEG-4, MJPEG
- **Guest access:** Many models allow unauthenticated JPEG snapshots via guest CGI

!!! note "Some AVTech models use port 88"
    Some newer AVTech models use port 88 instead of 554 for RTSP. If port 554 doesn't work, try port 88 with the URL pattern `rtsp://IP:88//live/h264_ulaw/VGA`.

!!! warning "Guest access security"
    Many AVTech cameras expose a guest CGI endpoint (`/cgi-bin/guest/Video.cgi`) that allows unauthenticated snapshot access. Ensure your camera's guest access settings are configured securely.

## RTSP URL Patterns

### Standard URL Format

AVTech cameras use the `/live/` path-based URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live/h264
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `/live/h264` | H.264 stream | Main H.264 video stream |
| `/live/mpeg4` | MPEG-4 stream | Legacy MPEG-4 video stream |
| `/live/h264/ch[N]` | Channel N | Channel-specific stream for DVRs/NVRs |

### Camera Models

| Model | Type | Main Stream URL | Notes |
|-------|------|----------------|-------|
| AVM217 | IP camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVM328 | IP dome | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVM357 | IP dome | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVM457 | IP camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVM459 | IP camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVM552 | IP camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVM561 | IP dome | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVM571 | IP camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN211 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN252 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN257 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN304 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN314 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN362 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN801 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN812 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVN813 | Network camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVI201 | IP camera | `rtsp://IP:554/live/h264` | H.264 main stream |
| AVI203 | IP camera | `rtsp://IP:554/live/h264` | H.264 main stream |

### DVR/NVR Channel URLs

For AVTech DVRs and NVRs (AVC series and others):

| Channel | Main Stream (H.264) | Main Stream (MPEG-4) |
|---------|---------------------|----------------------|
| Channel 1 | `rtsp://IP:554/live/h264/ch1` | `rtsp://IP:554/live/mpeg4/ch1` |
| Channel 2 | `rtsp://IP:554/live/h264/ch2` | `rtsp://IP:554/live/mpeg4/ch2` |
| Channel 3 | `rtsp://IP:554/live/h264/ch3` | `rtsp://IP:554/live/mpeg4/ch3` |
| Channel N | `rtsp://IP:554/live/h264/chN` | `rtsp://IP:554/live/mpeg4/chN` |

### Alternative URL Formats

Some AVTech models, particularly newer ones, use port 88 and different path formats:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/live/h264` | Standard H.264 (recommended) |
| `rtsp://IP:554/live/mpeg4` | MPEG-4 stream |
| `rtsp://IP//live/h264` | Without explicit port (some models) |
| `rtsp://IP:88//live/h264_ulaw/VGA` | Port 88, with audio, VGA resolution |
| `rtsp://IP:88//live/video_audio/profile1` | Port 88 with profile selection |

## Connecting with VisioForge SDK

Use your AVTech camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// AVTech AVM552, H.264 main stream
var uri = new Uri("rtsp://192.168.1.80:554/live/h264");
var username = "admin";
var password = "YourPassword";
```

For MPEG-4 stream access, use `/live/mpeg4` instead of `/live/h264`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot (guest) | `http://IP/cgi-bin/guest/Video.cgi?media=JPEG` | No authentication required (if guest access enabled) |
| JPEG Snapshot (channel) | `http://IP/cgi-bin/guest/Video.cgi?media=JPEG&channel=CHANNEL` | Channel-specific snapshot |
| MJPEG Live Stream | `http://IP/live/mjpeg` | Continuous MJPEG stream |

## Troubleshooting

### "401 Unauthorized" error

If you receive an authentication error:

1. Verify your credentials - default is admin / admin
2. Access the camera at `http://CAMERA_IP` in a browser to confirm login works
3. Ensure RTSP is enabled in the camera's network settings
4. Try including credentials in the URL: `rtsp://admin:password@IP:554/live/h264`

### Port 554 vs port 88

Some newer AVTech models use port 88 instead of the standard RTSP port 554. If you cannot connect on port 554:

1. Try port 88: `rtsp://IP:88//live/h264_ulaw/VGA`
2. Note the double slash (`//`) in some port 88 URL patterns
3. Check the camera's web interface under network settings for the configured RTSP port

### MPEG-4 vs H.264

Older AVTech models may only support MPEG-4. If the H.264 stream URL does not work:

- Try `rtsp://IP:554/live/mpeg4` instead
- Check the camera's encoding settings in the web interface
- Newer models support H.264; older models may be MPEG-4 only

### Double slash in URL

Some AVTech URL patterns include a double slash (`//`) after the IP or port. This is intentional and required by certain firmware versions. If a single-slash URL does not work, try the double-slash variant.

### EagleEyes mobile app

The EagleEyes app is AVTech's mobile viewing platform. RTSP access works independently of EagleEyes and does not require the app to be configured.

## FAQ

**What is the default RTSP URL for AVTech cameras?**

The URL is `rtsp://admin:password@CAMERA_IP:554/live/h264` for the main H.264 stream. For DVRs/NVRs, append the channel number: `rtsp://IP:554/live/h264/ch1` for channel 1.

**Do AVTech cameras support ONVIF?**

Newer AVTech models support ONVIF. Older models may not have ONVIF support and rely on proprietary protocols and RTSP for integration.

**What is the difference between AVM and AVN series?**

The AVM series are IP cameras designed for direct network connection, while the AVN series are network cameras that may include additional features such as built-in Wi-Fi or audio. Both series use the same RTSP URL format.

**Can I access AVTech snapshots without authentication?**

Many AVTech cameras have a guest CGI endpoint (`/cgi-bin/guest/Video.cgi?media=JPEG`) that allows unauthenticated JPEG snapshot access. This is a security concern if your camera is network-accessible. Check your camera's guest access settings and disable guest access if not needed.

**Why do some AVTech URLs use port 88?**

Some newer AVTech firmware versions default to port 88 for RTSP instead of the standard port 554. If you cannot connect on port 554, try port 88. The port setting can typically be verified and changed in the camera's web interface under network configuration.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [LILIN Connection Guide](lilin.md) — Taiwanese industrial cameras
- [BrickCom Connection Guide](brickcom.md) — Taiwanese industrial cameras
- [ONVIF IP Camera Integration](../videocapture/video-sources/ip-cameras/onvif.md) — AVTech ONVIF device discovery
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
