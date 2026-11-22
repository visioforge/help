---
title: Preguntas frecuentes de filtros DirectShow — MP4 y HEVC
description: Respuestas sobre los filtros DirectShow de VisioForge — fallos del multiplexor MP4 en grafos DES y los codificadores H.265 que usa MF Mux.
sidebar_label: FAQ
tags:
  - DirectShow
  - C++
  - .NET
  - Windows
  - Encoding
  - MP4
  - H.264
  - HEVC
  - Troubleshooting

---

# Preguntas frecuentes de los filtros DirectShow

Respuestas a las preguntas que hacen los desarrolladores que usan los filtros DirectShow de VisioForge directamente en sus propios grafos. Para los SDK .NET construidos sobre estos filtros, consulte [Solución de problemas](../dotnet/general/troubleshooting.md).

## Codificación y multiplexación

### El multiplexor MP4 falla, o el grafo falla con "Attempted to divide by zero"

Tanto el multiplexor MP4 v10 como el filtro MF Mux toman el tamaño de fotograma y la velocidad de fotogramas del tipo de medio de la conexión de video. Un tipo de medio sin esos datos (habitual en los grafos construidos por DirectShow Editing Services (DES) o por filtros de origen propios) puede hacer que el codificador no arranque, que `IMediaControl.Run()` lance un error de división por cero o que el multiplexor falle de forma intermitente.

Establézcalos explícitamente en el tipo de medio de video que conecta al codificador o al multiplexor, o en el grupo de DES:

- `biWidth` y `biHeight` en el `BITMAPINFOHEADER`;
- `AvgTimePerFrame` en el `VIDEOINFOHEADER` (en unidades de 100 ns, por ejemplo `400000` para 25 fps).

Un grafo completo en C# con el multiplexor MP4 v10 está en la [MP4 v10 Output Demo](https://github.com/visioforge/directshow-samples/tree/main/Encoding%20Filters%20Pack/dotnet/MP4%20v10%20Output%20Demo). Si el problema continúa con tipos de medio correctos, compare con la demostración y envíe al soporte un grafo pequeño que lo reproduzca.

### ¿Qué codificadores H.265 puede usar MF Mux?

MF Mux no tiene un codificador HEVC propio. Codifica H.265 con los codificadores de Media Foundation instalados en el PC:

- **Codificadores de GPU**: Intel Quick Sync, NVIDIA NVENC y AMD AMF admiten HEVC en la mayoría de las GPU lanzadas en los últimos años.
- **El codificador H.265 de Microsoft**: se instala con el paquete *HEVC Video Extensions* de Microsoft Store, que no está disponible en Windows Server.

Cuando ninguno de ellos puede codificar en un PC, el codificador no arranca para HEVC y la salida de depuración muestra un mensaje como `Unable to init NVENC H265 encoder.` (o su forma para Intel QSV y AMD), o `MS HEVC encoder not found.` para el codificador de Microsoft; codifique en H.264 en ese equipo. En MF Mux, los codificadores H.264 y H.265 de Microsoft admiten video de hasta 1920 píxeles de ancho y 1080 píxeles de alto; para un video mayor, use un codificador de GPU (NVENC, Quick Sync, AMF). H.264 está disponible en todas las ediciones de Windows con los componentes multimedia, por hardware o por software; las ediciones N y KN de Windows necesitan antes el Media Feature Pack, y Windows Server necesita la característica Media Foundation. La compatibilidad con H.265 en MF Mux requiere una versión actual del filtro: las compilaciones antiguas (la versión 11 de 2019, por ejemplo) no la tienen. Los filtros actuales forman parte del [Encoding Filters Pack](filters-enc/index.md).
