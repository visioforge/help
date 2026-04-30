---
title: Referencia de Efectos de Video SDK: Filtros, Overlays y Más
description: Guía de referencia completa de todos los efectos de video en VisioForge .NET SDKs incluyendo efectos Classic (solo Windows) y multiplataforma.
sidebar_label: Referencia de Efectos
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing
  - Effects
  - Decklink
  - NDI
  - GIF
  - C#
primary_api_classes:
  - VideoEffect
  - VideoEffectGrayscale
  - GPUVideoEffect
  - GrayscaleVideoEffect
  - GaussianBlurVideoEffect

---

# Referencia Completa de Efectos de Video

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Este documento proporciona una referencia completa para todos los efectos de video disponibles en los SDKs .NET de VisioForge. Los efectos están disponibles en dos implementaciones distintas:

### Tipos de Efectos por Motor SDK

#### Efectos Classic (Solo Windows)
- **Disponible en**: VideoCaptureCore, MediaPlayerCore, VideoEditCore
- **Plataforma**: Solo Windows (.NET Framework 4.7.2+ y .NET 6-10)
- **Tipos**: 
  - `VideoEffect*` - Procesamiento basado en CPU
  - `GPUVideoEffect*` - Acelerado por GPU (DirectX)
  - `Maxine*` - Efectos impulsados por IA de NVIDIA (GPUs RTX)
- **Ubicación**: Espacio de nombres `VisioForge.Core.Types.VideoEffects`

#### Efectos Multiplataforma
- **Disponible en**: VideoCaptureCoreX, MediaPlayerCoreX, VideoEditCoreX, Media Blocks SDK
- **Plataforma**: Windows, Linux, macOS, Android, iOS
- **Tipos**:
  - Clases `*VideoEffect` en `VisioForge.Core.Types.X.VideoEffects`
  - Procesamiento multiplataforma para compatibilidad universal
- **Ubicación**: Espacio de nombres `VisioForge.Core.Types.X.VideoEffects`

> **Importante**: Al elegir efectos, seleccione según su SDK objetivo y requisitos de plataforma. Los efectos multiplataforma proporcionan mayor compatibilidad, mientras que los efectos Classic ofrecen optimizaciones específicas de Windows y aceleración GPU DirectX.

## Categorías de Efectos

Las siguientes secciones enumeran todos los efectos disponibles. Cada efecto incluye marcadores de disponibilidad de SDK:
- 🪟 **Classic** = VideoCaptureCore/MediaPlayerCore/VideoEditCore (Solo Windows)
- 🌍 **Multiplataforma** = VideoCaptureCoreX/MediaPlayerCoreX/VideoEditCoreX/Media Blocks (Multiplataforma)

### Efectos de Ajuste de Color

#### Brillo y Oscuridad

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectDarkness | 🪟 Classic | Ajusta la oscuridad/luminosidad general. Valores superiores a 128 oscurecen, valores inferiores iluminan la imagen. |
| GPUVideoEffectDarkness | 🪟 Classic (GPU) | Ajuste de oscuridad acelerado por GPU. |
| GPUVideoEffectBrightness | 🪟 Classic (GPU) | Ajuste de brillo acelerado por GPU. Añade o resta valores uniformes a los componentes RGB. |
| VideoEffectLightness | 🪟 Classic | Ajusta la luminosidad en el espacio de color HSL, preservando las relaciones de tono y saturación. |
| VideoBalanceVideoEffect | 🌍 Multiplataforma | Ajuste multiplataforma de brillo, contraste, saturación y tono. |

