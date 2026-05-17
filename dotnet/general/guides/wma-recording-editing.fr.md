---
title: Enregistrement et édition audio WMA dans le SDK .NET avec C#
description: Enregistrez et éditez de l'audio WMA en .NET avec VideoCaptureCoreX et VideoEditCoreX pour la capture et l'édition Windows Media Audio.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - WinForms
  - GStreamer
  - Capture
  - Encoding
  - Editing
  - Effects
  - MP3
  - WMA
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoCaptureCoreX
  - WMAOutput
  - WMAEncoderSettings
  - AudioFileSource

---

# Enregistrer et éditer des fichiers WMA en C# et .NET : un guide complet

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à l'enregistrement et à l'édition WMA en .NET

Cet article fournit un guide complet pour les développeurs travaillant avec des fichiers Windows Media Audio (WMA) dans des applications .NET. Nous explorerons comment enregistrer de l'audio WMA en .NET depuis des microphones et d'autres périphériques de capture en utilisant la classe VideoCaptureCoreX, et comment éditer des fichiers WMA en dotnet en utilisant la classe VideoEditCoreX des SDK VisioForge .NET.

Windows Media Audio est un format audio populaire développé par Microsoft qui offre une excellente compression tout en maintenant une bonne qualité audio. Le format WMA est largement utilisé dans les applications Windows et prend en charge divers débits binaires et fréquences d'échantillonnage, ce qui le rend adapté à la fois aux enregistrements vocaux et à la musique de haute qualité.

La bibliothèque VisioForge fournit des classes puissantes pour capturer des données audio depuis les périphériques système et traiter du contenu audio vidéo. Que vous ayez besoin de créer une simple application console d'enregistrement vocal ou un éditeur audio WinForms complexe, ces SDK offrent les fonctionnalités dont vous avez besoin. Ce guide vous montrera comment capturer de l'audio WMA en dotnet et enregistrer des fichiers WMA en csharp avec facilité.

## Prérequis et installation

Avant de commencer à enregistrer ou à éditer des fichiers WMA dans votre application dotnet, assurez-vous d'avoir les éléments suivants :

- Visual Studio 2019 ou version ultérieure
- .NET 6.0 ou version ultérieure (ou .NET Framework 4.7.2+)
- VisioForge Video Capture SDK .NET ou Video Edit SDK .NET

### Installation des paquets NuGet

Installez les paquets requis à l'aide du Gestionnaire de paquets NuGet :

```bash
# Pour l'enregistrement WMA avec VideoCaptureCoreX
Install-Package VisioForge.DotNet.VideoCapture

# Pour l'édition WMA avec VideoEditCoreX
Install-Package VisioForge.DotNet.VideoEdit
```

Pour des instructions d'installation détaillées, veuillez consulter le [guide d'installation](../../install/index.md).

## Enregistrer des fichiers WMA depuis le microphone avec VideoCaptureCoreX

La classe VideoCaptureCoreX fournit une approche simple pour capturer de l'audio WMA en csharp depuis des microphones et d'autres périphériques d'entrée audio. Cette section montre comment enregistrer des fichiers audio WMA en csharp avec une énumération de périphériques et une configuration d'encodeur appropriées. Apprenez à capturer du contenu WMA en csharp pour divers scénarios d'application.

### Composants principaux pour l'enregistrement WMA

1. **VideoCaptureCoreX** : classe moteur principal pour gérer la capture audio et la sortie WMA en .NET.
2. **DeviceEnumerator** : classe pour découvrir les périphériques de capture audio disponibles sur le système.
3. **AudioCaptureDeviceSourceSettings** : paramètres de configuration pour un microphone ou un périphérique d'entrée audio.
4. **WMAOutput** : configuration du format de sortie spécifiquement pour la création de fichiers Windows Media Audio.
5. **WMAEncoderSettings** : classe de paramètres pour les paramètres de l'encodeur WMA comme le débit binaire et la fréquence d'échantillonnage.

### Implémentation basique de l'enregistrement WMA

