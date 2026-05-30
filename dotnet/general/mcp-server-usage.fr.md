---
title: Serveur MCP VisioForge pour le développement assisté par IA
description: Connectez votre assistant IA au serveur MCP VisioForge pour un accès instantané à l'API, aux guides de déploiement et aux exemples du SDK.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Conversion
  - IP Camera
  - RTSP
  - C#
  - NuGet
primary_api_classes:
  - MediaBlocksPipeline
  - VideoRendererBlock
  - RTSPSourceBlock

---

# Utilisation du serveur MCP VisioForge pour le développement assisté par IA

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au serveur MCP VisioForge

Le serveur MCP (Model Context Protocol) VisioForge fournit aux assistants de codage propulsés par IA un accès direct à la documentation complète du SDK VisioForge, aux guides de déploiement, aux exemples de code et aux références d'API. Cela permet à votre assistant IA de fournir une aide précise et contextuelle pendant que vous développez avec les SDK VisioForge.

### Qu'est-ce que Model Context Protocol (MCP) ?

Model Context Protocol (MCP) est une norme ouverte développée par Anthropic qui permet aux assistants IA de se connecter de manière sécurisée à des sources de connaissances et à des outils externes. Considérez-le comme un pont entre votre assistant de codage IA (comme Claude Code, GitHub Copilot ou les extensions VS Code) et des serveurs de documentation spécialisés.

Avec MCP, votre assistant IA peut :

- Interroger la documentation API en temps réel
- Récupérer les guides de déploiement pour des plateformes spécifiques
- Récupérer des exemples et extraits de code
- Effectuer des recherches dans la documentation du SDK
- Obtenir les détails de configuration propres à une plateforme

## Pourquoi utiliser le serveur MCP VisioForge ?

Lorsque vous développez avec les SDK VisioForge, le serveur MCP offre plusieurs avantages clés :

### 1. **Accès instantané à la documentation de l'API**

Votre assistant IA peut interroger l'intégralité de l'API du SDK VisioForge, incluant :

- Toutes les classes, méthodes, propriétés et événements
- Descriptions détaillées et notes d'utilisation
- Types de paramètres et valeurs de retour
- Exemples et extraits de code
- Références croisées vers des API associées

### 2. **Orientation de déploiement propre à la plateforme**

Obtenez des instructions de déploiement précises pour :

- **Bureau** : Windows, Linux, macOS
- **Mobile** : Android, iOS, Mac Catalyst
- **Frameworks** : MAUI, Uno, Avalonia, WPF, WinForms, Blazor, Console
- **Scénarios** : enregistrement RTSP, transcodage cloud, streaming HLS

### 3. **Références NuGet correctes**

Le serveur MCP génère des extraits `.csproj` prêts à coller avec :

- Paquets NuGet propres à la plateforme
- Numéros de version corrects
- Références conditionnelles aux paquets
- Références de projet requises (comme AndroidDependency)

### 4. **Configuration de build propre à la plateforme**

Récupérez des cibles MSBuild et des extraits de configuration pour :

- Copie de bibliothèques natives Mac Catalyst
- Autorisations Android (manifest + runtime)
- Autorisations Info.plist iOS
- Paramètres de build propres à la plateforme

## Prérequis

Avant de vous connecter au serveur MCP VisioForge, assurez-vous d'avoir :

- **Un assistant IA compatible MCP** :
  - [Claude Code](https://claude.ai/code) (recommandé)
  - VS Code avec l'extension MCP
  - GitHub Copilot avec prise en charge MCP
  - Autres outils compatibles MCP

- **Une connexion internet** pour accéder à `https://mcp.visioforge.com`

## Connexion au serveur MCP

### Claude Code (recommandé)

Claude Code intègre nativement la prise en charge MCP. Connectez-vous avec une seule commande :

```bash
claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp
```

**Vérifier la connexion :**

```bash
claude mcp list
```

Vous devriez voir `visioforge-sdk` dans la liste des serveurs connectés.

### VS Code avec extension MCP

Ajoutez le serveur MCP VisioForge à votre espace de travail ou à vos paramètres utilisateur :

1. Ouvrez VS Code
2. Installez l'extension MCP (si elle n'est pas déjà installée)
3. Créez ou modifiez `.vscode/mcp.json` dans votre projet :