#### Contraste y Color

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectContrast | 🪟 Classic | Ajusta la diferencia entre áreas claras y oscuras. Soporta transiciones animadas. |
| GPUVideoEffectContrast | 🪟 Classic (GPU) | Ajuste de contraste acelerado por GPU. |
| VideoEffectSaturation | 🪟 Classic | Controla la intensidad del color desde escala de grises (0) hasta saturación completa (255). |
| GPUVideoEffectSaturation | 🪟 Classic (GPU) | Ajuste de saturación acelerado por GPU. |
| VideoBalanceVideoEffect | 🌍 Multiplataforma | Proporciona control de contraste y saturación (multiplataforma). |
| GammaVideoEffect | 🌍 Multiplataforma | Corrección gamma para ajuste de curva de brillo. |

### Filtros de Color y Conversiones

#### Escala de Grises y Monocromático

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectGrayscale | 🪟 Classic | Convierte video a color a blanco y negro usando pesos de luminancia perceptual. |
| GPUVideoEffectGrayscale | 🪟 Classic (GPU) | Conversión a escala de grises acelerada por GPU. |
| GPUVideoEffectMonoChrome | 🪟 Classic (GPU) | Crea efecto monocromático con color de tinte personalizable. |
| GrayscaleVideoEffect | 🌍 Multiplataforma | Conversión a escala de grises multiplataforma usando elemento videobalance. |
| ColorEffectsVideoEffect | 🌍 Multiplataforma | Varios presets de color incluyendo monocromático, sepia, mapa de calor y más. |

#### Inversión de Color

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectInvert | 🪟 Classic | Invierte todos los valores de color RGB, creando efecto de negativo fotográfico. |
| GPUVideoEffectInvert | 🪟 Classic (GPU) | Inversión de color acelerada por GPU. |

#### Filtros de Canal de Color

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectRed | 🪟 Classic | Aísla el canal de color rojo, eliminando componentes verde y azul. |
| VideoEffectGreen | 🪟 Classic | Aísla el canal de color verde, eliminando componentes rojo y azul. |
| VideoEffectBlue | 🪟 Classic | Aísla el canal de color azul, eliminando componentes rojo y verde. |
| VideoEffectFilterRed | 🪟 Classic | Aplica efecto de filtro de color rojo con intensidad ajustable. |
| VideoEffectFilterGreen | 🪟 Classic | Aplica efecto de filtro de color verde con intensidad ajustable. |
| VideoEffectFilterBlue | 🪟 Classic | Aplica efecto de filtro de color azul con intensidad ajustable. |

#### Gradación de Color

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectLUT | 🪟 Classic | Gradación de color mediante tabla de búsqueda usando archivos LUT 3D para corrección de color profesional. |
| LUTVideoEffect | 🌍 Multiplataforma | Soporte LUT 3D multiplataforma con múltiples modos de interpolación. |
| VideoEffectPosterize | 🪟 Classic | Reduce el número de colores, creando efecto tipo póster con niveles de color discretos. |
| VideoEffectSolorize | 🪟 Classic | Efecto de solarización que invierte colores por encima de un nivel de brillo umbral. |

### Mejora de Imagen

#### Nitidez y Desenfoque

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectSharpen | 🪟 Classic | Mejora los bordes y detalles finos para imágenes más nítidas. |
| GPUVideoEffectSharpen | 🪟 Classic (GPU) | Nitidez acelerada por GPU. |
| VideoEffectBlur | 🪟 Classic | Aplica filtro de suavizado, suavizando la imagen y reduciendo detalles. |
| GPUVideoEffectBlur | 🪟 Classic (GPU) | Desenfoque acelerado por GPU. |
| GPUVideoEffectDirectionalBlur | 🪟 Classic (GPU) | Desenfoque con componente direccional para efectos de desenfoque de movimiento. |
| VideoEffectSmooth | 🪟 Classic | Filtro de suavizado para reducción de ruido y suavizado suave de imagen. |
| GaussianBlurVideoEffect | 🌍 Multiplataforma | Desenfoque Gaussiano multiplataforma con sigma ajustable. |
| SmoothVideoEffect | 🌍 Multiplataforma | Filtro de suavizado/alisado multiplataforma. |

