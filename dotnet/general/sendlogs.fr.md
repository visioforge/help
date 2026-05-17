---
title: Activer les journaux de débogage du SDK .NET VisioForge
description: Activez et capturez des journaux de débogage pour dépanner le SDK .NET avec des instructions pas à pas pour la démo et la production.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET

---

# Dépannage à l'aide des journaux pour les produits SDK .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Pourquoi les journaux sont importants pour le dépannage du SDK

Lorsque vous développez des applications qui utilisent des SDK multimédias, vous pouvez rencontrer des problèmes techniques qui nécessitent une enquête approfondie. Les journaux de débogage fournissent des informations critiques qui aident à identifier rapidement et efficacement la cause racine des problèmes. Ces journaux capturent tout, des séquences d'initialisation aux étapes détaillées des opérations, en passant par les conditions d'erreur et les informations système.

Des journaux correctement collectés offrent plusieurs avantages clés :

- **Résolution plus rapide des problèmes** : l'assistance technique peut identifier rapidement la source des problèmes
- **Contexte complet** : les journaux fournissent une vue d'ensemble de ce qui s'est passé avant, pendant et après un problème
- **Informations système** : les détails sur votre environnement aident à reproduire et à résoudre les problèmes
- **Aperçus de développement** : la compréhension des journaux peut vous aider à optimiser votre implémentation

## Collecte de journaux dans les applications de démonstration

Nos applications de démonstration incluent des capacités de débogage intégrées qui facilitent la collecte de journaux pour le dépannage. Suivez ces étapes pour activer et partager les journaux :

### Guide étape par étape pour la journalisation dans les applications de démonstration

1. **Lancez l'application de démonstration**
   - Ouvrez l'application de démonstration correspondant à votre SDK
   - Localisez l'interface principale où les paramètres peuvent être configurés

2. **Activez le mode de débogage**
   - Recherchez et cochez la case « Debug » dans l'interface de l'application
   - Cela active la journalisation détaillée de toutes les opérations du SDK

3. **Reproduisez le problème**
   - Configurez tous les autres paramètres requis pour votre scénario spécifique
   - Appuyez sur le bouton Start ou Play (selon le SDK que vous utilisez)
   - Laissez l'application fonctionner jusqu'à ce que le problème se produise
   - Après un temps suffisant pour capturer le problème, appuyez sur le bouton Stop

4. **Collectez les fichiers journaux**
   - Accédez à « Mes documents\VisioForge » sur votre système
   - Ce dossier contient tous les fichiers journaux générés
   - **Important** : excluez tout enregistrement audio/vidéo de votre collecte pour réduire la taille du fichier

5. **Partagez les journaux en toute sécurité**
   - Compressez les fichiers journaux dans une archive ZIP
   - Téléversez vers un service de partage de fichiers sécurisé comme Dropbox, Google Drive ou OneDrive
   - Partagez le lien d'accès avec l'assistance technique

## Implémenter la journalisation dans vos applications personnalisées

Lorsque vous développez vos propres applications avec nos SDK, vous devrez activer et configurer explicitement la journalisation. Cette section explique comment implémenter la journalisation avec différents composants du SDK.

### Activer les journaux de débogage dans votre code

Quel que soit le SDK que vous utilisez, l'approche de base pour activer les journaux suit un modèle similaire :

```csharp
// Exemple pour MediaPlayer SDK
mediaPlayer.Debug_Mode = true;
mediaPlayer.Debug_Dir = "C:\\Logs\\MyApplication";

// Exemple pour Video Capture SDK
videoCapture.Debug_Mode = true;
videoCapture.Debug_Dir = "C:\\Logs\\MyApplication";

// Exemple pour Video Edit SDK
videoEdit.Debug_Mode = true;
videoEdit.Debug_Dir = "C:\\Logs\\MyApplication";
```

### Guide d'implémentation détaillé

1. **Définir la propriété de mode de débogage**
   - Pour tout composant du SDK que vous utilisez, définissez la propriété `Debug_Mode` sur `true`
   - Cela doit être fait avant d'appeler les méthodes d'initialisation ou de lecture
   - Exemple : `MediaPlayer1.Debug_Mode = true;`

2. **Spécifier le répertoire des journaux**
   - Définissez la propriété `Debug_Dir` sur un chemin de répertoire valide
   - Assurez-vous que le répertoire spécifié existe et que votre application dispose des autorisations d'écriture
   - Exemple : `MediaPlayer1.Debug_Dir = "C:\\LogFiles\\MyApp";`

3. **Configurer les paramètres supplémentaires**
   - Configurez tous les autres paramètres requis pour votre cas d'usage spécifique
   - Ceux-ci peuvent inclure des sources vidéo, des codecs, des paramètres de sortie, etc.

4. **Initialiser et exécuter le composant**
   - Appelez la méthode appropriée pour démarrer le composant (par exemple, `Start()` ou `Play()`)
   - Laissez l'application s'exécuter jusqu'à ce que vous ayez reproduit le problème que vous dépannez

5. **Collecter et partager les journaux**
   - Localisez les fichiers journaux à la fois dans le répertoire que vous avez spécifié et dans « Mes documents\VisioForge »
   - Compressez tous les fichiers journaux dans une archive ZIP
   - Partagez via un service de partage de fichiers sécurisé

## Techniques de journalisation avancées

Pour des applications plus complexes ou des problèmes difficiles à reproduire, envisagez ces approches avancées de journalisation :

### Activation conditionnelle du débogage

Vous pouvez souhaiter activer la journalisation de débogage uniquement dans certains scénarios ou en fonction des actions de l'utilisateur :

```csharp
// Activer le mode de débogage uniquement lors du dépannage
if (troubleshootingMode)
{
    mediaPlayer.Debug_Mode = true;
    mediaPlayer.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "AppLogs"
    );
}
```

### Journalisation propre à l'environnement

Différents environnements de déploiement peuvent nécessiter différentes approches de journalisation :

```csharp
#if DEBUG
    // Journalisation pour l'environnement de développement
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "DevLogs"
    );
#else
    // Journalisation pour l'environnement de production (si autorisé par votre politique de confidentialité)
    string appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "YourCompany",
        "YourApp",
        "Logs"
    );
    Directory.CreateDirectory(appDataPath);
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = appDataPath;
#endif
```

## Bonnes pratiques pour une journalisation efficace

Pour vous assurer d'obtenir les informations les plus précieuses de vos journaux, suivez ces bonnes pratiques :

### 1. État initial propre

Avant de démarrer une session de journalisation, envisagez de réinitialiser l'état de votre application :

- Fermez et redémarrez l'application
- Effacez toutes les données en cache si pertinent
- Assurez-vous de capturer à partir d'un point de départ connu

### 2. Capturer des sessions complètes

Lorsque c'est possible, capturez toute la session du début à la fin :

- Activez la journalisation avant d'initialiser les composants du SDK
- Laissez la journalisation s'exécuter pendant toute l'opération
- Continuez la journalisation jusqu'après l'apparition du problème

### 3. Documenter les étapes de reproduction

Avec vos journaux, fournissez des étapes claires pour reproduire le problème :

- Notez les paramètres spécifiques utilisés
- Documentez la séquence exacte des opérations
- Incluez des informations de minutage si pertinent (par exemple, « le plantage se produit après 30 secondes de lecture »)

### 4. Gérer la taille des journaux

Les journaux de débogage peuvent devenir volumineux, en particulier pour les longues sessions :

- Pour les tests prolongés, envisagez de diviser la journalisation en plusieurs sessions
- Concentrez-vous sur la capture du seul scénario problématique
- Excluez toujours les fichiers multimédias volumineux lors du partage des journaux

### 5. Protéger les informations sensibles

Avant de partager des journaux, soyez conscient des données potentiellement sensibles :

- Examinez les journaux pour rechercher toute information personnelle ou sensible
- Envisagez d'utiliser du contenu de test assaini lorsque c'est possible
- Utilisez des méthodes sécurisées pour transférer les fichiers journaux

## Interprétation des messages de journal courants

Bien que l'analyse avancée des journaux soit mieux laissée à l'assistance technique, comprendre certains modèles courants de journaux peut vous aider à identifier les problèmes :

- **Erreurs d'initialisation** : recherchez les messages contenant « Init » ou « Initialize »
- **Problèmes de format** : surveillez les messages liés à « format » ou « codec »
- **Problèmes de ressources** : messages concernant « memory », « handles » ou « resources »
- **Avertissements de performance** : notes sur « frame drops », « processing time » ou « buffers »

## Conclusion

Une journalisation adéquate est essentielle pour un dépannage efficace des applications basées sur les SDK. En suivant les directives de ce document, vous pouvez fournir les informations détaillées nécessaires pour résoudre rapidement tout problème que vous rencontrez. N'oubliez pas que des journaux détaillés réduisent considérablement le temps de résolution et contribuent à améliorer la qualité de votre application et de nos SDK.

Pour des exemples de code supplémentaires et des guides d'implémentation, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
