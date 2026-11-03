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

# SDKs DirectShow - Referencia de archivos redistribuibles

## Descripción general

Este documento proporciona una lista completa de archivos requeridos para redistribuir cada SDK DirectShow con su aplicación. Todos los archivos deben incluirse en su instalador o paquete de despliegue.

---
## Filtro FFMPEG Source
### Archivos principales
#### x86 (32-bit)
**Filtro**:
- `VisioForge_FFMPEG_Source.ax` - Filtro DirectShow principal
**Bibliotecas FFmpeg** (requeridas):
- `avcodec-63.dll` - Biblioteca de códec de video/audio
- `avdevice-63.dll` - Manejo de dispositivos
- `avfilter-12.dll` - Filtrado de audio/video
- `avformat-63.dll` - Manejo de formatos de contenedor
- `avutil-61.dll` - Funciones de utilidad
- `swresample-7.dll` - Remuestreo de audio
- `swscale-10.dll` - Escalado de video y conversión de color
- `cacert.pem` - Certificados raíz; necesarios para fuentes https://, rtmps:// y tls://
**Tamaño total**: ~80-100 MB
#### x64 (64-bit)
**Filtro**:
- `VisioForge_FFMPEG_Source_x64.ax` - Filtro DirectShow principal (64-bit)
**Bibliotecas FFmpeg** (requeridas):
- `avcodec-63.dll` - Versión 64-bit
- `avdevice-63.dll` - Versión 64-bit
- `avfilter-12.dll` - Versión 64-bit
- `avformat-63.dll` - Versión 64-bit
- `avutil-61.dll` - Versión 64-bit
- `swresample-7.dll` - Versión 64-bit
- `swscale-10.dll` - Versión 64-bit
- `cacert.pem` - Certificados raíz; necesarios para fuentes https://, rtmps:// y tls://
**Tamaño total**: ~90-110 MB
### Estructura del directorio de instalación
```
SuApp\
├── x86\
│   ├── VisioForge_FFMPEG_Source.ax
│   ├── avcodec-63.dll
│   ├── avdevice-63.dll
│   ├── avfilter-12.dll
│   ├── avformat-63.dll
│   ├── avutil-61.dll
│   ├── swresample-7.dll
│   ├── swscale-10.dll
│   └── cacert.pem
└── x64\
    ├── VisioForge_FFMPEG_Source_x64.ax
    ├── avcodec-63.dll
    ├── avdevice-63.dll
    ├── avfilter-12.dll
    ├── avformat-63.dll
    ├── avutil-61.dll
    ├── swresample-7.dll
    ├── swscale-10.dll
    └── cacert.pem
```
Las DLL de FFmpeg x86 y x64 tienen los mismos nombres, por lo que cada arquitectura necesita su propia carpeta junto a su filtro.
### Archivos de licencia
- `license.rtf` - Acuerdo de licencia del SDK (incluir en instalador)
### Dependencias
- **Visual C++ Redistributable 2015-2022** (x86 o x64)
  - Descargar: https://aka.ms/vs/17/release/vc_redist.x64.exe
---

## Filtro VLC Source

### Archivos principales

#### x86, x64 y ARM64

**Filtro** (uno por arquitectura):
- `VisioForge_VLC_Source.ax` - Filtro DirectShow principal (x86)
- `VisioForge_VLC_Source_x64.ax` - Filtro DirectShow principal (x64)
- `VisioForge_VLC_Source_arm64.ax` - Filtro DirectShow principal (ARM64, del paquete NuGet `VisioForge.DotNet.Core.Redist.VLC.arm64`; el instalador incluye x86 y x64)

**Bibliotecas VLC** (requeridas):
- `libvlc.dll` - Biblioteca principal VLC
- `libvlccore.dll` - Funcionalidad principal VLC

**Directorio de plugins VLC** (requerido):
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

**Directorios de datos VLC**:
- `locale\` - Archivos de localización (opcional, ~50+ carpetas de idiomas)
- `lua\` - Scripts Lua para listas de reproducción y extensiones
- `hrtfs\` - Archivos de audio HRTF
  - `dodeca_and_7channel_3DSL_HRTF.sofa`

**Tamaño total**: ~150-200 MB (con todos los plugins y locales)

### Estructura del directorio de instalación

```
SuApp\
├── VisioForge_VLC_Source.ax          (x86; _x64.ax o _arm64.ax para x64 / ARM64)
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

