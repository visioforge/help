---
title: Filtre DirectShow encodeur FFmpeg — sortie MPEG, FLV, DVD
description: Interface COM IVFFFMPEGEncoder pour encoder vers MPEG-1, MPEG-2, FLV, VCD, SVCD, DVD et Transport Stream dans DirectShow. Codecs audio/vidéo C++/C#.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - MP4
  - FLV
  - TS
  - H.264
  - MPEG-2
  - AC-3
  - C#
primary_api_classes:
  - IVFFFMPEGEncoder
  - FFMPEGOutputSettings
  - IBaseFilter
  - CVFOutputSettings
  - TFFMPEGOutputSettings

---

# Référence de l'interface de l'encodeur FFMPEG

## Vue d'ensemble

L'interface **IVFFFMPEGEncoder** fournit une configuration complète pour l'encodage vidéo et audio vers divers formats à l'aide de la bibliothèque FFMPEG. Ce puissant encodeur prend en charge plusieurs formats de sortie, dont Flash Video (FLV), MPEG-1, MPEG-2, VCD, SVCD, DVD et MPEG-2 Transport Stream.

L'encodeur utilise une approche de configuration basée sur une structure où tous les paramètres d'encodage sont définis en une seule fois, offrant une interface simple mais complète pour l'encodage vidéo professionnel vers les formats hérités et de streaming.

**GUID de l'interface** : `{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}`

**CLSID du filtre** : `{554AB365-B293-4C1D-9245-E8DB01F027F7}`

**Hérite de** : `IUnknown`

## GUID du filtre et de l'interface

```csharp
// CLSID du filtre encodeur FFMPEG
public static readonly Guid CLSID_FFMPEGEncoder =
    new Guid("554AB365-B293-4C1D-9245-E8DB01F027F7");

// IID de l'interface IVFFFMPEGEncoder
public static readonly Guid IID_IVFFFMPEGEncoder =
    new Guid("17B8FF7D-A67F-45CE-B425-0E4F607D8C60");
```

## Formats de sortie

### Énumération VFFFMPEGDLLOutputFormat

```csharp
/// <summary>
/// Options de format de sortie de l'encodeur FFMPEG.
/// </summary>
public enum VFFFMPEGDLLOutputFormat
{
    /// <summary>
    /// Flash Video (.flv) - format de streaming web
    /// </summary>
    FLV = 0,

    /// <summary>
    /// MPEG-1 (.mpg) - video MPEG-1 standard
    /// </summary>
    MPEG1 = 1,

    /// <summary>
    /// MPEG-1 VCD - format conforme Video CD
    /// Resolution : 352x240 (NTSC) ou 352x288 (PAL)
    /// Debit : 1150 kbps
    /// </summary>
    MPEG1VCD = 2,

    /// <summary>
    /// MPEG-2 (.mpg) - video MPEG-2 standard
    /// </summary>
    MPEG2 = 3,

    /// <summary>
    /// MPEG-2 Transport Stream (.ts) - diffusion et streaming
    /// </summary>
    MPEG2TS = 4,

    /// <summary>
    /// MPEG-2 SVCD - format conforme Super Video CD
    /// Resolution : 480x480 (NTSC) ou 480x576 (PAL)
    /// Debit : 2500 kbps
    /// </summary>
    MPEG2SVCD = 5,

    /// <summary>
    /// MPEG-2 DVD - format conforme DVD-Video
    /// Resolution : 720x480 (NTSC) ou 720x576 (PAL)
    /// Debit : jusqu'a 9800 kbps
    /// </summary>
    MPEG2DVD = 6
}
```

### Standards de systèmes TV

```csharp
/// <summary>
/// Normes de systeme television pour l'encodage video.
/// </summary>
public enum VFFFMPEGDLLTVSystem
{
    /// <summary>
    /// Pas de systeme TV specifique / detection automatique
    /// </summary>
    None = 0,

    /// <summary>
    /// PAL (Phase Alternating Line)
    /// 25 fps, 576 lignes
    /// Utilise en : Europe, Asie, Australie, Afrique
    /// </summary>
    PAL = 1,

    /// <summary>
    /// NTSC (National Television System Committee)
    /// 29,97 fps, 480 lignes
    /// Utilise en : Amerique du Nord, Japon, Coree du Sud
    /// </summary>
    NTSC = 2,

    /// <summary>
    /// Norme cinema
    /// 24 fps
    /// Utilise pour : cinema, transferts film
    /// </summary>
    Film = 3
}
```

## Structure FFMPEGOutputSettings

```csharp
/// <summary>
/// Structure de configuration complete pour l'encodeur FFMPEG.
/// Contient tous les parametres audio, video et de format de sortie.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FFMPEGOutputSettings
{
    /// <summary>
    /// Nom du fichier de sortie avec chemin.
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Filename;

    /// <summary>
    /// True si un flux audio est inclus dans la sortie.
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool AudioAvailable;

    /// <summary>
    /// Debit binaire audio en bits par seconde (par exemple 128000 pour 128 kbps).
    /// </summary>
    public int AudioBitrate;

    /// <summary>
    /// Frequence d'echantillonnage audio en Hz (par exemple 44100, 48000).
    /// </summary>
    public int AudioSamplerate;

    /// <summary>
    /// Nombre de canaux audio (1 = mono, 2 = stereo).
    /// </summary>
    public int AudioChannels;

    /// <summary>
    /// Largeur de la trame video en pixels.
    /// </summary>
    public int VideoWidth;

    /// <summary>
    /// Hauteur de la trame video en pixels.
    /// </summary>
    public int VideoHeight;

    /// <summary>
    /// Largeur du rapport d'aspect d'affichage (par exemple 16 pour 16:9).
    /// </summary>
    public int AspectRatioW;

    /// <summary>
    /// Hauteur du rapport d'aspect d'affichage (par exemple 9 pour 16:9).
    /// </summary>
    public int AspectRatioH;

    /// <summary>
    /// Debit binaire video en bits par seconde (par exemple 5000000 pour 5 Mbps).
    /// </summary>
    public int VideoBitrate;

    /// <summary>
    /// Debit video maximal pour l'encodage VBR (bits par seconde).
    /// </summary>
    public int VideoMaxRate;

    /// <summary>
    /// Debit video minimal pour l'encodage VBR (bits par seconde).
    /// </summary>
    public int VideoMinRate;

    /// <summary>
    /// Taille du tampon video en bits (affecte la latence et la fluidite).
    /// </summary>
    public int VideoBufferSize;

    /// <summary>
    /// True pour activer l'encodage entrelace (pour la TV diffusee).
    /// </summary>
    [MarshalAs(UnmanagedType.Bool)]
    public bool Interlace;

    /// <summary>
    /// Taille du GOP (Group of Pictures) - intervalle d'image-cle.
    /// Typique : 12-15 pour MPEG-2, 30-60 pour video web.
    /// </summary>
    public int VideoGopSize;

    /// <summary>
    /// Norme du systeme TV (PAL, NTSC, Film ou None).
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public VFFFMPEGDLLTVSystem TVSystem;

    /// <summary>
    /// Format de sortie (FLV, MPEG-1, MPEG-2, etc.).
    /// </summary>
    [MarshalAs(UnmanagedType.I4)]
    public VFFFMPEGDLLOutputFormat OutputFormat;
}
```

## Définitions de l'interface

### Définition C#

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    /// <summary>
    /// Interface de configuration de l'encodeur FFMPEG.
    /// Fournit un encodage complet vers plusieurs formats via la bibliotheque FFMPEG.
    /// </summary>
    [ComImport]
    [Guid("17B8FF7D-A67F-45CE-B425-0E4F607D8C60")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVFFFMPEGEncoder
    {
        /// <summary>
        /// Definit la configuration complete de l'encodeur FFMPEG.
        /// Tous les parametres d'encodage doivent etre definis en une fois via cette structure.
        /// </summary>
        /// <param name="settings">Structure complete des parametres de l'encodeur</param>
        [PreserveSig]
        void set_settings([In] FFMPEGOutputSettings settings);
    }
}
```

### Définition C++

```cpp
#include <unknwn.h>

// {17B8FF7D-A67F-45CE-B425-0E4F607D8C60}
DEFINE_GUID(IID_IVFFFMPEGEncoder,
    0x17b8ff7d, 0xa67f, 0x45ce, 0xb4, 0x25, 0xe, 0x4f, 0x60, 0x7d, 0x8c, 0x60);

// {554AB365-B293-4C1D-9245-E8DB01F027F7}
DEFINE_GUID(CLSID_FFMPEGEncoder,
    0x554ab365, 0xb293, 0x4c1d, 0x92, 0x45, 0xe8, 0xdb, 0x01, 0xf0, 0x27, 0xf7);

/// <summary>
/// Enumeration des formats de sortie.
/// </summary>
enum FFOutputFormat
{
    of_FLV = 0,
    of_MPEG1 = 1,
    of_MPEG1VCD = 2,
    of_MPEG2 = 3,
    of_MPEG2TS = 4,
    of_MPEG2SVCD = 5,
    of_MPEG2DVD = 6
};

/// <summary>
/// Enumeration des systemes TV.
/// </summary>
enum video_tv_system_t
{
    video_norm_unknown = 0,
    video_norm_pal = 1,
    video_norm_ntsc = 2,
    video_norm_film = 3
};

/// <summary>
/// Structure des parametres de sortie de l'encodeur FFMPEG.
/// </summary>
struct CVFOutputSettings
{
    wchar_t* filename;

    BOOL audioAvailable;
    int audioBitrate;
    int audioSamplerate;
    int audioChannels;

    int videoWidth;
    int videoHeight;
    int aspectRatioW;
    int aspectRatioH;
    int videoBitrate;
    int videoMaxRate;
    int videoMinRate;
    int videoBufferSize;
    BOOL interlace;
    int videoGopSize;
    int tvSystem;

    int outputFormat;
};

/// <summary>
/// Interface de configuration de l'encodeur FFMPEG.
/// </summary>
DECLARE_INTERFACE_(IVFFFMPEGEncoder, IUnknown)
{
    /// <summary>
    /// Definit la configuration complete de l'encodeur FFMPEG.
    /// </summary>
    /// <param name="settings">Structure complete des parametres de l'encodeur</param>
    STDMETHOD(set_settings)(THIS_
        CVFOutputSettings settings
        ) PURE;
};
```

### Définition Delphi

```delphi
uses
  ActiveX, ComObj;

const
  IID_IVFFFMPEGEncoder: TGUID = '{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}';
  CLSID_FFMPEGEncoder: TGUID = '{554AB365-B293-4C1D-9245-E8DB01F027F7}';

type
  /// <summary>
  /// Enumeration des formats de sortie FFMPEG.
  /// </summary>
  TVFFFMPEGDLLOutputFormat = (
    FLV,
    MPEG1,
    MPEG1VCD,
    MPEG2,
    MPEG2TS,
    MPEG2SVCD,
    MPEG2DVD
  );

  /// <summary>
  /// Enumeration des systemes TV. Note : le membre `None` peut entrer en
  /// conflit avec l'identificateur `None` expose par d'autres unites Delphi
  /// (par exemple `Variants` de la RTL). Referencez les valeurs via le nom
  /// qualifie de l'enum — `TVFFFMPEGDLLTVSystem.None` — pour eviter
  /// l'ambiguite dans le code reel.
  /// </summary>
  TVFFFMPEGDLLTVSystem = (
    None,
    PAL,
    NTSC,
    Film
  );

  /// <summary>
  /// Structure des parametres de sortie de l'encodeur FFMPEG.
  /// </summary>
  TFFMPEGOutputSettings = record
    Filename: PWideChar;
    AudioAvailable: BOOL;
    AudioBitrate: Integer;
    AudioSamplerate: Integer;
    AudioChannels: Integer;
    VideoWidth: Integer;
    VideoHeight: Integer;
    AspectRatioW: Integer;
    AspectRatioH: Integer;
    VideoBitrate: Integer;
    VideoMaxRate: Integer;
    VideoMinRate: Integer;
    VideoBufferSize: Integer;
    Interlace: BOOL;
    VideoGopSize: Integer;
    TVSystem: Integer;
    OutputFormat: Integer;
  end;

  /// <summary>
  /// Interface de configuration de l'encodeur FFMPEG.
  /// </summary>
  IVFFFMPEGEncoder = interface(IUnknown)
    ['{17B8FF7D-A67F-45CE-B425-0E4F607D8C60}']

    /// <summary>
    /// Definit la configuration complete de l'encodeur FFMPEG.
    /// </summary>
    procedure set_settings(const settings: TFFMPEGOutputSettings); stdcall;
  end;
