---
title: Crear Video desde Múltiples Fuentes .NET
description: Combina múltiples archivos de video y audio en una única salida sin recodificación usando C# con guía paso a paso para mezclar pistas.
---

# Crear Nuevos Archivos desde Múltiples Fuentes Sin Recodificación

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introducción

Al desarrollar aplicaciones multimedia, puede que necesites combinar contenido de diferentes archivos. Esta guía demuestra cómo mezclar pistas de múltiples fuentes de video y audio en un único archivo de salida sin pérdida de calidad por recodificación.

## Beneficios de Trabajar con Múltiples Fuentes

- Preservar la calidad original de todos los archivos fuente
- Combinar pistas de audio de diferentes fuentes
- Añadir música de fondo a archivos de video
- Crear contenido multilingüe con diferentes pistas de audio
- Ahorrar tiempo de procesamiento evitando recodificación innecesaria

## Implementación Paso a Paso

### 1. Inicializar la Colección de Pistas

Primero, crea una lista para contener todas las referencias de pistas:

```cs
var streams = new List();
```

### 2. Añadir Pista de Video

Añade una pista de video desde tu primer archivo fuente. El ID "v" designa esto como el componente de video:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video.avi",
                ID = "v"
});
```

### 3. Añadir Pista de Audio Principal

Incorpora una pista de audio desde un archivo MP3. El ID "a" identifica esto como un componente de audio:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!sophie.mp3",
                ID = "a"
});
```

### 4. Añadir Pistas de Audio Adicionales

Puedes añadir más pistas de audio desde otros archivos de video. Nuevamente, usa el ID "a" para especificar esto como un componente de audio:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video2.avi",
                ID = "a"
});
```

### 5. Procesar y Generar Salida

Finalmente, combina todas las pistas en un único archivo de salida. Establecer el segundo parámetro en "true" asegura que la duración de salida coincida con la pista más corta, previniendo problemas de reproducción:

```cs
VideoEdit1.FastEdit_MuxStreams(streams, true, outputFile);
```

## Consideraciones Técnicas Importantes

Al combinar pistas de múltiples fuentes, ten en cuenta:

- Los formatos fuente deben ser compatibles con el formato de contenedor de salida
- La compatibilidad de códec de audio debe verificarse de antemano
- La sincronización de pistas puede requerir configuración adicional en escenarios complejos
- Algunos reproductores pueden tener problemas si las duraciones de las pistas varían significativamente

## Dependencias Requeridas

Para implementar esta funcionalidad, necesitarás referenciar:

- Redistribuible del SDK
- Redistribuible FFMPEG [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)

Para más información sobre cómo desplegar estas dependencias para usuarios finales, consulta [nuestra guía de despliegue](../deployment.md).

## Recursos Adicionales

Visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para muestras de código adicionales y ejemplos de implementación.
