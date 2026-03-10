---
title: Composición de Superposición de Video en C# .NET — Guía
description: Añada superposiciones de texto, imágenes, formas y PiP a video en vivo con VisioForge Media Blocks SDK OverlayManagerBlock y gestión de capas.
---

# Guía de Uso del Bloque Overlay Manager

## Descripción General

El `OverlayManagerBlock` es un componente poderoso de MediaBlocks que proporciona composición dinámica de superposiciones de video multicapa y gestión. Te permite agregar varios elementos de superposición (imágenes, texto, formas, animaciones) sobre el contenido de video con actualizaciones en tiempo real, gestión de capas y características avanzadas como sombras, rotación y control de opacidad.

## Características Principales

- **Múltiples Tipos de Superposición**: Texto, texto desplazable, imágenes, secuencias de imágenes, GIFs, SVG, formas (rectángulos, círculos, triángulos, estrellas, líneas), archivos de video/URLs, video en vivo (NDI, Decklink), contenido web WebView2 (Windows), controles WPF (Windows)
- **Superposiciones de Archivos de Video**: Reproduce archivos de video o URLs de streaming como superposiciones con control completo de reproducción y salida de audio opcional
- **Superposiciones de Controles WPF**: Renderiza elementos WPF en vivo con animaciones y data binding como superposiciones de video (solo Windows)
- **Grupos de Superposiciones**: Sincroniza múltiples superposiciones para inicio/parada coordinados con soporte de precarga
- **Efectos Squeezeback**: Escala el video a un rectángulo personalizado con imagen superpuesta (estilo broadcast)
- **Transformaciones de Video**: Efectos de zoom y panorámica que transforman todo el cuadro de video
- **Soporte de Animación**: Anima la posición/escala del video con funciones de suavizado
- **Efectos de Desvanecimiento**: Fade in/out para video y elementos superpuestos
- **Gestión de Capas**: Ordenamiento por z-index para apilamiento correcto de superposiciones
- **Efectos Avanzados**: Sombras, rotación, opacidad, posicionamiento personalizado
- **Actualizaciones en Tiempo Real**: Modificación dinámica de superposiciones durante la reproducción
- **Visualización Basada en Tiempo**: Mostrar/ocultar superposiciones en marcas de tiempo específicas
- **Dibujo Personalizado**: Soporte de callback para operaciones de dibujo Cairo personalizadas
- **Fuentes de Video en Vivo**: Soporte para fuentes de red NDI y tarjetas de captura Decklink
- **Multiplataforma**: Funciona en Windows, Linux, macOS, iOS y Android

## Referencia de Clase

### OverlayManagerBlock

**Namespace**: `VisioForge.Core.MediaBlocks.VideoProcessing`

```csharp
public class OverlayManagerBlock : MediaBlock, IMediaBlockInternals
```

#### Propiedades

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Type` | `MediaBlockType` | Retorna `MediaBlockType.OverlayManager` |
| `Input` | `MediaBlockPad` | Pad de entrada de video |
| `Output` | `MediaBlockPad` | Pad de salida de video con superposiciones |

#### Métodos

##### Métodos Estáticos

```csharp
public static bool IsAvailable()
```

Verifica si el overlay manager está disponible en el entorno actual (requiere soporte de overlay Cairo).

##### Métodos de Instancia

```csharp
public void Video_Overlay_Add(IOverlayManagerElement overlay)
```

Agrega un nuevo elemento de superposición a la composición de video.

```csharp
public void Video_Overlay_Remove(IOverlayManagerElement overlay)
```

Elimina un elemento de superposición específico.

```csharp
public void Video_Overlay_RemoveAt(int index)
```

Elimina una superposición en el índice especificado.

```csharp
public void Video_Overlay_Clear()
```

Elimina todos los elementos de superposición.

```csharp
public void Video_Overlay_Update(IOverlayManagerElement overlay)
```

Actualiza una superposición existente (elimina y re-agrega con nuevas propiedades).

## Tipos de Elementos de Superposición

### Propiedades Comunes (IOverlayManagerElement)

Todos los elementos de superposición implementan la interfaz `IOverlayManagerElement` con estas propiedades comunes:

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Name` | `string` | - | Nombre opcional para identificación |
| `Enabled` | `bool` | `true` | Habilitar/deshabilitar la superposición |
| `StartTime` | `TimeSpan` | `Zero` | Cuándo comenzar a mostrar (opcional) |
| `EndTime` | `TimeSpan` | `Zero` | Cuándo dejar de mostrar (opcional) |
| `Opacity` | `double` | `1.0` | Transparencia (0.0-1.0) |
| `Rotation` | `double` | `0.0` | Ángulo de rotación en grados (0-360) |
| `ZIndex` | `int` | `0` | Orden de capa (mayor = encima) |
| `Shadow` | `OverlayManagerShadowSettings` | - | Configuración de sombra |

### OverlayManagerText

Muestra texto con fondo y formato opcionales.

```csharp
public class OverlayManagerText : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Text` | `string` | "Hello!!!" | Texto a mostrar |
| `X` | `int` | `100` | Posición X |
| `Y` | `int` | `100` | Posición Y |
| `Font` | `FontSettings` | Predeterminado del sistema | Configuración de fuente |
| `Color` | `SKColor` | `Red` | Color del texto |
| `Background` | `IOverlayManagerBackground` | `null` | Fondo opcional |
| `CustomWidth` | `int` | `0` | Ancho de límite personalizado (0 = auto) |
| `CustomHeight` | `int` | `0` | Alto de límite personalizado (0 = auto) |

**Ejemplo:**

```csharp
var text = new OverlayManagerText("¡Hola Mundo!", 100, 100);
text.Color = SKColors.White;
text.Font.Size = 48;
text.Font.Name = "Arial";
text.Shadow = new OverlayManagerShadowSettings(true, depth: 5, direction: 45);
overlayManager.Video_Overlay_Add(text);
```

### OverlayManagerImage

Muestra imágenes estáticas con modos de estiramiento.

```csharp
public class OverlayManagerImage : IOverlayManagerElement, IDisposable
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `X` | `int` | - | Posición X |
| `Y` | `int` | - | Posición Y |
| `Width` | `int` | - | Ancho de visualización (0 = original) |
| `Height` | `int` | - | Alto de visualización (0 = original) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `None` | Modo de escalado de imagen |

**Modos de Estiramiento:**

- `None` - Tamaño original
- `Stretch` - Llenar área objetivo (puede distorsionar)
- `Letterbox` - Ajustar dentro del área (mantener relación de aspecto)
- `CropToFill` - Llenar área recortando (mantener relación de aspecto)

**Constructores:**

```csharp
// Desde archivo
new OverlayManagerImage(string filename, int x, int y, double alpha = 1.0)

// Desde bitmap de SkiaSharp
new OverlayManagerImage(SKBitmap image, int x, int y, double alpha = 1.0)

