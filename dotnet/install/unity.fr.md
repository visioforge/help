---
title: Installer Media Blocks SDK .NET dans Unity 6 — Guide
description: Installez VisioForge Media Blocks SDK .NET dans Unity 6 — importez le .unitypackage pour Windows, Android, macOS, iOS et lancez.
sidebar_label: Unity
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Installer le Media Blocks SDK dans Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce guide explique comment installer le **Media Blocks SDK .NET** dans **Unity 6**. Le SDK est
livré sous forme d'un seul **`.unitypackage`** autonome qui contient chaque runtime natif
pris en charge en un fichier — Windows x64, Android, macOS Standalone et iOS Standalone — et
laisse Unity choisir le bon par Build Target au moment du build. Vous ne compilez rien depuis
le code source, vous n'avez pas besoin de NuGet et il n'y a pas de dépendances externes à
installer.

Le paquet cible des assemblies managés **`netstandard2.1`**. Pour les projets ancrés au plus
ancien Mono Unity LTS, une déclinaison legacy `net48` Windows-seulement est encore publiée — voir le
spoiler en bas de cette page.

Une fois le paquet installé, voir les guides d'utilisation :
[Lire un fichier multimédia dans Unity](../general/unity/simple-player.md),
[Voir une caméra RTSP dans Unity](../general/unity/rtsp-viewer.md) et le
[Guide rapide](../general/unity/getting-started.md).

## Prérequis

| | |
|---|---|
| Unity | **6 (6000.x)** — vérifié sur `6000.4.6f1` |
| Targets de build livrés | **Windows x64**, **Android arm64**, **macOS Universel arm64+x86_64**, **iOS appareil arm64** |
| TFM managé | **`netstandard2.1`** |
| Réglages Éditeur obligatoires | `Api Compatibility Level = .NET Standard 2.1` et Disable Domain Reload |

!!! warning "Utilisez un chemin court sur NTFS — pas un volume Dev Drive / ReFS"
    Importer le paquet écrit des milliers de petits fichiers natifs, et l'import/compilation
    d'Unity est lourd en E/S de petits fichiers. Sur un Dev Drive (ReFS), c'est
    **dramatiquement plus lent** (un import à froid peut prendre plusieurs minutes au lieu de
    secondes) et plus sujet à la race `EPERM rename`. Gardez le projet sur un lecteur **NTFS**
    simple avec un chemin racine court, par ex. `C:\unity\MyApp`. Le cache de paquets d'Unity
    produit aussi des chemins profonds qui peuvent dépasser la limite de 260 caractères
    `MAX_PATH` de Windows.

## Téléchargement

Téléchargez le dernier paquet cumulatif — Windows + Android + macOS + iOS en un fichier :

[**VisioForge.MediaBlocks.Unity.unitypackage**](https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage)

```text
https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage
```

## Étape 1 — Créer ou ouvrir un projet Unity

