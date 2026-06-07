---
title: Bloque analizador de flujo de transporte MPEG-TS en C#
description: Analice programas MPEG-TS, bitrate por PID, PCR y conformidad ETSI TR 101 290 en C# con el TSAnalyzerBlock de Media Blocks SDK .NET, en vivo y en proceso.
sidebar_label: Analizador TS
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - MPEG-TS
  - Streaming
  - Analysis
  - TR 101 290
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerSettings
  - TSAnalyzerReport
  - UDPRAWMPEGTSSourceBlock

---

# Bloque Analizador de Flujos MPEG-TS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

El `TSAnalyzerBlock` convierte el [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) en un **monitor MPEG-TS de calidad profesional (broadcast) que se ejecuta dentro de su propia aplicación .NET** — sin herramientas externas, sin invocar una CLI, sin analizar salida de texto. Apúntelo a un archivo, a un feed UDP multicast en vivo o a un flujo SRT y reportará continuamente la lista de programas, el bitrate por PID, la temporización PCR, el cifrado (scrambling), la información de servicio y la conformidad completa con **ETSI TR 101 290** Prioridad 1/2/3 — como objetos fuertemente tipados que puede enlazar a un panel, registrar o usar para alertas.

Analiza el flujo de transporte sin procesar paquete por paquete (análisis de PAT/PMT/PSI, contadores de continuidad, recuperación del reloj PCR, temporización PES, cabeceras de códec) y expone todo a través de un único `TSAnalyzerReport`. El bloque se ejecuta como sumidero terminal (modo `Input`) o como passthrough en línea (modo `InputOutput`), de modo que puede analizar un flujo **mientras lo está grabando o retransmitiendo**.

En resumen, el bloque mide:

- **Estructura** — PAT/PMT/PSI, programas/servicios, PIDs de PMT/PCR, tipos de flujo y códecs.
- **Bitrate** — bitrate por PID y total (promedio móvil, instantáneo y de pico), además del relleno con paquetes nulos y el bitrate efectivo de carga útil.
- **Temporización** — intervalo PCR mín./prom./máx., discontinuidades, jitter, errores de repetición; presencia de PTS/DTS en PES y desfase de sincronización audio/video.
- **Integridad** — errores de contador de continuidad, indicador de error de transporte, CRC-32 de tablas PSI y la lista completa de comprobaciones TR 101 290 P1/P2/P3.
- **Información de servicio** — nombre/proveedor/tipo de servicio del SDT, nombre de red del NIT, hora UTC del flujo TDT/TOT, idioma de audio ISO 639, estado de cifrado y (opcionalmente) eventos EIT/EPG.
- **Detalles de códec** — resolución, perfil, nivel, formato de croma y relación de aspecto para video H.264, HEVC y MPEG-2.

## Cómo funciona

En modo `Input` el bloque es el terminal del pipeline — consume el flujo de transporte y produce el reporte:

```mermaid
graph LR;
    Source["Fuente (archivo / UDP / SRT)"]-->TSAnalyzerBlock;
```

En modo `InputOutput` el flujo sin alterar se reenvía al pad de salida, de modo que puede analizar y grabar/retransmitir al mismo tiempo:

```mermaid
graph LR;
    Source["Fuente (archivo / UDP / SRT)"]-->TSAnalyzerBlock;
    TSAnalyzerBlock-->MPEGTSSinkBlock;
```

### Información del bloque

Nombre: TSAnalyzerBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada | Flujo de bytes MPEG-TS | 1 |
| Salida | Flujo de bytes MPEG-TS (passthrough) | 1 en modo `InputOutput`, 0 en modo `Input` |

## Modos

El constructor recibe un `TSAnalyzerMode`:

| Modo | Descripción |
| --- | --- |
| `TSAnalyzerMode.Input` | Analizador terminal. El bloque acepta el flujo y no expone pad de salida — úselo cuando solo necesite el reporte. |
| `TSAnalyzerMode.InputOutput` | Analizador passthrough. El flujo se reenvía sin alterar al pad de salida mientras se analiza, de modo que puede grabarlo o retransmitirlo en el mismo pipeline. |

