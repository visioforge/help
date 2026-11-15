---
title: Crear un video a partir de imágenes con transiciones en C#
description: Convierta una carpeta de imágenes JPEG en un MP4 1080p con fundido cruzado, cortinillas y fundido de entrada y salida, con Video Edit SDK .NET.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - Editing
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoTransition
  - VideoTransitionType
  - MP4Output
  - StopEventArgs
video:
  id: Kef_q2cvdAI
  upload_date: 2026-11-14T07:54:56Z
  duration: PT9M17S

---

# Crear un video a partir de imágenes con transiciones en C#

[Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

Este tutorial crea una aplicación de consola en C# que convierte una carpeta de imágenes JPEG en un único video MP4 con [Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net) y su motor `VideoEditCoreX`. Cada imagen permanece 3 segundos en pantalla, y la siguiente empieza 1 segundo antes de que termine; en ese segundo, una transición lleva de una imagen a la siguiente: primero un fundido cruzado, después una cortinilla de barra, un iris, una cortinilla de reloj y una de puerta de granero. El video aparece desde negro y termina fundiéndose a negro, y sale en 1920x1080 a 30 fotogramas por segundo en H.264.

`VideoEditCoreX` es el motor GStreamer del SDK. Esta aplicación se compila para Windows x64; para macOS y Linux, los paquetes nativos del motor están en la [guía de despliegue multiplataforma](../../deployment-x/index.md).

!!! tip "Agentes de IA: use el servidor MCP de VisioForge"

    ¿Desarrolla esto con **Claude Code**, **Cursor** u otro agente de IA?
    Conéctese al [servidor MCP público de VisioForge](../../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue, más precisos que
    buscar en `llms.txt`. No requiere autenticación.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutorial en YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Kef_q2cvdAI?controls=1" title="Crear un video a partir de imágenes con transiciones en C# - tutorial en YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-console)

Las imágenes del video son fotogramas de [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, bajo licencia [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

El video crea el proyecto en .NET 10, que requiere Visual Studio 2026; en Visual Studio 2022, elija .NET 8 o .NET 9.

## Paquetes NuGet necesarios

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/): el SDK con `VideoEditCoreX`
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/): GStreamer y las bibliotecas nativas con las que funciona el motor, entre ellas el codificador H.264

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="[2026.11.13]" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.12]" />
```

## El archivo de proyecto

La plantilla de consola de Visual Studio usa `net10.0`, y para ese framework de destino el paquete del SDK resuelve su compilación para Linux. Añada `-windows` al framework de destino (`net8.0-windows` o `net9.0-windows` en Visual Studio 2022) y establezca `PlatformTarget` en `x64`, porque el paquete nativo es x64:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

## El código

`Program.cs`, con instrucciones de nivel superior. El proyecto tiene `ImplicitUsings` activado, por lo que `System`, `System.IO`, `System.Linq` y `System.Threading.Tasks` no aparecen en la lista.

```cs
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// Las imágenes, en orden de nombre de archivo, y el destino del video.
var folder = args.Length > 0 ? args[0] : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "slides");
var images = Directory.Exists(folder) ? Directory.GetFiles(folder, "*.jpg").Order().ToArray() : [];
var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "slideshow.mp4");
if (images.Length == 0)
{
    Console.WriteLine($"No .jpg files in {folder}");
    return;
}

await VisioForgeX.InitSDKAsync();

var edit = new VideoEditCoreX();
var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
edit.OnError += (s, e) => Console.WriteLine($"\n{e.Message}");
edit.OnProgress += (s, e) => Console.Write($"\rRendering: {e.Progress}%");
edit.OnStop += (s, e) => done.TrySetResult(e.Successful);

// Cada imagen permanece 3 segundos en pantalla, y la siguiente empieza 1 segundo antes de que termine.
var show = TimeSpan.FromSeconds(3);
var overlap = TimeSpan.FromSeconds(1);
VideoTransitionType[] transitions =
[
    VideoTransitionType.Crossfade,
    VideoTransitionType.BarWipeLr,
    VideoTransitionType.IrisRect,
    VideoTransitionType.SinglesweepCwt,
    VideoTransitionType.BarndoorV,
];

for (int i = 0; i < images.Length; i++)
{
    var start = (show - overlap) * i;
    edit.Input_AddImageFile(images[i], show, start);

    // La transición ocupa el segundo en que esta imagen y la anterior se solapan.
    if (i > 0)
    {
        var type = transitions[(i - 1) % transitions.Length];
        edit.Video_Transitions.Add(new VideoTransition(type, start, start + overlap));
    }
}

// Fundido desde negro al inicio y a negro al final.
var total = (show - overlap) * (images.Length - 1) + show;
edit.Video_Transition_Add_FadeIn(TimeSpan.Zero, overlap);
edit.Video_Transition_Add_FadeOut(total - overlap, total);

edit.Output_VideoSize = new Size(1920, 1080);
edit.Output_VideoFrameRate = new VideoFrameRate(30);
edit.Output_Format = new MP4Output(output);

// Start devuelve false cuando el renderizado no puede comenzar.
var saved = edit.Start() && await done.Task;
Console.WriteLine(saved ? $"\nSaved: {output}" : "\nThe video was not saved.");

edit.Dispose();
VisioForgeX.DestroySDK();
```

## Cómo funciona

1. **Las imágenes**: la carpeta llega por la línea de comandos o es `Pictures\slides`. `Directory.GetFiles` toma todos los archivos `.jpg` que contiene, y `Order()` los ordena por nombre, así que nombre los archivos `01.jpg`, `02.jpg`, etc. El video se guarda como `slideshow.mp4` en `Videos`.
2. **El motor**: `VisioForgeX.InitSDKAsync` inicializa el SDK antes de crear `VideoEditCoreX`. `OnError` y `OnProgress` escriben en la consola; `OnStop` completa un `TaskCompletionSource` con `StopEventArgs.Successful`, que es `false` cuando no se pudo terminar el archivo de salida.
3. **La línea de tiempo**: `Input_AddImageFile(file, duration, insertTime)` coloca cada imagen en su propio inicio, 2 segundos después de la anterior, durante 3 segundos, de modo que las imágenes vecinas se solapan 1 segundo. La imagen se escala al tamaño de salida y conserva su relación de aspecto.
4. **Las transiciones**: a partir de la segunda imagen, una `VideoTransition` de un `VideoTransitionType` cubre el solapamiento; `Video_Transitions` las contiene. `Crossfade` mezcla las dos imágenes; `BarWipeLr`, `IrisRect`, `SinglesweepCwt` y `BarndoorV` son cortinillas SMPTE. La lista se repite cuando hay más imágenes que transiciones.
5. **Fundido de entrada y salida**: el video dura 2 segundos por imagen más 1 por la última. `Video_Transition_Add_FadeIn` aparece desde negro durante el primer segundo, y `Video_Transition_Add_FadeOut` se funde a negro durante el último.
6. **La salida**: `Output_VideoSize` y `Output_VideoFrameRate` fijan 1920x1080 a 30 fotogramas por segundo, y `MP4Output` codifica el video en H.264 en un archivo MP4.
7. **El renderizado**: `Start` devuelve `false` cuando el renderizado no puede comenzar. Si no, la aplicación espera a `OnStop`; un renderizado que un error interrumpe por el camino también genera `OnStop`, con `Successful` en `false`. Después se libera el motor y `VisioForgeX.DestroySDK` libera el SDK.

Seis imágenes dan un video de 13 segundos: seis veces 3 segundos, menos cinco segundos de solapamiento.

## Notas

- **La marca de agua de prueba**: sin licencia, el SDK dibuja un UNREGISTERED rojo en la esquina superior izquierda de cada fotograma. Una licencia la elimina; el código no cambia.
- **El framework de destino**: con `net10.0` en lugar de `net10.0-windows`, la aplicación recibe la compilación para Linux del SDK y falla con una `DllNotFoundException`.
- **Nombres de archivo**: los nombres se ordenan como texto, así que `10.jpg` va antes que `2.jpg`. Rellene los números: `01`, `02`.
- **Extensiones de archivo**: el patrón `*.jpg` omite los archivos `.jpeg` y, en macOS y Linux, donde los nombres distinguen mayúsculas y minúsculas, también los `.JPG`.
- **Otros formatos de imagen**: el motor también lee PNG, BMP y GIF. `Directory.GetFiles` acepta un solo patrón, así que llámelo una vez por extensión y ordene los archivos juntos.
- **Cerrar la aplicación antes de tiempo**: el archivo MP4 solo se puede reproducir cuando `OnStop` se ha generado; una aplicación que termina antes deja el codificador sin finalizar.

## Aplicaciones de ejemplo

- [Tutorial de imágenes a video, consola (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/_VideoTutorials/images-to-video-console): el código de esta página y del video

## Preguntas frecuentes

### ¿Cómo creo un video de presentación a partir de imágenes en C#?

Cree `VideoEditCoreX`, añada cada imagen con `Input_AddImageFile(file, duration, insertTime)`, establezca `Output_Format` en `new MP4Output(path)`, llame a `Start` y espere a `OnStop`. El código anterior lo hace para una carpeta entera.

### ¿Cómo añado un fundido cruzado entre dos imágenes?

Haga que las dos imágenes se solapen en la línea de tiempo y añada `new VideoTransition(VideoTransitionType.Crossfade, start, stop)` a `Video_Transitions` para el solapamiento: `start` es donde empieza la segunda imagen y `stop` es donde termina la primera.

### ¿Cómo cambio cuánto tiempo se muestra cada imagen?

Cambie `show`, y `overlap` para la duración de las transiciones. Cada imagen empieza entonces `show - overlap` después de la anterior.

### ¿Qué transiciones puedo usar?

`Crossfade` y las cortinillas SMPTE del enum `VideoTransitionType`: cortinillas de barra, de caja, el iris, barridos de reloj, puertas de granero y más. Consulte [Transición entre clips](transition-video.md).

## Transcripción del video

### Qué vamos a construir

Vamos a crear un video a partir de una carpeta de imágenes, con transiciones entre ellas, en una aplicación de consola C#, con el VisioForge Video Edit SDK. Esto es lo que vamos a construir. Una aplicación de consola que toma una carpeta de imágenes JPEG y escribe un único video MP4. Cada imagen permanece en pantalla 3 segundos, y la siguiente empieza 1 segundo antes de que termine. En ese segundo, una transición pasa de una imagen a la siguiente: primero un fundido cruzado, luego un barrido de barra, un iris, un barrido de reloj y una puerta de granero. El video aparece desde negro y se desvanece a negro. Es de 1920 por 1080 a 30 fotogramas por segundo, en H.264. Este es el video que la aplicación crea a partir de seis imágenes. Son fotogramas de Big Buck Bunny, de la Blender Foundation, con licencia Creative Commons Attribution.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto es una aplicación de consola en .NET 10. Y una carpeta de imágenes JPEG. El motor aquí es VideoEditCoreX, el motor GStreamer del SDK.

### Crear un proyecto de consola

En Visual Studio, cree un proyecto nuevo. Busque console y elija Console App para C#. Llame al proyecto ImagesToVideo, elija .NET 10 y haga clic en Create.

### Añadir los paquetes NuGet

Ahora añada dos paquetes NuGet. VisioForge.DotNet.VideoEdit contiene VideoEditCoreX. CrossPlatform Core Windows x64 contiene GStreamer y las bibliotecas nativas con las que funciona el motor. O use el Administrador de paquetes NuGet: haga clic derecho en el proyecto, Manage NuGet Packages, e instale VisioForge.DotNet.VideoEdit desde la pestaña Browse. Luego instale el paquete Core Windows x64. La pestaña Installed muestra ambos.

### Editar el archivo del proyecto

Una aplicación de consola necesita un paso más. Abra el archivo del proyecto. La plantilla de consola apunta a net10.0, y esa compilación del SDK es la de Linux. Añada -windows al framework de destino. Y el paquete nativo es x64, así que establezca PlatformTarget en x64.

### Encontrar las imágenes

Program.cs empieza con cinco usings de VisioForge: el SDK en sí, sus tipos comunes, los formatos de salida, los tipos de edición de video y el motor. La carpeta viene de la línea de comandos, o es una carpeta slides dentro de Pictures. La aplicación toma todos los archivos JPEG que contiene, ordenados por nombre, así que nómbrelos 01, 02, 03, y así sucesivamente. El video se guarda como slideshow.mp4 en Videos. Si no hay imágenes, la aplicación lo indica y se detiene.

### Crear el editor

Inicialice el SDK con InitSDKAsync y cree VideoEditCoreX. Un TaskCompletionSource espera el final del renderizado. OnError muestra los errores, y OnProgress muestra el progreso en porcentaje. OnStop se dispara cuando termina el renderizado, y su indicador Successful dice si el archivo se completó.

### Construir la línea de tiempo

Cada imagen está en pantalla 3 segundos, y la siguiente se superpone a ella 1 segundo. Las transiciones vienen del enum VideoTransitionType: Crossfade, y luego cuatro barridos. BarWipeLr barre de izquierda a derecha. IrisRect abre un rectángulo desde el centro. SinglesweepCwt barre como la manecilla de un reloj. Y BarndoorV se abre como una puerta de granero. Cada imagen empieza 2 segundos después de la anterior. Input_AddImageFile coloca la imagen en la línea de tiempo en ese inicio, durante 3 segundos. La imagen se escala al tamaño de salida y mantiene su relación de aspecto. A partir de la segunda imagen, una VideoTransition ocupa el segundo en que las dos imágenes se superponen. La lista se repite cuando hay más imágenes que transiciones. La duración total es de 2 segundos por imagen, más 1 por la última. Video_Transition_Add_FadeIn aparece desde negro durante el primer segundo, y Video_Transition_Add_FadeOut se desvanece durante el último. Luego la salida: 1920 por 1080 a 30 fotogramas por segundo, a un archivo MP4. MP4Output codifica el video en H.264.

### Renderizar el vídeo

Start devuelve false cuando el renderizado no puede empezar. Si no, la aplicación espera OnStop con await y muestra si el video se guardó, y dónde. Si un error detiene el renderizado a mitad de camino, OnStop se dispara igualmente, con Successful en false. Por último, libere el motor con Dispose y libere el SDK.

### Ejecutarla

Ponga el código en Program.cs y ejecute la aplicación. Las seis imágenes ya están en Pictures, slides. La consola muestra el progreso y luego la ruta del video guardado.

### Comprobar el resultado

ffprobe lo confirma. H.264, 1920 por 1080, 30 fotogramas por segundo y 13 segundos: seis imágenes de 3 segundos, menos cinco segundos de superposición. A mitad de la primera transición, se ven las dos imágenes a la vez: es el fundido cruzado. Aquí el barrido de barra ha llegado a la mitad del fotograma. El iris abre un rectángulo desde el centro. El barrido de reloj gira alrededor del centro. Y la puerta de granero se abre desde el centro.

### La marca de agua de prueba

Quizá haya notado el UNREGISTERED rojo en la esquina superior izquierda. Es la versión de prueba: sin licencia, el SDK lo dibuja en cada imagen. Una licencia lo elimina, y el código sigue siendo el mismo.

### Errores frecuentes

Algunos errores frecuentes. Dejar la plataforma de destino en net10.0, sin -windows: esa es la compilación del SDK para Linux, y la aplicación falla con una DllNotFoundException. Nombrar los archivos 1, 2 y hasta 10: los nombres se ordenan como texto, así que 10 va antes que 2. Use 01, 02 en su lugar. Poner archivos PNG en la carpeta: este código solo busca archivos JPEG, así que cambie el patrón de búsqueda si necesita más. Y cerrar la aplicación antes de OnStop: el codificador no ha terminado, y el archivo MP4 no se puede reproducir.

### Próximos pasos

También hay guías sobre transiciones entre clips de video, superposiciones de texto e imagen, cómo unir varios archivos en uno y cómo distribuir los redists con su aplicación. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Véase también

- [Transición entre clips](transition-video.md): transiciones entre archivos de video en la línea de tiempo
- [Añadir una superposición de texto](add-text-overlay.md): títulos y subtítulos sobre el video
- [Añadir una superposición de imagen](add-image-overlay.md): un logotipo o una imagen sobre el video
- [Unir varias fuentes en un archivo](output-file-from-multiple-sources.md): archivos de video y audio unidos en una línea de tiempo
- [Despliegue multiplataforma](../../deployment-x/index.md): los paquetes nativos que `VideoEditCoreX` necesita en cada plataforma
- [Página del producto Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net)
