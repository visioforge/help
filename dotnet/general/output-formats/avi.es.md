---
title: Salida de Archivo AVI en SDKs .NET
description: Implemente salida de archivo AVI en .NET con codificación de video y audio, aceleración de hardware y soporte de contenedor multimedia multiplataforma.
---

# Salida de Archivo AVI en los SDK .NET de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

AVI (Audio Video Interleave) es un formato de contenedor multimedia desarrollado por Microsoft que almacena tanto datos de audio como de video en un solo archivo con reproducción sincronizada. Soporta tanto datos comprimidos como sin comprimir, ofreciendo flexibilidad aunque a veces resulta en tamaños de archivo más grandes.

## Descripción técnica del formato AVI

Los archivos AVI usan una estructura RIFF (Resource Interchange File Format) para organizar datos. Este formato divide el contenido en fragmentos, con cada fragmento conteniendo cuadros de audio o video. Los aspectos técnicos clave incluyen:

- Formato de contenedor que soporta múltiples códecs de audio y video
- Datos de audio y video intercalados para reproducción sincronizada
- Tamaño máximo de archivo de 4GB en AVI estándar (extendido a 16EB en OpenDML AVI)
- Soporte para múltiples pistas de audio y subtítulos
- Ampliamente soportado en plataformas y reproductores de medios

A pesar de que formatos de contenedor más nuevos como MP4 y MKV ofrecen más características, AVI sigue siendo valioso para ciertos flujos de trabajo debido a su simplicidad y compatibilidad con sistemas heredados.

## Implementación AVI multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La clase [AVIOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.AVIOutput.html) proporciona una forma robusta de configurar y generar archivos AVI con varias opciones de codificación.

### Configuración de salida AVI

Cree una instancia de `AVIOutput` especificando un nombre de archivo de destino:

```csharp
var aviOutput = new AVIOutput("video_salida.avi");
```

Este constructor inicializa automáticamente los codificadores predeterminados:

- Video: Codificador OpenH264
- Audio: Codificador MP3

### Opciones de codificador de video

Configure la codificación de video a través de la propiedad `Video` con varios codificadores disponibles. Para opciones de configuración detalladas, consulte la [documentación del codificador H.264](../video-encoders/h264.md), la [documentación del codificador HEVC](../video-encoders/hevc.md) y la [documentación del codificador MJPEG](../video-encoders/mjpeg.md):

#### Codificador estándar

```csharp
// Codificador H.264 de código abierto para uso general
aviOutput.Video = new OpenH264EncoderSettings();
```

#### Codificadores acelerados por hardware

```csharp
// Aceleración GPU NVIDIA
aviOutput.Video = new NVENCH264EncoderSettings();  // H.264
aviOutput.Video = new NVENCHEVCEncoderSettings(); // HEVC

// Aceleración Intel Quick Sync
aviOutput.Video = new QSVH264EncoderSettings();   // H.264
aviOutput.Video = new QSVHEVCEncoderSettings();   // HEVC

// Aceleración GPU AMD
aviOutput.Video = new AMFH264EncoderSettings();   // H.264
aviOutput.Video = new AMFHEVCEncoderSettings();   // HEVC
```

#### Codificador de propósito especial

```csharp
// Motion JPEG para codificación cuadro por cuadro de alta calidad
aviOutput.Video = new MJPEGEncoderSettings();
```

### Opciones de codificador de audio

La propiedad `Audio` le permite configurar los ajustes de codificación de audio. Para opciones de configuración detalladas, consulte la [documentación del codificador MP3](../audio-encoders/mp3.md) y la [documentación del codificador AAC](../audio-encoders/aac.md):

```csharp
// Codificación MP3 estándar
aviOutput.Audio = new MP3EncoderSettings();

// Opciones de codificación AAC
aviOutput.Audio = new VOAACEncoderSettings();
aviOutput.Audio = new AVENCAACEncoderSettings();
aviOutput.Audio = new MFAACEncoderSettings(); // Solo Windows
```

### Integración con componentes del SDK

#### Video Capture SDK

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(aviOutput, true);
```

#### Video Edit SDK

```csharp
var core = new VideoEditCoreX();
core.Output_Format = aviOutput;
```

#### Media Blocks SDK

```csharp
var aac = new VOAACEncoderSettings();
var h264 = new OpenH264EncoderSettings();
var aviSinkSettings = new AVISinkSettings("output.avi");
var aviOutput = new AVIOutputBlock(aviSinkSettings, h264, aac);
```

### Gestión de archivos

Puede obtener o cambiar el nombre de archivo de salida después de la inicialización:

```csharp
// Obtener nombre de archivo actual
string archivoActual = aviOutput.GetFilename();

// Establecer nuevo nombre de archivo
aviOutput.SetFilename("nueva_salida.avi");
```

### Ejemplo completo

Aquí hay un ejemplo completo que muestra cómo configurar salida AVI con aceleración de hardware:

```csharp
// Crear salida AVI con nombre de archivo especificado
var aviOutput = new AVIOutput("salida_alta_calidad.avi");

// Configurar codificación H.264 acelerada por NVIDIA
aviOutput.Video = new NVENCH264EncoderSettings();

