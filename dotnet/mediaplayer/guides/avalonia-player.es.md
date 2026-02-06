# Reproductor Multimedia Avalonia con VisioForge

Esta guía le guiará a través del proceso de construcción de una aplicación de reproductor multimedia multiplataforma utilizando Avalonia UI con el patrón Modelo-Vista-Modelo de Vista (MVVM) y el SDK de Reproductor Multimedia de VisioForge. La aplicación será capaz de reproducir archivos de video en Windows, macOS, Linux, Android e iOS.

Haremos referencia al proyecto de ejemplo `SimplePlayerMVVM`, que demuestra los conceptos centrales y los detalles de implementación.

## 1. Requisitos Previos

Antes de comenzar, asegúrese de tener instalado lo siguiente:

* .NET SDK (última versión, ej., .NET 8 o más reciente)
* Un IDE como Visual Studio, JetBrains Rider o VS Code con extensiones de C# y Avalonia.
* Para desarrollo en Android:
  * Android SDK
  * Java Development Kit (JDK)
* Para desarrollo en iOS (requiere una máquina macOS):
  * Xcode
  * Perfiles de aprovisionamiento y certificados necesarios.
* VisioForge .NET SDK (MediaPlayer SDK X). Puede obtenerlo desde el sitio web de VisioForge. Los paquetes necesarios se agregarán a través de NuGet.

## 2. Configuración del Proyecto

Esta sección describe cómo configurar la estructura de la solución e incluir los paquetes necesarios del SDK de VisioForge.

### 2.1. Estructura de la Solución

La solución `SimplePlayerMVVM` consta de varios proyectos:

* **SimplePlayerMVVM**: Una biblioteca .NET Standard que contiene la lógica central de la aplicación, incluyendo ViewModels, Views (AXAML) e interfaces compartidas. Este es el proyecto principal donde reside la mayor parte de nuestra lógica de aplicación.
* **SimplePlayerMVVM.Android**: El proyecto principal específico para Android.
* **SimplePlayerMVVM.Desktop**: El proyecto principal específico para escritorio (Windows, macOS, Linux).
* **SimplePlayerMVVM.iOS**: El proyecto principal específico para iOS.

### 2.2. Proyecto Central (`SimplePlayerMVVM.csproj`)

El proyecto principal, `SimplePlayerMVVM.csproj`, apunta a múltiples plataformas. Las referencias de paquetes clave incluyen:

* `Avalonia`: El marco de trabajo central de Avalonia UI.
* `Avalonia.Themes.Fluent`: Proporciona un tema de Diseño Fluido.
* `Avalonia.ReactiveUI`: Para soporte MVVM usando ReactiveUI.
* `VisioForge.DotNet.MediaBlocks`: Componentes centrales de procesamiento de medios de VisioForge.
* `VisioForge.DotNet.Core.UI.Avalonia`: Componentes de UI de VisioForge para Avalonia, incluyendo `VideoView`.

```xml
<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup>
  <Nullable>enable</Nullable>
  <LangVersion>latest</LangVersion>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
 </PropertyGroup>
 <ItemGroup>
  <AvaloniaResource Include="Assets\**" />
 </ItemGroup>
 <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-windows</TargetFrameworks>
 </PropertyGroup>
 <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
  <TargetFrameworks>net8.0-android;net8.0-ios;net8.0-macos14.0</TargetFrameworks>
 </PropertyGroup>
 <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Linux'))">
  <TargetFrameworks>net8.0-android;net8.0</TargetFrameworks>
 </PropertyGroup>
 <ItemGroup>
  <AvaloniaResource Include="Assets\**" />
 </ItemGroup>
 <ItemGroup>
  <PackageReference Include="Avalonia" Version="11.3.0" />
  <PackageReference Include="Avalonia.Themes.Fluent" Version="11.3.0" />
  <PackageReference Include="Avalonia.Fonts.Inter" Version="11.3.0" />
  <!--La condición a continuación es necesaria para eliminar el paquete Avalonia.Diagnostics de la salida de compilación en la configuración Release.-->
  <PackageReference Condition="'$(Configuration)' == 'Debug'" Include="Avalonia.Diagnostics" Version="11.3.0" />
  <PackageReference Include="Avalonia.ReactiveUI" Version="$(AvaloniaVersion)" />
 </ItemGroup>
 <ItemGroup Condition="'$(TargetFramework)' == 'net8.0-android'">
  <PackageReference Include="Avalonia.Android" Version="$(AvaloniaVersion)" />
 </ItemGroup>
 <ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.1" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2025.5.1" />
 </ItemGroup>
</Project>
```

Esta configuración permite que la lógica central se comparta entre todas las plataformas de destino.

### 2.3. Proyectos Específicos de Plataforma

Cada proyecto principal de plataforma (`SimplePlayerMVVM.Android.csproj`, `SimplePlayerMVVM.Desktop.csproj`, `SimplePlayerMVVM.iOS.csproj`) incluye dependencias y configuraciones específicas de la plataforma.

**Escritorio (`SimplePlayerMVVM.Desktop.csproj`):**

* Referencia `Avalonia.Desktop`.
* Incluye bibliotecas nativas de VisioForge específicas de la plataforma (ej., `VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Core.macOS`).

```xml
  <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
    <TargetFramework>net8.0-windows</TargetFramework>
    <OutputType>WinExe</OutputType>
  </PropertyGroup>
  <ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
    <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
  </ItemGroup>
  <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
    <TargetFramework>net8.0-macos14.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
  <ItemGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
    <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.2.15" />
  </ItemGroup>
  <PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Linux'))">
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>
```

**Android (`SimplePlayerMVVM.Android.csproj`):**

