---
title: Guía de Registro del SDK de Filtros DirectShow
description: Registra filtros DirectShow y SDKs en C++, C# y Delphi con interfaz IVFRegister y ejemplos de código de registro alternativos.
---

# Guía de Registro del SDK

Los filtros DirectShow y componentes del SDK frecuentemente requieren registro apropiado para funcionar correctamente dentro de tus aplicaciones. Esta guía proporciona métodos de implementación detallados para registrar filtros DirectShow a través de múltiples lenguajes de programación.

## Descripción General del Registro

La mayoría de los filtros DirectShow en el SDK pueden registrarse usando la interfaz IVFRegister. Este enfoque estandarizado funciona consistentemente a través de entornos de desarrollo. Sin embargo, algunos filtros especializados (como los convertidores RGB2YUV) están diseñados para funcionar sin registro explícito.

## Métodos de Registro por Lenguaje

### Implementación en C++

El siguiente código C++ demuestra cómo acceder a la interfaz de registro:

```cpp
// {59E82754-B531-4A8E-A94D-57C75F01DA30}
DEFINE_GUID(IID_IVFRegister,
    0x59E82754, 0xB531, 0x4A8E, 0xA9, 0x4D, 0x57, 0xC7, 0x5F, 0x01, 0xDA, 0x30);

/// <summary>
/// Interfaz de registro de filtros.
/// </summary>
DECLARE_INTERFACE_(IVFRegister, IUnknown)
{
    /// <summary>
    /// Establece el registro.
    /// </summary>
    /// <param name="licenseKey">
    /// Clave de Licencia.
    /// </param>
    STDMETHOD(SetLicenseKey)
        (THIS_
            WCHAR * licenseKey
            )PURE;
};
```

### Implementación en C#

Para desarrolladores .NET, la interfaz de registro puede importarse usando el siguiente código C#:

```cs
    /// <summary>
    /// Interfaz pública de registro de filtros.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFRegister
    {
        /// <summary>
        /// Establece el registro.
        /// </summary>
        /// <param name="licenseKey">
        /// Clave de Licencia.
        /// </param>
        [PreserveSig]
        void SetLicenseKey([In, MarshalAs(UnmanagedType.LPWStr)] string licenseKey);
    }
```

### Implementación en Delphi

Para desarrolladores Delphi, implementa la interfaz de registro de la siguiente manera:

```pascal
const
  IID_IVFRegister: TGUID = '{59E82754-B531-4A8E-A94D-57C75F01DA30}';

type
  /// <summary>
  /// Interfaz pública de registro de filtros.
  /// </summary>
  IVFRegister = interface(IUnknown)
    /// <summary>
    /// Establece el registro.
    /// </summary>
    /// <param name="licenseKey">
    /// Clave de Licencia.
    /// </param>
    procedure SetLicenseKey(licenseKey: PWideChar); stdcall;
  end;
```
