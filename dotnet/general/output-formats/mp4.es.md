---
title: Salida MP4 en C# .NET con H.264, HEVC, AAC y Muxing
description: Selecciona NVENC, QSV o AMF con fallback automático. Divide por tamaño, duración o timecode, y hace muxing de streams en apps .NET.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Recording
  - Encoding
  - Editing
  - Webcam
  - MP4
  - H.264
  - H.265
  - MPEG-2
  - AAC
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - MP4Output
  - MP4SplitSinkSettings
  - NVENCH264EncoderSettings
  - VOAACEncoderSettings
  - MP4OutputBlock
---

# Salida de archivo MP4

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

MP4 (MPEG-4 Part 14), introducido en 2001, es un formato de contenedor multimedia digital más comúnmente usado para almacenar video y audio. También soporta subtítulos e imágenes. MP4 es conocido por su alta compresión y compatibilidad entre varios dispositivos y plataformas, haciéndolo una opción popular para streaming y compartir.

Capturar videos desde una webcam y guardarlos en un archivo es un requisito común en muchas aplicaciones. Una forma de lograr esto es usando un kit de desarrollo de software (SDK) como VisioForge Video Capture SDK .Net, que proporciona una API fácil de usar para capturar y procesar videos en C#.

Para capturar video en formato MP4 usando Video Capture SDK, necesita configurar el formato de salida de video usando una de las clases para salida MP4. Puede usar varios codificadores de video de software y hardware disponibles, incluyendo Intel QuickSync, Nvidia NVENC y AMD/ATI APU.

## Salida MP4 multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La clase [MP4Output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP4Output.html?q=MP4Output) proporciona una forma flexible y potente de configurar ajustes de salida de video MP4 para operaciones de captura y edición de video. Esta guía le llevará a través de cómo usar la clase MP4Output efectivamente, cubriendo sus características principales y patrones de uso comunes.

MP4Output implementa varias interfaces importantes:

- IVideoEditXBaseOutput
- IVideoCaptureXBaseOutput
- Creación de Media Block

Esto lo hace adecuado tanto para escenarios de edición como de captura de video mientras proporciona control extenso sobre el procesamiento de video y audio.

### Uso básico

La forma más simple de crear una instancia MP4Output es usando el constructor con un nombre de archivo:

```csharp
var output = new MP4Output("output.mp4");
```

Esto crea un MP4Output con configuración predeterminada de codificador de video y audio. En Windows, usará OpenH264 para codificación de video y Media Foundation AAC para codificación de audio por defecto.

### Configuración del codificador de video

La clase MP4Output soporta múltiples codificadores de video a través de su propiedad `Video`. Aquí están los codificadores de video soportados:

**[Codificadores H.264](../video-encoders/h264.md)**

- OpenH264EncoderSettings (Predeterminado, CPU)
- AMFH264EncoderSettings (AMD)
- NVENCH264EncoderSettings (NVIDIA)
- QSVH264EncoderSettings (Intel Quick Sync)

**[Codificadores HEVC (H.265)](../video-encoders/hevc.md)**

- MFHEVCEncoderSettings (Solo Windows)
- AMFHEVCEncoderSettings (AMD)
- NVENCHEVCEncoderSettings (NVIDIA)
- QSVHEVCEncoderSettings (Intel Quick Sync)

Puede verificar la disponibilidad de codificadores específicos usando el método `IsAvailable`:

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
```

Ejemplo de configuración de un codificador de video específico:

```csharp
var output = new MP4Output("output.mp4");
output.Video = new NVENCH264EncoderSettings(); // Usar codificador NVIDIA
```

### Configuración del codificador de audio

La propiedad `Audio` le permite especificar el codificador de audio. Los codificadores de audio soportados incluyen:

- [VOAACEncoderSettings](../audio-encoders/aac.md)
- [AVENCAACEncoderSettings](../audio-encoders/aac.md)
- [MFAACEncoderSettings](../audio-encoders/aac.md) (Solo Windows)
- [MP3EncoderSettings](../audio-encoders/mp3.md)

Ejemplo de configuración de un codificador de audio personalizado:

```csharp
var output = new MP4Output("output.mp4");
output.Audio = new MP3EncoderSettings();
```

La clase MP4Output selecciona automáticamente codificadores predeterminados apropiados basándose en la plataforma.

### Código de ejemplo

Agregar la salida MP4 a la instancia del núcleo Video Capture SDK:

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(output, true);
```

