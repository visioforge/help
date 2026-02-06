---
title: Gestión de Dispositivos de Captura de Video en .NET
description: Detecta, enumera y configura dispositivos de captura de video en .NET con ejemplos de código para listar dispositivos, formatos y tasas de fotogramas.
---

# Trabajar con Dispositivos de Captura de Video en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introducción a la Gestión de Dispositivos de Video

El Video Capture SDK .Net proporciona soporte robusto para cualquier dispositivo de captura de video reconocido por tu sistema operativo. Esta guía demuestra cómo descubrir dispositivos disponibles, inspeccionar sus capacidades e integrarlos en tus aplicaciones.

## Enumerar Dispositivos de Captura de Video Disponibles

Antes de poder usar un dispositivo de captura, necesitas identificar cuáles están conectados al sistema. Los siguientes ejemplos de código muestran cómo recuperar una lista de dispositivos disponibles y mostrarlos en un componente de interfaz de usuario:

=== "VideoCaptureCore"

    
    ```csharp
    // Iterar a través de todos los dispositivos de captura de video disponibles conectados al sistema
    foreach (var device in VideoCapture1.Video_CaptureDevices())
    {
        // Añadir cada nombre de dispositivo a un control de selección desplegable
        cbVideoInputDevice.Items.Add(device.Name);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Recuperar asincrónicamente todas las fuentes de video usando el DeviceEnumerator compartido
    var devices = DeviceEnumerator.Shared.VideoSourcesAsync();
    
    // Iterar a través de cada dispositivo disponible
    foreach (var device in await devices)
    {
        // Añadir el nombre amigable del dispositivo al control de selección desplegable
        cbVideoInputDevice.Items.Add(device.DisplayName);
    }
    ```
    


## Descubrir Capacidades de Formato de Video

Después de identificar un dispositivo de captura, puedes examinar sus formatos de video y tasas de fotogramas soportados. Esto te permite ofrecer a los usuarios opciones de configuración apropiadas:

=== "VideoCaptureCore"

    
    ```csharp
    // Localizar un dispositivo específico por su nombre de visualización
    var deviceItem = VideoCapture1.Video_CaptureDevices().FirstOrDefault(device => device.Name == "Algún nombre de dispositivo");
    
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
    var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(device => device.DisplayName == "Algún nombre de dispositivo");
    
    // Iterar a través de todos los formatos de video soportados por este dispositivo
    foreach (var format in deviceItem.VideoFormats)
    {
        // Añadir cada formato al desplegable de selección de formato
        cbVideoInputFormat.Items.Add(format);
    
        // Añadir la tasa de fotogramas de este formato
        cbVideoInputFrameRate.Items.Add(format.FrameRate.ToString());
    }
    ```
    


## Seleccionar y Configurar un Dispositivo

Una vez que has identificado un dispositivo y sus capacidades, puedes configurarlo para uso:

=== "VideoCaptureCore"

    
    ```csharp
    // Establecer el dispositivo de captura de video
    VideoCapture1.Video_CaptureDevice = new VideoCaptureDevice(cbVideoInputDevice.Text);
    
    // Establecer el formato de video seleccionado
    VideoCapture1.Video_CaptureDevice.Format = cbVideoInputFormat.Text;
    
    // Establecer la tasa de fotogramas seleccionada
    VideoCapture1.Video_CaptureDevice.FrameRate = double.Parse(cbVideoInputFrameRate.Text);
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Obtener el dispositivo seleccionado
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())
        .FirstOrDefault(d => d.DisplayName == cbVideoInputDevice.Text);
    
    // Crear configuración de fuente
    var sourceSettings = new VideoCaptureDeviceSourceSettings(device)
    {
        Format = device.VideoFormats.First(f => f.ToString() == cbVideoInputFormat.Text)
    };
    
    // Establecer fuente de video
    VideoCapture1.Video_Source = sourceSettings;
    ```
    


## Obtener Resolución del Dispositivo

Puedes obtener información detallada sobre las resoluciones soportadas:

```csharp
// Obtener todas las resoluciones disponibles
foreach (var format in deviceItem.VideoFormats)
{
    int width = format.Width;
    int height = format.Height;
    Console.WriteLine($"Resolución: {width}x{height}");
}
```

## Verificar Estado del Dispositivo

Antes de usar un dispositivo, es buena práctica verificar que esté disponible:

```csharp
// Verificar si hay dispositivos disponibles
var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
if (devices.Length == 0)
{
    MessageBox.Show("No se encontraron dispositivos de captura de video.");
    return;
}
```

## Mejores Prácticas

1. **Enumeración Asíncrona**: Siempre usa métodos asíncronos para enumerar dispositivos para evitar bloquear la UI
2. **Manejo de Errores**: Implementa manejo de errores para cuando los dispositivos no estén disponibles
3. **Actualizar Lista de Dispositivos**: Proporciona forma de actualizar la lista cuando se conectan nuevos dispositivos
4. **Recordar Selecciones**: Guarda las preferencias del usuario para selección automática

## Aplicaciones de Ejemplo

Explora estas aplicaciones de ejemplo para ver la gestión de dispositivos de video en acción:

=== "VideoCaptureCore"

    
    - [Demo Principal de Video Capture (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
    - [Demo de Webcam (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Simple%20VideoCapture)
    

=== "VideoCaptureCoreX"

    
    - [Demo de Captura de Video X (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Simple%20Video%20Capture)
    


---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.