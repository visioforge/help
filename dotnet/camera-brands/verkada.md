---
title: Verkada Camera RTSP and C# .NET Integration Options
description: Verkada camera integration options in C# .NET. Understand cloud-managed architecture, RTSP limitations, and alternative approaches for Verkada cameras.
meta:
  - name: robots
    content: "noindex, follow"
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Webcam
  - IP Camera
  - RTSP
  - ONVIF
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - SystemVideoSourceBlock
  - VideoRendererBlock

---

# How to Connect to Verkada Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Verkada** is an American cloud-managed security camera company headquartered in San Mateo, California. Founded in 2016, Verkada offers enterprise-grade cameras with a fully cloud-managed architecture. Unlike traditional IP cameras, Verkada cameras are managed exclusively through Verkada's cloud platform — there are no local RTSP streams, ONVIF support, or direct network access to the cameras.

**Key facts:**

- **Product lines:** CD (mini dome), CB (bullet), CE (outdoor dome), CF (fisheye), CM (multi-sensor), CP (PTZ)
- **Architecture:** Cloud-managed — all video processing and access goes through Verkada Command platform
- **RTSP support:** No
- **ONVIF support:** No
- **Local network access:** No direct access — cameras communicate only with Verkada cloud
- **Video codecs:** H.264, H.265 (managed by cloud platform)
- **API access:** Verkada API (cloud-based, requires enterprise subscription)

!!! warning "No RTSP or Local Streaming"
    Verkada cameras do **not** support RTSP, ONVIF, or any standard local streaming protocol. They are cloud-managed devices that can only be accessed through Verkada's Command platform or API. Direct integration using VisioForge SDK's RTSP source is not possible with Verkada cameras.

## Why Verkada Has No RTSP

Verkada's architecture is fundamentally different from traditional IP cameras:

1. **Cloud-first design:** Video is processed on-camera and streamed to Verkada's cloud
2. **No local network ports:** Cameras do not expose port 554 or any RTSP endpoint
3. **Managed access:** All video access goes through Verkada Command (web/mobile)
4. **Zero-trust security:** No direct camera-to-client connections on the LAN

This architecture provides simplified deployment and centralized management but eliminates direct SDK integration.

## Integration Options

### Option 1: Verkada API (Cloud-Based)

Verkada offers a REST API for enterprise customers that provides:

- Camera listing and status
- Video export/download (clips)
- Thumbnail/snapshot retrieval
- Event and alert data

The API does **not** provide live RTSP or real-time video streams. It is designed for clip retrieval and metadata access.

### Option 2: HDMI Output (Select Models)

Some Verkada models include an HDMI output port for local display. You can capture this output using an HDMI capture card:

```csharp
// Capture HDMI output from Verkada camera via USB capture card
var pipeline = new MediaBlocksPipeline();

// Use system video source (HDMI capture card appears as webcam)
var captureDevice = new SystemVideoSourceBlock(captureDeviceSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(captureDevice.Output, videoRenderer.Input);
await pipeline.StartAsync();
```

This approach provides real-time local video but requires physical HDMI connectivity and a capture card.

### Option 3: Alternative Cameras with RTSP

If you need direct RTSP integration with enterprise-grade cameras, consider these alternatives:

| Alternative | Market Segment | RTSP | ONVIF | Guide |
|------------|---------------|------|-------|-------|
| Axis | Enterprise | Yes | Yes | [Connection Guide](axis.md) |
| Bosch | Enterprise | Yes | Yes | [Connection Guide](bosch.md) |
| Hanwha Vision | Enterprise | Yes | Yes | [Connection Guide](hanwha.md) |
| Avigilon | Enterprise | Yes | Yes | [Connection Guide](avigilon.md) |
| Hikvision | Enterprise | Yes | Yes | [Connection Guide](hikvision.md) |

## FAQ

**Can I connect to Verkada cameras with RTSP?**

No. Verkada cameras do not support RTSP, ONVIF, or any local streaming protocol. They are cloud-managed devices accessible only through Verkada's Command platform or API.

**Does Verkada have an API for video access?**

Yes, but the Verkada API provides clip export and snapshot retrieval, not live video streaming. Real-time video is only available through the Verkada Command web interface or mobile app. Enterprise API access requires a Verkada subscription.

**Can I use VisioForge SDK with Verkada cameras?**

Not directly via RTSP. The only local integration option is capturing the HDMI output of select Verkada models using a capture card with VisioForge SDK's system video source. For cloud-based integration, you would need to use Verkada's API separately.

**What enterprise cameras support RTSP?**

For enterprise cameras with full RTSP and ONVIF support, see our guides for [Axis](axis.md), [Bosch](bosch.md), [Hanwha Vision](hanwha.md), [Avigilon](avigilon.md), and [Hikvision](hikvision.md).

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Axis Connection Guide](axis.md) — Enterprise alternative with RTSP
- [Bosch Connection Guide](bosch.md) — Enterprise alternative with RTSP
- [Rhombus Connection Guide](rhombus.md) — Another cloud-managed platform
- [SDK Installation & Samples](index.md#get-started)
