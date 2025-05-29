---
title: Audio Capture to MP3 Files in Delphi, C++ MFC & VB6
description: Complete step-by-step guide for developers implementing MP3 audio capture functionality in Delphi, C++ MFC, and VB6 applications. Learn how to configure LAME encoder settings, manage channels, handle bitrates and create high-quality audio recordings.
sidebar_label: Audio capture to MP3 file
---

# Audio Capture to MP3 Files in Delphi, C++ MFC & VB6

## Introduction

Audio capture capabilities are essential for many modern applications, from voice recording tools to multimedia creation software. This guide walks through the implementation of MP3 audio capture functionality in Delphi, C++ MFC, and VB6 applications using the VideoCapture component.

MP3 remains one of the most widely used audio formats due to its excellent compression and broad compatibility. By implementing proper MP3 audio capture in your applications, you can provide users with efficient, high-quality audio recording capabilities.

## Prerequisites

Before implementing MP3 audio capture, ensure you have:

- Development environment with Delphi, Visual C++ (for MFC), or Visual Basic 6
- VideoCapture component properly installed and referenced in your project
- Basic understanding of audio encoding concepts
- Required permissions for audio device access in your application

## LAME Encoder Configuration

The LAME MP3 encoder provides extensive customization options for audio quality, bitrate management, and channel configuration. Properly configuring these settings is crucial for achieving the desired audio quality while managing file size.

### Configuring Basic Encoding Parameters

The following code snippets demonstrate how to configure basic LAME encoding parameters:

```pascal
// Delphi
VideoCapture1.Audio_LAME_CBR_Bitrate := StrToInt(cbLameCBRBitrate.Items[cbLameCBRBitrate.ItemIndex]);
VideoCapture1.Audio_LAME_VBR_Min_Bitrate := StrToInt(cbLameVBRMin.Items[cbLameVBRMin.ItemIndex]);
VideoCapture1.Audio_LAME_VBR_Max_Bitrate := StrToInt(cbLameVBRMax.Items[cbLameVBRMax.ItemIndex]);
VideoCapture1.Audio_LAME_Sample_Rate := StrToInt(cbLameSampleRate.Items[cbLameSampleRate.ItemIndex]);
VideoCapture1.Audio_LAME_VBR_Quality := tbLameVBRQuality.Position;
VideoCapture1.Audio_LAME_Encoding_Quality := tbLameEncodingQuality.Position;
```

```cpp
// C++ MFC
m_VideoCapture.Audio_LAME_CBR_Bitrate = _ttoi(m_cbLameCBRBitrate.GetItemData(m_cbLameCBRBitrate.GetCurSel()));
m_VideoCapture.Audio_LAME_VBR_Min_Bitrate = _ttoi(m_cbLameVBRMin.GetItemData(m_cbLameVBRMin.GetCurSel()));
m_VideoCapture.Audio_LAME_VBR_Max_Bitrate = _ttoi(m_cbLameVBRMax.GetItemData(m_cbLameVBRMax.GetCurSel()));
m_VideoCapture.Audio_LAME_Sample_Rate = _ttoi(m_cbLameSampleRate.GetItemData(m_cbLameSampleRate.GetCurSel()));
m_VideoCapture.Audio_LAME_VBR_Quality = m_tbLameVBRQuality.GetPos();
m_VideoCapture.Audio_LAME_Encoding_Quality = m_tbLameEncodingQuality.GetPos();
```

```vb
' VB6
VideoCapture1.Audio_LAME_CBR_Bitrate = CInt(cbLameCBRBitrate.List(cbLameCBRBitrate.ListIndex))
VideoCapture1.Audio_LAME_VBR_Min_Bitrate = CInt(cbLameVBRMin.List(cbLameVBRMin.ListIndex))
VideoCapture1.Audio_LAME_VBR_Max_Bitrate = CInt(cbLameVBRMax.List(cbLameVBRMax.ListIndex))
VideoCapture1.Audio_LAME_Sample_Rate = CInt(cbLameSampleRate.List(cbLameSampleRate.ListIndex))
VideoCapture1.Audio_LAME_VBR_Quality = tbLameVBRQuality.Value
VideoCapture1.Audio_LAME_Encoding_Quality = tbLameEncodingQuality.Value
```

