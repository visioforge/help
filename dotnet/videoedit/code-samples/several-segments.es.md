---
title: Múltiples Segmentos desde un Archivo de Video
description: Extrae y combina múltiples segmentos del mismo archivo de video o audio en C# con guía paso a paso y ejemplos de código para .NET.
---

# Añadir Múltiples Segmentos desde un Único Archivo de Video en C#

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introducción

Al desarrollar aplicaciones de edición de video, a menudo necesitas extraer porciones específicas de un archivo de video y combinarlas en una nueva composición. Esta técnica es esencial para crear resúmenes de momentos destacados, eliminar secciones no deseadas o ensamblar una compilación de momentos clave de un video más largo.

Esta guía demuestra cómo extraer y combinar programáticamente múltiples segmentos del mismo archivo de video usando C#. Aprenderás el proceso paso a paso con ejemplos de código funcionales que puedes implementar en tus propias aplicaciones.

## ¿Por qué Extraer Múltiples Segmentos?

Extraer segmentos específicos de videos sirve muchos propósitos prácticos:

- Crear resúmenes de momentos destacados de grabaciones más largas
- Eliminar secciones no deseadas (anuncios, errores, contenido irrelevante)
- Ensamblar una compilación de momentos clave
- Crear trailers o vistas previas de contenido de larga duración
- Generar clips más cortos para redes sociales de videos más largos

## Descripción General de la Implementación

La implementación involucra tres pasos clave:

1. Definir los segmentos de tiempo que quieres extraer
2. Crear una fuente de video que incluya estos segmentos especificados
3. Añadir el archivo segmentado a tu línea de tiempo de edición

Desglosemos cada paso con ejemplos de código detallados y explicaciones.

## Implementación Detallada

### Paso 1: Definir tus Segmentos

Primero, necesitas especificar los tiempos de inicio y fin de cada segmento. Cada segmento está definido por un punto de inicio y duración, medidos en milisegundos.

```cs
// Definir múltiples segmentos desde un único archivo de video
FileSegment[] segments = new[] { 
    new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5000)),  // Primeros 5 segundos
    new FileSegment(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(10000))  // Desde marca de 3s a 13s
};
```

En este ejemplo, hemos definido dos segmentos:

- El primer segmento comienza al principio del video (0ms) y dura 5 segundos
- El segundo segmento comienza en la marca de 3 segundos y continúa por 10 segundos

Nota que los segmentos pueden superponerse, como se muestra en este ejemplo donde el segundo segmento comienza antes de que el primero termine. Esto puede ser útil para crear transiciones suaves o cuando quieres que ciertas porciones aparezcan múltiples veces.

### Paso 2: Crear una Fuente de Video con Segmentos

A continuación, crea un objeto VideoSource que incorpore tus segmentos definidos:

```cs
// Crear una fuente de video que incluya los segmentos especificados
VideoSource videoFile = new VideoSource(
    videoFileName,   // Ruta a tu archivo de video
    segments,        // Array de segmentos definidos arriba
    VideoEditStretchMode.Letterbox,  // Cómo manejar diferencias de relación de aspecto
    0,               // Ángulo de rotación (0 = sin rotación)
    1.0);            // Factor de velocidad (1.0 = velocidad normal)
```

El constructor de VideoSource toma varios parámetros:

- `videoFileName`: La ruta a tu archivo de video fuente
- `segments`: El array de objetos FileSegment que definiste en el Paso 1
- `VideoEditStretchMode`: Cómo manejar diferencias de relación de aspecto (Letterbox, Stretch, Crop)
- Ángulo de rotación (en grados): Usa 0 para sin rotación, o 90, 180, 270 para video rotado
- Factor de velocidad: Usa 1.0 para velocidad normal, valores menores a 1.0 para cámara lenta, mayores a 1.0 para cámara rápida

### Paso 3: Añadir a la Línea de Tiempo

Finalmente, añade la fuente de video segmentada a tu línea de tiempo de edición:

```cs
// Añadir el archivo segmentado a la línea de tiempo (pista 0)
VideoEdit1.Input_AddVideoFile(videoFile, 0);
```

El método `Input_AddVideoFile` toma dos parámetros:

- `videoFile`: El objeto VideoSource que creaste
- `0`: El número de pista donde colocar el video (0 es típicamente la pista de video principal)

## Trabajar con Segmentos de Audio

El mismo enfoque funciona para archivos de audio. Simplemente usa AudioSource en lugar de VideoSource:

```cs
// Definir tus segmentos de audio
FileSegment[] audioSegments = new[] { 
    new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(8000)),
    new FileSegment(TimeSpan.FromMilliseconds(15000), TimeSpan.FromMilliseconds(12000))
};

// Crear fuente de audio con segmentos
AudioSource audioFile = new AudioSource(
    audioFileName,
    audioSegments,
    1.0);  // Factor de velocidad

// Añadir a la línea de tiempo (pista de audio 0)
VideoEdit1.Input_AddAudioFile(audioFile, 0);
```

## Escenarios de Uso Avanzado

### Segmentos de Velocidad Variable

Puedes crear efectos interesantes variando el factor de velocidad para diferentes segmentos:

```cs
// Crear segmentos con diferentes velocidades
VideoSource slowMotionSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(3000)) },
    VideoEditStretchMode.Letterbox,
    0,
    0.5);  // Mitad de velocidad (cámara lenta)

VideoSource fastForwardSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(5000)) },
    VideoEditStretchMode.Letterbox,
    0,
    2.0);  // Doble velocidad

// Añadir segmentos a diferentes posiciones en la línea de tiempo
VideoEdit1.Input_AddVideoFile(slowMotionSegment, 0);
VideoEdit1.Input_AddVideoFile(fastForwardSegment, 0, TimeSpan.FromMilliseconds(3000));
```

### Combinar Múltiples Archivos con Segmentos

Puedes combinar segmentos de diferentes archivos creando múltiples objetos VideoSource:

```cs
// Crear segmentos de diferentes archivos
VideoSource file1Segments = new VideoSource(
    videoFileName1,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5000)) },
    VideoEditStretchMode.Letterbox,
    0,
    1.0);

VideoSource file2Segments = new VideoSource(
    videoFileName2,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(4000)) },
    VideoEditStretchMode.Letterbox,
    0,
    1.0);

// Añadir a la línea de tiempo en secuencia
VideoEdit1.Input_AddVideoFile(file1Segments, 0);
VideoEdit1.Input_AddVideoFile(file2Segments, 0, TimeSpan.FromMilliseconds(5000));
```

## Dependencias Requeridas

Para usar esta funcionalidad, necesitarás instalar los paquetes redistribuibles apropiados:

- Redistribuibles de Video Edit SDK:
  - [paquete x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [paquete x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

Para información sobre cómo instalar o desplegar estos redistribuibles en los PCs de tus usuarios, consulta la [guía de despliegue](../deployment.md).

## Conclusión

Extraer y combinar múltiples segmentos de un archivo de video es una técnica poderosa para crear contenido de video dinámico en tus aplicaciones. Siguiendo los pasos descritos en esta guía, puedes implementar esta funcionalidad en tus aplicaciones C# con mínimo esfuerzo.

Este enfoque te da control detallado sobre qué porciones de un video se incluyen en tu salida final, permitiendo posibilidades de edición creativas sin requerir herramientas complejas de edición de video manual.

---
Visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para más muestras de código y ejemplos.