```csharp
TSAnalyzerBlock(TSAnalyzerMode mode = TSAnalyzerMode.Input, TSAnalyzerSettings settings = null)
```

## Inicio rápido

```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Special;

await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Feed MPEG-TS UDP multicast en vivo (bytes sin procesar, sin demux)
var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

// Analizador terminal con una cadencia de instantáneas de 1 segundo
var analyzer = new TSAnalyzerBlock(
    TSAnalyzerMode.Input,
    new TSAnalyzerSettings { StatisticsInterval = TimeSpan.FromSeconds(1) });

analyzer.OnAnalysisUpdated += (sender, e) =>
{
    TSAnalyzerReport report = e.Report;        // e.IsFinal == true al final del flujo
    Console.WriteLine($"{report.TotalBitrateKbps:F0} kbps, programas: {report.Programs.Count}");

    if (report.Tr101290 != null && report.Tr101290.P1Count > 0)
    {
        Console.WriteLine($"Errores TR 101 290 Prioridad 1: {report.Tr101290.P1Count}");
    }
};

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
// ... ejecutar hasta el final del flujo o una detención del usuario ...
await pipeline.StopAsync();

// También puede obtener una instantánea bajo demanda en cualquier momento:
TSAnalyzerReport final = analyzer.GetReport();
```

## Alimentar el analizador (archivo / UDP / SRT)

El analizador acepta un flujo de bytes MPEG-TS sin procesar de cualquier bloque de fuente que produzca uno. Los tres feeds habituales:

```csharp
// 1. Archivo local (.ts / .m2ts)
var fileSource = new BasicFileSourceBlock("stream.ts");
pipeline.Connect(fileSource.Output, analyzer.Input);

// 2. UDP en vivo (unicast o multicast) — MPEG-TS sin procesar, sin demux
var udpSource = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));
pipeline.Connect(udpSource.Output, analyzer.Input);

// 3. SRT en vivo
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://server:9000"), ignoreMediaInfoReader: true);
var srtSource = new SRTRAWSourceBlock(srtSettings);
pipeline.Connect(srtSource.Output, analyzer.Input);
```

### UDPRAWMPEGTSSourceBlock

`UDPRAWMPEGTSSourceBlock` recibe un flujo de transporte UDP unicast o multicast en vivo y expone el flujo de bytes MPEG-TS **sin alterar** en su pad `Output` — sin demultiplexar — que es exactamente lo que necesita el analizador. Se configura mediante `UDPRAWMPEGTSSourceSettings`:

| Propiedad | Tipo | Valor por defecto | Descripción |
| --- | :---: | :---: | --- |
| `Uri` | `Uri` | — | URI de la fuente, p. ej. `udp://239.0.0.1:1234` (multicast) o `udp://0.0.0.0:1234` (todas las interfaces). |
| `AutoJoinMulticast` | `bool` | `true` | Unirse/abandonar automáticamente el grupo multicast para direcciones multicast. |
| `MulticastInterface` | `string` | `null` | Interfaz(es) de red en la(s) que unirse al grupo multicast (p. ej. `"eth0"` o `"eth0,eth1"`). |
| `BufferSize` | `int` | `524288` | Tamaño del búfer de recepción del kernel en bytes; `0` usa el valor por defecto del SO. |
| `PacketSize` | `int` | `188` | Tamaño de paquete TS anunciado: `188` (estándar) o `192` (estilo M2TS con un prefijo de timecode de 4 bytes). |

```csharp
var settings = new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234")
{
    AutoJoinMulticast = true,
    PacketSize = 188
};

var source = new UDPRAWMPEGTSSourceBlock(settings);
```

> **Bloque relacionado.** Si desea el flujo **demultiplexado** en pads de video/audio codificados por separado (para grabar o remultiplexar sin recodificar) en lugar del flujo de bytes sin procesar, use `UDPRAWSourceBlock`. Consulte la [guía de Grabación UDP MPEG-TS](../Guides/udp-mpegts-record-without-reencoding.md).

## Configuración

`TSAnalyzerSettings` controla la cadencia de instantáneas y qué etapas de análisis están habilitadas. Las etapas más costosas pueden desactivarse cuando no las necesite.

