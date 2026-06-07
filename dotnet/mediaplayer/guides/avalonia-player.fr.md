---
title: Lecteur vidéo Avalonia en C# .NET — Lecture multiplateforme
description: Intégrez la lecture vidéo dans des apps Avalonia (Windows/macOS/Linux). H.264, HEVC, RTSP, HLS. API XAML. Exemple C# complet avec contrôle MediaPlayer.
tags:
  - Media Player SDK
  - .NET
  - C++
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - GStreamer
  - Playback
  - Streaming
  - RTSP
  - HLS
  - MPEG-DASH
  - MP4
  - H.264
  - H.265
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - MediaPlayerCoreX
  - IVideoView
  - UniversalSourceSettings
  - ErrorsEventArgs

---

# Construire un lecteur vidéo multiplateforme en C# avec Avalonia UI

Construisez un lecteur vidéo en C# qui s'exécute sur **Windows, macOS, Linux, Android et iOS** à partir d'une seule base de code Avalonia UI. Ce guide utilise le patron MVVM avec le moteur `MediaPlayerCoreX` de [VisioForge Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) et le contrôle `VideoView`, et fait référence à la [démo SimplePlayerMVVM](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia/SimplePlayerMVVM) sur GitHub pour chaque extrait présenté.

Formats pris en charge nativement : **H.264, H.265/HEVC, VP9, AV1, MP4, MKV, WebM, RTSP, HLS, MPEG-DASH**.

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Démarrage rapide

Pour un lecteur vidéo Avalonia minimal absolu — sans MVVM, sans plusieurs plateformes — ajoutez `VideoView` à un fichier AXAML et reliez-le à `MediaPlayerCoreX` dans le code-behind :

```xml
<UserControl xmlns:vf="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia">
    <vf:VideoView x:Name="videoView" Background="Black" />
</UserControl>
```

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

public partial class MainView : UserControl
{
    private MediaPlayerCoreX _player;

    public MainView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            _player = new MediaPlayerCoreX(videoView);
            var src = await UniversalSourceSettings.CreateAsync("video.mp4");
            await _player.OpenAsync(src);
            await _player.PlayAsync();
        };
    }
}
```

Voilà l'image complète : `VideoView` sur l'UI, instance `MediaPlayerCoreX` liée à celui-ci, `OpenAsync` + `PlayAsync`. Tout ce qui suit sur cette page ajoute des fonctionnalités de production par-dessus — MVVM, sélecteurs de fichiers spécifiques aux plateformes, positionnement, volume, vitesse, nettoyage et déploiement pour chaque OS cible.

Le reste de ce guide fait référence au projet de démo [`SimplePlayerMVVM`](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia/SimplePlayerMVVM) pour parcourir l'implémentation complète de production.

## 1. Prérequis

Avant de commencer, assurez-vous que les éléments suivants sont installés :

* SDK .NET (dernière version, par exemple .NET 8 ou plus récent)
* Un IDE comme Visual Studio, JetBrains Rider ou VS Code avec les extensions C# et Avalonia.
* Pour le développement Android :
  * Android SDK
  * Java Development Kit (JDK)
* Pour le développement iOS (nécessite une machine macOS) :
  * Xcode
  * Profils d'approvisionnement et certificats nécessaires.
* SDK .NET VisioForge (MediaPlayer SDK X). Vous pouvez l'obtenir sur le site VisioForge. Les paquets nécessaires seront ajoutés via NuGet.

## 2. Configuration du projet

Cette section décrit comment configurer la structure de la solution et inclure les paquets SDK VisioForge nécessaires.

### 2.1. Structure de la solution

La solution `SimplePlayerMVVM` se compose de plusieurs projets :

* **SimplePlayerMVVM** : une bibliothèque .NET Standard contenant la logique principale de l'application, notamment les ViewModels, les Views (AXAML) et les interfaces partagées. C'est le projet principal où réside la plupart de la logique de notre application.
* **SimplePlayerMVVM.Android** : le projet principal spécifique à Android.
* **SimplePlayerMVVM.Desktop** : le projet principal spécifique au bureau (Windows, macOS, Linux).
* **SimplePlayerMVVM.iOS** : le projet principal spécifique à iOS.

### 2.2. Projet principal (`SimplePlayerMVVM.csproj`)

Le projet principal, `SimplePlayerMVVM.csproj`, cible plusieurs plateformes. Les références de paquets clés incluent :

* `Avalonia` : le framework UI Avalonia principal.
* `Avalonia.Themes.Fluent` : fournit un thème Fluent Design.
* `Avalonia.ReactiveUI` : pour la prise en charge MVVM avec ReactiveUI.
* `VisioForge.DotNet.MediaBlocks` : composants principaux de traitement multimédia VisioForge.
* `VisioForge.DotNet.Core.UI.Avalonia` : composants UI VisioForge pour Avalonia, incluant le `VideoView`.

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

Cette configuration permet à la logique principale d'être partagée sur toutes les plateformes cibles.

### 2.3. Projets spécifiques aux plateformes

Chaque projet principal de plateforme (`SimplePlayerMVVM.Android.csproj`, `SimplePlayerMVVM.Desktop.csproj`, `SimplePlayerMVVM.iOS.csproj`) inclut les dépendances et configurations spécifiques à la plateforme.

**Bureau (`SimplePlayerMVVM.Desktop.csproj`) :**

* Référence `Avalonia.Desktop`.
* Inclut les bibliothèques natives VisioForge spécifiques à la plateforme (par exemple `VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Core.macOS`).

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

**Android (`SimplePlayerMVVM.Android.csproj`) :**

* Référence `Avalonia.Android`.
* Inclut les bibliothèques et dépendances VisioForge spécifiques à Android comme `VisioForge.CrossPlatform.Core.Android`.

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

**iOS (`SimplePlayerMVVM.iOS.csproj`) :**

* Référence `Avalonia.iOS`.
* Inclut les bibliothèques VisioForge spécifiques à iOS comme `VisioForge.CrossPlatform.Core.iOS`.

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

Ces fichiers de projet sont cruciaux pour gérer les dépendances et les configurations de build pour chaque plateforme.

## 3. Structure MVVM principale

L'application suit le patron MVVM, séparant l'UI (Views) de la logique (ViewModels) et des données (Models). ReactiveUI est utilisé pour faciliter ce patron.

### 3.1. `ViewModelBase.cs`

Cette classe abstraite sert de base à tous les ViewModels de l'application. Elle hérite de `ReactiveObject`, qui fait partie de ReactiveUI et fournit l'infrastructure nécessaire aux notifications de changement de propriété.

```csharp
using ReactiveUI;

