---
title: Capture matérielle MPEG-2 d'un tuner TV en C# .NET
description: Capturez la vidéo d'un tuner TV en MPEG-2 via encodeurs matériels avec le SDK VisioForge. Guide C# pour applications WPF, WinForms et Console.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Streaming
  - Encoding
  - TV Tuner
  - MPEG-2
  - C#
  - NuGet
primary_api_classes:
  - DirectCaptureMPEGOutput
  - SpecialFilterType
  - VideoCaptureMode

---

# Capture vidéo MPEG-2 via un encodeur matériel de tuner TV en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction à la capture vidéo MPEG-2

Le MPEG-2, introduit en 1995, a révolutionné la vidéo numérique en tant que norme pour « le codage générique des images animées et de l'information audio associée ». Ce format reste largement implémenté sur de nombreuses plateformes, notamment la diffusion télévisuelle numérique, le DVD vidéo et diverses applications de streaming. Bien qu'il s'agisse d'une norme plus ancienne, le MPEG-2 reste pertinent dans certains scénarios de diffusion en raison de son équilibre entre qualité et efficacité.

La capacité de capturer la vidéo directement au format MPEG-2 à l'aide d'encodeurs matériels offre des avantages significatifs aux développeurs créant des applications multimédias. L'encodage matériel décharge le CPU des traitements intensifs, ce qui se traduit par de meilleures performances système et une utilisation plus efficace de la batterie sur les appareils portables.

## Pourquoi utiliser l'encodage MPEG-2 accéléré matériellement ?

L'encodage matériel offre plusieurs avantages distincts pour les applications de capture vidéo :

1. **Utilisation CPU réduite** — En tirant parti du matériel d'encodage dédié, votre application peut maintenir des performances réactives pendant la capture vidéo
2. **Meilleure autonomie de la batterie** — Critique pour les applications portables où l'efficacité énergétique compte
3. **Encodage en temps réel** — Les encodeurs matériels peuvent traiter de la vidéo haute résolution en temps réel
4. **Qualité constante** — Les encodeurs matériels offrent des performances d'encodage fiables

Les tuners TV équipés d'encodeurs MPEG-2 matériels intégrés sont particulièrement précieux pour les applications qui ont besoin de capturer efficacement du contenu de diffusion. Ces périphériques gèrent à la fois le tuning et l'encodage, simplifiant l'architecture de votre application.

## Guide d'implémentation

Ce guide décrit la mise en œuvre de la capture vidéo MPEG-2 via des tuners TV avec encodeurs MPEG internes dans des applications .NET. Les exemples de code fonctionnent dans les applications WPF, WinForms et console.

### Prérequis

Avant d'implémenter la capture vidéo MPEG-2, assurez-vous d'avoir :

- Installé le Video Capture SDK .Net
- Un périphérique tuner TV compatible avec un encodage MPEG-2 interne
- Des paquets redist correctement configurés pour votre plateforme cible
- Une compréhension de base des concepts de capture vidéo en .NET

### Configuration du périphérique

