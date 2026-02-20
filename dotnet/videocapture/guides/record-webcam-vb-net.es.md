---
title: Grabar video de webcam en VB.NET - Guía de captura y guardado
description: Grabar video de webcam en VB.NET — código completo de Visual Basic para enumeración de dispositivos, grabación MP4, vista previa en vivo y captura de pantalla con ejemplos async/await.
---

# Grabar video de webcam en VB.NET: Guía completa

Grabar video de webcam en aplicaciones VB.NET es un requisito común para proyectos de videoconferencia, vigilancia y multimedia. Esta guía proporciona instrucciones paso a paso para capturar, previsualizar y guardar video de webcam en archivos MP4 usando [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) en VB.NET (Visual Basic .NET).

## Por qué usar Video Capture SDK .Net para grabación de webcam en VB.NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) ofrece soporte de primera clase para VB.NET con:

- Soporte completo de async/await para captura de webcam sin bloqueo
- Enumeración de dispositivos para fuentes de video, fuentes de audio y salidas de audio
- Grabación MP4 con codificación H.264/H.265 y aceleración GPU
- Vista previa de webcam en tiempo real durante la grabación
- Captura de capturas de pantalla desde la transmisión en vivo de la webcam
- Soporte para WinForms y WPF

## Paquetes NuGet requeridos

Agregue los siguientes paquetes a su proyecto VB.NET:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Ejemplo completo de grabación de webcam en VB.NET

El siguiente ejemplo demuestra una aplicación WinForms completa que enumera dispositivos de webcam, previsualiza la transmisión de la webcam y graba video en MP4.

### Inicialización del SDK y enumeración de dispositivos

Primero, inicialice el SDK y enumere los dispositivos disponibles cuando se cargue el formulario:

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
        ' Inicializar el SDK
        Await VisioForgeX.InitSDKAsync()

        ' Crear el motor de captura de video
        VideoCapture1 = New VideoCaptureCoreX(VideoView1)
        AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError

        ' Enumerar fuentes de video (webcams)
        Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
        For Each source In videoSources
            cbVideoInputDevice.Items.Add(source.DisplayName)
            If cbVideoInputDevice.Items.Count = 1 Then cbVideoInputDevice.SelectedIndex = 0
        Next

        ' Enumerar fuentes de audio (micrófonos)
        Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
        For Each source In audioSources
            cbAudioInputDevice.Items.Add(source.DisplayName)
            If cbAudioInputDevice.Items.Count = 1 Then cbAudioInputDevice.SelectedIndex = 0
        Next

        ' Enumerar salidas de audio (altavoces)
        Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync()
        For Each audioOutput In audioOutputs
            cbAudioOutputDevice.Items.Add(audioOutput.DisplayName)
            If cbAudioOutputDevice.Items.Count = 1 Then cbAudioOutputDevice.SelectedIndex = 0
        Next

        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "capture.mp4")
    End Sub
```

### Listar formatos de video y tasas de fotogramas disponibles

Cuando el usuario selecciona un dispositivo de webcam, complete los formatos de video disponibles:

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

### Iniciar la grabación de webcam a MP4

Configure las fuentes de video y audio, establezca la salida MP4 e inicie la grabación:

```vb.net
    Private Async Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click
        Try
            ' Configurar fuente de video
            Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
            Dim videoDevice = videoSources.FirstOrDefault(
                Function(d) d.DisplayName = cbVideoInputDevice.Text)

            If videoDevice IsNot Nothing Then
                Dim videoSourceSettings As New VideoCaptureDeviceSourceSettings(videoDevice)
                VideoCapture1.Video_Source = videoSourceSettings
            End If

            ' Configurar fuente de audio
            Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
            Dim audioDevice = audioSources.FirstOrDefault(
                Function(d) d.DisplayName = cbAudioInputDevice.Text)

            If audioDevice IsNot Nothing Then
                VideoCapture1.Audio_Source = audioDevice.CreateSourceSettingsVC(Nothing)
            End If

            ' Configurar salida de audio para monitoreo en vivo
            Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync()
            Dim audioOutput = audioOutputs.FirstOrDefault(
                Function(d) d.DisplayName = cbAudioOutputDevice.Text)

            If audioOutput IsNot Nothing Then
                VideoCapture1.Audio_OutputDevice = New AudioRendererSettings(audioOutput)
            End If

            VideoCapture1.Audio_Play = True
            VideoCapture1.Audio_Record = True
            VideoCapture1.Video_Play = True

            ' Configurar salida MP4
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Iniciar grabación
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Detener la grabación de webcam

Detenga la grabación y libere los recursos:

```vb.net
    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        Try
            Await VideoCapture1.StopAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Guardar capturas de pantalla desde la webcam

Capture imágenes fijas desde la transmisión en vivo de la webcam:

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

### Limpieza al cerrar el formulario

Libere correctamente los recursos cuando la aplicación se cierre:

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

## Modo de solo vista previa (sin grabación)

Para previsualizar la transmisión de la webcam sin grabar, simplemente omita agregar la salida MP4:

```vb.net
' Sin salida agregada - solo vista previa
VideoCapture1.Video_Play = True
VideoCapture1.Audio_Play = True

Await VideoCapture1.StartAsync()
```

## Opciones de formato de salida

Aunque esta guía se centra en la grabación MP4, puede guardar video de webcam en otros formatos:

### Salida WebM

```vb.net
VideoCapture1.Outputs_Add(New WebMOutput(edOutput.Text), True)
```

### Salida AVI

```vb.net
VideoCapture1.Outputs_Add(New AVIOutput(edOutput.Text), True)
```

Para una configuración detallada de formatos, consulte la [documentación del formato MP4](../../general/output-formats/mp4.md) y la [documentación del formato WebM](../../general/output-formats/webm.md).

## Aplicaciones de ejemplo

Hay aplicaciones de ejemplo completas en VB.NET disponibles:

- [Captura de video simple en VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WinForms/VB/Simple%20Video%20Capture%20X%20VB) — aplicación completa de captura de webcam

## Preguntas Frecuentes

### ¿Cómo convierto ejemplos de captura de webcam de C# a VB.NET?

La API de Video Capture SDK .Net es idéntica en C# y VB.NET — solo difiere la sintaxis del lenguaje. Conversiones clave: reemplace los manejadores de eventos `+=` con `AddHandler`/`AddressOf`, reemplace las lambdas de C# `(x) =>` con `Function(x)` o `Sub(x)`, y use `Async Sub` en lugar de `async void`. Todas las clases, métodos y propiedades mostrados en la documentación de C# funcionan directamente en VB.NET con estos ajustes de sintaxis.

### ¿VB.NET soporta grabación asíncrona de webcam con Await?

Sí. VB.NET soporta completamente `Async`/`Await` para todas las operaciones del SDK incluyendo `StartAsync()`, `StopAsync()` y `Snapshot_SaveAsync()`. Marque los manejadores de eventos como `Async Sub` y use `Await` antes de cada llamada al SDK para mantener el hilo de la interfaz de usuario responsivo durante la grabación de webcam. El patrón async de VB.NET es funcionalmente idéntico al `async`/`await` de C#.

### ¿Qué tipos de proyectos VB.NET soportan captura de video de webcam?

El SDK soporta aplicaciones VB.NET WinForms y WPF en .NET 8+ y .NET Framework 4.7.2+. WinForms es la opción más común para proyectos de webcam en VB.NET porque proporciona el control `VideoView` a través del diseñador visual. Para WPF, agregue el control `VideoView` en XAML. Las aplicaciones de consola pueden grabar sin vista previa omitiendo el parámetro `VideoView`.

### ¿Cómo manejo errores de webcam y desconexión de dispositivos en VB.NET?

Suscríbase al evento `OnError` usando `AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError`. Dentro del manejador, use `Me.InvokeRequired` y `Me.Invoke()` para actualizar la interfaz de usuario de forma segura desde un hilo en segundo plano. Envuelva las llamadas a `StartAsync()` y `StopAsync()` en bloques `Try`/`Catch` para manejar la desconexión del dispositivo o errores de permisos de forma elegante.

## Ver También

- [Guardar Video de Webcam en C#](save-webcam-video.md) — misma funcionalidad con ejemplos de código en C#
- [Captura de Pantalla en VB.NET](screen-capture-vb-net.md) — grabar pantalla del escritorio en lugar de webcam en Visual Basic
- [Crear un Reproductor de Video en VB.NET](../../mediaplayer/guides/video-player-vb-net.md) — reproductor multimedia VB.NET con controles de reproducción
- [Tutorial de Webcam a MP4](../video-tutorials/video-capture-webcam-mp4.md) — tutorial paso a paso de grabación de webcam en C#
- [Captura de Foto con Cámara Web](make-photo-using-webcam.md) — capturar imágenes fijas de la webcam en lugar de video
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas
