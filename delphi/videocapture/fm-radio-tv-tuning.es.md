---
title: Guía de Implementación de Radio FM y Sintonizador de TV en Delphi
description: Sintonización de radio FM y TV en Delphi: escaneo de canales, gestión de frecuencias y ejemplos de código para Delphi, C++, VB6.
---

# Implementando Sintonización de Radio FM y TV en Aplicaciones Delphi

## Introducción a la Sintonización de TV y Radio

Esta guía proporciona ejemplos detallados de implementación para desarrolladores Delphi que trabajan con funcionalidad de sintonización de radio FM y TV. Hemos incluido ejemplos de código equivalentes para C++ MFC y VB6 para soportar necesidades de desarrollo multiplataforma.

## Gestión de Dispositivos

### Recuperando Sintonizadores de TV Disponibles

El primer paso en implementar funcionalidad de sintonizador es identificar los dispositivos de hardware disponibles:

```pascal
// Iterar a través de todos los dispositivos de Sintonizador de TV conectados y poblar el desplegable
for I := 0 to VideoCapture1.TVTuner_Devices_GetCount - 1 do
  cbTVTuner.Items.Add(VideoCapture1.TVTuner_Devices_GetItem(i));
```

```cpp
// C++ MFC implementación para recuperar dispositivos de Sintonizador de TV
for (int i = 0; i < m_VideoCapture.TVTuner_Devices_GetCount(); i++)
  m_cbTVTuner.AddString(m_VideoCapture.TVTuner_Devices_GetItem(i));
```

```vb
' VB6 implementación para enumeración de dispositivos
For i = 0 To VideoCapture1.TVTuner_Devices_GetCount - 1
  cbTVTuner.AddItem VideoCapture1.TVTuner_Devices_GetItem(i)
Next i
```

### Enumerando Soporte de Formatos de TV

Diferentes regiones usan diferentes estándares de transmisión. Su aplicación debe detectar y manejar estos formatos:

```pascal
// Cargar formatos de TV disponibles (PAL, NTSC, SECAM, etc.)
for I := 0 to VideoCapture1.TVTuner_TVFormats_GetCount - 1 do
  cbTVSystem.Items.Add(VideoCapture1.TVTuner_TVFormats_GetItem(i));
```

```cpp
// C++ MFC - Poblar desplegable de formato de TV con estándares disponibles
for (int i = 0; i < m_VideoCapture.TVTuner_TVFormats_GetCount(); i++)
  m_cbTVSystem.AddString(m_VideoCapture.TVTuner_TVFormats_GetItem(i));
```

```vb
' VB6 - Obtener formatos de TV soportados para el sintonizador seleccionado
For i = 0 To VideoCapture1.TVTuner_TVFormats_GetCount - 1
  cbTVSystem.AddItem VideoCapture1.TVTuner_TVFormats_GetItem(i)
Next i
```

### Configuración Específica por País

Los estándares de transmisión varían por país, por lo que su aplicación debe proporcionar selección de región apropiada:

```pascal
// Cargar lista de países/regiones para parámetros de sintonización localizados
for I := 0 to VideoCapture1.TVTuner_Countries_GetCount - 1 do
  cbTVCountry.Items.Add(VideoCapture1.TVTuner_Countries_GetItem(i));
```

```cpp
// C++ MFC - Construir lista de selección de países para ajustes regionales
for (int i = 0; i < m_VideoCapture.TVTuner_Countries_GetCount(); i++)
  m_cbTVCountry.AddString(m_VideoCapture.TVTuner_Countries_GetItem(i));
```

```vb
' VB6 - Poblar desplegable de países para ajustes de transmisión regional
For i = 0 To VideoCapture1.TVTuner_Countries_GetCount - 1
  cbTVCountry.AddItem VideoCapture1.TVTuner_Countries_GetItem(i)
Next i
```

## Configuración de Dispositivos

### Seleccionando un Dispositivo Sintonizador de TV

Una vez que haya enumerado los dispositivos disponibles, los usuarios pueden seleccionar su sintonizador preferido:

```pascal
// Establecer el dispositivo sintonizador activo basado en la selección del usuario
VideoCapture1.TVTuner_Name := cbTVTuner.Items[cbTVTuner.ItemIndex];
```

```cpp
// C++ MFC - Aplicar la selección del dispositivo sintonizador del usuario
CString strText;
m_cbTVTuner.GetLBText(m_cbTVTuner.GetCurSel(), strText);
m_VideoCapture.put_TVTuner_Name(strText);
```

```vb
' VB6 - Establecer sintonizador seleccionado como dispositivo activo
VideoCapture1.TVTuner_Name = cbTVTuner.Text
```

### Leyendo la Configuración Actual del Sintonizador

Después de seleccionar un dispositivo, necesitará leer sus ajustes actuales:

```pascal
// Inicializar sintonizador y leer configuración actual
VideoCapture1.TVTuner_Read;
```

```cpp
// C++ MFC - Cargar ajustes actuales del sintonizador en la aplicación
m_VideoCapture.TVTuner_Read();
```

```vb
' VB6 - Leer configuración del sintonizador después de selección de dispositivo
VideoCapture1.TVTuner_Read
```

### Modos de Operación Disponibles

Los sintonizadores soportan diferentes modos como TV, Radio FM, etc:

```pascal
// Poblar desplegable de modo de operación con opciones disponibles
for I := 0 to VideoCapture1.TVTuner_Modes_GetCount - 1 do
  cbTVMode.Items.Add(VideoCapture1.TVTuner_Modes_GetItem(i));
```

```cpp
// C++ MFC - Obtener modos operacionales soportados para este dispositivo
for (int i = 0; i < m_VideoCapture.TVTuner_Modes_GetCount(); i++)
  m_cbTVMode.AddString(m_VideoCapture.TVTuner_Modes_GetItem(i));
```

```vb
' VB6 - Listar modos de sintonizador disponibles (TV, Radio FM, etc)
For i = 0 To VideoCapture1.TVTuner_Modes_GetCount - 1
  cbTVMode.AddItem VideoCapture1.TVTuner_Modes_GetItem(i)
Next i
```

## Gestión de Frecuencias

### Leyendo Frecuencias Actuales

Mostrar las frecuencias de audio y video actuales para proporcionar retroalimentación al usuario:

```pascal
// Mostrar frecuencias de video y audio actuales en Hz
edVideoFreq.Text := IntToStr(VideoCapture1.TVTuner_VideoFrequency);
edAudiofreq.Text := IntToStr(VideoCapture1.TVTuner_AudioFrequency);
```

```cpp
// C++ MFC - Mostrar valores de frecuencia actuales en la interfaz
CString strFreq;
strFreq.Format(_T("%d"), m_VideoCapture.get_TVTuner_VideoFrequency());
m_edVideoFreq.SetWindowText(strFreq);
strFreq.Format(_T("%d"), m_VideoCapture.get_TVTuner_AudioFrequency());
m_edAudioFreq.SetWindowText(strFreq);
```

```vb
' VB6 - Actualizar campos de visualización de frecuencia con valores actuales
edVideoFreq.Text = CStr(VideoCapture1.TVTuner_VideoFrequency)
edAudioFreq.Text = CStr(VideoCapture1.TVTuner_AudioFrequency)
```

## Configuración de Entrada y Modo

### Estableciendo Fuente de Señal de Entrada

Los sintonizadores pueden soportar múltiples fuentes de entrada que deben ser configurables:

```pascal
// Seleccionar la fuente de entrada apropiada basada en la configuración actual
cbTVInput.ItemIndex := cbTVInput.Items.IndexOf(VideoCapture1.TVTuner_InputType);
```

```cpp
// C++ MFC - Actualizar selección de fuente de entrada en UI
CString strInputType = m_VideoCapture.get_TVTuner_InputType();
m_cbTVInput.SetCurSel(m_cbTVInput.FindStringExact(-1, strInputType));
```

```vb
' VB6 - Establecer desplegable de fuente de entrada para coincidir con configuración actual
cbTVInput.ListIndex = cbTVInput.FindItem(VideoCapture1.TVTuner_InputType)
```

### Configurando Modo de Operación

Diferentes modos de sintonizador requieren ajustes de UI y parámetros específicos:

```pascal
// Establecer desplegable de modo de operación al modo actual (TV, Radio FM, etc)
cbTVMode.ItemIndex := cbTVMode.Items.IndexOf(VideoCapture1.TVTuner_Mode);
```

```cpp
// C++ MFC - Actualizar selector de modo para coincidir con configuración actual del sintonizador
CString strMode = m_VideoCapture.get_TVTuner_Mode();
m_cbTVMode.SetCurSel(m_cbTVMode.FindStringExact(-1, strMode));
```

```vb
' VB6 - Seleccionar modo de operación actual en desplegable
cbTVMode.ListIndex = cbTVMode.FindItem(VideoCapture1.TVTuner_Mode)
```

### Configuración de Formato de TV

Establecer el estándar de transmisión apropiado para la región:

```pascal
// Configurar el estándar de TV apropiado (PAL, NTSC, SECAM, etc)
cbTVSystem.ItemIndex := cbTVSystem.Items.IndexOf(VideoCapture1.TVTuner_TVFormat);
```

```cpp
// C++ MFC - Establecer desplegable de formato de TV al estándar de transmisión actual
CString strTVFormat = m_VideoCapture.get_TVTuner_TVFormat();
m_cbTVSystem.SetCurSel(m_cbTVSystem.FindStringExact(-1, strTVFormat));
```

