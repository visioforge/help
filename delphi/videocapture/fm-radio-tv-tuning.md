---
title: Delphi FM Radio & TV Tuner Implementation Guide
description: Implement FM radio and TV tuning in Delphi - channel scanning, frequency management, signal detection with code examples for Delphi, C++, VB6.
---

# Implementing FM Radio and TV Tuning in Delphi Applications

## Introduction to TV and Radio Tuning

This guide provides detailed implementation examples for Delphi developers working with FM radio and TV tuning functionality. We've included equivalent code samples for C++ MFC and VB6 to support cross-platform development needs.

## Device Management

### Retrieving Available TV Tuners

The first step in implementing tuner functionality is identifying available hardware devices:

```pascal
// Iterate through all connected TV Tuner devices and populate dropdown
for I := 0 to VideoCapture1.TVTuner_Devices_GetCount - 1 do
  cbTVTuner.Items.Add(VideoCapture1.TVTuner_Devices_GetItem(i));
```

```cpp
// C++ MFC implementation for retrieving TV Tuner devices
for (int i = 0; i < m_VideoCapture.TVTuner_Devices_GetCount(); i++)
  m_cbTVTuner.AddString(m_VideoCapture.TVTuner_Devices_GetItem(i));
```

```vb
' VB6 implementation for device enumeration
For i = 0 To VideoCapture1.TVTuner_Devices_GetCount - 1
  cbTVTuner.AddItem VideoCapture1.TVTuner_Devices_GetItem(i)
Next i
```

### Enumerating TV Format Support

Different regions use different broadcast standards. Your application should detect and handle these formats:

```pascal
// Load available TV formats (PAL, NTSC, SECAM, etc.)
for I := 0 to VideoCapture1.TVTuner_TVFormats_GetCount - 1 do
  cbTVSystem.Items.Add(VideoCapture1.TVTuner_TVFormats_GetItem(i));
```

```cpp
// C++ MFC - Populate TV format dropdown with available standards
for (int i = 0; i < m_VideoCapture.TVTuner_TVFormats_GetCount(); i++)
  m_cbTVSystem.AddString(m_VideoCapture.TVTuner_TVFormats_GetItem(i));
```

```vb
' VB6 - Get supported TV formats for the selected tuner
For i = 0 To VideoCapture1.TVTuner_TVFormats_GetCount - 1
  cbTVSystem.AddItem VideoCapture1.TVTuner_TVFormats_GetItem(i)
Next i
```

### Country-Specific Configuration

Broadcasting standards vary by country, so your application should provide appropriate region selection:

```pascal
// Load country/region list for localized tuning parameters
for I := 0 to VideoCapture1.TVTuner_Countries_GetCount - 1 do
  cbTVCountry.Items.Add(VideoCapture1.TVTuner_Countries_GetItem(i));
```

```cpp
// C++ MFC - Build country selection list for regional settings
for (int i = 0; i < m_VideoCapture.TVTuner_Countries_GetCount(); i++)
  m_cbTVCountry.AddString(m_VideoCapture.TVTuner_Countries_GetItem(i));
```

```vb
' VB6 - Populate country dropdown for regional broadcast settings
For i = 0 To VideoCapture1.TVTuner_Countries_GetCount - 1
  cbTVCountry.AddItem VideoCapture1.TVTuner_Countries_GetItem(i)
Next i
```

## Device Configuration

### Selecting a TV Tuner Device

Once you've enumerated the available devices, users can select their preferred tuner:

```pascal
// Set the active tuner device based on user selection
VideoCapture1.TVTuner_Name := cbTVTuner.Items[cbTVTuner.ItemIndex];
```

```cpp
// C++ MFC - Apply user's tuner device selection
CString strText;
m_cbTVTuner.GetLBText(m_cbTVTuner.GetCurSel(), strText);
m_VideoCapture.put_TVTuner_Name(strText);
```

```vb
' VB6 - Set selected tuner as active device
VideoCapture1.TVTuner_Name = cbTVTuner.Text
```

### Reading Current Tuner Configuration

After selecting a device, you'll need to read its current settings:

```pascal
// Initialize tuner and read current configuration
VideoCapture1.TVTuner_Read;
```

