---
title: Video Fingerprinting Database Guide
description: Complete guide for integrating video fingerprints with MongoDB, SQL Server, PostgreSQL with schemas, indexing, and optimization.
---

# Video Fingerprinting Database Guide

## SDK APIs for Storage

**The Video Fingerprinting SDK provides these classes for fingerprint storage:**

- **VFPFingerPrint** - Individual fingerprint with built-in serialization:
  - `Save(string filename)` or `Save(Stream stream)` - Saves to file/stream
  - `Load(string filename)` or `Load(Stream stream)` - Loads from file/stream
  - `Load(byte[] data)` - Loads from byte array
  - Properties: `Data`, `Duration`, `ID`, `OriginalFilename`, `Tag`

- **VFPFingerPrintDB** - Local database of fingerprints:
  - `Items` property - List<VFPFingerPrint> for all fingerprints
  - `Save(string filename)` - Saves database to JSON file
  - `Load(string filename)` - Loads database from JSON file
  - `GetDuplicates()` - Finds duplicate fingerprints

- **MongoDB Integration** (Optional NuGet: VisioForge.DotNet.VideoFingerprinting.MongoDB):
  - `VideoFingerprintDB` class for MongoDB/GridFS storage
  - Full CRUD operations with MongoDB

**Note:** The SDK doesn't include interfaces like IFingerprintStorage or IVideoEngine. Use the concrete classes above or implement your own storage layer using VFPFingerPrint's serialization methods.

## Overview

This guide covers integration of video fingerprints with various database systems, including storage strategies, query optimization, indexing, and scaling considerations for production deployments.

## Table of Contents

- [Database Design Principles](#database-design-principles)
- [MongoDB Integration](#mongodb-integration)
- [SQL Server Integration](#sql-server-integration)
- [PostgreSQL Integration](#postgresql-integration)
- [Performance Optimization](#performance-optimization)
- [Scaling Strategies](#scaling-strategies)

## Database Design Principles

### Fingerprint Data Characteristics

- **Binary Data**: Fingerprints are binary data (10KB - 1MB typical)
- **Immutable**: Once generated, fingerprints don't change
- **Metadata Rich**: Associated with video metadata
- **Query Patterns**: Lookup by ID, batch comparisons, similarity searches

### Storage Considerations

1. **Separate Binary Storage**: Store large binary data separately from metadata
2. **Compression**: Use compression for binary data (typically 30-50% reduction)
3. **Indexing**: Index metadata fields, not binary data
4. **Partitioning**: Partition by date or content type for large datasets
5. **Caching**: Cache frequently accessed fingerprints

## MongoDB Integration

### Using the SDK's Built-in Storage Classes

> **IMPORTANT**: The Video Fingerprinting SDK includes built-in classes for fingerprint storage:
>
> - `VFPFingerPrint` - Individual fingerprint with Save/Load methods for file persistence
> - `VFPFingerPrintDB` - Database of fingerprints with Items list and Save/Load methods
> - MongoDB integration available via separate NuGet package

### Official MongoDB NuGet Package

VisioForge provides an official NuGet package for MongoDB integration with the Video Fingerprinting SDK. This package offers a ready-to-use implementation for storing and managing video fingerprints in MongoDB with GridFS support for efficient binary data storage.

#### Package Information

- **Package Name**: `VisioForge.DotNet.VideoFingerprinting.MongoDB`
- **NuGet URL**: [https://www.nuget.org/packages/VisioForge.DotNet.VideoFingerprinting.MongoDB/](https://www.nuget.org/packages/VisioForge.DotNet.VideoFingerprinting.MongoDB/)
- **Note**: This package is **OPTIONAL** - you can implement your own database integration using the examples in this guide

#### Installation

Install the package using one of the following methods:

##### Package Manager Console

```powershell
Install-Package VisioForge.DotNet.VideoFingerprinting.MongoDB
```

##### .NET CLI

```bash
dotnet add package VisioForge.DotNet.VideoFingerprinting.MongoDB
```

##### Package Reference

Add to your `.csproj` file:

```xml
<PackageReference Include="VisioForge.DotNet.VideoFingerprinting.MongoDB" Version="*" />
```

#### VideoFingerprintDB Class Implementation

The package includes a complete `VideoFingerprintDB` class that provides all necessary functionality for managing fingerprints in MongoDB. Here's the full source code:

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
    /// Video Fingerprint DB.
    /// </summary>
    public class VideoFingerprintDB
    {
        /// <summary>
        /// Mongo client.
        /// </summary>
        private readonly MongoClient _mongoClient;

        /// <summary>
        /// Mongo DB.
        /// </summary>
        private readonly IMongoDatabase _mongoDB;

        /// <summary>
        /// GridFS bucket.
        /// </summary>
        private readonly GridFSBucket _mongoBucket;

        /// <summary>
        /// Gets the fingerprints.
        /// </summary>
        /// <remarks>
        /// This matches the SDK's VFPFingerPrintDB.Items property
        /// </remarks>
        public List<VFPFingerPrint> Items { get; } = new List<VFPFingerPrint>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoFingerprintDB"/> class.
        /// </summary>
        public VideoFingerprintDB(string dbname)
        {
            var mongoSettings = new MongoClientSettings();
            _mongoClient = new MongoClient(mongoSettings);

            _mongoDB = _mongoClient.GetDatabase(dbname);
            _mongoBucket = new GridFSBucket(_mongoDB);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoFingerprintDB"/> class.
        /// </summary>
        public VideoFingerprintDB(string dbname, string connectionString)
        {
            _mongoClient = new MongoClient(connectionString);

            _mongoDB = _mongoClient.GetDatabase(dbname);
            _mongoBucket = new GridFSBucket(_mongoDB);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoFingerprintDB"/> class.
        /// </summary>
        public VideoFingerprintDB(string dbname, MongoClientSettings settings)
        {
            _mongoClient = new MongoClient(settings);

            _mongoDB = _mongoClient.GetDatabase(dbname);
            _mongoBucket = new GridFSBucket(_mongoDB);
        }

        /// <summary>
        /// Loads fingerprints from DB.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
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
        /// Loads DB from folder.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
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
        /// Searches fingerprints in folder.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
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
        /// Gets max ad duration.
        /// </summary>
        /// <returns>
        /// The <see cref="long"/>.
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
        /// Uploads fingerprint.
        /// </summary>
        /// <param name="fingerprint">
        /// The fingerprint.
        /// </param>
        public void Upload(VFPFingerPrint fingerprint)
        {
            _mongoBucket.UploadFromBytes(fingerprint.ID.ToString(), fingerprint.Save());
        }

        /// <summary>
        /// DB removes item by id.
        /// </summary>
        /// <param name="id">
        /// The id.
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
        /// Remove all items from DB.
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
        /// Removes item by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="fromDB">
        /// The from db.
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
        /// Removes item by id.
        /// </summary>
        /// <param name="name">
        /// The id.
        /// </param>
        /// <param name="fromDB">
        /// The from db.
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

#### Usage Example

Here's how to use the `VideoFingerprintDB` class from the NuGet package:

```csharp
using VisioForge.VideoFingerPrinting.MongoDB;
using VisioForge.Core.VideoFingerPrinting;

// Initialize database with default connection (localhost)
var db = new VideoFingerprintDB("video_fingerprints");

// Or with custom connection string
var db = new VideoFingerprintDB("video_fingerprints", 
    "mongodb://username:password@host:27017");

// Or with custom MongoClientSettings
var settings = new MongoClientSettings
{
    Server = new MongoServerAddress("host", 27017),
    Credential = MongoCredential.CreateCredential("admin", "username", "password")
};
var db = new VideoFingerprintDB("video_fingerprints", settings);

// Load existing fingerprints from database
bool success = db.LoadFromDB();

// Upload a new fingerprint
var fingerprint = VFPFingerPrint.Load("video.vsigx");
db.Upload(fingerprint);

// Load fingerprints from a folder and add to collection
db.LoadFromFolder(@"C:\Fingerprints");

// Access loaded fingerprints
foreach (var fp in db.Items)
{
    Console.WriteLine($"ID: {fp.ID}, File: {fp.OriginalFilename}");
}

// Remove fingerprint by ID
db.RemoveByID(fingerprint.ID.ToString());

// Remove fingerprint by name
db.RemoveByName("video.mp4");

// Clear all fingerprints from database
db.RemoveAll();

// Get maximum duration among all fingerprints
long maxDuration = db.MaxAdDuration();
```

The `VideoFingerprintDB` class uses MongoDB GridFS for efficient storage of binary fingerprint data, which is ideal for handling the large binary blobs that video fingerprints represent. The class maintains an in-memory collection of fingerprints in the `Items` property for fast access after loading from the database.

### Schema Design

```javascript
// Fingerprint Collection Schema
{
  "_id": ObjectId("..."),
  "fingerprint_id": UUID("550e8400-e29b-41d4-a716-446655440000"),
  "original_filename": "video.mp4",
  "file_path": "/storage/videos/video.mp4",
  "file_hash": "sha256:abc123...",
  "fingerprint_data": BinData(0, "..."), // Binary fingerprint data
  "fingerprint_type": "compare", // or "search"
  "metadata": {
    "duration": 120000, // milliseconds
    "original_duration": 180000,
    "width": 1920,
    "height": 1080,
    "frame_rate": 29.97,
    "codec": "h264",
    "bitrate": 5000000
  },
  "processing": {
    "generated_at": ISODate("2024-01-15T10:30:00Z"),
    "processing_time": 45.2, // seconds
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

// Comparison Results Collection
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
  "comparison_time": 0.045 // seconds
}

// Search Results Collection
{
  "_id": ObjectId("..."),
  "search_id": UUID("..."),
  "fragment_id": UUID("..."),
  "target_id": UUID("..."),
  "matches": [
    {
      "timestamp": 930, // seconds
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
  "search_time": 1.234 // seconds
}
```

### Example Implementation with MongoDB Driver

> **Note**: The following `MongoDBFingerprintRepository` class is an **EXAMPLE IMPLEMENTATION** showing how you might create your own MongoDB integration if you choose not to use the official NuGet package.

```csharp
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using VisioForge.Core.VideoFingerPrinting;

// EXAMPLE IMPLEMENTATION - Not part of the SDK
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
        
        // Use GridFS for large binary data
        _gridFS = new GridFSBucket(_database, new GridFSBucketOptions
        {
            BucketName = "fingerprint_data",
            ChunkSizeBytes = 1048576 // 1MB chunks
        });
        
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        // Create indexes for optimal query performance
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
        // Compress fingerprint data
        byte[] compressedData = CompressData(fingerprint.Data);
        
        // Store large binary data in GridFS
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
        
        // Retrieve binary data from GridFS
        var bytes = await _gridFS.DownloadAsBytesAsync(document.GridFsId);
        var decompressedData = DecompressData(bytes);
        
        // Update access statistics
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
        // Get comparison history
        var comparisons = await GetComparisonHistoryAsync(fingerprintId, 1000);
        
        // Filter by threshold
        var duplicateIds = comparisons
            .Where(c => c.DifferenceScore < threshold)
            .Select(c => c.Fingerprint1Id == fingerprintId 
                ? c.Fingerprint2Id 
                : c.Fingerprint1Id)
            .Distinct();
        
        // Get fingerprint documents
        var filter = Builders<FingerprintDocument>.Filter.In(
            f => f.FingerprintId, duplicateIds);
        
        return await _fingerprints.Find(filter).ToListAsync();
    }

    // Aggregation pipeline for analytics
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
        // Extract tags from filename or implement custom logic
        var tags = new List<string>();
        var name = Path.GetFileNameWithoutExtension(filename).ToLower();
        
        if (name.Contains("commercial")) tags.Add("commercial");
        if (name.Contains("intro")) tags.Add("intro");
        if (name.Contains("outro")) tags.Add("outro");
        
        return tags;
    }
}

// Document models
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

### MongoDB Queries and Operations

```javascript
// Find all fingerprints for videos longer than 1 hour
db.fingerprints.find({
    "metadata.duration": { $gt: 3600000 }
})

// Find duplicates with high similarity
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

// Get most compared fingerprints
db.fingerprints.find().sort({ "statistics.comparison_count": -1 }).limit(10)

// Find commercials by tag and duration
db.fingerprints.find({
    tags: "commercial",
    "metadata.duration": { $gte: 25000, $lte: 35000 }
})

// Update tags for multiple fingerprints
db.fingerprints.updateMany(
    { "metadata.duration": { $lte: 60000 } },
    { $addToSet: { tags: "short-form" } }
)

// Clean up old comparison results
db.comparisons.deleteMany({
    compared_at: { $lt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000) }
})
```

## SQL Server Integration

### Database Schema

```sql
-- Main fingerprints table
CREATE TABLE Fingerprints (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FingerprintId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    OriginalFilename NVARCHAR(500) NOT NULL,
    FilePath NVARCHAR(1000),
    FileHash VARCHAR(64),
    FingerprintData VARBINARY(MAX), -- Compressed binary data
    FingerprintType VARCHAR(20) NOT NULL CHECK (FingerprintType IN ('compare', 'search')),
    Duration BIGINT NOT NULL, -- milliseconds
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

-- Metadata table
CREATE TABLE FingerprintMetadata (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FingerprintId UNIQUEIDENTIFIER NOT NULL,
    MetadataKey NVARCHAR(100) NOT NULL,
    MetadataValue NVARCHAR(MAX),
    FOREIGN KEY (FingerprintId) REFERENCES Fingerprints(FingerprintId) ON DELETE CASCADE
);

-- Tags table (many-to-many relationship)
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

-- Comparison results table
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

-- Search results table
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

-- Search match locations
CREATE TABLE SearchMatches (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SearchResultId UNIQUEIDENTIFIER NOT NULL,
    TimestampSeconds INT NOT NULL,
    TimestampFormatted VARCHAR(20),
    Difference INT NOT NULL,
    Confidence FLOAT,
    FOREIGN KEY (SearchResultId) REFERENCES SearchResults(Id) ON DELETE CASCADE
);

-- Usage statistics table
CREATE TABLE UsageStatistics (
    FingerprintId UNIQUEIDENTIFIER PRIMARY KEY,
    ComparisonCount INT NOT NULL DEFAULT 0,
    MatchCount INT NOT NULL DEFAULT 0,
    SearchCount INT NOT NULL DEFAULT 0,
    LastAccessed DATETIME2,
    FOREIGN KEY (FingerprintId) REFERENCES Fingerprints(FingerprintId) ON DELETE CASCADE
);

-- Create indexes for performance
CREATE INDEX IX_Fingerprints_OriginalFilename ON Fingerprints(OriginalFilename);
CREATE INDEX IX_Fingerprints_FileHash ON Fingerprints(FileHash);
CREATE INDEX IX_Fingerprints_GeneratedAt ON Fingerprints(GeneratedAt DESC);
CREATE INDEX IX_Fingerprints_Duration ON Fingerprints(Duration);
CREATE INDEX IX_ComparisonResults_Fingerprints ON ComparisonResults(Fingerprint1Id, Fingerprint2Id);
CREATE INDEX IX_ComparisonResults_ComparedAt ON ComparisonResults(ComparedAt DESC);
CREATE INDEX IX_SearchResults_Fingerprints ON SearchResults(FragmentId, TargetId);
CREATE INDEX IX_FingerprintTags_TagId ON FingerprintTags(TagId);

-- Full-text search on filenames
CREATE FULLTEXT CATALOG FingerprintCatalog;
CREATE FULLTEXT INDEX ON Fingerprints(OriginalFilename) 
    KEY INDEX PK__Fingerprints__[YourPrimaryKeyName] 
    ON FingerprintCatalog;
```

### Example Implementation with Dapper

> **Note**: The following `SqlServerFingerprintRepository` class is an **EXAMPLE IMPLEMENTATION** showing how you might integrate with SQL Server. This is not part of the SDK.

```csharp
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO.Compression;
using VisioForge.Core.VideoFingerPrinting;

// EXAMPLE IMPLEMENTATION - Create your own based on your needs
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
            // Compress fingerprint data
            var compressedData = CompressData(fingerprint.Data);
            
            // Insert fingerprint
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
            
            // Insert usage statistics
            await connection.ExecuteAsync(
                "INSERT INTO UsageStatistics (FingerprintId) VALUES (@FingerprintId)",
                new { FingerprintId = fingerprintId },
                transaction);
            
            // Extract and insert tags
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

    // Stored procedure for batch comparison
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
        // Insert tag if not exists
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

### SQL Server Stored Procedures

```sql
-- Stored procedure for batch fingerprint comparison
CREATE PROCEDURE sp_BatchCompareFingerprints
    @ReferenceFingerprintId UNIQUEIDENTIFIER,
    @Limit INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get reference fingerprint
    DECLARE @RefData VARBINARY(MAX);
    SELECT @RefData = FingerprintData 
    FROM Fingerprints 
    WHERE FingerprintId = @ReferenceFingerprintId;
    
    IF @RefData IS NULL
    BEGIN
        RAISERROR('Reference fingerprint not found', 16, 1);
        RETURN;
    END
    
    -- Return top candidates for comparison
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

-- Stored procedure to clean old data
CREATE PROCEDURE sp_CleanupOldData
    @DaysToKeep INT = 90
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CutoffDate DATETIME2 = DATEADD(DAY, -@DaysToKeep, GETUTCDATE());
    
    -- Delete old comparison results
    DELETE FROM ComparisonResults WHERE ComparedAt < @CutoffDate;
    
    -- Delete old search results
    DELETE FROM SearchResults WHERE SearchedAt < @CutoffDate;
    
    -- Delete orphaned fingerprints (no recent access)
    DELETE FROM Fingerprints 
    WHERE FingerprintId IN (
        SELECT f.FingerprintId
        FROM Fingerprints f
        LEFT JOIN UsageStatistics us ON f.FingerprintId = us.FingerprintId
        WHERE us.LastAccessed < @CutoffDate OR us.LastAccessed IS NULL
    );
    
    -- Return cleanup statistics
    SELECT 
        @@ROWCOUNT AS DeletedRecords,
        @CutoffDate AS CutoffDate;
END
GO

-- Function to calculate storage statistics
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

## PostgreSQL Integration

### Database Schema

```sql
-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- For similarity searches

-- Main fingerprints table
CREATE TABLE fingerprints (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    fingerprint_id UUID NOT NULL UNIQUE,
    original_filename VARCHAR(500) NOT NULL,
    file_path VARCHAR(1000),
    file_hash VARCHAR(64),
    fingerprint_data BYTEA, -- Compressed binary data
    fingerprint_type VARCHAR(20) NOT NULL CHECK (fingerprint_type IN ('compare', 'search')),
    duration BIGINT NOT NULL, -- milliseconds
    original_duration BIGINT NOT NULL,
    width INTEGER NOT NULL,
    height INTEGER NOT NULL,
    frame_rate DOUBLE PRECISION NOT NULL,
    generated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    processing_time DOUBLE PRECISION,
    sdk_version VARCHAR(20),
    metadata JSONB, -- Flexible metadata storage
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Tags table with GIN index for fast lookups
CREATE TABLE tags (
    id SERIAL PRIMARY KEY,
    tag_name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE fingerprint_tags (
    fingerprint_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id) ON DELETE CASCADE,
    tag_id INTEGER NOT NULL REFERENCES tags(id) ON DELETE CASCADE,
    PRIMARY KEY (fingerprint_id, tag_id)
);

-- Comparison results with partitioning by date
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

-- Create monthly partitions for comparison results
CREATE TABLE comparison_results_2024_01 PARTITION OF comparison_results
    FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
CREATE TABLE comparison_results_2024_02 PARTITION OF comparison_results
    FOR VALUES FROM ('2024-02-01') TO ('2024-03-01');
-- Continue for other months...

-- Search results table
CREATE TABLE search_results (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    fragment_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id),
    target_id UUID NOT NULL REFERENCES fingerprints(fingerprint_id),
    matches JSONB NOT NULL, -- Array of match locations
    searched_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    search_time DOUBLE PRECISION
);

-- Usage statistics with automatic updates
CREATE TABLE usage_statistics (
    fingerprint_id UUID PRIMARY KEY REFERENCES fingerprints(fingerprint_id) ON DELETE CASCADE,
    comparison_count INTEGER NOT NULL DEFAULT 0,
    match_count INTEGER NOT NULL DEFAULT 0,
    search_count INTEGER NOT NULL DEFAULT 0,
    last_accessed TIMESTAMP WITH TIME ZONE
);

-- Create indexes
CREATE INDEX idx_fingerprints_filename ON fingerprints(original_filename);
CREATE INDEX idx_fingerprints_filehash ON fingerprints(file_hash);
CREATE INDEX idx_fingerprints_generated ON fingerprints(generated_at DESC);
CREATE INDEX idx_fingerprints_duration ON fingerprints(duration);
CREATE INDEX idx_fingerprints_metadata ON fingerprints USING GIN (metadata);
CREATE INDEX idx_comparison_fps ON comparison_results(fingerprint1_id, fingerprint2_id);
CREATE INDEX idx_search_fps ON search_results(fragment_id, target_id);
CREATE INDEX idx_search_matches ON search_results USING GIN (matches);

-- Full-text search index
CREATE INDEX idx_fingerprints_fulltext ON fingerprints 
    USING GIN (to_tsvector('english', original_filename));

-- Trigram index for similarity searches
CREATE INDEX idx_fingerprints_trigram ON fingerprints 
    USING GIN (original_filename gin_trgm_ops);

-- Function to update timestamps
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

-- Function to update usage statistics
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

### Example Implementation with Npgsql

> **Note**: The following `PostgreSqlFingerprintRepository` class is an **EXAMPLE IMPLEMENTATION** showing how you might integrate with PostgreSQL. This is not part of the SDK.

```csharp
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
using VisioForge.Core.VideoFingerPrinting;

// EXAMPLE IMPLEMENTATION - Adapt to your specific requirements
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
            
            // Insert fingerprint with JSONB metadata
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
            
            // Insert usage statistics
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
        
        // Use PostgreSQL's trigram similarity
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
        
        // Query JSONB metadata
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

    // Batch operations using COPY
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

    // Helper methods (compression, hashing, etc.) same as previous examples
    private byte[] CompressData(byte[] data) { /* ... */ }
    private byte[] DecompressData(byte[] data) { /* ... */ }
    private Task<string> ComputeFileHashAsync(string path) { /* ... */ }
    private List<string> ExtractTags(string filename) { /* ... */ }
}
```

## Performance Optimization

### Indexing Strategies

```sql
-- MongoDB Compound Indexes
db.fingerprints.createIndex({
    "metadata.duration": 1,
    "processing.generated_at": -1
})

-- SQL Server Filtered Index
CREATE INDEX IX_Recent_Commercials 
ON Fingerprints(Duration, GeneratedAt DESC)
WHERE FingerprintType = 'search' 
  AND Duration BETWEEN 25000 AND 35000

-- PostgreSQL Partial Index
CREATE INDEX idx_short_videos 
ON fingerprints(duration, generated_at DESC)
WHERE duration < 60000;

-- PostgreSQL BRIN Index for time-series data
CREATE INDEX idx_fingerprints_generated_brin 
ON fingerprints USING BRIN (generated_at);
```

### Query Optimization

```csharp
// EXAMPLE IMPLEMENTATION - Performance optimization patterns
public class OptimizedFingerprintRepository
{
    // Use connection pooling
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
    
    // Batch processing with pagination
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
    
    // Parallel comparison with chunking
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
    
    // Caching layer with MemoryCache
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

### Database Maintenance

```sql
-- SQL Server: Update statistics and rebuild indexes
UPDATE STATISTICS Fingerprints WITH FULLSCAN;
ALTER INDEX ALL ON Fingerprints REBUILD WITH (ONLINE = ON);

-- PostgreSQL: Vacuum and analyze
VACUUM ANALYZE fingerprints;
REINDEX TABLE fingerprints;

-- PostgreSQL: Automatic partitioning maintenance
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

-- Schedule monthly execution
SELECT cron.schedule('create-partitions', '0 0 1 * *', 'SELECT create_monthly_partition()');
```

## Scaling Strategies

### Horizontal Scaling with Sharding

```csharp
// EXAMPLE IMPLEMENTATION - Sharding pattern for horizontal scaling
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
        // Use consistent hashing for shard selection
        var hash = fingerprintId.GetHashCode();
        return Math.Abs(hash) % _shardCount;
    }
    
    public async Task SaveFingerprintAsync(VFPFingerPrint fingerprint)
    {
        var shardId = GetShardId(fingerprint.ID);
        var connectionString = _shardConnections[shardId];
        
        // Save to appropriate shard
        using var connection = new SqlConnection(connectionString);
        // ... save logic
    }
    
    public async Task<VFPFingerPrint> GetFingerprintAsync(Guid id)
    {
        var shardId = GetShardId(id);
        var connectionString = _shardConnections[shardId];
        
        // Retrieve from appropriate shard
        using var connection = new SqlConnection(connectionString);
        // ... retrieval logic
    }
    
    // Parallel query across all shards
    public async Task<List<VFPFingerPrint>> SearchAllShardsAsync(string criteria)
    {
        var tasks = _shardConnections.Select(async shard =>
        {
            using var connection = new SqlConnection(shard.Value);
            // ... search logic
            return await SearchShardAsync(connection, criteria);
        });
        
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r).ToList();
    }
}
```

### Read Replicas for Scaling

```csharp
// EXAMPLE IMPLEMENTATION - Read replica pattern for scaling
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
    
    // Write operations go to primary
    public async Task SaveFingerprintAsync(VFPFingerPrint fingerprint)
    {
        using var connection = new SqlConnection(_primaryConnection);
        // ... save logic
    }
    
    // Read operations use round-robin on replicas
    public async Task<VFPFingerPrint> GetFingerprintAsync(Guid id)
    {
        var connectionString = GetNextReplicaConnection();
        using var connection = new SqlConnection(connectionString);
        // ... read logic
    }
    
    private string GetNextReplicaConnection()
    {
        var index = Interlocked.Increment(ref _currentReplica) % _replicaConnections.Count;
        return _replicaConnections[index];
    }
}
```

## Best Practices

1. **Binary Data Storage**
   - Use compression (30-50% size reduction)
   - Store large binaries separately (GridFS, FileStream)
   - Consider cloud storage for very large datasets

2. **Indexing**
   - Index metadata, not binary data
   - Use compound indexes for common query patterns
   - Partial/filtered indexes for specific subsets

3. **Caching**
   - Cache frequently accessed fingerprints
   - Use Redis for distributed caching
   - Implement cache invalidation strategies

4. **Partitioning**
   - Partition by date for time-series data
   - Shard by fingerprint ID for horizontal scaling
   - Archive old data to separate storage

5. **Performance**
   - Use connection pooling
   - Implement batch operations
   - Use async/await throughout
   - Monitor query performance
