---
title: Fuentes de Video para Desarrolladores .NET
description: Domine fuentes de entrada de video para .NET incluyendo webcams, Decklink, IP cameras, captura de pantalla, y cámaras industriales con integración.
sidebar_label: Fuentes de Video
order: 16

---

# Fuentes de Video para Desarrolladores .NET

[SDK de Captura de Video .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a Fuentes de Entrada de Video

El SDK de Captura de Video para .NET proporciona soporte robusto para prácticamente cualquier fuente de entrada de video estándar disponible en entornos de desarrollo modernos. Esta flexibilidad permite a las aplicaciones capturar, procesar y manipular contenido de video desde una amplia variedad de dispositivos hardware y streams de red.

Ya sea que esté desarrollando software de edición de video profesional, creando soluciones de vigilancia personalizadas o construyendo aplicaciones de imágenes médicas, entender las opciones de fuente de video disponibles es crucial para implementar la solución correcta para sus requisitos específicos.

## Webcams USB e Integradas

### Compatibilidad de Dispositivos

El SDK soporta todos los dispositivos de captura de video estándar que cumplen con interfaces de driver comunes, incluyendo:

- Webcams USB (USB 2.0, 3.0 y conectadas USB-C)
- Cámaras integradas de laptop y tablet
- Adaptadores y dongles de captura de video USB externos
- Dispositivos de software de cámara virtual

### Características de Implementación

Cuando se trabaja con webcams USB e integradas, los desarrolladores pueden:

- Acceder y enumerar todos los dispositivos conectados
- Seleccionar desde formatos de video y resoluciones disponibles
- Controlar parámetros específicos de cámara (enfoque, exposición, balance de blancos)
- Aplicar filtros de procesamiento de video en tiempo real
- Capturar frames raw para análisis de imagen personalizado
- Configurar brillo, contraste y color programáticamente

## Hardware Profesional Blackmagic Decklink

### Modelos y Características Soportados

El SDK proporciona integración nativa con hardware de captura de video profesional Blackmagic Design's Decklink:

- Soporte completo para toda la línea de productos Decklink:
  - Serie Decklink Mini (captura compacta, rentable)
  - Modelos Decklink Studio (funcionalidad broadcast de rango medio)
  - Serie Decklink 4K y 8K (producción de alta resolución)
  - Variantes Decklink Duo y Quad (captura multi-entrada)

### Capacidades Técnicas

- Soporte tanto para conexiones SDI (Serial Digital Interface) como HDMI
- Compatible con resoluciones broadcast estándar:
  - SD (PAL/NTSC)
  - HD (720p, 1080i, 1080p)
  - UHD/4K (2160p)
  - 8K donde el hardware lo soporte
- Acceso a todos los canales de audio embebidos (hasta 16 canales)
- Interpretación y sincronización de timecode
- Control de buffer de frame para rendimiento consistente de captura
- Acceso a metadatos de video y datos auxiliares

## Fuentes de Video de Red

### Soporte de IP Camera y Stream

El SDK permite a las aplicaciones conectarse directamente a fuentes de video en red:

- Streams RTSP (Real-Time Streaming Protocol) con varias opciones de transporte:
  - Transporte UDP (baja latencia, potencialmente menos confiable)
  - Transporte TCP (confiable, potencialmente latencia más alta)
  - Tunelización HTTP para cruce de firewall
- Fuentes RTMP (Real-Time Messaging Protocol):
  - Soporte para streams en vivo
  - Compatibilidad con servidores RTMP
  - Compatibilidad con Flash Media Server
- Streaming basado en HTTP:
  - Streams MJPEG
  - Fuentes de descarga progresiva HTTP
- Formatos de streaming estándar de la industria:
  - HLS (HTTP Live Streaming)
  - DASH (Dynamic Adaptive Streaming over HTTP)
  - SRT (Secure Reliable Transport)
- Integración WebRTC para comunicación de video basada en navegador

### Detalles de Implementación

- Soporte de autenticación para streams asegurados (Basic, Digest, NTLM)
- Ajustes de buffer configurables para equilibrar latencia vs. suavidad
- Lógica de reconexión automática para condiciones de red inestables
- Técnicas de cruce NAT para entornos de red complejos
- Estadísticas de tráfico para monitoreo de uso de ancho de banda
- Adaptación multi-bitrate para condiciones de red variables

## Captura de Pantalla y Ventana

### Opciones de Captura de Escritorio

Para escenarios que requieren grabación de pantalla o captura de aplicación:

- Captura de contenido de pantalla completa con soporte para:
  - Configuraciones de monitor único
  - Configuraciones multi-monitor con objetivos de pantalla selectables
  - Varias opciones de escalado para optimizar rendimiento
- Capacidades de captura específica de ventana:
  - Captura por handle de ventana
  - Targeting específico de aplicación
  - Captura de ventana sin bordes
- Selección de región de interés:
  - Selección de área rectangular personalizada
  - Seguimiento de región dinámica
  - Posicionamiento basado en coordenadas

### Implementación Técnica

- Captura acelerada por hardware donde esté disponible
- Inclusión/exclusión de cursor opcional
- Control de tasa de frames para equilibrar calidad vs. carga del sistema
- Opciones de visualización de clics del mouse para grabaciones de tutoriales
- Compatibilidad con contenido DirectX/OpenGL
- Manejo de ventanas en capas para composiciones de escritorio complejas

## Dispositivos Heredados y Especializados

### Integración de Camcorder DV

El SDK mantiene soporte para cámaras de formato Digital Video (DV):

- Conectividad FireWire/IEEE 1394
- Compatibilidad con formatos DV, DVCAM y HDV estándar
- Captura frame-accurate con preservación de timecode
- Características de control de dispositivo (cuando soportado por hardware):
  - Controles de play/pause/stop
  - Funciones de fast-forward y rewind
  - Inicio de grabación

### Cámaras Industriales y Científicas

Para escenarios de desarrollo especializados, el SDK soporta cámaras de visión industrial a través de múltiples estándares:

- Dispositivos compatibles con USB3 Vision presentando:
  - Adquisición de imagen de alta velocidad
  - Descubrimiento y enumeración de características de dispositivo
  - Manejo de eventos para captura activada
- Hardware compatible con GigE Vision con:
  - Protocolos de descubrimiento de red
  - Streaming de imagen de alto ancho de banda
  - Acceso de configuración de dispositivo
- Interfaz estándar GenICam:
  - Convenciones de nomenclatura de parámetros estandarizadas
  - Consistencia de acceso a características entre fabricantes
  - Configuración de dispositivo basada en descriptor XML
- Control sobre parámetros especializados de cámara:
  - Ajustes de exposición y ganancia
  - Opciones de triggering (software/hardware)
  - Definición de región de interés (ROI)
  - Varios formatos de píxel y profundidades de bit

## Optimización de Rendimiento Técnicas

Cuando se trabaja con fuentes de video, las consideraciones de rendimiento son críticas. El SDK proporciona varias rutas de optimización:

- Opciones de aceleración por hardware:
  - Procesamiento acelerado DirectX
  - Codificación/decodificación acelerada por GPU
  - Utilización de instrucciones SIMD
- Estrategias de manejo de memoria:
  - Pooling de buffers para reducir sobrecarga de asignación
  - Acceso directo a memoria donde esté soportado
  - Buffers de frame pre-asignados
- Procesamiento multi-hilo:
  - Procesamiento paralelo de frames de video
  - Utilización de pool de hilos para cadenas de filtros
  - Procesamiento en background para escenarios no en tiempo real

## Conclusión

El amplio soporte de fuente de video en SDK de Captura de Video .NET empodera a los desarrolladores para crear aplicaciones de video versátiles con restricciones mínimas en hardware de entrada. Al entender las capacidades y limitaciones de cada tipo de fuente, puede diseñar sistemas de procesamiento de video más efectivos y eficientes.

Para referencias detalladas de API y ejemplos de implementación para tipos específicos de fuente de video, refiérase a la documentación de clase y guías de método en los materiales de referencia del SDK.

---

Visite nuestra página [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más muestras de código.