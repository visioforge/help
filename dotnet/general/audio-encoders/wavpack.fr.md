---
title: Encodeur audio WavPack en .NET — Paramètres et configuration
description: Implémentez la compression audio WavPack sans perte et hybride avec perte en .NET avec qualité, modes de correction et encodage stéréo.
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
  - WavPack
  - C#
primary_api_classes:
  - WavPackEncoderSettings
  - VideoCaptureCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline
  - WavPackOutput

---

# Encodeur audio WavPack pour les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

## Introduction à WavPack

WavPack est un puissant codec audio qui offre des capacités de compression à la fois sans perte et hybride avec perte, ce qui le rend très polyvalent pour différentes exigences d'application. La bibliothèque VisioForge.Core fournit une implémentation robuste de ce codec pour les développeurs .NET à la recherche de solutions de compression audio de haute qualité.

Avec la prise en charge de divers niveaux de qualité, modes de correction et options d'encodage stéréo, l'encodeur WavPack peut gérer plusieurs configurations de canaux tout en offrant une excellente compression sur une large gamme de débits et fréquences d'échantillonnage.

## Prise en main de WavPack

### Configuration de base

Pour commencer à utiliser l'encodeur WavPack, vous devrez créer une instance de la classe `WavPackEncoderSettings` avec les paramètres souhaités :

```csharp
var encoder = new WavPackEncoderSettings
{
    Mode = WavPackEncoderMode.Normal,
    JointStereoMode = WavPackEncoderJSMode.Auto,
    CorrectionMode = WavPackEncoderCorrectionMode.Off,
    MD5 = false
};
```

Cette configuration simple utilise des paramètres de compression équilibrés et une sélection automatique du mode d'encodage stéréo, convenant à la plupart des cas d'usage généraux.

### Modes de compression expliqués

WavPack propose quatre modes de compression distincts qui équilibrent la vitesse de traitement avec l'efficacité de compression :

```csharp
public enum WavPackEncoderMode
{
    Fast = 1,      // Privilégie la vitesse d'encodage
    Normal = 2,    // Compression équilibrée (par défaut)
    High = 3,      // Taux de compression supérieur
    VeryHigh = 4   // Compression maximale
}
```

Pour les applications où la taille du fichier est critique, vous pouvez implémenter des paramètres de compression plus élevés :

```csharp
var encoder = new WavPackEncoderSettings
{
    Mode = WavPackEncoderMode.High,
    ExtraProcessing = 1 // Active les filtres avancés pour une meilleure compression
};
```

## Options de contrôle de qualité

### Encodage basé sur le débit

La méthode la plus simple pour contrôler la qualité de sortie est de spécifier un débit cible :

```csharp
var encoder = new WavPackEncoderSettings
{
    Bitrate = 192000 // 192 kbps
};
```

Spécifications clés pour le contrôle du débit :

- Plage valide : 24 000 à 9 600 000 bits/seconde
- Les valeurs inférieures à 24 000 désactivent l'encodage avec perte
- Active automatiquement le mode d'encodage avec perte

### Contrôle des bits par échantillon

Pour un contrôle plus précis de la qualité, en particulier lorsque le maintien d'une qualité cohérente sur différentes fréquences d'échantillonnage est important :

```csharp
var encoder = new WavPackEncoderSettings
{
    BitsPerSample = 16.0 // Équivalent à une qualité 16 bits
};
```

Notes importantes :

- Les valeurs inférieures à 2.0 désactivent l'encodage avec perte
- Cette approche maintient une qualité plus cohérente quelles que soient les variations de fréquence d'échantillonnage

## Fonctionnalités d'encodage avancées

### Options d'encodage stéréo

WavPack fournit trois méthodes pour encoder le contenu stéréo, chacune avec des caractéristiques différentes :

```csharp
var encoder = new WavPackEncoderSettings
{
    JointStereoMode = WavPackEncoderJSMode.Auto
};
```

Modes d'encodage stéréo disponibles :

- `Auto` : Sélectionne intelligemment la méthode d'encodage optimale en fonction du contenu
- `LeftRight` : Utilise la séparation traditionnelle des canaux gauche/droite
- `MidSide` : Implémente le codage mid/side qui produit souvent une meilleure compression pour le matériel stéréo

### Mode de correction hybride

L'une des fonctionnalités uniques de WavPack est son mode hybride, qui génère un fichier de correction aux côtés du fichier compressé principal :

```csharp
var encoder = new WavPackEncoderSettings
{
    CorrectionMode = WavPackEncoderCorrectionMode.Optimized,
    Bitrate = 192000 // Requis lors de l'utilisation des modes de correction
};
```

