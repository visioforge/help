---
title: Prise en main du Video Fingerprinting SDK .NET VisioForge
description: Guide complet d'installation et de configuration du VisioForge Video Fingerprinting SDK avec configuration, licences et instructions pas à pas.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - GStreamer
  - Fingerprinting
  - MP4
  - MKV
  - AVI
  - C#
  - NuGet
primary_api_classes:
  - VFPAnalyzer
  - VFPFingerprintSource

---

# Prise en main du Video Fingerprinting SDK

Bienvenue dans le VisioForge Video Fingerprinting SDK ! Ce guide complet vous accompagnera dans tout ce dont vous avez besoin pour démarrer, de l'installation à votre première application opérationnelle. À la fin de ce guide, vous disposerez d'une base solide pour construire des applications d'empreinte vidéo.

## Résumé du démarrage rapide

Si vous cherchez à être opérationnel rapidement :

1. Installez le SDK via NuGet : `Install-Package VisioForge.DotNet.Core`
2. Ajoutez le paquet de redistribution : `Install-Package VisioForge.DotNet.Core.Redist.VideoFingerprinting`
   - Ce paquet unique prend en charge Windows (x86/x64), Linux (x64/ARM64) et macOS
3. Définissez votre clé de licence : `VFPAnalyzer.SetLicenseKey("TRIAL");`
4. Générez votre première empreinte à l'aide des exemples ci-dessous

## Prérequis et configuration système

Pour les exigences détaillées, notamment les plateformes prises en charge, les spécifications matérielles et les considérations de performance, consultez notre guide complet de [configuration système requise](../system-requirements.md).

### Exigences spécifiques à .NET

- **Version de .NET** : 
  - Windows : .NET Framework 4.6.1+ ou .NET 6.0+
  - Linux/macOS : .NET 6.0+
- **IDE** : Visual Studio 2019+ (Windows), Visual Studio Code ou JetBrains Rider
- **Gestionnaire de paquets NuGet** : pour une installation et des mises à jour faciles

## Méthodes d'installation

### Méthode 1 : gestionnaire de paquets NuGet (recommandée)

La façon la plus simple d'installer le SDK passe par le gestionnaire de paquets NuGet dans Visual Studio.

#### Via l'interface du gestionnaire de paquets

1. Cliquez avec le bouton droit sur votre projet dans l'Explorateur de solutions
2. Sélectionnez « Gérer les paquets NuGet »
3. Cliquez sur « Parcourir » et recherchez « VisioForge.DotNet.Core »
4. Sélectionnez le paquet et cliquez sur « Installer »
5. Acceptez le contrat de licence

#### Via la console du gestionnaire de paquets

```powershell
# Installer le paquet principal du SDK
Install-Package VisioForge.DotNet.Core

# Installer le paquet de redistribution avec les bibliothèques natives (requis)
# Ce paquet inclut la prise en charge Windows (x86/x64), Linux (x64/arm64) et macOS
Install-Package VisioForge.DotNet.Core.Redist.VideoFingerprinting

# Pour l'intégration MongoDB (optionnel)
Install-Package VisioForge.DotNet.VideoFingerprinting.MongoDB
```

#### Via la CLI .NET

```bash
# Ajouter le paquet principal du SDK
dotnet add package VisioForge.DotNet.Core

# Ajouter le paquet de redistribution avec les bibliothèques natives (requis)
# Ce paquet inclut la prise en charge Windows (x86/x64), Linux (x64/arm64) et macOS
dotnet add package VisioForge.DotNet.Core.Redist.VideoFingerprinting

# Pour l'intégration MongoDB (optionnel)
dotnet add package VisioForge.DotNet.VideoFingerprinting.MongoDB

# Restaurer les paquets
dotnet restore
```

#### Via PackageReference (dans .csproj)

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core" Version="2025.8.7" />
  <PackageReference Include="VisioForge.DotNet.Core.Redist.VideoFingerprinting" Version="2025.8.7" />
  
  <!-- Optionnel : intégration MongoDB -->
  <PackageReference Include="VisioForge.DotNet.VideoFingerprinting.MongoDB" Version="2025.8.7" />
</ItemGroup>
```

!!! important "Exigences des paquets NuGet"

    Le SDK requiert deux paquets :

    1. **VisioForge.DotNet.Core** — bibliothèque principale du SDK avec l'API C#
    2. **VisioForge.DotNet.Core.Redist.VideoFingerprinting** — bibliothèques natives pour la fonctionnalité d'empreinte vidéo (prend en charge Windows x86/x64, Linux x64/arm64 et macOS)

    Les deux paquets doivent être installés pour que le SDK fonctionne correctement. Le paquet de redistribution contient des bibliothèques natives spécifiques à chaque plateforme, automatiquement déployées dans votre répertoire de sortie.

    **Paquets optionnels :**

    - **VisioForge.DotNet.VideoFingerprinting.MongoDB** — intégration MongoDB pour stocker les empreintes en base de données

    **Prise en charge des plateformes :**
    Le paquet `VisioForge.DotNet.Core.Redist.VideoFingerprinting` inclut les bibliothèques natives pour :

    - Windows (x86 et x64)
    - Linux (x64 et ARM64)
    - macOS (Intel et Apple Silicon)

    Les bibliothèques spécifiques à la plateforme correcte sont automatiquement sélectionnées et déployées selon votre runtime cible.

### Méthode 2 : installation manuelle

Pour les environnements où NuGet n'est pas disponible ou pour les scénarios de déploiement personnalisés :

1. **Téléchargez le SDK**
   - Visitez la [page produit](https://www.visioforge.com/video-fingerprinting-sdk)
   - Choisissez votre plateforme et architecture

2. **Exécutez le programme d'installation**

### Paquets supplémentaires spécifiques par plateforme

Bien que le paquet `VisioForge.DotNet.Core.Redist.VideoFingerprinting` inclue toutes les bibliothèques natives nécessaires à l'empreinte vidéo, vous pouvez avoir besoin de paquets supplémentaires pour des fonctionnalités étendues :

#### Pour les applications Windows

```powershell
# Paquets supplémentaires spécifiques Windows (optionnels, selon vos besoins)
Install-Package VisioForge.DotNet.Core.Redist.Base.x64  # Prise en charge Windows x64 étendue
Install-Package VisioForge.DotNet.Core.Redist.Base.x86  # Prise en charge Windows x86 étendue
```

#### Pour les applications mobiles

```powershell
# Prise en charge UI iOS/macOS/tvOS
Install-Package VisioForge.DotNet.Core.UI.Apple

# Prise en charge UI Android
Install-Package VisioForge.DotNet.Core.UI.Android
```

### Configuration spécifique à chaque plateforme

#### Configuration Windows

Non requise.

#### Configuration Linux

1. **Installer les dépendances GStreamer**

   ```bash
   # Ubuntu/Debian
   sudo apt-get update
   sudo apt-get install -y \
     gstreamer1.0-plugins-base \
     gstreamer1.0-plugins-good \
     gstreamer1.0-plugins-bad \
     gstreamer1.0-plugins-ugly \
     gstreamer1.0-libav
   
   # RHEL/CentOS
   sudo yum install -y \
     gstreamer1.0-plugins-base \
     gstreamer1.0-plugins-good \
     gstreamer1.0-plugins-bad \
     gstreamer1.0-plugins-ugly \
     gstreamer1.0-libav
   ```

#### Configuration macOS

Non requise.

## Activation de la clé de licence

### Obtenir une clé de licence

1. **Licence d'essai**
   - Utilisez une chaîne vide pour l'évaluation
   - Fonctionnalité complète avec filigrane
   - Période d'évaluation de 30 jours

2. **Licence commerciale**
   - Achetez depuis la [page produit](https://www.visioforge.com/video-fingerprinting-sdk)
   - Recevez la clé de licence par e-mail
   - Utilisez la clé de licence dans votre application

### Activer votre licence

```csharp
using VisioForge.Core.VideoFingerPrinting;

