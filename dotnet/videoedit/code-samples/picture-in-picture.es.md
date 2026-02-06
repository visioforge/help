---
title: Crear Videos Dinámicos Imagen-en-Imagen en .NET
description: Implementa Imagen-en-Imagen, videos lado a lado y diseños de video personalizados en C# con muestras de código completas para posicionamiento de superposición.
---

# Crear Videos Imagen-en-Imagen y Pantalla Dividida en .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introducción a las Técnicas de Composición de Video

Las aplicaciones de video profesionales a menudo requieren combinar múltiples fuentes de video en una única salida. Esta capacidad es esencial para crear contenido atractivo como tutoriales con superposiciones del presentador, videos de reacción, paneles de entrevistas o transmisiones deportivas con ventanas de repetición. El Video Edit SDK para .NET hace que estas técnicas avanzadas sean sencillas de implementar en tus aplicaciones C#.

Esta guía cubre tres enfoques principales de composición de video:

1. **Imagen-en-Imagen (PIP)**: Colocar una superposición de video más pequeña sobre un video principal
2. **División Horizontal**: Posicionar dos videos lado a lado horizontalmente
3. **División Vertical**: Organizar dos videos uno sobre otro

Cada técnica tiene casos de uso específicos y puede personalizarse para crear exactamente la presentación visual que tu aplicación requiere.

## Crear Superposiciones de Video Imagen-en-Imagen

Imagen-en-Imagen es la técnica de composición de video más común, donde un video más pequeño aparece en una esquina o posición personalizada sobre un video de fondo más grande. Esto es perfecto para crear videos de comentarios, tutoriales o cualquier contenido donde quieras mostrar dos perspectivas simultáneamente sin dividir la pantalla equitativamente.

### Paso 1: Definir tus Archivos de Video

Primero, especifica las rutas de archivo para los videos que quieres combinar:

```cs
string[] files = {  "!video.avi", "!video2.wmv" };
```

Puedes usar varios formatos de video incluyendo MP4, AVI, MOV, WMV y muchos otros soportados por el SDK.

### Paso 2: Crear Segmentos para el Video Principal

Define qué porción del video principal usar estableciendo tiempos de inicio y fin:

```cs
FileSegment[] segments1 = new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(10000)) };
```

Este ejemplo usa los primeros 10 segundos del video principal. Puedes ajustar estos valores para usar cualquier segmento de tu archivo fuente.

### Paso 3: Inicializar la Fuente de Video Principal

Crea un objeto VideoSource para tu video principal con tus configuraciones preferidas:

```cs
var videoFile = new VideoSource(
                                files[0],
                                segments1,
                                VideoEditStretchMode.Letterbox,
                                0,
                                1.0);
```

Los parámetros incluyen:

- Ruta del archivo
- Segmentos de tiempo a incluir
- Modo de estiramiento (cómo manejar diferencias de relación de aspecto)
- Ángulo de rotación
- Multiplicador de volumen

### Paso 4: Configurar la Fuente de Video PIP

De manera similar, configura el video que aparecerá como superposición:

```cs
FileSegment[] segments2 = new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(10000)) };

var videoFile2 = new VideoSource(
                                files[1],
                                segments2,
                                VideoEditStretchMode.Letterbox,
                                0,
                                1.0);
```

### Paso 5: Definir Rectángulos de Posicionamiento de Video

Especifica el tamaño y posición para ambos videos:

```cs
// Rectángulo para el video de fondo principal (fotograma completo)
var rect1 = new Rectangle(0, 0, 1280, 720);

// Rectángulo para la superposición de video PIP más pequeña
var rect2 = new Rectangle(100, 100, 320, 240);
```

Puedes ajustar la posición y tamaño del segundo rectángulo para colocar el video PIP donde quieras en la pantalla. Las posiciones comunes incluyen las esquinas superior derecha o inferior izquierda.

### Paso 6: Combinar los Videos con PIP

