---
title: Referencia de Multiplexores DirectShow
description: Referencia de formatos de contenedor DirectShow para multiplexores MP4, MKV, WebM, MPEG-TS y AVI con códecs compatibles y opciones de transmisión.
---

# Paquete de Filtros de Codificación - Referencia de Multiplexores

## Descripción General

Este documento proporciona información completa sobre todos los formatos de contenedor (multiplexores) compatibles con el Paquete de Filtros de Codificación DirectShow. Los multiplexores combinan flujos de video y audio en archivos contenedores para almacenamiento y transmisión.

---
## Contenedor MP4
### Descripción General
MPEG-4 Parte 14 (MP4) es el formato de contenedor más utilizado para la distribución de video.
**Extensiones de Archivo**: `.mp4`, `.m4v`, `.m4a` (solo audio)
**Tipo MIME**: `video/mp4`, `audio/mp4`
### Códecs Compatibles
#### Códecs de Video
- H.264/AVC ✓ (Principal)
- H.265/HEVC ✓
- MPEG-4 Parte 2 ✓
- MPEG-2 ✗ (usar MPEG-TS en su lugar)
- VP8/VP9 ✗ (usar WebM en su lugar)
#### Códecs de Audio
- AAC ✓ (Principal)
- MP3 ✓
- Opus ✗
- Vorbis ✗
- FLAC ✓
- PCM ✓
### Características
**Soporte de Transmisión**:
- **Descarga Progresiva**: ✓ (con la ubicación adecuada del átomo moov)
- **Transmisión Adaptativa**: ✓ (DASH, HLS con MP4 fragmentado)
- **Transmisión en Vivo**: ✓ (MP4 fragmentado)
**Soporte de Metadatos**:
- **Etiquetas Básicas**: Título, artista, álbum, año
- **Carátula**: ✓
- **Capítulos**: ✓
- **Subtítulos**: ✓ (VTT, SRT, varios formatos de texto)
**Características Técnicas**:
- **Múltiples Pistas de Audio**: ✓
- **Múltiples Pistas de Subtítulos**: ✓
- **Inicio Rápido**: ✓ (átomo moov al principio)
- **MP4 Fragmentado**: ✓ (para transmisión)
### Mejores Prácticas
**Para Descarga Progresiva (Web)**:
```
- Colocar átomo moov al principio (inicio rápido)
- Usar perfil H.264 Baseline/Main
- Audio AAC-LC
- Intervalo de fotogramas clave: 2-4 segundos
```
**Para Reproducción Local**:
```
- Perfil H.264 High o H.265
- AAC-LC o HE-AAC
- Cualquier intervalo de fotogramas clave
```
**Para Transmisión (DASH/HLS)**:
```
- MP4 Fragmentado
- Perfil H.264 Main/High
- Audio AAC-LC
- Fragmentos cortos (2-6 segundos)
```
### Compatibilidad
| Plataforma/Dispositivo | Compatibilidad |
|----------------|---------------|
| **Windows Media Player** | ✓ |
| **VLC** | ✓ |
| **Navegadores Web** | ✓ |
| **iOS/iPhone** | ✓ |
| **Android** | ✓ |
| **Smart TVs** | ✓ |
| **Consolas de Juegos** | ✓ |
### Problemas Comunes
**Problema**: El video no se puede buscar en la web
- **Solución**: Habilitar inicio rápido (moov al principio)
**Problema**: Problemas de sincronización de audio
- **Solución**: Usar tasa de fotogramas constante, verificar tasa de muestreo de audio
---

## Contenedor MKV (Matroska)

### Descripción General

Matroska es un formato de contenedor de estándar abierto y rico en funciones.

**Extensiones de Archivo**: `.mkv` (video), `.mka` (audio), `.mks` (subtítulos)

**Tipo MIME**: `video/x-matroska`, `audio/x-matroska`

### Códecs Compatibles

#### Códecs de Video
- H.264/AVC ✓
- H.265/HEVC ✓
- VP8 ✓
- VP9 ✓
- MPEG-4 Parte 2 ✓
- MPEG-2 ✓
- AV1 ✓

#### Códecs de Audio
- AAC ✓
- MP3 ✓
- Opus ✓
- Vorbis ✓
- FLAC ✓
- DTS ✓
- AC-3 ✓
- PCM ✓

### Características

**Características Avanzadas**:
- **Múltiples Pistas de Video**: ✓
- **Múltiples Pistas de Audio**: ✓ (ilimitado)
- **Múltiples Pistas de Subtítulos**: ✓ (ilimitado)
- **Adjuntos**: ✓ (fuentes, carátula)
- **Capítulos**: ✓ (con anidamiento)
- **Etiquetas/Metadatos**: ✓ (extenso)
- **Segmentación**: ✓ (segmentos vinculados)

**Capacidades Técnicas**:
- **Tasa de Fotogramas Variable**: ✓
- **Audio Sin Pérdida**: ✓
- **3D/Estereoscópico**: ✓
- **Metadatos HDR**: ✓

### Mejores Prácticas

**Para Archivo**:
```
- Usar FLAC o PCM para audio sin pérdida
- Incluir todas las pistas de audio/subtítulos
- Agregar marcadores de capítulo
- Incluir etiquetas de metadatos
```

**Para Distribución**:
```
- Video H.264/H.265
- Audio AAC (más compatible)
- Subtítulos suaves incrustados
- Tamaño de archivo razonable
```

**Para Transmisión**:
```
- No ideal para transmisión
- Considerar MP4 o WebM en su lugar
- Si se usa: deshabilitar características complejas
```

### Compatibilidad

| Plataforma/Dispositivo | Compatibilidad |
|----------------|---------------|
| **Windows Media Player** | ✗ (se requiere paquete de códecs) |
| **VLC** | ✓ |
| **Navegadores Web** | ✗ (sin soporte nativo) |
| **iOS/iPhone** | ✗ (solo aplicaciones de terceros) |
| **Android** | Limitado (depende de la aplicación) |
| **Smart TVs** | Limitado (depende del modelo) |
| **Reproductores Multimedia** | ✓ (Kodi, Plex, etc.) |

### Problemas Comunes

**Problema**: La búsqueda es lenta
- **Solución**: Habilitar cues (índice) durante la multiplexación

**Problema**: Tartamudeo en la reproducción con audio de alta calidad
- **Solución**: Verificar rendimiento del decodificador, considerar AAC en lugar de sin pérdida

---
## Contenedor WebM
### Descripción General
WebM es un formato abierto y libre de regalías diseñado para uso web.
**Extensiones de Archivo**: `.webm`
**Tipo MIME**: `video/webm`, `audio/webm`
### Códecs Compatibles
#### Códecs de Video
- VP8 ✓ (WebM 1.0)
- VP9 ✓ (WebM 2.0)
- AV1 ✓ (experimental)
- H.264 ✗
- H.265 ✗
#### Códecs de Audio
- Vorbis ✓ (Principal)
- Opus ✓ (Recomendado)
- AAC ✗
- MP3 ✗
### Características
**Optimizado para Web**:
- **Video HTML5**: ✓ (soporte nativo del navegador)
- **Transmisión**: ✓
- **Transmisión Adaptativa**: ✓ (DASH)
- **Baja Latencia**: ✓
**Soporte de Metadatos**:
- **Etiquetas Básicas**: ✓
- **Capítulos**: ✓
- **Subtítulos**: ✓ (WebVTT)
### Mejores Prácticas
**Para YouTube/Web**:
```
- Códec de video VP9
- Códec de audio Opus (96-160 kbps)
- Intervalo de fotogramas clave: 2-4 segundos
- Codificación de dos pasadas para mejor calidad
```
**Para Transmisión en Vivo**:
```
- VP8 para mejor rendimiento del codificador
- Audio Opus (modo de baja latencia)
- Tasa de bits CBR
- GOP corto
```
**Para Alta Calidad**:
```
- VP9 con alta tasa de bits
- Opus 128-256 kbps
- Codificación de dos pasadas
- Control de tasa basado en calidad
```
### Compatibilidad
| Plataforma/Dispositivo | Compatibilidad |
|----------------|---------------|
| **Chrome** | ✓ |
| **Firefox** | ✓ |
| **Edge** | ✓ |
| **Safari** | Limitado (solo VP8) |
| **Android** | ✓ |
| **iOS** | Limitado |
### Problemas Comunes
**Problema**: Safari no reproduce WebM
- **Solución**: Proporcionar respaldo MP4 con H.264
**Problema**: Codificación demasiado lenta
- **Solución**: Usar VP8 en lugar de VP9, o VP9 acelerado por hardware si está disponible
---

## MPEG-TS (Flujo de Transporte)

### Descripción General

MPEG Transport Stream está diseñado para transmisión y difusión, especialmente donde la resistencia a errores es importante.

**Extensiones de Archivo**: `.ts`, `.mts`, `.m2ts`

**Tipo MIME**: `video/mp2t`

### Códecs Compatibles

#### Códecs de Video
- H.264/AVC ✓
- H.265/HEVC ✓
- MPEG-2 ✓
- VP8/VP9 ✗

#### Códecs de Audio
- AAC ✓
- MP3 ✓
- AC-3 ✓
- PCM ✓

