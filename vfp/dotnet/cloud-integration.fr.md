---
title: Empreinte vidéo cloud avec Azure, AWS et MongoDB — .NET SDK
description: Déployez l'empreinte vidéo VisioForge sur Azure et AWS avec stockage MongoDB, traitement distribué et architectures serverless.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - MP4
  - MKV
  - AVI
  - MOV
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VFPAnalyzer
  - VFPFingerprintSource
  - FingerprintWorkflowInput
  - BlobServiceClient
  - SQSEvent

---

# Guide cloud pour l'empreinte vidéo

## Paquets NuGet disponibles pour l'intégration cloud

### Paquet d'intégration MongoDB (solution préconçue)

Le Video Fingerprinting SDK fournit un paquet NuGet prêt à l'emploi pour l'intégration MongoDB :

**Paquet :** `VisioForge.DotNet.VideoFingerPrinting.MongoDB`  
**Version :** 2025.8.7  
**Objectif :** intégration MongoDB complète avec prise en charge GridFS pour le stockage d'empreintes

#### Installation

```bash
# Console du gestionnaire de paquets
Install-Package VisioForge.DotNet.VideoFingerPrinting.MongoDB -Version 2025.8.7

# CLI .NET
dotnet add package VisioForge.DotNet.VideoFingerPrinting.MongoDB --version 2025.8.7

# PackageReference
<PackageReference Include="VisioForge.DotNet.VideoFingerPrinting.MongoDB" Version="2025.8.7" />
```

#### Fonctionnalités clés

Le paquet MongoDB fournit la classe `VideoFingerprintDB` avec ces capacités :

- **Intégration MongoDB GridFS** : stockage efficace de grandes données d'empreintes
- **Prise en charge cloud et locale** : fonctionne avec MongoDB Atlas (cloud) et avec des déploiements MongoDB locaux
- **Opérations CRUD complètes** : gestion d'empreintes complète (Create, Read, Update, Delete)
- **Options de connexion flexibles** : plusieurs surcharges de constructeur pour différents scénarios de connexion

#### Utilisation de base avec MongoDB Atlas

```csharp
using VisioForge.VideoFingerPrinting.MongoDB; // le namespace ne comporte pas de segment `DotNet.`
using VisioForge.Core.VideoFingerPrinting;

// Connexion à MongoDB Atlas (déploiement cloud).
// La signature du constructeur est (string dbname, string connectionString) — dbname en premier.
var connectionString = "mongodb+srv://username:password@cluster.mongodb.net/";
var db = new VideoFingerprintDB("fingerprint-database", connectionString);

// Charger les empreintes précédemment envoyées dans db.Items.
db.LoadFromDB();

// Générer une empreinte (l'analyseur requiert un VFPFingerprintSource, pas un chemin string).
var src = new VFPFingerprintSource("video.mp4");
var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(src);

// VFPFingerPrint.ID est un Guid, pas une string — choisir un Guid stable pour chaque vidéo.
var videoId = Guid.NewGuid();
fingerprint.ID = videoId;

// Stocker l'empreinte (Upload est sync et prend un seul VFPFingerPrint).
db.Upload(fingerprint);

// « Récupération » — le SDK garde les empreintes chargées dans db.Items ; choisir par ID (Guid).
var retrieved = db.Items.FirstOrDefault(f => f.ID == videoId);

// Rechercher dans la collection en mémoire via VFPAnalyzer.SearchAsync (forme à 5 arguments).
// VFPSearch.SearchAsync n'existe pas — utiliser VFPAnalyzer.SearchAsync.
foreach (var candidate in db.Items)
{
    var matches = await VFPAnalyzer.SearchAsync(
        fingerprint,
        candidate,
        candidate.Duration,
        maxDifference: 20,
        allowMultipleFragments: true);
    // ... inspecter matches (List<TimeSpan>) ...
}

// Suppression : RemoveByID(id, fromDB=true) supprime à la fois de Items et de la base.
db.RemoveByID(videoId);
```

### Déploiement cloud MongoDB Atlas

Le paquet MongoDB est particulièrement adapté aux déploiements cloud utilisant MongoDB Atlas, le service de base de données cloud entièrement managé de MongoDB :

#### Configuration de MongoDB Atlas