* Referencia `Avalonia.Android`.
* Incluye bibliotecas de VisioForge específicas de Android y dependencias como `VisioForge.CrossPlatform.Core.Android`.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0-android</TargetFramework>
    <SupportedOSPlatformVersion>21</SupportedOSPlatformVersion>
    <Nullable>enable</Nullable>
    <ApplicationId>com.CompanyName.Simple_Player_MVVM</ApplicationId>
    <ApplicationVersion>1</ApplicationVersion>
    <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
    <AndroidPackageFormat>apk</AndroidPackageFormat>
    <AndroidEnableProfiledAot>false</AndroidEnableProfiledAot>
  </PropertyGroup>
  <!-- ... otros elementos ... -->
  <ItemGroup>
    <ProjectReference Include="..\..\..\..\AndroidDependency\VisioForge.Core.Android.X8.csproj" />
    <ProjectReference Include="..\SimplePlayerMVVM\SimplePlayerMVVM.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />
  </ItemGroup>
</Project>
```

**iOS (`SimplePlayerMVVM.iOS.csproj`):**

* Referencia `Avalonia.iOS`.
* Incluye bibliotecas de VisioForge específicas de iOS como `VisioForge.CrossPlatform.Core.iOS`.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0-ios</TargetFramework>
    <SupportedOSPlatformVersion>13.0</SupportedOSPlatformVersion>
    <Nullable>enable</Nullable>
    <RootNamespace>Simple_Player_MVVM.iOS</RootNamespace>
    <ApplicationId>com.visioforge.avaloniaplayer</ApplicationId>
  </PropertyGroup>
  <!-- ... otros elementos ... -->
  <ItemGroup>
    <PackageReference Include="Avalonia.iOS" Version="$(AvaloniaVersion)" />
    <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\SimplePlayerMVVM\SimplePlayerMVVM.csproj" />
  </ItemGroup>
</Project>
```

Estos archivos de proyecto son cruciales para gestionar dependencias y configuraciones de compilación para cada plataforma.

## 3. Estructura MVVM Central

La aplicación sigue el patrón MVVM, separando la UI (Vistas) de la lógica (Modelos de Vista) y los datos (Modelos). Se utiliza ReactiveUI para facilitar este patrón.

### 3.1. `ViewModelBase.cs`

Esta clase abstracta sirve como base para todos los ViewModels en la aplicación. Hereda de `ReactiveObject`, que es parte de ReactiveUI y proporciona la infraestructura necesaria para notificaciones de cambio de propiedad.

```csharp
using ReactiveUI;

namespace Simple_Player_MVVM.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
    }
}
```

Cualquier ViewModel que necesite notificar a la UI sobre cambios de propiedad debe heredar de `ViewModelBase`.

### 3.2. `ViewLocator.cs`

La clase `ViewLocator` es responsable de localizar e instanciar Vistas basadas en el tipo de su ViewModel correspondiente. Implementa la interfaz `IDataTemplate` de Avalonia.

```csharp
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Simple_Player_MVVM.ViewModels;
using System;

namespace Simple_Player_MVVM
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is null)
                return null;

            var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
```

Cuando Avalonia necesita mostrar un ViewModel, el método `Match` de `ViewLocator` verifica si el objeto de datos es un `ViewModelBase`. Si lo es, el método `Build` intenta encontrar una Vista correspondiente reemplazando "ViewModel" con "View" en el nombre de la clase del ViewModel y la instancia.

Este enfoque basado en convenciones simplifica la asociación entre Vistas y ViewModels.

### 3.3. Inicialización de la Aplicación (`App.axaml` y `App.axaml.cs`)

El archivo `App.axaml` define los recursos a nivel de aplicación, incluyendo el `ViewLocator` como una plantilla de datos y el tema (FluentTheme).

**`App.axaml`**:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:Simple_Player_MVVM"
             x:Class="Simple_Player_MVVM.App"
             RequestedThemeVariant="Default">
    <Application.DataTemplates>
        <local:ViewLocator/>
    </Application.DataTemplates>

    <Application.Styles>
        <FluentTheme />
    </Application.Styles>
</Application>
```

**`App.axaml.cs`**:
El archivo `App.axaml.cs` maneja la inicialización y el ciclo de vida de la aplicación.

Responsabilidades clave en `OnFrameworkInitializationCompleted`:

1. Crea una instancia de `MainViewModel`.
2. Configura la ventana o vista principal basada en el tiempo de vida de la aplicación (`IClassicDesktopStyleApplicationLifetime` para escritorio, `ISingleViewApplicationLifetime` para vistas móviles/web).
3. Asigna la instancia de `MainViewModel` como el `DataContext` para la ventana/vista principal.
4. Recupera la instancia de `IVideoView` desde `MainView` (alojada dentro de `MainWindow` o directamente como `MainView`).
5. Pasa el `IVideoView` y el control `TopLevel` (necesario para diálogos de archivo y otras interacciones de nivel superior) al `MainViewModel`.

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Simple_Player_MVVM.ViewModels;
using Simple_Player_MVVM.Views;
using VisioForge.Core.Types;

namespace Simple_Player_MVVM
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            IVideoView videoView = null;
            var model = new MainViewModel();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = model
                };

                videoView = (desktop.MainWindow as MainWindow).GetVideoView();
                model.VideoViewIntf = videoView;
                model.TopLevel = desktop.MainWindow;
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = model
                };

                videoView = (singleViewPlatform.MainView as MainView).GetVideoView();
                model.VideoViewIntf = videoView;
                model.TopLevel = TopLevel.GetTopLevel(singleViewPlatform.MainView);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
```

Esta configuración asegura que el `MainViewModel` tenga acceso a los componentes de UI necesarios para la reproducción de video e interacción, independientemente de la plataforma.

## 4. Implementación de MainViewModel (`MainViewModel.cs`)

El `MainViewModel` es central para la funcionalidad del reproductor multimedia. Gestiona el estado del reproductor, maneja las interacciones del usuario y se comunica con el motor `MediaPlayerCoreX` de VisioForge.

Componentes clave de `MainViewModel`:

### 4.1. Propiedades para Enlace de UI

El ViewModel expone varias propiedades que están enlazadas a elementos de UI en `MainView.axaml`. Estas propiedades usan `RaiseAndSetIfChanged` de `ReactiveUI` para notificar a la UI de los cambios.

