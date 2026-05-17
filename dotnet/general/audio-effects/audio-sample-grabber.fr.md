---
title: Audio Sample Grabber pour .NET — Capturer l'audio brut
description: Capturez et traitez des images audio en temps réel avec Audio Sample Grabber sur les moteurs X et les moteurs classiques dans les applications SDK .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - C#
primary_api_classes:
  - AudioFrameBufferEventArgs
  - AudioSampleGrabberBlock

---

# Utilisation d'Audio Sample Grabber dans les SDK .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à Audio Sample Grabber

Audio Sample Grabber est une fonctionnalité puissante disponible dans nos SDK .NET, qui permet aux développeurs d'accéder directement aux trames audio brutes, qu'elles proviennent de sources en direct ou de fichiers multimédias. Cette capacité ouvre un large éventail de possibilités pour le traitement, l'analyse et la manipulation audio dans vos applications.

Lors du traitement audio, l'accès aux trames audio individuelles est essentiel pour des tâches telles que :

- Visualisation audio en temps réel
- Traitement d'effets audio personnalisés
- Intégration de la reconnaissance vocale
- Analyse et mesures audio
- Conversion personnalisée de format audio
- Algorithmes de détection sonore

L'événement `OnAudioFrameBuffer` est le mécanisme central qui fournit l'accès à ces trames audio brutes. Cet événement est déclenché chaque fois qu'une nouvelle trame audio est disponible, ce qui vous donne un accès direct à la mémoire non managée contenant les données audio décodées.

## Fonctionnement d'Audio Sample Grabber

Audio Sample Grabber intercepte le pipeline audio durant la lecture ou la capture, et vous fournit les données audio brutes avant qu'elles ne soient rendues vers le périphérique de sortie. Ces données sont généralement au format PCM (Pulse Code Modulation), le format standard de l'audio numérique non compressé, mais peuvent occasionnellement être au format à virgule flottante IEEE selon la source audio.

Chaque fois que l'événement `OnAudioFrameBuffer` est déclenché, il fournit un objet `AudioFrameBufferEventArgs` contenant des informations essentielles sur la trame audio :

- `Frame.Data` : un `IntPtr` pointant vers le bloc de mémoire non managée contenant les données audio brutes
- `Frame.DataSize` : la taille des données audio en octets
- `Frame.Info` : une structure contenant des informations détaillées sur le format audio, notamment :
  - Nombre de canaux (mono, stéréo, etc.)
  - Fréquence d'échantillonnage (typiquement 44,1 kHz, 48 kHz, etc.)
  - Bits par échantillon (16 bits, 24 bits, etc.)
  - Type de format audio (PCM, IEEE, etc.)
  - Informations d'horodatage
  - Alignement de bloc et autres détails propres au format

## Configuration d'Audio Sample Grabber

Le processus de configuration varie légèrement selon que vous utilisez nos nouveaux moteurs X ou les moteurs classiques. Examinons les deux approches :

=== "Moteurs X"

    
    Pour les moteurs X, la configuration d'Audio Sample Grabber est simple. Il vous suffit de créer un gestionnaire d'événements pour l'événement `OnAudioFrameBuffer` :
    
    ```csharp
    VideoCapture1.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    
    L'architecture des moteurs X active automatiquement la capture d'échantillons audio dès que vous vous abonnez à cet événement, sans configuration supplémentaire requise.
    

=== "Moteurs classiques"

    
    Avec les moteurs classiques, vous devez activer explicitement la fonctionnalité Audio Sample Grabber avant de créer le gestionnaire d'événements :
    
    ```csharp
    VideoCapture1.Audio_Sample_Grabber_Enabled = true;
    ```
    
    Ensuite, comme pour les moteurs X, créez votre gestionnaire d'événements :
    
    ```csharp
    VideoCapture1.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    
    **Note** : la propriété `Audio_Sample_Grabber_Enabled` n'est pas requise pour le composant VideoEditCore, qui a la capture d'échantillons audio activée par défaut.
    

=== "Media Blocks SDK"

    
    Le Media Blocks SDK prend également en charge la capture d'échantillons audio. Utilisez le composant `AudioSampleGrabberBlock` pour capturer les trames audio.
    
    ```csharp
    private AudioSampleGrabberBlock _audioSampleGrabberSink;
    ```
    
    Ensuite, comme pour les moteurs X, créez votre gestionnaire d'événements et spécifiez le format audio :
    
    ```csharp
    _audioSampleGrabberBlock = new AudioSampleGrabberBlock(VisioForge.Core.Types.X.AudioFormatX.S16LE);
    _audioSampleGrabberBlock.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    


## Traitement des trames audio

Une fois le gestionnaire d'événements configuré, vous pouvez traiter les trames audio au fur et à mesure de leur arrivée. Voici un exemple de base de prise en charge de l'événement `OnAudioFrameBuffer` :

```csharp
using VisioForge.Types;
using System.Diagnostics;

