---
title: Linksys IP Camera RTSP URL Patterns and C# .NET Setup
description: Connect to Linksys WVC, PVC, and LCAB cameras in C# .NET with RTSP URL patterns, ASF/MJPEG streams, and code samples for discontinued WVC series models.
---

# How to Connect to Linksys IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Linksys** is an American networking company based in Irvine, California. Originally acquired by Cisco Systems in 2003, the brand was sold to Belkin (a Foxconn subsidiary) in 2013. During the Cisco ownership years, Linksys produced the popular **WVC (Wireless Video Camera)** series of IP cameras for the consumer and prosumer market. The camera product line was discontinued around 2014, but many units remain deployed and operational.

Because Linksys was a Cisco brand, its cameras share identical URL patterns and firmware with Cisco consumer camera products. The `.sav` extension in RTSP URLs is a proprietary Cisco/Linksys endpoint format.

**Key facts:**

- **Product lines:** WVC Series (WVC54G, WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210, WVC2300), PVC Series (PVC2300), LCAB Series
- **Protocol support:** RTSP, HTTP/ASF, MJPEG, MMS (Windows Media)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Limited (LCAB series only)
- **Video codecs:** MPEG-4/ASF (WVC series), H.264 (newer models), MJPEG

!!! warning "Discontinued product line"
    Linksys IP cameras were discontinued around 2014. No new firmware updates or official support are available. The information on this page is provided for legacy deployments. Many WVC models require Internet Explorer with ActiveX for their web interface.

!!! info "Linksys = Cisco consumer cameras"
    Linksys cameras use the same URL patterns as Cisco consumer cameras since Linksys was a Cisco brand. See our [Cisco connection guide](cisco.md) for additional details and enterprise Cisco camera models.

## RTSP URL Patterns

### Standard URL Format

Most Linksys cameras use the Cisco `/img/media.sav` RTSP path:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/img/media.sav
```

!!! note "Unusual `.sav` extension"
    The `.sav` extension is a proprietary Cisco/Linksys RTSP endpoint -- it is not a standard media file format. Do not confuse it with a file download URL.

### RTSP URLs by Model

| Model | RTSP URL | Codec | Notes |
|-------|----------|-------|-------|
| WVC54GCA | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G, 640x480 |
| WVC80N | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-N, 640x480 |
| WVC80N (alt) | `rtsp://IP:554/img/video.sav` | MPEG-4 | Alternative video endpoint |
| WVC210 | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G PTZ |
| WVC200 | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G |
| PVC2300 | `rtsp://IP:554/video.mp4` | MPEG-4/H.264 | Small business box camera |
| LCAB03VLNOD | `rtsp://IP:554//ONVIF/channel2` | H.264 | ONVIF-enabled outdoor camera |

### Model Family Summary

| Model Family | RTSP Stream | HTTP ASF | MJPEG | Snapshot CGI |
|-------------|-------------|----------|-------|--------------|
| WVC54G / WVC54GC / WVC54GCA | `/img/media.sav` | Yes | Yes | Yes |
| WVC80N | `/img/media.sav`, `/img/video.sav` | Yes | Yes | -- |
| WVC200 / WVC210 | `/img/media.sav` | Yes | Yes | Yes |
| WVC2300 | `/img/media.sav` | Yes | -- | -- |
| PVC2300 | `/video.mp4` | Yes | -- | -- |
| LCAB series | `//ONVIF/channel2` | -- | -- | -- |

## Connecting with VisioForge SDK

