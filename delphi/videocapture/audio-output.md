---
title: Delphi Audio Output Device Selection | VideoCapture
description: Select audio output devices in Delphi - enumerate devices, control volume, adjust balance with code examples for Delphi, C++, and VB6.
---

# Audio Output Device Selection in Delphi

This guide provides detailed instructions and code examples for implementing audio output device selection in your video capture applications. Delphi, C++ MFC, and VB6 implementations are covered to help you integrate this functionality into your projects efficiently.

## Available Audio Output Device Enumeration

The first step in implementing audio output device selection is retrieving the complete list of available audio output devices on the system. This allows users to choose their preferred audio output device.

### Delphi Implementation

```pascal
// Iterate through all available audio output devices
for i := 0 to VideoCapture1.Audio_OutputDevices_GetCount - 1 do
  // Add each device to the dropdown list
  cbAudioOutputDevice.Items.Add(VideoCapture1.Audio_OutputDevices_GetItem(i));
```

### C++ MFC Implementation

```cpp
// Populate the combobox with all available audio output devices
for (int i = 0; i < m_VideoCapture.Audio_OutputDevices_GetCount(); i++) {
  CString deviceName = m_VideoCapture.Audio_OutputDevices_GetItem(i);
  m_AudioOutputDeviceCombo.AddString(deviceName);
}
```

### VB6 Implementation

```vb
' Iterate through all available audio output devices
For i = 0 To VideoCapture1.Audio_OutputDevices_GetCount - 1
  ' Add each device to the dropdown list
  cboAudioOutputDevice.AddItem VideoCapture1.Audio_OutputDevices_GetItem(i)
Next i
```

## Setting the Active Audio Output Device

After retrieving the available devices, the next step is to set the selected device as the active audio output device for your application.

### Delphi Implementation

```pascal
// Set the selected device as the active audio output device
VideoCapture1.Audio_OutputDevice := cbAudioOutputDevice.Items[cbAudioOutputDevice.ItemIndex];
```

### C++ MFC Implementation

```cpp
// Get the selected index from the combobox
int selectedIndex = m_AudioOutputDeviceCombo.GetCurSel();
CString selectedDevice;
m_AudioOutputDeviceCombo.GetLBText(selectedIndex, selectedDevice);

// Set the selected device as the active audio output device
m_VideoCapture.Audio_OutputDevice = selectedDevice;
```

### VB6 Implementation

```vb
' Set the selected device as the active audio output device
VideoCapture1.Audio_OutputDevice = cboAudioOutputDevice.Text
```

## Enabling Audio Playback

Once the output device is selected, you need to enable audio playback to hear the audio through the selected device.

### Delphi Implementation

```pascal
// Enable audio playback through the selected device
VideoCapture1.Audio_PlayAudio := true;
```

### C++ MFC Implementation

```cpp
// Enable audio playback through the selected device
m_VideoCapture.Audio_PlayAudio = TRUE;
```

### VB6 Implementation

```vb
' Enable audio playback through the selected device
VideoCapture1.Audio_PlayAudio = True
```

## Adjusting Audio Volume Levels

Providing volume control gives users the ability to customize their audio experience. This section shows how to implement volume adjustment.

### Delphi Implementation

```pascal
// Set the volume level based on trackbar position
VideoCapture1.Audio_OutputDevice_SetVolume(tbAudioVolume.Position);
```

### C++ MFC Implementation

```cpp
// Get the current position of the volume slider
int volumeLevel = m_VolumeSlider.GetPos();

// Set the volume level based on slider position
m_VideoCapture.Audio_OutputDevice_SetVolume(volumeLevel);
```

### VB6 Implementation

```vb
' Set the volume level based on slider position
VideoCapture1.Audio_OutputDevice_SetVolume sldVolume.Value
```

## Controlling Audio Balance

For stereo output, balance control allows users to adjust the relative volume between left and right channels.

### Delphi Implementation  

```pascal
// Set the balance level based on trackbar position
VideoCapture1.Audio_OutputDevice_SetBalance(tbAudioBalance.Position);
```
  
### C++ MFC Implementation

```cpp
// Get the current position of the balance slider
int balanceLevel = m_BalanceSlider.GetPos();

// Set the balance level based on slider position
m_VideoCapture.Audio_OutputDevice_SetBalance(balanceLevel);
```

### VB6 Implementation

```vb
' Set the balance level based on slider position
VideoCapture1.Audio_OutputDevice_SetBalance sldBalance.Value
```

## Best Practices for Audio Device Implementation

- Always check if the audio device is valid before attempting to use it
- Provide fallback mechanisms when the selected device becomes unavailable
- Consider saving user preferences for audio device selection between sessions
- Implement visual feedback when volume or balance settings are changed

---
Please contact our [support team](https://support.visioforge.com/) if you need assistance with this implementation. Visit our [GitHub repository](https://github.com/visioforge/) for additional code samples and resources.