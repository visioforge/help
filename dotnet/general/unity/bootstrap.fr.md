---
title: Bootstrap du SDK dans Unity — Configuration du runtime natif
description: Comment VisioForgeEnvironment.Configure démarre le runtime natif GStreamer dans Unity 6 sur Windows, Android, macOS et iOS.
sidebar_label: Démarrage et cycle de vie
order: 52
tags:
  - Media Blocks SDK
  - Media Player SDK
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - Unity
  - Bootstrap
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - VisioForgeX
---

# Démarrage et cycle de vie

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Le paquet Unity inclut un assistant statique, `VisioForgeEnvironment`, qui prépare le runtime natif
fourni avant le chargement de la première scène. Vous ne l'appelez pas depuis vos scripts —
Unity l'invoque automatiquement via `RuntimeInitializeOnLoadMethod`. Cette page explique ce
qu'il fait sur chaque plateforme et les règles de cycle de vie que vos scripts doivent
respecter.

Le même démarrage en deux étapes s'applique à **tous** les moteurs du paquet : le
`MediaBlocksPipeline` de bas niveau et les moteurs de haut niveau `MediaPlayerCoreX`,
`VideoCaptureCoreX` et `VideoEditCoreX` s'exécutent tous sur l'unique runtime GStreamer fourni que
démarrent `Configure()` et `InitializeSdk()`. Le démarrage est indépendant du moteur — rien sur
cette page n'est spécifique à un seul moteur.

Si vous voulez seulement déployer le SDK, vous pouvez sauter cette page : déposez
`MediaBlocksPlayer` ou `RTSPViewerPlayer` dans une scène et appuyez sur **Play**. Revenez ici
quand vous construisez vos propres scripts, rencontrez une erreur à l'exécution ou voulez
comprendre pourquoi les réglages Éditeur sont obligatoires.

## Les deux points d'entrée

`VisioForgeEnvironment` expose exactement deux méthodes publiques avec lesquelles votre code
interagit :

| Méthode | Quand elle s'exécute | Ce qu'elle fait |
|---|---|---|
| `Configure()` | Automatiquement, **avant le chargement de la première scène** (`[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]`). | Prépare le runtime natif pour la plateforme courante — chemin de recherche DLL, variables d'environnement, bundle CA, bootstrap Java. Idempotente. |
| `InitializeSdk()` | Vous l'appelez une fois avant de construire un pipeline. Les players d'exemple le font dans `Start()`. | Appelle `VisioForgeX.InitSDK()` et scanne le registre de plugins fourni. Idempotente. |

Les deux méthodes sont statiques. Les deux ne marquent leur succès qu'après l'exécution réussie
de chaque étape — un échec récupérable (par exemple le bootstrap Java Android avant que
`currentActivity` ne soit prête) laisse le drapeau désactivé pour qu'un appel ultérieur
relance le travail au lieu d'un no-op silencieux vers un état cassé.

## Ce que fait `Configure()` sur chaque plateforme

`Configure()` dispatch sur les symboles de compilation (`UNITY_STANDALONE_WIN`,
`UNITY_STANDALONE_OSX`, `UNITY_ANDROID`, `UNITY_IOS`) — Unity en définit exactement un par
target de build. Le corps de chaque branche est le minimum dont la plateforme a besoin pour
que GStreamer trouve ses plugins, localise ses racines TLS et résolve le reste du runtime via
son chargeur natif.

=== "Windows"

    1. Valide que le dossier de natifs fourni existe (`StreamingAssets/VisioForge/x64`).
       Refuse de modifier l'état du processus sinon — un dossier absent ne doit pas empoisonner
       le `PATH` du processus.
    2. Retire toute entrée GStreamer système du **PATH du processus** (une installation
       homebrew / système sur `PATH` chargerait une seconde copie physique de
       `gstreamer-1.0-0.dll`, enregistrerait les types GLib en double et bloquerait le
       pipeline).
    3. Pointe le chargeur DLL Win32 vers les natifs fournis via `SetDllDirectoryW`.
    4. Préfixe le dossier de natifs au `PATH` du processus pour que les dépendances core-lib
       transitives de chaque plugin se résolvent (`SetDllDirectory` seul ne suffit pas).
    5. Définit `GST_PLUGIN_PATH` / `GST_PLUGIN_SYSTEM_PATH` au même dossier plat.
    6. Définit `SSL_CERT_FILE` et `CA_CERTIFICATES` au `ca-certificates.crt` fourni pour que
       RTSPS et HTTPS vérifient les pairs.

    Le `PATH` utilisateur / système n'est jamais touché — seule la copie vive du processus.

=== "macOS"

    1. Résout le chemin des natifs en sondant les layouts connus d'Unity : `Plugins/macOS`
       (Éditeur et certaines versions du Standalone Player), puis `Contents/PlugIns`,
       `Contents/PlugIns/macOS`, `Contents/Frameworks` et
       `Contents/Resources/Data/Plugins/macOS` pour un `.app` Standalone. Le premier dossier
       qui contient `libgstreamer-1.0.0.dylib` gagne. Le résultat est mis en cache.
    2. Élimine tout GStreamer système / homebrew de `DYLD_LIBRARY_PATH` (`/opt/homebrew/lib`,
       `/usr/local/lib`) — même mode de défaillance double-init que le nettoyage `PATH` Windows.
    3. Définit `GST_PLUGIN_PATH` / `GST_PLUGIN_SYSTEM_PATH` au dossier des natifs pour que
       GStreamer puisse énumérer les plugins fournis.
    4. Définit `GIO_MODULE_DIR` pour que GIO trouve `libgioopenssl.so` (le backend TLS via
       lequel RTSPS / HTTPS vérifient les pairs).
    5. Définit `SSL_CERT_FILE` et `CA_CERTIFICATES` au bundle CA fourni.

    Chaque `.dylib` fourni a son `@rpath` / `@loader_path` précuit par le NuGet pack, donc une
    fois que dyld a chargé l'un d'eux via le premier `[DllImport]`, les autres résolvent leurs
    voisins automatiquement — pas d'équivalent `SetDllDirectory` nécessaire.

