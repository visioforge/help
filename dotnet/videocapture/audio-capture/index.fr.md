---
title: Capturer l'audio système et le microphone en C# .NET
description: Enregistrez le microphone et capturez le son système (loopback) en C# avec le SDK VisioForge. Exemples complets pour enregistrement audio en MP3, M4A, WAV.
sidebar_label: Capture audio
order: 10
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - DeviceEnumerator
  - VideoCaptureCoreX
  - LoopbackAudioCaptureDeviceSourceSettings
  - SystemAudioSourceBlock
  - MediaBlocksPipeline

video:
  id: dXehQhz7xa0
  upload_date: 2026-11-24T15:25:52Z
  duration: PT59S
---

# Capture audio et enregistrement du son système en C# .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Le VisioForge Video Capture SDK offre des fonctionnalités de capture audio aux développeurs .NET, couvrant l'enregistrement du microphone, la capture de l'audio système (loopback/haut-parleur) et l'enregistrement audio combiné. Que vous construisiez un enregistreur de podcast, un outil d'enregistrement d'écran avec audio ou une application de capture vocale, le SDK gère l'énumération des périphériques, la négociation des formats et l'encodage.

Ce guide fournit des exemples de code complets et exécutables pour les scénarios de capture audio les plus courants en utilisant les API **Video Capture SDK X** et **Media Blocks SDK**.

## Sources audio prises en charge

- **Microphones physiques** — microphones de bureau, USB et Bluetooth
- **Ports d'entrée ligne** — mélangeurs externes ou instruments
- **Audio système (loopback)** — enregistrer ce qui est lu par vos haut-parleurs ou écouteurs
- **Périphériques audio virtuels** — capturer l'audio d'autres applications
- **Flux réseau** — audio depuis RTSP, HTTP et autres sources de streaming

Pour la configuration détaillée des sources, consultez [Sources audio](../audio-sources/index.md).

## Prise en charge des formats audio

### Formats avec perte

- [MP3](../../general/audio-encoders/mp3.md) — standard de l'industrie, débits ajustables de 8 kbps à 320 kbps
- [M4A (AAC)](../../general/audio-encoders/aac.md) — excellent rapport qualité/taille
- [Windows Media Audio](../../general/audio-encoders/wma.md) — bonne compression avec intégration Windows
- [Ogg Vorbis](../../general/audio-encoders/vorbis.md) — open source, excellente qualité à des débits plus faibles
- [Speex](../../general/audio-encoders/speex.md) — optimisé pour la parole

### Formats sans perte

- [WAV](../../general/audio-encoders/wav.md) — non compressé, qualité parfaite
- [FLAC](../../general/audio-encoders/flac.md) — compression sans perte sans dégradation de qualité

## Enregistrer l'audio du microphone en MP3

Cette application console enregistre l'audio depuis le microphone par défaut et le sauvegarde sous forme de fichier MP3.

### Paquets NuGet requis

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.VideoCapture
```

Ajoutez le [paquet redist](../../deployment-x/index.md) pour votre plateforme (par ex. `VisioForge.DotNet.Core.Redist.Base.x64`).

Les mêmes paquets sont utilisés pour l’essai de 30 jours et les applications sous licence. Consultez la [page de licence VisioForge](https://www.visioforge.com/licensing) pour les conditions d’essai et l’activation avec `.vflicense`.

### Exemple complet

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();

        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Énumérer les périphériques de capture audio
            var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
                AudioCaptureDeviceAPI.DirectSound);

            if (audioDevices.Length == 0)
            {
                Console.WriteLine("No audio capture device found.");
                return;
            }

            // Afficher les périphériques disponibles
            Console.WriteLine("Available audio devices:");
            for (int i = 0; i < audioDevices.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioDevices[i].DisplayName}");
            }

            // Sélectionner le premier périphérique (microphone par défaut)
            var selectedDevice = audioDevices[0];
            var audioFormat = selectedDevice.GetDefaultFormat();
            var audioSource = selectedDevice.CreateSourceSettingsVC(audioFormat);

            // Configurer la capture audio uniquement
            videoCapture.Audio_Source = audioSource;
            videoCapture.Video_Source = null;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;

            // Configurer la sortie MP3
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"mic_recording_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");

            var mp3Output = new MP3Output(outputPath);
            videoCapture.Outputs_Add(mp3Output, autostart: true);

            // Démarrer l'enregistrement
            await videoCapture.StartAsync();
            Console.WriteLine($"Recording to: {outputPath}");
            Console.WriteLine("Press ENTER to stop...");
            Console.ReadLine();

            // Arrêter et sauvegarder
            await videoCapture.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### La version courte

La vidéo d'une minute exécute cette application console et lit le fichier MP3 qu'elle a enregistré.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/dXehQhz7xa0?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Une application console complète : elle enregistre le microphone par défaut dans `mic.mp3` jusqu'à l'appui sur Entrée. `DeviceEnumerator.Shared.AudioSourcesAsync()` liste les entrées audio de toutes les API audio de la machine ; sous Windows, `IsDefault` marque l'entrée WASAPI2 `Default Audio Capture Device`, qui suit le microphone choisi dans les paramètres de son de Windows. Pour enregistrer une autre entrée, passez en premier argument une partie de son nom qu'aucun autre périphérique n'a, exactement comme l'affichent les paramètres de son de Windows (la casse et les accents comptent). Chaque périphérique de sortie figure aussi comme entrée loopback sous son propre nom : passer le nom d'une sortie, comme `Haut-parleurs`, enregistre donc ce que joue cette sortie. Un périphérique peut figurer sous plusieurs API audio, et la première entrée dont le nom contient le texte est utilisée, quelle que soit son API. Sans entrée par défaut, ou si aucun nom ne correspond, `First` lève une exception. `CreateSourceSettingsVC()` ouvre le périphérique dans son format par défaut. `Audio_Play = false` garde le son hors des haut-parleurs, `Audio_Record = true` l'envoie vers la sortie, et `MP3Output` l'encode en MP3, à 192 kbit/s CBR par défaut. `StopAsync` termine le fichier : fermer la fenêtre ou appuyer sur Ctrl+C arrête l'application sans cet appel, et le fichier reste inachevé.

```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

await VisioForgeX.InitSDKAsync();
await using var rec = new VideoCaptureCoreX();
rec.OnError += (_, e) =>
    Console.WriteLine(e.Message);
var mics = await DeviceEnumerator.Shared
    .AudioSourcesAsync();
var mic = args.Length > 0
    ? mics.First(d => d.Name.Contains(args[0]))
    : mics.First(d => d.IsDefault);
rec.Audio_Source =
    mic.CreateSourceSettingsVC();
rec.Audio_Play = false;
rec.Audio_Record = true;
rec.Outputs_Add(new MP3Output("mic.mp3"));
if (!await rec.StartAsync()) return;
Console.ReadLine();
await rec.StopAsync();
```

Dans la vidéo, l'application est lancée par `dotnet run -- Speakers` : l'entrée est le loopback des haut-parleurs de la machine virtuelle, qui enregistre un extrait de voix synthétique joué sur cette machine ; aucun microphone en direct n'a été utilisé. `ffprobe` affiche du MP3 à 192 kbit/s, encodé à partir de l'entrée du périphérique en 48000 Hz stéréo. Le projet complet est [audio-capture-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/audio-capture-short-console), une application console Windows x64 qui référence `VisioForge.DotNet.VideoCapture` et le paquet natif du SDK pour cette plateforme, `VisioForge.CrossPlatform.Core.Windows.x64`.

## Capturer l'audio système (haut-parleur / loopback) { #capture-system-audio-speaker-loopback }

La capture audio système (également appelée loopback ou capture haut-parleur) enregistre tout son lu par le périphérique de sortie de votre ordinateur. Cela est couramment utilisé pour l'enregistrement d'écran avec audio, la capture d'appels en conférence ou l'enregistrement audio en streaming.

Sous Windows, la capture loopback utilise l'API **WASAPI2** pour accéder aux périphériques de sortie.

### Exemple complet — Video Capture SDK X

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();

        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Énumérer les périphériques de sortie audio WASAPI2 (haut-parleurs/écouteurs)
            var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
                AudioOutputDeviceAPI.WASAPI2);

            if (audioOutputs.Length == 0)
            {
                Console.WriteLine("No WASAPI2 audio output device found.");
                return;
            }

            // Afficher les sources loopback disponibles
            Console.WriteLine("Available loopback devices:");
            for (int i = 0; i < audioOutputs.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioOutputs[i].Name}");
            }

            // Sélectionner le premier périphérique
            var outputDevice = audioOutputs[0];

            // Créer les paramètres de source loopback
            var audioSource = new LoopbackAudioCaptureDeviceSourceSettings(outputDevice);
            videoCapture.Audio_Source = audioSource;

            // Configurer pour une capture audio uniquement
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;
            videoCapture.Video_Play = false;

            // Configurer la sortie M4A (AAC)
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"system_audio_{DateTime.Now:yyyyMMdd_HHmmss}.m4a");

            var m4aOutput = new M4AOutput(outputPath);
            videoCapture.Outputs_Add(m4aOutput, autostart: true);

            // Démarrer la capture de l'audio système
            await videoCapture.StartAsync();
            Console.WriteLine($"Capturing system audio to: {outputPath}");
            Console.WriteLine("Play some audio on your computer, then press ENTER to stop...");
            Console.ReadLine();

            // Arrêter et sauvegarder
            await videoCapture.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Exemple complet — Media Blocks SDK

Le Media Blocks SDK utilise une approche par pipeline où vous connectez des blocs de source, de traitement et de sortie. Cet exemple capture l'audio système à l'aide de `SystemAudioSourceBlock` et le sauvegarde dans un fichier M4A.

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();

        MediaBlocksPipeline pipeline = null;

        try
        {
            // Obtenir le premier périphérique de sortie WASAPI2 pour la capture loopback
            var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
                AudioOutputDeviceAPI.WASAPI2);

            if (audioOutputs.Length == 0)
            {
                Console.WriteLine("No WASAPI2 audio output device found.");
                return;
            }

            var outputDevice = audioOutputs[0];
            Console.WriteLine($"Using loopback device: {outputDevice.Name}");

            // Créer le pipeline
            pipeline = new MediaBlocksPipeline();

            // Créer la source audio loopback
            var sourceSettings = new LoopbackAudioCaptureDeviceSourceSettings(outputDevice);
            var audioSource = new SystemAudioSourceBlock(sourceSettings);

            // Créer la sortie M4A
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"system_audio_{DateTime.Now:yyyyMMdd_HHmmss}.m4a");

            var output = new M4AOutputBlock(outputPath);

            // Connecter la source à la sortie
            pipeline.Connect(audioSource, output);

            // Démarrer le pipeline
            await pipeline.StartAsync();
            Console.WriteLine($"Capturing system audio to: {outputPath}");
            Console.WriteLine("Press ENTER to stop...");
            Console.ReadLine();

            // Arrêter le pipeline
            await pipeline.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            if (pipeline != null)
            {
                await pipeline.DisposeAsync();
            }

            VisioForgeX.DestroySDK();
        }
    }
}
```

