---
title: Historial de Versiones y Cambios en SDKs .NET
description: Registro de cambios para los SDKs de video .NET, con Video Capture, Media Player, Video Edit y Media Blocks con las últimas características y correcciones.
hide_table_of_contents: true
---

# Registro de Cambios

Cambios y actualizaciones para todos los SDKs .Net.

## 2025.11.4

* Soporte de .Net 10 para todos los SDKs

## 2025.11.3

* Actualización de WPF VideoView: Se agregaron las propiedades RotationAngle, RotateCrop y RotationStretch para admitir la renderización de video rotado

## 2025.11.1

* [Media Blocks SDK .Net] Se agregó soporte de grupo de superposición sincronizada para OverlayManagerBlock

## 2025.10.10

* [**Windows SDKs**] VideoEffectRotate actualizado con opción sin recorte

## 2025.10.6

### 🚀 Característica Principal: Transmisión RTSP de Latencia Ultra Baja

* **[Media Blocks SDK .Net]** Modo revolucionario de baja latencia para fuentes RTSP logrando **60-120ms de latencia total** (mejora de 10-14x sobre el valor predeterminado de 1-2 segundos)
  * Se agregó la propiedad `RTSPSourceSettings.LowLatencyMode` para habilitar la transmisión optimizada en una línea
  * Optimización automática de la tubería: fuente RTSP (80ms), búferes de cola (10-20ms) y control de sincronización del renderizador
  * Integración con GStreamer: `latency=80ms`, `buffer-mode=0`, cola `max-size-buffers=2` con `leaky=downstream`
  * Perfecto para vigilancia en tiempo real, sistemas de seguridad, monitoreo en vivo y aplicaciones de video interactivo

* **[Media Blocks SDK .Net]** RTSPSourceBlock mejorado con configuración completa de baja latencia
  * Se agregó la enumeración `RTSPBufferMode` con 5 modos (None, Auto, Slave, Buffer, Synced) para un control detallado del búfer de fluctuación
  * Se agregó la enumeración `RTSPNTPTimeSource` (NTP, RunningTime, Clock) para la sincronización de marca de tiempo NTP en escenarios de múltiples cámaras
  * Nuevas propiedades: `LowLatencyMode`, `BufferMode`, `DropOnLatency`, `NTPSync`, `NTPTimeSource`
  * `QueueElement` optimizado con configuración automática de baja latencia (máximo 2 fotogramas, modo leaky downstream)

* **[Video Capture SDK X .Net]** Soporte completo de modo de baja latencia para fuentes RTSP
  * Compatible con el motor `VideoCaptureCoreX` en todas las plataformas
  * Misma API simple: `RTSPSourceSettings.LowLatencyMode = true`
  * Funciona perfectamente con la demostración de Captura IP y la demostración RTSP MultiView

* **[Soporte Multiplataforma]** Transmisión RTSP de baja latencia ahora disponible en todas las plataformas:
  * Windows (WPF, WinForms, Consola, Blazor)
  * macOS (MAUI, Consola)
  * Linux (Consola, WPF con Mono)
  * Android (MAUI, Nativo)
  * iOS (MAUI)

* **[Aplicaciones de Demostración]** 6 demostraciones actualizadas con controles de interfaz de usuario de modo de baja latencia:
  * Media Blocks SDK: RTSP Preview Demo (WPF), RTSP MultiView Demo (WinForms), MAUI RTSPViewer, Android RTSP Client
  * Video Capture SDK X: IP Capture (WPF), RTSP MultiView Demo (WinForms)
  * Todas las demostraciones incluyen casillas de verificación fáciles de usar o baja latencia habilitada por defecto para una experiencia de usuario óptima

* **[Documentación]** Guías y recursos completos:
  * `RTSP_LOW_LATENCY.md` - Guía completa de uso de Media Blocks SDK con ejemplos de código
  * `PIPELINE_LOW_LATENCY.md` - Análisis en profundidad de componentes de tubería y técnicas de optimización de latencia
  * `GSTREAMER_RTSP_EXAMPLES.md` - 7 ejemplos de tubería GStreamer de línea de comandos para pruebas
  * Documentación oficial de AYUDA actualizada con sección de baja latencia y mejores prácticas
  * Scripts de prueba de GStreamer: Bash (Linux/macOS), Batch (Windows), PowerShell (Windows, recomendado)

* **[Pruebas]** Cobertura de pruebas y validación completas:
  * 12 nuevas pruebas unitarias para la configuración de baja latencia de RTSPSourceSettings
  * Validado en cámaras IP reales en todas las plataformas
  * Pruebas de rendimiento: Windows (85ms), macOS (95ms), Linux (80ms), Android (110ms), iOS (100ms)

* **[Compatibilidad con Versiones Anteriores]** Implementación 100% compatible con versiones anteriores:
  * Comportamiento predeterminado sin cambios: el código existente funciona sin modificaciones
  * El modo de baja latencia es opcional a través de una propiedad explícita
  * Sin impacto en el rendimiento cuando no se usa el modo de baja latencia
  * Optimización de cola solo aplicada cuando `LowLatencyMode=true`

## 2025.10.3

* [Media Blocks SDK .Net] Se agregó soporte de sumidero DASH (Dynamic Adaptive Streaming over HTTP) con las clases DASHSinkBlock y DASHOutput
* [Media Blocks SDK .Net] Se agregó UniversalSourceBlockV2 con uso de memoria y rendimiento mejorados

## 2025.9.5

* [Video Fingerprinting SDK] Soporte mejorado para videos volteados

## 2025.9.3

* [Media Blocks SDK .Net] Se agregó soporte de código de barras DataMatrix usando el bloque DataMatrixDecoderBlock

## 2025.9.1

* [Video Fingerprinting SDK] Soporte mejorado para videos volteados

## 2025.8.9

* [Video Capture SDK .Net] VideoCaptureCoreX: Se resolvió el problema con la llamada Snapshot_GetSK en Android (espacio de color incorrecto)

## 2025.8.6

* [X-engines] Bloque de fuente RTSP RAW actualizado. Se agregaron las propiedades WaitForKeyframe y SyncAudioWithKeyframe. El bloque puede esperar fotogramas clave porque algunas cámaras pueden no enviarlos como primeros fotogramas.

## 2025.8.4

* [X-engines] Se agregó soporte de fuente NDI en Live Video Compositor

## 2025.8.2

* [ALL] Nuevo código de administrador ONVIF en VisioForge.Core.ONVIFX. Implementación completa de varios servicios ONVIF, incluidos Gestión de Dispositivos, Medios v1/v2, PTZ, Eventos, Imágenes, Análisis, Grabación y servicios de Reproducción.

## 2025.8.1

* [Media Player SDK] Se agregó la propiedad PauseOnStop a MediaPlayerCoreX

## 2025.6.30

* [X-engines] Se agregó soporte de GIF animado a las clases `ImageVideoSourceBlock`/`ImageVideoSourceSettings`
* [X-engines] Se resolvieron problemas con el inicio retrasado de archivos en Live Video Compositor
* [X-engines] Actualización de la API del mezclador de video para usar GUIDs en lugar de índices enteros para fuentes de video

## 2025.6.27

* [Video Capture SDK] Se resolvió el problema con el motor RTSP de Baja Latencia con algunas cámaras

## 2025.6.5

* [X-engines] Se resolvió el problema con la reproducción de fuentes NDI sin flujos de audio

## 2025.6.3

* [X-engines] Soporte de fuente GenICam actualizado para cámaras USB Vision. Se agregó soporte de fuente GenTL.

## 2025.6.2

* [X-engines] Se agregó soporte de desentrelazado para fuentes de video Decklink entrelazadas

## 2025.6.1

* [Live Video Compositor] Se resolvió el problema con las fuentes de archivo pausadas al inicio y reanudadas con error

## 2025.5.1

* [ALL] Actualizar paquetes de dependencia NuGet a las últimas versiones
* [X-engines] Se resolvió el problema con la transmisión de red RTMP a un servidor personalizado

## 2025.4.8

* [ALL] Se agregó la API de Movimiento Absoluto a la clase `ONVIFDeviceX`. Puede usar esta API para mover la cámara ONVIF a la posición absoluta especificada.

## 2025.2.24

* [X-engine] De forma predeterminada, la enumeración de dispositivos Media Foundation está deshabilitada. Puede habilitarla usando la propiedad `DeviceEnumerator.Shared.IsEnumerateMediaFoundationDevices`.

## 2025.2.18

* [Media Player SDK.Net] Se agregó soporte de bucle para el motor multiplataforma.
* [ALL] Salida del motor RTSP-X actualizada, se corrigió el problema de bloqueo con la salida RTSP y las reconexiones frecuentes del reproductor VLC
* [X-engines] Se cambió el soporte del detector de rostros para usar la interfaz IFaceDetector
* [Live Video Compositor] Se corrigieron problemas de registro con vista de video personalizada adjunta a la entrada de video
  
## 2025.2.9

* [X-engines] Velocidad de conexión NDI actualizada

## 2025.2.4

* [X-engines] RTSP Server Media Block y RTSPServerOutput agregados a Video Capture SDK. Puede usar el RTSPServerBlock para crear un servidor RTSP y transmitir video y audio a él.

## 2025.2.1

* [X-engines] Se agregó soporte para codificadores NVENC y AMF AV1

## 2025.1.25

* [Windows] Se resolvió el problema HTTPS con los certificados SSL no cargados

## 2025.1.22

* [Windows] Se resolvió el problema con fuentes ONVIF perdidas al enumerar en PC con múltiples interfaces de red
* [Media Blocks SDK .Net] Se agregó el evento `OnEOS` a la clase `MediaBlockPad`. Puede usar este evento para obtener el evento EOS (Fin de Flujo) del bloque multimedia. Puede ser útil si tiene varias fuentes de archivo con una duración diferente y necesita detener la tubería cuando finaliza la primera fuente.
* [Media Blocks SDK .Net] Se agregó el método `SendEOS` a la clase `MediaBlocksPipeline`. Puede usar este método para enviar el evento EOS (Fin de Flujo) a la tubería.
  
## 2025.1.18

* [NuGet] Los paquetes `VisioForge.Core.UI.Apple`, `VisioForge.Core.UI.Android` y `VisioForge.Core.UI.WinUI` se fusionan en el paquete `VisioForge.DotNet.Core`. Todos los espacios de nombres son los mismos.
* [Media Blocks SDK .Net] Se agregó la propiedad `ZOrder` a las clases `LVCVideoInput` y `LVCVideoAudioInput`. Puede usar esta propiedad para establecer el orden Z para la entrada de video.

## 2025.1.14

* [NuGet] Los paquetes `VisioForge.Core.UI.WPF` y `VisioForge.Core.UI.WinForms` se fusionan en el paquete `VisioForge.DotNet.Core`. En proyectos WPF debe actualizar el código XAML si se usan los nombres de ensamblado. Todos los espacios de nombres son los mismos.

## 2025.1.11

* [Video Capture SDK .Net] Se resolvió el problema del codificador QSV H264 FFMPEG con los símbolos incorrectos en los parámetros

## 2025.1.7

* [Cross-platform] Se agregó soporte de fuente `libcamera` para Linux/Raspberry Pi.

## 2025.1.5

* [Cross-platform] Reproducción de fotograma anterior mejorada en Media Player SDK .Net (Motor multiplataforma)

## 2025.1.4

* [Cross-platform] Se resolvió el problema con la inicialización del complemento AMD AMF

## 2025.1.1

* [Cross-platform] Se resolvió la fuga de memoria en `OverlayManagerImage`

## 2025.1.0

* [Cross-platform] Motor Live Video Compositor actualizado. Soporte Decklink mejorado para entrada y salida. Rendimiento mejorado. Las nuevas clases de motor se encuentran en el espacio de nombres `VisioForge.Core.LiveVideoCompositorV2`.

## 2025.0.29

* [Cross-platform] El renderizador de video predeterminado en Windows se ha cambiado a DirectX 11

## 2025.0.17

* [Media Blocks SDK .Net] Se agregó soporte de fuente libCamera (se puede usar en Raspberry Pi)

## 2025.0.16

* [Media Blocks SDK .Net] Se resolvió el problema al agregar varios AudioRendererBlocks a la tubería

## 2025.0.14

* [Media Blocks SDK .Net] Se agregó la clase "PushJPEGSourceSettings" para configurar la fuente JPEG para el "PushSourceBlock". Puede usar esta clase para establecer la configuración de la fuente JPEG para el "PushSourceBlock". También se agregó la muestra "video-from-images".

## 2025.0.7

* [ALL] Se resolvieron problemas de captura de ventana en SDKs multiplataforma
* [Media Blocks SDK .Net] Se agregó la muestra Bridge Source Switch

## 2025.0.5

* [iOS] Se resolvieron problemas con la velocidad de reproducción para algunos archivos de video
* [iOS] Se agregó soporte de Simulador iOS para todos los SDKs. La fuente de cámara no es compatible en el simulador.

## 2025.0.3

* [MacOS] Se resolvió el problema de paso incorrecto para videos de cámara vertical en MacOS
* [Video Capture SDK .Net] Se resolvió el problema de color de fondo para la superposición de texto desplazable

## 2025.0

* [ALL] Soporte de .Net 9
* [Media Blocks SDK .Net] Se agregó `AVIOutputBlock` para guardar flujos de video y audio en formato de archivo AVI
* [Media Blocks SDK .Net] El constructor `TeeBlock` ahora acepta el tipo de medio como parámetro
* [Video Capture SDK .Net] Se agregaron los métodos `Video_CaptureDevice_SetDefault` y `Audio_CaptureDevice_SetDefault` a la clase `VideoCaptureCore`. Puede usar este método para establecer los dispositivos de captura de video y audio predeterminados
* [Cross-platform] Rendimiento de renderizado de video `Metal` mejorado en dispositivos Apple
* [All] Rendimiento mejorado de operaciones comunes de procesamiento de video en SDKs clásicos de Windows
* [CV] Se agregaron detectores de rostros DNN para `Media Blocks SDK .Net` y `Video Capture SDK .Net`
* [Mobile] Compatibilidad AOT mejorada para iOS y Android
* [WinUI] Rendimiento mejorado del renderizado de video `WinUI`
* [Media Blocks SDK .Net] Se agregaron los métodos `GetLastFrameAsSKBitmap` y `GetLastFrameAsBitmap` a `VideoSampleGrabberBlock` para obtener el último fotograma como `SkiaSharp.SKBitmap` o `System.Drawing.Bitmap`
* [Video Capture SDK .Net] `VideoCaptureCore`: Se agregó la propiedad `AddFakeAudioSource` a `FFMPEGEXEOutput`. La propiedad `Network_Streaming_Audio_Enabled` de `VideoCaptureCore` debe establecerse en falso para usar este audio falso.
* [ALL] Rendimiento mejorado de VideoView WinUI (y MAUI en Windows)
* [Video Capture SDK .Net] `VideoCaptureCore`: Se agregó la API `PIP_Video_CaptureDevice_CameraControl_` para controlar la configuración de la cámara para el modo Imagen en Imagen
* [X-engines] Se agregó soporte de encabezados para las fuentes HTTP creadas usando la clase `HTTPSourceSettings`
* [X-engines] Muestras de Avalonia actualizadas, con proyectos para macOS, Linux y Windows
* [X-engines] Se agregaron paquetes redist NuGet para macOS y MacCatalyst (incluyendo MAUI)
* [Video Capture SDK .Net] `VideoCaptureCore`: Se agregó soporte de ruta de dispositivo para la API `PIP_Video_CaptureDevice_CameraControl`
* [Video Capture SDK .Net] `VideoCaptureCore`: Se agregó la propiedad `FFMPEG_MaxLoadTimeout` para fuentes de cámara IP. Le permite establecer el tiempo máximo de espera para que la fuente FFMPEG cargue el flujo
* [X-engines] Soporte de Linux actualizado para dispositivos de audio `ALSA`, `PulseAudio` y `PipeWire`
* [X-engines] Soporte de Linux actualizado para dispositivos `V4L2`
* [X-engines] Las muestras de Avalonia se han cambiado a una estructura moderna de 1 proyecto
* [X-engines] Se resolvió el problema con bloqueos de `MAUI` en Windows después de la actualización de `SkiaSharp`
* [X-engines] Se resolvió el problema con bloqueos de `TextureView` en Android en aplicaciones `MAUI`
* [X-engines] Se resolvió el problema de reproducción para fuentes http usando el `UniversalSourceBlock`
* [X-engines] Se agregó muestra de Mobile Streamer para Android
* [X-engines] Se agregó soporte de `OverlayManagerBlock` para Android (ahora está disponible para todas las plataformas)
* [Video Capture SDK .Net] `VideoCaptureCoreX`: Se agregaron las propiedades `CustomVideoProcessor`/`CustomAudioProcessor` para todos los formatos de salida. Puede usar estas propiedades para establecer bloques de procesamiento de video/audio personalizados para el formato de salida.
* [Media Blocks SDK .Net] Se agregó el `KeyFrameDetectorBlock` para detectar fotogramas clave en flujos de video (H264, H265, VP8, VP9, AV1, etc.)
* [Media Blocks SDK .Net] Se corrigió el problema de licencia para la clase `LiveVideoCompositor`
  
## 15.10.0

