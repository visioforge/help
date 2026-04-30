---
title: Fuentes de Audio en C# .NET — Micrófono y Loopback
description: Configure fuentes de audio en C# .NET — micrófono, loopback del sistema, audio de cámaras IP y Decklink con ejemplos de código completos.
sidebar_label: Fuentes de Audio
order: 15
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - IVideoCaptureBaseAudioSourceSettings
  - AudioCaptureSource
  - DeviceEnumerator
  - LoopbackAudioCaptureDeviceSourceSettings
  - AudioCaptureDeviceFormat

---

# Trabajando con Fuentes de Audio en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Fuentes de Audio Disponibles

Al construir aplicaciones multimedia, necesitará capturar audio de varias fuentes. Esta guía cubre cómo implementar la captura de audio desde múltiples tipos de entrada utilizando nuestro SDK:

* Dispositivos de captura de audio (micrófonos, entrada de línea)
* Audio del sistema (altavoces/auriculares a través de bucle invertido)
* Flujos de red (cámaras IP)
* Dispositivos profesionales Decklink

Cada tipo de fuente requiere diferentes métodos de inicialización y tiene capacidades únicas. Exploremos cómo trabajar con cada uno.

## Implementación de Dispositivos de Captura de Audio

Los dispositivos de captura de audio incluyen micrófonos, cámaras web con micrófonos incorporados y otro hardware de entrada conectado a su sistema. Trabajar con estos dispositivos implica tres pasos clave:

1. Enumerar los dispositivos disponibles
2. Seleccionar los formatos de audio apropiados
3. Configurar el dispositivo seleccionado como su fuente de audio

### Enumeración de Dispositivos de Audio Disponibles

Primero, necesita detectar todos los dispositivos de entrada de audio conectados al sistema:

=== "VideoCaptureCoreX"

    
    ```csharp
    var audioSources = await core.Audio_SourcesAsync();
    foreach (var source in audioSources)
    {
        // add to some combobox
        cbAudioInputDevice.Items.Add(source.DisplayName);
    }
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    foreach (var device in core.Audio_CaptureDevices())
    {
        // add to some combobox
        cbAudioInputDevice.Items.Add(device.Name);
    }
    ```
    


Este código recupera todos los dispositivos de entrada de audio y puede mostrarlos en un menú desplegable para la selección del usuario. El enfoque asíncrono en VideoCaptureCoreX proporciona un mejor rendimiento para sistemas con muchos dispositivos conectados.

### Descubriendo Formatos de Audio Soportados

Una vez que haya identificado los dispositivos disponibles, necesitará determinar qué formatos de audio soporta cada dispositivo:

=== "VideoCaptureCoreX"

    
    ```csharp
    // find the device by name
    var deviceItem = (await VideoCapture1.Audio_SourcesAsync()).FirstOrDefault(device => device.DisplayName == "Some device name");
    if (deviceItem == null)
    {
        return;
    }
    
    // enumerate formats
    foreach (var format in deviceItem.Formats)
    {
        cbAudioInputFormat.Items.Add(format.Name);
    }
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    // find the device by name
    var deviceItem = VideoCapture1.Audio_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // enumerate formats
    foreach (var format in deviceItem.Formats)
    {
        cbAudioInputFormat.Items.Add(format);
    }
    ```
    


Diferentes dispositivos de audio soportan varios formatos con diferentes profundidades de bits, tasas de muestreo y configuraciones de canales. Enumerar estas opciones le permite seleccionar el formato más apropiado para las necesidades de su aplicación.

### Configuración del Dispositivo de Captura de Audio

Después de seleccionar un dispositivo y formato, configúrelo como su fuente de audio:

=== "VideoCaptureCoreX"

    
    ```csharp
    // Enumera dispositivos de captura de audio (Audio_SourcesAsync devuelve AudioCaptureDeviceInfo[]).
    var devices = await VideoCapture1.Audio_SourcesAsync();
    var deviceItem = devices.FirstOrDefault(d => d.Name == "Nombre del dispositivo");
    if (deviceItem == null)
    {
        return;
    }

    // Toma el primer formato reportado por el dispositivo.
    AudioCaptureDeviceFormat format = deviceItem.Formats[0].ToFormat();

    // Construye los settings de la fuente y asígnalos.
    IVideoCaptureBaseAudioSourceSettings audioSource = deviceItem.CreateSourceSettingsVC(format);
    VideoCapture1.Audio_Source = audioSource;
    ```
    

=== "VideoCaptureCore"

    
    ```csharp
    // find the device by name
    var deviceItem = VideoCapture1.Audio_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource(deviceItem.Name);
    VideoCapture1.Audio_CaptureDevice.Format = deviceItem.Formats[0].ToString(); // set the first format
    ```
    


Este código configura su aplicación para capturar audio del dispositivo seleccionado utilizando el formato especificado. La API de VideoCaptureCoreX proporciona un control más granular sobre la selección de formato y la configuración del dispositivo.

## Captura de Audio del Sistema a través de Bucle Invertido (Loopback)

El bucle invertido de audio le permite grabar cualquier sonido que se reproduzca a través de los altavoces o auriculares de su sistema. Esto es particularmente útil para:

* Grabación de pantalla con audio
* Captura de sonidos de aplicaciones
* Grabación de audio de conferencias web o servicios de transmisión

