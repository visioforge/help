---
title: Motion Detection - Configurable Zones and Alerts in C# .NET
description: Add simple and advanced motion detection to webcam or IP camera streams with VisioForge Video Capture SDK. Configurable zones, sensitivity, and C# examples.
sidebar_label: Motion Detection
order: 6
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
primary_api_classes:
  - MotionDetectionSettings
  - MotionDetectionExSettings

---

# Motion Detection for Video Processing

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

The Video Capture SDK provides powerful motion detection capabilities for your .NET applications. Whether you need simple presence detection or advanced object tracking, the SDK offers two distinct motion detector implementations to match your specific requirements:

1. **Simple Motion Detector** - Efficient, lightweight processing with customizable detection matrices
2. **Advanced Motion Detector** - Enhanced capabilities including object detection, multiple processor types, and specialized detectors

These motion detection tools enable developers to implement sophisticated video analysis features such as security monitoring, automated alerts, object counting, and interactive motion-responsive applications.

## Simple Motion Detector

[VideoCaptureCore](#){ .md-button }

### How It Works

The simple motion detector offers an efficient solution for basic movement detection scenarios. Its streamlined approach makes it ideal for applications where processing speed and resource efficiency are priorities.

When activated, the detector:

- Processes each frame to detect movement changes
- Generates a two-dimensional byte array (motion matrix)
- Calculates the overall motion level as a percentage
- Optionally highlights detected motion areas visually

### Key Features

- **Customizable Matrix Size**: Adjust detection resolution to balance performance and accuracy
- **Channel Selection**: Analyze all RGB channels or focus on specific ones for optimized detection
- **Motion Highlighting**: Visually emphasize detected motion with color overlays
- **Performance Optimization**: Configure frame interval settings to manage processing load

### Implementation Example

```cs
// create motion detector settings
var motionDetector = new MotionDetectionSettings();

// set the motion detector matrix dimensions
motionDetector.Matrix_Width = 10;
motionDetector.Matrix_Height = 10;

// configure color channel analysis
motionDetector.Compare_Red = false;
motionDetector.Compare_Green = false;
motionDetector.Compare_Blue = false;
motionDetector.Compare_Greyscale = true;

// motion highlighting configuration
motionDetector.Highlight_Color = MotionCHLColor.Green;
motionDetector.Highlight_Enabled = true;

// performance optimization settings
motionDetector.FrameInterval = 5;
motionDetector.DropFrames_Enabled = false;

// enable detection (default is false — required for OnMotion events to fire)
motionDetector.Enabled = true;

// apply settings to the video capture component
VideoCapture1.Motion_Detection = motionDetector;
VideoCapture1.MotionDetection_Update();
```

### Retrieving Motion Data

Subscribe to the `OnMotion` event (type: `EventHandler<MotionDetectionEventArgs>`) before starting the pipeline. The handler fires once per frame when motion is detected:

```cs
VideoCapture1.OnMotion += (sender, e) =>
{
    // e.Level  — overall motion intensity (int, typically 0-100).
    // e.Matrix — byte[] grid overlay; each cell holds the motion amount for that region.
    if (e.Level > 25)
    {
        Console.WriteLine($"Motion level {e.Level}% — {e.Matrix?.Length ?? 0} grid cells");
        // Trigger recording, push an alert, enqueue a snapshot, etc.
    }
};
```

Events are raised on a background thread — marshal to the UI thread (`Dispatcher`/`Invoke`) before touching UI controls.

## Advanced Motion Detector

[VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

### Enhanced Capabilities

The advanced motion detector provides more sophisticated detection algorithms and analysis options. This detector is designed for applications requiring detailed motion information, object identification, and precise motion area definition.

Key advantages include:

- Object detection and counting
- Multiple processor types for different visual analysis needs
- Specialized detector algorithms for various environments
- Enhanced motion area processing

### Configuration Options

#### Motion Processor Types

The processor determines how motion is analyzed and visualized:

- **None**: Raw detection without visual highlighting
- **BlobCountingObjects**: Identifies and counts distinct moving objects
- **GridMotionAreaProcessing**: Divides frame into grid sections for analysis
- **MotionAreaHighlighting**: Highlights full areas where motion is detected
- **MotionBorderHighlighting**: Outlines the perimeter of motion areas

#### Motion Detector Types

The detector algorithm defines the fundamental approach to motion identification:

- **CustomFrameDifference**: Compares current frame against a predefined background
- **SimpleBackgroundModeling**: Uses adaptive background modeling techniques
- **TwoFramesDifference**: Analyzes differences between consecutive frames

### Implementation Steps

1. Create the advanced motion detector settings:

```cs
var motionDetector = new MotionDetectionExSettings();
```

2. Select the appropriate processor type for your application needs:

```cs
motionDetector.ProcessorType = MotionProcessorType.BlobCountingObjects;
```

3. Choose the detection algorithm that best suits your environment:

```cs
motionDetector.DetectorType = MotionDetectorType.CustomFrameDifference;
```

4. Apply the settings to your video capture component:

=== "VideoCaptureCoreX"

    
    ```cs
    VideoCapture1.Motion_Detection = motionDetector;
    ```
    

=== "VideoCaptureCore"

    
    ```cs
    VideoCapture1.Motion_DetectionEx = motionDetector;
    ```
    


5. Implement the corresponding event handler to receive detection data. Both events carry `MotionDetectionExEventArgs` with `Level` (float), `LevelPercent` (0-100 int), `ObjectsCount`, `ObjectRectangles` (`Rect[]`), and `MotionGrid` (`float[,]`):

    === "VideoCaptureCoreX"

        ```cs
        VideoCapture1.OnMotionDetection += (sender, e) =>
        {
            if (e.LevelPercent > 25)
            {
                Console.WriteLine($"Motion {e.LevelPercent}% — {e.ObjectsCount} moving objects");
                foreach (var rect in e.ObjectRectangles)
                {
                    Console.WriteLine($"  at {rect}");
                }
            }
        };
        ```

    === "VideoCaptureCore"

        ```cs
        VideoCapture1.OnMotionDetectionEx += (sender, e) =>
        {
            if (e.LevelPercent > 25)
            {
                Console.WriteLine($"Motion {e.LevelPercent}% — {e.ObjectsCount} moving objects");
            }
        };
        ```

    Handlers run on a worker thread — marshal to the UI thread before updating controls.

## Practical Applications

Motion detection capabilities enable developers to create powerful video-processing applications:

- **Security Systems**: Trigger recording or alerts when unauthorized movement is detected
- **Traffic Analysis**: Count and track vehicles or pedestrians
- **Interactive Installations**: Create motion-responsive digital experiences
- **Automated Video Indexing**: Mark and categorize sections containing activity
- **Industrial Automation**: Monitor production lines or restricted areas
- **Wildlife Observation**: Record animal activity without human intervention

## Performance Considerations

To optimize motion detection performance:

1. Adjust the matrix dimensions to balance accuracy and processing load
2. Use frame interval settings to analyze only essential frames
3. Select the appropriate color channels for your detection scenario
4. Consider enabling the frame dropping option for high-performance requirements
5. Choose the detector type based on your specific environment conditions

## Advanced Configuration

The advanced `MotionDetectionExSettings` class (used by `VideoCaptureCoreX` via `Motion_Detection` / by classic `VideoCaptureCore` via `Motion_DetectionEx`) exposes these additional properties:

- `DifferenceThreshold` — per-pixel difference threshold to filter out minor movements
- `MinObjectsWidth` / `MinObjectsHeight` — ignore detected objects below these dimensions
- `SuppressNoise` — noise-suppression filter to reduce false positives
- `HighlightMotionRegions` — draw bounding rectangles around detected object regions
- `KeepObjectsEdges` — preserve sharp edges when highlighting
- `HighlightColor` (SKColor) — color used for motion highlighting

## Event Integration

The motion detection events can be integrated with other SDK features:

- Video recording to capture detected motion
- Snapshot creation when motion is detected
- Custom notification systems
- Data logging and analysis

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
