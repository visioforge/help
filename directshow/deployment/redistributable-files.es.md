---
title: Archivos redistribuibles de filtros DirectShow de VisioForge
description: Lista completa de archivos redistribuibles para SDKs DirectShow de VisioForge con dependencias, archivos de arquitectura y requisitos de despliegue.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - Decoding
  - Mixing
  - Screen Capture
  - WebM
  - H.264
  - MP3

---

# SDKs DirectShow - Referencia de Archivos Redistribuibles

## Descripción General

Este documento proporciona una lista completa de archivos requeridos para redistribuir cada SDK DirectShow con su aplicación. Todos los archivos deben incluirse en su instalador o paquete de despliegue.

---
## Filtro FFMPEG Source
### Archivos Principales
#### x86 (32-bit)
**Filtro**:
- `VisioForge_FFMPEG_Source.ax` - Filtro DirectShow principal
**Bibliotecas FFmpeg** (requeridas):
- `avcodec-58.dll` - Biblioteca de códec de video/audio
- `avdevice-58.dll` - Manejo de dispositivos
- `avfilter-7.dll` - Filtrado de audio/video
- `avformat-58.dll` - Manejo de formatos de contenedor
- `avutil-56.dll` - Funciones de utilidad
- `swresample-3.dll` - Remuestreo de audio
- `swscale-5.dll` - Escalado de video y conversión de color
**Tamaño Total**: ~80-100 MB
#### x64 (64-bit)
**Filtro**:
- `VisioForge_FFMPEG_Source_x64.ax` - Filtro DirectShow principal (64-bit)
**Bibliotecas FFmpeg** (requeridas):
- `avcodec-58.dll` - Versión 64-bit
- `avdevice-58.dll` - Versión 64-bit
- `avfilter-7.dll` - Versión 64-bit
- `avformat-58.dll` - Versión 64-bit
- `avutil-56.dll` - Versión 64-bit
- `swresample-3.dll` - Versión 64-bit
- `swscale-5.dll` - Versión 64-bit
**Tamaño Total**: ~90-110 MB
### Estructura del Directorio de Instalación
```
SuApp\
├── VisioForge_FFMPEG_Source.ax          (x86)
├── VisioForge_FFMPEG_Source_x64.ax      (x64)
├── avcodec-58.dll
├── avdevice-58.dll
├── avfilter-7.dll
├── avformat-58.dll
├── avutil-56.dll
├── swresample-3.dll
└── swscale-5.dll
```
### Archivos de Licencia
- `license.rtf` - Acuerdo de licencia del SDK (incluir en instalador)
### Dependencias
- **Visual C++ Redistributable 2015-2022** (x86 o x64)
  - Descargar: https://aka.ms/vs/17/release/vc_redist.x64.exe
---

## Filtro VLC Source

### Archivos Principales

#### Solo x86 (32-bit)

**Filtro**:
- `VisioForge_VLC_Source.ax` - Filtro DirectShow principal

**Bibliotecas VLC** (requeridas):
- `libvlc.dll` - Biblioteca principal VLC
- `libvlccore.dll` - Funcionalidad principal VLC

