---
title: Streaming de Cámara IP a Archivos MP4 en .NET C#
description: Implementa streaming de video de cámara IP y grabación a archivos MP4 en .NET usando C# con conexión RTSP, opciones de codificación y ejemplos de código.
---

# Capturando Streams de Cámara IP a Archivos MP4 en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Grabación de Cámara IP

Las cámaras IP proporcionan potentes capacidades de vigilancia y monitoreo a través de conexiones de red. Esta guía demuestra cómo aprovechar estos dispositivos en aplicaciones .NET para capturar y guardar streams de video a archivos MP4. Usando prácticas modernas de codificación C#, aprenderás cómo establecer conexiones a cámaras IP, configurar streams de video y guardar grabaciones de alta calidad para varias aplicaciones incluyendo sistemas de seguridad, soluciones de monitoreo y herramientas de archivo de video.

## Tutorial de Implementación en Video

El siguiente video recorre el proceso de implementación completo:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/qX3AiGyWbO8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-capture-mp4)

## Guía de Implementación Paso a Paso

Esta guía demuestra cómo establecer conexiones a cámaras IP, transmitir su contenido de video y codificarlo directamente a archivos MP4 usando el componente VideoCaptureCoreX. La implementación soporta múltiples protocolos de cámara incluyendo RTSP, HTTP y ONVIF.

### Implementación de Código

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

using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core;

namespace ip_camera_capture_mp4
{
    public partial class Form1 : Form
    {
        private VideoCaptureCoreX videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCoreX(VideoView1);

            // Fuente de cámara RTSP
            var rtsp = await RTSPSourceSettings.CreateAsync(new Uri(edURL.Text), edLogin.Text, edPassword.Text, cbAudioStream.Checked);
            videoCapture1.Video_Source = rtsp;

            // Dispositivo de salida de audio
            var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound))[0];
            videoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);

            // Configurar salida MP4
            var mp4Output = new MP4Output(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4"));
            videoCapture1.Outputs_Add(mp4Output);

            // Iniciar
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();

            await videoCapture1.DisposeAsync();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await VisioForgeX.InitSDKAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Detalles de Implementación y Mejores Prácticas

### Inicialización del SDK

Antes de trabajar con cámaras IP, la inicialización adecuada del SDK es crucial para asegurar que todos los componentes estén cargados y listos:

```csharp
private async void Form1_Load(object sender, EventArgs e)
{
    await VisioForgeX.InitSDKAsync();
}
```

Siempre inicializa el SDK al inicio de la aplicación para asegurar que todos los componentes requeridos estén correctamente cargados antes de intentar conexiones de cámara.

### Configuración de Conexión de Cámara

El ejemplo usa RTSP (Real-Time Streaming Protocol), uno de los protocolos más comunes para streaming de cámara IP:

```csharp
// Fuente de cámara RTSP
var rtsp = await RTSPSourceSettings.CreateAsync(new Uri(edURL.Text), edLogin.Text, edPassword.Text, cbAudioStream.Checked);
videoCapture1.Video_Source = rtsp;
```

Al conectar a cámaras IP, considera estos parámetros importantes:

- **URL de Cámara** - La URL RTSP completa de tu cámara (ej., `rtsp://192.168.1.100:554/stream1`)
- **Autenticación** - Muchas cámaras requieren credenciales de nombre de usuario y contraseña
- **Stream de Audio** - Alterna si incluir audio de la cámara
- **Timeout de Conexión** - Para aplicaciones de producción, implementa manejo de timeout apropiado

### Configuración de Salida MP4

MP4 es un formato contenedor ideal para grabaciones de video debido a su excelente compatibilidad y compresión:

```csharp
// Configurar salida MP4
var mp4Output = new MP4Output(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4"));
videoCapture1.Outputs_Add(mp4Output);
```

Al configurar salida MP4, considera estas opciones:

- **Nombre de Archivo** - Implementa nombres de archivo dinámicos basados en fecha/hora para grabaciones organizadas
- **Ubicación de Almacenamiento** - Elige rutas de almacenamiento apropiadas con suficiente espacio en disco
- **Calidad de Video** - Configura bitrate, framerate y ajustes de resolución basados en tus requisitos
- **Metadatos** - Agrega metadatos relevantes a las grabaciones para clasificación y búsqueda más fáciles

### Gestión de Recursos

La gestión adecuada de recursos es crítica al trabajar con streams de video para prevenir fugas de memoria y asegurar rendimiento estable:

```csharp
private async void btStop_Click(object sender, EventArgs e)
{
    await videoCapture1.StopAsync();
    await videoCapture1.DisposeAsync();
}

private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    VisioForgeX.DestroySDK();
}
```

Siempre implementa limpieza adecuada de recursos, especialmente:

- Detén streams activos antes de cerrar la aplicación
- Libera recursos de captura cuando ya no se necesiten
- Libera recursos del SDK cuando tu aplicación termine

### Consideraciones Avanzadas de Implementación

Para aplicaciones de grado de producción, considera estas características adicionales:

1. **Manejo de Errores** - Implementa manejo de errores comprehensivo para desconexiones de red, fallos de autenticación y problemas de almacenamiento
2. **Monitoreo** - Agrega monitoreo de estado para rastrear salud del stream y estado de grabación
3. **Lógica de Reconexión** - Implementa reconexión automática para interrupciones de red
4. **Soporte Multi-Cámara** - Extiende la implementación para manejar múltiples streams de cámara simultáneamente
5. **Programación de Grabación** - Agrega funciones de grabación basadas en tiempo para aplicaciones de vigilancia

## Dependencias Requeridas

Para que esta implementación funcione correctamente, los siguientes paquetes deben incluirse en tu proyecto:

- Redistribuibles de captura de video [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuibles LAV [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)
- Redistribuibles MP4 [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Solución de Problemas Comunes

Al implementar captura de cámara IP, prepárate para abordar estos desafíos comunes:

1. **Fallos de Conexión** - Verifica conectividad de red, credenciales de cámara y configuración de firewall
2. **Rendimiento de Stream** - Equilibra ajustes de calidad con ancho de banda disponible y potencia de procesamiento
3. **Errores de Archivo de Salida** - Asegura espacio de disco adecuado y permisos de escritura apropiados
4. **Fugas de Recursos** - Monitorea uso de memoria durante sesiones de grabación largas
5. **Compatibilidad de Cámara** - Diferentes modelos de cámara pueden requerir ajustes de configuración específicos

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.