### Setting Audio Channel Modes

Channel configuration affects both sound quality and file size. The following code demonstrates how to set the channel mode:

```pascal
// Delphi
if rbLameStandardStereo.Checked then
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Standard_Stereo
else if rbLameJointStereo.Checked then
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Joint_Stereo
else if rbLameDualChannels.Checked then
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Dual_Stereo
else
  VideoCapture1.Audio_LAME_Channels_Mode := CH_Mono;
```

```cpp
// C++ MFC
if (m_rbLameStandardStereo.GetCheck())
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Standard_Stereo;
else if (m_rbLameJointStereo.GetCheck())
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Joint_Stereo;
else if (m_rbLameDualChannels.GetCheck())
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Dual_Stereo;
else
  m_VideoCapture.Audio_LAME_Channels_Mode = VisioForge_Video_Capture::CH_Mono;
```

```vb
' VB6
If rbLameStandardStereo.Value Then
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Standard_Stereo
ElseIf rbLameJointStereo.Value Then
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Joint_Stereo
ElseIf rbLameDualChannels.Value Then
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Dual_Stereo
Else
  VideoCapture1.Audio_LAME_Channels_Mode = CH_Mono
End If
```

### Advanced LAME Configuration Options

For more precise control over the encoding process, configure these advanced LAME options:

```pascal
// Delphi
VideoCapture1.Audio_LAME_VBR_Mode := rbLameVBR.Checked;
VideoCapture1.Audio_LAME_Copyright := cbLameCopyright.Checked;
VideoCapture1.Audio_LAME_Original := cbLameOriginalCopy.Checked;
VideoCapture1.Audio_LAME_CRC_Protected := cbLameCRCProtected.Checked;
VideoCapture1.Audio_LAME_Force_Mono := cbLameForceMono.Checked;
VideoCapture1.Audio_LAME_Strictly_Enforce_VBR_Min_Bitrate := cbLameStrictlyEnforceVBRMinBitrate.Checked;
VideoCapture1.Audio_LAME_Voice_Encoding_Mode := cbLameVoiceEncodingMode.Checked;
VideoCapture1.Audio_LAME_Keep_All_Frequencies := cbLameKeepAllFrequencies.Checked;
VideoCapture1.Audio_LAME_Strict_ISO_Compilance := cbLameStrictISOCompilance.Checked;
VideoCapture1.Audio_LAME_Disable_Short_Blocks := cbLameDisableShortBlocks.Checked;
VideoCapture1.Audio_LAME_Enable_Xing_VBR_Tag := cbLameEnableXingVBRTag.Checked;
VideoCapture1.Audio_LAME_Mode_Fixed := cbLameModeFixed.Checked;
```

```cpp
// C++ MFC
m_VideoCapture.Audio_LAME_VBR_Mode = m_rbLameVBR.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Copyright = m_cbLameCopyright.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Original = m_cbLameOriginalCopy.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_CRC_Protected = m_cbLameCRCProtected.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Force_Mono = m_cbLameForceMono.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Strictly_Enforce_VBR_Min_Bitrate = m_cbLameStrictlyEnforceVBRMinBitrate.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Voice_Encoding_Mode = m_cbLameVoiceEncodingMode.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Keep_All_Frequencies = m_cbLameKeepAllFrequencies.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Strict_ISO_Compilance = m_cbLameStrictISOCompilance.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Disable_Short_Blocks = m_cbLameDisableShortBlocks.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Enable_Xing_VBR_Tag = m_cbLameEnableXingVBRTag.GetCheck() ? true : false;
m_VideoCapture.Audio_LAME_Mode_Fixed = m_cbLameModeFixed.GetCheck() ? true : false;
```

```vb
' VB6
VideoCapture1.Audio_LAME_VBR_Mode = rbLameVBR.Value
VideoCapture1.Audio_LAME_Copyright = cbLameCopyright.Value
VideoCapture1.Audio_LAME_Original = cbLameOriginalCopy.Value
VideoCapture1.Audio_LAME_CRC_Protected = cbLameCRCProtected.Value
VideoCapture1.Audio_LAME_Force_Mono = cbLameForceMono.Value
VideoCapture1.Audio_LAME_Strictly_Enforce_VBR_Min_Bitrate = cbLameStrictlyEnforceVBRMinBitrate.Value
VideoCapture1.Audio_LAME_Voice_Encoding_Mode = cbLameVoiceEncodingMode.Value
VideoCapture1.Audio_LAME_Keep_All_Frequencies = cbLameKeepAllFrequencies.Value
VideoCapture1.Audio_LAME_Strict_ISO_Compilance = cbLameStrictISOCompilance.Value
VideoCapture1.Audio_LAME_Disable_Short_Blocks = cbLameDisableShortBlocks.Value
VideoCapture1.Audio_LAME_Enable_Xing_VBR_Tag = cbLameEnableXingVBRTag.Value
VideoCapture1.Audio_LAME_Mode_Fixed = cbLameModeFixed.Value
```

## Understanding LAME Configuration Options

### Bitrate Settings

- **CBR (Constant Bitrate)**: Maintains the same bitrate throughout the entire recording
- **VBR (Variable Bitrate)**: Adjusts bitrate based on audio complexity
- **Min/Max Bitrate**: Sets boundaries for VBR encoding
- **VBR Quality**: Controls the quality/file size balance in VBR mode

### Channel Modes

- **Standard Stereo**: Completely separate left and right channels
- **Joint Stereo**: Combines redundant information between channels to save space
- **Dual Stereo**: Two completely independent mono channels
- **Mono**: Single audio channel

### Special Encoding Options

- **Voice Encoding Mode**: Optimizes encoding for voice frequencies
- **Force Mono**: Converts stereo input to mono output
- **CRC Protection**: Adds error detection data
- **Strict ISO Compliance**: Ensures maximum compatibility with all MP3 players

## Configuring Output Format

After setting up LAME encoding parameters, specify MP3 as the output format:

```pascal
// Delphi
VideoCapture1.OutputFormat := Format_LAME;
```

```cpp
// C++ MFC
m_VideoCapture.OutputFormat = VisioForge_Video_Capture::Format_LAME;
```

```vb
' VB6
VideoCapture1.OutputFormat = Format_LAME
```

## Setting Audio Capture Mode

Set the VideoCapture component to audio-only capture mode:

```pascal
// Delphi
VideoCapture1.Mode := Mode_Audio_Capture;
```

```cpp
// C++ MFC
m_VideoCapture.Mode = VisioForge_Video_Capture::Mode_Audio_Capture;
```

```vb
' VB6
VideoCapture1.Mode = Mode_Audio_Capture
```

## Starting the Audio Capture

Once all parameters are configured, initiate the recording process:

```pascal
// Delphi
VideoCapture1.Start;
```

```cpp
// C++ MFC
m_VideoCapture.Start();
```

```vb
' VB6
VideoCapture1.Start
```

## Best Practices for MP3 Audio Capture

- **Quality vs. Size**: For voice recordings, lower bitrates (64-128 kbps) are usually sufficient. For music, use 192 kbps or higher.
- **Sample Rate Selection**: 44.1 kHz is standard for most audio. Lower rates can be used for voice-only recordings.
- **VBR vs. CBR**: VBR generally provides better quality-to-size ratio but might have compatibility issues with some players.
- **Error Handling**: Always implement proper error handling around the recording process.
- **User Feedback**: Provide visual feedback during recording (level meters, time elapsed).

## Conclusion

Implementing MP3 audio capture in your applications provides users with a widely compatible and efficient recording solution. By properly configuring LAME encoder settings, you can balance audio quality and file size based on your application's specific requirements.

The VideoCapture component makes this implementation straightforward in Delphi, C++ MFC, and VB6 applications, allowing you to focus on creating a great user experience around the audio capture functionality.

---

For additional code samples and advanced implementation techniques, visit our GitHub repository. If you encounter any issues during implementation, contact our technical support team for assistance.