```cpp
// C++ MFC - Load current tuner settings into application
m_VideoCapture.TVTuner_Read();
```

```vb
' VB6 - Read tuner configuration after device selection
VideoCapture1.TVTuner_Read
```

### Available Operation Modes

Tuners support different modes like TV, FM Radio, etc:

```pascal
// Populate operation mode dropdown with available options
for I := 0 to VideoCapture1.TVTuner_Modes_GetCount - 1 do
  cbTVMode.Items.Add(VideoCapture1.TVTuner_Modes_GetItem(i));
```

```cpp
// C++ MFC - Get supported operational modes for this device
for (int i = 0; i < m_VideoCapture.TVTuner_Modes_GetCount(); i++)
  m_cbTVMode.AddString(m_VideoCapture.TVTuner_Modes_GetItem(i));
```

```vb
' VB6 - List available tuner modes (TV, FM Radio, etc)
For i = 0 To VideoCapture1.TVTuner_Modes_GetCount - 1
  cbTVMode.AddItem VideoCapture1.TVTuner_Modes_GetItem(i)
Next i
```

## Frequency Management

### Reading Current Frequencies

Display the current audio and video frequencies to provide user feedback:

```pascal
// Display current video and audio frequencies in Hz
edVideoFreq.Text := IntToStr(VideoCapture1.TVTuner_VideoFrequency);
edAudiofreq.Text := IntToStr(VideoCapture1.TVTuner_AudioFrequency);
```

```cpp
// C++ MFC - Show current frequency values in the interface
CString strFreq;
strFreq.Format(_T("%d"), m_VideoCapture.get_TVTuner_VideoFrequency());
m_edVideoFreq.SetWindowText(strFreq);
strFreq.Format(_T("%d"), m_VideoCapture.get_TVTuner_AudioFrequency());
m_edAudioFreq.SetWindowText(strFreq);
```

```vb
' VB6 - Update frequency display fields with current values
edVideoFreq.Text = CStr(VideoCapture1.TVTuner_VideoFrequency)
edAudioFreq.Text = CStr(VideoCapture1.TVTuner_AudioFrequency)
```

## Input and Mode Configuration

### Setting Signal Input Source

Tuners may support multiple input sources that should be configurable:

```pascal
// Select the appropriate input source based on current configuration
cbTVInput.ItemIndex := cbTVInput.Items.IndexOf(VideoCapture1.TVTuner_InputType);
```

```cpp
// C++ MFC - Update input source selection in UI
CString strInputType = m_VideoCapture.get_TVTuner_InputType();
m_cbTVInput.SetCurSel(m_cbTVInput.FindStringExact(-1, strInputType));
```

```vb
' VB6 - Set input source dropdown to match current configuration
cbTVInput.ListIndex = cbTVInput.FindItem(VideoCapture1.TVTuner_InputType)
```

### Configuring Operation Mode

Different tuner modes require specific UI and parameter adjustments:

```pascal
// Set operation mode dropdown to current mode (TV, FM Radio, etc)
cbTVMode.ItemIndex := cbTVMode.Items.IndexOf(VideoCapture1.TVTuner_Mode);
```

```cpp
// C++ MFC - Update mode selector to match current tuner configuration
CString strMode = m_VideoCapture.get_TVTuner_Mode();
m_cbTVMode.SetCurSel(m_cbTVMode.FindStringExact(-1, strMode));
```

```vb
' VB6 - Select current operating mode in dropdown
cbTVMode.ListIndex = cbTVMode.FindItem(VideoCapture1.TVTuner_Mode)
```

### TV Format Configuration

Set the appropriate broadcast standard for the region:

```pascal
// Configure the appropriate TV standard (PAL, NTSC, SECAM, etc)
cbTVSystem.ItemIndex := cbTVSystem.Items.IndexOf(VideoCapture1.TVTuner_TVFormat);
```

```cpp
// C++ MFC - Set TV format dropdown to current broadcast standard
CString strTVFormat = m_VideoCapture.get_TVTuner_TVFormat();
m_cbTVSystem.SetCurSel(m_cbTVSystem.FindStringExact(-1, strTVFormat));
```

