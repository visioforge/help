---
title: Avigilon IP Camera RTSP URL Patterns and C# .NET Setup
description: Avigilon H5A, H5M, H5 Pro, H5SL, and Unity NVR RTSP URL patterns for C# .NET. Enterprise camera integration with VisioForge SDK code samples.
---

# How to Connect to Avigilon IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Avigilon** (Avigilon Corporation) is an enterprise security camera manufacturer originally based in Vancouver, Canada. Founded in 2004, Avigilon was acquired by **Motorola Solutions** in 2018 for approximately $1 billion. The company is known for high-resolution cameras (up to 61MP), AI-powered video analytics including Unusual Motion Detection (UMD) and Appearance Search, and proprietary HDSM (High Definition Stream Management) technology. Avigilon cameras are widely deployed in enterprise, government, and critical infrastructure environments.

**Key facts:**

- **Product lines:** H5A (bullet/dome), H5M (mini dome), H5 Pro (multi-sensor), H5SL (value line), Unity (NVRs)
- **Previous lines:** HD Pro, HD Multisensor, HD Micro Dome, HD PTZ
- **Protocol support:** RTSP, ONVIF (Profile S, Profile T), HTTP
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (must be changed on initial setup)
- **ONVIF support:** Yes (Profile S, Profile T)
- **Video codecs:** H.264, H.265, HDSM SmartCodec (H.265-based)
- **Known for:** AI analytics (Unusual Motion Detection, Appearance Search), HDSM SmartCodec

!!! info "Avigilon is now part of Motorola Solutions"
    Avigilon is now part of Motorola Solutions. The Avigilon camera line continues under the Motorola Solutions Video Security & Access Control division. See also our [Pelco guide](pelco.md) for another Motorola Solutions camera brand.

## RTSP URL Patterns

### Standard URL Format

Avigilon cameras use the `defaultPrimary` / `defaultSecondary` URL pattern with a unicast stream type parameter:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/defaultPrimary?streamType=u
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `defaultPrimary` | Primary stream | Main stream (highest resolution) |
| `defaultSecondary` | Secondary stream | Sub stream (lower resolution, less bandwidth) |
| `streamType` | `u` | Unicast stream delivery |

### Camera Models

| Model Series | Type | Main Stream URL | Notes |
|-------------|------|----------------|-------|
| H5A Bullet | Fixed bullet | `rtsp://IP:554/defaultPrimary?streamType=u` | AI-enabled, up to 8MP |
| H5A Dome | Fixed dome | `rtsp://IP:554/defaultPrimary?streamType=u` | AI-enabled, up to 8MP |
| H5M Mini Dome | Mini dome | `rtsp://IP:554/defaultPrimary?streamType=u` | Compact form factor |
| H5 Pro Multi-sensor | Multi-sensor | `rtsp://IP:554/defaultPrimary0?streamType=u` | See multi-sensor note below |
| H5SL Bullet | Value bullet | `rtsp://IP:554/defaultPrimary?streamType=u` | Cost-effective line |
| H5SL Dome | Value dome | `rtsp://IP:554/defaultPrimary?streamType=u` | Cost-effective line |
| HD Pro | Legacy high-res | `rtsp://IP:554/defaultPrimary?streamType=u` | Up to 61MP |

### Multi-Sensor Camera URLs

For Avigilon H5 Pro and other multi-sensor cameras, each sensor head has its own stream index:

| Sensor | Main Stream | Sub Stream |
|--------|-------------|------------|
| Sensor 1 | `rtsp://IP:554/defaultPrimary0?streamType=u` | `rtsp://IP:554/defaultSecondary0?streamType=u` |
| Sensor 2 | `rtsp://IP:554/defaultPrimary1?streamType=u` | `rtsp://IP:554/defaultSecondary1?streamType=u` |
| Sensor 3 | `rtsp://IP:554/defaultPrimary2?streamType=u` | `rtsp://IP:554/defaultSecondary2?streamType=u` |
| Sensor 4 | `rtsp://IP:554/defaultPrimary3?streamType=u` | `rtsp://IP:554/defaultSecondary3?streamType=u` |

### Alternative URL Formats

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/defaultPrimary?streamType=u` | Standard primary (recommended) |
| `rtsp://IP:554/defaultSecondary?streamType=u` | Secondary / sub stream |
| `rtsp://IP:554/defaultPrimary0?streamType=u` | Primary stream alternate (also used for multi-sensor sensor 1) |

## Connecting with VisioForge SDK

Use your Avigilon camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Avigilon H5A dome, primary stream (unicast)
var uri = new Uri("rtsp://192.168.1.100:554/defaultPrimary?streamType=u");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `defaultSecondary` instead of `defaultPrimary`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/snapshot.jpg` | Requires basic auth |

## Troubleshooting

### "401 Unauthorized" error

Avigilon cameras require the default password to be changed during initial setup. If you have not configured the camera yet:

1. Access the camera at `http://CAMERA_IP` in a browser
2. Complete the initial setup wizard and set a strong password
3. Use those credentials in your RTSP URL

### HDSM SmartCodec streams

Avigilon's HDSM SmartCodec is based on H.265. Ensure your decoder supports H.265 when connecting to cameras configured to use HDSM SmartCodec. If you experience decoding issues, try switching the camera to standard H.264 encoding in the camera's web interface.

### Stream type parameter

The `streamType=u` parameter requests unicast delivery. If you omit this parameter, the camera may default to multicast, which can cause issues on networks not configured for multicast routing.

### Multi-sensor cameras show only one view

For multi-sensor models (H5 Pro), each sensor is accessed as a separate stream. Use `defaultPrimary0`, `defaultPrimary1`, etc. to access individual sensor heads. See the multi-sensor URL table above.

## FAQ

**What is the default RTSP URL for Avigilon cameras?**

The URL is `rtsp://admin:password@CAMERA_IP:554/defaultPrimary?streamType=u` for the primary stream. Use `defaultSecondary` instead of `defaultPrimary` for the lower-resolution sub stream.

**Do Avigilon cameras support ONVIF?**

Yes. Avigilon cameras support ONVIF Profile S and Profile T. ONVIF can be enabled through the camera's web interface or Avigilon Control Center (ACC) software.

**What is HDSM SmartCodec?**

HDSM (High Definition Stream Management) SmartCodec is Avigilon's proprietary compression technology built on H.265. It reduces bandwidth and storage requirements by intelligently encoding different regions of the image at different quality levels while maintaining detail in areas of interest. Streams encoded with HDSM SmartCodec are compatible with standard H.265 decoders.

**Can I use Avigilon cameras without Avigilon Control Center?**

Yes. While Avigilon Control Center (ACC) is the recommended VMS, the cameras expose standard RTSP streams and support ONVIF, allowing integration with any RTSP-compatible application including VisioForge SDKs.

**How do I access individual sensors on multi-sensor cameras?**

Each sensor head on a multi-sensor camera (such as the H5 Pro) has its own stream URL. Use `defaultPrimary0` for sensor 1, `defaultPrimary1` for sensor 2, and so on. Each sensor can also have a secondary stream accessed via `defaultSecondary0`, `defaultSecondary1`, etc.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Pelco Connection Guide](pelco.md) — Also Motorola Solutions, enterprise cameras
- [ONVIF Capture with Postprocessing](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Avigilon ONVIF capture pipeline
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
