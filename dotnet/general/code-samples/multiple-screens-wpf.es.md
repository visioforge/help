---
title: Múltiples pantallas de video en WPF para apps C# .NET
description: Cree aplicaciones de video multi-pantalla en WPF con controles Image, manejo de eventos, gestión de memoria y técnicas de optimización de rendimiento.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - WPF
  - C#
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - VideoFrameBufferEventArgs
  - MultiscreenVideoView
  - VideoView

---

# Implementación de múltiples pantallas de salida de video en aplicaciones WPF

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Al desarrollar aplicaciones WPF que requieren manejar múltiples feeds de video simultáneamente, los desarrolladores a menudo enfrentan desafíos con rendimiento, sincronización y gestión de recursos. Esta guía proporciona un enfoque integral para implementar múltiples pantallas de salida de video en sus aplicaciones WPF usando C# y el control Image.

## Comenzar con múltiples pantallas de video

Consulte la guía de instalación para WPF [aquí](../../install/index.md).

Para comenzar a implementar múltiples salidas de video en su aplicación WPF, necesitará:

1. Agregar el control Video View apropiado a su aplicación
2. Configurar el manejo de eventos para procesamiento de frames de video
3. Configurar su pipeline de renderizado para rendimiento óptimo

### Dos patrones soportados

Para mostrar el mismo feed de video en varias vistas en WPF, elija uno de estos dos patrones reales:

1. **`MultiscreenVideoView` + `OnVideoFrameBuffer`** — un motor SDK empuja cada frame a tantos controles `MultiscreenVideoView` como quiera. Úselo cuando un único motor de captura/reproducción alimente varias copias en pantalla.
2. **Un motor por `VideoView`** — cada pantalla recibe su propia instancia de `VideoCaptureCore` / `MediaPlayerCore` / `VideoEditCore` enlazada a su propio `VideoView` regular. Úselo cuando las pantallas muestren fuentes **diferentes** (p. ej., una cuadrícula de seguridad de cuatro cámaras). Ver el ejemplo [Sistema de Seguridad de Cuatro Cámaras](#ejemplo-practico-sistema-de-seguridad-de-cuatro-camaras) a continuación.

El `VisioForge.Core.UI.WPF.VideoView` regular no expone un método `RenderFrame` — es controlado automáticamente por el motor al que está enlazado mediante `CreateAsync(IVideoView)`. El reparto de frames requiere `MultiscreenVideoView`.

### Configurar su proyecto WPF para reparto de frames

Coloque uno o más controles `VisioForge.Core.UI.WPF.MultiscreenVideoView` en su ventana WPF. Déles nombres descriptivos (p. ej. `multiView1`, `multiView2`). Estos son los controles que aceptan frames empujados.

### Manejar frames de video

Suscríbase al evento `OnVideoFrameBuffer` del motor SDK. El argumento del evento transporta un `VideoFrameBufferEventArgs` que cada `MultiscreenVideoView` puede renderizar.

## Implementar el manejador de frames de video

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    multiView1.RenderFrame(e);
}
```

`MultiscreenVideoView.RenderFrame(VideoFrameBufferEventArgs)` copia el frame a su propia superficie interna, por lo que el buffer del motor puede liberarse cuando retorna el manejador.

## Técnicas de implementación avanzada

### Crear MultiscreenVideoViews dinámicos

Para aplicaciones que requieren un número variable de salidas de video, cree controles `MultiscreenVideoView` dinámicamente:

```cs
private List<VisioForge.Core.UI.WPF.MultiscreenVideoView> multiViews = 
    new List<VisioForge.Core.UI.WPF.MultiscreenVideoView>();

private void CreateMultiView(Grid container, int row, int column)
{
    var view = new VisioForge.Core.UI.WPF.MultiscreenVideoView();
    Grid.SetRow(view, row);
    Grid.SetColumn(view, column);
    
    container.Children.Add(view);
    multiViews.Add(view);
}

