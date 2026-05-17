---
title: Lecture vidéo en boucle et plage de position en C# .NET
description: Implémentez la lecture en boucle et le contrôle par plage de position avec VisioForge Media Player SDK .NET. Exemples moteurs DirectShow et GStreamer inclus.
sidebar_label: Mode boucle et plage de position
order: 2
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Playback
  - Streaming
  - MP4
  - C#
primary_api_classes:
  - MediaPlayerCoreX
  - MediaPlayerCore
  - UniversalSourceSettings
  - MediaBlocksPipeline

---

# Mode boucle et lecture par plage de position

Ce guide explique comment utiliser le mode boucle et les fonctionnalités personnalisées de position de début et fin (plage de position) dans Media Player SDK à la fois pour MediaPlayerCore (moteur DirectShow) et MediaPlayerCoreX (moteur GStreamer).

## Vue d'ensemble

Les deux moteurs Media Player prennent en charge :

- **Mode boucle** : redémarrer automatiquement la lecture lorsque le média atteint la fin
- **Plage de position** : lire uniquement un segment spécifique du média entre les positions de début et de fin
- **Mode combiné** : boucler un segment spécifique du média en continu

Ces fonctionnalités sont utiles pour :

- Créer des boucles vidéo pour des kiosques ou des écrans
- Lecture d'aperçu de segments spécifiques
- Tester des portions spécifiques de fichiers multimédias
- Créer des boucles vidéo d'arrière-plan fluides
- Applications éducatives présentant du contenu répété

## MediaPlayerCore (moteur DirectShow)

MediaPlayerCore est le moteur de lecteur multimédia basé sur DirectShow et réservé à Windows.

### Propriétés du mode boucle

#### Propriété `Loop`

Active ou désactive la lecture en boucle automatique.

```csharp
// Activer le mode boucle
mediaPlayer.Loop = true;

// Désactiver le mode boucle
mediaPlayer.Loop = false;
```

**Valeur par défaut** : `false`

**Comportement** :
- Lorsque activée, la lecture redémarre automatiquement depuis le début lorsque la fin est atteinte
- L'événement `OnLoop` se déclenche chaque fois que la lecture redémarre
- Pour les listes de lecture, la boucle s'applique à toute la liste, pas aux fichiers individuels

#### Propriété `Loop_DoNotSeekToBeginning`

Contrôle s'il faut se positionner au début lorsque la boucle redémarre.

```csharp
// Redémarrer sans se positionner au début (boucle fluide)
mediaPlayer.Loop_DoNotSeekToBeginning = true;

// Se positionner au début avant de redémarrer (par défaut)
mediaPlayer.Loop_DoNotSeekToBeginning = false;
```

**Valeur par défaut** : `false`

**Comportement** :
- N'affecte le comportement que lorsque `Loop` est `true`
- Lorsque `true`, redémarre depuis la position actuelle sans se positionner
- Améliore les performances et évite les artefacts visuels durant les transitions de boucle
- Utile pour un bouclage fluide du contenu

### Propriétés de plage de position

#### Propriété `Selection_Active`

Active ou désactive la sélection de plage de lecture.

```csharp
// Activer la lecture par plage de position
mediaPlayer.Selection_Active = true;

// Désactiver la lecture par plage de position (lire le fichier entier)
mediaPlayer.Selection_Active = false;
```

**Valeur par défaut** : `false`

**Comportement** :
- Lorsque active, la lecture est contrainte entre `Selection_Start` et `Selection_Stop`
- Le lecteur s'arrête ou boucle automatiquement en atteignant `Selection_Stop`
- Utile pour lire des segments spécifiques ou créer des aperçus de clips

#### Propriété `Selection_Start`

Définit la position de début pour la lecture basée sur une plage.

```csharp
// Commencer la lecture à 30 secondes
mediaPlayer.Selection_Start = TimeSpan.FromSeconds(30);

// Commencer la lecture à 2 minutes 15 secondes
mediaPlayer.Selection_Start = new TimeSpan(0, 2, 15);
```

**Type** : `TimeSpan`

**Exigences** :
- Utilisée uniquement lorsque `Selection_Active` est `true`
- Doit être inférieure à `Selection_Stop`
- Doit être dans la durée du média
- Le lecteur se positionne automatiquement à cet endroit au démarrage

