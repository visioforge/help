---
title: Historial de Versiones de Video Edit SDK FFMPEG .Net
description: Historial de versiones detallado y actualizaciones de características para Video Edit SDK FFMPEG .Net con mejoras de rendimiento y cambios de API.
---

# Video Edit SDK FFMPEG .Net: Historial Completo de Versiones

## Novedades en la Versión 12.1

Nuestra última versión trae mejoras significativas en flexibilidad de despliegue y compatibilidad de frameworks:

### Actualización de .Net Framework

* Migración completa al framework .Net 4.6 asegurando mejor rendimiento y compatibilidad con sistemas modernos
* Confiabilidad mejorada en tiempo de ejecución con componentes principales actualizados

### Modelo de Distribución Simplificado

* Paquete instalador unificado para versiones TRIAL y FULL, simplificando el proceso de despliegue
* Paquetes NuGet idénticos a través de niveles de licenciamiento, eliminando confusión entre versiones

### Desarrollo Multiplataforma

* Paquetes .Net Core y .Net Framework consolidados en una única distribución unificada
* Gestión simplificada de dependencias a través de diferentes plataformas objetivo

### Mejoras de Despliegue

* Añadidos paquetes redistribuibles NuGet para gestión más fácil de dependencias
* Proceso de despliegue simplificado con manejo automático de referencias
* Complejidad de configuración reducida para aplicaciones empresariales

## Destacados de la Versión 11.3

Esta versión se enfocó en capacidades de audio principales y soporte multiplataforma:

### Mejora de Audio

* Efectos de fade-in/fade-out de audio completamente rediseñados para transiciones más suaves
* Rendimiento de algoritmo mejorado en procesadores multi-núcleo
* Estabilidad mejorada del pipeline de procesamiento de audio

### Actualizaciones de Framework

* Añadido soporte completo de .Net Core para desarrollo multiplataforma
* Compatibilidad hacia atrás mantenida con implementaciones existentes de .Net Framework
* Optimizaciones de rendimiento para ambos entornos de ejecución

### Mejoras Técnicas

* Serializador JSON integrado actualizado con mejor manejo de objetos complejos
* Gestión de memoria mejorada para tareas de procesamiento de medios grandes
* Corregidos problemas de threading en entornos multiprocesador

## Actualización Mayor Versión 10.0

Una actualización significativa con muchas características nuevas y mejoras arquitectónicas:

### Manejo Avanzado de Medios

* Lector de información de medios completamente rediseñado con mejor soporte de formatos
* Componente `MediaInfoNV` renombrado a `MediaInfoReader` más intuitivo
* Capacidades de extracción de metadatos mejoradas para una gama más amplia de formatos

### Sistema de Etiquetado de Medios

* Añadido soporte completo de etiquetas estándar para varios formatos:
  * Archivos de video: MP4, WMV y otros formatos de contenedor
  * Archivos de audio: MP3, AAC, M4A, Ogg Vorbis y formatos de audio adicionales
* Soporte de lectura de etiquetas en Media Player SDK
* Capacidades de escritura de etiquetas tanto en Video Capture SDK como en Video Edit SDK

### Mejoras de Sincronización

* Implementada funcionalidad de inicio retrasado en todos los componentes del SDK
* Nueva propiedad `Start_DelayEnabled` permitiendo inicialización casi simultánea de múltiples controles SDK
* Sincronización mejorada entre pipelines de procesamiento de audio y video

### Arquitectura de Procesamiento de Audio

* Efectos de audio reescritos en C# para compatibilidad con aplicaciones x64
* API de efectos legacy mantenida para compatibilidad hacia atrás
* Rendimiento mejorado y latencia reducida en procesamiento en tiempo real

### Experiencia del Desarrollador

* Añadido seguimiento de errores en ventana Output de Visual Studio
* Monitoreo de errores en tiempo real desde eventos OnError
* Serialización de configuraciones basada en JSON para gestión de configuración más fácil

### Adiciones de Formato de Salida

* Soporte de salida GIF tanto en Video Edit SDK .Net como en Video Capture SDK .Net
* Divisor MP3 personalizado que aborda problemas de reproducción con archivos MP3 problemáticos

### Cambios Estructurales

* Ensamblados `VisioForge.Controls.WinForms` y `VisioForge.Controls.WPF` consolidados en ensamblado unificado `VisioForge.Controls.UI`
* Añadida propiedad `ExecutableFilename` a `VFFFMPEGEXEOutput` para especificación de ejecutable FFMPEG personalizado
* Optimización significativa de efectos de video para arquitecturas de CPU Intel más recientes
* Soporte de multithreading mejorado para mejor utilización de multinúcleo

## Características de la Versión 9.0

Esta versión introdujo varias capacidades nuevas para mejorar la presentación de medios:

### Mejoras Visuales

* Añadido soporte de GIF animado como superposiciones de logo de imagen
* Pipeline de renderizado mejorado para animaciones más suaves
* Mejor manejo del canal alfa para superposiciones transparentes

### Acceso a Información del SDK

* Nueva propiedad `SDK_Version` para acceder programáticamente a versiones de ensamblados
* Añadida propiedad `SDK_State` para verificar información de registro y licenciamiento
* Capacidades de diagnóstico mejoradas para solución de problemas

### Mejoras de Licenciamiento

* Implementado sistema de eventos de licenciamiento dedicado para verificar edición de SDK requerida
* Mensajes de error más claros para problemas de licenciamiento
* Proceso de validación de licencia mejorado

## Actualización Versión 8.6

Una versión de mantenimiento enfocada en estabilidad:

### Mejoras de Estabilidad

* Corregidas fugas de memoria en operaciones de procesamiento de larga duración
* Abordados problemas de threading con operaciones de medios concurrentes
* Manejo de excepciones mejorado en componentes principales

## Versión 8.5

Esta actualización proporcionó mejoras del motor principal:

### Actualizaciones de FFMPEG

* Componentes principales de FFMPEG actualizados a la última versión estable
* Soporte de códecs mejorado para formatos de medios más nuevos
* Mejoras de rendimiento en operaciones de transcodificación

### Corrección de Errores

* Resueltos problemas con sincronización audio/video en formatos específicos
* Corregidos problemas de compatibilidad de formato de contenedor
* Estabilidad mejorada durante operaciones de conversión de formato

## Versión 7.0 Lanzamiento Inicial

La versión fundacional que estableció la funcionalidad principal:

### Características Clave

* Capacidades de edición de video de alto rendimiento
* Soporte completo de formatos para flujos de trabajo profesionales
* Diseño de API flexible para integración en varias aplicaciones
* Consideraciones de compatibilidad multiplataforma
* Base para desarrollo y mejoras futuras

## Compatibilidad y Requisitos

Al actualizar entre versiones, por favor considera lo siguiente:

* La versión 12.1 requiere .Net Framework 4.6 o superior
* La versión 11.3 y superiores soportan tanto .Net Core como .Net Framework
* La versión 10.0 introdujo cambios que rompen compatibilidad en estructura de ensamblados
* Los paquetes NuGet proporcionan la ruta de actualización más simple entre versiones

Nuestro desarrollo continuo busca mejorar la funcionalidad mientras mantiene compatibilidad donde sea posible. Los cambios de API están documentados en detalle para ayudar con la planificación de migración.

## Comenzar

Para desarrolladores nuevos en el SDK, recomendamos comenzar con la última versión para beneficiarse de todas las mejoras y optimizaciones. El instalador unificado y los paquetes NuGet hacen la integración sencilla tanto en proyectos nuevos como existentes.