* **`VideoViewIntf` (IVideoView):** Una referencia al control `VideoView` en la UI, pasada desde `App.axaml.cs`.
* **`TopLevel` (TopLevel):** Una referencia al control de nivel superior, usado para mostrar diálogos de archivo.
* **`Position` (string?):** Posición de reproducción actual (ej., "00:01:23").
* **`Duration` (string?):** Duración total del archivo multimedia (ej., "00:05:00").
* **`Filename` (string? o Foundation.NSUrl? para iOS):** El nombre o ruta del archivo cargado actualmente.
* **`VolumeValue` (double?):** Nivel de volumen actual (0-100).
* **`PlayPauseText` (string?):** Texto para el botón Reproducir/Pausar (ej., "PLAY" o "PAUSE").
* **`SpeedText` (string?):** Texto indicando la velocidad de reproducción actual (ej., "SPEED: 1X").
* **`SeekingValue` (double?):** Valor actual del control deslizante de búsqueda.
* **`SeekingMaximum` (double?):** Valor máximo del control deslizante de búsqueda (corresponde a la duración del medio en milisegundos).

```csharp
// Ejemplo de propiedad
private string? _Position = "00:00:00";
public string? Position
{
    get => _Position;
    set => this.RaiseAndSetIfChanged(ref _Position, value);
}

// ... otras propiedades ...
```

### 4.2. Comandos para Interacciones de UI

Se utilizan instancias de `ReactiveCommand` de ReactiveUI para manejar acciones activadas por elementos de UI (ej., clics de botón, cambios de valor de control deslizante).

* **`OpenFileCommand`:** Abre un diálogo de archivo para seleccionar un archivo multimedia.
* **`PlayPauseCommand`:** Reproduce o pausa el medio.
* **`StopCommand`:** Detiene la reproducción.
* **`SpeedCommand`:** Cicla a través de velocidades de reproducción (1x, 2x, 0.5x).
* **`VolumeValueChangedCommand`:** Actualiza el volumen del reproductor cuando cambia el control deslizante de volumen.
* **`SeekingValueChangedCommand`:** Busca una nueva posición cuando cambia el control deslizante de búsqueda.
* **`WindowClosingCommand`:** Maneja la limpieza cuando la ventana de la aplicación se está cerrando.

```csharp
// Constructor - Inicialización de comandos
public MainViewModel()
{
    OpenFileCommand = ReactiveCommand.Create(OpenFileAsync);
    PlayPauseCommand = ReactiveCommand.CreateFromTask(PlayPauseAsync);
    StopCommand = ReactiveCommand.CreateFromTask(StopAsync);
    // ... otras inicializaciones de comandos ...

    // Suscribirse a cambios de propiedad para activar comandos para controles deslizantes
    this.WhenAnyValue(x => x.VolumeValue).Subscribe(_ => VolumeValueChangedCommand.Execute().Subscribe());
    this.WhenAnyValue(x => x.SeekingValue).Subscribe(_ => SeekingValueChangedCommand.Execute().Subscribe());

    _tmPosition = new System.Timers.Timer(1000); // Temporizador para actualizaciones de posición
    _tmPosition.Elapsed += tmPosition_Elapsed;

    VisioForgeX.InitSDK(); // Inicializar SDK de VisioForge
}
```

Nota: `VisioForgeX.InitSDK()` inicializa el SDK de VisioForge. Esto debe llamarse una vez al inicio de la aplicación.

### 4.3. Integración de `MediaPlayerCoreX` de VisioForge

Un campo privado `_player` de tipo `MediaPlayerCoreX` contiene la instancia del motor del reproductor multimedia de VisioForge.

```csharp
private MediaPlayerCoreX _player;
```

### 4.4. Creación del Motor (`CreateEngineAsync`)

Este método asíncrono inicializa o reinicializa la instancia de `MediaPlayerCoreX`.

```csharp
private async Task CreateEngineAsync()
{
    if (_player != null)
    {
        await _player.StopAsync();
        await _player.DisposeAsync();
    }

    _player = new MediaPlayerCoreX(VideoViewIntf); // Pasar el VideoView de Avalonia
    _player.OnError += _player_OnError; // Suscribirse a eventos de error
    _player.Audio_Play = true; // Asegurar que el audio esté habilitado
    
    // Crear configuraciones de fuente desde el nombre de archivo
    var sourceSettings = await UniversalSourceSettings.CreateAsync(Filename);
    await _player.OpenAsync(sourceSettings);
}
```

Pasos clave:

1. Desecha cualquier instancia de reproductor existente.
2. Crea un nuevo `MediaPlayerCoreX`, pasando el `IVideoView` desde la UI.
3. Se suscribe al evento `OnError` para manejo de errores.
4. Establece `Audio_Play = true` para habilitar la reproducción de audio por defecto.
5. Usa `UniversalSourceSettings.CreateAsync(Filename)` para crear configuraciones de fuente apropiadas para el archivo seleccionado.
6. Abre la fuente de medios usando `_player.OpenAsync(sourceSettings)`.

### 4.5. Apertura de Archivo (`OpenFileAsync`)

Este método es responsable de permitir al usuario seleccionar un archivo multimedia.

```csharp
private async Task OpenFileAsync()
{
    await StopAllAsync(); // Detener cualquier reproducción actual
    PlayPauseText = "PLAY";

#if __IOS__ && !__MACCATALYST__
    // Específico de iOS: Usar IDocumentPickerService
    var filePicker = Locator.Current.GetService<IDocumentPickerService>();
    var res = await filePicker.PickVideoAsync();
    if (res != null)
    {
        Filename = (Foundation.NSUrl)res;
        var access = IOSHelper.CheckFileAccess(Filename); // Ayudante para verificar acceso a archivo
        if (!access)
        {
            IOSHelper.ShowToast("Error de acceso a archivo");
            return;
        }
    }
#else
    // Otras plataformas: Usar StorageProvider de Avalonia
    try
    {
        var files = await TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Abrir archivo de video",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            var file = files[0];
            Filename = file.Path.AbsoluteUri;

#if __ANDROID__
            // Específico de Android: Convertir URI de contenido a ruta de archivo si es necesario
            if (!Filename.StartsWith('/'))
            {
                Filename = global::VisioForge.Core.UI.Android.FileDialogHelper.GetFilePathFromUri(AndroidHelper.GetContext(), file.Path);
            }
#endif
        }
    }
    catch (Exception ex)
    {
        // Manejar cancelación o errores
        Debug.WriteLine($"Error al abrir archivo: {ex.Message}");
    }
#endif
}
```

