---
title: Guía de Instalación de TVFVideoCapture para Delphi
description: Instalar paquetes TVFVideoCapture en Delphi 6/7 hasta Delphi 11+ con instrucciones completas de configuración, rutas de biblioteca y configuración.
---

# Guía de Instalación Completa de TVFVideoCapture para Desarrolladores Delphi

> Productos relacionados: [All-in-One Media Framework (Delphi / ActiveX)](https://www.visioforge.com/all-in-one-media-framework)

## Instalación en Borland Delphi 6/7

El proceso de instalación para entornos heredados de Delphi 6/7 requiere varios pasos específicos para asegurar la integración adecuada de la biblioteca TVFVideoCapture.

### Paso 1: Crear un Nuevo Paquete

Comience creando un nuevo paquete en su entorno de desarrollo Delphi 6/7.

![Creando un nuevo paquete en Delphi 6/7](/help/docs/delphi/videocapture/install/vcd6_1.webp)

### Paso 2: Configurar Rutas de Biblioteca

Agregue el directorio fuente de TVFVideoCapture a los ajustes de ruta de biblioteca y navegador. Esto permite a Delphi localizar los archivos de componente necesarios.

![Agregando directorio fuente a rutas de biblioteca](/help/docs/delphi/videocapture/install/vcd6_2.webp)

### Paso 3: Abrir el Paquete de Biblioteca

Navegue y abra el archivo de paquete de biblioteca para preparar la instalación.

![Abriendo el paquete de biblioteca](/help/docs/delphi/videocapture/install/vcd6_3.webp)

### Paso 4: Instalar el Paquete de Componente

Complete la instalación seleccionando la opción de instalar dentro de la interfaz del paquete.

![Instalando el paquete](/help/docs/delphi/videocapture/install/vcd6_4.webp)

![Confirmación de instalación exitosa](/help/docs/delphi/videocapture/install/vcd6_5.webp)

### Limitaciones de Arquitectura

Mientras que TVFVideoCapture ofrece soporte para arquitecturas x86 y x64, Delphi 6/7 solo soporta x86 debido a limitaciones de la plataforma. Los desarrolladores usando estas versiones necesitarán utilizar la implementación de 32 bits exclusivamente.

## Proceso de Instalación para Delphi 2005 y Versiones Posteriores

Las versiones modernas de Delphi ofrecen un flujo de trabajo de instalación mejorado con capacidades avanzadas.

### Paso 1: Iniciar Delphi con Privilegios Administrativos

Asegúrese de ejecutar su IDE Delphi con derechos administrativos para prevenir problemas de instalación relacionados con permisos.

![Abriendo Delphi con derechos de admin](/help/docs/delphi/videocapture/install/vcd2005_1.webp)

### Paso 2: Acceder al Diálogo de Opciones

Navegue al menú Opciones para configurar ajustes esenciales de biblioteca.

![Accediendo a la ventana de Opciones](/help/docs/delphi/videocapture/install/vcd2005_11.webp)

### Paso 3: Configurar Rutas del Directorio Fuente

Agregue el directorio fuente de TVFVideoCapture a los ajustes de ruta de biblioteca y navegador para asegurar el descubrimiento apropiado de componentes.

![Configurando rutas del directorio fuente](/help/docs/delphi/videocapture/install/vcd2005_2.webp)

### Paso 4: Abrir el Paquete de Biblioteca de Componentes

Localice y abra el archivo de paquete de biblioteca incluido con TVFVideoCapture.

![Abriendo el paquete de biblioteca de componentes](/help/docs/delphi/videocapture/install/vcd2005_3.webp)

### Paso 5: Completar la Instalación del Paquete

Instale el paquete a través de la interfaz de instalación de paquetes del IDE.

![Instalando el paquete de componentes](/help/docs/delphi/videocapture/install/vcd2005_4.webp)

![Verificación de instalación exitosa](/help/docs/delphi/videocapture/install/vcd2005_41.webp)

## Instalación Avanzada para Delphi 11 y Versiones Más Nuevas

Las últimas versiones de Delphi requieren un enfoque ligeramente diferente que aprovecha las estructuras de proyecto modernas.

### Paso 1: Localizar y Abrir el Proyecto de Paquete

Después de instalar el framework, navegue a la carpeta de instalación y abra el archivo de paquete `.dproj`.

### Paso 2: Seleccionar la Configuración de Compilación Apropiada

Elija la configuración de compilación Release para asegurar el rendimiento óptimo del componente.

![Seleccionando configuración de compilación Release](/help/docs/delphi/videocapture/install/delphi11-1.webp)

### Paso 3: Instalar el Paquete de Componentes

Complete el proceso de instalación a través de la interfaz de instalación de paquetes del IDE.

![Instalando el paquete de componentes](/help/docs/delphi/videocapture/install/delphi11-2.webp)

### Paso 4: Verificar el Éxito de la Instalación

Confirme que la instalación se completó exitosamente antes de proceder con el desarrollo.

![Verificación de instalación exitosa](/help/docs/delphi/videocapture/install/delphi11-3.webp)

## Requisitos de Configuración del Proyecto y Mejores Prácticas

### Soporte Multi-Arquitectura

TVFVideoCapture soporta arquitecturas x86 y x64, permitiéndole desarrollar aplicaciones para diferentes objetivos de plataforma. Puede instalar ambas versiones de paquete simultáneamente para soportar escenarios de despliegue flexibles.

### Configuración de Ruta de Biblioteca

Para la funcionalidad apropiada del componente, asegúrese de haber configurado la ruta de carpeta de biblioteca correcta en los ajustes de su proyecto de aplicación. Esta ruta debe apuntar a la ubicación que contiene los archivos `.dcu` para su arquitectura objetivo.

Para configurar esto:
1. Abra el diálogo de opciones de su proyecto
2. Navegue a la sección de ruta de Biblioteca
3. Agregue la ruta de biblioteca apropiada de TVFVideoCapture
4. Guarde los ajustes de su proyecto

Esta configuración asegura que su aplicación pueda localizar todos los recursos de componente requeridos durante el desarrollo y tiempo de ejecución.

## Solución de Problemas Comunes de Instalación

Al instalar TVFVideoCapture, los desarrolladores podrían encontrar varios problemas conocidos. Aquí hay soluciones a los problemas más frecuentes:

### Problemas de Instalación de Paquete de 64 bits

Si tiene dificultades instalando la versión de paquete de 64 bits, consulte nuestra [guía detallada para resolver problemas de instalación de paquete Delphi de 64 bits](../../general/install-64bit.md).

### Problemas de Instalación de Archivo de Recursos (.otares)

Algunos desarrolladores encuentran problemas relacionados con archivos `.otares` durante la instalación del paquete. Para un proceso de resolución paso a paso, vea nuestra [guía de solución de problemas para problemas de instalación de .otares](../../general/install-otares.md).

## Soporte Técnico y Recursos Adicionales

Para desarrolladores que requieren asistencia adicional con el proceso de instalación o implementación de componentes:

- Contacte a nuestro [equipo de soporte técnico](https://support.visioforge.com/) para asistencia personalizada de instalación
- Visite nuestro [repositorio de GitHub](https://github.com/visioforge/) para ejemplos de código adicionales y ejemplos de implementación
- Revise nuestra documentación para escenarios de uso avanzado y patrones de integración

Siguiendo esta guía de instalación asegurará que tenga un entorno de desarrollo correctamente configurado para crear potentes aplicaciones multimedia con TVFVideoCapture en sus proyectos Delphi.
