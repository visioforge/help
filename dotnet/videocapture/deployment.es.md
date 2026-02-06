---
title: Guía de Despliegue de Video Capture SDK .NET
description: Despliegue aplicaciones de Video Capture SDK con paquetes NuGet, instaladores silenciosos, o instalación manual para arquitecturas x86/x64 en .NET.
---

# Guía Completa de Despliegue para Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

Al desplegar Video Capture SDK .Net en sistemas sin el SDK preinstalado, el despliegue adecuado de componentes es esencial para la funcionalidad. Para aplicaciones AnyCPU, se deben desplegar tanto los redistribuibles x86 como x64 para asegurar compatibilidad a través de diferentes arquitecturas de sistema.

## Resumen de Opciones de Motor

### Motor VideoCaptureCoreX (Compatibilidad Multiplataforma)

Para escenarios de despliegue multiplataforma, consulte nuestra [guía completa de despliegue](../deployment-x/index.md) que detalla requisitos específicos de plataforma y opciones de configuración.

### Motor VideoCaptureCore (Plataforma Windows)

El motor VideoCaptureCore está optimizado específicamente para entornos Windows y ofrece múltiples enfoques de despliegue basados en los requisitos de su aplicación y restricciones del entorno objetivo.

## Métodos de Despliegue

### Distribución por Paquete NuGet (Sin Privilegios de Administrador)

El enfoque de paquete NuGet proporciona un método de despliegue simplificado que no requiere privilegios de administrador, haciéndolo ideal para entornos restringidos o al desplegar en múltiples sistemas sin acceso elevado.

Agregue los paquetes NuGet requeridos a su proyecto de aplicación, y después de compilar, los archivos redistribuibles necesarios se incluirán automáticamente en su carpeta de aplicación.

#### Paquetes NuGet Esenciales

**Componentes Principales (Requeridos):**

- Paquete Base del SDK: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64)
- Video Capture SDK: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64)

**Paquetes Específicos por Característica:**

- Integración FFMPEG (para salida de archivo/streaming de red): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64)
- Soporte de Salida MP4: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64)
- Integración de Fuente VLC (para fuentes de archivo/cámara IP): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64)
- Formato de Salida WebM: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86)
- Soporte de Formatos XIPH (Ogg, Vorbis, FLAC): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64)
- Filtros LAV: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64)
- Soporte de Cámara Virtual: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VirtualCamera.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VirtualCamera.x64)

> **Nota:** Al usar el paquete de Cámara Virtual, se requiere registro adicional de archivos de cámara como se describe en la sección de Instalación Manual si desea que la cámara virtual sea accesible desde aplicaciones externas.

### Despliegue con Instalador Silencioso (Privilegios de Administrador Requeridos)

Para escenarios donde el acceso de administrador está disponible, los instaladores silenciosos proporcionan un enfoque de despliegue simplificado que maneja el registro de componentes automáticamente.

**Componentes Principales:**

- Paquete Base (requerido): [x86](https://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)
- Ensamblados .NET: Pueden instalarse en la Caché Global de Ensamblados (GAC) o usarse desde una carpeta local

**Instaladores Específicos por Característica:**

- Integración FFMPEG: [x86](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)
- Soporte de Salida MP4: [x86](https://files.visioforge.com/redists_net/redist_dotnet_mp4_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_mp4_x64.exe)
- Integración de Fuente VLC: [x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)
- Soporte de Formatos Adicionales: WebM ([x86](https://files.visioforge.com/redists_net/redist_dotnet_webm_x86.exe)) y formatos XIPH ([x86](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe))
- Filtros LAV: [x86](https://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

> **Nota de Desinstalación:** Para remover el paquete, ejecute el ejecutable del instalador con privilegios de administrador usando el parámetro `/x //`.

### Proceso de Instalación Manual

Para control completo sobre el proceso de despliegue o en entornos con requisitos específicos, la instalación manual proporciona la mayor flexibilidad:

1. **Dependencias de Runtime:** Instale o copie el runtime VC++ 2022 (v143) (x86/x64) y los DLLs de runtime OpenMP. Con derechos de administrador, use el exe redist o módulos MSM; de lo contrario, copie directamente a la carpeta de la aplicación.

2. **Componentes Principales:** Copie los DLLs `VisioForge_MFP`/`VisioForge_MFPX` (o versiones x64) desde la carpeta `Redist\Filters` al directorio de su aplicación.

3. **Ensamblados .NET:** Ya sea copie los ensamblados a su carpeta de aplicación o instálelos en la GAC.

4. **Filtros DirectShow:** Copie los filtros DirectShow del SDK a su carpeta de aplicación o a una carpeta redist designada (configurada vía la propiedad `CustomRedist_Path`).

5. **Configuración:** Establezca la propiedad `CustomRedist_Enabled` a `true` en el evento Window Load.

6. **Manejo de Arquitectura:** Para filtros LAV (que usan nombres idénticos para versiones x64 y x86), use carpetas redist separadas para cada arquitectura.

7. **Configuración de Ruta:** Si el ejecutable de su aplicación reside en una ubicación diferente, agregue la carpeta de filtros a la variable de entorno `PATH` del sistema.

#### Componentes Principales

**Características Básicas:**

- Filtros Base: VisioForge_BaseFilters.ax / VisioForge_BaseFilters_x64.ax
- Efectos de Video: VisioForge_Video_Effects_Pro.ax / VisioForge_Video_Effects_Pro_x64.ax
- Procesamiento de Audio: VisioForge_MP3_Splitter.ax / VisioForge_MP3_Splitter_x64.ax, VisioForge_Audio_Mixer.ax / VisioForge_Audio_Mixer_x64.ax

#### Componentes Específicos por Formato

**Salida MP3:**

- VisioForge_LAME.ax / VisioForge_LAME_x64.ax

**Salida MP4/M4A:**

- Versión Legacy: VisioForge_AAC_Encoder.ax, VisioForge_H264_Encoder_XP.ax, VisioForge_MP4_Muxer.ax con librerías de soporte
- Versión 10: VisioForge_AAC_Encoder_v10.ax, VisioForge_H264_Encoder.ax, VisioForge_MP4_Muxer_v10.ax con librerías de soporte
- Versión 11/Codificación HW: VisioForge_MFT.dll, VisioForge_MF_Mux.ax (con variantes x64)

**Salida WebM:**

- Muxer: VisioForge_WebM_Mux.ax / VisioForge_WebM_Mux_x64.ax
- Codificadores: VisioForge_WebM_Vorbis_Encoder.ax, VisioForge_WebM_VP8_Encoder.ax
- Mejora de Audio: VisioForge_Audio_Enhancer.ax / VisioForge_Audio_Enhancer_x64.ax

**Soporte Ogg/FLAC:**

- FLAC: VisioForge_Xiph_FLAC_Encoder.ax / VisioForge_Xiph_FLAC_Encoder_x64.ax
- Ogg Vorbis: VisioForge_Xiph_Ogg_Mux.ax, VisioForge_Xiph_Vorbis_Encoder.ax (con variantes x64)

#### Componentes de Streaming y Fuente

**Streaming RTSP:**

- VisioForge_RTSP_Sink.ax / VisioForge_RTSP_Sink_x64.ax
- Filtros MP4 (excluyendo Muxer)

**Integración de Fuente VLC:**

- VisioForge_VLC_Source.ax / VisioForge_VLC_Source_x64.ax
- Requiere copiar todos los archivos de la carpeta Redist\VLC, registro COM, y configuración apropiada de variables de entorno

**Integración FFMPEG:**

- VisioForge_FFMPEG_Source.ax / VisioForge_FFMPEG_Source_x64.ax
- Requiere todos los archivos de la carpeta Redist\FFMPEG y actualizaciones de la variable PATH

#### Componentes Especializados

**Captura de Pantalla:**

- VisioForge_Screen_Capture_DD.ax / VisioForge_Screen_Capture_DD_x64.ax

**Captura de Audio:**

- VisioForge_WhatYouHear_Source.ax / VisioForge_WhatYouHear_Source_x64.ax

**Cámara Virtual:**

- VisioForge_Virtual_Camera.ax / VisioForge_Virtual_Camera_x64.ax
- VisioForge_Virtual_Audio_Card.ax / VisioForge_Virtual_Audio_Card_x64.ax

**Procesamiento de Video:**

- Push Source: VisioForge_Push_Video_Source.ax / VisioForge_Push_Video_Source_x64.ax
- Streaming de Red: VisioForge_Network_Streamer_Audio.ax, VisioForge_Network_Streamer_Video.ax
- Encriptación de Video: Múltiples componentes incluyendo Desencriptadores, Codificadores, y librerías de soporte
- Picture-In-Picture: VisioForge_Video_Mixer.ax / VisioForge_Video_Mixer_x64.ax

#### Registro de Filtros

Para el registro COM de todos los filtros DirectShow en una carpeta específica, puede desplegar la utilidad `reg_special.exe` del SDK al directorio de filtros y ejecutarla con privilegios de administrador para automatizar el proceso de registro.
