---
title: Streaming de cámaras RTSP en C# .NET — modos UDP y TCP
description: Conéctate a flujos de cámaras RTSP con VisioForge Video Capture SDK. Configuración de baja latencia, opciones de transporte UDP/TCP y ejemplos de código en C#.
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
  - UDP
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureCore
  - RTSPSourceSettings
  - IPCameraSourceSettings
  - IPSourceEngine

---

# Integración de flujos de cámaras RTSP en aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! info "Soporte multiplataforma"
    El motor `VideoCaptureCoreX` funciona en **Windows, macOS, Linux, Android e iOS** vía GStreamer; el motor clásico `VideoCaptureCore` es solo Windows. Consulta la [matriz de soporte de plataformas](../../../platform-matrix.md) para detalles de códecs y aceleración por hardware, y la [guía de despliegue en Linux](../../../deployment-x/Ubuntu.md) para la configuración en Ubuntu / NVIDIA Jetson / Raspberry Pi.

## Configurar fuentes de cámaras RTSP estándar

Implementar flujos de cámaras RTSP en tus aplicaciones .NET proporciona acceso flexible a cámaras de red y flujos de video. Esta capacidad permite monitorización en tiempo real, funcionalidades de vigilancia y procesamiento de video directamente dentro de tu aplicación.

=== "VideoCaptureCore"

    Para opciones de conexión adicionales y protocolos alternativos, consulta nuestra documentación detallada de [cámaras IP](index.md), que cubre un amplio rango de enfoques de integración de cámaras.

=== "VideoCaptureCoreX"

    
    ```cs
    // Crear el objeto de configuración de fuente RTSP
    var rtsp = await RTSPSourceSettings.CreateAsync(new Uri("rtsp://192.168.1.1:554/live"), "login", "password", true /*¿capturar audio?*/);
    
    // Asignar la fuente al objeto VideoCaptureCoreX
    VideoCapture1.Video_Source = rtsp;
    ```
    


## Optimización para streaming RTSP de baja latencia

La baja latencia es crítica para muchas aplicaciones en tiempo real, incluyendo monitorización de seguridad, sistemas interactivos y transmisión en vivo. Nuestro SDK proporciona configuraciones especializadas para minimizar el retraso entre la captura y la visualización.

=== "VideoCaptureCore"

    
    Nuestro SDK incluye un modo dedicado específicamente diseñado para streaming RTSP de baja latencia. Cuando está configurado correctamente, este modo típicamente alcanza latencias por debajo de 250 milisegundos, lo que lo hace ideal para aplicaciones sensibles al tiempo.
    
    ```cs
    // Crear el objeto de configuración de fuente.
    var settings = new IPCameraSourceSettings();
    
    // Configurar dirección IP, nombre de usuario, contraseña, etc.
    // ...
    
    // Establecer el modo RTSP de baja latencia.
    settings.Type = IPSourceEngine.RTSP_LowLatency;
    
    // Establecer modo UDP o TCP.
    settings.RTSP_LowLatency_UseUDP = false; // true para usar UDP, false para usar TCP
    
    // Asignar la fuente al objeto VideoCaptureCore.
    VideoCapture1.IP_Camera_Source = settings;
    ```
    

