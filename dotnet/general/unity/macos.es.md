---
title: Compilar juego Unity de reproducción de video para macOS
description: Ajustes de build, layout de dylibs, firma de código y solución de problemas para el VisioForge Media Blocks SDK .NET en Unity 6 en macOS Standalone.
sidebar_label: Compilar para macOS
order: 55
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - macOS
  - Standalone Player
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Compilar para macOS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El flavor macOS distribuye un runtime GStreamer Universal (arm64 + x86_64) más los ensamblados
gestionados del paquete compilados contra `netstandard2.1`. El runtime nativo es un conjunto de
~300 dylibs separados — libs core de GStreamer, plugins, módulos GIO, backend OpenSSL TLS,
paquete CA — plano en `Assets/Plugins/macOS/`. Esta página cubre los ajustes del Build Profile,
el layout de dylibs y los problemas específicos de macOS; para la secuencia de arranque consulta
[Bootstrap y ciclo de vida](bootstrap.md).

El flavor macOS se incluye en el `.unitypackage` cuando el pipeline de build se ejecuta con
`-IncludeMacOS`. El resultado es un paquete acumulativo que contiene Windows-x64, Android y
macOS juntos — Unity elige los archivos correctos por Build Target a través de los metadatos
`PluginImporter` por archivo.

## Player Settings

| Ajuste | Valor | Dónde |
|---|---|---|
| Target Platform | **macOS** | File → Build Profiles → macOS |
| Architecture | **Intel 64-bit + Apple silicon** (Universal) | File → Build Profiles → macOS |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **Mono** *o* **IL2CPP** *(ambos probados)* | Project Settings → Player → Other Settings → Configuration |
| Mac App Store Validation | **Off** *(o firma primero los dylibs de GStreamer, ver abajo)* | Project Settings → Player → Other Settings |
| Enter Play Mode → Reload Domain | **Off** | Project Settings → Editor → Enter Play Mode Settings |

Ambos backends de scripting están probados. Mono es el predeterminado y es más rápido para
iterar; cambia a IL2CPP solo si tienes una razón a nivel de proyecto. El mismo `link.xml` que
distribuye el paquete preserva los tipos gestionados del SDK en cualquier backend.

## Organización en disco

El paso `deploy-unity-natives.ps1 -Platform macOS` despliega el runtime macOS así:

| Ruta | Contenido |
|---|---|
| `Assets/Plugins/macOS/libgstreamer-1.0.0.dylib` | Biblioteca core de GStreamer. |
| `Assets/Plugins/macOS/libgio-2.0.0.dylib`, `libglib-2.0.0.dylib`, `libgobject-2.0.0.dylib` | Core GLib. |
| `Assets/Plugins/macOS/libgst*.dylib` | Cada plugin de GStreamer (decoders, encoders, sources, sinks, elementos base). |
| `Assets/Plugins/macOS/libgioopenssl.so` | Backend GIO TLS por el que RTSPS / HTTPS verifican pares. |
| `Assets/Plugins/macOS/ca-certificates.crt` | Paquete CA para OpenSSL. |
| `Assets/Plugins/macOS/libVisioForge_Core.dylib` | Shim nativo del SDK. |
| `Assets/VisioForge/link.xml` | Reglas de preservación IL2CPP (compartidas con Android). |
| `Assets/Plugins/macOS/` | Ensamblados gestionados compilados contra `netstandard2.1` con `UNITY_NS21_MACOS` definido. |

El layout es plano — el `@rpath` / `@loader_path` de cada dylib está precocido por el NuGet
pack, así que una vez que dyld ha cargado uno mediante el primer `[DllImport]`, los hermanos
resuelven automáticamente.

## Build Standalone

**File → Build Profiles → macOS → Build** produce un bundle `.app`.

Dónde acaban los nativos en el bundle depende de la versión de Unity:

| Layout | Usado por | `VisioForgeEnvironment.NativesPath` resuelve a |
|---|---|---|
| `<app>.app/Contents/PlugIns/` | Unity 6 predeterminado | `…/Contents/PlugIns` |
| `<app>.app/Contents/PlugIns/macOS/` | algunas versiones patch de Unity 6 | `…/Contents/PlugIns/macOS` |
| `<app>.app/Contents/Frameworks/` | layouts Unity más antiguos | `…/Contents/Frameworks` |
| `<app>.app/Contents/Resources/Data/Plugins/macOS/` | layouts muy antiguos | `…/Contents/Resources/Data/Plugins/macOS` |

`NativesPath` sondea las cuatro ubicaciones al arrancar, buscando el centinela
`libgstreamer-1.0.0.dylib`. Gana la primera carpeta que lo contenga, y el resultado se cachea
durante el resto del proceso — no hay ajuste por versión de Unity.

## Tamaño de build

El flavor macOS añade aproximadamente **100 MB** al bundle `.app` (Universal arm64 + x86_64).
Es el más grande de los flavors del paquete acumulativo porque cada plugin se distribuye como
su propio dylib y se incluyen ambas arquitecturas. Si solo apuntas a Apple silicon, puedes
post-procesar el bundle para eliminar las slices x86_64 con `lipo`, pero el paquete en sí no
las separa por defecto.

## Firma de código y notarización

