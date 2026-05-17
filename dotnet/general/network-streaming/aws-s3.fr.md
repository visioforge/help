---
title: Streaming vidéo et audio vers Amazon S3 en SDK .NET
description: Implémentez la diffusion vidéo et audio AWS S3 en .NET avec configuration, paramètres d'encodage, gestion des erreurs et bonnes pratiques pour la sortie média.
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
  - C#
primary_api_classes:
  - AWSS3Output
  - AWSS3SinkSettings
  - IVideoEncoder
  - IAudioEncoder
  - IMediaBlockSettings

---

# Sortie AWS S3

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La fonctionnalité de sortie AWS S3 dans les SDK VisioForge permet la diffusion directe de sortie vidéo et audio vers le stockage Amazon S3. Ce guide vous accompagnera dans la configuration et l'utilisation de la sortie AWS S3 dans vos applications.

## Présentation

La classe `AWSS3Output` est un gestionnaire de sortie spécialisé au sein des SDK VisioForge qui facilite la diffusion de sortie vidéo et audio vers le stockage Amazon Web Services (AWS) S3. Cette classe implémente plusieurs interfaces pour prendre en charge à la fois les scénarios d'édition vidéo (`IVideoEditXBaseOutput`) et de capture vidéo (`IVideoCaptureXBaseOutput`), ainsi que des capacités de traitement pour les contenus vidéo et audio.

## Implémentation de la classe

```csharp
public class AWSS3Output : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

## Fonctionnalités clés

La classe `AWSS3Output` fournit une solution complète pour diffuser du contenu média vers AWS S3 en gérant :

- La configuration de l'encodage vidéo
- La configuration de l'encodage audio
- Le traitement média personnalisé
- Les paramètres spécifiques à AWS S3

## Propriétés

### Paramètres de l'encodeur vidéo

```csharp
public IVideoEncoder Video { get; set; }
```

Contrôle le processus d'encodage vidéo. L'encodeur vidéo sélectionné doit être compatible avec les paramètres de puits configurés. Cette propriété vous permet de spécifier les méthodes de compression, les paramètres de qualité et d'autres paramètres spécifiques à la vidéo.

### Paramètres de l'encodeur audio

```csharp
public IAudioEncoder Audio { get; set; }
```

Gère la configuration de l'encodage audio. Comme pour l'encodeur vidéo, l'encodeur audio doit être compatible avec les paramètres du puits. Cette propriété permet le contrôle de la qualité audio, de la compression et des paramètres de format.

### Paramètres du puits

```csharp
public IMediaBlockSettings Sink { get; set; }
```

Définit la configuration de la destination de sortie. Dans ce contexte, elle contient les paramètres spécifiques à AWS S3 pour le flux de sortie média.

### Blocs de traitement personnalisés

```csharp
public MediaBlock CustomVideoProcessor { get; set; }
```

```csharp
public MediaBlock CustomAudioProcessor { get; set; }
```

Permettent un traitement supplémentaire des flux vidéo et audio avant qu'ils ne soient encodés et téléversés vers S3. Ces blocs peuvent être utilisés pour implémenter des filtres personnalisés, des transformations ou des analyses du contenu média.

### Configuration AWS S3

```csharp
public AWSS3SinkSettings Settings { get; set; }
```

Contient toutes les options de configuration spécifiques à AWS S3, notamment :

- Identifiants d'accès (clé d'accès, clé d'accès secrète)
- Informations de bucket et de clé d'objet
- Configuration de la région
- Paramètres de comportement de téléversement
- Préférences de gestion des erreurs

## Constructeur

```csharp
public AWSS3Output(AWSS3SinkSettings settings, 
                   IVideoEncoder videoEnc, 
                   IAudioEncoder audioEnc, 
                   IMediaBlockSettings sink)
```

Crée une nouvelle instance de la classe `AWSS3Output` avec la configuration spécifiée :

- `settings` : configuration spécifique à AWS S3
- `videoEnc` : paramètres de l'encodeur vidéo
- `audioEnc` : paramètres de l'encodeur audio
- `sink` : configuration du puits média

## Méthodes

### Gestion de fichiers

```csharp
public string GetFilename()
```

```csharp
public void SetFilename(string filename)
```

Ces méthodes gèrent l'URI de l'objet S3 :

- `GetFilename()` : retourne l'URI S3 actuelle
- `SetFilename(string filename)` : définit l'URI S3 pour la sortie

### Prise en charge des encodeurs

Tous les encodeurs sont pris en charge. Assurez-vous que les paramètres de l'encodeur sont compatibles avec les paramètres du puits.

## Exemple d'utilisation

```csharp
// Créer les paramètres du puits AWS S3
var s3Settings = new AWSS3SinkSettings
{
    AccessKey = "your-access-key",
    SecretAccessKey = "your-secret-key",
    Bucket = "your-bucket-name",
    Key = "output-file-key",
    Region = "us-west-1"
};

// Configurer les encodeurs
IVideoEncoder videoEncoder = /* votre configuration d'encodeur vidéo */;
IAudioEncoder audioEncoder = /* votre configuration d'encodeur audio */;
IMediaBlockSettings sinkSettings = /* vos paramètres de puits */;

// Créer la sortie AWS S3
var s3Output = new AWSS3Output(s3Settings, videoEncoder, audioEncoder, sinkSettings);

// Optionnel : configurer des processeurs personnalisés
s3Output.CustomVideoProcessor = /* votre processeur vidéo personnalisé */;
s3Output.CustomAudioProcessor = /* votre processeur audio personnalisé */;
```

## Bonnes pratiques

1. Assurez-vous toujours que vos identifiants AWS sont correctement sécurisés et non codés en dur dans l'application.
2. Configurez des nombres de tentatives et des délais d'attente appropriés en fonction de vos conditions réseau et tailles de fichier.
3. Sélectionnez des encodeurs vidéo et audio compatibles avec votre cas d'usage cible.
4. Envisagez d'implémenter des processeurs personnalisés pour des exigences spécifiques comme l'ajout d'un filigrane ou la normalisation audio.

## Gestion des erreurs

La classe fonctionne en conjonction avec l'énumération `S3SinkOnError` définie dans `AWSS3SinkSettings`, qui fournit trois stratégies de gestion des erreurs :

- Abort : arrête le processus de téléversement en cas d'erreur
- Complete : tente de terminer le téléversement malgré les erreurs
- DoNothing : ignore les erreurs pendant le téléversement

## Composants associés

- AWSS3SinkSettings : contient la configuration détaillée pour la connectivité AWS S3
- IVideoEncoder : interface pour la configuration de l'encodage vidéo
- IAudioEncoder : interface pour la configuration de l'encodage audio
- IMediaBlockSettings : interface pour la configuration de la sortie média
