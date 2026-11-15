---
title: Unir pistas de video y audio sin recodificar en C# .NET
description: Tome el video de un archivo y el audio de otro y cópielos en un MP4 sin recodificar, en una aplicación C# WinForms con Video Edit SDK .NET.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCore
  - FFMPEGStream
video:
  id: l59WVEu4uIs
  upload_date: 2026-11-15T09:00:11Z
  duration: PT9M57S

---

# Unir video y audio sin recodificar en C#

[Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

Este tutorial crea una aplicación C# WinForms que toma la pista de video de un archivo y la pista de audio de otro, y copia ambas en un único MP4 con [Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net) y su motor `VideoEditCore`. `FastEdit_MuxStreamsAsync` copia las pistas comprimidas tal cual, así que nada se recodifica: el códec, la resolución, la velocidad de fotogramas y la tasa de bits no cambian, no hay pérdida de calidad, y un clip de 27 segundos se guarda en un par de segundos. La misma llamada pone una nueva banda sonora bajo un video, sustituye su audio o añade una segunda pista de audio.

`VideoEditCore` es el motor de Windows del SDK; sus métodos `FastEdit_` ejecutan la herramienta FFmpeg que viene en el paquete redist de FFmpeg.

!!! tip "Agentes de IA: use el servidor MCP de VisioForge"

    ¿Desarrolla esto con **Claude Code**, **Cursor** u otro agente de IA?
    Conéctese al [servidor MCP público de VisioForge](../../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue, más precisos que
    buscar en `llms.txt`. No requiere autenticación.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutorial en YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/l59WVEu4uIs?controls=1" title="Unir video y audio sin recodificar en C# - tutorial en YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/mux-streams-winforms)

El video del tutorial es un clip de [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, bajo licencia [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

El video crea el proyecto en .NET 10, que requiere Visual Studio 2026; en Visual Studio 2022, elija .NET 8 o .NET 9. En .NET Framework 4.7.2, `Form1.cs` se compila sin cambios en un proyecto de estilo SDK (créelo con la plantilla Windows Forms App y establezca `TargetFramework` en `net472`) con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`; en `Program.cs`, sustituya `ApplicationConfiguration.Initialize()` por `Application.EnableVisualStyles()` y `Application.SetCompatibleTextRenderingDefault(false)`.

## Paquetes NuGet necesarios

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/): el SDK con `VideoEditCore`
- [VisioForge.DotNet.Core.Redist.FFMPEG.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/): la herramienta FFmpeg que ejecutan los métodos `FastEdit_`; para una aplicación x86, [VisioForge.DotNet.Core.Redist.FFMPEG.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.11.13" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.FFMPEG.x64" Version="2026.11.13" />
```

El redist es x64, así que establezca `PlatformTarget` en `x64` en el archivo de proyecto:

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <UseWindowsForms>true</UseWindowsForms>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

## El formulario

`Form1` contiene un cuadro de texto y un botón `...` para el archivo de video (`edVideo`, `btVideo`), lo mismo para el archivo de audio (`edAudio`, `btAudio`), un cuadro de texto para el archivo de salida (`edOutput`), una casilla `cbShortest` ("Stop at the end of the shortest stream", marcada), el botón `btStart` y un `OpenFileDialog`. Los eventos `Load` y `FormClosing` del formulario y los eventos `Click` de los tres botones tienen controladores.

## El código

`Form1.cs`:

```cs
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;

namespace MuxStreams;

public partial class Form1 : Form
{
    private VideoEditCore videoEdit1;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoEdit1 = new VideoEditCore();

        // Muestra los errores del SDK en la barra de título
        videoEdit1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
    }

    private void btVideo_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK) edVideo.Text = openFileDialog1.FileName;
    }

    private void btAudio_Click(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.OK) edAudio.Text = openFileDialog1.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        // La primera pista de video de un archivo y la primera pista de audio de otro
        var streams = new List<FFMPEGStream>
        {
            new FFMPEGStream { Filename = edVideo.Text, ID = "v:0" },
            new FFMPEGStream { Filename = edAudio.Text, ID = "a:0" },
        };

        btStart.Enabled = false;
        Text = "Saving...";
        try
        {
            // Copia las pistas en un solo archivo tal cual, sin recodificar
            var saved = await videoEdit1.FastEdit_MuxStreamsAsync(streams, cbShortest.Checked, edOutput.Text);
            if (saved) Text = "Saved: " + edOutput.Text; // si falla, el título conserva el error
        }
        finally
        {
            btStart.Enabled = true;
        }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Deja terminar el archivo antes de que se cierre la ventana
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            return;
        }

        videoEdit1.Dispose();
    }
}
```

## Cómo funciona

1. **El motor**: `Form1_Load` crea `VideoEditCore`. `OnError` informa de los errores del motor; pueden llegar en un subproceso de trabajo, así que `BeginInvoke` los pone en la barra de título en el subproceso de la interfaz. El archivo de salida es `output.mp4` en `Videos` por defecto.
2. **Las pistas**: cada `FFMPEGStream` indica un archivo (`Filename`) y una pista dentro de él (`ID`). Los ID son especificadores de pista de FFmpeg: `"v:0"` es la primera pista de video del archivo, `"a:0"` la primera pista de audio y `"a:1"` la segunda. Una `"v"` o una `"a"` solas toman todas las pistas de ese tipo.
3. **La copia**: `FastEdit_MuxStreamsAsync(streams, shortest, outputFile)` copia las pistas de la lista, en ese orden, en el archivo de salida sin recodificar, y devuelve `true` cuando el archivo está guardado.
4. **La pista más corta**: con `shortest` en `true`, el archivo termina con la pista más corta. Con `false`, dura lo que la más larga: en el video, 108 segundos de música bajo un clip de 27 segundos.
5. **Cerrar la ventana**: mientras se guarda, `Start` está desactivado y `Form1_FormClosing` cancela el cierre para que el archivo termine primero. Si no, libera el motor.

La salida del video, comprobada con ffprobe: la pista H.264 del original, 1920x1080 con la misma tasa de bits, y la pista de música AAC. El MD5 de la pista de video es el mismo en el original y en la salida: la pista se copia byte a byte.

```console
ffprobe -v error -of compact output.mp4 -show_entries format=duration,size:stream=codec_name,width,height,bit_rate,sample_rate
stream|codec_name=h264|width=1920|height=1080|bit_rate=2783582
stream|codec_name=aac|sample_rate=48000|bit_rate=192448
format|duration=27.466667|size=10225512
```

## Notas

- **El contenedor debe aceptar los códecs**: nada se recodifica, así que el formato de salida tiene que admitir los códecs del original tal cual. H.264 o H.265 con AAC o MP3 van en MP4; para otros códecs, escriba un archivo MKV.
- **El audio propio del archivo de video**: `"v:0"` toma solo el video de ese archivo, así que su banda sonora queda fuera. Para conservarla como segunda pista, añada `new FFMPEGStream { Filename = edVideo.Text, ID = "a:0" }` a la lista.
- **Más pistas de audio**: cada `FFMPEGStream` con un ID de audio se convierte en una pista de audio más en la salida, por ejemplo un segundo idioma.
- **El redist de FFmpeg**: sin el paquete redist de FFmpeg, los métodos `FastEdit_` no tienen herramienta que ejecutar. Distribúyalo con su aplicación; consulte la [guía de despliegue](../deployment.md).

## Aplicaciones de ejemplo

- [Tutorial de unión de pistas, WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/mux-streams-winforms): el código de esta página y del video

## Preguntas frecuentes

### ¿Cómo añado una pista de audio a un video sin recodificar en C#?

Cree `VideoEditCore`, enumere la pista de video (`ID = "v:0"`) y la pista de audio (`ID = "a:0"`) como elementos `FFMPEGStream`, y llame a `FastEdit_MuxStreamsAsync(streams, true, outputFile)`. El código anterior lo hace con dos archivos elegidos en un cuadro de diálogo.

### ¿Cómo sustituyo el audio de un archivo MP4?

Tome solo el video del MP4 con `"v:0"` y el nuevo audio del otro archivo con `"a:0"`. La banda sonora propia del MP4 no está en la lista, así que no está en la salida.

### ¿Qué hace el parámetro shortest?

Con `true`, la salida termina donde termina la pista más corta. Con `false`, la salida dura lo que la pista más larga, y la más corta simplemente se detiene.

### ¿La calidad es la misma que la del original?

Sí. Las pistas se copian, no se decodifican y se vuelven a codificar, así que el códec, la resolución, la velocidad de fotogramas y la tasa de bits son los de los archivos originales.

## Transcripción del video

### Qué vamos a construir

Vamos a tomar el video de un archivo y el audio de otro, y a unirlos en un MP4 sin recodificar, en una aplicación C# WinForms con el VisioForge Video Edit SDK. Esto es lo que vamos a construir. Una aplicación WinForms que une dos archivos en uno. Toma la pista de video de un archivo y la pista de audio de otro. Ambas se copian tal cual, sin recodificar y sin pérdida de calidad. Y una casilla termina el archivo al final de la pista más corta. Esta es la aplicación terminada. Acaba de poner una nueva pista de música bajo un clip de Big Buck Bunny.

### Copiar, no recodificar

¿Por qué copiar en lugar de recodificar? Recodificar decodifica cada fotograma y lo vuelve a comprimir. Eso cuesta tiempo, y cada pasada pierde un poco de calidad. Una copia de pistas mueve los datos comprimidos a un archivo nuevo, así que el códec, la resolución, la velocidad de fotogramas y la tasa de bits quedan exactamente como estaban. No hay pérdida de calidad, y tarda segundos en lugar de minutos. La única regla: el contenedor de salida debe aceptar ambos códecs. H.264 o H.265 con AAC o MP3 van en MP4. Para cualquier otra cosa, use MKV.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Y necesita un archivo de video y un archivo de audio. VideoEditCore es el motor de Windows del SDK, y sus métodos FastEdit ejecutan la herramienta FFmpeg que viene con el SDK.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto MuxStreams. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada dos paquetes NuGet: el propio Video Edit SDK y el redist de FFmpeg. El redist incluye la herramienta FFmpeg que ejecutan los métodos FastEdit. En Visual Studio, es Manage NuGet Packages. Busque VisioForge.DotNet.VideoEdit en Browse e instálelo. Después instale el redist de FFmpeg para x64. El redist es el de x64, así que ponga PlatformTarget en x64 en el archivo del proyecto.

### Diseñar el formulario WinForms

Desde el Toolbox, añada una etiqueta, un cuadro de texto y un botón pequeño para el archivo de video, y dé a cada control un nombre claro en la ventana Properties. Añada la misma fila para el archivo de audio, y luego una etiqueta y un cuadro de texto para el archivo de salida. Añada una casilla para terminar al final de la pista más corta, marcada por defecto, y el botón Start. Después un OpenFileDialog. Va a la bandeja bajo el formulario. Su filtro enumera los archivos multimedia. Haga doble clic en el formulario y en los tres botones. Visual Studio añade un manejador vacío para cada uno.

### Crear el motor de edición

Al principio de Form1.cs, añada estos using. En el controlador Load del formulario, cree el motor VideoEditCore. Los errores del motor llegan en el evento OnError. Aquí se muestran en la barra de título. El archivo de salida va a la carpeta Videos por defecto.

### Elegir los archivos

Los dos botones pequeños abren el mismo cuadro de diálogo y ponen el archivo elegido en su cuadro de texto.

### Unir las pistas

El controlador de Start enumera las pistas que se copian. Cada FFMPEGStream indica un archivo y una pista dentro de él. v:0 es la primera pista de video del archivo de video, y a:0 es la primera pista de audio del archivo de audio. Los ID siguen los especificadores de pista de FFmpeg. v:0 es la primera pista de video, a:0 la primera pista de audio y a:1 la segunda. Una "v" o una "a" solas toman todas las pistas de ese tipo del archivo. Así que la banda sonora propia del clip queda fuera: solo pedimos su video. Después desactiva Start y llama a FastEdit_MuxStreamsAsync con las pistas, la casilla y el archivo de salida. La llamada copia las pistas en un solo archivo y devuelve true cuando el archivo está guardado. El segundo argumento es la casilla. Marcada, el archivo termina con la pista más corta: aquí, veintisiete segundos de video y veintisiete segundos de música. Desmarcada, el archivo dura lo que la pista más larga: aquí, ciento ocho segundos, la mayor parte música sin imagen.

### Cerrar la ventana

Cerrar la ventana mientras guarda dejaría el archivo sin terminar. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este manejador. Mientras se guarda, cancela el cierre. Si no, libera el motor.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. El archivo de salida ya apunta a la carpeta Videos. Elija el archivo de video: un clip de Big Buck Bunny. Después el archivo de audio: una pista de música. Haga clic en Start. Un par de segundos después, la barra de título dice que el archivo está guardado. Luego cerramos la ventana.

### Comprobar el archivo con ffprobe

El archivo queda en Videos: veintisiete segundos de video de 1920 por 1080 a treinta fotogramas por segundo. ffprobe muestra lo que hay dentro. El video es H.264 de 1920 por 1080, con la misma tasa de bits que el original. El audio es la pista de música AAC. Y el MD5 de la pista de video es el mismo en los dos archivos: se copió byte a byte.

### Errores frecuentes

Algunos errores frecuentes. Falta el paquete redist de FFmpeg, así que FastEdit no tiene herramienta que ejecutar. Una "v" o una "a" solas donde se quería v:0 o a:0, así que el archivo acaba con pistas de más. Un códec que el contenedor de salida no admite: use MKV en su lugar. Y cerrar la ventana a mitad de un guardado: espere primero a que termine.

### Próximos pasos

A continuación, el mismo motor corta una parte de un archivo sin recodificar, con FastEdit_CutFileAsync, y une archivos del mismo formato con FastEdit_JoinFilesAsync. Y el tutorial de imágenes a video crea un video completamente nuevo con transiciones. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Véase también

- [Crear un video a partir de imágenes con transiciones](video-images-console.md): un video nuevo renderizado a partir de una carpeta de imágenes
- [Múltiples pistas de audio en archivos de video](adding-video-file-with-multiple-audio-streams.md): seleccionar y mezclar las pistas de audio de un archivo de video
- [Despliegue](../deployment.md): los redists que `VideoEditCore` necesita en el equipo del usuario
- [Página del producto Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net)
