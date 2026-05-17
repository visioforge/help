---
title: Brillo, contraste y saturación de cámara en C# .NET
description: Controla la configuración de cámara, incluyendo brillo, contraste, tono y saturación en .NET con ejemplos de código de Video Capture SDK para ajustes.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - C#
primary_api_classes:
  - VideoCaptureDeviceAdjustValue

---

# Dominar los controles de imagen de cámara en aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introducción a los ajustes de hardware de cámara

Al desarrollar aplicaciones que utilizan webcams u otros dispositivos de captura de video, tener un control preciso sobre los parámetros de calidad de imagen es esencial para crear software de nivel profesional. El enum `VideoHardwareAdjustment` selecciona qué ajuste de hardware vas a leer o escribir, y el SDK expone un control programático sobre los ajustes de cámara como brillo, contraste, tono, saturación, nitidez, gamma y más.

## Propiedades de ajuste de hardware soportadas

El enum `VideoHardwareAdjustment` cubre:

- Brightness
- Contrast
- Hue
- Saturation
- Sharpness
- Gamma
- ColorEnable
- WhiteBalance
- BacklightCompensation
- Gain
- PowerLineFrequency

Exposición, enfoque, zoom y otros controles a nivel de objetivo viven en una familia de API separada (`Video_CaptureDevice_CameraControl_*` + el enum `CameraControlProperty`) — esta página cubre solo ajustes de *imagen* por hardware.

Ten en cuenta que no todas las cámaras soportan cada propiedad. El SDK ignorará elegantemente cualquier propiedad no soportada por el hardware de cámara específico que estés usando.

## Trabajar con ajustes de cámara

### Obtener los rangos de valores de las propiedades

Antes de establecer valores de ajuste, es importante entender el rango válido para cada propiedad. Usa el método `Video_CaptureDevice_VideoAdjust_GetRangesAsync` para obtener los valores mínimo, máximo, predeterminado y el tamaño de paso de cualquier propiedad de ajuste:

```cs
// Obtener el rango válido de valores para la propiedad de brillo
// Esto ayuda a entender los límites dentro de los cuales puedes ajustar la configuración
var brightnessRange = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRangesAsync(VideoHardwareAdjustment.Brightness);

// Comprobar los valores mínimo y máximo permitidos
int minValue = brightnessRange.Min;
int maxValue = brightnessRange.Max;
int defaultValue = brightnessRange.Default;
int step = brightnessRange.Step;
```

### Establecer valores de ajuste

Una vez que conoces el rango válido, puedes establecer un valor específico usando el método `Video_CaptureDevice_VideoAdjust_SetValueAsync`:

```cs
// Crear un objeto de valor de ajuste con los ajustes deseados
// Puedes especificar un valor personalizado y si debe usarse el ajuste automático
var adjustmentValue = new VideoCaptureDeviceAdjustValue(75, false); // Valor: 75, Auto: false

// Aplicar el ajuste de brillo a la cámara
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Brightness, adjustmentValue);
```

### Obtener los valores actuales

Para leer el valor actual de cualquier propiedad de ajuste, usa el método `Video_CaptureDevice_VideoAdjust_GetValueAsync`:

```cs
// Obtener el ajuste actual de brillo de la cámara
// Esto devuelve tanto el valor actual como si el ajuste automático está habilitado
var currentBrightness = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetValueAsync(VideoHardwareAdjustment.Brightness);

// Acceder al valor actual y al indicador auto
int value = currentBrightness.Value;
bool isAuto = currentBrightness.Auto;
```

## Mejores prácticas para los ajustes de cámara

1. **Comprueba siempre los rangos primero**: distintos modelos de cámara tienen distintos rangos válidos para cada propiedad.
2. **Maneja las propiedades no soportadas**: implementa siempre el manejo de errores para las propiedades que podrían no estar soportadas.
3. **Considera guardar las preferencias del usuario**: almacena los valores de ajuste en la configuración de la aplicación para una experiencia coherente.
4. **Proporciona controles de UI**: crea controles deslizantes con valores min/max adecuados según los rangos devueltos.
5. **Considera automático frente a manual**: algunos usuarios pueden preferir el ajuste automático mientras que otros necesitan un control manual preciso.

## Recursos adicionales

Echa un vistazo a nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos de código completos y ejemplos de implementación.