Establecer el formato de salida para la instancia del núcleo Video Edit SDK:

```csharp
var core = new VideoEditCoreX();
core.Output_Format = output;
```

Crear una instancia de salida MP4 de Media Blocks:

```csharp
var aac = new VOAACEncoderSettings();
var h264 = new OpenH264EncoderSettings();
var mp4SinkSettings = new MP4SinkSettings("output.mp4");
var mp4Output = new MP4OutputBlock(mp4SinkSettings, h264, aac);
```

### División de archivos

La clase [MP4SplitSinkSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Sinks.MP4SplitSinkSettings.html) proporciona capacidades de división automática de archivos, permitiéndole dividir la salida MP4 en múltiples archivos según el tamaño, la duración o el código de tiempo. Esta clase se puede usar tanto con `MP4OutputBlock` (que incluye codificación) como con `MP4SinkBlock` (solo multiplexado). Esto es particularmente útil para:

- Sesiones de grabación largas que necesitan dividirse en tamaños de archivo manejables
- Crear segmentos basados en tiempo para facilitar el archivo o distribución
- Gestionar el espacio en disco limitando el número de archivos mantenidos en almacenamiento
- Implementar grabación de búfer rotativo donde solo se conservan los archivos recientes

**Dividir por tamaño de archivo**

Divide la salida cuando un archivo alcanza un tamaño específico en bytes:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Crear configuración de división con patrón de nombre de archivo
var splitSettings = new MP4SplitSinkSettings("grabacion_%05d.mp4");

// Dividir cuando el archivo alcance 100 MB (104857600 bytes)
splitSettings.SplitFileSize = 104857600;

// Deshabilitar división basada en duración (predeterminado es 1 minuto)
splitSettings.SplitDuration = TimeSpan.Zero;

// Crear bloque de salida
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Dividir por duración**

Divide la salida cuando un archivo alcanza una duración específica:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Crear configuración de división con patrón de nombre de archivo
var splitSettings = new MP4SplitSinkSettings("grabacion_%05d.mp4");

// Dividir cada 5 minutos
splitSettings.SplitDuration = TimeSpan.FromMinutes(5);

// Crear bloque de salida
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Limitar archivos máximos**

Controla el número máximo de archivos a mantener en disco. Una vez alcanzado el límite, los archivos más antiguos se eliminan automáticamente:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Crear configuración de división
var splitSettings = new MP4SplitSinkSettings("grabacion_%05d.mp4");

// Dividir cada 10 minutos
splitSettings.SplitDuration = TimeSpan.FromMinutes(10);

// Mantener solo los últimos 6 archivos (1 hora de grabaciones)
splitSettings.SplitMaxFiles = 6;

// Crear bloque de salida
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Dividir por código de tiempo**

Divide la salida basándose en la diferencia de código de tiempo entre el primer y último fotograma:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Crear configuración de división
var splitSettings = new MP4SplitSinkSettings("grabacion_%05d.mp4");

// Dividir cuando la diferencia de código de tiempo alcance 1 hora
// Formato: "HH:MM:SS:FF" donde FF son fotogramas
splitSettings.SplitMaxSizeTimecode = "01:00:00:00";

// Deshabilitar otros métodos de división
splitSettings.SplitFileSize = 0;
splitSettings.SplitDuration = TimeSpan.Zero;

// Crear bloque de salida
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Configuraciones combinadas**

Puede combinar criterios de división. El archivo se dividirá cuando se cumpla cualquiera de los criterios habilitados:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Crear configuración de división
var splitSettings = new MP4SplitSinkSettings("grabacion_%05d.mp4");