Consideraciones específicas de plataforma:

* **iOS:** Usa un `IDocumentPickerService` (resuelto vía `Locator.Current.GetService`) para presentar el selector de documentos nativo. `IOSHelper.CheckFileAccess` se usa para asegurar que la aplicación tenga permiso para acceder al archivo seleccionado. El nombre de archivo se almacena como un `NSUrl`.
* **Android:** Si la ruta obtenida del selector de archivos es un URI de contenido, se usa `FileDialogHelper.GetFilePathFromUri` (de `VisioForge.Core.UI.Android`) para convertirlo a una ruta de archivo real. Esto requiere un `IAndroidHelper` para obtener el contexto de Android.
* **Escritorio/Otros:** Usa `TopLevel.StorageProvider.OpenFilePickerAsync` para el diálogo de archivo estándar de Avalonia.

### 4.6. Controles de Reproducción

* **`PlayPauseAsync`:**
  * Si el reproductor no está inicializado o detenido (`PlaybackState.Free`), llama a `CreateEngineAsync` y luego a `_player.PlayAsync()`.
  * Si está reproduciendo (`PlaybackState.Play`), llama a `_player.PauseAsync()`.
  * Si está pausado (`PlaybackState.Pause`), llama a `_player.ResumeAsync()`.
  * Actualiza `PlayPauseText` en consecuencia e inicia/detiene el temporizador `_tmPosition`.

    ```csharp
    private async Task PlayPauseAsync()
    {
        // ... (verificación de nombre de archivo nulo/vacío) ...

        if (_player == null || _player.State == PlaybackState.Free)
        {
            await CreateEngineAsync();
            await _player.PlayAsync();
            _tmPosition.Start();
            PlayPauseText = "PAUSE";
        }
        else if (_player.State == PlaybackState.Play)
        {
            await _player.PauseAsync();
            PlayPauseText = "PLAY";
        }
        else if (_player.State == PlaybackState.Pause)
        {
            await _player.ResumeAsync();
            PlayPauseText = "PAUSE";
        }
    }
    ```

* **`StopAsync`:**
  * Llama a `StopAllAsync` para detener el reproductor y restablecer elementos de UI.
  * Restablece `SpeedText` y `PlayPauseText`.

    ```csharp
    private async Task StopAsync()
    {
        await StopAllAsync();
        SpeedText = "SPEED: 1X";
        PlayPauseText = "PLAY";
    }
    ```

* **`StopAllAsync` (Ayudante):**
  * Detiene el temporizador `_tmPosition`.
  * Llama a `_player.StopAsync()`.
  * Restablece `SeekingMaximum` a nulo (para que se recalcule en la próxima reproducción).

    ```csharp
    private async Task StopAllAsync()
    {
        if (_player == null) return;
        _tmPosition.Stop();
        if (_player != null) await _player.StopAsync();
        await Task.Delay(300); // Pequeño retraso para asegurar que la detención se complete
        SeekingMaximum = null;
    }
    ```

### 4.7. Velocidad de Reproducción (`SpeedAsync`)

Cicla a través de tasas de reproducción: 1.0, 2.0 y 0.5.

```csharp
private async Task SpeedAsync()
{
    if (SpeedText == "SPEED: 1X")
    {
        SpeedText = "SPEED: 2X";
        await _player.Rate_SetAsync(2.0);
    }
    else if (SpeedText == "SPEED: 2X")
    {
        SpeedText = "SPEED: 0.5X";
        await _player.Rate_SetAsync(0.5);
    }
    else if (SpeedText == "SPEED: 0.5X") // Asume que este era el estado anterior
    {
        SpeedText = "SPEED: 1X";
        await _player.Rate_SetAsync(1.0);
    }
}
```

Usa `_player.Rate_SetAsync(double rate)` para cambiar la velocidad de reproducción.

### 4.8. Actualizaciones de Posición y Duración (`tmPosition_Elapsed`)

Este método es llamado por el temporizador `_tmPosition` (típicamente cada segundo) para actualizar la UI con la posición y duración de reproducción actuales.

```csharp
private async void tmPosition_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
{
    if (_player == null) return;

    var pos = await _player.Position_GetAsync();
    var progress = (int)pos.TotalMilliseconds;

    try
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (_player == null) return;

            _isTimerUpdate = true; // Bandera para prevenir bucle de búsqueda

            if (SeekingMaximum == null)
            {
                SeekingMaximum = (int)(await _player.DurationAsync()).TotalMilliseconds;
            }

            SeekingValue = Math.Min(progress, (int)(SeekingMaximum ?? progress));
            
            Position = $"{pos.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)}";
            Duration = $"{(await _player.DurationAsync()).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)}";

            _isTimerUpdate = false;
        });
    }
    catch (Exception exception)
    {
        System.Diagnostics.Debug.WriteLine(exception);
    }
}
```

Acciones clave:

1. Recupera la posición actual (`_player.Position_GetAsync()`) y duración (`_player.DurationAsync()`).
2. Actualiza `SeekingMaximum` si aún no se ha establecido (usualmente después de abrir un archivo).
3. Actualiza `SeekingValue` con el progreso actual.
4. Formatea y actualiza las cadenas `Position` y `Duration`.
5. Usa `Dispatcher.UIThread.InvokeAsync` para asegurar que las actualizaciones de UI ocurran en el hilo de UI.
6. Establece `_isTimerUpdate = true` antes de actualizar `SeekingValue` y `false` después, para prevenir que el manejador `OnSeekingValueChanged` vuelva a buscar cuando el temporizador actualiza la posición del control deslizante.

### 4.9. Búsqueda (`OnSeekingValueChanged`)

Llamado cuando la propiedad `SeekingValue` cambia (es decir, el usuario mueve el control deslizante de búsqueda).

