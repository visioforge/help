---
title: Delphi Video Processing - Resize & Crop Tutorial
description: Implement video resizing and cropping in Delphi - real-time processing, aspect ratio handling, and performance optimization with code samples.
---

# Video Resizing and Cropping in Delphi TVFVideoCapture

Video manipulation is a critical component of many modern applications. This guide provides detailed instructions for implementing real-time video resizing and cropping in your Delphi applications with minimal performance impact.

## Why Resize or Crop Video?

Video resizing and cropping serve multiple purposes in development:

- Optimize video for different display sizes
- Reduce bandwidth requirements for streaming
- Focus on specific regions of interest
- Create uniform video dimensions across your application
- Improve performance on resource-constrained devices

## Enabling Resize and Crop Functionality

Before applying any transformations, you must enable the resize/crop functionality in the TVFVideoCapture component.

### Step 1: Enable the Feature

```pascal
// Enable video resizing or cropping functionality
VideoCapture1.Video_ResizeOrCrop_Enabled := true;
```

```cpp
// C++ MFC - Enable video transformation capabilities
m_VideoCapture.SetVideo_ResizeOrCrop_Enabled(TRUE);
```

```vb
' VB6 - Activate resize/crop features
VideoCapture1.Video_ResizeOrCrop_Enabled = True
```

## Video Resizing Implementation

Resizing allows you to change the dimensions of your video stream while maintaining visual quality.

### Setting New Dimensions

```pascal
// Set the desired width and height for the resized video output
VideoCapture1.Video_Resize_NewWidth := StrToInt(edResizeWidth.Text);
VideoCapture1.Video_Resize_NewHeight := StrToInt(edResizeHeight.Text);
```

```cpp
// C++ MFC - Configure target dimensions for video resize
m_VideoCapture.SetVideo_Resize_NewWidth(_ttoi(m_strResizeWidth));
m_VideoCapture.SetVideo_Resize_NewHeight(_ttoi(m_strResizeHeight));
```

```vb
' VB6 - Define new video dimensions
VideoCapture1.Video_Resize_NewWidth = CInt(txtResizeWidth.Text)
VideoCapture1.Video_Resize_NewHeight = CInt(txtResizeHeight.Text)
```

### Handling Aspect Ratio Changes

When resizing video, you can choose between preserving the original aspect ratio (letterbox) or stretching the content to fit the new dimensions.

```pascal
// Letterbox mode adds black borders to preserve aspect ratio
// When false, the video will stretch to fit the new dimensions
VideoCapture1.Video_Resize_LetterBox := cbResizeLetterbox.Checked;
```

```cpp
// C++ MFC - Configure aspect ratio handling method
m_VideoCapture.SetVideo_Resize_LetterBox(m_bResizeLetterbox);
```

```vb
' VB6 - Set letterbox mode for aspect ratio preservation
VideoCapture1.Video_Resize_LetterBox = chkResizeLetterbox.Value
```

### Selecting Resize Algorithms

Choose from multiple resize algorithms based on your quality requirements and performance constraints:

```pascal
// Select the appropriate resize algorithm:
// - NearestNeighbor: Fastest but lowest quality
// - Bilinear: Good balance between speed and quality
// - Bilinear_HQ: Enhanced bilinear with improved quality
// - Bicubic: Better quality with moderate performance impact
// - Bicubic_HQ: Highest quality with highest CPU usage
case cbResizeMode.ItemIndex of
  0: VideoCapture1.Video_Resize_Mode := rm_NearestNeighbor;
  1: VideoCapture1.Video_Resize_Mode := rm_Bilinear;
  2: VideoCapture1.Video_Resize_Mode := rm_Bilinear_HQ;
  3: VideoCapture1.Video_Resize_Mode := rm_Bicubic;
  4: VideoCapture1.Video_Resize_Mode := rm_Bicubic_HQ;
end;
```

