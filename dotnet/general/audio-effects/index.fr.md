---
title: Effets audio en C# .NET — EQ, réverbération, filtres
description: Effets audio en temps réel en C# .NET avec VisioForge — égaliseur, réverbération, écho, réduction de bruit, pitch shift et 30+ effets.
sidebar_label: Effets audio
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - Media Player SDK
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

video:
  id: VsFSrYD80XE
  upload_date: 2026-11-20T15:10:00Z
  duration: PT11M57S
---

# Effets audio en temps réel pour applications .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le VisioForge Media Framework fournit plus de 30 effets audio pour le traitement audio en temps réel dans les applications C# et .NET. Construits sur GStreamer, les effets multiplateformes incluent des égaliseurs, de la réverbération, de l'écho, de la compression dynamique, des filtres, du pitch shifting, une réduction de bruit basée sur l'IA, et plus encore — tout fonctionnant sur Windows, macOS, Linux, iOS et Android.

## Tutoriel vidéo

La vidéo crée le même lecteur dans Visual Studio : une fenêtre WPF avec un égaliseur à dix bandes, un écho, une réverbération et la hauteur tonale, chacun modifié pendant que le fichier se lit.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/VsFSrYD80XE?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

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

- **PitchAudioEffect** — Décalage de hauteur en demi-tons sans changement de tempo
- **ScaleTempoAudioEffect** — Garde la hauteur quand la vitesse de lecture change

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
- **DS Reverb (I3DL2)** — Réverbération professionnelle avec modélisation environnementale (peut manquer sous Windows 11 ; l'alternative est DS Waves Reverb)
- **DS Waves Reverb** — Réverbération simplifiée

## Éléments GStreamer

Tous les effets audio multiplateformes s'exécutent dans un pipeline GStreamer. `ChannelOrder`, `DownMix`, `DynamicAmplify`, `Fade`, `Flanger`, `PhaseInvert`, `Phaser`, `Sound3D`, `TrebleEnhancer` et `TrueBass` traitent les échantillons en code managé. Les autres encapsulent un ou plusieurs éléments GStreamer :

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

## Transcription de la vidéo

### Ce que nous construisons

Ajoutons des effets audio à un lecteur C# WPF avec le VisioForge Media Player SDK, et modifions-les pendant que le fichier se lit. Voici ce que nous allons construire. Un lecteur WPF pour les fichiers vidéo et audio, avec quatre effets : un égaliseur à dix bandes, un écho, une réverbération et la hauteur tonale. Une case à cocher active ou désactive chaque effet, et ses curseurs le modifient, le tout pendant que le fichier continue de se lire. Le code derrière fait environ deux cent quarante lignes de C#. Voici le lecteur terminé. Le panneau d'effets se trouve sous la vidéo.

### Prérequis

Il vous faut Windows 10 ou 11, et Visual Studio 2022 ou 2026 avec la charge de travail développement desktop .NET. Le projet cible .NET 8 ou une version ultérieure. Et un fichier de musique ou de vidéo à lire. La partie lecteur de l'application est celle de notre tutoriel sur le lecteur vidéo, donc cette vidéo la parcourt rapidement.

### Créer un projet WPF

Dans Visual Studio, créez un nouveau projet. Recherchez WPF et choisissez Application WPF pour C#. Nommez le projet AudioEffectsWpf. Choisissez .NET 10 et cliquez sur Créer.

### Ajouter les paquets NuGet

Ajoutez maintenant quatre paquets NuGet. Le Media Player SDK lui-même. CrossPlatform Core, l'environnement d'exécution GStreamer sur lequel tourne le moteur X. CrossPlatform Core Adds, qui apporte d'autres greffons, dont la réverbération. Et CrossPlatform Libav, les codecs FFmpeg par lesquels se décodent beaucoup de formats. Ou utilisez le gestionnaire de paquets NuGet : clic droit sur le projet, Gérer les packages NuGet, et installez VisioForge.DotNet.MediaPlayer depuis l'onglet Parcourir. Installez ensuite les paquets Core, Core Adds et Libav pour Windows x64. L'onglet Installés les liste tous les quatre. Ce sont des paquets x64, donc mettez PlatformTarget à x64 dans le fichier de projet.

### La fenêtre du lecteur

La fenêtre est écrite en XAML, et le concepteur la dessine aussitôt. D'abord la partie lecteur. Associez l'espace de noms VisioForge.Core.UI.WPF à un préfixe, ici vf. La grille a deux colonnes, la vidéo et une colonne de réglages de 260 pixels, et quatre lignes : la vidéo, la chronologie, les boutons et les effets. Le VideoView remplit la première cellule. Sous la vidéo vient la chronologie : un curseur pour la position, avec le temps à côté. Puis Lecture, Pause, Reprendre et Arrêter. Ils sont tous désactivés au départ : Lecture jusqu'au chargement du SDK, et les trois autres jusqu'à la lecture d'un fichier. La colonne de réglages contient le nom du fichier avec un bouton de parcours, et une liste déroulante pour la sortie audio. Puis un curseur de volume, et une zone de texte en lecture seule pour le journal des erreurs.

### Le panneau d'effets

Maintenant le panneau d'effets, sur la dernière ligne, sous les boutons. Un StackPanel horizontal occupe les deux colonnes. Chaque effet a un groupe avec une case à cocher dans son en-tête. L'égaliseur vient en premier, et sa case affecte un même gestionnaire à Checked et à Unchecked. Une grille uniforme de deux lignes et dix colonnes contient les bandes : dix curseurs, et leurs fréquences en dessous, de 30 hertz à 15 kilohertz. Le Tag de chaque curseur est l'index de sa bande, ainsi les dix partagent un même gestionnaire. Un style configure les dix curseurs : verticaux, de moins 12 à plus 12 décibels, et reliés au même gestionnaire ValueChanged. Un second style centre les libellés. L'écho a un curseur de délai, de 50 à 1000 millisecondes, et un curseur d'intensité de 0 à 1. La réverbération a une taille de salle, et un niveau qui mélange le son réverbéré avec le son sec. Les deux vont de 0 à 1. Et la hauteur tonale a un curseur, de 12 demi-tons en bas à 12 en haut, qui s'aligne sur des demi-tons entiers.

### Initialiser le SDK

MainWindow.xaml.cs commence par ces using. VisioForge.Core.Types.X.AudioEffects contient les classes des effets. Chaque effet est un objet, créé une fois et réglé sur les valeurs initiales des curseurs. Equalizer10AudioEffect prend les dix niveaux de bande en décibels, tous à zéro ici. EchoAudioEffect prend le délai, puis son intensité et sa rétroaction. ReverberationAudioEffect reçoit une grande salle, et PitchAudioEffect est quatre demi-tons plus haut. Au chargement de la fenêtre, attendez une fois VisioForgeX.InitSDKAsync. Créez ensuite le moteur MediaPlayerCoreX et passez-lui le VideoView. Les erreurs arrivent dans l'événement OnError sur un autre thread, donc le gestionnaire revient sur le thread de l'interface et les ajoute au journal. Audio_OutputDevicesAsync liste les haut-parleurs et les casques, et leurs noms remplissent la liste déroulante. Lecture est activé une fois le SDK chargé. Le bouton de parcours affiche un OpenFileDialog pour les fichiers vidéo et audio.

### Lire avec les effets

Le gestionnaire de Lecture désactive les boutons et vide le journal. Il envoie le son vers la sortie choisie, au volume du curseur. OpenAsync ouvre le fichier. Puis Audio_Effects_AddOrUpdate ajoute chaque effet dont la case est cochée, et PlayAsync démarre la lecture avec eux. Les effets durent une lecture : après Arrêter ou à la fin du fichier ils disparaissent, donc chaque Lecture les ajoute à nouveau. PlaybackState.Free signifie que le fichier ne s'est pas ouvert ou n'a pas démarré, et la fenêtre revient à l'état arrêté. Sinon les boutons passent en lecture, et le minuteur de la chronologie démarre.

### Changer les effets pendant la lecture

ApplyEffect sert aux quatre cases. Pendant la lecture d'un fichier, cocher une case ajoute son effet avec Audio_Effects_AddOrUpdate, et la décocher appelle Audio_Effects_Remove. Quand rien ne se lit, il ne fait rien : Lecture ajoute les effets cochés. La case de l'égaliseur appelle ApplyEffect. Les curseurs de bande partagent un gestionnaire : il lit la bande dans le Tag du curseur, et SetLevel fixe le niveau de cette bande en décibels. L'égaliseur en cours de lecture le prend aussitôt. Les curseurs de l'écho fixent Delay et Intensity sur l'objet de l'effet. Une propriété affectée à un effet qui joue s'applique aussitôt, donc il n'est pas nécessaire de l'ajouter à nouveau. La réverbération fonctionne de la même façon, avec RoomSize et Level. Et le curseur de hauteur fixe PitchSemitones. Le tempo ne change pas : seule la hauteur bouge.

### Le reste du lecteur

Le reste est le lecteur du tutoriel sur le lecteur vidéo. Pause et Reprendre sont un appel chacun, et Arrêter attend StopAsync et réinitialise la fenêtre. Les effets continuent après une pause. ShowBusy désactive les quatre boutons, et ShowPlaying active soit Lecture, soit les trois autres. Stopped remet les boutons, l'étiquette de temps et la chronologie en place. OnStop se déclenche après StopAsync et à la fin du fichier. Le gestionnaire n'agit que pour la fin du fichier, quand Arrêter est actif et que State vaut Free. Un DispatcherTimer se déclenche deux fois par seconde et déplace la chronologie vers la position, avec un indicateur levé, pour que ce déplacement ne cherche pas. Un clic sur la chronologie cherche aussitôt, et un glissement cherche une fois, au relâchement du curseur. Le volume suit son curseur. À la fermeture de la fenêtre, Dispose arrête la lecture et libère le lecteur, et DestroySDK libère le SDK.

### Lancer l'application

Lancez l'application. Choisissez un fichier, cliquez sur Lecture, et cliquez sur la chronologie pour avancer. D'abord, écoutez-le sans effets. Cochez Égaliseur, puis baissez les bandes graves et montez les aiguës. Coupez l'égaliseur et activez Écho. Allongez ensuite le délai. Écho coupé, Réverbération activée : une grande salle. Montez ensuite le niveau pour entendre plus de salle. Réverbération coupée, Hauteur activée : quatre demi-tons plus haut. Faites-la ensuite descendre. Décochez Hauteur, et le son redevient comme avant. Puis Arrêter, et fermez la fenêtre.

### Erreurs fréquentes

Quelques erreurs courantes. Oublier le paquet Core Adds, si bien que la réverbération n'est pas créée. Ajouter les effets une seule fois : Arrêter ou la fin du fichier les retire, donc ajoutez-les à nouveau à chaque Lecture. Écrire un élément du tableau Levels de l'égaliseur pendant la lecture, ce qui n'atteint pas l'effet en cours : utilisez SetLevel. PitchShiftAudioEffect est obsolète, donc utilisez PitchAudioEffect. Et mélanger des paquets x86 et x64.

### Étapes suivantes

Le guide écrit énumère d'autres effets, comme un compresseur, un flanger et la suppression du bruit. Les mêmes effets fonctionnent pendant la capture vidéo, et le Media Blocks SDK a un bloc par effet. Et le capteur d'échantillons audio vous livre les échantillons dans votre propre code. Le code source complet est sur GitHub, et la documentation sur visioforge.com/help. Merci d'avoir regardé.

## Voir aussi

- [Référence des effets audio](reference.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Encodeurs audio](../audio-encoders/index.md)
- [Blocs de traitement audio (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
