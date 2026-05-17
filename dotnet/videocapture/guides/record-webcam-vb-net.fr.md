---
title: Enregistrer une vidéo webcam en VB.NET — SDK VisioForge
description: Apprenez à enregistrer une vidéo webcam en VB.NET avec énumération, sortie MP4, aperçu en direct et capture d'instantanés via async/await.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Webcam
  - Screen Capture
  - MP4
  - WebM
  - AVI
  - C#
  - VB.NET
  - NuGet
primary_api_classes:
  - VideoView

---

# Enregistrer une vidéo webcam en VB.NET : guide complet

L'enregistrement vidéo webcam dans les applications VB.NET est une exigence courante pour la visioconférence, la vidéosurveillance et les projets multimédias. Ce guide fournit des instructions étape par étape pour capturer, prévisualiser et sauvegarder une vidéo webcam dans des fichiers MP4 avec [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) en VB.NET (Visual Basic .NET).

## Pourquoi utiliser Video Capture SDK .Net pour l'enregistrement webcam en VB.NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) offre une prise en charge VB.NET de premier ordre avec :

- Prise en charge complète de async/await pour une capture webcam non bloquante
- Énumération de périphériques pour sources vidéo, sources audio et sorties audio
- Enregistrement MP4 avec encodage H.264/H.265 et accélération GPU
- Aperçu webcam en temps réel pendant l'enregistrement
- Capture d'instantanés depuis le flux webcam en direct
- Prise en charge de WinForms et WPF

## Paquets NuGet requis

Ajoutez les paquets suivants à votre projet VB.NET :

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Exemple complet d'enregistrement webcam en VB.NET

L'exemple suivant illustre une application WinForms complète qui énumère les périphériques webcam, prévisualise le flux webcam et enregistre la vidéo vers MP4.

### Initialisation du SDK et énumération des périphériques

Tout d'abord, initialisez le SDK et énumérez les périphériques disponibles au chargement du formulaire :

```vb.net
Imports System.IO
Imports VisioForge.Core
Imports VisioForge.Core.VideoCaptureX
Imports VisioForge.Core.Types.X.Sources
Imports VisioForge.Core.Types.X.Output
Imports VisioForge.Core.Types.X.AudioRenderers

Public Class Form1
    Private VideoCapture1 As VideoCaptureCoreX

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialiser le SDK
        Await VisioForgeX.InitSDKAsync()

        ' Créer le moteur de capture vidéo
        VideoCapture1 = New VideoCaptureCoreX(VideoView1)
        AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError

        ' Énumérer les sources vidéo (webcams)
        Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
        For Each source In videoSources
            cbVideoInputDevice.Items.Add(source.DisplayName)
            If cbVideoInputDevice.Items.Count = 1 Then cbVideoInputDevice.SelectedIndex = 0
        Next

        ' Énumérer les sources audio (microphones)
        Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
        For Each source In audioSources
            cbAudioInputDevice.Items.Add(source.DisplayName)
            If cbAudioInputDevice.Items.Count = 1 Then cbAudioInputDevice.SelectedIndex = 0
        Next

        ' Énumérer les sorties audio (haut-parleurs)
        Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync()
        For Each audioOutput In audioOutputs
            cbAudioOutputDevice.Items.Add(audioOutput.DisplayName)
            If cbAudioOutputDevice.Items.Count = 1 Then cbAudioOutputDevice.SelectedIndex = 0
        Next

        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "capture.mp4")
    End Sub
```

### Lister les formats vidéo et fréquences d'images disponibles

Lorsque l'utilisateur sélectionne un périphérique webcam, remplissez les formats vidéo disponibles :

```vb.net
    Private Async Sub cbVideoInputDevice_SelectedIndexChanged(
            sender As Object, e As EventArgs) Handles cbVideoInputDevice.SelectedIndexChanged

        If cbVideoInputDevice.SelectedIndex = -1 Then Return

        cbVideoInputFormat.Items.Clear()
        cbVideoInputFrameRate.Items.Clear()

        Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
        Dim device = videoSources.FirstOrDefault(
            Function(d) d.DisplayName = cbVideoInputDevice.Text)

        If device IsNot Nothing Then
            For Each videoFormat In device.VideoFormats
                cbVideoInputFormat.Items.Add(videoFormat.Name)
            Next
            If cbVideoInputFormat.Items.Count > 0 Then cbVideoInputFormat.SelectedIndex = 0
        End If
    End Sub
```

### Démarrer l'enregistrement webcam vers MP4

Configurez les sources vidéo et audio, définissez la sortie MP4 et démarrez l'enregistrement :

```vb.net
    Private Async Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click
        Try
            ' Configurer la source vidéo
            Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
            Dim videoDevice = videoSources.FirstOrDefault(
                Function(d) d.DisplayName = cbVideoInputDevice.Text)

            If videoDevice IsNot Nothing Then
                Dim videoSourceSettings As New VideoCaptureDeviceSourceSettings(videoDevice)
                VideoCapture1.Video_Source = videoSourceSettings
            End If

            ' Configurer la source audio
            Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
            Dim audioDevice = audioSources.FirstOrDefault(
                Function(d) d.DisplayName = cbAudioInputDevice.Text)

            If audioDevice IsNot Nothing Then
                VideoCapture1.Audio_Source = audioDevice.CreateSourceSettingsVC(Nothing)
            End If

            ' Configurer la sortie audio pour la surveillance en direct
            Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync()
            Dim audioOutput = audioOutputs.FirstOrDefault(
                Function(d) d.DisplayName = cbAudioOutputDevice.Text)

            If audioOutput IsNot Nothing Then
                VideoCapture1.Audio_OutputDevice = New AudioRendererSettings(audioOutput)
            End If

            VideoCapture1.Audio_Play = True
            VideoCapture1.Audio_Record = True
            VideoCapture1.Video_Play = True

            ' Configurer la sortie MP4
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Démarrer l'enregistrement
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Arrêter l'enregistrement webcam

Arrêtez l'enregistrement et libérez les ressources :

```vb.net
    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        Try
            Await VideoCapture1.StopAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Enregistrer des instantanés depuis la webcam

Capturez des images fixes depuis le flux webcam en direct :

```vb.net
    Private Async Sub btSaveScreenshot_Click(sender As Object, e As EventArgs) Handles btSaveScreenshot.Click
        Dim saveDialog As New SaveFileDialog With {
            .Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp",
            .FileName = "snapshot.jpg"
        }

        If saveDialog.ShowDialog() = DialogResult.OK Then
            Dim format As SKEncodedImageFormat = SKEncodedImageFormat.Jpeg
            If saveDialog.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) Then
                format = SKEncodedImageFormat.Png
            End If

            Await VideoCapture1.Snapshot_SaveAsync(saveDialog.FileName, format)
        End If
    End Sub
```

### Nettoyage à la fermeture du formulaire

Libérez correctement les ressources à la sortie de l'application :

```vb.net
    Private Async Sub Form1_FormClosing(
            sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If VideoCapture1 IsNot Nothing Then
            Await VideoCapture1.DisposeAsync()
            VideoCapture1 = Nothing
        End If

        VisioForgeX.DestroySDK()
    End Sub

    Private Sub VideoCapture1_OnError(sender As Object, e As ErrorsEventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() mmLog.Text &= e.Message & Environment.NewLine)
        Else
            mmLog.Text &= e.Message & Environment.NewLine
        End If
    End Sub
End Class
```

## Mode aperçu uniquement (sans enregistrement)

Pour prévisualiser le flux webcam sans enregistrement, omettez simplement l'ajout de la sortie MP4 :

```vb.net
' Aucune sortie ajoutée — aperçu uniquement
VideoCapture1.Video_Play = True
VideoCapture1.Audio_Play = True

Await VideoCapture1.StartAsync()
```

## Options de format de sortie

Bien que ce guide se concentre sur l'enregistrement MP4, vous pouvez sauvegarder la vidéo webcam dans d'autres formats :

### Sortie WebM

```vb.net
VideoCapture1.Outputs_Add(New WebMOutput(edOutput.Text), True)
```

### Sortie AVI

```vb.net
VideoCapture1.Outputs_Add(New AVIOutput(edOutput.Text), True)
```

Pour une configuration détaillée des formats, consultez la [documentation du format MP4](../../general/output-formats/mp4.md) et la [documentation du format WebM](../../general/output-formats/webm.md).

## Applications d'exemple

Des applications d'exemple VB.NET complètes sont disponibles :

- [Simple Video Capture VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WinForms/VB/Simple%20Video%20Capture%20X%20VB) — application complète de capture webcam

## Foire aux questions

### Comment convertir des exemples C# de capture webcam en VB.NET ?

L'API Video Capture SDK .Net est identique en C# et VB.NET — seule la syntaxe du langage diffère. Conversions clés : remplacez les gestionnaires d'événements `+=` par `AddHandler`/`AddressOf`, remplacez les lambdas C# `(x) =>` par `Function(x)` ou `Sub(x)`, et utilisez `Async Sub` au lieu de `async void`. Toutes les classes, méthodes et propriétés affichées dans la documentation C# fonctionnent directement en VB.NET avec ces ajustements syntaxiques.

### VB.NET prend-il en charge l'enregistrement webcam asynchrone avec Await ?

Oui. VB.NET prend entièrement en charge `Async`/`Await` pour toutes les opérations SDK, notamment `StartAsync()`, `StopAsync()` et `Snapshot_SaveAsync()`. Marquez les gestionnaires d'événements comme `Async Sub` et utilisez `Await` avant chaque appel au SDK pour garder le thread d'interface utilisateur réactif pendant l'enregistrement webcam. Le pattern async VB.NET est fonctionnellement identique à `async`/`await` en C#.

### Quels types de projets VB.NET prennent en charge la capture vidéo webcam ?

Le SDK prend en charge les applications VB.NET WinForms et WPF sur .NET 8+ et .NET Framework 4.7.2+. WinForms est le choix le plus courant pour les projets de webcam VB.NET car il fournit le contrôle `VideoView` via le concepteur visuel. Pour WPF, ajoutez le contrôle `VideoView` en XAML. Les applications console peuvent enregistrer sans aperçu en omettant le paramètre `VideoView`.

### Comment gérer les erreurs de webcam et les déconnexions de périphérique en VB.NET ?

Abonnez-vous à l'événement `OnError` avec `AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError`. À l'intérieur du gestionnaire, utilisez `Me.InvokeRequired` et `Me.Invoke()` pour mettre à jour l'UI en toute sécurité depuis un thread d'arrière-plan. Encapsulez les appels `StartAsync()` et `StopAsync()` dans des blocs `Try`/`Catch` pour gérer gracieusement les déconnexions de périphérique ou les erreurs de permission.

## Voir aussi

- [Enregistrer une vidéo webcam en C#](save-webcam-video.md) — même fonctionnalité avec exemples de code C#
- [Capture d'écran en VB.NET](screen-capture-vb-net.md) — enregistrer l'écran du bureau au lieu de la webcam en Visual Basic
- [Créer un lecteur vidéo en VB.NET](../../mediaplayer/guides/video-player-vb-net.md) — lecteur multimédia VB.NET avec contrôles de lecture
- [Tutoriel webcam vers MP4](../video-tutorials/video-capture-webcam-mp4.md) — pas-à-pas d'enregistrement webcam C#
- [Capture photo via webcam](make-photo-using-webcam.md) — capturer des images fixes depuis la webcam au lieu de la vidéo
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
