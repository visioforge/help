---
title: Bloques Fuente de Video y Audio para Pipelines C# .NET
description: Use los bloques fuente de VisioForge Media Blocks SDK para webcams, archivos, streams RTSP/RTMP, pantallas y fuentes virtuales con enumeración de dispositivos.
sidebar_label: Fuentes
---

# Bloques Fuente - SDK de VisioForge Media Blocks .Net

[SDK de Media Blocks .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Los bloques fuente proporcionan datos al pipeline y son típicamente los primeros bloques en cualquier cadena de procesamiento de medios. El SDK de VisioForge Media Blocks .Net proporciona una colección completa de bloques fuente para diversas entradas incluyendo dispositivos hardware, archivos, redes y fuentes virtuales.

## Bloques Fuente de Hardware

### Fuente de Video del Sistema

SystemVideoSourceBlock se usa para acceder a webcams y otros dispositivos de captura de video.

#### Información del bloque

Nombre: SystemVideoSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | video sin comprimir | 1 |

#### Enumerar dispositivos disponibles

Use el método `DeviceEnumerator.Shared.VideoSourcesAsync()` para obtener una lista de dispositivos disponibles y sus especificaciones: resoluciones disponibles, tasas de frames y formatos de video.
Este método devuelve una lista de objetos `VideoCaptureDeviceInfo`. Cada objeto `VideoCaptureDeviceInfo` proporciona información detallada sobre un dispositivo de captura.

#### El pipeline de muestra

```mermaid
graph LR;
    SystemVideoSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
// crear pipeline
var pipeline = new MediaBlocksPipeline();

// crear fuente de video
VideoCaptureDeviceSourceSettings videoSourceSettings = null;

// seleccionar el primer dispositivo
var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
if (device != null)
{
    // seleccionar el primer formato (puede no ser el mejor, pero es solo una muestra)
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };

        // seleccionar la primera tasa de frames
        videoSourceSettings.Format.FrameRate = formatItem.FrameRateList[0];
    }
}

// crear bloque fuente de video usando el dispositivo y formato seleccionado
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);

// crear bloque renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// conectar bloques
pipeline.Connect(videoSource.Output, videoRenderer.Input);

// iniciar pipeline
await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Captura de Video Simple (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

#### Comentarios

Puede especificar una API para usar durante la enumeración de dispositivos (consulte la descripción del enum `VideoCaptureDeviceAPI` bajo `SystemVideoSourceBlock` para valores típicos). Las plataformas Android e iOS tienen solo una API, mientras que Windows y Linux tienen múltiples APIs.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Fuente de Audio del Sistema

SystemAudioSourceBlock se usa para acceder a mics y otros dispositivos de captura de audio.

#### Información del bloque

Nombre: SystemAudioSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de audio | audio sin comprimir | 1 |

#### Enumerar dispositivos disponibles

Use la llamada al método `DeviceEnumerator.Shared.AudioSourcesAsync()` para obtener una lista de dispositivos disponibles y sus especificaciones.

Durante la enumeración de dispositivos, puede obtener la lista de dispositivos disponibles y sus especificaciones. Puede seleccionar el dispositivo y su formato para crear los ajustes de fuente.

#### El pipeline de muestra

```mermaid
graph LR;
    SystemAudioSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
// crear pipeline
var pipeline = new MediaBlocksPipeline();

// crear bloque fuente de audio
IAudioCaptureDeviceSourceSettings audioSourceSettings = null;

// seleccionar primer dispositivo
var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
if (device != null)
{
    // seleccionar primer formato
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        audioSourceSettings = device.CreateSourceSettings(formatItem.ToFormat());
    }    
}

// crear bloque fuente de audio usando dispositivo y formato seleccionado
var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

// crear bloque renderizador de audio  
var audioRenderer = new AudioRendererBlock();

// conectar bloques
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// iniciar pipeline
await pipeline.StartAsync();
```

#### Capturar audio desde altavoces (loopback)

Actualmente, la captura de audio loopback está soportada solo en Windows. Use la clase `LoopbackAudioCaptureDeviceSourceSettings` para crear ajustes de fuente para captura de audio loopback.

WASAPI2 se usa como la API predeterminada para captura de audio loopback. Puede especificar la API para usar durante la enumeración de dispositivos.

```csharp
// crear pipeline
var pipeline = new MediaBlocksPipeline();

// crear bloque fuente de audio
var deviceItem = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2))[0];
if (deviceItem == null)
{
    return;
}

var audioSourceSettings = new LoopbackAudioCaptureDeviceSourceSettings(deviceItem);
var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

// crear bloque renderizador de audio  
var audioRenderer = new AudioRendererBlock();

// conectar bloques
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// iniciar pipeline
await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Captura de Audio](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Audio%20Capture%20Demo)
- [Demo de Captura Simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

#### Comentarios

Puede especificar una API para usar durante la enumeración de dispositivos. Las plataformas Android e iOS tienen solo una API, mientras que Windows y Linux tienen múltiples APIs.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente Basler

El bloque fuente Basler soporta cámaras USB3 Vision y GigE de Basler.
El SDK o Runtime de Pylon debería estar instalado para usar la fuente de cámara.

#### Información del bloque

Nombre: BaslerSourceBlock.

| Dirección del pin   |      Tipo de medio      | Conteo de pines  |
|---------------------|:-----------------------:|:----------------:|
| Salida de video     |     Sin comprimir       |        1         |

#### El pipeline de muestra

```mermaid
graph LR;
    BaslerSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// obtener información de fuente Basler enumerando fuentes
var sources = await DeviceEnumerator.Shared.BaslerSourcesAsync();
var sourceInfo = sources[0];

// crear fuente Basler    
var source = new BaslerSourceBlock(new BaslerSourceSettings(sourceInfo));

// crear renderizador de video para VideoView
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// conectar
pipeline.Connect(source.Output, videoRenderer.Input);

// iniciar
await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Fuente Basler (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Basler%20Source%20Demo)

#### Plataformas

Windows, Linux.

### Bloque Fuente Spinnaker/FLIR

La fuente Spinnaker/FLIR soporta conexión a cámaras FLIR usando SDK Spinnaker.

#### Información del bloque

Nombre: SpinnakerSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | varios | uno o más |

#### El pipeline de muestra

```mermaid
graph LR;
    SpinnakerSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var sources = await DeviceEnumerator.Shared.SpinnakerSourcesAsync();
var sourceSettings = new SpinnakerSourceSettings(sources[0].Name, new VisioForge.Core.Types.Rect(0, 0, 1280, 720), new VideoFrameRate(10)); 

var source = new SpinnakerSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows

### Bloque Fuente Allied Vision

El Bloque Fuente Allied Vision permite integración con cámaras Allied Vision usando el SDK Vimba. Permite capturar streams de video desde estas cámaras industriales.

#### Información del bloque

Nombre: AlliedVisionSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de video   | Video sin comprimir   | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    AlliedVisionSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Enumerar cámaras Allied Vision
var alliedVisionCameras = await DeviceEnumerator.Shared.AlliedVisionSourcesAsync();
if (alliedVisionCameras.Count == 0)
{
    Console.WriteLine("No se encontraron cámaras Allied Vision.");
    return;
}

var cameraInfo = alliedVisionCameras[0]; // Seleccionar primera cámara

// Crear ajustes de fuente Allied Vision
// Ancho, alto, x, y son opcionales y dependen de si quiere establecer una ROI específica
// Si null, podría usar resolución predeterminada/completa del sensor. Camera.ReadInfo() debería llamarse.
cameraInfo.ReadInfo(); // Asegurar que información de cámara como Width/Height esté leída

var alliedVisionSettings = new AlliedVisionSourceSettings(
    cameraInfo,
    width: cameraInfo.Width, // O un ancho ROI específico
    height: cameraInfo.Height // O una altura ROI específica
);

// Configurar ajustes adicionales opcionalmente
alliedVisionSettings.ExposureAuto = VmbSrcExposureAutoModes.Continuous;
alliedVisionSettings.Gain = 10; // Valor de ganancia de ejemplo

var alliedVisionSource = new AlliedVisionSourceBlock(alliedVisionSettings);

// Crear renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1 es su control de visualización

// Conectar bloques
pipeline.Connect(alliedVisionSource.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux.

### Bloque Fuente Blackmagic Decklink

Para información sobre fuentes Decklink, vea [Decklink](../Decklink/index.md).

## Bloques Fuente de Archivo

### Bloque Fuente Universal

Una fuente universal que decodifica archivos de video y audio/network streams y proporciona datos sin comprimir a los bloques conectados.

El bloque soporta MP4, WebM, AVI, TS, MKV, MP3, AAC, M4A y muchos otros formatos. Si el redist FFMPEG está disponible, todos los decodificadores disponibles en FFMPEG también serán soportados.

#### Ajustes

El `UniversalSourceBlock` está configurado a través de `UniversalSourceSettings`. Se recomienda crear ajustes usando el método factory estático `await UniversalSourceSettings.CreateAsync(...)`.

Propiedades clave y parámetros para `UniversalSourceSettings`:

- **URI/Nombre de archivo**:
  - `UniversalSourceSettings.CreateAsync(string filename, bool renderVideo = true, bool renderAudio = true, bool renderSubtitle = false)`: Crea ajustes desde una ruta de archivo local.
  - `UniversalSourceSettings.CreateAsync(System.Uri uri, bool renderVideo = true, bool renderAudio = true, bool renderSubtitle = false)`: Crea ajustes desde un `System.Uri` (puede ser un URI de archivo o URI de red como HTTP, RTSP - aunque bloques dedicados son a menudo preferidos para streams de red). Para iOS, se usa un `Foundation.NSUrl`.
  - Los booleanos `renderVideo`, `renderAudio`, `renderSubtitle` control qué streams se procesan. El método `CreateAsync` puede actualizar estos basados en disponibilidad de stream real en el archivo/stream si `ignoreMediaInfoReader` es `false` (predeterminado).
- `StartPosition` (`TimeSpan?`): Establece la posición inicial para reproducción.
- `StopPosition` (`TimeSpan?`): Establece la posición de parada para reproducción.
- `VideoCustomFrameRate` (`VideoFrameRate?`): Si se establece, los frames de video serán descartados o duplicados para coincidir con esta tasa de frames personalizada.
- `UseAdvancedEngine` (bool): Si `true` (predeterminado, excepto Android donde es `false`), usa un motor avanzado con soporte de selección de stream.
- `DisableHWDecoders` (bool): Si `true` (predeterminado `false`, excepto Android donde es `true`), los decodificadores acelerados por hardware serán deshabilitados, forzando decodificación por software.
- `MPEGTSProgramNumber` (int): Para streams MPEG-TS, especifica el número de programa a seleccionar (predeterminado -1, significando automático o primer programa).
- `ReadInfoAsync()`: Lee asincrónicamente información de archivo de medios (`MediaFileInfo`). Esto se llama internamente por `CreateAsync` a menos que `ignoreMediaInfoReader` sea true.
- `GetInfo()`: Obtiene la `MediaFileInfo` en caché.

El `UniversalSourceBlock` mismo se instancia entonces con estos ajustes: `new UniversalSourceBlock(settings)`.
La propiedad `Filename` en la instancia `UniversalSourceBlock` (como visto en ejemplos más antiguos) es un atajo que crea ajustes básicos internamente. Usar `UniversalSourceSettings.CreateAsync` proporciona más control.

#### Información del bloque

Nombre: UniversalSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de audio | depende del decodificador | uno o más |
| Salida de video | depende del decodificador | uno o más |
| Salida de subtítulo | depende del decodificador | uno o más |

#### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->VideoRendererBlock;
    UniversalSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock();
fileSource.Filename = "test.mp4";

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);            

await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Reproductor Simple (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Player%20Demo%20WPF)

#### Plataformas

Windows, macOS, Linux, iOS, Android.


### Bloque Fuente de Subtítulo

El Bloque Fuente de Subtítulo carga subtítulos desde un archivo y los emite como un stream de subtítulos, que luego puede sobreponerse en video o renderizarse por separado.

#### Información del bloque

Nombre: `SubtitleSourceBlock`.

| Dirección del pin   | Tipo de medio         | Conteo de pines  |
|---------------------|:----------------------:|:----------------:|
| Salida de subtítulo | Datos de subtítulo    | 1                |

#### Ajustes

El `SubtitleSourceBlock` está configurado usando `SubtitleSourceSettings`. Propiedades clave incluyen:

- `Filename` (string): La ruta al archivo de subtítulos (ej. .srt, .ass).

#### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock --> SubtitleOverlayBlock;
    SubtitleSourceBlock --> SubtitleOverlayBlock;
    SubtitleOverlayBlock --> VideoRendererBlock;
    UniversalSourceBlock --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear ajustes de fuente de subtítulo
var subtitleSettings = new SubtitleSourceSettings("path/to/your/subtitles.srt");
var subtitleSource = new SubtitleSourceBlock(subtitleSettings);

// Ejemplo: Sobreponiendo subtítulos en un video desde UniversalSourceBlock
var fileSource = await UniversalSourceSettings.CreateAsync("path/to/your/video.mp4");
var universalSource = new UniversalSourceBlock(fileSource);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

// Esto es conceptual sobreposición. La implementación real podría necesitar un bloque de sobreposición de subtítulo específico.
// Para simplicidad, asumamos un bloque downstream puede consumir un stream de subtítulo,
// o conecte a un bloque que renderiza subtítulos en el video.
// Ejemplo con un hipotético SubtitleOverlayBlock:
// var subtitleOverlay = new SubtitleOverlayBlock(); // Asumiendo tal bloque existe
// pipeline.Connect(universalSource.VideoOutput, subtitleOverlay.VideoInput);
// pipeline.Connect(subtitleSource.Output, subtitleOverlay.SubtitleInput);
// pipeline.Connect(subtitleOverlay.Output, videoRenderer.Input);
// pipeline.Connect(universalSource.AudioOutput, audioRenderer.Input);

// Para un reproductor simple sin sobreposición explícita mostrada aquí:
pipeline.Connect(universalSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(universalSource.AudioOutput, audioRenderer.Input);
// Cómo se usan los subtítulos desde subtitleSource.Output dependería del diseño del resto del pipeline.
// Este bloque principalmente proporciona el stream de subtítulos.

Console.WriteLine("Fuente de subtítulos creada. Conecte su salida a un bloque compatible como sobreposición de subtítulos o renderizador.");

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente de Stream

El Bloque Fuente de Stream permite leer datos de medios desde un `System.IO.Stream`. Esto es útil para reproducir medios desde memoria, recursos embebidos o proveedores de stream personalizados sin necesidad de un archivo temporal. El formato de los datos dentro del stream debe ser parsable por el framework de medios subyacente (GStreamer).

#### Información del bloque

Nombre: `StreamSourceBlock`.
(La información de pin es dinámica, similar a `UniversalSourceBlock`, basada en contenido del stream. Típicamente tendría una salida que conecta a un demuxer/decodificador como `DecodeBinBlock`, o proporcionar pines de audio/video decodificados si incluye demuxing/decoding.)

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de datos | Varios (stream raw) | 1 |
| Salida de video | Depende del stream | 0 o 1 |
| Salida de audio | Depende del stream | 0 o 1+ |

#### Ajustes

El `StreamSourceBlock` se instancia típicamente directamente con un `System.IO.Stream`. La clase `StreamSourceSettings` sirve como contenedor para proporcionar este stream.

- `Stream` (`System.IO.Stream`): El stream de entrada conteniendo los datos de medios. El stream debe ser legible y, si se requiere búsqueda por el pipeline, buscable.

#### El pipeline de muestra

Si `StreamSourceBlock` emite datos raw que necesitan decodificación:

```mermaid
graph LR;
    StreamSourceBlock -- Datos de Stream --> DecodeBinBlock;
    DecodeBinBlock -- Salida de Video --> VideoRendererBlock;
    DecodeBinBlock -- Salida de Audio --> AudioRendererBlock;
```

Si `StreamSourceBlock` maneja decodificación internamente (menos común para una fuente de stream genérica):

```mermaid
graph LR;
    StreamSourceBlock -- Salida de Video --> VideoRendererBlock;
    StreamSourceBlock -- Salida de Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Ejemplo: Cargar un archivo de video en un MemoryStream
byte[] fileBytes = File.ReadAllBytes("path/to/your/video.mp4");
var memoryStream = new MemoryStream(fileBytes);

// StreamSourceSettings es un contenedor para el stream.
var streamSettings = new StreamSourceSettings(memoryStream);
// El método CreateBlock de StreamSourceSettings devolvería típicamente new StreamSourceBlock(streamSettings.Stream)
var streamSource = streamSettings.CreateBlock() as StreamSourceBlock; 
// O, más directamente: var streamSource = new StreamSourceBlock(memoryStream);

// Crear renderizadores de video y audio
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1
var audioRenderer = new AudioRendererBlock();

// Conectar salidas. Comúnmente, un StreamSourceBlock proporciona datos raw a un DecodeBinBlock.
var decodeBin = new DecodeBinBlock();
pipeline.Connect(streamSource.Output, decodeBin.Input); // Asumiendo un pin 'Output' único en StreamSourceBlock
pipeline.Connect(decodeBin.VideoOutput, videoRenderer.Input);
pipeline.Connect(decodeBin.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();

// Importante: Asegurar que el stream permanezca abierto y válido por la duración de la reproducción.
// Desechar el stream cuando el pipeline esté detenido o desechado.
// Considerar esto en relación con pipeline.DisposeAsync() o limpieza similar.
// memoryStream.Dispose(); // Típicamente después de pipeline.StopAsync() y pipeline.DisposeAsync()
```

#### Comentarios

El `StreamSourceBlock` mismo intentará leer desde el stream proporcionado. El éxito de la reproducción depende del formato de los datos en el stream y la disponibilidad de demuxers y decodificadores apropiados en las partes subsiguientes del pipeline (a menudo manejados vía `DecodeBinBlock`).

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Stream Source Block With Decoder

El `StreamSourceBlockWithDecoder` combina la lectura de stream y la decodificación automática en un único bloque. Acepta un `System.IO.Stream` o una ruta de archivo, y demultiplexa y decodifica el contenido automáticamente — sin necesidad de un `DecodeBinBlock` adicional.

#### Información del bloque

Nombre: StreamSourceBlockWithDecoder.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video sin comprimir | 0 o 1 |
| Salida de audio | Audio sin comprimir | 0 o 1 |

#### Constructores

```csharp
// Desde un stream de .NET
StreamSourceBlockWithDecoder(Stream stream)

// Desde una ruta de archivo
StreamSourceBlockWithDecoder(string filename)
```

El bloque conecta internamente un `StreamSourceBlock` a un `DecodeBinBlock`. Los pines `VideoOutput` y `AudioOutput` son `null` si la fuente no contiene el tipo de medio correspondiente.

#### El pipeline de muestra

```mermaid
graph LR;
    StreamSourceBlockWithDecoder -- Video --> VideoRendererBlock;
    StreamSourceBlockWithDecoder -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Desde una ruta de archivo
var source = new StreamSourceBlockWithDecoder("C:/media/video.mp4");

// O desde un stream:
// var stream = File.OpenRead("C:/media/video.mp4");
// var source = new StreamSourceBlockWithDecoder(stream);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Image Sequence Source Block

El `ImageSequenceSourceBlock` genera un flujo de video a partir de una secuencia de imágenes estáticas almacenadas en una carpeta. Admite tasa de fotogramas configurable y modos de transmisión en vivo o con capacidad de búsqueda.

#### Información del bloque

Nombre: ImageSequenceSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque acepta una instancia de `ImageSequenceSourceSettings`:

Constructor: `ImageSequenceSourceSettings(string folderPath, string filePattern = null)`

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `FolderPath` | `string` | requerido | Carpeta que contiene los archivos de imagen |
| `FilePattern` | `string` | auto-detectado | Patrón de nombre de archivo (p. ej., `image_%05d.jpg`); auto-detectado si es `null` |
| `StartIndex` | `int` | auto-detectado | Índice inicial de la secuencia de imágenes; auto-detectado del nombre del primer archivo |
| `FrameRate` | `VideoFrameRate` | `FPS_25` | Tasa de fotogramas de salida |
| `IsLive` | `bool` | `false` | Tratar la fuente como en vivo (no buscable) |
| `NumBuffers` | `int` | `-1` | Máximo de fotogramas a emitir (−1 = ilimitado) |

Formatos de imagen admitidos: `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.tiff`, `.tif`.

#### El pipeline de muestra

```mermaid
graph LR;
    ImageSequenceSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageSequenceSourceSettings("C:/images/sequence/")
{
    FrameRate = VideoFrameRate.FPS_25
};

var imageSource = new ImageSequenceSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(imageSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

`ImageSequenceSourceBlock.IsAvailable(settings)` devuelve `true` si el elemento GStreamer de secuencia de imágenes está disponible.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Animated GIF Source Block

El `AnimatedGIFSourceBlock` genera un flujo de video a partir de un archivo GIF animado. Admite tasa de fotogramas configurable, bucle y modos de transmisión en vivo o con capacidad de búsqueda.

#### Información del bloque

Nombre: AnimatedGIFSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque acepta una instancia de `ImageVideoSourceSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Filename` | `string` | requerido | Ruta al archivo GIF animado |
| `IsLive` | `bool` | `false` | Tratar la fuente como en vivo (no buscable) |
| `NumBuffers` | `int` | `-1` | Máximo de fotogramas a emitir (−1 = ilimitado) |
| `AnimationInfiniteLoop` | `bool` | `true` | Reproducir la animación en bucle indefinidamente |
| `FrameRate` | `VideoFrameRate` | `FPS_10` | Tasa de fotogramas de salida |

#### El pipeline de muestra

```mermaid
graph LR;
    AnimatedGIFSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageVideoSourceSettings("C:/media/animation.gif")
{
    AnimationInfiniteLoop = true,
    FrameRate = VideoFrameRate.FPS_25
};

var gifSource = new AnimatedGIFSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(gifSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

`AnimatedGIFSourceBlock.IsAvailable()` devuelve `true` si el elemento GStreamer de GIF animado está disponible.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente CDG

El Bloque Fuente CDG está diseñado para reproducir archivos CD+G (Compact Disc + Graphics), comúnmente usados para karaoke. Decodifica tanto el track de audio como el stream de gráficos de baja resolución.

#### Información del bloque

Nombre: CDGSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de audio   | Audio sin comprimir   | 1               |
| Salida de video   | Video sin comprimir   | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    CDGSourceBlock -- Audio --> AudioRendererBlock;
    CDGSourceBlock -- Video --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear ajustes de fuente CDG
var cdgSettings = new CDGSourceSettings(
    "path/to/your/file.cdg",  // Ruta al archivo de gráficos CDG
    "path/to/your/file.mp3"   // Ruta al archivo de audio correspondiente (MP3, WAV, etc.)
);
// Si audioFilename es null o vacío, audio será ignorado.

var cdgSource = new CDGSourceBlock(cdgSettings);

// Crear renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1 es su control de visualización
pipeline.Connect(cdgSource.VideoOutput, videoRenderer.Input);

// Crear renderizador de audio (si audio va a ser reproducido)
if (!string.IsNullOrEmpty(cdgSettings.AudioFilename) && cdgSource.AudioOutput != null)
{
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(cdgSource.AudioOutput, audioRenderer.Input);
}

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Comentarios

Requiere tanto un archivo `.cdg` para gráficos como un archivo de audio separado (ej. MP3, WAV) para la música.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

## Bloques Fuente de Red

### Bloque Fuente VNC

El Bloque Fuente VNC permite capturar video desde un servidor VNC (Virtual Network Computing) o RFB (Remote Framebuffer). Esto es útil para streaming del escritorio de una máquina remota.

#### Información del bloque

Nombre: `VNCSourceBlock`.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de video   | Video sin comprimir   | 1               |

#### Ajustes

El `VNCSourceBlock` está configurado usando `VNCSourceSettings`. Propiedades clave incluyen:

- `Host` (string): El hostname o dirección IP del servidor VNC.
- `Port` (int): El número de puerto del servidor VNC.
- `Password` (string): La contraseña para autenticación de servidor VNC, si requerida.
- `Uri` (string): Alternativamente, un URI RFB completo (ej. "rfb://host:port").
- `Width` (int): Ancho de salida deseado. El bloque puede conectarse a un servidor VNC que proporciona dimensiones específicas.
- `Height` (int): Altura de salida deseada.
- `Shared` (bool): Si compartir el escritorio con otros clientes (predeterminado `true`).
- `ViewOnly` (bool): Si `true`, no se enviará entrada (mouse/teclado) al servidor VNC (predeterminado `false`).
- `Incremental` (bool): Si usar actualizaciones incrementales (predeterminado `true`).
- `UseCopyrect` (bool): Si usar codificación copyrect (predeterminado `false`).

#### El pipeline de muestra

```mermaid
graph LR;
    VNCSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar ajustes de fuente VNC
var vncSettings = new VNCSourceSettings
{
    Host = "your-vnc-server-ip", // o usar Uri
    Port = 5900, // Puerto VNC estándar
    Password = "your-password", // si hay alguna
    // Width = 1920, // Opcional: ancho deseado
    // Height = 1080, // Opcional: altura deseada
};

var vncSource = new VNCSourceBlock(vncSettings);

// Crear renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1 es su control de visualización

// Conectar bloques
pipeline.Connect(vncSource.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux (Depende de disponibilidad del plugin VNC de GStreamer subyacente).

### Bloque Fuente RTSP

La fuente RTSP soporta conexión a cámaras IP y otros dispositivos que soportan el protocolo RTSP.

Códecs de video soportados: H264, HEVC, MJPEG.
Códecs de audio soportados: AAC, MP3, PCM, G726, G711 y algunos otros si el redist FFMPEG está instalado.

#### Información del bloque

Nombre: RTSPSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de audio | depende del decodificador | uno o más |
| Salida de video | depende del decodificador | uno o más |
| Salida de subtítulo | depende del decodificador | uno o más |

#### Ajustes

El `RTSPSourceBlock` está configurado usando `RTSPSourceSettings`. Propiedades clave incluyen:

- `Uri`: La URL RTSP del stream.
- `Login`: Nombre de usuario para autenticación RTSP, si requerida.
- `Password`: Contraseña para autenticación RTSP, si requerida.
- `AudioEnabled`: Un booleano indicando si intentar procesar el stream de audio.
- `Latency`: Especifica la duración de buffering para el stream entrante (predeterminado 1000ms).
- `AllowedProtocols`: Define los protocolos de transporte a usar para recibir el stream. Es un enum de flags `RTSPSourceProtocol` con valores:
  - `UDP`: Stream de datos sobre UDP.
  - `UDP_Multicast`: Stream de datos sobre UDP multicast.
  - `TCP` (Recomendado): Stream de datos sobre TCP.
  - `HTTP`: Stream de datos tunneled sobre HTTP.
  - `EnableTLS`: Encriptar TCP y HTTP con TLS (usar `rtsps://` o `httpsps://` en URI).
- `DoRTCP`: Habilita RTCP (RTP Control Protocol) para estadísticas de stream y control (generalmente true por defecto).
- `RTPBlockSize`: Especifica el tamaño de bloques RTP.
- `UDPBufferSize`: Tamaño de buffer para transporte UDP.
- `CustomVideoDecoder`: Permite especificar un nombre de elemento decodificador de video GStreamer personalizado si el predeterminado no es adecuado.
- `UseGPUDecoder`: Si se establece en `true`, el SDK intentará usar un decodificador GPU acelerado por hardware si disponible.
- `CompatibilityMode`: Si `true`, el SDK no intentará leer información de cámara antes de intentar reproducir, lo cual puede ser útil para streams problemáticos.
- `EnableRAWVideoAudioEvents`: Si `true`, habilita eventos para datos de muestra de video y audio raw (sin decodificar).

Es recomendado inicializar `RTSPSourceSettings` usando el método factory estático `RTSPSourceSettings.CreateAsync(Uri uri, string login, string password, bool audioEnabled, bool readInfo = true)`. Esto también puede manejar descubrimiento ONVIF si el URI apunta a un servicio de dispositivo ONVIF. Establecer `readInfo` en `false` habilita `CompatibilityMode`.

#### El pipeline de muestra

```mermaid
graph LR;
    RTSPSourceBlock-->VideoRendererBlock;
    RTSPSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Es recomendado usar CreateAsync para inicializar ajustes
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://login:pwd@192.168.1.64:554/Streaming/Channels/101?transportmode=unicast&profile=Profile_1"),
    "login", 
    "pwd",
    audioEnabled: true);

// Opcionalmente configurar más ajustes
// rtspSettings.Latency = TimeSpan.FromMilliseconds(500);
// rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP; // Preferir TCP

var rtspSource = new RTSPSourceBlock(rtspSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);      

await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Vista Previa RTSP](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo)
- [Demo RTSP MultiViewSync](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20MultiViewSync%20Demo)

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente RTSP RAW

El `RTSPRAWSourceBlock` se conecta a un stream RTSP y entrega los datos de video y audio **codificados** (sin decodificar) directamente. A diferencia de `RTSPSourceBlock`, no se crea ningún pipeline de decodificación, lo que resulta en menor uso de CPU. Use este bloque cuando necesite grabar, retransmitir o procesar el bitstream comprimido sin procesar.

#### Información del bloque

Nombre: RTSPRAWSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video codificado (p. ej. H.264) | 1 |
| Salida de audio | Audio codificado (p. ej. AAC) | 1 |

#### Configuración

Use el método de fábrica `RTSPRAWSourceSettings.CreateAsync(uri, login, password, audioEnabled)` para crear la configuración. Propiedades principales:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Uri` | `Uri` | — | URL del stream RTSP |
| `AllowedProtocols` | `RTSPSourceProtocol` | Todos | Protocolos de transporte (TCP, UDP, UDP_Multicast, HTTP) |
| `Latency` | `int` | — | Almacenamiento en búfer en milisegundos |
| `Login` | `string` | — | Nombre de usuario de autenticación |
| `Password` | `string` | — | Contraseña de autenticación |
| `AudioEnabled` | `bool` | `true` | Habilitar pad de salida de audio |
| `WaitForKeyframe` | `bool` | `true` | Esperar fotograma IDR antes de reenviar video |
| `SyncAudioWithKeyframe` | `bool` | `true` | Sincronizar entrega de audio al primer fotograma clave |
| `DoRTCP` | `bool` | — | Habilitar protocolo de control RTCP |
| `UDPBufferSize` | `int` | — | Tamaño del búfer de recepción UDP |

#### El pipeline de muestra

```mermaid
graph LR;
    RTSPRAWSourceBlock-->MP4SinkBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.64:554/stream"),
    login: "admin",
    password: "password",
    audioEnabled: true);

var rtspRawSource = new RTSPRAWSourceBlock(settings);

// VideoOutput lleva video codificado (p. ej. H.264)
// AudioOutput lleva audio codificado (p. ej. AAC)
var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(rtspRawSource.VideoOutput, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(rtspRawSource.AudioOutput, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

#### Disponibilidad

`RTSPRAWSourceBlock.IsAvailable()` devuelve `true` si los elementos GStreamer RTSP necesarios están disponibles.

#### Plataformas

Windows, macOS, Linux.

### Bloque Fuente RTMP

El bloque fuente RTMP se conecta a streams RTMP publicados por servidores multimedia (Nginx-RTMP, Wowza, OBS, FFmpeg, etc.). Emite pads de video y audio comprimidos para uso con bloques de decodificación o grabación.

Códecs de video admitidos: H.264, H.265, VP8.
Códecs de audio admitidos: AAC, MP3.

#### Información del bloque

Nombre: RTMPSourceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Salida de video | Video comprimido | 1 |
| Salida de audio | Audio comprimido | 1 |

#### Configuración

`RTMPSourceBlock` se configura con `RTMPSourceSettings`. Use el método de fábrica estático para inicializar la configuración con información del stream leída desde el servidor:

```csharp
var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true);
```

Establezca `readInfo: false` (que activa `CompatibilityMode`) para streams donde la lectura previa falla:

```csharp
var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true,
    readInfo: false);
```

Propiedades principales:

- `Uri`: URI del stream RTMP.
- `AudioEnabled`: Si se debe crear un pad de salida de audio (predeterminado `true`).
- `CompatibilityMode`: Omite la lectura de información del stream; útil para streams problemáticos.
- `UseGPUDecoder`: Intenta usar decodificación acelerada por hardware.
- `ExtraConnectArgs`: Parámetros de conexión RTMP adicionales.

#### El pipeline de muestra

```mermaid
graph LR;
    RTMPSourceBlock-->VideoRendererBlock;
    RTMPSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true);

var rtmpSource = new RTMPSourceBlock(rtmpSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtmpSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtmpSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux.

### Bloque Fuente HTTP

El bloque fuente HTTP permite recuperar datos usando protocolos HTTP/HTTPS.
Puede usarse para leer datos desde cámaras MJPEG IP, archivos MP4 de red, u otras fuentes.

#### Información del bloque

Nombre: HTTPSourceBlock.

| Dirección del pin |  Tipo de medio  | Conteo de pines  |
|-------------------|:---------------:|:----------------:|
| Salida            |     Datos       |        1         |

#### El pipeline de muestra

El pipeline de muestra lee datos desde una cámara MJPEG y los muestra usando VideoView.

```mermaid
graph LR;
    HTTPSourceBlock-->JPEGDecoderBlock;
    JPEGDecoderBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new HTTPSourceSettings(new Uri("http://mjpegcamera:8080"))
{
    UserID = "username",
    UserPassword = "password"
};

var source = new HTTPSourceBlock(settings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var jpegDecoder = new JPEGDecoderBlock();

pipeline.Connect(source.Output, jpegDecoder.Input);
pipeline.Connect(jpegDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux.


### Bloque Fuente HTTP MJPEG

El Bloque Fuente HTTP MJPEG está específicamente diseñado para conectar y decodificar streams de video MJPEG (Motion JPEG) sobre HTTP/HTTPS. Esto es común para muchas cámaras IP.

#### Información del bloque

Nombre: HTTPMJPEGSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de video   | Video sin comprimir   | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    HTTPMJPEGSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear ajustes para la fuente HTTP MJPEG
var mjpegSettings = await HTTPMJPEGSourceSettings.CreateAsync(
    new Uri("http://your-mjpeg-camera-url/stream"), // Reemplazar con URL de stream MJPEG de su cámara
    "username", // Opcional: nombre de usuario para autenticación de cámara
    "password"  // Opcional: contraseña para autenticación de cámara
);

if (mjpegSettings == null)
{
    Console.WriteLine("Falló al inicializar ajustes HTTP MJPEG.");
    return;
}

mjpegSettings.CustomVideoFrameRate = new VideoFrameRate(25); // Opcional: Establecer si cámara no reporta tasa de frames
mjpegSettings.Latency = TimeSpan.FromMilliseconds(200); // Opcional: Ajustar latencia

var httpMjpegSource = new HTTPMJPEGSourceBlock(mjpegSettings);

// Crear renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1 es su control de visualización

// Conectar bloques
pipeline.Connect(httpMjpegSource.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- Similar a Demo de Fuente HTTP MJPEG mencionada bajo el bloque fuente HTTP genérico.

#### Plataformas

Windows, macOS, Linux.

### Bloque Fuente NDI

El bloque fuente NDI soporta conexión a fuentes de software NDI y dispositivos que soportan el protocolo NDI.

#### Información del bloque

Nombre: NDISourceBlock.

| Dirección del pin   |      Tipo de medio      | Conteo de pines  |
|---------------------|:-----------------------:|:----------------:|
| Salida de audio     |     Sin comprimir       |        1         |
| Salida de video     |     Sin comprimir       |        1         |

#### El pipeline de muestra

```mermaid
graph LR;
    NDISourceBlock-->VideoRendererBlock;
    NDISourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// obtener información de fuente NDI enumerando fuentes
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSourceInfo = ndiSources[0];

// crear ajustes de fuente NDI  
var ndiSettings = NDISourceSettings.CreateAsync(ndiSourceInfo);

var ndiSource = new NDISourceBlock(ndiSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(ndiSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(ndiSource.AudioOutput, audioRenderer.Input);      

await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Fuente NDI](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/NDI%20Source%20Demo)

#### Plataformas

Windows, macOS, Linux.

### NDI Source X Block

El `NDISourceXBlock` es una fuente NDI extendida que captura video y audio de fuentes de red NDI. Usa la misma clase `NDISourceSettings` que `NDISourceBlock`, pero emplea el elemento GStreamer alternativo `ndisrcx`.

#### Información del bloque

Nombre: NDISourceXBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video sin comprimir | 1 |
| Salida de audio | Audio sin comprimir | 1 |

#### Configuración

El bloque acepta `NDISourceSettings`. Use el método estático asíncrono para crearlo:

```csharp
var ndiSettings = await NDISourceSettings.CreateAsync(context, ndiSourceInfo);
```

Propiedades clave de `NDISourceSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `SourceName` | `string` | `""` | Nombre de la fuente NDI |
| `URL` | `string` | `""` | URL de la fuente |
| `ReceiverName` | `string` | `"VF NDI Receiver"` | Nombre de la aplicación receptora |
| `Bandwidth` | `int` | `100` | −10 solo metadatos, 10 solo audio, 100 calidad completa |
| `ColorFormat` | `NDIRecvColorFormat` | `UyvyBgra` | Formato de píxel para el video recibido |
| `Timeout` | `TimeSpan` | 5 s | Tiempo de espera para detectar desconexión |
| `ConnectTimeout` | `TimeSpan` | 10 s | Tiempo de espera de conexión inicial |
| `TimestampMode` | `NDITimestampMode` | `Auto` | Modo de sincronización de marcas de tiempo |

#### El pipeline de muestra

```mermaid
graph LR;
    NDISourceXBlock -- Video --> VideoRendererBlock;
    NDISourceXBlock -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSourceInfo = ndiSources[0];

var ndiSettings = NDISourceSettings.CreateAsync(ndiSourceInfo);

var ndiSource = new NDISourceXBlock(ndiSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(ndiSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(ndiSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows.

### Bloque Fuente GenICam

La fuente GenICam soporta conexión a cámaras GigE y USB3 Vision que soportan el protocolo GenICam.

#### Información del bloque

Nombre: GenICamSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | varios | uno o más |

#### El pipeline de muestra

```mermaid
graph LR;
    GenICamSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var sourceSettings = new GenICamSourceSettings(cbCamera.Text, new VisioForge.Core.Types.Rect(0, 0, 512, 512), 15, GenICamPixelFormat.Mono8); 
var source = new GenICamSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Fuente GenICam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/GenICam%20Source%20Demo)

Lea más sobre la [Integración de Cámara USB3 Vision, GigE y GenICam](../../videocapture/video-sources/usb3v-gige-genicam/index.md).

#### Plataformas

Windows, macOS, Linux

### Bloque Fuente SRT (con decodificación)

El `Secure Reliable Transport (SRT)` es un protocolo de streaming de video de código abierto diseñado para entrega segura y baja latencia sobre redes impredecibles, como la internet pública. Desarrollado por Haivision, SRT optimiza el rendimiento de streaming adaptándose dinámicamente a variaciones de ancho de banda y minimizando los efectos de pérdida de paquetes. Incorpora encriptación AES para transmisión segura de contenido. Principalmente usado en broadcasting y streaming en línea, SRT es crucial para entregar feeds de video de alta calidad en aplicaciones en tiempo real, mejorando experiencias de visualización incluso en condiciones de red subóptimas. Soporta streaming punto a punto y multicast, haciéndolo versátil para diversas configuraciones.

El bloque fuente SRT proporciona streams de video y audio decodificados desde una fuente SRT.

#### Información del bloque

Nombre: SRTSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Sin comprimir | 0+ |
| Salida de audio | Sin comprimir | 0+ |

#### Ajustes

El `SRTSourceBlock` está configurado usando `SRTSourceSettings`. Proporciona opciones completas para conexiones SRT:

- `Uri` (string): La URI SRT (ej. "srt://127.0.0.1:8888" o "srt://example.com:9000?mode=listener"). Predeterminado "srt://127.0.0.1:8888".
- `Mode` (`SRTConnectionMode` enum): Especifica el modo de conexión SRT. Predeterminado `Caller`. Ver detalles del enum `SRTConnectionMode` abajo.
- `Passphrase` (string): La contraseña para transmisión encriptada.
- `PbKeyLen` (`SRTKeyLength` enum): La longitud de clave de cifrado AES. Predeterminado `NoKey`. Ver detalles del enum `SRTKeyLength` abajo.
- `Latency` (`TimeSpan`): La latencia máxima aceptada de transmisión (lado receptor para caller/listener, o para ambos en rendezvous). Predeterminado 125 milisegundos.
- `StreamId` (string): El ID de stream para control de acceso SRT.
- `LocalAddress` (string): La dirección local a la que enlazar cuando en modo `Listener` o `Rendezvous`. Predeterminado `null` (cualquiera).
- `LocalPort` (uint): El puerto local al que enlazar cuando en modo `Listener` o `Rendezvous`. Predeterminado 7001.
- `Authentication` (bool): Si autenticar la conexión. Predeterminado `true`.
- `AutoReconnect` (bool): Si la fuente debe intentar reconectar si la conexión falla. Predeterminado `true`.
- `KeepListening` (bool): Si `false` (predeterminado), el elemento señalará fin-de-stream cuando el cliente remoto se desconecte (en modo listener). Si `true`, mantiene esperando reconexión.
- `PollTimeout` (`TimeSpan`): Tiempo de espera de polling usado cuando se inicia un poll SRT. Predeterminado 1000 milisegundos.
- `WaitForConnection` (bool): Si `true` (predeterminado), bloquea el stream hasta que un cliente se conecte (en modo listener).

El `SRTSourceSettings` puede inicializarse usando `await SRTSourceSettings.CreateAsync(string uri, bool ignoreMediaInfoReader = false)`. Establecer `ignoreMediaInfoReader` en `true` puede ser útil si la lectura de información de medios falla para un stream en vivo.

##### Enum `SRTConnectionMode`

Define el modo operativo para una conexión SRT:

- `None` (0): No se especifica modo de conexión (no debería usarse típicamente directamente).
- `Caller` (1): La fuente inicia la conexión a un listener.
- `Listener` (2): La fuente espera una conexión entrante desde un caller.
- `Rendezvous` (3): Ambos extremos inician conexión entre sí simultáneamente, útil para atravesar firewalls.

##### Enum `SRTKeyLength`

Define la longitud de clave para cifrado AES de SRT:

- `NoKey` (0) / `Length0` (0): No se usa cifrado.
- `Length16` (16): Clave de cifrado AES de 16 bytes (128-bit).
- `Length24` (24): Clave de cifrado AES de 24 bytes (192-bit).
- `Length32` (32): Clave de cifrado AES de 32 bytes (256-bit).

#### El pipeline de muestra

```mermaid
graph LR;
    SRTSourceBlock-->VideoRendererBlock;
    SRTSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new SRTSourceBlock(new SRTSourceSettings() { Uri = edURL.Text });
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

pipeline.Connect(source.VideoOutput, videoRenderer.Input);
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Fuente SRT](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/SRT%20Source%20Demo)

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente SRT RAW

El `Secure Reliable Transport (SRT)` es un protocolo de streaming que optimiza la entrega de datos de video sobre redes impredecibles, como Internet. Es de código abierto y diseñado para alto rendimiento y audio streaming. SRT proporciona seguridad a través de encriptación de extremo a extremo, confiabilidad recuperando paquetes perdidos, y baja latencia, adecuada para broadcasts en vivo. Se adapta a condiciones de red variables gestionando dinámicamente el ancho de banda, asegurando streams de alta calidad incluso bajo condiciones subóptimas. Usado ampliamente en broadcasting y streaming de aplicaciones, SRT soporta interoperabilidad y es ideal para producción remota y distribución de contenido.

La fuente SRT soporta conexión a fuentes SRT y proporciona un stream de datos. Puede conectar este bloque a `DecodeBinBlock` para decodificar el stream.

#### Información del bloque

Nombre: SRTRAWSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de datos | Cualquiera | uno |

#### Ajustes

El `SRTRAWSourceBlock` está configurado usando `SRTSourceSettings`. Refiérase a la descripción detallada de `SRTSourceSettings` y sus enums relacionados (`SRTConnectionMode`, `SRTKeyLength`) bajo el `Bloque Fuente SRT (con decodificación)` para todas las propiedades disponibles y sus explicaciones.

#### El pipeline de muestra

```mermaid
graph LR;
    SRTRAWSourceBlock-->DecodeBinBlock;
    DecodeBinBlock-->VideoRendererBlock;
    DecodeBinBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new SRTRAWSourceBlock(new SRTSourceSettings() { Uri = edURL.Text });
var decodeBin = new DecodeBinBlock();
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

pipeline.Connect(source.Output, decodeBin.Input);
pipeline.Connect(decodeBin.VideoOutput, videoRenderer.Input);
pipeline.Connect(decodeBin.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

## Otros Bloques Fuente

### Bloque Fuente de Pantalla

La fuente de pantalla soporta grabación de video desde la pantalla. Puede seleccionar la pantalla (si hay más de una), la parte de la pantalla a grabar, y grabación opcional del cursor del mouse.

#### Ajustes

El `ScreenSourceBlock` usa clases de ajustes específicas de plataforma. La elección de clase de ajustes determina la tecnología de captura de pantalla subyacente. El `ScreenCaptureSourceType` enum indica las tecnologías disponibles:

##### Windows

- `ScreenCaptureDX9SourceSettings` - Usar `DirectX 9` para grabación de pantalla. (`ScreenCaptureSourceType.DX9`)
- `ScreenCaptureD3D11SourceSettings` - Usar `Direct3D 11` Desktop Duplication para grabación de pantalla. Permite captura de ventana específica. (`ScreenCaptureSourceType.D3D11DesktopDuplication`)
- `ScreenCaptureGDISourceSettings` - Usar `GDI` para grabación de pantalla. (`ScreenCaptureSourceType.GDI`)

##### macOS

`ScreenCaptureMacOSSourceSettings` - Usar `AVFoundation` para grabación de pantalla. (`ScreenCaptureSourceType.AVFoundation`)

##### Linux

`ScreenCaptureXDisplaySourceSettings` - Usar `X11` (XDisplay) para grabación de pantalla. (`ScreenCaptureSourceType.XDisplay`)

##### iOS

`IOSScreenSourceSettings` - Usar `AVFoundation` para grabación de ventana/app actual. (`ScreenCaptureSourceType.IOSScreen`)

#### Información del bloque

Nombre: ScreenSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    ScreenSourceBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

#### Código de muestra

```csharp
// crear pipeline
var pipeline = new MediaBlocksPipeline();

// crear ajustes de fuente
var screenSourceSettings = new ScreenCaptureDX9SourceSettings() { FrameRate = 15 }

// crear bloque fuente
var screenSourceBlock = new ScreenSourceBlock(screenSourceSettings);

// crear bloque codificador de video y conectarlo al bloque fuente
var h264EncoderBlock = new H264EncoderBlock(new OpenH264EncoderSettings());
pipeline.Connect(screenSourceBlock.Output, h264EncoderBlock.Input);

// crear bloque sink MP4 y conectarlo al bloque codificador
var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

// ejecutar pipeline
await pipeline.StartAsync();
```

#### [Windows] Captura de ventana

Puede capturar una ventana específica usando la clase `ScreenCaptureD3D11SourceSettings`.

```csharp
// crear fuente Direct3D11
var source = new ScreenCaptureD3D11SourceSettings();

// establecer tasa de frames
source.FrameRate = new VideoFrameRate(30);

// obtener handle de la ventana
var wih = new System.Windows.Interop.WindowInteropHelper(this);
source.WindowHandle = wih.Handle;

// crear bloque fuente
var screenSourceBlock = new ScreenSourceBlock(new ScreenCaptureDX9SourceSettings() { FrameRate = 15 });

// otro código es el mismo que arriba
```

#### Aplicaciones de muestra

- [Demo de Captura de Pantalla (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Screen%20Capture)
- [Demo de Captura de Pantalla (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/ScreenCaptureMB)
- [Demo de Captura de Pantalla (iOS)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/iOS/ScreenCapture)

#### Plataformas

Windows, macOS, Linux, iOS.

### Fallback Switch Source Block

El `FallbackSwitchSourceBlock` envuelve una fuente de video principal y cambia automáticamente a contenido de respaldo cuando la fuente principal falla o deja de estar disponible, ideal para pipelines de transmisión en vivo confiables.

#### Información del bloque

Nombre: FallbackSwitchSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video sin comprimir | 1 |
| Salida de audio | Audio sin comprimir | 1 |

#### Tipos de fuente principal admitidos

El primer argumento del constructor es `IVideoSourceSettings`. Implementaciones admitidas:

- `RTSPSourceSettings` — flujo RTSP
- `SRTSourceSettings` — flujo SRT
- `NDISourceSettings` — fuente NDI
- `RTMPSourceSettings` — flujo RTMP
- `HTTPSourceSettings` — flujo HTTP

#### Configuración

**`FallbackSwitchSettings`**

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Enabled` | `bool` | `false` | Habilitar la conmutación con respaldo |
| `Fallback` | `FallbackSwitchSettingsBase` | `null` | Configuración del contenido de respaldo |
| `EnableVideo` | `bool` | `true` | Aplicar respaldo al video |
| `EnableAudio` | `bool` | `true` | Aplicar respaldo al audio |
| `MinLatencyMs` | `int` | `0` | Latencia mínima antes de conmutar |
| `FallbackVideoCaps` | `string` | `null` | Anulación de caps de video (p. ej., `"video/x-raw,width=1920,height=1080"`) |
| `FallbackAudioCaps` | `string` | `null` | Anulación de caps de audio |

**Propiedades comunes de `FallbackSwitchSettingsBase`** (compartidas por todos los tipos de respaldo):

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `TimeoutMs` | `int` | `5000` | Milisegundos antes de cambiar al respaldo |
| `RestartTimeoutMs` | `int` | `5000` | Milisegundos antes de reintentar la fuente principal |
| `RetryTimeoutMs` | `int` | `60000` | Milisegundos antes de detener los reintentos |
| `ImmediateFallback` | `bool` | `false` | Cambiar al respaldo inmediatamente ante un error |
| `RestartOnEos` | `bool` | `false` | Reiniciar la fuente principal al fin del flujo |
| `ManualUnblock` | `bool` | `false` | Requerir llamada manual a `Unblock()` para reanudar |

**Tipos de contenido de respaldo** (asigne `Fallback` a uno de):

- `StaticTextFallbackSettings` — superposición de texto con fuente, color y posición configurables
- `StaticImageFallbackSettings` — muestra una imagen estática (ruta, escala, opciones de relación de aspecto)
- `MediaBlockFallbackSettings` — reproduce un URI de respaldo o pipeline GStreamer personalizado

**Métodos de diagnóstico:**

- `GetStatus()` — devuelve el estado actual de la fuente principal/respaldo como cadena
- `GetStatistics()` — devuelve un diccionario con estadísticas de reintentos y almacenamiento en búfer
- `Unblock()` — desbloquea manualmente cuando `ManualUnblock` está habilitado

#### El pipeline de muestra

```mermaid
graph LR;
    FallbackSwitchSourceBlock -- Video --> VideoRendererBlock;
    FallbackSwitchSourceBlock -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var mainSettings = new RTSPSourceSettings(new Uri("rtsp://camera.example.com/stream"));

var fallbackSettings = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticTextFallbackSettings
    {
        Text = "Stream unavailable",
        BackgroundColor = SKColors.Black,
        TextColor = SKColors.White
    }
};

var source = new FallbackSwitchSourceBlock(mainSettings, fallbackSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente de Video Virtual

VirtualVideoSourceBlock se usa para producir datos de video de prueba en una amplia variedad de formatos de video. El tipo de datos de prueba está controlado por los ajustes.

#### Ajustes

El `VirtualVideoSourceBlock` está configurado usando `VirtualVideoSourceSettings`. Propiedades clave:

- `Pattern` (`VirtualVideoSourcePattern` enum): Especifica el tipo de patrón de prueba a generar. Ver enum `VirtualVideoSourcePattern` abajo para patrones disponibles. Predeterminado `SMPTE`.
- `Width` (int): Ancho del video de salida (predeterminado 1280).
- `Height` (int): Altura del video de salida (predeterminado 720).
- `FrameRate` (`VideoFrameRate`): Tasa de frames del video de salida (predeterminado 30 fps).
- `Format` (`VideoFormatX` enum): Formato de píxel del video (predeterminado `RGB`).
- `ForegroundColor` (`SKColor`): Para patrones que usan un color de primer plano (ej. `SolidColor`), esta propiedad define eso (predeterminado `SKColors.White`).

Constructores:

- `VirtualVideoSourceSettings()`: Constructor predeterminado.
- `VirtualVideoSourceSettings(int width, int height, VideoFrameRate frameRate)`: Inicializa con dimensiones y tasa de frames especificadas.

##### Enum `VirtualVideoSourcePattern`

Define el patrón de prueba generado por `VirtualVideoSourceBlock`:

- `SMPTE` (0): Barras de color SMPTE 100%.
- `Snow` (1): Aleatorio (nieve de televisión).
- `Black` (2): 100% Negro.
- `White` (3): 100% Blanco.
- `Red` (4), `Green` (5), `Blue` (6): Colores sólidos.
- `Checkers1` (7) a `Checkers8` (10): Patrones de ajedrez con cuadrados de 1, 2, 4 u 8 píxeles.
- `Circular` (11): Patrón circular.
- `Blink` (12): Patrón parpadeante.
- `SMPTE75` (13): Barras de color SMPTE 75%.
- `ZonePlate` (14): Placa de zona.
- `Gamut` (15): Ajedreces de gamut.
- `ChromaZonePlate` (16): Placa de zona chroma.
- `SolidColor` (17): Un color sólido, definido por `ForegroundColor`.
- `Ball` (18): Bola moviéndose.
- `SMPTE100` (19): Alias para barras de color SMPTE 100%.
- `Bar` (20): Patrón de barra.
- `Pinwheel` (21): Patrón de molinete.
- `Spokes` (22): Patrón de radios.
- `Gradient` (23): Patrón de gradiente.
- `Colors` (24): Patrón de colores varios.
- `SMPTERP219` (25): Patrón de prueba SMPTE, conforme a RP 219.

#### Información del bloque

Nombre: VirtualVideoSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    VirtualVideoSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
                      
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSourceBlock.Output, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSourceBlock.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente de Audio Virtual

VirtualAudioSourceBlock se usa para producir datos de audio de prueba en una amplia variedad de formatos de audio. El tipo de datos de prueba está controlado por los ajustes.

#### Ajustes

El `VirtualAudioSourceBlock` está configurado usando `VirtualAudioSourceSettings`. Propiedades clave:

- `Wave` (`VirtualAudioSourceSettingsWave` enum): Especifica el tipo de forma de onda de audio a generar. Ver enum `VirtualAudioSourceSettingsWave` abajo. Predeterminado `Sine`.
- `Format` (`AudioFormatX` enum): Formato de muestra de audio (predeterminado `S16LE`).
- `SampleRate` (int): Tasa de muestreo en Hz (predeterminado 48000).
- `Channels` (int): Número de canales de audio (predeterminado 2).
- `Volume` (double): Volumen de la señal de prueba (0.0 a 1.0, predeterminado 0.8).
- `Frequency` (double): Frecuencia de la señal de prueba en Hz (ej. para Sine wave, predeterminado 440).
- `IsLive` (bool): Indica si la fuente es en vivo (predeterminado `true`).
- `ApplyTickRamp` (bool): Aplicar rampa a muestras de tick (predeterminado `false`).
- `CanActivatePull` (bool): Puede activar en modo pull (predeterminado `false`).
- `CanActivatePush` (bool): Puede activar en modo push (predeterminado `true`).
- `MarkerTickPeriod` (uint): Hacer cada tick Nth un tick marcador (para wave `Ticks`, 0 = no marcador, predeterminado 0).
- `MarkerTickVolume` (double): Volumen de ticks marcador (predeterminado 1.0).
- `SamplesPerBuffer` (int): Número de muestras en cada buffer saliente (predeterminado 1024).
- `SinePeriodsPerTick` (uint): Número de períodos de onda sinusoidal en un tick (para wave `Ticks`, predeterminado 10).
- `TickInterval` (`TimeSpan`): Distancia entre inicio de tick actual y inicio del siguiente (predeterminado 1 segundo).
- `TimestampOffset` (`TimeSpan`): Un offset agregado a timestamps (predeterminado `TimeSpan.Zero`).

Constructor:

- `VirtualAudioSourceSettings(VirtualAudioSourceSettingsWave wave = VirtualAudioSourceSettingsWave.Ticks, int sampleRate = 48000, int channels = 2, AudioFormatX format = AudioFormatX.S16LE)`

##### Enum `VirtualAudioSourceSettingsWave`

Define la forma de onda para `VirtualAudioSourceBlock`:

- `Sine` (0): Onda sinusoidal.
- `Square` (1): Onda cuadrada.
- `Saw` (2): Onda diente de sierra.
- `Triangle` (3): Onda triangular.
- `Silence` (4): Silencio.
- `WhiteNoise` (5): Ruido blanco uniforme.
- `PinkNoise` (6): Ruido rosa.
- `SineTable` (7): Tabla sinusoidal.
- `Ticks` (8): Ticks periódicos.
- `GaussianNoise` (9): Ruido blanco gaussiano.
- `RedNoise` (10): Ruido rojo (Browniano).
- `BlueNoise` (11): Ruido azul.
- `VioletNoise` (12): Ruido violeta.

#### Información del bloque

Nombre: VirtualAudioSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de audio | audio sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    VirtualAudioSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
                      
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSourceBlock.Output, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSourceBlock.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente Demuxer

El Bloque Fuente Demuxer se usa para demultiplexar archivos de medios locales en sus streams elementales (video, audio, subtítulos). Permite renderizado selectivo de estos streams.

#### Información del bloque

Nombre: DemuxerSourceBlock.

| Dirección del pin   | Tipo de medio         | Conteo de pines  |
|---------------------|:----------------------:|:----------------:|
| Salida de video     | Sin comprimir         | 0 o 1            |
| Salida de audio     | Sin comprimir         | 0 o 1+           |
| Salida de subtítulo | Sin comprimir         | 0 o 1+           |

#### El pipeline de muestra

```mermaid
graph LR;
    DemuxerSourceBlock -- Stream de Video --> VideoRendererBlock;
    DemuxerSourceBlock -- Stream de Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear ajustes, asegurar de await CreateAsync
var demuxerSettings = await DemuxerSourceSettings.CreateAsync(
    "path/to/your/video.mp4", 
    renderVideo: true, 
    renderAudio: true, 
    renderSubtitle: false);

if (demuxerSettings == null)
{
    Console.WriteLine("Falló al inicializar ajustes de demuxer. Asegurar que el archivo existe y es legible.");
    return;
}

var demuxerSource = new DemuxerSourceBlock(demuxerSettings);

// Configurar renderizado de video si video está disponible y renderizado
if (demuxerSettings.RenderVideo && demuxerSource.VideoOutput != null)
{
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1 es su control de visualización
    pipeline.Connect(demuxerSource.VideoOutput, videoRenderer.Input);
}

// Configurar renderizado de audio si audio está disponible y renderizado
if (demuxerSettings.RenderAudio && demuxerSource.AudioOutput != null)
{
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(demuxerSource.AudioOutput, audioRenderer.Input);
}

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- No hay enlace de aplicación de muestra específico, pero puede usarse en escenarios tipo reproductor.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente de Imagen Video

El Bloque Fuente de Imagen Video genera un stream de video desde un archivo de imagen estático (ej. JPG, PNG). Repite la salida de la imagen como frames de video según la tasa de frames especificada.

#### Información del bloque

Nombre: ImageVideoSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de video   | Video sin comprimir   | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    ImageVideoSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear ajustes de fuente de imagen video
var imageSourceSettings = new ImageVideoSourceSettings("path/to/your/image.jpg"); // Reemplazar con su ruta de imagen
imageSourceSettings.FrameRate = new VideoFrameRate(10); // Presentar 10 frames por segundo
imageSourceSettings.IsLive = true; // Tratar como fuente en vivo (opcional)
// imageSourceSettings.NumBuffers = 100; // Opcional: salida solo 100 frames luego detener

var imageSource = new ImageVideoSourceBlock(imageSourceSettings);

// Crear renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1 es su control de visualización

// Conectar bloques
pipeline.Connect(imageSource.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Comentarios

Este bloque usa SkiaSharp para decodificación de imagen, así que asegurar dependencias necesarias si no usando paquete VisioForge estándar que lo incluye.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente de Superposición de Texto

El `TextOverlaySourceBlock` genera un stream de video que muestra texto configurable sobre un fondo sólido. Utiliza los elementos GStreamer `videotestsrc` y `textoverlay` internamente. Este bloque es útil como fuente de marcador de posición o de respaldo — por ejemplo, mostrando "Sin Señal" cuando una cámara principal no está disponible.

#### Información del bloque

Nombre: TextOverlaySourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque se configura mediante `StaticTextFallbackSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Text` | `string` | `"Source Unavailable"` | Texto a mostrar |
| `FontFamily` | `string` | `"Arial"` | Nombre de la familia de fuentes |
| `FontSize` | `float` | `24` | Tamaño de fuente en puntos |
| `FontStyle` | `FontStyle` | `Normal` | Estilo de fuente (Normal, Negrita, Cursiva, etc.) |
| `TextColor` | `SKColor` | Blanco | Color del texto |
| `BackgroundColor` | `SKColor` | Negro | Color de relleno del fondo |
| `Position` | `SKPoint` | `(0.5, 0.5)` | Posición de anclaje del texto como proporción (0–1) |
| `CenterAlign` | `bool` | `true` | Centrar el texto |

#### El pipeline de muestra

```mermaid
graph LR;
    TextOverlaySourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new StaticTextFallbackSettings
{
    Text = "Sin Señal",
    FontSize = 36,
    TextColor = SKColors.White,
    BackgroundColor = SKColors.Black
};

var textSource = new TextOverlaySourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(textSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente Mezclador de Video

El `VideoMixerSourceBlock` compone múltiples fuentes de video en un único stream de salida. Cada entrada se coloca en un rectángulo configurable sobre el lienzo de salida. Durante la reproducción, el bloque admite reposicionamiento en vivo, fundidos de opacidad y eliminación de clave de color por entrada.

El backend del mezclador se controla mediante `VideoMixerType`: CPU (predeterminado), OpenGL o Direct3D 11.

#### Información del bloque

Nombre: VideoMixerSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida de video | Video sin comprimir | 1 |

#### Configuración (`VideoMixerSourceSettings`)

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Width` | `int` | `1920` | Ancho del lienzo de salida en píxeles |
| `Height` | `int` | `1080` | Alto del lienzo de salida en píxeles |
| `FrameRate` | `VideoFrameRate` | `FPS_30` | Velocidad de fotogramas de salida |
| `MixerType` | `VideoMixerType` | `CPU` | Backend de renderizado (CPU, OpenGL, D3D11) |

Use `settings.Add(source, rect)` o `settings.Add(source, left, top, width, height)` para registrar cada fuente de entrada y su rectángulo delimitador en el lienzo.

#### Control en tiempo real

El bloque implementa `IVideoMixerControl` para actualizaciones en vivo:

- `Input_Move(id, rect, duration, startAlpha, endAlpha)` — animar posición y opacidad
- `Input_Update(stream)` — actualizar propiedades del stream
- `Input_UpdateChromaKeySettings(id, settings)` — cambiar clave de color
- `StartFadeIn(id, duration)` / `StartFadeOut(id, duration)` — controles de fundido
- `Input_List()` — enumerar todos los streams de entrada

#### El pipeline de muestra

```mermaid
graph LR;
    Source1-->VideoMixerSourceBlock;
    Source2-->VideoMixerSourceBlock;
    VideoMixerSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new VideoMixerSourceSettings(1920, 1080, VideoFrameRate.FPS_30);

// Agregar una fuente de archivo en la mitad izquierda
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("source1.mp4")));
settings.Add(fileSource, new Rect(0, 0, 960, 1080));

// Agregar una cámara web en la mitad derecha
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);
settings.Add(videoSource, new Rect(960, 0, 960, 1080));

var mixer = new VideoMixerSourceBlock(pipeline, settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixer.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

`VideoMixerSourceBlock.IsAvailable()` devuelve `true` si los elementos GStreamer de compositor necesarios están disponibles.

#### Plataformas

Windows, macOS, Linux, iOS, Android.

## Bloques Fuente Push

Los bloques fuente Push permiten alimentar datos de medios (video, audio, imágenes JPEG, o datos genéricos) directamente en el pipeline de Media Blocks desde código de aplicación. Esto es útil cuando sus medios provienen de una fuente personalizada, como un dispositivo de captura propietario, un stream de red no soportado por bloques incorporados, o contenido generado proceduralmente.

El comportamiento de fuentes push está generalmente controlado por ajustes comunes disponibles a través de la interfaz `IPushSourceSettings`, implementada por clases de ajustes de fuente push específicas:

- `IsLive` (bool): Indica si la fuente es en vivo. Predeterminados varían por tipo (ej. `true` para audio/video).
- `DoTimestamp` (bool): Si `true`, el bloque intentará generar timestamps para los datos empujados.
- `StreamType` (`PushSourceStreamType` enum: `Stream` o `SeekableStream`): Define las características del stream.
- `PushFormat` (`PushSourceFormat` enum: `Bytes`, `Time`, `Default`, `Automatic`): Controla cómo se empujan los datos (ej. basado en conteo de bytes o tiempo).
- `BlockPushData` (bool): Si `true`, la operación push bloqueará hasta que los datos sean consumidos por el pipeline.

El tipo específico de fuente push está determinado por el enum `PushSourceType`: `Video`, `Audio`, `Data`, `JPEG`.

### Bloque Fuente Push de Video

Permite empujar frames de video raw en el pipeline.

#### Información del bloque

Nombre: PushVideoSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de video   | Video sin comprimir   | 1               |

#### Ajustes

Configurado usando `PushVideoSourceSettings`:

- `Width` (int): Ancho de los frames de video.
- `Height` (int): Altura de los frames de video.
- `FrameRate` (`VideoFrameRate`): Tasa de frames del video.
- `Format` (`VideoFormatX` enum): Formato de píxel de los frames de video (ej. `RGB`, `NV12`, `I420`).
- Hereda ajustes push comunes como `IsLive` (predeterminado `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructor: `PushVideoSourceSettings(int width, int height, VideoFrameRate frameRate, VideoFormatX format = VideoFormatX.RGB)`

#### El pipeline de muestra

```mermaid
graph LR;
    PushVideoSourceBlock-->VideoEncoderBlock-->MP4SinkBlock;
    PushVideoSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar fuente push de video
var videoPushSettings = new PushVideoSourceSettings(
    width: 640, 
    height: 480, 
    frameRate: new VideoFrameRate(30), 
    format: VideoFormatX.RGB);
// videoPushSettings.IsLive = true; // Predeterminado

var videoPushSource = new PushVideoSourceBlock(videoPushSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoPushSource.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Empuje frames desde su aplicación:
// var frame = new VideoFrameX(...);
// videoPushSource.PushFrame(frame);
// videoPushSource.ClearQueue(); // Limpiar frames en búfer si es necesario
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente Push de Audio

Permite empujar muestras de audio raw en el pipeline.

#### Información del bloque

Nombre: PushAudioSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de audio   | Audio sin comprimir   | 1               |

#### Ajustes

Configurado usando `PushAudioSourceSettings`:

- `SampleRate` (int): Tasa de muestreo del audio (ej. 44100, 48000).
- `Channels` (int): Número de canales de audio (ej. 1 para mono, 2 para estéreo).
- `Format` (`AudioFormatX` enum): Formato de las muestras de audio (ej. `S16LE` para PCM signed 16-bit little-endian).
- Hereda ajustes push comunes como `IsLive` (predeterminado `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructor: `PushAudioSourceSettings(bool isLive = true, int sampleRate = 48000, int channels = 2, AudioFormatX format = AudioFormatX.S16LE)`

#### El pipeline de muestra

```mermaid
graph LR;
    PushAudioSourceBlock-->AudioEncoderBlock-->MP4SinkBlock;
    PushAudioSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar fuente push de audio
var audioPushSettings = new PushAudioSourceSettings(
    isLive: true, 
    sampleRate: 44100, 
    channels: 2, 
    format: AudioFormatX.S16LE);

var audioPushSource = new PushAudioSourceBlock(audioPushSettings);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioPushSource.Output, audioRenderer.Input);

await pipeline.StartAsync();

// Empuje datos de audio desde su aplicación:
// var frame = new AudioFrame(...);
// audioPushSource.PushData(frame);
// O bytes raw: audioPushSource.PushData(audioData, audioData.Length);
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente Push de Datos

Permite empujar datos de bytes genéricos en el pipeline. La interpretación de estos datos depende de las `Caps` (capacidades) especificadas.

#### Información del bloque

Nombre: `PushSourceBlock` (configurado para datos).

| Dirección del pin | Tipo de medio | Conteo de pines |
|-------------------|:-------------:|:---------------:|
| Salida de datos   | Personalizado  | 1               |

#### Ajustes

Configurado usando `PushDataSourceSettings`:

- `Caps` (`Gst.Caps`): String de capacidades GStreamer describiendo el formato de datos (ej. "video/x-h264, stream-format=byte-stream"). Esto es crucial para que bloques downstream entiendan los datos.
- `PadMediaType` (`MediaBlockPadMediaType` enum): Especifica el tipo del pad de salida (ej. `Video`, `Audio`, `Data`, `Auto`).
- Hereda ajustes push comunes como `IsLive`, `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

#### El pipeline de muestra

```mermaid
graph LR;
    PushDataSourceBlock-->ParserOrDecoder-->Renderer;
```

Ejemplo: Empujando stream byte Annex B H.264

```mermaid
graph LR;
    PushDataSourceBlock-->H264ParserBlock-->H264DecoderBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar fuente push de datos para stream byte H.264
var dataPushSettings = new PushDataSourceSettings();
dataPushSettings.Caps = new Gst.Caps("video/x-h264, stream-format=byte-stream");
dataPushSettings.PadMediaType = MediaBlockPadMediaType.Video;
// dataPushSettings.IsLive = true; // Establecer si en vivo

var dataPushSource = new PushSourceBlock(dataPushSettings);

// Ejemplo: Decodificar y renderizar stream H.264
var h264Parser = new H264ParserBlock();
var h264Decoder = new H264DecoderBlock(); // O OpenH264DecoderBlock, etc.
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(dataPushSource.Output, h264Parser.Input);
pipeline.Connect(h264Parser.Output, h264Decoder.Input);
pipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();

// En un hilo o tarea separada, empujar datos NALU H.264:
// byte[] naluData = ... ; // Sus datos NALU H.264
// dataPushSource.PushFrame(naluData);
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

### Bloque Fuente Push JPEG

Permite empujar imágenes JPEG individuales, que luego se emiten como un stream de video.

#### Información del bloque

Nombre: `PushSourceBlock` (configurado para JPEG).

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de video   | Video sin comprimir   | 1               |

#### Ajustes

Configurado usando `PushJPEGSourceSettings`:

- `Width` (int): Ancho de las imágenes JPEG decodificadas.
- `Height` (int): Altura de las imágenes JPEG decodificadas.
- `FrameRate` (`VideoFrameRate`): La tasa de frames a la que las imágenes JPEG serán presentadas como un stream de video.
- Hereda ajustes push comunes como `IsLive` (predeterminado `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructor: `PushJPEGSourceSettings(int width, int height, VideoFrameRate frameRate)`

#### El pipeline de muestra

```mermaid
graph LR;
    PushJPEGSourceBlock-->VideoRendererBlock;
    PushJPEGSourceBlock-->VideoEncoderBlock-->MP4SinkBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar fuente push JPEG
var jpegPushSettings = new PushJPEGSourceSettings(
    width: 1280, 
    height: 720, 
    frameRate: new VideoFrameRate(10)); // Presentar JPEGs como video de 10 FPS

var jpegPushSource = new PushSourceBlock(jpegPushSettings);

// Ejemplo: Renderizar el stream de video desde JPEGs
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(jpegPushSource.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();

// En un hilo o tarea separada, empujar datos de imagen JPEG:
// byte[] jpegImageData = File.ReadAllBytes("image.jpg");
// jpegPushSource.PushFrame(jpegImageData); 
// Llamar PushFrame para cada nueva imagen JPEG.
```

#### Plataformas

Windows, macOS, Linux, iOS, Android.

## Bloques Fuente de Plataforma Apple

### Bloque Fuente de Video iOS

IOSVideoSourceBlock proporciona captura de video desde la cámara del dispositivo en plataformas iOS. Está disponible solo en iOS (no en macOS Catalyst).

#### Información del bloque

Nombre: IOSVideoSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de video   | Video sin comprimir   | 1               |

#### Enumerar dispositivos disponibles

Use `DeviceEnumerator.Shared.VideoSourcesAsync()` para obtener una lista de dispositivos de video disponibles en iOS. Cada dispositivo está representado por un objeto `VideoCaptureDeviceInfo`.

#### El pipeline de muestra

```mermaid
graph LR;
    IOSVideoSourceBlock-->VideoRendererBlock;
```

#### Código de muestra

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

// crear bloque fuente de video iOS
var videoSource = new IOSVideoSourceBlock(videoSourceSettings);

// crear bloque renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// conectar bloques
pipeline.Connect(videoSource.Output, videoRenderer.Input);

// iniciar pipeline
await pipeline.StartAsync();
```

#### Plataformas

iOS (no disponible en macOS Catalyst)

---

### Bloque Fuente de Audio macOS

OSXAudioSourceBlock proporciona captura de audio desde dispositivos de entrada en plataformas macOS.

#### Información del bloque

Nombre: OSXAudioSourceBlock.

| Dirección del pin | Tipo de medio         | Conteo de pines |
|-------------------|:----------------------:|:---------------:|
| Salida de audio   | Audio sin comprimir   | 1               |

#### Enumerar dispositivos disponibles

Use `DeviceEnumerator.Shared.AudioSourcesAsync()` para obtener una lista de dispositivos de audio disponibles en macOS. Cada dispositivo está representado por un objeto `AudioCaptureDeviceInfo`.

#### El pipeline de muestra

```mermaid
graph LR;
    OSXAudioSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

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

// crear bloque fuente de audio macOS
var audioSource = new OSXAudioSourceBlock(audioSourceSettings);

// crear bloque renderizador de audio
var audioRenderer = new AudioRendererBlock();

// conectar bloques
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// iniciar pipeline
await pipeline.StartAsync();
```

#### Plataformas

macOS (no disponible en iOS)

---

### Blu-Ray Source Block

El `BluRaySourceBlock` lee flujos de video, audio y subtítulos opcionales desde un disco Blu-Ray o una carpeta de imagen de disco Blu-Ray usando el elemento GStreamer `bluray`.

#### Información del bloque

Nombre: BluRaySourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Video sin comprimir | 0 o 1 |
| Salida audio | Audio sin comprimir | 0 o 1 |
| Salida subtítulo | Subtítulo | 0 o 1 |

#### Configuración

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Location` | `string` | requerido | Ruta al dispositivo Blu-Ray o carpeta de imagen de disco |
| `Title` | `int` | `-1` | Título a reproducir (−1 = título principal) |
| `RenderVideo` | `bool` | `true` | Habilitar salida de video |
| `RenderAudio` | `bool` | `true` | Habilitar salida de audio |
| `RenderSubtitle` | `bool` | `false` | Habilitar salida de subtítulos |
| `AudioStream` | `int` | `-1` | Índice del flujo de audio (−1 = predeterminado) |
| `SubtitleStream` | `int` | `-1` | Índice del flujo de subtítulos (−1 = predeterminado) |

#### El pipeline de muestra

```mermaid
graph LR;
    BluRaySourceBlock -- Video --> VideoRendererBlock;
    BluRaySourceBlock -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new BluRaySourceSettings("/dev/sr0")
{
    Title = -1,
    RenderVideo = true,
    RenderAudio = true
};

var source = new BluRaySourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = BluRaySourceBlock.IsAvailable();
```

#### Plataformas

Windows, macOS, Linux.

---

### DVB Source Block

El `DVBSourceBlock` captura un flujo de televisión digital en vivo desde un adaptador DVB (DVB-T, DVB-C, DVB-S, DVB-S2 o ISDB-T). El bloque sintoniza el canal especificado y emite un flujo MPEG-TS sin procesar.

#### Información del bloque

Nombre: DVBSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Flujo MPEG-TS | 1 |

#### Configuración

`DVBSourceSettings` contiene parámetros de sintonización extensivos. Propiedades principales:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `AdapterID` | `int` | `0` | Índice del adaptador DVB |
| `FrontendID` | `int` | `0` | Índice del dispositivo frontend |
| `DeliverySystem` | `DVBDeliverySystem` | Undefined | DVB-T / DVB-C / DVB-S / DVB-S2 / ISDB-T |
| `Frequency` | `uint` | `0` | Frecuencia central en Hz (o kHz para satélite) |
| `Pids` | `string` | `"8192"` | Lista de PIDs (8192 = flujo de transporte completo) |
| `Modulation` | `DVBModulation` | QAM_16 | Esquema de modulación |
| `SymbolRate` | `uint` | `0` | Tasa de símbolo en kBd (satélite) |
| `BandwidthHZ` | `uint` | `0` | Ancho de banda del canal en Hz (DVB-T) |

Eventos durante la sintonización: `TuningStart`, `TuningDone`, `TuningFail`.

#### El pipeline de muestra

```mermaid
graph LR;
    DVBSourceBlock-->DecodeBinBlock;
    DecodeBinBlock -- Video --> VideoRendererBlock;
    DecodeBinBlock -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new DVBSourceSettings
{
    AdapterID = 0,
    DeliverySystem = DVBDeliverySystem.DVBT,
    Frequency = 506000000,
    BandwidthHZ = 8000000
};

var dvb = new DVBSourceBlock(settings);
dvb.TuningDone += (s, e) => Console.WriteLine("Sintonización completa");

var decoder = new DecodeBinBlock();
pipeline.Connect(dvb.VideoOutput, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = DVBSourceBlock.IsAvailable();
```

#### Plataformas

Windows, Linux.

---

### PulseAudio Source Block

El `PulseAudioSourceBlock` captura audio desde un dispositivo PulseAudio en Linux. Implementa `IAudioVolumeMute` para control de volumen y silencio en tiempo real.

#### Información del bloque

Nombre: PulseAudioSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida audio | Audio sin comprimir | 1 |

#### Configuración

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Device` | `AudioCaptureDeviceInfo` | — | Dispositivo de captura |
| `Format` | `AudioCaptureDeviceFormat` | — | Formato de audio |
| `DeviceName` | `string` | `null` | Nombre del dispositivo PulseAudio (null = predeterminado) |
| `ClientName` | `string` | `"VisioForge"` | Nombre visible en aplicaciones de control de audio |
| `ProvideClock` | `bool` | `true` | Proveer reloj para el pipeline |

#### El pipeline de muestra

```mermaid
graph LR;
    PulseAudioSourceBlock-->AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var device = devices[0];
var format = device.Formats[0];

var settings = new PulseAudioSourceSettings(device, format);
var audioSource = new PulseAudioSourceBlock(settings);

// Control de volumen en tiempo real
audioSource.SetVolume(0.8);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSource.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = PulseAudioSourceBlock.IsAvailable();
```

#### Plataformas

Linux.

---

### Raspberry Pi Source Block

El `RaspberryPiSourceBlock` captura video desde el Módulo de Cámara Raspberry Pi usando el elemento GStreamer `rpicamsrc`. Emite video codificado en H.264 y admite controles de cámara incluyendo rotación, volteo y ajustes de imagen.

#### Información del bloque

Nombre: RaspberryPiSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Video H.264 | 1 |

#### Configuración

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `CameraNumber` | `int` | `0` | Índice de cámara para configuraciones multi-cámara |
| `Width` | `uint` | `1920` | Ancho de captura en píxeles |
| `Height` | `uint` | `1080` | Alto de captura en píxeles |
| `FrameRate` | `VideoFrameRate` | FPS_30 | Tasa de fotogramas de captura |
| `Bitrate` | `uint` | `17000000` | Bitrate H.264 en bps |
| `Rotation` | `int` | `0` | Rotación en grados: 0, 90, 180, 270 |
| `HorizontalFlip` | `bool` | `false` | Voltear imagen horizontalmente |
| `VerticalFlip` | `bool` | `false` | Voltear imagen verticalmente |
| `Brightness` | `int` | `50` | Brillo: 0–100 |
| `Contrast` | `int` | `0` | Contraste: −100 a 100 |
| `Saturation` | `int` | `0` | Saturación: −100 a 100 |
| `ISO` | `uint` | `0` | Sensibilidad ISO (0 = automático) |
| `ExposureMode` | `string` | `"auto"` | Modo de exposición |
| `AWBMode` | `string` | `"auto"` | Modo de balance de blancos automático |

#### El pipeline de muestra

```mermaid
graph LR;
    RaspberryPiSourceBlock-->H264DecoderBlock;
    H264DecoderBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new RaspberryPiSourceSettings
{
    Width = 1920,
    Height = 1080,
    FrameRate = VideoFrameRate.FPS_30,
    Rotation = 0,
    Bitrate = 17000000
};

var camera = new RaspberryPiSourceBlock(settings);

var decoder = new H264DecoderBlock();
pipeline.Connect(camera.Output, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = RaspberryPiSourceBlock.IsAvailable();
```

#### Plataformas

Linux (Raspberry Pi).

---

### GStreamer PlayBin Source Block

El `PlayBinSourceBlock` reproduce medios usando el elemento `playbin` integrado de GStreamer. Acepta los mismos `UniversalSourceSettings` que `UniversalSourceBlock`, pero delega toda la construcción del pipeline de decodificación al motor de reproducción de alto nivel de GStreamer.

#### Información del bloque

Nombre: PlayBinSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Video sin comprimir | 0 o 1 |
| Salida audio | Audio sin comprimir | 0 o 1 |
| Salida subtítulo | Subtítulo | 0 o 1 |

#### Configuración

Usa `UniversalSourceSettings`. Propiedades clave:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `URI` | `Uri` | requerido | URI de la fuente de medios |
| `RenderVideo` | `bool` | `true` | Habilitar salida de video |
| `RenderAudio` | `bool` | `true` | Habilitar salida de audio |
| `RenderSubtitle` | `bool` | `false` | Habilitar salida de subtítulos |
| `StartPosition` | `TimeSpan?` | `null` | Posición de inicio de reproducción |
| `StopPosition` | `TimeSpan?` | `null` | Posición de fin de reproducción |
| `VideoCustomFrameRate` | `VideoFrameRate?` | `null` | Anular la tasa de fotogramas de salida |

#### El pipeline de muestra

```mermaid
graph LR;
    PlayBinSourceBlock -- Video --> VideoRendererBlock;
    PlayBinSourceBlock -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await UniversalSourceSettings.CreateAsync(new Uri("file:///video.mp4"));
var source = new PlayBinSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, Android, iOS.

---

### Basic File Source Block

El `BasicFileSourceBlock` lee datos binarios sin procesar de un archivo y los expone como un único pad de salida. Una etapa de detección de tipo opcional puede detectar el formato del flujo automáticamente para conectarlo a un demuxer o decodificador.

#### Información del bloque

Nombre: BasicFileSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida | Auto (datos sin procesar) | 1 |

#### Parámetros

| Parámetro | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `filename` | `string` | — | Ruta al archivo de origen |
| `addTypeFind` | `bool` | `false` | Agregar un elemento de detección de tipo para detección automática de formato |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock-->DecodeBinBlock;
    DecodeBinBlock -- Video --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Leer archivo con detección automática de tipo
var fileSource = new BasicFileSourceBlock("video.ts", addTypeFind: true);

var decoder = new DecodeBinBlock();
pipeline.Connect(fileSource.Output, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = BasicFileSourceBlock.IsAvailable();
```

#### Plataformas

Windows, macOS, Linux, Android, iOS.

---

### Custom Mixer Source Block

El `CustomMixerSourceBlock` es una fuente de modo push que acepta fotogramas de video y audio proporcionados externamente a través de colas administradas, exponiéndolos como pads de salida en vivo. Está diseñado para pipelines de mezcla y composición personalizados donde los fotogramas provienen del código de la aplicación.

#### Información del bloque

Nombre: CustomMixerSourceBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Video sin comprimir | 1 |
| Salida audio | Audio sin comprimir | 1 |

#### Configuración

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Width` | `int` | `1920` | Ancho del video en píxeles |
| `Height` | `int` | `1080` | Alto del video en píxeles |
| `VideoFormat` | `VideoFormatX` | BGRA | Formato de píxeles de video |
| `FrameRate` | `VideoFrameRate` | 30 fps | Tasa de fotogramas de salida |
| `AudioFormat` | `AudioFormatX` | S16LE | Formato de muestra de audio |
| `AudioSampleRate` | `int` | `48000` | Tasa de muestreo de audio en Hz |
| `AudioChannels` | `int` | `2` | Número de canales de audio |
| `IsLive` | `bool` | `true` | Marcar fuente como en vivo |
| `DoTimestamp` | `bool` | `true` | Añadir marcas de tiempo automáticamente a los búferes enviados |
| `EnableThreadedPush` | `bool` | `true` | Enviar fotogramas desde un hilo dedicado |
| `ManagedQueueMaxBuffers` | `int` | `4` | Máximo de fotogramas en cola antes de descartar |

#### Eventos

- `QueueDropped` — se activa cuando la cola administrada descarta un búfer por contrapresión.

#### El pipeline de muestra

```mermaid
graph LR;
    CustomMixerSourceBlock -- Video --> VideoRendererBlock;
    CustomMixerSourceBlock -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new CustomMixerSourceSettings
{
    Width = 1280,
    Height = 720,
    FrameRate = VideoFrameRate.FPS_30,
    AudioSampleRate = 48000,
    AudioChannels = 2
};

var mixerSource = new CustomMixerSourceBlock(settings);
mixerSource.QueueDropped += (s, e) => Console.WriteLine("Búfer descartado");

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixerSource.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = CustomMixerSourceBlock.IsAvailable();
```

#### Plataformas

Windows, macOS, Linux, Android, iOS.

---

### Image Video Source Cairo Block

El `ImageVideoSourceCairoBlock` renderiza una imagen estática o animada (incluidos GIFs animados) como un flujo de video continuo usando la biblioteca gráfica 2D Cairo. A diferencia de la fuente de imagen estándar, el tamaño del fotograma de salida se especifica explícitamente mediante el parámetro `Size` del constructor.

#### Información del bloque

Nombre: ImageVideoSourceCairoBlock.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Video sin comprimir | 1 |

#### Parámetros del constructor

| Parámetro | Tipo | Descripción |
| --- | --- | --- |
| `pipeline` | `MediaBlocksPipeline` | Pipeline propietario |
| `settings` | `ImageVideoSourceSettings` | Configuración de la fuente de imagen |
| `videoSize` | `Size` | Dimensiones del fotograma de salida en píxeles |

#### Configuración (ImageVideoSourceSettings)

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Filename` | `string` | requerido | Ruta al archivo de imagen |
| `FrameRate` | `VideoFrameRate` | 10 fps | Tasa de fotogramas de salida |
| `IsLive` | `bool` | `false` | Marcar fuente como en vivo |
| `NumBuffers` | `int` | `-1` | Fotogramas a emitir (−1 = ilimitado) |
| `AnimationInfiniteLoop` | `bool` | `true` | Repetir imágenes animadas infinitamente |

#### El pipeline de muestra

```mermaid
graph LR;
    ImageVideoSourceCairoBlock-->VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageVideoSourceSettings("image.png")
{
    FrameRate = VideoFrameRate.FPS_30,
    IsLive = true
};

var imageSource = new ImageVideoSourceCairoBlock(pipeline, settings, new Size(1920, 1080));

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(imageSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux.

---

### Universal Source Block Mini

El `UniversalSourceBlockMini` es una variante ligera de `UniversalSourceBlock` que usa los mismos `UniversalSourceSettings`. Usa un pipeline interno simplificado, adecuado para entornos con recursos limitados o escenarios donde la sobrecarga del motor de decodificación completo no es deseable.

#### Información del bloque

Nombre: UniversalSourceBlockMini.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Video sin comprimir | 0 o 1 |
| Salida audio | Audio sin comprimir | 0 o 1 |
| Salida subtítulo | Subtítulo | 0 o 1 |

#### Configuración

Usa `UniversalSourceSettings` — la misma clase que `UniversalSourceBlock`.

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `URI` | `Uri` | requerido | URI de la fuente de medios |
| `RenderVideo` | `bool` | `true` | Habilitar salida de video |
| `RenderAudio` | `bool` | `true` | Habilitar salida de audio |
| `RenderSubtitle` | `bool` | `false` | Habilitar salida de subtítulos |
| `StartPosition` | `TimeSpan?` | `null` | Posición de inicio de reproducción |
| `StopPosition` | `TimeSpan?` | `null` | Posición de fin de reproducción |

#### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlockMini -- Video --> VideoRendererBlock;
    UniversalSourceBlockMini -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await UniversalSourceSettings.CreateAsync(new Uri("file:///video.mp4"));
var source = new UniversalSourceBlockMini(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = UniversalSourceBlockMini.IsAvailable();
```

#### Plataformas

Windows, macOS, Linux, Android, iOS.

---

### Universal Source Block V2

El `UniversalSourceBlockV2` es una fuente de medios multistream avanzada que expone listas de pads de salida separadas para flujos de video, audio, subtítulos y metadatos. Es adecuado para archivos de medios complejos con múltiples pistas y escenarios que requieren control por flujo sobre la decodificación.

#### Información del bloque

Nombre: UniversalSourceBlockV2.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Salida video | Video sin comprimir | 0…N |
| Salida audio | Audio sin comprimir | 0…N |
| Salida subtítulo | Subtítulo | 0…N |
| Salida metadatos | Metadatos | 0…N |

#### Configuración (UniversalSourceSettingsV2)

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `URI` | `Uri` | requerido | URI de la fuente de medios |
| `RenderVideo` | `bool` | `true` | Habilitar pads de salida de video |
| `RenderAudio` | `bool` | `true` | Habilitar pads de salida de audio |
| `RenderSubtitle` | `bool` | `false` | Habilitar pads de salida de subtítulos |
| `StartPosition` | `TimeSpan?` | `null` | Posición de inicio de reproducción |
| `StopPosition` | `TimeSpan?` | `null` | Posición de fin de reproducción |
| `VideoCustomFrameRate` | `VideoFrameRate?` | `null` | Anular la tasa de fotogramas de salida |

#### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlockV2 -- Video --> VideoRendererBlock;
    UniversalSourceBlockV2 -- Audio --> AudioRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new UniversalSourceSettingsV2(new Uri("file:///multi_track.mkv"))
{
    RenderVideo = true,
    RenderAudio = true
};

var source = new UniversalSourceBlockV2(settings);

// Acceder al primer flujo de video y audio
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Plataformas

Windows, macOS, Linux, Android, iOS.

macOS (no disponible en iOS)