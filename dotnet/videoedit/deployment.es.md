---
title: Guía de Despliegue de Video Edit SDK .Net | Windows
description: Despliega Video Edit SDK .Net en Windows con paquetes NuGet, instaladores silenciosos y configuraciones manuales para arquitecturas x86 y x64.
---

# Guía Completa de Despliegue para Video Edit SDK .Net

## Introducción al Despliegue de VideoEditCore

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

El SDK de Video Edit de VisioForge para .Net proporciona un conjunto poderoso de herramientas para procesamiento de video, edición y análisis en entornos Windows. Esta guía completa detalla las opciones de despliegue para asegurar que el SDK funcione correctamente en los sistemas de destino.

Para aplicaciones construidas con la configuración AnyCPU, debes desplegar tanto los redistribuibles x86 como x64 para asegurar compatibilidad en diferentes arquitecturas de procesador. Esta guía cubre todos los métodos de despliegue, desde simples paquetes NuGet hasta instalaciones manuales detalladas.

## Descripción General de Opciones de Despliegue

El SDK ofrece tres enfoques principales de despliegue:

1. **Paquetes NuGet**: Método más simple que no requiere privilegios administrativos
2. **Instaladores Automáticos**: Instalación silenciosa con derechos administrativos
3. **Instalación Manual**: Despliegue personalizado con control granular sobre los componentes

Cada enfoque tiene ventajas distintas dependiendo de los requisitos de tu aplicación, método de distribución y restricciones del entorno de destino.

## Despliegue Multiplataforma con VideoEditCoreX

Para desarrolladores que buscan compatibilidad multiplataforma, VisioForge ofrece el motor VideoEditCoreX. Esta implementación moderna soporta entornos Windows, macOS y Linux.

Para instrucciones detalladas sobre el despliegue de la versión multiplataforma, por favor consulta nuestra [guía de despliegue multiplataforma](../deployment-x/index.md) dedicada. El resto de este documento se enfoca en el motor VideoEditCore específico de Windows.

## Motor VideoEditCore (Solo Windows)

El motor VideoEditCore específico de Windows proporciona capacidades extensas de edición de video optimizadas para entornos Windows. A continuación se presentan las opciones de despliegue completas disponibles.

### Despliegue con Paquetes NuGet (Sin Derechos Administrativos Requeridos)

Los paquetes NuGet ofrecen el método de despliegue más simple, sin requerir privilegios administrativos en el sistema de destino. Este enfoque copia automáticamente los archivos necesarios a la carpeta de tu aplicación durante el proceso de compilación.

#### Paquetes NuGet Requeridos

**Componentes Base (Siempre Requeridos)**:

- Paquete Base del SDK: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64/)
- Paquete Video Edit SDK: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

**Componentes Específicos de Formato**:

- Salida MP4: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
- Salida WebM: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86/)
- Formatos XIPH (Ogg, Vorbis, FLAC): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64/)

**Componentes de Fuente Multimedia**:

- FFMPEG (Salida de archivo/streaming en red): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)
- Fuente VLC (Archivo/Cámara IP): [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64/)
- Filtros LAV: [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

La implementación es directa: añade los paquetes requeridos a tu proyecto de aplicación, y después de compilar, los archivos redistribuibles necesarios se incluirán automáticamente en la carpeta de tu aplicación.

### Instaladores Silenciosos Automáticos (Derechos Administrativos Requeridos)

Para escenarios donde los derechos administrativos están disponibles, los instaladores silenciosos proporcionan una solución de despliegue simplificada. Estos instaladores pueden integrarse en el proceso de configuración de tu aplicación para un despliegue fluido del SDK.

**Componentes Base**:

- Paquete Base (Siempre Requerido): [x86](https://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)

**Componentes de Fuente Multimedia**:

- Paquete FFMPEG: [x86](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)
- Paquete Fuente VLC: [x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)
- Filtros LAV: [x86](https://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

**Componentes Específicos de Formato**:

- Formatos XIPH (Ogg, Vorbis, FLAC): [x86](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe)

**Instalación y Desinstalación**:

- Para instalar: Ejecuta el ejecutable apropiado con privilegios administrativos
- Para desinstalar: Ejecuta el ejecutable con privilegios administrativos y los parámetros "/x //"
- Los ensamblados .NET pueden instalarse en el Caché de Ensamblados Global (GAC) o usarse directamente desde una carpeta local

### Instalación Manual (Avanzada)

La instalación manual proporciona el mayor nivel de control sobre el proceso de despliegue. Este enfoque se recomienda para escenarios avanzados donde componentes específicos deben personalizarse o para entornos de despliegue con restricciones únicas.

#### Proceso de Instalación Manual Paso a Paso

1. **Dependencias de Runtime**:
   - Para aplicaciones con privilegios administrativos: Instala VC++ 2022 (v143) runtime (x86/x64) y DLLs de runtime OpenMP usando redistribuibles ejecutables o módulos MSM
   - Para aplicaciones sin privilegios administrativos: Copia las DLLs de VC++ 2022 (v143) runtime (x86/x64) y runtime OpenMP directamente a la carpeta de la aplicación

2. **Componentes Principales**:
   - Copia las DLLs VisioForge_MFP/VisioForge_MFPX (o versiones x64) desde Redist\Filters a la carpeta de tu aplicación
   - Copia los ensamblados .NET a la carpeta de la aplicación o instálalos en el Caché de Ensamblados Global (GAC)

3. **Filtros DirectShow**:
   - Copia y registra en COM los filtros DirectShow del SDK usando [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5) o un método equivalente
   - Si el ejecutable de tu aplicación está en una carpeta diferente, añade la carpeta que contiene los filtros a la variable de entorno PATH del sistema

## Referencia de Filtros DirectShow Esenciales

### Filtros de Funcionalidad Principal

**Procesamiento de Video Básico**:

- VisioForge_Video_Effects_Pro.ax - Procesamiento de efectos de video principales
- VisioForge_Audio_Mixer.ax - Mezcla y procesamiento de audio
- VisioForge_MP3_Splitter.ax - Manejo de formato MP3
- VisioForge_H264_Decoder.ax - Decodificación de video H.264

**Procesamiento de Audio**:

- VisioForge_Audio_Effects_4.ax - Procesamiento de efectos de audio legacy

### Filtros de Streaming

**Streaming RTSP**:

- VisioForge_RTSP_Sink.ax - Salida de streaming RTSP
- Todos los filtros MP4 (legacy/modernos) excepto Muxer

**Streaming SSF**:

- VisioForge_SSF_Muxer.ax - Multiplexor de formato SSF
- Todos los filtros MP4 (legacy/modernos) excepto Muxer

**Fuentes RTSP/RTMP/HTTP**:

- VisioForge_RTSP_Source.ax - Entrada de stream RTSP
- VisioForge_RTSP_Source_Live555.ax - RTSP con biblioteca Live555
- VisioForge_IP_HTTP_Source.ax - Entrada de fuente HTTP
- Filtros FFMPEG, VLC o LAV según sea necesario

### Filtros de Fuente Multimedia

**Fuente VLC**:

- VisioForge_VLC_Source.ax - Entrada multimedia basada en VLC
- El despliegue completo requiere:
  - Copiar todos los archivos de la carpeta Redist\VLC
  - Registro COM de archivos .ax
  - Añadir variable de entorno VLC_PLUGIN_PATH apuntando a la carpeta VLC\plugins

**Fuente FFMPEG**:

- VisioForge_FFMPEG_Source.ax - Entrada multimedia basada en FFMPEG
- Copia todos los archivos de la carpeta Redist\FFMPEG y añade al PATH de Windows

**Fuente de Memoria**:

- VisioForge_AsyncEx.ax - Entrada de fuente basada en memoria

**Fuente LAV**:

- Copia todos los archivos de Redist\LAV\x86(x64)
- Registra todos los archivos .ax

### Filtros Específicos de Formato

**Decodificación WebM**:

- VisioForge_WebM_Ogg_Source.ax - Soporte de contenedor WebM/Ogg
- VisioForge_WebM_Source.ax - Fuente de formato WebM
- VisioForge_WebM_Split.ax - Demuxing de WebM
- VisioForge_WebM_Vorbis_Decoder.ax - Decodificador de audio Vorbis
- VisioForge_WebM_VP8_Decoder.ax - Decodificador de video VP8
- VisioForge_WebM_VP9_Decoder.ax - Decodificador de video VP9

**Fuente FLAC**:

- VisioForge_Xiph_FLAC_Source.ax - Soporte de formato de audio FLAC

**Fuente Ogg Vorbis**:

- VisioForge_Xiph_Ogg_Demux2.ax - Demuxer de contenedor Ogg
- VisioForge_Xiph_Vorbis_Decoder.ax - Decodificador de audio Vorbis

### Filtros de Funcionalidad Avanzada

**Encriptación de Video**:

- VisioForge_Encryptor_v8.ax - Encriptación versión 8
- VisioForge_Encryptor_v9.ax - Encriptación versión 9

**Aceleración GPU**:

- VisioForge_DXP.dll / VisioForge_DXP64.dll - Efectos GPU DirectX 11

### Registro Simplificado de Filtros

Para el registro conveniente de múltiples filtros DirectShow, coloca la utilidad `reg_special.exe` del redistribuible del SDK en la carpeta que contiene los filtros y ejecútala con privilegios de administrador.

## Recursos Adicionales

Para ejemplos de código y ejemplos de implementación, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

Para soporte técnico, actualizaciones de documentación y discusiones de la comunidad, visita el [Portal de Desarrolladores de VisioForge](https://support.visioforge.com/).
