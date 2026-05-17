---
title: Sortie de fichier AVI pour l'encodage vidéo et audio .NET
description: Implémentez la sortie de fichier AVI en .NET avec encodage vidéo et audio, accélération matérielle et prise en charge multiplateforme.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Recording
  - Encoding
  - Editing
  - AVI
  - H.264
  - H.265
  - MJPEG
  - AAC
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - AVIOutput
  - VideoCaptureCoreX
  - VOAACEncoderSettings
  - VideoEditCoreX
  - NVENCH264EncoderSettings

---

# Sortie de fichier AVI dans les SDK VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

L'AVI (Audio Video Interleave) est un format de conteneur multimédia développé par Microsoft qui stocke à la fois les données audio et vidéo dans un seul fichier avec une lecture synchronisée. Il prend en charge à la fois les données compressées et non compressées, offrant une flexibilité tout en aboutissant parfois à des tailles de fichier plus importantes.

## Présentation technique du format AVI

Les fichiers AVI utilisent une structure RIFF (Resource Interchange File Format) pour organiser les données. Ce format divise le contenu en blocs, chaque bloc contenant soit des trames audio, soit des images vidéo. Les aspects techniques clés incluent :

- Format de conteneur prenant en charge plusieurs codecs vidéo et audio
- Données audio et vidéo entrelacées pour une lecture synchronisée
- Taille de fichier maximale de 4 Go en AVI standard (étendue à 16 Eo en OpenDML AVI)
- Prise en charge de plusieurs pistes audio et de sous-titres
- Largement pris en charge sur les plateformes et les lecteurs multimédias

Malgré les formats de conteneur plus récents comme MP4 et MKV qui offrent davantage de fonctionnalités, l'AVI reste précieux pour certains flux de travail en raison de sa simplicité et de sa compatibilité avec les systèmes historiques.

## Implémentation AVI multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La classe [AVIOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.AVIOutput.html) fournit un moyen robuste de configurer et de générer des fichiers AVI avec diverses options d'encodage.

### Configuration de la sortie AVI

Créez une instance `AVIOutput` en spécifiant un nom de fichier cible :

```csharp
var aviOutput = new AVIOutput("output_video.avi");
```

Ce constructeur initialise automatiquement les encodeurs par défaut :

- Vidéo : encodeur OpenH264
- Audio : encodeur MP3

### Options d'encodeur vidéo

Configurez l'encodage vidéo via la propriété `Video` avec plusieurs encodeurs disponibles. Pour des options de configuration détaillées, consultez la [documentation de l'encodeur H.264](../video-encoders/h264.md), la [documentation de l'encodeur HEVC](../video-encoders/hevc.md) et la [documentation de l'encodeur MJPEG](../video-encoders/mjpeg.md) :

#### Encodeur standard

```csharp
// Encodeur H.264 open source pour usage général
aviOutput.Video = new OpenH264EncoderSettings();
```

#### Encodeurs accélérés matériellement

```csharp
// Accélération GPU NVIDIA
aviOutput.Video = new NVENCH264EncoderSettings();  // H.264
aviOutput.Video = new NVENCHEVCEncoderSettings(); // HEVC

// Accélération Intel Quick Sync
aviOutput.Video = new QSVH264EncoderSettings();   // H.264
aviOutput.Video = new QSVHEVCEncoderSettings();   // HEVC

// Accélération GPU AMD
aviOutput.Video = new AMFH264EncoderSettings();   // H.264
aviOutput.Video = new AMFHEVCEncoderSettings();   // HEVC
```

#### Encodeur à usage particulier

```csharp
// Motion JPEG pour un encodage image par image de haute qualité
aviOutput.Video = new MJPEGEncoderSettings();
```

### Options d'encodeur audio

