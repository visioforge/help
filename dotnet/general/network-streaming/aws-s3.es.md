---
title: Transmitir Video y Audio a Amazon S3 Storage
description: Implemente streaming de video y audio a AWS S3 en .NET con configuración, ajustes de codificación, manejo de errores y mejores prácticas para salida de medios.
---

# Salida AWS S3

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La funcionalidad de salida AWS S3 en los SDK de VisioForge habilita streaming directo de salida de video y audio al almacenamiento Amazon S3. Esta guía lo guiará a través de la configuración y uso de la salida AWS S3 en sus aplicaciones.

## Descripción general

La clase `AWSS3Output` es un manejador de salida especializado dentro de los SDK de VisioForge que facilita el streaming de salida de video y audio al almacenamiento Amazon Web Services (AWS) S3. Esta clase implementa múltiples interfaces para soportar escenarios tanto de edición de video (`IVideoEditXBaseOutput`) como de captura de video (`IVideoCaptureXBaseOutput`), junto con capacidades de procesamiento para contenido de video y audio.

## Implementación de la clase

```csharp
public class AWSS3Output : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

## Características clave

La clase `AWSS3Output` proporciona una solución completa para streaming de contenido de medios a AWS S3 gestionando:

- Configuración de codificación de video
- Configuración de codificación de audio
- Procesamiento de medios personalizado
- Configuraciones específicas de AWS S3

## Propiedades

### Configuración del codificador de video

```csharp
public IVideoEncoder Video { get; set; }
```

Controla el proceso de codificación de video. El codificador de video seleccionado debe ser compatible con la configuración de sink configurada. Esta propiedad le permite especificar métodos de compresión, ajustes de calidad y otros parámetros específicos de video.

### Configuración del codificador de audio

```csharp
public IAudioEncoder Audio { get; set; }
```

Gestiona la configuración de codificación de audio. Como el codificador de video, el codificador de audio debe ser compatible con la configuración de sink. Esta propiedad habilita el control sobre calidad de audio, compresión y configuración de formato.

### Configuración de sink

```csharp
public IMediaBlockSettings Sink { get; set; }
```

Define la configuración del destino de salida. En este contexto, contiene configuraciones específicas de AWS S3 para el flujo de salida de medios.

### Bloques de procesamiento personalizado

```csharp
public MediaBlock CustomVideoProcessor { get; set; }
```

```csharp
public MediaBlock CustomAudioProcessor { get; set; }
```

Permiten procesamiento adicional de flujos de video y audio antes de que sean codificados y subidos a S3. Estos bloques pueden usarse para implementar filtros personalizados, transformaciones o análisis del contenido de medios.

### Configuración de AWS S3

```csharp
public AWSS3SinkSettings Settings { get; set; }
```

Contiene todas las opciones de configuración específicas de AWS S3, incluyendo:

- Credenciales de acceso (Access Key, Secret Access Key)
- Información de bucket y clave de objeto
- Configuración de región
- Configuración de comportamiento de carga
- Preferencias de manejo de errores

## Constructor

```csharp
public AWSS3Output(AWSS3SinkSettings settings, 
                   IVideoEncoder videoEnc, 
                   IAudioEncoder audioEnc, 
                   IMediaBlockSettings sink)
```

Crea una nueva instancia de la clase `AWSS3Output` con la configuración especificada:

- `settings`: Configuración específica de AWS S3
- `videoEnc`: Configuración del codificador de video
- `audioEnc`: Configuración del codificador de audio
- `sink`: Configuración de sink de medios

## Ejemplo de uso

```csharp
// Crear configuración de sink AWS S3
var s3Settings = new AWSS3SinkSettings
{
    AccessKey = "su-clave-de-acceso",
    SecretAccessKey = "su-clave-secreta",
    Bucket = "nombre-de-su-bucket",
    Key = "clave-de-archivo-de-salida",
    Region = "us-west-1"
};

// Configurar codificadores
IVideoEncoder videoEncoder = /* su configuración de codificador de video */;
IAudioEncoder audioEncoder = /* su configuración de codificador de audio */;
IMediaBlockSettings sinkSettings = /* su configuración de sink */;

// Crear la salida AWS S3
var s3Output = new AWSS3Output(s3Settings, videoEncoder, audioEncoder, sinkSettings);

// Opcional: Configurar procesadores personalizados
s3Output.CustomVideoProcessor = /* su procesador de video personalizado */;
s3Output.CustomAudioProcessor = /* su procesador de audio personalizado */;
```

## Mejores prácticas

1. Siempre asegure que sus credenciales de AWS estén debidamente aseguradas y no codificadas directamente en la aplicación.
2. Configure intentos de reintento apropiados y tiempos de espera de solicitud basándose en sus condiciones de red y tamaños de archivo.
3. Seleccione codificadores de video y audio compatibles para su caso de uso objetivo.
4. Considere implementar procesadores personalizados para requisitos específicos como marcas de agua o normalización de audio.

## Manejo de errores

La clase trabaja en conjunto con la enumeración `S3SinkOnError` definida en `AWSS3SinkSettings`, que proporciona tres estrategias de manejo de errores:

- Abort: Detiene el proceso de carga en caso de error
- Complete: Intenta completar la carga a pesar de los errores
- DoNothing: Ignora errores durante la carga

## Componentes relacionados

- AWSS3SinkSettings: Contiene configuración detallada para conectividad AWS S3
- IVideoEncoder: Interfaz para configuración de codificación de video
- IAudioEncoder: Interfaz para configuración de codificación de audio
- IMediaBlockSettings: Interfaz para configuración de salida de medios
