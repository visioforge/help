---
title: Dahua IP Camera RTSP URL Patterns and C# .NET Guide
description: Integrate Dahua IPC-HDW, IPC-HFW, NVR, and DVR cameras into C# .NET apps. RTSP URL format, ONVIF auto-discovery, and VisioForge SDK code included.
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

# How to Connect to Dahua IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Dahua Technology** (Zhejiang Dahua Technology Co., Ltd.) is the world's second-largest video surveillance manufacturer. Founded in 2001 and headquartered in Hangzhou, China, Dahua produces IP cameras, NVRs, DVRs, access control systems, and video intercoms. Dahua cameras are also widely sold under OEM brands including Amcrest, Lorex, and others.

**Key facts:**

- **Product lines:** IPC-HDW (dome), IPC-HFW (bullet), IPC-HDBW (dome vandal-proof), SD (PTZ), NVR4xxx/5xxx (NVRs), XVR (DVRs)
- **Protocol support:** ONVIF Profile S/G/T, RTSP, HTTP, Dahua proprietary (DHIP)
- **Default RTSP port:** 554 (some models use 1554)
- **Default credentials:** admin / admin (older firmware); admin / (set during setup on newer firmware)
- **ONVIF support:** Full
- **Video codecs:** H.264, H.265, H.265+, MJPEG

## RTSP URL Patterns

Dahua cameras use a `cam/realmonitor` URL structure with channel and subtype parameters.

### URL Format

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:[PORT]/cam/realmonitor?channel=[CH]&subtype=[ST]
```

**Parameters:**

- `channel` = camera channel number (1 for single-channel cameras, 1-N for NVR/DVR)
- `subtype` = stream type: 0 = main stream, 1 = sub stream, 2 = third stream

### IP Cameras (Single Channel)

| Model Series | RTSP URL | Stream | Audio |
|-------------|----------|--------|-------|
| IPC-HDW (dome) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Main | Yes |
| IPC-HDW (dome) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sub | Yes |
| IPC-HFW (bullet) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Main | Yes |
| IPC-HDBW (vandal dome) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Main | Yes |
| SD (PTZ) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Main | Yes |
| DH-IPC-HF2100P | `rtsp://IP:1554/cam/realmonitor?channel=1&subtype=0` | Main | Yes |

### Simplified URL Format

Many Dahua cameras also accept a shorter URL format:

| URL Pattern | Stream | Notes |
|-------------|--------|-------|
| `rtsp://IP:554/cam/realmonitor` | Main (ch1) | Defaults to channel 1, main stream |
| `rtsp://IP:554/` | Main | Bare URL, some models only |
| `rtsp://IP:554/live` | Main | Legacy format |

### NVR / DVR Channels

| Device | Channel | RTSP URL | Stream |
|--------|---------|----------|--------|
| NVR Camera 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Main |
| NVR Camera 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sub |
| NVR Camera 2 | 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | Main |
| NVR Camera 4 | 4 | `rtsp://IP:554/cam/realmonitor?channel=4&subtype=0` | Main |
| DVR Channel 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=01` | Sub |

### Amcrest / Lorex (Dahua OEM)

Amcrest and Lorex cameras use the same Dahua RTSP URL format:

| Brand | RTSP URL | Notes |
|-------|----------|-------|
| Amcrest | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Identical to Dahua |
| Lorex | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Identical to Dahua |

## Connecting with VisioForge SDK

Use your Dahua camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Dahua IPC-HDW series, main stream
var uri = new Uri("rtsp://192.168.1.108:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `subtype=1` instead.

### ONVIF Discovery

Dahua cameras provide strong ONVIF support. See the [ONVIF integration guide](../mediablocks/Sources/index.md) for discovery code examples.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requires basic auth |
| JPEG Snapshot (legacy) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | URL-based auth |
| MJPEG Stream | `http://IP/cgi-bin/mjpg/video.cgi?channel=1` | Continuous MJPEG |
| Axis-compatible MJPEG | `http://IP/axis-cgi/mjpg/video.cgi?camera=1` | Emulated Axis API |
| CGI Snapshot | `http://IP/cgi-bin/video.jpg` | Simple snapshot |
| Image CGI | `http://IP/cgi-bin/jpg/image.cgi` | Alternative snapshot |

## Troubleshooting

### Port 554 vs 1554

Some Dahua models (especially the DH-IPC-HF series) use port **1554** instead of the standard 554. If connection fails on port 554, try 1554.

### Authentication methods

- Dahua supports both **basic** and **digest** RTSP authentication
- Newer firmware defaults to digest authentication
- VisioForge SDK handles both methods automatically
- If using HTTP snapshot URLs, some require URL-embedded credentials (`loginuse`/`loginpas` parameters) while newer firmware uses standard HTTP basic/digest auth

### Connection drops

- Dahua cameras can be sensitive to network congestion. Use TCP transport for reliability.
- Reduce main stream resolution or switch to sub stream (`subtype=1`) to lower bandwidth
- Check the camera's **Max User Connections** setting (Configuration > Network > Connection) -- the default is typically 10

### Amcrest/Lorex cameras not connecting

If you have an Amcrest or Lorex camera (Dahua OEM), use the exact same RTSP URL patterns listed above. The default ports and paths are identical to Dahua. The only difference may be in default credentials:

- **Amcrest default:** admin / admin
- **Lorex default:** admin / (set during setup)

### DVR extra stream format

When connecting to DVR channels, note that `subtype=00` and `subtype=0` are equivalent for the main stream. Some older firmware requires the two-digit format (`01` instead of `1`).

## FAQ

**What is the default RTSP URL for Dahua cameras?**

The standard URL is `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` for the main stream. Use `subtype=1` for the sub stream (lower resolution, less bandwidth).

**Do Amcrest cameras use the same RTSP URLs as Dahua?**

Yes. Amcrest cameras are manufactured by Dahua and use identical RTSP URL patterns, authentication, and port configurations. Any RTSP URL that works for a Dahua camera will work for the corresponding Amcrest model.

**How do I access multiple cameras on a Dahua NVR?**

Change the `channel` parameter in the RTSP URL. Channel 1 is the first camera, channel 2 is the second, and so on. For example, `rtsp://IP:554/cam/realmonitor?channel=3&subtype=0` connects to the third camera on the NVR's main stream.

**Why does my Dahua camera use port 1554 instead of 554?**

Some older Dahua models, particularly the DH-IPC-HF series, default to RTSP port 1554. You can change this in the camera's web interface under Configuration > Network > Port. Newer models default to port 554.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Amcrest Connection Guide](amcrest.md) — Dahua OEM, identical URL format
- [Lorex Connection Guide](lorex.md) — Uses Dahua URL format for many models
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
