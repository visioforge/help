---
title: Captura de Pantalla a AVI con Codificación MJPEG en C# .NET
description: Grabe la pantalla a formato AVI en C# con video MJPEG o sin comprimir. Cuándo elegir AVI sobre MP4, opciones de codec y ejemplos con VisioForge SDK.
sidebar_label: Captura de Pantalla a AVI
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - MJPEG
  - C#
  - NuGet
primary_api_classes:
  - AVIOutput
  - VideoCaptureCore
  - VideoCaptureCoreX
  - MJPEGEncoderSettings
  - VideoView

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

## Cuándo Usar AVI para Grabación de Pantalla

AVI (Audio Video Interleave) es un formato contenedor legado que sigue siendo útil en escenarios específicos:

- **Flujos de trabajo de edición basados en DirectShow** — Los archivos AVI se integran perfectamente con filtros DirectShow y herramientas de edición de video más antiguas
- **Codec MJPEG** — Cada fotograma se comprime independientemente, haciendo AVI ideal para edición de video fotograma a fotograma donde necesita acceso aleatorio a cualquier fotograma sin decodificar los anteriores
- **Máxima compatibilidad** — AVI es soportado por virtualmente todos los reproductores y editores de video en Windows, incluyendo aplicaciones legadas
- **Estructura de codec simple** — A diferencia del formato complejo basado en átomos de MP4, AVI tiene una estructura directa que es más fácil de recuperar de grabaciones incompletas

**Compensación:** Los archivos AVI con MJPEG son significativamente más grandes que los archivos MP4 con H.264. Una grabación de 1080p a 25 FPS produce aproximadamente 150–200 MB por minuto con MJPEG, comparado con ~25 MB por minuto con MP4 H.264.

Para la mayoría de los casos de uso de grabación de pantalla, [MP4 es el formato recomendado](screen-capture-mp4.md). Use AVI cuando específicamente necesite independencia de fotogramas MJPEG, compatibilidad con DirectShow o captura sin comprimir.

## API Moderna — Video Capture SDK X

La API moderna multiplataforma usa `VideoCaptureCoreX`. Para el patrón completo de aplicación de consola con configuración de captura de pantalla, audio y ciclo de vida de grabación, consulte la guía de [Captura de Pantalla a MP4](screen-capture-mp4.es.md#api-moderna-video-capture-sdk-x). Para generar AVI en lugar de MP4, reemplace la configuración de salida:

```csharp
// AVI con codecs predeterminados (OpenH264 video + MP3 audio)
var aviOutput = new AVIOutput(outputPath);
videoCapture.Outputs_Add(aviOutput, autostart: true);
```

### Opciones de Codec AVI

Elija el codificador de video según su flujo de trabajo:

```csharp
// MJPEG — independiente por fotograma, archivos más grandes, ideal para edición
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new MJPEGEncoderSettings();

// H.264 en contenedor AVI — archivos más pequeños, menos amigable para edición
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new OpenH264EncoderSettings();
```

Captura de región, grabación multi-monitor, audio, resaltado de cursor y codificación GPU se cubren en la [guía de MP4](screen-capture-mp4.es.md) — todas las funciones de configuración de fuente funcionan de forma idéntica con la salida AVI.

## API Legada — Video Capture SDK

Este ejemplo WPF demuestra la captura de pantalla a AVI usando la API legada `VideoCaptureCore`. Agregue un control `VideoView` llamado `VideoView1` a su ventana XAML.

### Ejemplo de Código

```csharp
using System;
using System.IO;
using System.Windows;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_avi
{
    public partial class MainWindow : Window
    {
        private VideoCaptureCore videoCapture1;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            // Capturar toda la pantalla
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
            {
                FullScreen = true
            };

            // Solo video — sin audio
            videoCapture1.Audio_RecordAudio = false;
            videoCapture1.Audio_PlayAudio = false;

            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                "output.avi");

            // Salida AVI con video MJPEG y audio PCM
            videoCapture1.Output_Format = new AVIOutput();
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            await videoCapture1.StopAsync();
        }
    }
}
```

### Dependencias Requeridas

Instale los siguientes paquetes NuGet:

- Componentes redistribuibles de captura de video:
  - [versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Preguntas Frecuentes

### ¿Por qué mi archivo de grabación de pantalla AVI es tan grande?

MJPEG comprime cada fotograma independientemente, sacrificando tamaño de archivo por conveniencia de edición (vea la comparación de tamaño arriba). Para reducir el tamaño del archivo: reduzca la tasa de fotogramas (10–15 FPS es suficiente para presentaciones), capture una región más pequeña en lugar de pantalla completa, o cambie a [salida MP4](screen-capture-mp4.es.md) que usa compresión H.264 inter-fotograma y produce archivos aproximadamente 6–8x más pequeños.

### ¿Cuándo debo usar AVI en lugar de MP4 para grabación de pantalla?

Elija AVI cuando necesite acceso independiente a fotogramas para edición de video (MJPEG permite navegar a cualquier fotograma sin decodificar los anteriores), cuando trabaje con pipelines basados en DirectShow legados, o cuando necesite máxima compatibilidad con herramientas de video antiguas de Windows. Para todos los demás casos, MP4 con H.264 ofrece mejor compresión, archivos más pequeños y soporte multiplataforma más amplio.

## Ver También

- [Captura de Pantalla a MP4](screen-capture-mp4.md) — formato recomendado con cobertura completa de funciones (región, multi-monitor, audio, codificación GPU, multiplataforma)
- [Captura de Pantalla a WMV](screen-capture-wmv.md) — alternativa en formato Windows Media
- [Captura de Pantalla en VB.NET](../guides/screen-capture-vb-net.md) — grabación de pantalla en Visual Basic .NET
- [Configuración de Fuente de Pantalla](../video-sources/screen.md) — referencia completa para configuración de captura
- [Ejemplos de Código](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — ejemplos adicionales de captura de pantalla en GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas
