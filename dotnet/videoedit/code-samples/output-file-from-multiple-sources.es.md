---
title: Mezclar pistas de video y audio de múltiples fuentes .NET
description: Combine múltiples archivos de video y audio en una única salida sin recodificación usando C# con guía paso a paso para mezclar pistas.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - AVI
  - C#
  - NuGet

---

# Crear nuevos archivos desde múltiples fuentes sin recodificación

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introducción

Al desarrollar aplicaciones multimedia, puede que necesite combinar contenido de diferentes archivos. Esta guía demuestra cómo mezclar pistas de múltiples fuentes de video y audio en un único archivo de salida sin pérdida de calidad por recodificación.

## Beneficios de trabajar con múltiples fuentes

- Preservar la calidad original de todos los archivos fuente
- Combinar pistas de audio de diferentes fuentes
- Añadir música de fondo a archivos de video
- Crear contenido multilingüe con diferentes pistas de audio
- Ahorrar tiempo de procesamiento evitando recodificación innecesaria

## Implementación paso a paso

### 1. Inicializar la colección de pistas

Primero, cree una lista para contener todas las referencias de pistas:

```cs
var streams = new List<FFMPEGStream>();
```

### 2. Añadir pista de video

Añada una pista de video desde su primer archivo fuente. El ID "v" designa esto como el componente de video:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video.avi",
                ID = "v"
});
```

### 3. Añadir pista de audio principal

Incorpora una pista de audio desde un archivo MP3. El ID "a" identifica esto como un componente de audio:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!sophie.mp3",
                ID = "a"
});
```

### 4. Añadir pistas de audio adicionales

Puede añadir más pistas de audio desde otros archivos de video. Nuevamente, use el ID "a" para especificar esto como un componente de audio:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video2.avi",
                ID = "a"
});
```

### 5. Procesar y generar salida

Finalmente, combine todas las pistas en un único archivo de salida. Establecer el segundo parámetro en "true" asegura que la duración de salida coincida con la pista más corta, previniendo problemas de reproducción:

```cs
await VideoEdit1.FastEdit_MuxStreamsAsync(streams, true, outputFile);
```

## Consideraciones técnicas importantes

Al combinar pistas de múltiples fuentes, tenga en cuenta:

- Los formatos fuente deben ser compatibles con el formato de contenedor de salida
- La compatibilidad de códec de audio debe verificarse de antemano
- La sincronización de pistas puede requerir configuración adicional en escenarios complejos
- Algunos reproductores pueden tener problemas si las duraciones de las pistas varían significativamente

## Dependencias requeridas

Para implementar esta funcionalidad, necesitará referenciar:

- Redistribuible del SDK
- Redistribuible FFMPEG [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)

Para más información sobre cómo desplegar estas dependencias para usuarios finales, consulte [nuestra guía de despliegue](../deployment.md).

## Recursos adicionales

Visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para muestras de código adicionales y ejemplos de implementación.
