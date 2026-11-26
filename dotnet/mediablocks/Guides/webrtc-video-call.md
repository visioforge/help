---
title: Two-Way WebRTC Video Call with a Browser in C# .NET
description: Build a 1:1 video call between a C# app and a web browser over WebRTC, with echo cancellation, mute and camera off, and no media server.
sidebar_label: Video Call with a Browser (WebRTC)
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - Streaming
  - Webcam
  - WebRTC
  - VP8
  - Opus
  - C#
  - JavaScript
  - NuGet
primary_api_classes:
  - WebRTCSourceBlock
  - WebRTCSourceSettings
  - WebRTCSinkBlock
  - WebRTCSinkSettings
  - EchoCancellationBlock
  - MediaBlocksPipeline

---

# Two-Way WebRTC Video Call Between a C# App and a Browser

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Demo Sample"
    This guide follows the [WebRTC Video Call Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/WebRTC%20Video%20Call%20Demo): a camera and a microphone with echo cancellation, mute and camera off, the caller's video and voice, and a call page served by the application.

    The block reference is in [Source blocks → WebRTC Source](../Sources/index.md#webrtc-source-block), [Sink blocks → WebRTC](../Sinks/index.md#webrtc) and [Audio processing → Echo Cancellation](../AudioProcessing/index.md#echo-cancellation).

## What You Will Build

A video call between a .NET desktop application and a person in a web browser — a help desk agent and a customer, a kiosk and an operator, a lab instrument and a remote expert. Both sides see and hear each other with sub-second latency, and nothing runs between them: no media server, no cloud service, no browser plug-in.

```mermaid
graph LR;
    subgraph app ["Your .NET application"]
        cam["Camera + microphone"] --> EchoCancellationBlock;
        EchoCancellationBlock --> WebRTCSinkBlock;
        WebRTCSourceBlock --> renderers["VideoView + speakers"];
    end
    WebRTCSinkBlock -- "WebRTC: your video and voice" --> browser["Browser<br/>call page"];
    browser -- "WebRTC: the caller's video and voice" --> WebRTCSourceBlock;
```

Three blocks do the work:

| Block | Role in the call |
| --- | --- |
| `WebRTCSinkBlock` | Sends your camera and microphone to the browser. Runs the signalling server and serves the call page. |
| `WebRTCSourceBlock` | Receives the browser's camera and microphone, decoded, for a `VideoRendererBlock` and the speakers. |
| `EchoCancellationBlock` | Removes the caller's voice that your speakers play and your microphone picks up again, so the caller does not hear an echo. Also suppresses noise and levels the microphone. |

## When to Use It

- **One application, one caller at a time**, on the same network or across the internet with a TURN server.
- You want the call inside your own application — next to your controls, recordings or measurements — and the caller to need nothing but a link.
- You do not want to run or pay for a media server.

For one stream to many viewers use `WebRTCSinkBlock` alone ([Screen and Camera to Browsers](webrtc-whip.md)), and to publish through a media server use WHIP and WHEP ([WebRTC Player (WHEP)](webrtc-whep-player.md)).

## Prerequisites

- Windows x64 (the platform the demo and the tests run on), .NET 10.
- Media Blocks SDK .NET 2026.11.26 or later, with the `VisioForge.CrossPlatform.Core.Windows.x64` and `VisioForge.CrossPlatform.Core.Windows.Adds.x64` packages. The `.Adds` package carries the WebRTC and echo cancellation plugins.
- A camera and a microphone, and Chrome, Edge, Firefox or Safari for the caller.

Check `WebRTCSinkBlock.IsAvailable()`, `WebRTCSourceBlock.IsAvailable()` and `EchoCancellationBlock.IsAvailable()` before you build the pipeline: each returns `false` when the `.Adds` package is missing.

## How the Two Sides Find Each Other

The browser and the application meet on the signalling server that `WebRTCSinkBlock` runs (WebSocket, port 8443 by default):

1. `WebRTCSinkBlock` registers as a **producer**: anyone on the server can ask for its stream.
2. `WebRTCSourceBlock`, with neither `ProducerPeerId` nor `ConnectToFirstProducer` set, registers as a consumer that **waits to be called**.
3. The call page registers as a producer and a listener. It asks for the application's stream (one WebRTC session) and calls the waiting `WebRTCSourceBlock` with its own camera and microphone (a second session).

Each direction is its own WebRTC session, so the two can use different codecs and bitrates, and the sink adapts its bitrate to the caller's network.

## The Pipeline

```csharp
var pipeline = new MediaBlocksPipeline();

// Your side: camera and microphone to the browser. The sink serves the call page from wwwroot.
var sink = new WebRTCSinkBlock(new WebRTCSinkSettings
{
    SignallingServerHost = "127.0.0.1",
    SignallingServerPort = 8443,
    WebServerDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
    WebServerAddress = "http://127.0.0.1:8080/",
});

var camera = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(videoDevice));
var microphone = new SystemAudioSourceBlock(audioDevice.CreateSourceSettings());
var echoCancellation = new EchoCancellationBlock(new EchoCancellationSettings());

pipeline.Connect(camera.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(microphone.Output, echoCancellation.Input);
pipeline.Connect(echoCancellation.Output, sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// The caller's side: the renderers wait for the call. The caller's voice passes the echo canceller
// on its way to the speakers, so the canceller knows what to remove from the microphone.
var remoteVideo = new VideoRendererBlock(pipeline, RemoteView) { IsSync = false };
pipeline.AddBlock(remoteVideo);
pipeline.Connect(echoCancellation.PlaybackOutput, new AudioRendererBlock().Input);

await pipeline.StartAsync();

// The source joins the sink's signalling server once it runs, and waits for the call page to call it.
var source = new WebRTCSourceBlock(new WebRTCSourceSettings { SignallingServer = "ws://127.0.0.1:8443" });
source.VideoOutput.OnDemandConnect = true;
source.AudioOutput.OnDemandConnect = true;

pipeline.AddBlockLive(source);
pipeline.ConnectLive(source.VideoOutput, remoteVideo.Input);
pipeline.ConnectLive(source.AudioOutput, echoCancellation.PlaybackInput);
pipeline.SyncBlockStateLive(source);
```

Why the source is added after `StartAsync`: its signalling client connects as soon as the source starts, and the sink's server is listening only once the pipeline runs. `OnDemandConnect = true` keeps the source's outputs open for `ConnectLive`.

`WebRTCSinkBlock.ConsumerAdded` and `ConsumerRemoved` tell you when the caller joins and leaves. When the caller hangs up and calls again, the same `WebRTCSourceBlock` takes the new call through the same outputs; nothing needs to be rebuilt.

## The Call Page

The demo's `wwwroot/index.html` is a single file of plain JavaScript, no libraries. The parts that matter:

```javascript
// Register as a producer (to call the application) and a listener (to see it arrive).
// callPage marks call pages: the 'list' and 'peerStatusChanged' handlers skip producers whose
// meta has it, so two open pages never take each other for the application.
send({ type: 'setPeerStatus', roles: ['producer', 'listener'], meta: { callPage: true } });
send({ type: 'list' });          // the application's sink: receive its stream
send({ type: 'listConsumers' }); // the application's waiting source: call it

// list → startSession with the sink's peer ID: the sink sends an offer, the page answers.
// listConsumers → startSession with the source's peer ID: the server replies with
// startSession, and the page offers its camera and microphone.
```

The page opens one `RTCPeerConnection` for each session and routes every `peer` message by its `sessionId`. Copy the file as is, or use it as a reference for your own page.

## Mute and Camera Off

- **Mute**: put a `VolumeBlock` before the echo canceller and set `Mute = true`. The call keeps its audio track; the caller hears silence.
- **Camera off**: put a `VideoBalanceBlock` after the camera and set `Settings.Contrast = 0`, `Settings.Saturation = 0` and `Settings.Brightness = -1`. The caller sees a black picture, and the video track stays, so turning the camera back on needs no new negotiation.

Both take effect while the call runs. In the browser, `track.enabled = false` does the same on the caller's side.

## Limits

- **HTTPS outside localhost.** Browsers give a page the camera and microphone only on `https://` or on `localhost`/`127.0.0.1`, and the sink's built-in web server is `http://` only. For a caller on another computer, host the call page on your own `https://` site, leave `WebServerDirectory` empty, and give the sink `SignallingServerCertificate` and `SignallingServerKey` so the page reaches the signalling server over `wss://`. Set `SignallingServerHost` to `0.0.0.0` so the page can connect at all, and, since the server then takes `wss://` only, set `WebRTCSourceSettings.SignallingServer` to `wss://127.0.0.1:<port>` (the source does not check the server's certificate, so the site's host name in it does not matter here).
- **TURN across the internet.** On one local network the two sides connect directly. Behind NAT or a firewall, run a TURN server (for example coturn) and set it in `WebRTCSinkSettings.TurnServers` and `WebRTCSourceSettings.TurnServers` (`turn://user:password@host:3478`) and in the page's `iceServers` (`{ urls: 'turn:host:3478', username, credential }`). STUN alone (`StunServer`, Google's server by default) is not enough for symmetric NAT.
- **One caller at a time.** `WebRTCSourceBlock` takes one call at a time: a 1:1 call, not a conference. A second caller who calls during a call receives your picture but is not shown, even after the first caller hangs up.
- **Echo cancellation needs both paths.** With `PlaybackInput` left unconnected the canceller has nothing to compare against and removes nothing. Both paths run at 48 kHz; the canceller resamples.
- **Windows is the tested platform.** The same plugins ship in `VisioForge.CrossPlatform.Core.macOS.Adds`; on other platforms check `IsAvailable()`.

