---
title: Despliegue Biblioteca Media Player Delphi & ActiveX
description: Desplegar TVFMediaPlayer en Delphi y ActiveX - métodos de instalación automatizados y manuales, configuración de códecs, filtros DirectShow y dependencias.
---

# Guía de Despliegue para TVFMediaPlayer

El despliegue de aplicaciones construidas con la biblioteca TVFMediaPlayer requiere asegurar que todos los componentes necesarios estén correctamente instalados y configurados en la máquina de destino. Esta guía proporciona instrucciones detalladas para métodos de despliegue automatizados y manuales, atendiendo diferentes escenarios y requisitos técnicos. Ya sea que prefiera la simplicidad de los instaladores silenciosos o el control granular de la configuración manual, este documento cubre los pasos esenciales para desplegar exitosamente su aplicación de reproductor multimedia Delphi o ActiveX.

## Comprendiendo los Requisitos de Despliegue

Antes de desplegar su aplicación, es crucial entender las dependencias de la biblioteca TVFMediaPlayer. La biblioteca depende de varios componentes centrales, incluyendo runtimes base, códecs específicos (como FFMPEG o VLC para ciertas fuentes), y Redistribuibles de Microsoft Visual C++. El método de despliegue que elija determinará cómo se manejan estas dependencias.

### Componentes Centrales

* **Biblioteca Base:** Contiene el motor esencial y filtros DirectShow para funcionalidad básica de reproducción.
* **Paquetes de Códecs:** Opcionales pero a menudo necesarios para soportar una amplia gama de formatos multimedia y streams de red (ej., cámaras IP). FFMPEG y VLC son opciones comunes proporcionadas.
* **Dependencias de Runtime:** Los paquetes Redistribuibles de Microsoft Visual C++ son requeridos para que los componentes de la biblioteca principal funcionen correctamente.

Elegir la estrategia de despliegue correcta depende de factores como los privilegios del usuario en la máquina de destino, la necesidad de instalación desatendida, y las funcionalidades específicas de su aplicación (ej., qué fuentes multimedia necesita soportar).

## Método 1: Instalación Automatizada (Se Requieren Derechos de Administrador)

Usar los instaladores silenciosos proporcionados es el método más directo para desplegar los componentes de la biblioteca TVFMediaPlayer. Estos instaladores manejan el registro de archivos necesarios y aseguran que todas las dependencias estén correctamente ubicadas. Este método requiere privilegios administrativos en la máquina de destino ya que involucra cambios a nivel de sistema como registrar componentes COM y potencialmente modificar el PATH del sistema.

### Instaladores Disponibles

VisioForge proporciona instaladores separados para la biblioteca base y paquetes de códecs opcionales, con versiones tanto para Delphi como ActiveX, y para arquitecturas x86 y x64.

#### Paquete Base (Obligatorio)

Este paquete instala los componentes centrales de TVFMediaPlayer y filtros DirectShow esenciales. Siempre es requerido, independientemente de las fuentes multimedia que su aplicación use. Elija el instalador correspondiente a su entorno de desarrollo (Delphi o ActiveX) y arquitectura objetivo (x86 o x64).

* **Delphi:**
  * [Instalador x86](https://files.visioforge.com/redists_delphi/redist_media_player_base_delphi.exe)
  * [Instalador x64](https://files.visioforge.com/redists_delphi/redist_media_player_base_delphi_x64.exe)
* **ActiveX:**
  * [Instalador x86](https://files.visioforge.com/redists_delphi/redist_media_player_base_ax.exe)
  * [Instalador x64](https://files.visioforge.com/redists_delphi/redist_media_player_base_ax_x64.exe)

#### Paquete FFMPEG (Opcional - Para Fuentes de Archivo/Cámara IP)

Si su aplicación necesita reproducir archivos locales o transmitir desde cámaras IP usando el motor FFMPEG, debe desplegar este paquete. FFMPEG proporciona una amplia gama de soporte de códecs.

* **FFMPEG:**
  * [Instalador x86](https://files.visioforge.com/redists_delphi/redist_media_player_ffmpeg.exe)
  * *Nota: Un enlace de instalador FFMPEG x64 no fue proporcionado explícitamente en la fuente original; asuma que x86 cubre la mayoría de necesidades o consulte la documentación de VisioForge para especificaciones x64 si es requerido.*

#### Paquete de Fuente VLC (Opcional - Para Fuentes de Archivo/Cámara IP)

Como alternativa o adición a FFMPEG, puede usar el motor VLC para fuentes de archivo y cámara IP. Esto requiere desplegar el paquete VLC. Asegúrese de seleccionar la arquitectura correcta.

* **VLC:**
  * [Instalador x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe)
  * [Instalador x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)

### Uso del Instalador

Estos instaladores están diseñados para ejecución silenciosa, haciéndolos adecuados para inclusión en rutinas de configuración de aplicaciones más grandes o para despliegue mediante scripts. Ejecute el(los) ejecutable(s) con privilegios de administrador en la máquina de destino.

```bash
# Ejemplo: Ejecutando el instalador base Delphi x86 silenciosamente
redist_media_player_base_delphi.exe /S
```

*(Nota: El interruptor silencioso exacto puede variar; consulte la documentación del instalador o use interruptores estándar como `/S`, `/silent`, o `/q` si `/S` no funciona).*

## Método 2: Instalación Manual (Se Recomiendan Derechos de Administrador)

La instalación manual ofrece más control pero requiere ejecución cuidadosa de cada paso. Este método es adecuado cuando los instaladores automatizados no pueden usarse, o al desplegar en entornos con restricciones específicas. Mientras que algunos pasos pueden lograrse sin derechos completos de administrador, registrar componentes COM típicamente requiere elevación.

### Prerrequisitos

Antes de copiar archivos de biblioteca, asegúrese de que las dependencias de runtime necesarias estén presentes en el sistema de destino.

#### Instalar Redistribuible VC++ 2010 SP1

La biblioteca TVFMediaPlayer depende del runtime Microsoft Visual C++ 2010 SP1. Instale la versión apropiada (x86 o x64) para la arquitectura objetivo de su aplicación.

* **VC++ 2010 SP1:**
  * [Redistribuible x86](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
  * [Redistribuible x64](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

Ejecute estos instaladores antes de proceder con el despliegue de archivos de biblioteca.

### Desplegando Archivos de Biblioteca Central

Siga estos pasos para instalar manualmente los componentes de la biblioteca base:

1. **Copiar DLLs Centrales:** Localice la carpeta `Redist\Filters` dentro de su directorio de instalación de TVFMediaPlayer. Copie todos los archivos DLL de esta carpeta a un directorio de despliegue en la máquina de destino. Una práctica común es colocar estos DLLs en la misma carpeta que el ejecutable de su aplicación.
2. **Registrar Filtros DirectShow:** La funcionalidad central depende de varios filtros DirectShow (archivos `.ax`). Estos deben ser registrados con el sistema operativo Windows usando el registro de Component Object Model (COM).
    * **Identificar Filtros:** Los filtros clave a registrar son:
        * `VisioForge_Audio_Effects_4.ax`
        * `VisioForge_Dump.ax`
        * `VisioForge_RGB2YUV.ax`
        * `VisioForge_Video_Effects_Pro.ax`
        * `VisioForge_YUV2RGB.ax`
        * *(Nota: Otros archivos `.ax` pueden estar presentes; registre todos los archivos `.ax` encontrados en el directorio `Redist\Filters`).*
    * **Método de Registro:** Use la herramienta de línea de comandos `regsvr32.exe`, que es parte de Windows. Abra un Símbolo del sistema **como Administrador** y ejecute el comando para cada archivo `.ax`.

        ```bash
        # Ejemplo: Registrando un filtro (ejecutar desde el directorio que contiene el archivo .ax)
        regsvr32.exe VisioForge_Video_Effects_Pro.ax
        ```

        Alternativamente, VisioForge proporciona una utilidad `reg_special.exe` en los redistribuibles. Copie esta utilidad a la carpeta que contiene los archivos `.ax` y ejecútela con privilegios de administrador para registrar automáticamente todos los filtros en ese directorio. Consulte la documentación de Microsoft para solucionar errores de `regsvr32.exe`: [Cómo usar la herramienta Regsvr32](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5).
3. **Actualizar PATH del Sistema (Opcional pero Recomendado):** Si los DLLs de filtro y archivos `.ax` se colocan en un directorio separado del ejecutable de su aplicación, debe agregar la ruta a este directorio a la variable de entorno `PATH` del sistema. Esto permite que el sistema operativo y su aplicación localicen estos archivos esenciales. No hacerlo puede resultar en errores de "DLL no encontrado" o errores de registro de filtro.

### Desplegando Paquetes Opcionales Manualmente

#### Despliegue de FFMPEG

1. **Copiar Archivos:** Copie todo el contenido de la carpeta `Redist\FFMPEG` de su instalación de TVFMediaPlayer a un directorio de despliegue en la máquina de destino (ej., una subcarpeta dentro del directorio de instalación de su aplicación).
2. **Actualizar PATH del Sistema:** Agregue la ruta completa a la carpeta donde copió los archivos FFMPEG a la variable de entorno `PATH` del sistema Windows. Esto es crucial para que la biblioteca encuentre y cargue los componentes FFMPEG.

#### Despliegue de VLC (Ejemplo: x86)

1. **Copiar Archivos:** Copie todo el contenido de la carpeta `Redist\VLC` (específicamente la versión x86 si aplica) a un directorio de despliegue.
2. **Registrar Filtro VLC:** Localice el archivo `.ax` dentro de los archivos VLC copiados (ej., `axvlc.dll` o similar, aunque el texto original solo menciona genéricamente "archivo .ax") y regístrelo usando `regsvr32.exe` con privilegios de administrador.
3. **Establecer Variable de Entorno:** Cree una nueva variable de entorno del sistema llamada `VLC_PLUGIN_PATH`. Establezca su valor a la ruta completa de la subcarpeta `plugins` dentro del directorio donde copió los archivos VLC (ej., `C:\SuApp\VLC\plugins`). Esto le dice al motor VLC dónde encontrar sus módulos de plugin necesarios.

## Verificación y Solución de Problemas

Después del despliegue, pruebe exhaustivamente su aplicación en la máquina de destino.

* Verifique la funcionalidad básica de reproducción.
* Pruebe cualquier funcionalidad específica que dependa de paquetes opcionales (FFMPEG o VLC), como reproducir varios formatos de archivo o conectarse a cámaras IP.
* Si ocurren errores, verifique:
  * Derechos de administrador durante instalación/registro.
  * Instalación correcta de Redistribuibles VC++.
  * Registro exitoso de todos los archivos `.ax` (verifique la salida de `regsvr32.exe`).
  * Configuración precisa de variables de entorno `PATH` y `VLC_PLUGIN_PATH`.
  * Coincidencia correcta de arquitectura (x86/x64) entre su aplicación, los componentes de biblioteca y las dependencias de runtime.

---
¿Necesita más asistencia? Contacte al [Soporte de VisioForge](https://support.visioforge.com/). Explore más ejemplos en nuestro [GitHub](https://github.com/visioforge/).