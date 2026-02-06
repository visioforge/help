---
title: Delphi MPEG-2 Video Capture with TV Tuner Hardware
description: Implement MPEG-2 capture in Delphi using TV tuner hardware encoders - device enumeration, format configuration, and optimized code examples.
---

# MPEG-2 Video Capture in Delphi Using TV Tuner Hardware Encoders

This comprehensive tutorial demonstrates how to implement high-quality MPEG-2 video capture functionality in your Delphi applications by leveraging TV tuners with built-in hardware encoding capabilities. Hardware encoding significantly reduces CPU usage while maintaining excellent video quality.

## Overview of MPEG-2 Hardware Encoding

MPEG-2 hardware encoders provide superior performance compared to software-based encoding solutions. They're particularly useful for developing professional video capture applications that require efficient processing and high-quality output.

## Enumerating Available MPEG-2 Hardware Encoders

The first step is to identify all available MPEG-2 hardware encoders in the system. This code demonstrates how to populate a dropdown with detected devices:

```pascal
// List all available MPEG-2 hardware encoders in the system
// This helps users select the appropriate encoding device
VideoCapture1.Special_Filters_Fill;
for I := 0 to VideoCapture1.Special_Filters_GetCount(SF_Hardware_Video_Encoder) - 1 do
  cbMPEGEncoder.Items.Add(VideoCapture1.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i));
```

```cpp
// C++ MFC implementation for MPEG-2 encoder enumeration
// Populates a combobox with all detected hardware encoders
m_VideoCapture.Special_Filters_Fill();
for (int i = 0; i < m_VideoCapture.Special_Filters_GetCount(SF_Hardware_Video_Encoder); i++)
{
  CString encoderName = m_VideoCapture.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i);
  m_cbMPEGEncoder.AddString(encoderName);
}
```

```vb
' VB6 implementation for finding hardware MPEG-2 encoders
' Lists all available encoders in a combobox control
VideoCapture1.Special_Filters_Fill
For i = 0 To VideoCapture1.Special_Filters_GetCount(SF_Hardware_Video_Encoder) - 1
  cbMPEGEncoder.AddItem VideoCapture1.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i)
Next i
```

## Selecting a Specific MPEG-2 Encoder

After enumerating the available encoders, the next step is to select a specific encoder for use:

```pascal
// Configure the component to use the selected MPEG-2 hardware encoder
// This must be done before starting the capture process
VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name := cbMPEGEncoder.Items[cbMPEGEncoder.ItemIndex];
```

```cpp
// C++ MFC: Select and configure the chosen MPEG-2 hardware encoder
// Retrieves the selected encoder name from the combobox
int nIndex = m_cbMPEGEncoder.GetCurSel();
CString encoderName;
m_cbMPEGEncoder.GetLBText(nIndex, encoderName);
m_VideoCapture.Video_CaptureDevice_InternalMPEGEncoder_Name = encoderName;
```

```vb
' VB6: Set the selected encoder as the active MPEG-2 hardware encoder
' Must be called before initializing the capture graph
VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name = cbMPEGEncoder.List(cbMPEGEncoder.ListIndex)
```

## Configuring DirectStream MPEG Format for Output

To properly capture MPEG-2 encoded video, you need to set the appropriate output format:

```pascal
// Set the output format to DirectStream MPEG
// This enables proper handling of hardware-encoded MPEG-2 streams
VideoCapture1.OutputFormat := Format_DirectStream_MPEG;
```

```cpp
// C++ MFC: Configure the output format for MPEG-2 encoded content
// DirectStream MPEG format preserves the hardware-encoded stream
m_VideoCapture.OutputFormat = Format_DirectStream_MPEG;
```

```vb
' VB6: Set the proper output format for MPEG-2 hardware encoding
' DirectStream format ensures the encoded data is properly handled
VideoCapture1.OutputFormat = Format_DirectStream_MPEG
```

## Establishing Video Capture Mode

Before starting the capture process, set the component to video capture mode:

```pascal
// Configure the component for video capture operation
// This prepares the internal DirectShow graph for recording
VideoCapture1.Mode := Mode_Video_Capture;
```

```cpp
// C++ MFC: Set the component to video capture mode
// Required before starting the MPEG-2 capture process
m_VideoCapture.Mode = Mode_Video_Capture;
```

```vb
' VB6: Set video capture mode before starting recording
' This initializes the appropriate DirectShow filters
VideoCapture1.Mode = Mode_Video_Capture
```

## Initiating the MPEG-2 Capture Process

Finally, start the capture process to begin recording MPEG-2 video:

```pascal
// Begin the video capture process with the configured settings
// The component will now start recording to the specified output
VideoCapture1.Start;
```

```cpp
// C++ MFC: Start the MPEG-2 video capture process
// Recording begins with the previously configured settings
m_VideoCapture.Start();
```

```vb
' VB6: Start the video capture with the current configuration
' The hardware encoder will now begin processing video data
VideoCapture1.Start
```

## Advanced MPEG-2 Capture Considerations

When implementing MPEG-2 capture with hardware encoders, consider these additional factors:

1. Hardware encoders typically offer better performance than software-based solutions
2. Some TV tuners provide additional encoding parameters that can be customized
3. Buffer sizes may need adjustment for higher quality captures
4. Hardware encoders often handle video scaling and frame rate conversion internally

## Troubleshooting Common Issues

If you encounter problems with MPEG-2 hardware encoding:

1. Verify that your TV tuner device supports hardware MPEG-2 encoding
2. Ensure proper driver installation for the capture device
3. Check that DirectX is properly installed and updated
4. Consider system resource availability, as some encoders require specific resources

Please contact our dedicated support team for assistance with implementing this tutorial in your specific application. Visit our GitHub repository for additional code samples and implementation examples.