namespace Simple_Player_MVVM.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
    }
}
```

Tout ViewModel qui doit notifier l'UI des changements de propriété doit hériter de `ViewModelBase`.

### 3.2. `ViewLocator.cs`

La classe `ViewLocator` est responsable de localiser et d'instancier les Views en fonction du type de leur ViewModel correspondant. Elle implémente l'interface `IDataTemplate` d'Avalonia.

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

Lorsqu'Avalonia doit afficher un ViewModel, la méthode `Match` du `ViewLocator` vérifie si l'objet de données est un `ViewModelBase`. Si c'est le cas, la méthode `Build` tente de trouver une View correspondante en remplaçant « ViewModel » par « View » dans le nom de la classe ViewModel et l'instancie.

Cette approche basée sur une convention simplifie l'association entre Views et ViewModels.

### 3.3. Initialisation de l'application (`App.axaml` et `App.axaml.cs`)

Le fichier `App.axaml` définit les ressources au niveau de l'application, notamment le `ViewLocator` comme template de données et le thème (FluentTheme).

**`App.axaml`** :

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

**`App.axaml.cs`** :
Le fichier `App.axaml.cs` gère l'initialisation et le cycle de vie de l'application.

Responsabilités clés dans `OnFrameworkInitializationCompleted` :

1. Crée une instance de `MainViewModel`.
2. Configure la fenêtre principale ou la vue en fonction du cycle de vie de l'application (`IClassicDesktopStyleApplicationLifetime` pour le bureau, `ISingleViewApplicationLifetime` pour les vues de type mobile/web).
3. Assigne l'instance `MainViewModel` comme `DataContext` pour la fenêtre/vue principale.
4. Récupère l'instance `IVideoView` depuis le `MainView` (hébergé dans `MainWindow` ou directement comme `MainView`).
5. Passe l'`IVideoView` et le contrôle `TopLevel` (nécessaire pour les dialogues de fichiers et autres interactions de niveau supérieur) au `MainViewModel`.

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

Cette configuration garantit que le `MainViewModel` a accès aux composants UI nécessaires pour la lecture vidéo et l'interaction, quelle que soit la plateforme.

## 4. Implémentation du MainViewModel (`MainViewModel.cs`)

Le `MainViewModel` est au cœur de la fonctionnalité du lecteur multimédia. Il gère l'état du lecteur, gère les interactions utilisateur et communique avec le moteur `MediaPlayerCoreX` de VisioForge.

Composants clés du `MainViewModel` :

### 4.1. Propriétés pour la liaison UI

Le ViewModel expose plusieurs propriétés qui sont liées aux éléments UI dans `MainView.axaml`. Ces propriétés utilisent `RaiseAndSetIfChanged` de `ReactiveUI` pour notifier l'UI des changements.

* **`VideoViewIntf` (IVideoView) :** une référence au contrôle `VideoView` dans l'UI, passée depuis `App.axaml.cs`.
* **`TopLevel` (TopLevel) :** une référence au contrôle de niveau supérieur, utilisée pour afficher les dialogues de fichiers.
* **`Position` (string?) :** position de lecture actuelle (par exemple « 00:01:23 »).
* **`Duration` (string?) :** durée totale du fichier multimédia (par exemple « 00:05:00 »).
* **`Filename` (string? ou Foundation.NSUrl? pour iOS) :** le nom ou le chemin du fichier actuellement chargé.
* **`VolumeValue` (double?) :** niveau de volume actuel (0-100).
* **`PlayPauseText` (string?) :** texte du bouton Lire/Pause (par exemple « PLAY » ou « PAUSE »).
* **`SpeedText` (string?) :** texte indiquant la vitesse de lecture actuelle (par exemple « SPEED: 1X »).
* **`SeekingValue` (double?) :** valeur actuelle du curseur de positionnement.
* **`SeekingMaximum` (double?) :** valeur maximale du curseur de positionnement (correspond à la durée du média en millisecondes).

```csharp
// Exemple de propriété
private string? _Position = "00:00:00";
public string? Position
{
    get => _Position;
    set => this.RaiseAndSetIfChanged(ref _Position, value);
}

// ... autres propriétés ...
```

### 4.2. Commandes pour les interactions UI

Les instances `ReactiveCommand` de ReactiveUI sont utilisées pour gérer les actions déclenchées par les éléments UI (par exemple les clics de bouton, les changements de valeur de curseur).

* **`OpenFileCommand` :** ouvre un dialogue de fichier pour sélectionner un fichier multimédia.
* **`PlayPauseCommand` :** lit ou met en pause le média.
* **`StopCommand` :** arrête la lecture.
* **`SpeedCommand` :** parcourt les vitesses de lecture (1x, 2x, 0.5x).
* **`VolumeValueChangedCommand` :** met à jour le volume du lecteur lorsque le curseur de volume change.
* **`SeekingValueChangedCommand` :** se positionne à une nouvelle position lorsque le curseur de positionnement change.
* **`WindowClosingCommand` :** gère le nettoyage à la fermeture de la fenêtre de l'application.

```csharp
// Constructeur — Initialisation des commandes
public MainViewModel()
{
    OpenFileCommand = ReactiveCommand.Create(OpenFileAsync);
    PlayPauseCommand = ReactiveCommand.CreateFromTask(PlayPauseAsync);
    StopCommand = ReactiveCommand.CreateFromTask(StopAsync);
    // ... autres initialisations de commandes ...

    // S'abonner aux changements de propriété pour déclencher les commandes pour les curseurs
    this.WhenAnyValue(x => x.VolumeValue).Subscribe(_ => VolumeValueChangedCommand.Execute().Subscribe());
    this.WhenAnyValue(x => x.SeekingValue).Subscribe(_ => SeekingValueChangedCommand.Execute().Subscribe());

    _tmPosition = new System.Timers.Timer(1000); // Timer pour les mises à jour de position
    _tmPosition.Elapsed += tmPosition_Elapsed;

    VisioForgeX.InitSDK(); // Initialiser le SDK VisioForge
}
```

Note : `VisioForgeX.InitSDK()` initialise le SDK VisioForge. Cela doit être appelé une fois au démarrage de l'application.

### 4.3. Intégration `MediaPlayerCoreX` VisioForge

Un champ privé `_player` de type `MediaPlayerCoreX` détient l'instance du moteur de lecteur multimédia VisioForge.

```csharp
private MediaPlayerCoreX _player;
```

### 4.4. Création du moteur (`CreateEngineAsync`)

Cette méthode asynchrone initialise ou réinitialise l'instance `MediaPlayerCoreX`.

```csharp
private async Task CreateEngineAsync()
{
    if (_player != null)
    {
        await _player.StopAsync();
        await _player.DisposeAsync();
    }

    _player = new MediaPlayerCoreX(VideoViewIntf); // Passer le VideoView Avalonia
    _player.OnError += _player_OnError; // S'abonner aux événements d'erreur
    _player.Audio_Play = true; // S'assurer que l'audio est activé

    // Créer les paramètres de source à partir du nom de fichier
    var sourceSettings = await UniversalSourceSettings.CreateAsync(Filename);
    await _player.OpenAsync(sourceSettings);
}
```

Étapes clés :

1. Libère toute instance de lecteur existante.
2. Crée un nouveau `MediaPlayerCoreX`, en passant l'`IVideoView` depuis l'UI.
3. S'abonne à l'événement `OnError` pour la gestion des erreurs.
4. Définit `Audio_Play = true` pour activer la lecture audio par défaut.
5. Utilise `UniversalSourceSettings.CreateAsync(Filename)` pour créer des paramètres de source adaptés au fichier sélectionné.
6. Ouvre la source multimédia avec `_player.OpenAsync(sourceSettings)`.

### 4.5. Ouverture de fichier (`OpenFileAsync`)

Cette méthode est responsable de permettre à l'utilisateur de sélectionner un fichier multimédia.

```csharp
private async Task OpenFileAsync()
{
    await StopAllAsync(); // Arrêter toute lecture actuelle
    PlayPauseText = "PLAY";

#if __IOS__ && !__MACCATALYST__
    // Spécifique iOS : utiliser IDocumentPickerService
    var filePicker = Locator.Current.GetService<IDocumentPickerService>();
    var res = await filePicker.PickVideoAsync();
    if (res != null)
    {
        Filename = (Foundation.NSUrl)res;
        var access = IOSHelper.CheckFileAccess(Filename); // Helper pour vérifier l'accès au fichier
        if (!access)
        {
            IOSHelper.ShowToast("File access error");
            return;
        }
    }
#else
    // Autres plateformes : utiliser StorageProvider d'Avalonia
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
            // Spécifique Android : convertir l'URI de contenu en chemin de fichier si nécessaire
            if (!Filename.StartsWith('/'))
            {
                Filename = global::VisioForge.Core.UI.Android.FileDialogHelper.GetFilePathFromUri(AndroidHelper.GetContext(), file.Path);
            }
#endif
        }
    }
    catch (Exception ex)
    {
        // Gérer l'annulation ou les erreurs
        Debug.WriteLine($"File open error: {ex.Message}");
    }
