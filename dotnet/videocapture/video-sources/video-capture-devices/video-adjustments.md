---
title: Mastering Camera Image Controls in .NET
description: Control camera settings including brightness, contrast, hue, and saturation in .NET with Video Capture SDK code examples for adjustments.
---

# Mastering Camera Image Controls in .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction to Camera Hardware Adjustments

When developing applications that utilize webcams or other video capture devices, having precise control over image quality parameters is essential for creating professional-grade software. The `VideoHardwareAdjustment` class provides developers with powerful tools to programmatically adjust camera settings such as brightness, contrast, hue, saturation, sharpness, gamma, and more.

## Supported Hardware Adjustment Properties

The SDK supports numerous adjustment properties including:

- Brightness
- Contrast
- Hue
- Saturation
- Sharpness
- Gamma
- White Balance
- Backlight Compensation
- Gain
- Exposure

Note that not all cameras support every property. The SDK will gracefully ignore any property not supported by the specific camera hardware you're using.

## Working with Camera Adjustments

### Getting Property Value Ranges

Before setting adjustment values, it's important to understand the valid range for each property. Use the `Video_CaptureDevice_VideoAdjust_GetRangesAsync` method to retrieve the minimum, maximum, default values, and step size for any adjustment property:

```cs
// Retrieve the valid range of values for the brightness property
// This helps understand the boundaries within which you can adjust settings
var brightnessRange = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRangesAsync(VideoHardwareAdjustment.Brightness);

// Check the minimum and maximum allowed values
int minValue = brightnessRange.Min;
int maxValue = brightnessRange.Max;
int defaultValue = brightnessRange.Default;
int step = brightnessRange.SteppingDelta;
```

### Setting Adjustment Values

Once you know the valid range, you can set a specific value using the `Video_CaptureDevice_VideoAdjust_SetValueAsync` method:

```cs
// Create an adjustment value object with your desired settings
// You can specify a custom value and whether automatic adjustment should be used
var adjustmentValue = new VideoCaptureDeviceAdjustValue(75, false); // Value: 75, Auto: false

// Apply the brightness adjustment to the camera
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Brightness, adjustmentValue);
```

### Retrieving Current Values

To read the current value of any adjustment property, use the `Video_CaptureDevice_VideoAdjust_GetValueAsync` method:

```cs
// Get the current brightness setting from the camera
// This returns both the current value and whether auto-adjustment is enabled
var currentBrightness = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetValueAsync(VideoHardwareAdjustment.Brightness);

// Access the current value and auto flag
int value = currentBrightness.Value;
bool isAuto = currentBrightness.Auto;
```

## Best Practices for Camera Adjustments

1. **Always check ranges first**: Different camera models have different valid ranges for each property.
2. **Handle unsupported properties**: Always implement error handling for properties that might not be supported.
3. **Consider saving user preferences**: Store adjustment values in application settings for a consistent experience.
4. **Provide UI controls**: Create sliders with proper min/max values based on the ranges returned.
5. **Consider automatic vs. manual**: Some users may prefer auto-adjustment while others need precise manual control.

## Additional Resources

Check out our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for complete code samples and implementation examples.
