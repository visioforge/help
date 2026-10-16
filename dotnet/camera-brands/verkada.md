---
title: Verkada camera RTSP URL and live streaming in C# .NET
description: Verkada RTSP URL format rtsp://IP:8554/standard for C# .NET. Enable RTSP in Verkada Command, connect from Media Blocks SDK, and the model limits that apply.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - IP Camera
  - RTSP
  - C#
primary_api_classes:
  - RTSPSourceSettings

---

# How to Connect to Verkada Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Verkada** is an American cloud-managed security camera company headquartered in San Mateo, California. Founded in 2016, Verkada offers enterprise-grade cameras managed through the Verkada Command platform. Alongside the cloud management, a camera can serve RTSP streams directly on the local network once RTSP is enabled for it — the remote CR-series cameras are the exception.

**Key facts:**

- **Product lines:** CD (dome), CB (bullet), CF (fisheye), CM (mini dome), CH and CY (multisensor), CP (PTZ), CR (remote); the `-E` suffix marks an outdoor variant
- **Architecture:** Cloud-managed, with local RTSP streaming
- **RTSP support:** Yes — enabled per camera in Command
- **ONVIF support:** No
- **RTSP port:** 8554
- **Stream paths:** `/standard` and `/high`
- **Local network access:** RTSP only, over RFC 1918 private addresses
- **Video codecs:** H.264, H.265 — chosen per camera in Command
- **API access:** Verkada API (cloud-based; an Organization Admin issues the keys in Command)

## Enabling RTSP

RTSP is off until a Site Admin enables it for a given camera: **All Products → Cameras → select the camera → Settings → Device → Real Time Streaming Protocol (RTSP)**. Set a username and password, then click **Enable**. The password is shown once, so store it. Command then shows the camera's RTSP URL with a `[PASSWORD]` placeholder in it: the placeholder is yours to replace with the password you just set, because the password cannot be read back out of Command. Command can also export a CSV of every RTSP-enabled camera in the organization, with its URL.

No separate RTSP licence is required — the normal one-camera-per-licence rule applies.

## RTSP URL Format

```
rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/[STREAM]
```

| Stream | Path | Notes |
|--------|------|-------|
| Standard | `/standard` | Lower bitrate |
| High | `/high` | Full quality |

Resolution and bitrate follow the camera's native streams and cannot be changed for RTSP.

Multisensor cameras (CH and CY series) put the camera id before the stream segment, `rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/[CAMERA-ID]/high`, where the id is the one Command shows for that sensor.

## Connecting with VisioForge SDK

Put the address and the stream path in the URI, and pass the credentials from Command as `login` and `password`:

```csharp
// Verkada camera with RTSP enabled, high-quality stream
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:8554/high"),
    login: "rtsp-user",
    password: "YourRtspPassword",
    audioEnabled: true);
```

A URI with the password embedded (`rtsp://rtsp-user:YourRtspPassword@192.168.1.100:8554/high`) plays as well, because `rtspsrc` authenticates from the URL's own credentials. Passing them separately is still preferable: the URL Command shows you carries a `[PASSWORD]` placeholder to fill in, and once filled it travels through logs, configuration files and screenshots, where the password does not belong.

## Limits

- **Local network only.** The stream comes straight from the camera over private addresses and is not relayed through Verkada Cloud, so a client on another network needs a VPN into that network.
- **Unencrypted.** Verkada does not offer RTSPS. Their own guidance is to keep the camera network private and to restrict which clients can reach it.
- **The codec belongs to the camera.** The camera's own **Settings → Device → Video Codec** choice decides whether the RTSP stream is H.264 or H.265, and H.265 is not turned on for RTSP automatically. There is no H.264 fallback: a client that cannot decode H.265 sees nothing until the camera itself is set to H.264.
- **Audio is off until it is switched on.** Camera audio is disabled by default; a Site Admin enables it per camera under **Settings → Device → Camera Audio**, which reboots that camera. Until then the RTSP stream carries no audio track at all, and `CreateAsync` returns settings with `AudioEnabled` cleared.
- **Some models have no RTSP audio even so.** CD22, CD22-E, CM22, CF81-E, CF83-E, the whole CB (Bullet) series, CH52-E, CP52-E and CP63-E — pass `audioEnabled: false` for those. Of the models that do serve RTSP, the rest carry an audio track.
- **No RTSP on the CR63-E and CR63-TE.**

## Verkada API

Verkada's REST API covers camera listing and status, clip export and download, thumbnails, and event and alert data, and it streams live video as well — for a key that has been granted streaming access. An Organization Admin issues the API keys in **Command → Admin → API & Integrations**, where their permissions are set as well. Reach for the API when the video has to come from outside the camera's own network; the RTSP stream is the local, low-latency path.

## Enterprise Cameras with RTSP and ONVIF

Verkada now streams RTSP, but it still offers no ONVIF, so discovery and profile selection are unavailable. When those matter, these brands provide both:

| Alternative | Market Segment | RTSP | ONVIF | Guide |
|------------|---------------|------|-------|-------|
| Axis | Enterprise | Yes | Yes | [Connection Guide](axis.md) |
| Bosch | Enterprise | Yes | Yes | [Connection Guide](bosch.md) |
| Hanwha Vision | Enterprise | Yes | Yes | [Connection Guide](hanwha.md) |
| Avigilon | Enterprise | Yes | Yes | [Connection Guide](avigilon.md) |
| Hikvision | Enterprise | Yes | Yes | [Connection Guide](hikvision.md) |

## FAQ

**What is the RTSP URL for a Verkada camera?**

`rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/high`, where `CAMERA-IP` is the camera's private address and `[USERNAME]` and `[PASSWORD]` are the credentials set for that camera in Command. Use `/standard` in place of `/high` for the lower-bitrate stream. Multisensor cameras insert the camera id before the stream segment.

**Do Verkada cameras support RTSP?**

Yes. Verkada announced low-latency RTSP for its cameras, serving the streams over the local network. It is off by default and a Site Admin has to enable it per camera in Command.

**Do Verkada cameras support ONVIF?**

No. There is no ONVIF endpoint, so ONVIF discovery and profile selection do not apply — use the RTSP URL Command gives you.

**Can I reach a Verkada RTSP stream from outside the local network?**

Not directly. Verkada serves RTSP on private addresses only and does not relay it through the cloud, so a client on another network needs a VPN.

**Does the Verkada RTSP stream include audio?**

Not until a Site Admin enables **Camera Audio** for that camera in Command (**Settings → Device → Camera Audio**, which reboots it) — until then no audio track is present at all. And not on the models whose RTSP stream carries no audio: CD22, CD22-E, CM22, CF81-E, CF83-E, the CB series, CH52-E, CP52-E and CP63-E. Among the models that do serve RTSP, the rest carry audio, so `audioEnabled: true` applies.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Rhombus Connection Guide](rhombus.md) — Another cloud-managed platform with RTSP
- [Axis Connection Guide](axis.md) — Enterprise alternative with RTSP and ONVIF
- [SDK Installation & Samples](index.md#get-started)
