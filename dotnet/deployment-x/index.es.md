---
title: Guía de Despliegue de SDK .NET Multiplataforma
description: Despliega aplicaciones .NET en Windows, macOS, iOS, Android y Linux con bibliotecas nativas, dependencias de plataforma e integración de frameworks de UI.
---

# Guía de Despliegue Multiplataforma para SDK .NET de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al Despliegue del SDK de VisioForge

La suite de SDKs de VisioForge proporciona potentes capacidades multimedia para aplicaciones .NET, soportando captura, edición, reproducción y procesamiento avanzado de medios en múltiples plataformas. El despliegue adecuado es crítico para asegurar que tus aplicaciones funcionen correctamente y aprovechen todo el potencial de estos SDKs.

Esta guía completa describe el proceso de despliegue para aplicaciones construidas con los SDKs .NET multiplataforma de VisioForge, ayudándote a navegar los requisitos específicos de cada sistema operativo soportado.

## Descripción General del Despliegue

El despliegue de aplicaciones construidas con SDKs de VisioForge requiere consideración cuidadosa de dependencias y configuraciones específicas de plataforma. El proceso de despliegue varía significativamente dependiendo de tu plataforma objetivo debido a diferencias en:

- Requisitos de bibliotecas nativas
- Dependencias del framework de medios
- Mecanismos de acceso a hardware
- Métodos de distribución de paquetes

### Consideraciones Clave de Despliegue

Antes de comenzar el proceso de despliegue, considera estos factores críticos:

1. **Arquitectura de la Plataforma Objetivo**: Asegúrate de seleccionar la arquitectura apropiada (x86, x64, ARM64) para tu plataforma de despliegue
2. **Dependencias Requeridas**: Algunas plataformas requieren bibliotecas adicionales que no están incluidas en los paquetes NuGet
3. **Compatibilidad del Framework**: Verifica la compatibilidad entre tu versión de .NET y el sistema operativo objetivo
4. **Integración de Bibliotecas Nativas**: Comprende cómo las bibliotecas nativas se integran y cargan en cada plataforma
5. **Selección del Framework de UI**: Elige el paquete de integración de UI apropiado para tu framework seleccionado

## Despliegue Específico por Plataforma

### Despliegue en Windows

El despliegue en Windows es el más directo, con soporte completo de paquetes NuGet cubriendo todas las dependencias:

- **Distribución de Paquetes**: Todos los componentes disponibles vía NuGet
- **Soporte de Arquitectura**: Arquitecturas x86 y x64 completamente soportadas
- **Bibliotecas Nativas**: Desplegadas automáticamente junto con tu aplicación
- **Opciones de Framework de UI**: Windows Forms, WPF, WinUI, Avalonia, Uno y MAUI soportados

Para instrucciones detalladas de despliegue en Windows, consulta la [guía de despliegue para Windows](Windows.md).

### Despliegue en Android

El despliegue en Android requiere configuración específica para extracción de bibliotecas nativas y permisos:

- **Distribución de Paquetes**: Componentes principales disponibles vía NuGet
- **Soporte de Arquitectura**: Arquitecturas ARM64, ARMv7 y x86_64 soportadas
- **Bibliotecas Nativas**: Requiere configuración adecuada para extracción a la ubicación correcta
- **Permisos**: Permisos de cámara, micrófono y almacenamiento deben solicitarse explícitamente
- **Integración de UI**: Controles de vista de video específicos de Android requeridos

Las aplicaciones Android usan una sola biblioteca nativa que debe desplegarse correctamente. Revisa la [guía de despliegue para Android](Android.md) para instrucciones completas.

### Despliegue en macOS

El despliegue en macOS requiere instalación adicional de la biblioteca GStreamer:

- **Distribución de Paquetes**: Componentes principales disponibles vía NuGet, GStreamer requiere instalación manual
- **Soporte de Arquitectura**: Arquitecturas Intel (x64) y Apple Silicon (ARM64) soportadas
- **Bibliotecas Nativas**: Múltiples bibliotecas no administradas requeridas
- **Opciones de Framework**: macOS nativo, MAUI y Avalonia soportados
- **Integración de Bundle**: Atención especial necesaria para estructura correcta del app bundle

Los despliegues en macOS pueden requerir configuraciones específicas de entitlements y permisos. Consulta la [guía de despliegue para macOS](macOS.md) para instrucciones detalladas.

### Despliegue en iOS

El despliegue en iOS involucra desafíos únicos relacionados con las restricciones de la plataforma de Apple:

- **Distribución de Paquetes**: Componentes principales disponibles vía NuGet
- **Soporte de Arquitectura**: Arquitectura ARM64 soportada
- **Guías de App Store**: Consideraciones especiales para envíos al App Store
- **Bibliotecas Nativas**: Biblioteca binaria no administrada única a desplegar
- **Integración de UI**: Controles de vista de video específicos de iOS requeridos

