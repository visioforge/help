---
title: Instalar Media Blocks SDK .NET en Unity 6 — Windows
description: Configuración paso a paso de VisioForge Media Blocks SDK .NET en Unity 6 en Windows — importa el .unitypackage autónomo, aplica los ajustes y ejecuta.
sidebar_label: Unity
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Instalar el Media Blocks SDK en Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta guía explica cómo instalar el **Media Blocks SDK .NET** en **Unity 6** en **Windows
x64**. El SDK se distribuye como un **`.unitypackage`** listo para importar y totalmente autónomo:
no compilas nada desde el código fuente, no necesitas NuGet y no hay dependencias externas que
instalar. Tras importarlo, abre una escena de ejemplo y pulsa **Play**.

Una vez instalado, consulta las guías de uso: [Reproducir un archivo multimedia en Unity](../general/unity/simple-player.md)
y [Ver una cámara RTSP en Unity](../general/unity/rtsp-viewer.md).

## Requisitos

| | |
|---|---|
| Unity | **6 (6000.x)** — verificado en `6000.4.6f1` |
| Plataforma | **Windows x64** (Editor y reproductor independiente) |

!!! warning "Usa una ruta corta en NTFS — no un volumen Dev Drive / ReFS"
    Importar el paquete escribe miles de pequeños archivos nativos, y la importación/compilación de
    Unity implica una intensa E/S de archivos pequeños. En un Dev Drive (ReFS) eso es
    **drásticamente más lento** (una importación en frío puede tardar varios minutos en lugar de
    segundos) y es más propenso a la condición de carrera `EPERM rename`. Mantén el proyecto en una
    unidad **NTFS** simple con una ruta raíz corta, p. ej. `C:\unity\MyApp`. La caché de paquetes de
    Unity también genera rutas profundas que pueden superar el límite de 260 caracteres `MAX_PATH`
    de Windows.

## Descarga

Descarga el paquete más reciente:

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
| SDK gestionado (`net48`) + dependencias | `Assets/Plugins/` | los ensamblados del Media Blocks SDK .NET |
| Bibliotecas nativas del SDK en tiempo de ejecución, incl. FFmpeg/libav | `Assets/StreamingAssets/VisioForge/x64/` | el motor multimedia; se copia textualmente en las compilaciones independientes |
| Scripts reutilizables | `Assets/Scripts/` | `VisioForgeEnvironment`, `VisioForgeVideoView` y los dos reproductores |
| Dos escenas listas | `Assets/Scenes/` | `SimplePlayer` (archivo) y `RTSPViewer` (RTSP) |
| Asistente de configuración inicial | `Assets/VisioForge/Editor/` | aplica los dos ajustes de proyecto requeridos |

## Paso 3 — Aplicar los ajustes de proyecto requeridos

En la primera importación, el asistente de configuración muestra un cuadro de diálogo que pide
aplicar dos ajustes de proyecto requeridos. Haz clic en **Apply** — ambos ajustes se configuran por
ti.

![Cuadro de diálogo de configuración del Media Blocks SDK de VisioForge con los botones Apply y Skip](unity-apply-dialog.webp)

Estos dos ajustes son **obligatorios** — el SDK no funcionará sin ellos:

- **Api Compatibility Level = .NET Framework** — Unity 6 usa de forma predeterminada
  `.NET Standard 2.1`, que no puede cargar esta compilación `net48` del SDK (síntoma:
  `TypeLoadException` al ejecutar).
- **Disable Domain Reload** — el SDK se inicializa una vez por proceso y se reutiliza entre sesiones
  de Play/Stop; con Domain Reload activado, el Editor puede colgarse al salir del modo Play.

## Paso 4 — Configurar los ajustes manualmente (solo si hiciste clic en Skip)

Si hiciste clic en **Skip**, configura ambos a mano:

1. **Api Compatibility Level = .NET Framework**
   *Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level*.

   ![Ajustes del Player con Api Compatibility Level establecido en .NET Framework](unity-apicompat.webp)

2. **Disable Domain Reload**
   *Edit → Project Settings → Editor → Enter Play Mode Settings* → establece **When entering Play Mode**
   en una opción que **no** recargue el dominio — ya sea **Reload Scene only** (coincide con lo que
   hace **Apply**) o **Do not reload Domain or Scene**.

   ![Enter Play Mode Settings del Editor con la recarga de dominio desactivada](unity-domain-reload.webp)

## Paso 5 — Ejecutar una escena de ejemplo

En la ventana **Project** abre `Assets/Scenes/SimplePlayer.unity` (haz doble clic en ella — no te
quedes en la escena predeterminada vacía), selecciona el GameObject **RawImage**, establece su
**File Path** en el Inspector y pulsa **▶ Play**. El video se renderiza en la vista Game y el audio
se reproduce a través del dispositivo predeterminado del sistema.

![La escena SimplePlayer reproduciendo video en la vista Game de Unity](unity-sample-play.webp)

!!! tip "El RawImage se ve vacío hasta que pulsas Play"
    La textura de video se crea en tiempo de ejecución, por lo que el `RawImage` está en blanco
    (blanco) en el modo de edición.

A continuación, lee las guías de uso:

- [Reproducir un archivo multimedia en Unity](../general/unity/simple-player.md) — el ejemplo `SimplePlayer`.
- [Ver una cámara RTSP en Unity](../general/unity/rtsp-viewer.md) — el ejemplo `RTSPViewer`.

## Compilaciones independientes

*File → Build Settings → Windows x64 → Build* produce un reproductor independiente que funciona sin
ningún paso adicional: Unity copia textualmente el runtime nativo de
`Assets/StreamingAssets/VisioForge/x64` en `<game>_Data/StreamingAssets/VisioForge/x64`, y los
ensamblados gestionados de `Assets/Plugins` se preparan automáticamente. La misma ruta de carga se
resuelve tanto en el Editor como en la compilación independiente.

## Desinstalar o actualizar el paquete

Un `.unitypackage` no tiene desinstalador: elimina los archivos manualmente.

1. **Cierra primero el Editor de Unity** — bloquea las DLL nativas y la caché `Library/`.
2. Elimina el contenido de VisioForge de `Assets/`:
   - `Assets/StreamingAssets/VisioForge/` — el runtime nativo (~300 archivos).
   - `Assets/VisioForge/` — el asistente de configuración inicial.
   - Los cuatro scripts reutilizables de `Assets/Scripts/`: `VisioForgeEnvironment.cs`,
     `VisioForgeVideoView.cs`, `MediaBlocksPlayer.cs`, `RTSPViewerPlayer.cs` (junto con sus `.meta`)
     — conserva cualquier script propio que esté en la misma carpeta.
   - Las escenas de ejemplo `Assets/Scenes/SimplePlayer.unity` y `Assets/Scenes/RTSPViewer.unity`.
   - Los ensamblados de VisioForge en `Assets/Plugins/` (`VisioForge.*.dll`, `GstSharp.dll`,
     `GLibSharp.dll` y sus dependencias, cada uno con su `.meta`). Elimina toda la carpeta
     `Assets/Plugins/` solo si no contiene nada más que ensamblados de VisioForge.
3. Elimina la carpeta `Library/` del proyecto (junto a `Assets/`) para limpiar el estado de
   importación en caché. Unity la regenera en la siguiente apertura (el primer arranque es más lento).

**Actualización:** importa el nuevo `.unitypackage` sobre el anterior — los GUID de los plugins
gestionados son deterministas, por lo que Unity sobrescribe los activos existentes en su sitio y se
conservan las referencias. Si vienes de un paquete mucho más antiguo o ves DLL duplicadas en
`Assets/Plugins/`, haz primero una eliminación limpia (pasos anteriores) y luego importa el paquete nuevo.

## Solución de problemas

| Síntoma | Causa | Solución |
|---|---|---|
| `TypeLoadException` al ejecutar | Api Compatibility Level es `.NET Standard 2.1` | Establécelo en `.NET Framework`, o reimporta y haz clic en **Apply** |
| El Editor se cuelga en "Reloading domain" al hacer Play/Stop | Domain Reload está activado | Mantén Disable Domain Reload activado |
| El Editor se bloquea en el 2.º Play | El SDK se cerró en Stop y se reinicializó | No cierres el SDK en Stop; mantén Disable Domain Reload activado |
| No se encuentra el runtime nativo | El paquete se importó parcialmente | Reimporta el paquete con todos los elementos marcados |
| Sin video, errores en la Console tras importar | El Editor necesita una recarga limpia después de preparar el runtime | Reinicia el Editor |
| El Editor se vuelve inestable tras una sesión de edición larga | Recompilaciones repetidas de scripts | Reinicia el Editor |

## Limitaciones conocidas

- **Solo Windows x64** — el runtime nativo incluido es Windows x64. El paquete aún no admite otras
  plataformas.

## Preguntas frecuentes

### ¿Puedo instalar el SDK en Unity mediante NuGet?

No. Unity no ejecuta la restauración de NuGet, y el SDK incluye ~300 archivos nativos que NuGet no
dispondría para Unity. El `.unitypackage` agrupa todo — ensamblados gestionados, runtime nativo,
scripts y escenas — por lo que importas un único archivo en su lugar.

### ¿Necesito instalar GStreamer o alguna otra dependencia del sistema?

No. El paquete es totalmente autónomo; todo lo que el SDK necesita está dentro de él. No se requiere
una instalación de GStreamer en tu máquina y el runtime incluido no la utiliza.

### ¿Funcionan otros SDKs de VisioForge en Unity?

Hoy en día el paquete de Unity incluye el runtime del **Media Blocks SDK .NET**, que cubre
reproducción, captura, procesamiento y streaming. Otros SDKs de VisioForge llegarán después.

## Véase también

- [Usar VisioForge en Unity](../general/unity/index.md) — visión general de cómo funciona la integración
- [Reproducir un archivo multimedia en Unity](../general/unity/simple-player.md) — el ejemplo de reproducción de archivos
- [Ver una cámara RTSP en Unity](../general/unity/rtsp-viewer.md) — el ejemplo RTSP
- [Visión general del Media Blocks SDK .NET](../mediablocks/index.md) — el catálogo completo de bloques
- [Guía de instalación](index.md) — instala el SDK en otros tipos de proyecto .NET
