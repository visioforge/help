---
title: Redimensionar, recortar y escalar video en vivo en C# .NET
description: Redimensiona y recorta video en vivo de webcams, pantallas y cámaras IP con Video Capture SDK — relación de aspecto y región de interés en C#.
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - NuGet
primary_api_classes:
  - VideoResizeSettings
  - VideoCropSettings

---

# Operaciones de redimensionado y recorte de video para desarrolladores .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al procesamiento de video

Al trabajar con flujos de video en aplicaciones .NET, controlar las dimensiones y el área de enfoque de tu video es esencial para crear aplicaciones profesionales. Esta guía explica cómo implementar operaciones de redimensionado y recorte en flujos de video provenientes de webcams, capturas de pantalla, cámaras IP y otras fuentes.

## Implementación de redimensionado de video

El redimensionado te permite estandarizar las dimensiones de video entre distintas fuentes, lo cual es particularmente útil cuando trabajas con múltiples entradas de cámara o cuando apuntas a formatos de salida específicos.

### Paso 1: habilitar la funcionalidad de redimensionado

Primero, habilita la funcionalidad de redimensionado o recorte en tu aplicación:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Paso 2: configurar los parámetros de redimensionado

Define el ancho y la altura deseados, y determina si quieres mantener la relación de aspecto con letterboxing:

```cs
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width = 640,
    Height = 480,
    LetterBox = true
};
```

### Paso 3: seleccionar el algoritmo de redimensionado apropiado

Elige el algoritmo que mejor se ajuste a tus requisitos de rendimiento y calidad:

```cs
// Video_Resize está tipado como la interfaz marcadora IVideoResizeSettings;
// Mode vive en la clase concreta VideoResizeSettings, así que hay que hacer cast antes de asignar.
var resize = (VideoResizeSettings)VideoCapture1.Video_Resize;
switch (cbResizeMode.SelectedIndex)
{
  case 0: resize.Mode = VideoResizeMode.NearestNeighbor; break;
  case 1: resize.Mode = VideoResizeMode.Bilinear; break;
  case 2: resize.Mode = VideoResizeMode.Bicubic; break;
  case 3: resize.Mode = VideoResizeMode.Lancroz; break;
}
```

## Implementación de recorte de video

El recorte te permite enfocarte en regiones de interés específicas en tu flujo de video, eliminando las áreas no deseadas del fotograma.

### Paso 1: habilitar la funcionalidad de recorte

De manera similar al redimensionado, primero habilita la funcionalidad de recorte:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Paso 2: definir la región de recorte

Especifica la región de recorte estableciendo los márgenes a eliminar de cada borde del fotograma de video:

```cs
VideoCapture1.Video_Crop = new VideoCropSettings(40, 0, 40, 0);
```

## Consideraciones de rendimiento

Al implementar operaciones de redimensionado y recorte en aplicaciones de producción, ten en cuenta lo siguiente:

- Las operaciones de redimensionado requieren recursos de CPU, especialmente a resoluciones más altas
- Los algoritmos más complejos (Bicubic, Lanczos) proporcionan mejor calidad pero requieren más potencia de procesamiento
- Para aplicaciones en tiempo real, equilibra calidad y rendimiento en función del hardware objetivo

## Dependencias requeridas

Asegúrate de que tu proyecto incluya los paquetes redistribuibles necesarios:

- Redist Video Capture [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Recursos adicionales

Para implementaciones más avanzadas y ejemplos de código, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) con numerosos ejemplos para desarrolladores .NET.
