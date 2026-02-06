---
title: Integración de Cámaras IP ONVIF - Guía Completa
description: Integración completa de cámaras IP ONVIF en .NET cubriendo descubrimiento, conexión, control PTZ, grabación, streaming y visión por computadora.
---

# Integración de Cámaras IP ONVIF - Guía Completa {: #integracion-de-camaras-ip-onvif---guia-completa }

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tabla de Contenidos

- [Integración de Cámaras IP ONVIF - Guía Completa](#integracion-de-camaras-ip-onvif---guia-completa)
  - [Tabla de Contenidos](#tabla-de-contenidos)
  - [¿Qué es ONVIF?](#que-es-onvif)
  - [Beneficios de la Integración ONVIF](#beneficios-de-la-integracion-onvif)
  - [Descubrimiento y Enumeración de Cámaras](#descubrimiento-y-enumeracion-de-camaras)
    - [Descubriendo Cámaras ONVIF en Tu Red](#descubriendo-camaras-onvif-en-tu-red)
    - [Consultando Capacidades de Cámara](#consultando-capacidades-de-camara)
  - [Conectando a Cámaras ONVIF](#conectando-a-camaras-onvif)
    - [Conexión Básica](#conexion-basica)
  - [Trabajando con Perfiles de Medios](#trabajando-con-perfiles-de-medios)
  - [Vista Previa de Video](#vista-previa-de-video)
    - [Configuración Básica de Vista Previa](#configuracion-basica-de-vista-previa)
    - [Vista Previa de Baja Latencia](#vista-previa-de-baja-latencia)
  - [Control PTZ (Pan-Tilt-Zoom)](#control-ptz-pan-tilt-zoom)
    - [Operaciones PTZ Básicas](#operaciones-ptz-basicas)
    - [Preajustes PTZ](#preajustes-ptz)
    - [Posicionamiento Absoluto](#posicionamiento-absoluto)
  - [Acciones y Capacidades de Cámara](#acciones-y-capacidades-de-camara)
    - [Consultar Capacidades de Cámara](#consultar-capacidades-de-camara)
    - [Reiniciar Cámara](#reiniciar-camara)
    - [Obtener Fecha y Hora del Sistema](#obtener-fecha-y-hora-del-sistema)
  - [Convertir Cámara Local en Fuente ONVIF](#convertir-camara-local-en-fuente-onvif)
  - [Mejores Prácticas](#mejores-practicas)
    - [Gestión de Conexión](#gestion-de-conexion)
    - [Optimización de Rendimiento](#optimizacion-de-rendimiento)
    - [Consideraciones de Red](#consideraciones-de-red)
    - [Seguridad](#seguridad)
    - [Manejo de Errores](#manejo-de-errores)
  - [Solución de Problemas](#solucion-de-problemas)
    - [Problemas Comunes](#problemas-comunes)
      - ["No se puede conectar a cámara ONVIF"](#no-se-puede-conectar-a-camara-onvif)
      - ["No se descubrieron cámaras"](#no-se-descubrieron-camaras)
      - ["El stream no se reproduce"](#el-stream-no-se-reproduce)
      - ["Alto uso de CPU durante grabación"](#alto-uso-de-cpu-durante-grabacion)
      - ["Comandos PTZ no funcionan"](#comandos-ptz-no-funcionan)
    - [Herramientas de Diagnóstico](#herramientas-de-diagnostico)
      - [Habilitar Registro de Depuración](#habilitar-registro-de-depuracion)
      - [Probar Conexión RTSP](#probar-conexion-rtsp)
    - [Obtener Ayuda](#obtener-ayuda)
    - [Demos Relacionados](#demos-relacionados)

## ¿Qué es ONVIF?

ONVIF (Open Network Video Interface Forum) es un protocolo estándar de la industria que habilita interoperabilidad perfecta entre productos de video en red de diferentes fabricantes. Este protocolo define una interfaz común para dispositivos de seguridad basados en IP incluyendo cámaras, NVR (Network Video Recorders), y sistemas de control de acceso.

**Beneficios Clave:**
- **Independencia de Proveedor**: Trabaja con cámaras de diferentes fabricantes usando una API unificada
- **Comunicación Estandarizada**: Métodos consistentes para descubrimiento de dispositivos, streaming y control
- **A Prueba del Futuro**: Nuevos dispositivos compatibles con ONVIF funcionan con aplicaciones existentes
- **Conjunto Rico de Características**: Acceso a perfiles, streams de medios, eventos, PTZ y más

## Beneficios de la Integración ONVIF

- **Neutralidad de Proveedor**: Construye aplicaciones que funcionan con cámaras de múltiples fabricantes
- **Desarrollo a Prueba del Futuro**: A medida que nuevas cámaras compatibles con ONVIF entran al mercado, tu aplicación las soportará
- **Comunicación Estandarizada**: Métodos consistentes para descubrimiento de dispositivos, streaming de video y controles PTZ
- **Tiempo de Desarrollo Reducido**: No hay necesidad de implementar APIs propietarias para cada marca de cámara
- **Características Avanzadas**: Acceso a perfiles, streams de medios, eventos y gestión de dispositivos

## Descubrimiento y Enumeración de Cámaras

### Descubriendo Cámaras ONVIF en Tu Red

El primer paso para trabajar con cámaras ONVIF es descubrirlas en tu red local usando el protocolo WS-Discovery.

```cs
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFDiscovery.Models;

private Discovery _onvifDiscovery = new Discovery();
private CancellationTokenSource _cts;

// Descubrir cámaras por 5 segundos
_cts = new CancellationTokenSource();

try
{
    await _onvifDiscovery.Discover(5, (device) =>
    {
        if (device.XAdresses?.Any() == true)
        {
            var address = device.XAdresses.FirstOrDefault();
            if (!string.IsNullOrEmpty(address))
            {
                Console.WriteLine($"Cámara encontrada en: {address}");
                // Agregar a tu lista de UI, etc.
            }
        }
    }, _cts.Token);
}
catch (OperationCanceledException)
{
    // Descubrimiento cancelado
}
```

**Características Clave:**
- **Protocolo WS-Discovery**: Descubre automáticamente cámaras compatibles con ONVIF en la red local
- **Control de Tiempo de Espera**: Especifica duración del descubrimiento en segundos
- **Callback Asíncrono**: Recibe dispositivos descubiertos en tiempo real a medida que responden
- **Soporte de Cancelación**: Cancela descubrimiento usando CancellationToken

### Consultando Capacidades de Cámara

Una vez descubiertas, puedes conectarte a una cámara y consultar sus capacidades:

```cs
using VisioForge.Core.ONVIFX;

var onvifClient = new ONVIFClientX();
var result = await onvifClient.ConnectAsync(cameraUrl, username, password);

if (result)
{
    // Obtener información del dispositivo
    var deviceInfo = onvifClient.DeviceInformation;
    Console.WriteLine($"Cámara: {deviceInfo?.Model}, S/N: {deviceInfo?.SerialNumber}");
    
    // Obtener perfiles disponibles
    var profiles = await onvifClient.GetProfilesAsync();
    if (profiles != null)
    {
        foreach (var profile in profiles)
        {
            var mediaUri = await onvifClient.GetStreamUriAsync(profile);
            if (mediaUri != null)
            {
                Console.WriteLine($"Perfil: {profile.Name}, URI: {mediaUri.Uri}");
            }
        }
    }
}
```

## Conectando a Cámaras ONVIF

### Conexión Básica

```cs
using VisioForge.Core.ONVIFX;

// Conectar a cámara ONVIF
var onvifClient = new ONVIFClientX();
var connected = await onvifClient.ConnectAsync(
    "http://192.168.1.100:80/onvif/device_service", 
    "admin", 
    "password");

if (connected)
{
    Console.WriteLine("Conectado exitosamente a la cámara");
}
else
{
    Console.WriteLine("Conexión fallida");
}
```

## Trabajando con Perfiles de Medios

Las cámaras ONVIF típicamente proporcionan múltiples perfiles de medios con diferentes resoluciones, codecs y tasas de cuadros.

```cs
// Obtener todos los perfiles disponibles
var profiles = await onvifClient.GetProfilesAsync();

if (profiles != null && profiles.Length > 0)
{
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Perfil: {profile.Name}");
        Console.WriteLine($"Token: {profile.token}");
        
        // Obtener URI de stream para este perfil
        var mediaUri = await onvifClient.GetStreamUriAsync(profile);
        if (mediaUri != null)
        {
            Console.WriteLine($"  URI de Stream: {mediaUri.Uri}");
        }
    }
    
    // Usar el primer perfil
    var selectedProfile = profiles[0];
    var streamUri = await onvifClient.GetStreamUriAsync(selectedProfile);
}
```

## Vista Previa de Video

### Configuración Básica de Vista Previa

=== "Media Blocks SDK"

    
    ```cs
    using VisioForge.Core.MediaBlocks;
    using VisioForge.Core.MediaBlocks.Sources;
    using VisioForge.Core.MediaBlocks.VideoRendering;
    using VisioForge.Core.MediaBlocks.AudioRendering;
    using VisioForge.Core.Types.X.Sources;
    
    // Crear pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Obtener URL RTSP del perfil ONVIF
    var streamUri = await onvifClient.GetStreamUriAsync(profile);
    
    // Crear fuente RTSP
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true); // habilitar audio
    
    var rtspSource = new RTSPSourceBlock(rtspSettings);
    
    // Crear renderizador de video
    var videoRenderer = new VideoRendererBlock(pipeline, videoView);
    
    // Conectar bloques
    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    
    // Opcional: Agregar renderizado de audio
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);
    
    // Iniciar vista previa
    await pipeline.StartAsync();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    using VisioForge.Core.Types.X.Sources;
    using VisioForge.Core.VideoCaptureX;
    
    // Crear motor de captura de video
    var videoCapture = new VideoCaptureCoreX(videoView);
    
    // Obtener URI de stream de ONVIF
    var streamUri = await onvifClient.GetStreamUriAsync(profile);
    
    // Crear configuración de fuente RTSP
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true); // habilitar audio
    
    // Establecer fuente de video
    videoCapture.Video_Source = rtspSettings;
    
    // Configurar audio
    videoCapture.Audio_Record = true;
    videoCapture.Audio_Play = true;
    
    // Iniciar vista previa
    await videoCapture.StartAsync();
    ```
    


### Vista Previa de Baja Latencia

Para aplicaciones de vigilancia y monitoreo en tiempo real, habilita el modo de baja latencia:

=== "Media Blocks SDK"

    
    ```cs
    // Crear fuente RTSP con baja latencia
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true);
    
    // Habilitar modo de baja latencia (60-120ms latencia total)
    rtspSettings.LowLatencyMode = true;
    
    var rtspSource = new RTSPSourceBlock(rtspSettings);
    var videoRenderer = new VideoRendererBlock(pipeline, videoView);
    
    // Deshabilitar sync para latencia aún menor
    videoRenderer.IsSync = false;
    
    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    await pipeline.StartAsync();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    // Crear fuente RTSP
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true);
    
    // Habilitar modo de baja latencia
    rtspSettings.LowLatencyMode = true;
    
    videoCapture.Video_Source = rtspSettings;
    videoCapture.Audio_Record = true;
    videoCapture.Audio_Play = true;
    
    await videoCapture.StartAsync();
    ```
    


## Control PTZ (Pan-Tilt-Zoom)

### Operaciones PTZ Básicas

```cs
using VisioForge.Core.ONVIFX;
using VisioForge.Core.ONVIFX.PTZ;

// Conectar a cámara
var onvifClient = new ONVIFClientX();
await onvifClient.ConnectAsync(cameraUrl, username, password);

// Obtener token de perfil
var profiles = await onvifClient.GetProfilesAsync();
var profileToken = profiles[0].token;

// Panear a la derecha
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = 0.5f, y = 0 }, 
    null);

// Panear a la izquierda
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = -0.5f, y = 0 }, 
    null);

// Inclinar hacia arriba
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = 0, y = 0.5f }, 
    null);

// Inclinar hacia abajo
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = 0, y = -0.5f }, 
    null);

// Zoom in
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    null, 
    new Vector1D { x = 0.5f });

// Zoom out
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    null, 
    new Vector1D { x = -0.5f });

// Detener movimiento
await onvifClient.StopAsync(profileToken, true, true);
```

### Preajustes PTZ

```cs
// Obtener preajustes disponibles
var presets = await onvifClient.GetPresetsAsync(profileToken);

foreach (var preset in presets)
{
    Console.WriteLine($"Preajuste: {preset.Name}, Token: {preset.token}");
}

// Ir a preajuste (posición home)
if (presets != null && presets.Length > 0)
{
    await onvifClient.GoToPresetAsync(
        profileToken, 
        presets[0].token, 
        1.0f,  // velocidad pan/tilt
        1.0f,  // velocidad zoom
        1.0f); // tiempo
}

// Establecer posición actual como preajuste
await onvifClient.SetPresetAsync(profileToken, "MiPreajuste");
```

### Posicionamiento Absoluto

```cs
// Mover a posición absoluta
await onvifClient.AbsoluteMoveAsync(
    profileToken,
    new PTZVector
    {
        PanTilt = new Vector2D { x = 0.5f, y = 0.3f },
        Zoom = new Vector1D { x = 0.7f }
    },
    1.0f); // velocidad
```

## Acciones y Capacidades de Cámara

### Consultar Capacidades de Cámara

```cs
using VisioForge.Core.ONVIFX;

var onvifClient = new ONVIFClientX();
await onvifClient.ConnectAsync(cameraUrl, username, password);

// Obtener información del dispositivo
var deviceInfo = onvifClient.DeviceInformation;
Console.WriteLine($"Fabricante: {deviceInfo?.Manufacturer}");
Console.WriteLine($"Modelo: {deviceInfo?.Model}");
Console.WriteLine($"Firmware: {deviceInfo?.FirmwareVersion}");
Console.WriteLine($"Número de Serie: {deviceInfo?.SerialNumber}");
Console.WriteLine($"ID de Hardware: {deviceInfo?.HardwareId}");

// Obtener capacidades de servicio
var capabilities = await onvifClient.GetCapabilitiesAsync();

// Verificar soporte PTZ
if (capabilities?.PTZ != null)
{
    Console.WriteLine("PTZ soportado");
}

// Verificar analytics
if (capabilities?.Analytics != null)
{
    Console.WriteLine("Analytics soportado");
}

// Verificar eventos
if (capabilities?.Events != null)
{
    Console.WriteLine("Eventos soportados");
}
```

### Reiniciar Cámara

```cs
await onvifClient.SystemRebootAsync();
```

### Obtener Fecha y Hora del Sistema

```cs
var dateTime = await onvifClient.GetSystemDateAndTimeAsync();
Console.WriteLine($"Hora de la cámara: {dateTime}");
```

## Convertir Cámara Local en Fuente ONVIF

Convierte tu cámara USB/webcam local en un stream IP que los clientes ONVIF puedan consumir. El demo de consola `RTSP Webcam Server` incluido con el Media Blocks SDK expone una cámara DirectShow como un endpoint RTSP que los grabadores ONVIF y software VMS pueden ingerir.

=== "Media Blocks SDK"

    
    ```cs
    using System;
    using System.Threading;
    using VisioForge.Core;
    using VisioForge.Core.MediaBlocks;
    using VisioForge.Core.MediaBlocks.Sinks;
    using VisioForge.Core.MediaBlocks.Sources;
    using VisioForge.Core.MediaBlocks.VideoEncoders;
    using VisioForge.Core.Types.X.Output;
    using VisioForge.Core.Types.X.Sources;
    using VisioForge.Core.Types.X.VideoCapture;
    
    Console.WriteLine("Inicializando VisioForge SDK.");
    VisioForgeX.InitSDK();
    
    var cameras = DeviceEnumerator.Shared.VideoSources();
    Console.WriteLine("Selecciona la webcam");
    for (int i = 0; i < cameras.Length; i++)
    {
        Console.WriteLine($"{i + 1}: {cameras[i].DisplayName}");
    }
    
    Console.Write("Ingresa el número de la cámara: ");
    VideoCaptureDeviceInfo cameraInfo = null;
    if (int.TryParse(Console.ReadLine(), out int cameraIndex) && cameraIndex > 0 && cameraIndex <= cameras.Length)
    {
        cameraInfo = cameras[cameraIndex - 1];
        Console.WriteLine($"Cámara seleccionada: {cameraInfo.DisplayName}");
    }
    else
    {
        Console.WriteLine("Selección inválida. Saliendo.");
    
        VisioForgeX.DestroySDK();
        return;
    }
    
    var pipeline = new MediaBlocksPipeline();
    
    var videoSourceSettings = new VideoCaptureDeviceSourceSettings(cameraInfo);
    videoSourceSettings.Format = cameraInfo.GetHDVideoFormatAndFrameRate(out var frameRate).ToFormat();
    videoSourceSettings.Format.FrameRate = frameRate;
    
    var cameraSource = new SystemVideoSourceBlock(videoSourceSettings);
    
    var rtspServerSettings = new RTSPServerSettings(H264EncoderBlock.GetDefaultSettings(), null)
    {
        Port = 8554,
    };
    
    var rtspBlock = new RTSPServerBlock(rtspServerSettings);
    
    Console.WriteLine("URL del Servidor RTSP: " + rtspBlock.Settings.URL);
    
    pipeline.Connect(cameraSource.Output, rtspBlock.Input);
    
    Console.WriteLine("Iniciando el pipeline...");
    
    new Thread(() =>
    {
        pipeline.Start();
    }).Start();
    
    Console.WriteLine("Pipeline iniciado. Presiona cualquier tecla para detener el servidor y salir.");
    Console.ReadKey();
    
    Console.WriteLine("Deteniendo el pipeline...");
    
    pipeline.Stop();
    
    Console.WriteLine("Pipeline detenido.");
    
    pipeline.Dispose();
    
    VisioForgeX.DestroySDK();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    // La funcionalidad de servidor ONVIF/RTSP es proporcionada por el Media Blocks SDK.
    ```
    


## Mejores Prácticas

### Gestión de Conexión

1. **Siempre desecha clientes ONVIF apropiadamente:**
   ```cs
   using (var onvifClient = new ONVIFClientX())
   {
       await onvifClient.ConnectAsync(url, username, password);
       // ... usar cliente ...
   } // Automáticamente desechado
   ```

2. **Maneja fallos de conexión con elegancia:**
   ```cs
   var maxRetries = 3;
   var retryCount = 0;
   
   while (retryCount < maxRetries)
   {
       try
       {
           var connected = await onvifClient.ConnectAsync(url, user, pass);
           if (connected)
               break;
       }
       catch (Exception ex)
       {
           Console.WriteLine($"Intento de conexión {retryCount + 1} fallido: {ex.Message}");
       }
       
       retryCount++;
       await Task.Delay(2000); // Esperar antes de reintentar
   }
   ```

3. **Usa tokens de cancelación para descubrimiento:**
   ```cs
   var cts = new CancellationTokenSource();
   cts.CancelAfter(TimeSpan.FromSeconds(10));
   
   await _onvifDiscovery.Discover(10, callback, cts.Token);
   ```

### Optimización de Rendimiento

1. **Usa RTSPRAWSourceBlock para grabación sin re-codificación** - reduce significativamente el uso de CPU
2. **Habilita modo de baja latencia solo cuando sea necesario** - intercambia estabilidad por velocidad
3. **Limita streams concurrentes** basado en tus capacidades de hardware
4. **Usa decodificadores de hardware** cuando estén disponibles:
   ```cs
   rtspSettings.UseGPUDecoder = true;
   ```

### Consideraciones de Red

1. **Usa TCP para conexiones confiables:**
   ```cs
   rtspSettings.Transport = RTSPTransport.TCP;
   ```

2. **Configura timeouts apropiados:**
   ```cs
   rtspSettings.Timeout = TimeSpan.FromSeconds(30);
   ```

3. **Monitorea desconexiones:**
   ```cs
   pipeline.OnError += (sender, e) =>
   {
       if (e.Message.Contains("disconnect"))
       {
           // Intentar reconexión
       }
   };
   ```

### Seguridad

1. **Nunca codifiques credenciales** - usa archivos de configuración o almacenamiento seguro
2. **Usa HTTPS para streaming web** cuando sea posible
3. **Implementa autenticación** para endpoints de streaming
4. **Valida entrada de usuario** cuando construyas URLs
5. **Mantén credenciales en memoria por tiempo mínimo**

### Manejo de Errores

1. **Registra todos los errores para diagnóstico:**
   ```cs
   pipeline.OnError += (sender, e) =>
   {
       Logger.Error($"Error de pipeline: {e.Message}");
   };
   ```

2. **Habilita modo debug durante desarrollo:**
   ```cs
   pipeline.Debug_Mode = true;
   pipeline.Debug_Dir = @"C:\Logs\VisioForge";
   ```

3. **Maneja interrupciones de stream:**
   ```cs
   // Implementa lógica de reconexión automática
   var reconnectAttempts = 0;
   const int maxReconnects = 5;
   
   pipeline.OnError += async (sender, e) =>
   {
       if (reconnectAttempts < maxReconnects)
       {
           reconnectAttempts++;
           await Task.Delay(5000);
           await pipeline.StopAsync();
           await pipeline.StartAsync();
       }
   };
   ```

## Solución de Problemas

### Problemas Comunes

#### "No se puede conectar a cámara ONVIF"

**Causas posibles:**
- Formato de URL incorrecto (debe ser: `http://IP:PUERTO/onvif/device_service`)
- Credenciales incorrectas
- Firewall de red bloqueando conexión
- Servicio ONVIF de la cámara deshabilitado

**Soluciones:**
```cs
// Prueba diferentes formatos de URL
var urls = new[]
{
    "http://192.168.1.100:80/onvif/device_service",
    "http://192.168.1.100:8080/onvif/device_service",
    "http://192.168.1.100/onvif/device_service"
};

foreach (var url in urls)
{
    if (await onvifClient.ConnectAsync(url, user, pass))
    {
        Console.WriteLine($"Conectado usando: {url}");
        break;
    }
}
```

#### "No se descubrieron cámaras"

**Causas posibles:**
- Cámaras en subred diferente
- Multicast bloqueado por red
- Firewall bloqueando WS-Discovery

**Soluciones:**
1. Verifica configuración de red
2. Prueba conexión directa con IP conocido
3. Verifica que el soporte ONVIF de la cámara esté habilitado
4. Incrementa timeout de descubrimiento

#### "El stream no se reproduce"

**Causas posibles:**
- Codec no soportado
- Ancho de banda de red insuficiente
- URL de stream incorrecta

**Soluciones:**
```cs
// Obtén información del stream antes de reproducir
var info = rtspSettings.GetInfo();
if (info == null)
{
    Console.WriteLine("No se puede obtener información del stream - verifica URL");
    return;
}

Console.WriteLine($"Codec de video: {info.VideoStreams[0].CodecName}");
Console.WriteLine($"Resolución: {info.VideoStreams[0].Width}x{info.VideoStreams[0].Height}");
Console.WriteLine($"Bitrate: {info.VideoStreams[0].Bitrate}");
```

#### "Alto uso de CPU durante grabación"

**Causas posibles:**
- Re-codificación cuando no es necesaria
- Demasiados streams concurrentes
- Decodificación por software en lugar de hardware

**Soluciones:**
1. Usa `RTSPRAWSourceBlock` para grabación sin re-codificación
2. Habilita decodificador de hardware:
   ```cs
   rtspSettings.UseGPUDecoder = true;
   ```
3. Limita streams concurrentes
4. Reduce resolución/bitrate en la cámara

#### "Comandos PTZ no funcionan"

**Causas posibles:**
- La cámara no soporta PTZ
- Perfil incorrecto seleccionado
- Servicio PTZ no habilitado

**Soluciones:**
```cs
// Verifica capacidades PTZ
var capabilities = await onvifClient.GetCapabilitiesAsync();
if (capabilities?.PTZ != null)
{
    Console.WriteLine("PTZ está soportado");
    
    // Obtén configuración PTZ
    var profiles = await onvifClient.GetProfilesAsync();
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Perfil: {profile.Name}, Token: {profile.token}");
    }
}
else
{
    Console.WriteLine("PTZ no está soportado por esta cámara");
}
```

### Herramientas de Diagnóstico

#### Habilitar Registro de Depuración

=== "Media Blocks SDK"

    
    ```cs
    pipeline.Debug_Mode = true;
    pipeline.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "VisioForge", 
        "Logs");
    ```
    

=== "Video Capture SDK"

    
    ```cs
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "VisioForge", 
        "Logs");
    ```
    


#### Probar Conexión RTSP

```cs
// Prueba si la URL RTSP es válida
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), 
    username, 
    password, 
    true);

var info = rtspSettings.GetInfo();
if (info != null)
{
    Console.WriteLine("✓ Conexión RTSP exitosa");
    Console.WriteLine($"  Streams de video: {info.VideoStreams.Count}");
    Console.WriteLine($"  Streams de audio: {info.AudioStreams.Count}");
    
    foreach (var stream in info.VideoStreams)
    {
        Console.WriteLine($"  Video: {stream.CodecName} {stream.Width}x{stream.Height}");
    }
}
else
{
    Console.WriteLine("✗ Conexión RTSP fallida");
}
```

### Obtener Ayuda

- **Código de Muestra**: [Repositorio de Muestras en GitHub](https://github.com/visioforge/.Net-SDK-s-samples)
- **Documentación**: [Documentación de VisioForge](https://www.visioforge.com/help/)
- **Foro de Soporte**: [Soporte de VisioForge](https://support.visioforge.com)

### Demos Relacionados

- [Demo RTSP MultiView](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) - Grabación de múltiples cámaras sin re-codificación
- [Demo RTSP Preview](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) - Vista previa y grabación con postprocesamiento
- [Demo IP Capture](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) - Integración completa de cámara IP con control PTZ