// Au démarrage de l'application
public static void InitializeSDK()
{
    // Pour l'évaluation d'essai — ne rien faire
    
    // Pour la licence commerciale
    VFPAnalyzer.SetLicenseKey("YOUR-LICENSE-KEY-HERE");    
}
```

### Validation de la licence

```csharp
// Vérifier l'état de la licence
public static bool ValidateLicense()
{
    try
    {
        // Tenter une opération simple pour vérifier la licence
        var testSource = new VFPFingerprintSource("test.mp4");
        testSource.StopTime = TimeSpan.FromSeconds(1);
        
        // Cela échouera si la licence est invalide
        var fp = VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(testSource).Result;
        
        return fp != null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Échec de la validation de licence : {ex.Message}");
        return false;
    }
}
```

### Types de licence

| Fonctionnalité | Essai | Commerciale |
|---------|-------|------------|
| Empreinte basique | ✅ | ✅ |
| Comparaison de vidéos | ✅ | ✅ |
| Recherche de fragments | ✅ | ✅ |
| Prise en charge de base de données | ✅ | ✅ |
| Prise en charge multiplateforme | ✅ | ✅ |
| Filigrane | Oui | Non |
| Support technique | Forum | E-mail/prioritaire |
| Mises à jour | 30 jours | 1 an |

## Votre première génération d'empreinte

Créons une application console simple qui génère une empreinte vidéo :

### Étape 1 : créer un nouveau projet

```bash
# Créer une nouvelle application console
dotnet new console -n VideoFingerprintingDemo
cd VideoFingerprintingDemo

# Ajouter les paquets du SDK
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoFingerprinting
```

### Étape 2 : implémentation de base

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types; // pour Size / Rect

namespace VideoFingerprintingDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialiser le SDK avec votre licence
            VFPAnalyzer.SetLicenseKey("TRIAL");
            
            // Spécifier le fichier vidéo à traiter
            string videoPath = @"C:\Videos\sample.mp4";
            
            if (!File.Exists(videoPath))
            {
                Console.WriteLine($"Erreur : fichier vidéo introuvable à {videoPath}");
                return;
            }
            
            try
            {
                // Générer l'empreinte
                await GenerateFingerprint(videoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
        
        static async Task GenerateFingerprint(string videoPath)
        {
            Console.WriteLine($"Traitement : {Path.GetFileName(videoPath)}");
            Console.WriteLine("----------------------------------------");
            
            // Créer la configuration de la source
            var source = new VFPFingerprintSource(videoPath);
            
            // Optionnel : ne traiter que les 30 premières secondes pour les tests
            source.StopTime = TimeSpan.FromSeconds(30);
            
            // Optionnel : réduire la résolution pour un traitement plus rapide
            source.CustomResolution = new VisioForge.Core.Types.Size(640, 480);
            
            // Générer l'empreinte avec suivi de progression
            var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                source,
                errorDelegate: (error) => {
                    Console.WriteLine($"Erreur : {error}");
                },
                progressDelegate: (progress) => {
                    Console.Write($"\rProgression : {progress}%");
                }
            );
            
            Console.WriteLine(); // Nouvelle ligne après la progression
            
            if (fingerprint != null)
            {
                // Afficher les informations de l'empreinte
                Console.WriteLine("\nEmpreinte générée avec succès !");
                Console.WriteLine($"  Durée : {fingerprint.Duration}");
                Console.WriteLine($"  Résolution : {fingerprint.Width}x{fingerprint.Height}");
                Console.WriteLine($"  Fréquence d'images : {fingerprint.FrameRate:F2} ips");
                Console.WriteLine($"  Taille des données : {fingerprint.Data?.Length ?? 0} octets");
                
                // Enregistrer l'empreinte dans un fichier
                string outputPath = Path.ChangeExtension(videoPath, ".vfp");
                fingerprint.Save(outputPath);
                
                Console.WriteLine($"\nEmpreinte enregistrée vers : {outputPath}");
                Console.WriteLine($"Taille du fichier : {new FileInfo(outputPath).Length / 1024} Ko");
            }
            else
            {
                Console.WriteLine("Échec de la génération de l'empreinte.");
            }
        }
    }
}
```

### Étape 3 : exécuter l'application

```bash
# Compiler et exécuter
dotnet build
dotnet run

# Sortie attendue :
# Traitement : sample.mp4
# ----------------------------------------
# Progression : 100%
# 
# Empreinte générée avec succès !
#   Durée : 00:00:30
#   Résolution : 1920x1080
#   Fréquence d'images : 29,97 ips
#   Taille des données : 125440 octets
# 
# Empreinte enregistrée vers : C:\Videos\sample.vfp
# Taille du fichier : 122 Ko
```

## Exemple de comparaison de base

Comparons maintenant deux vidéos pour déterminer leur similarité :

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types; // pour Size / Rect

class VideoComparisonDemo
{
    static async Task Main(string[] args)
    {
        VFPAnalyzer.SetLicenseKey("TRIAL");
        
        string video1 = @"C:\Videos\original.mp4";
        string video2 = @"C:\Videos\copy.mp4";
        
        await CompareVideos(video1, video2);
    }
    
    static async Task CompareVideos(string path1, string path2)
    {
        Console.WriteLine("Comparaison des vidéos...");
        Console.WriteLine($"Vidéo 1 : {Path.GetFileName(path1)}");
        Console.WriteLine($"Vidéo 2 : {Path.GetFileName(path2)}");
        Console.WriteLine("----------------------------------------");
        
        // Créer les sources avec limites de temps pour une comparaison rapide
        var source1 = new VFPFingerprintSource(path1)
        {
            StopTime = TimeSpan.FromSeconds(30),
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        var source2 = new VFPFingerprintSource(path2)
        {
            StopTime = TimeSpan.FromSeconds(30),
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        // Générer les empreintes
        Console.Write("Génération de l'empreinte 1...");
        var fp1 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source1);
        Console.WriteLine(" Terminé");
        
        Console.Write("Génération de l'empreinte 2...");
        var fp2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source2);
        Console.WriteLine(" Terminé");
        
        if (fp1 != null && fp2 != null)
        {
            // Comparer les empreintes
            int difference = VFPAnalyzer.Compare(
                fp1, 
                fp2, 
                TimeSpan.FromMilliseconds(500)
            );
            
            Console.WriteLine($"\nRésultats de la comparaison :");
            Console.WriteLine($"  Score de différence : {difference}");
            
            // Interpréter les résultats
            string interpretation = GetInterpretation(difference);
            Console.WriteLine($"  Interprétation : {interpretation}");
            
            // Fournir une analyse détaillée
            if (difference < 100)
            {
                double similarity = Math.Max(0, 100 - (difference / 3.0));
                Console.WriteLine($"  Similarité : {similarity:F1}%");
            }
        }
        else
        {
            Console.WriteLine("Erreur : échec de la génération d'une ou des deux empreintes");
        }
    }
    
    static string GetInterpretation(int difference)
    {
        if (difference < 5)
            return "IDENTIQUES — même vidéo, encodage éventuellement différent";
        else if (difference < 15)
            return "PRESQUE IDENTIQUES — même vidéo avec des différences de qualité mineures";
        else if (difference < 30)
            return "TRÈS SIMILAIRES — même contenu avec de légères modifications";
        else if (difference < 50)
            return "SIMILAIRES — même contenu avec des changements visibles (filigrane, logo, etc.)";
        else if (difference < 100)
            return "LIÉES — similarités significatives, probablement même matériau source";
        else if (difference < 300)
            return "QUELQUE PEU LIÉES — quelques scènes ou contenus en commun";
        else
            return "DIFFÉRENTES — vidéos complètement différentes";
    }
}
```

## Pièges courants et solutions

### Problème 1 : DllNotFoundException

**Problème** : l'application plante avec « Impossible de charger la DLL 'VisioForge_VideoFingerprinting' »

**Solution** :

Ajoutez le paquet NuGet `VisioForge.DotNet.Core.Redist.VideoFingerprinting` à votre projet.

### Problème 2 : exception de mémoire insuffisante

**Problème** : « System.OutOfMemoryException » lors du traitement de grandes vidéos

**Solutions** :

```csharp
// Solution 1 : utiliser un processus 64 bits et augmenter la mémoire
// Ajouter au .csproj :
<PropertyGroup>
  <PlatformTarget>x64</PlatformTarget>
  <LargeAddressAware>true</LargeAddressAware>
</PropertyGroup>

