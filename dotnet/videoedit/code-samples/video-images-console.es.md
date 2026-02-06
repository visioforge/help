---
title: Crear Videos desde Imágenes en Consola C#
description: Genera videos desde secuencias de imágenes en C# con orientación paso a paso, ejemplos de código y consejos de rendimiento para .NET.
---

# Crear Videos desde Imágenes en Aplicaciones de Consola C#

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introducción

Convertir una secuencia de imágenes en un archivo de video es un requisito común para muchas aplicaciones de software. Esta guía demuestra cómo crear un video desde imágenes usando una aplicación de consola C# con el Video Edit SDK .Net. El mismo enfoque funciona para aplicaciones WinForms y WPF con modificaciones mínimas.

## Prerrequisitos

Antes de comenzar, asegúrate de tener:

- Entorno de desarrollo .NET configurado
- Video Edit SDK .Net instalado
- Conocimiento básico de programación C#
- Una carpeta que contenga archivos de imagen (JPG, PNG, etc.)

## Conceptos Clave

Al crear videos desde imágenes, entender estos conceptos fundamentales te ayudará a lograr mejores resultados:

- **Tasa de fotogramas**: Determina qué tan suavemente se reproduce tu video (típicamente 25-30 fotogramas por segundo)
- **Duración de imagen**: Cuánto tiempo aparece cada imagen en el video
- **Efectos de transición**: Efectos opcionales entre imágenes
- **Formato de salida**: El contenedor de video y especificaciones de códec
- **Resolución**: Las dimensiones del video de salida

## Implementación Paso a Paso

### Configurar el Proyecto

Primero, crea un nuevo proyecto de aplicación de consola y añade las referencias necesarias:

```cs
using System;
using System.IO;
using VisioForge.Types;
using VisioForge.Types.Output;
using VisioForge.VideoEdit;
using VisioForge.Controls;
using VisioForge.Controls.VideoEdit;
```

### Implementación Principal

```cs
namespace ve_console
{
    class Program
    {
        // La carpeta contiene imágenes
        private const string AssetDir = "c:\\samples\\pics\\";

        static void Main(string[] args)
        {
            if (!Directory.Exists(AssetDir))
            {
                Console.WriteLine(@"La carpeta con imágenes no existe: " + AssetDir);
                return;
            }

            var images = Directory.GetFiles(AssetDir, "*.jpg");
            if (images.Length == 0)
            {
                Console.WriteLine(@"La carpeta con imágenes está vacía o no tiene archivos con extensión .jpg: " + AssetDir);
                return;
            }

            if (File.Exists(AssetDir + "output.avi"))
            {
                File.Delete(AssetDir + "output.avi");
            }

            var ve = new VideoEditCore();

            int insertTime = 0;

            foreach (string img in images)
            {
                ve.Input_AddImageFile(img, TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(insertTime), VideoEditStretchMode.Letterbox, 0, 640, 480);
                insertTime += 2000;
            }

            ve.Video_Effects_Clear();
            ve.Mode = VideoEditMode.Convert;

            ve.Video_Resize = true;
            ve.Video_Resize_Width = 640;
            ve.Video_Resize_Height = 480;

            ve.Video_FrameRate = 25;
            ve.Video_Renderer = new VideoRendererSettings
            {
                VideoRenderer = VideoRendererMode.None,
                StretchMode = VideoRendererStretchMode.Letterbox
            };

            var aviOutput = new AVIOutput
            {
                Video_Codec = "MJPEG Compressor"
            };

            ve.Output_Format = aviOutput;
            ve.Output_Filename = AssetDir + "output.avi";

            ve.Video_Effects_Enabled = true;
            ve.Video_Effects_Clear();

            ve.OnError += VideoEdit1_OnError;
            ve.OnProgress += VideoEdit1_OnProgress;

            ve.ConsoleUsage = true;

            ve.Start();

            Console.WriteLine(@"Video guardado en: " + ve.Output_Filename);
        }

        private static void VideoEdit1_OnProgress(object sender, ProgressEventArgs progressEventArgs)
        {
            Console.WriteLine(progressEventArgs.Progress);
        }

        private static void VideoEdit1_OnError(object sender, ErrorsEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
```

## Desglose Detallado de Componentes

### Configuración de Entrada de Imágenes

El código anterior usa `Input_AddImageFile()` para añadir cada imagen a la línea de tiempo con parámetros específicos:

- **Ruta del archivo**: Ruta al archivo de imagen
- **Duración**: Cuánto tiempo aparece la imagen (2000ms en este ejemplo)
- **Tiempo de inicio**: Cuándo aparece la imagen en la línea de tiempo
- **Modo de estiramiento**: Cómo encaja la imagen en el fotograma de video (Letterbox preserva la relación de aspecto)
- **Rotación**: Rotación de la imagen en grados
- **Ancho/Altura**: Dimensiones para la imagen en el video

### Configuración de Salida de Video

La configuración del video de salida se establece con estas propiedades clave:

- **Video_Resize**: Habilitar/deshabilitar redimensionamiento
- **Video_Resize_Width/Height**: Dimensiones del video de salida
- **Video_FrameRate**: Fotogramas por segundo (25 FPS es estándar para PAL)
- **Video_Renderer**: Configuración de renderizado incluyendo modo y estiramiento
- **Output_Format**: Formato de contenedor y configuración de códec
- **Output_Filename**: Dónde guardar el archivo de video resultante

### Manejo de Progreso y Errores

La implementación incluye manejadores de eventos para monitorear el progreso y capturar errores:

```cs
ve.OnError += VideoEdit1_OnError;
ve.OnProgress += VideoEdit1_OnProgress;
```

Estos manejadores proporcionan retroalimentación durante la creación del video, lo cual es esencial para operaciones más largas.

## Opciones de Personalización Avanzadas

### Efectos de Transición

Para añadir transiciones entre imágenes, puedes usar el método `Video_Transition_Add`:

```cs
// Ejemplo de añadir una transición de fundido entre imágenes

// Obtener el ID para el efecto de transición "FadeIn"
int transitionId = ve.Video_Transition_GetIDFromName("FadeIn");

// Añadir la transición - los parámetros son tiempo de inicio, tiempo de fin e ID de transición
ve.Video_Transition_Add(
    TimeSpan.FromMilliseconds(1900),  // Tiempo de inicio de la transición
    TimeSpan.FromMilliseconds(2100),  // Tiempo de fin de la transición
    transitionId                      // ID de la transición
);

// Para opciones de transición más avanzadas con borde y otras propiedades:
// ve.Video_Transition_Add(
//     TimeSpan.FromMilliseconds(1900),  // Tiempo de inicio
//     TimeSpan.FromMilliseconds(2100),  // Tiempo de fin
//     transitionId,                     // ID de la transición
//     Color.Blue,                       // Color del borde
//     5,                                // Suavidad del borde
//     2,                                // Ancho del borde
//     0,                                // Desplazamiento X
//     0,                                // Desplazamiento Y
//     0,                                // Replicar X
//     0,                                // Replicar Y
//     1,                                // Escala X
//     1                                 // Escala Y
// );
```

## Consejos de Optimización de Rendimiento

- **Pre-redimensionar imágenes**: Para mejor rendimiento, redimensiona las imágenes antes del procesamiento
- **Procesamiento por lotes**: Procesa imágenes en lotes más pequeños para grandes colecciones
- **Gestión de memoria**: Libera objetos grandes cuando ya no se necesiten
- **Códec de salida**: Elige códecs basándote en requisitos de calidad vs. velocidad de procesamiento
- **Aceleración por hardware**: Habilita la aceleración por hardware cuando esté disponible

## Solución de Problemas Comunes

### Errores de Códec Faltante

Si encuentras errores relacionados con códecs, asegúrate de haber instalado los redistribuibles requeridos:

- Redistribuible de Video Edit SDK [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

### Compatibilidad de Formato de Imagen

No todos los formatos de imagen son soportados igualmente. Para mejores resultados:

- Usa formatos comunes como JPG, PNG o BMP
- Asegura dimensiones consistentes entre imágenes
- Prueba con un pequeño subconjunto antes de procesar grandes colecciones

## Conclusión

Crear videos desde imágenes en una aplicación de consola C# es sencillo con el enfoque correcto. Esta guía cubrió los detalles esenciales de implementación, opciones de configuración y mejores prácticas para ayudarte a integrar exitosamente esta funcionalidad en tus aplicaciones.

Recuerda ajustar los parámetros para que coincidan con tus requisitos específicos, particularmente la duración de la imagen, la tasa de fotogramas y la configuración del formato de salida.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para más muestras de código e implementaciones.