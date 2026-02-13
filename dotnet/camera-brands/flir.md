---
title: How to Connect to FLIR IP Camera in C# .NET
description: Connect to FLIR (Teledyne FLIR) cameras in C# .NET with RTSP URL patterns and code samples for Quasar, Saros, Elara thermal, and CF/CM models.
---

# How to Connect to FLIR IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**FLIR Systems** (now **Teledyne FLIR** following the 2021 acquisition by Teledyne Technologies) is a leading manufacturer of thermal imaging cameras and visible-light security cameras. Headquartered in Wilsonville, Oregon, USA, FLIR serves enterprise, critical infrastructure, and government markets. FLIR is best known for thermal imaging but also produces a full range of visible-light IP cameras for professional surveillance. FLIR previously acquired **Lorex** and **DVTEL** (now FLIR Latitude VMS).

**Key facts:**

- **Product lines:** Quasar (premium multi-sensor/mini-dome), Saros (perimeter detection), Elara (thermal+visible dual-sensor), CM (compact mini dome), CF (compact fixed), PT/PTZ (pan-tilt-zoom), FC (thermal-only), FLIR FX (consumer, discontinued)
- **Protocol support:** RTSP, ONVIF (Quasar, Saros, Elara series), HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (most models), admin / fliradmin (some Quasar models)
- **ONVIF support:** Yes (Quasar, Saros, Elara series)
- **Video codecs:** H.264, H.265 (Quasar series), MJPEG
- **Thermal specialization:** FLIR thermal cameras output radiometric data in addition to visible-light video, with separate RTSP streams for each sensor

!!! warning "Thermal cameras have separate streams"
    FLIR dual-sensor cameras (Elara, PT-series) provide separate RTSP streams for visible and thermal channels. Typically `ch0` is the visible channel and `ch1` is the thermal channel.

## RTSP URL Patterns

### Current Models (Quasar, Saros, Elara)

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Visible channel | `rtsp://IP:554/ch0` | Primary visible stream |
| Thermal channel | `rtsp://IP:554/ch1` | Thermal stream (dual-sensor models) |
| Visible (alt) | `rtsp://IP:554/vis` | Visible on PT-series |
| Wide FOV thermal | `rtsp://IP:554/wfov` | PT-series, wide field of view |
| Auth stream | `rtsp://IP:554/0/USERNAME:PASSWORD/main` | With embedded credentials |

### Model-Specific URLs

| Model Series | RTSP URL | Type | Notes |
|-------------|----------|------|-------|
| Quasar CM-3308 | `rtsp://IP:554/ch0` | Mini dome | Compact multi-sensor |
| Quasar CM-6208 | `rtsp://IP:554/ch0` | Mini dome | Compact multi-sensor |
| D-series (fixed dome) | `rtsp://IP:554/ch0` | Fixed dome | Visible stream |
| F-series (fixed) | `rtsp://IP:554/ch0` | Fixed | Visible stream |
| PT-series (PTZ-35x140) | `rtsp://IP:554/vis` | PTZ | Visible channel |
| PT-series (PTZ-35x140) | `rtsp://IP:554/wfov` | PTZ | Wide FOV thermal |
| Elara (visible) | `rtsp://IP:554/ch0` | Thermal+visible | Visible channel |
| Elara (thermal) | `rtsp://IP:554/ch1` | Thermal+visible | Thermal channel |
| FC-series (thermal) | `rtsp://IP:554/ch0` | Thermal only | Thermal stream |

### Dual-Sensor Thermal Cameras

FLIR dual-sensor cameras provide both visible and thermal video on separate channels:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Elara visible | `rtsp://IP:554/ch0` | Visible-light sensor |
| Elara thermal | `rtsp://IP:554/ch1` | Thermal sensor |
| PT visible | `rtsp://IP:554/vis` | Visible-light sensor |
| PT wide thermal | `rtsp://IP:554/wfov` | Wide FOV thermal sensor |