// Ejemplo de uso:
// CreateMultiView(mainGrid, 0, 0);
// CreateMultiView(mainGrid, 0, 1);
```

### Distribuir frames de video a múltiples vistas

Reparta los frames entrantes a cada `MultiscreenVideoView` registrado:

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Renderizar a todas las instancias MultiscreenVideoView
    foreach (var view in multiViews)
    {
        view.RenderFrame(e);
    }
}
```

## Estrategias de optimización de rendimiento

### Reducir la carga de renderizado

Para múltiples vistas de video, considere estas técnicas de optimización:

1. **Omisión de frames**: No todas las vistas necesitan actualizarse a la tasa de frames completa
2. **Ocultar vistas fuera de pantalla**: WPF omite el renderizado para controles colapsados — use `Visibility.Collapsed` en vistas no visibles

```cs
private int frameCounter;

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // La vista principal obtiene cada frame
    primaryMultiView.RenderFrame(e);
    
    // Las vistas secundarias obtienen cada segundo frame
    if (frameCounter % 2 == 0)
    {
        foreach (var view in secondaryMultiViews)
        {
            view.RenderFrame(e);
        }
    }
    
    frameCounter++;
}
```

## Ejemplo práctico: sistema de seguridad de cuatro cámaras

Aquí hay un ejemplo más completo de implementación de un sistema de seguridad de cuatro cámaras usando el motor `VideoCaptureCore` del Video Capture SDK. Cada cámara obtiene su propia instancia del motor enlazada a un `VideoView` dedicado.

```cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI.WPF;
using VisioForge.Core.VideoCapture;

public partial class SecurityMonitorWindow : Window
{
    private readonly List<VideoView> _cameraViews = new List<VideoView>();
    private readonly List<VideoCaptureCore> _cameras = new List<VideoCaptureCore>();

    public SecurityMonitorWindow()
    {
        InitializeComponent();
    }

    public async Task InitializeCamerasAsync(IEnumerable<string> deviceNames)
    {
        // Toma los primeros cuatro dispositivos si no se pasan nombres.
        var names = deviceNames?.Take(4).ToList();

        // Construye una cuadrícula 2x2 de controles VideoView emparejados con motores VideoCaptureCore.
        int i = 0;
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 2; col++, i++)
            {
                var view = new VideoView();
                Grid.SetRow(view, row);
                Grid.SetColumn(view, col);
                mainGrid.Children.Add(view);
                _cameraViews.Add(view);

                // El motor clásico usa un constructor; CreateAsync es el patrón del motor X.
                // Para VideoCaptureCoreX use `await VideoCaptureCoreX.CreateAsync(view)` en su lugar.
                var camera = new VideoCaptureCore(view);
                _cameras.Add(camera);

                if (names != null && i < names.Count)
                {
                    camera.Video_CaptureDevice = new VideoCaptureSource(names[i]);
                    camera.Video_CaptureDevice.Format_UseBest = true;
                    camera.Mode = VideoCaptureMode.VideoPreview;
                    camera.Audio_PlayAudio = false;
                    camera.Audio_RecordAudio = false;
                }
            }
        }
    }

    public async Task StartCamerasAsync()
    {
        foreach (var camera in _cameras)
        {
            await camera.StartAsync();
        }
    }

    public async Task StopCamerasAsync()
    {
        foreach (var camera in _cameras)
        {
            await camera.StopAsync();
        }

        foreach (var camera in _cameras)
        {
            camera.Dispose();
        }
    }
}
```

Enumera los dispositivos disponibles con `camera.Video_CaptureDevices()` (o su variante asíncrona) para poblar el argumento `deviceNames` en tiempo de ejecución — consulte [la guía de enumeración de dispositivos](../../videocapture/video-sources/video-capture-devices/enumerate-and-select.md).

## Solución de problemas comunes

### Manejar sincronización de frames

Si experimenta problemas de timing de frames a través de múltiples pantallas:

```cs
private readonly object syncLock = new object();

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    lock (syncLock)
    {
        foreach (var view in multiViews)
        {
            view.RenderFrame(e);
        }
    }
}
```

---
Para más ejemplos de código y técnicas de implementación avanzada, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).