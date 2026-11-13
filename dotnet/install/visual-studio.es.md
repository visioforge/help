---
title: Cómo instalar los SDK .NET de VisioForge en Visual Studio
description: "Instale los SDK .NET de VisioForge en Visual Studio desde NuGet, en proyectos WinForms, WPF, MAUI y de consola: los paquetes, VideoView y un vídeo."
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Android
  - iOS
  - WinForms
  - WPF
  - MAUI
  - NuGet
primary_api_classes:
  - VideoView
  - VisioForgeX
video:
  id: o6wL9r6t6Lk
  upload_date: 2026-11-11T07:53:04Z
  duration: PT8M13S

---

# Instalar los SDK .NET de VisioForge en Visual Studio

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta guía instala los SDK .NET de VisioForge en Visual Studio desde NuGet, en cuatro tipos de proyecto: WinForms, WPF, .NET MAUI y una aplicación de consola. Cada parte termina con un proyecto vacío que se compila y se ejecuta.

Los cuatro SDK — Video Capture SDK .NET, Media Player SDK .NET, Video Edit SDK .NET y Media Blocks SDK .NET — vienen en un solo paquete NuGet, [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core). Junto a él va un paquete redist, el motor nativo de la plataforma. En Windows x64 es [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64). Una aplicación MAUI añade también [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI).

## Tutorial en vídeo

El vídeo muestra los cuatro tipos de proyecto en Visual Studio 2026:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/o6wL9r6t6Lk?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Requisitos

- Windows 10 u 11
- Visual Studio 2026, o Visual Studio 2022 para proyectos de .NET 8 y .NET 9
- La carga de trabajo **.NET desktop development** para WinForms, WPF y consola, y la carga de trabajo **.NET Multi-platform App UI development** para MAUI

## Los paquetes

| Proyecto | Paquetes |
|---|---|
| WinForms, WPF, consola (Windows x64) | `VisioForge.DotNet.Core`, `VisioForge.CrossPlatform.Core.Windows.x64` |
| .NET MAUI | `VisioForge.DotNet.Core`, `VisioForge.DotNet.Core.UI.MAUI`, más un redist por plataforma en el archivo de proyecto |

Fuera de Visual Studio, `dotnet add package` instala los mismos paquetes desde la línea de comandos:

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
```

También se publican paquetes por producto, como `VisioForge.DotNet.VideoCapture`; hacen referencia a `VisioForge.DotNet.Core`, que es todo lo que un proyecto necesita.

## WinForms

1. En la ventana de inicio, haga clic en **Create a new project**, busque *Windows Forms* y elija **Windows Forms App** para C#.
2. Asigne un nombre al proyecto, elija **.NET 10** y haga clic en **Create**.
3. Haga clic derecho en el proyecto en el **Solution Explorer** y abra **Manage NuGet Packages**, en la pestaña **Browse**.
4. Busque `VisioForge.DotNet.Core` e instálelo. Visual Studio muestra los cambios y la licencia: acéptelos.
5. Instale `VisioForge.CrossPlatform.Core.Windows.x64` de la misma forma. Ahora ambos paquetes están en la pestaña **Installed**.

![El administrador de paquetes NuGet con VisioForge.DotNet.Core](vs-nuget.webp)

El paquete también pone `VideoView` en el **Toolbox**, bajo **VisioForge**. Búsquelo y dibújelo en el formulario: aquí es donde el SDK muestra el vídeo. Inicie la aplicación sin depurar (Ctrl+F5) y el formulario se abre con el `VideoView`, en negro hasta que un motor dibuje en él.

![VideoView en el Toolbox, dibujado en el formulario](vs-winforms-videoview.webp)

## WPF

Cree una **WPF Application** para C# con .NET 10 e instale los mismos dos paquetes. Después arrastre `VideoView` desde el **Toolbox** al `Grid` de `MainWindow.xaml`. Visual Studio escribe la etiqueta y, con ella, el espacio de nombres:

```xml
<Window xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core" x:Class="VisioForgeWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:VisioForgeWpf"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <WPF:VideoView />
    </Grid>
</Window>
```

El espacio de nombres es `VisioForge.Core.UI.WPF`, en el ensamblado `VisioForge.Core`. El prefijo lo elige usted: el Toolbox escribe `WPF`, y `vf` es más corto.

![VideoView colocado en MainWindow.xaml](vs-wpf-xaml.webp)

## .NET MAUI

Un proyecto .NET MAUI App compila para Windows, Mac Catalyst, Android e iOS. Desde NuGet, instale `VisioForge.DotNet.Core` y `VisioForge.DotNet.Core.UI.MAUI`, que contiene el `VideoView` de MAUI.

Los motores nativos van, en cambio, en el archivo de proyecto, cada uno bajo la condición de su plataforma, de modo que cada destino incluye solo el motor de su propia plataforma. Abra **Edit Project File** y añádalos al final:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.12]" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="[2026.11.5]" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="[2026.11.5]" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="[2026.11.5]" />
</ItemGroup>
```

En un Mac, Mac Catalyst compila para el procesador del propio Mac:

```xml
<!-- Mac Catalyst: compilar para la CPU del propio Mac -->
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'X64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="$([MSBuild]::IsOSPlatform('osx')) AND '$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)' == 'Arm64' AND $(TargetFramework.Contains('-maccatalyst'))">
  <RuntimeIdentifier>maccatalyst-arm64</RuntimeIdentifier>
</PropertyGroup>
```

Dos plataformas necesitan algo más:

- **Android** añade además el proyecto de enlace de VisioForge para Android de la [carpeta AndroidDependency](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency), y los permisos de cámara y micrófono en su manifiesto.
- **iOS y Mac Catalyst** necesitan descripciones de uso (`NSCameraUsageDescription`, `NSMicrophoneUsageDescription`) en `Info.plist`. iOS se prueba en un dispositivo, no en el simulador.

La [guía de instalación de MAUI](maui.md) cubre ambos casos en detalle.

En `MauiProgram.cs`, añada dos usings y luego `UseSkiaSharp` y los handlers de VisioForge al builder. `UseSkiaSharp` activa SkiaSharp, que usan las vistas de VisioForge, y `AddVisioForgeHandlers` registra sus handlers de plataforma:

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    });
```

En `MainPage.xaml`, declare el espacio de nombres de VisioForge — `VisioForge.Core.UI.MAUI`, en el ensamblado `VisioForge.Core.UI.MAUI` — y ponga un `VideoView` en la página:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
             x:Class="VisioForgeMaui.MainPage">
    <ScrollView>
        <VerticalStackLayout Padding="30,0" Spacing="25">
            <vf:VideoView HeightRequest="300" />
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

Ejecútelo en **Windows Machine**, y el `VideoView` queda encima del contenido de la plantilla.

![La aplicación MAUI en ejecución en Windows](vs-maui-running.webp)

## Aplicación de consola

Cree una **Console App** para C# e instale los mismos dos paquetes que para WinForms. Una aplicación de consola necesita un paso más: su plantilla apunta a `net10.0`, y esa compilación del SDK es la de Linux. Sin `-windows`, la aplicación busca la biblioteca GStreamer de Linux y falla con una `DllNotFoundException`. Abra **Edit Project File** y añada `-windows` al framework de destino:

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <!-- net10.0 es la compilación del SDK para Linux: en Windows, añada -windows -->
  <TargetFramework>net10.0-windows</TargetFramework>
</PropertyGroup>
```

Los proyectos WinForms y WPF ya apuntan a `net10.0-windows`.

