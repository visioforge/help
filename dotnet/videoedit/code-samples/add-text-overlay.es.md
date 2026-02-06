---
title: Superposiciones de Texto en Videos con .NET
description: Crea superposiciones de texto dinámicas con control completo sobre fuente, color, posición y temporización. Incluye ejemplos de código y animaciones.
---

# Implementar Superposiciones de Texto en Proyectos de Video

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introducción a las Superposiciones de Texto

Las superposiciones de texto son componentes esenciales en la edición de video profesional. Te permiten añadir títulos, subtítulos, marcas de agua, leyendas y otros elementos de texto importantes a tus videos. Con el Video Edit SDK para .NET, puedes crear superposiciones de texto sofisticadas con control preciso sobre apariencia, posicionamiento y temporización.

## Características y Capacidades Principales

El SDK proporciona amplias opciones de personalización para superposiciones de texto, incluyendo:

- Selección de fuentes personalizadas de las fuentes instaladas en el sistema
- Control completo sobre tamaño, peso y estilo de fuente
- Opciones de color flexibles tanto para texto como para fondo
- Posicionamiento preciso con múltiples opciones de alineación
- Control de temporización para establecer cuándo aparece y desaparece el texto
- Configuraciones de transparencia y opacidad

## Ejemplo de Implementación

El siguiente código demuestra cómo crear y configurar una superposición de texto en tu aplicación .NET:

```cs
// Inicializar el objeto VideoEditCoreX (se asume que ya está creado)
// var videoEdit = new VideoEditCoreX();

// Crear un nuevo objeto de superposición de texto con el texto deseado
var textOverlay = new VisioForge.Core.Types.X.VideoEdit.TextOverlay("¡Hola mundo!");

// Establecer cuándo debe aparecer el texto y por cuánto tiempo
// Este ejemplo: el texto aparece 2 segundos en el video y permanece 5 segundos
textOverlay.Start = TimeSpan.FromMilliseconds(2000);
textOverlay.Duration = TimeSpan.FromMilliseconds(5000);

// Definir la posición del texto en el fotograma de video
// Las coordenadas X e Y se miden en píxeles desde la esquina superior izquierda
textOverlay.X = 50;
textOverlay.Y = 50;

// Configurar propiedades de fuente para el texto
textOverlay.FontFamily = "Arial";  // Establecer la familia de fuente
textOverlay.FontSize = 40;         // Establecer el tamaño de fuente en puntos
textOverlay.FontWidth = SkiaSharp.SKFontStyleWidth.Normal;   // Ancho de caracteres normal
textOverlay.FontSlant = SkiaSharp.SKFontStyleSlant.Italic;   // Aplicar estilo itálico
textOverlay.FontWeight = SkiaSharp.SKFontStyleWeight.Bold;   // Aplicar peso negrita

// Establecer el color del texto a rojo
textOverlay.Color = SkiaSharp.SKColors.Red;

// Establecer un fondo transparente detrás del texto
// Podrías usar cualquier color con canal alfa para semi-transparencia
textOverlay.BackgroundColor = SkiaSharp.SKColors.Transparent;

// Añadir la superposición de texto configurada a tu proyecto de video
videoEdit.Video_TextOverlays.Add(textOverlay);
```

## Opciones de Posicionamiento

El SDK usa las coordenadas X e Y para posicionamiento absoluto. X e Y representan coordenadas en píxeles desde la esquina superior izquierda del fotograma de video:

```cs
// Posicionar texto en coordenadas específicas (en píxeles)
textOverlay.X = 50;   // Posición horizontal desde el borde izquierdo
textOverlay.Y = 50;   // Posición vertical desde el borde superior

// También puedes posicionar el texto en otras áreas:
// Esquina inferior derecha (asumiendo video de 1920x1080):
// textOverlay.X = 1820;  // 1920 - 100 para algo de margen
// textOverlay.Y = 980;   // 1080 - 100 para algo de margen

// Centrado (asumiendo video de 1920x1080 y midiendo el tamaño del texto):
// textOverlay.X = 960;   // Mitad del ancho del video
// textOverlay.Y = 540;   // Mitad de la altura del video
```

## Trabajar con Fuentes

El SDK aprovecha la biblioteca SkiaSharp para capacidades potentes de renderizado de texto. Esto proporciona acceso a todas las fuentes del sistema y características tipográficas avanzadas.

### Obtener Familias de Fuentes Disponibles

Puedes recuperar dinámicamente la lista de fuentes disponibles en el sistema actual:

```cs
// Obtener todas las fuentes disponibles en el sistema actual
// Esto es útil para crear menús desplegables de selección de fuentes en tu UI
var availableFonts = videoEdit.Fonts;

// Ahora puedes iterar a través de las fuentes o vincularlas a un control de UI
foreach (var font in availableFonts)
{
    // Usar la información de fuente según sea necesario
    Console.WriteLine(font);
}
```

## Técnicas de Estilo Avanzadas

Para efectos de texto más sofisticados, considera combinar superposiciones de texto con otras características del SDK:

- Aplicar efectos de animación para hacer que el texto se mueva por la pantalla
- Usar múltiples superposiciones de texto con diferentes tiempos para visualización secuencial
- Combinar con superposiciones de forma para crear cuadros de texto con fondos personalizados
- Integrar con transiciones de video para efectos dinámicos de entrada y salida de texto

## Recursos de Desarrollo

Para más ejemplos de código y técnicas de implementación avanzadas, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) que contiene muestras completas del SDK .NET.