```vb
' VB6 - Actualizar selección de formato de sistema de TV
cbTVSystem.ListIndex = cbTVSystem.FindItem(VideoCapture1.TVTuner_TVFormat)
```

### Ajustes Regionales

Configurar parámetros de transmisión específicos de la región:

```pascal
// Establecer país/región para tablas de frecuencia y estándares apropiados
cbTVCountry.ItemIndex := cbTVCountry.Items.IndexOf(VideoCapture1.TVTuner_Country);
```

```cpp
// C++ MFC - Actualizar selección de país para coincidir con ajuste actual
CString strCountry = m_VideoCapture.get_TVTuner_Country();
m_cbTVCountry.SetCurSel(m_cbTVCountry.FindStringExact(-1, strCountry));
```

```vb
' VB6 - Establecer desplegable de país al ajuste regional actual
cbTVCountry.ListIndex = cbTVCountry.FindItem(VideoCapture1.TVTuner_Country)
```

## Escaneo de Canales

### Manejando Eventos de Escaneo de Canales

Implementar el manejador de eventos para el proceso de escaneo de canales:

```pascal
// Manejador de eventos para proceso de escaneo de canales
// Rastrea progreso y recopila canales encontrados
procedure TForm1.VideoCapture1TVTunerTuneChannels(SignalPresent: Boolean; Channel, Frequency, Progress: Integer);
begin
  // Actualizar barra de progreso con progreso de escaneo actual
  pbChannels.Position := Progress;

  // Agregar canal a lista si se detecta señal
  if SignalPresent then
    cbTVChannel.Items.Add(IntToStr(Channel));

  // Escaneo completo cuando Channel = -1
  if Channel = -1 then
    begin
      pbChannels.Position := 0;
      ShowMessage('AutoTune completo');
    end;
end;
```

```cpp
// C++ MFC - Implementación de manejador de eventos de escaneo de canales
// En archivo de cabecera (.h)
BEGIN_EVENTSINK_MAP(CMainDlg, CDialog)
    ON_EVENT(CMainDlg, IDC_VIDEOCAPTURE, 1, OnTVTunerTuneChannels, VTS_BOOL VTS_I4 VTS_I4 VTS_I4)
END_EVENTSINK_MAP()

// En archivo de implementación (.cpp)
void CMainDlg::OnTVTunerTuneChannels(BOOL SignalPresent, long Channel, long Frequency, long Progress)
{
    // Actualizar indicador de progreso de escaneo
    m_pbChannels.SetPos(Progress);
    
    // Agregar canales encontrados a la lista de selección
    if (SignalPresent)
    {
        CString strChannel;
        strChannel.Format(_T("%d"), Channel);
        m_cbTVChannel.AddString(strChannel);
    }
    
    // Manejar finalización de escaneo
    if (Channel == -1)
    {
        m_pbChannels.SetPos(0);
        MessageBox(_T("AutoTune completo"), _T("Información"), MB_OK | MB_ICONINFORMATION);
    }
}
```

```vb
' VB6 - Manejador de eventos de escaneo de canales
Private Sub VideoCapture1_TVTunerTuneChannels(ByVal SignalPresent As Boolean, ByVal Channel As Long, ByVal Frequency As Long, ByVal Progress As Long)
    ' Actualizar visualización de progreso de escaneo
    pbChannels.Value = Progress
    
    ' Agregar canal a lista cuando se encuentra señal
    If SignalPresent Then
        cbTVChannel.AddItem CStr(Channel)
    End If
    
    ' Manejar finalización de escaneo
    If Channel = -1 Then
        pbChannels.Value = 0
        MsgBox "AutoTune completo", vbInformation
    End If
End Sub
```

### Iniciando Escaneo de Canales

Iniciar el proceso de escaneo automático de canales:

```pascal
// Definir constantes de frecuencia para claridad
const KHz = 1000;
const MHz = 1000000;

// Inicializar sintonizador con ajustes actuales
VideoCapture1.TVTuner_Read;
// Limpiar lista de canales anterior
cbTVChannel.Items.Clear;

// Configurar parámetros especiales para escaneo de Radio FM
if ( (cbTVMode.ItemIndex <> -1) and (cbTVMode.Items[cbTVMode.ItemIndex] = 'FM Radio') ) then
  begin
    // Establecer rango de frecuencia para escaneo FM (100-110MHz)
    VideoCapture1.TVTuner_FM_Tuning_StartFrequency := 100 * Mhz;
    VideoCapture1.TVTuner_FM_Tuning_StopFrequency := 110 * MHz;
    // Establecer incrementos de 100KHz para escaneo FM
    VideoCapture1.TVTuner_FM_Tuning_Step := 100 * KHz;
  end;

// Iniciar escaneo automático de canales
VideoCapture1.TVTuner_TuneChannels_Start;
```

