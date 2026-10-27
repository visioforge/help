---
title: Video Sample Grabber — Extraer Frames RAW en C# .NET
description: Extraiga frames de video RAW de Video Capture, Media Player y Video Edit SDKs con acceso a memoria administrada y conversión de bitmap en C#.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Editing
  - C#
  - NuGet
primary_api_classes:
  - VideoSampleGrabberBlock
  - VideoFrameXBufferEventArgs
  - VideoFrameBitmapEventArgs
  - VideoFrameSKBitmapEventArgs
  - VideoFrameBufferEventArgs

---

# Uso del capturador de muestras de video

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Obtención de cuadros de video RAW como puntero de memoria no administrada dentro de la estructura

=== "Motores X"

    
    ```csharp
    // Suscribirse al evento de buffer de cuadro de video
    VideoCapture1.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
    {
        // Procesar el objeto VideoFrameX
        ProcessFrame(e.Frame);
    }
    
    // Ejemplo de procesamiento de un cuadro VideoFrameX - ajustando brillo
    private void ProcessFrame(VideoFrameX frame)
    {
        // Solo procesar formatos RGB/BGR/RGBA/BGRA
        if (frame.Format != VideoFormatX.RGB && 
            frame.Format != VideoFormatX.BGR && 
            frame.Format != VideoFormatX.RGBA && 
            frame.Format != VideoFormatX.BGRA)
        {
            return;
        }
        
        // Obtener los datos como un array de bytes para manipulación
        byte[] data = frame.ToArray();
        
        // Determinar el tamaño del píxel basándose en el formato
        int pixelSize = (frame.Format == VideoFormatX.RGB || frame.Format == VideoFormatX.BGR) ? 3 : 4;
        
        // Factor de brillo (1.2 = 20% más brillante, 0.8 = 20% más oscuro)
        float brightnessFactor = 1.2f;

        // Precalcular una tabla de búsqueda para que el bucle interno sea una consulta de tabla, no una multiplicación
        var lut = new byte[256];
        for (int v = 0; v < 256; v++)
        {
            lut[v] = (byte)Math.Min(255, (int)(v * brightnessFactor));
        }

        // Las filas se rellenan con una longitud de 4 bytes (stride): recorrer fila por fila, nunca data.Length
        int stride = frame.Stride;
        for (int y = 0; y < frame.Height; y++)
        {
            int row = y * stride;
            for (int x = 0; x < frame.Width; x++)
            {
                int pixel = row + x * pixelSize;
                data[pixel] = lut[data[pixel]];
                data[pixel + 1] = lut[data[pixel + 1]];
                data[pixel + 2] = lut[data[pixel + 2]];
            }
        }
        
        // Copiar los datos modificados de vuelta al cuadro
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    

=== "Motores clásicos"

    
    ```csharp
    // Suscribirse al evento de buffer de cuadro de video
    VideoCapture1.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
    {
        // Procesar la estructura VideoFrame
        ProcessFrame(e.Frame);
        
        // Si ha modificado el cuadro y quiere actualizar el flujo de video
        e.UpdateData = true;
    }
    
    // Ejemplo de procesamiento de un VideoFrame - ajustando brillo
    private void ProcessFrame(VideoFrame frame)
    {
        // Solo procesar formato RGB para este ejemplo
        if (frame.Info.Colorspace != RAWVideoColorSpace.RGB24)
        {
            return;
        }
        
        // Obtener los datos como un array de bytes para manipulación
        byte[] data = frame.ToArray();
        
        // Factor de brillo (1.2 = 20% más brillante, 0.8 = 20% más oscuro)
        float brightnessFactor = 1.2f;

        // Precalcular una tabla de búsqueda para que el bucle interno sea una consulta de tabla, no una multiplicación
        var lut = new byte[256];
        for (int v = 0; v < 256; v++)
        {
            lut[v] = (byte)Math.Min(255, (int)(v * brightnessFactor));
        }

        // Las filas RGB24 se rellenan con una longitud de 4 bytes (stride): recorrer fila por fila, nunca data.Length
        int stride = frame.Info.Stride;
        int rowBytes = frame.Info.Width * 3;
        for (int y = 0; y < frame.Info.Height; y++)
        {
            int row = y * stride;
            for (int i = 0; i < rowBytes; i++)
            {
                data[row + i] = lut[data[row + i]];
            }
        }
        
        // Copiar los datos modificados de vuelta al cuadro
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    

=== "Media Blocks SDK"

    
    ```csharp
    // Crear y configurar bloque capturador de muestras de video
    var videoSampleGrabberBlock = new VideoSampleGrabberBlock(VideoFormatX.RGB);
    // Necesario cuando el controlador escribe en el cuadro
    videoSampleGrabberBlock.AllowFrameModification = true;
    videoSampleGrabberBlock.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
    {
        // Procesar el objeto VideoFrameX
        ProcessFrame(e.Frame);
    }
    
    // Ejemplo de procesamiento de un cuadro VideoFrameX - ajustando brillo
    private void ProcessFrame(VideoFrameX frame)
    {
        if (frame.Format != VideoFormatX.RGB)
        {
            return;
        }
        
        // Obtener los datos como un array de bytes para manipulación
        byte[] data = frame.ToArray();
        
        // Factor de brillo (1.2 = 20% más brillante, 0.8 = 20% más oscuro)
        float brightnessFactor = 1.2f;

        // Precalcular una tabla de búsqueda para que el bucle interno sea una consulta de tabla, no una multiplicación
        var lut = new byte[256];
        for (int v = 0; v < 256; v++)
        {
            lut[v] = (byte)Math.Min(255, (int)(v * brightnessFactor));
        }

        // Las filas RGB se rellenan con una longitud de 4 bytes (stride): recorrer fila por fila, nunca data.Length
        int stride = frame.Stride;
        int rowBytes = frame.Width * 3;
        for (int y = 0; y < frame.Height; y++)
        {
            int row = y * stride;
            for (int i = 0; i < rowBytes; i++)
            {
                data[row + i] = lut[data[row + i]];
            }
        }
        
        // Copiar los datos modificados de vuelta al cuadro
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    


!!! note "Prefiera el efecto de brillo integrado para un brillo simple"

    El brillo es un efecto de vídeo integrado: llame a `VideoEffects_SetBrightness` (valores de -1.0 a 1.0) en los motores clásicos, o añada un `VideoBalanceBlock` con `VideoBalanceVideoEffect.Brightness` establecido en una pipeline de Media Blocks. Ambos se ejecutan de forma nativa y son más rápidos que el procesamiento por cuadro en código administrado. Utilice el sample grabber para el procesamiento personalizado por cuadro que los efectos integrados no cubren.

## Trabajando con cuadros de bitmap

Si necesita trabajar con objetos Bitmap administrados en lugar de punteros de memoria sin procesar, puede usar el evento `OnVideoFrameBitmap` de las clases `core` o el SampleGrabberBlock. Los motores X y el SampleGrabberBlock entregan una copia del cuadro, por lo que los cambios en ese bitmap solo llegan al flujo de video con los motores clásicos, y solo si establece `e.UpdateData = true`:

```csharp
// Suscribirse al evento de cuadro de bitmap
VideoCapture1.OnVideoFrameBitmap += OnVideoFrameBitmap;

private void OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Procesar el objeto Bitmap
    ProcessBitmap(e.Frame);
    
    // Si ha modificado el bitmap y quiere actualizar el flujo de video
    e.UpdateData = true;
}

