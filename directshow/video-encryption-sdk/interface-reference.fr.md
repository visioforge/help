---
title: Interface COM IVFCryptoConfig — API de chiffrement vidéo
description: Interfaces COM IVFCryptoConfig et IVFPasswordProvider pour le chiffrement vidéo AES-256 dans DirectShow. Méthodes, GUID et usage en C++, C#, Delphi.
tags:
  - Video Encryption SDK
  - DirectShow
  - C++
  - Windows
  - C#
primary_api_classes:
  - IVFCryptoConfig
  - IVFPasswordProvider
  - IBaseFilter
  - MuxerFilter

---

# Video Encryption SDK — référence des interfaces

## Vue d'ensemble

Le Video Encryption SDK fournit des interfaces COM pour chiffrer et déchiffrer des fichiers vidéo MP4 avec un chiffrement AES-256. Cette référence couvre toutes les interfaces, méthodes et classes utilitaires pour les développeurs C++, C# et Delphi.

---
## Interface IVFCryptoConfig
Interface principale pour configurer les mots de passe et clés de chiffrement sur les filtres multiplexeur de chiffrement et démultiplexeur de déchiffrement.
### GUID de l'interface
```
{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}
```
### Héritage
Hérite de `IUnknown`
---

### Définitions de l'interface

#### Définition C++

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

#### Définition C#

```csharp
using System;
using System.Runtime.InteropServices;

[ComImport]
[Guid("BAA5BD1E-3B30-425e-AB3B-CC20764AC253")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IVFCryptoConfig
{
    // NOTE : put_Provider / get_Provider sont des stubs dans le wrapper manage —
    // le parametre s'appelle litteralement `passwordProviderNotUsed` car le
    // marshaling .NET pour IVFPasswordProvider n'est pas branche. Depuis C#,
    // appelez put_Password directement avec les octets de cle binaires.
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

!!! warning "Fournisseur de mot de passe C# non pris en charge"
    Les wrappers managés `put_Provider` / `get_Provider` sont des **stubs non fonctionnels** (notez le nom du paramètre `passwordProviderNotUsed`). Pour définir des mots de passe depuis C#, appelez `put_Password` avec les octets de clé bruts via `IntPtr` (ou utilisez l'utilitaire `ApplyString`). Les rappels `IVFPasswordProvider` personnalisés doivent être implémentés depuis C++ ou Delphi.

#### Définition Delphi

```delphi
type
  IVFCryptoConfig = interface(IUnknown)
    ['{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}']
    function put_Provider(pProvider: IUnknown): HRESULT; stdcall;
    function get_Provider(out pProvider: IUnknown): HRESULT; stdcall;
    function put_Password(pBuffer: PByte; lSize: Integer): HRESULT; stdcall;
    function HavePassword(): HRESULT; stdcall;
  end;
