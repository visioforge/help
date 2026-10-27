---
title: Vista previa en vivo de cámara IP en C# .NET — tutorial RTSP
description: Implemente vista previa de cámara IP en tiempo real en aplicaciones .NET con tutorial paso a paso y ejemplos de código C# completos para WinForms, WPF, Console.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - IPCameraSourceSettings
  - IPSourceEngine
  - VideoCaptureMode
  - IVideoView

---

# Guía de implementación de vista previa de cámara IP

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial en video

Este tutorial demuestra cómo configurar la funcionalidad de vista previa de cámara IP en sus aplicaciones .NET:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/9n44ChQJT7s?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-preview)

## Redistribuibles requeridos

Antes de comenzar, asegúrese de tener instalados los siguientes paquetes:

- Redistribuibles de captura de video [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuibles LAV [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

## Ejemplo de implementación

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
            // Hay varios motores disponibles. Usaremos LAV como el más compatible. Para reproducción RTSP de baja latencia, usar el motor RTSP Low Latency.
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

## Detalles clave de implementación

### Configurar la fuente de cámara IP

El código demuestra la configuración de la fuente de cámara IP con la URL apropiada y tipo de motor:

```csharp
videoCapture1.IP_Camera_Source = new IPCameraSourceSettings()
{
    URL = new Uri("http://192.168.233.129:8000/camera/mjpeg"),
    Type = IPSourceEngine.Auto_LAV
};
```

### Manejar la configuración de audio

Para aplicaciones de vista previa simples, puede que quiera deshabilitar la reproducción y grabación de audio:

```csharp
videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
```

### Establecer el modo de captura

El modo correcto para vista previa de cámara IP es:

```csharp
videoCapture1.Mode = VideoCaptureMode.IPPreview;
```

## Opciones avanzadas

Para aplicaciones de producción, considere implementar:

- Manejo de errores y lógica de reintento de conexión
- Retroalimentación de UI durante intentos de conexión
- Manejo de autenticación de cámara
- Control de tasa de frames y resolución

## Guías Relacionadas

- [Configuración de fuente de cámara RTSP](../video-sources/ip-cameras/rtsp.md) — transporte UDP/TCP, ajuste de baja latencia, selección de motor
- [Integración de cámara IP ONVIF](../video-sources/ip-cameras/onvif.md) — auto-descubrimiento, selección de perfiles, control PTZ
- [Grabar stream RTSP a MP4](ip-camera-capture-mp4.md) — capturar el stream previsualizado a un archivo

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para explorar más ejemplos de código. ¿Necesita la URL RTSP para su cámara? Consulte nuestro [directorio de marcas de cámaras IP](../../camera-brands/index.md).
