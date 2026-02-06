---
title: Enumeración de Dispositivos de Medios en .NET
description: Enumere cámaras de video, entradas/salidas de audio, dispositivos Decklink, fuentes NDI y cámaras GenICam en .NET con ejemplos de código prácticos.
---

# Guía Completa para Enumeración de Dispositivos de Medios en .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El Media Blocks SDK proporciona una forma potente y eficiente de descubrir y trabajar con varios dispositivos de medios en sus aplicaciones .NET. Esta guía le llevará a través del proceso de enumerar diferentes tipos de dispositivos de medios usando la clase `DeviceEnumerator` del SDK.

## Introducción a la Enumeración de Dispositivos

La enumeración de dispositivos es un primer paso crítico al desarrollar aplicaciones que interactúan con hardware de medios. La clase `DeviceEnumerator` proporciona una forma centralizada de detectar y listar todos los dispositivos de medios disponibles conectados a su sistema.

El SDK usa un patrón singleton para la enumeración de dispositivos, facilitando el acceso a la funcionalidad desde cualquier lugar de su código:

```csharp
// Acceder a la instancia compartida de DeviceEnumerator
var enumerator = DeviceEnumerator.Shared;
```

## Descubriendo Dispositivos de Entrada de Video

### Fuentes de Video Estándar

Para listar todos los dispositivos de entrada de video disponibles (webcams, tarjetas de captura, cámaras virtuales):

```csharp
var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

foreach (var device in videoSources)
{
    Debug.WriteLine($"Dispositivo de video encontrado: {device.Name}");
    // Puede acceder a propiedades adicionales aquí si es necesario
}
```

Los objetos `VideoCaptureDeviceInfo` devueltos proporcionan información detallada sobre cada dispositivo, incluyendo nombre del dispositivo, identificadores internos y tipo de API.

## Trabajando con Dispositivos de Audio

### Enumerando Fuentes de Entrada de Audio

Para descubrir micrófonos y otros dispositivos de entrada de audio:

```csharp
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();

foreach (var device in audioSources)
{
    Debug.WriteLine($"Dispositivo de entrada de audio encontrado: {device.Name}");
    // Información adicional del dispositivo puede accederse aquí
}
```

También puede filtrar dispositivos de audio por su tipo de API:

```csharp
// Obtener solo fuentes de audio para una API específica
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.DirectSound);
```

### Encontrando Dispositivos de Salida de Audio

Para altavoces, auriculares y otros destinos de salida de audio:

```csharp
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();

foreach (var device in audioOutputs)
{
    Debug.WriteLine($"Dispositivo de salida de audio encontrado: {device.Name}");
    // Procesar información del dispositivo según sea necesario
}
```

Similar a las fuentes de audio, puede filtrar salidas por API:

```csharp
// Obtener solo salidas de audio para una API específica
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound);
```

## Integración Profesional Blackmagic Decklink

### Fuentes de Entrada de Video Decklink

Para flujos de trabajo de video profesionales usando hardware Blackmagic:

```csharp
var decklinkVideoSources = await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync();

foreach (var device in decklinkVideoSources)
{
    Debug.WriteLine($"Entrada de video Decklink: {device.Name}");
    // Puede trabajar con propiedades específicas de Decklink aquí
}
```

### Fuentes de Entrada de Audio Decklink

Para acceder a canales de audio desde dispositivos Decklink:

```csharp
var decklinkAudioSources = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();

foreach (var device in decklinkAudioSources)
{
    Debug.WriteLine($"Entrada de audio Decklink: {device.Name}");
    // Procesar información del dispositivo de audio Decklink
}
```

### Destinos de Salida de Video Decklink

Para enviar video a dispositivos de salida Decklink:

```csharp
var decklinkVideoOutputs = await DeviceEnumerator.Shared.DecklinkVideoSinksAsync();

foreach (var device in decklinkVideoOutputs)
{
    Debug.WriteLine($"Salida de video Decklink: {device.Name}");
    // Acceder a propiedades del dispositivo de salida según sea necesario
}
```

### Destinos de Salida de Audio Decklink

Para enrutar audio a salidas de hardware Decklink:

```csharp
var decklinkAudioOutputs = await DeviceEnumerator.Shared.DecklinkAudioSinksAsync();

foreach (var device in decklinkAudioOutputs)
{
    Debug.WriteLine($"Salida de audio Decklink: {device.Name}");
    // Trabajar con configuración de salida de audio aquí
}
```

## Integración de Dispositivos de Red

### Descubrimiento de Fuentes NDI

Para encontrar fuentes NDI disponibles en su red:

```csharp
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();

foreach (var device in ndiSources)
{
    Debug.WriteLine($"Fuente NDI descubierta: {device.Name}");
    // Procesar propiedades e información específicas de NDI
}
```

### Descubrimiento de Cámaras de Red ONVIF

Para encontrar cámaras IP que soportan el protocolo ONVIF:

```csharp
// Establecer un timeout para el descubrimiento (2 segundos en este ejemplo)
var timeout = TimeSpan.FromSeconds(2);
var onvifDevices = await DeviceEnumerator.Shared.ONVIF_ListSourcesAsync(timeout, null);

foreach (var deviceUri in onvifDevices)
{
    Debug.WriteLine($"Cámara ONVIF encontrada en: {deviceUri}");
    // Conectar a la cámara usando la URI descubierta
}
```

