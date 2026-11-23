---
title: RTSP IP Cameras in a Web Browser with WHEP in C# .NET
description: Show RTSP IP cameras with sound in a web browser over WebRTC (WHEP) from a C# app with sub-second latency, no video re-encoding, no media server.
sidebar_label: RTSP Cameras in a Browser (WHEP)
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Streaming
  - IP Camera
  - RTSP
  - WebRTC
  - H.264
  - C#
  - JavaScript
  - NuGet
primary_api_classes:
  - WHEPServerSinkBlock
  - WHEPServerSinkSettings
  - RTSPRAWSourceBlock
  - RTSPRAWSourceSettings
  - DecodeBinBlock
  - MediaBlocksPipeline

---

# Show RTSP IP Cameras in a Web Browser with WHEP in C# / .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Demo Sample"
    The code in this guide comes from the RTSP WHEP Camera Grid Demo (WPF) in the [samples repository](https://github.com/visioforge/.Net-SDK-s-samples): it adds cameras by RTSP URL, serves a grid page with every camera, and forwards each camera's WHEP requests to its own server. Its console version, RTSP WHEP Camera Grid (Console), does the same on Windows, macOS and Linux, with the camera URLs on the command line.

    The block reference is in [Sink blocks → WHEP server](../Sinks/index.md#whep-server).

## What You Will Build

A browser cannot open an RTSP URL. The usual ways to put an IP camera on a web page are to transcode it to HLS (several seconds of delay and a decoder plus an encoder per camera) or to run a separate media server next to your application. This guide takes a third route: your .NET application itself becomes a WebRTC server, the camera's H.264 goes to the browser exactly as the camera sent it, and the camera's audio goes with it as Opus.

![Two cameras in a browser grid, each playing over WebRTC from its own WHEPServerSinkBlock](rtsp-cameras-in-browser-whep-grid.webp)

The pipeline for one camera is two blocks for the video, and a decoder for the audio:

```mermaid
graph LR;
    cam["IP camera"] -- "RTSP, H.264 + AAC" --> RTSPRAWSourceBlock;
    subgraph app ["Your .NET application"]
        RTSPRAWSourceBlock -- "H.264, not decoded" --> WHEPServerSinkBlock;
        RTSPRAWSourceBlock -- "AAC / G.711" --> DecodeBinBlock;
        DecodeBinBlock -- "PCM" --> WHEPServerSinkBlock;
    end
    WHEPServerSinkBlock -- WebRTC --> b1["Browser"];
    WHEPServerSinkBlock -- WebRTC --> b2["Browser"];
```

- `RTSPRAWSourceBlock` connects to the camera and outputs its encoded H.264 without decoding it, and its encoded audio.
- `DecodeBinBlock` decodes the camera's AAC or G.711. Browsers take Opus over WebRTC, so the audio cannot be passed through as the video is.
- `WHEPServerSinkBlock` runs a WHEP (WebRTC-HTTP Egress Protocol) server. A browser posts a WebRTC offer to it over HTTP, gets an answer, and receives the video and the audio over its own WebRTC session. The block encodes the audio to Opus for each viewer.

What this buys you:

- **Sub-second latency.** The browser plays the stream over WebRTC, not through a segment-based protocol.
- **Almost no CPU for the video, for any number of viewers.** The video is never decoded or encoded: the block sends the camera's stream to every viewer as it arrives. The audio is decoded once per camera and encoded to Opus for each viewer. Each viewer still costs network bandwidth equal to the camera's bitrate.
- **No media server.** The WHEP server runs inside your process; the browser side is about 20 lines of JavaScript with no player library.

WHEP is a small HTTP protocol on top of WebRTC: one `POST` with the SDP offer, a `201 Created` with the SDP answer and a `Location` header for the session, and a `DELETE` on that `Location` when the viewer leaves. Any WHEP player can use the endpoint, not only the page in this guide.

## Prerequisites

- Windows x64, macOS, or Linux x64/arm64 (Debian 12, Ubuntu 24.04 and later). `WHEPServerSinkBlock` uses the `rswebrtc` GStreamer plugin, which ships in each platform's `Adds` runtime package.
- .NET 10 (.NET 8 or later on Linux).
- An IP camera with an H.264 RTSP stream; for sound, a camera that sends audio (AAC or G.711).
- Media Blocks SDK .NET version 2026.11.20 or later on Windows — the release that adds `WHEPServerSinkBlock` — and 2026.11.21 or later on macOS and Linux. The audio input, `WHEPServerSinkBlock.AudioInput`, needs 2026.11.23 or later.

NuGet packages:

- [VisioForge.DotNet.MediaBlocks](https://www.nuget.org/packages/VisioForge.DotNet.MediaBlocks/) — the SDK
- Windows: [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) and [VisioForge.CrossPlatform.Core.Windows.Adds.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.Adds.x64/) — the GStreamer runtime and the additional plugins, `rswebrtc` among them
- macOS: [VisioForge.CrossPlatform.Core.macOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS/) and [VisioForge.CrossPlatform.Core.macOS.Adds](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macOS.Adds/)
- Linux: `VisioForge.CrossPlatform.Core.Linux.x64` and `VisioForge.CrossPlatform.Core.Linux.Adds.x64` (`.arm64` for ARM), plus the distribution's GStreamer with `gstreamer1.0-plugins-bad` and `gstreamer1.0-nice`

Without the `Adds` package `WHEPServerSinkBlock.IsAvailable()` returns `false`. For deployment details see the [Deployment Guide](../../deployment-x/index.md).

The browser page is a file the application serves. Embed it in the assembly in the `.csproj`, as the console demo does — a macOS app bundle would put a copied file under `Contents/Resources`, not next to the executable:

```xml
<ItemGroup>
  <EmbeddedResource Include="wwwroot\index.html" LogicalName="index.html" />
</ItemGroup>
```

## The One Thing to Know First: Same Origin

The WHEP server sends no CORS headers. A browser therefore accepts its answers only on a page served from the same address and port as the endpoint. Opening `index.html` from disk, or serving it from another port, fails at the `fetch` of the offer.

The demo solves it the simple way: the application runs a small HTTP server for the page and forwards every request under `/whep/` to the WHEP server, which itself listens on loopback only. The browser talks to one origin; the WHEP server is never exposed directly.

```mermaid
sequenceDiagram
    participant Browser
    participant Page as Page server (your app)
    participant WHEPServerSinkBlock
    Browser->>Page: GET /
    Page-->>Browser: index.html
    Browser->>Page: POST /whep/endpoint (SDP offer)
    Page->>WHEPServerSinkBlock: POST /whep/endpoint
    WHEPServerSinkBlock-->>Page: 201 Created, SDP answer, Location
    Page-->>Browser: 201 Created, SDP answer, Location
    WHEPServerSinkBlock-)Browser: camera H.264 and Opus audio over WebRTC
    Browser->>Page: DELETE Location (page closed)
    Page->>WHEPServerSinkBlock: DELETE Location
```

Only the HTTP exchange goes through the page server. The video and the audio travel over the WebRTC connection that the offer and answer set up.

## One Camera: the Complete Server

The console program below is a single-camera version of the console demo. It builds the pipeline, serves the embedded `index.html` on `http://localhost:8080/` and forwards `/whep/...` to the WHEP server on `127.0.0.1:9100`.

```csharp
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

const string PageUrl = "http://localhost:8080/";
const string WhepServer = "http://127.0.0.1:9100";

await VisioForgeX.InitSDKAsync();

using var http = new HttpClient();
MediaBlocksPipeline? pipeline = null;
HttpListener? pageServer = null;

try
{
    if (!WHEPServerSinkBlock.IsAvailable())
    {
        Console.WriteLine("WHEPServerSinkBlock is not available: add the VisioForge.CrossPlatform.Core.*.Adds package for this platform.");
        return;
    }

    pipeline = new MediaBlocksPipeline();
    pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

    // Encoded H.264 straight from the camera, and its audio if it has any (CreateAsync turns audio off otherwise).
    var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(new Uri("rtsp://192.168.1.64:554/stream1"), "admin", "password", audioEnabled: true);
    var rtsp = new RTSPRAWSourceBlock(rtspSettings);

    // The WHEP server listens on loopback only; browsers reach it through the page server below.
    var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings { ServerAddress = WhepServer });
    whep.ConsumerAdded += (s, e) => Console.WriteLine($"Viewer connected: {e.PeerId}");
    whep.ConsumerRemoved += (s, e) => Console.WriteLine($"Viewer left: {e.PeerId}");

    pipeline.Connect(rtsp.VideoOutput, whep.VideoInput);

    // Browsers take Opus, not the camera's AAC or G.711: decode it here, the WHEP server encodes it to Opus.
    if (rtspSettings.AudioEnabled)
    {
        var audioDecoder = new DecodeBinBlock(renderVideo: false, renderAudio: true, renderSubtitle: false);
        pipeline.Connect(rtsp.AudioOutput, audioDecoder.Input);
        pipeline.Connect(audioDecoder.AudioOutput, whep.AudioInput);
    }

    // The page and the WHEP requests share one origin: whepserversink sends no CORS headers.
    pageServer = new HttpListener();
    pageServer.Prefixes.Add(PageUrl);
    pageServer.Start();
    _ = ServePageAsync(pageServer);

    if (await pipeline.StartAsync())
    {
        Console.WriteLine($"Open {PageUrl} in a browser. Press Enter to stop.");
        Console.ReadLine();
    }
}
catch (Exception ex)
{
    // CreateAsync throws when the camera cannot be read; HttpListener.Start when the page port is taken.
    Console.WriteLine(ex.Message);
}
finally
{
    pageServer?.Close();

    if (pipeline != null)
    {
        await pipeline.StopAsync();
        await pipeline.DisposeAsync();
    }

    VisioForgeX.DestroySDK();
}

async Task ServePageAsync(HttpListener server)
{
    while (server.IsListening)
    {
        HttpListenerContext context;
        try
        {
            context = await server.GetContextAsync();
        }
        catch (Exception ex) when (ex is HttpListenerException || ex is ObjectDisposedException)
        {
            return;
        }

        _ = HandleRequestAsync(context);
    }
}

async Task HandleRequestAsync(HttpListenerContext context)
{
    var request = context.Request;
    var response = context.Response;

    try
    {
        var path = request.Url?.AbsolutePath ?? string.Empty;

        if (path == "/")
        {
            // The page embedded by the .csproj above.
            using var page = System.Reflection.Assembly.GetEntryAssembly()!.GetManifestResourceStream("index.html")!;
            using var html = new MemoryStream();
            await page.CopyToAsync(html);
            await WriteAsync(response, "text/html", html.ToArray());
        }
        else if (path.StartsWith("/whep/", StringComparison.Ordinal))
        {
            // The offer and the session's DELETE go to the WHEP server unchanged.
            using var forward = new HttpRequestMessage(new HttpMethod(request.HttpMethod), WhepServer + path);
            if (request.HasEntityBody)
            {
                using var body = new MemoryStream();
                await request.InputStream.CopyToAsync(body);
                forward.Content = new ByteArrayContent(body.ToArray());
                forward.Content.Headers.TryAddWithoutValidation("Content-Type", request.ContentType);
            }

            using var reply = await http.SendAsync(forward);
            if (reply.Headers.Location != null)
            {
                // The session URL the player deletes when it leaves.
                response.Headers["Location"] = reply.Headers.Location.OriginalString;
            }

            response.StatusCode = (int)reply.StatusCode;
            await WriteAsync(response, reply.Content.Headers.ContentType?.ToString(), await reply.Content.ReadAsByteArrayAsync());
        }
        else
        {
            response.StatusCode = 404;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        response.StatusCode = 502;
    }
    finally
    {
        response.Close();
    }
}

static async Task WriteAsync(HttpListenerResponse response, string? contentType, byte[] body)
{
    if (contentType != null)
    {
        response.ContentType = contentType;
    }

    response.ContentLength64 = body.Length;
    await response.OutputStream.WriteAsync(body);
}
```

What each part does:

- **`RTSPRAWSourceSettings.CreateAsync`** connects to the camera and reads its stream description before the pipeline is built. With `audioEnabled: true` it keeps the camera's audio, and turns `AudioEnabled` off when the camera sends none — the code connects the audio only then. `CreateAsync` throws `InvalidOperationException` when it cannot read the stream — the camera is offline, the URL is wrong, or the credentials are refused.
- **`RTSPRAWSourceBlock.VideoOutput`** carries the camera's encoded H.264. Connecting it straight to **`WHEPServerSinkBlock.VideoInput`** (the same pad as `Input`) is the whole video path: no decoder, no encoder.
- **`RTSPRAWSourceBlock.AudioOutput`** carries the camera's encoded audio. **`DecodeBinBlock`** with only `renderAudio` decodes it, and **`WHEPServerSinkBlock.AudioInput`** takes the raw audio and encodes it to Opus for each viewer. A mono camera reaches the browser as two identical channels.
- **`WHEPServerSinkSettings.ServerAddress`** is the address the WHEP server listens on. `127.0.0.1` keeps it reachable from this machine only, which is what you want when the page server forwards to it. The block's `EndpointUrl` property returns the URL players post to — here `http://127.0.0.1:9100/whep/endpoint`.
- **`ConsumerAdded` / `ConsumerRemoved`** report each viewer that connects and leaves, with its session ID in `PeerId`. They are raised on a GStreamer thread: in a UI application marshal to the UI thread without blocking it — the demo uses `Dispatcher.BeginInvoke`. `ConsumerRemoved` is not raised for viewers still connected when the pipeline stops; their sessions end with it.
- **`try` / `finally`** reports what went wrong — `CreateAsync` failing on the camera, `HttpListener.Start` on a taken port — and the `finally` closes the page server, stops and disposes the pipeline and calls `VisioForgeX.DestroySDK()` whether start-up succeeded or failed.
- **The page server** uses `HttpListener`, which serves `http://localhost:<port>/` without administrator rights. It answers `/` with the page, forwards `/whep/...` with the same method, body and `Content-Type`, and copies the status code, the answer body and the `Location` header back to the browser.

The page server is started before the pipeline, so the page is reachable as soon as the stream is.

## The Browser Side: a Minimal WHEP Player

Put this file in `wwwroot/index.html`. The `play` function is the demo's player with one addition, the `iceServers` list passed to `RTCPeerConnection` (see [NAT, STUN and TURN](#nat-stun-and-turn)):

```html
<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title>Camera</title>
    <style>
        body { margin: 0; background: #111; color: #ddd; font-family: sans-serif; }
        figure { margin: 0; position: relative; background: #000; }
        video { width: 100%; aspect-ratio: 16 / 9; display: block; }
        figcaption { position: absolute; left: 8px; top: 6px; font-size: 13px; text-shadow: 0 0 3px #000; }
    </style>
</head>
<body>
<figure>
    <video id="video" autoplay muted playsinline controls></video>
    <figcaption id="caption" data-name="Camera">Camera</figcaption>
</figure>
<script>
    // STUN/TURN servers for the viewer's side of the connection; empty is enough on a local network.
    const iceServers = [];

    // A minimal WHEP player: offer, POST it to the endpoint, apply the answer.
    async function play(endpoint, video, caption) {
        const pc = new RTCPeerConnection({ iceServers });
        pc.addTransceiver('video', { direction: 'recvonly' });
        pc.addTransceiver('audio', { direction: 'recvonly' });
        // Video and audio arrive as two tracks; both go into the one stream the video element plays.
        video.srcObject = new MediaStream();
        pc.ontrack = e => video.srcObject.addTrack(e.track);

        await pc.setLocalDescription(await pc.createOffer());
        // Send the offer with all ICE candidates in it.
        await new Promise(resolve => {
            if (pc.iceGatheringState === 'complete') resolve();
            else pc.onicegatheringstatechange = () => pc.iceGatheringState === 'complete' && resolve();
        });

        const response = await fetch(endpoint, { method: 'POST', headers: { 'Content-Type': 'application/sdp' }, body: pc.localDescription.sdp });
        if (response.status !== 201) throw new Error(`${endpoint}: HTTP ${response.status}`);
        await pc.setRemoteDescription({ type: 'answer', sdp: await response.text() });

        // End the session on the server when the page closes.
        const session = response.headers.get('Location');
        addEventListener('pagehide', () => fetch(session, { method: 'DELETE', keepalive: true }));
        pc.onconnectionstatechange = () => caption.textContent = `${caption.dataset.name} - ${pc.connectionState}`;
    }

    const caption = document.getElementById('caption');
    play('/whep/endpoint', document.getElementById('video'), caption).catch(e => caption.textContent = e.message);
</script>
</body>
</html>
```

Step by step:

1. **Receive-only video and audio transceivers.** The page only watches and listens, so it offers to receive one video and one audio track and sends nothing. A camera without audio leaves the audio part of the answer inactive and the video plays alone.
2. **Wait for ICE gathering to complete** before sending the offer. All network candidates then travel inside the single `POST`, so the player never needs WHEP's `PATCH` requests for trickle ICE.
3. **`POST` the SDP offer** with `Content-Type: application/sdp`. The server answers `201 Created` with the SDP answer in the body.
4. **Apply the answer.** WebRTC connects and `ontrack` adds each track, video and audio, to the stream the `<video>` element plays.
5. **Delete the session on `pagehide`.** The `Location` header names the session; `keepalive: true` lets the `DELETE` go out while the page unloads. The server then raises `ConsumerRemoved`.

The `<video>` element is `muted`, which lets the browser start playback without a click, `controls` lets the viewer unmute it, and `playsinline` is for mobile browsers. Browsers do not autoplay sound before the viewer interacts with the page, so a page that should play the sound at once unmutes from a click handler. The caption shows the connection state (`connecting`, `connected`, `failed`), which is the first thing to look at when a tile stays black.

## Scaling to Several Cameras

A `WHEPServerSinkBlock` serves one camera. For several cameras the demo runs one block per camera, each on its own port, and gives each camera its own path on the page server.

```mermaid
graph LR;
    browser["Browser: grid page"] -- "/cam/0/whep/...<br/>/cam/1/whep/..." --> page["Page server<br/>localhost:8080"];
    subgraph cam0 ["Camera 0"]
        direction LR
        r0["RTSPRAWSourceBlock"] --> w0["WHEPServerSinkBlock<br/>127.0.0.1:9100"];
        r0 --> d0["DecodeBinBlock"] --> w0;
    end
    subgraph cam1 ["Camera 1"]
        direction LR
        r1["RTSPRAWSourceBlock"] --> w1["WHEPServerSinkBlock<br/>127.0.0.1:9101"];
        r1 --> d1["DecodeBinBlock"] --> w1;
    end
    page -- "/whep/... to :9100" --> cam0;
    page -- "/whep/... to :9101" --> cam1;
```

![The RTSP WHEP Camera Grid Demo window: two cameras with their WHEP URLs, the grid page link and two viewer sessions](rtsp-cameras-in-browser-whep-demo.webp)

The pipeline part, from the demo's start handler — one `RTSPRAWSourceBlock` → `WHEPServerSinkBlock` pair per camera, with a `DecodeBinBlock` for each camera that has audio, WHEP ports counted up from 9100:

```csharp
for (var i = 0; i < _cameras.Count; i++)
{
    var camera = _cameras[i];

    // Encoded H.264 straight from the camera, and its audio if it has any (CreateAsync turns audio off otherwise).
    var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(camera.Url, camera.Login, camera.Password, audioEnabled: true);
    var rtsp = new RTSPRAWSourceBlock(rtspSettings);

    // One WHEP server per camera; only this app talks to it, the browsers go through the page server below.
    var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings { ServerAddress = $"http://127.0.0.1:{FirstWhepPort + i}" });
    whep.ConsumerAdded += (s, args) => UpdateViewers(+1);
    whep.ConsumerRemoved += (s, args) => UpdateViewers(-1);

    _pipeline.Connect(rtsp.VideoOutput, whep.VideoInput);

    // Browsers take Opus, not the camera's AAC or G.711: decode it here, the WHEP server encodes it to Opus.
    if (rtspSettings.AudioEnabled)
    {
        var audioDecoder = new DecodeBinBlock(renderVideo: false, renderAudio: true, renderSubtitle: false);
        _pipeline.Connect(rtsp.AudioOutput, audioDecoder.Input);
        _pipeline.Connect(audioDecoder.AudioOutput, whep.AudioInput);
    }
}
```

The routing part: the page server forwards `/cam/N/whep/...` to camera N's server and puts the same `/cam/N` prefix in front of the `Location` it returns, so that the browser's `DELETE` reaches the right camera too:

```csharp
var path = request.Url?.AbsolutePath ?? string.Empty;
var parts = path.Split('/', 4, StringSplitOptions.RemoveEmptyEntries);

if (parts.Length == 4 && parts[0] == "cam" && int.TryParse(parts[1], out var camera) && camera >= 0 && camera < _cameras.Count)
{
    // /cam/N/whep/... is forwarded to camera N's WHEP server.
    using var forward = new HttpRequestMessage(new HttpMethod(request.HttpMethod), $"http://127.0.0.1:{FirstWhepPort + camera}/{parts[2]}/{parts[3]}");
    if (request.HasEntityBody)
    {
        using var body = new MemoryStream();
        await request.InputStream.CopyToAsync(body);
        forward.Content = new ByteArrayContent(body.ToArray());
        forward.Content.Headers.TryAddWithoutValidation("Content-Type", request.ContentType);
    }

    using var reply = await _http.SendAsync(forward);
    if (reply.Headers.Location != null)
    {
        // The session URL the player deletes when it leaves, under the same /cam/N prefix.
        response.Headers["Location"] = $"/cam/{camera}{reply.Headers.Location.OriginalString}";
    }

    response.StatusCode = (int)reply.StatusCode;
    await WriteAsync(response, reply.Content.Headers.ContentType?.ToString(), await reply.Content.ReadAsByteArrayAsync());
}
```

The demo's page server also answers `/cameras` with the number of cameras, and the grid page builds one tile per camera with its `play` function — the one above, without the `iceServers` addition:

```javascript
(async () => {
    const count = await (await fetch('/cameras')).json();
    for (let i = 0; i < count; i++) {
        const figure = document.createElement('figure');
        const video = Object.assign(document.createElement('video'), { autoplay: true, muted: true, playsInline: true, controls: true });
        const caption = document.createElement('figcaption');
        caption.dataset.name = caption.textContent = `Camera ${i}`;
        figure.append(video, caption);
        document.getElementById('grid').append(figure);
        play(`/cam/${i}/whep/endpoint`, video, caption).catch(e => caption.textContent = e.message);
    }
})();
```

With the CSS rule `#grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(420px, 1fr)); }` the tiles wrap to the window width.

Two design notes for a real NVR:

- **One pipeline or one per camera.** The demo puts every camera pair in one `MediaBlocksPipeline`, so they start and stop together, and a camera that cannot be read makes `CreateAsync` throw before the start. To restart one camera without touching the others, give each pair its own pipeline — the same pattern the [Multi-Camera RTSP Grid](multi-camera-rtsp-grid.md) guide uses for local rendering.
- **Bandwidth, not CPU, is the budget.** Every viewer of every tile receives the camera's full bitrate. For a grid, point the source at the camera's H.264 sub-stream and keep the main stream for single-camera views and for [recording without re-encoding](rtsp-save-original-stream.md).

## Limits to Know Before Shipping

### Codec: H.264 only

The block's input accepts encoded H.264 only. Raw video, or any other codec, fails to connect and the pipeline does not start: the block never encodes for viewers. An H.265 camera stream is refused for the same reason — most browsers cannot play H.265 over WebRTC anyway. Switch the camera, or its sub-stream, to H.264.

### H.264 profiles and browsers

The camera's stream reaches the browser untouched, so the browser's WebRTC decoder must take the camera's profile. Chromium-based browsers (Chrome, Edge) played a camera's 1080p Main profile H.264 in our tests. A browser whose WebRTC decoder takes only constrained baseline shows no picture for a Main or High profile camera. When one viewer sees an empty tile and another does not, open the page in Chrome or Edge, or switch the camera's stream to the Baseline profile if the camera offers one.

The SDK's own `WHEPSourceBlock` offers H.264 only as constrained baseline with the exact `profile-level-id` `42e016`, which camera streams do not match, so it gets no picture from this block. Use a browser, or a player that sends `level-asymmetry-allowed=1` as browsers do.

### Audio

`AudioInput` takes raw audio and the block encodes it to Opus for each viewer, the one audio codec every browser takes over WebRTC; already-encoded audio (the camera's AAC) fails to connect, so decode it first with `DecodeBinBlock`. A mono camera is sent as two identical channels. Connect the audio only when the camera has it: the WHEP server starts listening only once every connected input has delivered its format, so an audio input that receives nothing keeps it closed. `CreateAsync` turns `AudioEnabled` off for a camera without audio, which is the check the code above makes. Two cameras need `audioEnabled: false` instead: one that lists audio in its stream description but sends none, which keeps the server closed in the same way, and one whose audio codec has no decoder, which fails the pipeline — and in the camera grid above one pipeline carries every camera.

Opus is encoded for each viewer, so every viewer adds an audio encoder; the video, passed through, adds none.

### Listening address

`ServerAddress` takes `http://` addresses only and defaults to `http://127.0.0.1:9090`, which accepts players on this machine only; `http://0.0.0.0:<port>` accepts players from other machines. For a server listening on `0.0.0.0`, `EndpointUrl` names the loopback address — players on other machines use this machine's network address instead.

With the page-server pattern of this guide, keep the WHEP server on `127.0.0.1` and expose only the page server. The demo's page server listens on `localhost`; to show the cameras on other computers, put a reverse proxy (nginx, IIS, Caddy) in front of it, forward the same paths and set the `Host` header to `localhost:<port>`, because the page server answers no other host name.

### HTTPS

Receiving WebRTC needs no HTTPS, so plain `http://` works for viewers on the local network. The WHEP server itself speaks `http://` only; if your deployment needs HTTPS, terminate it in the server or reverse proxy that serves the page.

### NAT, STUN and TURN

Each side of a WebRTC connection gathers its own network candidates, so STUN and TURN are set on each side separately:

- **The server's side:** `StunServer` (`stun://hostname:port`) and `TurnServer` (`turn(s)://username:password@host:port`) in `WHEPServerSinkSettings`. Both are unset by default, which means no STUN server — enough on a local network.
- **The viewer's side:** the `iceServers` list the page passes to `RTCPeerConnection`. It is empty in the page above.

The signaller also announces the server's STUN server in a `Link` header of its answer, but the page server in this guide does not forward that header and the player does not read it, so the page sets its own `iceServers`.

```csharp
var whep = new WHEPServerSinkBlock(new WHEPServerSinkSettings
{
    ServerAddress = "http://127.0.0.1:9100",
    StunServer = "stun://stun.l.google.com:19302",
    TurnServer = "turn://user:password@turn.example.com:3478"
});
```

```javascript
const iceServers = [
    { urls: 'stun:stun.l.google.com:19302' },
    { urls: 'turn:turn.example.com:3478', username: 'user', credential: 'password' }
];
```

### Authentication

`WHEPServerSinkSettings` has no authentication setting: anyone who can reach the WHEP server's address can watch. Keep it on `127.0.0.1` and put your access control — login, session cookie, token check — in the page server that forwards the requests, before the forward.

### Errors the block reports

The block's build fails, with a message through the pipeline's `OnError` event, when `ServerAddress` is not an `http://` address with a port in 1–65535 or when its port is already in use. Give every camera its own free port.

### Platforms

Windows x64 with `VisioForge.CrossPlatform.Core.Windows.Adds.x64`, macOS with `VisioForge.CrossPlatform.Core.macOS.Adds`, Linux x64/arm64 with `VisioForge.CrossPlatform.Core.Linux.Adds.x64`/`.arm64` and the distribution's `gstreamer1.0-plugins-bad` and `gstreamer1.0-nice`. Check `WHEPServerSinkBlock.IsAvailable()` at startup, as the demo does.

## Troubleshooting

| Symptom | Cause and fix |
| --- | --- |
| `WHEPServerSinkBlock.IsAvailable()` returns `false` | The platform's `Adds` package is not referenced, or on Linux the distribution packages listed in [Prerequisites](#prerequisites) are missing. |
| `CreateAsync` throws `InvalidOperationException` ("Unable to read RTSP stream info") | The camera is unreachable, or the URL or credentials are wrong. Open the same URL in another RTSP client to check it. |
| `StartAsync` returns `false` | Read the `OnError` messages: a port already in use, an invalid `ServerAddress`, or a camera stream that is not H.264 (H.265, MJPEG). |
| The video plays, no sound | The `<video>` element starts muted: unmute it with its controls. If the camera sends no audio, `CreateAsync` turned `AudioEnabled` off and no audio was connected; check the camera's audio setting. |
| The page shows `HTTP 502` with the audio connected | The audio input received nothing, so the WHEP server never started listening. Connect `AudioInput` only for a camera that sends audio; for a camera that lists audio but sends none, create the source with `audioEnabled: false`. |
| The page shows a `fetch` error such as `Failed to fetch` | The page and the endpoint are on different origins. Serve the page from the server that forwards `/whep/`, never from disk or from another port. |
| The page shows `HTTP 404` or `HTTP 502` | The page server did not forward the path, or the WHEP server is not running on the forwarded port. |
| The caption stays at `connecting`, then `failed` | The browser cannot reach the application machine over WebRTC. Check the firewall on that machine; for viewers outside the local network set STUN/TURN on both sides — `StunServer` and `TurnServer` for the server, `iceServers` in the page. |
| The caption says `connected`, the tile stays black | The browser's decoder does not take the camera's H.264 profile. Try Chrome or Edge, or the camera's Baseline profile. |
| No `ConsumerRemoved` for a viewer that is gone | The page sends its `DELETE` on `pagehide`; a browser that crashes or loses the network does not send it. Viewers still connected when the pipeline stops raise no event. |
| `HttpListener.Start` throws `HttpListenerException` | The page port is taken, or the prefix is not `localhost` and needs administrator rights. Choose another port or keep `localhost`. |

## WHEP Server, WebRTC, WHIP or HLS?

- **`WHEPServerSinkBlock`** (this guide) — the source is already H.264 (an IP camera) and the viewers are browsers on your network. Video passthrough with no encoder, audio encoded to Opus, no media server.
- **[`WebRTCSinkBlock`](../Sinks/index.md#webrtc)** — the video is raw or processed in the pipeline (a webcam, the screen, overlays, analytics). It encodes for each viewer (VP8 video, Opus audio) with per-viewer congestion control, at the cost of an encoder per viewer, and uses the `webrtcsink` WebSocket signalling protocol instead of WHEP.
- **[`WHIPSinkBlock`](../Sinks/index.md#whip)** — you already run a WebRTC media server (MediaMTX, Janus, Cloudflare Stream) and want the application to push one stream to it; the server then handles the viewers. Requires H.264 video and Opus audio; runs on Windows, macOS and Linux.
- **[HLS](../Sinks/index.md#hls)** — viewers on the open internet through a CDN or a plain `<video>` player, when a delay of several seconds is acceptable. See also [HLS streaming](../../general/network-streaming/hls-streaming.md) and the [ASP.NET Web Forms HLS player](aspnet-webforms-hls-player.md) guide.

## Frequently Asked Questions

### Can a browser play an RTSP stream directly?

No. Browsers do not implement RTSP. The stream has to reach the page over a protocol the browser supports — WebRTC, as in this guide, or HLS. With `WHEPServerSinkBlock` the camera's H.264 is carried over WebRTC without being converted.

### Does the application re-encode the video?

No. `RTSPRAWSourceBlock` outputs the camera's encoded H.264 and `WHEPServerSinkBlock` sends it to every viewer as it arrives. That is why the video costs almost no CPU however many viewers watch it, and also why the camera must send H.264 in a profile the browser can decode. Only the audio is re-encoded, to Opus.

### Do I need a media server such as MediaMTX or Janus?

No. The WHEP server runs inside your .NET process. You need a media server only if you want one to handle the viewers, for example on the internet — then push the stream to it with `WHIPSinkBlock`.

### How does the camera's audio reach the browser?

Browsers receive Opus over WebRTC, and IP cameras send AAC or G.711, so the audio cannot be passed through as the video is. Create the source with `audioEnabled: true`, decode `RTSPRAWSourceBlock.AudioOutput` with `DecodeBinBlock` and connect the decoder to `WHEPServerSinkBlock.AudioInput`: the block encodes it to Opus for each viewer. The page offers an audio transceiver and plays both tracks in one `<video>` element, which starts muted until the viewer unmutes it.

### Can I play the stream with something other than a browser?

Yes, any WHEP player can post an offer to the endpoint. The exception is the SDK's own `WHEPSourceBlock`, which offers only constrained baseline `42e016` and gets no picture from a camera stream.

### How many cameras and viewers can one application serve?

There is no per-viewer video encoding — only an Opus encoder per viewer for the audio — so the limit is network bandwidth: each viewer of each camera receives that camera's full bitrate. Use the cameras' sub-streams for grid views.

## See Also

- [Sink blocks → WHEP server](../Sinks/index.md#whep-server) — `WHEPServerSinkBlock` and `WHEPServerSinkSettings` reference
- [Multi-Camera RTSP Grid](multi-camera-rtsp-grid.md) — a camera wall rendered in a desktop application instead of a browser
- [Save RTSP Stream Without Re-encoding](rtsp-save-original-stream.md) — record the same H.264 stream to MP4 alongside the live view
- [RTSP camera source](../../videocapture/video-sources/ip-cameras/rtsp.md) — RTSP URLs, transport and camera settings
- [ONVIF IP camera integration](../../videocapture/video-sources/ip-cameras/onvif.md) — discover cameras and find their sub-stream URLs
- [RTSP reconnection and fallback switch](../../general/network-sources/reconnection-and-fallback.md) — keep a camera's pipeline running through drops
