---
title: Video Encryption SDK con AES-256 vía filtros DirectShow
description: SDK DirectShow de encriptación de video con AES-256 para archivos MP4 H.264/AAC, aceleración GPU y API completa para C++, C# y Delphi.
tags:
  - Video Encryption SDK
  - DirectShow
  - C++
  - Windows
primary_api_classes:
  - IVFCryptoConfig
  - IVFPasswordProvider
video:
  id: y6JMVHwyfrs
  upload_date: 2026-11-18T07:33:05Z
  duration: PT8M4S

order: 5
sidebar_label: Video Encryption SDK
---

# Video Encryption SDK

## Introducción a la Encriptación de Video

El [Video Encryption SDK](https://www.visioforge.com/video-encryption-sdk) proporciona herramientas robustas para codificar archivos de video en formato MP4 H264/AAC con capacidades avanzadas de encriptación. Los desarrolladores pueden asegurar su contenido multimedia usando contraseñas personalizadas o métodos de encriptación con datos binarios.

El SDK se integra perfectamente con cualquier aplicación DirectShow a través de un conjunto completo de filtros. Estos filtros vienen con extensas interfaces que permiten a los desarrolladores ajustar la configuración según requisitos de seguridad específicos y necesidades de implementación.


## Tutorial en vídeo

El vídeo cifra archivos con la aplicación Video Encryptor, después compila las demos C#, recorre sus grafos de filtros y reproduce los archivos cifrados:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/y6JMVHwyfrs?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

---

## Instalación

Antes de usar los ejemplos de código e integrar el SDK en su aplicación, primero debe instalar el Video Encryption SDK desde la [página del producto](https://www.visioforge.com/video-encryption-sdk).

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
| **VisioForge Encryptor v9** | `{86D83D2E-70D2-4A32-B7BE-E94A08AA7BAA}` | Muxer con cifrado AES-256 (archivos de la versión 9) |
| **VisioForge Decryptor v9** | `{4B89F5E2-2BA9-4C8F-A78A-EA2701AB582C}` | Demuxer con descifrado AES-256 (archivos de la versión 9) |
| **VisioForge Encryptor v8** | `{F1D3727A-88DE-49ab-A635-280BEFEFF902}` | Muxer con cifrado AES-128 (archivos de la versión 8) |
| **VisioForge Decryptor v8** | `{D2C761F0-9988-4f79-9B0E-FB2B79C65851}` | Demuxer con descifrado AES-128 (archivos de la versión 8) |

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


## Cifrar y reproducir un archivo en C#

Las [demos C#](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK/dotnet) construyen dos grafos DirectShow: Encryption Demo escribe un archivo cifrado y Player Demo lo reproduce. Ambas son aplicaciones WinForms en .NET 8 para x64 y acceden a DirectShow mediante el paquete NuGet [VisioForge.DirectShowAPI](https://www.nuget.org/packages/VisioForge.DirectShowAPI). Leen archivos MP4 mediante LAV Filters, que incluye el instalador de Video Encryption SDK. El código siguiente se basa en las demos.

### Añadir los filtros

El filtro de cifrado recibe el video H.264 y el audio AAC y los escribe en un único archivo cifrado mediante File Writer. El cifrador de la versión 9 escribe archivos AES-256; el de la versión 8, archivos AES-128.

```csharp
// AddEncryptor() in the demo
// Encryptor v9 writes AES-256 files; Consts.CLSID_VFVideoEncryptor8 writes AES-128 files
muxFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    new Guid(Consts.CLSID_VFVideoEncryptor9),
    "VisioForge Encryptor v9");

videoEncFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
    "VisioForge H264 Encoder");

audioEncFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
    "VisioForge AAC Encoder v10");

Guid CLSID_FileWriter = new Guid("8596E5F0-0DA5-11d0-BD21-00A0C911CE86");
fileWriterFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_FileWriter, "Filewriter");
IFileSinkFilter fileSink = fileWriterFilter as IFileSinkFilter;
fileSink?.SetFileName(edOutput.Text, null);
```

### Establecer la clave

`IVFCryptoConfig` recibe la clave: `ApplyString` una contraseña, `ApplyFile` cualquier archivo, `ApplyBinary` un array de bytes. Establézcala en `AddEncryptor`, justo después de añadir los filtros anteriores y antes de que `RenderStream` conecte el cifrador. Con una clave de licencia, `IVFRegister.SetLicenseKey` registra el filtro y desaparecen los avisos de la versión de prueba.

```csharp
IVFRegister reg = muxFilter as IVFRegister;
if (reg != null && !string.IsNullOrEmpty(SDK_LICENSE_KEY))
{
    reg.SetLicenseKey(SDK_LICENSE_KEY);
}

IVFCryptoConfig cryptoConfig = muxFilter as IVFCryptoConfig;
if (cryptoConfig == null)
{
    throw new InvalidOperationException("The encryptor filter is not registered.");
}

cryptoConfig.ApplyString(password);
// or: cryptoConfig.ApplyFile(keyFileName);
// or: cryptoConfig.ApplyBinary(keyBytes);
```

### Conectar el grafo

LAV Splitter Source divide el MP4 en sus flujos de video y audio. El video se decodifica primero en el filtro RGB2YUV, y solo después se añaden los codificadores al grafo. Un archivo sin flujo de audio se cifra solo con video.

```csharp
// LAV Splitter Source splits the file into video and audio streams
Guid CLSID_LAVSplitterSource = new Guid("B98D13E7-55DB-4385-A33D-09FD1BA26338");
sourceVideoFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_LAVSplitterSource, "LAV Splitter Source");
hr = ((IFileSourceFilter)sourceVideoFilter).Load(edSourceFile.Text, null);
DsError.ThrowExceptionForHR(hr);

Guid CLSID_VFRGB2YUV = new Guid("3BDA461E-12DB-4C24-9815-B68D1AA4D34A");
rgb2yuvFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_VFRGB2YUV, "VisioForge RGB2YUV");

// Decode the video into RGB2YUV before the encoders join the graph
hr = captureGraphSource.RenderStream(null, MediaType.Video, sourceVideoFilter, null, rgb2yuvFilter);
DsError.ThrowExceptionForHR(hr);

AddEncryptor();

hr = captureGraphSource.RenderStream(null, null, rgb2yuvFilter, videoEncFilter, muxFilter);
DsError.ThrowExceptionForHR(hr);

// A file without audio is encrypted video-only
IPin audioPin = FilterHelper.GetFreePinWithMediaType(sourceVideoFilter, PinDirection.Output, MediaType.Audio);
if (audioPin != null)
{
    Marshal.ReleaseComObject(audioPin);
    hr = captureGraphSource.RenderStream(null, MediaType.Audio, sourceVideoFilter, audioEncFilter, muxFilter);
    DsError.ThrowExceptionForHR(hr);
}

hr = captureGraphSource.RenderStream(null, null, muxFilter, null, fileWriterFilter);
DsError.ThrowExceptionForHR(hr);

// No reference clock: the file is encrypted as fast as the encoders run
IMediaFilter mediaFilter = filterGraphSource as IMediaFilter;
mediaFilter?.SetSyncSource(null);

hr = mediaControlSource.Run();
DsError.ThrowExceptionForHR(hr);
```

### Reproducir el archivo

Un File Source normal lee el archivo cifrado, y el filtro de descifrado lo descifra con la misma clave. El descifrador debe corresponder al archivo: versión 9 para AES-256, versión 8 para AES-128. Con una clave incorrecta, el grafo no puede conectar la salida del descifrador.

```csharp
var CLSID_FileSourceAsync = new Guid("E436EBB5-524F-11CE-9F53-0020AF0BA770");
var source = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_FileSourceAsync, "File source (async.)");
int hr = ((IFileSourceFilter)source).Load(edSourceFile.Text, null);
DsError.ThrowExceptionForHR(hr);

// Decryptor v9 reads AES-256 files; Consts.CLSID_VFVideoDecryptor8 reads AES-128 files
decryptor = FilterGraphTools.AddFilterFromClsid(
    filterGraph,
    new Guid(Consts.CLSID_VFVideoDecryptor9),
    "VisioForge Decryptor v9");

IVFRegister reg = decryptor as IVFRegister;
if (reg != null && !string.IsNullOrEmpty(SDK_LICENSE_KEY))
{
    reg.SetLicenseKey(SDK_LICENSE_KEY);
}

IVFCryptoConfig cryptoConfig = decryptor as IVFCryptoConfig;
if (cryptoConfig == null)
{
    throw new InvalidOperationException("The decryptor filter is not registered.");
}

cryptoConfig.ApplyString(password);

Guid CLSID_VideoMixingRenderer9 = new Guid("51b4abf3-748f-4e3b-a276-c828330e926a");
videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_VideoMixingRenderer9, "VMR-9");

// Draw the video into a panel of the form
IVMRFilterConfig9 vmrFilterConfig = videoRenderer as IVMRFilterConfig9;
if (vmrFilterConfig == null)
{
    throw new InvalidOperationException("Unable to create the VMR-9 renderer.");
}

vmrFilterConfig.SetRenderingMode(VMR9Mode.Windowless);
IVMRWindowlessControl9 vmrWindowlessControl = videoRenderer as IVMRWindowlessControl9;
vmrWindowlessControl.SetVideoClippingWindow(pnScreen.Handle);

hr = captureGraph.RenderStream(null, null, source, null, decryptor);
DsError.ThrowExceptionForHR(hr);

hr = captureGraph.RenderStream(null, MediaType.Video, decryptor, null, videoRenderer);
DsError.ThrowExceptionForHR(hr);

// Audio goes to the default sound device; a file without audio plays video-only
IPin audioPin = FilterHelper.GetFreePinWithMediaType(decryptor, PinDirection.Output, MediaType.Audio);
if (audioPin != null)
{
    Marshal.ReleaseComObject(audioPin);
    hr = captureGraph.RenderStream(null, MediaType.Audio, decryptor, null, null);
    if (hr < 0)
    {
        MessageBox.Show(this, $"Unable to play the audio (0x{hr:X8}), playing video only.");
    }
}

vmrWindowlessControl.SetVideoPosition(null, new DsRect(0, 0, pnScreen.Width, pnScreen.Height));

hr = mediaControl.Run();
DsError.ThrowExceptionForHR(hr);
```

---

## Aplicaciones de Ejemplo

El SDK incluye aplicaciones de ejemplo funcionales que demuestran flujos de trabajo de encriptación y desencriptación:

### Ejemplos Incluidos

- **Demo de Encriptación** - Demuestra encriptación de archivos de video con codificación H.264/AAC
- **Demo de Reproductor** - Muestra desencriptación y reproducción de archivos de video encriptados

### Repositorio de GitHub

Código fuente completo para todos los ejemplos está disponible:
- [Ejemplos del Video Encryption SDK](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) - Ejemplos en C#, C++ y Delphi

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

- Compatibilidad completa con SDKs .NET de VisioForge 11.4
- Soporte mejorado de Nvidia NVENC para codificadores de video H264 y H265
- Soporte mejorado de Intel QuickSync para codificador de video H264
- Agregado soporte de espacio de color NV12 para rendimiento mejorado

### Versión 11.0

- Compatibilidad completa con SDKs .NET de VisioForge 11.0
- Soporte mejorado de codificadores GPU
- Funcionalidad de codificador AAC actualizada

### Versión 10.0

- Compatibilidad completa con SDKs .NET de VisioForge 10.0
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


## Transcripción del vídeo

### Qué veremos

Vamos a cifrar archivos de video con una contraseña en Windows. Primero con la aplicación lista para usar VisioForge Video Encryptor, y después en su propio código C# con Video Encryption SDK. Este es el plan. Video Encryptor cifra un archivo con una contraseña, sin escribir código. Su vista avanzada añade una lista de archivos, la carpeta de salida y los ajustes de los códecs. Después compilamos las demos del SDK en Visual Studio, ciframos un MP4 con AES-256 desde C# y reproducimos el archivo cifrado con la misma contraseña.

### Cómo funciona

¿Cómo funciona? El video se codifica en H.264 y el audio en AAC. El filtro de cifrado cifra ambos y los escribe en un único archivo. Solo el filtro de descifrado, con la misma clave, puede volver a leer ese archivo. Los archivos de la versión 9 usan AES-256, los de la versión 8 usan AES-128. La clave es una cadena de texto, y la versión 9 también acepta un archivo o datos binarios como clave. Todos ellos son filtros DirectShow, así que funcionan en cualquier grafo DirectShow. Necesita Windows 10 u 11. Para la parte de la aplicación, instale Video Encryptor. Para el código, instale Video Encryption SDK: registra los filtros e incluye LAV Filters, que la demo usa para leer archivos MP4. Después, Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET, y las demos del repositorio directshow-samples en GitHub. Ambos instaladores están en visioforge.com.

### Video Encryptor: vista simple

Video Encryptor se abre en la vista simple. Primero escriba una contraseña. Después haga clic en el candado, o suelte un archivo sobre él, y elija el video. La versión sin registrar muestra algunos avisos de prueba antes de empezar; aquí están recortados. El archivo cifrado queda junto al original, con la extensión .evf. Cifrado completado.

### Video Encryptor: vista avanzada

La vista avanzada acepta una lista de archivos. Haga clic en Add files y elija uno o varios videos. En la pestaña Settings, elija la carpeta de salida: la carpeta del original o una carpeta propia. Output format contiene los ajustes H.264 del video, con la tasa de bits, y los ajustes AAC del audio. Escriba la contraseña y haga clic en Start. La vista previa muestra el video mientras se cifra, y la barra de progreso avanza hasta el final. Listo: un segundo archivo .evf, junto a su original.

### Las demos del SDK

El SDK le da a su propia aplicación los mismos filtros. Su Encryption Demo cifra un archivo de video, o una cámara, en un archivo cifrado. Su Player Demo reproduce un archivo cifrado con su clave. Ambas son aplicaciones C# WinForms en .NET 8, para x64, y acceden a DirectShow mediante el paquete NuGet VisioForge.DirectShowAPI. Abra VideoEncryptionSDK.sln del repositorio y compile la solución. NuGet restaura el paquete y ambas demos se compilan.

### Cifrar: los filtros

El grafo de cifrado empieza en AddEncryptor. Añade el filtro de cifrado por su ID de clase: el cifrador de la versión 9 para AES-256, o el de la versión 8 para AES-128. Después, el codificador H.264. Después, el codificador AAC para el audio, y el File Writer, con el nombre del archivo de salida.

### Cifrar: la clave

ApplyEncryptorSettings configura el cifrador. Con una clave de licencia, IVFRegister.SetLicenseKey lo registra y los avisos de prueba desaparecen. IVFCryptoConfig recibe la clave. ApplyString recibe una contraseña, ApplyFile usa cualquier archivo como clave y ApplyBinary recibe una matriz de bytes.

### Cifrar: el grafo

SourceInitFile abre el original con LAV Splitter Source, que separa un MP4 en sus pistas de video y de audio. Si faltan los LAV Filters, la demo lo indica y se detiene. Primero el video se decodifica hacia el filtro RGB2YUV, y solo entonces los codificadores entran en el grafo. Después RenderStream conecta RGB2YUV, a través del codificador H.264, con el cifrador. Si el archivo tiene una pista de audio, también va al cifrador a través del codificador AAC. Un archivo sin audio se cifra solo con video. Por último, el cifrador se conecta con el File Writer.

### Cifrar un archivo

Ejecute la Encryption Demo. Indique el archivo original, el archivo de salida y la contraseña. AES-256 es la opción predeterminada. Haga clic en Start. Sin una clave de licencia, el SDK muestra primero un aviso de prueba; aquí está recortado. Treinta segundos de video tardan unos diez segundos, y la demo indica Completed.

### Reproducirlo

Ahora la Player Demo. Un File Source normal lee el archivo cifrado: para él son solo bytes hasta que los ve el descifrador. El filtro de descifrado corresponde al de cifrado: la versión 9 para AES-256. Recibe la clave mediante la misma interfaz IVFCryptoConfig. El origen se conecta con el descifrador. El video del descifrador va al renderizador VMR-9, y su audio, si el archivo lo tiene, al dispositivo de sonido predeterminado. Establezca Player Demo como proyecto de inicio y ejecútela. Elija el archivo cifrado y escriba la misma contraseña. Start, y se reproduce. El mismo reproductor abre el archivo que creó Video Encryptor. Ese es un archivo de la versión 8, así que elija AES-128. En la versión de prueba, este muestra una pequeña marca Unregistered en una esquina.

### Conviene saber

Algunas cosas que conviene saber. La contraseña es la clave: sin ella no hay vuelta atrás. El descifrador debe corresponder al archivo, versión 9 o versión 8. Una clave incorrecta falla: el grafo no puede conectar la salida del descifrador. Con una licencia, SetLicenseKey registra los filtros y los avisos de prueba desaparecen. Y la Encryption Demo también cifra una cámara: elija Device en lugar de Video file.

### Próximos pasos

Las demos están en GitHub, y la documentación está en visioforge.com/help, con una referencia de todas las interfaces de los filtros. Gracias por ver el video.

---

## Ver También

- [Referencia de Interfaz](interface-reference.md) - Documentación completa de API
- [Ejemplos](examples.md) - Ejemplos de código completos para encriptación y desencriptación
- [DirectShow Encoding Filters Pack](../filters-enc/index.md) - Codificadores de video compatibles (H.264, H.265, AAC)
