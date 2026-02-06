---
title: Referencia de Códecs: Filtros de Codificación DirectShow
description: Referencia de códecs DirectShow con video H.264/H.265/VP8/VP9, audio AAC/MP3/Opus y aceleración de hardware (NVENC, QuickSync, AMF).
---

# Paquete de Filtros de Codificación - Referencia de Códecs

## Resumen

Este documento proporciona una referencia completa para todos los códecs de video y audio admitidos por el Paquete de Filtros de Codificación DirectShow. El paquete incluye codificadores tanto de software como acelerados por hardware para codificación de medios profesionales.

---
## Códecs de Video
### H.264/AVC (MPEG-4 Parte 10)
El códec de video más utilizado para streaming, broadcasting y almacenamiento de archivos.
#### Opciones del Codificador
| Tipo de Codificador | Descripción | Soporte de Hardware | Rendimiento | Calidad |
|---------------------|-------------|---------------------|-------------|---------|
| **Software (x264)** | Codificador H.264 basado en CPU | Ninguno | Moderado | Excelente |
| **NVENC** | Codificador GPU NVIDIA | GPUs NVIDIA (Kepler+) | Muy Rápido | Bueno-Excelente |
| **QuickSync** | Gráficos integrados Intel | CPUs Intel (2ª gen+) | Rápido | Bueno |
| **AMD AMF** | Codificador GPU AMD | GPUs AMD (GCN+) | Rápido | Bueno |
| **Media Foundation** | Codificador MF de Windows | Varios (dependiente del SO) | Moderado | Bueno |
#### Perfiles y Niveles
**Perfiles**:
- **Baseline** - Características básicas, dispositivos móviles
- **Main** - Características estándar, la mayoría de aplicaciones
- **High** - Características avanzadas, contenido HD, Blu-ray
**Niveles Comunes**:
- **Level 3.0** - SD (720x480 @ 30 fps)
- **Level 3.1** - 720p (1280x720 @ 30 fps)
- **Level 4.0** - 1080p (1920x1080 @ 30 fps)
- **Level 4.1** - 1080p @ 60 fps
- **Level 5.0** - 4K (3840x2160 @ 30 fps)
- **Level 5.1** - 4K @ 60 fps
#### Modos de Control de Tasa
| Modo | Descripción | Caso de Uso | Comportamiento de Bitrate |
|------|-------------|-------------|---------------------------|
| **CBR** | Bitrate Constante | Streaming, broadcasting | Bitrate fijo |
| **VBR** | Bitrate Variable | Almacenamiento de archivos | Varía según complejidad |
| **CQP** | Cuantización Constante | Archivo de alta calidad | Varía significativamente |
#### Configuraciones Recomendadas
**Streaming (1080p @ 30fps)**:
- Bitrate: 4-6 Mbps
- Perfil: High
- Nivel: 4.0
- Tamaño GOP: 60 (2 segundos)
- B-frames: 2
**Grabación (1080p @ 60fps)**:
- Bitrate: 8-12 Mbps
- Perfil: High
- Nivel: 4.1
- Tamaño GOP: 120 (2 segundos)
- B-frames: 3
**Streaming de Baja Latencia**:
- Bitrate: 2-4 Mbps
- Perfil: Main
- Nivel: 3.1
- Tamaño GOP: 30 (1 segundo)
- B-frames: 0
---

### H.265/HEVC (Codificación de Video de Alta Eficiencia)

Códec de próxima generación que ofrece una compresión 40-50% mejor que H.264.

#### Opciones del Codificador

| Tipo de Codificador | Descripción | Soporte de Hardware | Rendimiento | Calidad |
|---------------------|-------------|---------------------|-------------|---------|
| **Software (x265)** | Codificador HEVC basado en CPU | Ninguno | Lento | Excelente |
| **NVENC** | Codificador GPU NVIDIA | GPUs NVIDIA (Maxwell+) | Rápido | Bueno-Excelente |
| **QuickSync** | Gráficos integrados Intel | CPUs Intel (6ª gen+) | Rápido | Bueno |
| **AMD AMF** | Codificador GPU AMD | GPUs AMD (Fiji+) | Rápido | Bueno |

#### Perfiles y Capas

**Perfiles**:
- **Main** - 8-bit, 4:2:0, uso estándar
- **Main 10** - 10-bit, soporte HDR
- **Main Still Picture** - Imágenes individuales

**Capas**:
- **Main Tier** - Aplicaciones estándar
- **High Tier** - Profesional/broadcast

**Niveles Comunes**:
- **Level 3.1** - 720p @ 30 fps
- **Level 4.0** - 1080p @ 30 fps
- **Level 4.1** - 1080p @ 60 fps
- **Level 5.0** - 4K @ 30 fps
- **Level 5.1** - 4K @ 60 fps

#### Configuraciones Recomendadas

**Streaming 4K (2160p @ 30fps)**:
- Bitrate: 15-20 Mbps
- Perfil: Main
- Nivel: 5.0
- Tamaño GOP: 60
- Codificación de Azulejos: Habilitada

**1080p Alta Calidad**:
- Bitrate: 3-5 Mbps
- Perfil: Main o Main 10
- Nivel: 4.0
- Tamaño GOP: 90

---
### VP8
Códec de video de código abierto de Google, principalmente para contenedores WebM.
#### Características
- **Licencia**: Gratuito, código abierto, sin royalties
- **Contenedor**: WebM (preferido), MKV
- **Soporte de Hardware**: Limitado
- **Calidad**: Buena en bitrates medio-altos
- **Complejidad**: Tiempo de codificación moderado
#### Configuraciones Recomendadas
**Streaming WebM (720p)**:
- Bitrate: 1-2 Mbps
- Tamaño GOP: 120
- Calidad: Buena (escala 0-63, más bajo es mejor)
- Hilos: Auto
---

### VP9

Sucesor de VP8 con eficiencia de compresión significativamente mejorada.

#### Características

- **Licencia**: Gratuito, código abierto, sin royalties
- **Contenedor**: WebM (primario), MKV
- **Soporte de Hardware**: GPUs recientes (Intel, NVIDIA, AMD)
- **Calidad**: Comparable a H.265
- **Complejidad**: Muy alta (software), moderada (hardware)

#### Perfiles

- **Profile 0** - 8-bit, 4:2:0 (más común)
- **Profile 1** - 8-bit, 4:2:2/4:4:4
- **Profile 2** - 10/12-bit, 4:2:0
- **Profile 3** - 10/12-bit, 4:2:2/4:4:4

#### Configuraciones Recomendadas

**YouTube/WebM (1080p @ 30fps)**:
- Bitrate: 2-4 Mbps
- Calidad/Velocidad: 1 (más rápido), 0 (más lento/mejor)
- Tamaño GOP: 60
- Columnas de Azulejos: 2

---
### MPEG-2
Códec heredado aún utilizado para DVDs y broadcasting.
#### Características
- **Licencia**: Requiere licencia
- **Contenedor**: MPEG-PS, MPEG-TS, VOB
- **Soporte de Hardware**: Universal
- **Calidad**: Buena pero requiere bitrates más altos
- **Casos de Uso**: Creación de DVD, broadcasting
#### Variantes Comunes
- **MPEG-2 DVD** - 4-8 Mbps, 720x480/720x576
- **MPEG-2 SVCD** - 2.5 Mbps, 480x480/480x576
- **MPEG-2 HD** - 15-25 Mbps, 1920x1080
#### Configuraciones Recomendadas
**Video DVD (NTSC)**:
- Resolución: 720x480
- Bitrate: 6 Mbps
- Tamaño GOP: 15 (NTSC) o 12 (PAL)
- Relación de Aspecto: 16:9 o 4:3
---

### MPEG-4 Parte 2

Códec MPEG-4 heredado (era DivX/Xvid).

#### Características

- **Licencia**: Requiere licencia
- **Contenedor**: AVI, MP4, MKV
- **Calidad**: Moderada
- **Casos de Uso**: Contenido heredado, dispositivos de baja potencia

#### Configuraciones Recomendadas

**Definición Estándar**:
- Resolución: 640x480
- Bitrate: 1-2 Mbps
- Tamaño GOP: 250

---
## Códecs de Audio
### AAC (Codificación de Audio Avanzada)
Códec de audio estándar de la industria para la mayoría de aplicaciones.
#### Opciones del Codificador
| Tipo de Codificador | Descripción | Calidad | Rendimiento |
|---------------------|-------------|---------|-------------|
| **FFmpeg AAC** | Codificador de software | Buena | Rápido |
| **Media Foundation AAC** | Incorporado en Windows | Buena | Rápido |
| **FAAC** | Codificador de código abierto | Moderada | Rápido |
#### Perfiles
- **AAC-LC (Low Complexity)** - Estándar, más compatible
- **HE-AAC (High Efficiency)** - Mejor en bitrates bajos
- **HE-AAC v2** - Aún mejor para bitrates muy bajos
#### Bitrates Recomendados
| Calidad | Estéreo | 5.1 Surround |
|---------|---------|--------------|
| **Baja** | 64-96 kbps | 192 kbps |
| **Media** | 128 kbps | 256 kbps |
| **Alta** | 192 kbps | 384 kbps |
| **Muy Alta** | 256-320 kbps | 448-640 kbps |
#### Tasas de Muestreo
- **44.1 kHz** - Calidad CD, música
- **48 kHz** - Audio profesional, video
- **32 kHz** - Calidad inferior (voz)
---

### MP3 (MPEG-1/2 Audio Layer III)

Códec de audio heredado pero aún ampliamente utilizado.

#### Opciones del Codificador

- **LAME** - Codificador de código abierto de excelente calidad
- **FFmpeg MP3** - Codificador incorporado

#### Modos de Bitrate

| Modo | Descripción | Tamaño de Archivo | Calidad |
|------|-------------|-------------------|---------|
| **CBR** | Bitrate Constante | Predecible | Consistente |
| **VBR** | Bitrate Variable | Más pequeño | Mejor |
| **ABR** | Bitrate Promedio | Equilibrado | Buena |

#### Configuraciones Recomendadas

**Música (Alta Calidad)**:
- Modo: VBR
- Calidad: V0-V2 (escala LAME)
- Bitrate Aproximado: 190-245 kbps
- Tasa de Muestreo: 44.1 kHz

**Podcast/Voz**:
- Modo: CBR
- Bitrate: 96-128 kbps
- Tasa de Muestreo: 44.1 kHz

**Bajo Ancho de Banda**:
- Modo: VBR
- Calidad: V5-V6
- Bitrate Aproximado: 120-150 kbps

---
### Vorbis
Alternativa de código abierto a MP3 y AAC.
#### Características
- **Licencia**: Completamente gratuita, sin patentes
- **Contenedor**: OGG (primario), WebM, MKV
- **Calidad**: Excelente, especialmente en medio-bajo bitrates
- **Compatibilidad**: Buena pero no universal
#### Configuraciones Recomendadas
**Música (Alta Calidad)**:
- Calidad: 6-8 (escala 0-10)
- Bitrate Aproximado: 192-256 kbps
- Tasa de Muestreo: 44.1 kHz o 48 kHz
**Streaming**:
- Calidad: 4-5
- Bitrate Aproximado: 128-160 kbps
---

### Opus

Códec moderno y altamente eficiente para voz y música.

#### Características

- **Licencia**: Sin royalties, estandarizado (RFC 6716)
- **Contenedor**: WebM, MKV, OGG
- **Latencia**: Extremadamente baja (5-66.5 ms)
- **Rango de Bitrate**: 6-510 kbps
- **Calidad**: Superior a MP3, AAC, Vorbis

#### Aplicaciones

- **VoIP/Chat de Voz**: 8-24 kbps
- **Streaming de Música**: 64-128 kbps
- **Fidelidad Alta**: 128-256 kbps

#### Configuraciones Recomendadas

**Chat de Voz**:
- Bitrate: 16-24 kbps
- Tasa de Muestreo: 16 kHz o 48 kHz
- Aplicación: VoIP

**Música**:
- Bitrate: 96-160 kbps
- Tasa de Muestreo: 48 kHz
- Aplicación: Audio

---
### FLAC (Código Libre de Audio Sin Pérdidas)
Compresión de audio sin pérdidas.
#### Características
- **Licencia**: Código abierto, sin royalties
- **Compresión**: Típicamente 40-60% del original
- **Calidad**: Sin pérdidas bit-perfect
- **Compatibilidad**: Buena y mejorando
#### Niveles de Compresión
- **Level 0** - Más rápido, ~50% de compresión
- **Level 5** - Predeterminado, ~55% de compresión
- **Level 8** - Más lento, ~60% de compresión
#### Configuraciones Recomendadas
**Archivo**:
- Nivel de Compresión: 5-8
- Tasa de Muestreo: Original (típicamente 44.1 o 48 kHz)
- Profundidad de Bit: Original (16 o 24-bit)
**Streaming**:
- Nivel de Compresión: 0-3
- Tasa de Muestreo: 44.1 o 48 kHz
---

### Speex

Códec especializado para compresión de voz.

#### Características

- **Licencia**: Código abierto (BSD)
- **Propósito**: Compresión de voz (no música)
- **Bitrate**: 2-44 kbps
- **Calidad**: Optimizado para voz

#### Modos

- **Narrowband** (8 kHz) - Calidad telefónica, 2.15-24.6 kbps
- **Wideband** (16 kHz) - Mayor claridad, 4-44 kbps
- **Ultra-wideband** (32 kHz) - Espectro completo de voz

#### Configuraciones Recomendadas

**VoIP**:
- Modo: Wideband
- Calidad: 6-8 (escala 0-10)
- Bitrate: ~15-20 kbps

---
## Resumen de Aceleración de Hardware
### NVIDIA NVENC
**Códecs Soportados**:
- H.264/AVC (todas las GPUs NVIDIA desde generación Kepler)
- H.265/HEVC (generación Maxwell y posteriores)
**Generaciones**:
- **Kepler** (GTX 600/700) - 1ª gen, H.264 básico
- **Maxwell** (GTX 900) - 2ª gen, soporte HEVC
- **Pascal** (GTX 10XX) - 3ª gen, calidad mejorada
- **Turing/Ampere** (RTX 20XX/30XX) - 7ª/8ª gen, calidad excelente
**Rendimiento**: Hasta 8K @ 30 fps (dependiente de GPU)
**Configuraciones de Calidad**:
- Preset: P1 (más rápido) a P7 (más lento, mejor calidad)
- Recomendado: P4-P6 para calidad/velocidad equilibrada
---

### Intel QuickSync

**Códecs Soportados**:
- H.264/AVC (2ª gen Core y posteriores)
- H.265/HEVC (6ª gen Core y posteriores)
- VP9 (9ª gen Core y posteriores)

**Generaciones**:
- **Sandy Bridge** (2ª gen) - Soporte H.264
- **Skylake** (6ª gen) - Soporte HEVC
- **Ice Lake** (10ª gen móvil) - Calidad mejorada
- **Rocket Lake** (11ª gen) - Características mejoradas

**Rendimiento**: Hasta 4K @ 60 fps

**Calidad**: Buena, mejorando con cada generación

---
### AMD AMF (Marco de Medios Avanzado)
**Códecs Soportados**:
- H.264/AVC (GCN 1.0 y posteriores)
- H.265/HEVC (Fiji/Polaris y posteriores)
**Generaciones**:
- **GCN 1-4** (R7/R9, RX 400/500) - Solo H.264
- **Vega** (RX Vega) - Soporte HEVC
- **RDNA** (RX 5000/6000) - Calidad mejorada
**Rendimiento**: Hasta 4K @ 60 fps
**Calidad**: Buena, competitiva con QuickSync
---

## Guía de Selección de Códec

### Para Streaming (En Vivo)

**Recomendado**: H.264 (NVENC/QuickSync)
- **Razón**: Compatibilidad universal, baja latencia, aceleración de hardware
- **Alternativa**: H.264 (software)

**Configuraciones**:
- 1080p @ 30fps: 4-6 Mbps
- 720p @ 30fps: 2.5-4 Mbps
- Baja latencia: Deshabilitar B-frames

---
### Para Grabación (Alta Calidad)
**Recomendado**: H.265 (NVENC/QuickSync) o H.264 (software)
- **Razón**: Mejor relación calidad-tamaño
- **Alternativa**: HEVC software para máxima calidad
**Configuraciones**:
- 4K @ 30fps: 15-25 Mbps (HEVC) o 35-50 Mbps (H.264)
- 1080p @ 60fps: 8-15 Mbps (HEVC) o 15-25 Mbps (H.264)
---

### Para Entrega Web

**Recomendado**: VP9 o H.264
- **Razón**: Compatibilidad de navegador, sin royalties (VP9)

**Configuraciones**:
- VP9: 1080p @ 2-4 Mbps
- H.264: 1080p @ 4-6 Mbps

---
### Para Audio
**Música**: AAC (128-192 kbps) o Opus (96-160 kbps)
**Voz**: Opus (16-32 kbps) o Speex (15-20 kbps)
**Archivo**: FLAC (sin pérdidas)
**Podcast**: MP3 VBR (V4-V2, ~130-190 kbps) o AAC (128 kbps)
---

## Matriz de Compatibilidad

| Códec | MP4 | MKV | AVI | WebM | OGG | MPEG-TS |
|-------|-----|-----|-----|------|-----|---------|
| **H.264** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **H.265** | ✓ | ✓ | ✗ | ✗ | ✗ | ✓ |
| **VP8** | ✗ | ✓ | ✗ | ✓ | ✗ | ✗ |
| **VP9** | ✗ | ✓ | ✗ | ✓ | ✗ | ✗ |
| **MPEG-2** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **AAC** | ✓ | ✓ | ✗ | ✗ | ✗ | ✓ |
| **MP3** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **Vorbis** | ✗ | ✓ | ✗ | ✓ | ✓ | ✗ |
| **Opus** | ✗ | ✓ | ✗ | ✓ | ✓ | ✗ |
| **FLAC** | ✓ | ✓ | ✗ | ✓ | ✓ | ✗ |

---
## Véase También
- [Resumen del Paquete de Filtros de Codificación](index.md)
- [Referencia de Muxers](muxers-reference.md)
- [Referencia de Interfaz NVENC](interfaces/nvenc.md)
- [Ejemplos de Código](examples.md)