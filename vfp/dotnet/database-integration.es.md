---
title: Guía de Base de Datos de Huellas Digitales de Video
description: Guía completa para integrar huellas digitales de video con MongoDB, SQL Server, PostgreSQL con esquemas, indexación y optimización.
---

# Guía de Base de Datos de Huellas Digitales de Video

## APIs del SDK para Almacenamiento

**El SDK de Huellas Digitales de Video proporciona estas clases para el almacenamiento de huellas digitales:**

- **VFPFingerPrint** - Huella digital individual con serialización integrada:
  - `Save(string filename)` o `Save(Stream stream)` - Guarda en archivo/stream
  - `Load(string filename)` o `Load(Stream stream)` - Carga desde archivo/stream
  - `Load(byte[] data)` - Carga desde arreglo de bytes
  - Propiedades: `Data`, `Duration`, `ID`, `OriginalFilename`, `Tag`

- **VFPFingerPrintDB** - Base de datos local de huellas digitales:
  - Propiedad `Items` - List<VFPFingerPrint> para todas las huellas digitales
  - `Save(string filename)` - Guarda la base de datos en archivo JSON
  - `Load(string filename)` - Carga la base de datos desde archivo JSON
  - `GetDuplicates()` - Encuentra huellas digitales duplicadas

- **Integración MongoDB** (Paquete NuGet opcional: VisioForge.DotNet.VideoFingerprinting.MongoDB):
  - Clase `VideoFingerprintDB` para almacenamiento MongoDB/GridFS
  - Operaciones CRUD completas con MongoDB

**Nota:** El SDK no incluye interfaces como IFingerprintStorage o IVideoEngine. Use las clases concretas arriba o implemente su propia capa de almacenamiento usando los métodos de serialización de VFPFingerPrint.

## Descripción General

Esta guía cubre la integración de huellas digitales de video con varios sistemas de base de datos, incluyendo estrategias de almacenamiento, optimización de consultas, indexación y consideraciones de escalado para despliegues de producción.

## Tabla de Contenidos

