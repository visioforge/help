---
title: Capture vidéo AVI en Delphi avec le SDK TVFVideoCapture
description: Enregistrez la vidéo au format AVI en Delphi avec TVFVideoCapture — sélection de codec, configuration audio et exemples d'implémentation complets.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - C++
  - Windows
  - VCL
  - Capture
  - AVI
primary_api_classes:
  - TVFVideoCapture

---

# Guide complet pour la capture vidéo vers des fichiers AVI en Delphi

Lors du développement d'applications multimédias en Delphi, la fonctionnalité de capture vidéo est souvent une exigence essentielle. Ce guide explore comment implémenter une capture vidéo de haute qualité vers des fichiers AVI à l'aide du composant TVFVideoCapture dans des applications Delphi. Nous couvrirons tous les aspects, depuis la configuration des codecs jusqu'à la configuration des paramètres audio et au démarrage du processus de capture.

## Comprendre la capture vidéo AVI en Delphi

Le composant TVFVideoCapture fournit un moyen puissant et flexible de capturer la vidéo directement au format AVI dans les applications Delphi. AVI (Audio Video Interleave) reste un format de conteneur vidéo populaire en raison de sa large compatibilité et de sa fiabilité à des fins d'enregistrement.

Lors de l'implémentation de la capture vidéo dans votre application Delphi, vous devrez tenir compte de plusieurs aspects clés :

1. Sélectionner les codecs vidéo et audio appropriés
2. Configurer les paramètres audio
3. Définir le format de sortie et le mode de capture
4. Gérer le processus de capture

Ce guide fournit des explications détaillées et des exemples de code pour chacune de ces étapes.

## Travailler avec les codecs vidéo et audio

### Récupération des codecs disponibles

Avant de capturer la vidéo, vous devrez remplir votre application avec les codecs vidéo et audio disponibles. Le composant TVFVideoCapture rend cette tâche simple :

```pascal
procedure TMyForm.PopulateCodecLists;
var
  I: Integer;
begin
  // Effacer les éléments existants
  cbVideoCodecs.Items.Clear;
  cbAudioCodecs.Items.Clear;
  
  // Remplir les codecs vidéo
  for I := 0 to VideoCapture1.Video_Codecs_GetCount - 1 do
    cbVideoCodecs.Items.Add(VideoCapture1.Video_Codecs_GetItem(i));
    
  // Remplir les codecs audio
  for I := 0 to VideoCapture1.Audio_Codecs_GetCount - 1 do
    cbAudioCodecs.Items.Add(VideoCapture1.Audio_Codecs_GetItem(i));
end;
```

Pour les développeurs utilisant C++ MFC, le code équivalent serait :

```cpp
void CMyDialog::PopulateCodecLists()
{
    // Effacer les éléments existants
    m_VideoCodecsCombo.ResetContent();
    m_AudioCodecsCombo.ResetContent();
    
    // Remplir les codecs vidéo
    for (int i = 0; i < m_VideoCapture.Video_Codecs_GetCount(); i++) {
        CString codecName = m_VideoCapture.Video_Codecs_GetItem(i);
        m_VideoCodecsCombo.AddString(codecName);
    }
    
    // Remplir les codecs audio
    for (int i = 0; i < m_VideoCapture.Audio_Codecs_GetCount(); i++) {
        CString codecName = m_VideoCapture.Audio_Codecs_GetItem(i);
        m_AudioCodecsCombo.AddString(codecName);
    }
}
```

Pour les développeurs VB6, voici comment implémenter la même fonctionnalité :

```vb
Private Sub PopulateCodecLists()
    ' Effacer les éléments existants
    cboVideoCodecs.Clear
    cboAudioCodecs.Clear
    
    ' Remplir les codecs vidéo
    Dim i As Integer
    For i = 0 To VideoCapture1.Video_Codecs_GetCount - 1
        cboVideoCodecs.AddItem VideoCapture1.Video_Codecs_GetItem(i)
    Next i
    
    ' Remplir les codecs audio
    For i = 0 To VideoCapture1.Audio_Codecs_GetCount - 1
        cboAudioCodecs.AddItem VideoCapture1.Audio_Codecs_GetItem(i)
    Next i
End Sub
```

### Sélection des codecs pour la capture

Une fois les listes remplies, vous devrez permettre aux utilisateurs de sélectionner leurs codecs préférés et appliquer ces sélections au composant de capture :

```pascal
procedure TMyForm.ApplyCodecSelections;
begin
  if cbVideoCodecs.ItemIndex >= 0 then
    VideoCapture1.Video_Codec := cbVideoCodecs.Items[cbVideoCodecs.ItemIndex];
    
  if cbAudioCodecs.ItemIndex >= 0 then
    VideoCapture1.Audio_Codec := cbAudioCodecs.Items[cbAudioCodecs.ItemIndex];
end;
```

Implémentation C++ MFC :

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

Implémentation VB6 :

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

## Configuration des paramètres audio

Une capture audio de qualité requiert une configuration appropriée de trois paramètres clés :

1. **Canaux audio** : généralement 1 (mono) ou 2 (stéréo)
2. **Bits par échantillon (BPS)** : les valeurs courantes incluent 8, 16 ou 24 bits
3. **Fréquence d'échantillonnage** : les fréquences standards incluent 44100 Hz (qualité CD) ou 48000 Hz

Voici comment appliquer ces paramètres en Delphi :

```pascal
procedure TMyForm.ConfigureAudioSettings;
begin
  // Appliquer la configuration des canaux audio (mono/stéréo)
  VideoCapture1.Audio_Channels := StrToInt(cbChannels.Items[cbChannels.ItemIndex]);
  
  // Définir les bits par échantillon pour la qualité audio
  VideoCapture1.Audio_BPS := StrToInt(cbBPS.Items[cbBPS.ItemIndex]);
  
  // Configurer la fréquence d'échantillonnage
  VideoCapture1.Audio_SampleRate := StrToInt(cbSampleRate.Items[cbSampleRate.ItemIndex]);
end;
```

Implémentation C++ MFC :

```cpp
void CMyDialog::ConfigureAudioSettings()
{
    CString channelStr, bpsStr, sampleRateStr;
    
    // Récupérer les valeurs sélectionnées dans les combo box
    m_ChannelsCombo.GetLBText(m_ChannelsCombo.GetCurSel(), channelStr);
    m_BpsCombo.GetLBText(m_BpsCombo.GetCurSel(), bpsStr);
    m_SampleRateCombo.GetLBText(m_SampleRateCombo.GetCurSel(), sampleRateStr);
    
    // Appliquer la configuration des canaux audio
    m_VideoCapture.Audio_Channels = _ttoi(channelStr);
    
    // Définir les bits par échantillon
    m_VideoCapture.Audio_BPS = _ttoi(bpsStr);
    
    // Configurer la fréquence d'échantillonnage
    m_VideoCapture.Audio_SampleRate = _ttoi(sampleRateStr);
}
```

Implémentation VB6 :

```vb
Private Sub ConfigureAudioSettings()
    ' Appliquer la configuration des canaux audio
    VideoCapture1.Audio_Channels = CInt(cboChannels.Text)
    
    ' Définir les bits par échantillon
    VideoCapture1.Audio_BPS = CInt(cboBPS.Text)
    
    ' Configurer la fréquence d'échantillonnage
    VideoCapture1.Audio_SampleRate = CInt(cboSampleRate.Text)
End Sub
```

## Configuration du format de sortie et du mode de capture

L'étape suivante consiste à configurer le format de sortie en AVI et à définir le mode de capture approprié :

```pascal
procedure TMyForm.PrepareForCapture;
begin
  // Définir AVI comme format de sortie
  VideoCapture1.OutputFormat := Format_AVI;
  
  // Configurer le mode de capture vidéo
  VideoCapture1.Mode := Mode_Video_Capture;
end;
```

Implémentation C++ MFC :

```cpp
void CMyDialog::PrepareForCapture()
{
    // Définir AVI comme format de sortie
    m_VideoCapture.OutputFormat = Format_AVI;
    
    // Configurer le mode de capture vidéo
    m_VideoCapture.Mode = Mode_Video_Capture;
}
```

Implémentation VB6 :

```vb
Private Sub PrepareForCapture()
    ' Définir AVI comme format de sortie
    VideoCapture1.OutputFormat = Format_AVI
    
    ' Configurer le mode de capture vidéo
    VideoCapture1.Mode = Mode_Video_Capture
End Sub
```

## Démarrage et gestion du processus de capture

Une fois tout configuré, vous pouvez démarrer le processus de capture :

```pascal
procedure TMyForm.StartCapture;
begin
  try
    // Définir le nom de fichier de sortie
    VideoCapture1.Output_Filename := ExtractFilePath(Application.ExeName) + 'CapturedVideo.avi';
    
    // Lancer le processus de capture
    VideoCapture1.Start;
    
    // Mettre à jour l'UI pour indiquer la capture en cours
    btnStart.Enabled := False;
    btnStop.Enabled := True;
    lblStatus.Caption := 'Enregistrement...';
  except
    on E: Exception do
      ShowMessage('Erreur de démarrage de la capture : ' + E.Message);
  end;
end;
```

Implémentation C++ MFC :

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
        
        // Définir le nom de fichier de sortie
        m_VideoCapture.Output_Filename = appDir + _T("CapturedVideo.avi");
        
        // Lancer le processus de capture
        m_VideoCapture.Start();
        
        // Mettre à jour l'UI
        GetDlgItem(IDC_START_BUTTON)->EnableWindow(FALSE);
        GetDlgItem(IDC_STOP_BUTTON)->EnableWindow(TRUE);
        SetDlgItemText(IDC_STATUS_STATIC, _T("Enregistrement..."));
    }
    catch (COleDispatchException* e) {
        CString errorMsg = _T("Erreur de démarrage de la capture : ");
        errorMsg += e->m_strDescription;
        MessageBox(errorMsg, _T("Erreur"), MB_ICONERROR);
        e->Delete();
    }
}
```

Implémentation VB6 :

```vb
Private Sub StartCapture()
    On Error GoTo ErrorHandler
    
    ' Définir le nom de fichier de sortie
    VideoCapture1.Output_Filename = App.Path & "\CapturedVideo.avi"
    
    ' Lancer le processus de capture
    VideoCapture1.Start
    
    ' Mettre à jour l'UI
    btnStart.Enabled = False
    btnStop.Enabled = True
    lblStatus.Caption = "Enregistrement..."
    
    Exit Sub
    
ErrorHandler:
    MsgBox "Erreur de démarrage de la capture : " & Err.Description, vbExclamation
End Sub
```

## Gestion de la fin de la capture

Il est important de fournir une fonctionnalité pour arrêter le processus de capture :

```pascal
procedure TMyForm.StopCapture;
begin
  try
    // Arrêter le processus de capture
    VideoCapture1.Stop;
    
    // Mettre à jour l'UI
    btnStart.Enabled := True;
    btnStop.Enabled := False;
    lblStatus.Caption := 'Capture terminée';
    
    // Ouvrir éventuellement le fichier capturé
    if FileExists(VideoCapture1.Output_Filename) and (MessageDlg('Ouvrir la vidéo capturée ?', 
                                                       mtConfirmation, [mbYes, mbNo], 0) = mrYes) then
      ShellExecute(0, 'open', PChar(VideoCapture1.Output_Filename), nil, nil, SW_SHOW);
  except
    on E: Exception do
      ShowMessage('Erreur d''arrêt de la capture : ' + E.Message);
  end;
end;
```

Implémentation C++ MFC :

```cpp
void CMyDialog::StopCapture()
{
    try {
        // Arrêter le processus de capture
        m_VideoCapture.Stop();
        
        // Mettre à jour l'UI
        GetDlgItem(IDC_START_BUTTON)->EnableWindow(TRUE);
        GetDlgItem(IDC_STOP_BUTTON)->EnableWindow(FALSE);
        SetDlgItemText(IDC_STATUS_STATIC, _T("Capture terminée"));
        
        // Ouvrir éventuellement le fichier capturé
        CString outputFile = m_VideoCapture.Output_Filename;
        if (PathFileExists(outputFile) && 
            MessageBox(_T("Ouvrir la vidéo capturée ?"), _T("Confirmation"), 
                      MB_YESNO | MB_ICONQUESTION) == IDYES) {
            ShellExecute(NULL, _T("open"), outputFile, NULL, NULL, SW_SHOW);
        }
    }
    catch (COleDispatchException* e) {
        CString errorMsg = _T("Erreur d'arrêt de la capture : ");
        errorMsg += e->m_strDescription;
        MessageBox(errorMsg, _T("Erreur"), MB_ICONERROR);
        e->Delete();
    }
}
```

Implémentation VB6 :

```vb
Private Sub StopCapture()
    On Error GoTo ErrorHandler
    
    ' Arrêter le processus de capture
    VideoCapture1.Stop
    
    ' Mettre à jour l'UI
    btnStart.Enabled = True
    btnStop.Enabled = False
    lblStatus.Caption = "Capture terminée"
    
    ' Ouvrir éventuellement le fichier capturé
    If Dir(VideoCapture1.Output_Filename) <> "" Then
        If MsgBox("Ouvrir la vidéo capturée ?", vbQuestion + vbYesNo) = vbYes Then
            Shell "explorer.exe """ & VideoCapture1.Output_Filename & """", vbNormalFocus
        End If
    End If
    
    Exit Sub
    
ErrorHandler:
    MsgBox "Erreur d'arrêt de la capture : " & Err.Description, vbExclamation
End Sub
```

## Conclusion

L'implémentation de la capture vidéo vers des fichiers AVI dans les applications Delphi à l'aide du composant TVFVideoCapture est un processus simple lorsque vous en comprenez les concepts clés. En suivant ce guide, vous pouvez créer des applications multimédias robustes avec une fonctionnalité de capture vidéo professionnelle.

Le composant TVFVideoCapture offre un large éventail de fonctionnalités supplémentaires et d'options de personnalisation au-delà de ce qui est couvert dans ce guide, notamment des effets vidéo, des superpositions et la configuration des propriétés des périphériques.

N'oubliez pas de tester soigneusement votre implémentation de capture vidéo avec différents codecs et configurations audio pour garantir la meilleure qualité pour votre cas d'usage spécifique.

---
Pour des exemples de code et des conseils d'implémentation supplémentaires, visitez notre dépôt GitHub. Si vous avez besoin d'aide supplémentaire pour ce tutoriel, notre équipe de support est disponible pour vous aider.