La propriété `Audio` permet de configurer les paramètres d'encodage audio. Pour des options de configuration détaillées, consultez la [documentation de l'encodeur MP3](../audio-encoders/mp3.md) et la [documentation de l'encodeur AAC](../audio-encoders/aac.md) :

```csharp
// Encodage MP3 standard
aviOutput.Audio = new MP3EncoderSettings();

// Options d'encodage AAC
aviOutput.Audio = new VOAACEncoderSettings();
aviOutput.Audio = new AVENCAACEncoderSettings();
aviOutput.Audio = new MFAACEncoderSettings(); // Windows uniquement
```

### Intégration avec les composants du SDK

#### Video Capture SDK

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(aviOutput, true);
```

#### Video Edit SDK

```csharp
var core = new VideoEditCoreX();
core.Output_Format = aviOutput;
```

#### Media Blocks SDK

```csharp
var aac = new VOAACEncoderSettings();
var h264 = new OpenH264EncoderSettings();
var aviSinkSettings = new AVISinkSettings("output.avi");
var aviOutput = new AVIOutputBlock(aviSinkSettings, h264, aac);
```

### Gestion des fichiers

Vous pouvez obtenir ou modifier le nom du fichier de sortie après initialisation :

```csharp
// Obtenir le nom de fichier actuel
string currentFile = aviOutput.GetFilename();

// Définir un nouveau nom de fichier
aviOutput.SetFilename("new_output.avi");
```

### Exemple complet

Voici un exemple complet montrant comment configurer une sortie AVI avec accélération matérielle :

```csharp
// Créer la sortie AVI avec le nom de fichier spécifié
var aviOutput = new AVIOutput("high_quality_output.avi");

// Configurer l'encodage H.264 NVIDIA accéléré matériellement
aviOutput.Video = new NVENCH264EncoderSettings();

// Configurer l'encodage audio AAC
aviOutput.Audio = new VOAACEncoderSettings();
```

## Implémentation AVI spécifique à Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Les composants réservés à Windows offrent des options supplémentaires pour la configuration de sortie AVI.

### Configuration de base

Créez l'objet AVIOutput :

```csharp
var aviOutput = new AVIOutput();
```

### Méthodes de configuration

#### Méthode 1 : Utilisation de la boîte de dialogue des paramètres

```csharp
var aviSettingsDialog = new AVISettingsDialog(
  VideoCapture1.Video_Codecs.ToArray(),
  VideoCapture1.Audio_Codecs.ToArray());

aviSettingsDialog.ShowDialog(this);
aviSettingsDialog.SaveSettings(ref aviOutput);
```

#### Méthode 2 : Configuration par programmation

Commencez par obtenir les codecs disponibles :

```csharp
// Renseigner les listes de codecs
foreach (string codec in VideoCapture1.Video_Codecs)
{
  cbVideoCodecs.Items.Add(codec);
}

foreach (string codec in VideoCapture1.Audio_Codecs)
{
  cbAudioCodecs.Items.Add(codec);
}
```

Définissez ensuite les paramètres vidéo et audio :

```csharp
// Configurer la vidéo
aviOutput.Video_Codec = cbVideoCodecs.Text;

// Configurer l'audio
aviOutput.ACM.Name = cbAudioCodecs.Text;
aviOutput.ACM.Channels = 2;
aviOutput.ACM.BPS = 16;
aviOutput.ACM.SampleRate = 44100;
aviOutput.ACM.UseCompression = true;
```

### Implémentation

Appliquez les paramètres et démarrez la capture :

```csharp
// Définir le format de sortie
VideoCapture1.Output_Format = aviOutput;

// Définir le mode de capture
VideoCapture1.Mode = VideoCaptureMode.VideoCapture;

// Définir le chemin du fichier de sortie
VideoCapture1.Output_Filename = "output.avi";

// Démarrer la capture
await VideoCapture1.StartAsync();
```

## Bonnes pratiques pour la sortie AVI

### Recommandations de sélection d'encodeur

1. **Applications à usage général**
   - OpenH264 offre une bonne compatibilité et qualité
   - Adapté à la plupart des scénarios de développement standard

2. **Applications critiques en termes de performance**
   - Utilisez les encodeurs accélérés matériellement (NVENC, QSV, AMF) lorsqu'ils sont disponibles
   - Offre des avantages de performance significatifs avec une perte de qualité minimale

3. **Applications axées sur la qualité**
   - Les encodeurs HEVC offrent une meilleure compression à qualité similaire
   - MJPEG pour les scénarios nécessitant une précision image par image

### Recommandations pour l'encodage audio

- MP3 : Bonne compatibilité avec une qualité raisonnable
- AAC : Meilleur rapport qualité/taille, préféré pour les applications plus récentes
- Choisissez en fonction de votre plateforme cible et de vos exigences de qualité

### Considérations de plateforme

- Certains encodeurs sont spécifiques à une plateforme :
  - Les encodeurs MF HEVC et MF AAC sont réservés à Windows
  - Les encodeurs accélérés matériellement nécessitent une prise en charge GPU appropriée

- Vérifiez la disponibilité des encodeurs avec `GetVideoEncoders()` et `GetAudioEncoders()` lors du développement d'applications multiplateformes

### Conseils de gestion des erreurs

- Vérifiez toujours la disponibilité de l'encodeur avant utilisation
- Implémentez des encodeurs de repli pour les scénarios spécifiques à une plateforme
- Vérifiez les autorisations d'écriture des fichiers avant de définir les chemins de sortie

## Dépannage des problèmes courants

### Codec introuvable

Si vous rencontrez des erreurs « Codec introuvable » :

```csharp
// Vérifier si le codec est disponible avant utilisation
if (!VideoCapture1.Video_Codecs.Contains("H264"))
{
    // Se replier sur un autre codec ou afficher une erreur
    MessageBox.Show("H264 codec not available. Please install required codecs.");
    return;
}
```

### Problèmes d'autorisation d'écriture de fichier

Gérez les erreurs liées aux autorisations :

```csharp
try
{
    // Tester les autorisations d'écriture
    using (var fs = File.Create(outputPath, 1, FileOptions.DeleteOnClose)) { }
    
    // En cas de succès, procéder avec la sortie AVI
    aviOutput.SetFilename(outputPath);
}
catch (UnauthorizedAccessException)
{
    // Gérer l'erreur d'autorisation
    MessageBox.Show("Cannot write to the specified location. Please select another folder.");
}
```

### Limite de 4 Go du conteneur AVI

Le conteneur AVI a un plafond strict de 4 Go, et `AVIOutput` n'expose pas d'API de découpe intégrée. Pour les longs enregistrements, vous pouvez :

- Arrêter le pipeline avant d'approcher 4 Go et en démarrer un nouveau avec un `AVIOutput(nextFilename)` neuf, ou
- Changer de conteneur : [MP4](mp4.md) et [MKV](mkv.md) gèrent tous deux des enregistrements de plusieurs heures sans cette limite.

Surveillez l'espace disque libre et effectuez une rotation au niveau de l'application lorsque vous avez besoin d'une capture multi-fichiers.

## Dépendances requises

### Video Capture SDK .Net

- [x86 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- [x64 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Video Edit SDK .Net

- [x86 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
- [x64 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Ressources supplémentaires

- [Documentation de l'API VisioForge](https://api.visioforge.org/dotnet/)
- [Dépôt de projets d'exemple](https://github.com/visioforge/.Net-SDK-s-samples)
- [Forums de support et communauté](https://support.visioforge.com/)
