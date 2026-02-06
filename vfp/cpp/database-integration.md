---
title: C++ Fingerprinting SDK Database Guide
description: Integrate Video Fingerprinting SDK C++ with databases for storing and retrieving fingerprints including SQLite and PostgreSQL examples.
---

# C++ Fingerprinting SDK Database Guide

This guide demonstrates how to integrate the Video Fingerprinting SDK for C++ with various database systems for efficient fingerprint storage and retrieval.

## Overview

Unlike the .NET SDK which provides built-in MongoDB integration, the C++ SDK offers flexibility to integrate with any database system. This guide provides implementation patterns and examples for common databases.

## Database Design Considerations

### Fingerprint Storage Requirements

- **Binary Data**: Fingerprints are binary data (typically 10-100KB per video)
- **Indexing**: Video metadata should be indexed for fast queries
- **Scalability**: Design should support millions of fingerprints
- **Performance**: Optimize for both write and read operations

### Recommended Schema

```sql
CREATE TABLE video_fingerprints (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    video_path TEXT NOT NULL,
    video_hash VARCHAR(64),
    fingerprint_type VARCHAR(10), -- 'search' or 'compare'
    fingerprint_data BLOB NOT NULL,
    duration_ms INTEGER,
    frame_count INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    metadata JSON,
    INDEX idx_video_path (video_path),
    INDEX idx_video_hash (video_hash),
    INDEX idx_created_at (created_at)
);
```

## SQLite Implementation

SQLite is ideal for embedded applications and single-machine deployments.

### Basic SQLite Integration

```cpp
#include <sqlite3.h>
#include "VisioForge_VFP.h"
#include "VisioForge_VFP_Types.h"
#include <vector>
#include <string>

class SQLiteFingerprintDB {
private:
    sqlite3* db;
    
public:
    SQLiteFingerprintDB(const std::string& dbPath) {
        int rc = sqlite3_open(dbPath.c_str(), &db);
        if (rc != SQLITE_OK) {
            throw std::runtime_error("Cannot open database");
        }
        InitializeSchema();
    }
    
    ~SQLiteFingerprintDB() {
        if (db) {
            sqlite3_close(db);
        }
    }
    
    void InitializeSchema() {
        const char* sql = R"(
            CREATE TABLE IF NOT EXISTS fingerprints (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                video_path TEXT NOT NULL,
                fingerprint_type TEXT NOT NULL,
                fingerprint_data BLOB NOT NULL,
                duration_ms INTEGER,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_video_path ON fingerprints(video_path);
        )";
        
        char* errMsg = nullptr;
        int rc = sqlite3_exec(db, sql, nullptr, nullptr, &errMsg);
        if (rc != SQLITE_OK) {
            std::string error = errMsg;
            sqlite3_free(errMsg);
            throw std::runtime_error("SQL error: " + error);
        }
    }
    
    bool StoreFingerprint(const std::string& videoPath, 
                         const VFPFingerPrint& fingerprint,
                         const std::string& type = "search") {
        const char* sql = R"(
            INSERT INTO fingerprints (video_path, fingerprint_type, fingerprint_data, duration_ms)
            VALUES (?, ?, ?, ?)
        )";
        
        sqlite3_stmt* stmt;
        int rc = sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
        if (rc != SQLITE_OK) return false;
        
        sqlite3_bind_text(stmt, 1, videoPath.c_str(), -1, SQLITE_STATIC);
        sqlite3_bind_text(stmt, 2, type.c_str(), -1, SQLITE_STATIC);
        sqlite3_bind_blob(stmt, 3, fingerprint.Data, fingerprint.DataSize, SQLITE_STATIC);
        sqlite3_bind_int64(stmt, 4, fingerprint.Duration);  // Duration is INT64
        
        rc = sqlite3_step(stmt);
        sqlite3_finalize(stmt);
        
        return rc == SQLITE_DONE;
    }
    
    VFPFingerPrint* LoadFingerprint(const std::string& videoPath) {
        const char* sql = "SELECT fingerprint_data, duration_ms FROM fingerprints WHERE video_path = ?";
        
        sqlite3_stmt* stmt;
        int rc = sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
        if (rc != SQLITE_OK) return nullptr;
        
        sqlite3_bind_text(stmt, 1, videoPath.c_str(), -1, SQLITE_STATIC);
        
        VFPFingerPrint* result = nullptr;
        if (sqlite3_step(stmt) == SQLITE_ROW) {
            result = new VFPFingerPrint();
            
            const void* data = sqlite3_column_blob(stmt, 0);
            int dataSize = sqlite3_column_bytes(stmt, 0);
            
            result->Data = new char[dataSize];  // VFPFingerPrint uses char*
            memcpy(result->Data, data, dataSize);
            result->DataSize = dataSize;
            result->Duration = sqlite3_column_int64(stmt, 1);  // Duration is INT64
        }
        
        sqlite3_finalize(stmt);
        return result;
    }
    
    std::vector<std::string> FindSimilarVideos(const VFPFingerPrint& queryFp, 
                                               int maxResults = 10) {
        std::vector<std::string> results;
        const char* sql = "SELECT video_path, fingerprint_data FROM fingerprints";
        
        sqlite3_stmt* stmt;
        if (sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr) != SQLITE_OK) {
            return results;
        }
        
        std::vector<std::pair<std::string, int>> similarities;
        
        while (sqlite3_step(stmt) == SQLITE_ROW) {
            std::string path = reinterpret_cast<const char*>(sqlite3_column_text(stmt, 0));
            
            const void* data = sqlite3_column_blob(stmt, 1);
            int dataSize = sqlite3_column_bytes(stmt, 1);
            
            // Compare fingerprints using actual SDK function
            double difference = VFPCompare_Compare(
                queryFp.Data, queryFp.DataSize,
                static_cast<const char*>(data), dataSize,
                1000  // maxDifference
            );
            
            if (difference < 1000) {  // Lower difference = more similar
                similarities.push_back({path, static_cast<int>(difference)});
            }
        }
        
        sqlite3_finalize(stmt);
        
        // Sort by difference (lower is better)
        std::sort(similarities.begin(), similarities.end(),
                 [](const auto& a, const auto& b) { return a.second < b.second; });
        
        // Return top results
        for (int i = 0; i < std::min(maxResults, (int)similarities.size()); i++) {
            results.push_back(similarities[i].first);
        }
        
        return results;
    }
};
```

### Usage Example

```cpp
int main() {
    // Initialize database
    SQLiteFingerprintDB db("fingerprints.db");
    
    // Set license
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");
    
    // Generate and store fingerprint
    VFPFingerprintSource source{};
    VFPFillSource(L"video.mp4", &source);
    source.StartTime = 0;
    source.StopTime = 60000;  // First 60 seconds
    
    VFPFingerPrint fingerprint{};
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
    
    if (error == nullptr) {
        db.StoreFingerprint("video.mp4", fingerprint);
        std::cout << "Fingerprint stored successfully" << std::endl;
        
        // Save fingerprint to file as well
        VFPFingerprintSave(&fingerprint, L"video.vfp");
        
        // Find similar videos
        auto similar = db.FindSimilarVideos(fingerprint, 5);
        for (const auto& path : similar) {
            std::cout << "Similar video: " << path << std::endl;
        }
        
        // Note: No VFPFingerPrint_Free function - manage memory manually
        delete[] fingerprint.Data;
    }
    
    return 0;
}
```

## PostgreSQL Implementation

For larger deployments requiring concurrent access and advanced features.

### PostgreSQL Integration with libpq

```cpp
#include <libpq-fe.h>
#include "VisioForge_VFP.h"
#include "VisioForge_VFP_Types.h"

class PostgreSQLFingerprintDB {
private:
    PGconn* conn;
    
public:
    PostgreSQLFingerprintDB(const std::string& connStr) {
        conn = PQconnectdb(connStr.c_str());
        
        if (PQstatus(conn) != CONNECTION_OK) {
            std::string error = PQerrorMessage(conn);
            PQfinish(conn);
            throw std::runtime_error("Connection failed: " + error);
        }
        
        InitializeSchema();
    }
    
    ~PostgreSQLFingerprintDB() {
        if (conn) {
            PQfinish(conn);
        }
    }
    
    void InitializeSchema() {
        const char* sql = R"(
            CREATE TABLE IF NOT EXISTS fingerprints (
                id SERIAL PRIMARY KEY,
                video_path TEXT NOT NULL,
                video_hash VARCHAR(64),
                fingerprint_type VARCHAR(10),
                fingerprint_data BYTEA NOT NULL,
                duration_ms INTEGER,
                metadata JSONB,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );
            CREATE INDEX IF NOT EXISTS idx_video_path ON fingerprints(video_path);
            CREATE INDEX IF NOT EXISTS idx_metadata ON fingerprints USING GIN(metadata);
        )";
        
        PGresult* res = PQexec(conn, sql);
        if (PQresultStatus(res) != PGRES_COMMAND_OK) {
            std::string error = PQresultErrorMessage(res);
            PQclear(res);
            throw std::runtime_error("Schema creation failed: " + error);
        }
        PQclear(res);
    }
    
    bool StoreFingerprint(const std::string& videoPath,
                         const VFPFingerPrint& fingerprint,
                         const std::string& metadata = "{}") {
        // Escape binary data for PostgreSQL
        size_t escapedLen;
        unsigned char* escapedData = PQescapeByteaConn(
            conn, 
            reinterpret_cast<const unsigned char*>(fingerprint.Data), 
            fingerprint.DataSize, 
            &escapedLen
        );
        
        std::string sql = "INSERT INTO fingerprints "
                         "(video_path, fingerprint_data, duration_ms, metadata) "
                         "VALUES ($1, $2, $3, $4::jsonb)";
        
        const char* paramValues[4];
        paramValues[0] = videoPath.c_str();
        paramValues[1] = reinterpret_cast<const char*>(escapedData);
        paramValues[2] = std::to_string(fingerprint.Duration).c_str();
        paramValues[3] = metadata.c_str();
        
        PGresult* res = PQexecParams(conn, sql.c_str(), 4, nullptr,
                                     paramValues, nullptr, nullptr, 0);
        
        bool success = (PQresultStatus(res) == PGRES_COMMAND_OK);
        
        PQclear(res);
        PQfreemem(escapedData);
        
        return success;
    }
    
    // Batch insert for better performance
    bool StoreFingerprintsBatch(const std::vector<std::pair<std::string, VFPFingerPrint>>& batch) {
        PQexec(conn, "BEGIN");
        
        for (const auto& [path, fp] : batch) {
            if (!StoreFingerprint(path, fp)) {
                PQexec(conn, "ROLLBACK");
                return false;
            }
        }
        
        PGresult* res = PQexec(conn, "COMMIT");
        bool success = (PQresultStatus(res) == PGRES_COMMAND_OK);
        PQclear(res);
        
        return success;
    }
};
```

## Redis Implementation

For high-performance caching and real-time processing.

```cpp
#include <hiredis/hiredis.h>
#include "VisioForge_VFP.h"
#include "VisioForge_VFP_Types.h"

class RedisFingerprintCache {
private:
    redisContext* redis;
    
public:
    RedisFingerprintCache(const std::string& host = "127.0.0.1", int port = 6379) {
        redis = redisConnect(host.c_str(), port);
        if (redis == nullptr || redis->err) {
            throw std::runtime_error("Redis connection failed");
        }
    }
    
    ~RedisFingerprintCache() {
        if (redis) {
            redisFree(redis);
        }
    }
    
    bool CacheFingerprint(const std::string& key, 
                         const VFPFingerPrint& fingerprint,
                         int ttlSeconds = 3600) {
        redisReply* reply = (redisReply*)redisCommand(
            redis,
            "SETEX %s %d %b",
            key.c_str(),
            ttlSeconds,
            fingerprint.Data,
            (size_t)fingerprint.DataSize
        );
        
        bool success = (reply && reply->type == REDIS_REPLY_STATUS);
        if (reply) freeReplyObject(reply);
        
        return success;
    }
    
    VFPFingerPrint* GetCachedFingerprint(const std::string& key) {
        redisReply* reply = (redisReply*)redisCommand(redis, "GET %s", key.c_str());
        
        if (reply && reply->type == REDIS_REPLY_STRING) {
            VFPFingerPrint* fp = new VFPFingerPrint();
            fp->DataSize = reply->len;
            fp->Data = new char[reply->len];  // VFPFingerPrint uses char*
            memcpy(fp->Data, reply->str, reply->len);
            
            freeReplyObject(reply);
            return fp;
        }
        
        if (reply) freeReplyObject(reply);
        return nullptr;
    }
};
```

## Performance Optimization

### Connection Pooling

