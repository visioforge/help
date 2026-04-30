---
title: Captura de cámaras RTSP en .NET: latencia y protocolos
description: Implementa y configura flujos de cámaras RTSP en .NET con opciones de baja latencia, código de ejemplo y mejores prácticas para UDP y TCP.
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

# Integrar Flujos de Cámaras RTSP en Aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! info "Soporte multiplataforma"
    El motor `VideoCaptureCoreX` funciona en **Windows, macOS, Linux, Android e iOS** vía GStreamer; el motor clásico `VideoCaptureCore` es solo Windows. Consulta la [matriz de soporte de plataformas](../../../platform-matrix.md) para códecs y detalles de aceleración por hardware, y la [guía de despliegue en Linux](../../../deployment-x/Ubuntu.md) para configuración en Ubuntu / NVIDIA Jetson / Raspberry Pi.

## Configurar Fuentes de Cámaras RTSP Estándar

Implementar flujos de cámaras RTSP en tus aplicaciones .NET proporciona acceso flexible a cámaras de red y flujos de video. Esta poderosa capacidad permite monitoreo en tiempo real, características de vigilancia y procesamiento de video directamente dentro de tu aplicación.

=== "VideoCaptureCore"

    Para opciones de conexión adicionales y protocolos alternativos, por favor consulta nuestra documentación detallada de [cámaras IP](index.md) que cubre un amplio rango de enfoques de integración de cámaras.

=== "VideoCaptureCoreX"

    
    ```cs
    // Crear objeto de configuración de fuente RTSP
    var rtsp = await RTSPSourceSettings.CreateAsync(new Uri("url"), "login", "password", true /*¿capturar audio?*/);
    
    // Establecer fuente al objeto VideoCaptureCoreX
    VideoCapture1.Video_Source = rtsp;
    ```
    


## Optimizar para Streaming RTSP de Baja Latencia

La baja latencia es crítica para muchas aplicaciones en tiempo real incluyendo monitoreo de seguridad, sistemas interactivos y transmisión en vivo. Nuestro SDK proporciona configuraciones especializadas para minimizar el retraso entre captura y visualización.

=== "VideoCaptureCore"

    
    Nuestro SDK incluye un modo dedicado específicamente diseñado para streaming RTSP de baja latencia. Cuando está configurado apropiadamente, este modo típicamente logra latencia inferior a 250 milisegundos, haciéndolo ideal para aplicaciones sensibles al tiempo.
    
    ```cs
    // Crear el objeto de configuración de fuente.
    var settings = new IPCameraSourceSettings();
    
    // Configurar dirección IP, nombre de usuario, contraseña, etc.
    // ...
    
    // Establecer modo RTSP de Baja Latencia.
    settings.Type = IPSourceEngine.RTSP_LowLatency;
    
    // Establecer modo UDP o TCP.
    settings.RTSP_LowLatency_UseUDP = false; // true para usar UDP, false para usar TCP
    
    // Establecer fuente al objeto VideoCaptureCore.
    VideoCapture1.IP_Camera_Source = settings;
    ```
    

=== "VideoCaptureCoreX"

    
    **NUEVO: Modo de Ultra Baja Latencia (60-120ms)**
    
    VideoCaptureCoreX ahora incluye un modo dedicado de baja latencia que logra 60-120ms de latencia total - hasta 10 veces más rápido que el modo estándar. Perfecto para vigilancia en tiempo real, monitoreo interactivo y aplicaciones de seguridad.
    
    ```cs
    // Crear configuración de fuente RTSP
    var rtsp = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream"), 
        "admin", 
        "password", 
        true); // habilitar audio
    
    // Habilitar modo de baja latencia - optimiza para retraso mínimo (60-120ms)
    rtsp.LowLatencyMode = true;
    
    // Establecer fuente a VideoCaptureCoreX
    VideoCapture1.Video_Source = rtsp;
    ```
    
    **Cómo Funciona:**
    - Establece búfer de jitter RTSP a 80ms (vs. 1000ms predeterminado)
    - Optimiza buffering de cola interna (máximo 2 fotogramas)
    - Deshabilita reordenamiento de paquetes para retraso mínimo
    - Compromiso: Optimiza velocidad sobre estabilidad
    


## Protocolo de Transporte: UDP vs TCP

La elección entre UDP y TCP depende de tu entorno de red y requisitos de aplicación:

### UDP (User Datagram Protocol)
- **Ventajas**: Menor latencia, menos sobrecarga de protocolo
- **Desventajas**: Puede perder paquetes en redes inestables
- **Mejor para**: Redes confiables, aplicaciones sensibles a latencia

### TCP (Transmission Control Protocol)
- **Ventajas**: Entrega confiable, funciona a través de firewalls
- **Desventajas**: Mayor latencia debido a retransmisión de paquetes
- **Mejor para**: Redes inestables, conexiones a través de Internet

```cs
// Configurar para usar TCP (más confiable)
settings.RTSP_LowLatency_UseUDP = false;

// O configurar para usar UDP (menor latencia)
settings.RTSP_LowLatency_UseUDP = true;
```

## Aplicaciones de Ejemplo

Explora estas aplicaciones de ejemplo para ver la integración de cámaras RTSP en acción:

=== "VideoCaptureCore"

    
    - [Demo Principal de Video Capture (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
    - [Demo de Cámara IP (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/IP%20Capture)
    

=== "VideoCaptureCoreX"

    
    - [Demo IP Capture X (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture)
    


## Mejores Prácticas

1. **Probar latencia con tu hardware específico** - El rendimiento varía según cámara y red
2. **Usar TCP si experimentas pérdida de paquetes** - Más confiable para conexiones remotas
3. **Monitorear uso de ancho de banda** - Los flujos de alta resolución requieren más capacidad
4. **Implementar reconexión automática** - Las conexiones de red pueden fallar
5. **Considerar seguridad** - Usar autenticación y considerar RTSP sobre TLS

¿Necesitas la URL RTSP para tu cámara? Consulta nuestro [directorio de marcas de cámaras IP](../../../camera-brands/index.md) para URLs RTSP específicas por marca y ejemplos de conexión.

## Documentación Relacionada

- [Inmersión profunda en el protocolo RTSP](../../../general/network-streaming/rtsp.md) — cómo funciona RTSP, opciones de transporte y arquitectura de streaming
- [Integración de cámara IP ONVIF](onvif.md) — WS-Discovery, gestión de perfiles y control PTZ
- [Integración de fuente NDI](ndi.md) — alternativa profesional de video-sobre-IP para estudio y broadcast
- [Tutorial de vista previa en vivo de cámara IP](../../video-tutorials/ip-camera-preview.md) — video explicativo con ejemplo mínimo en C#
- [Grabar stream RTSP a MP4](../../video-tutorials/ip-camera-capture-mp4.md) — capturar cualquier cámara IP a archivo
- [Reproductor RTSP de Media Blocks](../../../mediablocks/Guides/rtsp-player-csharp.md) — alternativa basada en pipeline con Media Blocks SDK
- [Grid RTSP multi-cámara (muro NVR)](../../../mediablocks/Guides/multi-camera-rtsp-grid.md) — muro de vista previa 4×4 para WPF y MAUI
- [Reconexión RTSP y fallback switch](../../../general/network-sources/reconnection-and-fallback.md) — eventos de desconexión y `FallbackSwitch` imagen/texto/media en todos los SDK

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.