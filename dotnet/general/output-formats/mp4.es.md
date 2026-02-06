---
title: Integración de Salida de Video MP4 para .NET
description: Genera salida MP4 en .NET con codificación hardware H.264/HEVC, configuración de audio y rendimiento optimizado para apps de video.
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
- AMFH265EncoderSettings (AMD)
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

## Salida MP4 solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

### Clase MP4Output

La clase [MP4Output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP4Output.html) es la clase principal para configurar la salida MP4.

### Propiedades

#### Video

Establezca el codificador de video a usar para el archivo MP4 de salida.

Codificadores de video disponibles:

- H264 MF
- H264 SW Nvidia
- H264 SW AMD/ATI
- H264 QSV
- H264 NVENC
- H264 AMF
- H265 SW
- H265 QSV
- H265 NVENC
- H265 AMF
- Apple ProRes (solo macOS)

#### Audio

Establezca el codificador de audio a usar para el archivo MP4 de salida. AAC es el formato recomendado.

Codificadores de audio disponibles:

- MF AAC
- voaacenc
- fdkaac enc

### Código de ejemplo

Usando Video Capture SDK para capturar video en formato MP4:

```csharp
// Crear una instancia del núcleo Video Capture SDK
var core = new VideoCaptureCore();
core.Mode = VideoCaptureMode.VideoCapture;
core.Output_Filename = "output.mp4";

// Crear una salida MP4
var mp4Output = new MP4Output();

// Seleccionar codificador H264
mp4Output.Video.Encoder = MP4VideoEncoder.H264_QSV; // Usar codificador Intel QSV

// Establecer tasa de bits de video
mp4Output.Video.Bitrate = 4000; // 4 Mbps

// Establecer codificador de audio
mp4Output.Audio.Encoder = MP4AudioEncoder.AAC_MF;
mp4Output.Audio.Bitrate = 192; // 192 Kbps

// Establecer el formato de salida
core.Output_Format = mp4Output;

// Iniciar la captura
await core.StartAsync();

// Detener después de 10 segundos
await Task.Delay(10000);

// Detener la captura
await core.StopAsync();
```

Usando Video Edit SDK para convertir video a formato MP4:

```csharp
// Crear una instancia del núcleo Video Edit SDK
var core = new VideoEditCore();

// Agregar el archivo de video fuente
var videoFile = new MediaFileSource(@"c:\samples\video.avi");
core.Input_AddVideoFile(videoFile, null, null);

// Establecer nombre de archivo de salida
core.Output_Filename = "output.mp4";

// Crear salida MP4
var mp4Output = new MP4Output();

// Seleccionar codificador H264
mp4Output.Video.Encoder = MP4VideoEncoder.H264_NVENC; // Usar codificador NVIDIA

// Establecer tasa de bits de video
mp4Output.Video.Bitrate = 5000; // 5 Mbps

// Establecer codificador de audio
mp4Output.Audio.Encoder = MP4AudioEncoder.AAC_MF;
mp4Output.Audio.Bitrate = 256; // 256 Kbps

// Establecer el formato de salida
core.Output_Format = mp4Output;

// Iniciar la edición
await core.StartAsync();
```

### Selección de la tasa de bits correcta

La tasa de bits óptima depende de su resolución y tipo de contenido:

- **720p (1280x720)**: 2-4 Mbps para contenido estándar, 4-6 Mbps para contenido de movimiento alto
- **1080p (1920x1080)**: 4-8 Mbps para contenido estándar, 8-12 Mbps para contenido de movimiento alto
- **4K (3840x2160)**: 15-25 Mbps para contenido estándar, 25-40 Mbps para contenido de movimiento alto

### Consideraciones de rendimiento

1. **Codificadores de hardware**: Los codificadores de hardware (NVENC, QSV, AMF) son significativamente más rápidos y usan menos recursos de CPU que los codificadores de software.

2. **HEVC vs H.264**: HEVC (H.265) proporciona mejor compresión pero requiere más potencia de procesamiento. Use H.264 para compatibilidad más amplia y codificación más rápida.

3. **Tasa de bits variable**: Use modos VBR para mejor relación calidad-tamaño, CBR para requisitos de tamaño de archivo predecibles o streaming.
