---
title: API .NET Video Fingerprinting — générer et comparer
description: Documentation complète de l'API du VisioForge Video Fingerprinting SDK pour générer, comparer et rechercher des empreintes vidéo avec exemples de code.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - MP4
  - C#
primary_api_classes:
  - VFPFingerprintSource
  - VFPAnalyzer

---

# Documentation de l'API .NET du Video Fingerprinting SDK

## Vue d'ensemble

Le namespace VisioForge Video Fingerprinting fournit des fonctionnalités puissantes pour l'identification, la comparaison et la recherche de contenu vidéo. Il permet aux applications de :

- Générer des empreintes uniques à partir de fichiers vidéo pour l'identification de contenu
- Comparer des vidéos pour déterminer la similarité et détecter les doublons
- Rechercher des fragments vidéo dans des vidéos plus longues (par exemple, trouver des publicités, des intros ou des scènes spécifiques)
- Comparer des images individuelles pour la détection de similarité (Windows uniquement)
- Traiter directement des images vidéo pour générer des empreintes à partir de flux ou de contenu généré

## Table des matières

- [Classe VFPAnalyzer](#vfpanalyzer-class)
- [Classe VFPFingerPrint](#vfpfingerprint-class)
- [Classe VFPFingerprintSource](#vfpfingerprintsource-class)
- [Classe VFPCompare](#vfpcompare-class)
- [Classe VFPSearch](#vfpsearch-class)
- [Classe VFPFingerPrintDB](#vfpfingerprintdb-class)
- [Classe VFPFingerprintFromFrames](#vfpfingerprintfromframes-class)
- [Types de support](#supporting-types)
- [Délégués](#delegates)

## Classe VFPAnalyzer { #vfpanalyzer-class }

Point d'entrée principal des opérations d'empreinte vidéo. Cette classe fournit des méthodes statiques de haut niveau pour l'analyse, la comparaison et la recherche.

### Propriétés

#### DebugDir

```csharp
public static string DebugDir { get; set; }
```

**Description :** chemin du répertoire pour la sortie de débogage. Lorsqu'il est défini, les résultats intermédiaires de traitement peuvent être enregistrés à des fins de dépannage.

**Valeur par défaut :** `null` (sortie de débogage désactivée)

**Exemple :**

```csharp
// Activer la sortie de débogage
VFPAnalyzer.DebugDir = @"C:\Temp\VFP_Debug";

// Désactiver la sortie de débogage
VFPAnalyzer.DebugDir = null;
```

### Méthodes

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string vfpLicense)
```

**Description :** définit la clé de licence du Video Fingerprinting SDK. Doit être appelée avant d'utiliser toute fonctionnalité d'empreinte.

**Paramètres :**

- `vfpLicense` (string) : votre clé de licence VisioForge

**Exemple :**

```csharp
// Définir la clé de licence au démarrage de l'application
VFPAnalyzer.SetLicenseKey("YOUR-LICENSE-KEY-HERE");
```

#### GetComparingFingerprintForVideoFileAsync

```csharp
public static async Task<VFPFingerPrint> GetComparingFingerprintForVideoFileAsync(
    VFPFingerprintSource source,
    VFPErrorCallback errorDelegate = null,
    VFPProgressCallback progressDelegate = null)
```

**Description :** génère une empreinte optimisée pour les opérations de comparaison de vidéo entière.

**Paramètres :**

- `source` (VFPFingerprintSource) : configuration de la source vidéo, comprenant le chemin du fichier, la plage temporelle et les options de traitement
- `errorDelegate` (VFPErrorCallback) : callback optionnel pour les messages d'erreur
- `progressDelegate` (VFPProgressCallback) : callback optionnel pour les mises à jour de progression (0-100)

**Retour :** `Task<VFPFingerPrint>` — empreinte générée ou `null` si une erreur s'est produite

**Cas d'usage :** comparer des vidéos entières ou de larges segments pour déterminer la similarité globale

**Exemple :**

```csharp
// Utilisation de base
var source = new VFPFingerprintSource(@"C:\Videos\movie.mp4");
var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);

if (fingerprint != null)
{
    fingerprint.Save(@"C:\Fingerprints\movie.vsigx");
    Console.WriteLine($"Empreinte créée pour une durée de {fingerprint.Duration}");
}

// Avec gestion d'erreur et rapport de progression
var source2 = new VFPFingerprintSource(@"C:\Videos\video.mp4")
{
    StartTime = TimeSpan.FromMinutes(5),
    StopTime = TimeSpan.FromMinutes(10)
};

var fingerprint2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
    source2,
    error => Console.WriteLine($"Erreur : {error}"),
    progress => Console.WriteLine($"Progression : {progress}%"));

// Avec zones ignorées (par exemple logos, filigranes)
var source3 = new VFPFingerprintSource(@"C:\Videos\broadcast.mp4");
source3.IgnoredAreas.Add(new Rect(1700, 50, 1870, 150)); // Logo en haut à droite
source3.IgnoredAreas.Add(new Rect(100, 950, 300, 1000)); // Horodatage en bas

var fingerprint3 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source3);
```

#### GetSearchFingerprintForVideoFileAsync

```csharp
public static async Task<VFPFingerPrint> GetSearchFingerprintForVideoFileAsync(
    VFPFingerprintSource source,
    VFPErrorCallback errorDelegate = null,
    VFPProgressCallback progressDelegate = null)
```

**Description :** génère une empreinte optimisée pour les opérations de recherche de fragments.

**Paramètres :**

- `source` (VFPFingerprintSource) : configuration de la source vidéo
- `errorDelegate` (VFPErrorCallback) : callback d'erreur optionnel
- `progressDelegate` (VFPProgressCallback) : callback de progression optionnel

**Retour :** `Task<VFPFingerPrint>` — empreinte générée ou `null` si une erreur s'est produite

**Cas d'usage :** créer des empreintes de clips courts à localiser dans des vidéos de longue durée

**Exemple :**

```csharp
// Créer l'empreinte d'une publicité
var commercialSource = new VFPFingerprintSource(@"C:\Videos\commercial.mp4");
var commercialFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    commercialSource,
    error => Console.WriteLine($"Erreur : {error}"),
    progress => Console.WriteLine($"Traitement : {progress}%"));

// Créer l'empreinte d'une scène spécifique
var sceneSource = new VFPFingerprintSource(@"C:\Videos\movie.mp4")
{
    StartTime = TimeSpan.FromMinutes(42),
    StopTime = TimeSpan.FromMinutes(43)
};

var sceneFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(sceneSource);

if (sceneFp != null)
{
    sceneFp.Tag = "Scène d'action au pont";
    sceneFp.Save(@"C:\Fingerprints\scene.vsigx");
}
```

#### Compare

```csharp
public static int Compare(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    TimeSpan shift)
```

**Description :** compare deux empreintes vidéo pour déterminer leur similarité.

**Paramètres :**

- `fp1` (VFPFingerPrint) : première empreinte
- `fp2` (VFPFingerPrint) : seconde empreinte
- `shift` (TimeSpan) : décalage temporel maximum autorisé pendant la comparaison

**Retour :** `int` — score de différence (plus bas = plus similaire), ou `Int32.MaxValue` si l'une des empreintes est null

**Exemple :**

```csharp
// Charger deux empreintes
var fp1 = VFPFingerPrint.Load(@"C:\Fingerprints\video1.vsigx");
var fp2 = VFPFingerPrint.Load(@"C:\Fingerprints\video2.vsigx");

// Comparer avec une tolérance de décalage de 5 secondes
int difference = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromSeconds(5));

// Interpréter les résultats
if (difference < 5)
{
    Console.WriteLine("Les vidéos sont presque identiques");
}
else if (difference < 15)
{
    Console.WriteLine("Les vidéos sont très similaires");
}
else if (difference < 30)
{
    Console.WriteLine("Les vidéos ont un contenu similaire avec des différences");
}
else if (difference < 100)
{
    Console.WriteLine("Les vidéos sont liées mais significativement différentes");
}
else
{
    Console.WriteLine("Les vidéos sont complètement différentes");
}

// Comparaison par lot
var fingerprints = new List<VFPFingerPrint>();
foreach (var file in Directory.GetFiles(@"C:\Fingerprints", "*.vsigx"))
{
    fingerprints.Add(VFPFingerPrint.Load(file));
}

var referenceFp = fingerprints[0];
foreach (var fp in fingerprints.Skip(1))
{
    int diff = VFPAnalyzer.Compare(referenceFp, fp, TimeSpan.FromSeconds(3));
    Console.WriteLine($"{fp.OriginalFilename} : Différence = {diff}");
}
```

#### Search / SearchAsync

```csharp
public static List<TimeSpan> Search(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    TimeSpan duration,
    int maxDifference,
    bool allowMultipleFragments)

public static Task<List<TimeSpan>> SearchAsync(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    TimeSpan duration,
    int maxDifference,
    bool allowMultipleFragments)
```

**Description :** recherche les occurrences d'un fragment vidéo dans une vidéo plus longue.

**Paramètres :**

- `fp1` (VFPFingerPrint) : empreinte du fragment (aiguille)
- `fp2` (VFPFingerPrint) : vidéo dans laquelle rechercher (botte de foin)
- `duration` (TimeSpan) : durée du fragment (empêche les correspondances qui se chevauchent)
- `maxDifference` (int) : différence maximale autorisée (typique : 5-20)
- `allowMultipleFragments` (bool) : trouver toutes les occurrences ou la première correspondance uniquement

**Retour :** `List<TimeSpan>` — horodatages où des correspondances ont été trouvées

**Exemple :**

```csharp
// Rechercher une publicité dans un enregistrement
var commercialFp = VFPFingerPrint.Load(@"C:\Fingerprints\commercial.vsigx");
var recordingFp = VFPFingerPrint.Load(@"C:\Fingerprints\tv_recording.vsigx");

// Trouver toutes les occurrences
var matches = await VFPAnalyzer.SearchAsync(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30), // Durée de la publicité
    maxDifference: 10,
    allowMultipleFragments: true);

foreach (var timestamp in matches)
{
    Console.WriteLine($"Publicité trouvée à : {timestamp:hh\\:mm\\:ss}");
}

// Trouver uniquement la première occurrence
var firstMatch = VFPAnalyzer.Search(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30),
    maxDifference: 15,
    allowMultipleFragments: false);

if (firstMatch.Any())
{
    Console.WriteLine($"Première occurrence à : {firstMatch[0]}");
}

// Recherche avec correspondance plus stricte
var exactMatches = VFPAnalyzer.Search(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30),
    maxDifference: 5, // Très strict
    allowMultipleFragments: true);
```

#### CompareVideoFilesAsync

```csharp
public static async Task<bool> CompareVideoFilesAsync(
    VFPFingerprintSource file1,
    VFPFingerprintSource file2,
    TimeSpan shift,
    VFPErrorCallback errorCallback,
    int threshold = 500)
```

**Description :** méthode de commodité qui génère les empreintes et compare deux fichiers vidéo en une seule opération.

**Paramètres :**

- `file1` (VFPFingerprintSource) : configuration de la première vidéo
- `file2` (VFPFingerprintSource) : configuration de la seconde vidéo
- `shift` (TimeSpan) : décalage temporel maximum autorisé
- `errorCallback` (VFPErrorCallback) : callback d'erreur
- `threshold` (int) : différence maximale à considérer comme correspondance (par défaut : 500)

**Retour :** `Task<bool>` — `true` si les vidéos correspondent (différence < seuil), sinon `false`

**Exemple :**

```csharp
// Comparer deux fichiers vidéo directement
var file1 = new VFPFingerprintSource(@"C:\Videos\original.mp4");
var file2 = new VFPFingerprintSource(@"C:\Videos\copy.mp4");

bool areIdentical = await VFPAnalyzer.CompareVideoFilesAsync(
    file1,
    file2,
    TimeSpan.FromSeconds(5),
    error => Console.WriteLine($"Erreur : {error}"),
    threshold: 20);

