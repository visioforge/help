---
title: Decklink SDK .NET — Captura de video con Blackmagic
description: Implemente captura de video profesional en .NET con dispositivos Blackmagic Decklink usando ejemplos de código para aplicaciones de calidad broadcast.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Capture
  - Decklink
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - DeviceEnumerator
  - DecklinkVideoSourceSettings
  - DecklinkAudioSourceSettings
  - DecklinkSourceSettings

---

# Integrar dispositivos Blackmagic Decklink con aplicaciones .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## ¿Qué son los dispositivos Decklink?

Los dispositivos Decklink de Blackmagic Design representan tarjetas de captura y reproducción estándar de la industria diseñadas para entornos de producción de video profesional. Estas soluciones de hardware de alto rendimiento entregan capacidades excepcionales para desarrolladores que integran funcionalidad de captura de video en aplicaciones .NET.

Las tarjetas Decklink son reconocidas por sus especificaciones técnicas superiores:

- Soporte para formatos de video de alta resolución incluyendo 4K, 8K y HD
- Múltiples opciones de conexión de entrada/salida (SDI, HDMI) para integración flexible
- Rendimiento de baja latencia crucial para aplicaciones de transmisión en tiempo real
- Compatibilidad cruzada con las principales plataformas de software de producción de video
- APIs amigables para desarrolladores que se integran perfectamente con varios lenguajes de programación

Para desarrolladores .NET que construyen aplicaciones de video profesionales, la integración Decklink proporciona una base confiable para manejar procesamiento de señales de video de calidad broadcast.

## Detección programática de dispositivos

El primer paso en la implementación de funcionalidad Decklink es la enumeración apropiada de dispositivos. Las siguientes muestras de código demuestran cómo detectar hardware Decklink disponible en su aplicación .NET.

### Enumerar dispositivos disponibles

=== "VideoCaptureCore"

    
    ```csharp
    foreach (var device in (await VideoCapture1.Decklink_CaptureDevicesAsync()))
    {   
        cbDecklinkCaptureDevice.Items.Add(device.Name);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    var videoCaptureDevices = await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync();
    if (videoCaptureDevices.Length > 0)
    {
        foreach (var item in videoCaptureDevices)
        {
            cbVideoInput.Items.Add(item.Name);
        }
    
        cbVideoInput.SelectedIndex = 0;
    }
    ```
    
    Al trabajar con VideoCaptureCoreX, también necesitará enumerar las fuentes de audio por separado:
    
    ```cs
    var audioCaptureDevices = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();
    if (audioCaptureDevices.Length > 0)
    {
        foreach (var item in audioCaptureDevices)
        {
            cbAudioInput.Items.Add(item.Name);
        }
    
        cbAudioInput.SelectedIndex = 0;
    }
    ```
    


## Trabajar con formatos de video y tasas de fotogramas

Después de enumerar dispositivos, el siguiente paso involucra determinar los formatos de video y tasas de fotogramas disponibles. Las tarjetas Decklink soportan numerosos formatos profesionales, pero las opciones específicas dependen de la fuente de entrada conectada.

### Obtener información de formato

=== "VideoCaptureCore"

    
    ```csharp
    // Enumerar y filtrar por nombre de dispositivo
    var deviceItem = (await VideoCapture1.Decklink_CaptureDevicesAsync()).Find(device => device.Name == cbDecklinkCaptureDevice.Text);
    if (deviceItem != null)
    {
        // Leer formatos de video y añadir al combobox
        foreach (var format in (await deviceItem.GetVideoFormatsAsync()))
        {
            cbDecklinkCaptureVideoFormat.Items.Add(format.Name);
        }
    
        // Si el formato no existe, añadir un mensaje
        if (cbDecklinkCaptureVideoFormat.Items.Count == 0)
        {
            cbDecklinkCaptureVideoFormat.Items.Add("No hay entrada conectada");
        }
    }
    ```
    
    Este enfoque proporciona información de diagnóstico valiosa. Si no se retornan formatos, típicamente indica que no hay una entrada activa conectada al dispositivo Decklink. Implementar esta verificación ayuda a los usuarios a solucionar problemas de conexión directamente desde la interfaz de su aplicación.
    

=== "VideoCaptureCoreX"

    
    Con VideoCaptureCoreX, trabajará con modos predefinidos de la enumeración DecklinkMode:
    
    ```cs
    var decklinkModes = Enum.GetValues(typeof(DecklinkMode));
    foreach (var item in decklinkModes)
    {
        cbVideoMode.Items.Add(item.ToString());
    }
    ```
    
    Este enfoque usa configuraciones de modo estandarizadas que deben configurarse en su hardware Decklink.
    


## Configurar Decklink como fuente de video

Una vez que ha detectado dispositivos e identificado los formatos disponibles, necesita configurar el hardware Decklink como su fuente de captura. Este paso crítico establece la conexión entre hardware y software.

### Establecer parámetros de fuente

=== "VideoCaptureCore"

    
    ```csharp
    VideoCapture1.Decklink_Source = new DecklinkSourceSettings
    {
        Name = cbDecklinkCaptureDevice.Text,
        VideoFormat = cbDecklinkCaptureVideoFormat.Text
    };

    // Seleccionar el modo Decklink antes de StartAsync:
    //   DecklinkSourcePreview — monitoreo en tiempo real con procesamiento mínimo.
    //   DecklinkSourceCapture — grabación de alta calidad con buffering mejorado.
    VideoCapture1.Mode = VideoCaptureMode.DecklinkSourcePreview;   // o VideoCaptureMode.DecklinkSourceCapture

    await VideoCapture1.StartAsync();
    ```
    
    - `DecklinkSourcePreview`: Optimizado para monitoreo en tiempo real con procesamiento mínimo
    - `DecklinkSourceCapture`: Configurado para grabación de alta calidad con buffering mejorado
    

=== "VideoCaptureCoreX"

    
    VideoCaptureCoreX requiere configuración separada de fuentes de video y audio:
    
    ```cs
    // Crear ajustes para la fuente de video
    DecklinkVideoSourceSettings videoSourceSettings = null;
    
    // Nombre del dispositivo desde el combo box
    var deviceName = cbVideoInput.Text;
    
    // Modo desde el combobox
    var mode = cbVideoMode.Text;
    if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(mode))
    {
        // Encontrar dispositivo
        var device = (await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync()).FirstOrDefault(x => x.Name == deviceName);
        if (device != null)
        {
            // Crear ajustes de fuente de video usando dispositivo y modo
            videoSourceSettings = new DecklinkVideoSourceSettings(device)
            {
                Mode = (DecklinkMode)Enum.Parse(typeof(DecklinkMode), mode, true)
            };
        }
    }
    
    // Establecer la fuente de video al objeto VideoCaptureCoreX
    VideoCapture1.Video_Source = videoSourceSettings;
    ```
    
    Para configuración de audio:
    
    ```cs
    // Crear ajustes para la fuente de audio
    DecklinkAudioSourceSettings audioSourceSettings = null;
    
    // Nombre del dispositivo desde el combobox
    deviceName = cbAudioInput.Text;
    if (!string.IsNullOrEmpty(deviceName))
    {
        // Encontrar dispositivo
        var device = (await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync()).FirstOrDefault(x => x.Name == deviceName);
        if (device != null)
        {
            // Crear ajustes para la fuente de audio usando dispositivo
            audioSourceSettings = new DecklinkAudioSourceSettings(device);
        }
    }
    
    // Establecer la fuente de audio al objeto VideoCaptureCoreX
    VideoCapture1.Audio_Source = audioSourceSettings;
    ```
    
    Esta separación ofrece mayor flexibilidad para escenarios avanzados donde podría necesitar procesar video y audio independientemente.
    


## Consideraciones de rendimiento

Al implementar captura Decklink en entornos de producción, considere estos factores de rendimiento:

1. **Gestión de búfer:** Los formatos de video profesional requieren asignación sustancial de memoria, especialmente para resoluciones 4K+
2. **Utilización de CPU:** La codificación en tiempo real de flujos Decklink puede ser intensiva en procesador
3. **E/S de disco:** Al capturar a almacenamiento, asegúrese de que sus velocidades de escritura soporten la tasa de datos del formato seleccionado
4. **Ancho de banda de memoria:** Los flujos de alta resolución sin comprimir demandan recursos significativos del sistema

Implementar manejo de errores apropiado alrededor de la conexión del dispositivo y detección de formato mejorará la resiliencia de su aplicación en entornos de producción.

## Aplicaciones de ejemplo y ejemplos de implementación

Examinar ejemplos funcionales proporciona valiosas perspectivas sobre patrones de implementación efectivos. El SDK incluye numerosas aplicaciones de ejemplo que demuestran la integración Decklink.

### Aplicaciones de referencia

=== "VideoCaptureCore"

    
    - [Muestra principal con entrada DeckLink, procesamiento de video/audio y muchos formatos de salida (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
    - [Muestra principal para WinForms](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [Muestra simple con entrada DeckLink y muchos formatos de salida (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Decklink%20Demo)
    

=== "VideoCaptureCoreX"

    
    - [Vista previa de video y captura a archivo MP4 o WebM (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Decklink%20Demo%20X)
    


## Mejores prácticas para integración

Basándonos en pruebas de campo extensas y experiencias de implementación de clientes, recomendamos estas mejores prácticas:

1. **Siempre verificar conectividad del dispositivo:** Verifique los formatos disponibles para confirmar la conexión apropiada de señal
2. **Implementar alternativas elegantes:** Proporcione mensajes de error significativos cuando los dispositivos esperados no están disponibles
3. **Probar con múltiples tasas de fotogramas:** Algunas aplicaciones pueden comportarse diferente con formatos de entrada variados
4. **Gestionar memoria efectivamente:** La captura de alta resolución requiere gestión apropiada de recursos
5. **Monitorear uso de CPU:** Las operaciones de codificación pueden ser intensivas en procesador durante la captura
6. **Proporcionar detalles de formato a usuarios:** Dé información clara sobre formatos detectados y estado de conexión

Estas recomendaciones ayudan a asegurar implementaciones robustas que funcionan confiablemente a través de diferentes configuraciones de hardware y condiciones de operación.

## Conclusión

Integrar dispositivos Blackmagic Decklink con aplicaciones .NET proporciona capacidades poderosas para escenarios de captura de video profesional. Siguiendo los patrones de implementación descritos en esta guía, los desarrolladores pueden crear aplicaciones estables de alto rendimiento que aprovechan todas las capacidades del hardware Decklink.

El Video Capture SDK ofrece un enfoque simplificado para trabajar con estos dispositivos profesionales, abstrayendo mucha de la complejidad mientras proporciona la flexibilidad necesaria para personalización avanzada.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.