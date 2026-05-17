---
title: Encodage WMV en .NET — Guide de sortie Windows Media Video
description: Implémentez l'encodage Windows Media Video en .NET avec configuration audio/vidéo, options de streaming et gestion multiplateforme des profils.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Encoding
  - Editing
  - Screen Capture
  - MP4
  - WMV
  - WMA
  - C#
primary_api_classes:
  - WMVOutput
  - WMVMode
  - WMVStreamMode
  - VideoCaptureCore

---

# Encodeurs Windows Media Video

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Cette documentation couvre les capacités d'encodage Windows Media Video (WMV) disponibles dans VisioForge, y compris les solutions spécifiques à Windows et multiplateformes.

## Sortie réservée à Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La classe [WMVOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WMVOutput.html) fournit des capacités d'encodage Windows Media complètes pour l'audio et la vidéo sur les plateformes Windows.

### Guide de démarrage rapide

#### Capture vidéo simple avec les paramètres par défaut

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;

var captureCore = new VideoCaptureCore();

// Utiliser les paramètres WMV par défaut (mode profil interne)
captureCore.Output_Format = new WMVOutput();
captureCore.Output_Filename = "output.wmv";

await captureCore.StartAsync();
```

#### Édition vidéo simple avec les paramètres par défaut

```csharp
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

var editCore = new VideoEditCore();

// Utiliser les paramètres WMV par défaut
editCore.Output_Format = new WMVOutput();
editCore.Output_Filename = "edited_output.wmv";

// Ajouter les fichiers d'entrée et configurer l'édition...

await editCore.StartAsync();
```

#### Exemple de paramètres personnalisés

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;

var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Configuration vidéo
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 90,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    
    // Configuration audio
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 90,
    Custom_Audio_Format = "48kHz 16bit Stereo"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = wmvOutput;
captureCore.Output_Filename = "custom_output.wmv";

await captureCore.StartAsync();
```

### Fonctionnalités d'encodage audio

La classe `WMVOutput` offre plusieurs options de configuration spécifiques à l'audio :

- Sélection de codec audio personnalisé
- Personnalisation du format audio
- Plusieurs modes de flux
- Contrôle du débit binaire
- Paramètres de qualité
- Prise en charge des langues
- Gestion de la taille du tampon

### Modes de contrôle de débit

L'encodage WMV prend en charge quatre modes de contrôle de débit via l'énumération `WMVStreamMode` :

1. CBR (débit constant)
2. VBRQuality (débit variable basé sur la qualité)
3. VBRBitrate (débit variable avec débit cible)
4. VBRPeakBitrate (débit variable avec contrainte de débit de crête)

### Modes de configuration

L'encodeur peut être configuré de plusieurs façons à l'aide de l'énumération `WMVMode` :

- ExternalProfile : charger les paramètres depuis un fichier de profil
- ExternalProfileFromText : charger les paramètres depuis une chaîne de texte
- InternalProfile : utiliser des profils intégrés
- CustomSettings : configuration manuelle
- V8SystemProfile : utiliser les profils système Windows Media 8

### Exemple de code

Créez une nouvelle configuration de sortie WMV personnalisée :

```csharp
var wmvOutput = new WMVOutput
{
    // Configuration de base
    Mode = WMVMode.CustomSettings,
    
    // Paramètres audio
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 98,
    Custom_Audio_PeakBitrate = 192000,
    Custom_Audio_PeakBufferSize = 3,
    
    // Paramètre de langue optionnel
    Custom_Audio_LanguageID = "en-US"
};
```

Utilisation d'un profil interne :

```csharp
var profileWmvOutput = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 for Local Network (768 kbps)"
};
```

Configuration du streaming réseau :

```csharp
var streamingWmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Network_Streaming_WMV_Maximum_Clients = 20,
    Custom_Audio_Mode = WMVStreamMode.CBR
};
```

### Configuration de profil personnalisé

Les profils personnalisés offrent la flexibilité maximale en vous permettant de configurer chaque aspect du processus d'encodage. Voici plusieurs exemples pour différents scénarios :

#### Comprendre les propriétés de paramètres personnalisés WMV

Avant de plonger dans les exemples, il est important de comprendre les propriétés clés disponibles dans la classe `WMVOutput` pour la configuration personnalisée :