// Configurar codificación de audio AAC
aviOutput.Audio = new VOAACEncoderSettings();
```

## Implementación AVI específica de Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Los componentes solo Windows proporcionan opciones adicionales para configuración de salida AVI.

### Configuración básica

Cree el objeto AVIOutput:

```csharp
var aviOutput = new AVIOutput();
```

### Métodos de configuración

#### Método 1: Usando diálogo de configuración

```csharp
var aviSettingsDialog = new AVISettingsDialog(
  VideoCapture1.Video_Codecs.ToArray(),
  VideoCapture1.Audio_Codecs.ToArray());

aviSettingsDialog.ShowDialog(this);
aviSettingsDialog.SaveSettings(ref aviOutput);
```

#### Método 2: Configuración programática

Primero, obtenga los códecs disponibles:

```csharp
// Llenar listas de códecs
foreach (string codec in VideoCapture1.Video_Codecs)
{
  cbVideoCodecs.Items.Add(codec);
}

foreach (string codec in VideoCapture1.Audio_Codecs)
{
  cbAudioCodecs.Items.Add(codec);
}
```

Luego establezca la configuración de video y audio:

```csharp
// Configurar video
aviOutput.Video_Codec = cbVideoCodecs.Text;

// Configurar audio
aviOutput.ACM.Name = cbAudioCodecs.Text;
aviOutput.ACM.Channels = 2;
aviOutput.ACM.BPS = 16;
aviOutput.ACM.SampleRate = 44100;
aviOutput.ACM.UseCompression = true;
```

### Implementación

Aplique la configuración e inicie la captura:

```csharp
// Establecer formato de salida
VideoCapture1.Output_Format = aviOutput;

// Establecer modo de captura
VideoCapture1.Mode = VideoCaptureMode.VideoCapture;

// Establecer ruta del archivo de salida
VideoCapture1.Output_Filename = "output.avi";

// Iniciar captura
await VideoCapture1.StartAsync();
```

## Mejores prácticas para salida AVI

### Guías de selección de codificador

1. **Aplicaciones de propósito general**
   - OpenH264 proporciona buena compatibilidad y calidad
   - Adecuado para la mayoría de escenarios de desarrollo estándar

2. **Aplicaciones críticas en rendimiento**
   - Use codificadores acelerados por hardware (NVENC, QSV, AMF) cuando estén disponibles
   - Ofrece ventajas significativas de rendimiento con pérdida mínima de calidad

3. **Aplicaciones enfocadas en calidad**
   - Los codificadores HEVC proporcionan mejor compresión a calidad similar
   - MJPEG para escenarios que requieren precisión cuadro por cuadro

### Recomendaciones de codificación de audio

- MP3: Buena compatibilidad con calidad razonable
- AAC: Mejor relación calidad-tamaño, preferido para aplicaciones más nuevas
- Elija según su plataforma objetivo y requisitos de calidad

### Consideraciones de plataforma

- Algunos codificadores son específicos de plataforma:
  - Los codificadores MF HEVC y MF AAC son solo Windows
  - Los codificadores acelerados por hardware requieren soporte de GPU apropiado

- Verifique la disponibilidad del codificador con `GetVideoEncoders()` y `GetAudioEncoders()` al desarrollar aplicaciones multiplataforma

### Consejos de manejo de errores

- Siempre verifique la disponibilidad del codificador antes de usar
- Implemente codificadores de respaldo para escenarios específicos de plataforma
- Verifique permisos de escritura de archivo antes de establecer rutas de salida

## Solución de problemas comunes

### Códec no encontrado

Si encuentra errores de "Códec no encontrado":

```csharp
// Verificar si el códec está disponible antes de usar
if (!VideoCapture1.Video_Codecs.Contains("H264"))
{
    // Recurrir a otro códec o mostrar error
    MessageBox.Show("Códec H264 no disponible. Por favor instale los códecs requeridos.");
    return;
}
```

### Problemas de permisos de escritura de archivo

Maneje errores relacionados con permisos:

```csharp
try
{
    // Probar permisos de escritura
    using (var fs = File.Create(rutaSalida, 1, FileOptions.DeleteOnClose)) { }
    
    // Si es exitoso, proceder con salida AVI
    aviOutput.SetFilename(rutaSalida);
}
catch (UnauthorizedAccessException)
{
    // Manejar error de permisos
    MessageBox.Show("No se puede escribir en la ubicación especificada. Por favor seleccione otra carpeta.");
}
```

### Problemas de memoria con archivos grandes

Para manejar grabación de archivos grandes:

```csharp
// Dividir grabación en múltiples archivos cuando se alcanza el límite de tamaño
void ConfigurarGrabacionArchivoGrande()
{
    var aviOutput = new AVIOutput("grabacion_parte1.avi");
    
    // Establecer límite de tamaño de archivo (3.5GB para mantenerse bajo el límite de 4GB de AVI)
    aviOutput.MaxFileSize = 3.5 * 1024 * 1024 * 1024;
    
    // Habilitar funcionalidad de división automática
    aviOutput.AutoSplit = true;
    aviOutput.SplitNamingPattern = "grabacion_parte{0}.avi";
    
    // Aplicar a Video Capture
    var core = new VideoCaptureCoreX();
    core.Outputs_Add(aviOutput, true);
}
```

## Dependencias requeridas

### Video Capture SDK .Net

- [Redist x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- [Redist x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Video Edit SDK .Net

- [Redist x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
- [Redist x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Recursos adicionales

- [Documentación API de VisioForge](https://api.visioforge.org/dotnet/)
- [Repositorio de proyectos de ejemplo](https://github.com/visioforge/.Net-SDK-s-samples)
- [Foros de soporte y comunidad](https://support.visioforge.com/)
