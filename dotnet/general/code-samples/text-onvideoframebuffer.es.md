---
title: Texto sobre Video en C# .NET con OnVideoFrameBuffer
description: Dibuja texto dinámico en frames de video en C# / .NET con el evento OnVideoFrameBuffer. Timestamps, datos de sensores, fuentes — renderizado pixel-level.
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
  - C#
primary_api_classes:
  - VideoEffectTextLogo
  - VideoFrameBufferEventArgs
  - VideoFrameXBufferEventArgs
  - VideoCaptureCoreX
  - MediaPlayerCoreX

---

# Creación de Superposiciones de Texto Personalizadas con OnVideoFrameBuffer en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

El evento `OnVideoFrameBuffer` da acceso directo a nivel de píxel a cada frame de video conforme pasa por el pipeline. Dibujar texto sobre el frame — timestamps, lecturas de sensores, telemetría de depuración o branding personalizado — es uno de los usos más comunes. Esta guía muestra cómo renderizar texto sobre frames crudos con control total de fuente, color, posición y lógica por frame.

!!! tip "¿Buscas la función de text overlay de alto nivel?"
    Si solo necesitas un overlay de texto estático, animado o basado en reloj con posicionamiento estándar, usa el [efecto de text overlay](../video-effects/text-overlay.md) dedicado — una línea de código mediante `Video_Effects_Add(new VideoEffectTextLogo(...))`. Usa `OnVideoFrameBuffer` (esta página) cuando necesites **control a nivel de píxel**: fuentes personalizadas, layout avanzado, contenido dinámico por frame o integración con bibliotecas de texto / gráficos de terceros.

### Motores soportados

El evento `OnVideoFrameBuffer` está expuesto en ambas familias de motores:

| Motor | Tipo de event args | Formato de píxel |
|---|---|---|
| `VideoCaptureCore` (DirectShow, Windows) | `VideoFrameBufferEventArgs` | RGB24 / RGB32 |
| `VideoCaptureCoreX` (GStreamer, multiplataforma) | `VideoFrameXBufferEventArgs` | BGRA (lo más común) |
| `MediaPlayerCoreX` (GStreamer, multiplataforma) | `VideoFrameXBufferEventArgs` | BGRA (lo más común) |

Ambos motores siguen el mismo patrón — suscribirse al evento, leer `e.Frame.Data` (un `IntPtr`) junto con `Width` / `Height` / `Stride`, renderizar en el buffer in-place, y poner `e.UpdateData = true` para propagar los cambios downstream.

## Entendiendo el Evento OnVideoFrameBuffer

El evento OnVideoFrameBuffer es un gancho poderoso que da a los desarrolladores acceso directo al búfer del frame de video durante el procesamiento. Este evento se dispara para cada frame de video, proporcionando una oportunidad para modificar los datos del frame antes de que se muestren o codifiquen.

Los beneficios clave de usar OnVideoFrameBuffer para superposiciones de texto incluyen:

- **Acceso a nivel de frame**: Modifica frames individuales con precisión de píxel perfecta
- **Contenido dinámico**: Actualiza el texto basándote en datos en tiempo real o marcas de tiempo
- **Estilo personalizado**: Aplica fuentes, colores y efectos personalizados más allá de lo que ofrecen las APIs incorporadas
- **Optimizaciones de rendimiento**: Implementa técnicas de renderizado eficientes para aplicaciones de alto rendimiento

## Descripción General de la Implementación

La técnica presentada aquí usa los siguientes componentes:

1. Un manejador de eventos para OnVideoFrameBuffer que procesa cada frame de video
2. Un objeto VideoEffectTextLogo para definir las propiedades del texto
3. La API FastImageProcessing para renderizar texto en el búfer del frame

Este enfoque es particularmente útil cuando necesitas:

- Mostrar datos dinámicos como marcas de tiempo, metadatos o lecturas de sensores
- Crear efectos de texto animados
- Posicionar texto con precisión de píxel perfecta
- Aplicar estilo personalizado no disponible a través de APIs estándar

## Implementación de Código de Ejemplo

