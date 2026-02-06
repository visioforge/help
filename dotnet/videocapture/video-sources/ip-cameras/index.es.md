---
title: Integración de Cámaras IP y Fuentes de Red en .NET
description: Implemente cámaras IP, flujos RTSP/RTMP, dispositivos ONVIF y fuentes de video de red en .NET con ejemplos de código y mejores prácticas.
sidebar_label: Cámaras IP y Fuentes de Red
order: 19
---

# Guía Completa para la Integración de Cámaras IP y Fuentes de Red

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introducción a las Fuentes de Video de Red

Las aplicaciones de video modernas a menudo requieren integración con varias fuentes de video de red. El Video Capture SDK para .NET proporciona un soporte robusto para diversos tipos de cámaras IP y flujos de video de red, permitiendo a los desarrolladores incorporar fácilmente video de red en vivo en aplicaciones .NET.

Esta guía completa cubre todas las fuentes de red soportadas y proporciona ejemplos de implementación claros tanto para los marcos VideoCaptureCore como VideoCaptureCoreX.

## Tipos de Cámaras IP y Fuentes de Red Soportadas

El SDK ofrece una amplia compatibilidad con varias fuentes de video de red, incluyendo:

* [Cámaras compatibles con ONVIF](onvif.md) - Estándar de la industria para productos de seguridad basados en IP
* [Cámaras RTSP](rtsp.md) - Cámaras con Protocolo de Transmisión en Tiempo Real
* Cámaras HTTP MJPEG - Transmisión Motion JPEG sobre HTTP
* Cámaras y flujos UDP - Flujos basados en el Protocolo de Datagramas de Usuario
* [Cámaras NDI](ndi.md) - Cámaras con tecnología de Interfaz de Dispositivo de Red
* Servidores y cámaras SRT - Protocolo de Transporte Seguro y Fiable
* Servidores VNC - Computación en Red Virtual para captura de pantalla
* Flujos RTMP - Fuentes de Protocolo de Mensajería en Tiempo Real
* Flujos HLS - Fuentes de Transmisión en Vivo HTTP
* Fuentes de video HTTP - Varios flujos de video basados en HTTP

Cada protocolo ofrece ventajas específicas dependiendo de los requisitos de su aplicación, desde necesidades de baja latencia hasta transmisión de video de alta calidad.

## Implementación de Fuente Universal para Protocolos de Red

Nuestro SDK proporciona un enfoque universal para manejar la mayoría de las fuentes de video de red, incluyendo RTSP, RTMP, HTTP y otros. Esta flexibilidad permite a los desarrolladores centrarse en la lógica de la aplicación en lugar de los detalles de implementación específicos del protocolo.

=== "VideoCaptureCore"

    
    ### Implementación de Fuente Universal en VideoCaptureCore
    
    Para aplicaciones VideoCaptureCore, puede usar la clase IPCameraSourceSettings para definir su fuente de video de red:
    
    ```cs
    // Crear y configurar la fuente de red
    VideoCapture1.IP_Camera_Source = new IPCameraSourceSettings
    {
        URL = "rtsp://192.168.1.100:554/stream1", // La URL del flujo
        Login = "admin", // Credenciales de autenticación opcionales
        Password = "password123",
        AudioCapture = true, // Establecer en true para incluir audio de la fuente
        Type = VFIPSource.Auto_VLC // El motor de procesamiento a utilizar
    };
    ```
    
    #### Tipos de Motores Disponibles
    
    El SDK soporta múltiples motores subyacentes para procesar flujos de red, proporcionando flexibilidad para diferentes escenarios:
    
    * `Auto_VLC` - Utiliza el motor VLC, ofreciendo amplio soporte de protocolos y compatibilidad
    * `Auto_FFMPEG` - Utiliza el motor FFMPEG, proporcionando amplio soporte de formatos y personalización
    * `Auto_LAV` - Utiliza el motor LAV, optimizado para entornos Windows
    
    ### Personalización de Configuraciones FFMPEG para Usuarios Avanzados
    
    El SDK permite un control detallado sobre la configuración de FFMPEG cuando se utiliza el motor FFMPEG. Esto proporciona a los usuarios avanzados amplias opciones de personalización:
    
    ```cs
    // Configurar parámetros personalizados de FFMPEG
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("rtsp_transport", "tcp"); // Forzar transporte TCP
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("timeout", "3000000"); // Establecer tiempo de espera en microsegundos
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("buffer_size", "1000000"); // Ajustar tamaño del búfer
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("max_delay", "500000"); // Establecer retardo máximo permitido
    ```
    
    Estos parámetros se pasan directamente a la función avformat_open_input en FFMPEG, proporcionando opciones de personalización profunda para el rendimiento de la transmisión de red.
    

