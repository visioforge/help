---
title: Captura de pantalla en VB.NET - Grabar escritorio a MP4
description: Capture la pantalla del escritorio en VB.NET. Grabación completa o por región a MP4 con audio del sistema usando Video Capture SDK .Net.
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

# Captura de pantalla en VB.NET: Guía completa para grabar video del escritorio

Grabar la pantalla del escritorio en aplicaciones VB.NET (Visual Basic .NET) es esencial para crear grabadores de pantalla, herramientas de tutoriales y software de vigilancia. Ya sea que necesite captura de pantalla completa, grabación por región o captura de audio del sistema, esta guía proporciona ejemplos de código Visual Basic paso a paso usando [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net).

## Por qué usar Video Capture SDK .Net para grabación de pantalla en VB.NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) proporciona una solución completa de grabación de pantalla para desarrolladores VB.NET:

- Captura de escritorio completa y grabación de pantalla por región personalizada
- Soporte multi-monitor con enumeración de pantallas
- Captura del cursor del ratón con resaltado opcional de clics
- Grabación de audio del sistema (micrófono y loopback/sonido del sistema)
- Salida MP4 con codificación H.264/H.265 y aceleración GPU
- Vista previa de pantalla en tiempo real durante la grabación
- Patrón async/await para captura sin bloqueo
- Soporte para WinForms y WPF

## Paquetes NuGet requeridos

Agregue los siguientes paquetes a su proyecto VB.NET:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Ejemplo completo de grabación de pantalla en VB.NET

El siguiente ejemplo de captura de pantalla demuestra una aplicación WinForms que graba el escritorio con audio y lo guarda en un archivo MP4.

### Inicialización del SDK y enumeración de pantallas

Inicialice el SDK y enumere las pantallas disponibles y los dispositivos de audio cuando se cargue el formulario:

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

        ' Crear el motor de captura de video con VideoView para vista previa
        VideoCapture1 = New VideoCaptureCoreX(VideoView1)
        AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError

        ' Enumerar pantallas disponibles
        For i As Integer = 0 To Screen.AllScreens.Length - 1
            Dim scr = Screen.AllScreens(i)
            cbDisplayIndex.Items.Add(
                $"{i}: {scr.DeviceName} ({scr.Bounds.Width}x{scr.Bounds.Height})")
        Next
        If cbDisplayIndex.Items.Count > 0 Then cbDisplayIndex.SelectedIndex = 0

        ' Enumerar dispositivos de entrada de audio (micrófonos)
        Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
        For Each source In audioSources
            cbAudioInputDevice.Items.Add(source.DisplayName)
            If cbAudioInputDevice.Items.Count = 1 Then cbAudioInputDevice.SelectedIndex = 0
        Next

        ' Enumerar dispositivos de audio loopback (sonido del sistema)
        Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync(
            AudioOutputDeviceAPI.WASAPI2)
        For Each audioOutput In audioOutputs
            cbAudioLoopbackDevice.Items.Add(audioOutput.Name)
            If cbAudioLoopbackDevice.Items.Count = 1 Then cbAudioLoopbackDevice.SelectedIndex = 0
        Next

        ' Establecer ruta de archivo de salida predeterminada
        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            "screen_capture.mp4")
    End Sub
```

### Captura de pantalla completa a MP4

Use la clase [ScreenCaptureD3D11SourceSettings](../video-sources/screen.md) para capturar todo el escritorio a una tasa de fotogramas especificada y guardar a MP4:

```vb.net
    Private Async Sub btStartFullScreen_Click(
            sender As Object, e As EventArgs) Handles btStartFullScreen.Click
        Try
            ' Configurar fuente de captura de pantalla completa
            Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
                .MonitorIndex = cbDisplayIndex.SelectedIndex,
                .FrameRate = New VideoFrameRate(15, 1),
                .CaptureCursor = True
            }

            VideoCapture1.Video_Source = screenSource
            VideoCapture1.Video_Play = True

            ' Configurar salida MP4
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Iniciar captura
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Captura por región (área personalizada)

Capture un área rectangular específica de la pantalla en lugar del escritorio completo:

```vb.net
    Private Async Sub btStartRegion_Click(
            sender As Object, e As EventArgs) Handles btStartRegion.Click
        Try
            ' Configurar captura por región con rectángulo personalizado
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

            ' Configurar salida MP4
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Iniciar captura
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Grabación con audio del sistema (micrófono)

Agregue grabación de audio de micrófono a su captura de pantalla:

```vb.net
    ' Configurar fuente de audio de micrófono
    Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
    Dim audioDevice = audioSources.FirstOrDefault(
        Function(d) d.DisplayName = cbAudioInputDevice.Text)

    If audioDevice IsNot Nothing Then
        VideoCapture1.Audio_Source = audioDevice.CreateSourceSettingsVC(Nothing)
    End If

    VideoCapture1.Audio_Record = True
```

### Grabación con audio loopback (sonido del sistema)

Capture la salida de audio del sistema (lo que escucha por los altavoces) en lugar de la entrada del micrófono:

```vb.net
    ' Configurar fuente de audio loopback (sonido del sistema)
    Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync(
        AudioOutputDeviceAPI.WASAPI2)
    Dim audioDevice = audioOutputs.FirstOrDefault(
        Function(d) d.Name = cbAudioLoopbackDevice.Text)

    If audioDevice IsNot Nothing Then
        VideoCapture1.Audio_Source = New LoopbackAudioCaptureDeviceSourceSettings(audioDevice)
    End If

    VideoCapture1.Audio_Record = True
