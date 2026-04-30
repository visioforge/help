---
title: IP Camera RTSP URL Directory - Connect Any Camera in C# .NET
description: Complete RTSP URL directory for 62 IP camera brands. Connect Hikvision, Dahua, Axis, Uniview, EZVIZ, Wisenet, Arlo and more using VisioForge .NET SDK.
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
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureCore
  - RTSPSourceSettings
  - IVideoView
  - IPCameraSourceSettings

---

# IP Camera Connection Guide by Brand

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Connecting to IP cameras in C# .NET is straightforward when you know the right RTSP URL pattern for your camera brand. Each manufacturer uses slightly different URL formats, ports, and authentication methods.

This directory provides **brand-specific RTSP URL patterns**, connection code samples using VisioForge SDK, and troubleshooting tips for the most popular IP camera manufacturers.

## How RTSP Camera Connections Work

Most modern IP cameras expose video streams via the **RTSP (Real-Time Streaming Protocol)** on port 554. The general connection flow is:

1. Determine your camera's IP address (via ONVIF discovery, DHCP lease table, or manufacturer utility)
2. Construct the RTSP URL using the brand-specific pattern
3. Authenticate with camera credentials
4. Connect and render the video stream

### Quick Start Code

Connect to any RTSP camera using one of three VisioForge SDK approaches:

=== "VideoCaptureCoreX"

    ```csharp
    // Initialize SDK (call once at app startup)
    await VisioForgeX.InitSDKAsync();

    var videoCapture = new VideoCaptureCoreX(VideoView1);

    // Create RTSP source
    var rtsp = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream1"),
        "admin",
        "password",
        true); // capture audio

    videoCapture.Video_Source = rtsp;

    await videoCapture.StartAsync();
    ```

=== "VideoCaptureCore"

    ```csharp
    var videoCapture = new VideoCaptureCore(VideoView1 as IVideoView);

    videoCapture.IP_Camera_Source = new IPCameraSourceSettings()
    {
        URL = new Uri("rtsp://admin:password@192.168.1.100:554/stream1"),
        Type = IPSourceEngine.Auto_LAV
    };

    videoCapture.Audio_PlayAudio = true;
    videoCapture.Audio_RecordAudio = false;
    videoCapture.Mode = VideoCaptureMode.IPPreview;

    await videoCapture.StartAsync();
    ```

=== "Media Blocks"

    ```csharp
    var pipeline = new MediaBlocksPipeline();

    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream1"),
        "admin",
        "password",
        audioEnabled: true);

    rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;

    var rtspSource = new RTSPSourceBlock(rtspSettings);
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    var audioRenderer = new AudioRendererBlock();

    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);

    await pipeline.StartAsync();
    ```

Replace the RTSP URL with your brand-specific pattern from the pages below.

### Which SDK Should I Choose?

| SDK | Best For | Platforms |
|-----|----------|-----------|
| **VideoCaptureCoreX** | New cross-platform projects, modern .NET | Windows, macOS, Linux, Android, iOS |
| **VideoCaptureCore** | Windows-only projects, legacy .NET Framework | Windows |
| **Media Blocks** | Advanced pipelines, custom processing chains | Windows, macOS, Linux, Android, iOS |

**VideoCaptureCoreX** is recommended for most new projects. Use **Media Blocks** when you need to build custom processing pipelines with multiple sources, filters, or outputs.

## Camera Brands

### Featured Brands (Full Guides)

