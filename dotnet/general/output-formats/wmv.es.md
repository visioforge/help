---
title: Guía de Salida y Codificación de Archivos WMV
description: Implemente codificación Windows Media Video en .NET con configuración de audio/video, opciones de transmisión y gestión de perfiles multiplataforma.
---

# Codificadores Windows Media Video

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta documentación cubre las capacidades de codificación Windows Media Video (WMV) disponibles en VisioForge, incluyendo soluciones tanto específicas de Windows como multiplataforma.

## Salida solo para Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La clase [WMVOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WMVOutput.html) proporciona capacidades completas de codificación Windows Media tanto para audio como para video en plataformas Windows.

### Guía de Inicio Rápido

#### Captura de Video Simple con Configuración Predeterminada

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;

var captureCore = new VideoCaptureCore();

// Usar configuración WMV predeterminada (modo Perfil Interno)
captureCore.Output_Format = new WMVOutput();
captureCore.Output_Filename = "output.wmv";

await captureCore.StartAsync();
```

#### Edición de Video Simple con Configuración Predeterminada

```csharp
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

var editCore = new VideoEditCore();

// Usar configuración WMV predeterminada
editCore.Output_Format = new WMVOutput();
editCore.Output_Filename = "edited_output.wmv";

// Agregar archivos de entrada y configurar edición...

await editCore.StartAsync();
```

#### Ejemplo de Configuración Personalizada

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;

var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 90,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    
    // Configuración de audio
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 90,
    Custom_Audio_Format = "48kHz 16bit Stereo"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = wmvOutput;
captureCore.Output_Filename = "custom_output.wmv";

await captureCore.StartAsync();
```

### Características de Codificación de Audio

La clase `WMVOutput` ofrece varias opciones de configuración específicas de audio:

- Selección de códec de audio personalizado
- Personalización de formato de audio
- Múltiples modos de flujo
- Control de tasa de bits
- Configuración de calidad
- Soporte de idioma
- Gestión de tamaño de búfer

### Modos de Control de Tasa

La codificación WMV soporta cuatro modos de control de tasa a través de la enumeración `WMVStreamMode`:

1. CBR (Tasa de Bits Constante)
2. VBRQuality (Tasa de Bits Variable basada en calidad)
3. VBRBitrate (Tasa de Bits Variable con tasa de bits objetivo)
4. VBRPeakBitrate (Tasa de Bits Variable con restricción de tasa de bits pico)

### Modos de Configuración

El codificador se puede configurar de varias maneras usando la enumeración `WMVMode`:

- ExternalProfile: Cargar configuración desde un archivo de perfil
- ExternalProfileFromText: Cargar configuración desde una cadena de texto
- InternalProfile: Usar perfiles integrados
- CustomSettings: Configuración manual
- V8SystemProfile: Usar perfiles de sistema Windows Media 8

### Código de Muestra

Crear nueva configuración de salida personalizada WMV:

```csharp
var wmvOutput = new WMVOutput
{
    // Configuración básica
    Mode = WMVMode.CustomSettings,
    
    // Configuración de audio
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 98,
    Custom_Audio_PeakBitrate = 192000,
    Custom_Audio_PeakBufferSize = 3,
    
    // Configuración de idioma opcional
    Custom_Audio_LanguageID = "en-US"
};
```

Usando un perfil interno:

```csharp
var profileWmvOutput = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 for Local Network (768 kbps)"
};
```

Configuración de transmisión en red:

```csharp
var streamingWmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Network_Streaming_WMV_Maximum_Clients = 20,
    Custom_Audio_Mode = WMVStreamMode.CBR
};
```

### Configuración de Perfil Personalizado

Los perfiles personalizados le brindan la mayor flexibilidad al permitirle configurar cada aspecto del proceso de codificación. Aquí hay varios ejemplos para diferentes escenarios:

#### Entendiendo las Propiedades de Configuración Personalizada WMV

Antes de sumergirse en los ejemplos, es importante entender las propiedades clave disponibles en la clase `WMVOutput` para configuración personalizada:

**Propiedades de Video:**
- `Custom_Video_StreamPresent` (bool): Habilita el flujo de video en la salida
- `Custom_Video_Codec` (string): Especifica el códec de video (ej., "Windows Media Video 9")
- `Custom_Video_Mode` (WMVStreamMode): Modo de control de tasa (CBR, VBRQuality, VBRBitrate, VBRPeakBitrate)
- `Custom_Video_Bitrate` (int): Tasa de bits objetivo en bits por segundo
- `Custom_Video_Quality` (byte): Nivel de calidad (0-100) para modo de calidad VBR
- `Custom_Video_Width` (int): Ancho de video de salida en píxeles
- `Custom_Video_Height` (int): Altura de video de salida en píxeles
- `Custom_Video_SizeSameAsInput` (bool): Usar dimensiones de video de entrada
- `Custom_Video_FrameRate` (double): Tasa de cuadros de salida
- `Custom_Video_KeyFrameInterval` (byte): Número de cuadros entre cuadros clave
- `Custom_Video_Smoothness` (byte): Nivel de suavidad (0-100)
- `Custom_Video_Peak_BitRate` (int): Tasa de bits pico para modo pico VBR
- `Custom_Video_Peak_BufferSizeSeconds` (byte): Ventana de búfer pico en segundos
- `Custom_Video_Buffer_Size` (int): Tamaño de búfer en milisegundos
- `Custom_Video_Buffer_UseDefault` (bool): Usar configuración de búfer predeterminada
- `Custom_Video_TVSystem` (WMVTVSystem): Estándar de sistema de TV (NTSC, PAL)

**Propiedades de Audio:**
- `Custom_Audio_StreamPresent` (bool): Habilita el flujo de audio en la salida
- `Custom_Audio_Codec` (string): Especifica el códec de audio (ej., "Windows Media Audio 9.2")
- `Custom_Audio_Format` (string): Especificación de formato (ej., "48kHz 16bit Stereo")
- `Custom_Audio_Mode` (WMVStreamMode): Modo de control de tasa
- `Custom_Audio_Quality` (byte): Nivel de calidad (0-100) para modo de calidad VBR
- `Custom_Audio_PeakBitrate` (int): Tasa de bits pico en bits por segundo
- `Custom_Audio_PeakBufferSize` (byte): Ventana de búfer pico en segundos
- `Custom_Audio_LanguageID` (string): Identificador de idioma (ej., "en-US")

**Metadatos de Perfil:**
- `Custom_Profile_Name` (string): Nombre del perfil para identificación
- `Custom_Profile_Description` (string): Descripción detallada del propósito del perfil
- `Custom_Profile_Language` (string): Identificador de idioma del perfil

#### Configuración de Transmisión de Video de Alta Calidad

Perfecto para aplicaciones de transmisión profesional que requieren excelente calidad visual:

```csharp
var highQualityConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video - Alta calidad 1080p
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 95,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 4,
    Custom_Video_Smoothness = 80,
    Custom_Video_Buffer_UseDefault = false,
    Custom_Video_Buffer_Size = 4000,
    
    // Configuración de audio - Estéreo de alta calidad
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 98,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    Custom_Audio_PeakBitrate = 320000,
    Custom_Audio_PeakBufferSize = 3,
    
    // Metadatos de perfil
    Custom_Profile_Name = "High Quality Streaming",
    Custom_Profile_Description = "1080p streaming profile with high quality audio",
    Custom_Profile_Language = "en-US"
};

// Aplicar a VideoCaptureCore
var captureCore = new VideoCaptureCore();
captureCore.Output_Format = highQualityConfig;
captureCore.Output_Filename = "output_hq.wmv";

// O aplicar a VideoEditCore
var editCore = new VideoEditCore();
editCore.Output_Format = highQualityConfig;
editCore.Output_Filename = "output_hq.wmv";
```

#### Configuración de Bajo Ancho de Banda para Transmisión Móvil

Optimizado para dispositivos móviles con ancho de banda limitado:

```csharp
var mobileLowBandwidthConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video optimizada para móvil
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 800000, // 800 kbps
    Custom_Video_Width = 854,
    Custom_Video_Height = 480,
    Custom_Video_FrameRate = 24.0,
    Custom_Video_KeyFrameInterval = 5,
    Custom_Video_Smoothness = 60,
    Custom_Video_Buffer_UseDefault = true,
    
    // Configuración de audio para bajo ancho de banda
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 64000, // 64 kbps
    Custom_Audio_Format = "44kHz 16bit Mono",
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Mobile Low Bandwidth",
    Custom_Profile_Description = "480p optimized for mobile devices"
};

// Aplicar a VideoCaptureCore
var captureCore = new VideoCaptureCore();
captureCore.Output_Format = mobileLowBandwidthConfig;
captureCore.Output_Filename = "output_mobile.wmv";
```

#### Configuración Centrada en Audio para Contenido Musical

Audio de alta calidad con procesamiento de video mínimo:

```csharp
var audioFocusedConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de audio de alta calidad
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2 Professional",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 99,
    Custom_Audio_Format = "96kHz 24bit Stereo",
    Custom_Audio_PeakBitrate = 512000,
    Custom_Audio_PeakBufferSize = 4,
    Custom_Audio_LanguageID = "en-US",
    
    // Configuración de video mínima
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRBitrate,
    Custom_Video_Bitrate = 500000,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 25.0,
    Custom_Video_KeyFrameInterval = 10,
    Custom_Video_Buffer_UseDefault = true,
    
    Custom_Profile_Name = "Audio Focus",
    Custom_Profile_Description = "High quality audio configuration for music content"
};

// Aplicar a VideoEditCore para procesar archivos de audio con video
var editCore = new VideoEditCore();
editCore.Output_Format = audioFocusedConfig;
editCore.Output_Filename = "output_audio_focus.wmv";
```

