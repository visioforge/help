---
title: Video Capture to AVI Files in Delphi Applications
description: Learn how to implement video capture functionality to AVI files in your Delphi applications using TVFVideoCapture component. This guide covers codec selection, audio configuration, and complete implementation steps with code examples.
sidebar_label: Video capture to AVI file
---

# Complete Guide to Video Capture to AVI Files in Delphi

When developing multimedia applications in Delphi, video capture functionality is often a critical requirement. This guide explores how to implement high-quality video capture to AVI files using the TVFVideoCapture component in Delphi applications. We'll cover everything from setting up codecs to configuring audio parameters and starting the capture process.

## Understanding AVI Video Capture in Delphi

The TVFVideoCapture component provides a powerful and flexible way to capture video directly to AVI format in Delphi applications. AVI (Audio Video Interleave) remains a popular video container format due to its broad compatibility and reliability for recording purposes.

When implementing video capture in your Delphi application, you'll need to consider several key aspects:

1. Selecting appropriate video and audio codecs
2. Configuring audio parameters
3. Setting the output format and capture mode
4. Managing the capture process

This guide provides detailed explanations and code samples for each of these steps.

## Working with Video and Audio Codecs

### Retrieving Available Codecs

Before capturing video, you'll need to populate your application with the available video and audio codecs. The TVFVideoCapture component makes this straightforward:

```pascal
procedure TMyForm.PopulateCodecLists;
var
  I: Integer;
begin
  // Clear existing items
  cbVideoCodecs.Items.Clear;
  cbAudioCodecs.Items.Clear;
  
  // Populate video codecs
  for I := 0 to VideoCapture1.Video_Codecs_GetCount - 1 do
    cbVideoCodecs.Items.Add(VideoCapture1.Video_Codecs_GetItem(i));
    
  // Populate audio codecs
  for I := 0 to VideoCapture1.Audio_Codecs_GetCount - 1 do
    cbAudioCodecs.Items.Add(VideoCapture1.Audio_Codecs_GetItem(i));
end;
```

For developers using C++ MFC, the equivalent code would be:

```cpp
void CMyDialog::PopulateCodecLists()
{
    // Clear existing items
    m_VideoCodecsCombo.ResetContent();
    m_AudioCodecsCombo.ResetContent();
    
    // Populate video codecs
    for (int i = 0; i < m_VideoCapture.Video_Codecs_GetCount(); i++) {
        CString codecName = m_VideoCapture.Video_Codecs_GetItem(i);
        m_VideoCodecsCombo.AddString(codecName);
    }
    
    // Populate audio codecs
    for (int i = 0; i < m_VideoCapture.Audio_Codecs_GetCount(); i++) {
        CString codecName = m_VideoCapture.Audio_Codecs_GetItem(i);
        m_AudioCodecsCombo.AddString(codecName);
    }
}
```

For VB6 developers, here's how to implement the same functionality:

```vb
Private Sub PopulateCodecLists()
    ' Clear existing items
    cboVideoCodecs.Clear
    cboAudioCodecs.Clear
    
    ' Populate video codecs
    Dim i As Integer
    For i = 0 To VideoCapture1.Video_Codecs_GetCount - 1
        cboVideoCodecs.AddItem VideoCapture1.Video_Codecs_GetItem(i)
    Next i
    
    ' Populate audio codecs
    For i = 0 To VideoCapture1.Audio_Codecs_GetCount - 1
        cboAudioCodecs.AddItem VideoCapture1.Audio_Codecs_GetItem(i)
    Next i
End Sub
```

### Selecting Codecs for Capture

Once you've populated the lists, you'll need to let users select their preferred codecs and apply those selections to the capture component:

```pascal
procedure TMyForm.ApplyCodecSelections;
begin
  if cbVideoCodecs.ItemIndex >= 0 then
    VideoCapture1.Video_Codec := cbVideoCodecs.Items[cbVideoCodecs.ItemIndex];
    
  if cbAudioCodecs.ItemIndex >= 0 then
    VideoCapture1.Audio_Codec := cbAudioCodecs.Items[cbAudioCodecs.ItemIndex];
end;
```

C++ MFC implementation:

```cpp
void CMyDialog::ApplyCodecSelections()
{
    int videoIndex = m_VideoCodecsCombo.GetCurSel();
    if (videoIndex >= 0) {
        CString videoCodec;
        m_VideoCodecsCombo.GetLBText(videoIndex, videoCodec);
        m_VideoCapture.Video_Codec = videoCodec;
    }
    
    int audioIndex = m_AudioCodecsCombo.GetCurSel();
    if (audioIndex >= 0) {
        CString audioCodec;
        m_AudioCodecsCombo.GetLBText(audioIndex, audioCodec);
        m_VideoCapture.Audio_Codec = audioCodec;
    }
}
```

