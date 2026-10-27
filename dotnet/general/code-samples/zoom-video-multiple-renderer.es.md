---
title: Configurar zoom en múltiples renderizadores de video
description: Configure parámetros de zoom y posición independientes para múltiples renderizadores de video en pantallas multi-monitor en aplicaciones multimedia .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoRendererType
---

# Configuración de zoom para múltiples renderizadores de video en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Al desarrollar aplicaciones multimedia que utilizan múltiples renderizadores de video, controlar los parámetros de zoom y posición de forma independiente para cada pantalla es esencial para crear interfaces de usuario de calidad profesional. Esta guía cubre los detalles de implementación, configuraciones de parámetros y mejores prácticas para configurar múltiples renderizadores de video con configuraciones de zoom personalizadas en sus aplicaciones .NET.

## Entender las configuraciones de múltiples renderizadores

El soporte de múltiples renderizadores (también conocido como funcionalidad multipantalla) permite a su aplicación mostrar contenido de video en diferentes áreas de visualización simultáneamente. Cada renderizador puede configurarse con su propio:

- Ratio de zoom (nivel de magnificación)
- Desplazamiento horizontal (posicionamiento en eje X)
- Desplazamiento vertical (posicionamiento en eje Y)

Esta capacidad es particularmente valiosa para aplicaciones como:

- Sistemas de videovigilancia que muestran múltiples feeds de cámaras
- Software de producción de medios con ventanas de vista previa y salida
- Aplicaciones de imágenes médicas que requieren diferentes niveles de zoom para análisis
- Sistemas de kiosco multi-pantalla con contenido sincronizado

## Implementar el método MultiScreen_SetZoom

El SDK proporciona el método `MultiScreen_SetZoom` que toma cuatro parámetros clave:

1. **Índice de Pantalla** (basado en cero): Identifica qué renderizador configurar
2. **Ratio de Zoom**: Controla el porcentaje de magnificación
3. **Desplazamiento X**: Ajusta el posicionamiento horizontal (píxeles o porcentaje)
4. **Desplazamiento Y**: Ajusta el posicionamiento vertical (píxeles o porcentaje)

### Firma del método y parámetros

```cs
// Firma del método
void MultiScreen_SetZoom(int screenIndex, int zoomRatio, int shiftX, int shiftY);
```

| Parámetro | Descripción | Rango Válido |
|-----------|-------------|--------------|
| screenIndex | Índice basado en cero del renderizador objetivo | 0 a (número de renderizadores - 1) |
| zoomRatio | Porcentaje de magnificación | 1 a 1000 (%) |
| shiftX | Desplazamiento horizontal | -1000 a 1000 |
| shiftY | Desplazamiento vertical | -1000 a 1000 |

## Ejemplo de código: configurar múltiples renderizadores

El siguiente ejemplo demuestra cómo establecer diferentes valores de zoom y posicionamiento para tres renderizadores separados:

```cs
// Configurar el renderizador primario (índice 0)
// 50% de zoom sin desplazamiento horizontal ni vertical
VideoCapture1.MultiScreen_SetZoom(0, 50, 0, 0);

// Configurar el renderizador secundario (índice 1)
// 20% de zoom con leve desplazamiento horizontal y vertical
VideoCapture1.MultiScreen_SetZoom(1, 20, 10, 20);

// Configurar el renderizador terciario (índice 2)
// 30% de zoom sin desplazamiento horizontal pero con desplazamiento vertical significativo
VideoCapture1.MultiScreen_SetZoom(2, 30, 0, 30);
```

## Mejores prácticas para gestión de múltiples renderizadores

Al implementar configuraciones de múltiples renderizadores, considere estas mejores prácticas:

### 1. Inicializar todos los renderizadores antes de establecer el zoom

Siempre asegúrese de que todos los renderizadores estén correctamente inicializados antes de aplicar configuraciones de zoom:

```cs
// Inicializar múltiples renderizadores
VideoCapture1.MultiScreen_Enabled = true;

// Agregar 3 renderizadores
VideoCapture1.MultiScreen_AddScreen(videoView1, 1280, 720);
VideoCapture1.MultiScreen_AddScreen(videoView2, 1920, 1080);
VideoCapture1.MultiScreen_AddScreen(videoView3, 1280, 720);

// Ahora es seguro configurar ajustes de zoom
VideoCapture1.MultiScreen_SetZoom(0, 50, 0, 0);
VideoCapture1.MultiScreen_SetZoom(1, 20, 10, 20);
VideoCapture1.MultiScreen_SetZoom(2, 30, 0, 30);

// Configuraciones adicionales...
```

### 2. Manejar cambios de resolución apropiadamente

Si el formato de la fuente cambia durante la sesión, lea la resolución actual dentro
del callback `OnVideoFrameBuffer` (`e.Frame.Info.Width` / `e.Frame.Info.Height`) y
vuelva a aplicar el zoom. Lleve usted mismo el conteo de pantallas — `VideoCaptureCore`
no expone una propiedad `MultiScreen_Count`; ya sabe cuántas añadió porque usted
llamó a `MultiScreen_AddScreen`.

```cs
private int _multiScreenCount = 0; // incrementado tras cada llamada a MultiScreen_AddScreen

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    int newZoom = CalculateOptimalZoom(e.Frame.Info.Width, e.Frame.Info.Height);

    // Aplicar a todos los renderizadores registrados
    for (int i = 0; i < _multiScreenCount; i++)
    {
        VideoCapture1.MultiScreen_SetZoom(i, newZoom, 0, 0);
    }
}
```

### 3. Proporcionar controles de usuario para ajuste de zoom

Para aplicaciones interactivas, considere implementar controles de UI que permitan a los usuarios ajustar configuraciones de zoom:

```cs
private void zoomTrackBar_ValueChanged(object sender, EventArgs e)
{
    int selectedRenderer = rendererComboBox.SelectedIndex;
    int zoomValue = zoomTrackBar.Value;
    int shiftX = horizontalShiftTrackBar.Value;
    int shiftY = verticalShiftTrackBar.Value;
    
    // Aplicar nuevas configuraciones de zoom al renderizador seleccionado
    VideoCapture1.MultiScreen_SetZoom(selectedRenderer, zoomValue, shiftX, shiftY);
}
```

## Configuraciones de zoom avanzadas

### Transiciones de zoom dinámicas

Para transiciones de zoom suaves, considere implementar cambios graduales de zoom:

```cs
async Task AnimateZoomAsync(int screenIndex, int startZoom, int targetZoom, int duration)
{
    int steps = 30; // Número de pasos de animación
    int delay = duration / steps; // Milisegundos entre pasos
    
    for (int i = 0; i <= steps; i++)
    {
        // Calcular valor de zoom intermedio
        int currentZoom = startZoom + ((targetZoom - startZoom) * i / steps);
        
        // Aplicar valor de zoom actual
        VideoCapture1.MultiScreen_SetZoom(screenIndex, currentZoom, 0, 0);
        
        // Esperar al siguiente paso
        await Task.Delay(delay);
    }
}

// Uso
await AnimateZoomAsync(0, 50, 100, 1000); // Animar de 50% a 100% en 1 segundo
```

## Optimizar rendimiento con múltiples renderizadores

Al trabajar con múltiples renderizadores, tenga en cuenta las implicaciones de rendimiento:

1. **Limitar Actualizaciones Frecuentes**: Evite cambiar configuraciones de zoom rápidamente ya que puede impactar el rendimiento
2. **Considerar Aceleración por Hardware**: Habilite aceleración por hardware cuando esté disponible
3. **Monitorear Uso de Memoria**: Múltiples renderizadores de alta resolución pueden consumir memoria significativa

```cs
// Usa el renderizador EVR para composición acelerada por hardware en Windows. Los
// ajustes del renderizador viven dentro de Video_Renderer (un VideoRendererSettings),
// no directamente sobre el core.
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
VideoCapture1.Video_Renderer.Deinterlace_EVR_Mode = VideoRendererEVRDeinterlaceMode.Auto;
```

## Solución de problemas comunes

### Problema: los renderizadores muestran pantalla negra después de cambios de zoom

Esto puede ocurrir cuando los valores de zoom exceden rangos válidos o cuando los renderizadores no están correctamente inicializados:

```cs
// Restablecer configuraciones de zoom a valores predeterminados para todos los renderizadores
public void ResetZoomSettings()
{
    for (int i = 0; i < _multiScreenCount; i++)
    {
        VideoCapture1.MultiScreen_SetZoom(i, 100, 0, 0); // 100% zoom, sin desplazamiento
    }
}
```

### Problema: imagen distorsionada después del zoom

Valores de zoom extremos pueden causar distorsión. Implemente límites para valores de zoom:

```cs
public void SetSafeZoom(int screenIndex, int requestedZoom, int shiftX, int shiftY)
{
    // Restringir valores a rangos seguros
    int safeZoom = Math.Clamp(requestedZoom, 10, 200); // 10% a 200%
    int safeShiftX = Math.Clamp(shiftX, -100, 100);
    int safeShiftY = Math.Clamp(shiftY, -100, 100);
    
    VideoCapture1.MultiScreen_SetZoom(screenIndex, safeZoom, safeShiftX, safeShiftY);
}
```

## Conclusión

Los múltiples renderizadores de video correctamente configurados con configuraciones de zoom independientes pueden mejorar significativamente la experiencia del usuario en aplicaciones multimedia. Siguiendo las guías y mejores prácticas descritas en este documento, puede implementar configuraciones de visualización de video sofisticadas adaptadas a los requisitos específicos de su aplicación.

Para ejemplos de código adicionales y guía de implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