```csharp
private async Task OnSeekingValueChanged()
{
    if (!_isTimerUpdate && _player != null && SeekingValue.HasValue)
    {
        await _player.Position_SetAsync(TimeSpan.FromMilliseconds(SeekingValue.Value));
    }
}
```

Si no está siendo actualizado actualmente por el temporizador (`!_isTimerUpdate`), llama a `_player.Position_SetAsync()` para buscar la nueva posición.

### 4.10. Control de Volumen (`OnVolumeValueChanged`)

Llamado cuando la propiedad `VolumeValue` cambia (es decir, el usuario mueve el control deslizante de volumen).

```csharp
private void OnVolumeValueChanged()
{
    if (_player != null && VolumeValue.HasValue)
    {
        // El volumen para MediaPlayerCoreX es 0.0 a 1.0
        _player.Audio_OutputDevice_Volume = VolumeValue.Value / 100.0;
    }
}
```

Establece `_player.Audio_OutputDevice_Volume`. Note que el ViewModel usa una escala de 0-100 para `VolumeValue`, mientras que el reproductor espera 0.0-1.0.

### 4.11. Manejo de Errores (`_player_OnError`)

Un manejador de errores simple que registra errores en la consola de depuración.

```csharp
private void _player_OnError(object sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
{
    Debug.WriteLine(e.Message);
}
```

Aquí se podría implementar un manejo de errores más sofisticado (ej., mostrar un mensaje al usuario).

### 4.12. Limpieza de Recursos (`OnWindowClosing`)

Este método se invoca cuando la ventana principal se está cerrando. Asegura que los recursos del SDK de VisioForge se liberen adecuadamente.

```csharp
private void OnWindowClosing()
{
    if (_player != null)
    {
        _player.OnError -= _player_OnError; // Cancelar suscripción a eventos
        _player.Stop(); // Asegurar que el reproductor esté detenido (versión síncrona aquí para limpieza rápida)
        _player.Dispose();
        _player = null;
    }

    VisioForgeX.DestroySDK(); // Destruir instancia del SDK de VisioForge
}
```

Detiene el reproductor, lo desecha y, lo que es más importante, llama a `VisioForgeX.DestroySDK()` para liberar todos los recursos del SDK. Esto es crucial para prevenir fugas de memoria o problemas cuando la aplicación sale.

Este ViewModel orquesta toda la lógica central del reproductor multimedia, desde cargar archivos hasta controlar la reproducción e interactuar con el SDK de VisioForge.

## 5. Interfaz de Usuario (Vistas)

La interfaz de usuario se define usando Avalonia XAML (archivos `.axaml`).

### 5.1. `MainView.axaml` - La Interfaz del Reproductor

Este `UserControl` define el diseño y los controles para el reproductor multimedia.

**Elementos de UI Clave:**

* **`avalonia:VideoView`:** Este es el control de VisioForge responsable de renderizar video. Se coloca en el área principal de la cuadrícula y se establece para estirarse.

    ```xml
    <avalonia:VideoView x:Name="videoView1" Margin="0,0,0,0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="#0C0C0C"  />
    ```

* **Control Deslizante de Búsqueda (`Slider Name="slSeeking"`):**
  * `Maximum="{Binding SeekingMaximum}"`: Se enlaza a la propiedad `SeekingMaximum` en `MainViewModel`.
  * `Value="{Binding SeekingValue}"`: Se enlaza bidireccionalmente a la propiedad `SeekingValue` en `MainViewModel`. Los cambios a este control deslizante por el usuario actualizarán `SeekingValue`, activando `OnSeekingValueChanged`. Las actualizaciones a `SeekingValue` desde el ViewModel (ej., por el temporizador) actualizarán la posición del control deslizante.
* **Visualización de Tiempo (`TextBlock`s para Posición y Duración):**
  * Enlazado a las propiedades `Position` y `Duration` en `MainViewModel`.
  * `TextBlock Text="{Binding Filename}"` muestra el nombre del archivo actual.
* **Botones de Control de Reproducción (`Button`s):**
  * **Abrir Archivo:** `Command="{Binding OpenFileCommand}"`
  * **Reproducir/Pausar:** `Command="{Binding PlayPauseCommand}"`, `Content="{Binding PlayPauseText}"` (cambia dinámicamente el texto del botón).
  * **Detener:** `Command="{Binding StopCommand}"`
* **Controles de Volumen y Velocidad:**
  * **Control Deslizante de Volumen:** `Value="{Binding VolumeValue}"` (se enlaza a `VolumeValue` para control de volumen).
  * **Botón de Velocidad:** `Command="{Binding SpeedCommand}"`, `Content="{Binding SpeedText}"`.

