---
title: Despliegue de VisioForge .NET SDK en todas las plataformas
description: "Despliegue sus aplicaciones .NET en Windows, macOS, iOS, Android y Linux: bibliotecas nativas, dependencias e integración con frameworks de UI."
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS

order: 17
sidebar_label: Despliegue
---

# Guía de despliegue multiplataforma para SDK .NET de VisioForge

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al despliegue del SDK de VisioForge

La suite de SDKs de VisioForge proporciona potentes capacidades multimedia para aplicaciones .NET, soportando captura, edición, reproducción y procesamiento avanzado de medios en múltiples plataformas. El despliegue adecuado es crítico para asegurar que sus aplicaciones funcionen correctamente y aprovechen todo el potencial de estos SDKs.

Esta guía completa describe el proceso de despliegue para aplicaciones construidas con los SDKs .NET multiplataforma de VisioForge, ayudándole a navegar los requisitos específicos de cada sistema operativo soportado.

## Descripción general del despliegue

El despliegue de aplicaciones construidas con SDKs de VisioForge requiere consideración cuidadosa de dependencias y configuraciones específicas de plataforma. El proceso de despliegue varía significativamente dependiendo de su plataforma objetivo debido a diferencias en:

- Requisitos de bibliotecas nativas
- Dependencias del framework de medios
- Mecanismos de acceso a hardware
- Métodos de distribución de paquetes

### Consideraciones clave de despliegue

Antes de comenzar el proceso de despliegue, considere estos factores críticos:

1. **Arquitectura de la plataforma objetivo**: Asegúrese de seleccionar la arquitectura apropiada (x86, x64, ARM64) para su plataforma de despliegue
2. **Dependencias requeridas**: Algunas plataformas requieren bibliotecas adicionales que no están incluidas en los paquetes NuGet
3. **Compatibilidad del framework**: Verifique la compatibilidad entre su versión de .NET y el sistema operativo objetivo
4. **Integración de bibliotecas nativas**: Comprenda cómo las bibliotecas nativas se integran y cargan en cada plataforma
5. **Selección del framework de UI**: Elija el paquete de integración de UI apropiado para su framework seleccionado

## Despliegue específico por plataforma

### Despliegue en Windows

El despliegue en Windows es el más directo, con soporte completo de paquetes NuGet cubriendo todas las dependencias:

- **Distribución de paquetes**: Todos los componentes disponibles vía NuGet
- **Soporte de arquitectura**: Arquitecturas x86 y x64 completamente soportadas
- **Bibliotecas nativas**: Desplegadas automáticamente junto con su aplicación
- **Opciones de framework de UI**: Windows Forms, WPF, WinUI, Avalonia, Uno y MAUI soportados

Para instrucciones detalladas de despliegue en Windows, consulte la [guía de despliegue para Windows](Windows.md).

### Despliegue en Android

El despliegue en Android requiere configuración específica para extracción de bibliotecas nativas y permisos:

- **Distribución de paquetes**: Componentes principales disponibles vía NuGet
- **Soporte de arquitectura**: Arquitecturas ARM64, ARMv7 y x86_64 soportadas
- **Bibliotecas nativas**: Requiere configuración adecuada para extracción a la ubicación correcta
- **Permisos**: Permisos de cámara, micrófono y almacenamiento deben solicitarse explícitamente
- **Integración de UI**: Controles de vista de video específicos de Android requeridos

Las aplicaciones Android usan una sola biblioteca nativa que debe desplegarse correctamente. Revise la [guía de despliegue para Android](Android.md) para instrucciones completas.

### Despliegue en macOS

El despliegue en macOS requiere instalación adicional de la biblioteca GStreamer:

- **Distribución de paquetes**: Componentes principales disponibles vía NuGet, GStreamer requiere instalación manual
- **Soporte de arquitectura**: Arquitecturas Intel (x64) y Apple Silicon (ARM64) soportadas
- **Bibliotecas nativas**: Múltiples bibliotecas no administradas requeridas
- **Opciones de framework**: macOS nativo, MAUI y Avalonia soportados
- **Integración de bundle**: Atención especial necesaria para estructura correcta del app bundle

Los despliegues en macOS pueden requerir configuraciones específicas de entitlements y permisos. Consulte la [guía de despliegue para macOS](macOS.md) para instrucciones detalladas.

### Despliegue en iOS

El despliegue en iOS involucra desafíos únicos relacionados con las restricciones de la plataforma de Apple:

- **Distribución de paquetes**: Componentes principales disponibles vía NuGet
- **Soporte de arquitectura**: Arquitectura ARM64 soportada
- **Guías de App Store**: Consideraciones especiales para envíos al App Store
- **Bibliotecas nativas**: Biblioteca binaria no administrada única a desplegar
- **Integración de UI**: Controles de vista de video específicos de iOS requeridos

