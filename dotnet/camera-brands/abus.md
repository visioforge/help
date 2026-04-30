---
title: ABUS IP Camera RTSP URLs and C# .NET Connection Guide
description: ABUS TVIP, CASA, and Digi-Lan camera RTSP URL patterns for C# .NET. Stream and record with VisioForge Video Capture SDK cross-platform integration.
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
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - H.264
  - MJPEG
  - C#

---

# How to Connect to ABUS IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**ABUS** (August Bremicker Soehne KG) is a German security company headquartered in Wetter, Germany. Founded in 1924, ABUS is one of Europe's largest security product manufacturers, known for locks, alarm systems, and video surveillance. The **TVIP** series of IP cameras is widely deployed across Europe, particularly in Germany, Austria, and the Benelux countries.

**Key facts:**

- **Product lines:** TVIP (IP cameras), CASA (consumer), TV (legacy analog IP), Digi-Lan (older IP)
- **TVIP model numbering:** TVIP1xxxx (consumer), TVIP2xxxx (2MP), TVIP4xxxx (4MP), TVIP5xxxx (5MP), TVIP6xxxx/7xxxx (special)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI, MJPEG
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (some models: root / pass)
- **ONVIF support:** Yes (TVIP2xxxx and newer)
- **Video codecs:** H.264 (TVIP2xxxx and newer), MJPEG (older models)

!!! info "ABUS TVIP Model Numbering"
    The TVIP model number indicates the resolution tier: **1xxxx** = basic/consumer, **2xxxx** = 2MP (1080p), **4xxxx** = 4MP, **5xxxx** = 5MP, **6xxxx/7xxxx** = special models. This helps identify which URL formats and codecs a camera supports.

## RTSP URL Patterns

### Primary URL Formats

ABUS cameras support multiple RTSP URL formats depending on the model generation:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video.mp4
```

| URL Pattern | Description |
|-------------|-------------|
| `rtsp://IP:554/video.mp4` | MP4 stream (recommended for H.264 models) |
| `rtsp://IP:554/live.sdp` | Live SDP stream (consumer and legacy models) |
| `rtsp://IP:554/video.h264` | Direct H.264 stream |
| `rtsp://IP:554/VideoInput/CHANNEL/h264/1` | VideoInput format (4MP models) |

### TVIP1xxxx Series (Consumer)

| Model | Main Stream URL | Notes |
|-------|----------------|-------|
| TVIP10000 | `rtsp://IP:554/live.sdp` | MJPEG-only |
| TVIP10500 | `rtsp://IP:554/live.sdp` | MJPEG-only |
| TVIP10550 | `rtsp://IP:554/live.sdp` | MJPEG-only |
| TVIP11000 | `rtsp://IP:554/live.sdp` | MJPEG/H.264 |

!!! warning "MJPEG-Only Models"
    Some older TVIP1xxxx models (TVIP10000, TVIP10500, TVIP10550) support only MJPEG encoding with no H.264. Use the MJPEG HTTP stream URLs listed in the Snapshot and MJPEG section below for these models.

### TVIP2xxxx Series (2MP)

| Model | Main Stream URL | Alternative URL |
|-------|----------------|-----------------|
| TVIP20000 | `rtsp://IP:554/video.mp4` | - |
| TVIP20550 | `rtsp://IP:554/video.mp4` | - |
| TVIP21550 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/live.sdp` |
| TVIP22500 | `rtsp://IP:554/video.h264` | - |

### TVIP4xxxx Series (4MP)

| Model | Main Stream URL | Alternative URL |
|-------|----------------|-----------------|
| TVIP41500 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/VideoInput/1/h264/1` |
| TVIP41550 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/VideoInput/1/h264/1` |

### TVIP5xxxx Series (5MP)

| Model | Main Stream URL |
|-------|----------------|
| TVIP51550 | `rtsp://IP:554/video.mp4` |

### TVIP6xxxx / TVIP7xxxx Series (Special)

| Model | Main Stream URL |
|-------|----------------|
| TVIP61500 | `rtsp://IP:554/video.mp4` |
| TVIP71550 | `rtsp://IP:554/video.mp4` |

### CASA Series (Consumer)

| Model | Main Stream URL |
|-------|----------------|
| CASA20550 | `rtsp://IP:554/live.sdp` |

### Legacy TV / Digi-Lan Series

| Model | Main Stream URL |
|-------|----------------|
| Digi-Lan TV7220 | `rtsp://IP:554/live.sdp` |
| TV7240-LAN | `rtsp://IP:554/live.sdp` |
| TV32500 | `rtsp://IP:554/video.mp4` |

!!! tip "Which URL Format to Try First"
    For ABUS cameras, try `video.mp4` first for H.264 streaming, then `live.sdp` as a fallback. For older TVIP1xxxx models, `live.sdp` is typically the only RTSP option. The `VideoInput` format is specific to TVIP4xxxx models.

## Connecting with VisioForge SDK

Use your ABUS camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// ABUS TVIP41550, main stream
var uri = new Uri("rtsp://192.168.1.90:554/video.mp4");
var username = "admin";
var password = "admin";
```

For models using the `VideoInput` format, use:

```csharp
// ABUS TVIP41500, VideoInput format
var uri = new Uri("rtsp://192.168.1.90:554/VideoInput/1/h264/1");
```

## Snapshot and MJPEG URLs

### JPEG Snapshots

| Type | URL Pattern | Supported Models |
|------|-------------|------------------|
| Standard snapshot | `http://IP/jpg/image.jpg` | TVIP10500, TVIP10550, TVIP11000, TVIP20000, TVIP21550, TVIP51550 |
| High-res snapshot | `http://IP/jpg/image.jpg?size=3` | TVIP10001, TVIP21050, TVIP71550 |
| CGI viewer | `http://IP/cgi-bin/viewer/video.jpg?channel=CH&resolution=WxH` | CASA20550, TVIP41550, TVIP51550 |
| Simple CGI snapshot | `http://IP/cgi-bin/video.jpg` | Digi-Lan, TV models |
| Alternative CGI | `http://IP/cgi-bin/jpg/image` | TVIP20050 |
| Profile image | `http://IP/cgi-bin/view/image?pro_CHANNEL` | TVIP20000, TVIP21500 |
| JPEG pull | `http://IP/jpeg/pull` | TVIP62000 |

### MJPEG Streams

| Type | URL Pattern | Supported Models |
|------|-------------|------------------|
| Standard MJPEG | `http://IP/video.mjpg` | TVIP10000, TVIP11000, TVIP21500, TVIP21550, TVIP51550, TVIP71501 |
| MJPEG with params | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | TVIP31550, TVIP21501, TVIP51550, TVIP71550 |

!!! note "MJPEG Parameters"
    The `q` parameter controls JPEG quality (1-100), `fps` sets the frame rate, and `id` is a session identifier. Adjust these values based on your bandwidth and quality requirements.

## Troubleshooting

### Multiple URL formats work on the same camera

Many ABUS cameras respond to several different RTSP and HTTP URL formats. This is by design. For the best results:

1. Try `rtsp://IP:554/video.mp4` first for H.264 streaming
2. Fall back to `rtsp://IP:554/live.sdp` if `video.mp4` does not work
3. Use `http://IP/video.mjpg` for MJPEG streaming as a last resort

### Older TVIP1xxxx models have no H.264

Some first-generation TVIP1xxxx cameras (TVIP10000, TVIP10500, TVIP10550) only support MJPEG encoding. These cameras will not respond to `video.mp4` or `video.h264` RTSP URLs. Use the MJPEG HTTP stream (`http://IP/video.mjpg`) or the `live.sdp` RTSP URL instead.

### Default credentials vary by model

Most ABUS cameras use `admin` / `admin` as default credentials. However, some models default to `root` / `pass`. If authentication fails with one set, try the other. Check the camera's documentation for the specific default credentials.

### TVIP model number decoding

If you are unsure which URL format to use, the TVIP model number provides guidance:

- **TVIP1xxxx:** Start with `live.sdp`, may be MJPEG-only
- **TVIP2xxxx:** Start with `video.mp4`, most support H.264
- **TVIP4xxxx:** Start with `video.mp4`, also try `VideoInput/1/h264/1`
- **TVIP5xxxx+:** Start with `video.mp4`

### Snapshot resolution parameter

For the `jpg/image.jpg?size=N` URL, the `size` parameter controls resolution:

- `size=1` = Lowest resolution
- `size=2` = Medium resolution
- `size=3` = Highest resolution

## FAQ

**What is the default RTSP URL for ABUS cameras?**

For most current ABUS cameras (TVIP2xxxx and newer), the default URL is `rtsp://admin:admin@CAMERA_IP:554/video.mp4`. For older consumer models (TVIP1xxxx), try `rtsp://admin:admin@CAMERA_IP:554/live.sdp` instead.

**Do ABUS cameras support ONVIF?**

Yes. ABUS cameras from the TVIP2xxxx generation onward support ONVIF, which provides standardized camera discovery and streaming. Older TVIP1xxxx models may not support ONVIF.

**Can I use MJPEG streaming with ABUS cameras?**

Yes. Most ABUS cameras support MJPEG streaming via `http://CAMERA_IP/video.mjpg`. This is particularly useful for older TVIP1xxxx models that do not support H.264 encoding. MJPEG uses more bandwidth than H.264 but is compatible with a wider range of software.

**What do the ABUS TVIP model numbers mean?**

The five-digit number after "TVIP" indicates the camera's resolution tier: 1xxxx = basic/consumer, 2xxxx = 2MP (1080p), 4xxxx = 4MP, 5xxxx = 5MP, and 6xxxx/7xxxx = special models. Higher numbers generally indicate newer hardware with broader protocol and codec support.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [INSTAR Connection Guide](instar.md) — German consumer / smart home cameras
- [ONVIF IP Camera Integration](../videocapture/video-sources/ip-cameras/onvif.md) — ABUS ONVIF device setup
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