// Solution 2 : réduire la résolution vidéo. VFPFingerprintSource n'a pas de propriété
// FrameRate — pour abaisser le nombre d'IPS analysées, transcoder la source d'abord
// ou rééchantillonner en amont. Réglages disponibles : CustomResolution, CustomCropSize,
// IgnoredAreas, StartTime / StopTime.
var source = new VFPFingerprintSource(videoPath)
{
    CustomResolution = new VisioForge.Core.Types.Size(320, 240) // Résolution très basse
};
```

### Problème 3 : vitesse de traitement lente

**Problème** : la génération d'empreinte prend trop de temps

**Solutions** :

```csharp
// Solution 1 : utiliser le traitement parallèle pour plusieurs vidéos
static async Task ProcessMultipleVideos(string[] videoPaths)
{
    var tasks = videoPaths.Select(path => Task.Run(async () =>
    {
        var source = new VFPFingerprintSource(path)
        {
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        return await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    }));
    
    var fingerprints = await Task.WhenAll(tasks);
}

// Solution 3 : mettre en cache les empreintes
class FingerprintCache
{
    private static Dictionary<string, VFPFingerPrint> cache = new();
    
    public static async Task<VFPFingerPrint> GetOrGenerate(string videoPath)
    {
        string cacheKey = GetCacheKey(videoPath);
        
        if (cache.ContainsKey(cacheKey))
            return cache[cacheKey];
        
        string cachePath = $"{videoPath}.vfp";
        
        if (File.Exists(cachePath))
        {
            var fp = VFPFingerPrint.Load(cachePath);
            cache[cacheKey] = fp;
            return fp;
        }
        
        // Générer une nouvelle empreinte
        var source = new VFPFingerprintSource(videoPath);
        var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
        
        if (fingerprint != null)
        {
            fingerprint.Save(cachePath);
            cache[cacheKey] = fingerprint;
        }
        
        return fingerprint;
    }
    
    private static string GetCacheKey(string path)
    {
        var info = new FileInfo(path);
        return $"{path}_{info.Length}_{info.LastWriteTimeUtc.Ticks}";
    }
}
```

### Problème 4 : résultats de similarité incorrects

**Problème** : des vidéos qui devraient correspondre apparaissent comme différentes

**Solutions** :

```csharp
// Solution 1 : ajuster les paramètres de comparaison
static int CompareWithTolerance(VFPFingerPrint fp1, VFPFingerPrint fp2)
{
    // Essayer différents décalages temporels
    int[] shifts = { 100, 500, 1000, 2000 }; // millisecondes
    int minDifference = int.MaxValue;
    
    foreach (int shift in shifts)
    {
        int diff = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromMilliseconds(shift));
        minDifference = Math.Min(minDifference, diff);
    }
    
    return minDifference;
}