VB6 implementation:

```vb
Private Sub ApplyCodecSelections()
    If cboVideoCodecs.ListIndex >= 0 Then
        VideoCapture1.Video_Codec = cboVideoCodecs.Text
    End If
    
    If cboAudioCodecs.ListIndex >= 0 Then
        VideoCapture1.Audio_Codec = cboAudioCodecs.Text
    End If
End Sub
```

## Configuring Audio Parameters

Quality audio capture requires proper configuration of three key parameters:

1. **Audio Channels**: Typically 1 (mono) or 2 (stereo)
2. **Bits Per Sample (BPS)**: Common values include 8, 16, or 24 bits
3. **Sample Rate**: Standard rates include 44100 Hz (CD quality) or 48000 Hz

Here's how to apply these settings in Delphi:

```pascal
procedure TMyForm.ConfigureAudioSettings;
begin
  // Apply audio channel configuration (mono/stereo)
  VideoCapture1.Audio_Channels := StrToInt(cbChannels.Items[cbChannels.ItemIndex]);
  
  // Set bits per sample for audio quality
  VideoCapture1.Audio_BPS := StrToInt(cbBPS.Items[cbBPS.ItemIndex]);
  
  // Configure sample rate
  VideoCapture1.Audio_SampleRate := StrToInt(cbSampleRate.Items[cbSampleRate.ItemIndex]);
end;
```

C++ MFC implementation:

```cpp
void CMyDialog::ConfigureAudioSettings()
{
    CString channelStr, bpsStr, sampleRateStr;
    
    // Get selected values from combo boxes
    m_ChannelsCombo.GetLBText(m_ChannelsCombo.GetCurSel(), channelStr);
    m_BpsCombo.GetLBText(m_BpsCombo.GetCurSel(), bpsStr);
    m_SampleRateCombo.GetLBText(m_SampleRateCombo.GetCurSel(), sampleRateStr);
    
    // Apply audio channel configuration
    m_VideoCapture.Audio_Channels = _ttoi(channelStr);
    
    // Set bits per sample
    m_VideoCapture.Audio_BPS = _ttoi(bpsStr);
    
    // Configure sample rate
    m_VideoCapture.Audio_SampleRate = _ttoi(sampleRateStr);
}
```

VB6 implementation:

```vb
Private Sub ConfigureAudioSettings()
    ' Apply audio channel configuration
    VideoCapture1.Audio_Channels = CInt(cboChannels.Text)
    
    ' Set bits per sample
    VideoCapture1.Audio_BPS = CInt(cboBPS.Text)
    
    ' Configure sample rate
    VideoCapture1.Audio_SampleRate = CInt(cboSampleRate.Text)
End Sub
```

## Configuring Output Format and Capture Mode

The next step is to configure the output format as AVI and set the appropriate capture mode:

```pascal
procedure TMyForm.PrepareForCapture;
begin
  // Set AVI as the output format
  VideoCapture1.OutputFormat := Format_AVI;
  
  // Configure video capture mode
  VideoCapture1.Mode := Mode_Video_Capture;
end;
```

C++ MFC implementation:

```cpp
void CMyDialog::PrepareForCapture()
{
    // Set AVI as the output format
    m_VideoCapture.OutputFormat = Format_AVI;
    
    // Configure video capture mode
    m_VideoCapture.Mode = Mode_Video_Capture;
}
```

VB6 implementation:

```vb
Private Sub PrepareForCapture()
    ' Set AVI as the output format
    VideoCapture1.OutputFormat = Format_AVI
    
    ' Configure video capture mode
    VideoCapture1.Mode = Mode_Video_Capture
End Sub
```

## Starting and Managing the Capture Process

Once everything is configured, you can start the capture process:

```pascal
procedure TMyForm.StartCapture;
begin
  try
    // Set output filename
    VideoCapture1.Output := ExtractFilePath(Application.ExeName) + 'CapturedVideo.avi';
    
    // Begin capture process
    VideoCapture1.Start;
    
    // Update UI to show capture in progress
    btnStart.Enabled := False;
    btnStop.Enabled := True;
    lblStatus.Caption := 'Recording...';
  except
    on E: Exception do
      ShowMessage('Error starting capture: ' + E.Message);
  end;
end;
```

C++ MFC implementation:

```cpp
void CMyDialog::StartCapture()
{
    try {
        TCHAR appPath[MAX_PATH];
        GetModuleFileName(NULL, appPath, MAX_PATH);
        CString appDir = appPath;
        int pos = appDir.ReverseFind('\\');
        if (pos != -1) {
            appDir = appDir.Left(pos + 1);
        }
        
        // Set output filename
        m_VideoCapture.Output = appDir + _T("CapturedVideo.avi");
        
        // Begin capture process
        m_VideoCapture.Start();
        
        // Update UI
        GetDlgItem(IDC_START_BUTTON)->EnableWindow(FALSE);
        GetDlgItem(IDC_STOP_BUTTON)->EnableWindow(TRUE);
        SetDlgItemText(IDC_STATUS_STATIC, _T("Recording..."));
    }
    catch (COleDispatchException* e) {
        CString errorMsg = _T("Error starting capture: ");
        errorMsg += e->m_strDescription;
        MessageBox(errorMsg, _T("Error"), MB_ICONERROR);
        e->Delete();
    }
}
```

VB6 implementation:

```vb
Private Sub StartCapture()
    On Error GoTo ErrorHandler
    
    ' Set output filename
    VideoCapture1.Output = App.Path & "\CapturedVideo.avi"
    
    ' Begin capture process
    VideoCapture1.Start
    
    ' Update UI
    btnStart.Enabled = False
    btnStop.Enabled = True
    lblStatus.Caption = "Recording..."
    
    Exit Sub
    
ErrorHandler:
    MsgBox "Error starting capture: " & Err.Description, vbExclamation
End Sub
```

## Handling Capture Completion

It's important to provide functionality to stop the capture process:

```pascal
procedure TMyForm.StopCapture;
begin
  try
    // Stop the capture process
    VideoCapture1.Stop;
    
    // Update UI
    btnStart.Enabled := True;
    btnStop.Enabled := False;
    lblStatus.Caption := 'Capture completed';
    
    // Optionally open the captured file
    if FileExists(VideoCapture1.Output) and (MessageDlg('Open captured video?', 
                                                       mtConfirmation, [mbYes, mbNo], 0) = mrYes) then
      ShellExecute(0, 'open', PChar(VideoCapture1.Output), nil, nil, SW_SHOW);
  except
    on E: Exception do
      ShowMessage('Error stopping capture: ' + E.Message);
  end;
end;
```

C++ MFC implementation:

```cpp
void CMyDialog::StopCapture()
{
    try {
        // Stop the capture process
        m_VideoCapture.Stop();
        
        // Update UI
        GetDlgItem(IDC_START_BUTTON)->EnableWindow(TRUE);
        GetDlgItem(IDC_STOP_BUTTON)->EnableWindow(FALSE);
        SetDlgItemText(IDC_STATUS_STATIC, _T("Capture completed"));
        
        // Optionally open the captured file
        CString outputFile = m_VideoCapture.Output;
        if (PathFileExists(outputFile) && 
            MessageBox(_T("Open captured video?"), _T("Confirmation"), 
                      MB_YESNO | MB_ICONQUESTION) == IDYES) {
            ShellExecute(NULL, _T("open"), outputFile, NULL, NULL, SW_SHOW);
        }
    }
    catch (COleDispatchException* e) {
        CString errorMsg = _T("Error stopping capture: ");
        errorMsg += e->m_strDescription;
        MessageBox(errorMsg, _T("Error"), MB_ICONERROR);
        e->Delete();
    }
}
```

VB6 implementation:

```vb
Private Sub StopCapture()
    On Error GoTo ErrorHandler
    
    ' Stop the capture process
    VideoCapture1.Stop
    
    ' Update UI
    btnStart.Enabled = True
    btnStop.Enabled = False
    lblStatus.Caption = "Capture completed"
    
    ' Optionally open the captured file
    If Dir(VideoCapture1.Output) <> "" Then
        If MsgBox("Open captured video?", vbQuestion + vbYesNo) = vbYes Then
            Shell "explorer.exe """ & VideoCapture1.Output & """", vbNormalFocus
        End If
    End If
    
    Exit Sub
    
ErrorHandler:
    MsgBox "Error stopping capture: " & Err.Description, vbExclamation
End Sub
```

## Conclusion

Implementing video capture to AVI files in Delphi applications using the TVFVideoCapture component is a straightforward process when you understand the key concepts. By following this guide, you can create robust multimedia applications with professional video capture functionality.

The TVFVideoCapture component provides a wide range of additional features and customization options beyond what's covered in this guide, including video effects, overlays, and device property configuration.

Remember to test your video capture implementation thoroughly with different codecs and audio configurations to ensure the best quality for your specific use case.

---

For additional code samples and implementation guidance, visit our GitHub repository. If you need further assistance with this tutorial, our support team is available to help.