```json
{
  "servers": {
    "visioforge-sdk": {
      "type": "http",
      "url": "https://mcp.visioforge.com/mcp"
    }
  }
}
```

### Configuration au niveau du projet (n'importe quel client MCP)

Pour une configuration MCP propre au projet, créez `.mcp.json` à la racine de votre dépôt :

```json
{
  "servers": {
    "visioforge-sdk": {
      "type": "http",
      "url": "https://mcp.visioforge.com/mcp",
      "description": "VisioForge SDK documentation and deployment guides"
    }
  }
}
```

## Outils MCP disponibles

Le serveur MCP VisioForge expose 14 outils spécialisés que votre assistant IA peut utiliser. Les noms et descriptions correspondent exactement à la réponse `tools/list` en direct depuis `https://mcp.visioforge.com/mcp`.

### 1. **Outils Media Blocks**

#### `list_media_blocks`
Liste les blocs multimédias VisioForge disponibles, éventuellement filtrés par catégorie. Les blocs multimédias sont les briques de construction des pipelines de traitement multimédia. Les catégories incluent : Sources, Sinks, VideoEncoders, AudioEncoders, VideoDecoders, VideoProcessing, AudioProcessing, AudioRendering, VideoRendering, Demuxers, Parsers, OpenGL, OpenCV, Nvidia, Decklink, AWS, RTSPServer, Bridge, Special, Outputs.

**Exemples de requêtes :**
- « Lister tous les blocs encodeurs vidéo »
- « Montrer les sources MediaBlocks »
- « Quels blocs se trouvent dans la catégorie OpenCV ? »

#### `get_media_block_info`
Obtient des informations détaillées sur un bloc multimédia spécifique, notamment ses propriétés, méthodes, événements, pads d'entrée/sortie, paramètres de constructeur et documentation. Utilisez-le pour comprendre comment configurer et utiliser un bloc multimédia spécifique dans un pipeline.

**Exemples de requêtes :**
- « Obtenir des informations sur RTSPSourceBlock »
- « Montrer les pads et propriétés de H264EncoderBlock »
- « Quels paramètres de constructeur prend VideoRendererBlock ? »

#### `get_pipeline_template`
Obtient un modèle de pipeline de blocs multimédias pour un cas d'usage spécifique. Retourne la liste des blocs requis et la manière de les connecter, accompagnée du code C# pour construire le pipeline.

**Exemples de requêtes :**
- « Modèle de pipeline pour l'enregistrement RTSP vers MP4 »
- « Modèle pour la capture d'écran avec audio »
- « Pipeline pour le streaming HLS »

### 2. **Outils de classes et d'API du SDK**

#### `list_sdk_classes`
Liste les classes principales du SDK VisioForge. Ce sont les classes principales servant de points d'entrée pour la création d'applications multimédia : VideoCaptureCoreX (capture/enregistrement vidéo), VideoEditCoreX (édition vidéo), MediaPlayerCoreX (lecture multimédia), MediaInfoReaderCoreX (analyse multimédia), SimplePlayerCoreX (lecture simple), et plus encore.

**Exemples de requêtes :**
- « Lister toutes les classes principales du SDK »
- « Montrer les classes de point d'entrée de premier niveau »

#### `get_class_info`
Obtient des informations détaillées sur n'importe quelle classe du SDK VisioForge, incluant la liste complète de ses propriétés, méthodes, événements, constructeurs, classe de base, interfaces et documentation. Fonctionne à la fois pour les classes principales du SDK et les blocs multimédias.

**Exemples de requêtes :**
- « Montrer la documentation de la classe MediaBlocksPipeline »
- « Obtenir des détails sur VideoCaptureCoreX »
- « Quels événements expose MediaPlayerCoreX ? »

#### `get_method_signature`
Obtient la signature détaillée et la documentation pour une méthode spécifique d'une classe. Utile lorsque vous avez besoin de comprendre les paramètres, le type de retour et le comportement d'une méthode.

