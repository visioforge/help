---
title: Configure Device Input Connections with Crossbar API
description: Select and configure multiple hardware video inputs for TV tuners and capture cards in .NET with C# crossbar implementation steps.
---

# Configuring Multiple Hardware Video Inputs with Crossbar

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction to Crossbar Functionality

Many professional video capture devices such as TV tuners, capture cards, and video acquisition hardware feature multiple physical input connections. These devices might include various input types like:

- Analog video inputs (Composite, S-Video)
- Digital video inputs (HDMI, DisplayPort)
- Professional video inputs (SDI, HD-SDI)
- Television tuner inputs (RF, Cable)

The crossbar interface allows your application to programmatically select between these different hardware inputs and route the signals appropriately.

## Implementation Guide

### Step 1: Initialize Crossbar Interface

First, you need to initialize the crossbar interface for your video capture device. This establishes the connection to the hardware's input selection capabilities.

```cs
// Initialize the crossbar interface and check if crossbar functionality exists
var crossBarFound = VideoCapture1.Video_CaptureDevice_CrossBar_Init();

// If crossBarFound is true, the device supports multiple inputs that can be configured
```

### Step 2: Discover Available Input Options

After initializing, you can retrieve all available inputs that can be connected to the specified output (typically "Video Decoder").

```cs
// Clear any existing crossbar connection settings
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections();

// Clear any previous items in your UI dropdown
cbCrossbarVideoInput.Items.Clear();

// Populate the dropdown with all available input sources that can connect to "Video Decoder"
foreach (string inputSource in VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput("Video Decoder"))
{
    // Add each available input source to your UI selection element
    cbCrossbarVideoInput.Items.Add(inputSource);
}
```

### Step 3: Apply Selected Input Configuration

When the user selects their desired input source, apply this configuration to the device by connecting the selected input to the "Video Decoder" output.

```cs
// First clear any existing connections to ensure clean state
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections(); 

// Connect the selected input (from UI dropdown) to the "Video Decoder" output
// The final parameter (true) enables the connection
VideoCapture1.Video_CaptureDevice_CrossBar_Connect(cbCrossbarVideoInput.Text, "Video Decoder", true);

// At this point, the device will use the selected input for video capture
```

### Step 4: Handling Connection Changes

For optimal user experience, consider implementing event handlers to detect when users change input selection and reapply the configuration accordingly.

## Required Dependencies

To implement crossbar functionality, your application must include the appropriate video capture redistributables:

- Video capture redist packages:
  - [x86 Architecture](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 Architecture](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Troubleshooting Tips

- Not all devices support crossbar functionality - check `crossBarFound` value after initialization
- Some devices may have different output names than "Video Decoder"
- Changes may not take effect until after the capture session is started

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) repository for additional code samples and implementation examples.