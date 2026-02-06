---
title: Lectura de Información de Archivos de Medios en C#
description: Extrae códecs, resolución, tasa de frames, tasa de bits, duración y metadatos de archivos de video y audio usando MediaInfoReader en aplicaciones C#.
---

# Lectura de Información de Archivos de Medios en C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Acceder a información detallada incrustada dentro de archivos de medios es esencial para desarrollar aplicaciones sofisticadas como reproductores de medios, editores de video, sistemas de gestión de contenido y herramientas de análisis de archivos. Entender propiedades como códecs, resolución, tasa de frames, tasa de bits, duración y etiquetas incrustadas permite a los desarrolladores construir software más inteligente y amigable para el usuario.

Esta guía demuestra cómo leer información completa de archivos de video y audio usando C# y la clase `MediaInfoReader`. Las técnicas mostradas son aplicables a varios proyectos .NET y proporcionan una base para manejar archivos de medios programáticamente.

## ¿Por Qué Extraer Información de Archivos de Medios?

La información de archivos de medios sirve múltiples propósitos en el desarrollo de aplicaciones:

- **Experiencia de Usuario**: Mostrar detalles técnicos a usuarios en reproductores de medios
- **Verificaciones de Compatibilidad**: Verificar si los archivos cumplen con las especificaciones requeridas
- **Procesamiento Automatizado**: Configurar parámetros de codificación basándose en propiedades de origen
- **Organización de Contenido**: Catalogar bibliotecas de medios con metadatos precisos
- **Evaluación de Calidad**: Evaluar archivos de medios para problemas potenciales

## Guía de Implementación

Exploremos el proceso de extracción de información de archivos de medios en un enfoque paso a paso. Los ejemplos asumen una aplicación WinForms con un control `TextBox` llamado `mmInfo` para mostrar la información extraída.

### Paso 1: Inicializar el Lector de Información de Medios

El primer paso implica crear una instancia de la clase `MediaInfoReader`:

```csharp
// Importar el espacio de nombres necesario
using VisioForge.Core.MediaInfo; // Espacio de nombres para MediaInfoReader
using VisioForge.Core.Helpers;  // Espacio de nombres para TagLibHelper (opcional)

// Crear una instancia de MediaInfoReader
var infoReader = new MediaInfoReader();
```

Esta inicialización prepara al lector para procesar archivos de medios.

### Paso 2: Verificar la Reproducibilidad del Archivo (Opcional)

Antes de profundizar en el análisis detallado, a menudo es útil verificar si el archivo es compatible:

```csharp
// Definir variables para mantener información potencial de error
FilePlaybackError errorCode;
string errorText;

// Especificar la ruta al archivo de medios
string filename = @"C:\ruta\a\tu\archivodemedio.mp4"; // Reemplazar con tu ruta de archivo real

// Verificar si el archivo es reproducible
if (MediaInfoReader.IsFilePlayable(filename, out errorCode, out errorText))
{
    // Mostrar mensaje de éxito
    mmInfo.Text += "Estado: Este archivo parece ser reproducible." + Environment.NewLine;
}
else
{
    // Mostrar mensaje de error incluyendo el código de error y descripción
    mmInfo.Text += $"Estado: Este archivo podría no ser reproducible. Error: {errorCode} - {errorText}" + Environment.NewLine;
}

mmInfo.Text += "------------------------------------" + Environment.NewLine;
```

Esta verificación proporciona retroalimentación temprana sobre la integridad y compatibilidad del archivo.

### Paso 3: Extraer Información Detallada de Flujos

Ahora podemos extraer los metadatos ricos del archivo:

```csharp
try
{
    // Asignar el nombre de archivo al lector
    infoReader.Filename = filename;

    // Leer la información del archivo (true para análisis completo)
    infoReader.ReadFileInfo(true);

    // Procesar Flujos de Video
    mmInfo.Text += $"Se encontraron {infoReader.VideoStreams.Count} flujo(s) de video." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.VideoStreams.Count; i++)
    {
        var stream = infoReader.VideoStreams[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Flujo de Video #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Códec: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Duración: {stream.Duration}" + Environment.NewLine;
        mmInfo.Text += $"  Dimensiones: {stream.Width}x{stream.Height}" + Environment.NewLine;
        mmInfo.Text += $"  FOURCC: {stream.FourCC}" + Environment.NewLine;
        
        if (stream.AspectRatio != null && stream.AspectRatio.Item1 > 0 && stream.AspectRatio.Item2 > 0)
        {
             mmInfo.Text += $"  Relación de Aspecto: {stream.AspectRatio.Item1}:{stream.AspectRatio.Item2}" + Environment.NewLine;
        }
        
        mmInfo.Text += $"  Tasa de Frames: {stream.FrameRate:F2} fps" + Environment.NewLine;
        mmInfo.Text += $"  Tasa de Bits: {stream.Bitrate / 1000.0:F0} kbps" + Environment.NewLine;
        mmInfo.Text += $"  Conteo de Frames: {stream.FramesCount}" + Environment.NewLine;
    }

    // Procesar Flujos de Audio
    mmInfo.Text += Environment.NewLine;
    mmInfo.Text += $"Se encontraron {infoReader.AudioStreams.Count} flujo(s) de audio." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.AudioStreams.Count; i++)
    {
        var stream = infoReader.AudioStreams[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Flujo de Audio #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Códec: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Info del Códec: {stream.CodecInfo}" + Environment.NewLine;
        mmInfo.Text += $"  Duración: {stream.Duration}" + Environment.NewLine;
        mmInfo.Text += $"  Tasa de Bits: {stream.Bitrate / 1000.0:F0} kbps" + Environment.NewLine;
        mmInfo.Text += $"  Canales: {stream.Channels}" + Environment.NewLine;
        mmInfo.Text += $"  Tasa de Muestreo: {stream.SampleRate} Hz" + Environment.NewLine;
        mmInfo.Text += $"  Bits por Muestra (BPS): {stream.BPS}" + Environment.NewLine;
        mmInfo.Text += $"  Idioma: {stream.Language}" + Environment.NewLine;
    }

    // Procesar Flujos de Subtítulos
    mmInfo.Text += Environment.NewLine;
    mmInfo.Text += $"Se encontraron {infoReader.Subtitles.Count} flujo(s) de subtítulos." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.Subtitles.Count; i++)
    {
        var stream = infoReader.Subtitles[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Flujo de Subtítulos #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Códec/Formato: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Nombre: {stream.Name}" + Environment.NewLine;
        mmInfo.Text += $"  Idioma: {stream.Language}" + Environment.NewLine;
    }
}
catch (Exception ex)
{
    // Manejar errores potenciales durante la lectura del archivo
    mmInfo.Text += $"{Environment.NewLine}Error leyendo info del archivo: {ex.Message}{Environment.NewLine}";
}
finally
{
    // Importante: Liberar el lector para liberar manejadores de archivo y recursos
    infoReader.Dispose();
}
```

El código itera a través de cada colección (`VideoStreams`, `AudioStreams` y `Subtitles`), extrayendo y mostrando información relevante para cada flujo encontrado.

### Paso 4: Extraer Etiquetas de Metadatos

Más allá de la información técnica del flujo, los archivos de medios a menudo contienen etiquetas de metadatos:

```csharp
// Leer Etiquetas de Metadatos
mmInfo.Text += Environment.NewLine + "--- Etiquetas de Metadatos ---" + Environment.NewLine;
try
{
    // Usar TagLibHelper para leer etiquetas del archivo
    var tags = TagLibHelper.ReadTags(filename);

    // Verificar si las etiquetas se leyeron exitosamente
    if (tags != null)
    {
        mmInfo.Text += $"Título: {tags.Title}" + Environment.NewLine;
        mmInfo.Text += $"Artista(s): {string.Join(", ", tags.Performers ?? new string[0])}" + Environment.NewLine;
        mmInfo.Text += $"Álbum: {tags.Album}" + Environment.NewLine;
        mmInfo.Text += $"Año: {tags.Year}" + Environment.NewLine;
        mmInfo.Text += $"Género: {string.Join(", ", tags.Genres ?? new string[0])}" + Environment.NewLine;
        mmInfo.Text += $"Comentario: {tags.Comment}" + Environment.NewLine;
    }
    else
    {
        mmInfo.Text += "No se encontraron etiquetas de metadatos estándar o no son legibles." + Environment.NewLine;
    }
}
catch (Exception ex)
{
    // Manejar errores durante la lectura de etiquetas
    mmInfo.Text += $"Error leyendo etiquetas: {ex.Message}" + Environment.NewLine;
}
```

## Mejores Prácticas para Análisis de Archivos de Medios

Al implementar análisis de archivos de medios en tus aplicaciones, considera estas mejores prácticas:

### Manejo de Errores

Siempre envuelve las operaciones de archivo en bloques try-catch apropiados. Los archivos de medios pueden estar corruptos, inaccesibles o en formatos inesperados, lo que podría causar excepciones.

```csharp
try {
    // Operaciones con archivos de medios
}
catch (Exception ex) {
    // Registrar error y proporcionar retroalimentación al usuario
}
```

### Gestión de Recursos

Libera apropiadamente los objetos que acceden a recursos de archivo para prevenir problemas de bloqueo de archivos:

```csharp
using (var infoReader = new MediaInfoReader())
{
    // Usar el lector
}
// O manualmente en un bloque finally
try {
    // Operaciones
}
finally {
    infoReader.Dispose();
}
```

### Consideraciones de Rendimiento

Para bibliotecas de medios grandes, considera:

1. Implementar mecanismos de caché para análisis repetidos
2. Usar hilos de fondo para procesamiento para mantener la UI responsiva
3. Limitar la profundidad del análisis para escaneos rápidos iniciales

## Componentes Requeridos

Para una implementación exitosa, asegúrate de que tu proyecto incluya las dependencias necesarias según lo especificado en la documentación del SDK.

## Conclusión

Extraer información de archivos de medios es una capacidad poderosa para desarrolladores que construyen aplicaciones que trabajan con contenido de audio y video. Con las técnicas descritas en esta guía, puedes acceder a propiedades técnicas detalladas y etiquetas de metadatos para mejorar la funcionalidad de tu aplicación.

La clase `MediaInfoReader` proporciona una manera conveniente y eficiente de extraer los metadatos necesarios, permitiéndote construir características de manejo de medios más sofisticadas en tus aplicaciones C#.

Para escenarios más avanzados, explora las capacidades completas del SDK y consulta la documentación detallada. Puedes encontrar ejemplos de código adicionales y muestras en GitHub para expandir aún más tus capacidades de procesamiento de archivos de medios.
