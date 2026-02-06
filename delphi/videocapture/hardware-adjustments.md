---
title: Camera Video Adjustments for Delphi Applications
description: Adjust camera brightness, contrast, and saturation in Delphi with TVFVideoCapture hardware controls and parameter configuration examples.
---

# Implementing Hardware Video Adjustments in Delphi Applications

## Overview

Modern video capture devices offer powerful hardware-level adjustments that can significantly enhance the quality of your video applications. By leveraging these capabilities in your Delphi applications, you can provide users with professional-grade video control features without complex software-based image processing.

## Supported Adjustment Types

Most webcams and video capture devices support several adjustment parameters:

- Brightness
- Contrast
- Saturation
- Hue
- Sharpness
- Gamma
- White balance
- Gain

## Retrieving Available Adjustment Ranges

Before setting adjustments, you'll need to determine what ranges are supported by the connected device. The `Video_CaptureDevice_VideoAdjust_GetRanges` method provides this information.

### Delphi Implementation

```pascal
// Retrieve the available range for brightness adjustment
// Returns minimum, maximum, step size, default value, and auto-adjustment capability
VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges(adj_Brightness, min, max, step, default, auto);
```

### C++ MFC Implementation 

```cpp
// C++ MFC implementation for getting brightness adjustment ranges
// Store results in integer variables for UI configuration
int min, max, step, default_value;
BOOL auto_value;
m_VideoCapture.Video_CaptureDevice_VideoAdjust_GetRanges(
  VF_VIDEOCAP_ADJ_BRIGHTNESS,
  &min,
  &max,
  &step,
  &default_value,
  &auto_value);
```

### VB6 Implementation

```vb
' VB6 implementation for retrieving brightness adjustment parameters
' Use these values to configure slider controls and checkboxes
Dim min As Integer, max As Integer, step As Integer, default_val As Integer
Dim auto_val As Boolean
VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges adj_Brightness, min, max, step, default_val, auto_val
```

## Setting Adjustment Values

Once you've determined the available ranges, you can use the `Video_CaptureDevice_VideoAdjust_SetValue` method to apply specific settings to the video stream.

### Delphi Implementation

```pascal
// Set the brightness level based on trackbar position
// The third parameter enables/disables automatic brightness adjustment
VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue(
  adj_Brightness, 
  tbAdjBrightness.Position,
  cbAdjBrightnessAuto.Checked);
```

### C++ MFC Implementation

```cpp
// C++ MFC implementation for setting brightness value
// Uses slider position for manual adjustment value
// Checkbox state determines if auto-adjustment is enabled
m_VideoCapture.Video_CaptureDevice_VideoAdjust_SetValue(
  VF_VIDEOCAP_ADJ_BRIGHTNESS,
  m_sliderBrightness.GetPos(),
  m_checkBrightnessAuto.GetCheck() == BST_CHECKED);
```

### VB6 Implementation

```vb
' VB6 implementation for applying brightness settings
' Uses trackbar value for adjustment level
' Checkbox value determines automatic adjustment mode
VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue _
  adj_Brightness, _
  tbAdjBrightness.Value, _
  cbAdjBrightnessAuto.Value = vbChecked
```

## Best Practices for Video Adjustment Implementation

When implementing video adjustments in your applications:

1. Always check device capabilities first, as not all devices support all adjustment types
2. Provide intuitive UI controls like sliders with appropriate min/max values
3. Include auto-adjustment options when available
4. Consider saving user preferences for future sessions
5. Implement real-time preview so users can see the effects of their adjustments

## Additional Resources

Please contact our support team for assistance with implementing these features in your application. Visit our GitHub repository for additional code samples and implementation examples.
