---
title: Référence des interfaces du filtre source DirectShow VLC
description: Famille d'interfaces IVlcSrc pour l'audio multi-piste, le support des sous-titres et les options de ligne de commande VLC dans les applications DirectShow.
tags:
  - DirectShow
  - C++
  - Windows
  - IP Camera
  - RTSP
  - HLS
  - MP4
  - MKV
  - C#
primary_api_classes:
  - IBaseFilter

---

# Référence des interfaces du filtre source VLC

## Vue d'ensemble

Le filtre source DirectShow VLC expose trois interfaces progressives (`IVlcSrc`, `IVlcSrc2`, `IVlcSrc3`) qui fournissent un contrôle complet sur la lecture multimédia, la sélection de pistes audio/sous-titres et la configuration de VLC. Ces interfaces permettent aux développeurs d'exploiter le puissant framework multimédia VLC dans les applications DirectShow.

## Hiérarchie des interfaces

```
IUnknown
  └── IVlcSrc
        └── IVlcSrc2
              └── IVlcSrc3
```

Chaque interface étend la précédente, ajoutant de nouvelles capacités tout en conservant la compatibilité ascendante.

---
## Interface IVlcSrc
L'interface de base fournissant les capacités essentielles de chargement de fichier et de sélection de piste.
### Définition de l'interface
- **Nom de l'interface** : `IVlcSrc`
- **GUID** : `{77493EB7-6D00-41C5-9535-7C593824E892}`
- **Hérite de** : `IUnknown`
- **Fichier d'en-tête** : `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)
### Méthodes
#### SetFile
Définit le fichier multimédia ou l'URL à lire.
**Syntaxe (C++)** :
```cpp
HRESULT SetFile(WCHAR *file);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetFile([MarshalAs(UnmanagedType.LPWStr)] string file);
```
**Paramètres** :
- `file` : chaîne de caractères larges contenant le chemin du fichier ou l'URL.
**Retour** : `S_OK` (0) en cas de succès, code d'erreur sinon.
**Sources prises en charge** :
- Fichiers locaux : `C:\Videos\movie.mp4`
- Flux HTTP : `https://example.com/stream.m3u8`
- Flux RTSP : `rtsp://example.com/live`
- Playlists HLS : `https://example.com/playlist.m3u8`
- Flux DASH : `https://example.com/manifest.mpd`
- Diffusions DVB-T/C/S
- Partages réseau : `\\server\share\video.mkv`
**Exemple (C++)** :
```cpp
IVlcSrc* pVlcSrc = nullptr;
pFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
pVlcSrc->SetFile(L"C:\\Videos\\movie.mkv");
pVlcSrc->Release();
```
**Exemple (C#)** :
```csharp
var vlcSrc = filter as IVlcSrc;
if (vlcSrc != null)
{
    vlcSrc.SetFile(@"C:\Videos\movie.mkv");
}
```
---

### Gestion des pistes audio

#### GetAudioTracksCount

Récupère le nombre total de pistes audio disponibles.

**Syntaxe (C++)** :

```cpp
HRESULT GetAudioTracksCount(int *count);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int GetAudioTracksCount(out int count);
```

**Paramètres** :

- `count` : [out] reçoit le nombre de pistes audio.

**Retour** : `S_OK` (0) en cas de succès.

**Remarques d'utilisation** :

- Appelez après le chargement du fichier et la construction du graphe de filtres
- Renvoie 0 si aucune piste audio n'est disponible ou si le fichier n'est pas chargé

**Exemple (C++)** :

```cpp
int audioCount = 0;
pVlcSrc->GetAudioTracksCount(&audioCount);
printf("Audio tracks: %d\n", audioCount);
```

---
#### GetAudioTrackInfo
Récupère les informations sur une piste audio spécifique.
**Syntaxe (C++)** :
```cpp
HRESULT GetAudioTrackInfo(int number, int *id, WCHAR *name);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int GetAudioTrackInfo(int number, out int id,
                      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name);
```
**Paramètres** :
- `number` : index de piste à partir de zéro (0 à count-1).
- `id` : [out] reçoit l'ID de la piste.
- `name` : [out] tampon pour recevoir le nom de la piste (doit être pré-alloué, minimum 256 caractères).
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- Pré-allouez le tampon de nom avec au moins 256 caractères larges
- Les noms de piste incluent généralement la langue et les informations de codec
- L'ID de piste est utilisé avec SetAudioTrack()
**Exemple (C++)** :
```cpp
int audioCount = 0;
pVlcSrc->GetAudioTracksCount(&audioCount);
for (int i = 0; i < audioCount; i++)
{
    int id = 0;
    WCHAR name[256] = {0};
    pVlcSrc->GetAudioTrackInfo(i, &id, name);
    wprintf(L"Track %d - ID: %d, Name: %s\n", i, id, name);
}
```
**Exemple (C#)** :
```csharp
int count = 0;
vlcSrc.GetAudioTracksCount(out count);
for (int i = 0; i < count; i++)
{
    int id;
    var name = new StringBuilder(256);
    vlcSrc.GetAudioTrackInfo(i, out id, name);
    Console.WriteLine($"Track {i} - ID: {id}, Name: {name}");
}
```
---

#### GetAudioTrack

Récupère l'ID de la piste audio active actuelle.

**Syntaxe (C++)** :

```cpp
HRESULT GetAudioTrack(int *id);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int GetAudioTrack(out int id);
```

**Paramètres** :

- `id` : [out] reçoit l'ID de la piste audio courante.

**Retour** : `S_OK` (0) en cas de succès.

**Exemple (C++)** :

```cpp
int currentTrack = 0;
pVlcSrc->GetAudioTrack(&currentTrack);
printf("Current audio track ID: %d\n", currentTrack);
```

---
#### SetAudioTrack
Définit la piste audio active par ID.
**Syntaxe (C++)** :
```cpp
HRESULT SetAudioTrack(int id);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetAudioTrack(int id);
```
**Paramètres** :
- `id` : l'ID de la piste à activer (obtenu via GetAudioTrackInfo).
**Retour** : `S_OK` (0) en cas de succès, code d'erreur si l'ID de piste est invalide.
**Remarques d'utilisation** :
- Peut être appelée pendant la lecture pour changer de piste dynamiquement
- Utilisez -1 pour désactiver toutes les pistes audio
- Le changement de piste peut provoquer une brève interruption audio
**Exemple (C++)** :
```cpp
// Basculer sur la deuxieme piste audio
int trackId = 0;
pVlcSrc->GetAudioTrackInfo(1, &trackId, nullptr);
pVlcSrc->SetAudioTrack(trackId);
```
**Exemple (C#)** :
```csharp
// Basculer sur la premiere piste audio
int trackId;
var name = new StringBuilder(256);
vlcSrc.GetAudioTrackInfo(0, out trackId, name);
vlcSrc.SetAudioTrack(trackId);
```
---

### Gestion des pistes de sous-titres

#### GetSubtitlesCount

Récupère le nombre total de pistes de sous-titres disponibles.

**Syntaxe (C++)** :

```cpp
HRESULT GetSubtitlesCount(int *count);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int GetSubtitlesCount(out int count);
```

**Paramètres** :

- `count` : [out] reçoit le nombre de pistes de sous-titres.

**Retour** : `S_OK` (0) en cas de succès.

**Exemple (C++)** :

```cpp
int subtitleCount = 0;
pVlcSrc->GetSubtitlesCount(&subtitleCount);
printf("Subtitle tracks: %d\n", subtitleCount);
```

---
#### GetSubtitleInfo
Récupère les informations sur une piste de sous-titres spécifique.
**Syntaxe (C++)** :
```cpp
HRESULT GetSubtitleInfo(int number, int *id, WCHAR *name);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int GetSubtitleInfo(int number, out int id,
                    [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name);
```
**Paramètres** :
- `number` : index de piste à partir de zéro (0 à count-1).
- `id` : [out] reçoit l'ID de la piste de sous-titres.
- `name` : [out] tampon pour recevoir le nom de la piste (minimum 256 caractères).
**Retour** : `S_OK` (0) en cas de succès.
**Exemple (C++)** :
```cpp
int subCount = 0;
pVlcSrc->GetSubtitlesCount(&subCount);
for (int i = 0; i < subCount; i++)
{
    int id = 0;
    WCHAR name[256] = {0};
    pVlcSrc->GetSubtitleInfo(i, &id, name);
    wprintf(L"Subtitle %d - ID: %d, Name: %s\n", i, id, name);
}
```
---

#### GetSubtitle

Récupère l'ID de la piste de sous-titres active actuelle.

**Syntaxe (C++)** :

```cpp
HRESULT GetSubtitle(int *id);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int GetSubtitle(out int id);
```

**Paramètres** :

- `id` : [out] reçoit l'ID de la piste de sous-titres courante.

**Retour** : `S_OK` (0) en cas de succès.

---
#### SetSubtitle
Définit la piste de sous-titres active par ID.
**Syntaxe (C++)** :
```cpp
HRESULT SetSubtitle(int id);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetSubtitle(int id);
```
**Paramètres** :
- `id` : l'ID de la piste de sous-titres à activer.
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- Utilisez -1 pour désactiver les sous-titres
- Le rendu des sous-titres est effectué par le moteur de rendu interne de VLC
- Peut être basculé pendant la lecture
**Exemple (C++)** :
```cpp
// Activer la premiere piste de sous-titres
int subtitleId = 0;
pVlcSrc->GetSubtitleInfo(0, &subtitleId, nullptr);
pVlcSrc->SetSubtitle(subtitleId);
// Desactiver les sous-titres
pVlcSrc->SetSubtitle(-1);
```
---

## Interface IVlcSrc2

Étend `IVlcSrc` avec la prise en charge des paramètres de ligne de commande VLC personnalisés.

### Définition de l'interface

- **Nom de l'interface** : `IVlcSrc2`
- **GUID** : `{CCE122C0-172C-4626-B4B6-42B039E541CB}`
- **Hérite de** : `IVlcSrc`
- **Fichier d'en-tête** : `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)

### Méthodes

#### SetCustomCommandLine

Définit des paramètres de ligne de commande VLC personnalisés.

**Syntaxe (C++)** :

```cpp
HRESULT SetCustomCommandLine(char* params[], int length);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int SetCustomCommandLine([In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] IntPtr[] params_,
                         int size);
```

**Paramètres** :

- `params_` : tableau de pointeurs IntPtr vers des chaînes encodées en UTF-8 contenant les paramètres de ligne de commande VLC.
- `size` : nombre de paramètres dans le tableau.

**Retour** : `S_OK` (0) en cas de succès.

**Remarques d'utilisation** :

- Doit être appelée **avant** de charger le fichier multimédia avec SetFile()
- Les paramètres doivent être convertis en IntPtr UTF-8 natif avec StringHelper.NativeUtf8FromString()
- La mémoire allouée pour les paramètres IntPtr doit être libérée après l'appel via Marshal.FreeHGlobal()
- Les paramètres sont transmis directement à l'initialisation de libVLC
- Les paramètres invalides sont ignorés avec des avertissements dans le journal VLC
- Utilisez la syntaxe standard de ligne de commande VLC (voir la documentation VLC)

**Paramètres VLC courants** :

| Paramètre | Description | Exemple de valeur |
|-----------|-------------|---------------|
| `--network-caching` | Mise en tampon réseau en ms | `1000` |
| `--file-caching` | Mise en tampon fichier en ms | `300` |
| `--live-caching` | Mise en tampon flux en direct en ms | `300` |
| `--avcodec-hw` | Accélération matérielle | `any`, `dxva2`, `d3d11va` |
| `--verbose` | Verbosité de la journalisation | `2` |
| `--rtsp-tcp` | Forcer RTSP via TCP | (option, sans valeur) |
| `--no-audio` | Désactiver l'audio | (option, sans valeur) |
| `--sout-mux-caching` | Mise en tampon du multiplexeur de sortie | `1000` |

**Exemple (C++)** :

```cpp
IVlcSrc2* pVlcSrc2 = nullptr;
pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);

// Configurer pour un RTSP a faible latence
char* params[] = {
    "--network-caching=300",
    "--rtsp-tcp",
    "--avcodec-hw=d3d11va",
    "--verbose=2"
};

pVlcSrc2->SetCustomCommandLine(params, 4);
pVlcSrc2->SetFile(L"rtsp://192.168.1.100/stream");

pVlcSrc2->Release();
```

**Exemple (C#)** :

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VisioForge.Core.Helpers;

var vlcSrc2 = filter as IVlcSrc2;
if (vlcSrc2 != null)
{
    // Activer l'acceleration materielle et ajuster la mise en tampon
    var parameters = new List<string>
    {
        "--avcodec-hw=any",
        "--network-caching=1000",
        "--file-caching=300"
    };

    // Convertir les chaines en tableau IntPtr UTF-8 natif
    var array = new IntPtr[parameters.Count];
    for (int i = 0; i < parameters.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, parameters.Count);
        vlcSrc2.SetFile(@"C:\Videos\movie.mkv");
    }
    finally
    {
        // Liberer la memoire non manageed allouee
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```

**Exemple (Delphi)** :

libVLC analyse la ligne de commande en **UTF-8**, et non dans la page de codes ANSI du système. Les littéraux `PAnsiChar` sont sans danger en ASCII mais corrompent les caractères non-ASCII ; encodez chaque paramètre explicitement en UTF-8 via `UTF8Encode` (ou stockez une `AnsiString` avec `CP_UTF8`). Pour un jeu de paramètres purement ASCII, les littéraux `PAnsiChar` font un aller-retour propre, mais l'extrait ci-dessous utilise le motif sûr.

```delphi
var
  VlcSrc2: IVlcSrc2;
  P0, P1, P2: AnsiString;
  Params: array[0..2] of PAnsiChar;
begin
  if Succeeded(Filter.QueryInterface(IID_IVlcSrc2, VlcSrc2)) then
  begin
    P0 := UTF8Encode('--network-caching=500');
    P1 := UTF8Encode('--rtsp-tcp');
    P2 := UTF8Encode('--avcodec-hw=dxva2');
    Params[0] := PAnsiChar(P0);
    Params[1] := PAnsiChar(P1);
    Params[2] := PAnsiChar(P2);

    VlcSrc2.SetCustomCommandLine(@Params, 3);
    VlcSrc2.SetFile('rtsp://example.com/stream');
  end;
end;
```

---
## Interface IVlcSrc3
Étend `IVlcSrc2` avec la capacité de surcharger la fréquence d'images.
### Définition de l'interface
- **Nom de l'interface** : `IVlcSrc3`
- **GUID** : `{3DFBED0C-E4A8-401C-93EF-CBBFB65223DD}`
- **Hérite de** : `IVlcSrc2`
- **Fichier d'en-tête** : `ivlcsrc.h` (C++), `IVlcSrc.cs` (.NET)
### Méthodes
#### SetDefaultFrameRate
Définit une fréquence d'images par défaut pour les médias sans information de fréquence d'images.
**Syntaxe (C++)** :
```cpp
HRESULT SetDefaultFrameRate(double frameRate);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetDefaultFrameRate(double frameRate);
```
**Paramètres** :
- `frameRate` : fréquence d'images en images par seconde (par ex. 29.97, 30.0, 25.0, 60.0).
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- Doit être appelée **avant** de charger le fichier multimédia
- Utilisée lorsque le média source ne spécifie pas de fréquence d'images
- Particulièrement utile pour les flux réseau sans information de cadencement
- Valeurs courantes : 23.976, 24.0, 25.0, 29.97, 30.0, 50.0, 59.94, 60.0
**Exemple (C++)** :
```cpp
IVlcSrc3* pVlcSrc3 = nullptr;
pFilter->QueryInterface(IID_IVlcSrc3, (void**)&pVlcSrc3);
// Definir la frequence d'images par defaut pour un flux camera IP MJPEG
pVlcSrc3->SetDefaultFrameRate(30.0);
pVlcSrc3->SetFile(L"http://192.168.1.50/video.mjpg");
pVlcSrc3->Release();
```
**Exemple (C#)** :
```csharp
var vlcSrc3 = filter as IVlcSrc3;
if (vlcSrc3 != null)
{
    // Definir la frequence d'images PAL pour un flux DV
    vlcSrc3.SetDefaultFrameRate(25.0);
    vlcSrc3.SetFile(@"dv://0");
}
```
---

## Exemples d'utilisation complets

### Exemple 1 : lecture de film multilingue (C++)

```cpp
#include <dshow.h>
#include "ivlcsrc.h"

void PlayMovieWithAudioSelection(IBaseFilter* pVlcFilter)
{
    HRESULT hr;
    IVlcSrc* pVlcSrc = nullptr;

    hr = pVlcFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
    if (FAILED(hr))
        return;

    // Charger le film
    pVlcSrc->SetFile(L"C:\\Movies\\multilang_movie.mkv");

    // Construire et lancer le graphe ici...
    // (IGraphBuilder::RenderFile, IMediaControl::Run, etc.)

    // Enumerer les pistes audio
    int audioCount = 0;
    pVlcSrc->GetAudioTracksCount(&audioCount);

    wprintf(L"Available audio tracks:\n");
    for (int i = 0; i < audioCount; i++)
    {
        int id = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetAudioTrackInfo(i, &id, name);
        wprintf(L"  [%d] %s (ID: %d)\n", i, name, id);
    }

    // Selectionner la piste audio anglaise (en supposant qu'il s'agit de la piste 1)
    int englishTrackId = 0;
    pVlcSrc->GetAudioTrackInfo(1, &englishTrackId, nullptr);
    pVlcSrc->SetAudioTrack(englishTrackId);

    // Activer les sous-titres
    int subCount = 0;
    pVlcSrc->GetSubtitlesCount(&subCount);
    if (subCount > 0)
    {
        int subId = 0;
        pVlcSrc->GetSubtitleInfo(0, &subId, nullptr);
        pVlcSrc->SetSubtitle(subId);
    }

    pVlcSrc->Release();
}
```

### Exemple 2 : flux RTSP à faible latence (C#)

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DirectShowLib;
using VisioForge.Core.Helpers;
using VisioForge.DirectShowAPI;

public class VLCRTSPPlayer
{
    public void SetupLowLatencyRTSP(IBaseFilter vlcFilter)
    {
        // Obtenir l'interface IVlcSrc3 (version la plus elevee)
        var vlcSrc3 = vlcFilter as IVlcSrc3;
        if (vlcSrc3 == null)
            throw new NotSupportedException("IVlcSrc3 not available");

        // Configurer VLC pour une latence minimale
        var parameters = new List<string>
        {
            "--network-caching=50",       // Tampon reseau minimal
            "--live-caching=50",          // Tampon en direct minimal
            "--rtsp-tcp",                 // Utiliser le transport TCP
            "--no-audio-time-stretch",    // Desactiver l'etirement audio
            "--avcodec-hw=d3d11va",      // Decodage materiel
            "--verbose=0"                 // Reduire la journalisation
        };

        // Convertir en tableau IntPtr
        var array = new IntPtr[parameters.Count];
        for (int i = 0; i < parameters.Count; i++)
        {
            array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
        }

        try
        {
            int hr = vlcSrc3.SetCustomCommandLine(array, parameters.Count);
            DsError.ThrowExceptionForHR(hr);

            // Definir la frequence d'images pour la camera IP
            hr = vlcSrc3.SetDefaultFrameRate(25.0);
            DsError.ThrowExceptionForHR(hr);

            // Charger le flux RTSP
            hr = vlcSrc3.SetFile("rtsp://admin:password@192.168.1.100:554/stream1");
            DsError.ThrowExceptionForHR(hr);
        }
        finally
        {
            // Liberer la memoire allouee
            for (int i = 0; i < array.Length; i++)
            {
                Marshal.FreeHGlobal(array[i]);
            }
        }

        // Construire le graphe de filtres et demarrer la lecture...
    }
}
```

### Exemple 3 : interface de basculement des sous-titres (Delphi)

```delphi
unit VLCSubtitles;

interface

uses
  Winapi.Windows, System.Classes, Vcl.Controls, Vcl.StdCtrls,
  DSPack, ivlcsrc;

type
  TSubtitleForm = class(TForm)
    ComboBoxSubtitles: TComboBox;
    procedure FormCreate(Sender: TObject);
    procedure ComboBoxSubtitlesChange(Sender: TObject);
  private
    FVlcSrc: IVlcSrc;
    FSubtitleIDs: TArray<Integer>;
    procedure LoadSubtitleTracks;
  public
    procedure SetVLCFilter(Filter: IBaseFilter);
  end;

implementation

procedure TSubtitleForm.SetVLCFilter(Filter: IBaseFilter);
begin
  if Succeeded(Filter.QueryInterface(IID_IVlcSrc, FVlcSrc)) then
  begin
    LoadSubtitleTracks;
  end;
end;

procedure TSubtitleForm.LoadSubtitleTracks;
var
  Count, I, ID: Integer;
  Name: array[0..255] of WideChar;
begin
  ComboBoxSubtitles.Clear;
  ComboBoxSubtitles.Items.Add('Disabled');

  if FVlcSrc.GetSubtitlesCount(Count) = S_OK then
  begin
    SetLength(FSubtitleIDs, Count + 1);
    FSubtitleIDs[0] := -1; // Desactive

    for I := 0 to Count - 1 do
    begin
      if FVlcSrc.GetSubtitleInfo(I, ID, Name) = S_OK then
      begin
        FSubtitleIDs[I + 1] := ID;
        ComboBoxSubtitles.Items.Add(Name);
      end;
    end;
  end;

  ComboBoxSubtitles.ItemIndex := 0;
end;

procedure TSubtitleForm.ComboBoxSubtitlesChange(Sender: TObject);
var
  Index: Integer;
begin
  Index := ComboBoxSubtitles.ItemIndex;
  if (Index >= 0) and (Index < Length(FSubtitleIDs)) then
  begin
    FVlcSrc.SetSubtitle(FSubtitleIDs[Index]);
  end;
end;

end.
```

## Bonnes pratiques

### Gestion des pistes

1. **Énumérez toujours les pistes après avoir construit le graphe de filtres** — les informations de piste ne sont pas disponibles tant que la source n'est pas chargée
2. **Gérez avec élégance les fichiers sans audio/sous-titres** — vérifiez le compteur avant d'accéder aux pistes
3. **Pré-allouez les tampons de nom avec 256 caractères** — évite les dépassements de tampon
4. **Mettez en cache les IDs de piste** — n'appelez pas répétitivement GetAudioTrackInfo/GetSubtitleInfo

### Configuration VLC

1. **Utilisez IVlcSrc3 lorsque c'est disponible** — fournit l'ensemble complet des fonctionnalités
2. **Définissez les paramètres personnalisés avant de charger le fichier** — les paramètres ne s'appliquent qu'à l'initialisation
3. **Testez les paramètres VLC indépendamment** — utilisez la ligne de commande VLC pour vérifier que les paramètres fonctionnent
4. **Utilisez des valeurs de mise en tampon appropriées** :
   - Fichiers locaux : 300 ms
   - Flux réseau : 1000-3000 ms
   - Flux à faible latence : 50-300 ms

### Accélération matérielle

1. **Activez le décodage matériel pour H.264/H.265** :

   ```cpp
   "--avcodec-hw=any"  // Detection automatique de la meilleure methode
   ```

2. **Options spécifiques à la plateforme** :
   - Windows : `d3d11va`, `dxva2`
   - Toutes plateformes : `any` (détection automatique)

### Performance

1. **Minimisez la mise en tampon réseau pour les flux en direct** — réduit la latence
2. **Utilisez RTSP sur TCP lorsque UDP échoue** — plus fiable à travers les pare-feu
3. **Activez la journalisation détaillée uniquement pour le débogage** — réduit la charge sur les performances

## Interfaces associées

- **IFileSourceFilter** — interface DirectShow standard alternative pour le chargement de fichiers
- **IAMStreamSelect** — standard DirectShow pour la sélection de flux (également pris en charge par le filtre VLC)
- **IMediaSeeking** — contrôle de positionnement dans le média
- **IBasicVideo** — contrôle de la fenêtre vidéo

## Voir aussi

- [Présentation du filtre source VLC](index.md)
- [Documentation de ligne de commande VLC](https://www.videolan.org/doc/)
- [Exemples de code](examples.md)
