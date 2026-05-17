---
title: Sélectionner les périphériques vidéo et audio en Delphi
description: Sélectionnez les périphériques vidéo et audio en Delphi — énumération, formats et fréquence d'images avec exemples Delphi, C++ et VB6.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture

---

# Exemple de code — Comment sélectionner les périphériques de capture vidéo et audio ?

Exemples de code Delphi, C++ MFC et VB6

## Sélection de la source vidéo

### Obtenir la liste des périphériques de capture vidéo disponibles

```pascal
for i := 0 to VideoCapture1.Video_CaptureDevices_GetCount - 1 do
  cbVideoInputDevice.Items.Add(VideoCapture1.Video_CaptureDevices_GetItem(i));
```

```cpp
// C++ MFC
for (int i = 0; i < m_VideoCapture.Video_CaptureDevices_GetCount(); i++)
  m_cbVideoInputDevice.AddString(m_VideoCapture.Video_CaptureDevices_GetItem(i));
```

```vb
' VB6
For i = 0 To VideoCapture1.Video_CaptureDevices_GetCount - 1
  cbVideoInputDevice.AddItem VideoCapture1.Video_CaptureDevices_GetItem(i)
Next i
```

### Sélectionner le périphérique d'entrée vidéo

```pascal
VideoCapture1.Video_CaptureDevice := cbVideoInputDevice.Items[cbVideoInputDevice.ItemIndex];
```

```cpp
// C++ MFC
CString strDevice;
m_cbVideoInputDevice.GetLBText(m_cbVideoInputDevice.GetCurSel(), strDevice);
m_VideoCapture.put_Video_CaptureDevice(strDevice);
```

```vb
' VB6
VideoCapture1.Video_CaptureDevice = cbVideoInputDevice.Text
```

### Obtenir la liste des formats vidéo disponibles

```pascal
VideoCapture1.Video_CaptureDevice_Formats_Fill;
for I := 0 to VideoCapture1.Video_CaptureDevice_Formats_GetCount - 1 do
 cbVideoInputFormat.Items.Add(VideoCapture1.Video_CaptureDevice_Formats_GetItem(i));
```

```cpp
// C++ MFC
m_VideoCapture.Video_CaptureDevice_Formats_Fill();
for (int i = 0; i < m_VideoCapture.Video_CaptureDevice_Formats_GetCount(); i++)
  m_cbVideoInputFormat.AddString(m_VideoCapture.Video_CaptureDevice_Formats_GetItem(i));
```

```vb
' VB6
VideoCapture1.Video_CaptureDevice_Formats_Fill
For i = 0 To VideoCapture1.Video_CaptureDevice_Formats_GetCount - 1
  cbVideoInputFormat.AddItem VideoCapture1.Video_CaptureDevice_Formats_GetItem(i)
Next i
```

### Sélectionner le format vidéo

```pascal
VideoCapture1.Video_CaptureFormat := cbVideoInputFormat.Items[cbVideoInputFormat.ItemIndex];
```

```cpp
// C++ MFC
CString strFormat;
m_cbVideoInputFormat.GetLBText(m_cbVideoInputFormat.GetCurSel(), strFormat);
m_VideoCapture.put_Video_CaptureFormat(strFormat);
```

```vb
' VB6
VideoCapture1.Video_CaptureFormat = cbVideoInputFormat.Text
```

ou

### Choisir automatiquement le meilleur format vidéo

```pascal
VideoCapture1.Video_CaptureFormat_UseBest := cbUseBestVideoInputFormat.Checked;
```

```cpp
// C++ MFC
m_VideoCapture.put_Video_CaptureFormat_UseBest(m_cbUseBestVideoInputFormat.GetCheck() == BST_CHECKED);
```

```vb
' VB6
VideoCapture1.Video_CaptureFormat_UseBest = cbUseBestVideoInputFormat.Value
```

### Obtenir la liste des fréquences d'images disponibles

```pascal
VideoCapture1.Video_CaptureDevice_FrameRates_Fill;
for I := 0 to VideoCapture1.Video_CaptureDevice_FrameRates_GetCount - 1 do
  cbFrameRate.Items.Add(VideoCapture1.Video_CaptureDevice_FrameRates_GetItem(i));
```

```cpp
// C++ MFC
m_VideoCapture.Video_CaptureDevice_FrameRates_Fill();
for (int i = 0; i < m_VideoCapture.Video_CaptureDevice_FrameRates_GetCount(); i++)
  m_cbFrameRate.AddString(m_VideoCapture.Video_CaptureDevice_FrameRates_GetItem(i));
```

```vb
' VB6
VideoCapture1.Video_CaptureDevice_FrameRates_Fill
For i = 0 To VideoCapture1.Video_CaptureDevice_FrameRates_GetCount - 1
  cbFrameRate.AddItem VideoCapture1.Video_CaptureDevice_FrameRates_GetItem(i)
Next i
```

### Sélectionner la fréquence d'images

```pascal
VideoCapture1.Video_FrameRate := StrToFloat(cbFrameRate.Items[cbFrameRate.ItemIndex]);
```

```cpp
// C++ MFC
CString strFrameRate;
m_cbFrameRate.GetLBText(m_cbFrameRate.GetCurSel(), strFrameRate);
m_VideoCapture.put_Video_FrameRate(_wtof(strFrameRate));
```

```vb
' VB6
VideoCapture1.Video_FrameRate = CDbl(cbFrameRate.Text)
```

Sélectionnez l'entrée vidéo souhaitée (configurez le crossbar) si nécessaire.

## Sélection de la source audio

### Utiliser le périphérique de capture vidéo comme source audio

