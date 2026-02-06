---
title: Video Encryption SDK - Interface Reference
description: Video Encryption SDK API with IVFCryptoConfig, IVFPasswordProvider, and helper methods for C++, C#, and Delphi with AES-256.
---

# Video Encryption SDK - Interface Reference

## Overview

The Video Encryption SDK provides COM interfaces for encrypting and decrypting MP4 video files with AES-256 encryption. This reference covers all interfaces, methods, and helper classes for C++, C#, and Delphi developers.

---
## IVFCryptoConfig Interface
Primary interface for configuring encryption passwords and keys on both the encryption muxer and decryption demuxer filters.
### Interface GUID
```
{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}
```
### Inheritance
Inherits from `IUnknown`
---

### Interface Definitions

#### C++ Definition

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

#### C# Definition

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

#### Delphi Definition

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
### Methods
#### put_Provider
Sets a password provider callback interface for advanced encryption scenarios.
**C++ Syntax**:
```cpp
HRESULT put_Provider(IPasswordProvider* pProvider);
```
**C# Syntax**:
```csharp
int put_Provider(IVFPasswordProvider passwordProvider);
```
**Parameters**:
- `pProvider` / `passwordProvider` - Password provider interface that implements `IVFPasswordProvider`
**Return Value**:
- `S_OK` (0) on success
- Error HRESULT on failure
**Remarks**:
Use this method for advanced scenarios where you need:
- Dynamic password generation based on file name
- Binary key data provision through a callback
- Custom key derivation functions
- File-specific encryption keys
- Per-file password policies
For simple string passwords, using `put_Password` directly is more straightforward. The password provider is useful when you need runtime password determination or when implementing a custom key management system.
**Example Use Cases**:
1. **Binary Key Provider**: Provide 256-bit encryption keys from a key management system
2. **Dynamic Passwords**: Generate different passwords based on file names or metadata
3. **Key Derivation**: Implement custom key derivation functions (PBKDF2, Argon2, etc.)
4. **Secure Storage Integration**: Retrieve keys from hardware security modules (HSM) or key vaults
---

#### get_Provider

Gets the currently set password provider interface.

**C++ Syntax**:
```cpp
HRESULT get_Provider(IPasswordProvider** ppProvider);
```

**C# Syntax**:
```csharp
int get_Provider(IVFPasswordProvider passwordProvider);
```

**Parameters**:
- `ppProvider` / `passwordProvider` - Pointer to receive password provider interface

**Return Value**:
- `S_OK` (0) on success
- `E_POINTER` if ppProvider is NULL
- Error HRESULT on failure

**Remarks**:
Retrieves the password provider interface that was previously set with `put_Provider`. Returns NULL if no provider has been set.

---
#### put_Password
Sets the encryption password or binary key data.
**C++ Syntax**:
```cpp
HRESULT put_Password(LPBYTE pBuffer, LONG lSize);
```
**C# Syntax**:
```csharp
int put_Password(IntPtr buffer, int size);
```
**Delphi Syntax**:
```delphi
function put_Password(buffer: PWideChar; size: integer): HRESULT; stdcall;
```
**Parameters**:
- `pBuffer` / `buffer` - Pointer to password or binary key data
- `lSize` / `size` - Size of buffer in bytes
**Return Value**:
- `S_OK` (0) on success
- `E_INVALIDARG` if buffer is null or size is invalid
- Error HRESULT on failure
**Remarks**:
- The SDK uses AES-256 encryption, which requires a 256-bit (32-byte) key
- If you provide a password string, it should be hashed to 256 bits (use SHA-256)
- The same password/key must be used for both encryption and decryption
- For string passwords, use the helper methods (C# only) or hash manually
- Binary data is hashed with SHA-256 internally to generate the encryption key
**Example (C++)**:
```cpp
ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    // Using string password (must convert to proper format)
    const wchar_t* password = L"MySecurePassword123";
    hr = pCrypto->put_Password(
        (LPBYTE)password,
        wcslen(password) * sizeof(wchar_t)
    );
    pCrypto->Release();
}
```
**Example (C#)**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Using helper method (recommended)
    cryptoConfig.ApplyString("MySecurePassword123");
    // Or manually with IntPtr
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
**Example (Delphi)**:
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

Checks whether a password has been set on the filter.

**C++ Syntax**:
```cpp
HRESULT HavePassword();
```

**C# Syntax**:
```csharp
int HavePassword();
```

**Delphi Syntax**:
```delphi
function HavePassword(): HRESULT; stdcall;
```

**Parameters**:
None

**Return Value**:
- `S_OK` (0) if password is set
- `S_FALSE` (1) if no password is set
- Error HRESULT on failure

**Remarks**:
Use this method to verify that a password has been configured before starting the filter graph.

**Example (C++)**:
```cpp
ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    HRESULT hrPassword = pCrypto->HavePassword();
    if (hrPassword == S_OK)
    {
        // Password is set, can start encoding
    }
    else
    {
        // No password set
        MessageBox(NULL, L"Please set encryption password", L"Error", MB_OK);
    }

    pCrypto->Release();
}
```

**Example (C#)**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    int hr = cryptoConfig.HavePassword();
    if (hr == 0) // S_OK
    {
        // Password is set
        Console.WriteLine("Password configured successfully");
    }
    else
    {
        // No password
        Console.WriteLine("Warning: No password set");
    }
}
```

---
## IVFPasswordProvider Interface
Callback interface for advanced password provision scenarios including binary key data, dynamic password generation, and custom key derivation functions.
### Interface GUID
```
{6F8162B5-778D-42b5-9242-1BBABB24FFC4}
```
### Inheritance
Inherits from `IUnknown`
---

### Interface Definitions

#### C++ Definition

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

#### C# Definition

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
### Methods
#### QueryPassword
Called by the filter to query the password or binary key data for a specific file.
**C++ Syntax**:
```cpp
HRESULT QueryPassword(
    LPCWSTR pszFileName,
    LPBYTE pBuffer,
    LONG* plSize
);
```
**C# Syntax**:
```csharp
int QueryPassword(
    string pszFileName,
    byte[] pBuffer,
    ref int plSize
);
```
**Parameters**:
- `pszFileName` - File name for which password is requested (can be used to determine file-specific keys)
- `pBuffer` - Buffer to receive password data or binary key
- `plSize` - Pointer to buffer size (input: max buffer size, output: actual data size returned)
**Return Value**:
- `S_OK` (0) if password provided successfully
- `E_OUTOFMEMORY` if buffer is too small (set plSize to required size)
- Error HRESULT on failure
**Remarks**:
Implement this interface to:
- Provide binary key data (256-bit keys for AES-256)
- Generate file-specific encryption keys based on file name
- Retrieve keys from external key management systems
- Implement custom password derivation logic
For simple scenarios with a single password for all files, using `IVFCryptoConfig::put_Password` directly is more straightforward.
**Implementation Example (C#)**:
```csharp
public class CustomPasswordProvider : IVFPasswordProvider
{
    public int QueryPassword(string pszFileName, byte[] pBuffer, ref int plSize)
    {
        // Generate file-specific key
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
        // Custom key generation logic
        using (var sha256 = SHA256.Create())
        {
            string seed = "MySalt" + fileName;
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));
        }
    }
}
```
---

## C# Helper Methods

The SDK provides convenient extension methods for C# developers in the `VFCryptoConfigHelper` class.

### ApplyString

Applies a string password with automatic SHA-256 hashing.

**Syntax**:
```csharp
public static int ApplyString(this IVFCryptoConfig cryptoConfig, string key)
```

**Parameters**:
- `cryptoConfig` - The IVFCryptoConfig interface instance
- `key` - String password to apply

**Return Value**:
- `0` on success
- Throws `Exception` if key is null or empty

**Remarks**:
- Automatically converts string to Unicode and applies SHA-256 hashing
- Most common method for setting passwords
- Ensures consistent password format across encryption/decryption

**Example**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    cryptoConfig.ApplyString("MySecurePassword123");
}
```