```vb
' VB6 - Update TV system format selection
cbTVSystem.ListIndex = cbTVSystem.FindItem(VideoCapture1.TVTuner_TVFormat)
```

### Regional Settings

Configure region-specific broadcast parameters:

```pascal
// Set country/region for appropriate frequency tables and standards
cbTVCountry.ItemIndex := cbTVCountry.Items.IndexOf(VideoCapture1.TVTuner_Country);
```

```cpp
// C++ MFC - Update country selection to match current setting
CString strCountry = m_VideoCapture.get_TVTuner_Country();
m_cbTVCountry.SetCurSel(m_cbTVCountry.FindStringExact(-1, strCountry));
```

```vb
' VB6 - Set country dropdown to current regional setting
cbTVCountry.ListIndex = cbTVCountry.FindItem(VideoCapture1.TVTuner_Country)
```

## Channel Scanning

### Handling Channel Scan Events

Implement the event handler for channel scanning process:

```pascal
// Event handler for channel scanning process
// Tracks progress and collects found channels
procedure TForm1.VideoCapture1TVTunerTuneChannels(SignalPresent: Boolean; Channel, Frequency, Progress: Integer);
begin
  // Update progress bar with current scan progress
  pbChannels.Position := Progress;

  // Add channel to list if signal is detected
  if SignalPresent then
    cbTVChannel.Items.Add(IntToStr(Channel));

  // Scan complete when Channel = -1
  if Channel = -1 then
    begin
      pbChannels.Position := 0;
      ShowMessage('AutoTune complete');
    end;
end;
```

```cpp
// C++ MFC - Channel scan event handler implementation
// In header file (.h)
BEGIN_EVENTSINK_MAP(CMainDlg, CDialog)
    ON_EVENT(CMainDlg, IDC_VIDEOCAPTURE, 1, OnTVTunerTuneChannels, VTS_BOOL VTS_I4 VTS_I4 VTS_I4)
END_EVENTSINK_MAP()

// In implementation file (.cpp)
void CMainDlg::OnTVTunerTuneChannels(BOOL SignalPresent, long Channel, long Frequency, long Progress)
{
    // Update scan progress indicator
    m_pbChannels.SetPos(Progress);
    
    // Add found channels to the selection list
    if (SignalPresent)
    {
        CString strChannel;
        strChannel.Format(_T("%d"), Channel);
        m_cbTVChannel.AddString(strChannel);
    }
    
    // Handle scan completion
    if (Channel == -1)
    {
        m_pbChannels.SetPos(0);
        MessageBox(_T("AutoTune complete"), _T("Information"), MB_OK | MB_ICONINFORMATION);
    }
}
```

```vb
' VB6 - Channel scan event handler
Private Sub VideoCapture1_TVTunerTuneChannels(ByVal SignalPresent As Boolean, ByVal Channel As Long, ByVal Frequency As Long, ByVal Progress As Long)
    ' Update scan progress display
    pbChannels.Value = Progress
    
    ' Add channel to list when signal is found
    If SignalPresent Then
        cbTVChannel.AddItem CStr(Channel)
    End If
    
    ' Handle scan completion
    If Channel = -1 Then
        pbChannels.Value = 0
        MsgBox "AutoTune complete", vbInformation
    End If
End Sub
```

### Initiating Channel Scan

Start the automatic channel scanning process:

```pascal
// Define frequency constants for clarity
const KHz = 1000;
const MHz = 1000000;

// Initialize tuner with current settings
VideoCapture1.TVTuner_Read;
// Clear previous channel list
cbTVChannel.Items.Clear;

// Configure special parameters for FM Radio scanning
if ( (cbTVMode.ItemIndex <> -1) and (cbTVMode.Items[cbTVMode.ItemIndex] = 'FM Radio') ) then
  begin
    // Set frequency range for FM scanning (100-110MHz)
    VideoCapture1.TVTuner_FM_Tuning_StartFrequency := 100 * Mhz;
    VideoCapture1.TVTuner_FM_Tuning_StopFrequency := 110 * MHz;
    // Set 100KHz increments for FM scanning
    VideoCapture1.TVTuner_FM_Tuning_Step := 100 * KHz;
  end;

// Begin automatic channel scan
VideoCapture1.TVTuner_TuneChannels_Start;
```

