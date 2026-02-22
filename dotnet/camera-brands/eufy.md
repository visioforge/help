---
title: Eufy Security Camera RTSP Connection Guide for C# .NET
description: Connect to Eufy Security cameras in C# .NET with RTSP URL patterns. ONVIF and RTSP support varies by model. eufyCam, SoloCam, and Indoor Cam guide.
---

# How to Connect to Eufy Security Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Eufy Security** is a smart home security brand owned by **Anker Innovations**, headquartered in Changsha, China. Eufy is known for local storage (no mandatory cloud subscription), AI-powered detection, and a wide range of indoor, outdoor, battery, and doorbell cameras. RTSP and ONVIF support varies significantly by model and firmware version.

**Key facts:**

- **Product lines:** eufyCam (battery), SoloCam (standalone), Indoor Cam, Floodlight Cam, Video Doorbell, HomeBase
- **Protocol support:** RTSP (select models, must be enabled), ONVIF (newer firmware), Eufy Security app
- **Default RTSP port:** 554
- **Default credentials:** No standard defaults — set during RTSP enablement
- **ONVIF support:** Added in recent firmware updates for many models
- **Video codecs:** H.264, H.265 (select models)
- **Local storage:** Yes — HomeBase or camera microSD (no cloud required for recording)

!!! info "RTSP/ONVIF Support Varies by Model"
    Eufy has been gradually adding RTSP and ONVIF support across its product line through firmware updates. Not all models support these features. Check the Eufy Security app settings for your specific camera to see if RTSP is available.

## RTSP Support by Model

| Model | RTSP | ONVIF | Notes |
|-------|------|-------|-------|
| eufyCam 2 / 2 Pro | Yes (via HomeBase) | Yes | Requires HomeBase 2 |
| eufyCam 2C / 2C Pro | Yes (via HomeBase) | Yes | Requires HomeBase 2 |
| eufyCam 3 / 3C | Yes (via HomeBase 3) | Yes | Requires HomeBase 3 |
| eufyCam S330 | Yes (via HomeBase 3) | Yes | 4K model |
| SoloCam S340 | Yes | Yes | Dual-lens, standalone RTSP |
| SoloCam C210 | Yes | Yes | Standalone with RTSP |
| Indoor Cam 2K | Yes | Yes | WiFi, firmware-dependent |
| Indoor Cam Pan & Tilt | Yes | Yes | WiFi, firmware-dependent |
| Floodlight Cam 2 Pro | Yes | Yes | Wired |
| Video Doorbell 2K | Limited | No | Via HomeBase only |
| Video Doorbell Dual | Limited | No | Via HomeBase only |

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

Newer Eufy firmware supports ONVIF discovery. Use ONVIF to automatically find cameras on your network instead of manually configuring RTSP URLs.

### Firmware inconsistencies

Eufy has been rolling out RTSP/ONVIF support gradually. Different cameras in your setup may have different capabilities depending on their firmware version. Always update all devices to the latest firmware.

## FAQ

**Do Eufy cameras support RTSP?**

Many Eufy cameras now support RTSP, but it must be enabled in the Eufy Security app and varies by model. HomeBase-connected cameras stream RTSP through the HomeBase, while standalone cameras stream directly. Check your specific model's capabilities in the app settings.

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