```

!!! note "Nommage entre wrappers de langage"
    L'en-tête C++ natif (`encryptor_intf.h`) utilise `ICryptoConfig` / `IPasswordProvider`. Les wrappers C# et Delphi exposent la même interface sous les noms `IVFCryptoConfig` / `IVFPasswordProvider`. Les deux noms font référence au **même** GUID `{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}`.

---
### Méthodes
#### put_Provider
Définit une interface de rappel fournisseur de mot de passe pour les scénarios de chiffrement avancés.
**Syntaxe C++** :
```cpp
HRESULT put_Provider(IPasswordProvider* pProvider);
```
**Syntaxe C#** :
```csharp
int put_Provider(IVFPasswordProvider passwordProvider);
```
**Paramètres** :
- `pProvider` / `passwordProvider` — interface de fournisseur de mot de passe qui implémente `IVFPasswordProvider`
**Valeur de retour** :
- `S_OK` (0) en cas de succès
- HRESULT d'erreur en cas d'échec
**Remarques** :
Utilisez cette méthode pour les scénarios avancés où vous avez besoin de :
- Génération dynamique de mots de passe basée sur le nom de fichier
- Fourniture de données de clé binaires via un rappel
- Fonctions de dérivation de clé personnalisées
- Clés de chiffrement spécifiques à un fichier
- Politiques de mot de passe par fichier
Pour les mots de passe textuels simples, utiliser `put_Password` directement est plus simple. Le fournisseur de mot de passe est utile lorsque vous avez besoin de déterminer le mot de passe à l'exécution ou lors de l'implémentation d'un système de gestion de clés personnalisé.
**Exemples de cas d'usage** :
1. **Fournisseur de clé binaire** : fournir des clés de chiffrement 256 bits depuis un système de gestion de clés
2. **Mots de passe dynamiques** : générer des mots de passe différents selon les noms de fichiers ou métadonnées
3. **Dérivation de clé** : implémenter des fonctions de dérivation de clé personnalisées (PBKDF2, Argon2, etc.)
4. **Intégration à un stockage sécurisé** : récupérer les clés depuis des modules de sécurité matériels (HSM) ou coffres de clés
---

#### get_Provider

Obtient l'interface de fournisseur de mot de passe actuellement définie.

**Syntaxe C++** :
```cpp
HRESULT get_Provider(IPasswordProvider** ppProvider);
```

**Syntaxe C#** :
```csharp
int get_Provider(IVFPasswordProvider passwordProvider);
```

**Paramètres** :
- `ppProvider` / `passwordProvider` — pointeur pour recevoir l'interface de fournisseur de mot de passe

**Valeur de retour** :
- `S_OK` (0) en cas de succès
- `E_POINTER` si ppProvider est NULL
- HRESULT d'erreur en cas d'échec

**Remarques** :
Récupère l'interface de fournisseur de mot de passe précédemment définie avec `put_Provider`. Retourne NULL si aucun fournisseur n'a été défini.

---
#### put_Password
Définit le mot de passe de chiffrement ou les données de clé binaires.
**Syntaxe C++** :
```cpp
HRESULT put_Password(LPBYTE pBuffer, LONG lSize);
```
**Syntaxe C#** :
```csharp
int put_Password(IntPtr buffer, int size);
```
**Syntaxe Delphi** :
```delphi
function put_Password(pBuffer: PByte; lSize: Integer): HRESULT; stdcall;
```
**Paramètres** :
- `pBuffer` / `buffer` — pointeur vers le mot de passe ou les données de clé binaires
- `lSize` / `size` — taille du tampon en octets
**Valeur de retour** :
- `S_OK` (0) en cas de succès
- `E_INVALIDARG` si le tampon est null ou si la taille est invalide
- HRESULT d'erreur en cas d'échec
**Remarques** :
- Le SDK utilise le chiffrement AES-256, qui nécessite une clé de 256 bits (32 octets)
- Si vous fournissez une chaîne de mot de passe, elle doit être hachée à 256 bits (utilisez SHA-256)
- Le même mot de passe/clé doit être utilisé pour le chiffrement et le déchiffrement
- Pour les mots de passe textuels, utilisez les méthodes utilitaires (C# uniquement) ou hachez manuellement
- Les données binaires sont hachées en interne avec SHA-256 pour générer la clé de chiffrement
**Exemple (C++)** :
```cpp
ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    // Utilisation d'un mot de passe textuel (doit etre converti au bon format)
    const wchar_t* password = L"MySecurePassword123";
    hr = pCrypto->put_Password(
        (LPBYTE)password,
        wcslen(password) * sizeof(wchar_t)
    );
    pCrypto->Release();
}
```
**Exemple (C#)** :
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Utiliser la methode utilitaire (recommande)
    cryptoConfig.ApplyString("MySecurePassword123");
    // Ou manuellement avec IntPtr
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
**Exemple (Delphi)** :
```delphi
var
  CryptoConfig: IVFCryptoConfig;
  Password: WideString;
begin
  if Supports(MuxerFilter, IVFCryptoConfig, CryptoConfig) then
  begin
    Password := 'MySecurePassword123';
    // pBuffer est constitue de donnees binaires opaques (LPBYTE) ; convertissez
    // le pointeur de wide-string en PByte et passez la longueur en octets
    // (caracteres UTF-16 * 2).
    CryptoConfig.put_Password(PByte(PWideChar(Password)), Length(Password) * 2);
  end;
end;
```
---

#### HavePassword

Vérifie si un mot de passe a été défini sur le filtre.

**Syntaxe C++** :
```cpp
HRESULT HavePassword();
```

**Syntaxe C#** :
```csharp
int HavePassword();
```

**Syntaxe Delphi** :
```delphi
function HavePassword(): HRESULT; stdcall;
```

**Paramètres** :
Aucun

**Valeur de retour** :
- `S_OK` (0) si un mot de passe est défini
- `S_FALSE` (1) si aucun mot de passe n'est défini
- HRESULT d'erreur en cas d'échec

**Remarques** :
Utilisez cette méthode pour vérifier qu'un mot de passe a été configuré avant de démarrer le graphe de filtres.

**Exemple (C++)** :
```cpp
ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    HRESULT hrPassword = pCrypto->HavePassword();
    if (hrPassword == S_OK)
    {
        // Mot de passe defini, l'encodage peut demarrer
    }
    else
    {
        // Aucun mot de passe
        MessageBox(NULL, L"Please set encryption password", L"Error", MB_OK);
    }

    pCrypto->Release();
}
```

**Exemple (C#)** :
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    int hr = cryptoConfig.HavePassword();
    if (hr == 0) // S_OK
    {
        // Mot de passe defini
        Console.WriteLine("Password configured successfully");
    }
    else
    {
        // Aucun mot de passe
        Console.WriteLine("Warning: No password set");
    }
}
```

---
## Interface IVFPasswordProvider
Interface de rappel pour les scénarios avancés de fourniture de mot de passe, dont les données de clé binaires, la génération dynamique de mots de passe et les fonctions de dérivation de clé personnalisées.
### GUID de l'interface
```
{6F8162B5-778D-42b5-9242-1BBABB24FFC4}
```
### Héritage
Hérite de `IUnknown`
---

### Définitions de l'interface

#### Définition C++

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

#### Définition C#

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
### Méthodes
#### QueryPassword
Appelée par le filtre pour interroger le mot de passe ou les données de clé binaires d'un fichier spécifique.
**Syntaxe C++** :
```cpp
HRESULT QueryPassword(
    LPCWSTR pszFileName,
    LPBYTE pBuffer,
    LONG* plSize
);
```
**Syntaxe C#** :
```csharp
int QueryPassword(
    string pszFileName,
    byte[] pBuffer,
    ref int plSize
);
```
**Paramètres** :
- `pszFileName` — nom du fichier pour lequel le mot de passe est demandé (peut servir à déterminer des clés spécifiques au fichier)
- `pBuffer` — tampon pour recevoir les données de mot de passe ou la clé binaire
- `plSize` — pointeur vers la taille du tampon (entrée : taille max ; sortie : taille réelle des données renvoyées)
**Valeur de retour** :
- `S_OK` (0) si le mot de passe a été fourni avec succès
- `E_OUTOFMEMORY` si le tampon est trop petit (définissez plSize à la taille requise)
- HRESULT d'erreur en cas d'échec
**Remarques** :
Implémentez cette interface pour :
- Fournir des données de clé binaires (clés 256 bits pour AES-256)
- Générer des clés de chiffrement spécifiques à un fichier selon le nom
- Récupérer des clés depuis des systèmes de gestion de clés externes
- Implémenter une logique de dérivation de mot de passe personnalisée
Pour les scénarios simples avec un mot de passe unique pour tous les fichiers, utiliser `IVFCryptoConfig::put_Password` directement est plus simple.
**Exemple d'implémentation (C#)** :
```csharp
public class CustomPasswordProvider : IVFPasswordProvider
{
    public int QueryPassword(string pszFileName, byte[] pBuffer, ref int plSize)
    {
        // Generer une cle specifique au fichier
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
        // Logique personnalisee de generation de cle
        using (var sha256 = SHA256.Create())
        {
            string seed = "MySalt" + fileName;
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(seed));
        }
    }
}
```
---

## Méthodes utilitaires C#

Le SDK fournit des méthodes d'extension pratiques pour les développeurs C# dans la classe `VFCryptoConfigHelper`.

### ApplyString

Applique un mot de passe textuel avec hachage SHA-256 automatique.

**Syntaxe** :
```csharp
public static int ApplyString(this IVFCryptoConfig cryptoConfig, string key)
```

**Paramètres** :
- `cryptoConfig` — l'instance de l'interface IVFCryptoConfig
- `key` — mot de passe textuel à appliquer

**Valeur de retour** :
- `0` en cas de succès
- Lève une `Exception` si la clé est null ou vide

**Remarques** :
- Convertit automatiquement la chaîne en Unicode et applique le hachage SHA-256
- Méthode la plus courante pour définir des mots de passe
- Garantit un format de mot de passe cohérent entre chiffrement et déchiffrement