Options de correction disponibles :

- `Off` : Fonctionnement standard sans fichier de correction
- `On` : Génère un fichier de correction standard
- `Optimized` : Crée un fichier de correction axé sur l'optimisation

Notez que les modes de correction ne fonctionnent que lorsque l'encodage avec perte est actif, ce qui les rend idéaux pour les applications où la taille initiale du fichier est importante mais où une restauration sans perte future pourrait être nécessaire.

## Spécifications techniques

L'encodeur WavPack prend en charge :

- Fréquences d'échantillonnage de 6 000 Hz à 192 000 Hz
- 1 à 8 canaux audio
- Stockage facultatif du hachage MD5 des échantillons bruts pour vérification
- Options de traitement supplémentaires pour l'amélioration de la qualité

Avant l'implémentation, vous pouvez vérifier la disponibilité de l'encodeur dans votre environnement :

```csharp
if (WavPackEncoderSettings.IsAvailable())
{
    // Configurer et utiliser l'encodeur
    var encoder = new WavPackEncoderSettings
    {
        Mode = WavPackEncoderMode.Normal,
        Bitrate = 192000,
        MD5 = true
    };
}
```

## Exemples d'implémentation

### Intégration Video Capture SDK

```csharp
// Initialiser le noyau Video Capture SDK
var core = new VideoCaptureCoreX();

// Créer une instance de sortie WavPack
var wavPackOutput = new WavPackOutput("output.wv");

// Ajouter la sortie WavPack au pipeline de capture
core.Outputs_Add(wavPackOutput, true);
```

### Intégration Video Edit SDK

```csharp
// Initialiser le noyau Video Edit SDK
var core = new VideoEditCoreX();

// Créer une instance de sortie WavPack
var wavPackOutput = new WavPackOutput("output.wv");

// Définir le format de sortie
core.Output_Format = wavPackOutput;
```

### Intégration Media Blocks SDK

```csharp
// Configurer les paramètres de l'encodeur WavPack
var wavPackSettings = new WavPackEncoderSettings();

// Créer le bloc d'encodeur
var wavPackOutput = new WavPackEncoderBlock(wavPackSettings);

// Créer une destination de sortie fichier
var fileSink = new FileSinkBlock("output.wv");

// Connecter l'encodeur au puits de fichier dans le pipeline
pipeline.Connect(wavPackOutput.Output, fileSink.Input); // pipeline est MediaBlocksPipeline
```

## Stratégies d'optimisation

### Performance vs qualité

Pour un équilibre optimal entre performance et qualité de l'encodeur :

=== "Par défaut"

    
    - Utilisez le mode `Normal` pour les tâches d'encodage quotidiennes
    - Activez `ExtraProcessing` uniquement lorsque le temps d'encodage n'est pas critique
    - Maintenez `JointStereoMode` sur `Auto` pour la plupart des types de contenu
    

=== "Archivage"

    
    - Implémentez le mode `High` ou `VeryHigh` à des fins d'archivage
    - Activez la génération du hachage MD5 pour la vérification du contenu
    - Envisagez l'encodage sans perte pour la préservation audio critique
    

=== "Streaming"

    
    - Utilisez le mode `Fast` pour les scénarios d'encodage en temps réel
    - Sélectionnez un débit approprié en fonction des contraintes de bande passante
    - Désactivez les fonctionnalités de traitement supplémentaires pour minimiser la latence
    


## Bonnes pratiques

Lors de l'implémentation de WavPack dans vos applications :

1. **Équilibrez qualité et performance** en sélectionnant le mode de compression approprié en fonction de votre cas d'usage
2. **Tirez parti du mode hybride** lors de la distribution de fichiers avec perte qui pourraient nécessiter une restauration sans perte ultérieure
3. **Considérez la compatibilité du format** avec vos plateformes cibles et environnements de lecture
4. **Testez minutieusement** sur différents types de contenu audio pour garantir des paramètres optimaux

## Conclusion

L'encodeur WavPack fournit une solution polyvalente pour la compression audio dans les applications .NET. Que vous ayez besoin d'une compression sans perte de qualité archivage ou d'une compression avec perte efficace avec un potentiel de mise à niveau futur, l'implémentation dans les SDK de VisioForge offre la flexibilité et la performance requises par les applications audio professionnelles.

En comprenant les diverses options de configuration et stratégies d'implémentation décrites dans ce guide, vous pouvez intégrer efficacement l'encodage WavPack dans vos projets de développement logiciel et offrir des capacités de traitement audio de haute qualité à vos utilisateurs.
