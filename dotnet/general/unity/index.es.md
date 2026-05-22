---
title: Reproducción de video y RTSP en Unity con Media Blocks SDK
description: Añade reproducción de video y streaming de cámara RTSP a Unity 6 con VisioForge Media Blocks SDK .NET — un .unitypackage autónomo y listo para importar.
sidebar_label: Unity
order: 50
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - RTSP
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - BufferSinkBlock
  - UniversalSourceBlock
  - RTSPSourceBlock
  - AudioRendererBlock
---

# Reproducción de video y streaming RTSP en Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El **Media Blocks SDK .NET** se distribuye con un **`.unitypackage`** listo para importar que aporta
reproducción de archivos de video y streaming en vivo de cámaras RTSP / IP a **Unity 6** en
**Windows x64**. Impórtalo, pulsa **Play** y el video se renderiza en un `RawImage` de Unity.

Para instalar el paquete, consulta **[Instalar el Media Blocks SDK en Unity](../../install/unity.md)**.
Esta guía se centra en cómo funciona la integración y cómo usar los dos ejemplos incluidos.

!!! tip "Agentes de programación con IA: usa el servidor MCP de VisioForge"
    ¿Lo estás construyendo con **Claude Code**, **Cursor** u otro agente de programación con IA?
    Conéctate al [servidor MCP público de VisioForge](../mcp-server-usage.md) en
    `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API y ejemplos de código
    verificados.

!!! info "El SDK completo está disponible — los ejemplos son solo un punto de partida"
    El paquete incluye el **Media Blocks SDK .NET completo**. Las dos escenas incluidas
    (`SimplePlayer` y `RTSPViewer`) son ejemplos para que arranques rápidamente — tus scripts tienen
    acceso a la **API completa de Media Blocks**: captura y múltiples tipos de fuente, decodificadores
    y codificadores, procesamiento y efectos de audio/video, mezcla y composición, grabación a
    archivo y salida de streaming en red. Construye cualquier pipeline que tu aplicación necesite.
    Consulta la [documentación del Media Blocks SDK .NET](../../mediablocks/index.md) para el catálogo
    completo de bloques.

## Ejemplos

El paquete incluye dos escenas listas en `Assets/Scenes/`. Abre una en la ventana **Project**
(haz doble clic en ella — no te quedes en la escena predeterminada vacía) y pulsa **▶ Play**:

- **[Reproducir un archivo multimedia](simple-player.md)** — la escena `SimplePlayer`, reproducción de archivos de video locales.
- **[Ver una cámara RTSP](rtsp-viewer.md)** — la escena `RTSPViewer`, stream en vivo de cámara RTSP / IP.

!!! tip "El RawImage se ve vacío hasta que pulsas Play"
    La textura de video se crea en tiempo de ejecución, por lo que el `RawImage` está en blanco
    (blanco) en el modo de edición. Eso es lo esperado — no se dibuja nada hasta que el pipeline
    arranca.

## Cómo funciona el renderizado

Dos ayudantes compartidos se encargan de la configuración y el renderizado por ti, de modo que cada
script de reproductor es solo el pipeline de Media Blocks:

1. **`VisioForgeEnvironment.Configure()`** se ejecuta automáticamente antes de cargar la primera
   escena y prepara el runtime nativo incluido — no gestionas ninguna dependencia ni ruta nativa.
2. **`VisioForgeEnvironment.InitializeSdk()`** inicializa el SDK una sola vez. Llámalo antes de
   construir un pipeline (los reproductores de ejemplo lo llaman en `Start()`).
3. Cada reproductor construye un pipeline que termina en un **`BufferSinkBlock(VideoFormatX.RGBA)`**;
   su evento `OnVideoFrameBuffer` entrega los fotogramas de video a **`VisioForgeVideoView`**.
4. **`VisioForgeVideoView`** sube cada fotograma a un `Texture2D` de Unity en el hilo principal y
   aplica el modo de aspecto, de modo que el video aparece en tu `RawImage`. No escribes ningún
   código de textura — solo lo adjuntas (los reproductores de ejemplo lo hacen por ti).

### Ciclo de vida en el Editor

El SDK se inicializa una vez por proceso y se reutiliza entre sesiones de Play/Stop en el Editor. De
ahí se derivan dos puntos:

- **Mantén Disable Domain Reload activado** para que al volver a entrar en el modo Play se reutilice
  el SDK ya inicializado. Con esa opción desactivada, el Editor puede colgarse al salir del modo
  Play.
- Los reproductores de ejemplo eliminan únicamente el pipeline de la reproducción actual en Stop
  (`StopAsync`) y, de forma intencionada, **no** cierran todo el SDK — mantén ese patrón en tus
  propios scripts.

Si encuentras inestabilidad tras una recompilación de scripts, reinicia el Editor.

## Preguntas frecuentes

### ¿Obtengo el Media Blocks SDK completo, o solo la reproducción?

El SDK completo. Las dos escenas de ejemplo son puntos de partida; tus scripts tienen acceso a toda
la API del Media Blocks SDK .NET — captura, múltiples tipos de fuente, decodificadores y
codificadores, procesamiento y efectos de audio/video, mezcla y composición, grabación a archivo y
streaming en red.

### ¿Puedo renderizar el video en mi propia UI en lugar de las escenas de ejemplo?

Sí. Añade un `RawImage`, adjunta `MediaBlocksPlayer` (archivo) o `RTSPViewerPlayer` (RTSP), o
construye tu propio pipeline y alimenta un `BufferSinkBlock` a `VisioForgeVideoView`. La subida de la
textura, el manejo del aspecto y el volteo se gestionan por ti.

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — configuración completa, paso a paso
- [Reproducir un archivo multimedia en Unity](simple-player.md) — el ejemplo de reproducción de archivos
- [Ver una cámara RTSP en Unity](rtsp-viewer.md) — el ejemplo en vivo de cámara RTSP / IP
- [Visión general del Media Blocks SDK .NET](../../mediablocks/index.md) — el catálogo completo de bloques y la guía de pipelines
- [Guía de streaming RTSP](../network-streaming/rtsp.md) — RTSP en los SDKs .NET de VisioForge
- [Directorio de marcas de cámaras IP](../../camera-brands/index.md) — URLs y ajustes de cámaras probadas
