---
title: Edición de Video en .NET MAUI — Android, iOS, Windows
description: Une y renderiza video en Android, iOS, macCatalyst y Windows desde un único código .NET MAUI con VisioForge Video Edit SDK y el motor VideoEditCoreX.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Android
  - iOS
  - MAUI
  - Editing
  - Encoding
  - MP4
  - H.264
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoView
  - IVideoView
  - MP4Output
  - ProgressEventArgs

---

# Edición de Video en Aplicaciones .NET MAUI

## Introducción

`VideoEditCoreX` es el motor de línea de tiempo multiplataforma del Video Edit SDK .NET. Se
ejecuta sobre el backend de GStreamer, así que el mismo código de edición compila y se ejecuta en
Android, iOS, macCatalyst y Windows desde un único proyecto .NET MAUI — la superficie de vista
previa es el control `VideoView` compartido, y la API de línea de tiempo es idéntica en cada
plataforma.

Esta guía construye el editor útil más pequeño posible: elija clips de la galería del
dispositivo, agréguelos a la línea de tiempo, previsualice el resultado y luego renderícelo a un
archivo MP4.

El ejemplo completo está en [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit).

## Requisitos previos

- Cargas de trabajo de .NET MAUI para las plataformas de destino (`maui-android`, `maui-ios`, `maui-maccatalyst`).
- Una licencia de VisioForge, o la prueba de 30 días.
- En iOS y macCatalyst, una cadena `NSPhotoLibraryUsageDescription` en `Info.plist` — el selector
  de galería es lo que alimenta la línea de tiempo.

## Paquetes NuGet

Dos paquetes: el propio SDK y el control `VideoView` de MAUI.

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.9.17" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.9.17" />
```

`VisioForge.DotNet.Core.UI.MAUI` solo proporciona el control `VideoView` — por sí solo no puede
compilar `VideoEditCoreX`. El paquete `VisioForge.DotNet.VideoEdit` es el que incorpora el motor.

Luego agregue el redistribuible para cada plataforma que compile. Consulta
[Instalación en aplicaciones MAUI](../../install/maui.md) para el desglose completo por
plataforma, incluida la referencia al proyecto de bindings Java de Android, que requieren las
compilaciones para Android.

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.7.27" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <!-- La versión del redistribuable de iOS va por detrás de la versión del SDK a propósito - sigue
       el ciclo de reconstrucción de GStreamer-iOS, no el lanzamiento del wrapper. -->
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.12.0" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2026.8.5" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
```

## MauiProgram.cs

Registra los handlers de VisioForge para que `VideoView` resuelva a su implementación nativa:

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

## Diseño XAML

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI">
    <Grid RowDefinitions="*,Auto">
        <my:VideoView Grid.Row="0" x:Name="videoView" Background="Black"
                      HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" />
        <VerticalStackLayout Grid.Row="1">
            <ProgressBar x:Name="pbProgress" />
            <Button x:Name="btAdd" Text="ADD CLIP" Clicked="btAdd_Clicked" />
            <Button x:Name="btPreview" Text="PREVIEW" Clicked="btPreview_Clicked" />
            <Button x:Name="btRender" Text="RENDER MP4" Clicked="btRender_Clicked" />
        </VerticalStackLayout>
    </Grid>
</ContentPage>
```

## Creando el Motor

`AddVisioForgeHandlers` registra el control. **No** carga el stack nativo — eso es una llamada
aparte, y debe ocurrir antes de construir el primer `VideoEditCoreX`. Si se omite, el constructor
lanza `DllNotFoundException` en una máquina limpia.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.VideoEditX;

private VideoEditCoreX _core;

private async void MainPage_Loaded(object sender, EventArgs e)
{
    // Carga el stack nativo de GStreamer. La primera llamada construye el registro de
    // plugins y puede tardar cientos de milisegundos, así que use la forma asíncrona
    // fuera del hilo de la UI.
    await VisioForgeX.InitSDKAsync();

    IVideoView vv = videoView.GetVideoView();
    _core = new VideoEditCoreX(vv);

    _core.OnError += Core_OnError;
    _core.OnProgress += Core_OnProgress;
    _core.OnStop += Core_OnStop;
}
```

!!! warning "Mantenga los controles inertes hasta que exista el motor"
    `InitSDKAsync` se ejecuta en el grupo de subprocesos y un arranque en frío dedica cientos de
    milisegundos a construir el registro de GStreamer. `MainPage_Loaded` es `async void`, así que
    la página es interactiva durante toda esa ventana y cualquier controlador que use `_core`
    encontraría una referencia nula. Deshabilite el panel de controles en el constructor y
    habilítelo cuando el motor esté construido, y envuelva el cuerpo en un `try`/`catch` para que
    un fallo de inicialización deje los controles apagados con el motivo en pantalla. El ejemplo
    hace ambas cosas.

Libérelo al salir:

```csharp
private void MainPage_Unloaded(object sender, EventArgs e)
{
    _core?.Stop();
    _core?.Dispose();
    _core = null;

    VisioForgeX.DestroySDK();
}
```

