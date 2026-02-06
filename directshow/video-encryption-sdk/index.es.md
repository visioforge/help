---
title: SDK de Encriptación de Video DirectShow
description: SDK DirectShow de encriptación de video con AES-256 para archivos MP4 H.264/AAC, aceleración GPU y API completa para C++, C# y Delphi.
---

# SDK de Encriptación de Video

## Introducción a la Encriptación de Video

El [SDK de Encriptación de Video](https://www.visioforge.com/video-encryption-sdk) proporciona herramientas robustas para codificar archivos de video en formato MP4 H264/AAC con capacidades avanzadas de encriptación. Los desarrolladores pueden asegurar su contenido multimedia usando contraseñas personalizadas o métodos de encriptación con datos binarios.

El SDK se integra perfectamente con cualquier aplicación DirectShow a través de un conjunto completo de filtros. Estos filtros vienen con extensas interfaces que permiten a los desarrolladores ajustar la configuración según requisitos de seguridad específicos y necesidades de implementación.

---

## Instalación

Antes de usar los ejemplos de código e integrar el SDK en su aplicación, primero debe instalar el SDK de Encriptación de Video desde la [página del producto](https://www.visioforge.com/video-encryption-sdk).

**Pasos de Instalación**:

1. Descargue el instalador del SDK desde la página del producto
2. Ejecute el instalador con privilegios administrativos
3. El instalador registrará todos los filtros DirectShow necesarios
4. Las aplicaciones de ejemplo y el código fuente estarán disponibles en el directorio de instalación

**Nota**: Los filtros del SDK deben estar correctamente registrados en el sistema antes de poder usarlos en sus aplicaciones. El instalador maneja esto automáticamente.

---

## Flexibilidad de Integración

Puede implementar el SDK en varias aplicaciones DirectShow como filtros para procesos de encriptación y desencriptación. El sistema funciona efectivamente con:

- Fuentes de video en vivo
- Fuentes de video basadas en archivos
- Codificadores de video por software
- Codificadores de video acelerados por GPU del [paquete de Filtros de Codificación DirectShow](https://www.visioforge.com/encoding-filters-pack) (disponible por separado)
- Filtros DirectShow de terceros para opciones adicionales de codificación de video

## Características y Capacidades Principales

### Funcionalidad Principal

- **Encriptación/Desencriptación Segura**: Procese archivos de video o streams de captura con algoritmos de seguridad robustos
- **Soporte de Formato**: Soporte completo de codificador H264 para contenido de video
- **Manejo de Audio**: Soporte completo de codificador AAC para streams de audio
- **Opciones de Seguridad Flexibles**: Implemente encriptación usando datos binarios o contraseñas de cadena

### Optimización de Rendimiento

- Motor de encriptación AES-256 para máxima seguridad
- Soporte de aceleración por hardware de CPU
- Compatibilidad con aceleración GPU
- Optimizado para procesos de encriptación de alta velocidad

## Recursos de Desarrollo

### Ejemplos de Código y Documentación

El SDK incluye ejemplos de código completos para múltiples lenguajes de programación:

- Ejemplos de implementación en C#
- Código de referencia en C++
- Proyectos de ejemplo en Delphi

Estos ejemplos proporcionan guía de implementación práctica para desarrolladores que construyen aplicaciones de video seguras.

### Aplicación Demo

Explore la aplicación Video Encryptor incluida para una demostración práctica de las capacidades del SDK en un entorno funcional.

---

## Referencia de API

### Interfaces Principales

El SDK proporciona interfaces COM completas para encriptación y desencriptación:

#### IVFCryptoConfig

Interfaz principal para configurar ajustes de encriptación en el filtro muxer (encriptación) y filtro demuxer (desencriptación).

**GUID**: `{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}`

**Métodos**:
- `put_Provider` - Establecer proveedor de contraseña para escenarios avanzados (claves binarias, contraseñas dinámicas)
- `get_Provider` - Obtener interfaz de proveedor de contraseña
- `put_Password` - Establecer contraseña o clave de encriptación directamente (datos binarios)
- `HavePassword` - Verificar si la contraseña está establecida

#### IVFPasswordProvider

Interfaz de callback para escenarios avanzados de provisión de contraseña como claves de datos binarios, generación dinámica de contraseñas o derivación personalizada de claves.

**GUID**: `{6F8162B5-778D-42b5-9242-1BBABB24FFC4}`

**Métodos**:
- `QueryPassword` - Consultar contraseña para archivo específico

**Casos de Uso**:
- Provisión de datos de clave binaria
- Generación dinámica de contraseñas
- Claves de encriptación específicas por archivo
- Funciones de derivación de claves personalizadas

#### Clases de Ayuda

El SDK incluye métodos de extensión de ayuda para desarrolladores .NET:
- `ApplyString` - Aplicar contraseña de cadena (hasheada con SHA-256)
- `ApplyFile` - Usar archivo como clave de encriptación (hash SHA-256 del contenido del archivo)
- `ApplyBinary` - Aplicar datos de clave binaria (hasheados con SHA-256)

### CLSIDs de Filtros

| Filtro | CLSID | Propósito |
|--------|-------|-----------|
| **Muxer de Encriptación** | `{F1D3727A-88DE-49ab-A635-280BEFEFF902}` | Muxer con encriptación |
| **Demuxer de Desencriptación** | `{D2C761F0-9988-4f79-9B0E-FB2B79C65851}` | Demuxer con desencriptación |

Para documentación detallada de interfaces y ejemplos de código, vea la [Referencia de Interfaz](interface-reference.md).

---

## Ejemplos de Código

### Inicio Rápido - Encriptación

#### Ejemplo C#

```csharp
using VisioForge.DirectShowAPI;

// Obtener interfaz de configuración de criptografía del muxer de encriptación
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Aplicar contraseña de cadena
    cryptoConfig.ApplyString("MiContraseñaSegura123");

    // O usar archivo como clave
    // cryptoConfig.ApplyFile(@"C:\keys\miclave.bin");

    // O usar datos binarios
    // byte[] keyData = new byte[] { 0x01, 0x02, 0x03, ... };
    // cryptoConfig.ApplyBinary(keyData);
}
```

#### Ejemplo C++

```cpp
#include "encryptor_intf.h"

ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    // Establecer contraseña
    const wchar_t* password = L"MiContraseñaSegura123";
    hr = pCrypto->put_Password(
        (LPBYTE)password,
        wcslen(password) * sizeof(wchar_t)
    );

    pCrypto->Release();
}
```

### Desencriptación

#### Ejemplo C#

```csharp
// Obtener interfaz de configuración de criptografía del demuxer de desencriptación
var cryptoConfig = demuxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Debe usar la misma contraseña/clave que la encriptación
    cryptoConfig.ApplyString("MiContraseñaSegura123");
}
```

Para ejemplos completos incluyendo configuración de grafo de filtros, vea la [Página de Ejemplos](examples.md).

---

## Aplicaciones de Ejemplo

El SDK incluye aplicaciones de ejemplo funcionales que demuestran flujos de trabajo de encriptación y desencriptación:

### Ejemplos Incluidos

- **Demo de Encriptación** - Demuestra encriptación de archivos de video con codificación H.264/AAC
- **Demo de Reproductor** - Muestra desencriptación y reproducción de archivos de video encriptados

### Repositorio de GitHub

Código fuente completo para todos los ejemplos está disponible:
- [Ejemplos del SDK de Encriptación de Video](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) - Ejemplos en C#, C++ y Delphi

Estos ejemplos incluyen:
- Construcción completa de grafo de filtros
- Configuración de encriptación
- Desencriptación y reproducción
- Manejo de errores
- Implementación de mejores prácticas

---

## Información de Licenciamiento

- [Acuerdo de Licencia de Usuario Final](../../eula.md)

## Historial de Versiones

### Versión 11.4

- Compatibilidad completa con SDKs .Net de VisioForge 11.4
- Soporte mejorado de Nvidia NVENC para codificadores de video H264 y H265
- Soporte mejorado de Intel QuickSync para codificador de video H264
- Agregado soporte de espacio de color NV12 para rendimiento mejorado

### Versión 11.0

- Compatibilidad completa con SDKs .Net de VisioForge 11.0
- Soporte mejorado de codificadores GPU
- Funcionalidad de codificador AAC actualizada

### Versión 10.0

- Compatibilidad completa con SDKs .Net de VisioForge 10.0
- Compatibilidad mejorada con formatos de video H264 y H265
- Integrado soporte de aceleración AMD AMF
- Agregado soporte de tecnología Intel QuickSync

### Versión 9.0

- Velocidad de procesamiento de encriptación significativamente mejorada
- Agregadas capacidades de aceleración por hardware de CPU
- Implementado nuevo motor basado en encriptación AES-256
- Agregado uso de archivo como clave (con soporte de array binario)
- Integrado soporte NVENC para aceleración GPU
- Soporte mejorado de codificador AAC HE

### Versión 8.0

- Codificadores de video y audio actualizados
- Rendimiento de encriptación de filtros mejorado

### Versión 7.0

- Lanzamiento inicial como producto independiente
- Anteriormente integrado dentro de Video Capture SDK, Video Edit SDK y Media Player SDK
- Compatible con cualquier aplicación DirectShow sin requerir SDKs adicionales de VisioForge

---

## Recursos

- [Página del Producto](https://www.visioforge.com/video-encryption-sdk) - Compra, licenciamiento e información del producto
- [Aplicaciones de Ejemplo](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) - Ejemplos completos de código fuente

---

## Ver También

- [Referencia de Interfaz](interface-reference.md) - Documentación completa de API
- [Ejemplos](examples.md) - Ejemplos de código completos para encriptación y desencriptación
- [Paquete de Filtros de Codificación DirectShow](../filters-enc/index.md) - Codificadores de video compatibles (H.264, H.265, AAC)
