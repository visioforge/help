---
title: Inicialización del SDK .NET para video multiplataforma
description: Inicializar y desinicializar SDKs .NET para captura, edición y reproducción de video con DirectShow y motores X multiplataforma.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing

---

# Inicialización

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tipos de Motores del SDK

Todos los SDKs contienen motores basados en DirectShow solo para Windows y motores X multiplataforma.

### Motores Solo para Windows

- VideoCaptureCore
- VideoEditCore
- MediaPlayerCore

### Motores X

- VideoCaptureCoreX
- VideoEditCoreX
- MediaPlayerCoreX
- MediaBlocksPipeline

Los motores X requieren pasos adicionales de inicialización y desinicialización.

## Inicialización y Desinicialización del SDK para Motores X

Todos los motores X (`VideoCaptureCoreX`, `VideoEditCoreX`, `MediaPlayerCoreX`, `MediaBlocksPipeline`) utilizan un único paso de inicialización compartido. Debes inicializar el SDK antes de usar cualquier clase del SDK y desinicializar el SDK antes de que la aplicación termine. Los núcleos DirectShow solo de Windows (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) no necesitan ningún paso de inicialización.

### Llamada a InitSDKAsync

Lo recomendado es la forma asíncrona, para que el arranque en frío de GStreamer (resolución de bibliotecas nativas, escaneo del registro de plugins) no congele el hilo que llama:

```csharp
await VisioForge.Core.VisioForgeX.InitSDKAsync();
```

La forma bloqueante es equivalente y puede llamarse desde cualquier hilo. Úsala directamente cuando la inicialización deba ejecutarse en un hilo específico (ver la nota a continuación):

```csharp
VisioForge.Core.VisioForgeX.InitSDK();
```

`InitSDKAsync` traslada `InitSDK` a un hilo del grupo de subprocesos mediante `Task.Run`. Los manejadores de excepciones no controladas de GStreamer / GLib se enlazan al hilo que primero desencadena la inicialización, así que si tu aplicación depende de que la inicialización se ejecute en un hilo concreto (por ejemplo, por estado GLib estático de subproceso), llama a `InitSDK()` directamente en ese hilo.

### Desinicialización

```csharp
VisioForge.Core.VisioForgeX.DestroySDK();
```

`DestroySDK` no tiene variante asíncrona y puede llamarse desde cualquier hilo.

Si el SDK no se desinicializa correctamente, la aplicación puede experimentar un bloqueo al salir debido a la incapacidad de finalizar uno de sus hilos. Este problema surge porque el SDK continúa operando, impidiendo que la aplicación se cierre correctamente. Para asegurar una salida limpia, es crucial desinicializar el SDK apropiadamente según el framework de UI que estés usando.

Para aplicaciones desarrolladas usando diferentes frameworks de UI, puedes desinicializar el SDK en el evento `FormClosing` u otro manejador de eventos relevante. Este enfoque asegura que el SDK se destruya correctamente antes de que la aplicación se cierre, permitiendo que todos los hilos terminen correctamente.

Además, el SDK puede destruirse desde cualquier hilo, proporcionando flexibilidad en cómo manejas el proceso de desinicialización. Como `DestroySDK` es síncrono, si quieres mantener la interfaz de usuario responsiva llámalo en un hilo en segundo plano (por ejemplo mediante `Task.Run`) — no existe una sobrecarga asíncrona de `DestroySDK`.

Implementar estas prácticas asegura que tu aplicación salga sin problemas sin bloquearse, proporcionando una experiencia fluida para los usuarios. Gestionar correctamente la desinicialización del SDK es crucial para mantener la estabilidad y rendimiento de tu aplicación.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.