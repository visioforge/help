---
title: Bloques de Plataforma Linux para .Net
description: Bloques específicos de plataforma Linux para captura y procesamiento de video/audio usando V4L2, PulseAudio, VAAPI y más en Media Blocks SDK para .NET.
---

# Bloques de Plataforma Linux - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Los bloques de plataforma Linux proporcionan funcionalidad específica para sistemas Linux. Estos bloques aprovechan las APIs nativas de Linux como V4L2 (Video4Linux2), PulseAudio, ALSA y VAAPI para proporcionar captura y procesamiento de medios optimizados.

## Bloques de Fuente de Video Linux

### Bloque de Fuente V4L2

El `V4L2SourceBlock` (Video4Linux2) proporciona captura de video desde webcams y otros dispositivos de video en Linux.

#### Información del bloque

Nombre: V4L2SourceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida video | Video sin comprimir | 1 |

#### Configuración

`V4L2SourceBlock` se configura usando configuraciones estándar de dispositivo de captura de video.

Propiedades clave:
- `Device` (`VideoCaptureDeviceInfo`): El dispositivo de video a usar.
- `Format` (`VideoCaptureDeviceFormat`): El formato de video incluyendo resolución, tasa de fotogramas y formato de píxel.

#### Enumerar dispositivos disponibles

```csharp
// Enumerar dispositivos de video en Linux
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
foreach (var device in devices)
{
    Console.WriteLine($"Dispositivo: {device.Name}");
    Console.WriteLine($"  Ruta: {device.DevicePath}");
    foreach (var format in device.VideoFormats)
    {
        Console.WriteLine($"  Formato: {format.Width}x{format.Height} @ {format.FrameRateList[0]}fps");
    }
}
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Seleccionar el primer dispositivo de video
var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
VideoCaptureDeviceSourceSettings videoSettings = null;

if (device != null)
{
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        videoSettings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };
        videoSettings.Format.FrameRate = formatItem.FrameRateList[0];
    }
}

var videoSource = new SystemVideoSourceBlock(videoSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Linux

---

### Bloque de Fuente de Pantalla XDisplay

El `ScreenCaptureXDisplaySourceBlock` captura la pantalla del escritorio en sistemas Linux usando X11.

#### Información del bloque

Nombre: ScreenCaptureXDisplaySourceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida video | Video sin comprimir | 1 |

#### Configuración

`ScreenCaptureXDisplaySourceBlock` se configura usando `ScreenCaptureXDisplaySourceSettings`.

Propiedades clave:
- `Display` (string): El nombre del display X11 (ej., ":0"). Predeterminado es el display actual.
- `Screen` (int): El número de pantalla a capturar.
- `FrameRate` (`VideoFrameRate`): La tasa de fotogramas de captura.
- `X`, `Y`, `Width`, `Height` (int): Región de captura. Si no se especifica, captura toda la pantalla.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar captura de pantalla X11
var screenSettings = new ScreenCaptureXDisplaySourceSettings
{
    Display = ":0",
    Screen = 0,
    FrameRate = new VideoFrameRate(30),
    // Opcionalmente especificar una región
    X = 0,
    Y = 0,
    Width = 1920,
    Height = 1080
};

var screenSource = new ScreenSourceBlock(screenSettings);

// Codificar y guardar
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());
pipeline.Connect(screenSource.Output, h264Encoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("screen_capture.mp4"));
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Plataformas

Linux (con servidor X11)

---

## Bloques de Fuente de Audio Linux

### Bloque de Fuente PulseAudio

El bloque de fuente PulseAudio proporciona captura de audio desde dispositivos de entrada usando el servidor de sonido PulseAudio.

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida audio | Audio sin comprimir | 1 |

#### Configuración

La configuración utiliza las configuraciones estándar de dispositivo de captura de audio con la API de PulseAudio.

#### Enumerar dispositivos disponibles

```csharp
// Enumerar dispositivos de audio en Linux (PulseAudio)
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
foreach (var device in devices)
{
    Console.WriteLine($"Dispositivo de Audio: {device.Name}");
    foreach (var format in device.Formats)
    {
        Console.WriteLine($"  Formato: {format.SampleRate}Hz, {format.Channels} canales");
    }
}
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Seleccionar el primer dispositivo de audio
var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
IAudioCaptureDeviceSourceSettings audioSettings = null;

if (device != null)
{
    var format = device.Formats[0];
    audioSettings = device.CreateSourceSettings(format.ToFormat());
}

var audioSource = new SystemAudioSourceBlock(audioSettings);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSource.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Linux (con PulseAudio instalado)

---

### Bloque de Fuente ALSA

El bloque de fuente ALSA proporciona acceso directo a dispositivos de audio usando la arquitectura ALSA (Advanced Linux Sound Architecture).

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
|---------------|:------------------:|:----------:|
| Salida audio | Audio sin comprimir | 1 |

#### Configuración

Para usar ALSA directamente, especifique la API ALSA durante la enumeración de dispositivos.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Enumerar usando ALSA
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.ALSA);
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

#### Plataformas

Linux

---

## Decodificadores Acelerados por Hardware VAAPI

### Bloque Decodificador VAAPI H.264

El `VAAPIH264DecoderBlock` proporciona decodificación acelerada por hardware de video H.264 usando VA-API (Video Acceleration API).

#### Información del bloque

Nombre: VAAPIH264DecoderBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
|-----------------|---------------------|------------|
| Entrada video | Video codificado H.264 | 1 |
| Salida video | Video sin comprimir | 1 |

#### Configuración

El decodificador VAAPI se selecciona automáticamente cuando está disponible, o puede especificarse explícitamente.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new BasicFileSourceBlock("video.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("video.mp4");
var mediaInfo = reader.Info;

var demux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

// Usar decodificador VAAPI explícitamente
var vaapiDecoder = new VAAPIH264DecoderBlock();

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(fileSource.Output, demux.Input);
pipeline.Connect(demux.GetVideoOutput(), vaapiDecoder.Input);
pipeline.Connect(vaapiDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad:

```csharp
bool available = VAAPIH264DecoderBlock.IsAvailable();
```

#### Plataformas

Linux (con controladores VA-API instalados)

---

### Bloque Decodificador VAAPI HEVC

El `VAAPIHEVCDecoderBlock` proporciona decodificación acelerada por hardware de video H.265/HEVC usando VA-API.

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
|-----------------|----------------------|------------|
| Entrada video | H.265/HEVC codificado | 1 |
| Salida video | Video sin comprimir | 1 |

#### Disponibilidad

```csharp
bool available = VAAPIHEVCDecoderBlock.IsAvailable();
```

#### Plataformas

Linux (con controladores VA-API que soporten HEVC)

---

### Bloque Decodificador VAAPI VP9

El decodificador VAAPI VP9 proporciona decodificación acelerada por hardware de video VP9.

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
|-----------------|---------------------|------------|
| Entrada video | Video codificado VP9 | 1 |
| Salida video | Video sin comprimir | 1 |

#### Plataformas

Linux (con controladores VA-API que soporten VP9)

---

## Codificadores Acelerados por Hardware VAAPI

### Bloque Codificador VAAPI H.264

El codificador VAAPI H.264 proporciona codificación de video acelerada por hardware en sistemas Linux.

#### Información del bloque

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada | Video sin comprimir | 1 |
| Salida | H.264 | 1 |

#### Configuración

Use `VAAPIH264EncoderSettings` para configurar el codificador:

- `Bitrate` (int): Bitrate objetivo en kbps.
- `RateControl` (enum): Modo de control de tasa (CBR, VBR, CQP).
- `QP` (int): Parámetro de cuantización para modo CQP.

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Usar codificador VAAPI
var vaapiEncoder = new H264EncoderBlock(new VAAPIH264EncoderSettings
{
    Bitrate = 5000 // 5 Mbps
});

pipeline.Connect(videoSource.Output, vaapiEncoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(vaapiEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Plataformas

Linux (con controladores VA-API)

---

## Renderizadores de Video Linux

### Renderizador EGL/Wayland

En sistemas Linux modernos con Wayland, el SDK puede usar EGL para renderizado eficiente.

### Renderizador GLX/X11

En sistemas con X11, el SDK puede usar GLX para renderizado OpenGL acelerado.

#### Selección automática

El SDK selecciona automáticamente el renderizador más apropiado basándose en el entorno de escritorio detectado.

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// El renderizador se adapta automáticamente a X11 o Wayland
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

---

## Consideraciones para Plataforma Linux

### Dependencias de GStreamer

El SDK depende de GStreamer en Linux. Asegúrese de que los paquetes necesarios estén instalados:

```bash
# Ubuntu/Debian
sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good \
    gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav \
    gstreamer1.0-vaapi gstreamer1.0-pulseaudio

# Fedora
sudo dnf install gstreamer1-plugins-base gstreamer1-plugins-good \
    gstreamer1-plugins-bad-free gstreamer1-plugins-ugly \
    gstreamer1-libav gstreamer1-vaapi gstreamer1-plugins-pulseaudio
```

### Permisos de Dispositivos

Los usuarios pueden necesitar permisos para acceder a dispositivos de video y audio:

```bash
# Añadir usuario al grupo video
sudo usermod -a -G video $USER

# Añadir usuario al grupo audio
sudo usermod -a -G audio $USER
```

### Variables de Entorno

Puede necesitar configurar variables de entorno para VA-API:

```bash
# Para GPUs Intel
export LIBVA_DRIVER_NAME=iHD

# Para GPUs AMD
export LIBVA_DRIVER_NAME=radeonsi
```

### Distribuciones Soportadas

El SDK ha sido probado en:
- Ubuntu 20.04, 22.04, 24.04
- Debian 11, 12
- Fedora 38, 39, 40
- CentOS Stream 8, 9
- openSUSE Leap 15.x

### Compatibilidad X11 vs Wayland

El SDK soporta tanto X11 como Wayland:
- **X11**: Soporte completo para captura de pantalla y renderizado
- **Wayland**: Soporte de renderizado; la captura de pantalla puede requerir XWayland

### Formatos de Video Soportados en Linux

- **Códecs de Video**: H.264, HEVC (H.265), VP8, VP9, AV1
- **Códecs de Audio**: AAC, MP3, Vorbis, Opus, FLAC
- **Contenedores**: MP4, MKV, WebM, AVI, OGG

### Aceleración por Hardware

| GPU | API | Códecs Soportados |
|-----|-----|-------------------|
| Intel | VA-API | H.264, HEVC, VP9, AV1 |
| AMD | VA-API | H.264, HEVC, VP9, AV1 |
| NVIDIA | NVDEC/NVENC | H.264, HEVC, VP9, AV1 |

Para verificar el soporte VA-API:

```bash
vainfo
```
