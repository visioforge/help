---
title: .Net Media OpenCV Blocks Guide
description: Implement computer vision tasks with OpenCV blocks for object detection, tracking, image manipulation, and video processing in .NET SDKs.
sidebar_label: OpenCV
---

# OpenCV Blocks - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

OpenCV (Open Source Computer Vision Library) blocks provide powerful video processing capabilities within the VisioForge Media Blocks SDK .Net. These blocks enable a wide range of computer vision tasks, from basic image manipulation to complex object detection and tracking.

To use OpenCV blocks, ensure that the VisioForge.CrossPlatform.OpenCV.Windows.x64 (or corresponding package for your platform) NuGet package is included in your project.

Most OpenCV blocks typically require a `videoconvert` element before them to ensure the input video stream is in a compatible format. The SDK handles this internally when you initialize the block.

## CV Dewarp Block

The CV Dewarp block applies dewarping effects to a video stream, which can correct distortions from wide-angle lenses, for example.

### Block info

Name: `CVDewarpBlock` (GStreamer element: `dewarp`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVDewarpBlock` is configured using `CVDewarpSettings`. Key properties:

- `DisplayMode` (`CVDewarpDisplayMode` enum): Specifies the display mode for dewarping (e.g., `SinglePanorama`, `DoublePanorama`). Default is `CVDewarpDisplayMode.SinglePanorama`.
- `InnerRadius` (double): Inner radius for dewarping.
- `InterpolationMethod` (`CVDewarpInterpolationMode` enum): Interpolation method used (e.g., `Bilinear`, `Bicubic`). Default is `CVDewarpInterpolationMode.Bilinear`.
- `OuterRadius` (double): Outer radius for dewarping.
- `XCenter` (double): X-coordinate of the center for dewarping.
- `XRemapCorrection` (double): X-coordinate remap correction factor.
- `YCenter` (double): Y-coordinate of the center for dewarping.
- `YRemapCorrection` (double): Y-coordinate remap correction factor.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVDewarpBlock;
    CVDewarpBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

// Create Dewarp settings
var dewarpSettings = new CVDewarpSettings
{
    DisplayMode = CVDewarpDisplayMode.SinglePanorama, // Example mode, default is SinglePanorama
    InnerRadius = 0.2, // Example value
    OuterRadius = 0.8, // Example value
    XCenter = 0.5,     // Example value, default is 0.5
    YCenter = 0.5,     // Example value, default is 0.5
    // InterpolationMethod = CVDewarpInterpolationMode.Bilinear, // This is the default
};

var dewarpBlock = new CVDewarpBlock(dewarpSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, dewarpBlock.Input0);
pipeline.Connect(dewarpBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project.

## CV Dilate Block

The CV Dilate block performs a dilation operation on the video stream. Dilation is a morphological operation that typically expands bright regions and shrinks dark regions.

### Block info

Name: `CVDilateBlock` (GStreamer element: `cvdilate`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

This block does not have specific settings beyond the default behavior. The dilation is performed with a default structuring element.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVDilateBlock;
    CVDilateBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var dilateBlock = new CVDilateBlock();

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, dilateBlock.Input0);
pipeline.Connect(dilateBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project.

## CV Edge Detect Block

The CV Edge Detect block uses the Canny edge detector algorithm to find edges in the video stream.

### Block info

Name: `CVEdgeDetectBlock` (GStreamer element: `edgedetect`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVEdgeDetectBlock` is configured using `CVEdgeDetectSettings`. Key properties:

- `ApertureSize` (int): Aperture size for the Sobel operator (e.g., 3, 5, or 7). Default is 3.
- `Threshold1` (int): First threshold for the hysteresis procedure. Default is 50.
- `Threshold2` (int): Second threshold for the hysteresis procedure. Default is 150.
- `Mask` (bool): If true, the output is a mask; otherwise, it's the original image with edges highlighted. Default is `false`.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVEdgeDetectBlock;
    CVEdgeDetectBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var edgeDetectSettings = new CVEdgeDetectSettings
{
    ApertureSize = 3, // Example value, default is 3
    Threshold1 = 2000, // Example value, actual C# type is int, default is 50
    Threshold2 = 4000, // Example value, actual C# type is int, default is 150
    Mask = true       // Example value, default is false
};

var edgeDetectBlock = new CVEdgeDetectBlock(edgeDetectSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, edgeDetectBlock.Input0);
pipeline.Connect(edgeDetectBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project.

## CV Equalize Histogram Block

The CV Equalize Histogram block equalizes the histogram of a video frame using the `cvEqualizeHist` function. This typically improves the contrast of the image.

### Block info

Name: `CVEqualizeHistogramBlock` (GStreamer element: `cvequalizehist`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

This block does not have specific settings beyond the default behavior.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVEqualizeHistogramBlock;
    CVEqualizeHistogramBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var equalizeHistBlock = new CVEqualizeHistogramBlock();

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, equalizeHistBlock.Input0);
pipeline.Connect(equalizeHistBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project.

## CV Erode Block

The CV Erode block performs an erosion operation on the video stream. Erosion is a morphological operation that typically shrinks bright regions and expands dark regions.

### Block info

Name: `CVErodeBlock` (GStreamer element: `cverode`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

This block does not have specific settings beyond the default behavior. The erosion is performed with a default structuring element.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVErodeBlock;
    CVErodeBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var erodeBlock = new CVErodeBlock();

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, erodeBlock.Input0);
pipeline.Connect(erodeBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project.

## CV Face Blur Block

The CV Face Blur block detects faces in the video stream and applies a blur effect to them.

### Block info

Name: `CVFaceBlurBlock` (GStreamer element: `faceblur`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVFaceBlurBlock` is configured using `CVFaceBlurSettings`. Key properties:

- `MainCascadeFile` (string): Path to the XML file for the primary Haar cascade classifier used for face detection (e.g., `haarcascade_frontalface_default.xml`). Default is `"haarcascade_frontalface_default.xml"`.
- `MinNeighbors` (int): Minimum number of neighbors each candidate rectangle should have to retain it. Default is 3.
- `MinSize` (`Size`): Minimum possible object size. Objects smaller than this are ignored. Default `new Size(30, 30)`.
- `ScaleFactor` (double): How much the image size is reduced at each image scale. Default is 1.25.

Note: `ProcessPaths(Context)` should be called on the settings object to ensure correct path resolution for cascade files.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVFaceBlurBlock;
    CVFaceBlurBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var faceBlurSettings = new CVFaceBlurSettings
{
    MainCascadeFile = "haarcascade_frontalface_default.xml", // Adjust path as needed, this is the default
    MinNeighbors = 5, // Example value, default is 3
    ScaleFactor = 1.2, // Example value, default is 1.25
    // MinSize = new VisioForge.Core.Types.Size(30, 30) // This is the default
};
// It's important to call ProcessPaths if you are not providing an absolute path
// and relying on SDK's internal mechanisms to locate the file, especially when deployed.
// faceBlurSettings.ProcessPaths(pipeline.Context); // or pass appropriate context

var faceBlurBlock = new CVFaceBlurBlock(faceBlurSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, faceBlurBlock.Input0);
pipeline.Connect(faceBlurBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

This block requires Haar cascade XML files for face detection. These files are typically bundled with OpenCV distributions. Ensure the path to `MainCascadeFile` is correctly specified. The `ProcessPaths` method on the settings object can help resolve paths if files are placed in standard locations known to the SDK.

## CV Face Detect Block

The CV Face Detect block detects faces, and optionally eyes, noses, and mouths, in the video stream using Haar cascade classifiers.

### Block info

Name: `CVFaceDetectBlock` (GStreamer element: `facedetect`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVFaceDetectBlock` is configured using `CVFaceDetectSettings`. Key properties:

- `Display` (bool): If `true`, draws rectangles around detected features on the output video. Default is `true`.
- `MainCascadeFile` (string): Path to the XML for the primary Haar cascade. Default is `"haarcascade_frontalface_default.xml"`.
- `EyesCascadeFile` (string): Path to the XML for eyes detection. Default is `"haarcascade_mcs_eyepair_small.xml"`. Optional.
- `NoseCascadeFile` (string): Path to the XML for nose detection. Default is `"haarcascade_mcs_nose.xml"`. Optional.
- `MouthCascadeFile` (string): Path to the XML for mouth detection. Default is `"haarcascade_mcs_mouth.xml"`. Optional.
- `MinNeighbors` (int): Minimum neighbors for candidate retention. Default 3.
- `MinSize` (`Size`): Minimum object size. Default `new Size(30, 30)`.
- `MinDeviation` (int): Minimum standard deviation. Default 0.
- `ScaleFactor` (double): Image size reduction factor at each scale. Default 1.25.
- `UpdatesMode` (`CVFaceDetectUpdates` enum): Controls how updates/events are posted (`EveryFrame`, `OnChange`, `OnFace`, `None`). Default `CVFaceDetectUpdates.EveryFrame`.

Note: `ProcessPaths(Context)` should be called on the settings object for cascade files.

### Events

- `FaceDetected`: Occurs when faces (and other enabled features) are detected. Provides `CVFaceDetectedEventArgs` with an array of `CVFace` objects and a timestamp.
  - `CVFace` contains `Rect` for `Position`, `Nose`, `Mouth`, and a list of `Rect` for `Eyes`.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVFaceDetectBlock;
    CVFaceDetectBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var faceDetectSettings = new CVFaceDetectSettings
{
    MainCascadeFile = "haarcascade_frontalface_default.xml", // Adjust path, default
    EyesCascadeFile = "haarcascade_mcs_eyepair_small.xml", // Adjust path, default, optional
    // NoseCascadeFile = "haarcascade_mcs_nose.xml", // Optional, default
    // MouthCascadeFile = "haarcascade_mcs_mouth.xml", // Optional, default
    Display = true, // Default
    UpdatesMode = CVFaceDetectUpdates.EveryFrame, // Default, possible values: EveryFrame, OnChange, OnFace, None
    MinNeighbors = 5, // Example value, default is 3
    ScaleFactor = 1.2, // Example value, default is 1.25
    // MinSize = new VisioForge.Core.Types.Size(30,30) // Default
};
// faceDetectSettings.ProcessPaths(pipeline.Context); // or appropriate context

var faceDetectBlock = new CVFaceDetectBlock(faceDetectSettings);

faceDetectBlock.FaceDetected += (s, e) => 
{
    Console.WriteLine($"Timestamp: {e.Timestamp}, Faces found: {e.Faces.Length}");
    foreach (var face in e.Faces)
    {
        Console.WriteLine($"  Face at [{face.Position.Left},{face.Position.Top},{face.Position.Width},{face.Position.Height}]");
        if (face.Eyes.Any())
        {
            Console.WriteLine($"    Eyes at [{face.Eyes[0].Left},{face.Eyes[0].Top},{face.Eyes[0].Width},{face.Eyes[0].Height}]");
        }
    }
};

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, faceDetectBlock.Input0);
pipeline.Connect(faceDetectBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Requires Haar cascade XML files. The `ProcessBusMessage` method in the C# class handles parsing messages from the GStreamer element to fire the `FaceDetected` event.

## CV Hand Detect Block

The CV Hand Detect block detects hand gestures (fist or palm) in the video stream using Haar cascade classifiers. It internally resizes the input video to 320x240 for processing.

### Block info

Name: `CVHandDetectBlock` (GStreamer element: `handdetect`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVHandDetectBlock` is configured using `CVHandDetectSettings`. Key properties:

- `Display` (bool): If `true`, draws rectangles around detected hands on the output video. Default is `true`.
- `FistCascadeFile` (string): Path to the XML for fist detection. Default is `"fist.xml"`.
- `PalmCascadeFile` (string): Path to the XML for palm detection. Default is `"palm.xml"`.
- `ROI` (`Rect`): Region Of Interest for detection. Coordinates are relative to the 320x240 processed image. Default (0,0,0,0) - full frame (corresponds to `new Rect()`).

Note: `ProcessPaths(Context)` should be called on the settings object for cascade files.

### Events

- `HandDetected`: Occurs when hands are detected. Provides `CVHandDetectedEventArgs` with an array of `CVHand` objects.
  - `CVHand` contains `Rect` for `Position` and `CVHandGesture` for `Gesture` (Fist or Palm).

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVHandDetectBlock;
    CVHandDetectBlock-->VideoRendererBlock;
```

Note: The `CVHandDetectBlock` internally includes a `videoscale` element to resize input to 320x240 before the `handdetect` GStreamer element.

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var handDetectSettings = new CVHandDetectSettings
{
    FistCascadeFile = "fist.xml", // Adjust path, default
    PalmCascadeFile = "palm.xml", // Adjust path, default
    Display = true, // Default
    ROI = new VisioForge.Core.Types.Rect(0, 0, 320, 240) // Example: full frame of scaled image, default is new Rect()
};
// handDetectSettings.ProcessPaths(pipeline.Context); // or appropriate context

var handDetectBlock = new CVHandDetectBlock(handDetectSettings);

handDetectBlock.HandDetected += (s, e) => 
{
    Console.WriteLine($"Hands found: {e.Hands.Length}");
    foreach (var hand in e.Hands)
    {
        Console.WriteLine($"  Hand at [{hand.Position.Left},{hand.Position.Top},{hand.Position.Width},{hand.Position.Height}], Gesture: {hand.Gesture}");
    }
};

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, handDetectBlock.Input0);
pipeline.Connect(handDetectBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Requires Haar cascade XML files for fist and palm detection. The input video is internally scaled to 320x240 for processing by the `handdetect` element. The `ProcessBusMessage` method handles GStreamer messages to fire `HandDetected`.

## CV Laplace Block

The CV Laplace block applies a Laplace operator to the video stream, which highlights regions of rapid intensity change, often used for edge detection.

### Block info

Name: `CVLaplaceBlock` (GStreamer element: `cvlaplace`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVLaplaceBlock` is configured using `CVLaplaceSettings`. Key properties:

- `ApertureSize` (int): Aperture size for the Sobel operator used internally (e.g., 1, 3, 5, or 7). Default 3.
- `Scale` (double): Optional scale factor for the computed Laplacian values. Default 1.
- `Shift` (double): Optional delta value that is added to the results prior to storing them. Default 0.
- `Mask` (bool): If true, the output is a mask; otherwise, it's the original image with the effect applied. Default is true.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVLaplaceBlock;
    CVLaplaceBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var laplaceSettings = new CVLaplaceSettings
{
    ApertureSize = 3, // Example value
    Scale = 1.0,      // Example value
    Shift = 0.0,      // Example value
    Mask = true
};

var laplaceBlock = new CVLaplaceBlock(laplaceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, laplaceBlock.Input0);
pipeline.Connect(laplaceBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project.

## CV Motion Cells Block

The CV Motion Cells block detects motion in a video stream by dividing the frame into a grid of cells and analyzing changes within these cells.

### Block info

Name: `CVMotionCellsBlock` (GStreamer element: `motioncells`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVMotionCellsBlock` is configured using `CVMotionCellsSettings`. Key properties:

- `CalculateMotion` (bool): Enable or disable motion calculation. Default `true`.
- `CellsColor` (`SKColor`): Color to draw motion cells if `Display` is true. Default `SKColors.Red`.
- `DataFile` (string): Path to a data file for loading/saving cell configuration. Extension is handled separately by `DataFileExtension`.
- `DataFileExtension` (string): Extension for the data file (e.g., "dat").
- `Display` (bool): If `true`, draws the grid and motion indication on the output video. Default `true`.
- `Gap` (`TimeSpan`): Interval after which motion is considered finished and a "motion finished" bus message is posted. Default `TimeSpan.FromSeconds(5)`. (Note: This is different from a pixel gap between cells).
- `GridSize` (`Size`): Number of cells in the grid (Width x Height). Default `new Size(10, 10)`.
- `MinimumMotionFrames` (int): Minimum number of frames motion must be detected in a cell to trigger. Default 1.
- `MotionCellsIdx` (string): Comma-separated string of cell indices (e.g., "0:0,1:1") to monitor for motion.
- `MotionCellBorderThickness` (int): Thickness of the border for cells with detected motion. Default 1.
- `MotionMaskCellsPos` (string): String defining cell positions for a motion mask.
- `MotionMaskCoords` (string): String defining coordinates for a motion mask.
- `PostAllMotion` (bool): Post all motion events. Default `false`.
- `PostNoMotion` (`TimeSpan`): Time after which a "no motion" event is posted if no motion is detected. Default `TimeSpan.Zero` (disabled).
- `Sensitivity` (double): Motion sensitivity. Expected range might be 0.0 to 1.0. Default `0.5`.
- `Threshold` (double): Threshold for motion detection, representing the fraction of cells that need to have moved. Default `0.01`.
- `UseAlpha` (bool): Use alpha channel for drawing. Default `true`.

### Events

- `MotionDetected`: Occurs when motion is detected or changes state. Provides `CVMotionCellsEventArgs`:
  - `Cells`: String indicating which cells have motion (e.g., "0:0,1:2").
  - `StartedTime`: Timestamp when motion began in the current event scope.
  - `FinishedTime`: Timestamp when motion finished (if applicable to the event).
  - `CurrentTime`: Timestamp of the current frame related to the event.
  - `IsMotion`: Boolean indicating if the event signifies motion (`true`) or no motion (`false`).

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVMotionCellsBlock;
    CVMotionCellsBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var motionCellsSettings = new CVMotionCellsSettings
{
    GridSize = new VisioForge.Core.Types.Size(8, 6), // Example: 8x6 grid, default is new Size(10,10)
    Sensitivity = 0.75, // Example value, C# default is 0.5. Represents sensitivity.
    Threshold = 0.05,   // Example value, C# default is 0.01. Represents fraction of moved cells.
    Display = true,     // Default is true
    CellsColor = SKColors.Aqua, // Example color, default is SKColors.Red
    PostNoMotion = TimeSpan.FromSeconds(5) // Post no_motion after 5s of inactivity, default is TimeSpan.Zero
};

var motionCellsBlock = new CVMotionCellsBlock(motionCellsSettings);

motionCellsBlock.MotionDetected += (s, e) => 
{
    if (e.IsMotion)
    {
        Console.WriteLine($"Motion DETECTED at {e.CurrentTime}. Cells: {e.Cells}. Started: {e.StartedTime}");
    }
    else
    {
        Console.WriteLine($"Motion FINISHED or NO MOTION at {e.CurrentTime}. Finished: {e.FinishedTime}");
    }
};

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, motionCellsBlock.Input0);
pipeline.Connect(motionCellsBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

The `ProcessBusMessage` method handles GStreamer messages to fire `MotionDetected`. Event structure provides timestamps for motion start, finish, and current event time.

## CV Smooth Block

The CV Smooth block applies various smoothing (blurring) filters to the video stream.

### Block info

Name: `CVSmoothBlock` (GStreamer element: `cvsmooth`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVSmoothBlock` is configured using `CVSmoothSettings`. Key properties:

- `Type` (`CVSmoothType` enum): Type of smoothing filter to apply (`Blur`, `Gaussian`, `Median`, `Bilateral`). Default `CVSmoothType.Gaussian`.
- `KernelWidth` (int): Width of the kernel for `Blur`, `Gaussian`, `Median` filters. Default 3.
- `KernelHeight` (int): Height of the kernel for `Blur`, `Gaussian`, `Median` filters. Default 3.
- `Width` (int): Width of the area to blur. Default `int.MaxValue` (full frame).
- `Height` (int): Height of the area to blur. Default `int.MaxValue` (full frame).
- `PositionX` (int): X position for the blur area. Default 0.
- `PositionY` (int): Y position for the blur area. Default 0.
- `Color` (double): Sigma for color space (for Bilateral filter) or standard deviation (for Gaussian if `SpatialSigma` is 0). Default 0.
- `SpatialSigma` (double): Sigma for coordinate space (for Bilateral and Gaussian filters). For Gaussian, if 0, it's calculated from `KernelWidth`/`KernelHeight`. Default 0.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVSmoothBlock;
    CVSmoothBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var smoothSettings = new CVSmoothSettings
{
    Type = CVSmoothType.Gaussian, // Example: Gaussian blur, also the default
    KernelWidth = 5,  // Kernel width, default is 3
    KernelHeight = 5, // Kernel height, default is 3
    SpatialSigma = 1.5 // Sigma for Gaussian. If 0 (default), it's calculated from kernel size.
};

var smoothBlock = new CVSmoothBlock(smoothSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, smoothBlock.Input0);
pipeline.Connect(smoothBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project. The specific parameters used by the GStreamer element (`color`, `spatial`, `kernel-width`, `kernel-height`) depend on the chosen `Type`. For kernel dimensions, use `KernelWidth` and `KernelHeight`. `Width` and `Height` define the area to apply the blur if not the full frame.

## CV Sobel Block

The CV Sobel block applies a Sobel operator to the video stream, which is used to calculate the derivative of an image intensity function, typically for edge detection.

### Block info

Name: `CVSobelBlock` (GStreamer element: `cvsobel`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVSobelBlock` is configured using `CVSobelSettings`. Key properties:

- `XOrder` (int): Order of the derivative x. Default 1.
- `YOrder` (int): Order of the derivative y. Default 1.
- `ApertureSize` (int): Size of the extended Sobel kernel (1, 3, 5, or 7). Default 3.
- `Mask` (bool): If true, the output is a mask; otherwise, it's the original image with the effect applied. Default is true.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVSobelBlock;
    CVSobelBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var sobelSettings = new CVSobelSettings
{
    XOrder = 1,       // Default is 1. Used for order of the derivative X.
    YOrder = 0,       // Example: Use 0 for Y-order to primarily detect vertical edges. C# class default is 1.
    ApertureSize = 3, // Default is 3. Size of the extended Sobel kernel.
    Mask = true       // Default is true. Output as a mask.
};

var sobelBlock = new CVSobelBlock(sobelSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, sobelBlock.Input0);
pipeline.Connect(sobelBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced in your project.

## CV Template Match Block

The CV Template Match block searches for occurrences of a template image within the video stream.

### Block info

Name: `CVTemplateMatchBlock` (GStreamer element: `templatematch`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVTemplateMatchBlock` is configured using `CVTemplateMatchSettings`. Key properties:

- `TemplateImage` (string): Path to the template image file (e.g., PNG, JPG) to search for.
- `Method` (`CVTemplateMatchMethod` enum): The comparison method to use (e.g., `Sqdiff`, `CcorrNormed`, `CcoeffNormed`). Default `CVTemplateMatchMethod.Correlation`.
- `Display` (bool): If `true`, draws a rectangle around the best match on the output video. Default `true`.

### Events

- `TemplateMatch`: Occurs when a template match is found. Provides `CVTemplateMatchEventArgs`:
  - `Rect`: A `Types.Rect` object representing the location (x, y, width, height) of the best match.
  - `Result`: A double value representing the quality or result of the match, depending on the method used.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVTemplateMatchBlock;
    CVTemplateMatchBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'
// Ensure "template.png" exists and is accessible.
var templateMatchSettings = new CVTemplateMatchSettings("path/to/your/template.png") // Adjust path as needed
{
    // Method: Specifies the comparison method.
    // Example: CVTemplateMatchMethod.CcoeffNormed is often a good choice.
    // C# class default is CVTemplateMatchMethod.Correlation.
    Method = CVTemplateMatchMethod.CcoeffNormed, 
    
    // Display: If true, draws a rectangle around the best match.
    // C# class default is true.
    Display = true 
};

var templateMatchBlock = new CVTemplateMatchBlock(templateMatchSettings);

templateMatchBlock.TemplateMatch += (s, e) => 
{
    Console.WriteLine($"Template matched at [{e.Rect.Left},{e.Rect.Top},{e.Rect.Width},{e.Rect.Height}] with result: {e.Result}");
};

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, templateMatchBlock.Input0);
pipeline.Connect(templateMatchBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package and a valid template image file are available. The `ProcessBusMessage` method handles GStreamer messages to fire the `TemplateMatch` event.

## CV Text Overlay Block

The CV Text Overlay block renders text onto the video stream using OpenCV drawing functions.

### Block info

Name: `CVTextOverlayBlock` (GStreamer element: `opencvtextoverlay`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVTextOverlayBlock` is configured using `CVTextOverlaySettings`. Key properties:

- `Text` (string): The text string to overlay. Default: `"Default text"`.
- `X` (int): X-coordinate of the bottom-left corner of the text string. Default: `50`.
- `Y` (int): Y-coordinate of the bottom-left corner of the text string (from the top, OpenCV origin is top-left, GStreamer textoverlay might be bottom-left). Default: `50`.
- `FontWidth` (double): Font scale factor that is multiplied by the font-specific base size. Default: `1.0`.
- `FontHeight` (double): Font scale factor (similar to FontWidth, though GStreamer element usually has one `font-scale` or relies on point size). Default: `1.0`.
- `FontThickness` (int): Thickness of the lines used to draw a text. Default: `1`.
- `Color` (`SKColor`): Color of the text. Default: `SKColors.Black`.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVTextOverlayBlock;
    CVTextOverlayBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var textOverlaySettings = new CVTextOverlaySettings
{
    Text = "VisioForge MediaBlocks.Net ROCKS!", // Default: "Default text"
    X = 20, // X position of the text start. Default: 50
    Y = 40, // Y position of the text baseline (from top). Default: 50
    FontWidth = 1.2, // Font scale. Default: 1.0
    FontHeight = 1.2, // Font scale (usually FontWidth is sufficient for opencvtextoverlay). Default: 1.0
    FontThickness = 2, // Default: 1
    Color = SKColors.Blue // Default: SKColors.Black
};

var textOverlayBlock = new CVTextOverlayBlock(textOverlaySettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, textOverlayBlock.Input0);
pipeline.Connect(textOverlayBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced. The GStreamer properties `colorR`, `colorG`, `colorB` are set based on the `Color` property.

## CV Tracker Block

The CV Tracker block implements various object tracking algorithms to follow an object defined by an initial bounding box in a video stream.

### Block info

Name: `CVTrackerBlock` (GStreamer element: `cvtracker`).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Input video   | Uncompressed video | 1          |
| Output video  | Uncompressed video | 1          |

### Settings

The `CVTrackerBlock` is configured using `CVTrackerSettings`. Key properties:

- `Algorithm` (`CVTrackerAlgorithm` enum): Specifies the tracking algorithm (`Boosting`, `CSRT`, `KCF`, `MedianFlow`, `MIL`, `MOSSE`, `TLD`). Default: `CVTrackerAlgorithm.MedianFlow`.
- `InitialRect` (`Rect`): The initial bounding box (Left, Top, Width, Height) of the object to track. Default: `new Rect(50, 50, 100, 100)`.
- `DrawRect` (bool): If `true`, draws a rectangle around the tracked object on the output video. Default: `true`.

### Sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->CVTrackerBlock;
    CVTrackerBlock-->VideoRendererBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Assuming SystemVideoSourceBlock is already created and configured as 'videoSource'

var trackerSettings = new CVTrackerSettings
{
    Algorithm = CVTrackerAlgorithm.CSRT, // CSRT is often a good general-purpose tracker. Default: CVTrackerAlgorithm.MedianFlow
    InitialRect = new VisioForge.Core.Types.Rect(150, 120, 80, 80), // Define your initial object ROI. Default: new Rect(50, 50, 100, 100)
    DrawRect = true // Default: true
};

var trackerBlock = new CVTrackerBlock(trackerSettings);

// Note: The tracker initializes with InitialRect. 
// To re-initialize tracking on a new object/location at runtime:
// 1. Pause or Stop the pipeline.
// 2. Update trackerBlock.Settings.InitialRect (or create new CVTrackerSettings).
//    It's generally safer to update settings on a stopped/paused pipeline, 
//    or if the block/element supports dynamic property changes, that might be an option.
//    Directly modifying `trackerBlock.Settings.InitialRect` might not re-initialize the underlying GStreamer element.
//    You may need to remove and re-add the block, or check SDK documentation for live update capabilities.
// 3. Resume/Start the pipeline.

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(videoSource.Output, trackerBlock.Input0);
pipeline.Connect(trackerBlock.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

### Remarks

Ensure the VisioForge OpenCV NuGet package is referenced. The choice of tracking algorithm can significantly impact performance and accuracy. Some algorithms (like CSRT, KCF) are generally more robust than older ones (like Boosting, MedianFlow). Some trackers might require OpenCV contrib modules to be available in your OpenCV build/distribution.

