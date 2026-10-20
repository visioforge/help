---
title: Decodificación de video por GPU con Direct3D 11 en C# .NET
description: Use decodificación de video acelerada por hardware con Direct3D 11, efectos e integración con DirectShow en Windows con VisioForge Media Blocks SDK para .NET.
sidebar_label: Windows Platform
tags:
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - D3D11H264DecoderBlock
  - D3D11UploadBlock
  - D3D11DownloadBlock
  - D3D12InterlaceBlock
  - D3D12InterlaceSettings
  - D3D12FisheyeDewarpBlock
  - D3D12FisheyeDewarpSettings
  - D3D12RemapBlock
  - D3D12RemapSettings
  - D3D12UVRemapMap
  - UniversalSourceBlock
  - UniversalSourceSettings

---

# Bloques de plataforma Windows - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección cubre los MediaBlocks específicamente optimizados para plataformas Windows.

## Bloques disponibles

### Decodificadores de hardware Direct3D 11

Windows proporciona decodificación de video acelerada por hardware mediante Direct3D 11:

- **D3D11H264DecoderBlock**: decodificación H.264/AVC por hardware
- **D3D11H265DecoderBlock**: decodificación H.265/HEVC por hardware
- **D3D11VP8DecoderBlock**: decodificación VP8 por hardware
- **D3D11VP9DecoderBlock**: decodificación VP9 por hardware
- **D3D11AV1DecoderBlock**: decodificación AV1 por hardware
- **D3D11MPEG2DecoderBlock**: decodificación MPEG-2 por hardware

Consulte la [documentación de decodificadores de video](../VideoDecoders/index.md)

### Procesamiento Direct3D 11

- **D3D11UploadBlock**: sube video desde la memoria del sistema a la GPU
- **D3D11DownloadBlock**: descarga video desde la GPU a la memoria del sistema
- **D3D11VideoConverterBlock**: conversión de espacio de color acelerada por GPU

