---
title: How to Connect to Speco Technologies IP Camera in C# .NET
description: Speco Technologies O Series, VIP, and DVR RTSP integration for C# .NET. URL patterns, channel selection, and VisioForge SDK code for IP and analog cameras.
---

# How to Connect to Speco Technologies IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Speco Technologies** is an American professional surveillance company based in Amityville, New York. Founded in 1969, Speco manufactures IP cameras, analog cameras, DVRs, NVRs, and access control equipment for the professional security integrator market. Speco products are sold through authorized distributors and security integrators rather than direct-to-consumer channels, making them a common choice in commercial installations.

**Key facts:**

- **Product lines:** O Series (IP cameras: O2B, O2D, OINT), VIP Series (IP cameras), ZIP Series, SIP Series, LS Series, DVR lines (TH/TL, RS, PCPRO)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (all current IP cameras)
- **Video codecs:** H.264 (all current models), MPEG-4 (older models)

!!! info "Multiple Product Lines"
    Speco Technologies has many distinct product lines, each with different RTSP URL formats. Identify your exact model series (O, VIP, LS, ZIP, SIP, or DVR type) before configuring the stream URL. The root stream `rtsp://IP:554/` works on many Speco devices as a quick test.

## RTSP URL Patterns

### O Series IP Cameras

The O Series is Speco's current IP camera line, including bullet (O2B), dome (O2D), and intensifier (OINT) models:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/
```

| Model | Resolution | Main Stream URL | Notes |
|-------|-----------|----------------|-------|
| O2B2 (bullet) | 1080p | `rtsp://IP:554/` | Root stream |
| O2D4 (dome) | 1080p | `rtsp://IP:554/` | Root stream |
| OINT56B1G (intensifier) | 1080p | `rtsp://IP:554/mpeg4` | MPEG-4 stream |
| OINT56B1G (intensifier) | 1080p | `rtsp://IP:554/` | Root stream (H.264) |

### VIP Series IP Cameras

The VIP Series uses a numbered stream path format:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/1/stream1
```

| Model | Resolution | Main Stream URL | Notes |
|-------|-----------|----------------|-------|
| VIP2B1M (bullet) | 1080p | `rtsp://IP:554/1/stream1` | Stream 1 (main) |
| VIP2C1N (cube) | 1080p | `rtsp://IP:554/1/stream1` | Stream 1 (main) |

### LS Series

The LS Series uses a channel-based H.264 path and also supports a credential-in-URL format:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/cam1/h264` | Channel 1 H.264 stream |
| `rtsp://IP:554/cam2/h264` | Channel 2 H.264 stream |
| `rtsp://IP:554/cam[N]/h264` | Channel N H.264 stream |
| `rtsp://IP:554//user=admin_password=tlJwpbo6_channel=1_stream=0.sdp` | Credential-in-URL format |

!!! warning "LS Series Credential Format"
    The LS Series supports an unusual credential-in-URL format where the username and password are embedded directly in the path. The password in this format is device-specific and may differ from the web interface password. Check the device's RTSP settings page for the correct value.

### ZIP Series

The ZIP Series uses a profile-based streaming format:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//stream0/Channel=0;Profile=0
```

| Model | Main Stream URL | Notes |
|-------|----------------|-------|
| ZIP2B (bullet) | `rtsp://IP:554//stream0/Channel=0;Profile=0` | Profile 0 (main) |

### DVR Models

Speco DVRs use various URL formats depending on the DVR line:

| DVR Series | URL Pattern | Notes |
|------------|-------------|-------|
| DVR4WM | `rtsp://IP:554/` | Root stream |
| RS Series | `rtsp://IP:554/Live/Channel=1` | Live channel format |
| RS Series | `rtsp://IP:554/Live/Channel=2` | Channel 2 |
| General DVRs | `rtsp://IP:554/` | Root stream (fallback) |

### DVR Channel URLs (RS Series)

For Speco RS Series DVRs:

| Channel | Main Stream URL |
|---------|----------------|
| Camera 1 | `rtsp://IP:554/Live/Channel=1` |
| Camera 2 | `rtsp://IP:554/Live/Channel=2` |
| Camera N | `rtsp://IP:554/Live/Channel=N` |

### All URL Formats Summary

| URL Pattern | Product Line | Notes |
|-------------|-------------|-------|
| `rtsp://IP:554/` | O Series, DVRs, general | Root stream (works on many devices) |
| `rtsp://IP:554/mpeg4` | O Series (older) | MPEG-4 stream |
| `rtsp://IP:554/1/stream1` | VIP Series | Numbered stream format |
| `rtsp://IP:554/cam[N]/h264` | LS Series | Channel-based H.264 |
| `rtsp://IP:554//stream0/Channel=0;Profile=0` | ZIP Series | Profile-based format |
| `rtsp://IP:554/Live/Channel=N` | RS DVR | Live channel format |

