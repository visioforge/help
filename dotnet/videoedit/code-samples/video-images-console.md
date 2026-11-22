---
title: Make a Video from Images with Transitions in C# .NET
description: Turn a folder of JPEG images into a 1080p MP4 slideshow with a crossfade, wipe transitions and a fade in and out, in a C# console app with Video Edit SDK .NET.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - Editing
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoTransition
  - VideoTransitionType
  - MP4Output
  - StopEventArgs
video:
  id: Kef_q2cvdAI
  upload_date: 2026-11-14T07:54:56Z
  duration: PT9M17S

---

# Make a Video from Images with Transitions in C\#

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

This tutorial builds a C# console app that turns a folder of JPEG images into one MP4 video with [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net) and its `VideoEditCoreX` engine. Each image stays on screen for 3 seconds, and the next one starts 1 second before it ends; in that second a transition takes one image to the next: a crossfade first, then a bar wipe, an iris, a clock wipe and a barn door. The video fades in from black and out to black, and comes out 1920x1080 at 30 frames per second in H.264.

`VideoEditCoreX` is the SDK's GStreamer engine. This app is built for Windows x64; for macOS and Linux, the engine's native packages are in the [cross-platform deployment guide](../../deployment-x/index.md).

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## YouTube tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Kef_q2cvdAI?controls=1" title="Make a video from images with transitions in C# - YouTube tutorial" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-console)

The images in the video are frames of [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

The video creates the project on .NET 10, which needs Visual Studio 2026; in Visual Studio 2022, choose .NET 8 or .NET 9.

### The short version

The same task in a few lines of C#, with a crossfade between the images:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/_jgqREj_xdo?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Its code: [images-to-video-short-console on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-short-console).

## Required NuGet packages

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/) — the SDK with `VideoEditCoreX`
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — GStreamer and the native libraries the engine runs on, among them the H.264 encoder

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="[2026.11.21]" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.16]" />
```

## The project file

The Visual Studio console template targets `net10.0`, and for that target framework the SDK package resolves its Linux build. Add `-windows` to the target framework (`net8.0-windows` or `net9.0-windows` on Visual Studio 2022), and set `PlatformTarget` to `x64`, because the native package is x64:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

## The code

`Program.cs`, with top-level statements. The project has `ImplicitUsings` enabled, so `System`, `System.IO`, `System.Linq` and `System.Threading.Tasks` are not listed.

```cs
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// The images, in file-name order, and where the video goes.
var folder = args.Length > 0 ? args[0] : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "slides");
var images = Directory.Exists(folder) ? Directory.GetFiles(folder, "*.jpg").Order().ToArray() : [];
var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "slideshow.mp4");
if (images.Length == 0)
{
    Console.WriteLine($"No .jpg files in {folder}");
    return;
}

await VisioForgeX.InitSDKAsync();

var edit = new VideoEditCoreX();
var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
edit.OnError += (s, e) => Console.WriteLine($"\n{e.Message}");
edit.OnProgress += (s, e) => Console.Write($"\rRendering: {e.Progress}%");
edit.OnStop += (s, e) => done.TrySetResult(e.Successful);

// Each image is on screen for 3 seconds, and the next one starts 1 second before it ends.
var show = TimeSpan.FromSeconds(3);
var overlap = TimeSpan.FromSeconds(1);
VideoTransitionType[] transitions =
[
    VideoTransitionType.Crossfade,
    VideoTransitionType.BarWipeLr,
    VideoTransitionType.IrisRect,
    VideoTransitionType.SinglesweepCwt,
    VideoTransitionType.BarndoorV,
];

for (int i = 0; i < images.Length; i++)
{
    var start = (show - overlap) * i;
    edit.Input_AddImageFile(images[i], show, start);

    // The transition fills the second where this image and the one before it overlap.
    if (i > 0)
    {
        var type = transitions[(i - 1) % transitions.Length];
        edit.Video_Transitions.Add(new VideoTransition(type, start, start + overlap));
    }
}

// Fade in from black at the start, and out to black at the end.
var total = (show - overlap) * (images.Length - 1) + show;
edit.Video_Transition_Add_FadeIn(TimeSpan.Zero, overlap);
edit.Video_Transition_Add_FadeOut(total - overlap, total);

edit.Output_VideoSize = new Size(1920, 1080);
edit.Output_VideoFrameRate = new VideoFrameRate(30);
edit.Output_Format = new MP4Output(output);

// Start returns false when the render cannot begin.
var saved = edit.Start() && await done.Task;
Console.WriteLine(saved ? $"\nSaved: {output}" : "\nThe video was not saved.");