#### Reducción de Ruido

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectDenoiseAdaptive | 🪟 Classic | Reducción de ruido adaptativa que preserva bordes mientras elimina ruido. |
| VideoEffectDenoiseCAST | 🪟 Classic | Algoritmo de eliminación de ruido CAST (Autómata Celular Espacio-Temporal) con múltiples parámetros. |
| VideoEffectDenoiseMosquito | 🪟 Classic | Reduce artefactos de ruido mosquito comunes en video comprimido. |
| GPUVideoEffectDenoise | 🪟 Classic (GPU) | Reducción de ruido general acelerada por GPU. |

### Transformaciones Geométricas

#### Rotación y Volteo

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectRotate | 🪟 Classic | Rota video por cualquier ángulo con opciones para estirar o preservar el fotograma completo. |
| FlipRotateVideoEffect | 🌍 Multiplataforma | Volteo y rotación multiplataforma (90°, 180°, 270°, volteo horizontal/vertical). |
| VideoEffectFlipHorizontal | 🪟 Classic | Voltea video horizontalmente (espejo izquierda-derecha). |
| VideoEffectFlipVertical | 🪟 Classic | Voltea video verticalmente (espejo arriba-abajo). |
| VideoEffectMirrorHorizontal | 🪟 Classic | Crea efecto espejo horizontal en el centro del fotograma. |
| VideoEffectMirrorVertical | 🪟 Classic | Crea efecto espejo vertical en el centro del fotograma. |

#### Escalado y Transformación

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectZoom | 🪟 Classic | Amplía una región específica del video con control de panorámica y escala. |
| VideoEffectPan | 🪟 Classic | Efecto de panorámica y recorte para seleccionar una región específica del video. |
| ResizeVideoEffect | 🌍 Multiplataforma | Escalado/redimensionamiento de video multiplataforma con múltiples métodos de interpolación. |
| CropVideoEffect | 🌍 Multiplataforma | Recorte de video multiplataforma a dimensiones específicas. |
| AspectRatioCropVideoEffect | 🌍 Multiplataforma | Recorta automáticamente el video a la relación de aspecto objetivo. |
| BoxVideoEffect | 🌍 Multiplataforma | Añade letterboxing/pillarboxing con color de relleno personalizado. |

### Efectos Artísticos y Estilísticos

#### Efectos de Película Clásica

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| GPUVideoEffectOldMovie | 🪟 Classic (GPU) | Efecto de película vintage con grano, rayaduras y tono sepia. |
| GPUVideoEffectNightVision | 🪟 Classic (GPU) | Simulación de cámara de visión nocturna con aspecto de fósforo verde. |
| AgingVideoEffect | 🌍 Multiplataforma | Efecto de envejecimiento/película vintage multiplataforma. |
| OpticalAnimationBWVideoEffect | 🌍 Multiplataforma | Efectos de animación de ilusión óptica en blanco y negro. |

#### Efectos de Bordes y Texturas

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| GPUVideoEffectEmboss | 🪟 Classic (GPU) | Crea efecto de relieve o grabado resaltando bordes. |
| GPUVideoEffectContour | 🪟 Classic (GPU) | Detección de bordes y mejora de contornos. |
| GPUVideoEffectPixelate | 🪟 Classic (GPU) | Efecto de pixelación con tamaño de bloque ajustable. |
| VideoEffectMosaic | 🪟 Classic | Crea patrón de mosaico con tamaño de baldosa ajustable. |
| EdgeVideoEffect | 🌍 Multiplataforma | Filtro de detección de bordes multiplataforma. |
| DiceVideoEffect | 🌍 Multiplataforma | Crea efecto de dados/cubista dividiendo la imagen en cuadrados rotados. |

#### Efectos Especiales de Distorsión (Solo multiplataforma)

Los siguientes efectos artísticos están disponibles exclusivamente en la implementación multiplataforma:

| Efecto | Descripción |
|--------|-------------|
| FishEyeVideoEffect | Efecto de distorsión de lente ojo de pez. |
| GLTwirlVideoEffect | Efecto de distorsión de remolino/espiral (OpenGL). |
| GLBulgeVideoEffect | Distorsión de abombamiento/magnificación (OpenGL). |
| StretchVideoEffect | Efecto de distorsión de estiramiento. |
| GLLightTunnelVideoEffect | Efecto de perspectiva de túnel (OpenGL). |
| SquareVideoEffect | Efecto de deformación cuadrada. |
| CircleVideoEffect | Efecto de deformación circular. |
| KaleidoscopeVideoEffect | Efecto de espejo caleidoscópico. |
| MarbleVideoEffect | Efecto de textura de mármol. |
| Pseudo3DVideoEffect | Efecto estéreo pseudo 3D. |
| QuarkVideoEffect | Efecto de partículas quark. |
| RippleVideoEffect | Efecto de ondulación de agua. |
| WaterRippleVideoEffect | Ondulación de agua mejorada con múltiples modos. |
| WarpVideoEffect | Efecto de distorsión de deformación general. |
| MovingBlurVideoEffect | Desenfoque de movimiento con control direccional. |
| MovingEchoVideoEffect | Efecto de eco/estela de movimiento. |
| MovingZoomEchoVideoEffect | Efecto combinado de zoom y eco. |
| SMPTEVideoEffect | Efectos de transición SMPTE (cortinas, fundidos). |
| SMPTEAlphaVideoEffect | Transiciones SMPTE con canal alfa. |

#### Efectos Especiales

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectColorNoise | 🪟 Classic | Añade ruido de color aleatorio para efectos de grano o interferencia. |
| VideoEffectMonoNoise | 🪟 Classic | Añade ruido monocromático (escala de grises). |
| VideoEffectSpray | 🪟 Classic | Efecto de pintura en aerosol o puntillista. |
| VideoEffectShakeDown | 🪟 Classic | Efecto de sacudida vertical para simulación de impacto o terremoto. |

### Desentrelazado

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectDeinterlaceBlend | 🪟 Classic | Mezcla campos entrelazados para salida progresiva. |
| GPUVideoEffectDeinterlaceBlend | 🪟 Classic (GPU) | Mezcla de desentrelazado acelerada por GPU. |
| VideoEffectDeinterlaceCAVT | 🪟 Classic | Desentrelazado Adaptativo Vertical Temporal del Contenido. |
| VideoEffectDeinterlaceTriangle | 🪟 Classic | Método de desentrelazado por interpolación triangular. |
| DeinterlaceVideoEffect | 🌍 Multiplataforma | Desentrelazado multiplataforma con múltiples métodos (linear, greedy, vfir, yadif, etc.). |
| AutoDeinterlaceSettings | 🌍 Multiplataforma | Desentrelazado automático cuando se detecta contenido entrelazado. |
| InterlaceSettings | 🌍 Multiplataforma | Crea salida entrelazada a partir de contenido progresivo. |

### Efectos de Transición

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectFadeIn | 🪟 Classic | Fundido gradual de negro a contenido de video. |
| VideoEffectFadeOut | 🪟 Classic | Fundido gradual de contenido de video a negro. |

### Superposiciones y Gráficos

#### Superposiciones de Texto

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectTextLogo | 🪟 Classic | Superposición de texto flexible con amplia personalización incluyendo fuentes, colores, rotación, efectos y texto animado. |
| VideoEffectScrollingTextLogo | 🪟 Classic | Banner de texto desplazable con control de dirección y velocidad. |
| TextOverlayVideoEffect | 🌍 Multiplataforma | Superposición de texto multiplataforma con control tipográfico avanzado. Soporta marcas de tiempo, hora del sistema y texto dinámico. |
| OverlayManagerText | 🌍 Multiplataforma | Superposición de texto avanzada con sombras, degradados y animaciones. |
| OverlayManagerScrollingText | 🌍 Multiplataforma | Texto desplazable con control total sobre velocidad, dirección y apariencia. |
| OverlayManagerDateTime | 🌍 Multiplataforma | Superposición de fecha/hora con formato personalizable. |