```pascal
VideoCapture1.Video_CaptureDevice_IsAudioSource := true;
```

```cpp
// C++ MFC
m_VideoCapture.put_Video_CaptureDevice_IsAudioSource(true);
```

```vb
' VB6
VideoCapture1.Video_CaptureDevice_IsAudioSource = True
```

ou

### Obtenir la liste des périphériques de capture audio disponibles

```pascal
for I := 0 to VideoCapture1.Audio_CaptureDevices_GetCount - 1 do
  cbAudioInputDevice.Items.Add(VideoCapture1.Audio_CaptureDevices_GetItem(i));
```

```cpp
// C++ MFC
for (int i = 0; i < m_VideoCapture.Audio_CaptureDevices_GetCount(); i++)
  m_cbAudioInputDevice.AddString(m_VideoCapture.Audio_CaptureDevices_GetItem(i));
```

```vb
' VB6
For i = 0 To VideoCapture1.Audio_CaptureDevices_GetCount - 1
  cbAudioInputDevice.AddItem VideoCapture1.Audio_CaptureDevices_GetItem(i)
Next i
```

### Sélectionner le périphérique d'entrée audio

```pascal
VideoCapture1.Audio_CaptureDevice := cbAudioInputDevice.Items[cbAudioInputDevice.ItemIndex];
```

```cpp
// C++ MFC
CString strAudioDevice;
m_cbAudioInputDevice.GetLBText(m_cbAudioInputDevice.GetCurSel(), strAudioDevice);
m_VideoCapture.put_Audio_CaptureDevice(strAudioDevice);
```

```vb
' VB6
VideoCapture1.Audio_CaptureDevice = cbAudioInputDevice.Text
```

### Obtenir la liste des formats audio disponibles

```pascal
VideoCapture1.Audio_CaptureDevice_Formats_Fill;
for I := 0 to VideoCapture1.Audio_CaptureDevice_Formats_GetCount - 1 do
  cbAudioInputFormat.Items.Add(VideoCapture1.Audio_CaptureDevice_Formats_GetItem(i));
```

```cpp
// C++ MFC
m_VideoCapture.Audio_CaptureDevice_Formats_Fill();
for (int i = 0; i < m_VideoCapture.Audio_CaptureDevice_Formats_GetCount(); i++)
  m_cbAudioInputFormat.AddString(m_VideoCapture.Audio_CaptureDevice_Formats_GetItem(i));
```

```vb
' VB6
VideoCapture1.Audio_CaptureDevice_Formats_Fill
For i = 0 To VideoCapture1.Audio_CaptureDevice_Formats_GetCount - 1
  cbAudioInputFormat.AddItem VideoCapture1.Audio_CaptureDevice_Formats_GetItem(i)
Next i
```

### Sélectionner le format

```pascal
VideoCapture1.Audio_CaptureFormat := cbAudioInputFormat.Items[cbAudioInputFormat.ItemIndex];
```

```cpp
// C++ MFC
CString strAudioFormat;
m_cbAudioInputFormat.GetLBText(m_cbAudioInputFormat.GetCurSel(), strAudioFormat);
m_VideoCapture.put_Audio_CaptureFormat(strAudioFormat);
```

```vb
' VB6
VideoCapture1.Audio_CaptureFormat = cbAudioInputFormat.Text
```

ou

### Choisir automatiquement le meilleur format audio

```pascal
VideoCapture1.Audio_CaptureFormat_UseBest := cbUseBestAudioInputFormat.Checked;
```

```cpp
// C++ MFC
m_VideoCapture.put_Audio_CaptureFormat_UseBest(m_cbUseBestAudioInputFormat.GetCheck() == BST_CHECKED);
```

```vb
' VB6
VideoCapture1.Audio_CaptureFormat_UseBest = cbUseBestAudioInputFormat.Value
```

### Obtenir la liste des entrées audio disponibles (lignes)

```pascal
VideoCapture1.Audio_CaptureDevice_Lines_Fill;
for I := 0 to VideoCapture1.Audio_CaptureDevice_Lines_GetCount - 1 do
  cbAudioInputLine.Items.Add(VideoCapture1.Audio_CaptureDevice_Lines_GetItem(i));
```

```cpp
// C++ MFC
m_VideoCapture.Audio_CaptureDevice_Lines_Fill();
for (int i = 0; i < m_VideoCapture.Audio_CaptureDevice_Lines_GetCount(); i++)
  m_cbAudioInputLine.AddString(m_VideoCapture.Audio_CaptureDevice_Lines_GetItem(i));
```

```vb
' VB6
VideoCapture1.Audio_CaptureDevice_Lines_Fill
For i = 0 To VideoCapture1.Audio_CaptureDevice_Lines_GetCount - 1
  cbAudioInputLine.AddItem VideoCapture1.Audio_CaptureDevice_Lines_GetItem(i)
Next i
```

### Sélectionner l'entrée audio

```pascal
VideoCapture1.Audio_CaptureLine := cbAudioInputLine.Items[cbAudioInputLine.ItemIndex];
```

```cpp
// C++ MFC
CString strAudioLine;
m_cbAudioInputLine.GetLBText(m_cbAudioInputLine.GetCurSel(), strAudioLine);
m_VideoCapture.put_Audio_CaptureLine(strAudioLine);
```

```vb
' VB6
VideoCapture1.Audio_CaptureLine = cbAudioInputLine.Text
```

---
Veuillez contacter le [support](https://support.visioforge.com/) pour obtenir de l'aide concernant ce tutoriel. Visitez notre page [GitHub](https://github.com/visioforge/) pour obtenir plus d'exemples de code.