**Propriétés vidéo :**
- `Custom_Video_StreamPresent` (bool) : active le flux vidéo dans la sortie
- `Custom_Video_Codec` (string) : spécifie le codec vidéo (par exemple « Windows Media Video 9 »)
- `Custom_Video_Mode` (WMVStreamMode) : mode de contrôle de débit (CBR, VBRQuality, VBRBitrate, VBRPeakBitrate)
- `Custom_Video_Bitrate` (int) : débit cible en bits par seconde
- `Custom_Video_Quality` (byte) : niveau de qualité (0-100) pour le mode VBR quality
- `Custom_Video_Width` (int) : largeur vidéo de sortie en pixels
- `Custom_Video_Height` (int) : hauteur vidéo de sortie en pixels
- `Custom_Video_SizeSameAsInput` (bool) : utiliser les dimensions vidéo d'entrée
- `Custom_Video_FrameRate` (double) : fréquence d'images de sortie
- `Custom_Video_KeyFrameInterval` (byte) : nombre d'images entre les keyframes
- `Custom_Video_Smoothness` (byte) : niveau de lissage (0-100)
- `Custom_Video_Peak_BitRate` (int) : débit de crête pour le mode VBR peak
- `Custom_Video_Peak_BufferSizeSeconds` (byte) : fenêtre de tampon de crête en secondes
- `Custom_Video_Buffer_Size` (int) : taille du tampon en millisecondes
- `Custom_Video_Buffer_UseDefault` (bool) : utiliser les paramètres de tampon par défaut
- `Custom_Video_TVSystem` (WMVTVSystem) : standard de système TV (NTSC, PAL)

**Propriétés audio :**
- `Custom_Audio_StreamPresent` (bool) : active le flux audio dans la sortie
- `Custom_Audio_Codec` (string) : spécifie le codec audio (par exemple « Windows Media Audio 9.2 »)
- `Custom_Audio_Format` (string) : spécification de format (par exemple « 48kHz 16bit Stereo »)
- `Custom_Audio_Mode` (WMVStreamMode) : mode de contrôle de débit
- `Custom_Audio_Quality` (byte) : niveau de qualité (0-100) pour le mode VBR quality
- `Custom_Audio_PeakBitrate` (int) : débit de crête en bits par seconde
- `Custom_Audio_PeakBufferSize` (byte) : fenêtre de tampon de crête en secondes
- `Custom_Audio_LanguageID` (string) : identifiant de langue (par exemple « en-US »)

**Métadonnées de profil :**
- `Custom_Profile_Name` (string) : nom du profil pour identification
- `Custom_Profile_Description` (string) : description détaillée de l'objectif du profil
- `Custom_Profile_Language` (string) : identifiant de langue du profil

#### Configuration de streaming vidéo haute qualité

Parfait pour les applications de streaming professionnelles nécessitant une excellente qualité visuelle :

```csharp
var highQualityConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres vidéo - 1080p haute qualité
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 95,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 4,
    Custom_Video_Smoothness = 80,
    Custom_Video_Buffer_UseDefault = false,
    Custom_Video_Buffer_Size = 4000,
    
    // Paramètres audio - stéréo haute qualité
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 98,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    Custom_Audio_PeakBitrate = 320000,
    Custom_Audio_PeakBufferSize = 3,
    
    // Métadonnées de profil
    Custom_Profile_Name = "High Quality Streaming",
    Custom_Profile_Description = "1080p streaming profile with high quality audio",
    Custom_Profile_Language = "en-US"
};

// Appliquer à VideoCaptureCore
var captureCore = new VideoCaptureCore();
captureCore.Output_Format = highQualityConfig;
captureCore.Output_Filename = "output_hq.wmv";

// Ou appliquer à VideoEditCore
var editCore = new VideoEditCore();
editCore.Output_Format = highQualityConfig;
editCore.Output_Filename = "output_hq.wmv";
```

#### Configuration faible bande passante pour streaming mobile

Optimisée pour les appareils mobiles à bande passante limitée :

```csharp
var mobileLowBandwidthConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres vidéo optimisés pour mobile
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 800000, // 800 kbps
    Custom_Video_Width = 854,
    Custom_Video_Height = 480,
    Custom_Video_FrameRate = 24.0,
    Custom_Video_KeyFrameInterval = 5,
    Custom_Video_Smoothness = 60,
    Custom_Video_Buffer_UseDefault = true,
    
    // Paramètres audio pour faible bande passante
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 64000, // 64 kbps
    Custom_Audio_Format = "44kHz 16bit Mono",
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Mobile Low Bandwidth",
    Custom_Profile_Description = "480p optimized for mobile devices"
};

// Appliquer à VideoCaptureCore
var captureCore = new VideoCaptureCore();
captureCore.Output_Format = mobileLowBandwidthConfig;
captureCore.Output_Filename = "output_mobile.wmv";
```

#### Configuration axée sur l'audio pour le contenu musical

Audio haute qualité avec un traitement vidéo minimal :

