---
title: LILIN IP Camera RTSP URL Patterns and C# .NET Setup
description: Set up LILIN IP camera streaming in C# .NET with RTSP URL patterns, snapshot URLs, and integration code for LR, Z, D, S, and P series cameras.
---

# How to Connect to LILIN IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**LILIN** (Merit LILIN Co., Ltd.) is a Taiwanese professional security camera manufacturer headquartered in New Taipei City, Taiwan. Founded in 1980, LILIN is one of the oldest IP camera manufacturers in the world. The company is known for professional-grade surveillance cameras with distinctive RTSP URL patterns that encode the resolution directly in the URL path.

**Key facts:**

- **Product lines:** Z Series (bullet), S Series (speed dome), D Series (dome), LR Series (IR), P Series (panoramic)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / pass
- **ONVIF support:** Yes (most current models)
- **Video codecs:** H.264, MJPEG
- **Unique URL pattern:** Resolution encoded in RTSP path (e.g., `rtsph264720p`, `rtsph2641080p`)

!!! info "Resolution-Based RTSP Paths"
    LILIN uses a unique URL pattern where the resolution is encoded directly in the RTSP path (e.g., `rtsph264720p` for 720p, `rtsph2641080p` for 1080p). Make sure to use the correct resolution suffix for your camera model.

## RTSP URL Patterns

### Standard URL Format

LILIN cameras use a resolution-based RTSP path format:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/rtsph2641080p
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `rtsph264720p` | 720p stream | H.264 at 1280x720 resolution |
| `rtsph2641080p` | 1080p stream | H.264 at 1920x1080 resolution |
| `rtsph2641024p` | 1024p stream | H.264 at 1280x1024 resolution (note: double-slash in some models) |

### Camera Models

| Model | Resolution | Main Stream URL | Notes |
|-------|-----------|----------------|-------|
| LR7022E4 (IR bullet) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | LR series, 1080p |
| LR7722X (IR bullet) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | LR series, 1080p |
| IPR712M4.3 (PTZ) | 1280x1024 | `rtsp://IP:554//rtsph2641024p` | IPR series, double-slash path |
| Z Series (bullet) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Outdoor bullet cameras |
| D Series (dome) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Indoor/outdoor dome |
| S Series (speed dome) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | PTZ speed dome |

### Alternative URL Formats

Some LILIN models or firmware versions support these alternative URLs:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/rtsph2641080p` | Standard 1080p (recommended) |
| `rtsp://IP:554/rtsph264720p` | 720p stream |
| `rtsp://IP:554//rtsph2641024p` | 1024p stream (double-slash, some PTZ models) |

## Connecting with VisioForge SDK

Use your LILIN camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// LILIN LR7022E4, 1080p main stream
var uri = new Uri("rtsp://192.168.1.90:554/rtsph2641080p");
var username = "admin";
var password = "pass";
```

For 720p access, use `rtsph264720p` instead of `rtsph2641080p`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot (VGA) | `http://IP/getimage?camera=CHANNEL&fmt=vga` | VGA resolution snapshot |
| Snapshot by Channel | `http://IP/getimage[CHANNEL]` | Replace CHANNEL with camera number |
| Quick Snapshot | `http://IP/snap` | Simple snapshot URL |
| CGI Snapshot | `http://IP/cgi-bin/net_jpeg.cgi?ch=CHANNEL` | CGI-based snapshot |
| Auth Snapshot | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | URL-based authentication |
| Direct Image | `http://IP/image/CHANNEL.jpg` | Direct JPEG image by channel |

## Troubleshooting

### "401 Unauthorized" error

LILIN cameras ship with default credentials of `admin` / `pass`. If you have changed the password through the web interface, ensure you update the credentials in your RTSP URL.

1. Access the camera at `http://CAMERA_IP` in a browser
2. Log in with your credentials
3. Verify the RTSP settings under the network configuration section

### Double-slash in RTSP path

Some LILIN models, particularly the IPR PTZ series, require a double-slash (`//`) before the resolution path. If a single-slash URL fails:

- Try `rtsp://IP:554//rtsph2641024p` instead of `rtsp://IP:554/rtsph2641024p`
- This is commonly seen with 1024p resolution models

### Choosing the correct resolution suffix

LILIN cameras do not use `subtype=0/1` like many other brands. Instead, the stream resolution is selected by changing the URL path:

- `rtsph264720p` for 720p (1280x720)
- `rtsph2641080p` for 1080p (1920x1080)
- `rtsph2641024p` for 1024p (1280x1024)

If you specify a resolution your camera does not support, the connection will fail.

### Port 554 connection refused

Verify that RTSP is enabled on the camera:

- Web interface: Check **Network > RTSP** settings
- Confirm port 554 is not blocked by a firewall
- Default RTSP port is 554

## FAQ

**What is the default RTSP URL for LILIN cameras?**

The most common URL is `rtsp://admin:pass@CAMERA_IP:554/rtsph2641080p` for the 1080p main stream. Replace the resolution suffix (`rtsph2641080p`) with the appropriate value for your camera's resolution.

**Do LILIN cameras support ONVIF?**

Yes. Most current LILIN models support ONVIF, which provides an alternative method for discovering and connecting to the camera without needing brand-specific URL patterns.

**Why does LILIN use a different RTSP URL format?**

LILIN encodes the resolution directly in the RTSP path rather than using channel/subtype parameters like Dahua or Hikvision. This is a proprietary design choice. The format is straightforward once you know which resolution suffix your camera model supports.

**What are the default login credentials for LILIN cameras?**

The default username is `admin` and the default password is `pass`. It is recommended to change these credentials after initial setup for security purposes.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [AVTech Connection Guide](avtech.md) — Taiwanese industrial cameras
- [BrickCom Connection Guide](brickcom.md) — Taiwanese industrial cameras
- [RTSP Camera Integration Guide](../videocapture/video-sources/ip-cameras/rtsp.md) — LILIN RTSP stream configuration
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