private void OnAudioFrameBuffer(object sender, AudioFrameBufferEventArgs e)
{
    // Journaliser les informations de la trame audio
    Debug.WriteLine($"Audio frame: {e.Frame.DataSize} bytes; Format: {e.Frame.Info}");
    
    // Accès aux données audio brutes via le pointeur non managé
    IntPtr rawAudioData = e.Frame.Data;
    
    // Obtenir les détails du format audio. RAWBaseAudioInfo contient quatre champs :
    // Channels, SampleRate, BPS (bits par échantillon), Format (enum AudioFormat).
    int channelCount  = e.Frame.Info.Channels;
    int sampleRate    = e.Frame.Info.SampleRate;
    int bitsPerSample = e.Frame.Info.BPS;
    AudioFormat format = e.Frame.Info.Format;
    
    // Votre code de traitement audio personnalisé ici
    // ...
}
```

## Travailler avec les données audio

### Conversion de la mémoire non managée en tableaux managés

Bien que `e.Frame.Data` fournisse un pointeur vers la mémoire non managée, il est souvent plus pratique de manipuler les données sous une forme plus accessible. La classe `AudioFrame` propose une méthode utile `GetDataArray()` qui retourne une copie des données audio sous forme de tableau d'octets :

```csharp
private void VideoCapture1_OnAudioFrameBuffer(object sender, AudioFrameBufferEventArgs e)
{
    // Obtenir une copie managée des données audio
    byte[] audioData = e.Frame.GetDataArray();
    
    // Vous pouvez désormais travailler avec les données via les opérations standard de tableau C#
    // ...
}
```

### Conversion des données PCM en échantillons

Pour de nombreuses tâches de traitement audio, vous souhaiterez convertir les octets PCM bruts en véritables valeurs d'échantillons audio. Voici une méthode utilitaire pour convertir un tableau d'octets PCM en tableau d'échantillons audio (en supposant des échantillons 16 bits) :

```csharp
private short[] ConvertBytesToSamples(byte[] audioData)
{
    short[] samples = new short[audioData.Length / 2];
    
    for (int i = 0; i < samples.Length; i++)
    {
        // Combiner deux octets en un échantillon 16 bits
        samples[i] = (short)(audioData[i * 2] | (audioData[i * 2 + 1] << 8));
    }
    
    return samples;
}
```

### Gestion de l'audio multicanal

Lorsque vous travaillez avec de l'audio stéréo ou multicanal, les échantillons sont généralement entrelacés. Pour un flux stéréo, les données sont organisées comme suit : [Gauche0, Droite0, Gauche1, Droite1, ...]. Vous voudrez peut-être séparer ces canaux avant traitement :

```csharp
private void ProcessStereoAudio(short[] samples, int channelCount)
{
    if (channelCount != 2) return;
    
    // Créer des tableaux pour chaque canal
    int samplesPerChannel = samples.Length / 2;
    short[] leftChannel = new short[samplesPerChannel];
    short[] rightChannel = new short[samplesPerChannel];
    
    // Séparer les canaux
    for (int i = 0; i < samplesPerChannel; i++)
    {
        leftChannel[i] = samples[i * 2];
        rightChannel[i] = samples[i * 2 + 1];
    }
    
    // Traiter chaque canal séparément
    // ...
}
```

## Scénarios courants de traitement audio

### Mesure du niveau audio

Un cas d'usage courant d'Audio Sample Grabber est l'implémentation d'un VU-mètre :

```csharp
private void CalculateAudioLevel(short[] samples)
{
    double sum = 0;
    
    // Calculer la valeur RMS (Root Mean Square)
    foreach (short sample in samples)
    {
        sum += sample * sample;
    }
    
    double rms = Math.Sqrt(sum / samples.Length);
    
    // Convertir en décibels
    double db = 20 * Math.Log10(rms / 32768);
    
    // Mettre à jour l'UI avec le niveau (utilisez Invoke si vous êtes sur un autre thread)
    Debug.WriteLine($"Audio level: {db} dB");
}
```

### FFT en temps réel pour l'analyse spectrale

Pour l'analyse du spectre de fréquences, vous pouvez réaliser une FFT (transformée de Fourier rapide) sur les données audio :

```csharp
// Note : vous aurez besoin d'une bibliothèque pour le calcul FFT
// Ceci est un exemple simplifié
private void PerformFFTAnalysis(short[] samples)
{
    // En général, vous utiliseriez une bibliothèque comme Math.NET Numerics
    // Convertir les échantillons en nombres complexes
    Complex[] complex = samples.Select(s => new Complex(s, 0)).ToArray();
    
    // Effectuer la FFT (pseudo-code)
    // Complex[] fftResult = FFT.Forward(complex);
    
    // Traiter les résultats de la FFT
    // ...
}
```

## Considérations de performance

Lorsque vous travaillez avec Audio Sample Grabber, gardez ces considérations de performance à l'esprit :

1. **Minimisez le temps de traitement** : l'événement `OnAudioFrameBuffer` est appelé sur le thread de traitement audio. Les opérations de longue durée peuvent provoquer des artefacts audio.

2. **Tenez compte de la sécurité des threads** : si vous devez mettre à jour des éléments d'interface utilisateur ou interagir avec d'autres composants, utilisez des mécanismes de synchronisation de threads adaptés.

3. **Évitez les allocations de mémoire** : des allocations de mémoire fréquentes dans le gestionnaire d'événements peuvent entraîner des pauses du ramasse-miettes. Réutilisez les tableaux lorsque c'est possible.

4. **Copie de tampons** : la méthode `GetDataArray()` crée une copie des données audio. Pour les scénarios à très hautes performances, envisagez de travailler directement avec le pointeur non managé.

## Conclusion

Audio Sample Grabber offre un moyen puissant d'accéder aux données audio brutes et de les traiter en temps réel, qu'elles proviennent de sources en direct ou de fichiers multimédias. En tirant parti de cette fonctionnalité, vous pouvez implémenter des fonctionnalités de traitement audio sophistiquées dans vos applications, du simple VU-mètre à l'analyse audio complexe et au traitement des effets.

Que vous construisiez une application audio professionnelle, que vous mettiez en œuvre une visualisation audio ou que vous intégriez des services de reconnaissance vocale, Audio Sample Grabber vous donne les données brutes nécessaires pour donner vie à vos idées de traitement audio.

---
Consultez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir d'autres exemples de code.
