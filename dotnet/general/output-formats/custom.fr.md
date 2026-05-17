---
title: Formats vidéo personnalisés DirectShow en .NET — guide SDK
description: Implémentez des formats de sortie vidéo personnalisés avec filtres DirectShow pour pipelines spécialisés et configuration de codecs en .NET.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - Capture
  - Streaming
  - Encoding
  - Editing
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - CustomOutput
  - VideoCaptureCore
  - VideoEditCore
  - VideoCaptureMode

---

# Création de formats de sortie vidéo personnalisés avec des filtres DirectShow

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Présentation

Le travail avec la vidéo dans les applications .NET nécessite souvent des formats de sortie personnalisés pour répondre à des exigences spécifiques de projet. Les SDK VisioForge offrent de puissantes capacités pour implémenter des sorties à format personnalisé à l'aide de filtres DirectShow, donnant aux développeurs un contrôle précis sur les pipelines de traitement audio et vidéo.

Ce guide présente des techniques pratiques pour implémenter des formats de sortie personnalisés qui fonctionnent de manière transparente avec Video Capture SDK .NET et Video Edit SDK .NET, vous permettant d'adapter vos applications vidéo à des spécifications exactes.

## Pourquoi utiliser des formats de sortie personnalisés ?

Les formats de sortie personnalisés offrent plusieurs avantages aux développeurs .NET :

- Prise en charge de codecs vidéo spécialisés non disponibles dans les formats standards
- Contrôle fin des paramètres de compression vidéo et audio
- Intégration avec des filtres DirectShow tiers
- Possibilité de créer des formats de sortie propriétaires ou spécifiques à un secteur d'activité
- Optimisation pour des cas d'usage spécifiques (streaming, archivage, édition)

## Prise en main de CustomOutput

La classe `CustomOutput` est la pierre angulaire pour configurer les paramètres de sortie personnalisés dans les SDK VisioForge. Cette classe vous permet de définir et de configurer les filtres utilisés dans votre pipeline de traitement vidéo.

Commencez par initialiser une nouvelle instance :

```cs
var customOutput = new CustomOutput();
```

Bien que nos exemples utilisent la classe `VideoCaptureCore`, les développeurs utilisant Video Edit SDK .NET peuvent appliquer les mêmes techniques avec `VideoEditCore`.

## Stratégies d'implémentation

Il existe deux approches principales pour implémenter une sortie à format personnalisé avec des filtres DirectShow :

### Stratégie 1 : pipeline à trois composants

Cette approche modulaire divise le pipeline de traitement en trois composants distincts :

1. Codec audio
2. Codec vidéo
3. Multiplexeur (conteneur de format de fichier)

Cette séparation procure une flexibilité maximale et un contrôle complet sur chaque étape du processus. Vous pouvez utiliser soit des filtres DirectShow standards, soit des codecs spécialisés pour les composants audio et vidéo.

#### Récupération des codecs disponibles

Commencez par remplir votre interface utilisateur avec les codecs et filtres disponibles :

```cs
// Remplir les options de codec vidéo
foreach (string codec in VideoCapture1.Video_Codecs)
{
    videoCodecDropdown.Items.Add(codec);
}

// Remplir les options de codec audio
foreach (string codec in VideoCapture1.Audio_Codecs)
{
    audioCodecDropdown.Items.Add(codec);
}

// Obtenir tous les filtres DirectShow disponibles
foreach (string filter in VideoCapture1.DirectShow_Filters)
{
    directShowAudioFilters.Items.Add(filter);
    directShowVideoFilters.Items.Add(filter);
    multiplexerFilters.Items.Add(filter);
    fileWriterFilters.Items.Add(filter);
}
```

#### Configuration des composants du pipeline

Ensuite, configurez vos composants de traitement vidéo et audio en fonction des sélections de l'utilisateur :

```cs
// Configurer le codec vidéo
if (useStandardVideoCodec.Checked)
{
    customOutput.Video_Codec = videoCodecDropdown.Text;
    customOutput.Video_Codec_UseFiltersCategory = false;
}
else
{
    customOutput.Video_Codec = directShowVideoFilters.Text;
    customOutput.Video_Codec_UseFiltersCategory = true;
}

// Configurer le codec audio
if (useStandardAudioCodec.Checked)
{
    customOutput.Audio_Codec = audioCodecDropdown.Text;
    customOutput.Audio_Codec_UseFiltersCategory = false;
}
else
{
    customOutput.Audio_Codec = directShowAudioFilters.Text;
    customOutput.Audio_Codec_UseFiltersCategory = true;
}

// Configurer le multiplexeur
customOutput.MuxFilter_Name = multiplexerFilters.Text;
customOutput.MuxFilter_IsEncoder = false;
```

#### Configuration d'un écrivain de fichier personnalisé