## Frequently Asked Questions

### Do I need a media server or a cloud service for a WebRTC call from .NET?

No. `WebRTCSinkBlock` runs the signalling server inside your application and serves the call page, and the media flows directly between the application and the browser. Only a TURN server is needed when the two sides cannot reach each other directly.

### How does the browser call the .NET application?

The call page registers on the application's signalling server as a producer and starts a session with the waiting `WebRTCSourceBlock`. The server tells the page to send its offer, and the source answers. A second session in the other direction carries the application's camera and microphone.

### Why does the caller hear an echo, and how is it removed?

The caller's voice comes out of your speakers and goes back into your microphone. `EchoCancellationBlock` listens to what the speakers play (its playback path) and subtracts it from the microphone (its main path). Connect both paths; headphones avoid the echo altogether.

### Which codecs are used?

The browser and the blocks negotiate them per session: VP8, H.264, VP9, H.265 or AV1 for video, whichever both sides support, and Opus for audio. The video the source outputs is decoded, in the size and frame rate the browser sends.

### Can the call run across the internet?

Yes, with the call page on an `https://` site, the signalling server on `wss://`, and a TURN server for callers behind NAT. See [Limits](#limits).

## See Also

- [Screen and Camera to Browsers (WebRTC, WHIP)](webrtc-whip.md) — one stream to many browsers
- [WebRTC Player (WHEP)](webrtc-whep-player.md) — play a stream from a WebRTC media server
- [Source blocks → WebRTC Source](../Sources/index.md#webrtc-source-block)
- [Audio processing → Echo Cancellation](../AudioProcessing/index.md#echo-cancellation)
