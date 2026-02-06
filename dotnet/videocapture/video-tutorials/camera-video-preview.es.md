---
title: Captura y Vista Previa de Frames de Webcam en C#
description: Implementa vista previa de video de webcam en tiempo real con captura de frames en C# .NET con ejemplos de código funcionales para WinForms, WPF y Console.
---

# Implementación de Vista Previa de Video de Webcam con Captura de Frames en C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción General

Este tutorial demuestra cómo crear una aplicación profesional de vista previa de video de webcam con la capacidad de capturar frames individuales como imágenes. Esta funcionalidad es esencial para desarrollar aplicaciones que requieren análisis de imagen, capacidades de captura de instantáneas o interfaces de cámara personalizadas.

## Tutorial de Video Paso a Paso

Mira nuestro video detallado que cubre todos los aspectos de la integración de webcam:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/kxC6JrJddek?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Primeros Pasos

Antes de sumergirte en el código, necesitarás configurar tu entorno de desarrollo con las dependencias necesarias. El código fuente completo está disponible en GitHub como referencia:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-preview-webcam-frame-capture)

## Guía de Implementación

### Dependencias Requeridas

Primero, asegúrate de tener instalados los siguientes paquetes NuGet:

- Redistribuibles de captura de video [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Ejemplo de Código Fuente

La siguiente implementación en C# demuestra cómo:

1. Inicializar un componente de captura de video
2. Conectar a un dispositivo de webcam
3. Mostrar vista previa de video en tiempo real
4. Capturar y guardar frames individuales como imágenes JPEG

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Referencias del SDK VisioForge para funcionalidad de captura de video
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_preview_webcam_frame_capture
{
    public partial class Form1 : Form
    {
        // Componente principal de captura de video para manejar la entrada de cámara
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        // Inicialización del formulario - crea una nueva instancia de VideoCaptureCore conectada a nuestra vista de video
        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar el componente de captura de video con el control VideoView
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        // Manejador de clic del botón Iniciar - configura e inicia la captura de video
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurar el dispositivo de captura de video predeterminado (primera cámara disponible)
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configurar el dispositivo de captura de audio predeterminado (primer micrófono disponible)
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Establecer modo a VideoPreview (solo visualización, sin grabación)
            videoCapture1.Mode = VideoCaptureMode.VideoPreview;

            // Iniciar la vista previa de video de forma asíncrona
            await videoCapture1.StartAsync();
        }

        // Manejador de clic del botón Detener - detiene la captura de video
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener la vista previa de video de forma asíncrona
            await videoCapture1.StopAsync();
        }

        // Manejador de clic del botón Guardar Frame - captura y guarda el frame actual como JPEG
        private async void btSaveFrame_Click(object sender, EventArgs e)
        {
            // Guardar el frame actual como imagen JPEG en la carpeta Imágenes del usuario
            // El parámetro de calidad (85) especifica el nivel de compresión JPEG (0-100)
            await videoCapture1.Frame_SaveAsync(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "frame.jpg"),
                ImageFormat.Jpeg,
                85);
        }
    }
}
```

## Desglose del Código

### Inicialización

La aplicación comienza creando una nueva instancia de `VideoCaptureCore` durante la carga del formulario, que sirve como la interfaz principal para interactuar con dispositivos de webcam.

### Selección de Dispositivo

Cuando el usuario hace clic en el botón Iniciar, la aplicación selecciona automáticamente los primeros dispositivos de captura de video y audio disponibles. En un entorno de producción, podrías querer proporcionar una selección desplegable para usuarios con múltiples cámaras.

### Modo de Vista Previa

La aplicación está configurada en modo `VideoPreview`, que habilita la visualización en tiempo real sin grabar el stream a disco. Esto es ideal para aplicaciones que solo necesitan mostrar la salida de la cámara.

### Captura de Frame

La funcionalidad de captura de frame demuestra cómo guardar el frame de video actual como una imagen JPEG con un nivel de calidad especificado. La imagen se guarda en la carpeta Imágenes del usuario por defecto.

## Aplicaciones Avanzadas

Este código puede extenderse para soportar varias aplicaciones del mundo real:

- Escaneo de documentos
- Aplicaciones de cabina de fotos
- Videoconferencia
- Visión por computadora y procesamiento de imágenes
- Sistemas de seguridad y vigilancia

## Recursos Adicionales

Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para explorar más ejemplos de código y técnicas de implementación avanzada.
