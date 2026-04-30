---
title: How to Connect to Arecont Vision IP Camera in C# .NET
description: Arecont Vision RTSP URL patterns for C# .NET. Integrate AV Series, MegaDome, and SurroundVideo panoramic cameras using VisioForge Video Capture SDK.
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
  - H.264
  - MJPEG
  - C#

---

# How to Connect to Arecont Vision IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Arecont Vision** (now part of Costar Group) is an American IP camera company originally founded in 2003 and based in Glendale, California. Arecont Vision was a pioneer of megapixel IP cameras and is known for high-resolution models (up to 20MP) and multi-sensor panoramic cameras. The company was acquired by **Costar Group** in 2019, which continues to support and manufacture Arecont Vision products.

**Key facts:**

- **Product lines:** AV Series (megapixel fixed), MegaDome, MegaBall, SurroundVideo (multi-sensor panoramic), MicroDome
- **Protocol support:** RTSP, ONVIF (newer models), PSIA, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** Varies by model (many ship with no default password)
- **ONVIF support:** Yes (newer models), PSIA support on most models
- **Video codecs:** H.264, MJPEG (older models MJPEG only)

!!! info "Arecont Vision and Costar Group"
    Arecont Vision was acquired by Costar Group in 2019. Existing Arecont cameras continue to use the same RTSP and PSIA URL formats. Newer Costar-branded models may use updated firmware but maintain backward-compatible URL patterns.

## RTSP URL Patterns

### Standard URL Formats

Arecont Vision cameras support multiple RTSP URL patterns depending on the model generation and configured protocol:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264.sdp
```

| URL Pattern | Protocol | Description |
|-------------|----------|-------------|
| `rtsp://IP:554/h264.sdp` | H.264 | Standard H.264 stream (most current models) |
| `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | PSIA | PSIA-based H.264 stream |
| `rtsp://IP:554/cam1/mpeg4` | MPEG-4 | Legacy MPEG-4 stream (older models) |

### H.264 Stream with ROI Parameters

Arecont cameras support optional Region of Interest (ROI) parameters for customizing the stream output:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264.sdp?res=half&x0=0&y0=0&x1=1600&y1=1200&quality=15&doublescan=0
```

| Parameter | Values | Description |
|-----------|--------|-------------|
| `res` | `full`, `half` | Stream resolution (full or half of sensor resolution) |
| `x0`, `y0` | 0 - max | Top-left corner of the region of interest |
| `x1`, `y1` | 0 - max | Bottom-right corner of the region of interest |
| `quality` | 1 - 21 | JPEG/H.264 quality factor (lower = higher quality) |
| `doublescan` | 0, 1 | Enable double-scan mode for improved image quality |

!!! tip "ROI Parameters Are Optional"
    The ROI parameters (`res`, `x0`, `y0`, `x1`, `y1`, `quality`, `doublescan`) are optional and can be omitted entirely for full-frame streaming. Use `rtsp://IP:554/h264.sdp` without parameters for the simplest connection.

### Camera Models

| Model Series | Resolution | Main Stream URL | Codec |
|-------------|-----------|----------------|-------|
| AV Series (generic megapixel) | Varies | `rtsp://IP:554/h264.sdp` | H.264 |
| AV2100 (2MP) | 1600x1200 | `rtsp://IP:554/cam1/mpeg4` | MPEG-4 |
| AV5115/AV5125 | 2592x1944 | `rtsp://IP:554/h264.sdp` | H.264 |
| AV8185DN (8MP multi-sensor) | 6400x1200 | `rtsp://IP:554/h264.sdp` | H.264 |
| AV10005/AV10115 (10MP) | 3648x2752 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| AV20185 (20MP multi-sensor) | 10240x1536 | `rtsp://IP:554/h264.sdp` | H.264 |
| MegaDome Series | Varies | `rtsp://IP:554/h264.sdp` | H.264 |
| MegaBall Series | Varies | `rtsp://IP:554/h264.sdp` | H.264 |
| MicroDome Series | Varies | `rtsp://IP:554/h264.sdp` | H.264 |

### PSIA Streaming URLs

Models that support the PSIA protocol can use the following URL format:

| Channel | URL |
|---------|-----|
| Channel 0 (default) | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` |
| Channel 1 | `rtsp://IP:554/PSIA/Streaming/channels/1?videoCodecType=H.264` |

