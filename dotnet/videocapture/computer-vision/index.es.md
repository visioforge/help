---
title: Visión por Computadora con Video Capture SDK
description: Integración de OpenCV y detección de objetos con Video Capture SDK .Net. Construye apps de visión artificial y machine learning avanzadas.
---

# Visión por Computadora - Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección cubre las capacidades de visión por computadora disponibles en Video Capture SDK .Net, incluyendo integración con OpenCV y detección de objetos.

## Características

- Integración con OpenCV
- Detección de códigos de barras y QR
- Detección de rostros
- Detección de movimiento
- Procesamiento de imágenes en tiempo real

## Detección de Códigos de Barras

El SDK puede detectar y decodificar varios tipos de códigos:

- Códigos QR
- Códigos de barras 1D (UPC, EAN, Code 128, etc.)
- Data Matrix
- PDF417

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Bloque detector de códigos de barras
var barcodeDetector = new BarcodeDetectorBlock(new BarcodeDetectorSettings
{
    DetectionInterval = TimeSpan.FromMilliseconds(500)
});

barcodeDetector.OnBarcodeDetected += (sender, e) =>
{
    Console.WriteLine($"Código detectado: {e.Type} - {e.Value}");
};

pipeline.Connect(videoSource.Output, barcodeDetector.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(barcodeDetector.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Detección de Rostros

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Bloque detector de rostros
var faceDetector = new FaceDetectorBlock(new FaceDetectorSettings
{
    DetectionInterval = TimeSpan.FromMilliseconds(100),
    MinFaceSize = 50
});

faceDetector.OnFaceDetected += (sender, e) =>
{
    foreach (var face in e.Faces)
    {
        Console.WriteLine($"Rostro en: ({face.X}, {face.Y}) - {face.Width}x{face.Height}");
    }
};

pipeline.Connect(videoSource.Output, faceDetector.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(faceDetector.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Integración con OpenCV

Para procesamiento personalizado, puede integrar OpenCV directamente:

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new SystemVideoSourceBlock(videoSettings);

// Capturador de muestras para acceder a los fotogramas
var sampleGrabber = new VideoSampleGrabberBlock();

sampleGrabber.OnVideoFrameBuffer += (sender, e) =>
{
    // Convertir a Mat de OpenCV
    using var mat = new Mat(e.Height, e.Width, MatType.CV_8UC3);
    Marshal.Copy(e.Data, 0, mat.Data, e.Data.Length);
    
    // Procesar con OpenCV
    Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
    Cv2.GaussianBlur(mat, mat, new Size(5, 5), 0);
    
    // Detectar bordes
    using var edges = new Mat();
    Cv2.Canny(mat, edges, 100, 200);
    
    // Usar resultado...
};

pipeline.Connect(videoSource.Output, sampleGrabber.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(sampleGrabber.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Detección de Movimiento

Ver la sección dedicada de [Detección de Movimiento](../motion-detection/index.md) para más detalles.

## Consideraciones de Rendimiento

1. **Frecuencia de detección**: No procese cada fotograma
2. **Resolución**: Considere reducir resolución para detección
3. **GPU**: Use aceleración por hardware cuando esté disponible
4. **Multi-threading**: Procese en hilos separados para no bloquear el pipeline
