---
title: Chiffrement vidéo AES-256 via les filtres COM DirectShow
description: Chiffrez la vidéo H.264/AAC MP4 en AES-256 via les filtres DirectShow. SDK VisioForge avec modes mot de passe et clé binaire pour C++, C# et Delphi.
sidebar_label: Video Encryption SDK
order: 5
tags:
  - Video Encryption SDK
  - DirectShow
  - C++
  - Windows
primary_api_classes:
  - IVFCryptoConfig
  - IVFPasswordProvider

---

# Video Encryption SDK

## Introduction au chiffrement vidéo

Le [Video Encryption SDK](https://www.visioforge.com/video-encryption-sdk) fournit des outils robustes pour encoder les fichiers vidéo au format MP4 H264/AAC avec des capacités de chiffrement avancées. Les développeurs peuvent sécuriser leur contenu multimédia à l'aide de mots de passe personnalisés ou de méthodes de chiffrement par données binaires.

Le SDK s'intègre de manière transparente à toute application DirectShow via un ensemble complet de filtres. Ces filtres exposent de nombreuses interfaces permettant aux développeurs d'affiner les paramètres en fonction d'exigences de sécurité et d'implémentation spécifiques.

---

## Installation

Avant d'utiliser les exemples de code et d'intégrer le SDK dans votre application, vous devez d'abord installer le Video Encryption SDK depuis la [page produit](https://www.visioforge.com/video-encryption-sdk).

**Étapes d'installation** :

1. Téléchargez l'installeur du SDK depuis la page produit
2. Exécutez l'installeur avec des privilèges administrateur
3. L'installeur enregistrera tous les filtres DirectShow nécessaires
4. Les applications d'exemple et le code source seront disponibles dans le répertoire d'installation

**Remarque** : les filtres du SDK doivent être correctement enregistrés sur le système avant de pouvoir être utilisés dans vos applications. L'installeur s'en charge automatiquement.

---

## Souplesse d'intégration

Vous pouvez implémenter le SDK dans diverses applications DirectShow comme filtres pour les processus de chiffrement et de déchiffrement. Le système fonctionne efficacement avec :

- Des sources vidéo en direct
- Des sources vidéo basées sur des fichiers
- Des encodeurs vidéo logiciels
- Des encodeurs vidéo accélérés par GPU du [pack de filtres d'encodage DirectShow](https://www.visioforge.com/encoding-filters-pack) (disponible séparément)
- Des filtres DirectShow tiers pour des options d'encodage vidéo supplémentaires

## Fonctionnalités clés

### Fonctionnalité de base

- **Chiffrement/déchiffrement sécurisé** : traitez des fichiers vidéo ou des flux de capture avec des algorithmes de sécurité robustes
- **Prise en charge des formats** : prise en charge complète de l'encodeur H264 pour le contenu vidéo
- **Gestion audio** : prise en charge complète de l'encodeur AAC pour les flux audio
- **Options de sécurité flexibles** : implémentez le chiffrement à l'aide de données binaires ou de mots de passe textuels

### Optimisation des performances

- Moteur de chiffrement AES-256 pour une sécurité maximale
- Prise en charge de l'accélération matérielle CPU
- Compatibilité avec l'accélération GPU
- Optimisé pour des processus de chiffrement haute vitesse

## Ressources de développement

### Exemples de code et documentation

Le SDK inclut des exemples de code complets pour plusieurs langages de programmation :

- Exemples d'implémentation en C#
- Code de référence en C++
- Projets exemples en Delphi

Ces exemples fournissent un guide d'implémentation concret pour les développeurs qui construisent des applications vidéo sécurisées.

### Application de démonstration

Explorez l'application Video Encryptor incluse pour une démonstration concrète des capacités du SDK dans un environnement fonctionnel.

---

## Référence d'API

### Interfaces principales

Le SDK fournit des interfaces COM complètes pour le chiffrement et le déchiffrement :

#### IVFCryptoConfig

Interface principale de configuration des paramètres de chiffrement sur le filtre multiplexeur (chiffrement) et le filtre démultiplexeur (déchiffrement).

**GUID** : `{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}`

**Méthodes** :
- `put_Provider` — définir un fournisseur de mot de passe pour les scénarios avancés (clés binaires, mots de passe dynamiques)
- `get_Provider` — obtenir l'interface du fournisseur de mot de passe
- `put_Password` — définir directement le mot de passe ou la clé de chiffrement (données binaires)
- `HavePassword` — vérifier si un mot de passe est défini

#### IVFPasswordProvider

Interface de rappel pour les scénarios avancés de fourniture de mot de passe comme les clés en données binaires, la génération dynamique de mots de passe ou la dérivation de clé personnalisée.

**GUID** : `{6F8162B5-778D-42b5-9242-1BBABB24FFC4}`

**Méthodes** :
- `QueryPassword` — interroger le mot de passe pour un fichier spécifique

**Cas d'usage** :
- Fourniture de données de clé binaires
- Génération dynamique de mots de passe
- Clés de chiffrement spécifiques à un fichier
- Fonctions de dérivation de clé personnalisées

#### Classes utilitaires

Le SDK inclut des méthodes d'extension utilitaires pour les développeurs .NET :
- `ApplyString` — applique un mot de passe textuel (haché en SHA-256)
- `ApplyFile` — utilise un fichier comme clé de chiffrement (hachage SHA-256 du contenu du fichier)
- `ApplyBinary` — applique des données de clé binaires (hachées en SHA-256)

### CLSID des filtres

| Filtre | CLSID | Rôle |
|--------|-------|---------|
| **Multiplexeur de chiffrement** | `{F1D3727A-88DE-49ab-A635-280BEFEFF902}` | Multiplexeur avec chiffrement |
| **Démultiplexeur de déchiffrement** | `{D2C761F0-9988-4f79-9B0E-FB2B79C65851}` | Démultiplexeur avec déchiffrement |

Pour une documentation détaillée des interfaces et des exemples de code, consultez la [référence des interfaces](interface-reference.md).

---

## Exemples de code

### Démarrage rapide — chiffrement

#### Exemple C#

```csharp
using VisioForge.DirectShowAPI;

// Obtenir l'interface de configuration crypto depuis le multiplexeur de chiffrement
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Appliquer un mot de passe textuel
    cryptoConfig.ApplyString("MySecurePassword123");

    // Ou utiliser un fichier comme cle
    // cryptoConfig.ApplyFile(@"C:\keys\mykey.bin");

    // Ou utiliser des donnees binaires
    // byte[] keyData = new byte[] { 0x01, 0x02, 0x03, ... };
    // cryptoConfig.ApplyBinary(keyData);
}
```

#### Exemple C++

```cpp
#include "encryptor_intf.h"

ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    // Definir le mot de passe
    const wchar_t* password = L"MySecurePassword123";
    hr = pCrypto->put_Password(
        (LPBYTE)password,
        wcslen(password) * sizeof(wchar_t)
    );

    pCrypto->Release();
}
```

### Déchiffrement

#### Exemple C#

```csharp
// Obtenir l'interface de configuration crypto depuis le demultiplexeur de dechiffrement
var cryptoConfig = demuxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Doit utiliser le meme mot de passe/cle qu'au chiffrement
    cryptoConfig.ApplyString("MySecurePassword123");
}
```

Pour des exemples complets incluant la configuration du graphe de filtres, consultez la [page d'exemples](examples.md).

---

## Applications d'exemple

Le SDK inclut des applications d'exemple opérationnelles qui démontrent les flux de chiffrement et de déchiffrement :

### Exemples inclus

- **Démo de chiffrement** — démontre le chiffrement d'un fichier vidéo avec encodage H.264/AAC
- **Démo lecteur** — montre le déchiffrement et la lecture de fichiers vidéo chiffrés

### Référentiel GitHub

Le code source complet de tous les exemples est disponible :
- [Exemples du Video Encryption SDK](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) — exemples C#, C++ et Delphi

Ces exemples incluent :
- Construction complète du graphe de filtres
- Configuration du chiffrement
- Déchiffrement et lecture
- Gestion d'erreurs
- Mise en œuvre des bonnes pratiques

---

## Informations de licence

- [Contrat de licence utilisateur final](../../eula.md)

## Historique des versions

### Version 11.4

- Compatibilité totale avec les SDK VisioForge .Net 11.4
- Prise en charge renforcée de Nvidia NVENC pour les encodeurs vidéo H264 et H265
- Prise en charge améliorée d'Intel QuickSync pour l'encodeur vidéo H264
- Ajout de la prise en charge de l'espace colorimétrique NV12 pour de meilleures performances

### Version 11.0

- Compatibilité complète avec les SDK VisioForge .Net 11.0
- Prise en charge renforcée des encodeurs GPU
- Mise à niveau de la fonctionnalité de l'encodeur AAC

### Version 10.0

- Compatibilité totale avec les SDK VisioForge .Net 10.0
- Compatibilité renforcée avec les formats vidéo H264 et H265
- Prise en charge intégrée de l'accélération AMD AMF
- Ajout de la prise en charge de la technologie Intel QuickSync

### Version 9.0

- Vitesse de traitement du chiffrement nettement améliorée
- Ajout des capacités d'accélération matérielle CPU
- Mise en œuvre d'un nouveau moteur basé sur le chiffrement AES-256
- Ajout de l'usage d'un fichier comme clé (avec prise en charge des tableaux binaires)
- Prise en charge intégrée de NVENC pour l'accélération GPU
- Prise en charge renforcée de l'encodeur AAC HE

### Version 8.0

- Encodeurs vidéo et audio mis à jour
- Performances de chiffrement de filtre améliorées

### Version 7.0

- Première publication en tant que produit autonome
- Précédemment intégré dans Video Capture SDK, Video Edit SDK et Media Player SDK
- Compatible avec toute application DirectShow sans nécessiter de SDK VisioForge supplémentaires

---

## Ressources

- [Page produit](https://www.visioforge.com/video-encryption-sdk) — achat, licences et informations produit
- [Applications d'exemple](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) — exemples de code source complets

---

## Voir aussi

- [Référence des interfaces](interface-reference.md) — documentation API complète
- [Exemples](examples.md) — exemples de code complets pour le chiffrement et le déchiffrement
- [Pack de filtres d'encodage DirectShow](../filters-enc/index.md) — encodeurs vidéo compatibles (H.264, H.265, AAC)