=== "Android"

    1. Acquiert `com.unity3d.player.UnityPlayer.currentActivity` via JNI. Si le champ est null
       — variantes Wear OS / Android TV, hôtes Unity-as-a-library qui ne l'ont pas encore
       assigné, paths de démarrage `BeforeSceneLoad` très précoces sur certains Unity 6 —
       échoue rapidement avec une exception descriptive pour que le client voie mieux qu'un
       `NullReferenceException` opaque depuis l'intérieur de JNI.
    2. Résout `getFilesDir()` et `getCacheDir()` depuis l'Activity.
    3. Capture les valeurs antérieures de `TMP`, `TEMP`, `TMPDIR`, `XDG_RUNTIME_DIR`,
       `XDG_CACHE_HOME`, `HOME`, `GST_REGISTRY`, `SSL_CERT_FILE`, `CA_CERTIFICATES` pour
       qu'une défaillance ultérieure puisse les restaurer.
    4. Pointe GLib vers les répertoires accessibles en écriture privés de l'app en définissant les variables
       ci-dessus (seuls les répertoires privés de l'app sont accessibles en écriture sur Android).
    5. Extrait la ressource embarquée `ca-certificates.crt` vers `filesDir/ssl/certs/` et
       pointe `SSL_CERT_FILE` / `CA_CERTIFICATES` là.
    6. Appelle `org.freedesktop.gstreamer.GStreamer.init(activity)` — cela charge
       `libgstreamer_android.so`, capture la JavaVM dans `JNI_OnLoad`, exécute `gst_init` et
       enregistre chaque plugin statiquement lié au monolithe.
    7. Si l'un d'eux lève une exception, restaure l'environnement capturé et la relance.

=== "iOS"

    1. Extrait la ressource embarquée `ca-certificates.crt` vers
       `Application.persistentDataPath/ssl/certs/`.
    2. Définit `SSL_CERT_FILE` / `CA_CERTIFICATES` à ce chemin pour que le backend OpenSSL de
       GIO puisse vérifier les pairs RTSPS / HTTPS.
    3. Précharge le cache `NativesPath` sur le thread principal (`s_cachedNativesPath =
       Application.dataPath.Replace('\\', '/')`). Sans ce préchargement, un lecteur sur un
       thread d'arrière-plan — le bus GStreamer, un callback de log async, un callback
       pad-added — accéderait plus tard au getter évalué à la demande et appellerait
       `Application.dataPath` hors du thread principal, ce qui lève `UnityException`. La
       formule de calcul correspond au getter évalué à la demande octet pour octet.

    iOS n'a pas besoin de scan de plugins : chaque plugin GStreamer est enregistré
    statiquement dans `GStreamerX.framework` à
    `gst_plugin_register_static()`, et dyld résout
    `@rpath/GStreamerX.framework/GStreamerX` automatiquement quand le premier `[DllImport]`
    se déclenche.

=== "Autre"

    `Configure()` définit le drapeau de succès et journalise un avertissement. Aucun runtime
    natif n'existe pour la plateforme courante, donc tout `[DllImport]` ultérieur lancera
    `DllNotFoundException`. Compilez pour une des quatre plateformes prises en charge à la
    place.

## Ce que fait `InitializeSdk()`

Après que `Configure()` a préparé le runtime, `InitializeSdk()` finalise le démarrage :

1. Refuse de s'exécuter si `Configure()` n'a jamais réussi — l'échec rapide ici fait remonter
   une erreur actionnable au lieu d'un `DllNotFoundException` au fond du SDK.
2. Refuse de s'exécuter sur une valeur `Application.platform` non prise en charge à l'exécution
   (Windows / Android / macOS / iOS sont admis ; tout le reste court-circuite avec un
   avertissement).
3. Sur Windows et macOS, avant d'appeler le SDK natif, revérifie que le dossier de natifs
   résolu existe sur le disque. Cela attrape le décalage entre déclinaisons (un `.unitypackage`
   Windows seulement importé dans un hôte macOS, ou l'inverse) avec un message clair plutôt
   qu'un `DllNotFoundException` opaque. Android et iOS sautent ce contrôle (pas de dossier à
   sonder).
4. Appelle `VisioForgeX.InitSDK()`. Capture et journalise en cas d'échec ; laisse le drapeau
   désactivé pour qu'une tentative ultérieure puisse réussir.
5. Sur Windows et macOS, scanne explicitement le dossier de plugins fourni avec
   `Gst.Registry.Get().ScanPath(NativesPath)`. Le scan in-process des plugins d'Unity
   n'honore pas de façon fiable `GST_PLUGIN_PATH` sur l'une ou l'autre plateforme ; le scan
   explicite est ce qui fait charger des blocs comme `BufferSinkBlock` (qui dépend de
   `appsink`). Android enregistre les plugins statiquement dans `GStreamer.init` ; iOS les
   enregistre statiquement dans le framework — les deux sautent le scan.
6. Définit le drapeau de succès.

Tous les scripts d'exemple — ceux de bas niveau `MediaBlocksPlayer` / `RTSPViewerPlayer` et ceux de
haut niveau `MediaPlayerXPlayer` / `VideoCaptureXRecorder` / `IPCameraXViewer` / `VideoEditXRenderer`
— appellent `InitializeSdk()` depuis leur méthode `Start()`, avant de construire un pipeline ou de
créer un moteur. Vos scripts devraient suivre le même schéma.

## Cycle de vie Éditeur

Le SDK s'initialise une fois par processus Éditeur et est réutilisé entre les sessions
**Play → Stop → Play**. Deux conséquences :

- **Disable Domain Reload est obligatoire.** Avec lui activé, sortir du mode Play déclenche un
  rechargement de domaine alors que le thread de la boucle principale GLib du SDK tourne
  encore, ce qui peut bloquer l'Éditeur. La boîte de dialogue de configuration que le paquet
  affiche au premier import le configure pour vous ; réglez-le manuellement dans
  **Edit → Project Settings → Editor → Enter Play Mode Settings** si vous avez sauté ce
  dialogue.
- **N'appelez pas `VisioForgeX.DestroySDK()` sur Stop ou dans `OnDestroy`.** Le `gst_deinit`
  de GStreamer ne peut pas être réinitialisé dans le même processus — détruire le SDK sur Stop
  et tenter de l'utiliser au prochain Play plante au sein du registre natif. Les players
  d'exemple respectent cette règle : leur `OnDestroy` ne libère que le pipeline par-Play. Le
  SDK reste vivant pour le reste du cycle de vie du processus.

Il y a un guard Éditeur-seul que le paquet installe automatiquement : un
`VisioForgeEditorReloadGuard` qui appelle `VisioForgeX.StopMainLoop()` sur
`beforeAssemblyReload` et `EditorApplication.quitting`. La boucle principale GLib tourne sur
un thread d'arrière-plan dédié, bloqué dans un appel natif qu'Unity ne peut pas abandonner —
sans ce guard, le rechargement de domaine suivant une recompilation de script se bloquerait.
Le guard **n'appelle pas** `DestroySDK` (voir ci-dessus) ; il arrête seulement le thread de
la boucle, et le prochain Play reconstruit la boucle. Ce guard est interne — vos scripts
doivent l'ignorer.

## Foire aux questions

### Dois-je appeler `Configure()` manuellement ?

Non. L'attribut `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` d'Unity l'exécute pour vous
avant le chargement de la première scène. Le seul cas où vous l'appelleriez à nouveau est
depuis un chemin de récupération personnalisé quand une tentative antérieure a échoué — et
`Configure()` est idempotente, donc un appel redondant est inoffensif.

### Pourquoi `Configure()` modifie-t-elle des variables d'environnement plutôt que de passer des arguments ?

GLib lit `GST_PLUGIN_PATH`, `GIO_MODULE_DIR`, `SSL_CERT_FILE`, `HOME`, etc. depuis `environ` C
directement pendant `gst_init` et à nouveau à la première utilisation TLS. Le SDK n'a pas
d'API pour les surcharger — la seule façon correcte de pointer le runtime vers les assets
fournis est de définir les variables avant la construction de tout pipeline. Les mutations
sont limitées au processus ; les environnements utilisateur et système restent intacts.

### Que se passe-t-il si j'appelle `InitializeSdk()` avant que `Configure()` ait réussi ?

Cela journalise une erreur et retourne. Le drapeau de succès reste désactivé pour qu'une
tentative ultérieure puisse réussir une fois `Configure()` fonctionnel. Ce guard existe parce
que `InitSDK()` planterait sinon au fond du code natif avec une erreur bien moins actionnable.

### Puis-je exécuter deux pipelines en parallèle ?

Oui. `InitializeSdk()` démarre le SDK une fois par processus ; après ça vous pouvez construire
autant d'instances `MediaBlocksPipeline` — ou de moteurs `MediaPlayerCoreX` / `VideoCaptureCoreX` /
`VideoEditCoreX` — que vous voulez. Chacune est indépendante — le motif multi-caméra RTSP de
l'exemple consiste à attacher un `RTSPViewerPlayer` par `RawImage`, et chacun construit et démolit
son propre pipeline.

## Voir aussi

- [Utiliser VisioForge dans Unity](index.md) — vue d'ensemble du paquet et fonctionnement du rendu
- [Compilation pour Windows](windows.md) — réglages Éditeur et Standalone pour Windows
- [Compilation pour Android](android.md) — réglages IL2CPP pour Android
- [Compilation pour macOS](macos.md) — réglages Standalone pour macOS
- [Compilation pour iOS](ios.md) — réglages d'appareil pour iOS
- [Dépannage](troubleshooting.md) — erreurs courantes de bootstrap et runtime