Pour les sorties spécialisées qui requièrent un écrivain de fichier dédié :

```cs
// Activer l'écrivain de fichier spécial si nécessaire
customOutput.SpecialFileWriter_Needed = useCustomFileWriter.Checked;
customOutput.SpecialFileWriter_FilterName = fileWriterFilters.Text;
```

Cette approche vous offre un contrôle granulaire sur chaque étape du processus d'encodage, ce qui la rend idéale pour les exigences de sortie complexes.

### Stratégie 2 : filtre tout-en-un

Cette approche simplifiée utilise un seul filtre DirectShow qui combine les fonctionnalités du multiplexeur, du codec vidéo et du codec audio. Le SDK gère intelligemment la détection des capacités du filtre, en déterminant s'il :

- Peut écrire directement des fichiers sans assistance
- Nécessite le filtre File Writer DirectShow standard
- A besoin d'un filtre d'écrivain de fichier spécialisé

L'implémentation est simple :

```cs
// Remplir les options de filtre à partir des filtres DirectShow disponibles
foreach (string filter in VideoCapture1.DirectShow_Filters)
{
    filterDropdown.Items.Add(filter);
}

// Configurer le filtre tout-en-un
customOutput.MuxFilter_Name = selectedFilter.Text;
customOutput.MuxFilter_IsEncoder = true;

// Configurer un écrivain de fichier spécialisé si requis
customOutput.SpecialFileWriter_Needed = requiresCustomWriter.Checked;
customOutput.SpecialFileWriter_FilterName = fileWriterFilter.Text;
```

Cette approche est plus simple à implémenter mais offre moins de contrôle granulaire sur les composants individuels du processus d'encodage.

## Simplification de la configuration avec une boîte de dialogue UI

Pour une implémentation plus conviviale, VisioForge fournit une boîte de dialogue de paramètres intégrée qui gère la configuration des formats personnalisés :

```cs
// Créer et configurer la boîte de dialogue de paramètres
CustomFormatSettingsDialog settingsDialog = new CustomFormatSettingsDialog(
    VideoCapture1.Video_Codecs.ToArray(),
    VideoCapture1.Audio_Codecs.ToArray(),
    VideoCapture1.DirectShow_Filters.ToArray());

// Appliquer les paramètres à votre instance CustomOutput
settingsDialog.SaveSettings(ref customOutput);
```

Cette boîte de dialogue fournit une interface utilisateur complète pour configurer tous les aspects des formats de sortie personnalisés, économisant du temps de développement tout en offrant une flexibilité totale.

## Mise en œuvre du processus de sortie

Après avoir configuré vos paramètres de format personnalisé, vous devez les appliquer à votre processus de capture ou d'édition :

```cs
// Appliquer la configuration de format personnalisé
VideoCapture1.Output_Format = customOutput;

// Définir le mode de capture
VideoCapture1.Mode = VideoCaptureMode.VideoCapture;

// Spécifier le chemin du fichier de sortie
VideoCapture1.Output_Filename = "output_video.mp4";

// Démarrer le processus de capture ou d'encodage
await VideoCapture1.StartAsync();
```

## Considérations de performance

Lors de l'implémentation de formats de sortie personnalisés, gardez ces conseils de performance à l'esprit :

- L'efficacité et l'utilisation des ressources varient d'un filtre DirectShow à l'autre
- Testez vos combinaisons de filtres avec des médias d'entrée typiques
- Certains filtres tiers peuvent introduire une latence supplémentaire
- Tenez compte de l'utilisation mémoire lors du traitement de vidéo haute résolution
- La compatibilité des filtres peut varier selon les versions de Windows

## Paquets requis

Pour utiliser des filtres DirectShow personnalisés, assurez-vous d'avoir installé les paquets redistribuables appropriés :

### Video Capture SDK .Net

- [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Video Edit SDK .Net

- [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
- [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Dépannage

Les problèmes courants lors du travail avec des filtres DirectShow personnalisés incluent :

- Conflits de compatibilité entre filtres
- Codecs ou dépendances manquants
- Problèmes d'enregistrement des composants COM
- Fuites mémoire dans les filtres tiers
- Goulets d'étranglement de performance avec des graphes de filtres complexes

Si vous rencontrez des problèmes, vérifiez que tous les filtres requis sont correctement enregistrés sur votre système et que vous disposez des dernières versions des filtres et du SDK VisioForge.

## Conclusion

Les formats de sortie personnalisés utilisant des filtres DirectShow offrent de puissantes capacités aux développeurs .NET travaillant avec des applications vidéo. Que vous choisissiez la flexibilité d'un pipeline à trois composants ou la simplicité d'un filtre tout-en-un, les SDK VisioForge vous donnent les outils nécessaires pour créer exactement le format de sortie dont votre application a besoin.

---
Pour davantage d'exemples de code et d'implémentation, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