if (areIdentical)
{
    Console.WriteLine("Les vidéos sont identiques ou très similaires");
}

// Comparer avec un traitement personnalisé
var source1 = new VFPFingerprintSource(@"C:\Videos\video1.mp4")
{
    CustomResolution = new Size(640, 480),
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromMinutes(5)
};

var source2 = new VFPFingerprintSource(@"C:\Videos\video2.mp4")
{
    CustomResolution = new Size(640, 480),
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromMinutes(5)
};

bool match = await VFPAnalyzer.CompareVideoFilesAsync(
    source1,
    source2,
    TimeSpan.FromSeconds(3),
    null,
    threshold: 50);
```

## Classe VFPFingerPrint { #vfpfingerprint-class }

Représente une empreinte vidéo avec des métadonnées et la prise en charge de la sérialisation.

### Propriétés

```csharp
public byte[] Data { get; set; }
public TimeSpan Duration { get; set; }
public Guid ID { get; set; }
public string OriginalFilename { get; set; }
public TimeSpan OriginalDuration { get; set; }
public string Tag { get; set; }
public int Width { get; set; }
public int Height { get; set; }
public double FrameRate { get; set; }
public List<Rect> IgnoredAreas { get; set; }
```

### Méthodes

#### Load (statique)

```csharp
public static VFPFingerPrint Load(string filename)
public static VFPFingerPrint Load(byte[] data)
```

**Description :** charge une empreinte depuis un fichier ou la mémoire.

**Paramètres :**

- `filename` (string) : chemin du fichier d'empreinte
- `data` (byte[]) : données d'empreinte en mémoire

**Retour :** `VFPFingerPrint` — objet empreinte chargé

**Exemple :**

```csharp
// Charger depuis un fichier
var fingerprint = VFPFingerPrint.Load(@"C:\Fingerprints\video.vsigx");
Console.WriteLine($"Chargée : {fingerprint.OriginalFilename}");
Console.WriteLine($"Durée : {fingerprint.Duration}");
Console.WriteLine($"ID : {fingerprint.ID}");