Ver: [Guía de Superposición de Texto](text-overlay.md)

#### Superposiciones de Imagen

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| VideoEffectImageLogo | 🪟 Classic | Superposición de imagen soportando PNG, JPG, BMP, GIF animado, con control de transparencia y posicionamiento. |
| ImageOverlayVideoEffect | 🌍 Multiplataforma | Superposición de imagen multiplataforma con posicionamiento y mezcla alfa. |
| ImageOverlayCairoVideoEffect | 🌍 Multiplataforma | Superposición de imagen avanzada usando gráficos Cairo con transformaciones. |
| OverlayManagerImage | 🌍 Multiplataforma | Superposición de imagen profesional con animaciones, transiciones y efectos. |
| OverlayManagerGIF | 🌍 Multiplataforma | Soporte de superposición de GIF animado. |
| SVGOverlayVideoEffect | 🌍 Multiplataforma | Superposición de gráficos vectoriales SVG. |

Ver: [Guía de Superposición de Imagen](image-overlay.md)

#### Características Avanzadas de Superposición (Solo multiplataforma)

| Característica | Descripción |
|----------------|-------------|
| OverlayManagerVideo | Superposición de video sobre video (imagen en imagen). |
| OverlayManagerDecklinkVideo | Superposición de fuente de video Blackmagic Decklink. |
| OverlayManagerNDIVideo | Superposición de fuente de video NDI. |
| OverlayManagerGroup | Agrupa múltiples superposiciones para control sincronizado. |
| OverlayManagerPan | Animación de panorámica de superposición. |
| OverlayManagerZoom | Animación de zoom de superposición. |
| OverlayManagerFade | Animación de fundido de entrada/salida de superposición. |
| OverlayManagerSqueezeback | Efecto squeezeback (reduce video principal para mostrar fondo). |
| OverlayManagerBackgroundImage | Capa de imagen de fondo. |
| OverlayManagerBackgroundRectangle | Fondo de rectángulo coloreado. |
| OverlayManagerBackgroundSquare | Fondo de cuadrado coloreado. |
| OverlayManagerBackgroundStar | Fondo en forma de estrella. |
| OverlayManagerBackgroundTriangle | Fondo en forma de triángulo. |
| OverlayManagerCircle | Superposición de forma circular. |
| OverlayManagerLine | Superposición de dibujo de línea. |
| OverlayManagerRectangle | Superposición de forma rectangular. |
| OverlayManagerStar | Superposición de forma de estrella. |
| OverlayManagerTriangle | Superposición de forma triangular. |

### Croma Key (Multiplataforma)

| Efecto | Descripción |
|--------|-------------|
| ChromaKeySettingsX | Croma key de pantalla verde / pantalla azul para eliminación de fondo (se integra con `MediaBlocksPipeline` + bloque de filtro croma). |

### Video 360° y VR (Solo Classic)

| Efecto | SDK | Descripción |
|--------|-----|-------------|
| GPUVideoEffectEquirectangular360 | 🪟 Classic (GPU) | Procesa formato de video 360° equirectangular. |
| GPUVideoEffectEquiangularCubemap360 | 🪟 Classic (GPU) | Convierte entre proyecciones equiangular y cubemap para video 360°. |
| GPUVideoEffectVR360Base | 🪟 Classic (GPU) | Clase base para efectos de video VR 360°. |

### Efectos Impulsados por IA (NVIDIA Maxine - Solo Classic)

Requiere GPU NVIDIA RTX con soporte del SDK Maxine.

#### Mejora de Video

