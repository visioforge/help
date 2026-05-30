---
title: Compiler un jeu Unity de lecture vidéo pour macOS Universel
description: Réglages de build, organisation des dylibs, signature de code et dépannage pour le VisioForge Media Blocks SDK .NET dans Unity 6 sur macOS Standalone.
sidebar_label: Compilation pour macOS
order: 55
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - macOS
  - Standalone Player
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Compilation pour macOS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

La déclinaison macOS livre un runtime GStreamer Universel (arm64 + x86_64) plus les assemblies
managés du paquet compilés contre `netstandard2.1`. Le runtime natif est un ensemble de
~300 dylibs séparés — libs core GStreamer, plugins, modules GIO, backend OpenSSL TLS, bundle
CA — à plat dans `Assets/Plugins/macOS/`. Cette page couvre les réglages du Build Profile,
l'organisation des dylibs et les pièges propres à macOS ; pour la séquence de démarrage
consultez [Démarrage et cycle de vie](bootstrap.md).

La déclinaison macOS est incluse dans le `.unitypackage` quand le pipeline de build est lancé avec
`-IncludeMacOS`. Le résultat est un paquet cumulatif qui contient Windows-x64, Android et
macOS ensemble — Unity choisit les bons fichiers par Build Target via les métadonnées
`PluginImporter` par fichier.

## Player Settings