// Solution 2 : gérer les vidéos avec des rapports d'aspect différents.
// Le constructeur de Rect est (left, top, right, bottom) — PAS width/height.
var source = new VFPFingerprintSource(videoFilePath);
{
    // Ignorer les zones letterbox/pillarbox (bande 1920x140 en haut + bas d'une image 1920x1080)
    source.IgnoredAreas.AddRange(new[]
    {
        new Rect(0, 0,    1920, 140),    // Letterbox haut : y dans [0, 140)
        new Rect(0, 940,  1920, 1080)    // Letterbox bas : y dans [940, 1080)
    });
};
```

## Résumé des bonnes pratiques

### À faire

- ✅ Toujours définir la clé de licence avant toute opération SDK
- ✅ Utiliser des blocs try-catch autour des appels SDK
- ✅ Traiter les vidéos en résolution inférieure pour une analyse plus rapide
- ✅ Mettre en cache les empreintes pour éviter le retraitement
- ✅ Utiliser le type d'empreinte approprié (Search vs Compare)
- ✅ Tester d'abord avec de petits segments vidéo
- ✅ Implémenter des callbacks de progression pour le retour utilisateur
- ✅ Libérer les objets d'empreinte une fois terminés

### À éviter

- ❌ Ne pas ignorer les callbacks d'erreur
- ❌ Ne pas comparer des empreintes de types différents
- ❌ Ne pas traiter plusieurs grandes vidéos simultanément sans gestion de la mémoire

## Étapes suivantes

Maintenant que le SDK est installé et fonctionnel, explorez ces ressources :

1. **[Documentation de l'API](api.md)** — référence complète pour toutes les classes et méthodes
2. **[Cas d'usage et applications](../use-cases.md)** — scénarios d'implémentation réels
3. **[Comprendre la technologie](../understanding-video-fingerprinting.md)** — plongée technique approfondie

## Obtenir de l'aide

### Ressources

- **Référence de l'API** : [https://api.visioforge.org/dotnet/](https://api.visioforge.org/dotnet/)
- **Exemples GitHub** : [https://github.com/visioforge/.Net-SDK-s-samples/](https://github.com/visioforge/.Net-SDK-s-samples/)
- **Forum de support** : [https://support.visioforge.com/](https://support.visioforge.com/)
- **Communauté Discord** : [https://discord.com/invite/yvXUG56WCH](https://discord.com/invite/yvXUG56WCH)

### Questions fréquentes

- **Q : puis-je utiliser le SDK dans une application web ?**
  R : oui, le SDK peut être utilisé dans les applications ASP.NET Core pour le traitement côté serveur.

- **Q : quels formats vidéo sont pris en charge ?**
  R : MP4, AVI, MKV, MOV, WMV, FLV, WebM et bien d'autres via GStreamer.

- **Q : quelle est la précision de l'empreinte ?**
  R : généralement 95-99 % de précision pour l'identification de contenu, selon les transformations.

- **Q : peut-il détecter des vidéos avec des filigranes ajoutés ?**
  R : oui, le SDK peut identifier les vidéos même avec des filigranes, logos ou sous-titres ajoutés.

## Exemple de travail complet

Voici une application console complète qui démontre toutes les opérations de base :

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types; // pour Size / Rect

namespace VideoFingerprintingDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialiser le SDK
            VFPAnalyzer.SetLicenseKey("TRIAL");
            
            // Configurer les chemins
            string videosDir = @"C:\Videos";
            string dbDir = @"C:\FingerprintDB";
            Directory.CreateDirectory(dbDir);
            
            var app = new FingerprintingApp(videosDir, dbDir);
            
            while (true)
            {
                Console.WriteLine("\n=== Démo Video Fingerprinting ===");
                Console.WriteLine("1. Générer une empreinte pour une vidéo");
                Console.WriteLine("2. Comparer deux vidéos");
                Console.WriteLine("3. Trouver un fragment dans une vidéo");
                Console.WriteLine("4. Construire une base d'empreintes");
                Console.WriteLine("5. Rechercher dans la base des vidéos similaires");
                Console.WriteLine("0. Quitter");
                Console.Write("\nSélectionnez une option : ");
                
                var choice = Console.ReadLine();
                Console.WriteLine();
                
                try
                {
                    switch (choice)
                    {
                        case "1":
                            await app.GenerateFingerprint();
                            break;
                        case "2":
                            await app.CompareTwoVideos();
                            break;
                        case "3":
                            await app.FindFragment();
                            break;
                        case "4":
                            await app.BuildDatabase();
                            break;
                        case "5":
                            await app.SearchDatabase();
                            break;
                        case "0":
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }
    }
    
    class FingerprintingApp
    {
        private string videosDir;
        private string dbDir;
        private Dictionary<string, VFPFingerPrint> database = new Dictionary<string, VFPFingerPrint>();
        
        public FingerprintingApp(string videosDir, string dbDir)
        {
            this.videosDir = videosDir;
            this.dbDir = dbDir;
            LoadDatabase();
        }
        
        public async Task GenerateFingerprint()
        {
            Console.Write("Entrez le nom du fichier vidéo : ");
            string filename = Console.ReadLine();
            string videoPath = Path.Combine(videosDir, filename);
            
            if (!File.Exists(videoPath))
            {
                Console.WriteLine("Fichier introuvable !");
                return;
            }
            
            var source = new VFPFingerprintSource(videoPath);
            
            Console.Write("Traiter la vidéo entière ? (y/n) : ");
            if (Console.ReadLine().ToLower() != "y")
            {
                Console.Write("Entrez la durée en secondes : ");
                if (int.TryParse(Console.ReadLine(), out int seconds))
                {
                    source.StopTime = TimeSpan.FromSeconds(seconds);
                }
            }
            
            Console.WriteLine("Génération de l'empreinte...");
            var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                source,
                errorDelegate: (msg) => Console.WriteLine($"Erreur : {msg}"),
                progressDelegate: (p) => Console.Write($"\rProgression : {p}%")
            );
            
            if (fp != null)
            {
                string outputPath = Path.ChangeExtension(videoPath, ".vfp");
                fp.Save(outputPath);
                Console.WriteLine($"\n✓ Empreinte enregistrée vers : {outputPath}");
                Console.WriteLine($"  Durée : {fp.Duration}");
                Console.WriteLine($"  Résolution : {fp.Width}x{fp.Height}");
                Console.WriteLine($"  Fréquence d'images : {fp.FrameRate:F2} ips");
            }
        }
        
        public async Task CompareTwoVideos()
        {
            Console.Write("Entrez le nom du premier fichier vidéo : ");
            string file1 = Path.Combine(videosDir, Console.ReadLine());
            
            Console.Write("Entrez le nom du second fichier vidéo : ");
            string file2 = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(file1) || !File.Exists(file2))
            {
                Console.WriteLine("Un ou les deux fichiers introuvables !");
                return;
            }
            
            Console.WriteLine("Génération des empreintes...");
            
            var source1 = new VFPFingerprintSource(file1);
            source1.StopTime = TimeSpan.FromSeconds(30); // Comparaison rapide
            
            var source2 = new VFPFingerprintSource(file2);
            source2.StopTime = TimeSpan.FromSeconds(30);
            
            var fp1 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source1);
            var fp2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source2);
            
            if (fp1 != null && fp2 != null)
            {
                int difference = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromMilliseconds(500));
                
                Console.WriteLine($"\nScore de différence : {difference}");
                
                if (difference < 5)
                    Console.WriteLine("✓ Les vidéos sont IDENTIQUES");
                else if (difference < 30)
                    Console.WriteLine("✓ Les vidéos sont TRÈS SIMILAIRES");
                else if (difference < 100)
                    Console.WriteLine("✓ Les vidéos sont SIMILAIRES");
                else if (difference < 300)
                    Console.WriteLine("⚠ Les vidéos ont QUELQUES SIMILARITÉS");
                else
                    Console.WriteLine("✗ Les vidéos sont DIFFÉRENTES");
            }
        }
        
        public async Task FindFragment()
        {
            Console.Write("Entrez le nom du fichier vidéo fragment : ");
            string fragmentFile = Path.Combine(videosDir, Console.ReadLine());
            
            Console.Write("Entrez le nom du fichier vidéo complet : ");
            string fullFile = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(fragmentFile) || !File.Exists(fullFile))
            {
                Console.WriteLine("Un ou les deux fichiers introuvables !");
                return;
            }
            
            Console.WriteLine("Traitement du fragment...");
            var fragmentFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
                new VFPFingerprintSource(fragmentFile),
                progressDelegate: (p) => Console.Write($"\rFragment : {p}%")
            );
            
            Console.WriteLine("\nTraitement de la vidéo complète...");
            var fullFp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                new VFPFingerprintSource(fullFile),
                progressDelegate: (p) => Console.Write($"\rVidéo complète : {p}%")
            );
            
            if (fragmentFp != null && fullFp != null)
            {
                Console.WriteLine("\n\nRecherche...");
                var positions = await VFPAnalyzer.SearchAsync(
                    fragmentFp, fullFp, fragmentFp.Duration, 50, true
                );
                
                if (positions.Count > 0)
                {
                    Console.WriteLine($"✓ {positions.Count} occurrence(s) trouvée(s) :");
                    foreach (var pos in positions)
                    {
                        Console.WriteLine($"  - À {pos:hh\\:mm\\:ss}");
                    }
                }
                else
                {
                    Console.WriteLine("✗ Fragment non trouvé");
                }
            }
        }
        
        public async Task BuildDatabase()
        {
            var videoFiles = Directory.GetFiles(videosDir, "*.mp4")
                .Concat(Directory.GetFiles(videosDir, "*.avi"))
                .Concat(Directory.GetFiles(videosDir, "*.mkv"))
                .ToList();
            
            Console.WriteLine($"{videoFiles.Count} fichiers vidéo trouvés");
            
            int processed = 0;
            foreach (var videoFile in videoFiles)
            {
                string id = Path.GetFileNameWithoutExtension(videoFile);
                string fpPath = Path.Combine(dbDir, $"{id}.vfp");
                
                if (File.Exists(fpPath))
                {
                    Console.WriteLine($"Ignoré {id} (existe déjà)");
                    continue;
                }
                
                Console.WriteLine($"Traitement de {id}...");
                
                var source = new VFPFingerprintSource(videoFile);
                source.StopTime = TimeSpan.FromSeconds(60); // Première minute uniquement
                
                var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                    source,
                    progressDelegate: (p) => Console.Write($"\r  Progression : {p}%")
                );
                
                if (fp != null)
                {
                    fp.ID = Guid.NewGuid();
                    fp.OriginalFilename = Path.GetFileName(videoFile);
                    fp.Save(fpPath);
                    processed++;
                    Console.WriteLine($"\r  ✓ Empreinte enregistrée pour {id}");
                }
            }
            
            Console.WriteLine($"\n✓ {processed} vidéos traitées");
            LoadDatabase();
        }
        
        public async Task SearchDatabase()
        {
            Console.Write("Entrez le nom du fichier vidéo de requête : ");
            string queryFile = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(queryFile))
            {
                Console.WriteLine("Fichier introuvable !");
                return;
            }
            
            Console.Write("Entrez le seuil de similarité (30 par défaut) : ");
            if (!int.TryParse(Console.ReadLine(), out int threshold))
                threshold = 30;
            
            Console.WriteLine("Génération de l'empreinte de requête...");
            var queryFp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                new VFPFingerprintSource(queryFile) { StopTime = TimeSpan.FromSeconds(60) }
            );
            
            if (queryFp == null) return;
            
            Console.WriteLine($"Recherche dans {database.Count} empreintes...");
            
            var matches = new List<(string id, int score)>();
            
            foreach (var entry in database)
            {
                int score = VFPAnalyzer.Compare(queryFp, entry.Value, TimeSpan.FromMilliseconds(500));
                if (score < threshold)
                {
                    matches.Add((entry.Key, score));
                }
            }
            
            if (matches.Count > 0)
            {
                Console.WriteLine($"\n✓ {matches.Count} vidéo(s) similaire(s) trouvée(s) :");
                foreach (var match in matches.OrderBy(m => m.score))
                {
                    var fp = database[match.id];
                    Console.WriteLine($"  - {fp.OriginalFilename} (score : {match.score})");
                }
            }
            else
            {
                Console.WriteLine("\n✗ Aucune vidéo similaire trouvée");
            }
        }
        
        private void LoadDatabase()
        {
            database.Clear();
            
            if (!Directory.Exists(dbDir))
                return;
            
            var files = Directory.GetFiles(dbDir, "*.vfp");
            foreach (var file in files)
            {
                try
                {
                    var fp = VFPFingerPrint.Load(file);
                    string id = Path.GetFileNameWithoutExtension(file);
                    database[id] = fp;
                }
                catch { }
            }
            
            Console.WriteLine($"{database.Count} empreintes chargées depuis la base");
        }
    }
}
```

