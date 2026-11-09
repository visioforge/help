---
title: API push .NET de Virtual Camera — fotogramas a una webcam
description: Envíe fotogramas BGRA, BGR24 o YUY2 y audio PCM desde C# a la webcam y el micrófono virtuales de VisioForge con el paquete VisioForge.DotNet.VirtualCamera.
sidebar_label: API push .NET
order: 2
tags:
  - Virtual Camera SDK
  - .NET
  - C#
  - Windows
  - Virtual Camera
primary_api_classes:
  - VirtualVideoWriter
  - VirtualAudioWriter

---

# API push .NET de Virtual Camera

El paquete NuGet `VisioForge.DotNet.VirtualCamera` envía fotogramas de video y muestras de audio desde su aplicación .NET
directamente a VisioForge Virtual Camera y Virtual Audio Card. Zoom, OBS Studio y los navegadores ven la cámara como
una webcam normal, y las aplicaciones DirectShow también ven la Virtual Audio Card como un micrófono. No necesita
construir ningún grafo DirectShow.

El paquete no depende del resto del SDK de VisioForge. Está disponible para .NET Framework 4.7.2, .NET 8 y .NET 10
en Windows. Sus tipos están en el espacio de nombres `VisioForge.VirtualCamera`, por lo que puede referenciarse junto con
`VisioForge.DotNet.Core`, que incluye los mismos escritores en `VisioForge.Core.VirtualCamera.Push`. Un archivo que
también importe `VisioForge.Core.VirtualCamera.Push` debe calificar `VirtualVideoWriter` y `VirtualAudioWriter`, y uno que
importe `VisioForge.Core` debe calificar `BaseContext`; un alias con `using` también sirve.

## Instalación

```xml
<PackageReference Include="VisioForge.DotNet.VirtualCamera" Version="[2026.11.6]" />
<!-- The virtual camera and virtual audio card filters -->
<PackageReference Include="VisioForge.DotNet.Core.Redist.VirtualCamera.x64" Version="[2026.11.6]" />
```

`Redist.VirtualCamera.x64` sirve sea cual sea la arquitectura de su aplicación: el filtro lo carga la aplicación
consumidora, y Zoom, OBS y los navegadores son de 64 bits (más abajo se indican otros consumidores).
Cada paquete `Redist.VirtualCamera` incluye el paquete `Redist.Shared` de su arquitectura, con las `VisioForge_MFP*.dll` y
`VisioForge_MFPX*.dll` que cargan sus filtros (y `vcomp140.dll` en x64). Una aplicación que no es x64 y llama a
`PushFrameBgra` o `PushFrameBgr24` referencia además el `Redist.Shared` de su propia arquitectura (`.x86` o `.arm64`) para la
conversión de color.

## Registro de los filtros

Los filtros de origen deben registrarse en COM una vez en el equipo, con derechos de administrador. El paquete redist
los copia en la carpeta de la aplicación:

```csharp
using System;
using System.Diagnostics;
using System.IO;

var dir = AppContext.BaseDirectory;
var args = $"/s \"{Path.Combine(dir, "VisioForge_Virtual_Camera_x64.ax")}\" " +
           $"\"{Path.Combine(dir, "VisioForge_Virtual_Audio_Card_x64.ax")}\"";

// Un solo aviso de UAC para ambos filtros.
var info = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "regsvr32.exe"), args)
{
    UseShellExecute = true,
    Verb = "runas",
};

using (var process = Process.Start(info))
{
    process.WaitForExit();
    if (process.ExitCode != 0)
    {
        throw new InvalidOperationException($"regsvr32 failed with exit code {process.ExitCode}.");
    }
}
```

La aplicación consumidora carga la versión del filtro que coincide con su propia arquitectura, sea cual sea la de la
aplicación que envía los fotogramas. Los filtros x64 anteriores sirven a Zoom, OBS y los navegadores de 64 bits.
Para consumidores de 32 bits, registre también `VisioForge_Virtual_Camera.ax` y `VisioForge_Virtual_Audio_Card.ax` del
redist `.x86`; para consumidores ARM64 nativos, `VisioForge_Virtual_Camera_arm64.ax` y
`VisioForge_Virtual_Audio_Card_arm64.ax` de la subcarpeta `arm64` que el redist `.arm64` crea en la carpeta de la
aplicación.

Un instalador puede registrar los mismos archivos; consulte [Registro de filtros](../deployment/filter-registration.md).

## Envío de video

```csharp
using VisioForge.VirtualCamera;

const int width = 1280, height = 720;

// El último argumento es su clave de licencia de Virtual Camera SDK; null muestra la franja TRIAL.
using (var video = new VirtualVideoWriter(width, height, 30, "VideoStream1", null, "YOUR-LICENSE-KEY"))
{
    video.Start();

    // Cada 1/30 s: un fotograma BGRA, fila superior primero, stride en bytes.
    video.PushFrameBgra(bgraPixels, width * 4);

    video.Stop();
}
```

`VirtualVideoWriter` acepta fotogramas en tres formatos:

| Método | Orden de bytes | Origen habitual |
|---|---|---|
| `PushFrameBgra(data, stride)` | B, G, R, A por píxel, filas de arriba abajo; el alfa se ignora | WPF `Bgra32` (`Pbgra32` con píxeles opacos), GDI+ `Format32bppArgb`, Direct3D `B8G8R8A8` |
| `PushFrameBgr24(data, stride)` | B, G, R por píxel, filas de arriba abajo | WPF `Bgr24`, GDI+ `Format24bppRgb`, OpenCV `CV_8UC3` |
| `PushFrame(data)` | YUY2, exactamente `width * height * 2` bytes | su propio codificador o convertidor |

El ancho debe ser par. `PushFrameBgra` y `PushFrameBgr24` convierten a YUY2 de forma nativa en `VisioForge_MFPX64.dll`
(`VisioForge_MFPX.dll` en x86, `VisioForge_MFPX_ARM64.dll` en ARM64). Cada envío devuelve `false` cuando no hay ningún búfer libre, de modo que un consumidor lento
descarta un fotograma en lugar de bloquear su hilo.

## Envío de audio

```csharp
// PCM de 48 kHz, estéreo, 16 bits para el micrófono "VisioForge Virtual Audio Card".
using (var audio = new VirtualAudioWriter(48000, 2, 16, "AudioStream1", null))
{
    audio.Start();

    audio.PushSamples(pcmBytes); // un número entero de tramas de muestra de 4 bytes

    audio.Stop();
}
```

Las marcas de tiempo se calculan a partir de los bytes enviados, así que envíe el audio al ritmo al que se reproduce.

## Registro de mensajes

Los escritores reciben un `BaseContext` para sus advertencias y errores; con `null` van a `System.Diagnostics.Trace`.
Para enviarlos a otro lugar, derive de `BaseContext` y sobrescriba `Error` y `Warning`.

## Licencias

Sin clave de licencia, la cámara virtual dibuja una franja TRIAL sobre la imagen. Pase la clave que recibió con la compra
de Virtual Camera SDK al constructor de `VirtualVideoWriter`; la clave en
`HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\License` también funciona.

## Demo

**Virtual Camera Push Demo** (WPF, C#), en el [repositorio de ejemplos del SDK](https://github.com/visioforge/.Net-SDK-s-samples),
dibuja una imagen en movimiento con WPF, la envía con `PushFrameBgra`, manda un tono de 440 Hz al micrófono virtual y
registra los filtros con un botón.

## Consulte también

* [Virtual Camera SDK](index.md)
* [Ejemplos](examples.md)
