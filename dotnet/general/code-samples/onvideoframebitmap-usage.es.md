---
title: Guía de Frames en Tiempo Real con OnVideoFrameBitmap
description: Accede y modifica frames de video en tiempo real con eventos OnVideoFrameBitmap para manipulación avanzada de video en aplicaciones C#.
---

# Guía de Frames en Tiempo Real con OnVideoFrameBitmap

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

El evento `OnVideoFrameBitmap` es una característica poderosa en las bibliotecas de procesamiento de video .NET que permite a los desarrolladores acceder y modificar frames de video en tiempo real. Esta guía explora las aplicaciones prácticas, técnicas de implementación y consideraciones de rendimiento al trabajar con manipulación de frames bitmap en aplicaciones C#.

## Entendiendo los Eventos OnVideoFrameBitmap

El evento `OnVideoFrameBitmap` proporciona una interfaz directa para acceder a frames de video mientras son procesados por el SDK. Esta capacidad es esencial para aplicaciones que requieren:

- Análisis de video en tiempo real
- Manipulación frame por frame
- Implementación de superposiciones dinámicas
- Efectos de video personalizados
- Integración de visión por computadora

Cuando el evento se dispara, entrega una representación bitmap del frame de video actual, permitiendo acceso y manipulación a nivel de píxel antes de que el frame continúe a través del pipeline de procesamiento.

## Implementación Básica

Para comenzar a trabajar con el evento `OnVideoFrameBitmap`, necesitarás suscribirte a él en tu código:

```csharp
// Suscribirse al evento OnVideoFrameBitmap
videoProcessor.OnVideoFrameBitmap += VideoProcessor_OnVideoFrameBitmap;

// Implementar el manejador de eventos
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // El código de manipulación de frame irá aquí
    // e.Frame contiene el frame actual como Bitmap
}
```

## Manipulando Frames de Video

### Ejemplo Simple de Superposición de Bitmap

El siguiente ejemplo demuestra cómo superponer una imagen en cada frame de video:

```csharp
Bitmap bmp = new Bitmap(@"c:\samples\pics\1.jpg");

using (Graphics g = Graphics.FromImage(e.Frame))
{
    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
    e.UpdateData = true;
}

bmp.Dispose();
```

En este código:

1. Creamos un objeto `Bitmap` desde un archivo de imagen
2. Usamos la clase `Graphics` para dibujar en el bitmap del frame
3. Establecemos `e.UpdateData = true` para informar al SDK que hemos modificado el frame
4. Liberamos nuestros recursos correctamente para prevenir fugas de memoria

> **Importante:** Siempre establece `e.UpdateData = true` cuando modifiques el bitmap del frame. Esto señala al SDK que use tu frame modificado en lugar del original.

### Agregando Superposiciones de Texto

Las superposiciones de texto se usan comúnmente para marcas de tiempo, subtítulos o visualizaciones informativas:

```csharp
using (Graphics g = Graphics.FromImage(e.Frame))
{
    // Crear un fondo semi-transparente para el texto
    using (SolidBrush brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
    {
        g.FillRectangle(brush, 10, 10, 200, 30);
    }
    
    // Agregar superposición de texto
    using (Font font = new Font("Arial", 12))
    using (SolidBrush textBrush = new SolidBrush(Color.White))
    {
        g.DrawString(DateTime.Now.ToString(), font, textBrush, new PointF(15, 15));
    }
    
    e.UpdateData = true;
}
```

## Consideraciones de Rendimiento

Al trabajar con `OnVideoFrameBitmap`, es crucial optimizar tu código para el rendimiento. Cada operación de procesamiento de frame debe completarse rápidamente para mantener una reproducción de video fluida.

### Gestión de Recursos

La gestión adecuada de recursos es esencial:

```csharp
// Enfoque de bajo rendimiento
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Bitmap overlay = new Bitmap(@"c:\logo.png");
    Graphics g = Graphics.FromImage(e.Frame);
    g.DrawImage(overlay, 0, 0);
    e.UpdateData = true;
    // ¡Fuga de memoria! Graphics y Bitmap no liberados
}

// Enfoque optimizado
private Bitmap _cachedOverlay;

private void InitializeResources()
{
    _cachedOverlay = new Bitmap(@"c:\logo.png");
}

private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    using (Graphics g = Graphics.FromImage(e.Frame))
    {
        g.DrawImage(_cachedOverlay, 0, 0);
        e.UpdateData = true;
    }
}

private void CleanupResources()
{
    _cachedOverlay?.Dispose();
}
```

### Optimizando el Tiempo de Procesamiento

Para mantener una reproducción de video fluida:

1. **Pre-computar donde sea posible**: Prepara recursos antes de que comience el procesamiento
2. **Cachear objetos usados frecuentemente**: Evita crear nuevos objetos para cada frame
3. **Procesar solo cuando sea necesario**: Agrega lógica condicional para omitir frames o realizar operaciones menos intensivas cuando sea necesario
4. **Usar operaciones de dibujo eficientes**: Elige métodos GDI+ apropiados según tus necesidades

```csharp
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Solo procesar cada segundo frame
    if (_frameCounter % 2 == 0)
    {
        using (Graphics g = Graphics.FromImage(e.Frame))
        {
            // Tu código de procesamiento de frame
            e.UpdateData = true;
        }
    }
    _frameCounter++;
}
```

## Técnicas Avanzadas de Manipulación de Frames

### Aplicando Filtros y Efectos

Puedes implementar filtros de procesamiento de imagen personalizados:

```csharp
private void ApplyGrayscaleFilter(Bitmap bitmap)
{
    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
    byte[] rgbValues = new byte[bytes];
    
    Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Procesar datos de píxeles
    for (int i = 0; i < rgbValues.Length; i += 4)
    {
        byte gray = (byte)(0.299 * rgbValues[i + 2] + 0.587 * rgbValues[i + 1] + 0.114 * rgbValues[i]);
        rgbValues[i] = gray;     // Azul
        rgbValues[i + 1] = gray; // Verde
        rgbValues[i + 2] = gray; // Rojo
    }
    
    Marshal.Copy(rgbValues, 0, ptr, bytes);
    bitmap.UnlockBits(bmpData);
}
```

## Integración con Bibliotecas de Visión por Computadora

El evento `OnVideoFrameBitmap` puede combinarse con bibliotecas populares de visión por computadora:

```csharp
// Ejemplo usando una biblioteca hipotética de visión por computadora
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Convertir bitmap al formato necesario por la biblioteca CV
    byte[] imageData = ConvertBitmapToByteArray(e.Frame);
    
    // Procesar con biblioteca CV
    var results = _computerVisionProcessor.DetectFaces(imageData, e.Frame.Width, e.Frame.Height);
    
    // Dibujar resultados de vuelta en el frame
    using (Graphics g = Graphics.FromImage(e.Frame))
    {
        foreach (var face in results)
        {
            g.DrawRectangle(new Pen(Color.Yellow, 2), face.X, face.Y, face.Width, face.Height);
        }
        
        e.UpdateData = true;
    }
}
```

## Solución de Problemas Comunes

### Fugas de Memoria

Si experimentas crecimiento de memoria durante procesamiento prolongado de video:

1. Asegúrate de que todos los objetos `Graphics` estén liberados
2. Libera correctamente cualquier objeto `Bitmap` temporal
3. Evita capturar objetos grandes en expresiones lambda

### Degradación del Rendimiento

Si el procesamiento de frames se vuelve lento:

1. Perfila tu manejador de eventos para identificar cuellos de botella
2. Considera reducir la frecuencia de procesamiento
3. Optimiza operaciones GDI+ o considera DirectX para aplicaciones críticas de rendimiento

## Integración del SDK

El evento `OnVideoFrameBitmap` está disponible en los siguientes SDKs:

## Dependencias Requeridas

Para usar la funcionalidad descrita en esta guía, necesitarás:

- Paquete de redistribución del SDK
- System.Drawing (incluido en .NET Framework)
- Soporte de Windows GDI+

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y proyectos que demuestran estas técnicas en acción.