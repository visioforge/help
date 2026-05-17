---
title: Empreinte vidéo temps réel sur flux en direct (.NET)
description: Empreinte des flux vidéo en direct en temps réel avec le SDK VisioForge — traitement par fragments, capture caméra et surveillance de flux IP en .NET.
tags:
  - Video Capture SDK
  - Video Fingerprinting SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Fingerprinting
  - RTSP
  - MP4
  - C#
primary_api_classes:
  - RealTimeFingerprintProcessor
  - VideoFrameXBufferEventArgs
  - VideoCaptureCoreX
  - VFPAnalyzer
  - VFPFingerprintSource

---

# Guide d'empreinte vidéo en temps réel

Ce guide montre comment traiter les flux vidéo en direct et générer des empreintes en temps réel à l'aide du VisioForge Video Fingerprinting SDK. L'approche utilise un traitement par fragments pour construire les empreintes à partir de flux vidéo continus.

## Vue d'ensemble

L'empreinte en temps réel fonctionne ainsi :
1. Capture des images d'une source en direct (caméra, flux IP, etc.)
2. Accumulation des images dans des fragments basés sur le temps
3. Construction des empreintes à partir des fragments complets
4. Recherche de correspondances par rapport aux empreintes de référence
5. Utilisation de fragments qui se chevauchent pour une meilleure précision de détection

## Composants principaux

### Classe VFPSearch

La classe `VFPSearch` fournit des méthodes statiques pour le traitement d'images en temps réel :

- `Process()` — traite des images individuelles et les ajoute aux données de recherche
- `Build()` — construit une empreinte à partir des données de recherche accumulées

### Classe VFPSearchData

Conteneur pour accumuler les données d'images pendant le traitement en temps réel :

```csharp
// Créer le conteneur de données de recherche pour une durée spécifique
var searchData = new VFPSearchData(TimeSpan.FromSeconds(10));
```

## Exemple complet : traitement d'empreinte en temps réel

Voici un exemple complet basé sur du code de production réel qui traite la vidéo en direct et détecte les correspondances de contenu :

