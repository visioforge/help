---
title: Listes de lecture en .NET avec exemples de code C#
description: Mettez en œuvre les listes de lecture dans WinForms, WPF et Console avec VisioForge Media Player SDK .NET. Ordre séquentiel ou personnalisé.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCore
  - Windows
  - Playback
  - MP4
  - C#
primary_api_classes:
  - NewFilePlaybackEventArgs

---

# Guide complet de l'API de liste de lecture en .NET

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [MediaPlayerCore](#){ .md-button }

## Introduction à la gestion des listes de lecture

L'API de liste de lecture offre un moyen puissant et flexible de gérer le contenu multimédia dans vos applications .NET. Que vous développiez un lecteur de musique, une application vidéo ou tout autre logiciel centré sur le multimédia, une gestion efficace des listes de lecture est essentielle pour offrir une expérience utilisateur fluide.

Ce guide couvre tout ce que vous devez savoir pour implémenter la fonctionnalité de liste de lecture à l'aide du composant MediaPlayerCore. Vous apprendrez à créer des listes de lecture, à naviguer entre les pistes, à gérer les événements de liste de lecture et à optimiser les performances dans divers environnements .NET.

## Fonctionnalités clés et avantages

- **Intégration simple** : API facile à implémenter qui s'intègre parfaitement aux applications .NET existantes
- **Compatibilité de formats** : prise en charge d'une large gamme de formats audio et vidéo
- **Multiplateforme** : fonctionne de manière cohérente dans les applications WinForms, WPF et console
- **Performances optimisées** : conçu pour une utilisation mémoire efficace et une lecture réactive
- **Architecture événementielle** : système d'événements riche pour créer des interfaces utilisateur réactives

## Prise en main de l'API de liste de lecture

Avant de vous plonger dans les méthodes spécifiques, assurez-vous d'avoir correctement initialisé le composant MediaPlayer dans votre application. Les sections suivantes contiennent des exemples de code pratiques que vous pouvez implémenter directement dans votre projet.

### Création de votre première liste de lecture

Créer une liste de lecture est la première étape pour gérer plusieurs fichiers multimédias. L'API fournit des méthodes simples pour ajouter des fichiers à votre collection de liste de lecture :

```csharp
// Initialiser le lecteur multimédia (en supposant que vous avez ajouté le composant à votre formulaire)
// this.mediaPlayer1 = new MediaPlayer();

// Ajouter des fichiers individuels à la liste de lecture
this.mediaPlayer1.Playlist_Add(@"c:\media\intro.mp4");
this.mediaPlayer1.Playlist_Add(@"c:\media\main_content.mp4");
this.mediaPlayer1.Playlist_Add(@"c:\media\conclusion.mp4");

// Démarrer la lecture depuis le premier élément
this.mediaPlayer1.Play();
```

Cette approche vous permet de construire des listes de lecture par programme, ce qui est idéal pour les applications où le contenu de la liste de lecture est déterminé à l'exécution.

## Opérations de base sur la liste de lecture

### Navigation entre les éléments de la liste

Une fois la liste de lecture créée, vos utilisateurs devront naviguer entre les éléments. L'API fournit des méthodes intuitives pour passer au fichier suivant ou précédent :

```csharp
// Lire le fichier suivant de la liste
this.mediaPlayer1.Playlist_PlayNext();

// Lire le fichier précédent de la liste
this.mediaPlayer1.Playlist_PlayPrevious();
```

Ces méthodes gèrent automatiquement la transition entre les fichiers multimédias, y compris l'arrêt de la lecture en cours et le démarrage du nouvel élément.

### Gestion du contenu de la liste de lecture

Pendant l'exécution de l'application, vous pouvez être amené à modifier la liste de lecture en supprimant certains éléments ou en la vidant entièrement :

```csharp
// Supprimer un fichier spécifique de la liste de lecture
this.mediaPlayer1.Playlist_Remove(@"c:\media\intro.mp4");

// Supprimer tous les éléments de la liste de lecture
this.mediaPlayer1.Playlist_Clear();
```

Cette gestion dynamique du contenu permet à votre application de s'adapter aux préférences des utilisateurs ou à l'évolution des exigences à la volée.

### Récupération des informations de la liste de lecture

Accéder aux informations sur l'état actuel de la liste de lecture est crucial pour créer une interface utilisateur informative :

```csharp
// Obtenir l'index du fichier en cours (indexation à partir de 0)
int currentIndex = this.mediaPlayer1.Playlist_GetPosition();

// Obtenir le nombre total de fichiers de la liste de lecture
int totalFiles = this.mediaPlayer1.Playlist_GetCount();

// Obtenir un nom de fichier spécifique par son index
string fileName = this.mediaPlayer1.Playlist_GetFilename(1); // Obtient le deuxième fichier

// Afficher les informations de lecture en cours
string statusMessage = $"Playing file {currentIndex + 1} of {totalFiles}: {fileName}";
```

Ces méthodes vous permettent de créer des interfaces dynamiques qui reflètent l'état actuel de la lecture multimédia.

## Contrôle avancé de la liste de lecture

### Réinitialisation et repositionnement

Pour un contrôle plus précis sur la navigation dans la liste de lecture, vous pouvez réinitialiser la liste ou sauter à une position spécifique :

```csharp
// Réinitialiser la liste de lecture pour redémarrer depuis le premier fichier
this.mediaPlayer1.Playlist_Reset();

// Sauter à une position spécifique de la liste (indexation à partir de 0)
this.mediaPlayer1.Playlist_SetPosition(2); // Sauter au troisième élément
```

Ces méthodes sont particulièrement utiles pour implémenter des fonctionnalités comme « redémarrer la liste de lecture » ou permettre aux utilisateurs de sélectionner des éléments spécifiques dans une vue de liste.

### Gestion personnalisée des événements pour la navigation dans la liste

Pour créer une application réactive, vous voudrez implémenter une gestion personnalisée des événements pour la navigation dans la liste de lecture. Comme `MediaPlayerCore` ne possède pas d'événement dédié au changement d'élément de la liste, vous pouvez créer votre propre mécanisme de suivi à l'aide des événements existants :

```csharp
private int _lastPlaylistIndex = -1;

// Suivre les changements de position de la liste de lecture lorsque la lecture démarre
private void mediaPlayer1_OnStart(object sender, EventArgs e)
{
    int currentIndex = this.mediaPlayer1.Playlist_GetPosition();
    if (currentIndex != _lastPlaylistIndex)
    {
        _lastPlaylistIndex = currentIndex;
        
        // Gérer le changement d'élément de la liste
        string currentFile = this.mediaPlayer1.Playlist_GetFilename(currentIndex);
        UpdatePlaylistUI(currentIndex, currentFile);
    }
}

// Suivre également le démarrage de la lecture d'un nouveau fichier
private void mediaPlayer1_OnNewFilePlaybackStarted(object sender, NewFilePlaybackEventArgs e)
{
    int currentIndex = this.mediaPlayer1.Playlist_GetPosition();
    _lastPlaylistIndex = currentIndex;
    
    // Gérer le changement d'élément de la liste
    string currentFile = this.mediaPlayer1.Playlist_GetFilename(currentIndex);
    UpdatePlaylistUI(currentIndex, currentFile);
}

// Gérer la fin de la liste de lecture
private void mediaPlayer1_OnPlaylistFinished(object sender, EventArgs e)
{
    // Gérer la fin de la liste de lecture
    this.lblPlaybackStatus.Text = "Playlist finished";
    
    // Optionnellement, réinitialiser la liste ou boucler
    // this.mediaPlayer1.Playlist_Reset();
    // this.mediaPlayer1.Play();
}

private void UpdatePlaylistUI(int index, string filename)
{
    // Mettre à jour les éléments d'interface avec les nouvelles informations
    this.lblCurrentTrack.Text = $"Now playing: {Path.GetFileName(filename)}";
    this.lblTrackNumber.Text = $"Track {index + 1} of {this.mediaPlayer1.Playlist_GetCount()}";
    
    // Mettre à jour la sélection de la liste dans l'interface utilisateur
    // ...
}
```

Cette approche vous permet de détecter les événements de navigation dans la liste de lecture et d'y répondre dans votre application en vous abonnant aux événements réels fournis par MediaPlayerCore :

```csharp
// S'abonner aux événements
this.mediaPlayer1.OnStart += mediaPlayer1_OnStart;
this.mediaPlayer1.OnNewFilePlaybackStarted += mediaPlayer1_OnNewFilePlaybackStarted;
this.mediaPlayer1.OnPlaylistFinished += mediaPlayer1_OnPlaylistFinished;
```

### Opérations asynchrones sur la liste de lecture

MediaPlayerCore propose des versions asynchrones des méthodes de navigation dans la liste de lecture pour une meilleure réactivité :

```csharp
// Lire le fichier suivant de manière asynchrone
await this.mediaPlayer1.Playlist_PlayNextAsync();

// Lire le fichier précédent de manière asynchrone
await this.mediaPlayer1.Playlist_PlayPreviousAsync();
```

L'utilisation de ces méthodes asynchrones est recommandée pour les applications avec interface utilisateur afin d'éviter de bloquer le thread principal pendant les transitions de lecture.

## Modèles d'implémentation et bonnes pratiques

### Implémentation des modes Répéter et Aléatoire

La plupart des lecteurs multimédias incluent une fonctionnalité de répétition et de lecture aléatoire. Voici comment implémenter ces fonctionnalités courantes :

```csharp
private bool repeatEnabled = false;
private bool shuffleEnabled = false;
private Random random = new Random();

// Gérer la navigation dans la liste de lecture lorsqu'une piste se termine. OnStop se déclenche aussi bien
// pour la fin normale du fichier que pour les appels explicites à Stop() — utilisez OnPlaylistFinished
// (ci-dessous) pour ne réagir qu'à la fin de toute la liste de lecture. StopEventArgs expose Successful
// (bool) et Position (TimeSpan) ; il n'y a pas d'enum StopReason.
private void MediaPlayer1_OnStop(object sender, StopEventArgs e)
{
    if (!e.Successful)
    {
        return; // Erreur ou interruption — ne pas avancer automatiquement
    }

    if (repeatEnabled)
    {
        // Simplement rejouer l'élément en cours
        this.mediaPlayer1.Play();
    }
    else if (shuffleEnabled)
    {
        // Lire un élément aléatoire
        int totalFiles = this.mediaPlayer1.Playlist_GetCount();
        int randomIndex = random.Next(0, totalFiles);
        this.mediaPlayer1.Playlist_SetPosition(randomIndex);
        this.mediaPlayer1.Play();
    }
    else
    {
        // Comportement standard : lire le suivant si disponible
        if (this.mediaPlayer1.Playlist_GetPosition() < this.mediaPlayer1.Playlist_GetCount() - 1)
        {
            this.mediaPlayer1.Playlist_PlayNext();
        }
        // Sinon, nous avons atteint la fin de la liste — OnPlaylistFinished se déclenche séparément.
    }
}

// S'abonner à l'événement d'arrêt
this.mediaPlayer1.OnStop += MediaPlayer1_OnStop;
```

### Gestion de la mémoire pour les grandes listes de lecture

Lorsque vous traitez de grandes listes de lecture, envisagez d'implémenter des techniques de chargement différé :

```csharp
// Stocker les informations de la liste de lecture séparément pour les grandes listes
private List<string> masterPlaylist = new List<string>();

public void LoadLargePlaylist(string[] filePaths)
{
    // Vider la liste de lecture existante
    this.mediaPlayer1.Playlist_Clear();
    masterPlaylist.Clear();
    
    // Stocker tous les chemins
    masterPlaylist.AddRange(filePaths);
    
    // Charger uniquement le premier lot d'éléments (par exemple, 100)
    int initialBatchSize = Math.Min(100, filePaths.Length);
    for (int i = 0; i < initialBatchSize; i++)
    {
        this.mediaPlayer1.Playlist_Add(filePaths[i]);
    }
    
    // Démarrer la lecture
    this.mediaPlayer1.Play();
}

// Implémenter le chargement dynamique lorsque l'utilisateur approche de la fin des éléments chargés
private void CheckAndLoadMoreItems()
{
    int currentPosition = this.mediaPlayer1.Playlist_GetPosition();
    int loadedCount = this.mediaPlayer1.Playlist_GetCount();
    
    // Si nous approchons de la fin des éléments chargés mais qu'il en reste dans la liste maîtresse
    if (currentPosition > loadedCount - 10 && loadedCount < masterPlaylist.Count)
    {
        // Charger le lot suivant
        int nextBatchSize = Math.Min(50, masterPlaylist.Count - loadedCount);
        for (int i = 0; i < nextBatchSize; i++)
        {
            this.mediaPlayer1.Playlist_Add(masterPlaylist[loadedCount + i]);
        }
    }
}
```

## Considérations multiplateformes

L'API de liste de lecture fonctionne de manière cohérente dans différents environnements .NET, mais certaines considérations sont spécifiques à chaque plateforme :

### Notes d'implémentation WPF

Lors de l'implémentation dans des applications WPF, vous utiliserez généralement la liaison de données avec votre liste de lecture :

```csharp
// Créer une collection observable à lier à l'interface utilisateur
private ObservableCollection<PlaylistItem> observablePlaylist = new ObservableCollection<PlaylistItem>();

// Synchroniser la collection observable avec la liste de lecture du lecteur
private void SyncObservablePlaylist()
{
    observablePlaylist.Clear();
    for (int i = 0; i < this.mediaPlayer1.Playlist_GetCount(); i++)
    {
        string filename = this.mediaPlayer1.Playlist_GetFilename(i);
        observablePlaylist.Add(new PlaylistItem
        {
            Index = i,
            FileName = System.IO.Path.GetFileName(filename),
            FullPath = filename
        });
    }
}
```

## Conclusion

L'API de liste de lecture fournit une base solide pour créer des applications multimédias riches en fonctionnalités en .NET. En utilisant les méthodes et les modèles décrits dans ce guide, vous pouvez créer des systèmes de gestion de listes de lecture intuitifs qui améliorent l'expérience utilisateur de votre application.

Pour des scénarios plus avancés, explorez les capacités supplémentaires du composant MediaPlayerCore, notamment la gestion personnalisée des événements, l'extraction des métadonnées multimédias et les optimisations spécifiques aux formats.
