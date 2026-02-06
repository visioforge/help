---
title: Captura de Pantalla a MP4 con C# y .NET
description: Implementa grabación de pantalla a MP4 con C# en .NET usando ejemplos de código completos para capturas con audio y silenciosas con codificación H264/AAC.
---

# Captura de pantalla a archivo MP4

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial de YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/fPJEoOz6lIM?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-mp4)

## Redistribuibles requeridos

- Redistribuibles de captura de video [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuibles MP4 [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Ejemplo de Código

```csharp
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_mp4
{
    public partial class Form1 : Form
    {
        // Componente principal de VideoCapture que maneja todas las operaciones de grabación
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inicia la grabación de pantalla con audio del dispositivo predeterminado
        /// </summary>
        private async void btStartWithAudio_Click(object sender, EventArgs e)
        {
            // Configurar captura de pantalla para grabar toda la pantalla
            // ScreenCaptureSourceSettings permite control fino de la región de captura y parámetros
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true  // Captura toda la pantalla en lugar de una región específica
            };

            // Configurar captura de audio seleccionando el primer dispositivo de entrada de audio disponible
            // Audio_CaptureDevices() retorna todos los micrófonos y entradas de audio conectados
            // Seleccionamos el primer dispositivo (índice 0) en la colección
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(
                videoCapture1.Audio_CaptureDevices()[0].Name);

            // Deshabilitar monitoreo/reproducción de audio durante la grabación para prevenir retroalimentación
            // Esto significa que no escucharemos el audio capturado mientras grabamos
            videoCapture1.Audio_PlayAudio = false;

            // Habilitar grabación de audio para incluir sonido en el archivo de salida
            videoCapture1.Audio_RecordAudio = true;

            // Establecer la ubicación del archivo de salida en la carpeta Videos del usuario
            // Environment.GetFolderPath asegura que la ruta funcione en diferentes sistemas Windows
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Usar formato contenedor MP4 con codecs de video H.264 y audio AAC (formato estándar)
            // MP4Output puede configurarse más con parámetros de codificación personalizados si es necesario
            videoCapture1.Output_Format = new MP4Output();
            
            // Establecer el modo de captura a grabación de pantalla
            // Otros modos incluyen captura de cámara, procesamiento de archivo de video, etc.
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Comenzar el proceso de captura de forma asíncrona
            // Usando patrón async/await para prevenir congelamiento de UI durante la operación
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Inicia la grabación de pantalla sin audio (solo video)
        /// </summary>
        private async void btStartWithoutAudio_Click(object sender, EventArgs e)
        {
            // Configurar captura de pantalla para grabación a pantalla completa
            // Misma ScreenCaptureSourceSettings que en la grabación con audio
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true 
            };

            // Deshabilitar tanto reproducción como grabación de audio en una sola línea
            // Esto crea un archivo MP4 solo de video sin pista de audio
            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;

            // Establecer ruta de archivo de salida al directorio Videos del usuario con extensión MP4
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Configurar salida como MP4 (codec de video H.264)
            videoCapture1.Output_Format = new MP4Output();
            
            // Establecer modo a captura de pantalla
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Iniciar grabación de pantalla de forma asíncrona
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Detiene el proceso de grabación actual de forma segura
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener la grabación de forma asíncrona
            // Esto finaliza correctamente el archivo MP4 y libera recursos
            // Usar async asegura que la UI permanezca receptiva durante la finalización del archivo
            await videoCapture1.StopAsync();
        }

        /// <summary>
        /// Inicializa el componente VideoCapture cuando el formulario se carga
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar el componente de captura de video y conectarlo a un control de vista previa de video
            // VideoView1 debe ser un control en tu formulario que implemente la interfaz IVideoView
            // Esto permite vista previa en vivo de la captura cuando se desee
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

## Cómo Funciona

Esta aplicación de Windows Forms demuestra la funcionalidad de captura de pantalla con y sin audio usando VisioForge Video Capture SDK:

1. **Configuración**: El objeto `VideoCaptureCore` se inicializa en el evento de carga del formulario, conectándolo a un componente de vista de video.

2. **Captura con Audio**:
   - Establece captura de pantalla a modo de pantalla completa
   - Selecciona el primer dispositivo de audio disponible para grabación
   - Deshabilita reproducción de audio pero habilita grabación de audio
   - Establece el archivo de salida a formato MP4 en la carpeta Videos del usuario
   - Usa método asíncrono para iniciar captura

3. **Captura sin Audio**:
   - Similar a lo anterior pero deshabilita tanto reproducción como grabación de audio
   - Usa el mismo formato de salida MP4 y modo de captura

4. **Deteniendo la Captura**:
   - Proporciona un método simple de detención que detiene la grabación de forma asíncrona

La aplicación demuestra cómo configurar diferentes escenarios de captura con código mínimo usando la interfaz fluida del SDK y patrones async.

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.