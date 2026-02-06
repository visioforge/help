---
title: Salida Personalizada DirectShow en Delphi
description: Implementar formatos de salida personalizados DirectShow en Delphi, C++, VB6 - integrar filtros de terceros, códecs y multiplexores con ejemplos de código.
---

# Ejemplo de código - Formatos de salida personalizados

Código de ejemplo para Delphi, C++ MFC y VB6.

Actualmente, hay varias opciones para conectar filtros DirectShow de terceros para obtener el formato necesario.

## La primera opción - 3 filtros DirectShow diferentes

Un códec de audio, un códec de video y un multiplexor – filtros diferentes. Puede usar tanto filtros DirectShow como códecs regulares como códecs.

## La segunda opción - un filtro DirectShow todo en uno

Un multiplexor, un códec de video y un códec de audio – el mismo filtro.
Otra diferencia es si el filtro puede escribir a un archivo por sí mismo, si debe usar el filtro File Writer estándar, o si necesita otro filtro especial.

En los primeros dos casos, VisioForge Video Capture lo detectará automáticamente y establecerá los parámetros necesarios, pero tiene que especificar el filtro necesario usted mismo en el tercer caso.

Ahora, veamos cómo luce el código para las diferentes opciones.

## Primera opción

Obtener listas de códecs de audio y video

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

Obtener la lista de filtros DirectShow

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

Seleccionar filtros y códecs

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

## Segunda opción

Obtener listas de filtros DirectShow.

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

Seleccionar filtro multiplexor (mux)

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

Si necesita un filtro File Writer especial, debe especificarlo. Esto es verdad para ambas opciones descritas anteriormente.

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

Iniciar captura

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
Por favor contacte con [soporte](https://support.visioforge.com/) para obtener ayuda con este tutorial. Visite nuestra página de [GitHub](https://github.com/visioforge/) para obtener más ejemplos de código.