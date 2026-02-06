---
title: Multi Pantallas Salida Video en Apps WPF
description: Crea aplicaciones de video multi-pantalla en WPF con controles Image, manejo de eventos, gestión de memoria y técnicas de optimización de rendimiento.
---

# Implementación de Múltiples Pantallas de Salida de Video en Aplicaciones WPF

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Al desarrollar aplicaciones WPF que requieren manejar múltiples feeds de video simultáneamente, los desarrolladores a menudo enfrentan desafíos con rendimiento, sincronización y gestión de recursos. Esta guía proporciona un enfoque integral para implementar múltiples pantallas de salida de video en tus aplicaciones WPF usando C# y el control Image.

## Comenzando con Múltiples Pantallas de Video

Consulta la guía de instalación para WPF [aquí](../../install/index.md).

Para comenzar a implementar múltiples salidas de video en tu aplicación WPF, necesitarás:

1. Agregar el control Video View apropiado a tu aplicación
2. Configurar el manejo de eventos para procesamiento de frames de video
3. Configurar tu pipeline de renderizado para rendimiento óptimo

### Configurando Tu Proyecto WPF

Primero, coloca el control `VisioForge.Core.UI.WPF.VideoView` en tu ventana WPF. Se recomienda dar a este control un nombre descriptivo, como `videoView`, para claridad en tu código. Este control servirá como tu elemento de visualización de video principal.

### Manejando Frames de Video

La clave para crear múltiples pantallas de salida es el manejo adecuado de eventos. Necesitarás suscribirte al evento "OnVideoFrameBuffer" para tu control SDK. Este evento proporciona acceso a los datos crudos del frame de video que luego puedes distribuir a múltiples elementos de visualización.

## Implementando el Manejador de Frames de Video

A continuación se muestra una implementación de ejemplo del manejador de frames de video que captura frames entrantes y los renderiza en una vista de video:

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    videoView.RenderFrame(e);
}
```

Este simple manejador recibe frames de video a través del parámetro `VideoFrameBufferEventArgs` y los pasa al método `RenderFrame` de tu control de vista de video.

## Técnicas de Implementación Avanzada

### Creando Vistas de Video Dinámicas

Para aplicaciones que requieren un número variable de salidas de video, puedes crear controles de vista de video dinámicamente:

```cs
private List<VisioForge.Core.UI.WPF.VideoView> videoViews = new List<VisioForge.Core.UI.WPF.VideoView>();

private void CreateVideoView(Grid container, int row, int column)
{
    var videoView = new VisioForge.Core.UI.WPF.VideoView();
    Grid.SetRow(videoView, row);
    Grid.SetColumn(videoView, column);
    
    container.Children.Add(videoView);
    videoViews.Add(videoView);
}

// Ejemplo de uso:
// CreateVideoView(mainGrid, 0, 0);
// CreateVideoView(mainGrid, 0, 1);
```

### Distribuyendo Frames de Video a Múltiples Vistas

Al trabajar con múltiples vistas de video, necesitas distribuir los frames entrantes a todas las vistas activas:

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Renderizar a todas las vistas de video
    foreach (var view in videoViews)
    {
        view.RenderFrame(e);
    }
}
```

### Consideraciones de Gestión de Memoria

Al trabajar con múltiples salidas de video, la gestión de memoria se convierte en una preocupación crítica. Los frames de video pueden consumir memoria significativa, especialmente en resoluciones más altas. Considera implementar un mecanismo de pooling de frames:

```cs
private ConcurrentQueue<VideoFrame> framePool = new ConcurrentQueue<VideoFrame>();
private const int MaxPoolSize = 10;

private VideoFrame GetFrameFromPool()
{
    if (framePool.TryDequeue(out var frame))
    {
        return frame;
    }
    
    return new VideoFrame();
}

private void ReturnFrameToPool(VideoFrame frame)
{
    frame.Clear();
    
    if (framePool.Count < MaxPoolSize)
    {
        framePool.Enqueue(frame);
    }
}
```

## Estrategias de Optimización de Rendimiento

### Reduciendo la Carga de Renderizado

Para múltiples vistas de video, considera estas técnicas de optimización:

1. **Resolución adaptativa**: Reduce la resolución para pantallas secundarias
2. **Omisión de frames**: No todas las vistas necesitan actualizarse a la tasa de frames completa
3. **Renderizado asíncrono**: Descarga el renderizado a hilos de fondo

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // La vista principal obtiene resolución completa, tasa de frames completa
    primaryVideoView.RenderFrame(e);
    
    // Las vistas secundarias obtienen cada segundo frame
    if (frameCounter % 2 == 0)
    {
        foreach (var view in secondaryVideoViews)
        {
            Task.Run(() => view.RenderFrameScaled(e, 0.5)); // Escalado al 50%
        }
    }
    
    frameCounter++;
}
```

## Ejemplo Práctico: Sistema de Seguridad de Cuatro Cámaras

Aquí hay un ejemplo más completo de implementación de un sistema de seguridad de cuatro cámaras:

```cs
public partial class SecurityMonitorWindow : Window
{
    private List<VisioForge.Core.UI.WPF.VideoView> cameraViews = new List<VisioForge.Core.UI.WPF.VideoView>();
    private List<VideoCapture> cameras = new List<VideoCapture>();
    
    public SecurityMonitorWindow()
    {
        InitializeComponent();
        
        // Configurar cuadrícula 2x2 de vistas de cámara
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 2; col++)
            {
                var view = new VisioForge.Core.UI.WPF.VideoView();
                Grid.SetRow(view, row);
                Grid.SetColumn(view, col);
                mainGrid.Children.Add(view);
                cameraViews.Add(view);
                
                // Crear y configurar cámara
                var camera = new VideoCapture();
                camera.OnVideoFrameBuffer += (s, e) => view.RenderFrame(e);
                cameras.Add(camera);
            }
        }
    }
    
    public async Task StartCamerasAsync()
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].VideoSource = VideoSource.CameraSource;
            cameras[i].CameraDevice = new CameraDevice(i); // Asumiendo que las cámaras están indexadas 0-3
            await cameras[i].StartAsync();
        }
    }
}
```

## Solución de Problemas Comunes

### Manejando Sincronización de Frames

Si experimentas problemas de timing de frames a través de múltiples pantallas:

```cs
private readonly object syncLock = new object();

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    lock (syncLock)
    {
        foreach (var view in videoViews)
        {
            view.RenderFrame(e);
        }
    }
}
```

---
Para más ejemplos de código y técnicas de implementación avanzada, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).