---
title: Aplicaciones de Edición de Video SDK .NET
description: Edición de video en SDK .NET con líneas de tiempo personalizables, múltiples formatos, transiciones, efectos y vistas previas en tiempo real.
sidebar_position: 0
---

# Construyendo Aplicaciones Profesionales de Edición de Video con SDK .NET

## Introducción a la Edición de Video con .NET

El Video Edit SDK ofrece funcionalidad potente para desarrolladores .NET que desean crear aplicaciones profesionales de edición de video. Este SDK soporta una amplia gama de plataformas y frameworks de UI, permitiéndote construir editores de video ricos en características que manejan múltiples formatos, aplican efectos, gestionan transiciones y entregan salida de alta calidad.

## Configurando Tu Entorno de Desarrollo

### Creando Tu Proyecto Inicial

El SDK está diseñado para funcionar sin problemas con varios entornos de desarrollo. Puedes utilizar tanto Visual Studio como JetBrains Rider para crear tu proyecto, seleccionando tu plataforma y framework de UI preferidos según tus requisitos.

Para un proceso de configuración fluido, por favor consulta nuestra [guía de instalación](index.md) detallada que proporciona instrucciones para agregar los paquetes NuGet necesarios y configurar las dependencias nativas correctamente.

## Implementando la Funcionalidad Principal de Edición de Video

### Inicializando el Motor de Edición de Video

El SDK proporciona un objeto de edición principal robusto que sirve como la base de tu aplicación de edición de video. Sigue estos pasos para crear e inicializar este componente esencial:

=== "VideoEditCore"

    
    ```cs
    private VideoEditCore core;
    
    core = new VideoEditCore(VideoView1 as IVideoView);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    private VideoEditCoreX core;
    
    core = new VideoEditCoreX(VideoView1 as IVideoView);
    ```
    


Necesitarás especificar un objeto Video View como parámetro para habilitar la funcionalidad de vista previa de video durante las operaciones de edición.

### Implementando Manejo de Eventos Robusto

#### Gestión de Eventos de Error

Para gestión adecuada de errores en tu aplicación, implementa el manejador de evento `OnError`:

```cs
core.OnError += Core_OnError;

private void Core_OnError(object sender, ErrorsEventArgs e)
{
    Debug.WriteLine("Error: " + e.Message);
}
```

#### Sistema de Seguimiento de Progreso

Para mantener a tus usuarios informados sobre procesos en curso, implementa el manejador de evento de progreso:

```cs
core.OnProgress += Core_OnProgress;

private void Core_OnProgress(object sender, ProgressEventArgs e)
{
    Debug.WriteLine("Progreso: " + e.Progress);
}
```

#### Notificación de Finalización de Operación

Para detectar cuando las operaciones de edición han completado, implementa el manejador de evento de parada:

```cs
core.OnStop += Core_OnStop;

private void Core_OnStop(object sender, EventArgs e)
{
    Debug.WriteLine("Edición completada");
}
```

### Configurando Parámetros de Línea de Tiempo

Antes de agregar fuentes de medios a tu proyecto, debes establecer los parámetros básicos de línea de tiempo como velocidad de cuadros y resolución, que varían dependiendo de qué motor estés usando:

=== "VideoEditCore"

    
    ```cs
    core.Video_FrameRate = new VideoFrameRate(30);
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Output_VideoSize = new VisioForge.Core.Types.Size(1920, 1080);
    core.Output_VideoFrameRate = new VideoFrameRate(30);
    ```
    


## Trabajando con Fuentes de Medios

### Gestionando Video y Audio en Tu Línea de Tiempo

El SDK te permite agregar varias fuentes de medios a tu línea de tiempo usando métodos de API directos. Para cada fuente, puedes controlar precisamente parámetros como tiempo de inicio, tiempo de fin y posición en la línea de tiempo. Tienes la flexibilidad de agregar fuentes de video y audio tanto independientemente como juntas.

### Integrando Archivos de Video

