---
title: SDK Telemetry and Data Collection in the .NET SDKs
description: What the VisioForge .NET SDKs report while a debugger is attached, what is never collected, and how to switch telemetry off.
sidebar_label: Telemetry and Privacy
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
primary_api_classes:
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline

---

# Telemetry and Privacy

Every engine exposes a `Debug_Telemetry` property. When it is `true` **and a debugger is attached to your process**, the SDK reports its own errors to VisioForge, so that failures you hit while integrating the SDK reach us without anyone having to file a ticket.

Both conditions are required, and both are checked every time an event would be sent:

- `Debug_Telemetry` is `true` (the default).
- `System.Diagnostics.Debugger.IsAttached` is `true`.

An application you build and ship to your users runs without a debugger, so **a released application sends nothing**. Detaching the debugger stops reporting immediately; attaching one mid-session starts it.

## Switching it off

Set the property to `false` before you start the engine:

```csharp
// Any of the engines - VideoCaptureCoreX, VideoCaptureCore, MediaPlayerCoreX,
// MediaPlayerCore, VideoEditCoreX, VideoEditCore, SimplePlayerX.
core.Debug_Telemetry = false;
```

```csharp
var pipeline = new MediaBlocksPipeline();
pipeline.Debug_Telemetry = false;
```

The property can be set at any point; the state at the moment an error occurs is what counts.

## What is sent

- The class and method name and the level of the log event.
- The log message, with credentials, file paths, host names, IP and MAC addresses, device identifiers, GUIDs and e-mail addresses removed.
- The exception type, its redacted message, and stack frames. A frame keeps the method name and the line number; the source file path is stripped, because that is your build machine's directory layout.
- Up to 50 preceding log lines as breadcrumbs, redacted the same way.
- The SDK version, .NET runtime, OS description, process architecture, target framework, engine name, and a random identifier that is regenerated on every process start.

## What is never sent

- Your pipeline or settings configuration - nothing enumerates the blocks you built or the settings you gave them.
- File names or paths.
- Device identifiers - PnP instance paths, DirectShow monikers and GUIDs.
- User identity, in any form.
- Environment variables.
- Media content - no frames, no samples, no snapshots.
- Any identifier that survives a restart. The random identifier exists only to group the events of one debugging session, and it is gone when the process ends.

One thing that list does not promise: an error message often names the element or codec it is about - "Failed to link h264parse to qtmux" is a typical one - so element and codec names do appear inside reported messages. What is never sent is a description of your configuration; what is sent is the text of the error itself.

The payload carries no user object and no IP address. The connection itself necessarily reveals your address to the server, which is configured to discard it rather than store it with the event.

## How much is reported

The SDK's own log level decides what can be reported at all:

- With `Debug_Mode` **off**, the SDK logs errors only, so errors are all that can be sent and the breadcrumb list is empty.
- With `Debug_Mode` **on**, the SDK logs from `Debug` upwards. Warnings and errors are reported; debug and informational lines are kept only as breadcrumbs attached to the next error.

## Where it goes

Reports are sent over HTTPS to `https://telemetry.visioforge.org`, a server operated by VisioForge. Warnings are queued and delivered by a background thread, so logging one costs the calling thread nothing. An error is different: the thread that logged it waits up to 300 milliseconds for the send to finish, because "Stop Debugging" kills the process and the last error is the one worth having. Past that budget the thread carries on and the request finishes in the background. If the network is unavailable, or the send rate cap is reached, the report is dropped rather than retried indefinitely.

The first time a process sends anything, the SDK writes one line naming what is sent, where, and how to turn it off - both to the log and to the debug output.

## See also

- [Sending logs](sendlogs.md) - collecting the full debug log for support
- [Privacy policy](https://www.visioforge.com/privacy-policy) - the company-wide policy this feature falls under
