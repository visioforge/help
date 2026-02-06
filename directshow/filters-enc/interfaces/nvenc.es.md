---
title: Codificador NVENC - Referencia de Interfaz
description: Interfaz INVEncConfig para codificación de video por hardware NVIDIA NVENC con códecs H.264 y H.265 y configuración de aceleración GPU.
---

# Referencia de Interfaz INVEncConfig

## Descripción General

La interfaz `INVEncConfig` proporciona control completo sobre la codificación de video por hardware NVIDIA NVENC. Esta interfaz extiende la interfaz estándar de DirectShow `IAMVideoCompression` con opciones de configuración específicas de NVENC para codificación H.264 y H.265.

NVENC es el codificador dedicado por hardware de NVIDIA disponible en GPUs GeForce, Quadro y Tesla, ofreciendo codificación de video de alto rendimiento con uso mínimo de CPU.

## GUIDs de Filtro e Interfaz

- **CLSID de Filtro**: `CLSID_NVEncoder`
  `{6EEC9161-7276-430B-A1197-0D4C3BCC87E5}`

- **Interfaz**: `INVEncConfig`
  **GUID**: `{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}`
  **Hereda De**: `IAMVideoCompression`
  **Archivo de Cabecera**: `Intf.h` (C++)

- **Interfaz**: `INVEncConfig2`
  **GUID**: `{2A741FB6-6DE1-460B-8FCA-76DB478C9357}`
  **Hereda De**: `IUnknown`
  **Archivo de Cabecera**: `Intf2.h` (C++)

## Definiciones de Interfaz

### Definición C++ (INVEncConfig)

```cpp
#include <strmif.h>

// {9A2AC42C-3E3D-4E6A-84E5-D097292D496B}
static const GUID IID_INVEncConfig =
{ 0x9a2ac42c, 0x3e3d, 0x4e6a, { 0x84, 0xe5, 0xd0, 0x97, 0x29, 0x2d, 0x49, 0x6b } };

// {6EEC9161-7276-430B-A1197-0D4C3BCC87E5}
static const GUID CLSID_NVEncoder =
{ 0x6eec9161, 0x7276, 0x430b, { 0xa1, 0x97, 0xd, 0x4c, 0x3b, 0xcc, 0x87, 0xe5 } };

MIDL_INTERFACE("9A2AC42C-3E3D-4E6A-84E5-D097292D496B")
INVEncConfig : public IAMVideoCompression
{
public:
    virtual HRESULT STDMETHODCALLTYPE SetDeviceType(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetDeviceType(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetPictureStructure(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetPictureStructure(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetNumBuffers(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetNumBuffers(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetRateControl(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetRateControl(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetPreset(GUID v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetPreset(GUID *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetQp(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetQp(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetBFrames(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetBFrames(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetGOP(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetGOP(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetBitrate(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetBitrate(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetVbvBitrate(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetVbvBitrate(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetVbvSize(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetVbvSize(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetProfile(GUID v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetProfile(GUID *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetLevel(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetLevel(int *v) = 0;

    virtual HRESULT STDMETHODCALLTYPE SetCodec(int v) = 0;
    virtual HRESULT STDMETHODCALLTYPE GetCodec(int *v) = 0;
};
```

### Definición C# (INVEncConfig)

```csharp
using System;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interfaz de configuración del codificador NVENC.
    /// Proporciona codificación H.264/H.265 acelerada por hardware en GPUs NVIDIA.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("9A2AC42C-3E3D-4E6A-84E5-D097292D496B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INVEncConfig
    {
        // Nota: También hereda métodos IAMVideoCompression
        // (put_KeyFrameRate, get_KeyFrameRate, put_PFramesPerKeyFrame, etc.)

        /// <summary>Establece el índice de dispositivo CUDA para codificación.</summary>
        /// <param name="v">Índice de dispositivo (0 para primera GPU, 1 para segunda, etc.)</param>
        [PreserveSig]
        int SetDeviceType(int v);

        /// <summary>Obtiene el índice de dispositivo CUDA.</summary>
        [PreserveSig]
        int GetDeviceType(out int v);

        /// <summary>Establece la estructura de imagen (progresiva o entrelazada).</summary>
        /// <param name="v">0 = Progresiva, 1 = Entrelazada</param>
        [PreserveSig]
        int SetPictureStructure(int v);

        /// <summary>Obtiene la estructura de imagen.</summary>
        [PreserveSig]
        int GetPictureStructure(out int v);

        /// <summary>Establece el número de buffers de codificación.</summary>
        /// <param name="v">Número de buffers (típicamente 4-8)</param>
        [PreserveSig]
        int SetNumBuffers(int v);

        /// <summary>Obtiene el número de buffers de codificación.</summary>
        [PreserveSig]
        int GetNumBuffers(out int v);

        /// <summary>Establece el modo de control de tasa.</summary>
        /// <param name="v">0 = CQP, 1 = VBR, 2 = CBR</param>
        [PreserveSig]
        int SetRateControl(int v);

        /// <summary>Obtiene el modo de control de tasa.</summary>
        [PreserveSig]
        int GetRateControl(out int v);

        /// <summary>Establece el preset de codificación.</summary>
        /// <param name="v">GUID de preset (P1-P7)</param>
        [PreserveSig]
        int SetPreset(Guid v);

        /// <summary>Obtiene el preset de codificación.</summary>
        [PreserveSig]
        int GetPreset(out Guid v);

        /// <summary>Establece el parámetro de cuantización para modo CQP.</summary>
        /// <param name="v">Valor QP (0-51, menor = mayor calidad)</param>
        [PreserveSig]
        int SetQp(int v);

        /// <summary>Obtiene el parámetro de cuantización.</summary>
        [PreserveSig]
        int GetQp(out int v);

        /// <summary>Establece el número de B-frames.</summary>
        /// <param name="v">Número de B-frames (0-4)</param>
        [PreserveSig]
        int SetBFrames(int v);

        /// <summary>Obtiene el número de B-frames.</summary>
        [PreserveSig]
        int GetBFrames(out int v);

        /// <summary>Establece el tamaño GOP (Grupo de Imágenes).</summary>
        /// <param name="v">Tamaño GOP en frames</param>
        [PreserveSig]
        int SetGOP(int v);

        /// <summary>Obtiene el tamaño GOP.</summary>
        [PreserveSig]
        int GetGOP(out int v);

        /// <summary>Establece la tasa de bits objetivo.</summary>
        /// <param name="v">Tasa de bits en bits por segundo</param>
        [PreserveSig]
        int SetBitrate(int v);

        /// <summary>Obtiene la tasa de bits objetivo.</summary>
        [PreserveSig]
        int GetBitrate(out int v);

        /// <summary>Establece la tasa de bits del buffer VBV.</summary>
        /// <param name="v">Tasa de bits VBV en bps</param>
        [PreserveSig]
        int SetVbvBitrate(int v);

        /// <summary>Obtiene la tasa de bits del buffer VBV.</summary>
        [PreserveSig]
        int GetVbvBitrate(out int v);

        /// <summary>Establece el tamaño del buffer VBV.</summary>
        /// <param name="v">Tamaño VBV en bits</param>
        [PreserveSig]
        int SetVbvSize(int v);

        /// <summary>Obtiene el tamaño del buffer VBV.</summary>
        [PreserveSig]
        int GetVbvSize(out int v);

        /// <summary>Establece el perfil de codificación.</summary>
        /// <param name="v">GUID de perfil (Baseline, Main, High, etc.)</param>
        [PreserveSig]
        int SetProfile(Guid v);

        /// <summary>Obtiene el perfil de codificación.</summary>
        [PreserveSig]
        int GetProfile(out Guid v);

        /// <summary>Establece el nivel de perfil.</summary>
        /// <param name="v">Valor de nivel (30, 31, 40, 41, 50, 51, etc.)</param>
        [PreserveSig]
        int SetLevel(int v);

        /// <summary>Obtiene el nivel de perfil.</summary>
        [PreserveSig]
        int GetLevel(out int v);

        /// <summary>Establece el códec de video.</summary>
        /// <param name="v">0 = H.264, 1 = H.265</param>
        [PreserveSig]
        int SetCodec(int v);

        /// <summary>Obtiene el códec de video.</summary>
        [PreserveSig]
        int GetCodec(out int v);
    }

    /// <summary>
    /// Interfaz de configuración NVENC 2 - verificación de disponibilidad.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("2A741FB6-6DE1-460B-8FCA-76DB478C9357")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INVEncConfig2
    {
        /// <summary>Verifica si NVENC está disponible en el sistema.</summary>
        /// <param name="result">Verdadero si NVENC está disponible</param>
        /// <param name="status">Código de estado NVENC</param>
        [PreserveSig]
        int CheckNVENCAvailable([MarshalAs(UnmanagedType.Bool)] out bool result, out int status);
    }
}
```

### Definición Delphi (INVEncConfig)

