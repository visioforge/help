---
title: Lecteur vidéo Android avec streaming HLS et RTSP en C# .NET
description: Créez des apps de lecteur vidéo Android avec streaming, accélération matérielle et multi-format avec VisioForge Media Player SDK .NET.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Playback
  - Streaming
  - RTSP
  - HLS
  - MP4
  - C#
primary_api_classes:
  - MediaPlayerCoreX

---

# Media Player SDK Android — Solution professionnelle de lecture vidéo

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Le VisioForge Android Player SDK permet aux développeurs d'intégrer la lecture vidéo professionnelle, le streaming et l'édition dans des applications Android natives. Construit sur GStreamer, il fournit une API complète pour des applications riches en fonctionnalités.

Le SDK prend en charge de nombreux formats vidéo, codecs et protocoles de streaming.

## Fonctionnalités clés

### Lecture vidéo et streaming

Notre SDK de lecteur Android offre une lecture puissante avec accélération matérielle, garantissant des performances optimales pour le contenu haute résolution. Les développeurs intègrent le lecteur en utilisant une API intuitive avec prise en charge de MP4, MKV, AVI, WebM et d'autres formats.

Le lecteur fournit un contrôle précis avec lire, pause, positionnement et navigation. Des vitesses de lecture variables et une navigation image par image donnent un contrôle complet de l'expérience de visionnage.

Diffusez du contenu depuis diverses sources, notamment HTTP Live Streaming (HLS), RTSP et RTMP. Le streaming à débit adaptatif ajuste la qualité en fonction de la bande passante pour les utilisateurs mobiles.

### Édition vidéo et effets

Le SDK inclut des capacités d'édition vidéo pour créer des applications d'éditeur. Appliquez des effets en temps réel, y compris des ajustements de luminosité, contraste et saturation.

Superposez du texte, des images et des graphiques SVG avec un contrôle sur le positionnement et la transparence pour l'image dans l'image, les filigranes et les éléments interactifs.

### Prise en charge Android native et multiplateforme

Le SDK s'intègre parfaitement à Android Studio, prenant en charge le développement Java et Kotlin. Le composant VideoView s'intègre dans toute disposition Android.

Le SDK prend également en charge .NET MAUI et Avalonia pour le développement multiplateforme, permettant le partage de code sur Android, iOS, Windows, macOS et Linux.

## Capacités techniques

### Prise en charge des codecs et formats

Le SDK prend en charge de nombreux codecs vidéo avec un décodage accéléré matériellement pour H.264, H.265/HEVC, VP8 et VP9. La lecture audio prend en charge AAC, MP3, Opus et Vorbis.

### API et performance

Notre référence API fournit une documentation détaillée. Les exemples de code démontrent les cas d'usage courants. Les événements et rappels fournissent des notifications en temps réel.

Le SDK est optimisé pour le mobile avec une attention à la batterie et à la mémoire. L'accélération matérielle garantit une lecture fluide.

## Prise en main

### Installation et configuration

Intégrez le VisioForge Android Player SDK en utilisant NuGet. Ajoutez la référence du paquet à votre projet. Pour .NET MAUI, configurez l'utilisation du contrôle VideoView.

Les instructions de configuration se trouvent dans notre documentation.

### Exemple de code de démarrage rapide

Voici comment créer un lecteur multimédia de base :

#### Ajouter VideoView à la disposition

```xml
<VisioForge.Core.UI.Android.VideoViewTX
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:id="@+id/videoView" />
```

#### Initialiser le lecteur

```csharp
using VisioForge.Core.MediaPlayerX;

public class MainActivity : Activity
{
    private MediaPlayerCoreX _player;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        var videoView = FindViewById<VisioForge.Core.UI.Android.VideoViewTX>(Resource.Id.videoView);
        _player = new MediaPlayerCoreX(videoView);
    }

    protected override void OnDestroy()
    {
        VisioForgeX.DestroySDK();
        base.OnDestroy();
    }
}
```

#### Contrôles de lecture

```csharp
private async void PlayVideo()
{
    await _player.OpenAsync(new Uri("https://example.com/video.mp4"));
    await _player.PlayAsync();
}

private async void PauseVideo() => await _player.PauseAsync();
private async void ResumeVideo() => await _player.ResumeAsync();
private async void StopVideo() => await _player.StopAsync();
```

### Applications d'exemple

Les exemples GitHub démontrent les capacités du SDK : [exemple Media Player](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Android/MediaPlayer) avec lecture, streaming et édition.

### Alternative : Media Blocks SDK

Le [Media Blocks SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Android/MediaPlayer) fournit une API de plus bas niveau pour des pipelines personnalisés.

## Cas d'usage

Le Android Player SDK est idéal pour :

- **Applications de streaming vidéo** : prise en charge du streaming adaptatif
- **Plateformes éducatives** : leçons vidéo et e-learning
- **Lecteurs multimédias** : applications de lecteur multimédia natif avec prise en charge des sous-titres
- **Réseaux sociaux** : lecture de contenu généré par les utilisateurs
- **Éditeurs vidéo** : édition mobile avec aperçu en temps réel
- **Sécurité** : applications de surveillance avec streaming en direct

Le SDK prend en charge Android TV et le mode image dans l'image sur Android 8.0+.

## Licence

Le Android Player SDK est disponible sous licence commerciale. Une seule licence couvre toutes les plateformes prises en charge. Des versions d'essai sont disponibles.

## Conclusion

Le VisioForge Android Player SDK fournit une lecture vidéo professionnelle pour les applications Android. Avec le streaming, l'édition et les fonctionnalités avancées, les développeurs peuvent créer rapidement de puissantes applications multimédias.

Pour plus d'informations, visitez notre [page produit](https://www.visioforge.com/media-player-sdk-net) ou la [documentation API](https://api.visioforge.org/dotnet/api/index.html).

## Ressources associées

- [Guide d'implémentation Android](../../deployment-x/Android.md) — instructions détaillées de déploiement pour Android
- [Exemples de code](../code-samples/index.md) — exemples et extraits fonctionnels
- [Guide du lecteur multiplateforme Avalonia](../guides/avalonia-player.md) — construction d'apps vidéo multiplateformes
- [Journal des modifications](../../changelog.md) — dernières mises à jour et versions
- [Contrat de licence utilisateur final](../../../eula.md) — informations de licence