* [Windows] API de captura de ventana actualizada para capturar solo la ventana principal especificada de forma predeterminada. Se agregó el método `UpdateHotkey` a la clase `WindowCaptureForm` para actualizar la tecla de acceso rápido para el formulario de captura de ventana.
* [X-engines] Mejor compatibilidad AOT para la configuración predeterminada de MAUI en iOS.
* [Media Blocks SDK .Net] Se agregó el `DNNFaceDetectorBlock` para detectar rostros y desenfocarlos/pixelarlos usando OpenCV y modelos DNN.
* [Media Blocks SDK .Net] Se agregó el `MKVOutputBlock` para guardar flujos de video y audio en formato de archivo MKV.
* [X-engines] Mejor soporte para el cambio dinámico del tamaño de la fuente de video en aplicaciones MAUI.
* [X-engines] Se resolvió un problema con dos o más medidores VU en la misma tubería.
* [X-engines] Se resolvió el problema de error de volumen/silencio con el mezclador de audio en el motor Live Video Compositor.
* [X-engines] La fuente `Spinnaker` para cámaras `FLIR`/`Teledyne` se incluye en el paquete principal y ya no requiere un complemento adicional.
* [Video Capture SDK .Net] Se resolvió el problema con la API `SeparateCapture` si no se usaba `VideoView`.
* [X-engines] El constructor `MediaBlocksPipeline` ya no tiene el parámetro `live`. Para tuberías más personalizables, los renderizadores de video y audio obtuvieron la propiedad `IsSync` (`true` por defecto).
* [X-engines] Se resolvió el bloqueo de `VideoViewTX` en aplicaciones MAUI Android.
* [X-engines] Se agregó la interfaz `IVideoEncoder` a la clase `MPEG2VideoEncoder`. Permite el uso de `MPEG2VideoEncoder` con `MPEGTSOutput`, `AVIOutput` y otras clases de salida.
* [X-engines] Se resolvió el problema con la captura de ventana usando la clase `ScreenCaptureD3D11SourceSettings`. Si el rectángulo era incorrecto o no se especificaba, causaba un error.
* [X-engines] Se agregó el renderizador `Metal` al SDK para dispositivos Apple y se usa de forma predeterminada para iOS y MAUI.
* [Media Blocks SDK .Net] Se agregó la muestra de Captura de Pantalla MAUI.
* [Video Capture SDK .Net] VideoCaptureCore: Se agregó la propiedad `VLC_CustomDefaultFrameRate` a `IPCameraSourceSettings` para establecer una velocidad de fotogramas personalizada para la fuente de cámara IP VLC si la fuente no proporciona la velocidad de fotogramas correcta.
* [Media Blocks SDK .Net] `RTSPSourceBlock`: Si la fuente RTSP tiene audio pero ha deshabilitado el flujo de audio en `RTSPSourceSettings`, el SDK agregará un renderizador nulo automáticamente para evitar advertencias.
* [ALL] Se resolvió el problema con la llamada `VideoFrameX.ToBitmap()` (espacio de color incorrecto)
* [Windows] Soporte KLV actualizado en salida MPEG-TS
* [Windows] Se resolvió el problema de serialización de MediaPlayerCore
* [ALL] La clase de configuración del renderizador de video ya no contiene color de fondo. Use la propiedad de color de fondo de VideoView en su lugar.
* [X-engines] Bibliotecas GStreamer actualizadas
* [X-engines] Se resolvieron problemas de renderizado de video en Android e iOS
* [X-engines] Se corrigió el bloqueo de iOS durante el uso de VideoViewGL
* [X-engines] Se agregó codificador AAC predeterminado para iOS
* [X-engines] Actualización de fuente de cámara iOS para soporte de alta velocidad de fotogramas
* [Windows] Fuente VLC actualizada - velocidad de carga de archivos mejorada
* [Media Blocks SDK .Net]: Se agregó `UniversalDemuxBlock` que permite demultiplexar flujos de video y audio de un archivo en formatos MP4, MKV, AVI, MOV, TS, VOB, FLV, OGG y WebM
* [Windows] Se resolvieron problemas de estabilidad de FFMPEG
* [X-engines] Se resolvió el problema con la fuente de audio de bucle invertido usando VideoCaptureCoreX y captura de audio a archivo
* [X-engines] Se agregó soporte de fuente y sumidero SRT en Media Blocks SDK .Net y Video Capture SDK .Net
* [Video Capture SDK .Net] VideoCaptureCore: El método `IP_Camera_ONVIF_ListSourcesAsyncEx` obtuvo una versión de sobrecarga con una devolución de llamada para una interfaz de usuario más responsable
* [X-engines] Actualización de compatibilidad de fuente RTSP
* [X-engines] `Cambio de API importante`. A partir de esta actualización, el SDK utiliza implementaciones de interfaz `IAudioRendererSettings` para la configuración de salida de audio. La salida WASAPI obtuvo las clases de configuración personalizadas. Las propiedades Output_AudioDevice de tipo `VideoCaptureCoreX`/`MediaPlayerCoreX` se han cambiado a `IAudioRendererSettings`. Puede crear la instancia de clase `AudioRendererSettings` desde `AudioOutputDeviceInfo` usando el constructor predeterminado.
* [X-engines] Se resolvió el problema con fuentes Media Foundation perdidas durante la enumeración de dispositivos
* [X-engines] Se resolvieron problemas de fuente RTSP con conexión de audio en algunas situaciones
* [X-engines] Se agregó la demostración RTSP Preview a Media Blocks SDK .Net
* [Windows] Salidas y fuente FFMPEG actualizadas a FFMPEG v7.0.
* [X-engines] Se corrigieron bloqueos raros en la fuente RTSP cuando la información de la cámara no está disponible por alguna razón (problema de red)
* [X-engines] Se resolvió un problema con el uso del renderizador de audio `WASAPI/WASAPI2`
* [X-engines] Se resolvió un problema con la fuente de audio de bucle invertido en Windows
* [X-engines] Rendimiento y estabilidad de renderizado de video iOS mejorados
* [X-engines] Se agregó salida AWS S3 Sink para Media Blocks SDK .Net
* [X-engines] Se agregó soporte para cámaras Allied Vision USB3/GigE en Media Blocks SDK .Net y Video Capture SDK .Net

## 15.9

* [X-engines] Se resolvió la relación de aspecto incorrecta con el efecto/bloque de cambio de tamaño de video
* [X-engines] Redist GStreamer actualizado
* [X-engines] Se agregó soporte para cámaras Basler USB3/GigE en Media Blocks SDK .Net y Video Capture SDK .Net
* [Video Edit SDK .Net] VideoEditCoreX: La clase TextOverlay cambió para usar configuraciones de fuente basadas en SkiaSharp. Además, puede establecer el nombre de archivo de fuente personalizado o configurar todos los parámetros de renderizado usando SKPaint personalizado.
* [Windows] Se agregó soporte de Stream en `MediaInfoReader`. Puede obtener la información del archivo de video/audio desde un flujo (DB, red, memoria, etc.).
* [X-engines] Motor Live Video Compositor actualizado, que mejoró el soporte de las fuentes de archivo
* [Video Capture SDK .Net] Se agregó detector cubierto por cámara en `Computer Vision Demo` y el paquete `VisioForge.Core.CV`
* [X-engines] Se agregó API para obtener instantáneas de archivos de video usando MediaInfoReaderX: GetFileSnapshotBitmap, GetFileSnapshotSKBitmap, GetFileSnapshotRGB
* [X-engines] Soporte iOS en muestras MAUI
* [X-engines] Se resolvió el problema de fuga de memoria para fuentes RTSP
* [Media Player SDK .Net] MediaPlayerCore: Se agregó soporte para flujos de datos en archivos de video usando el motor de fuente FFMPEG. Agregue el evento OnDataFrameBuffer para obtener fotogramas de datos (KLV u otros) del archivo de video.
* [Video Capture SDK .Net] VideoCaptureCore: Se agregó soporte para flujos de datos en archivos de video usando el motor de fuente IP Capture FFMPEG. Agregue el evento OnDataFrameBuffer para obtener fotogramas de datos (KLV u otros) del flujo de red MPEG-TS UDP u otra fuente compatible.
* [Video Capture SDK .Net] VideoCaptureCore: Se agregó la propiedad FFMPEG_CustomOptions a la clase IPCameraSourceSettings. Esta propiedad le permite establecer opciones FFMPEG personalizadas para la fuente de cámara IP
* [Windows] Se corrigió el problema de bloqueo con la fuente FFMPEG cuando se pierde una conexión de red
* [Media Blocks SDK .Net] Se agregó RTSP MultiView in Sync Demo
* [X-engines] Se agregó soporte para cámaras FLIR/Teledyne (USB3Vision/GigE) usando el SDK Spinnaker
* [Video Edit SDK .Net] VideoEditCoreX: Se agregó soporte para el uso de .Net Stream como fuente de entrada
* La interfaz IAsyncDisposable se agregó a todas las clases principales del SDK. La llamada `DisposeAsync` debe usarse para eliminar los objetos principales usando métodos asíncronos.  
* [Video Capture SDK .Net] VideoCaptureCoreX: Se resolvieron problemas con la captura de video de Android (a veces comenzaba solo una vez)
* [Media Blocks SDK .Net] Se agregó muestra de transmisión HLS
* [Video Capture SDK .Net] VideoCaptureCore: Se resolvió el bloqueo si `multiscreen` está habilitado y las pantallas se agregan como identificadores de ventana (WinForms)
* [X-engines] Velocidad de renderizado de video MAUI mejorada
* [X-engines] Se resolvieron problemas de reproducción de medios MAUI (decodificación) en MAUI Android
* [X-engines] Se resolvió un problema con las fuentes de cámara web H264 (a veces no conectadas)
* [X-engines] Se resolvió un problema con la reproducción de flujo de audio en el motor Live VideoCompositor
* [Media Blocks SDK .Net] Se resolvió un problema de audio incorrecto al mezclar usando el motor Live Video Compositor
* [Media Blocks SDK .Net] Se agregó salida Decklink y fuente de archivo en la muestra Live Video Compositor
* [Media Player SDK .Net] MediaPlayerCore: Se agregó soporte de archivo MPEG-TS creciente para el motor VLC. Puede reproducir archivos MPEG-TS crecientes mientras se graban
  
