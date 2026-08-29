---
title: Video Editing in .NET MAUI — Android, iOS, Windows
description: Join and render video on Android, iOS, macCatalyst and Windows from one .NET MAUI codebase with the VideoEditCoreX timeline engine.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Android
  - iOS
  - MAUI
  - Editing
  - Encoding
  - MP4
  - H.264
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoView
  - IVideoView
  - MP4Output
  - ProgressEventArgs

---

# Video Editing in .NET MAUI Apps

## Introduction

`VideoEditCoreX` is the cross-platform timeline engine of the Video Edit SDK .NET. It runs on
the GStreamer backend, so the same editing code compiles and runs on Android, iOS, macCatalyst
and Windows from a single .NET MAUI project — the preview surface is the shared `VideoView`
control, and the timeline API is identical on every target.

This guide builds the smallest useful editor: pick clips from the device gallery, append them to
the timeline, preview the result, then render it to an MP4 file.

The full sample is on [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit).

## Prerequisites

- .NET MAUI workloads for the platforms you target (`maui-android`, `maui-ios`, `maui-maccatalyst`).
- A VisioForge licence, or the 30-day trial.
- On iOS and macCatalyst, an `NSPhotoLibraryUsageDescription` string in `Info.plist` — the
  gallery picker is what feeds the timeline.

## NuGet Packages

Two packages: the SDK itself and the MAUI `VideoView` control.

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.8.16" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.8.16" />
```

`VisioForge.DotNet.Core.UI.MAUI` only supplies the `VideoView` control — on its own it cannot
compile `VideoEditCoreX`. The `VisioForge.DotNet.VideoEdit` package is what brings the engine in.

Then add the redistributable for each platform you build for. See
[Installing in MAUI apps](../../install/maui.md) for the full per-platform breakdown, including the
Android Java bindings project reference, which Android builds require.

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.7.27" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <!-- The iOS redist version trails the SDK version on purpose - it tracks the
       GStreamer-iOS rebuild cadence, not the wrapper release. -->
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.12.0" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
```

## MauiProgram.cs

Register the VisioForge handlers so `VideoView` resolves to its native implementation:

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

## XAML Layout

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI">
    <Grid RowDefinitions="*,Auto">
        <my:VideoView Grid.Row="0" x:Name="videoView" Background="Black"
                      HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" />
        <VerticalStackLayout Grid.Row="1">
            <ProgressBar x:Name="pbProgress" />
            <Button x:Name="btAdd" Text="ADD CLIP" Clicked="btAdd_Clicked" />
            <Button x:Name="btPreview" Text="PREVIEW" Clicked="btPreview_Clicked" />
            <Button x:Name="btRender" Text="RENDER MP4" Clicked="btRender_Clicked" />
        </VerticalStackLayout>
    </Grid>
</ContentPage>
```

## Creating the Engine

`AddVisioForgeHandlers` registers the control. It does **not** load the native stack — that is a
separate call, and it must happen before the first `VideoEditCoreX` is constructed. Skip it and
the constructor throws `DllNotFoundException` on a clean machine.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.VideoEditX;

private VideoEditCoreX _core;

private async void MainPage_Loaded(object sender, EventArgs e)
{
    // Loads the native GStreamer stack. The first call builds the plugin registry and
    // can take hundreds of milliseconds, so use the async form off the UI thread.
    await VisioForgeX.InitSDKAsync();

    IVideoView vv = videoView.GetVideoView();
    _core = new VideoEditCoreX(vv);

    _core.OnError += Core_OnError;
    _core.OnProgress += Core_OnProgress;
    _core.OnStop += Core_OnStop;
}
```

!!! warning "Keep the controls dead until the engine exists"
    `InitSDKAsync` runs on the thread pool and a cold start spends hundreds of milliseconds
    building the GStreamer registry. `MainPage_Loaded` is `async void`, so the page is
    interactive for that whole window and every handler that touches `_core` would hit a null
    reference. Disable the control panel in the constructor and enable it once the engine is
    constructed, and wrap the body in a `try`/`catch` so a failed init leaves the controls off
    with the reason on screen. The sample does both.

Release it on the way out:

```csharp
private void MainPage_Unloaded(object sender, EventArgs e)
{
    _core?.Stop();
    _core?.Dispose();
    _core = null;

    VisioForgeX.DestroySDK();
}
```

