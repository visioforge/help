---
title: Lorex IP Camera RTSP URL Connection Guide for C# .NET
description: Connect to Lorex security cameras and NVRs in C# .NET with RTSP URL patterns and code samples for LNB, LNE, LNZ, and Lorex DVR models.
---

# How to Connect to Lorex IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Lorex Technology** (a subsidiary of Dahua Technology through FLIR/Lorex) is a major consumer and prosumer security camera brand in North America. Lorex cameras are primarily manufactured by **Dahua Technology** and sold under the Lorex brand through retail channels including Amazon, Costco, and Best Buy. Lorex is one of the top-selling security camera brands in the United States and Canada.

**Key facts:**

- **Product lines:** LNB (bullet IP), LNE (dome/turret IP), LNZ (PTZ IP), LNC (consumer Wi-Fi), IPSC (legacy), L-series (legacy)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / (set during NVR/camera setup); some older models: admin / admin
- **ONVIF support:** Yes (most current models)
- **Video codecs:** H.264, H.265 (newer models)
- **OEM base:** Dahua Technology (some models use Hikvision firmware)

!!! info "Lorex uses multiple OEM sources"
    Most Lorex IP cameras are manufactured by Dahua and use Dahua's RTSP URL format. However, some Lorex models (particularly LNB2153 and MCNB2153) use Hikvision-based firmware with `/Streaming/Channels/` URLs. Check both URL formats if one doesn't work.

## RTSP URL Patterns

### Dahua-Based Models (Most Lorex IP Cameras)

Most Lorex IP cameras use Dahua's URL format:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Main stream | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Full resolution |
| Sub stream | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Lower resolution |

### Hikvision-Based Models

Some Lorex models use Hikvision firmware:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Main stream | `rtsp://IP:554//Streaming/Channels/1` | Full resolution (note double slash) |
| Sub stream | `rtsp://IP:554//Streaming/Channels/2` | Lower resolution |
| H.264 direct | `rtsp://IP:554/ch0_0.h264` | Direct H.264 stream |

### Model-Specific URLs

| Model | RTSP URL | OEM Base | Notes |
|-------|----------|----------|-------|
| LNB2153 | `rtsp://IP:554//Streaming/Channels/1` | Hikvision | 1080p bullet |
| LNB2184 | `rtsp://IP:554/video.mp4` | Dahua | 4MP bullet |
| LNE1001 | `rtsp://IP:554/` | Dahua | 1080p dome |
| LNE3003 | `rtsp://IP:554/video.mp4` | Dahua | 2K dome |
| LNZ4001 | `rtsp://IP:554/video.mp4` | Dahua | PTZ |
| MCNB2153 | `rtsp://IP:554//Streaming/Channels/1` | Hikvision | 1080p bullet |

### Alternative URL Formats

Some Lorex cameras also respond to these URLs:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/` | Root path (some models) |
| `rtsp://IP:554/video.mp4` | Video stream |
| `rtsp://IP:554/ch0_0.h264` | Direct H.264 |

### Legacy Models

| Model | URL | Notes |
|-------|-----|-------|
| IPSC Series | `rtsp://IP:554/` | Legacy IP cameras |
| L23WD | `rtsp://IP:554/` | Legacy wireless |
| IP1240 | `http://IP/GetData.cgi` | HTTP only |
| LNC104/116/204 | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Wi-Fi cameras, HTTP only |

## Connecting with VisioForge SDK

Use your Lorex camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code).

### Dahua-Based Models (Most Lorex Cameras)

```csharp
// Lorex camera (Dahua-based), main stream
var uri = new Uri("rtsp://192.168.1.65:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

### Hikvision-Based Lorex Models

```csharp
// Lorex LNB2153 (Hikvision-based), main stream
var uri = new Uri("rtsp://192.168.1.65:554//Streaming/Channels/1");
var username = "admin";
var password = "YourPassword";
```

See the [OEM identification guide](#determine-your-oem-base) in Troubleshooting to determine which URL format your Lorex camera uses.

## Snapshot URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/jpg/image.jpg` | Most Lorex IP cameras |
| Snapshot (auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Consumer Wi-Fi cameras |
| Snapshot (account) | `http://IP/snapshot.jpg?account=USER&password=PASS` | Alternative auth |
| GetData | `http://IP/GetData.cgi` | Legacy models |
| MJPEG Stream | `http://IP/video.mjpg` | Continuous MJPEG |

## Troubleshooting

### Determine your OEM base

Lorex cameras use firmware from different manufacturers. To determine which URL format to use:

1. Try the **Dahua format** first: `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0`
2. If that fails, try the **Hikvision format**: `rtsp://IP:554//Streaming/Channels/1`
3. Check the camera's web interface -- Dahua-based cameras have a blue/white web UI, while Hikvision-based ones have a dark gray/black UI

### NVR vs direct camera access

- When connecting through a Lorex NVR, use `channel=N` in the Dahua URL format to select the camera
- When connecting directly to an IP camera, always use `channel=1`

### Lorex consumer Wi-Fi cameras (LNC series)

The LNC series (LNC104, LNC116, LNC204) are consumer Wi-Fi cameras that typically don't support RTSP. They provide HTTP snapshot URLs only and are primarily designed for use with the Lorex app.

### Port 9000

Some very old Lorex cameras used port 9000 for streaming instead of 554. If standard port 554 doesn't work on an older model, try: `rtsp://IP:9000/`

## FAQ

**Are Lorex cameras the same as Dahua?**

Most Lorex IP cameras are manufactured by Dahua and use identical firmware. The RTSP URL format (`cam/realmonitor?channel=1&subtype=0`) is the same. However, some Lorex models use Hikvision firmware. See our [Dahua connection guide](dahua.md) for additional details.

**What is the default RTSP URL for Lorex cameras?**

Try `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` first (Dahua-based). If that fails, try `rtsp://admin:password@CAMERA_IP:554//Streaming/Channels/1` (Hikvision-based).

**Can I use Lorex cameras without the Lorex NVR?**

Yes. Lorex IP cameras with RTSP support can be connected directly using their individual IP addresses. You don't need the Lorex NVR for third-party software integration.

**Do Lorex cameras support ONVIF?**

Most current Lorex IP cameras support ONVIF. Consumer Wi-Fi cameras (LNC series) generally do not.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Dahua Connection Guide](dahua.md) — Same URL format for most models
- [Amcrest Connection Guide](amcrest.md) — Another Dahua OEM
- [Swann Connection Guide](swann.md) — Consumer/prosumer segment peer
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
