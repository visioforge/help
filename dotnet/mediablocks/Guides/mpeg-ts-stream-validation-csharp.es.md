---
title: Analizar y validar flujos MPEG-TS en C# .NET paso a paso
description: Guía en C# para analizar un archivo o flujo MPEG-TS UDP/SRT en vivo, leer resultados ETSI TR 101 290 y alertar sobre errores con Media Blocks SDK .NET.
sidebar_label: Validación de Flujos MPEG-TS
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - Analysis
  - TR 101 290
  - UDP
  - SRT
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerSettings
  - TSAnalyzerReport
  - UDPRAWMPEGTSSourceBlock

---

# Analizar y Validar Flujos MPEG-TS en C# .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Esta guía muestra cómo analizar y validar un flujo de transporte MPEG-TS desde C# usando el `TSAnalyzerBlock` del [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net). Usted hará lo siguiente:

1. Analizar un archivo local `.ts` / `.m2ts` y leer el reporte final.
2. Supervisar un feed `udp://` o `srt://` **en vivo** con actualizaciones periódicas.
3. Leer resultados **ETSI TR 101 290** y generar una alerta cuando el flujo no sea conforme.
4. Analizar un feed **mientras lo graba** usando el modo passthrough.

Para la referencia completa de la API (cada ajuste y campo del reporte), consulte la página [Bloque Analizador de Flujos MPEG-TS](../Special/TSAnalyzerBlock.md).

## Requisitos previos

Instale el paquete NuGet de Media Blocks SDK y el paquete de runtime de la plataforma:

- `VisioForge.DotNet.MediaBlocks`
- Un runtime de plataforma, p. ej. `VisioForge.CrossPlatform.Core.Windows.x64` en Windows.

Inicialice el SDK una vez al arrancar:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

Los espacios de nombres usados a lo largo de esta guía:

```csharp
using System;
using System.Linq;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Special;
```

## 1. Analizar un archivo

Para un archivo, ejecute el pipeline hasta el final del flujo y lea el reporte final. Conecte el analizador en modo `Input` (terminal):

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new BasicFileSourceBlock("stream.ts");
var analyzer = new TSAnalyzerBlock(TSAnalyzerMode.Input);

// Señalar la finalización cuando el pipeline se detenga por sí mismo (final del flujo).
var completed = new TaskCompletionSource<bool>();
pipeline.OnStop += (s, e) => completed.TrySetResult(true);

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
await completed.Task;             // esperar al final del flujo
await pipeline.StopAsync();

TSAnalyzerReport report = analyzer.GetReport();

Console.WriteLine($"Tamaño de paquete: {report.PacketSize}  |  total: {report.TotalBitrateKbps:F0} kbps");
Console.WriteLine($"Programas: {report.Programs.Count}  |  PAT: {(report.HasPAT ? "sí" : "no")}");

foreach (var program in report.Programs)
{
    var name = string.IsNullOrEmpty(program.ServiceName) ? "(sin nombre)" : program.ServiceName;
    Console.WriteLine($"Programa {program.ProgramNumber} \"{name}\" (PMT {program.PmtPid}, PCR {program.PcrPid})");

    foreach (var es in program.Streams)
    {
        var lang = string.IsNullOrEmpty(es.Language) ? "" : $" [{es.Language}]";
        Console.WriteLine($"    PID {es.Pid}  {es.Kind}  {es.Codec}{lang}");
    }
}
```

## 2. Supervisar un flujo en vivo

Para un feed `udp://` o `srt://` en vivo, suscríbase a `OnAnalysisUpdated` y lea cada instantánea periódica. Configure `StatisticsInterval` para controlar la cadencia.

```csharp
var pipeline = new MediaBlocksPipeline();

// UDP multicast en vivo (MPEG-TS sin procesar, sin demux)
var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

var analyzer = new TSAnalyzerBlock(
    TSAnalyzerMode.Input,
    new TSAnalyzerSettings { StatisticsInterval = TimeSpan.FromSeconds(1) });

analyzer.OnAnalysisUpdated += (sender, e) =>
{
    if (e.IsFinal)
    {
        return;     // reporte de final de flujo; las instantáneas periódicas tienen IsFinal == false
    }

    TSAnalyzerReport report = e.Report;
    Console.WriteLine(
        $"{report.TotalBitrateKbps:F0} kbps total, " +
        $"{report.EffectiveBitrateKbps:F0} kbps efectivo, " +
        $"programas: {report.Programs.Count}");
};

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
// ... mantener en ejecución mientras supervisa ...
```

Para alimentar SRT en lugar de UDP, sustituya la fuente:

```csharp
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://server:9000"), ignoreMediaInfoReader: true);
var source = new SRTRAWSourceBlock(srtSettings);
```

