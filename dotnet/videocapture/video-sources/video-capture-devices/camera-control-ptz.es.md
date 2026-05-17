---
title: Control programático de Pan, Tilt, Zoom de cámara en C# .NET
description: Controla Pan, Tilt, Zoom, Exposición, Iris y Enfoque programáticamente con VisioForge Video Capture SDK. API C# asíncrona con soporte de cámaras hardware.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - C#
  - NuGet

---

# Implementación avanzada de control de cámara y PTZ

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Resumen de las capacidades de control de cámara

La API de control de cámara proporciona a los desarrolladores acceso directo para manipular diversos parámetros de cámara al trabajar con dispositivos compatibles. Dependiendo de las especificaciones de tu hardware de cámara, puedes controlar programáticamente las siguientes características:

- **Pan** — control de movimiento horizontal
- **Tilt** — control de movimiento vertical
- **Roll** — movimiento rotacional a lo largo del eje del objetivo
- **Zoom** — ajuste del nivel de magnificación
- **Exposición** — ajustes de sensibilidad a la luz
- **Iris** — control de apertura para la admisión de luz
- **Enfoque** — ajuste de claridad y nitidez de la imagen

**Nota importante:** la API de control de cámara requiere una sesión activa de vista previa o captura para funcionar correctamente. Debes iniciar la vista previa o captura antes de intentar acceder a las funciones de control.

## Guía de implementación con ejemplos

A continuación encontrarás patrones de implementación prácticos que demuestran cómo integrar las funciones de control de cámara en tus aplicaciones .NET.

### Componentes de la interfaz

Para una interacción óptima con el usuario, considera implementar los siguientes elementos de UI:

- Controles deslizantes para el ajuste de parámetros
- Casillas de verificación para alternar entre modos auto/manual
- Etiquetas para mostrar los valores actuales, mínimos y máximos

Puedes consultar el código fuente del Demo Principal para un ejemplo de implementación completo.

### Paso 1: leer las capacidades de los parámetros de cámara

Primero, consulta la cámara para determinar los rangos soportados y los valores predeterminados de cada parámetro de control:

```cs
// Consultar la cámara por el rango soportado del parámetro Zoom.
// GetRangeAsync devuelve un objeto VideoCaptureDeviceCameraControlRanges
// (o null si el dispositivo no soporta esta propiedad).
var ranges = await VideoCapture1.Video_CaptureDevice_CameraControl_GetRangeAsync(
    CameraControlProperty.Zoom);
if (ranges != null)
{
    // Configurar el control deslizante con el rango soportado por la cámara
    tbCCZoom.Minimum = ranges.Min;
    tbCCZoom.Maximum = ranges.Max;
    tbCCZoom.SmallChange = ranges.Step;
    tbCCZoom.Value = ranges.Default;

    // Actualizar las etiquetas de la UI con la información del rango
    lbCCZoomMin.Text = "Min: " + ranges.Min;
    lbCCZoomMax.Text = "Max: " + ranges.Max;
    lbCCZoomCurrent.Text = "Current: " + ranges.Default;

    // Establecer las casillas del modo de control según las capacidades de la cámara
    cbCCZoomManual.Checked = (ranges.Flags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
    cbCCZoomAuto.Checked = (ranges.Flags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
    cbCCZoomRelative.Checked = (ranges.Flags & CameraControlFlags.Relative) == CameraControlFlags.Relative;
}
```

**Nota técnica:** cuando el indicador Auto está habilitado, la cámara ignorará el resto de los indicadores y los valores manuales. Esto sigue los protocolos estándar de la industria de control de cámara.

### Paso 2: aplicar cambios de parámetros

Cuando los usuarios ajustan la configuración a través de tu interfaz, aplica los cambios a la cámara con este patrón:

```cs
// Inicializar los indicadores de control
CameraControlFlags flags = CameraControlFlags.None;

// Construir los indicadores según el estado de las casillas de la UI
if (cbCCZoomManual.Checked)
{
    // Habilitar el modo de control manual
    flags = flags | CameraControlFlags.Manual;
}

if (cbCCZoomAuto.Checked)
{
    // Habilitar el modo de control automático (anulará la configuración manual)
    flags = flags | CameraControlFlags.Auto;
}

if (cbCCZoomRelative.Checked)
{
    // Habilitar el modo de valor relativo (los cambios son relativos a la posición actual)
    flags = flags | CameraControlFlags.Relative;
}

// Aplicar el nuevo valor de zoom con los indicadores de control especificados.
// SetAsync recibe un VideoCaptureDeviceCameraControlValue que agrupa valor + indicadores.
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Zoom,
    new VideoCaptureDeviceCameraControlValue(tbCCZoom.Value, flags));
```

## Manejo de errores y mejores prácticas

Al implementar funciones de control de cámara, ten en cuenta estas mejores prácticas:

- Comprueba siempre si un parámetro está soportado antes de intentar establecerlo
- Implementa un manejo de errores adecuado para las funciones no soportadas
- Proporciona retroalimentación al usuario cuando un comando falla
- Recuerda que las capacidades de las cámaras varían ampliamente entre fabricantes y modelos

## Dependencias requeridas

Para un funcionamiento correcto, asegúrate de que tu aplicación incluya estos paquetes redistribuibles:

- Redistribuibles de Video Capture:
  - [Arquitectura x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Arquitectura x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Recursos adicionales

Para más ejemplos y detalles completos de implementación, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) con numerosos ejemplos de código y aplicaciones demo.
