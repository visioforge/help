---
title: Captura de Video a AVI en Delphi
description: Grabar video en formato AVI en Delphi con TVFVideoCapture usando selección de códec, configuración de audio y ejemplos completos de implementación.
---

# Guía Completa de Captura de Video a Archivos AVI en Delphi

Al desarrollar aplicaciones multimedia en Delphi, la funcionalidad de captura de video es a menudo un requisito crítico. Esta guía explora cómo implementar captura de video de alta calidad a archivos AVI usando el componente TVFVideoCapture en aplicaciones Delphi. Cubriremos todo desde la configuración de códecs hasta la configuración de parámetros de audio y el inicio del proceso de captura.

## Entendiendo la Captura de Video AVI en Delphi

El componente TVFVideoCapture proporciona una forma potente y flexible de capturar video directamente a formato AVI en aplicaciones Delphi. AVI (Audio Video Interleave) sigue siendo un formato contenedor de video popular debido a su amplia compatibilidad y confiabilidad para propósitos de grabación.

Al implementar la captura de video en su aplicación Delphi, necesitará considerar varios aspectos clave:

1. Seleccionar códecs de video y audio apropiados
2. Configurar parámetros de audio
3. Establecer el formato de salida y modo de captura
4. Gestionar el proceso de captura

Esta guía proporciona explicaciones detalladas y ejemplos de código para cada uno de estos pasos.

## Trabajando con Códecs de Video y Audio

### Recuperando Códecs Disponibles

Antes de capturar video, necesitará poblar su aplicación con los códecs de video y audio disponibles. El componente TVFVideoCapture hace esto sencillo:

```pascal
procedure TMyForm.PopulateCodecLists;
var
  I: Integer;
begin
  // Limpiar elementos existentes
  cbVideoCodecs.Items.Clear;
  cbAudioCodecs.Items.Clear;
  
  // Poblar códecs de video
  for I := 0 to VideoCapture1.Video_Codecs_GetCount - 1 do
    cbVideoCodecs.Items.Add(VideoCapture1.Video_Codecs_GetItem(i));
    
  // Poblar códecs de audio
  for I := 0 to VideoCapture1.Audio_Codecs_GetCount - 1 do
    cbAudioCodecs.Items.Add(VideoCapture1.Audio_Codecs_GetItem(i));
end;
```

Para desarrolladores usando C++ MFC, el código equivalente sería:

```cpp
void CMyDialog::PopulateCodecLists()
{
    // Limpiar elementos existentes
    m_VideoCodecsCombo.ResetContent();
    m_AudioCodecsCombo.ResetContent();
    
    // Poblar códecs de video
    for (int i = 0; i < m_VideoCapture.Video_Codecs_GetCount(); i++) {
        CString codecName = m_VideoCapture.Video_Codecs_GetItem(i);
        m_VideoCodecsCombo.AddString(codecName);
    }
    
    // Poblar códecs de audio
    for (int i = 0; i < m_VideoCapture.Audio_Codecs_GetCount(); i++) {
        CString codecName = m_VideoCapture.Audio_Codecs_GetItem(i);
        m_AudioCodecsCombo.AddString(codecName);
    }
}
```

Para desarrolladores VB6, aquí está cómo implementar la misma funcionalidad:

```vb
Private Sub PopulateCodecLists()
    ' Limpiar elementos existentes
    cboVideoCodecs.Clear
    cboAudioCodecs.Clear
    
    ' Poblar códecs de video
    Dim i As Integer
    For i = 0 To VideoCapture1.Video_Codecs_GetCount - 1
        cboVideoCodecs.AddItem VideoCapture1.Video_Codecs_GetItem(i)
    Next i
    
    ' Poblar códecs de audio
    For i = 0 To VideoCapture1.Audio_Codecs_GetCount - 1
        cboAudioCodecs.AddItem VideoCapture1.Audio_Codecs_GetItem(i)
    Next i
End Sub
```

### Seleccionando Códecs para Captura

Una vez que haya poblado las listas, necesitará permitir a los usuarios seleccionar sus códecs preferidos y aplicar esas selecciones al componente de captura:

```pascal
procedure TMyForm.ApplyCodecSelections;
begin
  if cbVideoCodecs.ItemIndex >= 0 then
    VideoCapture1.Video_Codec := cbVideoCodecs.Items[cbVideoCodecs.ItemIndex];
    
  if cbAudioCodecs.ItemIndex >= 0 then
    VideoCapture1.Audio_Codec := cbAudioCodecs.Items[cbAudioCodecs.ItemIndex];
end;
```

Implementación C++ MFC:

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

Implementación VB6:

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

## Configurando Parámetros de Audio

