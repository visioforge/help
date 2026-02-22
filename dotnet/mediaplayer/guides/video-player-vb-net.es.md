---
title: Crear un reproductor de video en VB.NET - Guía completa
description: Aprenda a crear un reproductor de video en VB.NET con controles de reproducción, búsqueda en línea de tiempo y volumen usando Media Player SDK .Net.
---

# Crear un reproductor de video en VB.NET

Esta guía le muestra paso a paso cómo crear una aplicación de reproductor de video completa en VB.NET (Visual Basic .NET) utilizando [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net). El reproductor es compatible con MP4, AVI, MKV, WMV, WebM y muchos otros formatos, e incluye controles de reproducción, búsqueda en la línea de tiempo y ajuste de volumen.

## Por qué usar Media Player SDK .Net para VB.NET

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) proporciona una reproducción de video robusta en VB.NET con:

- Compatibilidad con todos los principales formatos de video (MP4, AVI, MKV, WMV, WebM, MOV, TS)
- Compatibilidad con formatos de audio (MP3, AAC, WAV, WMA, FLAC)
- Decodificación de video acelerada por hardware
- Búsqueda en la línea de tiempo y seguimiento de posición
- Control de volumen y velocidad de reproducción
- Integración con interfaces de usuario WinForms y WPF

## Paquetes NuGet requeridos

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Ejemplo completo de reproductor de video en VB.NET

### Inicialización del SDK y configuración del motor

Inicialice el SDK y cree el motor del reproductor cuando se cargue el formulario:

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
        ' Inicializar el SDK
        Await VisioForgeX.InitSDKAsync()

        ' Configurar un temporizador para las actualizaciones de la línea de tiempo
        _timer = New System.Timers.Timer(500)
        AddHandler _timer.Elapsed, AddressOf _timer_Elapsed

        ' Crear el motor del reproductor con el control VideoView
        CreateEngine()

        ' Enumerar los dispositivos de salida de audio
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

### Abrir y reproducir un archivo de video

Seleccione un archivo e inicie la reproducción:

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

        ' Destruir la instancia anterior del motor si existe
        Await DestroyEngineAsync()
        CreateEngine()

        ' Establecer el dispositivo de salida de audio
        Dim audioOutputDevice = (Await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound)).First(
            Function(x) x.Name = cbAudioOutput.Text)
        _player.Audio_OutputDevice = New AudioRendererSettings(audioOutputDevice)

        ' Abrir y reproducir el archivo
        Dim source = Await UniversalSourceSettings.CreateAsync(New Uri(edFilename.Text))
        Await _player.OpenAsync(source)
        Await _player.PlayAsync()

        _timer.Start()
    End Sub
```

### Controles de reproducción (Pausar, Reanudar, Detener)

Implemente los controles estándar del reproductor multimedia:

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

### Búsqueda en la línea de tiempo y seguimiento de posición

Actualice el control deslizante de la línea de tiempo a medida que se reproduce el video y permita la búsqueda:

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

### Control de volumen y velocidad de reproducción

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

### Manejo de errores y limpieza

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

## Formatos de video y audio compatibles

El reproductor de video en VB.NET es compatible con todos los principales formatos:

| Categoría | Formatos |
|-----------|----------|
| Contenedores de video | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS |
| Códecs de video | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Contenedores de audio | MP3, AAC, WAV, WMA, FLAC, OGG |
| Transmisión en vivo | RTSP, HTTP, HLS |

## Aplicaciones de ejemplo

Demos completas de reproductor de video en VB.NET:

- [Demo de reproductor simple VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Simple%20Player%20Demo%20X%20VB) — reproductor de video completo con todos los controles de reproducción

## Recursos relacionados

- [Crear un reproductor de video en C#](video-player-csharp.md)
- [Reproductor multiplataforma con Avalonia](avalonia-player.md)
- [Página del producto Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net)
