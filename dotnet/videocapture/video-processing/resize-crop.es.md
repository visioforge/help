---
title: Redimensionar y Recortar Video en .NET
description: Redimensionamiento y recorte de video profesional en .NET con ejemplos optimizados para webcams, capturas de pantalla y cámaras IP.
---

# Operaciones de Redimensionar y Recortar Video para Desarrolladores .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al Procesamiento de Video

Al trabajar con flujos de video en aplicaciones .NET, controlar las dimensiones y área de enfoque de tu video es esencial para crear aplicaciones profesionales. Esta guía explica cómo implementar operaciones de redimensionar y recortar en flujos de video de webcams, capturas de pantalla, cámaras IP y otras fuentes.

## Implementación de Redimensionamiento de Video

Redimensionar te permite estandarizar las dimensiones de video a través de diferentes fuentes de video, lo cual es particularmente útil cuando trabajas con múltiples entradas de cámara o cuando apuntas a formatos de salida específicos.

### Paso 1: Habilitar Funcionalidad de Redimensionamiento

Primero, habilita la característica de redimensionar o recortar en tu aplicación:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Paso 2: Configurar Parámetros de Redimensionamiento

Establece tu ancho y alto deseados, y determina si mantener la relación de aspecto con letterboxing:

```cs
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width = 640,
    Height = 480,
    LetterBox = true
};
```

### Paso 3: Seleccionar Algoritmo de Redimensionamiento Apropiado

Elige el algoritmo que mejor se ajuste a tus requisitos de rendimiento y calidad:

```cs
switch (cbModoRedimensionar.SelectedIndex)
{
  case 0: VideoCapture1.Video_Resize.Mode = VideoResizeMode.NearestNeighbor; 
          break;
  case 1: VideoCapture1.Video_Resize.Mode = VideoResizeMode.Bilinear; 
          break;
  case 2: VideoCapture1.Video_Resize.Mode = VideoResizeMode.Bicubic; 
          break;
  case 3: VideoCapture1.Video_Resize.Mode = VideoResizeMode.Lancroz; 
          break;
}
```

### Algoritmos de Redimensionamiento

| Algoritmo | Velocidad | Calidad | Mejor Uso |
|-----------|-----------|---------|-----------|
| NearestNeighbor | Más rápido | Más baja | Gráficos pixelados, rendimiento crítico |
| Bilinear | Rápido | Buena | Balance general entre velocidad y calidad |
| Bicubic | Medio | Muy buena | Fotos, video de alta calidad |
| Lanczos | Más lento | Mejor | Producción profesional de video |

## Implementación de Recorte de Video

Recortar te permite enfocarte en regiones específicas de interés en tu alimentación de video, eliminando áreas no deseadas del fotograma.

### Paso 1: Habilitar Funcionalidad de Recorte

Similar a redimensionar, primero habilita la funcionalidad de recorte:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Paso 2: Definir Región de Recorte

Especifica la región de recorte estableciendo los márgenes a eliminar de cada borde del fotograma de video:

```cs
// Recortar 40 píxeles de izquierda y derecha, 0 de arriba y abajo
VideoCapture1.Video_Crop = new VideoCropSettings(40, 0, 40, 0);
```

### Parámetros de VideoCropSettings

El constructor `VideoCropSettings` acepta cuatro parámetros:
- **Izquierda**: Píxeles a eliminar del borde izquierdo
- **Superior**: Píxeles a eliminar del borde superior
- **Derecha**: Píxeles a eliminar del borde derecho
- **Inferior**: Píxeles a eliminar del borde inferior

```cs
// Ejemplo: Recortar letterbox de video 16:9
VideoCapture1.Video_Crop = new VideoCropSettings(0, 60, 0, 60);
```

## Combinar Redimensionar y Recortar

Puedes combinar ambas operaciones para primero recortar a una región de interés y luego redimensionar a las dimensiones de salida deseadas:

```cs
// Habilitar procesamiento
VideoCapture1.Video_ResizeOrCrop_Enabled = true;

// Primero recortar la imagen
VideoCapture1.Video_Crop = new VideoCropSettings(50, 30, 50, 30);

// Luego redimensionar al tamaño de salida deseado
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width = 1280,
    Height = 720,
    LetterBox = false,
    Mode = VideoResizeMode.Bicubic
};
```

## Consideraciones de Rendimiento

Al implementar operaciones de redimensionar y recortar en aplicaciones de producción, considera lo siguiente:

### Uso de CPU
- NearestNeighbor usa mínima CPU pero produce calidad más baja
- Lanczos proporciona mejor calidad pero requiere más potencia de procesamiento
- Considera aceleración de hardware si está disponible

### Uso de Memoria
- Resoluciones más grandes requieren más memoria
- El recorte reduce los requisitos de memoria al reducir el tamaño del fotograma
- Considera liberar recursos no utilizados

### Latencia
- Algoritmos más complejos añaden latencia al pipeline de video
- Para aplicaciones en tiempo real, prefiere algoritmos más rápidos
- Prueba rendimiento en hardware objetivo

## Ejemplo de Código Completo

```cs
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;

public class VideoProcessingExample
{
    private VideoCaptureCore videoCapture;
    
    public void ConfigureVideoProcessing()
    {
        // Crear instancia de captura de video
        videoCapture = new VideoCaptureCore();
        
        // Configurar fuente de video
        // ...
        
        // Habilitar redimensionar/recortar
        videoCapture.Video_ResizeOrCrop_Enabled = true;
        
        // Configurar recorte (eliminar barras negras)
        videoCapture.Video_Crop = new VideoCropSettings(0, 60, 0, 60);
        
        // Configurar redimensionamiento
        videoCapture.Video_Resize = new VideoResizeSettings
        {
            Width = 1920,
            Height = 1080,
            LetterBox = false,
            Mode = VideoResizeMode.Bicubic
        };
    }
}
```

## Aplicaciones de Ejemplo

Explora estas aplicaciones de ejemplo para ver operaciones de redimensionar y recortar en acción:

- [Demo Principal de Video Capture (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
- [Demo de Procesamiento de Video (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.