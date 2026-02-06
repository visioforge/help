---
title: Integración del Codificador Windows Media Audio
description: Implemente codificación de audio WMA en .NET con enfoques multiplataforma y específicos de Windows, controles de tasa de bits y configuración de códec.
---

# Codificador Windows Media Audio

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Windows Media Audio (WMA) es un códec de audio popular desarrollado por Microsoft para compresión de audio eficiente. Esta documentación cubre las implementaciones del codificador WMA disponibles en los SDK .Net de VisioForge.

## Descripción general

El SDK de VisioForge proporciona dos enfoques distintos para codificación WMA: el [WMAOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WMAOutput.html) específico de plataforma para entornos Windows y el [WMAEncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.WMAEncoderSettings.html) multiplataforma. Exploremos ambas implementaciones en detalle para entender sus capacidades y casos de uso.

## Salida WMA multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

El `WMAEncoderSettings` proporciona una solución multiplataforma para codificación WMA. Esta implementación está construida sobre el SDK y ofrece comportamiento consistente a través de diferentes sistemas operativos.

### Características principales

El codificador soporta las siguientes configuraciones de audio:

- Tasas de muestreo: 44.1 kHz y 48 kHz
- Tasas de bits: 128, 192, 256 y 320 Kbps
- Configuraciones de canales: Mono (1) y Estéreo (2)

### Control de tasa

El codificador WMA implementa codificación de tasa de bits constante (CBR), permitiéndole especificar una tasa de bits fija de los valores soportados. Esto asegura calidad de audio consistente y tamaños de archivo predecibles a lo largo del contenido codificado.

### Ejemplo de uso

Agregar la salida WMA a la instancia del núcleo Video Capture SDK:

```csharp
// Crear una instancia del núcleo Video Capture SDK
var core = new VideoCaptureCoreX();

// Crear una salida WMA
var wmaOutput = new WMAOutput("output.wma");
wmaOutput.Audio.SampleRate = 48000;
wmaOutput.Audio.Channels = 2;
wmaOutput.Audio.Bitrate = 320;

// Agregar la salida WMA
core.Outputs_Add(wmaOutput, true);
```

Establecer el formato de salida para la instancia del núcleo Video Edit SDK:

```csharp
// Crear una instancia del núcleo Video Edit SDK
var core = new VideoEditCoreX();

// Crear una salida WMA
var wmaOutput = new WMAOutput("output.wma");
wmaOutput.Audio.SampleRate = 48000;
wmaOutput.Audio.Channels = 2;
wmaOutput.Audio.Bitrate = 320;

// Agregar la salida WMA
core.Output_Format = wmaOutput;
```

Crear una instancia de salida WMA de Media Blocks:

```csharp
// Crear una instancia de configuración del codificador WMA
var wmaSettings = new WMAEncoderSettings();

// Crear una instancia de salida WMA
var wmaOutput = new WMAEncoderBlock(wmaSettings);

// Crear una instancia de salida ASF
var asfOutput = new ASFSinkBlock(new ASFSinkSettings("output.wma"));

// Conectar el codificador WMA a la salida ASF
pipeline.Connect(wmaOutput.Output, asfOutput.Input); // pipeline es MediaBlocksPipeline
```

Verificar si la codificación MP3 está disponible.

```
if (!MP3EncoderSettings.IsAvailable())
{
   // Manejar error
}
```

## Salida WMA solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La clase `WMAOutput` proporciona una implementación específica de Windows completa con características avanzadas y opciones de configuración. Esta implementación aprovecha el SDK de formato Windows Media para rendimiento óptimo en sistemas Windows.

### Características principales

La implementación específica de Windows ofrece:

- Soporte de múltiples perfiles (interno, externo y personalizado)
- Configuraciones de idioma y localización
- Codificación basada en calidad
- Control avanzado de tasa de bits con configuraciones de tasa de bits pico
- Configuración de tamaño de buffer

### Control de tasa

La implementación de Windows soporta tres modos de flujo a través de la enumeración WMVStreamMode:

- CBR (Tasa de bits constante)
- VBR (Tasa de bits variable)
- VBR basado en calidad

### Ejemplo de uso

Aquí está cómo configurar el codificador WMA específico de Windows:

Usar un perfil interno para configuración simple

```csharp
var wmaOutput = new WMAOutput
{
    // Usar un perfil interno para configuración simple
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Audio 9 High (192K)"
};

core.Output_Format = wmaOutput; // Core es VideoCaptureCore o VideoEditCore
```

O configurar ajustes personalizados

```csharp
var wmaOutput = new WMAOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Quality = 98,        // Configuración de alta calidad
    Custom_Audio_PeakBitrate = 320,   // Tasa de bits máxima en Kbps
    Custom_Audio_PeakBufferSize = 3   // Tamaño de buffer para streaming
};

core.Output_Format = wmaOutput; // Core es VideoCaptureCore o VideoEditCore
```

### Gestión de perfiles

La implementación de Windows soporta tres modos de perfil:

1. Perfiles internos:
   - Perfiles preconfigurados para casos de uso comunes
   - Acceso a través de `Internal_Profile_Name`

2. Perfiles externos:
   - Cargar perfiles desde archivos externos
   - Configurar usando `External_Profile_FileName` o `External_Profile_Text`

3. Perfiles personalizados:
   - Control detallado sobre parámetros de codificación
   - Configurar a través de propiedades Custom_*

## Mejores prácticas

Al implementar codificación WMA en su aplicación:

1. Para aplicaciones Windows que requieren características avanzadas:
   - Use WMAOutput para acceso a optimizaciones específicas de Windows
   - Considere guardar configuraciones en JSON para reutilización
   - Implemente manejo de errores apropiado para carga de perfiles

2. Para aplicaciones multiplataforma:
   - Manténgase en WMAEncoderSettings para comportamiento consistente
   - Verifique tasas soportadas antes de establecer configuración
   - Use la tasa de muestreo y tasa de bits más altas soportadas para mejor calidad

Esta documentación proporciona una base para implementar codificación WMA en sus aplicaciones. La elección entre implementaciones multiplataforma y específicas de Windows debe basarse en los requisitos de su aplicación para soporte de plataforma, características de codificación y control de calidad.