> **Hilos (threading).** Las llamadas periódicas a `OnAnalysisUpdated` se ejecutan en un hilo de temporizador en segundo plano. En una aplicación de UI, marshalle al hilo de UI antes de actualizar los controles — p. ej. `Dispatcher.BeginInvoke(() => UpdateUI(e.Report));` en WPF.

## 3. Leer TR 101 290 y alertar sobre errores

El reporte TR 101 290 agrupa los errores por prioridad. Un patrón habitual es alertar cada vez que aparece un error de Prioridad 1 (crítico) y registrar las comprobaciones fallidas individuales:

```csharp
analyzer.OnAnalysisUpdated += (sender, e) =>
{
    var tr = e.Report.Tr101290;
    if (tr == null || tr.AllOk)
    {
        return;     // TR 101 290 desactivado, o el flujo es totalmente conforme
    }

    if (tr.P1Count > 0)
    {
        RaiseAlert($"Errores TR 101 290 (P1) críticos: {tr.P1Count}");
    }

    foreach (var check in tr.Checks.Where(c => c.Count > 0))
    {
        Console.WriteLine($"P{check.Priority} {check.Name}: {check.Count}");
    }
};
```

`tr.P1Count` / `P2Count` / `P3Count` le dan los totales por prioridad; `tr.Checks` le permite profundizar en una medición fallida específica (por ejemplo `PCR_repetition_error` o `Continuity_count_error`). Use `tr.AllOk` como compuerta de conformidad de una sola línea.

## 4. Analizar mientras se graba (passthrough)

Para validar un feed y grabarlo en el mismo pipeline, cree el analizador en modo `InputOutput` y conecte su `Output` a un sumidero. El flujo de transporte se reenvía sin alterar, de modo que la grabación es una copia exacta a nivel de bits:

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

// Analizador passthrough: reenvía el flujo mientras lo analiza
var analyzer = new TSAnalyzerBlock(TSAnalyzerMode.InputOutput);
analyzer.OnAnalysisUpdated += (s, e) => { /* supervisar mientras se graba */ };

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("recording.ts"));

pipeline.Connect(source.Output, analyzer.Input);
pipeline.Connect(analyzer.Output, sink.Input);

await pipeline.StartAsync();
```

## Solución de problemas

- **`HasPAT` es false / no hay programas.** Probablemente la entrada no sea un flujo de bytes MPEG-TS válido, o el tamaño de paquete se detectó mal. Fuércelo mediante `TSAnalyzerSettings.PacketSize` (`Size188` o `Size192`), o verifique el URI de la fuente.
- **Muchas entradas `PCR_repetition_error` en un archivo válido.** Esto es esperable cuando el muxer emite el PCR más lento que el límite de 40 ms de TR 101 290 (común en el muxing orientado a archivos). El flujo se reproduce igual; sencillamente no es conforme con PCR de 40 ms.
- **`GetReport()` devuelve null.** El bloque aún no se ha construido — llámelo después de `StartAsync()`.
- **No se reciben paquetes multicast.** Verifique que `AutoJoinMulticast` sea `true` (el valor por defecto) y configure `MulticastInterface` si el host tiene varias interfaces de red.
- **CPU alta en un flujo pesado.** Desactive las etapas que no necesite mediante `TSAnalyzerSettings` — `ParseEPG` está desactivado por defecto; también puede desactivar `ParseCodecDetails` o `TrackPtsDts`.

## Preguntas frecuentes

### ¿Cómo analizo un flujo MPEG-TS UDP multicast en vivo en C#?

Cree un `UDPRAWMPEGTSSourceBlock` con un URI `udp://group:port`, conecte su `Output` a un `TSAnalyzerBlock` en modo `Input` y suscríbase a `OnAnalysisUpdated` para obtener instantáneas periódicas. Consulte la [sección 2](#2-supervisar-un-flujo-en-vivo).

### ¿Puedo comprobar la conformidad con ETSI TR 101 290 desde .NET?

Sí. Con `ValidateTR101290` habilitado (el valor por defecto), `report.Tr101290` expone la lista de comprobaciones Prioridad 1/2/3 con conteos por comprobación y estado de aprobado/fallido, además de los totales `P1Count`/`P2Count`/`P3Count` y un indicador `AllOk`. Consulte la [sección 3](#3-leer-tr-101-290-y-alertar-sobre-errores).

### ¿Puedo grabar un flujo y analizarlo al mismo tiempo?

Sí. Use `TSAnalyzerMode.InputOutput` y conecte el `Output` del analizador a un sumidero como `MPEGTSSinkBlock`. El flujo se reenvía sin alterar. Consulte la [sección 4](#4-analizar-mientras-se-graba-passthrough).

## Véase también

- [Bloque Analizador de Flujos MPEG-TS](../Special/TSAnalyzerBlock.md) — la referencia completa del bloque.
- [Análisis MPEG-TS en C#: VisioForge vs ffprobe](mpeg-ts-analysis-vs-ffprobe.md) — posicionamiento y comparación de funcionalidades.