```

## Spécifications des formats de sortie

### FLV (Flash Video)

**Format** : Adobe Flash Video (.flv)
**Cas d'usage** : streaming web, contenu Flash hérité
**Paramètres typiques** :
- Résolution : 640x480, 854x480, 1280x720
- Débit vidéo : 500-2500 kbps
- Audio : MP3, 64-128 kbps, 44100 Hz
- GOP : 30-60 images

### MPEG-1

**Format** : vidéo MPEG-1 (.mpg)
**Cas d'usage** : vidéo basique, systèmes hérités, web
**Paramètres typiques** :
- Résolution : 352x240 (NTSC), 352x288 (PAL)
- Débit vidéo : 1150 kbps
- Audio : MPEG Layer 2, 224 kbps, 44100 Hz
- GOP : 12-15 images

### MPEG-1 VCD (Video CD)

**Format** : MPEG-1 conforme Video CD (.mpg)
**Cas d'usage** : création de VCD, distribution sur disque
**Paramètres requis** :
- Résolution : 352x240 (NTSC), 352x288 (PAL)
- Débit vidéo : 1150 kbps (fixe)
- Audio : MPEG Layer 2, 224 kbps, 44100 Hz
- GOP : 12 images (NTSC), 15 images (PAL)
- Système TV : doit correspondre (NTSC ou PAL)

### MPEG-2

**Format** : vidéo MPEG-2 (.mpg)
**Cas d'usage** : création de DVD, diffusion, haute qualité
**Paramètres typiques** :
- Résolution : 720x480 (NTSC), 720x576 (PAL)
- Débit vidéo : 4000-9800 kbps
- Audio : MPEG Layer 2 ou AC-3, 192-448 kbps
- GOP : 12-15 images

### MPEG-2 TS (Transport Stream)

**Format** : MPEG-2 Transport Stream (.ts)
**Cas d'usage** : diffusion, streaming, Blu-ray
**Paramètres typiques** :
- Résolution : 720x480, 1280x720, 1920x1080
- Débit vidéo : 3000-15000 kbps
- Audio : MPEG Layer 2 ou AC-3
- GOP : 12-30 images
- Meilleure résistance aux erreurs que MPEG-2 PS

### MPEG-2 SVCD (Super Video CD)

**Format** : MPEG-2 conforme Super Video CD (.mpg)
**Cas d'usage** : création de SVCD, distribution sur disque
**Paramètres requis** :
- Résolution : 480x480 (NTSC), 480x576 (PAL)
- Débit vidéo : 2500 kbps (typique)
- Audio : MPEG Layer 2, 224 kbps, 44100 Hz
- GOP : 12-15 images
- Système TV : doit correspondre (NTSC ou PAL)

### MPEG-2 DVD (DVD-Video)

**Format** : MPEG-2 conforme DVD-Video (.mpg)
**Cas d'usage** : création de DVD, distribution professionnelle
**Paramètres requis** :
- Résolution : 720x480 (NTSC), 720x576 (PAL)
- Débit vidéo : 4000-9800 kbps
- Audio : AC-3 ou PCM, jusqu'à 448 kbps
- GOP : 12 images (NTSC), 15 images (PAL)
- Système TV : doit correspondre (NTSC ou PAL)
- Entrelacé : généralement activé pour la diffusion

## Exemples d'utilisation

### Exemple C# — MPEG-2 qualité DVD (NTSC)

```csharp
using System;
using DirectShowLib;
using VisioForge.DirectShowAPI;

public class FFMPEGDVDEncoder
{
    public void ConfigureDVDNTSC(IBaseFilter ffmpegEncoder)
    {
        // Interroger l'interface de l'encodeur FFMPEG
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
        {
            Console.WriteLine("Error: Filter does not support IVFFFMPEGEncoder");
            return;
        }

        // Configurer l'encodage MPEG-2 conforme DVD (NTSC)
        var settings = new FFMPEGOutputSettings
        {
            // Fichier de sortie
            Filename = "C:\\output\\movie.mpg",

            // Parametres audio
            AudioAvailable = true,
            AudioBitrate = 224000,      // 224 kbps
            AudioSamplerate = 48000,    // 48 kHz pour DVD
            AudioChannels = 2,           // Stereo

            // Parametres video - specifications DVD NTSC
            VideoWidth = 720,
            VideoHeight = 480,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 6000000,     // 6 Mbps
            VideoMaxRate = 9800000,     // 9.8 Mbps max pour DVD
            VideoMinRate = 0,
            VideoBufferSize = 1835008,  // Tampon DVD standard
            Interlace = true,           // DVD est entrelace
            VideoGopSize = 12,          // 12 images pour NTSC

            // Parametres de format
            TVSystem = VFFFMPEGDLLTVSystem.NTSC,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2DVD
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for DVD NTSC:");
        Console.WriteLine("  Resolution: 720x480 (16:9)");
        Console.WriteLine("  Video: 6 Mbps MPEG-2, interlaced");
        Console.WriteLine("  Audio: 224 kbps, 48 kHz stereo");
        Console.WriteLine("  GOP: 12 frames");
    }
}
```

### Exemple C# — streaming web FLV

```csharp
public class FFMPEGWebStreaming
{
    public void ConfigureFLV(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configurer Flash Video pour le streaming web
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\video.flv",

            // Parametres audio
            AudioAvailable = true,
            AudioBitrate = 96000,       // 96 kbps
            AudioSamplerate = 44100,
            AudioChannels = 2,

            // Parametres video - streaming web 720p
            VideoWidth = 1280,
            VideoHeight = 720,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 2000000,     // 2 Mbps
            VideoMaxRate = 2500000,     // 2.5 Mbps max
            VideoMinRate = 1500000,     // 1.5 Mbps min
            VideoBufferSize = 2000000,
            Interlace = false,          // Progressif pour le web
            VideoGopSize = 60,          // 2 secondes a 30 fps

            TVSystem = VFFFMPEGDLLTVSystem.None,
            OutputFormat = VFFFMPEGDLLOutputFormat.FLV
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for FLV web streaming:");
        Console.WriteLine("  720p @ 2 Mbps, progressive");
    }
}
```

### Exemple C# — MPEG-1 conforme VCD (PAL)

```csharp
public class FFMPEGVCDEncoder
{
    public void ConfigureVCDPAL(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configurer MPEG-1 conforme VCD (PAL)
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\vcd.mpg",

            // Parametres audio - specification VCD
            AudioAvailable = true,
            AudioBitrate = 224000,      // 224 kbps requis
            AudioSamplerate = 44100,    // 44.1 kHz requis
            AudioChannels = 2,

            // Parametres video - specifications VCD PAL
            VideoWidth = 352,
            VideoHeight = 288,          // Resolution PAL
            AspectRatioW = 4,
            AspectRatioH = 3,
            VideoBitrate = 1150000,     // 1150 kbps requis
            VideoMaxRate = 1150000,
            VideoMinRate = 1150000,
            VideoBufferSize = 327680,   // Taille de tampon VCD
            Interlace = false,
            VideoGopSize = 15,          // 15 images pour PAL

            TVSystem = VFFFMPEGDLLTVSystem.PAL,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG1VCD
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for VCD PAL:");
        Console.WriteLine("  352x288 @ 1150 kbps");
    }
}
```

### Exemple C# — MPEG-2 Transport Stream

```csharp
public class FFMPEGMPEG2TS
{
    public void ConfigureMPEG2TS(IBaseFilter ffmpegEncoder)
    {
        var encoder = ffmpegEncoder as IVFFFMPEGEncoder;
        if (encoder == null)
            return;

        // Configurer MPEG-2 Transport Stream pour la diffusion
        var settings = new FFMPEGOutputSettings
        {
            Filename = "C:\\output\\stream.ts",

            // Parametres audio
            AudioAvailable = true,
            AudioBitrate = 192000,
            AudioSamplerate = 48000,
            AudioChannels = 2,

            // Parametres video - diffusion HD 1080i
            VideoWidth = 1920,
            VideoHeight = 1080,
            AspectRatioW = 16,
            AspectRatioH = 9,
            VideoBitrate = 12000000,    // 12 Mbps
            VideoMaxRate = 15000000,    // 15 Mbps max
            VideoMinRate = 8000000,     // 8 Mbps min
            VideoBufferSize = 8000000,
            Interlace = true,           // La diffusion est entrelacee
            VideoGopSize = 15,

            TVSystem = VFFFMPEGDLLTVSystem.NTSC,
            OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2TS
        };

        encoder.set_settings(settings);

        Console.WriteLine("FFMPEG encoder configured for MPEG-2 TS:");
        Console.WriteLine("  1080i HD broadcast stream");
    }
}
```

### Exemple C++ — DVD NTSC

```cpp
#include <dshow.h>
#include <iostream>
#include "InterfaceDefine.h"

void ConfigureFFMPEGDVD(IBaseFilter* pFFMPEGEncoder)
{
    IVFFFMPEGEncoder* pEncoder = NULL;
    HRESULT hr = S_OK;

    // Interroger l'interface de l'encodeur FFMPEG
    hr = pFFMPEGEncoder->QueryInterface(IID_IVFFFMPEGEncoder,
                                        (void**)&pEncoder);
    if (FAILED(hr) || !pEncoder)
    {
        std::cout << "Error: Filter does not support IVFFFMPEGEncoder" << std::endl;
        return;
    }

    // Configurer l'encodage MPEG-2 conforme DVD (NTSC)
    CVFOutputSettings settings;
    ZeroMemory(&settings, sizeof(settings));

    settings.filename = L"C:\\output\\movie.mpg";

    // Parametres audio
    settings.audioAvailable = TRUE;
    settings.audioBitrate = 224000;
    settings.audioSamplerate = 48000;
    settings.audioChannels = 2;

    // Parametres video - specifications DVD NTSC
    settings.videoWidth = 720;
    settings.videoHeight = 480;
    settings.aspectRatioW = 16;
    settings.aspectRatioH = 9;
    settings.videoBitrate = 6000000;
    settings.videoMaxRate = 9800000;
    settings.videoMinRate = 0;
    settings.videoBufferSize = 1835008;
    settings.interlace = TRUE;
    settings.videoGopSize = 12;

    settings.tvSystem = video_norm_ntsc;
    settings.outputFormat = of_MPEG2DVD;

    pEncoder->set_settings(settings);

    std::cout << "FFMPEG encoder configured for DVD NTSC" << std::endl;

    pEncoder->Release();
}
```

### Exemple C++ — streaming web FLV

```cpp
void ConfigureFFMPEGFLV(IBaseFilter* pFFMPEGEncoder)
{
    IVFFFMPEGEncoder* pEncoder = NULL;
    HRESULT hr = pFFMPEGEncoder->QueryInterface(IID_IVFFFMPEGEncoder,
                                                (void**)&pEncoder);
    if (SUCCEEDED(hr) && pEncoder)
    {
        CVFOutputSettings settings;
        ZeroMemory(&settings, sizeof(settings));

        settings.filename = L"C:\\output\\video.flv";

        // Parametres audio
        settings.audioAvailable = TRUE;
        settings.audioBitrate = 96000;
        settings.audioSamplerate = 44100;
        settings.audioChannels = 2;

        // Parametres video - web 720p
        settings.videoWidth = 1280;
        settings.videoHeight = 720;
        settings.aspectRatioW = 16;
        settings.aspectRatioH = 9;
        settings.videoBitrate = 2000000;
        settings.videoMaxRate = 2500000;
        settings.videoMinRate = 1500000;
        settings.videoBufferSize = 2000000;
        settings.interlace = FALSE;
        settings.videoGopSize = 60;

        settings.tvSystem = video_norm_unknown;
        settings.outputFormat = of_FLV;

        pEncoder->set_settings(settings);

        std::cout << "FFMPEG FLV encoder configured" << std::endl;

        pEncoder->Release();
    }
}
```

### Exemple Delphi — DVD PAL

```delphi
uses
  DirectShow9, ActiveX;

procedure ConfigureFFMPEGDVDPAL(FFMPEGEncoder: IBaseFilter);
var
  Encoder: IVFFFMPEGEncoder;
  Settings: TFFMPEGOutputSettings;
  hr: HRESULT;
begin
  hr := FFMPEGEncoder.QueryInterface(IID_IVFFFMPEGEncoder, Encoder);
  if Failed(hr) or (Encoder = nil) then
  begin
    WriteLn('Error: Filter does not support IVFFFMPEGEncoder');
    Exit;
  end;

  try
    ZeroMemory(@Settings, SizeOf(Settings));

    Settings.Filename := 'C:\output\movie.mpg';

    // Parametres audio
    Settings.AudioAvailable := True;
    Settings.AudioBitrate := 224000;
    Settings.AudioSamplerate := 48000;
    Settings.AudioChannels := 2;

    // Parametres video - specifications DVD PAL
    Settings.VideoWidth := 720;
    Settings.VideoHeight := 576;
    Settings.AspectRatioW := 16;
    Settings.AspectRatioH := 9;
    Settings.VideoBitrate := 6000000;
    Settings.VideoMaxRate := 9800000;
    Settings.VideoMinRate := 0;
    Settings.VideoBufferSize := 1835008;
    Settings.Interlace := True;
    Settings.VideoGopSize := 15;

    Settings.TVSystem := Ord(PAL);
    Settings.OutputFormat := Ord(MPEG2DVD);

    Encoder.set_settings(Settings);

    WriteLn('FFMPEG encoder configured for DVD PAL');
  finally
    Encoder := nil;
  end;
end;
```

## Bonnes pratiques

### Recommandations par format

**Pour la création de DVD (MPEG2DVD)** :
- Faites toujours correspondre le système TV à la région cible (NTSC pour les Amériques/Japon, PAL pour l'Europe/Asie)
- Utilisez 720x480 pour NTSC, 720x576 pour PAL
- Activez l'entrelacement pour la compatibilité diffusion
- GOP : 12 images (NTSC), 15 images (PAL)
- Débit vidéo : 4-9,8 Mbps
- Audio : 48 kHz, 224-448 kbps

**Pour VCD/SVCD (MPEG1VCD, MPEG2SVCD)** :
- Respectez strictement les spécifications de format (résolution, débit)
- Faites correspondre le système TV à la région cible
- Utilisez exactement les débits spécifiés pour la meilleure compatibilité
- Testez sur le matériel cible (lecteurs VCD/SVCD autonomes)

**Pour le streaming web (FLV)** :
- Utilisez un encodage progressif (non entrelacé)
- Débits plus bas pour une portée plus large (500-2500 kbps)
- GOP : 30-60 images pour des points de navigation toutes les 1-2 secondes
- Envisagez des alternatives modernes (MP4/H.264) pour une meilleure qualité

**Pour la diffusion (MPEG2TS)** :
- Transport Stream offre une meilleure résistance aux erreurs
- À utiliser pour la diffusion en direct et les applications de broadcast
- Débits plus élevés acceptables (10-15 Mbps pour HD)
- Faites correspondre l'entrelacement à la norme de diffusion

### Résolution et rapport d'aspect

**Résolutions standard par format** :

| Format | Résolution NTSC | Résolution PAL | Rapport d'aspect |
|--------|----------------|----------------|--------------|
| VCD | 352x240 | 352x288 | 4:3 |
| SVCD | 480x480 | 480x576 | 4:3 ou 16:9 |
| DVD | 720x480 | 720x576 | 4:3 ou 16:9 |
| FLV/MPEG-2 | Tout | Tout | Tout |

**Paramètres de rapport d'aspect** :
- 4:3 standard : `AspectRatioW = 4, AspectRatioH = 3`
- 16:9 écran large : `AspectRatioW = 16, AspectRatioH = 9`
- Assurez-vous que le rapport d'aspect d'affichage correspond au rapport d'aspect des pixels

### Directives sur la taille de GOP

**VCD/SVCD/DVD** :
- NTSC : 12 images (0,4 seconde à 29,97 fps)
- PAL : 15 images (0,6 seconde à 25 fps)
- Requis pour la conformité au format

**Streaming web** :
- 30-60 images (1-2 secondes)
- GOP plus court : meilleure navigation, fichier plus volumineux
- GOP plus long : fichier plus petit, navigation plus lente

**Diffusion** :
- 12-15 images pour une haute qualité
- À adapter à la norme du système TV

### Configuration du débit binaire

**VBR (débit binaire variable)** :
- Définissez `VideoBitrate` (moyenne), `VideoMinRate`, `VideoMaxRate`
- Meilleure qualité pour un même débit moyen
- À utiliser pour l'encodage basé sur fichier

**CBR (débit binaire constant)** :
- Définissez les trois débits à la même valeur
- Taille de fichier et bande passante prévisibles
- À utiliser pour le streaming et la diffusion

### Paramètres audio

**Fréquences d'échantillonnage** :
- 44100 Hz : qualité CD, VCD/SVCD
- 48000 Hz : DVD, diffusion professionnelle
- Faites correspondre à la source quand c'est possible

**Débits binaires** :
- Parole mono : 64-96 kbps
- Musique stéréo : 128-224 kbps
- Audio DVD : 192-448 kbps

## Dépannage

### Échec de l'initialisation de l'encodeur

**Symptômes** : l'appel `set_settings` échoue ou plante

**Causes possibles** :
1. Nom ou chemin de fichier invalide
2. Spécifications de format incorrectes
3. Combinaison résolution/débit non prise en charge

**Solutions** :
- Vérifiez que le répertoire de sortie existe et est accessible en écriture
- Vérifiez que la résolution correspond aux exigences du format
- Vérifiez que le débit est dans les limites du format
- Pour VCD/SVCD/DVD, respectez strictement les spécifications

### Le fichier de sortie ne se lit pas

**Symptômes** : le fichier encodé ne se lit pas ou comporte des erreurs

**Causes possibles** :
1. Les spécifications de format ne sont pas respectées
2. Mauvais système TV pour le format
3. Incohérence d'entrelacement
4. Problèmes de taille de GOP

**Solutions** :
- Pour DVD/VCD/SVCD : utilisez les paramètres exacts de la spécification
- Faites correspondre le système TV à la région cible
- Activez l'entrelacement pour DVD/diffusion
- Utilisez des tailles de GOP standard (12 pour NTSC, 15 pour PAL)

### Qualité vidéo médiocre

**Symptômes** : vidéo en blocs, floue ou avec artefacts

**Causes possibles** :
1. Débit trop faible pour la résolution
2. Taille de GOP incorrecte
3. Plage de débit trop restrictive

**Solutions** :
- Augmentez le débit vidéo (voir les recommandations par format)
- Pour VBR, élargissez la plage débit min/max
- Utilisez une taille de GOP adaptée au format
- Vérifiez que la résolution correspond aux spécifications du format

### Problèmes de synchronisation A/V

**Symptômes** : l'audio et la vidéo se désynchronisent

**Causes possibles** :
1. Fréquence d'échantillonnage incorrecte
2. Mauvaise fréquence d'images pour le système TV
3. Problèmes de taille de tampon

**Solutions** :
- Utilisez 48000 Hz pour DVD, 44100 Hz pour VCD
- Vérifiez que le système TV correspond à la fréquence d'images source
- Augmentez `VideoBufferSize` pour les contenus complexes
- Vérifiez que l'audio/vidéo source sont synchronisés

### Le DVD/VCD ne se lit pas sur le matériel

**Symptômes** : le fichier se lit sur l'ordinateur mais pas sur un lecteur autonome

**Causes possibles** :
1. Les spécifications de format ne sont pas strictement respectées
2. Mauvais système TV
3. Taille de GOP ou débit non conformes

**Solutions** :
- **Critique** : utilisez les spécifications exactes du format
- VCD : 352x240/288, 1150 kbps, GOP 12/15
- DVD : 720x480/576, max 9,8 Mbps, GOP 12/15
- Faites correspondre le système TV à la région du lecteur
- Activez l'entrelacement pour DVD
- Testez d'abord avec un lecteur DVD/VCD logiciel

### Fichiers de sortie volumineux

**Symptômes** : fichiers de sortie plus volumineux que prévu

**Causes possibles** :
1. Débit trop élevé
2. CBR au lieu de VBR
3. Taille de GOP trop petite

**Solutions** :
- Réduisez le débit vidéo
- Utilisez VBR avec une plage min/max appropriée
- Augmentez la taille de GOP (pour les formats hors DVD/VCD)
- Envisagez des formats plus efficaces (H.264/MP4 au lieu de MPEG-2)

---
## Voir aussi
- [Interface de l'encodeur H.264](h264.md)
- [Référence des codecs vidéo](../codecs-reference.md)
- [Interface du multiplexeur MP4](mp4-muxer.md)
- [Présentation du pack de filtres d'encodage](../index.md)
