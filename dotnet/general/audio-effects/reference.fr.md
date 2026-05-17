---
title: Référence API des effets audio pour le SDK .NET VisioForge
description: API pour 30+ effets audio dans VisioForge .NET — volume, EQ, compresseur, réverbération, écho, filtres, pitch shift avec exemples C#.
sidebar_label: Référence des effets audio
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Streaming
  - Editing
  - Effects
  - C#
primary_api_classes:
  - VolumeAudioEffect
  - Equalizer10AudioEffect
  - BandPassAudioEffect
  - BalanceAudioEffect
  - WideStereoAudioEffect

---

# Référence API des effets audio

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Référence complète des paramètres pour tous les effets audio disponibles dans les SDK VisioForge .NET. Chaque effet multiplateforme encapsule un élément GStreamer et prend en charge les changements de paramètres en temps réel depuis le code C# pendant la lecture. Les effets multiplateformes fonctionnent sur Windows, macOS, Linux, iOS et Android.

Pour un aperçu des catégories d'effets et des modèles d'utilisation, consultez [Effets audio](index.md).

## Effets de volume et de dynamique

### VolumeAudioEffect

**Élément GStreamer** : `volume`

**Objectif** : contrôler le niveau de volume audio avec mise en sourdine optionnelle.

**Paramètres** :

- `Level` (double) : multiplicateur de volume
    - Plage : 0.0 à illimité
    - Par défaut : 1.0 (100 %)
    - Exemples : 0.5 = 50 %, 2.0 = 200 %
- `Mute` (bool) : mettre la sortie audio en sourdine
    - Par défaut : false

**Utilisation** :

```csharp
var effect = new VolumeAudioEffect(1.5); // Volume à 150 %
effect.Mute = true; // Mettre temporairement en sourdine
```

---

### AmplifyAudioEffect

**Élément GStreamer** : `audioamplify`

**Objectif** : amplifier l'audio avec contrôle d'écrêtage.

**Paramètres** :

- `Level` (double) : niveau d'amplification
    - Plage : 1.0 à 10.0
    - Par défaut : 1.0
- `ClippingMethod` (AmplifyClippingMethod) : gestion des pics
    - Options : Normal, WrapNegative, WrapPositive, NoClip
    - Par défaut : Normal

**Utilisation** :

```csharp
var effect = new AmplifyAudioEffect(2.0);
effect.ClippingMethod = AmplifyClippingMethod.NoClip;
```

---

### CompressorExpanderAudioEffect

**Élément GStreamer** : `audiodynamic`

**Objectif** : compression ou expansion de la plage dynamique.

**Paramètres** :

- `Threshold` (double) : seuil d'activation
    - Plage : 0.0 à 1.0
    - Par défaut : 0.0
- `Ratio` (double) : ratio de compression/expansion
    - Plage : 1.0+
    - Par défaut : 1.0
    - Typique : 2:1 à 10:1 pour la compression
- `Mode` (AudioCompressorMode) : Compressor ou Expander
    - Par défaut : Compressor
- `Characteristics` (AudioDynamicCharacteristics) : HardKnee ou SoftKnee
    - Par défaut : SoftKnee

**Utilisation** :

```csharp
var effect = new CompressorExpanderAudioEffect();
effect.Threshold = 0.5;
effect.Ratio = 4.0; // Compression 4:1
effect.Characteristics = AudioDynamicCharacteristics.SoftKnee;
```

---

### DynamicAmplifyAudioEffect

**Objectif** : contrôle de gain adaptatif.

**Paramètres** :

- `AttackTime` (uint) : temps de réponse en ms
    - Typique : 10-100 ms
- `MaxAmplification` (uint) : gain maximum
    - 10000 = 1x (pas de changement)
    - 20000 = 2x amplification
    - Par défaut : 10000
- `ReleaseTime` (TimeSpan) : durée avant la reprise de l'amplification
    - Typique : 100-1000 ms

**Utilisation** :

```csharp
var effect = new DynamicAmplifyAudioEffect(50, 20000, TimeSpan.FromMilliseconds(500));
```

---

## Effets d'égalisation

### Equalizer10AudioEffect

**Élément GStreamer** : `equalizer-10bands`

**Objectif** : égaliseur graphique 10 bandes avec fréquences fixes.

**Bandes de fréquences** :

1. 29 Hz (sub-bass)
2. 59 Hz (basses)
3. 119 Hz (basses)
4. 237 Hz (bas-médium)
5. 474 Hz (médium)
6. 947 Hz (médium)
7. 1889 Hz (haut-médium)
8. 3770 Hz (présence)
9. 7523 Hz (brillance)
10. 15011 Hz (air)

**Paramètres** :

- `Levels` (double[]) : gain de chaque bande en dB
    - Plage par bande : -24 à +12 dB
    - Le tableau doit contenir exactement 10 valeurs

