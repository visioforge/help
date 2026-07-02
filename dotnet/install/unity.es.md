---
title: Instalar SDKs multimedia de VisioForge en Unity 6 — Guía
description: Configura los SDKs multimedia de VisioForge en Unity 6 — un .unitypackage para reproducción, captura y edición de video en Windows, Android, macOS, iOS.
sidebar_label: Unity
tags:
  - Media Blocks SDK
  - Media Player SDK
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - Unity
  - Windows
  - Android
  - macOS
  - iOS
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCoreX
---

# Instalar los SDKs multimedia de VisioForge en Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Esta guía explica cómo instalar los **SDKs multimedia** de VisioForge en **Unity 6**. Un único
**`.unitypackage`** autónomo integra cuatro productos en Unity a la vez — el pipeline del
**Media Blocks SDK .NET** así como los motores de alto nivel **Media Player SDK .NET**
(`MediaPlayerCoreX`), **Video Capture SDK .NET** (`VideoCaptureCoreX`) y **Video Edit SDK .NET**
(`VideoEditCoreX`). El paquete agrupa cada runtime nativo soportado en un solo archivo —
Windows x64, Android, macOS Standalone e iOS Standalone — permitiendo que Unity seleccione el
adecuado según la plataforma de destino (Build Target) al compilar. No compilas nada desde el
código fuente, no necesitas NuGet y no hay dependencias externas que instalar.

El paquete apunta a ensamblados gestionados **`netstandard2.1`**. Para proyectos limitados a la
versión más antigua de Mono en Unity LTS, todavía se publica una versión heredada (legacy)
`net48` exclusiva para Windows — consulta el spoiler al final de esta página.

Para ver qué incluye y cómo usarlo, consulta
**[Usar VisioForge en Unity](../general/unity/index.md)** — la visión general con el catálogo
completo de productos y ejemplos. Para el atajo de cinco pasos, consulta la
**[Guía rápida](../general/unity/getting-started.md)**.

## Requisitos

| | |
|---|---|
| Unity | **6 (6000.x)** — verificado en `6000.4.6f1` |
| Targets de build distribuidos | **Windows x64**, **Android arm64**, **macOS Universal arm64+x86_64**, **iOS dispositivo arm64** |
| TFM gestionado | **`netstandard2.1`** |
| Ajustes obligatorios del Editor | `Api Compatibility Level = .NET Standard 2.1` |

!!! warning "Usa una ruta corta en NTFS — no un volumen Dev Drive / ReFS"
    Importar el paquete escribe miles de pequeños archivos nativos, y la importación/compilación
    de Unity implica una intensa E/S de archivos pequeños. En un Dev Drive (ReFS) eso es
    **drásticamente más lento** (una importación en frío puede tardar varios minutos en lugar
    de segundos) y es más propenso a la condición de carrera `EPERM rename`. Mantén el
    proyecto en una unidad **NTFS** simple con una ruta raíz corta, p. ej. `C:\unity\MyApp`. La
    caché de paquetes de Unity también genera rutas profundas que pueden superar el límite de
    260 caracteres `MAX_PATH` de Windows.

## Descarga

Descarga el paquete acumulativo más reciente — Windows + Android + macOS + iOS en un solo
archivo:

[**VisioForge.MediaBlocks.Unity.unitypackage**](https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage)

```text
https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage
```

## Paso 1 — Crear o abrir un proyecto de Unity

Usa un proyecto de Unity 6 existente o crea uno nuevo (cualquier plantilla). Mantén la raíz del
proyecto en una ruta NTFS corta (consulta la advertencia anterior).

![Creando un proyecto de Unity 6 en una ruta NTFS corta en Unity Hub](unity-new-project.webp)

## Paso 2 — Importar el paquete

En el Editor: **Assets → Import Package → Custom Package…**, selecciona el `.unitypackage`
descargado y haz clic en **Import** (deja todos los elementos marcados).

![Cuadro de diálogo Import Unity Package mostrando el contenido del paquete VisioForge](unity-import-package.webp)

El paquete agrega:

| Contenido | Ubicación | Propósito |
|---|---|---|
| SDK gestionado (`netstandard2.1`) + dependencias | `Assets/Plugins/` (+ subcarpetas `Android/`, `macOS/`, `iOS/Managed/`) | los ensamblados del Media Blocks SDK .NET, por plataforma |
| Runtime nativo de Windows | `Assets/StreamingAssets/VisioForge/x64/` | libs y plugins de GStreamer para Windows |
| Runtime nativo de Android | `Assets/Plugins/Android/libs/arm64-v8a/` | `libgstreamer_android.so` monolítico + AAR Java |
| Runtime nativo de macOS | `Assets/Plugins/macOS/` | dylibs universales (arm64+x86_64) |
| Runtime nativo de iOS | `Assets/Plugins/iOS/GStreamerX.framework/` | framework embebido (dispositivo arm64) |
| Preservación IL2CPP | `Assets/VisioForge/link.xml` | preservación de tipos / miembros para Android e iOS |
| Scripts reutilizables | `Assets/Scripts/` | los asistentes `VisioForgeEnvironment` y `VisioForgeVideoView` más los seis scripts de ejemplo |
| Seis escenas de ejemplo | `Assets/Scenes/` | `SimplePlayer`, `RTSPViewer`, `MediaPlayerX`, `IPCameraX`, `VideoCaptureX`, `VideoEditX` — consulta la [visión general de ejemplos](../general/unity/index.md#ejemplos) |
| Asistente de configuración inicial | `Assets/VisioForge/Editor/` | aplica el ajuste de proyecto requerido |

Los metadatos `PluginImporter` por flavor en cada archivo nativo le dicen a Unity a qué Build
Target pertenece cada binario — cambiar el Build Target en Build Profiles elige
automáticamente el slot correcto en tiempo de build.

## Paso 3 — Aplicar los ajustes de proyecto requeridos

En la primera importación, el asistente de configuración muestra un cuadro de diálogo que pide
aplicar un ajuste de proyecto requerido. Haz clic en **Apply** — el ajuste se configura
por ti.

![Cuadro de diálogo de configuración del Media Blocks SDK de VisioForge con los botones Apply y Skip](unity-apply-dialog.webp)

Este ajuste es **obligatorio** — el SDK no funcionará sin él:

- **Api Compatibility Level = .NET Standard 2.1** — el SDK se distribuye como ensamblados
  `netstandard2.1`; el ajuste legacy `.NET Framework` no puede cargarlos.

El comportamiento predeterminado de Enter Play Mode de Unity (Domain + Scene Reload) está
totalmente soportado — no necesitas desactivar Domain Reload. El SDK sobrevive a las recargas
de Play/Stop del Editor.

Para targets móviles, también cambia **Scripting Backend** a **IL2CPP** — Mono no está
soportado en Android o iOS por el propio Unity. Consulta
[Compilar para Android](../general/unity/android.md) y
[Compilar para iOS](../general/unity/ios.md) para las listas de comprobación por target.

## Paso 4 — Configurar el ajuste manualmente (solo si hiciste clic en Skip)

Si hiciste clic en **Skip**, configúralo a mano:

1. **Api Compatibility Level = .NET Standard 2.1**
   *Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level*.

   ![Ajustes del Player con Api Compatibility Level establecido en .NET Standard 2.1](unity-apicompat.webp)

## Paso 5 — Ejecutar una escena de ejemplo

En la ventana **Project** abre `Assets/Scenes/SimplePlayer.unity` (haz doble clic en ella — no
te quedes en la escena predeterminada vacía), selecciona el GameObject **RawImage**, establece
su **File Path** en el Inspector y pulsa **▶ Play**. El video se renderiza en la vista Game y
el audio se reproduce a través del dispositivo predeterminado del sistema.

![La escena SimplePlayer reproduciendo video en la vista Game de Unity](unity-sample-play.webp)

!!! tip "El RawImage se ve vacío hasta que pulsas Play"
    La textura de video se crea en tiempo de ejecución, por lo que el `RawImage` está en
    blanco (blanco) en el modo de edición.

A continuación, lee las guías de uso:

- [Guía rápida](../general/unity/getting-started.md) — el camino en cinco pasos desde la
  importación hasta la reproducción.
- [Usar VisioForge en Unity](../general/unity/index.md) — la visión general con el catálogo
  completo de productos y ejemplos: reproducción de archivos, RTSP / cámara IP, captura de
  webcam y edición de línea de tiempo.

## Compila para una plataforma destino

El `.unitypackage` acumulativo contiene cada plataforma soportada, pero cada Build Target
tiene sus propios ajustes y trampas. Lee la página correspondiente:

- [Compilar para Windows](../general/unity/windows.md) — x86_64 Editor + Standalone Player.
- [Compilar para Android](../general/unity/android.md) — IL2CPP arm64, permisos
  AndroidManifest.
- [Compilar para macOS](../general/unity/macos.md) — Universal arm64+x86_64, firma de código.
- [Compilar para iOS](../general/unity/ios.md) — flujo de exportación a Xcode, permisos
  Info.plist.

## Desinstalar o actualizar el paquete

Un `.unitypackage` no tiene desinstalador: elimina los archivos manualmente.

1. **Cierra primero el Editor de Unity** — bloquea las DLL nativas y la caché `Library/`.
2. Elimina el contenido de VisioForge de `Assets/`:
   - `Assets/StreamingAssets/VisioForge/` — el runtime nativo de Windows.
   - `Assets/Plugins/Android/libs/arm64-v8a/libgstreamer_android.so`, `libVisioForge_Core.so`
     y `Assets/Plugins/Android/visioforge-gstreamer.aar` — el runtime de Android.
   - `Assets/Plugins/macOS/*.dylib` y `Assets/Plugins/macOS/ca-certificates.crt` — el runtime
     de macOS.
   - `Assets/Plugins/iOS/GStreamerX.framework/` y `Assets/Plugins/iOS/libVisioForge_Core.a` —
     el runtime de iOS.
   - `Assets/Plugins/` (con subcarpetas `Android/`, `macOS/`, `iOS/Managed/`) — los ensamblados gestionados, por plataforma.
   - `Assets/VisioForge/` — el asistente de configuración inicial y `link.xml`.
   - Los scripts de `Assets/Scripts/`: los asistentes `VisioForgeEnvironment.cs` y
     `VisioForgeVideoView.cs` más los seis scripts de ejemplo — `MediaBlocksPlayer.cs`,
     `RTSPViewerPlayer.cs`, `MediaPlayerXPlayer.cs`, `IPCameraXViewer.cs`,
     `VideoCaptureXRecorder.cs`, `VideoEditXRenderer.cs` (junto con sus `.meta`) — conserva
     cualquier script propio que esté en la misma carpeta.
   - Las escenas de ejemplo de `Assets/Scenes/`: `SimplePlayer.unity`, `RTSPViewer.unity`,
     `MediaPlayerX.unity`, `IPCameraX.unity`, `VideoCaptureX.unity`, `VideoEditX.unity`.
3. Elimina la carpeta `Library/` del proyecto (junto a `Assets/`) para limpiar el estado de
   importación en caché. Unity la regenera en la siguiente apertura (el primer arranque es
   más lento).

**Actualización:** importa el nuevo `.unitypackage` sobre el anterior — los GUID de los
plugins gestionados son deterministas, por lo que Unity sobrescribe los activos existentes en
su sitio y se conservan las referencias. Si vienes de un paquete mucho más antiguo o ves DLL
duplicadas en `Assets/Plugins/`, haz primero una eliminación limpia (pasos anteriores) y luego
importa el paquete nuevo.

## Solución de problemas

| Síntoma | Causa | Solución |
|---|---|---|
| `TypeLoadException` al ejecutar | Api Compatibility Level es `.NET Framework`, no `.NET Standard 2.1` | Establécelo en `.NET Standard 2.1`, o reimporta y haz clic en **Apply** |
| El Editor se cuelga en "Reloading domain" al hacer Play/Stop | Una versión del SDK anterior a esta release, previa a la incorporación del guard de recarga del Editor | Actualiza a la versión más reciente del SDK — su guard de recarga detiene automáticamente el hilo del bucle principal de GStreamer, por lo que Domain Reload es seguro |
| El Editor se bloquea en el 2.º Play | Se llamó a `VisioForgeX.DestroySDK()` en Stop y luego se reinicializó | No cierres el SDK en Stop — es global al proceso y se reutiliza; libera solo el pipeline por-Play |
| No se encuentra el runtime nativo | Paquete importado parcialmente o el flavor del Build Target correcto falta del paquete | Reimporta el paquete con todos los elementos marcados; confirma que el paquete contiene la plataforma a la que apuntaste |
| Sin video, errores en la Consola tras importar | El Editor necesita una recarga limpia después de preparar el runtime | Reinicia el Editor |
| `DllNotFoundException` en Android | El Scripting Backend es Mono | Cambia a IL2CPP |

Para la referencia completa por síntoma, consulta
[Solución de problemas](../general/unity/troubleshooting.md).

## Flavor legacy `net48` solo-Windows

??? note "Tengo un Unity LTS más antiguo fijado a Mono — ¿qué pasa con el build net48?"

    El build original solo-Windows del paquete apunta a ensamblados gestionados
    **`.NET Framework 4.8`** y se sigue produciendo para proyectos que no pueden migrar a
    `.NET Standard 2.1` (por ejemplo, Unity 2019.4 LTS sin la opción moderna de Api
    Compatibility). Se distribuye como un `.unitypackage` separado con `NET48` en el nombre del
    archivo, contiene solo el runtime nativo Windows-x64 y usa `.NET Framework` como Api
    Compatibility Level. Los proyectos nuevos deben usar el paquete `netstandard2.1` descrito
    arriba — cubre el mismo caso Windows-x64 más todas las demás plataformas, y Unity 6 lo usa
    por defecto. Si tienes un requisito estricto del build `net48`, contacta con soporte para
    el enlace de descarga más reciente.

## Preguntas frecuentes

### ¿Puedo instalar el SDK en Unity mediante NuGet?

No. Unity no ejecuta la restauración de NuGet, y el SDK incluye cientos de archivos nativos
que NuGet no dispondría para Unity. El `.unitypackage` agrupa todo — ensamblados gestionados,
runtime nativo de cada plataforma, scripts y escenas — por lo que importas un único archivo
en su lugar.

### ¿Necesito instalar GStreamer o alguna otra dependencia del sistema?

No. El paquete es totalmente autónomo; todo lo que el SDK necesita está dentro de él. No se
requiere una instalación de GStreamer en tu máquina y el runtime incluido no la utiliza — al
contrario, `VisioForgeEnvironment.Configure()` elimina activamente cualquier GStreamer del
sistema de la ruta de búsqueda del proceso para evitar una doble inicialización.

### ¿Qué SDKs de VisioForge están incluidos?

El paquete incluye cuatro productos desde una única superficie gestionada `netstandard2.1`: el
pipeline del **Media Blocks SDK .NET** y los motores de alto nivel **Media Player SDK .NET**
(`MediaPlayerCoreX`), **Video Capture SDK .NET** (`VideoCaptureCoreX`) y **Video Edit SDK .NET**
(`VideoEditCoreX`). Cada uno incluye una o más escenas de ejemplo listas — consulta la
[visión general de ejemplos](../general/unity/index.md#ejemplos).

### ¿Funciona el mismo paquete en Windows ARM64?

El runtime nativo Windows del paquete es solo x86_64 — no hay un build nativo ARM64 hoy.
Ejecutarlo vía emulación x64 solo bajo tu propio riesgo; el uso en producción en Windows 11
ARM64 no está ejercitado.

### ¿Puedo abrir el mismo paquete en el Editor host Mac?

Sí — si el paquete se construyó con `-IncludeMacOS`. La variante acumulativa publicada en
`files.visioforge.com/unity/` siempre contiene el flavor macOS. Un paquete solo-Windows
abierto en un Editor Mac muestra un mensaje claro
`[VisioForge] Native runtime folder not found at '…' for runtime platform OSXEditor`; consulta
[Bootstrap y ciclo de vida](../general/unity/bootstrap.md).

## Véase también

- [Usar VisioForge en Unity](../general/unity/index.md) — visión general de cómo funciona la
  integración
- [Guía rápida](../general/unity/getting-started.md) — camino en cinco pasos hasta un video
  reproduciéndose
- [Bootstrap y ciclo de vida](../general/unity/bootstrap.md) — qué hacen `Configure()` e
  `InitializeSdk()`
- [Reproducir un archivo multimedia en Unity](../general/unity/simple-player.md) — el ejemplo
  de reproducción de archivos
- [Ver una cámara RTSP en Unity](../general/unity/rtsp-viewer.md) — el ejemplo RTSP
- [Capturar una webcam](../general/unity/video-capture-x.md) · [Editar y renderizar](../general/unity/video-edit-x.md) — los ejemplos de los motores CoreX
- [Matriz de plataformas](../general/unity/platform-matrix.md) — soporte de funciones por
  plataforma Unity
- [Visión general del Media Blocks SDK .NET](../mediablocks/index.md) — el catálogo completo de bloques
- [Guía de instalación](index.md) — instala el SDK en otros tipos de proyecto .NET