Finalmente, añade ambas fuentes de video a tu proyecto usando el modo PIP:

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile,          // Video principal
    videoFile2,         // Video PIP
    TimeSpan.FromMilliseconds(0),         // Tiempo de inicio
    TimeSpan.FromMilliseconds(10000),     // Duración
    VideoEditPIPMode.Custom,              // Modo PIP
    true,                                // Mostrar ambos videos
    1280, 720,                           // Resolución de salida
    0,                                   // Tipo de transición
    rect2,                               // Rectángulo del video PIP
    rect1                                // Rectángulo del video principal
);
```

El video resultante mostrará tu video principal llenando todo el fotograma con el segundo video apareciendo en la posición del rectángulo especificado.

## Crear Diseños de Video Lado a Lado

Los diseños lado a lado dividen la pantalla equitativamente entre dos fuentes de video. Este enfoque funciona bien para videos de comparación, contenido de reacción o presentaciones de entrevistas donde ambos videos merecen igual espacio de pantalla.

### Pantalla Dividida Horizontal

Una división horizontal coloca los videos lado a lado horizontalmente. Esto funciona bien para comparar efectos antes/después o mostrar a dos personas en conversación:

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile, 
    videoFile2, 
    TimeSpan.FromMilliseconds(0), 
    TimeSpan.FromMilliseconds(10000), 
    VideoEditPIPMode.Horizontal, 
    false
);
```

### Pantalla Dividida Vertical

Una división vertical apila los videos uno sobre otro. Esto puede ser útil para mostrar relaciones de causa y efecto o crear diseños de panel superior/inferior:

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile, 
    videoFile2, 
    TimeSpan.FromMilliseconds(0), 
    TimeSpan.FromMilliseconds(10000), 
    VideoEditPIPMode.Vertical, 
    false
);
```

## Opciones de Personalización Avanzadas

El SDK ofrece numerosas opciones para personalizar aún más tus composiciones de video:

- **Posicionamiento Personalizado**: Define coordenadas exactas de pantalla para colocación precisa de video
- **Transiciones**: Añade efectos de fundido u otras transiciones entre segmentos de video
- **Control de Audio**: Ajusta los niveles de volumen para cada fuente de video independientemente
- **Efectos de Borde**: Añade bordes, sombras o marcos alrededor de elementos de video PIP
- **Animación**: Crea elementos PIP móviles que cambian de posición con el tiempo
- **Múltiples Superposiciones**: Añade más de dos videos para crear composiciones complejas

Estas capacidades te permiten crear producciones de video de nivel profesional directamente desde tus aplicaciones .NET.

## Requisitos de Implementación

Para implementar exitosamente estas técnicas de composición de video en tu aplicación, necesitarás incluir los paquetes redistribuibles apropiados:

- Redistribuible de Video Edit SDK [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

Para información sobre cómo desplegar estas dependencias en los sistemas de tus usuarios, consulta nuestra [guía de despliegue](../deployment.md).

## Consideraciones de Rendimiento

Al trabajar con múltiples fuentes de video, especialmente a altas resoluciones, ten en cuenta el uso de recursos del sistema. Las operaciones de composición de video pueden ser intensivas en procesador. Considera las siguientes mejores prácticas:

- Pre-renderizar composiciones complejas para reproducción en entornos de producción
- Optimizar la resolución y tasa de bits del video para tu plataforma objetivo
- Probar el rendimiento en hardware similar a tu entorno de despliegue objetivo

## Conclusión

Las composiciones de video Imagen-en-Imagen y pantalla dividida añaden capacidades profesionales a las aplicaciones multimedia. El Video Edit SDK para .NET hace que implementar estas características sea sencillo con su API intuitiva. Ya sea que estés desarrollando una aplicación de edición de video, una plataforma de streaming o integrando procesamiento de video en un sistema más grande, estas técnicas proporcionan formas poderosas de combinar y presentar múltiples fuentes de video.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más muestras de código.