---
title: FAQ Video Fingerprinting — licences, précision, formats
description: Trouvez des réponses sur le VisioForge Video Fingerprinting SDK : licences, performance, précision, formats et compatibilité de plateformes.
tags:
  - Video Fingerprinting SDK
  - .NET
  - C++
  - Windows
  - macOS
  - Linux
  - Fingerprinting
primary_api_classes:
  - VFPAnalyzer
  - VFPFingerprintSource
  - ParallelProcessor

---

# FAQ du Video Fingerprinting SDK

Cette FAQ complète répond aux questions les plus fréquentes sur le VisioForge Video Fingerprinting SDK. Si vous ne trouvez pas votre réponse ici, consultez notre [forum de support](https://support.visioforge.com/) ou notre [communauté Discord](https://discord.com/invite/yvXUG56WCH).

## Table des matières

- [Questions de licence](#licensing-questions)
- [Performance et optimisation](#performance-and-optimization)
- [Précision et détection](#accuracy-and-detection)
- [Formats et codecs pris en charge](#supported-formats-and-codecs)
- [Intégration et développement](#integration-and-development)
- [Base de données et stockage](#database-and-storage)

## Questions de licence { #licensing-questions }

### Q : quelle est la différence entre les licences d'essai et commerciale ?

**R :** les différences clés sont :

| Fonctionnalité | Essai | Commerciale |
|---------|-------|------------|
| Durée | 30 jours | Perpétuelle |
| Filigrane | Oui (superposition visuelle) | Non |
| Toutes les fonctionnalités | Oui | Oui |
| Usage en production | Non | Oui |
| Support technique | Forum uniquement | E-mail/prioritaire/téléphone |
| Mises à jour | Période d'essai uniquement | 1 an |

### Q : puis-je utiliser la licence d'essai pour le développement ?

**R :** oui ! La licence d'essai est parfaite pour le développement et les tests. Elle inclut toutes les fonctionnalités avec un filigrane. Vous pouvez développer votre application entière avec la licence d'essai et acheter une licence commerciale lorsque vous êtes prêt pour la production.

```csharp
// Environnement de développement
VFPAnalyzer.SetLicenseKey("TRIAL");

// Environnement de production
VFPAnalyzer.SetLicenseKey(Environment.GetEnvironmentVariable("VFP_LICENSE_KEY"));
```

### Q : que se passe-t-il lorsque mon essai expire ?

**R :** après 30 jours, le SDK cesse de traiter les vidéos et retourne une erreur. Votre code reste intact — il suffit d'acheter une licence et de mettre à jour la clé pour continuer.

### Q : existe-t-il des limitations fonctionnelles entre les licences ?

**R :** non, la licence commerciale inclut toutes les fonctionnalités sans limitation. La seule différence entre les licences d'essai et commerciale est le filigrane en mode essai.

### Q : puis-je utiliser une licence sur plusieurs machines ?

**R :** les conditions de licence dépendent de votre achat :
- **Licence développeur unique** : un développeur, machines de développement illimitées
- **Licence de site** : développeurs illimités sur un seul emplacement physique
- **Licence entreprise** : développeurs illimités sur plusieurs emplacements

Pour le déploiement, vous avez besoin d'une licence runtime pour chaque serveur de production ou application distribuée.

### Q : comment gérer la licence dans une application distribuée ?

**R :** pour les applications distribuées (installées sur les machines des clients), vous avez besoin de :

```csharp
public class LicenseManager
{
    private const string EncryptedLicense = "YOUR_ENCRYPTED_LICENSE";
    
    public static void Initialize()
    {
        // Déchiffrer la licence à l'exécution
        string licenseKey = DecryptLicense(EncryptedLicense);
        VFPAnalyzer.SetLicenseKey(licenseKey);
    }
    
    private static string DecryptLicense(string encrypted)
    {
        // Implémenter votre logique de déchiffrement
        // Ne jamais stocker de licences en clair dans des applis distribuées
        return Decrypt(encrypted);
    }
}
```

## Performance et optimisation { #performance-and-optimization }

### Q : à quelle vitesse le SDK peut-il traiter les vidéos ?

**R :** la vitesse de traitement dépend de plusieurs facteurs :

| Facteur | Impact sur la vitesse |
|--------|----------------|
| Résolution vidéo | 4K : ~0,5x temps réel, 1080p : ~2x temps réel, 480p : ~5x temps réel |
| Cœurs CPU | Mise à l'échelle linéaire jusqu'à 8 cœurs |
| Accélération matérielle | 2 à 5x plus rapide avec prise en charge GPU |
| Type de stockage | SSD apporte 30 à 50 % d'amélioration de vitesse |
| Type d'empreinte | Les empreintes de recherche sont 2x plus lentes que les empreintes de comparaison |

Benchmarks typiques sur du matériel moderne (Intel i7, 16 Go de RAM, SSD) :
- **Vidéo 1080p** : 60 à 120 secondes par heure de contenu
- **Vidéo 720p** : 30 à 60 secondes par heure de contenu
- **Vidéo 480p** : 15 à 30 secondes par heure de contenu

### Q : combien de mémoire la génération d'empreintes nécessite-t-elle ?

**R :** l'utilisation mémoire évolue avec la résolution vidéo et la durée :

```csharp
// Calcul approximatif de l'utilisation mémoire
long EstimateMemoryUsage(int width, int height, int durationSeconds)
{
    // Mémoire de base pour le décodeur et les tampons
    long baseMemory = 100 * 1024 * 1024; // 100 Mo
    
    // Mémoire des tampons d'image (3 à 5 images mises en tampon typiquement)
    long frameSize = width * height * 3; // RGB
    long frameBuffers = frameSize * 5;
    
    // Données d'empreinte (environ 1 Ko par seconde)
    long fingerprintSize = durationSeconds * 1024;
    
    return baseMemory + frameBuffers + fingerprintSize;
}

// Exemple : vidéo 1080p, 10 minutes
// Mémoire ≈ 100 Mo + (1920*1080*3*5) + (600*1Ko) ≈ 131 Mo
```

### Q : puis-je traiter plusieurs vidéos simultanément ?

**R :** oui, mais avec quelques considérations :

```csharp
public class ParallelProcessor
{
    private readonly SemaphoreSlim _semaphore;
    
    public ParallelProcessor(int maxConcurrency = 4)
    {
        // Limiter selon les cœurs CPU et la mémoire disponible
        _semaphore = new SemaphoreSlim(maxConcurrency);
    }
    
    public async Task ProcessMultipleVideos(string[] videoPaths)
    {
        var tasks = videoPaths.Select(ProcessVideoAsync);
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessVideoAsync(string videoPath)
    {
        await _semaphore.WaitAsync();
        try
        {
            var source = new VFPFingerprintSource(videoPath)
            {
                // Réduire la résolution pour le traitement parallèle
                CustomResolution = new Size(640, 480)
            };
            
            var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
            // Traiter l'empreinte...
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

Concurrence recommandée :
- **8 Go de RAM** : 2 à 3 vidéos concurrentes
- **16 Go de RAM** : 4 à 6 vidéos concurrentes
- **32 Go de RAM** : 8 à 12 vidéos concurrentes

### Q : comment optimiser pour le traitement en temps réel ?

**R :** pour un traitement en temps réel ou quasi temps réel :

1. **Réduire le travail par image** — `VFPFingerprintSource` n'expose pas les
   propriétés `UseHardwareAcceleration` ou `HardwareDevice`. Pour accélérer
   l'empreinte elle-même, transcodez la source vers une résolution inférieure
   en amont (par exemple, mise à l'échelle vers 480p) avant de la passer à
   `VFPAnalyzer` :
```csharp
var source = new VFPFingerprintSource(videoPath)
{
    CustomResolution = new Size(480, 360)
};
```

2. **Traiter en segments** (le constructeur de `VFPFingerprintSource` requiert
   un chemin sur disque — `FileNotFoundException` est levée pour les URL/flux ;
   segmentez une capture enregistrée, et non une URL RTSP en direct) :
```csharp
// Traiter par segments de 30 secondes d'un fichier capturé local
var source = new VFPFingerprintSource(localCapturePath)
{
    StartTime = TimeSpan.FromSeconds(segmentIndex * 30),
    StopTime = TimeSpan.FromSeconds((segmentIndex + 1) * 30),
    CustomResolution = new Size(480, 360) // Résolution inférieure
};
```

3. **Rogner / masquer les régions non essentielles** — `VFPFingerprintSource`
   n'a pas de réglage `FrameRate`. Pour analyser moins d'images par seconde,
   transcodez d'abord la source ; ou réduisez la zone analysée avec
   `CustomCropSize` / `IgnoredAreas` pour accélérer le travail par image sans
   supprimer d'images.

## Précision et détection { #accuracy-and-detection }

### Q : à quel point la correspondance vidéo est-elle précise ?

**R :** la précision dépend du type de transformation :

| Transformation | Taux de détection | Taux de faux positifs |
|----------------|---------------|-------------------|
| Réencodage | 99,9 % | <0,1 % |
| Changement de résolution | 99,5 % | <0,1 % |
| Superposition filigrane/logo | 98 % | <0,5 % |
| Rognage (< 20 %) | 95 % | <1 % |
| Ajustement de couleur | 93 % | <2 % |
| Compression élevée | 90 % | <3 % |
| Transformations combinées | 85 à 95 % | <5 % |

### Q : quel seuil de similarité dois-je utiliser ?

**R :** recommandations de seuil selon le cas d'usage :

```csharp
public enum MatchingThreshold
{
    Identical = 5,        // Même fichier, encodage différent
    NearDuplicate = 15,   // Différences de qualité mineures
    Similar = 30,         // Même contenu, quelques modifications
    Related = 50,         // Modifications significatives (filigranes, etc.)
    PossiblyRelated = 100 // Transformations lourdes
}

public bool IsMatch(int difference, MatchingThreshold threshold)
{
    return difference <= (int)threshold;
}
```

Cas d'usage :
- **Détection de droits d'auteur** : utiliser `Similar` (30) ou plus strict
- **Recherche de doublons** : utiliser `NearDuplicate` (15)
- **Surveillance de contenu** : utiliser `Related` (50) pour plus de flexibilité
- **Détection de scènes** : utiliser `PossiblyRelated` (100)

### Q : le SDK peut-il détecter des correspondances partielles ?

**R :** oui ! Le SDK excelle dans la recherche de fragments vidéo :

```csharp
// Rechercher un clip de 30 secondes dans un film de 2 heures
var searchFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    new VFPFingerprintSource("clip.mp4")
);

var mainFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    new VFPFingerprintSource("movie.mp4")
);

// VFPAnalyzer.SearchAsync nécessite la forme complète à 5 arguments ; le résultat est List<TimeSpan>,
// chaque entrée correspond au temps de début de la correspondance dans mainFp.
var results = await VFPAnalyzer.SearchAsync(
    mainFp,
    searchFp,
    searchFp.Duration,
    maxDifference: 25,
    allowMultipleFragments: true);

foreach (var matchStart in results)
{
    Console.WriteLine($"Correspondance trouvée commençant à {matchStart}");
}
```

### Q : comment le SDK gère-t-il les différents rapports d'aspect ?

**R :** le SDK normalise automatiquement les rapports d'aspect, mais vous pouvez améliorer la précision :

```csharp
// Pour les vidéos avec letterboxing/pillarboxing
// IgnoredAreas est `{ get; private set; }` de List<Rect> — alimenter via .Add(...).
// Le constructeur de Rect est (left, top, right, bottom), pas (x, y, width, height).
var source = new VFPFingerprintSource(videoPath);
source.IgnoredAreas.Add(new Rect(0, 0, 1920, 140));      // Letterbox haut
source.IgnoredAreas.Add(new Rect(0, 940, 1920, 1080));   // Letterbox bas

// Ou restreindre la zone d'analyse à une fenêtre de rognage fixe via CustomCropSize.
// (Aucune propriété AutoCropBlackBars n'existe — définissez CustomCropSize manuellement
// si vous souhaitez supprimer les barres letterbox avant l'empreinte.)
var croppedSource = new VFPFingerprintSource(videoPath)
{
    CustomCropSize = new Rect(0, 140, 1920, 940)         // Utiliser uniquement l'image centrale 1920x800
};
```

### Q : peut-il détecter des vidéos mises en miroir ou pivotées ?

**R :** le SDK peut détecter :
- ✅ Miroir horizontal (avec prétraitement)
- ✅ Rotations mineures (< 5 degrés)
- ⚠️ Rotations de 90/180/270 degrés (nécessite un prétraitement manuel)

```csharp
// Pour la détection de vidéo en miroir
public async Task<bool> CheckMirroredMatch(string video1, string video2)
{
    var fp1 = await GenerateFingerprint(video1);
    var fp2 = await GenerateFingerprint(video2);
    
    // Vérifier l'orientation normale
    int normalDiff = VFPAnalyzer.Compare(fp1, fp2);
    if (normalDiff < 30) return true;
    
    // VFPSearch.SearchMirror compare fp1 contre la version horizontalement
    // mise en miroir de fp2 en un seul appel (pas besoin de regénérer l'empreinte).
    var mirrorResult = VFPSearch.SearchMirror(fp1, fp2);
    return mirrorResult != null && mirrorResult.Difference < 30;
}
```

## Formats et codecs pris en charge { #supported-formats-and-codecs }

### Q : quels formats vidéo sont pris en charge ?

**R :** le SDK prend en charge virtuellement tous les formats courants via GStreamer :

**Formats de conteneur** :
- MP4, M4V, MOV
- AVI, WMV, ASF
- MKV, WebM
- FLV, F4V
- MPEG, MPG, M2TS, TS
- 3GP, 3G2
- OGV, OGG
- Et bien d'autres...

**Codecs vidéo** :
- H.264/AVC
- H.265/HEVC
- VP8, VP9
- MPEG-1, MPEG-2, MPEG-4
- WMV7, WMV8, WMV9
- ProRes, DNxHD
- AV1 (avec plugin)

### Q : existe-t-il des limitations de format ?

**R :** certaines limitations existent :

1. **Contenu protégé par DRM** : impossible de traiter les vidéos chiffrées
2. **Codecs rares** : peuvent nécessiter des plugins GStreamer supplémentaires
3. **Fichiers corrompus** : traitement partiel possible avec gestion des erreurs
4. **Flux en direct** : pris en charge mais nécessite un traitement segmenté

### Q : qu'en est-il des fichiers audio uniquement ?

**R :** le Video Fingerprinting SDK requiert du contenu vidéo.

## Compatibilité de plateformes

### Q : quelles plateformes sont prises en charge ?

**R :** matrice complète de prise en charge :

| Plateforme | Architecture | Version .NET | Statut |
|----------|-------------|-------------|---------|
| Windows 10/11 | x86, x64, ARM64 | .NET Framework 4.6.1+, .NET 6+ | ✅ Prise en charge complète |
| Windows Server 2016+ | x64 | .NET Framework 4.6.1+, .NET 6+ | ✅ Prise en charge complète |
| Ubuntu 20.04+ | x64, ARM64 | .NET 6+ | ✅ Prise en charge complète |
| Debian 11+ | x64, ARM64 | .NET 6+ | ✅ Prise en charge complète |
| RHEL/CentOS 8+ | x64 | .NET 6+ | ✅ Prise en charge complète |
| macOS 12+ | x64, ARM64 | .NET 6+ | ✅ Prise en charge complète |
| Docker/Kubernetes | Basé sur Linux | .NET 6+ | ✅ Prise en charge complète |

### Q : puis-je utiliser le SDK dans des conteneurs Docker ?

**R :** oui ! Voici un exemple de Dockerfile :

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# Installer GStreamer et les dépendances
RUN apt-get update && apt-get install -y \
    libgstreamer1.0-0 \
    gstreamer1.0-plugins-base \
    gstreamer1.0-plugins-good \
    gstreamer1.0-plugins-bad \
    gstreamer1.0-plugins-ugly \
    gstreamer1.0-libav \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["YourProject.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YourProject.dll"]
```

### Q : existe-t-il des différences de performance selon les plateformes ?

**R :** oui, les performances varient selon la plateforme :

| Plateforme | Performance relative | Accélération matérielle |
|----------|---------------------|----------------------|
| Windows x64 | 100 % (référence) | NVENC, Quick Sync, D3D11 |
| Linux x64 | 95 à 100 % | NVENC, VAAPI |
| macOS Intel | 90 à 95 % | VideoToolbox |
| macOS ARM64 | 85 à 90 % | VideoToolbox |
| Windows ARM64 | 70 à 80 % | Limitée |

### Q : puis-je utiliser le SDK dans des environnements cloud ?

**R :** oui, le SDK fonctionne sur toutes les principales plateformes cloud :

**AWS** :
```csharp
// Utiliser des instances GPU pour une meilleure performance
// Recommandé : g4dn.xlarge ou p3.2xlarge
```

**Azure** :
```csharp
// Utiliser des VM NCv3-series ou NVv4-series
// Activer l'accélération GPU dans les instances de conteneur
```

**Google Cloud** :
```csharp
// Utiliser N1 avec des GPU NVIDIA Tesla
// Ou la série de machines A2 pour les meilleures performances
```

## Intégration et développement { #integration-and-development }

### Q : puis-je utiliser le SDK dans une application web ?

**R :** oui, pour le traitement côté serveur dans ASP.NET Core :

```csharp
public class VideoFingerprintService
{
    private readonly ILogger<VideoFingerprintService> _logger;
    
    public VideoFingerprintService(ILogger<VideoFingerprintService> logger)
    {
        _logger = logger;
        VFPAnalyzer.SetLicenseKey(Environment.GetEnvironmentVariable("VFP_LICENSE"));
    }
    
    public async Task<IActionResult> ProcessUpload(IFormFile file)
    {
        var tempPath = Path.GetTempFileName();
        try
        {
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var fingerprint = await GenerateFingerprint(tempPath);
            // Stocker l'empreinte en base de données
            
            return Ok(new { Success = true, FingerprintId = fingerprint.Id });
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
```

### Q : comment implémenter une barre de progression ?

**R :** utiliser le callback de progression :

```csharp
public class ProgressTracker
{
    public event EventHandler<int> ProgressChanged;
    
    public async Task<VFPFingerPrint> ProcessWithProgress(string videoPath)
    {
        var source = new VFPFingerprintSource(videoPath);
        
        return await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
            source,
            errorDelegate: (error) => {
                Console.WriteLine($"Erreur : {error}");
            },
            progressDelegate: (progress) => {
                ProgressChanged?.Invoke(this, progress);
            }
        );
    }
}

// Utilisation dans WPF/WinForms
tracker.ProgressChanged += (s, progress) => {
    Dispatcher.Invoke(() => {
        progressBar.Value = progress;
        labelStatus.Text = $"Traitement : {progress}%";
    });
};
```

## Base de données et stockage { #database-and-storage }

### Q : comment dois-je stocker les empreintes dans une base de données ?

**R :** bonnes pratiques pour le stockage en base de données :

```csharp
// Exemple SQL Server
public class FingerprintRepository
{
    public async Task SaveFingerprint(VFPFingerPrint fp, string videoId)
    {
        // Option 1 : stocker comme binaire
        byte[] data = fp.Save();
        
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(
            @"INSERT INTO Fingerprints (VideoId, Data, Duration, Width, Height, Created)
              VALUES (@VideoId, @Data, @Duration, @Width, @Height, @Created)",
            new {
                VideoId = videoId,
                Data = data,
                Duration = fp.Duration.TotalSeconds,
                Width = fp.Width,
                Height = fp.Height,
                Created = DateTime.UtcNow
            });
    }
    
    public async Task<VFPFingerPrint> LoadFingerprint(string videoId)
    {
        using var connection = new SqlConnection(connectionString);
        var data = await connection.QuerySingleAsync<byte[]>(
            "SELECT Data FROM Fingerprints WHERE VideoId = @VideoId",
            new { VideoId = videoId });
        
        return VFPFingerPrint.Load(data);
    }
}

// Exemple MongoDB
public class MongoFingerprintRepository
{
    private readonly IMongoCollection<FingerprintDocument> _collection;
    
    public async Task SaveFingerprint(VFPFingerPrint fp, string videoId)
    {
        var document = new FingerprintDocument
        {
            VideoId = videoId,
            Data = Convert.ToBase64String(fp.Save()),
            Metadata = new FingerprintMetadata
            {
                Duration = fp.Duration,
                Width = fp.Width,
                Height = fp.Height,
                FrameRate = fp.FrameRate
            }
        };
        
        await _collection.InsertOneAsync(document);
    }
}
```

### Q : combien d'espace de stockage les empreintes nécessitent-elles ?

**R :** les tailles d'empreintes sont prévisibles :

| Type d'empreinte | Formule de taille | Exemple (vidéo de 10 min) |
|-----------------|--------------|------------------------|
| Compare | ~100 octets/seconde | ~60 Ko |
| Search | ~1 Ko/seconde | ~600 Ko |
| Avec miniatures | +10 Ko/minute | ~660 Ko |

Planification du stockage :
```csharp
public long EstimateStorageNeeded(int videoCount, double avgDurationMinutes)
{
    // Empreintes de recherche (pire cas)
    long bytesPerMinute = 60 * 1024; // 60 Ko
    long fingerprintSize = (long)(bytesPerMinute * avgDurationMinutes);
    
    // Ajouter 20 % de surcharge pour les métadonnées
    long totalPerVideo = (long)(fingerprintSize * 1.2);
    
    // Stockage total nécessaire
    return videoCount * totalPerVideo;
}

// Exemple : 10 000 vidéos, 30 minutes de moyenne
// Stockage ≈ 10 000 * (60 Ko * 30 * 1,2) = ~21 Go
```

### Q : dois-je utiliser le système de fichiers ou une base de données ?

**R :** cela dépend de vos besoins :

| Type de stockage | Avantages | Inconvénients | Idéal pour |
|-------------|------|------|----------|
| Système de fichiers | Rapide, simple, sauvegarde facile | Difficile à interroger, pas de transactions | < 10 000 vidéos |
| Base SQL | ACID, interrogeable, métadonnées | Plus lent, limites de taille | 10 000 à 100 000 vidéos |
| Base NoSQL | Évolutive, flexible | Configuration complexe | > 100 000 vidéos |
| Stockage objet (S3) | Échelle illimitée, peu coûteuse | Latence réseau | Archive/sauvegarde |

Approche hybride (recommandée pour les grandes échelles) :
```csharp
public class HybridStorage
{
    // Métadonnées en base de données pour des requêtes rapides
    private readonly SqlConnection _db;
    
    // Données d'empreinte dans le stockage objet
    private readonly IS3Client _s3;
    
    public async Task SaveFingerprint(VFPFingerPrint fp, VideoMetadata metadata)
    {
        // Enregistrer l'empreinte vers S3
        string s3Key = $"fingerprints/{metadata.VideoId}.vfp";
        await _s3.PutObjectAsync(s3Key, fp.Save());
        
        // Enregistrer les métadonnées en base
        await _db.ExecuteAsync(
            @"INSERT INTO Videos (Id, Title, Duration, S3Key)
              VALUES (@Id, @Title, @Duration, @S3Key)",
            new {
                metadata.Id,
                metadata.Title,
                metadata.Duration,
                S3Key = s3Key
            });
    }
}
```

### Q : comment déboguer les problèmes de performance ?

**R :** utiliser le profilage et les métriques :

```csharp
public class PerformanceMonitor
{
    public async Task<FingerprintMetrics> ProcessWithMetrics(string videoPath)
    {
        var metrics = new FingerprintMetrics();
        var sw = Stopwatch.StartNew();
        
        // Vérifier la taille du fichier
        var fileInfo = new FileInfo(videoPath);
        metrics.FileSize = fileInfo.Length;
        
        // Surveiller la mémoire avant
        long memoryBefore = GC.GetTotalMemory(false);
        
        var source = new VFPFingerprintSource(videoPath);
        var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
            source,
            progressDelegate: (progress) => {
                if (progress % 10 == 0)
                {
                    long currentMemory = GC.GetTotalMemory(false);
                    Console.WriteLine($"Progression : {progress}%, Mémoire : {currentMemory / 1024 / 1024} Mo");
                }
            }
        );
        
        sw.Stop();
        
        metrics.ProcessingTime = sw.Elapsed;
        metrics.MemoryUsed = GC.GetTotalMemory(false) - memoryBefore;
        metrics.ProcessingSpeed = fileInfo.Length / sw.Elapsed.TotalSeconds;
        
        Console.WriteLine($"Rapport de performance :");
        Console.WriteLine($"  Taille du fichier : {metrics.FileSize / 1024 / 1024} Mo");
        Console.WriteLine($"  Temps de traitement : {metrics.ProcessingTime}");
        Console.WriteLine($"  Mémoire utilisée : {metrics.MemoryUsed / 1024 / 1024} Mo");
        Console.WriteLine($"  Vitesse : {metrics.ProcessingSpeed / 1024 / 1024} Mo/s");
        
        return metrics;
    }
}
```

## Ressources supplémentaires

- **[Référence de l'API](https://api.visioforge.org/dotnet/)** — documentation complète de l'API
- **[Exemples GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)** — exemples de code
- **[Communauté Discord](https://discord.com/invite/yvXUG56WCH)** — obtenir de l'aide de la communauté

## Contacter le support

Si vous ne trouvez pas de réponse dans cette FAQ :

1. **Rechercher dans le forum** : [https://support.visioforge.com/](https://support.visioforge.com/)
2. **Rejoindre Discord** : [https://discord.com/invite/yvXUG56WCH](https://discord.com/invite/yvXUG56WCH)
3. **E-mail du support** : support@visioforge.com (licences commerciales)
4. **Signaler des bugs** : [GitHub Issues](https://github.com/visioforge/.Net-SDK-s-samples/issues)

Lorsque vous contactez le support, veuillez inclure :
- Version du SDK
- Plateforme et version .NET
- Type de licence
- Messages d'erreur et traces d'appel
- Code d'exemple reproduisant le problème
- Journaux de débogage si disponibles