## 15.8

* [X-engines] [Cambio de API importante] DeviceEnumerator ahora solo se puede usar usando la propiedad `DeviceEnumerator.Shared`. Se requiere un enumerador por aplicación. Los objetos DeviceEnumerator utilizados por la API se han eliminado
* [X-engines] [Cambio de API importante] Android Activity ya no es necesaria para crear motores SDK
* [X-engines] [Cambio de API importante] Los motores X requieren pasos adicionales de inicialización y desinicialización. Para inicializar el SDK, use la llamada `VisioForge.Core.VisioForgeX.InitSDK()`. Para desinicializar el SDK, use la llamada `VisioForge.Core.VisioForgeX.DestroySDK()`. Debe inicializar el SDK antes de cualquier uso de clase SDK y desinicializar el SDK antes de que salga la aplicación.
* [Windows] Rendimiento de renderizado de video MAUI mejorado en Windows
* [Windows] Se agregó un resaltado de mouse para fuentes de captura de pantalla
* [Windows] Se resolvió un problema de llamada CallbackOnCollectedDelegate con la clase BasicWindow
* [Avalonia] Se resolvió un problema con el cambio de tamaño de Avalonia VideoView
* [X-engines] Se agregaron las propiedades StartPosition y StopPosition a UniversalSourceSettings. Puede usar estas propiedades para establecer las posiciones de inicio y parada para la fuente de archivo.
* [ALL] Se resolvió el problema con contraseñas con caracteres especiales utilizados para fuentes RTSP
* [ALL] Se resolvió el raro problema de volteo de video con el motor Virtual Camera SDK
* [ALL] El filtro VisioForge MJPEG Decoder se eliminó de los paquetes NuGet del SDK. Opcionalmente, puede agregarlo a su proyecto mediante copia de archivos o implementación de registro COM.
* [X-engines] Se corrigió la fuga de memoria en OverlayManager
* [Media Blocks SDK .Net] Se resolvió el problema con VideoSampleGrabberBlock, opción SetLastFrame
* [Video Capture SDK .Net] VideoCaptureCoreX: Las fuentes de audio WASAPI y WASAPI2 se pueden usar ahora con el motor VideoCaptureCoreX
* [X-engines] DeviceEnumerator obtuvo eventos para notificar sobre dispositivos agregados/eliminados: OnVideoSourceAdded, OnVideoSourceRemoved, OnAudioSourceAdded, OnAudioSourceRemoved, OnAudioSinkAdded, OnAudioSinkRemoved
* [X-engines] Se agregó soporte de controlador de errores personalizado para motores MediaBlocks, VideoCaptureCoreX y MediaPlayerCoreX. Use la interfaz IMediaBlocksPipelineCustomErrorHandler y el método SetCustomErrorHandler para establecer un controlador de errores personalizado.
* [Video Capture SDK .Net] VideoCaptureCoreX: Se resolvió el problema con el error de índice de dispositivo incorrecto para fuentes de video KS (Windows)
* [Video Capture SDK .Net] VideoCaptureCore: Se agregó la propiedad Virtual_Camera_Output_AlternativeAudioFilterName para establecer un filtro de audio personalizado para la salida Virtual Camera SDK
* [Video Edit SDK .Net] VideoEditCore: Se agregó la propiedad Virtual_Camera_Output_AlternativeAudioFilterName para establecer un filtro de audio personalizado para la salida Virtual Camera SDK
* [Media Player SDK .Net] MediaPlayerCore: Se agregó la propiedad Virtual_Camera_Output_AlternativeAudioFilterName para establecer un filtro de audio personalizado para la salida Virtual Camera SDK
* [Video Capture SDK .Net] VideoCaptureCoreX: Se agregó soporte de transmisión NDI y aplicación de muestra.
* [Media Blocks SDK .Net] Se agregó el bloque BufferSink para obtener fotogramas de video/audio de la tubería
* [Media Blocks SDK .Net] Se agregó la clase CustomMediaBlock para crear bloques multimedia personalizados para cualquier elemento GStreamer
* [Media Blocks SDK .Net] Se agregó el método UpdateChannel para actualizar el canal de la fuente o sumidero del puente
* [Media Player SDK .Net] MediaPlayerCore: Efecto Tempo actualizado.
* [X-engines] Enumerador de dispositivos actualizado. Se eliminó el cuadro de diálogo de firewall no deseado al enumerar fuentes NDI.
* [X-engines] Se corrigió un problema con el mezclador de video al agregar/eliminar fuentes de video.
* [Media Blocks SDK .Net] Se agregaron bloques VideoCropBlock y VideoAspectRatioCropBlock para recortar fotogramas de video.
* [Media Blocks SDK .Net] Se resolvió el problema de velocidad de fotogramas incorrecta con VideoRateBlock.
* [All] Se resolvió un problema con el efecto de audio Tempo.
* [Video Capture SDK .Net] VideoCaptureCore: Se agregó soporte de renderizador de audio WASAPI para el motor VideoCaptureCore.

## 15.7

* [ALL] Soporte de .Net 8
* [Video Capture SDK .Net] VideoCaptureCore: Se corrigió el problema con el evento OnNetworkSourceDisconnect que se llamaba dos veces.
* [X-engines] Se agregó el codificador de video MPEG-2.
* [X-engines] Se agregó el codificador de audio MP2.
* [X-engines] Se resolvieron problemas de enumeración Decklink.
* [X-engines] La configuración predeterminada de VP8/VP9 cambió a grabación en vivo.
* [X-engines] Se agregó soporte de codificador de video DNxHD.
* [Video Capture SDK .Net] VideoCaptureCoreX: Se corrigió el problema con la configuración del formato de fuente de audio (regresión).
* [Video Capture SDK .Net] VideoCaptureCoreX: Se resolvió el problema de renderizado nativo de WPF con una ventana emergente.
* [All] Soporte de Avalonia 11.0.5.
* [Video Capture SDK .Net] VideoCaptureCoreX: Se resolvieron problemas de licencia.
* [Video Capture SDK .Net] VideoCaptureCore: El método Start/StartAsync devolverá falso si el dispositivo de captura de video ya está siendo utilizado por otra aplicación.
* [All] Fuente VLC actualizada (libVLC 3.0.19).
* [All] Fuentes y codificadores FFMPEG actualizados. Se resolvió el problema con dependencias MSVC perdidas.
* [Video Capture SDK] Motor ONVIF actualizado.
* [Cross-platform SDKs] Fuente Decklink actualizada. Se resolvió el problema con el nombre de dispositivo incorrecto.
* [All] Actualizaciones de seguridad de SkiaSharp.
* [Cross-platform SDKs] Overlay Manager actualizado. Se agregó la clase OverlayManagerDateTime para dibujar la fecha y hora actual y texto personalizado.
* [Cross-platform SDKs] OverlayManagerImage actualizado. Se resolvió el problema con el uso de System.Drawing.Bitmap.
* [ALL] VideoCaptureCore: Se resolvió el problema de bloqueo raro con WinUI VideoView
* [Video Capture SDK .Net] VideoCaptureCore: Salida FFMPEG.exe actualizada. Soporte mejorado de codificadores x264 y x265 de compilaciones FFMPEG personalizadas.

