---
title: Referencia de Interfaces: Encriptación de Video
description: API del SDK de Encriptación de Video con IVFCryptoConfig, IVFPasswordProvider y métodos auxiliares para C++, C# y Delphi con AES-256.
---

# SDK de Encriptación de Video - Referencia de Interfaces

## Descripción General

El SDK de Encriptación de Video proporciona interfaces COM para encriptar y desencriptar archivos de video MP4 con encriptación AES-256. Esta referencia cubre todas las interfaces, métodos y clases auxiliares para desarrolladores de C++, C# y Delphi.

---
## Interfaz IVFCryptoConfig
Interfaz principal para configurar contraseñas y claves de encriptación tanto en el muxer de encriptación como en los filtros demuxer de desencriptación.
### GUID de la Interfaz
```
{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}
```
### Herencia
Hereda de `IUnknown`
---

### Definiciones de la Interfaz

#### Definición en C++

```cpp
#include "encryptor_intf.h"

// {BAA5BD1E-3B30-425e-AB3B-CC20764AC253}
DEFINE_GUID(IID_ICryptoConfig,
    0xbaa5bd1e, 0x3b30, 0x425e, 0xab, 0x3b, 0xcc, 0x20, 0x76, 0x4a, 0xc2, 0x53);

DECLARE_INTERFACE_(ICryptoConfig, IUnknown)
{
    STDMETHOD(put_Provider)(THIS_ IPasswordProvider* pProvider) PURE;
    STDMETHOD(get_Provider)(THIS_ IPasswordProvider** ppProvider) PURE;
    STDMETHOD(put_Password)(THIS_ LPBYTE pBuffer, LONG lSize) PURE;
    STDMETHOD(HavePassword)(THIS_) PURE;
};
```

#### Definición en C#

```csharp
using System;
using System.Runtime.InteropServices;

[ComImport]
[Guid("BAA5BD1E-3B30-425e-AB3B-CC20764AC253")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFCryptoConfig
{
    [PreserveSig]
    int put_Provider([In] IVFPasswordProvider passwordProviderNotUsed);

    [PreserveSig]
    int get_Provider([Out] IVFPasswordProvider passwordProviderNotUsed);

    [PreserveSig]
    int put_Password(IntPtr buffer, [In] int size);

    [PreserveSig]
    int HavePassword();
}
```

#### Definición en Delphi

```delphi
type
  IVFCryptoConfig = interface(IUnknown)
    ['{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}']
    function put_Provider(passwordProviderNotUsed: TObject): HRESULT; stdcall;
    function get_Provider(out passwordProviderNotUsed: TObject): HRESULT; stdcall;
    function put_Password(buffer: PWideChar; size: integer): HRESULT; stdcall;
    function HavePassword(): HRESULT; stdcall;
  end;
```

---
### Métodos
#### put_Provider
Establece una interfaz de callback de proveedor de contraseñas para escenarios de encriptación avanzados.
**Sintaxis en C++**:
```cpp
HRESULT put_Provider(IPasswordProvider* pProvider);
```
**Sintaxis en C#**:
```csharp
int put_Provider(IVFPasswordProvider passwordProvider);
```
**Parámetros**:
- `pProvider` / `passwordProvider` - Interfaz del proveedor de contraseñas que implementa `IVFPasswordProvider`
**Valor de Retorno**:
- `S_OK` (0) en éxito
- HRESULT de error en caso de fallo
**Observaciones**:
Use este método para escenarios avanzados donde necesite:
- Generación dinámica de contraseñas basada en el nombre del archivo
- Provisión de datos de clave binaria a través de un callback
- Funciones de derivación de clave personalizadas
- Claves de encriptación específicas por archivo
- Políticas de contraseña por archivo
Para contraseñas de cadena simples, usar `put_Password` directamente es más sencillo. El proveedor de contraseñas es útil cuando necesita determinación de contraseña en tiempo de ejecución o cuando implementa un sistema de gestión de claves personalizado.
**Casos de Uso de Ejemplo**:
1. **Proveedor de Clave Binaria**: Proporcionar claves de encriptación de 256 bits desde un sistema de gestión de claves
2. **Contraseñas Dinámicas**: Generar diferentes contraseñas basadas en nombres de archivo o metadatos
3. **Derivación de Claves**: Implementar funciones de derivación de claves personalizadas (PBKDF2, Argon2, etc.)
4. **Integración de Almacenamiento Seguro**: Recuperar claves desde módulos de seguridad de hardware (HSM) o almacenes de claves
---

#### get_Provider

Obtiene la interfaz del proveedor de contraseñas actualmente establecida.

**Sintaxis en C++**:
```cpp
HRESULT get_Provider(IPasswordProvider** ppProvider);
```

**Sintaxis en C#**:
```csharp
int get_Provider(IVFPasswordProvider passwordProvider);
```

**Parámetros**:
- `ppProvider` / `passwordProvider` - Puntero para recibir la interfaz del proveedor de contraseñas

**Valor de Retorno**:
- `S_OK` (0) en éxito
- `E_POINTER` si ppProvider es NULL
- HRESULT de error en caso de fallo

**Observaciones**:
Recupera la interfaz del proveedor de contraseñas que fue establecida previamente con `put_Provider`. Retorna NULL si no se ha establecido ningún proveedor.

---
#### put_Password
Establece la contraseña de encriptación o datos de clave binaria.
**Sintaxis en C++**:
```cpp
HRESULT put_Password(LPBYTE pBuffer, LONG lSize);
```
**Sintaxis en C#**:
```csharp
int put_Password(IntPtr buffer, int size);
```
**Sintaxis en Delphi**:
```delphi
function put_Password(buffer: PWideChar; size: integer): HRESULT; stdcall;
```
**Parámetros**:
- `pBuffer` / `buffer` - Puntero a la contraseña o datos de clave binaria
- `lSize` / `size` - Tamaño del buffer en bytes
**Valor de Retorno**:
- `S_OK` (0) en éxito
- `E_INVALIDARG` si el buffer es nulo o el tamaño es inválido
- HRESULT de error en caso de fallo
**Observaciones**:
- El SDK usa encriptación AES-256, que requiere una clave de 256 bits (32 bytes)
- Si proporciona una cadena de contraseña, debe aplicarse hash a 256 bits (use SHA-256)
- La misma contraseña/clave debe usarse tanto para encriptación como para desencriptación
- Para contraseñas de cadena, use los métodos auxiliares (solo C#) o aplique hash manualmente
- Los datos binarios se procesan con hash SHA-256 internamente para generar la clave de encriptación
**Ejemplo (C++)**:
```cpp
ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    // Usando contraseña de cadena (debe convertirse al formato apropiado)
    const wchar_t* password = L"MySecurePassword123";
    hr = pCrypto->put_Password(
        (LPBYTE)password,
        wcslen(password) * sizeof(wchar_t)
    );
    pCrypto->Release();
}
```
**Ejemplo (C#)**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Usando método auxiliar (recomendado)
    cryptoConfig.ApplyString("MySecurePassword123");
    // O manualmente con IntPtr
    string password = "MySecurePassword123";
    IntPtr ptr = Marshal.StringToCoTaskMemUni(password);
    try
    {
        cryptoConfig.put_Password(ptr, password.Length * 2);
    }
    finally
    {
        Marshal.FreeCoTaskMem(ptr);
    }
}
```
**Ejemplo (Delphi)**:
```delphi
var
  CryptoConfig: IVFCryptoConfig;
  Password: WideString;
begin
  if Supports(MuxerFilter, IVFCryptoConfig, CryptoConfig) then
  begin
    Password := 'MySecurePassword123';
    CryptoConfig.put_Password(PWideChar(Password), Length(Password) * 2);
  end;
end;
```
---

#### HavePassword

Verifica si se ha establecido una contraseña en el filtro.

**Sintaxis en C++**:
```cpp
HRESULT HavePassword();
```

**Sintaxis en C#**:
```csharp
int HavePassword();
```

**Sintaxis en Delphi**:
```delphi
function HavePassword(): HRESULT; stdcall;
```

**Parámetros**:
Ninguno

**Valor de Retorno**:
- `S_OK` (0) si la contraseña está establecida
- `S_FALSE` (1) si no hay contraseña establecida
- HRESULT de error en caso de fallo

**Observaciones**:
Use este método para verificar que se ha configurado una contraseña antes de iniciar el grafo de filtros.

**Ejemplo (C++)**:
```cpp
ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    HRESULT hrPassword = pCrypto->HavePassword();
    if (hrPassword == S_OK)
    {
        // La contraseña está establecida, puede iniciar la codificación
    }
    else
    {
        // No hay contraseña establecida
        MessageBox(NULL, L"Por favor establezca la contraseña de encriptación", L"Error", MB_OK);
    }

    pCrypto->Release();
}
```

**Ejemplo (C#)**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    int hr = cryptoConfig.HavePassword();
    if (hr == 0) // S_OK
    {
        // La contraseña está establecida
        Console.WriteLine("Contraseña configurada exitosamente");
    }
    else
    {
        // Sin contraseña
        Console.WriteLine("Advertencia: No hay contraseña establecida");
    }
}
```

---
## Interfaz IVFPasswordProvider
Interfaz de callback para escenarios avanzados de provisión de contraseñas incluyendo datos de clave binaria, generación dinámica de contraseñas y funciones de derivación de claves personalizadas.
### GUID de la Interfaz
```
{6F8162B5-778D-42b5-9242-1BBABB24FFC4}
```
### Herencia
Hereda de `IUnknown`
---

### Definiciones de la Interfaz

#### Definición en C++

```cpp
// {6F8162B5-778D-42b5-9242-1BBABB24FFC4}
DEFINE_GUID(IID_IPasswordProvider,
    0x6f8162b5, 0x778d, 0x42b5, 0x92, 0x42, 0x1b, 0xba, 0xbb, 0x24, 0xff, 0xc4);

DECLARE_INTERFACE_(IPasswordProvider, IUnknown)
{
    STDMETHOD(QueryPassword)(
        THIS_
        LPCWSTR pszFileName,
        LPBYTE pBuffer,
        LONG* plSize
    ) PURE;
};
```

#### Definición en C#

```csharp
[ComImport]
[Guid("6F8162B5-778D-42b5-9242-1BBABB24FFC4")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFPasswordProvider
{
    [PreserveSig]
    int QueryPassword(
        [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
        [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuffer,
        [In, Out] ref int plSize
    );
}
```

---
### Métodos
#### QueryPassword
Llamado por el filtro para consultar la contraseña o datos de clave binaria para un archivo específico.
**Sintaxis en C++**:
```cpp
HRESULT QueryPassword(
    LPCWSTR pszFileName,
    LPBYTE pBuffer,
    LONG* plSize
);
```
**Sintaxis en C#**:
```csharp
int QueryPassword(
    string pszFileName,
    byte[] pBuffer,
    ref int plSize
);
```
**Parámetros**:
- `pszFileName` - Nombre del archivo para el cual se solicita la contraseña (puede usarse para determinar claves específicas del archivo)
- `pBuffer` - Buffer para recibir datos de contraseña o clave binaria
- `plSize` - Puntero al tamaño del buffer (entrada: tamaño máximo del buffer, salida: tamaño real de datos retornado)
**Valor de Retorno**:
- `S_OK` (0) si la contraseña se proporcionó exitosamente
- `E_OUTOFMEMORY` si el buffer es muy pequeño (establecer plSize al tamaño requerido)
- HRESULT de error en caso de fallo
**Observaciones**:
Implemente esta interfaz para:
- Proporcionar datos de clave binaria (claves de 256 bits para AES-256)
- Generar claves de encriptación específicas por archivo basadas en el nombre del archivo
- Recuperar claves desde sistemas externos de gestión de claves
- Implementar lógica de derivación de contraseñas personalizada
Para escenarios simples con una sola contraseña para todos los archivos, usar `IVFCryptoConfig::put_Password` directamente es más sencillo.
**Ejemplo de Implementación (C#)**:
```csharp
public class CustomPasswordProvider : IVFPasswordProvider
{
    public int QueryPassword(string pszFileName, byte[] pBuffer, ref int plSize)
    {
        // Generar clave específica del archivo
        byte[] key = GenerateKeyForFile(pszFileName);
        if (pBuffer == null || plSize < key.Length)
        {
            plSize = key.Length;
            return unchecked((int)0x8007000E); // E_OUTOFMEMORY
        }
        Array.Copy(key, pBuffer, key.Length);
        plSize = key.Length;
        return 0; // S_OK
    }
    private byte[] GenerateKeyForFile(string fileName)
    {
        // Lógica de generación de clave personalizada
        using (var sha256 = SHA256.Create())
        {
            string seed = "MySalt" + fileName;
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));
        }
    }
}
```
---

## Métodos Auxiliares de C#

El SDK proporciona métodos de extensión convenientes para desarrolladores de C# en la clase `VFCryptoConfigHelper`.

### ApplyString

Aplica una contraseña de cadena con hash SHA-256 automático.

**Sintaxis**:
```csharp
public static int ApplyString(this IVFCryptoConfig cryptoConfig, string key)
```

**Parámetros**:
- `cryptoConfig` - La instancia de la interfaz IVFCryptoConfig
- `key` - Contraseña de cadena a aplicar

**Valor de Retorno**:
- `0` en éxito
- Lanza `Exception` si la clave es nula o vacía

**Observaciones**:
- Convierte automáticamente la cadena a Unicode y aplica hash SHA-256
- Método más común para establecer contraseñas
- Asegura formato de contraseña consistente entre encriptación/desencriptación

**Ejemplo**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    cryptoConfig.ApplyString("MySecurePassword123");
}
```

---
### ApplyFile
Usa el contenido de un archivo como la clave de encriptación (hash SHA-256 del archivo).
**Sintaxis**:
```csharp
public static int ApplyFile(this IVFCryptoConfig cryptoConfig, string key)
```
**Parámetros**:
- `cryptoConfig` - La instancia de la interfaz IVFCryptoConfig
- `key` - Ruta al archivo a usar como clave de encriptación
**Valor de Retorno**:
- `0` en éxito
- Lanza `FileNotFoundException` si el archivo no existe
- Lanza `Exception` si la clave es nula o vacía
**Observaciones**:
- Lee el contenido completo del archivo y calcula el hash SHA-256
- Útil para usar archivos de clave o certificados como claves de encriptación
- El contenido del archivo nunca se almacena, solo el hash
- El mismo archivo debe usarse tanto para encriptación como para desencriptación
**Ejemplo**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    cryptoConfig.ApplyFile(@"C:\keys\encryption.key");
}
```
**Nota de Seguridad**:
- Almacene archivos de clave de forma segura
- Use permisos de archivo apropiados
- Considere usar sistemas de almacenamiento de claves dedicados para producción
---

### ApplyBinary

Aplica datos de clave binaria con hash SHA-256 automático.

**Sintaxis**:
```csharp
public static int ApplyBinary(this IVFCryptoConfig cryptoConfig, byte[] key)
```

**Parámetros**:
- `cryptoConfig` - La instancia de la interfaz IVFCryptoConfig
- `key` - Datos de clave binaria (cualquier longitud)

**Valor de Retorno**:
- `0` en éxito
- Lanza `Exception` si la clave es nula o vacía

**Observaciones**:
- Acepta datos de clave binaria de cualquier longitud
- Calcula automáticamente el hash SHA-256 para generar una clave de 256 bits
- Útil para claves generadas programáticamente o derivación de claves

**Ejemplo**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Generar clave aleatoria
    byte[] keyData = new byte[32];
    using (var rng = new RNGCryptoServiceProvider())
    {
        rng.GetBytes(keyData);
    }

    // Aplicar clave binaria
    cryptoConfig.ApplyBinary(keyData);

    // Almacenar keyData de forma segura para desencriptación posterior
    SaveKeyToSecureStorage(keyData);
}
```

---
## CLSIDs de Filtros
### Filtro Muxer de Encriptación
Multiplexa flujos de video y audio en formato encriptado.
**CLSID**:
```cpp
// {F1D3727A-88DE-49ab-A635-280BEFEFF902}
DEFINE_GUID(CLSID_EncryptMuxer,
    0xf1d3727a, 0x88de, 0x49ab, 0xa6, 0x35, 0x28, 0xb, 0xef, 0xef, 0xf9, 0x2);
```
**Uso (C++)**:
```cpp
IBaseFilter* pMuxer = nullptr;
hr = CoCreateInstance(
    CLSID_EncryptMuxer,
    NULL,
    CLSCTX_INPROC_SERVER,
    IID_IBaseFilter,
    (void**)&pMuxer
);
```
**Uso (C#)**:
```csharp
var muxerFilter = (IBaseFilter)Activator.CreateInstance(
    Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
);
```
---

### Filtro Demuxer de Desencriptación

Demultiplexa y desencripta archivos encriptados.

**CLSID**:
```cpp
// {D2C761F0-9988-4f79-9B0E-FB2B79C65851}
DEFINE_GUID(CLSID_EncryptDemuxer,
    0xd2c761f0, 0x9988, 0x4f79, 0x9b, 0xe, 0xfb, 0x2b, 0x79, 0xc6, 0x58, 0x51);
```

**Uso (C++)**:
```cpp
IBaseFilter* pDemuxer = nullptr;
hr = CoCreateInstance(
    CLSID_EncryptDemuxer,
    NULL,
    CLSCTX_INPROC_SERVER,
    IID_IBaseFilter,
    (void**)&pDemuxer
);
```

**Uso (C#)**:
```csharp
var demuxerFilter = (IBaseFilter)Activator.CreateInstance(
    Type.GetTypeFromCLSID(new Guid("D2C761F0-9988-4f79-9B0E-FB2B79C65851"))
);
```

---
## Vea También
- [Descripción General del SDK de Encriptación de Video](index.es.md) - Características y capacidades del producto
- [Ejemplos](examples.es.md) - Ejemplos de código completos
- [Pack de Filtros de Codificación DirectShow](../filters-enc/index.es.md) - Codificadores compatibles
