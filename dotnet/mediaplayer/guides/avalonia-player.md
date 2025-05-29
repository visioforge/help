# How to Create a Cross-Platform Media Player using Avalonia MVVM and VisioForge SDK

This guide will walk you through the process of building a cross-platform media player application using Avalonia UI with the Model-View-ViewModel (MVVM) pattern and the VisioForge Media Player SDK. The application will be capable of playing video files on Windows, macOS, Linux, Android, and iOS.

We will be referencing the `SimplePlayerMVVM` example project, which demonstrates the core concepts and implementation details.

`[SCREENSHOT: Final application running on multiple platforms]`

## 1. Prerequisites

Before you begin, ensure you have the following installed:

* .NET SDK (latest version, e.g., .NET 8 or newer)
* An IDE such as Visual Studio, JetBrains Rider, or VS Code with C# and Avalonia extensions.
* For Android development:
  * Android SDK
  * Java Development Kit (JDK)
* For iOS development (requires a macOS machine):
  * Xcode
  * Necessary provisioning profiles and certificates.
* VisioForge .NET SDK (MediaPlayer SDK X). You can obtain this from the VisioForge website. The necessary packages will be added via NuGet.

## 2. Project Setup

This section outlines how to set up the solution structure and include the necessary VisioForge SDK packages.

### 2.1. Solution Structure

The `SimplePlayerMVVM` solution consists of several projects:

* **SimplePlayerMVVM**: A .NET Standard library containing the core application logic, including ViewModels, Views (AXAML), and shared interfaces. This is the main project where most of our application logic resides.
* **SimplePlayerMVVM.Android**: The Android-specific head project.
* **SimplePlayerMVVM.Desktop**: The desktop-specific head project (Windows, macOS, Linux).
* **SimplePlayerMVVM.iOS**: The iOS-specific head project.

`[SCREENSHOT: Solution structure in the IDE]`

### 2.2. Core Project (`SimplePlayerMVVM.csproj`)

The main project, `SimplePlayerMVVM.csproj`, targets multiple platforms. Key package references include:

* `Avalonia`: The core Avalonia UI framework.
* `Avalonia.Themes.Fluent`: Provides a Fluent Design theme.
* `Avalonia.ReactiveUI`: For MVVM support using ReactiveUI.
* `VisioForge.DotNet.MediaBlocks`: Core VisioForge media processing components.
* `VisioForge.DotNet.Core.UI.Avalonia`: VisioForge UI components for Avalonia, including the `VideoView`.

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
  <!--Condition below is needed to remove Avalonia.Diagnostics package from build output in Release configuration.-->
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

This setup allows the core logic to be shared across all target platforms.

### 2.3. Platform-Specific Projects

Each platform head project (`SimplePlayerMVVM.Android.csproj`, `SimplePlayerMVVM.Desktop.csproj`, `SimplePlayerMVVM.iOS.csproj`) includes platform-specific dependencies and configurations.

**Desktop (`SimplePlayerMVVM.Desktop.csproj`):**

* References `Avalonia.Desktop`.
* Includes platform-specific VisioForge native libraries (e.g., `VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Core.macOS`).

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

* References `Avalonia.Android`.
* Includes Android-specific VisioForge libraries and dependencies like `VisioForge.CrossPlatform.Core.Android`.

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
  <!-- ... other items ... -->
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

* References `Avalonia.iOS`.
* Includes iOS-specific VisioForge libraries like `VisioForge.CrossPlatform.Core.iOS`.

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
  <!-- ... other items ... -->
  <ItemGroup>
    <PackageReference Include="Avalonia.iOS" Version="$(AvaloniaVersion)" />
    <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\SimplePlayerMVVM\SimplePlayerMVVM.csproj" />
  </ItemGroup>
</Project>
```

These project files are crucial for managing dependencies and build configurations for each platform.

## 3. Core MVVM Structure

The application follows the MVVM pattern, separating UI (Views) from logic (ViewModels) and data (Models). ReactiveUI is used to facilitate this pattern.

### 3.1. `ViewModelBase.cs`

This abstract class serves as the base for all ViewModels in the application. It inherits from `ReactiveObject`, which is part of ReactiveUI and provides the necessary infrastructure for property change notifications.

```csharp
using ReactiveUI;

namespace Simple_Player_MVVM.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
    }
}
```

Any ViewModel that needs to notify the UI of property changes should inherit from `ViewModelBase`.

`[SCREENSHOT: ViewModelBase.cs code]`

### 3.2. `ViewLocator.cs`

The `ViewLocator` class is responsible for locating and instantiating Views based on the type of their corresponding ViewModel. It implements Avalonia's `IDataTemplate` interface.

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

When Avalonia needs to display a ViewModel, the `ViewLocator`'s `Match` method checks if the data object is a `ViewModelBase`. If it is, the `Build` method attempts to find a corresponding View by replacing "ViewModel" with "View" in the ViewModel's class name and instantiates it.

This convention-based approach simplifies the association between Views and ViewModels.

`[SCREENSHOT: ViewLocator.cs code]`

### 3.3. Application Initialization (`App.axaml` and `App.axaml.cs`)

The `App.axaml` file defines the application-level resources, including the `ViewLocator` as a data template and the theme (FluentTheme).

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
The `App.axaml.cs` file handles the application's initialization and lifecycle.

Key responsibilities in `OnFrameworkInitializationCompleted`:

1. Creates an instance of `MainViewModel`.
2. Sets up the main window or view based on the application lifetime (`IClassicDesktopStyleApplicationLifetime` for desktop, `ISingleViewApplicationLifetime` for mobile/web-like views).
3. Assigns the `MainViewModel` instance as the `DataContext` for the main window/view.
4. Retrieves the `IVideoView` instance from the `MainView` (hosted within `MainWindow` or directly as `MainView`).
5. Passes the `IVideoView` and the `TopLevel` control (necessary for file dialogs and other top-level interactions) to the `MainViewModel`.

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

This setup ensures that the `MainViewModel` has access to the necessary UI components for video playback and interaction, regardless of the platform.

`[SCREENSHOT: App.axaml.cs code focusing on OnFrameworkInitializationCompleted]`

## 4. MainViewModel Implementation (`MainViewModel.cs`)

The `MainViewModel` is central to the media player's functionality. It manages the player's state, handles user interactions, and communicates with the VisioForge `MediaPlayerCoreX` engine.

`[SCREENSHOT: MainViewModel.cs overall structure or class definition]`

Key components of `MainViewModel`:

### 4.1. Properties for UI Binding

The ViewModel exposes several properties that are bound to UI elements in `MainView.axaml`. These properties use `ReactiveUI`'s `RaiseAndSetIfChanged` to notify the UI of changes.

* **`VideoViewIntf` (IVideoView):** A reference to the `VideoView` control in the UI, passed from `App.axaml.cs`.
* **`TopLevel` (TopLevel):** A reference to the top-level control, used for displaying file dialogs.
* **`Position` (string?):** Current playback position (e.g., "00:01:23").
* **`Duration` (string?):** Total duration of the media file (e.g., "00:05:00").
* **`Filename` (string? or Foundation.NSUrl? for iOS):** The name or path of the currently loaded file.
* **`VolumeValue` (double?):** Current volume level (0-100).
* **`PlayPauseText` (string?):** Text for the Play/Pause button (e.g., "PLAY" or "PAUSE").
* **`SpeedText` (string?):** Text indicating the current playback speed (e.g., "SPEED: 1X").
* **`SeekingValue` (double?):** Current value of the seeking slider.
* **`SeekingMaximum` (double?):** Maximum value of the seeking slider (corresponds to media duration in milliseconds).

```csharp
// Example property
private string? _Position = "00:00:00";
public string? Position
{
    get => _Position;
    set => this.RaiseAndSetIfChanged(ref _Position, value);
}

// ... other properties ...
```

### 4.2. Commands for UI Interactions

ReactiveUI `ReactiveCommand` instances are used to handle actions triggered by UI elements (e.g., button clicks, slider value changes).

* **`OpenFileCommand`:** Opens a file dialog to select a media file.
* **`PlayPauseCommand`:** Plays or pauses the media.
* **`StopCommand`:** Stops playback.
* **`SpeedCommand`:** Cycles through playback speeds (1x, 2x, 0.5x).
* **`VolumeValueChangedCommand`:** Updates the player volume when the volume slider changes.
* **`SeekingValueChangedCommand`:** Seeks to a new position when the seeking slider changes.
* **`WindowClosingCommand`:** Handles cleanup when the application window is closing.

```csharp
// Constructor - Command initialization
public MainViewModel()
{
    OpenFileCommand = ReactiveCommand.Create(OpenFileAsync);
    PlayPauseCommand = ReactiveCommand.CreateFromTask(PlayPauseAsync);
    StopCommand = ReactiveCommand.CreateFromTask(StopAsync);
    // ... other command initializations ...

    // Subscribe to property changes to trigger commands for sliders
    this.WhenAnyValue(x => x.VolumeValue).Subscribe(_ => VolumeValueChangedCommand.Execute().Subscribe());
    this.WhenAnyValue(x => x.SeekingValue).Subscribe(_ => SeekingValueChangedCommand.Execute().Subscribe());

    _tmPosition = new System.Timers.Timer(1000); // Timer for position updates
    _tmPosition.Elapsed += tmPosition_Elapsed;

    VisioForgeX.InitSDK(); // Initialize VisioForge SDK
}
```

Note: `VisioForgeX.InitSDK()` initializes the VisioForge SDK. This should be called once at application startup.

### 4.3. VisioForge `MediaPlayerCoreX` Integration

A private field `_player` of type `MediaPlayerCoreX` holds the instance of the VisioForge media player engine.

```csharp
private MediaPlayerCoreX _player;
```

### 4.4. Engine Creation (`CreateEngineAsync`)

This asynchronous method initializes or re-initializes the `MediaPlayerCoreX` instance.

```csharp
private async Task CreateEngineAsync()
{
    if (_player != null)
    {
        await _player.StopAsync();
        await _player.DisposeAsync();
    }

    _player = new MediaPlayerCoreX(VideoViewIntf); // Pass the Avalonia VideoView
    _player.OnError += _player_OnError; // Subscribe to error events
    _player.Audio_Play = true; // Ensure audio is enabled
    
    // Create source settings from the filename
    var sourceSettings = await UniversalSourceSettings.CreateAsync(Filename);
    await _player.OpenAsync(sourceSettings);
}
```

Key steps:

1. Disposes of any existing player instance.
2. Creates a new `MediaPlayerCoreX`, passing the `IVideoView` from the UI.
3. Subscribes to the `OnError` event for error handling.
4. Sets `Audio_Play = true` to enable audio playback by default.
5. Uses `UniversalSourceSettings.CreateAsync(Filename)` to create source settings appropriate for the selected file.
6. Opens the media source using `_player.OpenAsync(sourceSettings)`.

`[SCREENSHOT: CreateEngineAsync method code]`

### 4.5. File Opening (`OpenFileAsync`)

This method is responsible for allowing the user to select a media file.

```csharp
private async Task OpenFileAsync()
{
    await StopAllAsync(); // Stop any current playback
    PlayPauseText = "PLAY";

#if __IOS__ && !__MACCATALYST__
    // iOS specific: Use IDocumentPickerService
    var filePicker = Locator.Current.GetService<IDocumentPickerService>();
    var res = await filePicker.PickVideoAsync();
    if (res != null)
    {
        Filename = (Foundation.NSUrl)res;
        var access = IOSHelper.CheckFileAccess(Filename); // Helper to check file access
        if (!access)
        {
            IOSHelper.ShowToast("File access error");
            return;
        }
    }
#else
    // Other platforms: Use Avalonia's StorageProvider
    try
    {
        var files = await TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open video file",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            var file = files[0];
            Filename = file.Path.AbsoluteUri;

#if __ANDROID__
            // Android specific: Convert content URI to file path if necessary
            if (!Filename.StartsWith('/'))
            {
                Filename = global::VisioForge.Core.UI.Android.FileDialogHelper.GetFilePathFromUri(AndroidHelper.GetContext(), file.Path);
            }
#endif
        }
    }
    catch (Exception ex)
    {
        // Handle cancellation or errors
        Debug.WriteLine($"File open error: {ex.Message}");
    }
