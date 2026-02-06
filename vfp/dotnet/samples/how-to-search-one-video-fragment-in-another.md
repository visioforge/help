---
title: Finding Video Fragments in Larger Video Content
description: Implement video fragment search functionality in applications using fingerprinting technology with detailed guide and code examples.
---

# Finding Video Fragments in Larger Video Content

## Introduction

Video fingerprinting technology allows developers to identify and locate specific video segments within larger video files. This guide demonstrates the implementation process using a powerful Video Fingerprinting SDK that works across various video formats and quality levels.

The primary examples in this tutorial use the .NET API implementation, but equivalent functionality is available through the C++ API for developers preferring native code solutions.

## Implementation Process

### Step 1: Analyze the Fragment File

First, we need to extract a fingerprint from the smaller video fragment that we want to locate within the larger video. This process involves analyzing the video's unique characteristics and generating a digital signature.

```csharp
// create video file source
var fragmentSrc = new VFPFingerprintSource(ShortFile, VFSimplePlayerEngine.LAV);
fragmentSrc.StopTime = TimeSpan.FromMilliseconds(5000);
            
// get fingerprint
var fragment = VFPAnalyzer.GetSearchFingerprintForVideoFile(fragmentSrc, ErrorCallback);
```

In this code block, we:

- Create a fingerprint source pointing to the short fragment file
- Set a duration limit of 5 seconds for analysis
- Generate a searchable fingerprint using the analyzer

### Step 2: Analyze the Target Video

Next, we need to extract a fingerprint from the larger video where we'll search for our fragment. The process is similar, but without time limitations.

```csharp
// create video file source
var mainSrc = new VFPFingerprintSource(LongFile, VFSimplePlayerEngine.LAV);

// get fingerprint
var main = VFPAnalyzer.GetSearchFingerprintForVideoFile(mainSrc, ErrorCallback);
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
if (res > 0)
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
- Check if a matching position was found (positive result value)
- Convert the result to a timestamp and display it along with the difference value

## Performance Considerations

The fingerprinting technology uses sophisticated algorithms that balance accuracy and performance. For optimal results:

- Consider adjusting the maximum difference level based on your specific requirements
- Process videos at their native resolution when possible
- For very large files, consider splitting the search into manageable chunks

## Additional Resources

For complete documentation, implementation examples in other languages, and licensing information, visit the [product page](https://www.visioforge.com/video-fingerprinting-sdk).
