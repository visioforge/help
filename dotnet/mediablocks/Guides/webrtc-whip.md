---
title: Screen or Camera to a Browser over WebRTC in C# .NET
description: Stream a screen, window or camera from a C# app to web browsers over WebRTC with no media server, and publish it to MediaMTX or Janus with WHIP.
sidebar_label: Screen and Camera to Browsers (WebRTC, WHIP)
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - iOS
  - Streaming
  - Screen Capture
  - Webcam
  - WebRTC
  - WHIP
  - VP8
  - H.264
  - Opus
  - C#
  - JavaScript
  - NuGet
primary_api_classes:
  - WebRTCSinkBlock
  - WebRTCSinkSettings
  - WHIPSinkBlock
  - WHIPSinkSettings
  - ScreenSourceBlock
  - SystemVideoSourceBlock
  - MediaBlocksPipeline

---

# Stream a Screen or Camera to Web Browsers over WebRTC, and Publish with WHIP, in C# / .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Demo Samples"
    The browser part of this guide follows the WebRTC Browser Streamer Demo (WPF): a camera and a microphone, a local preview, and a viewer page served by the application. The WHIP part follows the [WebRTC WHIP Streamer Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/WebRTC%20WHIP%20Streamer%20Demo) and its [Avalonia version](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Avalonia/WebRTC%20WHIP%20Streamer) for Windows, macOS and Linux.

    The block reference is in [Sink blocks → WebRTC](../Sinks/index.md#webrtc) and [Sink blocks → WHIP](../Sinks/index.md#whip).

## What You Will Build

Two ways to put a live screen, an application window or a camera in front of people who only have a web browser, both with sub-second latency:

1. **Straight to the browsers, no server.** `WebRTCSinkBlock` turns your .NET application into a WebRTC server: it serves the viewer page, runs the signalling, and sends every viewer their own WebRTC stream. Nothing to install next to the application.
2. **Through a media server, with WHIP.** `WHIPSinkBlock` publishes one stream to a WebRTC media server — MediaMTX, Janus, Cloudflare Stream — over WHIP (WebRTC-HTTP Ingestion Protocol). The server then delivers it to any number of viewers, on the internet as well.

![An application window shared from a C# app with WebRTCSinkBlock and playing in a browser; the clock in the window runs about 0.15 seconds behind the browser's own clock](webrtc-whip-browser.webp)

```mermaid
graph LR;
    src["Screen, window or camera<br/>+ microphone"] --> app;
    subgraph app ["Your .NET application"]
        WebRTCSinkBlock;
        WHIPSinkBlock;
    end
    WebRTCSinkBlock -- "WebRTC, VP8 + Opus" --> b1["Browsers on your network"];
    WHIPSinkBlock -- "WHIP: H.264 + Opus" --> server["Media server<br/>MediaMTX, Janus"];
    server -- "WebRTC (WHEP)" --> b2["Browsers anywhere"];
```

Which one to use:

| | `WebRTCSinkBlock` | `WHIPSinkBlock` |
| --- | --- | --- |
| Extra software | none | a WebRTC media server |
| Encoding | VP8 and Opus, one encoder per viewer, with per-viewer congestion control | H.264 and Opus, encoded once in your pipeline |
| CPU per extra viewer | one more VP8 encoder | none: the server forwards the stream |
| Viewer page | served by the block, about 100 lines of JavaScript | the server's own player, or any WHEP player |
| Platforms | Windows, macOS, Linux, iOS | Windows, macOS, Linux, iOS |
| Typical use | screen sharing and camera views for a few viewers on a local network | publishing to many viewers, or across the internet |

## Prerequisites

- Windows, macOS, Linux or iOS.
- .NET 10 (.NET 8 or later on Linux).
- Media Blocks SDK .NET version 2026.11.21 or later. That release adds the screen-capture-to-`WebRTCSinkBlock` fix this guide relies on; with 2026.11.20, `WebRTCSinkBlock` plays cameras but a viewer of a Direct3D 11 screen capture never gets a picture. On iOS, both blocks need the SDK and `VisioForge.CrossPlatform.Core.iOS` at 2026.11.23 or later.
- For WHIP, a WebRTC media server. The examples use [MediaMTX](https://github.com/bluenviron/mediamtx), whose default configuration accepts WHIP on port 8889.

NuGet packages:

- [VisioForge.DotNet.MediaBlocks](https://www.nuget.org/packages/VisioForge.DotNet.MediaBlocks/) — the SDK
- Windows: [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) and [VisioForge.CrossPlatform.Core.Windows.Adds.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Adds.x64/) — the GStreamer runtime and the additional plugins, `rswebrtc` (both blocks) among them
- macOS: [VisioForge.CrossPlatform.Core.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS/) and [VisioForge.CrossPlatform.Core.macOS.Adds](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS.Adds/)
- Linux: `VisioForge.CrossPlatform.Core.Linux.x64` and `VisioForge.CrossPlatform.Core.Linux.Adds.x64` (`.arm64` for ARM), plus the distribution's GStreamer with `gstreamer1.0-plugins-bad` and `gstreamer1.0-nice`
- iOS: [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS/), which includes `rswebrtc`

Without the `Adds` package `WebRTCSinkBlock.IsAvailable()` and `WHIPSinkBlock.IsAvailable()` return `false`; on iOS, both come with `VisioForge.CrossPlatform.Core.iOS`. For deployment details see the [Deployment Guide](../../deployment-x/index.md).

## Part 1: Straight to Browsers with WebRTCSinkBlock

### How a viewer connects

`WebRTCSinkBlock` runs two servers inside your process: a web server for the viewer page and a WebSocket signalling server. The page asks the signalling server for the stream, the block sends a WebRTC offer, and the browser answers. From then on the video goes over the viewer's own WebRTC connection.

```mermaid
sequenceDiagram
    participant Browser
    participant Web as Web server (WebRTCSinkBlock)
    participant Sig as Signalling server (WebRTCSinkBlock)
    Browser->>Web: GET /?signalling=8443
    Web-->>Browser: index.html
    Browser->>Sig: WebSocket: register as a listener, list producers
    Browser->>Sig: start a session with the producer
    Sig-->>Browser: SDP offer (VP8, Opus)
    Browser->>Sig: SDP answer, ICE candidates
    Sig-->>Browser: ICE candidates
    Note over Browser,Sig: ConsumerAdded is raised
    Note over Browser,Sig: the screen or camera plays over the viewer's own WebRTC connection
```

Every viewer gets its own encoder and its own congestion control, so a viewer on a slow Wi-Fi link gets a lower bitrate without lowering the quality for the others.

### The complete program

The console program below sends the primary screen — or, with `camera` on the command line, the first camera — and the first microphone to every browser that opens `http://localhost:8090/?signalling=8443`.

<!-- vf-compile -->
```csharp
using System;
using System.IO;
using System.Threading.Tasks;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

// 127.0.0.1 serves this computer only; "0.0.0.0" serves viewers on other computers too.
const string Host = "127.0.0.1";
const int WebPort = 8090;
const int SignallingPort = 8443;

await VisioForgeX.InitSDKAsync();

MediaBlocksPipeline? pipeline = null;

try
{
    if (!WebRTCSinkBlock.IsAvailable())
    {
        Console.WriteLine("WebRTCSinkBlock is not available: add the VisioForge.CrossPlatform.Core.Windows.Adds.x64 package.");
        return;
    }

    pipeline = new MediaBlocksPipeline();
    pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

    // The primary screen by default, the first camera with "camera" on the command line.
    MediaBlock video;
    if (args.Length > 0 && args[0] == "camera")
    {
        var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (cameras.Length == 0)
        {
            Console.WriteLine("No camera found.");
            return;
        }

        video = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(cameras[0]));
    }
    else
    {
        video = new ScreenSourceBlock(new ScreenCaptureD3D11SourceSettings { FrameRate = new VideoFrameRate(30) });
    }

    // Its own signalling server, plus a web server for the viewer page in wwwroot.
    var webrtc = new WebRTCSinkBlock(new WebRTCSinkSettings
    {
        SignallingServerHost = Host,
        SignallingServerPort = SignallingPort,
        WebServerDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
        WebServerAddress = $"http://{Host}:{WebPort}/"
    });
    webrtc.ConsumerAdded += (s, e) => Console.WriteLine($"Viewer connected: {e.PeerId}");
    webrtc.ConsumerRemoved += (s, e) => Console.WriteLine($"Viewer left: {e.PeerId}");

    // Raw video: the sink encodes it to VP8 for each viewer.
    pipeline.Connect(video.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Video));

    // The first microphone, if there is one: encoded to Opus for each viewer.
    var microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
    if (microphones.Length > 0)
    {
        var microphone = new SystemAudioSourceBlock(microphones[0].CreateSourceSettings());
        pipeline.Connect(microphone.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Audio));
    }

    if (await pipeline.StartAsync())
    {
        Console.WriteLine($"Open http://localhost:{WebPort}/?signalling={SignallingPort} in a browser. Press Enter to stop.");
        Console.ReadLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    if (pipeline != null)
    {
        await pipeline.StopAsync();
        await pipeline.DisposeAsync();
    }

    VisioForgeX.DestroySDK();
}
```

Copy the viewer page to the output folder with the build — add this to the `.csproj`:

```xml
<ItemGroup>
  <Content Include="wwwroot\**" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

What each part does:

- **`ScreenSourceBlock` with `ScreenCaptureD3D11SourceSettings`** captures the primary monitor through Direct3D 11 at its native resolution. `FrameRate` defaults to 10; 30 makes mouse movement and scrolling smooth. `MonitorIndex` picks another monitor.
- **`SystemVideoSourceBlock`** opens a camera. `new VideoCaptureDeviceSourceSettings(device)` picks its HD format when it has one; set `Format` to choose another.
- **`WebRTCSinkBlock.CreateNewInput`** creates one input per call. Raw video and audio are encoded by the block for each viewer, video to VP8 and audio to Opus. Video already encoded to VP8 (with `VPXEncoderBlock`) is sent as it is, without an encoder per viewer.
- **`WebServerDirectory`** turns on the block's web server and points it at the folder with `index.html`. Leave it `null` to serve the page yourself (see [Serving the page from your own web server](#serving-the-page-from-your-own-web-server)).
- **`SignallingServerHost`** and the host in **`WebServerAddress`** decide who can connect: `127.0.0.1` accepts this computer only, `0.0.0.0` every network interface.
- **`ConsumerAdded` / `ConsumerRemoved`** report each viewer that connects and leaves. They are raised on a GStreamer thread: in a UI application marshal to the UI thread, as the demo does with `Dispatcher.Invoke`. `ConsumerRemoved` is not raised for viewers still connected when the pipeline stops; their sessions end with it.
- **`try` / `finally`** reports what went wrong and always stops and disposes the pipeline and calls `VisioForgeX.DestroySDK()`. When `StartAsync` returns `false`, the reason is in the `OnError` messages — a port already in use is the common one.

### The viewer page

The page speaks the signalling protocol of the GStreamer `webrtcsink` element: JSON messages over a WebSocket. Save it as `wwwroot/index.html`. It is the WebRTC Browser Streamer Demo's page: it plays the first stream the server announces, retries when the application is not running yet, and starts muted so the browser plays it without a click.

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>VisioForge WebRTC Viewer</title>
    <link rel="icon" href="data:,">
    <style>
        body { margin: 0; background: #111; color: #ddd; font-family: system-ui, sans-serif; display: flex; flex-direction: column; align-items: center; }
        video { width: 100%; max-width: 1280px; max-height: 80vh; background: #000; margin-top: 16px; }
        #bar { margin: 12px 16px; display: flex; gap: 12px; align-items: center; }
        button { padding: 6px 14px; }
    </style>
</head>
<body>
    <video id="video" autoplay playsinline muted></video>
    <div id="bar">
        <button id="unmute">Unmute</button>
        <span id="status">Connecting...</span>
    </div>

    <script>
        // Signalling port of WebRTCSinkBlock: ?signalling=8443 in the page URL.
        const port = new URLSearchParams(location.search).get('signalling') || '8443';
        const video = document.getElementById('video');
        const status = document.getElementById('status');
        let ws, pc, sessionId;

        document.getElementById('unmute').onclick = () => { video.muted = false; };

        function send(msg) { ws.send(JSON.stringify(msg)); }

        function connect() {
            // An https:// page needs wss://: set SignallingServerCertificate and SignallingServerKey.
            ws = new WebSocket(`${location.protocol === 'https:' ? 'wss' : 'ws'}://${location.hostname}:${port}`);
            ws.onopen = () => send({ type: 'setPeerStatus', roles: ['listener'], meta: null });
            ws.onclose = () => { reset(); status.textContent = 'Disconnected, retrying...'; setTimeout(connect, 2000); };
            ws.onmessage = e => onMessage(e.data).catch(err => fail('Error: ' + err));
        }

        function startSession(peerId) {
            if (sessionId === undefined) {
                sessionId = null;
                send({ type: 'startSession', peerId });
            }
        }

        async function onMessage(data) {
            const msg = JSON.parse(data);
            switch (msg.type) {
                case 'welcome':
                    send({ type: 'list' });
                    break;
                case 'list':
                    if (msg.producers.length > 0) startSession(msg.producers[0].id);
                    else status.textContent = 'Waiting for the stream...';
                    break;
                case 'peerStatusChanged':
                    if (msg.roles.includes('producer')) startSession(msg.peerId);
                    break;
                case 'sessionStarted':
                    sessionId = msg.sessionId;
                    break;
                case 'peer':
                    await onPeer(msg);
                    break;
                case 'endSession':
                    reset();
                    status.textContent = 'Stream ended, waiting...';
                    listLater();
                    break;
                case 'error':
                    fail('Error: ' + msg.details);
                    break;
            }
        }

        async function onPeer(msg) {
            if (msg.sdp && msg.sdp.type === 'offer') {
                // Mirror the sink's StunServer and TurnServers here when it is given them.
                if (pc) { pc.close(); video.srcObject = null; }
                pc = new RTCPeerConnection({ iceServers: [{ urls: 'stun:stun.l.google.com:19302' }] });
                const conn = pc;
                pc.onconnectionstatechange = () => {
                    if (conn === pc && conn.connectionState === 'failed') {
                        video.srcObject = null;
                        fail('Connection lost');
                    }
                };
                pc.ontrack = e => { video.srcObject = e.streams[0] || new MediaStream([e.track]); status.textContent = 'Playing'; };
                pc.onicecandidate = e => {
                    if (e.candidate) send({ type: 'peer', sessionId, ice: { candidate: e.candidate.candidate, sdpMLineIndex: e.candidate.sdpMLineIndex } });
                };
                await pc.setRemoteDescription(msg.sdp);
                const answer = await pc.createAnswer();
                await pc.setLocalDescription(answer);
                send({ type: 'peer', sessionId, sdp: { type: 'answer', sdp: answer.sdp } });
            } else if (msg.ice && pc) {
                await pc.addIceCandidate({ candidate: msg.ice.candidate, sdpMLineIndex: msg.ice.sdpMLineIndex });
            }
        }

        // An error while a stream plays (a stale message, a rejected candidate) leaves it alone;
        // otherwise the session is ended and the producer list asked for again.
        function fail(text) {
            if (video.srcObject) {
                status.textContent = text;
                return;
            }

            if (sessionId) send({ type: 'endSession', sessionId });
            reset();
            status.textContent = text + ', retrying...';
            listLater();
        }

        // Asks for the producer list again after a pause, so a failing session does not retry in a tight loop.
        function listLater() {
            const socket = ws;
            setTimeout(() => { if (ws === socket && ws.readyState === WebSocket.OPEN) send({ type: 'list' }); }, 2000);
        }

        function reset() {
            if (pc) pc.close();
            pc = undefined;
            sessionId = undefined;
            video.srcObject = null;
        }

        connect();
    </script>
</body>
</html>
```

The exchange, step by step:

1. **Register as a listener.** The page opens a WebSocket to the signalling port and sends `setPeerStatus` with the `listener` role, then asks for the `list` of producers. Your application is the producer.
2. **Start a session.** The page sends `startSession` with the producer's ID; a producer that appears later is announced with `peerStatusChanged`, so the page can be opened before the application starts.
3. **Answer the offer.** The block sends the SDP offer in a `peer` message; the page creates an `RTCPeerConnection`, answers, and both sides exchange ICE candidates in further `peer` messages.
4. **Play.** `ontrack` hands the stream to the `<video>` element. The status shows `Playing`; **Unmute** turns the sound on, because browsers play a muted video without a click and an unmuted one only after it.
5. **Recover.** When the connection fails or the application stops, the page resets and asks for the list again, so it reconnects by itself when the stream comes back.

### Share one window instead of the whole screen

Set `WindowHandle` to capture a single application window through Windows Graphics Capture. The window is captured even when other windows cover it, and the rest of the desktop stays private:

```csharp
var screen = new ScreenCaptureD3D11SourceSettings
{
    FrameRate = new VideoFrameRate(30),
    WindowHandle = windowHandle // HWND of the window to share, for example Process.MainWindowHandle
};

var video = new ScreenSourceBlock(screen);
```

The stream takes the window's size when the capture starts. A minimized window gives no frames until it is restored.

### Serving the page from your own web server

The block's web server is optional. With `WebServerDirectory` left `null`, only the signalling server runs; put `index.html` in your own site (ASP.NET Core, IIS, nginx) and pass the signalling port in the query string. The page connects to `ws://<the page's host>:<port>`, or `wss://` when the page itself is on `https://`, so the signalling server must be reachable on the same host name as the page.

### A viewer page on an HTTPS site

A browser blocks a `ws://` connection from an `https://` page. Give the signalling server your site's certificate and private key as PEM files and it speaks `wss://`:

```csharp
var webrtcSink = new WebRTCSinkBlock(new WebRTCSinkSettings
{
    SignallingServerHost = "0.0.0.0",
    SignallingServerPort = 8443,
    SignallingServerCertificate = @"C:\certs\fullchain.pem",
    SignallingServerKey = @"C:\certs\privkey.pem"
});
```

The certificate must be valid for the host name in the page's address, and the key must not be encrypted. `Build` fails, with an `OnError` message, when only one of the two is set, a file cannot be read, the key is not the certificate's (checked on .NET 5 and later), or `WebServerDirectory` is set as well. The block's own web server serves `http://` only, so put an `https://` page on your own web server (IIS, nginx, ASP.NET Core) and leave `WebServerDirectory` `null`.

### Viewers on other computers

1. Set `Host` to `"0.0.0.0"`, so both servers listen on every network interface.
2. Allow the web port and the signalling port (8090 and 8443 here) through Windows Defender Firewall, and the application itself for the WebRTC media, which uses UDP ports chosen at run time.
3. Open `http://<this computer's address>:8090/?signalling=8443` on the other computer.

Receiving WebRTC needs no HTTPS, so plain `http://` works on a local network: we opened the demo's page by the computer's network address, which the browser treats as not secure, and the camera played. `WebRTCSinkSettings` has no authentication setting: anyone who can reach the two ports can watch. Keep `127.0.0.1` when the stream should not leave the computer, and use WHIP with a media server that checks tokens when it must reach the internet.

## Part 2: Publish to a Media Server with WHIPSinkBlock

### The pipeline

WHIP takes encoded H.264 video and Opus audio. The pipeline encodes once; the media server forwards the stream to every viewer.

```mermaid
graph LR;
    ScreenSourceBlock -- "raw video" --> H264EncoderBlock;
    SystemAudioSourceBlock -- "raw audio" --> OPUSEncoderBlock;
    H264EncoderBlock -- H.264 --> WHIPSinkBlock;
    OPUSEncoderBlock -- Opus --> WHIPSinkBlock;
    WHIPSinkBlock -- "WHIP (HTTP + WebRTC)" --> MediaMTX;
```

### The complete program

The console program publishes the primary screen and the first microphone to `http://<server>:8889/screen/whip`, MediaMTX's WHIP endpoint for a stream named `screen`. Pass another endpoint on the command line. The screen is captured with `ScreenCaptureD3D11SourceSettings`, so this program builds for Windows; on macOS use `ScreenCaptureMacOSSourceSettings`, on Linux `ScreenCaptureXDisplaySourceSettings`, or a camera, as the Avalonia demo does.

<!-- vf-compile -->
```csharp
using System;
using System.Threading.Tasks;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// The media server's WHIP endpoint; MediaMTX takes http://<server>:8889/<stream name>/whip.
var whipUrl = args.Length > 0 ? args[0] : "http://192.168.1.10:8889/screen/whip";

await VisioForgeX.InitSDKAsync();

MediaBlocksPipeline? pipeline = null;

try
{
    if (!WHIPSinkBlock.IsAvailable())
    {
        Console.WriteLine("WHIPSinkBlock is not available: add the VisioForge.CrossPlatform.Core.*.Adds package for this platform.");
        return;
    }

    pipeline = new MediaBlocksPipeline();
    pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

    var screen = new ScreenSourceBlock(new ScreenCaptureD3D11SourceSettings { FrameRate = new VideoFrameRate(30) });

    // WHIP carries H.264 video and Opus audio: encode once here, the server serves every viewer.
    var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
    var whip = new WHIPSinkBlock(new WHIPSinkSettings { Location = whipUrl });

    pipeline.Connect(screen.Output, h264.Input);
    pipeline.Connect(h264.Output, whip.CreateNewInput(MediaBlockPadMediaType.Video));

    var microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
    if (microphones.Length > 0)
    {
        var microphone = new SystemAudioSourceBlock(microphones[0].CreateSourceSettings());
        var opus = new OPUSEncoderBlock(new OPUSEncoderSettings());
        pipeline.Connect(microphone.Output, opus.Input);
        pipeline.Connect(opus.Output, whip.CreateNewInput(MediaBlockPadMediaType.Audio));
    }

    if (await pipeline.StartAsync())
    {
        Console.WriteLine($"Publishing to {whipUrl}. Press Enter to stop.");
        Console.ReadLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    if (pipeline != null)
    {
        await pipeline.StopAsync();
        await pipeline.DisposeAsync();
    }

    VisioForgeX.DestroySDK();
}
```

What each part does:

- **`H264EncoderBlock` with `OpenH264EncoderSettings`** encodes in software, on every Windows computer, in the Constrained Baseline profile that every browser decodes. On a computer with an NVIDIA, AMD or Intel GPU, a hardware encoder's settings (`NVENCH264EncoderSettings`, `AMFH264EncoderSettings`, `QSVH264EncoderSettings`) take the encoding off the CPU.
- **`OPUSEncoderBlock`** encodes the microphone to Opus, the audio codec of WebRTC.
- **`WHIPSinkBlock.CreateNewInput`** creates the video and the audio input. The block adds the RTP packaging itself, so the encoders connect to it directly. Audio is optional: without a microphone the stream is video only.
- **`WHIPSinkSettings.Location`** is the WHIP endpoint. On `StartAsync` the block posts its SDP offer there, the server answers, and the media flows over WebRTC. On stop the block deletes the session on the server.

### Watching the stream

MediaMTX serves its own player for every stream at `http://<server>:8889/<stream name>/` — here `http://<server>:8889/screen/`. Any WHEP player can read it too, from `http://<server>:8889/screen/whep`, including the SDK's own [`WHEPSourceBlock`](../Sources/index.md#whep-source-block).

![An application window published to MediaMTX with WHIPSinkBlock, playing in MediaMTX's browser player](webrtc-whip-mediamtx.webp)

### Authentication, STUN and TURN

```csharp
var whip = new WHIPSinkBlock(new WHIPSinkSettings
{
    Location = "https://media.example.com/live/whip",
    AuthToken = "secret-token",                     // sent as "Authorization: Bearer secret-token"
    StunServer = "stun://stun.l.google.com:19302",
    TurnServer = "turn://user:password@turn.example.com:3478"
});
```

- **`AuthToken`** — the bearer token of services that require one (Cloudflare Stream, a Janus or MediaMTX server with authentication).
- **`StunServer` / `TurnServer`** — the publisher's side of NAT traversal. Unset, the block uses no STUN server, which is enough when the application and the server are on the same network. `UseLinkHeaders` (on by default) also takes the ICE servers the WHIP server announces in its `Link` headers.
- **`IceTransportPolicy = WHIPIceTransportPolicy.Relay`** sends everything through the TURN server, for networks that block direct UDP.
- **`Timeout`** (15 seconds by default) bounds every WHIP HTTP request, including the session's `DELETE` on stop.

## Both from One Capture

One capture can feed both outputs at the same time: viewers on the local network get the stream straight from the application, and the media server gets the WHIP stream for everyone else. `TeeBlock` splits the video:

```csharp
var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);
pipeline.Connect(screen.Output, tee.Input);

// Local viewers: raw video, encoded per viewer to VP8.
pipeline.Connect(tee.Outputs[0], webrtc.CreateNewInput(MediaBlockPadMediaType.Video));

// The media server: H.264, encoded once.
pipeline.Connect(tee.Outputs[1], h264.Input);
pipeline.Connect(h264.Output, whip.CreateNewInput(MediaBlockPadMediaType.Video));
```

`TeeBlock` is in `VisioForge.Core.MediaBlocks.Special`. The two outputs start and stop together: a WHIP server that cannot be reached fails the start of the whole pipeline. Put them in two pipelines when the local stream must keep running without the server.

## Limits to Know Before Shipping

### Platforms

`WebRTCSinkBlock` runs on Windows, macOS and Linux with the platform's `Adds` package, and on iOS with `VisioForge.CrossPlatform.Core.iOS` 2026.11.23 or later: an older `Core.iOS` has the plugin without its fixes, so the sink starts, but `StopAsync` can hang when a viewer leaves during its negotiation. `WHIPSinkBlock` runs on the same platforms with the same packages. On Linux both also need the distribution's `gstreamer1.0-plugins-bad` and `gstreamer1.0-nice`. Check `IsAvailable()` at startup, as both programs do.

### Codecs

`WebRTCSinkBlock` sends VP8 video and Opus audio, which every current browser plays. It takes raw video and audio, or VP8 and Opus already encoded; it does not take H.264. `WHIPSinkBlock` takes H.264 and Opus only.

### Latency

In our test on one Windows computer, a change in a 320×240 picture at 30 frames per second reached a viewer's decoded frame 55 to 60 ms after it left the source (median 58 ms), before the viewer's playout delay. With a window's capture and the browser's playout and display added, the screenshot at the start of this guide shows about 0.15 seconds.

### CPU and resolution

`WebRTCSinkBlock` runs a software VP8 encoder per viewer. In our tests on one Windows computer, a 1080p camera and a 1902×1131 window played at 30 frames per second, while a whole 3840×2160 monitor reached about 20 frames per second in the browser. For a 4K screen, scale the video down before the sink with `VideoResizeBlock` (namespace `VisioForge.Core.MediaBlocks.VideoProcessing`):

```csharp
var resize = new VideoResizeBlock(1920, 1080);
pipeline.Connect(screen.Output, resize.Input);
pipeline.Connect(resize.Output, webrtc.CreateNewInput(MediaBlockPadMediaType.Video));
```

For more than a handful of viewers, publish once with WHIP and let the media server serve them.

### Bitrate

`StartBitrate` (2 Mbit/s) is where each viewer starts; congestion control (`CongestionControl`, Google Congestion Control by default) raises it up to `MaxBitrate` (8 Mbit/s) while the network keeps up and lowers it when it does not. Raise `MaxBitrate` for sharp text on a large screen; set `CongestionControl = WebRTCSinkCongestionControl.Disabled` to hold every viewer at `StartBitrate`.

### Ports

The defaults are 8443 for signalling and 8080 for the web server. When a port is taken, `StartAsync` returns `false` and `OnError` says which port. The programs in this guide use 8090 for the page.

### NAT, STUN and TURN

On a local network no STUN server is needed. For viewers behind other networks, set `StunServer` and `TurnServers` in `WebRTCSinkSettings` (`StunServer` defaults to `stun://stun.l.google.com:19302`) and the same servers in the page's `iceServers` list. Across the internet, WHIP to a media server with a public address is usually the simpler route.

## Troubleshooting

| Symptom | Cause and fix |
| --- | --- |
| `IsAvailable()` returns `false` | The platform's `Adds` package is not referenced (on iOS, `VisioForge.CrossPlatform.Core.iOS` is missing or the SDK is older than 2026.11.23), or on Linux `gstreamer1.0-plugins-bad` and `gstreamer1.0-nice` are missing. |
| `StartAsync` returns `false`, `OnError` says the web server cannot listen | The web port is in use by another program. Choose another `WebServerAddress` port, or stop the other program. |
| The page says `Disconnected, retrying...` | The signalling server is not reachable: the application is not running, the `?signalling=` port is wrong, or the firewall blocks it. |
| The page says `Waiting for the stream...` | The page reached the signalling server, but the pipeline is not playing yet. Check `StartAsync` and the `OnError` messages. |
| The page says `Playing` but there is no sound | The video starts muted. Press **Unmute**. |
| A viewer on another computer stays at `Connection lost` | The WebRTC media is blocked: allow the application through the firewall, or set STUN/TURN on both sides. |
| A Direct3D 11 screen capture never plays in the browser, the page keeps reconnecting | Media Blocks SDK .NET 2026.11.20 or earlier. Update to 2026.11.21 or later. |
| The screen plays at a low frame rate | A 4K screen is encoded per viewer in software. Scale it to 1080p with `VideoResizeBlock`, or publish with WHIP. |
| WHIP: `StartAsync` returns `false` or `OnError` reports an HTTP error | The endpoint is wrong, the server is down, or it refused the token. Open the same URL's player page in a browser to check the server. |
| WHIP: the stream reaches the server but stays black or `ICE failed` | UDP between the application and the server is blocked. Allow the server's WebRTC UDP port (8189 on MediaMTX), or set a TURN server with `IceTransportPolicy = Relay`. |

## Frequently Asked Questions

### Do I need a media server to show my screen in a browser?

No. `WebRTCSinkBlock` serves the page, runs the signalling and streams to each browser from your process. A media server earns its place when there are many viewers or they are on the internet; then publish to it with `WHIPSinkBlock`.

### Why VP8 for browsers and H.264 for WHIP?

`WebRTCSinkBlock` encodes per viewer and uses VP8, which every browser decodes and which the block can adapt to each viewer's bandwidth. WHIP servers such as MediaMTX forward one stream to all viewers without re-encoding, so the stream must already be in a codec they accept; H.264 is the one every server and browser takes.

### How many viewers can WebRTCSinkBlock serve?

Each viewer costs one VP8 encoder. A 1080p stream runs a handful of viewers on a desktop CPU; at 4K the first viewer is already the limit. For more viewers, publish once with WHIP.

### Can I add audio?

Yes. Connect a raw audio source to `CreateNewInput(MediaBlockPadMediaType.Audio)` of `WebRTCSinkBlock` — it encodes Opus per viewer — or an `OPUSEncoderBlock` output to the audio input of `WHIPSinkBlock`.

### Does it work on macOS and Linux?

Both do, and both run on iOS too. To serve cameras without a viewer page of your own, use [`WHEPServerSinkBlock`](rtsp-cameras-in-browser-whep.md).

### Can a browser play the WHIP stream without the media server's page?

Yes. The server exposes the stream over WHEP, and a page with about 20 lines of JavaScript plays it — the player in [RTSP Cameras in a Browser with WHEP](rtsp-cameras-in-browser-whep.md#the-browser-side-a-minimal-whep-player) works against MediaMTX's `/whep` URL.

## See Also

- [Sink blocks → WebRTC](../Sinks/index.md#webrtc) — `WebRTCSinkBlock` and `WebRTCSinkSettings` reference
- [Sink blocks → WHIP](../Sinks/index.md#whip) — `WHIPSinkBlock` and `WHIPSinkSettings` reference
- [RTSP Cameras in a Browser with WHEP](rtsp-cameras-in-browser-whep.md) — IP cameras on a web page without re-encoding
- [Screen Source Block](../Sources/index.md#screen-source-block) — screen, window and monitor capture settings
- [Pre-Event Recording](pre-event-recording.md) — keep the seconds before an event while you stream