**Utilisation** :

```csharp
var levels = new double[] {
    3.0,   // 29 Hz : +3 dB
    2.0,   // 59 Hz : +2 dB
    0.0,   // 119 Hz : 0 dB (pas de changement)
    -2.0,  // 237 Hz : -2 dB
    0.0,   // 474 Hz
    0.0,   // 947 Hz
    1.0,   // 1889 Hz : +1 dB
    2.0,   // 3770 Hz : +2 dB
    3.0,   // 7523 Hz : +3 dB
    4.0    // 15011 Hz : +4 dB
};
var effect = new Equalizer10AudioEffect(levels);
```

---

### EqualizerParametricAudioEffect

**Élément GStreamer** : `equalizer-nbands`

**Objectif** : égaliseur paramétrique avec bandes configurables.

**Paramètres** :

- `Bands` (ParametricEqualizerBand[]) : tableau de bandes
    - Nombre : 1 à 64 bandes
    - Chaque bande : Frequency, Gain, Width (bande passante en Hz)

**Utilisation** :

```csharp
var effect = new EqualizerParametricAudioEffect(3);
effect.Bands[0].Frequency = 100;  // Hz
effect.Bands[0].Gain = -6;        // dB
effect.Bands[0].Width = 1.0f;     // bande passante
// Configurer les autres bandes...
effect.Update(); // Appliquer les modifications
```

---

### TrebleEnhancerAudioEffect

**Objectif** : renforcer les hautes fréquences.

**Paramètres** :

- `Frequency` (int) : fréquence de départ en Hz
    - Typique : 4000-8000 Hz
    - Les fréquences au-dessus sont renforcées
- `Volume` (ushort) : quantité de renforcement
    - Plage : 0 à 10000
    - 0 = aucun effet

**Utilisation** :

```csharp
var effect = new TrebleEnhancerAudioEffect(6000, 5000);
```

---

### TrueBassAudioEffect

**Objectif** : renforcer les basses fréquences.

**Paramètres** :

- `Frequency` (int) : limite supérieure de fréquence en Hz
    - Typique : 100-300 Hz
    - Les fréquences en dessous sont renforcées
- `Volume` (ushort) : quantité de renforcement
    - Plage : 0 à 10000
    - 0 = aucun effet

**Utilisation** :

```csharp
var effect = new TrueBassAudioEffect(150, 5000);
```

---

## Effets de filtrage

### HighPassAudioEffect

**Implémentation** : DSP personnalisé (filtre passe-haut IIR)

**Objectif** : supprimer les basses fréquences.

**Paramètres** :

- `CutOff` (uint) : fréquence de coupure en Hz
    - Les fréquences en dessous sont atténuées
    - Typique : 80-200 Hz pour la voix, 40 Hz pour la musique

**Fréquences courantes** :

- 20-40 Hz : suppression sub-sonique
- 60-80 Hz : suppression des grondements
- 100-150 Hz : amélioration de la clarté

**Utilisation** :

```csharp
var effect = new HighPassAudioEffect(100); // Supprimer les fréquences en dessous de 100 Hz
```

---

### LowPassAudioEffect

**Implémentation** : DSP personnalisé (filtre passe-bas IIR)

**Objectif** : supprimer les hautes fréquences.

**Paramètres** :

- `CutOff` (uint) : fréquence de coupure en Hz
    - Les fréquences au-dessus sont atténuées
    - Typique : 8000-12000 Hz pour suppression du souffle

**Fréquences courantes** :

- 15000-20000 Hz : réduction douce de l'air
- 8000-10000 Hz : chaleur
- 3000-5000 Hz : effet téléphone

**Utilisation** :

```csharp
var effect = new LowPassAudioEffect(10000); // Supprimer les fréquences au-dessus de 10 kHz
```

---

### BandPassAudioEffect

