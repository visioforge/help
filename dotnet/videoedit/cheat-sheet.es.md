---
title: Hoja de referencia de VisioForge Video Edit SDK en C# .NET
description: Referencia rápida de Video Edit SDK con paquetes NuGet, API VideoEditCoreX, ejemplo de línea de tiempo, plataformas y errores comunes.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - MAUI
  - WPF
  - WinForms
  - GStreamer
  - DirectShow
  - Editing
  - Encoding
  - Effects
  - Mixing
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoEditCore
  - MP4Output
  - VideoSource
  - VideoFileSource
---

# VisioForge Video Edit SDK .Net — Hoja de referencia

Video Edit SDK ensambla líneas de tiempo, aplica transiciones/efectos/superposiciones y exporta a MP4/MKV/WebM. `VideoEditCoreX` es el motor multiplataforma (GStreamer); `VideoEditCore` es el motor heredado exclusivo de Windows basado en DirectShow. Elija `VideoEditCoreX` para proyectos nuevos.

!!! tip "Agentes de IA para programación: usen el servidor MCP de VisioForge"

    ¿Está construyendo esto con **Claude Code**, **Cursor** u otro agente de IA para programación?
    Conéctese al [servidor MCP público de VisioForge](../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para búsquedas estructuradas en la API,
    ejemplos de código ejecutables y guías de despliegue — más precisos que
    rastrear `llms.txt`. No requiere autenticación.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Soporte de plataformas

- `VideoEditCoreX` (multiplataforma, GStreamer): Windows, macOS (Intel + Apple Silicon), Linux (Ubuntu/Debian/CentOS), Android (vía MAUI), iOS (vía MAUI).
- `VideoEditCore` (heredado, DirectShow): solo Windows x64.
- Frameworks de UI: WinForms, WPF, MAUI, Avalonia, Uno, Consola.
- Matriz completa motor × plataforma × UI: [platform-matrix.md](../platform-matrix.md).

## Paquetes NuGet

Motor multiplataforma (recomendado para proyectos nuevos):

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="*" />
<PackageReference Include="VisioForge.DotNet.VideoEditX" Version="*" />
```

Motor heredado exclusivo de Windows:

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="*" />
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="*" />
```

Integración de UI (elija el framework que utilice):

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.WinUI" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="*" />
```

Los redistribuibles nativos específicos de plataforma siguen el mismo patrón que el resto de SDKs de VisioForge (los paquetes redist por sistema operativo incorporan los binarios nativos de GStreamer). Consulte la [guía de instalación](../install/index.md) para ver la lista completa.

## Clases principales de la API

| Clase | Rol | Véase también |
|---|---|---|
| `VideoEditCoreX` | Motor multiplataforma de línea de tiempo (basado en GStreamer) | [getting-started.md](./getting-started.md) |
| `VideoEditCore` | Motor heredado exclusivo de Windows (DirectShow) — para mantener aplicaciones existentes en funcionamiento | [getting-started.md](./getting-started.md) |
| `MP4Output` | Configuración de salida MP4 codificada (H.264/H.265 + AAC) | [getting-started.md](./getting-started.md) |
| `GIFOutput` | Salida GIF animado (solo video, sin audio, paleta de 256 colores) — `gifenc` escribe el archivo directamente sin contenedor | [getting-started.md](./getting-started.md#renderizando-a-gif-animado) |
| `VideoSource` / `VideoFileSource` | Descriptor de entrada de vídeo por segmento (`VideoSource` para `VideoEditCore`, `VideoFileSource` para `VideoEditCoreX`) | [code-samples/several-segments.md](./code-samples/several-segments.md) |
| `AudioFileSource` | Entrada de audio desde archivo para mezclar en la línea de tiempo | [code-samples/volume-for-track.md](./code-samples/volume-for-track.md) |

## Ejemplo mínimo canónico

Fusión de línea de tiempo con dos clips, salida MP4 H.264, multiplataforma con `VideoEditCoreX`:

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// 1. Initialize SDK (cross-platform engines require this).
await VisioForgeX.InitSDKAsync();

// 2. Create editor. Pass an IVideoView for preview, or null for headless/console.
var editor = new VideoEditCoreX(videoView as IVideoView);

// 3. Wire up events.
editor.OnError    += (s, e) => Console.WriteLine($"Error: {e.Message}");
editor.OnProgress += (s, e) => Console.WriteLine($"Progress: {e.Progress}%");
editor.OnStop     += (s, e) => Console.WriteLine("Editing completed");

// 4. Set timeline output parameters BEFORE adding sources.
editor.Output_VideoSize      = new Size(1920, 1080);
editor.Output_VideoFrameRate = new VideoFrameRate(30);

// 5. Add sources to the timeline (audio + video in one call).
editor.Input_AddAudioVideoFile("/abs/path/intro.mp4", null, null, null);
editor.Input_AddAudioVideoFile("/abs/path/main.mp4",  null, null, null);

// 6. Configure MP4 output.
editor.Output_Format = new MP4Output("/abs/path/output.mp4");

// 7. Start processing. OnStop fires when rendering finishes.
editor.Start();

// ... dentro de su handler OnStop, o tras esperar una señal de finalización:
editor.Stop();
editor.Dispose();
VisioForgeX.DestroySDK(); // solo síncrono — no existe variante async
```

Para recortar, utilice `Input_AddVideoFile` con un `VideoFileSource(filename, startTime, stopTime, streamNumber, rate)` en su lugar — los valores `startTime`/`stopTime` delimitan el rango de la entrada.

## Flujo de trabajo típico

1. Inicialice el SDK: `await VisioForgeX.InitSDKAsync()` (solo motor multiplataforma — el `VideoEditCore` heredado lo omite).
2. Instancie `VideoEditCoreX` (o `VideoEditCore` para el heredado de Windows), pasando un `IVideoView` o `null` para modo sin interfaz.
3. Establezca resolución de salida, framerate y codificador (`Output_VideoSize`, `Output_VideoFrameRate`).
4. Añada las fuentes de entrada (archivos, segmentos, imágenes, audio) — **antes** de `Start`.
5. Establezca el archivo de salida mediante `MP4Output` (o `MKVOutput` / `WebMOutput` / `GIFOutput` para GIF animado) en `Output_Format`.
6. `Start()` (CoreX) o `await StartAsync()` (heredado) → espere a `OnStop` → `Dispose()` + `DestroySDK()`.

## Errores comunes

- **Establezca la resolución / framerate de salida de la línea de tiempo antes de añadir fuentes.** Cambiar `Output_VideoSize` u `Output_VideoFrameRate` después de `Input_Add*` puede reconfigurar el pipeline de forma silenciosa o provocar desincronización.
- **No mezcle las API de los motores.** `VideoEditCore` usa `VideoSource` + `Input_AddVideoFileAsync`; `VideoEditCoreX` usa `VideoFileSource` + `Input_AddVideoFile` (síncrono). Los espacios de nombres de tipos también difieren (`Types.VideoEdit` frente a `Types.X.VideoEdit`).
- **La inicialización solo aplica al motor X.** `VideoEditCoreX` requiere `VisioForgeX.InitSDKAsync()` / `DestroySDK()`. `VideoEditCore` (heredado) no — llamar a InitSDK en aplicaciones que solo usan el heredado es innecesario.
- **Utilice rutas de archivo absolutas.** Las rutas relativas se resuelven respecto al CWD del proceso y se comportan de forma inconsistente entre plataformas (especialmente en MAUI/iOS/Android, donde el CWD no es la carpeta del proyecto).
- **`Start` no es bloqueante; espere a `OnStop`.** No asuma que el renderizado ha terminado cuando `Start()` retorna. Utilice `OnStop` (o envuélvalo con `await`) para disparar el post-procesado, y después `Dispose()` y `DestroySDK()`.

## Véase también

- **Primeros pasos**
    - [Guía de primeros pasos](./getting-started.md) — recorrido completo para ambos motores.
    - [Índice de ejemplos de código](./code-samples/index.md) — superposiciones, transiciones, audio, composición.
- **Tareas específicas**
    - Fusionar / concatenar clips → [output-file-from-multiple-sources.md](./code-samples/output-file-from-multiple-sources.md)
    - Recortar / multisegmento desde un único archivo → [several-segments.md](./code-samples/several-segments.md)
    - Transiciones entre clips → [transition-video.md](./code-samples/transition-video.md), [referencia de transiciones](./transitions.md)
    - Superposición de texto → [add-text-overlay.md](./code-samples/add-text-overlay.md)
    - Superposición de imagen / logo → [add-image-overlay.md](./code-samples/add-image-overlay.md)
    - Imagen en imagen → [picture-in-picture.md](./code-samples/picture-in-picture.md)
    - Presentación de diapositivas a partir de imágenes → [video-images-console.md](./code-samples/video-images-console.md)
    - Mezcla de audio / envolvente de volumen → [audio-envelope.md](./code-samples/audio-envelope.md), [volume-for-track.md](./code-samples/volume-for-track.md)
    - Aplicación de edición para iOS → [ios-video-editor.md](./code-samples/ios-video-editor.md)
- **Despliegue** — [Windows / macOS / Ubuntu / Android / iOS](../deployment-x/index.md)
- **Instalación y matriz** — [Guía de instalación](../install/index.md) · [Matriz de plataformas](../platform-matrix.md)

## Preguntas frecuentes

### `VideoEditCoreX` vs `VideoEditCore` — ¿cuál debo usar?

Use `VideoEditCoreX` para proyectos nuevos. Funciona en Windows, macOS, Linux, Android e iOS sobre un backend de GStreamer. Use `VideoEditCore` únicamente para mantener en funcionamiento aplicaciones existentes de Windows basadas en DirectShow.

### ¿Puedo añadir transiciones entre clips?

Sí. `VideoEditCoreX` admite más de 100 transiciones de barrido SMPTE entre segmentos consecutivos — véase [transition-video.md](./code-samples/transition-video.md) y la [referencia de transiciones](./transitions.md).

### ¿Funciona en macOS / Linux?

Sí — `VideoEditCoreX` funciona en macOS (Intel + Apple Silicon) y Linux (Ubuntu/Debian/CentOS) gracias a los binarios nativos de GStreamer incluidos. `VideoEditCore` es exclusivo de Windows.

### ¿Cómo recorto un archivo sin volver a añadirlo varias veces?

Pase un array `FileSegment[]` al constructor de `VideoFileSource` — cada segmento se convierte en un rango recortado del mismo archivo de origen en la línea de tiempo de salida.