Para distribución fuera de la Mac App Store típicamente quieres firmar y notarizar el bundle:

1. **Hardened Runtime** habilitado (Project Settings → Player → Other Settings o tu flujo de
   firma).
2. **Codesign cada dylib en `Contents/PlugIns/`** con tu certificado Developer ID Application
   antes de firmar el propio `.app`. Unity no firma plugins de terceros por ti.
3. **Notariza** el `.app` final (o su `.zip` / `.dmg`) con `xcrun notarytool submit … --wait`.
4. **Staple** el ticket de notarización con `xcrun stapler staple <app-bundle>`.

Los dylibs de GStreamer no requieren ningún entitlement Apple más allá del valor predeterminado del
Hardened Runtime; no acceden a recursos protegidos desde su código nativo. Tu propia app
determina qué entitlements (cámara, micrófono, red, etc.) son requeridos.

Para envío a la Mac App Store, el `libgioopenssl.so` y `libgmp.10.dylib` incluidos están
enlazados estáticamente y se distribuyen bajo licencias permisivas, pero la revisión del App
Store puede marcar la extensión `.so` para un bundle macOS. Si necesitas distribución por App
Store, contacta con soporte — ese path no está ejercitado por la CI del paquete.

## Solución de problemas

| Síntoma | Causa | Arreglo |
|---|---|---|
| `DllNotFoundException: libgstreamer-1.0.0` en Play | `Plugins/macOS/` está vacío o falta el centinela `libgstreamer-1.0.0.dylib`. | Reimporta el `.unitypackage` con todos los ítems marcados. La Consola muestra el `NativesPath` resuelto — confirma que el centinela está allí. |
| `[VisioForge] Native runtime folder not found at '…/Contents/PlugIns'` en un build Standalone | Los plugins no se prepararon en el `.app` porque el flavor macOS no estaba en el paquete. | Reconstruye el paquete con `-IncludeMacOS` (o importa el `.unitypackage` acumulativo que incluya macOS). |
| El pipeline se cuelga al arrancar, el log muestra dos llamadas `gst_init` | Una instalación homebrew o de sistema de GStreamer está en `DYLD_LIBRARY_PATH`. | `Configure()` lo limpia — confirma que el número de eliminaciones no es cero en la Consola. Hardened Runtime elimina `DYLD_*` antes de que el proceso arranque, así que esto es principalmente una preocupación del Editor Mono. |
| RTSPS / HTTPS falla con `Peer certificate cannot be authenticated with given CA certificates` | `ca-certificates.crt` no encontrado en la ruta esperada. | `Configure()` registra una advertencia si el paquete falta. Reimporta el paquete o vuelve a ejecutar `deploy-unity-natives.ps1 -Platform macOS`. |
| El bundle lanzado desde Finder muestra el diálogo `Damaged app` | El `.app` no está firmado y se descargó con el flag de cuarentena. | Firma + notariza para distribución, o para pruebas locales ejecuta `xattr -d com.apple.quarantine <app-bundle>` una vez. |
| El bundle lanzado desde un build TestFlight de Mac App Store crashea | Los archivos `.so` en el bundle violan las reglas de layout del App Store. | Contacta con soporte — el envío a App Store necesita un empaquetado nativo alternativo. |

## Preguntas Frecuentes

### ¿Funciona el flavor macOS en el Editor en un Mac?

Sí — tanto `OSXEditor` (el propio Editor) como `OSXPlayer` (un build Standalone) son targets
de runtime admitidos. `Configure()` resuelve `Plugins/macOS/` desde la raíz del proyecto en el
Editor y sondea el layout del bundle en el player.

### ¿Necesito el flavor macOS para abrir el paquete en un Editor host Mac?

Sí. El `.unitypackage` que importas debe contener el flavor macOS (`-IncludeMacOS`) para que el
Editor host Mac encuentre un runtime nativo que cargar. Un paquete solo-Windows, abierto en un
Editor Mac, mostrará el desajuste de flavor cruzado como `[VisioForge] Native runtime folder not
found at '…' for runtime platform OSXEditor` — consulta
[Bootstrap y ciclo de vida](bootstrap.md).

### ¿Puedo distribuir solo Apple silicon y omitir x86_64?

Sí. Tras el build, ejecuta `lipo -thin arm64 <dylib> -output <dylib>` en cada `.dylib` en
`Contents/PlugIns/` para eliminar las slices Intel. El paquete no lo hace por defecto porque
ambas arquitecturas siguen siendo útiles para pruebas de compatibilidad.

### ¿Funciona el mismo paquete en iOS también?

El flavor iOS se distribuye como una plataforma separada dentro del mismo `.unitypackage`
cuando se construye con `-IncludeIOS`. Consulta [Compilar para iOS](ios.md).

## Véase también

- [Instalar el Media Blocks SDK en Unity](../../install/unity.md) — configuración del paquete
- [Bootstrap y ciclo de vida](bootstrap.md) — cómo `Configure()` encuentra el runtime macOS
- [Compilar para iOS](ios.md) — el flavor iOS correspondiente (requiere un host Mac)
- [Ver una cámara RTSP en Unity](rtsp-viewer.md) — el ejemplo `RTSPViewer`
- [Solución de problemas](troubleshooting.md) — referencia de errores entre plataformas
