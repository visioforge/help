---
title: Paramètres encodeur matériel NVIDIA NVENC pour DirectShow
description: Configurez l'encodage matériel NVIDIA NVENC dans DirectShow via l'interface COM INVEncConfig. Codecs H.264/H.265, préréglages et sélection GPU C++/C#.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - H.264
  - H.265
  - C#
primary_api_classes:
  - IBaseFilter

---

# Référence de l'interface INVEncConfig

## Vue d'ensemble

L'interface `INVEncConfig` fournit un contrôle complet de l'encodage vidéo matériel NVIDIA NVENC. Cette interface étend l'interface DirectShow standard `IAMVideoCompression` avec des options de configuration spécifiques à NVENC pour l'encodage H.264 et H.265.

NVENC est l'encodeur matériel dédié de NVIDIA disponible sur les GPU GeForce, Quadro et Tesla, offrant un encodage vidéo hautes performances avec une utilisation CPU minimale.

## GUID du filtre et de l'interface

- **CLSID du filtre** : `CLSID_NVEncoder`
  `{6EEC9161-7276-430B-A197-0D4C3BCC87E5}`

- **Interface** : `INVEncConfig`
  **GUID** : `{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}`
  **Hérite de** : `IAMVideoCompression`
  **Fichier d'en-tête** : `Intf.h` (C++)

- **Interface** : `INVEncConfig2`
  **GUID** : `{2A741FB6-6DE1-460B-8FCA-76DB478C9357}`
  **Hérite de** : `IUnknown`
  **Fichier d'en-tête** : `Intf2.h` (C++)

## Définitions de l'interface

### Définition C++ (INVEncConfig)

