---
title: Grabación de Webcam a MP4 en .NET - Tutorial C#
description: Implementa captura de video de webcam a archivos MP4 en .NET con ejemplos de código C# detallados mostrando integración de grabación de video de alta calidad.
---

# Implementación de Captura de Video de Webcam a MP4 en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Captura de Webcam en Aplicaciones .NET

Crear aplicaciones que graben video de webcams a archivos MP4 es un requisito común para muchos proyectos de software. Este tutorial proporciona un enfoque orientado a desarrolladores para implementar esta funcionalidad usando técnicas modernas de .NET y el Video Capture SDK.

Al implementar capacidades de grabación de webcam, los desarrolladores necesitan considerar:

- Seleccionar los dispositivos de entrada de video y audio apropiados
- Configurar ajustes de compresión de video y audio
- Gestionar el ciclo de vida de captura (inicialización, inicio, detención)
- Manejar la creación y gestión del archivo de salida

## Tutorial de Implementación en Video

El siguiente video demuestra el proceso completo de configurar una aplicación de captura de webcam:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/TunCZ_2bNr8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de Código Fuente

Para desarrolladores que prefieren examinar la implementación completa, todo el código fuente está disponible en nuestro repositorio de GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-mp4)

## Componentes Redistribuibles Requeridos

Antes de implementar la solución, asegúrate de haber instalado los paquetes redistribuibles necesarios:

### Componentes de Captura de Video

- Para aplicaciones de 32 bits: [Redist de Captura de Video x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- Para aplicaciones de 64 bits: [Redist de Captura de Video x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Componentes de Codificación MP4

- Para aplicaciones de 32 bits: [Redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- Para aplicaciones de 64 bits: [Redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Ejemplo de Implementación con Código C#

La siguiente implementación en C# demuestra cómo crear una aplicación de Windows Forms que captura video de una webcam y lo guarda a un archivo MP4. El código incluye inicialización, configuración y gestión de recursos apropiadas.

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Importar librerías de VisioForge para funcionalidad de captura de video
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_mp4
{
    public partial class Form1 : Form
    {
        // El objeto principal de captura de video que controla el proceso de captura
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Manejador de clic del botón Iniciar - configura e inicia la captura de video
        /// </summary>
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Seleccionar el primer dispositivo de video disponible (webcam) del sistema
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Seleccionar el primer dispositivo de audio disponible (micrófono) del sistema
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Establecer la ruta del archivo de salida a la carpeta Videos del usuario con nombre "output.mp4"
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
            
            // Configurar formato de salida como MP4 con configuración predeterminada (video H.264, audio AAC)
            videoCapture1.Output_Format = new MP4Output();
            
            // Establecer el modo a VideoCapture para capturar tanto video como audio
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Iniciar el proceso de captura de forma asíncrona
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Manejador de carga del formulario - inicializa el componente de captura de video
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar el objeto VideoCaptureCore, conectándolo al control VideoView en el formulario
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        /// <summary>
        /// Manejador de clic del botón Detener - detiene la captura activa
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener el proceso de captura de forma asíncrona y finalizar el archivo de salida
            await videoCapture1.StopAsync();
        }
    }
}
```

## Características Clave de Implementación

Esta implementación proporciona varias capacidades importantes para desarrolladores:

1. **Selección Automática de Dispositivo** - El código selecciona automáticamente los primeros dispositivos de video y audio disponibles
2. **Formato de Salida Estándar** - Configura salida MP4 con codecs de video H.264 y audio AAC estándar de la industria
3. **Operación Asíncrona** - Usa patrón async/await para UI sin bloqueo durante operaciones de captura
4. **Integración Simple** - Fácil de incorporar en aplicaciones Windows Forms existentes

## Opciones de Configuración Avanzada

Mientras el ejemplo muestra una implementación básica, los desarrolladores pueden extender la solución con características adicionales:

- Configuraciones personalizadas de resolución de video y tasa de frames
- Ajustes de bitrate para control de calidad
- Efectos de video y transformaciones al vuelo
- Soporte de múltiples pistas de audio

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y ejemplos de implementación.