## Benchmarks de performance

| Opération | Durée | Taille du fichier | Temps de traitement | Utilisation mémoire |
|-----------|----------|-----------|----------------|------------|
| Générer une empreinte | 1 minute | 100 Mo | ~5 secondes | 200 Mo |
| Générer une empreinte | 10 minutes | 1 Go | ~45 secondes | 400 Mo |
| Comparer des empreintes | N/A | N/A | <1 ms | Minimal |
| Rechercher un fragment | 30 s dans 1 heure | N/A | ~100 ms | 100 Mo |
| Requête de base | N/A | 1000 vidéos | ~50 ms | 250 Mo |

## Résumé

Vous avez maintenant appris à :

- ✅ Installer et configurer le Video Fingerprinting SDK avec les paquets NuGet appropriés
- ✅ Générer des empreintes à partir de fichiers vidéo
- ✅ Comparer des vidéos pour la similarité
- ✅ Rechercher des fragments dans des vidéos
- ✅ Construire et interroger une base d'empreintes
- ✅ Gérer les problèmes courants et optimiser les performances

Le Video Fingerprinting SDK fournit une base puissante pour les applications d'identification de contenu, de détection de doublons et de surveillance des médias. Commencez par les exemples simples et incorporez progressivement des fonctionnalités plus avancées à mesure que vos besoins évoluent.

Félicitations ! Vous êtes maintenant prêt à construire des applications d'empreinte vidéo puissantes avec le SDK VisioForge.
