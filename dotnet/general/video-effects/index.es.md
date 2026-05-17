---
title: Efectos de video .NET — superposiciones y procesamiento
description: Implementa efectos de video profesionales, superposiciones de texto/imagen y procesamiento personalizado con herramientas potentes para aplicaciones .NET.
sidebar_label: Efectos de video y procesamiento

order: 15
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing
primary_api_classes:
  - VideoEffect
  - GPUVideoEffect
  - VideoBalanceVideoEffect
  - ColorEffectsVideoEffect
  - GrayscaleVideoEffect

---

# Efectos de video y procesamiento para aplicaciones .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Nuestros SDK .Net ofrecen a los desarrolladores dos implementaciones distintas de efectos de video para cubrir tus requisitos de plataforma y rendimiento:

### Efectos clásicos (solo Windows)
Disponibles en **VideoCaptureCore**, **MediaPlayerCore** y **VideoEditCore**:
- Efectos basados en CPU (`VideoEffect*`)
- Efectos acelerados por GPU (`GPUVideoEffect*`) usando DirectX
- Efectos con IA (`Maxine*`) aprovechando la tecnología NVIDIA
- Solo Windows, optimizados para rendimiento de escritorio

### Efectos multiplataforma
Disponibles en **VideoCaptureCoreX**, **MediaPlayerCoreX**, **VideoEditCoreX** y **Media Blocks SDK**:
- Implementación multiplataforma (clases `*VideoEffect`)
- Funciona en Windows, Linux, macOS, Android e iOS
- Aceleración hardware mediante plugins multimedia específicos de la plataforma
- Sistema de superposición extendido y funciones avanzadas
- Más adecuado para despliegue móvil y multiplataforma

## Referencia completa de efectos

**[Ver la referencia completa de efectos de video →](effects-reference.md)**

Nuestra referencia exhaustiva proporciona información detallada sobre los más de 100 efectos de video disponibles en ambas implementaciones:
- **Efectos clásicos** para aplicaciones solo Windows
- **Efectos multiplataforma** para compatibilidad universal
- Parámetros de efectos, ejemplos de uso y disponibilidad por SDK
- Funciones y capacidades específicas de cada plataforma

## Categorías de efectos de video disponibles

Las siguientes categorías de efectos están disponibles tanto en las implementaciones Clásicas como Multiplataforma (cuando corresponda):

### Ajuste de color e imagen

* **Brillo, oscuridad y contraste** — controla la luminancia y el rango tonal
  - Clásico: `VideoEffect*` / `GPUVideoEffect*`
  - Multiplataforma: `VideoBalanceVideoEffect`, `GammaVideoEffect`
* **Saturación y gradación de color** — ajusta la intensidad de color y aplica correcciones de color
  - Clásico: `VideoEffectSaturation` / `GPUVideoEffectSaturation`
  - Multiplataforma: `VideoBalanceVideoEffect`, `ColorEffectsVideoEffect`, `LUTVideoEffect`
* **Luminosidad** — ajuste de brillo basado en HSL que preserva las relaciones de color
  - Clásico: `VideoEffectLightness`
* **Filtros de color** — aísla o realza canales de color específicos (Rojo, Verde, Azul)
  - Clásico: `VideoEffectRed/Green/Blue`, `VideoEffectFilterRed/Green/Blue`

### Efectos creativos y artísticos

* **Escala de grises y monocromo** — conversiones a blanco y negro con tintado opcional
  - Clásico: `VideoEffectGrayscale` / `GPUVideoEffectGrayscale`
  - Multiplataforma: `GrayscaleVideoEffect`, `ColorEffectsVideoEffect`
* **Inversión y solarización** — efectos de negativo fotográfico e inversión parcial
  - Clásico: `VideoEffectInvert` / `GPUVideoEffectInvert`, `VideoEffectSolorize`
* **Película antigua y visión nocturna** — simulaciones de película vintage y cámara de vigilancia
  - Clásico: `GPUVideoEffectOldMovie`, `GPUVideoEffectNightVision`
  - Multiplataforma: `AgingVideoEffect`, `OpticalAnimationBWVideoEffect`
* **Relieve y contorno** — detección de bordes y efectos de relieve
  - Clásico: `GPUVideoEffectEmboss`, `GPUVideoEffectContour`
  - Multiplataforma: `EdgeVideoEffect`
* **Pixelado y mosaico** — efectos estilizados de bloques y mosaicos
  - Clásico: `GPUVideoEffectPixelate`, `VideoEffectMosaic`
  - Multiplataforma: varios efectos de distorsión

### Mejora de imagen

* **Nitidez** — realza la definición de bordes y los detalles finos
  - Clásico: `VideoEffectSharpen` / `GPUVideoEffectSharpen`
* **Desenfoque y suavizado** — reduce el detalle, el ruido y crea efectos de enfoque suave
  - Clásico: `VideoEffectBlur` / `GPUVideoEffectBlur`, `VideoEffectSmooth`
  - Multiplataforma: `GaussianBlurVideoEffect`, `SmoothVideoEffect`
* **Reducción de ruido** — algoritmos adaptativos, CAST y de eliminación de ruido mosquito
  - Clásico: `VideoEffectDenoiseAdaptive/CAST/Mosquito` / `GPUVideoEffectDenoise`
* **Desentrelazado** — convierte video entrelazado a progresivo (métodos Blend, CAVT, Triangle)
  - Clásico: `VideoEffectDeinterlaceBlend/CAVT/Triangle` / `GPUVideoEffectDeinterlaceBlend`
  - Multiplataforma: `DeinterlaceVideoEffect`, `AutoDeinterlaceSettings`

### Transformaciones geométricas

* **Rotación** — rota a cualquier ángulo con opciones de estirar o no recortar
  - Clásico: `VideoEffectRotate`
  - Multiplataforma: `FlipRotateVideoEffect`
* **Volteo y espejado** — volteo y espejado horizontal y vertical
  - Clásico: `VideoEffectFlip*`, `VideoEffectMirror*`
  - Multiplataforma: `FlipRotateVideoEffect`
* **Zoom y paneo** — enfoca regiones específicas con control de escala
  - Clásico: `VideoEffectZoom`, `VideoEffectPan`
  - Multiplataforma: `ResizeVideoEffect`, `CropVideoEffect`, `AspectRatioCropVideoEffect`, `BoxVideoEffect`
* **Procesamiento de video 360°** — proyecciones equirrectangular y cubemap para video VR
  - Clásico: `GPUVideoEffectEquirectangular360`, `GPUVideoEffectEquiangularCubemap360`

### Efectos de distorsión artística (solo multiplataforma)

La implementación multiplataforma incluye amplios efectos de distorsión artística no disponibles en la versión Clásica:

* **Efectos de lente** — Ojo de pez, Esfera, Twirl, Bulge, Pinch
* **Efectos de patrón** — Caleidoscopio, Mármol, Quark, Dice
* **Efectos de movimiento** — Moving Blur, Moving Echo, Moving Zoom Echo
* **Efectos de deformación** — Ripple, Water Ripple, Warp, Stretch, Tunnel
* **Efectos de perspectiva** — Pseudo3D, Square, Circle, Perspective
* **Transiciones SMPTE** — Limpiezas y transiciones de estilo broadcast profesional

### Efectos de transición

* **Fundido de entrada/salida** — transiciones suaves a/desde negro
  - Clásico: `VideoEffectFadeIn`, `VideoEffectFadeOut`
* **Transiciones animadas** — interpolación de valores de efecto basada en tiempo para cambios dinámicos
  - Clásico: compatible mediante el parámetro ValueStop
  - Multiplataforma: basado en tiempo con StartTime/StopTime

## Capacidades de superposición

* [**Superposición de texto**](text-overlay.md) — agrega texto personalizable con control sobre fuente, tamaño, color, rotación y animación
  - Clásico: `VideoEffectTextLogo`, `VideoEffectScrollingTextLogo`
  - Multiplataforma: `TextOverlayVideoEffect`, `OverlayManagerText`, `OverlayManagerScrollingText`, `OverlayManagerDateTime`
* [**Superposición de imagen**](image-overlay.md) — incorpora logotipos, marcas de agua y elementos gráficos con soporte de transparencia
  - Clásico: `VideoEffectImageLogo`
  - Multiplataforma: `ImageOverlayVideoEffect`, `ImageOverlayCairoVideoEffect`, `OverlayManagerImage`, `OverlayManagerGIF`
  * Soporte para formatos PNG, JPG, BMP, GIF animado
  * Soporte de transparencia y canal alfa
  * Control de posicionamiento y alineación

### Funciones avanzadas de superposición (solo multiplataforma)

La implementación multiplataforma incluye un extenso sistema OverlayManager:

* **Superposiciones de video** — Picture-in-picture, Decklink, fuentes de video NDI
* **Superposiciones de formas** — círculos, rectángulos, estrellas, triángulos, líneas
* **Capas de fondo** — imágenes, colores sólidos, formas geométricas
* **Animaciones** — efectos de paneo, zoom, fundido, squeezeback
* **Gestión de grupos** — control sincronizado de varias superposiciones
* **Soporte SVG** — superposiciones de gráficos vectoriales
* **Códigos QR** — genera y superpone códigos QR

## Chroma Key y detección de movimiento (solo multiplataforma)

* **ChromaKeySettings** — keying de pantalla verde / azul para eliminación de fondo
* **MotionDetectionProcessor** — detección de movimiento en tiempo real con sensibilidad configurable

## Mejora de video con IA (solo Clásico)

Efectos NVIDIA Maxine con IA (requiere GPU NVIDIA RTX con soporte del SDK Maxine):

* **AI Denoise** — reducción inteligente de ruido preservando los detalles finos
* **Reducción de artefactos** — elimina artefactos de compresión y degradación de video
* **Super Resolution** — escalado por IA para mejorar la resolución
* **AI Green Screen** — eliminación de fondo sin necesidad de pantalla verde física
* **Upscaling** — mejora avanzada de calidad mediante IA

## Funciones de procesamiento de video
  
### Procesamiento en tiempo real

* Aplica efectos durante la captura, reproducción o edición de video
* Encadena varios efectos para pipelines de procesamiento complejos
* Aceleración GPU para procesamiento en tiempo real de alta resolución (Clásico)
* Aceleración hardware para rendimiento multiplataforma (Multiplataforma)
* Reserva CPU para compatibilidad universal

### Procesamiento avanzado

* Aplicación de efectos basada en línea de tiempo (tiempos de inicio/fin)
* Transiciones animadas de efectos (interpolación entre valores — Clásico)
* Control dinámico de efectos durante la reproducción
* [**Capturador de muestras de video**](video-sample-grabber.md) — extrae fotogramas y procesa datos de video en tiempo real

## Optimización del rendimiento

### Efectos clásicos (solo Windows)

- **Efectos GPU** (`GPUVideoEffect*`): mejor rendimiento para video de alta resolución en Windows
- **Efectos CPU** (`VideoEffect*`): compatibilidad universal en Windows, rendimiento moderado
- **Efectos IA** (`Maxine*`): calidad de última generación, requiere GPU NVIDIA RTX
- Usa efectos acelerados por GPU para procesamiento HD/4K en tiempo real
- Aceleración hardware DirectX para rendimiento óptimo en Windows

### Efectos multiplataforma

- **Aceleración hardware**: varía por plataforma mediante plugins multimedia
  - Windows: DirectX, DXVA
  - Linux: VA-API, VDPAU, OpenGL
  - macOS: VideoToolbox, Metal
  - Android/iOS: códecs hardware
- **Rendimiento**: generalmente bueno en todas las plataformas
- **Optimización móvil**: más adecuados para móvil que los efectos Clásicos
- **Sistema de superposición**: renderizado eficiente de múltiples superposiciones

### Buenas prácticas

* Elige efectos GPU/acelerados para video de alta resolución
* Considera las combinaciones de efectos y su impacto en el rendimiento
* Aplica efectos limitados en el tiempo cuando sea apropiado para reducir la sobrecarga de procesamiento
* Prueba el rendimiento en las configuraciones de hardware objetivo
* Usa efectos multiplataforma para despliegue multiplataforma

## Soporte de plataformas

### Efectos clásicos

| Plataforma | Efectos CPU | Efectos GPU | IA Maxine | SDK |
|------------|-------------|-------------|-----------|-----|
| Windows Desktop | ✅ Completo | ✅ Completo | ✅ GPU RTX | VideoCaptureCore<br/>MediaPlayerCore<br/>VideoEditCore |
| Linux    | ❌ | ❌ | ❌ | N/A |
| macOS    | ❌ | ❌ | ❌ | N/A |
| Android  | ❌ | ❌ | ❌ | N/A |
| iOS      | ❌ | ❌ | ❌ | N/A |

### Efectos multiplataforma

| Plataforma | Soporte | Aceleración hardware | SDK |
|------------|---------|----------------------|-----|
| Windows  | ✅ Completo | ✅ DirectX, DXVA | VideoCaptureCoreX<br/>MediaPlayerCoreX<br/>VideoEditCoreX<br/>Media Blocks SDK |
| Linux    | ✅ Completo | ✅ VA-API, VDPAU, OpenGL | ✓ |
| macOS    | ✅ Completo | ✅ VideoToolbox, Metal | ✓ |
| Android  | ✅ Completo | ✅ Códecs hardware | ✓ |
| iOS      | ✅ Completo | ✅ VideoToolbox | ✓ |

## Métodos de integración

Nuestros SDK proporcionan dos APIs distintas para la integración de efectos de video según el motor SDK que utilices.

### Ejemplo de uso básico (efectos Clásicos)

```csharp
// Solo Windows: efecto basado en CPU
var bwEffect = new VideoEffectGrayscale(
    enabled: true,
    name: "BlackAndWhite"
);
videoCapture.Video_Effects_Add(bwEffect);

// Solo Windows: efecto acelerado por GPU
var brighten = new GPUVideoEffectBrightness(
    enabled: true,
    value: 180,
    name: "Brighten"
);
videoCapture.Video_Effects_Add(brighten);
```

### Ejemplo de uso básico (efectos multiplataforma)

```csharp
// Multiplataforma: efecto de escala de grises
var grayscale = new GrayscaleVideoEffect("bw_effect");
await videoCaptureX.Video_Effects_AddOrUpdateAsync(grayscale);

// Multiplataforma: balance de video (brillo, contraste, saturación)
var balance = new VideoBalanceVideoEffect("color_adjust")
{
    Brightness = 0.2,    // Rango: -1.0 a 1.0
    Contrast = 1.2,      // Rango: 0.0 a 2.0
    Saturation = 1.5,    // Rango: 0.0 a 2.0
    Hue = 0.0            // Rango: -1.0 a 1.0
};
await videoCaptureX.Video_Effects_AddOrUpdateAsync(balance);
```

### Ejemplo de efecto animado (Clásico)

```csharp
// Fundido a negro en 3 segundos (solo Windows)
var fadeOut = new VideoEffectDarkness(
    enabled: true,
    value: 128,        // Empieza en normal
    valueStop: 255,    // Termina en negro
    name: "FadeOut",
    startTime: TimeSpan.FromSeconds(57),
    stopTime: TimeSpan.FromSeconds(60)
);
mediaPlayer.Video_Effects_Add(fadeOut);
```

### Ejemplo de efecto con tiempo limitado (multiplataforma)

```csharp
// Aplica desenfoque gaussiano en un rango de tiempo específico (multiplataforma)
// Ctor: GaussianBlurVideoEffect(double sigma, string name = "gaussian_blur")
var blur = new GaussianBlurVideoEffect(5.0, "scene_blur")
{
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromSeconds(15)
};
await mediaPlayerX.Video_Effects_AddOrUpdateAsync(blur);
```

### Ejemplo avanzado de superposición (multiplataforma)

```csharp
// Superposición de texto compleja con sombra paralela (multiplataforma)
var textOverlay = new OverlayManagerText
{
    Text = "Breaking News",
    Font = new FontSettings
    {
        Name = "Arial",
        Size = 48,
        Weight = FontWeight.Bold
    },
    Color = SKColors.Yellow,
    X = 50,
    Y = 100,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.Black,
        BlurRadius = 5
    }
};
videoCaptureX.Video_Overlay_Add(textOverlay);
```

## Recursos de documentación

* **[Referencia completa de efectos](effects-reference.md)** — guía exhaustiva de todos los efectos disponibles
* **[Guía de superposición de texto](text-overlay.md)** — implementación detallada de superposiciones de texto
* **[Guía de superposición de imagen](image-overlay.md)** — técnicas de superposición de imágenes y logotipos
* **[Capturador de muestras de video](video-sample-grabber.md)** — extracción y procesamiento de fotogramas
* **[Cómo añadir efectos](add.md)** — guía general para aplicar efectos

## Más información

Numerosos efectos de video adicionales y funciones de procesamiento están disponibles en los SDK. Consulta la [Referencia completa de efectos](effects-reference.md) para información detallada sobre todos los efectos, o visita la documentación del SDK específico que estés usando para ejemplos de implementación y referencias de API.
  
---

Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a más ejemplos de código y muestras de implementación.