=== "VideoCaptureCoreX"

    
    **NUEVO: modo de ultra baja latencia (60-120 ms)**
    
    VideoCaptureCoreX ahora incluye un modo dedicado de baja latencia que alcanza 60-120 ms de latencia total — hasta 10 veces más rápido que el modo estándar. Perfecto para vigilancia en tiempo real, monitorización interactiva y aplicaciones de seguridad.
    
    ```cs
    // Crear configuración de fuente RTSP
    var rtsp = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream"), 
        "admin", 
        "password", 
        true); // habilitar audio
    
    // Habilitar modo de baja latencia — optimiza para un retraso mínimo (60-120 ms)
    rtsp.LowLatencyMode = true;
    
    // Asignar la fuente a VideoCaptureCoreX
    VideoCapture1.Video_Source = rtsp;
    ```
    
    **Cómo funciona:**
    - Establece el búfer de jitter RTSP a 80 ms (frente a los 1000 ms por defecto)
    - Optimiza el buffering interno de la cola (máximo 2 fotogramas)
    - Deshabilita el reordenamiento de paquetes para minimizar el retraso
    - Compromiso: optimiza velocidad sobre estabilidad
    
    **Cuándo usarlo:**
    - ✓ Vigilancia y monitorización en tiempo real
    - ✓ Sistemas de seguridad en vivo
    - ✓ Aplicaciones de video interactivas
    - ✓ Sistemas de control remoto
    - ✗ Grabación en redes inestables (usa el modo predeterminado)
    - ✗ Captura para archivado a largo plazo (usa el modo predeterminado)
    
    **Configuración avanzada:**
    
    Para un control ajustado, puedes configurar manualmente los parámetros de latencia:
    
    ```cs
    var rtsp = await RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled);
    
    // Configuración manual de baja latencia
    rtsp.Latency = TimeSpan.FromMilliseconds(50);  // Tamaño de búfer personalizado
    rtsp.BufferMode = RTSPBufferMode.None;          // Sin buffering de jitter
    rtsp.DropOnLatency = false;                     // No descartar fotogramas
    ```
    


## Ejemplos de implementación y aplicaciones de referencia

Estos proyectos de ejemplo demuestran patrones prácticos de implementación RTSP y pueden servir como puntos de partida para tu propio desarrollo. Revisar estos ejemplos te ayudará a comprender las mejores prácticas para la integración RTSP.

=== "VideoCaptureCore"

    
    - [Demo principal del SDK (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [Ejemplo de fuente RTSP y otras (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/IP_Capture)
    - [Demo principal del SDK (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)

=== "VideoCaptureCoreX"

    - [Ejemplo de fuente RTSP (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture)


## Solución de problemas en conexiones RTSP

Cuando trabajas con cámaras RTSP, puedes encontrarte con problemas de conectividad relacionados con la configuración de red, los ajustes del firewall o la autenticación. Estos son los factores clave a considerar:

- Verifica la conectividad de red entre tu aplicación y la cámara
- Asegúrate de proporcionar las credenciales de autenticación correctas
- Comprueba si hay firewalls bloqueando los puertos requeridos (típicamente 554 para RTSP)
- Considera usar TCP en lugar de UDP si experimentas pérdida de paquetes
- Prueba los flujos de cámara con VLC o herramientas similares para aislar problemas específicos de la aplicación

¿Necesitas la URL RTSP para tu cámara? Explora nuestro [directorio de marcas de cámaras IP](../../../camera-brands/index.md) para URLs RTSP específicas por marca y ejemplos de conexión.

## Documentación relacionada

- [Inmersión profunda en el protocolo RTSP](../../../general/network-streaming/rtsp.md) — cómo funciona RTSP, opciones de transporte y arquitectura de streaming
- [Integración de cámara IP ONVIF](onvif.md) — WS-Discovery, gestión de perfiles y control PTZ
- [Integración de fuente NDI](ndi.md) — alternativa profesional de video sobre IP para estudio y broadcast
- [Tutorial de vista previa en vivo de cámara IP](../../video-tutorials/ip-camera-preview.md) — video explicativo con ejemplo mínimo en C#
- [Grabar flujo RTSP a MP4](../../video-tutorials/ip-camera-capture-mp4.md) — capturar cualquier cámara IP a archivo
- [Reproductor RTSP de Media Blocks](../../../mediablocks/Guides/rtsp-player-csharp.md) — alternativa basada en pipeline con Media Blocks SDK
- [Grid RTSP multi-cámara (muro NVR)](../../../mediablocks/Guides/multi-camera-rtsp-grid.md) — muro de vista previa 4×4 para WPF y MAUI
- [Reconexión RTSP y fallback switch](../../../general/network-sources/reconnection-and-fallback.md) — eventos de desconexión y `FallbackSwitch` imagen/texto/medios en todos los SDK
