---
title: Solución de problemas del Media Blocks SDK en Unity
description: Errores comunes y arreglos para Media Blocks SDK .NET en Unity 6 — bootstrap, nativos ausentes, stripping IL2CPP, TLS y ciclo de vida.
sidebar_label: Solución de problemas
order: 60
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Troubleshooting
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Solución de problemas

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Esta página recoge los síntomas que tiene más probabilidades de encontrar y la causa raíz de
cada uno. Los errores están agrupados por categoría. Las páginas por plataforma
([Windows](windows.md), [Android](android.md), [macOS](macos.md), [iOS](ios.md)) también
tienen una tabla de solución de problemas específica del target — consulte ambas.

## Bootstrap e inicialización

### `[VisioForge] Native runtime not found at <path>`

`VisioForgeEnvironment.Configure()` no pudo encontrar la carpeta de nativos incluida en disco.
Causas:

- La importación del `.unitypackage` fue parcial. Reimporte con todos los ítems marcados.
- En Standalone macOS, el Build Target no incluyó el flavor macOS — el paquete se construyó sin
  `-IncludeMacOS`. Reconstruya el paquete o importe la variante acumulativa.
- En Android, el paso de preparación del flavor por Build Target no se ejecutó. Abra
  `Assets/Plugins/Android/libs/arm64-v8a/` en la ventana Project y confirme que
  `libgstreamer_android.so` está presente.

### `[VisioForge] InitializeSdk() called before Configure() succeeded`

Una rama de plataforma de `Configure()` falló y dejó `s_envConfigured = false`. La línea
anterior de la Consola (`[VisioForge] Android GStreamer init failed: …`,
`[VisioForge] SetDllDirectoryW failed (Win32 error …)`, etc.) explica por qué. Arregle el
problema subyacente y deje que `Configure()` reintente en la siguiente carga de escena.

### `UnityException: get_dataPath can only be called from the main thread`

Un hilo de fondo dentro del SDK o su script leyó `Application.dataPath` (o
`Application.streamingAssetsPath`, o `Application.platform`) sin pasar por la ruta cacheada. El
arreglo:

- En iOS, `Configure()` precarga `s_cachedNativesPath` en el hilo principal — confirme que la
  Consola muestre `[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).`.
  Si está ausente, el bootstrap abortó antes del precargado y el siguiente lector golpea la
  ruta perezosa fuera del hilo principal.
- En su propio código, no llame a la API Unity desde un `Task.Run`, callback pad-added de
  GStreamer o handler de señal del bus. Marshallee la llamada de vuelta al hilo principal con
  `UnitySynchronizationContext` o estableciendo un flag que el método `Update()` revise.

### `InvalidOperationException: Unity Android bootstrap requires com.unity3d.player.UnityPlayer.currentActivity to be available`

El bootstrap Java de Android no pudo obtener un `currentActivity` no nulo en `BeforeSceneLoad`.
Ocurre en Wear OS, variantes Android TV sin `UnityPlayerActivity` y hosts Unity-as-a-library
que aún no han asignado el campo. Difiera `Configure()` a su primer evento de Activity
observable:

```csharp
private void Start()
{
    if (!VisioForgeIsConfigured())
        VisioForgeEnvironment.Configure();   // re-ejecuta tras Activity lista
    VisioForgeEnvironment.InitializeSdk();
}
```

`Configure()` es idempotente — una llamada redundante tras una exitosa es inocua.

## Bibliotecas ausentes

### `DllNotFoundException: gstreamer-1.0-0`

Windows: la carpeta de nativos falta en `Assets/StreamingAssets/VisioForge/x64/`. Reimporte el
paquete. Si está ejecutando un build Standalone, confirme que
`<game>_Data/StreamingAssets/VisioForge/x64/` también está poblada — Unity la copia
literalmente, así que una carpeta ausente en el build significa que ya faltaba en el proyecto.

### `DllNotFoundException: libgstreamer_android`

Android: el Scripting Backend está en **Mono**. Cambie a **IL2CPP** en Project Settings →
Player → Other Settings → Configuration. Mono no está soportado en Android por el propio
Unity.

### `DllNotFoundException: libgstreamer-1.0.0` *(macOS)*

El bundle `.app` no contiene `libgstreamer-1.0.0.dylib` en ninguna de las ubicaciones sondeadas
(`Contents/PlugIns`, `Contents/PlugIns/macOS`, `Contents/Frameworks`,
`Contents/Resources/Data/Plugins/macOS`). Reconstruya el paquete con `-IncludeMacOS` y vuelva a
exportar el build Standalone.

### `dyld: Library not loaded: @rpath/GStreamerX.framework/GStreamerX`

iOS: el slot PluginImporter del framework no obtuvo **Add To Embedded Binaries = YES**.
Reimporte el paquete; el archivo `.meta` del paquete marca el framework correctamente. Si
sustituyó el framework manualmente, restaure el `.meta` desde una importación fresca.

## IL2CPP / stripping gestionado

### `MissingMethodException: GLib.SignalArgs..ctor` *(o tipos GStreamer / GLib similares)*

IL2CPP eliminó un tipo gestionado al que el SDK accede por reflexión. `Assets/VisioForge/link.xml` preserva estos tipos — confirme que el archivo existe en su proyecto. Si lo
eliminó accidentalmente, reimporte el `.unitypackage`. **No** edite `link.xml` para
quitar reglas; el paquete distribuye un conjunto probado.

### `MarshalDirectiveException` en la primera llamada al SDK

Una signatura `[DllImport]` fue eliminada o falló el marshalling de su delegado. Misma causa
raíz que `MissingMethodException` — confirme que `link.xml` está en su sitio y que IL2CPP no
está configurado con un nivel de stripping extra-agresivo que lo anule.