**Diseño:**
La vista usa un `Grid` para organizar el `VideoView` y un `StackPanel` para los controles en la parte inferior. Los controles mismos se organizan usando `StackPanel`s anidados y `Grid`s para alineación.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="clr-namespace:Simple_Player_MVVM.ViewModels"
             xmlns:avalonia="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"
             x:Class="Simple_Player_MVVM.Views.MainView"
             x:DataType="vm:MainViewModel">
  <Design.DataContext>
    <vm:MainViewModel />
  </Design.DataContext>

  <Grid RowDefinitions="*,Auto" ColumnDefinitions="*">
    <!-- Marcador de posición de Vista de Video -->
    <Border Grid.Row="0" Background="Black" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
      <avalonia:VideoView x:Name="videoView1" Margin="0,0,0,0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="#0C0C0C"  />
    </Border>

    <!-- Controles -->
    <StackPanel Grid.Row="1" Background="#1e1e1e" Orientation="Vertical">
      <!-- Control deslizante para búsqueda -->
      <Slider Name="slSeeking" Margin="16,16,16,0" VerticalAlignment="Center" Maximum="{Binding SeekingMaximum}" Value="{Binding SeekingValue}"/>

      <!-- Visualización de tiempo y nombre de archivo -->
      <Grid Margin="0">
        <Grid.ColumnDefinitions>
          <ColumnDefinition Width="auto"/>
          <ColumnDefinition Width="*"/>
          <ColumnDefinition Width="auto"/>
        </Grid.ColumnDefinitions>
        <TextBlock Grid.Column="0" Text="{Binding Position}" Foreground="White" VerticalAlignment="Center" Margin="5,0,5,0"/>
        <TextBlock Grid.Column="1" Text="{Binding Filename}" Foreground="White" HorizontalAlignment="Center" />
        <TextBlock Grid.Column="2" Text="{Binding Duration}" Foreground="White" VerticalAlignment="Center" Margin="5,0,5,0"/>
      </Grid>

      <!-- Controles de Reproducción -->
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Center" Margin="16,0,5,0">
        <Button Command="{Binding OpenFileCommand}" Content="OPEN FILE" Margin="5" VerticalAlignment="Center"/>
        <Button Name="btPlayPause" Command="{Binding PlayPauseCommand}" Content="{Binding PlayPauseText}" Margin="5"/>
        <Button Name="btStop" Command="{Binding StopCommand}" Content="STOP" Margin="5"/>
      </StackPanel>

      <!-- Controles de Volumen y Velocidad -->
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" Margin="16,0,5,5">
        <TextBlock Text="Volume" Foreground="White" VerticalAlignment="Center"/>
        <Slider Value="{Binding VolumeValue}" Minimum="0" Maximum="100" Width="150" Margin="15,0,5,0" VerticalAlignment="Center"/>
        <Button Command="{Binding SpeedCommand}" Content="{Binding SpeedText}" Margin="5"/>
      </StackPanel>
    </StackPanel>
  </Grid>
</UserControl>
```

La directiva `x:DataType="vm:MainViewModel"` habilita enlaces compilados, proporcionando mejor rendimiento y verificación en tiempo de compilación de rutas de enlace. `Design.DataContext` se usa para proporcionar datos para el previsualizador XAML en IDEs.

### 5.2. `MainView.axaml.cs` - Código Detrás

El código detrás para `MainView` es mínimo. Su propósito principal es proporcionar una forma para que el código de configuración de la aplicación (en `App.axaml.cs`) acceda a la instancia del control `VideoView`.

```csharp
using Avalonia.Controls;
using VisioForge.Core.Types;

namespace Simple_Player_MVVM.Views
{
    public partial class MainView : UserControl
    {
        // Proporciona acceso a la instancia del control VideoView
        public IVideoView GetVideoView()
        {
            return videoView1; // videoView1 es el x:Name del VideoView en XAML
        }

        public MainView()
        {
            InitializeComponent(); // Inicialización estándar de control Avalonia
        }
    }
}
```

Este método `GetVideoView()` se llama durante el inicio de la aplicación para pasar la referencia de `VideoView` al `MainViewModel`.

### 5.3. `MainWindow.axaml` - La Ventana Principal de la Aplicación (Escritorio)

Para plataformas de escritorio, `MainWindow.axaml` sirve como la ventana de nivel superior que aloja el `MainView`.

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:Simple_Player_MVVM.ViewModels"
        xmlns:views="clr-namespace:Simple_Player_MVVM.Views"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="Simple_Player_MVVM.Views.MainWindow"
        Icon="/Assets/avalonia-logo.ico"
        Title="Simple_Player_MVVM">
  <views:MainView />
</Window>
```

Simplemente incrusta el control `MainView`. El `DataContext` (que será una instancia de `MainViewModel`) se establece típicamente en `App.axaml.cs` cuando se crea `MainWindow`.

### 5.4. `MainWindow.axaml.cs` - Código Detrás de Ventana Principal

El código detrás para `MainWindow` maneja principalmente dos cosas:

1. Proporciona una forma de obtener el `VideoView` desde el `MainView` contenido.
2. Se engancha al evento `Closing` de la ventana para activar el `WindowClosingCommand` en el `MainViewModel` para limpieza de recursos.

```csharp
using Avalonia.Controls;
using Simple_Player_MVVM.ViewModels;
using System;
using VisioForge.Core.Types;

namespace Simple_Player_MVVM.Views
{
    public partial class MainWindow : Window
    {
        // Ayudante para obtener VideoView desde el contenido de MainView
        public IVideoView GetVideoView()
        {
            return (Content as MainView).GetVideoView();
        }

        public MainWindow()
        {
            InitializeComponent();

            // Manejar el evento de cierre de ventana para activar limpieza en ViewModel
            Closing += async (sender, e) =>
            {
                if (DataContext is MainViewModel viewModel)
                {
                    // Ejecutar el comando y manejar errores potenciales o finalización
                    viewModel.WindowClosingCommand.Execute()
                        .Subscribe(_ => { /* Opcional: acción al completar */ }, 
                                   ex => Console.WriteLine($"Error durante el cierre: {ex.Message}"));
                }
            };
        }
    }
}
```

Cuando la ventana se cierra, verifica si el `DataContext` es un `MainViewModel` y luego ejecuta su `WindowClosingCommand`. Esto asegura que el `MainViewModel` pueda realizar la limpieza necesaria, como desechar la instancia de `MediaPlayerCoreX` y llamar a `VisioForgeX.DestroySDK()`.

## 6. Detalles de Implementación Específicos de Plataforma

Aunque Avalonia y .NET proporcionan un alto grado de compatibilidad multiplataforma, ciertos aspectos como el acceso al sistema de archivos y permisos requieren manejo específico de plataforma.

### 6.1. Interfaces para Servicios de Plataforma

Para abstraer la funcionalidad específica de plataforma, se definen interfaces en el proyecto central `SimplePlayerMVVM`:

* **`IAndroidHelper.cs`:**

    ```csharp
    namespace SimplePlayerMVVM
    {
        public interface IAndroidHelper
        {
    #if __ANDROID__
            global::Android.Content.Context GetContext();
    #endif
        }
    }
    ```

    Esta interfaz se usa para obtener el `Context` de Android, que se necesita para operaciones como convertir URIs de contenido a rutas de archivo.

* **`IDocumentPickerService.cs`:**

    ```csharp
    using System.Threading.Tasks;

    namespace SimplePlayerMVVM;

    public interface IDocumentPickerService
    {
        Task<object?> PickVideoAsync();
    }
    ```

    Esta interfaz abstrae el mecanismo de selección de archivos, específicamente para iOS donde se prefiere un selector de documentos nativo.

