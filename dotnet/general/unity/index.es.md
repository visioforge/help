---
title: Reproducción de video y RTSP en Unity con Media Blocks
description: Añade reproducción de video y RTSP en Unity 6 con VisioForge Media Blocks SDK .NET — .unitypackage para Windows, Android, macOS e iOS.
sidebar_label: Unity
order: 50
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Android
  - macOS
  - iOS
  - RTSP
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - BufferSinkBlock
  - UniversalSourceBlock
  - RTSPSourceBlock
  - AudioRendererBlock
  - VisioForgeEnvironment
---

# Reproducción de video y streaming RTSP en Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

El **Media Blocks SDK .NET** se distribuye con un **`.unitypackage`** multiplataforma listo
para importar que aporta reproducción de archivos de video, streaming RTSP / IP en vivo y el
resto del pipeline de Media Blocks a **Unity 6**. El mismo paquete apunta a cuatro
plataformas — **Windows x64**, **Android (IL2CPP arm64)**, **macOS Standalone
(Universal arm64+x86_64)** e **iOS Standalone (dispositivo arm64)**. Importa una vez, cambia
Build Target, compila.

Para instalar el paquete, consulta
**[Instalar el Media Blocks SDK en Unity](../../install/unity.md)**. Para el atajo de cinco
pasos, consulta la **[Guía rápida](getting-started.md)**.

