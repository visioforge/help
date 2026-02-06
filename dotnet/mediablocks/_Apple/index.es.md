---
title: Bloques de Plataforma Apple para .Net
description: Bloques específicos de plataforma Apple para captura y procesamiento de video/audio en iOS y macOS con Media Blocks SDK para .NET.
---

# Bloques de Plataforma Apple - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Los bloques de plataforma Apple proporcionan funcionalidad específica para dispositivos iOS y macOS. Estos bloques aprovechan los frameworks nativos de Apple como AVFoundation y VideoToolbox para proporcionar captura y procesamiento de medios optimizados.

## Bloques de Fuente iOS

### Bloque de Fuente de Video iOS

El `IOSVideoSourceBlock` proporciona captura de video desde la cámara del dispositivo en plataformas iOS.

#### Información del bloque

Nombre: IOSVideoSourceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida video | Video sin comprimir | 1 |

#### Configuración

`IOSVideoSourceBlock` se configura usando `VideoCaptureDeviceSourceSettings` que hereda de configuraciones genéricas de captura de video.

Propiedades clave:
- `Device` (`VideoCaptureDeviceInfo`): El dispositivo de cámara a usar.
- `Format` (`VideoCaptureDeviceFormat`): El formato de video seleccionado incluyendo resolución y tasa de fotogramas.

#### Enumerar dispositivos disponibles

```csharp
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
foreach (var device in devices)
{
    Console.WriteLine($"Dispositivo: {device.Name}");
    foreach (var format in device.VideoFormats)
    {
        Console.WriteLine($"  Formato: {format.Width}x{format.Height}");
    }
}
```

#### Código de ejemplo

```csharp
// crear pipeline
var pipeline = new MediaBlocksPipeline();

// seleccionar el primer dispositivo de video disponible
var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
VideoCaptureDeviceSourceSettings videoSourceSettings = null;
if (device != null)
{
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };
        videoSourceSettings.Format.FrameRate = formatItem.FrameRateList[0];
    }
}

// crear bloque de fuente de video iOS
var videoSource = new IOSVideoSourceBlock(videoSourceSettings);

// crear bloque de renderizado de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// conectar bloques
pipeline.Connect(videoSource.Output, videoRenderer.Input);

// iniciar pipeline
await pipeline.StartAsync();
```

#### Plataformas

iOS (no disponible en macOS Catalyst)

---

### Bloque de Fuente de Pantalla iOS

El `IOSScreenSourceBlock` permite capturar la pantalla del dispositivo iOS. Esto es útil para aplicaciones de grabación de pantalla.

#### Información del bloque

Nombre: IOSScreenSourceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida video | Video sin comprimir | 1 |

#### Configuración

`IOSScreenSourceBlock` se configura usando `IOSScreenSourceSettings`.

Propiedades clave:
- `FrameRate` (`VideoFrameRate`): La tasa de fotogramas de captura deseada.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar fuente de pantalla iOS
var screenSettings = new IOSScreenSourceSettings
{
    FrameRate = new VideoFrameRate(30)
};

var screenSource = new IOSScreenSourceBlock(screenSettings);

// Codificador H264
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());
pipeline.Connect(screenSource.Output, h264Encoder.Input);

// Sink MP4
var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("screen_recording.mp4"));
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Plataformas

iOS

---

## Bloques de Fuente macOS

### Bloque de Fuente de Audio macOS

El `OSXAudioSourceBlock` proporciona captura de audio desde dispositivos de entrada en plataformas macOS.

#### Información del bloque

Nombre: OSXAudioSourceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida audio | Audio sin comprimir | 1 |

#### Configuración

`OSXAudioSourceBlock` se configura usando `OSXAudioSourceSettings`.

Propiedades clave:
- `DeviceID` (int): ID del dispositivo de audio.
- `Format` (`AudioCaptureDeviceFormat`): Formato de audio (tasa de muestreo, canales, etc.).

#### Enumerar dispositivos disponibles

```csharp
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
foreach (var device in devices)
{
    Console.WriteLine($"Dispositivo de Audio: {device.Name}, ID: {device.DeviceID}");
    foreach (var format in device.Formats)
    {
        Console.WriteLine($"  Formato: {format.SampleRate}Hz, {format.Channels} canales");
    }
}
```

#### Código de ejemplo

```csharp
// crear pipeline
var pipeline = new MediaBlocksPipeline();

// seleccionar el primer dispositivo de audio disponible
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var device = devices.Length > 0 ? devices[0] : null;
OSXAudioSourceSettings audioSourceSettings = null;
if (device != null)
{
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        audioSourceSettings = new OSXAudioSourceSettings(device.DeviceID, formatItem);
    }
}

// crear bloque de fuente de audio macOS
var audioSource = new OSXAudioSourceBlock(audioSourceSettings);

// crear bloque de renderizado de audio
var audioRenderer = new AudioRendererBlock();

// conectar bloques
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// iniciar pipeline
await pipeline.StartAsync();
```

#### Plataformas

macOS (no disponible en iOS)

---

### Bloque de Fuente de Video macOS

El `OSXVideoSourceBlock` proporciona captura de video desde cámaras y otros dispositivos de captura de video en plataformas macOS.

#### Información del bloque

Nombre: OSXVideoSourceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida video | Video sin comprimir | 1 |

#### Configuración

`OSXVideoSourceBlock` utiliza las configuraciones estándar de `VideoCaptureDeviceSourceSettings`.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Enumerar y seleccionar dispositivo de video
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var device = devices[0];

var videoSettings = new VideoCaptureDeviceSourceSettings(device)
{
    Format = device.VideoFormats[0].ToFormat()
};
videoSettings.Format.FrameRate = device.VideoFormats[0].FrameRateList[0];

var videoSource = new OSXVideoSourceBlock(videoSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

macOS

---

## Codificadores Específicos de Apple

### Bloque Codificador Apple ProRes

El `AppleProResEncoderBlock` codifica video usando el códec Apple ProRes, que es ampliamente usado en flujos de trabajo de producción de video profesional.

#### Información del bloque

Nombre: AppleProResEncoderBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada | Video sin comprimir | 1 |
| Salida | ProRes | 1 |

#### Configuración

`AppleProResEncoderBlock` se configura usando `AppleProResEncoderSettings`.

Propiedades clave:
- `Profile` (`AppleProResProfile`): El perfil ProRes a usar. Opciones incluyen:
  - `ProRes422Proxy`: Menor bitrate, proxy de edición.
  - `ProRes422LT`: Bitrate ligero.
  - `ProRes422`: Estándar.
  - `ProRes422HQ`: Alta calidad.
  - `ProRes4444`: Con canal alfa.
  - `ProRes4444XQ`: Máxima calidad con alfa.

#### Pipeline de ejemplo

```mermaid
graph LR;
    UniversalSourceBlock-->AppleProResEncoderBlock;
    AppleProResEncoderBlock-->MOVSinkBlock;
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

// Configurar codificador ProRes con perfil de alta calidad
var proResSettings = new AppleProResEncoderSettings
{
    Profile = AppleProResProfile.ProRes422HQ
};

var proResEncoder = new AppleProResEncoderBlock(proResSettings);
pipeline.Connect(fileSource.VideoOutput, proResEncoder.Input);

// Guardar a archivo MOV
var movSink = new MOVSinkBlock(new MOVSinkSettings(@"output.mov"));
pipeline.Connect(proResEncoder.Output, movSink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Disponibilidad

Puede verificar si el codificador Apple ProRes está disponible usando:

```csharp
bool available = AppleProResEncoderBlock.IsAvailable();
```

#### Plataformas

macOS, iOS.

---

## Decodificadores Específicos de Apple

### Bloque Decodificador VideoToolbox H.264

El decodificador VideoToolbox H.264 utiliza la aceleración por hardware de Apple para decodificación de video de alto rendimiento.

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada video | Video codificado H.264 | 1 |
| Salida video | Video sin comprimir | 1 |

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video_h264.mp4")));

// El SDK selecciona automáticamente VideoToolbox cuando está disponible en plataformas Apple
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

macOS, iOS.

---

### Bloque Decodificador VideoToolbox HEVC

El decodificador VideoToolbox HEVC proporciona decodificación acelerada por hardware para contenido H.265/HEVC.

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada video | Video codificado H.265/HEVC | 1 |
| Salida video | Video sin comprimir | 1 |

#### Plataformas

macOS, iOS (en dispositivos con soporte de hardware HEVC).

---

## Renderizadores Específicos de Apple

### Renderizador de Video Metal

En plataformas Apple, el SDK puede usar Metal para renderizado de video de alto rendimiento. Esto se configura automáticamente cuando está disponible.

#### Características

- Renderizado acelerado por GPU usando el framework Metal de Apple
- Soporte para HDR y amplia gama de colores
- Escalado y transformación eficientes
- Bajo consumo de batería en dispositivos móviles

#### Uso

El renderizador Metal se selecciona automáticamente cuando usa `VideoRendererBlock` en plataformas Apple compatibles.

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new IOSVideoSourceBlock(videoSettings);

// El renderizador usará Metal automáticamente cuando esté disponible
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

macOS 10.11+, iOS 8+.

---

## Consideraciones para Plataformas Apple

### Permisos de Cámara y Micrófono

En iOS y macOS, debe declarar el uso de cámara y micrófono en el archivo Info.plist:

```xml
<key>NSCameraUsageDescription</key>
<string>Esta app necesita acceso a la cámara para captura de video.</string>
<key>NSMicrophoneUsageDescription</key>
<string>Esta app necesita acceso al micrófono para captura de audio.</string>
```

### Grabación de Pantalla en iOS

Para captura de pantalla en iOS, debe configurar las capacidades apropiadas de Broadcast Extension en su proyecto Xcode.

### Rendimiento en Batería

El SDK utiliza automáticamente codificadores/decodificadores de hardware cuando están disponibles para minimizar el consumo de batería en dispositivos móviles Apple.

### Formatos Soportados

Los dispositivos Apple soportan eficientemente los siguientes formatos:
- **Video**: H.264, HEVC (H.265), ProRes
- **Audio**: AAC, ALAC, MP3
- **Contenedores**: MOV, MP4, M4A
