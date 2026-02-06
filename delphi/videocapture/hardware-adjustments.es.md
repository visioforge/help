---
title: Ajustes de Video de Cámara para Aplicaciones Delphi
description: Ajustar brillo, contraste y saturación de cámara en Delphi con controles de hardware TVFVideoCapture y ejemplos de configuración de parámetros.
---

# Implementando Ajustes de Video de Hardware en Aplicaciones Delphi

## Resumen

Los dispositivos de captura de video modernos ofrecen potentes ajustes a nivel de hardware que pueden mejorar significativamente la calidad de sus aplicaciones de video. Al aprovechar estas capacidades en sus aplicaciones Delphi, puede proporcionar a los usuarios características de control de video de grado profesional sin procesamiento complejo de imágenes basado en software.

## Tipos de Ajustes Soportados

La mayoría de las webcams y dispositivos de captura de video soportan varios parámetros de ajuste:

- Brillo
- Contraste
- Saturación
- Tono
- Nitidez
- Gamma
- Balance de blancos
- Ganancia

## Recuperando Rangos de Ajuste Disponibles

Antes de establecer ajustes, necesitará determinar qué rangos son soportados por el dispositivo conectado. El método `Video_CaptureDevice_VideoAdjust_GetRanges` proporciona esta información.

### Implementación en Delphi

```pascal
// Recuperar el rango disponible para ajuste de brillo
// Devuelve mínimo, máximo, tamaño de paso, valor predeterminado y capacidad de auto-ajuste
VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges(adj_Brightness, min, max, step, default, auto);
```

### Implementación en C++ MFC

```cpp
// C++ MFC implementación para obtener rangos de ajuste de brillo
// Almacenar resultados en variables enteras para configuración de UI
int min, max, step, default_value;
BOOL auto_value;
m_VideoCapture.Video_CaptureDevice_VideoAdjust_GetRanges(
  VF_VIDEOCAP_ADJ_BRIGHTNESS,
  &min,
  &max,
  &step,
  &default_value,
  &auto_value);
```

### Implementación en VB6

```vb
' VB6 implementación para recuperar parámetros de ajuste de brillo
' Usar estos valores para configurar controles deslizantes y casillas de verificación
Dim min As Integer, max As Integer, step As Integer, default_val As Integer
Dim auto_val As Boolean
VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges adj_Brightness, min, max, step, default_val, auto_val
```

## Estableciendo Valores de Ajuste

Una vez que haya determinado los rangos disponibles, puede usar el método `Video_CaptureDevice_VideoAdjust_SetValue` para aplicar ajustes específicos al stream de video.

### Implementación en Delphi

```pascal
// Establecer el nivel de brillo basado en la posición del trackbar
// El tercer parámetro habilita/deshabilita el ajuste automático de brillo
VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue(
  adj_Brightness, 
  tbAdjBrightness.Position,
  cbAdjBrightnessAuto.Checked);
```

### Implementación en C++ MFC

```cpp
// C++ MFC implementación para establecer valor de brillo
// Usa posición del control deslizante para valor de ajuste manual
// Estado de casilla de verificación determina si el auto-ajuste está habilitado
m_VideoCapture.Video_CaptureDevice_VideoAdjust_SetValue(
  VF_VIDEOCAP_ADJ_BRIGHTNESS,
  m_sliderBrightness.GetPos(),
  m_checkBrightnessAuto.GetCheck() == BST_CHECKED);
```

### Implementación en VB6

```vb
' VB6 implementación para aplicar ajustes de brillo
' Usa valor del trackbar para nivel de ajuste
' Valor de casilla de verificación determina modo de ajuste automático
VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue _
  adj_Brightness, _
  tbAdjBrightness.Value, _
  cbAdjBrightnessAuto.Value = vbChecked
```

## Mejores Prácticas para Implementación de Ajustes de Video

Al implementar ajustes de video en sus aplicaciones:

1. Siempre verifique las capacidades del dispositivo primero, ya que no todos los dispositivos soportan todos los tipos de ajuste
2. Proporcione controles de UI intuitivos como controles deslizantes con valores mín/máx apropiados
3. Incluya opciones de auto-ajuste cuando estén disponibles
4. Considere guardar las preferencias del usuario para sesiones futuras
5. Implemente vista previa en tiempo real para que los usuarios puedan ver los efectos de sus ajustes

## Recursos Adicionales

Por favor contacte a nuestro equipo de soporte para asistencia con la implementación de estas características en su aplicación. Visite nuestro repositorio de GitHub para ejemplos de código adicionales y ejemplos de implementación.
