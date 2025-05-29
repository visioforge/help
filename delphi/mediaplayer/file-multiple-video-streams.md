---
title: Play Multiple Video Streams from Single File
description: Learn how to handle and play video files containing multiple video streams, including different camera angles and resolutions. Includes code examples for Delphi, C++, and VB6 with detailed implementation steps and best practices.
sidebar_label: How do I play a video file with multiple video streams?
---

# Playing Video Files with Multiple Video Streams

## Understanding Multiple Video Streams

### What Are Multiple Video Streams?

Multiple video streams refer to different video tracks contained within a single media file. These streams can vary in several ways:

- Different camera angles of the same scene
- Alternate versions with varying resolutions or bitrates
- Primary and secondary content (such as picture-in-picture)
- Different aspect ratios or formats of the same content
- Versions with or without special effects or graphics

### Supported File Formats

Many popular container formats support multiple video streams, including:

- **Matroska (MKV)**: Widely recognized for its flexibility and robust support for multiple streams
- **MP4/MPEG-4**: Common in both professional and consumer applications
- **AVI**: Although older, still widely used in some contexts
- **WebM**: Popular for web-based applications
- **TS/MTS**: Used in broadcast applications and consumer video cameras

Each format has its own characteristics and limitations regarding how it handles multiple video streams, but the `TVFMediaPlayer` component provides a unified approach to working with them.

## Implementing Multiple Video Stream Playback

### Setting Up the Media Player

The first step is to properly initialize the `TVFMediaPlayer` object. This involves creating the instance, configuring basic properties, and preparing it for playback:

```pascal
// Define and create the MediaPlayer object
var 
  MediaPlayer1: TVFMediaPlayer;
begin
  MediaPlayer1 := TVFMediaPlayer.Create(Self);
  
  // Set container size and position if needed
  MediaPlayer1.Parent := Panel1; // Assuming Panel1 is your container
  MediaPlayer1.Align := alClient;
  
  // Configure initial state
  MediaPlayer1.DoubleBuffered := True;
  MediaPlayer1.AutoPlay := False; // We'll control playback explicitly
```

### Configuring the Media Source

Next, we need to specify the media file and configure how it should be loaded:

```pascal
  // Set the file name - use full path for reliability
  MediaPlayer1.FilenameOrURL := 'C:\Videos\multistream-video.mkv';
  
  // Enable audio playback (default DirectSound audio renderer will be used)
  MediaPlayer1.Audio_Play := True;
  
  // Configure audio settings if needed
  MediaPlayer1.Audio_Volume := 85; // Set volume to 85%
  
  // Set the source mode to DirectShow
  // Other options include SM_File_FFMPEG or SM_File_VLC
  MediaPlayer1.Source_Mode := SM_File_DS;
```

### Selecting and Switching Video Streams

The key to working with multiple video streams is the `Source_VideoStreamIndex` property. This zero-based index allows you to select which video stream should be rendered:

```pascal
  // Set video stream index to 1 (second stream, as index is zero-based)
  MediaPlayer1.Source_VideoStreamIndex := 1;
  
  // Start playback
  MediaPlayer1.Play();
```

## C++ MFC Implementation

### Setting Up the Media Player

Here's how to implement multiple video stream playback using C++ with MFC:

```cpp
// In your header file (MyDlg.h)
private:
    CVFMediaPlayer* m_pMediaPlayer;

// In your implementation file (MyDlg.cpp)
BOOL CMyDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    
    // Create the MediaPlayer instance
    m_pMediaPlayer = new CVFMediaPlayer();
    
    // Initialize the control
    CWnd* pContainer = GetDlgItem(IDC_PLAYER_CONTAINER); // Your container control
    m_pMediaPlayer->Create(NULL, NULL, WS_CHILD | WS_VISIBLE, 
                          CRect(0, 0, 0, 0), pContainer, 1001);
    
    // Configure display settings
    m_pMediaPlayer->SetWindowPos(NULL, 0, 0, pContainer->GetClientRect().Width(),
                                pContainer->GetClientRect().Height(), SWP_NOZORDER);
    m_pMediaPlayer->PutDoubleBuffered(TRUE);
    m_pMediaPlayer->PutAutoPlay(FALSE);
    
    return TRUE;
}
```

### Configuring the Media Source

```cpp
void CMyDlg::PlayMultiStreamVideo()
{
    // Set the file path and configure source
    m_pMediaPlayer->PutFilenameOrURL(_T("C:\\Videos\\multistream-video.mkv"));
    
    // Configure audio
    m_pMediaPlayer->PutAudio_Play(TRUE);
    m_pMediaPlayer->PutAudio_Volume(85);
    
    // Set source mode to DirectShow
    m_pMediaPlayer->PutSource_Mode(SM_File_DS);
    
    // Select the second video stream (index 1)
    m_pMediaPlayer->PutSource_VideoStreamIndex(1);
    
    // Start playback
    m_pMediaPlayer->Play();
}

// Don't forget to clean up
void CMyDlg::OnDestroy()
{
    if (m_pMediaPlayer != NULL)
    {
        m_pMediaPlayer->DestroyWindow();
        delete m_pMediaPlayer;
        m_pMediaPlayer = NULL;
    }
    
    CDialog::OnDestroy();
}
```

## VB6 Implementation

Here's how to implement multiple video stream playback in Visual Basic 6:

```vb
' Declare the MediaPlayer object at form level
Private WithEvents MediaPlayer1 As TVFMediaPlayer

Private Sub Form_Load()
    ' Create the MediaPlayer instance
    Set MediaPlayer1 = New TVFMediaPlayer
    
    ' Set container properties
    MediaPlayer1.CreateControl
    MediaPlayer1.Parent = Frame1 ' Assuming Frame1 is your container
    MediaPlayer1.Left = 0
    MediaPlayer1.Top = 0
    MediaPlayer1.Width = Frame1.ScaleWidth
    MediaPlayer1.Height = Frame1.ScaleHeight
    
    ' Configure initial state
    MediaPlayer1.DoubleBuffered = True
    MediaPlayer1.AutoPlay = False
End Sub

Private Sub btnPlay_Click()
    ' Set the file name - use full path for reliability
    MediaPlayer1.FilenameOrURL = "C:\Videos\multistream-video.mkv"
    
    ' Enable audio playback
    MediaPlayer1.Audio_Play = True
    MediaPlayer1.Audio_Volume = 85 ' Set volume to 85%
    
    ' Set the source mode to DirectShow
    MediaPlayer1.Source_Mode = SM_File_DS
    
    ' Select the second video stream (index 1)
    MediaPlayer1.Source_VideoStreamIndex = 1
    
    ' Start playback
    MediaPlayer1.Play
End Sub

Private Sub Form_Unload(Cancel As Integer)
    ' Clean up resources
    Set MediaPlayer1 = Nothing
End Sub
```

## Conclusion

The ability to play video files with multiple streams opens up numerous possibilities for creating rich, interactive multimedia experiences. The `TVFMediaPlayer` component provides a straightforward approach to implementing this functionality, with flexible options to suit different application requirements.

By following the techniques outlined in this guide, you can effectively incorporate multiple video stream support into your applications, enhancing user experience and expanding the capabilities of your multimedia projects.

---

Please get in touch with [support](https://support.visioforge.com/) if you need assistance with this functionality. Visit our [GitHub](https://github.com/visioforge/) page for additional code samples and implementation examples.
