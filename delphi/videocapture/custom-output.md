---
title: DirectShow Output Formats in Delphi - Complete Guide
description: Implement DirectShow custom output formats in Delphi, C++, VB6 - integrate third-party filters, codecs, and multiplexers with code examples.
---

# Code sample - Custom output formats

Delphi, C++ MFC, and VB6 sample code.

Currently, there are several options for connecting third-party DirectShow filters to get the necessary format.

## The first option - 3 different DirectShow filters

An audio codec, a video codec, and a multiplexer – different filter. You can use both DirectShow filters and regular codecs as codecs.

## The second option - an all-in-one DirectShow filter

A multiplexer, a video codec, and an audio codec – the same filter.  
Another difference is whether the filter can write to a file itself, whether you should use the standard File Writer filter, or whether you need another special filter.

In the first two cases, VisioForge Video Capture will detect it automatically and set the necessary parameters, but you have to specify the necessary filter yourself in the third case.

Now, let us see what the code for different options looks like.

## First option

Get lists of audio and video codecs

```pascal
for I := 0 to VideoCapture1.Video_Codecs_GetCount - 1 do
  cbCustomVideoCodec.Items.Add(VideoCapture1.Video_Codecs_GetItem(i));
for I := 0 to VideoCapture1.Audio_Codecs_GetCount - 1 do
  cbCustomAudioCodec.Items.Add(VideoCapture1.Audio_Codecs_GetItem(i));
```

```cpp
// C++ MFC
for (int i = 0; i < m_VideoCapture.Video_Codecs_GetCount(); i++)
  m_CustomVideoCodecCombo.AddString(m_VideoCapture.Video_Codecs_GetItem(i));
for (int i = 0; i < m_VideoCapture.Audio_Codecs_GetCount(); i++)
  m_CustomAudioCodecCombo.AddString(m_VideoCapture.Audio_Codecs_GetItem(i));
```

```vb
' VB6
For i = 0 To VideoCapture1.Video_Codecs_GetCount - 1
  cbCustomVideoCodec.AddItem VideoCapture1.Video_Codecs_GetItem(i)
Next i
For i = 0 To VideoCapture1.Audio_Codecs_GetCount - 1
  cbCustomAudioCodec.AddItem VideoCapture1.Audio_Codecs_GetItem(i)
Next i
```

Get the list of DirectShow filters

```pascal
for I := 0 to VideoCapture1.DirectShow_Filters_GetCount - 1 do
  begin
    cbCustomDSFilterV.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
    cbCustomDSFilterA.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
    cbCustomMuxer.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
    cbCustomFilewriter.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
  end;
```

```cpp
// C++ MFC
for (int i = 0; i < m_VideoCapture.DirectShow_Filters_GetCount(); i++)
{
  m_CustomDSFilterVCombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
  m_CustomDSFilterACombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
  m_CustomMuxerCombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
  m_CustomFilewriterCombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
}
```

```vb
' VB6
For i = 0 To VideoCapture1.DirectShow_Filters_GetCount - 1
  cbCustomDSFilterV.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
  cbCustomDSFilterA.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
  cbCustomMuxer.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
  cbCustomFilewriter.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
Next i
```

Select filters and codecs

```pascal
if rbCustomUseVideoCodecsCat.Checked then
  begin
    VideoCapture1.Custom_Output_Video_Codec := cbCustomVideoCodec.Items[cbCustomVideoCodec.ItemIndex];
    VideoCapture1.Custom_Output_Video_Codec_Use_Filters_Category := false;
  end
else
  begin
    VideoCapture1.Custom_Output_Video_Codec := cbCustomDSFilterV.Items[cbCustomDSFilterV.ItemIndex];
    VideoCapture1.Custom_Output_Video_Codec_Use_Filters_Category := true;
  end;

if rbCustomUseAudioCodecsCat.Checked then
  begin
    VideoCapture1.Custom_Output_Audio_Codec := cbCustomAudioCodec.Items[cbCustomAudioCodec.ItemIndex];
    VideoCapture1.Custom_Output_Audio_Codec_Use_Filters_Category := false;
  end
else
  begin
    VideoCapture1.Custom_Output_Audio_Codec := cbCustomDSFilterA.Items[cbCustomDSFilterA.ItemIndex];
    VideoCapture1.Custom_Output_Audio_Codec_Use_Filters_Category := true;
  end;
VideoCapture1. Custom_Output_Mux_Filter_Name := cbCustomMuxer.Items[cbCustomMuxer.ItemIndex];
```

```cpp
// C++ MFC
if (m_CustomUseVideoCodecsCat.GetCheck())
{
  CString videoCodec;
  m_CustomVideoCodecCombo.GetLBText(m_CustomVideoCodecCombo.GetCurSel(), videoCodec);
  m_VideoCapture.Custom_Output_Video_Codec = videoCodec;
  m_VideoCapture.Custom_Output_Video_Codec_Use_Filters_Category = false;
}
else
{
  CString videoCodec;
  m_CustomDSFilterVCombo.GetLBText(m_CustomDSFilterVCombo.GetCurSel(), videoCodec);
  m_VideoCapture.Custom_Output_Video_Codec = videoCodec;
  m_VideoCapture.Custom_Output_Video_Codec_Use_Filters_Category = true;
}

if (m_CustomUseAudioCodecsCat.GetCheck())
{
  CString audioCodec;
  m_CustomAudioCodecCombo.GetLBText(m_CustomAudioCodecCombo.GetCurSel(), audioCodec);
  m_VideoCapture.Custom_Output_Audio_Codec = audioCodec;
  m_VideoCapture.Custom_Output_Audio_Codec_Use_Filters_Category = false;
}
else
{
  CString audioCodec;
  m_CustomDSFilterACombo.GetLBText(m_CustomDSFilterACombo.GetCurSel(), audioCodec);
  m_VideoCapture.Custom_Output_Audio_Codec = audioCodec;
  m_VideoCapture.Custom_Output_Audio_Codec_Use_Filters_Category = true;
}

CString muxerName;
m_CustomMuxerCombo.GetLBText(m_CustomMuxerCombo.GetCurSel(), muxerName);
m_VideoCapture.Custom_Output_Mux_Filter_Name = muxerName;
```

```vb
' VB6
If rbCustomUseVideoCodecsCat.Value Then
  VideoCapture1.Custom_Output_Video_Codec = cbCustomVideoCodec.List(cbCustomVideoCodec.ListIndex)
  VideoCapture1.Custom_Output_Video_Codec_Use_Filters_Category = False
Else
  VideoCapture1.Custom_Output_Video_Codec = cbCustomDSFilterV.List(cbCustomDSFilterV.ListIndex)
  VideoCapture1.Custom_Output_Video_Codec_Use_Filters_Category = True
End If

If rbCustomUseAudioCodecsCat.Value Then
  VideoCapture1.Custom_Output_Audio_Codec = cbCustomAudioCodec.List(cbCustomAudioCodec.ListIndex)
  VideoCapture1.Custom_Output_Audio_Codec_Use_Filters_Category = False
Else
  VideoCapture1.Custom_Output_Audio_Codec = cbCustomDSFilterA.List(cbCustomDSFilterA.ListIndex)
  VideoCapture1.Custom_Output_Audio_Codec_Use_Filters_Category = True
End If
VideoCapture1.Custom_Output_Mux_Filter_Name = cbCustomMuxer.List(cbCustomMuxer.ListIndex)
```

## Second option

Get lists of DirectShow filters.

```pascal
for I := 0 to VideoCapture1.DirectShow_Filters_GetCount - 1 do
  begin
    cbCustomDSFilterV.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
    cbCustomDSFilterA.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
    cbCustomMuxer.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
    cbCustomFilewriter.Items.Add(VideoCapture1.DirectShow_Filters_GetItem(i));
  end;
```

```cpp
// C++ MFC
for (int i = 0; i < m_VideoCapture.DirectShow_Filters_GetCount(); i++)
{
  m_CustomDSFilterVCombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
  m_CustomDSFilterACombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
  m_CustomMuxerCombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
  m_CustomFilewriterCombo.AddString(m_VideoCapture.DirectShow_Filters_GetItem(i));
}
```

```vb
' VB6
For i = 0 To VideoCapture1.DirectShow_Filters_GetCount - 1
  cbCustomDSFilterV.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
  cbCustomDSFilterA.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
  cbCustomMuxer.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
  cbCustomFilewriter.AddItem VideoCapture1.DirectShow_Filters_GetItem(i)
Next i
```

Select multiplexer (mux) filter

```pascal
VideoCapture1.Custom_Output_Mux_Filter_Name := cbCustomMuxer.Items[cbCustomMuxer.ItemIndex];
VideoCapture1.Custom_Output_Mux_Filter_Is_Encoder := cbCustomMuxFilterIsEncoder.Checked;
```

```cpp
// C++ MFC
CString muxerName;
m_CustomMuxerCombo.GetLBText(m_CustomMuxerCombo.GetCurSel(), muxerName);
m_VideoCapture.Custom_Output_Mux_Filter_Name = muxerName;
m_VideoCapture.Custom_Output_Mux_Filter_Is_Encoder = m_CustomMuxFilterIsEncoder.GetCheck();
```

```vb
' VB6
VideoCapture1.Custom_Output_Mux_Filter_Name = cbCustomMuxer.List(cbCustomMuxer.ListIndex)
VideoCapture1.Custom_Output_Mux_Filter_Is_Encoder = cbCustomMuxFilterIsEncoder.Value
```

If you need a special File Writer filter, you should specify it. This is true for both options described above.

```pascal
VideoCapture1.Custom_Output_Special_FileWriter_Needed := cbUseSpecialFilewriter.Checked;
VideoCapture1.Custom_Output_Special_FileWriter_Filter_Name := cbCustomFilewriter.Items[cbCustomFilewriter.ItemIndex];
```

```cpp
// C++ MFC
m_VideoCapture.Custom_Output_Special_FileWriter_Needed = m_UseSpecialFilewriter.GetCheck();
CString fileWriterName;
m_CustomFilewriterCombo.GetLBText(m_CustomFilewriterCombo.GetCurSel(), fileWriterName);
m_VideoCapture.Custom_Output_Special_FileWriter_Filter_Name = fileWriterName;
```

```vb
' VB6
VideoCapture1.Custom_Output_Special_FileWriter_Needed = cbUseSpecialFilewriter.Value
VideoCapture1.Custom_Output_Special_FileWriter_Filter_Name = cbCustomFilewriter.List(cbCustomFilewriter.ListIndex)
```

Start capture

```pascal
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

---
Please get in touch with [support](https://support.visioforge.com/) to get help with this tutorial. Visit our [GitHub](https://github.com/visioforge/) page to get more code samples.