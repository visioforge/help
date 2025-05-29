---
title: Video Renderer Options for Delphi Video Capture
description: Implement optimal video renderers in your Delphi applications with this developer guide. Learn how to use Video Renderer, VMR9, and EVR with detailed code examples for better performance, hardware acceleration, and compatibility across different Windows environments.
sidebar_label: Select video renderer
---

# Video Renderer Selection Guide for TVFVideoCapture

## Overview of Available Renderers

When developing video capture applications with TVFVideoCapture, selecting the appropriate video renderer significantly impacts performance and compatibility. This guide provides detailed implementation examples for the three available renderer options in Delphi, C++, and VB6 environments.

## Standard Video Renderer

The standard Video Renderer utilizes GDI for drawing operations. This renderer option is primarily recommended for:

- Legacy systems
- Environments where Direct3D acceleration is unavailable
- Maximum compatibility with older hardware

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_VideoRenderer;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_VideoRenderer);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_VideoRenderer
```

## Video Mixing Renderer 9 (VMR9)

VMR9 represents a modern filtering solution capable of leveraging GPU capabilities for enhanced rendering. Key advantages include:

- Hardware-accelerated video processing
- Advanced deinterlacing options
- Improved performance for high-resolution content

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_VMR9;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_VMR9);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_VMR9
```

### Accessing Deinterlacing Modes

VMR9 supports multiple deinterlacing techniques. The following code demonstrates how to retrieve available deinterlacing options:

```pascal
// Delphi
VideoCapture1.Video_Renderer_Deinterlace_Modes_Fill;
for I := 0 to VideoCapture1.Video_Renderer_Deinterlace_Modes_GetCount - 1 do
  cbDeinterlaceModes.Items.Add(VideoCapture1.Video_Renderer_Deinterlace_Modes_GetItem(i));
```

```cpp
// C++ MFC
m_VideoCapture.Video_Renderer_Deinterlace_Modes_Fill();
for (int i = 0; i < m_VideoCapture.GetVideo_Renderer_Deinterlace_Modes_GetCount(); i++) {
    m_DeinterlaceCombo.AddString(m_VideoCapture.GetVideo_Renderer_Deinterlace_Modes_GetItem(i));
}
```

```vb
' VB6
VideoCapture1.Video_Renderer_Deinterlace_Modes_Fill
For i = 0 To VideoCapture1.Video_Renderer_Deinterlace_Modes_GetCount - 1
    cboDeinterlaceModes.AddItem VideoCapture1.Video_Renderer_Deinterlace_Modes_GetItem(i)
Next i
```

## Enhanced Video Renderer (EVR)

EVR is the recommended renderer for modern Windows environments (Vista and later). This advanced renderer provides:

- Superior video acceleration capabilities
- Optimal performance on Windows 7/10/11
- Better resource utilization

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_EVR;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_EVR);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_EVR
```

## Managing Aspect Ratio and Display Options

When displaying video content, you'll often need to handle aspect ratio differences between the source video and the display area.

### Stretching the Video Image

To stretch the video to fill the entire display area:

```pascal
// Delphi
VideoCapture1.Screen_Stretch := true;
VideoCapture1.Screen_Update;
```

```cpp
// C++ MFC
m_VideoCapture.SetScreen_Stretch(true);
m_VideoCapture.Screen_Update();
```

```vb
' VB6
VideoCapture1.Screen_Stretch = True
VideoCapture1.Screen_Update
```

### Using Letterbox Mode (Black Borders)

For preserving the original aspect ratio with black borders:

```pascal
// Delphi
VideoCapture1.Screen_Stretch := false;
VideoCapture1.Screen_Update;
```

```cpp
// C++ MFC
m_VideoCapture.SetScreen_Stretch(false);
m_VideoCapture.Screen_Update();
```

```vb
' VB6
VideoCapture1.Screen_Stretch = False
VideoCapture1.Screen_Update
```

## Performance Considerations

When selecting a renderer for your application, consider these factors:

1. Target operating system version
2. Hardware capabilities of end-user systems
3. Video resolution and processing requirements
4. Compatibility needs for your deployment environment

---

Please get in touch with [support](https://support.visioforge.com/) if you need technical assistance with this implementation. Visit our [GitHub](https://github.com/visioforge/) repository for additional code samples and resources.
