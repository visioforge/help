---
title: .NET SDK Setup and Configuration Guide
description: Learn how to properly initialize and deinitialize .NET SDKs for video capture, editing, and media playback. Includes code examples for both Windows-only and cross-platform X-engines, with best practices for clean application exit.
sidebar_label: Initialization
order: 20

---

# Initialization

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

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

You need to initialize SDK before any SDK class usage and de-initialize SDK before the application exits.

To initialize SDK, use the following code:

```csharp
VisioForge.Core.VisioForgeX.InitSDK();
```

To de-initialize SDK, use the following code:

```csharp
VisioForge.Core.VisioForgeX.DestroySDK();
```

If the SDK is not properly deinitialized, the application may experience a hang-on exit due to the inability to finalize one of its threads. This issue arises because the SDK continues to operate, preventing the application from closing smoothly. To ensure a clean exit, it is crucial to deinitialize the SDK appropriately based on the UI framework you are using.

For applications developed using different UI frameworks, you can deinitialize the SDK in the `FormClosing` event or another relevant event handler. This approach ensures that the SDK is properly destroyed before the application closes, allowing for all threads to terminate correctly.

Moreover, the SDK can be destroyed from any thread, providing flexibility in how you manage the deinitialization process. To enhance the user experience and prevent the UI from freezing during this process, you can utilize asynchronous API calls. By using async methods, you allow the deinitialization to occur in the background, keeping the user interface responsive and avoiding any potential lag or freezing issues.

Implementing these practices ensures that your application exits smoothly without hanging, providing a seamless experience for the users. Properly managing the SDK deinitialization is crucial for maintaining the stability and performance of your application.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
