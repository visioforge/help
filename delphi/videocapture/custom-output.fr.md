---
title: Formats de sortie DirectShow en Delphi — Guide complet
description: Implémentez des formats de sortie DirectShow personnalisés en Delphi, C++, VB6 — intégrez des filtres tiers, codecs et multiplexeurs avec des exemples.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Capture
  - Encoding

---

# Exemple de code — Formats de sortie personnalisés

Exemples de code Delphi, C++ MFC et VB6.

Actuellement, il existe plusieurs options pour connecter des filtres DirectShow tiers afin d'obtenir le format nécessaire.

## Première option — 3 filtres DirectShow différents

Un codec audio, un codec vidéo et un multiplexeur — des filtres distincts. Vous pouvez utiliser à la fois des filtres DirectShow et des codecs ordinaires comme codecs.

## Deuxième option — un filtre DirectShow tout-en-un

Un multiplexeur, un codec vidéo et un codec audio — le même filtre.  
Une autre différence concerne la capacité du filtre à écrire lui-même dans un fichier, l'utilisation du filtre File Writer standard ou la nécessité d'un autre filtre spécifique.

Dans les deux premiers cas, VisioForge Video Capture le détectera automatiquement et définira les paramètres nécessaires, mais vous devrez spécifier vous-même le filtre nécessaire dans le troisième cas.

Voyons maintenant à quoi ressemble le code pour les différentes options.

## Première option

Obtenir les listes de codecs audio et vidéo

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

Obtenir la liste des filtres DirectShow

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

Sélectionner les filtres et codecs

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
VideoCapture1.Custom_Output_Mux_Filter_Name := cbCustomMuxer.Items[cbCustomMuxer.ItemIndex];
```

```cpp
// C++ MFC
// CComboBox::GetCurSel renvoie CB_ERR (-1) si rien n'est sélectionné ; on
// vérifie l'index avant chaque appel à GetLBText.
int nIndex;
if (m_CustomUseVideoCodecsCat.GetCheck())
{
  nIndex = m_CustomVideoCodecCombo.GetCurSel();
  if (nIndex != CB_ERR)
  {
    CString videoCodec;
    m_CustomVideoCodecCombo.GetLBText(nIndex, videoCodec);
    m_VideoCapture.Custom_Output_Video_Codec = videoCodec;
    m_VideoCapture.Custom_Output_Video_Codec_Use_Filters_Category = false;
  }
}
else
{
  nIndex = m_CustomDSFilterVCombo.GetCurSel();
  if (nIndex != CB_ERR)
  {
    CString videoCodec;
    m_CustomDSFilterVCombo.GetLBText(nIndex, videoCodec);
    m_VideoCapture.Custom_Output_Video_Codec = videoCodec;
    m_VideoCapture.Custom_Output_Video_Codec_Use_Filters_Category = true;
  }
}

if (m_CustomUseAudioCodecsCat.GetCheck())
{
  nIndex = m_CustomAudioCodecCombo.GetCurSel();
  if (nIndex != CB_ERR)
  {
    CString audioCodec;
    m_CustomAudioCodecCombo.GetLBText(nIndex, audioCodec);
    m_VideoCapture.Custom_Output_Audio_Codec = audioCodec;
    m_VideoCapture.Custom_Output_Audio_Codec_Use_Filters_Category = false;
  }
}
else
{
  nIndex = m_CustomDSFilterACombo.GetCurSel();
  if (nIndex != CB_ERR)
  {
    CString audioCodec;
    m_CustomDSFilterACombo.GetLBText(nIndex, audioCodec);
    m_VideoCapture.Custom_Output_Audio_Codec = audioCodec;
    m_VideoCapture.Custom_Output_Audio_Codec_Use_Filters_Category = true;
  }
}

nIndex = m_CustomMuxerCombo.GetCurSel();
if (nIndex != CB_ERR)
{
  CString muxerName;
  m_CustomMuxerCombo.GetLBText(nIndex, muxerName);
  m_VideoCapture.Custom_Output_Mux_Filter_Name = muxerName;
}
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

## Deuxième option

Obtenir les listes de filtres DirectShow.

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

Sélectionner le filtre multiplexeur (mux)

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

Si vous avez besoin d'un filtre File Writer spécial, vous devez le spécifier. Cela vaut pour les deux options décrites ci-dessus.

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

Démarrer la capture

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
Veuillez contacter le [support](https://support.visioforge.com/) pour obtenir de l'aide sur ce tutoriel. Visitez notre page [GitHub](https://github.com/visioforge/) pour davantage d'exemples de code.
