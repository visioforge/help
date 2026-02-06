---
title: Guía de Despliegue de Media Player SDK .Net
description: Despliega aplicaciones de Media Player SDK con paquetes NuGet, dependencias de runtime, filtros DirectShow para Windows y entornos multiplataforma.
---

# Guía de Despliegue de Media Player SDK .Net

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta guía completa cubre todos los escenarios de despliegue para el Media Player SDK .Net, asegurando que tus aplicaciones funcionen correctamente en diferentes entornos. Ya sea que estés desarrollando aplicaciones multiplataforma o soluciones específicas de Windows, esta guía proporciona los pasos necesarios para un despliegue exitoso.

## Descripción General de Tipos de Motor

El Media Player SDK .Net ofrece dos tipos principales de motor, cada uno diseñado para escenarios de despliegue específicos:

### Motor MediaPlayerCoreX (Multiplataforma)

MediaPlayerCoreX es nuestra solución multiplataforma que funciona en múltiples sistemas operativos. Para instrucciones detalladas de despliegue específicas de este motor, consulta la [Guía de Despliegue Multiplataforma](../deployment-x/index.md) principal.

### Motor MediaPlayerCore (Solo Windows)

El motor MediaPlayerCore está optimizado específicamente para entornos Windows. Cuando despliegues aplicaciones que usan este motor en computadoras sin el SDK preinstalado, debes incluir los componentes necesarios del SDK con tu aplicación.

> **Importante**: Para aplicaciones AnyCPU, debes desplegar tanto los redistribuibles x86 como x64 para asegurar compatibilidad en diferentes arquitecturas de sistema.

## Opciones de Despliegue

Hay tres métodos principales para desplegar los componentes del Media Player SDK .Net:

1. Usando paquetes NuGet (recomendado para la mayoría de escenarios)
2. Usando instaladores silenciosos automáticos (requiere privilegios administrativos)
3. Instalación manual (para control completo sobre el proceso de despliegue)

## Despliegue con Paquetes NuGet

Los paquetes NuGet proporcionan el método de despliegue más simple, manejando automáticamente la inclusión de archivos necesarios en la carpeta de tu aplicación durante el proceso de compilación.

### Paquetes NuGet Requeridos

#### Paquetes Principales (Siempre Requeridos)

* **Paquete Base del SDK**:
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86/)
  * [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64/)
* **Paquete Media Player SDK**:
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x86/)
  * [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x64/)

#### Paquetes Específicos de Características (Añadir según sea necesario)

##### Soporte de Formatos Multimedia

* **Paquete FFMPEG** (para reproducción de archivos usando modo de fuente FFMPEG):
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)
  * [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)
* **Paquete de Salida MP4**:
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
  * [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
* **Paquete de Salida WebM**:
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86/)

##### Soporte de Fuentes

* **Paquete Fuente VLC** (para fuentes de archivo/cámara IP):
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86/)
  * [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64/)

##### Soporte de Formatos de Audio

* **Paquete Formatos XIPH** (salida/fuente Ogg, Vorbis, FLAC):
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86/)
  * [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64/)

##### Soporte de Filtros

