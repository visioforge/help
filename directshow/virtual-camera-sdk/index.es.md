---
title: SDK de Cámara Virtual DirectShow para Streaming
description: Cree cámaras web virtuales para Zoom, Teams, Skype y OBS con SDK DirectShow para transmitir cualquier fuente de video con soporte de audio.
---

# SDK de Cámara Virtual DirectShow

## Descripción General

Nuestro robusto SDK de Cámara Virtual basado en DirectShow permite a los desarrolladores implementar potente funcionalidad de cámara virtual en sus aplicaciones. El SDK proporciona filtros sink que pueden utilizarse como salida en entornos de Video Capture SDK o Video Edit SDK, mientras que los filtros source pueden emplearse como fuentes de video para varias aplicaciones de captura.

Con este versátil kit de herramientas, puede transmitir contenido de video desde prácticamente cualquier fuente directamente a un dispositivo de cámara virtual. Estos dispositivos virtuales son completamente compatibles con plataformas de comunicación populares como `Skype`, `Zoom`, `Microsoft Teams`, navegadores web y numerosas otras aplicaciones que soportan cámaras virtuales DirectShow. El SDK también incluye capacidades completas de streaming de audio para soluciones multimedia completas.

Para ayudarle a comenzar rápidamente, el paquete SDK incluye una aplicación de ejemplo completamente funcional que demuestra cómo transmitir contenido de video desde archivos a dispositivos de cámara virtual.

Descargue el SDK desde nuestra [página del producto](https://www.visioforge.com/virtual-camera-sdk) para comenzar a integrar funcionalidad de cámara virtual en sus aplicaciones hoy.

---

## Instalación

Antes de usar los ejemplos de código e integrar el SDK en su aplicación, primero debe instalar el SDK de Cámara Virtual desde la [página del producto](https://www.visioforge.com/virtual-camera-sdk).

**Pasos de Instalación**:

1. Descargue el instalador del SDK desde la página del producto
2. Ejecute el instalador con privilegios administrativos
3. El instalador registrará el driver de cámara virtual y todos los filtros DirectShow necesarios
4. Las aplicaciones de ejemplo y el código fuente estarán disponibles en el directorio de instalación

**Nota**: El driver de cámara virtual y los filtros deben estar correctamente registrados en el sistema antes de poder usarlos en sus aplicaciones. El instalador maneja esto automáticamente.

---

## Características y Capacidades Principales

* **Soporte de Múltiples Fuentes**: Transmita video a la cámara virtual desde archivos, streams de red o dispositivos de captura
* **Compatibilidad de Arquitectura**: Soporte completo de arquitectura x86/x64
* **Soporte de Alta Resolución**: Transmita contenido de video hasta resolución 4K
* **Opciones de Personalización**: Defina e implemente nombres de cámara personalizados
* **Integración SDK**: Integración perfecta con otras herramientas de desarrollo
* **Soporte de Audio**: Capacidades completas de streaming de audio
* **Aplicaciones Profesionales**: Perfecto para teleconferencias, streaming y aplicaciones de video profesionales

## Implementación Técnica

### Arquitectura de Grafo DirectShow de Ejemplo

El diagrama a continuación ilustra la implementación estándar de grafo DirectShow al usar el SDK de Cámara Virtual:

![Grafo DirectShow de ejemplo](/help/docs/directshow/virtual-camera-sdk/demo.webp)

### Registro de Licencia vía Registro

Puede registrar el filtro con su clave de licencia válida usando el sistema de registro de Windows.

Configure el licenciamiento usando la siguiente clave de registro:

```reg
HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\License
```

Establezca su clave de licencia comprada como valor de cadena en esta ubicación del registro.

### Directrices de Despliegue

Para un despliegue adecuado, copie y registre COM los filtros DirectShow del SDK - estos son los archivos en la carpeta `Redist` con extensión .ax. El registro puede realizarse usando `regsvr32.exe` o a través del registro COM en su instalador de aplicación. Por favor note que se requieren privilegios administrativos para un registro exitoso.

### Configuración de Aplicación Sin Señal

Puede configurar una aplicación para ejecutarse automáticamente cuando la cámara virtual no está conectada a ninguna fuente de video.

Configure la aplicación sin señal usando esta clave de registro:

```reg
HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\StartupEXE
```

Establezca el nombre del archivo ejecutable como valor de cadena.

### Configuración de Imagen Sin Señal

En lugar de mostrar una pantalla negra cuando no hay fuente de video disponible, puede configurar una imagen personalizada para mostrar.

Configure la imagen sin señal usando esta clave de registro:

```reg
HKEY_LOCAL_MACHINE\SOFTWARE\VisioForge\Virtual Camera SDK\BackgroundImage
```

Establezca la ruta del archivo de imagen como valor de cadena.

## Referencia de Interfaz

### Interfaz IVFVirtualCameraSink

La interfaz `IVFVirtualCameraSink` se usa para configurar el filtro sink de cámara virtual, principalmente para registro de licencia.

**GUID de Interfaz**: `{A96631D2-4AC9-4F09-9F34-FF8229087DEB}`

**Hereda De**: `IUnknown`

#### Definición C#

```csharp
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Interfaz sink de cámara virtual para configuración de licencia.
/// </summary>
[ComImport]
[System.Security.SuppressUnmanagedCodeSecurity]
[Guid("A96631D2-4AC9-4F09-9F34-FF8229087DEB")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFVirtualCameraSink
{
    /// <summary>
    /// Establece la clave de licencia para el filtro sink de cámara virtual.
    /// </summary>
    /// <param name="license">Cadena de clave de licencia ("TRIAL" para versión de prueba)</param>
    /// <returns>HRESULT (0 para éxito)</returns>
    [PreserveSig]
    int set_license([MarshalAs(UnmanagedType.LPWStr)] string license);
}
```

**Ejemplo de Uso (C#)**:

```csharp
// Agregar filtro Virtual Camera Sink
var sinkFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraph,
    Consts.CLSID_VFVirtualCameraSink,
    "VisioForge Virtual Camera Sink");

// Establecer licencia
var sinkIntf = sinkFilter as IVFVirtualCameraSink;
if (sinkIntf != null)
{
    sinkIntf.set_license("SU-CLAVE-DE-LICENCIA"); // o "TRIAL" para versión de prueba
}
```

#### Definición C++

```cpp
#include <unknwn.h>

// {A96631D2-4AC9-4F09-9F34-FF8229087DEB}
DEFINE_GUID(IID_IVFVirtualCameraSink,
    0xa96631d2, 0x4ac9, 0x4f09, 0x9f, 0x34, 0xff, 0x82, 0x29, 0x8, 0x7d, 0xeb);

/// <summary>
/// Interfaz sink de cámara virtual para configuración de licencia.
/// </summary>
DECLARE_INTERFACE_(IVFVirtualCameraSink, IUnknown)
{
    /// <summary>
    /// Establece la clave de licencia para el filtro sink de cámara virtual.
    /// </summary>
    /// <param name="license">Cadena ancha de clave de licencia (L"TRIAL" para versión de prueba)</param>
    /// <returns>HRESULT (S_OK para éxito)</returns>
    STDMETHOD(set_license) (THIS_
        LPCWSTR license
        ) PURE;
};
```

---

## Bibliotecas de Terceros e Integración

El SDK de Cámara Virtual contiene componentes de terceros que se usan en las aplicaciones de demostración. Estos componentes no son requeridos para la funcionalidad principal del SDK.

Las aplicaciones de demostración de Delphi y .NET utilizan bibliotecas de terceros para simplificar el desarrollo DirectShow. Las aplicaciones de demostración C++ están construidas sin dependencias externas.

### Integración .NET

Las aplicaciones .NET aprovechan [DirectShowLib.Net (LGPL)](https://sourceforge.net/projects/directshownet/) para implementar funcionalidad DirectShow en entornos de código administrado.

Los desarrolladores pueden crear aplicaciones de consola, WinForms o WPF usando .NET. Las aplicaciones de demostración incluidas utilizan WinForms para la interfaz de usuario.

### Integración Delphi

Las aplicaciones Delphi usan [DSPack (MPL)](https://code.google.com/archive/p/dspack/) para implementar funcionalidad DirectShow. Aunque las versiones modernas de Delphi incluyen soporte DirectShow incorporado, DSPack se utiliza en las aplicaciones de demostración para mantener compatibilidad con versiones anteriores de Delphi.

### Integración C++

Las aplicaciones de demostración C++ no requieren bibliotecas de terceros y están construidas usando el SDK DirectShow estándar (parte del Windows SDK).

Los desarrolladores pueden utilizar MFC, ATL u otros frameworks C++ para construir sus aplicaciones. Las aplicaciones de demostración incluidas están construidas con MFC.

## Requisitos del Sistema

El SDK es compatible con los siguientes sistemas operativos Microsoft Windows:

* Windows 7, 8, 8.1, 10 y 11
* Windows Server 2008, 2012, 2016, 2019 y 2022

## Historial de Versiones y Actualizaciones

### Versión 14.0

* Optimizaciones de rendimiento y mejoras
* Compatibilidad mejorada con Windows 11
* Soporte mejorado para navegadores web modernos
* Actualizaciones menores y correcciones de errores

### Versión 12.0

* Mejoras de soporte Windows 10
* Mejoras de rendimiento
* Soporte de resolución 8K agregado
* Compatibilidad mejorada con Mozilla Firefox y Microsoft Edge
* Varias actualizaciones menores

### Versión 11.0

* Correcciones de errores críticos implementadas
* Compatibilidad actualizada con Google Chrome
* Resueltos problemas de clics de audio en varios navegadores web y aplicaciones

### Versión 10.0

* Soporte de alta tasa de frames agregado
* Mejoras significativas de rendimiento
* Actualizaciones menores y correcciones de errores

### Versión 9.0

* Soporte de resolución de video 4K agregado
* Soporte actualizado para navegadores web contemporáneos
* Varias actualizaciones y mejoras menores

### Versión 8.0

* Funcionalidad de imagen de fondo agregada para escenarios sin señal
* Implementada ejecución automática de aplicación para condiciones sin señal
* Compatibilidad mejorada con Skype

### Versión 7.1

* Soporte de streaming de audio vía salida de audio virtual y entrada de micrófono virtual
* Soporte de formato de audio PCM con tasas de muestreo y configuración de canales personalizables
* Correcciones de errores y mejoras de rendimiento
* Resoluciones de video adicionales agregadas

### Versión 7.0

* Lanzamiento inicial como producto independiente
* Anteriormente incluido en Video Edit SDK y Video Capture SDK
* Compatible con cualquier aplicación DirectShow

## Recursos Adicionales

* [Acuerdo de Licencia de Usuario Final](../../eula.md)