=== "VideoCaptureCoreX"

    
    ### Implementación de Fuente Universal en VideoCaptureCoreX
    
    Para aplicaciones VideoCaptureCoreX, el enfoque utiliza los patrones asíncronos modernos:
    
    ```cs
    // Preparar la URL de la fuente con autenticación si es necesario
    var uri = new Uri("rtsp://192.168.1.100:554/stream1");
    if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
    {
        uri = new UriBuilder(uri) { UserName = login, Password = password }.Uri;
    }
    
    // Crear la fuente universal con la configuración deseada
    var source = await UniversalSourceSettings.CreateAsync(
        uri,
        renderAudio: true, // Incluir flujo de audio
    );
    
    // Aplicar la fuente al objeto de captura
    VideoCapture1.Video_Source = source;
    ```
    
    El enfoque VideoCaptureCoreX proporciona un patrón asíncrono basado en tareas más moderno, haciéndolo ideal para aplicaciones de interfaz de usuario receptivas.
    


## Implementación de MJPEG de Baja Latencia

Para aplicaciones que requieren latencia mínima, como monitoreo de seguridad o sistemas de control en tiempo real, el SDK ofrece una implementación especializada de MJPEG de baja latencia con una latencia típica inferior a 100ms.

=== "VideoCaptureCore"

    
    ### Configuración de MJPEG de Baja Latencia en VideoCaptureCore
    
    ```cs
    // Crear objeto de configuración
    var settings = new IPCameraSourceSettings
    {
        URL = "http://192.168.1.100/video.mjpg",
        Login = "admin",
        Password = "pass123",
        // Usar el motor dedicado de MJPEG de baja latencia
        Type = IPSourceEngine.HTTP_MJPEG_LowLatency
    };
    
    // Aplicar configuración al objeto VideoCaptureCore
    VideoCapture1.IP_Camera_Source = settings;
    ```
    
    Este modo especializado evita el procesamiento innecesario para minimizar la latencia, haciéndolo ideal para aplicaciones sensibles al tiempo.
    

=== "VideoCaptureCoreX"

    
    ### Configuración de MJPEG de Baja Latencia en VideoCaptureCoreX
    
    ```cs
    // Crear configuración de fuente HTTP MJPEG especializada
    var mjpeg = await HTTPMJPEGSourceSettings.CreateAsync(
        new Uri("http://192.168.1.100/video.mjpg"),
        "admin", // Nombre de usuario
        "pass123"
    );
    
    // Aplicar configuración al objeto VideoCaptureCoreX
    VideoCapture1.Video_Source = mjpeg;
    ```
    
    La implementación de MJPEG de baja latencia es particularmente útil para sistemas de vigilancia, monitoreo remoto y aplicaciones industriales donde minimizar el retardo es crítico.
    


## Implementación de Transporte Seguro y Fiable (SRT)

SRT es un protocolo moderno diseñado para la transmisión de video fiable sobre redes impredecibles. Es especialmente valioso para mantener la calidad en condiciones de red desafiantes.

=== "VideoCaptureCoreX"

    
    ### Implementación de Fuente SRT en VideoCaptureCoreX
    
    ```cs
    // Crear configuración de fuente SRT con la URL del servidor
    var srt = await SRTSourceSettings.CreateAsync("srt://streaming-server.example.com:7001");
    
    // Aplicar la fuente SRT al objeto de captura
    VideoCapture1.Video_Source = srt;
    ```
    
    SRT proporciona ventajas significativas para la transmisión fiable a través de redes desafiantes, ofreciendo seguridad integrada, corrección de errores y control de congestión.
    


