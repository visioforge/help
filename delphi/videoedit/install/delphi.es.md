---
title: Instalar TVFVideoEdit en Delphi
description: Instale paquetes TVFVideoEdit en Delphi 6/7, 2005+ y 11+ con rutas de biblioteca, construcción de paquetes y configuración de solución de problemas.
---

# Instalar TVFVideoEdit en Delphi

> Productos relacionados: [VisioForge All-in-One Media Framework (Delphi / ActiveX)](https://www.visioforge.com/all-in-one-media-framework)

## Requisitos de Instalación

Antes de comenzar el proceso de instalación, asegúrese de tener:

1. Versión apropiada de Delphi instalada y correctamente configurada
2. Derechos administrativos para la instalación de paquetes
3. Descargada la última versión de la biblioteca TVFVideoEdit

## Instalación en Borland Delphi 6/7

### Paso 1: Configurar Rutas de Biblioteca

Comience abriendo la ventana "Opciones" en su IDE de Delphi.

![Captura de pantalla mostrando cómo abrir la ventana de Opciones](/help/docs/delphi/videoedit/install/ved6_1.webp)

Navegue a la sección de Biblioteca y agregue el directorio fuente tanto a las rutas de biblioteca como de navegador. Esto asegura que Delphi pueda localizar los archivos necesarios.

![Captura de pantalla mostrando la configuración de ruta de biblioteca](/help/docs/delphi/videoedit/install/ved6_2.webp)

### Paso 2: Abrir e Instalar el Paquete

Localice y abra el archivo principal del paquete desde la biblioteca.

![Captura de pantalla mostrando cómo abrir el paquete](/help/docs/delphi/videoedit/install/ved6_3.webp)

Instale el paquete haciendo clic en el botón Instalar en el IDE. Esto registra los componentes con la paleta de componentes de Delphi.

![Captura de pantalla mostrando la ubicación del botón de instalación](/help/docs/delphi/videoedit/install/ved6_4.webp)

![Captura de pantalla mostrando instalación exitosa](/help/docs/delphi/videoedit/install/ved6_5.webp)

### Consideraciones de Arquitectura

La biblioteca incluye versiones para arquitectura x86 y x64. Sin embargo, para Delphi 6/7, debe usar la versión x86 ya que estas versiones de Delphi no soportan desarrollo de 64-bit.

## Instalación en Delphi 2005 y Posterior

### Paso 1: Iniciar con Privilegios Administrativos

Para Delphi 2005 y versiones posteriores, inicie el IDE con derechos administrativos para asegurar permisos de instalación apropiados.

![Captura de pantalla mostrando inicio de Delphi 2005](/help/docs/delphi/videoedit/install/ved2005_1.webp)

![Captura de pantalla mostrando IDE de Delphi 2005](/help/docs/delphi/videoedit/install/ved2005_2.webp)

### Paso 2: Configurar Rutas de Biblioteca

Abra la ventana de Opciones y navegue a la sección de Biblioteca. Agregue el directorio fuente tanto a las rutas de biblioteca como de navegador.

![Captura de pantalla mostrando configuración de ruta de biblioteca en Delphi 2005](/help/docs/delphi/videoedit/install/ved2005_3.webp)

### Paso 3: Instalar el Paquete

Abra el archivo principal del paquete desde el directorio fuente de la biblioteca.

![Captura de pantalla mostrando apertura del paquete en Delphi 2005](/help/docs/delphi/videoedit/install/ved2005_4.webp)

Haga clic en el botón Instalar para registrar los componentes con la paleta de componentes de Delphi.

![Captura de pantalla mostrando ubicación del botón de instalación en Delphi 2005](/help/docs/delphi/videoedit/install/ved2005_5.webp)

![Captura de pantalla mostrando instalación exitosa en Delphi 2005](/help/docs/delphi/videoedit/install/ved2005_6.webp)

### Soporte de Arquitectura

Para Delphi 2005 y versiones posteriores, están disponibles versiones x86 y x64. Puede utilizar la versión de 64-bit si necesita desarrollar aplicaciones de 64-bit. Note que el IDE mismo puede requerir la versión x86 para operaciones de tiempo de diseño.

## Instalación en Delphi 11 y Posterior

Las versiones modernas de Delphi presentan un proceso de instalación simplificado:

1. Abra el archivo de paquete `.dproj` de la biblioteca ubicado en la carpeta de la biblioteca después de la instalación
2. Seleccione la configuración de construcción Release del menú desplegable
3. Construya e instale el paquete usando los comandos de construcción del IDE
4. Los componentes serán registrados y estarán listos para usar

## Mejores Prácticas de Configuración del Proyecto

Puede instalar paquetes x86 y x64 según los requisitos de su proyecto. Asegúrese de haber configurado correctamente los ajustes de ruta de biblioteca de su aplicación:

1. Agregue la ruta correcta de la carpeta de biblioteca a las opciones de su proyecto
2. Configure la ruta para localizar correctamente los archivos `.dcu`
3. Verifique la compatibilidad de arquitectura entre su proyecto y los paquetes instalados

## Solución de Problemas Comunes de Instalación

Si encuentra problemas durante la instalación, verifique estos problemas comunes:

### Problemas de Instalación de Paquete de 64-bit en Delphi

Algunos problemas específicos pueden ocurrir al instalar paquetes de 64-bit. Vea nuestra [guía detallada de solución de problemas](../../general/install-64bit.md) para soluciones.

### Problemas con Archivos .otares

Los problemas de instalación relacionados con archivos `.otares` están documentados en nuestra [página dedicada de solución de problemas](../../general/install-otares.md).

## Recursos Adicionales y Soporte

Para ejemplos de código adicionales e implementaciones, visite nuestro [repositorio de GitHub](https://github.com/visioforge/) donde mantenemos una colección de proyectos de muestra.

Si necesita asistencia personalizada con la instalación o implementación, por favor contacte a nuestro [equipo de soporte técnico](https://support.visioforge.com/) quienes pueden proporcionar orientación específica para su entorno de desarrollo.

---
Para preguntas técnicas o asistencia de instalación con esta biblioteca, por favor comuníquese con nuestro [equipo de soporte de desarrollo](https://support.visioforge.com/). Explore ejemplos de código adicionales y recursos en nuestra página de [GitHub](https://github.com/visioforge/).