```cpp
// C++ MFC - Initiate channel scan with appropriate parameters
const int KHz = 1000;
const int MHz = 1000000;

// Update tuner configuration
m_VideoCapture.TVTuner_Read();
// Reset channel list before scanning
m_cbTVChannel.ResetContent();

// Configure FM-specific parameters if in radio mode
CString strMode;
m_cbTVMode.GetLBText(m_cbTVMode.GetCurSel(), strMode);
if (strMode == _T("FM Radio"))
{
    // Set FM scan range (100-110MHz)
    m_VideoCapture.put_TVTuner_FM_Tuning_StartFrequency(100 * MHz);
    m_VideoCapture.put_TVTuner_FM_Tuning_StopFrequency(110 * MHz);
    // Use 100KHz steps for FM scanning
    m_VideoCapture.put_TVTuner_FM_Tuning_Step(100 * KHz);
}

// Start the channel scanning process
m_VideoCapture.TVTuner_TuneChannels_Start();
```

```vb
' VB6 - Begin channel scanning process
Const KHz = 1000
Const MHz = 1000000

' Read current tuner configuration
VideoCapture1.TVTuner_Read
' Clear existing channel list
cbTVChannel.Clear

' Special configuration for FM Radio scanning
If (cbTVMode.ListIndex <> -1) And (cbTVMode.Text = "FM Radio") Then
    ' Set FM band scan parameters (100-110MHz)
    VideoCapture1.TVTuner_FM_Tuning_StartFrequency = 100 * MHz
    VideoCapture1.TVTuner_FM_Tuning_StopFrequency = 110 * MHz
    ' Use 100KHz step size for FM scanning
    VideoCapture1.TVTuner_FM_Tuning_Step = 100 * KHz
End If

' Initiate automatic channel scan
VideoCapture1.TVTuner_TuneChannels_Start
```

## Manual Tuning Operations

### Setting Channel by Number

Allow direct channel selection by number:

```pascal
// Change to specified channel number
VideoCapture1.TVTuner_Channel := StrToInt(edChannel.Text);
// Apply tuning changes
VideoCapture1.TVTuner_Apply;
```

```cpp
// C++ MFC - Set tuner to specified channel number
CString strChannel;
m_edChannel.GetWindowText(strChannel);
m_VideoCapture.put_TVTuner_Channel(_ttoi(strChannel));
m_VideoCapture.TVTuner_Apply();
```

```vb
' VB6 - Tune to specific channel number
VideoCapture1.TVTuner_Channel = CInt(edChannel.Text)
VideoCapture1.TVTuner_Apply
```

### Setting Radio Frequency Directly

For FM radio, direct frequency tuning is often required:

```pascal
// Set channel to -1 for frequency-based tuning
VideoCapture1.TVTuner_Channel := -1; // must be -1 to use frequency
// Set specific frequency from input field
VideoCapture1.TVTuner_Frequency := StrToInt(edChannel.Text);
// Apply frequency change
VideoCapture1.TVTuner_Apply;
```

```cpp
// C++ MFC - Direct frequency tuning implementation
CString strFrequency;
m_edChannel.GetWindowText(strFrequency);
// Set channel to -1 to enable frequency-based tuning
m_VideoCapture.put_TVTuner_Channel(-1); // must be -1 to use frequency
// Apply the specified frequency
m_VideoCapture.put_TVTuner_Frequency(_ttoi(strFrequency));
m_VideoCapture.TVTuner_Apply();
```

```vb
' VB6 - Manual frequency tuning for radio
VideoCapture1.TVTuner_Channel = -1 ' must be -1 to use frequency
VideoCapture1.TVTuner_Frequency = CInt(edChannel.Text)
VideoCapture1.TVTuner_Apply
```

## Conclusion

This guide covers the essential aspects of implementing FM radio and TV tuning functionality in your Delphi applications. By following these examples, you can create robust tuning interfaces with proper channel scanning, frequency management, and signal detection.

For optimal integration into your projects, remember to handle error conditions and provide appropriate user feedback during lengthy operations such as channel scanning.

---
Please visit our [GitHub](https://github.com/visioforge/) page for additional code samples and implementation examples.