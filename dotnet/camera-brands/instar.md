---
title: INSTAR IP Camera RTSP URL and C# .NET Connection Guide
description: INSTAR IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx HD camera RTSP URL patterns for C# .NET. Stream and record with VisioForge Video Capture SDK integration.
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

# How to Connect to INSTAR IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**INSTAR** (INSTAR Deutschland GmbH) is a German IP camera manufacturer headquartered in Hanau, Germany. INSTAR specializes in affordable indoor and outdoor IP cameras for the consumer and small business market, with a strong presence in Europe, particularly Germany. INSTAR cameras are known for local storage options, MQTT smart home integration (Home Assistant, ioBroker, Node-RED), and straightforward setup.

**Key facts:**

- **Product lines:** IN-2xxx/3xxx/4xxx (legacy VGA), IN-5xxx (720p), IN-6xxx (720p HD), IN-7xxx (1080p Full HD), IN-8xxx (current 1080p+), IN-9xxx (current 4K/WQHD)
- **Protocol support:** RTSP, HTTP, ONVIF (IN-6xxx and newer), MQTT
- **Default RTSP port:** 554
- **Default credentials:** admin / instar (varies by model)
- **ONVIF support:** Yes (IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx series)
- **Video codecs:** H.265 (IN-9xxx), H.264 (IN-6xxx/7xxx/8xxx), MPEG-4 (IN-5xxx), MJPEG (legacy IN-2xxx/3xxx/4xxx)

## RTSP URL Patterns

INSTAR cameras use a distinctive URL format with a **double forward slash** before the stream number.

### URL Format (HD Models)

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//11
```

!!! warning "Double forward slash"
    INSTAR HD cameras use a **double forward slash** (`//`) before the stream number. Using a single slash will result in a connection failure.

### HD Models (IN-6xxx / IN-7xxx / IN-8xxx / IN-9xxx)

| Stream | RTSP URL | Resolution | Notes |
|--------|----------|------------|-------|
| Main stream | `rtsp://IP:554//11` | Full resolution | H.264 / H.265 |
| Sub stream | `rtsp://IP:554//12` | Lower resolution | Bandwidth-friendly |
| Third stream | `rtsp://IP:554//13` | Mobile optimized | Lowest resolution |

### Model-Specific URLs

| Model | RTSP URL | Resolution | Type |
|-------|----------|------------|------|
| IN-6012 HD | `rtsp://IP:554//11` | 720p | Indoor pan/tilt |
| IN-6014 HD | `rtsp://IP:554//11` | 720p | Indoor |
| IN-7011 HD | `rtsp://IP:554//11` | 1080p | Indoor pan/tilt |
| IN-8015 Full HD | `rtsp://IP:554//11` | 1080p | Indoor/Outdoor |
| IN-9008 Full HD | `rtsp://IP:554//11` | 1080p+ | Outdoor PoE |
| IN-9020 Full HD | `rtsp://IP:554//11` | WQHD/4K | Outdoor PoE |

### Older 720p Models (IN-5xxx -- MPEG-4)

IN-5xxx cameras use a different RTSP path with MPEG-4 encoding:

| Model | RTSP URL | Resolution | Notes |
|-------|----------|------------|-------|
| IN-5905 HD | `rtsp://IP:554/MediaInput/mpeg4` | 720p | Outdoor |
| IN-5907 HD | `rtsp://IP:554/MediaInput/mpeg4` | 720p | Outdoor |

### Legacy Models (IN-2xxx / IN-3xxx / IN-4xxx -- HTTP Only)

Legacy VGA-resolution INSTAR cameras do not support RTSP. They use HTTP-based streaming only:

| Model Series | HTTP URL | Type | Notes |
|-------------|----------|------|-------|
| IN-2xxx/3xxx/4xxx | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=32&rate=0` | ASF stream | VGA resolution |
| IN-2xxx/3xxx/4xxx | `http://IP/videostream.cgi?rate=11` | MJPEG | No audio |
| IN-2xxx/3xxx/4xxx | `http://IP//iphone/11?USER:PASS&` | Mobile stream | iPhone-compatible |