edit.Dispose();
VisioForgeX.DestroySDK();
```

## How it works

1. **The images** — the folder comes from the command line, or it is `Pictures\slides`. `Directory.GetFiles` takes every `.jpg` file in it, and `Order()` sorts them by name, so name the files `01.jpg`, `02.jpg` and so on. The video goes to `slideshow.mp4` in `Videos`.
2. **The engine** — `VisioForgeX.InitSDKAsync` initializes the SDK before `VideoEditCoreX` is created. `OnError` and `OnProgress` print to the console; `OnStop` completes a `TaskCompletionSource` with `StopEventArgs.Successful`, which is `false` when the output file could not be finished.
3. **The timeline** — `Input_AddImageFile(file, duration, insertTime)` places each image at its own start, 2 seconds after the one before, for 3 seconds, so neighboring images overlap by 1 second. The image is scaled to the output size and keeps its aspect ratio.
4. **The transitions** — from the second image on, a `VideoTransition` of a `VideoTransitionType` covers the overlap; `Video_Transitions` holds them. `Crossfade` blends the two images; `BarWipeLr`, `IrisRect`, `SinglesweepCwt` and `BarndoorV` are SMPTE wipes. The list repeats when there are more images than transitions.
5. **Fade in and out** — the video is 2 seconds per image plus 1 for the last one. `Video_Transition_Add_FadeIn` fades in from black over the first second, and `Video_Transition_Add_FadeOut` fades out over the last one.
6. **The output** — `Output_VideoSize` and `Output_VideoFrameRate` set 1920x1080 at 30 frames per second, and `MP4Output` encodes the video in H.264 into an MP4 file.
7. **The render** — `Start` returns `false` when the render cannot begin. Otherwise the app awaits `OnStop`; a render that an error ends on the way still raises `OnStop`, with `Successful` set to `false`. Then the engine is disposed and `VisioForgeX.DestroySDK` releases the SDK.

Six images make a 13-second video: six times 3 seconds, minus five seconds of overlap.

## Notes

- **The trial watermark** — without a license, the SDK draws a red UNREGISTERED in the top-left corner of every frame. A license removes it; the code stays the same.
- **The target framework** — with `net10.0` instead of `net10.0-windows`, the app gets the Linux build of the SDK and fails with a `DllNotFoundException`.
- **File names** — names sort as text, so `10.jpg` comes before `2.jpg`. Pad the numbers: `01`, `02`.
- **File extensions** — the `*.jpg` pattern skips `.jpeg` files, and on macOS and Linux, where file names are case-sensitive, `.JPG` files too.
- **Other image formats** — the engine also reads PNG, BMP and GIF. `Directory.GetFiles` takes one pattern, so call it once per extension and sort the files together.
- **Ending the app early** — the MP4 file is playable only once `OnStop` has fired; an app that exits before it leaves the encoder unfinished.

## Sample Applications

- [Images to video tutorial console (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-console) — the code of this page and the video

## Frequently Asked Questions

### How do I make a slideshow video from images in C#?

Create `VideoEditCoreX`, add each image with `Input_AddImageFile(file, duration, insertTime)`, set `Output_Format` to `new MP4Output(path)`, call `Start` and wait for `OnStop`. The code above does that for a whole folder.

### How do I add a crossfade between two images?

Let the two images overlap on the timeline, and add `new VideoTransition(VideoTransitionType.Crossfade, start, stop)` to `Video_Transitions` for the overlap: `start` is where the second image begins, `stop` is where the first one ends.

### How do I change how long each image is shown?

Change `show`, and `overlap` for the length of the transitions. Each image then starts `show - overlap` after the one before.

### Which transitions can I use?

`Crossfade` and the SMPTE wipes of the `VideoTransitionType` enum: bar wipes, box wipes, the iris, clock sweeps, barn doors and more. See [Transition between clips](transition-video.md).

## Video Transcript

### What we build

Let's make a video from a folder of images, with transitions between them, in a C# console app, with the VisioForge Video Edit SDK. Here's what we build. A console app that takes a folder of JPEG images and writes one MP4 video. Each image stays on screen for 3 seconds, and the next one starts 1 second before it ends. In that second, a transition takes one image to the next: a crossfade first, then a bar wipe, an iris, a clock wipe and a barn door. The video fades in from black and out to black. It is 1920 by 1080 at 30 frames per second, in H.264. This is the video the app makes from six images. They are frames of Big Buck Bunny, by the Blender Foundation, under a Creative Commons Attribution license.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project is a console app on .NET 10. And a folder of JPEG images. The engine here is VideoEditCoreX, the SDK's GStreamer engine.

### Create a console project

In Visual Studio, create a new project. Search for console, and pick Console App for C#. Name the project ImagesToVideo, choose .NET 10, and click Create.

### Add the NuGet packages

Now add two NuGet packages. VisioForge.DotNet.VideoEdit holds VideoEditCoreX. CrossPlatform Core Windows x64 holds GStreamer and the native libraries the engine runs on. Or use the NuGet Package Manager: right-click the project, Manage NuGet Packages, and install VisioForge.DotNet.VideoEdit from the Browse tab. Then install the Core Windows x64 package. The Installed tab lists both.

### Edit the project file

A console app needs one more step. Open the project file. The console template targets net10.0, and that build of the SDK is the Linux one. Add dash windows to the target framework. And the native package is x64, so set PlatformTarget to x64.

### Find the images

Program.cs starts with five VisioForge usings: the SDK itself, its common types, the output formats, the video edit types, and the engine. The folder comes from the command line, or it is a slides folder in Pictures. The app takes every JPEG file in it, sorted by name, so name them 01, 02, 03, and so on. The video goes to slideshow.mp4 in Videos. With no images, the app says so and stops.

### Create the editor

Initialize the SDK with InitSDKAsync, and create VideoEditCoreX. A TaskCompletionSource waits for the end of the render. OnError prints errors, and OnProgress prints the progress in percent. OnStop fires when the render ends, and its Successful flag tells whether the file was finished.

### Build the timeline

Each image is on screen for 3 seconds, and the next one overlaps it by 1 second. The transitions come from the VideoTransitionType enum: Crossfade, then four wipes. BarWipeLr wipes from left to right. IrisRect opens a rectangle from the center. SinglesweepCwt sweeps like a clock hand. And BarndoorV opens like a barn door. For each image, its start is 2 seconds after the one before. Input_AddImageFile puts the image on the timeline at that start, for 3 seconds. The image is scaled to the output size and keeps its aspect ratio. From the second image on, a VideoTransition fills the second where the two images overlap. The list repeats when there are more images than transitions. The total length is 2 seconds per image, plus 1 for the last one. Video_Transition_Add_FadeIn fades in from black over the first second, and Video_Transition_Add_FadeOut fades out over the last one. Then the output: 1920 by 1080 at 30 frames per second, to an MP4 file. MP4Output encodes the video in H.264.

### Render the video

Start returns false when the render cannot begin. Otherwise the app awaits OnStop and prints whether the video was saved, and where. If an error ends the render on the way, OnStop still fires, with Successful set to false. Last, dispose the engine and release the SDK.

### Run it

Put the code into Program.cs, and run the app. The six images are already in Pictures, slides. The console shows the progress, and then the path of the saved video.

### Check the result

ffprobe confirms it. H.264, 1920 by 1080, 30 frames per second, and 13 seconds: six images of 3 seconds, minus five seconds of overlap. Halfway through the first transition, both images show at once: that is the crossfade. Here the bar wipe has reached the middle of the frame. The iris opens a rectangle from the center. The clock wipe sweeps around the center. And the barn door opens from the middle.

### The trial watermark

You may have noticed the red UNREGISTERED in the top-left corner. That is the trial: without a license, the SDK draws it on every image. A license removes it, and the code stays the same.

### Common mistakes

A few common mistakes. Leaving the target framework at net10.0, without dash windows: that is the Linux build of the SDK, and the app fails with a DllNotFoundException. Naming files 1, 2 and up to 10: names sort as text, so 10 comes before 2. Use 01, 02 instead. Putting PNG files in the folder: this code looks for JPEG files only, so change the search pattern if you need more. And ending the app before OnStop: the encoder has not finished, and the MP4 file is not playable.

### Next steps

There are also guides for transitions between video clips, for text and image overlays, for joining several files into one, and for deploying the redists with your app. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Transition between clips](transition-video.md) — transitions between video files on the timeline
- [Add a text overlay](add-text-overlay.md) — titles and captions over the video
- [Add an image overlay](add-image-overlay.md) — a logo or a picture over the video
- [Merge video and audio without re-encoding](output-file-from-multiple-sources.md) — the video of one file and the audio of another, copied into one MP4
- [Cross-platform deployment](../../deployment-x/index.md) — the native packages `VideoEditCoreX` needs on each platform
- [Video Edit SDK .NET Product Page](https://www.visioforge.com/video-edit-sdk-net)