## 15.6

* [Video Capture SDK .Net] VideoCaptureCore: Rendimiento de recorte de video mejorado en CPU modernas
* [ALL] VideoCaptureCore, MediaPlayerCore, VideoEditCore: Se agregó el método estático CreateAsync que se puede usar en lugar del constructor para crear motores sin retraso de interfaz de usuario.
* [Video Capture SDK .Net] VideoCaptureCore: Se resolvieron problemas con el recorte de video.
* [Video Capture SDK .Net] VideoCaptureCoreX: Se agregó API de superposiciones de video. La demostración de Overlay Manager muestra cómo usarla.
* [Video Capture SDK .Net] Detección de codificador HW mejorada. Si tiene varias GPU, a veces solo se puede usar la GPU principal para la codificación de video.
* [Cross-platform SDKs] Avalonia VideoView actualizado. Se resolvió el problema con la recreación de VideoView.
* [Media Player SDK .Net] MediaPlayerCoreX: Se resolvió el problema de inicio con la versión de Android del motor MediaPlayerCoreX.
* [Media Player SDK .Net] MediaPlayerCore: La propiedad Video_Stream_Index ha sido reemplazada por los métodos Video_Stream_Select/Video_Stream_SelectAsync.
* [Media Player SDK .Net] MediaPlayerCoreX: Se agregó el método Video_Stream_Select.
* [Video Capture SDK .Net] VideoCaptureCore: La propiedad Network_Streaming_WMV_Maximum_Clients se movió a la clase WMVOutput. Puede establecer el número máximo de clientes para la salida WMV de red.
* [All] Renderizado WPF actualizado. Rendimiento mejorado para videos 4K y 8K.
* [Video Capture SDK .Net] VideoCaptureCoreX: Se resolvió el problema con múltiples salidas utilizadas.
* [Video Capture SDK .Net] VideoCaptureCoreX: Se resolvió el problema con el evento OnAudioFrameBuffer.
* [Video Capture SDK .Net] La fuente Decklink cambió para mejorar la velocidad de inicio. El método Decklink_CaptureDevices ha sido reemplazado por Decklink_CaptureDevicesAsync asíncrono.
* [Media Player SDK .Net] MediaPlayerCoreX: Se agregaron las propiedades Custom_Video_Outputs/Custom_Audio_Outputs para establecer renderizadores de video/audio personalizados
* [Media Player SDK .Net] MediaPlayerCoreX: Se agregó Decklink Output Player Demo (WPF)
* [Video Edit SDK .Net] Se agregó Multiple Audio Tracks Demo (WPF)
* [Video Edit SDK .Net] Salida MP4 actualizada para múltiples pistas de audio
* [Cross-platform SDKs] Enumerador de dispositivos actualizado
* [Video Capture SDK .Net] Se resolvió el problema con el medidor VU en el motor multiplataforma
* [Cross-platform SDKs] Se resolvió el problema con el medidor VU (evento no disparado)
* [Media Player SDK .Net] Reproducción de memoria actualizada
* [ALL] Se agregó soporte de interfaz IAsyncDisposable para clases principales multiplataforma. Debe usarse para eliminar los objetos principales en métodos asíncronos.
* [Video Capture SDK .Net] Se agregó soporte madVR para multiscreen
* [Video Capture SDK .Net] Se resolvió el problema de enumeración NDI en el motor VideoCaptureCore
* [Media Player SDK .Net] Se agregó madVR Demo
* [Video Capture SDK .Net] Se agregó madVR Demo
* [ALL] Se resolvieron problemas de madVR en todos los SDKs
* [Media Blocks SDK .Net] Se agregó demostración de fuente NDI
* [Video Capture SDK .Net] Se agregó soporte NDI para motor multiplataforma
* [ALL] Resolver el problema de "imagen no encontrada" con el paquete NuGet WinUI
* [Media Blocks SDK .Net/Media Player SDK .Net (cross-platform)] Se agregó demostración de reproductor de Karaoke MP3+CDG
* [Media Blocks SDK .Net] Se agregó CDGSourceBlock para reproducción de archivos de karaoke MP3+CDG
* [ALL] Soporte madVR mejorado
* WinUI VideoView actualizado para solucionar problemas durante la reproducción de archivos de audio
* [Video Capture SDK .Net] Soporte de fuente VNC mejorado para el motor VideoCaptureCoreX.
* [Video Capture SDK .Net] Se agregó soporte de fuente VNC para el motor VideoCaptureCoreX. Puede usar la clase VNCSourceSettings para configurar Video_Source.
* [Media Blocks SDK .Net] Se agregó soporte de fuente VNC. Puede usar la clase VNCSourceBlock como bloque de fuente de video.
* [Video Capture SDK .Net] La propiedad Video_Resize se ha cambiado al tipo IVideoResizeSettings. Puede usar la clase VideoResizeSettings para realizar un cambio de tamaño clásico igual que antes o usar MaxineUpscaleSettings/MaxineSuperResSettings para realizar un cambio de tamaño de IA en GPU Nvidia usando Nvidia Maxine SDK (se requiere SDK o modelos SDK para implementar).
* [ALL] Se resolvieron problemas con la detección de fuentes NDI en la red local
* [ALL] Se agregó la clase KLVParser para leer y decodificar datos de archivos binarios KLV.
* [ALL] Se agregó el bloque KLVFileSink. Puede exportar datos KLV desde archivos MPEG-TS.
* [Media Blocks SDK .Net] Se agregó demostración KLV.
* [Video Capture SDK .Net] Se agregó transmisor de red MJPEG.
* [ALL] Se agregó soporte WASAPI 2.
* [Media Blocks SDK .Net] API de efectos de video actualizada. Se agregó bloque de escala de grises.
* [Media Blocks SDK .Net] Se agregó API y muestra de Live Video Compositor.
* [ALL] Control Avalonia VideoView actualizado. Se resolvieron problemas con la reproducción de video en Windows en pantallas HighDPI.
* [Video Capture SDK .Net] Se agregó la propiedad CustomVideoFrameRate a MFOutput. Puede establecer una velocidad de fotogramas personalizada si su fuente proporciona una velocidad de fotogramas incorrecta (cámara IP, por ejemplo).
* [Video Capture SDK .Net] Codificador NVENC actualizado. Se resolvió el problema con la captura de video de alta definición.
* [Video Capture SDK .Net] Se resolvió el problema con la sintonización de TV en dispositivos Avermedia
* [Media Blocks SDK .Net] Se agregaron bloques OpenCV: CVDewarp, CVDilate, CVEdgeDetect, CVEqualizeHistogram, CVErode, CVFaceBlur, CVFaceDetect, CVHandDetect, CVLaplace, CVMotionCells, CVSmooth, CVSobel, CVTemplateMatch, CVTextOverlay, CVTracker
* [CV] Se resolvió el problema con las coordenadas faciales incorrectas.
* [CV, Media Blocks SDK .Net] Se agregó bloque de detector de rostros.
* [Media Blocks SDK .Net] Se agregó codificador de video rav1e AV1.
* [Media Blocks SDK .Net] Se agregó codificador de video GIF.
* [Media Blocks SDK .Net] Se agregaron bloques de sumidero NDI y fuente NDI.
* [ALL] Se resolvieron problemas de detección de SDK NDI.
* [Media Blocks SDK .Net] Codificador Speex actualizado.
* [Media Blocks SDK .Net] Bloque mezclador de video actualizado.
* [ALL] Se agregaron métodos Save/Load para formato de salida para serializar en JSON.
* [Media Blocks SDK .Net] Se agregó bloque de sumidero de transmisión en vivo HTTP MJPEG.
* [ALL] Se resolvió la regresión MP4 HW QSV H264.
* [ALL] Actualizaciones de estabilidad de WinForms y WPF VideoView.
* [Media Player SDK .Net] Se eliminó la propiedad heredada FilenamesOrURL. Utilice la API `Playlist` en su lugar.
* [Media Blocks SDK .Net] Se agregó función de fundido de entrada/salida para bloque de superposición de imagen.
* [ALL] Actualización de telemetría
* [ALL] SDKs actualizados para usar `ObservableCollection` en lugar de `List` en la API pública.
* [ALL] Salida MP4 HW actualizada. Rendimiento NVENC mejorado.
* [Media Blocks SDK .Net] Se agregó muestra de Video Compositor.
* [Media Blocks SDK .Net] Se agregaron bloques YouTubeSink y FacebookLiveSink con configuraciones personalizadas de YouTube/Facebook. El `RTMPSink` puede transmitir a YouTube/Facebook de la misma manera que antes.
* [Media Blocks SDK .Net] Se agregó bloque mezclador de video SqueezeBack.
* [ALL] Logotipo de texto desplazable actualizado. Hemos agregado el método Preload para renderizar una superposición de texto antes de la reproducción.
* [ALL] Logotipo de texto desplazable actualizado (rendimiento)
* [Media Blocks SDK .Net] Bloques de sumidero Decklink actualizados
* [ALL] Se resolvieron bloqueos con un logotipo de texto con una resolución personalizada
* [Media Blocks SDK .Net] Se agregó soporte para codificadores Intel QuickSync H264, HEVC, VP9 y MJPEG.
* [Video Edit SDK .Net] Se agregó el método FastEdit_ExtractAudioStreamAsync para extraer el flujo de audio del archivo de video.
* [Video Edit SDK .Net] Se agregó muestra WinForms "Audio Extractor".
* [Media Blocks SDK .Net] MP4SinkBlock actualizado. El sumidero puede dividir archivos de salida por duración, tamaño de archivo o código de tiempo. Use MP4SplitSinkSettings en lugar de MP4SinkSettings para configurar.
* [Video Capture SDK .Net] Se agregó el evento OnMJPEGLowLatencyRAWFrame que se dispara cuando el motor de baja latencia MJPEG recibe un fotograma RAW de una cámara.
* [Media Blocks SDK .Net] Se agregó VideoEffectsBlock para usar efectos de video, disponibles en SDKs de Windows
* [Media Blocks SDK .Net] Fuente Decklink actualizada
* [Media Blocks SDK .Net] Se agregó Decklink Demo (WPF)
* [ALL] Se resolvió el bloqueo del efecto de video DeinterlaceBlend
* [ALL] Bibliotecas de terceros utilizadas movidas al ensamblado/NuGet VisioForge.Libs.External
* [ALL] Se agregó Nvidia Maxine Video Effects SDK (BETA) y aplicación de muestra para Media Player SDK .Net y Video Capture SDK .Net
* [Video Capture SDK .Net] Se agregó API Decklink_Input_GetVideoFramesCount/Decklink_Input_GetVideoFramesCountAsync para obtener fotogramas totales y perdidos para la fuente Decklink
* [ALL] Actualización de codificadores HW VisioForge