```cpp
class DatabaseConnectionPool {
private:
    std::queue<std::unique_ptr<SQLiteFingerprintDB>> connections;
    std::mutex poolMutex;
    std::condition_variable poolCV;
    std::string dbPath;
    size_t maxConnections;
    
public:
    DatabaseConnectionPool(const std::string& path, size_t maxConn = 10)
        : dbPath(path), maxConnections(maxConn) {
        // Pre-create connections
        for (size_t i = 0; i < maxConnections; i++) {
            connections.push(std::make_unique<SQLiteFingerprintDB>(dbPath));
        }
    }
    
    std::unique_ptr<SQLiteFingerprintDB> GetConnection() {
        std::unique_lock<std::mutex> lock(poolMutex);
        poolCV.wait(lock, [this] { return !connections.empty(); });
        
        auto conn = std::move(connections.front());
        connections.pop();
        return conn;
    }
    
    void ReturnConnection(std::unique_ptr<SQLiteFingerprintDB> conn) {
        std::lock_guard<std::mutex> lock(poolMutex);
        connections.push(std::move(conn));
        poolCV.notify_one();
    }
};
```

### Batch Processing

```cpp
class BatchFingerprintProcessor {
private:
    DatabaseConnectionPool& pool;
    std::vector<std::pair<std::string, VFPFingerPrint>> batch;
    std::mutex batchMutex;
    size_t batchSize;
    
public:
    BatchFingerprintProcessor(DatabaseConnectionPool& p, size_t size = 100)
        : pool(p), batchSize(size) {}
    
    void AddFingerprint(const std::string& path, const VFPFingerPrint& fp) {
        std::lock_guard<std::mutex> lock(batchMutex);
        batch.push_back({path, fp});
        
        if (batch.size() >= batchSize) {
            FlushBatch();
        }
    }
    
    void FlushBatch() {
        if (batch.empty()) return;
        
        auto conn = pool.GetConnection();
        
        // Start transaction for batch insert
        for (const auto& [path, fp] : batch) {
            conn->StoreFingerprint(path, fp);
        }
        
        pool.ReturnConnection(std::move(conn));
        batch.clear();
    }
};
```

## Best Practices

### 1. Index Optimization

- Create indexes on frequently queried fields (video_path, hash)
- Use partial indexes for filtered queries
- Consider composite indexes for complex queries

### 2. Data Compression

```cpp
// Example: Save and load fingerprints using SDK functions
void SaveFingerprintToDB(const std::string& path) {
    VFPFingerprintSource source{};
    VFPFillSource(std::wstring(path.begin(), path.end()).c_str(), &source);
    
    VFPFingerPrint fp{};
    // Generate search fingerprint for fragment detection
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fp);
    
    if (error == nullptr) {
        // Save to file format
        VFPFingerprintSave(&fp, L"temp.vfp");
        
        // Or save legacy format for compatibility
        VFPFingerprintSaveLegacy(&fp, L"temp_legacy.vfp");
        
        // Store in database...
        delete[] fp.Data;
    }
}
```

### 3. Sharding Strategy

For very large databases, implement sharding:

```cpp
class ShardedFingerprintDB {
private:
    std::vector<std::unique_ptr<SQLiteFingerprintDB>> shards;
    
    int GetShardIndex(const std::string& key) {
        std::hash<std::string> hasher;
        return hasher(key) % shards.size();
    }
    
public:
    void StoreFingerprint(const std::string& path, const VFPFingerPrint& fp) {
        int shard = GetShardIndex(path);
        shards[shard]->StoreFingerprint(path, fp);
    }
};
```

## Comparison with .NET MongoDB Integration

| Feature | C++ Custom Implementation | .NET MongoDB Integration |
|---------|--------------------------|-------------------------|
| **Setup Complexity** | Higher (manual schema) | Lower (automatic) |
| **Flexibility** | Complete control | MongoDB-specific |
| **Performance** | Can be optimized | Good out-of-box |
| **Database Options** | Any database | MongoDB only |
| **Code Required** | More boilerplate | Minimal |
| **Maintenance** | Manual updates | SDK updates |

## Next Steps

- [C++ API Reference](api.md) - Complete API documentation
- [Sample Applications](samples/index.md) - Working examples

## Additional Resources

- [SQLite Documentation](https://www.sqlite.org/doclist.html)
- [PostgreSQL libpq Documentation](https://www.postgresql.org/docs/current/libpq.html)
- [Redis C++ Client](https://github.com/redis/hiredis)