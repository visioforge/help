---
title: Initialize and Configure VisioForge .NET Video SDKs
description: Initialize and deinitialize .NET SDKs for video capture, editing, and playback with DirectShow and cross-platform X-engines.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing

---

# Initialization

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Type of SDK engines

All SDKs contain Windows-only DirectShow-based engines and cross-platform X-engines.

### Windows-only engines

- VideoCaptureCore
- VideoEditCore
- MediaPlayerCore

### X-engines

- VideoCaptureCoreX
- VideoEditCoreX
- MediaPlayerCoreX
- MediaBlocksPipeline

X-engines require additional initialization and de-initialization steps.

## SDK initialization and de-initialization for X-engines

All X-engines (`VideoCaptureCoreX`, `VideoEditCoreX`, `MediaPlayerCoreX`, `MediaBlocksPipeline`) use one shared initialization step. You must initialize the SDK before any SDK class usage and de-initialize the SDK before the application exits. The Windows-only DirectShow cores (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) need no initialization step.

### Calling InitSDKAsync

The recommended way is asynchronous, so a cold GStreamer startup (native library resolution, plugin registry scan) does not freeze the calling thread:

```csharp
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

The blocking form is equivalent and may be called from any thread. Use it directly when initialization must run on a specific thread (see the note below):

```csharp
VisioForge.Core.VisioForgeX.InitSDK();
```

`InitSDKAsync` offloads `InitSDK` to a thread-pool thread via `Task.Run`. GStreamer / GLib unhandled-exception handlers are bound to whichever thread first triggered initialization, so if your application relies on init running on a particular thread (for thread-static GLib state), call `InitSDK()` on that thread directly.

### De-initialization

```csharp
VisioForge.Core.VisioForgeX.DestroySDK();
```

`DestroySDK` has no asynchronous variant and may be called from any thread.

If the SDK is not properly deinitialized, the application may experience a hang-on exit due to the inability to finalize one of its threads. This issue arises because the SDK continues to operate, preventing the application from closing smoothly. To ensure a clean exit, it is crucial to deinitialize the SDK appropriately based on the UI framework you are using.

For applications developed using different UI frameworks, you can deinitialize the SDK in the `FormClosing` event or another relevant event handler. This approach ensures that the SDK is properly destroyed before the application closes, allowing for all threads to terminate correctly.

Moreover, the SDK can be destroyed from any thread, providing flexibility in how you manage the deinitialization process. Because `DestroySDK` is synchronous, if you want to keep the user interface responsive call it on a background thread (for example via `Task.Run`) — there is no asynchronous `DestroySDK` overload.

Implementing these practices ensures that your application exits smoothly without hanging, providing a seamless experience for the users. Properly managing the SDK deinitialization is crucial for maintaining the stability and performance of your application.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.