## Building the Timeline

Each `Input_Add*` call appends to the end of the timeline. Use
`Input_AddAudioVideoFile` for a normal clip, `Input_AddAudioFile` for an audio-only track and
`Input_AddImageFile` for a still with an explicit duration.

```csharp
private async void btAdd_Clicked(object sender, EventArgs e)
{
    var picked = await MediaPicker.Default.PickVideoAsync();
    if (picked == null)
    {
        return;
    }

    _core.Input_AddAudioVideoFile(await ResolveLocalPathAsync(picked));
}
```

`Input_Clear_List()` empties the timeline.

!!! warning "The picker does not always hand you a file path"
    On Android, `MediaPicker` copies the asset into a cache folder, so `FullPath` is absolute and
    usable as is. On iOS, `PHPicker` hands MAUI an `NSItemProvider` and only the original file
    name survives in `FullPath` — the stream has to be copied into your own cache first. The
    sample's `ResolveLocalPathAsync` covers both cases.

## Preview and Render

The engine has one switch between the two modes: `Output_Format`. A null value plays the
timeline into the `VideoView`; a format object renders it to a file.

```csharp
private void btPreview_Clicked(object sender, EventArgs e)
{
    _core.Output_Format = null;
    _core.Start();
}

private void btRender_Clicked(object sender, EventArgs e)
{
    var output = Path.Combine(FileSystem.Current.AppDataDirectory, "joined.mp4");

    _core.Output_Format = new MP4Output(output, H264EncoderBlock.GetDefaultSettings());
    _core.Start();
}
```

!!! warning "`new MP4Output(path)` alone gives you software H.264 off macOS and Android"
    `MP4Output`'s built-in default picks `AppleMediaH264EncoderSettings` on macOS and calls
    `H264EncoderBlock.GetDefaultSettings()` on Android — but iOS, Mac Catalyst and Windows all
    fall through to `OpenH264EncoderSettings`, a software encoder.

    `H264EncoderBlock.GetDefaultSettings()` is the SDK's own platform selector and prefers
    hardware everywhere: VideoToolbox on Apple, MediaCodec on Android, then NVENC, AMF and QSV
    before falling back to software. Pass it explicitly to keep iOS, Mac Catalyst and Windows on hardware.

    It lives in the `VisioForge.Core.MediaBlocks.VideoEncoders` namespace, but it is a shared
    helper rather than a Media Blocks pipeline — `MP4Output` itself calls it.

    Leave the audio encoder unset: `MP4Output` picks AAC on Windows and MP3 elsewhere.

Write to a per-app directory such as `FileSystem.Current.AppDataDirectory`. It needs no runtime
permission on any platform, unlike the shared media store on Android.

## Progress and Completion

`OnProgress` reports 0–100 and `OnStop` reports success. Both fire on a background thread, so
marshal to the UI thread before touching controls:

```csharp
private void Core_OnProgress(object sender, ProgressEventArgs e)
{
    MainThread.BeginInvokeOnMainThread(() => pbProgress.Progress = e.Progress / 100.0);
}

private void Core_OnStop(object sender, StopEventArgs e)
{
    MainThread.BeginInvokeOnMainThread(() =>
        DisplayAlert("Render", e.Successful ? "Completed" : "Failed", "OK"));
}
```

## Platform Notes

- **Android** needs the Java bindings project reference. Without it the app builds but the native
  stack fails to initialise at runtime.
- **iOS and macCatalyst** need `<UseInterpreter>true</UseInterpreter>`. MAUI's XAML type
  conversion uses `DynamicMethod`, which requires JIT and throws
  `ExecutionEngineException` in AOT-only mode.
- **Windows** builds resolve the redists through `VisioForge.CrossPlatform.Core.Windows.x64`.

## Sample Applications

- **[SimpleEdit (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit)** — the app this guide builds.
- **[Video Join Demo X (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/WPF/CSharp/Video%20Join%20Demo%20X)** — the same timeline API on the desktop, with per-format output options.

## See Also

- [Installing in MAUI apps](../../install/maui.md)
- [Video Edit SDK cheat sheet](../cheat-sheet.md)
- [Transitions](../transitions.md)
- [Android deployment](../../deployment-x/Android.md)
- [iOS deployment](../../deployment-x/iOS.md)
