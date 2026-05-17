---
title: Serveur HTTP MJPEG en C# .NET — diffusion vidéo en direct
description: Diffusez la vidéo en HTTP MJPEG depuis webcams, caméras IP ou fichiers en C# .NET. Points de terminaison navigateur, clients concurrents, JPEG GPU.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Editing
  - MJPEG
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - HTTPMJPEGLiveOutput
  - HTTPMJPEGLiveSinkBlock
  - IVideoCaptureXBaseOutput
  - IMediaBlockSink

---

# Diffusion HTTP MJPEG

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    Le moteur `VideoCaptureCoreX` et le Media Blocks SDK fonctionnent sur **Windows, macOS, Linux, Android et iOS** via GStreamer. Consultez la [matrice de prise en charge des plateformes](../../platform-matrix.md) pour les détails des codecs et de l'accélération matérielle, et le [guide de déploiement Linux](../../deployment-x/Ubuntu.md) pour la configuration sur Ubuntu / NVIDIA Jetson / Raspberry Pi.

La fonctionnalité du SDK qui consiste à diffuser de la vidéo encodée en Motion JPEG (MJPEG) sur HTTP est avantageuse par sa simplicité et sa large compatibilité. MJPEG encode chaque image vidéo individuellement comme une image JPEG, ce qui simplifie le décodage et le rend idéal pour des applications comme le streaming web et la vidéosurveillance. L'utilisation de HTTP garantit une intégration aisée et une grande compatibilité entre différentes plateformes et appareils, et reste efficace même sur des réseaux aux configurations strictes. Cette méthode convient particulièrement aux flux vidéo en temps réel et aux applications nécessitant une analyse image par image directe. Avec des fréquences d'images et résolutions ajustables, le SDK offre la flexibilité nécessaire pour diverses conditions réseau et exigences de qualité, ce qui en fait un choix polyvalent pour les développeurs implémentant le streaming vidéo dans leurs applications.

!!! tip "Agents de code IA : utilisez le serveur MCP VisioForge"

    Vous développez avec **Claude Code**, **Cursor** ou un autre agent de code IA ?
    Connectez-vous au [serveur MCP VisioForge](../mcp-server-usage.md) public
    à `https://mcp.visioforge.com/mcp` pour des recherches API structurées, des
    exemples de code exécutables et des guides de déploiement — plus précis que de
    parcourir `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Sortie MJPEG multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La fonctionnalité de diffusion est implémentée via deux classes principales :

1. `HTTPMJPEGLiveOutput` : classe de configuration de haut niveau qui définit la sortie de diffusion
2. `HTTPMJPEGLiveSinkBlock` : bloc d'implémentation sous-jacent qui gère le processus de diffusion proprement dit

### Classe HTTPMJPEGLiveOutput

Cette classe sert de point d'entrée de configuration pour mettre en place un flux MJPEG HTTP. Elle implémente l'interface `IVideoCaptureXBaseOutput`, ce qui la rend compatible avec le système de pipeline de capture vidéo.

#### Propriétés clés

- `Port` : retourne le numéro de port réseau sur lequel le flux MJPEG sera servi

#### Utilisation

```csharp
// Créer une nouvelle sortie de diffusion MJPEG sur le port 8080
var mjpegOutput = new HTTPMJPEGLiveOutput(8080);

// Ajouter la sortie MJPEG au moteur VideoCaptureCoreX
core.Outputs_Add(mjpegOutput, true);
```

#### Détails d'implémentation

- La classe est conçue pour être immuable, le port étant défini uniquement via le constructeur
- Elle ne prend pas en charge les encodeurs vidéo ou audio, car MJPEG utilise un encodage JPEG direct
- Les méthodes liées aux noms de fichiers renvoient null ou sont des no-ops, car il s'agit d'une implémentation exclusivement de diffusion

### Classe HTTPMJPEGLiveSinkBlock

Cette classe gère l'implémentation effective de la fonctionnalité de diffusion MJPEG. Elle est chargée de :

- Configurer le pipeline pour le traitement vidéo
- Gérer le serveur HTTP pour la diffusion
- Traiter les données vidéo en entrée et les convertir au format MJPEG
- Gérer les connexions clientes et la livraison du flux

#### Fonctionnalités clés

- Implémente plusieurs interfaces pour l'intégration au pipeline multimédia :
  - `IMediaBlockInternals` : pour l'intégration au pipeline
  - `IMediaBlockDynamicInputs` : pour la gestion des connexions d'entrée dynamiques
  - `IMediaBlockSink` : pour la fonctionnalité de puits
  - `IDisposable` : pour le nettoyage correct des ressources

#### Configuration entrée/sortie

- Accepte une seule entrée vidéo via le pad `Input`
- Aucun pad de sortie (puisqu'il s'agit d'un bloc puits)
- Pad d'entrée configuré pour le type de média vidéo uniquement

### Notes d'implémentation

#### Initialisation

```csharp
// Le bloc doit être initialisé avec un numéro de port
var mjpegSink = new HTTPMJPEGLiveSinkBlock(8080);
pipeline.Connect(videoSource.Output, mjpegSink.Input);