// Charger depuis la mémoire
byte[] fpData = File.ReadAllBytes(@"C:\Fingerprints\video.vsigx");
var fingerprint2 = VFPFingerPrint.Load(fpData);

// Charger plusieurs empreintes
var fingerprints = new List<VFPFingerPrint>();
foreach (var file in Directory.GetFiles(@"C:\Fingerprints", "*.vsigx"))
{
    try
    {
        var fp = VFPFingerPrint.Load(file);
        fingerprints.Add(fp);
        Console.WriteLine($"Chargée : {fp.OriginalFilename} ({fp.Duration})");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Échec du chargement de {file} : {ex.Message}");
    }
}
```

#### Save

```csharp
public void Save(string filename)
public byte[] Save()
```

**Description :** enregistre l'empreinte dans un fichier ou en mémoire. Extension par défaut : `.vsigx`

**Paramètres :**

- `filename` (string) : chemin du fichier de sortie

**Retour :** `byte[]` — données d'empreinte sérialisées (version mémoire)

**Exemple :**

```csharp
// Enregistrer vers un fichier
fingerprint.Save(@"C:\Fingerprints\output.vsigx");

// Enregistrer en mémoire
byte[] data = fingerprint.Save();
File.WriteAllBytes(@"C:\Backup\fingerprint.vsigx", data);

// Enregistrer avec métadonnées
fingerprint.Tag = "Scène importante à 00:42:00";
fingerprint.Save(@"C:\Fingerprints\tagged.vsigx");

// Enregistrement par lot avec nommage organisé
foreach (var fp in fingerprints)
{
    string safeName = Path.GetFileNameWithoutExtension(fp.OriginalFilename)
        .Replace(" ", "_")
        .Replace(".", "_");
    string outputPath = Path.Combine(
        @"C:\Fingerprints",
        $"{safeName}_{fp.ID}.vsigx");
    fp.Save(outputPath);
}
```

## Classe VFPFingerprintSource { #vfpfingerprintsource-class }

Configuration des opérations d'empreinte vidéo.

### Constructeur

```csharp
public VFPFingerprintSource(string filename)
```

**Description :** crée une nouvelle configuration source.

**Paramètres :**

- `filename` (string) : chemin du fichier vidéo

**Lève :** `FileNotFoundException` si le fichier n'existe pas

### Propriétés

```csharp
public string Filename { get; set; }
public TimeSpan StartTime { get; set; }
public TimeSpan StopTime { get; set; }
public Rect CustomCropSize { get; set; }
public Size CustomResolution { get; set; }
public List<Rect> IgnoredAreas { get; }
public TimeSpan OriginalDuration { get; set; }
```

- **`Filename`** (`string`) : chemin du fichier vidéo source
- **`StartTime`** (`TimeSpan`) : heure de début pour l'empreinte (par défaut : 0)
- **`StopTime`** (`TimeSpan`) : heure de fin pour l'empreinte (par défaut : durée de la vidéo)
- **`CustomCropSize`** (`Rect`) : rectangle de rognage (distances Left, Top, Right, Bottom)
- **`CustomResolution`** (`Size`) : résolution cible (Empty = pas de redimensionnement)
- **`IgnoredAreas`** (`List<Rect>`) : régions à exclure (par exemple logos, horodatages)
- **`OriginalDuration`** (`TimeSpan`) : durée totale de la vidéo (auto-renseignée)

### Exemples

```csharp
// Configuration de base
var source = new VFPFingerprintSource(@"C:\Videos\movie.mp4");

