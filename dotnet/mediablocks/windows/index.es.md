---
title: Bloques de Plataforma Windows para .Net
description: Bloques de plataforma Windows para captura y procesamiento de video/audio usando DirectShow, MediaFoundation y Direct3D en Media Blocks SDK .NET.
---

# Bloques de Plataforma Windows - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Los bloques de plataforma Windows proporcionan funcionalidad específica para sistemas Windows. Estos bloques aprovechan las APIs nativas de Windows como DirectShow, Media Foundation, WASAPI, Direct3D 11 y DXVA para proporcionar captura y procesamiento de medios optimizados.

## Bloques de Fuente de Video Windows

### Fuente de Video DirectShow

DirectShow proporciona acceso a una amplia variedad de dispositivos de captura de video en Windows, incluyendo webcams USB, tarjetas de captura y sintonizadores de TV.

#### Configuración

Use `VideoCaptureDeviceSourceSettings` con la API `DirectShow` para acceder a dispositivos DirectShow.

#### Enumerar dispositivos

```csharp
// Enumerar dispositivos DirectShow
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync(VideoCaptureDeviceAPI.DirectShow);
foreach (var device in devices)
{
    Console.WriteLine($"Dispositivo: {device.Name}");
    foreach (var format in device.VideoFormats)
    {
        Console.WriteLine($"  Formato: {format.Width}x{format.Height} @ {format.FrameRateList[0]}fps");
    }
}
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Seleccionar dispositivo DirectShow
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync(VideoCaptureDeviceAPI.DirectShow);
var device = devices[0];

var videoSettings = new VideoCaptureDeviceSourceSettings(device)
{
    Format = device.VideoFormats[0].ToFormat()
};
videoSettings.Format.FrameRate = device.VideoFormats[0].FrameRateList[0];

var videoSource = new SystemVideoSourceBlock(videoSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

---

### Fuente de Video Media Foundation

Media Foundation es la API de medios moderna de Microsoft, disponible en Windows Vista y versiones posteriores.

#### Configuración

Use `VideoCaptureDeviceSourceSettings` con la API `MediaFoundation` para acceder a dispositivos MF.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Seleccionar dispositivo Media Foundation
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync(VideoCaptureDeviceAPI.MediaFoundation);
var device = devices[0];

var videoSettings = new VideoCaptureDeviceSourceSettings(device)
{
    Format = device.VideoFormats[0].ToFormat()
};

var videoSource = new SystemVideoSourceBlock(videoSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

---

## Bloques de Captura de Pantalla Windows

### Captura de Pantalla DirectX 9

El `ScreenCaptureDX9SourceBlock` usa DirectX 9 para captura de pantalla.

#### Configuración

`ScreenCaptureDX9SourceSettings` propiedades clave:
- `Monitor` (int): Índice del monitor a capturar.
- `FrameRate` (`VideoFrameRate`): Tasa de fotogramas de captura.
- `CaptureMouseCursor` (bool): Si capturar el cursor del ratón.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var screenSettings = new ScreenCaptureDX9SourceSettings
{
    Monitor = 0,
    FrameRate = new VideoFrameRate(30),
    CaptureMouseCursor = true
};

var screenSource = new ScreenSourceBlock(screenSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(screenSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

---

### Captura de Pantalla Direct3D 11 (Desktop Duplication)

El `ScreenCaptureD3D11SourceBlock` usa la API Desktop Duplication de Windows 8+ para captura de pantalla de alto rendimiento.

#### Configuración

`ScreenCaptureD3D11SourceSettings` propiedades clave:
- `Monitor` (int): Índice del monitor a capturar.
- `FrameRate` (`VideoFrameRate`): Tasa de fotogramas de captura.
- `CaptureMouseCursor` (bool): Si capturar el cursor del ratón.
- `WindowHandle` (IntPtr): Handle de ventana específica a capturar (opcional).

#### Características

- Mejor rendimiento que DirectX 9
- Soporte para captura de ventana específica
- Menor uso de CPU

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var screenSettings = new ScreenCaptureD3D11SourceSettings
{
    Monitor = 0,
    FrameRate = new VideoFrameRate(60),
    CaptureMouseCursor = true
};

var screenSource = new ScreenSourceBlock(screenSettings);

// Codificar y guardar
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());
pipeline.Connect(screenSource.Output, h264Encoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("screen_capture.mp4"));
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

---

### Captura de Pantalla GDI

El `ScreenCaptureGDISourceBlock` usa GDI para captura de pantalla. Es compatible con versiones antiguas de Windows pero tiene menor rendimiento.

#### Configuración

`ScreenCaptureGDISourceSettings` propiedades clave:
- `Monitor` (int): Índice del monitor a capturar.
- `FrameRate` (`VideoFrameRate`): Tasa de fotogramas de captura.
- `CaptureMouseCursor` (bool): Si capturar el cursor del ratón.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var screenSettings = new ScreenCaptureGDISourceSettings
{
    Monitor = 0,
    FrameRate = new VideoFrameRate(15),
    CaptureMouseCursor = true
};

var screenSource = new ScreenSourceBlock(screenSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(screenSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

---

## Bloques de Fuente de Audio Windows

### Fuente de Audio WASAPI

WASAPI (Windows Audio Session API) proporciona acceso de baja latencia a dispositivos de audio.

#### Captura de Audio

```csharp
var pipeline = new MediaBlocksPipeline();

// Enumerar dispositivos de audio WASAPI
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.WASAPI2);
var device = devices[0];

if (device != null)
{
    var format = device.Formats[0];
    var audioSettings = device.CreateSourceSettings(format.ToFormat());
    
    var audioSource = new SystemAudioSourceBlock(audioSettings);
    
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(audioSource.Output, audioRenderer.Input);
    
    await pipeline.StartAsync();
}
```

#### Captura Loopback (Audio de Altavoces)

```csharp
var pipeline = new MediaBlocksPipeline();

// Obtener dispositivo de salida de audio para loopback
var outputDevices = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2);
var outputDevice = outputDevices[0];

if (outputDevice != null)
{
    // Crear configuración de loopback
    var loopbackSettings = new LoopbackAudioCaptureDeviceSourceSettings(outputDevice);
    
    var audioSource = new SystemAudioSourceBlock(loopbackSettings);
    
    // Guardar a archivo o procesar
    var mp3Encoder = new MP3EncoderBlock(new MP3EncoderSettings { Bitrate = 192 });
    pipeline.Connect(audioSource.Output, mp3Encoder.Input);
    
    var fileSink = new FileSinkBlock("loopback_audio.mp3");
    pipeline.Connect(mp3Encoder.Output, fileSink.Input);
    
    await pipeline.StartAsync();
}
```

---

### Fuente de Audio DirectSound

DirectSound proporciona compatibilidad con dispositivos de audio más antiguos.

```csharp
var pipeline = new MediaBlocksPipeline();

// Enumerar dispositivos DirectSound
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.DirectSound);
var device = devices[0];

if (device != null)
{
    var format = device.Formats[0];
    var audioSettings = device.CreateSourceSettings(format.ToFormat());
    
    var audioSource = new SystemAudioSourceBlock(audioSettings);
    
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(audioSource.Output, audioRenderer.Input);
    
    await pipeline.StartAsync();
}
```

---

## Decodificadores Acelerados por Hardware D3D11/DXVA

### Bloque Decodificador D3D11 H.264

El `D3D11H264DecoderBlock` proporciona decodificación acelerada por hardware usando DirectX Video Acceleration (DXVA).

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
|-----------------|---------------------|------------|
| Entrada video | Video codificado H.264 | 1 |
| Salida video | Video sin comprimir | 1 |

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new BasicFileSourceBlock("video.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("video.mp4");
var mediaInfo = reader.Info;

var demux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

// Usar decodificador D3D11
var d3d11Decoder = new D3D11H264DecoderBlock();

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(fileSource.Output, demux.Input);
pipeline.Connect(demux.GetVideoOutput(), d3d11Decoder.Input);
pipeline.Connect(d3d11Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = D3D11H264DecoderBlock.IsAvailable();
```

---

### Bloque Decodificador D3D11 HEVC

El `D3D11H265DecoderBlock` proporciona decodificación HEVC acelerada por hardware.

#### Disponibilidad

```csharp
bool available = D3D11H265DecoderBlock.IsAvailable();
```

---

### Bloque Decodificador D3D11 VP9

El `D3D11VP9DecoderBlock` proporciona decodificación VP9 acelerada por hardware.

#### Disponibilidad

```csharp
bool available = D3D11VP9DecoderBlock.IsAvailable();
```

---

### Bloque Decodificador D3D11 AV1

El `D3D11AV1DecoderBlock` proporciona decodificación AV1 acelerada por hardware.

#### Disponibilidad

```csharp
bool available = D3D11AV1DecoderBlock.IsAvailable();
```

---

## Codificadores Media Foundation