El siguiente ejemplo en C# demuestra cómo implementar un sistema básico de superposición de texto usando el evento OnVideoFrameBuffer:

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logoInitiated)
    {
        logoInitiated = true;

        InitTextLogo();
    }

    // AddTextLogo(context, pixels, pixels32bit, pixels32tmp, frameWidth, frameHeight,
    //             ref textLogo, timeStamp, frameNumber)
    // Pasa pixels32bit: false + pixels32tmp: IntPtr.Zero para frames RGB24.
    FastImageProcessing.AddTextLogo(
        context: null,
        pixels: e.Frame.Data,
        pixels32bit: false,
        pixels32tmp: IntPtr.Zero,
        frameWidth: e.Frame.Info.Width,
        frameHeight: e.Frame.Info.Height,
        textLogo: ref textLogo,
        timeStamp: e.Frame.Timestamp,
        frameNumber: 0);
}

private bool logoInitiated = false;

private VideoEffectTextLogo textLogo = null;

private void InitTextLogo()
{
    textLogo = new VideoEffectTextLogo(true);
    textLogo.Text = "¡Hola mundo!";
    textLogo.Left = 50;
    textLogo.Top = 50;
}
```

## Explicación Detallada del Código

Analicemos los componentes clave de esta implementación:

### El Manejador de Eventos

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
```

Este método se activa para cada frame de video. El VideoFrameBufferEventArgs proporciona acceso a:

- Datos del frame (búfer de píxeles)
- Dimensiones del frame (ancho y alto)
- Información de marca de tiempo

### Lógica de Inicialización

```cs
if (!logoInitiated)
{
    logoInitiated = true;
    InitTextLogo();
}
```

Este código asegura que el logo de texto solo se inicialice una vez, previniendo la creación innecesaria de objetos para cada frame. Este patrón es importante para el rendimiento al procesar video a altas tasas de frames.

### Configuración del Logo de Texto

```cs
private void InitTextLogo()
{
    textLogo = new VideoEffectTextLogo(true);
    textLogo.Text = "¡Hola mundo!";
    textLogo.Left = 50;
    textLogo.Top = 50;
}
```

La clase VideoEffectTextLogo se usa para definir las propiedades de la superposición de texto:

- El contenido del texto ("¡Hola mundo!")
- Coordenadas de posición (50 píxeles desde la izquierda y desde arriba)

### Renderizado de la Superposición de Texto

```cs
FastImageProcessing.AddTextLogo(
    context: null,
    pixels: e.Frame.Data,
    pixels32bit: false,        // true cuando el motor entrega RGB32
    pixels32tmp: IntPtr.Zero,  // buffer de scratch opcional; IntPtr.Zero deja que el helper asigne bajo demanda
    frameWidth:  e.Frame.Info.Width,
    frameHeight: e.Frame.Info.Height,
    textLogo: ref textLogo,
    timeStamp: e.Frame.Timestamp,
    frameNumber: 0);
```

La firma de 9 args refleja exactamente `FastImageProcessing.AddTextLogo`. El ancho/alto viven dentro de `e.Frame.Info` en el struct clásico `VideoFrame`; la marca de tiempo vive en `e.Frame.Timestamp`. Pasa `pixels32bit: true` cuando tu fuente es RGB32.

## Opciones de Personalización Avanzada

Mientras que el ejemplo básico demuestra una superposición de texto estático simple, la clase VideoEffectTextLogo soporta numerosas opciones de personalización:

### Formato de Texto

```cs
// Font es un System.Drawing.Font completo — funciona cualquier combinación tipografía + estilo.
textLogo.Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold);
textLogo.FontColor = System.Drawing.Color.White;
textLogo.TransparencyLevel = 200;   // 0 (totalmente transparente) - 255 (opaco)
```

### Fondo y Bordes

```cs
textLogo.BackgroundTransparent = false;
textLogo.BackgroundColor = System.Drawing.Color.Black;

// El anillo del borde se configura vía BorderMode + colores y tamaños por anillo (interno/externo).
// Valores de TextEffectMode: None, Inner, Outer, InnerAndOuter, Embossed, Outline, FilledOutline, Halo.
textLogo.BorderMode = TextEffectMode.InnerAndOuter;
textLogo.BorderInnerColor = System.Drawing.Color.Yellow;
textLogo.BorderInnerSize = 2;
textLogo.BorderOuterColor = System.Drawing.Color.Black;
textLogo.BorderOuterSize = 1;
```