## 15.5

* Soporte de .Net 7
* Se agregó soporte de evento NetworkDisconnect al motor de cámara IP de baja latencia MJPEG
* Se agregó soporte de Linux para las demostraciones basadas en VideoEditCoreX
* Se agregó el evento OnRTSPLowLatencyRAWFrame para obtener fotogramas RAW del flujo RTSP, usando el motor de baja latencia RTSP
* Se agregó la propiedad AutoTransitions al motor VideoEditCoreX
* Los tipos System.Drawing.Rectangle y System.Drawing.Size se reemplazan por VisioForge.Types.Rectangle y VisioForge.Types.Size en todas las API multiplataforma
* Se agregan muestras MAUI (BETA)
* Compatibilidad mejorada con Snap Camera para codificación MP4 HW
* Licencias en línea actualizadas
* Se agregó demostración de luz de cámara
* Se agregó soporte de segmentos en Media Player SDK .Net (Motor multiplataforma)
* Se agregó API de lista de reproducción en Media Player SDK .Net (Motor solo para Windows)
* Se resolvieron problemas con la llamada "rtsp_source_create_audio_resampler" en el motor de baja latencia RTSP en Video Capture SDK .Net (Motor solo para Windows)
* Se agregó soporte para múltiples salidas Decklink en Video Capture SDK .Net y Video Edit SDK .Net (Motor solo para Windows)
* Se resolvieron problemas con el motor de reproducción inversa en Media Player SDK .Net (Motor solo para Windows)
* ONVIFControl y otras API relacionadas con ONVIF están disponibles para todas las plataformas
* Cambio importante en la API: la velocidad de fotogramas cambió de doble a VideoFrameRate en todas las API
* Se agregó decodificación HW de GPU para el motor VLC
* Se resolvió el problema con aplicaciones WPF HighDPI que usan EVR
* Se resolvió el problema con el método MediaPlayerCore.Video_Renderer_SetCustomWindowHandle
* Se agregó reproducción de fotograma anterior en Media Player SDK .Net (Motor multiplataforma)
* Se agregó demostración de captura de pantalla WPF a Media Blocks SDK .Net

## 15.4

* Se resolvió un problema con la propiedad Play_PauseAtFirstFrame ignorada
* Soporte HighDPI actualizado en muestras WinForms
* Se resolvió un problema con el soporte HighDPI para el renderizador de video Direct2D
* Se agregó API adicional a la clase ONVIFControl: GetDeviceCapabilities, GetMediaEndpoints
* Se resolvió el problema de recodificación forzada con la unión de archivos FFMPEG sin recodificación
* Actualización de Sentry
* Se agregaron configuraciones de interpolación de video para efectos de video Zoom y Pan
* Se agregó soporte de marco de interfaz de usuario GtkSharp para renderizado de video
* La API FastEdit se ha cambiado a asíncrona
* Se resolvió el problema de volteo de pantalla con la propiedad Video_Effects_AllowMultipleStreams del núcleo Video Capture SDK .Net
* Demostración RTSP MultiView actualizada (se agregó decodificación GPU, se agregó acceso a flujo RAW)
* Se agregó evento OnLoop en Media Player SDK .Net
* Se agregó función Loop en Media Blocks SDK .Net
* Avalonia VideoView se degradó a 0.10.12 debido a problemas de Avalonia UI con NativeControl
* Se agregó demostración de encriptador de archivos para Video Edit SDK .Net

## 15.3

* Tiempo de inicio de la aplicación mejorado para PC con tarjetas Decklink
* Soporte NDI SDK v5
* Se resolvió un problema con la salida MKV Legacy (excepción de conversión incorrecta).
* Optimizaciones de rendimiento de efectos de zoom y panorámica
* Se agregó API básica de Media Blocks (WIP)
* Se agregó transmisión de red HLS a Video Edit SDK .Net
* Se agregó la propiedad Rotate a WPF VideoView. Puede rotar el video 90, 180 o 270 grados. Además, puede usar el método GetImageLayer() para obtener la capa de imagen y aplicar transformaciones personalizadas
* Cambio de API - FilterHelpers renombrado a FilterDialogHelper
* Ensamblados VisioForge.Types y VisioForge.MediaFramework fusionados en VisioForge.Core
* Clases de interfaz de usuario movidas a ensamblados VisioForge.Core.UI.* y paquetes NuGet independientes
* VisioForge.Types renombrado a VisioForge.Core.Types
* VisioForge.Core ya no depende del marco Windows Forms

## 15.2

* Se agregaron propiedades HorizontalAlignment y VerticalAlignment a los logotipos de texto e imagen
* Soporte ONVIF actualizado, se resolvió un problema con el nombre de usuario y la contraseña especificados en la URL pero no especificados en la configuración de la fuente
* Se resolvió un problema con el cuadro de diálogo de salida FFMPEG.exe
* Se resolvió un problema con la captura separada en aplicaciones de servicio
* SDK migrado a System.Text.Json desde NewtonsoftJson
* Salida DirectCapture actualizada para cámaras IP
* Optimizaciones de rendimiento de procesamiento de video
* El tipo de propiedad IPCameraSourceSettings.URL cambió de cadena a `System.Uri`
* Se agregó salida DirectCapture ASF para cámaras IP

## 15.1

* Mensajes de depuración de Sentry deshabilitados en la consola
* Se agregó transmisión Icecast
* El tipo de propiedad VideoStreamInfo.FrameRate cambió a VideoFrameRate (con numerador y denominador) desde doble
* WPF VideoView actualizado, se resolvió el problema para la reproducción de flujo de cámara IP
* Cambio importante en la API: los ensamblados `VisioForge.Controls`, `VisioForge.Controls.UI`, `VisioForge.Controls.UI.Dialogs` y `VisioForge.Tools` se fusionan dentro del ensamblado `VisioForge.Core`
* La API de efectos de audio ahora usa el nombre de cadena en lugar del índice
* Se agregó soporte de Android en Media Player SDK .Net
* Se agregó un nuevo motor multiplataforma basado en GStreamer para admitir Windows y otras plataformas dentro del ciclo de desarrollo v15

## 15.0

