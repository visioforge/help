---
title: Merge Video and Audio Without Re-encoding in C# .NET
description: Take the video from one file and the audio from another and copy them into one MP4 without re-encoding, in a C# WinForms app with Video Edit SDK .NET.
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
  - FFMPEGStream
video:
  id: l59WVEu4uIs
  upload_date: 2026-11-15T09:00:11Z
  duration: PT9M57S

---

# Merge Video and Audio Without Re-encoding in C#

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

This tutorial builds a C# WinForms app that takes the video stream from one file and the audio stream from another, and copies both into one MP4 with [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net) and its `VideoEditCore` engine. `FastEdit_MuxStreamsAsync` copies the compressed streams as they are, so nothing is re-encoded: the codec, resolution, frame rate and bit rate stay the same, there is no quality loss, and a 27-second clip is saved in a couple of seconds. The same call puts a new soundtrack under a video, replaces its audio, or adds a second audio track.

`VideoEditCore` is the SDK's Windows engine; its `FastEdit_` methods run the FFmpeg tool that comes in the FFmpeg redist package.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## YouTube tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/l59WVEu4uIs?controls=1" title="Merge video and audio without re-encoding in C# - YouTube tutorial" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/mux-streams-winforms)

The video in the tutorial is a clip of [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

The video creates the project on .NET 10, which needs Visual Studio 2026; in Visual Studio 2022, choose .NET 8 or .NET 9. On .NET Framework 4.7.2, `Form1.cs` builds unchanged in an SDK-style project (create it from the Windows Forms App template and set `TargetFramework` to `net472`) with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`; in `Program.cs`, replace `ApplicationConfiguration.Initialize()` with `Application.EnableVisualStyles()` and `Application.SetCompatibleTextRenderingDefault(false)`.

## Required NuGet packages

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/) — the SDK with `VideoEditCore`
- [VisioForge.DotNet.Core.Redist.FFMPEG.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/) — the FFmpeg tool the `FastEdit_` methods run; for an x86 app, [VisioForge.DotNet.Core.Redist.FFMPEG.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.11.13" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.FFMPEG.x64" Version="2026.11.13" />
```

The redist is x64, so set `PlatformTarget` to `x64` in the project file:

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <UseWindowsForms>true</UseWindowsForms>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

## The form

`Form1` holds a text box and a `...` button for the video file (`edVideo`, `btVideo`), the same for the audio file (`edAudio`, `btAudio`), a text box for the output file (`edOutput`), a check box `cbShortest` ("Stop at the end of the shortest stream", checked), the `btStart` button and an `OpenFileDialog`. The form's `Load` and `FormClosing` events and the three buttons' `Click` events have handlers.

## The code

`Form1.cs`:

```cs
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;

namespace MuxStreams;

public partial class Form1 : Form
{
    private VideoEditCore videoEdit1;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoEdit1 = new VideoEditCore();

        // Show SDK errors in the title bar
        videoEdit1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
    }

    private void btVideo_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK) edVideo.Text = openFileDialog1.FileName;
    }

    private void btAudio_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK) edAudio.Text = openFileDialog1.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        // The first video stream of one file and the first audio stream of another
        var streams = new List<FFMPEGStream>
        {
            new FFMPEGStream { Filename = edVideo.Text, ID = "v:0" },
            new FFMPEGStream { Filename = edAudio.Text, ID = "a:0" },
        };

        btStart.Enabled = false;
        Text = "Saving...";
        try
        {
            // Copies the streams into one file as they are, without re-encoding
            var saved = await videoEdit1.FastEdit_MuxStreamsAsync(streams, cbShortest.Checked, edOutput.Text);
            if (saved) Text = "Saved: " + edOutput.Text; // on failure the title keeps the error
        }
        finally
        {
            btStart.Enabled = true;
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Let the file be finished before the window goes away
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            return;
        }

        videoEdit1.Dispose();
    }
}
```

## How it works

1. **The engine** — `Form1_Load` creates `VideoEditCore`. `OnError` reports engine errors; they can arrive on a worker thread, so `BeginInvoke` puts them into the title bar on the UI thread. The output file is `output.mp4` in `Videos` by default.
2. **The streams** — each `FFMPEGStream` names a file (`Filename`) and a stream in it (`ID`). The IDs are FFmpeg stream specifiers: `"v:0"` is the first video stream of the file, `"a:0"` the first audio stream, `"a:1"` the second. A bare `"v"` or `"a"` takes every stream of that type.
3. **The copy** — `FastEdit_MuxStreamsAsync(streams, shortest, outputFile)` copies the listed streams, in that order, into the output file without re-encoding, and returns `true` when the file is saved.
4. **The shortest stream** — with `shortest` set to `true`, the file ends with the shorter stream. With `false`, it runs as long as the longest one: in the video, 108 seconds of music under a 27-second clip.
5. **Closing the window** — while a save runs, `Start` is disabled, and `Form1_FormClosing` cancels the close so the file is finished first. Otherwise it disposes the engine.

The output of the video, checked with ffprobe: the H.264 stream of the source, 1920x1080 at the same bit rate, and the AAC music track. An MD5 of the video stream is the same in the source and in the output: the stream is copied byte for byte.

```console
ffprobe -v error -of compact output.mp4 -show_entries format=duration,size:stream=codec_name,width,height,bit_rate,sample_rate
stream|codec_name=h264|width=1920|height=1080|bit_rate=2783582
stream|codec_name=aac|sample_rate=48000|bit_rate=192448
format|duration=27.466667|size=10225512
```

## Notes

- **The container must accept the codecs** — nothing is re-encoded, so the output format has to hold the source codecs as they are. H.264 or H.265 with AAC or MP3 go into MP4; for other codecs, write an MKV file.
- **The video file's own audio** — `"v:0"` takes only the video of that file, so its soundtrack is left out. To keep it as a second track, add `new FFMPEGStream { Filename = edVideo.Text, ID = "a:0" }` to the list.
- **More audio tracks** — every `FFMPEGStream` with an audio ID becomes one more audio track in the output, for example a second language.
- **The FFmpeg redist** — without the FFmpeg redist package, the `FastEdit_` methods have no tool to run. Deploy it with your app; see the [deployment guide](../deployment.md).

## Sample Applications

- [Mux streams tutorial WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/mux-streams-winforms) — the code of this page and the video

## Frequently Asked Questions

### How do I add an audio track to a video without re-encoding in C#?

Create `VideoEditCore`, list the video stream (`ID = "v:0"`) and the audio stream (`ID = "a:0"`) as `FFMPEGStream` items, and call `FastEdit_MuxStreamsAsync(streams, true, outputFile)`. The code above does that with two files picked in a dialog.

### How do I replace the audio of an MP4 file?

Take only the video of the MP4 with `"v:0"` and the new audio from the other file with `"a:0"`. The MP4's own soundtrack is not in the list, so it is not in the output.

### What does the shortest parameter do?

With `true`, the output ends where the shorter stream ends. With `false`, the output is as long as the longest stream, and the shorter one simply stops.

### Is the quality the same as the source?

Yes. The streams are copied, not decoded and encoded again, so the codec, resolution, frame rate and bit rate are those of the source files.

## Video Transcript

### What we build

Let's take the video from one file and the audio from another, and put them into one MP4 without re-encoding, in a C# WinForms app with the VisioForge Video Edit SDK. Here's what we build. A WinForms app that merges two files into one. It takes the video stream from one file and the audio stream from another. Both are copied as they are, with no re-encoding and no quality loss. And a check box stops the file at the end of the shorter stream. This is the finished app. It has just put a new music track under a clip of Big Buck Bunny.

### Copy, not re-encode

Why copy instead of re-encode? Re-encoding decodes every frame and compresses it again. That costs time, and each pass loses a little quality. A stream copy moves the compressed data into a new file, so the codec, the resolution, the frame rate and the bit rate stay exactly as they were. There's no quality loss, and it takes seconds instead of minutes. The one rule: the output container must accept both codecs. H.264 or H.265 with AAC or MP3 go into MP4. For anything else, use MKV.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. And you need a video file and an audio file. VideoEditCore is the SDK's Windows engine, and its FastEdit methods run the FFmpeg tool that comes with the SDK.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project MuxStreams. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add two NuGet packages: the Video Edit SDK itself, and the FFmpeg redist. The redist carries the FFmpeg tool that the FastEdit methods run. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.VideoEdit and install it. Then install the FFmpeg redist for x64. The redist is the x64 one, so set PlatformTarget to x64 in the project file.

### Design the WinForms form

From the Toolbox, add a label, a text box and a small button for the video file, and give each control a clear name in the Properties window. Add the same row for the audio file, then a label and a text box for the output file. Add a check box to stop at the end of the shortest stream, checked by default, and the Start button. Then an OpenFileDialog. It goes into the tray under the form. Its filter lists the media files. Double-click the form and the three buttons. Visual Studio adds an empty handler for each one.

### Create the editing engine

At the top of Form1.cs, add these usings. In the form's Load handler, create the VideoEditCore engine. Engine errors arrive in the OnError event. Here they go to the title bar. The output file goes into the Videos folder by default.

### Pick the files

The two small buttons open the same dialog, and put the picked file into their text box.

### Merge the streams

The Start handler lists the streams to copy. Each FFMPEGStream names a file and a stream in it. v:0 is the first video stream of the video file, and a:0 is the first audio stream of the audio file. The IDs follow FFmpeg's stream specifiers. v:0 is the first video stream, a:0 the first audio stream, and a:1 the second. A plain "v" or "a" takes every stream of that type in the file. So the clip's own soundtrack is left out here: we only ask for its video. Then it disables Start and calls FastEdit_MuxStreamsAsync with the streams, the check box, and the output file. The call copies the streams into one file, and returns true when the file is saved. The second argument is the check box. Checked, the file ends with the shorter stream: here, twenty-seven seconds of video and twenty-seven seconds of music. Unchecked, the file runs as long as the longer stream: here, a hundred and eight seconds, most of it music without a picture.

### Close the window

Closing the window while it saves would leave the file unfinished. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While a save runs, it cancels the close. Otherwise, it disposes the engine.

### Run it

Run the app from Visual Studio. The output file is already set to the Videos folder. Pick the video file: a clip of Big Buck Bunny. Then the audio file: a music track. Click Start. A couple of seconds later, the title bar says the file is saved. Then we close the window.

### Check the file with ffprobe

The file lands in Videos: twenty-seven seconds of 1920 by 1080 video at thirty frames per second. ffprobe shows what's inside. The video is H.264 at 1920 by 1080, with the same bit rate as the source. The audio is the AAC music track. And an MD5 of the video stream is the same in both files: it was copied byte for byte.

### Common mistakes

A few common mistakes. The FFmpeg redist package is missing, so FastEdit has no tool to run. A plain "v" or "a", where v:0 or a:0 was meant, so extra streams end up in the file. A codec the output container can't hold: use MKV instead. And closing the window in the middle of a save: wait for it to finish first.

### Next steps

Next, the same engine cuts a part of a file without re-encoding, with FastEdit_CutFileAsync, and joins files of the same format with FastEdit_JoinFilesAsync. And the images to video tutorial builds a whole new video with transitions. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Make a video from images with transitions](video-images-console.md) — a new video rendered from a folder of images
- [Multiple audio streams in video files](adding-video-file-with-multiple-audio-streams.md) — select and mix the audio tracks of one video file
- [Deployment](../deployment.md) — the redists `VideoEditCore` needs on the user's PC
- [Video Edit SDK .NET Product Page](https://www.visioforge.com/video-edit-sdk-net)
