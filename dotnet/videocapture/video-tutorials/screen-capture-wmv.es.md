---
title: Captura de Pantalla a WMV en C# .NET — Windows Media
description: Grabe la pantalla a formato WMV en C# con codecs Windows Media. Cuándo elegir WMV, configuración de codec y ejemplos de código completos usando VisioForge SDK.
sidebar_label: Captura de Pantalla a WMV
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - Screen Capture
  - MP4
  - AVI
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - WMVOutput
  - VideoCaptureCoreX
  - VideoView
  - IVideoView

---

# Implementación de Grabación de Pantalla a WMV en Aplicaciones C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial en Video Paso a Paso

Mira nuestro tutorial detallado en video que demuestra cada paso del proceso de implementación:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/8JYSDw2JeAo?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de Código Fuente

Accede al código fuente completo para este tutorial en nuestro repositorio de GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-wmv)

## Cuándo Usar WMV para Grabación de Pantalla

WMV (Windows Media Video) usa los codecs Windows Media de Microsoft y el contenedor ASF. Sigue siendo útil en escenarios específicos centrados en Windows:

- **Integración nativa con Windows** — Los archivos WMV se reproducen en Windows Media Player sin codecs adicionales y se integran con Windows Movie Maker y otras herramientas de Microsoft
- **Streaming ASF** — El contenedor ASF soporta streaming en vivo a través de Windows Media Services, útil para transmisión en intranet
- **Archivos más pequeños que AVI** — La compresión WMV es más eficiente que MJPEG, aunque menos eficiente que MP4 H.264
- **Entornos empresariales legados** — Muchos entornos corporativos estandarizan los formatos Windows Media para distribución interna de video

**Compensación:** WMV es un formato solo para Windows con soporte limitado en macOS y Linux. Para compatibilidad multiplataforma, [MP4 es el formato recomendado](screen-capture-mp4.md).

## API Moderna — Video Capture SDK X

La API moderna multiplataforma usa `VideoCaptureCoreX`. Para el patrón completo de aplicación de consola con configuración de captura de pantalla, audio y ciclo de vida de grabación, consulte la guía de [Captura de Pantalla a MP4](screen-capture-mp4.es.md#api-moderna-video-capture-sdk-x). Para generar WMV en lugar de MP4, reemplace la configuración de salida:

```csharp
// Salida WMV (codecs Windows Media Video + WMA audio)
var wmvOutput = new WMVOutput(outputPath);
videoCapture.Outputs_Add(wmvOutput, autostart: true);
```

Captura de región, grabación multi-monitor, audio, resaltado de cursor y codificación GPU se cubren en la [guía de MP4](screen-capture-mp4.es.md) — todas las funciones de configuración de fuente funcionan de forma idéntica con la salida WMV.

## API Legada — Video Capture SDK

### Dependencias Requeridas

Antes de comenzar, asegúrate de haber instalado los paquetes redistribuibles necesarios:

- Redistribuibles de captura de video:
  - [paquete x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [paquete x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Ejemplo de Código

El siguiente fragmento de código demuestra cómo crear una aplicación básica de grabación de pantalla que captura tu pantalla a un archivo WMV:

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

namespace screen_capture_wmv
{
    public partial class Form1 : Form
    {
        // Declarar el objeto principal VideoCaptureCore que manejará la grabación de pantalla
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            // Inicializar componentes del formulario (botones, paneles, etc.)
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar la instancia de VideoCaptureCore y asociarla con el control VideoView
            // VideoView1 es un control de UI donde se mostrará la vista previa de captura de pantalla
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurar captura de pantalla para grabar la pantalla completa
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { FullScreen = true };

            // Deshabilitar grabación y reproducción de audio
            videoCapture1.Audio_RecordAudio = videoCapture1.Audio_PlayAudio = false;

            // Establecer la ruta del archivo de salida a la carpeta Videos del usuario
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");

            // Establecer el formato de salida a WMV con configuración predeterminada
            videoCapture1.Output_Format = new WMVOutput();

            // Establecer el modo de captura a captura de pantalla
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Iniciar el proceso de captura de pantalla de forma asíncrona
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener el proceso de captura de pantalla de forma asíncrona
            await videoCapture1.StopAsync();
        }
    }
}
```

## Preguntas Frecuentes

### ¿Cuándo debo usar WMV en lugar de MP4 para grabación de pantalla?

Elija WMV cuando su público objetivo usa Windows exclusivamente y necesita reproducción nativa sin instalación de codecs adicionales, cuando distribuye video a través de Windows Media Services o SharePoint, o cuando trabaja dentro de entornos empresariales que estandarizan los formatos Windows Media. Para distribución multiplataforma o publicación web, MP4 con H.264 es la mejor opción — ofrece archivos más pequeños, soporte más amplio de dispositivos y mejor compresión.

## Ver También

- [Captura de Pantalla a MP4](screen-capture-mp4.md) — formato recomendado con cobertura completa de funciones (región, multi-monitor, audio, codificación GPU, multiplataforma)
- [Captura de Pantalla a AVI](screen-capture-avi.md) — formato AVI con MJPEG para edición independiente de fotogramas
- [Captura de Pantalla en VB.NET](../guides/screen-capture-vb-net.md) — grabación de pantalla en Visual Basic .NET
- [Configuración de Fuente de Pantalla](../video-sources/screen.md) — referencia completa para configuración de captura
- [Ejemplos de Código](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — ejemplos adicionales de captura de pantalla en GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas
