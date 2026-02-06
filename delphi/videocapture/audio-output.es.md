---
title: Dispositivo de Salida de Audio en Delphi
description: Seleccionar dispositivos de salida de audio en Delphi - enumerar dispositivos, controlar volumen, ajustar balance con ejemplos de código para Delphi, C++ y VB6.
---

# Selección de Dispositivo de Salida de Audio en Delphi

Esta guía proporciona instrucciones detalladas y ejemplos de código para implementar la selección de dispositivos de salida de audio en sus aplicaciones de captura de video. Se cubren las implementaciones de Delphi, C++ MFC y VB6 para ayudarle a integrar esta funcionalidad en sus proyectos de manera eficiente.

## Enumeración de Dispositivos de Salida de Audio Disponibles

El primer paso para implementar la selección de dispositivos de salida de audio es recuperar la lista completa de dispositivos de salida de audio disponibles en el sistema. Esto permite a los usuarios elegir su dispositivo de salida de audio preferido.

### Implementación en Delphi

```pascal
// Iterar a través de todos los dispositivos de salida de audio disponibles
for i := 0 to VideoCapture1.Audio_OutputDevices_GetCount - 1 do
  // Agregar cada dispositivo a la lista desplegable
  cbAudioOutputDevice.Items.Add(VideoCapture1.Audio_OutputDevices_GetItem(i));
```

### Implementación en C++ MFC

```cpp
// Poblar el combobox con todos los dispositivos de salida de audio disponibles
for (int i = 0; i < m_VideoCapture.Audio_OutputDevices_GetCount(); i++) {
  CString deviceName = m_VideoCapture.Audio_OutputDevices_GetItem(i);
  m_AudioOutputDeviceCombo.AddString(deviceName);
}
```

### Implementación en VB6

```vb
' Iterar a través de todos los dispositivos de salida de audio disponibles
For i = 0 To VideoCapture1.Audio_OutputDevices_GetCount - 1
  ' Agregar cada dispositivo a la lista desplegable
  cboAudioOutputDevice.AddItem VideoCapture1.Audio_OutputDevices_GetItem(i)
Next i
```

## Estableciendo el Dispositivo de Salida de Audio Activo

Después de recuperar los dispositivos disponibles, el siguiente paso es establecer el dispositivo seleccionado como el dispositivo de salida de audio activo para su aplicación.

### Implementación en Delphi

```pascal
// Establecer el dispositivo seleccionado como el dispositivo de salida de audio activo
VideoCapture1.Audio_OutputDevice := cbAudioOutputDevice.Items[cbAudioOutputDevice.ItemIndex];
```

### Implementación en C++ MFC

```cpp
// Obtener el índice seleccionado del combobox
int selectedIndex = m_AudioOutputDeviceCombo.GetCurSel();
CString selectedDevice;
m_AudioOutputDeviceCombo.GetLBText(selectedIndex, selectedDevice);

// Establecer el dispositivo seleccionado como el dispositivo de salida de audio activo
m_VideoCapture.Audio_OutputDevice = selectedDevice;
```

### Implementación en VB6

```vb
' Establecer el dispositivo seleccionado como el dispositivo de salida de audio activo
VideoCapture1.Audio_OutputDevice = cboAudioOutputDevice.Text
```

## Habilitando la Reproducción de Audio

Una vez que el dispositivo de salida está seleccionado, necesita habilitar la reproducción de audio para escuchar el audio a través del dispositivo seleccionado.

### Implementación en Delphi

```pascal
// Habilitar reproducción de audio a través del dispositivo seleccionado
VideoCapture1.Audio_PlayAudio := true;
```

### Implementación en C++ MFC

```cpp
// Habilitar reproducción de audio a través del dispositivo seleccionado
m_VideoCapture.Audio_PlayAudio = TRUE;
```

### Implementación en VB6

```vb
' Habilitar reproducción de audio a través del dispositivo seleccionado
VideoCapture1.Audio_PlayAudio = True
```

## Ajustando Niveles de Volumen de Audio

Proporcionar control de volumen da a los usuarios la capacidad de personalizar su experiencia de audio. Esta sección muestra cómo implementar el ajuste de volumen.

### Implementación en Delphi

```pascal
// Establecer el nivel de volumen basado en la posición del trackbar
VideoCapture1.Audio_OutputDevice_SetVolume(tbAudioVolume.Position);
```

### Implementación en C++ MFC

```cpp
// Obtener la posición actual del control deslizante de volumen
int volumeLevel = m_VolumeSlider.GetPos();

// Establecer el nivel de volumen basado en la posición del control deslizante
m_VideoCapture.Audio_OutputDevice_SetVolume(volumeLevel);
```

### Implementación en VB6

```vb
' Establecer el nivel de volumen basado en la posición del control deslizante
VideoCapture1.Audio_OutputDevice_SetVolume sldVolume.Value
```

## Controlando el Balance de Audio

Para salida estéreo, el control de balance permite a los usuarios ajustar el volumen relativo entre los canales izquierdo y derecho.

### Implementación en Delphi

```pascal
// Establecer el nivel de balance basado en la posición del trackbar
VideoCapture1.Audio_OutputDevice_SetBalance(tbAudioBalance.Position);
```

### Implementación en C++ MFC

```cpp
// Obtener la posición actual del control deslizante de balance
int balanceLevel = m_BalanceSlider.GetPos();

// Establecer el nivel de balance basado en la posición del control deslizante
m_VideoCapture.Audio_OutputDevice_SetBalance(balanceLevel);
```

### Implementación en VB6

```vb
' Establecer el nivel de balance basado en la posición del control deslizante
VideoCapture1.Audio_OutputDevice_SetBalance sldBalance.Value
```

## Mejores Prácticas para Implementación de Dispositivos de Audio

- Siempre verifique si el dispositivo de audio es válido antes de intentar usarlo
- Proporcione mecanismos de respaldo cuando el dispositivo seleccionado no esté disponible
- Considere guardar las preferencias del usuario para la selección de dispositivos de audio entre sesiones
- Implemente retroalimentación visual cuando se cambien los ajustes de volumen o balance

---
Por favor contacte a nuestro [equipo de soporte](https://support.visioforge.com/) si necesita asistencia con esta implementación. Visite nuestro [repositorio de GitHub](https://github.com/visioforge/) para ejemplos de código adicionales y recursos.