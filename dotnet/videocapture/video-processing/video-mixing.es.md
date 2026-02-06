---
title: Combinar Múltiples Flujos de Video en .NET
description: Implementa Picture-in-Picture con múltiples fuentes de video en .NET usando ejemplos de código C# para mezclar flujos con layouts personalizados.
---

# Mezclar Múltiples Flujos de Video en Aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Mezcla de Flujos de Video

El SDK proporciona capacidades poderosas para mezclar varios flujos de video con diferentes resoluciones y tasas de fotogramas. Esta funcionalidad es esencial para crear aplicaciones de video profesionales que requieren efectos Picture-in-Picture, vistas multi-cámara o salidas de visualización combinadas.

## Características Clave de la Mezcla de Video

- Combinar múltiples fuentes de video en una sola salida
- Soporte para diferentes resoluciones de video y tasas de fotogramas
- Múltiples opciones de layout (horizontal, vertical, cuadrícula, personalizado)
- Ajustes de posición y parámetros en tiempo real
- Controles de transparencia y volteo
- Soporte para diversas fuentes de entrada (cámaras, pantallas, cámaras IP)

## Motor VideoCaptureCore

El proceso de implementación implica tres pasos principales:

1. Configurar el modo de mezcla de video
2. Añadir múltiples fuentes de video
3. Configurar y ajustar parámetros de fuentes

Exploremos cada paso en detalle con ejemplos de código.

### Configurar Modo de Mezcla de Video

El SDK soporta varios layouts predefinidos así como configuraciones personalizadas.

#### Layout de Pila Horizontal

Este modo organiza todas las fuentes de video horizontalmente en una fila:

```cs
VideoCapture1.PIP_Mode = PIPMode.Horizontal;
```

#### Layout de Pila Vertical

Este modo organiza todas las fuentes de video verticalmente en una columna:

```cs
VideoCapture1.PIP_Mode = PIPMode.Vertical;
```

#### Layout de Cuadrícula (2×2)

Este modo organiza hasta cuatro fuentes de video en una cuadrícula cuadrada:

```cs
VideoCapture1.PIP_Mode = PIPMode.Mode2x2;
```

#### Layout Personalizado con Resolución Específica

Para escenarios avanzados, puedes definir un layout personalizado con dimensiones de salida precisas:

```cs
VideoCapture1.PIP_Mode = PIPMode.Custom;
VideoCapture1.PIP_CustomOutputSize_Set(1920, 1080);
```

### Añadir Fuentes de Video

La primera fuente configurada sirve como entrada principal, mientras que fuentes adicionales pueden añadirse usando la API PIP.

#### Añadir un Dispositivo de Captura de Video

Para incorporar una cámara u otro dispositivo de captura de video:

```cs
VideoCapture1.PIP_Sources_Add_VideoCaptureDevice(
    nombreDispositivo,
    formato,
    false,
    tasaFotogramas,
    entrada,
    izquierda,
    arriba,
    ancho,
    alto);
```

#### Añadir una Fuente de Cámara IP

Para cámaras basadas en red:

```cs
var fuenteCamaraIP = new IPCameraSourceSettings
{
    URL = "url de cámara"
};

// Establecer parámetros adicionales de cámara IP según sea necesario
// Autenticación, protocolo, buffering, etc.

VideoCapture1.PIP_Sources_Add_IPCamera(
    fuenteCamaraIP,
    izquierda,
    arriba,
    ancho,
    alto);
```

#### Añadir una Fuente de Captura de Pantalla

Para incluir escritorio o ventanas de aplicación:

```cs
ScreenCaptureSourceSettings fuentePantalla = new ScreenCaptureSourceSettings();
fuentePantalla.Mode = ScreenCaptureMode.Screen;
fuentePantalla.FullScreen = true;
VideoCapture1.PIP_Sources_Add_ScreenSource(
    fuentePantalla,
    izquierda,
    arriba,
    ancho,
    alto);
```

### Ajustes Dinámicos de Fuentes

Una ventaja principal del SDK es la capacidad de ajustar parámetros de fuentes en tiempo real durante la operación.

#### Reposicionar Fuentes

Puedes modificar la posición y dimensiones de cualquier fuente:

```cs
await VideoCapture1.PIP_Sources_SetSourcePositionAsync(
    indice,  // 0 es fuente principal, 1+ son fuentes adicionales
    izquierda,
    arriba,
    ancho,
    alto);
```

#### Ajustar Propiedades Visuales

Ajusta finamente la apariencia con controles de transparencia y orientación:

```cs
int transparencia = 127; // Rango: 0-255
bool voltearX = false;
bool voltearY = false;

await VideoCapture1.PIP_Sources_SetSourceSettingsAsync(
    indice,
    transparencia, 
    transparencia, 
    voltearX, 
    voltearY);
```

## Motor VideoCaptureCoreX

El motor VideoCaptureCoreX es una versión más nueva del motor VideoCaptureCore que proporciona características adicionales y mejoras.

### Mezcla de Video con VideoCaptureCoreX

El motor VideoCaptureCoreX ofrece un enfoque más flexible y poderoso para la mezcla de video a través de la clase `VideoMixerSourceSettings`.

#### Configuración Básica

```cs
using System;
using System.Threading.Tasks;
using VisioForge.Core.Types.MediaInfo;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;

// Crear un mezclador de video con resolución 1080p a 30fps
var mezcladorVideo = new VideoMixerSourceSettings(1920, 1080, VideoFrameRate.FPS_30);

// Inicializar el motor VideoCaptureCoreX
var captureX = new VideoCaptureCoreX();
captureX.Video_Source = mezcladorVideo;
```

#### Añadir Múltiples Fuentes

Puedes añadir varias fuentes de video al mezclador, posicionando cada una exactamente donde se necesite:

```cs
// Añadir una cámara como primera fuente (fondo de pantalla completa)
var fuenteCamara = new VideoCaptureDeviceSourceSettings(); 

// Configurar ajustes de cámara
// ...

var rect = new Rect(0, 0, 1920, 1080);
mezcladorVideo.Add(fuenteCamara, rect);

// Añadir una fuente de captura de pantalla en la esquina superior derecha
var fuentePantalla = new ScreenCaptureDX9SourceSettings();
fuentePantalla.CaptureCursor = true;
fuentePantalla.FrameRate = VideoFrameRate.FPS_30;
 
var rect2 = new Rect(1280, 0, 640, 480);
mezcladorVideo.Add(fuentePantalla, rect2);
```

#### Gestión Dinámica de Fuentes

El motor VideoCaptureCoreX permite modificaciones en tiempo real al mezclador de video:

```cs
// Cambiar la posición de una fuente (índice 1, que es la captura de pantalla)
var mezclador = _videoCapture.GetSourceMixerControl();
if (mezclador != null)
{
    var stream = mezclador.Input_Get(1);
    if (stream != null)
    {
        stream.Rectangle = new Rect(0, 0, 1280, 720);
        mezclador.Input_Update(1, stream);
    }
}
```

### Comparación con Motor VideoCaptureCore

| Característica | VideoCaptureCore | VideoCaptureCoreX |
|----------------|------------------|-------------------|
| Estilo de API | Enfoque basado en métodos | Orientado a objetos con clases de configuración |
| Flexibilidad | Layouts predefinidos | Posicionamiento de fuentes completamente personalizable |
| Gestión de Fuentes | API fija para añadir fuentes | Añadir cualquier fuente que implemente IVideoMixerSource |
| Rendimiento | Bueno | Mejorado con pipeline de renderizado optimizado |
| Cambios en Tiempo Real | Limitado | Manipulación completa de fuentes |

## Escenarios de Uso Avanzado

Las capacidades de mezcla de video habilitan numerosos escenarios de aplicación:

- Monitoreo de seguridad multi-cámara
- Videoconferencia con compartir pantalla
- Broadcasting con superposición de comentarista
- Presentaciones educativas con múltiples entradas
- Streams de gaming con cámara de jugador

## Consejos de Solución de Problemas

- Asegura que los recursos de hardware sean suficientes para el número de flujos
- Monitorea uso de CPU y memoria durante la operación
- Considera resoluciones más bajas para rendimiento más fluido con muchas fuentes
- Prueba diferentes configuraciones de layout para resultados visuales óptimos

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para muestras de código adicionales y ejemplos de implementación.