Utilisez un projet Unity 6 existant ou créez-en un nouveau (n'importe quel modèle). Gardez la
racine du projet sur un chemin NTFS court (voir l'avertissement plus haut).

![Création d'un projet Unity 6 sur un chemin NTFS court dans Unity Hub](unity-new-project.webp)

## Étape 2 — Importer le paquet

Dans l'Éditeur : **Assets → Import Package → Custom Package…**, sélectionnez le
`.unitypackage` téléchargé et cliquez sur **Import** (gardez tous les éléments cochés).

![Boîte de dialogue Import Unity Package montrant le contenu du paquet VisioForge](unity-import-package.webp)

Le paquet ajoute :

| Contenu | Emplacement | Rôle |
|---|---|---|
| SDK managé (`netstandard2.1`) + dépendances | `Assets/Plugins/` (+ sous-dossiers `Android/`, `macOS/`, `iOS/Managed/`) | les assemblies Media Blocks SDK .NET, par plateforme |
| Runtime natif Windows | `Assets/StreamingAssets/VisioForge/x64/` | libs et plugins GStreamer pour Windows |
| Runtime natif Android | `Assets/Plugins/Android/libs/arm64-v8a/` | `libgstreamer_android.so` monolithique + AAR Java |
| Runtime natif macOS | `Assets/Plugins/macOS/` | dylibs universels (arm64+x86_64) |
| Runtime natif iOS | `Assets/Plugins/iOS/GStreamerX.framework/` | framework embarqué (appareil arm64) |
| Préservation IL2CPP | `Assets/VisioForge/link.xml` | préservation des types / membres pour Android et iOS |
| Scripts réutilisables | `Assets/Scripts/` | `VisioForgeEnvironment`, `VisioForgeVideoView` et les deux players |
| Deux scènes prêtes | `Assets/Scenes/` | `SimplePlayer` (fichier) et `RTSPViewer` (RTSP) |
| Assistant de configuration unique | `Assets/VisioForge/Editor/` | applique les deux réglages de projet requis |

Les métadonnées `PluginImporter` par déclinaison sur chaque fichier natif disent à Unity à quel
Build Target chaque binaire appartient — basculer le Build Target dans Build Profiles choisit
automatiquement le bon slot au moment du build.

## Étape 3 — Appliquer les réglages de projet requis

Au premier import, l'assistant de configuration affiche une boîte de dialogue qui demande
d'appliquer deux réglages de projet requis. Cliquez sur **Apply** — les deux sont configurés
pour vous.

![Boîte de dialogue de configuration du Media Blocks SDK VisioForge avec les boutons Apply et Skip](unity-apply-dialog.webp)

Ces deux réglages sont **obligatoires** — le SDK ne fonctionnera pas sans eux :

- **Api Compatibility Level = .NET Standard 2.1** — le SDK est livré sous forme d'assemblies
  `netstandard2.1` ; le réglage legacy `.NET Framework` ne peut pas les charger.
- **Disable Domain Reload** — le SDK s'initialise une fois par processus et est réutilisé
  entre les sessions Play/Stop ; avec Domain Reload activé, l'Éditeur peut se bloquer en
  sortant du mode Play.

Pour les targets mobiles, basculez aussi **Scripting Backend** sur **IL2CPP** — Mono n'est
pas pris en charge sur Android ou iOS par Unity lui-même. Voir
[Compilation pour Android](../general/unity/android.md) et
[Compilation pour iOS](../general/unity/ios.md) pour les checklists Build Profile par cible.

## Étape 4 — Régler les réglages manuellement (seulement si vous avez cliqué sur Skip)

Si vous avez cliqué sur **Skip**, réglez les deux à la main :

1. **Api Compatibility Level = .NET Standard 2.1**
   *Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility
   Level*.

   ![Réglages Player avec Api Compatibility Level défini sur .NET Standard 2.1](unity-apicompat.webp)

2. **Disable Domain Reload**
   *Edit → Project Settings → Editor → Enter Play Mode Settings* → réglez
   **When entering Play Mode** sur une option qui **ne recharge pas** le domaine — soit
   **Reload Scene only** (correspond à ce que fait **Apply**) soit
   **Do not reload Domain or Scene**.

   ![Enter Play Mode Settings de l'Éditeur avec le rechargement de domaine désactivé](unity-domain-reload.webp)

## Étape 5 — Exécuter une scène d'exemple

Dans la fenêtre **Project** ouvrez `Assets/Scenes/SimplePlayer.unity` (double-clic — ne restez
pas sur la scène par défaut vide), sélectionnez le GameObject **RawImage**, réglez son
**File Path** dans l'Inspector et appuyez sur **▶ Play**. La vidéo s'affiche dans la vue Game
et l'audio joue via le périphérique audio par défaut du système.

![La scène SimplePlayer joue une vidéo dans la vue Game d'Unity](unity-sample-play.webp)

!!! tip "Le RawImage paraît vide jusqu'à ce que vous appuyiez sur Play"
    La texture vidéo est créée à l'exécution, donc le `RawImage` est vide (blanc) en mode
    édition.

Ensuite, lisez les guides d'utilisation :

- [Guide rapide](../general/unity/getting-started.md) — le chemin en cinq étapes de l'import
  à la lecture.
- [Lire un fichier multimédia dans Unity](../general/unity/simple-player.md) — l'exemple
  `SimplePlayer`.
- [Voir une caméra RTSP dans Unity](../general/unity/rtsp-viewer.md) — l'exemple `RTSPViewer`.

## Compilez pour une plateforme cible

Le `.unitypackage` cumulatif contient chaque plateforme prise en charge, mais chaque Build
Target a ses propres réglages et pièges. Lisez la page correspondante :

- [Compilation pour Windows](../general/unity/windows.md) — x86_64 Éditeur + Standalone
  Player.
- [Compilation pour Android](../general/unity/android.md) — IL2CPP arm64, permissions
  AndroidManifest.
- [Compilation pour macOS](../general/unity/macos.md) — Universel arm64+x86_64, signature de
  code.
- [Compilation pour iOS](../general/unity/ios.md) — flux d'export Xcode, permissions
  Info.plist.

## Désinstaller ou mettre à jour le paquet

Un `.unitypackage` n'a pas de désinstalleur : retirez les fichiers manuellement.

1. **Fermez d'abord l'Éditeur Unity** — il verrouille les DLL natives et le cache `Library/`.
2. Supprimez le contenu VisioForge de `Assets/` :
   - `Assets/StreamingAssets/VisioForge/` — le runtime natif Windows.
   - `Assets/Plugins/Android/libs/arm64-v8a/libgstreamer_android.so`, `libVisioForge_Core.so`
     et `Assets/Plugins/Android/visioforge-gstreamer.aar` — le runtime Android.
   - `Assets/Plugins/macOS/*.dylib` et `Assets/Plugins/macOS/ca-certificates.crt` — le
     runtime macOS.
   - `Assets/Plugins/iOS/GStreamerX.framework/` et `Assets/Plugins/iOS/libVisioForge_Core.a`
     — le runtime iOS.
   - `Assets/Plugins/` (avec les sous-dossiers `Android/`, `macOS/`, `iOS/Managed/`) — les assemblies managés, par plateforme.
   - `Assets/VisioForge/` — l'assistant de configuration unique et `link.xml`.
   - Les quatre scripts réutilisables dans `Assets/Scripts/` : `VisioForgeEnvironment.cs`,
     `VisioForgeVideoView.cs`, `MediaBlocksPlayer.cs`, `RTSPViewerPlayer.cs` (plus leurs
     `.meta`) — gardez tout script de votre cru qui vit dans le même dossier.
   - Les scènes d'exemple `Assets/Scenes/SimplePlayer.unity` et
     `Assets/Scenes/RTSPViewer.unity`.
3. Supprimez le dossier `Library/` du projet (à côté de `Assets/`) pour effacer l'état
   d'import en cache. Unity le régénère à la prochaine ouverture (le premier lancement est
   plus lent).

**Mise à jour :** importez le nouveau `.unitypackage` par-dessus l'ancien — les GUIDs des
plugins managés sont déterministes, donc Unity écrase les assets existants sur place et les
références sont préservées. Si vous venez d'un paquet bien plus ancien ou voyez des DLLs
dupliquées dans `Assets/Plugins/`, faites d'abord une suppression propre (étapes ci-dessus),
puis importez le nouveau paquet.

## Dépannage

| Symptôme | Cause | Solution |
|---|---|---|
| `TypeLoadException` au lancement | Api Compatibility Level est `.NET Framework`, pas `.NET Standard 2.1` | Réglez-le sur `.NET Standard 2.1`, ou réimportez et cliquez sur **Apply** |
| L'Éditeur se bloque sur "Reloading domain" sur Play/Stop | Domain Reload est activé | Gardez Disable Domain Reload activé |
| L'Éditeur plante au 2e Play | Le SDK a été arrêté sur Stop et réinitialisé | Ne coupez pas le SDK sur Stop ; gardez Disable Domain Reload activé |
| Runtime natif introuvable | Paquet importé partiellement, ou déclinaison du bon Build Target absente du paquet | Réimportez le paquet avec tous les éléments cochés ; confirmez que le paquet contient la plateforme que vous ciblez |
| Pas de vidéo, erreurs dans la Console après l'import | L'Éditeur a besoin d'un rechargement propre après que le runtime a été stagé | Redémarrez l'Éditeur |
| `DllNotFoundException` sur Android | Le Scripting Backend est Mono | Passez à IL2CPP |

Pour la référence complète par symptôme, voir
[Dépannage](../general/unity/troubleshooting.md).

## Déclinaison legacy `net48` Windows-seulement

??? note "J'ai un Unity LTS plus ancien ancré à Mono — qu'en est-il du build net48 ?"

    Le build Windows-seulement original du paquet cible des assemblies managés
    **`.NET Framework 4.8`** et est encore produit pour les projets qui ne peuvent pas
    migrer vers `.NET Standard 2.1` (par exemple, Unity 2019.4 LTS sans l'option Api
    Compatibility moderne). Il est livré comme un `.unitypackage` séparé avec `NET48` dans
    le nom de fichier, contient seulement le runtime natif Windows-x64 et utilise
    `.NET Framework` comme Api Compatibility Level. Les nouveaux projets devraient utiliser
    le paquet `netstandard2.1` décrit plus haut — il couvre le même cas Windows-x64 plus
    toutes les autres plateformes, et Unity 6 l'utilise par défaut. Si vous avez une
    exigence stricte pour le build `net48`, contactez le support pour le lien de
    téléchargement le plus récent.

## Foire aux questions

### Puis-je installer le SDK dans Unity via NuGet ?

Non. Unity n'exécute pas la restauration NuGet, et le SDK livre des centaines de fichiers
natifs que NuGet ne disposerait pas pour Unity. Le `.unitypackage` rassemble tout —
assemblies managés, runtime natif de chaque plateforme, scripts et scènes — donc vous
importez un seul fichier à la place.

### Dois-je installer GStreamer ou une autre dépendance système ?

Non. Le paquet est entièrement autonome ; tout ce dont le SDK a besoin est à l'intérieur.
Une installation GStreamer système sur votre machine n'est pas requise et n'est pas
utilisée par le runtime fourni — au contraire,
`VisioForgeEnvironment.Configure()` retire activement tout GStreamer système du chemin de
recherche du processus pour éviter un double-init.

### Les autres SDKs VisioForge fonctionnent-ils dans Unity ?

Aujourd'hui le paquet Unity livre le runtime **Media Blocks SDK .NET**, qui couvre la
lecture, la capture, le traitement et le streaming. D'autres SDKs VisioForge suivront.

### Le même paquet fonctionne-t-il sur Windows ARM64 ?

Le runtime natif Windows du paquet est x86_64 seulement — il n'y a pas de build natif ARM64
aujourd'hui. Exécutez-le via émulation x64 seulement à vos propres risques ; un usage en
production sur Windows 11 ARM64 n'est pas exercé.

### Puis-je ouvrir le même paquet dans l'Éditeur hôte Mac ?

Oui — si le paquet a été construit avec `-IncludeMacOS`. La variante cumulative publiée à
`files.visioforge.com/unity/` contient toujours la déclinaison macOS. Un paquet Windows-seulement
ouvert dans un Éditeur Mac fait apparaître un message clair
`[VisioForge] Native runtime folder not found at '…' for runtime platform OSXEditor` ; voir
[Démarrage et cycle de vie](../general/unity/bootstrap.md).

## Voir aussi

- [Utiliser VisioForge dans Unity](../general/unity/index.md) — vue d'ensemble du
  fonctionnement de l'intégration
- [Guide rapide](../general/unity/getting-started.md) — chemin en cinq étapes vers une vidéo
  qui joue
- [Démarrage et cycle de vie](../general/unity/bootstrap.md) — ce que font `Configure()` et
  `InitializeSdk()`
- [Lire un fichier multimédia dans Unity](../general/unity/simple-player.md) — l'exemple de
  lecture de fichiers
- [Voir une caméra RTSP dans Unity](../general/unity/rtsp-viewer.md) — l'exemple RTSP
- [Matrice des plateformes](../general/unity/platform-matrix.md) — prise en charge des
  fonctionnalités par plateforme Unity
- [Aperçu du Media Blocks SDK .NET](../mediablocks/index.md) — le catalogue complet de blocs
- [Guide d'installation](index.md) — installez le SDK dans d'autres types de projets .NET
