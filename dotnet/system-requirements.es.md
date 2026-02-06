---
title: Requisitos del Sistema y Compatibilidad SDK .NET
description: Soporte de plataformas y requisitos del sistema del SDK .NET para Windows, macOS, Linux, iOS y Android con detalles de compatibilidad de frameworks.
---

# Requisitos del Sistema para SDKs .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta guía detalla los requisitos del sistema y compatibilidad de plataformas para la suite de SDKs .NET de VisioForge, diseñados para aplicaciones de procesamiento y reproducción de video de alto rendimiento.

## Descripción General

Desbloquea potentes capacidades de video multiplataforma con los SDKs .NET de VisioForge, totalmente compatibles con Windows, Linux, macOS, Android e iOS. Nuestros SDKs proporcionan soporte robusto para .NET Framework, .NET Core y .NET 5+ moderno (incluyendo .NET 8 LTS y .NET 9), permitiendo una integración perfecta con WinForms, WPF, WinUI 3, Avalonia, .NET MAUI y Xamarin. Desarrolla aplicaciones de video de alto rendimiento con paradigmas familiares de C# en todos los sistemas operativos principales y frameworks de UI.

> **Nota Importante**: Mientras que los usuarios de Windows se benefician de nuestro paquete instalador dedicado, los desarrolladores que trabajan en otras plataformas deben utilizar el método de distribución de paquetes NuGet para la implementación.

## Requisitos del Entorno de Desarrollo

Las siguientes secciones describen los requisitos específicos para configurar tu entorno de desarrollo al trabajar con nuestros SDKs.

### Sistemas Operativos para Desarrollo

El desarrollo de aplicaciones usando nuestros SDKs es soportado en las siguientes plataformas:

#### Windows

* Windows 10 (todas las ediciones)
* Windows 11 (todas las ediciones)
* Recomendado: Última actualización de características con parches de seguridad actuales

#### Linux

* Ubuntu 22.04 LTS o más reciente
* Debian 11 o más reciente
* Otras distribuciones con bibliotecas equivalentes pueden funcionar pero no están oficialmente soportadas

#### macOS

* macOS 12 (Monterey) o más reciente
* Procesadores Apple Silicon (M1/M2/M3) e Intel soportados

### Requisitos de Hardware

Para una experiencia de desarrollo óptima, recomendamos:

* Procesador: 4+ núcleos, 2.5 GHz o más rápido
* RAM: 8 GB mínimo, 16 GB recomendado para proyectos complejos
* Almacenamiento: SSD con al menos 10 GB de espacio libre
* Gráficos: GPU compatible con DirectX 11 (Windows) o GPU compatible con Metal (macOS)

## Plataformas de Despliegue Objetivo

Nuestros SDKs pueden desplegarse en una variedad de plataformas, permitiendo una amplia distribución de tus aplicaciones.

### Plataformas de Escritorio

#### Windows

* Windows 10 (versión 1809 o más reciente)
* Windows 11 (todas las versiones)
* Arquitecturas x86 y x64 soportadas
* Soporte ARM64 para dispositivos Windows en ARM

#### Linux

* Ubuntu 22.04 LTS o más reciente
* Otras distribuciones requieren bibliotecas y dependencias equivalentes
* Arquitecturas x64 y ARM64 soportadas

#### macOS

* macOS 12 (Monterey) o más reciente
* Arquitecturas Intel y Apple Silicon soportadas nativamente
* Rosetta 2 no requerido para dispositivos Apple Silicon

### Plataformas Móviles

#### Android

* Android 10 (nivel de API 29) o más reciente
* Arquitecturas ARM, ARM64 y x86 soportadas
* Compatible con Google Play Store
* Renderizado acelerado por hardware recomendado

#### iOS

* iOS 12 o versiones más recientes
* Compatible con iPhone, iPad e iPod Touch
* Soporta arquitecturas ARMv7 y ARM64
* Compatible con distribución en App Store

## Compatibilidad con .NET Framework

Nuestros SDKs proporcionan amplia compatibilidad con varias implementaciones de .NET:

### .NET Framework

* .NET Framework 4.6.1
* .NET Framework 4.7.x
* .NET Framework 4.8
* .NET Framework 4.8.1

### .NET Moderno

* .NET Core 3.1 (LTS)
* .NET 5.0
* .NET 6.0 (LTS)
* .NET 7.0
* .NET 8.0 (LTS)
* .NET 9.0

### Xamarin (Legacy)

* Xamarin.iOS 12.0+
* Xamarin.Android 9.0+
* Xamarin.Mac 5.0+

## Integración con Frameworks de UI

Los SDKs se integran con una amplia variedad de frameworks de UI, permitiendo desarrollo de aplicaciones flexible:

### Frameworks Específicos de Windows

* Windows Forms (WinForms)
  * .NET Framework 4.6.1+ y .NET Core 3.1+
  * Opciones de renderizado de alto rendimiento
  * Soporta integración con el diseñador

* Windows Presentation Foundation (WPF)
  * .NET Framework 4.6.1+ y .NET Core 3.1+
  * Renderizado acelerado por hardware
  * Diseño basado en XAML con soporte de binding

* Windows UI Library 3 (WinUI 3)
  * Solo aplicaciones de escritorio
  * Componentes modernos de Fluent Design
  * Integración con Windows App SDK

### Frameworks Multiplataforma

* .NET MAUI
  * Desarrollo unificado para Windows, macOS, iOS y Android
  * Código de UI compartido entre plataformas
  * Rendimiento nativo con base de código compartida

* Avalonia UI
  * Framework de UI verdaderamente multiplataforma
  * Basado en XAML con paradigmas familiares
  * Compatible con Windows, Linux, macOS

### Frameworks Específicos para Móviles

* UI Nativa de iOS
  * Integración con UIKit
  * Capa de compatibilidad con SwiftUI
  * Soporte para Storyboard y XIB

* macOS / Mac Catalyst
  * Integración con AppKit y UIKit
  * Mac Catalyst para adaptación de apps de iPad
  * Elementos de UI nativos de macOS

* UI Nativa de Android
  * Integración con el toolkit de UI de Android
  * Soporte para Activities y Fragments
  * Compatibilidad con componentes de Material Design

## Métodos de Distribución

### Paquetes NuGet

Nuestros SDKs están disponibles como paquetes NuGet, simplificando la integración con tu flujo de trabajo de desarrollo.

### Instalador de Windows

Para desarrolladores de Windows, ofrecemos un paquete instalador dedicado que incluye:

* Binarios del SDK y dependencias
* Documentación y proyectos de ejemplo
* Componentes de integración con Visual Studio
* Herramientas y utilidades para desarrolladores

## Consideraciones de Rendimiento

### Requisitos de Memoria

* Huella de memoria base: ~50MB
* Procesamiento de video: 100-500MB adicionales dependiendo de la resolución y complejidad
* Procesamiento de video 4K: 1GB+ recomendado

### Utilización de CPU

* Captura de video: 10-30% en una CPU moderna de cuatro núcleos
* Efectos en tiempo real: 10-40% adicional dependiendo de la complejidad
* Aceleración de hardware recomendada para entornos de producción

### Requisitos de Almacenamiento

* Instalación del SDK: ~250MB
* Caché en tiempo de ejecución: ~100MB
* Archivos de procesamiento temporal: Hasta varios GB dependiendo de la carga de trabajo

## Licenciamiento y Despliegue

Consulta nuestra página de [Licenciamiento](../../licensing.md) para más información sobre las diferentes opciones de licenciamiento disponibles para nuestros SDKs.

## Recursos de Soporte Técnico

Proporcionamos amplios recursos para asegurar una implementación exitosa:

* Documentación de API con ejemplos de código
* Guías de implementación para varias plataformas
* Consejos de solución de problemas y optimización
* Canales de soporte directo para desarrolladores licenciados

## Ejemplos de Código y Muestras

Visita nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para una extensa colección de ejemplos de código que demuestran características del SDK y patrones de implementación en las plataformas soportadas.

## Actualizaciones y Mantenimiento

* Actualizaciones regulares del SDK con nuevas características y optimizaciones
* Parches de seguridad y correcciones de errores
* Consideraciones de compatibilidad hacia atrás
* Guías de migración para transiciones de versión

---
Este documento de especificación técnica describe los requisitos del sistema y matriz de compatibilidad para nuestro Video Capture SDK .Net y productos relacionados. Para detalles específicos de implementación o escenarios de integración personalizados, por favor contacta a nuestro equipo de soporte para desarrolladores.