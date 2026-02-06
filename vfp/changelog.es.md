---
title: Notas de Versión del SDK de Video Fingerprinting
description: Evolución del SDK de Video Fingerprinting: historial de versiones con nuevas características, actualizaciones de rendimiento y mejoras técnicas.
---

# Historial de Versiones del SDK de Video Fingerprinting

## Versión 12.1 - Mejoras de Rendimiento y Características

### Mejoras en .NET Framework

* **Nueva Capacidad de Fingerprinting**: Se introdujo la clase `VFPFingerprintFromFrames` que permite a los desarrolladores generar fingerprints de video directamente desde secuencias de cuadros RGB24
* **Modernización de API**: Implementación completamente renovada de API async/await para mejor procesamiento asíncrono
* **Optimización del Motor**: Se mejoró significativamente el rendimiento del motor de fingerprinting principal con algoritmos de procesamiento mejorados

## Versión 12.0 - Integración de Base de Datos y Aceleración por Hardware

### Actualizaciones de .NET Framework

* **Almacenamiento Multi-fingerprint**: Se agregó nueva clase `VFPFingerPrintDB` para almacenar eficientemente múltiples fingerprints en un formato de archivo binario único
* **Integración de Herramienta de Monitoreo de Medios**: Se actualizó la aplicación Media Monitoring Tool para aprovechar las nuevas capacidades de base de datos
* **Dependencias Actualizadas**: Se integró la última versión de FFMPEG para capacidades mejoradas de manejo de video
* **Cambio de Requisito de Framework**: Se aumentó el requisito mínimo de .NET Framework a la versión 4.7.2
* **Registro Externo**: Se agregó NLog como dependencia externa para capacidades de registro mejoradas
* **Aceleración GPU**: Soporte mejorado para aceleración por hardware vía decodificadores de video GPU Nvidia, Intel y AMD

## Versión 11.0 - Modernización del Motor

### Implementación .NET

* **Instalación Independiente**: Se lanzó como paquete instalador independiente sin requerir otras instalaciones de SDK .NET
* **Motor de Fuente de Video**: Se implementó nuevo motor para procesar video desde archivos y fuentes de red
* **Soporte de Dispositivo de Captura**: Se desarrolló nuevo motor para manejar video desde dispositivos de captura
* **Mejoras del Núcleo**: Se actualizó el motor de fingerprinting con los algoritmos más recientes

### Soporte C++ para Linux

* **Resolución de Errores**: Se corrigieron múltiples problemas que afectaban las implementaciones de Linux
* **Actualizaciones del Motor**: Se mejoró el motor de fingerprinting con optimizaciones específicas de plataforma

## Versión 10.0 - Características de Personalización

### Mejoras de .NET

* **Control de Resolución**: Se agregaron opciones de resolución personalizada para video fuente
* **Funcionalidad de Recorte**: Se implementaron capacidades de recorte personalizado para material fuente
* **Actualizaciones del Motor**: Se actualizaron los motores de decodificación y fingerprinting

### Mejoras C++ para Linux

* **Aplicación Demo**: Se actualizó la demo Media Monitoring Tool con la última compatibilidad FFMPEG
* **Mejoras de Estabilidad**: Se resolvieron varios errores que afectaban el rendimiento

## Versión 3.1 - Versión de Optimización

### Mejoras Generales

* **Correcciones de Errores**: Se abordaron problemas menores que afectaban la estabilidad general
* **Actualizaciones del Motor**: Se mejoró el motor de procesamiento para implementación .NET
* **Cambio de Licencia**: Las herramientas Media Monitoring Tool (Live) y Duplicates Video Search ahora están disponibles para uso comercial gratuito

## Versión 3.0 - Primer Lanzamiento Público

### Características Clave

* Primer lanzamiento público del SDK de Video Fingerprinting
* Se introdujeron capacidades principales de fingerprinting para identificación de contenido de video
* Se estableció la base para desarrollo multiplataforma