// Dividir a 200 MB O 10 minutos, lo que ocurra primero
splitSettings.SplitFileSize = 209715200; // 200 MB
splitSettings.SplitDuration = TimeSpan.FromMinutes(10);

// Mantener solo los últimos 12 archivos
splitSettings.SplitMaxFiles = 12;

// Comenzar numeración desde 1 en lugar de 0
splitSettings.StartIndex = 1;

// Crear bloque de salida
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Uso con MP4SinkBlock**

Para escenarios donde ya tiene flujos codificados y solo necesita multiplexado (creación de contenedor), use `MP4SinkBlock` con `MP4SplitSinkSettings`:

```csharp
// Crear configuración de división
var splitSettings = new MP4SplitSinkSettings("salida_%05d.mp4");
splitSettings.SplitDuration = TimeSpan.FromMinutes(5);

// Crear bloque de receptor MP4 (solo multiplexado, sin codificación)
var mp4Sink = new MP4SinkBlock(splitSettings);

// Conectar sus flujos pre-codificados al bloque receptor
// (la lógica de conexión depende de la estructura de su pipeline)
```

**Notas importantes:**

- El parámetro de nombre de archivo debe incluir un especificador de formato (como `%05d`) para el índice del archivo
- Establezca `SplitFileSize` a 0 para deshabilitar la división basada en tamaño (predeterminado es 0)
- El valor predeterminado de `SplitDuration` es 1 minuto; establézcalo a `TimeSpan.Zero` para deshabilitar la división basada en duración
- Establezca `SplitMaxFiles` a 0 para mantener todos los archivos (predeterminado es 0, sin eliminación)
- Al combinar criterios, la división ocurre cuando se cumple CUALQUIER criterio
- La propiedad `StartIndex` controla el número de archivo inicial (predeterminado es 0)

### Mejores prácticas

**Aceleración de hardware**: Cuando sea posible, use codificadores acelerados por hardware (NVENC, AMF, QSV) para mejor rendimiento:

```csharp
var output = new MP4Output("output.mp4");
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
```

**Selección de codificador**: Use los métodos proporcionados para enumerar codificadores disponibles:

```csharp
var output = new MP4Output("output.mp4");
var availableVideoEncoders = output.GetVideoEncoders();
var availableAudioEncoders = output.GetAudioEncoders();
```

### Problemas comunes y soluciones

1. **Acceso a archivos**: El constructor de MP4Output intenta verificar el acceso de escritura creando e inmediatamente eliminando un archivo de prueba. Asegúrese de que la aplicación tenga permisos apropiados al directorio de salida.

2. **Disponibilidad del codificador**: Los codificadores de hardware pueden no estar disponibles en todos los sistemas. Siempre proporcione un respaldo:

```csharp
var output = new MP4Output("output.mp4");
if (!NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new OpenH264EncoderSettings(); // Recurrir al codificador de software
}
```

3. **Compatibilidad entre plataformas**: Algunos codificadores son específicos de una plataforma. Use compilación condicional o comprobaciones en tiempo de ejecución cuando apunte a varias plataformas:

```csharp
#if NET_WINDOWS
    output.Audio = new MFAACEncoderSettings();
#else
    output.Audio = new MP3EncoderSettings();
#endif
```

## Salida MP4 solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

`El mismo ejemplo de código puede usarse con Video Edit SDK .Net. Utilice la clase VideoEditCore en lugar de VideoCaptureCore.`

### Codificador de CPU o codificador GPU Intel QuickSync

Cree un objeto `MP4Output` para la salida MP4.

```cs
var mp4Output = new MP4Output();
```

Establezca el modo MP4 a `CPU_QSV`.

```cs
mp4Output.MP4Mode = MP4Mode.CPU_QSV;
```

Establezca los parámetros de video.

```cs
mp4Output.Video.Profile = H264Profile.ProfileMain; // perfil H264
mp4Output.Video.Level = H264Level.Level4; // nivel H264
mp4Output.Video.Bitrate = 2000; // tasa de bits

// parámetros opcionales
mp4Output.Video.MBEncoding = H264MBEncoding.CABAC; //CABAC / CAVLC
mp4Output.Video.BitrateAuto = false; // true para usar tasa de bits automática
mp4Output.Video.RateControl = H264RateControl.VBR; // control de tasa - CBR o VBR
```

