---
title: Captura de video NDI en .NET - guía de integración C#
description: Descubra, conecte y capture fuentes NDI con VisioForge Video Capture SDK. Incluye guía para reproductores NDI en Android y MAUI con C# .NET.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - IP Camera
  - NDI Source
  - RTSP
  - ONVIF
  - NDI
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureCore
  - NDISourceSettings
  - NDISourceInfo
  - DeviceEnumerator
  - IPCameraSourceSettings

---

# Implementar Fuentes de Video NDI en Aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! info "Soporte multiplataforma"
    El motor `VideoCaptureCoreX` con NDI funciona en **Windows, macOS, Linux, Android e iOS** vía GStreamer; instala el runtime NDI para cada plataforma objetivo. La fuente NDI del `VideoCaptureCore` clásico es solo Windows. Consulta la [matriz de soporte de plataformas](../../../platform-matrix.md) y la [guía de despliegue en Linux](../../../deployment-x/Ubuntu.md).

## Introducción a la Tecnología NDI

Network Device Interface (NDI) es un estándar de alto rendimiento para flujos de trabajo de producción basados en IP, desarrollado por NewTek. Permite que productos compatibles con video se comuniquen, entreguen y reciban video de calidad broadcast sobre una conexión de red estándar con baja latencia.

Nuestro SDK proporciona soporte robusto para fuentes NDI, permitiendo que tus aplicaciones .NET se integren perfectamente con cámaras NDI y software habilitado para NDI. Esto lo hace ideal para entornos de producción en vivo, aplicaciones de streaming, soluciones de videoconferencia y cualquier sistema que requiera integración de video de red de alta calidad.

### Prerrequisitos para Integración NDI

Antes de implementar funcionalidad NDI en tu aplicación, necesitarás instalar uno de los siguientes:

- [NDI SDK](https://ndi.video/for-developers/ndi-sdk/#download) - Recomendado para desarrolladores construyendo aplicaciones profesionales
- NDI Tools - Suficiente para pruebas básicas y desarrollo

Estas herramientas proporcionan los componentes de tiempo de ejecución necesarios para la comunicación NDI. Después de la instalación, tu sistema podrá descubrir y conectarse a fuentes NDI en tu red.

Para reproducción en Android, instale el **NDI Advanced SDK for Android** y empaquete los archivos `libndi.so` específicos por ABI con su APK. Las muestras Android y MAUI NDI Player buscan el directorio `Lib` del SDK usando primero la propiedad MSBuild `NdiAndroidSdkLib`, después la variable de entorno `NDI_ANDROID_SDK_LIB` y finalmente `C:\Program Files\NDI\NDI 6 SDK (Android)\Lib`.

Las aplicaciones Android que descubren fuentes NDI deben solicitar estos permisos:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<uses-permission android:name="android.permission.CHANGE_WIFI_MULTICAST_STATE" />
```

## Descubrir Fuentes NDI en Tu Red

El primer paso para trabajar con NDI es enumerar las fuentes disponibles. Nuestro SDK hace este proceso sencillo con métodos dedicados para escanear tu red en busca de dispositivos y aplicaciones habilitados para NDI.

### Enumerar Fuentes NDI Disponibles

=== "VideoCaptureCore"


    ```cs
    var lst = await VideoCapture1.IP_Camera_NDI_ListSourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri);
    }
    ```


=== "VideoCaptureCoreX"


    ```cs
    var lst = await DeviceEnumerator.Shared.NDISourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri.URL);
    }
    ```



Los métodos de enumeración asíncronos escanean tu red y devuelven una lista de fuentes NDI disponibles. Cada fuente tiene un identificador único que usarás para establecer una conexión. El proceso de enumeración típicamente toma unos segundos, dependiendo de las condiciones de red y el número de fuentes disponibles.

Para interfaces de reproducción, suscríbase a `NDISourcesChanged` e inicie el watcher para que la lista de fuentes refleje transmisores que aparecen o desaparecen después del escaneo inicial:

```cs
DeviceEnumerator.Shared.NDISourcesChanged += OnNDISourcesChanged;
DeviceEnumerator.Shared.StartNDISourceWatch();
```

Detenga el watcher y cancele la suscripción durante el cierre de la aplicación.

## Conectar a Fuentes NDI

Una vez que has identificado las fuentes NDI en tu red, el siguiente paso es establecer una conexión. Esto implica crear el objeto de configuración apropiado y configurarlo para tus requisitos específicos.

### Configurar Ajustes de Fuente NDI

=== "VideoCaptureCore"


    ```cs
    // Crear un objeto de configuración de fuente de cámara IP
    settings = new IPCameraSourceSettings
    {
        URL = new Uri("URL de fuente NDI")
    };

    // Establecer el tipo de fuente a NDI
    settings.Type = IPSourceEngine.NDI;

    // Habilitar o deshabilitar captura de audio
    settings.AudioCapture = false;

    // Establecer información de inicio de sesión si es necesario
    settings.Login = "usuario";
    settings.Password = "contraseña";

    // Establecer la fuente de cámara IP
    VideoCapture1.IP_Camera_Source = settings;

    // Selecciona el modo operativo antes de StartAsync:
    //   IPPreview — solo vista previa en vivo (sin salida a archivo).
    //   IPCapture — vista previa + grabación al destino Output_Format configurado.
    VideoCapture1.Mode = VideoCaptureMode.IPPreview;   // o VideoCaptureMode.IPCapture

    await VideoCapture1.StartAsync();
    ```


=== "VideoCaptureCoreX"


    En VideoCaptureCoreX tiene dos opciones para crear la configuración de fuente NDI:

    **Opción 1: Usando la URL de la fuente NDI**

    ```cs
    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), null, "URL NDI");
    ```

    **Opción 2: Usando el nombre de la fuente NDI**

    ```cs
    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), cbIPURL.Text, null);
    ```

    Finalmente, asigne la fuente al objeto VideoCaptureCoreX:

    ```cs
    VideoCapture1.Video_Source = ndiSettings;
    ```



## Patrón de Reproductor NDI en Android y MAUI

Las demos `Video Capture SDK X` Android y MAUI NDI Player usan `VideoCaptureCoreX` como receptor/reproductor NDI simple. El mismo flujo sirve para reproductores de pantalla completa, herramientas de monitorización y paneles de previsualización:

1. Inicialice el SDK.
2. Enumere fuentes NDI con `DeviceEnumerator.Shared.NDISourcesAsync()`.
3. Mantenga la lista actualizada con `NDISourcesChanged` y `StartNDISourceWatch()`.
4. Cree `NDISourceSettings` desde la `NDISourceInfo` seleccionada.
5. Asigne la configuración a `VideoCaptureCoreX.Video_Source`.
6. Habilite reproducción de audio solo cuando la fuente seleccionada exponga streams de audio.
7. Detenga y libere el core cuando se detenga la reproducción o se cierre la vista.

```cs
var sources = await DeviceEnumerator.Shared.NDISourcesAsync();
var info = sources[0];

var settings = await NDISourceSettings.CreateAsync(null, info);
if (settings == null || !settings.IsValid())
{
    throw new InvalidOperationException("No se pudo crear la configuración de fuente NDI.");
}

var core = new VideoCaptureCoreX(videoView);
core.Video_Source = settings;
core.Audio_Play = settings.GetInfo()?.AudioStreams?.Count > 0;

await core.StartAsync();
```

### UI Android Nativa

La muestra Android usa `VisioForge.Core.UI.Android.VideoViewGL` como superficie de renderizado:

```cs
var core = new VideoCaptureCoreX(videoView);
```

En Android, solicite los permisos de red y multicast antes del descubrimiento. Si la aplicación se compila sin `libndi.so` para el ABI actual, la reproducción fallará en tiempo de ejecución con `DllNotFoundException`; verifique la ruta `Lib` del NDI Advanced SDK antes de empaquetar.

### UI .NET MAUI

La muestra MAUI registra los handlers de VisioForge en `MauiProgram`:

```cs
builder
    .UseMauiApp<App>()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

