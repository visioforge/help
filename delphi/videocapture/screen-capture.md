---
title: Delphi Screen Capture and Recording with TVFVideoCapture
description: Implement screen recording in Delphi with TVFVideoCapture - capture regions, full screen, customize frame rates, track cursor with code examples.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Screen Capture
primary_api_classes:
  - TVFVideoCapture

---

# Screen Recording Implementation in Delphi

## Introduction to Screen Capture Functionality

TVFVideoCapture provides powerful screen recording capabilities for Delphi developers. This guide walks through the implementation of screen capture features in your applications, allowing you to record specific regions or the entire screen with customizable settings.

## Configuring Screen Capture Area

You can precisely control which portion of the screen to record by setting coordinate parameters. This is particularly useful when you want to focus on specific application windows or screen regions.

### Setting Specific Screen Coordinates

Use these parameters to define the exact boundaries of your capture area:

```pascal
// Define the top edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Top := StrToInt(edScreenTop.Text);
// Define the bottom edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Bottom := StrToInt(edScreenBottom.Text);
// Define the left edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Left := StrToInt(edScreenLeft.Text);
// Define the right edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Right := StrToInt(edScreenRight.Text);
```

```cpp
// CEdit::GetWindowText(CString&) returns void and fills the buffer by reference,
// so we must declare a CString first and then convert it to int via atoi().
CString sTop, sBottom, sLeft, sRight;
m_edScreenTop.GetWindowText(sTop);
m_edScreenBottom.GetWindowText(sBottom);
m_edScreenLeft.GetWindowText(sLeft);
m_edScreenRight.GetWindowText(sRight);

// Define the top edge position of the capture rectangle (in pixels)
m_VideoCapture.SetScreen_Capture_Top(atoi(sTop));
// Define the bottom edge position of the capture rectangle (in pixels)
m_VideoCapture.SetScreen_Capture_Bottom(atoi(sBottom));
// Define the left edge position of the capture rectangle (in pixels)
m_VideoCapture.SetScreen_Capture_Left(atoi(sLeft));
// Define the right edge position of the capture rectangle (in pixels)
m_VideoCapture.SetScreen_Capture_Right(atoi(sRight));
```

```vb
' Define the top edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Top = CInt(edScreenTop.Text)
' Define the bottom edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Bottom = CInt(edScreenBottom.Text)
' Define the left edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Left = CInt(edScreenLeft.Text)
' Define the right edge position of the capture rectangle (in pixels)
VideoCapture1.Screen_Capture_Right = CInt(edScreenRight.Text)
```

### Capturing the Full Screen

For complete screen recording, simply enable the full screen capture option:

```pascal
// Enable full screen capture mode - will record the entire display
VideoCapture1.Screen_Capture_FullScreen := true;
```

```cpp
// Enable full screen capture mode - will record the entire display
m_VideoCapture.SetScreen_Capture_FullScreen(true);
```

```vb
' Enable full screen capture mode - will record the entire display
VideoCapture1.Screen_Capture_FullScreen = True
```

## Optimizing Frame Rate Settings

The frame rate directly impacts both the quality and file size of your screen recordings. Higher frame rates produce smoother video but generate larger files.

```pascal
// Set capture frame rate to 10 frames per second
// Adjust this value based on your performance requirements
VideoCapture1.Screen_Capture_FrameRate := 10;
```

```cpp
// Set capture frame rate to 10 frames per second
// Adjust this value based on your performance requirements
m_VideoCapture.SetScreen_Capture_FrameRate(10);
```

```vb
' Set capture frame rate to 10 frames per second
' Adjust this value based on your performance requirements
VideoCapture1.Screen_Capture_FrameRate = 10
```

## Cursor Tracking Configuration

For instructional videos or demonstrations, capturing the mouse cursor movement is essential:

```pascal
// Enable mouse cursor capture in the recording
// Set to false to hide cursor in the output video
VideoCapture1.Screen_Capture_Grab_Mouse_Cursor := true;
```

```cpp
// Enable mouse cursor capture in the recording
// Set to false to hide cursor in the output video
m_VideoCapture.SetScreen_Capture_Grab_Mouse_Cursor(true);
```

```vb
' Enable mouse cursor capture in the recording
' Set to false to hide cursor in the output video
VideoCapture1.Screen_Capture_Grab_Mouse_Cursor = True
```

## Activating Screen Capture Mode

After configuring all settings, set the component to screen capture mode to begin recording:

```pascal
// Set component to screen capture operational mode
// This activates all screen recording functionality
VideoCapture1.Mode := Mode_Screen_Capture;
```

```cpp
// Set component to screen capture operational mode
// This activates all screen recording functionality
m_VideoCapture.SetMode(Mode_Screen_Capture);
```

```vb
' Set component to screen capture operational mode
' This activates all screen recording functionality
VideoCapture1.Mode = Mode_Screen_Capture
```

## Advanced Implementation Tips

For optimal screen recording performance:

- Consider system resources when selecting frame rates
- Use region capture when possible to minimize processing load
- Test different quality settings to balance file size and visual quality
- Remember that cursor capture adds slight processing overhead

---
For additional code samples and implementation examples, visit our [GitHub](https://github.com/visioforge/) repository. For technical assistance with implementation, please contact our [support team](https://support.visioforge.com/).