// Desde System.Drawing.Bitmap (solo Windows)
new OverlayManagerImage(System.Drawing.Bitmap image, int x, int y, double alpha = 1.0)
```

**Ejemplo:**

```csharp
var image = new OverlayManagerImage("logo.png", 10, 10);
image.StretchMode = OverlayManagerImageStretchMode.Letterbox;
image.Width = 200;
image.Height = 100;
overlayManager.Video_Overlay_Add(image);
```

### OverlayManagerGIF

Muestra imágenes GIF animadas.

```csharp
public class OverlayManagerGIF : IOverlayManagerElement, IDisposable
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Position` | `SKPoint` | Posición del GIF |
| `AnimationLength` | `TimeSpan` | Duración total de la animación |

**Ejemplo:**

```csharp
var gif = new OverlayManagerGIF("animacion.gif", new SKPoint(150, 150));
overlayManager.Video_Overlay_Add(gif);
```

### OverlayManagerImageSequence

Muestra una secuencia de imágenes, cada una mostrada durante una duración especificada, con soporte para bucle, animación, efectos de desvanecimiento y todas las propiedades estándar de superposición.

```csharp
public class OverlayManagerImageSequence : IOverlayManagerElement, IDisposable
```

| Propiedad         | Tipo                             | Predeterminado | Descripción                                        |
|-------------------|----------------------------------|----------------|----------------------------------------------------|
| `X`               | `int`                            | -              | Posición X                                         |
| `Y`               | `int`                            | -              | Posición Y                                         |
| `Width`           | `int`                            | `0`            | Ancho de visualización (0 = original)              |
| `Height`          | `int`                            | `0`            | Alto de visualización (0 = original)               |
| `Loop`            | `bool`                           | `true`         | Reiniciar secuencia después del último cuadro      |
| `StretchMode`     | `OverlayManagerImageStretchMode` | `None`         | Modo de escalado de imagen                         |
| `AnimationLength` | `TimeSpan`                       | -              | Duración total de todos los cuadros (solo lectura) |
| `FrameCount`      | `int`                            | -              | Número de cuadros cargados (solo lectura)          |

**Propiedades de Animación:**

| Propiedad             | Tipo                      | Predeterminado | Descripción                            |
|-----------------------|---------------------------|----------------|----------------------------------------|
| `AnimationEnabled`    | `bool`                    | `false`        | Habilitar animación de posición/tamaño |
| `TargetX`             | `int`                     | `0`            | X destino de la animación              |
| `TargetY`             | `int`                     | `0`            | Y destino de la animación              |
| `TargetWidth`         | `int`                     | `0`            | Ancho destino (0 = mantener actual)    |
| `TargetHeight`        | `int`                     | `0`            | Alto destino (0 = mantener actual)     |
| `AnimationStartTime`  | `TimeSpan`                | `Zero`         | Tiempo de inicio de la animación       |
| `AnimationEndTime`    | `TimeSpan`                | `Zero`         | Tiempo de fin de la animación          |
| `Easing`              | `OverlayManagerPanEasing` | `Linear`       | Función de suavizado de posición       |

**Propiedades de Desvanecimiento:**

| Propiedad       | Tipo                      | Predeterminado | Descripción                                |
|-----------------|---------------------------|----------------|--------------------------------------------|
| `FadeEnabled`   | `bool`                    | `false`        | Habilitar animación de desvanecimiento     |
| `FadeType`      | `OverlayManagerFadeType`  | `FadeIn`       | Dirección del desvanecimiento              |
| `FadeStartTime` | `TimeSpan`                | `Zero`         | Tiempo de inicio del desvanecimiento       |
| `FadeEndTime`   | `TimeSpan`                | `Zero`         | Tiempo de fin del desvanecimiento          |
| `FadeEasing`    | `OverlayManagerPanEasing` | `Linear`       | Función de suavizado                       |

**Constructores:**

```csharp
// Básico: solo posición
new OverlayManagerImageSequence(IEnumerable<ImageSequenceItem> items, int x, int y)

// Completo: posición, tamaño y modo de escalado
new OverlayManagerImageSequence(
    IEnumerable<ImageSequenceItem> items,
    int x, int y,
    int width, int height,
    OverlayManagerImageStretchMode stretchMode = None)
```

**Métodos:**

```csharp
// Agregar un cuadro dinámicamente durante la reproducción
void AddFrame(string filename, TimeSpan duration)

// Iniciar animación de posición/tamaño
void StartAnimation(int targetX, int targetY, int targetWidth, int targetHeight,
    TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)

// Obtener posición/tamaño interpolado en el tiempo dado
(int X, int Y, int Width, int Height) GetCurrentRect(TimeSpan currentTime)

// Efectos de desvanecimiento
void StartFadeIn(TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)
void StartFadeOut(TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)
double GetCurrentOpacity(TimeSpan currentTime)
```

**Ejemplo - Secuencia de Imágenes Básica:**

```csharp
// Definir imágenes con duraciones por cuadro
var items = new List<ImageSequenceItem>
{
    new ImageSequenceItem("diapositiva1.png", TimeSpan.FromSeconds(3)),
    new ImageSequenceItem("diapositiva2.png", TimeSpan.FromSeconds(2)),
    new ImageSequenceItem("diapositiva3.png", TimeSpan.FromSeconds(4))
};

// Crear secuencia en posición (100, 100), escalada a 320x240
var sequence = new OverlayManagerImageSequence(items, 100, 100, 320, 240)
{
    Loop = true,
    Opacity = 0.9,
    ZIndex = 5,
    Name = "SlideShow"
};

overlayManager.Video_Overlay_Add(sequence);
```

**Ejemplo - Con Animación y Desvanecimiento:**

```csharp
// Animar la secuencia desde arriba a la izquierda hacia el centro en 3 segundos
sequence.StartAnimation(
    targetX: 400, targetY: 200,
    targetWidth: 640, targetHeight: 480,
    startTime: TimeSpan.FromSeconds(2),
    duration: TimeSpan.FromSeconds(3),
    easing: OverlayManagerPanEasing.EaseInOut);

// Aparecer gradualmente durante los primeros 1.5 segundos
sequence.StartFadeIn(
    startTime: TimeSpan.Zero,
    duration: TimeSpan.FromSeconds(1.5),
    easing: OverlayManagerPanEasing.EaseOut);
```

**Métodos de Conveniencia en OverlayManagerBlock:**

```csharp
// Agregar superposición de secuencia de imágenes
var element = overlayManager.Video_Overlay_AddImageSequence(
    items, x: 100, y: 100, width: 320, height: 240,
    loop: true, name: "SlideShow");

// Actualizar posición
overlayManager.Video_Overlay_UpdateImageSequencePosition(
    "SlideShow", x: 200, y: 150, width: 400, height: 300);

// Animar posición/tamaño
overlayManager.Video_Overlay_AnimateImageSequence(
    "SlideShow", targetX: 500, targetY: 300, targetWidth: 640, targetHeight: 480,
    startTime: currentPosition, duration: TimeSpan.FromSeconds(2),
    easing: OverlayManagerPanEasing.EaseInOut);

// Efectos de desvanecimiento
overlayManager.Video_Overlay_ImageSequenceFadeIn(
    "SlideShow", startTime: currentPosition, duration: TimeSpan.FromSeconds(1));
overlayManager.Video_Overlay_ImageSequenceFadeOut(
    "SlideShow", startTime: currentPosition, duration: TimeSpan.FromSeconds(1));
```

#### ImageSequenceItem

Representa un cuadro de imagen individual en una secuencia de imágenes.

```csharp
public class ImageSequenceItem
```

| Propiedad  | Tipo       | Descripción                               |
|------------|------------|-------------------------------------------|
| `Filename` | `string`   | Ruta completa al archivo de imagen        |
| `Duration` | `TimeSpan` | Duración de visualización de esta imagen  |

```csharp
// Constructores
new ImageSequenceItem()
new ImageSequenceItem(string filename, TimeSpan duration)
```

### OverlayManagerDateTime

Muestra fecha/hora actual con formato personalizado.

```csharp
public class OverlayManagerDateTime : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Text` | `string` | "[DATETIME]" | Plantilla de texto |
| `Format` | `string` | "MM/dd/yyyy HH:mm:ss" | Formato de DateTime |
| `X` | `int` | `100` | Posición X |
| `Y` | `int` | `100` | Posición Y |
| `Font` | `FontSettings` | Predeterminado del sistema | Configuración de fuente |
| `Color` | `SKColor` | `Red` | Color del texto |

**Ejemplo:**

```csharp
var dateTime = new OverlayManagerDateTime();
dateTime.Format = "yyyy-MM-dd HH:mm:ss";
dateTime.X = 10;
dateTime.Y = 30;
overlayManager.Video_Overlay_Add(dateTime);
```

### OverlayManagerScrollingText

Muestra texto desplazable que se mueve a través del video en una dirección especificada.

```csharp
public class OverlayManagerScrollingText : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Text` | `string` | "Scrolling Text" | Texto a mostrar |
| `X` | `int` | `0` | Posición X del área de desplazamiento |
| `Y` | `int` | `100` | Posición Y del área de desplazamiento |
| `Width` | `int` | `0` | Ancho del área de desplazamiento (0 = usa DefaultWidth) |
| `Height` | `int` | `0` | Alto del área de desplazamiento (0 = auto según fuente) |
| `DefaultWidth` | `int` | `1920` | Ancho predeterminado cuando Width es 0 (configurar al ancho del video) |
| `DefaultHeight` | `int` | `1080` | Alto predeterminado cuando Height es 0 para desplazamiento vertical |
| `Speed` | `int` | `5` | Velocidad de desplazamiento en píxeles por cuadro |
| `Direction` | `ScrollDirection` | `RightToLeft` | Dirección de desplazamiento |
| `Font` | `FontSettings` | Predeterminado del sistema | Configuración de fuente |
| `Color` | `SKColor` | `White` | Color del texto |
| `BackgroundTransparent` | `bool` | `true` | Si el fondo es transparente |
| `BackgroundColor` | `SKColor` | `Black` | Color de fondo (cuando no es transparente) |
| `Infinite` | `bool` | `true` | Repetir desplazamiento infinitamente |
| `TextRestarted` | `EventHandler` | `null` | Se llama cuando el texto vuelve al inicio |

**Enumeración ScrollDirection:**

- `LeftToRight` - El texto se desplaza de izquierda a derecha
- `RightToLeft` - El texto se desplaza de derecha a izquierda
- `BottomToTop` - El texto se desplaza de abajo hacia arriba
- `TopToBottom` - El texto se desplaza de arriba hacia abajo

**Ejemplo:**

```csharp
// Crear un texto desplazable estilo ticker de noticias
var scrollingText = new OverlayManagerScrollingText(
    "Última hora: VisioForge Media Framework ahora soporta superposiciones de texto desplazable!",
    x: 0,
    y: 50,
    speed: 3,
    direction: ScrollDirection.RightToLeft);

scrollingText.Font.Size = 24;
scrollingText.Color = SKColors.Yellow;
scrollingText.BackgroundTransparent = false;
scrollingText.BackgroundColor = SKColors.DarkBlue;

// Establecer el ancho predeterminado para coincidir con la resolución de video
// Se usa cuando Width no está configurado explícitamente
scrollingText.DefaultWidth = 1920; // Ancho Full HD

// O configurar Width directamente para un ancho específico del área de desplazamiento
// scrollingText.Width = 1920;

// Agregar manejador de evento para cuando el texto se reinicie
scrollingText.TextRestarted += (sender, e) => {
    Console.WriteLine("El texto desplazable se reinició");
};

overlayManager.Video_Overlay_Add(scrollingText);

// Para reiniciar la posición de desplazamiento
scrollingText.Reset();

// Para actualizar después de cambiar texto o fuente
scrollingText.Text = "Texto de noticias actualizado...";
scrollingText.Update();
```

### Superposiciones de Video

El Overlay Manager soporta superposiciones de video de múltiples fuentes, incluyendo archivos de video/URLs, tarjetas de captura Decklink y fuentes de red NDI. Las superposiciones de video se reproducen dentro de la composición con control completo de reproducción.

#### OverlayManagerVideo

Reproduce archivos de video o URLs de streaming como superposiciones en la composición de video. Cada superposición de video ejecuta su propio pipeline de reproducción interno con salida de audio opcional.

```csharp
public class OverlayManagerVideo : IOverlayManagerElement, IDisposable
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Source` | `string` | - | Ruta del archivo de video o URL |
| `X` | `int` | - | Posición X |
| `Y` | `int` | - | Posición Y |
| `Width` | `int` | - | Ancho de superposición |
| `Height` | `int` | - | Alto de superposición |
| `Loop` | `bool` | `true` | Si el video se repite en bucle |
| `PlaybackRate` | `double` | `1.0` | Velocidad de reproducción (1.0 = normal) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Cómo ajustar el video |
| `VideoView` | `IVideoView` | `null` | Ventana de vista previa de video externa opcional |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Configuración del renderizador para VideoView |
| `AudioOutput` | `AudioOutputDeviceInfo` | `null` | Dispositivo de salida de audio (null = descartar audio) |
| `AudioOutput_Volume` | `double` | `1.0` | Volumen de audio (0.0-1.0+, superior a 1.0 amplifica) |
| `AudioOutput_Mute` | `bool` | `false` | Silenciar salida de audio |

**Métodos:**

- `Initialize(bool autoStart)` - Inicializar el pipeline de video. Si `autoStart` es true, comienza la reproducción inmediatamente; si es false, precarga en estado PAUSED.
- `Play()` - Iniciar o reanudar reproducción de video
- `Pause()` - Pausar reproducción de video
- `Stop()` - Detener reproducción de video
- `Seek(TimeSpan position)` - Ir a una posición específica en el video
- `UpdateSource(string source)` - Cambiar la fuente de video dinámicamente
- `Dispose()` - Liberar recursos

**Ejemplo - Superposición de Archivo de Video:**

```csharp
// Crear una superposición de archivo de video
var videoOverlay = new OverlayManagerVideo(
    source: "intro.mp4",
    x: 100,
    y: 100,
    width: 640,
    height: 360)
{
    Loop = true,
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 5
};

// Opcionalmente habilitar salida de audio
var audioOutputs = await AudioRendererBlock.GetDevicesAsync(AudioOutputDeviceAPI.DirectSound);
videoOverlay.AudioOutput = audioOutputs[0];
videoOverlay.AudioOutput_Volume = 0.5;

// Inicializar y agregar al overlay manager
if (videoOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(videoOverlay);
}

// Controlar reproducción en tiempo de ejecución
videoOverlay.Pause();
videoOverlay.Seek(TimeSpan.FromSeconds(10));
videoOverlay.Play();

// Cambiar fuente dinámicamente
videoOverlay.UpdateSource("outro.mp4");

// Limpiar cuando termine
videoOverlay.Stop();
videoOverlay.Dispose();
```

**Ejemplo - Picture-in-Picture:**

```csharp
// Crear un video PiP pequeño en la esquina
var pipVideo = new OverlayManagerVideo(
    source: "camera_feed.mp4",
    x: 20,
    y: 20,
    width: 240,
    height: 135)
{
    Loop = true,
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 100,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.DarkGray,
        Opacity = 0.7,
        BlurRadius = 8,
        Depth = 3,
        Direction = 45
    }
};

if (pipVideo.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(pipVideo);
}
```

**Ejemplo - Superposición de URL de Streaming:**

```csharp
// Superponer un stream de red
var streamOverlay = new OverlayManagerVideo(
    source: "rtsp://192.168.1.21:554/Streaming/Channels/101",
    x: 400,
    y: 50,
    width: 320,
    height: 240)
{
    Loop = false,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 10
};