### 6.2. Implementación Android (Proyecto `SimplePlayerMVVM.Android`)

* **`MainActivity.cs`:**
  * Hereda de `AvaloniaMainActivity<App>` e implementa `IAndroidHelper`.
  * **`CustomizeAppBuilder`:** Configuración estándar de Avalonia Android.
  * **Permisos:** En `OnCreate`, llama a `RequestPermissionsAsync` para solicitar permisos necesarios como `Manifest.Permission.Internet`, `Manifest.Permission.ReadExternalStorage` y `Manifest.Permission.ReadMediaVideo` (para versiones más nuevas de Android).

        ```csharp
        // En MainActivity.cs
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainViewModel.AndroidHelper = this; // Proporcionar implementación IAndroidHelper a ViewModel
            RequestPermissionsAsync();
        }

        private async void RequestPermissionsAsync()
        {
            RequestPermissions(
                new String[]{
                        Manifest.Permission.Internet,
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.ReadMediaVideo}, 1004);
        }

        public Context GetContext() // Implementación IAndroidHelper
        {
            return this;
        }
        ```

  * La línea `MainViewModel.AndroidHelper = this;` hace que la instancia de `MainActivity` (que implementa `IAndroidHelper`) esté disponible para el `MainViewModel` para obtener el contexto de Android.
  * El Manifiesto de Android (`AndroidManifest.xml`, no mostrado explícitamente en los archivos proporcionados pero esencial) también debe declarar estos permisos.

* **Manejo de Ruta de Archivo:** Como se ve en `MainViewModel.OpenFileAsync`, el método `FileDialogHelper.GetFilePathFromUri` usa el contexto obtenido vía `IAndroidHelper` para resolver rutas de archivo desde URIs de contenido, lo cual es común al usar el selector de archivos de Android.

* **Archivo de Proyecto (`SimplePlayerMVVM.Android.csproj`):** Configura la compilación específica de Android, versiones de SDK de destino e incluye bibliotecas de VisioForge Android necesarias.

### 6.3. Implementación iOS (Proyecto `SimplePlayerMVVM.iOS`)

* **`AppDelegate.cs`:**
  * Hereda de `AvaloniaAppDelegate<App>`.
  * **`CustomizeAppBuilder`:** Registra el `IOSDocumentPickerService` con el resolutor de dependencias Splat (`Locator.CurrentMutable.RegisterConstant`). Esto hace que el `IDocumentPickerService` esté disponible para inyección o ubicación de servicio en el `MainViewModel`.

        ```csharp
        // En AppDelegate.cs
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            Locator.CurrentMutable.RegisterConstant(new IOSDocumentPickerService(), typeof(IDocumentPickerService));
            
            return base.CustomizeAppBuilder(builder)
                .WithInterFont()
                .UseReactiveUI();
        }
        ```

* **`IOSDocumentPickerService.cs`:**
  * Implementa `IDocumentPickerService`.
  * Usa `UIDocumentPickerViewController` para presentar el selector de archivos nativo de iOS para archivos de video (`UTType.Video`, `UTType.Movie`).
  * Maneja los eventos `DidPickDocumentAtUrls` y `WasCancelled` del selector.
  * Retorna la URL del archivo seleccionado (`NSUrl`) vía un `TaskCompletionSource`.
  * Incluye código de utilidad (`GetTopViewController`) para encontrar el controlador de vista superior desde donde presentar el selector.

    ```csharp
    // Fragmento de IOSDocumentPickerService.cs
    public Task<object?> PickVideoAsync()
    {
        _tcs = new TaskCompletionSource<object?>();
        string[] allowedUTIs = { UTType.Video, UTType.Movie };
        var picker = new UIDocumentPickerViewController(allowedUTIs, UIDocumentPickerMode.Import);
        // ... suscripciones a eventos y presentación ...
        return _tcs.Task;
    }

    private void OnDocumentPicked(object sender, UIDocumentPickedAtUrlsEventArgs e)
    {
        // ... maneja URL seleccionada, resuelve _tcs ...
        NSUrl fileUrl = e.Urls[0];
        _tcs?.TrySetResult(fileUrl);
    }
    ```

* **`Info.plist`:**
  * Este archivo es crucial para aplicaciones iOS. Debe incluir claves como `NSPhotoLibraryUsageDescription` si se accede a la biblioteca de fotos, u otros permisos relevantes dependiendo de dónde se almacenen/accedan los archivos. El `Info.plist` proporcionado incluye:

        ```xml
        <key>NSPhotoLibraryUsageDescription</key>
        <string>Biblioteca de fotos usada para reproducir archivos</string>
        ```

  * También define identificadores de paquete, números de versión, orientaciones soportadas, etc.

* **Archivo de Proyecto (`SimplePlayerMVVM.iOS.csproj`):** Configura la compilación específica de iOS, versión de SO de destino e incluye bibliotecas de VisioForge iOS.

### 6.4. Implementación de Escritorio (Proyecto `SimplePlayerMVVM.Desktop`)

* **`Program.cs`:**
  * Contiene el punto de entrada `Main` para aplicaciones de escritorio.
  * Usa `BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);` para inicializar y ejecutar la aplicación Avalonia para escritorio.
  * `BuildAvaloniaApp()` configura Avalonia con detección de plataforma, fuentes, ReactiveUI y registro.

    ```csharp
    internal sealed class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .UseReactiveUI()
                .LogToTrace();
    }
    ```

* **Acceso a Archivos:** En escritorio, el acceso a archivos es generalmente más directo. El `MainViewModel` usa `TopLevel.StorageProvider.OpenFilePickerAsync` que funciona a través de Windows, macOS y Linux sin complejidades específicas de servicios de ayuda como las de URI/permisos de Android o iOS.