`Program.cs` inicializa el SDK, muestra si está listo y lo libera de nuevo:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
Console.WriteLine($"VisioForge SDK initialized: {VisioForgeX.IsSDKInitialized}");
VisioForgeX.DestroySDK();
```

La última línea de la salida informa de que el SDK se inicializó.

![La aplicación de consola informa de que el SDK se inicializó](vs-console.webp)

## Otros paquetes

- **Motores solo para Windows.** `VideoCaptureCore`, `MediaPlayerCore` y `VideoEditCore` usan en su lugar los redist clásicos: `VisioForge.DotNet.Core.Redist.VideoCapture.x64`, `…MediaPlayer.x64` o `…VideoEdit.x64`. Consulte las páginas de implementación de [Video Capture SDK .NET](../videocapture/deployment.md), [Media Player SDK .NET](../mediaplayer/deployment.md) y [Video Edit SDK .NET](../videoedit/deployment.md).
- **x86 y ARM64.** Una aplicación compilada para otra CPU usa el redist de esa CPU, como `VisioForge.CrossPlatform.Core.Windows.x86` o `VisioForge.CrossPlatform.Core.Windows.arm64`.
- **Otros frameworks de interfaz.** WinUI, Avalonia y Uno tienen cada uno su propio paquete de interfaz: `VisioForge.DotNet.Core.UI.WinUI`, `VisioForge.DotNet.Core.UI.Avalonia` y `VisioForge.DotNet.Core.UI.Uno`.

La [guía de implementación](../deployment-x/index.md) enumera los paquetes redist de cada plataforma.

## Instalación manual

Cuando NuGet no es una opción, el SDK también se distribuye como instalador:

1. Descargue el [instalador del SDK](https://files.visioforge.com/trials/visioforge_sdks_installer_dotnet_setup.exe) y ejecútelo.
2. En su proyecto, añada una referencia al `VisioForge.Core.dll` instalado.
3. Para tener `VideoView` en el Toolbox, haga clic derecho en el Toolbox, seleccione **Choose Items...** y busque `VisioForge.Core.dll`.

NuGet es la forma recomendada: trae el motor nativo con el paquete y se actualiza con un clic.

## Transcripción del vídeo

### Introducción

Instalemos los SDK .NET de VisioForge en Visual Studio, en cuatro tipos de proyecto: WinForms, WPF, MAUI y una aplicación de consola. Los cuatro SDK .NET de VisioForge vienen en un solo paquete NuGet, VisioForge.DotNet.Core: Video Capture, Media Player, Video Edit y Media Blocks. Junto a él va un paquete redist, el motor nativo de la plataforma. En Windows x64, es VisioForge.CrossPlatform.Core.Windows.x64. Una aplicación MAUI añade también el paquete de interfaz de MAUI. Necesita Windows 10 u 11, y Visual Studio 2026, o 2022 para proyectos de .NET 8 y 9. Las aplicaciones WinForms, WPF y de consola necesitan la carga de trabajo .NET desktop development, y MAUI necesita la carga de trabajo .NET Multi-platform App UI. Este video usa Visual Studio 2026 y .NET 10.

### WinForms

Empiece con WinForms. En la ventana de inicio, haga clic en Create a new project, busque Windows Forms y elija Windows Forms App para C#. Asigne un nombre al proyecto, aquí VisioForgeWinForms, elija .NET 10 y haga clic en Create. Haga clic derecho en el proyecto en el Solution Explorer y abra Manage NuGet Packages, en la pestaña Browse. Busque VisioForge.DotNet.Core e instálelo. Visual Studio muestra los cambios y la licencia: acéptelos. Después instale VisioForge.CrossPlatform.Core.Windows.x64, el motor nativo para Windows x64. Ahora ambos paquetes están en la pestaña Installed. El paquete también pone VideoView en el Toolbox, bajo VisioForge. Búsquelo y dibújelo en el formulario. Aquí es donde el SDK muestra el video. Inícielo sin depurar. El formulario se abre con el VideoView, en negro hasta que un motor dibuje en él.

### WPF

WPF sigue los mismos pasos. Cree una WPF Application para C#, aquí VisioForgeWpf, con .NET 10. Instale los mismos dos paquetes desde NuGet: VisioForge.DotNet.Core y el redist de Windows x64. Arrastre VideoView desde el Toolbox al Grid de MainWindow.xaml. Visual Studio escribe la etiqueta y, con ella, el espacio de nombres. El espacio de nombres es VisioForge.Core.UI.WPF, en el ensamblado VisioForge.Core. El prefijo lo elige usted: el Toolbox escribe WPF, y vf es más corto. Ejecútelo, y la ventana muestra el VideoView.

### MAUI

Ahora MAUI. Cree una .NET MAUI App, aquí VisioForgeMaui. Un solo proyecto compila para Windows, Mac Catalyst, Android e iOS. Desde NuGet, instale VisioForge.DotNet.Core y VisioForge.DotNet.Core.UI.MAUI, que contiene el VideoView de MAUI. Los motores nativos van, en cambio, en el archivo de proyecto, cada uno bajo la condición de su plataforma. Abra Edit Project File y añádalos al final. El destino Windows lleva el redist de Windows x64, y Mac Catalyst, Android e iOS llevan cada uno el suyo. Cada destino incluye solo el motor nativo de su plataforma. En un Mac, Mac Catalyst compila para el procesador del propio Mac: Intel o Apple silicon elige su RuntimeIdentifier. Dos plataformas necesitan algo más. Android añade además el proyecto de enlace de VisioForge para Android, de la carpeta AndroidDependency en GitHub, y los permisos de cámara y micrófono en su manifiesto. iOS y Mac Catalyst necesitan descripciones de uso en Info.plist, e iOS se prueba en un dispositivo, no en el simulador. En MauiProgram.cs, añada dos usings y luego UseSkiaSharp y los handlers de VisioForge al builder. UseSkiaSharp activa SkiaSharp, que usan las vistas de VisioForge, y AddVisioForgeHandlers registra sus handlers de plataforma. En MainPage.xaml, declare el espacio de nombres de VisioForge y ponga un VideoView en la parte superior de la página. En MAUI, el espacio de nombres es VisioForge.Core.UI.MAUI, en el ensamblado VisioForge.Core.UI.MAUI. Ejecútelo en Windows Machine. El VideoView queda encima del contenido de la plantilla.

### Consola

Por último, una aplicación de consola. Cree una Console App para C#, aquí VisioForgeConsole. Instale los mismos dos paquetes que para WinForms. Un paso más para una aplicación de consola. Su plantilla apunta a net10.0, y esa compilación del SDK es la de Linux. Añada -windows al framework de destino. Sin eso, la aplicación busca la biblioteca GStreamer de Linux y falla con una DllNotFoundException. WinForms y WPF ya apuntan a net10.0-windows. Program.cs inicializa el SDK, muestra si está listo y lo libera de nuevo. Ejecútelo. La última línea informa de que el SDK se inicializó.

### Otros paquetes

Algunos paquetes más. Los motores solo para Windows, VideoCaptureCore, MediaPlayerCore y VideoEditCore, usan los redist clásicos: Redist.VideoCapture, MediaPlayer o VideoEdit. Las aplicaciones x86 y ARM64 usan el redist de su CPU. Y WinUI, Avalonia y Uno tienen cada uno su propio paquete de interfaz. Fuera de Visual Studio, dotnet add package instala los mismos dos paquetes desde la línea de comandos.

### Próximos pasos

A continuación, grabe una webcam en MP4 en el tutorial de WinForms o WPF. Las guías de despliegue enumeran los paquetes redist de cada plataforma, y hay guías de instalación para Rider, Avalonia, Uno y Unity. La documentación está en visioforge.com/help. Gracias por ver el video.

## Consulte también

- [Webcam a MP4 en WinForms](../videocapture/video-tutorials/video-capture-webcam-mp4.md) y [en WPF](../videocapture/video-tutorials/video-capture-webcam-mp4-wpf.md) — el siguiente paso tras la instalación
- [Inicialización del SDK](../init.md) — `InitSDKAsync` y `DestroySDK` en detalle
- Guías de instalación para [MAUI](maui.md), [Rider](rider.md), [Avalonia](avalonia.md), [Uno](uno.md) y [Unity](unity.md)

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
