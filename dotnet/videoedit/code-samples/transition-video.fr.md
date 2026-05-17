---
title: Effets de transition entre clips vidéo en C# .NET VisioForge
description: Appliquez des effets de transition entre clips vidéo avec VisioForge Video Edit SDK .NET. Fondus, balayages et plus de 100 transitions SMPTE en C#.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Editing
  - MP4
  - AVI
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoTransition
  - VideoSource
  - VideoFileSource
  - MP4Output

---

# Créer des transitions vidéo professionnelles entre clips en C #

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction aux transitions vidéo

Les transitions vidéo créent un flux visuel fluide entre différents clips vidéo dans vos projets d'édition. Des transitions efficaces peuvent améliorer considérablement l'expérience de visualisation, donnant à vos vidéos une apparence plus professionnelle et attrayante. Ce guide montre comment mettre en œuvre des transitions dans vos applications C# en utilisant Video Edit SDK .Net.

Les transitions nécessitent des segments de timeline qui se chevauchent où les deux vidéos existent simultanément. Pendant ce chevauchement, l'effet de transition se produit, remplaçant progressivement la première vidéo par la seconde. Le SDK prend en charge plus de 100 effets de transition différents, des simples fondus aux balayages SMPTE standard complexes.

## Comprendre le positionnement sur la timeline pour les transitions

Pour que les transitions fonctionnent correctement, vous devez comprendre comment les clips vidéo sont positionnés sur une timeline. Voici comment fonctionne le positionnement :

1. **Première vidéo** : placée au début de la timeline (position 0 ms)
2. **Deuxième vidéo** : placée avec un léger chevauchement avec la première vidéo
3. **Transition** : appliquée dans la région de chevauchement où les deux vidéos existent

Cette région de chevauchement est cruciale - c'est là que l'effet de transition sera rendu.

## Créer des fragments vidéo pour la transition

Ajoutons deux fragments vidéo provenant de fichiers séparés, chacun d'une durée de 5 secondes (5000 ms). Le premier fragment sera positionné au début de la timeline, et le second fragment commencera à la marque des 4 secondes, créant un chevauchement d'une seconde où notre transition se produira.

=== "VideoEditCore"

    
    ```cs
    // Définir les chemins vers nos fichiers vidéo sources
    string[] files = { @"c:\samples\video1.avi", @"c:\samples\video2.avi" };
    
    // Créer la première source vidéo - ce sera le premier clip de notre timeline
    var videoFile = new VideoSource(
            files[0],                         // Chemin du premier fichier vidéo
            TimeSpan.Zero,                    // Démarrer depuis le début du fichier source
            TimeSpan.FromMilliseconds(5000),  // Utiliser 5 secondes de la vidéo
            VideoEditStretchMode.Letterbox,   // Conserver le rapport d'aspect, ajouter des bandes noires si nécessaire
            0,                                // Pas de rotation (0 degrés)
            1.0);                             // Vitesse de lecture normale (1.0x)
    
    // Créer la seconde source vidéo - ce sera notre second clip avec chevauchement
    var videoFile2 = new VideoSource(
            files[1],                         // Chemin du second fichier vidéo
            TimeSpan.Zero,                    // Démarrer depuis le début du fichier source
            TimeSpan.FromMilliseconds(5000),  // Utiliser 5 secondes de la vidéo
            VideoEditStretchMode.Letterbox,   // Conserver le rapport d'aspect, ajouter des bandes noires si nécessaire
            0,                                // Pas de rotation (0 degrés)
            1.0);                             // Vitesse de lecture normale (1.0x)
    
    // Ajouter la première vidéo au début de la timeline (position 0 ms)
    await VideoEdit1.Input_AddVideoFileAsync(
            videoFile,
            TimeSpan.FromMilliseconds(0));    // Position sur la timeline : 0 ms (début)
    
    // Ajouter la seconde vidéo à 4 secondes, créant un chevauchement d'une seconde avec la première vidéo
    // Ce chevauchement sera l'endroit où notre transition se produira
    await VideoEdit1.Input_AddVideoFileAsync(
            videoFile2,
            TimeSpan.FromMilliseconds(4000)); // Position sur la timeline : 4000 ms (4 secondes)
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    // Définir les chemins vers nos fichiers vidéo sources
    string[] files = { @"c:\samples\video1.avi", @"c:\samples\video2.avi" };
    
    // Créer la première source vidéo - ce sera le premier clip de notre timeline.
    // VideoFileSource(filename, startTime, stopTime, streamNumber, rate) — l'argument int
    // est l'index de flux audio/vidéo, pas un angle de rotation. Passez 0 pour utiliser
    // le flux par défaut.
    var videoFile = new VideoFileSource(
            files[0],                         // Chemin du premier fichier vidéo
            TimeSpan.Zero,                    // Démarrer depuis le début du fichier source
            TimeSpan.FromMilliseconds(5000),  // Utiliser 5 secondes de la vidéo
            0,                                // streamNumber — 0 sélectionne le flux vidéo par défaut
            1.0);                             // Vitesse de lecture normale (1.0x)

    // Créer la seconde source vidéo - ce sera notre second clip avec chevauchement
    var videoFile2 = new VideoFileSource(
            files[1],                         // Chemin du second fichier vidéo
            TimeSpan.Zero,                    // Démarrer depuis le début du fichier source
            TimeSpan.FromMilliseconds(5000),  // Utiliser 5 secondes de la vidéo
            0,                                // streamNumber — 0 sélectionne le flux vidéo par défaut
            1.0);                             // Vitesse de lecture normale (1.0x)
    
    // Ajouter la première vidéo au début de la timeline (position 0 ms)
    VideoEdit1.Input_AddVideoFile(
            videoFile,
            TimeSpan.FromMilliseconds(0));    // Position sur la timeline : 0 ms (début)
    
    // Ajouter la seconde vidéo à 4 secondes, créant un chevauchement d'une seconde avec la première vidéo
    // Ce chevauchement crée la région où notre transition se produira
    VideoEdit1.Input_AddVideoFile(
            videoFile2,
            TimeSpan.FromMilliseconds(4000)); // Position sur la timeline : 4000 ms (4 secondes)
    ```
    