```csharp
var audioFocusedConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres audio haute qualité
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2 Professional",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 99,
    Custom_Audio_Format = "96kHz 24bit Stereo",
    Custom_Audio_PeakBitrate = 512000,
    Custom_Audio_PeakBufferSize = 4,
    Custom_Audio_LanguageID = "en-US",
    
    // Paramètres vidéo minimaux
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRBitrate,
    Custom_Video_Bitrate = 500000,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 25.0,
    Custom_Video_KeyFrameInterval = 10,
    Custom_Video_Buffer_UseDefault = true,
    
    Custom_Profile_Name = "Audio Focus",
    Custom_Profile_Description = "High quality audio configuration for music content"
};

// Appliquer à VideoEditCore pour traiter des fichiers audio avec vidéo
var editCore = new VideoEditCore();
editCore.Output_Format = audioFocusedConfig;
editCore.Output_Filename = "output_audio_focus.wmv";
```

#### Débit constant (CBR) pour le streaming

Le mode CBR est idéal pour le streaming réseau où une bande passante constante est requise :

```csharp
var cbrStreamingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres vidéo avec CBR
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 2000000, // 2 Mbps constant
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 3,
    Custom_Video_Buffer_Size = 3000,
    Custom_Video_Buffer_UseDefault = false,
    
    // Paramètres audio avec CBR
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 128000, // 128 kbps constant
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    // Configuration du streaming réseau
    Network_Streaming_WMV_Maximum_Clients = 50,
    
    Custom_Profile_Name = "CBR Streaming",
    Custom_Profile_Description = "Constant bitrate for reliable network streaming"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = cbrStreamingConfig;
captureCore.Output_Filename = "output_cbr_stream.wmv";
```

#### Débit variable avec contrôle de crête

Le VBR avec contrainte de débit de crête fournit une optimisation de la qualité tout en limitant la bande passante maximale :

```csharp
var vbrPeakConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres vidéo avec contrôle de débit de crête
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRPeakBitrate,
    Custom_Video_Bitrate = 3000000, // 3 Mbps moyen
    Custom_Video_Peak_BitRate = 5000000, // 5 Mbps crête
    Custom_Video_Peak_BufferSizeSeconds = 3,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 4,
    Custom_Video_Smoothness = 75,
    
    // Paramètres audio avec contrôle de crête
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRPeakBitrate,
    Custom_Audio_PeakBitrate = 256000,
    Custom_Audio_PeakBufferSize = 2,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    Custom_Profile_Name = "VBR Peak Control",
    Custom_Profile_Description = "Variable bitrate with peak constraints for quality and bandwidth balance"
};

var editCore = new VideoEditCore();
editCore.Output_Format = vbrPeakConfig;
editCore.Output_Filename = "output_vbr_peak.wmv";
```

#### Configuration optimisée pour l'enregistrement d'écran

Optimisée pour la capture d'écran avec encodage efficace du contenu statique :

```csharp
var screenRecordingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres vidéo optimisés pour le contenu d'écran
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Screen",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 90,
    Custom_Video_SizeSameAsInput = true, // Utiliser la résolution d'écran
    Custom_Video_FrameRate = 15.0, // Fréquence d'images plus faible pour l'enregistrement d'écran
    Custom_Video_KeyFrameInterval = 10,
    Custom_Video_Smoothness = 50,
    Custom_Video_Buffer_UseDefault = true,
    
    // Paramètres audio pour la narration vocale
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 85,
    Custom_Audio_Format = "44kHz 16bit Mono", // Mono pour la voix
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Screen Recording",
    Custom_Profile_Description = "Optimized for screen capture with efficient compression"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = screenRecordingConfig;
captureCore.Output_Filename = "screen_recording.wmv";
```

#### Configuration qualité archivage

Qualité maximale à des fins d'archivage :

```csharp
var archivalConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres vidéo pour qualité maximale
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 100,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 1, // Chaque image est un keyframe
    Custom_Video_Smoothness = 100,
    Custom_Video_Buffer_Size = 8000,
    Custom_Video_Buffer_UseDefault = false,
    
    // Paramètres audio pour qualité maximale
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2 Lossless",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 100,
    Custom_Audio_Format = "96kHz 24bit Stereo",
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Archival Quality",
    Custom_Profile_Description = "Maximum quality for long-term storage",
    Custom_Profile_Language = "en-US"
};

var editCore = new VideoEditCore();
editCore.Output_Format = archivalConfig;
editCore.Output_Filename = "archival_quality.wmv";
```

### Utilisation de profil interne

Les profils internes fournissent des paramètres préconfigurés optimisés pour les scénarios courants. Voici des exemples d'utilisation de différents profils internes :

#### Codecs et formats disponibles

Avant de configurer des paramètres personnalisés, il est utile de comprendre les codecs et formats disponibles :

**Codecs vidéo :**
- « Windows Media Video 9 » - codec WMV9 standard
- « Windows Media Video 9 Advanced Profile » - prise en charge des fonctionnalités avancées
- « Windows Media Video 9 Screen » - optimisé pour le contenu d'écran
- « Windows Media Video 8 » - codec WMV8 historique

**Codecs audio :**
- « Windows Media Audio 9.2 » - codec WMA standard
- « Windows Media Audio 9.2 Professional » - audio haute qualité
- « Windows Media Audio 9.2 Lossless » - compression sans perte
- « Windows Media Audio Voice 9 » - optimisé pour la parole

**Formats audio :**
Chaînes de format courantes pour la propriété `Custom_Audio_Format` :
- « 48kHz 16bit Stereo » - stéréo qualité CD
- « 44kHz 16bit Stereo » - stéréo qualité standard
- « 44kHz 16bit Mono » - mono qualité standard
- « 96kHz 24bit Stereo » - audio haute résolution
- « 22kHz 16bit Mono » - qualité d'enregistrement vocal

Profil de qualité diffusion standard :

```csharp
var broadcastProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Advanced Profile",
    Custom_Video_TVSystem = WMVTVSystem.NTSC  // Remplacement optionnel du système TV
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = broadcastProfile;
captureCore.Output_Filename = "broadcast_output.wmv";
```

Profil de streaming Web :

```csharp
var webStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 for Broadband (2 Mbps)",
    Network_Streaming_WMV_Maximum_Clients = 100  // Remplacement de streaming optionnel
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = webStreamingProfile;
captureCore.Output_Filename = "web_stream.wmv";
```

Profil basse latence pour le streaming en direct :

```csharp
var liveStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Screen (Low Rate)",
    Network_Streaming_WMV_Maximum_Clients = 50
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = liveStreamingProfile;
captureCore.Output_Filename = "live_stream.wmv";
```

### Configuration de profil externe

Les profils externes vous permettent de charger des paramètres d'encodage depuis des fichiers ou du texte. C'est utile pour partager des configurations entre différents projets ou stocker plusieurs configurations :

Chargement de profil depuis un fichier :

```csharp
var fileBasedProfile = new WMVOutput
{
    Mode = WMVMode.ExternalProfile,
    External_Profile_FileName = @"C:\Profiles\HighQualityStreaming.prx"
};
```

Chargement de profil depuis une configuration texte :

```csharp
var textBasedProfile = new WMVOutput
{
    Mode = WMVMode.ExternalProfileFromText,
    External_Profile_Text = @"
        <profile version=""589824"" 
                 storageformat=""1"" 
                 name=""Custom Streaming Profile"" 
                 description=""High quality streaming profile"">
            <streamconfig majortype=""{73647561-0000-0010-8000-00AA00389B71}"" 
                         streamnumber=""1"" 
                         streamname=""Audio Stream"" 
                         inputname=""Audio409"" 
                         bitrate=""128000"" 
                         bufferwindow=""5000"" 
                         reliabletransport=""0"" 
                         decodercomplexity="""" 
                         rfc1766langid=""en-us""/>
            <!-- Configuration de profil supplémentaire -->
        </profile>"
};
```

Enregistrement et chargement de profils par programmation :

```csharp
async Task SaveAndLoadProfile(WMVOutput profile, string filename)
{
    // Enregistrer la configuration du profil au format JSON
    string jsonConfig = profile.Save();
    await File.WriteAllTextAsync(filename, jsonConfig);
    
    // Charger la configuration du profil depuis JSON
    string loadedJson = await File.ReadAllTextAsync(filename);
    WMVOutput loadedProfile = WMVOutput.Load(loadedJson);
}
```

Exemple d'utilisation de l'enregistrement/chargement de profil :

```csharp
var profile = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configurer les paramètres ...
};

await SaveAndLoadProfile(profile, "encoding_profile.json");
```

### Utilisation des profils historiques Windows Media 8

Pour la compatibilité avec les anciens systèmes, vous pouvez utiliser les profils système Windows Media 8 :

Utilisation d'un profil Windows Media 8 :

```csharp
var wmv8Profile = new WMVOutput
{
    Mode = WMVMode.V8SystemProfile,
    V8ProfileName = "Windows Media Video 8 for Dial-up Access (28.8 Kbps)",
};
```

Personnalisation des paramètres de streaming pour les profils Windows Media 8 :

```csharp
var wmv8StreamingProfile = new WMVOutput
{
    Mode = WMVMode.V8SystemProfile,
    V8ProfileName = "Windows Media Video 8 for Local Area Network (384 Kbps)",
    Network_Streaming_WMV_Maximum_Clients = 25,
    Custom_Video_TVSystem = WMVTVSystem.PAL  // Remplacement optionnel du système TV
};
```

### Appliquer les paramètres à votre objet principal

```csharp
var core = new VideoCaptureCore(); // ou VideoEditCore
core.Output_Format = wmvOutput;
core.Output_Filename = "output.wmv";
```

### Exemple de travail complet

Voici un exemple complet montrant la capture vidéo avec des paramètres WMV personnalisés, incluant une initialisation et une gestion d'erreurs correctes :

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace WMVCaptureExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialiser le SDK VisioForge
            VisioForgeX.InitSDK();
            
            // Créer une instance VideoCaptureCore
            var captureCore = new VideoCaptureCore();
            
            try
            {
                // Configurer la source vidéo (première caméra disponible)
                var videoDevices = captureCore.Video_CaptureDevices();
                if (videoDevices.Length > 0)
                {
                    captureCore.Video_CaptureDevice = new VideoCaptureSource(videoDevices[0].Name);
                }
                
                // Configurer la source audio (premier microphone disponible)
                var audioDevices = captureCore.Audio_CaptureDevices();
                if (audioDevices.Length > 0)
                {
                    captureCore.Audio_CaptureDevice = new AudioCaptureSource(audioDevices[0].Name);
                }
                
                // Configurer la sortie WMV avec des paramètres personnalisés
                var wmvOutput = new WMVOutput
                {
                    Mode = WMVMode.CustomSettings,
                    
                    // Paramètres vidéo
                    Custom_Video_StreamPresent = true,
                    Custom_Video_Codec = "Windows Media Video 9",
                    Custom_Video_Mode = WMVStreamMode.VBRQuality,
                    Custom_Video_Quality = 85,
                    Custom_Video_Width = 1280,
                    Custom_Video_Height = 720,
                    Custom_Video_FrameRate = 30.0,
                    Custom_Video_KeyFrameInterval = 5,
                    
                    // Paramètres audio
                    Custom_Audio_StreamPresent = true,
                    Custom_Audio_Codec = "Windows Media Audio 9.2",
                    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
                    Custom_Audio_Quality = 90,
                    Custom_Audio_Format = "48kHz 16bit Stereo",
                    
                    Custom_Profile_Name = "Standard Capture",
                    Custom_Profile_Description = "Standard quality capture profile"
                };
                
                // Appliquer les paramètres de sortie
                captureCore.Output_Format = wmvOutput;
                captureCore.Output_Filename = "capture_output.wmv";
                captureCore.Mode = VideoCaptureMode.VideoCapture;
                
                // Démarrer la capture
                Console.WriteLine("Starting video capture...");
                await captureCore.StartAsync();
                
                Console.WriteLine("Recording... Press any key to stop.");
                Console.ReadKey();
                
                // Arrêter la capture
                Console.WriteLine("Stopping video capture...");
                await captureCore.StopAsync();
                
                Console.WriteLine($"Video saved to: capture_output.wmv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Nettoyer
                captureCore?.Dispose();
                VisioForgeX.DestroySDK();
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

### Exemple complet d'édition vidéo

Voici un exemple complet pour l'édition vidéo avec des paramètres WMV personnalisés :

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEdit;

namespace WMVEditExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialiser le SDK VisioForge
            VisioForgeX.InitSDK();
            
            var editCore = new VideoEditCore();
            
            try
            {
                // Ajouter les fichiers vidéo d'entrée
                editCore.Input_AddVideoFile("input_video1.mp4", false);
                editCore.Input_AddVideoFile("input_video2.mp4", false);
                
                // Configurer la sortie WMV
                var wmvOutput = new WMVOutput
                {
                    Mode = WMVMode.CustomSettings,
                    
                    // Paramètres vidéo haute qualité
                    Custom_Video_StreamPresent = true,
                    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
                    Custom_Video_Mode = WMVStreamMode.VBRPeakBitrate,
                    Custom_Video_Bitrate = 4000000, // 4 Mbps moyen
                    Custom_Video_Peak_BitRate = 6000000, // 6 Mbps crête
                    Custom_Video_Peak_BufferSizeSeconds = 3,
                    Custom_Video_Width = 1920,
                    Custom_Video_Height = 1080,
                    Custom_Video_FrameRate = 30.0,
                    Custom_Video_KeyFrameInterval = 4,
                    Custom_Video_Smoothness = 80,
                    
                    // Paramètres audio haute qualité
                    Custom_Audio_StreamPresent = true,
                    Custom_Audio_Codec = "Windows Media Audio 9.2 Professional",
                    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
                    Custom_Audio_Quality = 95,
                    Custom_Audio_Format = "48kHz 16bit Stereo",
                    
                    Custom_Profile_Name = "High Quality Edit",
                    Custom_Profile_Description = "High quality output for edited videos"
                };
                
                // Appliquer les paramètres de sortie
                editCore.Output_Format = wmvOutput;
                editCore.Output_Filename = "edited_output.wmv";
                
                // Configurer le mode d'édition
                editCore.Mode = VideoEditMode.Convert;
                
                // Démarrer l'édition
                Console.WriteLine("Starting video editing...");
                await editCore.StartAsync();
                
                // Surveiller la progression
                while (editCore.State == VideoEditCoreState.Working)
                {
                    var progress = editCore.Progress();
                    Console.WriteLine($"Progress: {progress}%");
                    await Task.Delay(500);
                }
                
                Console.WriteLine($"Video editing complete! Output saved to: edited_output.wmv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Nettoyer
                editCore?.Dispose();
                VisioForgeX.DestroySDK();
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

## Sortie WMV multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La classe `WMVEncoderSettings` fournit une solution multiplateforme pour l'encodage WMV en utilisant la technologie GStreamer.

### Fonctionnalités

- Implémentation indépendante de la plateforme
- Intégration avec le backend GStreamer
- Interface de configuration simple
- Vérification de la disponibilité

### Exemple de code

#### Configuration VideoCaptureCoreX

Ajoutez la sortie WMV à l'instance principale du Video Capture SDK :

```csharp
// Configuration de base avec les paramètres par défaut
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoCaptureCoreX();
core.Outputs_Add(wmvOutput, true);

// Avec paramètres d'encodeur personnalisés
var wmvOutput2 = new WMVOutput("output_custom.wmv");
wmvOutput2.Video = new WMVEncoderSettings();
wmvOutput2.Audio = new WMAEncoderSettings
{
    Bitrate = 192,  // Débit binaire en Kbps
    SampleRate = 48000,  // Fréquence d'échantillonnage en Hz
    Channels = 2  // Stéréo
};

var core2 = new VideoCaptureCoreX();
core2.Outputs_Add(wmvOutput2, true);
```

#### Configuration VideoEditCoreX

Définissez le format de sortie pour l'instance principale du Video Edit SDK :

```csharp
// Configuration de base
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoEditCoreX();
core.Output_Format = wmvOutput;

// Avec paramètres audio personnalisés
var wmvOutput2 = new WMVOutput("output_high_quality.wmv");
wmvOutput2.Audio = new WMAEncoderSettings
{
    Bitrate = 256,  // Audio haute qualité
    SampleRate = 48000,
    Channels = 2
};

var core2 = new VideoEditCoreX();
core2.Output_Format = wmvOutput2;
```

#### Configuration Media Blocks Pipeline

Créez une instance de sortie WMV Media Blocks :

```csharp
// Configurer l'encodeur audio
var wma = new WMAEncoderSettings
{
    Bitrate = 128,  // Débit binaire en Kbps
    SampleRate = 48000,  // 48 kHz
    Channels = 2  // Stéréo
};

// Configurer l'encodeur vidéo  
var wmv = new WMVEncoderSettings();

// Configurer le puits ASF (conteneur)
var sinkSettings = new ASFSinkSettings("output.wmv");

// Créer le bloc de sortie
var wmvOutput = new WMVOutputBlock(sinkSettings, wmv, wma);

// Ajouter au pipeline
var pipeline = new MediaBlocksPipeline();
// ... configurer les sources et connecter à wmvOutput
```

#### Vérification de la disponibilité de l'encodeur

Vérifiez toujours si les encodeurs sont disponibles avant utilisation :

```csharp
// Vérifier la disponibilité de l'encodeur WMV
if (WMVEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMV encoder is available");
    var wmvOutput = new WMVOutput("output.wmv");
    // ... utiliser l'encodeur
}
else
{
    Console.WriteLine("WMV encoder is not available on this system");
    // Se replier sur un encodeur alternatif
}

// Vérifier la disponibilité de l'encodeur WMA
if (WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMA encoder is available");
}
```

#### Configuration multiplateforme avancée

```csharp
// Créer une sortie WMV multiplateforme haute qualité
var wmvOutput = new WMVOutput("output_hq.wmv");

// Configurer l'audio haute qualité
wmvOutput.Audio = new WMAEncoderSettings
{
    Bitrate = 320,  // Qualité maximale
    SampleRate = 48000,
    Channels = 2
};

// Paramètres d'encodeur vidéo (utilise l'encodeur WMV1 par défaut)
wmvOutput.Video = new WMVEncoderSettings();

// Vérifier si un processeur vidéo personnalisé est nécessaire
// wmvOutput.CustomVideoProcessor = myCustomProcessor;

// Appliquer au core
var core = new VideoCaptureCoreX();
core.Outputs_Add(wmvOutput, true);

// Démarrer la capture
await core.StartAsync();
```

#### Paramètres audio disponibles

La classe `WMAEncoderSettings` fournit les options de configuration suivantes :

```csharp
var audioSettings = new WMAEncoderSettings
{
    // Débit binaire en Kbps - valeurs prises en charge : 128, 192, 256, 320
    Bitrate = 192,
    
    // Fréquence d'échantillonnage en Hz - valeurs prises en charge : 44100, 48000
    SampleRate = 48000,
    
    // Nombre de canaux - valeurs prises en charge : 1 (mono), 2 (stéréo)
    Channels = 2
};

// Obtenir les débits binaires pris en charge
int[] supportedBitrates = audioSettings.GetSupportedBitrates();
// Retourne : [128, 192, 256, 320]

// Obtenir les fréquences d'échantillonnage prises en charge
int[] supportedSampleRates = audioSettings.GetSupportedSampleRates();
// Retourne : [44100, 48000]

// Obtenir les nombres de canaux pris en charge
int[] supportedChannels = audioSettings.GetSupportedChannelCounts();
// Retourne : [1, 2]
```

### Choisir entre les encodeurs

Considérez les facteurs suivants lors du choix entre `WMVOutput` spécifique à Windows et `WMVEncoderSettings` multiplateforme :

#### WMVOutput spécifique à Windows

- Avantages :
  - Accès complet aux fonctionnalités du format Windows Media
  - Options de contrôle de débit avancées
  - Prise en charge du streaming réseau
  - Configuration basée sur les profils
- Inconvénients :
  - Compatibilité Windows uniquement
  - Nécessite des composants Windows Media

#### WMV multiplateforme

- Avantages :
  - Indépendance de plateforme
  - Implémentation plus simple
- Inconvénients :
  - Ensemble de fonctionnalités plus limité
  - Options de configuration de base uniquement

## Bonnes pratiques

### Références MSDN

Pour des informations détaillées sur les technologies Windows Media, consultez ces ressources officielles Microsoft :

- [Windows Media Format SDK](https://learn.microsoft.com/es-es/windows/win32/wmformat/windows-media-format-11-sdk) - Documentation complète Windows Media Format
- [Working with Profiles](https://learn.microsoft.com/en-us/windows/win32/wmformat/working-with-profiles) - Gestion et configuration des profils
- [Windows Media Codecs](https://learn.microsoft.com/en-us/windows/win32/medfound/windows-media-codecs) - Informations sur les codecs audio et vidéo
- [ASF File Structure](https://learn.microsoft.com/en-us/windows/win32/medfound/asf-file-structure) - Détails du conteneur Advanced Systems Format
- [Configuring Video Streams](https://learn.microsoft.com/en-us/windows/win32/wmformat/configuring-video-streams) - Paramètres d'encodage vidéo
- [Configuring Audio Streams](https://learn.microsoft.com/en-us/windows/win32/wmformat/configuring-audio-streams) - Paramètres d'encodage audio

### Choisir les modes de contrôle de débit

Sélectionnez le mode de contrôle de débit approprié en fonction de votre cas d'usage :

1. **CBR (débit constant)**
   - À utiliser pour : le streaming réseau, la radiodiffusion
   - Avantages : bande passante prévisible, qualité constante
   - Inconvénients : compression moins efficace, peut ne pas s'adapter à la complexité du contenu
   - Exemple : streaming en direct pour assurer une lecture fluide

2. **VBRQuality (débit variable - qualité)**
   - À utiliser pour : sortie en fichier, archivage, vidéo haute qualité
   - Avantages : meilleur rapport qualité/taille, s'adapte à la complexité du contenu
   - Inconvénients : taille de fichier et débit imprévisibles
   - Exemple : enregistrement de tutoriels ou de présentations pour lecture ultérieure

3. **VBRBitrate (débit variable - débit cible)**
   - À utiliser pour : lorsque vous avez besoin d'optimisation de la qualité avec contraintes de taille
   - Avantages : équilibre la qualité et la taille de fichier cible
   - Inconvénients : la qualité peut varier entre les scènes
   - Exemple : création de vidéos pour téléversement avec limites de taille

4. **VBRPeakBitrate (débit variable - contrainte de crête)**
   - À utiliser pour : streaming avec contraintes de bande passante
   - Avantages : optimisation de la qualité avec plafond de bande passante
   - Inconvénients : configuration plus complexe
   - Exemple : scénarios de streaming adaptatif

### Optimisation des performances

1. **Configuration du tampon**
   - Définissez `Custom_Video_Buffer_UseDefault = false` pour un contrôle ajusté
   - Augmentez `Custom_Video_Buffer_Size` pour un streaming plus fluide (par défaut : 3000 ms)
   - Équilibrez la taille du tampon avec les exigences de latence

2. **Intervalle de keyframe**
   - Valeurs inférieures (1-3) : meilleures performances de positionnement, taille de fichier plus grande
   - Valeurs supérieures (5-10) : taille de fichier plus petite, moins de précision de positionnement
   - Recommandé : 3-5 pour le streaming, 10+ pour l'enregistrement d'écran

3. **Paramètres de lissage**
   - 0-50 : prioriser l'efficacité de compression
   - 50-75 : qualité et efficacité équilibrées
   - 75-100 : prioriser la qualité visuelle

### Recommandations de résolution et de fréquence d'images

```csharp
// Configuration 4K/UHD
var uhd4KConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 95,
    Custom_Video_Width = 3840,
    Custom_Video_Height = 2160,
    Custom_Video_FrameRate = 30.0,
    // ... autres paramètres
};

// Configuration Full HD
var fullHDConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    // ... autres paramètres
};

// Configuration HD Ready
var hdReadyConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    // ... autres paramètres
};

// Configuration SD
var sdConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 720,
    Custom_Video_Height = 480,
    Custom_Video_FrameRate = 29.97,
    Custom_Video_TVSystem = WMVTVSystem.NTSC,
    // ... autres paramètres
};
```

### Gestion des erreurs et validation

Validez toujours votre configuration avant de démarrer la capture ou l'édition :

```csharp
var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configuration
};

try
{
    var captureCore = new VideoCaptureCore();
    captureCore.Output_Format = wmvOutput;
    captureCore.Output_Filename = "output.wmv";
    
    // Valider la configuration
    if (captureCore.Output_Filename == null || captureCore.Output_Filename.Length == 0)
    {
        throw new InvalidOperationException("Output filename is required");
    }
    
    await captureCore.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    // Gérer l'erreur de manière appropriée
}
```

### Configuration du streaming réseau

Pour les scénarios de streaming réseau, configurez à la fois les paramètres d'encodeur et de streaming :

```csharp
var streamingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Paramètres vidéo optimisés pour le streaming
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 1500000, // 1.5 Mbps
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 3,
    Custom_Video_Buffer_Size = 2000, // Tampon plus petit pour latence réduite
    Custom_Video_Buffer_UseDefault = false,
    
    // Paramètres audio
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 128000,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    // Paramètres de streaming réseau
    Network_Streaming_WMV_Maximum_Clients = 100,
    
    Custom_Profile_Name = "Network Streaming",
    Custom_Profile_Description = "Optimized for network streaming with low latency"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = streamingConfig;
captureCore.Output_Filename = "http://localhost:8080/stream"; // Ou chemin de fichier
```

### Test et validation

1. **Testez toujours votre configuration** avec un contenu d'exemple avant utilisation en production
2. **Surveillez les performances d'encodage** pour garantir la capacité d'encodage en temps réel
3. **Vérifiez la compatibilité des fichiers** avec vos appareils de lecture cibles
4. **Validez la synchronisation audio** en particulier avec des fréquences d'images personnalisées
5. **Testez le streaming réseau** dans diverses conditions de bande passante

### Gestion des profils

Enregistrez et réutilisez les configurations pour la cohérence :

```csharp
// Enregistrer la configuration au format JSON
var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configuration
};

string jsonConfig = wmvOutput.Save();
File.WriteAllText("profile_high_quality.json", jsonConfig);

// Charger la configuration depuis JSON
string loadedJson = File.ReadAllText("profile_high_quality.json");
var loadedProfile = WMVOutput.Load(loadedJson);

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = loadedProfile;
```

### Problèmes courants et solutions

1. **Tailles de fichier importantes**
   - Utilisez le mode VBRBitrate au lieu de VBRQuality
   - Réduisez la qualité ou la résolution vidéo
   - Augmentez KeyFrameInterval
   - Envisagez d'utiliser le codec d'écran pour les enregistrements d'écran

2. **Qualité médiocre**
   - Augmentez le paramètre de qualité vidéo
   - Utilisez un débit binaire plus élevé
   - Passez en mode VBRQuality
   - Assurez-vous d'une taille de tampon suffisante

3. **Problèmes de streaming**
   - Utilisez le mode CBR pour une bande passante constante
   - Réduisez la taille du tampon pour une latence plus faible
   - Testez avec un nombre approprié de clients
   - Surveillez la bande passante du réseau

4. **Codec non disponible**
   - Assurez-vous que les composants Windows Media sont installés
   - Vérifiez l'énumération des codecs par programmation
   - Repli sur les profils internes par défaut
   - Envisagez des alternatives multiplateformes (WMVEncoderSettings)

---
Consultez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