La captura de audio de calidad requiere una configuración adecuada de tres parámetros clave:

1. **Canales de Audio**: Típicamente 1 (mono) o 2 (estéreo)
2. **Bits Por Muestra (BPS)**: Los valores comunes incluyen 8, 16, o 24 bits
3. **Tasa de Muestreo**: Las tasas estándar incluyen 44100 Hz (calidad CD) o 48000 Hz

Aquí está cómo aplicar estos ajustes en Delphi:

```pascal
procedure TMyForm.ConfigureAudioSettings;
begin
  // Aplicar configuración de canales de audio (mono/estéreo)
  VideoCapture1.Audio_Channels := StrToInt(cbChannels.Items[cbChannels.ItemIndex]);
  
  // Establecer bits por muestra para calidad de audio
  VideoCapture1.Audio_BPS := StrToInt(cbBPS.Items[cbBPS.ItemIndex]);
  
  // Configurar tasa de muestreo
  VideoCapture1.Audio_SampleRate := StrToInt(cbSampleRate.Items[cbSampleRate.ItemIndex]);
end;
```

Implementación C++ MFC:

```cpp
void CMyDialog::ConfigureAudioSettings()
{
    CString channelStr, bpsStr, sampleRateStr;
    
    // Obtener valores seleccionados de los combo boxes
    m_ChannelsCombo.GetLBText(m_ChannelsCombo.GetCurSel(), channelStr);
    m_BpsCombo.GetLBText(m_BpsCombo.GetCurSel(), bpsStr);
    m_SampleRateCombo.GetLBText(m_SampleRateCombo.GetCurSel(), sampleRateStr);
    
    // Aplicar configuración de canales de audio
    m_VideoCapture.Audio_Channels = _ttoi(channelStr);
    
    // Establecer bits por muestra
    m_VideoCapture.Audio_BPS = _ttoi(bpsStr);
    
    // Configurar tasa de muestreo
    m_VideoCapture.Audio_SampleRate = _ttoi(sampleRateStr);
}
```

Implementación VB6:

```vb
Private Sub ConfigureAudioSettings()
    ' Aplicar configuración de canales de audio
    VideoCapture1.Audio_Channels = CInt(cboChannels.Text)
    
    ' Establecer bits por muestra
    VideoCapture1.Audio_BPS = CInt(cboBPS.Text)
    
    ' Configurar tasa de muestreo
    VideoCapture1.Audio_SampleRate = CInt(cboSampleRate.Text)
End Sub
```

## Configurando Formato de Salida y Modo de Captura

El siguiente paso es configurar el formato de salida como AVI y establecer el modo de captura apropiado:

```pascal
procedure TMyForm.PrepareForCapture;
begin
  // Establecer AVI como formato de salida
  VideoCapture1.OutputFormat := Format_AVI;
  
  // Configurar modo de captura de video
  VideoCapture1.Mode := Mode_Video_Capture;
end;
```

Implementación C++ MFC:

```cpp
void CMyDialog::PrepareForCapture()
{
    // Establecer AVI como formato de salida
    m_VideoCapture.OutputFormat = Format_AVI;
    
    // Configurar modo de captura de video
    m_VideoCapture.Mode = Mode_Video_Capture;
}
```

Implementación VB6:

```vb
Private Sub PrepareForCapture()
    ' Establecer AVI como formato de salida
    VideoCapture1.OutputFormat = Format_AVI
    
    ' Configurar modo de captura de video
    VideoCapture1.Mode = Mode_Video_Capture
End Sub
```

## Iniciando y Gestionando el Proceso de Captura

Una vez que todo está configurado, puede iniciar el proceso de captura:

```pascal
procedure TMyForm.StartCapture;
begin
  try
    // Establecer nombre de archivo de salida
    VideoCapture1.Output := ExtractFilePath(Application.ExeName) + 'VideoCapturado.avi';
    
    // Iniciar proceso de captura
    VideoCapture1.Start;
    
    // Actualizar UI para mostrar captura en progreso
    btnStart.Enabled := False;
    btnStop.Enabled := True;
    lblStatus.Caption := 'Grabando...';
  except
    on E: Exception do
      ShowMessage('Error al iniciar captura: ' + E.Message);
  end;
end;
```

