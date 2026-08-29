---
title: Superponer controles WPF sobre el video en C# .NET
description: Coloque botones, texto y paneles WPF sobre VideoView. Renderizador componible D3D11, modo WriteableBitmap, airspace y migración desde WPF_WinUI_Callback.
sidebar_label: Controles WPF sobre el video
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
  - WPF
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - MediaPlayerCore
  - VideoCaptureCore
  - Windows
  - Playback
  - Capture
  - C#
primary_api_classes:
  - VideoView
  - VideoRendererMode
  - IVideoSurfaceProvider
  - MediaPlayerCoreX
  - VideoCaptureCore

---

# Superponer controles WPF sobre el video

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Colocar elementos WPF — botones, superposiciones, banners de estado, controles de reproducción — sobre la vista previa del video solo funciona cuando el video se dibuja *dentro* del árbol visual de WPF. Si los fotogramas van a una ventana hija nativa (HWND), WPF no puede pintar nada encima: esa es la clásica limitación de **airspace** de WPF, y ningún valor de `Panel.ZIndex` la resuelve.

El control WPF `VisioForge.Core.UI.WPF.VideoView` admite tres formas de dibujar un fotograma. Dos de ellas se componen con WPF, una no:

| Modo de renderizado | Cómo activarlo | Controles WPF encima | Notas |
|---|---|---|---|
| `D3D11Composable` | `VideoView1.UseD3D11ComposableRenderer()` | Sí | Los fotogramas permanecen en la GPU y se presentan a WPF como un `D3DImage`. Recomendado. |
| Software (`WriteableBitmap`) | Motores X: `VideoView1.SetNativeRendering(false)`; motores clásicos: `VideoRendererMode.FrameCallback` en el motor | Sí | Carga por CPU en cada fotograma. El modo clásico `WPF_WinUI_Callback`. |
| HWND nativo | predeterminado (`VideoView1.SetNativeRendering(true)`) | No | La ruta más rápida, pero airspace bloquea cualquier superposición. |

!!! tip "¿Viene de `VideoRendererMode.WPF_WinUI_Callback`?"
    En los motores clásicos el modo se establecía en el motor: `VideoCapture1.Video_Renderer.VideoRenderer`. Los motores X multiplataforma (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`) no tienen un objeto `Video_Renderer` — el modo de renderizado se trasladó al propio control `VideoView`, de modo que una única vista funciona con cualquier motor. Consulte [Migración desde WPF_WinUI_Callback](#migracion-desde-wpf_winui_callback) más abajo.

## Renderizador componible D3D11 (recomendado)

`D3D11Composable` carga cada fotograma en una textura compartida de Direct3D 11, la abre en un dispositivo Direct3D 9Ex oculto y la expone a WPF como un `D3DImage`. El video se convierte en un elemento común del árbol visual: las transformaciones de renderizado, la opacidad, el orden Z y el recorte redondeado se aplican sobre él, y no hay ningún HWND contra el que luchar.

**Requisitos:** Windows Vista o posterior (Direct3D 9Ex) y una GPU de clase Direct3D 10/11.

### Activarlo con los motores X

Llame a `UseD3D11ComposableRenderer()` antes de iniciar la reproducción o la captura — la llamada es idempotente, así que es seguro repetirla tras reiniciar el motor:

```cs
// Antes de crear / iniciar el motor.
VideoView1.UseD3D11ComposableRenderer();

_player = new MediaPlayerCoreX(VideoView1);

var source = await UniversalSourceSettingsV2.CreateAsync(new Uri(filename));
await _player.OpenAsync(source);
await _player.PlayAsync();
```

La misma llamada funciona para `VideoCaptureCoreX` y `VideoEditCoreX`, y para un `VideoRendererBlock` construido sobre la misma vista en Media Blocks SDK .NET.

### Activarlo con los motores clásicos

`VideoCaptureCore`, `MediaPlayerCore` y `VideoEditCore` usan exactamente la misma llamada sobre la vista:

```cs
VideoView1.UseD3D11ComposableRenderer();
```

El motor pasa internamente al canal de callbacks de fotogramas y sus fotogramas se cargan en la textura compartida. Prefiera esta llamada antes que seleccionar `VideoRendererMode.D3D11Composable` en la configuración `Video_Renderer` del motor: la vista arranca en modo de renderizado nativo y, mientras siga ahí, anula la elección del motor con un HWND nativo. `UseD3D11ComposableRenderer()` se encarga de eso por usted.

### Diseño XAML

Coloque la vista y la superposición en la misma celda de un `Grid`. Los hijos posteriores se dibujan encima:

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
    x:Class="MyPlayer.MainWindow">
    <Grid>
        <WPF:VideoView x:Name="VideoView1" />

        <Border Background="#88000000" CornerRadius="8"
                VerticalAlignment="Top" HorizontalAlignment="Left"
                Margin="8" Padding="10,6">
            <TextBlock Text="LIVE" Foreground="White" FontWeight="SemiBold" />
        </Border>

        <StackPanel Orientation="Horizontal" VerticalAlignment="Bottom"
                    HorizontalAlignment="Center" Margin="0,0,0,16">
            <Button Content="Play" Width="80" Click="btPlay_Click" />
            <Button Content="Pause" Width="80" Margin="8,0,0,0" Click="btPause_Click" />
        </StackPanel>
    </Grid>
</Window>
```

Como en este modo el video es un elemento WPF normal, la propia vista también puede transformarse:

```cs
VideoView1.RenderTransformOrigin = new Point(0.5, 0.5);
VideoView1.RenderTransform = new RotateTransform(10);
VideoView1.Opacity = 0.7;
```

### Procesamiento GPU personalizado sobre el fotograma

En el modo `D3D11Composable` la vista expone la textura compartida, por lo que puede ejecutar sus propios shaders en cada fotograma sin pasar por la memoria de la CPU:

```cs
var provider = VideoView1.GetSurfaceProvider();
if (provider != null)
{
    provider.FrameReady += (sender, e) =>
    {
        // e.SharedHandle — handle compartido DXGI, ábralo en su propio ID3D11Device
        // mediante OpenSharedResource; e.Width / e.Height — tamaño de la textura.
    };
}
```

`FrameReady` se genera de forma síncrona en el hilo que envió el fotograma — un callback de DirectShow o un hilo de streaming de GStreamer, no el dispatcher de WPF, y no necesariamente el mismo hilo en cada fotograma. Haga el marshalling al dispatcher antes de tocar cualquier control WPF y mantenga fuera del manejador los recursos ligados a un hilo.

`GetSurfaceProvider()` devuelve `null` en cualquier otro modo de renderizado — una forma rápida de confirmar que el panel componible está realmente activo.

## Modo de renderizado por software

Si no puede usar Direct3D — una máquina virtual sin aceleración de GPU, una sesión de escritorio remoto o un equipo de destino muy antiguo — pase a la ruta por software. Los fotogramas se dibujan en un `WriteableBitmap` alojado en un `Image` de WPF dentro de la vista, que se compone con WPF igual de bien, a costa de una carga de CPU por fotograma.

Con los motores X, desactive el renderizado nativo en la vista antes de que arranque el motor:

```cs
VideoView1.SetNativeRendering(false);
```

En los motores clásicos el modo pertenece al motor. Establézcalo antes de `PlayAsync()` / `StartAsync()` y la vista sale del modo nativo por sí misma:

```cs
MediaPlayer1.Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback;
```

En las compilaciones anteriores a 2026.8.18 la vista permanecía en modo nativo y sustituía ese ajuste por un renderizador HWND nativo, así que añada `VideoView1.SetNativeRendering(false)` antes de la línea anterior si su objetivo es una de ellas. La llamada adicional es inofensiva en las compilaciones actuales.

El XAML anterior no cambia en ninguno de los dos casos. El renderizado por software y el panel componible son mutuamente excluyentes: una vez instalado el panel, `SetNativeRendering(true)` se ignora y se registra como advertencia.

## Por qué el renderizado HWND nativo bloquea las superposiciones

La ruta nativa entrega los fotogramas a una ventana hija Win32 alojada dentro del diseño WPF. Es la opción más rápida y el comportamiento predeterminado de la vista, pero el administrador de ventanas del escritorio compone esa ventana hija *después* de que WPF haya dibujado su propio contenido, de modo que cualquier elemento WPF situado sobre el video resulta simplemente invisible. La misma limitación se aplica al modo clásico `VideoRendererMode.WPF_NativeHWND`.

Si necesita a la vez el máximo rendimiento y una superposición, prefiera `D3D11Composable` — mantiene el fotograma residente en la GPU y aun así se compone con WPF.

## Migración desde WPF_WinUI_Callback

No se ha eliminado nada de los motores clásicos: `VideoRendererMode.WPF_WinUI_Callback` sigue existiendo, y `VideoRendererMode.FrameCallback` es un alias del mismo valor con un nombre más descriptivo. Se establece en el motor, tal como se muestra en [Modo de renderizado por software](#modo-de-renderizado-por-software).

Lo que cambia es dónde vive el ajuste cuando pasa a un motor X:

| Motor clásico | Equivalente en el motor X |
|---|---|
| `Video_Renderer.VideoRenderer = VideoRendererMode.WPF_WinUI_Callback` | `VideoView1.SetNativeRendering(false)` |
| `Video_Renderer.VideoRenderer = VideoRendererMode.D3D11Composable` | `VideoView1.UseD3D11ComposableRenderer()` (ambas familias) |
| `Video_Renderer.VideoRenderer = VideoRendererMode.WPF_NativeHWND` | comportamiento predeterminado, o `VideoView1.SetNativeRendering(true)` |

Para el acceso por fotograma a los datos de píxeles — visión por computador, inferencia de IA, dibujo personalizado — consulte [Efectos de video personalizados mediante eventos de fotograma](custom-video-effects.md) e [Dibujo de imágenes mediante OnVideoFrameBuffer](image-onvideoframebuffer.md).

## Solución de problemas

**Se abre una ventana de video independiente en lugar de renderizar dentro del diseño.** La vista está ejecutando la ruta HWND mientras su código espera el panel componible. Llame a `UseD3D11ComposableRenderer()` *antes* de asociar el motor y compruebe que `GetSurfaceProvider()` devuelve un resultado distinto de `null` una vez iniciada la reproducción.

**La superposición es invisible y el video se reproduce.** La vista está en modo de renderizado nativo. Llame a `UseD3D11ComposableRenderer()` o, para la ruta por software, a `SetNativeRendering(false)` en un motor X, o a `Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback` en uno clásico. Todas deben ejecutarse antes de iniciar la reproducción, así que reinicie la vista previa para que el cambio surta efecto.

**La vista previa se queda en negro en una sesión de escritorio remoto o de máquina virtual.** Puede que la aceleración Direct3D no esté disponible allí — recurra a la ruta por software descrita más arriba.

## Demos

- **[Simple Player Demo D3D11 (motor X)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF/Simple%20Player%20Demo%20D3D11)** — `MediaPlayerCoreX` con un banner WPF sobre el video, además de rotación y opacidad de la vista.
- **[Simple Player Demo D3D11 (motor clásico)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Simple%20Player%20Demo%20D3D11)** — el mismo renderizador gobernado por `MediaPlayerCore`, basado en DirectShow.

## Páginas relacionadas

- [Selección del renderizador de video (WinForms)](select-video-renderer-winforms.md) — todos los modos de renderizado de los motores clásicos.
- [Salida de video multipantalla en WPF](multiple-screens-wpf.md) — varias superficies de vista previa independientes en una misma aplicación.
- [Imagen personalizada en VideoView](video-view-set-custom-image.md) — sustituir la vista previa por una imagen estática.
