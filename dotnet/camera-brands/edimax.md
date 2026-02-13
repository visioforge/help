---
title: How to Connect to Edimax IP Camera in C# .NET
description: Connect to Edimax cameras in C# .NET with RTSP URL patterns and code samples for IC, IR, PT, and VS series models.
---

# How to Connect to Edimax IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Edimax** (Edimax Technology Co.) is a Taiwanese networking equipment manufacturer headquartered in Taipei, Taiwan. Founded in 1986, Edimax is known primarily for networking products such as routers, switches, and Wi-Fi adapters, but also manufactures a range of consumer and small-to-medium business IP cameras under the IC series. Over the years, Edimax cameras have evolved through several URL format generations.

**Key facts:**

- **Product lines:** IC (IP camera), IR (infrared), PT (pan/tilt), VS (video server)
- **Multiple URL format generations:** older models use `/ipcam.sdp`, newer models use `/stream1`
- **Default RTSP port:** 554 (some models use 8000)
- **Default credentials:** admin / 1234
- **ONVIF support:** Yes (newer models)
- **Video codecs:** H.264, MJPEG
- **Primary RTSP URL:** `rtsp://IP:554/ipcam_h264.sdp`

!!! note "URL format generations"
    Edimax cameras have evolved through several URL format generations. Older IC-1500/IC-3000 series use `/ipcam.sdp` or `/ipcam_h264.sdp`, while newer IR/PT series use `/stream1`. Try both formats if one doesn't work.

!!! warning "Dual authentication styles"
    Edimax cameras use two different authentication parameter styles in HTTP URLs: `account=USER&password=PASS` (older firmware) and `user=USER&pwd=PASS` (newer firmware). Check which format your camera supports.

## RTSP URL Patterns

### Standard URL Format

