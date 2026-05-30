---
title: Compilar app Unity de reproducción de video para iOS arm64
description: Ajustes de build, flujo Xcode, permisos Info.plist y solución de problemas para el VisioForge Media Blocks SDK .NET en Unity 6 en iOS Standalone.
sidebar_label: Compilar para iOS
order: 56
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - iOS
  - IL2CPP
  - RTSP
  - Xcode
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Compilar para iOS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El flavor iOS se distribuye como un único `GStreamerX.framework` (~68 MB en disco, dispositivo arm64
solamente) con cada plugin de GStreamer registrado estáticamente dentro del framework, más los
ensamblados gestionados del paquete compilados contra `netstandard2.1`. No hay archivos de
plugin separados ni escaneo de plugins en tiempo de ejecución — dyld carga el framework
mediante `@rpath` cuando dispara el primer `[DllImport]`. Esta página cubre el flujo de
exportación a Xcode, las entradas Info.plist y los problemas específicos de iOS; para la
secuencia de arranque consulta [Bootstrap y ciclo de vida](bootstrap.md).

El flavor iOS se incluye en el `.unitypackage` cuando el pipeline de build se ejecuta con
`-IncludeMacOS` y `-IncludeIOS`. El desarrollo iOS ocurre en un host Mac, así que el flavor
macOS es necesario junto al de iOS — sin él, el Editor en el Mac no tendría runtime nativo que
cargar al abrir el paquete. El resultado es un paquete acumulativo que contiene Windows,
Android, macOS e iOS juntos.

## Player Settings

| Ajuste | Valor | Dónde |
|---|---|---|
| Target Platform | **iOS** | File → Build Profiles → iOS |
| Target SDK | **Device SDK** | File → Build Profiles → iOS |
| Target minimum iOS Version | **15.0** o posterior | Project Settings → Player → Other Settings → Configuration |
| Architecture | **ARM64** | Project Settings → Player → Other Settings → Configuration |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **IL2CPP** *(obligatorio — Mono no está soportado en iOS por Unity)* | Project Settings → Player → Other Settings → Configuration |
| Target Device | **iPhone + iPad** | Project Settings → Player → Other Settings → Configuration |
| Enable Bitcode | **Off** *(eliminado de Xcode 14+)* | Project Settings → Player → Other Settings → Configuration |

El Editor aplica automáticamente `Api Compatibility Level = .NET Standard 2.1` para iOS cuando
se ejecuta el diálogo de configuración de un único uso del paquete. Si saltaste ese diálogo,
ajústalo manualmente.

## Entradas Info.plist obligatorias

Estas se añaden al `Info.plist` del proyecto Xcode generado. Edítalas a través de la UI
**Player Settings** de Unity o mediante un script post-process en tus scripts de Editor:

| Clave | Valor | Necesaria para |
|---|---|---|
| `NSCameraUsageDescription` | Razón por la que necesitas acceso a la cámara | Fuentes de captura de cámara |
| `NSMicrophoneUsageDescription` | Razón por la que necesitas acceso al micrófono | Fuentes de captura de micrófono |
| `NSLocalNetworkUsageDescription` | Razón por la que necesitas acceso LAN | Streams RTSP en la red local (`192.168.*`, discovery mDNS) |
| `NSAppTransportSecurity` → `NSAllowsArbitraryLoads` = `YES` | (opcional) | URLs `http://` planas y certificados `https://` / `rtsps://` autofirmados |