Use your Linksys camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Linksys WVC80N, primary RTSP stream
var uri = new Uri("rtsp://192.168.1.60:554/img/media.sav");
var username = "admin";
var password = "admin";
```

For PVC2300 cameras, use `/video.mp4` instead of `/img/media.sav`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Compatible Models |
|------|-------------|-------------------|
| ASF Video Stream | `http://IP/img/video.asf` | WVC54G, WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210, WVC11b |
| MJPEG Stream | `http://IP/img/video.mjpeg` | WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210 |
| MJPEG Single Frame | `http://IP/img/mjpeg.jpg` | Most WVC models |
| MJPEG CGI | `http://IP/img/mjpeg.cgi` | Most WVC models |
| MJPEG (uppercase) | `http://IP/MJPEG.CGI` | Some WVC models |
| High-Res Snapshot | `http://IP/img/snapshot.cgi?size=3` | WVC54GCA, WVC200, WVC210 |
| Medium Snapshot | `http://IP/img/snapshot.cgi?size=2` | WVC54GCA, WVC200, WVC210 |
| VGA Snapshot | `http://IP/img/snapshot.cgi?img=vga` | WVC54GCA, WVC200, WVC210 |
| Alternative ASF | `http://IP/videostream.asf` | WVC54GC, WVC80N |
| MMS Stream | `mms://IP/img/video.asf` | Legacy Windows Media (all WVC models) |

!!! tip "HTTP streams as RTSP fallback"
    Many Linksys WVC cameras work more reliably with HTTP-based ASF or MJPEG streams than with RTSP. If the RTSP URL is unresponsive, try the ASF stream at `http://IP/img/video.asf` as a fallback.

## Troubleshooting

### RTSP stream not connecting

Linksys WVC cameras have limited RTSP support. Many models primarily serve video over HTTP using ASF (Advanced Streaming Format) rather than true RTSP:

1. Verify the camera IP address and that port 554 is open
2. Confirm RTSP is enabled in the camera's web interface
3. Try the HTTP ASF stream (`http://IP/img/video.asf`) as an alternative
4. Some models require the web interface to be accessed first (via Internet Explorer with ActiveX) before RTSP becomes available

### ASF stream requires specific handling

The ASF (Advanced Streaming Format) streams from WVC cameras use a Microsoft proprietary container. The VisioForge SDK handles ASF streams automatically. If you encounter issues:

- Ensure you are connecting via HTTP, not RTSP, for ASF URLs
- ASF streams may require Windows Media components or LAV filters on some systems

### MMS protocol streams

The `mms://` protocol URLs are Windows Media-specific and only work with Windows Media Player or compatible decoders. For modern applications, use the HTTP ASF URL (`http://IP/img/video.asf`) instead of the MMS equivalent.

### Web interface requires Internet Explorer

Many WVC models require Internet Explorer with ActiveX controls for their web configuration interface. Use Internet Explorer or an ActiveX-compatible browser to access camera settings. The RTSP and HTTP streams themselves work with any client.

### Camera not discoverable on network

Linksys cameras do not support modern discovery protocols (except LCAB series with ONVIF). To find the camera:

1. Check your router's DHCP lease table for the camera's IP address
2. Use the Linksys Camera Utility (if still available) for discovery
3. Try the default IP address assigned by the camera (check the model's manual)
4. Use a network scanner such as Advanced IP Scanner

## FAQ

**What is the default RTSP URL for Linksys cameras?**

For most Linksys WVC cameras, use `rtsp://admin:admin@CAMERA_IP:554/img/media.sav`. For the PVC2300, use `rtsp://admin:admin@CAMERA_IP:554/video.mp4` instead. If RTSP does not work, try the HTTP ASF stream at `http://CAMERA_IP/img/video.asf`.

**Are Linksys cameras still available for purchase?**

No. Linksys discontinued its entire IP camera product line around 2014, shortly after the brand was sold from Cisco to Belkin/Foxconn. No firmware updates or official support are available. However, many WVC and PVC cameras remain in use and functional.

**Do Linksys cameras support ONVIF?**

Only the LCAB series cameras have ONVIF support. The WVC and PVC series do not support ONVIF. For WVC cameras, use the direct RTSP or HTTP URL patterns listed above.

**Are Linksys and Cisco camera URLs the same?**

Yes. Linksys cameras were produced during Cisco's ownership of the brand and share the same firmware and URL patterns as Cisco consumer cameras. The `/img/media.sav` RTSP path and `/img/video.asf` HTTP path are identical across both brands. See our [Cisco connection guide](cisco.md) for more details.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Cisco Connection Guide](cisco.md) — Same URL patterns, parent company
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
