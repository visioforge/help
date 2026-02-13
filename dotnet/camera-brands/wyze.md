---
title: How to Connect to Wyze Camera in C# .NET - RTSP Workarounds
description: Connect to Wyze cameras in C# .NET using RTSP firmware or Docker RTSP bridge. RTSP limitations, workarounds, and alternative approaches explained.
meta:
  - name: robots
    content: "noindex, follow"
---

# How to Connect to Wyze Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Wyze Labs** is an American consumer electronics company based in Kirkland, Washington. Known for extremely affordable smart home cameras, Wyze became one of the best-selling camera brands in North America. However, Wyze cameras are **cloud-first devices** with very limited RTSP support, making direct integration challenging.

**Key facts:**

- **Product lines:** Cam v3/v4 (indoor/outdoor), Cam Pan v3 (PTZ), Cam OG (compact), Doorbell, Floodlight
- **Protocol support:** Wyze Cloud (primary), RTSP (limited — requires special firmware on select models)
- **Default RTSP port:** 8554 (when using RTSP firmware)
- **ONVIF support:** No
- **Video codecs:** H.264
- **Cloud dependency:** High — most features require Wyze app and cloud

!!! warning "Very Limited RTSP Support"
    Wyze cameras do **not** natively support RTSP. Official RTSP firmware was released only for the **Wyze Cam v2** and original **Wyze Cam Pan**, and these firmware builds are no longer actively maintained. For newer models (v3, v4, OG, Pan v3), RTSP requires third-party solutions like custom firmware.

## RTSP Support by Model

| Model | Native RTSP | RTSP Firmware | Third-Party RTSP | Notes |
|-------|------------|--------------|-----------------|-------|
| Wyze Cam v2 | No | Yes (beta) | Yes | Official RTSP firmware available |
| Wyze Cam Pan v1 | No | Yes (beta) | Yes | Official RTSP firmware available |
| Wyze Cam v3 | No | No | Yes (docker-wyze-bridge) | Community workaround |
| Wyze Cam v4 | No | No | Yes (docker-wyze-bridge) | Community workaround |
| Wyze Cam Pan v3 | No | No | Yes (docker-wyze-bridge) | Community workaround |
| Wyze Cam OG | No | No | Yes (docker-wyze-bridge) | Community workaround |
| Wyze Doorbell v2 | No | No | Limited | May work with bridge |
| Wyze Cam Floodlight v2 | No | No | Yes (docker-wyze-bridge) | Community workaround |

## Option 1: Official RTSP Firmware (Cam v2 / Pan v1 Only)

Wyze released beta RTSP firmware for the Cam v2 and original Cam Pan. When flashed:

### RTSP URL Format

```
rtsp://[IP]:8554/live
```

!!! note "Non-Standard Port"
    Wyze RTSP firmware uses port **8554**, not the standard 554.

### Setup Steps

1. Download the RTSP firmware from Wyze support (search "Wyze RTSP firmware")
2. Flash the firmware to the camera via microSD card
3. In the Wyze app, go to camera settings and enable RTSP
4. Note the RTSP URL shown in the app (usually `rtsp://CAMERA_IP:8554/live`)

### Connecting with VisioForge SDK

```csharp
// Wyze Cam v2 with RTSP firmware
var uri = new Uri("rtsp://192.168.1.90:8554/live");
var username = ""; // no authentication on Wyze RTSP firmware
var password = "";
```

Use your Wyze camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code).

## Option 2: Docker Wyze Bridge (All Models)

For Wyze Cam v3, v4, Pan v3, OG, and other newer models, the community-developed **docker-wyze-bridge** project creates an RTSP proxy that converts Wyze cloud streams to local RTSP:

### How It Works

1. Docker Wyze Bridge authenticates with your Wyze account
2. It connects to the camera through the Wyze cloud API
3. It re-streams the video as a local RTSP stream
4. Your application connects to the bridge, not directly to the camera

### RTSP URL Format (via Bridge)

```
rtsp://[BRIDGE_IP]:8554/[CAMERA_NAME]
```

Where `CAMERA_NAME` is the name you assigned in the Wyze app (spaces replaced with dashes, lowercased).

### Connecting via Bridge with VisioForge SDK

```csharp
// Wyze Cam v3 via docker-wyze-bridge
var uri = new Uri("rtsp://192.168.1.50:8554/front-door");
var username = ""; // bridge handles auth
var password = "";
```

!!! warning "Bridge Limitations"
    Docker Wyze Bridge introduces additional latency (typically 3-10 seconds) since the video passes through the Wyze cloud before reaching your local RTSP stream. It also requires your Wyze account credentials and an active internet connection.

## Troubleshooting

### No RTSP option in Wyze app

The RTSP toggle only appears on Wyze Cam v2 and Pan v1 when flashed with the RTSP firmware. It is not available on newer models. For v3/v4/OG/Pan v3, use the Docker Wyze Bridge approach.

### RTSP firmware not connecting

After flashing RTSP firmware on the Cam v2:

1. Wait 2-3 minutes for the camera to fully boot
2. Verify the camera is on the same network as your application
3. Try `rtsp://CAMERA_IP:8554/live` in VLC first to confirm the stream works
4. The stream has no authentication -- leave username/password empty

### High latency with Docker Wyze Bridge

The bridge routes video through Wyze's cloud servers, adding latency. For low-latency requirements, Wyze cameras may not be suitable. Consider cameras with native RTSP support like [Reolink](reolink.md), [Amcrest](amcrest.md), or [TP-Link Tapo](tp-link.md).

### Stream quality

Wyze cameras typically output 1080p H.264 streams. The RTSP firmware does not support changing resolution or codec. What the camera captures is what the RTSP stream provides.

## FAQ

**Do Wyze cameras support RTSP natively?**

No. Wyze cameras are cloud-first devices. The Wyze Cam v2 and original Cam Pan have official beta RTSP firmware, but it is no longer actively maintained. Newer models (v3, v4, OG, Pan v3) do not have RTSP firmware and require third-party bridges.

**Can I use Wyze cameras without the cloud?**

Very limited. Even with RTSP firmware, the initial setup requires the Wyze app and cloud account. The RTSP firmware for Cam v2/Pan v1 disables some cloud features. For newer models, the Docker Wyze Bridge still routes through the cloud.

**What cameras should I use instead of Wyze for RTSP?**

For affordable cameras with native RTSP support, consider [Reolink](reolink.md) (consumer), [Amcrest](amcrest.md) (consumer/SMB), [TP-Link Tapo](tp-link.md) (consumer), or [EZVIZ](ezviz.md) (smart home). All of these provide direct RTSP access without workarounds.

**Do Wyze cameras support ONVIF?**

No. Wyze cameras do not support ONVIF in any firmware version.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Reolink Connection Guide](reolink.md) — Affordable alternative with native RTSP
- [Amcrest Connection Guide](amcrest.md) — Consumer cameras with full RTSP
- [TP-Link Connection Guide](tp-link.md) — Budget cameras with RTSP support
- [SDK Installation & Samples](index.md#get-started)
