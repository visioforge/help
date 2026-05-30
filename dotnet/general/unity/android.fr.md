---
title: Compiler une app Unity de lecture vidéo pour Android
description: Réglages de build, IL2CPP, permissions AndroidManifest et dépannage pour Media Blocks SDK .NET dans Unity 6 sur Android.
sidebar_label: Compilation pour Android
order: 54
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Android
  - IL2CPP
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Compilation pour Android

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

La déclinaison Android est livrée sous forme d'un `libgstreamer_android.so` monolithique avec chaque
plugin GStreamer lié statiquement, plus les assemblies managés du paquet compilés contre
`netstandard2.1`. Cette page couvre les réglages du Build Profile, les permissions du
manifeste et les pièges propres à Android. Pour la séquence de démarrage partagée entre
plateformes, consultez [Démarrage et cycle de vie](bootstrap.md).

La déclinaison Android est incluse dans le `.unitypackage` quand le pipeline de build est lancé avec
`-IncludeAndroid`. Le résultat est un paquet cumulatif qui contient les runtimes Windows,
Android et les opt-in Apple ensemble — Unity choisit le bon au moment du build à partir des
métadonnées `PluginImporter` par fichier.

## Player Settings

| Réglage | Valeur | Où |
|---|---|---|
| Target Platform | **Android** | File → Build Profiles → Android |
| Texture Compression | **ASTC** *(recommandé ; défaut dans Unity 6)* | File → Build Profiles → Android |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **IL2CPP** *(obligatoire — Mono n'est pas pris en charge sur Android par Unity)* | Project Settings → Player → Other Settings → Configuration |
| Target Architectures | **ARM64** *(ARMv7 non livré — décochez-le)* | Project Settings → Player → Other Settings → Configuration |
| Internet Access | **Require** *(nécessaire pour les sources RTSP / HTTPS)* | Project Settings → Player → Other Settings → Configuration |
| Write Permission | **External (SDCard)** si vous écrivez ou enregistrez des médias sur le stockage externe | Project Settings → Player → Other Settings → Configuration |
| Minimum API Level | **24 (Android 7.0)** ou supérieur | Project Settings → Player → Other Settings → Identification |

Mono ne peut pas charger `libgstreamer_android.so` correctement via le runtime Android d'Unity
— seul IL2CPP est exercé en CI et pris en charge en production.

## Entrées obligatoires dans AndroidManifest

Unity génère `AndroidManifest.xml` pour vous. Les réglages ci-dessus se traduisent dans les
entrées standard ; si vous avez besoin d'un manifeste personnalisé, assurez-vous qu'il
contienne :

```xml
<uses-permission android:name="android.permission.INTERNET" />

<!-- Seulement si votre app utilise la découverte RTSP / diffuse de l'audio out via UDP sur le segment local -->
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<!-- Seulement si votre app utilise le micro ou la caméra comme source -->
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.CAMERA" />

<!-- Seulement si vous lisez des médias depuis le stockage externe -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"
                 android:maxSdkVersion="32" />
<uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
<uses-permission android:name="android.permission.READ_MEDIA_AUDIO" />
```

`READ_MEDIA_VIDEO` / `READ_MEDIA_AUDIO` remplacent le legacy `READ_EXTERNAL_STORAGE` sur
Android 13+ (API 33+) ; déclarez les deux formes pour que les appareils plus anciens
continuent de fonctionner.

## Ce que le paquet ajoute pour Android

L'étape `deploy-unity-natives.ps1` met en place le runtime Android dans votre projet comme
suit :

| Chemin | Contenu |
|---|---|
| `Assets/Plugins/Android/libs/arm64-v8a/libgstreamer_android.so` | Runtime GStreamer monolithique — tous les plugins liés statiquement. |
| `Assets/Plugins/Android/libs/arm64-v8a/libVisioForge_Core.so` | Shim natif de la déclinaison Android du SDK. |
| `Assets/Plugins/Android/visioforge-gstreamer.aar` | L'archive Java qui expose `org.freedesktop.gstreamer.GStreamer.init(Context)`. |
| `Assets/VisioForge/link.xml` | Règles de préservation des types / membres pour IL2CPP. |
| `Assets/Plugins/Android/` | Assemblies managés compilés contre `netstandard2.1` avec `UNITY_NS21_ANDROID` défini. |

Le `link.xml` est obligatoire. Sans lui, le stripping de code managé d'IL2CPP supprime les
types auxquels le SDK accède par réflexion — sous-classes `GLib.SignalArgs`,
`SignalClosure.MarshalCallback` — et le premier appel de délégué lance
`MissingMethodException`. Le paquet fournit un `link.xml` testé ; ne le supprimez pas de
`Assets/`.

## Taille de build

La déclinaison Android ajoute environ **35 Mo** à l'APK / AAB :

- `libgstreamer_android.so` — ~30 Mo (arm64-v8a, dépouillé)
- `libVisioForge_Core.so` — ~2 Mo
- Assemblies managés — ~3 Mo

Si vous livrez aussi ARMv7 (non inclus actuellement par le paquet, mais si vous le stagez
manuellement), prévoyez de doubler les bibliothèques natives.

## Build Standalone

**File → Build Profiles → Android → Build** (ou **Build And Run**) produit un APK / AAB.

Testé sur :

- Unity 6 (`6000.x`) avec le module Android Build Support installé
- Appareils Android 9 à Android 15
- Pixel 6 / 7 / 8 / 9, Galaxy S22 / S23 / S24

## Dépannage

| Symptôme | Cause | Solution |
|---|---|---|
| `DllNotFoundException: libgstreamer_android` au démarrage de la scène | Le Scripting Backend est Mono, pas IL2CPP. | Passez à IL2CPP dans Project Settings → Player → Other Settings. |
| `MissingMethodException` depuis `GLib.SignalArgs` ou `SignalClosure.MarshalCallback` | `link.xml` est absent ou a été supprimé. | Confirmez que `Assets/VisioForge/link.xml` existe. Réimportez le paquet s'il n'y est pas. |
| `InvalidOperationException: Unity Android bootstrap requires com.unity3d.player.UnityPlayer.currentActivity to be available` | Wear OS / Android TV / hôte Unity-as-a-library où le champ est null à `BeforeSceneLoad`. | Différez `VisioForgeEnvironment.Configure()` au premier événement Activity observable et appelez-le manuellement depuis là. |
| L'init Java échoue avec `failed to find getFilesDir` | L'Activity n'est pas une `UnityPlayerActivity` et n'expose pas l'API Context Android standard. | Confirmez que l'Activity hôte hérite d'une `Activity` Android réelle. |
| Les streams RTSPS / HTTPS échouent avec erreur TLS | L'extraction du bundle CA a échoué silencieusement. | Cherchez dans Logcat `[VisioForge] CA cert extraction failed`. Réimportez le paquet si la ressource embarquée manque. |
| L'app plante au second `Awake` après être revenue d'arrière-plan | `VisioForgeX.DestroySDK()` a été appelé dans `OnDestroy`. | Ne l'appelez pas — voir [Démarrage et cycle de vie](bootstrap.md#cycle-de-vie-editeur). |

## Foire aux questions

### Pourquoi ARMv7 n'est-il pas livré ?

Les appareils Android modernes (base API 24 / Android 7.0) sont majoritairement arm64. Livrer
les ~30 Mo du GStreamer monolithique pour les deux ABIs doublerait la taille du paquet pour
une part d'appareils minuscule. Si vous avez un besoin ARMv7 strict, contactez le support.

### Puis-je utiliser le SDK dans un projet Android non-Unity ?

Oui — le SDK sous-jacent est distribué sous forme de paquet NuGet
`VisioForge.CrossPlatform.Core.Android` autonome pour les apps .NET Android pures. Le paquet
Unity emballe ce runtime plus le bootstrap Java et `link.xml` ; le wrapper est spécifique à
Unity.

### Le SDK fonctionne-t-il dans le mode Android Editor (Project Player → Run In Editor) ?

Run-in-Editor pour les targets Android n'est pas exercé ; compilez et déployez sur un
appareil réel. L'Éditeur lui-même exécute la déclinaison **Windows** du SDK sur un hôte Windows —
basculer le Build Target sur Android dans Build Profiles ne change pas quel runtime natif
l'Éditeur charge.

### Quelles sources fonctionnent sur RTSP sous Android ?

Le même `RTSPSourceBlock` utilisé sur Windows. Auto-reconnexion, identifiants optionnels,
streams vidéo seule et vidéo+audio, et les transports RTSP standard (TCP, UDP, multicast
UDP) sont tous pris en charge. La déclinaison Android utilise l'élément GStreamer `rtspsrc` en
interne — la même que les déclinaisons Windows et macOS.

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — configuration du paquet
- [Démarrage et cycle de vie](bootstrap.md) — le bootstrap Java Android expliqué
- [Voir une caméra RTSP dans Unity](rtsp-viewer.md) — l'exemple `RTSPViewer`
- [Dépannage](troubleshooting.md) — référence des erreurs inter-plateformes
- [Matrice des plateformes](platform-matrix.md) — prise en charge des fonctionnalités par
  plateforme Unity