if (streamOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(streamOverlay);
}
```

#### OverlayManagerDecklinkVideo

Captura y muestra video de tarjetas de captura Blackmagic Decklink.

```csharp
public class OverlayManagerDecklinkVideo : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `DecklinkSettings` | `DecklinkVideoSourceSettings` | - | Configuración del dispositivo Decklink |
| `X` | `int` | - | Posición X |
| `Y` | `int` | - | Posición Y |
| `Width` | `int` | - | Ancho de superposición |
| `Height` | `int` | - | Alto de superposición |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Cómo ajustar el video |
| `VideoView` | `IVideoView` | `null` | Vista previa de video opcional |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Configuración del renderizador |

**Ejemplo:**

```csharp
// Obtener dispositivos Decklink
var devices = await DecklinkVideoSourceBlock.GetDevicesAsync();
var decklinkSettings = new DecklinkVideoSourceSettings(devices[0]);
decklinkSettings.Mode = DecklinkMode.HD1080p2997;

// Crear superposición Decklink
var decklinkOverlay = new OverlayManagerDecklinkVideo(
    decklinkSettings, 
    x: 10, 
    y: 10, 
    width: 640, 
    height: 360);

// Inicializar y agregar al overlay manager
if (decklinkOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(decklinkOverlay);
}

// Limpiar cuando termine
decklinkOverlay.Stop();
decklinkOverlay.Dispose();
```

#### OverlayManagerNDIVideo

Captura y muestra video de fuentes NDI (Network Device Interface).

```csharp
public class OverlayManagerNDIVideo : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `NDISettings` | `NDISourceSettings` | - | Configuración de fuente NDI |
| `X` | `int` | - | Posición X |
| `Y` | `int` | - | Posición Y |
| `Width` | `int` | - | Ancho de superposición |
| `Height` | `int` | - | Alto de superposición |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Cómo ajustar el video |
| `VideoView` | `IVideoView` | `null` | Vista previa de video opcional |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Configuración del renderizador |

**Ejemplo:**

```csharp
// Descubrir fuentes NDI en la red
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSettings = await NDISourceSettings.CreateAsync(
    null, 
    ndiSources[0].Name, 
    ndiSources[0].URL);

// Crear superposición NDI
var ndiOverlay = new OverlayManagerNDIVideo(
    ndiSettings, 
    x: 10, 
    y: 10, 
    width: 640, 
    height: 360);

// Inicializar y agregar al overlay manager
if (ndiOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(ndiOverlay);
}

// Limpiar cuando termine
ndiOverlay.Stop();
ndiOverlay.Dispose();
```

**Métodos Comunes para Superposiciones de Video:**

- `Initialize(bool autoStart)` - Inicializar el pipeline de video
- `Play()` - Iniciar o reanudar reproducción/captura de video
- `Pause()` - Pausar reproducción/captura de video
- `Stop()` - Detener reproducción/captura de video
- `Dispose()` - Limpiar recursos

**Métodos Adicionales (solo OverlayManagerVideo):**

- `Seek(TimeSpan position)` - Ir a una posición específica
- `UpdateSource(string source)` - Cambiar la fuente de video dinámicamente

### OverlayManagerWPFControl (Solo Windows)

Renderiza un `FrameworkElement` de WPF como superposición de video. Esto permite usar cualquier árbol visual de WPF — incluyendo controles con animaciones Storyboard, data binding y layouts complejos — como superposición. El elemento se captura periódicamente a una tasa de refresco configurable.

> **Nota**: Este tipo de superposición solo está disponible en Windows (target de compilación `NET_WINDOWS`).

```csharp
public class OverlayManagerWPFControl : IOverlayManagerElement, IDisposable
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `ElementFactory` | `Func<FrameworkElement>` | - | Fábrica que crea el elemento WPF en el hilo STA interno |
| `X` | `int` | - | Posición X |
| `Y` | `int` | - | Posición Y |
| `Width` | `int` | - | Ancho de superposición (debe ser > 0) |
| `Height` | `int` | - | Alto de superposición (debe ser > 0) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Stretch` | Cómo ajustar el control renderizado |
| `RefreshRate` | `int` | `15` | Capturas por segundo (1-60) |
| `Dpi` | `double` | `96` | DPI para renderizado |

**Métodos:**

- `Initialize()` - Inicializar la superposición de control WPF y comenzar renderizado periódico
- `InvokeOnUIThread(Action action)` - Ejecutar una acción en el hilo STA de WPF para actualizaciones seguras en tiempo de ejecución
- `InvokeOnUIThread<T>(Func<T> func)` - Ejecutar una función en el hilo STA de WPF y retornar el resultado
- `Dispose()` - Liberar recursos

**Método de Conveniencia en OverlayManagerBlock:**

```csharp
public OverlayManagerWPFControl Video_Overlay_AddWPFControl(
    Func<FrameworkElement> elementFactory,
    int x, int y, int width, int height,
    int refreshRate = 15,
    string name = null)
```

Crea, inicializa y agrega una superposición de control WPF en una sola llamada. Retorna la instancia de la superposición, o `null` si la inicialización falló.

**Ejemplo - Usando Método de Conveniencia:**

```csharp
// Agregar una superposición de texto con cambio de color usando el método de conveniencia
var wpfOverlay = overlayManager.Video_Overlay_AddWPFControl(
    elementFactory: () =>
    {
        var border = new Border
        {
            Width = 350,
            Height = 60,
            Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(15, 5, 15, 5)
        };

        var text = new TextBlock
        {
            Text = "VisioForge Media Framework",
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var brush = new SolidColorBrush(Colors.Red);
        text.Foreground = brush;
        border.Child = text;

        // Animar color del texto
        var colorAnim = new ColorAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromSeconds(5),
            RepeatBehavior = RepeatBehavior.Forever
        };
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Red, KeyTime.FromPercent(0.0)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Gold, KeyTime.FromPercent(0.25)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Cyan, KeyTime.FromPercent(0.5)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Magenta, KeyTime.FromPercent(0.75)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Red, KeyTime.FromPercent(1.0)));
        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);

        return border;
    },
    x: 50, y: 300, width: 350, height: 60,
    refreshRate: 30, name: "ColorText");

if (wpfOverlay == null)
{
    // La inicialización falló
}
```

**Ejemplo - Creación Manual con Reloj Animado:**

```csharp
// Crear una superposición de reloj analógico WPF manualmente
var clockOverlay = new OverlayManagerWPFControl(
    elementFactory: () =>
    {
        var canvas = new Canvas { Width = 200, Height = 200 };

        // Esfera del reloj
        var face = new Ellipse
        {
            Width = 180, Height = 180,
            Stroke = Brushes.White, StrokeThickness = 3
        };
        Canvas.SetLeft(face, 10);
        Canvas.SetTop(face, 10);
        canvas.Children.Add(face);

        // Manecilla de segundos con animación de rotación
        var secondHand = new Line
        {
            X1 = 100, Y1 = 100, X2 = 100, Y2 = 25,
            Stroke = Brushes.Red, StrokeThickness = 1
        };
        var secondRotate = new RotateTransform(DateTime.Now.Second * 6, 100, 100);
        secondHand.RenderTransform = secondRotate;
        canvas.Children.Add(secondHand);

        var anim = new DoubleAnimation
        {
            From = DateTime.Now.Second * 6,
            To = DateTime.Now.Second * 6 + 360,
            Duration = TimeSpan.FromSeconds(60),
            RepeatBehavior = RepeatBehavior.Forever
        };
        secondRotate.BeginAnimation(RotateTransform.AngleProperty, anim);

        return canvas;
    },
    x: 20, y: 20, width: 200, height: 200, refreshRate: 30);

if (clockOverlay.Initialize())
{
    overlayManager.Video_Overlay_Add(clockOverlay);
}

// Actualizar control WPF en tiempo de ejecución (seguro entre hilos)
clockOverlay.InvokeOnUIThread(() =>
{
    // Es seguro modificar elementos WPF aquí
});

// Limpiar
clockOverlay.Dispose();
```

### OverlayManagerWebView2Video (Solo Windows)

Renderiza contenido web en vivo (HTML, CSS, JavaScript) como una superposición de video usando Microsoft WebView2. Esto permite mostrar páginas web, dashboards HTML dinámicos, contenido web animado, tickers de noticias o cualquier contenido renderizado por navegador como superposición en tu video. La página web se renderiza fuera de pantalla y se captura como cuadros de video a la tasa de refresco nativa del navegador.

> **Nota**: Este tipo de superposición solo está disponible en Windows (target de compilación `NET_WINDOWS`). Requiere el Runtime de Microsoft WebView2 y el plugin GStreamer WebView2 (`webview2src`).

```csharp
public class OverlayManagerWebView2Video : IOverlayManagerElement, IDisposable
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Location` | `string` | `"about:blank"` | URL a mostrar en la superposición |
| `JavaScript` | `string` | `null` | Código JavaScript a ejecutar después de cada navegación completada |
| `Adapter` | `int` | `-1` | Índice del adaptador DXGI para selección de GPU (-1 = cualquier dispositivo disponible) |
| `UserDataFolder` | `string` | `null` | Ruta absoluta a la carpeta de datos de usuario de WebView2 para caché y datos de perfil |
| `X` | `int` | - | Posición X |
| `Y` | `int` | - | Posición Y |
| `Width` | `int` | - | Ancho de la superposición |
| `Height` | `int` | - | Alto de la superposición |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Cómo ajustar el contenido renderizado |
| `VideoView` | `IVideoView` | `null` | Ventana de vista previa de video externa opcional |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Configuración del renderizador para VideoView |

**Métodos:**

- `Initialize(bool autoStart = true)` - Inicializa el pipeline de renderizado WebView2. Si `autoStart` es true, comienza a renderizar inmediatamente; si es false, precarga en estado PAUSADO. Retorna `true` si tiene éxito.
- `Play()` - Inicia o reanuda el renderizado de la página web
- `Pause()` - Pausa el renderizado de la página web
- `Stop()` - Detiene el renderizado de la página web
- `UpdateLocation(string location)` - Cambia la URL mostrada dinámicamente
- `Dispose()` - Libera recursos

**Ejemplo - Superposición Básica de Página Web:**

```csharp
// Mostrar una página web como superposición de video
var webOverlay = new OverlayManagerWebView2Video(
    location: "https://example.com/dashboard",
    x: 50,
    y: 50,
    width: 640,
    height: 480)
{
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 5
};

// Inicializar y agregar al overlay manager
if (webOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(webOverlay);
}
else
{
    webOverlay.Dispose();
}
```

**Ejemplo - Superposición Web con Inyección de JavaScript:**

```csharp
// Superponer una página web e inyectar JavaScript para personalizarla
var tickerOverlay = new OverlayManagerWebView2Video(
    location: "https://example.com/ticker",
    x: 0,
    y: 680,
    width: 1920,
    height: 40)
{
    // JavaScript se ejecuta después de cada navegación completada
    JavaScript = "document.body.style.background = 'transparent';",
    Opacity = 0.8,
    ZIndex = 10,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.Black,
        Opacity = 0.5,
        BlurRadius = 5,
        Depth = 5,
        Direction = 45
    }
};

if (tickerOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(tickerOverlay);
}
```

**Ejemplo - Actualización Dinámica de URL en Tiempo de Ejecución:**

```csharp
// Cambiar la página mostrada en tiempo de ejecución
webOverlay.UpdateLocation("https://example.com/new-page");

// Controlar el renderizado
webOverlay.Pause();
webOverlay.Play();

// Liberar recursos cuando termine
webOverlay.Stop();
webOverlay.Dispose();
```

### Superposiciones de Formas

#### OverlayManagerLine

```csharp
public class OverlayManagerLine : IOverlayManagerElement
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Start` | `SKPoint` | Punto de inicio de línea |
| `End` | `SKPoint` | Punto de fin de línea |
| `Color` | `SKColor` | Color de línea |

#### OverlayManagerRectangle

```csharp
public class OverlayManagerRectangle : IOverlayManagerElement
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Rectangle` | `SKRect` | Límites del rectángulo |
| `Color` | `SKColor` | Color de relleno/trazo |
| `Fill` | `bool` | Rellenar o solo trazo |

#### OverlayManagerCircle

```csharp
public class OverlayManagerCircle : IOverlayManagerElement
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Center` | `SKPoint` | Centro del círculo |
| `Radius` | `double` | Radio del círculo |
| `Color` | `SKColor` | Color de relleno/trazo |
| `Fill` | `bool` | Rellenar o solo trazo |

#### OverlayManagerTriangle

```csharp
public class OverlayManagerTriangle : IOverlayManagerElement
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Point1` | `SKPoint` | Primer vértice |
| `Point2` | `SKPoint` | Segundo vértice |
| `Point3` | `SKPoint` | Tercer vértice |
| `Color` | `SKColor` | Color de relleno/trazo |
| `Fill` | `bool` | Rellenar o solo trazo |

#### OverlayManagerStar

```csharp
public class OverlayManagerStar : IOverlayManagerElement
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Center` | `SKPoint` | Centro de la estrella |
| `OuterRadius` | `double` | Radio de puntas externas |
| `InnerRadius` | `double` | Radio de puntas internas |
| `StrokeColor` | `SKColor` | Color de trazo |
| `FillColor` | `SKColor` | Color de relleno |

### OverlayManagerSVG

Muestra gráficos vectoriales SVG.

```csharp
public class OverlayManagerSVG : IOverlayManagerElement, IDisposable
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `X` | `int` | Posición X |
| `Y` | `int` | Posición Y |
| `Width` | `int` | Ancho de visualización |
| `Height` | `int` | Alto de visualización |

### OverlayManagerCallback

Dibujo personalizado usando gráficos Cairo.

```csharp
public class OverlayManagerCallback : IOverlayManagerElement
```

**Evento:**

```csharp
public event EventHandler<OverlayManagerCallbackEventArgs> OnDraw;
```

**Ejemplo:**

```csharp
var callback = new OverlayManagerCallback();
callback.OnDraw += (sender, e) => {
    var ctx = e.Context;
    ctx.SetSourceRGB(1, 0, 0);
    ctx.Arc(200, 200, 50, 0, 2 * Math.PI);
    ctx.Fill();
};
overlayManager.Video_Overlay_Add(callback);
```

### OverlayManagerGroup

Agrupa múltiples superposiciones para gestión sincronizada del ciclo de vida. Esto es especialmente útil cuando necesitas precargar múltiples superposiciones de video e iniciarlas exactamente al mismo tiempo.

```csharp
public class OverlayManagerGroup : IOverlayManagerElement, IDisposable
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Overlays` | `List<IOverlayManagerElement>` | Lista vacía | Superposiciones en este grupo (solo lectura) |

> **Nota**: Las propiedades estándar de `IOverlayManagerElement` (`Opacity`, `Rotation`, `ZIndex`, `Shadow`) están presentes en el grupo pero no se aplican a nivel de grupo — se usan las propiedades individuales de cada superposición dentro del grupo para el renderizado.

**Métodos:**

- `Add(IOverlayManagerElement overlay)` - Agregar una superposición al grupo. Lanza `InvalidOperationException` si ya está inicializado.
- `Remove(IOverlayManagerElement overlay)` - Eliminar una superposición del grupo. Lanza `InvalidOperationException` si ya está inicializado.
- `Initialize()` - Precargar todas las superposiciones `OverlayManagerVideo` en el grupo a estado PAUSED. Retorna `true` si todas tuvieron éxito. Otros tipos de superposición (Decklink, NDI) deben inicializarse manualmente.
- `Play()` - Iniciar todas las superposiciones `OverlayManagerVideo` sincrónicamente. Debe llamar `Initialize()` primero. Otros tipos de superposición de video deben llamar `Play()` individualmente.
- `Pause()` - Pausar todas las superposiciones `OverlayManagerVideo` en el grupo.
- `Stop()` - Detener todas las superposiciones `OverlayManagerVideo` en el grupo.
- `GetRenderableOverlays()` - Retorna todas las superposiciones habilitadas en el grupo (uso interno).
- `Dispose()` - Detener y liberar todas las superposiciones en el grupo.

> **Importante**: No se pueden agregar ni eliminar superposiciones después de que se haya llamado `Initialize()`.

**¿Por qué agregar superposiciones que no son de video a un grupo?** Mientras que `Initialize()`, `Play()`, `Pause()` y `Stop()` solo controlan instancias de `OverlayManagerVideo`, agregar otros tipos de superposición (Decklink, NDI, texto, imágenes) a un grupo proporciona agrupación organizativa para renderizado y liberación centralizada de recursos — `Dispose()` limpia **todas** las superposiciones del grupo independientemente de su tipo.

**Ejemplo - Grupo Sincronizado de Video + Decklink:**

```csharp
// Crear un grupo para superposiciones que deben iniciar simultáneamente
var group = new OverlayManagerGroup("SyncGroup");

// Agregar una superposición de archivo de video
var videoOverlay = new OverlayManagerVideo(
    source: "intro.mp4",
    x: 10, y: 10, width: 640, height: 360)
{
    Loop = true,
    ZIndex = 10
};
group.Add(videoOverlay);

// Agregar una superposición de captura Decklink
var decklinkSettings = new DecklinkVideoSourceSettings(deviceNumber);
decklinkSettings.Mode = DecklinkMode.HD1080p2997;

var decklinkOverlay = new OverlayManagerDecklinkVideo(
    settings: decklinkSettings,
    x: 660, y: 10, width: 640, height: 360)
{
    ZIndex = 11
};

// Inicializar Decklink manualmente (el grupo maneja solo OverlayManagerVideo)
decklinkOverlay.Initialize(autoStart: false);
group.Add(decklinkOverlay);

// También puedes mezclar superposiciones que no son de video
var label = new OverlayManagerText("Camera 1", 10, 380);
label.Font.Size = 14;
label.Color = SKColors.White;
group.Add(label);

// Agregar grupo al overlay manager
overlayManager.Video_Overlay_Add(group);

// Iniciar todas las superposiciones OverlayManagerVideo del grupo
group.Play();
// Decklink debe iniciarse por separado (group.Play() solo maneja OverlayManagerVideo)
decklinkOverlay.Play();

// Más tarde, pausar/detener todas a la vez
group.Pause();
group.Stop();

// Limpiar
group.Dispose();
```

## Efectos de Transformación de Video

El OverlayManagerBlock soporta efectos avanzados de transformación de video que modifican todo el cuadro de video, no solo las superposiciones encima.

### OverlayManagerZoom

Aplica un efecto de zoom al cuadro de video, escalando desde un punto central.

```csharp
public class OverlayManagerZoom : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `ZoomFactor` | `double` | `1.0` | Nivel de zoom (1.0 = sin zoom, 2.0 = 2x zoom) |
| `CenterX` | `double` | `0.5` | Centro horizontal (0.0-1.0, relativo al cuadro) |
| `CenterY` | `double` | `0.5` | Centro vertical (0.0-1.0, relativo al cuadro) |
| `InterpolationMode` | `OverlayManagerInterpolationMode` | `Bilinear` | Balance calidad/velocidad |

**Ejemplo:**

```csharp
// Crear zoom 1.5x centrado en el cuadro
var zoom = new OverlayManagerZoom
{
    ZoomFactor = 1.5,
    CenterX = 0.5,
    CenterY = 0.5,
    Name = "VideoZoom"
};
zoom.ZIndex = -1000; // Procesar antes de otras superposiciones
overlayManager.Video_Overlay_Add(zoom);
```

### OverlayManagerPan

Aplica un efecto de panorámica (traslación) al cuadro de video.

```csharp
public class OverlayManagerPan : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `OffsetX` | `double` | `0.0` | Desplazamiento horizontal en píxeles |
| `OffsetY` | `double` | `0.0` | Desplazamiento vertical en píxeles |
| `InterpolationMode` | `OverlayManagerInterpolationMode` | `Bilinear` | Balance calidad/velocidad |

**Ejemplo:**

```csharp
// Panorámica de video 100 píxeles a la derecha y 50 hacia abajo
var pan = new OverlayManagerPan
{
    OffsetX = 100,
    OffsetY = 50,
    Name = "VideoPan"
};
pan.ZIndex = -1000; // Procesar antes de otras superposiciones
overlayManager.Video_Overlay_Add(pan);
```

### OverlayManagerFade

Aplica un efecto de desvanecimiento a todo el cuadro de video.

```csharp
public class OverlayManagerFade : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `FadeMode` | `OverlayManagerFadeMode` | `None` | Tipo de fade (None, FadeIn, FadeOut) |
| `StartTime` | `TimeSpan` | `Zero` | Cuándo comienza el efecto de fade |
| `Duration` | `TimeSpan` | `1 segundo` | Duración del fade |
| `MinOpacity` | `double` | `0.0` | Opacidad mínima (totalmente desvanecido) |
| `MaxOpacity` | `double` | `1.0` | Opacidad máxima (totalmente visible) |

### OverlayManagerSqueezeback

Crea un efecto "squeezeback" estilo broadcast donde el video se escala a un rectángulo personalizado y una imagen superpuesta (típicamente PNG con transparencia alfa) se dibuja encima. Se usa comúnmente para tercios inferiores, marcos y gráficos de transmisión.

```csharp
public class OverlayManagerSqueezeback : IOverlayManagerElement
```

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `BackgroundImageFilename` | `string` | - | Ruta de imagen superpuesta (PNG con alfa recomendado) |
| `VideoRect` | `Rect` | - | Dónde se escala y posiciona el video |
| `BackgroundRect` | `Rect` | `null` | Posición de imagen superpuesta (null = cuadro completo) |
| `VideoOnTop` | `bool` | `false` | Orden de capas (false = video abajo, imagen encima) |
| `VideoOpacity` | `double` | `1.0` | Opacidad de capa de video |
| `BackgroundOpacity` | `double` | `1.0` | Opacidad de imagen superpuesta |

**Propiedades de Animación:**

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `VideoAnimationEnabled` | `bool` | Habilitar animación de posición de video |
| `VideoAnimationStartRect` | `Rect` | Posición inicial de animación |
| `VideoAnimationTargetRect` | `Rect` | Posición final de animación |
| `VideoAnimationStartTimeMs` | `double` | Tiempo de inicio de animación (ms) |
| `VideoAnimationDurationMs` | `double` | Duración de animación (ms) |
| `VideoAnimationEasing` | `OverlayManagerPanEasing` | Función de suavizado |

**Propiedades de Desvanecimiento:**

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `VideoFadeMode` | `OverlayManagerFadeMode` | Tipo de fade de video |
| `VideoFadeStartTimeMs` | `double` | Tiempo de inicio de fade de video |
| `VideoFadeDurationMs` | `double` | Duración de fade de video |
| `BackgroundFadeMode` | `OverlayManagerFadeMode` | Tipo de fade de fondo |
| `BackgroundFadeStartTimeMs` | `double` | Tiempo de inicio de fade de fondo |
| `BackgroundFadeDurationMs` | `double` | Duración de fade de fondo |

**Ejemplo - Squeezeback Básico:**

```csharp
// Crear squeezeback con video en esquina inferior derecha
var squeezeback = new OverlayManagerSqueezeback
{
    BackgroundImageFilename = "marco.png", // PNG con centro transparente
    VideoRect = new Rect(960, 540, 1920, 1080), // Cuadrante inferior derecho
    Name = "Squeezeback"
};
squeezeback.ZIndex = -2000; // Procesar primero
overlayManager.Video_Overlay_Add(squeezeback);
```

**Ejemplo - Squeezeback Animado:**

```csharp
// Obtener posición actual de reproducción
var position = await pipeline.Position_GetAsync();