// "IMG tag URL is http://127.0.0.1:8090";
```

#### Gestion des ressources

- La classe implémente le nettoyage correct des ressources via le pattern `IDisposable`
- La méthode `CleanUp` garantit que toutes les ressources sont correctement libérées
- Les gestionnaires d'événements sont correctement connectés et déconnectés durant le cycle de vie du pipeline

#### Intégration au pipeline

La méthode `Build` gère le processus de configuration critique :

1. Crée l'élément puits HTTP MJPEG sous-jacent
2. Initialise le puits avec le port spécifié
3. Configure les connexions de pad GStreamer nécessaires
4. Connecte les gestionnaires d'événements du pipeline

### Gestion des erreurs

- L'implémentation comprend une vérification d'erreur complète pendant le processus de construction
- Les échecs d'initialisation sont correctement signalés via le système d'erreur du contexte
- Le nettoyage des ressources est géré même en cas d'erreur

### Considérations techniques

#### Performance

- L'implémentation utilise les éléments natifs de GStreamer pour des performances optimales
- Les connexions de pad directes minimisent les copies et la surcharge
- Le bloc puits est conçu pour gérer efficacement plusieurs connexions clientes

#### Gestion de la mémoire

- Les patterns de libération corrects garantissent l'absence de fuites mémoire
- Les ressources sont nettoyées à l'arrêt du pipeline ou à la libération du bloc
- L'implémentation gère correctement le cycle de vie des éléments GStreamer

#### Threading

- L'implémentation est thread-safe pour les opérations du pipeline
- Les gestionnaires d'événements sont correctement synchronisés avec les changements d'état du pipeline
- Les connexions clientes sont gérées de manière asynchrone

#### Utilisation côté client

Pour consommer le flux MJPEG :

1. Initialisez la sortie de diffusion avec le port souhaité
2. Connectez-la à votre pipeline vidéo
3. Accédez au flux via un navigateur web ou un client HTTP à :
   ```
   http://[server-address]:[port]
   ```

#### Exemple HTML client

```html
<img src="http://localhost:8080" />
```

### Limitations et considérations

1. Utilisation de la bande passante
   - Les flux MJPEG peuvent utiliser une bande passante importante puisque chaque image est un JPEG complet
   - Tenez compte de la fréquence d'images et de la résolution pour des performances optimales

2. Prise en charge par les navigateurs
   - Bien que MJPEG soit largement pris en charge, certains navigateurs modernes peuvent avoir des limitations
   - Les appareils mobiles peuvent gérer les flux MJPEG différemment

3. Latence
   - Bien que MJPEG offre une latence relativement faible, il n'est pas adapté aux exigences de latence ultra-faible
   - Les conditions réseau peuvent affecter le timing de livraison des images

### Bonnes pratiques

1. Choix du port
   - Choisissez des ports qui n'entrent pas en conflit avec d'autres services
   - Tenez compte des implications pour le pare-feu lors du choix des ports

2. Gestion des ressources
   - Libérez toujours correctement le bloc puits
   - Surveillez les connexions clientes et l'utilisation des ressources

3. Gestion des erreurs
   - Implémentez une gestion d'erreurs correcte pour les problèmes réseau et de pipeline
   - Surveillez l'état du pipeline pour détecter les problèmes potentiels

### Considérations de sécurité

1. Sécurité réseau
   - Le flux MJPEG n'est pas chiffré par défaut
   - Envisagez d'implémenter des mesures de sécurité supplémentaires pour les contenus sensibles

2. Contrôle d'accès
   - Aucun mécanisme d'authentification intégré
   - Envisagez d'implémenter un contrôle d'accès au niveau applicatif si nécessaire

3. Sécurité des ports
   - Assurez-vous que les règles de pare-feu appropriées sont en place
   - Envisagez l'isolation réseau pour les flux internes

## Sortie MJPEG Windows uniquement

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Définissez la propriété `Network_Streaming_Enabled` sur true pour activer la diffusion réseau.

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

Définissez la sortie HTTP MJPEG.

```cs
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.HTTP_MJPEG;
```

Créez l'objet de paramètres et définissez le port.

```cs
VideoCapture1.Network_Streaming_Output = new MJPEGOutput(8080);
```

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir davantage d'exemples de code.