// Traiter une plage temporelle spécifique
var source2 = new VFPFingerprintSource(@"C:\Videos\long_video.mp4")
{
    StartTime = TimeSpan.FromMinutes(10),
    StopTime = TimeSpan.FromMinutes(20)
};

// Rogner vers la région d'intérêt
var source3 = new VFPFingerprintSource(@"C:\Videos\video.mp4")
{
    CustomCropSize = new Rect(100, 100, 1820, 980) // Supprimer les bordures
};

// Redimensionner pour un traitement plus rapide
var source4 = new VFPFingerprintSource(@"C:\Videos\4k_video.mp4")
{
    CustomResolution = new Size(1280, 720) // Réduire depuis 4K
};

// Ignorer les superpositions et logos
var source5 = new VFPFingerprintSource(@"C:\Videos\broadcast.mp4");
source5.IgnoredAreas.Add(new Rect(1700, 50, 1870, 150)); // Logo de chaîne
source5.IgnoredAreas.Add(new Rect(50, 50, 250, 100)); // Bug du réseau
source5.IgnoredAreas.Add(new Rect(100, 950, 400, 1000)); // Bandeau défilant

// Options de traitement combinées
var source6 = new VFPFingerprintSource(@"C:\Videos\tv_show.mp4")
{
    StartTime = TimeSpan.FromSeconds(90), // Sauter l'intro
    StopTime = TimeSpan.FromMinutes(42), // Avant le générique
    CustomResolution = new Size(640, 480),
    CustomCropSize = new Rect(60, 0, 1860, 1080) // Supprimer le pillarboxing
};
source6.IgnoredAreas.Add(new Rect(1600, 100, 1800, 200));
```

## Classe VFPCompare { #vfpcompare-class }

Fonctionnalité de comparaison d'empreintes de bas niveau.

### Méthodes

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string licenseKey)
```

**Description :** définit la clé de licence du SDK.

**Exemple :**

```csharp
VFPCompare.SetLicenseKey("YOUR-LICENSE-KEY");
```

#### Process

```csharp
public static int Process(
    IntPtr ptr,
    int w,
    int h,
    int s,
    TimeSpan dTime,
    ref VFPCompareData data)
```

**Description :** traite une image RGB24 pour l'empreinte de comparaison.

**Paramètres :**

- `ptr` (IntPtr) : pointeur vers les données d'image RGB24
- `w` (int) : largeur de l'image
- `h` (int) : hauteur de l'image
- `s` (int) : stride (octets par ligne)
- `dTime` (TimeSpan) : horodatage de l'image
- `data` (ref VFPCompareData) : structure de données de comparaison

**Retour :** `int` — code de statut (0 = succès)

**Exemple :**

```csharp
// Initialiser les données de comparaison
var compareData = new VFPCompareData(durationInSeconds: 120);

// Traiter les images (généralement fait en interne par VFPAnalyzer)
IntPtr frameData = GetRGB24Frame(); // Votre source d'images
int result = VFPCompare.Process(
    frameData,
    1920, // largeur
    1080, // hauteur
    5760, // stride pour 1920x3 RGB24
    TimeSpan.FromSeconds(1.5),
    ref compareData);

if (result == 0)
{
    Console.WriteLine("Image traitée avec succès");
}

// Construire l'empreinte après le traitement de toutes les images
IntPtr fpData = VFPCompare.Build(out long length, ref compareData);

// Nettoyer
compareData.Free();
```

#### Build

```csharp
public static IntPtr Build(
    out long length,
    ref VFPCompareData video)
```