## Soporte de Cámaras Industriales

### Cámaras Industriales Basler

Para aplicaciones que requieren cámaras industriales Basler:

```csharp
var baslerCameras = await DeviceEnumerator.Shared.BaslerSourcesAsync();

foreach (var device in baslerCameras)
{
    Debug.WriteLine($"Cámara Basler detectada: {device.Name}");
    // Acceder a características específicas de cámaras Basler
}
```

### Cámaras Industriales Allied Vision

Para trabajar con cámaras Allied Vision en su aplicación:

```csharp
var alliedCameras = await DeviceEnumerator.Shared.AlliedVisionSourcesAsync();

foreach (var device in alliedCameras)
{
    Debug.WriteLine($"Cámara Allied Vision encontrada: {device.Name}");
    // Configurar parámetros específicos de Allied Vision
}
```

### Cámaras Compatibles con Spinnaker SDK

Para cámaras que soportan el Spinnaker SDK (solo Windows):

```csharp
#if NET_WINDOWS
var spinnakerCameras = await DeviceEnumerator.Shared.SpinnakerSourcesAsync();

foreach (var device in spinnakerCameras)
{
    Debug.WriteLine($"Cámara Spinnaker SDK: {device.Name}");
    Debug.WriteLine($"Modelo: {device.Model}, Fabricante: {device.Vendor}");
    Debug.WriteLine($"Resolución: {device.WidthMax}x{device.HeightMax}");
    // Trabajar con propiedades específicas de la cámara
}
#endif
```

### Cámaras Genéricas Estándar GenICam

Para otras cámaras industriales que soportan el estándar GenICam:

```csharp
var genicamCameras = await DeviceEnumerator.Shared.GenICamSourcesAsync();

foreach (var device in genicamCameras)
{
    Debug.WriteLine($"Dispositivo compatible GenICam: {device.Name}");
    Debug.WriteLine($"Modelo: {device.Model}, Fabricante: {device.Vendor}");
    Debug.WriteLine($"Protocolo: {device.Protocol}, Serie: {device.SerialNumber}");
    // Trabajar con características estándar GenICam
}
```

## Monitoreo de Dispositivos

El SDK también soporta monitoreo de conexiones y desconexiones de dispositivos:

```csharp
// Iniciar monitoreo de cambios en dispositivos de video
await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();

// Iniciar monitoreo de cambios en dispositivos de audio
await DeviceEnumerator.Shared.StartAudioSourceMonitorAsync();
await DeviceEnumerator.Shared.StartAudioSinkMonitorAsync();

// Suscribirse a eventos de cambio de dispositivos
DeviceEnumerator.Shared.OnVideoSourceAdded += (sender, device) => 
{
    Debug.WriteLine($"Nuevo dispositivo de video conectado: {device.Name}");
};

DeviceEnumerator.Shared.OnVideoSourceRemoved += (sender, device) => 
{
    Debug.WriteLine($"Dispositivo de video desconectado: {device.Name}");
};
```

## Consideraciones Específicas de Plataforma

### Windows

En Windows, el SDK puede detectar eventos de conexión y eliminación de dispositivos USB a nivel de sistema:

```csharp
#if NET_WINDOWS
// Suscribirse a eventos de dispositivos a nivel de sistema
DeviceEnumerator.Shared.OnDeviceAdded += (sender, args) => 
{
    // Refrescar listas de dispositivos cuando se conecta nuevo hardware
    RefreshDeviceLists();
};

DeviceEnumerator.Shared.OnDeviceRemoved += (sender, args) => 
{
    // Actualizar UI cuando se desconecta hardware
    RefreshDeviceLists();
};
#endif
```

Por defecto, la enumeración de dispositivos Media Foundation está deshabilitada para evitar duplicación con dispositivos DirectShow. Puede habilitarla si es necesario:

```csharp
#if NET_WINDOWS
// Habilitar enumeración de dispositivos Media Foundation si es requerido
DeviceEnumerator.Shared.IsEnumerateMediaFoundationDevices = true;
#endif
```

### iOS y Android

En plataformas móviles, el SDK maneja las solicitudes de permisos requeridas al enumerar dispositivos:

```csharp
#if __IOS__ || __ANDROID__
// Esto solicitará automáticamente permisos de cámara si es necesario
var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

// Esto solicitará automáticamente permisos de micrófono si es necesario
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();
#endif
```

## Mejores Prácticas para Enumeración de Dispositivos

Al trabajar con enumeración de dispositivos en aplicaciones de producción:

1. Siempre maneje casos donde no se encuentran dispositivos
2. Considere cachear listas de dispositivos cuando sea apropiado para mejorar rendimiento
3. Implemente manejo de excepciones apropiado para fallos de acceso a dispositivos
4. Proporcione retroalimentación clara al usuario cuando faltan dispositivos requeridos
5. Use los métodos async para evitar bloquear el hilo de UI durante la enumeración
6. Limpie recursos llamando a `Dispose()` cuando termine con el DeviceEnumerator

```csharp
// Limpieza apropiada cuando termine
DeviceEnumerator.Shared.Dispose();
```
