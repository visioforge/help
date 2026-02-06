---
title: Historial de Versiones de la Biblioteca TVFVideoCapture
description: Historial de versiones de TVFVideoCapture: aceleración GPU, streaming y actualizaciones desde la versión 4.1 hasta la 11.0.
---

# Historial de Versiones de TVFVideoCapture

## Versión 11.00 - Codificación GPU Mejorada y Soporte para Delphi Moderno

- **Compatibilidad de Framework Expandida**: Añadido soporte para entornos de desarrollo Delphi 10.4 y 11.0
- **Aceleración GPU AMD Avanzada**: Implementada codificación de video MP4 (H264/AAC) utilizando unidades de procesamiento gráfico AMD
- **Codificación Hardware GPU Intel**: Añadida codificación de video MP4 (H264/AAC) a través de GPUs Intel integradas y discretas
- **Aceleración NVIDIA CUDA**: Introducida codificación de video MP4 (H264/AAC) potenciada por hardware gráfico NVIDIA
- **Mejoras en Formato de Contenedor**: MKV mejorado con rendimiento y fiabilidad optimizados
- **Nuevo Formato de Salida**: Añadido soporte para formato contenedor MOV para compatibilidad con ecosistema Apple

## Versión 10.0 - Optimizaciones de Rendimiento y Soporte Multi-Plataforma

- **Mejora MP4**: Capacidades de salida MP4 actualizadas y mejoradas exhaustivamente
- **Mejoras de Streaming**: Filtro de fuente VLC actualizado con soporte mejorado para RTMP y HTTPS
- **Gestión de Memoria**: Corregida fuga de memoria crítica del codificador CUDA para codificación estable de larga duración
- **Optimización de Recursos**: Resuelta fuga de memoria de fuente FFMPEG para estabilidad de aplicación mejorada
- **Captura de Audio**: Filtro What You Hear mejorado para grabación superior de audio del sistema
- **Arquitectura de 64 bits**: Añadida fuente VLC x64 para TVFMediaPlayer y TVFVideoCapture (tanto Delphi como ActiveX)
- **Soporte de Formato Extendido**: Filtro YUV2RGB mejorado con soporte de formato HDYC
- **Codificación de Audio**: Codificador LAME actualizado con corrección para problemas de audio mono de baja tasa de bits
- **Entorno de Desarrollo**: Añadido soporte para Delphi 10, 10.1 para flujos de trabajo de desarrollo moderno

## Versión 8.7 - Actualizaciones del Motor Principal

- **Integración VLC**: Motor VLC actualizado a libVLC 2.2.1.0 para capacidades de streaming mejoradas
- **Mejora de Decodificador**: Motor FFMPEG actualizado para mejor compatibilidad de formatos y rendimiento

## Versión 8.6 - Mejoras de Fiabilidad y Soporte de Formatos

- **Gestión de Recursos**: Corregida fuga de memoria crítica para estabilidad de aplicación mejorada
- **Manejo de Archivos**: Resueltos problemas con archivos de entrada y salida cerrados incorrectamente
- **Nuevo Soporte de Formato**: Añadidos filtros WebM personalizados basados en las especificaciones del proyecto WebM

## Versión 8.4 - Expansión de Arquitectura

- **Delphi Moderno**: Añadido soporte para Delphi XE8 para los entornos de desarrollo más recientes
- **Arquitectura de 64 bits**: Introducidas versiones Delphi y ActiveX x64 para rendimiento en sistemas modernos

## Versión 8.31 - Actualización del Entorno de Desarrollo

- **Compatibilidad de Framework**: Añadido soporte para Delphi XE7 para opciones de desarrollo expandidas

## Versión 8.3 - Mejoras de API y Rendimiento

- **Mejora de Interfaz**: API ActiveX actualizada para experiencia de desarrollador mejorada
- **Optimización de Decodificador**: Decodificador FFMPEG mejorado para mejor rendimiento y soporte de formatos
- **Estabilidad**: Implementadas varias correcciones de errores críticos y mejoras de rendimiento

## Versión 8.0 - Capacidades de Streaming

- **Streaming de Red**: Introducido motor VLC para capacidades de captura de video IP
- **Fiabilidad**: Corregidos varios errores para estabilidad mejorada en todos los componentes

## Versión 7.15 - Opciones de Salida Avanzadas y Seguridad

