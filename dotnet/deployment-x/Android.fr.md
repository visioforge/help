---
title: Déployer le SDK VisioForge .NET sur Android — guide complet
description: Déployez des applications VisioForge .NET sur Android — paquets NuGet, intégration VideoView, ABI ARM64 et x86, permissions et dépannage du déploiement.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Android
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - VideoPlayerActivity

---

# Guide d'implémentation et de déploiement Android

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction aux SDK VisioForge pour Android

Les développeurs Android travaillant avec les technologies .NET peuvent tirer parti des puissantes capacités des SDK VisioForge pour intégrer des fonctionnalités multimédias avancées à leurs applications. Les SDK proposent des solutions robustes pour la manipulation, la lecture, la capture et l'édition de médias sur la plateforme Android en utilisant les technologies .NET.

Le SDK VisioForge pour Android offre de puissantes capacités de traitement vidéo, de capture, d'édition et de lecture, le tout optimisé pour la plateforme Android tout en conservant une expérience de développement multiplateforme cohérente.

Le processus de déploiement Android requiert une attention particulière en matière de gestion des paquets, de compatibilité des appareils, de permissions et d'optimisation des performances. Ce document fournit des instructions détaillées pour garantir que votre application s'exécute correctement sur les appareils Android.

## Configuration requise

Avant de commencer le processus d'implémentation et de déploiement Android, assurez-vous que votre environnement de développement satisfait aux exigences suivantes :

### Exigences relatives aux appareils

- Appareil Android sous Android 10.0 ou ultérieur
- Architecture de processeur ARM ou ARM64
- Espace de stockage suffisant pour les ressources de l'application et le traitement multimédia
- Matériel caméra et microphone (si vous utilisez les fonctionnalités de capture vidéo/audio)

### Exigences de l'environnement de développement

- Ordinateur Windows, Linux ou macOS
- Visual Studio avec les charges de travail .NET MAUI ou Xamarin installées, JetBrains Rider ou Visual Studio Code
- SDK .NET 8.0 ou ultérieur (dernière version stable recommandée)
- SDK Android avec les niveaux d'API appropriés installés
- Java Development Kit (JDK) 11 ou ultérieur
- Connaissances de base du développement .NET pour Android

## Prise en charge des architectures

Le SDK VisioForge pour Android prend nativement en charge les architectures courantes des appareils Android :

### Prise en charge ARM64

- Optimisé pour les appareils Android modernes
- Traitement vidéo accéléré matériellement
- Performances accrues pour les opérations multimédias
- Cible principale pour la plupart des applications

### Prise en charge ARM/ARMv7

- Compatibilité avec les appareils Android plus anciens
- Solutions logicielles de repli pour l'accélération matérielle au besoin
- Approche équilibrée entre performances et compatibilité

## Processus d'installation et de configuration

Suivez ces étapes pour configurer et déployer correctement votre application Android propulsée par VisioForge :

1. Créez un nouveau projet Android dans l'IDE de votre choix (Visual Studio ou JetBrains Rider recommandés).
2. Ajoutez les paquets NuGet requis à votre projet (détaillés dans la section suivante).
3. Configurez les permissions nécessaires dans votre fichier AndroidManifest.xml.
4. Implémentez la logique de votre application en utilisant les composants du SDK VisioForge.
5. Compilez, signez et déployez votre application sur des appareils de test.

### Gestion des paquets NuGet

Le SDK VisioForge pour Android est distribué via des paquets NuGet. Ajoutez les paquets suivants à votre projet Android :

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) — paquet SDK managé principal (classes principales, VideoCaptureCoreX / MediaPlayerCoreX / MediaBlocksPipeline).
- [VisioForge.CrossPlatform.Core.Android](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Android) — contient les composants de redistribution (bibliothèques natives) requis pour les applications Android.

Vous pouvez ajouter ces paquets via le gestionnaire de paquets NuGet de votre IDE ou en ajoutant ce qui suit à votre fichier projet (utilisez les dernières versions) :

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.*" />
</ItemGroup>
```

Note : remplacez les numéros de version par les dernières versions disponibles sur NuGet.org.

## Intégration de la bibliothèque de bindings Java

Les applications Android utilisant le SDK VisioForge requièrent une bibliothèque de bindings Java personnalisée pour fonctionner correctement. Cette étape essentielle garantit une communication correcte entre le framework .NET et l'environnement Java d'Android.

Suivez ces étapes détaillées pour l'intégrer :

1. Clonez le dépôt de la bibliothèque de bindings depuis [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency).
2. Choisissez le projet de bindings qui correspond à la cible .NET pour laquelle vous compilez — le dossier contient un fichier `VisioForge.Core.Android.X{N}.csproj` par version .NET prise en charge (par exemple, `VisioForge.Core.Android.X9.csproj` pour .NET 9, `VisioForge.Core.Android.X10.csproj` pour .NET 10). Si votre cible .NET n'est pas listée, choisissez la version prise en charge la plus proche.
3. Ajoutez une référence au projet de bindings depuis le fichier .csproj de votre application :

```xml
<ItemGroup>
  <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X9.csproj" />