#### Tasa de Bits Constante (CBR) para Transmisión

El modo CBR es ideal para transmisión en red donde se requiere un ancho de banda consistente:

```csharp
var cbrStreamingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video con CBR
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 2000000, // 2 Mbps constante
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 3,
    Custom_Video_Buffer_Size = 3000,
    Custom_Video_Buffer_UseDefault = false,
    
    // Configuración de audio con CBR
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 128000, // 128 kbps constante
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    // Configuración de transmisión en red
    Network_Streaming_WMV_Maximum_Clients = 50,
    
    Custom_Profile_Name = "CBR Streaming",
    Custom_Profile_Description = "Constant bitrate for reliable network streaming"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = cbrStreamingConfig;
captureCore.Output_Filename = "output_cbr_stream.wmv";
```

#### Tasa de Bits Variable con Control de Pico

VBR con restricción de tasa de bits pico proporciona optimización de calidad mientras limita el ancho de banda máximo:

```csharp
var vbrPeakConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video con control de tasa de bits pico
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRPeakBitrate,
    Custom_Video_Bitrate = 3000000, // 3 Mbps promedio
    Custom_Video_Peak_BitRate = 5000000, // 5 Mbps pico
    Custom_Video_Peak_BufferSizeSeconds = 3,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 4,
    Custom_Video_Smoothness = 75,
    
    // Configuración de audio con control de pico
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRPeakBitrate,
    Custom_Audio_PeakBitrate = 256000,
    Custom_Audio_PeakBufferSize = 2,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    Custom_Profile_Name = "VBR Peak Control",
    Custom_Profile_Description = "Variable bitrate with peak constraints for quality and bandwidth balance"
};

var editCore = new VideoEditCore();
editCore.Output_Format = vbrPeakConfig;
editCore.Output_Filename = "output_vbr_peak.wmv";
```

#### Configuración Optimizada para Grabación de Pantalla

Optimizado para captura de pantalla con codificación eficiente de contenido estático:

```csharp
var screenRecordingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video optimizada para contenido de pantalla
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Screen",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 90,
    Custom_Video_SizeSameAsInput = true, // Usar resolución de pantalla
    Custom_Video_FrameRate = 15.0, // Tasa de cuadros más baja para grabación de pantalla
    Custom_Video_KeyFrameInterval = 10,
    Custom_Video_Smoothness = 50,
    Custom_Video_Buffer_UseDefault = true,
    
    // Configuración de audio para narración de voz
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 85,
    Custom_Audio_Format = "44kHz 16bit Mono", // Mono para voz
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Screen Recording",
    Custom_Profile_Description = "Optimized for screen capture with efficient compression"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = screenRecordingConfig;
captureCore.Output_Filename = "screen_recording.wmv";
```

#### Configuración de Calidad de Archivo

Máxima calidad para propósitos de archivo:

```csharp
var archivalConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video para máxima calidad
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 100,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 1, // Cada cuadro es un cuadro clave
    Custom_Video_Smoothness = 100,
    Custom_Video_Buffer_Size = 8000,
    Custom_Video_Buffer_UseDefault = false,
    
    // Configuración de audio para máxima calidad
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2 Lossless",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 100,
    Custom_Audio_Format = "96kHz 24bit Stereo",
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Archival Quality",
    Custom_Profile_Description = "Maximum quality for long-term storage",
    Custom_Profile_Language = "en-US"
};

var editCore = new VideoEditCore();
editCore.Output_Format = archivalConfig;
editCore.Output_Filename = "archival_quality.wmv";
```

### Uso de Perfil Interno

Los perfiles internos proporcionan configuraciones preconfiguradas optimizadas para escenarios comunes. Aquí hay ejemplos de uso de diferentes perfiles internos:

#### Códecs y Formatos Disponibles

Antes de configurar ajustes personalizados, es útil entender los códecs y formatos disponibles:

**Códecs de Video:**
- "Windows Media Video 9" - Códec WMV9 estándar
- "Windows Media Video 9 Advanced Profile" - Soporte de características avanzadas
- "Windows Media Video 9 Screen" - Optimizado para contenido de pantalla
- "Windows Media Video 8" - Códec WMV8 heredado

**Códecs de Audio:**
- "Windows Media Audio 9.2" - Códec WMA estándar
- "Windows Media Audio 9.2 Professional" - Audio de alta calidad
- "Windows Media Audio 9.2 Lossless" - Compresión sin pérdida
- "Windows Media Audio Voice 9" - Optimizado para voz

**Formatos de Audio:**
Cadenas de formato comunes para la propiedad `Custom_Audio_Format`:
- "48kHz 16bit Stereo" - Estéreo calidad CD
- "44kHz 16bit Stereo" - Estéreo calidad estándar
- "44kHz 16bit Mono" - Mono calidad estándar
- "96kHz 24bit Stereo" - Audio de alta resolución
- "22kHz 16bit Mono" - Calidad de grabación de voz

#### Enumerando Códecs Disponibles

Puede enumerar los códecs disponibles programáticamente:

```csharp
// Para VideoCaptureCore
var captureCore = new VideoCaptureCore();

// Obtener códecs de video disponibles
string[] videoCodecs = captureCore.WMV_VideoCodecs_Available();
foreach (var codec in videoCodecs)
{
    Console.WriteLine($"Video Codec: {codec}");
}

// Obtener códecs de audio disponibles
string[] audioCodecs = captureCore.WMV_AudioCodecs_Available();
foreach (var codec in audioCodecs)
{
    Console.WriteLine($"Audio Codec: {codec}");
}

// Obtener formatos de audio disponibles para un códec específico
string selectedCodec = audioCodecs[0];
string[] audioFormats = captureCore.WMV_AudioFormats_Available(selectedCodec);
foreach (var format in audioFormats)
{
    Console.WriteLine($"Audio Format: {format}");
}
```

Perfil de calidad de transmisión estándar:

```csharp
var broadcastProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Advanced Profile",
    Custom_Video_TVSystem = WMVTVSystem.NTSC  // Anulación opcional de sistema de TV
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = broadcastProfile;
captureCore.Output_Filename = "broadcast_output.wmv";
```

Perfil de transmisión web:

```csharp
var webStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 for Broadband (2 Mbps)",
    Network_Streaming_WMV_Maximum_Clients = 100  // Anulación opcional de transmisión
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = webStreamingProfile;
captureCore.Output_Filename = "web_stream.wmv";
```

Perfil de baja latencia para transmisión en vivo:

```csharp
var liveStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Screen (Low Rate)",
    Network_Streaming_WMV_Maximum_Clients = 50
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = liveStreamingProfile;
captureCore.Output_Filename = "live_stream.wmv";
```

#### Enumerando Perfiles Internos Disponibles

Puede obtener una lista de todos los perfiles internos disponibles:

```csharp
var captureCore = new VideoCaptureCore();
string[] profiles = captureCore.WMV_InternalProfiles_Available();

foreach (var profile in profiles)
{
    Console.WriteLine($"Available Profile: {profile}");
}

// Perfiles internos comunes incluyen:
// - "Windows Media Video 9 for Local Network (768 kbps)"
// - "Windows Media Video 9 for Broadband (2 Mbps)"
// - "Windows Media Video 9 Advanced Profile"
// - "Windows Media Video 9 Screen (Low Rate)"
// - "Windows Media Video 9 Screen (Medium Rate)"
```

### Configuración de Perfil Externo

Los perfiles externos le permiten cargar configuraciones de codificación desde archivos o texto. Esto es útil para compartir configuraciones entre diferentes proyectos o almacenar múltiples configuraciones:

Cargando perfil desde un archivo:

```csharp
var fileBasedProfile = new WMVOutput
{
    Mode = WMVMode.ExternalProfile,
    External_Profile_FileName = @"C:\Profiles\HighQualityStreaming.prx"
};
```

Cargando perfil desde configuración de texto:

```csharp
var textBasedProfile = new WMVOutput
{
    Mode = WMVMode.ExternalProfileFromText,
    External_Profile_Text = @"
        <profile version=""589824"" 
                 storageformat=""1"" 
                 name=""Custom Streaming Profile"" 
                 description=""High quality streaming profile"">
            <streamconfig majortype=""{73647561-0000-0010-8000-00AA00389B71}"" 
                         streamnumber=""1"" 
                         streamname=""Audio Stream"" 
                         inputname=""Audio409"" 
                         bitrate=""128000"" 
                         bufferwindow=""5000"" 
                         reliabletransport=""0"" 
                         decodercomplexity="""" 
                         rfc1766langid=""en-us""/>
            <!-- Additional profile configuration -->
        </profile>"
};
```

Guardando y cargando perfiles programáticamente:

```csharp
async Task SaveAndLoadProfile(WMVOutput profile, string filename)
{
    // Guardar configuración de perfil a JSON
    string jsonConfig = profile.Save();
    await File.WriteAllTextAsync(filename, jsonConfig);
    
    // Cargar configuración de perfil desde JSON
    string loadedJson = await File.ReadAllTextAsync(filename);
    WMVOutput loadedProfile = WMVOutput.Load(loadedJson);
}
```

Ejemplo de uso de guardado/carga de perfil:

```csharp
var profile = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configurar ajustes ...
};

await SaveAndLoadProfile(profile, "encoding_profile.json");
```

### Trabajando con Perfiles Heredados de Windows Media 8

Para compatibilidad con sistemas más antiguos, puede usar perfiles de sistema Windows Media 8:

Usando perfil Windows Media 8:

```csharp
var wmv8Profile = new WMVOutput
{
    Mode = WMVMode.V8SystemProfile,
    V8ProfileName = "Windows Media Video 8 for Dial-up Access (28.8 Kbps)",
};
```

Personalizando configuraciones de transmisión para perfiles Windows Media 8:

```csharp
var wmv8StreamingProfile = new WMVOutput
{
    Mode = WMVMode.V8SystemProfile,
    V8ProfileName = "Windows Media Video 8 for Local Area Network (384 Kbps)",
    Network_Streaming_WMV_Maximum_Clients = 25,
    Custom_Video_TVSystem = WMVTVSystem.PAL  // Anulación opcional de sistema de TV
};
```

### Aplicar configuraciones a su objeto principal

```csharp
var core = new VideoCaptureCore(); // o VideoEditCore
core.Output_Format = wmvOutput;
core.Output_Filename = "output.wmv";
```

### Ejemplo Completo de Trabajo

Aquí hay un ejemplo completo que muestra la captura de video con configuraciones WMV personalizadas, incluyendo inicialización adecuada y manejo de errores:

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace WMVCaptureExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Inicializar SDK de VisioForge
            VisioForge.Core.VisioForge.InitSDK();
            
            // Crear instancia de VideoCaptureCore
            var captureCore = new VideoCaptureCore();
            
            try
            {
                // Configurar fuente de video (primera cámara disponible)
                var videoDevices = captureCore.Video_CaptureDevices();
                if (videoDevices.Length > 0)
                {
                    captureCore.Video_CaptureDevice = new VideoCaptureSource(videoDevices[0].Name);
                }
                
                // Configurar fuente de audio (primer micrófono disponible)
                var audioDevices = captureCore.Audio_CaptureDevices();
                if (audioDevices.Length > 0)
                {
                    captureCore.Audio_CaptureDevice = new AudioCaptureSource(audioDevices[0].Name);
                }
                
                // Configurar salida WMV con ajustes personalizados
                var wmvOutput = new WMVOutput
                {
                    Mode = WMVMode.CustomSettings,
                    
                    // Configuración de video
                    Custom_Video_StreamPresent = true,
                    Custom_Video_Codec = "Windows Media Video 9",
                    Custom_Video_Mode = WMVStreamMode.VBRQuality,
                    Custom_Video_Quality = 85,
                    Custom_Video_Width = 1280,
                    Custom_Video_Height = 720,
                    Custom_Video_FrameRate = 30.0,
                    Custom_Video_KeyFrameInterval = 5,
                    
                    // Configuración de audio
                    Custom_Audio_StreamPresent = true,
                    Custom_Audio_Codec = "Windows Media Audio 9.2",
                    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
                    Custom_Audio_Quality = 90,
                    Custom_Audio_Format = "48kHz 16bit Stereo",
                    
                    Custom_Profile_Name = "Standard Capture",
                    Custom_Profile_Description = "Standard quality capture profile"
                };
                
                // Aplicar configuraciones de salida
                captureCore.Output_Format = wmvOutput;
                captureCore.Output_Filename = "capture_output.wmv";
                captureCore.Mode = VideoCaptureMode.VideoCapture;
                
                // Iniciar captura
                Console.WriteLine("Iniciando captura de video...");
                await captureCore.StartAsync();
                
                Console.WriteLine("Grabando... Presione cualquier tecla para detener.");
                Console.ReadKey();
                
                // Detener captura
                Console.WriteLine("Deteniendo captura de video...");
                await captureCore.StopAsync();
                
                Console.WriteLine($"Video guardado en: capture_output.wmv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Limpieza
                captureCore?.Dispose();
                VisioForge.Core.VisioForge.DestroySDK();
            }
            
            Console.WriteLine("Presione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
```

### Ejemplo Completo de Edición de Video

Aquí hay un ejemplo completo para edición de video con configuraciones WMV personalizadas:

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEdit;

namespace WMVEditExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Inicializar SDK de VisioForge
            VisioForge.Core.VisioForge.InitSDK();
            
            var editCore = new VideoEditCore();
            
            try
            {
                // Agregar archivos de video de entrada
                editCore.Input_AddVideoFile("input_video1.mp4", false);
                editCore.Input_AddVideoFile("input_video2.mp4", false);
                
                // Configurar salida WMV
                var wmvOutput = new WMVOutput
                {
                    Mode = WMVMode.CustomSettings,
                    
                    // Configuración de video de alta calidad
                    Custom_Video_StreamPresent = true,
                    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
                    Custom_Video_Mode = WMVStreamMode.VBRPeakBitrate,
                    Custom_Video_Bitrate = 4000000, // 4 Mbps promedio
                    Custom_Video_Peak_BitRate = 6000000, // 6 Mbps pico
                    Custom_Video_Peak_BufferSizeSeconds = 3,
                    Custom_Video_Width = 1920,
                    Custom_Video_Height = 1080,
                    Custom_Video_FrameRate = 30.0,
                    Custom_Video_KeyFrameInterval = 4,
                    Custom_Video_Smoothness = 80,
                    
                    // Configuración de audio de alta calidad
                    Custom_Audio_StreamPresent = true,
                    Custom_Audio_Codec = "Windows Media Audio 9.2 Professional",
                    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
                    Custom_Audio_Quality = 95,
                    Custom_Audio_Format = "48kHz 16bit Stereo",
                    
                    Custom_Profile_Name = "High Quality Edit",
                    Custom_Profile_Description = "High quality output for edited videos"
                };
                
                // Aplicar configuraciones de salida
                editCore.Output_Format = wmvOutput;
                editCore.Output_Filename = "edited_output.wmv";
                
                // Configurar modo de edición
                editCore.Mode = VideoEditMode.Convert;
                
                // Iniciar edición
                Console.WriteLine("Iniciando edición de video...");
                await editCore.StartAsync();
                
                // Monitorear progreso
                while (editCore.State == VideoEditCoreState.Working)
                {
                    var progress = editCore.Progress();
                    Console.WriteLine($"Progreso: {progress}%");
                    await Task.Delay(500);
                }
                
                Console.WriteLine($"¡Edición de video completa! Salida guardada en: edited_output.wmv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Limpieza
                editCore?.Dispose();
                VisioForge.Core.VisioForge.DestroySDK();
            }
            
            Console.WriteLine("Presione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
```

## Salida WMV Multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La clase `WMVEncoderSettings` proporciona una solución multiplataforma para codificación WMV utilizando tecnología GStreamer.

### Características

- Implementación independiente de plataforma
- Integración con backend GStreamer
- Interfaz de configuración simple
- Verificación de disponibilidad

### Código de Muestra

#### Configuración VideoCaptureCoreX

Agregar la salida WMV a la instancia principal del SDK de Captura de Video:

```csharp
// Configuración básica con ajustes predeterminados
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoCaptureCoreX();
core.Outputs_Add(wmvOutput, true);

// Con configuraciones de codificador personalizadas
var wmvOutput2 = new WMVOutput("output_custom.wmv");
wmvOutput2.Video = new WMVEncoderSettings();
wmvOutput2.Audio = new WMAEncoderSettings
{
    Bitrate = 192,  // Tasa de bits en Kbps
    SampleRate = 48000,  // Frecuencia de muestreo en Hz
    Channels = 2  // Estéreo
};

var core2 = new VideoCaptureCoreX();
core2.Outputs_Add(wmvOutput2, true);
```

#### Configuración VideoEditCoreX

Establecer el formato de salida para la instancia principal del SDK de Edición de Video:

```csharp
// Configuración básica
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoEditCoreX();
core.Output_Format = wmvOutput;

// Con configuraciones de audio personalizadas
var wmvOutput2 = new WMVOutput("output_high_quality.wmv");
wmvOutput2.Audio = new WMAEncoderSettings
{
    Bitrate = 256,  // Audio de alta calidad
    SampleRate = 48000,
    Channels = 2
};

var core2 = new VideoEditCoreX();
core2.Output_Format = wmvOutput2;
```

#### Configuración de Tubería Media Blocks

Crear una instancia de salida WMV de Media Blocks:

```csharp
// Configurar codificador de audio
var wma = new WMAEncoderSettings
{
    Bitrate = 128,  // Tasa de bits en Kbps
    SampleRate = 48000,  // 48 kHz
    Channels = 2  // Estéreo
};

// Configurar codificador de video  
var wmv = new WMVEncoderSettings();

// Configurar sumidero ASF (contenedor)
var sinkSettings = new ASFSinkSettings("output.wmv");

// Crear bloque de salida
var wmvOutput = new WMVOutputBlock(sinkSettings, wmv, wma);

// Agregar a tubería
var pipeline = new MediaBlocksPipeline();
// ... configurar fuentes y conectar a wmvOutput
```

#### Verificando Disponibilidad del Codificador

Siempre verifique si los codificadores están disponibles antes de usar:

```csharp
// Verificar disponibilidad del codificador WMV
if (WMVEncoderSettings.IsAvailable())
{
    Console.WriteLine("El codificador WMV está disponible");
    var wmvOutput = new WMVOutput("output.wmv");
    // ... usar codificador
}
else
{
    Console.WriteLine("El codificador WMV no está disponible en este sistema");
    // Recurrir a codificador alternativo
}

// Verificar disponibilidad del codificador WMA
if (WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("El codificador WMA está disponible");
}
```

#### Configuración Multiplataforma Avanzada

```csharp
// Crear una salida WMV multiplataforma de alta calidad
var wmvOutput = new WMVOutput("output_hq.wmv");

// Configurar audio de alta calidad
wmvOutput.Audio = new WMAEncoderSettings
{
    Bitrate = 320,  // Máxima calidad
    SampleRate = 48000,
    Channels = 2
};

// Configuración del codificador de video (usa codificador WMV1 predeterminado)
wmvOutput.Video = new WMVEncoderSettings();

// Verificar si se necesita procesador de video personalizado
// wmvOutput.CustomVideoProcessor = myCustomProcessor;

// Aplicar a núcleo
var core = new VideoCaptureCoreX();
core.Outputs_Add(wmvOutput, true);

// Iniciar captura
await core.StartAsync();
```

#### Configuraciones de Audio Disponibles

La clase `WMAEncoderSettings` proporciona las siguientes opciones de configuración:

```csharp
var audioSettings = new WMAEncoderSettings
{
    // Tasa de bits en Kbps - valores soportados: 128, 192, 256, 320
    Bitrate = 192,
    
    // Frecuencia de muestreo en Hz - valores soportados: 44100, 48000
    SampleRate = 48000,
    
    // Número de canales - valores soportados: 1 (mono), 2 (estéreo)
    Channels = 2
};

// Obtener tasas de bits soportadas
int[] supportedBitrates = audioSettings.GetSupportedBitrates();
// Retorna: [128, 192, 256, 320]

// Obtener frecuencias de muestreo soportadas
int[] supportedSampleRates = audioSettings.GetSupportedSampleRates();
// Retorna: [44100, 48000]

// Obtener conteos de canales soportados
int[] supportedChannels = audioSettings.GetSupportedChannelCounts();
// Retorna: [1, 2]
```

### Eligiendo Entre Codificadores

Considere los siguientes factores al elegir entre `WMVOutput` específico de Windows y `WMVEncoderSettings` multiplataforma:

#### WMVOutput Específico de Windows

- Pros:
  - Acceso completo a características del formato Windows Media
  - Opciones avanzadas de control de tasa
  - Soporte de transmisión en red
  - Configuración basada en perfiles
- Contras:
  - Compatibilidad solo con Windows
  - Requiere componentes de Windows Media

#### WMV Multiplataforma

- Pros:
  - Independencia de plataforma
  - Implementación más simple
- Contras:
  - Conjunto de características más limitado
  - Solo opciones de configuración básicas

## Mejores Prácticas

### Referencias MSDN

Para información detallada sobre tecnologías Windows Media, consulte estos recursos oficiales de Microsoft:

- [Windows Media Format SDK](https://learn.microsoft.com/es-es/windows/win32/wmformat/windows-media-format-11-sdk) - Documentación completa de Formato Windows Media
- [Trabajando con Perfiles](https://learn.microsoft.com/en-us/windows/win32/wmformat/working-with-profiles) - Gestión y configuración de perfiles
- [Códecs Windows Media](https://learn.microsoft.com/en-us/windows/win32/medfound/windows-media-codecs) - Información de códecs de audio y video
- [Estructura de Archivo ASF](https://learn.microsoft.com/en-us/windows/win32/medfound/asf-file-structure) - Detalles del contenedor Advanced Systems Format
- [Configurando Flujos de Video](https://learn.microsoft.com/en-us/windows/win32/wmformat/configuring-video-streams) - Parámetros de codificación de video
- [Configurando Flujos de Audio](https://learn.microsoft.com/en-us/windows/win32/wmformat/configuring-audio-streams) - Parámetros de codificación de audio

### Eligiendo Modos de Control de Tasa

Seleccione el modo de control de tasa apropiado basado en su caso de uso:

1. **CBR (Tasa de Bits Constante)**
   - Usar para: Transmisión en red, radiodifusión
   - Ventajas: Ancho de banda predecible, calidad consistente
   - Desventajas: Compresión menos eficiente, puede no adaptarse a la complejidad del contenido
   - Ejemplo: Transmisión en vivo para asegurar reproducción fluida

2. **VBRQuality (Tasa de Bits Variable - Calidad)**
   - Usar para: Salida basada en archivos, archivo, video de alta calidad
   - Ventajas: Mejor relación calidad-tamaño, se adapta a la complejidad del contenido
   - Desventajas: Tamaño de archivo y tasa de bits impredecibles
   - Ejemplo: Grabación de tutoriales o presentaciones para reproducción posterior

3. **VBRBitrate (Tasa de Bits Variable - Tasa de Bits Objetivo)**
   - Usar para: Cuando necesita optimización de calidad con restricciones de tamaño
   - Ventajas: Equilibra calidad y tamaño de archivo objetivo
   - Desventajas: La calidad puede variar entre escenas
   - Ejemplo: Creación de videos para carga con límites de tamaño

4. **VBRPeakBitrate (Tasa de Bits Variable - Pico Restringido)**
   - Usar para: Transmisión con restricciones de ancho de banda
   - Ventajas: Optimización de calidad con techo de ancho de banda
   - Desventajas: Configuración más compleja
   - Ejemplo: Escenarios de transmisión adaptativa

### Optimización de Rendimiento

1. **Configuración de Búfer**
   - Establezca `Custom_Video_Buffer_UseDefault = false` para control ajustado
   - Aumente `Custom_Video_Buffer_Size` para transmisión más fluida (predeterminado: 3000ms)
   - Equilibre el tamaño del búfer con los requisitos de latencia

2. **Intervalo de Cuadros Clave**
   - Valores más bajos (1-3): Mejor rendimiento de búsqueda, tamaño de archivo más grande
   - Valores más altos (5-10): Tamaño de archivo más pequeño, menos precisión de búsqueda
   - Recomendado: 3-5 para transmisión, 10+ para grabación de pantalla

3. **Configuración de Suavidad**
   - 0-50: Priorizar eficiencia de compresión
   - 50-75: Calidad y eficiencia equilibradas
   - 75-100: Priorizar calidad visual

### Pautas de Resolución y Tasa de Cuadros

```csharp
// Configuración 4K/UHD
var uhd4KConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 95,
    Custom_Video_Width = 3840,
    Custom_Video_Height = 2160,
    Custom_Video_FrameRate = 30.0,
    // ... otros ajustes
};

// Configuración Full HD
var fullHDConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    // ... otros ajustes
};

// Configuración HD Ready
var hdReadyConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    // ... otros ajustes
};

// Configuración SD
var sdConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 720,
    Custom_Video_Height = 480,
    Custom_Video_FrameRate = 29.97,
    Custom_Video_TVSystem = WMVTVSystem.NTSC,
    // ... otros ajustes
};
```

### Manejo de Errores y Validación

Siempre valide su configuración antes de iniciar la captura o edición:

```csharp
var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configuración
};

try
{
    var captureCore = new VideoCaptureCore();
    captureCore.Output_Format = wmvOutput;
    captureCore.Output_Filename = "output.wmv";
    
    // Validar configuración
    if (captureCore.Output_Filename == null || captureCore.Output_Filename.Length == 0)
    {
        throw new InvalidOperationException("Se requiere nombre de archivo de salida");
    }
    
    // Verificar si los códecs personalizados están disponibles
    if (wmvOutput.Mode == WMVMode.CustomSettings)
    {
        var videoCodecs = captureCore.WMV_VideoCodecs_Available();
        if (!videoCodecs.Contains(wmvOutput.Custom_Video_Codec))
        {
            Console.WriteLine($"Advertencia: El códec de video '{wmvOutput.Custom_Video_Codec}' puede no estar disponible");
        }
    }
    
    await captureCore.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    // Manejar error apropiadamente
}
```

### Configuración de Transmisión en Red

Para escenarios de transmisión en red, configure tanto el codificador como los ajustes de transmisión:

```csharp
var streamingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuración de video optimizada para transmisión
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 1500000, // 1.5 Mbps
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 3,
    Custom_Video_Buffer_Size = 2000, // Búfer más bajo para latencia reducida
    Custom_Video_Buffer_UseDefault = false,
    
    // Configuración de audio
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 128000,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    // Configuración de transmisión en red
    Network_Streaming_WMV_Maximum_Clients = 100,
    
    Custom_Profile_Name = "Network Streaming",
    Custom_Profile_Description = "Optimized for network streaming with low latency"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = streamingConfig;
captureCore.Output_Filename = "http://localhost:8080/stream"; // O ruta de archivo
```

### Pruebas y Validación

1. **Siempre pruebe su configuración** con contenido de muestra antes del uso en producción
2. **Monitoree el rendimiento de codificación** para asegurar capacidad de codificación en tiempo real
3. **Verifique la compatibilidad de archivos** con sus dispositivos de reproducción de destino
4. **Valide la sincronización de audio** especialmente con tasas de cuadros personalizadas
5. **Pruebe la transmisión en red** bajo varias condiciones de ancho de banda

### Gestión de Perfiles

Guarde y reutilice configuraciones para consistencia:

```csharp
// Guardar configuración a JSON
var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configuración
};

string jsonConfig = wmvOutput.Save();
File.WriteAllText("profile_high_quality.json", jsonConfig);

// Cargar configuración desde JSON
string loadedJson = File.ReadAllText("profile_high_quality.json");
var loadedProfile = WMVOutput.Load(loadedJson);

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = loadedProfile;
```

### Problemas Comunes y Soluciones

1. **Tamaños de Archivo Grandes**
   - Use modo VBRBitrate en lugar de VBRQuality
   - Reduzca la calidad o resolución de video
   - Aumente KeyFrameInterval
   - Considere usar códec de pantalla para grabaciones de pantalla

2. **Mala Calidad**
   - Aumente la configuración de calidad de video
   - Use una tasa de bits más alta
   - Cambie a modo VBRQuality
   - Asegure un tamaño de búfer suficiente

3. **Problemas de Transmisión**
   - Use modo CBR para ancho de banda consistente
   - Reduzca el tamaño del búfer para menor latencia
   - Pruebe con conteo de clientes apropiado
   - Monitoree el ancho de banda de red

4. **Códec No Disponible**
   - Asegure que los componentes de Windows Media estén instalados
   - Verifique la enumeración de códecs programáticamente
   - Recurra a perfiles internos predeterminados
   - Considere alternativas multiplataforma (WMVEncoderSettings)

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más muestras de código.