```cpp
// C++ MFC - Iniciar escaneo de canales con parámetros apropiados
const int KHz = 1000;
const int MHz = 1000000;

// Actualizar configuración del sintonizador
m_VideoCapture.TVTuner_Read();
// Restablecer lista de canales antes de escanear
m_cbTVChannel.ResetContent();

// Configurar parámetros específicos de FM si está en modo radio
CString strMode;
m_cbTVMode.GetLBText(m_cbTVMode.GetCurSel(), strMode);
if (strMode == _T("FM Radio"))
{
    // Establecer rango de escaneo FM (100-110MHz)
    m_VideoCapture.put_TVTuner_FM_Tuning_StartFrequency(100 * MHz);
    m_VideoCapture.put_TVTuner_FM_Tuning_StopFrequency(110 * MHz);
    // Usar pasos de 100KHz para escaneo FM
    m_VideoCapture.put_TVTuner_FM_Tuning_Step(100 * KHz);
}

// Iniciar el proceso de escaneo de canales
m_VideoCapture.TVTuner_TuneChannels_Start();
```

```vb
' VB6 - Iniciar proceso de escaneo de canales
Const KHz = 1000
Const MHz = 1000000

' Leer configuración actual del sintonizador
VideoCapture1.TVTuner_Read
' Limpiar lista de canales existente
cbTVChannel.Clear

' Configuración especial para escaneo de Radio FM
If (cbTVMode.ListIndex <> -1) And (cbTVMode.Text = "FM Radio") Then
    ' Establecer parámetros de escaneo de banda FM (100-110MHz)
    VideoCapture1.TVTuner_FM_Tuning_StartFrequency = 100 * MHz
    VideoCapture1.TVTuner_FM_Tuning_StopFrequency = 110 * MHz
    ' Usar tamaño de paso de 100KHz para escaneo FM
    VideoCapture1.TVTuner_FM_Tuning_Step = 100 * KHz
End If

' Iniciar escaneo automático de canales
VideoCapture1.TVTuner_TuneChannels_Start
```

## Operaciones de Sintonización Manual

### Estableciendo Canal por Número

Permitir selección directa de canal por número:

```pascal
// Cambiar al número de canal especificado
VideoCapture1.TVTuner_Channel := StrToInt(edChannel.Text);
// Aplicar cambios de sintonización
VideoCapture1.TVTuner_Apply;
```

```cpp
// C++ MFC - Establecer sintonizador al número de canal especificado
CString strChannel;
m_edChannel.GetWindowText(strChannel);
m_VideoCapture.put_TVTuner_Channel(_ttoi(strChannel));
m_VideoCapture.TVTuner_Apply();
```

```vb
' VB6 - Sintonizar a número de canal específico
VideoCapture1.TVTuner_Channel = CInt(edChannel.Text)
VideoCapture1.TVTuner_Apply
```

### Estableciendo Frecuencia de Radio Directamente

Para radio FM, la sintonización directa de frecuencia es a menudo requerida:

```pascal
// Establecer canal a -1 para sintonización basada en frecuencia
VideoCapture1.TVTuner_Channel := -1; // debe ser -1 para usar frecuencia
// Establecer frecuencia específica desde campo de entrada
VideoCapture1.TVTuner_Frequency := StrToInt(edChannel.Text);
// Aplicar cambio de frecuencia
VideoCapture1.TVTuner_Apply;
```

```cpp
// C++ MFC - Implementación de sintonización directa de frecuencia
CString strFrequency;
m_edChannel.GetWindowText(strFrequency);
// Establecer canal a -1 para habilitar sintonización basada en frecuencia
m_VideoCapture.put_TVTuner_Channel(-1); // debe ser -1 para usar frecuencia
// Aplicar la frecuencia especificada
m_VideoCapture.put_TVTuner_Frequency(_ttoi(strFrequency));
m_VideoCapture.TVTuner_Apply();
```

```vb
' VB6 - Sintonización manual de frecuencia para radio
VideoCapture1.TVTuner_Channel = -1 ' debe ser -1 para usar frecuencia
VideoCapture1.TVTuner_Frequency = CInt(edChannel.Text)
VideoCapture1.TVTuner_Apply
```

## Conclusión

Esta guía cubre los aspectos esenciales de implementar funcionalidad de sintonización de radio FM y TV en sus aplicaciones Delphi. Siguiendo estos ejemplos, puede crear interfaces de sintonización robustas con escaneo de canales apropiado, gestión de frecuencias y detección de señal.

Para una integración óptima en sus proyectos, recuerde manejar condiciones de error y proporcionar retroalimentación apropiada al usuario durante operaciones prolongadas como el escaneo de canales.

---
Por favor visite nuestra página de [GitHub](https://github.com/visioforge/) para ejemplos de código adicionales y ejemplos de implementación.