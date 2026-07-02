---
title: Exemples de code d'édition vidéo pour C# et VB.NET — SDK
description: Exemples de code C# prêts à l'emploi pour VisioForge Video Edit SDK .NET. Superpositions, transitions, rognage, fusion et contrôle audio avec tutoriels.
sidebar_label: Exemples de code
tags:
  - Video Edit SDK
  - .NET

---

# Exemples de code et tutoriels d'édition vidéo .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

Cette page rassemble des recettes C# prêtes à l'emploi pour les scénarios d'édition les plus courants utilisant Video Edit SDK .Net. Chaque extrait est vérifié par rapport au code source du SDK et aux démos sous [`Video Edit SDK`](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK). Les recettes ci-dessous utilisent le moteur `VideoEditCore` (Windows uniquement). Le code multi-plateforme basé sur `VideoEditCoreX` suit une forme similaire avec des types de source et d'effet spécifiques au moteur.

## Recettes disponibles

### Effets visuels et superpositions

- [**Ajout de superpositions d'image sur la vidéo**](add-image-overlay.md) — Apprenez à superposer des images sur votre contenu vidéo
- [**Mise en œuvre d'une superposition de texte**](add-text-overlay.md) — Techniques d'ajout et de mise en forme de superpositions de texte sur les vidéos
- [**Effets image dans l'image**](picture-in-picture.md) — Créez des effets PiP professionnels dans vos applications vidéo

### Manipulation audio

- [**Effets d'enveloppe de volume audio**](audio-envelope.md) — Contrôlez les variations de volume audio au fil du temps
- [**Plusieurs flux audio dans des fichiers AVI**](multiple-audio-streams-avi.md) — Travailler avec plusieurs pistes audio
- [**Contrôle de volume personnalisé pour les pistes audio**](volume-for-track.md) — Techniques précises de gestion des niveaux audio

### Composition vidéo

- [**Créer des vidéos à partir de plusieurs sources**](output-file-from-multiple-sources.md) — Combinez divers fichiers d'entrée en une seule sortie
- [**Travailler avec des segments vidéo**](several-segments.md) — Extrayez et utilisez plusieurs segments du même fichier source
- [**Effets de transition entre fragments vidéo**](transition-video.md) — Mise en œuvre de transitions fluides entre clips
- [**Générer des vidéos à partir de séquences d'images**](video-images-console.md) — Exemple d'application console pour la conversion image vers vidéo
- [**Intégration vidéo multi-flux audio**](adding-video-file-with-multiple-audio-streams.md) — Travailler avec des combinaisons audio-vidéo complexes

## iOS

- [Éditeur vidéo iOS](ios-video-editor.md) — Création d'applications d'édition vidéo pour iPhone et iPad

## Recette — Concaténer plusieurs fichiers sources en un seul MP4

`VideoSource` accepte un nom de fichier plus des temps de début/fin optionnels. Pour rogner une section d'une source, passez les décalages de début et de fin ; pour utiliser le fichier complet, passez `null`. Chaque appel à `Input_AddVideoFile` ajoute à la timeline.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

public async Task JoinClipsAsync(string output)
{
    var videoEdit = new VideoEditCore();

    // Conteneur de sortie.
    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    // Premier clip — les 10 premières secondes.
    var clip1 = new VideoSource(
        @"intro.mp4",
        TimeSpan.Zero,
        TimeSpan.FromSeconds(10),
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip1);

    // Deuxième clip — fichier complet ajouté à la timeline.
    var clip2 = new VideoSource(
        @"content.mp4",
        null,
        null,
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip2);

    // Troisième clip — fichier complet ajouté.
    var clip3 = new VideoSource(
        @"outro.mp4",
        null,
        null,
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip3);

    // Lancer le moteur.
    await videoEdit.StartAsync();
}
```

## Recette — Ajouter un logo image en superposition

`VideoEffectImageLogo` est l'effet de superposition d'image historique. Créez-le avec un nom d'effet unique (le deuxième argument du constructeur), affectez le fichier image via `Filename`, et ajoutez-le au moteur. La position est contrôlée par `Left`/`Top` (en pixels) lorsqu'aucun alignement automatique n'est utilisé.

```csharp
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEffects;

public async Task AddLogoAsync(string source, string output)
{
    var videoEdit = new VideoEditCore();

    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    var video = new VideoSource(source, null, null, VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(video);

    // Superposition de logo image (PNG avec alpha recommandé pour les logos transparents).
    var logo = new VideoEffectImageLogo(enabled: true, name: "logo1")
    {
        Filename = "logo.png",
        Left = 10,
        Top = 10
    };
    videoEdit.Video_Effects_Add(logo);

    await videoEdit.StartAsync();
}
```

## Recette — Remplacer l'audio source par une piste musicale

Ajoutez le fichier vidéo en tant que source vidéo uniquement (via les surcharges de `Input_AddVideoFile` conscientes de `targetStreamIndex` si nécessaire), puis ajoutez un `AudioSource` séparé pour la piste musicale. Le constructeur `AudioSource` accepte un nom de fichier plus des décalages de début/fin optionnels.

```csharp
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

public async Task ReplaceAudioAsync(string source, string music, string output)
{
    var videoEdit = new VideoEditCore();

    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    // Vidéo source (l'audio original est ignoré par le moteur quand un AudioSource
    // séparé est ajouté ; consultez les surcharges de Input_AddAudioFile pour contrôler le mixage).
    var video = new VideoSource(source, null, null, VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(video);

    // Musique de fond — fichier complet.
    var music_src = new AudioSource(music);
    await videoEdit.Input_AddAudioFileAsync(music_src);

    await videoEdit.StartAsync();
}
```

## Ressources supplémentaires

Trouvez davantage d'exemples de code et de ressources sur notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples), où nous mettons régulièrement à jour notre collection avec de nouveaux exemples et techniques d'implémentation pour les développeurs .NET.