```csharp
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.Events;

public class RealTimeFingerprintProcessor
{
    // Conteneur de données en direct par fragments
    public class FingerprintLiveData : IDisposable
    {
        public VFPSearchData Data { get; private set; }
        public DateTime StartTime { get; private set; }

        public FingerprintLiveData(TimeSpan duration, DateTime startTime)
        {
            Data = new VFPSearchData(duration);
            StartTime = startTime;
        }

        public void Dispose()
        {
            Data?.Dispose();
        }
    }

    // Classe principale du processeur
    private FingerprintLiveData _searchLiveData;
    private FingerprintLiveData _searchLiveOverlapData;
    private ConcurrentQueue<FingerprintLiveData> _fingerprintQueue;
    private List<VFPFingerPrint> _referenceFingerprints;
    private VideoCaptureCoreX _videoCapture;
    
    private IntPtr _tempBuffer;
    private long _fragmentDuration;
    private int _fragmentCount;
    private int _overlapFragmentCount;
    private readonly object _processingLock = new object();

    public async Task InitializeAsync()
    {
        // Initialiser la capture vidéo
        _videoCapture = new VideoCaptureCoreX();
        _videoCapture.OnVideoFrameBuffer += OnVideoFrameReceived;
        
        // Initialiser la file de traitement
        _fingerprintQueue = new ConcurrentQueue<FingerprintLiveData>();
        
        // Charger les empreintes de référence (publicités, contenu protégé, etc.)
        _referenceFingerprints = await LoadReferenceFingerprintsAsync();
        
        // Calculer la durée optimale de fragment
        // Doit être au moins 2x la durée du contenu de référence le plus long
        var maxReferenceDuration = GetMaxReferenceDuration();
        _fragmentDuration = ((maxReferenceDuration + 1000) / 1000 + 1) * 1000 * 2;
    }

    private async Task<List<VFPFingerPrint>> LoadReferenceFingerprintsAsync()
    {
        var fingerprints = new List<VFPFingerPrint>();
        
        // Charger ou générer les empreintes de référence
        foreach (var videoFile in GetReferenceVideos())
        {
            VFPFingerPrint fp;
            
            // Vérifier si l'empreinte existe déjà
            var fpFile = videoFile + ".vfsigx";
            if (File.Exists(fpFile))
            {
                fp = VFPFingerPrint.Load(fpFile);
            }
            else
            {
                // Générer et enregistrer l'empreinte. La signature réelle est
                // (VFPFingerprintSource, VFPErrorCallback errorDelegate = null,
                //  VFPProgressCallback progressDelegate = null) — il n'y a pas
                // de paramètre CancellationToken.
                var source = new VFPFingerprintSource(videoFile);
                fp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
                    source,
                    errorDelegate: null,
                    progressDelegate: null);
                
                fp.Save(fpFile);
            }
            
            fingerprints.Add(fp);
        }
        
        return fingerprints;
    }

    private void OnVideoFrameReceived(object sender, VideoFrameXBufferEventArgs e)
    {
        // Allouer un tampon temporaire si nécessaire
        if (_tempBuffer == IntPtr.Zero)
        {
            var stride = ((e.Frame.Width * 3 + 3) / 4) * 4; // Stride RGB24
            _tempBuffer = Marshal.AllocCoTaskMem(stride * e.Frame.Height);
        }

        // Traiter le fragment principal
        ProcessMainFragment(e);
        
        // Traiter le fragment chevauchant pour une meilleure détection
        ProcessOverlappingFragment(e);
    }

    private void ProcessMainFragment(VideoFrameXBufferEventArgs e)
    {
        // Initialiser un nouveau fragment si nécessaire
        if (_searchLiveData == null)
        {
            _searchLiveData = new FingerprintLiveData(
                TimeSpan.FromMilliseconds(_fragmentDuration), 
                DateTime.Now);
            _fragmentCount++;
        }

        // Calculer l'horodatage relatif au début du fragment
        long timestamp = (long)(e.Frame.Timestamp.TotalMilliseconds * 1000);
        
        // Vérifier si nous sommes toujours dans la durée du fragment actuel
        if (timestamp < _fragmentDuration * _fragmentCount)
        {
            // Copier les données d'image vers le tampon temporaire
            CopyMemory(_tempBuffer, e.Frame.Data, e.Frame.DataSize);
            
            // Calculer l'horodatage corrigé (relatif au début du fragment)
            var correctedTimestamp = timestamp - _fragmentDuration * (_fragmentCount - 1);
            
            // Traiter l'image et l'ajouter aux données de recherche
            VFPSearch.Process(
                _tempBuffer,                                    // Données d'image
                e.Frame.Width,                                   // Largeur
                e.Frame.Height,                                  // Hauteur
                ((e.Frame.Width * 3 + 3) / 4) * 4,             // Stride
                TimeSpan.FromMilliseconds(correctedTimestamp),  // Horodatage
                ref _searchLiveData.Data);                      // Données de recherche
        }
        else
        {
            // Fragment complet, mettre en file pour traitement
            _fingerprintQueue.Enqueue(_searchLiveData);
            _searchLiveData = null;
            
            // Traiter les fragments en file
            ProcessQueuedFragments();
        }
    }

    private void ProcessOverlappingFragment(VideoFrameXBufferEventArgs e)
    {
        long timestamp = (long)(e.Frame.Timestamp.TotalMilliseconds * 1000);
        
        // Démarrer le traitement de chevauchement après la moitié de la durée du fragment
        if (timestamp < _fragmentDuration / 2)
            return;

        // Initialiser le fragment chevauchant si nécessaire
        if (_searchLiveOverlapData == null)
        {
            _searchLiveOverlapData = new FingerprintLiveData(
                TimeSpan.FromMilliseconds(_fragmentDuration),
                DateTime.Now);
            _overlapFragmentCount++;
        }

        // Vérifier si nous sommes dans la durée du fragment chevauchant
        if (timestamp < _fragmentDuration * _overlapFragmentCount + _fragmentDuration / 2)
        {
            CopyMemory(_tempBuffer, e.Frame.Data, e.Frame.DataSize);
            
            VFPSearch.Process(
                _tempBuffer,
                e.Frame.Width,
                e.Frame.Height,
                ((e.Frame.Width * 3 + 3) / 4) * 4,
                TimeSpan.FromMilliseconds(timestamp),
                ref _searchLiveOverlapData.Data);
        }
        else
        {
            // Fragment chevauchant complet
            _fingerprintQueue.Enqueue(_searchLiveOverlapData);
            _searchLiveOverlapData = null;
            
            ProcessQueuedFragments();
        }
    }

    private void ProcessQueuedFragments()
    {
        lock (_processingLock)
        {
            if (_fingerprintQueue.TryDequeue(out var fragmentData))
            {
                // Construire l'empreinte à partir des données du fragment
                long dataSize;
                IntPtr fingerprintPtr = VFPSearch.Build(out dataSize, ref fragmentData.Data);
                
                // Créer l'objet empreinte
                var fingerprint = new VFPFingerPrint
                {
                    Data = new byte[dataSize],
                    OriginalFilename = string.Empty
                };
                
                Marshal.Copy(fingerprintPtr, fingerprint.Data, 0, (int)dataSize);
                
                // Rechercher des correspondances par rapport aux empreintes de référence
                SearchForMatches(fingerprint, fragmentData.StartTime);
                
                // Nettoyer
                fragmentData.Data.Free();
                fragmentData.Dispose();
            }
        }
    }

    private void SearchForMatches(VFPFingerPrint liveFingerprint, DateTime fragmentStartTime)
    {
        foreach (var referenceFingerprint in _referenceFingerprints)
        {
            // Rechercher les correspondances avec un seuil de différence configurable.
            // VFPAnalyzer.SearchAsync retourne Task<List<TimeSpan>>. La contrepartie
            // synchrone est VFPSearch.Search (note : VFPSearch.SearchAsync n'existe PAS —
            // l'asynchrone vit sur VFPAnalyzer). Noms réels des paramètres :
            // maxDifference, allowMultipleFragments.
            var matches = await VFPAnalyzer.SearchAsync(
                referenceFingerprint,            // Empreinte de référence
                liveFingerprint,                 // Empreinte en direct à rechercher
                referenceFingerprint.Duration,   // Durée de la recherche
                maxDifference: 10,               // Seuil de similarité (0-100)
                allowMultipleFragments: true);   // Trouver toutes les correspondances
            
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    // Calculer l'horodatage réel de la correspondance
                    var matchTime = fragmentStartTime.AddMilliseconds(match.TotalMilliseconds);
                    
                    OnMatchFound(new MatchResult
                    {
                        ReferenceFile = referenceFingerprint.OriginalFilename,
                        Timestamp = matchTime,
                        Position = match,
                        Confidence = CalculateConfidence(maxDifference: 10)
                    });
                }
            }
        }
    }

    public async Task StartCameraStreamAsync(string deviceName, string format, double frameRate)
    {
        // Configurer la source caméra
        var devices = await _videoCapture.Video_SourcesAsync();
        var device = devices.FirstOrDefault(d => d.Name == deviceName);
        var videoFormat = device?.VideoFormats.FirstOrDefault(f => f.Name == format);
        
        var settings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = videoFormat.ToFormat()
        };
        settings.Format.FrameRate = new VideoFrameRate(frameRate);
        
        _videoCapture.Video_Source = settings;
        _videoCapture.Start();
    }

    public async Task StartNetworkStreamAsync(string streamUrl)
    {
        // Configurer la source de flux réseau
        var sourceSettings = await UniversalSourceSettings.CreateAsync(streamUrl);
        
        // Pour les caméras IP avec authentification
        if (requiresAuth)
        {
            sourceSettings.Login = "username";
            sourceSettings.Password = "password";
        }
        
        await _videoCapture.StartAsync(sourceSettings);
    }

    public void Stop()
    {
        _videoCapture?.Stop();
        
        // Traiter les fragments restants
        while (_fingerprintQueue.TryDequeue(out var fragment))
        {
            ProcessQueuedFragments();
        }
        
        // Nettoyer
        if (_tempBuffer != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(_tempBuffer);
            _tempBuffer = IntPtr.Zero;
        }
        
        _searchLiveData?.Dispose();
        _searchLiveOverlapData?.Dispose();
    }

    // Méthodes utilitaires
    private long GetMaxReferenceDuration()
    {
        return _referenceFingerprints.Max(fp => (long)fp.Duration.TotalMilliseconds);
    }

    private List<string> GetReferenceVideos()
    {
        // Retourner la liste des fichiers vidéo de référence
        return new List<string> { "ad1.mp4", "ad2.mp4", "copyrighted_content.mp4" };
    }

    private double CalculateConfidence(int maxDifference)
    {
        return (100.0 - maxDifference) / 100.0;
    }

    private void OnMatchFound(MatchResult result)
    {
        Console.WriteLine($"Correspondance trouvée : {result.ReferenceFile} à {result.Timestamp:HH:mm:ss.fff}");
    }

    [DllImport("msvcrt.dll", EntryPoint = "memcpy")]
    private static extern void CopyMemory(IntPtr dest, IntPtr src, int length);
}

// Classes de support
public class MatchResult
{
    public string ReferenceFile { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Position { get; set; }
    public double Confidence { get; set; }
}
```

