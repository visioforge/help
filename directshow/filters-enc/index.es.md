---
title: Paquete de Filtros de Codificación DirectShow
description: Codificadores DirectShow de video/audio con H.264, H.265, VP8, VP9, AAC, MP3 y aceleración GPU (NVENC, QuickSync, AMF) para aplicaciones Windows.
---

# Paquete de Filtros de Codificación DirectShow

## Introducción

El Paquete de Filtros de Codificación DirectShow proporciona un potente conjunto de componentes de codificación de medios diseñados específicamente para desarrolladores de software que construyen aplicaciones multimedia profesionales. Este kit de herramientas permite la integración fluida de capacidades de codificación de alto rendimiento para streams de audio y video en una amplia variedad de formatos populares.

---

## Instalación

Antes de usar los ejemplos de código e integrar los filtros en su aplicación, primero debe instalar el Paquete de Filtros de Codificación DirectShow desde la [página del producto](https://www.visioforge.com/encoding-filters-pack).

**Pasos de Instalación**:

1. Descargue el instalador del Paquete de Filtros de Codificación desde la página del producto
2. Ejecute el instalador con privilegios administrativos
3. El instalador registrará todos los filtros de codificación y muxing
4. Las aplicaciones de ejemplo y código fuente estarán disponibles en el directorio de instalación

**Nota**: Todos los filtros deben estar correctamente registrados en el sistema antes de poder usarlos en sus aplicaciones. El instalador maneja esto automáticamente.

---

## Características Principales

### Soporte de Codificación Multi-Formato

El paquete de filtros soporta numerosos formatos estándar de la industria, incluyendo:

- **Contenedor MP4** con códecs H264, HEVC y AAC
- Streams **MPEG-TS**
- Contenedores **MKV** (Matroska)
- Formato **WebM** con códecs de video VP8/VP9
- Múltiples formatos de audio incluyendo **Vorbis**, **MP3**, **FLAC** y **Opus**

### Aceleración por Hardware

Los desarrolladores pueden aprovechar la aceleración GPU para mejorar el rendimiento de codificación:

- Tecnología **Intel** QuickSync
- Aceleración por hardware **AMD/ATI**
- Soporte de codificación **Nvidia** NVENC

Esta optimización por hardware mejora dramáticamente las velocidades de codificación mientras reduce la carga de CPU en sus aplicaciones.

### Opciones de Implementación Flexibles

El paquete incluye:

- Codificadores H264/AAC independientes utilizando recursos de CPU
- Componentes muxer especializados con codificadores de video y audio integrados
- Opciones para rutas de codificación basadas en CPU y GPU

## Capacidades Técnicas

Los componentes de filtros se integran perfectamente en pipelines de aplicaciones DirectShow, proporcionando a los desarrolladores:

- Codificación de video de alta calidad a varias tasas de bits y resoluciones
- Compresión de audio eficiente con ajustes de calidad configurables
- Soporte de formatos de contenedor avanzado con parámetros personalizables
- Compatibilidad con grafos de filtros DirectShow para implementación directa

Para especificaciones detalladas y una lista completa de todos los codificadores de video/audio soportados y formatos de salida, visite la [página del producto](https://www.visioforge.com/encoding-filters-pack).

## Historial de Versiones

### Versión 11.4

- Componentes de filtros actualizados para coincidir con implementaciones actuales del SDK .Net
- Codificadores AMD AMF H264/H265 mejorados con últimas optimizaciones
- Codificadores Intel QuickSync H265 mejorados para mejor rendimiento
- Aplicaciones de ejemplo actualizadas con nuevos ejemplos de código

### Versión 11.0

- Filtros sincronizados con versiones actuales del SDK .Net
- Codificadores Nvidia NVENC H264/H265 actualizados para mejor calidad
- Introducido nuevo componente de filtro muxer SSF

### Versión 10.0

- Todos los filtros actualizados para alinear con implementaciones del SDK .Net
- Codificadores Media Foundation mejorados (H264, H265, AAC)
- Agregado filtro de codificador de video NVENC dedicado como reemplazo del codificador CUDA

### Versión 9.0

- Contenedor MP4 optimizado con salida H264/AAC
- Soporte de formato WebM expandido con capacidades de codificación VP9
- Rendimiento del filtro codificador H265 mejorado
- Codificadores Intel QuickSync H264 mejorados

### Versión 8.6

- Implementado filtro sink RTSP para aplicaciones de streaming
- Agregado filtro sink RTMP en estado BETA
- Filtro codificador AAC actualizado con mejoras de calidad

### Versión 8.5 - Lanzamiento Inicial

- Primer lanzamiento público incluyendo filtros de SDKs .Net
- Componentes principales: codificador AAC, codificadores H264 (CPU/GPU)
- Codificadores adicionales: H265 (CPU/GPU), VP8, Vorbis
- Soporte de contenedor: muxer MP4, muxer WebM
- Capacidades de streaming: fuente RTSP, fuente RTMP

---

## Recursos

- [Página del Producto](https://www.visioforge.com/encoding-filters-pack) - Compra, licenciamiento e información del producto
- [Ejemplos de Código](https://github.com/visioforge/directshow-samples/tree/main/Encoding%20Filters%20Pack) - Aplicaciones de ejemplo y ejemplos de implementación

---

## Ver También

- [Referencia de Códecs](codecs-reference.md) - Documentación completa de códecs de video y audio
- [Referencia de Muxers](muxers-reference.md) - Especificaciones de formatos de contenedor
- [Interfaz NVENC](interfaces/nvenc.md) - API del codificador hardware NVIDIA
- [Ejemplos de Código](examples.md) - Ejemplos prácticos de codificación
