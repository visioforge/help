---
title: Creating smooth video transitions in C#
description: Master video transition effects in C# with step-by-step guide and complete code examples for VideoEditCore and VideoEditCoreX APIs.
---

# Creating Professional Video Transitions Between Clips in C #

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Video Transitions

Video transitions create a smooth visual flow between different video clips in your editing projects. Effective transitions can significantly enhance the viewing experience, making your videos appear more professional and engaging. This guide demonstrates how to implement transitions in your C# applications using Video Edit SDK .Net.

Transitions require overlapping timeline segments where both videos exist simultaneously. During this overlap, the transition effect occurs, gradually replacing the first video with the second one. The SDK supports over 100 different transition effects, from simple fades to complex SMPTE standard wipes.

## Understanding Timeline Positioning for Transitions

For transitions to work properly, you need to understand how video clips are positioned on a timeline. Here's how the positioning works:

1. **First video**: Placed at the beginning of the timeline (0ms position)
2. **Second video**: Placed with a slight overlap with the first video
3. **Transition**: Applied in the overlapping region where both videos exist

This overlapping region is crucial - it's where the transition effect will be rendered.

## Creating Video Fragments for Transition

Let's add two video fragments from separate files, each 5 seconds (5000ms) long. The first fragment will be positioned at the start of the timeline, and the second fragment will start at the 4-second mark, creating a 1-second overlap where our transition will occur.

=== "VideoEditCore"

    
    ```cs
    // Define paths to our source video files
    string[] files = { @"c:\samples\video1.avi", @"c:\samples\video2.avi" };
    
    // Create the first video source - this will be the first clip in our timeline
    var videoFile = new VideoSource(
            files[0],                         // Path to first video file
            TimeSpan.Zero,                    // Start from the beginning of the source file
            TimeSpan.FromMilliseconds(5000),  // Use 5 seconds of the video
            VideoEditStretchMode.Letterbox,   // Maintain aspect ratio, add black bars if needed
            0,                                // No rotation (0 degrees)
            1.0);                             // Normal playback speed (1.0x)
    
    // Create the second video source - this will be our second clip with overlap
    var videoFile2 = new VideoSource(
            files[1],                         // Path to second video file
            TimeSpan.Zero,                    // Start from the beginning of the source file
            TimeSpan.FromMilliseconds(5000),  // Use 5 seconds of the video
            VideoEditStretchMode.Letterbox,   // Maintain aspect ratio, add black bars if needed
            0,                                // No rotation (0 degrees)
            1.0);                             // Normal playback speed (1.0x)
    
    // Add the first video at the beginning of the timeline (0ms position)
    await VideoEdit1.Input_AddVideoFileAsync(
            videoFile,
            TimeSpan.FromMilliseconds(0));    // Position on the timeline: 0ms (start)
    
    // Add the second video at 4 seconds, creating a 1-second overlap with the first video
    // This overlap will be where our transition happens
    await VideoEdit1.Input_AddVideoFileAsync(
            videoFile2,
            TimeSpan.FromMilliseconds(4000)); // Position on the timeline: 4000ms (4 seconds)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    // Define paths to our source video files
    string[] files = { @"c:\samples\video1.avi", @"c:\samples\video2.avi" };
    
    // Create the first video source - this will be the first clip in our timeline
    var videoFile = new VideoFileSource(
            files[0],                         // Path to first video file
            TimeSpan.Zero,                    // Start from the beginning of the source file
            TimeSpan.FromMilliseconds(5000),  // Use 5 seconds of the video
            0,                                // No rotation (0 degrees)
            1.0);                             // Normal playback speed (1.0x)
                                              // Note: VideoEditCoreX doesn't require StretchMode here
    
    // Create the second video source - this will be our second clip with overlap
    var videoFile2 = new VideoFileSource(
            files[1],                         // Path to second video file
            TimeSpan.Zero,                    // Start from the beginning of the source file
            TimeSpan.FromMilliseconds(5000),  // Use 5 seconds of the video
            0,                                // No rotation (0 degrees)
            1.0);                             // Normal playback speed (1.0x)
    
    // Add the first video at the beginning of the timeline (0ms position)
    VideoEdit1.Input_AddVideoFile(
            videoFile,
            TimeSpan.FromMilliseconds(0));    // Position on the timeline: 0ms (start)
    
    // Add the second video at 4 seconds, creating a 1-second overlap with the first video
    // This overlap creates the region where our transition will occur
    VideoEdit1.Input_AddVideoFile(
            videoFile2,
            TimeSpan.FromMilliseconds(4000)); // Position on the timeline: 4000ms (4 seconds)
    ```
    