## Construyendo la Línea de Tiempo

Cada llamada `Input_Add*` agrega al final de la línea de tiempo. Use
`Input_AddAudioVideoFile` para un clip normal, `Input_AddAudioFile` para una pista de solo audio
y `Input_AddImageFile` para una imagen fija con una duración explícita.

```csharp
private async void btAdd_Clicked(object sender, EventArgs e)
{
    var picked = await MediaPicker.Default.PickVideoAsync();
    if (picked == null)
    {
        return;
    }

    _core.Input_AddAudioVideoFile(await ResolveLocalPathAsync(picked));
}
```

`Input_Clear_List()` vacía la línea de tiempo.

!!! warning "El selector no siempre entrega una ruta de archivo"
    En Android, `MediaPicker` copia el recurso a una carpeta de caché, así que `FullPath` es
    absoluta y se puede usar tal cual. En iOS, `PHPicker` entrega a MAUI un `NSItemProvider` y
    solo el nombre del archivo original sobrevive en `FullPath` — el stream debe copiarse antes a
    su propia caché. El `ResolveLocalPathAsync` del ejemplo cubre ambos casos.

## Vista Previa y Renderizado

El motor tiene un único interruptor entre los dos modos: `Output_Format`. Un valor nulo reproduce
la línea de tiempo en el `VideoView`; un objeto de formato la renderiza a un archivo.

```csharp
private void btPreview_Clicked(object sender, EventArgs e)
{
    _core.Output_Format = null;
    _core.Start();
}

private void btRender_Clicked(object sender, EventArgs e)
{
    var output = Path.Combine(FileSystem.Current.AppDataDirectory, "joined.mp4");

    _core.Output_Format = new MP4Output(output, H264EncoderBlock.GetDefaultSettings());
    _core.Start();
}
```

!!! warning "`new MP4Output(path)` por sí solo entrega H.264 por software fuera de macOS y Android"
    El valor por defecto de `MP4Output` elige `AppleMediaH264EncoderSettings` en macOS y llama a
    `H264EncoderBlock.GetDefaultSettings()` en Android — pero iOS, Mac Catalyst y Windows caen
    todos en `OpenH264EncoderSettings`, un codificador por software.

    `H264EncoderBlock.GetDefaultSettings()` es el selector de plataforma del propio SDK y
    prefiere hardware en todas partes: VideoToolbox en Apple, MediaCodec en Android, y luego
    NVENC, AMF y QSV antes de recurrir al software. Páselo de forma explícita para mantener
    iOS, Mac Catalyst y Windows en hardware.

    Reside en el espacio de nombres `VisioForge.Core.MediaBlocks.VideoEncoders`, pero es un
    ayudante compartido y no un pipeline de Media Blocks — `MP4Output` lo llama internamente.

    Deje el codificador de audio sin especificar: `MP4Output` elige AAC en Windows y MP3 en el
    resto.

Escribe en un directorio propio de la app, como `FileSystem.Current.AppDataDirectory`. No
requiere permiso en tiempo de ejecución en ninguna plataforma, a diferencia del almacenamiento
multimedia compartido de Android.

## Progreso y Finalización

`OnProgress` reporta 0–100 y `OnStop` reporta el éxito. Ambos se disparan en un hilo en segundo
plano, así que pasa al hilo de la UI antes de tocar los controles:

```csharp
private void Core_OnProgress(object sender, ProgressEventArgs e)
{
    MainThread.BeginInvokeOnMainThread(() => pbProgress.Progress = e.Progress / 100.0);
}

private void Core_OnStop(object sender, StopEventArgs e)
{
    MainThread.BeginInvokeOnMainThread(() =>
        DisplayAlert("Render", e.Successful ? "Completed" : "Failed", "OK"));
}
```

## Notas por Plataforma

- **Android** necesita la referencia al proyecto de bindings Java. Sin ella, la app compila pero
  el stack nativo falla al inicializarse en tiempo de ejecución.
- **iOS y macCatalyst** necesitan `<UseInterpreter>true</UseInterpreter>`. La conversión de tipos
  XAML de MAUI usa `DynamicMethod`, que requiere JIT y lanza `ExecutionEngineException` en modo
  solo AOT.
- Las compilaciones de **Windows** resuelven los redistribuibles a través de
  `VisioForge.CrossPlatform.Core.Windows.x64`.

## Aplicaciones de Ejemplo

- **[SimpleEdit (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit)** — la app que construye esta guía.
- **[Video Join Demo X (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/WPF/CSharp/Video%20Join%20Demo%20X)** — la misma API de línea de tiempo en escritorio, con opciones de salida por formato.

## Ver También

- [Instalación en aplicaciones MAUI](../../install/maui.md)
- [Hoja de referencia de Video Edit SDK](../cheat-sheet.md)
- [Transiciones](../transitions.md)
- [Despliegue en Android](../../deployment-x/Android.md)
- [Despliegue en iOS](../../deployment-x/iOS.md)
