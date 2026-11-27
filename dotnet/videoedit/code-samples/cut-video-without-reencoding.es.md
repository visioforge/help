---
title: Cortar un video sin recodificar en pocas líneas de C# .NET
description: Copie una parte de un archivo de video en un archivo nuevo sin recodificar, con unas líneas de C#, Video Edit SDK .NET y FastEdit_CutFileAsync.
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
  - FastEditSeekMode

video:
  id: EASyGlUymvY
  upload_date: 2026-11-24T07:00:00Z
  duration: PT1M3S
---

# Cortar un video sin recodificar en C\#

[Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

Esta página corta una parte de un archivo de video en un archivo nuevo con [Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net) y su motor `VideoEditCore`, en una aplicación de consola C#. `FastEdit_CutFileAsync` copia las pistas comprimidas tal como están, así que nada se recodifica: los códecs, la resolución, la velocidad de fotogramas y la tasa de bits no cambian, no hay pérdida de calidad, y 30 segundos de una película 720p se guardan en aproximadamente medio segundo.

`VideoEditCore` es el motor de Windows del SDK; sus métodos `FastEdit_` ejecutan la herramienta FFmpeg incluida en el paquete redist de FFmpeg.

!!! tip "Agentes de IA: use el servidor MCP de VisioForge"

    ¿Desarrolla esto con **Claude Code**, **Cursor** u otro agente de IA?
    Conéctese al [servidor MCP público de VisioForge](../../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue, más precisos que
    buscar en `llms.txt`. No requiere autenticación.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutorial en video

El video de un minuto escribe y ejecuta esta aplicación de consola: 30 segundos de una película 720p se cortan en aproximadamente medio segundo.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/EASyGlUymvY?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Paquetes NuGet necesarios

- [VisioForge.DotNet.VideoEdit](https://www.nuget.org/packages/VisioForge.DotNet.VideoEdit/) — el SDK con `VideoEditCore`
- [VisioForge.DotNet.Core.Redist.FFMPEG.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/) — la herramienta FFmpeg que ejecutan los métodos `FastEdit_`; para una aplicación x86, [VisioForge.DotNet.Core.Redist.FFMPEG.x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="[2026.11.27]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.FFMPEG.x64" Version="[2026.11.27]" />
```

El redist es x64, así que establezca `PlatformTarget` en `x64` en el archivo del proyecto:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <TargetFramework>net10.0-windows</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

## El código

La aplicación de consola completa: copia de 1:00 a 1:30 de `movie.mp4` en `cut.mp4`.

```csharp
using System;
using VisioForge.Core.VideoEdit;

await using var core = new VideoEditCore();
core.OnError += (s, e) =>
    Console.WriteLine(e.Message);
var ok = await core.FastEdit_CutFileAsync(
    "movie.mp4",
    TimeSpan.FromSeconds(60),
    TimeSpan.FromSeconds(90),
    "cut.mp4");
Console.WriteLine(ok ? "Done" : "Failed");
```

El proyecto completo es [cut-video-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/cut-video-short-console). Los nombres de archivo son relativos al directorio de trabajo, así que coloque `movie.mp4` en la carpeta desde la que inicia la aplicación.

## Cómo funciona

1. **El motor** — `VideoEditCore` no necesita una vista de video para cortar. `await using` lo libera cuando termina la aplicación.
2. **Errores** — `OnError` indica por qué falló un corte, por ejemplo un archivo de entrada que no existe o la falta del redist de FFmpeg; la aplicación lo muestra antes de `Failed`.
3. **El corte** — `FastEdit_CutFileAsync(input, start, stop, output)` copia la parte de `start` a `stop` en el archivo de salida sin recodificar, y devuelve `true` cuando el archivo se ha guardado. Un `stop` igual a `TimeSpan.Zero` corta hasta el final del archivo.
4. **Fotogramas clave** — el corte empieza en el fotograma clave en `start` o antes de él. Los fotogramas entre ese fotograma clave y `start` también se copian. En una salida MP4 o MOV se marcan para omitirse, así que el reproductor empieza en `start`; en otros contenedores, y con una entrada MPEG-TS, el corte se reproduce desde ese fotograma clave y dura ese tiempo de más.

La salida con un Big Buck Bunny de 10 minutos (H.264 1280x720 a 24 fps, AAC), comprobada con ffprobe: 30,0 segundos, el perfil H.264 High y el AAC del original. El primer paquete de video del corte es el fotograma clave del original en 57,75 s, con exactamente el mismo tamaño; los fotogramas anteriores a 1:00 llevan la marca de descarte. [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, bajo [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

```console
ffprobe -v error -of compact cut.mp4 -show_entries format=duration:stream=codec_name,profile,width,height
stream|codec_name=h264|profile=High|width=1280|height=720
stream|codec_name=aac|profile=LC
format|duration=30.000544
```

## Notas

- **Modo de búsqueda** — la sobrecarga con `FastEditSeekMode` elige dónde busca FFmpeg. `FastEditSeekMode.Input` (el predeterminado) empieza en el fotograma clave en la posición inicial o antes de ella. `FastEditSeekMode.Output` descarta el video hasta el primer fotograma clave posterior, así que un corte más corto que un intervalo entre fotogramas clave puede quedar sin video, y un `stop` igual a `TimeSpan.Zero` copia el archivo completo.
- **Precisión de fotograma** — un corte sin recodificar no puede empezar entre fotogramas clave. Para cortar en un fotograma exacto, recodifique con la línea de tiempo de `VideoEditCore` o `VideoEditCoreX`.
- **El contenedor** — la salida recibe los códecs del original, así que use un contenedor que los admita: MP4 para H.264 o H.265 con AAC, MKV para los demás.
- **El redist de FFmpeg** — sin el paquete redist de FFmpeg, los métodos `FastEdit_` no tienen ninguna herramienta que ejecutar. Distribúyalo con su aplicación; consulte la [guía de implementación](../deployment.md).

## Aplicaciones de ejemplo

- [Cut video short console (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/_VideoTutorials/cut-video-short-console) — el código de esta página
- [Cut Video File (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/WPF/CSharp/Cut%20Video%20File) — la misma llamada en una ventana WPF con barra de progreso

## Preguntas frecuentes

### ¿Cómo corto un video sin recodificar en C#?

Cree `VideoEditCore` y llame a `FastEdit_CutFileAsync(inputFile, start, stop, outputFile)`. Las pistas se copian tal como están, así que el corte tarda aproximadamente lo mismo que copiar esa parte del archivo.

### ¿Por qué mi corte empieza un poco antes de la hora de inicio?

Un corte sin recodificar empieza en un fotograma clave. Con el modo de búsqueda predeterminado, los fotogramas desde el fotograma clave anterior a la hora de inicio se copian pero se marcan para omitirse, así que en un archivo MP4 o MOV los reproductores empiezan en la hora de inicio; algunos editores los muestran igualmente. Otros contenedores, y una entrada MPEG-TS, se reproducen desde el fotograma clave.

### ¿Puedo cortar hasta el final del archivo?

Sí: pase `TimeSpan.Zero` como hora de fin.

### ¿La calidad es la misma que la del original?

Sí. Nada se decodifica ni se codifica, así que cada fotograma copiado es el fotograma del original.

## Véase también

- [Unir video y audio sin recodificar](output-file-from-multiple-sources.md) — el video de un archivo y el audio de otro, copiados en un MP4
- [Varios segmentos](several-segments.md) — unir partes de archivos en un video, recodificado
- [Implementación](../deployment.md) — los redists que `VideoEditCore` necesita en el equipo del usuario
- [Página del producto Video Edit SDK .NET](https://www.visioforge.com/es/video-edit-sdk-net)