Voici une implémentation csharp complète pour capturer et enregistrer des fichiers WMA depuis un microphone :

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public class WmaRecorder
{
    private VideoCaptureCoreX _videoCapture;

    // Appeler cette méthode une fois au démarrage de l'application ou au chargement du formulaire
    public async Task InitializeAsync()
    {
        // Initialiser le SDK VisioForge
        await VisioForgeX.InitSDKAsync();
    }

    public async Task StartRecordingAsync(string outputPath)
    {
        // Créer une instance VideoCaptureCoreX pour la capture audio
        _videoCapture = new VideoCaptureCoreX();

        // Configurer le périphérique de capture audio (microphone)
        await ConfigureAudioSourceAsync();

        // Désactiver la capture vidéo — nous n'avons besoin que de l'audio
        _videoCapture.Video_Source = null;
        _videoCapture.Video_Play = false;

        // Configurer les paramètres audio
        _videoCapture.Audio_Play = false;    // Désactiver la surveillance audio pendant l'enregistrement
        _videoCapture.Audio_Record = true;   // Activer l'enregistrement audio vers le fichier

        // Configurer le format de sortie WMA
        var wmaOutput = new WMAOutput(outputPath);
        
        // Configurer les paramètres de l'encodeur WMA
        wmaOutput.Audio.Bitrate = 192;       // Débit binaire de 192 Kbps
        wmaOutput.Audio.SampleRate = 48000;  // Fréquence d'échantillonnage de 48 kHz
        wmaOutput.Audio.Channels = 2;        // Enregistrement stéréo

        // Ajouter la sortie WMA au pipeline de capture
        _videoCapture.Outputs_Add(wmaOutput, autostart: true);

        // Démarrer le processus de capture audio
        await _videoCapture.StartAsync();

        Console.WriteLine("Recording started. Press any key to stop...");
    }

    private async Task ConfigureAudioSourceAsync()
    {
        // Obtenir les périphériques de capture audio disponibles via l'API DirectSound
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);

        if (audioDevices.Length == 0)
        {
            throw new Exception("No audio capture device found.");
        }

        // Obtenir le premier périphérique de capture audio disponible (généralement le microphone par défaut)
        var audioDevice = audioDevices[0];

        // Obtenir le format pris en charge depuis le périphérique
        var audioFormat = audioDevice.GetDefaultFormat();

        // Créer les paramètres de la source audio avec le périphérique et le format sélectionnés
        var audioSourceSettings = audioDevice.CreateSourceSettingsVC(audioFormat);

        // Configurer le périphérique de capture audio
        _videoCapture.Audio_Source = audioSourceSettings;
    }

    public async Task StopRecordingAsync()
    {
        if (_videoCapture != null)
        {
            // Arrêter toute la capture et l'encodage
            await _videoCapture.StopAsync();

            // Nettoyer les ressources
            await _videoCapture.DisposeAsync();
            _videoCapture = null;

            Console.WriteLine("Recording stopped and file saved.");
        }
    }
}
```

### Exemple d'application console pour l'enregistrement WMA

Voici une application console complète qui enregistre de l'audio WMA depuis un microphone :

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
        Console.WriteLine("WMA Audio Recorder - Console Application");
        Console.WriteLine("========================================");

        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();

        // Créer l'instance de capture
        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Énumérer et afficher les périphériques de capture audio disponibles
            var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
                AudioCaptureDeviceAPI.DirectSound);

            Console.WriteLine("\nAvailable audio capture devices:");
            for (int i = 0; i < audioDevices.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioDevices[i].DisplayName}");
            }

            if (audioDevices.Length == 0)
            {
                Console.WriteLine("No audio capture device found. Exiting.");
                return;
            }

            // Sélectionner le premier périphérique pour l'enregistrement
            var selectedDevice = audioDevices[0];
            var audioFormat = selectedDevice.GetDefaultFormat();
            var audioSourceSettings = selectedDevice.CreateSourceSettingsVC(audioFormat);

            // Configurer la capture vidéo pour un enregistrement audio uniquement
            videoCapture.Audio_Source = audioSourceSettings;
            videoCapture.Video_Source = null;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;

            // Configurer le fichier de sortie WMA
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wma");

            var wmaOutput = new WMAOutput(outputPath);
            wmaOutput.Audio.Bitrate = 192;
            wmaOutput.Audio.SampleRate = 48000;
            wmaOutput.Audio.Channels = 2;

            videoCapture.Outputs_Add(wmaOutput, autostart: true);

            // Démarrer l'enregistrement
            Console.WriteLine($"\nRecording to: {outputPath}");
            Console.WriteLine("Press ENTER to stop recording...\n");

            await videoCapture.StartAsync();

            // Attendre l'entrée utilisateur pour arrêter
            Console.ReadLine();

            // Arrêter l'enregistrement
            await videoCapture.StopAsync();
            Console.WriteLine($"\nRecording saved to: {outputPath}");
        }
        finally
        {
            // Nettoyage
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Application WinForms pour l'enregistrement WMA

Pour une application Windows Forms avec contrôles visuels, voici un exemple d'implémentation :

```csharp
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public partial class WmaRecorderForm : Form
{
    private VideoCaptureCoreX _videoCapture;
    private bool _isRecording = false;

    public WmaRecorderForm()
    {
        InitializeComponent();
    }

    private async void Form_Load(object sender, EventArgs e)
    {
        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();

        // Remplir la liste déroulante des périphériques audio
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);

        foreach (var device in audioDevices)
        {
            cmbAudioDevices.Items.Add(device.DisplayName);
        }

        if (cmbAudioDevices.Items.Count > 0)
        {
            cmbAudioDevices.SelectedIndex = 0;
        }

        // Définir le chemin de sortie par défaut
        txtOutputPath.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            "recording.wma");
    }

    private async void btnStartStop_Click(object sender, EventArgs e)
    {
        if (!_isRecording)
        {
            await StartRecordingAsync();
        }
        else
        {
            await StopRecordingAsync();
        }
    }

    private async Task StartRecordingAsync()
    {
        _videoCapture = new VideoCaptureCoreX();

        // Obtenir le périphérique audio sélectionné
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);
        var selectedDevice = audioDevices.FirstOrDefault(
            d => d.DisplayName == cmbAudioDevices.SelectedItem.ToString());

        if (selectedDevice == null)
        {
            MessageBox.Show("Please select an audio device.");
            return;
        }

        // Configurer la source audio
        var audioFormat = selectedDevice.GetDefaultFormat();
        var audioSourceSettings = selectedDevice.CreateSourceSettingsVC(audioFormat);
        _videoCapture.Audio_Source = audioSourceSettings;

        // Configurer pour une capture audio uniquement
        _videoCapture.Video_Source = null;
        _videoCapture.Video_Play = false;
        _videoCapture.Audio_Play = false;
        _videoCapture.Audio_Record = true;

        // Configurer la sortie WMA
        var wmaOutput = new WMAOutput(txtOutputPath.Text);
        wmaOutput.Audio.Bitrate = (int)numBitrate.Value;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = rbStereo.Checked ? 2 : 1;

        _videoCapture.Outputs_Add(wmaOutput, autostart: true);

        // Démarrer l'enregistrement
        await _videoCapture.StartAsync();

        _isRecording = true;
        btnStartStop.Text = "Stop Recording";
        lblStatus.Text = "Recording...";
    }

    private async Task StopRecordingAsync()
    {
        if (_videoCapture != null)
        {
            await _videoCapture.StopAsync();
            await _videoCapture.DisposeAsync();
            _videoCapture = null;
        }

        _isRecording = false;
        btnStartStop.Text = "Start Recording";
        lblStatus.Text = "Recording saved.";
    }

    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_isRecording)
        {
            StopRecordingAsync().Wait();
        }

        VisioForgeX.DestroySDK();
    }
}
```

### Paramètres avancés de capture audio

La classe VideoCaptureCoreX prend en charge diverses configurations de capture audio pour différents scénarios d'enregistrement :

```csharp
// Configurer un enregistrement WMA de haute qualité
var wmaOutput = new WMAOutput("high_quality.wma");
wmaOutput.Audio.Bitrate = 320;       // Débit binaire de qualité maximale
wmaOutput.Audio.SampleRate = 48000;  // Fréquence d'échantillonnage professionnelle
wmaOutput.Audio.Channels = 2;        // Stéréo

// Configurer l'enregistrement vocal (taille de fichier réduite)
var voiceOutput = new WMAOutput("voice_memo.wma");
voiceOutput.Audio.Bitrate = 128;     // Bon pour la voix
voiceOutput.Audio.SampleRate = 44100; // Fréquence d'échantillonnage standard
voiceOutput.Audio.Channels = 1;       // Le mono est suffisant pour la voix

// Vérifier si l'encodeur WMA est disponible sur le système
if (!WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMA encoder is not available on this system.");
    // Envisager de basculer vers MP3 ou un autre format
}
```

## Éditer des fichiers WMA avec VideoEditCoreX

La classe VideoEditCoreX offre des capacités puissantes pour éditer des fichiers WMA et convertir du contenu audio vidéo au format Windows Media. Cette section montre comment éditer des fichiers WMA en dotnet avec du rognage, de la fusion et de la conversion de format.

### Composants principaux pour l'édition WMA

1. **VideoEditCoreX** : classe moteur principal pour gérer les opérations d'édition audio et vidéo.
2. **AudioFileSource** : classe pour ajouter des sources de fichier audio à la timeline d'édition.
3. **WMAOutput** : configuration du format de sortie pour l'export Windows Media Audio.
4. **Audio_Effects** : collection pour appliquer des effets audio pendant l'édition.

### Édition basique de fichier WMA

Voici comment éditer des fichiers WMA en utilisant la classe VideoEditCoreX :

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

public class WmaEditor
{
    private VideoEditCoreX _videoEdit;

    // Appeler cette méthode une fois au démarrage de l'application ou au chargement du formulaire
    public async Task InitializeAsync()
    {
        // Initialiser le SDK VisioForge
        await VisioForgeX.InitSDKAsync();
    }

    public async Task EditWmaFileAsync(
        string inputPath, 
        string outputPath,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null)
    {
        // Créer l'instance VideoEditCoreX
        _videoEdit = new VideoEditCoreX();

        // Configurer les gestionnaires d'événements
        _videoEdit.OnProgress += VideoEdit_OnProgress;
        _videoEdit.OnStop += VideoEdit_OnStop;
        _videoEdit.OnError += VideoEdit_OnError;

        // Ajouter le fichier WMA d'entrée avec rognage optionnel
        var audioFile = new AudioFileSource(
            inputPath,
            startTime,  // Heure de début pour le rognage (null pour le début)
            endTime);   // Heure de fin pour le rognage (null pour la fin)

        _videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configurer le format de sortie WMA
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        _videoEdit.Output_Format = wmaOutput;

        // Démarrer le processus d'édition
        _videoEdit.Start();

        Console.WriteLine("Editing in progress...");
    }

    private void VideoEdit_OnProgress(object sender, ProgressEventArgs e)
    {
        Console.WriteLine($"Progress: {e.Progress}%");
    }

    private void VideoEdit_OnStop(object sender, StopEventArgs e)
    {
        if (e.Successful)
        {
            Console.WriteLine("Editing completed successfully!");
        }
        else
        {
            Console.WriteLine("Editing stopped with errors.");
        }

        // Nettoyage
        _videoEdit?.Dispose();
        _videoEdit = null;
    }

    private void VideoEdit_OnError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
}
```

