---
title: How to Connect to Reolink IP Camera in C# .NET
description: Connect to Reolink cameras in C# .NET with RTSP URL patterns and code samples for RLC, E1, Argus, CX, and Duo series models.
---

# How to Connect to Reolink IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Reolink** (Reolink Digital Technology Co., Ltd.) is a consumer and prosumer IP camera manufacturer headquartered in Hong Kong. Founded in 2009, Reolink has grown rapidly through direct-to-consumer sales on Amazon and their own website, offering competitively priced cameras with straightforward RTSP access. Reolink is notable for clear documentation of RTSP URLs and easy integration with third-party software.

**Key facts:**

- **Product lines:** RLC series (PoE wired), RLN series (NVRs), E1 series (Wi-Fi pan/tilt), Argus series (battery/solar), CX series (ColorX night vision), Duo series (dual-lens), TrackMix (auto-tracking)
- **Protocol support:** RTSP, ONVIF, HTTP/HTTPS, proprietary Reolink protocol
- **Default RTSP port:** 554
- **Default credentials:** admin / (blank password or set during setup)
- **ONVIF support:** Yes (most current models, may require enabling in camera settings)
- **Video codecs:** H.264 (all models), H.265 (most current models)

## RTSP URL Patterns

Reolink uses a consistent URL pattern across most models. The main difference is between cameras and NVRs (which use channel numbers).

### Camera RTSP URLs

| Stream | RTSP URL | Resolution | Notes |
|--------|----------|------------|-------|
| Main (clear) | `rtsp://IP:554/h264Preview_01_main` | Full (up to 4K/8MP) | H.264 main stream |
| Sub (fluent) | `rtsp://IP:554/h264Preview_01_sub` | Reduced (640x360) | Lower bandwidth |

!!! info "H.265 streams"
    For cameras with H.265 enabled, the URL remains the same (`h264Preview_01_main`). The `h264` in the URL path is not codec-specific -- it works for both H.264 and H.265 streams.

### NVR Channel URLs

For Reolink NVRs (RLN8-410, RLN16-410, RLN36, etc.), append the channel number:

| Channel | Main Stream URL | Sub Stream URL |
|---------|----------------|----------------|
| Channel 1 | `rtsp://IP:554/h264Preview_01_main` | `rtsp://IP:554/h264Preview_01_sub` |
| Channel 2 | `rtsp://IP:554/h264Preview_02_main` | `rtsp://IP:554/h264Preview_02_sub` |
| Channel 3 | `rtsp://IP:554/h264Preview_03_main` | `rtsp://IP:554/h264Preview_03_sub` |
| Channel N | `rtsp://IP:554/h264Preview_0N_main` | `rtsp://IP:554/h264Preview_0N_sub` |

### Models and Resolutions

| Model | Resolution | Codec | Wi-Fi | RTSP |
|-------|-----------|-------|-------|------|
| RLC-410 | 2560x1440 (4MP) | H.264/H.265 | No (PoE) | Yes |
| RLC-510A | 2560x1920 (5MP) | H.264/H.265 | No (PoE) | Yes |
| RLC-520A | 2560x1920 (5MP) | H.264/H.265 | No (PoE) | Yes |
| RLC-810A | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Yes |
| RLC-811A | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Yes |
| RLC-820A | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Yes |
| RLC-1212A | 4512x2512 (12MP) | H.264/H.265 | No (PoE) | Yes |
| E1 Zoom | 2560x1920 (5MP) | H.264/H.265 | Yes | Yes |
| E1 Pro | 2560x1440 (4MP) | H.264 | Yes | Yes |
| E1 Outdoor | 2560x1920 (5MP) | H.264/H.265 | Yes | Yes |
| CX410 | 2560x1440 (4MP) | H.264/H.265 | No (PoE) | Yes |
| CX810 | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Yes |
| TrackMix PoE | 3840x2160 (8MP) | H.264/H.265 | No (PoE) | Yes |
| Duo 2 PoE | 4608x1728 (8MP) | H.264/H.265 | No (PoE) | Yes |
| Argus 3 Pro | 2560x1440 (4MP) | H.264 | Yes (battery) | Yes |

!!! warning "Argus battery cameras"
    Argus series battery-powered cameras support RTSP but drain the battery quickly when streaming continuously. Use RTSP only for testing or event-triggered recording, not 24/7 monitoring.

## Connecting with VisioForge SDK

Use your Reolink camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Reolink RLC-810A, main stream
var uri = new Uri("rtsp://192.168.1.88:554/h264Preview_01_main");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `h264Preview_01_sub` instead of `h264Preview_01_main`. For NVR channels, change the channel number (e.g., `h264Preview_03_main` for channel 3).

## Snapshot URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/api.cgi?cmd=Snap&channel=0&rs=abc123&user=USER&password=PASS` | API-based snapshot |

## Troubleshooting

### RTSP not working -- "Connection refused"

RTSP may need to be enabled on some Reolink models:

1. Open the **Reolink app** or web interface
2. Go to **Settings > Network > Advanced > Port Settings**
3. Ensure **RTSP port** is enabled and set to 554
4. Some older firmware versions have RTSP disabled by default

### H.265 stream not decoding

If your Reolink camera is configured for H.265 and the stream fails to decode:

- The SDK supports H.265 natively, but ensure you're using a recent SDK version
- Try switching the camera to H.264 in **Settings > Display > Encode** as a workaround
- The RTSP URL path (`h264Preview`) remains the same regardless of the actual codec

### Sub stream shows low quality

The sub stream (`h264Preview_01_sub`) is intentionally lower resolution (typically 640x360) to reduce bandwidth. Use `h264Preview_01_main` for full resolution. You can adjust sub stream quality in the Reolink app under Display settings.

### NVR channel numbering

Reolink NVR channels are 1-indexed with zero-padded two-digit format: `01`, `02`, `03`... `16`. Channel 0 does not exist.

## FAQ

**What is the default RTSP URL for Reolink cameras?**

The URL is `rtsp://admin:password@CAMERA_IP:554/h264Preview_01_main` for the main stream and `h264Preview_01_sub` for the sub stream. The password is whatever you set during camera setup.

**Does the RTSP URL change when using H.265?**

No. The URL path `h264Preview_01_main` is used for both H.264 and H.265 streams. The `h264` in the path is a legacy naming convention, not a codec selector.

**Can I access Reolink cameras remotely via RTSP?**

RTSP is designed for local network access. For remote access, you would need to set up port forwarding on your router (forward port 554 to the camera's IP) or use a VPN. Reolink's cloud/P2P access uses a proprietary protocol, not RTSP.

**Do Reolink Duo cameras have separate RTSP streams for each lens?**

Yes. Reolink Duo cameras expose the wide-angle lens on the standard channel and may provide additional streams. Check your specific model's documentation for dual-lens stream access.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Amcrest Connection Guide](amcrest.md) — Consumer alternative with RTSP
- [TP-Link Connection Guide](tp-link.md) — Budget cameras with native RTSP
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
