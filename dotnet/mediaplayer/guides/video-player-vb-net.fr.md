---
title: Lecteur vidéo VB.NET avec contrôles et positionnement
description: Créez un lecteur vidéo en VB.NET avec contrôles, positionnement et réglage du volume. Exemples de code complets utilisant VisioForge Media Player SDK .NET.
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Playback
  - Streaming
  - Editing
  - Webcam
  - MPEG-DASH
  - MP4
  - MKV
  - WebM
  - AVI
  - MOV
  - WMV
  - H.265
  - AAC
  - MP3
  - FLAC
  - WAV
  - WMA
  - VB.NET
  - NuGet
primary_api_classes:
  - MediaPlayerCoreX
  - MediaPlayerCore

---

# Construire un lecteur vidéo en VB.NET

Ce guide vous accompagne dans la construction d'une application de lecteur vidéo complète en VB.NET (Visual Basic .NET) en utilisant [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net). Le lecteur prend en charge MP4, AVI, MKV, WMV, WebM et de nombreux autres formats avec des contrôles de lecture, le positionnement dans la timeline et le réglage du volume.

## Pourquoi utiliser Media Player SDK .Net pour VB.NET

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) fournit une lecture vidéo VB.NET robuste avec :

- Prise en charge de tous les principaux formats vidéo (MP4, AVI, MKV, WMV, WebM, MOV, TS)
- Prise en charge des formats audio (MP3, AAC, WAV, WMA, FLAC)
- Décodage vidéo accéléré matériellement
- Positionnement dans la timeline et suivi de la position
- Contrôle du volume et de la vitesse de lecture
- Intégration UI WinForms et WPF

## Paquets NuGet requis

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Exemple complet de lecteur vidéo VB.NET

### Initialisation du SDK et configuration du moteur

Initialisez le SDK et créez le moteur du lecteur lorsque le formulaire se charge :

```vb.net
Imports VisioForge.Core
Imports VisioForge.Core.MediaPlayerX
Imports VisioForge.Core.Types.Events
Imports VisioForge.Core.Types.X.AudioRenderers
Imports VisioForge.Core.Types.X.Sources

Public Class Form1
    Private WithEvents _timer As System.Timers.Timer
    Private _timerFlag As Boolean
    Private _player As MediaPlayerCoreX

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialiser le SDK
        Await VisioForgeX.InitSDKAsync()

        ' Configurer un timer pour les mises à jour de la timeline
        _timer = New System.Timers.Timer(500)
        AddHandler _timer.Elapsed, AddressOf _timer_Elapsed

        ' Créer le moteur du lecteur avec le contrôle VideoView
        CreateEngine()

        ' Énumérer les périphériques de sortie audio
        For Each device In Await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound)
            cbAudioOutput.Items.Add(device.Name)
        Next

        If cbAudioOutput.Items.Count > 0 Then
            cbAudioOutput.SelectedIndex = 0
        End If
    End Sub

    Private Sub CreateEngine()
        _player = New MediaPlayerCoreX(VideoView1)
        AddHandler _player.OnError, AddressOf Player_OnError
        AddHandler _player.OnStop, AddressOf Player_OnStop
    End Sub
```

### Ouvrir et lire un fichier vidéo

Sélectionnez un fichier et démarrez la lecture :

```vb.net
    Private Sub btSelectFile_Click(sender As Object, e As EventArgs) Handles btSelectFile.Click
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Video Files|*.mp4;*.ts;*.mts;*.mov;*.avi;*.mkv;*.wmv;*.webm" &
                     "|Audio Files|*.mp3;*.aac;*.wav;*.wma|All Files|*.*"
        If ofd.ShowDialog() = DialogResult.OK Then
            edFilename.Text = ofd.FileName
        End If
    End Sub

    Private Async Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click
        edLog.Clear()

        ' Détruire l'instance précédente du moteur si elle existe
        Await DestroyEngineAsync()
        CreateEngine()

        ' Définir le périphérique de sortie audio
        Dim audioOutputDevice = (Await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound)).First(
            Function(x) x.Name = cbAudioOutput.Text)
        _player.Audio_OutputDevice = New AudioRendererSettings(audioOutputDevice)

        ' Ouvrir et lire le fichier
        Dim source = Await UniversalSourceSettings.CreateAsync(New Uri(edFilename.Text))
        Await _player.OpenAsync(source)
        Await _player.PlayAsync()

        _timer.Start()
    End Sub
```