---
### ApplyFile
Uses a file's content as the encryption key (SHA-256 hash of file).
**Syntax**:
```csharp
public static int ApplyFile(this IVFCryptoConfig cryptoConfig, string key)
```
**Parameters**:
- `cryptoConfig` - The IVFCryptoConfig interface instance
- `key` - Path to file to use as encryption key
**Return Value**:
- `0` on success
- Throws `FileNotFoundException` if file doesn't exist
- Throws `Exception` if key is null or empty
**Remarks**:
- Reads entire file content and computes SHA-256 hash
- Useful for using key files or certificates as encryption keys
- File content is never stored, only the hash
- Same file must be used for both encryption and decryption
**Example**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    cryptoConfig.ApplyFile(@"C:\keys\encryption.key");
}
```
**Security Note**:
- Store key files securely
- Use appropriate file permissions
- Consider using dedicated key storage systems for production
---

### ApplyBinary

Applies binary key data with automatic SHA-256 hashing.

**Syntax**:
```csharp
public static int ApplyBinary(this IVFCryptoConfig cryptoConfig, byte[] key)
```

**Parameters**:
- `cryptoConfig` - The IVFCryptoConfig interface instance
- `key` - Binary key data (any length)

**Return Value**:
- `0` on success
- Throws `Exception` if key is null or empty

**Remarks**:
- Accepts binary key data of any length
- Automatically computes SHA-256 hash to generate 256-bit key
- Useful for programmatically generated keys or key derivation

**Example**:
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Generate random key
    byte[] keyData = new byte[32];
    using (var rng = new RNGCryptoServiceProvider())
    {
        rng.GetBytes(keyData);
    }

    // Apply binary key
    cryptoConfig.ApplyBinary(keyData);

    // Store keyData securely for later decryption
    SaveKeyToSecureStorage(keyData);
}
```

---
## Filter CLSIDs
### Encrypt Muxer Filter
Muxes video and audio streams into encrypted format.
**CLSID**:
```cpp
// {F1D3727A-88DE-49ab-A635-280BEFEFF902}
DEFINE_GUID(CLSID_EncryptMuxer,
    0xf1d3727a, 0x88de, 0x49ab, 0xa6, 0x35, 0x28, 0xb, 0xef, 0xef, 0xf9, 0x2);
```
**Usage (C++)**:
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
**Usage (C#)**:
```csharp
var muxerFilter = (IBaseFilter)Activator.CreateInstance(
    Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
);
```
---

### Decrypt Demuxer Filter

Demuxes and decrypts encrypted files.

**CLSID**:
```cpp
// {D2C761F0-9988-4f79-9B0E-FB2B79C65851}
DEFINE_GUID(CLSID_EncryptDemuxer,
    0xd2c761f0, 0x9988, 0x4f79, 0x9b, 0xe, 0xfb, 0x2b, 0x79, 0xc6, 0x58, 0x51);
```

**Usage (C++)**:
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

**Usage (C#)**:
```csharp
var demuxerFilter = (IBaseFilter)Activator.CreateInstance(
    Type.GetTypeFromCLSID(new Guid("D2C761F0-9988-4f79-9B0E-FB2B79C65851"))
);
```

---
## See Also
- [Video Encryption SDK Overview](index.md) - Product features and capabilities
- [Examples](examples.md) - Complete code examples
- [DirectShow Encoding Filters Pack](../filters-enc/index.md) - Compatible encoders