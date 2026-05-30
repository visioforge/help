---
title: Bootstrap del SDK en Unity — Configura el runtime nativo
description: Cómo VisioForgeEnvironment.Configure arranca el runtime nativo GStreamer en Unity 6 en Windows, Android, macOS e iOS más el ciclo de vida del Editor.
sidebar_label: Bootstrap y ciclo de vida
order: 52
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Bootstrap
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - VisioForgeX
---

# Bootstrap y ciclo de vida

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El paquete de Unity incluye un asistente estático, `VisioForgeEnvironment`, que pone en marcha el
runtime nativo incluido antes de que cargue la primera escena. No lo invocas desde tus scripts:
Unity lo llama automáticamente mediante `RuntimeInitializeOnLoadMethod`. Esta página explica qué
hace en cada plataforma y las reglas de ciclo de vida que tus scripts deben respetar.

Si solo necesitas distribuir el SDK, puedes saltar esta página: arrastra `MediaBlocksPlayer` o
`RTSPViewerPlayer` a una escena y pulsa **Play**. Vuelve aquí cuando escribas tus propios
scripts, encuentres un error en tiempo de ejecución o quieras entender por qué los ajustes del
Editor son obligatorios.

## Los dos puntos de entrada

`VisioForgeEnvironment` tiene exactamente dos métodos públicos con los que tu código interactúa:

| Método | Cuándo se ejecuta | Qué hace |
|---|---|---|
| `Configure()` | Automáticamente, **antes de cargar la primera escena** (`[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]`). | Prepara el runtime nativo para la plataforma actual — ruta de búsqueda DLL, variables de entorno, paquete CA, bootstrap Java. Idempotente. |
| `InitializeSdk()` | Lo llamas una vez antes de construir un pipeline. Los players de ejemplo lo hacen en `Start()`. | Llama a `VisioForgeX.InitSDK()` y escanea el registro de plugins incluido. Idempotente. |

Ambos métodos son estáticos. Ambos marcan su finalización solo tras ejecutar con éxito cada
paso — así un fallo recuperable (por ejemplo, el bootstrap Java de Android ejecutándose antes
de que `currentActivity` esté lista) deja el flag sin marcar y una llamada posterior vuelve a
intentarlo en lugar de fallar silenciosamente en un estado roto.

## Qué hace `Configure()` en cada plataforma

`Configure()` despacha sobre símbolos de compilación (`UNITY_STANDALONE_WIN`,
`UNITY_STANDALONE_OSX`, `UNITY_ANDROID`, `UNITY_IOS`) — Unity define exactamente uno por target
de build. El cuerpo de cada rama es lo mínimo que la plataforma necesita para que GStreamer
encuentre sus plugins, localice sus raíces TLS y resuelva el resto del runtime incluido a través
de su cargador nativo.

=== "Windows"

    1. Valida que la carpeta de nativos incluida existe (`StreamingAssets/VisioForge/x64`).
       Rechaza modificar cualquier estado del proceso si no existe — una carpeta ausente no debe
       envenenar el `PATH` del proceso.
    2. Elimina cualquier entrada de GStreamer del sistema del **PATH del proceso** (una
       instalación homebrew / sistema en `PATH` cargaría una segunda copia física de
       `gstreamer-1.0-0.dll`, registraría tipos GLib por duplicado y colgaría el pipeline).
    3. Apunta el cargador DLL Win32 a los nativos incluidos mediante `SetDllDirectoryW`.
    4. Antepone la carpeta de nativos al `PATH` del proceso para que las dependencias core-lib
       transitivas de cada plugin se resuelvan (`SetDllDirectory` por sí solo no basta para
       esas).
    5. Establece `GST_PLUGIN_PATH` / `GST_PLUGIN_SYSTEM_PATH` a la misma carpeta plana.
    6. Establece `SSL_CERT_FILE` y `CA_CERTIFICATES` al `ca-certificates.crt` incluido para que
       RTSPS y HTTPS verifiquen pares.

    El `PATH` del usuario / sistema nunca se toca — solo la copia viva del proceso.

=== "macOS"

    1. Resuelve la ruta de nativos sondeando los layouts conocidos de Unity: `Plugins/macOS`
       (Editor y algunas versiones del Standalone Player), luego `Contents/PlugIns`,
       `Contents/PlugIns/macOS`, `Contents/Frameworks` y
       `Contents/Resources/Data/Plugins/macOS` para un Standalone `.app`. Gana la primera carpeta
       que contenga `libgstreamer-1.0.0.dylib`. El resultado se cachea.
    2. Elimina cualquier GStreamer de sistema / homebrew de `DYLD_LIBRARY_PATH`
       (`/opt/homebrew/lib`, `/usr/local/lib`) — el mismo modo de fallo de doble init que la
       limpieza del `PATH` de Windows.
    3. Establece `GST_PLUGIN_PATH` / `GST_PLUGIN_SYSTEM_PATH` a la carpeta de nativos para que
       GStreamer pueda enumerar los plugins incluidos.
    4. Establece `GIO_MODULE_DIR` para que GIO encuentre `libgioopenssl.so` (el backend TLS por
       el que RTSPS / HTTPS verifican pares).
    5. Establece `SSL_CERT_FILE` y `CA_CERTIFICATES` al paquete CA incluido.

    Cada `.dylib` incluido tiene su `@rpath` / `@loader_path` precocido por el NuGet pack, así
    que una vez que dyld ha cargado uno mediante el primer `[DllImport]`, los demás resuelven
    hermanos automáticamente — no se necesita equivalente de `SetDllDirectory`.