### Archivos de licencia

- `license.rtf` - Acuerdo de licencia del SDK

### Dependencias

- **Visual C++ Redistributable 2015-2022** (misma arquitectura que el filtro)

### Notas importantes

- **Todos los plugins VLC deben incluirse** - Los plugins faltantes causarán fallos de reproducción para ciertos formatos
- **Mantener estructura de directorios** - VLC espera plugins en subdirectorio `plugins\`
- **Una arquitectura por carpeta** - Cada compilación incluye sus propios `libvlc.dll`, `libvlccore.dll` y `plugins\`; despliegue el conjunto que corresponde al filtro y no los mezcle en una misma carpeta. Una aplicación AnyCPU guarda cada conjunto en su propia subcarpeta (por ejemplo `x86\` y `x64\`)

---
## Processing Filters Pack
### Filtros principales
#### x86 (32-bit)
**Procesamiento de video**:
- `VisioForge_Video_Effects_Pro.ax` - Filtro de efectos de video (35+ efectos)
- `VisioForge_Video_Mixer.ax` - Mezclador de video multi-fuente
- `VisioForge_Screen_Capture_DD.ax` - Captura de pantalla DirectDraw
**Procesamiento de audio**:
- `VisioForge_Audio_Enhancer.ax` - Filtro de mejora de audio
- `VisioForge_Audio_Effects_4.ax` - Efectos de audio (opcional)
- `VisioForge_Audio_Mixer.ax` - Mezclador de audio
**Filtros base** (requeridos):
- `VisioForge_BaseFilters.ax` - Biblioteca de filtros base principal
- `VisioForge_AsyncEx.ax` - Lector de archivos asíncrono (opcional)
**Bibliotecas auxiliares**:
- `VisioForge_MFP.dll` - Auxiliar Media Foundation
- `VisioForge_MFPX.dll` - Funciones MF extendidas
#### x64 (64-bit)
**Procesamiento de video**:
- `VisioForge_Video_Effects_Pro_x64.ax`
- `VisioForge_Video_Mixer_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Procesamiento de audio**:
- `VisioForge_Audio_Enhancer_x64.ax`
- `VisioForge_Audio_Mixer_x64.ax`
**Filtros base** (requeridos):
- `VisioForge_BaseFilters_x64.ax`
- `VisioForge_AsyncEx_x64.ax` (opcional)
**Bibliotecas auxiliares**:
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### Filtros LAV (opcional pero recomendado)
Los filtros LAV proporcionan soporte adicional de códecs y se incluyen con el Processing Filters Pack.
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
**Bibliotecas adicionales**:
- `libbluray.dll` - Soporte Blu-ray
- `IntelQuickSyncDecoder.dll` - Decodificación por hardware Intel QuickSync
**Manifiesto**:
- `LAVFilters.Dependencies.manifest`
**Licencia**:
- `COPYING` - Licencia de filtros LAV (LGPL)
#### x64
Mismos archivos que x86 pero versiones de 64-bit.
### Estructura del directorio de instalación
```
YourApp\
├── Filters\
│   ├── VisioForge_Video_Effects_Pro.ax
│   ├── VisioForge_Video_Effects_Pro_x64.ax
│   ├── VisioForge_Video_Mixer.ax
│   ├── VisioForge_Video_Mixer_x64.ax
│   ├── VisioForge_Audio_Enhancer.ax
│   ├── VisioForge_Audio_Enhancer_x64.ax
│   ├── VisioForge_BaseFilters.ax
│   ├── VisioForge_BaseFilters_x64.ax
│   ├── VisioForge_MFP.dll
│   ├── VisioForge_MFP64.dll
│   ├── VisioForge_MFPX.dll
│   └── VisioForge_MFPX64.dll
└── LAV\
    ├── x86\
    │   ├── LAVSplitter.ax
    │   ├── LAVVideo.ax
    │   ├── LAVAudio.ax
    │   ├── avcodec-lav-58.dll
    │   └── ... (otros archivos LAV)
    └── x64\
        ├── LAVSplitter.ax
        ├── LAVVideo.ax
        └── ... (otros archivos LAV)
```
### Archivos de licencia
- `license.rtf` - Licencia del SDK VisioForge
- `VisioForge_AsyncEx_license.htm` - Licencia del filtro Async
- `VisioForge_Audio_Effects_4_note.txt` - Notas de efectos de audio
- `COPYING` - Licencia de filtros LAV (en el directorio LAV)
### Tamaño total
- **Solo filtros principales**: ~20-30 MB
- **Con filtros LAV**: ~80-100 MB
---