| Efecto | Descripción |
|--------|-------------|
| MaxineDenoiseVideoEffect | Reducción de ruido impulsada por IA que preserva inteligentemente los detalles. |
| MaxineArtifactReductionVideoEffect | Reduce artefactos de compresión y degradación de video usando IA. |
| MaxineSuperResSettings | POCO de configuración para super-resolución IA (se conecta al pipeline de efectos Maxine). |

#### Efectos de Contenido

| Efecto | Descripción |
|--------|-------------|
| MaxineAIGSVideoEffect | Pantalla verde/eliminación de fondo impulsada por IA sin requerir pantalla verde real. |
| MaxineUpscaleSettings | POCO de configuración para escalado IA (se conecta al pipeline de efectos Maxine). |

#### Efectos Especiales

- **VideoEffectColorNoise** - Añade ruido de color aleatorio para efectos de grano o interferencia.
- **VideoEffectMonoNoise** - Añade ruido monocromático (escala de grises).
- **VideoEffectSpray** - Efecto de pintura en aerosol o puntillista.
- **VideoEffectShakeDown** - Efecto de sacudida vertical para simulación de impacto o terremoto.

### Desentrelazado

- **VideoEffectDeinterlaceBlend** / **GPUVideoEffectDeinterlaceBlend** - Mezcla campos entrelazados para salida progresiva.
- **VideoEffectDeinterlaceCAVT** - Desentrelazado Adaptativo Vertical Temporal del Contenido.
- **VideoEffectDeinterlaceTriangle** - Método de desentrelazado por interpolación triangular.

### Efectos de Transición

#### Efectos de Fundido

- **VideoEffectFadeIn** - Fundido gradual de negro a contenido de video.
- **VideoEffectFadeOut** - Fundido gradual de contenido de video a negro.

### Superposiciones y Gráficos

#### Superposiciones de Texto

- **VideoEffectTextLogo** - Superposición de texto flexible con amplia personalización incluyendo fuentes, colores, rotación, efectos y texto animado.
- **VideoEffectScrollingTextLogo** - Banner de texto desplazable con control de dirección y velocidad.

Ver: [Guía de Superposición de Texto](text-overlay.md)

#### Superposiciones de Imagen

- **VideoEffectImageLogo** - Superposición de imagen soportando PNG, JPG, BMP, GIF animado, con control de transparencia y posicionamiento.

Ver: [Guía de Superposición de Imagen](image-overlay.md)

### Video 360° y VR

- **GPUVideoEffectEquirectangular360** - Procesa formato de video 360° equirectangular.
- **GPUVideoEffectEquiangularCubemap360** - Convierte entre proyecciones equiangular y cubemap para video 360°.
- **GPUVideoEffectVR360Base** - Clase base para efectos de video VR 360°.

### Efectos Impulsados por IA (NVIDIA Maxine)

Requiere GPU NVIDIA RTX con soporte del SDK Maxine.

#### Mejora de Video

- **MaxineDenoiseVideoEffect** - Reducción de ruido impulsada por IA que preserva inteligentemente los detalles.
- **MaxineArtifactReductionVideoEffect** - Reduce artefactos de compresión y degradación de video usando IA.
- **MaxineSuperResSettings** - POCO de configuración para super-resolución IA.

#### Efectos de Contenido

- **MaxineAIGSVideoEffect** - Pantalla verde/eliminación de fondo impulsada por IA sin requerir pantalla verde real.
- **MaxineUpscaleSettings** - POCO de configuración para escalado IA.

## Parámetros de Efectos

### Parámetros Comunes

Todos los efectos de video soportan estos parámetros estándar:

- **Enabled** - Si el efecto está activo (puede activarse/desactivarse)
- **Name** - Identificador para recuperar instancias específicas de efectos
- **StartTime** - Cuándo comienza el efecto (TimeSpan.Zero = desde el inicio)
- **StopTime** - Cuándo termina el efecto (TimeSpan.Zero = hasta el final)

### Rangos de Valores de Efectos Classic

La mayoría de los efectos de ajuste Classic usan rangos de valores 0-255 donde:
- **128** típicamente representa neutral/sin cambio
- **0** representa mínimo (a menudo más oscuro/más débil)
- **255** representa máximo (a menudo más brillante/más fuerte)

### Transiciones Animadas (Efectos Classic)

Muchos efectos Classic soportan el parámetro **ValueStop** para animación suave:
- Establecer **Value** para valor inicial
- Establecer **ValueStop** para valor final
- Definir **StartTime** y **StopTime** para duración de animación

### Parámetros de Efectos Multiplataforma

Los efectos multiplataforma usan propiedades de elementos multimedia con rangos variables. Consulte la documentación individual de cada efecto para detalles específicos de parámetros.

## Comparación de SDKs

### Efectos Classic (VideoCaptureCore/MediaPlayerCore)

- **Plataforma**: Solo Windows (.NET Framework 4.7.2+ y .NET 6-10)
- **Tipos de Procesamiento**:
  - Basado en CPU: Clases `VideoEffect*`
  - Acelerado por GPU: Clases `GPUVideoEffect*` (DirectX)
  - Impulsado por IA: Clases `Maxine*` (requiere NVIDIA RTX)
- **Rendimiento**: Optimizado para Windows, aceleración GPU DirectX
- **Compatibilidad**: Solo escritorio Windows
- **Espacio de nombres**: `VisioForge.Core.Types.VideoEffects`

### Efectos Multiplataforma (VideoCaptureCoreX/Media Blocks)

- **Plataforma**: Multiplataforma (Windows, Linux, macOS, Android, iOS)
- **Procesamiento**: Multiplataforma, aceleración de hardware a través de plugins multimedia
- **Rendimiento**: Buen rendimiento en todas las plataformas, aceleración GPU varía según plataforma
- **Compatibilidad**: Universal - escritorio y móvil
- **Espacio de nombres**: `VisioForge.Core.Types.X.VideoEffects`
- **Características Adicionales**: 
  - Más efectos artísticos (distorsiones, deformaciones)
  - Sistema de superposición avanzado (OverlayManager)
  - Croma key / detección de movimiento
  - Mejor adaptado para plataformas móviles

## Ejemplos de Uso

### Aplicación de Efecto Classic

```csharp
// Efecto basado en CPU
var effect = new VideoEffectGrayscale(
    enabled: true,
    name: "BWEffect"
);
capture.Video_Effects_Add(effect);

// Efecto acelerado por GPU
var gpuEffect = new GPUVideoEffectBrightness(
    enabled: true,
    value: 180,
    name: "Brighten"
);
capture.Video_Effects_Add(gpuEffect);
```

### Aplicación de Efecto Multiplataforma

```csharp
// Escala de grises multiplataforma
var grayscale = new GrayscaleVideoEffect("bw_effect");
await videoCapture.Video_Effects_AddOrUpdateAsync(grayscale);

// Superposición de texto multiplataforma
var textOverlay = new TextOverlayVideoEffect
{
    Text = "Hola Mundo",
    Font = new FontSettings("Arial", "Bold", 24),
    Color = SKColors.Yellow,
    HorizontalAlignment = TextOverlayHAlign.Left,
    VerticalAlignment = TextOverlayVAlign.Top
};
await videoCapture.Video_Effects_AddOrUpdateAsync(textOverlay);
```

### Efecto Animado (Classic)

```csharp
// Fundido a negro durante 3 segundos
var fade = new VideoEffectDarkness(
    enabled: true,
    value: 128,        // Comenzar normal
    valueStop: 255,    // Terminar en oscuridad máxima
    name: "FadeOut",
    startTime: TimeSpan.FromSeconds(57),
    stopTime: TimeSpan.FromSeconds(60)
);
capture.Video_Effects_Add(fade);
```

