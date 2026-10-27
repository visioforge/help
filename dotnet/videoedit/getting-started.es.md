---
title: Video Edit SDK .NET - Cree aplicaciones de edición de video
description: Edición de video en SDK .NET con líneas de tiempo personalizables, múltiples formatos, transiciones, efectos y vistas previas en tiempo real.
sidebar_position: 0
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Editing
  - MP4
  - C#
primary_api_classes:
  - VideoEditCoreX
  - VideoEditCore
  - VideoSource
  - VideoFileSource
  - IVideoView

---

# Construir aplicaciones profesionales de edición de video con SDK .NET

## Introducción a la edición de video con .NET

El Video Edit SDK ofrece funcionalidad potente para desarrolladores .NET que desean crear aplicaciones profesionales de edición de video. Este SDK soporta una amplia gama de plataformas y frameworks de UI, permitiéndole construir editores de video ricos en características que manejan múltiples formatos, aplican efectos, gestionan transiciones y entregan salida de alta calidad.

## Configurar su entorno de desarrollo

### Crear su proyecto inicial

El SDK está diseñado para funcionar sin problemas con varios entornos de desarrollo. Puede utilizar tanto Visual Studio como JetBrains Rider para crear su proyecto, seleccionando su plataforma y framework de UI preferidos según sus requisitos.

Para un proceso de configuración fluido, por favor consulte nuestra [guía de instalación](../install/index.md) detallada que proporciona instrucciones para agregar los paquetes NuGet necesarios y configurar las dependencias nativas correctamente.

## Implementar la funcionalidad principal de edición de video

### Inicializar el motor de edición de video

El SDK proporciona un objeto de edición principal robusto que sirve como la base de su aplicación de edición de video. `VideoEditCoreX` es un motor X y requiere inicialización del SDK antes de crear cualquier núcleo (consulte [Inicialización](../init.md)); el `VideoEditCore` DirectShow no la requiere. Siga estos pasos para crear e inicializar este componente esencial:

=== "VideoEditCore"

    
    ```cs
    private VideoEditCore core;
    
    core = new VideoEditCore(VideoView1 as IVideoView);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    private VideoEditCoreX core;
    
    private async void Form_Load(object sender, EventArgs e)
    {
        // Motor X: inicializa el SDK una vez antes de crear cualquier núcleo.
        // InitSDKAsync puede fallar (p. ej., faltan librerías nativas, o la inicialización falló en otro hilo); protege para que la app no se bloquee.
        try
        {
            await VisioForge.Core.VisioForgeX.InitSDKAsync();
        }
        catch (Exception ex)
        {
            // Registra el fallo de inicialización en lugar de fallar por un await faulted en async void.
            System.Diagnostics.Debug.WriteLine($"La inicialización del SDK falló: {ex.Message}");
            return;
        }
        
        core = new VideoEditCoreX(VideoView1 as IVideoView);
    }
    ```
    


Necesitará especificar un objeto Video View como parámetro para habilitar la funcionalidad de vista previa de video durante las operaciones de edición. Para el motor `VideoEditCoreX`, inicialice el SDK y cree el núcleo dentro de un controlador `async void Form_Load` (o su ruta de inicio equivalente) antes de configurar el editor — el `core` del motor está listo solo cuando `await InitSDKAsync()` termina.

### Implementar manejo de eventos robusto

#### Gestión de eventos de error

Para gestión adecuada de errores en su aplicación, implemente el manejador de evento `OnError`:

```cs
core.OnError += Core_OnError;

private void Core_OnError(object sender, ErrorsEventArgs e)
{
    Debug.WriteLine("Error: " + e.Message);
}
```

#### Sistema de seguimiento de progreso

Para mantener a sus usuarios informados sobre procesos en curso, implemente el manejador de evento de progreso:

```cs
core.OnProgress += Core_OnProgress;

private void Core_OnProgress(object sender, ProgressEventArgs e)
{
    Debug.WriteLine("Progreso: " + e.Progress);
}
```

#### Notificación de finalización de operación

Para detectar cuando las operaciones de edición han completado, implemente el manejador de evento de parada:

```cs
core.OnStop += Core_OnStop;

private void Core_OnStop(object sender, StopEventArgs e)
{
    Debug.WriteLine("Edición completada");
}
```

### Configurar parámetros de línea de tiempo

Antes de agregar fuentes de medios a su proyecto, debe establecer los parámetros básicos de línea de tiempo como velocidad de cuadros y resolución, que varían dependiendo de qué motor esté usando:

=== "VideoEditCore"

    
    ```cs
    core.Video_FrameRate = new VideoFrameRate(30);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Output_VideoSize = new VisioForge.Core.Types.Size(1920, 1080);
    core.Output_VideoFrameRate = new VideoFrameRate(30);
    ```
    


## Trabajar con fuentes de medios

### Gestionar video y audio en su línea de tiempo

El SDK le permite agregar varias fuentes de medios a su línea de tiempo usando métodos de API directos. Para cada fuente, puede controlar precisamente parámetros como tiempo de inicio, tiempo de fin y posición en la línea de tiempo. Tiene la flexibilidad de agregar fuentes de video y audio tanto independientemente como juntas.

### Integrar archivos de video

El SDK soporta una extensa gama de formatos de video incluyendo MP4, AVI, MOV, WMV y muchos otros. Así es como agregar contenido de video a su proyecto:

Primero, cree un objeto de fuente de video y establezca la ruta del archivo fuente. Puede especificar tiempos de inicio y fin en el constructor o usar parámetros null para incluir el archivo completo.

Para archivos con múltiples streams de video, puede seleccionar qué stream usar especificando el número de stream.

=== "VideoEditCore"

    
    ```cs
    var videoFile = new VisioForge.Core.Types.VideoEdit.VideoSource(
        filename,
        null,
        null, 
        VideoEditStretchMode.Letterbox, 
        0, 
        1.0);
    ```
    
    API:
    
    ```cs
    public VideoSource(
        string filename,
        TimeSpan? startTime = null,
        TimeSpan? stopTime = null,
        VideoEditStretchMode stretchMode = VideoEditStretchMode.Letterbox,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    var videoFile = new VisioForge.Core.Types.X.VideoEdit.VideoFileSource(
        filename,
        null,
        null, 
        0, 
        1.0);
    ```
    
    API:
    
    ```cs
    public VideoFileSource(
        string filename,
        TimeSpan? startTime = null,
        TimeSpan? stopTime = null,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    


Puede controlar la velocidad de reproducción ajustando el parámetro rate. Por ejemplo, establecer rate a 2.0 reproducirá el archivo al doble de la velocidad normal.

Un constructor alternativo le permite agregar múltiples segmentos de archivo:

=== "VideoEditCore"

    
    ```cs
    public VideoSource(
        string filename,
        FileSegment[] segments,
        VideoEditStretchMode stretchMode = VideoEditStretchMode.Letterbox,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    public VideoFileSource(
        string filename,
        FileSegment[] segments,
        int streamNumber = 0,
        double rate = 1.0)
    ```
    


Para agregar la fuente a su línea de tiempo, use los siguientes métodos:

=== "VideoEditCore"

    
    ```cs
    await core.Input_AddVideoFileAsync(
        videoFile, 
        null, 
        0);
    ```
    
    El tercer parámetro especifica el número de stream de video de destino. Use 0 para agregar la fuente al primer stream de video.
    
    API:
    
    ```cs
    public Task<bool> Input_AddVideoFileAsync(
        VideoSource fileSource,
        TimeSpan? timelineInsertTime = null,
        int targetVideoStream = 0,
        int customWidth = 0,
        int customHeight = 0)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Input_AddVideoFile(
        videoFile,
        null);
    ```
    
    API:
    
    ```cs
    public bool Input_AddVideoFile(
        VideoFileSource source, 
        TimeSpan? insertTime = null)
    ```
    


El segundo parámetro determina la posición en la línea de tiempo. Usar null automáticamente agrega la fuente al final de la línea de tiempo existente.

### Incorporar archivos de audio

El SDK soporta numerosos formatos de audio incluyendo AAC, MP3, WMA, OPUS, Vorbis y más. El proceso para agregar audio es similar a agregar video:

=== "VideoEditCore"

    
    ```cs
    var audioFile = new VisioForge.Core.Types.VideoEdit.AudioSource(
        filename,
        startTime: null,
        stopTime: null,
        fileToSync: string.Empty,
        streamNumber: 0,
        rate: 1.0);                      
    ```
    
    El parámetro `fileToSync` habilita la sincronización audio-video. Al trabajar con archivos separados de video y audio, puede especificar el nombre del archivo de video en este parámetro para asegurar que el audio se sincronice correctamente con el video.
    
    ```cs
    await core.Input_AddAudioFileAsync(
        audioFile,
        insertTime: null, 
        0);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    var audioFile = new AudioFileSource(
        filename,
        startTime: null,
        stopTime: null);
    
    core.Input_AddAudioFile(
        audioFile,
        insertTime: null);
    ```
    


### Combinar fuentes de video y audio

Para eficiencia, puede agregar fuentes combinadas de video y audio con una sola llamada de método:

=== "VideoEditCoreX"

    
    ```cs
    core.Input_AddAudioVideoFile(
        filename,
        startTime: null,
        stopTime: null,
        insertTime: null);
    ```
    


### Trabajar con imágenes estáticas

El SDK soporta agregar imágenes fijas a su línea de tiempo, incluyendo formatos JPG, PNG, BMP y GIF. Al agregar una imagen, necesitará especificar cuánto tiempo debe aparecer en la línea de tiempo:

=== "VideoEditCore"

    
    ```cs
    await core.Input_AddImageFileAsync(
        filename,
        duration: TimeSpan.FromMilliseconds(2000),
        timelineInsertTime: null,
        stretchMode: VideoEditStretchMode.Letterbox);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Input_AddImageFile(
        filename,
        duration: TimeSpan.FromMilliseconds(2000),
        insertTime: null);
    ```
    


## Configurar ajustes de salida

### Establecer formato de salida y opciones de codificación

El SDK ofrece opciones de salida flexibles con soporte para numerosos formatos de video y audio, incluyendo MP4, AVI, WMV, MKV, WebM, AAC, MP3, GIF animado y muchos otros.

Use la propiedad `Output_Format` para configurar su formato de salida deseado:

=== "VideoEditCore"

    
    ```cs
    var mp4Output = new MP4HWOutput();
    core.Output_Format = mp4Output;
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    var mp4Output = new MP4Output("output.mp4");
    core.Output_Format = mp4Output;
    ```
    


Para una lista completa de formatos de salida soportados y ejemplos de código detallados, por favor consulte nuestra sección de documentación de [Formatos de salida](../general/output-formats/index.md).

### Renderizar a GIF animado

`VideoEditCoreX` puede renderizar la línea de tiempo directamente a un GIF animado mediante `GIFOutput`. GIF no tiene contenedor — `gifenc` escribe el archivo `.gif` final en un solo paso, por lo que el perfil de codificación contiene solo el flujo de video y cualquier audio en la línea de tiempo se descarta.

```cs
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEncoders;

core.Output_Format = new GIFOutput("output.gif", new GIFEncoderSettings
{
    Speed = 10,    // 1..30 — un valor mayor = codificacion mas rapida, menor calidad visual
    Repeat = 0     // -1 = bucle infinito, 0 = reproducir una vez, n = reproducir n+1 veces
});
```

Notas:

- La salida GIF es solo de video — las pistas de audio se ignoran.
- La paleta de 256 colores por fotograma es una limitacion del formato — los archivos seran mas grandes que con codecs de video equivalentes para clips largos.
- `SmartRender = true` es incompatible con la salida GIF (ningun clip de origen estara ya en `image/gif`).

## Mejorar sus videos

### Aplicar efectos de video profesionales

El SDK proporciona una rica colección de efectos de video que puede aplicar para mejorar su contenido de video. Para información detallada sobre implementación de efectos, consulte nuestra guía de [Efectos de video](../general/video-effects/index.md).

El motor `VideoEditCoreX` incluye métodos de API dedicados para agregar superposiciones de texto. Para detalles de implementación, consulte nuestra guía de [Superposiciones de texto](./code-samples/add-text-overlay.md).

### Agregar transiciones suaves

Para crear transiciones de aspecto profesional entre clips de video, consulte nuestro [ejemplo de código de uso de transiciones](./code-samples/transition-video.md) detallado.

## Procesar su proyecto de video

### Iniciar el proceso de edición

Una vez que haya configurado todas sus fuentes, efectos y ajustes de salida, puede iniciar el proceso de edición:

=== "VideoEditCore"

    
    ```cs
    await core.StartAsync();
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Start();
    ```
    


Durante el proceso de edición, su aplicación recibirá actualizaciones de progreso a través de los manejadores de eventos que ha implementado, y será notificado cuando la operación complete a través del evento de parada.

## Conclusión

Siguiendo esta guía, ha aprendido las técnicas fundamentales para crear una aplicación potente de edición de video usando el Video Edit SDK para .NET. Esta base le permitirá construir herramientas sofisticadas de edición de video que pueden competir con software profesional de edición de video mientras están adaptadas a sus requisitos específicos.

Cuando su aplicación termine, desinicialice el SDK con `VisioForge.Core.VisioForgeX.DestroySDK()` (necesario para los motores X, incluido `VideoEditCoreX`) para evitar un bloqueo al salir; consulte [Inicialización](../init.md).