**Directorio de Plugins VLC** (requerido):
- `plugins\` - Carpeta completa de plugins VLC (~100+ DLLs de plugins)
  - `plugins\access\` - Protocolos de entrada
  - `plugins\audio_filter\` - Procesamiento de audio
  - `plugins\audio_mixer\` - Mezcla de audio
  - `plugins\audio_output\` - Salida de audio
  - `plugins\codec\` - Códecs
  - `plugins\control\` - Interfaces de control
  - `plugins\demux\` - Demultiplexores
  - `plugins\misc\` - Misceláneos
  - `plugins\packetizer\` - Empaquetadores
  - `plugins\services_discovery\` - Descubrimiento de servicios
  - `plugins\stream_filter\` - Filtros de stream
  - `plugins\stream_out\` - Salida de stream
  - `plugins\text_renderer\` - Renderizado de texto
  - `plugins\video_chroma\` - Conversión de color
  - `plugins\video_filter\` - Filtros de video
  - `plugins\video_output\` - Salida de video
  - `plugins\visualization\` - Visualizaciones

**Directorios de Datos VLC**:
- `locale\` - Archivos de localización (opcional, ~50+ carpetas de idiomas)
- `lua\` - Scripts Lua para listas de reproducción y extensiones
- `hrtfs\` - Archivos de audio HRTF
  - `dodeca_and_7channel_3DSL_HRTF.sofa`

**Tamaño Total**: ~150-200 MB (con todos los plugins y locales)

### Estructura del Directorio de Instalación

```
SuApp\
├── VisioForge_VLC_Source.ax
├── libvlc.dll
├── libvlccore.dll
├── plugins\
│   ├── access\
│   ├── audio_filter\
│   ├── codec\
│   └── ... (todos los directorios de plugins)
├── locale\           (opcional)
├── lua\
└── hrtfs\
```

### Archivos de Licencia

- `license.rtf` - Acuerdo de licencia del SDK

### Dependencias

- **Visual C++ Redistributable 2015-2022** (x86)

### Notas Importantes

- **Todos los plugins VLC deben incluirse** - Los plugins faltantes causarán fallos de reproducción para ciertos formatos
- **Mantener estructura de directorios** - VLC espera plugins en subdirectorio `plugins\`
- **Sin versión x64** - El Filtro VLC Source es solo 32-bit

---
## Paquete de Filtros de Procesamiento
### Filtros Principales
#### x86 (32-bit)
**Procesamiento de Video**:
- `VisioForge_Video_Effects_Pro.ax` - Filtro de efectos de video (35+ efectos)
- `VisioForge_Video_Mixer.ax` - Mezclador de video multi-fuente
- `VisioForge_Screen_Capture_DD.ax` - Captura de pantalla DirectDraw
**Procesamiento de Audio**:
- `VisioForge_Audio_Enhancer.ax` - Filtro de mejora de audio
- `VisioForge_Audio_Effects_4.ax` - Efectos de audio (opcional)
- `VisioForge_Audio_Mixer.ax` - Mezclador de audio
**Filtros Base** (requeridos):
- `VisioForge_BaseFilters.ax` - Biblioteca de filtros base principal
- `VisioForge_AsyncEx.ax` - Lector de archivos asíncrono (opcional)
**Bibliotecas Auxiliares**:
- `VisioForge_MFP.dll` - Auxiliar Media Foundation
- `VisioForge_MFPX.dll` - Funciones MF extendidas
#### x64 (64-bit)
**Procesamiento de Video**:
- `VisioForge_Video_Effects_Pro_x64.ax`
- `VisioForge_Video_Mixer_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Procesamiento de Audio**:
- `VisioForge_Audio_Enhancer_x64.ax`
- `VisioForge_Audio_Mixer_x64.ax`
**Filtros Base** (requeridos):
- `VisioForge_BaseFilters_x64.ax`
- `VisioForge_AsyncEx_x64.ax` (opcional)
**Bibliotecas Auxiliares**:
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### Filtros LAV (Opcional pero Recomendado)
Los Filtros LAV proporcionan soporte adicional de códecs y se incluyen con el Paquete de Filtros de Procesamiento.
#### x86
**Filtros LAV**:
- `LAVSplitter.ax` - Splitter de fuente
- `LAVVideo.ax` - Decodificador de video
- `LAVAudio.ax` - Decodificador de audio
**Bibliotecas FFmpeg para LAV**:
- `avcodec-lav-58.dll`
- `avformat-lav-58.dll`
- `avfilter-lav-7.dll`
- `avresample-lav-4.dll`
- `avutil-lav-56.dll`
- `swscale-lav-5.dll`
**Bibliotecas Adicionales**:
- `libbluray.dll` - Soporte Blu-ray
- `IntelQuickSyncDecoder.dll` - Decodificación por hardware Intel QuickSync
**Manifiesto**:
- `LAVFilters.Dependencies.manifest`
**Licencia**:
- `COPYING` - Licencia de Filtros LAV (LGPL)
#### x64
Mismos archivos que x86 pero versiones de 64-bit.
### Tamaño Total
- **Solo Filtros Principales**: ~20-30 MB
- **Con Filtros LAV**: ~80-100 MB
---

## Paquete de Filtros de Codificación

### Filtros Principales

#### x86 (32-bit)

**Codificadores de Video**:
- `VisioForge_NVENC.ax` - Codificador hardware NVIDIA
- `VisioForge_H264_Encoder.ax` - Codificador software H.264
- `VisioForge_H264_Encoder_v9.ax` - Codificador H.264 v9
- `VisioForge_H264_Decoder.ax` - Decodificador H.264
- `VisioForge_WebM_VP8_Encoder.ax` - Codificador VP8
- `VisioForge_WebM_VP9_Encoder.ax` - Codificador VP9 (en x64)
- `VisioForge_WebM_VP8_Decoder.ax` - Decodificador VP8
- `VisioForge_WebM_VP9_Decoder.ax` - Decodificador VP9

**Codificadores de Audio**:
- `VisioForge_AAC_Encoder.ax` - Codificador AAC
- `VisioForge_AAC_Encoder_v10.ax` - Codificador AAC v10
- `VisioForge_LAME.ax` - Codificador MP3 (LAME)
- `VisioForge_WebM_Vorbis_Encoder.ax` - Codificador Vorbis
- `VisioForge_WebM_Vorbis_Decoder.ax` - Decodificador Vorbis

**Muxers/Demuxers**:
- `VisioForge_MP4_Muxer.ax` - Muxer de contenedor MP4
- `VisioForge_MP4_Muxer_v10.ax` - Muxer MP4 v10
- `VisioForge_MF_Mux.ax` - Muxer Media Foundation
- `VisioForge_WebM_Mux.ax` - Muxer WebM
- `VisioForge_WebM_Split.ax` - Splitter WebM
- `VisioForge_WebM_Source.ax` - Fuente WebM
- `VisioForge_WebM_Ogg_Source.ax` - Fuente Ogg
- `VisioForge_SSF_Muxer.ax` - Muxer SSF

**Red**:
- `VisioForge_RTSP_Sink.ax` - Sink RTSP
- `VisioForge_RTSP_Source_Live555.ax` - Fuente RTSP

**Filtros Base** (requeridos):
- `VisioForge_BaseFilters.ax`

**Bibliotecas Auxiliares** (requeridas):
- `VisioForge_MFP.dll` - Auxiliar Media Foundation
- `VisioForge_MFP64.dll` - Auxiliar MF 64-bit
- `VisioForge_MFPX.dll` - Funciones MF extendidas
- `VisioForge_MFPX64.dll` - MF extendido 64-bit
- `VisioForge_MFT.dll` - Media Foundation Transform

**Intel QuickSync** (opcional):
- `libmfxsw32.dll` - Biblioteca software QuickSync
- `libmfxxp32.dll` - Biblioteca XP QuickSync

#### x64 (64-bit)

**Codificadores de Video**:
- `VisioForge_NVENC_x64.ax`
- `VisioForge_H264_Encoder_x64.ax`
- `VisioForge_H264_Encoder_v9_x64.ax`
- `VisioForge_H264_Decoder_x64.ax`
- `VisioForge_WebM_VP8_Encoder_x64.ax`
- `VisioForge_WebM_VP9_Encoder_x64.ax`
- `VisioForge_WebM_VP8_Decoder_x64.ax`
- `VisioForge_WebM_VP9_Decoder_x64.ax`

**Codificadores de Audio**:
- `VisioForge_AAC_Encoder_x64.ax`
- `VisioForge_AAC_Encoder_v10_x64.ax`
- `VisioForge_LAME_x64.ax`
- `VisioForge_WebM_Vorbis_Encoder_x64.ax`
- `VisioForge_WebM_Vorbis_Decoder_x64.ax`

**Muxers/Demuxers**:
- `VisioForge_MP4_Muxer_x64.ax`
- `VisioForge_MP4_Muxer_v10_x64.ax`
- `VisioForge_MF_Mux_x64.ax`
- `VisioForge_WebM_Mux_x64.ax`
- `VisioForge_WebM_Split_x64.ax`
- `VisioForge_WebM_Source_x64.ax`
- `VisioForge_WebM_Ogg_Source_x64.ax`
- `VisioForge_SSF_Muxer_x64.ax`

**Red**:
- `VisioForge_RTSP_Sink_x64.ax`
- `VisioForge_RTSP_Sink_X_x64.ax`
- `VisioForge_RTSP_Source_Live555_x64.ax`

**Filtros Base** (requeridos):
- `VisioForge_BaseFilters_x64.ax`

**Bibliotecas Auxiliares** (igual que x86):
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
- `VisioForge_MFT64.dll`

**Intel QuickSync** (opcional):
- `libmfxsw64.dll`
- `libmfxxp64.dll`

### Tamaño Total

- **Solo Filtros Principales**: ~40-60 MB
- **Con Codificador FFMPEG**: ~120-150 MB
- **Paquete Completo**: ~150-180 MB

### Requisitos de Hardware

- **NVENC**: Requiere GPU NVIDIA (GeForce GTX 600+ o Quadro K+) y drivers
- **QuickSync**: Requiere CPU Intel con gráficos integrados (4ta gen+)