Sin `NSLocalNetworkUsageDescription`, iOS 14+ bloquea silenciosamente el primer intento de
conexión a cualquier dirección de red local — `RTSPSourceBlock` entonces presenta un timeout de
conexión que parece un error del lado de la cámara. Establece la cadena a algo que los
revisores del App Store aceptarán (por ejemplo, "Esta app reproduce video de cámaras IP
locales en tu red.").

Si solo streameas URLs públicas `https://` / `rtsps://` desde hosts de internet con
certificados válidos firmados por CA, puedes saltarte la excepción ATS completamente — App
Transport Security las acepta por defecto.

## Organización en disco

El paso `deploy-unity-natives.ps1 -Platform iOS` despliega el runtime iOS así:

| Ruta | Contenido |
|---|---|
| `Assets/Plugins/iOS/GStreamerX.framework/GStreamerX` | El binario Mach-O arm64 — cada plugin de GStreamer registrado estáticamente. |
| `Assets/Plugins/iOS/GStreamerX.framework/Info.plist` | Metadatos del framework. |
| `Assets/Plugins/iOS/GStreamerX.framework/Modules/module.modulemap` | Mapa de módulos Swift / Objective-C. |
| `Assets/Plugins/iOS/libVisioForge_Core.a` | El archivo estático flavor-iOS del SDK. |
| `Assets/VisioForge/link.xml` | Reglas de preservación IL2CPP (compartidas entre flavors móviles). |
| `Assets/Plugins/iOS/Managed/` | Ensamblados gestionados compilados contra `netstandard2.1` con `UNITY_NS21_IOS` definido. |

Los metadatos `PluginImporter` en la carpeta del framework la marcan como
**Add To Embedded Binaries = YES**, así que Unity embebe el framework en el proyecto Xcode
generado automáticamente. Dyld resuelve `@rpath/GStreamerX.framework/GStreamerX` cuando el
primer `[DllImport]` del SDK dispara — no se necesita configuración de ruta de búsqueda.

El paquete CA **no** es un archivo separado en iOS — es un recurso gestionado embebido dentro
de `VisioForge.Core.dll` (`VisioForge.Core.ResourcesData.ca-certificates.crt`).
`VisioForgeEnvironment.Configure()` lo extrae a `Application.persistentDataPath/ssl/certs/` al
arrancar y apunta `SSL_CERT_FILE` allí.

## Flujo Xcode

1. **File → Build Profiles → iOS → Build** — Unity produce un proyecto Xcode, no un `.ipa`
   final.
2. Abre el `.xcworkspace` (o `.xcodeproj`) generado en Xcode en el mismo Mac.
3. Pestaña **Signing & Capabilities** en el target Unity-iPhone — establece tu equipo Apple
   Developer y un bundle identifier que poseas.
4. Conecta el iPhone (o stub de simulador — ver la nota Simulator abajo), elígelo como target
   Run y pulsa **▶ Run**.

El primer build tarda unos minutos — Xcode está compilando C++ generado por IL2CPP a arm64
dispositivo. Los builds incrementales son de segundos.

!!! note "Simulator no soportado"
    `GStreamerX.framework` se distribuye solo dispositivo-arm64. El Simulator iOS (x86_64 en
    Macs Intel, arm64-sim en Apple silicon) no puede cargarlo — Xcode aborta el build con
    `Could not find module 'GStreamerX' for target 'arm64-apple-ios-simulator'`. Prueba en un
    iPhone o iPad real. Si tienes un requisito Simulator estricto, contacta con soporte.

## Tamaño de build

El flavor iOS añade aproximadamente **40 MB** al `.ipa` final:

- `GStreamerX.framework/GStreamerX` — ~38 MB binario dispositivo-arm64 tras el thinning en el enlace (desde el framework de ~68 MB en disco)
- `libVisioForge_Core.a` — enlazado dentro del binario IL2CPP, delta ~2 MB
- Ensamblados gestionados — ~3 MB

El `.ipa` comprimido es típicamente más pequeño tras el App Store thinning.

## Solución de problemas

| Síntoma | Causa | Arreglo |
|---|---|---|
| Xcode aborta: `dyld: Library not loaded: @rpath/GStreamerX.framework/GStreamerX` | `Add To Embedded Binaries` no se aplicó al slot PluginImporter del framework. | Confirma que el `.meta` de `Assets/Plugins/iOS/GStreamerX.framework` tiene `AddToEmbeddedBinaries: 1`. Si lo sustituiste manualmente, reimporta el paquete. |
| `UnityException: get_dataPath can only be called from the main thread` desde un callback de GStreamer | El primer lector de `NativesPath` corrió en un hilo de fondo antes del precargado de hilo principal de `Configure()`. | Confirma que `Configure()` se completó — imprime `[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).` en la Consola. Si está ausente, el bootstrap falló antes del precargado. |
| `MissingMethodException` de `GLib.SignalArgs` o `SignalClosure.MarshalCallback` | `link.xml` fue eliminado o el stripping gestionado IL2CPP está activo sin él. | Confirma que `Assets/VisioForge/link.xml` existe. Reimporta el paquete si falta. |
| El stream RTSP agota timeout antes de conectar en iOS 14+ | `NSLocalNetworkUsageDescription` falta — iOS bloquea la primera conexión LAN. | Añade la clave al `Info.plist` con una razón cara al usuario. |
| RTSPS / HTTPS falla con error TLS en la primera petición | La extracción del paquete CA falló silenciosamente. | Revisa la Consola para `[VisioForge] CA cert extraction failed`. El recurso embebido se distribuye en `VisioForge.Core.dll` — confirma que el DLL no fue eliminado. |
| App rechazada de revisión del App Store por "razón de privacidad ausente" | Una fuente de captura necesita `NSCameraUsageDescription` o `NSMicrophoneUsageDescription`. | Añade las claves correspondientes con razones cara al usuario. |
| Crash en el segundo `Play` en Xcode | Se llamó a `VisioForgeX.DestroySDK()` en `OnDestroy`. | No lo llames — consulta [Bootstrap y ciclo de vida](bootstrap.md#el-ciclo-de-vida-del-editor). |

## Preguntas Frecuentes

### ¿Puedo usar Mono en iOS?

No. El propio Unity no soporta Mono en iOS — IL2CPP es el único backend para builds Standalone
iOS. El SDK coincide con esa restricción.

### ¿Funciona el flavor iOS en el Simulator de iOS?

No. `GStreamerX.framework` es solo dispositivo-arm64 — ver la nota arriba. Prueba en hardware
real.

### ¿Por qué el build Xcode es el paso lento?

IL2CPP transpila cada ensamblado gestionado (tus scripts + motor Unity + el SDK) a C++, y luego
Xcode compila ese C++ para dispositivo arm64. El primer build en frío es de ~3 — 5 minutos;
los builds incrementales son de segundos porque Xcode cachea casi todo.

### ¿El SDK sube datos a los servidores de VisioForge?

No. El SDK corre completamente en proceso — sin telemetría, sin call-home de licencia, sin
analíticas de uso. El requisito `NSLocalNetworkUsageDescription` es puramente sobre las
conexiones RTSP / HTTP salientes de tu app, que iOS trata como visibles al usuario.

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — configuración del paquete
- [Bootstrap y ciclo de vida](bootstrap.md) — cómo `Configure()` arranca el runtime iOS
- [Compilar para macOS](macos.md) — el host Mac correspondiente que necesitas para compilar iOS
- [Ver una cámara RTSP en Unity](rtsp-viewer.md) — el ejemplo `RTSPViewer`
- [Solución de problemas](troubleshooting.md) — referencia de errores entre plataformas