## Concepts clés

### Durée du fragment

La durée du fragment détermine la quantité de données vidéo accumulées avant la construction d'une empreinte :

```csharp
// Calculer la durée optimale du fragment
// Doit être au moins 2x la durée du contenu le plus long que vous voulez détecter
var maxContentDuration = 30000; // 30 secondes en millisecondes
var fragmentDuration = ((maxContentDuration + 1000) / 1000 + 1) * 1000 * 2;
// Résultat : fragments de 62000 ms (62 secondes)
```

### Fragments qui se chevauchent

L'utilisation de fragments chevauchants améliore la précision de détection en garantissant que le contenu n'est pas manqué aux frontières des fragments :

```csharp
// Fragment principal : 0-60 secondes
// Fragment de chevauchement 1 : 30-90 secondes (démarre à 50 % du principal)
// Fragment de chevauchement 2 : 60-120 secondes
// Ceci garantit que tout contenu est entièrement capturé dans au moins un fragment
```

### Traitement des images

Chaque image est traitée individuellement et ajoutée aux données de recherche du fragment actuel :

```csharp
VFPSearch.Process(
    frameData,      // Pointeur de données d'image RGB24
    width,          // Largeur de l'image
    height,         // Hauteur de l'image  
    stride,         // Stride de ligne en octets
    timestamp,      // Horodatage de l'image
    ref searchData  // Accumulateur pour ce fragment
);
```

### Construction des empreintes

Une fois un fragment complet, construisez l'empreinte :

```csharp
long dataSize;
IntPtr fingerprintPtr = VFPSearch.Build(out dataSize, ref searchData);

// Copier vers un tableau managé
var fingerprintData = new byte[dataSize];
Marshal.Copy(fingerprintPtr, fingerprintData, 0, (int)dataSize);
```

## Considérations de performance

### Gestion de la mémoire

- Pré-allouer les tampons pour éviter les allocations répétées
- Utiliser `Marshal.AllocCoTaskMem` pour les tampons non managés
- Libérer correctement les objets `VFPSearchData` après usage
- Libérer les données de recherche avec `searchData.Free()` après construction

### Stratégie de traitement

- Utiliser une file d'attente pour découpler la capture d'images du traitement d'empreintes
- Traiter les fragments dans un thread séparé pour éviter de bloquer la capture
- Ajuster la durée du fragment selon la longueur du contenu de référence
- Utiliser des fragments chevauchants pour une meilleure précision de détection

### Conseils d'optimisation

1. **Durée du fragment** : fragments plus longs = meilleure précision mais latence plus élevée
2. **Pourcentage de chevauchement** : 50 % de chevauchement est typique, à ajuster selon les besoins
3. **Seuil de différence** : valeurs plus basses = correspondance plus stricte (échelle 0-100)
4. **Taille du tampon** : pré-allouer selon la taille d'image maximale attendue

## Cas d'usage courants

### Surveillance de diffusion en direct

Surveillez les diffusions TV en direct à la recherche de contenu protégé ou de publicités :

```csharp
// Initialiser avec une bibliothèque de contenu diffusé
var processor = new RealTimeFingerprintProcessor();
await processor.LoadReferenceFingerprintsAsync("ads_library/");

// Démarrer le traitement du flux diffusé
await processor.StartNetworkStreamAsync("rtsp://broadcast-server/stream1");
```

### Analyse de caméras de sécurité

Détectez des événements ou objets spécifiques dans les flux de caméras de sécurité :

```csharp
// Traiter plusieurs flux de caméras
var processors = new List<RealTimeFingerprintProcessor>();

foreach (var camera in GetSecurityCameras())
{
    var processor = new RealTimeFingerprintProcessor();
    await processor.StartNetworkStreamAsync(camera.StreamUrl);
    processors.Add(processor);
}
```

### Conformité de contenu

Garantissez la conformité des plateformes de streaming aux restrictions de contenu :

```csharp
// Surveiller les flux générés par les utilisateurs
var processor = new RealTimeFingerprintProcessor();
await processor.LoadReferenceFingerprintsAsync("prohibited_content/");

// Traiter le flux entrant
await processor.StartNetworkStreamAsync(userStreamUrl);
```

## Dépannage

### Aucune correspondance trouvée

- Vérifiez que la durée du fragment est appropriée à la longueur du contenu de référence
- Vérifiez que le seuil de différence n'est pas trop strict
- Assurez-vous que les fragments chevauchants sont activés
- Vérifiez que les empreintes de référence ont été générées correctement

### Utilisation mémoire élevée

- Réduisez la durée du fragment si possible
- Traitez et libérez les fragments plus fréquemment
- Utilisez un plus petit pourcentage de chevauchement
- Libérez correctement les tampons non managés

### Latence de traitement

- Utilisez des threads séparés pour la capture et le traitement
- Implémentez l'abandon d'images si le traitement ne peut pas suivre
- Optimisez la recherche d'empreintes de référence (index, recherche parallèle)
- Envisagez l'accélération GPU si disponible

## Résumé

L'empreinte en temps réel avec le SDK VisioForge utilise une approche par fragments qui :
- Accumule les images dans des fragments basés sur le temps avec `VFPSearchData`
- Traite les images en temps réel avec `VFPSearch.Process()`
- Construit des empreintes à partir de fragments avec `VFPSearch.Build()`
- Utilise des fragments chevauchants pour une détection robuste
- Permet la correspondance de contenu en direct par rapport aux empreintes de référence

Cette approche a fait ses preuves en environnements de production pour la surveillance de diffusion, la conformité de contenu et les applications de sécurité.

## Application d'exemple complète

Pour un exemple de travail complet d'empreinte vidéo en temps réel, consultez l'application d'exemple **MMT Live** :
- [Exemple MMT Live sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT%20Live)

Cet exemple démontre tous les concepts couverts dans ce guide avec une application Windows Forms complète pour la détection de publicités en direct.