### Características

**Características de Difusión**:
- **Resistencia a Errores**: ✓ (recuperación de errores incorporada)
- **Time-shifting**: ✓
- **Multiplexación de Programas**: ✓ (múltiples programas en un flujo)
- **Cifrado**: ✓ (acceso condicional)

**Características de Transmisión**:
- **Transmisión HLS**: ✓ (Apple HTTP Live Streaming)
- **Difusión DVB**: ✓
- **IPTV**: ✓

### Mejores Prácticas

**Para Transmisión HLS**:
```
- Video H.264
- Audio AAC
- Duración del segmento: 6-10 segundos
- Codificación CBR
- GOP cerrado
```

**Para Difusión**:
```
- MPEG-2 o H.264
- Audio AC-3 o AAC
- Tasa de bits constante
- Tamaño de paquete fijo (188 bytes)
```

### Compatibilidad

| Plataforma/Dispositivo | Compatibilidad |
|----------------|---------------|
| **Reproductores HLS** | ✓ |
| **Decodificadores (Set-top Boxes)** | ✓ |
| **Smart TVs** | ✓ |
| **VLC** | ✓ |
| **Navegadores Web** | Vía soporte HLS |

---
## FLV (Video Flash)
### Descripción General
Formato heredado anteriormente utilizado para video web (YouTube, reproductores Flash).
**Extensiones de Archivo**: `.flv`, `.f4v`
**Tipo MIME**: `video/x-flv`
**Estado**: ⚠️ Obsoleto - Usar MP4 o WebM en su lugar
### Códecs Compatibles
#### Códecs de Video
- H.264 ✓
- VP6 ✓ (heredado)
- Sorenson Spark ✓ (heredado)
#### Códecs de Audio
- AAC ✓
- MP3 ✓
- Speex ✓
### Características
- **Transmisión**: ✓ (RTMP)
- **Metadatos**: Básico (onMetaData)
- **Puntos de Referencia (Cue Points)**: ✓
**No Recomendado**: Fin de vida de Flash Player (2020) hace que FLV sea obsoleto
---

## Contenedor OGG

### Descripción General

Contenedor de código abierto principalmente para audio Vorbis.

**Extensiones de Archivo**: `.ogg`, `.oga` (audio), `.ogv` (video)

**Tipo MIME**: `audio/ogg`, `video/ogg`

### Códecs Compatibles

#### Códecs de Video
- Theora ✓ (calidad heredada)
- VP8 ✗ (usar WebM)

#### Códecs de Audio
- Vorbis ✓ (Principal)
- Opus ✓
- FLAC ✓
- Speex ✓

### Características

- **Transmisión**: ✓
- **Encadenamiento**: ✓ (múltiples archivos en secuencia)
- **Metadatos**: ✓ (comentarios Vorbis)

### Mejores Prácticas

**Para Audio**:
```
- Códec Vorbis u Opus
- Codificación basada en calidad
- Comentarios Vorbis para metadatos
```

**Para Video**:
```
- No recomendado
- Usar WebM (VP8/VP9) en su lugar
```

### Compatibilidad

| Plataforma/Dispositivo | Compatibilidad |
|----------------|---------------|
| **Firefox** | ✓ |
| **Chrome** | ✓ |
| **VLC** | ✓ |
| **La mayoría de dispositivos móviles** | Limitado |

---
## AVI (Audio Video Interleave)
### Descripción General
Formato de contenedor heredado de Microsoft.
**Extensiones de Archivo**: `.avi`
**Tipo MIME**: `video/x-msvideo`
**Estado**: ⚠️ Heredado - Usar MP4 o MKV para nuevos proyectos
### Códecs Compatibles
#### Códecs de Video
- H.264 ✓ (soporte limitado)
- MPEG-4 Parte 2 ✓
- MPEG-2 ✓
- Varios códecs heredados ✓
#### Códecs de Audio
- MP3 ✓
- PCM ✓
- AC-3 ✓
- AAC Limitado
### Limitaciones
- **Tamaño Máximo de Archivo**: 2 GB (sin OpenDML)
- **Metadatos Limitados**: Muy básico
- **Sin Transmisión**: No diseñado para transmisión
- **Sin Capítulos**: No soportado
### Cuándo Usar
- Compatibilidad con sistemas heredados
- Captura desde hardware antiguo
- Requisitos de software específicos
**Recomendación**: Usar MP4 o MKV para nuevos proyectos
---

## Contenedor WAV

### Descripción General

Contenedor de audio sin comprimir.

**Extensiones de Archivo**: `.wav`

**Tipo MIME**: `audio/wav`, `audio/x-wav`

### Características

- **Sin Pérdida**: ✓ (PCM)
- **Comprimido**: ✓ (MP3, AAC en envoltorio WAV)
- **Metadatos**: Limitado (etiquetas RIFF)

### Formatos Comunes

- **PCM 44.1 kHz 16-bit**: Calidad CD
- **PCM 48 kHz 24-bit**: Audio profesional
- **PCM 96 kHz 24-bit**: Audio de alta resolución

### Mejores Prácticas

**Para Producción de Audio**:
```
- PCM 48 kHz, 24-bit
- Mono o estéreo
- Evitar compresión
```

**Para Distribución**:
```
- Usar FLAC o AAC en su lugar
- Los archivos WAV son grandes
```

---
## Guía de Selección de Contenedores
### Para Entrega Web
**Principal**: MP4 (H.264 + AAC)
- **Razón**: Compatibilidad universal
- **Respaldo**: WebM (VP9 + Opus) para navegadores modernos
### Para Archivo Profesional
**Principal**: MKV (H.265 + FLAC)
- **Razón**: Rico en funciones, soporte de audio sin pérdida
- **Alternativa**: MP4 (H.265 + AAC) para mejor compatibilidad
### Para Difusión/IPTV
**Principal**: MPEG-TS (H.264 + AAC)
- **Razón**: Resistencia a errores, estándar de la industria
- **Alternativa**: MPEG-TS (MPEG-2 + AC-3) para sistemas heredados
### Para Transmisión en Vivo
**HLS**: Segmentos MPEG-TS (H.264 + AAC)
**DASH**: MP4 Fragmentado (H.264 + AAC)
**WebRTC**: Audio Opus, video VP8/H.264
### Solo Audio
**Alta Calidad**: FLAC (.flac) o MP3 VBR (.mp3)
**Transmisión**: AAC en MP4 (.m4a) u Opus en WebM
**Voz**: Opus en OGG o Speex
---

## Tabla de Comparación de Formatos

| Característica | MP4 | MKV | WebM | MPEG-TS | FLV | OGG |
|---------|-----|-----|------|---------|-----|-----|
| **Compatibilidad Web** | ★★★★★ | ★☆☆☆☆ | ★★★★☆ | ★★☆☆☆ | ☆☆☆☆☆ | ★★☆☆☆ |
| **Compatibilidad Móvil** | ★★★★★ | ★★☆☆☆ | ★★★☆☆ | ★★★★☆ | ☆☆☆☆☆ | ★☆☆☆☆ |
| **Soporte de Transmisión** | ★★★★★ | ★★☆☆☆ | ★★★★★ | ★★★★★ | ★★★☆☆ | ★★★☆☆ |
| **Riqueza de Funciones** | ★★★★☆ | ★★★★★ | ★★★☆☆ | ★★★☆☆ | ★★☆☆☆ | ★★☆☆☆ |
| **Soporte de Códecs** | ★★★★☆ | ★★★★★ | ★★☆☆☆ | ★★★☆☆ | ★★☆☆☆ | ★★★☆☆ |
| **Eficiencia de Tamaño de Archivo** | ★★★★☆ | ★★★★☆ | ★★★★★ | ★★★☆☆ | ★★★☆☆ | ★★★★☆ |
| **Resistencia a Errores** | ★★☆☆☆ | ★★☆☆☆ | ★★☆☆☆ | ★★★★★ | ★★★☆☆ | ★★☆☆☆ |

---
## Especificaciones Técnicas
### Estructura MP4
```
ftyp (tipo de archivo)
moov (metadatos - colocar al principio para inicio rápido)
  ├── mvhd (encabezado de película)
  ├── trak (pista de video)
  ├── trak (pista de audio)
  └── udta (datos de usuario/metadatos)
mdat (datos multimedia)
```
### MP4 Fragmentado (para transmisión)
```
ftyp
moov
  └── mvex (extensión de película)
moof (fragmento de película)
  └── traf (fragmento de pista)
mdat (datos de fragmento)
[repetir moof/mdat para cada fragmento]
```
### Estructura MKV
```
Encabezado EBML
Segmento
  ├── SeekHead (índice)
  ├── Info (información del segmento)
  ├── Tracks (definiciones de pista)
  ├── Chapters (opcional)
  ├── Attachments (opcional)
  ├── Tags (metadatos)
  └── Cluster (datos multimedia)
```
---

## Ver También

- [Descripción General del Paquete de Filtros de Codificación](index.md)
- [Referencia de Códecs](codecs-reference.md)
- [Ejemplos de Código](examples.md)
- [Referencia de Interfaz NVENC](interfaces/nvenc.md)
