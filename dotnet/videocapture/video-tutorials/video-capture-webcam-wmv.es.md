---
title: Captura de Webcam a WMV en .NET
description: Implementa captura de video de webcam a formato WMV en .NET con instrucciones paso a paso, ejemplos de código C# y mejores prácticas de integración.
---

# Captura de Video de Webcam a Formato WMV en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial de Implementación en Video

Este tutorial demuestra cómo crear una aplicación de Windows Forms que captura video de una webcam y lo guarda en formato WMV usando el Video Capture SDK .NET. El ejemplo a continuación proporciona un recorrido completo con código fuente completamente comentado.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Bqss-zdalXg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Acceso al Código Fuente

Accede al proyecto completo con todos los archivos fuente y ejemplos adicionales en nuestro repositorio de GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-wmv)

## Dependencias Requeridas

Antes de implementar el código, asegúrate de haber instalado los paquetes redistribuibles necesarios vía NuGet:

- **Redistribuibles de Captura de Video:**
  - [Paquete x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Paquete x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Pasos de Implementación

Las siguientes secciones describen los pasos clave para implementar funcionalidad de captura de webcam en tu aplicación .NET.

### Configurar Estructura del Proyecto

Primero, crea una aplicación de Windows Forms y agrega las referencias del SDK a través de NuGet. Luego implementa el formulario con los controles necesarios incluyendo un área de vista previa de video y botones de inicio/detención.

### Código de Implementación C#

```csharp
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_wmv
{
    public partial class Form1 : Form
    {
        // Instancia principal del componente VideoCapture
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configurar el dispositivo de captura de video predeterminado (webcam)
            // Usando el primer dispositivo disponible en el sistema
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configurar el dispositivo de captura de audio predeterminado (micrófono)
            // Usando el primer dispositivo de audio disponible en el sistema
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Establecer la ruta del archivo de salida a la carpeta Videos del usuario
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");

            // Configurar formato de salida como WMV con configuración predeterminada
            // WMV es una buena elección para compatibilidad con Windows
            videoCapture1.Output_Format = new WMVOutput();
            
            // Establecer el modo de captura a VideoCapture (captura tanto video como audio)
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Iniciar el proceso de captura de forma asíncrona
            // Esto permite que la UI permanezca receptiva durante la captura
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener el proceso de captura de forma asíncrona
            // Esto asegura limpieza adecuada de recursos
            await videoCapture1.StopAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar el VideoCaptureCore cuando el formulario se carga
            // Conectarlo al control VideoView en el formulario para vista previa
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

### Detalles Clave de Implementación

1. **Inicialización del Componente**: El `VideoCaptureCore` se inicializa cuando el formulario se carga, conectándose al control de vista previa de video.

2. **Selección de Dispositivo**: El código selecciona automáticamente la primera webcam y dispositivo de micrófono disponibles en el sistema.

3. **Configuración de Salida**: El formato WMV está seleccionado por su compatibilidad con sistemas Windows y amplio soporte de plataformas.

4. **Gestión de Recursos**: El método `StopAsync()` asegura limpieza adecuada de recursos del sistema cuando la grabación termina.

## Opciones de Personalización Avanzada

El SDK proporciona opciones adicionales no mostradas en este ejemplo básico:

- Configuraciones de resolución y tasa de frames de cámara
- Parámetros de calidad de video y compresión
- Marcas de agua y superposiciones personalizadas
- Capacidades de captura multi-dispositivo

## Consejos de Solución de Problemas

- Asegura que tu webcam esté correctamente conectada y reconocida por Windows
- Verifica que los redistribuibles correctos estén instalados para tu plataforma objetivo (x86/x64)
- Verifica los permisos de Windows para acceso a cámara en sistemas operativos más nuevos

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y ejemplos de implementación avanzada.