### Fusionner plusieurs fichiers WMA

La classe VideoEditCoreX vous permet de fusionner plusieurs fichiers audio en une seule sortie WMA :

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

public class WmaMerger
{
    // Appeler cette méthode une fois au démarrage de l'application ou au chargement du formulaire
    public async Task InitializeAsync()
    {
        // Initialiser le SDK
        await VisioForgeX.InitSDKAsync();
    }

    public async Task MergeWmaFilesAsync(
        List<string> inputFiles, 
        string outputPath)
    {
        var videoEdit = new VideoEditCoreX();

        try
        {
            // Configurer le suivi de progression
            videoEdit.OnProgress += (s, e) => 
                Console.WriteLine($"Merging progress: {e.Progress}%");

            videoEdit.OnError += (s, e) => 
                Console.WriteLine($"Error: {e.Message}");

            // Ajouter chaque fichier d'entrée séquentiellement
            foreach (var inputFile in inputFiles)
            {
                var audioFile = new AudioFileSource(inputFile);
                
                // L'ajout avec insertTime null l'ajoute à la fin de la timeline
                videoEdit.Input_AddAudioFile(audioFile, insertTime: null);
                
                Console.WriteLine($"Added: {inputFile}");
            }

            // Configurer le format de sortie
            var wmaOutput = new WMAOutput(outputPath);
            wmaOutput.Audio.Bitrate = 192;
            wmaOutput.Audio.SampleRate = 48000;
            wmaOutput.Audio.Channels = 2;

            videoEdit.Output_Format = wmaOutput;

            // Créer un signal d'achèvement
            var completionSource = new TaskCompletionSource<bool>();
            videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

            // Démarrer la fusion
            videoEdit.Start();

            // Attendre la fin
            bool success = await completionSource.Task;

            if (success)
            {
                Console.WriteLine($"Files merged successfully to: {outputPath}");
            }
            else
            {
                Console.WriteLine("Merge operation failed.");
            }
        }
        finally
        {
            videoEdit.Dispose();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Rogner des fichiers WMA

Extraire une partie spécifique d'un fichier WMA :

```csharp
// Note : appelez VisioForgeX.InitSDKAsync() une fois au démarrage de l'application ou au chargement du formulaire
public async Task TrimWmaFileAsync(
    string inputPath,
    string outputPath,
    TimeSpan startTime,
    TimeSpan endTime)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Ajouter le fichier d'entrée avec des heures de début et de fin spécifiques
        var audioFile = new AudioFileSource(
            inputPath,
            startTime,   // par ex. TimeSpan.FromSeconds(10)
            endTime);    // par ex. TimeSpan.FromSeconds(60)

        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configurer la sortie WMA
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        // Créer un signal d'achèvement
        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

        // Démarrer le rognage
        videoEdit.Start();

        // Attendre la fin
        bool success = await completionSource.Task;

        Console.WriteLine(success 
            ? "Trim completed successfully!" 
            : "Trim operation failed.");
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

### Convertir des fichiers vidéo en audio WMA

Extraire l'audio des fichiers vidéo et l'enregistrer en WMA :

```csharp
// Note : appelez VisioForgeX.InitSDKAsync() une fois au démarrage de l'application ou au chargement du formulaire
public async Task ExtractAudioToWmaAsync(
    string videoInputPath,
    string wmaOutputPath)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Ajouter le fichier vidéo — l'audio sera extrait automatiquement
        var audioFile = new AudioFileSource(videoInputPath);
        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configurer la sortie WMA pour l'extraction audio
        var wmaOutput = new WMAOutput(wmaOutputPath);
        wmaOutput.Audio.Bitrate = 256;       // Qualité supérieure pour la musique
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnProgress += (s, e) => 
            Console.WriteLine($"Extraction progress: {e.Progress}%");
        videoEdit.OnStop += (s, e) => 
            completionSource.SetResult(e.Successful);

        videoEdit.Start();

        bool success = await completionSource.Task;

        Console.WriteLine(success 
            ? $"Audio extracted to: {wmaOutputPath}" 
            : "Audio extraction failed.");
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

### Appliquer des effets audio pendant l'édition WMA

La classe VideoEditCoreX prend en charge divers effets audio qui peuvent être appliqués pendant le processus d'édition :

```csharp
using VisioForge.Core.Types.X.AudioEffects;

// Note : appelez VisioForgeX.InitSDKAsync() une fois au démarrage de l'application ou au chargement du formulaire
public async Task EditWmaWithEffectsAsync(
    string inputPath,
    string outputPath)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Ajouter le fichier d'entrée
        var audioFile = new AudioFileSource(inputPath);
        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Appliquer les effets audio

        // Effet d'amplification de volume
        var amplifyEffect = new AmplifyAudioEffect(1.5); // Volume à 150 %
        videoEdit.Audio_Effects.Add(amplifyEffect);

        // Effet égaliseur 10 bandes
        var eqLevels = new double[] 
        { 
            3.0,   // 60 Hz
            2.0,   // 170 Hz
            1.0,   // 310 Hz
            0.0,   // 600 Hz
            0.0,   // 1 kHz
            0.0,   // 3 kHz
            1.0,   // 6 kHz
            2.0,   // 12 kHz
            2.0,   // 14 kHz
            3.0    // 16 kHz
        };
        var equalizerEffect = new Equalizer10AudioEffect(eqLevels);
        videoEdit.Audio_Effects.Add(equalizerEffect);

        // Configurer la sortie WMA
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

        videoEdit.Start();

        await completionSource.Task;
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

## Configuration de l'encodeur WMA

La classe WMAEncoderSettings fournit diverses options de configuration pour l'encodeur Windows Media Audio :

### Paramètres disponibles

```csharp
var wmaSettings = new WMAEncoderSettings
{
    // Débit binaire en Kbps — valeurs prises en charge : 128, 192, 256, 320
    Bitrate = 192,
    
    // Fréquence d'échantillonnage en Hz — valeurs prises en charge : 44100, 48000
    SampleRate = 48000,
    
    // Nombre de canaux — valeurs prises en charge : 1 (mono), 2 (stéréo)
    Channels = 2
};

// Interroger les configurations prises en charge
int[] supportedBitrates = wmaSettings.GetSupportedBitrates();
// Retourne : [128, 192, 256, 320]

int[] supportedSampleRates = wmaSettings.GetSupportedSampleRates();
// Retourne : [44100, 48000]

int[] supportedChannels = wmaSettings.GetSupportedChannelCounts();
// Retourne : [1, 2]
```

### Préréglages de qualité

Voici les préréglages recommandés pour différents cas d'usage :

```csharp
// Enregistrement musique haute qualité
var musicPreset = new WMAEncoderSettings
{
    Bitrate = 320,
    SampleRate = 48000,
    Channels = 2
};

// Enregistrement vocal / podcasts
var voicePreset = new WMAEncoderSettings
{
    Bitrate = 128,
    SampleRate = 44100,
    Channels = 1
};

// Qualité et taille de fichier équilibrées
var balancedPreset = new WMAEncoderSettings
{
    Bitrate = 192,
    SampleRate = 48000,
    Channels = 2
};
```

## Travailler avec les paquets et tampons audio

Pour des scénarios avancés, vous pouvez avoir besoin de travailler directement avec les paquets de données audio. Le SDK fournit des mécanismes pour accéder aux données audio brutes pendant la capture et le traitement.

### Traitement des paquets audio

```csharp
// Pendant la capture, vous pouvez surveiller les niveaux et paquets audio
_videoCapture.OnAudioVUMeter += (sender, args) =>
{
    // args est VUMeterXEventArgs — les niveaux par canal proviennent de MeterData (VUMeterXData)
    var meterData = args.MeterData;
    if (meterData == null || meterData.ChannelsCount == 0) return;

    double leftPeak  = meterData.Peak[0];
    double rightPeak = meterData.ChannelsCount > 1 ? meterData.Peak[1] : leftPeak;

    // Mettre à jour l'UI avec les niveaux audio
    UpdateVUMeter(leftPeak, rightPeak);
};
```

## Gestion des erreurs et bonnes pratiques

### Gestion correcte des ressources

Assurez-vous toujours du nettoyage approprié des ressources du SDK :

```csharp
public class WmaProcessor : IDisposable
{
    private VideoCaptureCoreX _videoCapture;
    private bool _disposed = false;

    public async Task InitializeAsync()
    {
        await VisioForgeX.InitSDKAsync();
        _videoCapture = new VideoCaptureCoreX();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _videoCapture?.Dispose();
                VisioForgeX.DestroySDK();
            }
            _disposed = true;
        }
    }
}
```

### Gestion des erreurs

Implémentez une gestion d'erreurs complète pour les applications de production :

```csharp
try
{
    await _videoCapture.StartAsync();
}
catch (Exception ex)
{
    // Journaliser l'erreur
    Console.WriteLine($"Failed to start recording: {ex.Message}");
    
    // Nettoyer les ressources
    await _videoCapture.DisposeAsync();
    
    // Notifier l'utilisateur ou réessayer
    throw;
}
```

### Vérifier la disponibilité de l'encodeur

Avant de créer des fichiers WMA, vérifiez que l'encodeur est disponible :

```csharp
if (!WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMA encoder is not available.");
    Console.WriteLine("Falling back to MP3 format...");
    
    // Utiliser MP3 comme alternative
    var mp3Output = new MP3Output("output.mp3");
    // ... continuer avec l'encodage MP3
}
```

## Considérations multiplateformes

Le format WMA et les composants Windows Media sont principalement conçus pour les systèmes Windows. Lors du développement d'applications multiplateformes :

- **Windows** : prise en charge WMA complète avec toutes les options d'encodage
- **Linux/macOS** : l'encodage WMA peut nécessiter des plugins GStreamer supplémentaires
- **Mobile (Android/iOS)** : envisagez d'utiliser des formats plus universels comme AAC ou MP3

Pour un enregistrement audio multiplateforme, envisagez ces alternatives :

```csharp
// Vérifier la plateforme et sélectionner le format approprié
string outputFormat = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? "output.wma"
    : "output.m4a";  // Conteneur AAC pour les systèmes non Windows

if (outputFormat.EndsWith(".wma"))
{
    var wmaOutput = new WMAOutput(outputFormat);
    _videoCapture.Outputs_Add(wmaOutput, true);
}
else
{
    var m4aOutput = new M4AOutput(outputFormat);
    _videoCapture.Outputs_Add(m4aOutput, true);
}
```

## Applications d'exemple et ressources

Explorez des applications d'exemple complètes démontrant l'enregistrement et l'édition WMA :

- [Exemples Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)
- [Exemples Video Edit SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X)

### Documentation supplémentaire

- [Guide de l'encodeur Windows Media Audio](../audio-encoders/wma.md)
- [Guide de la sortie Windows Media Video](../output-formats/wmv.md)
- [Audio Sample Grabber](../audio-effects/audio-sample-grabber.md)
- [Documentation de l'API](https://api.visioforge.org/dotnet/api/index.html)

## Conclusion

Ce guide complet a démontré comment enregistrer et éditer des fichiers WMA en utilisant les SDK VisioForge .NET. Vous avez appris à enregistrer de l'audio WMA en .NET, à capturer du contenu WMA en dotnet et à créer des applications audio professionnelles. La classe VideoCaptureCoreX fournit des capacités puissantes pour capturer de l'audio depuis des microphones et d'autres périphériques, tandis que la classe VideoEditCoreX offre des options flexibles pour éditer et convertir du contenu audio au format Windows Media.

Points clés :

- **Enregistrer des fichiers WMA** : utilisez VideoCaptureCoreX avec WMAOutput pour capturer de l'audio depuis les périphériques système et conservez des paramètres de qualité optimaux pour votre sortie
- **Éditer des fichiers WMA** : utilisez VideoEditCoreX pour rogner, fusionner et appliquer des effets aux enregistrements audio
- **Configuration** : la classe WMAEncoderSettings permet un ajustement fin du débit binaire, de la fréquence d'échantillonnage et des canaux
- **Multiplateforme** : tenez compte des exigences propres à chaque plateforme lorsque vous travaillez avec les formats Windows Media
- **Prise en charge des applications Windows** : créez facilement des applications WinForms, WPF et console

Les deux classes s'intègrent parfaitement aux applications WinForms, WPF et console, ce qui facilite la création de puissantes solutions d'enregistrement et d'édition audio dans vos applications .NET. Les capacités de traitement de données et les fonctionnalités de la bibliothèque vous permettent de créer des applications d'éditeur audio de qualité professionnelle.
