---
title: Salida de Video WebM en .NET
description: Cree videos WebM en .NET con códecs VP8, VP9 y AV1 para streaming de video eficiente listo para web y entrega de contenido HTML5.
---

# Salida de Video WebM en los SDK .NET de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## ¿Qué es WebM?

WebM es un formato de archivo multimedia de código abierto y libre de regalías optimizado para entrega web. Desarrollado para proporcionar streaming de video eficiente con requisitos mínimos de procesamiento, WebM se ha convertido en un estándar para contenido de video HTML5. El formato soporta códecs modernos incluyendo VP8 y VP9 para compresión de video, junto con Vorbis y Opus para codificación de audio.

Las ventajas clave de WebM incluyen:

- **Rendimiento optimizado para web** con tiempos de carga rápidos
- **Amplio soporte de navegadores** en las principales plataformas
- **Video de alta calidad** con tamaños de archivo más pequeños
- **Licenciamiento de código abierto** sin costos de regalías
- **Capacidades de streaming eficientes** para aplicaciones de medios

## Implementación en Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

En plataformas Windows, la implementación de VisioForge aprovecha la clase [WebMOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WebMOutput.html) del namespace `VisioForge.Core.Types.X.Output`.

### Configuración básica

Para implementar rápidamente la salida WebM en su aplicación Windows:

```csharp
using VisioForge.Core.Types.Output;

// Inicializar configuración de salida WebM
var webmOutput = new WebMOutput();

// Configurar parámetros esenciales
webmOutput.Video_Mode = VP8QualityMode.Realtime;
webmOutput.Video_EndUsage = VP8EndUsageMode.VBR;
webmOutput.Video_Encoder = WebMVideoEncoder.VP8;
webmOutput.Video_Bitrate = 2000;
webmOutput.Audio_Quality = 80;

// Aplicar a su instancia del núcleo
var core = new VideoCaptureCore(); // o VideoEditCore
core.Output_Format = webmOutput;
core.Output_Filename = "output.webm";
```

### Configuración de calidad de video

Ajustar la calidad de su video WebM implica equilibrar varios parámetros:

```csharp
var webmOutput = new WebMOutput();

// Parámetros de calidad
webmOutput.Video_MinQuantizer = 4;    // Valores más bajos = mayor calidad (rango: 0-63)
webmOutput.Video_MaxQuantizer = 48;   // Límite superior de calidad (rango: 0-63)
webmOutput.Video_Bitrate = 2000;      // Tasa de bits objetivo en kbps

// Codificar con múltiples hilos para mejor rendimiento
webmOutput.Video_ThreadCount = 4;     // Ajustar según núcleos de CPU disponibles
```

### Control de keyframes

La configuración apropiada de keyframes es crucial para streaming y búsqueda eficientes:

```csharp
// Configuración de keyframes
webmOutput.Video_Keyframe_MinInterval = 30;   // Cuadros mínimos entre keyframes
webmOutput.Video_Keyframe_MaxInterval = 300;  // Cuadros máximos entre keyframes
webmOutput.Video_Keyframe_Mode = VP8KeyframeMode.Auto;
```

### Optimización de rendimiento

Equilibre velocidad de codificación y calidad con estos parámetros:

```csharp
// Configuración de rendimiento
webmOutput.Video_CPUUsed = 0;           // Rango: -16 a 16 (mayor = codificación más rápida, menor calidad)
webmOutput.Video_LagInFrames = 25;      // Buffer de anticipación de cuadros (mayor = mejor calidad)
webmOutput.Video_ErrorResilient = true; // Habilitar para aplicaciones de streaming
```

### Gestión de buffer

Para aplicaciones de streaming, la configuración apropiada de buffer mejora la estabilidad de reproducción:

```csharp
// Configuración de buffer
webmOutput.Video_Decoder_Buffer_Size = 6000;        // Tamaño de buffer en milisegundos
webmOutput.Video_Decoder_Buffer_InitialSize = 4000; // Nivel inicial de llenado del buffer
webmOutput.Video_Decoder_Buffer_OptimalSize = 5000; // Nivel objetivo del buffer

// Ajuste fino del control de tasa
webmOutput.Video_UndershootPct = 50;  // Permite que la tasa de bits baje del objetivo
webmOutput.Video_OvershootPct = 50;   // Permite que la tasa de bits exceda temporalmente el objetivo
```

## Implementación multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Para aplicaciones multiplataforma, VisioForge proporciona la clase [WebMOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WebMOutput.html) del namespace `VisioForge.Core.Types.X.Output`, ofreciendo mayor flexibilidad de códecs.

### Configuración básica

```csharp
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types.X.AudioEncoders;

// Crear salida WebM
var webmOutput = new WebMOutput("output.webm");

// Configurar codificador de video (VP8)
webmOutput.Video = new VP8EncoderSettings();

// Configurar codificador de audio (Vorbis)
webmOutput.Audio = new VorbisEncoderSettings();
```

### Integración con Video Capture SDK

