---
title: Pelco IP Camera RTSP URL Patterns and C# .NET Setup
description: Connect to Pelco Sarix and Spectra cameras in C# .NET with RTSP URL patterns and code samples for IX, IMP, IME, and Spectra PTZ models.
---

# How to Connect to Pelco IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Pelco** (now part of **Motorola Solutions**) is a leading manufacturer of professional video surveillance equipment, headquartered in Fresno, California. Pelco is particularly strong in enterprise, government, and critical infrastructure markets. The brand is known for its **Sarix** fixed camera line and **Spectra** PTZ camera line. Motorola Solutions acquired Pelco in 2020.

**Key facts:**

- **Product lines:** Sarix (Professional/Enhanced/Value fixed cameras), Spectra (Professional PTZ), IX (fixed box), IMP/IME (mini dome), D-series (dome PTZ)
- **Protocol support:** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, Pelco D/P serial protocol
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (must be changed on first login for current models)
- **ONVIF support:** Yes (all current Sarix and Spectra models)
- **Video codecs:** H.264, H.265 (Sarix Professional), MJPEG

!!! info "Double slash in RTSP URLs"
    Pelco cameras consistently use a **double forward slash** before the stream path: `rtsp://IP:554//stream1`. This is intentional and required for most Pelco models.

## RTSP URL Patterns

### Current Models (Sarix Professional/Enhanced/Value)

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Main stream | `rtsp://IP:554//stream1` | Full resolution (note double slash) |
| Sub stream | `rtsp://IP:554//stream2` | Lower resolution |
| Low-res stream | `rtsp://IP:554/LowResolutionVideo` | Lowest quality |
| Channel stream | `rtsp://IP:554/stream1` | Single slash (some models) |
| Numbered channel | `rtsp://IP:554/1/stream1` | Channel-specific |

### Model-Specific URLs

| Model Series | RTSP URL | Type | Notes |
|-------------|----------|------|-------|
| Sarix Pro (IMP/IME) | `rtsp://IP:554//stream1` | Fixed dome | Current generation |
| Sarix Enhanced (IX) | `rtsp://IP:554//stream1` | Fixed box | Mid-range |
| Sarix Value | `rtsp://IP:554//stream1` | Fixed | Entry-level |
| IX10 | `rtsp://IP:554//stream1` | Fixed box | Professional |
| IX30C / IX30DN | `rtsp://IP:554//stream1` | Fixed box | Day/night |
| IXDN30 | `rtsp://IP:554//stream1` | Fixed box | Day/night |
| IXE10LW | `rtsp://IP:554//stream1` | Fixed dome | Wireless |
| IXE20DN | `rtsp://IP:554//stream1` | Fixed dome | Day/night |
| IXP31 | `rtsp://IP:554//stream1` | Fixed dome | Professional |
| IMP519 | `rtsp://IP:554//stream1` | Mini dome | 5MP |
| IMP1110-1 / IMP1110-1E | `rtsp://IP:554//stream1` | Mini dome | Sarix Pro |
| IM10C10 | `rtsp://IP:554//stream1` | Multi-sensor | Sarix IMM |
| IM10DN10-1E | `rtsp://IP:554//stream1` | Multi-sensor | Day/night |
| D5230-ADFRZ28 | `rtsp://IP:554//stream1` | PTZ dome | Spectra |
| Spectra IV | `rtsp://IP:554//stream1` | PTZ dome | Legacy PTZ |
| Spectra Professional | `rtsp://IP:554//stream1` | PTZ dome | Current PTZ |

### Multi-Channel / Multi-Sensor

For multi-channel Pelco devices:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Channel 1, main | `rtsp://IP:554/1/stream1` | First sensor/channel |
| Channel 2, main | `rtsp://IP:554/2/stream1` | Second sensor/channel |
| Channel stream (alt) | `rtsp://IP:554/stream1` | Single channel (some models) |

### Legacy Models

| Model | URL | Notes |
|-------|-----|-------|
| IP110 / IP-110 | `http://IP/api/jpegControl.php?frameRate=10` | JPEG stream |
| Spectra IV (HTTP) | `http://IP/jpeg` | JPEG snapshot |
| Spectra IV (pull) | `http://IP/jpeg/pull` | Continuous JPEG |
| Spectra IV (API) | `http://IP/api/jpegControl.php?frameRate=10` | Frame-rate JPEG |

## Connecting with VisioForge SDK

Use your Pelco camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Pelco Sarix camera, main stream
var uri = new Uri("rtsp://192.168.1.85:554//stream1");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `//stream2` instead. For multi-sensor cameras, use `/1/stream1` for channel selection.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/jpeg` | Most current models |
| JPEG (channel) | `http://IP/jpeg?id=1` | Channel-specific |
| JPEG (API) | `http://IP/api/jpegControl.php?frameRate=10` | Legacy models |
| JPEG (tmpfs) | `http://IP/tmpfs/auto.jpg` | Auto-capture |
| Image file | `http://IP/img.jpg` | Simple snapshot |

## Troubleshooting

### Double slash is required

Most Pelco cameras require a **double forward slash** before the stream path:

- Correct: `rtsp://IP:554//stream1`
- May not work: `rtsp://IP:554/stream1`

If a single-slash URL fails, always try the double-slash variant first.

### Channel numbering for multi-sensor

Pelco multi-sensor cameras (IM10-series, Sarix IMM) use numbered channel paths:

- `rtsp://IP:554/1/stream1` — first sensor
- `rtsp://IP:554/2/stream1` — second sensor

Single-sensor cameras should use `//stream1` without a channel number.

### Pelco D/P protocol vs RTSP

Pelco is also known for the **Pelco D** and **Pelco P** serial communication protocols used to control PTZ cameras. These are serial protocols for PTZ control, not video streaming. Video streaming always uses RTSP or HTTP regardless of which PTZ control protocol is used.

### Spectra PTZ cameras

Pelco Spectra PTZ cameras use the same RTSP URL format (`//stream1`) as fixed cameras. PTZ control is handled separately via ONVIF PTZ commands or Pelco D/P serial protocol, not through the RTSP URL.

## FAQ

**What is the default RTSP URL for Pelco cameras?**

For most Pelco cameras, use `rtsp://admin:password@CAMERA_IP:554//stream1` (note the double slash). For the sub-stream, use `//stream2`. Multi-sensor models use `/1/stream1` for channel-specific access.

**Is Pelco still an independent company?**

No. Pelco was acquired by Motorola Solutions in 2020. Current Pelco cameras are manufactured and supported by Motorola Solutions. The Pelco brand and product lines (Sarix, Spectra) continue under Motorola Solutions' video security portfolio.

**Do Pelco cameras support ONVIF?**

Yes. All current Pelco Sarix and Spectra cameras support ONVIF Profile S, G, and T. ONVIF is the recommended discovery and configuration method for new Pelco integrations.

**What is the difference between Pelco D and RTSP?**

Pelco D (and Pelco P) are serial protocols for PTZ camera control (pan, tilt, zoom commands). RTSP is the video streaming protocol. You use RTSP for video and Pelco D/ONVIF for PTZ control — they serve different purposes and are not interchangeable.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Avigilon Connection Guide](avigilon.md) — Also Motorola Solutions, enterprise cameras
- [Honeywell Connection Guide](honeywell.md) — Enterprise surveillance cameras
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