// Copia reutilizable del fotograma
private Bitmap _source;

// Ejemplo de procesamiento de un Bitmap - ajustando brillo
private void ProcessBitmap(Bitmap bitmap)
{
    // Usar métodos de Bitmap o Graphics para manipular la imagen
    // Este ejemplo usa ColorMatrix para ajuste de brillo
    
    // GDI+ no puede dibujar un bitmap sobre su propio Graphics, así que primero se copia el fotograma en un búfer reutilizable
    if (_source == null || _source.Width != bitmap.Width || _source.Height != bitmap.Height)
    {
        _source?.Dispose();
        _source = new Bitmap(bitmap.Width, bitmap.Height);
    }

    using (Graphics copy = Graphics.FromImage(_source))
    {
        copy.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
    }

    // Crear un objeto graphics desde el bitmap
    using (Graphics g = Graphics.FromImage(bitmap))
    {
        // Crear una matriz de color para ajuste de brillo
        float brightnessFactor = 1.2f; // 1.0 = sin cambio, >1.0 = más brillante, <1.0 = más oscuro
        ColorMatrix colorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {brightnessFactor, 0, 0, 0, 0},
            new float[] {0, brightnessFactor, 0, 0, 0},
            new float[] {0, 0, brightnessFactor, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });
        
        // Crear un objeto ImageAttributes y establecer la matriz de color
        using (ImageAttributes attributes = new ImageAttributes())
        {
            attributes.SetColorMatrix(colorMatrix);
            
            // Dibujar la imagen con el ajuste de brillo
            g.DrawImage(_source, 
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                0, 0, bitmap.Width, bitmap.Height,
                GraphicsUnit.Pixel, attributes);
        }
    }
}
```

## Trabajando con SkiaSharp para aplicaciones multiplataforma

Para aplicaciones multiplataforma, el VideoSampleGrabberBlock proporciona la capacidad de trabajar con SkiaSharp, una API de gráficos 2D de alto rendimiento para .NET. Esto es especialmente útil para aplicaciones que apuntan a múltiples plataformas incluyendo móviles y web.

### Usando el evento OnVideoFrameSKBitmap

```csharp
// Primero, agregar el paquete NuGet SkiaSharp a su proyecto
// Install-Package SkiaSharp