- [Principios de Diseño de Base de Datos](#principios-de-diseno-de-base-de-datos)
- [Integración MongoDB](#integracion-mongodb)
- [Integración SQL Server](#integracion-sql-server)
- [Integración PostgreSQL](#integracion-postgresql)
- [Optimización de Rendimiento](#optimizacion-de-rendimiento)
- [Estrategias de Escalado](#estrategias-de-escalado)

## Principios de Diseño de Base de Datos

### Características de los Datos de Huellas Digitales

- **Datos Binarios**: Las huellas digitales son datos binarios (10KB - 1MB típico)
- **Inmutables**: Una vez generadas, las huellas digitales no cambian
- **Ricos en Metadatos**: Asociados con metadatos de video
- **Patrones de Consulta**: Búsqueda por ID, comparaciones por lotes, búsquedas de similitud

### Consideraciones de Almacenamiento

1. **Almacenamiento Binario Separado**: Almacene datos binarios grandes por separado de metadatos
2. **Compresión**: Use compresión para datos binarios (reducción típica del 30-50%)
3. **Indexación**: Indexe campos de metadatos, no datos binarios
4. **Particionamiento**: Particione por fecha o tipo de contenido para conjuntos de datos grandes
5. **Almacenamiento en Caché**: Almacene en caché huellas digitales accedidas frecuentemente

## Integración MongoDB

### Usando las Clases de Almacenamiento Integradas del SDK

> **IMPORTANTE**: El SDK de Huellas Digitales de Video incluye clases integradas para almacenamiento de huellas digitales:
>
> - `VFPFingerPrint` - Huella digital individual con métodos Save/Load para persistencia de archivos
> - `VFPFingerPrintDB` - Base de datos de huellas digitales con lista Items y métodos Save/Load
> - Integración MongoDB disponible vía paquete NuGet separado

### Paquete NuGet Oficial de MongoDB

VisioForge proporciona un paquete NuGet oficial para integración MongoDB con el SDK de Huellas Digitales de Video. Este paquete ofrece una implementación lista para usar para almacenar y gestionar huellas digitales de video en MongoDB con soporte GridFS para almacenamiento eficiente de datos binarios.

#### Información del Paquete

- **Nombre del Paquete**: `VisioForge.DotNet.VideoFingerprinting.MongoDB`
- **URL de NuGet**: [https://www.nuget.org/packages/VisioForge.DotNet.VideoFingerprinting.MongoDB/](https://www.nuget.org/packages/VisioForge.DotNet.VideoFingerprinting.MongoDB/)
- **Nota**: Este paquete es **OPCIONAL** - puede implementar su propia integración de base de datos usando los ejemplos en esta guía

#### Instalación

Instale el paquete usando uno de los siguientes métodos:

##### Consola del Administrador de Paquetes

```powershell
Install-Package VisioForge.DotNet.VideoFingerprinting.MongoDB
```

##### CLI de .NET

```bash
dotnet add package VisioForge.DotNet.VideoFingerprinting.MongoDB
```

##### Referencia de Paquete

Agregue a su archivo `.csproj`:

```xml
<PackageReference Include="VisioForge.DotNet.VideoFingerprinting.MongoDB" Version="*" />
```

#### Implementación de la Clase VideoFingerprintDB

El paquete incluye una clase completa `VideoFingerprintDB` que proporciona toda la funcionalidad necesaria para gestionar huellas digitales en MongoDB. Aquí está el código fuente completo:

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using VisioForge.Core.VideoFingerPrinting;

namespace VisioForge.VideoFingerPrinting.MongoDB
{
    /// <summary>
    /// Base de datos de huellas digitales de video.
    /// </summary>
    public class VideoFingerprintDB
    {
        /// <summary>
        /// Cliente Mongo.
        /// </summary>
        private readonly MongoClient _mongoClient;

        /// <summary>
        /// Base de datos Mongo.
        /// </summary>
        private readonly IMongoDatabase _mongoDB;

        /// <summary>
        /// Bucket GridFS.
        /// </summary>
        private readonly GridFSBucket _mongoBucket;

        /// <summary>
        /// Obtiene las huellas digitales.
        /// </summary>
        /// <remarks>
        /// Esto coincide con la propiedad Items del SDK VFPFingerPrintDB
        /// </remarks>
        public List<VFPFingerPrint> Items { get; } = new List<VFPFingerPrint>();

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VideoFingerprintDB"/>.
        /// </summary>
        public VideoFingerprintDB(string dbname)
        {
            var mongoSettings = new MongoClientSettings();
            _mongoClient = new MongoClient(mongoSettings);

            _mongoDB = _mongoClient.GetDatabase(dbname);
            _mongoBucket = new GridFSBucket(_mongoDB);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VideoFingerprintDB"/>.
        /// </summary>
        public VideoFingerprintDB(string dbname, string connectionString)
        {
            _mongoClient = new MongoClient(connectionString);

            _mongoDB = _mongoClient.GetDatabase(dbname);
            _mongoBucket = new GridFSBucket(_mongoDB);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VideoFingerprintDB"/>.
        /// </summary>
        public VideoFingerprintDB(string dbname, MongoClientSettings settings)
        {
            _mongoClient = new MongoClient(settings);

            _mongoDB = _mongoClient.GetDatabase(dbname);
            _mongoBucket = new GridFSBucket(_mongoDB);
        }

        /// <summary>
        /// Carga huellas digitales desde la base de datos.
        /// </summary>
        /// <returns>
        /// El <see cref="bool"/>.
        /// </returns>
        public bool LoadFromDB()
        {
            try
            {
                Items.Clear();

                var filter =
                    Builders<GridFSFileInfo>.Filter.And(
                        Builders<GridFSFileInfo>.Filter.Gte(
                            x => x.UploadDateTime,
                            new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

                var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
                var options = new GridFSFindOptions
                {
                    Sort = sort
                };

                using (var cursor = _mongoBucket.Find(filter, options))
                {
                    var files = cursor.ToList();
                    foreach (var fileInfo in files)
                    {
                        var file = _mongoBucket.DownloadAsBytes(fileInfo.Id);
                        VFPFingerPrint vfp = VFPFingerPrint.Load(file);

                        Items.Add(vfp);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Carga la base de datos desde carpeta.
        /// </summary>
        /// <param name="folder">
        /// La carpeta.
        /// </param>
        /// <returns>
        /// El <see cref="bool"/>.
        /// </returns>
        public bool LoadFromFolder(string folder)
        {
            var list = SearchFingerprintsInFolder(folder);
            foreach (var filename in list)
            {
                VFPFingerPrint fgp = VFPFingerPrint.Load(filename);
                Items.Add(fgp);
            }

            return false;
        }

        /// <summary>
        /// Busca huellas digitales en carpeta.
        /// </summary>
        /// <param name="folder">
        /// La carpeta.
        /// </param>
        /// <returns>
        /// El <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<string> SearchFingerprintsInFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileList = new DirectoryInfo(folder).GetFiles("*", SearchOption.AllDirectories);
            var fileList2 = fileList.Where(a =>
                a.Extension.ToLowerInvariant() == ".vsigx");

            List<string> files = new List<string>();
            foreach (var fileInfo in fileList2)
            {
                files.Add(fileInfo.FullName);
            }

            return files;
        }

        /// <summary>
        /// Obtiene duración máxima de anuncio.
        /// </summary>
        /// <returns>
        /// El <see cref="long"/>.
        /// </returns>
        public long MaxAdDuration()
        {
            long maxDuration = 0;
            foreach (var fingerprint in Items)
            {
                maxDuration = Math.Max(maxDuration, (long)fingerprint.Duration.TotalSeconds);
            }

            return maxDuration;
        }

        /// <summary>
        /// Sube huella digital.
        /// </summary>
        /// <param name="fingerprint">
        /// La huella digital.
        /// </param>
        public void Upload(VFPFingerPrint fingerprint)
        {
            _mongoBucket.UploadFromBytes(fingerprint.ID.ToString(), fingerprint.Save());
        }

        /// <summary>
        /// Elimina elemento por id de la base de datos.
        /// </summary>
        /// <param name="id">
        /// El id.
        /// </param>
        private void DBRemoveByID(string id)
        {
            id = id.ToLowerInvariant();
            var filter =
                    Builders<GridFSFileInfo>.Filter.And(
                        Builders<GridFSFileInfo>.Filter.Eq(
                            x => x.Filename, id));

            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Sort = sort
            };

            using (var cursor = _mongoBucket.Find(filter, options))
            {
                var files = cursor.ToList();
                foreach (var fileInfo in files)
                {
                    _mongoBucket.Delete(fileInfo.Id);
                    break;
                }
            }
        }

        /// <summary>
        /// Elimina todos los elementos de la base de datos.
        /// </summary>
        public void RemoveAll()
        {
            var filter =
                Builders<GridFSFileInfo>.Filter.And(
                    Builders<GridFSFileInfo>.Filter.Gte(
                        x => x.UploadDateTime,
                        new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Sort = sort
            };

            using (var cursor = _mongoBucket.Find(filter, options))
            {
                var files = cursor.ToList();
                foreach (var fileInfo in files)
                {
                    _mongoBucket.Delete(fileInfo.Id);
                }
            }
        }

        /// <summary>
        /// Elimina elemento por id.
        /// </summary>
        /// <param name="id">
        /// El id.
        /// </param>
        /// <param name="fromDB">
        /// De la base de datos.
        /// </param>
        public void RemoveByID(string id, bool fromDB = true)
        {
            foreach (var fingerprint in Items)
            {
                if (fingerprint.ID.ToString().ToUpperInvariant() == id.ToUpperInvariant())
                {
                    if (fromDB)
                    {
                        DBRemoveByID(id);
                    }

                    Items.Remove(fingerprint);
                    
                    break;
                }
            }
        }

        /// <summary>
        /// Elimina elemento por nombre.
        /// </summary>
        /// <param name="name">
        /// El nombre.
        /// </param>
        /// <param name="fromDB">
        /// De la base de datos.
        /// </param>
        public void RemoveByName(string name, bool fromDB = true)
        {
            foreach (var fingerprint in Items)
            {
                if (fingerprint.OriginalFilename.ToString().ToUpperInvariant() == name.ToUpperInvariant())
                {
                    if (fromDB)
                    {
                        DBRemoveByID(fingerprint.ID.ToString());
                    }

                    Items.Remove(fingerprint);

                    break;
                }
            }
        }
    }
}
```

#### Ejemplo de Uso

Aquí está cómo usar la clase `VideoFingerprintDB` del paquete NuGet:

```csharp
using VisioForge.VideoFingerPrinting.MongoDB;
using VisioForge.Core.VideoFingerPrinting;

// Inicializar base de datos con conexión predeterminada (localhost)
var db = new VideoFingerprintDB("video_fingerprints");

// O con cadena de conexión personalizada
var db = new VideoFingerprintDB("video_fingerprints", 
    "mongodb://username:password@host:27017");

// O con MongoClientSettings personalizados
var settings = new MongoClientSettings
{
    Server = new MongoServerAddress("host", 27017),
    Credential = MongoCredential.CreateCredential("admin", "username", "password")
};
var db = new VideoFingerprintDB("video_fingerprints", settings);

// Cargar huellas digitales existentes desde la base de datos
bool success = db.LoadFromDB();

// Subir una nueva huella digital
var fingerprint = VFPFingerPrint.Load("video.vsigx");
db.Upload(fingerprint);

// Cargar huellas digitales desde una carpeta y agregar a la colección
db.LoadFromFolder(@"C:\Fingerprints");

// Acceder a huellas digitales cargadas
foreach (var fp in db.Items)
{
    Console.WriteLine($"ID: {fp.ID}, Archivo: {fp.OriginalFilename}");
}

// Eliminar huella digital por ID
db.RemoveByID(fingerprint.ID.ToString());

// Eliminar huella digital por nombre
db.RemoveByName("video.mp4");

// Limpiar todas las huellas digitales de la base de datos
db.RemoveAll();

// Obtener duración máxima entre todas las huellas digitales
long maxDuration = db.MaxAdDuration();
```

La clase `VideoFingerprintDB` usa MongoDB GridFS para almacenamiento eficiente de datos binarios de huellas digitales, lo cual es ideal para manejar los blobs binarios grandes que representan las huellas digitales de video. La clase mantiene una colección en memoria de huellas digitales en la propiedad `Items` para acceso rápido después de cargar desde la base de datos.

### Diseño de Esquema

```javascript
// Esquema de Colección de Huellas Digitales
{
  "_id": ObjectId("..."),
  "fingerprint_id": UUID("550e8400-e29b-41d4-a716-446655440000"),
  "original_filename": "video.mp4",
  "file_path": "/storage/videos/video.mp4",
  "file_hash": "sha256:abc123...",
  "fingerprint_data": BinData(0, "..."), // Datos binarios de huella digital
  "fingerprint_type": "compare", // o "search"
  "metadata": {
    "duration": 120000, // milisegundos
    "original_duration": 180000,
    "width": 1920,
    "height": 1080,
    "frame_rate": 29.97,
    "codec": "h264",
    "bitrate": 5000000
  },
  "processing": {
    "generated_at": ISODate("2024-01-15T10:30:00Z"),
    "processing_time": 45.2, // segundos
    "sdk_version": "2024.1.0",
    "parameters": {
      "resolution": "1280x720",
      "ignored_areas": [
        {"left": 1700, "top": 50, "right": 1870, "bottom": 150}
      ]
    }
  },
  "tags": ["commercial", "30-second", "automotive"],
  "collections": ["2024-campaigns", "ford"],
  "statistics": {
    "comparison_count": 156,
    "match_count": 12,
    "last_accessed": ISODate("2024-01-20T15:45:00Z")
  }
}

// Colección de Resultados de Comparación
{
  "_id": ObjectId("..."),
  "comparison_id": UUID("..."),
  "fingerprint1_id": UUID("..."),
  "fingerprint2_id": UUID("..."),
  "difference_score": 12,
  "similarity_percentage": 98.5,
  "is_match": true,
  "threshold_used": 20,
  "compared_at": ISODate("2024-01-15T11:00:00Z"),
  "comparison_time": 0.045 // segundos
}

// Colección de Resultados de Búsqueda
{
  "_id": ObjectId("..."),
  "search_id": UUID("..."),
  "fragment_id": UUID("..."),
  "target_id": UUID("..."),
  "matches": [
    {
      "timestamp": 930, // segundos
      "timestamp_formatted": "00:15:30",
      "difference": 8,
      "confidence": 0.95
    },
    {
      "timestamp": 2722,
      "timestamp_formatted": "00:45:22",
      "difference": 12,
      "confidence": 0.92
    }
  ],
  "searched_at": ISODate("2024-01-15T12:00:00Z"),
  "search_time": 1.234 // segundos
}
```

### Implementación de Ejemplo con Controlador MongoDB

> **Nota**: La siguiente clase `MongoDBFingerprintRepository` es una **IMPLEMENTACIÓN DE EJEMPLO** que muestra cómo podría crear su propia integración MongoDB si elige no usar el paquete NuGet oficial.

```csharp
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using VisioForge.Core.VideoFingerPrinting;

// IMPLEMENTACIÓN DE EJEMPLO - No es parte del SDK
public class MongoDBFingerprintRepository
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<FingerprintDocument> _fingerprints;
    private readonly IMongoCollection<ComparisonResult> _comparisons;
    private readonly IMongoCollection<SearchResult> _searches;
    private readonly IGridFSBucket _gridFS;

    public MongoDBFingerprintRepository(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
        
        _fingerprints = _database.GetCollection<FingerprintDocument>("fingerprints");
        _comparisons = _database.GetCollection<ComparisonResult>("comparisons");
        _searches = _database.GetCollection<SearchResult>("searches");
        
        // Usar GridFS para datos binarios grandes
        _gridFS = new GridFSBucket(_database, new GridFSBucketOptions
        {
            BucketName = "fingerprint_data",
            ChunkSizeBytes = 1048576 // Chunks de 1MB
        });
        
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        // Crear índices para rendimiento óptimo de consultas
        _fingerprints.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<FingerprintDocument>(
                Builders<FingerprintDocument>.IndexKeys.Ascending(f => f.FingerprintId)),
            new CreateIndexModel<FingerprintDocument>(
                Builders<FingerprintDocument>.IndexKeys.Ascending(f => f.OriginalFilename)),
            new CreateIndexModel<FingerprintDocument>(
                Builders<FingerprintDocument>.IndexKeys.Ascending(f => f.FileHash)),
            new CreateIndexModel<FingerprintDocument>(
                Builders<FingerprintDocument>.IndexKeys.Descending(f => f.Processing.GeneratedAt)),
            new CreateIndexModel<FingerprintDocument>(
                Builders<FingerprintDocument>.IndexKeys.Ascending("tags")),
            new CreateIndexModel<FingerprintDocument>(
                Builders<FingerprintDocument>.IndexKeys.Text(f => f.OriginalFilename))
        });

        _comparisons.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<ComparisonResult>(
                Builders<ComparisonResult>.IndexKeys
                    .Ascending(c => c.Fingerprint1Id)
                    .Ascending(c => c.Fingerprint2Id)),
            new CreateIndexModel<ComparisonResult>(
                Builders<ComparisonResult>.IndexKeys.Descending(c => c.ComparedAt))
        });

        _searches.Indexes.CreateOne(
            new CreateIndexModel<SearchResult>(
                Builders<SearchResult>.IndexKeys
                    .Ascending(s => s.FragmentId)
                    .Ascending(s => s.TargetId)));
    }

    public async Task<string> SaveFingerprintAsync(VFPFingerPrint fingerprint, string filePath)
    {
        // Comprimir datos de huella digital
        byte[] compressedData = CompressData(fingerprint.Data);
        
        // Almacenar datos binarios grandes en GridFS
        var gridFsId = await _gridFS.UploadFromBytesAsync(
            $"{fingerprint.ID}.vsigx",
            compressedData);
        
        var document = new FingerprintDocument
        {
            FingerprintId = fingerprint.ID,
            OriginalFilename = fingerprint.OriginalFilename,
            FilePath = filePath,
            FileHash = ComputeFileHash(filePath),
            GridFsId = gridFsId,
            FingerprintType = "compare",
            Metadata = new VideoMetadata
            {
                Duration = (long)fingerprint.Duration.TotalMilliseconds,
                OriginalDuration = (long)fingerprint.OriginalDuration.TotalMilliseconds,
                Width = fingerprint.Width,
                Height = fingerprint.Height,
                FrameRate = fingerprint.FrameRate
            },
            Processing = new ProcessingInfo
            {
                GeneratedAt = DateTime.UtcNow,
                SdkVersion = "2024.1.0"
            },
            Tags = ExtractTags(fingerprint.OriginalFilename),
            Statistics = new UsageStatistics
            {
                LastAccessed = DateTime.UtcNow
            }
        };
        
        await _fingerprints.InsertOneAsync(document);
        return document.Id.ToString();
    }

    public async Task<VFPFingerPrint> GetFingerprintAsync(Guid fingerprintId)
    {
        var filter = Builders<FingerprintDocument>.Filter.Eq(f => f.FingerprintId, fingerprintId);
        var document = await _fingerprints.Find(filter).FirstOrDefaultAsync();
        
        if (document == null)
            return null;
        
        // Recuperar datos binarios desde GridFS
        var bytes = await _gridFS.DownloadAsBytesAsync(document.GridFsId);
        var decompressedData = DecompressData(bytes);
        
        // Actualizar estadísticas de acceso
        var update = Builders<FingerprintDocument>.Update
            .Inc(f => f.Statistics.ComparisonCount, 1)
            .Set(f => f.Statistics.LastAccessed, DateTime.UtcNow);
        
        await _fingerprints.UpdateOneAsync(filter, update);
        
        return new VFPFingerPrint
        {
            ID = document.FingerprintId,
            Data = decompressedData,
            OriginalFilename = document.OriginalFilename,
            Duration = TimeSpan.FromMilliseconds(document.Metadata.Duration),
            OriginalDuration = TimeSpan.FromMilliseconds(document.Metadata.OriginalDuration),
            Width = document.Metadata.Width,
            Height = document.Metadata.Height,
            FrameRate = document.Metadata.FrameRate
        };
    }

    public async Task<List<FingerprintDocument>> SearchByTagsAsync(params string[] tags)
    {
        var filter = Builders<FingerprintDocument>.Filter.All("tags", tags);
        return await _fingerprints.Find(filter).ToListAsync();
    }

    public async Task<List<FingerprintDocument>> GetRecentFingerprintsAsync(int limit = 100)
    {
        return await _fingerprints
            .Find(FilterDefinition<FingerprintDocument>.Empty)
            .SortByDescending(f => f.Processing.GeneratedAt)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task SaveComparisonResultAsync(
        Guid fp1Id, Guid fp2Id, int difference, double similarity)
    {
        var result = new ComparisonResult
        {
            ComparisonId = Guid.NewGuid(),
            Fingerprint1Id = fp1Id,
            Fingerprint2Id = fp2Id,
            DifferenceScore = difference,
            SimilarityPercentage = similarity,
            IsMatch = difference < 20,
            ThresholdUsed = 20,
            ComparedAt = DateTime.UtcNow
        };
        
        await _comparisons.InsertOneAsync(result);
    }

    public async Task<List<ComparisonResult>> GetComparisonHistoryAsync(
        Guid fingerprintId, int limit = 50)
    {
        var filter = Builders<ComparisonResult>.Filter.Or(
            Builders<ComparisonResult>.Filter.Eq(c => c.Fingerprint1Id, fingerprintId),
            Builders<ComparisonResult>.Filter.Eq(c => c.Fingerprint2Id, fingerprintId)
        );
        
        return await _comparisons
            .Find(filter)
            .SortByDescending(c => c.ComparedAt)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<FingerprintDocument>> FindDuplicatesAsync(
        Guid fingerprintId, int threshold = 20)
    {
        // Obtener historial de comparación
        var comparisons = await GetComparisonHistoryAsync(fingerprintId, 1000);
        
        // Filtrar por umbral
        var duplicateIds = comparisons
            .Where(c => c.DifferenceScore < threshold)
            .Select(c => c.Fingerprint1Id == fingerprintId 
                ? c.Fingerprint2Id 
                : c.Fingerprint1Id)
            .Distinct();
        
        // Obtener documentos de huella digital
        var filter = Builders<FingerprintDocument>.Filter.In(
            f => f.FingerprintId, duplicateIds);
        
        return await _fingerprints.Find(filter).ToListAsync();
    }

    // Pipeline de agregación para análisis
    public async Task<object> GetStatisticsAsync()
    {
        var pipeline = new[]
        {
            new BsonDocument("$group", new BsonDocument
            {
                {"_id", BsonNull.Value},
                {"totalFingerprints", new BsonDocument("$sum", 1)},
                {"totalSize", new BsonDocument("$sum", "$metadata.duration")},
                {"avgDuration", new BsonDocument("$avg", "$metadata.duration")},
                {"avgWidth", new BsonDocument("$avg", "$metadata.width")},
                {"avgHeight", new BsonDocument("$avg", "$metadata.height")}
            }),
            new BsonDocument("$project", new BsonDocument
            {
                {"totalFingerprints", 1},
                {"totalSizeHours", new BsonDocument("$divide", 
                    new BsonArray { "$totalSize", 3600000 })},
                {"avgDurationSeconds", new BsonDocument("$divide", 
                    new BsonArray { "$avgDuration", 1000 })},
                {"avgWidth", 1},
                {"avgHeight", 1}
            })
        };
        
        var result = await _fingerprints.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
        return result?.ToJson();
    }

    private byte[] CompressData(byte[] data)
    {
        using (var output = new MemoryStream())
        {
            using (var gzip = new System.IO.Compression.GZipStream(
                output, System.IO.Compression.CompressionLevel.Optimal))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }

    private byte[] DecompressData(byte[] compressedData)
    {
        using (var input = new MemoryStream(compressedData))
        using (var output = new MemoryStream())
        {
            using (var gzip = new System.IO.Compression.GZipStream(
                input, System.IO.Compression.CompressionMode.Decompress))
            {
                gzip.CopyTo(output);
            }
            return output.ToArray();
        }
    }

    private string ComputeFileHash(string filePath)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        using (var stream = File.OpenRead(filePath))
        {
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    private List<string> ExtractTags(string filename)
    {
        // Extraer etiquetas del nombre de archivo o implementar lógica personalizada
        var tags = new List<string>();
        var name = Path.GetFileNameWithoutExtension(filename).ToLower();
        
        if (name.Contains("commercial")) tags.Add("commercial");
        if (name.Contains("intro")) tags.Add("intro");
        if (name.Contains("outro")) tags.Add("outro");
        
        return tags;
    }
}

// Modelos de documento
public class FingerprintDocument
{
    public ObjectId Id { get; set; }
    public Guid FingerprintId { get; set; }
    public string OriginalFilename { get; set; }
    public string FilePath { get; set; }
    public string FileHash { get; set; }
    public ObjectId GridFsId { get; set; }
    public string FingerprintType { get; set; }
    public VideoMetadata Metadata { get; set; }
    public ProcessingInfo Processing { get; set; }
    public List<string> Tags { get; set; }
    public List<string> Collections { get; set; }
    public UsageStatistics Statistics { get; set; }
}

public class VideoMetadata
{
    public long Duration { get; set; }
    public long OriginalDuration { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double FrameRate { get; set; }
    public string Codec { get; set; }
    public long Bitrate { get; set; }
}

public class ProcessingInfo
{
    public DateTime GeneratedAt { get; set; }
    public double ProcessingTime { get; set; }
    public string SdkVersion { get; set; }
    public ProcessingParameters Parameters { get; set; }
}

public class ProcessingParameters
{
    public string Resolution { get; set; }
    public List<IgnoredArea> IgnoredAreas { get; set; }
}

public class IgnoredArea
{
    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
}

public class UsageStatistics
{
    public int ComparisonCount { get; set; }
    public int MatchCount { get; set; }
    public DateTime LastAccessed { get; set; }
}

public class ComparisonResult
{
    public ObjectId Id { get; set; }
    public Guid ComparisonId { get; set; }
    public Guid Fingerprint1Id { get; set; }
    public Guid Fingerprint2Id { get; set; }
    public int DifferenceScore { get; set; }
    public double SimilarityPercentage { get; set; }
    public bool IsMatch { get; set; }
    public int ThresholdUsed { get; set; }
    public DateTime ComparedAt { get; set; }
    public double ComparisonTime { get; set; }
}

public class SearchResult
{
    public ObjectId Id { get; set; }
    public Guid SearchId { get; set; }
    public Guid FragmentId { get; set; }
    public Guid TargetId { get; set; }
    public List<MatchLocation> Matches { get; set; }
    public DateTime SearchedAt { get; set; }
    public double SearchTime { get; set; }
}

public class MatchLocation
{
    public int Timestamp { get; set; }
    public string TimestampFormatted { get; set; }
    public int Difference { get; set; }
    public double Confidence { get; set; }
}
```

### Consultas y Operaciones MongoDB

```javascript
// Encontrar todas las huellas digitales para videos más largos que 1 hora
db.fingerprints.find({
    "metadata.duration": { $gt: 3600000 }
})

// Encontrar duplicados con alta similitud
db.comparisons.aggregate([
    { $match: { similarity_percentage: { $gte: 95 } } },
    { $group: {
        _id: null,
        duplicates: { $push: {
            fp1: "$fingerprint1_id",
            fp2: "$fingerprint2_id",
            similarity: "$similarity_percentage"
        }}
    }}
])

// Obtener huellas digitales más comparadas
db.fingerprints.find().sort({ "statistics.comparison_count": -1 }).limit(10)

// Encontrar comerciales por etiqueta y duración
db.fingerprints.find({
    tags: "commercial",
    "metadata.duration": { $gte: 25000, $lte: 35000 }
})

// Actualizar etiquetas para múltiples huellas digitales
db.fingerprints.updateMany(
    { "metadata.duration": { $lte: 60000 } },
    { $addToSet: { tags: "short-form" } }
)

// Limpiar resultados de comparación antiguos
db.comparisons.deleteMany({
    compared_at: { $lt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000) }
})
```

## Integración SQL Server

### Esquema de Base de Datos

```sql
-- Tabla principal de huellas digitales
CREATE TABLE Fingerprints (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FingerprintId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    OriginalFilename NVARCHAR(500) NOT NULL,
    FilePath NVARCHAR(1000),
    FileHash VARCHAR(64),
    FingerprintData VARBINARY(MAX), -- Datos binarios comprimidos
    FingerprintType VARCHAR(20) NOT NULL CHECK (FingerprintType IN ('compare', 'search')),
    Duration BIGINT NOT NULL, -- milisegundos
    OriginalDuration BIGINT NOT NULL,
    Width INT NOT NULL,
    Height INT NOT NULL,
    FrameRate FLOAT NOT NULL,
    GeneratedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ProcessingTime FLOAT,
    SdkVersion VARCHAR(20),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Tabla de metadatos
CREATE TABLE FingerprintMetadata (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FingerprintId UNIQUEIDENTIFIER NOT NULL,
    MetadataKey NVARCHAR(100) NOT NULL,
    MetadataValue NVARCHAR(MAX),
    FOREIGN KEY (FingerprintId) REFERENCES Fingerprints(FingerprintId) ON DELETE CASCADE
);

-- Tabla de etiquetas (relación muchos-a-muchos)
CREATE TABLE Tags (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TagName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE FingerprintTags (
    FingerprintId UNIQUEIDENTIFIER NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (FingerprintId, TagId),
    FOREIGN KEY (FingerprintId) REFERENCES Fingerprints(FingerprintId) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
);

-- Tabla de resultados de comparación
CREATE TABLE ComparisonResults (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Fingerprint1Id UNIQUEIDENTIFIER NOT NULL,
    Fingerprint2Id UNIQUEIDENTIFIER NOT NULL,
    DifferenceScore INT NOT NULL,
    SimilarityPercentage FLOAT NOT NULL,
    IsMatch BIT NOT NULL,
    ThresholdUsed INT NOT NULL,
    ComparedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ComparisonTime FLOAT,
    FOREIGN KEY (Fingerprint1Id) REFERENCES Fingerprints(FingerprintId),
    FOREIGN KEY (Fingerprint2Id) REFERENCES Fingerprints(FingerprintId)
);

-- Tabla de resultados de búsqueda
CREATE TABLE SearchResults (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FragmentId UNIQUEIDENTIFIER NOT NULL,
    TargetId UNIQUEIDENTIFIER NOT NULL,
    SearchedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    SearchTime FLOAT,
    MatchCount INT NOT NULL,
    FOREIGN KEY (FragmentId) REFERENCES Fingerprints(FingerprintId),
    FOREIGN KEY (TargetId) REFERENCES Fingerprints(FingerprintId)
);

-- Ubicaciones de coincidencias de búsqueda
CREATE TABLE SearchMatches (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SearchResultId UNIQUEIDENTIFIER NOT NULL,
    TimestampSeconds INT NOT NULL,
    TimestampFormatted VARCHAR(20),
    Difference INT NOT NULL,
    Confidence FLOAT,
    FOREIGN KEY (SearchResultId) REFERENCES SearchResults(Id) ON DELETE CASCADE
);

-- Tabla de estadísticas de uso
CREATE TABLE UsageStatistics (
    FingerprintId UNIQUEIDENTIFIER PRIMARY KEY,
    ComparisonCount INT NOT NULL DEFAULT 0,
    MatchCount INT NOT NULL DEFAULT 0,
    SearchCount INT NOT NULL DEFAULT 0,
    LastAccessed DATETIME2,
    FOREIGN KEY (FingerprintId) REFERENCES Fingerprints(FingerprintId) ON DELETE CASCADE
);

-- Crear índices para rendimiento
CREATE INDEX IX_Fingerprints_OriginalFilename ON Fingerprints(OriginalFilename);
CREATE INDEX IX_Fingerprints_FileHash ON Fingerprints(FileHash);
CREATE INDEX IX_Fingerprints_GeneratedAt ON Fingerprints(GeneratedAt DESC);
CREATE INDEX IX_Fingerprints_Duration ON Fingerprints(Duration);
CREATE INDEX IX_ComparisonResults_Fingerprints ON ComparisonResults(Fingerprint1Id, Fingerprint2Id);
CREATE INDEX IX_ComparisonResults_ComparedAt ON ComparisonResults(ComparedAt DESC);
CREATE INDEX IX_SearchResults_Fingerprints ON SearchResults(FragmentId, TargetId);
CREATE INDEX IX_FingerprintTags_TagId ON FingerprintTags(TagId);

-- Búsqueda de texto completo en nombres de archivo
CREATE FULLTEXT CATALOG FingerprintCatalog;
CREATE FULLTEXT INDEX ON Fingerprints(OriginalFilename) 
    KEY INDEX PK__Fingerprints__[YourPrimaryKeyName] 
    ON FingerprintCatalog;
```

### Implementación de Ejemplo con Dapper

> **Nota**: La siguiente clase `SqlServerFingerprintRepository` es una **IMPLEMENTACIÓN DE EJEMPLO** que muestra cómo podría integrar con SQL Server. Esta no es parte del SDK.

```csharp
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO.Compression;
using VisioForge.Core.VideoFingerPrinting;

// IMPLEMENTACIÓN DE EJEMPLO - Cree su propia basada en sus necesidades
public class SqlServerFingerprintRepository
{
    private readonly string _connectionString;

    public SqlServerFingerprintRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Guid> SaveFingerprintAsync(VFPFingerPrint fingerprint, string filePath)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var transaction = connection.BeginTransaction();
        try
        {
            // Comprimir datos de huella digital
            var compressedData = CompressData(fingerprint.Data);
            
            // Insertar huella digital
            var fingerprintId = Guid.NewGuid();
            const string insertSql = @"
                INSERT INTO Fingerprints 
                (FingerprintId, OriginalFilename, FilePath, FileHash, FingerprintData, 
                 FingerprintType, Duration, OriginalDuration, Width, Height, FrameRate)
                VALUES 
                (@FingerprintId, @OriginalFilename, @FilePath, @FileHash, @FingerprintData,
                 @FingerprintType, @Duration, @OriginalDuration, @Width, @Height, @FrameRate)";
            
            await connection.ExecuteAsync(insertSql, new
            {
                FingerprintId = fingerprintId,
                OriginalFilename = fingerprint.OriginalFilename,
                FilePath = filePath,
                FileHash = await ComputeFileHashAsync(filePath),
                FingerprintData = compressedData,
                FingerprintType = "compare",
                Duration = (long)fingerprint.Duration.TotalMilliseconds,
                OriginalDuration = (long)fingerprint.OriginalDuration.TotalMilliseconds,
                Width = fingerprint.Width,
                Height = fingerprint.Height,
                FrameRate = fingerprint.FrameRate
            }, transaction);
            
            // Insertar estadísticas de uso
            await connection.ExecuteAsync(
                "INSERT INTO UsageStatistics (FingerprintId) VALUES (@FingerprintId)",
                new { FingerprintId = fingerprintId },
                transaction);
            
            // Extraer e insertar etiquetas
            var tags = ExtractTags(fingerprint.OriginalFilename);
            foreach (var tag in tags)
            {
                await InsertTagAsync(connection, transaction, fingerprintId, tag);
            }
            
            await transaction.CommitAsync();
            return fingerprintId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<VFPFingerPrint> GetFingerprintAsync(Guid fingerprintId)
    {
        using var connection = new SqlConnection(_connectionString);
        
        const string query = @"
            SELECT * FROM Fingerprints WHERE FingerprintId = @FingerprintId;
            
            UPDATE UsageStatistics 
            SET ComparisonCount = ComparisonCount + 1,
                LastAccessed = GETUTCDATE()
            WHERE FingerprintId = @FingerprintId";
        
        using var multi = await connection.QueryMultipleAsync(query, 
            new { FingerprintId = fingerprintId });
        
        var data = await multi.ReadSingleOrDefaultAsync<dynamic>();
        if (data == null)
            return null;
        
        var decompressedData = DecompressData(data.FingerprintData);
        
        return new VFPFingerPrint
        {
            ID = data.FingerprintId,
            Data = decompressedData,
            OriginalFilename = data.OriginalFilename,
            Duration = TimeSpan.FromMilliseconds(data.Duration),
            OriginalDuration = TimeSpan.FromMilliseconds(data.OriginalDuration),
            Width = data.Width,
            Height = data.Height,
            FrameRate = data.FrameRate
        };
    }

    public async Task<IEnumerable<dynamic>> SearchByTagsAsync(params string[] tags)
    {
        using var connection = new SqlConnection(_connectionString);
        
        const string query = @"
            SELECT DISTINCT f.* 
            FROM Fingerprints f
            INNER JOIN FingerprintTags ft ON f.FingerprintId = ft.FingerprintId
            INNER JOIN Tags t ON ft.TagId = t.Id
            WHERE t.TagName IN @Tags";
        
        return await connection.QueryAsync<dynamic>(query, new { Tags = tags });
    }

    public async Task SaveComparisonResultAsync(
        Guid fp1Id, Guid fp2Id, int difference, double similarity)
    {
        using var connection = new SqlConnection(_connectionString);
        
        const string sql = @"
            INSERT INTO ComparisonResults 
            (Fingerprint1Id, Fingerprint2Id, DifferenceScore, SimilarityPercentage, 
             IsMatch, ThresholdUsed, ComparisonTime)
            VALUES 
            (@Fp1Id, @Fp2Id, @Difference, @Similarity, @IsMatch, @Threshold, @Time)";
        
        await connection.ExecuteAsync(sql, new
        {
            Fp1Id = fp1Id,
            Fp2Id = fp2Id,
            Difference = difference,
            Similarity = similarity,
            IsMatch = difference < 20,
            Threshold = 20,
            Time = 0.045
        });
    }

    public async Task<IEnumerable<dynamic>> FindDuplicatesAsync(
        Guid fingerprintId, int threshold = 20)
    {
        using var connection = new SqlConnection(_connectionString);
        
        const string query = @"
            WITH Matches AS (
                SELECT 
                    CASE 
                        WHEN Fingerprint1Id = @FingerprintId THEN Fingerprint2Id
                        ELSE Fingerprint1Id
                    END AS MatchedId,
                    DifferenceScore,
                    SimilarityPercentage
                FROM ComparisonResults
                WHERE (Fingerprint1Id = @FingerprintId OR Fingerprint2Id = @FingerprintId)
                    AND DifferenceScore < @Threshold
            )
            SELECT f.*, m.DifferenceScore, m.SimilarityPercentage
            FROM Fingerprints f
            INNER JOIN Matches m ON f.FingerprintId = m.MatchedId
            ORDER BY m.DifferenceScore";
        
        return await connection.QueryAsync<dynamic>(query, 
            new { FingerprintId = fingerprintId, Threshold = threshold });
    }

    // Procedimiento almacenado para comparación por lotes
    public async Task<IEnumerable<dynamic>> BatchCompareAsync(
        Guid referenceFingerprintId, int limit = 100)
    {
        using var connection = new SqlConnection(_connectionString);
        
        return await connection.QueryAsync<dynamic>(
            "sp_BatchCompareFingerprints",
            new { ReferenceFingerprintId = referenceFingerprintId, Limit = limit },
            commandType: CommandType.StoredProcedure);
    }

    private async Task InsertTagAsync(
        SqlConnection connection, 
        IDbTransaction transaction,
        Guid fingerprintId, 
        string tagName)
    {
        // Insertar etiqueta si no existe
        const string insertTag = @"
            IF NOT EXISTS (SELECT 1 FROM Tags WHERE TagName = @TagName)
                INSERT INTO Tags (TagName) VALUES (@TagName);
            
            DECLARE @TagId INT = (SELECT Id FROM Tags WHERE TagName = @TagName);
            
            INSERT INTO FingerprintTags (FingerprintId, TagId)
            VALUES (@FingerprintId, @TagId)";
        
        await connection.ExecuteAsync(insertTag, 
            new { FingerprintId = fingerprintId, TagName = tagName },
            transaction);
    }

    private byte[] CompressData(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
        {
            gzip.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    private byte[] DecompressData(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        {
            gzip.CopyTo(output);
        }
        return output.ToArray();
    }

    private async Task<string> ComputeFileHashAsync(string filePath)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await Task.Run(() => sha256.ComputeHash(stream));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private List<string> ExtractTags(string filename)
    {
        var tags = new List<string>();
        var name = Path.GetFileNameWithoutExtension(filename).ToLower();
        
        if (name.Contains("commercial")) tags.Add("commercial");
        if (name.Contains("intro")) tags.Add("intro");
        if (name.Contains("outro")) tags.Add("outro");
        
        return tags;
    }
}
```

### Procedimientos Almacenados SQL Server

```sql
-- Procedimiento almacenado para comparación por lotes de huellas digitales
CREATE PROCEDURE sp_BatchCompareFingerprints
    @ReferenceFingerprintId UNIQUEIDENTIFIER,
    @Limit INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Obtener huella digital de referencia
    DECLARE @RefData VARBINARY(MAX);
    SELECT @RefData = FingerprintData 
    FROM Fingerprints 
    WHERE FingerprintId = @ReferenceFingerprintId;
    
    IF @RefData IS NULL
    BEGIN
        RAISERROR('Huella digital de referencia no encontrada', 16, 1);
        RETURN;
    END
    
    -- Devolver candidatos principales para comparación
    SELECT TOP (@Limit)
        FingerprintId,
        OriginalFilename,
        Duration,
        Width,
        Height
    FROM Fingerprints
    WHERE FingerprintId != @ReferenceFingerprintId
        AND ABS(Duration - (SELECT Duration FROM Fingerprints WHERE FingerprintId = @ReferenceFingerprintId)) < 10000
    ORDER BY GeneratedAt DESC;
END
GO

-- Procedimiento almacenado para limpiar datos antiguos
CREATE PROCEDURE sp_CleanupOldData
    @DaysToKeep INT = 90
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CutoffDate DATETIME2 = DATEADD(DAY, -@DaysToKeep, GETUTCDATE());
    
    -- Eliminar resultados de comparación antiguos
    DELETE FROM ComparisonResults WHERE ComparedAt < @CutoffDate;
    
    -- Eliminar resultados de búsqueda antiguos
    DELETE FROM SearchResults WHERE SearchedAt < @CutoffDate;
    
    -- Eliminar huellas digitales huérfanas (sin acceso reciente)
    DELETE FROM Fingerprints 
    WHERE FingerprintId IN (
        SELECT f.FingerprintId
        FROM Fingerprints f
        LEFT JOIN UsageStatistics us ON f.FingerprintId = us.FingerprintId
        WHERE us.LastAccessed < @CutoffDate OR us.LastAccessed IS NULL
    );
    
    -- Devolver estadísticas de limpieza
    SELECT 
        @@ROWCOUNT AS DeletedRecords,
        @CutoffDate AS CutoffDate;
END
GO

-- Función para calcular estadísticas de almacenamiento
CREATE FUNCTION fn_GetStorageStatistics()
RETURNS TABLE
AS
RETURN
(
    SELECT 
        COUNT(*) AS TotalFingerprints,
        SUM(DATALENGTH(FingerprintData)) / 1048576.0 AS TotalSizeMB,
        AVG(DATALENGTH(FingerprintData)) / 1024.0 AS AvgSizeKB,
        SUM(Duration) / 3600000.0 AS TotalHours,
        AVG(Duration) / 1000.0 AS AvgDurationSeconds
    FROM Fingerprints
);
GO
```

## Integración PostgreSQL

### Esquema de Base de Datos

```sql
-- Habilitar extensiones requeridas
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- Para búsquedas de similitud

-- Tabla principal de huellas digitales
CREATE TABLE fingerprints (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    fingerprint_id UUID NOT NULL UNIQUE,
    original_filename VARCHAR(500) NOT NULL,
    file_path VARCHAR(1000),
    file_hash VARCHAR(64),
    fingerprint_data BYTEA, -- Datos binarios comprimidos
    fingerprint_type VARCHAR(20) NOT NULL CHECK (fingerprint_type IN ('compare', 'search')),
    duration BIGINT NOT NULL, -- milisegundos
    original_duration BIGINT NOT NULL,
    width INTEGER NOT NULL,
    height INTEGER NOT NULL,
    frame_rate DOUBLE PRECISION NOT NULL,
    generated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    processing_time DOUBLE PRECISION,
    sdk_version VARCHAR(20),
    metadata JSONB, -- Almacenamiento de metadatos flexible
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tabla de etiquetas con índice GIN para búsquedas rápidas
CREATE TABLE tags (
    id SERIAL PRIMARY KEY,
    tag_name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE fingerprint_tags (
    fingerprint_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id) ON DELETE CASCADE,
    tag_id INTEGER NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
    PRIMARY KEY (fingerprint_id, tag_id)
);

-- Tabla de resultados de comparación con particionamiento por fecha
CREATE TABLE comparison_results (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    fingerprint1_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id),
    fingerprint2_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id),
    difference_score INTEGER NOT NULL,
    similarity_percentage DOUBLE PRECISION NOT NULL,
    is_match BOOLEAN NOT NULL,
    threshold_used INTEGER NOT NULL,
    compared_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    comparison_time DOUBLE PRECISION
) PARTITION BY RANGE (compared_at);

-- Crear particiones mensuales para resultados de comparación
CREATE TABLE comparison_results_2024_01 PARTITION OF comparison_results
    FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
CREATE TABLE comparison_results_2024_02 PARTITION OF comparison_results
    FOR VALUES FROM ('2024-02-01') TO ('2024-03-01');
-- Continuar para otros meses...

-- Tabla de resultados de búsqueda
CREATE TABLE search_results (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    fragment_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id),
    target_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id),
    matches JSONB NOT NULL, -- Matriz de ubicaciones de coincidencias
    searched_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    search_time DOUBLE PRECISION
);

-- Tabla de estadísticas de uso con actualizaciones automáticas
CREATE TABLE usage_statistics (
    fingerprint_id UUID PRIMARY KEY REFERENCES fingerprints(fingerprint_id) ON DELETE CASCADE,
    comparison_count INTEGER NOT NULL DEFAULT 0,
    match_count INTEGER NOT NULL DEFAULT 0,
    search_count INTEGER NOT NULL DEFAULT 0,
    last_accessed TIMESTAMP WITH TIME ZONE
);

-- Crear índices
CREATE INDEX idx_fingerprints_filename ON fingerprints(original_filename);
CREATE INDEX idx_fingerprints_filehash ON fingerprints(file_hash);
CREATE INDEX idx_fingerprints_generated ON fingerprints(generated_at DESC);
CREATE INDEX idx_fingerprints_duration ON fingerprints(duration);
CREATE INDEX idx_fingerprints_metadata ON fingerprints USING GIN (metadata);
CREATE INDEX idx_comparison_fps ON comparison_results(fingerprint1_id, fingerprint2_id);
CREATE INDEX idx_search_fps ON search_results(fragment_id, target_id);
CREATE INDEX idx_search_matches ON search_results USING GIN (matches);

-- Índice de búsqueda de texto completo
CREATE INDEX idx_fingerprints_fulltext ON fingerprints 
    USING GIN (to_tsvector('english', original_filename));

-- Índice trigram para búsquedas de similitud
CREATE INDEX idx_fingerprints_trigram ON fingerprints 
    USING GIN (original_filename gin_trgm_ops);

-- Función para actualizar timestamps
CREATE OR REPLACE FUNCTION update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_fingerprints_updated_at
    BEFORE UPDATE ON fingerprints
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at();

-- Función para actualizar estadísticas de uso
CREATE OR REPLACE FUNCTION update_usage_stats()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO usage_statistics (fingerprint_id, comparison_count)
    VALUES (NEW.fingerprint_id, 1)
    ON CONFLICT (fingerprint_id)
    DO UPDATE SET 
        comparison_count = usage_statistics.comparison_count + 1,
        last_accessed = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
```

### Implementación de Ejemplo con Npgsql

> **Nota**: La siguiente clase `PostgreSqlFingerprintRepository` es una **IMPLEMENTACIÓN DE EJEMPLO** que muestra cómo podría integrar con PostgreSQL. Esta no es parte del SDK.

```csharp
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
using VisioForge.Core.VideoFingerPrinting;

// IMPLEMENTACIÓN DE EJEMPLO - Adapte a sus requisitos específicos
public class PostgreSqlFingerprintRepository
{
    private readonly string _connectionString;

    public PostgreSqlFingerprintRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Guid> SaveFingerprintAsync(VFPFingerPrint fingerprint, string filePath)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var fingerprintId = Guid.NewGuid();
            var compressedData = CompressData(fingerprint.Data);
            
            // Insertar huella digital con metadatos JSONB
            const string sql = @"
                INSERT INTO fingerprints 
                (fingerprint_id, original_filename, file_path, file_hash, 
                 fingerprint_data, fingerprint_type, duration, original_duration,
                 width, height, frame_rate, metadata)
                VALUES 
                (@fingerprint_id, @original_filename, @file_path, @file_hash,
                 @fingerprint_data, @fingerprint_type, @duration, @original_duration,
                 @width, @height, @frame_rate, @metadata::jsonb)";
            
            var metadata = new
            {
                codec = "h264",
                bitrate = 5000000,
                tags = ExtractTags(fingerprint.OriginalFilename),
                ignoredAreas = fingerprint.IgnoredAreas?.Select(r => new
                {
                    left = r.Left,
                    top = r.Top,
                    right = r.Right,
                    bottom = r.Bottom
                })
            };
            
            await using (var cmd = new NpgsqlCommand(sql, connection, transaction))
            {
                cmd.Parameters.AddWithValue("fingerprint_id", fingerprintId);
                cmd.Parameters.AddWithValue("original_filename", fingerprint.OriginalFilename);
                cmd.Parameters.AddWithValue("file_path", filePath ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("file_hash", 
                    await ComputeFileHashAsync(filePath) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("fingerprint_data", compressedData);
                cmd.Parameters.AddWithValue("fingerprint_type", "compare");
                cmd.Parameters.AddWithValue("duration", 
                    (long)fingerprint.Duration.TotalMilliseconds);
                cmd.Parameters.AddWithValue("original_duration", 
                    (long)fingerprint.OriginalDuration.TotalMilliseconds);
                cmd.Parameters.AddWithValue("width", fingerprint.Width);
                cmd.Parameters.AddWithValue("height", fingerprint.Height);
                cmd.Parameters.AddWithValue("frame_rate", fingerprint.FrameRate);
                cmd.Parameters.AddWithValue("metadata", 
                    JsonSerializer.Serialize(metadata));
                
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Insertar estadísticas de uso
            await using (var cmd = new NpgsqlCommand(
                "INSERT INTO usage_statistics (fingerprint_id) VALUES (@id)", 
                connection, transaction))
            {
                cmd.Parameters.AddWithValue("id", fingerprintId);
                await cmd.ExecuteNonQueryAsync();
            }
            
            await transaction.CommitAsync();
            return fingerprintId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<VFPFingerPrint>> SearchSimilarAsync(
        string filename, double threshold = 0.3)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // Usar similitud trigram de PostgreSQL
        const string sql = @"
            SELECT *, similarity(original_filename, @filename) AS sim
            FROM fingerprints
            WHERE original_filename % @filename
            ORDER BY sim DESC
            LIMIT 10";
        
        var results = new List<VFPFingerPrint>();
        
        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("filename", filename);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (reader.GetDouble(reader.GetOrdinal("sim")) >= threshold)
            {
                var compressedData = reader["fingerprint_data"] as byte[];
                var decompressedData = DecompressData(compressedData);
                
                results.Add(new VFPFingerPrint
                {
                    ID = reader.GetGuid(reader.GetOrdinal("fingerprint_id")),
                    Data = decompressedData,
                    OriginalFilename = reader.GetString(reader.GetOrdinal("original_filename")),
                    Duration = TimeSpan.FromMilliseconds(
                        reader.GetInt64(reader.GetOrdinal("duration"))),
                    Width = reader.GetInt32(reader.GetOrdinal("width")),
                    Height = reader.GetInt32(reader.GetOrdinal("height")),
                    FrameRate = reader.GetDouble(reader.GetOrdinal("frame_rate"))
                });
            }
        }
        
        return results;
    }

    public async Task<List<dynamic>> QueryByMetadataAsync(string jsonPath, object value)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // Consultar metadatos JSONB
        const string sql = @"
            SELECT fingerprint_id, original_filename, metadata
            FROM fingerprints
            WHERE metadata @> @filter::jsonb";
        
        var filter = new Dictionary<string, object>();
        SetNestedValue(filter, jsonPath.Split('.'), value);
        
        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("filter", JsonSerializer.Serialize(filter));
        
        var results = new List<dynamic>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new
            {
                FingerprintId = reader.GetGuid(0),
                OriginalFilename = reader.GetString(1),
                Metadata = JsonSerializer.Deserialize<dynamic>(reader.GetString(2))
            });
        }
        
        return results;
    }

    // Operaciones por lotes usando COPY
    public async Task BulkInsertFingerprintsAsync(List<VFPFingerPrint> fingerprints)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var writer = await connection.BeginBinaryImportAsync(
            "COPY fingerprints (fingerprint_id, original_filename, fingerprint_data, " +
            "fingerprint_type, duration, original_duration, width, height, frame_rate) " +
            "FROM STDIN (FORMAT BINARY)");
        
        foreach (var fp in fingerprints)
        {
            await writer.StartRowAsync();
            await writer.WriteAsync(Guid.NewGuid(), NpgsqlDbType.Uuid);
            await writer.WriteAsync(fp.OriginalFilename, NpgsqlDbType.Varchar);
            await writer.WriteAsync(CompressData(fp.Data), NpgsqlDbType.Bytea);
            await writer.WriteAsync("compare", NpgsqlDbType.Varchar);
            await writer.WriteAsync((long)fp.Duration.TotalMilliseconds, NpgsqlDbType.Bigint);
            await writer.WriteAsync((long)fp.OriginalDuration.TotalMilliseconds, NpgsqlDbType.Bigint);
            await writer.WriteAsync(fp.Width, NpgsqlDbType.Integer);
            await writer.WriteAsync(fp.Height, NpgsqlDbType.Integer);
            await writer.WriteAsync(fp.FrameRate, NpgsqlDbType.Double);
        }
        
        await writer.CompleteAsync();
    }

    private void SetNestedValue(Dictionary<string, object> dict, string[] path, object value)
    {
        if (path.Length == 1)
        {
            dict[path[0]] = value;
        }
        else
        {
            if (!dict.ContainsKey(path[0]))
                dict[path[0]] = new Dictionary<string, object>();
            
            SetNestedValue((Dictionary<string, object>)dict[path[0]], 
                path.Skip(1).ToArray(), value);
        }
    }

    // Métodos auxiliares (compresión, hashing, etc.) igual que ejemplos anteriores
    private byte[] CompressData(byte[] data) { /* ... */ }
    private byte[] DecompressData(byte[] data) { /* ... */ }
    private Task<string> ComputeFileHashAsync(string path) { /* ... */ }
    private List<string> ExtractTags(string filename) { /* ... */ }
}
```

## Optimización de Rendimiento

### Estrategias de Indexación

```sql
-- Índices compuestos MongoDB
db.fingerprints.createIndex({
    "metadata.duration": 1,
    "processing.generated_at": -1
})

-- Índice filtrado SQL Server
CREATE INDEX IX_Recent_Commercials 
ON Fingerprints(Duration, GeneratedAt DESC)
WHERE FingerprintType = 'search' 
  AND Duration BETWEEN 25000 AND 35000

-- Índice parcial PostgreSQL
CREATE INDEX idx_short_videos 
ON fingerprints(duration, generated_at DESC)
WHERE duration < 60000;

-- Índice BRIN PostgreSQL para datos de series temporales
CREATE INDEX idx_fingerprints_generated_brin 
ON fingerprints USING BRIN (generated_at);
```

### Optimización de Consultas

```csharp
// IMPLEMENTACIÓN DE EJEMPLO - Patrones de optimización de rendimiento
public class OptimizedFingerprintRepository
{
    // Usar agrupamiento de conexiones
    private readonly SqlConnectionStringBuilder _connectionString;
    
    public OptimizedFingerprintRepository(string connectionString)
    {
        _connectionString = new SqlConnectionStringBuilder(connectionString)
        {
            MinPoolSize = 5,
            MaxPoolSize = 100,
            Pooling = true
        };
    }
    
    // Procesamiento por lotes con paginación
    public async IAsyncEnumerable<VFPFingerPrint> GetFingerprintsStreamAsync(
        int batchSize = 100)
    {
        using var connection = new SqlConnection(_connectionString.ToString());
        await connection.OpenAsync();
        
        int offset = 0;
        bool hasMore = true;
        
        while (hasMore)
        {
            const string query = @"
                SELECT * FROM Fingerprints
                ORDER BY FingerprintId
                OFFSET @Offset ROWS
                FETCH NEXT @BatchSize ROWS ONLY";
            
            var batch = (await connection.QueryAsync<dynamic>(
                query, 
                new { Offset = offset, BatchSize = batchSize })).ToList();
            
            if (batch.Count == 0)
            {
                hasMore = false;
            }
            else
            {
                foreach (var item in batch)
                {
                    yield return MapToFingerprint(item);
                }
                
                offset += batchSize;
            }
        }
    }
    
    // Comparación paralela con fragmentación
    public async Task<List<ComparisonResult>> ParallelCompareAsync(
        List<Guid> fingerprintIds, 
        int maxDegreeOfParallelism = 4)
    {
        var results = new ConcurrentBag<ComparisonResult>();
        var chunks = fingerprintIds.Chunk(100);
        
        await Parallel.ForEachAsync(chunks, 
            new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
            async (chunk, ct) =>
            {
                var chunkResults = await ProcessComparisonChunkAsync(chunk);
                foreach (var result in chunkResults)
                {
                    results.Add(result);
                }
            });
        
        return results.ToList();
    }
    
    // Capa de almacenamiento en caché con MemoryCache
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions
    {
        SizeLimit = 1000
    });
    
    public async Task<VFPFingerPrint> GetFingerprintCachedAsync(Guid id)
    {
        if (_cache.TryGetValue(id, out VFPFingerPrint cached))
        {
            return cached;
        }
        
        var fingerprint = await GetFingerprintFromDatabaseAsync(id);
        
        if (fingerprint != null)
        {
            _cache.Set(id, fingerprint, new MemoryCacheEntryOptions
            {
                Size = 1,
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }
        
        return fingerprint;
    }
}
```

### Mantenimiento de Base de Datos

```sql
-- SQL Server: Actualizar estadísticas y reconstruir índices
UPDATE STATISTICS Fingerprints WITH FULLSCAN;
ALTER INDEX ALL ON Fingerprints REBUILD WITH (ONLINE = ON);

-- PostgreSQL: Vacuum y analizar
VACUUM ANALYZE fingerprints;
REINDEX TABLE fingerprints;

-- PostgreSQL: Mantenimiento automático de particionamiento
CREATE OR REPLACE FUNCTION create_monthly_partition()
RETURNS void AS $$
DECLARE
    start_date date;
    end_date date;
    partition_name text;
BEGIN
    start_date := date_trunc('month', CURRENT_DATE);
    end_date := start_date + interval '1 month';
    partition_name := 'comparison_results_' || to_char(start_date, 'YYYY_MM');
    
    EXECUTE format('CREATE TABLE IF NOT EXISTS %I PARTITION OF comparison_results 
                    FOR VALUES FROM (%L) TO (%L)',
                    partition_name, start_date, end_date);
END;
$$ LANGUAGE plpgsql;

-- Programar ejecución mensual
SELECT cron.schedule('create-partitions', '0 0 1 * *', 'SELECT create_monthly_partition()');
```

## Estrategias de Escalado

### Escalado Horizontal con Fragmentación

```csharp
// IMPLEMENTACIÓN DE EJEMPLO - Patrón de fragmentación para escalado horizontal
public class ShardedFingerprintRepository
{
    private readonly Dictionary<int, string> _shardConnections;
    private readonly int _shardCount;
    
    public ShardedFingerprintRepository(Dictionary<int, string> shardConnections)
    {
        _shardConnections = shardConnections;
        _shardCount = shardConnections.Count;
    }
    
    private int GetShardId(Guid fingerprintId)
    {
        // Usar hash consistente para selección de fragmento
        var hash = fingerprintId.GetHashCode();
        return Math.Abs(hash) % _shardCount;
    }
    
    public async Task SaveFingerprintAsync(VFPFingerPrint fingerprint)
    {
        var shardId = GetShardId(fingerprint.ID);
        var connectionString = _shardConnections[shardId];
        
        // Guardar en fragmento apropiado
        using var connection = new SqlConnection(connectionString);
        // ... lógica de guardado
    }
    
    public async Task<VFPFingerPrint> GetFingerprintAsync(Guid id)
    {
        var shardId = GetShardId(id);
        var connectionString = _shardConnections[shardId];
        
        // Recuperar desde fragmento apropiado
        using var connection = new SqlConnection(connectionString);
        // ... lógica de recuperación
    }
    
    // Consulta paralela en todos los fragmentos
    public async Task<List<VFPFingerPrint>> SearchAllShardsAsync(string criteria)
    {
        var tasks = _shardConnections.Select(async shard =>
        {
            using var connection = new SqlConnection(shard.Value);
            // ... lógica de búsqueda
            return await SearchShardAsync(connection, criteria);
        });
        
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r).ToList();
    }
}
```

### Réplicas de Lectura para Escalado

```csharp
// IMPLEMENTACIÓN DE EJEMPLO - Patrón de réplica de lectura para escalado
public class ReadReplicaFingerprintRepository
{
    private readonly string _primaryConnection;
    private readonly List<string> _replicaConnections;
    private int _currentReplica = 0;
    
    public ReadReplicaFingerprintRepository(
        string primaryConnection, 
        List<string> replicaConnections)
    {
        _primaryConnection = primaryConnection;
        _replicaConnections = replicaConnections;
    }
    
    // Operaciones de escritura van a primaria
    public async Task SaveFingerprintAsync(VFPFingerPrint fingerprint)
    {
        using var connection = new SqlConnection(_primaryConnection);
        // ... lógica de guardado
    }
    
    // Operaciones de lectura usan round-robin en réplicas
    public async Task<VFPFingerPrint> GetFingerprintAsync(Guid id)
    {
        var connectionString = GetNextReplicaConnection();
        using var connection = new SqlConnection(connectionString);
        // ... lógica de lectura
    }
    
    private string GetNextReplicaConnection()
    {
        var index = Interlocked.Increment(ref _currentReplica) % _replicaConnections.Count;
        return _replicaConnections[index];
    }
}
```

## Mejores Prácticas

1. **Almacenamiento de Datos Binarios**
   - Usar compresión (reducción del 30-50% de tamaño)
   - Almacenar binarios grandes por separado (GridFS, FileStream)
   - Considerar almacenamiento en la nube para conjuntos de datos muy grandes

2. **Indexación**
   - Indexar campos de metadatos, no datos binarios
   - Usar índices compuestos para patrones de consulta comunes
   - Índices filtrados/parciales para subconjuntos específicos

3. **Almacenamiento en Caché**
   - Almacenar en caché huellas digitales accedidas frecuentemente
   - Usar Redis para almacenamiento en caché distribuido
   - Implementar estrategias de invalidación de caché

4. **Particionamiento**
   - Particionar por fecha para datos de series temporales
   - Fragmentar por ID de huella digital para escalado horizontal
   - Archivar datos antiguos a almacenamiento separado

5. **Rendimiento**
   - Usar agrupamiento de conexiones
   - Implementar operaciones por lotes
   - Usar async/await en todo
   - Monitorear rendimiento de consultas