Las aplicaciones iOS requieren perfiles de aprovisionamiento y entitlements adecuados. La [guía de despliegue para iOS](iOS.md) proporciona instrucciones completas.

### Despliegue en Ubuntu/Linux

El despliegue en Linux requiere instalación manual de dependencias de GStreamer:

- **Distribución de Paquetes**: Componentes principales disponibles vía NuGet, GStreamer requiere paquetes del sistema
- **Soporte de Arquitectura**: Arquitectura x64 principalmente soportada
- **Dependencias del Sistema**: Paquetes requeridos deben instalarse en el sistema objetivo
- **Consideraciones de Distribución**: Diferentes distribuciones de Linux pueden requerir diferentes paquetes de dependencias
- **Opciones de UI**: Principalmente framework de UI Avalonia soportado

El despliegue en Linux a menudo involucra gestión de paquetes específica de la distribución. La [guía de despliegue para Ubuntu](Ubuntu.md) proporciona instrucciones para distribuciones basadas en Ubuntu.

### Despliegue en Uno Platform

Uno Platform permite desplegar aplicaciones desde una única base de código a múltiples plataformas:

- **Distribución de Paquetes**: Componentes principales disponibles vía NuGet con redistribuibles específicos de plataforma
- **Plataformas Soportadas**: Windows, Android, iOS, macOS (Catalyst), WebAssembly y Linux Desktop
- **Soporte de Arquitectura**: Varía según la plataforma objetivo
- **Bibliotecas Nativas**: Redistribuibles específicos de plataforma requeridos para cada objetivo
- **Integración de UI**: Control VideoView específico de Uno que se adapta a cada plataforma

Uno Platform simplifica el desarrollo multiplataforma mientras aprovecha las capacidades nativas. Consulta la [guía de despliegue de Uno Platform](uno.md) para instrucciones completas.

### Requisitos de Runtime

Los dispositivos objetivo deben cumplir estos requisitos mínimos:

- **Windows**: Windows 7 o posterior (x86 o x64)
- **macOS**: macOS 10.15 (Catalina) o posterior (x64 o ARM64)
- **iOS**: iOS 14.0 o posterior (ARM64)
- **Android**: Android 7.0 (nivel de API 24) o posterior
- **Linux**: Ubuntu 20.04 LTS o posterior (x64 o ARM64)

## Desafíos Comunes de Despliegue

### Problemas de Carga de Bibliotecas Nativas

Uno de los problemas de despliegue más comunes involucra fallos en la carga de bibliotecas nativas:

- **Síntomas**: Excepciones de runtime mencionando DllNotFoundException o similar
- **Causas**: Arquitectura incorrecta, dependencias faltantes o extracción inapropiada
- **Soluciones**: Verificar referencias de paquetes, revisar configuración de despliegue, asegurar que las bibliotecas estén en la ubicación correcta

### Restricciones de Permisos y Seguridad

Los sistemas operativos modernos aplican políticas de seguridad estrictas:

- **Acceso a Cámara**: Requiere permiso explícito en todas las plataformas móviles
- **Acceso a Almacenamiento**: Las restricciones del sistema de archivos varían por plataforma
- **Uso de Red**: Puede requerir entitlements específicos o entradas de manifiesto
- **Operación en Segundo Plano**: Reglas específicas de plataforma para procesamiento de medios en segundo plano

### Consideraciones de Rendimiento

El procesamiento de medios puede ser intensivo en recursos:

- **Uso de CPU**: Implementa threading apropiado para evitar congelamiento de UI
- **Gestión de Memoria**: Monitorea y optimiza el uso de memoria para archivos de medios grandes
- **Consumo de Energía**: Balancea configuraciones de calidad con consideraciones de vida de batería

## Lista de Verificación de Despliegue

Usa esta lista de verificación para asegurar un despliegue exitoso:

- ✅ Paquetes NuGet correctos seleccionados para plataforma y arquitectura objetivo
- ✅ Dependencias específicas de plataforma instaladas y configuradas
- ✅ SDK inicializado y limpiado correctamente
- ✅ Controles de vista de video apropiados integrados
- ✅ Permisos requeridos solicitados y justificados
- ✅ Aplicación probada en plataforma objetivo bajo condiciones realistas
- ✅ Métricas de rendimiento validadas para experiencia de usuario aceptable
- ✅ Manejo de errores implementado para recuperación elegante

## Despliegue de Computer Vision

El SDK de Computer Vision es un paquete NuGet separado. Consulta la [guía de despliegue de Computer Vision](computer-vision.md) para más información.

## Recursos Adicionales

- [Repositorio GitHub de VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) - Ejemplos de código y proyectos de ejemplo
- [Documentación de API](https://api.visioforge.org/dotnet/) - Referencia completa de API
- [Portal de Soporte](https://support.visioforge.com/) - Soporte técnico y base de conocimientos
