---
title: Rhombus Camera RTSP URL and Cloud API C# Integration
description: Rhombus camera integration options in C# .NET. Cloud-managed architecture, API access, and alternative approaches for Rhombus Systems cameras.
meta:
  - name: robots
    content: "noindex, follow"
---

# How to Connect to Rhombus Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Rhombus Systems** (Rhombus, Inc.) is an American cloud-managed video security company headquartered in Sacramento, California. Founded in 2016, Rhombus provides enterprise cameras, sensors, and access control with a cloud-first management platform. Similar to Verkada, Rhombus cameras are managed through a centralized cloud console.

**Key facts:**

- **Product lines:** R-series (dome), R-series Pro (advanced), R-series Mini (compact)
- **Architecture:** Cloud-managed with on-camera AI processing
- **RTSP support:** Limited — available on some models via LAN configuration
- **ONVIF support:** No
- **Video codecs:** H.264, H.265
- **API access:** Rhombus API (REST, requires subscription)
- **On-camera storage:** Yes (local SD card for edge storage)

!!! info "Limited Local Streaming"
    Some Rhombus camera models support RTSP for local LAN streaming, but this feature must be enabled through the Rhombus console and is not available on all models or subscription tiers. Check your Rhombus account settings for RTSP availability.

## RTSP Access (Where Available)

### Enabling RTSP

On supported Rhombus cameras:

1. Log in to the **Rhombus Console** (console.rhombus.com)
2. Navigate to your camera's settings
3. Look for **Local Streaming** or **RTSP** settings
4. Enable RTSP and note the provided URL

### RTSP URL Format

When RTSP is available:

```
rtsp://[IP]:554/live
```

The exact URL format and authentication method depend on the camera model and firmware version. The Rhombus console will provide the specific URL when RTSP is enabled.

## Connecting with VisioForge SDK

If RTSP is available on your Rhombus camera, use the URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Rhombus camera with RTSP enabled
var uri = new Uri("rtsp://192.168.1.90:554/live");
var username = "admin";
var password = "YourPassword"; // from Rhombus console
```

## Integration via Rhombus API

For cameras without RTSP access, Rhombus provides a REST API that offers:

- Camera listing and status
- Video clip export and download
- Snapshot/thumbnail retrieval
- Event and analytics data
- Webhook notifications

The API does not provide real-time RTSP streams. It is designed for clip retrieval, metadata access, and automation workflows.

## Troubleshooting

### RTSP not available on my camera

Not all Rhombus cameras or subscription tiers support RTSP. Contact Rhombus support to verify RTSP availability for your specific camera model and plan.

### RTSP stream disconnects

Rhombus cameras prioritize cloud connectivity. If the local RTSP stream is unstable:

1. Ensure the camera has sufficient network bandwidth for both cloud and local streams
2. Use the sub stream for lower bandwidth requirements
3. Check the Rhombus console for firmware updates

## FAQ

**Do Rhombus cameras support RTSP?**

Some Rhombus cameras support RTSP for local LAN streaming, but it must be enabled through the Rhombus console and may not be available on all models or subscription tiers. Contact Rhombus support for specifics.

**Can I use VisioForge SDK with Rhombus cameras?**

If your Rhombus camera has RTSP enabled, yes. Use the RTSP URL from the Rhombus console with VisioForge SDK's RTSP source. For cameras without RTSP, you would need to use the Rhombus REST API separately for clip and snapshot access.

**How does Rhombus compare to Verkada?**

Both are cloud-managed platforms. Rhombus offers RTSP on some models, while Verkada does not support RTSP at all. Both provide REST APIs for clip/snapshot access. See our [Verkada guide](verkada.md) for comparison.

**What are good alternatives to Rhombus with full RTSP support?**

For enterprise cameras with native RTSP and ONVIF support, consider [Axis](axis.md), [Bosch](bosch.md), [Hanwha Vision](hanwha.md), or [Avigilon](avigilon.md).

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Verkada Connection Guide](verkada.md) — Another cloud-managed platform
- [Axis Connection Guide](axis.md) — Enterprise alternative with full RTSP
- [Hanwha Vision Connection Guide](hanwha.md) — Enterprise alternative with RTSP
- [SDK Installation & Samples](index.md#get-started)