**Implémentation** : DSP personnalisé (filtre à variables d'état)

**Objectif** : autoriser uniquement une plage de fréquences spécifique.

**Paramètres** :

- `CutOffHigh` (float) : limite supérieure de fréquence en Hz
- `CutOffLow` (float) : limite inférieure de fréquence en Hz

**Utilisation** :

```csharp
// Constructeur : BandPassAudioEffect(cutOffHigh, cutOffLow)
var effect = new BandPassAudioEffect(5000, 300); // Autoriser 300-5000 Hz
```

---

### NotchAudioEffect

**Implémentation** : DSP personnalisé (filtre réjecteur de bande)

**Objectif** : supprimer une fréquence spécifique.

**Paramètres** :

- `CutOff` (uint) : fréquence centrale à supprimer en Hz
    - Typique : 50/60 Hz pour suppression du ronflement

**Utilisation** :

```csharp
var effect = new NotchAudioEffect(60); // Supprimer le ronflement 60 Hz
```

---

### ChebyshevLimitAudioEffect

**Élément GStreamer** : `audiocheblimit`

**Objectif** : filtrage passe-bas/passe-haut net avec contrôle d'ondulation.

**Paramètres** :

- `CutOffFrequency` (float) : fréquence de coupure en Hz
- `Mode` (ChebyshevLimitAudioEffectMode) : LowPass ou HighPass
- `Poles` (int) : ordre du filtre (typique 2-8)
    - Par défaut : 4
    - Plus de pôles = pente plus raide
- `Ripple` (float) : ondulation de la bande passante en dB
    - Par défaut : 0.25
- `Type` (int) : type de Chebyshev (1 ou 2)
    - Par défaut : 1

**Utilisation** :

```csharp
var effect = new ChebyshevLimitAudioEffect();
effect.CutOffFrequency = 100;
effect.Mode = ChebyshevLimitAudioEffectMode.HighPass;
effect.Poles = 6;
```

---

### ChebyshevBandPassRejectAudioEffect

**Élément GStreamer** : `audiochebband`

**Objectif** : filtrage passe-bande ou réjecteur de bande net.

**Paramètres** :

- `LowerFrequency` (float) : limite inférieure de bande en Hz
- `UpperFrequency` (float) : limite supérieure de bande en Hz
- `Mode` (ChebyshevBandPassRejectAudioEffectMode) : BandPass ou BandReject
- `Poles` (int) : ordre du filtre (typique 2-8)
    - Par défaut : 4
- `Ripple` (float) : ondulation de la bande passante en dB
    - Par défaut : 0.25
- `Type` (int) : type de Chebyshev (1 ou 2)
    - Par défaut : 1

**Utilisation** :

```csharp
var effect = new ChebyshevBandPassRejectAudioEffect();
effect.LowerFrequency = 300;
effect.UpperFrequency = 3000;
effect.Mode = ChebyshevBandPassRejectAudioEffectMode.BandPass;
```

---

## Effets spatiaux et stéréo

### BalanceAudioEffect

**Élément GStreamer** : `audiopanorama`

**Objectif** : contrôle de balance stéréo (panoramique).

**Paramètres** :

- `Level` (double) : position de la balance
    - Plage : -1.0 à 1.0
    - -1.0 = tout à gauche
    - 0.0 = centre
    - 1.0 = tout à droite
    - Par défaut : 0.0

**Utilisation** :

```csharp
var effect = new BalanceAudioEffect(-0.5); // Panoramique 50 % à gauche
```

---

### WideStereoAudioEffect

**Élément GStreamer** : `stereo`

**Objectif** : améliorer la largeur stéréo.

**Paramètres** :

- `Level` (float) : intensité d'élargissement
    - Plage : 0.0+
    - Par défaut : 0.01
    - Typique : 0.01 à 1.0
    - Valeurs plus élevées = champ stéréo plus large

**Utilisation** :

```csharp
var effect = new WideStereoAudioEffect();
effect.Level = 0.5f;
```

---

### Sound3DAudioEffect

**Objectif** : simulation d'audio spatial 3D.

**Paramètres** :

- `Value` (uint) : amplification spatiale
    - Plage : 1 à 20000
    - 1000 = neutre (désactivé)
    - < 1000 = stéréo plus étroite
    - > 1000 = stéréo plus large
    - > 10000 peut distordre

**Utilisation** :

```csharp
var effect = new Sound3DAudioEffect(2000); // Largeur spatiale x2
```

---

### PhaseInvertAudioEffect

**Objectif** : inverser la phase audio de 180 degrés.

**Paramètres** : aucun.

**Utilisation** :

```csharp
var effect = new PhaseInvertAudioEffect();
```

---

### HRTFRenderAudioEffect

**Élément GStreamer** : `hrtfrender` (rsaudiofx)

**Objectif** : audio spatial 3D basé sur HRTF.

**Paramètres** :

- `HrirFile` (string) : chemin vers le fichier de données HRIR
- `InterpolationSteps` (ulong) : qualité d'interpolation
    - Par défaut : 8
- `BlockLength` (ulong) : taille du bloc de traitement
    - Par défaut : 512
- `DistanceGain` (float) : facteur d'atténuation par la distance
    - Par défaut : 1.0

**Utilisation** :

```csharp
var effect = new HRTFRenderAudioEffect("/path/to/hrir.dat");
effect.InterpolationSteps = 16; // Qualité supérieure
```

---

## Effets temporels

### EchoAudioEffect

**Élément GStreamer** : `audioecho`

**Objectif** : effets d'écho et de delay.

**Paramètres** :

- `Delay` (TimeSpan) : durée de delay de l'écho
    - Ne doit pas dépasser MaxDelay
- `MaxDelay` (TimeSpan) : tampon de delay maximum
    - Doit être >= Delay
    - À définir avant de démarrer la lecture
- `Intensity` (float) : volume de l'écho
    - Plage : 0.0 à 1.0
    - Par défaut : 1.0
- `Feedback` (float) : quantité de répétition de l'écho
    - Plage : 0.0 à 1.0
    - Par défaut : 0.0
    - Plus élevé = plus d'échos

**Utilisation** :

```csharp
var delay = TimeSpan.FromMilliseconds(500);
var effect = new EchoAudioEffect(delay);
effect.Intensity = 0.7f;
effect.Feedback = 0.4f;
```

---

### RSAudioEchoAudioEffect

**Élément GStreamer** : `rsaudioecho` (rsaudiofx)

**Objectif** : écho amélioré avec contrôles avancés.

**Paramètres** :

- `Delay` (TimeSpan) : durée de delay de l'écho
- `MaxDelay` (TimeSpan) : tampon de delay maximum
- `Intensity` (double) : intensité de l'écho
    - Plage : 0.0 à 1.0
- `Feedback` (double) : quantité de feedback
    - Plage : 0.0 à 1.0

**Utilisation** :

```csharp
var effect = new RSAudioEchoAudioEffect();
effect.Delay = TimeSpan.FromMilliseconds(750);
effect.Intensity = 0.6;
effect.Feedback = 0.3;
```

---

### ReverberationAudioEffect

**Élément GStreamer** : `freeverb`

**Objectif** : simulation de réverbération de pièce.

**Paramètres** :

- `RoomSize` (float) : taille virtuelle de la pièce
    - Plage : 0.0 à 1.0
    - Par défaut : 0.5
    - Plus grand = queue de réverbération plus longue
- `Damping` (float) : absorption des hautes fréquences
    - Plage : 0.0 à 1.0
    - Par défaut : 0.2
    - Plus élevé = réverbération plus sombre
- `Level` (float) : mix wet/dry
    - Plage : 0.0 à 1.0
    - Par défaut : 0.5
    - 0 = dry, 1 = wet
- `Width` (float) : largeur stéréo
    - Plage : 0.0 à 1.0
    - Par défaut : 1.0
    - 0 = mono, 1 = stéréo complète

**Utilisation** :

```csharp
var effect = new ReverberationAudioEffect();
effect.RoomSize = 0.8f; // Grande pièce
effect.Damping = 0.3f;
effect.Level = 0.4f;
```

---

### FadeAudioEffect

**Objectif** : automation de fondu volumique en entrée/sortie.

**Paramètres** :

- `StartVolume` (uint) : volume à l'heure de début
- `StopVolume` (uint) : volume à l'heure d'arrêt
- `StartTime` (TimeSpan) : moment où le fondu commence
- `StopTime` (TimeSpan) : moment où le fondu se termine

**Utilisation** :

```csharp
// Fondu sortant sur 3 secondes à partir de 10 secondes
var effect = new FadeAudioEffect(
    100, 0,
    TimeSpan.FromSeconds(10),
    TimeSpan.FromSeconds(13)
);
```

---

## Effets de modulation

### PhaserAudioEffect

**Objectif** : effet phaser avec modulation LFO.

**Paramètres** :

- `Depth` (byte) : profondeur du balayage
    - Plage : 0 à 255
- `DryWetRatio` (byte) : ratio de mix
    - Plage : 0 à 255
    - 0 = dry, 255 = wet
- `Feedback` (byte) : résonance
    - Plage : -100 à 100
- `Frequency` (float) : vitesse LFO en Hz
    - Typique : 0.1 à 5 Hz
- `Stages` (byte) : nombre d'étages
    - Plage : 2 à 24 recommandée
    - Plus = effet plus prononcé
- `StartPhase` (float) : phase de départ du LFO en radians

**Utilisation** :

```csharp
var effect = new PhaserAudioEffect(
    150,      // profondeur
    128,      // mix 50 %
    50,       // feedback
    0.5f,     // LFO 0,5 Hz
    6,        // 6 étages
    0f        // phase de départ
);
```

---

### FlangerAudioEffect

**Objectif** : effet flanging avec modulation de delay.

**Paramètres** :

- `Delay` (TimeSpan) : durée de delay de base
    - Typique : 1-15 ms
- `Frequency` (float) : vitesse LFO en Hz
    - Typique : 0.1 à 5 Hz
- `PhaseInvert` (bool) : inverser la phase du signal retardé
    - Par défaut : false

**Utilisation** :

```csharp
var effect = new FlangerAudioEffect(
    TimeSpan.FromMilliseconds(5),
    1.0f,
    false
);
```

---

## Effets de hauteur et de tempo

### PitchShiftAudioEffect

**Objectif** : changer la hauteur sans changer la vitesse.

**Paramètres** :

- `Pitch` (float) : ratio de décalage de hauteur
    - 1.0 = aucun changement
    - 2.0 = une octave plus haut
    - 0.5 = une octave plus bas
    - Plage typique : 0.5 à 2.0

**Intervalles musicaux** :

- 0.5 = -12 demi-tons
- 1.059 = +1 demi-ton
- 1.122 = +2 demi-tons
- 2.0 = +12 demi-tons

**Utilisation** :

```csharp
var effect = new PitchShiftAudioEffect(1.5f); // Hausser d'une quinte
```

---

### ScaleTempoAudioEffect

**Élément GStreamer** : `scaletempo`

**Objectif** : changer le tempo sans changer la hauteur (algorithme WSOLA).

**Paramètres** :

- `Rate` (double) : vitesse de lecture
    - 1.0 = normale
    - 2.0 = double vitesse
    - 0.5 = demi-vitesse
    - Par défaut : 1.0
- `Stride` (TimeSpan) : pas de traitement
    - Par défaut : 30 ms
- `Overlap` (double) : pourcentage de recouvrement
    - Plage : 0.0 à 1.0
    - Par défaut : 0.2
- `Search` (TimeSpan) : fenêtre de recherche
    - Par défaut : 14 ms

**Utilisation** :

```csharp
var effect = new ScaleTempoAudioEffect(1.5); // Vitesse 1,5x
```

---

## Effets spéciaux

### KaraokeAudioEffect

**Élément GStreamer** : `audiokaraoke`

**Objectif** : supprimer les voix centrées dans le panoramique.

**Paramètres** :

- `FilterBand` (float) : fréquence centrale en Hz
    - Par défaut : 220 Hz
    - Typique : 80-400 Hz
- `FilterWidth` (float) : bande passante du filtre en Hz
    - Par défaut : 100 Hz
- `Level` (float) : intensité de l'effet
    - Plage : 0.0 à 1.0
    - Par défaut : 1.0
- `MonoLevel` (float) : niveau du canal mono
    - Plage : 0.0 à 1.0
    - Par défaut : 1.0

**Utilisation** :

```csharp
var effect = new KaraokeAudioEffect();
effect.FilterBand = 250f;
effect.Level = 1.0f;
```

---

### RemoveSilenceAudioEffect

**Objectif** : supprimer automatiquement les sections silencieuses.

**Paramètres** :

- `Threshold` (double) : seuil de détection du silence
    - Plage : 0.0 à 1.0
    - Par défaut : 0.05
    - Plus bas = plus sensible
- `Squash` (bool) : supprimer vs réduire le silence
    - true = supprimer complètement
    - false = réduire le niveau
    - Par défaut : true

**Utilisation** :

```csharp
var effect = new RemoveSilenceAudioEffect("silence-remover");
effect.Threshold = 0.02;
effect.Squash = true;
```

---

### CsoundAudioEffect

**Élément GStreamer** : `csoundfilter`

**Objectif** : programmation audio basée sur Csound.

**Plateformes** : Windows, macOS, Linux (nécessite l'installation de Csound).

**Paramètres** :

- `CsdText` (string) : document CSD Csound sous forme de texte
- `Location` (string) : chemin vers un fichier CSD
- `Loop` (bool) : boucler la partition en continu
    - Par défaut : false
- `ScoreOffset` (double) : heure de début en secondes
    - Par défaut : 0.0

**Utilisation** :

```csharp
var csd = @"<CsoundSynthesizer>
<CsInstruments>
; Votre code Csound ici
</CsInstruments>
<CsScore>
; Votre partition ici
</CsScore>
</CsoundSynthesizer>";

var effect = new CsoundAudioEffect("my-csound", csd);
effect.Loop = false;
```

---

## Réduction de bruit et mesure

### AudioRNNoiseAudioEffect

**Élément GStreamer** : `audiornnoise` (rsaudiofx)

**Objectif** : réduction de bruit basée sur l'IA.

**Paramètres** :

- `VadThreshold` (float) : seuil de détection d'activité vocale
    - Plage : 0.0 à 1.0
    - Par défaut : 0.0
    - Plus élevé = plus sensible à la voix

**Utilisation** :

```csharp
var effect = new AudioRNNoiseAudioEffect();
effect.VadThreshold = 0.5f;
```

---

### AudioLoudNormAudioEffect

**Élément GStreamer** : `audioloudnorm` (rsaudiofx)

**Objectif** : normalisation de la sonie EBU R128.

**Paramètres** :

- `LoudnessTarget` (double) : sonie cible en LUFS
    - Plage : -70.0 à -5.0
    - Par défaut : -24.0
- `LoudnessRangeTarget` (double) : plage cible en LU
    - Plage : 1.0 à 20.0
    - Par défaut : 7.0
- `MaxTruePeak` (double) : pic vrai max en dbTP
    - Plage : -9.0 à 0.0
    - Par défaut : -2.0
- `Offset` (double) : gain d'offset en LU
    - Plage : -99.0 à 99.0
    - Par défaut : 0.0

**Utilisation** :

```csharp
var effect = new AudioLoudNormAudioEffect();
effect.LoudnessTarget = -16.0; // Norme de streaming
effect.MaxTruePeak = -1.0;
```

---

### EbuR128LevelAudioEffect

**Élément GStreamer** : `ebur128level` (rsaudiofx)

**Objectif** : mesure de sonie EBU R128.

**Paramètres** :

- `Mode` (EbuR128Mode) : types de mesure à calculer
    - Options : Momentary, ShortTerm, Global, LoudnessRange, SamplePeak, TruePeak, All
    - Par défaut : All
- `PostMessages` (bool) : publier les messages de mesure
    - Par défaut : true
- `Interval` (TimeSpan) : intervalle de mise à jour des mesures
    - Par défaut : 1 seconde

**Utilisation** :

```csharp
var effect = new EbuR128LevelAudioEffect();
effect.Mode = EbuR128Mode.All;
effect.Interval = TimeSpan.FromSeconds(0.5);
```

---

## Gestion des canaux

### ChannelOrderAudioEffect

**Objectif** : remapper les canaux audio.

**Paramètres** :

- `Orders` (byte[,]) : tableau 2D de paires [cible, source]
    - Format : [[target0, source0], [target1, source1], ...]
    - Les canaux sont indexés à partir de zéro

**Utilisation** :

```csharp
// Échanger les canaux gauche et droit
var orders = new byte[2, 2] {
    { 0, 1 },  // Le canal de sortie 0 reçoit le canal d'entrée 1 (droit)
    { 1, 0 }   // Le canal de sortie 1 reçoit le canal d'entrée 0 (gauche)
};
var effect = new ChannelOrderAudioEffect(orders);
```

---

### DownMixAudioEffect

**Implémentation** : DSP personnalisé (moyennage des canaux)

**Objectif** : réduire le nombre de canaux (par exemple, 5.1 vers stéréo).

**Paramètres** : aucun (downmixage automatique).

**Utilisation** :

```csharp
var effect = new DownMixAudioEffect();
```

---

## Effets DirectSound (SDK classiques, Windows uniquement)

Les effets suivants sont disponibles uniquement dans Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore) et Video Edit SDK (VideoEditCore) sur Windows. Ils utilisent la technologie DirectSound/DirectX.

### DS Chorus

Crée un effet chorus avec plusieurs copies retardées et modulées.

**Propriétés** :

- **WetDryMix** (float) : mix dry/wet (0-100)
- **Depth** (float) : profondeur de modulation (0-100)
- **Feedback** (float) : quantité de feedback (0-100)
- **Frequency** (float) : fréquence du LFO (0-10 Hz)
- **Waveform** : Sine ou Triangle
- **Delay** (float) : delay de base (0-20 ms)
- **Phase** : relation de phase pour la stéréo (-180 à 180 degrés)

**Utilisation** :

```csharp
// Signature : (int streamIndex, string name, float delay, float depth,
//             float feedback, float frequency, DSChorusPhase phase,
//             DSChorusWaveForm waveformTriangle, float wetDryMix)
videoCaptureCore.Audio_Effects_DS_Chorus(0, "chorus",
    delay: 16, depth: 25, feedback: 25, frequency: 1.1f,
    phase: DSChorusPhase.Phase90, waveformTriangle: DSChorusWaveForm.Sine,
    wetDryMix: 50);
```

---

### DS Distortion

Ajoute de la distorsion/overdrive au signal audio.

**Propriétés** :

- **Gain** (float) : gain avant distorsion (-60 à 0 dB)
- **Edge** (float) : quantité de distorsion (0-100 %)
- **PostEQCenterFrequency** (float) : centre de l'EQ post-distorsion (100-8000 Hz)
- **PostEQBandwidth** (float) : bande passante de l'EQ post (100-8000 Hz)
- **PreLowpassCutoff** (float) : passe-bas avant distorsion (100-8000 Hz)

**Utilisation** :

```csharp
// Signature : (int streamIndex, string name, float edge, float gain,
//             float postEQBandwidth, float postEQCenterFrequency,
//             float preLowpassCutOff)
videoCaptureCore.Audio_Effects_DS_Distortion(0, "distortion",
    edge: 50, gain: -18, postEQBandwidth: 2400,
    postEQCenterFrequency: 2400, preLowpassCutOff: 8000);
```

---

### DS Gargle

Crée un effet de modulation gargouillis/trémolo.

**Propriétés** :

- **RateHz** (int) : taux de modulation (1-1000 Hz)
- **WaveForm** : onde triangulaire ou carrée

**Utilisation** :

```csharp
// Signature : (int streamIndex, string name, int rateHz, DSGargleWaveForm waveForm)
videoCaptureCore.Audio_Effects_DS_Gargle(0, "gargle",
    rateHz: 20, waveForm: DSGargleWaveForm.Triangle);
```

---

### DS Reverb (I3DL2)

Réverbération professionnelle avec modélisation environnementale.

**Propriétés** :

- **Room** (int) : niveau d'effet de pièce (-10000 à 0 mB)
- **RoomHF** (int) : effet de pièce hautes fréquences (-10000 à 0 mB)
- **RoomRolloffFactor** (float) : facteur de roll-off (0 à 10)
- **DecayTime** (float) : temps de décroissance (0,1 à 20 secondes)
- **DecayHFRatio** (float) : ratio de décroissance HF (0,1 à 2,0)
- **Reflections** (int) : premières réflexions (-10000 à 1000 mB)
- **ReflectionsDelay** (float) : delay des réflexions (0 à 0,3 seconde)
- **Reverb** (int) : niveau de réverbération tardive (-10000 à 2000 mB)
- **ReverbDelay** (float) : delay de réverbération (0 à 0,1 seconde)
- **Diffusion** (float) : diffusion (0 à 100 %)
- **Density** (float) : densité (0 à 100 %)
- **HFReference** (float) : référence HF (20 à 20000 Hz)

---

### DS Waves Reverb

Réverbération simplifiée avec paramètres de base.

**Propriétés** :

- **InGain** (float) : gain d'entrée (0 à 96 dB)
- **ReverbMix** (float) : mix de réverbération (0 à 96 dB)
- **ReverbTime** (float) : temps de réverbération (0,001 à 3000 ms)
- **HighFreqRTRatio** (float) : ratio de temps de réverbération HF (0,001 à 0,999)

**Utilisation** :

```csharp
// Signature : (int streamIndex, string name, float highFreqRTRatio,
//             float inGain, float reverbMix, float reverbTime)
videoCaptureCore.Audio_Effects_DS_WavesReverb(0, "reverb",
    highFreqRTRatio: 0.001f, inGain: 0, reverbMix: -10, reverbTime: 1000);
```

---

## Matrice de disponibilité des effets

| Effet | SDK multiplateformes | SDK classiques | Plateformes |
|--------|---------------------|--------------|-----------|
| **Contrôle de volume/niveau** | | | |
| Volume | Oui | Oui | Multiplateforme / Windows |
| Amplify | Oui | Oui | Multiplateforme / Windows |
| **Traitement stéréo** | | | |
| Balance | Oui | Non | Multiplateforme |
| Wide Stereo | Oui | Non | Multiplateforme |
| Karaoke | Oui | Non | Multiplateforme |
| **Delay et modulation** | | | |
| Echo | Oui | Oui | Multiplateforme / Windows |
| Reverberation (Freeverb) | Oui | Non | Multiplateforme |
| Flanger | Oui | Oui | Multiplateforme / Windows |
| Phaser | Oui | Oui | Multiplateforme / Windows |
| **Hauteur et tempo** | | | |
| Pitch Shift | Oui | Oui | Multiplateforme / Windows |
| Scale Tempo | Oui | Non | Multiplateforme |
| Tempo | Oui | Oui | Multiplateforme / Windows |
| **Égalisation** | | | |
| Equalizer 10-band | Oui | Non | Multiplateforme |
| Equalizer Parametric | Oui | Oui | Multiplateforme / Windows |
| **Filtrage** | | | |
| High-Pass | Oui | Oui | Multiplateforme / Windows |
| Low-Pass | Oui | Oui | Multiplateforme / Windows |
| Band-Pass | Oui | Oui | Multiplateforme / Windows |
| Notch | Oui | Oui | Multiplateforme / Windows |
| Chebyshev Band Pass/Reject | Oui | Non | Multiplateforme |
| Chebyshev Limit | Oui | Non | Multiplateforme |
| **Traitement dynamique** | | | |
| Compressor/Expander | Oui | Non | Multiplateforme |
| Dynamic Amplify | Oui | Oui | Multiplateforme / Windows |
| **Renforcement de fréquence** | | | |
| TrueBass | Oui | Oui | Multiplateforme / Windows |
| Treble Enhancer | Oui | Oui | Multiplateforme / Windows |
| **Effets avancés** | | | |
| Phase Invert | Oui | Oui | Multiplateforme / Windows |
| Sound 3D | Oui | Oui | Multiplateforme / Windows |
| Channel Order | Oui | Oui | Multiplateforme / Windows |
| Down Mix | Oui | Oui | Multiplateforme / Windows |
| Fade | Oui | Oui | Multiplateforme / Windows |
| **Réduction de bruit** | | | |
| Remove Silence | Oui | Non | Multiplateforme |
| Audio RNNoise | Oui | Non | Multiplateforme (nécessite un plugin) |
| Audio Loud Norm | Oui | Non | Multiplateforme (nécessite un plugin) |
| **Analyse** | | | |
| EBU R128 Level | Oui | Non | Multiplateforme (nécessite un plugin) |
| **Audio spatial** | | | |
| HRTF Render | Oui | Non | Multiplateforme (nécessite un plugin) |
| **Spécialisés** | | | |
| RS Audio Echo | Oui | Non | Multiplateforme (nécessite un plugin) |
| Csound Filter | Oui | Non | Windows/macOS/Linux (nécessite Csound) |
| **Effets DirectSound (Windows classique uniquement)** | | | |
| DS Chorus | Non | Oui | Windows uniquement |
| DS Distortion | Non | Oui | Windows uniquement |
| DS Gargle | Non | Oui | Windows uniquement |
| DS Reverb (I3DL2) | Non | Oui | Windows uniquement |
| DS Waves Reverb | Non | Oui | Windows uniquement |

**Légende** :

- **SDK multiplateformes** = Media Blocks SDK, Video Capture SDK (VideoCaptureCoreX), Media Player SDK (MediaPlayerCoreX)
- **SDK classiques** = Video Capture SDK (VideoCaptureCore), Media Player SDK (MediaPlayerCore), Video Edit SDK (VideoEditCore) — Windows uniquement

## Référence des éléments audio

| Effet | Élément audio | Plugin |
|--------|---------------|--------|
| Volume | volume | coreelements |
| Amplify | audioamplify | audiofx |
| Balance | audiopanorama | audiofx |
| Echo | audioecho | audiofx |
| Karaoke | audiokaraoke | audiofx |
| Wide Stereo | stereo | audiofx |
| Reverberation | freeverb | freeverb |
| Equalizer 10-band | equalizer-10bands | audiofx |
| High-Pass | audiocheblimit | audiofx |
| Low-Pass | audiocheblimit | audiofx |
| Chebyshev Band | audiochebband | audiofx |
| Chebyshev Limit | audiocheblimit | audiofx |
| Compressor | audiodynamic | audiofx |
| Scale Tempo | scaletempo | audiofx |
| Pitch Shift | pitch | soundtouch |
| Audio RNNoise | audiornnoise | rsaudiofx |
| Audio Loud Norm | audioloudnorm | rsaudiofx |
| EBU R128 Level | ebur128level | rsaudiofx |
| RS Audio Echo | rsaudioecho | rsaudiofx |
| HRTF Render | hrtfrender | rsaudiofx |
| Csound Filter | csoundfilter | csound |

## Foire aux questions

???+ "Quelle est la valeur par défaut des paramètres des effets audio ?"
    Chaque effet possède des valeurs par défaut documentées qui représentent un comportement neutre/bypass. Par exemple, `VolumeAudioEffect` a par défaut un niveau de 1.0 (100 %), `Equalizer10AudioEffect` a toutes les bandes par défaut à 0 dB, et `ReverberationAudioEffect` a par défaut une simulation de pièce modérée. Consultez le tableau de paramètres de chaque effet ci-dessus pour les valeurs par défaut spécifiques.

???+ "Comment réinitialiser un effet audio à ses paramètres par défaut ?"
    Créez une nouvelle instance de l'effet avec les paramètres de constructeur par défaut et appelez `Audio_Effects_AddOrUpdate()` pour remplacer les paramètres actuels. Le constructeur par défaut de chaque effet initialise tous les paramètres à leurs valeurs par défaut documentées.

???+ "Puis-je utiliser plusieurs instances du même type d'effet ?"
    Oui. Les **noms** d'effets sont utilisés comme identifiants uniques, pas les types d'effets. Pour exécuter plusieurs instances du même type simultanément, donnez à chaque instance un nom distinct. Appeler `Audio_Effects_AddOrUpdate()` avec un effet dont le nom correspond à un existant remplace cette instance ; un nouveau nom ajoute une nouvelle instance à la chaîne.

???+ "Quelles fréquences d'échantillonnage et configurations de canaux sont prises en charge ?"
    Tous les effets audio multiplateformes prennent en charge les fréquences d'échantillonnage standard (8 kHz à 192 kHz) et les configurations de canaux (mono, stéréo, multicanal). Les effets s'adaptent automatiquement au format audio du flux d'entrée. Certains effets comme `BalanceAudioEffect` et `WideStereoAudioEffect` nécessitent une entrée stéréo.

???+ "Comment supprimer un effet audio pendant la lecture ?"
    Utilisez la méthode `Audio_Effects_Remove()` avec le type d'effet pour le supprimer de la chaîne de traitement pendant la lecture. Le changement prend effet immédiatement sans interrompre le flux audio.

## Voir aussi

- [Aperçu des effets audio](index.md)
- [Audio Sample Grabber](audio-sample-grabber.md)
- [Blocs de traitement audio (Media Blocks SDK)](../../mediablocks/AudioProcessing/index.md)
