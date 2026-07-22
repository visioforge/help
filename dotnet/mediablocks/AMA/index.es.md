---
title: Codificación de video por hardware AMD AMA en C# .NET
description: Codifique y decodifique H.264, HEVC y AV1 en aceleradores AMD Alveo con el AMA Video SDK en VisioForge Media Blocks SDK para .NET en Linux.
sidebar_label: AMA
tags:
  - Media Blocks SDK
  - .NET
  - Linux
  - Streaming
  - AV1
  - H.265
primary_api_classes:
  - AMAH264EncoderSettings
  - AMAHEVCEncoderSettings
  - AMAAV1EncoderSettings
  - AMAH264DecoderSettings
  - AMAHEVCDecoderSettings
  - AMAAV1DecoderSettings
  - AMAScalerBlock
  - AMAScalerSettings

---

# Bloques AMD AMA - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El **AMD AMA Video SDK** es la plataforma de aceleración multimedia por hardware de AMD para los aceleradores de medios AMD Alveo. Incluye una familia de elementos de GStreamer (`ama_h264enc`, `ama_h265enc`, `ama_av1enc`, `ama_h264dec`, `ama_h265dec`, `ama_av1dec`, `ama_scaler`, `ama_upload`, `ama_download`) que ejecutan la codificación, decodificación y escalado de H.264, HEVC y AV1 íntegramente en el acelerador. VisioForge Media Blocks SDK envuelve esta plataforma para que usted pueda descargar el procesamiento de video al hardware de AMD desde un pipeline estándar de C#. La plataforma AMA es distinta de AMD AMF (que controla las GPU Radeon): AMA se dirige a aceleradores de medios Alveo dedicados, como el **Alveo MA35D**, el dispositivo compatible actualmente.

**Datos clave:**

- **Plataforma:** AMD AMA Video SDK en aceleradores de medios AMD Alveo (actualmente el Alveo MA35D).
- **Códecs:** H.264, HEVC (H.265) y AV1: codificación y decodificación por hardware.
- **Sistema operativo:** solo Linux x86_64. No hay compatibilidad con AMA en Windows ni macOS.
- **Selección de dispositivo:** la propiedad `Device` elige el acelerador: `-1` (predeterminado) selecciona automáticamente, `0`/`1`/`2`… fijan una tarjeta concreta cuando hay varios dispositivos AMD presentes.
- **Modelo de memoria:** la codificación, decodificación y escalado se ejecutan en la memoria del dispositivo; el SDK inserta `ama_upload` / `ama_download` automáticamente cuando los datos cruzan hacia o desde la memoria del sistema.
- **Distribución:** VisioForge envía únicamente el wrapper administrado. El runtime de AMA (controlador de kernel + plugins de GStreamer) se instala por separado desde AMD; el SDK no incluye ningún binario de AMD.

## Requisitos previos

Antes de poder usar los bloques AMA, la máquina de destino debe tener:

- Linux x86_64 (Ubuntu, Alma/RHEL o Debian según la matriz de compatibilidad del AMD AMA SDK).
- Un acelerador de medios AMD Alveo instalado y aprovisionado (controlador de kernel, huge pages, IOMMU / decodificación por encima de 4 GB habilitada en el firmware del host).
- El AMD AMA Video SDK instalado, incluidos sus plugins de GStreamer, para que los elementos `ama_*` sean detectables por GStreamer.

Cada clase de configuración y bloque de AMA expone un método estático `IsAvailable()` que comprueba la presencia de los elementos `ama_*` requeridos. Llámelo antes de construir un pipeline y recurra a un codificador por software o de otro proveedor cuando devuelva `false`:

```csharp
if (AMAH264EncoderSettings.IsAvailable())
{
    // El codificador H.264 AMA está presente: use la codificación por hardware.
}
```

## Codificador H.264 AMA

Codificador H.264 por hardware basado en el elemento `ama_h264enc`. Configúrelo con `AMAH264EncoderSettings` (implementa `IH264EncoderSettings`) y páselo a un `H264EncoderBlock` estándar.

### Información del bloque

Nombre: H264EncoderBlock (con `AMAH264EncoderSettings`).

Dirección del pin | Tipo de medio | Cantidad de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | H264 | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

// Codificador H.264 por hardware AMD AMA (Alveo). Device = -1 selecciona el acelerador automáticamente.
var h264Settings = new AMAH264EncoderSettings
{
    Device = -1,
    Bitrate = 6000, // Kbps
    RateControl = AMARateControl.CBR
};
var h264EncoderBlock = new H264EncoderBlock(h264Settings);
pipeline.Connect(fileSource.VideoOutput, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Linux (x86_64) con el AMD AMA Video SDK.

## Codificador HEVC AMA

Codificador HEVC (H.265) por hardware basado en el elemento `ama_h265enc`. Configúrelo con `AMAHEVCEncoderSettings` (implementa `IHEVCEncoderSettings`) y páselo a un `HEVCEncoderBlock` estándar.

### Información del bloque

Nombre: HEVCEncoderBlock (con `AMAHEVCEncoderSettings`).

Dirección del pin | Tipo de medio | Cantidad de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | HEVC | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->HEVCEncoderBlock;
    HEVCEncoderBlock-->MP4SinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

var hevcSettings = new AMAHEVCEncoderSettings
{
    Device = -1,
    Bitrate = 6000, // Kbps
    TuneMetrics = AMATuneMetrics.VMAF
};
var hevcEncoderBlock = new HEVCEncoderBlock(hevcSettings);
pipeline.Connect(fileSource.VideoOutput, hevcEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(hevcEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Linux (x86_64) con el AMD AMA Video SDK.

## Codificador AV1 AMA

Codificador AV1 por hardware basado en el elemento `ama_av1enc`. Configúrelo con `AMAAV1EncoderSettings` (implementa `IAV1EncoderSettings`) y páselo a un `AV1EncoderBlock` estándar. AV1 añade una propiedad `DeviceType` (`AMAAV1DeviceType`) que selecciona el motor de AV1.

### Información del bloque

Nombre: AV1EncoderBlock (con `AMAAV1EncoderSettings`).

Dirección del pin | Tipo de medio | Cantidad de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | AV1 | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->AV1EncoderBlock;
    AV1EncoderBlock-->MP4SinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

var av1Settings = new AMAAV1EncoderSettings
{
    Device = -1,
    DeviceType = AMAAV1DeviceType.Type1,
    Bitrate = 6000 // Kbps
};
var av1EncoderBlock = new AV1EncoderBlock(av1Settings);
pipeline.Connect(fileSource.VideoOutput, av1EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(av1EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Linux (x86_64) con el AMD AMA Video SDK.

## Configuración común del codificador

Las tres clases de configuración de codificadores AMA comparten las mismas propiedades principales (AV1 expone además `DeviceType`, y su rango de QP es 0–255 en lugar de 0–51):

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Device` | int | −1 | Acelerador en el que codificar. −1 selecciona automáticamente. |
| `Slice` | int | −1 | Sub-motor (slice) del dispositivo. −1 = automático. |
| `Bitrate` | uint | 5000 | Bitrate objetivo en Kbps. |
| `MaxBitrate` | int | −1 | Bitrate máximo en Kbps para VBR/CVBR. −1 = automático. |
| `RateControl` | `AMARateControl` | Auto | Modo de control de tasa. |
| `QPMode` | `AMAQPMode` | Auto | Modo de control de QP. |
| `QP` | int | −1 | QP fijo en modo QP constante. −1 = deshabilitado. |
| `MinQP` / `MaxQP` | int | 0 / 51 (255 para AV1) | Rango de QP permitido. |
| `BFrames` | int | −1 | Fotogramas B entre fotogramas P. −1 = automático. |
| `GOPLength` | int | −1 | Distancia máxima entre fotogramas I. −1 = automático. |
| `Tier` | `AMATier` | Auto | Tier de codificación. |
| `TuneMetrics` | `AMATuneMetrics` | VQ | Métrica de calidad objetiva para la que ajustar. |
| `SpatialAQ` / `TemporalAQ` | bool | true | Interruptores de cuantización adaptativa (con `*Gain` 0–255). |
| `LookaheadDepth` | int | −1 | Profundidad de look-ahead. −1 = automático. |
| `LatencyMs` | int | −1 | Latencia objetivo del codificador (ms). −1 = automático. |

## Decodificadores AMA

Decodificadores por hardware para H.264 (`ama_h264dec`), HEVC (`ama_h265dec`) y AV1 (`ama_av1dec`). Configúrelos con `AMAH264DecoderSettings` / `AMAHEVCDecoderSettings` / `AMAAV1DecoderSettings` (que implementan `IH264/HEVC/AV1DecoderSettings`) y páselos al bloque decodificador correspondiente. Cuando un decodificador alimenta a un consumidor de memoria del sistema, el SDK añade `ama_download` automáticamente.

### Información del bloque

Nombre: `H264DecoderBlock` / `HEVCDecoderBlock` / `AV1DecoderBlock` (con un objeto de configuración AMA).

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Video de entrada | Video codificado | 1 |
| Video de salida | Video sin comprimir | 1 |

### Configuración

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Device` | int | −1 | Acelerador en el que decodificar. −1 selecciona automáticamente. |
| `LowLatency` | bool | false | Habilita la decodificación de baja latencia. |
| `LatencyLogging` | bool | false | Registra la información de latencia en syslog. |
| `AllowDownscaling` | bool | false | Permite el reescalado a la baja a nivel del decodificador. |

### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock-->UniversalDemuxBlock;
    UniversalDemuxBlock-->HEVCDecoderBlock;
    HEVCDecoderBlock-->VideoRendererBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Decodificador HEVC por hardware AMD AMA (Alveo).
var hevcDecoder = new HEVCDecoderBlock(new AMAHEVCDecoderSettings { Device = -1 });

var basicFileSource = new BasicFileSourceBlock("test_hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_hevc.mp4");
var mediaInfo = reader.Info;
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), hevcDecoder.Input);
pipeline.Connect(hevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Plataformas

Linux (x86_64) con el AMD AMA Video SDK.

## Escalador AMA

El `AMAScalerBlock` redimensiona el video (y, opcionalmente, reduce a la mitad la tasa de fotogramas) en el acelerador usando el elemento `ama_scaler`. El bloque envuelve la cadena `ama_upload ! ama_scaler ! capsfilter ! ama_download`, de modo que acepta fotogramas de memoria del sistema en su entrada y emite fotogramas de memoria del sistema aguas abajo: puede insertarlo en cualquier pipeline sin gestionar usted mismo la memoria del dispositivo.

### Información del bloque

Nombre: `AMAScalerBlock`.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Video de entrada | Video (memoria del sistema) | 1 |
| Video de salida | Video (memoria del sistema) | 1 |

### Configuración

`AMAScalerSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Width` / `Height` | int | 0 | Tamaño de salida objetivo en píxeles (144–7580). 0 deja la dimensión sin restringir. |
| `Device` | int | −1 | Acelerador en el que escalar. −1 selecciona automáticamente. |
| `Framerate` | `AMAScalerFramerate` | Auto | Modo de tasa de fotogramas de salida (Auto / Full / Half). |

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->AMAScalerBlock;
    AMAScalerBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

// Escalar a 1280x720 en el acelerador de AMD.
var scaler = new AMAScalerBlock(new AMAScalerSettings(1280, 720));
pipeline.Connect(fileSource.VideoOutput, scaler.Input);

var h264EncoderBlock = new H264EncoderBlock(new AMAH264EncoderSettings());
pipeline.Connect(scaler.Output, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Linux (x86_64) con el AMD AMA Video SDK.

## Transcodificación completa por hardware

Un decodificador AMA, el escalador AMA y un codificador AMA encadenados ejecutan cada etapa —decodificación, escalado y codificación— en el acelerador de AMD. Cada bloque es autónomo: acepta y emite fotogramas en memoria del sistema, por lo que el SDK mueve los datos hacia y desde el dispositivo (`ama_download` / `ama_upload`) en cada límite de bloque, mientras que el trabajo de decodificación, escalado y codificación se ejecuta en el acelerador:

```mermaid
graph LR;
    Source-->AMADecoder-->AMAScaler-->AMAEncoder-->Sink;
```

Conecte en secuencia un `H264DecoderBlock` (con `AMAH264DecoderSettings`), un `AMAScalerBlock` y un `HEVCEncoderBlock` (con `AMAHEVCEncoderSettings`) para transcodificar H.264 → HEVC escalado, con cada etapa ejecutándose en el acelerador de AMD.

## Referencia de enumeraciones

| Enumeración | Miembros |
| --- | --- |
| `AMARateControl` | Auto, CQP, CBR, VBR, CVBR |
| `AMAQPMode` | Auto, RelativeLoad, Uniform |
| `AMATier` | Auto, Main, High |
| `AMATuneMetrics` | VQ, PSNR, SSIM, VMAF |
| `AMAAV1DeviceType` | Any, Type1, Type2 |
| `AMAScalerFramerate` | Auto, Full, Half |

## Selección del dispositivo

Cada objeto de configuración AMA (`AMA*EncoderSettings`, `AMA*DecoderSettings` y `AMAScalerSettings`) expone una propiedad `Device` que decide qué acelerador de AMD ejecuta el trabajo — establézcala en el objeto de configuración que pasa al bloque (los bloques en sí no tienen una propiedad `Device`):

- `Device = -1` (predeterminado): el runtime de AMA selecciona automáticamente un acelerador disponible.
- `Device = 0` / `1` / `2`…: fija el trabajo a una tarjeta concreta, que es como usted elige un acelerador Alveo cuando hay varios dispositivos AMD instalados en el mismo host.

Como AMA se dirige a aceleradores de medios Alveo dedicados en lugar de a una GPU Radeon, seleccionar una clase de configuración AMA ya dirige el trabajo al acelerador, no a ningún gráfico AMD integrado.

## Limitaciones

- **Solo Linux x86_64.** Los elementos AMA no existen en Windows ni macOS; las clases de configuración se compilan únicamente en la versión de Linux del SDK.
- **Solo el pipeline de Media Blocks.** Los codificadores AMA se ejecutan en el pipeline directo de Media Blocks. No son compatibles con la ruta de salida encodebin de `VideoEditCoreX` y lanzan una `NotSupportedException` allí: construya la codificación AMA con `H264EncoderBlock` / `HEVCEncoderBlock` / `AV1EncoderBlock` en su lugar.
- El runtime de AMA debe instalarse por separado (consulte [Requisitos previos](#requisitos-previos)); el SDK no incluye ningún binario de AMD.

## Preguntas frecuentes

### ¿Es compatible VisioForge con la codificación por hardware AMD AMA y Alveo?

Sí. VisioForge Media Blocks SDK envuelve el AMD AMA Video SDK para ofrecer codificación y decodificación por hardware de H.264, HEVC y AV1, además de un escalador por hardware, en aceleradores de medios AMD Alveo. La compatibilidad es solo para Linux x86_64 y requiere que el AMD AMA Video SDK esté instalado.

### ¿Cómo elijo en qué dispositivo AMD se ejecuta el codificador AMA?

Establezca la propiedad `Device` en el objeto de configuración AMA. `-1` (el valor predeterminado) selecciona automáticamente un acelerador disponible; `0`, `1`, `2`, etc., fijan la codificación, decodificación o escalado a una tarjeta concreta cuando hay varios dispositivos AMD presentes en el host.

### ¿Está disponible la aceleración por hardware AMA en Windows o macOS?

No. El AMD AMA Video SDK es solo para Linux x86_64, por lo que los bloques de codificador, decodificador y escalador AMA solo están disponibles en Linux. En Windows o macOS, use los codificadores AMF, NVENC, QSV o por software en su lugar.

### ¿Cuál es la diferencia entre los codificadores AMD AMF y AMD AMA?

AMD AMF controla las GPU Radeon y está disponible en Windows, Linux y macOS a través de clases como `AMFH264EncoderSettings`. AMD AMA es una plataforma independiente que controla aceleradores de medios Alveo dedicados (como el MA35D) en Linux, expuestos a través de las clases `AMA*`. Se dirigen a hardware diferente y se configuran con clases de configuración distintas.

### ¿Cómo ejecuto una transcodificación completa en el acelerador de AMD?

Encadene un decodificador AMA, el `AMAScalerBlock` y un codificador AMA. Cada etapa se ejecuta en el acelerador de AMD; los bloques intercambian fotogramas a través de la memoria del sistema, y el SDK mueve los datos hacia y desde el dispositivo automáticamente (`ama_download` / `ama_upload`) en cada límite.

## Consulte también

- [Codificadores de Video](../VideoEncoders/index.md): todos los bloques de codificadores H.264, HEVC, AV1 y VP9, incluidos los codificadores por hardware AMD AMF, NVENC y QSV.
- [Decodificadores de Video](../VideoDecoders/index.md): bloques de decodificadores por hardware y por software para todos los códecs compatibles.
- [Bloques específicos de Linux](../_Linux/index.md): bloques y funciones disponibles en las versiones de Linux del SDK.
- [Guía de codificación HEVC](../../general/video-encoders/hevc.md): codificación HEVC por hardware en AMD, NVIDIA e Intel.
- [Guía de codificación AV1](../../general/video-encoders/av1.md): opciones de codificación AV1 por hardware y por software.