### Comprendre les paramètres

Lors de l'ajout de fichiers vidéo à la timeline, chaque paramètre sert un objectif spécifique :

- **Chemin du fichier** : emplacement du fichier vidéo sur le disque
- **Heure de début** : position dans la vidéo source à partir de laquelle commencer (TimeSpan.Zero signifie le début)
- **Durée** : longueur de la vidéo à utiliser (5000 ms dans notre exemple)
- **Mode d'étirement** (VideoEditCore uniquement) : comment gérer les différences de rapport d'aspect (Letterbox, Stretch, etc.)
- **Numéro de flux** : index du flux vidéo/audio à lire dans le fichier source (0 = défaut). C'est l'argument entier entre StopTime et rate — *pas* un angle de rotation.
- **Vitesse de lecture** : multiplicateur de vitesse (1.0 signifie vitesse normale)
- **Heure d'insertion** : position sur la timeline où ce clip doit être placé

## Mise en œuvre de l'effet de transition

Maintenant que nous avons nos deux clips vidéo qui se chevauchent, nous allons ajouter un effet de transition qui se produira entre les marques de 4 et 5 secondes sur notre timeline.

=== "VideoEditCore"

    
    D'abord, obtenons l'ID de notre effet de transition souhaité :
    
    ```cs
    // Obtenir l'ID pour l'effet de transition « Upper right ».
    // Les API Video_Transition_* sont des méthodes d'instance sur VideoEditCore — appelez-les sur l'instance du moteur, pas sur le type.
    int id = VideoEdit1.Video_Transition_GetIDFromName("Upper right");
    ```
    
    Ensuite, nous ajouterons la transition en spécifiant l'heure de début, l'heure de fin et l'ID de transition :
    
    ```cs
    // Ajouter la transition à la timeline
    // Paramètres :
    // - Heure de début : 4000 ms (où commence le second clip et où débute le chevauchement)
    // - Heure de fin : 5000 ms (où se termine le premier clip et où se termine le chevauchement)
    // - ID de transition : l'ID que nous avons récupéré pour la transition « Upper right »
    VideoEdit1.Video_Transition_Add(TimeSpan.FromMilliseconds(4000), TimeSpan.FromMilliseconds(5000), id);
    ```
    
    Pour voir tous les effets de transition disponibles, vous pouvez utiliser :
    
    ```cs
    // Obtenir la collection de tous les noms d'effets de transition disponibles (méthode d'instance).
    ObservableCollection<string> availableTransitions = VideoEdit1.Video_Transition_Names();
    
    // Exemple d'itération sur toutes les transitions disponibles
    foreach (string transitionName in availableTransitions)
    {
        // Obtenir l'ID pour chaque transition
        int transitionId = VideoEdit1.Video_Transition_GetIDFromName(transitionName);
        // Vous pourriez l'utiliser dans l'UI de votre application pour permettre aux utilisateurs de choisir des transitions
        Console.WriteLine($"Transition: {transitionName}, ID: {transitionId}");
    }
    ```
    

