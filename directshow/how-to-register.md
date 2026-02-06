---
title: DirectShow Filter SDK Registration Guide
description: Register DirectShow filters and SDKs in C++, C#, and Delphi with IVFRegister interface and alternative registration code examples.
---

# SDK Registration Guide

DirectShow filters and SDK components often require proper registration to function correctly within your applications. This guide provides detailed implementation methods for registering DirectShow filters across multiple programming languages.

## Registration Overview

Most DirectShow filters in the SDK can be registered using the IVFRegister interface. This standardized approach works consistently across development environments. However, some specialized filters (like RGB2YUV converters) are designed to work without explicit registration.

## Registration Methods by Language

### C++ Implementation

The following C++ code demonstrates how to access the registration interface:

```cpp
// {59E82754-B531-4A8E-A94D-57C75F01DA30}
DEFINE_GUID(IID_IVFRegister,
    0x59E82754, 0xB531, 0x4A8E, 0xA9, 0x4D, 0x57, 0xC7, 0x5F, 0x01, 0xDA, 0x30);

/// <summary>
/// Filter registration interface.
/// </summary>
DECLARE_INTERFACE_(IVFRegister, IUnknown)
{
    /// <summary>
    /// Sets registered.
    /// </summary>
    /// <param name="licenseKey">
    /// License Key.
    /// </param>
    STDMETHOD(SetLicenseKey)
        (THIS_
            WCHAR * licenseKey
            )PURE;
};
```

### C# Implementation

For .NET developers, the registration interface can be imported using the following C# code:

```cs
    /// <summary>
    /// Public filter registration interface.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFRegister
    {
        /// <summary>
        /// Sets registered.
        /// </summary>
        /// <param name="licenseKey">
        /// License Key.
        /// </param>
        [PreserveSig]
        void SetLicenseKey([In, MarshalAs(UnmanagedType.LPWStr)] string licenseKey);
    }
```

### Delphi Implementation

For Delphi developers, implement the registration interface as follows:

```pascal
const
  IID_IVFRegister: TGUID = '{59E82754-B531-4A8E-A94D-57C75F01DA30}';

type
  /// <summary>
  /// Public filter registration interface.
  /// </summary>
  IVFRegister = interface(IUnknown)
    /// <summary>
    /// Sets registered.
    /// </summary>
    /// <param name="licenseKey">
    /// License Key.
    /// </param>
    procedure SetLicenseKey(licenseKey: PWideChar); stdcall;
  end;
```