Consulte la [documentación de procesamiento de video](../VideoProcessing/index.md#d3d11-video-converter)

### Composición Direct3D 11

- **D3D11VideoCompositorBlock**: composición y mezcla de video acelerada por GPU

### Procesamiento Direct3D 12

- **D3D12InterlaceBlock**: conversión en la GPU de video progresivo a video entrelazado, para difusión y otras salidas que requieren un orden de campos explícito

Es lo contrario del desentrelazado (`DeinterlaceBlock`): la entrada debe ser progresiva y la salida es video entrelazado (interleaved). `D3D12InterlaceSettings` define:

- `FieldPattern`: `OneToOne` combina dos fotogramas progresivos en un fotograma entrelazado y reduce a la mitad la velocidad de fotogramas (60p a 60i, 30 fotogramas por segundo); `TwoToTwo` divide cada fotograma progresivo en dos campos y conserva la velocidad de fotogramas (30p a 60i, 30 fotogramas por segundo)
- `TopFieldFirst`: campo superior primero con `true`, campo inferior primero con `false` (valor predeterminado)
- `Engine`: el motor de la GPU (`Auto`, `ThreeD`, `Compute`)
- `AdapterIndex`: el adaptador de GPU, `-1` para el adaptador predeterminado

El bloque mueve por sí mismo los fotogramas hacia la GPU y desde ella: tanto su entrada como su salida aceptan memoria del sistema, D3D11 o D3D12, según el bloque vecino, y la memoria D3D12 del adaptador de GPU del propio bloque se pasa sin copia tanto en la entrada como en la salida. Requiere una GPU compatible con Direct3D 12. El consumidor posterior debe aceptar video entrelazado (por ejemplo, el codificador MPEG-2): uno que solo acepta video progresivo no se puede enlazar con el bloque, y el pipeline informa del enlace fallido mediante `OnError`. Un adaptador de GPU inexistente hace fallar el inicio del pipeline.

- **D3D12FisheyeDewarpBlock**: corrección en la GPU de una imagen fisheye circular a una vista equirrectangular, panorámica o en perspectiva

`D3D12FisheyeDewarpSettings` define:

- `Projection`: `Equirectangular` (valor predeterminado), `Panorama`, `Perspective` o `Passthrough`
- `CenterX`, `CenterY`, `RadiusX`, `RadiusY`: el círculo fisheye en la imagen de entrada, normalizado de 0 a 1, con el radio de al menos 1e-6 (por defecto: centrado, radio 0,5)
- `FisheyeFOV`: el campo de visión del objetivo en grados (180 por defecto)
- `HorizontalFOV`, `VerticalFOV`: el campo de visión de la salida en grados (90 por defecto); `Panorama` no los usa. El elemento reduce los ángulos módulo 360; una vez reducido, un ángulo debe estar entre 0,001 y 359,999 grados, y no superar 179,999 para `Perspective`. `FisheyeFOV` sigue el rango de 0,001 a 359,999 en todas las proyecciones. Un fotograma `Equirectangular` abarca `HorizontalFOV` por `VerticalFOV`: uno amplio usa, por ejemplo, 300 por 180
- `RotationX` (cabeceo), `RotationY` (guiñada), `RotationZ` (alabeo) en grados, con `RotationOrder` y `RotationSpace` (`Local`, relativo a la cámara, o `World`, ejes fijos); a `Panorama` solo se aplica el alabeo
- `InnerRadius`: la parte central que se recorta de un `Panorama`, normalizada (0,3 por defecto)
- `RoiX`, `RoiY`, `RoiWidth`, `RoiHeight`: la región de interés en el espacio de la imagen de salida, normalizada, con ancho y alto de al menos 1e-6; el elemento no la limita. Fuera del fotograma en horizontal gira la vista (un panorama cruza su costura); en vertical, un panorama muestra filas negras y un fotograma equirrectangular se pliega sobre el polo
- `ViewportX`, `ViewportY`, `ViewportWidth`, `ViewportHeight`: dónde se dibuja el resultado en el fotograma de salida, normalizado, con ancho y alto de al menos 1e-6
- `AdapterIndex`: el adaptador de GPU, `-1` para el adaptador predeterminado

Todos los ajustes excepto `AdapterIndex` se pueden cambiar mientras el pipeline se ejecuta: modifique `Settings` y llame a `Update()`, que devuelve `false` cuando un valor está fuera de rango y, una vez que el bloque está en un pipeline, informa de la causa mediante `OnError`. Así, una sola cámara fisheye se convierte en una cámara PTZ virtual. La salida conserva el tamaño, el formato y la velocidad de fotogramas de la entrada; coloque un `VideoResizeBlock` después del bloque para obtener otro tamaño, por ejemplo 2:1 para un fotograma equirrectangular. Al igual que el bloque de entrelazado, mueve por sí mismo los fotogramas hacia la GPU y desde ella, y requiere una GPU compatible con Direct3D 12. En la CPU, la alternativa es `CVDewarpBlock` (OpenCV).

- **D3D12RemapBlock**: reasignación del video en la GPU según un mapa UV, para la calibración de lentes, la corrección de distorsión y otras transformaciones geométricas

`D3D12RemapSettings` configura:

- `Map`: un `D3D12UVRemapMap`, o `null` para dejar pasar el video sin cambios
- `AdapterIndex`: el adaptador de GPU, `-1` para el adaptador predeterminado

Un `D3D12UVRemapMap` se crea a partir de su ancho y su alto (cada uno de 1 a 16384) y de una matriz de valores `ushort`, cuatro por punto del mapa, fila por fila desde la esquina superior izquierda: U, V, sin uso y máscara. U y V son la posición de entrada que muestra el punto, de 0 a 65535 para 0 a 1 del ancho y del alto de la entrada. Una máscara de 32768 o más muestra allí la entrada; una menor muestra negro. El mapa se estira sobre el fotograma de salida, por lo que su tamaño no tiene que coincidir con el del video: el punto (x, y) del mapa corresponde a la posición de salida ((x + 0,5) / ancho, (y + 0,5) / alto). El mapa se copia al crearlo y ocupa 8 bytes por punto; un mapa del tamaño del video es más que suficiente. Un pipeline guardado (`ToJson`, `SaveJsonAsync`) incluye el mapa, de hasta 80 millones de puntos (8192x8192 tiene 67 millones); un pipeline con un mapa más grande se guarda sin los ajustes del bloque, y `ToDocument` lo informa. Para reemplazarlo o quitarlo mientras el pipeline se ejecuta, modifique `Settings.Map` y llame a `Update()`; el siguiente fotograma muestra el cambio. El bloque sube el mapa a la GPU en la que están los fotogramas de video y es el propietario de esa copia, por lo que la aplicación nunca maneja un recurso Direct3D; el fotograma que recibe un mapa nuevo espera a que se suba. Un fallo de la subida se informa mediante `OnError`, y el mapa anterior sigue en uso (si no lo hay, o se hizo para otra GPU, el video pasa sin cambios) hasta el siguiente `Update()`, que vuelve a subir el mapa, o hasta que el video pase a otra GPU. Al igual que el bloque de entrelazado, mueve por sí mismo los fotogramas hacia la GPU y desde ella, y requiere una GPU compatible con Direct3D 12.

### Efectos de video de Windows

- **VideoEffectsWinBlock**: efectos de video específicos de Windows
- **VR360ProcessorBlock**: procesamiento de video VR 360 grados

### Bloques especiales

Consulte la [documentación de bloques especiales](../Special/index.md)

## Requisitos de plataforma

- **Windows**: Windows 7 SP1 o superior
- **Direct3D 11**: GPU compatible con D3D11
- **Decodificación por hardware**: GPU con soporte de decodificación de video por hardware

## Características

- **Aceleración por hardware**: aproveche la GPU para codificación, decodificación y procesamiento
- **Integración con Direct3D 11**: procesamiento de video eficiente en la GPU
- **Bajo uso de CPU**: descarga el procesamiento al hardware dedicado
- **Alto rendimiento**: gestiona simultáneamente varios flujos HD/4K
- **Eficiencia energética**: reduce el consumo de energía con aceleración por hardware

## Código de ejemplo

### Decodificación H.264 por hardware

```csharp
var pipeline = new MediaBlocksPipeline();

// El decodificador recibe el flujo codificado: demultiplexar el MP4 y analizar su H.264 en lugar de decodificarlo en un bloque de origen (otro contenedor necesita su propio demultiplexor)
var fileSource = new BasicFileSourceBlock("video.mp4");
var demux = new QTDemuxBlock(renderVideo: true, renderAudio: false);
var parse = new H264ParseBlock();
pipeline.Connect(fileSource.Output, demux.Input);
pipeline.Connect(demux.VideoOutput, parse.Input);

// Decodificador D3D11 por hardware
var d3d11Decoder = new D3D11H264DecoderBlock();
pipeline.Connect(parse.Output, d3d11Decoder.Input);

// Procesar en GPU o descargar a la memoria del sistema
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Decoder.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Pipeline de procesamiento de video en GPU

```csharp
var pipeline = new MediaBlocksPipeline();

// Decodificación por software: un decodificador por hardware ya entrega memoria de GPU y la subida no tendría nada que hacer
var settings = await UniversalSourceSettings.CreateAsync("video.mp4");
settings.DisableHWDecoders = true;
var fileSource = new UniversalSourceBlock(settings);

// Subir a la GPU
var d3d11Upload = new D3D11UploadBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Upload.Input);

// Conversión de color en GPU, aplicada cuando el bloque siguiente necesita otro formato
var d3d11Converter = new D3D11VideoConverterBlock();
pipeline.Connect(d3d11Upload.Output, d3d11Converter.Input);

// Descargar desde la GPU
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Converter.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### De progresivo a entrelazado en la GPU

```csharp
var pipeline = new MediaBlocksPipeline();

// Una fuente progresiva 25p
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// 25p a 50i, campo superior primero
var interlace = new D3D12InterlaceBlock(new D3D12InterlaceSettings
{
    FieldPattern = D3D12InterlacePattern.TwoToTwo,
    TopFieldFirst = true
});
pipeline.Connect(fileSource.VideoOutput, interlace.Input);

// MPEG-2 conserva el entrelazado y el orden de campos en el flujo
var encoder = new MPEG2EncoderBlock(new MPEG2VideoEncoderSettings());
pipeline.Connect(interlace.Output, encoder.Input);

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts"));
pipeline.Connect(encoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Corrección fisheye en la GPU

```csharp
var pipeline = new MediaBlocksPipeline();

// Una grabación fisheye circular
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("fisheye.mp4"));

// Una vista en perspectiva de 90 grados, girada 30 grados alrededor del eje vertical (guiñada)
var dewarp = new D3D12FisheyeDewarpBlock(new D3D12FisheyeDewarpSettings
{
    Projection = D3D12FisheyeProjection.Perspective,
    RotationY = 30
});
pipeline.Connect(fileSource.VideoOutput, dewarp.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(dewarp.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Más tarde, durante la reproducción: girar la vista
dewarp.Settings.RotationY = -45;
dewarp.Update();
```

### Reasignación de video en la GPU

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// Su propio mapa, por ejemplo de una calibración de lente: 1920 x 1080 puntos, cuatro valores cada uno (U, V, sin uso, máscara)
ushort[] calibration = LoadCalibrationMap();
var remap = new D3D12RemapBlock(new D3D12RemapSettings
{
    Map = new D3D12UVRemapMap(1920, 1080, calibration)
});
pipeline.Connect(fileSource.VideoOutput, remap.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(remap.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Más tarde, durante la reproducción: cambiar a otra calibración
remap.Settings.Map = new D3D12UVRemapMap(1920, 1080, LoadOtherCalibrationMap());
remap.Update();
```

### Composición de video

```csharp
var pipeline = new MediaBlocksPipeline();

var source1 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video1.mp4"));
var source2 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video2.mp4"));

// Subir ambos a la GPU
var upload1 = new D3D11UploadBlock();
var upload2 = new D3D11UploadBlock();
pipeline.Connect(source1.VideoOutput, upload1.Input);
pipeline.Connect(source2.VideoOutput, upload2.Input);

// Configurar el compositor — dos flujos en un canvas 1920x1080 @ 30 fps
var compositorSettings = new D3D11VideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_30);
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(0, 0, 960, 1080), zorder: 0));
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(960, 0, 1920, 1080), zorder: 1));

// Componer en GPU — cada flujo añadido crea un pad de entrada; acceda mediante Inputs[].
var compositor = new D3D11VideoCompositorBlock(compositorSettings);
pipeline.Connect(upload1.Output, compositor.Inputs[0]);
pipeline.Connect(upload2.Output, compositor.Inputs[1]);

// Descargar resultado
var download = new D3D11DownloadBlock();
pipeline.Connect(compositor.Output, download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Consejos de rendimiento

- Mantenga el video en memoria GPU entre operaciones para evitar el coste de upload/download
- Use decodificadores por hardware cuando estén disponibles para mejor rendimiento
- Encadene operaciones en GPU antes de descargar a la memoria del sistema
- Monitorice el uso de memoria GPU al procesar varios flujos
- Compruebe el soporte de hardware antes de usar bloques D3D11

## Comprobar el soporte de hardware

```csharp
// Comprobar si el decodificador D3D11 H.264 está disponible
if (D3D11H264DecoderBlock.IsAvailable())
{
    // Usar decodificador por hardware
    var decoder = new D3D11H264DecoderBlock();
}
else
{
    // Recurrir a decodificador por software
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
}
```

## Plataformas

Windows 7 SP1 o superior (Windows 10/11 recomendado para mejor soporte de hardware).

## Documentación relacionada

- [VideoDecoders](../VideoDecoders/index.md) — bloques de decodificación por hardware
- [VideoProcessing](../VideoProcessing/index.md) — bloques de procesamiento por GPU
- [Special](../Special/index.md) — utilidades específicas de plataforma
