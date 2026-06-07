---
title: Aceleración por hardware VA-API en Linux con C# .NET
description: Codificación y decodificación de video por GPU mediante VA-API en Linux con Media Blocks SDK — Ubuntu y Debian en aplicaciones .NET.
sidebar_label: Linux Platform
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - VAAPIH264DecoderBlock
  - UniversalSourceBlock
  - UniversalSourceSettings
  - VideoRendererBlock
  - VAAPIHEVCDecoderBlock

---

# Bloques de plataforma Linux - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección cubre los MediaBlocks específicamente optimizados para plataformas Linux.

## Bloques disponibles

### Decodificadores de hardware VA-API

Linux proporciona decodificación de video acelerada por hardware mediante VA-API (Video Acceleration API):

- **VAAPIH264DecoderBlock**: decodificación H.264/AVC por hardware
- **VAAPIHEVCDecoderBlock**: decodificación H.265/HEVC por hardware
- **VAAPIJPEGDecoderBlock**: decodificación JPEG por hardware
- **VAAPIVC1DecoderBlock**: decodificación VC-1 por hardware

Consulte la [documentación de decodificadores de video](../VideoDecoders/index.md)

## Requisitos de plataforma

- **Linux**: cualquier distribución Linux moderna
- **VA-API**: libva y el controlador apropiado para su GPU
- **Hardware**: GPU Intel, AMD u otra con soporte VA-API

## Características

- **Aceleración por hardware**: decodificación de video basada en GPU
- **Bajo uso de CPU**: descarga la decodificación al hardware dedicado
- **Amplia compatibilidad**: funciona con GPUs Intel, AMD y otras
- **Eficiencia energética**: reduce el consumo de energía
- **Multi-flujo**: gestiona varios flujos HD/4K

## Código de ejemplo

### Decodificación H.264 por hardware

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// Decodificador VA-API por hardware
if (VAAPIH264DecoderBlock.IsAvailable())
{
    var vaapiDecoder = new VAAPIH264DecoderBlock();
    pipeline.Connect(fileSource.VideoOutput, vaapiDecoder.Input);
    
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    pipeline.Connect(vaapiDecoder.Output, videoRenderer.Input);
}
else
{
    // Recurrir al decodificador por software
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
    pipeline.Connect(fileSource.VideoOutput, decoder.Input);
    
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);
}

await pipeline.StartAsync();
```

### Varios decodificadores de hardware

```csharp
var pipeline = new MediaBlocksPipeline();

// Decodificar flujo H.264
var h264Source = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("h264.mp4"));
var h264Decoder = new VAAPIH264DecoderBlock();
pipeline.Connect(h264Source.VideoOutput, h264Decoder.Input);

// Decodificar flujo H.265
var h265Source = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("h265.mp4"));
var h265Decoder = new VAAPIHEVCDecoderBlock();
pipeline.Connect(h265Source.VideoOutput, h265Decoder.Input);

// Mezclar ambos flujos (ejemplo)
var mixer = new VideoMixerBlock(mixerSettings);
pipeline.Connect(h264Decoder.Output, mixer.Inputs[0]);
pipeline.Connect(h265Decoder.Output, mixer.Inputs[1]);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixer.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Instalación y configuración

### Instalar VA-API

**Ubuntu/Debian:**
```bash
sudo apt-get install libva2 libva-drm2 vainfo
```

**Fedora/RHEL:**
```bash
sudo dnf install libva libva-utils
```

### Comprobar el soporte VA-API

```bash
vainfo
```

Este comando muestra los perfiles y entrypoints VA-API disponibles.

### Instalación de controladores

**GPUs Intel:**
```bash
# Ubuntu/Debian
sudo apt-get install intel-media-va-driver

# Fedora/RHEL
sudo dnf install intel-media-driver
```

**GPUs AMD:**
```bash
# Ubuntu/Debian
sudo apt-get install mesa-va-drivers

# Fedora/RHEL
sudo dnf install mesa-va-drivers
```

## Comprobar el soporte de hardware en código

```csharp
// Comprobar si el decodificador VA-API H.264 está disponible
if (VAAPIH264DecoderBlock.IsAvailable())
{
    Console.WriteLine("Decodificación H.264 por hardware VA-API disponible");
    var decoder = new VAAPIH264DecoderBlock();
}
else
{
    Console.WriteLine("VA-API no disponible, usando decodificador por software");
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
}
```

## Consejos de rendimiento

- Asegúrese de que los controladores VA-API están instalados correctamente
- Use decodificadores por hardware cuando estén disponibles para mejor rendimiento
- Compruebe la disponibilidad del decodificador antes de crear bloques
- Monitorice el uso de memoria GPU al procesar varios flujos
- Use el comando `vainfo` para verificar el soporte de hardware

## Resolución de problemas

**VA-API no funciona:**
1. Verifique la instalación del controlador con `vainfo`
2. Compruebe los permisos del usuario (puede necesitar pertenecer al grupo `video` o `render`)
3. Asegúrese de que los plugins GStreamer VA-API están instalados: `gstreamer1.0-vaapi`

**Problemas de permisos:**
```bash
# Añadir el usuario al grupo video
sudo usermod -a -G video $USER
# Cierre sesión y vuelva a iniciarla para que surta efecto
```

## Plataformas

Linux (Ubuntu, Debian, Fedora, RHEL, Arch y otras distribuciones).

Requiere soporte VA-API con los controladores apropiados.

## Documentación relacionada

- [VideoDecoders](../VideoDecoders/index.md) — bloques de decodificación por hardware
- [Nvidia](../Nvidia/index.md) — aceleración con GPU Nvidia (también funciona en Linux)