Establezca los parámetros de audio AAC.

```cs
mp4Output.Audio_AAC.Bitrate = 192;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio_AAC.Output = AACOutput.RAW; // RAW o ADTS
mp4Output.Audio_AAC.Object = AACObject.Low; // tipo de AAC
```

### Codificador Nvidia NVENC

Cree el objeto `MP4Output` para la salida MP4.

```cs
var mp4Output = new MP4Output();
```

Establezca el modo MP4 a `NVENC`.

```cs
mp4Output.MP4Mode = MP4Mode.NVENC;
```

Establezca los parámetros de video.

```cs
mp4Output.Video_NVENC.Profile = NVENCVideoEncoderProfile.H264_Main; // perfil H264
mp4Output.Video_NVENC.Level = NVENCEncoderLevel.H264_4; // nivel H264
mp4Output.Video_NVENC.Bitrate = 2000; // tasa de bits

// parámetros opcionales
mp4Output.Video_NVENC.RateControl = NVENCRateControlMode.VBR; // control de tasa - CBR o VBR
```

Establezca los parámetros de audio.

```cs
mp4Output.Audio_AAC.Bitrate = 192;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio_AAC.Output = AACOutput.RAW; // RAW o ADTS
mp4Output.Audio_AAC.Object = AACObject.Low; // tipo de AAC
```

### Codificadores CPU/GPU

Usando la salida MP4 HW, puede usar codificadores acelerados por hardware de Intel (QuickSync), Nvidia (NVENC) y AMD/ATI.

Cree el objeto `MP4HWOutput` para la salida MP4 HW.

```cs
var mp4Output = new MP4HWOutput();
```

Obtenga los codificadores disponibles.

```cs
var availableEncoders = VideoCaptureCore.HWEncodersAvailable();
// o
var availableEncoders = VideoEditCore.HWEncodersAvailable();
```

Según los codificadores disponibles, seleccione el códec de video.

```cs
mp4Output.Video.Codec = MFVideoEncoder.MS_H264; // Microsoft H264
mp4Output.Video.Profile = MFH264Profile.Main; // perfil H264
mp4Output.Video.Level = MFH264Level.Level4; // nivel H264
mp4Output.Video.AvgBitrate = 2000; // tasa de bits

// parámetros opcionales
mp4Output.Video.CABAC = true; // CABAC / CAVLC
mp4Output.Video.RateControl = MFCommonRateControlMode.CBR; // control de tasa

// hay muchos otros parámetros disponibles
```

Establezca los parámetros de audio.

```cs
mp4Output.Audio.Bitrate = 192;
mp4Output.Audio.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio.Output = AACOutput.RAW; // RAW o ADTS
mp4Output.Audio.Object = AACObject.Low; // tipo de AAC
```

Ahora podemos aplicar la configuración de salida MP4 a la clase principal (VideoCaptureCore o VideoEditCore) e iniciar la captura o edición de video.

### Aplicar la configuración de captura de video

Establezca la configuración del formato MP4 para la salida.

```cs
core.Output_Format = mp4Output;
```

Establezca un modo de captura de video (o modo de conversión de video si usa Video Edit SDK).

```cs
core.Mode = VideoCaptureMode.VideoCapture;
```

Establezca un nombre de archivo (asegúrese de tener permisos de escritura).

```cs
core.Output_Filename = "output.mp4";
```

Inicie la captura (conversión) de video a un archivo.

```cs
await VideoCapture1.StartAsync();
```

Finalmente, cuando hayamos terminado de capturar el video, detenga el pipeline y libere los recursos. `StopAsync()` vacía el multiplexor y finaliza el archivo de salida — no se la salte o el MP4 quedará ilegible:

```cs
await VideoCapture1.StopAsync();
VideoCapture1.Dispose();
```

### Redistribuibles requeridos

- Video Capture SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