#endif
}
```

Considérations spécifiques aux plateformes :

* **iOS :** utilise un `IDocumentPickerService` (résolu via `Locator.Current.GetService`) pour présenter le sélecteur de documents natif. `IOSHelper.CheckFileAccess` est utilisé pour s'assurer que l'application a la permission d'accéder au fichier sélectionné. Le nom de fichier est stocké en tant que `NSUrl`.
* **Android :** si le chemin obtenu depuis le sélecteur de fichier est un URI de contenu, `FileDialogHelper.GetFilePathFromUri` (depuis `VisioForge.Core.UI.Android`) est utilisé pour le convertir en chemin de fichier réel. Cela nécessite un `IAndroidHelper` pour obtenir le contexte Android.
* **Bureau/Autres :** utilise `TopLevel.StorageProvider.OpenFilePickerAsync` pour le dialogue de fichier Avalonia standard.

### 4.6. Contrôles de lecture

* **`PlayPauseAsync` :**
  * Si le lecteur n'est pas initialisé ou arrêté (`PlaybackState.Free`), il appelle `CreateEngineAsync` puis `_player.PlayAsync()`.
  * S'il lit (`PlaybackState.Play`), il appelle `_player.PauseAsync()`.
  * S'il est en pause (`PlaybackState.Pause`), il appelle `_player.ResumeAsync()`.
  * Met à jour `PlayPauseText` en conséquence et démarre/arrête le timer `_tmPosition`.

    ```csharp
    private async Task PlayPauseAsync()
    {
        // ... (vérification null/empty filename) ...

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

* **`StopAsync` :**
  * Appelle `StopAllAsync` pour arrêter le lecteur et réinitialiser les éléments UI.
  * Réinitialise `SpeedText` et `PlayPauseText`.

    ```csharp
    private async Task StopAsync()
    {
        await StopAllAsync();
        SpeedText = "SPEED: 1X";
        PlayPauseText = "PLAY";
    }
    ```

* **`StopAllAsync` (Helper) :**
  * Arrête le timer `_tmPosition`.
  * Appelle `_player.StopAsync()`.
  * Réinitialise `SeekingMaximum` à null (afin qu'il soit recalculé à la prochaine lecture).

    ```csharp
    private async Task StopAllAsync()
    {
        if (_player == null) return;
        _tmPosition.Stop();
        if (_player != null) await _player.StopAsync();
        await Task.Delay(300); // Petit délai pour s'assurer que l'arrêt se termine
        SeekingMaximum = null;
    }
    ```

### 4.7. Vitesse de lecture (`SpeedAsync`)

Parcourt les vitesses de lecture : 1.0, 2.0 et 0.5.

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
    else if (SpeedText == "SPEED: 0.5X") // Suppose que c'était l'état précédent
    {
        SpeedText = "SPEED: 1X";
        await _player.Rate_SetAsync(1.0);
    }
}
```

Utilise `_player.Rate_SetAsync(double rate)` pour changer la vitesse de lecture.

### 4.8. Mises à jour de position et de durée (`tmPosition_Elapsed`)

Cette méthode est appelée par le timer `_tmPosition` (typiquement chaque seconde) pour mettre à jour l'UI avec la position et la durée de lecture actuelles.

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

            _isTimerUpdate = true; // Flag pour empêcher la boucle de positionnement

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

Actions clés :

1. Récupère la position actuelle (`_player.Position_GetAsync()`) et la durée (`_player.DurationAsync()`).
2. Met à jour `SeekingMaximum` s'il n'a pas encore été défini (généralement après l'ouverture d'un fichier).
3. Met à jour `SeekingValue` avec la progression actuelle.
4. Formate et met à jour les chaînes `Position` et `Duration`.
5. Utilise `Dispatcher.UIThread.InvokeAsync` pour garantir que les mises à jour UI se font sur le thread UI.
6. Définit `_isTimerUpdate = true` avant la mise à jour de `SeekingValue` et `false` après, pour empêcher le gestionnaire `OnSeekingValueChanged` de se re-positionner lorsque le timer met à jour la position du curseur.

### 4.9. Positionnement (`OnSeekingValueChanged`)

Appelée lorsque la propriété `SeekingValue` change (c'est-à-dire que l'utilisateur déplace le curseur de positionnement).

```csharp
private async Task OnSeekingValueChanged()
{
    if (!_isTimerUpdate && _player != null && SeekingValue.HasValue)
    {
        await _player.Position_SetAsync(TimeSpan.FromMilliseconds(SeekingValue.Value));
    }
}
```

Si elle n'est pas en cours de mise à jour par le timer (`!_isTimerUpdate`), elle appelle `_player.Position_SetAsync()` pour se positionner à la nouvelle position.

### 4.10. Contrôle du volume (`OnVolumeValueChanged`)

Appelée lorsque la propriété `VolumeValue` change (c'est-à-dire que l'utilisateur déplace le curseur de volume).

```csharp
private void OnVolumeValueChanged()
{
    if (_player != null && VolumeValue.HasValue)
    {
        // Le volume pour MediaPlayerCoreX est de 0.0 à 1.0
        _player.Audio_OutputDevice_Volume = VolumeValue.Value / 100.0;
    }
}
```

Définit `_player.Audio_OutputDevice_Volume`. Notez que le ViewModel utilise une échelle 0-100 pour `VolumeValue`, tandis que le lecteur attend 0.0-1.0.

### 4.11. Gestion des erreurs (`_player_OnError`)

Un gestionnaire d'erreur simple qui journalise les erreurs dans la console de débogage.

```csharp
private void _player_OnError(object sender, VisioForge.Core.Types.Events.ErrorsEventArgs e)
{
    Debug.WriteLine(e.Message);
}
```

Une gestion d'erreur plus sophistiquée (par exemple afficher un message à l'utilisateur) pourrait être implémentée ici.

### 4.12. Nettoyage des ressources (`OnWindowClosing`)

Cette méthode est invoquée à la fermeture de la fenêtre principale. Elle garantit que les ressources SDK VisioForge sont correctement libérées.

```csharp
private void OnWindowClosing()
{
    if (_player != null)
    {
        _player.OnError -= _player_OnError; // Se désabonner des événements
        _player.Stop(); // S'assurer que le lecteur est arrêté (version synchrone ici pour un nettoyage rapide)
        _player.Dispose();
        _player = null;
    }

    VisioForgeX.DestroySDK(); // Détruire l'instance du SDK VisioForge
}
```

Elle arrête le lecteur, le libère, et surtout, appelle `VisioForgeX.DestroySDK()` pour libérer toutes les ressources du SDK. C'est crucial pour empêcher les fuites de mémoire ou les problèmes à la sortie de l'application.

Ce ViewModel orchestre toute la logique principale du lecteur multimédia, du chargement des fichiers au contrôle de la lecture et à l'interaction avec le SDK VisioForge.

## 5. Interface utilisateur (Views)

L'interface utilisateur est définie en utilisant Avalonia XAML (fichiers `.axaml`).

### 5.1. `MainView.axaml` — L'interface du lecteur

Ce `UserControl` définit la disposition et les contrôles du lecteur multimédia.

**Éléments UI clés :**

* **`avalonia:VideoView` :** c'est le contrôle VisioForge responsable du rendu vidéo. Il est placé dans la zone principale de la grille et configuré pour s'étirer.

    ```xml
    <avalonia:VideoView x:Name="videoView1" Margin="0,0,0,0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="#0C0C0C"  />
    ```

* **Curseur de positionnement (`Slider Name="slSeeking"`) :**
  * `Maximum="{Binding SeekingMaximum}"` : se lie à la propriété `SeekingMaximum` dans `MainViewModel`.
  * `Value="{Binding SeekingValue}"` : se lie en bidirectionnel à la propriété `SeekingValue` dans `MainViewModel`. Les changements de ce curseur par l'utilisateur mettront à jour `SeekingValue`, déclenchant `OnSeekingValueChanged`. Les mises à jour de `SeekingValue` depuis le ViewModel (par exemple par le timer) mettront à jour la position du curseur.
* **Affichage du temps (`TextBlock` pour Position et Durée) :**
  * Liés aux propriétés `Position` et `Duration` dans `MainViewModel`.
  * `TextBlock Text="{Binding Filename}"` affiche le nom du fichier actuel.
* **Boutons de contrôle de lecture (`Button`) :**
  * **Ouvrir un fichier :** `Command="{Binding OpenFileCommand}"`
  * **Lire/Pause :** `Command="{Binding PlayPauseCommand}"`, `Content="{Binding PlayPauseText}"` (change dynamiquement le texte du bouton).
  * **Arrêter :** `Command="{Binding StopCommand}"`
* **Contrôles de volume et de vitesse :**
  * **Curseur de volume :** `Value="{Binding VolumeValue}"` (se lie à `VolumeValue` pour le contrôle du volume).
  * **Bouton de vitesse :** `Command="{Binding SpeedCommand}"`, `Content="{Binding SpeedText}"`.

**Disposition :**
La vue utilise une `Grid` pour disposer le `VideoView` et un `StackPanel` pour les contrôles en bas. Les contrôles eux-mêmes sont organisés en utilisant des `StackPanel` et `Grid` imbriqués pour l'alignement.

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
    <!-- Placeholder pour la vue vidéo -->
    <Border Grid.Row="0" Background="Black" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
      <avalonia:VideoView x:Name="videoView1" Margin="0,0,0,0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Background="#0C0C0C"  />
    </Border>

    <!-- Contrôles -->
    <StackPanel Grid.Row="1" Background="#1e1e1e" Orientation="Vertical">
      <!-- Curseur pour le positionnement -->
      <Slider Name="slSeeking" Margin="16,16,16,0" VerticalAlignment="Center" Maximum="{Binding SeekingMaximum}" Value="{Binding SeekingValue}"/>

      <!-- Affichage du temps et du nom de fichier -->
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

      <!-- Contrôles de lecture -->
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Center" Margin="16,0,5,0">
        <Button Command="{Binding OpenFileCommand}" Content="OPEN FILE" Margin="5" VerticalAlignment="Center"/>
        <Button Name="btPlayPause" Command="{Binding PlayPauseCommand}" Content="{Binding PlayPauseText}" Margin="5"/>
        <Button Name="btStop" Command="{Binding StopCommand}" Content="STOP" Margin="5"/>
      </StackPanel>

      <!-- Contrôles de volume et de vitesse -->
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" Margin="16,0,5,5">
        <TextBlock Text="Volume" Foreground="White" VerticalAlignment="Center"/>
        <Slider Value="{Binding VolumeValue}" Minimum="0" Maximum="100" Width="150" Margin="15,0,5,0" VerticalAlignment="Center"/>
        <Button Command="{Binding SpeedCommand}" Content="{Binding SpeedText}" Margin="5"/>
      </StackPanel>
    </StackPanel>
  </Grid>
</UserControl>
```

La directive `x:DataType="vm:MainViewModel"` active les liaisons compilées, offrant de meilleures performances et une vérification au moment de la compilation des chemins de liaison. Le `Design.DataContext` est utilisé pour fournir des données pour l'aperçu XAML dans les IDE.

### 5.2. `MainView.axaml.cs` — Code-Behind

Le code-behind de `MainView` est minimal. Son objectif principal est de fournir un moyen pour le code de configuration de l'application (dans `App.axaml.cs`) d'accéder à l'instance du contrôle `VideoView`.

```csharp
using Avalonia.Controls;
using VisioForge.Core.Types;

namespace Simple_Player_MVVM.Views
{
    public partial class MainView : UserControl
    {
        // Fournit l'accès à l'instance du contrôle VideoView
        public IVideoView GetVideoView()
        {
            return videoView1; // videoView1 est le x:Name du VideoView dans XAML
        }

        public MainView()
        {
            InitializeComponent(); // Initialisation standard du contrôle Avalonia
        }
    }
}
```

Cette méthode `GetVideoView()` est appelée pendant le démarrage de l'application pour passer la référence `VideoView` au `MainViewModel`.

### 5.3. `MainWindow.axaml` — La fenêtre principale de l'application (Bureau)

Pour les plateformes bureau, `MainWindow.axaml` sert de fenêtre de niveau supérieur qui héberge le `MainView`.

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

Elle intègre simplement le contrôle `MainView`. Le `DataContext` (qui sera une instance de `MainViewModel`) est typiquement défini dans `App.axaml.cs` lorsque le `MainWindow` est créé.

### 5.4. `MainWindow.axaml.cs` — Code-Behind de la fenêtre principale

Le code-behind de `MainWindow` gère principalement deux choses :

1. Fournit un moyen d'obtenir le `VideoView` depuis le `MainView` contenu.
2. S'accroche à l'événement `Closing` de la fenêtre pour déclencher le `WindowClosingCommand` dans le `MainViewModel` pour le nettoyage des ressources.

```csharp
using Avalonia.Controls;
using Simple_Player_MVVM.ViewModels;
using System;
using VisioForge.Core.Types;

namespace Simple_Player_MVVM.Views
{
    public partial class MainWindow : Window
    {
        // Helper pour obtenir VideoView depuis le contenu du MainView
        public IVideoView GetVideoView()
        {
            return (Content as MainView).GetVideoView();
        }

        public MainWindow()
        {
            InitializeComponent();

            // Gérer l'événement de fermeture de la fenêtre pour déclencher le nettoyage dans le ViewModel
            Closing += async (sender, e) =>
            {
                if (DataContext is MainViewModel viewModel)
                {
                    // Exécuter la commande et gérer les erreurs potentielles ou l'achèvement
                    viewModel.WindowClosingCommand.Execute()
                        .Subscribe(_ => { /* Optionnel : action à l'achèvement */ },
                                   ex => Console.WriteLine($"Error during closing: {ex.Message}"));
                }
            };
        }
    }
}
```

Lorsque la fenêtre se ferme, elle vérifie si le `DataContext` est un `MainViewModel` puis exécute son `WindowClosingCommand`. Cela garantit que le `MainViewModel` peut effectuer le nettoyage nécessaire, comme libérer l'instance `MediaPlayerCoreX` et appeler `VisioForgeX.DestroySDK()`.

## 6. Détails d'implémentation spécifiques aux plateformes

Bien qu'Avalonia et .NET offrent un haut degré de compatibilité multiplateforme, certains aspects comme l'accès au système de fichiers et les permissions nécessitent une gestion spécifique à la plateforme.

### 6.1. Interfaces pour les services de plateforme

Pour abstraire les fonctionnalités spécifiques aux plateformes, des interfaces sont définies dans le projet principal `SimplePlayerMVVM` :

* **`IAndroidHelper.cs` :**

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

    Cette interface est utilisée pour obtenir le `Context` Android, qui est nécessaire pour des opérations comme la conversion d'URI de contenu en chemins de fichiers.

* **`IDocumentPickerService.cs` :**

    ```csharp
    using System.Threading.Tasks;

    namespace SimplePlayerMVVM;

    public interface IDocumentPickerService
    {
        Task<object?> PickVideoAsync();
    }
    ```

    Cette interface abstrait le mécanisme de sélection de fichiers, spécifiquement pour iOS où un sélecteur de documents natif est préféré.

### 6.2. Implémentation Android (projet `SimplePlayerMVVM.Android`)

* **`MainActivity.cs` :**
  * Hérite de `AvaloniaMainActivity<App>` et implémente `IAndroidHelper`.
  * **`CustomizeAppBuilder` :** configuration Avalonia Android standard.
  * **Permissions :** dans `OnCreate`, elle appelle `RequestPermissionsAsync` pour demander les permissions nécessaires comme `Manifest.Permission.Internet`, `Manifest.Permission.ReadExternalStorage` et `Manifest.Permission.ReadMediaVideo` (pour les versions Android plus récentes).

        ```csharp
        // Dans MainActivity.cs
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            MainViewModel.AndroidHelper = this; // Fournir l'implémentation IAndroidHelper au ViewModel
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

        public Context GetContext() // Implémentation IAndroidHelper
        {
            return this;
        }
        ```

  * La ligne `MainViewModel.AndroidHelper = this;` rend l'instance `MainActivity` (qui implémente `IAndroidHelper`) disponible pour le `MainViewModel` afin d'obtenir le contexte Android.
  * Le Manifest Android (`AndroidManifest.xml`, non explicitement présenté dans les fichiers fournis mais essentiel) doit également déclarer ces permissions.

* **Gestion des chemins de fichiers :** comme vu dans `MainViewModel.OpenFileAsync`, la méthode `FileDialogHelper.GetFilePathFromUri` utilise le contexte obtenu via `IAndroidHelper` pour résoudre les chemins de fichiers depuis les URI de contenu, ce qui est courant lors de l'utilisation du sélecteur de fichiers Android.

* **Fichier de projet (`SimplePlayerMVVM.Android.csproj`) :** configure le build spécifique à Android, les versions cibles du SDK et inclut les bibliothèques Android VisioForge nécessaires.

### 6.3. Implémentation iOS (projet `SimplePlayerMVVM.iOS`)

* **`AppDelegate.cs` :**
  * Hérite de `AvaloniaAppDelegate<App>`.
  * **`CustomizeAppBuilder` :** enregistre l'`IOSDocumentPickerService` avec le résolveur de dépendances Splat (`Locator.CurrentMutable.RegisterConstant`). Cela rend l'`IDocumentPickerService` disponible pour l'injection ou la localisation de service dans le `MainViewModel`.

        ```csharp
        // Dans AppDelegate.cs
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            Locator.CurrentMutable.RegisterConstant(new IOSDocumentPickerService(), typeof(IDocumentPickerService));

            return base.CustomizeAppBuilder(builder)
                .WithInterFont()
                .UseReactiveUI();
        }
        ```

* **`IOSDocumentPickerService.cs` :**
  * Implémente `IDocumentPickerService`.
  * Utilise `UIDocumentPickerViewController` pour présenter le sélecteur de fichiers iOS natif pour les fichiers vidéo (`UTType.Video`, `UTType.Movie`).
  * Gère les événements `DidPickDocumentAtUrls` et `WasCancelled` du sélecteur.
  * Retourne l'URL du fichier sélectionné (`NSUrl`) via un `TaskCompletionSource`.
  * Inclut un code utilitaire (`GetTopViewController`) pour trouver le contrôleur de vue le plus haut à partir duquel présenter le sélecteur.

    ```csharp
    // Extrait de IOSDocumentPickerService.cs
    public Task<object?> PickVideoAsync()
    {
        _tcs = new TaskCompletionSource<object?>();
        string[] allowedUTIs = { UTType.Video, UTType.Movie };
        var picker = new UIDocumentPickerViewController(allowedUTIs, UIDocumentPickerMode.Import);
        // ... abonnements aux événements et présentation ...
        return _tcs.Task;
    }

    private void OnDocumentPicked(object sender, UIDocumentPickedAtUrlsEventArgs e)
    {
        // ... gère l'URL sélectionnée, résout _tcs ...
        NSUrl fileUrl = e.Urls[0];
        _tcs?.TrySetResult(fileUrl);
    }
    ```

* **`Info.plist` :**
  * Ce fichier est crucial pour les apps iOS. Il doit inclure des clés comme `NSPhotoLibraryUsageDescription` si on accède à la photothèque, ou d'autres permissions pertinentes selon l'endroit où les fichiers sont stockés/accessibles. L'`Info.plist` fourni inclut :

        ```xml
        <key>NSPhotoLibraryUsageDescription</key>
        <string>Photo library used to play files</string>
        ```

  * Il définit également les identifiants de bundle, numéros de version, orientations prises en charge, etc.

* **Fichier de projet (`SimplePlayerMVVM.iOS.csproj`) :** configure le build spécifique à iOS, la version cible de l'OS et inclut les bibliothèques iOS VisioForge.

### 6.4. Implémentation Bureau (projet `SimplePlayerMVVM.Desktop`)

* **`Program.cs` :**
  * Contient le point d'entrée `Main` pour les applications bureau.
  * Utilise `BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);` pour initialiser et exécuter l'application Avalonia pour le bureau.
  * `BuildAvaloniaApp()` configure Avalonia avec la détection de plateforme, les polices, ReactiveUI et la journalisation.

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

* **Accès aux fichiers :** sur le bureau, l'accès aux fichiers est généralement plus simple. Le `MainViewModel` utilise `TopLevel.StorageProvider.OpenFilePickerAsync` qui fonctionne sur Windows, macOS et Linux sans services helper spécifiques comme ceux pour les complexités d'URI/permissions Android ou iOS.

* **Fichier de projet (`SimplePlayerMVVM.Desktop.csproj`) :**
  * Cible des frameworks bureau spécifiques (par exemple `net8.0-windows`, `net8.0-macos14.0`, `net8.0` pour Linux).
  * Inclut `Avalonia.Desktop`.
  * Inclut les bibliothèques natives VisioForge spécifiques à la plateforme pour Windows (x64) et macOS via des conditions `PackageReference`.

        ```xml
        <ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
          <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="..." />
          <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="..." />
        </ItemGroup>
        <ItemGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
          <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="..." />
        </ItemGroup>
        ```

* **Manifest Windows (`app.manifest`) :** utilisé sur Windows pour les paramètres d'application, comme la compatibilité avec des versions spécifiques de Windows.
* **Info macOS (`Info.plist` dans le projet Desktop) :** fournit les informations de bundle pour les applications macOS.

Ces projets et configurations spécifiques aux plateformes garantissent que la logique principale partagée dans `SimplePlayerMVVM` peut interagir correctement avec les fonctionnalités natives et les exigences de chaque système d'exploitation.

## 7. Composants clés du SDK VisioForge utilisés

Cette application exploite plusieurs composants clés du VisioForge Media Player SDK X :

* **`VisioForge.Core.MediaPlayerX.MediaPlayerCoreX` :**
  * Le moteur central pour la lecture multimédia. Il gère l'ouverture des sources multimédias, le contrôle de la lecture (lire, pause, arrêter, positionner, vitesse), la gestion de l'audio et de la vidéo, et fournit des informations d'état (position, durée).
  * Initialisé dans `MainViewModel` et lié à `VideoViewIntf`.
* **`VisioForge.Core.UI.Avalonia.VideoView` :**
  * Un contrôle Avalonia qui sert de surface de rendu pour la vidéo. Il est déclaré dans `MainView.axaml` et sa référence (`IVideoView`) est passée à `MediaPlayerCoreX`.
* **`VisioForge.Core.Types.X.Sources.UniversalSourceSettings` :**
  * Utilisé pour configurer la source multimédia. `UniversalSourceSettings.CreateAsync(filename)` détermine automatiquement la meilleure façon d'ouvrir un fichier ou une URL donnée.
* **Classe statique `VisioForge.Core.VisioForgeX` :**
  * **`InitSDK()` :** initialise le SDK VisioForge. Cela doit être appelé une fois au démarrage de l'application (fait dans le constructeur `MainViewModel` dans cet exemple, mais peut aussi être fait dans `App.axaml.cs`).
  * **`DestroySDK()` :** libère toutes les ressources du SDK. Cela doit être appelé à la fermeture de l'application pour empêcher les fuites de ressources (fait dans `MainViewModel.OnWindowClosing`).
* **Bibliothèques spécifiques aux plateformes :**
  * Comme détaillé dans la configuration du projet et les sections spécifiques aux plateformes, divers paquets NuGet comme `VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Core.macOS`, `VisioForge.CrossPlatform.Core.Android` et `VisioForge.CrossPlatform.Core.iOS` fournissent les binaires natifs et les liaisons nécessaires pour chaque plateforme.
* **`VisioForge.Core.UI.Android.FileDialogHelper` (pour Android) :**
  * Contient des méthodes helper comme `GetFilePathFromUri` pour travailler avec le système de fichiers Android et les URI de contenu.

Comprendre ces composants est crucial pour travailler avec le SDK VisioForge et étendre les fonctionnalités du lecteur.

## 8. Compilation et exécution de l'application

1. **Cloner/télécharger le code source :** obtenez le projet d'exemple `SimplePlayerMVVM`.
2. **Restaurer les paquets NuGet :** ouvrez la solution dans votre IDE et assurez-vous que tous les paquets NuGet sont restaurés pour tous les projets.
3. **Sélectionner le projet de démarrage et la cible :**
    * **Bureau :** définissez `SimplePlayerMVVM.Desktop` comme projet de démarrage. Vous pouvez ensuite l'exécuter directement sur Windows, macOS ou Linux (assurez-vous d'avoir le runtime .NET pour votre OS).
    * **Android :** définissez `SimplePlayerMVVM.Android` comme projet de démarrage. Sélectionnez un émulateur Android ou un appareil connecté. Compilez et déployez.
        * Assurez-vous que le SDK Android et les émulateurs/appareils sont correctement configurés dans votre IDE.
        * Vous devrez peut-être accepter les invites de permission sur l'appareil/émulateur lors du premier lancement.
    * **iOS :** définissez `SimplePlayerMVVM.iOS` comme projet de démarrage. Sélectionnez un simulateur iOS ou un appareil connecté (nécessite une machine de build macOS et un approvisionnement Apple Developer approprié).
        * Assurez-vous que Xcode et les outils développeur sont correctement configurés.
        * Vous devrez peut-être faire confiance au certificat développeur sur l'appareil.
4. **Compiler et exécuter :** compilez le projet de démarrage sélectionné et exécutez-le.

## 9. Conclusion

Ce guide a démontré comment construire un lecteur multimédia multiplateforme en utilisant Avalonia UI avec le patron MVVM et le VisioForge Media Player SDK X. En tirant parti d'un projet principal partagé pour les ViewModels et les Views, et en gérant les spécificités des plateformes dans des projets principaux dédiés, nous pouvons créer une application maintenable qui s'exécute sur une large gamme d'appareils.

Points clés à retenir :

* Le patron MVVM aide à séparer les préoccupations et améliore la testabilité.
* ReactiveUI simplifie l'implémentation MVVM avec Avalonia.
* VisioForge Media Player SDK X fournit de puissantes capacités de lecture multimédia, avec `MediaPlayerCoreX` comme moteur principal et `VideoView` pour l'intégration UI Avalonia.
* Les considérations spécifiques aux plateformes, en particulier pour l'accès aux fichiers et les permissions, sont gérées via des interfaces et des implémentations spécifiques aux plateformes.
* L'initialisation correcte (`VisioForgeX.InitSDK()`) et le nettoyage (`VisioForgeX.DestroySDK()`) du SDK VisioForge sont essentiels.

Vous pouvez étendre cet exemple en ajoutant plus de fonctionnalités comme la prise en charge des listes de lecture, le streaming réseau, les effets vidéo, ou des contrôles UI plus avancés.
## Foire aux questions

