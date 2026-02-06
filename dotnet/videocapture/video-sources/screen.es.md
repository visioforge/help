---
title: Fuente de Captura de Pantalla para SDK de Video .NET
description: Captura pantallas completas, ventanas o áreas personalizadas en apps .NET con integración DirectX, soporte de cursor y alta performance.
---

# Guía de Implementación de Captura de Pantalla

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introducción a la Captura de Pantalla

La tecnología de captura de pantalla permite a los desarrolladores grabar y transmitir programáticamente el contenido visual mostrado en un monitor de computadora. Esta poderosa funcionalidad sirve como base para numerosas aplicaciones incluyendo:

- Herramientas de soporte remoto y asistencia técnica
- Demostración de software y creación de tutoriales
- Grabación y transmisión de juegos
- Sistemas de webinars y presentaciones
- Automatización de control de calidad y pruebas

Video Capture SDK .Net proporciona a los desarrolladores herramientas robustas para capturar contenido de pantalla con alto rendimiento y flexibilidad. El SDK soporta capturar pantallas completas, ventanas de aplicaciones individuales o regiones de pantalla personalizadas.

## Soporte de Plataforma y Descripción General de la Tecnología

### Implementación en Windows

En plataformas Windows, el SDK aprovecha el poder de las tecnologías DirectX para lograr un rendimiento óptimo. Los desarrolladores pueden elegir entre:

- **DirectX 9**: Soporte legacy para sistemas más antiguos
- **DirectX 11/12**: Implementación moderna que ofrece rendimiento y eficiencia superiores

DirectX 11 es particularmente recomendado para escenarios de captura de ventanas debido a su manejo mejorado de composición de ventanas y características de rendimiento superiores.

=== "VideoCaptureCore"

    
    ### Configuración de Captura Principal
    
    La implementación de VideoCaptureCore proporciona opciones de configuración sencillas para controlar el proceso de captura:
    
    - `AllowCaptureMouseCursor`: Habilitar o deshabilitar la visibilidad del cursor en el contenido capturado
    - `DisplayIndex`: Seleccionar qué pantalla capturar en configuraciones de múltiples monitores (indexado desde cero)
    - `ScreenPreview` / `ScreenCapture`: Establecer el modo operacional para visualización o grabación
    

=== "VideoCaptureCoreX"

    
    ### Configuración de Captura Avanzada
    
    VideoCaptureCoreX ofrece control más granular a través de clases de configuración dedicadas:
    
    - `ScreenCaptureDX9SourceSettings`: Configurar captura basada en DirectX 9
    - `ScreenCaptureD3D11SourceSettings`: Configurar captura basada en DirectX 11 con rendimiento mejorado
    


## Implementación de Captura de Pantalla Completa y Región

Capturar ya sea una pantalla completa o una región de pantalla definida es un requisito común para muchas aplicaciones. A continuación están los enfoques de implementación para VideoCaptureCore y VideoCaptureCoreX.

=== "VideoCaptureCore"

    
    ### Configurar Captura de Pantalla Completa y Región
    
    El siguiente código demuestra cómo configurar los ajustes de captura de pantalla para modo de pantalla completa o una región rectangular específica:
    
    ```csharp
    // Establecer ajustes de fuente de captura de pantalla
    VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
    {
         // Establecer a true para capturar la pantalla completa
        FullScreen = false,
    
         // Establecer la posición izquierda del área de pantalla
        Left = 0,
    
        // Establecer la posición superior del área de pantalla
        Top = 0, 
    
        // Establecer el ancho del área de pantalla
        Width = 640, 
    
        // Establecer la altura del área de pantalla
        Height = 480, 
    
        // Establecer el índice de pantalla
        DisplayIndex = 0, 
    
        // Establecer la tasa de fotogramas
        FrameRate = new VideoFrameRate(25), 
    
         // Establecer a true para capturar el cursor del ratón
        AllowCaptureMouseCursor = true
    };
    ```
    
    Cuando `FullScreen` está establecido a `true`, las propiedades `Left`, `Top`, `Width` y `Height` son ignoradas, y se captura la pantalla completa especificada por `DisplayIndex`.
    
    Para configuraciones de múltiples monitores, la propiedad `DisplayIndex` identifica qué monitor capturar, con 0 representando la pantalla principal.
    

=== "VideoCaptureCoreX"

    
    ### Captura de Pantalla Avanzada con DirectX 11
    
    VideoCaptureCoreX proporciona una implementación más potente usando tecnología DirectX 11:
    
    ```cs
    // Índice de pantalla
    var screenID = 0;
    
    // Crear una nueva fuente de captura de pantalla usando DirectX 11
    var source = new ScreenCaptureD3D11SourceSettings(); 
    
    // Establecer la API de captura
    source.API = D3D11ScreenCaptureAPI.WGC; 
    
    // Establecer la tasa de fotogramas
    source.FrameRate = new VideoFrameRate(25);
    
    // Establecer el área de pantalla o modo de pantalla completa
    if (fullscreen)
    {
        // Enumerar todas las pantallas y establecer el área de pantalla
        for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
        {
            if (i == screenID)
            {
                source.Rectangle = new VisioForge.Core.Types.Rect(System.Windows.Forms.Screen.AllScreens[i].Bounds);
            }
        }
    }
    else
    {
        // Establecer el área de pantalla
        source.Rectangle = new VisioForge.Core.Types.Rect(0, 0, 1280, 720); 
    }
    
    // Establecer a true para capturar el cursor del ratón
    source.CaptureCursor = true; 
    
    // Establecer el índice del monitor
    source.MonitorIndex = screenID; 
    
    // Establecer la fuente de captura de pantalla
    VideoCapture1.Video_Source = source; 
    ```
    
    La opción de API Windows Graphics Capture (WGC) proporciona excelente rendimiento en Windows 10 y superior. Este enfoque también demuestra el uso de `System.Windows.Forms.Screen.AllScreens` para determinar programáticamente los límites de las pantallas disponibles.
    


## Implementación de Captura de Ventana

Capturar ventanas de aplicación específicas permite la grabación dirigida de aplicaciones individuales sin incluir otro contenido del escritorio. Esto es particularmente útil para:

- Tutoriales específicos de aplicación
- Demos de software
- Escenarios de soporte donde solo una aplicación es relevante

=== "VideoCaptureCore"

    
    ### Captura de Ventana Básica
    
    Para capturar una ventana específica con VideoCaptureCore:
    
    ```csharp
    // Establecer ajustes de fuente de captura de pantalla
    VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
    {
        // Deshabilitar captura de pantalla completa
        FullScreen = false, 
    
        // Establecer el handle de la ventana
        WindowHandle = windowHandle, 
    
         // Establecer la tasa de fotogramas
        FrameRate = new VideoFrameRate(25),
    
         // Establecer a true para capturar el cursor del ratón
        AllowCaptureMouseCursor = true
    };
    ```
    
    El parámetro `windowHandle` debe contener un handle válido a la ventana objetivo. Esto puede obtenerse usando funciones de la API de Windows como `FindWindow` o usando bibliotecas de automatización de UI.
    

=== "VideoCaptureCoreX"

    
    ### Captura de Ventana Mejorada
    
    VideoCaptureCoreX proporciona una implementación optimizada de captura de ventana:
    
    ```cs
    // Crear fuente Direct3D11
    var source = new ScreenCaptureD3D11SourceSettings();
    
    // Establecer la API de captura
    source.API = D3D11ScreenCaptureAPI.WGC; 
    
    // Establecer tasa de fotogramas
    source.FrameRate = new VideoFrameRate(25);
    
    // Establecer el handle de la ventana
    source.WindowHandle = windowHandle;
    
    VideoCapture1.Video_Source = source; // Establecer la fuente de captura de pantalla
    ```
    
    La implementación DirectX 11 ofrece mejor rendimiento, particularmente para capturar aplicaciones que usan aceleración por hardware.
    


## Técnicas de Optimización de Rendimiento

Optimizar el rendimiento de captura de pantalla es crucial para mantener altas tasas de fotogramas mientras se minimiza el uso de CPU y memoria. Considera las siguientes mejores prácticas:

### Gestión de Tasa de Fotogramas

Selecciona cuidadosamente una tasa de fotogramas apropiada basada en los requisitos de tu aplicación:

- Para grabación de propósito general: 15-30 FPS es típicamente suficiente
- Para juegos o contenido con mucho movimiento: 30-60 FPS puede ser necesario
- Para contenido estático o basado en documentos: 5-10 FPS puede reducir significativamente el uso de recursos

### Consideraciones de Resolución

Las capturas de mayor resolución requieren más poder de procesamiento y memoria. Considera:

- Capturar a menor resolución y escalar hacia arriba si es apropiado
- Usar captura de región en lugar de pantalla completa cuando solo parte de la pantalla es relevante
- Implementar cambio de resolución basado en el tipo de contenido

### Aceleración por Hardware

Cuando esté disponible, usar DirectX 11/12 con aceleración por hardware puede mejorar significativamente el rendimiento:

- Reduce la carga de CPU aprovechando la GPU
- Proporciona mejores tasas de fotogramas, especialmente con contenido de alta resolución
- Permite codificación más eficiente cuando se combina con codificadores de video acelerados por hardware

## Escenarios de Implementación Avanzada

### Configuración Multi-Monitor

Trabajar con configuraciones de múltiples monitores requiere consideración especial:

```csharp
// Detectar todos los monitores disponibles
var screens = System.Windows.Forms.Screen.AllScreens;

// Crear una lista para presentar al usuario
var screenOptions = new List<string>();
for (int i = 0; i < screens.Length; i++)
{
    screenOptions.Add($"Monitor {i+1}: {screens[i].Bounds.Width} x {screens[i].Bounds.Height}");
}

// Una vez hecha la selección, establecer el DisplayIndex/MonitorIndex apropiado
```

### Selección de Ventana de Aplicación

Proporcionar a los usuarios la capacidad de seleccionar una ventana para capturar:

```csharp
// Obtener todas las ventanas abiertas
var openWindows = GetOpenWindows(); // La implementación depende de tu enfoque

// Presentar la lista al usuario para selección
// Una vez seleccionada, obtener el handle de la ventana

// Configurar la captura con el handle de ventana seleccionado
VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
{
    WindowHandle = selectedWindowHandle,
    // Configuración adicional...
};
```

### Selección de Región Dinámica

Permitir a los usuarios seleccionar interactivamente una región de pantalla para capturar:

```csharp
// Crear un formulario con fondo transparente
var selectionForm = new Form
{
    FormBorderStyle = FormBorderStyle.None,
    WindowState = FormWindowState.Maximized,
    Opacity = 0.3,
    BackColor = Color.Black
};

// Agregar manejadores de eventos de ratón para rastrear el rectángulo de selección
// Una vez completada la selección

// Configurar captura con la región seleccionada
VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
{
    Left = selection.Left,
    Top = selection.Top,
    Width = selection.Width,
    Height = selection.Height,
    // Configuración adicional...
};
```

## Solución de Problemas Comunes

### Captura en Blanco o Negra

Si el contenido capturado aparece en blanco o negro:

- Verifica que tienes permisos apropiados para la ventana o pantalla
- Comprueba si la aplicación usa aceleración por hardware que podría conflictuar con la captura
- Prueba versiones alternativas de DirectX (9 vs 11/12)
- Para contenido protegido (como video DRM), la captura puede estar bloqueada por mecanismos de seguridad

### Problemas de Rendimiento

Si experimentas captura lenta o con interrupciones:

- Reduce la resolución de captura y/o tasa de fotogramas
- Usa DirectX 11/12 en lugar de DirectX 9 cuando esté disponible
- Cierra aplicaciones de fondo innecesarias
- Verifica que la aceleración por hardware está habilitada cuando sea aplicable

## Conclusión

La funcionalidad de captura de pantalla permite a los desarrolladores crear aplicaciones potentes para demostración, educación, soporte y entretenimiento. Video Capture SDK .Net proporciona un framework robusto para implementar esta funcionalidad con mínimo esfuerzo de desarrollo.

Aprovechando las opciones de configuración apropiadas para tus requisitos específicos, puedes implementar características de captura de pantalla de alto rendimiento en tus aplicaciones .NET.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más muestras de código.