### Animación y Contenido Dinámico

Para contenido dinámico que cambia por frame:

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logoInitiated)
    {
        logoInitiated = true;
        InitTextLogo();
    }

    // La marca de tiempo vive en e.Frame.Timestamp (TimeSpan)
    textLogo.Text = $"Marca de tiempo: {e.Frame.Timestamp:hh\\:mm\\:ss\\.fff}";

    // Animar posición
    textLogo.Left = 50 + (int)(Math.Sin(e.Frame.Timestamp.TotalSeconds) * 50);

    FastImageProcessing.AddTextLogo(
        context: null,
        pixels: e.Frame.Data,
        pixels32bit: false,
        pixels32tmp: IntPtr.Zero,
        frameWidth:  e.Frame.Info.Width,
        frameHeight: e.Frame.Info.Height,
        textLogo: ref textLogo,
        timeStamp: e.Frame.Timestamp,
        frameNumber: 0);
}
```

## Consideraciones de Rendimiento

Al implementar superposiciones de texto personalizadas, considera estas mejores prácticas de rendimiento:

1. **Inicializar objetos una vez**: Crea el objeto VideoEffectTextLogo solo una vez, no por frame
2. **Minimizar cambios de texto**: Actualiza el contenido del texto solo cuando sea necesario
3. **Usar fuentes eficientes**: Las fuentes simples se renderizan más rápido que las complejas
4. **Considerar la resolución**: Los videos de mayor resolución requieren más potencia de procesamiento
5. **Probar en hardware objetivo**: Asegúrate de que tu implementación funcione bien en sistemas de producción

## Múltiples Elementos de Texto

Para mostrar múltiples elementos de texto en el mismo frame:

```cs
private VideoEffectTextLogo titleLogo = null;
private VideoEffectTextLogo timestampLogo = null;

private void InitTextLogos()
{
    titleLogo = new VideoEffectTextLogo(true);
    titleLogo.Text = "Transmisión de Cámara";
    titleLogo.Left = 50;
    titleLogo.Top = 50;
    
    timestampLogo = new VideoEffectTextLogo(true);
    timestampLogo.Left = 50;
    timestampLogo.Top = 100;
}

private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logosInitiated)
    {
        logosInitiated = true;
        InitTextLogos();
    }
    
    // Actualizar contenido dinámico
    timestampLogo.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

    // Renderizar ambos elementos de texto
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, false, IntPtr.Zero,
        e.Frame.Info.Width, e.Frame.Info.Height, ref titleLogo, e.Frame.Timestamp, 0);
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, false, IntPtr.Zero,
        e.Frame.Info.Width, e.Frame.Info.Height, ref timestampLogo, e.Frame.Timestamp, 0);
}
```

## Componentes Requeridos

Para implementar esta solución, necesitarás:

- Paquete redist del SDK instalado en tu aplicación (NuGet en los motores X, instalador en `VideoCaptureCore`).
- Referencia al SDK apropiado (Video Capture SDK, Video Edit SDK o Media Player SDK — X o clásico).
- Para el ejemplo de motor X: referencia transitiva a SkiaSharp (ya incluida por el SDK) o tu propia biblioteca de renderizado de texto.

## Ejemplo con VideoCaptureCoreX / MediaPlayerCoreX (motores X)

En los motores X multiplataforma la firma del evento es `VideoFrameXBufferEventArgs` y el buffer del frame típicamente llega en formato **BGRA** (4 bytes por píxel). El ejemplo siguiente usa SkiaSharp para envolver el buffer crudo y dibujar texto encima; SkiaSharp es una dependencia transitiva de los motores X, así que no se necesita un paquete NuGet adicional.

```cs
using SkiaSharp;

// Crea los paints una vez, reutiliza entre frames
private SKPaint _textPaint = new SKPaint
{
    Color = SKColors.White,
    TextSize = 32,
    IsAntialias = true,
    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
};

