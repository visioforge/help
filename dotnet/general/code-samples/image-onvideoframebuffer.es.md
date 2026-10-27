---
title: Procesar Frames de Video en C# .NET — OnVideoFrameBuffer
description: Acceda al buffer de frames de video en C# / .NET mediante OnVideoFrameBuffer. Modifique píxeles, dibuje imágenes, aplique mezclas personalizadas.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Encoding
  - C#
primary_api_classes:
  - VideoFrameXBufferEventArgs
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - VideoCaptureCore
  - VideoFrameBufferEventArgs

---

# Dibujo de Imágenes con OnVideoFrameBuffer en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

El evento `OnVideoFrameBuffer` da acceso directo, a nivel de píxel, a cada frame de video conforme pasa por el pipeline. Los handlers reciben el buffer crudo y pueden inspeccionar, modificar o reescribir píxeles antes de que el frame continúe a la siguiente etapa (preview, encoder, salida a archivo). Dibujar una imagen sobre el frame — para una marca de agua, logo, overlay de depuración o anotación de visión por computadora — es el caso de uso más común y el que recorre esta guía.

!!! tip "¿Busca la función de overlay de alto nivel?"
    Si solo necesita colocar una imagen estática o animada sobre el video (PNG / JPG / GIF / BMP), use el [efecto de image overlay](../video-effects/image-overlay.md) dedicado — una línea de código mediante `Video_Effects_Add(new VideoEffectImageLogo(...))`. Use `OnVideoFrameBuffer` (esta página) cuando necesite **control a nivel de píxel**: modos de blending personalizados, lógica por frame, anotaciones de CV o integración con bibliotecas de imagen de terceros.

### Motores soportados

El evento `OnVideoFrameBuffer` está expuesto en ambas familias de motores:

| Motor | Tipo de event args | Formato de píxel |
|---|---|---|
| `VideoCaptureCore` (DirectShow, Windows) | `VideoFrameBufferEventArgs` | RGB24 / RGB32 |
| `VideoCaptureCoreX` (GStreamer, multiplataforma) | `VideoFrameXBufferEventArgs` | RGBA |
| `MediaPlayerCoreX` (GStreamer, multiplataforma) | `VideoFrameXBufferEventArgs` | RGB (RGBA cuando `OnVideoFrameUnity` está suscrito) |

Ambos motores siguen el mismo patrón — suscribirse al evento, leer `e.Frame.Data` (un `IntPtr`) junto con `Width` / `Height` / `Stride`, y opcionalmente modificar el buffer in-place. El motor clásico solo toma los cambios si pone `e.UpdateData = true`; los motores X no leen `UpdateData`.

## Entender el proceso

Al trabajar con frames de video necesita:

1. Cargar su imagen (logo, marca de agua, etc.) en memoria.
2. Convertir la imagen a un formato de buffer compatible (RGB24/RGB32 para el motor clásico, RGBA para los motores X — compruebe `e.Frame.Format`).
3. Suscribirse al evento `OnVideoFrameBuffer`.
4. Dibujar la imagen en cada frame de video mientras se procesa.
5. Con el motor clásico, establecer `e.UpdateData = true` para que el frame modificado reemplace al original downstream.

## Ejemplo con VideoCaptureCore (DirectShow)

Revisemos la implementación paso a paso:

### Paso 1: Cargar su imagen

Primero, cargue el archivo de imagen que quiere superponer en el video:

```cs
// Carga de Bitmap desde archivo
private Bitmap logoImage = new Bitmap(@"logo24.jpg");
// También puede usar PNG con canal alfa para transparencia
//private Bitmap logoImage = new Bitmap(@"logo32.png");
```

### Paso 2: Preparar búferes de memoria

Inicializa punteros para el búfer de imagen:

```cs
// Búfer RGB24/RGB32 del logo
private IntPtr logoImageBuffer = IntPtr.Zero;
private int logoImageBufferSize = 0;
```

### Paso 3: Implementar el manejador del evento OnVideoFrameBuffer

La implementación completa del manejador de eventos:

```cs
private void VideoCapture1_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
{
    // Crear búfer de logo si no está asignado o tiene tamaño cero
    if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
    {
        if (logoImageBuffer == IntPtr.Zero)
        {
            if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
            else
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB24(logoImage.Width) * logoImage.Height;
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
        }
        else
        {
            if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;

                Marshal.FreeCoTaskMem(logoImageBuffer);
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
            else
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB24(logoImage.Width) * logoImage.Height;

                Marshal.FreeCoTaskMem(logoImageBuffer);
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
        }

        if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
        {
            BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format32bppArgb);
        }
        else
        {
            BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format24bppRgb);
        }
    }

    // Dibujar imagen — el struct VideoFrame clásico mantiene Width/Height/Stride dentro de Frame.Info
    if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
    {
        FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
            e.Frame.Info.Height, 0, 0);
    }
    else
    {
        FastImageProcessing.Draw_RGB24OnRGB24Old(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
            e.Frame.Info.Height, 0, 0);
    }

    e.UpdateData = true;
}
```

## Explicación detallada

### Gestión de memoria

El código maneja tanto formatos de imagen de 24 bits como de 32 bits. Esto es lo que sucede:

1. **Verificación de Inicialización de Búfer**: El código primero verifica si el búfer del logo necesita ser creado o recreado.

2. **Detección de Formato**: Determina si usar formato RGB24 o RGB32 basándose en la imagen cargada:
   - RGB24: Color estándar de 24 bits (8 bits cada uno para R, G, B)
   - RGB32: Color de 32 bits con canal alfa para transparencia (8 bits cada uno para R, G, B, A)

3. **Asignación de Memoria**: Asigna memoria no administrada usando `Marshal.AllocCoTaskMem()` para almacenar los datos de imagen.

4. **Conversión de Imagen**: Convierte el Bitmap a datos de píxeles crudos en el búfer asignado usando `BitmapHelper.BitmapToIntPtr()`.

### Proceso de dibujo

Una vez que el búfer está preparado, el dibujo se realiza:

1. **Dibujo Específico por Formato**: El código selecciona el método de dibujo apropiado basándose en el formato de imagen:
   - `FastImageProcessing.Draw_RGB32OnRGB24()` para imágenes de 32 bits con transparencia
   - `FastImageProcessing.Draw_RGB24OnRGB24Old()` para imágenes estándar de 24 bits (forma de 8 argumentos) o `Draw_RGB24OnRGB24S()` cuando se conocen los strides de origen/destino

2. **Parámetros de Posición**: Los parámetros `0, 0` especifican dónde dibujar la imagen (esquina superior izquierda en este ejemplo).

3. **Actualización de Frame**: Establecer `e.UpdateData = true` asegura que los datos del frame modificado se usen para visualización o procesamiento posterior.

## Ejemplo con VideoCaptureCoreX / MediaPlayerCoreX (motores X)

En los motores X multiplataforma la firma del evento cambia a `VideoFrameXBufferEventArgs` y el buffer del frame llega en el formato del grabber del motor — RGBA en `VideoCaptureCoreX`, RGB en `MediaPlayerCoreX` (RGBA cuando `OnVideoFrameUnity` está suscrito). Aplique el mismo patrón — suscribirse, inspeccionar, modificar, marcar los cambios. El ejemplo siguiente usa SkiaSharp para envolver el buffer crudo y dibujar un logo PNG encima; SkiaSharp ya es una dependencia transitiva de los motores X, así que no se necesita un paquete NuGet adicional.

```cs
using SkiaSharp;
using VisioForge.Core.Helpers;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X;

// Carga el logo una sola vez (PNG con alpha funciona bien para marcas de agua)
private SKBitmap _logo = SKBitmap.Decode(@"logo.png");

// Suscribirse después de construir VideoCaptureCoreX / MediaPlayerCoreX
_videoCapture.OnVideoFrameBuffer += VideoCapture_OnVideoFrameBuffer;

private void VideoCapture_OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
{
    if (e.Frame == null || e.Frame.Data == IntPtr.Zero)
    {
        return;
    }

    // El formato del grabber difiere por motor: VideoCaptureCoreX entrega RGBA,
    // MediaPlayerCoreX RGB (RGBA cuando OnVideoFrameUnity está suscrito). Nunca asumas BGRA.
    if (e.Frame.Format == VideoFormatX.RGB)
    {
        // El RGB de 24 bits no tiene SKColorType — convertirlo con el helper del SDK
        // (una copia por frame; el dibujo ocurre en la copia, el frame original queda intacto).
        using (var bmp = e.Frame.VideoFrameRGBToSKBitmap())
        {
            var x = bmp.Width - _logo.Width - 16;
            var y = bmp.Height - _logo.Height - 16;
            bmp.Canvas.DrawBitmap(_logo, x, y);
        }

        return;
    }

    var colorType = e.Frame.Format == VideoFormatX.BGRA ? SKColorType.Bgra8888 : SKColorType.Rgba8888;
    var info = new SKImageInfo(e.Frame.Width, e.Frame.Height, colorType, SKAlphaType.Premul);

    // Envolver el buffer crudo de 32 bits en un canvas de SkiaSharp (sin asignación extra)
    using (var pixmap = new SKPixmap(info, e.Frame.Data, e.Frame.Stride))
    using (var surface = SKSurface.Create(pixmap))
    {
        var canvas = surface.Canvas;

        // Dibujar el logo en la esquina inferior derecha con 16px de padding
        var x = e.Frame.Width - _logo.Width - 16;
        var y = e.Frame.Height - _logo.Height - 16;
        canvas.DrawBitmap(_logo, x, y);
        canvas.Flush();
    }
}
```

**Qué formato recibe.** Lea `e.Frame.Format` en lugar de asumir un formato: `VideoCaptureCoreX` entrega RGBA y `MediaPlayerCoreX` RGB (RGBA cuando `OnVideoFrameUnity` está suscrito). RGB tiene 3 bytes por píxel y no tiene un `SKColorType` equivalente, así que el ejemplo lo convierte con el helper del SDK `VideoFrameRGBToSKBitmap` (una copia extra). Si necesita otro formato (I420, NV12, BGRA), solicite un bloque de conversión de formato aguas arriba de su handler en lugar de convertir en cada frame.

**Pilas de imagen alternativas.** También puede usar `System.Drawing.Bitmap` mediante `new Bitmap(width, height, stride, pixelFormat, data)` en Windows — elija el `PixelFormat` que coincida con `e.Frame.Format` (`Format32bppArgb` para datos BGRA) —, o escrituras manuales de bytes con `Marshal.Copy` / `Span<byte>` para el control máximo. SkiaSharp es la opción recomendada en macOS / Linux / iOS / Android.

**Paridad entre motores.** Todo lo documentado en las secciones [Gestión de Memoria](#gestion-de-memoria), [Manejo de Errores](#manejo-de-errores) y [Optimización de Rendimiento](#optimizacion-de-rendimiento) aplica por igual a los motores X — el evento se dispara en un hilo de procesamiento, y el trabajo pesado debe moverse fuera para evitar descartar frames.

## Mejores prácticas para superposición de imágenes

Para rendimiento óptimo al superponer imágenes en frames de video:

1. **Gestión de Memoria**: Siempre libere la memoria asignada cuando ya no se necesite para prevenir fugas de memoria.

2. **Reutilización de Búfer**: Cree el búfer una vez y reutilícelo para frames subsiguientes en lugar de recrearlo para cada frame.

3. **Consideraciones de Tamaño de Imagen**: Use imágenes de tamaño apropiado; superponer imágenes grandes puede impactar el rendimiento.

4. **Selección de Formato**:
   - Use PNG (RGB32) cuando necesite transparencia
   - Use JPG (RGB24) cuando no se requiera transparencia (más eficiente)

5. **Cálculo de Posición**: Para posicionamiento dinámico, calcule coordenadas basándose en las dimensiones del frame. En el motor clásico (`VideoFrameBufferEventArgs`) Width/Height viven en `e.Frame.Info`; en los motores X (`VideoFrameXBufferEventArgs`) están planos en `e.Frame`.

   ```cs
   // Motor clásico — Width/Height viven en e.Frame.Info
   int xPos = e.Frame.Info.Width - logoImage.Width - 10;
   int yPos = e.Frame.Info.Height - logoImage.Height - 10;
   ```

## Manejo de errores

Al implementar esta funcionalidad, considere agregar manejo de errores:

```cs
try 
{
    // Su implementación existente
}
catch (OutOfMemoryException ex)
{
    // Manejar fallas de asignación de memoria
    Console.WriteLine("Fallo al asignar memoria: " + ex.Message);
}
catch (Exception ex)
{
    // Manejar otras excepciones
    Console.WriteLine("Error durante el procesamiento del frame: " + ex.Message);
}
finally 
{
    // Código de limpieza opcional
}
```

## Optimización de rendimiento

Para aplicaciones de alto rendimiento, considere estas optimizaciones:

1. **Pre-asignación de Búfer**: Inicialice búferes durante el inicio de la aplicación en lugar de durante el procesamiento de video.

2. **Procesamiento Condicional**: Solo procese frames que necesitan la superposición (por ejemplo, omitir procesamiento para ciertos frames).

3. **Procesamiento Paralelo**: Para operaciones complejas, considere usar técnicas de procesamiento paralelo.

## Conclusión

El evento `OnVideoFrameBuffer` da acceso directo a cada frame crudo tanto en el motor clásico `VideoCaptureCore` (RGB24/RGB32 vía `VideoFrameBufferEventArgs`) como en los motores X multiplataforma (`VideoCaptureCoreX` / `MediaPlayerCoreX`, frames del grabber vía `VideoFrameXBufferEventArgs`). Es la herramienta correcta cuando necesita control a nivel de píxel — modos de blending personalizados, anotaciones de CV por frame, o integración con bibliotecas de imagen de terceros.

Para overlays de imagen estáticos o animados sin escribir un handler por frame, el [efecto de image overlay](../video-effects/image-overlay.md) de una línea suele ser la mejor elección.

## Documentación relacionada

- [Efecto de image overlay](../video-effects/image-overlay.md) — marca de agua / logo overlay declarativo de alto nivel sin escribir callback.
- [Text overlay vía OnVideoFrameBuffer](text-onvideoframebuffer.md) — la misma técnica aplicada a texto en lugar de imágenes.
- [Dibujo de video en un PictureBox](draw-video-picturebox.md) — patrón de rendering WinForms que suele combinarse con trabajo a nivel de píxel.

---

¿Busca más ejemplos de código? Visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos y recursos adicionales.