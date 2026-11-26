---
title: Phone Camera Zoom, Torch, Focus and Exposure in C#
description: Control the iOS and Android camera from C# .NET - 0.5x ultra-wide zoom, torch, tap to focus and manual exposure with VisioForge Media Blocks SDK.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Android
  - iOS
  - Capture
  - Webcam
  - C#
  - NuGet
primary_api_classes:
  - SystemVideoSourceBlock
  - ICamera2Controls
  - Camera2Capabilities
  - DeviceEnumerator
---

# Camera Zoom, Torch, Focus and Exposure on iOS and Android

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

`SystemVideoSourceBlock` gives a running phone camera one control interface on both platforms: `Camera2Controls` (`ICamera2Controls`) changes zoom, torch, focus, exposure, white balance and stabilization while the pipeline runs, and `Capabilities` (`Camera2Capabilities`) says what the open camera supports. On Android the members drive the Camera2 API; on iOS they drive the `AVCaptureDevice` the source opened. The same MAUI or iOS/Android code runs on both.

## Get the controls

Read them after the pipeline has started the camera: before that `Camera2Controls` is `null`, and on iOS `Capabilities` too. On iOS each start creates new controls and puts the camera back in its opening state, as Android does - automatic focus, exposure and white balance at the centre of the frame, no compensation, zoom 1.0 on a multi-camera device and a single lens at its widest, the torch off - so read `Camera2Controls` and subscribe to its events again after every `StartAsync`.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Sources.AndroidCamera2;

var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var device = devices[0];

var format = device.GetHDVideoFormatAndFrameRate(out var frameRate);
var settings = new VideoCaptureDeviceSourceSettings(device) { Format = format.ToFormat() };
settings.Format.FrameRate = frameRate;

var videoSource = new SystemVideoSourceBlock(settings);
// ... connect the source to a renderer or an encoder, then:
await pipeline.StartAsync();

ICamera2Controls controls = videoSource.Camera2Controls;
Camera2Capabilities caps = videoSource.Capabilities;
```

## Zoom and the 0.5x ultra-wide lens

`SetZoom` takes the ratio on the scale the iPhone Camera app shows, from `MinZoom` to `MaxZoom`; a value outside is clamped.

```csharp
controls.SetZoom(controls.MinZoom);   // the widest view the camera has
controls.SetZoom(2.0f);               // 2x
```

- **Android:** `MinZoom` is the camera's own minimum: below 1.0 on Android 11 and later where the camera reaches its ultra-wide lens through the zoom ratio, otherwise 1.0. `MaxZoom` is the camera's maximum zoom.
- **iOS:** on a multi-camera device with an ultra-wide lens - "Back Triple Camera" or "Back Dual Wide Camera" - `MinZoom` is 0.5: 0.5 is the ultra-wide lens, 1.0 the wide lens, and iOS switches to the telephoto lens on its own as the zoom passes it. On "Back Camera" and "Front Camera" it is 1.0, and a lens opened on its own reports where it sits on that scale, so read `MinZoom` rather than assuming 1.0. A multi-camera device opens on the wide lens, 1.0, as the Camera app does.

On iOS, `VideoSourcesAsync` lists "Back Camera" and "Front Camera" first, as before, and then the other back cameras the phone has: the ultra-wide, the telephoto, and the multi-camera devices ("Back Dual Wide Camera", "Back Triple Camera", "Back Dual Camera"). Their names come from iOS and follow the phone's language, so let the user pick from the list rather than matching a name.

## Torch

```csharp
if (caps.HasFlashUnit)
{
    controls.SetTorchMode(true);
}
```

## Focus

```csharp
controls.SetAfMode(Camera2AfMode.ContinuousVideo);

// Tap to focus: a region in the sensor frame, here 100 x 100 around a point
// given as 0-1 fractions (fx, fy) of that frame.
var sensor = caps.SensorInfo;
var x = (int)(fx * sensor.ActiveWidth) - 50;
var y = (int)(fy * sensor.ActiveHeight) - 50;
controls.SetAfRegions(new[] { new Camera2MeteringRegion(x, y, 100, 100, 1000) });
controls.TriggerAutoFocus();

// Manual focus: 0 is the furthest distance, caps.LensInfo.MinimumFocusDistance the closest.
controls.SetAfMode(Camera2AfMode.Off);
controls.SetFocusDistance(caps.LensInfo.MinimumFocusDistance / 2);
```

The sensor frame is the camera's own landscape frame on both platforms, not the rotated preview: convert a tap on a portrait preview before you build the region. `FocusStateChanged` reports the scan. On iOS:

- the centre of the first region is the focus point, and the weight is not used;
- `SensorInfo.Orientation` is 0, and the sensor frame is the landscape frame with the Home button side on the right, whatever way the phone is held. A tap at (fx, fy), as 0-1 fractions of a portrait preview of the back camera that shows the whole frame (remove the crop of an aspect-fill preview first), is at (fy, 1 - fx) of the sensor frame; for a mirrored front-camera preview, mirror fx first;
- `FocusStateChanged` says when a scan ends, not whether it found focus, so `IsFocused` is not a measurement there;
- the focus distance is uncalibrated - it is in order, but not in metres;
- `MinimumFocusDistance` is 0 when the camera cannot set its lens position.

## Exposure

```csharp
// One stop darker; a step is caps.ExposureCompensationStep stops (a third of a stop on iOS).
controls.SetExposureCompensation((int)Math.Round(-1 / caps.ExposureCompensationStep));

// Exposure point, the same region form as the focus.
controls.SetAeRegions(new[] { new Camera2MeteringRegion(x, y, 100, 100, 1000) });

// Manual exposure: 1/120 s at ISO 400, clamped to caps.SensorInfo's ranges; on iOS also to one frame
// at the configured rate, so the frame rate stays.
if (caps.ManualSensorSupported)
{
    controls.SetManualExposure(1_000_000_000L / 120, 400);
}

// Back to automatic exposure.
controls.LockAutoExposure(false);
```

`ExposureStateChanged` reports when the exposure is searching, converged or locked. A locked or manual exposure stays locked on iOS, as on Android: `SetAeRegions` takes effect once the exposure is automatic again.

## What iOS does not support

The interface is the same on both platforms, and a call iOS has no control for is ignored. `Capabilities` reports it as unsupported, so check it rather than the platform.

| Member | Android | iOS |
|---|---|---|
| `SetAwbMode` | the Camera2 AWB modes, presets included | `Auto`, and `Off` to keep the current gains |
| `SetAwbRegions` | yes | no (`MaxAwbRegions` is 0) |
| `SetAeMode` | the Camera2 AE modes | `On`, and `Off` to keep the current exposure |
| `SetFlashMode(Single)` | yes | no, video uses the torch |
| `SetVideoStabilization` | `Off`, `On`, `PreviewStabilization` | `Off`, and `On` as the standard stabilization |
| `SetNoiseReduction`, `SetEdgeEnhancement`, `SetOIS`, `SetFaceDetection` | yes | no |
| `Capabilities.HardwareLevel` | the Camera2 hardware level | not used; check `ManualSensorSupported` |
| `Capabilities` | once the block is built | once the camera is open |

## See also

- [Media Blocks SDK sources](../Sources/index.md)
- [Device enumeration](../GettingStarted/device-enum.md)
- [Code samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)