**Description :** construit l'empreinte à partir des images traitées.

**Paramètres :**

- `length` (out long) : taille des données d'empreinte
- `video` (ref VFPCompareData) : données d'images traitées

**Retour :** `IntPtr` — pointeur vers les données d'empreinte

#### Compare

```csharp
public static double Compare(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    int maxDifference)
```

**Description :** compare deux empreintes.

**Paramètres :**

- `fp1` (VFPFingerPrint) : première empreinte
- `fp2` (VFPFingerPrint) : seconde empreinte
- `maxDifference` (int) : différence maximale autorisée

**Retour :** `double` — score de similarité (0-100, plus élevé = plus similaire)

**Exemple :**

```csharp
var fp1 = VFPFingerPrint.Load(@"C:\Fingerprints\video1.vsigx");
var fp2 = VFPFingerPrint.Load(@"C:\Fingerprints\video2.vsigx");

double similarity = VFPCompare.Compare(fp1, fp2, maxDifference: 50);
Console.WriteLine($"Similarité : {similarity:F2}%");

if (similarity > 90)
{
    Console.WriteLine("Les vidéos sont très similaires");
}
else if (similarity > 70)
{
    Console.WriteLine("Les vidéos ont des similarités significatives");
}
else
{
    Console.WriteLine("Les vidéos sont différentes");
}
```

## Classe VFPSearch { #vfpsearch-class }

Fonctionnalité de recherche d'empreintes de bas niveau.

### Méthodes

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string licenseKey)
```

**Description :** définit la clé de licence du SDK.

#### Process

```csharp
public static int Process(
    IntPtr ptr,
    int w,
    int h,
    int s,
    TimeSpan dTime,
    ref VFPSearchData data)
```

**Description :** traite une image vidéo pour l'empreinte de recherche.

**Paramètres :**

- `ptr` (IntPtr) : pointeur vers les données d'image RGB24
- `w` (int) : largeur de l'image
- `h` (int) : hauteur de l'image
- `s` (int) : stride
- `dTime` (TimeSpan) : horodatage de l'image
- `data` (ref VFPSearchData) : structure de données de recherche

**Retour :** `int` — code de statut

#### Build

```csharp
public static IntPtr Build(
    out long length,
    ref VFPSearchData data)
```

**Description :** construit l'empreinte de recherche.

**Paramètres :**

- `length` (out long) : taille des données d'empreinte
- `data` (ref VFPSearchData) : images traitées

**Retour :** `IntPtr` — pointeur vers les données d'empreinte

#### Search

```csharp
public static int Search(
    VFPFingerPrint fp1,
    int skip1,
    VFPFingerPrint fp2,
    int skip2,
    out double difference,
    int maxDiff)
```

**Description :** recherche un fragment dans une vidéo.

**Paramètres :**

- `fp1` (VFPFingerPrint) : empreinte du fragment
- `skip1` (int) : images à ignorer au début de `fp1`
- `fp2` (VFPFingerPrint) : empreinte de la vidéo principale
- `skip2` (int) : images à ignorer au début de `fp2`
- `difference` (out double) : score de différence de correspondance
- `maxDiff` (int) : différence maximale autorisée

**Retour :** `int` — position où trouvé (en secondes) ou Int32.MaxValue si non trouvé

**Exemple :**

```csharp
// Rechercher un fragment
var fragmentFp = VFPFingerPrint.Load(@"C:\Fingerprints\fragment.vsigx");
var videoFp = VFPFingerPrint.Load(@"C:\Fingerprints\full_video.vsigx");

int position = VFPSearch.Search(
    fragmentFp,
    skip1: 0,
    videoFp,
    skip2: 0,
    out double matchDifference,
    maxDiff: 20);

if (position != Int32.MaxValue)
{
    Console.WriteLine($"Fragment trouvé à {position} secondes");
    Console.WriteLine($"Qualité de correspondance : {matchDifference}");
}
else
{
    Console.WriteLine("Fragment non trouvé");
}

// Rechercher depuis une position spécifique
int nextPosition = VFPSearch.Search(
    fragmentFp,
    0,
    videoFp,
    position + 30, // Sauter la première correspondance
    out matchDifference,
    maxDifference: 20);
