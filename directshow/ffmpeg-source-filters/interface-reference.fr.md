---
title: Référence API du filtre source DirectShow FFmpeg — COM
description: Interface IFFmpegSourceSettings avec accélération matérielle, modes de mise en tampon, options FFmpeg personnalisées et rappels pour DirectShow.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Decoding
  - RTSP
  - C#
primary_api_classes:
  - IFFmpegSourceSettings
  - IFileSourceFilter
  - IFFMPEGSourceSettings

---

# Référence de l'interface IFFMPEGSourceSettings

## Vue d'ensemble

L'interface `IFFMPEGSourceSettings` fournit des options de configuration avancées pour le filtre source DirectShow FFMPEG. Cette interface permet aux développeurs de contrôler l'accélération matérielle, le comportement de mise en tampon, les options FFmpeg personnalisées et divers rappels pour la lecture multimédia.

> Les en-têtes .NET / C++ / Delphi nomment cette interface `IFFMPEGSourceSettings` (tout en majuscules `FFMPEG`). Les chemins d'en-tête hérités `IFFmpegSourceSettings.h` / `.cs` conservent le nom de fichier historique à casse mixte — mais le symbole exposé par les trois langages est la forme tout en majuscules. Les deux sont des identifiants équivalents en C# ; cette page utilise la forme tout en majuscules par cohérence avec examples.md / index.md.

## Définition de l'interface

