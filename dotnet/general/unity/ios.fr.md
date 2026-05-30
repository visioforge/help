---
title: Compiler une app Unity de lecture vidéo pour iOS arm64
description: Réglages de build, flux Xcode, permissions Info.plist et dépannage pour le VisioForge Media Blocks SDK .NET dans Unity 6 sur iOS Standalone.
sidebar_label: Compilation pour iOS
order: 56
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - iOS
  - IL2CPP
  - RTSP
  - Xcode
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Compilation pour iOS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

La déclinaison iOS est livrée sous forme d'un unique `GStreamerX.framework` (~68 Mo sur disque, appareil
arm64 seulement) avec chaque plugin GStreamer enregistré statiquement dans le framework, plus
les assemblies managés du paquet compilés contre `netstandard2.1`. Il n'y a pas de fichiers de
plugin séparés ni de scan de plugins à l'exécution — dyld charge le framework via `@rpath`
quand le premier `[DllImport]` se déclenche. Cette page couvre le flux d'export Xcode, les
entrées Info.plist et les pièges propres à iOS ; pour la séquence de démarrage consultez
[Démarrage et cycle de vie](bootstrap.md).

La déclinaison iOS est incluse dans le `.unitypackage` quand le pipeline de build est lancé avec
`-IncludeMacOS` et `-IncludeIOS`. Le développement iOS se fait sur un hôte Mac, donc
la déclinaison macOS est requise aux côtés d'iOS — sans elle, l'Éditeur sur le Mac n'aurait pas de
runtime natif à charger à l'ouverture du paquet. Le résultat est un paquet cumulatif qui
contient Windows, Android, macOS et iOS ensemble.

## Player Settings