**Exemple** :
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    cryptoConfig.ApplyString("MySecurePassword123");
}
```

---
### ApplyFile
Utilise le contenu d'un fichier comme clé de chiffrement (hachage SHA-256 du fichier).
**Syntaxe** :
```csharp
public static int ApplyFile(this IVFCryptoConfig cryptoConfig, string key)
```
**Paramètres** :
- `cryptoConfig` — l'instance de l'interface IVFCryptoConfig
- `key` — chemin du fichier à utiliser comme clé de chiffrement
**Valeur de retour** :
- `0` en cas de succès
- Lève `FileNotFoundException` si le fichier n'existe pas
- Lève `Exception` si la clé est null ou vide
**Remarques** :
- Lit l'intégralité du contenu du fichier et calcule le hachage SHA-256
- Utile pour utiliser des fichiers de clés ou des certificats comme clés de chiffrement
- Le contenu du fichier n'est jamais stocké, seul le hachage l'est
- Le même fichier doit être utilisé pour le chiffrement et le déchiffrement
**Exemple** :
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    cryptoConfig.ApplyFile(@"C:\keys\encryption.key");
}
```
**Note de sécurité** :
- Stockez les fichiers de clés de façon sécurisée
- Utilisez des autorisations de fichier appropriées
- Envisagez d'utiliser des systèmes de stockage de clés dédiés en production
---

### ApplyBinary

Applique des données de clé binaires avec hachage SHA-256 automatique.

**Syntaxe** :
```csharp
public static int ApplyBinary(this IVFCryptoConfig cryptoConfig, byte[] key)
```

**Paramètres** :
- `cryptoConfig` — l'instance de l'interface IVFCryptoConfig
- `key` — données de clé binaires (de n'importe quelle longueur)

**Valeur de retour** :
- `0` en cas de succès
- Lève une `Exception` si la clé est null ou vide

**Remarques** :
- Accepte des données de clé binaires de n'importe quelle longueur
- Calcule automatiquement le hachage SHA-256 pour générer une clé 256 bits
- Utile pour les clés générées programmatiquement ou la dérivation de clé

**Exemple** :
```csharp
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Generer une cle aleatoire
    byte[] keyData = new byte[32];
    using (var rng = new RNGCryptoServiceProvider())
    {
        rng.GetBytes(keyData);
    }

    // Appliquer la cle binaire
    cryptoConfig.ApplyBinary(keyData);

    // Stocker keyData de facon securisee pour un dechiffrement ulterieur
    SaveKeyToSecureStorage(keyData);
}
```

---
## CLSID des filtres
### Filtre multiplexeur de chiffrement
Multiplexe les flux vidéo et audio dans un format chiffré.
**CLSID** :
```cpp
// {F1D3727A-88DE-49ab-A635-280BEFEFF902}
DEFINE_GUID(CLSID_EncryptMuxer,
    0xf1d3727a, 0x88de, 0x49ab, 0xa6, 0x35, 0x28, 0xb, 0xef, 0xef, 0xf9, 0x2);
```
**Utilisation (C++)** :
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
**Utilisation (C#)** :
```csharp
var muxerFilter = (IBaseFilter)Activator.CreateInstance(
    Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
);
```
---

### Filtre démultiplexeur de déchiffrement

Démultiplexe et déchiffre les fichiers chiffrés.

**CLSID** :
```cpp
// {D2C761F0-9988-4f79-9B0E-FB2B79C65851}
DEFINE_GUID(CLSID_EncryptDemuxer,
    0xd2c761f0, 0x9988, 0x4f79, 0x9b, 0xe, 0xfb, 0x2b, 0x79, 0xc6, 0x58, 0x51);
```

**Utilisation (C++)** :
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

**Utilisation (C#)** :
```csharp
var demuxerFilter = (IBaseFilter)Activator.CreateInstance(
    Type.GetTypeFromCLSID(new Guid("D2C761F0-9988-4f79-9B0E-FB2B79C65851"))
);
```

---
## Voir aussi
- [Présentation du Video Encryption SDK](index.md) — fonctionnalités et capacités du produit
- [Exemples](examples.md) — exemples de code complets
- [Pack de filtres d'encodage DirectShow](../filters-enc/index.md) — encodeurs compatibles
