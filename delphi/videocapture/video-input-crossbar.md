---
title: Delphi Video Input Source Selection with Crossbar
description: Master video input source selection in Delphi applications using crossbar technology. Learn to programmatically configure composite, S-Video, HDMI inputs with step-by-step code examples. Implement robust camera input switching for professional Delphi video capture applications.
sidebar_label: Video Input Selection (Crossbar)
---

# Selecting Video Input Sources with Crossbar Technology

## Introduction to Video Input Selection

When developing applications that capture video from external devices, you'll often need to handle multiple input sources. The crossbar is a crucial component in video capture systems that allows you to route different physical inputs (like composite, S-Video, HDMI) to your application. This guide walks you through the process of detecting, configuring, and selecting video inputs using the crossbar interface in Delphi, C++ MFC, and Visual Basic 6 applications.

## Understanding Crossbar Technology

Crossbar technology functions as a routing matrix in video capture devices, enabling the connection between various inputs and outputs. Modern capture cards and TV tuners frequently incorporate crossbar functionality to facilitate switching between different video sources such as:

- Composite video inputs
- S-Video connections
- Component video
- HDMI inputs
- TV tuner inputs
- Digital video interfaces

Properly configuring these connections programmatically is essential for applications that need to dynamically switch between different video sources.

## Implementation Steps Overview

The implementation process for configuring crossbar connections in your application involves three main steps:

1. Initializing the crossbar interface and verifying its availability
2. Enumerating available video inputs for selection
3. Connecting the selected input to the video decoder output

Let's examine each step in detail with sample code for Delphi, C++ MFC, and VB6 environments.

## Detailed Implementation Guide

### Step 1: Initialize the Crossbar Interface

Before you can work with input selection, you need to initialize the crossbar interface and verify it's available on the current capture device.

#### Delphi Implementation

```pascal
// Initialize the crossbar interface
CrossBarFound := VideoCapture1.Video_CaptureDevice_CrossBar_Init;

// Check if crossbar functionality is available
if CrossBarFound then
  ShowMessage('Crossbar functionality detected and initialized')
else
  ShowMessage('No crossbar available on this capture device');
```

#### C++ MFC Implementation

```cpp
// Initialize the crossbar interface
BOOL bCrossBarFound = m_videoCapture.Video_CaptureDevice_CrossBar_Init();

// Check if crossbar functionality is available
if (bCrossBarFound) {
    AfxMessageBox(_T("Crossbar functionality detected and initialized"));
} else {
    AfxMessageBox(_T("No crossbar available on this capture device"));
}
```

#### VB6 Implementation

```vb
' Initialize the crossbar interface
Dim CrossBarFound As Boolean
CrossBarFound = VideoCapture1.Video_CaptureDevice_CrossBar_Init()

' Check if crossbar functionality is available
If CrossBarFound Then
    MsgBox "Crossbar functionality detected and initialized"
Else
    MsgBox "No crossbar available on this capture device"
End If
```

The initialization function returns a boolean value indicating whether the crossbar functionality is available on the current capture device. Not all capture devices support crossbar functionality, so this check is crucial.

### Step 2: Enumerate Available Video Inputs

Once you've confirmed that the crossbar is available, the next step is to retrieve a list of available inputs for the "Video Decoder" output. This allows users to select from available physical connections.

#### Delphi Implementation

```pascal
// Clear any existing connections and UI elements
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections;
cbCrossbarVideoInput.Clear;

// Get count of available inputs for the "Video Decoder" output
var inputCount: Integer := VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount('Video Decoder');

// Populate UI with available inputs
for i := 0 to inputCount - 1 do begin
  var inputName: String := VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem('Video Decoder', i);
  cbCrossbarVideoInput.Items.Add(inputName);
end;

// Select the first item by default if available
if cbCrossbarVideoInput.Items.Count > 0 then
  cbCrossbarVideoInput.ItemIndex := 0;
```

#### C++ MFC Implementation

```cpp
// Clear any existing connections and UI elements
m_videoCapture.Video_CaptureDevice_CrossBar_ClearConnections();
m_comboVideoInputs.ResetContent();

// Get count of available inputs for the "Video Decoder" output
int inputCount = m_videoCapture.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount(_T("Video Decoder"));

// Populate UI with available inputs
for (int i = 0; i < inputCount; i++) {
    CString inputName = m_videoCapture.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem(_T("Video Decoder"), i);
    m_comboVideoInputs.AddString(inputName);
}

// Select the first item by default if available
if (m_comboVideoInputs.GetCount() > 0) {
    m_comboVideoInputs.SetCurSel(0);
}
```

