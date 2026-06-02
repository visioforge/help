---
title: Dépanner le Media Blocks SDK dans Unity — Erreurs courantes
description: Erreurs courantes et solutions pour Media Blocks SDK .NET dans Unity 6 — bootstrap, natifs manquants, IL2CPP, TLS et cycle de vie.
sidebar_label: Dépannage
order: 60
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Troubleshooting
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Dépannage

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Cette page rassemble les symptômes que vous avez le plus de chances de rencontrer et la cause
racine de chacun. Les erreurs sont groupées par catégorie. Les pages par plateforme
([Windows](windows.md), [Android](android.md), [macOS](macos.md), [iOS](ios.md)) ont aussi
une table de dépannage spécifique à la cible — consultez les deux.

## Bootstrap et initialisation

### `[VisioForge] Native runtime not found at <path>`

`VisioForgeEnvironment.Configure()` n'a pas pu trouver le dossier des natifs fourni sur le
disque. Causes :

- L'import `.unitypackage` était partiel. Réimportez avec tous les éléments cochés.
- Sur Standalone macOS, le Build Target n'a pas inclus la déclinaison macOS — le paquet a été
  construit sans `-IncludeMacOS`. Reconstruisez le paquet ou importez la variante
  cumulative.
- Sur Android, l'étape de mise en place de la déclinaison par-Build Target ne s'est pas exécutée.
  Ouvrez `Assets/Plugins/Android/libs/arm64-v8a/` dans la fenêtre Project et confirmez que
  `libgstreamer_android.so` est présent.

### `[VisioForge] InitializeSdk() called before Configure() succeeded`

Une branche plateforme de `Configure()` a échoué et a laissé `s_envConfigured = false`. La
ligne précédente de la Console (`[VisioForge] Android GStreamer init failed: …`,
`[VisioForge] SetDllDirectoryW failed (Win32 error …)`, etc.) explique pourquoi. Corrigez le
problème sous-jacent et laissez `Configure()` retenter au chargement de la scène suivante.

### `UnityException: get_dataPath can only be called from the main thread`

Un thread d'arrière-plan dans le SDK ou votre script a lu `Application.dataPath` (ou
`Application.streamingAssetsPath`, ou `Application.platform`) sans passer par le chemin mis
en cache. La solution :

- Sur iOS, `Configure()` précharge `s_cachedNativesPath` sur le thread principal — confirmez
  que la Console affiche `[VisioForge] iOS environment configured (GStreamerX.framework via
  @rpath).`. Si absent, le bootstrap s'est interrompu avant le préchargement et le prochain
  lecteur emprunte le chemin d'évaluation différée hors du thread principal.
- Dans votre propre code, n'appelez pas l'API Unity depuis un `Task.Run`, un callback
  pad-added GStreamer ou un handler de signal du bus. Marshallez l'appel vers le thread
  principal avec `UnitySynchronizationContext` ou en posant un drapeau que la méthode
  `Update()` vérifie.

### `InvalidOperationException: Unity Android bootstrap requires com.unity3d.player.UnityPlayer.currentActivity to be available`

Le bootstrap Java Android n'a pas pu obtenir un `currentActivity` non-null à
`BeforeSceneLoad`. Arrive sur Wear OS, variantes Android TV sans `UnityPlayerActivity`, et
hôtes Unity-as-a-library qui n'ont pas encore assigné le champ. Différez `Configure()` à
votre premier événement Activity observable :

```csharp
private void Start()
{
    if (!VisioForgeIsConfigured())
        VisioForgeEnvironment.Configure();   // réexécute après Activity prête
    VisioForgeEnvironment.InitializeSdk();
}
```

`Configure()` est idempotente — un appel redondant après un appel réussi est inoffensif.

## Bibliothèques manquantes

### `DllNotFoundException: gstreamer-1.0-0`

Windows : le dossier des natifs manque dans `Assets/StreamingAssets/VisioForge/x64/`.
Réimportez le paquet. Si vous exécutez un build Standalone, confirmez que
`<game>_Data/StreamingAssets/VisioForge/x64/` est aussi peuplé — Unity le copie
littéralement, donc un dossier absent dans le build signifie qu'il manquait dans le projet.

### `DllNotFoundException: libgstreamer_android`

Android : le Scripting Backend est réglé sur **Mono**. Passez à **IL2CPP** dans Project
Settings → Player → Other Settings → Configuration. Mono n'est pas pris en charge sur
Android par Unity lui-même.

### `DllNotFoundException: libgstreamer-1.0.0` *(macOS)*

Le bundle `.app` ne contient pas `libgstreamer-1.0.0.dylib` à l'un des emplacements sondés
(`Contents/PlugIns`, `Contents/PlugIns/macOS`, `Contents/Frameworks`,
`Contents/Resources/Data/Plugins/macOS`). Reconstruisez le paquet avec `-IncludeMacOS` et
réexportez le build Standalone.

### `dyld: Library not loaded: @rpath/GStreamerX.framework/GStreamerX`

iOS : le slot PluginImporter du framework n'a pas obtenu **Add To Embedded Binaries = YES**.
Réimportez le paquet ; le fichier `.meta` du paquet marque le framework correctement. Si
vous avez remplacé le framework manuellement, restaurez le `.meta` depuis un import frais.

## IL2CPP / stripping managé

### `MissingMethodException: GLib.SignalArgs..ctor` *(ou types GStreamer / GLib similaires)*

IL2CPP a supprimé un type managé auquel le SDK accède par réflexion. `Assets/VisioForge/link.xml` préserve ces types — confirmez que le fichier existe dans votre projet. Si vous
l'avez supprimé accidentellement, réimportez le `.unitypackage`. **Ne modifiez pas**
`link.xml` pour retirer des règles ; le paquet livre un ensemble testé.

### `MarshalDirectiveException` au premier appel SDK

Une signature `[DllImport]` a été supprimée ou son marshalling de délégué a échoué. Même
cause racine que `MissingMethodException` — confirmez que `link.xml` est en place et qu'IL2CPP
n'est pas configuré avec un niveau de stripping extra-agressif qui le surcharge.

## TLS / réseau

### Le stream RTSP expire avant de connecter *(iOS 14+)*

iOS bloque la première tentative de connexion à toute adresse réseau local jusqu'à ce que
votre `Info.plist` déclare pourquoi votre app a besoin d'un accès LAN. Ajoutez :

```xml
<key>NSLocalNetworkUsageDescription</key>
<string>Cette application diffuse des vidéos à partir de caméras IP locales sur votre réseau.</string>
```

Les relecteurs App Store attendent une raison visible par l'utilisateur, pas du boilerplate.

### RTSPS / HTTPS échoue avec erreur TLS / vérification de certificat

Le backend OpenSSL TLS de GIO n'a pas pu trouver un bundle CA :

- Windows / macOS : `Configure()` règle `SSL_CERT_FILE` sur le `ca-certificates.crt` fourni.
  Si manquant, la Console journalise `[VisioForge] CA certificate bundle not found at …`.
  Restagez les natifs via `deploy-unity-natives.ps1` et reconstruisez.
- Android / iOS : le bundle CA est une ressource managée embarquée extraite vers
  `<filesDir>/ssl/certs/`. Si l'extraction échoue, la Console journalise
  `[VisioForge] CA cert extraction failed`. Confirmez que `VisioForge.Core.dll` est dans
  `Assets/Plugins/Android/` (Android) ou `Assets/Plugins/iOS/Managed/` (iOS).

Pour les certificats auto-signés, installez-les dans le magasin de confiance système (hors
de la portée du SDK) ou — pour les tests seulement — désactivez la vérification des pairs
sur le bloc source. Le nom de la propriété au niveau du bloc varie ; voir
[Voir une caméra RTSP dans Unity](rtsp-viewer.md) pour l'exemple RTSP.

### Les URLs `http://` plates échouent sur iOS

App Transport Security (ATS) bloque `http://` par défaut. Soit passez à `https://`, soit, si
vous devez garder `http://`, ajoutez `NSAppTransportSecurity → NSAllowsArbitraryLoads = YES`
à votre `Info.plist`. Sachez que les relecteurs App Store peuvent demander pourquoi vous en
avez besoin.

## Cycle de vie Éditeur

### L'Éditeur se bloque sur "Reloading domain" après Play / Stop

Disable Domain Reload est désactivé. Réactivez-le dans Project Settings → Editor → Enter
Play Mode Settings → réglez **When entering Play Mode** sur **Reload Scene only** ou
**Do not reload Domain or Scene**. La boîte de dialogue de configuration unique du paquet le
configure pour vous ; si vous avez cliqué sur **Skip**, réglez-le manuellement.

### L'Éditeur plante au second Play

Le SDK a été arrêté sur Stop et réinitialisé sur Play. La solution :

- N'appelez pas `VisioForgeX.DestroySDK()` dans `OnDestroy` ni nulle part ailleurs. Le SDK
  est global au processus et réutilisé entre les sessions Play.
- Les players d'exemple (`MediaBlocksPlayer`, `RTSPViewerPlayer`) respectent ce motif —
  copiez la forme de leur teardown (ne libère que le pipeline par-Play ; ne détruit jamais
  le SDK).

Consultez [Démarrage et cycle de vie](bootstrap.md#cycle-de-vie-editeur) pour l'explication
complète.

### L'Éditeur devient instable après une longue session d'édition

Les recompilations de scripts répétées accumulent l'état GStreamer à travers les
rechargements de domaine. Redémarrez l'Éditeur pour récupérer. C'est surtout cosmétique —
les builds Standalone Player ne l'exhibent pas.

## Build / packaging

### Le bundle lancé depuis le Finder affiche "Damaged app" *(macOS)*

Le `.app` n'est pas signé et a été téléchargé avec le flag de quarantaine. Pour la
distribution, signez et notarisez le bundle (voir
[Compilation pour macOS](macos.md#signature-de-code-et-notarisation)). Pour les tests locaux
seulement, exécutez `xattr -d com.apple.quarantine <app-bundle>` une fois.

### App rejetée de la revue App Store pour "raison de confidentialité manquante" *(iOS)*

Une source de capture a besoin d'une clé `Info.plist` explicite :

- `NSCameraUsageDescription` si votre app capture depuis la caméra
- `NSMicrophoneUsageDescription` si votre app capture depuis le micro
- `NSLocalNetworkUsageDescription` si votre app parle à un appareil LAN

La chaîne visible par l'utilisateur doit décrire l'usage réel, pas "Required by SDK".

### `[VisioForge] Native runtime folder not found at '…' for runtime platform …`

Le `.unitypackage` que vous avez importé ne contient pas de déclinaison pour le Build Target
courant. Par exemple, un paquet Windows-seulement ouvert dans un Éditeur hôte Mac fait
apparaître cela au premier appel `InitializeSdk()`. Reconstruisez (ou retéléchargez) le
paquet avec le switch `-Include*` correspondant, ou importez la variante cumulative qui
contient toutes les plateformes.

## Quand tout le reste a échoué

Récupérez un log et contactez le support :

1. Export de la Console Éditeur ou Player (`Window → General → Console`, clic droit → Save
   All / via Logcat / via Xcode → Devices → Open Console).
2. Le nom du fichier `.unitypackage` et sa date de build.
3. Version d'Unity, Build Target, Scripting Backend, Api Compatibility Level.
4. Une scène reproductible minimale si possible.

Ouvrez un ticket à <https://support.visioforge.com/>.

## Voir aussi

- [Démarrage et cycle de vie](bootstrap.md) — comment le runtime est démarré sur chaque
  plateforme
- [Compilation pour Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
  — tables de dépannage spécifiques par plateforme
- [Matrice des plateformes](platform-matrix.md) — prise en charge des fonctionnalités par
  plateforme Unity