| Brand | Headquarters | Market Segment | Guide |
|-------|-------------|----------------|-------|
| **Hikvision** | Hangzhou, China | Enterprise / Consumer | [Connection Guide](hikvision.md) |
| **Dahua** | Hangzhou, China | Enterprise / Consumer | [Connection Guide](dahua.md) |
| **Axis** | Lund, Sweden | Enterprise / Professional | [Connection Guide](axis.md) |
| **Reolink** | Hong Kong | Consumer / Prosumer | [Connection Guide](reolink.md) |
| **Amcrest** | Houston, USA | Consumer / SMB | [Connection Guide](amcrest.md) |
| **Samsung/Hanwha** | Grasbrunn, Germany / Seoul, South Korea | Enterprise / Professional | [Connection Guide](samsung.md) |
| **Bosch** | Grasbrunn, Germany | Enterprise / Critical Infrastructure | [Connection Guide](bosch.md) |
| **Ubiquiti** | New York, USA | Prosumer / SMB | [Connection Guide](ubiquiti.md) |
| **Foscam** | Shenzhen, China | Consumer / SMB | [Connection Guide](foscam.md) |
| **TP-Link** | Shenzhen, China | Consumer / SMB | [Connection Guide](tp-link.md) |
| **Vivotek** | New Taipei City, Taiwan | Enterprise / Professional | [Connection Guide](vivotek.md) |
| **Panasonic/i-PRO** | Tokyo, Japan | Enterprise / Government | [Connection Guide](panasonic.md) |
| **Sony** | Tokyo, Japan | Enterprise (discontinued 2020) | [Connection Guide](sony.md) |
| **Lorex** | Markham, Canada | Consumer / Prosumer | [Connection Guide](lorex.md) |
| **D-Link** | Taipei, Taiwan | Consumer / SMB | [Connection Guide](dlink.md) |
| **Honeywell** | Charlotte, USA | Enterprise / Commercial | [Connection Guide](honeywell.md) |
| **Pelco** | Fresno, USA (Motorola Solutions) | Enterprise / Government | [Connection Guide](pelco.md) |
| **Cisco** | San Jose, USA | Enterprise / Consumer-SMB (legacy) | [Connection Guide](cisco.md) |
| **Grandstream** | Boston, USA | SMB / Professional | [Connection Guide](grandstream.md) |
| **Swann** | Melbourne, Australia | Consumer / Prosumer | [Connection Guide](swann.md) |
| **GeoVision** | Taipei, Taiwan | Enterprise / Professional | [Connection Guide](geovision.md) |
| **ACTi** | Taipei, Taiwan | Professional / Enterprise | [Connection Guide](acti.md) |
| **Canon** | Tokyo, Japan | Professional / Enterprise | [Connection Guide](canon.md) |
| **FLIR (Teledyne)** | Wilsonville, USA | Enterprise / Thermal | [Connection Guide](flir.md) |
| **Milesight** | Xiamen, China | Professional / SMB | [Connection Guide](milesight.md) |
| **INSTAR** | Hanau, Germany | Consumer / Smart Home | [Connection Guide](instar.md) |
| **Zmodo** | Shenzhen, China | Consumer / Budget | [Connection Guide](zmodo.md) |
| **Arecont Vision** | Glendale, USA (Costar Group) | Professional / Enterprise | [Connection Guide](arecont.md) |
| **JVC** | Yokohama, Japan | Professional (discontinued ~2015) | [Connection Guide](jvc.md) |
| **Toshiba** | Tokyo, Japan | Enterprise (discontinued) | [Connection Guide](toshiba.md) |
| **LG** | Seoul, South Korea | Enterprise (discontinued) | [Connection Guide](lg.md) |
| **Linksys** | Irvine, USA | Consumer (discontinued ~2014) | [Connection Guide](linksys.md) |
| **LTS** | City of Industry, USA | Professional (Hikvision OEM) | [Connection Guide](lts.md) |
| **Q-See** | Anaheim, USA | Consumer (defunct ~2020) | [Connection Guide](q-see.md) |
| **Speco Technologies** | Amityville, USA | Professional | [Connection Guide](speco.md) |
| **EverFocus** | New Taipei City, Taiwan | Professional | [Connection Guide](everfocus.md) |
| **ABUS** | Wetter, Germany | Consumer / Professional | [Connection Guide](abus.md) |
| **Basler** | Ahrensburg, Germany | Machine Vision / Industrial | [Connection Guide](basler.md) |
| **Mobotix** | Langmeil, Germany (Konica Minolta) | Industrial / Critical Infrastructure | [Connection Guide](mobotix.md) |
| **Avigilon** | Vancouver, Canada (Motorola Solutions) | Enterprise / Critical Infrastructure | [Connection Guide](avigilon.md) |
| **AVTech** | Taipei, Taiwan | Commercial / Industrial | [Connection Guide](avtech.md) |
| **LILIN** | New Taipei City, Taiwan | Professional / Enterprise | [Connection Guide](lilin.md) |
| **Zavio** | Hsinchu, Taiwan | Professional / SMB | [Connection Guide](zavio.md) |
| **CP Plus** | Delhi, India | Enterprise / Commercial | [Connection Guide](cp-plus.md) |
| **Sanyo** | Osaka, Japan (now Panasonic) | Professional (discontinued) | [Connection Guide](sanyo.md) |
| **BrickCom** | Taipei, Taiwan | Professional / Industrial | [Connection Guide](brickcom.md) |
| **Edimax** | Taipei, Taiwan | Consumer / SMB | [Connection Guide](edimax.md) |
| **Uniview (UNV)** | Hangzhou, China | Enterprise / Government | [Connection Guide](uniview.md) |
| **Hanwha Vision** | Seoul, South Korea | Enterprise / Professional | [Connection Guide](hanwha.md) |
| **Tiandy** | Tianjin, China | Enterprise / SMB | [Connection Guide](tiandy.md) |
| **EZVIZ** | Hangzhou, China (Hikvision) | Consumer / Smart Home | [Connection Guide](ezviz.md) |
| **Wisenet** | Seoul, South Korea (Hanwha Vision) | Enterprise / Professional | [Connection Guide](wisenet.md) |
| **Annke** | Hong Kong | Consumer / Prosumer | [Connection Guide](annke.md) |
| **Imou** | Hangzhou, China (Dahua) | Consumer / Smart Home | [Connection Guide](imou.md) |
| **Wyze** | Kirkland, USA | Consumer (limited RTSP) | [Connection Guide](wyze.md) |
| **Aqara** | Shenzhen, China | Smart Home / HomeKit | [Connection Guide](aqara.md) |
| **Verkada** | San Mateo, USA | Enterprise / Cloud-managed | [Connection Guide](verkada.md) |
| **Rhombus** | Sacramento, USA | Enterprise / Cloud-managed | [Connection Guide](rhombus.md) |
| **Arlo** | Carlsbad, USA | Consumer (no RTSP) | [Connection Guide](arlo.md) |
| **Eufy Security** | Changsha, China (Anker) | Consumer / Smart Home | [Connection Guide](eufy.md) |
| **Tenda** | Shenzhen, China | Consumer / Budget | [Connection Guide](tenda.md) |
| **Mercusys** | Shenzhen, China (TP-Link) | Consumer / Budget | [Connection Guide](mercusys.md) |