---
## SDK de Cámara Virtual
### Archivos Principales
#### x86 (32-bit)
**Drivers de Cámara Virtual**:
- `VisioForge_Virtual_Camera.ax` - Driver de dispositivo de cámara virtual
- `VisioForge_Virtual_Audio_Card.ax` - Driver de dispositivo de audio virtual
**Filtros de Fuente**:
- `VisioForge_Push_Video_Source.ax` - Fuente push para streaming a cámara virtual
- `VisioForge_Screen_Capture_DD.ax` - Captura de pantalla DirectDraw
**Procesamiento** (incluido):
- `VisioForge_Video_Effects_Pro.ax` - Efectos de video
**Filtros Base** (requeridos):
- `VisioForge_BaseFilters.ax`
**Bibliotecas Auxiliares** (requeridas):
- `VisioForge_MFP.dll`
- `VisioForge_MFPX.dll`
**Runtime** (requerido):
- `vcomp140.dll` - Runtime Visual C++ OpenMP
#### x64 (64-bit)
**Drivers de Cámara Virtual**:
- `VisioForge_Virtual_Camera_x64.ax`
- `VisioForge_Virtual_Audio_Card_x64.ax`
**Filtros de Fuente**:
- `VisioForge_Push_Video_Source_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Procesamiento**:
- `VisioForge_Video_Effects_Pro_x64.ax`
**Filtros Base** (requeridos):
- `VisioForge_BaseFilters_x64.ax`
**Bibliotecas Auxiliares** (requeridas):
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### Tamaño Total
~15-20 MB
### Notas Importantes
- Los dispositivos de cámara virtual aparecen en aplicaciones de videoconferencia (Zoom, Teams, Skype, etc.)
- Soporta hasta 4 instancias de cámara virtual
- Requiere instalación de driver (incluido en instalador)
---

## Dependencias Comunes

### Visual C++ Redistributables

Todos los SDKs requieren Visual C++ Redistributable 2015-2022.

**Enlaces de Descarga**:
- x86: https://aka.ms/vs/17/release/vc_redist.x86.exe
- x64: https://aka.ms/vs/17/release/vc_redist.x64.exe

**Verificación de Instalación** (programática):
```cpp
// Verificar si VC++ Redistributable está instalado
bool IsVCRedistInstalled()
{
    HKEY hKey;
    LONG result = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
        L"SOFTWARE\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\x64",
        0, KEY_READ, &hKey);

    if (result == ERROR_SUCCESS)
    {
        RegCloseKey(hKey);
        return true;
    }
    return false;
}
```

### Utilidad de Registro

Todos los SDKs incluyen:
- `reg_special.exe` - Utilidad de registro personalizada

Esta herramienta puede usarse en lugar de `regsvr32` para registro de filtros.

---
## Lista de Verificación de Despliegue
### Archivos Mínimos Requeridos
Para cada SDK, debe incluir:
1. ✅ **Archivos de Filtros** - Todos los archivos .ax para su arquitectura (x86/x64)
2. ✅ **Filtros Base** - VisioForge_BaseFilters.ax (si el SDK lo requiere)
3. ✅ **DLLs Auxiliares** - VisioForge_MFP*.dll, VisioForge_MFPX*.dll
4. ✅ **Dependencias** - DLLs FFmpeg, bibliotecas VLC, etc.
5. ✅ **Archivo de Licencia** - license.rtf (mostrar en instalador)
6. ✅ **VC++ Redistributable** - Incluir o descargar en instalador
### Archivos Opcionales
- 📄 **Filtros LAV** - Soporte de códecs mejorado (Paquete de Filtros de Procesamiento)
- 📄 **DLLs QuickSync** - Codificación hardware Intel (Paquete de Filtros de Codificación)
- 📄 **Locale VLC** - Soporte multi-idioma (VLC Source)
- 📄 **Utilidad de Registro** - reg_special.exe (alternativa a regsvr32)
### Consideraciones de Arquitectura
**Aplicación 32-bit**:
- Incluir solo archivos x86 (.ax)
- No se necesitan versiones x64
**Aplicación 64-bit**:
- Incluir solo archivos x64 (_x64.ax)
- No se necesitan versiones x86
**Aplicación AnyCPU/.NET**:
- Incluir versiones x86 y x64
- Registrar ambas durante la instalación
- La aplicación usará la arquitectura apropiada en tiempo de ejecución
---

## Resumen de Tamaño de Archivos

| SDK | Tamaño Mínimo | Con Todas las Opciones |
|-----|---------------|------------------------|
| **FFMPEG Source** | ~80 MB (x86) | ~190 MB (ambas arq.) |
| **VLC Source** | ~150 MB | ~200 MB (con locales) |
| **Filtros de Procesamiento** | ~20 MB | ~180 MB (con LAV) |
| **Filtros de Codificación** | ~40 MB | ~300 MB (completo) |
| **Cámara Virtual** | ~15 MB | ~35 MB (ambas arq.) |

---
## Prueba del Paquete de Despliegue
Antes de lanzar, verifique que todos los archivos estén incluidos:
```batch
@echo off
echo Probando Registro de Filtros...
REM Probar cada filtro
regsvr32 /s "VisioForge_FFMPEG_Source_x64.ax"
if %errorLevel% neq 0 (
    echo ERROR: FFMPEG Source falló al registrar
    echo Verifique si todas las DLLs FFmpeg están presentes
    exit /b 1
)
REM Probar creación de filtro
SuAppDePrueba.exe
echo ¡Todos los filtros registrados exitosamente!
```
---

## Ver También

- [Registro de Filtros](filter-registration.md) - Cómo registrar filtros
- [Integración con Instaladores](installer-integration.md) - Creación de instaladores
- [Descripción del Despliegue](index.md) - Guía principal de despliegue
