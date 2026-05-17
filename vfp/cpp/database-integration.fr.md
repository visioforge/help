---
title: Stockage d'empreintes vidéo en C++ avec SQLite et PostgreSQL
description: Stockez et interrogez les empreintes vidéo dans SQLite et PostgreSQL avec le SDK C++ VisioForge — exemples de schéma et stratégies d'indexation.
tags:
  - Video Fingerprinting SDK
  - C++
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - MP4
primary_api_classes:
  - VFPFingerprintSource
  - VFPFingerPrint

---

# Guide de base de données pour le SDK Fingerprinting C++

Ce guide démontre comment intégrer le Video Fingerprinting SDK pour C++ avec divers systèmes de base de données pour un stockage et une récupération efficaces des empreintes.

## Vue d'ensemble

Contrairement au SDK .NET qui fournit une intégration MongoDB intégrée, le SDK C++ offre la flexibilité d'intégrer n'importe quel système de base de données. Ce guide fournit des schémas d'implémentation et des exemples pour les bases de données courantes.

## Considérations de conception de base de données

### Exigences de stockage des empreintes

- **Données binaires** : les empreintes sont des données binaires (typiquement 10 à 100 Ko par vidéo)
- **Indexation** : les métadonnées vidéo doivent être indexées pour des requêtes rapides
- **Évolutivité** : la conception doit prendre en charge des millions d'empreintes
- **Performance** : optimiser pour les opérations de lecture et d'écriture

### Schéma recommandé

```sql
CREATE TABLE video_fingerprints (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    video_path TEXT NOT NULL,
    video_hash VARCHAR(64),
    fingerprint_type VARCHAR(10), -- 'search' ou 'compare'
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

## Implémentation SQLite

SQLite est idéal pour les applications embarquées et les déploiements sur une seule machine.

### Intégration SQLite de base

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
            throw std::runtime_error("Impossible d'ouvrir la base");
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
            throw std::runtime_error("Erreur SQL : " + error);
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
        sqlite3_bind_int64(stmt, 4, fingerprint.Duration);  // Duration est INT64
        
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
            
            result->Data = new char[dataSize];  // VFPFingerPrint utilise char*
            memcpy(result->Data, data, dataSize);
            result->DataSize = dataSize;
            result->Duration = sqlite3_column_int64(stmt, 1);  // Duration est INT64
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
            
            // Comparer les empreintes en utilisant la fonction réelle du SDK
            double difference = VFPCompare_Compare(
                queryFp.Data, queryFp.DataSize,
                static_cast<const char*>(data), dataSize,
                1000  // maxDifference
            );
            
            if (difference < 1000) {  // Différence plus basse = plus similaire
                similarities.push_back({path, static_cast<int>(difference)});
            }
        }
        
        sqlite3_finalize(stmt);
        
        // Trier par différence (plus bas est meilleur)
        std::sort(similarities.begin(), similarities.end(),
                 [](const auto& a, const auto& b) { return a.second < b.second; });
        
        // Retourner les meilleurs résultats
        for (int i = 0; i < std::min(maxResults, (int)similarities.size()); i++) {
            results.push_back(similarities[i].first);
        }
        
        return results;
    }
};
```

### Exemple d'utilisation

```cpp
int main() {
    // Initialiser la base de données
    SQLiteFingerprintDB db("fingerprints.db");
    
    // Définir la licence
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");
    
    // Générer et stocker l'empreinte
    // Générer l'empreinte via l'API de haut niveau
    VFPFingerprintSource src{};
    VFPFillSource(L"video.mp4", &src);

    VFPFingerPrint fingerprint{};
    VFPSearch_GetFingerprintForVideoFile(src, &fingerprint);

    db.StoreFingerprint("video.mp4", fingerprint);
    std::cout << "Empreinte stockée avec succès" << std::endl;

    // Enregistrer également l'empreinte dans un fichier
    VFPFingerprintSave(&fingerprint, L"video.vfp");

    // Trouver des vidéos similaires
    auto similar = db.FindSimilarVideos(fingerprint, 5);
    for (const auto& path : similar) {
        std::cout << "Vidéo similaire : " << path << std::endl;
    }

    return 0;
}
```

