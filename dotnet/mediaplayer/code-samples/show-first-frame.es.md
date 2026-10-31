---
title: Mostrar primer fotograma de video con Media Player SDK .NET
description: Muestre el primer fotograma de video en aplicaciones WinForms, WPF y Consola con ejemplos de implementación en C# usando Media Player SDK.
tags:
  - Media Player SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Playback
  - Streaming
  - C#

---

# Mostrar el primer fotograma de archivos de video en aplicaciones .NET

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Descripción general

Cuando se desarrollan aplicaciones multimedia, a menudo es necesario previsualizar el contenido de video sin reproducir el archivo completo. Esta técnica es particularmente útil para crear galerías de miniaturas, pantallas de selección de video, o proporcionar a los usuarios una vista previa visual antes de comprometerse a ver un video.

## Guía de implementación

El Media Player SDK .NET proporciona una forma simple pero poderosa de mostrar el primer fotograma de cualquier archivo de video. Esto se logra a través de la propiedad `Play_PauseAtFirstFrame`, que cuando se establece en `true`, instruye al reproductor a pausar inmediatamente después de cargar el primer fotograma.

### Cómo funciona

Cuando la propiedad `Play_PauseAtFirstFrame` está habilitada:

1. El reproductor carga el archivo de video
2. Renderiza el primer fotograma en la superficie de visualización de video
3. Pausa automáticamente la reproducción
4. Mantiene el primer fotograma en pantalla hasta una acción posterior del usuario

Si esta propiedad no está habilitada (establecida en `false`), el reproductor procederá con la reproducción normal después de cargar.

## Implementación de código

### Ejemplo Básico

```cs
// crear reproductor y configurar el nombre del archivo
// ...

// establecer la propiedad en true
MediaPlayer1.Play_PauseAtFirstFrame = true;

// reproducir el archivo
await MediaPlayer1.PlayAsync();
```

Reanudar la reproducción desde el primer fotograma:

```cs
// reanudar reproducción
await MediaPlayer1.ResumeAsync();
```

## Aplicaciones prácticas

Esta característica es particularmente útil para:

- Proporcionar capacidades de vista previa en aplicaciones de edición de video
- Generar fotogramas de póster de video para aplicaciones de streaming
- Implementar funcionalidad de "pasar el cursor para previsualizar" en navegadores multimedia

## Componentes requeridos

Para implementar esta funcionalidad en su aplicación, necesitará:

- Paquete redistribuible base
- Paquete redistribuible del SDK

Para más información sobre cómo distribuir estos componentes con su aplicación, consulte: [¿Cómo pueden instalarse o desplegarse los redistribuibles requeridos en la PC del usuario?](../deployment.md)

## Recursos adicionales

Encuentre más ejemplos de código y ejemplos de implementación en nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

## Consideraciones técnicas

Cuando implemente esta característica, tenga en cuenta:

- La visualización del primer fotograma es casi instantánea para la mayoría de los formatos de video
- El uso de recursos es mínimo ya que el reproductor no almacena en buffer más allá del primer fotograma
- Funciona con todos los formatos de video soportados incluyendo MP4, MOV, AVI y más
- En `MediaPlayerCore`, la propiedad se aplica a un solo inicio y después vuelve a `false`; no se aplica con `ConsoleUsage`, cuyo `Play()` espera hasta el final del archivo, ni a una fuente en vivo que indica que no puede mostrar un fotograma en pausa, la cual se reproduce en su lugar
