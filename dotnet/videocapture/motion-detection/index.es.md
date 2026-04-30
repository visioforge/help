---
title: Detección de Movimiento en Video SDK .NET - Guía Completa
description: Implemente detección de movimiento avanzada y simple en .NET con múltiples tipos de detectores, configuraciones personalizables y procesamiento en tiempo real.
sidebar_label: Detección de Movimiento
order: 6
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
primary_api_classes:
  - MotionDetectionSettings
  - MotionDetectionExSettings

---

# Detección de Movimiento para Procesamiento de Video

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción General

El Video Capture SDK proporciona potentes capacidades de detección de movimiento para sus aplicaciones .NET. Ya sea que necesite una detección de presencia simple o un seguimiento de objetos avanzado, el SDK ofrece dos implementaciones distintas de detectores de movimiento para satisfacer sus requisitos específicos:

1. **Detector de Movimiento Simple** - Procesamiento eficiente y ligero con matrices de detección personalizables
2. **Detector de Movimiento Avanzado** - Capacidades mejoradas que incluyen detección de objetos, múltiples tipos de procesadores y detectores especializados

Estas herramientas de detección de movimiento permiten a los desarrolladores implementar características sofisticadas de análisis de video, como monitoreo de seguridad, alertas automatizadas, conteo de objetos y aplicaciones interactivas sensibles al movimiento.

## Detector de Movimiento Simple

[VideoCaptureCore](#){ .md-button }

### Cómo Funciona

El detector de movimiento simple ofrece una solución eficiente para escenarios básicos de detección de movimiento. Su enfoque simplificado lo hace ideal para aplicaciones donde la velocidad de procesamiento y la eficiencia de recursos son prioridades.

Cuando se activa, el detector:

- Procesa cada cuadro para detectar cambios de movimiento
- Genera una matriz de bytes bidimensional (matriz de movimiento)
- Calcula el nivel general de movimiento como un porcentaje
- Opcionalmente resalta visualmente las áreas de movimiento detectadas

### Características Clave

- **Tamaño de Matriz Personalizable**: Ajuste la resolución de detección para equilibrar el rendimiento y la precisión
- **Selección de Canales**: Analice todos los canales RGB o concéntrese en canales específicos para una detección optimizada
- **Resaltado de Movimiento**: Enfatice visualmente el movimiento detectado con superposiciones de color
- **Optimización del Rendimiento**: Configure los ajustes de intervalo de cuadros para gestionar la carga de procesamiento

### Ejemplo de Implementación

```cs
// create motion detector settings
var motionDetector = new MotionDetectionSettings();

// set the motion detector matrix dimensions
motionDetector.Matrix_Width = 10;
motionDetector.Matrix_Height = 10;

// configure color channel analysis
motionDetector.Compare_Red = false;
motionDetector.Compare_Green = false;
motionDetector.Compare_Blue = false;
motionDetector.Compare_Greyscale = true;

// motion highlighting configuration
motionDetector.Highlight_Color = MotionCHLColor.Green;
motionDetector.Highlight_Enabled = true;

// performance optimization settings
motionDetector.FrameInterval = 5;
motionDetector.DropFrames_Enabled = false;

// enable detection (default is false — required for OnMotion events to fire)
motionDetector.Enabled = true;

// apply settings to the video capture component
VideoCapture1.Motion_Detection = motionDetector;
VideoCapture1.MotionDetection_Update();
```

### Recuperación de Datos de Movimiento

Suscríbete al evento `OnMotion` (tipo: `EventHandler<MotionDetectionEventArgs>`) antes de iniciar el pipeline. El handler se dispara una vez por frame cuando se detecta movimiento:

```cs
VideoCapture1.OnMotion += (sender, e) =>
{
    // e.Level  — intensidad global de movimiento (int, típicamente 0-100).
    // e.Matrix — byte[] grid overlay; cada celda contiene la cantidad de movimiento para esa región.
    if (e.Level > 25)
    {
        Console.WriteLine($"Nivel de movimiento {e.Level}% — {e.Matrix?.Length ?? 0} celdas");
        // Activa grabación, envía alerta, encola un snapshot, etc.
    }
};
```

Los eventos se disparan en un hilo worker — haz marshal al hilo UI (`Dispatcher`/`Invoke`) antes de tocar controles UI.

## Detector de Movimiento Avanzado

[VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

### Capacidades Mejoradas

El detector de movimiento avanzado proporciona algoritmos de detección y opciones de análisis más sofisticados. Este detector está diseñado para aplicaciones que requieren información detallada sobre el movimiento, identificación de objetos y definición precisa del área de movimiento.

Las ventajas clave incluyen:

- Detección y conteo de objetos
- Múltiples tipos de procesadores para diferentes necesidades de análisis visual
- Algoritmos de detección especializados para diversos entornos
- Procesamiento mejorado del área de movimiento

### Opciones de Configuración

#### Tipos de Procesadores de Movimiento

El procesador determina cómo se analiza y visualiza el movimiento:

- **None**: Detección sin procesar sin resaltado visual
- **BlobCountingObjects**: Identifica y cuenta objetos en movimiento distintos
- **GridMotionAreaProcessing**: Divide el cuadro en secciones de cuadrícula para el análisis
- **MotionAreaHighlighting**: Resalta áreas completas donde se detecta movimiento
- **MotionBorderHighlighting**: Delinea el perímetro de las áreas de movimiento

#### Tipos de Detectores de Movimiento

El algoritmo del detector define el enfoque fundamental para la identificación de movimiento:

- **CustomFrameDifference**: Compara el cuadro actual con un fondo predefinido
- **SimpleBackgroundModeling**: Utiliza técnicas de modelado de fondo adaptativo
- **TwoFramesDifference**: Analiza las diferencias entre cuadros consecutivos

### Pasos de Implementación

1. Cree la configuración del detector de movimiento avanzado:

```cs
var motionDetector = new MotionDetectionExSettings();
```

2. Seleccione el tipo de procesador apropiado para las necesidades de su aplicación:

```cs
motionDetector.ProcessorType = MotionProcessorType.BlobCountingObjects;
```

3. Elija el algoritmo de detección que mejor se adapte a su entorno:

```cs
motionDetector.DetectorType = MotionDetectorType.CustomFrameDifference;
```

4. Aplique la configuración a su componente de captura de video:

=== "VideoCaptureCoreX"

    
    ```cs
    VideoCapture1.Motion_Detection = motionDetector;
    ```
    

=== "VideoCaptureCore"

    
    ```cs
    VideoCapture1.Motion_DetectionEx = motionDetector;
    ```
    


5. Implementa el manejador de eventos correspondiente. Ambos eventos llevan `MotionDetectionExEventArgs` con `Level` (float), `LevelPercent` (0-100 int), `ObjectsCount`, `ObjectRectangles` (`Rect[]`) y `MotionGrid` (`float[,]`):

    === "VideoCaptureCoreX"

        ```cs
        VideoCapture1.OnMotionDetection += (sender, e) =>
        {
            if (e.LevelPercent > 25)
            {
                Console.WriteLine($"Movimiento {e.LevelPercent}% — {e.ObjectsCount} objetos móviles");
                foreach (var rect in e.ObjectRectangles)
                {
                    Console.WriteLine($"  en {rect}");
                }
            }
        };
        ```

    === "VideoCaptureCore"

        ```cs
        VideoCapture1.OnMotionDetectionEx += (sender, e) =>
        {
            if (e.LevelPercent > 25)
            {
                Console.WriteLine($"Movimiento {e.LevelPercent}% — {e.ObjectsCount} objetos móviles");
            }
        };
        ```

    Los handlers corren en un hilo worker — marshal al hilo UI antes de actualizar controles.

## Aplicaciones Prácticas

Las capacidades de detección de movimiento permiten a los desarrolladores crear potentes aplicaciones de procesamiento de video:

- **Sistemas de Seguridad**: Activar grabación o alertas cuando se detecta movimiento no autorizado
- **Análisis de Tráfico**: Contar y rastrear vehículos o peatones
- **Instalaciones Interactivas**: Crear experiencias digitales sensibles al movimiento
- **Indexación Automatizada de Video**: Marcar y categorizar secciones que contienen actividad
- **Automatización Industrial**: Monitorear líneas de producción o áreas restringidas
- **Observación de Vida Silvestre**: Grabar actividad animal sin intervención humana

## Consideraciones de Rendimiento

Para optimizar el rendimiento de la detección de movimiento:

1. Ajuste las dimensiones de la matriz para equilibrar la precisión y la carga de procesamiento
2. Use configuraciones de intervalo de cuadros para analizar solo los cuadros esenciales
3. Seleccione los canales de color apropiados para su escenario de detección
4. Considere habilitar la opción de eliminación de cuadros para requisitos de alto rendimiento
5. Elija el tipo de detector basado en las condiciones específicas de su entorno

## Configuración Avanzada

La clase avanzada `MotionDetectionExSettings` (usada por `VideoCaptureCoreX` vía `Motion_Detection` / por el clásico `VideoCaptureCore` vía `Motion_DetectionEx`) expone estas propiedades adicionales:

- `DifferenceThreshold` — umbral de diferencia por píxel para filtrar movimientos menores
- `MinObjectsWidth` / `MinObjectsHeight` — ignorar objetos detectados por debajo de estas dimensiones
- `SuppressNoise` — filtro de supresión de ruido para reducir falsos positivos
- `HighlightMotionRegions` — dibujar rectángulos delimitadores alrededor de las regiones de objetos detectados
- `KeepObjectsEdges` — preservar bordes nítidos al resaltar
- `HighlightColor` (SKColor) — color usado para resaltar el movimiento

## Integración de Eventos

Los eventos de detección de movimiento se pueden integrar con otras características del SDK:

- Grabación de video para capturar el movimiento detectado
- Creación de instantáneas cuando se detecta movimiento
- Sistemas de notificación personalizados
- Registro y análisis de datos

---

Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
