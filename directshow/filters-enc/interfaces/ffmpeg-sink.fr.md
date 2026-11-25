---
title: Broches du filtre FFMPEG Encoder et options du codec
description: Options de l'encodeur FFmpeg, décimation d'images et filtres audio par broche du filtre DirectShow FFMPEG Encoder, avec exemples WebM, Ogg Speex et FLAC.
tags:
  - DirectShow
  - C++
  - C#
  - Windows
  - Encoding
  - FFmpeg
  - WebM
  - VP8
  - VP9
  - Vorbis
  - Opus
  - Speex
  - FLAC
primary_api_classes:
  - IFFmpegSinkPinSettings2
  - IFFmpegSinkPinSettings
  - IFFmpegSinkSettings
  - IFileSinkFilter

---

# Référence des réglages de broche de FFMPEG Encoder

## Vue d'ensemble

**VisioForge FFMPEG Encoder** (`VisioForge_FFMPEG_Encoder.ax`) est un moteur de rendu DirectShow qui encode et multiplexe ses entrées avec FFmpeg. Chaque broche d'entrée connectée est un flux du fichier de sortie : vidéo, audio ou sous-titres. Le filtre ajoute une nouvelle broche d'entrée chaque fois qu'une broche est connectée.

| Élément | Valeur |
|---|---|
| CLSID du filtre | `{69F244AB-31A9-4B74-81CB-CF8C9FACB2EC}` |
| `IFFmpegSinkSettings` (filtre) | `{CAEC87C3-24B5-404C-9985-FA18D2B78A4A}` |
| `IFFmpegSinkPinSettings` (broche d'entrée) | `{496D3BA7-0630-499E-A730-577D4A049F9F}` |
| `IFFmpegSinkPinSettings2` (broche d'entrée) | `{1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B}`, depuis la version 2026.11.25.1 du filtre |

Le nom du fichier de sortie se définit avec la méthode standard `IFileSinkFilter::SetFileName`, le conteneur avec `IFFmpegSinkSettings::SetFormat`. Chaque broche d'entrée reçoit son codec et les réglages de l'encodeur par `IFFmpegSinkPinSettings`, et les réglages FFmpeg plus fins par `IFFmpegSinkPinSettings2` :

- **`SetCodecOption`** transmet n'importe quelle option de l'encodeur FFmpeg par son nom : le deadline de VP8/VP9, le placement des images clés et le redimensionnement, le VBR de Speex, la taille de bloc FLAC et tout ce que l'encodeur accepte d'autre.
- **`SetVideoDecimate`** conserve une image vidéo sur *n*, pour enregistrer à faible fréquence d'images une source à pleine fréquence.
- **`SetAudioFilter`** fait passer l'audio par un graphe de filtres FFmpeg avant l'encodeur : contrôle du gain, réduction du bruit, égalisation.

Ce filtre n'est pas celui décrit dans [Interface de FFmpeg Encoder (IVFFFMPEGEncoder)](ffmpeg.md) ; cette page traite d'un filtre plus ancien, avec un autre CLSID.

## Limites de plateforme et d'utilisation

- Windows uniquement, comme filtre DirectShow, en versions x86 et x64. Il n'existe pas de version ARM64.
- Le filtre a besoin, à côté de lui, des DLL FFmpeg de la même architecture ; voir [Fichiers redistribuables de FFMPEG Encoder](../../deployment/redistributable-files.md#encodeur-ffmpeg).
- Définissez les réglages de la broche après sa connexion et avant l'exécution du graphe. Ils sont lus à l'ouverture de la sortie, au passage de l'état arrêté à l'état en pause ou en cours d'exécution ; une modification pendant l'exécution du graphe s'applique à l'exécution suivante.
- Un réglage refusé par l'encodeur fait échouer cette ouverture : `IMediaControl::Run` (ou `Pause`) renvoie un HRESULT d'échec et aucun fichier n'est écrit. La raison est écrite dans la sortie de débogage (`OutputDebugString`, visible dans DebugView), par exemple `Session open failed: libvorbis has no option 'no_such_option'`.

## Formats et codecs

`IFFmpegSinkSettings::SetFormat` prend `FFMPEG_SINK_FORMAT`. `FFMPEG_SINK_FORMAT_FLAC` (un fichier `.flac` natif) est nouveau dans la version 2026.11.25.1 :

| Valeur | Nom | Valeur | Nom |
|---|---|---|---|
| 0 | `FFMPEG_SINK_FORMAT_AUTO` (d'après le nom du fichier) | 8 | `FFMPEG_SINK_FORMAT_WEBM` |
| 1 | `FFMPEG_SINK_FORMAT_MP4` | 9 | `FFMPEG_SINK_FORMAT_M4A` |
| 2 | `FFMPEG_SINK_FORMAT_MKV` | 10 | `FFMPEG_SINK_FORMAT_MOV` |
| 3 | `FFMPEG_SINK_FORMAT_FLV` | 11 | `FFMPEG_SINK_FORMAT_MPEG1` |
| 4 | `FFMPEG_SINK_FORMAT_OGG` | 12 | `FFMPEG_SINK_FORMAT_MPEG2` |
| 5 | `FFMPEG_SINK_FORMAT_MPEG_TS` | 13 | `FFMPEG_SINK_FORMAT_MPEG_TS_KLV` |
| 6 | `FFMPEG_SINK_FORMAT_VOB` | 14 | `FFMPEG_SINK_FORMAT_FLAC` |
| 7 | `FFMPEG_SINK_FORMAT_AVI` | | |

`IFFmpegSinkPinSettings::SetAudioCodec` prend `FFMPEG_SINK_AUDIO_CODEC`. `FFMPEG_SINK_AUDIO_CODEC_SPEEX` (libspeex, en Ogg) est nouveau dans la version 2026.11.25.1 :

| Valeur | Nom | Valeur | Nom |
|---|---|---|---|
| 0 | `FFMPEG_SINK_AUDIO_CODEC_AUTO` | 4 | `FFMPEG_SINK_AUDIO_CODEC_FLAC` |
| 1 | `FFMPEG_SINK_AUDIO_CODEC_MP2` | 5 | `FFMPEG_SINK_AUDIO_CODEC_VORBIS` |
| 2 | `FFMPEG_SINK_AUDIO_CODEC_MP3` | 6 | `FFMPEG_SINK_AUDIO_CODEC_OPUS` |
| 3 | `FFMPEG_SINK_AUDIO_CODEC_AAC` | 7 | `FFMPEG_SINK_AUDIO_CODEC_SPEEX` |

Speex n'encode qu'à 8, 16 ou 32 kHz. Sans `SetAudioSampleRate`, le filtre prend la fréquence prise en charge la plus proche de celle de la source : une source à 44,1 kHz est donc encodée à 32 kHz ; indiquez 16000 pour une voix en large bande à un débit plus faible. MP2 n'accepte que les débits standard de Layer II, chacun à certaines fréquences d'échantillonnage seulement : 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 ou 384 kbps à 32, 44,1 et 48 kHz ; 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144 ou 160 kbps à 16, 22,05 et 24 kHz. Un débit défini avec `SetAudioBitRate` ou l'option `b`/`ab` doit être l'un d'eux, sinon le démarrage échoue ; sans débit défini, MP2 utilise 384 kbps à 32-48 kHz et 160 kbps à 16-24 kHz. Sans `SetAudioSampleRate`, il prend la fréquence la plus proche de celle de la source parmi celles qui acceptent son débit, par exemple 32 kHz pour 192 kbps depuis une source à 22,05 kHz.

## Méthodes de IFFmpegSinkPinSettings2

`IFFmpegSinkPinSettings2` dérive de `IFFmpegSinkPinSettings` : un seul pointeur donne accès aux deux. Interrogez-la sur la broche d'entrée connectée.

### SetCodecOption

```cpp
HRESULT SetCodecOption(LPCSTR key, LPCSTR value);
```

Définit une option de l'encodeur de cette broche par son nom d'AVOption FFmpeg : les options génériques (`g`, `b`, `maxrate`, `bufsize`, `flags`, `global_quality`, ...) et celles propres à l'encodeur (`deadline`, `cpu-used`, `vbr_bitrate`, ...). Les noms sont ceux que liste `ffmpeg -h encoder=<nom>`. Les spécificateurs de flux de la ligne de commande, comme `b:v` ou `q:a`, ne sont pas des noms d'option : écrivez `b` ou `global_quality`.

- Une option l'emporte sur les autres réglages de la broche : `g` = `10` l'emporte sur `SetVideoGOPSize(24)`.
- Redéfinir la même clé remplace sa valeur.
- Une option que l'encodeur ne connaît pas fait échouer l'ouverture ; elle n'est jamais ignorée en silence.
- Les options qui changeraient le format d'image que la broche tient de sa connexion renvoient immédiatement `E_INVALIDARG` : `pixel_format`, `video_size`, `time_base`, `ar`, `ch_layout`, ainsi que les graphies de la ligne de commande `pix_fmt`, `s`, `ac`, `sample_fmt`. Définissez la taille, la fréquence d'échantillonnage et les canaux avec `SetVideoWidth`/`SetVideoHeight`, `SetAudioSampleRate` et `SetAudioChannelMask`.

**`frame_size`** (taille de la trame audio en échantillons) n'est tenue que là où l'encodeur conserve la taille qu'il reçoit :

| Encodeur | `frame_size` |
|---|---|
| FLAC | Acceptée : l'encodeur écrit des blocs de ce nombre d'échantillons (par exemple 1152 ou 4096). |
| AAC, Opus, MP3, MP2, Vorbis, Speex | Fait échouer l'ouverture, sauf si elle est égale à la taille propre de l'encodeur. Opus fixe sa taille avec `frame_duration` (millisecondes) : utilisez cette option. |
| PCM, et tout encodeur qui accepte des trames de toute taille | Fait échouer l'ouverture (il compléterait la dernière trame par du silence jusqu'à cette taille). |
| Tout encodeur vidéo | Fait échouer l'ouverture. |

`0` demande la taille propre de l'encodeur, quel que soit l'encodeur. La valeur est lue comme FFmpeg la lit : `0x480` vaut donc 1152. Une taille de trame plus grande fait aussi perdre davantage d'audio lors d'une discontinuité de l'entrée (un saut dans les horodatages de la source), où la partie qui ne remplit pas une trame de l'encodeur est abandonnée.

**Options ajoutées à la version de FFmpeg de ce filtre** (2026.11.25.1), pour les réglages des anciens filtres WebM et Ogg :

| Encodeur | Option | Valeurs | Effet |
|---|---|---|---|
| VP8, VP9 (libvpx) | `kf-mode` | `auto`, `disabled` | `disabled` ne place aucune image clé après la première, y compris en VP9, qui sinon en impose une toutes les `kf_max_dist` images. |
| VP8 | `resize-allowed` | `0`, `1` | Permet à l'encodeur de réduire la taille de l'image quand le débit manque. |
| VP8 | `resize-up-thresh` | 0 à 100 | Niveau du tampon, en pourcentage, au-dessus duquel l'encodeur revient à une taille supérieure. |
| VP8 | `resize-down-thresh` | 0 à 100 | Niveau du tampon, en pourcentage, en dessous duquel l'encodeur réduit la taille. |
| VP8 | `buf-optimal-sz` | millisecondes | Le niveau du tampon du décodeur que vise le contrôle de débit. |
| Speex | `vbr_bitrate` | bits par seconde | VBR à la plus haute qualité dont le débit tient dans ce débit, plafonné à celui-ci. La stéréo ajoute environ 800 bps d'information latérale. |
| Speex | `vbr_max_bitrate` | bits par seconde, 0 = aucun | Plafonne un flux VBR : avec `vbr_bitrate`, ou avec `flags` = `+qscale` et `global_quality`. |

Non définie (la valeur par défaut -1 ou 0), chacune garde le comportement propre de FFmpeg.

### SetVideoDecimate

```cpp
HRESULT SetVideoDecimate(LONG decimate);
```

Conserve la première image vidéo sur `decimate` et abandonne les autres ; les images conservées gardent leurs propres horodatages. `0` ou `1` conserve toutes les images. Utilisez-la pour enregistrer une caméra à 25 i/s à 5 i/s (`decimate` = 5) sans filtre de fréquence d'images dans le graphe.

L'encodeur reçoit la fréquence des images conservées : le débit défini sur la broche reste donc tenu. Le GOP défini avec `SetVideoGOPSize` compte les images d'entrée et est arrondi à l'image conservée la plus proche, si bien que l'intervalle des images clés dans le temps reste lui aussi tenu ; une option de codec `g` compte les images conservées. Le réglage est ignoré sur une broche audio.

### SetAudioFilter

```cpp
HRESULT SetAudioFilter(LPCSTR graph);
```

Fait passer l'audio de la broche par un graphe de filtres FFmpeg, écrit comme pour l'option `ffmpeg -af`, avant le rééchantillonneur et l'encodeur. Une chaîne vide le supprime. Un graphe que FFmpeg ne peut pas construire fait échouer l'ouverture. Le réglage est ignoré sur une broche vidéo.

| Graphe | Usage |
|---|---|
| `speechnorm` | Contrôle automatique du gain pour la voix (remplace l'AGC de Speex). |
| `afftdn` | Réduction du bruit (remplace le débruiteur de Speex). |
| `speechnorm,afftdn=nr=20` | Les deux, enchaînés. |
| `highpass=f=100` | Coupe le grondement sous 100 Hz. |

Lors d'une discontinuité de l'entrée, le graphe est vidé sur l'ancienne ligne de temps puis redémarré : les filtres à mémoire, comme `speechnorm`, repartent donc de ce point.

## Définitions de l'interface

### Définition C++

L'en-tête complet, `IFFmpegSinkPinSettings.h`, est livré avec le filtre ; la nouvelle interface est :

```cpp
MIDL_INTERFACE("1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B")
IFFmpegSinkPinSettings2 : IFFmpegSinkPinSettings {
    STDMETHOD(SetCodecOption)(LPCSTR key, LPCSTR value) = 0;
    STDMETHOD(SetVideoDecimate)(LONG decimate) = 0;
    STDMETHOD(SetAudioFilter)(LPCSTR graph) = 0;
};

_COM_SMARTPTR_TYPEDEF(IFFmpegSinkPinSettings2, __uuidof(IFFmpegSinkPinSettings2));
```

### Définition C\#

En C#, une interface COM énumère d'abord les méthodes de ses interfaces de base, dans leur ordre. Les paramètres de profil et de niveau sont déclarés ici en `int` ; leurs valeurs sont celles de `FFMPEG_SINK_VIDEO_PROFILE` et `FFMPEG_SINK_VIDEO_LEVEL` dans l'en-tête C++ (`0` = automatique).

```csharp
using System;
using System.Runtime.InteropServices;

namespace VisioForge.DirectShowAPI
{
    public enum FFMPEG_SINK_FORMAT
    {
        AUTO, MP4, MKV, FLV, OGG, MPEG_TS, VOB, AVI, WEBM, M4A, MOV, MPEG1, MPEG2, MPEG_TS_KLV, FLAC
    }

    public enum FFMPEG_SINK_VIDEO_CODEC
    {
        AUTO, H264_MF, H264_QSV, H264_NVENC, H264_AMF, HEVC_MF, HEVC_QSV, HEVC_NVENC, HEVC_AMF,
        MPEG1, MPEG2, VP8, VP9, MPEG4, OPENH264
    }

    public enum FFMPEG_SINK_AUDIO_CODEC
    {
        AUTO, MP2, MP3, AAC, FLAC, VORBIS, OPUS, SPEEX
    }

    public enum FFMPEG_SINK_SUBTITLE_CODEC
    {
        AUTO, SUBRIP, ASS
    }

    [ComImport]
    [Guid("CAEC87C3-24B5-404C-9985-FA18D2B78A4A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFFmpegSinkSettings
    {
        [PreserveSig] int SetFormat(FFMPEG_SINK_FORMAT format);
    }

    [ComImport]
    [Guid("1AF1F1D2-F63B-4C9B-83EA-144AE31B6E1B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFFmpegSinkPinSettings2
    {
        // IFFmpegSinkPinSettings
        [PreserveSig] int SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC codec);
        [PreserveSig] int SetVideoBitRate(int bitRate);
        [PreserveSig] int SetVideoWidth(int width);
        [PreserveSig] int SetVideoHeight(int height);
        [PreserveSig] int SetVideoProfile(int profile);
        [PreserveSig] int SetVideoLevel(int level);
        [PreserveSig] int SetVideoGOPSize(int gopSize);
        [PreserveSig] int SetVideoMaxBFrames(int maxBFrames);
        [PreserveSig] int SetVideoThreadCount(int threadCount);
        [PreserveSig] int SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC codec);
        [PreserveSig] int SetAudioBitRate(int bitRate);
        [PreserveSig] int SetAudioSampleRate(int sampleRate);
        [PreserveSig] int SetAudioChannelMask(uint channelMask);
        [PreserveSig] int SetSubtitleCodec(FFMPEG_SINK_SUBTITLE_CODEC codec);

        // IFFmpegSinkPinSettings2
        [PreserveSig] int SetCodecOption([MarshalAs(UnmanagedType.LPStr)] string key,
                                         [MarshalAs(UnmanagedType.LPStr)] string value);
        [PreserveSig] int SetVideoDecimate(int decimate);
        [PreserveSig] int SetAudioFilter([MarshalAs(UnmanagedType.LPStr)] string graph);
    }
}
```

`IFFmpegSinkSettings` ne déclare ici que sa première méthode ; elle en a une seconde, `WriteKLVData`, pour la sortie MPEG-TS KLV.

## Exemples d'utilisation

Les exemples commencent une fois la source connectée à FFMPEG Encoder, après `IFileSinkFilter::SetFileName` et `IFFmpegSinkSettings::SetFormat`, et avant `IMediaControl::Run`.

### Exemple C++ : WebM VP8 à 5 i/s, sans image clé après la première

```cpp
#include <dshow.h>
#include "IFFmpegSinkPinSettings.h"

// Pour chaque broche d'entrée connectée de FFMPEG Encoder :
HRESULT ConfigureWebMPin(IPin* pin)
{
    AM_MEDIA_TYPE mt = {};
    HRESULT hr = pin->ConnectionMediaType(&mt);
    if (FAILED(hr)) return hr;
    const bool isVideo = mt.majortype == MEDIATYPE_Video;
    FreeMediaType(mt);

    IFFmpegSinkPinSettings2Ptr settings;
    hr = pin->QueryInterface(IID_PPV_ARGS(&settings));
    if (FAILED(hr)) return hr;   // un filtre antérieur à 2026.11.25.1

    if (isVideo) {
        settings->SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC_VP8);
        settings->SetVideoBitRate(500000);
        settings->SetVideoDecimate(5);                       // 25 i/s en entrée, 5 i/s en sortie
        settings->SetCodecOption("deadline", "realtime");
        settings->SetCodecOption("kf-mode", "disabled");
    } else {
        settings->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_VORBIS);
        settings->SetAudioBitRate(96000);
    }
    return S_OK;
}
```

### Exemple C++ : enregistrement vocal Ogg Speex avec AGC et réduction du bruit

```cpp
// Format : IFFmpegSinkSettings::SetFormat(FFMPEG_SINK_FORMAT_OGG)
IFFmpegSinkPinSettings2Ptr audio;
hr = audioPin->QueryInterface(IID_PPV_ARGS(&audio));
if (SUCCEEDED(hr)) {
    audio->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_SPEEX);
    audio->SetAudioSampleRate(16000);                        // large bande
    audio->SetCodecOption("vbr_bitrate", "16000");           // VBR, 16 kbps au maximum
    audio->SetAudioFilter("speechnorm,afftdn");
}
```

### Exemple C++ : fichier FLAC avec des blocs de 4096 échantillons

```cpp
// Format : IFFmpegSinkSettings::SetFormat(FFMPEG_SINK_FORMAT_FLAC)
IFFmpegSinkPinSettings2Ptr audio;
hr = audioPin->QueryInterface(IID_PPV_ARGS(&audio));
if (SUCCEEDED(hr)) {
    audio->SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC_FLAC);
    audio->SetCodecOption("frame_size", "4096");
    audio->SetCodecOption("compression_level", "8");
}
```

### Exemple C# : options sur les broches de l'encodeur

Ce fragment utilise DirectShowLib et les définitions ci-dessus.

```csharp
using System;
using System.Runtime.InteropServices;
using DirectShowLib;
using VisioForge.DirectShowAPI;

// encoder : l'IBaseFilter de FFMPEG Encoder, déjà connecté à la source.
void ConfigurePins(IBaseFilter encoder)
{
    encoder.EnumPins(out IEnumPins pins);
    var pin = new IPin[1];
    while (pins.Next(1, pin, IntPtr.Zero) == 0)
    {
        var mt = new AMMediaType();
        if (pin[0].ConnectionMediaType(mt) == 0 && pin[0] is IFFmpegSinkPinSettings2 settings)
        {
            if (mt.majorType == MediaType.Video)
            {
                settings.SetVideoCodec(FFMPEG_SINK_VIDEO_CODEC.VP9);
                settings.SetCodecOption("deadline", "realtime");
                settings.SetCodecOption("cpu-used", "8");
            }
            else if (mt.majorType == MediaType.Audio)
            {
                settings.SetAudioCodec(FFMPEG_SINK_AUDIO_CODEC.OPUS);
                settings.SetAudioFilter("speechnorm");
            }
        }

        DsUtils.FreeAMMediaType(mt);
        Marshal.ReleaseComObject(pin[0]);
    }

    Marshal.ReleaseComObject(pins);
}
```

## Dépannage

### Run échoue après la définition d'une option

Surveillez la sortie de débogage (DebugView) à la recherche de `Session open failed:`. La ligne donne la cause :

- `<encoder> has no option '<key>'` : le nom n'est pas valable pour cet encodeur, ou une graphie de la ligne de commande comme `b:v` a été utilisée. Consultez `ffmpeg -h encoder=<nom>`.
- `<encoder> does not take frame_size <n>` : voir le tableau `frame_size` ci-dessus ; le même texte signale aussi une valeur qui n'est pas un nombre à partir de 0, comme `-1` ou `abc`.
- Un texte d'erreur FFmpeg, comme `Invalid argument` : la valeur sort de la plage de l'option, ou le graphe de filtres audio ne se construit pas.

### SetCodecOption renvoie E_INVALIDARG

La clé change le format d'image que la broche tient de sa connexion. Utilisez le réglage propre à la broche : `SetVideoWidth`/`SetVideoHeight`, `SetAudioSampleRate`, `SetAudioChannelMask`.

### QueryInterface pour IFFmpegSinkPinSettings2 renvoie E_NOINTERFACE

Le `VisioForge_FFMPEG_Encoder.ax` installé est antérieur à la version 2026.11.25.1. Mettez à jour le filtre en même temps que les DLL FFmpeg placées à côté de lui.

## Voir aussi

- [Interface de FFmpeg Encoder (IVFFFMPEGEncoder)](ffmpeg.md)
- [Fichiers redistribuables](../../deployment/redistributable-files.md#encodeur-ffmpeg)
- [Présentation du pack de filtres d'encodage](../index.md)
