---
title: Cut a Video Without Re-encoding in a Few Lines of C# .NET
description: Copy a part of a video file into a new file without re-encoding, in a few lines of C# with Video Edit SDK .NET and VideoEditCore.FastEdit_CutFileAsync.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCore
  - FastEditSeekMode

video:
  id: EASyGlUymvY
  upload_date: 2026-11-24T07:00:00Z
  duration: PT1M3S
---

# Cut a Video Without Re-encoding in C\#

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

This page cuts a part of a video file into a new file with [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net) and its `VideoEditCore` engine, in a C# console app. `FastEdit_CutFileAsync` copies the compressed streams as they are, so nothing is re-encoded: the codecs, resolution, frame rate and bit rate stay the same, there is no quality loss, and 30 seconds of a 720p movie are saved in about half a second.

`VideoEditCore` is the SDK's Windows engine; its `FastEdit_` methods run the FFmpeg tool that comes in the FFmpeg redist package.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Video tutorial

The one-minute video writes and runs this console app: 30 seconds of a 720p movie cut in about half a second.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/EASyGlUymvY?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Required NuGet packages

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/) — the SDK with `VideoEditCore`
- [VisioForge.DotNet.Core.Redist.FFMPEG.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/) — the FFmpeg tool the `FastEdit_` methods run; for an x86 app, [VisioForge.DotNet.Core.Redist.FFMPEG.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="[2026.11.25]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.FFMPEG.x64" Version="[2026.11.25]" />
```

The redist is x64, so set `PlatformTarget` to `x64` in the project file:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

## The code

The whole console app: it copies 1:00 to 1:30 of `movie.mp4` into `cut.mp4`.

<!-- vf-compile -->
```csharp
using System;
using VisioForge.Core.VideoEdit;

await using var core = new VideoEditCore();
core.OnError += (s, e) =>
    Console.WriteLine(e.Message);
var ok = await core.FastEdit_CutFileAsync(
    "movie.mp4",
    TimeSpan.FromSeconds(60),
    TimeSpan.FromSeconds(90),
    "cut.mp4");
Console.WriteLine(ok ? "Done" : "Failed");
```

The full project is [cut-video-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/cut-video-short-console). File names are relative to the working directory, so put `movie.mp4` in the folder you start the app from.

## How it works

1. **The engine** — `VideoEditCore` needs no video view for a cut. `await using` disposes it when the app ends.
2. **Errors** — `OnError` reports why a cut failed, for example a missing input file or a missing FFmpeg redist; the app prints it before `Failed`.
3. **The cut** — `FastEdit_CutFileAsync(input, start, stop, output)` copies the part from `start` to `stop` into the output file without re-encoding, and returns `true` when the file is saved. A `stop` of `TimeSpan.Zero` cuts to the end of the file.
4. **Keyframes** — the cut starts on the keyframe at or before `start`. The frames between that keyframe and `start` are copied too. In an MP4 or MOV output they are marked to be skipped, so a player starts at `start`; in other containers, and for an MPEG-TS input, the cut plays from that keyframe and is that much longer.

The output on a 10-minute Big Buck Bunny (1280x720 H.264 at 24 fps, AAC), checked with ffprobe: 30.0 seconds, the source's H.264 High profile and AAC. The first video packet of the cut is the source's keyframe at 57.75 s, byte for byte the same size; the frames before 1:00 carry the discard flag. [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

```console
ffprobe -v error -of compact cut.mp4 -show_entries format=duration:stream=codec_name,profile,width,height
stream|codec_name=h264|profile=High|width=1280|height=720
stream|codec_name=aac|profile=LC
format|duration=30.000544
```

## Notes

- **Seek mode** — the overload with `FastEditSeekMode` chooses where FFmpeg seeks. `FastEditSeekMode.Input` (the default) starts on the keyframe at or before the start position. `FastEditSeekMode.Output` drops the video up to the first keyframe after it, so a cut shorter than one keyframe interval can come out without video, and a `stop` of `TimeSpan.Zero` copies the whole file.
- **Frame accuracy** — a cut without re-encoding cannot start between keyframes. For a cut on an exact frame, re-encode with the timeline of `VideoEditCore` or `VideoEditCoreX`.
- **The container** — the output gets the source's codecs, so keep a container that holds them: an MP4 for H.264 or H.265 with AAC, an MKV for others.
- **The FFmpeg redist** — without the FFmpeg redist package, the `FastEdit_` methods have no tool to run. Deploy it with your app; see the [deployment guide](../deployment.md).

## Sample Applications

- [Cut video short console (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/cut-video-short-console) — the code of this page
- [Cut Video File (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/WPF/CSharp/Cut%20Video%20File) — the same call behind a WPF window with a progress bar

## Frequently Asked Questions

### How do I cut a video without re-encoding in C#?

Create `VideoEditCore` and call `FastEdit_CutFileAsync(inputFile, start, stop, outputFile)`. The streams are copied as they are, so the cut takes about as long as copying that part of the file.

### Why does my cut start a little before the start time?

A cut without re-encoding starts on a keyframe. With the default seek mode the frames from the keyframe before the start time are copied but marked to be skipped, so in an MP4 or MOV file players start at the start time; some editors show them anyway. Other containers, and an MPEG-TS input, play from the keyframe.

### Can I cut to the end of the file?

Yes: pass `TimeSpan.Zero` as the stop time.

### Is the quality the same as the source?

Yes. Nothing is decoded or encoded, so every copied frame is the source's frame.

## See Also

- [Merge video and audio without re-encoding](output-file-from-multiple-sources.md) — the video of one file and the audio of another, copied into one MP4
- [Several segments](several-segments.md) — join parts of files into one video, re-encoded
- [Deployment](../deployment.md) — the redists `VideoEditCore` needs on the user's PC
- [Video Edit SDK .NET Product Page](https://www.visioforge.com/video-edit-sdk-net)
