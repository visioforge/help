---
title: Diffusion réseau WMV dans des applications .NET — guide SDK
description: Implémentez le streaming Windows Media Video en .NET avec algorithmes de compression, débits adaptatifs et optimisation de bande passante.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Streaming
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - WMVOutput
  - WMVMode
  - NetworkStreamingFormat

---

# Guide d'implémentation de la diffusion réseau Windows Media Video (WMV)

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction à la technologie de streaming WMV

Windows Media Video (WMV) représente une technologie de streaming polyvalente et puissante développée par Microsoft. En tant que composant intégral du framework Windows Media, WMV s'est imposé comme une solution fiable pour diffuser efficacement du contenu vidéo sur les réseaux. Ce format utilise des algorithmes de compression sophistiqués qui réduisent substantiellement la taille des fichiers tout en maintenant une qualité visuelle acceptable, ce qui le rend particulièrement bien adapté aux applications de streaming où l'optimisation de la bande passante est critique.

Le format WMV prend en charge une large gamme de résolutions vidéo et de débits, permettant aux développeurs d'adapter leurs implémentations de streaming aux conditions réseau variées et aux exigences des utilisateurs finaux. Cette adaptabilité fait de WMV un excellent choix pour les applications qui doivent servir des environnements clients divers avec différentes contraintes de connectivité.

## Présentation technique du format WMV

### Fonctionnalités et capacités clés

WMV implémente le conteneur Advanced Systems Format (ASF), qui fournit plusieurs avantages techniques pour les applications de streaming :

- **Compression efficace** : utilise une technologie de codec qui équilibre qualité et taille de fichier
- **Ajustement évolutif du débit** : s'adapte aux conditions de bande passante disponibles
- **Résilience aux erreurs** : mécanismes intégrés de récupération en cas de perte de paquets
- **Protection du contenu** : prend en charge la gestion des droits numériques (DRM) lorsque nécessaire
- **Prise en charge des métadonnées** : permet l'incorporation d'informations descriptives sur le flux

### Spécifications techniques

| Fonctionnalité | Spécification |
|---------|---------------|
| Codec | VC-1 (principalement) |
| Conteneur | ASF (Advanced Systems Format) |
| Résolutions prises en charge | Jusqu'à 4K UHD (selon le profil) |
| Plage de débit | 10 Kbps à 20+ Mbps |
| Prise en charge audio | WMA (Windows Media Audio) |
| Protocoles de streaming | HTTP, RTSP, MMS |

## Implémentation du streaming WMV uniquement Windows

[VideoCaptureCore](#){ .md-button }

Le SDK VisioForge fournit un framework robuste pour implémenter le streaming WMV dans des environnements Windows. Cette implémentation permet aux applications de diffuser de la vidéo sur les réseaux tout en capturant simultanément vers un fichier si souhaité.

### Prérequis d'implémentation

Avant d'implémenter le streaming WMV dans votre application, assurez-vous que les exigences suivantes sont satisfaites :

1. Votre environnement de développement inclut le VisioForge Video Capture SDK
2. Les redistribuables requis sont installés (détails fournis dans la section Déploiement)
3. Votre application cible les systèmes d'exploitation Windows
4. Les ports réseau sont correctement configurés et accessibles

### Guide d'implémentation étape par étape

#### 1. Initialiser le composant de capture vidéo

Commencez par configurer le composant principal de capture vidéo dans votre application :

```cs
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

// Initialiser VideoCaptureCore avec le VideoView qui héberge l'aperçu
var VideoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

// Configurer les paramètres de capture de base (sélection de périphérique, mode, etc.)
// ...
```

#### 2. Activer la diffusion réseau

Pour activer la fonctionnalité de diffusion réseau, vous devez l'activer explicitement et définir le format sur WMV :

```cs
// Activer la diffusion réseau
VideoCapture1.Network_Streaming_Enabled = true;

// Définir le format de diffusion sur WMV
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.WMV;
```

#### 3. Configurer les paramètres de sortie WMV

Créez et configurez un objet de sortie WMV avec les paramètres appropriés :

```cs
// Créer la sortie WMV. Le constructeur par défaut sélectionne le profil interne « Windows Media Video 9 for Local Network
// (768 kbps) » avec Mode = WMVMode.InternalProfile.
var wmvOutput = new WMVOutput();

// Option A : choisir un profil intégré différent
wmvOutput.Mode = WMVMode.InternalProfile;
wmvOutput.Internal_Profile_Name = "Windows Media Video 9 for Broadband (NTSC, 1400 Kbps)";

// Option B : piloter l'encodeur à partir de paramètres personnalisés au lieu d'un profil.
// Notez la nomenclature plate « Custom_* » — WMVOutput n'a pas d'objet Profile imbriqué.
// Bitrate est en bits/s ; KeyFrameInterval est en secondes entre keyframes ;
// Quality est un octet de 0 à 100.
wmvOutput.Mode = WMVMode.CustomSettings;
wmvOutput.Custom_Video_StreamPresent = true;
wmvOutput.Custom_Video_Bitrate = 2_000_000;      // 2 Mbps
wmvOutput.Custom_Video_KeyFrameInterval = 3;     // secondes
wmvOutput.Custom_Video_Quality = 85;             // 0..100
wmvOutput.Custom_Video_SizeSameAsInput = true;
wmvOutput.Custom_Audio_StreamPresent = true;

// Limiter le nombre de clients simultanés (sur WMVOutput, pas sur VideoCaptureCore)
wmvOutput.Network_Streaming_WMV_Maximum_Clients = 25;

// Câbler la sortie WMV dans le pipeline de diffusion réseau
VideoCapture1.Network_Streaming_Output = wmvOutput;

// Le port sur lequel le serveur Windows Media écoute
VideoCapture1.Network_Streaming_Network_Port = 12345;
```

#### 4. Démarrer le processus de diffusion

Une fois tout configuré, vous pouvez démarrer le processus de diffusion :

```cs
// Démarrer le processus de diffusion
try
{
    await VideoCapture1.StartAsync();

    // L'URL de diffusion est désormais disponible pour les clients
    string streamingUrl = VideoCapture1.Network_Streaming_URL;

    // Afficher ou journaliser l'URL de diffusion pour les connexions clientes
    Console.WriteLine($"Diffusion disponible à : {streamingUrl}");
}
catch (Exception ex)
{
    // Gérer toutes les exceptions lors de l'initialisation de la diffusion
    Console.WriteLine($"Erreur de diffusion : {ex.Message}");
}
```

### Options de configuration avancées

#### Fichiers de profil .prx externes

Pour un contrôle fin au-delà des profils intégrés, pointez WMVOutput vers un fichier de profil
créé avec Windows Media Profile Editor :

```cs
wmvOutput.Mode = WMVMode.ExternalProfile;
wmvOutput.External_Profile_FileName = @"C:\profiles\my-stream.prx";

// Ou collez le XML du profil en ligne :
// wmvOutput.Mode = WMVMode.ExternalProfileFromText;
// wmvOutput.External_Profile_Text = "<profile ...>...</profile>";

VideoCapture1.Network_Streaming_Output = wmvOutput;
```

## Implémentation de connexion côté client

Les clients peuvent se connecter au flux WMV à l'aide de Windows Media Player ou de toute application qui prend en charge le protocole de streaming Windows Media. L'URL de connexion suit ce format :

```
http://[server_ip]:[port]/
```

Par exemple :
```
http://192.168.1.100:12345/
```

### Exemple de code de connexion client

Pour des connexions programmatiques au flux WMV dans des applications clientes :

```cs
// Connexion client au flux WMV à l'aide du contrôle Windows Media Player
using System.Windows.Forms;

public partial class StreamViewerForm : Form
{
    public StreamViewerForm(string streamUrl)
    {
        InitializeComponent();
        
        // En supposant que vous avez un contrôle Windows Media Player nommé 'wmPlayer' sur votre formulaire
        wmPlayer.URL = streamUrl;
        wmPlayer.Ctlcontrols.play();
    }
}
```

## Optimisation des performances

Lors de l'implémentation du streaming réseau WMV, envisagez ces stratégies d'optimisation :

1. **Ajuster le débit en fonction des conditions réseau** : débits plus faibles pour les réseaux contraints
2. **Équilibrer les intervalles de keyframes** : des keyframes fréquentes améliorent les performances de seek mais augmentent la bande passante
3. **Surveiller l'utilisation CPU** : l'encodage WMV peut être intensif en CPU ; ajustez les paramètres de qualité en conséquence
4. **Implémenter la détection de la qualité réseau** : adaptez dynamiquement les paramètres de diffusion
5. **Tenir compte des paramètres de tampon** : des tampons plus grands améliorent la stabilité mais augmentent la latence

## Dépannage des problèmes courants

| Problème | Solution possible |
|-------|-------------------|
| Échecs de connexion | Vérifiez que le port réseau est ouvert dans les paramètres du pare-feu |
| Mauvaise qualité vidéo | Augmentez le débit ou ajustez les paramètres de compression |
| Utilisation CPU élevée | Réduisez la résolution ou la fréquence d'images |
| Mise en tampon côté client | Ajustez les paramètres de fenêtre de tampon ou réduisez le débit |
| Erreurs d'authentification | Vérifiez les identifiants côté serveur et côté client |

## Exigences de déploiement

### Redistribuables requis

Pour déployer avec succès des applications utilisant la fonctionnalité de streaming WMV, incluez les paquets redistribuables suivants :

- Redist Video capture [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Commandes d'installation

À l'aide de NuGet Package Manager :

```
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

Ou pour les systèmes 32 bits :

```
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
```

## Conclusion

Le streaming réseau WMV fournit un moyen fiable de diffuser du contenu vidéo sur les réseaux dans les environnements Windows. Le SDK VisioForge simplifie l'implémentation grâce à son API complète tout en donnant aux développeurs un contrôle fin sur les paramètres de diffusion. En suivant les lignes directrices de ce document, vous pouvez créer des applications de streaming robustes qui diffusent du contenu vidéo de haute qualité à plusieurs clients simultanément.

Pour des implémentations plus avancées et des exemples de code supplémentaires, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