## Implémentation PostgreSQL

Pour les déploiements plus importants nécessitant un accès concurrent et des fonctionnalités avancées.

### Intégration PostgreSQL avec libpq

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
            throw std::runtime_error("Échec de connexion : " + error);
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
            throw std::runtime_error("Échec de création du schéma : " + error);
        }
        PQclear(res);
    }
    
    bool StoreFingerprint(const std::string& videoPath,
                         const VFPFingerPrint& fingerprint,
                         const std::string& metadata = "{}") {
        // Échapper les données binaires pour PostgreSQL
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
    
    // Insertion par lot pour de meilleures performances
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

## Implémentation Redis

Pour la mise en cache haute performance et le traitement en temps réel.

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
            throw std::runtime_error("Échec de connexion Redis");
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
            fp->Data = new char[reply->len];  // VFPFingerPrint utilise char*
            memcpy(fp->Data, reply->str, reply->len);
            
            freeReplyObject(reply);
            return fp;
        }
        
        if (reply) freeReplyObject(reply);
        return nullptr;
    }
};
```

## Optimisation des performances

### Pooling de connexions

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
        // Pré-créer les connexions
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

### Traitement par lots

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
        
        // Démarrer la transaction pour l'insertion par lot
        for (const auto& [path, fp] : batch) {
            conn->StoreFingerprint(path, fp);
        }
        
        pool.ReturnConnection(std::move(conn));
        batch.clear();
    }
};
```

## Bonnes pratiques

### 1. Optimisation des index

- Créer des index sur les champs fréquemment interrogés (video_path, hash)
- Utiliser des index partiels pour les requêtes filtrées
- Envisager des index composites pour les requêtes complexes

### 2. Compression des données

```cpp
// Exemple : enregistrer et charger les empreintes en utilisant les fonctions du SDK
void SaveFingerprintToDB(const std::string& path) {
    // Générer l'empreinte via l'API de haut niveau
    VFPFingerprintSource src{};
    VFPFillSource(std::wstring(path.begin(), path.end()).c_str(), &src);

    VFPFingerPrint fp{};
    VFPSearch_GetFingerprintForVideoFile(src, &fp);

    // Enregistrer au format fichier
    VFPFingerprintSave(&fp, L"temp.vfp");

    // Ou enregistrer au format hérité pour la compatibilité
    VFPFingerprintSaveLegacy(&fp, L"temp_legacy.vfp");

    // Stocker en base de données...
}
```

### 3. Stratégie de sharding

Pour de très grandes bases de données, implémenter le sharding :

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

## Comparaison avec l'intégration MongoDB .NET

| Fonctionnalité | Implémentation C++ personnalisée | Intégration MongoDB .NET |
|---------|--------------------------|-------------------------|
| **Complexité de configuration** | Plus élevée (schéma manuel) | Plus faible (automatique) |
| **Flexibilité** | Contrôle complet | Spécifique à MongoDB |
| **Performance** | Peut être optimisée | Bonne d'emblée |
| **Options de base de données** | Toute base de données | MongoDB uniquement |
| **Code requis** | Plus de code répétitif | Minimal |
| **Maintenance** | Mises à jour manuelles | Mises à jour SDK |

## Étapes suivantes

- [Référence de l'API C++](api.md) — documentation complète de l'API
- [Applications d'exemple](samples/index.md) — exemples fonctionnels

## Ressources supplémentaires

- [Documentation SQLite](https://www.sqlite.org/doclist.html)
- [Documentation libpq PostgreSQL](https://www.postgresql.org/docs/current/libpq.html)
- [Client Redis C++](https://github.com/redis/hiredis)
