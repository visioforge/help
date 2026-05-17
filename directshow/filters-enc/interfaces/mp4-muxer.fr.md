---
title: Multiplexeur MP4 DirectShow — interfaces COM, timing
description: Interfaces DirectShow du multiplexeur MP4 — configuration de threading, correction de timing et options de streaming en direct pour sortie MP4.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - MP4
  - C#
primary_api_classes:
  - MP4V10Flags
  - IBaseFilter
  - MP4MuxerStandardConfig
  - MP4MuxerDeterministicConfig
  - MP4V10LiveStreamingConfig

---

# Référence de l'interface du multiplexeur MP4

## Vue d'ensemble

Les filtres DirectShow du multiplexeur MP4 fournissent des interfaces pour configurer la sortie au conteneur MP4 (MPEG-4 Part 14). Ces interfaces permettent aux développeurs de contrôler le comportement de threading, la correction de timing et la gestion spéciale pour les scénarios de streaming en direct.

Deux interfaces de multiplexeur sont disponibles :
- **IMP4MuxerConfig** : configuration de base du multiplexeur MP4 pour le threading et le timing
- **IMP4V10MuxerConfig** : configuration avancée pour le multiplexeur version 10 avec indicateurs de timing et contrôle du streaming en direct

## Interface IMP4MuxerConfig

### Vue d'ensemble

L'interface **IMP4MuxerConfig** fournit une configuration de base pour le multiplexage MP4, contrôlant le fonctionnement en thread unique et le comportement de correction de timing.

**GUID de l'interface** : `{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}`

**Hérite de** : `IUnknown`

### Définitions de l'interface