#### Propriété `Selection_Stop`

Définit la position de fin pour la lecture basée sur une plage.

```csharp
// Arrêter la lecture à 1 minute
mediaPlayer.Selection_Stop = TimeSpan.FromSeconds(60);

// Lire jusqu'à la fin du fichier
mediaPlayer.Selection_Stop = TimeSpan.Zero;
```

**Type** : `TimeSpan`

**Exigences** :
- Utilisée uniquement lorsque `Selection_Active` est `true`
- Doit être supérieure à `Selection_Start`
- Utilisez `TimeSpan.Zero` pour lire jusqu'à la fin du fichier multimédia
- Lorsque la lecture atteint cette position, elle s'arrête (ou boucle si `Loop` est activée)

### Événements MediaPlayerCore

#### Événement `OnLoop`

Se déclenche chaque fois que la lecture redémarre en mode boucle.

```csharp
mediaPlayer.OnLoop += (sender, e) =>
{
    Console.WriteLine("Playback looped!");
    // Mettre à jour le compteur de boucle, effectuer des actions au point de boucle, etc.
};
```

**Quand il se déclenche** :
- Uniquement lorsque la propriété `Loop` est `true`
- Chaque fois que la lecture cycle de la fin vers le début
- Après le positionnement au début (le cas échéant)

**Cas d'usage** :
- Suivre les itérations de boucle
- Mettre à jour les compteurs de boucle dans l'UI
- Effectuer des actions à chaque point de boucle
- Journaliser les statistiques de lecture

### Exemples de code pour MediaPlayerCore

#### Exemple 1 : Mode boucle de base

```csharp
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;

// Créer une instance de lecteur multimédia
var player = new MediaPlayerCore();

// Activer le mode boucle
player.Loop = true;

// S'abonner à l'événement de boucle
player.OnLoop += (sender, e) =>
{
    Console.WriteLine($"Loop iteration at {DateTime.Now}");
};

// Définir la source et lire
player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\sample.mp4");
await player.PlayAsync();
```

#### Exemple 2 : Boucle fluide sans positionnement

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Activer la boucle fluide (pas de positionnement au début)
player.Loop = true;
player.Loop_DoNotSeekToBeginning = true;

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\background.mp4");
await player.PlayAsync();
```

#### Exemple 3 : Lire un segment spécifique

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Activer la plage de position
player.Selection_Active = true;

// Lire de 1 minute à 2 minutes
player.Selection_Start = TimeSpan.FromMinutes(1);
player.Selection_Stop = TimeSpan.FromMinutes(2);

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\long-video.mp4");
await player.PlayAsync();
```

#### Exemple 4 : Boucler un segment spécifique

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Activer à la fois la boucle et la plage de position
player.Loop = true;
player.Selection_Active = true;

// Boucler le segment de 30 à 45 secondes
player.Selection_Start = TimeSpan.FromSeconds(30);
player.Selection_Stop = TimeSpan.FromSeconds(45);

// Suivre le nombre de boucles
int loopCount = 0;
player.OnLoop += (sender, e) =>
{
    loopCount++;
    Console.WriteLine($"Segment loop #{loopCount}");
};

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\video.mp4");
await player.PlayAsync();
```

#### Exemple 5 : Mise à jour dynamique de la plage de position

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

player.Selection_Active = true;
player.Selection_Start = TimeSpan.FromSeconds(10);
player.Selection_Stop = TimeSpan.FromSeconds(20);

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\video.mp4");
await player.PlayAsync();

// Plus tard, pendant la lecture, mettre à jour la plage
await Task.Delay(5000);

// Passer à un segment différent
player.Selection_Start = TimeSpan.FromSeconds(30);
player.Selection_Stop = TimeSpan.FromSeconds(40);

// Se positionner à la nouvelle position de début
player.Position_Set_Time(player.Selection_Start);
```

## MediaPlayerCoreX (moteur GStreamer)

MediaPlayerCoreX est le moteur de lecteur multimédia multiplateforme basé sur GStreamer, prenant en charge Windows, Linux, macOS, Android et iOS.

### Propriété du mode boucle

#### Propriété `Loop`

Active ou désactive la lecture en boucle automatique.

```csharp
// Activer le mode boucle
mediaPlayer.Loop = true;

// Désactiver le mode boucle
mediaPlayer.Loop = false;
```

**Valeur par défaut** : `false`

**Comportement** :
- Lorsque activée, la lecture redémarre automatiquement lorsque la fin du flux (EOS) est atteinte
- Le `MediaBlocksPipeline` sous-jacent gère la logique de boucle
- L'événement `OnLoop` se déclenche chaque fois que la lecture redémarre
- Redémarre la lecture sans surcharge de positionnement

### Propriétés de plage de position

#### Propriété `Segment_Start`

Définit la position de début pour la lecture par segment.

```csharp
// Commencer la lecture à 45 secondes
mediaPlayer.Segment_Start = TimeSpan.FromSeconds(45);

// Commencer la lecture à 3 minutes
mediaPlayer.Segment_Start = TimeSpan.FromMinutes(3);
```

**Type** : `TimeSpan`

**Valeur par défaut** : `TimeSpan.Zero`

**Comportement** :
- Définit l'endroit où la lecture doit commencer
- Le lecteur se positionne automatiquement à cet endroit au démarrage
- Utilisée en combinaison avec `Segment_Stop` pour définir la plage de lecture
- Appliquée via la propriété `MediaBlocksPipeline.StartPosition` sous-jacente

#### Propriété `Segment_Stop`

Définit la position de fin pour la lecture par segment.

```csharp
// Arrêter la lecture à 2 minutes
mediaPlayer.Segment_Stop = TimeSpan.FromMinutes(2);

// Lire jusqu'à la fin (pas de position d'arrêt)
mediaPlayer.Segment_Stop = TimeSpan.Zero;
```

**Type** : `TimeSpan`

**Valeur par défaut** : `TimeSpan.Zero`

**Comportement** :
- Définit l'endroit où la lecture doit se terminer
- Lorsque la lecture atteint cette position, la fin du flux (EOS) est déclenchée
- Si `Loop` est activée, la lecture redémarre depuis `Segment_Start`
- Utilisez `TimeSpan.Zero` pour aucune position d'arrêt (lire jusqu'à la fin)
- Appliquée via la propriété `MediaBlocksPipeline.StopPosition` sous-jacente

### Événements MediaPlayerCoreX

#### Événement `OnLoop`

Se déclenche chaque fois que la lecture redémarre en mode boucle.

```csharp
mediaPlayer.OnLoop += (sender, e) =>
{
    Console.WriteLine("Playback looped!");
};
```

**Quand il se déclenche** :
- Uniquement lorsque la propriété `Loop` est `true`
- Lorsque la fin du flux (EOS) est atteinte
- Avant que la lecture ne redémarre

### Exemples de code pour MediaPlayerCoreX

#### Exemple 1 : Mode boucle de base

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

// Créer une instance de lecteur multimédia
var player = new MediaPlayerCoreX();

// Activer le mode boucle
player.Loop = true;

// S'abonner à l'événement de boucle
player.OnLoop += (sender, e) =>
{
    Console.WriteLine($"Loop iteration at {DateTime.Now}");
};

// Définir la source et lire
var source = await UniversalSourceSettings.CreateAsync(new Uri(@"C:\Videos\sample.mp4"));

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Exemple 2 : Lire un segment spécifique

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Définir la plage du segment : lire de 30s à 90s
player.Segment_Start = TimeSpan.FromSeconds(30);
player.Segment_Stop = TimeSpan.FromSeconds(90);

var source = await UniversalSourceSettings.CreateAsync(new Uri(@"C:\Videos\long-video.mp4"));

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Exemple 3 : Boucler un segment spécifique

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Activer la boucle et définir le segment
player.Loop = true;
player.Segment_Start = TimeSpan.FromMinutes(1);
player.Segment_Stop = TimeSpan.FromMinutes(2);

// Suivre le nombre de boucles
int loopCount = 0;
player.OnLoop += (sender, e) =>
{
    loopCount++;
    Console.WriteLine($"Segment loop #{loopCount}");
};

var source = await UniversalSourceSettings.CreateAsync(new Uri(@"C:\Videos\video.mp4"));

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Exemple 4 : Vidéo en boucle multiplateforme

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Activer la boucle fluide
player.Loop = true;

// Pour un chemin de fichier vidéo multiplateforme
string videoPath;
#if ANDROID
    videoPath = "/storage/emulated/0/Movies/background.mp4";
#elif IOS
    videoPath = Path.Combine(NSBundle.MainBundle.BundlePath, "background.mp4");
#else
    videoPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.MyVideos), "background.mp4");
#endif

var source = await UniversalSourceSettings.CreateAsync(new Uri(videoPath));

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Exemple 5 : Aperçu de segment avec mise à jour UI

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Mode aperçu : afficher des clips de 10 secondes
TimeSpan clipDuration = TimeSpan.FromSeconds(10);

async Task PreviewSegment(TimeSpan startTime)
{
    player.Segment_Start = startTime;
    player.Segment_Stop = startTime + clipDuration;
    
    // Se positionner à la position de début
    await player.Position_SetAsync(startTime);
    
    Console.WriteLine($"Previewing segment: {startTime} to {startTime + clipDuration}");
}

var source = await UniversalSourceSettings.CreateAsync(new Uri(@"C:\Videos\movie.mp4"));

await player.OpenAsync(source);
await player.PlayAsync();

// Aperçu du premier segment
await PreviewSegment(TimeSpan.FromSeconds(0));

// Aperçu du segment démarrant à 30 secondes
await Task.Delay(11000);
await PreviewSegment(TimeSpan.FromSeconds(30));
```

#### Exemple 6 : Boucle avec pause à l'arrêt

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Mode boucle avec pause au lieu d'arrêt en fin
player.Loop = true;
player.PauseOnStop = true; // Pause au lieu d'arrêt à l'EOS

player.OnLoop += (sender, e) =>
{
    Console.WriteLine("Reached end, pausing briefly before loop...");
    
    // Attendre avant de reprendre (si nécessaire)
    Task.Delay(1000).ContinueWith(_ => 
    {
        // La lecture redémarrera automatiquement grâce à Loop = true
    });
};

var source = await UniversalSourceSettings.CreateAsync(new Uri(@"C:\Videos\video.mp4"));

await player.OpenAsync(source);
await player.PlayAsync();
```

## Bonnes pratiques

### Considérations de performance

1. **Bouclage fluide (MediaPlayerCore)** :
   - Utilisez `Loop_DoNotSeekToBeginning = true` pour des boucles fluides sans artefacts visuels
   - Testez avec votre format multimédia spécifique pour les meilleurs résultats

2. **Segments courts** :
   - Pour des segments très courts (< 1 seconde), assurez-vous que le média est correctement indexé
   - Certains formats peuvent ne pas prendre en charge un positionnement précis à l'image près

3. **Multiplateforme (MediaPlayerCoreX)** :
   - Testez sur toutes les plateformes cibles car le comportement GStreamer peut légèrement varier
   - Utilisez des codecs vidéo appropriés qui prennent en charge le positionnement (H.264, H.265)

### Cas d'usage courants

#### Boucle vidéo de kiosque
```csharp
// MediaPlayerCore (Windows)
player.Loop = true;
player.Loop_DoNotSeekToBeginning = true;

// MediaPlayerCoreX (multiplateforme)
player.Loop = true;
```

#### Fenêtre d'aperçu
```csharp
// MediaPlayerCore
player.Selection_Active = true;
player.Selection_Start = previewStart;
player.Selection_Stop = previewEnd;

// MediaPlayerCoreX
player.Segment_Start = previewStart;
player.Segment_Stop = previewEnd;
```

#### Boucle de segment continue
```csharp
// MediaPlayerCore
player.Loop = true;
player.Selection_Active = true;
player.Selection_Start = segmentStart;
player.Selection_Stop = segmentEnd;

// MediaPlayerCoreX
player.Loop = true;
player.Segment_Start = segmentStart;
player.Segment_Stop = segmentEnd;
```

### Dépannage

#### La boucle ne fonctionne pas

**MediaPlayerCore** :
- Vérifiez que la propriété `Loop` est définie avant d'appeler `PlayAsync()`
- Vérifiez que l'événement `OnStop` n'appelle pas la méthode `Stop()`
- Vérifiez que le fichier multimédia n'est pas corrompu

**MediaPlayerCoreX** :
- Vérifiez que la propriété `Loop` est définie avant d'appeler `PlayAsync()`
- Vérifiez que GStreamer est correctement initialisé
- Vérifiez que le pipeline n'est pas libéré prématurément

#### Plage de position imprécise

**MediaPlayerCore** :
- Vérifiez que `Selection_Active` est défini à `true`
- Vérifiez que `Selection_Start` < `Selection_Stop`
- Certains formats multimédias peuvent ne pas prendre en charge un positionnement précis à l'image près

**MediaPlayerCoreX** :
- Vérifiez que `Segment_Start` et `Segment_Stop` sont valides
- Utilisez des formats multimédias positionnables (MP4, MKV avec indexation correcte)
- Vérifiez que le fichier multimédia prend en charge le positionnement (toutes les sources de streaming ne le font pas)

#### La lecture du segment commence à la mauvaise position

**Les deux moteurs** :
- Vérifiez que les positions sont définies avant d'appeler `PlayAsync()`
- Attendez que le média soit entièrement chargé avant de définir les positions
- Utilisez un positionnement basé sur les images-clés pour une meilleure précision

## Tableau comparatif des propriétés

| Fonctionnalité | MediaPlayerCore | MediaPlayerCoreX |
|---------|-----------------|------------------|
| **Mode boucle** | `Loop` (bool) | `Loop` (bool) |
| **Boucle fluide** | `Loop_DoNotSeekToBeginning` (bool) | Intégrée (pas de propriété supplémentaire) |
| **Plage active** | `Selection_Active` (bool) | Implicite (quand les propriétés Segment sont définies) |
| **Position de début** | `Selection_Start` (TimeSpan) | `Segment_Start` (TimeSpan) |
| **Position de fin** | `Selection_Stop` (TimeSpan) | `Segment_Stop` (TimeSpan) |
| **Événement de boucle** | `OnLoop` | `OnLoop` |
| **Plateforme** | Windows uniquement | Multiplateforme |
| **Moteur** | DirectShow | GStreamer |

## Foire aux questions

### Comment créer une boucle vidéo fluide sans artefacts en C# ?

Pour le moteur DirectShow (MediaPlayerCore), définissez `Loop = true` et `Loop_DoNotSeekToBeginning = true` pour éviter un retour visible entre les itérations. Pour le moteur multiplateforme GStreamer (MediaPlayerCoreX), le redémarrage fluide est intégré — il suffit de définir `Loop = true`. Dans les deux cas, utilisez des formats bien indexés comme MP4 avec encodage H.264 ou H.265 pour les transitions les plus fluides.

### Media Player SDK prend-il en charge un positionnement précis à l'image près pour la lecture par segments ?

La précision du positionnement dépend du conteneur multimédia et du codec. Les fichiers MP4 et MKV avec une indexation correcte des images-clés fournissent les meilleurs résultats. Pour les segments très courts (moins d'une seconde), le lecteur peut atterrir sur l'image-clé la plus proche plutôt que sur la position exacte demandée. Si une précision au niveau de l'image est critique, encodez votre source avec une longueur de GOP (Group of Pictures) courte pour augmenter la densité des images-clés.

### Puis-je boucler une plage de temps spécifique au lieu du fichier vidéo entier ?

Oui. Dans MediaPlayerCore, activez `Loop` et `Selection_Active`, puis définissez `Selection_Start` et `Selection_Stop` pour définir le segment. Dans MediaPlayerCoreX, activez `Loop` et définissez `Segment_Start` et `Segment_Stop`. Le lecteur répétera continuellement uniquement la plage spécifiée. Consultez les exemples de code « Boucler un segment spécifique » ci-dessus pour les deux moteurs.

## Voir aussi

- [Construire un lecteur vidéo en C#](video-player-csharp.md) — implémentation complète du lecteur avec contrôles de lecture et positionnement
- [Lecteur .NET MAUI](maui-player.md) — iOS, Android, macOS et Windows à partir d'une seule base de code C#
- [Lecteur multimédia Avalonia](avalonia-player.md) — lecteur multiplateforme avec patron MVVM
- [Construire un lecteur vidéo en VB.NET](video-player-vb-net.md) — lecteur VB.NET avec positionnement et contrôle du volume
- [Exemples de code](../code-samples/index.md) — exemples d'extraction d'image, listes de lecture et lecture inversée
- [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) — page produit et téléchargements
