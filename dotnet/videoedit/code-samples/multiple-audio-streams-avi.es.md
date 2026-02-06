---
title: Añadir Múltiples Pistas de Audio a AVI en .NET
description: Implementa múltiples pistas de audio en archivos AVI usando Video Edit SDK para .NET con guía paso a paso y ejemplos de código C# para multi-idioma.
---

# Añadir Múltiples Pistas de Audio a Archivos AVI en .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introducción

Las múltiples pistas de audio te permiten incluir diferentes pistas de idioma, comentarios u opciones de música dentro de un único archivo de video. Esta funcionalidad es esencial para crear contenido multilingüe o proporcionar experiencias de audio alternativas para los espectadores.

## Detalles de Implementación

Al crear múltiples pistas de audio en un archivo AVI, necesitas añadir cada fuente de audio a la línea de tiempo usando parámetros de destino específicos. Este enfoque asegura que cada pista de audio esté correctamente indexada y accesible durante la reproducción.

## Ejemplo de Código

El siguiente ejemplo en C# demuestra cómo añadir dos pistas de audio diferentes a un archivo AVI:

```cs
var videoSource = new VideoSource("video1.avi");
var audioSource1 = new AudioSource("video1.avi");
var audioSource2 = new AudioSource("audio2.mp3"); 

VideoEdit1.Input_Clear_List();
VideoEdit1.Input_AddVideoFile(videoSource);
VideoEdit1.Input_AddAudioFile(audioSource1, targetStreamIndex: 0);
VideoEdit1.Input_AddAudioFile(audioSource2, targetStreamIndex: 1);
```

## Explicación de Parámetros Clave

- `targetStreamIndex`: Define a qué índice de pista de audio se asignará la fuente
- La primera pista de audio usa índice 0, la segunda usa índice 1, y así sucesivamente
- Puedes añadir tantas pistas de audio como necesites usando valores de índice incrementales

## Dependencias Requeridas

Para implementar esta funcionalidad, necesitarás:

- Redistribuibles de Video Edit SDK:
  - [versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Información de Despliegue

Para detalles sobre la instalación o despliegue de las dependencias requeridas en sistemas de usuarios finales, consulta nuestra [guía de despliegue](../deployment.md).

---
Encuentra ejemplos de código adicionales y detalles de implementación en nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).