## Manejo de Desconexión de Red

Las implementaciones robustas de fuentes de red deben manejar las interrupciones de conexión con elegancia. El SDK proporciona mecanismos integrados para detectar y responder a desconexiones de red.

=== "VideoCaptureCore"

    
    ### Implementación de Manejo de Desconexión de Red en VideoCaptureCore
    
    ```cs
    // Habilitar detección de desconexión de red
    VideoCapture1.DisconnectEventInterval = TimeSpan.FromSeconds(5); // Comprobar cada 5 segundos
    
    // Registrar el manejador de eventos de desconexión
    VideoCapture1.OnNetworkSourceDisconnect += VideoCapture1_OnNetworkSourceDisconnect;
    
    // Implementar el manejador de eventos de desconexión
    private void VideoCapture1_OnNetworkSourceDisconnect(object sender, EventArgs e)
    {
        Invoke((Action)(
            async () =>
            {
                await VideoCapture1.StopAsync();
                
                // Notificar al usuario
                MessageBox.Show(this, "¡Fuente de red desconectada!");
            }));
    }
    ```
    
    Implementar un manejo adecuado de desconexiones mejora la fiabilidad de la aplicación y la experiencia del usuario durante las fluctuaciones de la red.
    


## Implementación de Fuente VNC

Virtual Network Computing (VNC) permite capturar pantallas de escritorio remoto como fuentes de video, útil para grabación de pantalla y aplicaciones de asistencia remota.

=== "VideoCaptureCoreX"

    
    ### Implementación de Fuente VNC en VideoCaptureCoreX
    
    ```cs
    // Crear objeto de configuración de fuente VNC
    var vncSettings = new VNCSourceSettings();
    
    // Configurar usando host y puerto
    vncSettings.Host = "remote-server.example.com";
    vncSettings.Port = 5900; // Puerto VNC predeterminado
    
    // O configurar usando formato URI
    // vncSettings.Uri = "vnc://remote-server.example.com:5900";
    
    // Establecer credenciales de autenticación
    vncSettings.Password = "secure-password";
    
    // Opcional: Configurar ajustes avanzados de VNC
    vncSettings.EnableCursor = true; // Capturar cursor del ratón
    vncSettings.CompressionLevel = 5; // 0-9, valores más altos = más compresión
    vncSettings.QualityLevel = 8; // 0-9, valores más altos = mejor calidad
    vncSettings.UpdateInterval = 100; // Intervalo de actualización en milisegundos
    
    // Aplicar la configuración al objeto de captura
    VideoCapture1.Video_Source = vncSettings;
    ```
    
    La implementación de fuente VNC proporciona una solución completa para la captura de escritorio remoto con configuraciones de calidad y rendimiento personalizables.
    
    * [Ejemplo completo de fuente VNC (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/VNC%20Source%20Demo)
    


## Mejores Prácticas para Fuentes de Red

Para un rendimiento óptimo al trabajar con fuentes de video de red:

1. **Gestión de Búfer**: Ajuste el almacenamiento en búfer según la estabilidad de la fuente y los requisitos de latencia.
2. **Manejo de Errores**: Implemente un manejo de errores completo para interrupciones de red.
3. **Autenticación**: Utilice siempre almacenamiento seguro de credenciales para la autenticación de cámaras.
4. **Agrupación de Conexiones**: Reutilice conexiones al acceder a múltiples flujos desde el mismo dispositivo.
5. **Consideración de Ancho de Banda**: Supervise y gestione el consumo de ancho de banda, especialmente con múltiples fuentes.

## Conclusión

El Video Capture SDK para .NET proporciona un soporte completo para cámaras IP y fuentes de red, permitiendo a los desarrolladores construir aplicaciones de video sofisticadas. Con soporte para múltiples protocolos y opciones de configuración flexibles, se adapta a una amplia gama de casos de uso, desde vigilancia de seguridad hasta aplicaciones de transmisión de medios.

Para ejemplos de implementación adicionales y escenarios de uso avanzados, explore nuestro repositorio de GitHub con ejemplos de código completos.

---

Visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener ejemplos de código más completos y aplicaciones de demostración.
