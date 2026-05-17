---
title: Filtre DirectShow encodeur AAC — référence d'interface COM
description: Configurez l'encodage AAC dans DirectShow avec les interfaces IMonogramAACEncoder et IVFAACEncoder — débit, profil, fréquence et canaux pour C++/C#.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - Decoding
  - MP4
  - TS
  - AAC
  - MP3
  - C#
primary_api_classes:
  - IMonogramAACEncoder
  - IVFAACEncoder
  - AACConfig
  - VFAACEncoder
  - IBaseFilter

---

# Référence de l'interface de l'encodeur AAC

## Vue d'ensemble

Les filtres DirectShow encodeur AAC (Advanced Audio Coding) fournissent des interfaces pour un encodage audio de haute qualité au format AAC. AAC est le successeur de MP3, offrant une meilleure qualité sonore à débit binaire égal ; c'est le codec audio standard pour les applications MP4, M4A et de streaming.

Deux interfaces d'encodeur AAC sont disponibles :
- **IMonogramAACEncoder** : interface de configuration simple utilisant une structure de configuration unique
- **IVFAACEncoder** : interface complète avec des méthodes de propriété individuelles pour un contrôle fin

## Interface IMonogramAACEncoder

### Vue d'ensemble

L'interface **IMonogramAACEncoder** fournit une approche de configuration simple, basée sur une structure, pour l'encodage AAC. La configuration s'effectue à l'aide de la structure `AACConfig` qui contient tous les paramètres d'encodage essentiels.

**GUID de l'interface** : `{B2DE30C0-1441-4451-A0CE-A914FD561D7F}`

**Hérite de** : `IUnknown`

### Structure AACConfig

```csharp
/// <summary>
/// Structure de configuration de l'encodeur AAC.
/// </summary>
public struct AACConfig
{
    /// <summary>
    /// Version/profil AAC (generalement 2 pour AAC-LC, 4 pour AAC-HE)
    /// </summary>
    public int version;

    /// <summary>
    /// Type d'objet / profil :
    /// 2 = AAC-LC (Low Complexity) - recommande pour la plupart des usages
    /// 5 = AAC-HE (High Efficiency)
    /// 29 = AAC-HEv2 (High Efficiency version 2)
    /// </summary>
    public int object_type;

    /// <summary>
    /// Type de format de sortie (0 = AAC brut, 1 = ADTS)
    /// </summary>
    public int output_type;

    /// <summary>
    /// Debit binaire cible en bits par seconde (par exemple 128000 pour 128 kbps)
    /// </summary>
    public int bitrate;
}
```

### Structure AACInfo

```csharp
/// <summary>
/// Informations d'execution de l'encodeur AAC.
/// </summary>
public struct AACInfo
{
    /// <summary>
    /// Frequence d'echantillonnage d'entree en Hz (par exemple 44100, 48000)
    /// </summary>
    public int samplerate;

    /// <summary>
    /// Nombre de canaux audio (1 = mono, 2 = stereo, 6 = 5.1, etc.)
    /// </summary>
    public int channels;

    /// <summary>
    /// Taille de trame AAC en echantillons (generalement 1024 pour AAC-LC)
    /// </summary>
    public int frame_size;

    /// <summary>
    /// Nombre total de trames encodees
    /// </summary>
    public long frames_done;
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
    /// Structure de configuration de l'encodeur AAC.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AACConfig
    {
        public int version;
        public int object_type;
        public int output_type;
        public int bitrate;
    }

    /// <summary>
    /// Informations d'execution de l'encodeur AAC.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AACInfo
    {
        public int samplerate;
        public int channels;
        public int frame_size;
        public long frames_done;
    }

    /// <summary>
    /// Interface de configuration de l'encodeur AAC Monogram.
    /// Fournit une configuration basee sur une structure pour l'encodage AAC.
    /// </summary>
    [ComImport]
    [Guid("B2DE30C0-1441-4451-A0CE-A914FD561D7F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMonogramAACEncoder
    {
        /// <summary>
        /// Obtient la configuration actuelle de l'encodeur AAC.
        /// </summary>
        /// <param name="config">Reference vers la structure AACConfig recevant les parametres</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetConfig(ref AACConfig config);

        /// <summary>
        /// Definit la configuration de l'encodeur AAC.
        /// </summary>
        /// <param name="config">Reference vers la structure AACConfig contenant les parametres voulus</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetConfig(ref AACConfig config);
    }
}
```

#### Définition C++

```cpp
#include <unknwn.h>

// {B2DE30C0-1441-4451-A0CE-A914FD561D7F}
DEFINE_GUID(IID_IMonogramAACEncoder,
    0xb2de30c0, 0x1441, 0x4451, 0xa0, 0xce, 0xa9, 0x14, 0xfd, 0x56, 0x1d, 0x7f);

/// <summary>
/// Structure de configuration de l'encodeur AAC.
/// </summary>
struct AACConfig
{
    int version;
    int object_type;
    int output_type;
    int bitrate;
};

/// <summary>
/// Informations d'execution de l'encodeur AAC.
/// </summary>
struct AACInfo
{
    int samplerate;
    int channels;
    int frame_size;
    __int64 frames_done;
};

/// <summary>
/// Interface de configuration de l'encodeur AAC Monogram.
/// </summary>
DECLARE_INTERFACE_(IMonogramAACEncoder, IUnknown)
{
    /// <summary>
    /// Obtient la configuration actuelle de l'encodeur AAC.
    /// </summary>
    /// <param name="config">Pointeur vers la structure AACConfig recevant les parametres</param>
    /// <returns>S_OK pour reussite</returns>
    STDMETHOD(GetConfig)(THIS_
        AACConfig* config
        ) PURE;

    /// <summary>
    /// Definit la configuration de l'encodeur AAC.
    /// </summary>
    /// <param name="config">Pointeur vers la structure AACConfig avec les parametres voulus</param>
    /// <returns>S_OK pour reussite</returns>
    STDMETHOD(SetConfig)(THIS_
        const AACConfig* config
        ) PURE;
};
```

#### Définition Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IMonogramAACEncoder: TGUID = '{B2DE30C0-1441-4451-A0CE-A914FD561D7F}';

type
  /// <summary>
  /// Structure de configuration de l'encodeur AAC.
  /// </summary>
  TAACConfig = record
    version: Integer;
    object_type: Integer;
    output_type: Integer;
    bitrate: Integer;
  end;

  /// <summary>
  /// Informations d'execution de l'encodeur AAC.
  /// </summary>
  TAACInfo = record
    samplerate: Integer;
    channels: Integer;
    frame_size: Integer;
    frames_done: Int64;
  end;

  /// <summary>
  /// Interface de configuration de l'encodeur AAC Monogram.
  /// </summary>
  IMonogramAACEncoder = interface(IUnknown)
    ['{B2DE30C0-1441-4451-A0CE-A914FD561D7F}']

    /// <summary>
    /// Obtient la configuration actuelle de l'encodeur AAC.
    /// </summary>
    function GetConfig(var config: TAACConfig): HRESULT; stdcall;

    /// <summary>
    /// Definit la configuration de l'encodeur AAC.
    /// </summary>
    function SetConfig(const config: TAACConfig): HRESULT; stdcall;
  end;
```

---
## Interface IVFAACEncoder
### Vue d'ensemble
L'interface **IVFAACEncoder** fournit une configuration complète basée sur les propriétés pour l'encodage AAC, avec des méthodes individuelles getter/setter pour chaque paramètre. Cette interface offre un contrôle plus fin et est plus facile à utiliser pour des modifications incrémentales.
**GUID de l'interface** : `{0BEF7533-39E6-42a5-863F-E087FAB5D84F}`
**Hérite de** : `IUnknown`
### Définitions de l'interface
#### Définition C#
```csharp
using System;
using System.Runtime.InteropServices;
namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interface de configuration de l'encodeur AAC VisioForge.
    /// Fournit un controle complet base sur les proprietes des parametres d'encodage AAC.
    /// </summary>
    [ComImport]
    [Guid("0BEF7533-39E6-42a5-863F-E087FAB5D84F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFAACEncoder
    {
        /// <summary>
        /// Force une frequence d'echantillonnage d'entree specifique. Definir a 0 pour accepter toute frequence.
        /// </summary>
        /// <param name="ulSampleRate">Frequence d'echantillonnage en Hz (par exemple 44100, 48000). 0 = toute frequence</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetInputSampleRate(uint ulSampleRate);
        /// <summary>
        /// Obtient la frequence d'echantillonnage d'entree configuree.
        /// </summary>
        /// <param name="pulSampleRate">Recoit la frequence en Hz. 0 si non fixee</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetInputSampleRate(out uint pulSampleRate);
        /// <summary>
        /// Definit le nombre de canaux d'entree.
        /// </summary>
        /// <param name="nChannels">Nombre de canaux (1=mono, 2=stereo, 6=5.1, etc.)</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetInputChannels(short nChannels);
        /// <summary>
        /// Obtient le nombre de canaux d'entree.
        /// </summary>
        /// <param name="pnChannels">Recoit le nombre de canaux</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetInputChannels(out short pnChannels);
        /// <summary>
        /// Definit le debit binaire cible. Definir a -1 pour utiliser le debit maximal.
        /// </summary>
        /// <param name="ulBitRate">Debit binaire en bits par seconde (par exemple 128000). -1 = maximum</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetBitRate(uint ulBitRate);
        /// <summary>
        /// Obtient le debit binaire configure.
        /// </summary>
        /// <param name="pulBitRate">Recoit le debit binaire en bps. -1 si defini au maximum</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetBitRate(out uint pulBitRate);
        /// <summary>
        /// Definit le type de profil AAC.
        /// </summary>
        /// <param name="uProfile">Profil : 2=AAC-LC, 5=AAC-HE, 29=AAC-HEv2</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetProfile(uint uProfile);
        /// <summary>
        /// Obtient le profil AAC actuel.
        /// </summary>
        /// <param name="puProfile">Recoit le type de profil</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetProfile(out uint puProfile);
        /// <summary>
        /// Definit le format de sortie.
        /// </summary>
        /// <param name="uFormat">Format : 0=AAC brut, 1=ADTS</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetOutputFormat(uint uFormat);
        /// <summary>
        /// Obtient le format de sortie.
        /// </summary>
        /// <param name="puFormat">Recoit le format de sortie</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetOutputFormat(out uint puFormat);
        /// <summary>
        /// Definit la valeur de decalage temporel pour l'ajustement des horodatages.
        /// </summary>
        /// <param name="timeShift">Decalage temporel en millisecondes</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetTimeShift(int timeShift);
        /// <summary>
        /// Obtient la valeur de decalage temporel.
        /// </summary>
        /// <param name="ptimeShift">Recoit le decalage temporel en millisecondes</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetTimeShift(out int ptimeShift);
        /// <summary>
        /// Active ou desactive le canal Low Frequency Effects (LFE).
        /// </summary>
        /// <param name="lfe">1 pour activer LFE, 0 pour desactiver</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetLFE(uint lfe);
        /// <summary>
        /// Obtient l'etat du canal LFE.
        /// </summary>
        /// <param name="p">Recoit l'etat LFE (1=active, 0=desactive)</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetLFE(out uint p);
        /// <summary>
        /// Active ou desactive le Temporal Noise Shaping (TNS).
        /// TNS ameliore l'encodage des sons transitoires.
        /// </summary>
        /// <param name="tns">1 pour activer TNS, 0 pour desactiver</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetTNS(uint tns);
        /// <summary>
        /// Obtient l'etat TNS.
        /// </summary>
        /// <param name="p">Recoit l'etat TNS (1=active, 0=desactive)</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetTNS(out uint p);
        /// <summary>
        /// Active ou desactive le codage stereo Mid-Side.
        /// Peut ameliorer la compression pour l'audio stereo.
        /// </summary>
        /// <param name="v">1 pour activer le codage mid-side, 0 pour desactiver</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int SetMidSide(uint v);
        /// <summary>
        /// Obtient l'etat du codage mid-side.
        /// </summary>
        /// <param name="p">Recoit l'etat mid-side (1=active, 0=desactive)</param>
        /// <returns>HRESULT (0 pour reussite)</returns>
        [PreserveSig]
        int GetMidSide(out uint p);
    }
}
```
#### Définition C++
```cpp
#include <unknwn.h>
// {0BEF7533-39E6-42a5-863F-E087FAB5D84F}
DEFINE_GUID(IID_IVFAACEncoder,
    0x0bef7533, 0x39e6, 0x42a5, 0x86, 0x3f, 0xe0, 0x87, 0xfa, 0xb5, 0xd8, 0x4f);
/// <summary>
/// Interface de configuration de l'encodeur AAC VisioForge.
/// </summary>
DECLARE_INTERFACE_(IVFAACEncoder, IUnknown)
{
    STDMETHOD(SetInputSampleRate)(THIS_
        unsigned long ulSampleRate
        ) PURE;
    STDMETHOD(GetInputSampleRate)(THIS_
        unsigned long* pulSampleRate
        ) PURE;
    STDMETHOD(SetInputChannels)(THIS_
        short nChannels
        ) PURE;
    STDMETHOD(GetInputChannels)(THIS_
        short* pnChannels
        ) PURE;
    STDMETHOD(SetBitRate)(THIS_
        unsigned long ulBitRate
        ) PURE;
    STDMETHOD(GetBitRate)(THIS_
        unsigned long* pulBitRate
        ) PURE;
    STDMETHOD(SetProfile)(THIS_
        unsigned long uProfile
        ) PURE;
    STDMETHOD(GetProfile)(THIS_
        unsigned long* puProfile
        ) PURE;
    STDMETHOD(SetOutputFormat)(THIS_
        unsigned long uFormat
        ) PURE;
    STDMETHOD(GetOutputFormat)(THIS_
        unsigned long* puFormat
        ) PURE;
    STDMETHOD(SetTimeShift)(THIS_
        int timeShift
        ) PURE;
    STDMETHOD(GetTimeShift)(THIS_
        int* ptimeShift
        ) PURE;
    STDMETHOD(SetLFE)(THIS_
        unsigned long lfe
        ) PURE;
    STDMETHOD(GetLFE)(THIS_
        unsigned long* p
        ) PURE;
    STDMETHOD(SetTNS)(THIS_
        unsigned long tns
        ) PURE;
    STDMETHOD(GetTNS)(THIS_
        unsigned long* p
        ) PURE;
    STDMETHOD(SetMidSide)(THIS_
        unsigned long v
        ) PURE;
    STDMETHOD(GetMidSide)(THIS_
        unsigned long* p
        ) PURE;
};
```
#### Définition Delphi
```delphi
uses
  ActiveX, ComObj;
const
  IID_IVFAACEncoder: TGUID = '{0BEF7533-39E6-42a5-863F-E087FAB5D84F}';
type
  /// <summary>
  /// Interface de configuration de l'encodeur AAC VisioForge.
  /// </summary>
  IVFAACEncoder = interface(IUnknown)
    ['{0BEF7533-39E6-42a5-863F-E087FAB5D84F}']
    function SetInputSampleRate(ulSampleRate: Cardinal): HRESULT; stdcall;
    function GetInputSampleRate(out pulSampleRate: Cardinal): HRESULT; stdcall;
    function SetInputChannels(nChannels: SmallInt): HRESULT; stdcall;
    function GetInputChannels(out pnChannels: SmallInt): HRESULT; stdcall;
    function SetBitRate(ulBitRate: Cardinal): HRESULT; stdcall;
    function GetBitRate(out pulBitRate: Cardinal): HRESULT; stdcall;
    function SetProfile(uProfile: Cardinal): HRESULT; stdcall;
    function GetProfile(out puProfile: Cardinal): HRESULT; stdcall;
    function SetOutputFormat(uFormat: Cardinal): HRESULT; stdcall;
    function GetOutputFormat(out puFormat: Cardinal): HRESULT; stdcall;
    function SetTimeShift(timeShift: Integer): HRESULT; stdcall;
    function GetTimeShift(out ptimeShift: Integer): HRESULT; stdcall;
    function SetLFE(lfe: Cardinal): HRESULT; stdcall;
    function GetLFE(out p: Cardinal): HRESULT; stdcall;
    function SetTNS(tns: Cardinal): HRESULT; stdcall;
    function GetTNS(out p: Cardinal): HRESULT; stdcall;
    function SetMidSide(v: Cardinal): HRESULT; stdcall;
    function GetMidSide(out p: Cardinal): HRESULT; stdcall;
  end;
```
## Profils et configuration AAC
### Profils AAC
**AAC-LC (Low Complexity) — profil 2** (recommandé) :
- Meilleur rapport qualité/débit binaire
- Complexité de calcul la plus faible
- Prise en charge décodeur universelle
- À utiliser pour : musique, podcasts, bandes-son vidéo
- Plage de débit : 64-320 kbps
**AAC-HE (High Efficiency) — profil 5** :
- Optimisé pour les débits faibles
- Utilise la Spectral Band Replication (SBR)
- Meilleure qualité qu'AAC-LC à faible débit (<= 64 kbps)
- À utiliser pour : streaming, voix, applications à bas débit
- Plage de débit : 32-80 kbps
**AAC-HEv2 (High Efficiency version 2) — profil 29** :
- Encore plus optimisé pour les très bas débits
- Utilise le Parametric Stereo (PS) en plus de SBR
- Idéal pour mono/stéréo à débit extrêmement faible
- À utiliser pour : streaming vocal, très faible bande passante
- Plage de débit : 16-40 kbps
### Formats de sortie
**AAC brut (format 0)** :
- Flux AAC pur sans conteneur
- Nécessite un conteneur externe (MP4, M4A, MKV)
- À utiliser pour : multiplexage dans des fichiers MP4/M4A
- Taille de sortie la plus petite
**ADTS (Audio Data Transport Stream) — format 1** :
- AAC avec en-têtes de trame
- Autonome, peut être lu directement
- Légèrement plus volumineux qu'AAC brut
- À utiliser pour : fichiers AAC autonomes, streaming
- Meilleure résistance aux erreurs
### Débits binaires recommandés
| Cas d'usage | Canaux | Profil | Débit | Notes |
|----------|----------|---------|---------|-------|
| Voix/podcast (mono) | 1 | AAC-LC | 64-96 kbps | Parole claire |
| Voix/podcast (stéréo) | 2 | AAC-LC | 96-128 kbps | Parole haute qualité |
| Musique (stéréo) standard | 2 | AAC-LC | 128-192 kbps | Bonne qualité |
| Musique (stéréo) haute qualité | 2 | AAC-LC | 256-320 kbps | Excellente qualité |
| Streaming bas débit | 2 | AAC-HE | 48-64 kbps | Qualité acceptable |
| Très bas débit | 1-2 | AAC-HEv2 | 24-40 kbps | Qualité de base |
| Surround 5.1 | 6 | AAC-LC | 384-512 kbps | Qualité cinéma |
## Exemples d'utilisation
### Exemple C# — IMonogramAACEncoder (musique haute qualité)
```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;
public class MonogramAACHighQuality
{
    public void ConfigureHighQualityMusic(IBaseFilter audioEncoder)
    {
        // Interroger l'interface de l'encodeur AAC Monogram
        var aacEncoder = audioEncoder as IMonogramAACEncoder;
        if (aacEncoder == null)
        {
            Console.WriteLine("Error: Filter does not support IMonogramAACEncoder");
            return;
        }
        // Configurer l'encodage musique stereo haute qualite
        var config = new AACConfig
        {
            version = 2,            // AAC version 2
            object_type = 2,        // Profil AAC-LC
            output_type = 0,        // AAC brut (pour multiplexage MP4)
            bitrate = 192000        // 192 kbps
        };
        int hr = aacEncoder.SetConfig(ref config);
        if (hr == 0)
        {
            Console.WriteLine("AAC encoder configured for high quality music:");
            Console.WriteLine("  Profile: AAC-LC");
            Console.WriteLine("  Bitrate: 192 kbps");
            Console.WriteLine("  Output: Raw AAC for MP4 container");
        }
        else
        {
            Console.WriteLine($"Error configuring AAC encoder: 0x{hr:X8}");
        }
    }
}
```
### Exemple C# — IVFAACEncoder (configuration complète)
```csharp
public class VFAACHighQualityMusic
{
    public void ConfigureComprehensive(IBaseFilter audioEncoder)
    {
        // Interroger l'interface de l'encodeur AAC VisioForge
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
        {
            Console.WriteLine("Error: Filter does not support IVFAACEncoder");
            return;
        }
        // Configurer l'encodage musique stereo complet
        vfAacEncoder.SetInputSampleRate(48000);     // 48 kHz
        vfAacEncoder.SetInputChannels(2);            // Stereo
        vfAacEncoder.SetBitRate(256000);            // 256 kbps
        vfAacEncoder.SetProfile(2);                 // AAC-LC
        vfAacEncoder.SetOutputFormat(0);            // AAC brut
        vfAacEncoder.SetTNS(1);                     // Activer TNS
        vfAacEncoder.SetMidSide(1);                 // Activer le codage mid-side
        vfAacEncoder.SetLFE(0);                     // Pas de LFE (stereo uniquement)
        vfAacEncoder.SetTimeShift(0);               // Aucun decalage temporel
        Console.WriteLine("VisioForge AAC encoder configured:");
        // Verifier la configuration
        vfAacEncoder.GetBitRate(out uint bitrate);
        vfAacEncoder.GetProfile(out uint profile);
        vfAacEncoder.GetInputChannels(out short channels);
        Console.WriteLine($"  Bitrate: {bitrate / 1000} kbps");
        Console.WriteLine($"  Profile: {(profile == 2 ? "AAC-LC" : profile.ToString())}");
        Console.WriteLine($"  Channels: {channels}");
    }
}
```
### Exemple C# — streaming bas débit (AAC-HE)
```csharp
public class VFAACLowBitrateStreaming
{
    public void ConfigureLowBitrate(IBaseFilter audioEncoder)
    {
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
            return;
        // Configurer pour le streaming a bas debit
        vfAacEncoder.SetInputSampleRate(44100);     // 44.1 kHz
        vfAacEncoder.SetInputChannels(2);            // Stereo
        vfAacEncoder.SetBitRate(64000);             // 64 kbps
        vfAacEncoder.SetProfile(5);                 // AAC-HE (High Efficiency)
        vfAacEncoder.SetOutputFormat(1);            // ADTS pour streaming
        vfAacEncoder.SetTNS(1);                     // Activer TNS
        vfAacEncoder.SetMidSide(1);                 // Activer mid-side
        vfAacEncoder.SetLFE(0);                     // Pas de LFE
        Console.WriteLine("AAC-HE configured for low-bitrate streaming");
        Console.WriteLine("  64 kbps stereo with ADTS output");
    }
}
```
### Exemple C# — encodage voix/podcast
```csharp
public class VFAACVoicePodcast
{
    public void ConfigureVoicePodcast(IBaseFilter audioEncoder)
    {
        var vfAacEncoder = audioEncoder as IVFAACEncoder;
        if (vfAacEncoder == null)
            return;
        // Configurer pour voix/podcast (mono)
        vfAacEncoder.SetInputSampleRate(44100);     // 44.1 kHz
        vfAacEncoder.SetInputChannels(1);            // Mono
        vfAacEncoder.SetBitRate(80000);             // 80 kbps
        vfAacEncoder.SetProfile(2);                 // AAC-LC
        vfAacEncoder.SetOutputFormat(0);            // AAC brut
        vfAacEncoder.SetTNS(1);                     // Activer TNS pour la parole
        vfAacEncoder.SetMidSide(0);                 // N/A pour mono
        vfAacEncoder.SetLFE(0);                     // Pas de LFE
        Console.WriteLine("AAC configured for voice/podcast");
        Console.WriteLine("  80 kbps mono AAC-LC");
    }
}
```
### Exemple C++ — IMonogramAACEncoder
```cpp
#include <dshow.h>
#include <iostream>
#include "IMonogramAACEncoder.h"
void ConfigureMonogramAAC(IBaseFilter* pAudioEncoder)
{
    IMonogramAACEncoder* pAACEncoder = NULL;
    HRESULT hr = S_OK;
    // Interroger l'interface de l'encodeur AAC Monogram
    hr = pAudioEncoder->QueryInterface(IID_IMonogramAACEncoder,
                                       (void**)&pAACEncoder);
    if (FAILED(hr) || !pAACEncoder)
    {
        std::cout << "Error: Filter does not support IMonogramAACEncoder" << std::endl;
        return;
    }
    // Configurer l'encodage musique haute qualite
    AACConfig config;
    config.version = 2;         // AAC version 2
    config.object_type = 2;     // AAC-LC
    config.output_type = 0;     // AAC brut
    config.bitrate = 192000;    // 192 kbps
    hr = pAACEncoder->SetConfig(&config);
    if (SUCCEEDED(hr))
    {
        std::cout << "AAC encoder configured for high quality music" << std::endl;
        std::cout << "  Profile: AAC-LC" << std::endl;
        std::cout << "  Bitrate: 192 kbps" << std::endl;
    }
    pAACEncoder->Release();
}
```
### Exemple C++ — IVFAACEncoder
```cpp
#include "IVFAACEncoder.h"
void ConfigureVFAAC(IBaseFilter* pAudioEncoder)
{
    IVFAACEncoder* pVFAACEncoder = NULL;
    HRESULT hr = pAudioEncoder->QueryInterface(IID_IVFAACEncoder,
                                               (void**)&pVFAACEncoder);
    if (SUCCEEDED(hr) && pVFAACEncoder)
    {
        // Configurer l'encodage stereo complet
        pVFAACEncoder->SetInputSampleRate(48000);   // 48 kHz
        pVFAACEncoder->SetInputChannels(2);          // Stereo
        pVFAACEncoder->SetBitRate(256000);          // 256 kbps
        pVFAACEncoder->SetProfile(2);               // AAC-LC
        pVFAACEncoder->SetOutputFormat(0);          // AAC brut
        pVFAACEncoder->SetTNS(1);                   // Activer TNS
        pVFAACEncoder->SetMidSide(1);               // Activer mid-side
        pVFAACEncoder->SetLFE(0);                   // Pas de LFE
        std::cout << "VisioForge AAC encoder configured" << std::endl;
        pVFAACEncoder->Release();
    }
}
```
### Exemple Delphi — IMonogramAACEncoder
```delphi
uses
  DirectShow9, ActiveX;
procedure ConfigureMonogramAAC(AudioEncoder: IBaseFilter);
var
  AACEncoder: IMonogramAACEncoder;
  Config: TAACConfig;
  hr: HRESULT;
begin
  // Interroger l'interface de l'encodeur AAC Monogram
  hr := AudioEncoder.QueryInterface(IID_IMonogramAACEncoder, AACEncoder);
  if Failed(hr) or (AACEncoder = nil) then
  begin
    WriteLn('Error: Filter does not support IMonogramAACEncoder');
    Exit;
  end;
  try
    // Configurer l'encodage musique haute qualite
    Config.version := 2;         // AAC version 2
    Config.object_type := 2;     // AAC-LC
    Config.output_type := 0;     // AAC brut
    Config.bitrate := 192000;    // 192 kbps
    hr := AACEncoder.SetConfig(Config);
    if Succeeded(hr) then
    begin
      WriteLn('AAC encoder configured for high quality music');
      WriteLn('  Profile: AAC-LC');
      WriteLn('  Bitrate: 192 kbps');
    end;
  finally
    AACEncoder := nil;
  end;
end;
```
### Exemple Delphi — IVFAACEncoder
```delphi
procedure ConfigureVFAAC(AudioEncoder: IBaseFilter);
var
  VFAACEncoder: IVFAACEncoder;
begin
  if Succeeded(AudioEncoder.QueryInterface(IID_IVFAACEncoder, VFAACEncoder)) then
  begin
    try
      // Configurer l'encodage stereo complet
      VFAACEncoder.SetInputSampleRate(48000);   // 48 kHz
      VFAACEncoder.SetInputChannels(2);          // Stereo
      VFAACEncoder.SetBitRate(256000);          // 256 kbps
      VFAACEncoder.SetProfile(2);               // AAC-LC
      VFAACEncoder.SetOutputFormat(0);          // AAC brut
      VFAACEncoder.SetTNS(1);                   // Activer TNS
      VFAACEncoder.SetMidSide(1);               // Activer mid-side
      VFAACEncoder.SetLFE(0);                   // Pas de LFE
      WriteLn('VisioForge AAC encoder configured');
    finally
      VFAACEncoder := nil;
    end;
  end;
end;
```
## Bonnes pratiques
### Choix du profil
**Utilisez AAC-LC (profil 2) lorsque** :
- Vous encodez de la musique ou de l'audio haute qualité
- Le débit binaire est >= 96 kbps
- Une compatibilité maximale du décodeur est requise
- **Recommandé pour la plupart des scénarios**
**Utilisez AAC-HE (profil 5) lorsque** :
- Vous avez des contraintes de débit (32-80 kbps)
- Vous streamez sur une bande passante limitée
- Le contenu vocal est acceptable à une qualité moindre
- Applications de streaming mobile/web
**Utilisez AAC-HEv2 (profil 29) lorsque** :
- La bande passante est extrêmement limitée (< 40 kbps)
- Le contenu est uniquement vocal
- Mono ou stéréo uniquement (pas de multicanal)
### Recommandations de débit binaire
**Parole/podcast mono** :
- Minimum : 48-64 kbps (AAC-LC)
- Recommandé : 80-96 kbps (AAC-LC)
- Haute qualité : 128 kbps (AAC-LC)
**Musique stéréo** :
- Minimum : 96-128 kbps (AAC-LC)
- Recommandé : 192-256 kbps (AAC-LC)
- Haute qualité : 256-320 kbps (AAC-LC)
**Applications de streaming** :
- Faible bande passante : 48-64 kbps (AAC-HE, stéréo)
- Bande passante standard : 96-128 kbps (AAC-LC, stéréo)
- Haute bande passante : 192-256 kbps (AAC-LC, stéréo)
### Choix du format de sortie
**Utilisez AAC brut (format 0) lorsque** :
- Vous multiplexez dans des conteneurs MP4, M4A ou MKV
- Le conteneur fournit le découpage en trames et la synchronisation
- **Recommandé pour la plupart des applications vidéo/multimédia**
**Utilisez ADTS (format 1) lorsque** :
- Vous créez des fichiers .aac autonomes
- Vous streamez sans conteneur
- Vous avez besoin d'une meilleure récupération d'erreurs
- Vous testez/déboguez l'audio indépendamment
### Indicateurs de fonctionnalités
**Temporal Noise Shaping (TNS)** :
- **Activez** pour tous les scénarios d'encodage
- Améliore la réponse transitoire
- Meilleure qualité pour les sons percussifs
- Surcharge de calcul minimale
**Codage stéréo mid-side** :
- **Activez** pour l'encodage musique stéréo
- Améliore l'efficacité de compression
- Meilleure image stéréo
- Aucun bénéfice pour mono ou stéréo non corrélé
**Low Frequency Effects (LFE)** :
- **Activez** uniquement pour le son surround 5.1/7.1
- Canal subwoofer dédié (.1)
- Désactivez pour stéréo/mono
## Dépannage
### Qualité audio faible
**Symptômes** : son étouffé, artefacts, manque de clarté
**Causes possibles** :
1. Débit binaire trop faible pour le contenu
2. Mauvais profil pour le débit
3. TNS désactivé
**Solutions** :
- Augmentez le débit aux niveaux recommandés (voir les tableaux ci-dessus)
- Pour les bas débits (<= 80 kbps), utilisez AAC-HE au lieu d'AAC-LC
- Activez TNS : `SetTNS(1)`
- Pour la musique, assurez-vous que le débit est >= 128 kbps avec AAC-LC
### Échecs d'initialisation de l'encodeur
**Symptômes** : SetConfig ou les méthodes Set renvoient des codes d'erreur
**Causes possibles** :
1. Fréquence d'échantillonnage non prise en charge
2. Débit invalide pour le profil
3. Configuration de canaux incompatible
**Solutions** :
- Utilisez des fréquences standard : 44100, 48000 Hz
- Vérifiez que le débit convient au profil
- Vérifiez que le nombre de canaux correspond à l'audio source
- Pour AAC-HE, gardez un débit <= 128 kbps
### Le fichier ne se lit pas
**Symptômes** : le fichier AAC ne se lit pas dans les lecteurs multimédias
**Causes possibles** :
1. AAC brut sans conteneur
2. Profil non pris en charge
3. Flux corrompu
**Solutions** :
- Utilisez le format de sortie ADTS (`SetOutputFormat(1)`) pour les fichiers autonomes
- Utilisez AAC brut (`SetOutputFormat(0)`) uniquement avec un conteneur MP4/M4A
- Vérifiez que le lecteur prend en charge le profil AAC (HE/HEv2 peut ne pas être pris en charge sur d'anciens lecteurs)
- Assurez-vous d'une finalisation correcte du flux dans le graphe de filtres
### Problèmes de compatibilité
**Symptômes** : AAC se lit sur certains appareils mais pas sur d'autres
**Causes possibles** :
1. Profil avancé non pris en charge (AAC-HE/HEv2)
2. Configuration non standard
**Solutions** :
- Utilisez AAC-LC (profil 2) pour une compatibilité maximale
- Utilisez des fréquences d'échantillonnage standard (44100 ou 48000 Hz)
- Gardez les débits dans les plages recommandées
- Évitez les très bas débits (< 64 kbps) pour AAC-LC
---

## Voir aussi

- [Interface de l'encodeur MP3 LAME](lame.md)
- [Interface de l'encodeur FLAC](flac.md)
- [Référence des codecs audio](../codecs-reference.md)
- [Interface du multiplexeur MP4](mp4-muxer.md)
- [Présentation du pack de filtres d'encodage](../index.md)