=== "VideoEditCoreX"

    
    Dans VideoEditCoreX, nous pouvons d'abord lister toutes les transitions disponibles :
    
    ```cs
    // Obtenir tous les noms de transitions disponibles sous forme de tableau
    var transitionNames = VideoEdit1.Video_Transitions_Names();
    
    // Sélectionner une transition spécifique par index
    // Note : le tableau commence à zéro, donc l'index 10 est la 11e transition de la liste
    var transitionName = transitionNames[10]; 
    
    // Vous pourriez également itérer sur toutes les transitions pour les afficher dans une liste déroulante d'UI
    // foreach (var name in transitionNames)
    // {
    //     Console.WriteLine($"Available transition: {name}");
    // }
    ```
    
    Ensuite, nous créerons un objet de transition et l'ajouterons à notre timeline :
    
    ```cs
    // Créer un nouvel objet de transition en spécifiant :
    // - Le nom de la transition sélectionnée ci-dessus
    // - Heure de début (4000 ms) - où commence le chevauchement
    // - Heure de fin (5000 ms) - où se termine le chevauchement
    var trans = new VideoTransition(
            transitionName,                          // Le nom de la transition 
            TimeSpan.FromMilliseconds(4000),         // Heure de début de la transition
            TimeSpan.FromMilliseconds(5000));        // Heure de fin de la transition
    
    // Ajouter la transition à la collection de transitions du composant VideoEdit
    VideoEdit1.Video_Transitions.Add(trans);
    ```
    
    Vous pouvez également spécifier directement le nom de la transition si vous le connaissez :
    
    ```cs
    // Créer une transition en utilisant un nom spécifique sans le rechercher au préalable.
    // Le constructeur prenant une string analyse via Enum.Parse(typeof(VideoTransitionType), name).
    // Les véritables membres de VideoTransitionType sont : "Crossfade", "FadeIn", "FadeOut",
    // plus 100+ wipes SMPTE (par ex. "BarWipeLr", "BarWipeTb", "BoxWipeTl",
    // "BoxWipeTr", "ClockCw12", "IrisRect", "IrisDiamond", etc.). Passez tout
    // autre identifiant (comme "Fade", "FadeFromBlack", "Push", "Slide",
    // "Iris", "Pixelate") et Enum.Parse lèvera ArgumentException à l'exécution.
    // La liste énumérable des noms disponibles retournée par
    // Video_Transitions_GetList est la source de vérité la plus sûre.
    var trans = new VideoTransition(
            "Crossfade",                             // Utilisation d'un identifiant VideoTransitionType réel
            TimeSpan.FromMilliseconds(4000),         // Heure de début de la transition
            TimeSpan.FromMilliseconds(5000));        // Heure de fin de la transition

    // Ajouter la transition au composant VideoEdit
    VideoEdit1.Video_Transitions.Add(trans);

    // Vous pouvez également créer plusieurs transitions entre différents clips :
    // var secondTrans = new VideoTransition("FadeIn", TimeSpan.FromMilliseconds(9000), TimeSpan.FromMilliseconds(10000));
    // VideoEdit1.Video_Transitions.Add(secondTrans);
    ```
    


## Effets de transition populaires et quand les utiliser

Le SDK propose de nombreux effets de transition adaptés à différentes situations :

1. **Transitions par fondu** (fondu enchaîné) : idéales pour des transitions subtiles et élégantes
2. **Transitions par balayage** (horizontal, vertical, diagonal) : excellentes pour des changements de scène dynamiques
3. **Transitions zoom/push** : efficaces pour mettre l'accent sur la scène suivante
4. **Transitions géométriques** (cercle, carré, losange) : créent des effets visuels intéressants
5. **Transitions spéciales** (blocs aléatoires, effets matrice) : pour des transitions créatives ou dramatiques

## Traiter votre vidéo avec des transitions

Après avoir configuré vos clips vidéo et votre transition, vous devrez lancer le traitement :