* Se agregó la propiedad StatusOverlay para la clase VideoCapture. Asigne el objeto `TextStatusOverlay` a esta propiedad para agregar superposición de estado de texto, por ejemplo, para mostrar el texto "Conectando..." durante la conexión de la cámara IP.
* El motor de cámara IP RTSP Live555 se ha eliminado. Utilice motores RTSP de baja latencia o FFMPEG.
* Se resolvió el posible problema de SDK_Version.
* Se agregó API Settings_Load. Puede cargar el archivo de configuración guardado por Settings_JSON. Asegúrese de que los nombres de los dispositivos sean correctos.
* Se resolvió el problema con una excepción si la captura separada comenzaba antes de la llamada al método Start/StartAsync.
* Soporte RTP para el motor de fuente VLC.
* Cambio importante en la API: la propiedad SDK_State se ha eliminado. Ya no tenemos versiones TRIAL y FULL SDK.
* Cambio importante en la API: DirectShow_Filters_Show_Dialog, DirectShow_Filters_Has_Dialog, Audio_Codec_HasDialog, Audio_Codec_ShowDialog, Video_Codec_HasDialog, Video_Codec_ShowDialog, Filter_Supported_LAV, Filter_Exists_MatroskaMuxer, Filter_Exists_OGGMuxer, Filter_Exists_VorbisEncoder, Filter_Supported_EVR, Filter_Supported_VMR9 y Filter_Supported_NVENC se han movido a la clase VisioForge.Tools.FilterHelpers.
* Las clases `VFAudioStreamInfo`/`VFVideoStreamInfo` usan `Timespan` para la duración.
* Tipos Decklink del ensamblado VisioForge.Types movidos al espacio de nombres VisioForge.Types.Decklink.
* Telemetría actualizada.
* Cargador redist personalizado actualizado.
* Actualización NDI.
* Cambio importante en la API: La propiedad `Status` fue renombrada a `State`. El tipo de propiedad es `PlaybackState` en todos los SDKs.
* Cambio importante en la API: controles de interfaz de usuario divididos en Core (VideoCaptureCore, MediaPlayerCore, VideoEditCore) y VideoView.
* Cambio importante en la API: propiedades Video_CaptureDevice... fusionadas en la propiedad Video_CaptureDevice del tipo VideoCaptureSource.
* Cambio importante en la API: propiedades Audio_CaptureDevice... fusionadas en la propiedad Audio_CaptureDevice del tipo AudioCaptureSource.
* Cambio importante en la API: En Media Player SDK, las propiedades de API `Source_Stream` se fusionaron en la propiedad `Source_MemoryStream` del tipo `MemoryStreamSource`
* Reproducción de DVD actualizada
* Fuente FFMPEG actualizada
* Cambio importante en la API: tipos de Media Player SDK movidos del espacio de nombres VisioForge.Types a VisioForge.Types.MediaPlayer
* Cambio importante en la API: tipos de Video Capture SDK movidos del espacio de nombres VisioForge.Types a VisioForge.Types.VideoCapture
* Cambio importante en la API: tipos de Video Edit SDK movidos del espacio de nombres VisioForge.Types a "VisioForge.Types.VideoEdit"
* Cambio importante en la API: tipos de salida movidos del espacio de nombres VisioForge.Types a VisioForge.Types.Output
* Cambio importante en la API: tipos de efectos de video movidos del espacio de nombres VisioForge.Types a VisioForge.Types.VideoEffects
* Cambio importante en la API: tipos de efectos de audio movidos del espacio de nombres VisioForge.Types a VisioForge.Types.AudioEffects
* Cambio importante en la API: tipos de eventos movidos del espacio de nombres VisioForge.Types a VisioForge.Types.Events
* Se agregó el método Video_Renderer_SetCustomWindowHandle para establecer un renderizador de video personalizado mediante el identificador HWND de ventana/control Win32

## 14.4

* Soporte de Windows 11
* Telemetría actualizada
* Se resolvieron problemas con Imagen en Imagen en modo 2x2
* Se resolvieron problemas con la fuente de baja latencia MJPEG en .Net 5/.Net 6/.Net Core 3.1
* Se resolvió el problema con la transmisión de red UDP para la fuente Decklink
* VFMP4v11Output renombrado a VFMP4HWOutput
* Se agregó soporte de codificador Microsoft H265
* Se agregó soporte de codificador Intel QuickSync H265
* Se agregaron eventos OnDecklinkInputDisconnected/OnDecklinkInputReconnected
* Salida Decklink actualizada
* Se resolvieron problemas con la captura separada para salidas MP4 HW, MOV, MPEG-TS y MKVv2
* Se agregó la propiedad Video_CaptureDevice_CustomPinName. Puede usar esta propiedad para establecer un nombre de pin de salida personalizado para un dispositivo de captura de video con varios pines de video de salida
* Configuración redist personalizada actualizada
* Motor de cámara IP RTSP de baja latencia actualizado

## 14.3

* Se ha resuelto un problema con la creación del filtro de cambio de tamaño de video para redists NuGet
* Telemetría actualizada
* Salida VFDirectCaptureMP4Output actualizada
* Soporte de .Net 6 (vista previa)
* Nvidia CUDA eliminado. NVENC es una alternativa moderna y está disponible para codificación H264/HEVC.
* El motor de cámara IP MJPEG de baja latencia se ha actualizado
* La lista de fuentes NDI se ha actualizado
* Soporte ONVIF mejorado
* Se agregó soporte de .Net Core 3.1 para el motor de fuente RTSP de baja latencia
* Se resolvieron problemas con Imagen en Imagen para el modo 2x2
* Proyecto y soluciones divididos por archivos independientes para .Net Framework 4.7.2, .Net Core 3.1, .Net 5 y .Net 6

## 14.2

* Se resolvió un problema con la captura de flujo de audio con la salida Virtual Camera SDK habilitada
* VFMP4v8v10Output fue reemplazado por VFMP4Output
* Se agregó el método "CanStart" para elementos Video_CaptureDevices. El método devuelve verdadero si el dispositivo puede iniciarse y no se usa exclusivamente en otra aplicación
* Se agregó API async/await al ONVIFControl
* Se resolvió un problema con el procesamiento incorrecto de ColorKey en el efecto de video Text Overlay
* Se agregó soporte de velocidad de fotogramas forzada para la fuente de cámara IP RTSP de baja latencia
* Los codificadores AMD MP4v11 se actualizaron
* Se resolvió el problema de marca de tiempo que ocurría durante la pausa/reanudación de captura separada MP4v11
* Actualización de transmisión de red FFMPEG.exe
* La salida FFMPEG se actualizó a la última versión de FFMPEG
* Ya no es necesario instalar VC++ redist. La vinculación de VC++ cambió a estática (excepto salida XIPH opcional)
* Muchos filtros DirectShow base movidos al módulo VisioForge_BaseFilters

## 14.1

* Se agregó control WPF VideoView. Puede enviar fotogramas de video desde el evento OnVideoFrameBuffer al control para renderizarlos
* Valor de transparencia predeterminado correcto para un logotipo de texto
* Soporte ONVIF agregado a compilaciones .Net 5 / .Net Core 3.1
* Se agregó el método IP_Camera_ONVIF_ListSourcesAsync para descubrir cámaras ONVIF en la red local
* (CAMBIO DE API IMPORTANTE) Se cambió la API del dispositivo de captura de video para enumerar velocidades de fotogramas para admitir cámaras 4K modernas
* Decodificador MJPEG actualizado (rendimiento mejorado)
* Se eliminaron los codificadores heredados MP4 v8
* Soporte INotifyPropertyChanged en envoltorios WinForms/WPF para proporcionar soporte de aplicación MVVM
* Se resolvió el problema con la transmisión RTMPS a Facebook
* Fuente de cámara IP agregada a la demostración TimeShift
* Se agregó soporte de salida separada para MOV
* Se agregó bandera de inicio rápido FFMPEG para salida MP4v11 que usaba muxer MP4 FFMPEG
* Se agregó decodificación GPU para la fuente de cámara IP en aplicaciones de demostración
* Se agregó la propiedad CustomRedist_DisableDialog para deshabilitar el cuadro de diálogo de mensaje redist
* Se eliminaron ensamblados y demostraciones de Kinect. Contáctenos si aún necesita paquetes Kinect
* El perfil predeterminado MP4v10 se ha cambiado a Baseline / 5.0 para una mejor compatibilidad con el navegador

## 14.0

* Soporte de .Net 5.0
* Se resolvió el problema con fuentes Decklink no visibles en la versión SDK NuGet
* Se resolvió el problema con el notificador de dispositivo agregado/eliminado
* Se agregó fuente NDI alternativa en Video Capture SDK .Net
* Se agregó transmisión NDI (servidor) en Video Capture SDK .Net
* Se resolvió el problema de uso de captura separada para implementación NuGet
* Se resolvió el problema con logotipos de texto/imagen fusionados
* Notificador de dispositivo actualizado
* Se agregó la clase CameraPowerLineControl para controlar la opción de frecuencia de línea de alimentación de la cámara web
* Se han eliminado los efectos de audio heredados.
* Se eliminaron HTTP_FFMPEG, RTSP_UDP_FFMPEG, RTSP_TCP_FFMPEG y RTSP_HTTP_FFMPEG de la enumeración VFIPSource. Puede usar el valor Auto_FFMPEG
* Servidor HLS actualizado. Informe de error correcto sobre el puerto utilizado
* Se agregó transmisión NDI (servidor) en Video Edit SDK .Net
* Se agregó transmisión NDI (servidor) en Media Player SDK .Net
* Se agregó el método IP_Camera_CheckAvailable en Video Capture SDK .Net
* Filtro de fuente FFMPEG actualizado, más códecs compatibles y decodificación GPU agregada

## 12.1

* Migrado a .Net 4.6
* Se agregó la propiedad Debug_DisableMessageDialogs para deshabilitar el cuadro de diálogo de error si no se implementa el evento OnError.
* Se corrigió el problema con el cambio de tamaño en la pausa para controles WPF.
* Motor ONVIF actualizado en Video Capture SDK .Net
* Fuente What You Hear actualizada en Video Capture SDK .Net
* Se agregaron eventos OnPause/OnResume
* Demostración de YouTube actualizada en Media Player SDK .Net
* Soporte mejorado de cámaras web con codificador H264 integrado en Video Capture SDK .Net
* Fuente VLC actualizada
* Se eliminó la advertencia no deseada en la salida MP4 v11
* Un instalador para versiones TRIAL y FULL
* Mismos paquetes NuGet para versiones TRIAL y FULL
* Paquete NuGet .Net Core fusionado con paquete .Net Framework
* Se agregaron redists NuGet. ¡La implementación nunca fue tan simple!

## 12.0

* API Async / await para todos los SDKs
* Cambio importante en la API: toda la API relacionada con el tiempo ahora usa TimeSpan en lugar de long (milisegundos)
* Lector/escritor de etiquetas: carga de logotipo correcta para algunos formatos de video
* Se eliminaron los efectos de video DirectX 9 heredados
* Se corrigió el problema de progreso de conversión de audio en Video Edit SDK .Net
* Compatibilidad mejorada con .Net Core
* Salida Virtual Camera SDK agregada a Media Player SDK .Net (como uno de los renderizadores de video)
* Soporte de dispositivos NewTek NDI agregado a Video Capture SDK .Net como un nuevo motor para cámaras IP
* Se agregaron las propiedades Video_Effects_MergeImageLogos y Video_Effects_MergeTextLogos. Si tiene tres o más logotipos, puede establecer estas propiedades en verdadero para optimizar el rendimiento de los efectos de video
* Se agregó opción de tipo de lista de reproducción para transmisión de red HLS
* Se agregó servidor HTTP ligero integrado para transmisión de red HLS
* Se agregó soporte de video VR 360° en Media Player SDK .Net
* Procesamiento de video DirectX 11 mejorado
* Se agregó soporte de solo audio AAC MPEG-TS sin video para salida MPEG-TS
* Fuente de audio What You Hear mejorada
* Varias aplicaciones de demostración nuevas
* Salida MP4 v11 mejorada
* La captura separada para MP4 v11 puede dividir archivos sin pérdida de fotogramas
* Muchas correcciones de errores menores
* Ensamblados .Net Core actualizados a .Net Core 3.1 LTS
* Repositorio de demostraciones actualizado en GitHub

## 11.4

* Se agregó aplicación de demostración de conversión de video ASP.Net MVC a Video Edit SDK .Net
* Implementación alternativa de OSD para manejar cambios de Windows 10
* Efectos de video GPU actualizados
* Fuente de memoria actualizada en Media Player SDK .Net
* API OSD actualizada
* Se resolvieron problemas con el cifrado de video usando claves binarias
* Actualizar demostraciones de captura de pantalla para Video Capture SDK .Net, se agregó selección de ventana para capturar. Puede capturar cualquier ventana, incluidas las ventanas en segundo plano
* Efecto de mosaico agregado para demostración de Visión por Computadora en Video Capture SDK .Net
* Se agregó demostración de múltiples cámaras IP (WPF) en Video Capture SDK .Net
* Se agregó opción de cambio de tamaño de video personalizado para salida MP4v11
* Redists de módulo de fusión (MSM) agregados a todos los SDKs
* Salida FFMPEG.exe actualizada usando tuberías en lugar de dispositivos virtuales
* Se resolvió el problema con la opción de resolución de salida personalizada PIP en Video Capture SDK .Net
* Se resolvió el problema con el bloqueo de archivos usando el motor LAV en Media Player SDK .Net
* Se agregó procesamiento de video GPU basado en DirectX11

## 11.3

* Se resolvió el problema con la conexión del renderizador de audio si la salida Virtual Camera SDK está habilitada en Video Capture SDK
* Soporte de subtítulos mejorado con carga automática en Media Player SDK .Net
* Efectos de fundido de entrada/salida de audio actualizados
* Se agregó soporte de archivos MIDI y KAR en Media Player SDK .Net
* Se agregó soporte de archivos de karaoke CDG (y nueva aplicación de demostración) en Media Player SDK .Net
* Se agregó reproducción asíncrona en Media Player SDK .Net
* Serializador JSON integrado actualizado
* Se agregó decodificación GPU opcional en Media Player SDK .Net. Motores de decodificación disponibles: DXVA2, Direct3D 11, nVidia CUVID, Intel QuickSync
* Se agregó soporte de .Net Core 3.0, incluidas aplicaciones de demostración WinForms y WPF (solo Windows)

## 11.2

* Se agregó la propiedad Loop a Video Edit SDK .Net
* Potenciador de audio actualizado
* Fuente de baja latencia RTSP actualizada
* Se resolvió el problema de recorte para la fuente Decklink
* Se agregó propiedad para usar TCP o UDP en motor de baja latencia RTSP
* Implementación sin registro COM y derechos de administrador para Video Edit SDK y Media Player SDK (BETA)
* Mezclador de video actualizado con rendimiento mejorado
* Se agregó fragmento de código de reproducción de YouTube
* Se agregó método para mover OSD

## 11.1

* Se corrigió el problema de búsqueda con algunos archivos MP4 en Video Edit SDK
* Se corrigió el problema de estiramiento/buzón en la versión WPF de todos los SDKs
* Se corrigió el problema con un ecualizador en frecuencia de muestreo 16000 o menos
* Se corrigió el problema con el capturador de muestras para la fuente DirectShow en Media Player SDK
* Se corrigió la reproducción de archivos cifrados en Media Player SDK
* Se agregó la clase DVDInfoReader para leer información sobre archivos DVD
* Se resolvió el problema con el nombre de archivo incorrecto en el evento OnSeparateCaptureStopped
* Calidad de detección de código de barras mejorada para imágenes rotadas
* La versión mínima de .Net Framework es .Net 4.5 ahora
* Reproducción de YouTube mejorada en Media Player SDK. Se agregó el evento OnYouTubeVideoPlayback para seleccionar la calidad de video para la reproducción
* Se agregó la propiedad `Play_PauseAtFirstFrame` a Media Player SDK .Net. Si es verdadero, la reproducción se pausará en el primer fotograma
* Soporte de múltiples pantallas en demostración de Captura de Pantalla en Video Capture SDK .Net
* Se resolvió el problema con la reproducción de flujo de red en aplicaciones WPF de Media Player SDK .Net
* Se agregó reproducción de flujo MJPEG HTTP de baja latencia (cámaras IP u otras fuentes) en Video Capture SDK .Net
* Se agregó filtro DirectShow de fuente de audio falsa, que produce una señal de tono
* Demostración de Visión por Computadora actualizada en Video Capture SDK .Net
* Se agregó el método Frame_GetCurrentFromRenderer a todos los SDKs. Usando este método, puede obtener el fotograma de video renderizado actualmente directamente desde el renderizador de video.
* Se agregó reproducción de fuente RTSP de baja latencia en Video Capture SDK .Net

## 11.0

* Se corrigió error con salida MP4 v11, configuración GOP personalizada
* Decodificador MJPEG actualizado
* Se corrigió error con salida MP4 v11. Se agregó soporte completo de Windows 7
* El evento OnStop de Video Edit SDK devuelve un estado exitoso
* Actualización de demostración principal de Video Capture SDK: la pantalla múltiple puede usar automáticamente pantallas externas conectadas
* Actualización de demostración principal de Media Player SDK: la pantalla múltiple puede usar automáticamente pantallas externas conectadas
* Se agregó fundido de entrada / salida para logotipo de texto
* Salida Decklink actualizada
* Video Edit SDK puede cortar rápidamente archivos de fuentes de red (HTTP/HTTPS)
* Se agregó demostración de Visión por Computadora, con contador de autos/peatones y detector/rastreador de rostros/ojos/nariz/boca
* Salida MP4 actualizada para usar muxer alternativo que proporciona velocidad de fotogramas constante
* Se agregó salida MPEG-TS
* Se agregó salida MOV