```

### Detener la grabación

Detenga la grabación de pantalla y finalice el archivo de salida:

```vb.net
    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        Try
            Await VideoCapture1.StopAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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

Para previsualizar la captura del escritorio sin guardar a un archivo, omita agregar la salida:

```vb.net
' Configurar fuente de pantalla
Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
    .MonitorIndex = 0,
    .FrameRate = New VideoFrameRate(15, 1),
    .CaptureCursor = True
}

VideoCapture1.Video_Source = screenSource
VideoCapture1.Video_Play = True

' Sin salida agregada - solo vista previa
Await VideoCapture1.StartAsync()
```

## Opciones de formato de salida

Aunque esta guía se centra en la [grabación MP4](../../general/output-formats/mp4.md), puede guardar su captura de pantalla en otros formatos:

### Salida WebM

[WebM](../../general/output-formats/webm.md) es ideal para grabaciones de pantalla basadas en web, ofreciendo codificación VP8/VP9 libre de regalías con tamaños de archivo más pequeños:

```vb.net
VideoCapture1.Outputs_Add(New WebMOutput(edOutput.Text), True)
```

### Salida AVI

AVI proporciona máxima compatibilidad con editores de video heredados y flujos de trabajo basados en Windows:

```vb.net
VideoCapture1.Outputs_Add(New AVIOutput(edOutput.Text), True)
```

## Aplicaciones de ejemplo

Hay aplicaciones de ejemplo completas en VB.NET disponibles:

- [Screen Capture X VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WinForms/VB/Screen%20Capture%20X%20VB) — aplicación completa de captura de pantalla con modos de región y pantalla completa

## Preguntas Frecuentes

### ¿Cómo grabo audio de micrófono y del sistema simultáneamente en la captura de pantalla con VB.NET?

`VideoCaptureCoreX` expone un único `Audio_Source`, así que para mezclar micrófono con audio loopback del sistema construyes un pipeline de Media Blocks: un `SystemAudioSourceBlock` para el micrófono, uno loopback, y un `AudioMixerBlock` que los combina antes del encoder/sink. La [referencia de procesamiento de audio de Media Blocks](../../mediablocks/AudioProcessing/index.md) cubre el setup de `AudioMixerBlock`. Alternativamente, en Windows usa `VideoCaptureCore` (non-X) donde `Additional_Audio_CaptureDevice_MixChannels` y la lista interna de dispositivos adicionales gestionan la mezcla directamente.

### ¿Qué tasa de fotogramas debo usar para la grabación de pantalla en VB.NET?

Use 10–15 FPS para grabación general del escritorio (documentos, presentaciones, navegación) y 25–30 FPS para contenido con mucho movimiento como reproducción de video o animaciones. Las tasas de fotogramas más altas aumentan el uso de CPU y el tamaño del archivo. El código de ejemplo en esta guía usa `New VideoFrameRate(15, 1)` — cambie el primer parámetro a los FPS deseados. Para codificación acelerada por GPU, combine tasas de fotogramas más altas con `MP4Output` que usa codificación H.264 por hardware cuando está disponible.

### ¿Puedo grabar la pantalla sin mostrar una vista previa en VB.NET?

Sí. Cree `VideoCaptureCoreX` sin un parámetro `VideoView`: `VideoCapture1 = New VideoCaptureCoreX()`. Establezca `Video_Play = False` para omitir el renderizado, configure la fuente de pantalla y la salida MP4 como de costumbre, luego llame a `Await VideoCapture1.StartAsync()`. Esto reduce el uso de CPU porque no se renderizan fotogramas en la interfaz de usuario. Las aplicaciones de consola y los Servicios de Windows pueden usar este enfoque para grabación de pantalla sin interfaz gráfica.

### ¿Cómo manejo pantallas de alto DPI y escaladas en la grabación de pantalla con VB.NET?

`ScreenCaptureD3D11SourceSettings` captura la resolución nativa de la pantalla independientemente del escalado de pantalla de Windows. Un monitor 4K al 150% de escalado se graba igualmente a 3840×2160 píxeles. Si su aplicación VB.NET WinForms muestra coordenadas incorrectas en la captura por región, agregue `<dpiAware>true</dpiAware>` a su archivo `app.manifest` o llame a `SetProcessDPIAware()` al inicio para que `Screen.AllScreens` reporte dimensiones en píxeles físicos en lugar de valores escalados.

## Ver También

- [Tutorial de captura de pantalla a MP4](../video-tutorials/screen-capture-mp4.md) — tutorial paso a paso de grabación de pantalla en C#
- [Configuración de fuente de pantalla](../video-sources/screen.md) — referencia de API de captura DirectX 11/12 y WGC
- [Grabar video de webcam en VB.NET](record-webcam-vb-net.md) — capturar desde webcam en lugar de pantalla en Visual Basic
- [Guardar video de webcam en C#](save-webcam-video.md) — misma funcionalidad de captura de webcam con ejemplos de código en C#
- [Crear un reproductor de video en VB.NET](../../mediaplayer/guides/video-player-vb-net.md) — reproductor multimedia VB.NET con controles de reproducción
- [Grabación pre-evento](pre-event-recording.md) — grabación de búfer circular que captura material antes del evento desencadenante
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas
