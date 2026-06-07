---
title: Lire des fragments et segments de fichiers vidéo en C# .NET
description: Lisez des segments précis basés sur le temps de fichiers vidéo et audio avec VisioForge Media Player SDK .NET sur Windows et multiplateforme.
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
  - MKV
  - C#
primary_api_classes:
  - MediaPlayerCoreX
  - UniversalSourceSettings

---

# Lecture de fragments de fichiers multimédias : guide d'implémentation pour les développeurs .NET

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Lors du développement d'applications multimédias, une fonctionnalité fréquemment demandée est la capacité à lire des segments spécifiques d'un fichier vidéo ou audio. Cette fonctionnalité est cruciale pour créer des éditeurs vidéo, des séquences de moments forts, des plateformes éducatives ou toute application nécessitant une lecture précise par segment multimédia.

## Comprendre la lecture par fragments dans les applications .NET

La lecture par fragments vous permet de définir des segments temporels spécifiques d'un fichier multimédia pour la lecture, créant effectivement des clips sans modifier le fichier source. Cette technique est particulièrement utile lorsque vous devez :

- Créer des segments d'aperçu à partir de fichiers multimédias plus longs
- Vous concentrer sur des sections spécifiques de vidéos d'instruction
- Créer des segments en boucle pour des démonstrations ou présentations
- Construire des lecteurs multimédias basés sur des clips pour des moments forts sportifs ou des compilations vidéo
- Implémenter des applications de formation qui se concentrent sur des segments vidéo spécifiques

Le Media Player SDK .NET fournit deux moteurs principaux pour implémenter la lecture par fragments, chacun avec sa propre approche et ses considérations de compatibilité de plateforme.

## Implémentation Windows uniquement : moteur MediaPlayerCore

Le moteur MediaPlayerCore fournit une implémentation simple pour les applications Windows. Cette solution fonctionne sur WPF, WinForms et les applications console mais est limitée aux systèmes d'exploitation Windows.

### Configuration de la lecture par fragments

Pour implémenter la lecture par fragments avec le moteur MediaPlayerCore, vous devrez suivre trois étapes clés :

1. Activer le mode sélection sur votre instance MediaPlayer
2. Définir la position de début de votre fragment (en millisecondes)
3. Définir la position de fin de votre fragment (en millisecondes)

### Exemple d'implémentation

Le code C# suivant montre comment configurer la lecture par fragments pour ne lire que le segment entre 2000 ms et 5000 ms de votre fichier source :

```csharp
// Étape 1 : activer le mode de sélection de fragment
MediaPlayer1.Selection_Active = true;

// Étape 2 : définir la position de début à 2000 millisecondes (2 secondes)
MediaPlayer1.Selection_Start = TimeSpan.FromMilliseconds(2000);

// Étape 3 : définir la position de fin à 5000 millisecondes (5 secondes)
MediaPlayer1.Selection_Stop = TimeSpan.FromMilliseconds(5000);

// Lorsque vous appelez Play() ou PlayAsync(), seul le fragment spécifié sera lu
```

Lorsque votre application appelle la méthode Play ou PlayAsync après avoir défini ces propriétés, le lecteur sautera automatiquement à la position de début de la sélection et arrêtera la lecture lorsqu'il atteindra la position de fin de la sélection.

### Redistribuables requis pour l'implémentation Windows

Pour que l'implémentation du moteur MediaPlayerCore fonctionne correctement, vous devez inclure :

- Paquet redistribuable de base
- Paquet redistribuable du SDK

Ces paquets contiennent les composants nécessaires à la fonctionnalité de lecture basée sur Windows. Pour des informations détaillées sur le déploiement de ces redistribuables sur les machines des utilisateurs finaux, référez-vous à la [documentation de déploiement](../deployment.md).

## Implémentation multiplateforme : moteur MediaPlayerCoreX

Pour les développeurs nécessitant une fonctionnalité de lecture par fragments sur plusieurs plateformes, le moteur MediaPlayerCoreX fournit une solution plus polyvalente. Cette implémentation fonctionne dans les environnements Windows, macOS, iOS, Android et Linux.

### Configuration de la lecture par fragments multiplateforme

L'implémentation multiplateforme suit une approche conceptuelle similaire mais utilise des noms de propriétés différents. Les étapes clés incluent :

1. Créer une instance MediaPlayerCoreX
2. Charger votre source multimédia
3. Définir les positions de début et de fin du segment
4. Initier la lecture

### Exemple d'implémentation multiplateforme

L'exemple suivant montre comment implémenter la lecture par fragments dans une application .NET multiplateforme :

```csharp
// Étape 1 : créer une nouvelle instance de MediaPlayerCoreX avec votre vue vidéo
MediaPlayerCoreX MediaPlayer1 = new MediaPlayerCoreX(VideoView1);

// Étape 2 : définir le fichier multimédia source
var fileSource = await UniversalSourceSettings.CreateAsync("video.mkv");
await MediaPlayer1.OpenAsync(fileSource);

// Étape 3 : définir le temps de début du segment (2 secondes depuis le début)
MediaPlayer1.Segment_Start = TimeSpan.FromMilliseconds(2000);

// Étape 4 : définir le temps de fin du segment (5 secondes depuis le début)
MediaPlayer1.Segment_Stop = TimeSpan.FromMilliseconds(5000);

// Étape 5 : démarrer la lecture du segment défini
await MediaPlayer1.PlayAsync();
```

Cette implémentation utilise les propriétés Segment_Start et Segment_Stop au lieu des propriétés Selection utilisées dans l'implémentation Windows uniquement. Notez également l'approche asynchrone utilisée dans l'exemple multiplateforme, qui améliore la réactivité de l'UI.

## Techniques avancées de lecture par fragments

### Ajustement dynamique des fragments

Dans des applications plus complexes, vous pourriez avoir besoin d'ajuster dynamiquement les limites des fragments. Les deux moteurs prennent en charge le changement des limites de segment à l'exécution :

```csharp
// Pour l'implémentation Windows uniquement
private void UpdateFragmentBoundaries(int startMs, int endMs)
{
    MediaPlayer1.Selection_Start = TimeSpan.FromMilliseconds(startMs);
    MediaPlayer1.Selection_Stop = TimeSpan.FromMilliseconds(endMs);
    
    // Si la lecture est en cours, redémarrez-la pour appliquer les nouvelles limites
    if (MediaPlayer1.State() == PlaybackState.Play)
    {
        MediaPlayer1.Position_Set_Time(MediaPlayer1.Selection_Start);
    }
}

// Pour l'implémentation multiplateforme
private async Task UpdateFragmentBoundariesAsync(int startMs, int endMs)
{
    MediaPlayer1.Segment_Start = TimeSpan.FromMilliseconds(startMs);
    MediaPlayer1.Segment_Stop = TimeSpan.FromMilliseconds(endMs);
    
    // Si la lecture est en cours, redémarrer depuis la nouvelle position de début (State est une propriété sync sur MediaPlayerCoreX)
    if (MediaPlayer1.State == PlaybackState.Play)
    {
        await MediaPlayer1.Position_SetAsync(MediaPlayer1.Segment_Start);
    }
}
```

### Lecture de fragments multiples

Pour les applications qui doivent lire plusieurs fragments séquentiellement, vous pouvez implémenter une file d'attente de fragments :

```csharp
public class MediaFragment
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

private Queue<MediaFragment> fragmentQueue = new Queue<MediaFragment>();
private bool isProcessingQueue = false;

// Ajouter des fragments à la file
public void EnqueueFragment(TimeSpan start, TimeSpan end)
{
    fragmentQueue.Enqueue(new MediaFragment { StartTime = start, EndTime = end });
    
    if (!isProcessingQueue && MediaPlayer1 != null)
    {
        PlayNextFragment();
    }
}

// Traiter la file de fragments
private async void PlayNextFragment()
{
    if (fragmentQueue.Count == 0)
    {
        isProcessingQueue = false;
        return;
    }
    
    isProcessingQueue = true;
    var fragment = fragmentQueue.Dequeue();
    
    // Définir les limites du fragment
    MediaPlayer1.Segment_Start = fragment.StartTime;
    MediaPlayer1.Segment_Stop = fragment.EndTime;
    
    // S'abonner à l'événement de fin pour ce fragment
    MediaPlayer1.OnStop += (s, e) => PlayNextFragment();
    
    // Démarrer la lecture
    await MediaPlayer1.PlayAsync();
}
```

### Considérations de performance

Pour des performances optimales lors de l'utilisation de la lecture par fragments, considérez les conseils suivants :

1. Pour un positionnement fréquent entre fragments, utilisez des formats avec une bonne densité d'images-clés
2. Les fichiers MP4 et MOV fonctionnent généralement mieux pour les applications à forte densité de fragments
3. Définir les fragments aux frontières des images-clés améliore les performances de positionnement
4. Envisagez de précharger les fichiers avant de définir les limites de fragment
5. Sur les plateformes mobiles, gardez les fragments de taille raisonnable pour éviter la pression sur la mémoire

## Conclusion

L'implémentation de la lecture par fragments dans vos applications multimédias .NET fournit une flexibilité substantielle et une expérience utilisateur améliorée. Que vous développiez uniquement pour Windows ou que vous cibliez plusieurs plateformes, le Media Player SDK .NET offre des solutions robustes pour une lecture précise par segment multimédia.

En tirant parti des techniques démontrées dans ce guide, vous pouvez créer des expériences multimédias sophistiquées qui permettent aux utilisateurs de se concentrer exactement sur le contenu dont ils ont besoin, sans la surcharge d'édition ou de division des fichiers sources.

Pour plus d'exemples de code et d'implémentations, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) où vous trouverez des exemples complets d'implémentations de lecteur multimédia, y compris la lecture par fragments et d'autres fonctionnalités multimédias avancées.
