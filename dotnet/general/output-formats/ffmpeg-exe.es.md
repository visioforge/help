---
title: Integración de FFMPEG.exe con SDKs .NET
description: Configure salida FFMPEG.exe en .NET para captura y edición de video con aceleración de hardware, códecs personalizados y opciones de codificación profesional.
---

# Integración de FFMPEG.exe con los SDK .Net de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Introducción a salida FFMPEG en .NET

Esta guía proporciona instrucciones detalladas para implementar salida FFMPEG.exe en aplicaciones Windows usando los SDK .NET de VisioForge. La integración funciona tanto con [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) como con [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net), utilizando los motores `VideoCaptureCore` y `VideoEditCore`.

FFMPEG funciona como un poderoso framework multimedia que permite a los desarrolladores generar salida a una amplia variedad de formatos de video y audio. Su flexibilidad proviene del extenso soporte de códecs y control granular sobre parámetros de codificación tanto para flujos de video como de audio.

## ¿Por qué usar FFMPEG con los SDK de VisioForge?

Integrar FFMPEG en sus aplicaciones potenciadas por VisioForge proporciona varias ventajas técnicas:

- **Versatilidad de formatos**: Soporte para virtualmente todos los formatos de contenedor modernos
- **Flexibilidad de códecs**: Acceso a códecs tanto de código abierto como propietarios
- **Optimización de rendimiento**: Opciones para aceleración CPU y GPU
- **Profundidad de personalización**: Control detallado sobre parámetros de codificación
- **Compatibilidad multiplataforma**: Salida consistente en diferentes sistemas

## Características y capacidades clave

### Formatos de salida soportados

FFMPEG soporta numerosos formatos de contenedor, incluyendo pero no limitado a:

- MP4 (MPEG-4 Part 14)
- WebM (VP8/VP9 con Vorbis/Opus)
- MKV (Matroska)
- AVI (Audio Video Interleave)
- MOV (QuickTime)
- WMV (Windows Media Video)
- FLV (Flash Video)
- TS (MPEG Transport Stream)

### Opciones de aceleración de hardware

La codificación de video moderna se beneficia de tecnologías de aceleración de hardware que mejoran significativamente la velocidad y eficiencia de codificación:

- **Intel QuickSync**: Aprovecha gráficos integrados Intel para codificación H.264 y HEVC
- **NVIDIA NVENC**: Utiliza GPUs NVIDIA para codificación acelerada (requiere tarjeta gráfica NVIDIA compatible)
- **AMD AMF/VCE**: Emplea procesadores gráficos AMD para aceleración de codificación

### Soporte de códecs de video

La integración ofrece acceso a múltiples códecs de video con parámetros personalizables:

- **H.264/AVC**: Estándar de la industria con excelente relación calidad-tamaño
- **H.265/HEVC**: Códec de mayor eficiencia para contenido 4K+
- **VP9**: Códec de video abierto de Google usado en WebM
- **AV1**: Códec abierto de próxima generación (donde sea soportado)
- **MPEG-2**: Códec heredado para compatibilidad con DVD y transmisión
- **ProRes**: Códec profesional para flujos de trabajo de edición

## Proceso de implementación

### 1. Configuración de su entorno de desarrollo

Antes de implementar salida FFMPEG, asegúrese de que su entorno de desarrollo esté correctamente configurado:

1. Cree un nuevo proyecto .NET o abra uno existente
2. Instale los paquetes NuGet apropiados del SDK de VisioForge
3. Agregue los paquetes de dependencia FFMPEG (detallados en la sección Dependencias)
4. Importe los namespaces necesarios en su código:

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEdit;
```

### 2. Inicialización de salida FFMPEG

Comience creando una instancia de `FFMPEGEXEOutput` para manejar su configuración de salida:

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
```

Este objeto servirá como el contenedor para todos sus ajustes y preferencias de codificación.

### 3. Configuración del formato de contenedor de salida

Establezca su formato de contenedor de salida deseado usando la propiedad `OutputMuxer`:

```csharp
ffmpegOutput.OutputMuxer = OutputMuxer.MP4;
```

Otras opciones comunes de contenedor incluyen:

- `OutputMuxer.MKV` - Para contenedor Matroska
- `OutputMuxer.WebM` - Para formato WebM
- `OutputMuxer.AVI` - Para formato AVI
- `OutputMuxer.MOV` - Para contenedor QuickTime

### 4. Configuración del codificador de video

FFMPEG proporciona múltiples opciones de codificador de video. Seleccione y configure el codificador apropiado según sus requisitos y hardware disponible:

#### Codificación H.264 estándar basada en CPU

```csharp
var videoEncoder = new H264MFSettings
{
    Bitrate = 5000000,
    RateControlMode = RateControlMode.CBR
};
ffmpegOutput.Video = videoEncoder;
```

#### Codificación NVIDIA acelerada por hardware

```csharp
var nvidiaEncoder = new H264NVENCSettings
{
    Bitrate = 8000000,        // 8 Mbps
};
ffmpegOutput.Video = nvidiaEncoder;
```

#### Codificación Intel QuickSync acelerada por hardware

```csharp
var intelEncoder = new H264QSVSettings
{
    Bitrate = 6000000
};
ffmpegOutput.Video = intelEncoder;
```

#### Codificación HEVC/H.265 para mayor eficiencia

```csharp
var hevcEncoder = new HEVCQSVSettings
{
    Bitrate = 3000000,  
};
ffmpegOutput.Video = hevcEncoder;
```

### 5. Configuración del codificador de audio

Configure sus ajustes de codificación de audio según requisitos de calidad y compatibilidad de plataforma objetivo:

```csharp
var audioEncoder = new BasicAudioSettings
{
    Bitrate = 192000,    // 192 kbps
    Channels = 2,        // Estéreo
    SampleRate = 48000,  // 48 kHz - estándar profesional
    Encoder = AudioEncoder.AAC,
    Mode = AudioMode.CBR
};

ffmpegOutput.Audio = audioEncoder;
```

### 6. Configuración final y ejecución

Aplique todos los ajustes e inicie el proceso de codificación:

```csharp
// Aplicar configuración de formato
core.Output_Format = ffmpegOutput;

// Establecer modo de operación
core.Mode = VideoCaptureMode.VideoCapture;  // Para Video Capture SDK
// core.Mode = VideoEditMode.Convert;       // Para Video Edit SDK

// Establecer ruta de salida
core.Output_Filename = "output.mp4";

// Comenzar procesamiento
await core.StartAsync();
```

## Dependencias requeridas

Instale los siguientes paquetes NuGet según su arquitectura objetivo para asegurar funcionalidad apropiada:

### Dependencias de Video Capture SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x64
```

Para objetivos x86:

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x86
```

### Dependencias de Video Edit SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x64
```

Para objetivos x86:

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x86
```

## Solución de problemas y optimización

### Problemas comunes y soluciones

- **Errores de códec no encontrado**: Asegúrese de haber instalado el paquete FFMPEG correcto con soporte de códec apropiado
- **Fallos de aceleración de hardware**: Verifique compatibilidad de GPU y versiones de controladores
- **Problemas de rendimiento**: Ajuste conteo de hilos y preset de codificación según recursos de CPU disponibles
- **Problemas de calidad de salida**: Ajuste tasa de bits, perfil y parámetros de codificación

### Consejos de optimización de rendimiento

- Use aceleración de hardware cuando esté disponible
- Elija presets apropiados según sus requisitos de calidad/velocidad
- Establezca tasas de bits razonables según el tipo de contenido y resolución
- Considere codificación de dos pasadas para escenarios no en tiempo real que requieren máxima calidad

## Recursos adicionales

Para más ejemplos de código e implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

Para aprender más sobre parámetros y capacidades de FFMPEG, consulte la [documentación oficial de FFMPEG](https://ffmpeg.org/documentation.html).