## Fonctionnalités clés

### Contrôle des périphériques

- Énumérer tous les périphériques d'entrée et de sortie audio disponibles
- Sélectionner par programme des périphériques d'entrée spécifiques
- Définir le volume d'entrée et l'état muet
- Surveiller les niveaux audio en temps réel avec des [VU-mètres](../../general/code-samples/vu-meters.md)
- Auto-sélectionner les périphériques système par défaut

### Traitement avancé

- Visualisation audio en temps réel avec analyse de spectre et de forme d'onde
- Réduction du bruit et annulation d'écho
- Contrôle de gain et normalisation
- Détection d'activité vocale (VAD)
- Gestion des canaux stéréo/mono
- Conversion de fréquence d'échantillonnage

### Contrôles d'enregistrement

- Démarrer, mettre en pause, reprendre et arrêter l'enregistrement
- Gestion du tampon pour un fonctionnement à faible latence
- Enregistrements minutés avec arrêt automatique
- Division de fichiers pour les enregistrements volumineux
- Nommage automatique des fichiers avec horodatage

## Notes multiplateformes

| Plateforme | Microphone | Audio système (loopback) |
| -------- | ---------- | ----------------------- |
| Windows  | DirectSound, WASAPI2 | loopback WASAPI2 |
| macOS    | CoreAudio | Non disponible via le SDK |
| Linux    | PulseAudio, ALSA | moniteur PulseAudio |

La capture loopback de l'audio système est principalement une fonctionnalité Windows utilisant l'API WASAPI2. Sous Linux, les périphériques moniteur PulseAudio peuvent offrir une fonctionnalité similaire.

## Bonnes pratiques

1. **Vérifiez la disponibilité du périphérique** avant de démarrer la capture — les périphériques peuvent être déconnectés à tout moment
2. **Surveillez les niveaux audio** pendant l'enregistrement pour détecter le silence ou la saturation
3. **Choisissez le bon format** — MP3/M4A pour une sortie compressée, WAV pour une qualité maximale, FLAC pour une compression sans perte
4. **Utilisez WASAPI2** pour la capture loopback sous Windows — il offre la latence la plus faible et la capture audio système la plus fiable
5. **Gérez les erreurs avec soin** — implémentez la gestion des erreurs pour les événements de déconnexion de périphérique
6. **Testez sur le matériel cible** — le comportement des périphériques audio varie selon les systèmes

## Exemples d'applications

Des exemples complets et fonctionnels sont disponibles sur GitHub :

- [Capture haut-parleur — Media Blocks SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_CodeSnippets/speaker-capture)
- [Capture haut-parleur — Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_CodeSnippets/speaker-capture)
- [Démo de capture audio — Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)

## Documentation associée

- [Sources audio — configuration des périphériques](../audio-sources/index.md)
- [Rendu audio — lecture](../audio-rendering/index.md)
- [Enregistrement et édition WMA](../../general/guides/wma-recording-editing.md)
- [Capture d'écran avec audio](../video-tutorials/screen-capture-mp4.md)

## Transcription de la vidéo

Enregistrez un microphone dans un fichier MP3, en quelques lignes de C#. Chargez le SDK et créez le moteur de capture. Listez les entrées audio et prenez celle nommée sur la ligne de commande, ou le microphone par défaut. Audio seul, sans lecture, enregistré dans un fichier MP3. Démarrez, attendez Entrée et arrêtez : `StopAsync` termine le fichier. Ici, il enregistre les haut-parleurs de la machine virtuelle, qui jouent un extrait de voix synthétique, pas un microphone en direct. `ffprobe` lit du MP3 à 48 kHz, stéréo, 192 kb/s. Le guide ajoute le son du système, d'autres formats et périphériques.