## Encoding Filters Pack

### Filtros principales

#### x86 (32-bit)

**Codificadores de video**:
- `VisioForge_NVENC.ax` - Codificador hardware NVIDIA
- `VisioForge_H264_Encoder.ax` - Codificador software H.264
- `VisioForge_H264_Encoder_v9.ax` - Codificador H.264 v9
- `VisioForge_H264_Decoder.ax` - Decodificador H.264
- `VisioForge_WebM_VP8_Encoder.ax` - Codificador VP8
- `VisioForge_WebM_VP9_Encoder.ax` - Codificador VP9 (en x64)
- `VisioForge_WebM_VP8_Decoder.ax` - Decodificador VP8
- `VisioForge_WebM_VP9_Decoder.ax` - Decodificador VP9

**Codificadores de audio**:
- `VisioForge_AAC_Encoder.ax` - Codificador AAC
- `VisioForge_AAC_Encoder_v10.ax` - Codificador AAC v10
- `VisioForge_LAME.ax` - Codificador MP3 (LAME)
- `VisioForge_WebM_Vorbis_Encoder.ax` - Codificador Vorbis
- `VisioForge_WebM_Vorbis_Decoder.ax` - Decodificador Vorbis

**Muxers/demuxers**:
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

**Filtros base** (requeridos):
- `VisioForge_BaseFilters.ax`

**Bibliotecas auxiliares** (requeridas):
- `VisioForge_MFP.dll` - Auxiliar Media Foundation
- `VisioForge_MFP64.dll` - Auxiliar MF 64-bit
- `VisioForge_MFPX.dll` - Funciones MF extendidas
- `VisioForge_MFPX64.dll` - MF extendido 64-bit
- `VisioForge_MFT.dll` - Media Foundation Transform

**Intel QuickSync** (opcional):
- `libmfxsw32.dll` - Biblioteca software QuickSync
- `libmfxxp32.dll` - Biblioteca XP QuickSync

#### x64 (64-bit)

**Codificadores de video**:
- `VisioForge_NVENC_x64.ax`
- `VisioForge_H264_Encoder_x64.ax`
- `VisioForge_H264_Encoder_v9_x64.ax`
- `VisioForge_H264_Decoder_x64.ax`
- `VisioForge_WebM_VP8_Encoder_x64.ax`
- `VisioForge_WebM_VP9_Encoder_x64.ax`
- `VisioForge_WebM_VP8_Decoder_x64.ax`
- `VisioForge_WebM_VP9_Decoder_x64.ax`

**Codificadores de audio**:
- `VisioForge_AAC_Encoder_x64.ax`
- `VisioForge_AAC_Encoder_v10_x64.ax`
- `VisioForge_LAME_x64.ax`
- `VisioForge_WebM_Vorbis_Encoder_x64.ax`
- `VisioForge_WebM_Vorbis_Decoder_x64.ax`

**Muxers/demuxers**:
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
- `VisioForge_RTSP_Source_Live555_x64.ax`

**Filtros base** (requeridos):
- `VisioForge_BaseFilters_x64.ax`

**Bibliotecas auxiliares** (igual que x86):
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
- `VisioForge_MFT64.dll`

**Intel QuickSync** (opcional):
- `libmfxsw64.dll`
- `libmfxxp64.dll`

### Codificador FFMPEG

El codificador FFMPEG tiene su propio conjunto de bibliotecas FFmpeg:

#### x86

**Filtro**:
- `VisioForge_FFMPEG_Encoder.ax`

**Bibliotecas FFmpeg**:
- `avcodec-63.dll`
- `avdevice-63.dll`
- `avfilter-12.dll`
- `avformat-63.dll`
- `avutil-61.dll`
- `swresample-7.dll`
- `swscale-10.dll`
- `cacert.pem` - Certificados raíz; necesarios para publicar en salidas rtmps:// y tls://