### Contrôles de lecture (pause, reprise, arrêt)

Implémentez les contrôles standard du lecteur multimédia :

```vb.net
    Private Async Sub btPause_Click(sender As Object, e As EventArgs) Handles btPause.Click
        Await _player.PauseAsync()
    End Sub

    Private Async Sub btResume_Click(sender As Object, e As EventArgs) Handles btResume.Click
        Await _player.ResumeAsync()
    End Sub

    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        _timer.Stop()

        If _player IsNot Nothing Then
            Await _player.StopAsync()
            Await DestroyEngineAsync()
            _player = Nothing
        End If

        tbTimeline.Value = 0
    End Sub
```

### Positionnement dans la timeline et suivi de la position

Mettez à jour le curseur de timeline pendant la lecture vidéo et autorisez le positionnement :

```vb.net
    Private Async Sub _timer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs)
        _timerFlag = True

        If _player Is Nothing Then Return

        Dim position = Await _player.Position_GetAsync()
        Dim duration = Await _player.DurationAsync()

        Invoke(Sub()
                   tbTimeline.Maximum = CInt(duration.TotalSeconds)
                   lbTime.Text = position.ToString("hh\:mm\:ss") & " | " &
                                 duration.ToString("hh\:mm\:ss")

                   If tbTimeline.Maximum >= position.TotalSeconds Then
                       tbTimeline.Value = CInt(position.TotalSeconds)
                   End If
               End Sub)

        _timerFlag = False
    End Sub

    Private Async Sub tbTimeline_Scroll(sender As Object, e As EventArgs) Handles tbTimeline.Scroll
        If Not _timerFlag AndAlso _player IsNot Nothing Then
            Await _player.Position_SetAsync(TimeSpan.FromSeconds(tbTimeline.Value))
        End If
    End Sub
```

### Contrôle du volume et de la vitesse de lecture

```vb.net
    Private Sub tbVolume1_Scroll(sender As Object, e As EventArgs) Handles tbVolume1.Scroll
        If _player IsNot Nothing Then
            _player.Audio_OutputDevice_Volume = tbVolume1.Value / 100.0
        End If
    End Sub

    Private Async Sub tbSpeed_Scroll(sender As Object, e As EventArgs) Handles tbSpeed.Scroll
        Await _player.Rate_SetAsync(tbSpeed.Value / 10.0)
    End Sub
```

### Gestion des erreurs et nettoyage

```vb.net
    Private Sub Player_OnError(sender As Object, e As ErrorsEventArgs)
        Invoke(Sub()
                   edLog.Text = edLog.Text + e.Message + Environment.NewLine
               End Sub)
    End Sub

    Private Sub Player_OnStop(sender As Object, e As StopEventArgs)
        Invoke(Sub()
                   tbTimeline.Value = 0
               End Sub)
    End Sub

    Private Async Function DestroyEngineAsync() As Task
        If _player IsNot Nothing Then
            RemoveHandler _player.OnError, AddressOf Player_OnError
            RemoveHandler _player.OnStop, AddressOf Player_OnStop
            Await _player.DisposeAsync()
            _player = Nothing
        End If
    End Function

    Private Async Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        _timer.Stop()
        Await DestroyEngineAsync()
        VisioForgeX.DestroySDK()
    End Sub
End Class
```

## Formats vidéo et audio pris en charge

Le lecteur vidéo VB.NET prend en charge tous les principaux formats :

| Catégorie | Formats |
|----------|---------|
| Conteneurs vidéo | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS |
| Codecs vidéo | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Conteneurs audio | MP3, AAC, WAV, WMA, FLAC, OGG |
| Streaming | RTSP, HTTP, HLS |

## Applications d'exemple

Démos complètes de lecteur vidéo VB.NET :

