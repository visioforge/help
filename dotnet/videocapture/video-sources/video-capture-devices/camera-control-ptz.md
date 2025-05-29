---
title: Advanced Camera Control & PTZ for .NET SDK
description: Implement powerful camera control features including Pan, Tilt, Zoom (PTZ), Exposure, Iris, and Focus in your .NET applications. This detailed guide with C# code samples shows how to access and manipulate camera parameters for professional video capture solutions.
sidebar_label: Camera control and PTZ

---

# Advanced Camera Control and PTZ Implementation

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge variant="dark" size="xl" text="VideoCaptureCore"]

## Overview of Camera Control Capabilities

The Camera Control API provides developers with direct access to manipulate various camera parameters when working with compatible devices. Depending on your camera hardware specifications, you can programmatically control the following features:

- **Pan** - Horizontal movement control
- **Tilt** - Vertical movement control
- **Roll** - Rotational movement along the lens axis
- **Zoom** - Magnification level adjustment
- **Exposure** - Light sensitivity settings
- **Iris** - Aperture control for light admission
- **Focus** - Image clarity and sharpness adjustment

**Important Note:** The Camera Control API requires an active camera preview or capture session to function properly. You must start the preview or capture before attempting to access control features.

## Implementation Guide with Examples

Below you'll find practical implementation patterns that demonstrate how to integrate camera control features in your .NET applications.

### Interface Components

For optimal user interaction, consider implementing the following UI elements:

- Slider controls for parameter adjustment
- Checkboxes for toggling auto/manual modes
- Labels for displaying current, minimum, and maximum values

You can reference the Main Demo source code for a complete implementation example.

### Step 1: Reading Camera Parameter Capabilities

First, query the camera to determine the supported ranges and default values for each control parameter:

```cs
// Initialize variables to store camera parameter information
int max;
int defaultValue;
int min;
int step;
CameraControlFlags flags;

// Query the camera for the supported range of the Zoom parameter
if (await VideoCapture1.Video_CaptureDevice_CameraControl_GetRangeAsync(
    CameraControlProperty.Zoom, out min, out max, out step, out defaultValue, out flags))
{
    // Configure slider control with the camera's supported range
    tbCCZoom.Minimum = min;
    tbCCZoom.Maximum = max;
    tbCCZoom.SmallChange = step;
    tbCCZoom.Value = defaultValue;

    // Update UI labels with range information
    lbCCZoomMin.Text = "Min: " + Convert.ToString(min);
    lbCCZoomMax.Text = "Max: " + Convert.ToString(max);
    lbCCZoomCurrent.Text = "Current: " + Convert.ToString(defaultValue);

    // Set control mode checkboxes based on camera capabilities
    cbCCZoomManual.Checked = (flags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
    cbCCZoomAuto.Checked = (flags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
    cbCCZoomRelative.Checked = (flags & CameraControlFlags.Relative) == CameraControlFlags.Relative;
}
```

**Technical Note:** When the Auto flag is enabled, the camera will ignore all other flags and manual value settings. This follows industry-standard camera control protocols.

### Step 2: Applying Parameter Changes

When users adjust settings through your interface, apply the changes to the camera with this pattern:

```cs
// Initialize control flags
CameraControlFlags flags = CameraControlFlags.None;

// Build the flags based on UI checkbox states
if (cbCCZoomManual.Checked)
{
    // Enable manual control mode
    flags = flags | CameraControlFlags.Manual;
}

if (cbCCZoomAuto.Checked)
{
    // Enable automatic control mode (will override manual settings)
    flags = flags | CameraControlFlags.Auto;
}

if (cbCCZoomRelative.Checked)
{
    // Enable relative value mode (changes are relative to current position)
    flags = flags | CameraControlFlags.Relative;
}

// Apply the new zoom value with the specified control flags
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(CameraControlProperty.Zoom, tbCCZoom.Value, flags);
```

## Error Handling and Best Practices

When implementing camera control features, consider these best practices:

- Always check if a parameter is supported before attempting to set it
- Implement proper error handling for unsupported features
- Provide feedback to users when a command fails
- Remember that camera capabilities vary widely between manufacturers and models

## Required Dependencies  

For proper functionality, ensure your application includes these redistributable packages:

- Video capture redistributables:
  - [x86 Architecture](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 Architecture](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Additional Resources

For more examples and complete implementation details, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) containing numerous code samples and demo applications.
