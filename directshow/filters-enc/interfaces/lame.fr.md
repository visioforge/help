---
title: Paramètres de l'encodeur MP3 LAME via COM DirectShow
description: Interface IAudioEncoderProperties pour l'encodage MP3 LAME — modes débit variable et constant et configuration de la qualité.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - MP3
  - C#
primary_api_classes:
  - IBaseFilter
  - AudioEncoder

---

# Référence de l'interface de l'encodeur MP3 LAME

## Vue d'ensemble

L'interface `IAudioEncoderProperties` fournit un contrôle complet de l'encodage audio MP3 LAME. LAME (LAME Ain't an MP3 Encoder) est un encodeur MP3 de haute qualité qui produit une excellente qualité audio avec une compression efficace.

Cette interface permet la configuration du débit binaire, de la qualité, des paramètres de débit binaire variable (VBR) et de divers indicateurs d'encodage pour une sortie MP3 optimale.

## Définition de l'interface

- **Nom de l'interface** : `IAudioEncoderProperties`
- **GUID** : `{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}`
- **Hérite de** : `IUnknown`

## Définitions de l'interface

### Définition C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interface de l'encodeur MP3 LAME.
    /// </summary>
    /// <remarks>
    /// Configurer les parametres de l'encodeur audio MPEG avec un type de flux
    /// d'entree non specifie peut entrainer des comportements anormaux et des
    /// resultats confus. Dans la plupart des cas, les parametres specifies seront
    /// remplaces par les valeurs par defaut du type de media d'entree.
    /// Pour obtenir des resultats corrects, utilisez cette interface sur le filtre
    /// encodeur audio avec un pin d'entree connecte a une source valide.
    /// </remarks>
    [ComImport]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [Guid("595EB9D1-F454-41AD-A1FA-EC232AD9DA52")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEncoderProperties
    {
        // Controle de la sortie PES
        [PreserveSig]
        int get_PESOutputEnabled(out int dwEnabled);

        [PreserveSig]
        int set_PESOutputEnabled([In] int dwEnabled);

        // Configuration du debit binaire
        [PreserveSig]
        int get_Bitrate(out int dwBitrate);

        [PreserveSig]
        int set_Bitrate([In] int dwBitrate);

        // Debit binaire variable (VBR)
        [PreserveSig]
        int get_Variable(out int dwVariable);

        [PreserveSig]
        int set_Variable([In] int dwVariable);

        [PreserveSig]
        int get_VariableMin(out int dwmin);

        [PreserveSig]
        int set_VariableMin([In] int dwmin);

        [PreserveSig]
        int get_VariableMax(out int dwmax);

        [PreserveSig]
        int set_VariableMax([In] int dwmax);

        // Parametres de qualite
        [PreserveSig]
        int get_Quality(out int dwQuality);

        [PreserveSig]
        int set_Quality([In] int dwQuality);

        [PreserveSig]
        int get_VariableQ(out int dwVBRq);

        [PreserveSig]
        int set_VariableQ([In] int dwVBRq);

        // Informations source
        [PreserveSig]
        int get_SourceSampleRate(out int dwSampleRate);

        [PreserveSig]
        int get_SourceChannels(out int dwChannels);

        // Configuration de sortie
        [PreserveSig]
        int get_SampleRate(out int dwSampleRate);

        [PreserveSig]
        int set_SampleRate([In] int dwSampleRate);

        [PreserveSig]
        int get_ChannelMode(out int dwChannelMode);

        [PreserveSig]
        int set_ChannelMode([In] int dwChannelMode);

        // Indicateurs
        [PreserveSig]
        int get_CRCFlag(out int dwFlag);

        [PreserveSig]
        int set_CRCFlag([In] int dwFlag);

        [PreserveSig]
        int get_OriginalFlag(out int dwFlag);

        [PreserveSig]
        int set_OriginalFlag([In] int dwFlag);

        [PreserveSig]
        int get_CopyrightFlag(out int dwFlag);

        [PreserveSig]
        int set_CopyrightFlag([In] int dwFlag);

        [PreserveSig]
        int get_EnforceVBRmin(out int dwFlag);

        [PreserveSig]
        int set_EnforceVBRmin([In] int dwFlag);

        [PreserveSig]
        int get_VoiceMode(out int dwFlag);

        [PreserveSig]
        int set_VoiceMode([In] int dwFlag);

        [PreserveSig]
        int get_KeepAllFreq(out int dwFlag);

        [PreserveSig]
        int set_KeepAllFreq([In] int dwFlag);

        [PreserveSig]
        int get_StrictISO(out int dwFlag);

        [PreserveSig]
        int set_StrictISO([In] int dwFlag);

        [PreserveSig]
        int get_NoShortBlock(out int dwDisable);

        [PreserveSig]
        int set_NoShortBlock([In] int dwDisable);

        [PreserveSig]
        int get_XingTag(out int dwXingTag);

        [PreserveSig]
        int set_XingTag([In] int dwXingTag);

        [PreserveSig]
        int get_ForceMS(out int dwFlag);

        [PreserveSig]
        int set_ForceMS([In] int dwFlag);

        [PreserveSig]
        int get_ModeFixed(out int dwFlag);

        [PreserveSig]
        int set_ModeFixed([In] int dwFlag);

        // Gestion de la configuration
        // pcBlock est un tampon BYTE[] en C++ ; utiliser LPArray avec SizeParamIndex=1
        // pour que le marshaler lise pdwSize octets depuis le code natif.
        [PreserveSig]
        int get_ParameterBlockSize(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pcBlock,
            out int pdwSize);

        [PreserveSig]
        int set_ParameterBlockSize(
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pcBlock,
            [In] int dwSize);

        [PreserveSig]
        int DefaultAudioEncoderProperties();

        [PreserveSig]
        int LoadAudioEncoderPropertiesFromRegistry();

        [PreserveSig]
        int SaveAudioEncoderPropertiesToRegistry();

        [PreserveSig]
        int InputTypeDefined();
    }
}
```

### Définition C++

```cpp
#include <unknwn.h>

// {595EB9D1-F454-41AD-A1FA-EC232AD9DA52}
static const GUID IID_IAudioEncoderProperties =
{ 0x595eb9d1, 0xf454, 0x41ad, { 0xa1, 0xfa, 0xec, 0x23, 0x2a, 0xd9, 0xda, 0x52 } };

DECLARE_INTERFACE_(IAudioEncoderProperties, IUnknown)
{
    // Sortie PES
    STDMETHOD(get_PESOutputEnabled)(THIS_ int* dwEnabled) PURE;
    STDMETHOD(set_PESOutputEnabled)(THIS_ int dwEnabled) PURE;

    // Debit binaire
    STDMETHOD(get_Bitrate)(THIS_ int* dwBitrate) PURE;
    STDMETHOD(set_Bitrate)(THIS_ int dwBitrate) PURE;

    // Debit binaire variable
    STDMETHOD(get_Variable)(THIS_ int* dwVariable) PURE;
    STDMETHOD(set_Variable)(THIS_ int dwVariable) PURE;
    STDMETHOD(get_VariableMin)(THIS_ int* dwmin) PURE;
    STDMETHOD(set_VariableMin)(THIS_ int dwmin) PURE;
    STDMETHOD(get_VariableMax)(THIS_ int* dwmax) PURE;
    STDMETHOD(set_VariableMax)(THIS_ int dwmax) PURE;

    // Qualite
    STDMETHOD(get_Quality)(THIS_ int* dwQuality) PURE;
    STDMETHOD(set_Quality)(THIS_ int dwQuality) PURE;
    STDMETHOD(get_VariableQ)(THIS_ int* dwVBRq) PURE;
    STDMETHOD(set_VariableQ)(THIS_ int dwVBRq) PURE;

    // Informations source
    STDMETHOD(get_SourceSampleRate)(THIS_ int* dwSampleRate) PURE;
    STDMETHOD(get_SourceChannels)(THIS_ int* dwChannels) PURE;

    // Configuration de sortie
    STDMETHOD(get_SampleRate)(THIS_ int* dwSampleRate) PURE;
    STDMETHOD(set_SampleRate)(THIS_ int dwSampleRate) PURE;
    STDMETHOD(get_ChannelMode)(THIS_ int* dwChannelMode) PURE;
    STDMETHOD(set_ChannelMode)(THIS_ int dwChannelMode) PURE;

    // Indicateurs
    STDMETHOD(get_CRCFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_CRCFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_OriginalFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_OriginalFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_CopyrightFlag)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_CopyrightFlag)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_EnforceVBRmin)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_EnforceVBRmin)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_VoiceMode)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_VoiceMode)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_KeepAllFreq)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_KeepAllFreq)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_StrictISO)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_StrictISO)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_NoShortBlock)(THIS_ int* dwDisable) PURE;
    STDMETHOD(set_NoShortBlock)(THIS_ int dwDisable) PURE;
    STDMETHOD(get_XingTag)(THIS_ int* dwXingTag) PURE;
    STDMETHOD(set_XingTag)(THIS_ int dwXingTag) PURE;
    STDMETHOD(get_ForceMS)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_ForceMS)(THIS_ int dwFlag) PURE;
    STDMETHOD(get_ModeFixed)(THIS_ int* dwFlag) PURE;
    STDMETHOD(set_ModeFixed)(THIS_ int dwFlag) PURE;

    // Gestion de la configuration
    STDMETHOD(get_ParameterBlockSize)(THIS_ byte* pcBlock, int* pdwSize) PURE;
    STDMETHOD(set_ParameterBlockSize)(THIS_ byte* pcBlock, int dwSize) PURE;
    STDMETHOD(DefaultAudioEncoderProperties)(THIS) PURE;
    STDMETHOD(LoadAudioEncoderPropertiesFromRegistry)(THIS) PURE;
    STDMETHOD(SaveAudioEncoderPropertiesToRegistry)(THIS) PURE;
    STDMETHOD(InputTypeDefined)(THIS) PURE;
};
```

### Définition Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IAudioEncoderProperties: TGUID = '{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}';

type
  IAudioEncoderProperties = interface(IUnknown)
    ['{595EB9D1-F454-41AD-A1FA-EC232AD9DA52}']

    // Sortie PES
    function get_PESOutputEnabled(out dwEnabled: Integer): HRESULT; stdcall;
    function set_PESOutputEnabled(dwEnabled: Integer): HRESULT; stdcall;

    // Debit binaire
    function get_Bitrate(out dwBitrate: Integer): HRESULT; stdcall;
    function set_Bitrate(dwBitrate: Integer): HRESULT; stdcall;

    // Debit binaire variable
    function get_Variable(out dwVariable: Integer): HRESULT; stdcall;
    function set_Variable(dwVariable: Integer): HRESULT; stdcall;
    function get_VariableMin(out dwmin: Integer): HRESULT; stdcall;
    function set_VariableMin(dwmin: Integer): HRESULT; stdcall;
    function get_VariableMax(out dwmax: Integer): HRESULT; stdcall;
    function set_VariableMax(dwmax: Integer): HRESULT; stdcall;

    // Qualite
    function get_Quality(out dwQuality: Integer): HRESULT; stdcall;
    function set_Quality(dwQuality: Integer): HRESULT; stdcall;
    function get_VariableQ(out dwVBRq: Integer): HRESULT; stdcall;
    function set_VariableQ(dwVBRq: Integer): HRESULT; stdcall;

    // Informations source
    function get_SourceSampleRate(out dwSampleRate: Integer): HRESULT; stdcall;
    function get_SourceChannels(out dwChannels: Integer): HRESULT; stdcall;

    // Configuration de sortie
    function get_SampleRate(out dwSampleRate: Integer): HRESULT; stdcall;
    function set_SampleRate(dwSampleRate: Integer): HRESULT; stdcall;
    function get_ChannelMode(out dwChannelMode: Integer): HRESULT; stdcall;
    function set_ChannelMode(dwChannelMode: Integer): HRESULT; stdcall;

    // Indicateurs
    function get_CRCFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_CRCFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_OriginalFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_OriginalFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_CopyrightFlag(out dwFlag: Integer): HRESULT; stdcall;
    function set_CopyrightFlag(dwFlag: Integer): HRESULT; stdcall;
    function get_EnforceVBRmin(out dwFlag: Integer): HRESULT; stdcall;
    function set_EnforceVBRmin(dwFlag: Integer): HRESULT; stdcall;
    function get_VoiceMode(out dwFlag: Integer): HRESULT; stdcall;
    function set_VoiceMode(dwFlag: Integer): HRESULT; stdcall;
    function get_KeepAllFreq(out dwFlag: Integer): HRESULT; stdcall;
    function set_KeepAllFreq(dwFlag: Integer): HRESULT; stdcall;
    function get_StrictISO(out dwFlag: Integer): HRESULT; stdcall;
    function set_StrictISO(dwFlag: Integer): HRESULT; stdcall;
    function get_NoShortBlock(out dwDisable: Integer): HRESULT; stdcall;
    function set_NoShortBlock(dwDisable: Integer): HRESULT; stdcall;
    function get_XingTag(out dwXingTag: Integer): HRESULT; stdcall;
    function set_XingTag(dwXingTag: Integer): HRESULT; stdcall;
    function get_ForceMS(out dwFlag: Integer): HRESULT; stdcall;
    function set_ForceMS(dwFlag: Integer): HRESULT; stdcall;
    function get_ModeFixed(out dwFlag: Integer): HRESULT; stdcall;
    function set_ModeFixed(dwFlag: Integer): HRESULT; stdcall;

    // Gestion de la configuration
    function get_ParameterBlockSize(out pcBlock: Byte; out pdwSize: Integer): HRESULT; stdcall;
    function set_ParameterBlockSize(pcBlock: Byte; dwSize: Integer): HRESULT; stdcall;
    function DefaultAudioEncoderProperties: HRESULT; stdcall;
    function LoadAudioEncoderPropertiesFromRegistry: HRESULT; stdcall;
    function SaveAudioEncoderPropertiesToRegistry: HRESULT; stdcall;
    function InputTypeDefined: HRESULT; stdcall;
  end;
```

---
## Référence des méthodes
### Configuration du débit binaire
#### set_Bitrate / get_Bitrate
Définit ou récupère le débit binaire de compression cible en Kbits/s.
**Paramètres** :
- `dwBitrate` : débit binaire en kilobits par seconde
**Débits MP3 courants** :
- **320 kbps** — qualité la plus élevée, quasi-transparent
- **256 kbps** — très haute qualité
- **192 kbps** — haute qualité (recommandé pour la musique)
- **128 kbps** — qualité standard (acceptable pour la plupart des contenus)
- **96 kbps** — qualité inférieure, fichiers plus petits
- **64 kbps** — qualité voix/podcast
**Exemple (C#)** :
```csharp
var lame = audioEncoder as IAudioEncoderProperties;
if (lame != null)
{
    // Definir une haute qualite a 192 kbps
    lame.set_Bitrate(192);
}
```
---

### Débit binaire variable (VBR)

#### set_Variable / get_Variable

Active ou désactive le mode débit binaire variable.

**Paramètres** :
- `dwVariable` : 1 pour activer VBR, 0 pour désactiver (mode CBR)

**Notes d'utilisation** :
- VBR offre un meilleur rapport qualité/taille que CBR
- VBR alloue plus de bits aux passages audio complexes
- CBR offre des tailles de fichier prévisibles
- VBR est recommandé pour l'archivage musical

#### set_VariableMin / get_VariableMin

Définit le débit minimal pour le mode VBR.

**Paramètres** :
- `dwmin` : débit minimal en kbps

#### set_VariableMax / get_VariableMax

Définit le débit maximal pour le mode VBR.

**Paramètres** :
- `dwmax` : débit maximal en kbps

**Exemple (C#)** :
```csharp
// Activer VBR avec une plage de 128-256 kbps
lame.set_Variable(1);
lame.set_VariableMin(128);
lame.set_VariableMax(256);
lame.set_VariableQ(4); // Niveau de qualite VBR
```

---
### Paramètres de qualité
#### set_Quality / get_Quality
Définit la qualité d'encodage pour le mode CBR.
**Paramètres** :
- `dwQuality` : niveau de qualité (0-9)
  - **0** — qualité la plus élevée (le plus lent)
  - **2** — qualité quasi-maximale (recommandé)
  - **5** — bon équilibre qualité/vitesse
  - **7** — encodage plus rapide, qualité inférieure
  - **9** — qualité la plus basse (le plus rapide)
**Exemple (C++)** :
```cpp
IAudioEncoderProperties* pLame = nullptr;
pFilter->QueryInterface(IID_IAudioEncoderProperties, (void**)&pLame);
// Encodage CBR haute qualite
pLame->set_Bitrate(192);
pLame->set_Quality(2);
pLame->Release();
```
#### set_VariableQ / get_VariableQ
Définit le niveau de qualité pour le mode VBR.
**Paramètres** :
- `dwVBRq` : qualité VBR (0-9)
  - **0** — qualité la plus élevée (~245 kbps)
  - **2** — très haute qualité (~190 kbps)
  - **4** — haute qualité (~165 kbps) — recommandé
  - **6** — qualité moyenne (~130 kbps)
  - **9** — qualité la plus basse (~65 kbps)
---

### Mode de canal

#### set_ChannelMode / get_ChannelMode

Définit le mode d'encodage stéréo.

**Paramètres** :
- `dwChannelMode` : valeur du mode de canal
  - **0** — Stéréo
  - **1** — Joint Stereo (recommandé)
  - **2** — Dual Channel
  - **3** — Mono

**Notes d'utilisation** :
- Joint Stereo offre la meilleure qualité à bas débit
- Utilisez Stéréo pour l'écoute critique à haut débit
- Mono réduit la taille des fichiers pour la parole/podcasts

**Exemple (C#)** :
```csharp
// Joint stereo pour la musique a 192 kbps
lame.set_ChannelMode(1);
lame.set_Bitrate(192);
```

---
### Indicateurs d'encodage
#### set_CRCFlag / get_CRCFlag
Active la protection d'erreur CRC.
**Paramètres** :
- `dwFlag` : 1 pour activer, 0 pour désactiver
**Utilisation** : ajoute une détection d'erreurs, augmentation de taille minimale (~0,2 %)
#### set_CopyrightFlag / get_CopyrightFlag
Définit l'indicateur de copyright dans l'en-tête MP3.
**Paramètres** :
- `dwFlag` : 1 si soumis au droit d'auteur, 0 sinon
#### set_OriginalFlag / get_OriginalFlag
Définit l'indicateur original/copie.
**Paramètres** :
- `dwFlag` : 1 pour original, 0 pour copie
#### set_VoiceMode / get_VoiceMode
Optimise l'encodage pour le contenu vocal.
**Paramètres** :
- `dwFlag` : 1 pour activer l'optimisation vocale
**Utilisation** : améliore la qualité de la parole à bas débit
**Exemple (C#)** :
```csharp
// Optimiser pour podcast/contenu vocal
lame.set_VoiceMode(1);
lame.set_Bitrate(64);
lame.set_ChannelMode(3); // Mono
```
#### set_XingTag / get_XingTag
Ajoute la balise Xing VBR pour une navigation précise.
**Paramètres** :
- `dwFlag` : 1 pour ajouter la balise (recommandé pour VBR)
**Utilisation** : essentielle pour les fichiers VBR afin de permettre une navigation correcte
---

## Gestion de la configuration

### SaveAudioEncoderPropertiesToRegistry

Enregistre la configuration actuelle de l'encodeur dans le registre.

**Notes d'utilisation** :
- Doit être appelée après modification des propriétés
- Les paramètres persistent entre les sessions
- Nécessite les autorisations appropriées sur le registre

### LoadAudioEncoderPropertiesFromRegistry

Charge la configuration de l'encodeur depuis le registre.

### DefaultAudioEncoderProperties

Réinitialise toutes les propriétés de l'encodeur aux valeurs par défaut selon le type de flux d'entrée.

### InputTypeDefined

Vérifie si le format d'entrée a été spécifié.

**Retourne** :
- `S_OK` — le type d'entrée est défini, l'encodeur peut être configuré
- `E_FAIL` — type d'entrée non spécifié, la configuration peut échouer

---
## Exemples complets
### Exemple 1 : encodage musique haute qualité (C#)
```csharp
using VisioForge.DirectShowAPI;
public void ConfigureHighQualityMP3(IBaseFilter audioEncoder)
{
    var lame = audioEncoder as IAudioEncoderProperties;
    if (lame == null)
        return;
    // Verifier si l'entree est connectee
    if (lame.InputTypeDefined() != 0)
    {
        Console.WriteLine("Warning: Input not connected, using defaults");
    }
    // Parametres VBR haute qualite
    lame.set_Variable(1);              // Activer VBR
    lame.set_VariableQ(2);             // Tres haute qualite
    lame.set_VariableMin(192);         // Min 192 kbps
    lame.set_VariableMax(320);         // Max 320 kbps
    // Joint stereo pour l'efficacite
    lame.set_ChannelMode(1);
    // Indicateurs de qualite
    lame.set_XingTag(1);               // Ajouter la balise VBR
    lame.set_OriginalFlag(1);          // Marquer comme original
    lame.set_CopyrightFlag(1);         // Definir le copyright
    // Enregistrer les parametres
    lame.SaveAudioEncoderPropertiesToRegistry();
}
```
### Exemple 2 : encodage podcast/voix (C++)
```cpp
#include "LAME.h"
HRESULT ConfigurePodcastMP3(IBaseFilter* pAudioEncoder)
{
    HRESULT hr;
    IAudioEncoderProperties* pLame = nullptr;
    hr = pAudioEncoder->QueryInterface(IID_IAudioEncoderProperties,
                                       (void**)&pLame);
    if (FAILED(hr))
        return hr;
    // Parametres optimises pour la voix
    pLame->set_VoiceMode(1);           // Optimisation voix
    pLame->set_Bitrate(64);            // 64 kbps pour la parole
    pLame->set_Quality(5);             // Qualite equilibree
    pLame->set_ChannelMode(3);         // Mono
    // Desactiver VBR pour une taille de fichier previsible
    pLame->set_Variable(0);
    // Ajouter la balise Xing pour la compatibilite
    pLame->set_XingTag(1);
    // Enregistrer la configuration
    pLame->SaveAudioEncoderPropertiesToRegistry();
    pLame->Release();
    return S_OK;
}
```
### Exemple 3 : encodage musical standard (Delphi)
```delphi
procedure ConfigureStandardMP3(AudioEncoder: IBaseFilter);
var
  Lame: IAudioEncoderProperties;
  hr: HRESULT;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IAudioEncoderProperties, Lame)) then
  begin
    // Parametres musicaux VBR standard
    Lame.set_Variable(1);              // Activer VBR
    Lame.set_VariableQ(4);             // Haute qualite (~165 kbps moyen)
    Lame.set_VariableMin(128);         // Min 128 kbps
    Lame.set_VariableMax(256);         // Max 256 kbps
    // Joint stereo
    Lame.set_ChannelMode(1);
    // Indicateurs essentiels
    Lame.set_XingTag(1);               // Balise VBR pour la navigation
    // Enregistrer dans le registre
    Lame.SaveAudioEncoderPropertiesToRegistry;
    Lame := nil;
  end;
end;
```
---

## Bonnes pratiques

### Recommandations de qualité

1. **Archivage musical** : VBR Q0-Q2 (245-190 kbps en moyenne)
2. **Distribution musicale** : VBR Q4 (165 kbps) ou CBR 192 kbps
3. **Streaming** : CBR 128 kbps
4. **Podcasts/parole** : CBR 64 kbps mono avec mode voix

### Conseils de performance

1. **Utilisez Joint Stereo** aux débits inférieurs à 192 kbps
2. **Activez VBR** pour un meilleur rapport qualité/taille
3. **Ajoutez la balise Xing** pour les fichiers VBR
4. **Utilisez le mode voix** pour le contenu vocal à < 96 kbps

### Flux de configuration

1. Connectez le pin d'entrée avant la configuration
2. Vérifiez `InputTypeDefined()` avant de définir les propriétés
3. Configurez toutes les propriétés souhaitées
4. Appelez `SaveAudioEncoderPropertiesToRegistry()`
5. Vérifiez les paramètres avec les méthodes get

---
## Dépannage
### Problème : paramètres non appliqués
**Solution** :
```csharp
// S'assurer que l'entree est connectee d'abord
if (lame.InputTypeDefined() == 0)
{
    // Configurer les parametres
    lame.set_Bitrate(192);
    lame.SaveAudioEncoderPropertiesToRegistry();
}
else
{
    // Connecter l'entree d'abord, puis configurer
}
```
### Problème : qualité de sortie médiocre
**Solutions** :
- Augmentez la qualité VBR : `set_VariableQ(2)` ou inférieur
- Augmentez le débit CBR : `set_Bitrate(192)` ou supérieur
- Utilisez un meilleur paramètre de qualité : `set_Quality(2)`
- Désactivez le mode voix pour la musique : `set_VoiceMode(0)`
### Problème : fichiers volumineux
**Solutions** :
```cpp
// Utiliser VBR au lieu d'un CBR eleve
pLame->set_Variable(1);
pLame->set_VariableQ(4);        // ~165 kbps en moyenne
pLame->set_VariableMax(192);    // Plafonner le debit maximal
```
---

## Voir aussi

- [Présentation du pack de filtres d'encodage](../index.md)
- [Référence des codecs audio](../codecs-reference.md)
- [Encodeur AAC](aac.md)
- [Encodeur FLAC](flac.md)