### Efecto con Tiempo Limitado (Ambos)

```csharp
// Classic: Aplicar efecto solo durante rango de tiempo específico
var flashback = new VideoEffectGrayscale(
    enabled: true,
    name: "Flashback",
    startTime: TimeSpan.FromMinutes(2),
    stopTime: TimeSpan.FromMinutes(2.5)
);
player.Video_Effects_Add(flashback);

// Multiplataforma: Efecto con tiempo limitado
// Ctor: GaussianBlurVideoEffect(double sigma, string name = "gaussian_blur")
var blur = new GaussianBlurVideoEffect(5.0, "blur")
{
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromSeconds(15)
};
await player.Video_Effects_AddOrUpdateAsync(blur);
```

## Consideraciones de Rendimiento

### Efectos Classic

1. **Efectos GPU**: Mejor rendimiento para video de alta resolución en Windows
2. **Apilamiento de Efectos**: Minimice efectos simultáneos para mejor rendimiento
3. **Efectos Animados**: Las animaciones suaves añaden sobrecarga mínima
4. **Efectos IA**: Los más intensivos en recursos, usar en GPUs RTX
5. **Impacto de Resolución**: Resoluciones más altas aumentan significativamente los requisitos de procesamiento

### Efectos Multiplataforma

1. **Aceleración de Hardware**: Varía según plataforma y compilación del framework multimedia
2. **Multiplataforma**: Generalmente buen rendimiento en todas las plataformas soportadas
3. **Optimización Móvil**: Mejor adaptado para móvil que los efectos Classic
4. **Sistema de Superposición**: Renderizado eficiente de múltiples superposiciones
5. **Disponibilidad de Plugins**: Algunos efectos requieren plugins multimedia específicos

## Disponibilidad por Plataforma

### Efectos Classic

| Plataforma | Efectos CPU | Efectos GPU | IA Maxine |
|------------|-------------|-------------|-----------|
| Windows Desktop | ✅ Completo | ✅ Completo | ✅ GPUs RTX |
| Linux    | ❌ | ❌ | ❌ |
| macOS    | ❌ | ❌ | ❌ |
| Android  | ❌ | ❌ | ❌ |
| iOS      | ❌ | ❌ | ❌ |

### Efectos Multiplataforma

| Plataforma | Soporte | Aceleración de Hardware |
|------------|---------|-------------------------|
| Windows  | ✅ Completo | ✅ Vía plugins multimedia |
| Linux    | ✅ Completo | ✅ VA-API, VDPAU, etc. |
| macOS    | ✅ Completo | ✅ VideoToolbox |
| Android  | ✅ Completo | ✅ Códecs de hardware |
| iOS      | ✅ Completo | ✅ VideoToolbox |

## Elegir el Tipo de Efecto Correcto

### Use Efectos Classic Cuando:
- ✅ Apunta solo a escritorio Windows
- ✅ Necesita máximo rendimiento en Windows
- ✅ Usa aceleración GPU DirectX
- ✅ Necesita características de IA NVIDIA Maxine
- ✅ Trabaja con motores VideoCaptureCore/MediaPlayerCore

### Use Efectos Multiplataforma Cuando:
- ✅ Necesita soporte multiplataforma
- ✅ Apunta a plataformas móviles (Android/iOS)
- ✅ Apunta a Linux o macOS
- ✅ Usa VideoCaptureCoreX/Media Blocks
- ✅ Necesita características avanzadas de superposición
- ✅ Necesita croma key o detección de movimiento

## Recursos Adicionales

- [Guía de Superposición de Texto](text-overlay.md)
- [Guía de Superposición de Imagen](image-overlay.md)
- [Capturador de Muestras de Video](video-sample-grabber.md)
- [Cómo Añadir Efectos](add.md)

---

Visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos de código completos y ejemplos de implementación.
