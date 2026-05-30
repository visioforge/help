---
title: Empieza con Media Blocks SDK en Unity 6 — Guía rápida
description: Guía rápida de cinco pasos desde un proyecto Unity 6 nuevo a un video reproduciéndose con el .unitypackage de VisioForge.
sidebar_label: Primeros pasos
order: 51
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Quickstart
  - Windows
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - BufferSinkBlock
---

# Primeros pasos

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta página es el camino en cinco pasos desde un proyecto Unity 6 nuevo hasta un video
reproduciéndose. Para detalles de instalación, notas profundas por plataforma y la explicación
del bootstrap, sigue los enlaces al final.

## Qué necesitas

- **Unity 6 LTS** (`6000.x`) — verificado en `6000.4.6f1`. Versiones Unity LTS anteriores
  (2022 / 2023) podrían funcionar (sin verificar), siempre que ofrezcan `.NET Standard 2.1` como opción de
  Api Compatibility Level.
- **Una ruta NTFS corta** en Windows — por ejemplo, `C:\unity\MyApp`. Evita Dev Drive (ReFS) y
  rutas muy largas; la caché de paquetes de Unity puede desbordar el límite de 260 caracteres
  `MAX_PATH` de Windows.
- Para los targets móviles / Apple, el módulo Unity correspondiente instalado mediante Unity Hub
  (Android Build Support, iOS Build Support, macOS Build Support).

## Paso 1 — Descarga el `.unitypackage` acumulativo

[**VisioForge.MediaBlocks.Unity.unitypackage**](https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage)

```text
https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage
```

El paquete es autónomo — ensamblados gestionados, cada runtime nativo soportado, escenas de
ejemplo y el asistente de configuración de un único uso, todo está dentro. Sin restauración NuGet,
sin instalación externa de GStreamer, sin descargas por plataforma.

## Paso 2 — Importa el paquete

En Unity: **Assets → Import Package → Custom Package…**, selecciona el `.unitypackage`
descargado y haz clic en **Import** con todos los ítems marcados.

El paquete acumulativo publicado añade los cuatro runtimes de plataforma (un build privado personalizado solo incluye las plataformas para las que se optó con sus switches `-Include*`):

- `Assets/StreamingAssets/VisioForge/x64/` — runtime nativo de Windows
- `Assets/Plugins/Android/` — runtime nativo de Android
- `Assets/Plugins/macOS/` — runtime nativo de macOS
- `Assets/Plugins/iOS/GStreamerX.framework/` — runtime nativo de iOS
- `Assets/Plugins/` — ensamblados gestionados del SDK
- `Assets/Scripts/` — `VisioForgeEnvironment`, `VisioForgeVideoView` y los dos componentes
  player de ejemplo compartidos
- `Assets/Scenes/SimplePlayer.unity` y `Assets/Scenes/RTSPViewer.unity` — escenas de ejemplo
- `Assets/VisioForge/` — el asistente de configuración de un único uso y (en builds móviles /
  macOS) `link.xml`

## Paso 3 — Aplica los ajustes del proyecto

En la primera importación, el asistente de configuración ofrece aplicar los ajustes obligatorios.
Pulsa **Apply** y ambos se configuran por ti:

| Ajuste | Valor | Por qué |
|---|---|---|
| Api Compatibility Level | `.NET Standard 2.1` | El SDK se distribuye como ensamblados `netstandard2.1`. El ajuste legacy `.NET Framework` no puede cargarlos. |
| Enter Play Mode → Reload Domain | **Deshabilitado** | El SDK se inicializa una vez por proceso; una recarga de dominio entre sesiones Play puede colgar el Editor mientras el bucle principal GLib está en medio de una llamada. |

Si pulsas **Skip**, configúralos manualmente en **Edit → Project Settings**:

- Player → Other Settings → Configuration → Api Compatibility Level
- Editor → Enter Play Mode Settings → When entering Play Mode (cualquier opción que **no**
  recargue el dominio — `Reload Scene only` coincide con lo que hace **Apply**)

Para targets móviles (Android, iOS), también establece **Scripting Backend = IL2CPP** en la
misma sección Configuration. Mono no está soportado en Android o iOS por el propio Unity.

## Paso 4 — Ejecuta la escena de ejemplo

1. En la ventana **Project** abre `Assets/Scenes/SimplePlayer.unity` (doble clic — no te quedes
   en la escena predeterminada vacía).
2. Selecciona el GameObject **RawImage** en la **Hierarchy**.
3. En el **Inspector** establece **File Path** a una ruta absoluta a un archivo multimedia local.
4. Pulsa **▶ Play**.

El video se renderiza en la Game view; el audio se reproduce por el dispositivo de audio
predeterminado del sistema.

Si tienes una cámara RTSP local, abre `Assets/Scenes/RTSPViewer.unity` en su lugar, establece
**Rtsp Url** (más **Login** / **Password** si la cámara requiere autenticación) y pulsa **Play**.

## Paso 5 — Adáptalo a tu propia escena

No tienes que usar las escenas de ejemplo. Para reproducir un video en tu propia UI:

1. Añade un **Canvas → Raw Image** (*GameObject → UI → Raw Image*).
2. Selecciona el **Raw Image** y **Add Component →** `MediaBlocksPlayer` (o `RTSPViewerPlayer`
   para RTSP).
3. Establece el campo **File Path** (o **Rtsp Url**) y pulsa **▶ Play**.

El manejo de aspecto, el flip vertical y la subida de `Texture2D` están gestionados por el
`VisioForgeVideoView` incluido. Tu script es solo el pipeline — consulta
[Reproducir un archivo multimedia en Unity](simple-player.md) para el desglose en C#.

## Compila para una plataforma destino

Cuando estés listo para distribuir:

- [Windows x64](windows.md) — base del Editor y el Standalone Player.
- [Android](android.md) — IL2CPP arm64, permisos AndroidManifest, notas de tamaño.
- [macOS](macos.md) — Universal arm64+x86_64, firma de código y notarización.
- [iOS](ios.md) — exportación a Xcode, permisos Info.plist, dispositivo arm64 IL2CPP solamente.

El `.unitypackage` acumulativo contiene cada plataforma que se incluyó cuando se construyó;
Unity elige el runtime correcto por Build Target mediante los metadatos `PluginImporter` por
fichero.

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — referencia de instalación
- [Uso de VisioForge en Unity](index.md) — visión general de arquitectura y rendering
- [Bootstrap y ciclo de vida](bootstrap.md) — qué hacen `Configure()` e `InitializeSdk()`
- [Matriz de plataformas](platform-matrix.md) — disponibilidad de funciones por plataforma
- [Solución de problemas](troubleshooting.md) — errores comunes y arreglos
