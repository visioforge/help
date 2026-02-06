---
title: Dibujo de Imágenes con OnVideoFrameBuffer en .NET
description: Superpón imágenes en frames de video con OnVideoFrameBuffer para marcas de agua, logos y elementos visuales en aplicaciones de video .NET.
---

# Dibujo de Imágenes con OnVideoFrameBuffer en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

El evento `OnVideoFrameBuffer` proporciona una manera poderosa de manipular frames de video en tiempo real. Esta guía demuestra cómo superponer imágenes en frames de video usando este evento en aplicaciones .NET. Esta técnica es útil para agregar marcas de agua, logos u otros elementos visuales al contenido de video durante el procesamiento o reproducción.

## Entendiendo el Proceso

Al trabajar con frames de video en .NET, necesitas:

1. Cargar tu imagen (logo, marca de agua, etc.) en memoria
2. Convertir la imagen a un formato de búfer compatible
3. Escuchar el evento `OnVideoFrameBuffer`
4. Dibujar la imagen en cada frame de video mientras se procesa
5. Actualizar los datos del frame para mostrar los cambios

## Implementación de Código

Revisemos la implementación paso a paso:

### Paso 1: Cargar Tu Imagen

Primero, carga el archivo de imagen que quieres superponer en el video:

```cs
// Carga de Bitmap desde archivo
private Bitmap logoImage = new Bitmap(@"logo24.jpg");
// También puedes usar PNG con canal alfa para transparencia
//private Bitmap logoImage = new Bitmap(@"logo32.png");
```

### Paso 2: Preparar Búferes de Memoria

Inicializa punteros para el búfer de imagen:

```cs
// Búfer RGB24/RGB32 del logo
private IntPtr logoImageBuffer = IntPtr.Zero;
private int logoImageBufferSize = 0;
```

### Paso 3: Implementar el Manejador del Evento OnVideoFrameBuffer

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
            ImageHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format32bppArgb);
        }
        else
        {
            ImageHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format24bppRgb);
        }
    }

    // Dibujar imagen
    if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
    {
        FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Width,
            e.Frame.Height, 0, 0);
    }
    else
    {
        FastImageProcessing.Draw_RGB24OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Width,
            e.Frame.Height, 0, 0);
    }

    e.UpdateData = true;
}
```

## Explicación Detallada

### Gestión de Memoria

El código maneja tanto formatos de imagen de 24 bits como de 32 bits. Esto es lo que sucede:

1. **Verificación de Inicialización de Búfer**: El código primero verifica si el búfer del logo necesita ser creado o recreado.

2. **Detección de Formato**: Determina si usar formato RGB24 o RGB32 basándose en la imagen cargada:
   - RGB24: Color estándar de 24 bits (8 bits cada uno para R, G, B)
   - RGB32: Color de 32 bits con canal alfa para transparencia (8 bits cada uno para R, G, B, A)

3. **Asignación de Memoria**: Asigna memoria no administrada usando `Marshal.AllocCoTaskMem()` para almacenar los datos de imagen.

4. **Conversión de Imagen**: Convierte el Bitmap a datos de píxeles crudos en el búfer asignado usando `ImageHelper.BitmapToIntPtr()`.

### Proceso de Dibujo

Una vez que el búfer está preparado, el dibujo se realiza:

1. **Dibujo Específico por Formato**: El código selecciona el método de dibujo apropiado basándose en el formato de imagen:
   - `FastImageProcessing.Draw_RGB32OnRGB24()` para imágenes de 32 bits con transparencia
   - `FastImageProcessing.Draw_RGB24OnRGB24()` para imágenes estándar de 24 bits

2. **Parámetros de Posición**: Los parámetros `0, 0` especifican dónde dibujar la imagen (esquina superior izquierda en este ejemplo).

3. **Actualización de Frame**: Establecer `e.UpdateData = true` asegura que los datos del frame modificado se usen para visualización o procesamiento posterior.

## Mejores Prácticas para Superposición de Imágenes

Para rendimiento óptimo al superponer imágenes en frames de video:

1. **Gestión de Memoria**: Siempre libera la memoria asignada cuando ya no se necesite para prevenir fugas de memoria.

2. **Reutilización de Búfer**: Crea el búfer una vez y reutilízalo para frames subsiguientes en lugar de recrearlo para cada frame.

3. **Consideraciones de Tamaño de Imagen**: Usa imágenes de tamaño apropiado; superponer imágenes grandes puede impactar el rendimiento.

4. **Selección de Formato**:
   - Usa PNG (RGB32) cuando necesites transparencia
   - Usa JPG (RGB24) cuando no se requiera transparencia (más eficiente)

5. **Cálculo de Posición**: Para posicionamiento dinámico, calcula coordenadas basándote en las dimensiones del frame:

   ```cs
   // Ejemplo: Posicionar logo en esquina inferior derecha con 10px de margen
   int xPos = e.Frame.Width - logoImage.Width - 10;
   int yPos = e.Frame.Height - logoImage.Height - 10;
   ```

## Manejo de Errores

Al implementar esta funcionalidad, considera agregar manejo de errores:

```cs
try 
{
    // Tu implementación existente
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

## Optimización de Rendimiento

Para aplicaciones de alto rendimiento, considera estas optimizaciones:

1. **Pre-asignación de Búfer**: Inicializa búferes durante el inicio de la aplicación en lugar de durante el procesamiento de video.

2. **Procesamiento Condicional**: Solo procesa frames que necesitan la superposición (por ejemplo, omitir procesamiento para ciertos frames).

3. **Procesamiento Paralelo**: Para operaciones complejas, considera usar técnicas de procesamiento paralelo.

## Conclusión

El evento `OnVideoFrameBuffer` proporciona una manera poderosa de manipular frames de video en tiempo real. Siguiendo esta guía, puedes superponer imágenes eficientemente en contenido de video para propósitos de marca de agua, branding o mejora visual.

La técnica demostrada aquí funciona a través de múltiples productos SDK y puede adaptarse para varios escenarios de procesamiento de video en tus aplicaciones .NET.

---
¿Buscas más ejemplos de código? Visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos y recursos adicionales.