- **Captura de Red**: Motor de captura IP mejorado para mejor estabilidad de conexión y rendimiento
- **Soporte de Formato Moderno**: Añadida salida MP4 con H264/AAC para compatibilidad de estándar de la industria
- **Característica de Seguridad**: Implementado cifrado de video para flujos de trabajo de contenido protegido
- **Integración de Sistema**: Añadida salida de Cámara Virtual para escenarios de integración de software
- **Estabilidad**: Múltiples pequeñas correcciones de errores para fiabilidad mejorada

## Versión 7.0 - Mejoras del Motor de Captura

- **Rendimiento de Red**: Motor de captura IP mejorado con rendimiento y fiabilidad mejorados
- **Captura de Escritorio**: Motor de captura de pantalla actualizado para mejor rendimiento y calidad
- **Opciones de Salida**: Salida FFMPEG mejorada para soporte de formato expandido
- **Efectos Visuales**: Añadido efecto de video Pan/zoom para manipulación avanzada de video
- **Fiabilidad**: Implementadas múltiples pequeñas correcciones de errores para estabilidad mejorada

## Versión 6.0 - Multi-Fuente y Compatibilidad con Windows 8

- **Composición Avanzada**: Picture-In-Picture mejorado con soporte para cualquier fuente de video incluyendo captura de pantalla y cámaras IP
- **Protocolo de Streaming**: Soporte de fuentes RTSP mejorado para mejor integración de video de red
- **Modo de Captura Especial**: Añadido soporte de captura de pantalla de ventanas en capas para grabación de UI compleja
- **Soporte de Hardware**: Implementado soporte de cámaras iCube para aplicaciones de imagen especializadas
- **Compatibilidad de SO**: Añadido soporte para Windows 8 Developer Preview para compatibilidad hacia adelante
- **Procesamiento Visual**: Efectos de video mejorados con nuevas opciones y rendimiento mejorado
- **Gestión de Audio**: Introducido soporte de múltiples streams de audio para salidas AVI y WMV

## Versión 5.5 - Mejoras de Estabilidad y Características

- **Procesamiento Visual**: Efectos de video mejorados con calidad y rendimiento mejorados
- **Video de Red**: Soporte de cámaras IP mejorado para mejor conectividad y compatibilidad
- **Fiabilidad**: Corregidos varios errores para estabilidad general mejorada

## Versión 5.4 - Soporte para Delphi Moderno

- **Entorno de Desarrollo**: Añadido soporte para Delphi XE2 para desarrollo de aplicaciones modernas
- **Estabilidad**: Implementadas varias correcciones de errores para fiabilidad mejorada

## Versión 5.3 - Mejoras de Procesamiento de Video

- **Efectos Visuales**: Efectos de video mejorados con opciones adicionales y mejor rendimiento
- **Video de Red**: Soporte de cámaras IP mejorado para compatibilidad de dispositivos más amplia
- **Fiabilidad**: Corregidos múltiples errores para operación más estable

## Versión 5.2 - Mejoras de Procesamiento de Fotogramas

- **Efectos Visuales**: Efectos de video mejorados y funcionalidad de captura de fotogramas de video
- **Estabilidad**: Corregidos varios errores para fiabilidad mejorada

## Versión 5.1 - Mejoras de Video de Red y Efectos

- **Integración de Cámara IP**: Soporte de cámara IP mejorado para conectividad mejorada
- **Procesamiento Visual**: Calidad y rendimiento de efectos de video mejorados
- **Fiabilidad**: Corregidos varios problemas para mejor estabilidad

## Versión 5.0 - Expansión Mayor de Soporte de Formatos

- **Video de Red**: Añadido soporte de cámara IP RTSP/HTTP (MJPEG/MPEG-4/H264 con o sin audio)
- **Formato Moderno**: Implementada salida WebM para compatibilidad de estándares web abiertos
- **Flexibilidad de Formato**: Añadida salida MPEG-1/2/4 y FLV usando integración FFMPEG

## Versión 4.22 - Mejoras de Captura de Pantalla

- **Grabación de Escritorio**: Corregidos errores en filtro de captura de pantalla para calidad de grabación mejorada

## Versión 4.21 - Mejoras de Captura de Pantalla

- **Grabación de Escritorio**: Implementadas múltiples correcciones de errores y mejoras en filtro de captura de pantalla

## Versión 4.2 - Mejora de Procesamiento de Audio

- **Efectos de Sonido**: Filtro de efectos de audio mejorado con calidad y rendimiento mejorados

## Versión 4.1 - Integración de Delphi Moderno

- **Entorno de Desarrollo**: Añadido soporte para Delphi 2010 para la edición Delphi
- **Estabilidad**: Corregidos varios errores para fiabilidad mejorada