### Understanding the Parameters

When adding video files to the timeline, each parameter serves a specific purpose:

- **File path**: Location of the video file on disk
- **Start time**: Position in the source video to start from (TimeSpan.Zero means beginning)  
- **Duration**: Length of video to use (5000ms in our example)
- **Stretch mode** (VideoEditCore only): How to handle aspect ratio differences (Letterbox, Stretch, etc.)
- **Rotation**: Degrees to rotate the video (0 means no rotation)
- **Playback speed**: Speed multiplier (1.0 means normal speed)
- **Insert time**: Position on the timeline where this clip should be placed

## Implementing the Transition Effect

Now that we have our two overlapping video clips, we'll add a transition effect that will occur between the 4-second and 5-second marks on our timeline.

=== "VideoEditCore"

    
    First, let's get the ID of our desired transition effect:
    
    ```cs
    // Get the ID for the "Upper right" transition effect
    // Each transition has a unique name and corresponding ID
    int id = VideoEdit.Video_Transition_GetIDFromName("Upper right");
    ```
    
    Then, we'll add the transition by specifying the start time, end time, and transition ID:
    
    ```cs
    // Add the transition to the timeline
    // Parameters:
    // - Start time: 4000ms (where the second clip begins and overlap starts)
    // - End time: 5000ms (where the first clip ends and overlap ends)
    // - Transition ID: The ID we retrieved for the "Upper right" transition
    VideoEdit1.Video_Transition_Add(TimeSpan.FromMilliseconds(4000), TimeSpan.FromMilliseconds(5000), id);
    ```
    
    To see all available transition effects, you can use:
    
    ```cs
    // Get an array of all available transition effect names
    string[] availableTransitions = VideoEdit.Video_Transition_Names();
    
    // Example of iterating through all available transitions
    foreach (string transitionName in availableTransitions)
    {
        // Get the ID for each transition
        int transitionId = VideoEdit.Video_Transition_GetIDFromName(transitionName);
        // You could use this in your app UI to let users choose transitions
        Console.WriteLine($"Transition: {transitionName}, ID: {transitionId}");
    }
    ```
    

=== "VideoEditCoreX"

    
    In VideoEditCoreX, we can first list all available transitions:
    
    ```cs
    // Get all available transition names as an array
    var transitionNames = VideoEdit1.Video_Transitions_Names();
    
    // Select a specific transition by index
    // Note: Array is zero-based, so index 10 is the 11th transition in the list
    var transitionName = transitionNames[10]; 
    
    // You could also iterate through all transitions to show them in a UI dropdown
    // foreach (var name in transitionNames)
    // {
    //     Console.WriteLine($"Available transition: {name}");
    // }
    ```
    
    Then, we'll create a transition object and add it to our timeline:
    
    ```cs
    // Create a new transition object specifying:
    // - The transition name we selected above
    // - Start time (4000ms) - where the overlap begins
    // - End time (5000ms) - where the overlap ends
    var trans = new VideoTransition(
            transitionName,                          // The transition name 
            TimeSpan.FromMilliseconds(4000),         // Start time of transition
            TimeSpan.FromMilliseconds(5000));        // End time of transition
    
    // Add the transition to the VideoEdit component's transitions collection
    VideoEdit1.Video_Transitions.Add(trans);
    ```
    
    You can also directly specify the transition name if you know it:
    
    ```cs
    // Create a transition using a specific name without looking it up first
    // This is useful when you already know which transition you want to use
    var trans = new VideoTransition(
            "Circle",                                // Using "Circle" transition directly
            TimeSpan.FromMilliseconds(4000),         // Start time of transition
            TimeSpan.FromMilliseconds(5000));        // End time of transition
    
    // Add the transition to the VideoEdit component
    VideoEdit1.Video_Transitions.Add(trans);
    
    // You can also create multiple transitions between different clips:
    // var secondTrans = new VideoTransition("Fade", TimeSpan.FromMilliseconds(9000), TimeSpan.FromMilliseconds(10000));
    // VideoEdit1.Video_Transitions.Add(secondTrans);
    ```
    


## Popular Transition Effects and When to Use Them

The SDK offers many transition effects suitable for different situations:

