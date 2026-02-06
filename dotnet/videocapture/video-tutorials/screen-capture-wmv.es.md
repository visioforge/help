---
title: Grabación de Pantalla C# a WMV - Implementación .NET
description: Implementa grabación de pantalla profesional en aplicaciones .NET con C# usando guía paso a paso, ejemplos de código funcionales y opciones de configuración.
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

## Dependencias Requeridas

Antes de comenzar, asegúrate de haber instalado los paquetes redistribuibles necesarios:

- Redistribuibles de captura de video:
  - [paquete x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [paquete x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Código de Implementación Esencial en C#

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

## Opciones de Configuración Avanzada

### Capturando Regiones Específicas de Pantalla

Si necesitas grabar solo una porción de la pantalla en lugar de toda la pantalla:

```csharp
// Definir una región rectangular específica para capturar (x, y, ancho, alto)
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
    FullScreen = false,
    Rectangle = new Rectangle(0, 0, 800, 600) 
};
```

## Escenarios de Implementación Comunes

### Creando una Aplicación de Grabación Ligera

Para escenarios donde los recursos del sistema son limitados:

1. Reducir la tasa de frames de captura
2. Grabar a un codec más eficiente
3. Capturar regiones de pantalla más pequeñas
4. Usar aceleración por hardware cuando esté disponible

### Implementando Grabación en Segundo Plano

Para aplicaciones que necesitan grabar en segundo plano:

1. Inicializar el componente de captura en un hilo separado
2. Implementar UI mínima para control
3. Considerar agregar funcionalidad de bandeja del sistema
4. Implementar gestión de recursos adecuada

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y ejemplos de implementación.