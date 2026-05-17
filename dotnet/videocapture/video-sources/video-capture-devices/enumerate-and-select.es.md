---
title: Enumerar dispositivos de captura de video en C# .NET
description: Detecta, enumera y configura dispositivos de captura de video en .NET con ejemplos de código para listar dispositivos, formatos y tasas de fotogramas.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - C#
primary_api_classes:
  - DeviceEnumerator
  - VideoCaptureDeviceSourceSettings
  - VideoCaptureSource

---

# Trabajar con dispositivos de captura de video en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introducción a la gestión de dispositivos de video

Video Capture SDK .Net proporciona un soporte robusto para cualquier dispositivo de captura de video reconocido por tu sistema operativo. Esta guía demuestra cómo descubrir los dispositivos disponibles, inspeccionar sus capacidades e integrarlos en tus aplicaciones.

## Enumerar los dispositivos de captura de video disponibles

Antes de poder usar un dispositivo de captura, necesitas identificar cuáles están conectados al sistema. Los siguientes ejemplos de código muestran cómo obtener una lista de los dispositivos disponibles y mostrarlos en un componente de la interfaz de usuario:

=== "VideoCaptureCore"

    
    ```csharp
    // Iterar a través de todos los dispositivos de captura de video disponibles conectados al sistema
    foreach (var device in VideoCapture1.Video_CaptureDevices())
    {
        // Añadir cada nombre de dispositivo a un control desplegable de selección
        cbVideoInputDevice.Items.Add(device.Name);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Obtener asíncronamente todas las fuentes de video usando el DeviceEnumerator compartido
    var devices = DeviceEnumerator.Shared.VideoSourcesAsync();
    
    // Iterar a través de cada dispositivo disponible
    foreach (var device in await devices)
    {
        // Añadir el nombre amigable del dispositivo al control desplegable de selección
        cbVideoInputDevice.Items.Add(device.DisplayName);
    }
    ```
    


## Descubrir las capacidades de formato de video

Después de identificar un dispositivo de captura, puedes examinar sus formatos de video y tasas de fotogramas soportados. Esto te permite ofrecer a los usuarios las opciones de configuración adecuadas:

=== "VideoCaptureCore"

    
    ```csharp
    // Localizar un dispositivo específico por su nombre de visualización
    var deviceItem = VideoCapture1.Video_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // Iterar a través de todos los formatos de video soportados por este dispositivo
    foreach (var format in deviceItem.VideoFormats)
    {
        // Añadir cada formato al desplegable de selección de formato
        cbVideoInputFormat.Items.Add(format);
    
        // Para cada formato, iterar a través de sus tasas de fotogramas soportadas
        foreach (var frameRate in format.FrameRates)
        {
            // Añadir cada opción de tasa de fotogramas al desplegable de selección
            cbVideoInputFrameRate.Items.Add(frameRate.ToString(CultureInfo.CurrentCulture));
        }
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Localizar un dispositivo específico por su nombre de visualización
    var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(device => device.DisplayName == "Some device name");
    
    // Iterar a través de todos los formatos de video soportados por este dispositivo
    foreach (var format in deviceItem.VideoFormats)
    {
        // Añadir el nombre de visualización del formato al desplegable de selección
        cbVideoInputFormat.Items.Add(format.Name);
    
        // Para cada formato, obtener y mostrar las tasas de fotogramas disponibles
        foreach (var frameRate in format.FrameRateList)
        {
            // Añadir cada valor de tasa de fotogramas al desplegable de selección
            cbVideoInputFrameRate.Items.Add(frameRate.ToString());
        }
    }
    ```
    


## Configurar y activar un dispositivo de captura de video

Una vez que has seleccionado un dispositivo e identificado tus ajustes de formato preferidos, puedes inicializar la fuente de captura con estos parámetros:

=== "VideoCaptureCore"

    
    ```csharp
    // Encontrar el dispositivo seleccionado en la lista de dispositivos
    var deviceItem = VideoCapture1.Video_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // Crear una nueva fuente de captura de video usando el dispositivo seleccionado
    VideoCapture1.Video_CaptureDevice = new VideoCaptureSource(deviceItem.Name);
    
    // Configurar el formato de video por defecto desde la primera opción disponible
    VideoCapture1.Video_CaptureDevice.Format = deviceItem.VideoFormats[0].ToString();
    
    // Establecer la tasa de fotogramas por defecto desde la primera opción disponible para el formato seleccionado
    VideoCapture1.Video_CaptureDevice.FrameRate = deviceItem.VideoFormats[0].FrameRates[0];
    
    // Nota: después de esta configuración, usa el modo VideoPreview o VideoCapture para iniciar la transmisión
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Inicializar la variable de ajustes de fuente de video
    VideoCaptureDeviceSourceSettings videoSourceSettings = null;
    
    // Encontrar el dispositivo seleccionado por su nombre de visualización
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(x => x.DisplayName == deviceName);
    if (device != null)
    {
        // Localizar el formato seleccionado por su nombre
        var formatItem = device.VideoFormats.FirstOrDefault(x => x.Name == format);
        if (formatItem != null)
        {
            // Crear los ajustes de configuración usando el dispositivo seleccionado
            videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
            {
                // Convertir la representación del formato al objeto Format requerido
                Format = formatItem.ToFormat()
            };
    
            // Establecer la tasa de fotogramas deseada desde la selección del desplegable
            videoSourceSettings.Format.FrameRate = new VideoFrameRate(Convert.ToDouble(cbVideoInputFrameRate.Text));
        }
    }
    
    // Aplicar los ajustes configurados al componente de captura de video
    VideoCapture1.Video_Source = videoSourceSettings;
    ```
    


## Recursos adicionales y ejemplos de código

Para escenarios de uso más avanzados y ejemplos completos de implementación, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) con proyectos de demostración exhaustivos.

## Solución de problemas en la detección de dispositivos

Si tu aplicación no puede detectar los dispositivos esperados, considera estos problemas comunes:

1. Asegúrate de que el dispositivo esté correctamente conectado y encendido
2. Verifica que los controladores del dispositivo estén correctamente instalados
3. Comprueba que ninguna otra aplicación esté usando el dispositivo en este momento
4. Reinicia la aplicación después de conectar nuevos dispositivos