* **Archivo de Proyecto (`SimplePlayerMVVM.Desktop.csproj`):**
  * Apunta a marcos de trabajo de escritorio específicos (ej., `net8.0-windows`, `net8.0-macos14.0`, `net8.0` para Linux).
  * Incluye `Avalonia.Desktop`.
  * Incluye bibliotecas nativas de VisioForge específicas de plataforma para Windows (x64) y macOS a través de condiciones `PackageReference`.

        ```xml
        <ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
          <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="..." />
          <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="..." />
        </ItemGroup>
        <ItemGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
          <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="..." />
        </ItemGroup>
        ```

* **Manifiesto de Windows (`app.manifest`):** Usado en Windows para configuraciones de aplicación, como compatibilidad con versiones específicas de Windows.
* **Info de macOS (`Info.plist` en proyecto Desktop):** Proporciona información de paquete para aplicaciones macOS.

Estos proyectos y configuraciones específicos de plataforma aseguran que la lógica central compartida en `SimplePlayerMVVM` pueda interactuar correctamente con las características nativas y requisitos de cada sistema operativo.

## 7. Componentes Clave del SDK de VisioForge Usados

Esta aplicación aprovecha varios componentes clave del SDK de Reproductor Multimedia X de VisioForge:

* **`VisioForge.Core.MediaPlayerX.MediaPlayerCoreX`:**
  * El motor central para reproducción de medios. Maneja la apertura de fuentes de medios, control de reproducción (reproducir, pausar, detener, buscar, tasa), gestión de audio y video, y proporciona información de estado (posición, duración).
  * Inicializado en `MainViewModel` y vinculado al `VideoViewIntf`.
* **`VisioForge.Core.UI.Avalonia.VideoView`:**
  * Un control de Avalonia que sirve como la superficie de renderizado para video. Se declara en `MainView.axaml` y su referencia (`IVideoView`) se pasa a `MediaPlayerCoreX`.
* **`VisioForge.Core.Types.X.Sources.UniversalSourceSettings`:**
  * Usado para configurar la fuente de medios. `UniversalSourceSettings.CreateAsync(filename)` determina automáticamente la mejor manera de abrir un archivo o URL dado.
* **Clase estática `VisioForge.Core.VisioForgeX`:**
  * **`InitSDK()`:** Inicializa el SDK de VisioForge. Esto debe llamarse una vez al inicio de la aplicación (hecho en el constructor de `MainViewModel` en este ejemplo, pero también puede hacerse en `App.axaml.cs`).
  * **`DestroySDK()`:** Libera todos los recursos del SDK. Esto debe llamarse cuando la aplicación se está cerrando para prevenir fugas de recursos (hecho en `MainViewModel.OnWindowClosing`).
* **Bibliotecas Específicas de Plataforma:**
  * Como se detalla en la configuración del proyecto y secciones específicas de plataforma, varios paquetes NuGet como `VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Core.macOS`, `VisioForge.CrossPlatform.Core.Android` y `VisioForge.CrossPlatform.Core.iOS` proporcionan los binarios nativos y enlaces necesarios para cada plataforma.
* **`VisioForge.Core.UI.Android.FileDialogHelper` (para Android):**
  * Contiene métodos de ayuda como `GetFilePathFromUri` para trabajar con el sistema de archivos de Android y URIs de contenido.

Entender estos componentes es crucial para trabajar con el SDK de VisioForge y extender la funcionalidad del reproductor.

## 8. Construyendo y Ejecutando la Aplicación

1. **Clonar/Descargar el Código Fuente:** Obtenga el proyecto de ejemplo `SimplePlayerMVVM`.
2. **Restaurar Paquetes NuGet:** Abra la solución en su IDE y asegúrese de que todos los paquetes NuGet estén restaurados para todos los proyectos.
3. **Seleccionar Proyecto de Inicio y Destino:**
    * **Escritorio:** Establezca `SimplePlayerMVVM.Desktop` como el proyecto de inicio. Luego puede ejecutarlo directamente en Windows, macOS o Linux (asegúrese de tener el tiempo de ejecución .NET para su SO).
    * **Android:** Establezca `SimplePlayerMVVM.Android` como el proyecto de inicio. Seleccione un emulador de Android o dispositivo conectado. Compile e implemente.
        * Asegúrese de que el SDK de Android y los emuladores/dispositivos estén configurados correctamente en su IDE.
        * Es posible que necesite aceptar solicitudes de permiso en el dispositivo/emulador al primer lanzamiento.
    * **iOS:** Establezca `SimplePlayerMVVM.iOS` como el proyecto de inicio. Seleccione un simulador de iOS o dispositivo conectado (requiere una máquina de compilación macOS y aprovisionamiento de Desarrollador de Apple apropiado).
        * Asegúrese de que Xcode y las herramientas de desarrollador estén configurados correctamente.
        * Es posible que necesite confiar en el certificado de desarrollador en el dispositivo.
4. **Compilar y Ejecutar:** Compile el proyecto de inicio seleccionado y ejecútelo.

## 9. Conclusión

Esta guía ha demostrado cómo construir un reproductor multimedia multiplataforma usando Avalonia UI con el patrón MVVM y el SDK de Reproductor Multimedia X de VisioForge. Al aprovechar un proyecto central compartido para ViewModels y Vistas, y manejar los aspectos específicos de plataforma en proyectos principales dedicados, podemos crear una aplicación mantenible que se ejecuta en una amplia gama de dispositivos.

Puntos clave:

* El patrón MVVM ayuda a separar preocupaciones y mejora la capacidad de prueba.
* ReactiveUI simplifica la implementación de MVVM con Avalonia.
* El SDK de Reproductor Multimedia X de VisioForge proporciona potentes capacidades de reproducción de medios, con `MediaPlayerCoreX` como el motor central y `VideoView` para integración con Avalonia UI.
* Las consideraciones específicas de plataforma, especialmente para acceso a archivos y permisos, se manejan a través de interfaces e implementaciones específicas de plataforma.
* La inicialización adecuada (`VisioForgeX.InitSDK()`) y limpieza (`VisioForgeX.DestroySDK()`) del SDK de VisioForge son esenciales.

Puede extender este ejemplo agregando más características como soporte de listas de reproducción, transmisión en red, efectos de video o controles de UI más avanzados.