### Codificador HEVC Media Foundation

El `MFHEVCEncoderBlock` usa el codificador HEVC incorporado de Windows.

#### Configuración

`MFHEVCEncoderSettings` propiedades clave:
- `Bitrate` (int): Bitrate objetivo en bps.
- `Quality` (int): Nivel de calidad (para modo de calidad).
- `Profile` (`HEVCProfile`): Perfil HEVC a usar.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Usar codificador HEVC de Media Foundation
var hevcEncoder = new HEVCEncoderBlock(new MFHEVCEncoderSettings
{
    Bitrate = 5000000 // 5 Mbps
});

pipeline.Connect(videoSource.Output, hevcEncoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(hevcEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

---

## Codificadores NVIDIA NVENC

### Codificador NVENC H.264

El `NVENCH264EncoderBlock` proporciona codificación H.264 acelerada por GPU NVIDIA.

#### Configuración

`NVENCH264EncoderSettings` propiedades clave:
- `Bitrate` (int): Bitrate objetivo.
- `RateControl` (enum): Modo de control de tasa.
- `Preset` (enum): Preset de codificación (calidad vs velocidad).
- `Profile` (enum): Perfil H.264.
- `Level` (enum): Nivel H.264.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Usar codificador NVIDIA NVENC
var nvencEncoder = new H264EncoderBlock(new NVENCH264EncoderSettings
{
    Bitrate = 8000000, // 8 Mbps
    Preset = NVENCPreset.Default
});

pipeline.Connect(videoSource.Output, nvencEncoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(nvencEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = NVENCH264EncoderSettings.IsAvailable();
```

---

## Codificadores Intel QuickSync

### Codificador QSV H.264

El `QSVH264EncoderBlock` proporciona codificación H.264 acelerada por GPU Intel.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Usar codificador Intel QuickSync
var qsvEncoder = new H264EncoderBlock(new QSVH264EncoderSettings
{
    Bitrate = 5000000 // 5 Mbps
});

pipeline.Connect(videoSource.Output, qsvEncoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(qsvEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

---

## Codificadores AMD AMF

### Codificador AMF H.264

El `AMFH264EncoderBlock` proporciona codificación H.264 acelerada por GPU AMD.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Usar codificador AMD AMF
var amfEncoder = new H264EncoderBlock(new AMFH264EncoderSettings
{
    Bitrate = 5000000 // 5 Mbps
});

pipeline.Connect(videoSource.Output, amfEncoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(amfEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

---

## Renderizadores de Video Windows

### Renderizador Direct3D 11

El renderizador D3D11 proporciona renderizado de video de alto rendimiento con aceleración por GPU.

#### Características

- Renderizado acelerado por GPU
- Soporte para HDR
- Escalado y transformación eficientes
- Sincronización vertical

### Renderizador D3D9

El renderizador D3D9 proporciona compatibilidad con hardware más antiguo.

### Renderizador EVR (Enhanced Video Renderer)

El EVR es el renderizador recomendado para aplicaciones WPF.

#### Selección automática

El SDK selecciona automáticamente el mejor renderizador disponible:

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// El renderizador se selecciona automáticamente
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

---

## Consideraciones para Plataforma Windows

### Versiones de Windows Soportadas

- Windows 10 versión 1809 o posterior (recomendado)
- Windows 11
- Windows Server 2019, 2022

### Redistribuibles Requeridos

- Visual C++ Redistributable 2015-2022
- GStreamer Runtime

### Aceleración por Hardware

| GPU | API | Códecs Soportados |
|-----|-----|-------------------|
| NVIDIA | NVENC/NVDEC | H.264, HEVC, VP9, AV1 |
| Intel | QuickSync | H.264, HEVC, VP9, AV1 |
| AMD | AMF | H.264, HEVC, AV1 |
| Cualquiera | D3D11/DXVA | H.264, HEVC, VP9, AV1 |

### Verificar soporte de hardware

```csharp
// Verificar NVIDIA
bool nvencAvailable = NVENCH264EncoderSettings.IsAvailable();

// Verificar Intel QuickSync
bool qsvAvailable = QSVH264EncoderSettings.IsAvailable();

// Verificar AMD AMF
bool amfAvailable = AMFH264EncoderSettings.IsAvailable();

// Verificar D3D11
bool d3d11Available = D3D11H264DecoderBlock.IsAvailable();
```

### Frameworks de UI Soportados

- WPF
- WinForms
- WinUI 3
- UWP (limitado)
- Console Applications