| Propiedad | Tipo | Valor por defecto | Descripción |
| --- | :---: | :---: | --- |
| `StatisticsInterval` | `TimeSpan` | `1 second` | Con qué frecuencia se produce una instantánea y se genera `OnAnalysisUpdated`. |
| `TrackPCR` | `bool` | `true` | Rastrear la temporización PCR (estadísticas de intervalo y discontinuidades). |
| `ValidateContinuity` | `bool` | `true` | Validar errores de contador de continuidad por PID. |
| `CalculateBitrate` | `bool` | `true` | Calcular el bitrate por PID y total. |
| `PacketSize` | `TSPacketSizeMode` | `Auto` | Manejo del tamaño de paquete TS — `Auto`, `Size188` o `Size192`. |
| `ParseServiceInfo` | `bool` | `true` | Analizar la información de servicio DVB (SDT, NIT, TDT/TOT) para obtener nombres de servicio y red. |
| `ParseEPG` | `bool` | `false` | Analizar EIT para eventos EPG. Opcional — es la etapa más costosa. |
| `TrackScrambling` | `bool` | `true` | Rastrear el cifrado mediante los bits `transport_scrambling_control`. |
| `ValidateTR101290` | `bool` | `true` | Evaluar las comprobaciones ETSI TR 101 290 Prioridad 1/2/3. |
| `TrackPtsDts` | `bool` | `true` | Rastrear la presencia de PTS/DTS en PES y la sincronización audio/video. |
| `ParseCodecDetails` | `bool` | `true` | Analizar los detalles del códec (resolución/perfil/nivel) desde las cabeceras SPS/de secuencia. |

Valores de `TSPacketSizeMode`: `Auto` (detecta 188 o 192 a partir del espaciado del byte de sincronización), `Size188` (MPEG-TS estándar), `Size192` (M2TS / Blu-ray con un prefijo de marca de tiempo de 4 bytes).

## El modelo del reporte

Cada instantánea es un `TSAnalyzerReport`. Los campos de nivel superior resumen todo el flujo; las colecciones anidadas describen los programas, los PIDs y la temporización.

| Propiedad | Tipo | Descripción |
| --- | :---: | --- |
| `PacketSize` | `int` | Tamaño de paquete TS detectado (188 o 192). |
| `TotalBitrateKbps` | `double` | Bitrate total del flujo (promedio móvil desde el inicio del análisis). |
| `TotalPackets` | `long` | Total de paquetes TS analizados. |
| `HasPAT` | `bool` | Si se detectó una PAT (PID 0). |
| `TransportErrors` | `long` | Paquetes con el indicador de error de transporte activado. |
| `Programs` | `List<TSProgramInfo>` | Programas/servicios descubiertos a partir de PAT + PMT. |
| `Pids` | `List<TSPidInfo>` | Estadísticas por PID. |
| `Pcr` | `List<TSPcrStats>` | Estadísticas de temporización PCR (una entrada por PID de PCR). |
| `NullPacketCount` | `long` | Conteo de paquetes de relleno nulo (PID 0x1FFF). |
| `NullBitrateKbps` | `double` | Bitrate consumido por el relleno nulo. |
| `EffectiveBitrateKbps` | `double` | Bitrate efectivo de carga útil (total menos el relleno nulo). |
| `SyncByteErrors` | `long` | Errores de byte de sincronización (0x47 no en la posición esperada). |
| `SyncLossEvents` | `long` | Eventos de pérdida de sincronización (ocurrencias de re-sincronización del analizador). |
| `NetworkName` | `string` | Nombre de red del descriptor de red del NIT, o `null`. |
| `StreamTimeUtc` | `DateTime?` | Hora UTC del flujo desde TDT/TOT, o `null`. |
| `AvSyncMs` | `double?` | Desfase de sincronización audio/video en ms (PTS de video menos PTS de audio), o `null`. |
| `Tr101290` | `TSTR101290Report` | El reporte de mediciones TR 101 290, o `null` si la validación está desactivada. |
| `Events` | `List<TSEpgEvent>` | Eventos EPG desde EIT (cuando `ParseEPG` está habilitado). |

