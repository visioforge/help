---
title: Effets audio en C# .NET — EQ, réverbération, filtres
description: Effets audio en temps réel en C# .NET avec VisioForge — égaliseur, réverbération, écho, réduction de bruit, pitch shift et 30+ effets.
sidebar_label: Effets audio
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Streaming
  - Editing
primary_api_classes:
  - VolumeAudioEffect
  - Equalizer10AudioEffect
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - CompressorExpanderAudioEffect

---

# Effets audio en temps réel pour applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le VisioForge Media Framework fournit plus de 30 effets audio pour le traitement audio en temps réel dans les applications C# et .NET. Construits sur GStreamer, les effets multiplateformes incluent des égaliseurs, de la réverbération, de l'écho, de la compression dynamique, des filtres, du pitch shifting, une réduction de bruit basée sur l'IA, et plus encore — tout fonctionnant sur Windows, macOS, Linux, iOS et Android.

## SDK et plateformes

### Effets multiplateformes

- **SDK** : Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **Plateformes** : Windows, macOS, Linux, iOS, Android
- **Espace de noms** : `VisioForge.Core.Types.X.AudioEffects`

### Effets DSP classiques

- **SDK** : Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore)
- **Plateformes** : Windows uniquement
- **Espace de noms** : `VisioForge.Core.DSP`

### Effets DirectSound

- **SDK** : Video Capture SDK, Media Player SDK, Video Edit SDK (moteurs classiques)
- **Plateformes** : Windows uniquement
- **Espace de noms** : `VisioForge.Core.Types.X._Windows.AudioEffects`

Pour les paramètres et propriétés détaillés de chaque effet, consultez la [référence des effets audio](reference.md).

## Catégories d'effets

### Volume et dynamique

- **VolumeAudioEffect** — Contrôle de volume basique avec fonctionnalité de mise en sourdine
- **AmplifyAudioEffect** — Amplification du signal audio avec contrôle d'écrêtage
- **CompressorExpanderAudioEffect** — Compression et expansion de la plage dynamique
- **DynamicAmplifyAudioEffect** — Contrôle de gain adaptatif avec temps d'attaque/relâchement

### Égalisation

- **Equalizer10AudioEffect** — Égaliseur graphique 10 bandes avec fréquences fixes
- **EqualizerParametricAudioEffect** — Égaliseur paramétrique avec bandes configurables
- **TrebleEnhancerAudioEffect** — Renforcement des hautes fréquences
- **TrueBassAudioEffect** — Renforcement des basses fréquences

### Filtres

- **HighPassAudioEffect** — Filtre passe-haut pour supprimer les basses fréquences
- **LowPassAudioEffect** — Filtre passe-bas pour supprimer les hautes fréquences
- **BandPassAudioEffect** — Filtre passe-bande pour des plages de fréquences spécifiques
- **NotchAudioEffect** — Filtre notch pour supprimer des fréquences spécifiques
- **ChebyshevLimitAudioEffect** — Filtres passe-bas/passe-haut de Chebyshev avec coupures nettes
- **ChebyshevBandPassRejectAudioEffect** — Filtres passe-bande/réjecteur de Chebyshev

### Spatial et stéréo

- **BalanceAudioEffect** — Contrôle de balance stéréo (panoramique gauche/droite)
- **WideStereoAudioEffect** — Amélioration de la largeur stéréo
- **Sound3DAudioEffect** — Effets audio spatiaux 3D
- **HRTFRenderAudioEffect** — Audio spatial à fonction de transfert relative à la tête (HRTF)
- **PhaseInvertAudioEffect** — Inversion de phase pour correction de polarité

### Effets temporels

- **EchoAudioEffect** — Effets d'écho et de delay
- **RSAudioEchoAudioEffect** — Écho amélioré avec contrôles avancés
- **ReverberationAudioEffect** — Réverbération de pièce (algorithme Freeverb)
- **FadeAudioEffect** — Automation de fondu volumique entrée/sortie

### Effets de modulation

- **PhaserAudioEffect** — Effet phaser avec modulation LFO
- **FlangerAudioEffect** — Effet flanging avec modulation de delay

### Hauteur et tempo

- **PitchShiftAudioEffect** — Décalage de hauteur sans changement de tempo
- **ScaleTempoAudioEffect** — Changement de tempo sans décalage de hauteur

### Effets spéciaux

- **KaraokeAudioEffect** — Suppression de voix pour karaoké
- **RemoveSilenceAudioEffect** — Détection et suppression automatiques de silence
- **CsoundAudioEffect** — Traitement audio avancé basé sur Csound

### Réduction de bruit et mesure

- **AudioRNNoiseAudioEffect** — Réduction de bruit basée sur l'IA avec RNN
- **AudioLoudNormAudioEffect** — Normalisation de la sonie EBU R128
- **EbuR128LevelAudioEffect** — Mesure de sonie EBU R128

### Gestion des canaux

- **ChannelOrderAudioEffect** — Remappage et routage des canaux
- **DownMixAudioEffect** — Downmixage multicanal vers stéréo/mono

### Effets DirectSound (Windows uniquement, SDK classiques)

- **DS Chorus** — Plusieurs copies retardées et modulées
- **DS Distortion** — Distorsion/overdrive audio
- **DS Gargle** — Modulation gargouillis/trémolo
- **DS Reverb (I3DL2)** — Réverbération professionnelle avec modélisation environnementale
- **DS Waves Reverb** — Réverbération simplifiée

## Éléments GStreamer

Tous les effets audio multiplateformes sont construits au-dessus du framework multimédia GStreamer. Chaque effet encapsule un ou plusieurs éléments GStreamer :

| Catégorie | Éléments GStreamer |
|----------|-------------------|
| Volume/Dynamique | volume, audioamplify, audiodynamic |
| Égalisation | equalizer-10bands, equalizer-nbands |
| Filtres | audiocheblimit, audiochebband, audioiirfilter |
| Spatial | audiopanorama, stereo, hrtfrender |
| Temporel | audioecho, rsaudioecho, freeverb |
| Modulation | Implémentations phaser/flanger personnalisées |
| Hauteur/Tempo | scaletempo, pitch (SoundTouch) |
| Spécial | audiokaraoke, csoundfilter, removesilence |
| Réduction de bruit | audiornnoise, audioloudnorm, ebur128level |
| Canaux | audioconvert, routage personnalisé |

## Modèles d'utilisation courants

### Ajout d'effets (SDK multiplateformes)

```csharp
// Créer un effet audio
var volumeEffect = new VolumeAudioEffect(1.5); // Volume à 150 %

// VideoCaptureCoreX / MediaPlayerCoreX
core.Audio_Effects_AddOrUpdate(volumeEffect);
```

### Combinaison de plusieurs effets

Les effets sont traités dans l'ordre dans lequel ils sont ajoutés :

```csharp
// Créer une chaîne de traitement
core.Audio_Effects_AddOrUpdate(new HighPassAudioEffect(80));           // Supprimer les grondements
core.Audio_Effects_AddOrUpdate(new CompressorExpanderAudioEffect());   // Compresser la dynamique
core.Audio_Effects_AddOrUpdate(new Equalizer10AudioEffect(levels));    // Ajustements EQ
core.Audio_Effects_AddOrUpdate(new ReverberationAudioEffect());        // Ajouter de la réverbération
```

### Mises à jour de paramètres en temps réel

La plupart des effets prennent en charge des changements de paramètres en temps réel :

```csharp
var volumeEffect = new VolumeAudioEffect(1.0);
core.Audio_Effects_AddOrUpdate(volumeEffect);

// Plus tard, pendant la lecture :
volumeEffect.Level = 0.5; // Réduire le volume à 50 %
volumeEffect.Mute = true; // Mettre l'audio en sourdine
core.Audio_Effects_AddOrUpdate(volumeEffect); // Appliquer les modifications
```

### Utilisation du Media Blocks SDK

Pour l'utilisation orientée pipeline du Media Blocks SDK avec des blocs d'effets audio dédiés, consultez [Blocs de traitement audio et d'effets](../../mediablocks/AudioProcessing/index.md).

## Considérations de performance

- **Utilisation CPU** : les effets complexes comme la réverbération, Csound et plusieurs égaliseurs peuvent être gourmands en CPU
- **Ordre des effets** : placez les effets coûteux en calcul après les plus simples pour réduire la charge de traitement
- **Traitement en temps réel** : tous les effets sont conçus pour le streaming audio en temps réel

## Foire aux questions

???+ "Quels effets audio sont disponibles dans les SDK VisioForge .NET ?"
    VisioForge fournit plus de 30 effets audio, notamment le contrôle de volume, des égaliseurs 10 bandes et paramétriques, la réverbération, l'écho, le compresseur/expandeur, des filtres passe-haut/passe-bas/passe-bande, le pitch shift, la réduction de bruit (basée RNN), la suppression de voix karaoké, l'audio spatial 3D, et plus encore. Tous les effets multiplateformes fonctionnent sur Windows, macOS, Linux, iOS et Android.

???+ "Comment ajouter des effets audio à mon application C# ?"
    Créez une instance d'effet et ajoutez-la à l'aide de `Audio_Effects_AddOrUpdate()`. Par exemple :

    ```csharp
    // EQ 10 bandes : toutes les bandes à 0 dB (neutre)
    var eq = new Equalizer10AudioEffect(new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
    core.Audio_Effects_AddOrUpdate(eq);
    ```

    Les effets peuvent être ajoutés, mis à jour et supprimés pendant la lecture. Pour le Media Blocks SDK, utilisez le `AudioEffectsBlock`.

???+ "Puis-je chaîner plusieurs effets audio ensemble ?"
    Oui. Les effets sont traités dans l'ordre dans lequel ils sont ajoutés. Vous pouvez créer des chaînes de traitement complexes combinant filtres, EQ, compression, réverbération et autres effets. Chaque effet traite la sortie audio du précédent.

???+ "Les effets audio fonctionnent-ils en temps réel pendant la lecture ?"
    Oui. Tous les effets audio VisioForge prennent en charge les changements de paramètres en temps réel. Vous pouvez ajuster le volume, les bandes EQ, les niveaux de réverbération et d'autres paramètres pendant la lecture audio sans interrompre le flux.

???+ "Quelle est la différence entre les effets multiplateformes et DirectSound ?"
    Les effets multiplateformes (espace de noms `VisioForge.Core.Types.X.AudioEffects`) fonctionnent sur toutes les plateformes en utilisant GStreamer. Les effets DirectSound sont exclusifs à Windows et disponibles dans les moteurs classiques du SDK. Les effets multiplateformes couvrent les mêmes fonctionnalités et plus.

## Voir aussi

- [Référence des effets audio](reference.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Encodeurs audio](../audio-encoders/index.md)
- [Blocs de traitement audio (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
