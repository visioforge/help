---
title: Integración USB3 Vision, GigE & GenICam - Guía
description: Integra cámaras USB3 Vision, GigE y GenICam con drivers DirectShow. Soporte machine vision multiplataforma con .NET SDK y alta velocidad.
sidebar_label: Cámaras USB3 Vision, GigE y GenICam
order: 15
---

# Integración de Cámaras USB3 Vision, GigE y GenICam

[SDK de Captura de Video .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button }

## Resumen

Las cámaras industriales usando estándares USB3 Vision, GigE Vision y GenICam proporcionan calidad de imagen superior y rendimiento para aplicaciones de machine vision. Nuestro SDK permite integración perfecta con estos tipos de cámara profesionales a través de varias opciones de conectividad.

## Protocolo GigE Vision

GigE Vision es una interfaz de cámara industrial estándar basada en tecnología Gigabit Ethernet. Ofrece varias ventajas para aplicaciones de machine vision:

- **Transferencia de datos de alta velocidad**: Soporta hasta 1 Gbps en redes Gigabit estándar y 10+ Gbps en redes modernas 10GigE
- **Longitud de cable larga**: Puede operar a distancias de hasta 100 metros usando cableado Ethernet estándar
- **Arquitectura de red**: Múltiples cámaras pueden compartir la misma infraestructura de red
- **Power over Ethernet (PoE)**: Las cámaras pueden recibir alimentación a través del mismo cable Ethernet (cuando se usan switches PoE-enabled)
- **Descubrimiento de dispositivo**: Detección automática de cámaras GigE Vision en la red
- **Capacidades multicast**: Permite streaming a múltiples clientes simultáneamente

GigE Vision combina la interfaz de programación GenICam con la capa de transporte GigE, proporcionando estructuras de comando consistentes en diferentes cámaras de fabricantes.

## Protocolo USB3 Vision

USB3 Vision es un estándar de interfaz de cámara que aprovecha la interfaz USB 3.0 de alta velocidad para aplicaciones de imaging industrial:

- **Ancho de banda alto**: Hasta 5 Gbps de tasa de transferencia teórica, habilitando alta resolución y tasas de frames
- **Plug-and-play**: Conectividad simple sin necesidad de tarjetas de interfaz especializadas
- **Hot-swappable**: Los dispositivos pueden conectarse o desconectarse sin reinicio del sistema
- **Longitud de cable**: Típicamente soporta distancias de hasta 5 metros (puede extenderse con cables activos)
- **Entrega de alimentación**: Hasta 4.5W proporcionados directamente a través de la conexión USB
- **Arquitectura de driver estándar**: Usa drivers USB estándar de sistemas operativos

USB3 Vision funciona junto con el estándar GenICam para proporcionar control consistente de cámara entre diferentes fabricantes.

## Soporte de Protocolo GenTL (Generic Transport Layer)

VisioForge proporciona soporte completo para el estándar **GenICam GenTL (Generic Transport Layer)**, que es un componente clave de sistemas de machine vision industrial. GenTL define una interfaz estandarizada para acceder a cámaras a través de varios protocolos de transporte mientras mantiene compatibilidad neutral al fabricante.

### ¿Qué es GenTL?

GenTL (Generic Transport Layer) es una especificación de interfaz estandarizada que proporciona:

- **Acceso transport-agnostic**: API unificada para cámaras independientemente de la capa de transporte físico (GigE, USB3, CoaXPress, Camera Link, etc.)
- **Neutralidad al fabricante**: Interfaz consistente entre diferentes fabricantes de cámaras
- **Arquitectura modular**: Separa implementaciones específicas de transporte de la lógica de aplicación
- **Modelo Producer/Consumer**: GenTL Producers manejan especificidades de transporte, mientras que GenTL Consumers (aplicaciones) usan interfaces estandarizadas

### Implementación GenTL de VisioForge

Nuestro SDK incluye soporte GenTL completo a través de:

#### 1. **Detección Automática de Protocolo**

El sistema detecta automáticamente cuando una cámara está conectada vía GenTL y establece el protocolo en consecuencia.

#### 2. **Configuración de Entorno GenTL**

Soporte para variables de entorno GenTL estándar:

- **GENICAM_GENTL64_PATH**: Ruta a bibliotecas producer GenTL (64-bit)
- Descubrimiento automático de producers GenTL instalados

#### 3. **Manejo de Errores Completo**

Soporte completo para códigos de error específicos de GenTL incluyendo:

- Errores de inicialización de sistema
- Problemas de comunicación de capa de transporte
- Acceso y gestión de recursos de dispositivo
- Errores de buffer y streaming
- Condiciones de timeout y abort

#### 4. **Características Avanzadas**

- **Enumeración de dispositivo**: Descubrimiento de dispositivos compatibles con GenTL en todas las capas de transporte disponibles
- **Gestión de stream**: Streaming de alto rendimiento con gestión de buffer GenTL
- **Acceso a características**: Acceso completo al árbol de características GenICam a través de interfaz GenTL
- **Soporte multi-transporte**: Acceso simultáneo a cámaras en diferentes capas de transporte

### Compatibilidad de Producer GenTL

La implementación GenTL de VisioForge es compatible con producers de fabricantes principales:

- **Camera Link**: Interfaces de frame grabber de alta velocidad
- **CoaXPress**: Conexiones de largo alcance, alto ancho de banda
- **10 GigE**: Conexiones Ethernet de ultra-alta velocidad
- **Capas de transporte personalizadas**: Implementaciones de transporte específicas del fabricante
- **Sistemas multi-interfaz**: Entornos de transporte mixto

### Beneficios de Integración

Usar GenTL con VisioForge proporciona varias ventajas:

1. **Arquitectura future-proof**: Soporte para nuevas capas de transporte sin cambios en aplicación
2. **Desarrollo simplificado**: Una sola API para todos los tipos de transporte soportados
3. **Rendimiento mejorado**: Implementaciones específicas de transporte optimizadas
4. **Soporte de cámara más amplio**: Acceso a cámaras no disponibles a través de interfaces nativas
5. **Características profesionales**: Triggering avanzado, sincronización y capacidades de control

### Requisitos de Configuración

Para usar cámaras GenTL con VisioForge:

1. Instalar el SDK apropiado del fabricante de la cámara
2. Establecer la variable de entorno `GENICAM_GENTL64_PATH` para apuntar a la biblioteca producer
3. Asegurar que las cámaras estén conectadas y reconocidas apropiadamente por el producer GenTL
4. Usar métodos estándar de enumeración GenICam de VisioForge para descubrir dispositivos GenTL

El sistema maneja automáticamente inicialización GenTL, descubrimiento de dispositivo y gestión de capa de transporte.

## Soporte de Driver DirectShow

La mayoría de fabricantes de cámaras industriales incluyen drivers compatibles con DirectShow con sus kits de desarrollo. Estos drivers crean un puente entre la interfaz nativa de la cámara y el framework DirectShow, permitiendo que nuestro SDK acceda y controle estos dispositivos especializados.

Beneficios clave:

- Ruta de integración simplificada
- Acceso completo a streams de cámara
- Compatibilidad con flujos de trabajo DirectShow existentes

## Soporte GenICam Cross-Platform

Para desarrolladores trabajando en entornos multi-plataforma, el engine cross-platform de nuestro SDK soporta cámaras implementando el estándar de interfaz unificada GenICam. Esto proporciona acceso consistente a características de cámara entre diferentes sistemas operativos.

## Prerrequisitos

### macOS

Instalar el paquete `Aravis` usando Homebrew:

```bash
brew install aravis
```

### Linux

Instalar el paquete `Aravis` usando el gestor de paquetes:

```bash
sudo apt-get install libaravis-0.8-dev
```

### Windows

Instalar el paquete `VisioForge.CrossPlatform.GenICam.Windows.x64` en su proyecto usando NuGet.

#### Instalación de Driver USB en Windows

Por defecto en Windows, las cámaras USB3 Vision pueden no tener el driver USB apropiado instalado, lo que puede prevenir que aparezcan en listas de enumeración de dispositivos. Este es un problema común con cámaras industriales USB que requieren soporte de driver específico.

#### Soluciones de Instalación de Driver

##### Opción 1: Instalación de Driver USB Genérico con Zadig

Para cámaras sin drivers específicos del fabricante, puede instalar drivers USB genéricos usando [Zadig](https://zadig.akeo.ie/), una aplicación Windows que simplifica la instalación de drivers USB:

1. **Descargar y ejecutar Zadig** desde [https://zadig.akeo.ie/](https://zadig.akeo.ie/)
2. **Seleccionar el dispositivo de cámara USB3 Vision** de la lista de dispositivos en Zadig
3. **Elegir el driver apropiado**:
   - **WinUSB**: Recomendado para la mayoría de aplicaciones GenICam
   - **libusb-win32**: Para aplicaciones basadas en libusb legacy
   - **libusbK**: Driver USB de alto rendimiento alternativo
4. **Instalar el driver** haciendo clic en "Install Driver" o "Replace Driver"

Después de la instalación, la cámara debería aparecer en la enumeración de VisioForge y ser accesible a través de la interfaz GenICam.

##### Opción 2: SDK de Fabricante con Bridge GenTL

Si tiene un SDK de cámara del fabricante, la cámara puede conectarse usando el enfoque **bridge GenTL**:

1. **Instalar el SDK del fabricante** (ej. pylon de Basler, Spinnaker de FLIR)
2. **Configurar el entorno GenTL** configurando la variable de entorno `GENICAM_GENTL64_PATH`
3. **Usar el producer GenTL** proporcionado por el SDK del fabricante
4. **Acceder a la cámara** a través del soporte GenTL unificado de VisioForge

Este enfoque proporciona acceso a características específicas del fabricante y optimizaciones mientras mantiene compatibilidad con la interfaz GenICam unificada de VisioForge.

## SDKs Compatibles de Fabricantes Principales

Los siguientes SDKs de fabricante se conocen por trabajar bien con nuestra integración:

- [SDK pylon de Basler](https://www.baslerweb.com/en/software/pylon/sdk/) - Toolkit completo para cámaras Basler
- [SDK Spinnaker de FLIR/Teledyne](https://www.teledynevisionsolutions.com/) - Solución avanzada de imaging para cámaras FLIR y Teledyne

## Ejemplos de Código

Los siguientes ejemplos demuestran implementación práctica de GenICam, USB3 Vision y cámaras GigE usando el SDK de Captura de Video de VisioForge con integración GenICam.

### Descubrimiento Básico de Cámara e Información

```csharp
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Inicializar el SDK
await VisioForgeX.InitSDKAsync();

// Descubrir cámaras GenICam disponibles
GenICamCameraManager.UpdateDeviceList();
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();

Console.WriteLine($"Encontradas {devices.Length} dispositivos GenICam");

foreach (var device in devices)
{
    Console.WriteLine($"Cámara: {device.CameraName}");
    Console.WriteLine($"ID de Dispositivo: {device.DeviceId}");
    Console.WriteLine($"Dirección: {device.Address}");
    Console.WriteLine();
}

// Obtener información detallada sobre una cámara específica
if (devices.Length > 0)
{
    var cameraDeviceId = devices[0].DeviceId;
    var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
    
    if (camera != null && GenICamCameraManager.OpenCamera(cameraDeviceId))
    {
        camera.ReadInfo();
        
        Console.WriteLine($"Conectada a: {camera.VendorName} {camera.ModelName}");
        Console.WriteLine($"Número de Serie: {camera.SerialNumber}");
        Console.WriteLine($"Protocolo: {camera.Protocol}");
        Console.WriteLine($"Tamaño de Sensor: {camera.SensorSize.Width}x{camera.SensorSize.Height}");
        Console.WriteLine($"Formatos de Píxel Disponibles: {string.Join(", ", camera.AvailablePixelFormats)}");
        
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
}
```

### Vista Previa en Vivo con VideoCaptureCoreX

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Inicializar SDK
await VisioForgeX.InitSDKAsync();

// Crear instancia VideoCaptureCoreX (asumiendo que tiene un control de vista de video)
var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);

try
{
    // Descubrir cámaras
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No se encontraron cámaras GenICam!");
        return;
    }

    var selectedDevice = devices[0]; // Usar primera cámara
    Console.WriteLine($"Usando cámara: {selectedDevice.CameraName}");

    // Configurar cámara antes de iniciar captura
    var camera = GenICamCameraManager.GetCamera(selectedDevice.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(selectedDevice.DeviceId))
    {
        camera.ReadInfo();
        
        // Configurar ajustes de cámara
        if (camera.ExposureTimeAvailable)
        {
            camera.SetExposureTime(10000); // 10ms exposición
        }
        
        if (camera.GainAvailable)
        {
            camera.SetGain(0.0); // Ganancia mínima
        }
        
        // Obtener resolución de cámara y tasa de frames
        var sensorSize = camera.GetSensorSize();
        var frameRate = camera.GetFrameRate();
        
        // Crear fuente GenICam
        var sourceSettings = new GenICamSourceSettings(
            selectedDevice.DeviceId,
            new VisioForge.Core.Types.Rect(0, 0, sensorSize.Width, sensorSize.Height),
            frameRate,
            GenICamPixelFormat.Default
        );
        
        videoCapture.Video_Source = sourceSettings;
        
        // Iniciar vista previa
        await videoCapture.StartAsync();
        
        Console.WriteLine("Vista previa en vivo iniciada. Presione cualquier tecla para detener...");
        Console.ReadKey();
        
        await videoCapture.StopAsync();
        GenICamCameraManager.CloseCamera(selectedDevice.DeviceId);
    }
}
finally
{
    await videoCapture.DisposeAsync();
}
```

### Grabación a Archivo MP4

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.IO;
using System.Threading.Tasks;

// Inicializar SDK
await VisioForgeX.InitSDKAsync();

// Crear instancia VideoCaptureCoreX
var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);

try
{
    // Configurar modo debug
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

    // Descubrir y seleccionar cámara
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No se encontraron cámaras GenICam!");
        return;
    }

    var selectedDevice = devices[0];
    Console.WriteLine($"Grabando desde cámara: {selectedDevice.CameraName}");

    // Configurar ajustes de cámara
    var camera = GenICamCameraManager.GetCamera(selectedDevice.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(selectedDevice.DeviceId))
    {
        camera.ReadInfo();
        
        // Establecer parámetros de cámara
        if (camera.ExposureTimeAvailable)
        {
            camera.SetExposureTime(5000); // 5ms exposición
        }
        
        if (camera.FrameRateAvailable)
        {
            var targetFps = Math.Min(30.0, camera.FrameRateBounds.Max);
            camera.SetFrameRate(new VideoFrameRate(targetFps));
        }

        // Obtener resolución de cámara y tasa de frames
        var sensorSize = camera.GetSensorSize();
        var frameRate = camera.GetFrameRate();
        
        // Crear fuente GenICam
        var sourceSettings = new GenICamSourceSettings(
            selectedDevice.DeviceId,
            new VisioForge.Core.Types.Rect(0, 0, sensorSize.Width, sensorSize.Height),
            frameRate,
            GenICamPixelFormat.Default
        );
        
        videoCapture.Video_Source = sourceSettings;
        
        // Configurar salida MP4
        string outputFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "genicam_capture.mp4");
        var mp4Output = new MP4Output(outputFilename, H264EncoderBlock.GetDefaultSettings(), null);
        videoCapture.Outputs_Add(mp4Output);
        
        // Iniciar grabación
        await videoCapture.StartAsync();
        
        Console.WriteLine($"Grabación iniciada a: {outputFilename}");
        Console.WriteLine("Presione cualquier tecla para detener la grabación...");
        Console.ReadKey();
        
        // Detener grabación
        await videoCapture.StopAsync();
        Console.WriteLine($"Grabación guardada a: {outputFilename}");
        
        GenICamCameraManager.CloseCamera(selectedDevice.DeviceId);
    }
}
finally
{
    await videoCapture.DisposeAsync();
}
```

### Configuración Avanzada de Cámara

```csharp
using VisioForge.Core.GenICam;
using VisioForge.Core.Types;
using System;
using System.Linq;
using System.Threading;

// Descubrir y conectar a cámara
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
if (devices.Length == 0) return;

var camera = GenICamCameraManager.GetCamera(devices[0].DeviceId);

if (camera != null && GenICamCameraManager.OpenCamera(devices[0].DeviceId))
{
    camera.ReadInfo();
    
    // Mostrar capacidades de cámara
    Console.WriteLine($"Cámara: {camera}");
    Console.WriteLine($"Formatos de píxel disponibles: {string.Join(", ", camera.AvailablePixelFormats)}");
    
    // Configurar formato de píxel
    if (camera.AvailablePixelFormats.Contains("Mono8"))
    {
        camera.SetPixelFormat("Mono8");
        Console.WriteLine("Formato de píxel establecido a Mono8");
    }
    
    // Configurar exposición con modo auto
    if (camera.IsExposureAutoAvailable)
    {
        // Primero intentar exposición auto
        camera.SetExposureAuto(GenICamAuto.Once);
        Thread.Sleep(1000); // Esperar a que la exposición auto se complete
        
        // Luego cambiar a manual y leer el valor calculado auto
        camera.SetExposureAuto(GenICamAuto.Off);
        var autoExposure = camera.GetExposureTime();
        Console.WriteLine($"Exposición auto calculada: {autoExposure:F2} μs");
        
        // Ajustar manualmente si es necesario
        camera.SetExposureTime(autoExposure * 1.2); // 20% más larga exposición
    }
    
    // Configurar ganancia
    if (camera.IsGainAutoAvailable)
    {
        camera.SetGainAuto(GenICamAuto.Continuous);
        Console.WriteLine("Ganancia auto continua habilitada");
    }
    
    // Configurar binning para tasas de frames más altas
    if (camera.BinningAvailable)
    {
        camera.SetBinning(2, 2); // Binning 2x2
        Console.WriteLine("Binning 2x2 establecido para mayor sensibilidad y tasa de frames");
    }
    
    // Configurar triggering por software
    if (camera.SoftwareTriggerSupported)
    {
        camera.SetStringFeature("TriggerMode", "On");
        camera.SetStringFeature("TriggerSource", "Software");
        camera.SetAcquisitionMode(GenICamAcquisitionMode.Continuous);
        
        Console.WriteLine("Configurado para triggering por software");
        
        // Nota: Cuando se usa con VideoCaptureCoreX, el triggering por software sería
        // integrado en el pipeline de captura en lugar de llamado directamente
    }
    
    // Leer y mostrar características avanzadas
    camera.ReadAvailableFeatures();
    Console.WriteLine($"La cámara tiene {camera.AvailableStringFeatures.Length + camera.AvailableIntegerFeatures.Length + camera.AvailableFloatFeatures.Length + camera.AvailableBooleanFeatures.Length} características");
    Console.WriteLine($"Características avanzadas disponibles: {camera.HasAdvancedFeatures}");
    
    GenICamCameraManager.CloseCamera(devices[0].DeviceId);
}
```

### Usando GenICamSourceBlock con Pipeline de Media Blocks

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.IO;
using System.Threading.Tasks;

// Inicializar SDK
await VisioForgeX.InitSDKAsync();

// Crear Pipeline de Media Blocks
var pipeline = new MediaBlocksPipeline();

try
{
    // Configurar modo debug
    pipeline.Debug_Mode = true;
    pipeline.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

    // Descubrir cámaras
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No se encontraron cámaras GenICam!");
        return;
    }

    var selectedDevice = devices[0];
    string cameraDeviceId = selectedDevice.DeviceId;

    // Configurar cámara
    if (GenICamCameraManager.OpenCamera(cameraDeviceId))
    {
        var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
        camera?.ReadInfo();

        // Crear bloque fuente GenICam
        var sourceSettings = new GenICamSourceSettings(cameraDeviceId);
        var sourceBlock = new GenICamSourceBlock(sourceSettings);

        // Crear renderizador de video para vista previa
        var videoRenderer = new VideoRendererBlock(pipeline, yourVideoViewControl) { IsSync = false };

        // Crear bloque tee para dividir el stream
        var videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);

        // Crear bloque de salida MP4
        string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "genicam_capture.mp4");
        var mp4Output = new MP4OutputBlock(new MP4SinkSettings(outputFile), H264EncoderBlock.GetDefaultSettings(), aacSettings: null);

        // Conectar el pipeline
        pipeline.Connect(sourceBlock.Output, videoTee.Input);
        pipeline.Connect(videoTee.Outputs[0], videoRenderer.Input);
        
        var videoInput = mp4Output.CreateNewInput(MediaBlockPadMediaType.Video);
        pipeline.Connect(videoTee.Outputs[1], videoInput);

        // Iniciar el pipeline
        await pipeline.StartAsync();

        Console.WriteLine($"Grabando a: {outputFile}");
        Console.WriteLine("Presione cualquier tecla para detener...");
        Console.ReadKey();

        // Detener el pipeline
        await pipeline.StopAsync();
        Console.WriteLine($"Grabación guardada a: {outputFile}");

        // Limpiar
        mp4Output.Dispose();
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
}
finally
{
    await pipeline.DisposeAsync();
}
```

### Manejo de Errores y Recuperación

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

// Inicializar SDK
await VisioForgeX.InitSDKAsync();

string cameraDeviceId = null;
VideoCaptureCoreX videoCapture = null;

try
{
    // Descubrir cámaras con lógica de reintento
    int maxDiscoveryRetries = 3;
    var devices = new GenICamSourceInfo[0];
    
    for (int attempt = 1; attempt <= maxDiscoveryRetries; attempt++)
    {
        try
        {
            GenICamCameraManager.UpdateDeviceList();
            devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
            
            if (devices.Length > 0)
            {
                Console.WriteLine($"Cámaras encontradas en intento {attempt}");
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Intento de descubrimiento {attempt} falló: {ex.Message}");
            if (attempt < maxDiscoveryRetries)
            {
                Thread.Sleep(2000); // Esperar antes de reintentar
            }
        }
    }
    
    if (devices.Length == 0)
    {
        Console.WriteLine("No se encontraron cámaras después de todos los intentos");
        return;
    }
    
    cameraDeviceId = devices[0].DeviceId;
    
    // Conexión de cámara con lógica de reintento
    int maxRetries = 3;
    bool connected = false;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            connected = GenICamCameraManager.OpenCamera(cameraDeviceId);
            if (connected)
            {
                Console.WriteLine($"Conectado a cámara en intento {attempt}");
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Intento de conexión {attempt} falló: {ex.Message}");
            if (attempt < maxRetries)
            {
                Thread.Sleep(2000); // Esperar antes de reintentar
            }
        }
    }
    
    if (!connected)
    {
        Console.WriteLine("Falló al conectar después de todos los intentos");
        return;
    }
    
    // Configurar cámara
    var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
    camera?.ReadInfo();
    
    // Crear VideoCaptureCoreX con manejo de errores
    videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);
    
    // Establecer manejador de eventos de error
    videoCapture.OnError += (sender, e) =>
    {
        Console.WriteLine($"Error de captura: {e.Message}");
    };
    
    // Configurar fuente
    var sourceSettings = new GenICamSourceSettings(
        cameraDeviceId,
        new VisioForge.Core.Types.Rect(0, 0, camera.SensorSize.Width, camera.SensorSize.Height),
        camera.GetFrameRate(),
        GenICamPixelFormat.Default
    );
    
    videoCapture.Video_Source = sourceSettings;
    
    // Iniciar captura con monitoreo
    await videoCapture.StartAsync();
    
    Console.WriteLine("Captura iniciada. Monitoreando errores...");
    
    // Monitorear por 30 segundos
    var startTime = DateTime.Now;
    while ((DateTime.Now - startTime).TotalSeconds < 30)
    {
        Thread.Sleep(1000);
        
        // Verificar estado de captura
        if (videoCapture.State != VisioForge.Core.Types.PlaybackState.Play)
        {
            Console.WriteLine("Captura se detuvo inesperadamente. Intentando reinicio...");
            
            try
            {
                await videoCapture.StopAsync();
                await Task.Delay(1000);
                await videoCapture.StartAsync();
                Console.WriteLine("Captura reiniciada exitosamente");
            }
            catch (Exception restartEx)
            {
                Console.WriteLine($"Falló al reiniciar captura: {restartEx.Message}");
                break;
            }
        }
    }
    
    await videoCapture.StopAsync();
    Console.WriteLine("Monitoreo de captura completado");
}
catch (Exception ex)
{
    Console.WriteLine($"Error inesperado: {ex.Message}");
}
finally
{
    // Limpiar
    if (videoCapture != null)
    {
        await videoCapture.DisposeAsync();
    }
    
    if (!string.IsNullOrEmpty(cameraDeviceId))
    {
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
    
    Console.WriteLine("Recursos limpiados");
}
```

### Trabajando con Cámaras GenTL

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Para cámaras GenTL, asegurar que la variable de entorno esté configurada
// GENICAM_GENTL64_PATH debería apuntar a la biblioteca producer GenTL

// Ejemplo: Establecer en el inicio de su aplicación o entorno
Environment.SetEnvironmentVariable("GENICAM_GENTL64_PATH", @"C:\Program Files\Basler\pylon 7\Runtime\x64");

// Inicializar SDK
await VisioForgeX.InitSDKAsync();

// Descubrir cámaras GenTL (aparecerán junto con otras GenICam devices)
GenICamCameraManager.UpdateDeviceList();
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();

foreach (var device in devices)
{
    // Verificar información de cámara
    var camera = GenICamCameraManager.GetCamera(device.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(device.DeviceId))
    {
        camera.ReadInfo();
        
        // Verificar si esta es una device GenTL
        if (camera.Protocol == "GenTL")
        {
            Console.WriteLine($"Encontrada cámara GenTL: {camera}");
            
            try
            {
                // Configurar características específicas de GenTL para rendimiento máximo
                if (camera.IsFeatureAvailable("StreamBufferCountMode"))
                {
                    camera.SetStringFeature("StreamBufferCountMode", "Manual");
                }
                
                if (camera.IsFeatureAvailable("StreamBufferCountManual"))
                {
                    camera.SetIntegerFeature("StreamBufferCountManual", 20); // Más buffers
                }
                
                // Establecer parámetros de adquisición
                if (camera.ExposureTimeAvailable)
                {
                    camera.SetExposureTime(1000); // 1ms exposición
                }
                
                // Usar con VideoCaptureCoreX
                var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);
                
                try
                {
                    var sourceSettings = new GenICamSourceSettings(
                        device.DeviceId,
                        new VisioForge.Core.Types.Rect(0, 0, camera.SensorSize.Width, camera.SensorSize.Height),
                        camera.GetFrameRate(),
                        GenICamPixelFormat.Default
                    );
                    
                    videoCapture.Video_Source = sourceSettings;
                    
                    // Iniciar vista previa
                    await videoCapture.StartAsync();
                    Console.WriteLine($"Vista previa de cámara GenTL iniciada: {camera.SensorSize.Width}x{camera.SensorSize.Height}");
                    
                    // Dejar que corra por unos segundos
                    await Task.Delay(3000);
                    
                    await videoCapture.StopAsync();
                    Console.WriteLine("Vista previa de cámara GenTL detenida");
                }
                finally
                {
                    await videoCapture.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error usando cámara GenTL: {ex.Message}");
            }
        }
        
        GenICamCameraManager.CloseCamera(device.DeviceId);
    }
}
```

## Proyectos de Muestra

Para ejemplos completos de integración y proyectos de muestra, explore estas implementaciones GenICam específicas:

- **[Demo de Captura GenICam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/GenICam%20Capture)** - Aplicación WPF completa demostrando integración de cámara GenICam con VideoCaptureCoreX
- **[Demo de Fuente Media Blocks GenICam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/GenICam%20Source%20Demo)** - Implementación avanzada de pipeline Media Blocks usando fuentes GenICam

Para ejemplos adicionales de integración y proyectos de muestra, visite nuestro [repositorio de muestras GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para explorar más muestras de código en diferentes plataformas y casos de uso.