Cree `VideoCaptureCoreX` con la vista de plataforma obtenida desde el `VideoView` de MAUI:

```cs
var core = new VideoCaptureCoreX(videoView.GetVideoView());
```

Durante el cierre, cancele enumeraciones pendientes, cancele la suscripción a `NDISourcesChanged`, llame a `StopNDISourceWatch()`, detenga y libere `VideoCaptureCoreX`, y destruya el SDK si su aplicación controla el ciclo de vida del SDK.


## Capturar Video desde Fuentes NDI

Después de configurar la fuente NDI, puedes iniciar la captura y vista previa de video.

### Vista Previa de Video en Vivo

```cs
// Iniciar vista previa
await VideoCapture1.StartAsync();
```

### Grabar a Archivo

```cs
// Configurar salida de archivo
var mp4Output = new MP4Output("output.mp4");
VideoCapture1.Outputs_Add(mp4Output);

// Iniciar grabación
await VideoCapture1.StartAsync();
```

## Características Avanzadas de NDI

### Baja Latencia

NDI está diseñado para transmisión de video de baja latencia. Para entornos de producción en vivo, puedes optimizar ajustes para latencia mínima:

```cs
// Habilitar modo de baja latencia si está disponible
ndiSource.LowLatencyMode = true;
```

### Tally y Comunicación Bidireccional

NDI soporta señales de tally para indicar cuando una fuente está al aire. Esto es útil para entornos de producción en vivo:

```cs
// Las señales de tally se manejan automáticamente cuando corresponde
```

## Mejores Prácticas

1. **Configuración de Red**: Asegúrate de que tu red soporte tráfico multicast para descubrimiento NDI óptimo
2. **Ancho de Banda**: Los flujos NDI pueden consumir ancho de banda significativo; planifica tu capacidad de red apropiadamente
3. **Latencia**: Usa conexiones por cable cuando sea posible para la menor latencia
4. **Firewall**: Asegúrate de que los puertos NDI estén abiertos en tu firewall
5. **Bibliotecas nativas Android por ABI**: Para receptores Android, incluya `libndi.so` para cada ABI que distribuya. Las bibliotecas faltantes producen fallos en tiempo de ejecución aunque el APK compile.
6. **Ciclo de vida móvil**: Detenga la reproducción cuando se detengan actividades Android o se cierren páginas/ventanas MAUI, libere el core, cancele la enumeración de fuentes y quite los manejadores de eventos del watcher.

## Solución de Problemas

### No se Descubren Fuentes
- Verificar que NDI SDK o Tools estén instalados
- Asegurar que los dispositivos estén en la misma red
- Comprobar configuración del firewall

### Problemas de Calidad de Video
- Verificar capacidad de ancho de banda de red
- Revisar configuración de calidad del encoder en la fuente

### Problemas de Latencia
- Usar conexiones por cable en lugar de WiFi
- Optimizar configuración del buffer

## Aplicaciones de Ejemplo

Explora estas aplicaciones de ejemplo para ver integración NDI en acción:

- [Demo Fuente NDI (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/NDI%20Source%20Demo)
- [NDI Player (Android)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/Android/NDIPlayer)
- [NDI Player (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/MAUI/NDIPlayer)
- [Demo Principal de Video Capture (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)

## Documentación Relacionada

- [Visión general de cámaras IP](index.md) — tipos de fuentes RTSP, ONVIF y NDI de un vistazo
- [Configuración de fuente de cámara RTSP](rtsp.md) — protocolo de cámara IP más común
- [Integración de cámara IP ONVIF](onvif.md) — descubrimiento y control PTZ estándar
- [Tutorial de vista previa en vivo de cámara IP](../../video-tutorials/ip-camera-preview.md) — ejemplo mínimo de vista previa
- [Inmersión profunda en el protocolo RTSP](../../../general/network-streaming/rtsp.md) — internos del protocolo de streaming
- [Guía de salida streaming NDI](../../../general/network-streaming/ndi.md) — enviar cámaras, dispositivos de captura y archivos a NDI

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.