## Connecting with VisioForge SDK

Use your FLIR camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// FLIR Quasar mini-dome, visible stream
var uri = new Uri("rtsp://192.168.1.70:554/ch0");
var username = "admin";
var password = "admin";
```

For dual-sensor cameras, use `ch1` to access the thermal stream. For PT-series cameras, use `/vis` for the visible channel or `/wfov` for the wide FOV thermal channel.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/jpg/image.jpg` | Some models |
| Snapshot (alt) | `http://IP/snapshot.jpg` | Alternative path |

## Troubleshooting

### Thermal vs. visible channels

FLIR dual-sensor cameras (Elara, PT-series) expose separate RTSP streams for each sensor:

- `ch0` = visible-light channel (most models)
- `ch1` = thermal channel (most models)
- `/vis` = visible channel (PT-series)
- `/wfov` = wide FOV thermal (PT-series)

If you connect to the wrong channel, you may receive thermal imagery when you expected visible or vice versa. Check your camera's documentation for channel assignments.

### Default credentials differ by model

- **Most FLIR cameras:** admin / admin
- **Some Quasar models:** admin / fliradmin
- **Current Teledyne FLIR firmware:** Password may need to be set during initial configuration

Always change default credentials before deploying cameras on a production network.

### Teledyne FLIR rebranding

Teledyne Technologies acquired FLIR Systems in 2021. Current firmware versions may display Teledyne FLIR branding, and newer cameras may ship with updated web interfaces and configuration tools. RTSP URL patterns remain consistent with legacy FLIR cameras.

### FLIR FX consumer cameras

The discontinued FLIR FX consumer camera line used cloud-only access and does not support RTSP streaming. These cameras cannot be connected via direct RTSP URLs.

### FLIR Lorex cameras

FLIR acquired Lorex, but Lorex cameras use their own RTSP URL patterns (based on Dahua firmware). Do not use FLIR URL patterns for Lorex cameras. See the [Lorex](lorex.md) page for Lorex-specific URLs.

### ONVIF availability

ONVIF is supported on current-generation cameras (Quasar, Saros, Elara). Older FLIR cameras and consumer models (FLIR FX) do not support ONVIF. For ONVIF-supported models, use ONVIF discovery as an alternative to manual RTSP URL configuration.

## FAQ

**What is the default RTSP URL for FLIR cameras?**

For most FLIR cameras, use `rtsp://admin:admin@CAMERA_IP:554/ch0` for the visible stream. For dual-sensor thermal cameras, use `ch1` for the thermal stream. For PT-series cameras, use `/vis` (visible) or `/wfov` (wide FOV thermal).

**Does FLIR support H.265?**

Quasar-series cameras support H.265 encoding. Other FLIR camera lines primarily use H.264 and MJPEG. Check your specific model's datasheet for codec support.

**How do I access the thermal stream on a FLIR dual-sensor camera?**

Dual-sensor cameras provide separate RTSP streams for visible and thermal channels. On Elara models, `ch0` is visible and `ch1` is thermal. On PT-series models, `/vis` is visible and `/wfov` is the wide FOV thermal stream. Connect to the appropriate URL for the desired sensor.

**Are FLIR and Teledyne FLIR the same company?**

Yes. Teledyne Technologies acquired FLIR Systems in 2021. The company now operates as Teledyne FLIR. Existing FLIR cameras continue to work with the same RTSP URL patterns. Newer products may carry Teledyne FLIR branding.

**Can I use FLIR URL patterns for Lorex cameras?**

No. Although FLIR acquired Lorex, Lorex cameras use Dahua-based firmware with different RTSP URL patterns. See the [Lorex](lorex.md) camera connection guide for the correct URLs.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Basler Connection Guide](basler.md) — Industrial / machine vision cameras
- [Mobotix Connection Guide](mobotix.md) — German industrial cameras
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