### Quelles plateformes le lecteur vidéo Avalonia prend-il en charge ?

Le lecteur Avalonia s'exécute sur Windows, macOS, Linux, Android et iOS à partir d'une seule base de code partagée. Chaque plateforme utilise un projet principal dédié (`SimplePlayerMVVM.Desktop`, `.Android`, `.iOS`) avec des paquets NuGet spécifiques à la plateforme pour le runtime GStreamer natif. L'API principale de lecture (`MediaPlayerCoreX`) et le contrôle `VideoView` sont identiques sur toutes les cibles.

### Le patron MVVM est-il requis pour utiliser le SDK VisioForge avec Avalonia ?

Non. Le SDK fonctionne avec n'importe quelle architecture, y compris le code-behind. Ce guide utilise ReactiveUI pour MVVM parce qu'il s'associe bien au système de liaison d'Avalonia et améliore la testabilité. Les classes principales — `MediaPlayerCoreX` pour la lecture et `VideoView` pour le rendu — n'ont aucune dépendance à ReactiveUI ou à tout framework MVVM. Vous pouvez aussi utiliser CommunityToolkit.Mvvm ou Prism à la place.

### Comment déployer le lecteur vidéo Avalonia sur Linux ?

Publiez avec `dotnet publish -r linux-x64` (ou `linux-arm64` pour ARM). Sur la machine cible, installez le runtime GStreamer (`libgstreamer1.0-0` et les paquets de plugins sur Ubuntu/Debian, ou l'équivalent sur votre distribution). Les paquets NuGet VisioForge incluent les liaisons natives, mais GStreamer lui-même doit être présent sur le système. Aucune configuration spécifique X11 ou Wayland n'est nécessaire — Avalonia gère automatiquement la sélection du serveur d'affichage.

### Le lecteur Avalonia prend-il en charge le rendu vidéo accéléré par GPU ?

Oui. `VideoView` utilise le rendu GPU natif à la plateforme : Direct3D sur Windows, OpenGL ou Vulkan sur Linux, et Metal sur macOS et iOS. Le moteur GStreamer prend également en charge le décodage vidéo accéléré matériellement via VA-API sur Linux, DXVA/D3D11 sur Windows et VideoToolbox sur macOS/iOS. L'accélération matérielle est activée par défaut lorsqu'un GPU compatible est disponible.

### Comment gérer l'accès aux fichiers et les permissions de stockage sur Android et iOS ?

Implémentez une interface `IFilePickerService` spécifique à la plateforme comme indiqué dans la section 6 de ce guide. Sur Android, utilisez `Intent` avec `ActionOpenDocument` et déclarez la permission `READ_EXTERNAL_STORAGE` dans `AndroidManifest.xml`. Sur iOS, utilisez `UIDocumentPickerViewController` pour présenter le sélecteur de fichiers système. Sur le bureau, utilisez le `StorageProvider.OpenFilePickerAsync()` intégré d'Avalonia. Le SDK accepte tout chemin de fichier ou flux lisible quel que soit le mode de sélection du fichier.

## Voir aussi

- [Construire un lecteur vidéo en C#](video-player-csharp.md) — lecteur WinForms et WPF avec configuration plus simple en un seul projet
- [Lecteur .NET MAUI](maui-player.md) — mobile + bureau (iOS, Android, macOS, Windows) à partir d'une seule base de code C#
- [Construire un lecteur vidéo en VB.NET](video-player-vb-net.md) — lecteur multimédia VB.NET pour WinForms
- [Mode boucle et plage de position](loop-and-position-range.md) — configurez la lecture en boucle, la répétition A-B et la lecture par segments
- [Exemples de code](../code-samples/index.md) — extraction d'image, listes de lecture et exemples de lecture additionnels
- [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) — page produit et téléchargements