```cpp
// C++ MFC - Set the resize algorithm based on quality/performance needs
switch(m_nResizeMode)
{
  case 0: m_VideoCapture.SetVideo_Resize_Mode(rm_NearestNeighbor); break; // Fastest
  case 1: m_VideoCapture.SetVideo_Resize_Mode(rm_Bilinear); break;        // Standard
  case 2: m_VideoCapture.SetVideo_Resize_Mode(rm_Bilinear_HQ); break;     // Enhanced
  case 3: m_VideoCapture.SetVideo_Resize_Mode(rm_Bicubic); break;         // High quality
  case 4: m_VideoCapture.SetVideo_Resize_Mode(rm_Bicubic_HQ); break;      // Maximum quality
}
```

```vb
' VB6 - Choose resize algorithm based on quality and performance needs
Select Case cboResizeMode.ListIndex
  Case 0: VideoCapture1.Video_Resize_Mode = rm_NearestNeighbor  ' Fastest, lower quality
  Case 1: VideoCapture1.Video_Resize_Mode = rm_Bilinear         ' Balanced option
  Case 2: VideoCapture1.Video_Resize_Mode = rm_Bilinear_HQ      ' Enhanced bilinear
  Case 3: VideoCapture1.Video_Resize_Mode = rm_Bicubic          ' Better quality
  Case 4: VideoCapture1.Video_Resize_Mode = rm_Bicubic_HQ       ' Highest quality
End Select
```

## Video Cropping Implementation

Cropping allows you to select a specific region of interest from your video stream.

### Step 1: Enable Cropping

As with resizing, you must first enable the feature:

```pascal
// Enable video transformation capabilities before applying crop
VideoCapture1.Video_ResizeOrCrop_Enabled := true;
```

```cpp
// C++ MFC - Activate video manipulation features
m_VideoCapture.SetVideo_ResizeOrCrop_Enabled(TRUE);
```

```vb
' VB6 - Enable video transformation functionality
VideoCapture1.Video_ResizeOrCrop_Enabled = True
```

### Step 2: Define Crop Region

Specify the boundaries of your crop region by defining the left, top, right, and bottom coordinates:

```pascal
// Define the crop region coordinates in pixels
// These values represent the distance from each edge of the original video
VideoCapture1.Video_Crop_Left := StrToInt(edCropLeft.Text);
VideoCapture1.Video_Crop_Top := StrToInt(edCropTop.Text);
VideoCapture1.Video_Crop_Right := StrToInt(edCropRight.Text);
VideoCapture1.Video_Crop_Bottom := StrToInt(edCropBottom.Text);
```

```cpp
// C++ MFC - Set the crop boundaries in pixels
// Each value defines how many pixels to crop from the respective edge
m_VideoCapture.SetVideo_Crop_Left(_ttoi(m_strCropLeft));
m_VideoCapture.SetVideo_Crop_Top(_ttoi(m_strCropTop));
m_VideoCapture.SetVideo_Crop_Right(_ttoi(m_strCropRight));
m_VideoCapture.SetVideo_Crop_Bottom(_ttoi(m_strCropBottom));
```

```vb
' VB6 - Configure crop region boundaries
' Values represent pixel counts from each edge to exclude
VideoCapture1.Video_Crop_Left = CInt(txtCropLeft.Text)
VideoCapture1.Video_Crop_Top = CInt(txtCropTop.Text)
VideoCapture1.Video_Crop_Right = CInt(txtCropRight.Text)
VideoCapture1.Video_Crop_Bottom = CInt(txtCropBottom.Text)
```

## Best Practices for Video Manipulation

For optimal results when implementing video resizing and cropping:

1. **Test on target hardware** - Different resize algorithms have varying CPU requirements
2. **Consider your use case** - For real-time applications, favor performance over quality
3. **Maintain aspect ratios** - Unless specifically needed, preserve original proportions
4. **Combine operations judiciously** - Applying both resize and crop increases processing overhead
5. **Cache settings** - Avoid changing parameters frequently during capture

## Troubleshooting Common Issues

- If performance is poor, try a faster resize algorithm
- Ensure crop values don't exceed the dimensions of your video stream
- When using letterbox mode, account for the black borders in your UI design
- For best results, resize to dimensions that are multiples of 8 or 16

---
For additional code samples and implementation examples, visit our [GitHub](https://github.com/visioforge/) repository. Need technical assistance? Contact our support team for personalized guidance.