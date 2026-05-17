---
title: Écrire des métadonnées audio en C# .NET - ID3, Vorbis
description: Ajoutez des métadonnées ID3, Vorbis Comments et MP4 aux fichiers audio avec VisioForge Media Blocks. Exemples pour MP3, OGG, M4A et WMA.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Encoding
  - Metadata
  - MP4
  - WMV
  - OGG
  - AAC
  - MP3
  - Vorbis
  - WMA
  - C#
primary_api_classes:
  - MediaFileTags
  - MP3OutputBlock
  - OGGVorbisOutputBlock
  - M4AOutputBlock
  - WMVOutputBlock
  - MediaBlocksPipeline
  - SystemAudioSourceBlock

---

# Écrire des étiquettes audio avec le Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Table des matières

- [Écrire des étiquettes audio avec le Media Blocks SDK](#ecrire-des-etiquettes-audio-avec-le-media-blocks-sdk)
  - [Table des matières](#table-des-matieres)
  - [Vue d'ensemble](#vue-densemble)
  - [Fonctionnalités principales](#fonctionnalites-principales)
  - [Formats audio pris en charge](#formats-audio-pris-en-charge)
  - [Prérequis](#prerequis)
  - [MediaFileTags : l'interface unifiée](#mediafiletags-linterface-unifiee)
  - [Exemples de code par format](#exemples-de-code-par-format)
    - [Sortie MP3 avec étiquettes ID3](#sortie-mp3-avec-etiquettes-id3)
    - [Sortie OGG Vorbis avec Vorbis Comments](#sortie-ogg-vorbis-avec-vorbis-comments)
    - [Sortie M4A avec métadonnées MP4](#sortie-m4a-avec-metadonnees-mp4)
    - [Sortie WMV/WMA avec métadonnées ASF](#sortie-wmvwma-avec-metadonnees-asf)
  - [Exemple complet d'enregistrement audio](#exemple-complet-denregistrement-audio)
  - [Scénarios d'étiquetage avancés](#scenarios-detiquetage-avances)
    - [Prise en charge des pochettes d'album](#prise-en-charge-des-pochettes-dalbum)
    - [Modification des étiquettes à l'exécution](#modification-des-etiquettes-a-lexecution)
    - [Albums multi-pistes](#albums-multi-pistes)
  - [Bonnes pratiques](#bonnes-pratiques)
    - [Qualité des données d'étiquette](#qualite-des-donnees-detiquette)
    - [Considérations de performance](#considerations-de-performance)
    - [Recommandations par format](#recommandations-par-format)
  - [Dépannage](#depannage)
    - [Problèmes courants et solutions](#problemes-courants-et-solutions)
    - [Déboguer l'écriture d'étiquettes](#deboguer-lecriture-detiquettes)
  - [Spécifications des formats d'étiquettes](#specifications-des-formats-detiquettes)
    - [Étiquettes ID3 (MP3)](#etiquettes-id3-mp3)
    - [Vorbis Comments (OGG)](#vorbis-comments-ogg)
    - [Métadonnées MP4 (M4A)](#metadonnees-mp4-m4a)
    - [Attributs ASF (WMV/WMA)](#attributs-asf-wmvwma)

## Vue d'ensemble

Le VisioForge Media Blocks SDK prend en charge l'écriture d'étiquettes de métadonnées audio dans les fichiers de sortie pour tous les principaux formats audio. Que vous construisiez une application de production musicale, un enregistreur de podcast ou un système de gestion de contenu audio, vous pouvez intégrer des métadonnées riches dans vos fichiers audio via une interface de programmation unifiée.

Ce guide montre comment ajouter des étiquettes de métadonnées telles que l'artiste, l'album, le titre, l'année, le genre et plus encore à des fichiers audio MP3, OGG Vorbis, M4A et WMV/WMA en utilisant des mécanismes d'étiquetage adaptés à chaque format tout en respectant les standards de l'industrie.

## Fonctionnalités principales

- **Prise en charge universelle des étiquettes** : écrivez des métadonnées au format MP3 (ID3), OGG (Vorbis Comments), M4A (atoms MP4) et WMV (attributs ASF)
- **Métadonnées complètes** : plus de 20 champs d'étiquette dont titre, artiste, album, année, numéros de piste, paroles et pochette
- **Conforme aux standards** : utilise les mécanismes d'étiquetage natifs des conteneurs pour une compatibilité optimale
- **API unifiée** : une instance unique `MediaFileTags` fonctionne pour tous les formats de sortie
- **Flexibilité à l'exécution** : modifiez les étiquettes avant l'exécution du pipeline

## Formats audio pris en charge

| Format | Conteneur | Système d'étiquette | Standards |
|--------|-----------|------------|-----------|
| **MP3** | MPEG-1 Layer 3 | ID3v1/ID3v2 | ISO/IEC 11172-3, ID3v2.4 |
| **OGG Vorbis** | OGG | Vorbis Comments | Spécification Xiph.Org |
| **M4A** | MP4/MPEG-4 | Atoms de métadonnées MP4 | ISO Base Media File Format |
| **WMV/WMA** | ASF | Attributs de métadonnées ASF | Spécification Microsoft ASF |

## Prérequis

- VisioForge Media Blocks SDK .NET
- .NET Framework 4.7.2+ ou .NET Core 3.1+ ou .NET 5+
- Compréhension de base des pipelines de traitement audio

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Outputs;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sources;
```

## MediaFileTags : l'interface unifiée

La classe `MediaFileTags` fournit une interface unifiée pour les métadonnées audio dans tous les formats pris en charge. Elle contient des champs de métadonnées communs et est mappée vers le format d'étiquettes approprié pour chaque conteneur de sortie par le bloc de sortie spécifique.

```csharp
// Créer les métadonnées audio
var audioTags = new MediaFileTags
{
    // Métadonnées de base
    Title = "Bohemian Rhapsody",
    Performers = new[] { "Queen" },
    Album = "A Night at the Opera",
    Year = 1975,
    
    // Informations de piste
    Track = 11,
    TrackCount = 12,
    Disc = 1,
    DiscCount = 1,
    
    // Genre et catégorisation
    Genres = new[] { "Progressive Rock", "Opera Rock" },
    
    // Métadonnées étendues
    Composers = new[] { "Freddie Mercury" },
    Conductor = "Roy Thomas Baker",
    Comment = "6-minute epic masterpiece",
    Copyright = "© 1975 Queen Productions Ltd.",
    
    // Métadonnées techniques
    BeatsPerMinute = 72,
    Grouping = "Epic Songs",
    
    // Paroles (pour les formats compatibles)
    Lyrics = @"Is this the real life?
Is this just fantasy?
Caught in a landslide
No escape from reality..."
};
```

Tous les champs de type tableau de chaînes (`Performers`, `Composers`, `Genres`, `AlbumArtists`) acceptent plusieurs valeurs et sont écrits sous forme de trames répétées dans les formats qui les prennent en charge (Vorbis Comments, ID3v2). Les champs numériques (`Year`, `Track`, `TrackCount`, `Disc`, `DiscCount`, `BeatsPerMinute`) sont de type `uint`.

## Exemples de code par format

### Sortie MP3 avec étiquettes ID3

Les fichiers MP3 utilisent des étiquettes ID3 (v1 et v2) pour le stockage des métadonnées. `MP3OutputBlock` écrit des étiquettes ID3 conformes aux standards via l'élément GStreamer `id3mux`.

```csharp
public async Task CreateMP3WithTags()
{
    // Configurer les paramètres de l'encodeur MP3
    var mp3Settings = new MP3EncoderSettings
    {
        Bitrate = 320,                              // Kbit/s
        RateControl = MP3EncoderRateControl.CBR    // CBR / ABR / VBR
    };
    
    // Créer les étiquettes de métadonnées
    var tags = new MediaFileTags
    {
        Title = "Summer Vibes",
        Performers = new[] { "Indie Artist" },
        Album = "Seasonal Collection",
        Year = 2025,
        Track = 3,
        TrackCount = 10,
        Genres = new[] { "Indie Pop", "Electronic" },
        Comment = "Recorded in home studio",
        Copyright = "© 2025 Independent Label"
    };
    
    // Créer le bloc de sortie MP3 avec les étiquettes
    var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings, tags);
    
    // Alternative : définir les étiquettes après la création via la propriété Tags
    // var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);
    // mp3Output.Tags = tags;
    
    // Construire le pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Ajouter une source audio (microphone, fichier, etc.)
    var audioSource = new SystemAudioSourceBlock();
    
    // Connecter la source directement à la sortie MP3
    pipeline.Connect(audioSource.Output, mp3Output.Input);
    
    // Démarrer l'enregistrement avec les métadonnées
    await pipeline.StartAsync();
    
    // L'enregistrement écrit les étiquettes ID3 dans le fichier MP3
    await Task.Delay(TimeSpan.FromSeconds(30));
    
    await pipeline.StopAsync();
}
```

### Sortie OGG Vorbis avec Vorbis Comments

Les fichiers OGG Vorbis utilisent les Vorbis Comments pour les métadonnées, qui sont intégrées directement dans le flux audio par l'encodeur Vorbis.

```csharp
public async Task CreateOGGWithTags()
{
    // Configurer les paramètres de l'encodeur Vorbis.
    // Quality est un int dans la plage [-1..10] (par défaut 4). Utilisé lorsque RateControl = Quality.
    var vorbisSettings = new VorbisEncoderSettings
    {
        Quality = 8,
        RateControl = VorbisEncoderRateControl.Quality
    };
    
    // Créer les métadonnées
    var tags = new MediaFileTags
    {
        Title = "Acoustic Session",
        Performers = new[] { "Folk Artist", "Guest Vocalist" },
        AlbumArtists = new[] { "Folk Artist" },
        Album = "Live Sessions",
        Year = 2025,
        Track = 1,
        Genres = new[] { "Folk", "Acoustic" },
        Composers = new[] { "Folk Artist", "Traditional" },
        Comment = "Recorded live at Studio A",
        Conductor = "Sound Engineer",
        Grouping = "Live Recordings",
        Lyrics = @"In the quiet of the morning
When the world begins to wake
There's a song within the silence..."
    };
    
    // Créer le bloc de sortie OGG avec les Vorbis Comments
    var oggOutput = new OGGVorbisOutputBlock("output.ogg", vorbisSettings, tags);
    
    // Construire et exécuter le pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Utiliser UniversalSourceBlock pour décoder n'importe quel format de fichier en audio brut
    var sourceSettings = await UniversalSourceSettings.CreateAsync(new Uri("input.wav"));
    var fileSource = new UniversalSourceBlock(sourceSettings);
    
    pipeline.Connect(fileSource.AudioOutput, oggOutput.Input);
    
    await pipeline.StartAsync();
    await pipeline.WaitForStopAsync();   // Attendre l'EOS
}
```

### Sortie M4A avec métadonnées MP4

Les fichiers M4A utilisent des atoms de métadonnées MP4, compatibles avec iTunes et la plupart des lecteurs multimédias. Le constructeur par défaut de `M4AOutputBlock` choisit un encodeur AAC par défaut ; utilisez la surcharge à 3 arguments pour choisir une implémentation AAC spécifique (`AVENCAACEncoderSettings`, `VOAACEncoderSettings` ou `MFAACEncoderSettings` sous Windows).

```csharp
public async Task CreateM4AWithTags()
{
    // Créer les métadonnées de podcast
    var tags = new MediaFileTags
    {
        Title = "Episode 42: The Future of AI",
        Performers = new[] { "Tech Podcast Host" },
        Album = "Weekly Tech Talk",
        Year = 2025,
        Track = 42,
        Genres = new[] { "Technology", "Podcast" },
        Comment = "Special guest interview with AI researcher",
        Copyright = "© 2025 Tech Media Network",
        Subtitle = "Exploring artificial intelligence trends",
        Grouping = "Season 3"
    };
    
    // Option A : la plus simple — encodeur AAC par défaut choisi en interne
    var m4aOutput = new M4AOutputBlock("podcast_episode_42.m4a", tags);
    
    // Option B : choisir un encodeur AAC spécifique et des paramètres de puits
    // var sinkSettings = new MP4SinkSettings("podcast_episode_42.m4a");
    // var aacSettings = new AVENCAACEncoderSettings { Bitrate = 256 }; // 256 Kbit/s
    // var m4aOutput = new M4AOutputBlock(sinkSettings, aacSettings, tags);
    
    // Configuration du pipeline pour l'enregistrement du podcast
    var pipeline = new MediaBlocksPipeline();
    var micSource = new SystemAudioSourceBlock();
    
    pipeline.Connect(micSource.Output, m4aOutput.Input);
    
    await pipeline.StartAsync();
}
```

### Sortie WMV/WMA avec métadonnées ASF

Les formats Windows Media utilisent des attributs de métadonnées ASF (Advanced Systems Format). `WMVOutputBlock` accepte un paramètre `MediaFileTags` et gère à la fois les sorties audio seules et audio+vidéo.

```csharp
public async Task CreateWMVWithTags()
{
    // Créer les métadonnées de présentation
    var tags = new MediaFileTags
    {
        Title = "Q4 Business Review",
        Performers = new[] { "CEO", "CFO", "VP Sales" },
        Album = "Corporate Presentations 2025",
        Year = 2025,
        Genres = new[] { "Business", "Corporate" },
        Comment = "Quarterly financial review and outlook",
        Copyright = "© 2025 Business Corp. Confidential",
        Conductor = "Meeting Organizer",
        Grouping = "Executive Presentations"
    };
    
    // Forme la plus simple — encodeurs par défaut, juste nom de fichier + étiquettes.
    // WMVOutputBlock utilisera les paramètres d'encodeur WMV/WMA par défaut en interne.
    var wmvOutput = new WMVOutputBlock("presentation.wmv", tags);
    
    // Alternative : passer des objets de paramètres explicites puits/vidéo/audio
    // var asfSettings = new ASFSinkSettings("presentation.wmv");
    // var wmvSettings = WMVEncoderBlock.GetDefaultSettings();
    // var wmaSettings = WMAEncoderBlock.GetDefaultSettings();
    // var wmvOutput = new WMVOutputBlock(asfSettings, wmvSettings, wmaSettings, tags);
    
    // Configuration pour l'enregistrement vidéo + audio
    var pipeline = new MediaBlocksPipeline();
    
    // Source vidéo — choisir le premier périphérique disponible
    var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
    var videoSource = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(videoDevice));
    
    // Source audio
    var audioSource = new SystemAudioSourceBlock();
    
    // Créer des pads d'entrée dynamiques sur la sortie WMV
    var videoPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Video);
    var audioPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Audio);
    
    pipeline.Connect(videoSource.Output, videoPad);
    pipeline.Connect(audioSource.Output, audioPad);
    
    await pipeline.StartAsync();
}
```

## Exemple complet d'enregistrement audio

Enregistrez la même source audio vers plusieurs formats de sortie étiquetés simultanément :

```csharp
public class AudioRecorderWithTags
{
    public async Task RecordAudioWithMetadata()
    {
        // Métadonnées riches partagées entre tous les fichiers de sortie
        var sessionTags = new MediaFileTags
        {
            Title = "Studio Session #1",
            Performers = new[] { "John Doe", "Jane Smith" },
            Album = "Demo Recordings",
            Year = 2025,
            Track = 1,
            Genres = new[] { "Rock", "Alternative" },
            Composers = new[] { "John Doe" },
            Comment = "First studio recording session",
            Copyright = "© 2025 Demo Productions",
            BeatsPerMinute = 120,
            Grouping = "Demo Sessions"
        };
        
        // Sortie MP3 (CBR 320 Kbit/s + étiquettes ID3)
        var mp3Output = new MP3OutputBlock(
            "session1.mp3",
            new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR },
            sessionTags);
        
        // Sortie OGG Vorbis (qualité maximale + Vorbis Comments)
        var oggOutput = new OGGVorbisOutputBlock(
            "session1.ogg",
            new VorbisEncoderSettings { Quality = 10, RateControl = VorbisEncoderRateControl.Quality },
            sessionTags);
        
        // Sortie M4A (AAC par défaut + atoms MP4)
        var m4aOutput = new M4AOutputBlock("session1.m4a", sessionTags);
        
        // Pipeline avec une source audio unique répartie vers les trois fichiers
        var pipeline = new MediaBlocksPipeline();
        var audioSource = new SystemAudioSourceBlock();
        
        // La connexion du même pad source à plusieurs puits insère automatiquement un tee
        pipeline.Connect(audioSource.Output, mp3Output.Input);
        pipeline.Connect(audioSource.Output, oggOutput.Input);
        pipeline.Connect(audioSource.Output, m4aOutput.Input);
        
        Console.WriteLine("Starting recording with metadata...");
        await pipeline.StartAsync();
        
        await Task.Delay(TimeSpan.FromMinutes(3));
        
        Console.WriteLine("Stopping recording...");
        await pipeline.StopAsync();
        
        Console.WriteLine("Recording complete — files written with metadata:");
        Console.WriteLine("- session1.mp3 (ID3 tags)");
        Console.WriteLine("- session1.ogg (Vorbis comments)");
        Console.WriteLine("- session1.m4a (MP4 metadata)");
    }
}
```

## Scénarios d'étiquetage avancés

### Prise en charge des pochettes d'album

Attachez des pochettes d'album aux formats compatibles (MP3, M4A, WMV). Sous Windows, `MediaFileTags.Pictures` accepte `System.Drawing.Bitmap[]` ; les builds multiplateformes utilisent `IBitmap[]`.

```csharp
var tags = new MediaFileTags
{
    Title = "Album Title Track",
    Performers = new[] { "Artist Name" },
    Album = "Album Name"
};

// Attacher la pochette d'album (Windows — System.Drawing)
#if NET_WINDOWS
if (File.Exists("album_cover.jpg"))
{
    var albumArt = new System.Drawing.Bitmap("album_cover.jpg");
    tags.Pictures = new[] { albumArt };
    tags.Pictures_Descriptions = new[] { "Front Cover" };
}
#endif

var mp3Output = new MP3OutputBlock(
    "track.mp3",
    new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR },
    tags);
```

### Modification des étiquettes à l'exécution

Définissez ou modifiez les étiquettes avant de démarrer le pipeline — une fois le pipeline démarré, la charge utile d'étiquettes pour cette sortie est déjà en cours d'émission.

```csharp
var mp3Settings = new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR };
var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);

// Affecter les étiquettes via la propriété Tags avant StartAsync
mp3Output.Tags = new MediaFileTags
{
    Title = "Live Recording",
    Performers = new[] { "Artist" }
};

// Ajuster les champs jusqu'au démarrage
mp3Output.Tags.Comment = $"Recorded on {DateTime.Now:yyyy-MM-dd}";
mp3Output.Tags.Year = (uint)DateTime.Now.Year;

await pipeline.StartAsync();
```

### Albums multi-pistes

Créez des métadonnées cohérentes pour toutes les pistes d'un album en utilisant un objet d'étiquette de base partagé :

```csharp
public class AlbumRecorder
{
    private readonly MediaFileTags _baseAlbumTags;
    private readonly MP3EncoderSettings _mp3Settings =
        new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR };

    public AlbumRecorder()
    {
        _baseAlbumTags = new MediaFileTags
        {
            Album = "My Album",
            AlbumArtists = new[] { "Main Artist" },
            Year = 2025,
            Genres = new[] { "Pop", "Electronic" },
            TrackCount = 12,
            Copyright = "© 2025 Record Label"
        };
    }

    public MP3OutputBlock CreateTrackOutput(int trackNumber, string title, string[] performers)
    {
        var trackTags = new MediaFileTags
        {
            // Hériter des métadonnées au niveau album
            Album = _baseAlbumTags.Album,
            AlbumArtists = _baseAlbumTags.AlbumArtists,
            Year = _baseAlbumTags.Year,
            Genres = _baseAlbumTags.Genres,
            TrackCount = _baseAlbumTags.TrackCount,
            Copyright = _baseAlbumTags.Copyright,

            // Métadonnées propres à la piste
            Track = (uint)trackNumber,
            Title = title,
            Performers = performers
        };

        return new MP3OutputBlock($"track_{trackNumber:D2}.mp3", _mp3Settings, trackTags);
    }
}
```

## Bonnes pratiques

### Qualité des données d'étiquette

- **Encodage cohérent** : utilisez UTF-8 pour les caractères internationaux
- **Informations complètes** : remplissez autant de champs d'étiquette pertinents que possible
- **Genres standardisés** : utilisez des noms de genre reconnus pour une meilleure compatibilité
- **Copyright correct** : incluez des mentions de droits d'auteur appropriées

### Considérations de performance

- **Taille des étiquettes** : limitez la longueur des champs texte pour éviter d'alourdir les fichiers
- **Compression d'image** : compressez les pochettes d'album de manière appropriée (JPEG recommandé)
- **Réutilisez les instances** : lors de la création de plusieurs fichiers, partagez les objets d'étiquette de base et ne redéfinissez que les champs par piste

### Recommandations par format

```csharp
// MP3 : ID3v2 prend en charge des métadonnées étendues
var mp3Tags = new MediaFileTags
{
    Title = "Song Title",
    Subtitle = "Song Subtitle",     // Trame ID3v2 TIT3
    Lyrics = "Full lyrics text",    // Trame USLT
    BeatsPerMinute = 128            // Trame TBPM
};

// OGG : les Vorbis Comments sont flexibles et gèrent nativement les champs multi-valeurs
var oggTags = new MediaFileTags
{
    Composers = new[] { "Composer 1", "Composer 2" },
    Performers = new[] { "Artist 1", "Artist 2" }
};

// M4A : métadonnées compatibles iTunes
var m4aTagsForPodcast = new MediaFileTags
{
    Title = "Episode Title",
    Album = "Podcast Series Name",  // Affiché comme « Album » dans iTunes
    Performers = new[] { "Host Name" }, // Affiché comme « Artist »
    Genres = new[] { "Podcast" },
    Comment = "Episode description"
};
```

## Dépannage

### Problèmes courants et solutions

**Les étiquettes n'apparaissent pas dans les lecteurs multimédias :**

- Assurez-vous que le format de sortie prend en charge les champs d'étiquette spécifiques que vous utilisez
- Vérifiez que le lecteur prend en charge le format d'étiquette (certains préfèrent ID3v2.3 à ID3v2.4)
- Vérifiez que l'encodage du texte est correct (UTF-8 recommandé)

**Taille de fichier inattendue :**

- Réduisez la résolution de la pochette d'album (600×600 suffit généralement)
- Évitez les champs texte extrêmement longs dans les commentaires ou les paroles
- Utilisez une compression d'image appropriée pour les pochettes

**Erreurs d'encodage :**

- Vérifiez que les caractères spéciaux sont correctement encodés
- Assurez-vous que les chemins de fichiers sont accessibles et inscriptibles
- Vérifiez que les paramètres de l'encodeur sont compatibles avec votre système

### Déboguer l'écriture d'étiquettes

Abonnez-vous à l'événement `OnError` du pipeline pour voir les défaillances d'encodeur/multiplexeur lors de l'écriture des étiquettes. Il n'existe pas de flux « messages d'étiquettes uniquement » — inspectez le fichier produit avec un lecteur d'étiquettes (TagLib, MediaInfo ou le propre `MediaInfoReader` du SDK) pour confirmer ce qui a été écrit.

```csharp
var pipeline = new MediaBlocksPipeline();

pipeline.OnError += (sender, e) =>
{
    Console.WriteLine($"Pipeline error: {e.Message}");
};

// Continuer la configuration du pipeline...
```

## Spécifications des formats d'étiquettes

### Étiquettes ID3 (MP3)

- **ID3v1** : structure basique de 128 octets avec des champs limités
- **ID3v2** : format extensible prenant en charge Unicode, plusieurs valeurs et des trames personnalisées
- **Trames courantes** : TIT2 (Title), TPE1 (Artist), TALB (Album), TDRC (Year), TCON (Genre)

### Vorbis Comments (OGG)

- **Format** : texte UTF-8 au format NOM=VALEUR
- **Champs standard** : TITLE, ARTIST, ALBUM, DATE, GENRE, TRACKNUMBER
- **Flexible** : les noms de champs arbitraires et les valeurs multiples sont autorisés

### Métadonnées MP4 (M4A)

- **Atoms** : métadonnées de style iTunes stockées dans des atoms MP4
- **Atoms courants** : ©nam (Title), ©ART (Artist), ©alb (Album), ©day (Year)
- **Données binaires** : la pochette intégrée va dans l'atom `covr`

### Attributs ASF (WMV/WMA)

- **Structure** : paires clé-valeur dans l'en-tête ASF
- **Attributs standard** : Title, Author, Copyright, Description
- **Étendu** : les attributs personnalisés sont pris en charge

---

Ce guide couvre l'écriture d'étiquettes de métadonnées audio avec le VisioForge Media Blocks SDK. La classe unifiée `MediaFileTags` simplifie le code tout en conservant le format d'étiquette natif de chaque conteneur (ID3, Vorbis Comments, atoms MP4, ASF). Pour des scénarios de traitement audio plus avancés, explorez la [documentation complète du VisioForge Media Blocks SDK](../index.md).