=== "Android"

    1. Adquiere `com.unity3d.player.UnityPlayer.currentActivity` mediante JNI. Si el campo es
       null — variantes Wear OS / Android TV, hosts Unity-as-a-library que aún no lo han
       asignado, paths de arranque `BeforeSceneLoad` muy tempranos en algunos Unity 6 — falla
       rápido con una excepción descriptiva para que el cliente vea algo mejor que un
       `NullReferenceException` opaco desde el interior de JNI.
    2. Resuelve `getFilesDir()` y `getCacheDir()` desde la Activity.
    3. Captura los valores previos de `TMP`, `TEMP`, `TMPDIR`, `XDG_RUNTIME_DIR`,
       `XDG_CACHE_HOME`, `HOME`, `GST_REGISTRY`, `SSL_CERT_FILE`, `CA_CERTIFICATES` para que un
       fallo posterior pueda revertirlos.
    4. Apunta GLib a los directorios escribibles privados de la app estableciendo las variables
       anteriores (solo los directorios privados de la app son escribibles en Android).
    5. Extrae el recurso embebido `ca-certificates.crt` a `filesDir/ssl/certs/` y apunta
       `SSL_CERT_FILE` / `CA_CERTIFICATES` allí.
    6. Llama a `org.freedesktop.gstreamer.GStreamer.init(activity)` — esto carga
       `libgstreamer_android.so`, captura la JavaVM en `JNI_OnLoad`, ejecuta `gst_init` y
       registra cada plugin enlazado estáticamente al monolito.
    7. Si algo de lo anterior lanza, restaura el entorno capturado y relanza.

=== "iOS"

    1. Extrae el recurso embebido `ca-certificates.crt` a `Application.persistentDataPath/ssl/certs/`.
    2. Establece `SSL_CERT_FILE` / `CA_CERTIFICATES` a esa ruta para que el backend OpenSSL de
       GIO pueda verificar pares RTSPS / HTTPS.
    3. Precarga la caché de `NativesPath` desde el hilo principal (`s_cachedNativesPath =
       Application.dataPath.Replace('\\', '/')`). Sin esta precarga, un lector en un hilo de
       fondo — el bus de GStreamer, un callback de log async, un callback pad-added — golpearía
       después al getter perezoso y llamaría a `Application.dataPath` fuera del hilo principal,
       lo que lanza `UnityException`. La fórmula de cálculo coincide con el getter perezoso
       byte a byte.

    iOS no necesita un escaneo de plugins: cada plugin de GStreamer está registrado
    estáticamente dentro de `GStreamerX.framework` en el momento de
    `gst_plugin_register_static()`, y dyld resuelve `@rpath/GStreamerX.framework/GStreamerX`
    automáticamente cuando dispara el primer `[DllImport]`.

=== "Otra"

    `Configure()` establece el flag de éxito y registra una advertencia. No existe runtime
    nativo para la plataforma actual, así que cualquier `[DllImport]` posterior lanzaría
    `DllNotFoundException`. Compila para una de las cuatro plataformas soportadas.

## Qué hace `InitializeSdk()`

Tras que `Configure()` haya preparado el runtime, `InitializeSdk()` termina la puesta en marcha:

1. Rechaza ejecutarse si `Configure()` nunca tuvo éxito — fallar rápido aquí saca un error
   accionable en lugar de un `DllNotFoundException` desde el interior del SDK.
2. Rechaza ejecutarse en un valor de `Application.platform` no soportado en tiempo de ejecución
   (se admiten Windows / Android / macOS / iOS; cualquier otro provoca un cortocircuito con
   advertencia).
3. En Windows y macOS, antes de llamar al SDK nativo, vuelve a comprobar que la carpeta de
   nativos resuelta exista en disco. Esto detecta el desajuste de flavor cruzado (un
   `.unitypackage` solo-Windows importado en un host macOS, o lo contrario) con un mensaje claro
   en lugar de un `DllNotFoundException` opaco. Android e iOS omiten esta comprobación (no hay
   carpeta que sondear).
4. Llama a `VisioForgeX.InitSDK()`. Captura y registra en caso de fallo; deja el flag sin
   marcar para que un reintento posterior pueda tener éxito.
