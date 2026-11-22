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
video:
  id: y6JMVHwyfrs
  upload_date: 2026-11-18T07:33:05Z
  duration: PT8M4S

---

# Video Encryption SDK

## Introduction au chiffrement vidéo

Le [Video Encryption SDK](https://www.visioforge.com/video-encryption-sdk) fournit des outils robustes pour encoder les fichiers vidéo au format MP4 H264/AAC avec des capacités de chiffrement avancées. Les développeurs peuvent sécuriser leur contenu multimédia à l'aide de mots de passe personnalisés ou de méthodes de chiffrement par données binaires.

Le SDK s'intègre de manière transparente à toute application DirectShow via un ensemble complet de filtres. Ces filtres exposent de nombreuses interfaces permettant aux développeurs d'affiner les paramètres en fonction d'exigences de sécurité et d'implémentation spécifiques.


## Tutoriel vidéo

La vidéo chiffre des fichiers avec l'application Video Encryptor, puis compile les démos C#, parcourt leurs graphes de filtres et relit les fichiers chiffrés :

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/y6JMVHwyfrs?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

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
| **VisioForge Encryptor v9** | `{86D83D2E-70D2-4A32-B7BE-E94A08AA7BAA}` | Multiplexeur avec chiffrement AES-256 (fichiers de la version 9) |
| **VisioForge Decryptor v9** | `{4B89F5E2-2BA9-4C8F-A78A-EA2701AB582C}` | Démultiplexeur avec déchiffrement AES-256 (fichiers de la version 9) |
| **VisioForge Encryptor v8** | `{F1D3727A-88DE-49ab-A635-280BEFEFF902}` | Multiplexeur avec chiffrement AES-128 (fichiers de la version 8) |
| **VisioForge Decryptor v8** | `{D2C761F0-9988-4f79-9B0E-FB2B79C65851}` | Démultiplexeur avec déchiffrement AES-128 (fichiers de la version 8) |

Pour une documentation détaillée des interfaces et des exemples de code, consultez la [référence des interfaces](interface-reference.md).

---

## Exemples de code

### Démarrage rapide — chiffrement

#### Exemple C\#

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

#### Exemple C\#

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


## Chiffrer et lire un fichier en C\#

Les [démos C#](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK/dotnet) construisent deux graphes DirectShow : Encryption Demo écrit un fichier chiffré et Player Demo le relit. Ce sont deux applications WinForms sur .NET 8 pour x64, qui accèdent à DirectShow via le paquet NuGet [VisioForge.DirectShowAPI](https://www.nuget.org/packages/VisioForge.DirectShowAPI). Elles lisent les fichiers MP4 via LAV Filters, inclus dans l'installateur de Video Encryption SDK. Le code ci-dessous s'appuie sur les démos.

### Ajouter les filtres

Le filtre de chiffrement reçoit la vidéo H.264 et l'audio AAC et les écrit dans un seul fichier chiffré via File Writer. Le chiffreur de la version 9 écrit des fichiers AES-256, celui de la version 8 des fichiers AES-128.

```csharp
// AddEncryptor() in the demo
// Encryptor v9 writes AES-256 files; Consts.CLSID_VFVideoEncryptor8 writes AES-128 files
muxFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    new Guid(Consts.CLSID_VFVideoEncryptor9),
    "VisioForge Encryptor v9");

videoEncFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
    "VisioForge H264 Encoder");

audioEncFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
    "VisioForge AAC Encoder v10");

Guid CLSID_FileWriter = new Guid("8596E5F0-0DA5-11d0-BD21-00A0C911CE86");
fileWriterFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_FileWriter, "Filewriter");
IFileSinkFilter fileSink = fileWriterFilter as IFileSinkFilter;
fileSink?.SetFileName(edOutput.Text, null);
```

### Définir la clé

`IVFCryptoConfig` reçoit la clé : `ApplyString` un mot de passe, `ApplyFile` n'importe quel fichier, `ApplyBinary` un tableau d'octets. Définissez-la dans `AddEncryptor`, juste après l'ajout des filtres ci-dessus et avant que `RenderStream` ne connecte le chiffreur. Avec une clé de licence, `IVFRegister.SetLicenseKey` enregistre le filtre et les avis de la version d'essai disparaissent.

```csharp
IVFRegister reg = muxFilter as IVFRegister;
if (reg != null && !string.IsNullOrEmpty(SDK_LICENSE_KEY))
{
    reg.SetLicenseKey(SDK_LICENSE_KEY);
}

IVFCryptoConfig cryptoConfig = muxFilter as IVFCryptoConfig;
if (cryptoConfig == null)
{
    throw new InvalidOperationException("The encryptor filter is not registered.");
}

cryptoConfig.ApplyString(password);
// or: cryptoConfig.ApplyFile(keyFileName);
// or: cryptoConfig.ApplyBinary(keyBytes);
```

### Connecter le graphe

LAV Splitter Source sépare le MP4 en flux vidéo et audio. La vidéo est d'abord décodée dans le filtre RGB2YUV, et seulement ensuite les encodeurs rejoignent le graphe. Un fichier sans flux audio est chiffré en vidéo seule.

```csharp
// LAV Splitter Source splits the file into video and audio streams
Guid CLSID_LAVSplitterSource = new Guid("B98D13E7-55DB-4385-A33D-09FD1BA26338");
sourceVideoFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_LAVSplitterSource, "LAV Splitter Source");
hr = ((IFileSourceFilter)sourceVideoFilter).Load(edSourceFile.Text, null);
DsError.ThrowExceptionForHR(hr);

Guid CLSID_VFRGB2YUV = new Guid("3BDA461E-12DB-4C24-9815-B68D1AA4D34A");
rgb2yuvFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_VFRGB2YUV, "VisioForge RGB2YUV");

// Decode the video into RGB2YUV before the encoders join the graph
hr = captureGraphSource.RenderStream(null, MediaType.Video, sourceVideoFilter, null, rgb2yuvFilter);
DsError.ThrowExceptionForHR(hr);

AddEncryptor();

hr = captureGraphSource.RenderStream(null, null, rgb2yuvFilter, videoEncFilter, muxFilter);
DsError.ThrowExceptionForHR(hr);

// A file without audio is encrypted video-only
IPin audioPin = FilterHelper.GetFreePinWithMediaType(sourceVideoFilter, PinDirection.Output, MediaType.Audio);
if (audioPin != null)
{
    Marshal.ReleaseComObject(audioPin);
    hr = captureGraphSource.RenderStream(null, MediaType.Audio, sourceVideoFilter, audioEncFilter, muxFilter);
    DsError.ThrowExceptionForHR(hr);
}

hr = captureGraphSource.RenderStream(null, null, muxFilter, null, fileWriterFilter);
DsError.ThrowExceptionForHR(hr);

// No reference clock: the file is encrypted as fast as the encoders run
IMediaFilter mediaFilter = filterGraphSource as IMediaFilter;
mediaFilter?.SetSyncSource(null);

hr = mediaControlSource.Run();
DsError.ThrowExceptionForHR(hr);
```

### Relire le fichier

Un File Source ordinaire lit le fichier chiffré, et le filtre de déchiffrement le déchiffre avec la même clé. Le déchiffreur doit correspondre au fichier : version 9 pour AES-256, version 8 pour AES-128. Avec une mauvaise clé, le graphe ne peut pas connecter la sortie du déchiffreur.

```csharp
var CLSID_FileSourceAsync = new Guid("E436EBB5-524F-11CE-9F53-0020AF0BA770");
var source = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_FileSourceAsync, "File source (async.)");
int hr = ((IFileSourceFilter)source).Load(edSourceFile.Text, null);
DsError.ThrowExceptionForHR(hr);

// Decryptor v9 reads AES-256 files; Consts.CLSID_VFVideoDecryptor8 reads AES-128 files
decryptor = FilterGraphTools.AddFilterFromClsid(
    filterGraph,
    new Guid(Consts.CLSID_VFVideoDecryptor9),
    "VisioForge Decryptor v9");

IVFRegister reg = decryptor as IVFRegister;
if (reg != null && !string.IsNullOrEmpty(SDK_LICENSE_KEY))
{
    reg.SetLicenseKey(SDK_LICENSE_KEY);
}

IVFCryptoConfig cryptoConfig = decryptor as IVFCryptoConfig;
if (cryptoConfig == null)
{
    throw new InvalidOperationException("The decryptor filter is not registered.");
}

cryptoConfig.ApplyString(password);

Guid CLSID_VideoMixingRenderer9 = new Guid("51b4abf3-748f-4e3b-a276-c828330e926a");
videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_VideoMixingRenderer9, "VMR-9");

// Draw the video into a panel of the form
IVMRFilterConfig9 vmrFilterConfig = videoRenderer as IVMRFilterConfig9;
if (vmrFilterConfig == null)
{
    throw new InvalidOperationException("Unable to create the VMR-9 renderer.");
}

vmrFilterConfig.SetRenderingMode(VMR9Mode.Windowless);
IVMRWindowlessControl9 vmrWindowlessControl = videoRenderer as IVMRWindowlessControl9;
vmrWindowlessControl.SetVideoClippingWindow(pnScreen.Handle);

hr = captureGraph.RenderStream(null, null, source, null, decryptor);
DsError.ThrowExceptionForHR(hr);

hr = captureGraph.RenderStream(null, MediaType.Video, decryptor, null, videoRenderer);
DsError.ThrowExceptionForHR(hr);

// Audio goes to the default sound device; a file without audio plays video-only
IPin audioPin = FilterHelper.GetFreePinWithMediaType(decryptor, PinDirection.Output, MediaType.Audio);
if (audioPin != null)
{
    Marshal.ReleaseComObject(audioPin);
    hr = captureGraph.RenderStream(null, MediaType.Audio, decryptor, null, null);
    if (hr < 0)
    {
        MessageBox.Show(this, $"Unable to play the audio (0x{hr:X8}), playing video only.");
    }
}

vmrWindowlessControl.SetVideoPosition(null, new DsRect(0, 0, pnScreen.Width, pnScreen.Height));

hr = mediaControl.Run();
DsError.ThrowExceptionForHR(hr);
```

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

- Compatibilité totale avec les SDK VisioForge .NET 11.4
- Prise en charge renforcée de Nvidia NVENC pour les encodeurs vidéo H264 et H265
- Prise en charge améliorée d'Intel QuickSync pour l'encodeur vidéo H264
- Ajout de la prise en charge de l'espace colorimétrique NV12 pour de meilleures performances

### Version 11.0

- Compatibilité complète avec les SDK VisioForge .NET 11.0
- Prise en charge renforcée des encodeurs GPU
- Mise à niveau de la fonctionnalité de l'encodeur AAC

### Version 10.0

- Compatibilité totale avec les SDK VisioForge .NET 10.0
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


## Transcription de la vidéo

### Au programme

Chiffrons des fichiers vidéo avec un mot de passe sous Windows. D'abord avec l'application prête à l'emploi VisioForge Video Encryptor, puis dans votre propre code C# avec Video Encryption SDK. Voici le programme. Video Encryptor chiffre un fichier avec un mot de passe, sans aucun code. Sa vue avancée ajoute une liste de fichiers, le dossier de sortie et les réglages des codecs. Ensuite, nous compilons les démos du SDK dans Visual Studio, chiffrons un MP4 en AES-256 depuis C# et relisons le fichier chiffré avec le même mot de passe.

### Le principe

Comment ça marche ? La vidéo est encodée en H.264 et l'audio en AAC. Le filtre de chiffrement chiffre les deux et les écrit dans un seul fichier. Seul le filtre de déchiffrement, avec la même clé, peut relire ce fichier. Les fichiers de la version 9 utilisent AES-256, ceux de la version 8 AES-128. La clé est une chaîne de caractères, et la version 9 accepte aussi un fichier ou des données binaires comme clé. Ce sont tous des filtres DirectShow : ils fonctionnent dans n'importe quel graphe DirectShow. Il vous faut Windows 10 ou 11. Pour la partie application, installez Video Encryptor. Pour le code, installez Video Encryption SDK : il enregistre les filtres et inclut LAV Filters, que la démo utilise pour lire les fichiers MP4. Puis Visual Studio 2022 ou 2026 avec la charge de travail Développement .NET Desktop, et les démos du dépôt directshow-samples sur GitHub. Les deux programmes d'installation sont sur visioforge.com.

### Video Encryptor : vue simple

Video Encryptor s'ouvre en vue simple. Saisissez d'abord un mot de passe. Puis cliquez sur le cadenas, ou déposez-y un fichier, et choisissez la vidéo. La version non enregistrée affiche quelques avis d'essai avant de commencer ; ils sont coupés ici. Le fichier chiffré est créé à côté de la source, avec l'extension .evf. Chiffrement terminé.

### Video Encryptor : vue avancée

La vue avancée accepte une liste de fichiers. Cliquez sur Add files et choisissez une ou plusieurs vidéos. Dans l'onglet Settings, choisissez le dossier de sortie : le dossier de la source, ou un dossier de votre choix. Output format contient les réglages H.264 de la vidéo, avec le débit, et les réglages AAC de l'audio. Saisissez le mot de passe et cliquez sur Start. L'aperçu montre la vidéo pendant son chiffrement, et la barre de progression va jusqu'au bout. Terminé : un second fichier .evf, à côté de sa source.

### Les démos du SDK

Le SDK donne les mêmes filtres à votre propre application. Sa Encryption Demo chiffre un fichier vidéo, ou une caméra, dans un fichier chiffré. Sa Player Demo relit un fichier chiffré avec sa clé. Ce sont deux applications C# WinForms sous .NET 8, pour x64, qui accèdent à DirectShow via le paquet NuGet VisioForge.DirectShowAPI. Ouvrez VideoEncryptionSDK.sln depuis le dépôt et générez la solution. NuGet restaure le paquet, et les deux démos sont générées.

### Chiffrer : les filtres

Le graphe de chiffrement commence dans AddEncryptor. Il ajoute le filtre de chiffrement par son ID de classe : le chiffreur de la version 9 pour AES-256, ou celui de la version 8 pour AES-128. Puis l'encodeur H.264. Puis l'encodeur AAC pour l'audio, et le File Writer, avec le nom du fichier de sortie.

### Chiffrer : la clé

ApplyEncryptorSettings configure le chiffreur. Avec une clé de licence, IVFRegister.SetLicenseKey l'enregistre, et les avis d'essai disparaissent. IVFCryptoConfig reçoit la clé. ApplyString prend un mot de passe, ApplyFile utilise n'importe quel fichier comme clé, et ApplyBinary prend un tableau d'octets.

### Chiffrer : le graphe

SourceInitFile ouvre la source avec LAV Splitter Source, qui sépare un MP4 en ses flux vidéo et audio. Si LAV Filters manque, la démo le signale et s'arrête. La vidéo est d'abord décodée vers le filtre RGB2YUV, et seulement ensuite les encodeurs rejoignent le graphe. Puis RenderStream relie RGB2YUV au chiffreur en passant par l'encodeur H.264. Si le fichier a un flux audio, il passe lui aussi par l'encodeur AAC jusqu'au chiffreur. Un fichier sans audio est chiffré en vidéo seule. Enfin, le chiffreur se connecte au File Writer.

### Chiffrer un fichier

Lancez la Encryption Demo. Indiquez le fichier source, le fichier de sortie et le mot de passe. AES-256 est l'option par défaut. Cliquez sur Start. Sans clé de licence, le SDK affiche d'abord un avis d'essai ; il est coupé ici. Trente secondes de vidéo prennent environ dix secondes, et la démo affiche Completed.

### La lecture

Passons à la Player Demo. Un simple File Source lit le fichier chiffré : ce ne sont que des octets tant que le déchiffreur ne les voit pas. Le filtre de déchiffrement correspond au chiffreur : la version 9 pour AES-256. Il reçoit la clé par la même interface IVFCryptoConfig. La source se connecte au déchiffreur. La vidéo du déchiffreur va au moteur de rendu VMR-9, et son audio, si le fichier en a, au périphérique audio par défaut. Définissez Player Demo comme projet de démarrage et lancez-la. Choisissez le fichier chiffré et saisissez le même mot de passe. Start, et la lecture commence. Le même lecteur ouvre le fichier créé par Video Encryptor. C'est un fichier de la version 8 : choisissez donc AES-128. En version d'essai, celui-ci affiche une petite mention Unregistered dans un coin.

### À retenir

Quelques points à retenir. Le mot de passe est la clé : sans lui, impossible de revenir en arrière. Le déchiffreur doit correspondre au fichier, version 9 ou version 8. Une mauvaise clé échoue : le graphe ne peut pas connecter la sortie du déchiffreur. Avec une licence, SetLicenseKey enregistre les filtres, et les avis d'essai disparaissent. Et la Encryption Demo chiffre aussi une caméra : choisissez Device au lieu de Video file.

### Pour aller plus loin

Les démos sont sur GitHub, et la documentation sur visioforge.com/help, avec une référence de toutes les interfaces des filtres. Merci de votre attention.

---

## Voir aussi

- [Référence des interfaces](interface-reference.md) — documentation API complète
- [Exemples](examples.md) — exemples de code complets pour le chiffrement et le déchiffrement
- [Pack de filtres d'encodage DirectShow](../filters-enc/index.md) — encodeurs vidéo compatibles (H.264, H.265, AAC)