| Réglage | Valeur | Où |
|---|---|---|
| Target Platform | **macOS** | File → Build Profiles → macOS |
| Architecture | **Intel 64-bit + Apple silicon** (Universel) | File → Build Profiles → macOS |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **Mono** *ou* **IL2CPP** *(les deux sont testés)* | Project Settings → Player → Other Settings → Configuration |
| Mac App Store Validation | **Off** *(ou signez d'abord les dylibs GStreamer, voir plus bas)* | Project Settings → Player → Other Settings |
| Enter Play Mode → Reload Domain | **Off** | Project Settings → Editor → Enter Play Mode Settings |

Les deux scripting backends sont testés. Mono est par défaut et plus rapide à itérer ; passez
à IL2CPP seulement si vous avez une raison à l'échelle du projet. Le même `link.xml` que le
paquet livre préserve les types managés du SDK sur n'importe quel backend.

## Organisation sur disque

L'étape `deploy-unity-natives.ps1 -Platform macOS` met en place le runtime macOS comme suit :

| Chemin | Contenu |
|---|---|
| `Assets/Plugins/macOS/libgstreamer-1.0.0.dylib` | Bibliothèque core GStreamer. |
| `Assets/Plugins/macOS/libgio-2.0.0.dylib`, `libglib-2.0.0.dylib`, `libgobject-2.0.0.dylib` | Core GLib. |
| `Assets/Plugins/macOS/libgst*.dylib` | Chaque plugin GStreamer (decoders, encoders, sources, sinks, éléments de base). |
| `Assets/Plugins/macOS/libgioopenssl.so` | Backend GIO TLS via lequel RTSPS / HTTPS vérifient les pairs. |
| `Assets/Plugins/macOS/ca-certificates.crt` | Bundle CA pour OpenSSL. |
| `Assets/Plugins/macOS/libVisioForge_Core.dylib` | Shim natif du SDK. |
| `Assets/VisioForge/link.xml` | Règles de préservation IL2CPP (partagées avec Android). |
| `Assets/Plugins/macOS/` | Assemblies managés compilés contre `netstandard2.1` avec `UNITY_NS21_MACOS` défini. |

L'organisation est plate — le `@rpath` / `@loader_path` de chaque dylib est précuit par le
NuGet pack, donc une fois que dyld a chargé l'un d'eux via le premier `[DllImport]`, les
voisins se résolvent automatiquement.

## Build Standalone

**File → Build Profiles → macOS → Build** produit un bundle `.app`.

L'emplacement final des natifs dans le bundle dépend de la version d'Unity :

| Layout | Utilisé par | `VisioForgeEnvironment.NativesPath` résout vers |
|---|---|---|
| `<app>.app/Contents/PlugIns/` | Unity 6 par défaut | `…/Contents/PlugIns` |
| `<app>.app/Contents/PlugIns/macOS/` | certaines versions patch d'Unity 6 | `…/Contents/PlugIns/macOS` |
| `<app>.app/Contents/Frameworks/` | layouts Unity plus anciens | `…/Contents/Frameworks` |
| `<app>.app/Contents/Resources/Data/Plugins/macOS/` | layouts très anciens | `…/Contents/Resources/Data/Plugins/macOS` |

`NativesPath` sonde les quatre emplacements au démarrage, à la recherche de la sentinelle
`libgstreamer-1.0.0.dylib`. Le premier dossier qui la contient gagne, et le résultat est mis
en cache pour le reste du processus — pas de réglage par version d'Unity.

## Taille de build

La déclinaison macOS ajoute environ **100 Mo** au bundle `.app` (Universel arm64 + x86_64). C'est
la plus grosse des déclinaisons du paquet cumulatif parce que chaque plugin est livré sous forme de
son propre dylib et que les deux architectures sont incluses. Si vous ne ciblez qu'Apple
silicon, vous pouvez post-traiter le bundle pour retirer les slices x86_64 avec `lipo`, mais
le paquet lui-même ne les sépare pas par défaut.

## Signature de code et notarisation

Pour la distribution hors Mac App Store, vous voulez généralement signer et notariser le
bundle :

1. **Hardened Runtime** activé (Project Settings → Player → Other Settings ou votre flux de
   signature).
2. **Codesignez chaque dylib dans `Contents/PlugIns/`** avec votre certificat Developer ID
   Application avant de signer le `.app` lui-même. Unity ne signe pas les plugins tiers pour
   vous.
3. **Notarisez** le `.app` final (ou son `.zip` / `.dmg`) avec
   `xcrun notarytool submit … --wait`.
4. **Staplez** le ticket de notarisation avec `xcrun stapler staple <app-bundle>`.

Les dylibs GStreamer ne nécessitent aucun entitlement Apple au-delà de la configuration par défaut Hardened
Runtime ; ils n'accèdent pas à des ressources protégées depuis leur code natif. Votre propre
app détermine quels entitlements (caméra, micro, réseau, etc.) sont requis.

Pour la soumission Mac App Store, les `libgioopenssl.so` et `libgmp.10.dylib` fournis sont
liés statiquement et livrés sous licences permissives, mais la revue App Store peut signaler
l'extension `.so` pour un bundle macOS. Si vous avez besoin d'une distribution App Store,
contactez le support — ce chemin n'est pas exercé par la CI du paquet.

## Dépannage

| Symptôme | Cause | Solution |
|---|---|---|
| `DllNotFoundException: libgstreamer-1.0.0` sur Play | `Plugins/macOS/` est vide ou la sentinelle `libgstreamer-1.0.0.dylib` manque. | Réimportez le `.unitypackage` avec tous les éléments cochés. La Console affiche le `NativesPath` résolu — confirmez que la sentinelle est là. |
| `[VisioForge] Native runtime folder not found at '…/Contents/PlugIns'` sur un build Standalone | Les plugins n'ont pas été stagés dans le `.app` parce que la déclinaison macOS n'était pas dans le paquet. | Reconstruisez le paquet avec `-IncludeMacOS` (ou importez le `.unitypackage` cumulatif qui contient macOS). |
| Le pipeline se bloque au démarrage, le log montre deux appels `gst_init` | Une installation GStreamer homebrew ou système est sur `DYLD_LIBRARY_PATH`. | `Configure()` la nettoie — confirmez que le nombre de suppressions est non nul dans la Console. Hardened Runtime supprime `DYLD_*` avant que le processus démarre, donc c'est surtout une préoccupation de l'Éditeur Mono. |
| RTSPS / HTTPS échoue avec `Peer certificate cannot be authenticated with given CA certificates` | `ca-certificates.crt` introuvable au chemin attendu. | `Configure()` journalise un avertissement si le bundle manque. Réimportez le paquet ou réexécutez `deploy-unity-natives.ps1 -Platform macOS`. |
| Le bundle lancé depuis le Finder affiche le dialogue `Damaged app` | Le `.app` n'est pas signé et a été téléchargé avec le flag de quarantaine. | Signez + notarisez pour la distribution, ou pour les tests locaux exécutez `xattr -d com.apple.quarantine <app-bundle>` une fois. |
| Le bundle lancé depuis un build TestFlight Mac App Store plante | Les fichiers `.so` dans le bundle violent les règles d'organisation App Store. | Contactez le support — la soumission App Store nécessite un packaging natif alternatif. |

## Foire aux questions

### La déclinaison macOS fonctionne-t-elle dans l'Éditeur sur un Mac ?

Oui — `OSXEditor` (l'Éditeur lui-même) et `OSXPlayer` (un build Standalone) sont tous deux
des targets runtime admis. `Configure()` résout `Plugins/macOS/` depuis la racine du projet
dans l'Éditeur et sonde le layout du bundle dans le player.

### Ai-je besoin de la déclinaison macOS pour ouvrir le paquet dans un Éditeur hôte Mac ?

Oui. Le `.unitypackage` que vous importez doit contenir la déclinaison macOS (`-IncludeMacOS`)
pour que l'Éditeur hôte Mac trouve un runtime natif à charger. Un paquet Windows-seulement
ouvert dans un Éditeur Mac fera apparaître le décalage inter-déclinaisons sous la forme
`[VisioForge] Native runtime folder not found at '…' for runtime platform OSXEditor` —
consultez [Démarrage et cycle de vie](bootstrap.md).

### Puis-je livrer seulement Apple silicon et omettre x86_64 ?

Oui. Après le build, exécutez `lipo -thin arm64 <dylib> -output <dylib>` sur chaque `.dylib`
dans `Contents/PlugIns/` pour retirer les slices Intel. Le paquet ne le fait pas par défaut
parce que les deux architectures restent utiles pour les tests de compatibilité.

### Le même paquet fonctionne-t-il sur iOS aussi ?

La déclinaison iOS est livrée comme plateforme séparée dans le même `.unitypackage` quand il est
construit avec `-IncludeIOS`. Consultez [Compilation pour iOS](ios.md).

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — configuration du paquet
- [Démarrage et cycle de vie](bootstrap.md) — comment `Configure()` trouve le runtime macOS
- [Compilation pour iOS](ios.md) — la déclinaison iOS correspondante (nécessite un hôte Mac)
- [Voir une caméra RTSP dans Unity](rtsp-viewer.md) — l'exemple `RTSPViewer`
- [Dépannage](troubleshooting.md) — référence des erreurs inter-plateformes