## TLS / red

### El stream RTSP agota timeout antes de conectar *(iOS 14+)*

iOS bloquea el primer intento de conexión a cualquier dirección de red local hasta que su
`Info.plist` declare por qué su app necesita acceso LAN. Añada:

```xml
<key>NSLocalNetworkUsageDescription</key>
<string>Esta app reproduce video de cámaras IP locales en tu red.</string>
```

Los revisores del App Store esperan una razón cara al usuario, no boilerplate.

### RTSPS / HTTPS falla con error TLS / verificación de certificado

El backend OpenSSL TLS de GIO no pudo encontrar un paquete CA:

- Windows / macOS: `Configure()` establece `SSL_CERT_FILE` al `ca-certificates.crt` incluido.
  Si falta, la Consola registra `[VisioForge] CA certificate bundle not found at …`. Vuelva a
  preparar los nativos mediante `deploy-unity-natives.ps1` y reconstruya.
- Android / iOS: el paquete CA es un recurso gestionado embebido extraído a
  `<filesDir>/ssl/certs/`. Si la extracción falla, la Consola registra `[VisioForge] CA cert
  extraction failed`. Confirme que `VisioForge.Core.dll` está en
  `Assets/Plugins/Android/` (Android) o `Assets/Plugins/iOS/Managed/` (iOS).

Para certificados autofirmados, o instálelos en el almacén de confianza del sistema (fuera del
alcance del SDK) o — solo para pruebas — deshabilite la verificación de pares en el bloque
fuente. El nombre de la propiedad a nivel de bloque varía; consulte
[Ver una cámara RTSP en Unity](rtsp-viewer.md) para el ejemplo RTSP.

### URLs `http://` planas fallan en iOS

App Transport Security (ATS) bloquea `http://` por defecto. O cambie a `https://` o, si debe
mantener `http://`, añada `NSAppTransportSecurity → NSAllowsArbitraryLoads = YES` a su
`Info.plist`. Tenga en cuenta que los revisores del App Store pueden preguntar por qué lo
necesita.

## Ciclo de vida del Editor

### El Editor se cuelga en "Reloading domain" tras Play / Stop

El Domain Reload predeterminado de Unity está totalmente soportado y no debería colgar el
Editor: el paquete instala un guard de recarga que detiene el hilo del bucle principal GLib de
GStreamer antes de cada recarga de dominio. Si aun así ve un cuelgue, está en una versión del
SDK anterior a esta release (previa a la incorporación del guard) — actualice a la versión más
reciente del SDK. No necesita desactivar Domain Reload.

### El Editor crashea en el segundo Play

El SDK fue apagado en Stop y reinicializado en Play. El arreglo:

- No llame a `VisioForgeX.DestroySDK()` en `OnDestroy` ni en ningún otro sitio. El SDK es
  global al proceso y se reusa entre sesiones Play.
- Los players de ejemplo (`MediaBlocksPlayer`, `RTSPViewerPlayer`) siguen este patrón — copia
  la forma de su teardown (libera solo el pipeline por-Play; nunca destruye el SDK).

Consulte [Bootstrap y ciclo de vida](bootstrap.md#el-ciclo-de-vida-del-editor) para la
explicación completa.

### El Editor se vuelve inestable tras una sesión de edición larga

Recompilaciones de scripts repetidas acumulan estado de GStreamer a través de las recargas de
dominio. Reinicie el Editor para recuperar. Esto es principalmente cosmético — los builds
Standalone Player no lo exhiben.

## Build / empaquetado

### El bundle lanzado desde Finder muestra "Damaged app" *(macOS)*

El `.app` no está firmado y se descargó con el flag de cuarentena. Para distribución, firme y
notarice el bundle (consulte [Compilar para macOS](macos.md#firma-de-codigo-y-notarizacion)).
Para pruebas locales solamente, ejecute `xattr -d com.apple.quarantine <app-bundle>` una vez.

### App rechazada de revisión del App Store por "razón de privacidad ausente" *(iOS)*

Una fuente de captura necesita una clave `Info.plist` explícita:

- `NSCameraUsageDescription` si su app captura desde la cámara
- `NSMicrophoneUsageDescription` si su app captura desde el micrófono
- `NSLocalNetworkUsageDescription` si su app habla con un dispositivo LAN

La cadena cara al usuario debe describir el uso real, no "Required by SDK".

### `[VisioForge] Native runtime folder not found at '…' for runtime platform …`

El `.unitypackage` que importó no contiene un flavor para el Build Target actual. Por
ejemplo, un paquete solo-Windows abierto en un Editor host Mac muestra esto en la primera
llamada `InitializeSdk()`. Reconstruya (o re-descargue) el paquete con el switch `-Include*`
correspondiente, o importa la variante acumulativa que contenga todas las plataformas.

## Cuando todo lo demás falla

Recoja un log y contacte con soporte:

1. Exportación de la Consola del Editor o Player (`Window → General → Console`, clic derecho
   → Save All / vía Logcat / vía Xcode → Devices → Open Console).
2. El nombre del archivo `.unitypackage` y su fecha de build.
3. Versión de Unity, Build Target, Scripting Backend, Api Compatibility Level.
4. Una escena reproducible mínima si es posible.

Abra un ticket en <https://helpdesk.visioforge.com/helpdesk/my-tickets/new>.

## Véase también

- [Bootstrap y ciclo de vida](bootstrap.md) — cómo se arranca el runtime en cada plataforma
- [Compilar para Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
  — tablas de solución de problemas específicas por plataforma
- [Matriz de plataformas](platform-matrix.md) — soporte de funciones por plataforma Unity
