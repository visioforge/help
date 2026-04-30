---
title: Captura de Fotos con Webcam en C# .NET — API Still Frame
description: Tome fotos desde una webcam en C# / .NET. Captura still frame por botón hardware o trigger software. Guardar en JPEG, PNG, BMP. Ejemplo de código incluido.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Webcam
  - C#
primary_api_classes:
  - VideoCaptureCore
  - VideoFrameBitmapEventArgs
  - VideoFrameBufferEventArgs

---

# Captura de Fotos con Webcam en Aplicaciones .NET

!!! tip "Agentes de IA: usa el servidor MCP de VisioForge"

    ¿Lo construyes con **Claude Code**, **Cursor** u otro agente de IA?
    Conecta al servidor MCP público de VisioForge
    ([documentación](../../general/mcp-server-usage.md))
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que
    buscar en `llms.txt`. Sin autenticación requerida.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introducción a la Integración de Webcam

Las aplicaciones modernas requieren cada vez más integración de webcam para varios propósitos, desde fotos de perfil de usuario hasta escaneo de documentos. Implementar una funcionalidad efectiva de captura de fotos con webcam requiere entender los mecanismos subyacentes de cómo funcionan las webcams con el framework .NET.

Las webcams pueden capturar imágenes a través de dos métodos principales: capturas activadas por software (donde la aplicación inicia el proceso) y capturas activadas por hardware, donde un botón físico en el dispositivo de webcam activa la captura de imagen. Este último método se conoce como "captura de fotograma fijo" y proporciona una experiencia de usuario más intuitiva en muchas aplicaciones.

## Entendiendo la Captura de Fotogramas Fijos

La captura de fotogramas fijos es una función especializada disponible en muchos modelos de webcam que permite a los usuarios capturar imágenes de alta calidad presionando un botón dedicado en el dispositivo. Este enfoque ofrece varias ventajas:

- Experiencia de usuario más intuitiva
- Complejidad de aplicación reducida
- Menor probabilidad de movimiento de cámara
- A menudo mejor calidad de imagen que la extracción de fotogramas de video

No todas las webcams soportan captura de fotogramas fijos, por lo que es importante verificar las especificaciones de tu dispositivo o probar esta funcionalidad antes de depender de ella en tu aplicación.

## Implementando Captura de Fotos con Webcam en .NET

El Video Capture SDK para .NET proporciona un framework robusto para implementar captura de fotos con webcam en tus aplicaciones. A continuación, cubriremos los pasos esenciales para integrar esta funcionalidad.

### Configurando Tu Proyecto

Antes de entrar en los detalles de implementación, asegúrate de que tu entorno de desarrollo esté correctamente configurado:

1. Crea un nuevo proyecto de aplicación .NET
2. Añade la referencia del Video Capture SDK a tu proyecto
3. Importa los namespaces necesarios:

```csharp
using System.Drawing;
using System.Drawing.Imaging;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.VideoCapture;
```

### Habilitando la Captura de Fotogramas Fijos

El primer paso para implementar la captura de fotogramas fijos es configurar correctamente tu aplicación para detectar y responder a las pulsaciones del botón de hardware de la webcam. Así es como:

```csharp
// Inicializar el componente de captura de video. Pase un IVideoView para vista previa,
// o use la sobrecarga CreateAsync() sin parámetros para operación headless.
var videoCapture = await VideoCaptureCore.CreateAsync(VideoView1);

// Habilitar la captura de fotogramas fijos antes de iniciar el stream de video.
videoCapture.Video_Still_Frames_Grabber_Enabled = true;

// Seleccionar dispositivo de captura por nombre. Enumera dispositivos con videoCapture.Video_CaptureDevices().
videoCapture.Video_CaptureDevice = new VideoCaptureSource("USB Camera");
videoCapture.Video_CaptureDevice.Format_UseBest = true;     // o establece Format + FrameRate explícitamente
videoCapture.Audio_RecordAudio = false;
videoCapture.Audio_PlayAudio = false;

// Modo solo vista previa (sin captura a archivo).
videoCapture.Mode = VideoCaptureMode.VideoPreview;

// Iniciar el pipeline (asíncrono — OnError se dispara en errores del pipeline).
await videoCapture.StartAsync();
```

Configurar la propiedad `Video_Still_Frames_Grabber_Enabled` a `true` es crucial. Esta configuración le dice al SDK que monitoree las pulsaciones del botón de hardware y active los eventos apropiados cuando se capture un fotograma fijo.

### Manejando Fotogramas Capturados

Una vez que la captura de fotogramas fijos está habilitada, necesitas manejar los eventos que se activan cuando se captura un fotograma. El SDK proporciona dos eventos principales para este propósito:

```csharp
// Para manejar fotogramas como objetos Bitmap
videoCapture.OnStillVideoFrameBitmap += VideoCapture_OnStillVideoFrameBitmap;

// Para manejar fotogramas como datos de buffer sin procesar
videoCapture.OnStillVideoFrameBuffer += VideoCapture_OnStillVideoFrameBuffer;
```

Aquí un ejemplo de cómo implementar el manejador de eventos para fotogramas bitmap (el evento es `EventHandler<VideoFrameBitmapEventArgs>`; el bitmap vive en `e.Frame`):

```csharp
private void VideoCapture_OnStillVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Clona el bitmap porque e.Frame pertenece al SDK y se reutiliza tras salir del callback.
    Bitmap capturedImage = (Bitmap)e.Frame.Clone();

    // Marshal al hilo UI cuando se ejecuta en WinForms / WPF.
    if (pictureBox1.InvokeRequired)
    {
        pictureBox1.BeginInvoke((Action)(() =>
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = capturedImage;
        }));
    }
    else
    {
        pictureBox1.Image?.Dispose();
        pictureBox1.Image = capturedImage;
    }
}
```

Para acceso al buffer crudo (sin asignación de Bitmap, útil en pipelines de imagen personalizados), suscríbete a `OnStillVideoFrameBuffer`. Su firma es `EventHandler<VideoFrameBufferEventArgs>`:

```csharp
private void VideoCapture_OnStillVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Metadatos del frame crudo: ancho / alto / stride / formato de píxeles.
    var width  = e.Frame.Info.Width;
    var height = e.Frame.Info.Height;

    // e.FrameArray es una copia managed byte[] (puede ser null si el SDK mantuvo los datos en memoria nativa).
    if (e.FrameArray != null)
    {
        File.WriteAllBytes($"frame-{e.Frame.Timestamp.Ticks}.raw", e.FrameArray);
    }

    // Marca UpdateData = true si mutaste el buffer y quieres que el cambio se propague downstream.
    // e.UpdateData = true;
}
```

### Guardando Imágenes Capturadas

Después de capturar y potencialmente procesar la imagen, a menudo querrás guardarla en disco. El SDK proporciona un método conveniente para este propósito:

```csharp
// Guardar el fotograma actual en un archivo (API async — funciona después de StartAsync).
await videoCapture.Frame_SaveAsync("imagenCapturada.jpg", ImageFormat.Jpeg);
```

Puedes especificar diferentes formatos de imagen basados en los requisitos de tu aplicación, como PNG para calidad sin pérdida o JPEG para tamaños de archivo más pequeños.

### Obteniendo el Fotograma Actual

En algunos escenarios, podrías querer capturar una imagen programáticamente sin depender del botón de hardware. Puedes hacer esto usando el método `Frame_GetCurrent`:

```csharp
// Obtener el fotograma actual como un Bitmap
Bitmap currentFrame = videoCapture.Frame_GetCurrent();

// Procesar o guardar el fotograma
if (currentFrame != null)
{
    // Usar la imagen
    pictureBox1.Image = currentFrame;
    
    // Guardar si es necesario
    currentFrame.Save("capturaManual.png", ImageFormat.Png);
}
```

## Consideraciones de Rendimiento

Las aplicaciones de webcam pueden ser intensivas en recursos, especialmente al procesar imágenes de alta resolución. Considera estas técnicas de optimización:

1. Usa procesamiento en segundo plano para operaciones de guardado de imágenes
2. Implementa limitación de tasa de fotogramas si es necesario el monitoreo continuo
3. Reduce la resolución para vista previa mientras mantienes alta resolución para capturas
4. Libera los recursos correctamente cuando la aplicación se cierra:

   ```csharp
   protected override void OnFormClosing(FormClosingEventArgs e)
   {
       // Detener captura y liberar recursos
       videoCapture.Stop();
       videoCapture.Dispose();
       base.OnFormClosing(e);
   }
   ```

## Solución de Problemas Comunes

- **Cámara No Detectada**: Asegúrate de que la webcam esté correctamente conectada y los controladores estén instalados
- **Captura de Fotograma Fijo No Funciona**: Verifica que tu modelo de webcam soporte captura con botón de hardware
- **Mala Calidad de Imagen**: Verifica los ajustes de resolución y asegura condiciones de iluminación adecuadas
- **Aplicación Se Bloquea**: Implementa manejo de errores apropiado y gestión de recursos

## Conclusión

Implementar captura de fotos con webcam en aplicaciones .NET proporciona funcionalidad valiosa para muchos escenarios. Siguiendo las directrices en este artículo, puedes crear aplicaciones robustas y fáciles de usar que aprovechan efectivamente las capacidades de la webcam.

Recuerda probar tu implementación en diferentes modelos de webcam y configuraciones para asegurar un rendimiento y fiabilidad consistentes.

---
Para más ejemplos de código e implementaciones, visita nuestro repositorio de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples).