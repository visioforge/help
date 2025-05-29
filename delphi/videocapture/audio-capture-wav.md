---
title: Delphi WAV Audio Capture Implementation Guide
description: Master WAV file audio capture in Delphi applications with this developer tutorial. Learn codec selection, audio parameter configuration, and implementation techniques for creating high-quality audio recording functionality. Includes sample code for mono/stereo recording, compression options, and troubleshooting tips.
sidebar_label: Audio capture to WAV file
---

# Audio Capture to WAV Files: Developer Implementation Guide

## Introduction

Capturing audio to WAV files is a fundamental requirement for many multimedia applications. This guide provides detailed instructions for implementing audio capture functionality with or without compression in your applications. Whether you're developing in Delphi, C++ MFC, or VB6 using our ActiveX controls, this guide will walk you through the entire process from initial setup to final implementation.

## Setting Up Your Development Environment

Before you begin implementing audio capture, ensure you have:

1. Installed the SDK in your development environment
2. Added the VideoCapture component to your form/project
3. Set up basic error handling to manage capture exceptions
4. Configured your application to access audio hardware

## Audio Codec Management

### Retrieving Available Audio Codecs

The first step in implementing audio capture is to retrieve a list of available audio codecs on the system. This allows you to present users with codec options or to programmatically select the most appropriate codec for your application's needs.

#### Delphi Implementation

```pascal
// Iterate through all available audio codecs
for i := 0 to VideoCapture1.Audio_Codecs_GetCount - 1 do
  cbAudioCodec.Items.Add(VideoCapture1.Audio_Codecs_GetItem(i));
```

#### C++ MFC Implementation

```cpp
// Get all available audio codecs and populate combo box
for (int i = 0; i < m_VideoCapture.Audio_Codecs_GetCount(); i++) {
  CString codec = m_VideoCapture.Audio_Codecs_GetItem(i);
  m_AudioCodecCombo.AddString(codec);
}
```

#### VB6 Implementation

```vb
' Iterate through all available audio codecs
For i = 0 To VideoCapture1.Audio_Codecs_GetCount - 1
  cboAudioCodec.AddItem VideoCapture1.Audio_Codecs_GetItem(i)
Next i
```

### Selecting an Audio Codec

Once you've populated the list of available codecs, you'll need to provide a way to select the desired codec for the audio capture operation. This can be done programmatically or via user selection.

#### Delphi Implementation

```pascal
// Set the codec based on user selection from combo box
VideoCapture1.Audio_Codec := cbAudioCodec.Items[cbAudioCodec.ItemIndex];
```

#### C++ MFC Implementation

```cpp
// Get the selected codec from the combo box
int selectedIndex = m_AudioCodecCombo.GetCurSel();
CString selectedCodec;
m_AudioCodecCombo.GetLBText(selectedIndex, selectedCodec);

// Set the codec
m_VideoCapture.SetAudio_Codec(selectedCodec);
```

#### VB6 Implementation

```vb
' Set the codec based on user selection
VideoCapture1.Audio_Codec = cboAudioCodec.Text
```

## Configuring Audio Parameters

Proper audio parameter configuration is crucial for achieving the desired quality and file size balance. The three primary parameters to configure are channels, bits per sample (BPS), and sample rate.

### Setting Audio Channels

Audio channels determine whether the captured audio is mono (1 channel) or stereo (2 channels). Stereo provides better spatial audio representation but requires more storage space.

#### Delphi Implementation

```pascal
// Set the number of audio channels (1 for mono, 2 for stereo)
VideoCapture1.Audio_Channels := StrToInt(cbChannels2.Items[cbChannels2.ItemIndex]);
```

#### C++ MFC Implementation

```cpp
// Set audio channels (1 for mono, 2 for stereo)
int channels = _ttoi(m_ChannelsCombo.GetSelectedItem());
m_VideoCapture.SetAudio_Channels(channels);
```

#### VB6 Implementation

```vb
' Set audio channels (1 for mono, 2 for stereo)
VideoCapture1.Audio_Channels = CInt(cboChannels.Text)
```

### Configuring Bits Per Sample (BPS)

The bits per sample (BPS) setting affects the dynamic range and quality of the audio. Common values include 8, 16, and 24 bits, with higher values providing better quality at the cost of larger file sizes.

#### Delphi Implementation

```pascal
// Set bits per sample (typically 8, 16, or 24)
VideoCapture1.Audio_BPS := StrToInt(cbBPS2.Items[cbBPS2.ItemIndex]);
```

#### C++ MFC Implementation

```cpp
// Set bits per sample
int bps = _ttoi(m_BPSCombo.GetSelectedItem());
m_VideoCapture.SetAudio_BPS(bps);
```

#### VB6 Implementation

```vb
' Set bits per sample
VideoCapture1.Audio_BPS = CInt(cboBPS.Text)
```

### Setting Sample Rate

The sample rate determines how many audio samples are captured per second. Common values include 8000 Hz, 44100 Hz (CD quality), and 48000 Hz (professional audio). Higher sample rates capture more high-frequency detail but increase file size.

#### Delphi Implementation

```pascal
// Set audio sample rate in Hz (common values: 8000, 44100, 48000)
VideoCapture1.Audio_SampleRate := StrToInt(cbSamplerate.Items[cbSamplerate.ItemIndex]);
```

#### C++ MFC Implementation

```cpp
// Set sample rate
int sampleRate = _ttoi(m_SampleRateCombo.GetSelectedItem());
m_VideoCapture.SetAudio_SampleRate(sampleRate);
```

#### VB6 Implementation

```vb
' Set sample rate
VideoCapture1.Audio_SampleRate = CInt(cboSampleRate.Text)
```

## Configuring Output Format

### Selecting PCM/ACM Format

The Windows Audio Compression Manager (ACM) supports various audio formats including PCM (uncompressed) and compressed formats. Setting the output format to PCM/ACM enables codec-based compression when a codec other than PCM is selected.

#### Delphi Implementation

```pascal
// Set output to PCM/ACM format to enable codec-based compression
VideoCapture1.OutputFormat := Format_PCM_ACM;
```

#### C++ MFC Implementation

```cpp
// Set output format to PCM/ACM
m_VideoCapture.SetOutputFormat(Format_PCM_ACM);
```

#### VB6 Implementation

```vb
' Set output format to PCM/ACM
VideoCapture1.OutputFormat = Format_PCM_ACM
```

## Setting the Audio Capture Mode

Before starting the capture operation, you need to set the component to audio capture mode. This ensures that only audio is captured without any video streams.

### Delphi Implementation

```pascal
// Set to audio-only capture mode
VideoCapture1.Mode := Mode_Audio_Capture;
```

### C++ MFC Implementation

```cpp
// Set to audio-only capture mode
m_VideoCapture.SetMode(Mode_Audio_Capture);
```

### VB6 Implementation

```vb
' Set to audio-only capture mode
VideoCapture1.Mode = Mode_Audio_Capture
```

## Starting the Audio Capture

With all parameters configured, you can now start the audio capture process. This initializes the audio hardware, applies the selected codec and settings, and begins capturing audio to the specified output file.

### Delphi Implementation

```pascal
// Begin audio capture process
VideoCapture1.Start;
```

### C++ MFC Implementation

```cpp
// Begin audio capture process
m_VideoCapture.Start();
```

### VB6 Implementation

```vb
' Begin audio capture process
VideoCapture1.Start
```

## Advanced Implementation Considerations

### User Interface Integration

To provide a better user experience, consider implementing:

1. Real-time audio level metering
2. Elapsed time display
3. File size estimation
4. Pause/resume functionality

### Performance Optimization

For optimal performance when capturing extended audio sessions:

1. Monitor system memory usage
2. Implement file splitting for long recordings
3. Consider buffering strategies for high-quality captures

## Troubleshooting Common Issues

When implementing audio capture, you might encounter these common issues:

1. **No audio devices detected**: Ensure proper hardware connections and drivers
2. **Poor audio quality**: Verify sample rate and bits per sample settings
3. **Codec compatibility issues**: Test with standard codecs like PCM or MP3
4. **High CPU usage**: Consider reducing sample rate or using hardware acceleration

## Conclusion

Implementing audio capture to WAV files in your applications requires careful configuration of codecs, audio parameters, and output settings. By following this guide, you can create robust audio capture functionality that balances quality and file size requirements.

For complex implementations or specific technical challenges, our support team is available to assist with custom solutions tailored to your application requirements.

## Additional Resources

Visit our GitHub page for more code samples and implementation examples that demonstrate advanced audio capture techniques and integration patterns.

---

For technical assistance with this implementation, please contact our support team. Additional code samples are available on our GitHub page.