</ItemGroup>
```

> **Note :** assurez-vous d'ajuster le chemin relatif pour qu'il corresponde à la structure de votre projet

## Implémentation de VideoView dans votre application

### Ajout de VideoView à votre disposition

Le contrôle `VideoView` est l'interface principale pour afficher du contenu vidéo dans votre application Android. Pour l'intégrer à votre application, suivez ces étapes :

1. Ouvrez le fichier de disposition de votre Activity ou Fragment (généralement un fichier `.axml` ou `.xml`)
2. Ajoutez l'élément VideoView comme illustré dans l'exemple ci-dessous :

```xml
<VisioForge.Core.UI.Android.VideoView
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:minWidth="25px"
    android:minHeight="25px"
    android:id="@+id/videoView" />
```

### Initialisation de VideoView dans le code

Après avoir ajouté le VideoView à votre disposition, vous devez l'initialiser dans le code de votre Activity ou Fragment :

```csharp
using VisioForge.Core.UI.Android;

namespace YourApp
{
    [Activity(Label = "VideoPlayerActivity")]
    public class VideoPlayerActivity : Activity
    {
        private VideoView _videoView;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.your_layout);
            
            // Initialiser la vue vidéo
            _videoView = FindViewById<VideoView>(Resource.Id.videoView);
        }
    }
}
```

## Considérations de performance

Utilisez autant que possible des appareils Android physiques pour les tests. Les simulateurs peuvent ne pas refléter fidèlement les performances en conditions réelles, en particulier pour les opérations vidéo accélérées matériellement.

## Signature et publication de l'application

### Signature de l'application

Pour distribuer votre application Android, vous devez la signer avec un certificat numérique :

1. Créez un fichier keystore si vous n'en avez pas déjà un :

```bash
keytool -genkey -v -keystore your-app-key.keystore -alias your-app-alias -keyalg RSA -keysize 2048 -validity 10000
```

2. Configurez la signature dans votre projet :

Ajoutez ce qui suit à votre fichier `android/app/build.gradle` :

```text
android {
    ...
    
    signingConfigs {
        release {
            storeFile file("your-app-key.keystore")
            storePassword "your-store-password"
            keyAlias "your-app-alias"
            keyPassword "your-key-password"
        }
    }
    
    buildTypes {
        release {
            signingConfig signingConfigs.release
            ...
        }
    }
}
```

Pour les projets .NET MAUI ou Xamarin.Android, configurez la signature dans votre fichier .csproj :

```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
    <AndroidKeyStore>True</AndroidKeyStore>
    <AndroidSigningKeyStore>your-app-key.keystore</AndroidSigningKeyStore>
    <AndroidSigningStorePass>your-store-password</AndroidSigningStorePass>
    <AndroidSigningKeyAlias>your-app-alias</AndroidSigningKeyAlias>
    <AndroidSigningKeyPass>your-key-password</AndroidSigningKeyPass>
</PropertyGroup>
```

### Publication sur le Google Play Store

1. Générez un AAB (Android App Bundle) pour la distribution :

```bash
dotnet build -f net8.0-android -c Release /p:AndroidPackageFormat=aab
```

2. Créez un compte développeur sur la Google Play Console si vous n'en avez pas déjà un.

3. Créez une nouvelle application sur la Google Play Console.

4. Téléversez votre fichier AAB sur la voie de production.

5. Complétez les informations de la fiche du Store.

6. Soumettez pour examen.

## Dépannage

### Problèmes courants

1. **Permissions manquantes** : assurez-vous que toutes les permissions requises sont déclarées dans AndroidManifest.xml et demandées à l'exécution.
2. **Compatibilité d'architecture** : vérifiez que votre application prend en charge l'architecture de l'appareil cible (ARM/ARM64).
3. **Contraintes de mémoire** : surveillez l'utilisation de la mémoire et implémentez une gestion appropriée des ressources.
4. **Problèmes de performance** : utilisez l'accélération matérielle et optimisez les opérations multimédias pour les appareils mobiles.
5. **Erreurs de bindings Java** : en cas de problèmes avec les bindings Java :
   - Confirmez que vous utilisez la bonne version de la bibliothèque de bindings
   - Vérifiez les incompatibilités de version entre .NET et la bibliothèque de bindings
   - Vérifiez que toutes les dépendances sont correctement référencées

### Obtenir de l'aide

Si vous rencontrez des problèmes lors du déploiement du SDK VisioForge sur Android, veuillez consulter :

- [Portail de support](https://support.visioforge.com)
- [Exemples GitHub](https://github.com/visioforge/.Net-SDK-s-samples)

## Conclusion

L'implémentation et le déploiement d'applications SDK VisioForge sur des appareils Android nécessitent une attention particulière aux considérations spécifiques à la plateforme. En suivant les recommandations de ce document, vous pouvez assurer un processus de développement et de déploiement fluide et livrer des applications vidéo de haute qualité à vos utilisateurs Android.

N'oubliez pas de tester en profondeur sur les appareils cibles, en particulier pour les opérations gourmandes en performances comme la capture et le traitement vidéo. Avec une implémentation appropriée, le SDK VisioForge permet de créer de puissantes applications multimédias sur tout l'écosystème Android.
