---
title: "Análisis MPEG-TS en C# .NET: VisioForge frente a ffprobe"
description: Análisis MPEG-TS en proceso en .NET con monitorización ETSI TR 101 290 en vivo — cómo se compara el TS Analyzer de Media Blocks SDK con ffprobe.
sidebar_label: Análisis TS vs ffprobe
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - Analysis
  - TR 101 290
  - ffprobe
  - Streaming
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerReport
  - TSAnalyzerSettings

---

# Análisis MPEG-TS en C#: VisioForge vs ffprobe

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Por qué esto importa

ffprobe es una excelente herramienta de línea de comandos para echar un vistazo rápido a un archivo multimedia. Pero en cuanto necesita análisis de flujo de transporte **dentro de una aplicación .NET** — un panel de monitorización en vivo, un validador de ingesta, una compuerta de control de calidad automatizada — el modelo de CLI empieza a jugar en su contra: tiene que lanzar un proceso separado, capturar su salida de texto o JSON, analizarla, y aun así solo obtiene una instantánea única sin marco de conformidad.

El `TSAnalyzerBlock` del [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) está creado exactamente para ese trabajo. Es un **monitor MPEG-TS en proceso, en vivo y de calidad broadcast** que reporta resultados fuertemente tipados de forma continua, con conformidad completa con ETSI **TR 101 290** Prioridad 1/2/3 — algo que ffprobe sencillamente no proporciona.

## Qué distingue al TS Analyzer

### 1. En proceso, fuertemente tipado — sin CLI, sin análisis de texto

Conecta un bloque de fuente al analizador y lee un objeto `TSAnalyzerReport`. Sin `Process.Start`, sin captura de stdout, sin análisis frágil de cadenas/JSON, sin formato de salida dependiente de la versión que rastrear. Cada métrica es una propiedad tipada que puede enlazar, registrar o validar directamente.

### 2. En vivo y orientado a eventos

ffprobe es de un solo disparo: se ejecuta, imprime, termina. El `TSAnalyzerBlock` genera `OnAnalysisUpdated` con una cadencia configurable (cada segundo por defecto) durante toda la vida del flujo — perfecto para paneles en tiempo real, detección de deriva y alertas. Se entrega un reporte final al terminar el flujo.

### 3. TR 101 290 de calidad broadcast — integrado

El analizador evalúa la lista completa de comprobaciones TR 101 290 Prioridad 1/2/3 (pérdida de sincronización, errores de PAT/PMT/PID, errores de contador de continuidad, errores de transporte, errores de CRC, errores de PCR/PTS, errores de SDT/EIT/TDT/NIT) y devuelve cada comprobación con su prioridad, conteo de errores y estado de aprobado/fallido. ffprobe no tiene un marco de conformidad equivalente — tendría que construir uno usted mismo sobre su salida.

### 4. Modo passthrough — analice mientras graba o retransmite

En modo `InputOutput` el flujo de transporte original se reenvía sin alterar al siguiente bloque, de modo que puede validar un feed **al mismo tiempo** que lo graba o retransmite — cero remultiplexado, un solo pipeline. ffprobe se sitúa por completo fuera de su ruta multimedia.

### 5. Una API, cualquier fuente, todas las plataformas

Archivo, UDP (unicast y multicast) y SRT alimentan el mismo analizador a través de la misma API, en Windows, macOS, Linux, iOS y Android.

## Comparación de funcionalidades

La matriz siguiente refleja las capacidades **actuales** del `TSAnalyzerBlock`. ✅ = compatible, ⚠️ = parcial/indirecto, ❌ = no disponible.

| Capacidad | VisioForge TS Analyzer | ffprobe |
| --- | :---: | :---: |
| Autodetección de tamaño de paquete 188/192 | ✅ | ✅ |
| PAT/PMT, programas, PID de PCR | ✅ | ✅ |
| stream_type → códec | ✅ | ✅ |
| Bitrate por PID + conteo de paquetes | ✅ | ⚠️ |
| Bitrate instantáneo + de pico por PID | ✅ | ❌ |
| Errores de contador de continuidad | ✅ | ⚠️ |
| Intervalo PCR mín/prom/máx + discontinuidades | ✅ | ❌ |
| Jitter PCR (máx) + repetición >40 ms | ✅ | ❌ |
| Precisión / deriva PCR (ppm) | ⚠️ | ❌ |
| Indicador de error de transporte | ✅ | ⚠️ |
| % nulo / relleno / bitrate efectivo | ✅ | ❌ |
| Cifrado (TSC + free_CA_mode) | ✅ | ⚠️ |
| Presencia de PTS/DTS + desfase de sincronización A/V | ✅ | ✅ |
| SDT → nombre / proveedor / tipo de servicio | ✅ | ⚠️ |
| Idioma de audio (ISO 639, PMT) | ✅ | ✅ |
| Nombre de red NIT; UTC del flujo TDT/TOT | ✅ | ⚠️ |
| EIT → eventos EPG | ✅ (opcional) | ⚠️ |
| Detección de PID no referenciado | ✅ | ❌ |
| Resolución / perfil / nivel / croma / aspecto de códec | ✅ (H.264/HEVC/MPEG-2) | ✅ |
| Validación CRC-32 de PSI | ✅ | ⚠️ |
| **TR 101 290 P1/P2/P3 estructurado** | ✅ | ❌ |
| **API .NET en proceso (sin CLI / análisis)** | ✅ | ❌ |
| **Monitorización continua en vivo (eventos)** | ✅ | ❌ |
| **Passthrough (analizar mientras se graba/retransmite)** | ✅ | ❌ |
| Multiplataforma (Windows/macOS/Linux/móvil) | ✅ | ✅ |

Algunas notas honestas para que la tabla siga siendo fiable. La cobertura de códecs es por atributo, no uniforme: la **resolución** se analiza para H.264, HEVC y MPEG-2; el **perfil, nivel y croma** solo para H.264/HEVC; la **relación de aspecto y la tasa de fotogramas** desde la cabecera de secuencia MPEG-1/2 (todavía no desde el VUI de H.264/HEVC). El campo de **deriva del reloj PCR en ppm** está reservado y todavía no se calcula (el jitter y la repetición sí). ffprobe también sigue siendo excelente en metadatos exhaustivos por códec y admite una gama mucho más amplia de formatos de contenedor/códec más allá de MPEG-TS.

## Cuándo usar cada uno

- **Opte por ffprobe** cuando quiera una comprobación rápida en terminal de un archivo multimedia arbitrario, o cuando esté escribiendo un script puntual en un shell y el formato pueda no ser MPEG-TS.
- **Opte por el `TSAnalyzerBlock`** cuando el análisis de flujo de transporte deba residir **dentro de su aplicación .NET** — monitorización continua, compuertas de conformidad TR 101 290, paneles/alertas en vivo, o analizar un feed mientras lo graba o retransmite simultáneamente.

No son mutuamente excluyentes: muchos equipos usan ffprobe en el escritorio y entregan VisioForge en el producto.

## Próximos pasos

- [Bloque Analizador de Flujos MPEG-TS](../Special/TSAnalyzerBlock.md) — la referencia completa del bloque: modos, configuración y el modelo de reporte completo.
- [Analizar y validar un flujo MPEG-TS en C#](mpeg-ts-stream-validation-csharp.md) — una guía de tareas paso a paso.
