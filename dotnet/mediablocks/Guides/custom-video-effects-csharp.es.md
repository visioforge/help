---
title: Efectos de Video y Shaders OpenGL Personalizados en C# .NET
description: Aplique efectos de video en tiempo real, shaders GLSL, gradación de color LUT y animaciones pan/zoom en C# .NET con VisioForge Media Blocks SDK y GPU.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Encoding
  - Effects
  - Webcam
  - C#
primary_api_classes:
  - LUTProcessorBlock
  - MediaBlocksPipeline
  - GLUploadBlock
  - GLDownloadBlock
  - GLShaderBlock

---

# Efectos de Video Personalizados y Shaders OpenGL en C# .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El SDK Media Blocks de VisioForge proporciona más de 70 bloques de procesamiento de video para efectos en tiempo real, shaders OpenGL acelerados por GPU, gradación de color LUT profesional y transformaciones animadas de pan/zoom. Los efectos se aplican insertando bloques de procesamiento en el pipeline entre una fuente de video y un renderizador o codificador.

## Efectos de Video Integrados

[MediaBlocksPipeline](#){ .md-button }

Todos los efectos integrados siguen el mismo patrón de pipeline — inserte el bloque de efecto entre su fuente y renderizador:

```text
VideoSource → EffectBlock → VideoRenderer
```

### Desenfoque Gaussiano

Aplique desenfoque o nitidez con sigma configurable. Los valores positivos desenfocan, los negativos agudizan:

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;

// Desenfoque con sigma 1.2 (mayor = más desenfoque)
var blur = new GaussianBlurBlock(1.2);

pipeline.Connect(videoSource.Output, blur.Input);
pipeline.Connect(blur.Output, videoRenderer.Input);
```

### Reducción de Ruido (Filtro Suavizado)

Reducción de ruido basada en OpenCV con suavizado que preserva bordes:

```csharp
using VisioForge.Core.Types.X.VideoEffects;

var smoothSettings = new SmoothVideoEffect()
{
    FilterSize = 5,       // tamaño del kernel (mayor = suavizado más fuerte)
    Tolerance = 128,      // umbral de contraste (0-255)
    LumaOnly = true       // suavizar solo brillo, preservar color
};

var smooth = new SmoothBlock(smoothSettings);

pipeline.Connect(videoSource.Output, smooth.Input);
pipeline.Connect(smooth.Output, videoRenderer.Input);
```

### Balance de Color

Ajuste brillo, contraste, saturación y tono en tiempo real:

```csharp
var balance = new VideoBalanceBlock(new VideoBalanceVideoEffect
{
    Brightness = 0.1,    // -1.0 a 1.0 (0 = sin cambio)
    Contrast = 1.2,      // 0.0 a infinito (1 = sin cambio)
    Saturation = 1.5,    // 0.0 a infinito (1 = sin cambio)
    Hue = 0.0            // -1.0 a 1.0 (0 = sin cambio)
});

pipeline.Connect(videoSource.Output, balance.Input);
pipeline.Connect(balance.Output, videoRenderer.Input);
```

### Efectos de Color Predefinidos

Aplique presets artísticos de color (sepia, mapa de calor, procesamiento cruzado, etc.):

```csharp
var colorFx = new ColorEffectsBlock(ColorEffectsPreset.Sepia);

pipeline.Connect(videoSource.Output, colorFx.Input);
pipeline.Connect(colorFx.Output, videoRenderer.Input);
```

## Efectos Acelerados por GPU con OpenGL

[MediaBlocksPipeline](#){ .md-button }

Los efectos OpenGL se ejecutan en la GPU para un rendimiento significativamente mejor con video HD/4K. Requieren cargar los fotogramas de video en la memoria de la GPU y descargarlos después:

```text
VideoSource → GLUploadBlock → GLEffectBlock → GLDownloadBlock → VideoRenderer
```

### Uso de Efectos OpenGL Integrados

El SDK incluye más de 25 efectos acelerados por GPU:

```csharp
using VisioForge.Core.MediaBlocks.OpenGL;

var glUpload = new GLUploadBlock();
var glBlur = new GLBlurBlock();
var glDownload = new GLDownloadBlock();

pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, glBlur.Input);
pipeline.Connect(glBlur.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

### Efectos OpenGL Disponibles

| Bloque de Efecto | Descripción |
| --- | --- |
| `GLBlurBlock` | Desenfoque por convolución separable 9x9 |
| `GLColorBalanceBlock` | Brillo, contraste, tono, saturación |
| `GLGrayscaleBlock` | Conversión a escala de grises |
| `GLSepiaBlock` | Tono sepia |
| `GLSobelBlock` | Detección de bordes Sobel |
| `GLLaplacianBlock` | Detección de bordes Laplaciano |
| `GLFishEyeBlock` | Distorsión ojo de pez |
| `GLBulgeBlock` | Distorsión de abultamiento |
| `GLTwirlBlock` | Efecto de remolino |
| `GLMirrorBlock` | Reflejo de espejo |
| `GLSqueezeBlock` | Distorsión de compresión |
| `GLStretchBlock` | Distorsión de estiramiento |
| `GLHeatBlock` | Visualización de mapa de calor |
| `GLGlowLightingBlock` | Efecto de resplandor/iluminación |
| `GLLightTunnelBlock` | Efecto de túnel de luz |
| `GLSinCityBlock` | Sin City (desaturación selectiva) |
| `GLXRayBlock` | Visualización de rayos X |
| `GLLumaCrossProcessingBlock` | Procesamiento cruzado de luminancia |
| `GLFlipBlock` | Volteo/espejo por GPU |
| `GLDeinterlaceBlock` | Desentrelazado por GPU |
| `GLTransformationBlock` | Transformaciones afines |
| `GLAlphaBlock` | Canal alfa / clave de croma |
| `GLEquirectangularViewBlock` | Proyección equirectangular 360° |

## Shaders GLSL Personalizados

[MediaBlocksPipeline](#){ .md-button }

Escriba shaders GLSL personalizados de fragmento y vértice y aplíquelos a video en vivo en tiempo real. El `GLShaderBlock` ejecuta su shader en la GPU para cada fotograma de video.

### Arquitectura del Pipeline

```text
SystemVideoSourceBlock → GLUploadBlock → GLShaderBlock → GLDownloadBlock → VideoRendererBlock
```

### Ejemplo Completo

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenGL;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenGL;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Fuente de video (webcam)
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var videoSource = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(videoDevices[0]));

// Cargar fotogramas a la memoria de la GPU
var glUpload = new GLUploadBlock();

// Shader GLSL personalizado — conversión a escala de grises
var vertexShader = @"
#version 100
#ifdef GL_ES
precision mediump float;
#endif
attribute vec4 a_position;
attribute vec2 a_texcoord;
varying vec2 v_texcoord;
void main() {
    gl_Position = a_position;
    v_texcoord = a_texcoord;
}";

var fragmentShader = @"
#version 100
#ifdef GL_ES
precision mediump float;
#endif
varying vec2 v_texcoord;
uniform sampler2D tex;
void main() {
    vec4 color = texture2D(tex, v_texcoord);
    float gray = dot(color.rgb, vec3(0.299, 0.587, 0.114));
    gl_FragColor = vec4(vec3(gray), color.a);
}";

var shader = new GLShader(vertexShader, fragmentShader);
var shaderBlock = new GLShaderBlock(new GLShaderSettings(shader));

// Descargar fotogramas de la GPU
var glDownload = new GLDownloadBlock();

// Renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Construir pipeline
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, shaderBlock.Input);
pipeline.Connect(shaderBlock.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Cambio de Shaders en Tiempo de Ejecución

Puede cambiar el shader mientras el pipeline está en ejecución:

```csharp
// Shader de inversión de color
var invertFragment = @"
#version 100
#ifdef GL_ES
precision mediump float;
#endif
varying vec2 v_texcoord;
uniform sampler2D tex;
void main() {
    vec4 color = texture2D(tex, v_texcoord);
    gl_FragColor = vec4(vec3(1.0) - color.rgb, color.a);
}";

shaderBlock.Settings.Fragment = invertFragment;
shaderBlock.Update();  // aplicar cambios sin reiniciar el pipeline
```

## Uniformes de Shader para Efectos Dinámicos

[MediaBlocksPipeline](#){ .md-button }

Pase parámetros a shaders GLSL en tiempo de ejecución usando el diccionario `Uniforms`. Esto permite control dinámico en tiempo real sobre el comportamiento del shader.

### Desenfoque Gaussiano de Dos Pasadas con Radio Ajustable

```csharp
// Pasada de desenfoque horizontal
var horizontalSettings = new GLShaderSettings(vertexShader, horizontalFragmentShader);
horizontalSettings.Uniforms["blur_radius"] = 2.0f;
horizontalSettings.Uniforms["tex_width"] = 1920.0f;
var blurH = new GLShaderBlock(horizontalSettings);

// Pasada de desenfoque vertical
var verticalSettings = new GLShaderSettings(vertexShader, verticalFragmentShader);
verticalSettings.Uniforms["blur_radius"] = 2.0f;
verticalSettings.Uniforms["tex_height"] = 1080.0f;
var blurV = new GLShaderBlock(verticalSettings);

// Pipeline: Source → Upload → H-Blur → V-Blur → Download → Renderer
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, blurH.Input);
pipeline.Connect(blurH.Output, blurV.Input);
pipeline.Connect(blurV.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

### Actualización de Uniformes en Tiempo de Ejecución

```csharp
// Ajustar radio de desenfoque en respuesta a un cambio de slider
float newRadius = 5.0f;

blurH.Settings.Uniforms["blur_radius"] = newRadius;
blurH.Update();

blurV.Settings.Uniforms["blur_radius"] = newRadius;
blurV.Update();
```

## Gradación de Color LUT

[MediaBlocksPipeline](#){ .md-button }

Aplique gradación de color profesional usando archivos de Tabla de Consulta 3D (LUT). El `LUTProcessorBlock` transforma el color de cada píxel a través de un cubo de color 3D para efectos de color cinematográficos.

### Formatos LUT Soportados

| Formato | Extensión | Origen |
| --- | --- | --- |
| Iridas/Resolve | `.cube` | DaVinci Resolve, Adobe |
| After Effects | `.3dl` | Adobe After Effects |
| DaVinci | `.dat` | DaVinci Resolve |
| Pandora | `.m3d` | Pandora |
| CineSpace | `.csp` | CineSpace |

### Aplicación Básica de LUT

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

var lutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cinematic.cube");
var lutProcessor = new LUTProcessorBlock(new LUTVideoEffect(lutPath));

pipeline.Connect(videoSource.Output, lutProcessor.Input);
pipeline.Connect(lutProcessor.Output, videoRenderer.Input);
```

### Comparación Lado a Lado

Use un `TeeBlock` para mostrar video original y gradado lado a lado:

```csharp
using VisioForge.Core.MediaBlocks.Special;

var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);

var lutProcessor = new LUTProcessorBlock(new LUTVideoEffect(lutPath));

// Salida original
var rendererOriginal = new VideoRendererBlock(pipeline, null);

// Salida gradada
var rendererGraded = new VideoRendererBlock(pipeline, null);

pipeline.Connect(videoSource.Output, tee.Input);
pipeline.Connect(tee.Outputs[0], lutProcessor.Input);
pipeline.Connect(lutProcessor.Output, rendererGraded.Input);
pipeline.Connect(tee.Outputs[1], rendererOriginal.Input);
```

### Modos de Interpolación

El `LUTVideoEffect` soporta tres modos de interpolación para equilibrar calidad y rendimiento:

- **Tetrahedral** — mayor calidad, mejor para salida final
- **Trilinear** — buen equilibrio entre calidad y velocidad
- **NearestNeighbor** — más rápido, menor calidad

## Animaciones de Pan, Zoom y Ken Burns

[MediaBlocksPipeline](#){ .md-button }

El `PanZoomBlock` proporciona transformaciones estáticas y animadas de pan/zoom — ideal para efectos Ken Burns, visualización de regiones de interés y encuadre dinámico de video.

### Configuración del Pipeline

```csharp
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

var panZoom = new PanZoomBlock();

pipeline.Connect(fileSource.VideoOutput, panZoom.Input);
pipeline.Connect(panZoom.Output, videoRenderer.Input);
```

### Zoom Estático

Acercar a un punto específico del fotograma:

```csharp
// Zoom 2x centrado en el medio del fotograma
panZoom.SetZoom(new VideoStreamZoomSettings(
    zoomX: 2.0,     // factor de zoom horizontal
    zoomY: 2.0,     // factor de zoom vertical
    centerX: 0.5,   // punto central X (0.0 - 1.0)
    centerY: 0.5    // punto central Y (0.0 - 1.0)
));
```

### Pan Estático

Desplazar la región visible:

```csharp
// Desplazar 100 píxeles a la derecha, 50 píxeles abajo
panZoom.SetPan(new VideoStreamPanSettings(100, 50));
```

### Zoom Dinámico (Efecto Ken Burns)

Animar una transición de zoom suave durante un rango de tiempo:

```csharp
// Zoom lento de 1x a 2x durante 5 segundos desde la posición actual
var startTime = await pipeline.Position_GetAsync();
var endTime = startTime + TimeSpan.FromSeconds(5);

panZoom.SetDynamicZoom(new VideoStreamDynamicZoomSettings(
    startZoomX: 1.0, startZoomY: 1.0,   // comenzar en tamaño normal
    stopZoomX: 2.0,  stopZoomY: 2.0,     // terminar en zoom 2x
    startTime: startTime,
    stopTime: endTime
));
```

### Pan Dinámico

Animar un movimiento de pan suave:

```csharp
panZoom.SetDynamicPan(new VideoStreamDynamicPanSettings(
    startPanX: 0, startPanY: 0,       // posición inicial
    stopPanX: 200, stopPanY: 100,     // posición final
    startTime: startTime,
    stopTime: endTime
));
```

### Recorte y Pan por Rectángulo

Enfocarse en una región rectangular específica del video:

```csharp
// Mostrar solo la región comenzando en (50, 50) con tamaño 400x300
panZoom.SetRect(VideoStreamRectSettings.FromPositionAndSize(50, 50, 400, 300));
```

### Reiniciar Pan/Zoom

Limpiar todas las configuraciones de pan/zoom para volver a la vista original:

```csharp
panZoom.SetZoom(null);
panZoom.SetDynamicZoom(null);
panZoom.SetPan(null);
panZoom.SetDynamicPan(null);
panZoom.SetRect(null);
```

## Encadenar Múltiples Efectos

Conecte múltiples bloques de efecto en serie para construir cadenas de procesamiento complejas:

```text
VideoSource → GaussianBlurBlock → VideoBalanceBlock → LUTProcessorBlock → VideoRenderer
```

```csharp
var blur = new GaussianBlurBlock(0.8);
var balance = new VideoBalanceBlock(new VideoBalanceVideoEffect
{
    Brightness = 0.05,
    Contrast = 1.1
});
var lut = new LUTProcessorBlock(new LUTVideoEffect("cinematic.cube"));

pipeline.Connect(videoSource.Output, blur.Input);
pipeline.Connect(blur.Output, balance.Input);
pipeline.Connect(balance.Output, lut.Input);
pipeline.Connect(lut.Output, videoRenderer.Input);
```

Para cadenas aceleradas por GPU, mantenga los fotogramas en memoria OpenGL entre efectos:

```csharp
pipeline.Connect(videoSource.Output, glUpload.Input);
pipeline.Connect(glUpload.Output, glBlur.Input);
pipeline.Connect(glBlur.Output, glColorBalance.Input);
pipeline.Connect(glColorBalance.Output, glDownload.Input);
pipeline.Connect(glDownload.Output, videoRenderer.Input);
```

## Solución de Problemas

### Los Efectos OpenGL No Funcionan

**Síntoma:** El pipeline falla al iniciar o el video aparece negro al usar efectos GL.

**Soluciones:**

- Verifique que los controladores de GPU estén actualizados — los efectos OpenGL requieren soporte de hardware OpenGL
- Asegúrese de que `GLUploadBlock` y `GLDownloadBlock` se usen para transferir fotogramas hacia/desde la memoria de la GPU
- Retroceda a equivalentes basados en CPU (ej., `GaussianBlurBlock` en lugar de `GLBlurBlock`) en sistemas sin soporte GPU
- Verifique `GLShaderBlock.IsAvailable()` antes de crear bloques OpenGL

### El Archivo LUT No se Carga

**Síntoma:** `LUTProcessorBlock` lanza una excepción o no tiene efecto visible.

**Soluciones:**

- Verifique que la ruta del archivo LUT sea correcta y que el archivo exista
- Asegúrese de que el formato del archivo sea soportado (`.cube`, `.3dl`, `.dat`, `.m3d`, `.csp`)
- Verifique `LUTProcessorBlock.IsAvailable()` antes de crear el bloque
- Use una ruta absoluta o `Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "archivo.cube")`

### Alto Uso de CPU con Efectos

**Síntoma:** El rendimiento se degrada al aplicar múltiples efectos.

**Soluciones:**

- Cambie de efectos CPU a equivalentes acelerados por GPU OpenGL
- Reduzca la resolución del video antes de aplicar efectos (use `VideoResizeBlock`)
- Minimice el número de efectos encadenados — combine operaciones en un solo shader personalizado cuando sea posible
- Para desenfoque de dos pasadas, use el desenfoque simple de una pasada como alternativa más rápida

## Preguntas Frecuentes

### ¿Cómo aplico un shader GLSL personalizado a video en vivo en C#?

Cree un `GLShaderBlock` con el código fuente de su shader de vértice y fragmento, luego insértelo entre `GLUploadBlock` y `GLDownloadBlock` en su pipeline. El shader de vértice estándar pasa la posición y las coordenadas de textura. Su shader de fragmento recibe el fotograma de video como `uniform sampler2D tex` y envía la salida a `gl_FragColor`. Use `shaderBlock.Update()` para cambiar shaders en tiempo de ejecución sin reiniciar el pipeline. Consulte la sección [Shaders GLSL Personalizados](#shaders-glsl-personalizados) para un ejemplo completo.

### ¿Qué formatos de archivo LUT soporta el SDK para gradación de color?

El `LUTProcessorBlock` soporta cinco formatos estándar de la industria de LUT 3D: Iridas/Resolve `.cube`, After Effects `.3dl`, DaVinci `.dat`, Pandora `.m3d` y CineSpace `.csp`. El formato `.cube` es el más utilizado y se exporta desde DaVinci Resolve, Adobe Premiere y la mayoría de herramientas de gradación de color. Tres modos de interpolación están disponibles: Tetrahedral (mayor calidad), Trilinear y NearestNeighbor.

### ¿Puedo encadenar múltiples efectos de video en un pipeline?

Sí. Conecte bloques de efecto en serie: `Source → Efecto1 → Efecto2 → Efecto3 → Renderer`. Cada bloque procesa la salida del anterior. Para cadenas aceleradas por GPU, mantenga los fotogramas en memoria OpenGL conectando bloques de efecto GL directamente sin insertar `GLDownloadBlock` / `GLUploadBlock` entre ellos — solo cargue al inicio y descargue al final.

### ¿Cómo creo una animación pan/zoom Ken Burns?

Use `PanZoomBlock` con `SetDynamicZoom()` para animar una transición de zoom suave a lo largo del tiempo. Pase un `VideoStreamDynamicZoomSettings` con factores de zoom inicio/fin y marcas de tiempo inicio/fin. El bloque interpola entre los valores automáticamente. Combine con `SetDynamicPan()` para animación simultánea de pan y zoom. Consulte la sección [Animaciones de Pan, Zoom y Ken Burns](#animaciones-de-pan-zoom-y-ken-burns) para ejemplos de código.

### ¿Qué plataformas soportan efectos de video OpenGL?

Los efectos OpenGL son soportados en Windows, Linux, macOS y Android — cualquier plataforma con soporte OpenGL ES 2.0 o superior. En iOS y macOS, el SDK también proporciona equivalentes acelerados por Metal (`MetalVideoFilterBlock`) para rendimiento nativo de GPU. Los efectos basados en CPU (`GaussianBlurBlock`, `SmoothBlock`, `ColorEffectsBlock`, etc.) funcionan en todas las plataformas incluyendo iOS.

## Ver También

- [Referencia de Bloques de Procesamiento de Video](../VideoProcessing/index.md) — lista completa de más de 70 bloques de procesamiento
- [Referencia de Efectos OpenGL](../OpenGL/index.md) — configuraciones y tipos de efectos OpenGL acelerados por GPU
- [Resumen de Efectos de Video](../../general/video-effects/index.md) — categorías de efectos Clásicos y Multiplataforma
- [Referencia de Efectos (100+ efectos)](../../general/video-effects/effects-reference.md) — parámetros detallados para todos los efectos
- [Guía de Despliegue](../../deployment-x/index.md) — paquetes de runtime específicos de plataforma
- [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — demos de shaders y efectos
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — página del producto y descargas