## Connecting with VisioForge SDK

Use your INSTAR camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// INSTAR IN-8015 Full HD, main stream -- note the double forward slash!
var uri = new Uri("rtsp://192.168.1.50:554//11");
var username = "admin";
var password = "instar";
```

For sub-stream access, use `//12` instead of `//11`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| Snapshot (HD) | `http://IP/tmpfs/auto.jpg` | IN-6xxx/7xxx/8xxx/9xxx |
| Snapshot (HD, auth) | `http://IP/snap.jpg?usr=USER&pwd=PASS` | With credentials |
| Snapshot (legacy) | `http://IP/snapshot.cgi` | IN-2xxx/3xxx/4xxx |
| Snapshot (legacy, auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Legacy with credentials |
| ASF Stream (legacy) | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=32&rate=0` | VGA ASF |
| MJPEG Stream (legacy) | `http://IP/videostream.cgi?rate=11` | Legacy MJPEG |

## Troubleshooting

### Double forward slash is required

The most common INSTAR connection issue is forgetting the **double forward slash** before the stream number. The correct URL is `rtsp://IP:554//11` (two slashes), not `rtsp://IP:554/11` (one slash).

### Legacy cameras have no RTSP support

IN-2xxx, IN-3xxx, and IN-4xxx cameras are HTTP-only. They do not support RTSP at all. Use the ASF or MJPEG HTTP streaming URLs for these models.

### IN-5xxx uses a different RTSP path

IN-5xxx cameras use `rtsp://IP:554/MediaInput/mpeg4` instead of the `//11` path used by newer HD models. If the `//11` URL fails on a 720p INSTAR camera, check whether your model is from the IN-5xxx series.

### MQTT and smart home integration

INSTAR cameras support MQTT for integration with Home Assistant, ioBroker, and Node-RED. MQTT is used for camera control and event notifications, not for video streaming. For video integration with smart home platforms, use the RTSP URL.

### PoE availability

IN-8xxx and IN-9xxx outdoor models support Power over Ethernet (PoE), allowing a single cable for both power and data. Indoor models typically require a separate power adapter.

### Credentials vary by model

While the common default credentials are admin / instar, some models may use different defaults. Check the camera's documentation or label for the factory credentials. INSTAR cameras typically require changing the default password during initial setup.

## FAQ

**What is the default RTSP URL for INSTAR cameras?**

For current HD models (IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx), the URL is `rtsp://admin:instar@CAMERA_IP:554//11`. Note the double forward slash before `11`. For IN-5xxx models, use `rtsp://admin:instar@CAMERA_IP:554/MediaInput/mpeg4`.

**Do all INSTAR cameras support RTSP?**

No. Legacy models (IN-2xxx, IN-3xxx, IN-4xxx) are VGA-resolution cameras that only support HTTP-based streaming in ASF or MJPEG format. All IN-5xxx and newer cameras support RTSP.

**What is the difference between stream //11, //12, and //13?**

Stream `//11` is the main (highest quality) stream, `//12` is a lower-resolution sub stream suitable for remote viewing over limited bandwidth, and `//13` is a mobile-optimized third stream with the lowest resolution.

**Do INSTAR cameras support ONVIF?**

Yes. ONVIF is supported on IN-6xxx, IN-7xxx, IN-8xxx, and IN-9xxx series cameras. Legacy models do not support ONVIF. You can use the VisioForge SDK's ONVIF features for camera discovery and PTZ control on supported models.

**Can I integrate INSTAR cameras with Home Assistant?**

Yes. INSTAR cameras support MQTT, making them easy to integrate with Home Assistant, ioBroker, and Node-RED for automation and event-driven actions. For video streaming in Home Assistant, use the RTSP URL in a generic camera integration.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [ABUS Connection Guide](abus.md) — German consumer / smart home cameras
- [Save Original RTSP Stream](../mediablocks/Guides/rtsp-save-original-stream.md) — Record INSTAR streams without re-encoding
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
