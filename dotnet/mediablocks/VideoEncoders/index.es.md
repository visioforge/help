---
title: Dominando los Codificadores de Video en .NET SDK
description: Codifica video con códecs AV1, H264, HEVC, VP9 y más usando aceleración GPU y configuraciones optimizadas en Media Blocks SDK para .NET.
sidebar_label: Codificadores de Video
order: 18
---

# Codificación de video

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

La codificación de video es el proceso de convertir datos de video sin comprimir en un formato comprimido. Este proceso es esencial para reducir el tamaño de los archivos de video, facilitando su almacenamiento y transmisión por internet. VisioForge Media Blocks SDK proporciona una amplia gama de codificadores de video que soportan varios formatos y códecs.

Para algunos codificadores de video, el SDK puede usar aceleración GPU para acelerar el proceso de codificación. Esta característica es especialmente útil cuando se trabaja con archivos de video de alta resolución o cuando se codifican múltiples videos simultáneamente.

Se soportan GPUs NVidia, Intel y AMD para aceleración de hardware.

## Codificador AV1

`AV1 (AOMedia Video 1)`: Desarrollado por la Alliance for Open Media, AV1 es un formato de codificación de video abierto y libre de regalías diseñado para transmisiones de video por Internet. Es conocido por su alta eficiencia de compresión y mejor calidad a tasas de bits más bajas en comparación con sus predecesores, haciéndolo adecuado para aplicaciones de streaming de video de alta resolución.

Usa clases que implementan la interfaz `IAV1EncoderSettings` para establecer los parámetros.

#### Codificadores CPU

##### AOMAV1EncoderSettings

Configuraciones del codificador AOM AV1. Codificador CPU.

**Plataformas:** Windows, Linux, macOS.

##### RAV1EEncoderSettings

Configuraciones del codificador RAV1E AV1. Codificador CPU.

- **Propiedades clave**:
  - `Bitrate` (entero): Tasa de bits objetivo en kilobits por segundo.
  - `LowLatency` (booleano): Habilita o deshabilita el modo de baja latencia. Predeterminado es `false`.
  - `MaxKeyFrameInterval` (ulong): Intervalo máximo entre fotogramas clave. Predeterminado es `240`.
  - `MinKeyFrameInterval` (ulong): Intervalo mínimo entre fotogramas clave. Predeterminado es `12`.
  - `MinQuantizer` (uint): Valor mínimo del cuantizador (rango 0-255). Predeterminado es `0`.
  - `Quantizer` (uint): Valor del cuantizador (rango 0-255). Predeterminado es `100`.
  - `SpeedPreset` (int): Preajuste de velocidad de codificación (10 más rápido, 0 más lento). Predeterminado es `6`.
  - `Tune` (`RAV1EEncoderTune`): Configuración de ajuste para el codificador. Predeterminado es `RAV1EEncoderTune.Psychovisual`.

**Plataformas:** Windows, Linux, macOS.

###### Enum RAV1EEncoderTune

Especifica la opción de ajuste para el codificador RAV1E.

- `PSNR` (0): Ajusta para mejor PSNR (Relación Señal-Ruido de Pico).
- `Psychovisual` (1): Ajusta para calidad psicovisual.

#### Codificadores GPU

##### AMFAV1EncoderSettings

Codificador de video AV1 GPU AMD.

**Plataformas:** Windows, Linux, macOS.

##### NVENCAV1EncoderSettings

Codificador de video AV1 GPU Nvidia.

**Plataformas:** Windows, Linux, macOS.

##### QSVAV1EncoderSettings

Codificador de video AV1 GPU Intel.

**Plataformas:** Windows, Linux, macOS.

*Nota: Los codificadores Intel QSV también pueden utilizar enumeraciones comunes como `QSVCodingOption` (`On`, `Off`, `Unknown`) para configurar características específicas de hardware.*

### Información del bloque

Nombre: AV1EncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
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

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new AV1EncoderBlock(new QSVAV1EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(videoEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador DV

`DV (Digital Video)`: Un formato para almacenar video digital introducido en los años 90, usado principalmente en videocámaras digitales de consumo. DV emplea compresión intra-frame para entregar video de alta calidad en cintas digitales, haciéndolo adecuado para videos caseros así como producciones semi-profesionales.

### Información del bloque

Nombre: DVEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | video/x-dv | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->DVEncoderBlock;
    DVEncoderBlock-->AVISinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new DVEncoderBlock(new DVVideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var sinkBlock = new AVISinkBlock(new AVISinkSettings(@"output.avi"));
pipeline.Connect(videoEncoderBlock.Output, sinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador de video personalizado

El bloque CustomVideoEncoder proporciona un marco flexible para integrar codificadores de video de terceros o especializados que no están directamente soportados por el SDK. Esto permite la integración de códecs propietarios, codificadores experimentales o soluciones de codificación específicas de plataforma.

### Información del bloque

Nombre: CustomVideoEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | Video comprimido | 1

### Configuraciones

#### CustomVideoEncoderSettings

```csharp
public class CustomVideoEncoderSettings
{
    // Nombre del elemento codificador GStreamer (ej. "x264enc", "nvh264enc")
    public string EncoderName { get; set; }
    
    // Diccionario de propiedades específicas del codificador
    public Dictionary<string, object> Properties { get; set; }
}
```

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->CustomVideoEncoderBlock;
    CustomVideoEncoderBlock-->MP4SinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

// Configurar codificador personalizado (ejemplo con x264enc)
var customSettings = new CustomVideoEncoderSettings
{
    EncoderName = "x264enc",
    Properties = new Dictionary<string, object>
    {
        { "bitrate", 2000 },
        { "speed-preset", "medium" }
    }
};
var customEncoder = new CustomVideoEncoderBlock(customSettings);
pipeline.Connect(fileSource.VideoOutput, customEncoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(customEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux (requiere plugins GStreamer apropiados).

## Codificador GIF

El bloque codificador GIF crea imágenes GIF animadas a partir de flujos de video, adecuado para contenido web, redes sociales y documentación.

### Información del bloque

Nombre: GIFEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | GIF | 1

### Configuraciones

#### GIFEncoderSettings

```csharp
public class GIFEncoderSettings
{
    // Tasa de fotogramas para el GIF de salida
    public VideoFrameRate FrameRate { get; set; }
    
    // Habilitar tramado para mejor representación de color
    public bool Dither { get; set; }
}
```

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->GIFEncoderBlock;
    GIFEncoderBlock-->FileSink;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var gifSettings = new GIFEncoderSettings
{
    FrameRate = new VideoFrameRate(10, 1), // 10 fps para tamaño de archivo más pequeño
    Dither = true
};
var gifEncoder = new GIFEncoderBlock(gifSettings, "output.gif");
pipeline.Connect(fileSource.VideoOutput, gifEncoder.Input);

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador H264

El bloque codificador H264 se usa para codificar archivos en MP4, MKV y algunos otros formatos, así como para streaming de red usando RTSP y HLS.

Usa clases que implementan la interfaz IH264EncoderSettings para establecer los parámetros.

### Configuraciones

#### NVENCH264EncoderSettings

Codificador de video H264 GPUs Nvidia.

**Plataformas:** Windows, Linux, macOS.

#### AMFHEVCEncoderSettings

Codificador de video H264 GPUs AMD/ATI.

**Plataformas:** Windows, Linux, macOS.

#### QSVH264EncoderSettings

Codificador de video H264 GPU Intel.

**Plataformas:** Windows, Linux, macOS.

#### OpenH264EncoderSettings

Codificador H264 CPU software.

**Plataformas:** Windows, macOS, Linux, iOS, Android.

#### CustomH264EncoderSettings

Permite usar un elemento GStreamer personalizado para codificación H264. Puedes especificar el nombre del elemento GStreamer y configurar sus propiedades.

**Plataformas:** Windows, Linux, macOS.

### Información del bloque

Nombre: H264EncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
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

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var h264EncoderBlock = new H264EncoderBlock(new NVENCH264EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Aplicaciones de muestra

- [Demo de Captura Simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)
- [Demo de Captura de Pantalla](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Screen%20Capture)

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador HEVC/H265

El codificador HEVC se usa para codificar archivos en MP4, MKV y algunos otros formatos, así como para streaming de red usando RTSP y HLS.

Usa clases que implementan la interfaz IHEVCEncoderSettings para establecer los parámetros.

### Configuraciones

#### MFHEVCEncoderSettings

Codificador HEVC Microsoft Media Foundation. Codificador CPU.

**Plataformas:** Windows.

#### NVENCHEVCEncoderSettings

Codificador de video HEVC GPUs Nvidia.

**Plataformas:** Windows, Linux, macOS.

#### AMFHEVCEncoderSettings

Codificador de video HEVC GPUs AMD/ATI.

**Plataformas:** Windows, Linux, macOS.

#### QSVHEVCEncoderSettings

Codificador de video HEVC GPU Intel.

**Plataformas:** Windows, Linux, macOS.

#### CustomHEVCEncoderSettings

Permite usar un elemento GStreamer personalizado para codificación HEVC. Puedes especificar el nombre del elemento GStreamer y configurar sus propiedades.

**Plataformas:** Windows, Linux, macOS.

### Información del bloque

Nombre: HEVCEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
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

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var hevcEncoderBlock = new HEVCEncoderBlock(new NVENCHEVCEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, hevcEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(hevcEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador MJPEG

`MJPEG (Motion JPEG)`: Un formato de compresión de video donde cada fotograma de video se comprime por separado en una imagen JPEG. Esta técnica es directa y resulta en ninguna compresión inter-frame, haciéndola ideal para situaciones donde se requiere edición o acceso específico de fotograma, como en vigilancia e imágenes médicas.
Usa clases que implementan la interfaz IH264EncoderSettings para establecer los parámetros.

### Configuraciones

#### MJPEGEncoderSettings

Codificador MJPEG predeterminado. Codificador CPU.

- **Propiedades clave**:
  - `Quality` (int): Nivel de calidad JPEG (10-100). Predeterminado es `85`.
- **Tipo de codificador**: `MJPEGEncoderType.CPU`.

**Plataformas:** Windows, Linux, macOS, iOS, Android.

#### QSVMJPEGEncoderSettings

Codificador MJPEG GPUs Intel.

- **Propiedades clave**:
  - `Quality` (uint): Nivel de calidad JPEG (10-100). Predeterminado es `85`.
- **Tipo de codificador**: `MJPEGEncoderType.GPU_Intel_QSV_MJPEG`.

**Plataformas:** Windows, Linux, macOS.

#### Enum MJPEGEncoderType

Especifica el tipo de codificador MJPEG.

- `CPU`: Codificador basado en CPU predeterminado.
- `GPU_Intel_QSV_MJPEG`: Codificador MJPEG basado en GPU Intel QuickSync.

### Información del bloque

Nombre: MJPEGEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | MJPEG | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->MJPEGEncoderBlock;
    MJPEGEncoderBlock-->AVISinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new MJPEGEncoderBlock(new MJPEGEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var aviSinkBlock = new AVISinkBlock(new AVISinkSettings(@"output.avi"));
pipeline.Connect(videoEncoderBlock.Output, aviSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador Theora

El codificador [Theora](https://www.theora.org/) se usa para codificar archivos de video en formato WebM.

### Configuraciones

#### TheoraEncoderSettings

Proporciona configuraciones para el codificador Theora.

- **Propiedades clave**:
  - `Bitrate` (kbps)
  - `CapOverflow`, `CapUnderflow` (limitación de depósito de bits)
  - `DropFrames` (permitir/no permitir eliminación de fotogramas)
  - `KeyFrameAuto` (detección automática de fotograma clave)
  - `KeyFrameForce` (intervalo para forzar fotograma clave cada N fotogramas)
  - `KeyFrameFrequency` (frecuencia de fotograma clave)
  - `MultipassCacheFile` (ruta de cadena para caché multipaso)
  - `MultipassMode` (usando enum `TheoraMultipassMode`: `SinglePass`, `FirstPass`, `SecondPass`)
  - `Quality` (valor entero, típicamente 0-63 para libtheora, el significado puede variar)
  - `RateBuffer` (tamaño del búfer de control de tasa en unidades de fotogramas, 0 = auto)
  - `SpeedLevel` (cantidad de búsqueda de vector de movimiento, 0-2 o superior dependiendo de la implementación)
  - `VP3Compatible` (booleano para habilitar compatibilidad VP3)
- **Disponibilidad**: Puede verificarse usando `TheoraEncoderSettings.IsAvailable()`.

### Información del bloque

Nombre: TheoraEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | video/x-theora | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->TheoraEncoderBlock;
    TheoraEncoderBlock-->WebMSinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var theoraEncoderBlock = new TheoraEncoderBlock(new TheoraEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, theoraEncoderBlock.Input);

var webmSinkBlock = new WebMSinkBlock(new WebMSinkSettings(@"output.webm"));
pipeline.Connect(theoraEncoderBlock.Output, webmSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador VPX

El bloque codificador VPX se usa para codificar archivos en WebM, MKV o archivos OGG. El codificador VPX es un conjunto de códecs de video para codificación en formatos VP8 y VP9.

El bloque codificador VPX utiliza clases de configuraciones que implementan la interfaz `IVPXEncoderSettings`. Las clases de configuraciones clave incluyen:

### Configuraciones

La clase base común para configuraciones de codificador CPU VP8 y VP9 es `VPXEncoderSettings`. Proporciona una amplia gama de propiedades compartidas para ajustar el proceso de codificación, como:

- `ARNRMaxFrames`, `ARNRStrength`, `ARNRType` (reducción de ruido AltRef)
- `BufferInitialSize`, `BufferOptimalSize`, `BufferSize` (configuraciones de búfer del cliente)
- `CPUUsed`, `CQLevel` (calidad restringida)
- `Deadline` (plazo de codificación por fotograma)
- `DropframeThreshold`
- `RateControl` (usando enum `VPXRateControl`)
- `ErrorResilient` (usando enum `VPXErrorResilientFlags`)
- `HorizontalScalingMode`, `VerticalScalingMode` (usando enum `VPXScalingMode`)
- `KeyFrameMaxDistance`, `KeyFrameMode` (usando enum `VPXKeyFrameMode`)
- `MinQuantizer`, `MaxQuantizer`
- `MultipassCacheFile`, `MultipassMode` (usando enum `VPXMultipassMode`)
- `NoiseSensitivity`
- `TargetBitrate` (en Kbits/s)
- `NumOfThreads`
- `TokenPartitions` (usando enum `VPXTokenPartitions`)
- `Tuning` (usando enum `VPXTuning`)

#### VP8EncoderSettings

Codificador CPU para VP8. Hereda de `VPXEncoderSettings`.

- **Propiedades clave**: Aprovecha propiedades de `VPXEncoderSettings` adaptadas para VP8.
- **Tipo de codificador**: `VPXEncoderType.VP8`.
- **Disponibilidad**: Puede verificarse usando `VP8EncoderSettings.IsAvailable()`.

#### VP9EncoderSettings

Codificador CPU para VP9. Hereda de `VPXEncoderSettings`.

- **Propiedades clave**: Además de las propiedades `VPXEncoderSettings`, incluye configuraciones específicas de VP9:
  - `AQMode` (modo de cuantización adaptativa, usando enum `VPXAdaptiveQuantizationMode`)
  - `FrameParallelDecoding` (permitir procesamiento paralelo)
  - `RowMultithread` (codificación de fila multi-hilo)
  - `TileColumns`, `TileRows` (valores log2)
- **Tipo de codificador**: `VPXEncoderType.VP9`.
- **Disponibilidad**: Puede verificarse usando `VP9EncoderSettings.IsAvailable()`.

#### QSVVP9EncoderSettings

Codificador Intel QSV (acelerado por GPU) para VP9.

- **Propiedades clave**:
  - `LowLatency`
  - `TargetUsage` (1: Mejor calidad, 4: Equilibrado, 7: Mejor velocidad)
  - `Bitrate` (Kbit/seg)
  - `GOPSize`
  - `ICQQuality` (Calidad Constante Inteligente)
  - `MaxBitrate` (Kbit/seg)
  - `QPI`, `QPP` (cuantizador constante para fotogramas I y P)
  - `Profile` (0-3)
  - `RateControl` (usando enum `QSVVP9EncRateControl`)
  - `RefFrames`
- **Tipo de codificador**: `VPXEncoderType.QSV_VP9`.
- **Disponibilidad**: Puede verificarse usando `QSVVP9EncoderSettings.IsAvailable()`.

#### CustomVPXEncoderSettings

Permite usar un elemento GStreamer personalizado para codificación VPX.

- **Propiedades clave**:
  - `ElementName` (cadena para especificar el nombre del elemento GStreamer)
  - `Properties` (Dictionary<string, object> para configurar el elemento)
  - `VideoFormat` (formato de video requerido como `VideoFormatX.NV12`)
- **Tipo de codificador**: `VPXEncoderType.CustomEncoder`.

### Enumeraciones VPX

Varias enumeraciones están disponibles para configurar codificadores VPX:

- `VPXAdaptiveQuantizationMode`: Define modos de cuantización adaptativa (ej. `Off`, `Variance`, `Complexity`, `CyclicRefresh`, `Equator360`, `Perceptual`, `PSNR`, `Lookahead`).
- `VPXErrorResilientFlags`: Banderas para características de resiliencia a errores (ej. `None`, `Default`, `Partitions`).
- `VPXKeyFrameMode`: Define estrategias de colocación de fotograma clave (ej. `Auto`, `Disabled`).
- `VPXMultipassMode`: Modos para codificación multipaso (ej. `OnePass`, `FirstPass`, `LastPass`).
- `VPXRateControl`: Modos de control de tasa (ej. `VBR`, `CBR`, `CQ`).
- `VPXScalingMode`: Modos de escalado (ej. `Normal`, `_4_5`, `_3_5`, `_1_2`).
- `VPXTokenPartitions`: Número de particiones de token (ej. `One`, `Two`, `Four`, `Eight`).
- `VPXTuning`: Opciones de ajuste para el codificador (ej. `PSNR`, `SSIM`).
- `VPXEncoderType`: Especifica la variante del codificador VPX (ej. `VP8`, `VP9`, `QSV_VP9`, `CustomEncoder`, y específicos de plataforma como `OMXExynosVP8Encoder`).
- `QSVVP9EncRateControl`: Modos de control de tasa específicos para `QSVVP9EncoderSettings` (ej. `CBR`, `VBR`, `CQP`, `ICQ`).

### Información del bloque

Nombre: VPXEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | VP8/VP9 | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->VPXEncoderBlock;
    VPXEncoderBlock-->WebMSinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var vp8EncoderBlock = new VPXEncoderBlock(new VP8EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, vp8EncoderBlock.Input);

var webmSinkBlock = new WebMSinkBlock(new WebMSinkSettings(@"output.webm"));
pipeline.Connect(vp8EncoderBlock.Output, webmSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador MPEG2

`MPEG-2`: Un estándar ampliamente usado para compresión de video y audio, comúnmente encontrado en DVDs, transmisiones de televisión digital (como DVB y ATSC), y SVCDs. Ofrece buena calidad a tasas de bits relativamente bajas para contenido de definición estándar.

### Información del bloque

Nombre: MPEG2EncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | video/mpeg | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->MPEG2EncoderBlock;
    MPEG2EncoderBlock-->MPEGTSSinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var mpeg2EncoderBlock = new MPEG2EncoderBlock(new MPEG2VideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, mpeg2EncoderBlock.Input);

// Ejemplo: Usando un MPGSinkBlock para archivos .mpg o .ts
var mpgSinkBlock = new MPGSinkBlock(new MPGSinkSettings(@"output.mpg"));
pipeline.Connect(mpeg2EncoderBlock.Output, mpgSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux.

## Codificador MPEG4

`MPEG-4 Part 2 Visual` (a menudo referido simplemente como video MPEG-4) es un estándar de compresión de video que es parte del conjunto MPEG-4. Se usa en varias aplicaciones, incluyendo streaming de video, videoconferencia y discos ópticos como DivX y Xvid.

### Información del bloque

Nombre: MPEG4EncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | video/mpeg, mpegversion=4 | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->MPEG4EncoderBlock;
    MPEG4EncoderBlock-->MP4SinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4"; // Archivo de entrada
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var mpeg4EncoderBlock = new MPEG4EncoderBlock(new MPEG4VideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, mpeg4EncoderBlock.Input);

// Ejemplo: Usando un MP4SinkBlock para archivos .mp4
var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output_mpeg4.mp4"));
pipeline.Connect(mpeg4EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux.

## Codificador PNG

El bloque codificador PNG proporciona compresión de imagen sin pérdida con alta calidad, adecuado para archivo, capturas de pantalla y aplicaciones que requieren calidad de imagen perfecta con soporte de transparencia.

### Información del bloque

Nombre: PNGEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | PNG | 1

### Configuraciones

#### PNGEncoderSettings

```csharp
public class PNGEncoderSettings
{
    // Nivel de compresión (0-9, mayor = mejor compresión pero más lento)
    public int CompressionLevel { get; set; }
    
    // Tipo de filtro PNG para compresión óptima
    public PNGFilterType FilterType { get; set; }
}
```

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->PNGEncoderBlock;
    PNGEncoderBlock-->FileSink;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4"; // Archivo de entrada
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var pngSettings = new PNGEncoderSettings
{
    CompressionLevel = 6 // Compresión equilibrada
};
var pngEncoder = new PNGEncoderBlock(pngSettings, "frame.png");
pipeline.Connect(fileSource.VideoOutput, pngEncoder.Input);

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux, iOS, Android.

## Codificador Apple ProRes

`Apple ProRes`: Un formato de compresión de video de alta calidad con pérdida desarrollado por Apple Inc., ampliamente usado en producción y post-producción de video profesional por su excelente equilibrio de calidad de imagen y rendimiento.

### Información del bloque

Nombre: AppleProResEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | ProRes | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->AppleProResEncoderBlock;
    AppleProResEncoderBlock-->MOVSinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var proResEncoderBlock = new AppleProResEncoderBlock(new AppleProResEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, proResEncoderBlock.Input);

var movSinkBlock = new MOVSinkBlock(new MOVSinkSettings(@"output.mov"));
pipeline.Connect(proResEncoderBlock.Output, movSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

macOS, iOS.

### Disponibilidad

Puedes verificar si el codificador Apple ProRes está disponible en tu entorno usando:

```csharp
bool available = AppleProResEncoderBlock.IsAvailable();
```

## Codificador WMV

### Resumen

El bloque codificador WMV codifica video en formato WMV.

### Información del bloque

Nombre: WMVEncoderBlock.

Dirección del pin | Tipo de medio | Conteo de pines
--- | :---: | :---:
Entrada | Video sin comprimir | 1
Salida | video/x-wmv | 1

### El pipeline de muestra

```mermaid
graph LR;
    UniversalSourceBlock-->WMVEncoderBlock;
    WMVEncoderBlock-->ASFSinkBlock;
```

### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline(false);

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var wmvEncoderBlock = new WMVEncoderBlock(new WMVEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, wmvEncoderBlock.Input);

var asfSinkBlock = new ASFSinkBlock(new ASFSinkSettings(@"output.wmv"));
pipeline.Connect(wmvEncoderBlock.Output, asfSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plataformas

Windows, macOS, Linux.

## Consideraciones Generales de Configuraciones de Video

Mientras que las clases de configuraciones de codificador específicas proporcionan control detallado, algunos conceptos generales o enumeraciones podrían ser relevantes a través de diferentes codificadores o para entender opciones de calidad de video.