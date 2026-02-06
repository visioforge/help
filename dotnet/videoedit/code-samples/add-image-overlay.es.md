---
title: Superposiciones de Imagen en Videos con .NET
description: Añade superposiciones de imagen a videos con guía paso a paso. Incluye ejemplos de código para posicionamiento, transparencia y temporización.
---

# Añadir Superposiciones de Imagen a Videos en .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introducción a las Superposiciones de Imagen

Nuestro SDK .NET proporciona una potente funcionalidad para añadir superposiciones de imagen a tus proyectos de video. Con esta característica, los desarrolladores pueden integrar sin problemas logotipos, marcas de agua, gráficos y otros elementos visuales en el contenido de video. El SDK ofrece amplias opciones de personalización incluyendo posicionamiento preciso, ajuste de transparencia y control de temporización.

## Formatos de Archivo de Imagen Soportados

El SDK es compatible con todos los formatos de imagen estándar utilizados en la producción de video profesional:

- BMP (Mapa de bits)
- GIF (Formato de Intercambio de Gráficos)
- JPEG/JPG (Grupo Conjunto de Expertos Fotográficos)
- PNG (Gráficos de Red Portátil)
- TIFF (Formato de Archivo de Imagen Etiquetado)

## Guía de Implementación

A continuación encontrarás ejemplos de código detallados que demuestran cómo implementar superposiciones de imagen en tus aplicaciones de procesamiento de video usando nuestro SDK.

### Usando el Motor VideoEditCoreX

El siguiente ejemplo de código demuestra cómo añadir una superposición de imagen con posicionamiento personalizado, transparencia y temporización usando el motor VideoEditCoreX:

```cs
// añadir una superposición de imagen a los efectos de la fuente de video desde un archivo PNG
var imageOverlay = new ImageOverlayVideoEffect("logo.png");

// establecer posición
imageOverlay.X = 50;
imageOverlay.Y = 50;

// establecer alfa
imageOverlay.Alpha = 0.5;

// establecer tiempo de inicio y tiempo de fin
imageOverlay.StartTime = TimeSpan.FromSeconds(0);
imageOverlay.StopTime = TimeSpan.FromSeconds(5);

// añadir fuente de video a la línea de tiempo
VideoEdit1.Video_Effects.Add(imageOverlay);
```

### Usando el Motor VideoEditCore

Para desarrolladores que trabajan con el motor VideoEditCore, aquí está cómo lograr la misma funcionalidad:

```cs
var effect = new VideoEffectImageLogo(true, name);

   // establecer posición
   effect.Left = 50;
   effect.Top = 50;

   // establecer alfa (0..255)
   effect.TransparencyLevel = 127;

   // establecer tiempo de inicio y tiempo de fin
   effect.StartTime = TimeSpan.FromSeconds(5);
   effect.StopTime = TimeSpan.FromSeconds(15);

VideoEdit1.Video_Effects_Add(effect);
```

## Opciones de Configuración Avanzadas

Al implementar superposiciones de imagen, considera estas opciones de configuración adicionales:

- **Posicionamiento**: Ajusta los valores X/Y o Left/Top para colocar tu superposición con precisión
- **Transparencia**: Configura Alpha o TransparencyLevel para controlar la opacidad de la superposición
- **Temporización**: Establece StartTime y StopTime para determinar cuándo aparece y desaparece la superposición
- **Tamaño**: Puedes redimensionar las superposiciones para ajustarse a tus requisitos específicos

## Recursos Adicionales

Para más ejemplos de código y guías de implementación, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