## Connecting with VisioForge SDK

Use your Speco camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Speco O2D4 dome camera, root stream
var uri = new Uri("rtsp://192.168.1.64:554/");
var username = "admin";
var password = "admin";
```

For VIP Series cameras, use the `/1/stream1` path instead:

```csharp
// Speco VIP2B1M bullet camera, main stream
var uri = new Uri("rtsp://192.168.1.64:554/1/stream1");
var username = "admin";
var password = "admin";
```

## Snapshot and MJPEG URLs

### IP Camera Snapshots

| Type | URL Pattern | Models | Notes |
|------|-------------|--------|-------|
| Still Image | `http://IP/stillimg.jpg` | O2B2, O2D4, OINT56B1G | Basic JPEG snapshot |
| Still Image (port 554) | `http://IP:554/stillimg.jpg` | O2B2, O2D4, OINT56B1G | Alternate port |
| Encoder Snapshot | `http://IP/cgi-bin/encoder?USER=user&PWD=pass&SNAPSHOT` | IP-SD10X, SIP Series | CGI-based with credentials |
| System Stream | `http://IP/cgi-bin/cmd/system?GET_STREAM&USER=user&PWD=pass` | Various IP cameras | System command format |

### DVR Snapshots

| Type | URL Pattern | DVR Series | Notes |
|------|-------------|------------|-------|
| Full Image | `http://IP/images1full` | Various DVRs | Replace `1` with channel number |
| SIF Image | `http://IP/images1sif` | Various DVRs | Lower resolution, replace `1` with channel |
| Get Image | `http://IP/getimage?camera=1&fmt=full` | Various DVRs | Replace `1` with channel number |
| Mobile Snapshot | `http://IP/mobile/channel1.jpg` | PCPRO DVR | Mobile-optimized, replace `1` with channel |
| TH/TL Stream | `http://IP/ivop.get?action=live&THREAD_ID=` | TH/TL DVR | Live stream via HTTP |

## Troubleshooting

### Inconsistent URL formats across product lines

Speco Technologies has many different product lines, each with its own RTSP URL format. If one URL pattern does not work, try the root stream `rtsp://IP:554/` first as a baseline test. Then try the format specific to your product line as listed in the tables above.

### Root stream limitations

The root stream (`rtsp://IP:554/`) works on many Speco devices for the main stream but is unreliable for accessing sub-streams or specific channels on multi-channel devices. Use the product-line-specific URL format for full control over stream selection.

### LS Series credential-in-URL format

The LS Series uses an unusual URL format where credentials are embedded in the path (`/user=admin_password=VALUE_channel=1_stream=0.sdp`). The password in this format may be a device-generated value that differs from the web interface password. Check the device's **RTSP Settings** page in the web interface for the correct credential string.

### Network discovery

Speco provides a DDNS tool and device discovery utility for finding cameras on the network. Download the Speco DDNS tool from the Speco Technologies website to locate devices that are not responding at expected IP addresses.

### Default credentials

Speco devices typically ship with default credentials of `admin` / `admin`. If these do not work, the password may have been changed during installation by the security integrator.

## FAQ

**What is the default RTSP URL for Speco cameras?**

For most Speco IP cameras, try the root stream first: `rtsp://admin:admin@CAMERA_IP:554/`. For VIP Series cameras use `rtsp://IP:554/1/stream1`, and for LS Series cameras use `rtsp://IP:554/cam1/h264`. The correct URL depends on your specific product line.

**Do Speco cameras support ONVIF?**

Yes. All current Speco IP cameras support ONVIF. ONVIF discovery and streaming is the most reliable way to connect to Speco cameras if you are unsure of the exact RTSP URL format for your model.

**Why are there so many different URL formats for Speco cameras?**

Speco Technologies has been manufacturing surveillance equipment since 1969 and has acquired or developed multiple product lines over the decades. Each product line (O Series, VIP, LS, ZIP, SIP, DVR lines) may use different firmware and streaming architectures, resulting in different URL formats. Always identify your exact model series before configuring the connection.

**How do I find my Speco camera on the network?**

Use Speco's DDNS tool or device discovery utility, available from the Speco Technologies website. Alternatively, use ONVIF discovery through the VisioForge SDK or a network scanning tool to locate the camera's IP address on your local network.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [EverFocus Connection Guide](everfocus.md) — Professional surveillance cameras
- [Save Original RTSP Stream](../mediablocks/Guides/rtsp-save-original-stream.md) — Record Speco streams without re-encoding
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
