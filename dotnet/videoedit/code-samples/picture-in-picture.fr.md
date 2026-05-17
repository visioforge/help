---
title: Vidéos image dans l'image et écran partagé en C# .NET
description: Créez des vidéos image dans l'image et écran partagé en C# .NET avec mises en page personnalisées et export MP4 grâce à Video Edit SDK.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Streaming
  - Editing
  - MP4
  - AVI
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - FileSegment
  - VideoSource

---

# Créer des vidéos image dans l'image et écran partagé en .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction aux techniques de composition vidéo

Les applications vidéo professionnelles nécessitent souvent de combiner plusieurs sources vidéo en une seule sortie. Cette capacité est essentielle pour créer du contenu attrayant tel que des tutoriels avec incrustations du présentateur, des vidéos de réaction, des panels d'interviews ou des retransmissions sportives avec fenêtres de ralenti. Le Video Edit SDK pour .NET facilite la mise en œuvre de ces techniques avancées dans vos applications C#.

Ce guide couvre trois approches principales de composition vidéo :

1. **Image dans l'image (PIP)** : placer une superposition vidéo plus petite par-dessus une vidéo principale
2. **Partage horizontal** : positionner deux vidéos côte à côte horizontalement
3. **Partage vertical** : disposer deux vidéos l'une au-dessus de l'autre

Chaque technique a des cas d'usage spécifiques et peut être personnalisée pour créer exactement la présentation visuelle requise par votre application.

## Créer des superpositions vidéo image dans l'image

Image dans l'image est la technique de composition vidéo la plus courante, où une vidéo plus petite apparaît dans un coin ou à une position personnalisée sur une vidéo d'arrière-plan plus grande. C'est parfait pour créer des vidéos de commentaires, des tutoriels ou tout contenu où vous souhaitez montrer deux perspectives simultanément sans diviser l'écran de manière égale.

### Étape 1 : définir vos fichiers vidéo

D'abord, spécifiez les chemins de fichier des vidéos que vous souhaitez combiner :

```cs
string[] files = {  "!video.avi", "!video2.wmv" };
```

Vous pouvez utiliser divers formats vidéo, notamment MP4, AVI, MOV, WMV et bien d'autres pris en charge par le SDK.

### Étape 2 : créer des segments pour la vidéo principale

Définissez quelle portion de la vidéo principale utiliser en définissant les heures de début et de fin :

```cs
FileSegment[] segments1 = new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(10000)) };
```

Cet exemple utilise les 10 premières secondes de la vidéo principale. Vous pouvez ajuster ces valeurs pour utiliser n'importe quel segment de votre fichier source.

### Étape 3 : initialiser la source vidéo principale

Créez un objet VideoSource pour votre vidéo principale avec vos paramètres préférés :

```cs
var videoFile = new VideoSource(
                                files[0],
                                segments1,
                                VideoEditStretchMode.Letterbox,
                                0,
                                1.0);
```

Les paramètres incluent :

- Le chemin du fichier
- Les segments temporels à inclure
- Le mode d'étirement (comment gérer les différences de rapport d'aspect)
- Le numéro de flux (index commençant à zéro dans un fichier multi-flux)
- La vitesse de lecture (1.0 = vitesse normale ; 2.0 = avance rapide 2× ; 0.5 = demi-vitesse)

### Étape 4 : configurer la source vidéo PIP

De manière similaire, configurez la vidéo qui apparaîtra en superposition :

```cs
FileSegment[] segments2 = new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(10000)) };

var videoFile2 = new VideoSource(
                                files[1],
                                segments2,
                                VideoEditStretchMode.Letterbox,
                                0,
                                1.0);
```

### Étape 5 : définir les rectangles de placement des vidéos

Spécifiez la taille et la position pour les deux vidéos :

```cs
// Rectangle pour la vidéo d'arrière-plan principale (image complète)
var rect1 = new Rectangle(0, 0, 1280, 720);

// Rectangle pour la superposition vidéo PIP plus petite
var rect2 = new Rectangle(100, 100, 320, 240);
```

Vous pouvez ajuster la position et la taille du second rectangle pour placer la vidéo PIP où vous le souhaitez à l'écran. Les positions courantes incluent les coins supérieur droit ou inférieur gauche.

### Étape 6 : combiner les vidéos en PIP

Enfin, ajoutez les deux sources vidéo à votre projet en utilisant le mode PIP :

```cs
// Signature : Input_AddVideoFile_PIP(fileSource1, fileSource2, timelineInsertTime, duration,
//   mode, letterbox, customWidth=0, customHeight=0, targetVideoStream=0,
//   rectangle1=null, rectangle2=null)
// rectangle1 positionne fileSource1 (la vidéo principale) ; rectangle2 positionne fileSource2 (la superposition PIP).
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile,                            // fileSource1 — vidéo principale
    videoFile2,                           // fileSource2 — vidéo de superposition PIP
    TimeSpan.FromMilliseconds(0),         // timelineInsertTime
    TimeSpan.FromMilliseconds(10000),     // duration
    VideoEditPIPMode.Custom,              // Mode PIP — Custom permet de passer des rectangles
    true,                                 // letterbox
    1280, 720,                            // customWidth / customHeight
    0,                                    // targetVideoStream
    rect1,                                // rectangle1 — position de la vidéo principale (image complète)
    rect2                                 // rectangle2 — position de la superposition PIP
);
```

La vidéo résultante affichera votre vidéo principale remplissant toute l'image avec la seconde vidéo apparaissant à la position du rectangle spécifié.

## Créer des mises en page vidéo côte à côte

Les mises en page côte à côte divisent l'écran de manière égale entre deux sources vidéo. Cette approche fonctionne bien pour les vidéos de comparaison, le contenu de réaction ou les présentations d'interview où les deux vidéos méritent un espace d'écran égal.

### Écran partagé horizontalement

Un partage horizontal place les vidéos côte à côte horizontalement. Cela fonctionne bien pour comparer des effets avant/après ou montrer deux personnes en conversation :

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile, 
    videoFile2, 
    TimeSpan.FromMilliseconds(0), 
    TimeSpan.FromMilliseconds(10000), 
    VideoEditPIPMode.Horizontal, 
    false
);
```

### Écran partagé verticalement

Un partage vertical empile les vidéos l'une au-dessus de l'autre. Cela peut être utile pour montrer des relations de cause à effet ou créer des mises en page à panneaux haut/bas :

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile, 
    videoFile2, 
    TimeSpan.FromMilliseconds(0), 
    TimeSpan.FromMilliseconds(10000), 
    VideoEditPIPMode.Vertical, 
    false
);
```

## Options de personnalisation avancées

Le SDK offre de nombreuses options pour personnaliser davantage vos compositions vidéo :

- **Positionnement personnalisé** : définissez des coordonnées d'écran exactes pour un placement vidéo précis
- **Transitions** : ajoutez des effets de fondu ou d'autres transitions entre les segments vidéo
- **Contrôle audio** : ajustez les niveaux de volume pour chaque source vidéo indépendamment
- **Effets de bordure** : ajoutez des bordures, ombres ou cadres autour des éléments vidéo PIP
- **Animation** : créez des éléments PIP mouvants qui changent de position au fil du temps
- **Superpositions multiples** : ajoutez plus de deux vidéos pour créer des compositions complexes

Ces capacités vous permettent de créer des productions vidéo de niveau professionnel directement depuis vos applications .NET.

## Exigences d'implémentation

Pour mettre en œuvre avec succès ces techniques de composition vidéo dans votre application, vous devrez inclure les paquets redistribuables appropriés :

- Redist Video Edit SDK [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

Pour des informations sur le déploiement de ces dépendances sur les systèmes de vos utilisateurs, consultez notre [guide de déploiement](../deployment.md).

## Considérations de performance

Lorsque vous travaillez avec plusieurs sources vidéo, en particulier à haute résolution, soyez attentif à l'utilisation des ressources système. Les opérations de composition vidéo peuvent être gourmandes en processeur. Tenez compte des bonnes pratiques suivantes :

- Pré-rendre les compositions complexes pour la lecture dans des environnements de production
- Optimiser la résolution vidéo et le débit binaire pour votre plateforme cible
- Tester les performances sur un matériel similaire à votre environnement de déploiement cible

## Conclusion

Les compositions vidéo image dans l'image et écran partagé ajoutent des capacités professionnelles aux applications multimédias. Le Video Edit SDK pour .NET facilite la mise en œuvre de ces fonctionnalités grâce à son API intuitive. Que vous développiez une application d'édition vidéo, une plateforme de streaming ou que vous intégriez le traitement vidéo dans un système plus vaste, ces techniques offrent des moyens puissants de combiner et présenter plusieurs sources vidéo.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.