**Exemples de requêtes :**
- « Signature de StartAsync sur MediaBlocksPipeline »
- « Quels paramètres prend Connect ? »

#### `search_api`
Recherche dans toute l'API du SDK VisioForge — noms de classes, noms de méthodes, noms de propriétés, noms d'événements et leur documentation. Retourne des résultats classés. Utilisez ceci lorsque vous ne connaissez pas le nom exact de la classe, ou pour trouver toutes les classes liées à un concept (par exemple, « RTSP streaming », « video overlay », « audio capture »).

**Exemples de requêtes :**
- « Rechercher les classes de capture vidéo »
- « Trouver les méthodes liées au streaming RTSP »
- « Montrer tous les encodeurs audio MediaBlocks »

#### `get_enum_values`
Obtient toutes les valeurs d'un type d'énumération du SDK VisioForge avec leur description. Utile pour comprendre les options disponibles pour les propriétés de configuration (par exemple, MediaBlockType, codecs vidéo, formats audio, formats de pixels).

**Exemples de requêtes :**
- « Lister les valeurs de l'enum VideoCodec »
- « Montrer les valeurs de l'enum MediaBlockType »

#### `list_namespaces`
Parcourez hiérarchiquement les espaces de noms du SDK VisioForge. Affiche les espaces de noms enfants et les classes au sein d'un espace de noms donné. Commencez avec `VisioForge.Core` ou laissez vide pour voir les espaces de noms de premier niveau.

**Exemples de requêtes :**
- « Lister les espaces de noms de premier niveau »
- « Montrer les classes dans VisioForge.Core.MediaBlocks »

#### `get_code_example`
Obtient un exemple de code pour un scénario courant du SDK VisioForge. Retourne des extraits de code C# complets et fonctionnels qui démontrent comment utiliser le SDK pour des tâches telles que la capture vidéo, le streaming RTSP, la lecture multimédia, et plus encore.

**Exemples de requêtes :**
- « Exemple de code pour la capture de caméra RTSP »
- « Montrer un extrait d'enregistrement MP4 »
- « Exemple d'application d'effets vidéo »

### 3. **Outils des guides de déploiement**

#### `list_deployment_guides`
Liste les guides de déploiement disponibles pour le SDK VisioForge. Filtrez par plateforme, type de projet, type de SDK ou scénario. Retourne une liste de guides avec titres, résumés et étiquettes.

**Exemples de requêtes :**
- « Lister les guides de déploiement Android »
- « Montrer les guides de déploiement MAUI »
- « Trouver les guides de déploiement pour Linux »

#### `get_deployment_guide`
Obtient le guide de déploiement complet pour un scénario spécifique. Retourne des instructions détaillées, des extraits de code, les paquets NuGet et des notes propres à la plateforme.

**Exemples de requêtes :**
- « Obtenir le guide de déploiement Android »
- « Montrer les étapes de déploiement pour la plateforme Uno »
- « Comment déployer sur macOS »

#### `get_nuget_packages_snippet`
Obtient un extrait `.csproj` contenant les paquets NuGet requis pour un scénario de déploiement spécifique. Retourne un extrait XML prêt à copier-coller dans votre fichier de projet.

**Exemples de requêtes :**
- « Générer les paquets NuGet pour un projet MAUI Android »
- « Obtenir les références de paquets pour Avalonia sur Windows »
- « Montrer les paquets requis pour iOS »

#### `get_platform_specific_config`
Obtient le code de configuration de copie de fichiers / de build propre à la plateforme. Retourne des cibles MSBuild ou des scripts post-build pour des exigences de déploiement particulières.

**Exemples de requêtes :**
- « Montrer la cible de copie de fichiers Mac Catalyst »
- « Obtenir les permissions du manifest Android »
- « Clés de permissions Info.plist iOS »

## Exemples d'utilisation

### Exemple 1 : configuration d'un projet Android MAUI

**Vous demandez à votre assistant IA :**
> "Je crée une application de capture vidéo avec MAUI pour Android. Quels paquets NuGet me faut-il ?"