El SDK soporta una extensa gama de formatos de video incluyendo MP4, AVI, MOV, WMV y muchos otros. Así es como agregar contenido de video a tu proyecto:

Primero, crea un objeto de fuente de video y establece la ruta del archivo fuente. Puedes especificar tiempos de inicio y fin en el constructor o usar parámetros null para incluir el archivo completo.

Para archivos con múltiples streams de video, puedes seleccionar qué stream usar especificando el número de stream.

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
    


Puedes controlar la velocidad de reproducción ajustando el parámetro rate. Por ejemplo, establecer rate a 2.0 reproducirá el archivo al doble de la velocidad normal.

Un constructor alternativo te permite agregar múltiples segmentos de archivo:

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
    


Para agregar la fuente a tu línea de tiempo, usa los siguientes métodos:

=== "VideoEditCore"

    
    ```cs
    await core.Input_AddVideoFileAsync(
        videoFile, 
        null, 
        0);
    ```
    
    El tercer parámetro especifica el número de stream de video de destino. Usa 0 para agregar la fuente al primer stream de video.
    
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

### Incorporando Archivos de Audio

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
    
    El parámetro `fileToSync` habilita la sincronización audio-video. Al trabajar con archivos separados de video y audio, puedes especificar el nombre del archivo de video en este parámetro para asegurar que el audio se sincronice correctamente con el video.
    
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
    


### Combinando Fuentes de Video y Audio

Para eficiencia, puedes agregar fuentes combinadas de video y audio con una sola llamada de método:

=== "VideoEditCoreX"

    
    ```cs
    core.Input_AddAudioVideoFile(
        filename,
        startTime: null,
        stopTime: null,
        insertTime: null);
    ```
    


### Trabajando con Imágenes Estáticas

El SDK soporta agregar imágenes fijas a tu línea de tiempo, incluyendo formatos JPG, PNG, BMP y GIF. Al agregar una imagen, necesitarás especificar cuánto tiempo debe aparecer en la línea de tiempo:

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
    


## Configurando Ajustes de Salida

### Estableciendo Formato de Salida y Opciones de Codificación

El SDK ofrece opciones de salida flexibles con soporte para numerosos formatos de video y audio, incluyendo MP4, AVI, WMV, MKV, WebM, AAC, MP3 y muchos otros.

Usa la propiedad `Output_Format` para configurar tu formato de salida deseado:

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
    


Para una lista completa de formatos de salida soportados y ejemplos de código detallados, por favor consulta nuestra sección de documentación de [Formatos de Salida](../general/output-formats/index.md).

## Mejorando Tus Videos

### Aplicando Efectos de Video Profesionales

El SDK proporciona una rica colección de efectos de video que puedes aplicar para mejorar tu contenido de video. Para información detallada sobre implementación de efectos, consulta nuestra guía de [Efectos de Video](../general/video-effects/index.md).

El motor `VideoEditCoreX` incluye métodos de API dedicados para agregar superposiciones de texto. Para detalles de implementación, consulta nuestra guía de [Superposiciones de Texto](code-samples/add-text-overlay.md).

### Agregando Transiciones Suaves

Para crear transiciones de aspecto profesional entre clips de video, consulta nuestro [ejemplo de código de uso de transiciones](code-samples/transition-video.md) detallado.

## Procesando Tu Proyecto de Video

### Iniciando el Proceso de Edición

Una vez que hayas configurado todas tus fuentes, efectos y ajustes de salida, puedes iniciar el proceso de edición:

=== "VideoEditCore"

    
    ```cs
    await core.StartAsync();
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    core.Start();
    ```
    


Durante el proceso de edición, tu aplicación recibirá actualizaciones de progreso a través de los manejadores de eventos que has implementado, y serás notificado cuando la operación complete a través del evento de parada.

## Conclusión

Siguiendo esta guía, has aprendido las técnicas fundamentales para crear una aplicación potente de edición de video usando el Video Edit SDK para .NET. Esta base te permitirá construir herramientas sofisticadas de edición de video que pueden competir con software profesional de edición de video mientras están adaptadas a tus requisitos específicos.
