---
title: Capturar y Grabar Video de Webcam en C# .NET - Tutorial
description: Guía completa para capturar y guardar video de webcam usando .NET y C#. Implemente funcionalidad con formatos MP4/WebM y aceleración GPU.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Encoding
  - Webcam
  - IP Camera
  - Screen Capture
  - MP4
  - WebM
  - H.264
  - H.265
  - VP8
  - VP9
  - AV1
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - DeviceEnumerator
  - VideoCaptureDeviceSourceSettings
  - MP4Output
  - WebMOutput

---

# Guardar Video de Webcam Usando .Net: Una Guía Completa para Grabar y Capturar Webcam

Capturar y guardar video de webcams es un requisito común en muchas aplicaciones modernas, desde herramientas de videoconferencia hasta sistemas de vigilancia. Ya sea que necesite grabar imágenes de webcam, mostrar la transmisión de la webcam o capturar imágenes, implementar funcionalidad de webcam confiable en .NET (DotNet) puede ser un desafío. Este tutorial proporciona una guía simple paso a paso para guardar video de webcam usando C# (C Sharp) con código mínimo.

## Características Clave de Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) es una biblioteca poderosa diseñada específicamente para desarrolladores .NET que necesitan implementar funcionalidad de captura de webcam en sus aplicaciones. Ya sea que desee grabar video de webcam, guardar fotogramas de webcam como imágenes o mostrar la transmisión de la webcam en su aplicación, este SDK lo tiene cubierto. Algunas de sus características destacadas incluyen:

- Integración perfecta con aplicaciones .NET
- Soporte para múltiples dispositivos de captura (webcams USB, cámaras IP, tarjetas de captura)
- Grabación y procesamiento de video y audio de alto rendimiento
- Código simple para obtener y mostrar transmisiones de webcam
- Crear y guardar archivos de video en varios formatos
- Codificación acelerada por GPU para un rendimiento óptimo
- Compatibilidad multiplataforma

## Formatos de Salida: MP4 y WebM en Detalle

### Formato MP4

MP4 es uno de los formatos de contenedor de video más ampliamente soportados, lo que lo convierte en una excelente opción para aplicaciones donde la compatibilidad es una prioridad. Para opciones de configuración detalladas, consulte la [documentación del formato MP4](../../general/output-formats/mp4.md).

Códecs soportados para MP4:

- **H.264 (AVC):** El estándar de la industria para compresión de video, ofreciendo excelente calidad y amplia compatibilidad. Consulte la [documentación del codificador H.264](../../general/video-encoders/h264.md).
- **H.265 (HEVC):** Códec de próxima generación que proporciona hasta un 50% mejor compresión que H.264 manteniendo la misma calidad. Consulte la [documentación del codificador HEVC](../../general/video-encoders/hevc.md).
- **AAC:** Codificación de Audio Avanzada, el estándar de la industria para compresión de audio digital con pérdida.

### Formato WebM

WebM es un formato de archivo multimedia abierto y libre de regalías diseñado para la web. Para opciones de configuración detalladas, consulte la [documentación del formato WebM](../../general/output-formats/webm.md).

Códecs soportados para WebM:

- **VP8:** Códec de video de código abierto desarrollado por Google, utilizado principalmente con el formato WebM.
- **VP9:** Sucesor de VP8, ofreciendo eficiencia de compresión significativamente mejorada.
- **AV1:** El códec de video de código abierto más nuevo con compresión y calidad superiores, particularmente a tasas de bits más bajas.
- **Vorbis:** Formato de compresión de audio libre y de código abierto que ofrece buena calidad a tasas de bits más bajas.

Cada códec puede ajustarse finamente con varios parámetros para lograr el equilibrio óptimo entre calidad y tamaño de archivo para los requisitos específicos de su aplicación.

## Aceleración GPU para Codificación Eficiente

Una de las características destacadas de Video Capture SDK .Net es su robusto soporte para codificación de video acelerada por GPU, que ofrece varias ventajas significativas:

### Beneficios de la Aceleración GPU

- **Uso de CPU dramáticamente reducido:** Libere recursos de CPU para otras tareas de la aplicación
- **Velocidades de codificación más rápidas:** Hasta 10 veces más rápido que la codificación solo por CPU
- **Codificación de alta resolución en tiempo real:** Habilite captura de video 4K con impacto mínimo en el sistema
- **Menor consumo de energía:** Particularmente importante para aplicaciones móviles y portátiles
- **Mayor calidad a la misma tasa de bits:** Algunos codificadores GPU ofrecen mejor calidad por bit que la codificación por CPU

### Tecnologías GPU Soportadas

Video Capture SDK .Net aprovecha múltiples tecnologías de aceleración GPU:

- **NVIDIA NVENC:** Codificación acelerada por hardware en GPUs NVIDIA
- **AMD AMF/VCE:** Aceleración en tarjetas gráficas AMD
- **Intel Quick Sync Video:** Codificación por hardware en gráficos integrados Intel

El SDK detecta automáticamente el hardware disponible y selecciona la ruta de codificación óptima basándose en las capacidades de su sistema, con respaldo a codificación por software cuando sea necesario.

## Ejemplo de Implementación (Código C# para Capturar Video de Webcam)

Veamos un tutorial simple sobre cómo grabar video de webcam usando C#. Implementar captura de webcam con Video Capture SDK .Net es sencillo. Aquí hay un ejemplo completo que muestra cómo obtener la transmisión de su webcam, mostrarla en su aplicación y guardarla en un archivo MP4 con codificación H.264:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.AudioRenderers;

namespace video_capture_webcam_mp4
{
    public partial class Form1 : Form
    {
        // Objeto principal de Video Capture
        private VideoCaptureCoreX videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Crear instancia de VideoCaptureCoreX y establecer VideoView para renderizado de video
            videoCapture1 = new VideoCaptureCoreX(VideoView1);

            // Enumerar fuentes de video y audio
            var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();
            var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();

            // Establecer fuente de video predeterminada
            videoCapture1.Video_Source = new VideoCaptureDeviceSourceSettings(videoSources[0]);

            // Establecer fuente de audio predeterminada
            videoCapture1.Audio_Source = audioSources[0].CreateSourceSettingsVC();

            // Establecer dispositivo de salida de audio predeterminado
            var audioRenderers = await DeviceEnumerator.Shared.AudioOutputsAsync();
            videoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioRenderers[0]);
            videoCapture1.Audio_Play = true;

            // Configurar salida MP4. Se usarán codificadores de video y audio predeterminados.
            // Se usarán codificadores GPU si están disponibles.
            var mp4Output = new MP4Output(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4"));
            videoCapture1.Outputs_Add(mp4Output);

            videoCapture1.Audio_Record = true;

            // Iniciar captura
            await videoCapture1.StartAsync();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Debemos inicializar el motor al inicio
            Text += " [PRIMERA CARGA, CONSTRUYENDO EL REGISTRO...]";
            this.Enabled = false;
            await VisioForgeX.InitSDKAsync();
            this.Enabled = true;
            Text = Text.Replace("[PRIMERA CARGA, CONSTRUYENDO EL REGISTRO...]", "");
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener captura
            await videoCapture1.StopAsync();

            // Liberar recursos
            await videoCapture1.DisposeAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Liberar SDK
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Ejemplos de Código Adicionales

### Salida WebM con VP9

Para salida WebM con codificación VP9, simplemente modifique la configuración del codificador:

```csharp
using VisioForge.Core.Types.X.Output;

// Configurar salida WebM con códec VP9
var webmOutput = new WebMOutput(Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
    "output.webm"));
videoCapture1.Outputs_Add(webmOutput);
```

### Habilitar Capturador de Instantáneas

Aquí hay un ejemplo simple de cómo guardar una sola imagen de la webcam. Habilite el capturador de muestras de video:

```csharp
// Habilitar capturador de instantáneas antes de iniciar la captura
videoCapture1.Snapshot_Grabber_Enabled = true;
await videoCapture1.StartAsync();
```

### Guardar Instantánea

Obtener y guardar una sola imagen de la webcam:

```csharp
using SkiaSharp;

// Guardar fotograma actual como JPEG
await videoCapture1.Snapshot_SaveAsync(
    "snapshot.jpg", 
    SKEncodedImageFormat.Jpeg, 
    85);

// O guardar como PNG
await videoCapture1.Snapshot_SaveAsync(
    "snapshot.png", 
    SKEncodedImageFormat.Png);
```

## Dependencias Nativas

Video Capture SDK .Net depende de bibliotecas nativas para acceder a dispositivos de webcam y realizar procesamiento de video y audio. Estas dependencias nativas se incluyen con el SDK y se implementan automáticamente con su aplicación, asegurando una integración perfecta y compatibilidad en diferentes sistemas.

### Referencia de Paquete

Paquete principal del SDK:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
```

### Paquetes Redist Específicos de Plataforma

Windows x64:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

Para otras plataformas:

```xml
<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

## Compatibilidad Multiplataforma

Video Capture SDK .Net está diseñado con la compatibilidad multiplataforma en mente, lo que lo convierte en una opción ideal para desarrolladores que trabajan en aplicaciones que necesitan ejecutarse en múltiples sistemas operativos.

### Compatibilidad con MAUI

Para desarrolladores que trabajan con .NET MAUI (Interfaz de Usuario de Aplicación Multiplataforma), Video Capture SDK .Net ofrece:

- Compatibilidad total con aplicaciones MAUI
- API consistente en todas las plataformas soportadas
- Optimizaciones específicas de plataforma mientras mantiene una base de código unificada
- Proyectos MAUI de ejemplo que demuestran la implementación en todas las plataformas

Esta capacidad multiplataforma permite a los desarrolladores escribir código una vez e implementarlo en Windows, macOS y plataformas móviles a través de MAUI, reduciendo significativamente el tiempo de desarrollo y la sobrecarga de mantenimiento.

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) proporciona una solución integral para agregar capacidades de captura de video de webcam a sus aplicaciones DotNet. Ya sea que necesite grabar imágenes de webcam, guardar imágenes de webcam o simplemente mostrar la transmisión de la webcam en su aplicación, esta biblioteca hace que el proceso sea simple con solo unas pocas líneas de código C#.

Con soporte para formatos estándar de la industria como MP4 y WebM, códecs modernos incluyendo H.264/H.265 y VP8/VP9/AV1, y potente aceleración GPU, ofrece el rendimiento y la flexibilidad necesarios incluso para las aplicaciones de captura de video más exigentes. La capacidad de crear y guardar archivos de video eficientemente hace que esta biblioteca sea perfecta para cualquier aplicación que necesite grabar contenido de webcam.

La compatibilidad multiplataforma del SDK, extendiéndose a macOS y aplicaciones MAUI, asegura que su solución de captura de webcam funcione consistentemente en diferentes sistemas operativos. Ya sea que esté construyendo una herramienta de videoconferencia, una aplicación de vigilancia o cualquier otro software que requiera funcionalidad de webcam, Video Capture SDK .Net ofrece las herramientas que necesita para implementar estas características rápidamente.

Comenzar es tan simple como seguir el tutorial paso a paso y los ejemplos de código proporcionados anteriormente. Para casos de uso más avanzados y documentación detallada sobre cómo grabar video de webcam usando .NET, visite nuestro sitio web o consulte la documentación del SDK.

## Preguntas Frecuentes

### ¿Qué formato debo usar para guardar video de webcam — MP4 o WebM?

MP4 con codificación H.264 es la mejor opción para la mayoría de las aplicaciones porque ofrece amplia compatibilidad con dispositivos y reproductores, compresión eficiente y codificación acelerada por hardware en la mayoría de las GPUs. Elija WebM con VP9 o AV1 si necesita un formato libre de regalías para distribución web o reproducción en navegadores. Ambos formatos admiten grabación de audio de alta calidad junto con el video.

### ¿Cómo configuro la resolución y la velocidad de fotogramas para la grabación de webcam?

Use la clase `VideoCaptureDeviceSourceSettings` para seleccionar una resolución y velocidad de fotogramas específicas de los formatos compatibles de su cámara. Después de crear el objeto de configuración de fuente, llame a `GetFormats()` para enumerar los modos disponibles, luego asigne su formato preferido antes de iniciar la captura. El SDK negocia automáticamente la coincidencia más cercana si el formato exacto no está disponible.

### ¿Puedo usar aceleración GPU para grabar video de webcam en C#?

Sí. Video Capture SDK .Net detecta automáticamente el hardware GPU disponible — NVIDIA NVENC, AMD AMF/VCE e Intel Quick Sync Video — y selecciona la codificación acelerada por hardware cuando crea un `MP4Output` o `WebMOutput`. No se requiere configuración adicional. Si no se encuentra una GPU compatible, el SDK recurre a la codificación por software optimizada de forma transparente.

### ¿Cómo grabo video de webcam con audio?

Configure la propiedad `Audio_Source` con un micrófono u otro dispositivo de entrada de audio, luego habilite la grabación con `Audio_Record = true`. Para monitorear el audio durante la captura, establezca `Audio_Play = true` y asigne un `Audio_OutputDevice`. Al guardar en MP4, el audio se codifica como AAC por defecto; WebM usa Vorbis.

### ¿La grabación de webcam funciona en macOS y Linux?

Sí. El SDK es completamente multiplataforma. Agregue los paquetes NuGet redist específicos de plataforma apropiados (macOS, Linux x64) junto con el paquete principal `VisioForge.DotNet.VideoCapture`. El mismo código C# se ejecuta en Windows, macOS y Linux, y el SDK también admite .NET MAUI para implementación móvil y de escritorio multiplataforma.

## Soporte VB.NET

¿Busca grabación de webcam en VB.NET? Consulte nuestra guía dedicada: [Grabar Video de Webcam en VB.NET](record-webcam-vb-net.md).

## Ver También

- [Grabar Video de Webcam en VB.NET](record-webcam-vb-net.md) — misma funcionalidad con ejemplos de código VB.NET
- [Captura de Pantalla a MP4](../video-tutorials/screen-capture-mp4.md) — grabar la pantalla del escritorio en lugar de la webcam
- [Tutorial de Webcam a MP4](../video-tutorials/video-capture-webcam-mp4.md) — guía paso a paso para grabación en MP4
- [Captura de Cámara IP a MP4](../video-tutorials/ip-camera-capture-mp4.md) — grabar desde cámaras de red en lugar de webcams locales
- [Escáner de Códigos de Barras y QR](../../mediablocks/Guides/barcode-qr-reader-guide.md) — combinar captura de webcam con detección de códigos de barras
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas
