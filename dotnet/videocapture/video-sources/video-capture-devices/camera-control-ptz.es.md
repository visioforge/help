---
title: Control Avanzado de Cámara y PTZ para SDK .NET
description: Implementa características de control de cámara incluyendo Pan, Tilt, Zoom (PTZ), Exposición, Iris y Enfoque en .NET con ejemplos de código C#.
---

# Implementación Avanzada de Control de Cámara y PTZ

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Descripción General de Capacidades de Control de Cámara

La API de Control de Cámara proporciona a los desarrolladores acceso directo para manipular varios parámetros de cámara cuando trabajan con dispositivos compatibles. Dependiendo de las especificaciones de tu hardware de cámara, puedes controlar programáticamente las siguientes características:

- **Pan** - Control de movimiento horizontal
- **Tilt** - Control de movimiento vertical
- **Roll** - Movimiento rotacional a lo largo del eje del lente
- **Zoom** - Ajuste de nivel de magnificación
- **Exposición** - Configuraciones de sensibilidad a la luz
- **Iris** - Control de apertura para admisión de luz
- **Enfoque** - Ajuste de claridad y nitidez de imagen

**Nota Importante:** La API de Control de Cámara requiere una sesión activa de vista previa o captura para funcionar apropiadamente. Debes iniciar la vista previa o captura antes de intentar acceder a las características de control.

## Guía de Implementación con Ejemplos

A continuación encontrarás patrones de implementación prácticos que demuestran cómo integrar características de control de cámara en tus aplicaciones .NET.

### Componentes de Interfaz

Para interacción óptima del usuario, considera implementar los siguientes elementos de UI:

- Controles deslizantes para ajuste de parámetros
- Casillas de verificación para alternar modos auto/manual
- Etiquetas para mostrar valores actuales, mínimos y máximos

Puedes referenciar el código fuente del Demo Principal para un ejemplo de implementación completo.

### Paso 1: Leer Capacidades de Parámetros de Cámara

Primero, consulta la cámara para determinar los rangos soportados y valores predeterminados para cada parámetro de control:

```cs
// Inicializar variables para almacenar información de parámetros de cámara
int max;
int defaultValue;
int min;
int step;
CameraControlFlags flags;

// Consultar la cámara por el rango soportado del parámetro Zoom
if (await VideoCapture1.Video_CaptureDevice_CameraControl_GetRangeAsync(
    CameraControlProperty.Zoom, out min, out max, out step, out defaultValue, out flags))
{
    // Configurar control deslizante con el rango soportado de la cámara
    tbCCZoom.Minimum = min;
    tbCCZoom.Maximum = max;
    tbCCZoom.SmallChange = step;
    tbCCZoom.Value = defaultValue;

    // Actualizar etiquetas de UI con información de rango
    lbCCZoomMin.Text = "Min: " + Convert.ToString(min);
    lbCCZoomMax.Text = "Max: " + Convert.ToString(max);
    lbCCZoomCurrent.Text = "Actual: " + Convert.ToString(defaultValue);

    // Establecer casillas de modo de control basadas en capacidades de cámara
    cbCCZoomManual.Checked = (flags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
    cbCCZoomAuto.Checked = (flags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
    cbCCZoomRelative.Checked = (flags & CameraControlFlags.Relative) == CameraControlFlags.Relative;
}
```

**Nota Técnica:** Cuando la bandera Auto está habilitada, la cámara ignorará todas las demás banderas y configuraciones de valor manual. Esto sigue los protocolos estándar de control de cámara de la industria.

### Paso 2: Aplicar Cambios de Parámetros

Cuando los usuarios ajustan configuraciones a través de tu interfaz, aplica los cambios a la cámara con este patrón:

```cs
// Determinar qué banderas de control deben estar activas basadas en selecciones de UI
CameraControlFlags flags = CameraControlFlags.None;

// Verificar si el modo manual está seleccionado
if (cbCCZoomManual.Checked)
{
    flags = flags | CameraControlFlags.Manual;
}

// Verificar si el modo automático está seleccionado
if (cbCCZoomAuto.Checked)
{
    flags = flags | CameraControlFlags.Auto;
}

// Aplicar el valor de zoom con las banderas seleccionadas
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Zoom,
    tbCCZoom.Value,
    flags
);
```

## Propiedades de Control Disponibles

### Control PTZ (Pan-Tilt-Zoom)

```cs
// Controlar Pan (movimiento horizontal)
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Pan,
    panValue,
    CameraControlFlags.Manual
);

// Controlar Tilt (movimiento vertical)
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Tilt,
    tiltValue,
    CameraControlFlags.Manual
);

// Controlar Zoom
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Zoom,
    zoomValue,
    CameraControlFlags.Manual
);
```

### Control de Exposición

```cs
// Establecer exposición manual
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Exposure,
    exposureValue,
    CameraControlFlags.Manual
);

// Habilitar exposición automática
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Exposure,
    0, // Valor ignorado en modo auto
    CameraControlFlags.Auto
);
```

### Control de Enfoque

```cs
// Establecer enfoque manual
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Focus,
    focusValue,
    CameraControlFlags.Manual
);

// Habilitar auto-enfoque
await VideoCapture1.Video_CaptureDevice_CameraControl_SetAsync(
    CameraControlProperty.Focus,
    0,
    CameraControlFlags.Auto
);
```

## Mejores Prácticas

1. **Verificar disponibilidad**: Siempre verifica si la cámara soporta un control específico antes de usarlo
2. **Respetar rangos**: Usa solo valores dentro de los rangos mínimo y máximo reportados
3. **Iniciar vista previa primero**: El control de cámara requiere una sesión activa
4. **Manejar errores**: Algunas cámaras pueden no soportar todas las propiedades
5. **Considerar modos automáticos**: Muchas cámaras proporcionan excelentes resultados con configuraciones automáticas

## Aplicaciones de Ejemplo

Explora estas aplicaciones de ejemplo para ver control de cámara en acción:

- [Demo Principal de Video Capture (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
- [Demo de Control de Cámara (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.