```csharp
// Agregar salida WebM a Video Capture SDK
var core = new VideoCaptureCoreX();
core.Outputs_Add(webmOutput, true);
```

### Integración con Video Edit SDK

```csharp
// Establecer WebM como formato de salida para Video Edit SDK
var core = new VideoEditCoreX();
core.Output_Format = webmOutput;
```

### Integración con Media Blocks SDK

```csharp
// Crear codificadores
var vorbis = new VorbisEncoderSettings();
var vp9 = new VP9EncoderSettings();

// Configurar bloque de salida WebM
var webmSettings = new WebMSinkSettings("output.webm");
var webmOutput = new WebMOutputBlock(webmSettings, vp9, vorbis);

// Agregar a su pipeline
// pipeline.AddBlock(webmOutput);
```

## Guía de selección de códec

### Códecs de video

Los SDK de VisioForge soportan múltiples códecs de video para WebM:

1. **VP8**
   - Velocidad de codificación más rápida
   - Menores requisitos computacionales
   - Compatibilidad más amplia con navegadores antiguos
   - Buena calidad para video estándar

2. **VP9**
   - Mejor eficiencia de compresión (archivos 30-50% más pequeños vs. VP8)
   - Mayor calidad a la misma tasa de bits
   - Rendimiento de codificación más lento
   - Ideal para contenido de alta resolución

3. **AV1**
   - Códec de próxima generación con compresión superior
   - Máxima calidad por bit
   - Complejidad de codificación significativamente mayor
   - Mejor para situaciones donde el tiempo de codificación no es crítico

Para configuraciones específicas de códec, consulte nuestras páginas de documentación dedicadas:

- [Configuración VP8/VP9](../video-encoders/vp8-vp9.md)
- [Configuración AV1](../video-encoders/av1.md)

### Códecs de audio

Dos opciones principales de códec de audio están disponibles:

1. **Vorbis**
   - Códec establecido con buena calidad general
   - Compatible con todos los navegadores que soportan WebM
   - Opción predeterminada para la mayoría de aplicaciones

2. **Opus**
   - Calidad de audio superior, especialmente a tasas de bits bajas
   - Mejor para contenido de voz y música
   - Menor latencia para aplicaciones de streaming
   - Más eficiente para escenarios con restricciones de ancho de banda

Para configuraciones detalladas de audio, vea:

- [Configuración Vorbis](../audio-encoders/vorbis.md)
- [Configuración Opus](../audio-encoders/opus.md)

## Estrategias de optimización

### Para calidad de video

Para lograr la máxima calidad de video posible:

- Use VP9 o AV1 para codificación de video
- Establezca valores de cuantizador más bajos (mayor calidad)
- Aumente `LagInFrames` para mejor análisis de anticipación
- Use codificación de 2 pasadas para procesamiento de video sin conexión
- Establezca tasas de bits más altas para contenido visual complejo

```csharp
// Configuración VP9 enfocada en calidad
var vp9 = new VP9EncoderSettings
{
    Bitrate = 3000,      // Tasa de bits más alta para mejor calidad
    Speed = 0,           // Codificación más lenta/mayor calidad
}
```

### Para aplicaciones en tiempo real

Cuando la baja latencia es crítica:

- Elija VP8 para codificación más rápida
- Use codificación de una pasada
- Establezca `CPUUsed` a valores más altos
- Use buffers de anticipación de cuadros más pequeños
- Configure intervalos de keyframe más cortos

```csharp
// Configuración VP8 de baja latencia
var vp8 = new VP8EncoderSettings
{
    EndUsage = VP8EndUsageMode.CBR,  // Tasa de bits constante para streaming predecible
    Speed = 8,                        // Codificación más rápida
    Deadline = VP8Deadline.Realtime,  // Priorizar velocidad sobre calidad
    ErrorResilient = true             // Mejor recuperación de pérdida de paquetes
};
```

### Para eficiencia de tamaño de archivo

Para minimizar requisitos de almacenamiento:

- Use VP9 o AV1 para máxima compresión
- Habilite codificación de dos pasadas
- Establezca tasas de bits objetivo apropiadas
- Use codificación de tasa de bits variable (VBR)
- Evite keyframes innecesarios

```csharp
// Configuración optimizada para almacenamiento
var av1 = new AV1EncoderSettings
{
    EndUsage = AOMEndUsage.VBR,    // Tasa de bits variable para eficiencia
    TwoPass = true,                // Habilitar codificación multi-pasada
    CpuUsed = 2,                   // Balance entre velocidad y compresión
    KeyframeMaxDistance = 300      // Menos keyframes = archivos más pequeños
};
```

## Dependencias

Para implementar salida WebM, agregue los paquetes NuGet apropiados a su proyecto:

- Para plataformas x86: [VisioForge.DotNet.Core.Redist.WebM.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86)
- Para plataformas x64: [VisioForge.DotNet.Core.Redist.WebM.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x64)

## Recursos de aprendizaje

Para ejemplos de implementación adicionales y escenarios más avanzados, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) que contiene código de ejemplo para todos los SDK de VisioForge.