```

## Classe VFPFingerPrintDB { #vfpfingerprintdb-class }

Base de données pour gérer des collections d'empreintes.

### Propriétés

```csharp
public List<VFPFingerPrint> Items { get; }
```

### Méthodes

#### Save

```csharp
public void Save(string filename)
```

**Description :** enregistre la base de données dans un fichier.

**Exemple :**

```csharp
var db = new VFPFingerPrintDB();

// Ajouter des empreintes
foreach (var videoFile in Directory.GetFiles(@"C:\Videos", "*.mp4"))
{
    var source = new VFPFingerprintSource(videoFile);
    var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    if (fp != null)
    {
        db.Items.Add(fp);
    }
}

// Enregistrer la base de données
db.Save(@"C:\Database\fingerprints.db");
Console.WriteLine($"{db.Items.Count} empreintes enregistrées en base");
```

#### Load (statique)

```csharp
public static VFPFingerPrintDB Load(string filename)
```

**Description :** charge la base de données depuis un fichier.

**Exemple :**

```csharp
// Charger une base existante
var db = VFPFingerPrintDB.Load(@"C:\Database\fingerprints.db");
Console.WriteLine($"{db.Items.Count} empreintes chargées");

// Interroger la base
var recentVideos = db.Items
    .Where(fp => fp.OriginalDuration > TimeSpan.FromMinutes(30))
    .OrderBy(fp => fp.OriginalFilename)
    .ToList();

foreach (var fp in recentVideos)
{
    Console.WriteLine($"{fp.OriginalFilename} : {fp.Duration}");
}
```

#### ContainsFile

```csharp
public bool ContainsFile(VFPFingerprintSource source)
```

**Description :** vérifie si la base contient une empreinte pour la source.

**Exemple :**

```csharp
var source = new VFPFingerprintSource(@"C:\Videos\new_video.mp4");

if (!db.ContainsFile(source))
{
    // Générer et ajouter une nouvelle empreinte
    var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    db.Items.Add(fp);
    db.Save(@"C:\Database\fingerprints.db");
}
else
{
    Console.WriteLine("L'empreinte existe déjà dans la base");
}
```

#### GetFingerprint

```csharp
public VFPFingerPrint GetFingerprint(VFPFingerprintSource source)
```

**Description :** récupère l'empreinte correspondant à la source.

**Exemple :**

```csharp
var source = new VFPFingerprintSource(@"C:\Videos\video.mp4");
var fp = db.GetFingerprint(source);

if (fp != null)
{
    Console.WriteLine($"Empreinte trouvée : {fp.ID}");
    Console.WriteLine($"Durée : {fp.Duration}");
    Console.WriteLine($"Étiquette : {fp.Tag}");
}
```

## Classe VFPFingerprintFromFrames { #vfpfingerprintfromframes-class }

Crée des empreintes à partir d'images individuelles. Disponible sur toutes les plateformes.

### Constructeur

```csharp
public VFPFingerprintFromFrames(
    double frameRate,
    int width,
    int height,
    TimeSpan totalDuration)
```

**Description :** initialise le générateur d'empreinte basé sur les images.

**Paramètres :**

- `frameRate` (double) : fréquence d'images de la vidéo
- `width` (int) : largeur des images
- `height` (int) : hauteur des images
- `totalDuration` (TimeSpan) : durée totale de la vidéo

### Méthodes

#### Push

```csharp
public void Push(byte[] rgb24frame)           // Toutes plateformes
public void Push(Bitmap frame)                // Windows uniquement
public void Push(SKBitmap frame)              // Toutes plateformes (nouveau)
public void Push(IntPtr rgb24frame, int rgb24frameSize)  // Toutes plateformes
```

**Description :** ajoute des images au processus de génération d'empreinte. Les images doivent correspondre aux dimensions configurées.

- **byte[]** : données d'image RGB24 brutes (multiplateforme)
- **Bitmap** : System.Drawing.Bitmap (Windows uniquement)
- **SKBitmap** : bitmap SkiaSharp pour la prise en charge multiplateforme
- **IntPtr** : pointeur vers les données d'image RGB24 (multiplateforme)

**Exemple :**

```csharp
// Créer le générateur
var builder = new VFPFingerprintFromFrames(
    frameRate: 30.0,
    width: 1920,
    height: 1080,
    totalDuration: TimeSpan.FromMinutes(5));