```cpp
#include <strmif.h>

// {9A2AC42C-3E3D-4E6A-84E5-D097292D496B}
static const GUID IID_INVEncConfig =
{ 0x9a2ac42c, 0x3e3d, 0x4e6a, { 0x84, 0xe5, 0xd0, 0x97, 0x29, 0x2d, 0x49, 0x6b } };

// {6EEC9161-7276-430B-A197-0D4C3BCC87E5}
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

### Définition C# (INVEncConfig)

```csharp
using System;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interface de configuration de l'encodeur NVENC.
    /// Fournit un encodage H.264/H.265 accelere materiellement sur GPU NVIDIA.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("9A2AC42C-3E3D-4E6A-84E5-D097292D496B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INVEncConfig
    {
        // Remarque : herite aussi des methodes de IAMVideoCompression
        // (put_KeyFrameRate, get_KeyFrameRate, put_PFramesPerKeyFrame, etc.)

        /// <summary>Definit l'index du peripherique CUDA pour l'encodage.</summary>
        /// <param name="v">Index du peripherique (0 pour le premier GPU, 1 pour le second, etc.)</param>
        [PreserveSig]
        int SetDeviceType(int v);

        /// <summary>Obtient l'index du peripherique CUDA.</summary>
        [PreserveSig]
        int GetDeviceType(out int v);

        /// <summary>Definit la structure d'image (progressif ou entrelace).</summary>
        /// <param name="v">0 = progressif, 1 = entrelace</param>
        [PreserveSig]
        int SetPictureStructure(int v);

        /// <summary>Obtient la structure d'image.</summary>
        [PreserveSig]
        int GetPictureStructure(out int v);

        /// <summary>Definit le nombre de tampons d'encodage.</summary>
        /// <param name="v">Nombre de tampons (generalement 4-8)</param>
        [PreserveSig]
        int SetNumBuffers(int v);

        /// <summary>Obtient le nombre de tampons d'encodage.</summary>
        [PreserveSig]
        int GetNumBuffers(out int v);

        /// <summary>Definit le mode de controle de debit.</summary>
        /// <param name="v">0 = CQP, 1 = VBR, 2 = CBR</param>
        [PreserveSig]
        int SetRateControl(int v);

        /// <summary>Obtient le mode de controle de debit.</summary>
        [PreserveSig]
        int GetRateControl(out int v);

        /// <summary>Definit le preset d'encodage.</summary>
        /// <param name="v">GUID du preset (P1-P7)</param>
        [PreserveSig]
        int SetPreset(Guid v);

        /// <summary>Obtient le preset d'encodage.</summary>
        [PreserveSig]
        int GetPreset(out Guid v);

        /// <summary>Definit le parametre de quantification pour le mode CQP.</summary>
        /// <param name="v">Valeur QP (0-51, plus bas = meilleure qualite)</param>
        [PreserveSig]
        int SetQp(int v);

        /// <summary>Obtient le parametre de quantification.</summary>
        [PreserveSig]
        int GetQp(out int v);

        /// <summary>Definit le nombre d'images B.</summary>
        /// <param name="v">Nombre d'images B (0-4)</param>
        [PreserveSig]
        int SetBFrames(int v);

        /// <summary>Obtient le nombre d'images B.</summary>
        [PreserveSig]
        int GetBFrames(out int v);

        /// <summary>Definit la taille du GOP (Group of Pictures).</summary>
        /// <param name="v">Taille de GOP en images</param>
        [PreserveSig]
        int SetGOP(int v);

        /// <summary>Obtient la taille du GOP.</summary>
        [PreserveSig]
        int GetGOP(out int v);

        /// <summary>Definit le debit binaire cible.</summary>
        /// <param name="v">Debit en bits par seconde</param>
        [PreserveSig]
        int SetBitrate(int v);

        /// <summary>Obtient le debit binaire cible.</summary>
        [PreserveSig]
        int GetBitrate(out int v);

        /// <summary>Definit le debit du tampon VBV.</summary>
        /// <param name="v">Debit VBV en bps</param>
        [PreserveSig]
        int SetVbvBitrate(int v);

        /// <summary>Obtient le debit du tampon VBV.</summary>
        [PreserveSig]
        int GetVbvBitrate(out int v);

        /// <summary>Definit la taille du tampon VBV.</summary>
        /// <param name="v">Taille du VBV en bits</param>
        [PreserveSig]
        int SetVbvSize(int v);

        /// <summary>Obtient la taille du tampon VBV.</summary>
        [PreserveSig]
        int GetVbvSize(out int v);

        /// <summary>Definit le profil d'encodage.</summary>
        /// <param name="v">GUID du profil (Baseline, Main, High, etc.)</param>
        [PreserveSig]
        int SetProfile(Guid v);

        /// <summary>Obtient le profil d'encodage.</summary>
        [PreserveSig]
        int GetProfile(out Guid v);

        /// <summary>Definit le niveau du profil.</summary>
        /// <param name="v">Valeur du niveau (30, 31, 40, 41, 50, 51, etc.)</param>
        [PreserveSig]
        int SetLevel(int v);

        /// <summary>Obtient le niveau du profil.</summary>
        [PreserveSig]
        int GetLevel(out int v);

        /// <summary>Definit le codec video.</summary>
        /// <param name="v">0 = H.264, 1 = H.265</param>
        [PreserveSig]
        int SetCodec(int v);

        /// <summary>Obtient le codec video.</summary>
        [PreserveSig]
        int GetCodec(out int v);
    }

    /// <summary>
    /// Interface de configuration NVENC 2 - verification de disponibilite.
    /// </summary>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("2A741FB6-6DE1-460B-8FCA-76DB478C9357")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INVEncConfig2
    {
        /// <summary>Verifie si NVENC est disponible sur le systeme.</summary>
        /// <param name="result">True si NVENC est disponible</param>
        /// <param name="status">Code d'etat NVENC</param>
        [PreserveSig]
        int CheckNVENCAvailable([MarshalAs(UnmanagedType.Bool)] out bool result, out int status);
    }
}
```

### Définition Delphi (INVEncConfig)

```delphi
uses
  ActiveX, ComObj;

const
  IID_INVEncConfig: TGUID = '{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}';
  IID_INVEncConfig2: TGUID = '{2A741FB6-6DE1-460B-8FCA-76DB478C9357}';
  CLSID_NVEncoder: TGUID = '{6EEC9161-7276-430B-A197-0D4C3BCC87E5}';

type
  /// <summary>
  /// Interface de configuration de l'encodeur NVENC.
  /// Etend IAMVideoCompression avec des parametres specifiques a NVENC.
  /// </summary>
  INVEncConfig = interface(IUnknown)
    ['{9A2AC42C-3E3D-4E6A-84E5-D097292D496B}']

    // Remarque : herite aussi des methodes de IAMVideoCompression

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
  /// Interface de configuration NVENC 2 - verification de disponibilite.
  /// </summary>
  INVEncConfig2 = interface(IUnknown)
    ['{2A741FB6-6DE1-460B-8FCA-76DB478C9357}']

    function CheckNVENCAvailable(out result: BOOL; out status: Integer): HRESULT; stdcall;
  end;
```

## Configuration matérielle requise

### Générations de GPU

| Génération GPU | H.264 | H.265 | Qualité | Notes |
|----------------|-------|-------|---------|-------|
| **Kepler** (GTX 600/700) | ✓ | ✗ | Basique | NVENC 1re génération |
| **Maxwell** (GTX 900) | ✓ | ✓ | Bonne | 2e gén., prise en charge HEVC |
| **Pascal** (GTX 10XX) | ✓ | ✓ | Meilleure | 3e gén., qualité améliorée |
| **Turing** (RTX 20XX) | ✓ | ✓ | Excellente | 7e gén., prise en charge images B |
| **Ampere** (RTX 30XX) | ✓ | ✓ | Excellente | 8e gén., prise en charge AV1 |
| **Ada/Hopper** (RTX 40XX) | ✓ | ✓ | Optimale | Dernière génération |

### Capacités de performance

- **1080p @ 60 fps** : toutes les générations NVENC
- **4K @ 60 fps** : Maxwell et plus récent
- **8K @ 30 fps** : Turing et plus récent
- **Flux simultanés** : 3-5 (variable selon le GPU)

---
## Référence des méthodes
Toutes les méthodes héritées de `IAMVideoCompression` sont disponibles. Les méthodes suivantes sont des extensions spécifiques à NVENC :
### Configuration du périphérique
#### SetDeviceType / GetDeviceType
Définit ou récupère l'index du périphérique CUDA pour l'encodage.
**Syntaxe (C++)** :
```cpp
HRESULT SetDeviceType(int v);
HRESULT GetDeviceType(int *v);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetDeviceType(int v);
[PreserveSig]
int GetDeviceType(out int v);
```
**Paramètres** :
- `v` : index du périphérique CUDA (0 pour le premier GPU, 1 pour le second, etc.)
**Retourne** : `S_OK` (0) en cas de succès.
**Notes d'utilisation** :
- Doit être appelé **avant** la connexion du filtre encodeur
- Utilisez 0 pour les systèmes à GPU unique
- Pour les systèmes multi-GPU, sélectionnez le GPU à utiliser pour l'encodage
- Interrogez les périphériques CUDA disponibles via l'API CUDA ou les outils NVIDIA
**Exemple (C++)** :
```cpp
INVEncConfig* pNVEnc = nullptr;
pFilter->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
// Utiliser le premier GPU
pNVEnc->SetDeviceType(0);
pNVEnc->Release();
```
---

### Structure d'image

#### SetPictureStructure / GetPictureStructure

Définit le type de codage d'image (progressif ou entrelacé).

**Syntaxe (C++)** :
```cpp
HRESULT SetPictureStructure(int v);
HRESULT GetPictureStructure(int *v);
```

**Paramètres** :
- `v` : type de structure d'image
  - `0` — progressif (basé sur l'image)
  - `1` — entrelacé (basé sur le champ)

**Retourne** : `S_OK` en cas de succès.

**Notes d'utilisation** :
- Par défaut : progressif (0)
- Utilisez entrelacé (1) uniquement pour le contenu de diffusion/DVD
- Le progressif est recommandé pour le contenu moderne

**Exemple (C++)** :
```cpp
// Definir l'encodage progressif
pNVEnc->SetPictureStructure(0);
```

---
### Configuration des tampons
#### SetNumBuffers / GetNumBuffers
Définit le nombre de tampons d'encodage.
**Syntaxe (C++)** :
```cpp
HRESULT SetNumBuffers(int v);
HRESULT GetNumBuffers(int *v);
```
**Paramètres** :
- `v` : nombre de tampons (généralement 4-8)
**Retourne** : `S_OK` en cas de succès.
**Notes d'utilisation** :
- Plus de tampons = latence plus élevée mais encodage plus fluide
- Moins de tampons = latence plus faible mais risque de perte d'images
- Valeurs recommandées :
  - Faible latence : 4 tampons
  - Normal : 6 tampons
  - Haute qualité : 8 tampons
**Exemple (C++)** :
```cpp
// Configuration faible latence
pNVEnc->SetNumBuffers(4);
```
---

### Contrôle de débit

#### SetRateControl / GetRateControl

Définit le mode de contrôle de débit pour la gestion du débit binaire.

**Syntaxe (C++)** :
```cpp
HRESULT SetRateControl(int v);
HRESULT GetRateControl(int *v);
```

**Paramètres** :
- `v` : mode de contrôle de débit
  - `0` — **CQP** (Constant Quantization Parameter) — qualité fixe
  - `1` — **VBR** (débit binaire variable) — débit variable, qualité cible
  - `2` — **CBR** (débit binaire constant) — débit fixe pour le streaming

**Retourne** : `S_OK` en cas de succès.

**Détails des modes de contrôle de débit** :

| Mode | Comportement de débit | Cas d'usage | Qualité | Taille de fichier |
|------|------------------|----------|---------|-----------|
| **CQP** | Varie fortement | Archivage, qualité maximale | Excellente | Imprévisible |
| **VBR** | Varie modérément | Stockage, YouTube | Très bonne | Modérée |
| **CBR** | Constant | Streaming en direct, diffusion | Bonne | Prévisible |

**Exemple (C++)** :
```cpp
// Utiliser CBR pour le streaming en direct
pNVEnc->SetRateControl(2);
pNVEnc->SetBitrate(5000000); // 5 Mbps
```

**Exemple (C#)** :
```csharp
// Utiliser VBR pour l'enregistrement fichier
nvenc.SetRateControl(1);
nvenc.SetBitrate(8000000); // Cible 8 Mbps
```

---
### Configuration du préréglage
#### SetPreset / GetPreset
Définit le préréglage d'encodage qui équilibre vitesse et qualité.
**Syntaxe (C++)** :
```cpp
HRESULT SetPreset(GUID v);
HRESULT GetPreset(GUID *v);
```
**Paramètres** :
- `v` : GUID de préréglage du SDK NVENC
**Options de préréglage** (valeurs typiques) :
| Préréglage | Description | Vitesse | Qualité | Cas d'usage |
|--------|-------------|-------|---------|----------|
| **P1** | Le plus rapide | ★★★★★ | ★☆☆☆☆ | Temps réel faible latence |
| **P2** | Plus rapide | ★★★★☆ | ★★☆☆☆ | Streaming en direct |
| **P3** | Rapide | ★★★☆☆ | ★★★☆☆ | Streaming standard |
| **P4** | Moyen | ★★☆☆☆ | ★★★★☆ | Équilibré (recommandé) |
| **P5** | Lent | ★☆☆☆☆ | ★★★★☆ | Streaming haute qualité |
| **P6** | Plus lent | ☆☆☆☆☆ | ★★★★★ | Qualité archive |
| **P7** | Le plus lent | ☆☆☆☆☆ | ★★★★★ | Qualité maximale |
**Notes d'utilisation** :
- P4 est recommandé pour la plupart des cas d'usage
- P1-P2 pour les applications à faible latence
- P6-P7 pour la qualité maximale (encodage plus lent)
- Le préréglage affecte : estimation de mouvement, lookahead, mouvement sous-pixel
**Exemple (C++)** :
```cpp
// Utiliser le preset P4 (equilibre)
GUID presetP4 = /* GUID pour le preset P4 */;
pNVEnc->SetPreset(presetP4);
```
---

### Paramètre de qualité (QP)

#### SetQp / GetQp

Définit le paramètre de quantification pour le mode CQP.

**Syntaxe (C++)** :
```cpp
HRESULT SetQp(int v);
HRESULT GetQp(int *v);
```

**Paramètres** :
- `v` : valeur QP (0-51)
  - Valeurs plus faibles = meilleure qualité, fichiers plus volumineux
  - Valeurs plus élevées = qualité inférieure, fichiers plus petits
  - Plage typique : 18-28

**Retourne** : `S_OK` en cas de succès.

**Notes d'utilisation** :
- Efficace uniquement en mode de contrôle CQP
- Ignoré en modes CBR/VBR
- Valeurs recommandées :
  - Haute qualité : 18-22
  - Qualité moyenne : 23-26
  - Basse qualité : 27-30

**Exemple (C++)** :
```cpp
// Encodage CQP haute qualite
pNVEnc->SetRateControl(0); // Mode CQP
pNVEnc->SetQp(20);         // Haute qualite
```

---
### Configuration des images B
#### SetBFrames / GetBFrames
Définit le nombre d'images B entre les images I et P.
**Syntaxe (C++)** :
```cpp
HRESULT SetBFrames(int v);
HRESULT GetBFrames(int *v);
```
**Paramètres** :
- `v` : nombre d'images B (0-4)
  - `0` — pas d'images B (latence minimale)
  - `1-2` — amélioration modérée de la compression
  - `3-4` — meilleure compression (latence plus élevée)
**Retourne** : `S_OK` en cas de succès.
**Notes d'utilisation** :
- Les images B améliorent l'efficacité de compression
- Plus d'images B = latence plus élevée
- Nécessite Turing (RTX 20XX) ou plus récent pour la prise en charge complète
- Valeurs recommandées :
  - Faible latence : 0
  - Streaming : 2
  - Enregistrement : 3
**Exemple (C++)** :
```cpp
// Faible latence - desactiver les images B
pNVEnc->SetBFrames(0);
// Enregistrement haute qualite - utiliser des images B
pNVEnc->SetBFrames(3);
```
---

### Configuration du GOP

#### SetGOP / GetGOP

Définit la taille du Group of Pictures (intervalle d'images-clés).

**Syntaxe (C++)** :
```cpp
HRESULT SetGOP(int v);
HRESULT GetGOP(int *v);
```

**Paramètres** :
- `v` : taille du GOP en images
  - Valeurs typiques : 30-300 images
  - Fréquence d'images × secondes = taille de GOP
  - Exemple : 60 fps × 2 secondes = taille de GOP de 120

**Retourne** : `S_OK` en cas de succès.

**Notes d'utilisation** :
- GOP plus petit = meilleure navigation, fichier plus volumineux
- GOP plus grand = meilleure compression, mauvaise navigation
- Pour le streaming : 2-4 secondes (fps × 2-4)
- Pour l'enregistrement : 5-10 secondes

**Exemple (C++)** :
```cpp
// GOP de 2 secondes pour streaming a 30 fps
pNVEnc->SetGOP(60);

// GOP de 5 secondes pour enregistrement a 60 fps
pNVEnc->SetGOP(300);
```

---
### Configuration du débit binaire
#### SetBitrate / GetBitrate
Définit le débit binaire cible pour l'encodage.
**Syntaxe (C++)** :
```cpp
HRESULT SetBitrate(int v);
HRESULT GetBitrate(int *v);
```
**Paramètres** :
- `v` : débit binaire en bits par seconde (bps)
**Retourne** : `S_OK` en cas de succès.
**Débits binaires recommandés** :
| Résolution | Fréquence | Débit (H.264) | Débit (H.265) |
|------------|-----------|-----------------|-----------------|
| 720p | 30 fps | 2,5-4 Mbps | 1,5-2,5 Mbps |
| 720p | 60 fps | 4-6 Mbps | 2,5-4 Mbps |
| 1080p | 30 fps | 4-6 Mbps | 2,5-4 Mbps |
| 1080p | 60 fps | 8-12 Mbps | 5-8 Mbps |
| 1440p | 30 fps | 10-15 Mbps | 6-10 Mbps |
| 1440p | 60 fps | 15-25 Mbps | 10-15 Mbps |
| 4K | 30 fps | 25-40 Mbps | 15-25 Mbps |
| 4K | 60 fps | 45-70 Mbps | 30-45 Mbps |
**Exemple (C++)** :
```cpp
// Streaming 1080p @ 60 fps
pNVEnc->SetBitrate(10000000); // 10 Mbps
```
---

### Configuration du tampon VBV

#### SetVbvBitrate / GetVbvBitrate

Définit le débit binaire du tampon VBV (Video Buffering Verifier).

**Syntaxe (C++)** :
```cpp
HRESULT SetVbvBitrate(int v);
HRESULT GetVbvBitrate(int *v);
```

**Paramètres** :
- `v` : débit VBV en bps (généralement identique ou supérieur au débit cible)

**Notes d'utilisation** :
- Contrôle les pics maximaux de débit
- Généralement défini à 1,0-1,5 × le débit cible
- Important pour le streaming afin d'éviter les sous-alimentations de tampon

---
#### SetVbvSize / GetVbvSize
Définit la taille du tampon VBV.
**Syntaxe (C++)** :
```cpp
HRESULT SetVbvSize(int v);
HRESULT GetVbvSize(int *v);
```
**Paramètres** :
- `v` : taille du tampon VBV en bits
**Notes d'utilisation** :
- Tampon plus grand = débit plus fluide mais latence plus élevée
- Tampon plus petit = latence plus faible mais plus de variance de débit
- Typique : 1-2 secondes de vidéo au débit cible
**Exemple (C++)** :
```cpp
// Flux 10 Mbps avec tampon de 2 secondes
pNVEnc->SetBitrate(10000000);
pNVEnc->SetVbvBitrate(12000000);  // 1.2x debit
pNVEnc->SetVbvSize(20000000);     // 2 secondes
```
---

### Configuration du profil

#### SetProfile / GetProfile

Définit le profil d'encodage H.264/H.265.

**Syntaxe (C++)** :
```cpp
HRESULT SetProfile(GUID v);
HRESULT GetProfile(GUID *v);
```

**Paramètres** :
- `v` : GUID du profil

**Profils H.264** :
- **Baseline** — fonctionnalités de base, compatibilité mobile
- **Main** — fonctionnalités standard, la plupart des appareils
- **High** — fonctionnalités avancées, contenu HD/4K

**Profils H.265** :
- **Main** — 8 bits, 4:2:0
- **Main 10** — 10 bits, prise en charge HDR

**Notes d'utilisation** :
- Utilisez le profil High pour H.264 dans la plupart des cas
- Utilisez le profil Main pour une compatibilité maximale
- HEVC Main 10 pour le contenu HDR

---
### Configuration du niveau
#### SetLevel / GetLevel
Définit le niveau de profil (contraintes de résolution/débit).
**Syntaxe (C++)** :
```cpp
HRESULT SetLevel(int v);
HRESULT GetLevel(int *v);
```
**Paramètres** :
- `v` : valeur de niveau (voir le tableau des niveaux H.264/H.265)
**Niveaux H.264 courants** :
- **30** (3.0) — vidéo SD
- **31** (3.1) — 720p @ 30 fps
- **40** (4.0) — 1080p @ 30 fps
- **41** (4.1) — 1080p @ 60 fps
- **50** (5.0) — 4K @ 30 fps
- **51** (5.1) — 4K @ 60 fps
**Exemple (C++)** :
```cpp
// 1080p @ 60 fps
pNVEnc->SetLevel(41);
```
---

### Sélection du codec

#### SetCodec / GetCodec

Définit le codec vidéo à utiliser.

**Syntaxe (C++)** :
```cpp
HRESULT SetCodec(int v);
HRESULT GetCodec(int *v);
```

**Paramètres** :
- `v` : type de codec
  - `0` — H.264/AVC
  - `1` — H.265/HEVC

**Retourne** : `S_OK` en cas de succès.

**Notes d'utilisation** :
- H.264 pour une compatibilité maximale
- H.265 pour une meilleure compression (fichiers 40-50 % plus petits)
- H.265 nécessite un GPU Maxwell (GTX 900) ou plus récent

**Exemple (C++)** :
```cpp
// Utiliser H.265
pNVEnc->SetCodec(1);
```

---
## Méthodes INVEncConfig2
### CheckNVENCAvailable
Vérifie si l'encodage matériel NVENC est disponible sur le système.
**Syntaxe (C++)** :
```cpp
HRESULT CheckNVENCAvailable(BOOL* result, int* status);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int CheckNVENCAvailable([MarshalAs(UnmanagedType.Bool)] out bool result, out int status);
```
**Paramètres** :
- `result` : reçoit `TRUE` si NVENC est disponible, `FALSE` sinon
- `status` : reçoit le code d'état NVENC (spécifique au vendeur)
**Retourne** : `S_OK` (0) en cas de succès.
**Notes d'utilisation** :
- Appelez ceci avant d'essayer d'utiliser l'encodeur NVENC
- Retourne `FALSE` si :
  - Aucun GPU NVIDIA n'est présent
  - Le GPU ne prend pas en charge NVENC (pré-Kepler)
  - Les pilotes NVIDIA ne sont pas installés
  - La bibliothèque NVENC n'est pas disponible
- Le code d'état fournit des informations de diagnostic supplémentaires
**Exemple (C++)** :
```cpp
#include "Intf2.h"
HRESULT CheckNVENCSupport(IBaseFilter* pEncoder)
{
    HRESULT hr;
    INVEncConfig2* pNVEnc2 = nullptr;
    hr = pEncoder->QueryInterface(IID_INVEncConfig2, (void**)&pNVEnc2);
    if (FAILED(hr))
    {
        // INVEncConfig2 non pris en charge par ce filtre
        return hr;
    }
    BOOL available = FALSE;
    int status = 0;
    hr = pNVEnc2->CheckNVENCAvailable(&available, &status);
    if (SUCCEEDED(hr))
    {
        if (available)
        {
            printf("NVENC is available (status: %d)\n", status);
            // Proceder a la configuration NVENC
        }
        else
        {
            printf("NVENC not available (status: %d)\n", status);
            // Solution de repli vers l'encodeur logiciel
        }
    }
    pNVEnc2->Release();
    return hr;
}
```
**Exemple (C#)** :
```csharp
using VisioForge.DirectShowAPI;
public bool IsNVENCAvailable(IBaseFilter encoder)
{
    var nvenc2 = encoder as INVEncConfig2;
    if (nvenc2 == null)
    {
        // INVEncConfig2 non pris en charge
        return false;
    }
    bool available;
    int status;
    int hr = nvenc2.CheckNVENCAvailable(out available, out status);
    if (hr == 0)
    {
        if (available)
        {
            Console.WriteLine($"NVENC is available (status: {status})");
            return true;
        }
        else
        {
            Console.WriteLine($"NVENC not available (status: {status})");
            return false;
        }
    }
    return false;
}
```
**Exemple (Delphi)** :
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
        WriteLn(Format('NVENC is available (status: %d)', [Status]));
        Result := True;
      end
      else
      begin
        WriteLn(Format('NVENC not available (status: %d)', [Status]));
      end;
    end;
    NVEnc2 := nil;
  end;
end;
```
---

## Exemples de configuration complets

### Exemple 1 : streaming faible latence (C++)

```cpp
#include "Intf.h"

HRESULT ConfigureLowLatencyNVENC(IBaseFilter* pEncoder)
{
    HRESULT hr;
    INVEncConfig* pNVEnc = nullptr;

    hr = pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
    if (FAILED(hr))
        return hr;

    // Configuration de base
    pNVEnc->SetDeviceType(0);           // Premier GPU
    pNVEnc->SetCodec(0);                // H.264
    pNVEnc->SetPictureStructure(0);     // Progressif

    // Parametres faible latence
    pNVEnc->SetRateControl(2);          // CBR
    pNVEnc->SetBitrate(5000000);        // 5 Mbps
    pNVEnc->SetBFrames(0);              // Pas d'images B
    pNVEnc->SetGOP(60);                 // GOP de 2 secondes (30 fps)
    pNVEnc->SetNumBuffers(4);           // Bufferisation minimale

    // Preset rapide
    GUID presetP2 = /* GUID de P2 */;
    pNVEnc->SetPreset(presetP2);

    // Profil/niveau pour 1080p30
    GUID highProfile = /* GUID du profil High */;
    pNVEnc->SetProfile(highProfile);
    pNVEnc->SetLevel(40);               // Level 4.0

    pNVEnc->Release();
    return S_OK;
}
```

### Exemple 2 : enregistrement haute qualité (C#)

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
            throw new NotSupportedException("NVENC not available");

        // Configuration de base
        nvenc.SetDeviceType(0);          // Premier GPU
        nvenc.SetCodec(1);               // H.265 pour une meilleure compression
        nvenc.SetPictureStructure(0);    // Progressif

        // Parametres VBR haute qualite
        nvenc.SetRateControl(1);         // VBR
        nvenc.SetBitrate(15000000);      // Moyenne 15 Mbps
        nvenc.SetBFrames(3);             // Utiliser des images B
        nvenc.SetGOP(300);               // GOP de 5 secondes (60 fps)
        nvenc.SetNumBuffers(8);          // Plus de bufferisation pour la qualite

        // Preset qualite
        Guid presetP6 = /* GUID de P6 */;
        nvenc.SetPreset(presetP6);

        // Profil HEVC Main pour 4K
        Guid hevcMain = /* GUID HEVC Main */;
        nvenc.SetProfile(hevcMain);
        nvenc.SetLevel(51);              // Level 5.1 pour 4K60

        // Configuration VBV
        nvenc.SetVbvBitrate(20000000);   // Max 20 Mbps
        nvenc.SetVbvSize(30000000);      // Tampon de 2 secondes
    }
}
```

### Exemple 3 : streaming équilibré (C++)

```cpp
HRESULT ConfigureBalancedStreaming(IBaseFilter* pEncoder)
{
    INVEncConfig* pNVEnc = nullptr;
    pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);

    // Peripherique et codec
    pNVEnc->SetDeviceType(0);
    pNVEnc->SetCodec(0);                // H.264 pour la compatibilite

    // Streaming CBR equilibre
    pNVEnc->SetRateControl(2);          // CBR
    pNVEnc->SetBitrate(8000000);        // 8 Mbps
    pNVEnc->SetBFrames(2);              // Images B moderees
    pNVEnc->SetGOP(120);                // GOP de 2 secondes (60 fps)
    pNVEnc->SetNumBuffers(6);           // Bufferisation standard

    // Preset equilibre P4
    GUID presetP4 = /* GUID de P4 */;
    pNVEnc->SetPreset(presetP4);

    // Profil/niveau 1080p60
    GUID highProfile = /* GUID du profil High */;
    pNVEnc->SetProfile(highProfile);
    pNVEnc->SetLevel(41);

    // VBV pour le streaming
    pNVEnc->SetVbvBitrate(10000000);    // 1.25x debit
    pNVEnc->SetVbvSize(16000000);       // Tampon de 2 secondes

    pNVEnc->Release();
    return S_OK;
}
```

---
## Bonnes pratiques
### Recommandations générales
1. **Utilisez le préréglage P4 par défaut** — meilleur équilibre qualité/performances
2. **CBR pour le streaming** — débit prévisible pour la diffusion réseau
3. **VBR pour l'enregistrement** — meilleure qualité pour le stockage de fichiers
4. **Désactivez les images B pour la faible latence** — réduit le délai d'encodage
5. **Adaptez le GOP à la fréquence d'images** — 2-4 secondes typique (fps × 2-4)
### Optimisation de la qualité
1. **Préréglage plus élevé = meilleure qualité** — utilisez P5-P7 lorsque le temps d'encodage le permet
2. **Plus d'images B = meilleure compression** — utilisez 3 pour l'enregistrement
3. **Débit approprié** — n'allez pas trop bas, la qualité en souffre considérablement
4. **Taille de tampon VBV** — 1-2 secondes au débit cible
### Optimisation des performances
1. **Préréglage plus faible = encodage plus rapide** — utilisez P1-P3 pour le temps réel
2. **Désactivez les images B** — réduit la latence et la complexité
3. **Moins de tampons d'encodage** — latence plus faible mais risque de pertes
4. **Sélectionnez le GPU approprié** — utilisez SetDeviceType() pour les systèmes multi-GPU
### Compatibilité
1. **Utilisez le profil H.264 High** — compatibilité maximale
2. **Définissez le bon niveau** — adapté à la résolution et à la fréquence d'images
3. **CBR pour le streaming** — plus compatible avec lecteurs/serveurs
4. **Taille de GOP standard** — 2-4 secondes
---

## Dépannage

### Problème : NVENC non disponible

**Symptômes** : QueryInterface échoue pour INVEncConfig

**Solutions** :
- Vérifiez qu'un GPU NVIDIA est installé
- Vérifiez la génération du GPU (Kepler ou plus récente requise)
- Mettez à jour les pilotes NVIDIA vers la dernière version
- Vérifiez que le filtre DirectShow est enregistré

### Problème : qualité de sortie médiocre

**Solutions** :
```cpp
// Augmenter le debit
pNVEnc->SetBitrate(15000000);  // Debit plus eleve

// Utiliser un meilleur preset
pNVEnc->SetPreset(presetP6);   // Plus lent mais meilleur

// Ajouter des images B
pNVEnc->SetBFrames(3);         // Meilleure compression
```

### Problème : latence élevée

**Solutions** :
```cpp
// Desactiver les images B
pNVEnc->SetBFrames(0);

// Utiliser un preset plus rapide
pNVEnc->SetPreset(presetP1);

// Reduire les tampons
pNVEnc->SetNumBuffers(4);

// GOP plus petit
pNVEnc->SetGOP(30);  // 1 seconde a 30 fps
```

### Problème : pics de débit

**Solutions** :
```cpp
// Utiliser CBR au lieu de VBR
pNVEnc->SetRateControl(2);

// Configurer correctement le VBV
pNVEnc->SetVbvBitrate(bitrate * 1.2);
pNVEnc->SetVbvSize(bitrate * 2);
```

---
## Benchmarks de performance
### Performances d'encodage typiques
| Résolution | Préréglage | Génération GPU | FPS (approx.) |
|------------|--------|----------------|--------------|
| 1080p | P1 | Pascal+ | 200-300 |
| 1080p | P4 | Pascal+ | 150-200 |
| 1080p | P7 | Pascal+ | 60-100 |
| 4K | P1 | Turing+ | 90-120 |
| 4K | P4 | Turing+ | 60-90 |
| 4K | P7 | Turing+ | 30-50 |
### Comparaison de qualité (PSNR)
| Préréglage | Qualité vs x264 | Vitesse vs x264 |
|--------|-----------------|---------------|
| P1 | -2 dB | 100× plus rapide |
| P4 | -0,5 dB | 50× plus rapide |
| P7 | ≈ égal | 20× plus rapide |
---

## Interfaces associées

- **IAMVideoCompression** — interface de compression DirectShow de base
- **IBaseFilter** — interface de base du filtre DirectShow
- **IMediaControl** — contrôle du graphe (run, stop)

## Voir aussi

- [Présentation du pack de filtres d'encodage](../index.md)
- [Référence des codecs](../codecs-reference.md)
- [Exemples de code](../examples.md)
- [Documentation NVIDIA NVENC](https://developer.nvidia.com/video-codec-sdk)
