---
title: Guía de Base de Datos del SDK de Huellas C++
description: Integra el SDK de Huellas de Video C++ con bases de datos para almacenar y recuperar huellas incluyendo ejemplos de SQLite y PostgreSQL.
---

# Guía de Base de Datos del SDK de Huellas C++

Esta guía demuestra cómo integrar el SDK de Huellas de Video para C++ con varios sistemas de bases de datos para almacenamiento y recuperación eficiente de huellas.

## Descripción General

A diferencia del SDK .NET que proporciona integración incorporada con MongoDB, el SDK C++ ofrece flexibilidad para integrarse con cualquier sistema de base de datos. Esta guía proporciona patrones de implementación y ejemplos para bases de datos comunes.

## Consideraciones de Diseño de Base de Datos

### Requisitos de Almacenamiento de Huellas

- **Datos Binarios**: Las huellas son datos binarios (típicamente 10-100KB por video)
- **Indexación**: Los metadatos del video deben indexarse para consultas rápidas
- **Escalabilidad**: El diseño debe soportar millones de huellas
- **Rendimiento**: Optimizar para operaciones de escritura y lectura

### Esquema Recomendado

```sql
CREATE TABLE video_fingerprints (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    video_path TEXT NOT NULL,
    video_hash VARCHAR(64),
    fingerprint_type VARCHAR(10), -- 'search' o 'compare'
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

## Implementación SQLite

SQLite es ideal para aplicaciones embebidas y despliegues de una sola máquina.

### Integración Básica SQLite

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
            throw std::runtime_error("No se puede abrir la base de datos");
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
            throw std::runtime_error("Error SQL: " + error);
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
        sqlite3_bind_int64(stmt, 4, fingerprint.Duration);  // Duration es INT64
        
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
            
            result->Data = new char[dataSize];  // VFPFingerPrint usa char*
            memcpy(result->Data, data, dataSize);
            result->DataSize = dataSize;
            result->Duration = sqlite3_column_int64(stmt, 1);  // Duration es INT64
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
            
            // Comparar huellas usando la función real del SDK
            double difference = VFPCompare_Compare(
                queryFp.Data, queryFp.DataSize,
                static_cast<const char*>(data), dataSize,
                1000  // maxDifference
            );
            
            if (difference < 1000) {  // Menor diferencia = más similar
                similarities.push_back({path, static_cast<int>(difference)});
            }
        }
        
        sqlite3_finalize(stmt);
        
        // Ordenar por diferencia (menor es mejor)
        std::sort(similarities.begin(), similarities.end(),
                 [](const auto& a, const auto& b) { return a.second < b.second; });
        
        // Retornar mejores resultados
        for (int i = 0; i < std::min(maxResults, (int)similarities.size()); i++) {
            results.push_back(similarities[i].first);
        }
        
        return results;
    }
};
```

### Ejemplo de Uso

```cpp
int main() {
    // Inicializar base de datos
    SQLiteFingerprintDB db("fingerprints.db");
    
    // Establecer licencia
    VFPSetLicenseKey(L"TU-CLAVE-DE-LICENCIA");
    
    // Generar y almacenar huella
    VFPFingerprintSource source{};
    VFPFillSource(L"video.mp4", &source);
    source.StartTime = 0;
    source.StopTime = 60000;  // Primeros 60 segundos
    
    VFPFingerPrint fingerprint{};
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
    
    if (error == nullptr) {
        db.StoreFingerprint("video.mp4", fingerprint);
        std::cout << "Huella almacenada exitosamente" << std::endl;
        
        // Guardar huella en archivo también
        VFPFingerprintSave(&fingerprint, L"video.vfp");
        
        // Encontrar videos similares
        auto similar = db.FindSimilarVideos(fingerprint, 5);
        for (const auto& path : similar) {
            std::cout << "Video similar: " << path << std::endl;
        }
        
        // Nota: No hay función VFPFingerPrint_Free - gestionar memoria manualmente
        delete[] fingerprint.Data;
    }
    
    return 0;
}
```

## Implementación PostgreSQL

Para despliegues más grandes que requieren acceso concurrente y características avanzadas.

### Integración PostgreSQL con libpq

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
            throw std::runtime_error("Conexión fallida: " + error);
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
            throw std::runtime_error("Creación de esquema fallida: " + error);
        }
        PQclear(res);
    }
    
    bool StoreFingerprint(const std::string& videoPath,
                         const VFPFingerPrint& fingerprint,
                         const std::string& metadata = "{}") {
        // Escapar datos binarios para PostgreSQL
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
    
    // Inserción por lotes para mejor rendimiento
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

## Implementación Redis

Para caché de alto rendimiento y procesamiento en tiempo real.

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
            throw std::runtime_error("Conexión Redis fallida");
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
            fp->Data = new char[reply->len];  // VFPFingerPrint usa char*
            memcpy(fp->Data, reply->str, reply->len);
            
            freeReplyObject(reply);
            return fp;
        }
        
        if (reply) freeReplyObject(reply);
        return nullptr;
    }
};
```

## Optimización de Rendimiento

### Pool de Conexiones

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
        // Pre-crear conexiones
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

### Procesamiento por Lotes

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
        
        // Iniciar transacción para inserción por lotes
        for (const auto& [path, fp] : batch) {
            conn->StoreFingerprint(path, fp);
        }
        
        pool.ReturnConnection(std::move(conn));
        batch.clear();
    }
};
```

## Mejores Prácticas

### 1. Optimización de Índices

- Crear índices en campos consultados frecuentemente (video_path, hash)
- Usar índices parciales para consultas filtradas
- Considerar índices compuestos para consultas complejas

### 2. Compresión de Datos

```cpp
// Ejemplo: Guardar y cargar huellas usando funciones del SDK
void SaveFingerprintToDB(const std::string& path) {
    VFPFingerprintSource source{};
    VFPFillSource(std::wstring(path.begin(), path.end()).c_str(), &source);
    
    VFPFingerPrint fp{};
    // Generar huella de búsqueda para detección de fragmentos
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fp);
    
    if (error == nullptr) {
        // Guardar en formato de archivo
        VFPFingerprintSave(&fp, L"temp.vfp");
        
        // O guardar formato legado para compatibilidad
        VFPFingerprintSaveLegacy(&fp, L"temp_legacy.vfp");
        
        // Almacenar en base de datos...
        delete[] fp.Data;
    }
}
```

### 3. Estrategia de Fragmentación

Para bases de datos muy grandes, implementar fragmentación:

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

## Comparación con Integración MongoDB .NET

| Característica | Implementación Personalizada C++ | Integración MongoDB .NET |
|----------------|----------------------------------|--------------------------|
| **Complejidad de Configuración** | Mayor (esquema manual) | Menor (automático) |
| **Flexibilidad** | Control completo | Específico de MongoDB |
| **Rendimiento** | Puede optimizarse | Bueno por defecto |
| **Opciones de BD** | Cualquier base de datos | Solo MongoDB |
| **Código Requerido** | Más repetitivo | Mínimo |
| **Mantenimiento** | Actualizaciones manuales | Actualizaciones del SDK |

## Próximos Pasos

- [Referencia de API C++](api.md) - Documentación completa de la API
- [Aplicaciones de Ejemplo](samples/index.md) - Ejemplos funcionales

## Recursos Adicionales

- [Documentación SQLite](https://www.sqlite.org/doclist.html)
- [Documentación libpq PostgreSQL](https://www.postgresql.org/docs/current/libpq.html)
- [Cliente Redis C++](https://github.com/redis/hiredis)
