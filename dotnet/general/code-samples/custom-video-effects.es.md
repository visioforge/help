---
title: Creación de Efectos de Video Personalizados en C#
description: Crea efectos de video personalizados en C# usando eventos OnVideoFrameBitmap y OnVideoFrameBuffer para procesamiento en tiempo real y overlays.
---

# Creación de Efectos de Video Personalizados en Tiempo Real en Aplicaciones C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al Procesamiento de Frames de Video

Al desarrollar aplicaciones de video, a menudo necesitas aplicar efectos personalizados o superposiciones a flujos de video en tiempo real. El SDK .NET proporciona dos eventos potentes para este propósito: `OnVideoFrameBitmap` y `OnVideoFrameBuffer`. Estos eventos te dan acceso directo a cada frame de video, permitiéndote modificar píxeles antes de que sean renderizados o codificados.

## Métodos de Implementación

Hay dos enfoques principales para implementar efectos de video personalizados:

1. **Usando OnVideoFrameBitmap**: Procesa frames como objetos Bitmap con GDI+ - más fácil de usar pero con rendimiento moderado
2. **Usando OnVideoFrameBuffer**: Manipula el búfer de imagen RGB24 crudo directamente - ofrece mejor rendimiento pero requiere código de más bajo nivel

## Ejemplos de Código para Efectos de Video Personalizados

### Implementación de Superposición de Texto

Añadir superposiciones de texto al video es útil para marcas de agua, mostrar información o crear subtítulos. Este ejemplo demuestra cómo añadir texto simple a tus frames de video:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Graphics grf = Graphics.FromImage(e.Frame);

    grf.DrawString("¡Hola!", new Font(FontFamily.GenericSansSerif, 20), new SolidBrush(Color.White), 20, 20);
    grf.Dispose();

    e.UpdateData = true;
}
```

### Implementación de Efecto de Escala de Grises

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

### Implementación de Ajuste de Brillo

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

### Implementación de Superposición de Marca de Tiempo

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

## Consejos de Optimización de Rendimiento

### Trabajando con Datos de Búfer Crudos

Para aplicaciones de alto rendimiento, procesar datos de búfer crudos ofrece ventajas significativas de velocidad:

```cs
// Ejemplo del evento OnVideoFrameBuffer (pseudo-código)
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Buffer contiene datos RGB24 crudos
    // Cada píxel usa 3 bytes en orden RGB
    // Procesar directamente para máximo rendimiento
}
```

### Mejores Prácticas para Procesamiento de Frames

* **Gestión de Memoria**: Siempre libera objetos Graphics y desbloquea datos de bitmap
* **Consideraciones de Rendimiento**: Para procesamiento en tiempo real, mantén las operaciones ligeras
* **Procesamiento de Búfer**: Recomendamos encarecidamente procesar datos CRUDOS en el evento OnVideoFrameBuffer para rendimiento óptimo
* **Bibliotecas Externas**: Considera usar Intel IPP u otras bibliotecas optimizadas de procesamiento de imagen para operaciones complejas

---
## Recursos Adicionales
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a más ejemplos de código y ejemplos de proyectos completos.