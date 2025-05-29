---
title: Enable Camera Light on Windows 10+ Tablet Devices
description: Learn how to programmatically control camera light functionality on Windows 10+ tablets in .NET applications. This guide includes C# code examples, required packages, and implementation steps for developers working with video capture capabilities.
sidebar_label: Enable Camera Light on Windows 10+ Tablets
---

# Enabling Camera Light on Windows 10+ Tablets

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge variant="dark" size="xl" text="VideoCaptureCore"]

## Introduction

Modern Windows 10+ tablets come equipped with camera light functionality that developers can control programmatically. This guide explains how to implement camera light controls in your .NET applications using the TorchControl API.

## Implementation with TorchControl API

The TorchControl API provides a comprehensive way to manage camera lights on Windows 10+ tablets. This API offers:

- Device discovery for torch-compatible cameras
- Granular control for enabling and disabling camera lights
- Cross-device compatibility

### Basic Implementation Steps

1. Initialize the VideoCaptureCore component
2. Get available devices with torch capabilities
3. Enable or disable torch functionality for specific devices

## Working Code Example

```cs
// Initialize VideoCaptureCore
VideoCaptureCore videoCapture = await VideoCaptureCore.CreateAsync();

// Get available devices with torch capability
string[] devices = await videoCapture.TorchControl_GetDevicesAsync();

// Enable torch for the first available device
if (devices.Length > 0)
{
    await videoCapture.TorchControl_EnableAsync(devices[0], true);
}

// Disable torch when needed
await videoCapture.TorchControl_EnableAsync(devices[0], false);
```

## Complete Implementation Example

```cs
using System;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.WindowsExtensions;

namespace Camera_Light_Demo
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore VideoCapture1;
        private string[] _devices;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (VideoCapture1 != null)
            {
                VideoCapture1.Dispose();
                VideoCapture1 = null;
            }
        }

        private async void btTurnOn_Click(object sender, EventArgs e)
        {
            if (_devices.Length > 0)
            {
                await VideoCapture1.TorchControl_EnableAsync(_devices[0], true);
            }
        }

        private async void btTurnOff_Click(object sender, EventArgs e)
        {
            if (_devices.Length > 0)
            {
                await VideoCapture1.TorchControl_EnableAsync(_devices[0], false);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            VideoCapture1 = await VideoCaptureCore.CreateAsync();

            _devices = await VideoCapture1.TorchControl_GetDevicesAsync();
            lbDeviceCount.Text = $"Devices found: {_devices.Length}.";
        }
    }
}
```

## Required Dependencies

To implement camera light functionality in your application, you'll need:

1. **NuGet Package**: Install the [VisioForge.DotNet.Core.WindowsExtensions](https://www.nuget.org/packages/VisioForge.DotNet.Core.WindowsExtensions) package.

2. **Video Capture Redistributables**:
   - [x86 Redist Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
   - [x64 Redist Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Complete Sample Application

For a fully functional implementation, explore our [Camera Light Demo application](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Camera%20Light%20Demo) available in our GitHub repository.

## Compatibility Notes

- This functionality is primarily designed for Windows 10 and newer tablet devices
- Device hardware must support programmable camera light control
- Some device manufacturers may implement proprietary APIs that require additional configuration

## Troubleshooting Tips

If you encounter issues enabling the camera light:

- Verify that your device has compatible hardware
- Ensure all required packages are properly installed
- Check device permissions in your application manifest
- Make sure the device reports as torch-capable with TorchControl_GetDevicesAsync()

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to access more code samples and examples.
