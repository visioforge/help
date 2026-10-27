---
title: Creación de efectos de video personalizados en C# .NET
description: Implemente efectos de video personalizados en C# con eventos OnVideoFrameBitmap y OnVideoFrameBuffer para procesamiento en tiempo real y overlays.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoFrameBitmapEventArgs
  - VideoFrameBufferEventArgs
---

# Creación de efectos de video personalizados en tiempo real en aplicaciones C#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al procesamiento de frames de video

Al desarrollar aplicaciones de video, a menudo necesita aplicar efectos personalizados o superposiciones a flujos de video en tiempo real. El SDK .NET proporciona dos eventos potentes para este propósito: `OnVideoFrameBitmap` y `OnVideoFrameBuffer`. Estos eventos le dan acceso directo a cada frame de video, permitiéndole modificar píxeles antes de que sean renderizados o codificados.

## Métodos de implementación

Hay dos enfoques principales para implementar efectos de video personalizados:

1. **Usando OnVideoFrameBitmap**: Procesa frames como objetos Bitmap con GDI+ - más fácil de usar pero con rendimiento moderado
2. **Usando OnVideoFrameBuffer**: Manipula el búfer de imagen RGB24 crudo directamente - ofrece mejor rendimiento pero requiere código de más bajo nivel

## Ejemplos de código para efectos de video personalizados

### Implementación de Superposición de Texto

Añadir superposiciones de texto al video es útil para marcas de agua, mostrar información o crear subtítulos. Este ejemplo demuestra cómo añadir texto simple a sus frames de video:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Graphics grf = Graphics.FromImage(e.Frame);

    grf.DrawString("¡Hola!", new Font(FontFamily.GenericSansSerif, 20), new SolidBrush(Color.White), 20, 20);
    grf.Dispose();

    e.UpdateData = true;
}
```

### Implementación de efecto de escala de grises

Convertir video a escala de grises es una técnica fundamental de procesamiento de imagen. Este ejemplo muestra cómo acceder y modificar valores individuales de píxeles:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Bitmap bmp = e.Frame;
    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
    byte[] rgbValues = new byte[bytes];
    
    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Aplicar fórmula estándar de luminancia (0.3R + 0.59G + 0.11B) para conversión precisa a escala de grises
    for (int i = 0; i < rgbValues.Length; i += 3)
    {
        int gray = (int)(rgbValues[i] * 0.3 + rgbValues[i + 1] * 0.59 + rgbValues[i + 2] * 0.11);
        rgbValues[i] = (byte)gray;
        rgbValues[i + 1] = (byte)gray;
        rgbValues[i + 2] = (byte)gray;
    }
    
    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
    bmp.UnlockBits(bmpData);
    
    e.UpdateData = true;
}
```

### Implementación de ajuste de brillo

Este ejemplo demuestra cómo ajustar el brillo de frames de video - un requisito común en aplicaciones de procesamiento de video:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    float brightness = 1.2f; // Valores > 1 aumentan el brillo, < 1 lo disminuyen
    
    Bitmap bmp = e.Frame;
    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
    byte[] rgbValues = new byte[bytes];
    
    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Aplicar ajuste de brillo a cada canal de color
    for (int i = 0; i < rgbValues.Length; i++)
    {
        int newValue = (int)(rgbValues[i] * brightness);
        rgbValues[i] = (byte)Math.Min(255, Math.Max(0, newValue));
    }
    
    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
    bmp.UnlockBits(bmpData);
    
    e.UpdateData = true;
}
```

### Implementación de superposición de marca de tiempo

Añadir marcas de tiempo a frames de video es esencial para aplicaciones de vigilancia y registro. Este ejemplo muestra cómo crear una marca de tiempo con aspecto profesional con un fondo semi-transparente:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Graphics grf = Graphics.FromImage(e.Frame);
    
    // Crear un fondo semi-transparente para mejor legibilidad
    Rectangle textBackground = new Rectangle(10, e.Frame.Height - 50, 250, 40);
    grf.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), textBackground);
    
    // Mostrar fecha y hora actual
    string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    grf.DrawString(dateTime, new Font(FontFamily.GenericSansSerif, 16), 
                  new SolidBrush(Color.White), 15, e.Frame.Height - 45);
    
    grf.Dispose();
    
    e.UpdateData = true;
}
```

## Consejos de optimización de rendimiento

### Trabajar con datos de búfer crudos

Para aplicaciones de alto rendimiento, procesar datos de búfer crudos ofrece ventajas significativas de velocidad:

```cs
// Ejemplo del evento OnVideoFrameBuffer (pseudo-código)
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Frame.Data es un IntPtr al búfer nativo (longitud = e.Frame.DataSize).
    // Las dimensiones y el stride están en e.Frame.Info; el layout real de píxeles
    // vive en e.Frame.Info.Colorspace (RGB24/RGB32/YUY2/NV12/...). No asumas RGB24.
    int width   = e.Frame.Info.Width;
    int height  = e.Frame.Info.Height;
    int stride  = e.Frame.Info.Stride;
    var colorspace = e.Frame.Info.Colorspace;

    // Procesa e.Frame.Data in-place para máximo rendimiento y luego fija
    // e.UpdateData = true para que el búfer modificado fluya río abajo.
}
```

### Mejores prácticas para procesamiento de frames

* **Gestión de Memoria**: Siempre libere objetos Graphics y desbloquee datos de bitmap
* **Consideraciones de Rendimiento**: Para procesamiento en tiempo real, mantenga las operaciones ligeras
* **Procesamiento de Búfer**: Recomendamos encarecidamente procesar datos CRUDOS en el evento OnVideoFrameBuffer para rendimiento óptimo
* **Bibliotecas Externas**: Considere usar Intel IPP u otras bibliotecas optimizadas de procesamiento de imagen para operaciones complejas

---
## Recursos adicionales
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a más ejemplos de código y ejemplos de proyectos completos.