5. En Windows y macOS, escanea explícitamente la carpeta de plugins incluida con
   `Gst.Registry.Get().ScanPath(NativesPath)`. El escaneo in-process de plugins de Unity no
   honra fiablemente `GST_PLUGIN_PATH` en ninguna de las dos plataformas; el escaneo explícito
   es lo que hace que bloques como `BufferSinkBlock` (que depende de `appsink`) se carguen.
   Android registra plugins estáticamente en `GStreamer.init`; iOS los registra estáticamente
   en el framework — ambos omiten el escaneo.
6. Establece el flag de éxito.

Los players de ejemplo (`MediaBlocksPlayer`, `RTSPViewerPlayer`) llaman a `InitializeSdk()`
desde su método `Start()`, antes de construir un pipeline. Tus scripts deben seguir el mismo
patrón.

## El ciclo de vida del Editor

El SDK se inicializa una vez por proceso del Editor y se reusa a través de las sesiones
**Play → Stop → Play**. Dos consecuencias:

- **Disable Domain Reload es obligatorio.** Con él habilitado, salir de modo Play dispara una
  recarga de dominio mientras el hilo del bucle principal GLib del SDK aún corre, lo que puede
  colgar el Editor. El diálogo de ajustes del Editor que el paquete muestra al primer import lo
  configura por ti; ajústalo manualmente en **Edit → Project Settings → Editor → Enter Play
  Mode Settings** si saltaste ese diálogo.
- **No llames a `VisioForgeX.DestroySDK()` en Stop ni en `OnDestroy`.** `gst_deinit` de
  GStreamer no puede reinicializarse en el mismo proceso — destruir el SDK en Stop e intentar
  usarlo de nuevo en el siguiente Play crashea dentro del registro nativo. Los players de
  ejemplo siguen esta regla: su `OnDestroy` solo libera el pipeline por-Play. El SDK
  permanece vivo durante el resto del ciclo de vida del proceso.

Hay un guard Editor-only que el paquete instala automáticamente: un
`VisioForgeEditorReloadGuard` que llama a `VisioForgeX.StopMainLoop()` en
`beforeAssemblyReload` y `EditorApplication.quitting`. El bucle principal GLib corre en un hilo
de fondo dedicado, bloqueado dentro de una llamada nativa que Unity no puede abortar — sin este
guard, la recarga de dominio que sigue a una recompilación de script se colgaría. El guard
**no** llama a `DestroySDK` (ver arriba); solo para el hilo del bucle, y el siguiente Play
reconstruye el bucle. Este guard es interno — tus scripts deben ignorarlo.

## Preguntas Frecuentes

### ¿Tengo que llamar a `Configure()` manualmente?

No. El atributo `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` de Unity lo ejecuta por ti
antes de que cargue la primera escena. La única vez que lo volverías a llamar es desde un path
de recuperación personalizado cuando un intento anterior falló — y `Configure()` es
idempotente, así que una llamada redundante es inocua.

### ¿Por qué `Configure()` modifica variables de entorno en lugar de pasar argumentos?

GLib lee `GST_PLUGIN_PATH`, `GIO_MODULE_DIR`, `SSL_CERT_FILE`, `HOME`, etc. del `environ` C
directamente durante `gst_init` y de nuevo en el primer uso TLS. El SDK no tiene una API para
sobrescribirlos — la única forma correcta de apuntar el runtime a los assets incluidos es fijar
las variables antes de construir cualquier pipeline. Las mutaciones se limitan al proceso; los
entornos de usuario y sistema quedan intactos.

### ¿Qué pasa si llamo a `InitializeSdk()` antes de que `Configure()` haya tenido éxito?

Registra un error y retorna. El flag de éxito queda sin marcar para que un reintento posterior
pueda tener éxito una vez que `Configure()` funcione. Este guard existe porque `InitSDK()` de
otro modo crashearía dentro del código nativo con un error mucho menos accionable.

### ¿Puedo ejecutar dos pipelines en paralelo?

Sí. `InitializeSdk()` arranca el SDK una vez por proceso; después puedes construir tantas
instancias de `MediaBlocksPipeline` como quieras. Cada una es independiente — el patrón de
muestra multi-cámara RTSP consiste en adjuntar un `RTSPViewerPlayer` por `RawImage`, y cada uno
construye y destruye su propio pipeline.

## Véase también

- [Uso de VisioForge en Unity](index.md) — visión general del paquete y cómo funciona el rendering
- [Compilar para Windows](windows.md) — ajustes del Editor y Standalone para Windows
- [Compilar para Android](android.md) — ajustes IL2CPP para Android
- [Compilar para macOS](macos.md) — ajustes del Standalone para macOS
- [Compilar para iOS](ios.md) — ajustes de dispositivo para iOS
- [Solución de problemas](troubleshooting.md) — errores comunes de bootstrap y runtime
