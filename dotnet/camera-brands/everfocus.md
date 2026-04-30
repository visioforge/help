---
title: EverFocus IP Camera RTSP URL Connection Guide for C# .NET
description: EverFocus EAN, EHN, EMN, EPN, and EQN series RTSP URL patterns for C# .NET. Stream and record using VisioForge Video Capture SDK integration.
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
  - MJPEG
  - C#

---

# How to Connect to EverFocus IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**EverFocus Electronics** is a Taiwanese professional surveillance company headquartered in New Taipei City, Taiwan, with US operations based in Duarte, California. Founded in 1995, EverFocus manufactures IP cameras, DVRs, and mobile surveillance solutions designed for professional security integrators. The company is well-known in the commercial and industrial surveillance market.

**Key facts:**

- **Product lines:** EAN (bullet), EHN (dome), EMN (mini dome), EPN (PTZ), EZN (compact), EQN (turret), ECOR/EPARA (DVRs)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (all current IP cameras)
- **Video codecs:** H.264 (all current models)

!!! info "EverFocus RTSP URL Format"
    EverFocus cameras use a unique `rtspStreamOvf` path in their RTSP URLs. This format is specific to EverFocus and should not be confused with other manufacturers' URL patterns. Note the required double-slash (`//`) before `cgi-bin`.

## RTSP URL Patterns

### Standard URL Format

EverFocus cameras use the `rtspStreamOvf` CGI path for RTSP streaming:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//cgi-bin/rtspStreamOvf/0
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| Stream index | `/0` | Main stream (highest resolution) |
| Stream index | `/1` | Sub stream (lower resolution, less bandwidth) |

!!! warning "Double-Slash Required"
    The URL path must include a double-slash before `cgi-bin` (i.e., `//cgi-bin/rtspStreamOvf/...`). Omitting the leading slash will cause the connection to fail.

### Primary URL Format (rtspStreamOvf)

Most EverFocus IP cameras use the `rtspStreamOvf` format:

| Model | Series | Main Stream URL | Sub Stream URL |
|-------|--------|----------------|----------------|
| EAN3220 | EAN (bullet) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EHN3260 | EHN (dome) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN2220 | EMN (mini dome) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN1360 | EMN (mini dome) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN3260 | EMN (mini dome) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EPN4220 | EPN (PTZ) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EZN3160 | EZN (compact) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |

### Alternative URL Format (streaming/channels)

Some newer EverFocus models also support the `streaming/channels` format:

| Model | Series | Main Stream URL |
|-------|--------|----------------|
| EPN4220 | EPN (PTZ) | `rtsp://IP:554/streaming/channels/0` |
| EZN3240 | EZN (compact) | `rtsp://IP:554/streaming/channels/0` |
| EHN3260 | EHN (dome) | `rtsp://IP:554/streaming/channels/0` |

!!! tip "Which Format to Use"
    Try the `rtspStreamOvf` format first, as it is supported across all EverFocus IP camera product lines. The `streaming/channels` format is an alternative available on select newer models.

## Connecting with VisioForge SDK

Use your EverFocus camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// EverFocus EHN3260, main stream
var uri = new Uri("rtsp://192.168.1.90:554//cgi-bin/rtspStreamOvf/0");
var username = "admin";
var password = "admin";
```

For sub-stream access, use `/1` instead of `/0` at the end of the URL.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Supported Models |
|------|-------------|------------------|
| Snapshot (with auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS&strm=CHANNEL` | EQN2101 |
| Snapshot (simple) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | General IP cameras |
| Mobile snapshot | `http://IP/m/camera[CHANNEL].jpg` | ECOR/EPARA DVRs |

!!! note "DVR Snapshots"
    For ECOR and EPARA DVR models, replace `[CHANNEL]` with the camera channel number (e.g., `camera1.jpg` for channel 1).

## Troubleshooting

### Connection refused or timeout

EverFocus cameras use the `rtspStreamOvf` CGI path which is unique to this brand. Make sure you are not accidentally using a URL format from another manufacturer:

1. Verify the URL includes the double-slash: `//cgi-bin/rtspStreamOvf/0`
2. Confirm the RTSP port is 554 (or check the camera's network settings for a custom port)
3. Ensure the camera is accessible on the network by pinging its IP address

### Stream index starts at 0

Unlike some other camera brands where channels start at 1, EverFocus stream indices start at **0**:

- `/0` = Main stream (full resolution)
- `/1` = Sub stream (reduced resolution)

Using `/1` expecting the main stream will return the sub stream instead.

### Alternative URL format not working

The `streaming/channels/0` URL format is only available on certain newer models (EPN4220, EZN3240, EHN3260). If this format does not work, fall back to the standard `//cgi-bin/rtspStreamOvf/0` format.

### Authentication issues

EverFocus cameras default to `admin` / `admin`. If you have changed the password via the web interface and forgotten it, a hardware reset button on the camera will restore factory defaults.

## FAQ

**What is the default RTSP URL for EverFocus cameras?**

The default URL is `rtsp://admin:admin@CAMERA_IP:554//cgi-bin/rtspStreamOvf/0` for the main stream. Use `/1` at the end for the sub stream. Note the double-slash before `cgi-bin` which is required.

**Why does the EverFocus RTSP URL look different from other cameras?**

EverFocus uses a proprietary `rtspStreamOvf` CGI path that is unique to their camera firmware. This is different from the more common formats used by Hikvision, Dahua, or ONVIF generic paths. The double-slash (`//cgi-bin/...`) is intentional and required.

**Do EverFocus cameras support ONVIF?**

Yes. All current EverFocus IP cameras support ONVIF, which provides a standardized way to discover and connect to the camera. You can use ONVIF as an alternative to the proprietary RTSP URL format.

**Can I connect to EverFocus DVRs (ECOR/EPARA) via RTSP?**

EverFocus DVRs primarily expose HTTP-based snapshot URLs for individual channels (`http://IP/m/camera[CHANNEL].jpg`). For RTSP streaming from DVR channels, consult your specific DVR model's documentation or use ONVIF discovery.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Speco Connection Guide](speco.md) — Professional surveillance cameras
- [RTSP Video Streaming Guide](../general/network-streaming/rtsp.md) — EverFocus RTSP streaming setup
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
