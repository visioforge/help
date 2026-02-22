---
title: Basler IP Camera RTSP URL Connection Guide for C# .NET
description: Connect to Basler BIP2 IP cameras in C# .NET with RTSP URL patterns and code samples. Includes notes on machine vision vs IP security camera protocols.
---

# How to Connect to Basler IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Basler** (Basler AG) is a German camera manufacturer headquartered in Ahrensburg, Germany, founded in 1988. Basler is a world leader in industrial machine vision cameras and also produces IP security cameras under the **BIP2** product line. While Basler's machine vision cameras use specialized industrial protocols, the BIP2 series provides standard RTSP and ONVIF connectivity for security and surveillance applications.

**Key facts:**

- **Product lines:** ace (machine vision), dart (compact), boost (high-speed), BIP2 (IP security)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI (BIP2 series); GigE Vision, USB3 Vision (machine vision)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (BIP2 series)
- **Video codecs:** H.264, MPEG-4, MJPEG
- **Primary use:** Industrial vision, factory automation, quality inspection, IP surveillance

!!! info "Machine Vision Cameras Use Different Protocols"
    Basler's ace, dart, and boost machine vision cameras use GigE Vision or USB3 Vision protocols, not RTSP. These require Basler's Pylon SDK or a GenICam-compatible framework. The RTSP URLs on this page apply to Basler's BIP2 IP security camera line.

## RTSP URL Patterns

### Standard URL Format

Basler BIP2 IP cameras use simple path-based RTSP URLs:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264
```

| Stream | URL Pattern | Description |
|--------|-------------|-------------|
| H.264 main stream | `rtsp://IP:554/h264` | Primary stream, best quality |
| MPEG-4 stream | `rtsp://IP:554/mpeg4` | Legacy MPEG-4 encoded stream |
| JPEG over RTSP | `rtsp://IP:554/jpeg` | JPEG frames over RTSP |

### Camera Models

| Model | Type | Main Stream URL | Codec |
|-------|------|----------------|-------|
| BIP2-1280c (720p) | IP bullet | `rtsp://IP:554/h264` | H.264 |
| BIP2-1300c (1.3MP) | IP bullet | `rtsp://IP:554/h264` | H.264 |
| BIP2-1920c (1080p) | IP bullet | `rtsp://IP:554/h264` | H.264 |
| BIP2-1300c-dn | IP day/night | `rtsp://IP:554/h264` | H.264 |
| BIP2-1920c-dn | IP day/night | `rtsp://IP:554/h264` | H.264 |

### Alternative URL Formats

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/h264` | H.264 stream (recommended) |
| `rtsp://IP:554/mpeg4` | MPEG-4 stream (legacy) |
| `rtsp://IP:554/jpeg` | JPEG over RTSP |

## Connecting with VisioForge SDK

Use your Basler camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Basler BIP2, H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/h264");
var username = "admin";
var password = "admin";
```

For MPEG-4 streams, replace `/h264` with `/mpeg4` in the URL.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| MJPEG Stream | `http://IP/cgi-bin/mjpeg` | Continuous MJPEG stream |
| JPEG Snapshot | `http://IP/cgi-bin/jpeg?stream=0` | Snapshot from channel 0 |
| JPEG Snapshot (channel) | `http://IP/cgi-bin/jpeg?stream=CHANNEL` | Snapshot from specific channel |

## Troubleshooting

### Machine vision camera not connecting via RTSP

Basler's ace, dart, and boost cameras do not support RTSP. These cameras use GigE Vision (Ethernet) or USB3 Vision (USB) protocols and require Basler's Pylon SDK or a GenICam-compatible library. Only the BIP2 IP camera series supports RTSP streaming.

### "401 Unauthorized" error

Basler BIP2 cameras ship with default credentials `admin` / `admin`. If the credentials have been changed:

1. Access the camera web interface at `http://CAMERA_IP`
2. Log in and verify or reset the credentials
3. Use the updated credentials in your RTSP URL

### No video output on MPEG-4 stream

Some newer Basler BIP2 firmware versions may default to H.264 only. If the MPEG-4 stream returns no data:

1. Open the camera web interface
2. Navigate to video stream settings
3. Ensure MPEG-4 encoding is enabled
4. Alternatively, use the `/h264` stream path instead

### ONVIF discovery not finding the camera

ONVIF is supported on BIP2 series cameras only. Ensure:

- The camera firmware is up to date
- ONVIF is enabled in the camera's network settings
- The camera and discovery client are on the same subnet

## FAQ

**What is the default RTSP URL for Basler cameras?**

For Basler BIP2 IP cameras, the default URL is `rtsp://admin:admin@CAMERA_IP:554/h264` for the H.264 main stream. Replace the credentials if they have been changed from the defaults.

**Can I use VisioForge SDK with Basler machine vision cameras?**

The RTSP-based connection described on this page applies to Basler BIP2 IP security cameras only. Basler's machine vision cameras (ace, dart, boost) use GigE Vision or USB3 Vision protocols and require Basler's Pylon SDK or a GenICam-compatible framework for direct integration.

**Do Basler cameras support ONVIF?**

Yes, but only the BIP2 IP security camera series supports ONVIF. Basler's machine vision cameras use industrial protocols (GigE Vision, USB3 Vision) instead.

**What codecs do Basler IP cameras support?**

Basler BIP2 cameras support H.264, MPEG-4, and MJPEG. H.264 is recommended for the best balance of quality and bandwidth efficiency.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Mobotix Connection Guide](mobotix.md) — German industrial cameras
- [FLIR Connection Guide](flir.md) — Industrial and thermal imaging
- [Building Camera Applications with Media Blocks](../mediablocks/GettingStarted/camera.md)
- [SDK Installation & Samples](index.md#get-started)
