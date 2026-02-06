---
title: Captura de Pantalla a AVI con C#
description: Implementa captura de pantalla con cursor del ratón a archivos de video AVI en C# con guía paso a paso y ejemplos de código fuente completos para grabación.
---

# Guía de Implementación de Captura de Pantalla a AVI en C#

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial en Video Paso a Paso

Mira nuestro tutorial detallado que demuestra el proceso de implementación:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/AUT8oVPinUs?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de Código Fuente

Accede al código fuente completo para este tutorial:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-avi)

## Ejemplo de Código de Implementación

A continuación se muestra la implementación completa en C# para capturar tu pantalla a un archivo AVI:

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
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_avi
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { FullScreen = true };
            videoCapture1.Audio_RecordAudio = videoCapture1.Audio_PlayAudio = false;
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");

            // Salida AVI predeterminada con MJPEG para video y PCM para audio
            videoCapture1.Output_Format = new AVIOutput(); 

            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

## Explicación del Código

La implementación muestra:

- Captura de toda la pantalla con una configuración simple
- Guardado de la salida en la carpeta Videos del usuario
- Uso de compresión MJPEG para el formato AVI
- Métodos de inicio y detención asíncronos para mejor capacidad de respuesta de la aplicación

## Dependencias Requeridas

Para usar este código en tu proyecto, instala los siguientes paquetes NuGet:

- Componentes redistribuibles de captura de video:
  - [versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Recursos Adicionales

Para más ejemplos y técnicas de implementación avanzada:

- Visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos de código adicionales
- Explora opciones de personalización para regiones de captura de pantalla, calidad de video y formatos

Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
