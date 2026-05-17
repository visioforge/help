---
title: Enregistrer un flux RTSP vers MP4 sans réencodage en C# .NET
description: Enregistrez les flux RTSP de caméras IP directement vers MP4 sans transcodage avec le VisioForge Media Blocks SDK. Capture passthrough avec exemples C#.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Recording
  - Encoding
  - Decoding
  - Conversion
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - RTSPRecorder
  - MediaBlocksPipeline
  - RTSPRAWSourceBlock
  - RTSPRAWSourceSettings
  - AACEncoderBlock

---

# Enregistrer un flux RTSP dans un fichier : capturer la vidéo de caméra IP sans réencodage

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!!info Exemple de démo
Pour un exemple complet et fonctionnel d'enregistrement de flux RTSP sans réencodage, consultez la [démo RTSP MultiView](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo).

Pour une documentation spécifique aux caméras ONVIF, consultez le [guide d'intégration ONVIF de caméra IP](../../videocapture/video-sources/ip-cameras/onvif.md).
!!!

## Table des matières

- [Enregistrer un flux RTSP dans un fichier : capturer la vidéo de caméra IP sans réencodage](#enregistrer-un-flux-rtsp-dans-un-fichier-capturer-la-video-de-camera-ip-sans-reencodage)
  - [Table des matières](#table-des-matieres)
  - [Vue d'ensemble](#vue-densemble)
  - [Fonctionnalités principales](#fonctionnalites-principales)
  - [Concept central](#concept-central)
  - [Prérequis](#prerequis)
  - [Exemple de code : classe RTSPRecorder](#exemple-de-code-classe-rtsprecorder)
  - [Explication du code](#explication-du-code)
  - [Comment utiliser le `RTSPRecorder`](#comment-utiliser-le-rtsprecorder)
  - [Considérations clés](#considerations-cles)
  - [Exemple GitHub complet](#exemple-github-complet)
  - [Bonnes pratiques](#bonnes-pratiques)
  - [Dépannage](#depannage)

## Vue d'ensemble

Ce guide montre comment enregistrer un flux RTSP dans un fichier MP4 en capturant le flux vidéo d'origine d'une caméra IP RTSP sans réencodage de la vidéo. Cette approche est très avantageuse pour préserver la qualité vidéo d'origine des caméras et minimiser la consommation CPU lorsque vous devez enregistrer des séquences. Le flux audio peut être transmis ou, en option, réencodé pour une meilleure compatibilité, permettant de sauvegarder toutes les données du streaming. Des outils comme FFmpeg et VLC offrent des méthodes en ligne de commande ou avec interface graphique pour enregistrer un flux RTSP ; toutefois, ce guide se concentre sur une approche programmatique avec le VisioForge Media Blocks SDK pour les développeurs .NET qui doivent créer des applications se connectant à des caméras RTSP et enregistrant leur vidéo.

## Fonctionnalités principales

- **Enregistrement direct du flux** : enregistrez les flux des caméras RTSP sans perte de qualité
- **Traitement efficace en CPU** : aucun réencodage vidéo requis
- **Gestion flexible de l'audio** : pass-through ou réencodage selon le besoin
- **Intégration professionnelle** : contrôle programmatique pour les applications d'entreprise
- **Hautes performances** : optimisé pour l'enregistrement continu

Nous utiliserons le VisioForge Media Blocks SDK, une puissante bibliothèque .NET pour construire des applications de traitement multimédia personnalisées, afin d'enregistrer efficacement un flux RTSP dans un fichier.

## Concept central

L'idée principale est de prendre le flux vidéo brut de la source RTSP et de l'envoyer directement à un puits de fichier (par ex. multiplexeur MP4) sans aucune étape de décodage ou d'encodage pour la vidéo. C'est une exigence courante pour enregistrer les flux RTSP avec une fidélité maximale.

- **Flux vidéo** : transmis directement de la source RTSP au puits MP4. Cela garantit que les données vidéo d'origine sont enregistrées, crucial pour les applications devant enregistrer des séquences haute qualité depuis les caméras.
- **Flux audio** : peut soit être transmis directement (si le codec audio d'origine est compatible avec le conteneur MP4) soit être réencodé (par ex. en AAC) pour garantir la compatibilité et potentiellement réduire la taille du fichier lors de la sauvegarde du flux RTSP.

## Prérequis

Vous aurez besoin du VisioForge Media Blocks SDK. Vous pouvez l'ajouter à votre projet .NET via NuGet :

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

Selon votre plateforme cible (Windows, macOS, Linux, y compris les systèmes ARM comme Jetson Nano pour les applications de caméras embarquées), vous aurez également besoin des paquets de runtime natifs correspondants. Par exemple, sous Windows pour enregistrer la vidéo :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

Pour des informations détaillées sur les exigences de déploiement et les dépendances spécifiques aux plateformes, consultez notre [guide de déploiement](../../deployment-x/index.md). Il est important de vérifier ces détails pour que votre application de capture de flux vidéo fonctionne correctement.

Reportez-vous au fichier `RTSP Capture Original.csproj` dans le projet exemple pour la liste complète des dépendances pour les différentes plateformes.

## Exemple de code : classe RTSPRecorder

Le code C# suivant définit une classe `RTSPRecorder` qui encapsule la logique de capture et d'enregistrement du flux RTSP.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

namespace RTSPCaptureOriginalStream
{
    /// <summary>
    /// La classe RTSPRecorder encapsule la fonctionnalité d'enregistrement RTSP pour sauvegarder un flux RTSP dans un fichier.
    /// Elle utilise le SDK MediaBlocks pour créer un pipeline qui connecte une 
    /// source RTSP (comme une caméra IP) à un puits MP4 (fichier).
    /// </summary>
    public class RTSPRecorder : IAsyncDisposable
    {
        /// <summary>
        /// Le pipeline MediaBlocks qui gère le flux de données multimédias.
        /// </summary>
        public MediaBlocksPipeline Pipeline { get; private set; }

        // Champs privés pour les composants MediaBlock
        private MediaBlock _muxer;               // Multiplexeur conteneur MP4 (puits)
        private RTSPRAWSourceBlock _rtspRawSource; // Source de flux RTSP (fournit les flux bruts)
        private DecodeBinBlock _decodeBin;       // Facultatif : décodeur audio (si réencodage audio)
        private AACEncoderBlock _audioEncoder;   // Facultatif : encodeur audio AAC (si réencodage audio)
        private bool disposedValue;              // Drapeau pour empêcher les libérations multiples

        /// <summary>
        /// Événement déclenché lorsqu'une erreur survient dans le pipeline.
        /// </summary>
        public event EventHandler<ErrorsEventArgs> OnError;

        /// <summary>
        /// Événement déclenché lorsqu'un message de statut est disponible.
        /// </summary>
        public event EventHandler<string> OnStatusMessage;

        /// <summary>
        /// Nom de fichier de sortie pour l'enregistrement MP4.
        /// </summary>
        public string Filename { get; set; } = "output.mp4";

        /// <summary>
        /// Indique s'il faut réencoder l'audio au format AAC (recommandé pour la compatibilité).
        /// Si false, l'audio est transmis tel quel.
        /// </summary>
        public bool ReencodeAudio { get; set; } = true;

        /// <summary>
        /// Démarre la session d'enregistrement en créant et configurant le pipeline MediaBlocks.
        /// </summary>
        /// <param name="rtspSettings">Paramètres de configuration de la source RTSP.</param>
        /// <returns>True si le pipeline a démarré avec succès, false sinon.</returns>
        public async Task<bool> StartAsync(RTSPRAWSourceSettings rtspSettings)
        {
            // Créer un nouveau pipeline MediaBlocks
            Pipeline = new MediaBlocksPipeline();
            Pipeline.OnError += (sender, e) => OnError?.Invoke(this, e); // Remonter les erreurs

            OnStatusMessage?.Invoke(this, "Creating pipeline to record RTSP stream...");

            // 1. Créer le bloc source RTSP.
            // RTSPRAWSourceBlock fournit des flux élémentaires bruts et non décodés (vidéo et audio) depuis votre caméra IP ou autres caméras RTSP.
            _rtspRawSource = new RTSPRAWSourceBlock(rtspSettings);
            
            // 2. Créer le bloc puits MP4 (multiplexeur).
            // Ce bloc écrira les flux multimédias dans un fichier MP4.
            _muxer = new MP4SinkBlock(new MP4SinkSettings(Filename));

            // 3. Connecter le flux vidéo (passthrough)
            // Créer un pad d'entrée dynamique sur le multiplexeur pour le flux vidéo.
            // Nous connectons la sortie vidéo brute de la source RTSP directement au puits MP4.
            // Cela garantit que la vidéo n'est pas réencodée lors de l'enregistrement du flux caméra.
            var inputVideoPad = (_muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video);
            Pipeline.Connect(_rtspRawSource.VideoOutput, inputVideoPad);
            OnStatusMessage?.Invoke(this, "Video stream connected (passthrough for original quality video).");

            // 4. Connecter le flux audio (réencodage facultatif)
            // Cette section gère la manière dont l'audio du flux RTSP est traité et sauvegardé dans le fichier.
            if (rtspSettings.AudioEnabled)
            {
                // Créer un pad d'entrée dynamique sur le multiplexeur pour le flux audio.
                var inputAudioPad = (_muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio);

                if (ReencodeAudio)
                {
                    // Si le réencodage audio est activé (par ex. vers AAC pour la compatibilité) :
                    OnStatusMessage?.Invoke(this, "Setting up audio re-encoding to AAC for the recording...");
                    
                    // Créer un bloc décodeur qui ne traite que l'audio.
                    // Nous devons décoder l'audio d'origine avant de le réencoder pour enregistrer le flux MP4 avec un audio compatible.
                    _decodeBin = new DecodeBinBlock(renderVideo: false, renderAudio: true, renderSubtitle: false) 
                    {
                         // On peut désactiver le convertisseur audio interne si l'on est sûr du format 
                         // ou si l'encodeur gère la conversion. Pour AAC, c'est généralement OK.
                         DisableAudioConverter = true 
                    };

                    // Créer un encodeur AAC avec les paramètres par défaut.
                    _audioEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings());

                    // Connecter le pipeline de traitement audio :
                    // sortie audio RTSP -> décodeur -> encodeur AAC -> entrée audio du puits MP4
                    Pipeline.Connect(_rtspRawSource.AudioOutput, _decodeBin.Input);
                    Pipeline.Connect(_decodeBin.AudioOutput, _audioEncoder.Input);
                    Pipeline.Connect(_audioEncoder.Output, inputAudioPad);
                    OnStatusMessage?.Invoke(this, "Audio stream connected (re-encoding to AAC for MP4 file).");
                }
                else
                {
                    // Si le réencodage audio est désactivé, connecter l'audio RTSP directement au multiplexeur.
                    // Remarque : cela peut poser problème si le format audio d'origine n'est pas 
                    // compatible avec le conteneur MP4 (par ex. G.711 PCMU/PCMA) lors de la sauvegarde du flux RTSP.
                    // Les formats compatibles courants incluent AAC. Vérifiez le format audio de votre caméra.
                    Pipeline.Connect(_rtspRawSource.AudioOutput, inputAudioPad);
                    OnStatusMessage?.Invoke(this, "Audio stream connected (passthrough). Warning: Compatibility depends on original camera audio format for the file.");
                }
            }

            // 5. Démarrer le pipeline pour enregistrer la vidéo
            OnStatusMessage?.Invoke(this, "Starting recording pipeline to save RTSP stream to file...");
            bool success = await Pipeline.StartAsync();
            if (success)
            {
                OnStatusMessage?.Invoke(this, "Recording pipeline started successfully.");
            }
            else
            {
                OnStatusMessage?.Invoke(this, "Failed to start recording pipeline.");
            }
            return success;
        }

        /// <summary>
        /// Arrête l'enregistrement en stoppant le pipeline MediaBlocks.
        /// </summary>
        /// <returns>True si le pipeline a été arrêté avec succès, false sinon.</returns>
        public async Task<bool> StopAsync()
        {
            if (Pipeline == null)
                return false;

            OnStatusMessage?.Invoke(this, "Stopping recording pipeline...");
            bool success = await Pipeline.StopAsync();
            if (success)
            {
                OnStatusMessage?.Invoke(this, "Recording pipeline stopped successfully.");
            }
            else
            {
                OnStatusMessage?.Invoke(this, "Failed to stop recording pipeline.");
            }
            
            // Détacher le gestionnaire d'erreurs pour éviter les problèmes si StopAsync est appelé plusieurs fois
            // ou avant DisposeAsync
            if (Pipeline != null)
            {
                 Pipeline.OnError -= OnError;
            }

            return success;
        }

        /// <summary>
        /// Libère de manière asynchrone le RTSPRecorder et toutes ses ressources.
        /// Implémente le motif IAsyncDisposable pour un nettoyage correct des ressources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                if (Pipeline != null)
                {
                    Pipeline.OnError -= (sender, e) => OnError?.Invoke(this, e); // Garantir le détachement
                    await Pipeline.DisposeAsync();
                    Pipeline = null;
                }

                // Libérer tous les composants MediaBlock
                // Utilisation de « as IDisposable » pour un cast et une libération sûrs.
                (_muxer as IDisposable)?.Dispose();
                _muxer = null;

                _rtspRawSource?.Dispose();
                _rtspRawSource = null;

                _decodeBin?.Dispose();
                _decodeBin = null;

                _audioEncoder?.Dispose();
                _audioEncoder = null;

                disposedValue = true;
            }
        }
    }
}
```

## Explication du code

1. **Classe `RTSPRecorder`** : cette classe est centrale pour aider un utilisateur à enregistrer un flux RTSP dans un fichier.
    - Implémente `IAsyncDisposable` pour une bonne gestion des ressources.
    - `Pipeline` : l'objet `MediaBlocksPipeline` qui orchestre le flux multimédia.
    - `_rtspRawSource` : un `RTSPRAWSourceBlock` est utilisé. Le « RAW » est essentiel ici, car il fournit les flux élémentaires (vidéo et audio) depuis la caméra sans tenter de les décoder initialement.
    - `_muxer` : un `MP4SinkBlock` sert à écrire les flux vidéo et audio entrants dans un fichier MP4.
    - `_decodeBin` et `_audioEncoder` : ces blocs facultatifs ne sont utilisés que si `ReencodeAudio` est true. `_decodeBin` décode l'audio d'origine de la caméra IP, et `_audioEncoder` (par ex. `AACEncoderBlock`) le réencode dans un format plus compatible comme AAC.
    - `Filename` : spécifie le chemin du fichier MP4 de sortie où la vidéo sera enregistrée.
    - `ReencodeAudio` : propriété booléenne pour contrôler le traitement audio. Si `true`, l'audio est réencodé en AAC. Si `false`, l'audio est transmis directement. Vérifiez le format audio de votre caméra pour la compatibilité si false.

2. **Méthode `StartAsync(RTSPRAWSourceSettings rtspSettings)`** : cette méthode initie le processus pour **enregistrer le flux RTSP**.
    - Initialise `MediaBlocksPipeline`.
    - **Source RTSP** : crée `_rtspRawSource` avec `RTSPRAWSourceSettings`. Ces paramètres incluent l'URL (le chemin du flux de votre caméra), les identifiants d'accès et les paramètres de capture audio.
    - **Puits MP4** : crée `_muxer` (puits MP4) avec le nom de fichier cible.
    - **Chemin vidéo (passthrough)** :
        - Un nouveau pad d'entrée dynamique pour la vidéo est créé sur `_muxer`.
        - `Pipeline.Connect(_rtspRawSource.VideoOutput, inputVideoPad);` Cette ligne connecte directement la sortie vidéo brute de la source RTSP à l'entrée vidéo du multiplexeur MP4. Aucun réencodage n'a lieu pour le flux vidéo.
    - **Chemin audio (conditionnel)** : détermine comment l'audio de la **caméra** est traité lors de la **sauvegarde dans le fichier**.
        - Si `rtspSettings.AudioEnabled` est true :
            - Un nouveau pad d'entrée dynamique pour l'audio est créé sur `_muxer`.
            - Si `ReencodeAudio` est `true` (recommandé pour une compatibilité plus large du fichier) :
                - `_decodeBin` est créé pour décoder l'audio entrant de la caméra. Il est configuré pour ne traiter que l'audio (`audioDisabled: false`).
                - `_audioEncoder` (par ex. `AACEncoderBlock`) est créé.
                - Le pipeline est connecté : `_rtspRawSource.AudioOutput` -> `_decodeBin.Input` -> `_decodeBin.AudioOutput` -> `_audioEncoder.Input` -> `_audioEncoder.Output` -> `inputAudioPad` (entrée audio du multiplexeur).
            - Si `ReencodeAudio` est `false` :
                - `Pipeline.Connect(_rtspRawSource.AudioOutput, inputAudioPad);` La sortie audio brute de la source caméra est connectée directement au multiplexeur MP4. *Attention* : cela repose sur la compatibilité du codec audio d'origine de la caméra avec le conteneur MP4 (par ex. AAC). Les formats comme G.711 (PCMU/PCMA) sont courants dans les caméras RTSP mais ne sont pas standard dans MP4 et peuvent entraîner des problèmes de lecture ou nécessiter des lecteurs spécialisés si vous sauvegardez ainsi. Consultez la documentation de votre caméra.
    - Démarre le pipeline via `Pipeline.StartAsync()` pour commencer le processus d'enregistrement vidéo en streaming.

3. **Méthode `StopAsync()`** : arrête le `Pipeline`.

4. **Méthode `DisposeAsync()`** :
    - Nettoie toutes les ressources, y compris le pipeline et les blocs multimédias individuels.

## Comment utiliser le `RTSPRecorder`

Voici un exemple basique de la manière dont vous pourriez utiliser la classe `RTSPRecorder` :

```csharp
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VisioForge.Core; // Pour VisioForgeX.DestroySDK()
using VisioForge.Core.Types.X.Sources; // Pour RTSPRAWSourceSettings
using RTSPCaptureOriginalStream; // Espace de noms de votre classe RTSPRecorder

class Demo
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("RTSP Camera to MP4 Capture (Original Video Stream)");
        Console.WriteLine("-------------------------------------------------");

        string rtspUrl = "rtsp://your_camera_ip:554/stream_path"; // Remplacer par votre URL RTSP
        string username = "admin"; // Remplacer par votre nom d'utilisateur, ou vide si aucun
        string password = "password"; // Remplacer par votre mot de passe, ou vide si aucun
        string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "rtsp_original_capture.mp4");

        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

        Console.WriteLine($"Capturing from: {rtspUrl}");
        Console.WriteLine($"Saving to: {outputFilePath}");
        Console.WriteLine("Press any key to stop recording...");

        var cts = new CancellationTokenSource();
        RTSPRecorder recorder = null;

        try
        {
            recorder = new RTSPRecorder
            {
                Filename = outputFilePath,
                ReencodeAudio = true // Définir à false pour transmettre l'audio (vérifier la compatibilité)
            };

            recorder.OnError += (s, e) => Console.WriteLine($"ERROR: {e.Message}");
            recorder.OnStatusMessage += (s, msg) => Console.WriteLine($"STATUS: {msg}");

            // Configurer les paramètres de la source RTSP. Le ctor est privé — utilisez la fabrique async.
            var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(
                new Uri(rtspUrl), username, password, audioEnabled: true);
            // Ajustez d'autres paramètres au besoin, par ex. limiter le transport à TCP uniquement :
            // rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;

            if (await recorder.StartAsync(rtspSettings))
            {
                Console.ReadKey(true); // Attendre une touche pour arrêter
            }
            else
            {
                Console.WriteLine("Failed to start recording. Check status messages and RTSP URL/credentials.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            if (recorder != null)
            {
                Console.WriteLine("Stopping recording...");
                await recorder.StopAsync();
                await recorder.DisposeAsync();
                Console.WriteLine("Recording stopped and resources disposed.");
            }

            // Important : nettoyer les ressources du SDK VisioForge à la sortie de l'application
            VisioForgeX.DestroySDK(); 
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);
    }
}
```

## Considérations clés

- **Compatibilité audio (passthrough)** : si vous choisissez `ReencodeAudio = false`, assurez-vous que le codec audio de la caméra (par ex. AAC, MP3) est compatible avec le conteneur MP4. Les codecs audio RTSP courants comme G.711 (PCMU/PCMA) ne sont généralement pas directement pris en charge dans les fichiers MP4 et entraîneront probablement un audio silencieux ou des erreurs de lecture. Le réencodage en AAC est généralement plus sûr pour une compatibilité plus large.
- **Conditions réseau** : le streaming RTSP est sensible à la stabilité du réseau, assurez-vous donc d'une connexion réseau fiable vers la caméra.
- **Gestion des erreurs** : les applications robustes doivent implémenter une gestion d'erreurs minutieuse en s'abonnant à l'événement `OnError` du `RTSPRecorder` (ou directement du `MediaBlocksPipeline`).
- **Gestion des ressources** : appelez toujours `DisposeAsync` sur l'instance `RTSPRecorder` (et donc le `MediaBlocksPipeline`) une fois terminé pour libérer les ressources. `VisioForgeX.DestroySDK()` doit être appelé une fois à la fermeture de l'application.

## Exemple GitHub complet

Pour une application console complète et exécutable illustrant ces concepts, y compris la saisie utilisateur pour les détails RTSP et l'affichage dynamique de la durée, consultez le dépôt officiel des exemples VisioForge :

- **[Exemple RTSP Capture Original Stream sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/RTSP%20Capture%20Original)**

Cet exemple fournit un exemple plus complet et présente des fonctionnalités supplémentaires.

## Bonnes pratiques

- Implémentez toujours une gestion d'erreurs correcte
- Surveillez la stabilité réseau pour un streaming fiable
- Utilisez des paramètres d'encodage audio appropriés
- Gérez efficacement les ressources système
- Mettez en place des procédures de nettoyage correctes

## Dépannage

Problèmes courants et leurs solutions lors de l'enregistrement de flux RTSP :

- Problèmes de connectivité réseau
- Compatibilité des codecs audio
- Gestion des ressources
- Erreurs d'initialisation du flux
- Considérations de stockage des enregistrements

## Voir aussi

- [Enregistrement pré-événement avec caméra IP](pre-event-recording.md) — mettez en tampon la vidéo RTSP et enregistrez sur déclenchement par détection de mouvement
- [Diffusion vidéo RTSP](../../general/network-streaming/rtsp.md) — fondamentaux du streaming RTSP et configuration du serveur
- [Reconnexion RTSP et solution de repli](../../general/network-sources/reconnection-and-fallback.md) — maintenez les enregistrements à travers les coupures de caméra avec `FallbackSwitch` automatique

---
Ce guide fournit une compréhension fondamentale de la manière de sauvegarder la vidéo d'origine d'un flux RTSP tout en gérant de manière flexible le flux audio avec le VisioForge Media Blocks SDK. En tirant parti du `RTSPRAWSourceBlock` et des connexions directes du pipeline, vous pouvez obtenir des enregistrements efficaces et de haute qualité.
