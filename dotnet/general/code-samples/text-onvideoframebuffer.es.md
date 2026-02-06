---
title: Superposición de Texto OnVideoFrameBuffer .NET
description: Crea superposiciones de texto personalizadas en frames de video con OnVideoFrameBuffer para renderizado dinámico en aplicaciones .NET.
---

# Creación de Superposiciones de Texto Personalizadas con OnVideoFrameBuffer en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a las Superposiciones de Texto en Procesamiento de Video

Agregar superposiciones de texto al contenido de video es un requisito común en muchas aplicaciones profesionales, desde software de edición de video hasta transmisiones de cámaras de seguridad, herramientas de transmisión y aplicaciones educativas. Mientras que las APIs estándar de efectos de video proporcionan capacidades básicas de superposición de texto, los desarrolladores a menudo necesitan más control sobre cómo aparece el texto en los frames de video.

Esta guía demuestra cómo implementar manualmente superposiciones de texto personalizadas usando el evento OnVideoFrameBuffer disponible en los motores VideoCaptureCore, VideoEditCore y MediaPlayerCore. Al interceptar frames de video durante el procesamiento, puedes aplicar texto y gráficos personalizados con control preciso sobre posicionamiento, formato y animación.

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

    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref textLogo, e.Timestamp, 0);
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
FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref textLogo, e.Timestamp, 0);
```

Esta línea hace el trabajo real de renderizar el texto en el frame:

- Toma el búfer de datos del frame como entrada
- Usa las dimensiones del frame para posicionar correctamente el texto
- Referencia el objeto textLogo que contiene las propiedades del texto
- Puede utilizar la marca de tiempo para contenido dinámico

## Opciones de Personalización Avanzada

Mientras que el ejemplo básico demuestra una superposición de texto estático simple, la clase VideoEffectTextLogo soporta numerosas opciones de personalización:

### Formato de Texto

```cs
textLogo.FontName = "Arial";
textLogo.FontSize = 24;
textLogo.FontBold = true;
textLogo.FontItalic = false;
textLogo.Color = System.Drawing.Color.White;
textLogo.Opacity = 0.8f;
```

### Fondo y Bordes

```cs
textLogo.BackgroundEnabled = true;
textLogo.BackgroundColor = System.Drawing.Color.Black;
textLogo.BackgroundOpacity = 0.5f;
textLogo.BorderEnabled = true;
textLogo.BorderColor = System.Drawing.Color.Yellow;
textLogo.BorderThickness = 2;
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
    
    // Actualizar texto basado en marca de tiempo
    textLogo.Text = $"Marca de tiempo: {e.Timestamp.ToString("HH:mm:ss.fff")}";
    
    // Animar posición
    textLogo.Left = 50 + (int)(Math.Sin(e.Timestamp.TotalSeconds) * 50);
    
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref textLogo, e.Timestamp, 0);
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
    timestampLogo.Text = e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
    
    // Renderizar ambos elementos de texto
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref titleLogo, e.Timestamp, 0);
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref timestampLogo, e.Timestamp, 0);
}
```

## Componentes Requeridos

Para implementar esta solución, necesitarás:

- Paquete redist del SDK instalado en tu aplicación
- Referencia al SDK apropiado (.NET Video Capture, Video Edit o Media Player)
- Entendimiento básico de conceptos de procesamiento de frames de video

## Conclusión

El evento OnVideoFrameBuffer proporciona un mecanismo poderoso para implementar superposiciones de texto personalizadas en aplicaciones de video. Al acceder directamente al búfer del frame, los desarrolladores pueden crear efectos de texto sofisticados con control preciso sobre la apariencia y el comportamiento.

Este enfoque es particularmente valioso cuando las APIs estándar de superposición de texto no proporcionan la flexibilidad o características requeridas para tu aplicación. Con las técnicas demostradas en esta guía, puedes implementar superposiciones de texto de calidad profesional para una amplia gama de escenarios de procesamiento de video.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.