#endif
}
```

Platform-specific considerations:

* **iOS:** Uses an `IDocumentPickerService` (resolved via `Locator.Current.GetService`) to present the native document picker. `IOSHelper.CheckFileAccess` is used to ensure the app has permission to access the selected file. The filename is stored as an `NSUrl`.
* **Android:** If the path obtained from the file picker is a content URI, `FileDialogHelper.GetFilePathFromUri` (from `VisioForge.Core.UI.Android`) is used to convert it to an actual file path. This requires an `IAndroidHelper` to get the Android context.
* **Desktop/Other:** Uses `TopLevel.StorageProvider.OpenFilePickerAsync` for the standard Avalonia file dialog.

`[SCREENSHOT: OpenFileAsync method with platform-specific blocks highlighted]`

### 4.6. Playback Controls

* **`PlayPauseAsync`:**
  * If the player is not initialized or stopped (`PlaybackState.Free`), it calls `CreateEngineAsync` and then `_player.PlayAsync()`.
  * If playing (`PlaybackState.Play`), it calls `_player.PauseAsync()`.
  * If paused (`PlaybackState.Pause`), it calls `_player.ResumeAsync()`.
  * Updates `PlayPauseText` accordingly and starts/stops the `_tmPosition` timer.

    ```csharp
    private async Task PlayPauseAsync()
    {
        // ... (null/empty filename check) ...

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
  * Calls `StopAllAsync` to stop the player and reset UI elements.
  * Resets `SpeedText` and `PlayPauseText`.

    ```csharp
    private async Task StopAsync()
    {
        await StopAllAsync();
        SpeedText = "SPEED: 1X";
        PlayPauseText = "PLAY";
    }
    ```

* **`StopAllAsync` (Helper):**
  * Stops the `_tmPosition` timer.
  * Calls `_player.StopAsync()`.
  * Resets `SeekingMaximum` to null (so it gets re-calculated on next play).

    ```csharp
    private async Task StopAllAsync()
    {
        if (_player == null) return;
        _tmPosition.Stop();
        if (_player != null) await _player.StopAsync();
        await Task.Delay(300); // Small delay to ensure stop completes
        SeekingMaximum = null;
    }
    ```

### 4.7. Playback Speed (`SpeedAsync`)

Cycles through playback rates: 1.0, 2.0, and 0.5.

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
    else if (SpeedText == "SPEED: 0.5X") // Assumes this was the previous state
    {
        SpeedText = "SPEED: 1X";
        await _player.Rate_SetAsync(1.0);
    }
}
```

Uses `_player.Rate_SetAsync(double rate)` to change the playback speed.

### 4.8. Position and Duration Updates (`tmPosition_Elapsed`)

This method is called by the `_tmPosition` timer (typically every second) to update the UI with the current playback position and duration.

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

            _isTimerUpdate = true; // Flag to prevent seeking loop

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

Key actions:

1. Retrieves current position (`_player.Position_GetAsync()`) and duration (`_player.DurationAsync()`).
2. Updates `SeekingMaximum` if it hasn't been set yet (usually after a file is opened).
3. Updates `SeekingValue` with the current progress.
4. Formats and updates `Position` and `Duration` strings.
5. Uses `Dispatcher.UIThread.InvokeAsync` to ensure UI updates happen on the UI thread.
6. Sets `_isTimerUpdate = true` before updating `SeekingValue` and `false` after, to prevent the `OnSeekingValueChanged` handler from re-seeking when the timer updates the slider position.

`[SCREENSHOT: tmPosition_Elapsed method]`

### 4.9. Seeking (`OnSeekingValueChanged`)

Called when the `SeekingValue` property changes (i.e., the user moves the seeking slider).

```csharp
private async Task OnSeekingValueChanged()
{
    if (!_isTimerUpdate && _player != null && SeekingValue.HasValue)
    {
        await _player.Position_SetAsync(TimeSpan.FromMilliseconds(SeekingValue.Value));
    }
}
```

If not currently being updated by the timer (`!_isTimerUpdate`), it calls `_player.Position_SetAsync()` to seek to the new position.

### 4.10. Volume Control (`OnVolumeValueChanged`)

Called when the `VolumeValue` property changes (i.e., the user moves the volume slider).

```csharp
private void OnVolumeValueChanged()
{
    if (_player != null && VolumeValue.HasValue)
    {
        // Volume for MediaPlayerCoreX is 0.0 to 1.0
        _player.Audio_OutputDevice_Volume = VolumeValue.Value / 100.0;
    }
}
```

Sets `_player.Audio_OutputDevice_Volume`. Note that the ViewModel uses a 0-100 scale for `VolumeValue`, while the player expects 0.0-1.0.

### 4.11. Error Handling (`_player_OnError`)

A simple error handler that logs errors to the debug console.

```csharp
private void _player_OnError(object sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
{
    Debug.WriteLine(e.Message);
}
```

More sophisticated error handling (e.g., showing a message to the user) could be implemented here.

### 4.12. Resource Cleanup (`OnWindowClosing`)

This method is invoked when the main window is closing. It ensures that VisioForge SDK resources are properly released.

```csharp
private void OnWindowClosing()
{
    if (_player != null)
    {
        _player.OnError -= _player_OnError; // Unsubscribe from events
        _player.Stop(); // Ensure player is stopped (sync version here for quick cleanup)
        _player.Dispose();
        _player = null;
    }

    VisioForgeX.DestroySDK(); // Destroy VisioForge SDK instance
}
```

It stops the player, disposes of it, and importantly, calls `VisioForgeX.DestroySDK()` to release all SDK resources. This is crucial to prevent memory leaks or issues when the application exits.

This ViewModel orchestrates all the core logic of the media player, from loading files to controlling playback and interacting with the VisioForge SDK.

## 5. User Interface (Views)

The user interface is defined using Avalonia XAML (`.axaml` files).

### 5.1. `MainView.axaml` - The Player Interface

This `UserControl` defines the layout and controls for the media player.

`[SCREENSHOT: MainView.axaml rendered UI design]`

**Key UI Elements:**

* **`avalonia:VideoView`:** This is the VisioForge control responsible for rendering video. It's placed in the main area of the grid and set to stretch.

    ```xml
    <avalonia:VideoView x:Name="videoView1" Margin="0,0,0,0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="#0C0C0C"  />
    ```

* **Seeking Slider (`Slider Name="slSeeking"`):**
  * `Maximum="{Binding SeekingMaximum}"`: Binds to the `SeekingMaximum` property in `MainViewModel`.
  * `Value="{Binding SeekingValue}"`: Binds two-way to the `SeekingValue` property in `MainViewModel`. Changes to this slider by the user will update `SeekingValue`, triggering `OnSeekingValueChanged`. Updates to `SeekingValue` from the ViewModel (e.g., by the timer) will update the slider's position.
* **Time Display (`TextBlock`s for Position and Duration):**
  * Bound to `Position` and `Duration` properties in `MainViewModel`.
  * `TextBlock Text="{Binding Filename}"` displays the current file name.
* **Playback Control Buttons (`Button`s):**
  * **Open File:** `Command="{Binding OpenFileCommand}"`
  * **Play/Pause:** `Command="{Binding PlayPauseCommand}"`, `Content="{Binding PlayPauseText}"` (dynamically changes button text).
  * **Stop:** `Command="{Binding StopCommand}"`
* **Volume and Speed Controls:**
  * **Volume Slider:** `Value="{Binding VolumeValue}"` (binds to `VolumeValue` for volume control).
  * **Speed Button:** `Command="{Binding SpeedCommand}"`, `Content="{Binding SpeedText}"`.

**Layout:**
The view uses a `Grid` to arrange the `VideoView` and a `StackPanel` for the controls at the bottom. The controls themselves are organized using nested `StackPanel`s and `Grid`s for alignment.

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
    <!-- Video View Placeholder -->
    <Border Grid.Row="0" Background="Black" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
      <avalonia:VideoView x:Name="videoView1" Margin="0,0,0,0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="#0C0C0C"  />
    </Border>

    <!-- Controls -->
    <StackPanel Grid.Row="1" Background="#1e1e1e" Orientation="Vertical">
      <!-- Slider for seeking -->
      <Slider Name="slSeeking" Margin="16,16,16,0" VerticalAlignment="Center" Maximum="{Binding SeekingMaximum}" Value="{Binding SeekingValue}"/>

      <!-- Time and filename display -->
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

      <!-- Playback Controls -->
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Center" Margin="16,0,5,0">
        <Button Command="{Binding OpenFileCommand}" Content="OPEN FILE" Margin="5" VerticalAlignment="Center"/>
        <Button Name="btPlayPause" Command="{Binding PlayPauseCommand}" Content="{Binding PlayPauseText}" Margin="5"/>
        <Button Name="btStop" Command="{Binding StopCommand}" Content="STOP" Margin="5"/>
      </StackPanel>

      <!-- Volume and Speed Controls -->
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" Margin="16,0,5,5">
        <TextBlock Text="Volume" Foreground="White" VerticalAlignment="Center"/>
        <Slider Value="{Binding VolumeValue}" Minimum="0" Maximum="100" Width="150" Margin="15,0,5,0" VerticalAlignment="Center"/>
        <Button Command="{Binding SpeedCommand}" Content="{Binding SpeedText}" Margin="5"/>
      </StackPanel>
    </StackPanel>
  </Grid>
</UserControl>
```

The `x:DataType="vm:MainViewModel"` directive enables compiled bindings, providing better performance and compile-time checking of binding paths. The `Design.DataContext` is used to provide data for the XAML previewer in IDEs.

`[SCREENSHOT: MainView.axaml XAML code, perhaps highlighting binding expressions]`

### 5.2. `MainView.axaml.cs` - Code-Behind

The code-behind for `MainView` is minimal. Its primary purpose is to provide a way for the application setup code (in `App.axaml.cs`) to access the `VideoView` control instance.

```csharp
using Avalonia.Controls;
using VisioForge.Core.Types;

namespace Simple_Player_MVVM.Views
{
    public partial class MainView : UserControl
    {
        // Provides access to the VideoView control instance
        public IVideoView GetVideoView()
        {
            return videoView1; // videoView1 is the x:Name of the VideoView in XAML
        }

        public MainView()
        {
            InitializeComponent(); // Standard Avalonia control initialization
        }
    }
}
```

This `GetVideoView()` method is called during application startup to pass the `VideoView` reference to the `MainViewModel`.

### 5.3. `MainWindow.axaml` - The Main Application Window (Desktop)

For desktop platforms, `MainWindow.axaml` serves as the top-level window that hosts the `MainView`.

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

It simply embeds the `MainView` control. The `DataContext` (which will be an instance of `MainViewModel`) is typically set in `App.axaml.cs` when the `MainWindow` is created.

### 5.4. `MainWindow.axaml.cs` - Main Window Code-Behind

The code-behind for `MainWindow` primarily handles two things:

1. Provides a way to get the `VideoView` from the contained `MainView`.
2. Hooks into the `Closing` event of the window to trigger the `WindowClosingCommand` in the `MainViewModel` for resource cleanup.

```csharp
using Avalonia.Controls;
using Simple_Player_MVVM.ViewModels;
using System;
using VisioForge.Core.Types;

namespace Simple_Player_MVVM.Views
{
    public partial class MainWindow : Window
    {
        // Helper to get VideoView from the MainView content
        public IVideoView GetVideoView()
        {
            return (Content as MainView).GetVideoView();
        }

        public MainWindow()
        {
            InitializeComponent();

            // Handle the window closing event to trigger cleanup in ViewModel
            Closing += async (sender, e) =>
            {
                if (DataContext is MainViewModel viewModel)
                {
                    // Execute the command and handle potential errors or completion
                    viewModel.WindowClosingCommand.Execute()
                        .Subscribe(_ => { /* Optional: action on completion */ }, 
                                   ex => Console.WriteLine($"Error during closing: {ex.Message}"));
                }
            };
        }
    }
}
```

When the window closes, it checks if the `DataContext` is a `MainViewModel` and then executes its `WindowClosingCommand`. This ensures that the `MainViewModel` can perform necessary cleanup, such as disposing of the `MediaPlayerCoreX` instance and calling `VisioForgeX.DestroySDK()`.

`[SCREENSHOT: MainWindow.axaml.cs code, highlighting the Closing event handler]`

## 6. Platform-Specific Implementation Details

While Avalonia and .NET provide a high degree of cross-platform compatibility, certain aspects like file system access and permissions require platform-specific handling.

### 6.1. Interfaces for Platform Services

To abstract platform-specific functionality, interfaces are defined in the core `SimplePlayerMVVM` project:

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

    This interface is used to get the Android `Context`, which is needed for operations like converting content URIs to file paths.

* **`IDocumentPickerService.cs`:**

    ```csharp
    using System.Threading.Tasks;

    namespace SimplePlayerMVVM;

    public interface IDocumentPickerService
    {
        Task<object?> PickVideoAsync();
    }
    ```

    This interface abstracts the file picking mechanism, specifically for iOS where a native document picker is preferred.

### 6.2. Android Implementation (`SimplePlayerMVVM.Android` project)

`[SCREENSHOT: Android project structure or MainActivity.cs]`

* **`MainActivity.cs`:**
  * Inherits from `AvaloniaMainActivity<App>` and implements `IAndroidHelper`.
  * **`CustomizeAppBuilder`:** Standard Avalonia Android setup.
  * **Permissions:** In `OnCreate`, it calls `RequestPermissionsAsync` to request necessary permissions like `Manifest.Permission.Internet`, `Manifest.Permission.ReadExternalStorage`, and `Manifest.Permission.ReadMediaVideo` (for newer Android versions).

        ```csharp
        // In MainActivity.cs
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainViewModel.AndroidHelper = this; // Provide IAndroidHelper implementation to ViewModel
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

        public Context GetContext() // IAndroidHelper implementation
        {
            return this;
        }
        ```

  * The `MainViewModel.AndroidHelper = this;` line makes the `MainActivity` instance (which implements `IAndroidHelper`) available to the `MainViewModel` for getting the Android context.
  * The Android Manifest (`AndroidManifest.xml`, not explicitly shown in the provided files but essential) must also declare these permissions.

* **File Path Handling:** As seen in `MainViewModel.OpenFileAsync`, the `FileDialogHelper.GetFilePathFromUri` method uses the context obtained via `IAndroidHelper` to resolve file paths from content URIs, which is common when using Android's file picker.

* **Project File (`SimplePlayerMVVM.Android.csproj`):** Configures the Android-specific build, target SDK versions, and includes necessary VisioForge Android libraries.

### 6.3. iOS Implementation (`SimplePlayerMVVM.iOS` project)

`[SCREENSHOT: iOS project structure or AppDelegate.cs]`

* **`AppDelegate.cs`:**
  * Inherits from `AvaloniaAppDelegate<App>`.
  * **`CustomizeAppBuilder`:** Registers the `IOSDocumentPickerService` with the Splat dependency resolver (`Locator.CurrentMutable.RegisterConstant`). This makes the `IDocumentPickerService` available for injection or service location in the `MainViewModel`.

        ```csharp
        // In AppDelegate.cs
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            Locator.CurrentMutable.RegisterConstant(new IOSDocumentPickerService(), typeof(IDocumentPickerService));
            
            return base.CustomizeAppBuilder(builder)
                .WithInterFont()
                .UseReactiveUI();
        }
        ```

* **`IOSDocumentPickerService.cs`:**
  * Implements `IDocumentPickerService`.
  * Uses `UIDocumentPickerViewController` to present the native iOS file picker for video files (`UTType.Video`, `UTType.Movie`).
  * Handles the `DidPickDocumentAtUrls` and `WasCancelled` events from the picker.
  * Returns the selected file URL (`NSUrl`) via a `TaskCompletionSource`.
  * Includes utility code (`GetTopViewController`) to find the topmost view controller to present the picker from.

    ```csharp
    // Snippet from IOSDocumentPickerService.cs
    public Task<object?> PickVideoAsync()
    {
        _tcs = new TaskCompletionSource<object?>();
        string[] allowedUTIs = { UTType.Video, UTType.Movie };
        var picker = new UIDocumentPickerViewController(allowedUTIs, UIDocumentPickerMode.Import);
        // ... event subscriptions and presentation ...
        return _tcs.Task;
    }

    private void OnDocumentPicked(object sender, UIDocumentPickedAtUrlsEventArgs e)
    {
        // ... handles picked URL, resolves _tcs ...
        NSUrl fileUrl = e.Urls[0];
        _tcs?.TrySetResult(fileUrl);
    }
    ```

    `[SCREENSHOT: IOSDocumentPickerService.cs showing picker setup or result handling]`

* **`Info.plist`:**
  * This file is crucial for iOS apps. It must include keys like `NSPhotoLibraryUsageDescription` if accessing the photo library, or other relevant permissions depending on where files are stored/accessed. The provided `Info.plist` includes:

        ```xml
        <key>NSPhotoLibraryUsageDescription</key>
        <string>Photo library used to play files</string>
        ```

  * It also defines bundle identifiers, version numbers, supported orientations, etc.

* **Project File (`SimplePlayerMVVM.iOS.csproj`):** Configures the iOS-specific build, target OS version, and includes VisioForge iOS libraries.

### 6.4. Desktop Implementation (`SimplePlayerMVVM.Desktop` project)

`[SCREENSHOT: Desktop project structure or Program.cs]`

* **`Program.cs`:**
  * Contains the `Main` entry point for desktop applications.
  * Uses `BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);` to initialize and run the Avalonia application for desktop.
  * `BuildAvaloniaApp()` configures Avalonia with platform detection, fonts, ReactiveUI, and logging.

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

* **File Access:** On desktop, file access is generally more straightforward. The `MainViewModel` uses `TopLevel.StorageProvider.OpenFilePickerAsync` which works across Windows, macOS, and Linux without specific helper services like those for Android or iOS URI/permission complexities.

* **Project File (`SimplePlayerMVVM.Desktop.csproj`):**
  * Targets specific desktop frameworks (e.g., `net8.0-windows`, `net8.0-macos14.0`, `net8.0` for Linux).
  * Includes `Avalonia.Desktop`.
  * Includes platform-specific VisioForge native libraries for Windows (x64) and macOS through `PackageReference` conditions.

        ```xml
        <ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
          <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="..." />
          <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="..." />
        </ItemGroup>
        <ItemGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
          <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="..." />
        </ItemGroup>
        ```

* **Windows Manifest (`app.manifest`):** Used on Windows for application settings, such as compatibility with specific Windows versions.
* **macOS Info (`Info.plist` in Desktop project):** Provides bundle information for macOS applications.

These platform-specific projects and configurations ensure that the shared core logic in `SimplePlayerMVVM` can interact correctly with the native features and requirements of each operating system.

## 7. Key VisioForge SDK Components Used

This application leverages several key components from the VisioForge Media Player SDK X:

* **`VisioForge.Core.MediaPlayerX.MediaPlayerCoreX`:**
  * The central engine for media playback. It handles opening media sources, controlling playback (play, pause, stop, seek, rate), managing audio and video, and providing status information (position, duration).
  * Initialized in `MainViewModel` and linked to the `VideoViewIntf`.
* **`VisioForge.Core.UI.Avalonia.VideoView`:**
  * An Avalonia control that serves as the rendering surface for video. It is declared in `MainView.axaml` and its reference (`IVideoView`) is passed to `MediaPlayerCoreX`.
* **`VisioForge.Core.Types.X.Sources.UniversalSourceSettings`:**
  * Used to configure the media source. `UniversalSourceSettings.CreateAsync(filename)` automatically determines the best way to open a given file or URL.
* **`VisioForge.Core.VisioForgeX` static class:**
  * **`InitSDK()`:** Initializes the VisioForge SDK. This must be called once at application startup (done in `MainViewModel` constructor in this example, but can also be done in `App.axaml.cs`).
  * **`DestroySDK()`:** Releases all SDK resources. This must be called when the application is closing to prevent resource leaks (done in `MainViewModel.OnWindowClosing`).
* **Platform-Specific Libraries:**
  * As detailed in the project setup and platform-specific sections, various NuGet packages like `VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Core.macOS`, `VisioForge.CrossPlatform.Core.Android`, and `VisioForge.CrossPlatform.Core.iOS` provide the necessary native binaries and bindings for each platform.
* **`VisioForge.Core.UI.Android.FileDialogHelper` (for Android):**
  * Contains helper methods like `GetFilePathFromUri` to work with Android's file system and content URIs.

Understanding these components is crucial for working with the VisioForge SDK and extending the player's functionality.

`[SCREENSHOT: Diagram showing interaction between MainViewModel, MediaPlayerCoreX, and VideoView]`

## 8. Building and Running the Application

1. **Clone/Download the Source Code:** Obtain the `SimplePlayerMVVM` example project.
2. **Restore NuGet Packages:** Open the solution in your IDE and ensure all NuGet packages are restored for all projects.
3. **Select Startup Project and Target:**
    * **Desktop:** Set `SimplePlayerMVVM.Desktop` as the startup project. You can then run it directly on Windows, macOS, or Linux (ensure you have the .NET runtime for your OS).
    * **Android:** Set `SimplePlayerMVVM.Android` as the startup project. Select an Android emulator or connected device. Build and deploy.
        * Ensure Android SDK and emulators/devices are correctly configured in your IDE.
        * You might need to accept permission prompts on the device/emulator upon first launch.
    * **iOS:** Set `SimplePlayerMVVM.iOS` as the startup project. Select an iOS simulator or connected device (requires a macOS build machine and appropriate Apple Developer provisioning).
        * Ensure Xcode and developer tools are correctly configured.
        * You might need to trust the developer certificate on the device.
4. **Build and Run:** Build the selected startup project and run it.

`[SCREENSHOT: IDE showing startup project selection and run button]`
`[SCREENSHOT: Application running on Android emulator]`
`[SCREENSHOT: Application running on iOS simulator]`
`[SCREENSHOT: Application running on Windows/macOS/Linux desktop]`

## 9. Conclusion

This guide has demonstrated how to build a cross-platform media player using Avalonia UI with the MVVM pattern and the VisioForge Media Player SDK X. By leveraging a shared core project for ViewModels and Views, and handling platform-specifics in dedicated head projects, we can create a maintainable application that runs on a wide range of devices.

Key takeaways:

* The MVVM pattern helps separate concerns and improves testability.
* ReactiveUI simplifies implementing MVVM with Avalonia.
* VisioForge Media Player SDK X provides powerful media playback capabilities, with `MediaPlayerCoreX` as the core engine and `VideoView` for Avalonia UI integration.
* Platform-specific considerations, especially for file access and permissions, are handled through interfaces and platform-specific implementations.
* Proper initialization (`VisioForgeX.InitSDK()`) and cleanup (`VisioForgeX.DestroySDK()`) of the VisioForge SDK are essential.

You can extend this example by adding more features like playlist support, network streaming, video effects, or more advanced UI controls.
