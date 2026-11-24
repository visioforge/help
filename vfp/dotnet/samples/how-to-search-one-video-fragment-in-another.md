---
title: Video Fragment Search Using Fingerprinting in C# .NET
description: Find video fragments inside larger files using VisioForge fingerprinting SDK in C# .NET with step-by-step implementation and code examples.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - C#
primary_api_classes:
  - VFPFingerprintSource
  - VFSimplePlayerEngine
  - VFPAnalyzer

video:
  id: MM3IJxIxwww
  upload_date: 2026-11-24T09:52:31Z
  duration: PT1M8S
---

# Finding Video Fragments in Larger Video Content

## Introduction

Video fingerprinting technology allows developers to identify and locate specific video segments within larger video files. This guide demonstrates the implementation process using a powerful Video Fingerprinting SDK that works across various video formats and quality levels.

The primary examples in this tutorial use the .NET API implementation, but equivalent functionality is available through the C++ API for developers preferring native code solutions.

## Implementation Process

### Step 1: Analyze the Fragment File

First, we need to extract a fingerprint from the smaller video fragment that we want to locate within the larger video. This process involves analyzing the video's unique characteristics and generating a digital signature.

```csharp
// VFPFingerprintSource has only a (string filename) ctor — there is no
// engine-selection overload. Limit the analysis window via StartTime/StopTime.
var fragmentSrc = new VFPFingerprintSource(ShortFile);
fragmentSrc.StopTime = TimeSpan.FromMilliseconds(5000);

// VFPAnalyzer.GetSearchFingerprintForVideoFile has only an Async variant.
var fragment = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(fragmentSrc, ErrorCallback);
```

In this code block, we:

- Create a fingerprint source pointing to the short fragment file
- Set a duration limit of 5 seconds for analysis
- Generate a searchable fingerprint using the analyzer

### Step 2: Analyze the Target Video

Next, we need to extract a fingerprint from the larger video where we'll search for our fragment. The process is similar, but without time limitations.

```csharp
// Same single-arg ctor + async getter for the longer file.
var mainSrc = new VFPFingerprintSource(LongFile);

var main = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(mainSrc, ErrorCallback);
```

### Step 3: Set Up Error Handling

To maintain robust error handling, we implement a callback function that captures and displays any errors encountered during the fingerprinting process.

```csharp
private static void ErrorCallback(string error)
{
    Console.WriteLine(error);
}
```

### Step 4: Perform the Search Operation

With both fingerprints ready, we can now search for the fragment within the larger video file.

```csharp
// set the maximum difference level
var maxDifference = 500;

// search one video fragment in another video using fingerprints
var res = VFPSearch.Search(fragment, 0, main, 0, out var difference, maxDifference);

// check the result
if (res != int.MaxValue)
{
    TimeSpan ts = new TimeSpan(res * TimeSpan.TicksPerSecond);
    Console.WriteLine($"Detected fragment file at {ts:g}, difference level is {difference}");
}
else
{
    Console.WriteLine("Fragment file not found.");
}
```

In this code:

- We define a tolerance level for differences between fingerprints
- Perform the search operation between our fragment and main video
- Check if a matching position was found (`Search` returns `int.MaxValue` when the fragment is not found; 0 means a match at the very start)
- Convert the result to a timestamp and display it along with the difference value

### Finding Every Match in a Few Lines

A complete console app: it finds every place where `clip.mp4` appears in `movie.mp4` and prints the start of each match. `VFPAnalyzer.SearchAsync` takes the two fingerprints, the clip's duration, the difference limit (a match must score below 500 here) and `true` to return every match instead of only the best one, and it returns the start of each match as a `List<TimeSpan>`. `GetSearchFingerprintForVideoFileAsync` returns `null` when a file cannot be analyzed and prints the reason through the callback; a missing file throws `FileNotFoundException` from the `VFPFingerprintSource` constructor. A 10-second 480p clip was found at all five places in 2 hours 18 minutes of 720p video, in about nine minutes.

<!-- vf-compile -->
```csharp
using System;
using VisioForge.Core;
using VisioForge.Core.VideoFingerPrinting;
using static VisioForge.Core.VideoFingerPrinting.VFPAnalyzer;

await VisioForgeX.InitSDKAsync();
SetLicenseKey("TRIAL");
var clip = await
    GetSearchFingerprintForVideoFileAsync(
        new VFPFingerprintSource("clip.mp4"),
        Console.WriteLine);
var movie = await
    GetSearchFingerprintForVideoFileAsync(
        new VFPFingerprintSource("movie.mp4"),
        Console.WriteLine);
if (clip != null && movie != null)
{
    var found = await SearchAsync(
        clip, movie, clip.Duration, 500, true);
    foreach (var at in found)
        Console.WriteLine(
            $"Found at {at:hh\\:mm\\:ss}");
    if (found.Count == 0)
        Console.WriteLine("Not found");
}
VisioForgeX.DestroySDK();
```

The full project is [find-clip-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/_VideoTutorials/find-clip-short-console), a Windows x64 console app that references `VisioForge.DotNet.Core.Redist.VideoFingerprinting` and the SDK's native packages for that platform, `VisioForge.CrossPlatform.Core.Windows.x64` and `VisioForge.CrossPlatform.Libav.Windows.x64`. File names are relative to the working directory, so put both files in the folder you start the app from. Without a license key the SDK runs in trial mode and shows an "Unregistered version" message once; click OK and the search goes on.

The one-minute video writes and runs this app.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/MM3IJxIxwww?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Performance Considerations

The fingerprinting technology uses sophisticated algorithms that balance accuracy and performance. For optimal results:

- Consider adjusting the maximum difference level based on your specific requirements
- Process videos at their native resolution when possible
- For very large files, consider splitting the search into manageable chunks

## Additional Resources

For complete documentation, implementation examples in other languages, and licensing information, visit the [product page](https://www.visioforge.com/video-fingerprinting-sdk).