### Programas y flujos elementales

`TSProgramInfo` (por programa/servicio):

| Propiedad | Tipo | Descripción |
| --- | :---: | --- |
| `ProgramNumber` | `int` | Número de programa (ID de servicio). |
| `PmtPid` | `int` | PID de la PMT de este programa. |
| `PcrPid` | `int` | PID que transporta el PCR de este programa. |
| `Streams` | `List<TSElementaryStreamInfo>` | Flujos elementales de este programa. |
| `ServiceName` | `string` | Nombre de servicio del SDT, o `null`. |
| `ServiceProvider` | `string` | Proveedor de servicio del SDT, o `null`. |
| `ServiceType` | `int` | Byte `service_type` de DVB (p. ej. `0x01` = televisión digital). |
| `IsScrambled` | `bool` | Si el servicio está cifrado (`free_CA_mode` del SDT). |

`TSElementaryStreamInfo` (por flujo elemental):

| Propiedad | Tipo | Descripción |
| --- | :---: | --- |
| `Pid` | `int` | PID del flujo elemental. |
| `StreamType` | `byte` | Byte `stream_type` de MPEG-TS. |
| `Codec` | `string` | Nombre legible del códec/contenido. |
| `Kind` | `TSPidKind` | Tipo de medio clasificado (Video, Audio, Private, …). |
| `Language` | `string` | Código de idioma ISO 639 del descriptor de la PMT, o `null`. |
| `CodecDetails` | `TSCodecDetails` | Detalles del códec analizados, o `null`. |

`TSCodecDetails`: `Width`, `Height`, `FrameRateNum`/`FrameRateDen`, `Profile`, `Level`, `ChromaFormat`, `AspectRatio`. La resolución se analiza para H.264, HEVC y MPEG-2. El perfil, el nivel y el formato de croma se analizan solo para H.264 y HEVC. La tasa de fotogramas y la relación de aspecto se leen de la cabecera de secuencia MPEG-1/2 (todavía no del VUI de H.264/HEVC).

### Estadísticas por PID

`TSPidInfo`:

| Propiedad | Tipo | Descripción |
| --- | :---: | --- |
| `Pid` | `int` | Identificador de paquete. |
| `Kind` | `TSPidKind` | Rol clasificado del PID. |
| `StreamType` | `byte` | Byte `stream_type` (para flujos elementales). |
| `Codec` | `string` | Nombre legible del códec/contenido. |
| `BitrateKbps` | `double` | Bitrate promedio (promedio móvil). |
| `InstantBitrateKbps` | `double` | Bitrate por intervalo más reciente. |
| `PeakBitrateKbps` | `double` | Bitrate de pico por intervalo observado. |
| `PacketCount` | `long` | Total de paquetes en este PID. |
| `ContinuityErrors` | `long` | Errores de contador de continuidad detectados. |
| `IsPcrPid` | `bool` | Si este PID transporta el PCR. |
| `IsScrambled` | `bool` | Si se observaron paquetes cifrados. |
| `ScrambledPacketCount` | `long` | Paquetes con `transport_scrambling_control` distinto de cero. |
| `IsUnreferenced` | `bool` | PID no referenciado por PAT/PMT/PSI y que no es relleno nulo. |
| `HasPts` / `HasDts` | `bool` | Si se detectó un PTS / DTS en la carga útil PES. |

### Temporización PCR

`TSPcrStats`: `Pid`, `SampleCount`, `MinIntervalMs`, `AvgIntervalMs`, `MaxIntervalMs`, `Discontinuities`, `MaxJitterMs` (mayor desviación respecto al intervalo medio móvil) y `RepetitionErrors` (intervalos que superan los 40 ms según TR 101 290).

## ETSI TR 101 290

TR 101 290 es el estándar de mediciones de DVB para la supervisión de flujos de transporte. Agrupa los errores en tres prioridades — **P1** (deben estar presentes para decodificar), **P2** (recomendados para supervisión continua) y **P3** (información de servicio/SI opcional). Cuando `ValidateTR101290` está habilitado, el analizador rellena `report.Tr101290`:

| Propiedad | Tipo | Descripción |
| --- | :---: | --- |
| `Checks` | `List<TSTR101290Check>` | Comprobaciones individuales de todos los grupos de prioridad. |
| `P1Count` / `P2Count` / `P3Count` | `long` | Conteo total de errores por grupo de prioridad. |
| `TotalCount` | `long` | Suma de todos los grupos. |
| `AllOk` | `bool` | `true` cuando todas las comprobaciones pasaron (`TotalCount == 0`). |

Cada `TSTR101290Check` tiene `Name` (p. ej. `PAT_error`, `CRC_error`), `Priority` (1/2/3), `Count` y `Ok`. Las comprobaciones evaluadas incluyen:

- **Prioridad 1:** TS_sync_loss, Sync_byte_error, PAT_error, Continuity_count_error, PMT_error, PID_error.
- **Prioridad 2:** Transport_error, CRC_error, PCR_error, PCR_repetition_error (>40 ms), PCR_discontinuity_indicator_error, PCR_accuracy_error, PTS_error (>700 ms), CAT_error.
- **Prioridad 3:** NIT_error, SI_repetition_error, Unreferenced_PID, SDT_error, EIT_error, TDT_error.

```csharp
var tr = report.Tr101290;
if (tr != null && !tr.AllOk)
{
    foreach (var check in tr.Checks.Where(c => c.Count > 0))
    {
        Console.WriteLine($"P{check.Priority} {check.Name}: {check.Count}");
    }
}
```

> **Por qué esto importa.** Un flujo puede reproducirse en un reproductor multimedia y aun así no ser conforme — un PCR que se repite más lento que 40 ms, un salto de CC, una PMT ausente. TR 101 290 es la forma en que los operadores de broadcast cuantifican eso. El `TSAnalyzerBlock` le brinda los mismos veredictos estructurados P1/P2/P3 que un analizador de hardware dedicado, directamente en código gestionado.

## Actualizaciones en vivo vs instantáneas bajo demanda

Hay dos formas de leer el reporte:

- **`OnAnalysisUpdated`** — se genera cada `StatisticsInterval` con una instantánea nueva, y una vez más al final del flujo con `IsFinal == true`. Las actualizaciones periódicas se ejecutan en un hilo de temporizador en segundo plano; el reporte final se ejecuta en el hilo de GStreamer. Marshalle al hilo de UI antes de tocar elementos de UI.
- **`GetReport()`** — devuelve la instantánea actual bajo demanda (o `null` si el bloque no se ha construido). No perturba la contabilidad periódica de bitrate instantáneo/de pico.

```csharp
analyzer.OnAnalysisUpdated += (sender, e) =>
{
    // e.IsFinal distingue las actualizaciones periódicas del reporte de final de flujo.
    Dispatcher.BeginInvoke(() => UpdateUI(e.Report));   // WPF: marshalle al hilo de UI
};
```

## Aplicaciones de ejemplo

- **WPF — TS Analyzer Demo:** una UI completa con una cuadrícula por PID, un panel de TR 101 290 y una línea de resumen (nombre de servicio, bitrate nulo/efectivo, nombre de red, UTC del flujo, sincronización A/V). [Examinar el código fuente](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/TS%20Analyzer%20Demo).
- **Consola — TS Analyzer CLI:** analiza un archivo o un feed `udp://` / `srt://` en vivo e imprime el reporte completo. [Examinar el código fuente](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/TS%20Analyzer%20CLI).

## Disponibilidad

Llame a `TSAnalyzerBlock.IsAvailable()` para verificar que el analizador está disponible en el entorno actual antes de crear una instancia.

## Plataformas

Windows, macOS, Linux, iOS, Android.

## Véase también

- [Análisis MPEG-TS en C#: VisioForge vs ffprobe](../Guides/mpeg-ts-analysis-vs-ffprobe.md) — cómo se compara el bloque con ffprobe y los analizadores de broadcast profesionales.
- [Analizar y validar un flujo MPEG-TS en C#](../Guides/mpeg-ts-stream-validation-csharp.md) — una guía de tareas paso a paso.
- [Grabación UDP MPEG-TS sin recodificar](../Guides/udp-mpegts-record-without-reencoding.md).