Edimax cameras primarily use an SDP-based RTSP URL:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/ipcam_h264.sdp
```

| URL Pattern | Description |
|-------------|-------------|
| `/ipcam.sdp` | SDP-based stream (older IC series) |
| `/ipcam_h264.sdp` | H.264 SDP stream (recommended for most models) |
| `/stream1` | Primary stream (newer IR/PT series) |
| `/stream2` | Secondary stream (newer IR/PT series) |
| `/live1.sdp` | Live SDP stream (PT series) |

### Camera Models

| Model | Type | Main Stream URL | Notes |
|-------|------|----------------|-------|
| IC-1500WG (VGA wireless) | VGA | `rtsp://IP:554/ipcam.sdp` | Older SDP format |
| IC-3010 (HD) | HD | `rtsp://IP:554/ipcam.sdp` | Older SDP format |
| IC-3015WN (HD wireless) | HD Wireless | `rtsp://IP:554/ipcam.sdp` | Wi-Fi, SDP format |
| IC-3030WN (HD wireless) | HD Wireless | `rtsp://IP:554/ipcam.sdp` | Wi-Fi, SDP format |
| IC-3030IWN (HD wireless) | HD Wireless | `rtsp://IP:554/ipcam.sdp` | Indoor Wi-Fi |
| IC-3100W (HD wireless) | HD Wireless | `rtsp://IP:554/ipcam_h264.sdp` | H.264 SDP format |
| IC-3110W (HD wireless) | HD Wireless | `rtsp://IP:554/ipcam_h264.sdp` | H.264 SDP format |
| IC-3116W (HD wireless) | HD Wireless | `rtsp://IP:554/ipcam_h264.sdp` | H.264 SDP format |
| IC-7000 (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam.sdp` | Pan/tilt/zoom |
| IC-7010PTN (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam.sdp` | Network PTZ |
| IC-7100 (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | H.264 PTZ |
| IC-7110P (HD PTZ PoE) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | PoE PTZ |
| IC-7110W (HD PTZ wireless) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | Wi-Fi PTZ |
| IC-9000 (outdoor) | Outdoor | `rtsp://IP:554/CHANNEL/USERNAME:PASSWORD/main` | Credential-in-URL |
| IR-112E (infrared) | Infrared | `rtsp://IP:554//stream2` | Newer stream format |
| IR-113E (infrared) | Infrared | `rtsp://IP:554//stream1` | Newer stream format |
| PT-112E (pan/tilt) | Pan/Tilt | `rtsp://IP:554/live1.sdp` | Live SDP format |
| PT-31E (pan/tilt) | Pan/Tilt | `rtsp://IP:8000//stream1` | Port 8000 |
| VS100 (video server) | Video Server | `rtsp://IP:554//stream1` | Encoder/server |

### Alternative URL Formats

Some Edimax models and firmware versions support these additional RTSP URLs:

| URL Pattern | Supported Models | Notes |
|-------------|-----------------|-------|
| `rtsp://IP:554/ipcam.sdp` | IC-1500, IC-3010, IC-3015WN, IC-3030WN, IC-7000, IC-7010PTN | Older SDP format |
| `rtsp://IP:554/ipcam_h264.sdp` | IC-3100W, IC-3110W, IC-3116W, IC-7100, IC-7110P, IC-7110W | H.264 SDP (recommended) |
| `rtsp://IP:554//stream1` | IR-113E, VS100 | Newer stream format (note double slash) |
| `rtsp://IP:554//stream2` | IR-112E | Sub stream (double slash) |
| `rtsp://IP:554/stream1` | Select newer models | Stream without double slash |
| `rtsp://IP:554/live1.sdp` | PT-112E | Live SDP for pan/tilt |
| `rtsp://IP:8000//stream1` | PT-31E | Non-standard port 8000 |

## Connecting with VisioForge SDK

Use your Edimax camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Edimax IC-3116W, H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/ipcam_h264.sdp");
var username = "admin";
var password = "1234";
```

For newer IR/PT series models, use `/stream1` or `//stream1` instead of `/ipcam_h264.sdp`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/snapshot.jpg` | Basic snapshot, may require auth |
| Snapshot (account auth) | `http://IP/snapshot.jpg?account=USER&password=PASS` | Older firmware auth style |
| Snapshot (user auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Newer firmware auth style |
| JPEG Image | `http://IP/jpg/image.jpg` | Direct JPEG |
| Channel JPEG | `http://IP/jpg/1/image.jpg` | Channel-specific JPEG |
| MJPEG Stream | `http://IP/mjpg/video.mjpg` | Continuous MJPEG stream |
| Channel MJPEG | `http://IP/mjpg/1/video.mjpg` | Channel-specific MJPEG |
| CGI Snapshot | `http://IP/snapshot.cgi` | CGI-based snapshot |
| CGI MJPEG | `http://IP/cgi/mjpg/mjpeg.cgi` | CGI MJPEG stream |
| Stream CGI | `http://IP/cgi-bin/Stream?Video` | Alternative video stream |

## Troubleshooting

### "401 Unauthorized" error

Edimax cameras ship with default credentials of **admin / 1234**. If authentication fails:

1. Access the camera at `http://CAMERA_IP` in a browser
2. Log in with the default credentials or the ones you configured
3. Navigate to **Configuration > Security** to verify or reset credentials
4. Use those credentials in your RTSP URL

### URL format not working

Edimax has used several different RTSP URL formats across model generations. If your URL does not connect:

1. Try `/ipcam_h264.sdp` first (works with most mid-generation models)
2. Try `/ipcam.sdp` for older IC-1500 and IC-3000 series
3. Try `//stream1` (with double slash) for newer IR and PT series
4. Try `/stream1` (single slash) if double slash fails
5. Check if your model uses port 8000 instead of 554 (e.g., PT-31E)

### Double-slash URLs

Some newer Edimax models use a double-slash in the RTSP path (e.g., `rtsp://IP:554//stream1`). This is intentional and not a typo. If `/stream1` does not work, try `//stream1`.

### Port 554 vs port 8000

Most Edimax cameras use the standard RTSP port 554, but some models (such as the PT-31E) use port 8000. Check your camera's web interface under **Configuration > Network > RTSP** for the correct port setting.

### Snapshot authentication style

If snapshot URLs return 401 errors even with correct credentials, try switching between the two authentication parameter styles:

- Older firmware: `?account=USER&password=PASS`
- Newer firmware: `?user=USER&pwd=PASS`

## FAQ

**What is the default RTSP URL for Edimax cameras?**

For most Edimax IC series cameras, the URL is `rtsp://admin:1234@CAMERA_IP:554/ipcam_h264.sdp`. For newer IR and PT series, use `rtsp://admin:1234@CAMERA_IP:554//stream1`. The exact format depends on the model and firmware version.

**Do Edimax cameras support ONVIF?**

Newer Edimax models support ONVIF. Older IC-1500 and IC-3000 series models may not have ONVIF support. Check your camera's specifications or web interface for ONVIF settings.

**Why does my Edimax camera use a double slash in the URL?**

Some newer Edimax models (IR and PT series) use a double-slash path format like `//stream1`. This is the correct format for those models and is not a typo. Both `//stream1` and `/stream1` may work depending on the firmware version.

**What is the difference between ipcam.sdp and ipcam_h264.sdp?**

`/ipcam.sdp` is the generic SDP stream used by older models and may deliver either MJPEG or H.264 depending on the camera configuration. `/ipcam_h264.sdp` explicitly requests the H.264 encoded stream and is recommended for better compression and quality.

**Can I use both snapshot authentication styles?**

No. Each camera firmware version supports only one authentication style for HTTP URLs. Older firmware uses `account=USER&password=PASS` while newer firmware uses `user=USER&pwd=PASS`. Try both to determine which your camera expects.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Zavio Connection Guide](zavio.md) — Taiwanese SMB cameras
- [RTSP Video Streaming Guide](../general/network-streaming/rtsp.md) — Edimax RTSP network streaming
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
