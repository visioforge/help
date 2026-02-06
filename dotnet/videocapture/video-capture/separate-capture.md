---
title: Start/Stop Video Capture Without Stopping Preview
description: Control video capture and preview independently in .NET with step-by-step code examples for efficient recording and streaming management.
---

# Managing Video Capture and Preview Independently in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

When developing video applications, it's often necessary to start or stop recording while maintaining an uninterrupted preview. This capability is essential for creating professional video recording software, security applications, or any scenario where continuous visual feedback is required regardless of the recording state.

This guide demonstrates how to independently control video capture operations without affecting the preview display. This technique applies to various capture scenarios including camera recording, screen capture, and other input sources.

## Why Separate Preview and Capture?

There are several advantages to separating preview and capture functionality:

1. **Enhanced User Experience** - Users can continuously see what's being captured, even when not recording
2. **Resource Efficiency** - Prevents unnecessary stopping/restarting of video streams
3. **Reduced Latency** - Eliminates the delay associated with reestablishing preview after stopping a recording
4. **Greater Control** - Provides more precise management of recording sessions

## Implementation Options

There are two main approaches to implementing this functionality depending on which SDK version you're using:

=== "VideoCaptureCoreX"

    
    ### Method 1: Using VideoCaptureCoreX
    
    The VideoCaptureCoreX approach offers a streamlined way to manage outputs and control capture states.
    
    #### Step 1: Configure the Output
    
    First, add a new output with your desired settings. In this example, we'll use MP4 output. Note the `false` parameter which indicates we don't want to start capture immediately:
    
    ```cs
    VideoCapture1.Outputs_Add(new MP4Output("output.mp4"), false); // false - don't start capture immediately. 
    ```
    
    #### Step 2: Start Preview Only
    
    Next, start the video preview without initiating capture:
    
    ```cs
    await VideoCapture1.StartAsync();
    ```
    
    #### Step 3: Start Capture When Needed
    
    When you want to begin recording, start the actual video capture to your output destination:
    
    ```cs
    await VideoCapture1.StartCaptureAsync(0, "output.mp4"); // 0 - index of the output.
    ```
    
    #### Step 4: Stop Capture While Maintaining Preview
    
    To stop recording while keeping the preview active:
    
    ```cs
    await VideoCapture1.StopCaptureAsync(0); // 0 - index of the output.
    ```
    
    ### Advanced Output Management
    
    You can add multiple outputs with different settings:
    
    ```cs
    // Add MP4 output
    VideoCapture1.Outputs_Add(new MP4Output("primary_recording.mp4"), false);
    
    // Add additional output for streaming
    VideoCapture1.Outputs_Add(new RTMPOutput("rtmp://streaming.example.com/live/stream"), false);
    
    // Start preview
    await VideoCapture1.StartAsync();
    
    // Start recording to both outputs
    await VideoCapture1.StartCaptureAsync(0, "primary_recording.mp4");
    await VideoCapture1.StartCaptureAsync(1, "rtmp://streaming.example.com/live/stream");
    ```
    
    ### Output Control With Indices
    
    When managing multiple outputs, the index parameter becomes crucial:
    
    ```cs
    // Stop the MP4 recording but continue streaming
    await VideoCapture1.StopCaptureAsync(0);
    
    // Later, stop the stream too
    await VideoCapture1.StopCaptureAsync(1);
    ```
    

=== "VideoCaptureCore"

    
    ### Method 2: Using VideoCaptureCore
    
    The older VideoCaptureCore approach uses a different pattern with explicit separate capture enablement.
    
    #### Step 1: Enable Separate Capture Mode
    
    Begin by enabling the separate capture functionality:
    
    ```cs
    VideoCapture1.SeparateCapture_Enabled = true;
    ```
    
    #### Step 2: Configure Capture Mode
    
    Set the appropriate capture mode for your application:
    
    ```cs
    VideoCapture1.Mode = VideoCaptureMode.VideoCapture;
    // Other options include:
    // VideoCaptureMode.ScreenCapture
    // VideoCaptureMode.AudioCapture
    // etc.
    ```
    
    #### Step 3: Configure Output Format
    
    Set your desired output format configuration:
    
    ```cs
    VideoCapture1.Output_Format = ...
    ```
    
    #### Step 4: Start Preview
    
    Begin the preview without starting the actual recording:
    
    ```cs
    await VideoCapture1.StartAsync();
    ```
    
    #### Step 5: Start Capture When Needed
    
    When you want to begin recording, start the separate capture process:
    
    ```cs
    await VideoCapture1.SeparateCapture_StartAsync();
    ```
    
    #### Step 6: Stop Capture While Maintaining Preview
    
    To stop recording while keeping the preview active:
    
    ```cs
    await VideoCapture1.SeparateCapture_StopAsync();
    ```
    
    ### Dynamic Filename Changes
    
    A key advantage of the separate capture approach is the ability to change the output filename during an active recording session:
    
    ```cs
    await VideoCapture1.SeparateCapture_ChangeFilenameOnTheFlyAsync("newfile.mp4");
    ```
    
    This is particularly useful for:
    
    - Creating sequential file segments
    - Implementing file size limits with automatic continuation
    - Responding to user-initiated filename changes
    


## Implementation Considerations

### Memory and Performance

When implementing separate capture and preview functionality, be mindful of these performance considerations:

- **Memory Usage**: Maintaining an active preview while not capturing consumes system resources
- **CPU Impact**: Encoding operations during capture increase CPU load
- **Buffer Management**: Ensure proper buffer handling to prevent memory leaks

### UI Considerations

Your application UI should clearly indicate the current state of both preview and capture:

- Use different visual indicators for preview-only vs. active recording
- Implement appropriate UI controls for each state
- Consider adding recording timers and indicators

## Integration Best Practices

For optimal performance and reliability:

1. **Initialize Early**: Set up your capture configuration at application startup
2. **Release Resources**: Always stop capture and preview when they're no longer needed
3. **Handle Device Changes**: Implement proper detection and handling of device connection/disconnection
4. **Thread Management**: Perform capture operations on background threads to prevent UI freezing

## Conclusion

Separating video capture and preview operations provides greater flexibility and a better user experience in video applications. By following the approaches outlined in this guide, you can implement this functionality in your .NET applications with either the VideoCaptureCoreX or VideoCaptureCore components.

These techniques can be applied to a wide range of scenarios including webcam recording, screen capture, surveillance systems, and professional video production tools.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.