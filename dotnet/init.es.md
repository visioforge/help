---
title: Guía de Configuración e Instalación del SDK .NET
description: Inicializar y desinicializar SDKs .NET para captura, edición y reproducción de video con DirectShow y motores X multiplataforma.
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

Necesitas inicializar el SDK antes de usar cualquier clase del SDK y desinicializar el SDK antes de que la aplicación termine.

Para inicializar el SDK, usa el siguiente código:

```csharp
VisioForge.Core.VisioForgeX.InitSDK();
```

Para desinicializar el SDK, usa el siguiente código:

```csharp
VisioForge.Core.VisioForgeX.DestroySDK();
```

Si el SDK no se desinicializa correctamente, la aplicación puede experimentar un bloqueo al salir debido a la incapacidad de finalizar uno de sus hilos. Este problema surge porque el SDK continúa operando, impidiendo que la aplicación se cierre correctamente. Para asegurar una salida limpia, es crucial desinicializar el SDK apropiadamente según el framework de UI que estés usando.

Para aplicaciones desarrolladas usando diferentes frameworks de UI, puedes desinicializar el SDK en el evento `FormClosing` u otro manejador de eventos relevante. Este enfoque asegura que el SDK se destruya correctamente antes de que la aplicación se cierre, permitiendo que todos los hilos terminen correctamente.

Además, el SDK puede destruirse desde cualquier hilo, proporcionando flexibilidad en cómo manejas el proceso de desinicialización. Para mejorar la experiencia del usuario y evitar que la UI se congele durante este proceso, puedes utilizar llamadas de API asíncronas. Al usar métodos async, permites que la desinicialización ocurra en segundo plano, manteniendo la interfaz de usuario responsiva y evitando cualquier posible retraso o problemas de congelamiento.

Implementar estas prácticas asegura que tu aplicación salga sin problemas sin bloquearse, proporcionando una experiencia fluida para los usuarios. Gestionar correctamente la desinicialización del SDK es crucial para mantener la estabilidad y rendimiento de tu aplicación.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.