## Connecting with VisioForge SDK

Use your Arecont Vision camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Arecont Vision AV Series, H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/h264.sdp");
var username = "admin";
var password = "YourPassword";
```

For PSIA-based streaming, use the PSIA URL instead:

```csharp
// Arecont Vision via PSIA protocol
var uri = new Uri("rtsp://192.168.1.90:554/PSIA/Streaming/channels/0?videoCodecType=H.264");
```

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/img.jpg` | Most models, requires basic auth |
| JPEG Snapshot (alt) | `http://IP/Jpeg/CamImg.jpg` | Alternative snapshot URL |
| Configurable Snapshot | `http://IP/image?res=half&x0=0&y0=0&x1=1600&y1=1200&quality=15&doublescan=0` | Snapshot with ROI parameters |
| MJPEG Stream | `http://IP/mjpeg?res=full&x0=0&y0=0&x1=100%&y1=100%&quality=12&doublescan=0` | Continuous MJPEG stream |

### Multi-Sensor (SurroundVideo) Snapshot URLs

For SurroundVideo and other multi-sensor cameras, each sensor has its own snapshot URL:

| Sensor | URL Pattern | Notes |
|--------|-------------|-------|
| Channel 1 | `http://IP/image1?res=half&x1=0&y1=0` | First sensor |
| Channel 2 | `http://IP/image2?res=half&x1=0&y1=0` | Second sensor |
| Channel 3 | `http://IP/image3?res=half&x1=0&y1=0` | Third sensor |
| Channel 4 | `http://IP/image4?res=half&x1=0&y1=0` | Fourth sensor |

## Troubleshooting

### ROI parameters causing issues

Arecont cameras have unique Region of Interest parameters (`res`, `x0`, `y0`, `x1`, `y1`, `quality`, `doublescan`) embedded in their URLs. If you encounter connection problems:

1. Remove all ROI parameters and use the bare URL: `rtsp://IP:554/h264.sdp`
2. Verify the camera resolution supports the requested ROI coordinates
3. Use `res=full` for full-resolution streaming or `res=half` for reduced bandwidth

### MJPEG-only on older models

Some older Arecont models (AV1300, AV2100, AV3100) only support MJPEG encoding and do not have H.264 capability. For these cameras:

- Use the MPEG-4 RTSP URL: `rtsp://IP:554/cam1/mpeg4`
- Or use the HTTP MJPEG stream: `http://IP/mjpeg`

### PSIA vs direct RTSP

Arecont cameras support both direct RTSP and PSIA-based RTSP URLs. If one format does not work:

- Try the alternative format (switch between `h264.sdp` and `PSIA/Streaming/channels/0`)
- Verify the camera firmware version supports the chosen protocol
- Check that PSIA is enabled in the camera's web interface

### Connection timeout

Arecont cameras may take longer to establish an RTSP session than other brands, especially for high-resolution (10MP+) models:

- Increase your connection timeout to at least 10 seconds
- For multi-sensor models, connect to individual channels rather than the composite stream for lower latency

## FAQ

**What RTSP URL format does Arecont Vision use?**

The primary RTSP URL is `rtsp://IP:554/h264.sdp` for H.264 streaming. PSIA-based streaming uses `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264`. Older models may use `rtsp://IP:554/cam1/mpeg4` for MPEG-4 streams.

**Are Arecont Vision cameras still supported after the Costar acquisition?**

Yes. Costar Group acquired Arecont Vision in 2019 and continues to manufacture and support Arecont Vision camera products. Existing cameras remain fully functional, and firmware updates are available through Costar's support channels.

**How do I connect to a multi-sensor SurroundVideo camera?**

SurroundVideo cameras expose individual sensor channels through numbered URLs. For snapshots, use `http://IP/image1`, `http://IP/image2`, etc. For RTSP, use the standard H.264 URL with the full panoramic composite, or PSIA channel-based URLs for individual sensors.

**Do Arecont Vision cameras support ONVIF?**

Newer Arecont Vision models support ONVIF Profile S. Older models rely on the PSIA protocol instead. Check your camera's specifications or web interface to confirm ONVIF availability.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [GeoVision Connection Guide](geovision.md) — Professional surveillance cameras
- [ONVIF Capture with Postprocessing](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Arecont ONVIF capture pipeline
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
