---
title: Compilar juego Unity de video para Windows x64 (Editor)
description: Ajustes de build y solución de problemas para VisioForge Media Blocks SDK .NET en Unity 6 en Windows x64 — Editor y Standalone.
sidebar_label: Compilar para Windows
order: 53
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Standalone Player
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Compilar para Windows

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Windows x64 es el target base del paquete Unity — se distribuye en cada variante de
`.unitypackage` que produce el pipeline de build. Esta página cubre los Player Settings, el
layout en disco y los problemas que puedes encontrar específicamente en Windows. Para el resto,
consulta [Bootstrap y ciclo de vida](bootstrap.md).

## Player Settings

| Ajuste | Valor | Dónde |
|---|---|---|
| Architecture | **x86_64** | File → Build Profiles → Windows |
| Target Platform | **Windows** | File → Build Profiles → Windows |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **Mono** *(IL2CPP también funciona; Mono es el predeterminado en Windows)* | Project Settings → Player → Other Settings → Configuration |
| Enter Play Mode → Reload Domain | **Off** | Project Settings → Editor → Enter Play Mode Settings |

Si importaste el paquete con el diálogo **Apply**, los dos ajustes de proyecto obligatorios
(`.NET Standard 2.1` y Disable Domain Reload) ya están en su sitio.

## Organización en disco

El paso de build `deploy-unity-natives.ps1` despliega el runtime de Windows en tu proyecto así:

| Ruta | Contenido |
|---|---|
| `Assets/StreamingAssets/VisioForge/x64/` | Layout plano: libs core de GStreamer, todos los DLL de plugins, módulos GIO, `ca-certificates.crt`. ~300 archivos. |
| `Assets/Plugins/` | Ensamblados gestionados (`VisioForge.Core.dll`, `VisioForge.Libs.dll`, `GstSharp.dll`, `GLibSharp.dll`, etc.) con sus `.meta`. |
| `Assets/Scripts/` | Los helpers compartidos: `VisioForgeEnvironment.cs`, `VisioForgeVideoView.cs`, `MediaBlocksPlayer.cs`, `RTSPViewerPlayer.cs`. |
| `Assets/Scenes/` | Las dos escenas de ejemplo: `SimplePlayer.unity` y `RTSPViewer.unity`. |

Se usa `StreamingAssets` (no `Plugins/x64`) porque Unity copia esa carpeta literalmente a un
build Standalone, que es exactamente a donde `VisioForgeEnvironment.Configure()` apunta el
cargador DLL. La misma ruta resuelve tanto en el Editor como en el player —
`Application.streamingAssetsPath` devuelve el `Assets/StreamingAssets` del proyecto en el
Editor y `<game>_Data/StreamingAssets` en el player.

## Build del Standalone Player

**File → Build Profiles → Windows → x86_64 → Build** produce un player autónomo. Sin pasos
adicionales:

- Unity copia `Assets/StreamingAssets/VisioForge/x64/` a
  `<game>_Data/StreamingAssets/VisioForge/x64/` automáticamente.
- Los plugins gestionados de `Assets/Plugins/` se preparan en `<game>_Data/Managed/`.
- `VisioForgeEnvironment.Configure()` se ejecuta `BeforeSceneLoad` y apunta `SetDllDirectoryW`
  a la carpeta de nativos preparada.

El `<game>.exe` resultante junto con su carpeta `_Data/` es todo el artefacto distribuible.

## Tamaño en disco

El runtime de Windows añade aproximadamente **50 MB** de bibliotecas nativas a tu build
(hasta ~30 MB si excluyes libav con `-SkipLibav` al reconstruir el paquete). Los ensamblados
gestionados añaden otros ~5 MB.

## Solución de problemas

| Síntoma | Causa | Arreglo |
|---|---|---|
| `DllNotFoundException: gstreamer-1.0-0` en Play | `Assets/StreamingAssets/VisioForge/x64/` falta o está vacía. | Reimporta el `.unitypackage` con todos los ítems marcados, o revisa la línea de la Consola `[VisioForge] Native runtime not found at …` para la ruta resuelta. |
| El pipeline se cuelga al arrancar, el log muestra dos llamadas `gst_init` | Una instalación de GStreamer en el `PATH` del sistema carga una segunda copia de `gstreamer-1.0-0.dll`. | `Configure()` ya limpia el `PATH` — confirma inspeccionando la Consola: se registra el conteo de entradas eliminadas. Si el conteo es 0 pero sigues viendo el síntoma, un lanzador externo está reinyectando GStreamer tras `Configure()`. |
| `TypeLoadException` en la primera llamada al SDK | Api Compatibility Level es `.NET Framework` en lugar de `.NET Standard 2.1`. | Ajústalo a `.NET Standard 2.1` (Project Settings → Player → Other Settings → Configuration → Api Compatibility Level). |
| Streams RTSPS / HTTPS fallan al conectar con error TLS | `SSL_CERT_FILE` no apunta al paquete CA incluido. | `Configure()` lo establece cuando `ca-certificates.crt` está presente en la carpeta de nativos. Un paquete CA ausente se registra como advertencia — vuelve a preparar el runtime mediante `deploy-unity-natives.ps1`. |
| El Editor se cuelga en "Reloading domain" tras Play/Stop | Disable Domain Reload se volvió a activar. | Reactívalo en Project Settings → Editor → Enter Play Mode Settings (ajusta **When entering Play Mode** a una opción que no recargue). |

## Preguntas Frecuentes

### ¿Puedo usar IL2CPP en Windows?

Sí — compila y se ejecuta. Mono es el predeterminado y es lo que ejercita la matriz CI del
paquete. Cambia a IL2CPP solo si tienes una razón a nivel de proyecto (otros plugins que lo
requieran, superficie de despliegue más pequeña). El mismo `link.xml` que se distribuye con el
paquete preserva los tipos gestionados del SDK frente al stripping de IL2CPP en cualquier
backend.

### ¿Funciona Windows 11 ARM64?

No desde este `.unitypackage`. El runtime nativo incluido es solo x86_64 — ejecutarlo bajo
emulación x64 en ARM64 no está soportado. No hay un build nativo ARM64 en el paquete Unity
actual.

### ¿El SDK necesita derechos de administrador?

No. Todo se ejecuta desde la carpeta del proyecto / la carpeta `_Data` del player. El runtime
no toca claves de registro, no instala drivers globales y solo escribe en
`Application.persistentDataPath` (para logs / archivos temporales cuando están habilitados).

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — configuración del paquete
- [Bootstrap y ciclo de vida](bootstrap.md) — cómo `Configure()` arranca el runtime de Windows
- [Reproducir un archivo multimedia en Unity](simple-player.md) — el ejemplo `SimplePlayer`
- [Ver una cámara RTSP en Unity](rtsp-viewer.md) — el ejemplo `RTSPViewer`
- [Solución de problemas](troubleshooting.md) — errores comunes de runtime en todas las plataformas
