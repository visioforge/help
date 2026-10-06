---
title: Superponer controles Avalonia sobre el video en C# .NET
description: Coloque botones, paneles y contenido con scroll de Avalonia sobre VideoView. Modo de renderizado por software, limitaciones de airspace nativo y WSLg.
sidebar_label: Controles Avalonia sobre el video
tags:
  - Media Blocks SDK
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Avalonia
  - VideoRendererBlock
  - VideoView
  - Windows
  - Linux
  - macOS
  - Playback
  - Capture
  - C#
primary_api_classes:
  - VideoView
  - VideoRendererBlock

---

# Superponer controles Avalonia sobre el video

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Colocar elementos de Avalonia — botones, superposiciones, banners de estado — sobre la vista previa de video solo funciona cuando el video se dibuja *dentro* del árbol visual de Avalonia. El control `VideoView` de Avalonia dibuja allí en uno de dos modos. El modo por defecto envía los fotogramas a una ventana hija nativa alojada a través de `NativeControlHost` (una ventana hija HWND en Windows, una ventana X11 vía XEmbed en Linux y macOS), y una ventana hija nativa se compone después — y por encima — de todo lo que Avalonia ha dibujado: las superposiciones colocadas sobre ella quedan invisibles, y no se recorta con el contenido de un `ScrollViewer`. Es la misma limitación de airspace que tiene el control WPF, y Avalonia la documenta como [limitación de la incrustación nativa](https://docs.avaloniaui.net/docs/app-development/native-interop).

| Modo de renderizado | Cómo activarlo | Controles Avalonia encima | Notas |
|---|---|---|---|
| Incrustación de ventana nativa | por defecto (`SetNativeRendering(true)`) | No | La vía más rápida. El video se recorta a los propios límites del control host y siempre se dibuja sobre el contenido de Avalonia. |
| Software (`WriteableBitmap`) | `VideoView1.SetNativeRendering(false)` | Sí | Los fotogramas se copian en un `WriteableBitmap` dentro del árbol visual, al coste de una copia CPU por fotograma. |

No existe un equivalente GPU-compuesto del modo `D3D11Composable` de WPF en el control Avalonia; el modo software es el modo que compone con la UI.

## Modo de renderizado por software

Llame a `SetNativeRendering(false)` en la vista **antes** de que arranque el pipeline o el motor:

```cs
VideoView1.SetNativeRendering(false);
```

En Media Blocks SDK .NET, conecte la vista de la forma habitual — `VideoRendererBlock` lee el modo de la vista al construirse y entrega los fotogramas a través de un buffer sink en lugar de incrustar un renderizador de ventana:

```cs
_pipeline = new MediaBlocksPipeline();

_fileSource = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(filename, renderVideo: true, renderAudio: false));

_videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);
_pipeline.Connect(_fileSource.VideoOutput, _videoRenderer.Input);

await _pipeline.StartAsync();
```

En modo software el video es un elemento Avalonia ordinario: los paneles, botones y popups se dibujan encima, y se recorta correctamente dentro de un `ScrollViewer` o cualquier contenedor con recorte. El fotograma se escala de forma uniforme y se centra dentro de la vista, igual que en modo nativo.

Los motores X (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`) respetan la misma llamada — configúrela antes de adjuntar el motor / iniciar la reproducción.

## Layout

Coloque la vista y la superposición en la misma celda del `Grid`. Los hijos posteriores se dibujan encima:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"
        x:Class="MyPlayer.MainWindow">
    <Panel>
        <vf:VideoView x:Name="VideoView1" />

        <Border Background="#88000000" CornerRadius="8"
                VerticalAlignment="Top" HorizontalAlignment="Left"
                Margin="8" Padding="10,6">
            <TextBlock Text="LIVE" Foreground="White" FontWeight="SemiBold" />
        </Border>
    </Panel>
</Window>
```

## Ejecución bajo WSL (WSLg)

Dentro de WSLg la incrustación de la ventana hija nativa hace que la aplicación se cierre en cuanto llega el primer fotograma de video: Mesa no puede inicializar su ruta de GPU (`ZINK: failed to choose pdev`, `failed to create dri2 screen`), y la creación de la ventana XEmbed que sigue falla con ello. Es una limitación del sistema de ventanas de WSLg, no un defecto del SDK ni de su aplicación — la misma aplicación se renderiza correctamente en una instalación nativa de Ubuntu.

El modo software no crea ninguna ventana hija nativa, por lo que se renderiza normalmente bajo WSLg. Si desarrolla dentro de WSL, llame a `SetNativeRendering(false)`, o condicione la llamada a una detección de WSL al arrancar. Véase [Despliegue en Ubuntu](../../deployment-x/Ubuntu.md) para los paquetes de GStreamer que WSL necesita.

## Solución de problemas

**La superposición es invisible, el video se reproduce.** La vista está en modo de incrustación nativa. Llame a `SetNativeRendering(false)` antes de iniciar el pipeline — el modo se lee cuando el pipeline se construye, así que reinicie la reproducción para que el cambio surta efecto.

**Se abre una ventana de video separada en lugar de renderizarse dentro del layout.** Un `VideoView` que no forma parte de una ventana con raíz no proporciona ventana padre al sink de video, y el sink abre su propia ventana de primer nivel. Añada la vista al árbol visual de la ventana (el control añade su host nativo al arrancar).

**La aplicación se cierra en cuanto llega el primer fotograma, con errores de Mesa/Zink en la consola.** Incrustación nativa bajo WSLg — véase la sección anterior.

## Páginas relacionadas

- [Superponer controles WPF sobre el video](wpf-controls-over-video.md) — el mismo tema de airspace para WPF, incluido el modo GPU-compuesto `D3D11Composable`.
- [Despliegue en Ubuntu](../../deployment-x/Ubuntu.md) — paquetes de GStreamer para Linux y WSL.
