---
title: CP Plus IP Camera RTSP URL and C# .NET Connection Guide
description: CP Plus UNC, NC, RNP, Guard+, and Cosmic series RTSP URL patterns for C# .NET. Integrate with VisioForge SDK for IP camera streaming and recording.
---

# How to Connect to CP Plus IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**CP Plus** (Aditya Infotech Ltd.) is India's #1 security camera brand and one of the largest surveillance manufacturers in the world, headquartered in Delhi, India. CP Plus cameras are primarily **Dahua OEM** products, meaning most models run Dahua firmware and share identical RTSP URL patterns. Some models use alternative chipsets with different URL formats. CP Plus dominates the Indian, Middle Eastern, and Southeast Asian markets with a comprehensive range of IP cameras, NVRs, and analog systems.

**Key facts:**

- **Product lines:** UNC (IP cameras), RNP (NVRs), VAC (analog), Guard+ (wireless), E series (entry-level), Cosmic (value)
- **OEM base:** Dahua (most models use Dahua firmware and URL patterns)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (most models)
- **Video codecs:** H.264, H.265 (newer models)
- **Dominant market:** India (#1), Middle East, Southeast Asia

!!! info "CP Plus = Dahua OEM"
    CP Plus cameras are primarily Dahua OEM products and use the same RTSP URL patterns. See our [Dahua connection guide](dahua.md) for additional details. Some CP Plus models use the `/VideoInput/` URL format instead.

## RTSP URL Patterns

### Standard URL Format (Dahua-style)

Most CP Plus cameras use the Dahua `cam/realmonitor` URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=1
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Camera channel (1 for standalone cameras) |
| `subtype` | 0 | Main stream (highest resolution) |
| `subtype` | 1 | Sub stream (lower resolution, less bandwidth) |

### Camera Models

| Model | Type | Main Stream URL | Notes |
|-------|------|----------------|-------|
| CP-UNC-DP10L2C | IP dome | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua-style URL |
| CP-UNC-TY20FL2C | IP turret | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua-style URL |
| CP-NC9W-K | Network camera | `rtsp://IP:554/VideoInput/1/mpeg4/1` | VideoInput format |
| B series | Basic | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua-style URL |

### NVR Channel URLs

For CP Plus NVRs (CP-RNP-36D, CN-RNP-36D, etc.):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Camera 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Camera N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

### Alternative URL Formats

Some CP Plus models use different URL patterns depending on firmware and chipset:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Standard Dahua-style (most models) |
| `rtsp://IP:554//cam/realmonitor` | Dahua-style alternate (double slash) |
| `rtsp://IP:554/VideoInput/1/mpeg4/1` | VideoInput format (CP-NC9W-K, CP-UNC-DP10L2C on some firmware) |
| `rtsp://IP:554//cam/realmonitor?channel=1&subtype=00&authbasic=AUTH` | With base64-encoded authentication |

## Connecting with VisioForge SDK

Use your CP Plus camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// CP Plus CP-UNC-DP10L2C, main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `subtype=1` instead of `subtype=0`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi?1` | Requires basic auth (CP-UNC-TY20FL2C) |
| JPEG Snapshot (legacy) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | URL-based auth for older models |
| JPEG Image | `http://IP/cgi-bin/jpg/image.cgi` | Alternative JPEG endpoint |
| MJPEG Stream | `http://IP/api/mjpegvideo.cgi?InputNumber=1&StreamNumber=CHANNEL` | Continuous MJPEG stream |
| Direct HTTP Stream | `http://IP:8008/` | Some NVR models (CN-RNP-36D, CP-RNP-36D) |

## Troubleshooting

### "401 Unauthorized" error

CP Plus cameras ship with default credentials (`admin` / `admin`). If you've changed the password via the web interface or mobile app, ensure your RTSP URL uses the updated credentials.

1. Access the camera at `http://CAMERA_IP` in a browser
2. Log in with your credentials
3. Verify RTSP is enabled under **Setup > Network > Port**
4. Use those credentials in your RTSP URL

### Dahua-style URL not working

Some CP Plus models (particularly the CP-NC series) use the `/VideoInput/` URL format instead of the Dahua `cam/realmonitor` pattern. Try:

```
rtsp://admin:password@IP:554/VideoInput/1/mpeg4/1
```

### Port 554 vs custom port

Check the RTSP port setting at:

- Web interface: **Setup > Network > Port > RTSP Port**
- Default is 554

### Stream type confusion

- `subtype=0` = Main stream (full resolution, higher bandwidth)
- `subtype=1` = Sub stream (reduced resolution, lower bandwidth)

### Direct HTTP stream on port 8008

Some CP Plus NVR models (CP-RNP-36D, CN-RNP-36D) expose a direct HTTP stream on port 8008. Try accessing `http://CAMERA_IP:8008/` if standard RTSP is unavailable.

## FAQ

**Are CP Plus cameras the same as Dahua?**

Most CP Plus cameras are manufactured by Dahua and use Dahua firmware. The RTSP URL format (`cam/realmonitor?channel=1&subtype=0`) is identical for the majority of models. However, some CP Plus models use different chipsets with the `/VideoInput/` URL format. Any code written for Dahua cameras generally works with CP Plus and vice versa.

**What is the default RTSP URL for CP Plus cameras?**

The URL is `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=1` for the sub stream. Replace `subtype=1` with `subtype=0` for the main stream. For NVR setups, change `channel=1` to the appropriate channel number.

**Do CP Plus cameras support ONVIF?**

Yes. Most current CP Plus IP cameras support ONVIF, which provides a standardized way to discover and connect to cameras regardless of the specific URL format.

**What if my CP Plus camera uses a different URL format?**

Some CP Plus models (especially the CP-NC series) use `rtsp://IP:554/VideoInput/1/mpeg4/1` instead of the Dahua-style URL. If the standard URL does not work, try the VideoInput format. You can also use ONVIF discovery to automatically detect the correct URL.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Dahua Connection Guide](dahua.md) — Same URL format for most models
- [Amcrest Connection Guide](amcrest.md) — Another Dahua OEM
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