Las aplicaciones iOS requieren perfiles de aprovisionamiento y entitlements adecuados. La [guía de despliegue para iOS](iOS.md) proporciona instrucciones completas.

### Despliegue en Ubuntu/Linux

El despliegue en Linux requiere instalación manual de dependencias de GStreamer:

- **Distribución de paquetes**: Componentes principales disponibles vía NuGet, GStreamer requiere paquetes del sistema
- **Soporte de arquitectura**: Arquitectura x64 principalmente soportada
- **Dependencias del sistema**: Paquetes requeridos deben instalarse en el sistema objetivo
- **Consideraciones de distribución**: Diferentes distribuciones de Linux pueden requerir diferentes paquetes de dependencias
- **Opciones de UI**: Principalmente framework de UI Avalonia soportado

El despliegue en Linux a menudo involucra gestión de paquetes específica de la distribución. La [guía de despliegue para Ubuntu](Ubuntu.md) proporciona instrucciones para distribuciones basadas en Ubuntu.

### Despliegue en Uno Platform

Uno Platform permite desplegar aplicaciones desde una única base de código a múltiples plataformas:

- **Distribución de paquetes**: Componentes principales disponibles vía NuGet con redistribuibles específicos de plataforma
- **Plataformas soportadas**: Windows, Android, iOS, macOS (Catalyst), WebAssembly y Linux Desktop
- **Soporte de arquitectura**: Varía según la plataforma objetivo
- **Bibliotecas nativas**: Redistribuibles específicos de plataforma requeridos para cada objetivo
- **Integración de UI**: Control VideoView específico de Uno que se adapta a cada plataforma

Uno Platform simplifica el desarrollo multiplataforma mientras aprovecha las capacidades nativas. Consulte la [guía de despliegue de Uno Platform](uno.md) para instrucciones completas.

### Requisitos de runtime

Los dispositivos objetivo deben cumplir estos requisitos mínimos:

- **Windows**: Windows 7 o posterior (x86 o x64)
- **macOS**: macOS 12 (Monterey) o posterior (x64 o ARM64)
- **iOS**: iOS 18.0 o posterior (ARM64)
- **Android**: Android 7.0 (nivel de API 24) o posterior
- **Linux**: Ubuntu 20.04 LTS o posterior (x64 o ARM64)

## Desafíos comunes de despliegue

### Problemas de carga de bibliotecas nativas

Uno de los problemas de despliegue más comunes involucra fallos en la carga de bibliotecas nativas:

- **Síntomas**: Excepciones de runtime mencionando DllNotFoundException o similar
- **Causas**: Arquitectura incorrecta, dependencias faltantes o extracción inapropiada
- **Soluciones**: Verificar referencias de paquetes, revisar configuración de despliegue, asegurar que las bibliotecas estén en la ubicación correcta

### Restricciones de permisos y seguridad

Los sistemas operativos modernos aplican políticas de seguridad estrictas:

- **Acceso a cámara**: Requiere permiso explícito en todas las plataformas móviles
- **Acceso a almacenamiento**: Las restricciones del sistema de archivos varían por plataforma
- **Uso de red**: Puede requerir entitlements específicos o entradas de manifiesto
- **Operación en segundo plano**: Reglas específicas de plataforma para procesamiento de medios en segundo plano

### Consideraciones de rendimiento

El procesamiento de medios puede ser intensivo en recursos:

- **Uso de CPU**: Implemente threading apropiado para evitar congelamiento de UI
- **Gestión de memoria**: Monitoree y optimice el uso de memoria para archivos de medios grandes
- **Consumo de energía**: Equilibre configuraciones de calidad con consideraciones de vida de batería

## Lista de verificación de despliegue

Use esta lista de verificación para asegurar un despliegue exitoso:

- ✅ Paquetes NuGet correctos seleccionados para plataforma y arquitectura objetivo
- ✅ Dependencias específicas de plataforma instaladas y configuradas
- ✅ SDK inicializado y limpiado correctamente
- ✅ Controles de vista de video apropiados integrados
- ✅ Permisos requeridos solicitados y justificados
- ✅ Aplicación probada en plataforma objetivo bajo condiciones realistas
- ✅ Métricas de rendimiento validadas para experiencia de usuario aceptable
- ✅ Manejo de errores implementado para recuperación elegante

## Despliegue de Computer Vision

El SDK de Computer Vision es un paquete NuGet separado. Consulte la [guía de despliegue de Computer Vision](computer-vision.md) para más información.

## Recursos adicionales

- [Repositorio GitHub de VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) - Ejemplos de código y proyectos de ejemplo
- [Documentación de API](https://api.visioforge.org/dotnet/) - Referencia completa de API
- [Portal de Soporte](https://helpdesk.visioforge.com/helpdesk/my-tickets) - Soporte técnico y base de conocimientos
