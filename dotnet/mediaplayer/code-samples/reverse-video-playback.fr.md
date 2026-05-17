---
title: Lecture vidéo inversée en .NET avec exemples de code C#
description: Mettez en œuvre la lecture vidéo inversée et la navigation image par image avec VisioForge Media Player SDK .NET sur Windows et plateformes croisées.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - MP4
  - C#
primary_api_classes:
  - MediaPlayerCoreX
  - MediaPlayerCore
  - PlaybackState
  - UniversalSourceSettings

---

# Mise en œuvre de la lecture vidéo inversée dans les applications .NET

La lecture vidéo en sens inverse est une fonctionnalité puissante pour les applications multimédias, permettant aux utilisateurs de revoir du contenu, de créer des effets visuels uniques ou d'améliorer l'expérience utilisateur grâce à des options de lecture non linéaires. Ce guide fournit des implémentations complètes de la lecture inversée dans les applications .NET, en se concentrant à la fois sur les solutions multiplateformes et spécifiques à Windows.

## Comprendre les mécanismes de lecture inversée

La lecture vidéo inversée peut être obtenue grâce à plusieurs techniques, chacune ayant des avantages distincts selon les exigences de votre application :

1. **Lecture inversée basée sur la vitesse** — définir une vitesse de lecture négative pour inverser le flux vidéo
2. **Navigation en arrière image par image** — parcourir manuellement les images vidéo mises en cache
3. **Approches basées sur un tampon** — créer un tampon d'images pour permettre une navigation inverse fluide

Explorons comment implémenter chaque approche à l'aide du Media Player SDK pour .NET.

## Lecture inversée multiplateforme avec MediaPlayerCoreX

Le moteur MediaPlayerCoreX offre une prise en charge multiplateforme de la lecture vidéo inversée avec une implémentation simple. Cette approche fonctionne sur Windows, macOS et autres plateformes prises en charge.

### Implémentation de base

La méthode la plus simple pour la lecture inversée consiste à définir une valeur de vitesse négative :

```cs
// Créer une nouvelle instance de MediaPlayerCoreX
MediaPlayerCoreX MediaPlayer1 = new MediaPlayerCoreX(VideoView1);

// Définir le fichier source
var fileSource = await UniversalSourceSettings.CreateAsync(new Uri("video.mp4"));
await MediaPlayer1.OpenAsync(fileSource);

// Démarrer d'abord la lecture normale
await MediaPlayer1.PlayAsync();

// Passer à la lecture inversée à vitesse normale
MediaPlayer1.Rate_Set(-1.0);
```

### Contrôle de la vitesse de lecture inversée

Vous pouvez contrôler la vitesse de lecture inversée en ajustant la valeur de vitesse négative :

```cs
// Lecture inversée à double vitesse
MediaPlayer1.Rate_Set(-2.0);

// Lecture inversée à demi-vitesse (ralenti inversé)
MediaPlayer1.Rate_Set(-0.5);

// Lecture inversée à quart de vitesse (ralenti très lent inversé)
MediaPlayer1.Rate_Set(-0.25);
```

### Suivi de la position pendant la lecture inversée

`MediaPlayerCoreX` ne déclenche pas d'événement de changement de position ; interrogez la position via un minuteur :

```cs
// Interroger la position du lecteur toutes les 100 ms et mettre à jour l'interface utilisateur
var positionTimer = new System.Threading.Timer(_ =>
{
    TimeSpan currentPosition = MediaPlayer1.Position_Get();
    UpdatePositionUI(currentPosition);

    // Détecter l'arrivée au début lors d'une lecture en arrière
    if (MediaPlayer1.Rate_Get() < 0 && currentPosition <= TimeSpan.FromMilliseconds(100))
    {
        // Passer à la lecture en avant (ou mettre en pause)
        MediaPlayer1.Rate_Set(1.0);
    }
}, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
```

## Navigation inverse image par image spécifique à Windows

Le moteur classique `MediaPlayerCore` (Windows uniquement) offre un contrôle avancé image par image grâce à son système de mise en cache d'images, permettant une navigation arrière précise même avec des codecs qui ne la prennent pas en charge nativement. Déclarez une instance distincte du moteur classique — l'API `ReversePlayback_*` se trouve sur `MediaPlayerCore` et **n'est pas** disponible sur `MediaPlayerCoreX`.

### Configuration de la mise en cache des images

Avant de démarrer la lecture, configurez le cache de lecture inversée :

```cs
// Moteur Windows classique — type différent de MediaPlayerCoreX ci-dessus
MediaPlayerCore classicPlayer = new MediaPlayerCore(VideoView1);

// Configurer la lecture inversée avant le démarrage
classicPlayer.ReversePlayback_CacheSize = 100; // Mettre 100 images en cache
classicPlayer.ReversePlayback_Enabled = true;  // Activer la fonctionnalité

// Démarrer la lecture
await classicPlayer.PlayAsync();
```

### Navigation image par image

Avec le cache configuré, vous pouvez naviguer vers les images précédentes :

```cs
// Naviguer vers l'image précédente
classicPlayer.ReversePlayback_PreviousFrame();

// Naviguer en arrière sur plusieurs images
for(int i = 0; i < 5; i++)
{
    classicPlayer.ReversePlayback_PreviousFrame();
    // Facultatif : ajouter un délai entre les images pour une lecture contrôlée
    await Task.Delay(40); // Timing équivalent à environ 25 ips
}
```

### Configuration avancée du cache d'images

Pour les applications avec des exigences spécifiques de mémoire ou de performance, vous pouvez affiner le cache :

```cs
// Pour les vidéos haute résolution, vous pourriez avoir besoin de moins d'images pour gérer la mémoire
classicPlayer.ReversePlayback_CacheSize = 50; // Réduire la taille du cache

// Pour les applications nécessitant une navigation arrière étendue
classicPlayer.ReversePlayback_CacheSize = 250; // Augmenter la taille du cache
```

## Implémentation des contrôles d'interface utilisateur pour la lecture inversée

Une implémentation complète de la lecture inversée inclut généralement des contrôles d'interface utilisateur dédiés :

```cs
// Gestionnaire de clic de bouton pour la lecture inversée
private async void ReversePlaybackButton_Click(object sender, EventArgs e)
{
    if(MediaPlayer1.State == PlaybackState.Play)
    {
        // Basculer entre lecture avant et arrière
        if(MediaPlayer1.Rate_Get() > 0)
        {
            MediaPlayer1.Rate_Set(-1.0);
            UpdateUIForReverseMode(true);
        }
        else
        {
            MediaPlayer1.Rate_Set(1.0);
            UpdateUIForReverseMode(false);
        }
    }
    else
    {
        // Démarrer la lecture en inverse
        await MediaPlayer1.PlayAsync();
        MediaPlayer1.Rate_Set(-1.0);
        UpdateUIForReverseMode(true);
    }
}

// Gestionnaire de clic de bouton pour la navigation en arrière image par image
// (suppose que le moteur classique Windows-only `classicPlayer` a été déclaré ci-dessus)
private async void PreviousFrameButton_Click(object sender, EventArgs e)
{
    // S'assurer d'abord que la lecture est en pause — le MediaPlayerCore classique expose State() comme une méthode (X est une propriété).
    if (classicPlayer.State() == PlaybackState.Play)
    {
        await classicPlayer.PauseAsync();
    }
    
    // Naviguer vers l'image précédente
    classicPlayer.ReversePlayback_PreviousFrame();
    UpdateFrameCountDisplay();
}
```

## Considérations de performance

La lecture inversée peut être gourmande en ressources, en particulier avec des vidéos haute résolution. Envisagez ces techniques d'optimisation :

1. **Limitez la taille du cache** pour les appareils avec des contraintes de mémoire
2. **Utilisez l'accélération matérielle** lorsque c'est possible
3. **Surveillez les performances** pendant la lecture inversée avec des outils de débogage
4. **Fournissez des options de repli** pour les appareils qui peinent avec une lecture inversée à pleine vitesse

```cs
// Exemple de surveillance des performances pendant la lecture inversée.
// Le suivi de la vitesse se trouve sur MediaPlayerCoreX (`MediaPlayer1`) ;
// l'ajustement du cache cible le MediaPlayerCore classique (`classicPlayer`).
private void MonitorPerformance()
{
    Timer performanceTimer = new Timer(1000);
    performanceTimer.Elapsed += (s, e) => 
    {
        if(MediaPlayer1.Rate_Get() < 0)
        {
            // Journaliser ou afficher l'utilisation actuelle de la mémoire, la fréquence d'images, etc.
            LogPerformanceMetrics();
        }

        // Ajuster le cache d'images du moteur classique si la pression mémoire est élevée
        if(IsMemoryUsageHigh())
        {
            classicPlayer.ReversePlayback_CacheSize = 
                Math.Max(10, classicPlayer.ReversePlayback_CacheSize / 2);
        }
    };
    performanceTimer.Start();
}
```

## Dépendances requises

Pour garantir le bon fonctionnement des fonctionnalités de lecture inversée, incluez ces dépendances :

- Paquet redistribuable de base
- Paquet redistribuable du SDK

Ces paquets contiennent les codecs et les composants de traitement multimédia nécessaires pour permettre une lecture inversée fluide pour différents formats vidéo.

## Ressources supplémentaires et techniques avancées

Pour les applications multimédias complexes nécessitant des fonctionnalités avancées de lecture inversée, envisagez d'explorer :

- L'extraction d'images et le rendu manuel pour des effets personnalisés
- L'analyse des images-clés pour une navigation optimisée
- Les stratégies de mise en tampon pour une lecture inversée plus fluide

## Conclusion

L'implémentation de la lecture vidéo inversée apporte une valeur significative aux applications multimédias, offrant aux utilisateurs un contrôle accru sur la navigation dans le contenu. En suivant les modèles d'implémentation de ce guide, les développeurs peuvent créer des expériences de lecture inversée robustes et performantes dans leurs applications .NET.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code complets et des exemples d'implémentation supplémentaires.