**Información**:
- `vfffmpeg_info.txt` - Información de compilación de FFmpeg

#### x64

Mismos archivos que x86 pero versiones de 64-bit.

### Estructura del directorio de instalación

```
YourApp\
├── Filters\
│   ├── VisioForge_NVENC.ax
│   ├── VisioForge_NVENC_x64.ax
│   ├── VisioForge_H264_Encoder.ax
│   ├── VisioForge_H264_Encoder_x64.ax
│   ├── VisioForge_AAC_Encoder.ax
│   ├── VisioForge_AAC_Encoder_x64.ax
│   ├── VisioForge_MP4_Muxer.ax
│   ├── VisioForge_MP4_Muxer_x64.ax
│   ├── VisioForge_BaseFilters.ax
│   ├── VisioForge_BaseFilters_x64.ax
│   ├── VisioForge_MFP.dll
│   ├── VisioForge_MFP64.dll
│   ├── VisioForge_MFPX.dll
│   ├── VisioForge_MFPX64.dll
│   ├── VisioForge_MFT.dll
│   ├── VisioForge_MFT64.dll
│   ├── libmfxsw32.dll        (QuickSync)
│   ├── libmfxsw64.dll        (QuickSync)
│   └── ... (otros filtros)
└── FFMPEG\
    ├── x86\
    │   ├── VisioForge_FFMPEG_Encoder.ax
    │   ├── avcodec-63.dll
    │   ├── avformat-63.dll
    │   └── ... (otros DLL de FFmpeg)
    └── x64\
        ├── VisioForge_FFMPEG_Encoder_x64.ax
        └── ... (DLL de FFmpeg)
```

### Archivos de licencia

- `license.rtf` - Licencia del SDK

### Tamaño total

- **Solo filtros principales**: ~40-60 MB
- **Con codificador FFMPEG**: ~120-150 MB
- **Paquete completo**: ~150-180 MB

### Requisitos de hardware

- **NVENC**: Requiere GPU NVIDIA (GeForce GTX 600+ o Quadro K+) y drivers
- **QuickSync**: Requiere CPU Intel con gráficos integrados (4ta gen+)

