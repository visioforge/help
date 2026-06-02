---
title: Compilar app Unity de video para Android arm64 IL2CPP
description: Ajustes de build, requisitos IL2CPP, permisos AndroidManifest y solución de problemas para el VisioForge Media Blocks SDK .NET en Unity 6 en Android.
sidebar_label: Compilar para Android
order: 54
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Android
  - IL2CPP
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Compilar para Android

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

El flavor Android se distribuye como un `libgstreamer_android.so` monolítico con cada plugin de
GStreamer enlazado estáticamente, más los ensamblados gestionados del paquete compilados contra
`netstandard2.1`. Esta página cubre los ajustes del Build Profile, los permisos del manifiesto
y los problemas específicos de Android. Para la secuencia de arranque compartida entre
plataformas, consulta [Bootstrap y ciclo de vida](bootstrap.md).

El flavor Android se incluye en el `.unitypackage` cuando el pipeline de build se ejecuta con
`-IncludeAndroid`. El resultado es un paquete acumulativo que contiene los runtimes Windows,
Android y los opt-in de Apple — Unity elige el correcto en tiempo de build a partir de los
metadatos `PluginImporter` por archivo.

## Player Settings

| Ajuste | Valor | Dónde |
|---|---|---|
| Target Platform | **Android** | File → Build Profiles → Android |
| Texture Compression | **ASTC** *(recomendado; predeterminado en Unity 6)* | File → Build Profiles → Android |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **IL2CPP** *(obligatorio — Mono no está soportado en Android por Unity)* | Project Settings → Player → Other Settings → Configuration |
| Target Architectures | **ARM64** *(ARMv7 no se distribuye — desmárcalo)* | Project Settings → Player → Other Settings → Configuration |
| Internet Access | **Require** *(necesario para fuentes RTSP / HTTPS)* | Project Settings → Player → Other Settings → Configuration |
| Write Permission | **External (SDCard)** si escribes o grabas medios en el almacenamiento externo | Project Settings → Player → Other Settings → Configuration |
| Minimum API Level | **24 (Android 7.0)** o posterior | Project Settings → Player → Other Settings → Identification |

Mono no puede cargar `libgstreamer_android.so` correctamente a través del runtime Android de
Unity — solo se ejercita IL2CPP en CI y se soporta en producción.

## Entradas obligatorias en AndroidManifest

Unity genera `AndroidManifest.xml` por ti. Los ajustes anteriores se traducen en las entradas
estándar; si necesitas un manifiesto personalizado, asegúrate de que contenga:

```xml
<uses-permission android:name="android.permission.INTERNET" />

<!-- Solo si tu app usa discovery RTSP / hace stream audio-out vía UDP en el segmento local -->
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<!-- Solo si tu app usa micrófono o cámara como fuente -->
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.CAMERA" />

<!-- Solo si lees medios desde almacenamiento externo -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"
                 android:maxSdkVersion="32" />
<uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
<uses-permission android:name="android.permission.READ_MEDIA_AUDIO" />
```

`READ_MEDIA_VIDEO` / `READ_MEDIA_AUDIO` reemplazan al `READ_EXTERNAL_STORAGE` legacy en
Android 13+ (API 33+); declara ambas formas para que los dispositivos más antiguos sigan
funcionando.

## Qué añade el paquete para Android

El paso `deploy-unity-natives.ps1` despliega el runtime Android en tu proyecto así:

| Ruta | Contenido |
|---|---|
| `Assets/Plugins/Android/libs/arm64-v8a/libgstreamer_android.so` | Runtime monolítico de GStreamer — todos los plugins enlazados estáticamente. |
| `Assets/Plugins/Android/libs/arm64-v8a/libVisioForge_Core.so` | Shim nativo flavor-Android del SDK. |
| `Assets/Plugins/Android/visioforge-gstreamer.aar` | El archivo Java que expone `org.freedesktop.gstreamer.GStreamer.init(Context)`. |
| `Assets/VisioForge/link.xml` | Reglas de preservación de tipos / miembros para IL2CPP. |
| `Assets/Plugins/Android/` | Ensamblados gestionados compilados contra `netstandard2.1` con `UNITY_NS21_ANDROID` definido. |

El `link.xml` es obligatorio. Sin él, el stripping de código gestionado de IL2CPP elimina tipos
a los que el SDK accede por reflexión — subclases `GLib.SignalArgs`, `SignalClosure.MarshalCallback`
— y el primer disparo de delegado lanza `MissingMethodException`. El paquete distribuye un
`link.xml` probado; no lo elimines de `Assets/`.

## Tamaño de build

El flavor Android añade aproximadamente **35 MB** al APK / AAB:

- `libgstreamer_android.so` — ~30 MB (arm64-v8a, sin símbolos)
- `libVisioForge_Core.so` — ~2 MB
- Ensamblados gestionados — ~3 MB

Si distribuyes también ARMv7 (no incluido actualmente por el paquete, pero si lo despliegas
manualmente), espera duplicar las bibliotecas nativas.

## Build Standalone

**File → Build Profiles → Android → Build** (o **Build And Run**) produce un APK / AAB.

Probado en:

- Unity 6 (`6000.x`) con el módulo Android Build Support instalado
- Dispositivos Android 9 a Android 15
- Pixel 6 / 7 / 8 / 9, Galaxy S22 / S23 / S24

## Solución de problemas

| Síntoma | Causa | Arreglo |
|---|---|---|
| `DllNotFoundException: libgstreamer_android` al cargar la escena | El Scripting Backend es Mono, no IL2CPP. | Cambia a IL2CPP en Project Settings → Player → Other Settings. |
| `MissingMethodException` de `GLib.SignalArgs` o `SignalClosure.MarshalCallback` | `link.xml` falta o fue eliminado. | Confirma que `Assets/VisioForge/link.xml` existe. Reimporta el paquete si no está. |
| `InvalidOperationException: Unity Android bootstrap requires com.unity3d.player.UnityPlayer.currentActivity to be available` | Wear OS / Android TV / host Unity-as-a-library donde el campo es null en `BeforeSceneLoad`. | Difiere `VisioForgeEnvironment.Configure()` al primer evento de Activity observable y llámalo manualmente desde allí. |
| El init Java falla con `failed to find getFilesDir` | La Activity no es una `UnityPlayerActivity` y no expone la API Context estándar de Android. | Confirma que la Activity host hereda de una `Activity` real de Android. |
| Streams RTSPS / HTTPS fallan con error TLS | La extracción del paquete CA falló silenciosamente. | Mira en Logcat `[VisioForge] CA cert extraction failed`. Reimporta el paquete si el recurso embebido falta. |
| La app crashea en el segundo `Awake` tras volver de fondo | Se llamó a `VisioForgeX.DestroySDK()` en `OnDestroy`. | No lo llames — consulta [Bootstrap y ciclo de vida](bootstrap.md#el-ciclo-de-vida-del-editor). |

## Preguntas Frecuentes

### ¿Por qué no se distribuye ARMv7?

Los dispositivos Android modernos (línea base API 24 / Android 7.0) son predominantemente
arm64. Distribuir los ~30 MB del GStreamer monolítico para ambas ABIs duplicaría el tamaño del
paquete para una cuota de dispositivos minúscula. Si tienes un requisito ARMv7 estricto,
contacta con soporte.

### ¿Puedo usar el SDK en un proyecto Android no-Unity?

Sí — el SDK subyacente se distribuye como un paquete NuGet independiente
`VisioForge.CrossPlatform.Core.Android` para apps .NET Android puras. El paquete Unity envuelve
ese runtime más el bootstrap Java y `link.xml`; el wrapper es específico de Unity.

### ¿Funciona el SDK en el modo Android Editor (Project Player → Run In Editor)?

Run-in-Editor para targets Android no se ejercita; compila y despliega a un dispositivo real.
El propio Editor ejecuta el flavor **Windows** del SDK en un host Windows — cambiar el Build
Target a Android en Build Profiles no cambia qué runtime nativo carga el Editor.

### ¿Qué fuentes funcionan sobre RTSP en Android?

El mismo `RTSPSourceBlock` usado en Windows. Auto-reconexión, credenciales opcionales, streams
solo-video y video+audio, y los transportes RTSP estándar (TCP, UDP, multicast UDP) están
todos soportados. El flavor Android usa el elemento GStreamer `rtspsrc` internamente — el mismo
que los flavors Windows y macOS.

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — configuración del paquete
- [Bootstrap y ciclo de vida](bootstrap.md) — el bootstrap Java de Android explicado
- [Ver una cámara RTSP en Unity](rtsp-viewer.md) — el ejemplo `RTSPViewer`
- [Solución de problemas](troubleshooting.md) — referencia de errores entre plataformas
- [Matriz de plataformas](platform-matrix.md) — soporte de funciones por plataforma Unity
