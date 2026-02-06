---
title: Vista Previa de Cámara IP en Aplicaciones .NET
description: Implementa vista previa de cámara IP en tiempo real en aplicaciones .NET con tutorial paso a paso y ejemplos de código C# completos para WinForms, WPF, Console.
---

# Guía de Implementación de Vista Previa de Cámara IP

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial en Video

Este tutorial demuestra cómo configurar la funcionalidad de vista previa de cámara IP en tus aplicaciones .NET:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/9n44ChQJT7s?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-preview)

## Redistribuibles Requeridos

Antes de comenzar, asegúrate de tener instalados los siguientes paquetes:

- Redistribuibles de captura de video [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuibles LAV [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

## Ejemplo de Implementación

A continuación se muestra un ejemplo completo de WinForms mostrando cómo integrar la funcionalidad de vista previa de cámara IP:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace ip_camera_preview
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Hay varios motores disponibles. Usaremos LAV como el más compatible. Para reproducción RTSP de baja latencia, usa el motor RTSP Low Latency.
            videoCapture1.IP_Camera_Source = new IPCameraSourceSettings()
            {
                URL = new Uri("http://192.168.233.129:8000/camera/mjpeg"),
                Type = IPSourceEngine.Auto_LAV
            };

            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
            videoCapture1.Mode = VideoCaptureMode.IPPreview;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();
        }
    }
}
```

## Detalles Clave de Implementación

### Configurando la Fuente de Cámara IP

El código demuestra la configuración de la fuente de cámara IP con la URL apropiada y tipo de motor:

```csharp
videoCapture1.IP_Camera_Source = new IPCameraSourceSettings()
{
    URL = new Uri("http://192.168.233.129:8000/camera/mjpeg"),
    Type = IPSourceEngine.Auto_LAV
};
```

### Manejando Configuración de Audio

Para aplicaciones de vista previa simples, puede que quieras deshabilitar la reproducción y grabación de audio:

```csharp
videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
```

### Estableciendo el Modo de Captura

El modo correcto para vista previa de cámara IP es:

```csharp
videoCapture1.Mode = VideoCaptureMode.IPPreview;
```

## Opciones Avanzadas

Para aplicaciones de producción, considera implementar:

- Manejo de errores y lógica de reintento de conexión
- Retroalimentación de UI durante intentos de conexión
- Manejo de autenticación de cámara
- Control de tasa de frames y resolución

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para explorar más ejemplos de código.