Implementación C++ MFC:

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
        
        // Establecer nombre de archivo de salida
        m_VideoCapture.Output = appDir + _T("VideoCapturado.avi");
        
        // Iniciar proceso de captura
        m_VideoCapture.Start();
        
        // Actualizar UI
        GetDlgItem(IDC_START_BUTTON)->EnableWindow(FALSE);
        GetDlgItem(IDC_STOP_BUTTON)->EnableWindow(TRUE);
        SetDlgItemText(IDC_STATUS_STATIC, _T("Grabando..."));
    }
    catch (COleDispatchException* e) {
        CString errorMsg = _T("Error al iniciar captura: ");
        errorMsg += e->m_strDescription;
        MessageBox(errorMsg, _T("Error"), MB_ICONERROR);
        e->Delete();
    }
}
```

Implementación VB6:

```vb
Private Sub StartCapture()
    On Error GoTo ErrorHandler
    
    ' Establecer nombre de archivo de salida
    VideoCapture1.Output = App.Path & "\VideoCapturado.avi"
    
    ' Iniciar proceso de captura
    VideoCapture1.Start
    
    ' Actualizar UI
    btnStart.Enabled = False
    btnStop.Enabled = True
    lblStatus.Caption = "Grabando..."
    
    Exit Sub
    
ErrorHandler:
    MsgBox "Error al iniciar captura: " & Err.Description, vbExclamation
End Sub
```

## Manejando la Finalización de la Captura

Es importante proporcionar funcionalidad para detener el proceso de captura:

```pascal
procedure TMyForm.StopCapture;
begin
  try
    // Detener el proceso de captura
    VideoCapture1.Stop;
    
    // Actualizar UI
    btnStart.Enabled := True;
    btnStop.Enabled := False;
    lblStatus.Caption := 'Captura completada';
    
    // Opcionalmente abrir el archivo capturado
    if FileExists(VideoCapture1.Output) and (MessageDlg('¿Abrir video capturado?', 
                                                       mtConfirmation, [mbYes, mbNo], 0) = mrYes) then
      ShellExecute(0, 'open', PChar(VideoCapture1.Output), nil, nil, SW_SHOW);
  except
    on E: Exception do
      ShowMessage('Error al detener captura: ' + E.Message);
  end;
end;
```

Implementación C++ MFC:

```cpp
void CMyDialog::StopCapture()
{
    try {
        // Detener el proceso de captura
        m_VideoCapture.Stop();
        
        // Actualizar UI
        GetDlgItem(IDC_START_BUTTON)->EnableWindow(TRUE);
        GetDlgItem(IDC_STOP_BUTTON)->EnableWindow(FALSE);
        SetDlgItemText(IDC_STATUS_STATIC, _T("Captura completada"));
        
        // Opcionalmente abrir el archivo capturado
        CString outputFile = m_VideoCapture.Output;
        if (PathFileExists(outputFile) && 
            MessageBox(_T("¿Abrir video capturado?"), _T("Confirmación"), 
                      MB_YESNO | MB_ICONQUESTION) == IDYES) {
            ShellExecute(NULL, _T("open"), outputFile, NULL, NULL, SW_SHOW);
        }
    }
    catch (COleDispatchException* e) {
        CString errorMsg = _T("Error al detener captura: ");
        errorMsg += e->m_strDescription;
        MessageBox(errorMsg, _T("Error"), MB_ICONERROR);
        e->Delete();
    }
}
```

Implementación VB6:

```vb
Private Sub StopCapture()
    On Error GoTo ErrorHandler
    
    ' Detener el proceso de captura
    VideoCapture1.Stop
    
    ' Actualizar UI
    btnStart.Enabled = True
    btnStop.Enabled = False
    lblStatus.Caption = "Captura completada"
    
    ' Opcionalmente abrir el archivo capturado
    If Dir(VideoCapture1.Output) <> "" Then
        If MsgBox("¿Abrir video capturado?", vbQuestion + vbYesNo) = vbYes Then
            Shell "explorer.exe """ & VideoCapture1.Output & """", vbNormalFocus
        End If
    End If
    
    Exit Sub
    
ErrorHandler:
    MsgBox "Error al detener captura: " & Err.Description, vbExclamation
End Sub
```

## Conclusión

Implementar captura de video a archivos AVI en aplicaciones Delphi usando el componente TVFVideoCapture es un proceso directo cuando entiende los conceptos clave. Siguiendo esta guía, puede crear aplicaciones multimedia robustas con funcionalidad de captura de video profesional.

El componente TVFVideoCapture proporciona una amplia gama de características adicionales y opciones de personalización más allá de lo cubierto en esta guía, incluyendo efectos de video, superposiciones y configuración de propiedades de dispositivo.

Recuerde probar exhaustivamente su implementación de captura de video con diferentes códecs y configuraciones de audio para asegurar la mejor calidad para su caso de uso específico.

---
Para ejemplos de código adicionales y guía de implementación, visite nuestro repositorio de GitHub. Si necesita más asistencia con este tutorial, nuestro equipo de soporte está disponible para ayudar.