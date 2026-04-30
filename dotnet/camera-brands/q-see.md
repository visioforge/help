---
title: How to Connect to Q-See IP Camera & DVR in C# .NET
description: Q-See QC, QCN, and QS series camera and DVR RTSP URL patterns for C# .NET. Default credentials, channel setup, and VisioForge SDK integration code.
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

# How to Connect to Q-See IP Camera & DVR in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Q-See** was an American consumer surveillance brand based in Anaheim, California. Q-See DVRs and IP cameras were popular budget surveillance systems sold through major US retailers including Costco and Amazon. The company became essentially defunct by 2020, but a large number of Q-See systems remain deployed in homes and businesses. Q-See products used a mix of **Dahua OEM cameras** and components from other Chinese manufacturers, meaning most Q-See devices follow Dahua RTSP URL conventions.

**Key facts:**

- **Product lines:** QC Series (DVRs), QCN Series (IP cameras), QS Series (DVR kits)
- **Protocol support:** RTSP, HTTP/CGI, ONVIF (some IP camera models)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin or admin / 123456
- **ONVIF support:** Some IP camera models (QCN series)
- **Video codecs:** H.264 (most models), MPEG-4 (older DVRs)
- **OEM base:** Mix of Dahua and other manufacturers

!!! warning "Q-See Is Defunct"
    Q-See ceased operations around 2020. No firmware updates, technical support, or cloud services are available. If you are integrating Q-See hardware, treat it as Dahua-compatible equipment and try Dahua URL patterns first. See our [Dahua connection guide](dahua.md) for additional details.

## RTSP URL Patterns

### Standard URL Format (Dahua-Based)

Most Q-See devices use the Dahua `cam/realmonitor` URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Camera channel (1 for standalone cameras, 1-N for DVR channels) |
| `subtype` | 0 | Main stream (highest resolution) |
| `subtype` | 1 | Sub stream (lower resolution, less bandwidth) |

### DVR Models (QC Series, QS Series)

| Model / Series | Main Stream URL | Notes |
|----------------|----------------|-------|
| QC-804 (4-ch DVR) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua format, change `channel` for each input |
| QS408 / QS411 (DVR kits) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua format |
| Various DVRs | `rtsp://IP:554/` | Root stream (fallback) |
| Various DVRs | `rtsp://IP:554/live.sdp` | Live SDP stream |
| Various DVRs | `rtsp://IP:554/ch0_unicast_firststream` | Unicast first stream |

### IP Camera Models (QCN Series)

| Model | Resolution | URL | Notes |
|-------|-----------|-----|-------|
| QCN7001B | 1080p | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua format (recommended) |
| QCN7001B | 1080p | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | PSIA format |
| QCN7001B | 1080p | `rtsp://IP:554/VideoInput/1/h264/1` | VideoInput H.264 |
| QCN7001B | 1080p | `rtsp://IP:554/VideoInput/1/mpeg4/1` | VideoInput MPEG-4 |
| QCN7005B | 1080p | `rtsp://IP:554/` | Root stream |

### Alternative URL Formats

Some Q-See devices support additional URL patterns:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Standard Dahua format (recommended) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sub stream (lower bandwidth) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=00&authbasic=BASE64` | With base64-encoded credentials |
| `rtsp://IP:554/` | Root stream (fallback for many models) |
| `rtsp://IP:554/live.sdp` | Live SDP format |
| `rtsp://IP:554/ch0_unicast_firststream` | Unicast first stream |

!!! note "Base64 Authentication"
    The `authbasic=` parameter used in some Q-See URLs takes base64-encoded credentials in the format `username:password`. For example, `admin:admin` encodes to `YWRtaW46YWRtaW4=`.

### DVR Channel URLs

For Q-See multi-channel DVRs (QC-804, QS408, QS411, etc.):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Camera 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Camera N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

## Connecting with VisioForge SDK

Use your Q-See camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Q-See QC-804 DVR, channel 1 main stream
var uri = new Uri("rtsp://192.168.1.100:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "admin";
```

For sub-stream access, use `subtype=1` instead of `subtype=0`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| CGI Snapshot | `http://IP/cgi-bin/snapshot.cgi?chn=1&u=USER&p=PASS` | Channel-based snapshot with credentials |
| Login-Based Snapshot | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Login parameter-based snapshot |
| Still Image | `http://IP/stillimg1.jpg` | Replace `1` with channel number |
| Stream Image | `http://IP/images/stream_1.jpg` | Replace `1` with channel number |
| Fast Stream (QS Series) | `http://IP/control/faststream.jpg?stream=MxPEG&needlength&fps=6` | Continuous fast stream |

## Troubleshooting

### No firmware updates or support

Q-See ceased operations around 2020. There are no firmware updates, no technical support, and no replacement parts available. If your Q-See device has a security vulnerability or bug, it cannot be patched. Consider upgrading to a currently supported camera brand.

### Try Dahua URL patterns first

Most Q-See DVRs and many IP cameras use Dahua firmware internally. If the Q-See-specific URLs listed above do not work, try the standard Dahua `cam/realmonitor` format. See our [Dahua connection guide](dahua.md) for the full set of Dahua URL patterns.

### Base64 authentication parameter

Some Q-See devices use an `authbasic=` parameter in the RTSP URL instead of embedding credentials in the URI. Encode `username:password` as base64:

- `admin:admin` = `YWRtaW46YWRtaW4=`
- `admin:123456` = `YWRtaW46MTIzNDU2`

### Port forwarding for remote access

Q-See DVRs typically require manual port forwarding for remote RTSP access. Forward port **554** (RTSP) and optionally port **80** or **8080** (HTTP) on your router to the DVR's local IP address.

### Default credentials

Q-See devices commonly ship with one of these credential pairs:

- `admin` / `admin`
- `admin` / `123456`

If neither works, the password may have been changed by the previous owner or installer.

## FAQ

**What RTSP URL format do Q-See cameras use?**

Most Q-See devices use the Dahua `cam/realmonitor` format: `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0`. This is because Q-See cameras and DVRs were primarily OEM versions of Dahua hardware. Use `channel=1` for standalone cameras or the appropriate channel number for DVR inputs.

**Are Q-See cameras still supported?**

No. Q-See ceased operations around 2020. No firmware updates, cloud services, or technical support are available. The hardware still functions, but there will be no future patches or improvements. Many users have migrated to other brands like Amcrest or Reolink that use similar Dahua-based protocols.

**Can I use Q-See cameras with ONVIF?**

Some Q-See IP cameras (QCN series) support ONVIF, but most Q-See DVRs do not. If ONVIF discovery fails, use the direct RTSP URL patterns listed above instead.

**What is the default password for Q-See cameras?**

The default credentials are typically `admin` / `admin` or `admin` / `123456`. Since Q-See is no longer available, there is no official password reset tool. A factory reset (usually a pinhole button on the device) will restore default credentials on most models.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Dahua Connection Guide](dahua.md) — Same URL format for most Q-See devices
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