| Réglage | Valeur | Où |
|---|---|---|
| Target Platform | **iOS** | File → Build Profiles → iOS |
| Target SDK | **Device SDK** | File → Build Profiles → iOS |
| Target minimum iOS Version | **15.0** ou plus récent | Project Settings → Player → Other Settings → Configuration |
| Architecture | **ARM64** | Project Settings → Player → Other Settings → Configuration |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **IL2CPP** *(obligatoire — Mono n'est pas pris en charge sur iOS par Unity)* | Project Settings → Player → Other Settings → Configuration |
| Target Device | **iPhone + iPad** | Project Settings → Player → Other Settings → Configuration |
| Enable Bitcode | **Off** *(retiré de Xcode 14+)* | Project Settings → Player → Other Settings → Configuration |

L'Éditeur applique automatiquement `Api Compatibility Level = .NET Standard 2.1` pour iOS
quand la boîte de dialogue de configuration unique du paquet s'exécute. Si vous avez sauté
ce dialogue, réglez-le manuellement.

## Entrées Info.plist obligatoires

Celles-ci sont ajoutées au `Info.plist` du projet Xcode généré. Éditez-les via l'UI **Player
Settings** d'Unity ou via un script de post-traitement dans vos scripts d'Éditeur :

| Clé | Valeur | Nécessaire pour |
|---|---|---|
| `NSCameraUsageDescription` | Raison pour laquelle vous avez besoin de l'accès caméra | Sources de capture caméra |
| `NSMicrophoneUsageDescription` | Raison pour laquelle vous avez besoin de l'accès micro | Sources de capture micro |
| `NSLocalNetworkUsageDescription` | Raison pour laquelle vous avez besoin de l'accès LAN | Streams RTSP sur le réseau local (`192.168.*`, découverte mDNS) |
| `NSAppTransportSecurity` → `NSAllowsArbitraryLoads` = `YES` | (optionnel) | URLs `http://` plates et certificats `https://` / `rtsps://` auto-signés |

Sans `NSLocalNetworkUsageDescription`, iOS 14+ bloque silencieusement la première tentative
de connexion à toute adresse réseau local — `RTSPSourceBlock` fait alors apparaître un
timeout de connexion qui ressemble à une erreur côté caméra. Réglez la chaîne sur quelque
chose que les relecteurs App Store accepteront (par exemple, "Cette application diffuse des
vidéos à partir de caméras IP locales sur votre réseau.").

Si vous streamez uniquement des URLs publiques `https://` / `rtsps://` depuis des hôtes
Internet avec des certificats CA valides, vous pouvez ignorer entièrement l'exception ATS —
App Transport Security les accepte par défaut.

## Organisation sur disque

L'étape `deploy-unity-natives.ps1 -Platform iOS` met en place le runtime iOS comme suit :

| Chemin | Contenu |
|---|---|
| `Assets/Plugins/iOS/GStreamerX.framework/GStreamerX` | Le binaire Mach-O arm64 — chaque plugin GStreamer enregistré statiquement. |
| `Assets/Plugins/iOS/GStreamerX.framework/Info.plist` | Métadonnées du framework. |
| `Assets/Plugins/iOS/GStreamerX.framework/Modules/module.modulemap` | Carte de modules Swift / Objective-C. |
| `Assets/Plugins/iOS/libVisioForge_Core.a` | L'archive statique de la déclinaison iOS du SDK. |
| `Assets/VisioForge/link.xml` | Règles de préservation IL2CPP (partagées entre déclinaisons mobiles). |
| `Assets/Plugins/iOS/Managed/` | Assemblies managés compilés contre `netstandard2.1` avec `UNITY_NS21_IOS` défini. |

Les métadonnées `PluginImporter` sur le dossier du framework le marquent
**Add To Embedded Binaries = YES**, donc Unity embarque le framework dans le projet Xcode
généré automatiquement. Dyld résout `@rpath/GStreamerX.framework/GStreamerX` quand le
premier `[DllImport]` du SDK se déclenche — aucune configuration de chemin de recherche
n'est requise.

Le bundle CA n'est **pas** un fichier séparé sur iOS — c'est une ressource managée embarquée
dans `VisioForge.Core.dll` (`VisioForge.Core.ResourcesData.ca-certificates.crt`).
`VisioForgeEnvironment.Configure()` l'extrait vers
`Application.persistentDataPath/ssl/certs/` au démarrage et pointe `SSL_CERT_FILE` là.

## Flux Xcode

1. **File → Build Profiles → iOS → Build** — Unity produit un projet Xcode, pas un `.ipa`
   final.
2. Ouvrez le `.xcworkspace` (ou `.xcodeproj`) généré dans Xcode sur le même Mac.
3. Onglet **Signing & Capabilities** sur la cible Unity-iPhone — réglez votre équipe Apple
   Developer et un bundle identifier que vous possédez.
4. Connectez l'iPhone (ou stub simulateur — voir la note Simulator plus bas), choisissez-le
   comme cible Run et appuyez sur **▶ Run**.

Le premier build prend quelques minutes — Xcode compile le C++ généré par IL2CPP en arm64
appareil. Les builds incrémentaux sont en quelques secondes.

!!! note "Simulator non pris en charge"
    `GStreamerX.framework` est livré appareil-arm64 seulement. Le Simulator iOS (x86_64 sur
    Macs Intel, arm64-sim sur Apple silicon) ne peut pas le charger — Xcode abandonne le
    build avec `Could not find module 'GStreamerX' for target
    'arm64-apple-ios-simulator'`. Testez sur un vrai iPhone ou iPad. Si vous avez un besoin
    Simulator strict, contactez le support.

## Taille de build

La déclinaison iOS ajoute environ **40 Mo** au `.ipa` final :

- `GStreamerX.framework/GStreamerX` — ~38 Mo binaire appareil-arm64 après thinning à l'édition de liens (depuis le framework de ~68 Mo sur disque)
- `libVisioForge_Core.a` — lié dans le binaire IL2CPP, delta ~2 Mo
- Assemblies managés — ~3 Mo

Le `.ipa` compressé est généralement plus petit après le thinning App Store.

## Dépannage

| Symptôme | Cause | Solution |
|---|---|---|
| Xcode abandonne : `dyld: Library not loaded: @rpath/GStreamerX.framework/GStreamerX` | `Add To Embedded Binaries` n'a pas été appliqué au slot PluginImporter du framework. | Confirmez que le `.meta` de `Assets/Plugins/iOS/GStreamerX.framework` a `AddToEmbeddedBinaries: 1`. Si vous l'avez remplacé manuellement, réimportez le paquet. |
| `UnityException: get_dataPath can only be called from the main thread` depuis un callback GStreamer | Le premier lecteur de `NativesPath` s'est exécuté sur un thread d'arrière-plan avant le préchargement main-thread de `Configure()`. | Confirmez que `Configure()` est complétée — elle imprime `[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).` dans la Console. Si absent, le bootstrap a échoué avant le préchargement. |
| `MissingMethodException` depuis `GLib.SignalArgs` ou `SignalClosure.MarshalCallback` | `link.xml` a été supprimé ou le stripping managé IL2CPP est actif sans lui. | Confirmez que `Assets/VisioForge/link.xml` existe. Réimportez le paquet s'il manque. |
| Le stream RTSP expire avant de connecter sur iOS 14+ | `NSLocalNetworkUsageDescription` manque — iOS bloque la première connexion LAN. | Ajoutez la clé au `Info.plist` avec une raison visible par l'utilisateur. |
| RTSPS / HTTPS échoue avec erreur TLS à la première requête | L'extraction du bundle CA a échoué silencieusement. | Vérifiez la Console pour `[VisioForge] CA cert extraction failed`. La ressource embarquée est livrée dans `VisioForge.Core.dll` — confirmez que le DLL n'a pas été supprimé. |
| App rejetée de la revue App Store pour "raison de confidentialité manquante" | Une source de capture nécessite `NSCameraUsageDescription` ou `NSMicrophoneUsageDescription`. | Ajoutez les clés correspondantes avec des raisons visibles par l'utilisateur. |
| Plantage au second `Play` dans Xcode | `VisioForgeX.DestroySDK()` a été appelé dans `OnDestroy`. | Ne l'appelez pas — voir [Démarrage et cycle de vie](bootstrap.md#cycle-de-vie-editeur). |

## Foire aux questions

### Puis-je utiliser Mono sur iOS ?

Non. Unity lui-même ne prend pas en charge Mono sur iOS — IL2CPP est le seul backend pour
les builds Standalone iOS. Le SDK correspond à cette contrainte.

### La déclinaison iOS fonctionne-t-elle dans le Simulator iOS ?

Non. `GStreamerX.framework` est appareil-arm64 seulement — voir la note plus haut. Testez sur
du matériel réel.

### Pourquoi le build Xcode est-il l'étape lente ?

IL2CPP transpile chaque assembly managé (vos scripts + moteur Unity + le SDK) en C++, puis
Xcode compile ce C++ pour appareil arm64. Le premier build à froid est de ~3 — 5 minutes ;
les builds incrémentaux sont en quelques secondes parce que Xcode met presque tout en cache.

### Le SDK envoie-t-il des données vers les serveurs VisioForge ?

Non. Le SDK tourne entièrement en processus — pas de télémétrie, pas d'appel-maison de
licence, pas d'analyse d'usage. L'exigence `NSLocalNetworkUsageDescription` concerne
purement les connexions RTSP / HTTP sortantes de votre app, qu'iOS traite comme visibles
par l'utilisateur.

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — configuration du paquet
- [Démarrage et cycle de vie](bootstrap.md) — comment `Configure()` démarre le runtime iOS
- [Compilation pour macOS](macos.md) — l'hôte Mac correspondant dont vous avez besoin pour
  compiler iOS
- [Voir une caméra RTSP dans Unity](rtsp-viewer.md) — l'exemple `RTSPViewer`
- [Dépannage](troubleshooting.md) — référence des erreurs inter-plateformes