1. **Fade transitions** (crossfade): Ideal for subtle, elegant transitions
2. **Wipe transitions** (horizontal, vertical, diagonal): Great for dynamic scene changes
3. **Zoom/push transitions**: Effective for emphasizing the next scene
4. **Geometric transitions** (circle, square, diamond): Create interesting visual effects
5. **Special transitions** (random blocks, matrix effects): For creative or dramatic transitions

## Processing Your Video with Transitions

After setting up your video clips and transition, you'll need to start the processing:

=== "VideoEditCore"

    
    ```cs
    // STEP 1: Configure the output file path
    VideoEdit1.Output_Filename = "output.mp4";  // Set the destination file path
    
    // STEP 2: Create and configure the output format
    var outputFormat = new MP4Output();
    // You can customize the output with various properties like:
    // outputFormat.VideoBitrate = 5000000;  // Set video bitrate to 5Mbps
    // outputFormat.VideoFrameRate = 30;     // Set frame rate to 30fps
    // outputFormat.VideoWidth = 1920;       // Set output width to 1920px
    // outputFormat.VideoHeight = 1080;      // Set output height to 1080px
    
    // STEP 3: Assign the output format to the VideoEdit component
    VideoEdit1.Output_Format = outputFormat;
    
    // STEP 4: Start the asynchronous processing
    // This will render the video with the transition and save it to the output file
    await VideoEdit1.StartAsync();
    
    // After this call, you should listen for processing events like:
    // - VideoEdit1.OnProgress to track processing progress
    // - VideoEdit1.OnStop to detect when processing is complete
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    // STEP 1: Create and configure the output format
    // In VideoEditCoreX, we specify the output filename directly in the constructor
    var outputFormat = new MP4Output("output.mp4");
    
    // You can customize the output with various properties like:
    // outputFormat.VideoBitrate = 5000000;  // Set video bitrate to 5Mbps
    // outputFormat.AudioBitrate = 192000;   // Set audio bitrate to 192kbps
    // outputFormat.VideoFrameRate = 30;     // Set frame rate to 30fps
    // outputFormat.Width = 1920;            // Set output width to 1920px
    // outputFormat.Height = 1080;           // Set output height to 1080px
    
    // STEP 2: Assign the output format to the VideoEdit component
    VideoEdit1.Output_Format = outputFormat;
    
    // STEP 3: Start the processing (non-async in VideoEditCoreX)
    // This will render the video with the transition and save it to the output file
    VideoEdit1.Start();
    
    // ALTERNATIVE: For background processing, you could use:
    // VideoEdit1.Start(true);  // true means run in a background thread
    
    // You should also set up event handlers before calling Start():
    // VideoEdit1.OnProgress += (s, e) => { Console.WriteLine($"Progress: {e.Progress}%"); };
    // VideoEdit1.OnStop += (s, e) => { Console.WriteLine("Processing completed!"); };
    ```
    


## Common Transition Challenges and Solutions

When implementing video transitions, you might encounter these common challenges:

### Challenge 1: Transitions Not Appearing

If your transitions aren't showing up:

- Ensure the video clips actually overlap on the timeline
- Verify the transition time span falls within this overlap
- Check that the transition name or ID is valid

### Challenge 2: Poor Visual Quality

For higher quality transitions:

- Use higher resolution source videos
- Use a higher bitrate for your output
- Consider adding a slight blur effect for smoother transitions

### Challenge 3: Performance Issues

If transition rendering is slow:

- Use hardware acceleration if available
- Simplify complex transitions when targeting lower-end hardware
- Consider pre-rendering transitions for performance-critical applications

## Required Dependencies

To implement video transitions using Video Edit SDK, you'll need:

- Video Edit SDK redist packages: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

For guidance on installing these dependencies, see our [deployment guide](../deployment.md).

## Advanced Transition Techniques

For more advanced transition effects:

1. **Combining transitions with effects**: Apply a blur or color effect during the transition
2. **Varying transition speeds**: Use different durations for the start and end of transitions
3. **Keyframe animation**: Create custom transitions with precise control
4. **Audio crossfading**: Synchronize audio transitions with your video transitions

## Conclusion

Video transitions are a powerful way to enhance your C# video applications. With the Video Edit SDK, you have access to a wide range of transition effects that can be customized to suit your specific needs. By following the examples in this guide, you can implement professional-quality transitions in your video editing projects.

For additional options and detailed information about SMPTE transitions, check our [comprehensive transitions reference](../transitions.md).

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.