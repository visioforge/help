---
title: Instalar Media Player SDK en Delphi — Setup 32/64 bits
description: Instale VisioForge Media Player SDK en Delphi 10.x-12.x — componentes VCL/FMX, registro de paquetes, rutas de bibliotecas. Windows 32/64 bits compatible.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Playback
  - MP4
primary_api_classes:
  - TVFMediaPlayer

---

# Instalando TVFMediaPlayer en Delphi

Bienvenido a la guía detallada para instalar el VisioForge Media Player SDK, específicamente el componente `TVFMediaPlayer`, en su entorno de desarrollo Delphi. Esta guía cubre instalaciones para versiones clásicas de Delphi como Delphi 6 y 7, así como versiones modernas desde Delphi 2005 en adelante, incluyendo las últimas versiones que soportan desarrollo de 64 bits.

## Entendiendo TVFMediaPlayer

`TVFMediaPlayer` es un potente componente VCL de VisioForge diseñado para integración perfecta de capacidades de reproducción de video y audio en aplicaciones Delphi. Simplifica tareas como reproducir varios formatos multimedia, capturar instantáneas, controlar la velocidad de reproducción, gestionar streams de audio, y mucho más. Construido sobre un motor multimedia robusto, ofrece alto rendimiento y extenso soporte de formatos, haciéndolo una opción versátil para el desarrollo de aplicaciones multimedia en Delphi.

Esta guía asume que tiene una instalación funcional de Embarcadero Delphi o una versión compatible más antigua (Borland Delphi).

## Paso 1: Prerrequisitos y Descarga del Framework

Antes de proceder con la instalación, asegúrese de que su entorno de desarrollo cumpla con los prerrequisitos necesarios. Principalmente, necesita una versión con licencia o de prueba de Delphi instalada en su máquina Windows.

El componente `TVFMediaPlayer` se distribuye como parte del VisioForge All-in-One Media Framework. Este framework agrupa varios SDKs de VisioForge, proporcionando un kit de herramientas completo para el manejo de medios.

1. **Navegue a la Página del Producto:** Abra su navegador web y vaya a la página oficial del producto [All-in-One Media Framework](https://www.visioforge.com/all-in-one-media-framework) de VisioForge.
2. **Seleccione la Versión Delphi:** Localice la sección de descarga específicamente para Delphi. VisioForge típicamente ofrece versiones adaptadas para diferentes plataformas de desarrollo.
3. **Descargar:** Haga clic en el enlace de descarga para obtener el archivo ejecutable del instalador (`.exe`). Guarde este archivo en una ubicación conocida en su computadora, como su carpeta de Descargas.

El archivo descargado contiene no solo el componente `TVFMediaPlayer` sino también otras bibliotecas relacionadas, código fuente (si aplica según la licencia), archivos de runtime necesarios, y documentación.

## Paso 2: Ejecutando el Instalador

Una vez que la descarga esté completa, necesita ejecutar el instalador para colocar los archivos del SDK necesarios en su sistema.

1. **Localizar el Instalador:** Navegue a la carpeta donde guardó el archivo `.exe` descargado.
2. **Ejecutar como Administrador:** Haga clic derecho en el archivo del instalador y seleccione "Ejecutar como administrador". Esto es crucial porque el instalador necesita registrar componentes y potencialmente escribir en directorios del sistema, requiriendo privilegios elevados.
3. **Seguir las Instrucciones en Pantalla:** El asistente de instalación lo guiará a través del proceso. Típicamente, esto involucra:
    * Aceptar el acuerdo de licencia.
    * Elegir el directorio de instalación (la ubicación predeterminada es usualmente apropiada, ej., `C:\Program Files\VisioForge\All-in-One Media Framework Delphi\`). Anote esta ruta, ya que la necesitará más tarde.
    * Seleccionar componentes a instalar (asegúrese de que el Media Player SDK esté seleccionado).
    * Confirmar la instalación.
4. **Completar Instalación:** Permita que el instalador termine de copiar archivos y realizar las tareas de configuración necesarias.

Este proceso desempaqueta el SDK, incluyendo las unidades precompiladas (`.dcu`), el paquete del componente (`.dpk` / `.dproj`) y las DLL de ejecución requeridas. El componente Delphi se distribuye como unidades precompiladas — el código fuente `.pas` del paquete no se distribuye.

## Paso 3: Integrando con el IDE de Delphi

Después de ejecutar el instalador principal, el siguiente paso crítico es integrar el componente `TVFMediaPlayer` en el IDE de Delphi para poder usarlo visualmente en el diseñador de formularios y referenciar sus unidades en su código. El proceso difiere ligeramente entre versiones antiguas (Delphi 6/7) y más nuevas (Delphi 2005+).

**Importante:** Para todas las versiones de Delphi, se recomienda ejecutar el IDE de Delphi **como administrador** durante el proceso de instalación del paquete. Esto ayuda a evitar posibles problemas de permisos al compilar y registrar el paquete del componente.

### Instalación en Delphi 6 / Delphi 7

Estas versiones antiguas requieren configuración manual de rutas e instalación de paquetes.

1. **Lanzar Delphi (como Administrador):** Inicie su IDE de Delphi 6 o Delphi 7 con privilegios administrativos.
2. **Abrir Opciones del IDE:** Vaya al menú `Tools` y seleccione `Environment Options`.
3. **Configurar Ruta de Biblioteca:**
    * Navegue a la pestaña `Library`.
    * En el campo `Library path`, haga clic en el botón de puntos suspensivos (`...`).
    * Haga clic en el botón `Add` o `New` (el icono puede variar) y navegue a la carpeta del paquete correspondiente a su versión de Delphi — aquí es donde se encuentran las unidades precompiladas `.dcu`, ej., `C:\Program Files\VisioForge\All-in-One Media Framework Delphi\Media Player\Packages\Delphi 7`. Agregue esta ruta. Esto le dice a Delphi dónde encontrar las unidades compiladas durante la compilación.
    * Haga clic en `OK` para cerrar el editor de rutas.
4. **Configurar Ruta de Navegación:**
    * Mientras todavía está en la pestaña `Library`, localice el campo `Browsing path` (puede estar combinado o separado dependiendo de la versión/actualización exacta de Delphi).
    * Agregue la misma ruta de la carpeta del paquete aquí también. Esto ayuda al IDE a localizar archivos para características como autocompletado de código y navegación.
    * Haga clic en `OK` para guardar las Opciones del Entorno.
5. **Abrir el Archivo del Paquete:**
    * Vaya al menú `File` y seleccione `Open...`.
    * Navegue a la subcarpeta `Media Player\Packages\Delphi 7` (o `Delphi 6`) dentro del directorio de instalación de VisioForge (ej., `C:\Program Files\VisioForge\All-in-One Media Framework Delphi\Media Player\Packages\Delphi 7`).
    * Abra el archivo del paquete `VisioForge_Media_Player.dpk`. Es un paquete único — no hay paquetes de runtime y de tiempo de diseño separados.
6. **Compilar el Paquete:**
    * Con `VisioForge_Media_Player.dpk` abierto como proyecto activo en el Administrador de Proyectos, haga clic en el botón `Compile` (o use `Project -> Compile`). Resuelva cualquier error de compilación si ocurre (aunque típicamente innecesario con paquetes oficiales).
7. **Instalar el Paquete:**
    * Una vez compilado exitosamente, haga clic en el botón `Install` en el Administrador de Proyectos.
8. **Confirmación:** Debería ver un mensaje de confirmación indicando que el(los) paquete(s) fueron instalados. El componente `TVFMediaPlayer` (y potencialmente otros del SDK) deberían ahora aparecer en la paleta de componentes de Delphi, probablemente bajo una pestaña de categoría "VisioForge" o similar.

*Nota sobre Arquitectura:* Delphi 6/7 son estrictamente entornos de 32 bits (x86). Por lo tanto, solo instalará y usará la versión de 32 bits del componente `TVFMediaPlayer`. El SDK puede contener archivos de 64 bits, pero no son aplicables aquí.

### Instalación en Delphi 2005 y Posterior (XE, 10.x, 11.x, 12.x)

Las versiones modernas de Delphi ofrecen un proceso más optimizado y soporte robusto para múltiples plataformas (Win32, Win64).

1. **Lanzar Delphi (como Administrador):** Inicie su IDE de Delphi (ej., Delphi 11 Alexandria, Delphi 12 Athens) con privilegios administrativos.
2. **Abrir Opciones del IDE:** Vaya a `Tools -> Options`.
3. **Configurar Ruta de Biblioteca:**
    * En el diálogo de Opciones, navegue a `Language -> Delphi -> Library` (la ruta exacta puede variar ligeramente entre versiones).
    * Seleccione la plataforma objetivo para la cual desea configurar la ruta (ej., `Windows 32-bit`, `Windows 64-bit`). Se recomienda configurar ambas si planea construir para ambas arquitecturas.
    * Haga clic en el botón de puntos suspensivos (`...`) junto al campo `Library path`.
    * Agregue la ruta a la carpeta del paquete correspondiente a su versión de Delphi y plataforma, ej., `C:\Program Files\VisioForge\All-in-One Media Framework Delphi\Media Player\Packages\Delphi 13` (Win32) o `C:\Program Files\VisioForge\All-in-One Media Framework Delphi\Media Player\Packages\Delphi 13 x64` (Win64). Esta carpeta contiene las unidades precompiladas `.dcu`.
    * Haga clic en `Add` y luego `OK`. Repita para la otra plataforma si lo desea.
4. **Configurar Ruta de Navegación (Opcional pero Recomendado):**
    * Bajo la misma sección `Library`, agregue esa ruta de la carpeta del paquete al campo `Browsing path` también.
    * Haga clic en `OK` para guardar las Opciones.
5. **Abrir el Archivo del Paquete:**
    * Vaya a `File -> Open Project...`.
    * Navegue al directorio `Media Player\Packages` dentro de la instalación de VisioForge. Encuentre la subcarpeta correspondiente a su versión de Delphi y plataforma (ej., `Delphi XE11`, `Delphi XE12`, `Delphi 13`, o la carpeta `Delphi <versión> x64` correspondiente para 64 bits).
    * Abra el proyecto de paquete `VisioForge_Media_Player.dproj` (o `VisioForge_Media_Player.dpk`). Existe un paquete único que proporciona tanto la funcionalidad de runtime como de tiempo de diseño.
6. **Compilar e Instalar:**
    * En el Administrador de Proyectos, haga clic derecho en el proyecto del paquete (archivo `.dpk`).
    * Seleccione `Compile` del menú contextual.
    * Una vez compilado exitosamente, haga clic derecho nuevamente y seleccione `Install`.
7. **Confirmación:** Delphi confirmará la instalación, y los componentes aparecerán en la paleta.

*Nota sobre Arquitectura:* Delphi moderno soporta tanto objetivos de 32 bits (Win32) como de 64 bits (Win64). El SDK de VisioForge típicamente proporciona unidades precompiladas (`.dcu`) para ambos. Cuando compila e instala el paquete, Delphi usualmente maneja el registro para la plataforma actualmente activa. Puede cambiar plataformas en el Administrador de Proyectos y reconstruir/reinstalar si es necesario, aunque a menudo el IDE maneja esta asociación correctamente después de la instalación inicial.

## Paso 4: Configuración del Proyecto

Después de instalar el paquete del componente en el IDE, necesita asegurar que sus *proyectos* individuales puedan encontrar los archivos de VisioForge necesarios en tiempo de compilación y ejecución.

1. **Opciones del Proyecto:** Abra su proyecto Delphi (archivo `.dpr`). Vaya a `Project -> Options`.
2. **Ruta de Biblioteca:** Navegue a `Delphi Compiler -> Search path` (o similar dependiendo de la versión).
3. **Agregar Ruta del SDK:** Para cada plataforma objetivo (`Windows 32-bit`, `Windows 64-bit`) que pretenda usar:
    * Agregue la ruta a la carpeta del paquete correspondiente a la plataforma objetivo, ej., `C:\Program Files\VisioForge\All-in-One Media Framework Delphi\Media Player\Packages\Delphi 13` para Win32 y `C:\Program Files\VisioForge\All-in-One Media Framework Delphi\Media Player\Packages\Delphi 13 x64` para Win64. Esto asegura que el compilador pueda encontrar las unidades precompiladas `.dcu`. Cada versión de Delphi y cada plataforma tiene su propia carpeta `Packages\Delphi <versión>` (y `Delphi <versión> x64`).
4. **Guardar Cambios:** Haga clic en `OK` o `Guardar` para aplicar las opciones del proyecto.

Establecer la ruta de búsqueda del proyecto correctamente es crucial. Si el compilador se queja de no encontrar unidades como `MediaPlayerMain` o `MediaPlayerTypes`, rutas de búsqueda incorrectas o faltantes son la causa más común.

## Paso 5: Verificación

Para confirmar que la instalación fue exitosa:

1. **Verificar Paleta de Componentes:** Busque la pestaña "VisioForge" (o similar) en la paleta de componentes en el IDE de Delphi. Debería ver el icono `TVFMediaPlayer`.
2. **Crear una Aplicación de Prueba:**
    * Cree una nueva Aplicación de Formularios VCL (`File -> New -> VCL Forms Application - Delphi`).
    * Arrastre y suelte el componente `TVFMediaPlayer` de la paleta al formulario principal.
    * Si el componente aparece en el formulario sin errores, la instalación de tiempo de diseño es probablemente correcta.
    * Agregue un botón simple. En su manejador de evento `OnClick`, agregue una línea básica de código para interactuar con el reproductor, por ejemplo:

        ```delphi
        procedure TForm1.Button1Click(Sender: TObject);
        begin
          // Asegúrese de que VFMediaPlayer1 sea el nombre de su instancia del componente
          VFMediaPlayer1.FilenameOrURL := 'C:\ruta\a\su\test_video.mp4'; // Reemplace con una ruta de archivo multimedia real
          VFMediaPlayer1.Play();
        end;
        ```

    * Compile el proyecto (`Project -> Compile`). Si compila sin errores de "Archivo no encontrado" relacionados con unidades de VisioForge, la configuración de rutas es probablemente correcta.
    * Ejecute la aplicación. Si se ejecuta y puede reproducir el archivo multimedia usando el botón, la configuración de runtime está funcionando.

## Problemas Comunes de Instalación y Solución de Problemas

Aunque el proceso es generalmente directo, pueden surgir problemas ocasionales:

* **Permisos del IDE:** Olvidar ejecutar el IDE de Delphi como administrador durante la instalación del paquete puede llevar a errores escribiendo en el registro o carpetas del sistema, previniendo el registro del componente. **Solución:** Cierre Delphi, reinícielo como administrador, e intente los pasos de instalación del paquete nuevamente.
* **Errores de Configuración de Rutas:** Rutas incorrectas ya sea en la `Library Path` del IDE o en la `Search Path` del proyecto son comunes. **Solución:** Verifique que las rutas apunten *exactamente* al directorio `Packages\Delphi <versión>` (o `Delphi <versión> x64`) del SDK de VisioForge que contiene las unidades `.dcu`. Asegúrese de que las rutas sean correctas para la plataforma objetivo específica (Win32/Win64).
* **Errores de Compilación de Paquetes:** A veces, conflictos con otros paquetes instalados o problemas dentro del código fuente del paquete pueden causar fallos de compilación. **Solución:** Asegúrese de estar usando la versión correcta del paquete para su versión específica de Delphi. Consulte el soporte o foros de VisioForge si los errores persisten.
* **Problemas Específicos de 64 bits:** Instalar paquetes para la plataforma de 64 bits a veces puede presentar desafíos únicos, especialmente en versiones más antiguas de Delphi que introdujeron por primera vez el soporte de Win64. Consulte el artículo vinculado [Problema de instalación de paquete Delphi de 64 bits](../../general/install-64bit.md) para problemas conocidos específicos y soluciones alternativas.
* **Problemas con Archivos `.otares`:** Algunas versiones de Delphi utilizan archivos `.otares` para recursos. Pueden ocurrir problemas durante la instalación del paquete relacionados con estos archivos. Vea el artículo vinculado [Problema de instalación de paquete Delphi con .otares](../../general/install-otares.md).
* **DLLs de Runtime Faltantes:** El `TVFMediaPlayer` a menudo depende de DLLs subyacentes (ej., componentes FFmpeg) para su funcionalidad. Aunque el instalador principal usualmente maneja estos, asegúrese de que estén correctamente ubicados ya sea en el directorio de salida de su aplicación, un directorio en el PATH del sistema, o las carpetas System32/SysWOW64 según corresponda. El despliegue requiere distribuir estos DLLs necesarios con su aplicación. Consulte la documentación de VisioForge para una lista de archivos de runtime requeridos.

## Pasos Adicionales y Recursos

Con `TVFMediaPlayer` instalado exitosamente, ahora puede explorar sus extensas características.

* **Explorar Propiedades y Eventos:** Use el Inspector de Objetos de Delphi para examinar las numerosas propiedades y eventos disponibles para el componente `TVFMediaPlayer`.
* **Consultar Documentación:** Consulte la documentación oficial de VisioForge instalada con el SDK o disponible en línea para referencias detalladas de API y ejemplos de uso.
* **Ejemplos de Código:** Visite el [repositorio de GitHub](https://github.com/visioforge/) de VisioForge para encontrar proyectos demo y fragmentos de código que demuestran varias funcionalidades.
* **Buscar Soporte:** Si encuentra problemas persistentes o tiene preguntas específicas no cubiertas aquí, contacte al [soporte de VisioForge](https://support.visioforge.com/) para asistencia.

---
Por favor contacte con [soporte](https://support.visioforge.com/) para obtener ayuda con este tutorial. Visite nuestra página de [GitHub](https://github.com/visioforge/) para obtener más ejemplos de código.