// Animar video de pantalla completa a esquina en 2 segundos
var squeezeback = overlayManager.Video_Overlay_GetByName("Squeezeback") as OverlayManagerSqueezeback;
squeezeback.AnimateVideo(
    startRect: new Rect(0, 0, 1920, 1080),      // Pantalla completa
    targetRect: new Rect(1280, 720, 1920, 1080), // Esquina inferior derecha
    startTime: position,
    duration: TimeSpan.FromSeconds(2),
    easing: OverlayManagerPanEasing.EaseInOut
);
```

**Métodos de Conveniencia en OverlayManagerBlock:**

```csharp
// Agregar squeezeback
var element = overlayManager.Video_Overlay_AddSqueezeback(
    backgroundImageFilename: "marco.png",
    videoRect: new Rect(960, 540, 1920, 1080),
    backgroundRect: null,  // Cuadro completo
    name: "Squeezeback"
);

// Actualizar posición de video
overlayManager.Video_Overlay_Squeezeback_UpdateVideoPosition("Squeezeback", newRect);

// Animar video
overlayManager.Video_Overlay_Squeezeback_AnimateVideo(
    "Squeezeback", startRect, targetRect, startTime, duration, easing);

// Controles de fade
overlayManager.Video_Overlay_Squeezeback_VideoFadeIn("Squeezeback", startTime, duration);
overlayManager.Video_Overlay_Squeezeback_VideoFadeOut("Squeezeback", startTime, duration);

// Orden de capas
overlayManager.Video_Overlay_Squeezeback_SetVideoOnTop("Squeezeback");
overlayManager.Video_Overlay_Squeezeback_SetBackgroundOnTop("Squeezeback");
```

### Funciones de Suavizado (Easing)

Opciones de suavizado de animación disponibles via `OverlayManagerPanEasing`:

| Valor | Descripción |
|-------|-------------|
| `Linear` | Velocidad constante |
| `EaseIn` | Inicio lento, final rápido |
| `EaseOut` | Inicio rápido, final lento |
| `EaseInOut` | Inicio y final lentos |
| `EaseInCubic` | Inicio lento cúbico |
| `EaseOutCubic` | Final lento cúbico |
| `EaseInOutCubic` | Inicio y final lentos cúbicos |

### Modos de Interpolación

Balance calidad/rendimiento para escalado via `OverlayManagerInterpolationMode`:

| Valor | Descripción |
|-------|-------------|
| `Nearest` | Más rápido, bordes pixelados |
| `Bilinear` | Buen balance calidad/velocidad |
| `Gaussian` | Mayor calidad, más lento |

## Configuración de Sombras

Configura sombras paralelas para elementos de superposición:

```csharp
public class OverlayManagerShadowSettings
```

| Propiedad | Tipo | Rango | Predeterminado | Descripción |
|-----------|------|-------|----------------|-------------|
| `Enabled` | `bool` | - | `false` | Habilitar sombras |
| `Depth` | `double` | 0-30 | `5.0` | Distancia de desplazamiento de sombra |
| `Direction` | `double` | 0-360° | `45.0` | Dirección de sombra |
| `Opacity` | `double` | 0-1 | `0.5` | Transparencia de sombra |
| `BlurRadius` | `double` | 0-10 | `2.0` | Cantidad de desenfoque de sombra |
| `Color` | `SKColor` | - | `Black` | Color de sombra |

**Referencia de Dirección:**

- 0° = Derecha
- 90° = Abajo
- 180° = Izquierda
- 270° = Arriba

## Fondos de Texto

Las superposiciones de texto pueden tener varias formas de fondo:

### OverlayManagerBackgroundRectangle

```csharp
var text = new OverlayManagerText("Info", 100, 100);
text.Background = new OverlayManagerBackgroundRectangle {
    Color = SKColors.Black.WithAlpha(128),
    Fill = true,
    Margin = new Thickness(5, 3, 5, 3)
};
```

### OverlayManagerBackgroundSquare

Similar a rectángulo pero mantiene relación de aspecto cuadrada.

### OverlayManagerBackgroundImage

Usa una imagen como fondo de texto con modos de estiramiento.

### OverlayManagerBackgroundTriangle/Star

Fondos de forma personalizada para texto.

## Configuración de Fuente

Configura la apariencia del texto:

```csharp
public class FontSettings
```

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `Name` | `string` | Nombre de familia de fuente |
| `Size` | `int` | Tamaño de fuente en puntos |
| `Style` | `FontStyle` | Normal, Italic, Oblique |
| `Weight` | `FontWeight` | Normal, Bold, Light, etc. |

## Ejemplo Completo

```csharp
// Crear pipeline y bloques
var pipeline = new MediaBlocksPipeline();
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(videoUri));
var overlayManager = new OverlayManagerBlock();
var videoRenderer = new VideoRendererBlock(pipeline, videoView);

// Conectar pipeline
pipeline.Connect(fileSource.VideoOutput, overlayManager.Input);
pipeline.Connect(overlayManager.Output, videoRenderer.Input);

// Agregar marca de agua con logo
var logo = new OverlayManagerImage("logo.png", 10, 10);
logo.Opacity = 0.5;
logo.ZIndex = 10; // Encima
overlayManager.Video_Overlay_Add(logo);

// Agregar marca de tiempo
var timestamp = new OverlayManagerDateTime();
timestamp.X = 10;
timestamp.Y = pipeline.Height - 30;
timestamp.Font.Size = 16;
timestamp.Color = SKColors.White;
timestamp.Shadow = new OverlayManagerShadowSettings(true);
overlayManager.Video_Overlay_Add(timestamp);

// Agregar título animado (aparece después de 5 segundos)
var title = new OverlayManagerText("¡Bienvenido!", 100, 100);
title.Font.Size = 72;
title.Color = SKColors.Yellow;
title.StartTime = TimeSpan.FromSeconds(5);
title.EndTime = TimeSpan.FromSeconds(10);
title.Rotation = -10; // Ligera inclinación
title.Background = new OverlayManagerBackgroundRectangle {
    Color = SKColors.DarkBlue,
    Fill = true
};
overlayManager.Video_Overlay_Add(title);

// Iniciar reproducción
await pipeline.StartAsync();

// Actualizar superposiciones dinámicamente
title.Text = "¡Texto Actualizado!";
overlayManager.Video_Overlay_Update(title);
```

## Aplicación de Ejemplo

Para un ejemplo funcional completo que demuestra todos los tipos de superposición, consulta:

- [Overlay Manager Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Overlay%20Manager%20Demo)

## Consideraciones de Rendimiento

1. **Ordenamiento Z-Index**: Los elementos se ordenan por Z-index antes del renderizado. Usa valores apropiados para minimizar sobrecarga de ordenamiento.

2. **Formatos de Imagen**: Usa imágenes formato RGBA8888 cuando sea posible para evitar conversión de color.

3. **Efectos de Sombra**: Las sombras con desenfoque son computacionalmente costosas. Úsalas con moderación para aplicaciones en tiempo real.

4. **Actualizaciones**: Usa `Video_Overlay_Update()` para elementos existentes en lugar de operaciones de eliminar/agregar.

5. **Gestión de Recursos**: Libera superposiciones de imagen, GIF y secuencia de imágenes cuando ya no se necesiten para liberar memoria.

6. **Superposiciones de Video**: Cada superposición `OverlayManagerVideo` ejecuta su propio pipeline interno de GStreamer. Limita la cantidad de superposiciones de video simultáneas para evitar uso excesivo de CPU y memoria.

7. **Superposiciones de Controles WPF**: Valores más altos de `RefreshRate` aumentan el uso de CPU. Usa la tasa de refresco mínima necesaria para actualizaciones visuales fluidas — 15 fps es suficiente para la mayoría del contenido estático o que cambia lentamente.

8. **Superposiciones WebView2**: Cada superposición `OverlayManagerWebView2Video` ejecuta su propio pipeline de renderizado interno con un navegador fuera de pantalla. Limita la cantidad de superposiciones WebView2 simultáneas para evitar uso excesivo de CPU, GPU y memoria.

9. **Grupos de Superposiciones**: Usa `OverlayManagerGroup` para precargar superposiciones de video. Esto evita tiempos de inicio escalonados cuando múltiples superposiciones de video necesitan comenzar simultáneamente.

## Notas de Plataforma

- **Windows**: Soporta System.Drawing.Bitmap además de SkiaSharp
- **Windows (WPF)**: Soporta `OverlayManagerWPFControl` para renderizar elementos visuales WPF como superposiciones. Requiere target de compilación `NET_WINDOWS`.
- **Windows (WebView2)**: Soporta `OverlayManagerWebView2Video` para renderizar contenido web en vivo (HTML/CSS/JS) como superposiciones. Requiere el Runtime de Microsoft WebView2 y el plugin GStreamer WebView2 (`webview2src`).
- **iOS**: La fuente predeterminada es "System-ui"
- **Android**: La fuente predeterminada es "System-ui"
- **Linux/macOS**: Enumera fuentes disponibles en tiempo de ejecución

## Seguridad de Hilos

El overlay manager usa bloqueo interno para operaciones seguras entre hilos. Puedes agregar, eliminar o actualizar superposiciones de forma segura desde cualquier hilo.

## Solución de Problemas

1. **Superposición no visible**: Verifica la propiedad `Enabled`, `StartTime`/`EndTime` y ordenamiento `ZIndex`.

2. **El texto aparece borroso**: Asegura que el tamaño de fuente sea apropiado para la resolución del video.

3. **Uso de memoria**: Libera superposiciones de imagen/GIF/secuencia de imágenes no usadas y usa tamaños de imagen apropiados.

4. **La superposición de video no muestra cuadros**: Asegúrate de que `Initialize()` retorne `true` antes de agregar al overlay manager. Verifica que la ruta del archivo fuente sea válida y accesible, y que GStreamer tenga los codecs necesarios.

5. **La superposición WPF no se actualiza**: Verifica que `RefreshRate` sea apropiado para tu contenido. Usa `InvokeOnUIThread()` para todas las modificaciones de elementos WPF para evitar excepciones de hilos cruzados.

6. **La superposición WebView2 no renderiza**: Asegúrate de que el Runtime de Microsoft WebView2 esté instalado en la máquina de destino. Verifica que `Initialize()` retorne `true` antes de agregar al overlay manager. El plugin GStreamer WebView2 (`webview2src`) debe estar disponible.

7. **Las superposiciones del grupo no inician juntas**: Asegúrate de que todas las superposiciones se agreguen al grupo antes de llamar `Initialize()`. No se pueden agregar superposiciones después de la inicialización.

## Preguntas Frecuentes

### ¿Cómo superpongo un archivo de video encima de otro video en C#?

Usa `OverlayManagerVideo` para reproducir un archivo de video o URL de streaming como superposición. Crea una instancia con la ruta fuente, posición y dimensiones, luego llama `Initialize()` y agrégalo al `OverlayManagerBlock`. Tienes control completo de reproducción con los métodos `Play()`, `Pause()`, `Stop()` y `Seek()`, además de salida de audio opcional. Consulta la sección [OverlayManagerVideo](#overlaymanagervideo) para ejemplos.

### ¿Puedo usar controles WPF como superposiciones de video en vivo?

Sí. `OverlayManagerWPFControl` renderiza cualquier `FrameworkElement` de WPF como superposición de video, incluyendo controles con animaciones Storyboard, data binding y árboles visuales complejos. El elemento se captura periódicamente a una tasa de refresco configurable (1-60 fps). Esto es solo para Windows y requiere el target de compilación `NET_WINDOWS`. Usa el método de conveniencia `Video_Overlay_AddWPFControl()` para la configuración más simple. Consulta la sección [OverlayManagerWPFControl](#overlaymanagerwpfcontrol-solo-windows).

### ¿Cómo sincronizo múltiples superposiciones de video para que inicien al mismo tiempo?

Usa `OverlayManagerGroup` para agrupar superposiciones que necesitan ciclo de vida coordinado. Agrega todas las superposiciones al grupo antes de llamar `Initialize()`, que precarga las superposiciones de video a estado PAUSED. Luego llama `Play()` para iniciarlas todas simultáneamente. Esto es especialmente útil para composiciones multi-cámara. Consulta la sección [OverlayManagerGroup](#overlaymanagergroup).

### ¿Puedo reproducir audio desde una superposición de archivo de video?

Sí. Establece la propiedad `AudioOutput` en `OverlayManagerVideo` a un dispositivo de salida de audio antes de llamar `Initialize()`. Controla el volumen con `AudioOutput_Volume` (0.0-1.0+) y silencia con `AudioOutput_Mute`. Si `AudioOutput` es null (valor predeterminado), el audio del archivo de video se descarta.

### ¿Qué tipos de superposición soporta OverlayManagerBlock?

El OverlayManagerBlock soporta: texto (`OverlayManagerText`), fecha/hora (`OverlayManagerDateTime`), texto desplazable (`OverlayManagerScrollingText`), imágenes (`OverlayManagerImage`), GIFs animados (`OverlayManagerGIF`), secuencias de imágenes (`OverlayManagerImageSequence`), gráficos SVG (`OverlayManagerSVG`), formas (rectángulo, círculo, triángulo, estrella, línea), archivos de video/URLs (`OverlayManagerVideo`), tarjetas de captura Decklink (`OverlayManagerDecklinkVideo`), fuentes de red NDI (`OverlayManagerNDIVideo`), contenido web WebView2 (`OverlayManagerWebView2Video`, solo Windows), controles WPF (`OverlayManagerWPFControl`, solo Windows), grupos de superposiciones (`OverlayManagerGroup`), dibujo personalizado con Cairo (`OverlayManagerCallback`), y efectos de transformación de video (zoom, panorámica, desvanecimiento, squeezeback).

### ¿Puedo renderizar contenido web en vivo como superposición de video?

Sí. `OverlayManagerWebView2Video` renderiza cualquier página web (HTML, CSS, JavaScript) como superposición de video usando Microsoft WebView2. Puedes mostrar dashboards, contenido web animado, tickers o cualquier contenido renderizado por navegador. Soporta inyección de JavaScript después de la navegación para personalizar la página mostrada. Esto es solo para Windows y requiere el Runtime de Microsoft WebView2. Consulta la sección [OverlayManagerWebView2Video](#overlaymanagerwebview2video-solo-windows).
