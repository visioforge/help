---
title: Montage vidéo dans .NET MAUI — Android, iOS, Windows
description: Assemblez et rendez de la vidéo sur Android, iOS, macCatalyst et Windows depuis un seul code .NET MAUI avec Video Edit SDK et le moteur VideoEditCoreX.
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

# Montage vidéo dans des applications .NET MAUI

## Introduction

`VideoEditCoreX` est le moteur de timeline multiplateforme du Video Edit SDK .NET. Il s'appuie
sur le backend GStreamer, si bien que le même code de montage compile et s'exécute sur Android,
iOS, macCatalyst et Windows depuis un seul projet .NET MAUI — la surface de prévisualisation est
le contrôle `VideoView` partagé, et l'API de timeline est identique sur chaque cible.

Ce guide construit le plus petit éditeur utile possible : sélectionner des clips dans la galerie
de l'appareil, les ajouter à la timeline, prévisualiser le résultat, puis le rendre dans un
fichier MP4.

L'exemple complet se trouve sur [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit).

## Prérequis

- Les charges de travail .NET MAUI pour les plateformes ciblées (`maui-android`, `maui-ios`, `maui-maccatalyst`).
- Une licence VisioForge, ou l'essai de 30 jours.
- Sur iOS et macCatalyst, une chaîne `NSPhotoLibraryUsageDescription` dans `Info.plist` — le
  sélecteur de galerie est ce qui alimente la timeline.

## Paquets NuGet

Deux paquets : le SDK lui-même et le contrôle `VideoView` de MAUI.

```xml
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.9.17" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.9.17" />
```

`VisioForge.DotNet.Core.UI.MAUI` ne fournit que le contrôle `VideoView` — à lui seul, il ne peut
pas compiler `VideoEditCoreX`. C'est le paquet `VisioForge.DotNet.VideoEdit` qui apporte le
moteur.

Ajoutez ensuite le redistribuable pour chaque plateforme que vous ciblez. Consultez
[Installer dans des applications MAUI](../../install/maui.md) pour le détail complet par
plateforme, y compris la référence au projet de bindings Java Android, requise pour les
builds Android.

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.7.27" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <!-- La version du redistribuable iOS est volontairement en retard sur celle du SDK - elle suit
       le rythme de reconstruction de GStreamer-iOS, pas celui du wrapper. -->
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

Enregistrez les handlers VisioForge pour que `VideoView` se résolve vers son implémentation
native :

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

## Disposition XAML

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

## Création du moteur

`AddVisioForgeHandlers` enregistre le contrôle. Il ne charge **pas** le stack natif — c'est un
appel distinct, qui doit avoir lieu avant la construction du premier `VideoEditCoreX`. Sans lui,
le constructeur lève une `DllNotFoundException` sur une machine vierge.

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
    // Charge le stack GStreamer natif. Le premier appel construit le registre de plugins
    // et peut prendre plusieurs centaines de millisecondes, utilisez donc la forme
    // asynchrone hors du thread UI.
    await VisioForgeX.InitSDKAsync();

    IVideoView vv = videoView.GetVideoView();
    _core = new VideoEditCoreX(vv);

    _core.OnError += Core_OnError;
    _core.OnProgress += Core_OnProgress;
    _core.OnStop += Core_OnStop;
}
```

!!! warning "Gardez les contrôles inertes tant que le moteur n'existe pas"
    `InitSDKAsync` s'exécute sur le pool de threads et un démarrage à froid consacre des
    centaines de millisecondes à construire le registre GStreamer. `MainPage_Loaded` est
    `async void`, donc la page reste interactive pendant toute cette fenêtre et tout gestionnaire
    touchant `_core` rencontrerait une référence nulle. Désactivez le panneau de contrôles dans
    le constructeur et activez-le une fois le moteur construit, et enveloppez le corps dans un
    `try`/`catch` pour qu'un échec d'initialisation laisse les contrôles éteints avec la raison
    à l'écran. L'exemple fait les deux.

Libérez-le à la sortie :

```csharp
private void MainPage_Unloaded(object sender, EventArgs e)
{
    _core?.Stop();
    _core?.Dispose();
    _core = null;

    VisioForgeX.DestroySDK();
}
```

## Construction de la timeline

Chaque appel `Input_Add*` ajoute un élément à la fin de la timeline. Utilisez
`Input_AddAudioVideoFile` pour un clip normal, `Input_AddAudioFile` pour une piste audio seule et
`Input_AddImageFile` pour une image fixe avec une durée explicite.

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

`Input_Clear_List()` vide la timeline.

!!! warning "Le sélecteur ne fournit pas toujours un chemin de fichier"
    Sur Android, `MediaPicker` copie l'élément dans un dossier de cache, donc `FullPath` est
    absolu et directement utilisable. Sur iOS, `PHPicker` remet à MAUI un `NSItemProvider` et
    seul le nom du fichier d'origine survit dans `FullPath` — le flux doit d'abord être copié
    dans votre propre cache. Le `ResolveLocalPathAsync` de l'exemple couvre les deux cas.

## Prévisualisation et rendu

Le moteur ne possède qu'un seul commutateur entre les deux modes : `Output_Format`. Une valeur
nulle lit la timeline dans le `VideoView` ; un objet de format la rend dans un fichier.

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

!!! warning "`new MP4Output(path)` seul vous donne du H.264 logiciel hors macOS et Android"
    La valeur par défaut de `MP4Output` choisit `AppleMediaH264EncoderSettings` sur macOS et
    appelle `H264EncoderBlock.GetDefaultSettings()` sur Android — mais iOS, Mac Catalyst et
    Windows retombent tous sur `OpenH264EncoderSettings`, un encodeur logiciel.

    `H264EncoderBlock.GetDefaultSettings()` est le sélecteur de plateforme du SDK lui-même et
    privilégie le matériel partout : VideoToolbox sur Apple, MediaCodec sur Android, puis NVENC,
    AMF et QSV avant de se rabattre sur le logiciel. Passez-le explicitement pour garder iOS,
    Mac Catalyst et Windows sur le matériel.

    Il réside dans l'espace de noms `VisioForge.Core.MediaBlocks.VideoEncoders`, mais c'est un
    utilitaire partagé et non un pipeline Media Blocks — `MP4Output` l'appelle lui-même.

    Laissez l'encodeur audio non spécifié : `MP4Output` choisit AAC sur Windows et MP3 ailleurs.

Écrivez dans un répertoire propre à l'application, comme `FileSystem.Current.AppDataDirectory`.
Cela ne nécessite aucune permission au runtime sur aucune plateforme, contrairement au stockage
multimédia partagé d'Android.

## Progression et fin

`OnProgress` rapporte 0–100 et `OnStop` rapporte le succès. Les deux se déclenchent sur un thread
en arrière-plan, il faut donc revenir sur le thread UI avant de toucher aux contrôles :

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

## Notes par plateforme

- **Android** nécessite la référence au projet de bindings Java. Sans elle, l'application compile
  mais le stack natif échoue à s'initialiser au runtime.
- **iOS et macCatalyst** nécessitent `<UseInterpreter>true</UseInterpreter>`. La conversion de
  types XAML de MAUI utilise `DynamicMethod`, qui requiert le JIT et lève une
  `ExecutionEngineException` en mode AOT uniquement.
- Les builds **Windows** résolvent les redistribuables via
  `VisioForge.CrossPlatform.Core.Windows.x64`.

## Exemples d'applications

- **[SimpleEdit (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit)** — l'application construite par ce guide.
- **[Video Join Demo X (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/WPF/CSharp/Video%20Join%20Demo%20X)** — la même API de timeline sur le bureau, avec des options de sortie par format.

## Voir aussi

- [Installer dans des applications MAUI](../../install/maui.md)
- [Aide-mémoire Video Edit SDK](../cheat-sheet.md)
- [Transitions](../transitions.md)
- [Déploiement Android](../../deployment-x/Android.md)
- [Déploiement iOS](../../deployment-x/iOS.md)