---
## Virtual Camera SDK
### Archivos principales
#### x86 (32-bit)
**Drivers de cámara virtual**:
- `VisioForge_Virtual_Camera.ax` - Driver de dispositivo de cámara virtual
- `VisioForge_Virtual_Audio_Card.ax` - Driver de dispositivo de audio virtual
**Filtros de fuente**:
- `VisioForge_Push_Video_Source.ax` - Fuente push para streaming a cámara virtual
- `VisioForge_Screen_Capture_DD.ax` - Captura de pantalla DirectDraw
**Procesamiento** (incluido):
- `VisioForge_Video_Effects_Pro.ax` - Efectos de video
**Filtros base** (requeridos):
- `VisioForge_BaseFilters.ax`
**Bibliotecas auxiliares** (requeridas):
- `VisioForge_MFP.dll`
- `VisioForge_MFPX.dll`
**Runtime** (requerido):
- `vcomp140.dll` - Runtime Visual C++ OpenMP
#### x64 (64-bit)
**Drivers de cámara virtual**:
- `VisioForge_Virtual_Camera_x64.ax`
- `VisioForge_Virtual_Audio_Card_x64.ax`
**Filtros de fuente**:
- `VisioForge_Push_Video_Source_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Procesamiento**:
- `VisioForge_Video_Effects_Pro_x64.ax`
**Filtros base** (requeridos):
- `VisioForge_BaseFilters_x64.ax`
**Bibliotecas auxiliares** (requeridas):
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### Estructura del directorio de instalación
```
YourApp\
├── VisioForge_Virtual_Camera.ax
├── VisioForge_Virtual_Camera_x64.ax
├── VisioForge_Virtual_Audio_Card.ax
├── VisioForge_Virtual_Audio_Card_x64.ax
├── VisioForge_Push_Video_Source.ax
├── VisioForge_Push_Video_Source_x64.ax
├── VisioForge_Screen_Capture_DD.ax
├── VisioForge_Screen_Capture_DD_x64.ax
├── VisioForge_Video_Effects_Pro.ax
├── VisioForge_Video_Effects_Pro_x64.ax
├── VisioForge_BaseFilters.ax
├── VisioForge_BaseFilters_x64.ax
├── VisioForge_MFP.dll
├── VisioForge_MFP64.dll
├── VisioForge_MFPX.dll
├── VisioForge_MFPX64.dll
└── vcomp140.dll
```
### Archivos de licencia
- `license.rtf` - Licencia del SDK
### Tamaño total
~15-20 MB
### Notas importantes
- Los dispositivos de cámara virtual aparecen en aplicaciones de videoconferencia (Zoom, Teams, Skype, etc.)
- Soporta hasta 4 instancias de cámara virtual
- Requiere instalación de driver (incluido en instalador)
---

## Dependencias comunes

### Visual C++ Redistributables

Todos los SDKs requieren Visual C++ Redistributable 2015-2022.

**Enlaces de descarga**:
- x86: https://aka.ms/vs/17/release/vc_redist.x86.exe
- x64: https://aka.ms/vs/17/release/vc_redist.x64.exe

**Verificación de instalación** (programática):
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

### Utilidad de registro

Todos los SDKs incluyen:
- `reg_special.exe` - Utilidad de registro personalizada

Esta herramienta puede usarse en lugar de `regsvr32` para registro de filtros.

---
## Lista de verificación de despliegue
### Archivos mínimos requeridos
Para cada SDK, debe incluir:
1. ✅ **Archivos de filtros** - Todos los archivos .ax para su arquitectura (x86/x64)
2. ✅ **Filtros base** - VisioForge_BaseFilters.ax (si el SDK lo requiere)
3. ✅ **DLLs auxiliares** - VisioForge_MFP*.dll, VisioForge_MFPX*.dll
4. ✅ **Dependencias** - DLLs FFmpeg, bibliotecas VLC, etc.
5. ✅ **Archivo de licencia** - license.rtf (mostrar en instalador)
6. ✅ **VC++ Redistributable** - Incluir o descargar en instalador
### Archivos opcionales
- 📄 **Filtros LAV** - Soporte de códecs mejorado (Processing Filters Pack)
- 📄 **DLLs QuickSync** - Codificación hardware Intel (Encoding Filters Pack)
- 📄 **Locale VLC** - Soporte multi-idioma (VLC Source)
- 📄 **Utilidad de registro** - reg_special.exe (alternativa a regsvr32)
### Consideraciones de arquitectura
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

## Resumen de tamaño de archivos

| SDK | Tamaño mínimo | Con todas las opciones |
|-----|---------------|------------------------|
| **FFMPEG Source** | ~80 MB (x86) | ~190 MB (ambas arq.) |
| **VLC Source** | ~150 MB | ~200 MB (con locales) |
| **Processing Filters** | ~20 MB | ~180 MB (con LAV) |
| **Encoding Filters** | ~40 MB | ~300 MB (completo) |
| **Virtual Camera** | ~15 MB | ~35 MB (ambas arq.) |

---
## Prueba del paquete de despliegue
Antes de lanzar, verifique que todos los archivos estén incluidos. Coloque el script en la carpeta del paquete y ejecútelo en una máquina de prueba limpia: al terminar, anula el registro del filtro.
```batch
@echo off
setlocal
echo Probando registro de filtros...

REM Verificar privilegios de administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: Este script debe ejecutarse como administrador.
    exit /b 1
)

REM Probar cada filtro
regsvr32 /s "%~dp0VisioForge_FFMPEG_Source_x64.ax"
if %errorLevel% neq 0 (
    echo ERROR: no se pudo registrar FFMPEG Source
    echo Compruebe que el archivo .ax y las DLL de FFmpeg se encuentren en la carpeta del script
    exit /b 1
)
REM Probar creación de filtro
"%~dp0SuAppDePrueba.exe"
set TEST_RESULT=%errorLevel%
REM Quitar el registro de prueba
regsvr32 /s /u "%~dp0VisioForge_FFMPEG_Source_x64.ax"
if %TEST_RESULT% neq 0 (
    echo ERROR: SuAppDePrueba.exe no pudo crear el filtro
    exit /b 1
)
echo Prueba de FFMPEG Source superada.
```
---

## Ver también

- [Registro de filtros](filter-registration.md) - Cómo registrar filtros
- [Integración con instaladores](installer-integration.md) - Creación de instaladores
- [Descripción del despliegue](index.md) - Guía principal de despliegue
