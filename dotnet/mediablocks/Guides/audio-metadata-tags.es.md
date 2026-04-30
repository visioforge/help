---
title: Escribir Metadatos de Audio en C# .NET - ID3, Vorbis
description: Añada metadatos ID3, Vorbis Comments y MP4 a archivos de audio con VisioForge Media Blocks SDK. Ejemplos de código para MP3, OGG, M4A y WMA.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Encoding
  - Metadata
  - MP4
  - WMV
  - OGG
  - AAC
  - MP3
  - Vorbis
  - WMA
  - C#
primary_api_classes:
  - MediaFileTags
  - MP3OutputBlock
  - OGGVorbisOutputBlock
  - M4AOutputBlock
  - WMVOutputBlock
  - MediaBlocksPipeline
  - SystemAudioSourceBlock

---

# Escribir Etiquetas de Audio con SDK de Media Blocks

[SDK de Media Blocks .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tabla de Contenidos

- [Escribir Etiquetas de Audio con SDK de Media Blocks](#escribir-etiquetas-de-audio-con-sdk-de-media-blocks)
  - [Tabla de Contenidos](#tabla-de-contenidos)
  - [Resumen](#resumen)
  - [Características Principales](#caracteristicas-principales)
  - [Formatos de Audio Soportados](#formatos-de-audio-soportados)
  - [Prerrequisitos](#prerrequisitos)
  - [MediaFileTags: La Interfaz Unificada](#mediafiletags-la-interfaz-unificada)
  - [Ejemplos de Código por Formato](#ejemplos-de-codigo-por-formato)
    - [Salida MP3 con Etiquetas ID3](#salida-mp3-con-etiquetas-id3)
    - [Salida OGG Vorbis con Comentarios Vorbis](#salida-ogg-vorbis-con-comentarios-vorbis)
    - [Salida M4A con Metadatos MP4](#salida-m4a-con-metadatos-mp4)
    - [Salida WMV/WMA con Metadatos ASF](#salida-wmvwma-con-metadatos-asf)

## Resumen

El VisioForge Media Blocks SDK admite la escritura de metadatos de audio en archivos de salida para todos los formatos de audio principales. Tanto si construye una aplicación de producción musical, una grabadora de podcast o un sistema de gestión de contenido, puede incrustar metadatos en sus archivos de audio a través de una interfaz de programación unificada.

Esta guía muestra cómo añadir etiquetas (artista, álbum, título, año, género, etc.) a archivos MP3, OGG Vorbis, M4A y WMV/WMA usando los mecanismos de etiquetado apropiados para cada formato y manteniendo el cumplimiento de los estándares del sector.

## Características Principales

- **Soporte universal de etiquetas**: escribe metadatos en MP3 (ID3), OGG (Vorbis Comments), M4A (átomos MP4) y WMV (atributos ASF)
- **Metadatos completos**: más de 20 campos, incluidos título, artista, álbum, año, números de pista, letras y carátulas
- **Compatible con estándares**: usa los mecanismos nativos de etiquetado de cada contenedor
- **API unificada**: una sola instancia de `MediaFileTags` vale para todos los formatos
- **Flexible en tiempo de ejecución**: modifique etiquetas antes de iniciar el pipeline

## Formatos de Audio Soportados

| Formato | Contenedor | Sistema de etiquetas | Estándares |
|--------|-----------|------------|-----------|
| **MP3** | MPEG-1 Layer 3 | ID3v1/ID3v2 | ISO/IEC 11172-3, ID3v2.4 |
| **OGG Vorbis** | OGG | Vorbis Comments | especificación Xiph.Org |
| **M4A** | MP4/MPEG-4 | átomos de metadatos MP4 | ISO Base Media File Format |
| **WMV/WMA** | ASF | atributos de metadatos ASF | especificación ASF de Microsoft |

## Prerrequisitos

- VisioForge Media Blocks SDK .NET
- .NET Framework 4.7.2+ o .NET Core 3.1+ o .NET 5+
- Comprensión básica de pipelines de procesamiento de audio

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Outputs;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sources;
```

## MediaFileTags: La Interfaz Unificada

La clase `MediaFileTags` proporciona una interfaz unificada para metadatos de audio en todos los formatos soportados. Contiene los campos comunes y es mapeada al formato de etiquetas adecuado por cada bloque de salida.

```csharp
// Crear metadatos de audio
var audioTags = new MediaFileTags
{
    // Metadatos básicos
    Title = "Bohemian Rhapsody",
    Performers = new[] { "Queen" },
    Album = "A Night at the Opera",
    Year = 1975,
    
    // Información de pista
    Track = 11,
    TrackCount = 12,
    Disc = 1,
    DiscCount = 1,
    
    // Género y categorización
    Genres = new[] { "Progressive Rock", "Opera Rock" },
    
    // Metadatos extendidos
    Composers = new[] { "Freddie Mercury" },
    Conductor = "Roy Thomas Baker",
    Comment = "Obra maestra épica de 6 minutos",
    Copyright = "© 1975 Queen Productions Ltd.",
    
    // Metadatos técnicos
    BeatsPerMinute = 72,
    Grouping = "Epic Songs",
    
    // Letras (para formatos compatibles)
    Lyrics = @"Is this the real life?
Is this just fantasy?
Caught in a landslide
No escape from reality..."
};
```

Todos los campos de tipo `string[]` (`Performers`, `Composers`, `Genres`, `AlbumArtists`) aceptan múltiples valores y se escriben como frames repetidos en formatos que lo admiten (Vorbis Comments, ID3v2). Los campos numéricos (`Year`, `Track`, `TrackCount`, `Disc`, `DiscCount`, `BeatsPerMinute`) son `uint`.

## Ejemplos de Código por Formato

### Salida MP3 con Etiquetas ID3

Los archivos MP3 usan etiquetas ID3 (v1 y v2) para almacenar metadatos. `MP3OutputBlock` escribe etiquetas ID3 conformes con los estándares mediante el elemento GStreamer `id3mux`.

```csharp
public async Task CreateMP3WithTags()
{
    // Configurar los ajustes del codificador MP3
    var mp3Settings = new MP3EncoderSettings
    {
        Bitrate = 320,                              // Kbit/s
        RateControl = MP3EncoderRateControl.CBR    // CBR / ABR / VBR
    };
    
    // Crear etiquetas de metadatos
    var tags = new MediaFileTags
    {
        Title = "Summer Vibes",
        Performers = new[] { "Indie Artist" },
        Album = "Seasonal Collection",
        Year = 2025,
        Track = 3,
        TrackCount = 10,
        Genres = new[] { "Indie Pop", "Electronic" },
        Comment = "Grabado en estudio casero",
        Copyright = "© 2025 Independent Label"
    };
    
    // Crear bloque de salida MP3 con etiquetas
    var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings, tags);
    
    // Alternativa: asignar etiquetas tras la creación vía la propiedad Tags
    // var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);
    // mp3Output.Tags = tags;
    
    // Construir el pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Añadir una fuente de audio (micrófono, archivo, etc.)
    var audioSource = new SystemAudioSourceBlock();
    
    // Conectar la fuente directamente a la salida MP3
    pipeline.Connect(audioSource.Output, mp3Output.Input);
    
    // Iniciar la grabación con metadatos
    await pipeline.StartAsync();
    
    // La grabación escribe las etiquetas ID3 en el archivo MP3
    await Task.Delay(TimeSpan.FromSeconds(30));
    
    await pipeline.StopAsync();
}
```

### Salida OGG Vorbis con Comentarios Vorbis

Los archivos OGG Vorbis usan Vorbis Comments para los metadatos, que se incrustan directamente en el flujo de audio por el codificador Vorbis.

```csharp
public async Task CreateOGGWithTags()
{
    // Configurar los ajustes del codificador Vorbis.
    // Quality es un int en el rango [-1..10] (por defecto 4). Se usa cuando RateControl = Quality.
    var vorbisSettings = new VorbisEncoderSettings
    {
        Quality = 8,
        RateControl = VorbisEncoderRateControl.Quality
    };
    
    // Crear metadatos
    var tags = new MediaFileTags
    {
        Title = "Acoustic Session",
        Performers = new[] { "Folk Artist", "Guest Vocalist" },
        AlbumArtists = new[] { "Folk Artist" },
        Album = "Live Sessions",
        Year = 2025,
        Track = 1,
        Genres = new[] { "Folk", "Acoustic" },
        Composers = new[] { "Folk Artist", "Traditional" },
        Comment = "Grabado en directo en el Estudio A",
        Conductor = "Sound Engineer",
        Grouping = "Live Recordings",
        Lyrics = @"In the quiet of the morning
When the world begins to wake
There's a song within the silence..."
    };
    
    // Crear bloque de salida OGG con comentarios Vorbis
    var oggOutput = new OGGVorbisOutputBlock("output.ogg", vorbisSettings, tags);
    
    // Construir y ejecutar el pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Usar UniversalSourceBlock para decodificar cualquier archivo a audio crudo
    var sourceSettings = await UniversalSourceSettings.CreateAsync(new Uri("input.wav"));
    var fileSource = new UniversalSourceBlock(sourceSettings);
    
    pipeline.Connect(fileSource.AudioOutput, oggOutput.Input);
    
    await pipeline.StartAsync();
    await pipeline.WaitForStopAsync();   // Esperar EOS
}
```

### Salida M4A con Metadatos MP4

Los archivos M4A usan átomos de metadatos MP4, compatibles con iTunes y la mayoría de reproductores. El ctor por defecto de `M4AOutputBlock` elige un codificador AAC predeterminado internamente; use la sobrecarga de 3 argumentos para elegir una implementación AAC específica (`AVENCAACEncoderSettings`, `VOAACEncoderSettings` o `MFAACEncoderSettings` en Windows).

```csharp
public async Task CreateM4AWithTags()
{
    // Crear metadatos de un podcast
    var tags = new MediaFileTags
    {
        Title = "Episodio 42: El Futuro de la IA",
        Performers = new[] { "Tech Podcast Host" },
        Album = "Weekly Tech Talk",
        Year = 2025,
        Track = 42,
        Genres = new[] { "Technology", "Podcast" },
        Comment = "Entrevista especial con un investigador de IA",
        Copyright = "© 2025 Tech Media Network",
        Subtitle = "Explorando tendencias de inteligencia artificial",
        Grouping = "Season 3"
    };
    
    // Opción A: más simple — codificador AAC por defecto elegido internamente
    var m4aOutput = new M4AOutputBlock("podcast_episode_42.m4a", tags);
    
    // Opción B: elegir un codificador AAC específico y ajustes de sink
    // var sinkSettings = new MP4SinkSettings("podcast_episode_42.m4a");
    // var aacSettings = new AVENCAACEncoderSettings { Bitrate = 256 }; // 256 Kbit/s
    // var m4aOutput = new M4AOutputBlock(sinkSettings, aacSettings, tags);
    
    // Configurar el pipeline para grabación de podcast
    var pipeline = new MediaBlocksPipeline();
    var micSource = new SystemAudioSourceBlock();
    
    pipeline.Connect(micSource.Output, m4aOutput.Input);
    
    await pipeline.StartAsync();
}
```

### Salida WMV/WMA con Metadatos ASF

Los formatos de Windows Media usan atributos de metadatos ASF (Advanced Systems Format). `WMVOutputBlock` admite un parámetro `MediaFileTags` y funciona tanto para salidas solo-audio como audio+vídeo.

```csharp
public async Task CreateWMVWithTags()
{
    // Crear metadatos de presentación
    var tags = new MediaFileTags
    {
        Title = "Revisión de Negocio Q4",
        Performers = new[] { "CEO", "CFO", "VP Sales" },
        Album = "Corporate Presentations 2025",
        Year = 2025,
        Genres = new[] { "Business", "Corporate" },
        Comment = "Revisión financiera trimestral y perspectivas",
        Copyright = "© 2025 Business Corp. Confidential",
        Conductor = "Meeting Organizer",
        Grouping = "Executive Presentations"
    };
    
    // Forma más simple — codificadores por defecto, sólo filename + tags.
    // WMVOutputBlock usa internamente los ajustes por defecto de WMV/WMA.
    var wmvOutput = new WMVOutputBlock("presentation.wmv", tags);
    
    // Alternativa: pasar objetos explícitos de sink/video/audio
    // var asfSettings = new ASFSinkSettings("presentation.wmv");
    // var wmvSettings = WMVEncoderBlock.GetDefaultSettings();
    // var wmaSettings = WMAEncoderBlock.GetDefaultSettings();
    // var wmvOutput = new WMVOutputBlock(asfSettings, wmvSettings, wmaSettings, tags);
    
    // Configurar grabación de vídeo + audio
    var pipeline = new MediaBlocksPipeline();
    
    // Fuente de vídeo — elegir el primer dispositivo disponible
    var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
    var videoSource = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(videoDevice));
    
    // Fuente de audio
    var audioSource = new SystemAudioSourceBlock();
    
    // Crear pads de entrada dinámicos en la salida WMV
    var videoPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Video);
    var audioPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Audio);
    
    pipeline.Connect(videoSource.Output, videoPad);
    pipeline.Connect(audioSource.Output, audioPad);
    
    await pipeline.StartAsync();
}
```

## Ejemplo Completo de Grabación de Audio

Grabe la misma fuente de audio en varios formatos de salida con etiquetas simultáneamente:

```csharp
public class AudioRecorderWithTags
{
    public async Task RecordAudioWithMetadata()
    {
        // Metadatos compartidos entre todos los archivos de salida
        var sessionTags = new MediaFileTags
        {
            Title = "Studio Session #1",
            Performers = new[] { "John Doe", "Jane Smith" },
            Album = "Demo Recordings",
            Year = 2025,
            Track = 1,
            Genres = new[] { "Rock", "Alternative" },
            Composers = new[] { "John Doe" },
            Comment = "Primera sesión de grabación de estudio",
            Copyright = "© 2025 Demo Productions",
            BeatsPerMinute = 120,
            Grouping = "Demo Sessions"
        };
        
        // Salida MP3 (CBR 320 Kbit/s + etiquetas ID3)
        var mp3Output = new MP3OutputBlock(
            "session1.mp3",
            new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR },
            sessionTags);
        
        // Salida OGG Vorbis (máxima calidad + Vorbis Comments)
        var oggOutput = new OGGVorbisOutputBlock(
            "session1.ogg",
            new VorbisEncoderSettings { Quality = 10, RateControl = VorbisEncoderRateControl.Quality },
            sessionTags);
        
        // Salida M4A (AAC por defecto + átomos MP4)
        var m4aOutput = new M4AOutputBlock("session1.m4a", sessionTags);
        
        // Pipeline con una única fuente de audio distribuida a los tres archivos
        var pipeline = new MediaBlocksPipeline();
        var audioSource = new SystemAudioSourceBlock();
        
        // Conectar la misma salida de fuente a varios sinks inserta un tee automáticamente
        pipeline.Connect(audioSource.Output, mp3Output.Input);
        pipeline.Connect(audioSource.Output, oggOutput.Input);
        pipeline.Connect(audioSource.Output, m4aOutput.Input);
        
        Console.WriteLine("Iniciando grabación con metadatos...");
        await pipeline.StartAsync();
        
        await Task.Delay(TimeSpan.FromMinutes(3));
        
        Console.WriteLine("Deteniendo grabación...");
        await pipeline.StopAsync();
        
        Console.WriteLine("Grabación completada — archivos escritos con metadatos:");
        Console.WriteLine("- session1.mp3 (etiquetas ID3)");
        Console.WriteLine("- session1.ogg (Vorbis comments)");
        Console.WriteLine("- session1.m4a (metadatos MP4)");
    }
}
```

## Escenarios Avanzados de Etiquetas

### Soporte de Carátulas de Álbum

Adjunte carátulas a los formatos compatibles (MP3, M4A, WMV). En Windows, `MediaFileTags.Pictures` acepta `System.Drawing.Bitmap[]`; las compilaciones multiplataforma usan `IBitmap[]`.

```csharp
var tags = new MediaFileTags
{
    Title = "Album Title Track",
    Performers = new[] { "Artist Name" },
    Album = "Album Name"
};

// Adjuntar carátula (Windows — System.Drawing)
#if NET_WINDOWS
if (File.Exists("album_cover.jpg"))
{
    var albumArt = new System.Drawing.Bitmap("album_cover.jpg");
    tags.Pictures = new[] { albumArt };
    tags.Pictures_Descriptions = new[] { "Front Cover" };
}
#endif

var mp3Output = new MP3OutputBlock(
    "track.mp3",
    new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR },
    tags);
```

### Modificación de Etiquetas en Tiempo de Ejecución

Establezca o modifique las etiquetas antes de iniciar el pipeline — una vez iniciado, la carga útil de etiquetas de esa salida ya se está emitiendo.

```csharp
var mp3Settings = new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR };
var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);

// Asignar etiquetas a través de la propiedad Tags antes de StartAsync
mp3Output.Tags = new MediaFileTags
{
    Title = "Live Recording",
    Performers = new[] { "Artist" }
};

// Ajustar campos hasta el momento del inicio
mp3Output.Tags.Comment = $"Grabado el {DateTime.Now:yyyy-MM-dd}";
mp3Output.Tags.Year = (uint)DateTime.Now.Year;

await pipeline.StartAsync();
```

### Álbumes Multi-Pista

Cree metadatos consistentes entre pistas de un álbum usando un objeto base compartido:

```csharp
public class AlbumRecorder
{
    private readonly MediaFileTags _baseAlbumTags;
    private readonly MP3EncoderSettings _mp3Settings =
        new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR };

    public AlbumRecorder()
    {
        _baseAlbumTags = new MediaFileTags
        {
            Album = "My Album",
            AlbumArtists = new[] { "Main Artist" },
            Year = 2025,
            Genres = new[] { "Pop", "Electronic" },
            TrackCount = 12,
            Copyright = "© 2025 Record Label"
        };
    }

    public MP3OutputBlock CreateTrackOutput(int trackNumber, string title, string[] performers)
    {
        var trackTags = new MediaFileTags
        {
            // Heredar metadatos de nivel álbum
            Album = _baseAlbumTags.Album,
            AlbumArtists = _baseAlbumTags.AlbumArtists,
            Year = _baseAlbumTags.Year,
            Genres = _baseAlbumTags.Genres,
            TrackCount = _baseAlbumTags.TrackCount,
            Copyright = _baseAlbumTags.Copyright,

            // Metadatos específicos de pista
            Track = (uint)trackNumber,
            Title = title,
            Performers = performers
        };

        return new MP3OutputBlock($"track_{trackNumber:D2}.mp3", _mp3Settings, trackTags);
    }
}
```

## Mejores Prácticas

### Calidad de los Datos de Etiquetas

- **Codificación consistente**: use UTF-8 para caracteres internacionales
- **Información completa**: rellene tantos campos relevantes como sea posible
- **Géneros estandarizados**: use nombres de género reconocidos para mayor compatibilidad
- **Copyright adecuado**: incluya avisos de copyright apropiados

### Consideraciones de Rendimiento

- **Tamaño de etiquetas**: mantenga los campos de texto con una longitud razonable para no inflar los archivos
- **Compresión de imágenes**: comprima las carátulas adecuadamente (JPEG recomendado)
- **Reutilizar instancias**: al crear varios archivos, comparta objetos base de etiquetas y sólo cambie los campos específicos por pista

### Pautas Específicas por Formato

```csharp
// MP3: ID3v2 admite metadatos extensos
var mp3Tags = new MediaFileTags
{
    Title = "Song Title",
    Subtitle = "Song Subtitle",     // frame TIT3 de ID3v2
    Lyrics = "Full lyrics text",    // frame USLT
    BeatsPerMinute = 128            // frame TBPM
};

// OGG: los comentarios Vorbis son flexibles y admiten campos multi-valor nativamente
var oggTags = new MediaFileTags
{
    Composers = new[] { "Composer 1", "Composer 2" },
    Performers = new[] { "Artist 1", "Artist 2" }
};

// M4A: metadatos compatibles con iTunes
var m4aTagsForPodcast = new MediaFileTags
{
    Title = "Episode Title",
    Album = "Podcast Series Name",  // Aparece como "Album" en iTunes
    Performers = new[] { "Host Name" }, // Aparece como "Artist"
    Genres = new[] { "Podcast" },
    Comment = "Episode description"
};
```

## Solución de Problemas

### Problemas Comunes y Soluciones

**Las etiquetas no aparecen en los reproductores multimedia:**

- Asegúrese de que el formato de salida soporta los campos que está usando
- Verifique que el reproductor soporta el formato de etiquetas (algunos prefieren ID3v2.3 sobre ID3v2.4)
- Compruebe que la codificación de texto es correcta (UTF-8 recomendado)

**Tamaño de archivo inesperadamente grande:**

- Reduzca la resolución de la carátula (600×600 suele ser suficiente)
- Evite campos de texto extremadamente largos en comentarios o letras
- Use compresión adecuada para las imágenes

**Errores de codificación:**

- Valide que los caracteres especiales estén correctamente codificados
- Asegúrese de que las rutas de archivo son accesibles y escribibles
- Compruebe que los ajustes del codificador son compatibles con su sistema

### Depurar la Escritura de Etiquetas

Suscríbase al evento `OnError` del pipeline para ver fallos de codificador/muxer durante la escritura de etiquetas. No hay un flujo específico de "mensajes de tag" — inspeccione el archivo producido con un lector de etiquetas (TagLib, MediaInfo o el propio `MediaInfoReader` del SDK) para confirmar qué se escribió.

```csharp
var pipeline = new MediaBlocksPipeline();

pipeline.OnError += (sender, e) =>
{
    Console.WriteLine($"Error del pipeline: {e.Message}");
};

// Continuar con la configuración del pipeline...
```

## Especificaciones de Formato de Etiquetas

### Etiquetas ID3 (MP3)

- **ID3v1**: estructura básica de 128 bytes con campos limitados
- **ID3v2**: formato extensible compatible con Unicode, múltiples valores y frames personalizados
- **Frames comunes**: TIT2 (Título), TPE1 (Artista), TALB (Álbum), TDRC (Año), TCON (Género)

### Comentarios Vorbis (OGG)

- **Formato**: texto UTF-8 en formato NAME=VALUE
- **Campos estándar**: TITLE, ARTIST, ALBUM, DATE, GENRE, TRACKNUMBER
- **Flexible**: se permiten nombres de campo arbitrarios y múltiples valores

### Metadatos MP4 (M4A)

- **Átomos**: metadatos estilo iTunes almacenados en átomos MP4
- **Átomos comunes**: ©nam (Título), ©ART (Artista), ©alb (Álbum), ©day (Año)
- **Datos binarios**: la carátula incrustada va en el átomo `covr`

### Atributos ASF (WMV/WMA)

- **Estructura**: pares clave-valor en la cabecera ASF
- **Atributos estándar**: Title, Author, Copyright, Description
- **Extendido**: se admiten atributos personalizados

---

Esta guía cubre la escritura de metadatos de audio con el VisioForge Media Blocks SDK. La clase unificada `MediaFileTags` simplifica el código manteniendo intacto el formato nativo de cada contenedor (ID3, Vorbis Comments, átomos MP4, ASF). Para escenarios de procesamiento de audio más avanzados, consulte la [documentación completa del VisioForge Media Blocks SDK](../index.md).