```delphi
uses
  ActiveX, ComObj;

const
  IID_INVEncConfig: TGUID = '{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}';
  IID_INVEncConfig2: TGUID = '{2A741FB6-6DE1-460B-8FCA-76DB478C9357}';
  CLSID_NVEncoder: TGUID = '{6EEC9161-7276-430B-A1197-0D4C3BCC87E5}';

type
  /// <summary>
  /// Interfaz de configuración del codificador NVENC.
  /// Extiende IAMVideoCompression con configuraciones específicas de NVENC.
  /// </summary>
  INVEncConfig = interface(IUnknown)
    ['{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}']

    // Nota: También hereda métodos IAMVideoCompression

    function SetDeviceType(v: Integer): HRESULT; stdcall;
    function GetDeviceType(out v: Integer): HRESULT; stdcall;

    function SetPictureStructure(v: Integer): HRESULT; stdcall;
    function GetPictureStructure(out v: Integer): HRESULT; stdcall;

    function SetNumBuffers(v: Integer): HRESULT; stdcall;
    function GetNumBuffers(out v: Integer): HRESULT; stdcall;

    function SetRateControl(v: Integer): HRESULT; stdcall;
    function GetRateControl(out v: Integer): HRESULT; stdcall;

    function SetPreset(v: TGUID): HRESULT; stdcall;
    function GetPreset(out v: TGUID): HRESULT; stdcall;

    function SetQp(v: Integer): HRESULT; stdcall;
    function GetQp(out v: Integer): HRESULT; stdcall;

    function SetBFrames(v: Integer): HRESULT; stdcall;
    function GetBFrames(out v: Integer): HRESULT; stdcall;

    function SetGOP(v: Integer): HRESULT; stdcall;
    function GetGOP(out v: Integer): HRESULT; stdcall;

    function SetBitrate(v: Integer): HRESULT; stdcall;
    function GetBitrate(out v: Integer): HRESULT; stdcall;

    function SetVbvBitrate(v: Integer): HRESULT; stdcall;
    function GetVbvBitrate(out v: Integer): HRESULT; stdcall;

    function SetVbvSize(v: Integer): HRESULT; stdcall;
    function GetVbvSize(out v: Integer): HRESULT; stdcall;

    function SetProfile(v: TGUID): HRESULT; stdcall;
    function GetProfile(out v: TGUID): HRESULT; stdcall;

    function SetLevel(v: Integer): HRESULT; stdcall;
    function GetLevel(out v: Integer): HRESULT; stdcall;

    function SetCodec(v: Integer): HRESULT; stdcall;
    function GetCodec(out v: Integer): HRESULT; stdcall;
  end;

  /// <summary>
  /// Interfaz de configuración NVENC 2 - verificación de disponibilidad.
  /// </summary>
  INVEncConfig2 = interface(IUnknown)
    ['{2A741FB6-6DE1-460B-8FCA-76DB478C9357}']

    function CheckNVENCAvailable(out result: BOOL; out status: Integer): HRESULT; stdcall;
  end;
```

## Requisitos de Hardware

### Generaciones de GPU

| Generación de GPU | H.264 | H.265 | Calidad | Notas |
|-------------------|-------|-------|---------|-------|
| **Kepler** (GTX 600/700) | ✓ | ✗ | Básica | 1ª generación NVENC |
| **Maxwell** (GTX 900) | ✓ | ✓ | Buena | 2ª gen, soporte HEVC añadido |
| **Pascal** (GTX 10XX) | ✓ | ✓ | Mejor | 3ª gen, calidad mejorada |
| **Turing** (RTX 20XX) | ✓ | ✓ | Excelente | 7ª gen, soporte B-frame |
| **Ampere** (RTX 30XX) | ✓ | ✓ | Excelente | 8ª gen, soporte AV1 |
| **Ada/Hopper** (RTX 40XX) | ✓ | ✓ | Mejor | Última generación |

### Capacidades de Rendimiento

- **1080p @ 60fps**: Todas las generaciones NVENC
- **4K @ 60fps**: Maxwell y más recientes
- **8K @ 30fps**: Turing y más recientes
- **Streams Simultáneos**: 3-5 (varía por GPU)

---
## Referencia de Métodos
Todos los métodos heredados de `IAMVideoCompression` están disponibles. Los siguientes son extensiones específicas de NVENC:
### Configuración de Dispositivo
#### SetDeviceType / GetDeviceType
Establece o recupera el índice de dispositivo CUDA para codificación.
**Sintaxis (C++)**:
```cpp
HRESULT SetDeviceType(int v);
HRESULT GetDeviceType(int *v);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int SetDeviceType(int v);
[PreserveSig]
int GetDeviceType(out int v);
```
**Parámetros**:
- `v`: Índice de dispositivo CUDA (0 para primera GPU, 1 para segunda, etc.)
**Devuelve**: `S_OK` (0) en caso de éxito.
**Notas de Uso**:
- Debe llamarse **antes** de conectar el filtro codificador
- Usar 0 para sistemas con GPU única
- Para sistemas multi-GPU, seleccionar la GPU a usar para codificación
- Consultar dispositivos CUDA disponibles usando API CUDA o herramientas NVIDIA
**Ejemplo (C++)**:
```cpp
INVEncConfig* pNVEnc = nullptr;
pFilter->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
// Usar primera GPU
pNVEnc->SetDeviceType(0);
pNVEnc->Release();
```
---

### Estructura de Imagen

#### SetPictureStructure / GetPictureStructure

Establece el tipo de codificación de imagen (progresiva o entrelazada).

**Sintaxis (C++)**:
```cpp
HRESULT SetPictureStructure(int v);
HRESULT GetPictureStructure(int *v);
```

**Parámetros**:
- `v`: Tipo de estructura de imagen
  - `0` - Progresiva (basada en frame)
  - `1` - Entrelazada (basada en campo)

**Devuelve**: `S_OK` en caso de éxito.

**Notas de Uso**:
- Predeterminado es progresiva (0)
- Usar entrelazada (1) solo para contenido de broadcast/DVD
- Progresiva recomendada para contenido moderno

**Ejemplo (C++)**:
```cpp
// Establecer codificación progresiva
pNVEnc->SetPictureStructure(0);
```

---
### Configuración de Buffer
#### SetNumBuffers / GetNumBuffers
Establece el número de buffers de codificación.
**Sintaxis (C++)**:
```cpp
HRESULT SetNumBuffers(int v);
HRESULT GetNumBuffers(int *v);
```
**Parámetros**:
- `v`: Número de buffers (típicamente 4-8)
**Devuelve**: `S_OK` en caso de éxito.
**Notas de Uso**:
- Más buffers = mayor latencia pero codificación más suave
- Menos buffers = menor latencia pero posibles caídas de frames
- Valores recomendados:
  - Baja latencia: 4 buffers
  - Normal: 6 buffers
  - Alta calidad: 8 buffers
**Ejemplo (C++)**:
```cpp
// Configuración de baja latencia
pNVEnc->SetNumBuffers(4);
```
---

### Control de Tasa

#### SetRateControl / GetRateControl

Establece el modo de control de tasa para gestión de bitrate.

**Sintaxis (C++)**:
```cpp
HRESULT SetRateControl(int v);
HRESULT GetRateControl(int *v);
```

**Parámetros**:
- `v`: Modo de control de tasa
  - `0` - **CQP** (Parámetro de Cuantización Constante) - Calidad fija
  - `1` - **VBR** (Bitrate Variable) - Bitrate variable, calidad objetivo
  - `2` - **CBR** (Bitrate Constante) - Bitrate fijo para streaming

**Devuelve**: `S_OK` en caso de éxito.

**Detalles del Modo de Control de Tasa**:

| Modo | Comportamiento de Bitrate | Caso de Uso | Calidad | Tamaño de Archivo |
|------|---------------------------|-------------|---------|-------------------|
| **CQP** | Varía ampliamente | Archival, máxima calidad | Excelente | Impredecible |
| **VBR** | Varía moderadamente | Almacenamiento de archivos, YouTube | Muy Buena | Moderado |
| **CBR** | Constante | Streaming en vivo, broadcasting | Buena | Predecible |

**Ejemplo (C++)**:
```cpp
// Usar CBR para streaming en vivo
pNVEnc->SetRateControl(2);
pNVEnc->SetBitrate(5000000); // 5 Mbps
```

**Ejemplo (C#)**:
```csharp
// Usar VBR para grabación de archivos
nvenc.SetRateControl(1);
nvenc.SetBitrate(8000000); // 8 Mbps objetivo
```

---
### Configuración de Preset
#### SetPreset / GetPreset
Establece el preset de codificación que equilibra velocidad y calidad.
**Sintaxis (C++)**:
```cpp
HRESULT SetPreset(GUID v);
HRESULT GetPreset(GUID *v);
```
**Parámetros**:
- `v`: GUID de preset del SDK NVENC
**Opciones de Preset** (valores típicos):
| Preset | Descripción | Velocidad | Calidad | Caso de Uso |
|--------|-------------|-----------|---------|-------------|
| **P1** | Más rápido | ★★★★★ | ★☆☆☆☆ | Tiempo real baja latencia |
| **P2** | Más rápido | ★★★★☆ | ★★☆☆☆ | Streaming en vivo |
| **P3** | Rápido | ★★★☆☆ | ★★★☆☆ | Streaming estándar |
| **P4** | Medio | ★★☆☆☆ | ★★★★☆ | Equilibrado (recomendado) |
| **P5** | Lento | ★☆☆☆☆ | ★★★★☆ | Streaming de alta calidad |
| **P6** | Más lento | ☆☆☆☆☆ | ★★★★★ | Calidad de archivo |
| **P7** | Más lento | ☆☆☆☆☆ | ★★★★★ | Máxima calidad |
**Notas de Uso**:
- P4 es recomendado para la mayoría de casos de uso
- P1-P2 para aplicaciones de baja latencia
- P6-P7 para máxima calidad (codificación más lenta)
- Preset afecta: estimación de movimiento, lookahead, movimiento subpíxel
**Ejemplo (C++)**:
```cpp
// Usar preset P4 (equilibrado)
GUID presetP4 = /* GUID para P4 */;
pNVEnc->SetPreset(presetP4);
```
---

### Parámetro de Calidad (QP)

#### SetQp / GetQp

Establece el parámetro de cuantización para modo CQP.

**Sintaxis (C++)**:
```cpp
HRESULT SetQp(int v);
HRESULT GetQp(int *v);
```

**Parámetros**:
- `v`: Valor QP (0-51)
  - Valores menores = mayor calidad, archivos más grandes
  - Valores mayores = menor calidad, archivos más pequeños
  - Rango típico: 18-28

**Devuelve**: `S_OK` en caso de éxito.

**Notas de Uso**:
- Solo efectivo cuando se usa modo de control de tasa CQP
- Ignorado en modos CBR/VBR
- Valores recomendados:
  - Alta calidad: 18-22
  - Calidad media: 23-26
  - Baja calidad: 27-30

**Ejemplo (C++)**:
```cpp
// Codificación CQP de alta calidad
pNVEnc->SetRateControl(0); // Modo CQP
pNVEnc->SetQp(20);         // Alta calidad
```

---
### Configuración de B-Frames
#### SetBFrames / GetBFrames
Establece el número de B-frames entre frames I y P.
**Sintaxis (C++)**:
```cpp
HRESULT SetBFrames(int v);
HRESULT GetBFrames(int *v);
```
**Parámetros**:
- `v`: Número de B-frames (0-4)
  - `0` - Sin B-frames (menor latencia)
  - `1-2` - Mejora moderada de compresión
  - `3-4` - Mejor compresión (mayor latencia)
**Devuelve**: `S_OK` en caso de éxito.
**Notas de Uso**:
- B-frames mejoran la eficiencia de compresión
- Más B-frames = mayor latencia
- Requiere Turing (RTX 20XX) o más reciente para soporte completo
- Valores recomendados:
  - Baja latencia: 0
  - Streaming: 2
  - Grabación: 3
**Ejemplo (C++)**:
```cpp
// Baja latencia - deshabilitar B-frames
pNVEnc->SetBFrames(0);
// Alta calidad grabación - usar B-frames
pNVEnc->SetBFrames(3);
```
---

### Configuración GOP

#### SetGOP / GetGOP

Establece el tamaño del Grupo de Imágenes (keyframe interval).

**Sintaxis (C++)**:
```cpp
HRESULT SetGOP(int v);
HRESULT GetGOP(int *v);
```

**Parámetros**:
- `v`: Tamaño GOP en frames
  - Valores típicos: 30-300 frames
  - Frame rate × segundos = tamaño GOP
  - Ejemplo: 60 fps × 2 segundos = 120 tamaño GOP

**Devuelve**: `S_OK` en caso de éxito.

**Notas de Uso**:
- GOP más pequeño = mejor búsqueda, archivo más grande
- GOP más grande = mejor compresión, búsqueda pobre
- Para streaming: 2-4 segundos (fps × 2-4)
- Para grabación: 5-10 segundos

**Ejemplo (C++)**:
```cpp
// GOP de 2 segundos para streaming 30fps
pNVEnc->SetGOP(60);

// GOP de 5 segundos para grabación 60fps
pNVEnc->SetGOP(300);
```

---
### Configuración de Bitrate
#### SetBitrate / GetBitrate
Establece el bitrate objetivo para codificación.
**Sintaxis (C++)**:
```cpp
HRESULT SetBitrate(int v);
HRESULT GetBitrate(int *v);
```
**Parámetros**:
- `v`: Bitrate en bits por segundo (bps)
**Devuelve**: `S_OK` en caso de éxito.
**Bitrates Recomendados**:
| Resolución | Frame rate | Bitrate (H.264) | Bitrate (H.265) |
|------------|------------|-----------------|-----------------|
| 720p | 30 fps | 2.5-4 Mbps | 1.5-2.5 Mbps |
| 720p | 60 fps | 4-6 Mbps | 2.5-4 Mbps |
| 1080p | 30 fps | 4-6 Mbps | 2.5-4 Mbps |
| 1080p | 60 fps | 8-12 Mbps | 5-8 Mbps |
| 1440p | 30 fps | 10-15 Mbps | 6-10 Mbps |
| 1440p | 60 fps | 15-25 Mbps | 10-15 Mbps |
| 4K | 30 fps | 25-40 Mbps | 15-25 Mbps |
| 4K | 60 fps | 45-70 Mbps | 30-45 Mbps |
**Ejemplo (C++)**:
```cpp
// Streaming 1080p @ 60fps
pNVEnc->SetBitrate(10000000); // 10 Mbps
```
---

### Configuración de Buffer VBV

#### SetVbvBitrate / GetVbvBitrate

Establece el bitrate del buffer VBV (Video Buffering Verifier).

**Sintaxis (C++)**:
```cpp
HRESULT SetVbvBitrate(int v);
HRESULT GetVbvBitrate(int *v);
```

**Parámetros**:
- `v`: Bitrate VBV en bps (usualmente igual o mayor que el bitrate objetivo)

**Notas de Uso**:
- Controla los picos máximos de bitrate
- Típicamente establecido en 1.0-1.5× bitrate objetivo
- Importante para streaming para prevenir underruns de buffer

---
#### SetVbvSize / GetVbvSize
Establece el tamaño del buffer VBV.
**Sintaxis (C++)**:
```cpp
HRESULT SetVbvSize(int v);
HRESULT GetVbvSize(int *v);
```
**Parámetros**:
- `v`: Tamaño del buffer VBV en bits
**Notas de Uso**:
- Buffer más grande = bitrate más suave pero mayor latencia
- Buffer más pequeño = menor latencia pero más varianza de bitrate
- Típico: 1-2 segundos de video a bitrate objetivo
**Ejemplo (C++)**:
```cpp
// Stream de 10 Mbps con buffer de 2 segundos
pNVEnc->SetBitrate(10000000);
pNVEnc->SetVbvBitrate(12000000);  // 1.2× bitrate
pNVEnc->SetVbvSize(20000000);     // 2 segundos
```
---

### Configuración de Perfil

#### SetProfile / GetProfile

Establece el perfil de codificación H.264/H.265.

**Sintaxis (C++)**:
```cpp
HRESULT SetProfile(GUID v);
HRESULT GetProfile(GUID *v);
```

**Parámetros**:
- `v`: GUID de perfil

**Perfiles H.264**:
- **Baseline** - Características básicas, compatibilidad móvil
- **Main** - Características estándar, mayoría de dispositivos
- **High** - Características avanzadas, contenido HD/4K

**Perfiles H.265**:
- **Main** - 8-bit, 4:2:0
- **Main 10** - 10-bit, soporte HDR

**Notas de Uso**:
- Usar perfil High para H.264 en la mayoría de casos
- Usar perfil Main para máxima compatibilidad
- HEVC Main 10 para contenido HDR

---
### Configuración de Nivel
#### SetLevel / GetLevel
Establece el nivel de perfil (restricciones de resolución/bitrate).
**Sintaxis (C++)**:
```cpp
HRESULT SetLevel(int v);
HRESULT GetLevel(int *v);
```
**Parámetros**:
- `v`: Valor de nivel (ver tabla de niveles H.264/H.265)
**Niveles H.264 Comunes**:
- **30** (3.0) - Video SD
- **31** (3.1) - 720p @ 30fps
- **40** (4.0) - 1080p @ 30fps
- **41** (4.1) - 1080p @ 60fps
- **50** (5.0) - 4K @ 30fps
- **51** (5.1) - 4K @ 60fps
**Ejemplo (C++)**:
```cpp
// 1080p @ 60fps
pNVEnc->SetLevel(41);
```
---

### Selección de Códec

#### SetCodec / GetCodec

Establece el códec de video a usar.

**Sintaxis (C++)**:
```cpp
HRESULT SetCodec(int v);
HRESULT GetCodec(int *v);
```

**Parámetros**:
- `v`: Tipo de códec
  - `0` - H.264/AVC
  - `1` - H.265/HEVC

**Devuelve**: `S_OK` en caso de éxito.

**Notas de Uso**:
- H.264 para máxima compatibilidad
- H.265 para mejor compresión (40-50% archivos más pequeños)
- H.265 requiere GPU Maxwell (GTX 900) o más reciente

**Ejemplo (C++)**:
```cpp
// Usar H.265
pNVEnc->SetCodec(1);
```

---
## Métodos INVEncConfig2
### CheckNVENCAvailable
Verifica si la codificación por hardware NVENC está disponible en el sistema.
**Sintaxis (C++)**:
```cpp
HRESULT CheckNVENCAvailable(BOOL* result, int* status);
```
**Sintaxis (C#)**:
```csharp
[PreserveSig]
int CheckNVENCAvailable([MarshalAs(UnmanagedType.Bool)] out bool result, out int status);
```
**Parámetros**:
- `result`: Recibe `TRUE` si NVENC está disponible, `FALSE` en caso contrario
- `status`: Recibe código de estado NVENC (específico del proveedor)
**Devuelve**: `S_OK` (0) en caso de éxito.
**Notas de Uso**:
- Llamar esto antes de intentar usar el codificador NVENC
- Devuelve `FALSE` si:
  - No hay GPU NVIDIA presente
  - GPU no soporta NVENC (pre-Kepler)
  - Controladores NVIDIA no instalados
  - Biblioteca NVENC no disponible
- El código de estado proporciona información diagnóstica adicional
**Ejemplo (C++)**:
```cpp
#include "Intf2.h"
HRESULT CheckNVENCSupport(IBaseFilter* pEncoder)
{
    HRESULT hr;
    INVEncConfig2* pNVEnc2 = nullptr;
    hr = pEncoder->QueryInterface(IID_INVEncConfig2, (void**)&pNVEnc2);
    if (FAILED(hr))
    {
        // INVEncConfig2 no soportado por este filtro
        return hr;
    }
    BOOL available = FALSE;
    int status = 0;
    hr = pNVEnc2->CheckNVENCAvailable(&available, &status);
    if (SUCCEEDED(hr))
    {
        if (available)
        {
            printf("NVENC está disponible (estado: %d)\n", status);
            // Proceder con configuración NVENC
        }
        else
        {
            printf("NVENC no disponible (estado: %d)\n", status);
            // Retroceder a codificador software
        }
    }
    pNVEnc2->Release();
    return hr;
}
```
**Ejemplo (C#)**:
```csharp
using VisioForge.DirectShowAPI;
public bool IsNVENCAvailable(IBaseFilter encoder)
{
    var nvenc2 = encoder as INVEncConfig2;
    if (nvenc2 == null)
    {
        // INVEncConfig2 no soportado
        return false;
    }
    bool available;
    int status;
    int hr = nvenc2.CheckNVENCAvailable(out available, out status);
    if (hr == 0)
    {
        if (available)
        {
            Console.WriteLine($"NVENC está disponible (estado: {status})");
            return true;
        }
        else
        {
            Console.WriteLine($"NVENC no disponible (estado: {status})");
            return false;
        }
    }
    return false;
}
```
**Ejemplo (Delphi)**:
```delphi
function CheckNVENCSupport(Encoder: IBaseFilter): Boolean;
var
  NVEnc2: INVEncConfig2;
  Available: BOOL;
  Status: Integer;
  hr: HRESULT;
begin
  Result := False;
  if Succeeded(Encoder.QueryInterface(IID_INVEncConfig2, NVEnc2)) then
  begin
    hr := NVEnc2.CheckNVENCAvailable(Available, Status);
    if Succeeded(hr) then
    begin
      if Available then
      begin
        WriteLn(Format('NVENC está disponible (estado: %d)', [Status]));
        Result := True;
      end
      else
      begin
        WriteLn(Format('NVENC no disponible (estado: %d)', [Status]));
      end;
    end;
    NVEnc2 := nil;
  end;
end;
```
---

## Ejemplos de Configuración Completa

### Ejemplo 1: Streaming de Baja Latencia (C++)

```cpp
#include "Intf.h"

HRESULT ConfigureLowLatencyNVENC(IBaseFilter* pEncoder)
{
    HRESULT hr;
    INVEncConfig* pNVEnc = nullptr;

    hr = pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
    if (FAILED(hr))
        return hr;

    // Configuración básica
    pNVEnc->SetDeviceType(0);           // Primera GPU
    pNVEnc->SetCodec(0);                // H.264
    pNVEnc->SetPictureStructure(0);     // Progresiva

    // Configuraciones de baja latencia
    pNVEnc->SetRateControl(2);          // CBR
    pNVEnc->SetBitrate(5000000);        // 5 Mbps
    pNVEnc->SetBFrames(0);              // Sin B-frames
    pNVEnc->SetGOP(60);                 // GOP de 2 segundos (30fps)
    pNVEnc->SetNumBuffers(4);           // Buffering mínimo

    // Preset rápido
    GUID presetP2 = /* GUID P2 */;
    pNVEnc->SetPreset(presetP2);

    // Perfil/Nivel para 1080p30
    GUID highProfile = /* GUID High Profile */;
    pNVEnc->SetProfile(highProfile);
    pNVEnc->SetLevel(40);               // Nivel 4.0

    pNVEnc->Release();
    return S_OK;
}
```

### Ejemplo 2: Grabación de Alta Calidad (C#)

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class NVENCHighQualityRecording
{
    public void ConfigureNVENC(IBaseFilter encoder)
    {
        var nvenc = encoder as INVEncConfig;
        if (nvenc == null)
            throw new NotSupportedException("NVENC no disponible");

        // Configuración básica
        nvenc.SetDeviceType(0);          // Primera GPU
        nvenc.SetCodec(1);               // H.265 para mejor compresión
        nvenc.SetPictureStructure(0);    // Progresiva

        // Configuraciones VBR de alta calidad
        nvenc.SetRateControl(1);         // VBR
        nvenc.SetBitrate(15000000);      // 15 Mbps promedio
        nvenc.SetBFrames(3);             // Usar B-frames
        nvenc.SetGOP(300);               // GOP de 5 segundos (60fps)
        nvenc.SetNumBuffers(8);          // Más buffering para calidad

        // Preset de calidad
        Guid presetP6 = /* GUID P6 */;
        nvenc.SetPreset(presetP6);

        // Perfil HEVC Main para 4K
        Guid hevcMain = /* GUID HEVC Main */;
        nvenc.SetProfile(hevcMain);
        nvenc.SetLevel(51);              // Nivel 5.1 para 4K60

        // Configuración VBV
        nvenc.SetVbvBitrate(20000000);   // 20 Mbps máximo
        nvenc.SetVbvSize(30000000);      // Buffer de 2 segundos
    }
}
```

### Ejemplo 3: Streaming Equilibrado (C++)

```cpp
HRESULT ConfigureBalancedStreaming(IBaseFilter* pEncoder)
{
    INVEncConfig* pNVEnc = nullptr;
    pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);

    // Dispositivo y códec
    pNVEnc->SetDeviceType(0);
    pNVEnc->SetCodec(0);                // H.264 para compatibilidad

    // Streaming CBR equilibrado
    pNVEnc->SetRateControl(2);          // CBR
    pNVEnc->SetBitrate(8000000);        // 8 Mbps
    pNVEnc->SetBFrames(2);              // B-frames moderados
    pNVEnc->SetGOP(120);                // GOP de 2 segundos (60fps)
    pNVEnc->SetNumBuffers(6);           // Buffering estándar

    // Preset P4 equilibrado
    GUID presetP4 = /* GUID P4 */;
    pNVEnc->SetPreset(presetP4);

    // Perfil/nivel 1080p60
    GUID highProfile = /* GUID High Profile */;
    pNVEnc->SetProfile(highProfile);
    pNVEnc->SetLevel(41);

    // VBV para streaming
    pNVEnc->SetVbvBitrate(10000000);    // 1.25× bitrate
    pNVEnc->SetVbvSize(16000000);       // Buffer de 2 segundos

    pNVEnc->Release();
    return S_OK;
}
```

---
## Mejores Prácticas
### Recomendaciones Generales
1. **Usar preset P4 como predeterminado** - Mejor equilibrio de calidad y rendimiento
2. **CBR para streaming** - Bitrate predecible para entrega en red
3. **VBR para grabación** - Mejor calidad para almacenamiento de archivos
4. **Deshabilitar B-frames para baja latencia** - Reduce retraso de codificación
5. **Hacer coincidir GOP con framerate** - 2-4 segundos típico (fps × 2-4)
### Optimización de Calidad
1. **Preset más alto = mejor calidad** - Usar P5-P7 cuando el tiempo de codificación lo permita
2. **Más B-frames = mejor compresión** - Usar 3 para grabación
3. **Bitrate apropiado** - No ir demasiado bajo, la calidad sufre significativamente
4. **Tamaño de buffer VBV** - 1-2 segundos a bitrate objetivo
### Optimización de Rendimiento
1. **Preset más bajo = codificación más rápida** - Usar P1-P3 para tiempo real
2. **Deshabilitar B-frames** - Reduce latencia y complejidad
3. **Buffers de codificación más bajos** - Menor latencia pero posibles caídas
4. **Seleccionar GPU apropiada** - Usar SetDeviceType() para sistemas multi-GPU
### Compatibilidad
1. **Usar perfil High H.264** - Máxima compatibilidad
2. **Establecer nivel correcto** - Coincidir resolución y framerate
3. **CBR para streaming** - Más compatible con reproductores/servidores
4. **Tamaño GOP estándar** - 2-4 segundos
---

## Solución de Problemas

### Problema: NVENC No Disponible

**Síntomas**: QueryInterface falla para INVEncConfig

**Soluciones**:
- Verificar GPU NVIDIA instalada
- Verificar generación GPU (Kepler o más reciente requerido)
- Actualizar controladores NVIDIA a versión más reciente
- Verificar filtro DirectShow registrado

### Problema: Salida de Calidad Pobre

**Soluciones**:
```cpp
// Aumentar bitrate
pNVEnc->SetBitrate(15000000);  // Bitrate más alto

// Usar preset mejor
pNVEnc->SetPreset(presetP6);   // Más lento pero mejor

// Añadir B-frames
pNVEnc->SetBFrames(3);         // Mejor compresión
```

### Problema: Alta Latencia

**Soluciones**:
```cpp
// Deshabilitar B-frames
pNVEnc->SetBFrames(0);

// Usar preset rápido
pNVEnc->SetPreset(presetP1);

// Reducir buffers
pNVEnc->SetNumBuffers(4);

// GOP más pequeño
pNVEnc->SetGOP(30);  // 1 segundo a 30fps
```

### Problema: Picos de Bitrate

**Soluciones**:
```cpp
// Usar CBR en lugar de VBR
pNVEnc->SetRateControl(2);

// Configurar VBV correctamente
pNVEnc->SetVbvBitrate(bitrate * 1.2);
pNVEnc->SetVbvSize(bitrate * 2);
```

---
## Benchmarks de Rendimiento
### Rendimiento de Codificación Típico
| Resolución | Preset | Generación GPU | FPS (aprox) |
|------------|--------|----------------|-------------|
| 1080p | P1 | Pascal+ | 200-300 |
| 1080p | P4 | Pascal+ | 150-200 |
| 1080p | P7 | Pascal+ | 60-100 |
| 4K | P1 | Turing+ | 90-120 |
| 4K | P4 | Turing+ | 60-90 |
| 4K | P7 | Turing+ | 30-50 |
### Comparación de Calidad (PSNR)
| Preset | Calidad vs x264 | Velocidad vs x264 |
|--------|-----------------|------------------|
| P1 | -2 dB | 100× más rápido |
| P4 | -0.5 dB | 50× más rápido |
| P7 | ≈ igual | 20× más rápido |
---

## Interfaces Relacionadas

- **IAMVideoCompression** - Interfaz base de compresión DirectShow
- **IBaseFilter** - Interfaz base de filtro DirectShow
- **IMediaControl** - Control de grafo (ejecutar, detener)

## Ver También

- [Visión General del Paquete de Filtros de Codificación](../index.md)
- [Referencia de Códecs](../codecs-reference.md)
- [Ejemplos de Código](../examples.md)
- [Documentación NVENC de NVIDIA](https://developer.nvidia.com/video-codec-sdk)