1. **Créer un compte MongoDB Atlas** : inscrivez-vous sur [cloud.mongodb.com](https://www.mongodb.com/products/platform/atlas-database)
2. **Créer un cluster** : choisissez votre fournisseur cloud (AWS, Azure ou Google Cloud)
3. **Configurer l'accès réseau** : ajoutez des adresses IP ou activez l'accès depuis n'importe où
4. **Créer un utilisateur de base** : configurez les identifiants d'authentification
5. **Obtenir la chaîne de connexion** : copiez la chaîne de connexion depuis la vue d'ensemble du cluster

#### Utilisation avancée de MongoDB Atlas

```csharp
using VisioForge.VideoFingerPrinting.MongoDB; // le namespace ne comporte pas de segment `DotNet.`
using VisioForge.Core.VideoFingerPrinting;
using MongoDB.Driver;

public class CloudFingerprintService
{
    private readonly VideoFingerprintDB _db;

    public CloudFingerprintService(string atlasConnectionString)
    {
        // Format de chaîne de connexion pour Atlas :
        // mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority
        // Le constructeur prend (dbname, connectionString) dans cet ordre.
        _db = new VideoFingerprintDB("production-fingerprints", atlasConnectionString);
        _db.LoadFromDB(); // hydrater db.Items
    }

    /// <summary>
    /// Traiter et stocker une empreinte vidéo dans MongoDB Atlas. Le paquet MongoDB
    /// stocke les blobs VFPFingerPrint bruts — les métadonnées supplémentaires sont
    /// gérées par l'application (par exemple, une collection séparée indexée sur VFPFingerPrint.ID).
    /// </summary>
    public async Task<string> ProcessAndStoreVideoAsync(string videoPath, string videoId)
    {
        var src = new VFPFingerprintSource(videoPath);
        var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(src);
        fingerprint.ID = videoId;

        _db.Upload(fingerprint); // sync, un seul argument
        return videoId;
    }

    /// <summary>
    /// Rechercher dans la collection en mémoire (db.Items) des empreintes similaires.
    /// </summary>
    public async Task<List<(string Id, double Difference)>> FindSimilarVideosAsync(
        string videoPath, int maxDiff = 20)
    {
        var src = new VFPFingerprintSource(videoPath);
        var searchFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(src);

        var hits = new List<(string, double)>();
        foreach (var candidate in _db.Items)
        {
            var pos = VFPSearch.Search(searchFp, 0, candidate, 0, out double diff, maxDiff);
            if (pos != int.MaxValue)
                hits.Add((candidate.ID, diff));
        }
        return hits;
    }
}

// MongoDB Atlas avec MongoClientSettings personnalisés (utiliser le constructeur
// (dbname, MongoClientSettings) — il n'existe pas de surcharge (IMongoClient, string)).
public class ResilientMongoDBService
{
    private readonly VideoFingerprintDB _db;
    private readonly MongoClientSettings _settings;

    public ResilientMongoDBService(string atlasConnectionString)
    {
        _settings = MongoClientSettings.FromConnectionString(atlasConnectionString);
        _settings.MaxConnectionPoolSize = 100;
        _settings.MinConnectionPoolSize = 10;
        _settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
        _settings.RetryReads = true;
        _settings.RetryWrites = true;

        _db = new VideoFingerprintDB("fingerprint-database", _settings);
    }
}
```

#### Fonctionnalités MongoDB Atlas pour l'empreinte vidéo

- **Clusters globaux** : déployez les empreintes sur plusieurs régions pour une faible latence
- **Mise à l'échelle automatique** : mise à l'échelle automatique du stockage et du calcul selon la charge
- **Atlas Search** : capacités de recherche en texte intégral pour les métadonnées
- **Change Streams** : notifications en temps réel lors de l'ajout/modification d'empreintes
- **Sauvegarde et restauration** : sauvegardes automatisées avec restauration à un instant précis
- **Sécurité** : chiffrement au repos et en transit, liste blanche d'IP, AWS PrivateLink

### Intégration Azure et AWS (schémas d'implémentation)

**Note importante :** contrairement au paquet MongoDB, les intégrations Azure et AWS sont fournies en tant que **schémas d'implémentation et exemples de code** à adapter à vos exigences spécifiques. Vous devrez installer les SDK respectifs des fournisseurs cloud :

#### SDK fournisseurs cloud requis

Pour l'intégration Azure :

```bash
# Azure Storage
Install-Package Azure.Storage.Blobs
# Azure Functions
Install-Package Microsoft.Azure.Functions.Worker
# Azure Identity (pour les identités managées)
Install-Package Azure.Identity
```

Pour l'intégration AWS :

```bash
# AWS SDK pour S3
Install-Package AWSSDK.S3
# AWS Lambda
Install-Package Amazon.Lambda.Core
Install-Package Amazon.Lambda.APIGatewayEvents
# AWS SQS (pour le traitement par file)
Install-Package AWSSDK.SQS
```

## API principales du SDK pour l'intégration cloud

Le Video Fingerprinting SDK fournit ces classes essentielles pour toutes les implémentations cloud :

- **VFPAnalyzer** (statique) — génère des empreintes à partir de fichiers vidéo :
  - `GetComparingFingerprintForVideoFileAsync(VFPFingerprintSource source, VFPErrorCallback errorCallback = null, VFPProgressCallback progressCallback = null)`
  - `GetSearchFingerprintForVideoFileAsync(VFPFingerprintSource source, VFPErrorCallback errorCallback = null, VFPProgressCallback progressCallback = null)`
  - `SearchAsync(VFPFingerPrint fp1, VFPFingerPrint fp2, TimeSpan duration, int maxDifference, bool allowMultipleFragments)` — retourne `Task<List<TimeSpan>>` des temps de début des correspondances
  - `SetLicenseKey(licenseKey)` — définir la licence du SDK

  > Les deux générateurs d'empreinte prennent un `VFPFingerprintSource`, pas un nom de fichier string.
  > Construisez la source avec `new VFPFingerprintSource(filename)` et configurez
  > `CustomResolution` / `IgnoredAreas` / `CustomCropSize` / `StartTime` / `StopTime`
  > sur la source avant de la passer à l'analyseur.

- **VFPFingerPrint** — données d'empreinte avec sérialisation :
  - `Save(Stream)` — enregistre vers n'importe quel flux (mémoire, fichier, réseau)
  - `Load(Stream)` ou `Load(byte[])` — charge depuis un flux ou des octets
  - Parfait pour le stockage cloud (S3, Azure Blob, MongoDB GridFS)

- **VFPFingerprintSource** — spécifie la source du fichier vidéo :
  - Propriété `Filename` — chemin du fichier vidéo
  - `StartTime`, `StopTime` — traiter des segments spécifiques

- **VFPCompare** et **VFPSearch** — classes statiques pour les opérations :
  - `VFPCompare.Process()`, `Build()`, `Compare()`
  - `VFPSearch.Process()`, `Build()`, `Search()`

**Schéma d'intégration cloud :**

1. Télécharger la vidéo depuis le stockage cloud vers un fichier temporaire local
2. Traiter avec VFPAnalyzer.GetComparingFingerprintForVideoFileAsync()
3. Sérialiser l'empreinte avec Save() vers un flux mémoire
4. Envoyer les données sérialisées vers le stockage cloud

**Note :** le SDK fonctionne avec des FICHIERS vidéo, pas des flux. Pour les vidéos cloud, téléchargez d'abord vers un fichier temporaire.

Ce guide complet couvre les flux de travail d'empreinte vidéo basés sur le cloud, notamment l'intégration MongoDB préconçue et les schémas d'implémentation pour Azure et AWS, le traitement serverless, les architectures distribuées et les stratégies d'optimisation des coûts.

## Vue d'ensemble

L'empreinte vidéo basée sur le cloud offre des avantages significatifs en termes d'évolutivité, de rentabilité et de distribution globale. Ce guide couvre les schémas d'implémentation pour les principaux fournisseurs cloud et les bonnes pratiques architecturales.

### Avantages de l'empreinte basée sur le cloud

- **Évolutivité** : mise à l'échelle automatique du traitement selon la demande
- **Efficacité des coûts** : paiement uniquement pour les ressources utilisées
- **Distribution globale** : traitement et stockage d'empreintes proches des utilisateurs
- **Services managés** : exploitation des services cloud natifs pour stockage, file d'attente et traitement
- **Haute disponibilité** : redondance et bascule intégrées
- **Sans maintenance** : aucune charge de gestion d'infrastructure

## Intégration Azure

### Stockage d'empreintes avec Azure Blob Storage

Azure Blob Storage fournit un stockage évolutif et rentable pour les fichiers vidéo et les empreintes.

#### Configuration d'Azure Storage

```csharp
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;

public class AzureFingerprintStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    
    public AzureFingerprintStorage(string connectionString, string containerName)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = containerName;
        
        // Vérifier que le conteneur existe
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        containerClient.CreateIfNotExists(PublicAccessType.None);
    }
    
    /// <summary>
    /// Envoyer une empreinte vers Azure Blob Storage
    /// </summary>
    public async Task<string> UploadFingerprintAsync(
        VFPFingerPrint fingerprint, 
        string videoId,
        Dictionary<string, string> metadata = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        // Créer une structure hiérarchique pour l'organisation
        string blobName = $"fingerprints/{DateTime.UtcNow:yyyy/MM/dd}/{videoId}.vfp";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        // Sérialiser l'empreinte vers un flux mémoire
        using var stream = new MemoryStream();
        fingerprint.Save(stream);
        stream.Position = 0;
        
        // Définir les métadonnées
        var blobMetadata = new Dictionary<string, string>
        {
            ["videoId"] = videoId,
            ["duration"] = fingerprint.Duration.ToString(),
            ["resolution"] = $"{fingerprint.Width}x{fingerprint.Height}",
            ["createdUtc"] = DateTime.UtcNow.ToString("O")
        };
        
        if (metadata != null)
        {
            foreach (var kvp in metadata)
            {
                blobMetadata[kvp.Key] = kvp.Value;
            }
        }
        
        // Envoyer avec les métadonnées
        var uploadOptions = new BlobUploadOptions
        {
            Metadata = blobMetadata,
            Tags = new Dictionary<string, string>
            {
                ["type"] = "fingerprint",
                ["version"] = "1.0"
            }
        };
        
        await blobClient.UploadAsync(stream, uploadOptions);
        
        return blobClient.Uri.ToString();
    }
    
    /// <summary>
    /// Télécharger une empreinte depuis Azure Blob Storage
    /// </summary>
    public async Task<VFPFingerPrint> DownloadFingerprintAsync(string blobUri)
    {
        var blobClient = new BlobClient(new Uri(blobUri));
        
        // Télécharger en mémoire
        var response = await blobClient.DownloadContentAsync();
        var bytes = response.Value.Content.ToArray();
        
        // Désérialiser l'empreinte
        using var stream = new MemoryStream(bytes);
        return VFPFingerPrint.Load(stream);
    }
    
    /// <summary>
    /// Lister les empreintes avec filtrage optionnel
    /// </summary>
    public async Task<List<FingerprintMetadata>> ListFingerprintsAsync(
        string prefix = null,
        DateTimeOffset? createdAfter = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var results = new List<FingerprintMetadata>();
        
        // Construire la requête
        string query = "type = 'fingerprint'";
        
        await foreach (var blobItem in containerClient.GetBlobsAsync(
            prefix: prefix ?? "fingerprints/",
            traits: BlobTraits.Metadata | BlobTraits.Tags))
        {
            if (createdAfter.HasValue && 
                blobItem.Properties.CreatedOn < createdAfter.Value)
                continue;
            
            results.Add(new FingerprintMetadata
            {
                Uri = containerClient.GetBlobClient(blobItem.Name).Uri.ToString(),
                VideoId = blobItem.Metadata.GetValueOrDefault("videoId"),
                Duration = blobItem.Metadata.GetValueOrDefault("duration"),
                Resolution = blobItem.Metadata.GetValueOrDefault("resolution"),
                CreatedUtc = DateTimeOffset.Parse(
                    blobItem.Metadata.GetValueOrDefault("createdUtc")),
                Size = blobItem.Properties.ContentLength ?? 0
            });
        }
        
        return results;
    }
}

public class FingerprintMetadata
{
    public string Uri { get; set; }
    public string VideoId { get; set; }
    public string Duration { get; set; }
    public string Resolution { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public long Size { get; set; }
}
```

#### Implémentation d'un stockage par paliers

```csharp
using Azure.Storage.Blobs.Models;

public class TieredFingerprintStorage : AzureFingerprintStorage
{
    /// <summary>
    /// Déplacer les anciennes empreintes vers le palier cool/archive pour économiser
    /// </summary>
    public async Task OptimizeStorageTiersAsync(int hotTierDays = 7, int coolTierDays = 30)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        await foreach (var blobItem in containerClient.GetBlobsAsync(
            traits: BlobTraits.Metadata))
        {
            var age = DateTime.UtcNow - blobItem.Properties.CreatedOn.Value.DateTime;
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            
            AccessTier targetTier;
            
            if (age.TotalDays <= hotTierDays)
            {
                targetTier = AccessTier.Hot;
            }
            else if (age.TotalDays <= coolTierDays)
            {
                targetTier = AccessTier.Cool;
            }
            else
            {
                targetTier = AccessTier.Archive;
            }
            
            // Modifier uniquement si différent du palier actuel
            if (blobItem.Properties.AccessTier != targetTier.ToString())
            {
                await blobClient.SetAccessTierAsync(targetTier);
                
                Console.WriteLine($"{blobItem.Name} déplacé vers le palier {targetTier}");
            }
        }
    }
    
    /// <summary>
    /// Réhydrater une empreinte archivée pour l'accès
    /// </summary>
    public async Task<bool> RehydrateFingerprintAsync(string blobUri, AccessTier targetTier = AccessTier.Hot)
    {
        var blobClient = new BlobClient(new Uri(blobUri));
        
        var properties = await blobClient.GetPropertiesAsync();
        
        if (properties.Value.AccessTier == "Archive")
        {
            await blobClient.SetAccessTierAsync(targetTier, rehydratePriority: RehydratePriority.High);
            
            // Attendre la réhydratation (peut prendre des heures pour la priorité standard)
            while (properties.Value.ArchiveStatus == "rehydrate-pending-to-hot")
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                properties = await blobClient.GetPropertiesAsync();
            }
            
            return true;
        }
        
        return false;
    }
}
```

### Azure Functions pour le traitement serverless

Azure Functions fournit du calcul serverless pour traiter les vidéos à la demande.

#### Configuration de Function App

```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VisioForge.Core.VideoFingerPrinting;

public class FingerprintFunction
{
    private readonly ILogger<FingerprintFunction> _logger;
    private readonly AzureFingerprintStorage _storage;
    
    public FingerprintFunction(ILogger<FingerprintFunction> logger)
    {
        _logger = logger;
        _storage = new AzureFingerprintStorage(
            Environment.GetEnvironmentVariable("AzureStorageConnection"),
            "fingerprints"
        );
        
        // Initialiser le SDK
        VFPAnalyzer.SetLicenseKey(Environment.GetEnvironmentVariable("VFP_LICENSE_KEY"));
    }
    
    /// <summary>
    /// Fonction déclenchée par HTTP pour la génération d'empreintes à la demande
    /// </summary>
    [Function("GenerateFingerprint")]
    public async Task<HttpResponseData> GenerateFingerprint(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("Traitement de la requête de génération d'empreinte");
        
        // Analyser la requête
        var requestBody = await req.ReadAsStringAsync();
        var request = JsonSerializer.Deserialize<FingerprintRequest>(requestBody);
        
        try
        {
            // Télécharger la vidéo depuis blob storage
            var videoPath = await DownloadVideoAsync(request.VideoUri);
            
            // Configurer la génération d'empreinte
            var source = new VFPFingerprintSource(videoPath)
            {
                CustomResolution = new VisioForge.Core.Types.Size(640, 480),
                FrameRate = request.FrameRate ?? 10
            };
            
            if (request.StartTime.HasValue)
                source.StartTime = TimeSpan.FromSeconds(request.StartTime.Value);
            
            if (request.StopTime.HasValue)
                source.StopTime = TimeSpan.FromSeconds(request.StopTime.Value);
            
            // Générer l'empreinte
            var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                source,
                errorDelegate: (error) => _logger.LogError($"Erreur d'empreinte : {error}")
            );
            
            if (fingerprint != null)
            {
                // Envoyer vers le stockage
                var fingerprintUri = await _storage.UploadFingerprintAsync(
                    fingerprint, 
                    request.VideoId,
                    new Dictionary<string, string>
                    {
                        ["sourceVideo"] = request.VideoUri,
                        ["processedBy"] = Environment.MachineName
                    }
                );
                
                // Retourner la réponse de succès
                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new FingerprintResponse
                {
                    Success = true,
                    FingerprintUri = fingerprintUri,
                    Duration = fingerprint.Duration.TotalSeconds,
                    ProcessingTime = DateTime.UtcNow.Subtract(request.RequestTime).TotalSeconds
                });
                
                return response;
            }
            
            throw new Exception("Échec de la génération de l'empreinte");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération de l'empreinte");
            
            var response = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new FingerprintResponse
            {
                Success = false,
                Error = ex.Message
            });
            
            return response;
        }
        finally
        {
            // Nettoyer les fichiers temporaires
            CleanupTempFiles();
        }
    }
    
    /// <summary>
    /// Fonction déclenchée par file d'attente pour le traitement par lots
    /// </summary>
    [Function("ProcessFingerprintQueue")]
    public async Task ProcessQueue(
        [QueueTrigger("fingerprint-requests")] FingerprintRequest request)
    {
        _logger.LogInformation($"Traitement de la requête en file pour la vidéo : {request.VideoId}");
        
        try
        {
            // Logique de traitement similaire au déclencheur HTTP
            // mais optimisée pour le traitement en arrière-plan
            
            // Télécharger la vidéo
            var videoPath = await DownloadVideoAsync(request.VideoUri);
            
            // Générer l'empreinte avec logique de nouvelle tentative
            VFPFingerPrint fingerprint = null;
            int maxRetries = 3;
            
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var source = new VFPFingerprintSource(videoPath)
                    {
                        CustomResolution = new VisioForge.Core.Types.Size(640, 480)
                    };
                    
                    fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
                    
                    if (fingerprint != null)
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Tentative {i + 1} échouée : {ex.Message}");
                    
                    if (i == maxRetries - 1)
                        throw;
                    
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Backoff exponentiel
                }
            }
            
            // Stocker l'empreinte
            if (fingerprint != null)
            {
                await _storage.UploadFingerprintAsync(fingerprint, request.VideoId);
                
                // Envoyer la notification de fin
                await NotifyCompletionAsync(request.VideoId, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Échec du traitement de la vidéo {request.VideoId}");
            await NotifyCompletionAsync(request.VideoId, false, ex.Message);
            throw; // Relancer pour déclencher les politiques de réessai
        }
    }
    
    private async Task<string> DownloadVideoAsync(string videoUri)
    {
        // Implémentation pour télécharger la vidéo depuis blob storage
        // Retourne le chemin local du fichier téléchargé
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");
        
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(videoUri);
        using var fileStream = File.Create(tempPath);
        await response.Content.CopyToAsync(fileStream);
        
        return tempPath;
    }
    
    private void CleanupTempFiles()
    {
        // Nettoyer les fichiers vidéo temporaires
        var tempDir = Path.GetTempPath();
        var files = Directory.GetFiles(tempDir, "*.mp4")
            .Where(f => File.GetCreationTime(f) < DateTime.Now.AddHours(-1));
        
        foreach (var file in files)
        {
            try { File.Delete(file); } catch { }
        }
    }
    
    private async Task NotifyCompletionAsync(string videoId, bool success, string error = null)
    {
        // Envoyer une notification via Service Bus, Event Grid ou webhook
        // L'implémentation dépend de votre stratégie de notification
    }
}

public class FingerprintRequest
{
    public string VideoId { get; set; }
    public string VideoUri { get; set; }
    public int? FrameRate { get; set; }
    public double? StartTime { get; set; }
    public double? StopTime { get; set; }
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
}

public class FingerprintResponse
{
    public bool Success { get; set; }
    public string FingerprintUri { get; set; }
    public double Duration { get; set; }
    public double ProcessingTime { get; set; }
    public string Error { get; set; }
}
```

#### Durable Functions pour les flux de travail complexes

```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

public class DurableFingerprintOrchestrator
{
    /// <summary>
    /// Fonction d'orchestration pour des flux de travail d'empreinte complexes
    /// </summary>
    [Function("FingerprintOrchestrator")]
    public static async Task<FingerprintWorkflowResult> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<FingerprintWorkflowInput>();
        var results = new FingerprintWorkflowResult();
        
        try
        {
            // Étape 1 : valider la vidéo
            var validation = await context.CallActivityAsync<ValidationResult>(
                "ValidateVideo", 
                input.VideoUri);
            
            if (!validation.IsValid)
            {
                results.Success = false;
                results.Error = validation.Error;
                return results;
            }
            
            // Étape 2 : diviser la vidéo en segments pour le traitement parallèle
            var segments = await context.CallActivityAsync<List<VideoSegment>>(
                "SplitVideo",
                new SplitVideoInput
                {
                    VideoUri = input.VideoUri,
                    SegmentDuration = 300 // 5 minutes
                });
            
            // Étape 3 : traiter les segments en parallèle
            var fingerprintTasks = segments.Select(segment =>
                context.CallActivityAsync<SegmentFingerprint>(
                    "GenerateSegmentFingerprint",
                    segment)
            ).ToList();
            
            var segmentFingerprints = await Task.WhenAll(fingerprintTasks);
            
            // Étape 4 : fusionner les empreintes
            var mergedFingerprint = await context.CallActivityAsync<string>(
                "MergeFingerprints",
                segmentFingerprints);
            
            // Étape 5 : stocker et indexer
            var storageResult = await context.CallActivityAsync<StorageResult>(
                "StoreAndIndexFingerprint",
                new StoreInput
                {
                    FingerprintUri = mergedFingerprint,
                    VideoId = input.VideoId,
                    Metadata = input.Metadata
                });
            
            results.Success = true;
            results.FingerprintUri = storageResult.Uri;
            results.ProcessingTime = context.CurrentUtcDateTime.Subtract(
                context.GetInput<FingerprintWorkflowInput>().StartTime).TotalSeconds;
        }
        catch (Exception ex)
        {
            results.Success = false;
            results.Error = ex.Message;
        }
        
        return results;
    }
    
    /// <summary>
    /// Déclencheur HTTP pour démarrer l'orchestration
    /// </summary>
    [Function("StartFingerprintWorkflow")]
    public static async Task<HttpResponseData> StartWorkflow(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client)
    {
        var input = await req.ReadFromJsonAsync<FingerprintWorkflowInput>();
        input.StartTime = DateTime.UtcNow;
        
        // Démarrer l'orchestration
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            "FingerprintOrchestrator",
            input);
        
        // Retourner les URL de vérification de statut
        var response = req.CreateResponse(System.Net.HttpStatusCode.Accepted);
        response.Headers.Add("Location", $"/api/status/{instanceId}");
        
        await response.WriteAsJsonAsync(new
        {
            instanceId,
            statusQueryGetUri = $"/api/status/{instanceId}",
            message = "Flux de travail d'empreinte démarré"
        });
        
        return response;
    }
}

public class FingerprintWorkflowInput
{
    public string VideoId { get; set; }
    public string VideoUri { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public DateTime StartTime { get; set; }
}

public class FingerprintWorkflowResult
{
    public bool Success { get; set; }
    public string FingerprintUri { get; set; }
    public double ProcessingTime { get; set; }
    public string Error { get; set; }
}
```

## Intégration AWS

### Schémas de stockage d'empreintes avec AWS S3

Amazon S3 fournit un stockage d'objets hautement durable pour les empreintes et les vidéos.

#### Implémentation du stockage S3

```csharp
using Amazon.S3;
using Amazon.S3.Model;
using System.Text.Json;

public class S3FingerprintStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    
    public S3FingerprintStorage(string region, string bucketName)
    {
        _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(region));
        _bucketName = bucketName;
        
        // Vérifier que le bucket existe
        EnsureBucketExistsAsync().Wait();
    }
    
    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            await _s3Client.HeadBucketAsync(new HeadBucketRequest { BucketName = _bucketName });
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await _s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _bucketName,
                BucketRegion = S3Region.USEast1
            });
            
            // Activer le versioning pour la protection des données
            await _s3Client.PutBucketVersioningAsync(new PutBucketVersioningRequest
            {
                BucketName = _bucketName,
                VersioningConfig = new S3BucketVersioningConfig
                {
                    Status = VersionStatus.Enabled
                }
            });
        }
    }
    
    /// <summary>
    /// Envoyer une empreinte avec le tiering intelligent S3
    /// </summary>
    public async Task<string> UploadFingerprintAsync(
        VFPFingerPrint fingerprint,
        string videoId,
        Dictionary<string, string> metadata = null)
    {
        // Créer la clé S3 avec partitionnement par date pour une meilleure organisation
        string s3Key = $"fingerprints/{DateTime.UtcNow:yyyy/MM/dd}/{videoId}.vfp";
        
        // Sérialiser l'empreinte
        using var stream = new MemoryStream();
        fingerprint.Save(stream);
        stream.Position = 0;
        
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = s3Key,
            InputStream = stream,
            ContentType = "application/octet-stream",
            StorageClass = S3StorageClass.IntelligentTiering, // Optimisation automatique des coûts
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            Metadata = 
            {
                ["video-id"] = videoId,
                ["duration"] = fingerprint.Duration.TotalSeconds.ToString(),
                ["resolution"] = $"{fingerprint.Width}x{fingerprint.Height}",
                ["created-utc"] = DateTime.UtcNow.ToString("O")
            }
        };
        
        // Ajouter les métadonnées personnalisées
        if (metadata != null)
        {
            foreach (var kvp in metadata)
            {
                putRequest.Metadata[kvp.Key] = kvp.Value;
            }
        }
        
        // Ajouter les tags pour la gestion du cycle de vie
        putRequest.TagSet = new List<Tag>
        {
            new Tag { Key = "Type", Value = "Fingerprint" },
            new Tag { Key = "Version", Value = "1.0" },
            new Tag { Key = "Environment", Value = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production" }
        };
        
        var response = await _s3Client.PutObjectAsync(putRequest);
        
        // Retourner l'URI S3
        return $"s3://{_bucketName}/{s3Key}";
    }
    
    /// <summary>
    /// Télécharger une empreinte avec prise en charge de cache
    /// </summary>
    public async Task<VFPFingerPrint> DownloadFingerprintAsync(string s3Uri, bool useCache = true)
    {
        // Analyser l'URI S3
        var uri = new Uri(s3Uri);
        string key = uri.AbsolutePath.TrimStart('/');
        
        // Vérifier d'abord le cache local
        if (useCache)
        {
            var cached = GetFromCache(key);
            if (cached != null)
                return cached;
        }
        
        var getRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };
        
        using var response = await _s3Client.GetObjectAsync(getRequest);
        using var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream);
        
        memoryStream.Position = 0;
        var fingerprint = VFPFingerPrint.Load(memoryStream);
        
        // Ajouter au cache
        if (useCache)
        {
            AddToCache(key, fingerprint);
        }
        
        return fingerprint;
    }
    
    /// <summary>
    /// Envoi par lot d'empreintes avec S3 Transfer Utility
    /// </summary>
    public async Task<List<string>> BatchUploadFingerprintsAsync(
        Dictionary<string, VFPFingerPrint> fingerprints)
    {
        var uploadTasks = new List<Task<string>>();
        
        // Utiliser S3 Transfer Utility pour des envois optimisés
        using var transferUtility = new Amazon.S3.Transfer.TransferUtility(_s3Client);
        
        foreach (var kvp in fingerprints)
        {
            uploadTasks.Add(Task.Run(async () =>
            {
                using var stream = new MemoryStream();
                kvp.Value.Save(stream);
                stream.Position = 0;
                
                var uploadRequest = new Amazon.S3.Transfer.TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    Key = $"fingerprints/batch/{DateTime.UtcNow:yyyy-MM-dd}/{kvp.Key}.vfp",
                    InputStream = stream,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    PartSize = 5 * 1024 * 1024 // Parts de 5 Mo pour l'envoi multipart
                };
                
                await transferUtility.UploadAsync(uploadRequest);
                
                return $"s3://{_bucketName}/{uploadRequest.Key}";
            }));
        }
        
        return (await Task.WhenAll(uploadTasks)).ToList();
    }
    
    /// <summary>
    /// Interroger les empreintes via S3 Select
    /// </summary>
    public async Task<List<FingerprintQueryResult>> QueryFingerprintsAsync(
        string sqlExpression)
    {
        // Créer d'abord un inventaire des empreintes au format JSON
        var listRequest = new ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = "fingerprints/"
        };
        
        var objects = new List<S3Object>();
        ListObjectsV2Response listResponse;
        
        do
        {
            listResponse = await _s3Client.ListObjectsV2Async(listRequest);
            objects.AddRange(listResponse.S3Objects);
            listRequest.ContinuationToken = listResponse.NextContinuationToken;
        } while (listResponse.IsTruncated);
        
        // Utiliser S3 Select pour les requêtes complexes
        var selectRequest = new SelectObjectContentRequest
        {
            BucketName = _bucketName,
            Key = "fingerprints/inventory.json", // En supposant que nous maintenons un inventaire
            Expression = sqlExpression,
            ExpressionType = ExpressionType.SQL,
            InputSerialization = new InputSerialization
            {
                JSON = new JSONInput
                {
                    JsonType = JsonType.Lines
                }
            },
            OutputSerialization = new OutputSerialization
            {
                JSON = new JSONOutput()
            }
        };
        
        var results = new List<FingerprintQueryResult>();
        
        try
        {
            using var selectResponse = await _s3Client.SelectObjectContentAsync(selectRequest);
            
            foreach (var ev in selectResponse.Payload)
            {
                if (ev is RecordsEvent records)
                {
                    using var reader = new StreamReader(records.Payload);
                    var json = await reader.ReadToEndAsync();
                    var result = JsonSerializer.Deserialize<FingerprintQueryResult>(json);
                    results.Add(result);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Échec de la requête S3 Select : {ex.Message}");
        }
        
        return results;
    }
    
    private VFPFingerPrint GetFromCache(string key)
    {
        // Implémenter la logique de cache local
        // Pourrait utiliser MemoryCache, Redis ou un cache basé fichier
        return null;
    }
    
    private void AddToCache(string key, VFPFingerPrint fingerprint)
    {
        // Ajouter à l'implémentation du cache
    }
}

public class FingerprintQueryResult
{
    public string VideoId { get; set; }
    public string S3Uri { get; set; }
    public double Duration { get; set; }
    public DateTime CreatedUtc { get; set; }
}
```

### Implémentation AWS Lambda

AWS Lambda fournit du calcul serverless pour traiter les vidéos à grande échelle.

#### Configuration de la fonction Lambda

```csharp
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

// Attribut d'assembly pour activer la fonction Lambda
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

public class FingerprintLambdaFunction
{
    private readonly S3FingerprintStorage _storage;
    
    public FingerprintLambdaFunction()
    {
        // Initialiser dans le constructeur pour la réutilisation du conteneur Lambda
        _storage = new S3FingerprintStorage(
            Environment.GetEnvironmentVariable("AWS_REGION"),
            Environment.GetEnvironmentVariable("S3_BUCKET")
        );
        
        VFPAnalyzer.SetLicenseKey(Environment.GetEnvironmentVariable("VFP_LICENSE_KEY"));
    }
    
    /// <summary>
    /// Lambda déclenchée par API Gateway pour le traitement synchrone
    /// </summary>
    public async Task<APIGatewayProxyResponse> HandleApiRequest(
        APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        context.Logger.LogLine($"Traitement de la requête API : {request.RequestContext.RequestId}");
        
        try
        {
            var fingerprintRequest = JsonSerializer.Deserialize<FingerprintRequest>(request.Body);
            
            // Traitement avec prise en compte du timeout
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(context.RemainingTime.TotalMilliseconds - 5000)); // Marge de 5 s
            
            var result = await ProcessFingerprintAsync(fingerprintRequest, cts.Token);
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" },
                Body = JsonSerializer.Serialize(result)
            };
        }
        catch (OperationCanceledException)
        {
            // Lambda sur le point d'expirer, retourner un résultat partiel
            return new APIGatewayProxyResponse
            {
                StatusCode = 202,
                Body = JsonSerializer.Serialize(new { status = "Processing", message = "Requête mise en file pour traitement asynchrone" })
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Erreur : {ex.Message}");
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new { error = ex.Message })
            };
        }
    }
    
    /// <summary>
    /// Lambda déclenchée par SQS pour le traitement asynchrone par lots
    /// </summary>
    public async Task HandleSQSBatch(SQSEvent sqsEvent, ILambdaContext context)
    {
        var tasks = new List<Task>();
        
        foreach (var record in sqsEvent.Records)
        {
            tasks.Add(ProcessSQSMessageAsync(record, context));
        }
        
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessSQSMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        try
        {
            var request = JsonSerializer.Deserialize<FingerprintRequest>(message.Body);
            
            context.Logger.LogLine($"Traitement de la vidéo : {request.VideoId}");
            
            // Télécharger la vidéo depuis S3
            var videoPath = await DownloadFromS3Async(request.VideoUri);
            
            // Les méthodes VFPAnalyzer prennent un chemin de fichier et des callbacks optionnels
            var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                videoPath,
                null); // callback de progression
            
            if (fingerprint != null)
            {
                // Envoyer vers S3
                var fingerprintUri = await _storage.UploadFingerprintAsync(
                    fingerprint,
                    request.VideoId,
                    new Dictionary<string, string>
                    {
                        ["processed-by"] = context.FunctionName,
                        ["request-id"] = context.RequestId
                    }
                );
                
                // Envoyer la notification de succès
                await SendNotificationAsync(request.VideoId, true, fingerprintUri);
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Erreur de traitement du message : {ex.Message}");
            
            // Vérifier si nous devons réessayer
            var receiveCount = int.Parse(message.Attributes.GetValueOrDefault("ApproximateReceiveCount", "1"));
            
            if (receiveCount >= 3)
            {
                // Déplacer vers DLQ après 3 tentatives
                await SendToDLQAsync(message, ex.Message);
            }
            else
            {
                throw; // Relancer pour déclencher la nouvelle tentative
            }
        }
    }
    
    private async Task<FingerprintResult> ProcessFingerprintAsync(
        FingerprintRequest request,
        CancellationToken cancellationToken)
    {
        // Implémentation du traitement d'empreinte
        // Similaire aux exemples précédents mais avec optimisations spécifiques Lambda
        
        return new FingerprintResult
        {
            Success = true,
            VideoId = request.VideoId,
            FingerprintUri = "s3://bucket/path/to/fingerprint.vfp"
        };
    }
    
    private async Task<string> DownloadFromS3Async(string s3Uri)
    {
        // Télécharger la vidéo dans le répertoire /tmp de Lambda
        var tempPath = $"/tmp/{Guid.NewGuid()}.mp4";
        
        // Implémentation pour télécharger depuis S3
        
        return tempPath;
    }
    
    private async Task SendNotificationAsync(string videoId, bool success, string fingerprintUri)
    {
        // Envoyer une notification SNS ou mettre à jour DynamoDB
    }
    
    private async Task SendToDLQAsync(SQSEvent.SQSMessage message, string error)
    {
        // Envoyer vers la dead letter queue pour examen manuel
    }
}

public class FingerprintResult
{
    public bool Success { get; set; }
    public string VideoId { get; set; }
    public string FingerprintUri { get; set; }
    public double ProcessingTime { get; set; }
}
```

#### Step Functions pour les flux de travail complexes

```csharp
// Définition de la machine d'état pour AWS Step Functions
public class StepFunctionsWorkflow
{
    public static string GetStateMachineDefinition()
    {
        return @"
        {
          ""Comment"": ""Flux de travail d'empreinte vidéo avec traitement parallèle"",
          ""StartAt"": ""ValidateInput"",
          ""States"": {
            ""ValidateInput"": {
              ""Type"": ""Task"",
              ""Resource"": ""arn:aws:lambda:us-east-1:123456789:function:ValidateVideo"",
              ""Next"": ""CheckVideoSize"",
              ""Catch"": [{
                ""ErrorEquals"": [""ValidationError""],
                ""Next"": ""ValidationFailed""
              }]
            },
            ""CheckVideoSize"": {
              ""Type"": ""Choice"",
              ""Choices"": [{
                ""Variable"": ""$.videoSize"",
                ""NumericGreaterThan"": 1073741824,
                ""Next"": ""SplitVideo""
              }],
              ""Default"": ""ProcessSingleVideo""
            },
            ""SplitVideo"": {
              ""Type"": ""Task"",
              ""Resource"": ""arn:aws:lambda:us-east-1:123456789:function:SplitVideo"",
              ""Next"": ""ProcessSegments""
            },
            ""ProcessSegments"": {
              ""Type"": ""Map"",
              ""ItemsPath"": ""$.segments"",
              ""MaxConcurrency"": 10,
              ""Iterator"": {
                ""StartAt"": ""GenerateSegmentFingerprint"",
                ""States"": {
                  ""GenerateSegmentFingerprint"": {
                    ""Type"": ""Task"",
                    ""Resource"": ""arn:aws:lambda:us-east-1:123456789:function:GenerateFingerprint"",
                    ""End"": true
                  }
                }
              },
              ""Next"": ""MergeFingerprints""
            },
            ""MergeFingerprints"": {
              ""Type"": ""Task"",
              ""Resource"": ""arn:aws:lambda:us-east-1:123456789:function:MergeFingerprints"",
              ""Next"": ""StoreResult""
            },
            ""ProcessSingleVideo"": {
              ""Type"": ""Task"",
              ""Resource"": ""arn:aws:lambda:us-east-1:123456789:function:GenerateFingerprint"",
              ""Next"": ""StoreResult""
            },
            ""StoreResult"": {
              ""Type"": ""Task"",
              ""Resource"": ""arn:aws:lambda:us-east-1:123456789:function:StoreFingerprint"",
              ""Next"": ""SendNotification""
            },
            ""SendNotification"": {
              ""Type"": ""Task"",
              ""Resource"": ""arn:aws:sns:us-east-1:123456789:FingerprintComplete"",
              ""End"": true
            },
            ""ValidationFailed"": {
              ""Type"": ""Fail"",
              ""Error"": ""ValidationError"",
              ""Cause"": ""La validation d'entrée a échoué""
            }
          }
        }";
    }
}
```

## Schémas de traitement distribué

### Architecture basée sur file d'attente

```csharp
public class DistributedFingerprintProcessor
{
    private readonly IMessageQueue _queue;
    private readonly IDistributedCache _cache;
    private readonly VFPFingerPrintDB _localDb; // Base d'empreintes locale
    // Note : le SDK fournit VFPFingerPrintDB pour le stockage local
    
    /// <summary>
    /// Producteur — ajoute les vidéos à la file de traitement
    /// </summary>
    public async Task QueueVideoForProcessingAsync(string videoUri, ProcessingPriority priority = ProcessingPriority.Normal)
    {
        var message = new ProcessingMessage
        {
            Id = Guid.NewGuid().ToString(),
            VideoUri = videoUri,
            Priority = priority,
            QueuedAt = DateTime.UtcNow,
            RetryCount = 0
        };
        
        // Ajouter à la file appropriée selon la priorité
        string queueName = priority switch
        {
            ProcessingPriority.High => "fingerprint-high-priority",
            ProcessingPriority.Low => "fingerprint-low-priority",
            _ => "fingerprint-normal-priority"
        };
        
        await _queue.SendMessageAsync(queueName, message);
        
        // Suivre dans le cache distribué
        await _cache.SetAsync($"processing:{message.Id}", "queued", TimeSpan.FromHours(24));
    }
    
    /// <summary>
    /// Consommateur — traite les vidéos depuis la file
    /// </summary>
    public async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Obtenir le message de la file
                var message = await _queue.ReceiveMessageAsync<ProcessingMessage>("fingerprint-normal-priority");
                
                if (message == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    continue;
                }
                
                // Mettre à jour le statut
                await _cache.SetAsync($"processing:{message.Content.Id}", "processing");
                
                // Traiter l'empreinte
                var result = await ProcessVideoAsync(message.Content);
                
                if (result.Success)
                {
                    // Marquer comme terminé
                    await _queue.DeleteMessageAsync(message);
                    await _cache.SetAsync($"processing:{message.Content.Id}", "completed");
                }
                else
                {
                    // Gérer l'échec
                    await HandleFailureAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de traitement : {ex.Message}");
            }
        }
    }
    
    private async Task<ProcessingResult> ProcessVideoAsync(ProcessingMessage message)
    {
        try
        {
            // Télécharger la vidéo
            var videoPath = await DownloadVideoAsync(message.VideoUri);
            
            // Générer l'empreinte avec limites de ressources
            using var semaphore = new SemaphoreSlim(1, 1); // Limiter le traitement concurrent
            await semaphore.WaitAsync();
            
            try
            {
                var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                    videoPath,
                    null); // callback de progression
                
                if (fingerprint != null)
                {
                    // Stocker l'empreinte
                    var uri = await _storage.UploadAsync(fingerprint, message.Id);
                    
                    return new ProcessingResult
                    {
                        Success = true,
                        FingerprintUri = uri
                    };
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            return new ProcessingResult
            {
                Success = false,
                Error = ex.Message
            };
        }
        
        return new ProcessingResult { Success = false };
    }
}

public enum ProcessingPriority
{
    Low,
    Normal,
    High
}

public class ProcessingMessage
{
    public string Id { get; set; }
    public string VideoUri { get; set; }
    public ProcessingPriority Priority { get; set; }
    public DateTime QueuedAt { get; set; }
    public int RetryCount { get; set; }
}
```

### Traitement par conteneurs avec Kubernetes

```yaml
# Déploiement Kubernetes pour le traitement d'empreintes
apiVersion: apps/v1
kind: Deployment
metadata:
  name: fingerprint-processor
spec:
  replicas: 3
  selector:
    matchLabels:
      app: fingerprint-processor
  template:
    metadata:
      labels:
        app: fingerprint-processor
    spec:
      containers:
      - name: processor
        image: myregistry/fingerprint-processor:latest
        resources:
          requests:
            memory: "2Gi"
            cpu: "1"
          limits:
            memory: "4Gi"
            cpu: "2"
        env:
        - name: VFP_LICENSE_KEY
          valueFrom:
            secretKeyRef:
              name: vfp-secrets
              key: license-key
        - name: STORAGE_CONNECTION
          valueFrom:
            secretKeyRef:
              name: storage-secrets
              key: connection-string
---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: fingerprint-processor-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: fingerprint-processor
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```
## Stratégies d'optimisation des coûts
### 1. Optimisation du stockage
```csharp
public class CostOptimizedStorage
{
    /// <summary>
    /// Compresser les empreintes avant stockage
    /// </summary>
    public async Task<byte[]> CompressFingerprintAsync(VFPFingerPrint fingerprint)
    {
        using var originalStream = new MemoryStream();
        fingerprint.Save(originalStream);
        using var compressedStream = new MemoryStream();
        using (var gzipStream = new System.IO.Compression.GZipStream(
            compressedStream, 
            System.IO.Compression.CompressionLevel.Optimal))
        {
            await originalStream.CopyToAsync(gzipStream);
        }
        var compressed = compressedStream.ToArray();
        // Journaliser le ratio de compression
        double ratio = (double)compressed.Length / originalStream.Length;
        Console.WriteLine($"Ratio de compression : {ratio:P2}");
        return compressed;
    }
    /// <summary>
    /// Implémenter une mise en cache intelligente
    /// </summary>
    public class TieredCache
    {
        private readonly MemoryCache _l1Cache; // Cache chaud
        private readonly IDistributedCache _l2Cache; // Cache tiède (Redis)
        private readonly VFPFingerPrintDB _coldStorage; // Base de stockage froid
        public async Task<VFPFingerPrint> GetFingerprintAsync(string id)
        {
            // Vérifier le cache L1
            if (_l1Cache.TryGetValue(id, out VFPFingerPrint fingerprint))
            {
                return fingerprint;
            }
            // Vérifier le cache L2
            var cached = await _l2Cache.GetAsync(id);
            if (cached != null)
            {
                fingerprint = DeserializeFingerprint(cached);
                _l1Cache.Set(id, fingerprint, TimeSpan.FromMinutes(5));
                return fingerprint;
            }
            // Récupérer depuis le stockage froid
            fingerprint = await _l3Storage.DownloadAsync(id);
            // Alimenter les caches
            await _l2Cache.SetAsync(id, SerializeFingerprint(fingerprint), TimeSpan.FromHours(1));
            _l1Cache.Set(id, fingerprint, TimeSpan.FromMinutes(5));
            return fingerprint;
        }
    }
}
```
### 2. Optimisation du traitement
```csharp
public class ProcessingCostOptimizer
{
    /// <summary>
    /// Utiliser des instances spot/préemptives pour le traitement par lots
    /// </summary>
    public async Task<bool> ProcessWithSpotInstancesAsync(List<string> videoUris)
    {
        // Vérifier la disponibilité et le prix des instances spot
        var spotPrice = await GetCurrentSpotPriceAsync();
        var onDemandPrice = await GetOnDemandPriceAsync();
        if (spotPrice < onDemandPrice * 0.5) // Seuil d'économie de 50 %
        {
            // Lancer les instances spot
            await LaunchSpotInstancesAsync(videoUris.Count / 10); // 10 vidéos par instance
            // Traiter avec gestion des interruptions
            return await ProcessWithInterruptionHandlingAsync(videoUris);
        }
        // Repli sur on-demand ou serverless
        return await ProcessWithServerlessAsync(videoUris);
    }
    /// <summary>
    /// Traitement par lots pour l'efficacité
    /// </summary>
    public async Task BatchProcessAsync(List<string> videoUris, int batchSize = 50)
    {
        var batches = videoUris
            .Select((uri, index) => new { uri, index })
            .GroupBy(x => x.index / batchSize)
            .Select(g => g.Select(x => x.uri).ToList());
        foreach (var batch in batches)
        {
            // Traiter le lot en parallèle
            var tasks = batch.Select(uri => Task.Run(() => ProcessVideoAsync(uri)));
            await Task.WhenAll(tasks);
            // Petit délai entre les lots pour éviter le throttling
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
    /// <summary>
    /// Qualité adaptative selon les exigences
    /// </summary>
    public VFPFingerprintSource GetOptimizedSource(
        string videoPath,
        ProcessingTier tier)
    {
        // Note : VFPFingerprintSource sert à spécifier les sources de fichiers vidéo
        // Traiter la vidéo différemment selon le palier avant l'empreinte
        // VFPFingerprintSource n'a pas de propriété FrameRate — la fréquence d'images
        // est déterminée par la source/le décodeur sous-jacent. CustomResolution doit
        // être un VisioForge.Core.Types.Size (pas System.Drawing.Size).
        return tier switch
        {
            ProcessingTier.Economy => new VFPFingerprintSource(videoPath)
            {
                CustomResolution = new VisioForge.Core.Types.Size(320, 240),
                StopTime = TimeSpan.FromSeconds(30)
            },
            ProcessingTier.Standard => new VFPFingerprintSource(videoPath)
            {
                CustomResolution = new VisioForge.Core.Types.Size(640, 480),
                StopTime = TimeSpan.FromSeconds(60)
            },
            ProcessingTier.Premium => new VFPFingerprintSource(videoPath)
            {
                CustomResolution = new VisioForge.Core.Types.Size(1280, 720)
            },
            _ => throw new ArgumentException("Palier de traitement invalide")
        };
    }
}
public enum ProcessingTier
{
    Economy,  // Coût le plus bas, précision réduite
    Standard, // Équilibre coût/précision
    Premium   // Précision maximale, coût plus élevé
}
```
### 3. Surveillance des coûts et alertes
```csharp
// IMPLÉMENTATION D'EXEMPLE — schéma de surveillance des coûts
public class CostMonitor
{
    private readonly CloudCostTracker _costTracker;
    /// <summary>
    /// Suivre les coûts par opération
    /// </summary>
    public async Task<CostReport> TrackOperationCostAsync(
        Func<Task> operation,
        string operationType)
    {
        var startTime = DateTime.UtcNow;
        var startResources = await GetResourceUsageAsync();
        await operation();
        var endTime = DateTime.UtcNow;
        var endResources = await GetResourceUsageAsync();
        var cost = CalculateCost(startResources, endResources, endTime - startTime);
        // Journaliser dans le système de surveillance
        await _costTracker.LogCostAsync(new CostEntry
        {
            OperationType = operationType,
            Duration = endTime - startTime,
            ComputeCost = cost.ComputeCost,
            StorageCost = cost.StorageCost,
            TransferCost = cost.TransferCost,
            TotalCost = cost.TotalCost
        });
        // Alerter si le coût dépasse le seuil
        if (cost.TotalCost > GetCostThreshold(operationType))
        {
            await SendCostAlertAsync(operationType, cost);
        }
        return cost;
    }
    /// <summary>
    /// Optimiser selon les schémas de coût
    /// </summary>
    public async Task<CostOptimizationRecommendations> AnalyzeCostPatternsAsync(
        DateTime startDate,
        DateTime endDate)
    {
        var costs = await _costTracker.GetCostsAsync(startDate, endDate);
        var recommendations = new CostOptimizationRecommendations();
        // Analyser les coûts de calcul
        var avgComputeCost = costs.Average(c => c.ComputeCost);
        if (avgComputeCost > 100) // Seuil de 100 $
        {
            recommendations.Add("Envisager des instances spot pour le traitement par lots");
            recommendations.Add("Implémenter la mise à l'échelle automatique pour réduire le calcul inactif");
        }
        // Analyser les coûts de stockage
        var totalStorageCost = costs.Sum(c => c.StorageCost);
        if (totalStorageCost > 500) // Seuil de 500 $
        {
            recommendations.Add("Déplacer les anciennes empreintes vers le stockage d'archive");
            recommendations.Add("Implémenter la compression pour le stockage des empreintes");
            recommendations.Add("Examiner et supprimer les empreintes inutilisées");
        }
        // Analyser les coûts de transfert
        var totalTransferCost = costs.Sum(c => c.TransferCost);
        if (totalTransferCost > 200) // Seuil de 200 $
        {
            recommendations.Add("Utiliser un CDN pour les empreintes fréquemment accédées");
            recommendations.Add("Traiter les vidéos dans la même région que le stockage");
        }
        return recommendations;
    }
}
```
## Exemples d'architecture serverless
### Solution serverless complète
```csharp
public class ServerlessFingerprintingArchitecture
{
    /// <summary>
    /// API Gateway + Lambda + DynamoDB + S3
    /// </summary>
    public class ServerlessApi
    {
        // Gestionnaire de point de terminaison API
        public async Task<APIGatewayProxyResponse> HandleRequest(
            APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return request.HttpMethod switch
            {
                "POST" => await CreateFingerprintAsync(request, context),
                "GET" => await GetFingerprintAsync(request, context),
                "DELETE" => await DeleteFingerprintAsync(request, context),
                _ => new APIGatewayProxyResponse { StatusCode = 405 }
            };
        }
        private async Task<APIGatewayProxyResponse> CreateFingerprintAsync(
            APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            var body = JsonSerializer.Deserialize<CreateFingerprintRequest>(request.Body);
            // Valider la requête
            if (string.IsNullOrEmpty(body?.VideoUrl))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = JsonSerializer.Serialize(new { error = "VideoUrl est requis" })
                };
            }
            // Générer un ID unique
            var fingerprintId = Guid.NewGuid().ToString();
            // Stocker la requête dans DynamoDB
            await StoreRequestAsync(fingerprintId, body);
            // Mettre en file pour traitement asynchrone
            await QueueForProcessingAsync(fingerprintId, body.VideoUrl);
            // Retourner la réponse immédiate
            return new APIGatewayProxyResponse
            {
                StatusCode = 202,
                Headers = new Dictionary<string, string>
                {
                    ["Location"] = $"/fingerprints/{fingerprintId}"
                },
                Body = JsonSerializer.Serialize(new
                {
                    id = fingerprintId,
                    status = "processing",
                    statusUrl = $"/fingerprints/{fingerprintId}/status"
                })
            };
        }
    }
    /// <summary>
    /// Traitement piloté par événement avec Step Functions
    /// </summary>
    public class EventDrivenProcessor
    {
        public async Task ProcessS3UploadEvent(S3Event s3Event, ILambdaContext context)
        {
            foreach (var record in s3Event.Records)
            {
                if (record.EventName.StartsWith("ObjectCreated"))
                {
                    var bucket = record.S3.Bucket.Name;
                    var key = record.S3.Object.Key;
                    // Vérifier s'il s'agit d'un fichier vidéo
                    if (IsVideoFile(key))
                    {
                        // Démarrer l'exécution Step Functions
                        await StartWorkflowAsync(bucket, key);
                    }
                }
            }
        }
        private bool IsVideoFile(string key)
        {
            var videoExtensions = new[] { ".mp4", ".avi", ".mov", ".mkv", ".wmv" };
            return videoExtensions.Any(ext => key.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
```
## Bonnes pratiques de sécurité
### 1. Chiffrement et contrôle d'accès
```csharp
public class SecureFingerprintStorage
{
    /// <summary>
    /// Chiffrer les empreintes au repos et en transit
    /// </summary>
    public async Task<string> StoreSecurelyAsync(VFPFingerPrint fingerprint, string videoId)
    {
        // Chiffrer les données d'empreinte
        var encryptedData = await EncryptDataAsync(fingerprint);
        // Stocker avec chiffrement
        var storageClient = new BlobServiceClient(
            new Uri("https://storage.blob.core.windows.net"),
            new DefaultAzureCredential()); // Utiliser l'identité managée
        var blobClient = storageClient
            .GetBlobContainerClient("secure-fingerprints")
            .GetBlobClient($"{videoId}.vfp.encrypted");
        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "application/octet-stream"
            },
            Metadata = new Dictionary<string, string>
            {
                ["encrypted"] = "true",
                ["algorithm"] = "AES256",
                ["videoId"] = videoId
            }
        };
        await blobClient.UploadAsync(new BinaryData(encryptedData), uploadOptions);
        // Journal d'audit
        await LogAccessAsync("STORE", videoId, "SUCCESS");
        return blobClient.Uri.ToString();
    }
    /// <summary>
    /// Implémenter un contrôle d'accès basé sur les rôles
    /// </summary>
    public async Task<bool> AuthorizeAccessAsync(string userId, string resource, string action)
    {
        // Vérifier les permissions de l'utilisateur
        var permissions = await GetUserPermissionsAsync(userId);
        return action switch
        {
            "READ" => permissions.Contains("fingerprint:read"),
            "WRITE" => permissions.Contains("fingerprint:write"),
            "DELETE" => permissions.Contains("fingerprint:delete"),
            _ => false
        };
    }
}
```
### 2. Sécurité réseau
```csharp
public class NetworkSecurityConfig
{
    /// <summary>
    /// Configurer les points de terminaison VPC pour les communications privées
    /// </summary>
    public static void ConfigurePrivateEndpoints()
    {
        // Utiliser des points de terminaison VPC pour éviter le routage internet
        Environment.SetEnvironmentVariable("AWS_S3_ENDPOINT", "https://s3.vpc-endpoint.amazonaws.com");
        Environment.SetEnvironmentVariable("AZURE_STORAGE_ENDPOINT", "https://storage.privatelink.blob.core.windows.net");
    }
    /// <summary>
    /// Implémenter la limitation d'API
    /// </summary>
    public class ApiThrottling
    {
        private readonly IMemoryCache _cache;
        public async Task<bool> CheckRateLimitAsync(string clientId)
        {
            var key = $"ratelimit:{clientId}";
            var requests = await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return 0;
            });
            if (requests >= 100) // 100 requêtes par minute
            {
                return false;
            }
            _cache.Set(key, requests + 1);
            return true;
        }
    }
}
```
## Benchmarks de performance
### Comparaison des fournisseurs cloud
| Métrique | Azure Functions | AWS Lambda | Google Cloud Run |
|--------|----------------|------------|------------------|
| Démarrage à froid | 1 à 3 secondes | 1 à 2 secondes | 2 à 4 secondes |
| Démarrage à chaud | 100 à 200 ms | 50 à 100 ms | 200 à 300 ms |
| Temps d'exécution max | 10 minutes | 15 minutes | 60 minutes |
| Mémoire max | 14 Go | 10 Go | 32 Go |
| Coût par million de requêtes | 0,20 $ | 0,20 $ | 0,40 $ |
| Coût par Go-seconde | 0,000016 $ | 0,0000166 $ | 0,0000025 $ |
### Performance de traitement
```csharp
public class PerformanceBenchmarks
{
    public static async Task RunBenchmarksAsync()
    {
        var results = new List<BenchmarkResult>();
        // Tester différentes configurations
        // VFPFingerprintSource n'a pas de propriété FrameRate ; CustomResolution est
        // un VisioForge.Core.Types.Size, pas System.Drawing.Size.
        var configurations = new[]
        {
            new { Resolution = new VisioForge.Core.Types.Size(320, 240), Label = "Economy" },
            new { Resolution = new VisioForge.Core.Types.Size(640, 480), Label = "Standard" },
            new { Resolution = new VisioForge.Core.Types.Size(1280, 720), Label = "Premium" }
        };
        foreach (var config in configurations)
        {
            var sw = Stopwatch.StartNew();
            var source = new VFPFingerprintSource("test.mp4")
            {
                CustomResolution = config.Resolution
            };
            var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
            sw.Stop();
            results.Add(new BenchmarkResult
            {
                Configuration = config.Label,
                ProcessingTime = sw.Elapsed,
                FingerprintSize = fingerprint?.Data?.Length ?? 0,
                MemoryUsed = GC.GetTotalMemory(false) / (1024 * 1024) // Mo
            });
        }
        // Afficher les résultats
        foreach (var result in results)
        {
            Console.WriteLine($"{result.Configuration} :");
            Console.WriteLine($"  Temps de traitement : {result.ProcessingTime.TotalSeconds:F2} s");
            Console.WriteLine($"  Taille de l'empreinte : {result.FingerprintSize / 1024} Ko");
            Console.WriteLine($"  Mémoire utilisée : {result.MemoryUsed} Mo");
        }
    }
}
```
## Dépannage des déploiements cloud
### Problèmes courants et solutions
1. **Problèmes de timeout Lambda**
   - Augmenter les paramètres de timeout
   - Optimiser la résolution vidéo et la fréquence d'images
   - Utiliser Step Functions pour les processus de longue durée
2. **Erreurs d'accès au stockage**
   - Vérifier les rôles et permissions IAM
   - Vérifier la connectivité réseau et les paramètres VPC
   - Assurer une configuration SDK appropriée
3. **Dépassements de coûts**
   - Implémenter des politiques de cycle de vie appropriées
   - Utiliser des instances spot pour le traitement par lots
   - Surveiller et alerter sur les schémas d'utilisation inhabituels
4. **Dégradation des performances**
   - Mettre à l'échelle les ressources de calcul de façon appropriée
   - Implémenter des stratégies de mise en cache
   - Utiliser un CDN pour le contenu fréquemment accédé
## Résumé
L'empreinte vidéo basée sur le cloud offre une évolutivité et une rentabilité sans précédent. Points clés à retenir :
- **Choisir le bon service** : serverless pour les charges variables, conteneurs pour le traitement constant
- **Optimiser les coûts** : utiliser le stockage par paliers, les instances spot et la mise en cache intelligente
- **Assurer la sécurité** : chiffrer les données, utiliser les identités managées, mettre en place des contrôles d'accès
- **Surveiller les performances** : suivre les coûts, configurer des alertes et optimiser en continu
- **Planifier pour l'échelle** : concevoir pour le traitement distribué dès le départ
## Étapes suivantes
- Explorer l'[intégration de base de données](database-integration.md) pour stocker les empreintes
## Ressources supplémentaires
- [Documentation Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/)
- [Documentation AWS Lambda](https://docs.aws.amazon.com/lambda/latest/dg/welcome.html)
- [Bonnes pratiques de stockage cloud](https://cloud.google.com/storage/docs/best-practices)
- [Schémas d'architecture serverless](https://serverlessland.com/patterns)