**Votre assistant IA utilise le serveur MCP pour :**
1. Appeler `get_nuget_packages_snippet` avec `platform: Android, projectType: MAUI, sdkType: MediaBlocks`
2. Récupérer les références de paquets correctes
3. Vous fournir un XML prêt à coller :

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.2.4" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />
  <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X9.csproj" />
</ItemGroup>
```

### Exemple 2 : trouver comment utiliser le streaming RTSP

**Vous demandez à votre assistant IA :**
> "Montre-moi comment capturer depuis une caméra RTSP avec le Media Blocks SDK"

**Votre assistant IA utilise le serveur MCP pour :**
1. Appeler `search_api` avec la requête "RTSP camera capture"
2. Identifier la classe `RTSPSourceBlock`
3. Appeler `get_code_example` pour les scénarios RTSP
4. Vous fournir du code fonctionnel :

```csharp
var pipeline = new MediaBlocksPipeline();

// RTSPSourceBlock prend RTSPSourceSettings (pas une Uri directement).
// Construire les paramètres via la fabrique asynchrone — le constructeur est privé.
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://camera.example.com:554/stream"),
    login: null,
    password: null,
    audioEnabled: false);

var rtspSource = new RTSPSourceBlock(rtspSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
await pipeline.StartAsync();
```

### Exemple 3 : déploiement Mac Catalyst

**Vous demandez à votre assistant IA :**
> "Comment déployer mon application Uno vers Mac Catalyst ?"

**Votre assistant IA utilise le serveur MCP pour :**
1. Appeler `get_deployment_guide` avec `guideId: "uno-platform"`
2. Extraire la section Mac Catalyst
3. Appeler `get_platform_specific_config` avec `platform: "maccatalyst"`
4. Vous fournir :
   - Les commandes de build
   - La cible MSBuild pour la copie de fichiers
   - Les étapes de déploiement

### Exemple 4 : comprendre une API spécifique

**Vous demandez à votre assistant IA :**
> "Quels paramètres accepte UniversalSourceBlock ?"

**Votre assistant IA utilise le serveur MCP pour :**
1. Appeler `search_api` avec la requête "UniversalSourceBlock"
2. Trouver la classe dans les résultats
3. Appeler `get_class_info` avec le nom de la classe
4. Analyser la documentation et expliquer :
   - Les paramètres du constructeur
   - Les formats de fichier pris en charge
   - Les options de configuration
   - Les exemples d'utilisation

## Bonnes pratiques

### 1. **Soyez précis dans vos questions**

Au lieu de questions génériques, fournissez du contexte :

- ❌ « Comment capturer de la vidéo ? »
- ✅ « Comment capturer de la vidéo depuis une caméra RTSP avec le SDK MediaBlocks sur Android ? »

### 2. **Précisez votre plateforme et votre framework**

Mentionnez toujours votre plateforme cible et votre framework UI :

- « J'utilise MAUI sur iOS… »
- « Mon application Avalonia cible Windows et Linux… »
- « Pour mon application Uno Platform sur Android… »

### 3. **Renseignez-vous tôt sur le déploiement**

Avant de plonger dans le code, renseignez-vous sur les exigences de déploiement :

- « De quels paquets NuGet ai-je besoin pour Mac Catalyst ? »
- « Montre-moi le guide de déploiement pour Avalonia sur Linux »
- « Quelles permissions sont requises pour l'accès à la caméra sur iOS ? »

### 4. **Demandez des exemples de code**

N'hésitez pas à demander du code fonctionnel :

- « Montre-moi un exemple complet de… »
- « Génère du code pour… »
- « Exemple d'implémentation de… »

## Dépannage

### Problèmes de connexion

Si votre assistant IA ne parvient pas à se connecter au serveur MCP :

1. **Vérifiez votre connexion internet** — Le serveur MCP est hébergé sur `https://mcp.visioforge.com`
2. **Vérifiez l'URL** — Assurez-vous d'utiliser le bon point de terminaison : `https://mcp.visioforge.com/mcp`
3. **Redémarrez votre assistant IA** — Parfois, un redémarrage résout les problèmes de connexion
4. **Vérifiez les journaux du client MCP** — Recherchez les erreurs de connexion dans les journaux de votre client

### Informations incorrectes ou obsolètes

Le serveur MCP est mis à jour régulièrement, mais si vous remarquez des informations incorrectes :

1. **Vérifiez la version du SDK** — Assurez-vous d'utiliser la dernière version du SDK
2. **Vérifiez les versions des paquets** — Comparez avec [NuGet.org](https://www.nuget.org/packages?q=VisioForge)
3. **Signalez les problèmes** — Contactez notre équipe d'assistance (voir Ressources supplémentaires ci-dessous)

### L'assistant IA n'utilise pas le serveur MCP

Si votre assistant IA ne semble pas utiliser le serveur MCP :

1. **Mentionnez-le explicitement** — Dites « Utilise le serveur MCP VisioForge pour trouver… »
2. **Vérifiez la connexion** — Exécutez `claude mcp list` ou vérifiez votre configuration MCP
3. **Redémarrez la session** — Démarrez une nouvelle conversation avec votre assistant IA

## Sécurité et confidentialité

### Transmission des données

- Toute communication avec le serveur MCP utilise un **chiffrement HTTPS**
- Le serveur est en lecture seule — il ne fournit que de la documentation, aucune collecte de données
- Aucune information personnelle ni code n'est envoyé au serveur
- Les requêtes API sont traitées en temps réel et ne sont pas stockées

### Authentification

- Le serveur MCP VisioForge est **publiquement accessible** — aucune authentification requise
- Votre assistant IA se connecte directement à `https://mcp.visioforge.com/mcp`
- Aucune clé d'API ni identifiants nécessaires

## Détails techniques

### Point de terminaison du serveur MCP

```
https://mcp.visioforge.com/mcp
```

### Capacités du serveur

- **Protocole** : MCP (Model Context Protocol)
- **Transport** : HTTP/HTTPS
- **Outils** : 14 outils spécialisés de documentation et de déploiement
- **Couverture de l'API** : API complète du SDK VisioForge .NET (toutes les classes, méthodes, propriétés)
- **Guides de déploiement** : plus de 15 guides par plateforme et type de projet
- **Exemples de code** : des centaines d'extraits de code fonctionnels
- **Fréquence de mise à jour** : mis à jour à chaque version du SDK

### Architecture du serveur

Le serveur MCP propose :

- **Haute disponibilité** : 99,9 % de disponibilité
- **Temps de réponse rapides** : moins de 200 ms en moyenne
- **Chiffrement SSL/TLS** : tout le trafic chiffré
- **Mises à jour automatiques** : synchronisées avec les versions du SDK
- **Limitation de débit** : politique d'usage équitable (pas de limites strictes pour les développeurs)

## Ressources supplémentaires

### Documentation

- [Documentation du SDK VisioForge](https://www.visioforge.com/help/)
- [Spécification du protocole MCP](https://modelcontextprotocol.io/specification/2025-11-25)
- [Documentation Claude Code](https://claude.ai/code)

### Assistance et communauté

Besoin d'aide ? Prenez contact :

- **[Portail d'assistance](https://support.visioforge.com/)** — Assistance technique et signalement de problèmes
- **[Communauté Discord](https://discord.com/invite/yvXUG56WCH)** — Échangez avec les développeurs et obtenez des réponses rapides
- **[Exemples GitHub](https://github.com/visioforge/.Net-SDK-s-samples)** — Projets d'exemple complets
- **E-mail :** <support@visioforge.com>

### Guides associés

- [Guide d'installation](../install/index.md)
- [Configuration requise](../system-requirements.md)
- [Guides de déploiement](../deployment-x/index.md)
- [Référence de l'API](https://api.visioforge.org/dotnet/api/index.html)

## Conclusion

Le serveur MCP VisioForge transforme le développement assisté par IA en fournissant à votre assistant de codage un accès direct à une documentation complète et à jour du SDK. Que vous construisiez une application de capture vidéo sur Android, un lecteur multimédia sur Windows ou un outil d'édition multiplateforme avec Avalonia, le serveur MCP garantit que votre assistant IA dispose des connaissances nécessaires pour vous aider à réussir.

Connectez-vous dès aujourd'hui et découvrez l'avenir du développement SDK propulsé par l'IA !