- **Nom de l'interface** : `IFFMPEGSourceSettings`
- **CLSID du filtre interrogé** : `{1974D893-83E4-4F89-9908-795C524CC17E}` (le filtre source FFMPEG — l'IID de l'interface est distinct ; consultez l'en-tête).
- **Hérite de** : `IUnknown`

### Fichiers de définition de l'interface

Les définitions complètes de l'interface sont disponibles sur GitHub :

- **C# (.NET)** : [IFFmpegSourceSettings.cs](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/dotnet/IFFmpegSourceSettings.cs)
- **En-tête C++** : [IFFmpegSourceSettings.h](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/cpp/FFMPEG%20Source/IFFmpegSourceSettings.h)
- **Delphi** : [VCFiltersAPI.pas](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/delphi/VCFiltersAPI.pas) (recherchez `IFFMPEGSourceSettings`)

Toutes les définitions d'interface incluent :

- Signatures de méthodes complètes avec les attributs de marshalling appropriés
- Définitions de délégués de rappel
- Types énumérés (modes de mise en tampon, types de média)
- Documentation d'utilisation et exemples

## Référence des méthodes

### Accélération matérielle

#### GetHWAccelerationEnabled

Récupère l'état actuel de l'accélération matérielle.

**Syntaxe (C++)** :

```cpp
BOOL GetHWAccelerationEnabled();
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
bool GetHWAccelerationEnabled();
```

**Retour** : `TRUE` si l'accélération matérielle est activée, `FALSE` sinon.

**Valeur par défaut** : `TRUE`

---
#### SetHWAccelerationEnabled
Active ou désactive l'accélération matérielle du décodage vidéo.
**Syntaxe (C++)** :
```cpp
HRESULT SetHWAccelerationEnabled(BOOL enabled);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetHWAccelerationEnabled([MarshalAs(UnmanagedType.Bool)] bool enabled);
```
**Paramètres** :
- `enabled` : définir sur `TRUE` pour activer l'accélération matérielle, `FALSE` pour la désactiver.
**Retour** : `S_OK` (0) en cas de succès, code d'erreur sinon.
**Remarques d'utilisation** :
- Doit être appelée **avant** de connecter les filtres vidéo en aval
- Lorsqu'elle est activée, le filtre tente d'utiliser le décodage matériel (DXVA, NVDEC, QuickSync, etc.)
- Bascule sur le décodage logiciel si l'accélération matérielle n'est pas disponible
- L'accélération matérielle améliore significativement les performances pour les codecs H.264, H.265, VP9 et AV1
**Exemple (C++)** :
```cpp
IFFmpegSourceSettings* pSettings = nullptr;
pFilter->QueryInterface(IID_IFFmpegSourceSettings, (void**)&pSettings);
// Activer l'acceleration materielle
pSettings->SetHWAccelerationEnabled(TRUE);
pSettings->Release();
```
**Exemple (C#)** :
```csharp
var settings = filter as IFFmpegSourceSettings;
if (settings != null)
{
    // Activer l'acceleration materielle
    settings.SetHWAccelerationEnabled(true);
}
```
---

### Configuration du délai de chargement

#### GetLoadTimeOut

Récupère la valeur actuelle du délai de chargement de la source.

**Syntaxe (C++)** :

```cpp
DWORD GetLoadTimeOut();
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
uint GetLoadTimeOut();
```

**Retour** : valeur du délai en millisecondes.

**Valeur par défaut** : `15000` (15 secondes)

---
#### SetLoadTimeOut
Définit la durée du délai pour les opérations de chargement de source.
**Syntaxe (C++)** :
```cpp
HRESULT SetLoadTimeOut(DWORD milliseconds);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetLoadTimeOut(uint milliseconds);
```
**Paramètres** :
- `milliseconds` : durée du délai en millisecondes.
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- Doit être appelée **avant** de charger le fichier source/URL
- Particulièrement importante pour les flux réseau pouvant avoir des temps de connexion lents
- Définissez des valeurs plus élevées pour les connexions réseau lentes ou les fichiers volumineux
- Définissez des valeurs plus basses pour échouer rapidement sur des sources injoignables
**Exemple (C++)** :
```cpp
// Definir un delai de 30 secondes pour les flux reseau
pSettings->SetLoadTimeOut(30000);
// Charger le flux RTSP
IFileSourceFilter* pFileSource = nullptr;
pFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
pFileSource->Load(L"rtsp://example.com/stream", nullptr);
```
---

### Configuration de la mise en tampon

#### GetBufferingMode

Récupère le mode de mise en tampon actuel.

**Syntaxe (C++)** :

```cpp
FFMPEG_SOURCE_BUFFERING_MODE GetBufferingMode();
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
FFMPEG_SOURCE_BUFFERING_MODE GetBufferingMode();
```

**Retour** : mode de mise en tampon actuel (voir l'énumération ci-dessous).

**Valeur par défaut** : `FFMPEG_SOURCE_BUFFERING_MODE_AUTO`

---
#### SetBufferingMode
Définit le mode de mise en tampon pour les sources en direct.
**Syntaxe (C++)** :
```cpp
HRESULT SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE mode);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE mode);
```
**Paramètres** :
- `mode` : mode de mise en tampon à utiliser.
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- Doit être appelée **avant** de charger la source
- Affecte la latence et la stabilité pour les flux en direct
**Modes de mise en tampon** :
| Mode | Valeur | Description | Cas d'usage |
|------|-------|-------------|----------|
| `FFMPEG_SOURCE_BUFFERING_MODE_AUTO` | 0 | Détection automatique si la mise en tampon est nécessaire | Par défaut — recommandé pour la plupart des scénarios |
| `FFMPEG_SOURCE_BUFFERING_MODE_ON` | 1 | Forcer la mise en tampon activée | À utiliser pour les flux réseau instables |
| `FFMPEG_SOURCE_BUFFERING_MODE_OFF` | 2 | Forcer la mise en tampon désactivée | À utiliser pour les flux en direct à faible latence |
**Exemple (C++)** :
```cpp
// Desactiver la mise en tampon pour un flux RTSP a faible latence
pSettings->SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE_OFF);
pSettings->SetLoadTimeOut(5000); // Delai de 5 secondes
```
**Exemple (C#)** :
```csharp
// Activer la mise en tampon pour un reseau instable
settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.ON);
```
---

### Options FFmpeg personnalisées

#### SetCustomOption

Définit une option FFmpeg personnalisée pour le démultiplexeur ou le décodeur.

**Syntaxe (C++)** :

```cpp
HRESULT SetCustomOption(LPSTR name, LPSTR value);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int SetCustomOption([MarshalAs(UnmanagedType.LPStr)] string name,
                     [MarshalAs(UnmanagedType.LPStr)] string value);
```

**Paramètres** :

- `name` : nom de l'option (chaîne ASCII).
- `value` : valeur de l'option (chaîne ASCII).

**Retour** : `S_OK` (0) en cas de succès.

**Remarques d'utilisation** :

- Doit être appelée **avant** de charger la source
- Permet de transmettre n'importe quelle option AVFormatContext ou AVCodecContext de FFmpeg
- Les options sont transmises directement aux bibliothèques FFmpeg
- Les options invalides sont ignorées avec un avertissement

**Options courantes** :

| Option | Valeur | Description |
|--------|-------|-------------|
| `rtsp_transport` | `tcp` ou `udp` | Force le protocole de transport RTSP |
| `timeout` | Microsecondes | Délai réseau pour les protocoles |
| `buffer_size` | Octets | Taille du tampon d'entrée |
| `analyzeduration` | Microsecondes | Durée d'analyse du flux |
| `probesize` | Octets | Taille des données à sonder |
| `fflags` | `nobuffer` | Désactiver la mise en tampon |
| `threads` | Nombre | Nombre de threads de décodeur |

**Exemple (C++)** :

```cpp
// Configurer RTSP pour utiliser le transport TCP
pSettings->SetCustomOption("rtsp_transport", "tcp");

// Definir le delai reseau a 5 secondes
pSettings->SetCustomOption("timeout", "5000000"); // 5 secondes en microsecondes

// Augmenter la taille de sondage pour une meilleure detection de format
pSettings->SetCustomOption("probesize", "10000000"); // 10 Mo
```

**Exemple (C#)** :

```csharp
// Configuration faible latence
settings.SetCustomOption("fflags", "nobuffer");
settings.SetCustomOption("flags", "low_delay");
settings.SetCustomOption("probesize", "32");
```

---
#### ClearCustomOptions
Efface toutes les options personnalisées précédemment définies.
**Syntaxe (C++)** :
```cpp
HRESULT ClearCustomOptions();
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int ClearCustomOptions();
```
**Retour** : `S_OK` (0) en cas de succès.
**Remarques d'utilisation** :
- Doit être appelée **avant** de charger la source
- Rétablit toutes les options personnalisées aux valeurs par défaut de FFmpeg
**Exemple (C++)** :
```cpp
pSettings->ClearCustomOptions();
```
---

### Configuration des rappels

#### SetDataCallback

Définit une fonction de rappel pour recevoir des données vidéo/audio décodées.

**Syntaxe (C++)** :

```cpp
HRESULT SetDataCallback(FFMPEGDataCallbackDelegate callback);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int SetDataCallback([MarshalAs(UnmanagedType.FunctionPtr)] FFMPEGDataCallbackDelegate callback);
```

**Paramètres** :

- `callback` : pointeur vers la fonction de rappel.

**Retour** : `S_OK` (0) en cas de succès.

**Signature du rappel (C++)** :

```cpp
typedef HRESULT(_stdcall* FFMPEGDataCallbackDelegate) (
    BYTE* buffer,        // Pointeur vers le tampon de donnees
    int bufferLen,       // Longueur du tampon en octets
    int dataType,        // 0 = video, 1 = audio
    LONGLONG startTime,  // Horodatage de debut (unites de 100 nanosecondes)
    LONGLONG stopTime    // Horodatage de fin (unites de 100 nanosecondes)
);
```

**Remarques d'utilisation** :

- Le rappel est invoqué pour chaque image/échantillon audio décodé
- Appelé depuis le thread de streaming du filtre — maintenez le traitement minimal
- Les données du tampon ne sont valides que pendant l'exécution du rappel
- Renvoyez `S_OK` depuis le rappel pour continuer le traitement

**Exemple (C++)** :

```cpp
HRESULT __stdcall DataCallback(BYTE* buffer, int bufferLen, int dataType,
                                LONGLONG startTime, LONGLONG stopTime)
{
    if (dataType == 0) // Video
    {
        // Traiter l'image video
        ProcessVideoFrame(buffer, bufferLen, startTime);
    }
    else // Audio
    {
        // Traiter les donnees audio
        ProcessAudioData(buffer, bufferLen);
    }
    return S_OK;
}

// Definir le rappel
pSettings->SetDataCallback(&DataCallback);
```

---
#### SetTimestampCallback
Définit une fonction de rappel pour recevoir des informations d'horodatage.
**Syntaxe (C++)** :
```cpp
HRESULT SetTimestampCallback(FFMPEGTimestampCallbackDelegate callback);
```
**Syntaxe (C#)** :
```csharp
[PreserveSig]
int SetTimestampCallback([MarshalAs(UnmanagedType.FunctionPtr)] FFMPEGTimestampCallbackDelegate callback);
```
**Paramètres** :
- `callback` : pointeur vers la fonction de rappel.
**Retour** : `S_OK` (0) en cas de succès.
**Signature du rappel (C++)** :
```cpp
typedef HRESULT(_stdcall* FFMPEGTimestampCallbackDelegate) (
    int mediaType,              // 0 = video, 1 = audio
    __int64 demuxerStartTime,   // Heure de demarrage du demuxer
    __int64 streamStartTime,    // Heure de demarrage du flux
    __int64 timestamp           // Horodatage courant
);
```
**Remarques d'utilisation** :
- Utile pour l'analyse d'horodatage et le débogage de synchronisation
- Appelé pour chaque image/échantillon décodé
---

### Contrôle audio

#### SetAudioEnabled

Active ou désactive le traitement du flux audio.

**Syntaxe (C++)** :

```cpp
HRESULT SetAudioEnabled(BOOL enabled);
```

**Syntaxe (C#)** :

```csharp
[PreserveSig]
int SetAudioEnabled([MarshalAs(UnmanagedType.Bool)] bool enabled);
```

**Paramètres** :

- `enabled` : définir sur `TRUE` pour activer l'audio, `FALSE` pour le désactiver.

**Retour** : `S_OK` (0) en cas de succès.

**Remarques d'utilisation** :

- Doit être appelée **avant** de charger la source
- Lorsqu'elle est désactivée, les flux audio ne sont pas décodés (économise CPU/mémoire)
- Utile pour les applications vidéo uniquement

**Exemple (C++)** :

```cpp
// Desactiver l'audio pour un traitement video uniquement
pSettings->SetAudioEnabled(FALSE);
```

## Interfaces associées

- **IFileSourceFilter** — interface DirectShow standard pour le chargement de fichiers/URL
- **IAMStreamSelect** — sélectionner entre plusieurs flux audio/vidéo
- **IMediaSeeking** — se positionner à un endroit spécifique du média
- **IAMStreamConfig** — configurer le format vidéo/audio

## Voir aussi

### Documentation

- [Présentation du filtre source FFMPEG](index.md) — vue d'ensemble du produit et fonctionnalités
- [Exemples de code](examples.md) — exemples de code fonctionnels complets

### Définitions d'interface

- [Interface C# (.NET)](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/dotnet/IFFmpegSourceSettings.cs) — définition complète de l'interface .NET
- [En-tête C++ de l'interface](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/cpp/FFMPEG%20Source/IFFmpegSourceSettings.h) — fichier d'en-tête C++
- [Interface Delphi](https://github.com/visioforge/directshow-samples/blob/main/Interfaces/delphi/VCFiltersAPI.pas) — définition de l'interface Delphi

### Exemples fonctionnels

- [Référentiel d'exemples GitHub](https://github.com/visioforge/directshow-samples) — exemples fonctionnels complets pour toutes les plateformes

### Ressources externes

- [Documentation FFmpeg](https://ffmpeg.org/documentation.html) — documentation de la bibliothèque FFmpeg
- [SDK DirectShow](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow) — documentation Microsoft DirectShow
