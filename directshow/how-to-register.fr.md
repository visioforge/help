---
title: Enregistrer les filtres DirectShow en C++, C# et Delphi
description: Enregistrez les filtres et SDK DirectShow en C++, C# et Delphi avec l'interface IVFRegister et des exemples de code d'enregistrement alternatifs.
tags:
  - DirectShow
  - C++
  - Windows
  - C#
primary_api_classes:
  - IVFRegister

---

# Guide d'enregistrement du SDK

Les filtres DirectShow et les composants du SDK doivent souvent être correctement enregistrés pour fonctionner dans vos applications. Ce guide présente des méthodes détaillées d'implémentation pour enregistrer les filtres DirectShow dans plusieurs langages de programmation.

## Vue d'ensemble de l'enregistrement

La plupart des filtres DirectShow du SDK peuvent être enregistrés via l'interface IVFRegister. Cette approche standardisée fonctionne de façon cohérente dans tous les environnements de développement. Certains filtres spécialisés (comme les convertisseurs RGB2YUV) sont toutefois conçus pour fonctionner sans enregistrement explicite.

## Méthodes d'enregistrement par langage

### Implémentation en C++

Le code C++ suivant montre comment accéder à l'interface d'enregistrement :

```cpp
// {59E82754-B531-4A8E-A94D-57C75F01DA30}
DEFINE_GUID(IID_IVFRegister,
    0x59E82754, 0xB531, 0x4A8E, 0xA9, 0x4D, 0x57, 0xC7, 0x5F, 0x01, 0xDA, 0x30);

/// <summary>
/// Interface d'enregistrement des filtres.
/// </summary>
DECLARE_INTERFACE_(IVFRegister, IUnknown)
{
    /// <summary>
    /// Définit l'enregistrement.
    /// </summary>
    /// <param name="licenseKey">
    /// Clé de licence.
    /// </param>
    STDMETHOD(SetLicenseKey)
        (THIS_
            WCHAR * licenseKey
            )PURE;
};
```

### Implémentation en C#

Pour les développeurs .NET, l'interface d'enregistrement peut être importée à l'aide du code C# suivant :

```cs
    /// <summary>
    /// Interface publique d'enregistrement des filtres.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFRegister
    {
        /// <summary>
        /// Définit l'enregistrement.
        /// </summary>
        /// <param name="licenseKey">
        /// Clé de licence.
        /// </param>
        [PreserveSig]
        void SetLicenseKey([In, MarshalAs(UnmanagedType.LPWStr)] string licenseKey);
    }
```

### Implémentation en Delphi

Pour les développeurs Delphi, implémentez l'interface d'enregistrement comme suit :

```pascal
const
  IID_IVFRegister: TGUID = '{59E82754-B531-4A8E-A94D-57C75F01DA30}';

type
  /// <summary>
  /// Interface publique d'enregistrement des filtres.
  /// </summary>
  IVFRegister = interface(IUnknown)
    /// <summary>
    /// Définit l'enregistrement.
    /// </summary>
    /// <param name="licenseKey">
    /// Clé de licence.
    /// </param>
    procedure SetLicenseKey(licenseKey: PWideChar); stdcall;
  end;
```
