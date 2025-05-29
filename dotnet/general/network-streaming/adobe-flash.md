---
title: Network Video Streaming to Flash Media Server
description: Learn how to implement network video streaming to Adobe Flash Media Server in .NET applications. Tutorial covers real-time effects, quality settings, and device switching for professional video streaming solutions.
sidebar_label: Adobe Flash Media Server

---

# Streaming to Adobe Flash Media Server: Advanced Implementation Guide

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) 

## Introduction

Adobe Flash Media Server (FMS) remains a powerful solution for streaming video content across various platforms. This guide demonstrates how to implement high-quality video streaming to Adobe Flash Media Server using VisioForge's .NET SDKs. The integration supports real-time video effects, quality adjustment, and seamless device switching during streaming sessions.

## Prerequisites

Before implementing the streaming functionality, ensure you have:

- VisioForge Video Capture SDK .NET or Video Edit SDK .NET installed
- Adobe Flash Media Server (or a compatible service like Wowza with RTMP support)
- Adobe Flash Media Live Encoder (FMLE)
- .NET Framework 4.7.2 or later
- Visual Studio 2022 or newer
- Basic understanding of C# programming

## Demo Application Walkthrough

The demo application provided with VisioForge SDKs offers a straightforward way to test streaming functionality. Here's a detailed walkthrough:

1. Start the Main Demo application
2. Navigate to the "Network Streaming" tab
3. Enable streaming by selecting the "Enabled" checkbox
4. Select the "External" radio button for external encoder compatibility
5. Start preview or capture to initialize the video stream
6. Open Adobe Flash Media Live Encoder
7. Configure FMLE to use "VisioForge Network Source" as the video source
8. Configure video parameters:
   - Resolution (e.g., 1280x720, 1920x1080)
   - Frame rate (typically 25-30 fps for smooth streaming)
   - Keyframe interval (recommend 2 seconds)
   - Video quality settings
9. Select "VisioForge Network Source Audio" as the audio source
10. Configure your connection to Adobe Flash Media Server
11. Press Start to initiate streaming

The video from the SDK is now being streamed to your FMS instance. You can apply real-time effects, adjust settings, or even stop the SDK to switch input devices without terminating the streaming session on the server side.

## Implementation in Custom Applications

### Required Components

To implement this functionality in your custom application, you'll need:

- SDK redistributables (available in the SDK installation package)
- References to the VisioForge SDK assemblies
- Proper firewall and network configurations to allow streaming

## Required Redistributables

Ensure the following components are included with your application:

- VisioForge SDK redistributable packages
- Microsoft Visual C++ Runtime (appropriate version for your SDK)
- .NET Framework runtime (if not using self-contained deployment)

## Conclusion

Streaming to Adobe Flash Media Server using VisioForge's Video Capture or Edit SDKs offers a flexible and powerful solution for implementing high-quality video streaming in .NET applications. The implementation supports real-time effects, quality adjustments, and seamless device switching, making it suitable for a wide range of streaming applications.

By following this guide, developers can implement robust streaming solutions that leverage the powerful features of both the VisioForge SDKs and Adobe's streaming platform.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and example projects.