### Common RTSP URL Patterns by Brand

For quick reference, here are the primary RTSP URL patterns for popular camera brands:

| Brand | Primary RTSP URL Pattern | Default Port |
|-------|--------------------------|-------------|
| Hikvision | `rtsp://IP:554/Streaming/Channels/101` | 554 |
| Dahua | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Axis | `rtsp://IP:554/axis-media/media.amp` | 554 |
| Foscam | `rtsp://IP:88/videoMain` | 88 |
| TP-Link (Tapo) | `rtsp://IP:554/stream1` | 554 |
| Amcrest | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Reolink | `rtsp://IP:554/h264Preview_01_main` | 554 |
| Ubiquiti | `rtsp://IP:7447/STREAM_TOKEN` | 7447 |
| Samsung/Hanwha | `rtsp://IP:554/profile2/media.smp` | 554 |
| Bosch | `rtsp://IP:554/video?inst=1` | 554 |
| Vivotek | `rtsp://IP:554/live.sdp` | 554 |
| Panasonic/i-PRO | `rtsp://IP:554/MediaInput/h264` | 554 |
| Sony | `rtsp://IP:554/media/video1` | 554 |
| Lorex | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| D-Link | `rtsp://IP:554/live1.sdp` | 554 |
| Honeywell | `rtsp://IP:554/h264` | 554 |
| Pelco | `rtsp://IP:554//stream1` | 554 |
| Cisco | `rtsp://IP:554/img/media.sav` | 554 |
| Grandstream | `rtsp://IP:554/live/ch00_0` | 554 |
| Swann | `rtsp://IP:554/live/h264` | 554 |
| GeoVision | `rtsp://IP:8554//CH001.sdp` | 8554 |
| ACTi | `rtsp://IP:7070//stream1` | 7070 |
| Canon | `rtsp://IP:554/cam1/h264` | 554 |
| FLIR (Teledyne) | `rtsp://IP:554/ch0` | 554 |
| Milesight | `rtsp://IP:554//main` | 554 |
| INSTAR | `rtsp://IP:554//11` | 554 |
| Zmodo | `rtsp://IP:10554//tcp/av0_0` | 10554 |
| Arecont Vision | `rtsp://IP:554/h264.sdp` | 554 |
| JVC | `rtsp://IP:554/PSIA/Streaming/channels/0` | 554 |
| Toshiba | `rtsp://IP:554/live.sdp` | 554 |
| LG | `rtsp://IP:554/video1+audio1` | 554 |
| Linksys | `rtsp://IP:554/img/media.sav` | 554 |
| LTS | `rtsp://IP:554//Streaming/Channels/1` | 554 |
| Q-See | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | 554 |
| Speco | `rtsp://IP:554/1/stream1` | 554 |
| EverFocus | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | 554 |
| ABUS | `rtsp://IP:554/video.mp4` | 554 |
| Basler | `rtsp://IP:554/h264` | 554 |
| Mobotix | `rtsp://IP:554/mobotix.h264` | 554 |
| Avigilon | `rtsp://IP:554/defaultPrimary?streamType=u` | 554 |
| AVTech | `rtsp://IP:554/live/h264` | 554 |
| LILIN | `rtsp://IP:554/rtsph2641080p` | 554 |
| Zavio | `rtsp://IP:554/video.mp4` | 554 |
| CP Plus | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | 554 |
| Sanyo | `rtsp://IP:554/VideoInput/1/h264/1` | 554 |
| BrickCom | `rtsp://IP:554/channel1` | 554 |
| Edimax | `rtsp://IP:554/ipcam_h264.sdp` | 554 |
| Uniview (UNV) | `rtsp://IP:554/media/video1` | 554 |
| Hanwha Vision | `rtsp://IP:554/profile2/media.smp` | 554 |
| Tiandy | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| EZVIZ | `rtsp://IP:554/h264/ch1/main/av_stream` | 554 |
| Wisenet | `rtsp://IP:554/profile2/media.smp` | 554 |
| Annke | `rtsp://IP:554/Streaming/Channels/101` | 554 |
| Imou | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Wyze | `rtsp://IP:8554/live` | 8554 |
| Aqara | `rtsp://IP:554/live/ch00_1` | 554 |
| Verkada | N/A (cloud-only) | N/A |
| Rhombus | `rtsp://IP:554/live` (if enabled) | 554 |
| Arlo | N/A (no RTSP) | N/A |
| Eufy Security | `rtsp://IP:554/live0` | 554 |
| Tenda | `rtsp://IP:554/stream1` | 554 |
| Mercusys | `rtsp://IP:554/stream1` | 554 |

## ONVIF Discovery

Most modern IP cameras support **ONVIF (Open Network Video Interface Forum)**, which allows automatic camera discovery on your network. VisioForge SDK supports ONVIF discovery -- see our [ONVIF integration guide](../mediablocks/Sources/index.md) for details.

## Get Started

### Install via NuGet

=== "Cross-platform (recommended)"

    ```bash
    dotnet add package VisioForge.CrossPlatform.Core
    ```

=== "Windows-only"

    ```bash
    dotnet add package VisioForge.DotNet.Core
    dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
    ```

### Sample Projects

Complete working examples for IP camera integration:

- [IP Camera Preview (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-preview) — Live camera view
- [IP Camera Recording to MP4](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-capture-mp4) — Record streams to file
- [All .NET SDK Samples](https://github.com/visioforge/.Net-SDK-s-samples) — Full sample repository

## Related Resources

- [RTSP Source Block Documentation](../mediablocks/Sources/index.md)
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [IP Camera Recording to MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md)
- [Building Camera Applications with Media Blocks](../mediablocks/GettingStarted/camera.md)
- [Device Enumeration Guide](../mediablocks/GettingStarted/device-enum.md)
