---
title: Captura de Audio para Desarrolladores .NET
description: Implemente grabación de micrófono, entrada de línea y audio en streaming en .NET con soporte de formatos, procesamiento y monitoreo en tiempo real.
sidebar_label: Captura de Audio
order: 10

---

# Captura de Audio para Desarrolladores .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Captura de Audio

Nuestro SDK proporciona capacidades robustas de captura de audio diseñadas específicamente para desarrolladores .NET. Ya sea que esté construyendo una aplicación de grabación profesional, agregando chat de voz a su software o creando una herramienta de podcasting, nuestros componentes de captura de audio ofrecen un rendimiento y flexibilidad excepcionales.

La funcionalidad de captura de audio le permite grabar desde cualquier dispositivo de entrada de audio en el sistema, incluyendo micrófonos, puertos de entrada de línea y dispositivos de audio virtuales. Todo el procesamiento está optimizado para un uso mínimo de CPU mientras se mantiene una calidad de audio prístina.

## Fuentes de Audio Soportadas

El SDK soporta la captura desde múltiples fuentes de audio:

- **Micrófonos físicos** - Micrófonos de escritorio, USB y Bluetooth
- **Puertos de entrada de línea** - Para capturar desde mezcladores externos o instrumentos
- **Dispositivos de audio virtuales** - Capture audio de otras aplicaciones
- **Audio del sistema** - Grabe lo que se reproduce a través de sus altavoces
- **Flujos de red** - Capture audio de RTSP, HTTP y otras fuentes de transmisión

## Soporte de Formatos de Audio

Nuestro SDK le permite capturar y codificar audio en varios formatos estándar de la industria para satisfacer cualquier requisito:

### Formatos con Pérdida

- [MP3](../../general/audio-encoders/mp3.md) - Audio comprimido estándar de la industria con tasas de bits ajustables de 8kbps a 320kbps
- [M4A (AAC)](../../general/audio-encoders/aac.md) - Codificación de Audio Avanzada con excelente relación calidad-tamaño
- [Windows Media Audio](../../general/audio-encoders/wma.md) - Formato de audio de Microsoft con buena compresión e integración con Windows
- [Ogg Vorbis](../../general/audio-encoders/vorbis.md) - Formato libre y de código abierto con excelente calidad a tasas de bits más bajas
- [Speex](../../general/audio-encoders/speex.md) - Optimizado para voz con buena calidad a tasas de bits muy bajas

### Formatos sin Pérdida

- [WAV](../../general/audio-encoders/wav.md) - Audio sin comprimir con calidad perfecta y amplia compatibilidad
- [FLAC](../../general/audio-encoders/flac.md) - Códec de Audio Sin Pérdida Libre que proporciona compresión sin pérdida de calidad

## Características Clave

### Control de Dispositivos

- Enumerar todos los dispositivos de entrada de audio disponibles
- Seleccionar dispositivos de entrada específicos programáticamente
- Establecer niveles de volumen de entrada y estado de silencio
- Monitorear niveles de audio en tiempo real
- Selección automática de dispositivos predeterminados del sistema

### Procesamiento Avanzado

- Visualización de audio en tiempo real con análisis de espectro y forma de onda
- Reducción de ruido y cancelación de eco
- Control de ganancia y normalización
- Detección de actividad de voz (VAD)
- Gestión de canales estéreo/mono
- Conversión de frecuencia de muestreo

### Controles de Grabación

- Iniciar, pausar, reanudar y detener grabación
- Gestión de búfer para operación de baja latencia
- Grabaciones temporizadas con parada automática
- División de archivos para grabaciones grandes
- Nombramiento automático de archivos con marcas de tiempo
- Perfiles de grabación para configuración rápida

## Mejores Prácticas

Para una captura de audio óptima en sus aplicaciones:

1. **Siempre verifique la disponibilidad del dispositivo** antes de iniciar la captura
2. **Monitoree los niveles de audio** durante la grabación para detectar silencio o saturación
3. **Use formatos apropiados** basados en sus requisitos de calidad y tamaño de archivo
4. **Implemente manejo de errores** para eventos de desconexión de dispositivos
5. **Proporcione retroalimentación visual** a los usuarios durante la grabación
6. **Pruebe en varios hardware** para asegurar compatibilidad
7. **Aplique reducción de ruido** solo cuando sea necesario ya que puede afectar la calidad del audio

## Integración de Captura de Audio

El componente de captura de audio se integra perfectamente con otros elementos del SDK:

- Combine con captura de video para grabación AV completa
- Mezcle con reproducción de audio para aplicaciones de grabación de llamadas
- Use con componentes de transmisión para radiodifusión en vivo
- Integre con editor de línea de tiempo para edición de audio básica
- Empareje con conversión de archivos para flujos de trabajo de post-procesamiento

## Consideraciones de Rendimiento

El SDK está optimizado para eficiencia, pero aquí hay algunos consejos para el mejor rendimiento:

- Tasas de muestreo más bajas (44.1kHz vs 48kHz) reducen el uso de CPU
- La grabación mono usa menos potencia de procesamiento que estéreo
- La codificación MP3 es más intensiva en CPU que la grabación WAV
- Tasas de bits más altas requieren más potencia de procesamiento
- Los tamaños de búfer afectan la latencia y estabilidad

---

Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más muestras de código y ejemplos de implementación.