[Tout d'abord, configurez le périphérique de capture vidéo](../video-sources/video-capture-devices/index.md) en suivant les procédures standards. Cela inclut la sélection du périphérique d'entrée correct et la configuration des paramètres vidéo de base.

Le tuner TV doit être correctement installé et reconnu par votre système d'exploitation avant de pouvoir être utilisé par votre application. Utilisez le Gestionnaire de périphériques Windows pour vérifier que le périphérique est correctement installé et fonctionne.

### Étape 1 : énumérer les encodeurs matériels MPEG-2 disponibles

Votre première tâche consiste à découvrir quels encodeurs matériels MPEG-2 sont disponibles sur le système. Cela permet aux utilisateurs de sélectionner l'encodeur approprié à leurs besoins :

```cs
// Obtenir tous les encodeurs vidéo matériels disponibles sur le système
foreach (var specialFilter in VideoCapture1.Special_Filters(SpecialFilterType.HardwareVideoEncoder))
{
  // Ajouter chaque encodeur à une liste déroulante ou de sélection
  cbMPEGEncoder.Items.Add(specialFilter);
}
```

Ce code énumère tous les encodeurs vidéo matériels et alimente un élément d'interface de sélection, permettant aux utilisateurs de choisir leur périphérique d'encodage préféré.

### Étape 2 : sélectionner l'encodeur MPEG-2

Une fois que l'utilisateur a sélectionné un encodeur matériel, définissez-le comme encodeur actif pour la session de capture :

```cs
// Appliquer l'encodeur MPEG-2 sélectionné au périphérique de capture vidéo
VideoCapture1.Video_CaptureDevice.InternalMPEGEncoder_Name = cbMPEGEncoder.Text;
```

Cette ligne configure le composant de capture vidéo pour utiliser l'encodeur matériel sélectionné par l'utilisateur. La propriété `InternalMPEGEncoder_Name` accepte le nom du périphérique encodeur exactement tel qu'il est retourné par l'énumération `Special_Filters`.

### Étape 3 : configurer la sortie DirectCapture MPEG

Ensuite, configurez le format de sortie pour utiliser DirectCapture MPEG, qui optimise le pipeline de capture pour le MPEG-2 encodé matériellement :

```cs
// Définir le format de sortie pour la capture MPEG-2
VideoCapture1.Output_Format = new DirectCaptureMPEGOutput();
```

La classe `DirectCaptureMPEGOutput` gère les exigences spécifiques de la sortie au format MPEG-2, y compris le formatage correct du conteneur et le multiplexage des flux.

### Étape 4 : définir le mode de capture vidéo et démarrer la capture

Enfin, configurez le mode de capture, spécifiez le nom du fichier de sortie et démarrez le processus de capture :

```cs
// Configurer le mode de capture pour l'enregistrement vidéo
VideoCapture1.Mode = VideoCaptureMode.VideoCapture;

// Définir le nom du fichier de sortie pour le fichier MPEG-2 capturé
VideoCapture1.Output_Filename = "output.mpg";

// Démarrer le processus de capture asynchrone
await VideoCapture1.StartAsync();
```

La méthode `StartAsync` démarre le processus de capture de manière asynchrone, permettant à votre application de rester réactive pendant la capture.

### Configuration audio

Une configuration audio appropriée est essentielle pour une capture MPEG-2 complète. La plupart des tuners TV avec encodeurs MPEG-2 gèrent l'audio automatiquement, mais vous pouvez vérifier et ajuster les paramètres :

```cs
// S'assurer que l'audio est activé pour la capture
VideoCapture1.Audio_RecordAudio = true;
```

### Gestion des événements de capture

L'implémentation de gestionnaires d'événements améliore l'expérience utilisateur en fournissant un retour pendant le processus de capture :

```cs
// Gérer les erreurs pendant la capture
VideoCapture1.OnError += (sender, args) =>
{
    // Journaliser ou afficher les informations d'erreur
    Console.WriteLine($"Error: {args.Message}");
};
```

## Considérations de performance

Lors de la capture vidéo avec des encodeurs MPEG-2 matériels, prenez en compte ces facteurs de performance :

1. **Vitesse de disque** : assurez-vous que votre périphérique de stockage peut soutenir les vitesses d'écriture requises pour la capture MPEG-2
2. **Paramètres de tampon** : ajustez la taille des tampons en fonction de la mémoire disponible et des performances du système
3. **Traitement en arrière-plan** : minimisez les tâches gourmandes en CPU pendant la capture pour éviter les pertes d'images
4. **Gestion thermique** : les sessions de capture prolongées peuvent provoquer un échauffement du périphérique ; surveillez les températures système

## Dépannage des problèmes courants

### Encodeur introuvable

Si votre application ne parvient pas à trouver des encodeurs matériels, vérifiez :

- Que le périphérique est correctement connecté et alimenté
- Que les pilotes appropriés sont installés
- Que le périphérique prend en charge l'encodage matériel MPEG-2

### Mauvaise qualité vidéo

Si la qualité vidéo capturée est insatisfaisante :

- Vérifiez la force du signal de la source TV
- Vérifiez les paramètres de qualité de l'encodeur
- Assurez-vous des conditions d'éclairage appropriées si vous utilisez une source caméra

### Échecs de capture

En cas d'échec ou de plantage de la capture :

- Vérifiez que le répertoire de sortie est accessible en écriture
- Vérifiez que l'espace disque est suffisant
- Assurez-vous qu'aucune autre application n'utilise le périphérique de capture

## Redistribuables requis

Pour un fonctionnement correct, votre application a besoin de ces paquets redist :

- Capture vidéo redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

Installez le paquet approprié en fonction de l'architecture cible de votre application.

## Conclusion

L'implémentation de la capture vidéo MPEG-2 à l'aide de tuners TV avec encodeurs matériels permet un enregistrement de diffusion efficace dans vos applications .NET. L'accélération matérielle offre des avantages en termes de performances tout en conservant une bonne qualité vidéo. En suivant les étapes décrites dans ce guide, vous pouvez créer des solutions de capture vidéo robustes pour diverses applications de diffusion.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