- [Simple Player Demo VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Simple%20Player%20Demo%20X%20VB) — lecteur vidéo complet avec tous les contrôles de lecture

## Foire aux questions

### Quelle licence faut-il pour une application de lecteur vidéo VB.NET ?

Media Player SDK .Net nécessite une licence pour le développement et la distribution. Une licence Developer supprime le filigrane d'évaluation et débloque toutes les fonctionnalités durant le développement. Une licence Release est requise lors de la distribution de votre application aux utilisateurs finaux. Le SDK est disponible en édition Premium qui inclut tous les formats pris en charge, l'accélération matérielle et les deux moteurs DirectShow et GStreamer. Vous pouvez évaluer le SDK sans licence — la lecture fonctionne entièrement mais inclut un filigrane superposé. Visitez la [page produit](https://www.visioforge.com/media-player-sdk-net) pour les tarifs et les options de licence.

### Quels formats vidéo et audio le lecteur VB.NET prend-il en charge ?

Le lecteur prend en charge tous les formats listés dans le tableau ci-dessus, notamment les conteneurs MP4, AVI, MKV, WMV, WebM, MOV et TS avec les codecs vidéo H.264, H.265/HEVC, VP8, VP9, AV1 et MPEG-2. Les formats audio incluent MP3, AAC, WAV, WMA, FLAC et OGG. Les protocoles de streaming réseau tels que RTSP, HTTP, HLS et MPEG-DASH sont également pris en charge. La prise en charge des formats est identique à la version C# car les deux langages utilisent le même moteur de lecture basé sur GStreamer.

### Dois-je utiliser le moteur DirectShow ou GStreamer pour mon lecteur vidéo VB.NET ?

L'API `MediaPlayerCoreX` présentée dans ce guide utilise le moteur GStreamer et constitue le choix recommandé pour les nouveaux projets multiplateformes (Windows, macOS, Linux, iOS, Android), avec la plus large couverture de formats et un décodage accéléré matériellement. Le moteur DirectShow (`MediaPlayerCore`) est réservé à Windows mais reste un moteur de premier rang entièrement pris en charge — choisissez-le lorsque votre application est uniquement pour Windows et que vous souhaitez la surcharge par image plus faible et l'écosystème de filtres DirectShow mature. Les deux moteurs fonctionnent à l'identique depuis le code VB.NET ; pour les nouveaux travaux multiplateformes, commencez avec `MediaPlayerCoreX`, et pour les déploiements Windows uniquement, l'un ou l'autre moteur convient bien.

### Comment déployer une application de lecteur vidéo VB.NET ?

Incluez les paquets redist NuGet listés dans la section de référence NuGet ci-dessus — ils regroupent les bibliothèques GStreamer natives dont votre application a besoin. Utilisez `dotnet publish` avec un déploiement autonome pour éviter de nécessiter une installation runtime séparée sur la machine cible. Le paquet `VisioForge.CrossPlatform.Core.Windows.x64` contient le runtime GStreamer, donc aucune installation séparée de GStreamer n'est nécessaire. Pour les projets WPF, la même approche de déploiement fonctionne — il suffit de référencer le contrôle VideoView spécifique à WPF au lieu de celui de WinForms.

## Voir aussi

- [Construire un lecteur vidéo en C#](video-player-csharp.md) — lecteur WinForms et WPF avec moteurs DirectShow et GStreamer
- [Lecteur vidéo .NET MAUI](maui-player.md) — iOS, Android, macOS et Windows à partir d'une seule base de code C#
- [Guide du lecteur Avalonia](avalonia-player.md) — implémentation MVVM complète avec dialogues de fichiers et configuration des plateformes
- [Mode boucle et plage de position](loop-and-position-range.md) — lecture en boucle et sélection de segments pour les deux moteurs
- [Enregistrer une vidéo webcam en VB.NET](../../videocapture/guides/record-webcam-vb-net.md) — application de capture webcam en Visual Basic .NET
- [Exemples de code](../code-samples/index.md) — exemples d'extraction d'image, listes de lecture et lecture inversée
- [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) — page produit et téléchargements
