---
title: Capture d'écran en VB.NET — bureau vers MP4 avec VisioForge
description: Apprenez à enregistrer l'écran du bureau en VB.NET. Guide complet pour la capture plein écran et région vers MP4 avec audio système via Video Capture SDK .Net.
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
  - ScreenCaptureD3D11SourceSettings
  - VideoCaptureCoreX
  - MP4Output
  - VideoView
  - LoopbackAudioCaptureDeviceSourceSettings

---

# Capture d'écran en VB.NET : guide complet pour enregistrer la vidéo du bureau

L'enregistrement de l'écran du bureau dans les applications VB.NET (Visual Basic .NET) est essentiel pour créer des enregistreurs d'écran, des outils de tutoriels et des logiciels de surveillance. Que vous ayez besoin de capture plein écran, d'enregistrement par région ou de capture audio système, ce guide fournit des exemples Visual Basic étape par étape avec [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net).

## Pourquoi utiliser Video Capture SDK .Net pour l'enregistrement d'écran en VB.NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) offre une solution complète d'enregistrement d'écran pour les développeurs VB.NET :

- Capture complète du bureau et enregistrement d'écran par région personnalisée
- Prise en charge multi-écran avec énumération des moniteurs
- Capture du curseur de la souris avec mise en évidence optionnelle des clics
- Enregistrement audio système (microphone et loopback/son système)
- Sortie MP4 avec encodage H.264/H.265 et accélération GPU
- Aperçu d'écran en temps réel pendant l'enregistrement
- Pattern async/await pour une capture non bloquante
- Prise en charge de WinForms et WPF

## Paquets NuGet requis

Ajoutez les paquets suivants à votre projet VB.NET :

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Exemple complet d'enregistrement d'écran en VB.NET

L'exemple de capture d'écran suivant illustre une application WinForms qui enregistre le bureau avec audio et le sauvegarde dans un fichier MP4.

### Initialisation du SDK et énumération des écrans

Initialisez le SDK et énumérez les écrans et périphériques audio disponibles au chargement du formulaire :

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

        ' Créer le moteur de capture vidéo avec VideoView pour l'aperçu
        VideoCapture1 = New VideoCaptureCoreX(VideoView1)
        AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError

        ' Énumérer les écrans disponibles
        For i As Integer = 0 To Screen.AllScreens.Length - 1
            Dim scr = Screen.AllScreens(i)
            cbDisplayIndex.Items.Add(
                $"{i}: {scr.DeviceName} ({scr.Bounds.Width}x{scr.Bounds.Height})")
        Next
        If cbDisplayIndex.Items.Count > 0 Then cbDisplayIndex.SelectedIndex = 0

        ' Énumérer les périphériques d'entrée audio (microphones)
        Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
        For Each source In audioSources
            cbAudioInputDevice.Items.Add(source.DisplayName)
            If cbAudioInputDevice.Items.Count = 1 Then cbAudioInputDevice.SelectedIndex = 0
        Next

        ' Énumérer les périphériques audio loopback (son système)
        Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync(
            AudioOutputDeviceAPI.WASAPI2)
        For Each audioOutput In audioOutputs
            cbAudioLoopbackDevice.Items.Add(audioOutput.Name)
            If cbAudioLoopbackDevice.Items.Count = 1 Then cbAudioLoopbackDevice.SelectedIndex = 0
        Next

        ' Définir le chemin de fichier de sortie par défaut
        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            "screen_capture.mp4")
    End Sub
```

### Capture plein écran vers MP4

Utilisez la classe [ScreenCaptureD3D11SourceSettings](../video-sources/screen.md) pour capturer l'ensemble du bureau à une fréquence d'images spécifiée et le sauvegarder en MP4 :

```vb.net
    Private Async Sub btStartFullScreen_Click(
            sender As Object, e As EventArgs) Handles btStartFullScreen.Click
        Try
            ' Configurer la source de capture plein écran
            Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
                .MonitorIndex = cbDisplayIndex.SelectedIndex,
                .FrameRate = New VideoFrameRate(15, 1),
                .CaptureCursor = True
            }

            VideoCapture1.Video_Source = screenSource
            VideoCapture1.Video_Play = True

            ' Configurer la sortie MP4
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Démarrer la capture
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Capture par région (zone personnalisée)

Capturez une zone rectangulaire spécifique de l'écran au lieu du bureau complet :

```vb.net
    Private Async Sub btStartRegion_Click(
            sender As Object, e As EventArgs) Handles btStartRegion.Click
        Try
            ' Configurer la capture par région avec rectangle personnalisé
            Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
                .Rectangle = New Rect(
                    CInt(edLeft.Text),
                    CInt(edTop.Text),
                    CInt(edRight.Text),
                    CInt(edBottom.Text)),
                .FrameRate = New VideoFrameRate(15, 1),
                .CaptureCursor = True
            }

            VideoCapture1.Video_Source = screenSource
            VideoCapture1.Video_Play = True

            ' Configurer la sortie MP4
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Démarrer la capture
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Enregistrement avec audio système (microphone)

Ajoutez l'enregistrement audio du microphone à votre capture d'écran :

```vb.net
    ' Configurer la source audio du microphone
    Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
    Dim audioDevice = audioSources.FirstOrDefault(
        Function(d) d.DisplayName = cbAudioInputDevice.Text)

    If audioDevice IsNot Nothing Then
        VideoCapture1.Audio_Source = audioDevice.CreateSourceSettingsVC(Nothing)
    End If

    VideoCapture1.Audio_Record = True
```

### Enregistrement avec audio loopback (son système)

Capturez la sortie audio système (ce que vous entendez par les haut-parleurs) au lieu de l'entrée microphone :

```vb.net
    ' Configurer la source audio loopback (son système)
    Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync(
        AudioOutputDeviceAPI.WASAPI2)
    Dim audioDevice = audioOutputs.FirstOrDefault(
        Function(d) d.Name = cbAudioLoopbackDevice.Text)

    If audioDevice IsNot Nothing Then
        VideoCapture1.Audio_Source = New LoopbackAudioCaptureDeviceSourceSettings(audioDevice)
    End If

    VideoCapture1.Audio_Record = True
```

### Arrêter l'enregistrement

Arrêtez l'enregistrement d'écran et finalisez le fichier de sortie :

```vb.net
    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        Try
            Await VideoCapture1.StopAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

Pour prévisualiser le flux de capture du bureau sans sauvegarder dans un fichier, omettez l'ajout de la sortie :

```vb.net
' Configurer la source d'écran
Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
    .MonitorIndex = 0,
    .FrameRate = New VideoFrameRate(15, 1),
    .CaptureCursor = True
}

VideoCapture1.Video_Source = screenSource
VideoCapture1.Video_Play = True

' Aucune sortie ajoutée — aperçu uniquement
Await VideoCapture1.StartAsync()
```

## Options de format de sortie

Bien que ce guide se concentre sur l'[enregistrement MP4](../../general/output-formats/mp4.md), vous pouvez sauvegarder votre capture d'écran dans d'autres formats :

### Sortie WebM

[WebM](../../general/output-formats/webm.md) est idéal pour les enregistrements d'écran orientés web, offrant un encodage VP8/VP9 libre de droits avec des tailles de fichier réduites :

```vb.net
VideoCapture1.Outputs_Add(New WebMOutput(edOutput.Text), True)
```

### Sortie AVI

AVI offre une compatibilité maximale avec les éditeurs vidéo hérités et les workflows basés sur Windows :

```vb.net
VideoCapture1.Outputs_Add(New AVIOutput(edOutput.Text), True)
```

## Applications d'exemple

Des applications d'exemple VB.NET complètes sont disponibles :

- [Screen Capture X VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WinForms/VB/Screen%20Capture%20X%20VB) — application complète de capture d'écran avec modes région et plein écran

## Foire aux questions

### Comment enregistrer simultanément microphone et audio système en capture d'écran VB.NET ?

`VideoCaptureCoreX` expose une seule `Audio_Source`, donc pour mélanger un microphone avec l'audio en loopback système, vous construisez un pipeline Media Blocks : un `SystemAudioSourceBlock` pour le microphone, une source loopback et un `AudioMixerBlock` qui les combine avant l'encodeur/le puits. La [référence de traitement audio Media Blocks](../../mediablocks/AudioProcessing/index.md) couvre la configuration `AudioMixerBlock`. Alternativement, sous Windows, basculez vers `VideoCaptureCore` (non-X) où `Additional_Audio_CaptureDevice_MixChannels` + la liste interne de périphériques supplémentaires gèrent directement le mélange.

### Quelle fréquence d'images utiliser pour l'enregistrement d'écran en VB.NET ?

Utilisez 10–15 FPS pour l'enregistrement général du bureau (documents, présentations, navigation) et 25–30 FPS pour le contenu à fort mouvement comme la lecture vidéo ou les animations. Des fréquences d'images plus élevées augmentent l'utilisation CPU et la taille du fichier. Le code d'exemple de ce guide utilise `New VideoFrameRate(15, 1)` — changez le premier paramètre pour votre FPS souhaité. Pour l'encodage accéléré par GPU, associez les fréquences d'images plus élevées à `MP4Output` qui utilise l'encodage H.264 matériel lorsque disponible.

### Puis-je enregistrer l'écran sans afficher d'aperçu en VB.NET ?

Oui. Créez `VideoCaptureCoreX` sans paramètre `VideoView` : `VideoCapture1 = New VideoCaptureCoreX()`. Définissez `Video_Play = False` pour ignorer le rendu, configurez la source d'écran et la sortie MP4 comme d'habitude, puis appelez `Await VideoCapture1.StartAsync()`. Cela réduit l'utilisation CPU car aucune image n'est rendue dans l'UI. Les applications console et services Windows peuvent utiliser cette approche pour l'enregistrement d'écran sans interface.

### Comment gérer les écrans haute résolution (DPI) et mis à l'échelle en enregistrement d'écran VB.NET ?

`ScreenCaptureD3D11SourceSettings` capture la résolution native de l'écran indépendamment de la mise à l'échelle d'affichage Windows. Un moniteur 4K à 150 % de mise à l'échelle est toujours enregistré en 3840×2160 pixels. Si votre application WinForms VB.NET affiche des coordonnées incorrectes en capture par région, ajoutez `<dpiAware>true</dpiAware>` à votre fichier `app.manifest` ou appelez `SetProcessDPIAware()` au démarrage afin que `Screen.AllScreens` rapporte les dimensions en pixels physiques au lieu des valeurs mises à l'échelle.

## Voir aussi

- [Tutoriel de capture d'écran vers MP4](../video-tutorials/screen-capture-mp4.md) — pas-à-pas d'enregistrement d'écran C#
- [Configuration des sources d'écran](../video-sources/screen.md) — référence de l'API de capture DirectX 11/12 et WGC
- [Enregistrer une vidéo webcam en VB.NET](record-webcam-vb-net.md) — capturer depuis la webcam au lieu de l'écran en Visual Basic
- [Enregistrer une vidéo webcam en C#](save-webcam-video.md) — même fonctionnalité de capture webcam avec exemples C#
- [Créer un lecteur vidéo en VB.NET](../../mediaplayer/guides/video-player-vb-net.md) — lecteur multimédia VB.NET avec contrôles de lecture
- [Enregistrement pré-événement](pre-event-recording.md) — enregistrement par tampon circulaire qui capture les images avant l'événement déclencheur
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — page produit et téléchargements
