---
title: "Eufy RTSP and ONVIF Support by Model, Stream URL and Setup"
description: "Do Eufy cameras support RTSP or ONVIF? Model table for eufyCam, SoloCam and Indoor Cam, enabling RTSP in the app, live0/live1 URLs on port 554, C# samples."
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
  - C#

---

# How to Connect to Eufy Security Camera in C# .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Eufy Security** is a smart home security brand owned by **Anker Innovations**, headquartered in Changsha, China. Eufy is known for local storage (no mandatory cloud subscription), AI-powered detection, and a wide range of indoor, outdoor, battery, and doorbell cameras. RTSP support varies significantly by model and firmware version, and Eufy's support pages rule out ONVIF for its SoloCams, floodlight cameras and every Video Doorbell.

**Key facts:**

- **Product lines:** eufyCam (battery), SoloCam (standalone), Indoor Cam, Floodlight Cam, Video Doorbell, HomeBase
- **Protocol support:** RTSP (select models, must be enabled), Eufy Security app
- **Default RTSP port:** 554
- **Default credentials:** No standard defaults — set during RTSP enablement
- **ONVIF support:** No for SoloCams, floodlight cameras and every Video Doorbell (Eufy's support pages); not documented for eufyCam and Indoor Cam
- **Doorbells:** no RTSP (Eufy's doorbell FAQ)
- **Video codecs:** H.264, H.265 (select models)
- **Local storage:** Yes — HomeBase or camera microSD (no cloud required for recording)

!!! info "RTSP Support Varies by Model"
    Eufy has been gradually adding RTSP support across its product line through firmware updates. Not all models support it, and we found no Eufy page that confirms ONVIF for any model. Check the Eufy Security app settings for your specific camera to see if RTSP is available. For doorbells the answer is line-wide rather than per model: Eufy's doorbell FAQ rules out RTSP and ONVIF for all of them, and we found no separate page for the newer E340, C31 or C30.

## RTSP and ONVIF Support by Model { #rtsp-support-by-model }

| Model | RTSP | ONVIF | Notes |
|-------|------|-------|-------|
| eufyCam 2 / 2 Pro | Yes (via HomeBase) | Not documented | Requires HomeBase 2 |
| eufyCam 2C / 2C Pro | Yes (via HomeBase) | Not documented | Requires HomeBase 2 |
| eufyCam 3 / 3C | Yes (via HomeBase 3) | Not documented | Requires HomeBase 3 |
| eufyCam S330 | Yes (via HomeBase 3) | Not documented | 4K model |
| SoloCam S340 | Yes | No | Dual-lens, standalone RTSP |
| SoloCam C210 | Yes | No | Standalone with RTSP |
| Indoor Cam 2K | Yes | Not documented | WiFi, firmware-dependent |
| Indoor Cam Pan & Tilt | Yes | Not documented | WiFi, firmware-dependent |
| Floodlight Cam 2 Pro | Yes | No | Wired |
| Video Doorbell 2K (Wired / Battery-Powered) | No | No | Eufy documents no RTSP or ONVIF for any doorbell |
| Video Doorbell Dual (Wired / Battery-Powered) | No | No | Eufy documents no RTSP or ONVIF for any doorbell |

## Enabling RTSP

### For HomeBase-Connected Cameras (eufyCam series)

1. Open the **Eufy Security** app
2. Go to **HomeBase Settings > Storage > NAS** or **RTSP**
3. Enable RTSP streaming
4. Set an RTSP username and password
5. Note the RTSP URL displayed for each camera

### For Standalone Cameras (SoloCam, Indoor Cam, Floodlight)

1. Open the **Eufy Security** app
2. Select your camera → **Settings** (gear icon)
3. Look for **RTSP** or **Advanced > RTSP Stream**
4. Enable RTSP and set credentials
5. Note the RTSP URL provided

## RTSP URL Patterns

### Standard URL Format

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live0
```

| Stream | URL Pattern | Description |
|--------|-------------|-------------|
| Main stream | `rtsp://IP:554/live0` | Full resolution |
| Sub stream | `rtsp://IP:554/live1` | Lower resolution |

### HomeBase RTSP URLs

When connected through a HomeBase, the RTSP URL points to the HomeBase IP:

```
rtsp://[USERNAME]:[PASSWORD]@[HOMEBASE_IP]:554/live0
```

For multiple cameras on one HomeBase, each camera gets a unique stream path shown in the app.

### Alternative URL Formats

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/live0` | Main stream (common) |
| `rtsp://IP:554/live1` | Sub stream |
| `rtsp://IP:554/stream1` | Alternative format (some models) |
| `rtsp://IP:554/h264_stream` | H.264 explicit (some firmware) |

## Connecting with VisioForge SDK

Use your Eufy camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Eufy SoloCam S340, main stream
var uri = new Uri("rtsp://192.168.1.90:554/live0");
var username = "rtsp_user"; // set in Eufy Security app
var password = "rtsp_pass";
```

For sub-stream access, use `/live1` instead of `/live0`.

## Troubleshooting

### RTSP option not visible in app

RTSP support requires specific firmware versions. Update your camera and HomeBase firmware through the Eufy Security app. If RTSP still does not appear, your model may not yet support it.

### HomeBase vs standalone RTSP

- **HomeBase cameras** (eufyCam series): RTSP streams come from the **HomeBase IP**, not the camera IP. The HomeBase acts as a proxy.
- **Standalone cameras** (SoloCam, Indoor Cam): RTSP streams come directly from the **camera IP**.

### Stream drops on battery cameras

Battery-powered eufyCam models may stop RTSP streaming when in standby mode. The camera must be actively recording or in "always streaming" mode for continuous RTSP access. This significantly impacts battery life.

### ONVIF discovery

Eufy says its SoloCams, floodlight cameras and every Video Doorbell do not support ONVIF, and ONVIF is not documented for any other camera, so do not rely on ONVIF discovery to find them. Enable RTSP in the Eufy Security app and use the RTSP URL directly.

### Firmware inconsistencies

Eufy has been rolling out RTSP support gradually. Different cameras in your setup may have different capabilities depending on their firmware version. Always update all devices to the latest firmware.

## FAQ

**Do Eufy cameras support RTSP?**

Many Eufy cameras now support RTSP, but it must be enabled in the Eufy Security app and varies by model. Eufy's doorbell FAQ is blunt: no Eufy doorbell supports RTSP. HomeBase-connected cameras stream RTSP through the HomeBase, while standalone cameras stream directly. Check your specific model's capabilities in the app settings.

**Do Eufy cameras support ONVIF?**

No, as far as Eufy documents it. Eufy's support pages state that SoloCams, floodlight cameras and every doorbell do not support ONVIF, and we found no Eufy page confirming ONVIF for eufyCam or Indoor Cam models. Use RTSP instead: enable it in the Eufy Security app and connect with the URL the app shows.

**Do Eufy cameras require a cloud subscription for RTSP?**

No. RTSP streaming works locally without any cloud subscription. Eufy cameras store footage on the HomeBase or camera microSD card. The cloud subscription (Eufy Security Plan) is optional and provides additional cloud storage and features.

**Can I use Eufy cameras without the Eufy app?**

Initial setup requires the Eufy Security app. After setup and RTSP enablement, you can access the RTSP stream without the app. However, firmware updates and configuration changes still require the app.

**What is the difference between HomeBase RTSP and standalone RTSP?**

HomeBase RTSP streams all connected cameras through the HomeBase's IP address. The HomeBase acts as a gateway. Standalone cameras (SoloCam, Indoor Cam, Floodlight) stream directly from their own IP. HomeBase RTSP may have slightly higher latency.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Arlo Connection Guide](arlo.md) — Consumer alternative (no RTSP)
- [Reolink Connection Guide](reolink.md) — Consumer with native RTSP
- [EZVIZ Connection Guide](ezviz.md) — Smart home cameras with RTSP
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