// Multiplateforme : ajouter des images comme tableaux d'octets
foreach (var frameData in videoStream.GetFrames())
{
    builder.Push(frameData); // byte[] RGB24
}

// Multiplateforme : ajouter des bitmaps SkiaSharp
using (var skBitmap = SKBitmap.Decode(imageData))
{
    builder.Push(skBitmap);
}

// Windows uniquement : ajouter des images System.Drawing.Bitmap
#if NET_WINDOWS
for (int i = 0; i < frameCount; i++)
{
    Bitmap frame = GetFrameAsBitmap(i);
    builder.Push(frame);
}
#endif

// Multiplateforme : ajouter des images via IntPtr
unsafe
{
    fixed (byte* ptr = frameData)
    {
        builder.Push(new IntPtr(ptr), frameData.Length);
    }
}

// Construire l'empreinte finale
var fingerprint = builder.Build();
fingerprint.OriginalFilename = "stream_capture.mp4";
fingerprint.Save(@"C:\Fingerprints\stream.vsigx");
```

#### Build

```csharp
public VFPFingerPrint Build()
```

**Description :** génère l'empreinte à partir des images traitées.

**Retour :** `VFPFingerPrint` — empreinte générée

## Types de support { #supporting-types }

### VFPCompareData

```csharp
public struct VFPCompareData
{
    public IntPtr Data { get; set; }
    public VFPCompareData(int duration)
    public void Free()
}
```

**Description :** gère les données de comparaison natives.

**Exemple :**

```csharp
// Créer et utiliser des données de comparaison
var data = new VFPCompareData(durationInSeconds: 60);
try
{
    // Traiter les images...
    // Construire l'empreinte...
}
finally
{
    data.Free(); // Toujours libérer la mémoire native
}
```

### VFPSearchData

```csharp
public class VFPSearchData : IDisposable
{
    public IntPtr Data { get; set; }
    public VFPSearchData(TimeSpan duration)
    public void Free()
    public void Dispose()
}
```

**Description :** gère les données de recherche natives avec libération automatique.

**Exemple :**

```csharp
// L'instruction using garantit le nettoyage approprié
using (var searchData = new VFPSearchData(TimeSpan.FromMinutes(2)))
{
    // Traiter les images pour l'empreinte de recherche
    // Construire l'empreinte
} // Libérée automatiquement
```

## Délégués { #delegates }

### VFPProgressCallback

```csharp
public delegate void VFPProgressCallback(int percent)
```

**Description :** signale la progression durant les opérations d'empreinte (0-100).

**Exemple :**

```csharp
// Affichage simple de progression
VFPProgressCallback progressCallback = (percent) =>
{
    Console.Write($"\rProgression : {percent}%");
    if (percent == 100) Console.WriteLine();
};

// Progression avec mise à jour de l'UI
VFPProgressCallback uiProgress = (percent) =>
{
    progressBar.Value = percent;
    labelStatus.Text = $"Traitement : {percent}%";
    Application.DoEvents();
};

// Progression avec vérification d'annulation
CancellationToken token = GetCancellationToken();
VFPProgressCallback cancellableProgress = (percent) =>
{
    if (token.IsCancellationRequested)
        throw new OperationCanceledException();
    UpdateProgress(percent);
};
```

### VFPErrorCallback

```csharp
public delegate void VFPErrorCallback(string error)
```

**Description :** signale les erreurs durant les opérations d'empreinte.

**Exemple :**

```csharp
// Journaliser les erreurs
VFPErrorCallback errorCallback = (error) =>
{
    logger.Error($"Erreur VFP : {error}");
    File.AppendAllText(@"C:\Logs\vfp_errors.log", 
        $"{DateTime.Now} : {error}{Environment.NewLine}");
};

// Afficher les erreurs à l'utilisateur
VFPErrorCallback userErrorCallback = (error) =>
{
    MessageBox.Show($"Erreur de traitement vidéo : {error}", 
        "Erreur d'empreinte", 
        MessageBoxButtons.OK, 
        MessageBoxIcon.Error);
};

// Collecter les erreurs pour un traitement par lot
var errors = new List<string>();
VFPErrorCallback collectErrors = (error) => errors.Add(error);
```
