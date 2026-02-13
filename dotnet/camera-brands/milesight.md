---
title: How to Connect to Milesight IP Camera in C# .NET
description: Connect to Milesight IP cameras in C# .NET with RTSP URL patterns and code samples for MS-C, MS-A, MS-V, MS-F, and MS-B series models.
---

# How to Connect to Milesight IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Milesight Technology** is a Chinese manufacturer of IP cameras and IoT devices, headquartered in Xiamen, China. Milesight targets professional and SMB markets with a rapidly growing lineup of AI-enabled cameras at competitive price points. The brand is known for strong ONVIF compliance, built-in AI analytics (face detection, LPR, people counting), and straightforward integration with third-party VMS platforms.

**Key facts:**

- **Product lines:** MS-C (mini dome/bullet), MS-A (PTZ/speed dome), MS-V (vandal dome), MS-F (fisheye), MS-B (box), MS-N (NVR)
- **Protocol support:** RTSP, ONVIF (Profile S/G/T on all current models), HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / ms1234 (must be changed on first login)
- **ONVIF support:** Yes (all current models, Profile S/G/T)
- **Video codecs:** H.264, H.265, MJPEG

!!! info "Double slash in RTSP URLs"
    Milesight cameras use a **double forward slash** before `main` and `sub` in their RTSP URLs: `rtsp://IP:554//main`. This is intentional and required for all current Milesight models.

## RTSP URL Patterns

### Current Models (All Series)

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Main stream | `rtsp://IP:554//main` | Full resolution (note double slash) |
| Sub stream | `rtsp://IP:554//sub` | Lower resolution |
| Root stream | `rtsp://IP:554/` | Fallback |

### Model-Specific URLs

All current Milesight camera models use the same RTSP URL pattern:

| Model Series | RTSP URL | Type | Notes |
|-------------|----------|------|-------|
| MS-C2672-P | `rtsp://IP:554//main` | Mini dome | 2MP |
| MS-C3366-FP | `rtsp://IP:554//main` | Bullet | 3MP, AI |
| MS-C3366-FPH | `rtsp://IP:554//main` | Bullet | 3MP, AI, heater |
| MS-C2363 | `rtsp://IP:554//main` | Mini dome | 2MP |
| MS-2681 | `rtsp://IP:554//main` | Dome | 8MP |
| MS-3672 | `rtsp://IP:554//main` | Bullet | 3MP |
| MS-A series (PTZ) | `rtsp://IP:554//main` | PTZ | Speed dome |
| MS-V series (vandal) | `rtsp://IP:554//main` | Vandal dome | IK10 rated |
| MS-F series (fisheye) | `rtsp://IP:554//main` | Fisheye | 360-degree |
| MS-B series (box) | `rtsp://IP:554//main` | Box | Professional |

### NVR Channel Streams

For Milesight MS-N series NVRs, use the `channel` parameter to select individual camera streams:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Channel 1, main | `rtsp://IP:554//main?channel=1` | NVR channel 1 |
| Channel 2, main | `rtsp://IP:554//main?channel=2` | NVR channel 2 |
| Channel 1, sub | `rtsp://IP:554//sub?channel=1` | NVR channel 1, low-res |
| Channel 2, sub | `rtsp://IP:554//sub?channel=2` | NVR channel 2, low-res |

## Connecting with VisioForge SDK

Use your Milesight camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Milesight camera, main stream -- note the double slash before "main"
var uri = new Uri("rtsp://192.168.1.90:554//main");
var username = "admin";
var password = "ms1234";
```

For sub-stream access, use `//sub` instead of `//main`. For NVR channel selection, append `?channel=N` to the URL.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| CGI Snapshot | `http://IP/cgi-bin/snapshot.cgi` | Basic auth required |

## Troubleshooting

### Double slash is required

Milesight cameras require a **double forward slash** before the stream path:

- Correct: `rtsp://IP:554//main`
- May not work: `rtsp://IP:554/main`

If your connection fails, verify that you are using the double-slash URL format. This pattern is similar to Pelco and ACTi cameras.

### Default password must be changed

The factory default password is `ms1234`, but Milesight cameras require this password to be changed during first login through the web interface or Milesight CMS. If the default password does not work, the camera has likely been configured with a new password.

### AI features are independent of RTSP

Milesight AI features (face detection, license plate recognition, people counting, intrusion detection) run on the camera's built-in processor and do not affect RTSP streaming. AI metadata and events are delivered through ONVIF events or the Milesight API, not through the RTSP stream itself.

### Milesight CMS is optional

Milesight CMS (Central Management Software) is Milesight's proprietary VMS. It is not required for RTSP streaming. Milesight cameras work with any ONVIF-compatible VMS or any application that supports standard RTSP connections.

### NVR channel numbering

When connecting through a Milesight MS-N NVR, channel numbers start at 1 and correspond to the physical camera input or network camera order configured in the NVR. Use `?channel=1` for the first camera, `?channel=2` for the second, and so on.

### ONVIF discovery

All current Milesight cameras support ONVIF Profile S, G, and T. If you prefer automatic discovery over manual RTSP URL configuration, use ONVIF device discovery to find cameras on your network and retrieve their streaming URLs automatically.

## FAQ

**What is the default RTSP URL for Milesight cameras?**

For all current Milesight cameras, use `rtsp://admin:ms1234@CAMERA_IP:554//main` (note the double slash). For the sub-stream, use `//sub`. For NVR access, append `?channel=N` to select the desired camera channel.

**Do all Milesight models use the same RTSP URL?**

Yes. All current Milesight camera models (MS-C, MS-A, MS-V, MS-F, MS-B series) use the same `//main` and `//sub` URL pattern. This makes Milesight one of the most consistent brands for RTSP integration.

**Does Milesight support H.265?**

Yes. All current Milesight cameras support H.264, H.265, and MJPEG encoding. H.265 can be enabled through the camera's web interface or Milesight CMS to reduce bandwidth and storage requirements.

**Why does the double slash matter in Milesight URLs?**

The double slash (`//main` instead of `/main`) is part of Milesight's RTSP URL specification. Omitting the extra slash may cause connection failures. This convention is shared with a few other camera brands (Pelco, ACTi) but is not universal across the industry.

**Can I access Milesight AI analytics through RTSP?**

No. RTSP delivers the video stream only. AI analytics results (face detection events, license plate data, people counting statistics) are accessible through ONVIF events, the Milesight HTTP API, or Milesight CMS. The video stream itself does not contain embedded AI metadata.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Grandstream Connection Guide](grandstream.md) — SMB / professional camera segment
- [IP Camera Capture to MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md) — Record Milesight streams to file
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