Aquí se explica cómo implementarlo:

=== "VideoCaptureCoreX"

    
    Primero, enumere los dispositivos de bucle invertido disponibles:
    
    ```csharp
    // Enumerate audio loopback devices
    var audioSinks = await DeviceEnumerator.Shared.AudioOutputsAsync();
    foreach (var sink in audioSinks)
    {   
        // Filter by WASAPI2 API
        if (sink.API == AudioOutputDeviceAPI.WASAPI2)
        {
            // Add to some combobox
            cbAudioLoopbackDevice.Items.Add(sink.Name);
        }
    }
    ```
    
    A continuación, cree la configuración de fuente para su dispositivo de salida seleccionado:
    
    ```csharp
    // audio input
    var deviceItem = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)).FirstOrDefault(device => device.Name == "Output device name");
    if (deviceItem == null)
    {
        return;
    }
    
    IVideoCaptureBaseAudioSourceSettings audioSource = new LoopbackAudioCaptureDeviceSourceSettings(deviceItem);
    
    VideoCapture1.Audio_Source = audioSource;
    ```
    
    La API WASAPI2 proporciona la funcionalidad de bucle invertido más confiable en sistemas Windows, con menor latencia y mejor rendimiento en comparación con otras opciones.
    

=== "VideoCaptureCore"

    
    En VideoCaptureCore, la funcionalidad de bucle invertido se simplifica con un dispositivo virtual dedicado:
    
    ```cs
    VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource("VisioForge What You Hear Source");
    VideoCapture1.Audio_CaptureDevice.Format_UseBest = true;
    ```
    
    Este enfoque selecciona automáticamente el mejor formato disponible para la fuente de bucle invertido, haciendo que la implementación sea sencilla.
    


Para ejemplos completos y ejecutables de captura de altavoz (incluyendo el enfoque de pipeline con Media Blocks SDK), consulte la [guía de Captura de Audio](../audio-capture/index.md#capturar-audio-del-sistema-altavoz-loopback).

## Trabajando con Fuentes de Audio de Red

Para cámaras IP y otros flujos de red, el audio viaja sobre el mismo transporte que el video — normalmente no construyes una fuente de audio separada. Crea los settings de fuente IP con `audioEnabled: true` y el SDK demuxea audio y video desde la misma URL:

```csharp
// Cámara RTSP — el audio viene automáticamente cuando audioEnabled es true.
var rtsp = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:554/Streaming/Channels/101"),
    login: "admin",
    password: "password",
    audioEnabled: true);

VideoCapture1.Video_Source = rtsp;
VideoCapture1.Audio_Record = true;   // incluye el audio RTSP en la salida a archivo
```

El audio de fuentes de red puede venir en varios formatos (AAC, MP3, PCM) dependiendo del dispositivo. El SDK convierte y sincroniza automáticamente.

## Implementación de Captura de Audio Decklink

Los dispositivos Decklink entregan audio de grado profesional (hasta 192 kHz, multicanal, SDI embebido). Usa `DecklinkAudioSourceSettings` y conéctalo junto a la fuente de video Decklink:

```csharp
// Enumera entradas de audio Decklink.
var devices = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();
var dl = devices.First();

// Adjunta la fuente de audio a VideoCaptureCoreX. La fuente de video correspondiente va en Video_Source.
VideoCapture1.Audio_Source = new DecklinkAudioSourceSettings(dl);
VideoCapture1.Audio_Record = true;
```

Los ajustes de audio están mayormente determinados por el modo actual del dispositivo (sample rate y canales los fija la señal SDI entrante) — normalmente no los sobreescribes en la clase de settings.

## Mejores Prácticas para la Captura de Audio

Para garantizar una captura de audio de alta calidad en sus aplicaciones:

1. **Selección de tasa de muestreo**: Elija tasas de muestreo apropiadas basadas en su salida objetivo. Para la mayoría de las aplicaciones, 44.1kHz o 48kHz es suficiente.

2. **Gestión de búfer**: Configure tamaños de búfer apropiados para equilibrar entre latencia y estabilidad. Los búferes más pequeños reducen la latencia pero pueden causar interrupciones de audio.

3. **Manejo de formatos**: Soporte múltiples formatos para acomodar varios dispositivos. Siempre tenga opciones de respaldo cuando formatos específicos no estén disponibles.

4. **Monitoreo de nivel**: Implemente monitoreo de nivel de audio para detectar silencio o saturación, permitiendo que su aplicación responda apropiadamente.

5. **Manejo de errores**: Implemente un manejo de errores robusto para desconexiones de dispositivos o fallas en la negociación de formatos.

## Conclusión

La implementación de capacidades de captura de audio en su aplicación .NET implica seleccionar la fuente adecuada, configurar formatos y gestionar el flujo de audio. Ya sea que esté capturando desde micrófonos, audio del sistema o fuentes de red, nuestro SDK proporciona las herramientas necesarias para construir aplicaciones de audio sofisticadas.

Siguiendo los ejemplos de código y los patrones de implementación descritos en esta guía, podrá integrar una potente funcionalidad de captura de audio en sus proyectos de manera eficiente.

## Aplicaciones de Ejemplo

Ejemplos de trabajo completos están disponibles en GitHub:

* [Todos los Ejemplos de Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)
