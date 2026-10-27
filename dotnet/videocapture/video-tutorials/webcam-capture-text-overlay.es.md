---
title: Superposición de Texto en Video de Webcam (C# .NET)
description: Capture video de webcam y agregue superposiciones de texto personalizadas en C# .NET con instrucciones paso a paso, ejemplos de código completos y técnicas.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Playback
  - Webcam
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - IVideoView
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4Output

---

# Agregar superposiciones de texto a captura de video de webcam en C# .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial en video paso a paso

El siguiente video demuestra el proceso de captura de video de una webcam y agregado de superposiciones de texto usando C# y .NET:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/D_JPo9A9HMA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Acceso al código fuente

Acceda al código fuente completo para esta implementación en nuestro repositorio de GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-text-overlay)

## Dependencias requeridas

Para implementar esta funcionalidad en su aplicación, necesitará instalar los siguientes paquetes NuGet:

- **Dependencias de captura de video:**
  - [versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
  
- **Dependencias de procesamiento MP4:**
  - [versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
  - [versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Descripción general de implementación

Este tutorial demuestra cómo:

- Acceder y capturar video de una webcam conectada
- Procesar el stream de video en tiempo real
- Agregar superposiciones de texto personalizables con color y posicionamiento
- Guardar el video capturado con superposición de texto a un archivo MP4

La implementación usa Windows Forms para la interfaz de usuario, pero la funcionalidad core de captura y superposición de texto funciona con cualquier framework de UI .NET.

## Implementación completa en C#

El siguiente ejemplo de código demuestra una implementación completa de captura de webcam con funcionalidad de superposición de texto:

```csharp
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;

namespace video_capture_text_overlay
{
    public partial class Form1 : Form
    {
        // El componente principal de captura de video que maneja toda la funcionalidad de captura
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inicia la captura de video con superposición de texto
        /// </summary>
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurar el dispositivo de captura de video - usa la primera cámara disponible
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configurar el dispositivo de captura de audio - usa el primer micrófono disponible
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Establecer el modo de captura a captura de video estándar
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;
            
            // Establecer el nombre del archivo de salida para guardar en la carpeta Videos del usuario
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
            
            // Configurar MP4 como el formato de salida - también soporta otros formatos
            videoCapture1.Output_Format = new MP4Output();

            // Agregar superposición de texto al video
            // Paso 1: Habilitar efectos de video
            videoCapture1.Video_Effects_Enabled = true;
            
            // Paso 2: Limpiar cualquier efecto existente
            videoCapture1.Video_Effects_Clear();
            
            // Paso 3: Crear un nuevo efecto de superposición de texto
            var textOverlay = new VideoEffectTextLogo(true) { 
                Text = "¡Hola Mundo!",  // El texto a mostrar
                Top = 50,               // Posición Y (desde arriba)
                Left = 50,              // Posición X (desde izquierda)
                FontColor = Color.Red   // Color del texto
            };
            
            // Paso 4: Agregar la superposición de texto a la colección de efectos
            videoCapture1.Video_Effects_Add(textOverlay);

            // Iniciar el proceso de captura de forma asíncrona
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Detiene la captura de video
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener el proceso de captura de forma asíncrona
            await videoCapture1.StopAsync();
        }

        /// <summary>
        /// Inicializar el componente de captura de video cuando el formulario se carga
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar el VideoCaptureCore con el control de vista de video
            // VideoView1 debe ser un control en su formulario que implemente IVideoView
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
            
            // Opciones de inicialización adicionales:
            // videoCapture1.Audio_PlayAudio = true;  // Reproducir audio durante la captura
            // videoCapture1.Audio_RecordAudio = true;  // Grabar audio con video
        }
    }
}
```

## Detalles clave de implementación

### Selección y configuración de dispositivo

El código selecciona automáticamente la primera cámara y dispositivo de micrófono disponibles para captura de video y audio. En un entorno de producción, podría querer proporcionar a los usuarios opciones para seleccionar dispositivos específicos.

### Propiedades de superposición de texto

La implementación de superposición de texto soporta varias opciones de personalización:

- **Contenido de texto**: Cualquier cadena puede mostrarse en el video
- **Posición**: Especifique las coordenadas exactas para colocación del texto
- **Color**: Elija cualquier color para la visualización del texto
- **Tamaño y estilo**: Personalice fuente, tamaño y estilo (opciones comentadas)

### Operación asíncrona

La implementación usa el patrón async/await de C# para operaciones de video sin bloqueo, asegurando que su aplicación permanezca receptiva durante la captura.

### Salida de archivo

El video capturado con superposición de texto se guarda como archivo MP4 en la carpeta Videos del usuario. El formato de salida y ubicación pueden personalizarse según los requisitos de su aplicación.

## Recursos adicionales

Para más ejemplos y muestras de código relacionadas con procesamiento y manipulación de video, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