=== "VideoEditCore"

    
    ```cs
    // ÉTAPE 1 : configurer le chemin du fichier de sortie
    VideoEdit1.Output_Filename = "output.mp4";  // Définir le chemin du fichier de destination
    
    // ÉTAPE 2 : créer et configurer le format de sortie
    var outputFormat = new MP4Output();
    // Ajustez les paramètres de l'encodeur via les sous-paramètres imbriqués Video / Audio, par ex. :
    // (outputFormat.Video as MP4OutputH264Settings).Bitrate = 5000;  // kbit/s
    // La taille de sortie et la fréquence d'images sont déterminées par VideoEdit1.Video_FrameRate et la résolution de la timeline.
    
    // ÉTAPE 3 : assigner le format de sortie au composant VideoEdit
    VideoEdit1.Output_Format = outputFormat;
    
    // ÉTAPE 4 : démarrer le traitement asynchrone
    // Ceci effectuera le rendu de la vidéo avec la transition et l'enregistrera dans le fichier de sortie
    await VideoEdit1.StartAsync();
    
    // Après cet appel, vous devriez écouter les événements de traitement comme :
    // - VideoEdit1.OnProgress pour suivre la progression du traitement
    // - VideoEdit1.OnStop pour détecter la fin du traitement
    ```
    

=== "VideoEditCoreX"

    
    ```cs
    // ÉTAPE 1 : créer et configurer le format de sortie
    // Dans VideoEditCoreX, nous spécifions le nom du fichier de sortie directement dans le constructeur
    var outputFormat = new MP4Output("output.mp4");
    
    // Ajustez les paramètres de l'encodeur via les sous-paramètres imbriqués Video / Audio sur MP4Output, par ex. :
    // (outputFormat.Video as H264EncoderSettings).Bitrate = 5000;  // kbit/s
    // (outputFormat.Audio as AACEncoderSettings).Bitrate = 192;    // kbit/s
    // La taille de l'image de sortie provient de VideoEdit1.Output_VideoSize et la fréquence d'images de Output_VideoFrameRate.
    
    // ÉTAPE 2 : assigner le format de sortie au composant VideoEdit
    VideoEdit1.Output_Format = outputFormat;
    
    // ÉTAPE 3 : démarrer le traitement (appel synchrone ; retourne true en cas de succès)
    // Ceci effectuera le rendu de la vidéo avec la transition et l'enregistrera dans le fichier de sortie.
    VideoEdit1.Start();
    
    // Vous devriez également configurer les gestionnaires d'événements avant d'appeler Start() :
    // VideoEdit1.OnProgress += (s, e) => { Console.WriteLine($"Progress: {e.Progress}%"); };
    // VideoEdit1.OnStop += (s, e) => { Console.WriteLine("Processing completed!"); };
    ```
    


## Défis courants des transitions et solutions

Lors de la mise en œuvre de transitions vidéo, vous pourriez rencontrer ces défis courants :

### Défi 1 : les transitions n'apparaissent pas

Si vos transitions ne s'affichent pas :

- Assurez-vous que les clips vidéo se chevauchent réellement sur la timeline
- Vérifiez que la plage de temps de la transition se situe dans ce chevauchement
- Vérifiez que le nom ou l'ID de transition est valide

### Défi 2 : qualité visuelle médiocre

Pour des transitions de meilleure qualité :

- Utilisez des vidéos sources de résolution plus élevée
- Utilisez un débit binaire plus élevé pour votre sortie
- Envisagez d'ajouter un léger effet de flou pour des transitions plus fluides

### Défi 3 : problèmes de performance

Si le rendu de la transition est lent :

- Utilisez l'accélération matérielle si disponible
- Simplifiez les transitions complexes lorsque vous ciblez du matériel moins puissant
- Envisagez de pré-rendre les transitions pour les applications critiques en performance

## Dépendances requises

Pour mettre en œuvre des transitions vidéo en utilisant Video Edit SDK, vous aurez besoin de :

- Paquets redist Video Edit SDK : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

Pour des conseils sur l'installation de ces dépendances, consultez notre [guide de déploiement](../deployment.md).

## Techniques de transition avancées

Pour des effets de transition plus avancés :

1. **Combinaison de transitions avec des effets** : appliquez un effet de flou ou de couleur pendant la transition
2. **Variation des vitesses de transition** : utilisez des durées différentes pour le début et la fin des transitions
3. **Animation par image-clé** : créez des transitions personnalisées avec un contrôle précis
4. **Fondu enchaîné audio** : synchronisez les transitions audio avec vos transitions vidéo

## Conclusion

Les transitions vidéo sont un moyen puissant d'améliorer vos applications vidéo C#. Avec le Video Edit SDK, vous avez accès à une large gamme d'effets de transition qui peuvent être personnalisés pour répondre à vos besoins spécifiques. En suivant les exemples de ce guide, vous pouvez mettre en œuvre des transitions de qualité professionnelle dans vos projets d'édition vidéo.

Pour des options supplémentaires et des informations détaillées sur les transitions SMPTE, consultez notre [référence complète des transitions](../transitions.md).

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.