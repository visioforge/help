---
title: Grabación de Webcam a AVI en Aplicaciones .NET C#
description: Implementa grabación de webcam a archivo AVI en .NET con tutorial paso a paso incluyendo ejemplo de código C# completo con async y selección de dispositivo.
---

# Grabación de Webcam a Archivo AVI en Aplicaciones .NET C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción General

Este tutorial demuestra cómo capturar video de una webcam y guardarlo directamente a formato de archivo AVI en tus aplicaciones .NET. La implementación usa técnicas de programación asíncrona para una respuesta de UI fluida y proporciona un enfoque de arquitectura limpia adecuado para desarrollo de software profesional.

## Tutorial en Video

Mira nuestro tutorial de video detallado cubriendo el proceso de implementación:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/8yFKz1QOJbk?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de Código Fuente

Accede al código fuente completo desde nuestro repositorio oficial de GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-avi)

## Dependencias Requeridas

Antes de implementar esta solución, asegúrate de haber instalado las dependencias necesarias:

- Redistribuibles de captura de video:
  - [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Detalles de Implementación

### Prerrequisitos

Antes de comenzar, asegúrate de tener:

- Visual Studio con herramientas de desarrollo .NET
- Una webcam conectada a tu máquina de desarrollo
- Entendimiento básico de aplicaciones Windows Forms

### Implementación de Código

El siguiente ejemplo demuestra una implementación completa para captura de webcam a archivo AVI:

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

// Importar namespaces del SDK VisioForge
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_avi
{
    public partial class Form1 : Form
    {
        // Declarar el objeto VideoCaptureCore que manejará todas las operaciones de captura
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            // Inicialización estándar de Windows Forms
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar el objeto VideoCaptureCore y vincularlo al control VideoView
            // VideoView1 es un control que muestra la vista previa del video
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Establecer el dispositivo de captura de video - usando la primera cámara disponible
            // videoCapture1.Video_CaptureDevices() retorna una lista de dispositivos de video disponibles
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Establecer el dispositivo de captura de audio - usando el primer micrófono disponible
            // videoCapture1.Audio_CaptureDevices() retorna una lista de dispositivos de audio disponibles
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Establecer el nombre del archivo de salida a "output.avi" en la carpeta Videos del usuario
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");

            // Configurar formato de salida como AVI
            // Por defecto, esto usa codec MJPEG para video y PCM para audio
            videoCapture1.Output_Format = new AVIOutput();
            
            // Establecer el modo a VideoCapture (otros modos incluyen ScreenCapture, AudioCapture, etc.)
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Iniciar el proceso de captura de forma asíncrona
            // Usando patrón async/await para UI sin bloqueo
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener el proceso de captura de forma asíncrona
            await videoCapture1.StopAsync();
        }
    }
}
```

### Puntos Clave de Implementación

#### Selección de Dispositivo

El ejemplo selecciona automáticamente los primeros dispositivos de video y audio disponibles. En una aplicación de producción, podrías querer presentar a los usuarios una lista de dispositivos disponibles para selección.

#### Configuración de Salida de Archivo

El ejemplo guarda la salida a un archivo AVI en la carpeta Videos del usuario. Puedes personalizar la ruta y el nombre del archivo según los requisitos de tu aplicación.

#### Operación Asíncrona

La implementación usa el patrón async/await para prevenir el congelamiento de la UI durante las operaciones de captura, lo cual es crítico para mantener una experiencia de aplicación receptiva.

## Opciones de Personalización Avanzada

El SDK proporciona numerosas opciones de personalización incluyendo:

- Configuraciones de resolución y tasa de frames de video
- Configuración de calidad de audio
- Selección de codec para diferentes requisitos de salida
- Efectos de video y transformaciones

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y explorar escenarios de implementación adicionales.