#### Définition C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interface de configuration du multiplexeur MP4.
    /// Controle le threading et le timing pour la creation de conteneurs MP4.
    /// </summary>
    [ComImport]
    [Guid("99DC9BE5-0AFA-45d4-8370-AB021FB07CF4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMP4MuxerConfig
    {
        /// <summary>
        /// Obtient l'etat de traitement en thread unique.
        /// </summary>
        /// <param name="pValue">Recoit true si le mode thread unique est active, false sinon</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int get_SingleThread([Out] [MarshalAs(UnmanagedType.Bool)] out bool pValue);

        /// <summary>
        /// Active ou desactive le traitement en thread unique.
        /// Si active, toutes les operations du multiplexeur s'executent sur un seul thread pour un comportement deterministe.
        /// </summary>
        /// <param name="value">True pour activer le mode thread unique, false pour multi-thread</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int put_SingleThread([In] [MarshalAs(UnmanagedType.Bool)] bool value);

        /// <summary>
        /// Obtient l'etat de la correction de timing.
        /// </summary>
        /// <param name="pValue">Recoit true si la correction de timing est activee, false sinon</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int get_CorrectTiming([Out] [MarshalAs(UnmanagedType.Bool)] out bool pValue);

        /// <summary>
        /// Active ou desactive la correction de timing.
        /// Si active, le multiplexeur ajuste les horodatages pour corriger la derive et les incoherences.
        /// </summary>
        /// <param name="value">True pour activer la correction de timing, false pour desactiver</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int put_CorrectTiming([In] [MarshalAs(UnmanagedType.Bool)] bool value);
    }
}
```

#### Définition C++

```cpp
#include <unknwn.h>

// {99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}
DEFINE_GUID(IID_IMP4MuxerConfig,
    0x99dc9be5, 0x0afa, 0x45d4, 0x83, 0x70, 0xab, 0x02, 0x1f, 0xb0, 0x7c, 0xf4);

/// <summary>
/// Interface de configuration du multiplexeur MP4.
/// Controle le threading et le timing.
/// </summary>
DECLARE_INTERFACE_(IMP4MuxerConfig, IUnknown)
{
    STDMETHOD(get_SingleThread)(THIS_ BOOL* pValue) PURE;
    STDMETHOD(put_SingleThread)(THIS_ BOOL value) PURE;
    STDMETHOD(get_CorrectTiming)(THIS_ BOOL* pValue) PURE;
    STDMETHOD(put_CorrectTiming)(THIS_ BOOL value) PURE;
};
```

#### Définition Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IMP4MuxerConfig: TGUID = '{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}';

type
  /// <summary>
  /// Interface de configuration du multiplexeur MP4.
  /// </summary>
  IMP4MuxerConfig = interface(IUnknown)
    ['{99DC9BE5-0AFA-45d4-8370-AB021FB07CF4}']

    function get_SingleThread(out pValue: BOOL): HRESULT; stdcall;
    function put_SingleThread(value: BOOL): HRESULT; stdcall;
    function get_CorrectTiming(out pValue: BOOL): HRESULT; stdcall;
    function put_CorrectTiming(value: BOOL): HRESULT; stdcall;
  end;
```

### Référence des méthodes

#### get_SingleThread / put_SingleThread

Contrôle si le multiplexeur traite les données avec un seul thread ou plusieurs.

**Mode thread unique (activé)** :
- Toutes les opérations de multiplexage s'exécutent sur un seul thread
- Comportement déterministe et prévisible
- Plus facile à déboguer
- Performances légèrement inférieures sur les systèmes multicœurs
- **Recommandé pour** : les scénarios nécessitant une sortie cohérente et reproductible

**Mode multi-thread (désactivé)** :
- Le multiplexeur peut utiliser plusieurs threads pour le traitement
- Meilleures performances sur les processeurs multicœurs
- Ordre d'opérations non déterministe
- **Recommandé pour** : l'encodage haute performance avec plusieurs flux

**Par défaut** : généralement multi-thread (false)

**Exemple** :
```csharp
// Activer le mode thread unique pour une sortie coherente
mp4Muxer.put_SingleThread(true);
```

#### get_CorrectTiming / put_CorrectTiming

Active ou désactive la correction automatique des horodatages pour les flux audio et vidéo.

**Correction de timing activée (true)** :
- Le multiplexeur ajuste automatiquement les horodatages pour corriger la dérive
- Corrige les incohérences de timing des filtres source
- Garantit une bonne synchronisation A/V
- Ajoute une faible surcharge de traitement
- **Recommandé pour** : la plupart des scénarios, en particulier avec des sources en direct

**Correction de timing désactivée (false)** :
- Horodatages transmis sans modification
- Suppose que les filtres source fournissent des horodatages précis
- Performances légèrement meilleures
- **À utiliser uniquement quand** : la source fournit des horodatages garantis précis

**Par défaut** : généralement activée (true)

**Exemple** :
```csharp
// Activer la correction de timing pour la synchro A/V
mp4Muxer.put_CorrectTiming(true);
```

---
## Interface IMP4V10MuxerConfig
### Vue d'ensemble
L'interface **IMP4V10MuxerConfig** fournit une configuration avancée pour le multiplexeur MP4 version 10, dont les indicateurs de remplacement de timing et le contrôle du streaming en direct.
**GUID de l'interface** : `{9E26CE8B-6708-4535-AAA4-23F9A97C7937}`
**Hérite de** : `IUnknown`
### Énumération MP4V10Flags
```csharp
/// <summary>
/// Indicateurs de configuration du multiplexeur MP4 v10.
/// </summary>
[Flags]
public enum MP4V10Flags
{
    /// <summary>
    /// Aucun indicateur special.
    /// </summary>
    None = 0,
    /// <summary>
    /// Mode de remplacement de temps - permet le controle manuel des horodatages.
    /// </summary>
    TimeOverride = 0x00000001,
    /// <summary>
    /// Mode d'ajustement de temps - active l'ajustement automatique des horodatages.
    /// </summary>
    TimeAdjust = 0x00000002
}
```
### Définitions de l'interface
#### Définition C#
```csharp
using System;
using System.Runtime.InteropServices;
namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Indicateurs du multiplexeur MP4 v10.
    /// </summary>
    [Flags]
    public enum MP4V10Flags
    {
        /// <summary>
        /// Par defaut - aucun indicateur special.
        /// </summary>
        None = 0,
        /// <summary>
        /// Remplacement de temps - permet le controle manuel des horodatages.
        /// </summary>
        TimeOverride = 0x00000001,
        /// <summary>
        /// Ajustement de temps - active l'ajustement automatique des horodatages.
        /// </summary>
        TimeAdjust = 0x00000002
    }
    /// <summary>
    /// Interface de configuration du multiplexeur MP4 version 10.
    /// Fournit un controle avance du timing et des options de streaming en direct.
    /// </summary>
    [ComImport]
    [Guid("9E26CE8B-6708-4535-AAA4-23F9A97C7937")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMP4V10MuxerConfig
    {
        /// <summary>
        /// Definit les indicateurs de configuration du multiplexeur.
        /// </summary>
        /// <param name="value">Combinaison de valeurs MP4V10Flags</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetFlags([In] uint value);
        /// <summary>
        /// Obtient les indicateurs de configuration actuels du multiplexeur.
        /// </summary>
        /// <param name="pValue">Recoit les indicateurs actuels</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetFlags([Out] out uint pValue);
        /// <summary>
        /// Desactive les optimisations de streaming en direct.
        /// Si desactive, le multiplexeur utilise le mode de sortie standard basee fichier.
        /// </summary>
        /// <param name="liveDisabled">True pour desactiver le mode live, false pour activer</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetLiveDisabled([MarshalAs(UnmanagedType.Bool)] bool liveDisabled);
    }
}
```
#### Définition C++
```cpp
#include <unknwn.h>
// {9E26CE8B-6708-4535-AAA4-23F9A97C7937}
DEFINE_GUID(IID_IMP4V10MuxerConfig,
    0x9e26ce8b, 0x6708, 0x4535, 0xaa, 0xa4, 0x23, 0xf9, 0xa9, 0x7c, 0x79, 0x37);
/// <summary>
/// Indicateurs du multiplexeur MP4 v10.
/// </summary>
enum MP4V10Flags
{
    MP4V10_NONE = 0,
    MP4V10_TIME_OVERRIDE = 0x00000001,
    MP4V10_TIME_ADJUST = 0x00000002
};
/// <summary>
/// Interface de configuration du multiplexeur MP4 version 10.
/// Fournit un controle avance du timing et des options de streaming en direct.
/// </summary>
DECLARE_INTERFACE_(IMP4V10MuxerConfig, IUnknown)
{
    STDMETHOD(SetFlags)(THIS_ unsigned long value) PURE;
    STDMETHOD(GetFlags)(THIS_ unsigned long* pValue) PURE;
    STDMETHOD(SetLiveDisabled)(THIS_ BOOL liveDisabled) PURE;
};
```
#### Définition Delphi
```delphi
uses
  ActiveX, ComObj;
const
  IID_IMP4V10MuxerConfig: TGUID = '{9E26CE8B-6708-4535-AAA4-23F9A97C7937}';
  // Constantes MP4V10Flags
  MP4V10_NONE = 0;
  MP4V10_TIME_OVERRIDE = $00000001;
  MP4V10_TIME_ADJUST = $00000002;
type
  /// <summary>
  /// Interface de configuration du multiplexeur MP4 version 10.
  /// </summary>
  IMP4V10MuxerConfig = interface(IUnknown)
    ['{9E26CE8B-6708-4535-AAA4-23F9A97C7937}']
    function SetFlags(value: Cardinal): HRESULT; stdcall;
    function GetFlags(out pValue: Cardinal): HRESULT; stdcall;
    function SetLiveDisabled(liveDisabled: BOOL): HRESULT; stdcall;
  end;
```
### Référence des méthodes
#### SetFlags / GetFlags
Définit ou récupère les indicateurs de configuration du multiplexeur qui contrôlent le comportement de timing.
**Valeurs MP4V10Flags** :
**None (0)** :
- Fonctionnement standard
- Gestion par défaut des horodatages
- Pas de modifications spéciales du timing
**TimeOverride (0x00000001)** :
- Active le remplacement manuel des horodatages
- Permet à l'application de contrôler directement les horodatages
- Désactive la génération automatique d'horodatages
- **À utiliser quand** : l'application a besoin d'un contrôle total du timing
**TimeAdjust (0x00000002)** :
- Active l'ajustement automatique des horodatages
- Le multiplexeur corrige la dérive et les irrégularités de timing
- Similaire à IMP4MuxerConfig::CorrectTiming
- **À utiliser pour** : les sources avec des horodatages incohérents
**Combinaison d'indicateurs** :
```csharp
// Activer a la fois remplacement et ajustement de temps
uint flags = (uint)(MP4V10Flags.TimeOverride | MP4V10Flags.TimeAdjust);
mp4V10Muxer.SetFlags(flags);
```
#### SetLiveDisabled
Contrôle si le multiplexeur fonctionne en mode streaming en direct ou en mode basé sur fichier.
**Mode live activé** (liveDisabled = false) :
- Optimisé pour le streaming en direct/temps réel
- Tampon minimal
- Latence plus faible
- Sortie MP4 progressive (peut être lue pendant l'écriture)
- **À utiliser pour** : streaming en direct vers fichier, sortie de streaming réseau
**Mode live désactivé** (liveDisabled = true) :
- Multiplexage standard basé sur fichier
- Peut effectuer une optimisation en plusieurs passes
- Structure MP4 complète écrite à la fin
- Peut nécessiter une navigation dans le fichier de sortie
- **À utiliser pour** : encodage basé sur fichier, scénarios de post-traitement
**Exemple** :
```csharp
// Activer le mode base sur fichier (desactiver les optimisations live)
mp4V10Muxer.SetLiveDisabled(true);
```
## Exemples d'utilisation
### Exemple C# — création MP4 standard
```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;
public class MP4MuxerStandardConfig
{
    public void ConfigureStandardMP4(IBaseFilter mp4Muxer)
    {
        // Interroger l'interface standard du multiplexeur MP4
        var muxerConfig = mp4Muxer as IMP4MuxerConfig;
        if (muxerConfig == null)
        {
            Console.WriteLine("Error: Filter does not support IMP4MuxerConfig");
            return;
        }
        // Configurer pour un encodage standard base fichier
        muxerConfig.put_SingleThread(false);     // Multi-thread pour les performances
        muxerConfig.put_CorrectTiming(true);     // Activer la correction de timing
        Console.WriteLine("MP4 muxer configured for standard file creation");
        // Verifier la configuration
        muxerConfig.get_SingleThread(out bool singleThread);
        muxerConfig.get_CorrectTiming(out bool correctTiming);
        Console.WriteLine($"  Single-threaded: {singleThread}");
        Console.WriteLine($"  Timing correction: {correctTiming}");
    }
}
```
### Exemple C# — sortie déterministe
```csharp
public class MP4MuxerDeterministicConfig
{
    public void ConfigureDeterministicMP4(IBaseFilter mp4Muxer)
    {
        var muxerConfig = mp4Muxer as IMP4MuxerConfig;
        if (muxerConfig == null)
            return;
        // Configurer pour une sortie deterministe et reproductible
        muxerConfig.put_SingleThread(true);      // Thread unique pour la coherence
        muxerConfig.put_CorrectTiming(true);     // Activer la correction de timing
        Console.WriteLine("MP4 muxer configured for deterministic output");
        Console.WriteLine("  Suitable for regression testing and validation");
    }
}
```
### Exemple C# — streaming en direct vers fichier (MP4 V10)
```csharp
public class MP4V10LiveStreamingConfig
{
    public void ConfigureLiveStreaming(IBaseFilter mp4V10Muxer)
    {
        // Interroger l'interface du multiplexeur MP4 v10
        var muxerV10Config = mp4V10Muxer as IMP4V10MuxerConfig;
        if (muxerV10Config == null)
        {
            Console.WriteLine("Error: Filter does not support IMP4V10MuxerConfig");
            return;
        }
        // Configurer pour le streaming en direct vers fichier
        muxerV10Config.SetLiveDisabled(false);   // Activer le mode live
        // Activer l'ajustement de timing pour les sources live
        uint flags = (uint)MP4V10Flags.TimeAdjust;
        muxerV10Config.SetFlags(flags);
        Console.WriteLine("MP4 v10 muxer configured for live streaming");
        // Verifier la configuration
        muxerV10Config.GetFlags(out uint currentFlags);
        Console.WriteLine($"  Flags: 0x{currentFlags:X8}");
        Console.WriteLine($"  Time Adjust: {((currentFlags & (uint)MP4V10Flags.TimeAdjust) != 0)}");
    }
}
```
### Exemple C# — contrôle manuel des horodatages (MP4 V10)
```csharp
public class MP4V10ManualTimestampConfig
{
    public void ConfigureManualTimestamps(IBaseFilter mp4V10Muxer)
    {
        var muxerV10Config = mp4V10Muxer as IMP4V10MuxerConfig;
        if (muxerV10Config == null)
            return;
        // Configurer pour un controle manuel des horodatages
        muxerV10Config.SetLiveDisabled(true);    // Desactiver le mode live
        // Activer le remplacement de temps pour le controle manuel
        uint flags = (uint)MP4V10Flags.TimeOverride;
        muxerV10Config.SetFlags(flags);
        Console.WriteLine("MP4 v10 muxer configured for manual timestamp control");
        Console.WriteLine("  Application must provide accurate timestamps");
    }
}
```
### Exemple C++ — configuration standard
```cpp
#include <dshow.h>
#include <iostream>
#include "IMP4MuxerConfig.h"
void ConfigureMP4Muxer(IBaseFilter* pMp4Muxer)
{
    IMP4MuxerConfig* pMuxerConfig = NULL;
    HRESULT hr = S_OK;
    // Interroger l'interface du multiplexeur MP4
    hr = pMp4Muxer->QueryInterface(IID_IMP4MuxerConfig,
                                   (void**)&pMuxerConfig);
    if (FAILED(hr) || !pMuxerConfig)
    {
        std::cout << "Error: Filter does not support IMP4MuxerConfig" << std::endl;
        return;
    }
    // Configurer le multiplexeur
    pMuxerConfig->put_SingleThread(FALSE);     // Multi-thread
    pMuxerConfig->put_CorrectTiming(TRUE);     // Activer la correction de timing
    // Verifier la configuration
    BOOL singleThread, correctTiming;
    pMuxerConfig->get_SingleThread(&singleThread);
    pMuxerConfig->get_CorrectTiming(&correctTiming);
    std::cout << "MP4 muxer configured:" << std::endl;
    std::cout << "  Single-threaded: " << (singleThread ? "Yes" : "No") << std::endl;
    std::cout << "  Timing correction: " << (correctTiming ? "Yes" : "No") << std::endl;
    pMuxerConfig->Release();
}
```
### Exemple C++ — streaming en direct (MP4 V10)
```cpp
#include "IMP4V10MuxerConfig.h"
void ConfigureMP4V10LiveStreaming(IBaseFilter* pMp4V10Muxer)
{
    IMP4V10MuxerConfig* pMuxerV10Config = NULL;
    HRESULT hr = pMp4V10Muxer->QueryInterface(IID_IMP4V10MuxerConfig,
                                               (void**)&pMuxerV10Config);
    if (SUCCEEDED(hr) && pMuxerV10Config)
    {
        // Configurer pour le streaming en direct
        pMuxerV10Config->SetLiveDisabled(FALSE);     // Activer le mode live
        // Activer l'ajustement de timing
        unsigned long flags = MP4V10_TIME_ADJUST;
        pMuxerV10Config->SetFlags(flags);
        std::cout << "MP4 v10 muxer configured for live streaming" << std::endl;
        pMuxerV10Config->Release();
    }
}
```
### Exemple Delphi — configuration standard
```delphi
uses
  DirectShow9, ActiveX;
procedure ConfigureMP4Muxer(Mp4Muxer: IBaseFilter);
var
  MuxerConfig: IMP4MuxerConfig;
  SingleThread, CorrectTiming: BOOL;
  hr: HRESULT;
begin
  // Interroger l'interface du multiplexeur MP4
  hr := Mp4Muxer.QueryInterface(IID_IMP4MuxerConfig, MuxerConfig);
  if Failed(hr) or (MuxerConfig = nil) then
  begin
    WriteLn('Error: Filter does not support IMP4MuxerConfig');
    Exit;
  end;
  try
    // Configurer le multiplexeur
    MuxerConfig.put_SingleThread(False);     // Multi-thread
    MuxerConfig.put_CorrectTiming(True);     // Activer la correction de timing
    // Verifier la configuration
    MuxerConfig.get_SingleThread(SingleThread);
    MuxerConfig.get_CorrectTiming(CorrectTiming);
    WriteLn('MP4 muxer configured:');
    WriteLn('  Single-threaded: ', SingleThread);
    WriteLn('  Timing correction: ', CorrectTiming);
  finally
    MuxerConfig := nil;
  end;
end;
```
### Exemple Delphi — streaming en direct (MP4 V10)
```delphi
procedure ConfigureMP4V10LiveStreaming(Mp4V10Muxer: IBaseFilter);
var
  MuxerV10Config: IMP4V10MuxerConfig;
  Flags: Cardinal;
begin
  if Succeeded(Mp4V10Muxer.QueryInterface(IID_IMP4V10MuxerConfig, MuxerV10Config)) then
  begin
    try
      // Configurer pour le streaming en direct
      MuxerV10Config.SetLiveDisabled(False);     // Activer le mode live
      // Activer l'ajustement de timing
      Flags := MP4V10_TIME_ADJUST;
      MuxerV10Config.SetFlags(Flags);
      WriteLn('MP4 v10 muxer configured for live streaming');
    finally
      MuxerV10Config := nil;
    end;
  end;
end;
```
## Bonnes pratiques
### Quand utiliser IMP4MuxerConfig
**Utilisez IMP4MuxerConfig quand** :
- Vous avez besoin d'une configuration de base du multiplexeur
- Vous travaillez avec une sortie MP4 standard
- Une simple correction de timing suffit
- Vous n'avez pas besoin de fonctionnalités avancées de streaming en direct
**Configuration typique** :
```csharp
mp4Muxer.put_SingleThread(false);    // Multi-thread pour les performances
mp4Muxer.put_CorrectTiming(true);    // Activer la correction de timing
```
### Quand utiliser IMP4V10MuxerConfig
**Utilisez IMP4V10MuxerConfig quand** :
- Vous avez besoin d'un contrôle avancé du timing
- Vous travaillez avec des scénarios de streaming en direct
- Vous avez besoin du remplacement manuel des horodatages
- Vous avez besoin d'une sortie MP4 progressive
**Configuration de streaming en direct** :
```csharp
mp4V10Muxer.SetLiveDisabled(false);               // Activer le mode live
mp4V10Muxer.SetFlags((uint)MP4V10Flags.TimeAdjust); // Ajustement automatique
```
### Thread unique vs multi-thread
**Utilisez le mode thread unique quand** :
- Vous déboguez le comportement du multiplexeur
- Vous avez besoin d'une sortie déterministe et reproductible
- Vous exécutez des tests automatisés
- Vous résolvez des problèmes de timing
**Utilisez le mode multi-thread quand** :
- Les performances sont critiques
- Vous encodez de la vidéo haute résolution (1080p+)
- Le système dispose de plusieurs cœurs CPU
- Encodage de production standard
### Correction de timing
**Activez toujours la correction de timing quand** :
- Vous travaillez avec des sources en direct (caméras, périphériques de capture)
- Les sources peuvent avoir des incohérences d'horodatage
- Vous combinez plusieurs flux (audio + vidéo)
- Vous avez besoin d'une synchronisation A/V fiable
**Vous pouvez désactiver la correction de timing quand** :
- La source fournit des horodatages garantis précis
- Encodage basé sur fichier avec horodatages prévalidés
- Les performances sont absolument critiques
- Vous utilisez le contrôle manuel des horodatages (indicateur TimeOverride)
### Optimisation du streaming en direct
**Activez le mode live** (SetLiveDisabled = false) **quand** :
- Vous encodez pour un streaming en temps réel
- La sortie doit être lisible pendant l'écriture
- Vous créez des fichiers MP4 progressifs
- Une faible latence est importante
**Désactivez le mode live** (SetLiveDisabled = true) **quand** :
- Vous créez des fichiers pour le post-traitement
- Vous avez besoin d'une structure MP4 complète à la fin
- Vous pouvez effectuer une optimisation en plusieurs passes
- Le fichier de sortie sera lu uniquement après l'achèvement
## Dépannage
### Problèmes de synchronisation audio/vidéo
**Symptômes** : l'audio et la vidéo se désynchronisent dans le temps
**Solutions** :
1. Activez la correction de timing : `put_CorrectTiming(true)`
2. Pour le multiplexeur v10, utilisez l'indicateur TimeAdjust : `SetFlags((uint)MP4V10Flags.TimeAdjust)`
3. Vérifiez que les filtres source fournissent des horodatages précis
4. Vérifiez que les fréquences d'échantillonnage audio et vidéo sont correctes
### Le fichier ne peut pas être lu pendant l'enregistrement
**Symptôme** : le fichier MP4 n'est lisible qu'après la fin de l'encodage
**Cause** : le mode live est désactivé
**Solution** :
- Utilisez l'interface IMP4V10MuxerConfig
- Activez le mode live : `SetLiveDisabled(false)`
- Cela crée des fichiers MP4 progressifs lisibles pendant l'encodage
### Sortie de fichier incohérente
**Symptômes** : la même entrée produit des fichiers de sortie différents
**Cause** : fonctionnement multi-thread avec conditions de concurrence
**Solutions** :
1. Activez le mode thread unique : `put_SingleThread(true)`
2. Activez la correction de timing : `put_CorrectTiming(true)`
3. Utilisez l'indicateur TimeAdjust pour le multiplexeur v10
### Problèmes de performances
**Symptômes** : encodage plus lent que prévu, utilisation CPU élevée
**Causes possibles** :
1. Mode thread unique sur un système multicœur
2. Surcharge excessive de correction de timing
**Solutions** :
- Désactivez le mode thread unique : `put_SingleThread(false)`
- Si les sources ont des horodatages précis, vous pouvez essayer de désactiver la correction de timing
- Assurez-vous que l'encodeur vidéo (et non le multiplexeur) est le goulot d'étranglement
- Envisagez l'encodage matériel (NVENC, QuickSync)
### Fichiers MP4 corrompus
**Symptômes** : le fichier MP4 ne se lit pas ou présente des erreurs
**Causes possibles** :
1. Correction de timing désactivée avec des horodatages médiocres
2. Mauvais paramètre de mode live pour le cas d'usage
3. Multiplexeur arrêté avant la finalisation correcte
**Solutions** :
- Activez la correction de timing pour les sources en direct
- Adaptez le paramètre de mode live au cas d'usage (live vs basé fichier)
- Assurez un arrêt correct du graphe de filtres et la finalisation du flux
- Vérifiez que tous les flux se terminent correctement (envoyez l'événement EC_COMPLETE)
---

## Voir aussi

- [Interface de l'encodeur H.264](h264.md)
- [Interfaces de l'encodeur AAC](aac.md)
- [Référence des multiplexeurs](../muxers-reference.md)
- [Présentation du pack de filtres d'encodage](../index.md)
