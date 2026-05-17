---
title: Guide d'intégration de l'encodeur Windows Media Audio (WMA)
description: Implémentez l'encodage audio WMA en .NET avec des approches multiplateformes et spécifiques à Windows, contrôles de débit et configuration de codec.
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
  - Capture
  - Streaming
  - Encoding
  - Editing
  - MP3
  - WMA
  - C#
primary_api_classes:
  - WMAOutput
  - WMAEncoderSettings
  - VideoCaptureCore
  - VideoEditCore
  - VideoCaptureCoreX

---

# Encodeur Windows Media Audio

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Windows Media Audio (WMA) est un codec audio populaire développé par Microsoft pour une compression audio efficace. Cette documentation couvre les implémentations d'encodeur WMA disponibles dans les SDK .Net de VisioForge.

## Vue d'ensemble

Le SDK VisioForge fournit deux approches distinctes pour l'encodage WMA : la classe [WMAOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WMAOutput.html) spécifique à la plateforme pour les environnements Windows et la classe multiplateforme [WMAEncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.WMAEncoderSettings.html). Explorons les deux implémentations en détail pour comprendre leurs capacités et cas d'usage.

## Sortie WMA multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

`WMAEncoderSettings` fournit une solution multiplateforme pour l'encodage WMA. Cette implémentation est construite sur le SDK et offre un comportement cohérent sur différents systèmes d'exploitation.

### Caractéristiques principales

L'encodeur prend en charge les configurations audio suivantes :

- Fréquences d'échantillonnage : 44,1 kHz et 48 kHz
- Débits : 128, 192, 256 et 320 Kbps
- Configurations de canaux : mono (1) et stéréo (2)

### Contrôle de débit

L'encodeur WMA implémente l'encodage à débit constant (CBR), vous permettant de spécifier un débit fixe parmi les valeurs prises en charge. Cela garantit une qualité audio cohérente et des tailles de fichier prévisibles tout au long du contenu encodé.

### Exemple d'utilisation

Ajouter la sortie WMA à l'instance du noyau Video Capture SDK :

```csharp
// Créer une instance du noyau Video Capture SDK
var core = new VideoCaptureCoreX();

// Créer une sortie WMA
var wmaOutput = new WMAOutput("output.wma");
wmaOutput.Audio.SampleRate = 48000;
wmaOutput.Audio.Channels = 2;
wmaOutput.Audio.Bitrate = 320;

// Ajouter la sortie WMA
core.Outputs_Add(wmaOutput, true);
```

Définir le format de sortie pour l'instance du noyau Video Edit SDK :

```csharp
// Créer une instance du noyau Video Edit SDK
var core = new VideoEditCoreX();

// Créer une sortie WMA
var wmaOutput = new WMAOutput("output.wma");
wmaOutput.Audio.SampleRate = 48000;
wmaOutput.Audio.Channels = 2;
wmaOutput.Audio.Bitrate = 320;

// Ajouter la sortie WMA
core.Output_Format = wmaOutput;
```

Créer une instance de sortie WMA Media Blocks :

```csharp
// Créer une instance des paramètres de l'encodeur WMA
var wmaSettings = new WMAEncoderSettings();

// Créer une instance de sortie WMA
var wmaOutput = new WMAEncoderBlock(wmaSettings);

// Créer une instance de sortie ASF
var asfOutput = new ASFSinkBlock(new ASFSinkSettings("output.wma"));

// Connecter l'encodeur WMA à la sortie ASF
pipeline.Connect(wmaOutput.Output, asfOutput.Input); // pipeline est MediaBlocksPipeline
```

Vérifiez si l'encodage WMA est disponible avant de construire le pipeline :

```csharp
if (!WMAEncoderSettings.IsAvailable())
{
   // Gérer l'erreur — le plugin d'encodeur WMA est manquant sur cette plateforme.
}
```

## Sortie WMA Windows uniquement

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La classe `WMAOutput` fournit une implémentation Windows complète avec des fonctionnalités et options de configuration avancées. Cette implémentation tire parti du Windows Media Format SDK pour des performances optimales sur les systèmes Windows.

### Caractéristiques principales

L'implémentation spécifique à Windows offre :

- Prise en charge de plusieurs profils (interne, externe et personnalisé)
- Paramètres de langue et de localisation
- Encodage basé sur la qualité
- Contrôle avancé du débit avec paramètres de débit de pointe
- Configuration de la taille du tampon

### Modes de configuration

L'implémentation Windows classique sélectionne sa source d'encodeur via l'énumération `WMVMode` sur `WMAOutput.Mode` :

- `WMVMode.InternalProfile` — choisit un profil Windows Media prédéfini par son nom (le plus simple).
- `WMVMode.ExternalProfile` — charge un fichier de profil `.prx` depuis le disque.
- `WMVMode.ExternalProfileFromText` — passe le XML de profil en ligne sous forme de chaîne.
- `WMVMode.CustomSettings` — pilote manuellement tous les paramètres de l'encodeur (qualité, débit de pointe, tampon, etc.) via les propriétés `Custom_Audio_*`.
- `WMVMode.V8SystemProfile` — utilise un profil système Windows Media Video 8 pour la compatibilité avec les anciens systèmes Windows Media (efficacité de compression inférieure à WMV9 ; uniquement pour cibler les systèmes hérités).

Le contrôle de débit (CBR / VBR / VBR par qualité) s'exprime via ces propriétés `Custom_Audio_*` ou est intégré au profil choisi — ce n'est pas une énumération distincte.

### Exemple d'utilisation

Voici comment configurer l'encodeur WMA spécifique à Windows :

Utilisez un profil interne pour une configuration simple

```csharp
var wmaOutput = new WMAOutput
{
    // Utiliser un profil interne pour une configuration simple
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Audio 9 High (192K)"
};

core.Output_Format = wmaOutput; // Core est VideoCaptureCore ou VideoEditCore
```

Ou configurez des paramètres personnalisés

```csharp
var wmaOutput = new WMAOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Quality = 98,        // Réglage de haute qualité
    Custom_Audio_PeakBitrate = 320,   // Débit maximum en Kbps
    Custom_Audio_PeakBufferSize = 3   // Taille de tampon pour le streaming
};

core.Output_Format = wmaOutput; // Core est VideoCaptureCore ou VideoEditCore
```

### Gestion des profils

L'implémentation Windows prend en charge trois modes de profil :

1. Profils internes :
   - Profils préconfigurés pour les cas d'usage courants
   - Accès via `Internal_Profile_Name`

2. Profils externes :
   - Chargement de profils depuis des fichiers externes
   - Configuration via `External_Profile_FileName` ou `External_Profile_Text`

3. Profils personnalisés :
   - Contrôle fin sur les paramètres d'encodage
   - Configuration via les propriétés Custom_*

## Bonnes pratiques

Lors de l'implémentation de l'encodage WMA dans votre application :

1. Pour les applications Windows nécessitant des fonctionnalités avancées :
   - Utilisez WMAOutput pour accéder aux optimisations spécifiques à Windows
   - Envisagez de sauvegarder les configurations en JSON pour réutilisation
   - Implémentez une gestion d'erreurs appropriée pour le chargement des profils

2. Pour les applications multiplateformes :
   - Tenez-vous-en à WMAEncoderSettings pour un comportement cohérent
   - Vérifiez les fréquences prises en charge avant de définir la configuration
   - Utilisez la fréquence d'échantillonnage et le débit pris en charge les plus élevés pour la meilleure qualité

Cette documentation fournit une base pour implémenter l'encodage WMA dans vos applications. Le choix entre les implémentations multiplateformes et spécifiques à Windows doit se baser sur les exigences de votre application en matière de prise en charge des plateformes, fonctionnalités d'encodage et contrôle de qualité.
