---
title: Guía de uso de OverlayManagerBlock
description: Usa OverlayManagerBlock para agregar superposiciones a contenido de video con funciones para gestionar capas, sombras, rotación y opacidad en tiempo real.
---

# Guía de Uso del Bloque Overlay Manager

## Descripción General

El `OverlayManagerBlock` es un componente poderoso de MediaBlocks que proporciona composición dinámica de superposiciones de video multicapa y gestión. Te permite agregar varios elementos de superposición (imágenes, texto, formas, animaciones) sobre el contenido de video con actualizaciones en tiempo real, gestión de capas y características avanzadas como sombras, rotación y control de opacidad.

## Características Principales

- **Múltiples Tipos de Superposición**: Texto, texto desplazable, imágenes, GIFs, SVG, formas (rectángulos, círculos, triángulos, estrellas, líneas), video en vivo (NDI, Decklink)
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

### Superposiciones de Video en Vivo

El Overlay Manager soporta fuentes de video en vivo como superposiciones, permitiéndote componer video en tiempo real desde tarjetas de captura Decklink o fuentes de red NDI.

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

**Métodos Comunes para Superposiciones de Video en Vivo:**

- `Initialize(bool autoStart)` - Inicializar el pipeline de captura de video
- `Play()` - Iniciar o reanudar captura de video
- `Pause()` - Pausar captura de video
- `Stop()` - Detener captura de video
- `Dispose()` - Limpiar recursos

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

## Consideraciones de Rendimiento

1. **Ordenamiento Z-Index**: Los elementos se ordenan por Z-index antes del renderizado. Usa valores apropiados para minimizar sobrecarga de ordenamiento.

2. **Formatos de Imagen**: Usa imágenes formato RGBA8888 cuando sea posible para evitar conversión de color.

3. **Efectos de Sombra**: Las sombras con desenfoque son computacionalmente costosas. Úsalas con moderación para aplicaciones en tiempo real.

4. **Actualizaciones**: Usa `Video_Overlay_Update()` para elementos existentes en lugar de operaciones de eliminar/agregar.

5. **Gestión de Recursos**: Libera superposiciones de imagen y GIF cuando ya no se necesiten para liberar memoria.

## Notas de Plataforma

- **Windows**: Soporta System.Drawing.Bitmap además de SkiaSharp
- **iOS**: La fuente predeterminada es "System-ui"
- **Android**: La fuente predeterminada es "System-ui"
- **Linux/macOS**: Enumera fuentes disponibles en tiempo de ejecución

## Seguridad de Hilos

El overlay manager usa bloqueo interno para operaciones seguras entre hilos. Puedes agregar, eliminar o actualizar superposiciones de forma segura desde cualquier hilo.

## Solución de Problemas

1. **Superposición no visible**: Verifica la propiedad `Enabled`, `StartTime`/`EndTime` y ordenamiento `ZIndex`.

2. **El texto aparece borroso**: Asegura que el tamaño de fuente sea apropiado para la resolución del video.

3. **Uso de memoria**: Libera superposiciones de imagen/GIF no usadas y usa tamaños de imagen apropiados.
