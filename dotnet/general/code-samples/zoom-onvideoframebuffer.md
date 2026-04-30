---
title: Zoom Effects in C# .NET Video Apps with Pan Control
description: Apply zoom and pan effects in C# / .NET with VideoEffectZoom and VideoEffectPan — runtime-adjustable focal point for capture, playback, and editing.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoEffectZoom
  - VideoEffectPan

---

# Implementing Zoom Effects in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Zoom and pan are a built-in video effect in the VisioForge classic (Windows / DirectShow) engines — `VideoCaptureCore`, `MediaPlayerCore`, and `VideoEditCore`. The `VideoEffectZoom` and `VideoEffectPan` classes handle scaling, centering, and runtime adjustments without touching the frame buffer directly. This is the recommended path when you want zoom.

You only need to drop down to `OnVideoFrameBuffer` when `VideoEffectZoom` cannot express what you need — for example, a custom non-affine warp or integration with an external image library.

## Applying VideoEffectZoom (recommended)

`VideoEffectZoom` gets added to the pipeline once and can be tweaked while video is playing. It scales every frame automatically — no per-frame C# code.

```cs
using VisioForge.Core.Types.VideoEffects;

var zoomEffect = new VideoEffectZoom(
    zoomX: 2.0,    // 2.0 = 200% horizontal zoom (1.0 = no zoom)
    zoomY: 2.0,    // 2.0 = 200% vertical zoom — keep equal to zoomX for uniform scaling
    shiftX: 0,     // Pixel offset from centre; positive shifts right
    shiftY: 0,     // Pixel offset from centre; positive shifts down
    enabled: true,
    name: "Zoom");

// VideoCapture1 is a VideoCaptureCore instance (same API on MediaPlayerCore / VideoEditCore).
VideoCapture1.Video_Effects_Enabled = true;
VideoCapture1.Video_Effects_Add(zoomEffect);
```

### Adjust zoom at runtime

Keep a reference to the effect and modify its properties while the pipeline is running — the SDK picks up the new values on the next frame:

```cs
zoomEffect.ZoomX = 3.0;
zoomEffect.ZoomY = 3.0;
zoomEffect.ShiftX = 200;   // Pan focus 200 px to the right of centre
zoomEffect.ShiftY = -100;  // And 100 px up
```

Toggling without removing:

```cs
zoomEffect.Enabled = false;   // Bypass on the fly
zoomEffect.Enabled = true;    // Re-enable later
```

### Interpolation quality

`InterpolationMode` defaults to `VideoInterpolationMode.Bilinear`. For sharper results at higher zoom factors set it to a higher-quality mode; for lowest CPU set `NearestNeighbor`.

```cs
zoomEffect.InterpolationMode = VideoInterpolationMode.Bicubic;
```

## Pairing with VideoEffectPan

If you want smooth pan animation across a source that is larger than the output (for example, "Ken Burns" slow zoom over a still image), combine `VideoEffectZoom` with `VideoEffectPan` from the same namespace. Drive both from a timer or animation curve.

## Dropping down to OnVideoFrameBuffer

Implement a custom zoom by hand only when `VideoEffectZoom` can't do what you need — for example, a non-affine warp, per-pixel magnification around the cursor, or integration with a third-party imaging library. You get the raw frame bytes, transform them in place (or into `e.Frame.Data`), and set `e.UpdateData = true` so the modified pixels flow downstream.

```cs
using System.Runtime.InteropServices;

private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Frame.Data     — IntPtr to the pixel buffer
    // e.Frame.DataSize — buffer size in bytes
    // e.Frame.Info.Width / Info.Height / Info.Stride — frame dimensions (RAWBaseVideoInfo)
    // e.Frame.Timestamp — per-frame TimeSpan

    // 1. Read/copy bytes into your own scratch buffer:
    byte[] scratch = new byte[e.Frame.DataSize];
    Marshal.Copy(e.Frame.Data, scratch, 0, e.Frame.DataSize);

    // 2. Apply whatever custom transform you need on the bytes of `scratch`
    //    (resample, warp, composite, etc.). Keep the output size == input size
    //    because the SDK will not negotiate a new resolution mid-pipeline.

    // 3. Write the result back into the original buffer:
    Marshal.Copy(scratch, 0, e.Frame.Data, e.Frame.DataSize);

    // 4. Tell the pipeline we mutated the pixels.
    e.UpdateData = true;
}
```

### X-engine note

On the cross-platform X engines (`VideoCaptureCoreX`, `MediaPlayerCoreX`) the buffer arrives in `VideoFrameXBufferEventArgs`. Flat dimensions live directly on `e.Frame.Width` / `Height` / `Stride`, and frames are typically BGRA. For heavy pixel math, wrap the buffer in `SKPixmap` (SkiaSharp is already a transitive dependency of the X engines).

## Performance Considerations

- Prefer `VideoEffectZoom` over the frame-buffer path — the native scaler is faster and thread-safe.
- Reuse scratch buffers instead of allocating per frame.
- Keep the output resolution equal to the input resolution from the handler — the pipeline does not renegotiate caps mid-stream.
- Offload heavy CV / AI work to a worker thread; return quickly from the event handler to avoid back-pressure.

## Conclusion

For virtually every application, `VideoEffectZoom` (optionally paired with `VideoEffectPan`) is the right tool — it's one line of setup, runtime-adjustable, and implemented in native code. `OnVideoFrameBuffer` remains available for the cases where you genuinely need to own the bytes.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