#### VB6 Implementation

```vb
' Clear any existing connections and UI elements
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections
cboVideoInputs.Clear

' Get count of available inputs for the "Video Decoder" output
Dim inputCount As Integer
inputCount = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount("Video Decoder")

' Populate UI with available inputs
Dim i As Integer
Dim inputName As String
For i = 0 To inputCount - 1
    inputName = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem("Video Decoder", i)
    cboVideoInputs.AddItem inputName
Next i

' Select the first item by default if available
If cboVideoInputs.ListCount > 0 Then
    cboVideoInputs.ListIndex = 0
End If
```

Common input types you might encounter include:

- Composite
- S-Video
- HDMI
- Component
- TV Tuner

The exact list depends on your specific capture hardware capabilities.

### Step 3: Apply the Selected Input

After the user selects their desired input source, you need to apply this selection by establishing a connection between the selected input and the video decoder output.

#### Delphi Implementation

```pascal
// First clear any existing connections
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections;

// Connect the selected input to the "Video Decoder" output
// Parameters: input name, output name, automatic signal routing
if cbCrossbarVideoInput.ItemIndex >= 0 then begin
  var selectedInput: String := cbCrossbarVideoInput.Items[cbCrossbarVideoInput.ItemIndex];
  var success: Boolean := VideoCapture1.Video_CaptureDevice_CrossBar_Connect(selectedInput, 'Video Decoder', true);
  
  if success then
    ShowMessage('Successfully connected ' + selectedInput + ' to Video Decoder')
  else
    ShowMessage('Failed to establish connection');
end;
```

#### C++ MFC Implementation

```cpp
// First clear any existing connections
m_videoCapture.Video_CaptureDevice_CrossBar_ClearConnections();

// Connect the selected input to the "Video Decoder" output
// Parameters: input name, output name, automatic signal routing
int selectedIndex = m_comboVideoInputs.GetCurSel();
if (selectedIndex >= 0) {
    CString selectedInput;
    m_comboVideoInputs.GetLBText(selectedIndex, selectedInput);
    
    BOOL success = m_videoCapture.Video_CaptureDevice_CrossBar_Connect(
        selectedInput, _T("Video Decoder"), TRUE);
    
    if (success) {
        CString msg;
        msg.Format(_T("Successfully connected %s to Video Decoder"), selectedInput);
        AfxMessageBox(msg);
    } else {
        AfxMessageBox(_T("Failed to establish connection"));
    }
}
```

#### VB6 Implementation

```vb
' First clear any existing connections
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections

' Connect the selected input to the "Video Decoder" output
' Parameters: input name, output name, automatic signal routing
If cboVideoInputs.ListIndex >= 0 Then
    Dim selectedInput As String
    selectedInput = cboVideoInputs.Text
    
    Dim success As Boolean
    success = VideoCapture1.Video_CaptureDevice_CrossBar_Connect(selectedInput, "Video Decoder", True)
    
    If success Then
        MsgBox "Successfully connected " & selectedInput & " to Video Decoder"
    Else
        MsgBox "Failed to establish connection"
    End If
End If
```

The third parameter (`true`) enables automatic signal routing, which helps handle complex connection scenarios where intermediate routing might be required.

## Best Practices for Crossbar Implementation

For robust video input selection in your applications:

1. **Always initialize the crossbar first**: Check for availability before attempting operations
2. **Clear existing connections**: Before setting a new connection, clear any existing ones
3. **Handle missing crossbar gracefully**: Provide fallback options when crossbar functionality isn't available
4. **Validate selections**: Ensure a valid input is selected before attempting to establish connections
5. **Provide user feedback**: Inform users about successful or failed connection attempts

## Troubleshooting Common Issues

If you encounter problems with crossbar connections:

- Verify your capture device supports crossbar functionality
- Check that input and output names match exactly what the device reports
- Ensure proper device driver installation
- Use debug logging to track connection attempts
- Test with different input sources to isolate hardware-specific issues

## Conclusion

Proper implementation of crossbar technology in your video capture applications gives users the flexibility to work with multiple input sources. By following the steps outlined in this guide, you can create a robust and user-friendly video input selection system for your applications regardless of whether you're developing in Delphi, C++ MFC, or Visual Basic 6.

The code samples provided demonstrate how to initialize the crossbar, enumerate available inputs, and connect selected inputs to the video decoder output. With these fundamentals in place, you can build sophisticated video capture applications that support a wide range of input devices and connection types.

---

For additional assistance with implementing this functionality, explore our other documentation pages and code samples repository for more advanced techniques and solutions.