!!! tip "Agentes de programación con IA: usa el servidor MCP de VisioForge"
    ¿Lo estás construyendo con **Claude Code**, **Cursor** u otro agente de programación con
    IA? Conéctate al [servidor MCP público de VisioForge](../mcp-server-usage.md) en
    `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API y ejemplos de código
    verificados.

!!! info "El SDK completo está disponible — los ejemplos son solo un punto de partida"
    El paquete incluye el **Media Blocks SDK .NET completo**. Las dos escenas incluidas
    (`SimplePlayer` y `RTSPViewer`) son ejemplos para que arranques rápidamente — tus scripts
    tienen acceso a la **API completa de Media Blocks**: captura y múltiples tipos de fuente,
    decodificadores y codificadores, procesamiento y efectos de audio/video, mezcla y
    composición, grabación a archivo y salida de streaming en red. Construye cualquier pipeline
    que tu aplicación necesite. Consulta la
    [documentación del Media Blocks SDK .NET](../../mediablocks/index.md) para el catálogo
    completo de bloques.

## Empaquetado acumulativo por plataforma

El `.unitypackage` distribuido es **acumulativo** — un archivo lleva los ensamblados
gestionados más cada runtime nativo, y los metadatos `PluginImporter` por archivo de Unity
eligen la copia correcta al cambiar Build Target. No hay descarga por plataforma.

| Plataforma | Runtime nativo | Arquitectura | Perfil de build |
|---|---|---|---|
| Windows | instalación plana de GStreamer en `StreamingAssets/VisioForge/x64/` | x86_64 | [Compilar para Windows](windows.md) |
| Android | `libgstreamer_android.so` monolítico + AAR Java | arm64-v8a | [Compilar para Android](android.md) |
| macOS | ~300 dylibs separados en `Plugins/macOS/` | Universal arm64 + x86_64 | [Compilar para macOS](macos.md) |
| iOS | `GStreamerX.framework` embebido (plugins registrados estáticamente) | dispositivo arm64 | [Compilar para iOS](ios.md) |

Los cuatro flavors comparten la misma superficie gestionada — `MediaBlocksPipeline`,
`BufferSinkBlock`, `RTSPSourceBlock`, `UniversalSourceBlock`, cada efecto, cada codificador,
cada sink. Lo único específico por plataforma que tu script ve es el valor de
`Application.platform` en tiempo de ejecución.

## Ejemplos

El paquete incluye escenas listas en `Assets/Scenes/`. Abre una en la ventana **Project**
(haz doble clic en ella — no te quedes en la escena predeterminada vacía) y pulsa **▶ Play**:

- **[Reproducir un archivo multimedia](simple-player.md)** — dos escenas: la de bajo nivel
  `SimplePlayer` (`MediaBlocksPipeline`) y la de alto nivel `MediaPlayerX` (`MediaPlayerCoreX`, con
  búsqueda/pausa/volumen).
- **[Ver una cámara RTSP](rtsp-viewer.md)** — dos escenas: la de bajo nivel `RTSPViewer`
  (`MediaBlocksPipeline`) y la de alto nivel `IPCameraX` (`VideoCaptureCoreX`, con grabación
  opcional).
- **[Capturar una webcam](video-capture-x.md)** — la escena `VideoCaptureX`: webcam + micrófono
  local con grabación MP4 (`VideoCaptureCoreX`, Windows / macOS).
- **[Editar y renderizar](video-edit-x.md)** — la escena `VideoEditX`: una línea de tiempo de varios
  clips previsualizada en vivo y luego renderizada a MP4 (`VideoEditCoreX`).

Las escenas de bajo nivel usan la API `MediaBlocksPipeline`; las escenas CoreX de alto nivel
renderizan en un `RawImage` mediante el evento exclusivo de Unity `OnVideoFrameUnity` (RGBA32
empaquetado de forma compacta, listo para `Texture2D`).

!!! tip "El RawImage se ve vacío hasta que pulsas Play"
    La textura de video se crea en tiempo de ejecución, por lo que el `RawImage` está en
    blanco (blanco) en el modo de edición. Es lo esperado — no se dibuja nada hasta que el
    pipeline arranca.

## Cómo funciona el rendering

Dos helpers compartidos manejan la configuración y el rendering por ti, así que cada script
player es solo el pipeline de Media Blocks:

1. **`VisioForgeEnvironment.Configure()`** se ejecuta automáticamente antes de que cargue la
   primera escena y prepara el runtime nativo incluido para la plataforma actual — ruta de
   búsqueda DLL de Windows, hints del cargador dylib de macOS, bootstrap Java de Android o
   puesta en marcha del framework de iOS. No gestionas ninguna dependencia nativa ni rutas. La
   historia completa está en [Bootstrap y ciclo de vida](bootstrap.md).
2. **`VisioForgeEnvironment.InitializeSdk()`** inicializa el SDK una vez. Llámalo antes de
   construir un pipeline (los players de ejemplo lo llaman en `Start()`).
3. Cada player construye un pipeline que termina en un
   **`BufferSinkBlock(VideoFormatX.RGBA)`**; su evento `OnVideoFrameBuffer` entrega fotogramas
   de video a **`VisioForgeVideoView`**.
4. **`VisioForgeVideoView`** sube cada fotograma a un `Texture2D` de Unity en el hilo
   principal y aplica el modo de aspecto, así el video aparece en tu `RawImage`. No escribes
   código de textura — solo adjúntalo (los players de ejemplo lo hacen por ti).

### Ciclo de vida del Editor

El SDK se inicializa una vez por proceso y se reutiliza entre sesiones Play/Stop en el Editor.
Dos puntos derivan de eso:

- **Mantén Disable Domain Reload activado** para que volver a entrar en modo Play reutilice el
  SDK inicializado. Con él desactivado, el Editor puede colgarse al salir del modo Play.
- Los players de ejemplo solo eliminan el pipeline por-Play en Stop (`StopAsync`) y
  **intencionadamente no** cierran todo el SDK — sigue ese patrón en tus propios scripts.

Si encuentras inestabilidad tras una recompilación de scripts, reinicia el Editor. Consulta
[Bootstrap y ciclo de vida](bootstrap.md#el-ciclo-de-vida-del-editor) para la razón de fondo.

## Preguntas frecuentes

### ¿Obtengo el Media Blocks SDK completo, o solo la reproducción?

El SDK completo. Las dos escenas de ejemplo son puntos de partida; tus scripts tienen acceso a
toda la API del Media Blocks SDK .NET — captura, múltiples tipos de fuente, decodificadores y
codificadores, procesamiento y efectos de audio/video, mezcla y composición, grabación a
archivo y salida de streaming en red.

### ¿Puedo renderizar video en mi propia UI en lugar de las escenas de ejemplo?

Sí. Añade un `RawImage`, adjunta `MediaBlocksPlayer` (archivo) o `RTSPViewerPlayer` (RTSP), o
construye tu propio pipeline y alimenta un `BufferSinkBlock` a `VisioForgeVideoView`. La
subida de textura, el manejo de aspecto y el flip se gestionan por ti.

### ¿Se usa el mismo paquete para cada plataforma?

Sí — un `.unitypackage` acumulativo contiene los runtimes nativos de Windows, Android, macOS e
iOS más un único conjunto de ensamblados gestionados `netstandard2.1`. Unity elige el slot
correcto en tiempo de build a partir de los metadatos `PluginImporter` por archivo; no
importas un paquete separado por plataforma.

### ¿Puedo ver qué rama de plataforma se está ejecutando?

Sí. `VisioForgeEnvironment.Configure()` registra una de
`[VisioForge] Environment configured. Natives: <path>` (Windows / macOS),
`[VisioForge] Android GStreamer bootstrap complete.` o
`[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).` en la Consola.
Usa esa línea para confirmar qué rama corrió.

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — configuración completa,
  paso a paso
- [Guía rápida](getting-started.md) — el camino en cinco pasos a un video reproduciéndose
- [Bootstrap y ciclo de vida](bootstrap.md) — qué hacen `Configure()` e `InitializeSdk()`
- [Reproducir un archivo multimedia en Unity](simple-player.md) — el ejemplo de reproducción
  de archivos
- [Ver una cámara RTSP en Unity](rtsp-viewer.md) — el ejemplo RTSP / cámara IP en vivo
- [Capturar una webcam con VideoCaptureCoreX](video-capture-x.md) · [Editar y renderizar con VideoEditCoreX](video-edit-x.md) — los ejemplos independientes de motores CoreX
- [Compilar para Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
- [Matriz de plataformas](platform-matrix.md) — soporte de funciones por plataforma Unity
- [Solución de problemas](troubleshooting.md) — errores comunes y arreglos
- [Visión general del Media Blocks SDK .NET](../../mediablocks/index.md) — el catálogo
  completo de bloques
- [Directorio de marcas de cámaras IP](../../camera-brands/index.md) — URLs probadas y ajustes