private SKPaint _shadowPaint = new SKPaint
{
    Color = SKColors.Black.WithAlpha(160),
    TextSize = 32,
    IsAntialias = true,
    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
};

// Suscríbete después de construir VideoCaptureCoreX / MediaPlayerCoreX
_videoCapture.OnVideoFrameBuffer += VideoCapture_OnVideoFrameBuffer;

private void VideoCapture_OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
{
    if (e.Frame == null || e.Frame.Data == IntPtr.Zero)
    {
        return;
    }

    // Envuelve el buffer crudo BGRA en una surface de SkiaSharp (sin asignación extra)
    var info = new SKImageInfo(e.Frame.Width, e.Frame.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

    using (var pixmap = new SKPixmap(info, e.Frame.Data, e.Frame.Stride))
    using (var surface = SKSurface.Create(pixmap))
    {
        var canvas = surface.Canvas;

        // Contenido dinámico construido por frame
        var timestamp = e.Frame.Timestamp.ToString(@"hh\:mm\:ss\.fff");
        var line = $"REC  {timestamp}";

        // Dibuja primero la sombra y luego el texto principal para legibilidad sobre cualquier fondo
        canvas.DrawText(line, 18, 42, _shadowPaint);
        canvas.DrawText(line, 16, 40, _textPaint);
        canvas.Flush();
    }

    // Propaga el frame modificado downstream
    e.UpdateData = true;
}
```

**Por qué importa BGRA.** Los motores X solicitan BGRA por defecto para los callbacks de frame porque mapea 1:1 a SkiaSharp, System.Drawing y la mayoría de rutas de interop amigables con GPU. Si necesitas otro formato, solicita un bloque de conversión aguas arriba en vez de convertir en cada frame.

**Medir y posicionar el texto.** Usa `_textPaint.MeasureText(line)` para calcular el ancho y alinear a la derecha o al centro. SkiaSharp también expone `SKFontMetrics` vía `_textPaint.FontMetrics` para baseline / ascent / descent y posicionar el texto con precisión contra los bordes del frame.

**Pilas de imagen alternativas.** También puedes usar `System.Drawing.Graphics` envolviendo un `Bitmap` construido sobre el buffer crudo en Windows, o escrituras de bytes directas con `Marshal.Copy` / `Span<byte>` para control total. SkiaSharp es la opción recomendada en macOS / Linux / iOS / Android.

**Paridad entre motores.** Todo lo de [Consideraciones de Rendimiento](#consideraciones-de-rendimiento) y [Múltiples Elementos de Texto](#multiples-elementos-de-texto) aplica por igual a los motores X — el evento se dispara en un hilo de procesamiento, `UpdateData` propaga los cambios y el trabajo pesado debe moverse fuera para evitar descartar frames.

## Conclusión

El evento `OnVideoFrameBuffer` da acceso directo a cada frame crudo tanto en el motor clásico `VideoCaptureCore` (RGB24/RGB32 vía `VideoFrameBufferEventArgs` + `FastImageProcessing`) como en los motores X multiplataforma (`VideoCaptureCoreX` / `MediaPlayerCoreX`, BGRA vía `VideoFrameXBufferEventArgs` + SkiaSharp). Es la herramienta correcta cuando necesitas renderizado de texto a nivel de píxel — fuentes personalizadas, contenido dinámico por frame, anti-aliasing que controlas, o integración con bibliotecas de texto / gráficos de terceros.

Para overlays de texto estáticos o basados en reloj sin escribir un handler por frame, el [efecto de text overlay](../video-effects/text-overlay.md) de una línea suele ser la mejor elección.

## Documentación relacionada

- [Efecto de text overlay](../video-effects/text-overlay.md) — overlay de texto declarativo de alto nivel sin escribir callback.
- [Dibujo de imágenes vía OnVideoFrameBuffer](image-onvideoframebuffer.md) — la misma técnica aplicada a imágenes en vez de texto.
- [Dibujo de video en un PictureBox](draw-video-picturebox.md) — patrón de rendering WinForms que suele combinarse con trabajo a nivel de píxel.

---

Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.