* **Paquete Filtros LAV**:
  * [Versión x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/)
  * [Versión x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

## Instaladores Silenciosos Automáticos

Para escenarios donde prefieras despliegue basado en instaladores, el SDK ofrece instaladores silenciosos automáticos que requieren privilegios administrativos.

### Instaladores Disponibles

#### Componentes Principales

* **Paquete Base** (siempre requerido):
  * [Instalador x86](https://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe)
  * [Instalador x64](https://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)

#### Soporte de Formatos Multimedia

* **Paquete FFMPEG** (para fuentes de archivo/cámara IP):
  * [Instalador x86](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe)
  * [Instalador x64](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)

#### Soporte de Fuentes

* **Paquete Fuente VLC** (para fuentes de archivo/cámara IP):
  * [Instalador x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe)
  * [Instalador x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)

#### Soporte de Formatos de Audio

* **Paquete Formatos XIPH** (salida/fuente Ogg, Vorbis, FLAC):
  * [Instalador x86](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe)
  * [Instalador x64](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe)

#### Soporte de Filtros

* **Paquete Filtros LAV**:
  * [Instalador x86](https://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe)
  * [Instalador x64](https://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

> **Nota**: Para desinstalar cualquier paquete instalado, ejecuta el ejecutable con privilegios administrativos usando los parámetros: `/x //`

## Instalación Manual

Para escenarios de despliegue avanzados que requieren control preciso sobre la instalación de componentes, sigue estos pasos:

### Paso 1: Dependencias de Runtime

* **Con Privilegios Administrativos**: Instala el runtime VC++ 2022 (v143) (x86/x64) y las DLLs de runtime OpenMP usando ejecutables redistribuibles o módulos MSM.
* **Sin Privilegios Administrativos**: Copia las DLLs de runtime VC++ 2022 (v143) (x86/x64) y runtime OpenMP directamente a la carpeta de tu aplicación.

### Paso 2: Componentes Principales

* Copia las DLLs VisioForge_MFP/VisioForge_MFPX (o versiones x64) desde el directorio Redist\Filters a la carpeta de tu aplicación.

### Paso 3: Ensamblados .NET

* Ya sea copia los ensamblados .NET a la carpeta de tu aplicación o instálalos en el Caché de Ensamblados Global (GAC).

### Paso 4: Filtros DirectShow

* Copia y registra en COM los filtros DirectShow del SDK usando [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5) u otro método adecuado.

### Paso 5: Configuración del Entorno

* Añade la carpeta que contiene los filtros a la variable de entorno PATH del sistema si el ejecutable de tu aplicación está ubicado en un directorio diferente.

## Configuración de Filtros DirectShow

El SDK usa varios filtros DirectShow para funcionalidad específica. A continuación se presenta una lista completa organizada por categoría de característica:

### Filtros de Características Básicas

* VisioForge_Video_Effects_Pro.ax
* VisioForge_MP3_Splitter.ax
* VisioForge_H264_Decoder.ax
* VisioForge_Audio_Mixer.ax

### Filtros de Efectos de Audio

* VisioForge_Audio_Effects_4.ax (efectos de audio legacy)

### Filtros de Soporte de Streaming

#### Streaming RTSP

* VisioForge_RTSP_Sink.ax
* Filtros MP4 (legacy/modernos, excluyendo muxer)

#### Streaming SSF

* VisioForge_SSF_Muxer.ax
* Filtros MP4 (legacy/modernos, excluyendo muxer)

### Filtros de Fuente

#### Fuente VLC

* VisioForge_VLC_Source.ax
* Carpeta Redist\VLC completa con registro COM
* Variable de entorno VLC_PLUGIN_PATH apuntando a carpeta VLC\plugins

#### Fuente FFMPEG

* VisioForge_FFMPEG_Source.ax
* Carpeta Redist\FFMPEG completa, añadida a la variable PATH de Windows

#### Fuente de Memoria

* VisioForge_AsyncEx.ax

#### Decodificación WebM

* VisioForge_WebM_Ogg_Source.ax
* VisioForge_WebM_Source.ax
* VisioForge_WebM_Split.ax
* VisioForge_WebM_Vorbis_Decoder.ax
* VisioForge_WebM_VP8_Decoder.ax
* VisioForge_WebM_VP9_Decoder.ax

#### Fuentes de Streaming en Red

* VisioForge_RTSP_Source.ax
* VisioForge_RTSP_Source_Live555.ax
* Filtros FFMPEG, VLC o LAV

#### Fuentes de Formato de Audio

* VisioForge_Xiph_FLAC_Source.ax (fuente FLAC)
* VisioForge_Xiph_Ogg_Demux2.ax (fuente Ogg Vorbis)
* VisioForge_Xiph_Vorbis_Decoder.ax (fuente Ogg Vorbis)

### Filtros de Características Especiales

#### Encriptación de Video

* VisioForge_Encryptor_v8.ax
* VisioForge_Encryptor_v9.ax

#### Aceleración GPU

* VisioForge_DXP.dll / VisioForge_DXP64.dll (efectos de video GPU DirectX 11)

#### Fuente LAV

* Contenido completo de redist\LAV\x86(x64), con todos los archivos .ax registrados

### Consejo de Registro de Filtros

Para simplificar el proceso de registro COM para todos los filtros DirectShow en un directorio, coloca el archivo "reg_special.exe" del redistribuible del SDK en la carpeta de filtros y ejecútalo con privilegios administrativos.

---
Para más ejemplos de código y ejemplos, visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).