// Importar namespaces necesarios
using SkiaSharp;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.Events;

// Crear un VideoSampleGrabberBlock con formato RGBA o BGRA
// Nota: El evento OnVideoFrameSKBitmap solo funciona con formatos RGBA o BGRA
var videoSampleGrabberBlock = new VideoSampleGrabberBlock(VideoFormatX.BGRA);

// Habilitar la propiedad SaveLastFrame si quiere tomar capturas después
videoSampleGrabberBlock.SaveLastFrame = true;

// Suscribirse al evento de bitmap SkiaSharp
videoSampleGrabberBlock.OnVideoFrameSKBitmap += OnVideoFrameSKBitmap;

// Manejador de evento para cuadros de bitmap SkiaSharp
private void OnVideoFrameSKBitmap(object sender, VideoFrameSKBitmapEventArgs e)
{
    // Procesar el SKBitmap
    ProcessSKBitmap(e.Frame);
    
    // Nota: A diferencia de VideoFrameBitmapEventArgs, VideoFrameSKBitmapEventArgs no tiene
    // una propiedad UpdateData ya que está diseñado solo para visualización/análisis de cuadros
}

// Ejemplo de procesamiento de un SKBitmap - ajustando brillo
private void ProcessSKBitmap(SKBitmap bitmap)
{
    // Crear un nuevo bitmap para contener la imagen procesada
    using (var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height)))
    {
        var canvas = surface.Canvas;
        
        // Configurar un paint con un filtro de color para ajuste de brillo
        using (var paint = new SKPaint())
        {
            // Crear un filtro de brillo (1.2 = 20% más brillante)
            float brightnessFactor = 1.2f;
            var colorMatrix = new float[]
            {
                brightnessFactor, 0, 0, 0, 0,
                0, brightnessFactor, 0, 0, 0,
                0, 0, brightnessFactor, 0, 0,
                0, 0, 0, 1, 0
            };
            
            paint.ColorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
            
            // Dibujar el bitmap original con el filtro de brillo aplicado
            canvas.DrawBitmap(bitmap, 0, 0, paint);
            
            // Si necesita obtener el resultado como un nuevo SKBitmap:
            var processedImage = surface.Snapshot();
            using (var processedBitmap = SKBitmap.FromImage(processedImage))
            {
                // Usar processedBitmap para operaciones adicionales o visualización
                // Por ejemplo, mostrarlo en una vista SkiaSharp
                // mySkiaView.SKBitmap = processedBitmap.Copy();
            }
        }
    }
}
```

### Tomando capturas con SkiaSharp

```csharp
// Crear un método para capturar y guardar una captura
private void CaptureSnapshot(string filePath)
{
    // Asegúrese de que SaveLastFrame fue habilitado en el VideoSampleGrabberBlock
    if (videoSampleGrabberBlock.SaveLastFrame)
    {
        // Obtener el último cuadro como un SKBitmap
        using (var bitmap = videoSampleGrabberBlock.GetLastFrameAsSKBitmap())
        {
            if (bitmap != null)
            {
                // Guardar el bitmap en un archivo
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }
}
```

### Ventajas de usar SkiaSharp

1. **Compatibilidad multiplataforma**: Funciona en Windows, macOS, Linux, iOS, Android y WebAssembly
2. **Rendimiento**: Proporciona procesamiento de gráficos de alto rendimiento
3. **API moderna**: Ofrece un conjunto completo de funciones de dibujo, filtrado y transformación
4. **Eficiencia de memoria**: Gestión de memoria más eficiente comparada con System.Drawing
5. **Sin dependencias de plataforma**: Sin dependencia de bibliotecas de imagen específicas de plataforma

## Información de procesamiento de cuadros

Puede obtener cuadros de video de fuentes en vivo o archivos usando los eventos `OnVideoFrameBuffer` y `OnVideoFrameBitmap`.

El evento `OnVideoFrameBuffer` es más rápido y proporciona el puntero de memoria no administrada para el cuadro decodificado. El evento `OnVideoFrameBitmap` es más lento, pero obtiene el cuadro decodificado como objeto de la clase `Bitmap`.

### Entendiendo los objetos de cuadro

- **VideoFrameX** (Motores X): Contiene datos del cuadro, dimensiones, formato, marca de tiempo y métodos para manipular datos de video sin procesar
- **VideoFrame** (Motores clásicos): Estructura similar pero con un diseño de memoria diferente
- **Propiedades comunes**:
  - Width/Height: Dimensiones del cuadro
  - Format/Colorspace: Formato de píxel (RGB, BGR, RGBA, etc.)
  - Stride: Número de bytes por línea de escaneo
  - Timestamp: Posición del cuadro en la línea de tiempo del video
  - Data: Puntero a memoria no administrada con datos de píxel

### Consideraciones importantes

1. El formato de píxel del cuadro afecta cómo procesa los datos:
   - RGB/BGR: 3 bytes por píxel
   - RGBA/BGRA/ARGB: 4 bytes por píxel (con canal alfa)
   - Formatos YUV: Diferentes arreglos de componentes

2. Con los motores clásicos, establezca `e.UpdateData = true` si ha modificado los datos del cuadro y quiere que los cambios sean visibles en el flujo de video. El Media Blocks SDK y los motores X no leen `UpdateData`.

3. Establezca `AllowFrameModification = true` en el `VideoSampleGrabberBlock` (Media Blocks SDK) antes de iniciar la canalización, o en el `CustomVideoSampleGrabberEffect` (Media Blocks SDK y motores X) antes de agregar el efecto. Sin ello, el cuadro se asigna solo para lectura y sus cambios pueden llegar a otra rama de un `TeeBlock` que comparte el mismo cuadro.

4. Para procesamiento que requiere múltiples cuadros u operaciones complejas, considere usar un buffer o cola para almacenar cuadros.

5. Al usar `OnVideoFrameSKBitmap`, seleccione RGBA o BGRA como el formato del cuadro al crear el VideoSampleGrabberBlock.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.