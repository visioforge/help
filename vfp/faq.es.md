---
title: Preguntas Frecuentes del SDK de Video Fingerprinting
description: Encuentra respuestas sobre el SDK de Video Fingerprinting de VisioForge incluyendo licencias, rendimiento, precisión, formatos y compatibilidad de plataformas.
---

# Preguntas Frecuentes del SDK de Video Fingerprinting

Esta FAQ completa aborda las preguntas más comunes sobre el SDK de Video Fingerprinting de VisioForge. Si no puedes encontrar tu respuesta aquí, por favor visita nuestro [foro de soporte](https://support.visioforge.com/) o [comunidad de Discord](https://discord.com/invite/yvXUG56WCH).

## Tabla de Contenidos

- [Preguntas sobre Licencias](#preguntas-sobre-licencias)
- [Rendimiento y Optimización](#rendimiento-y-optimizacion)
- [Precisión y Detección](#precision-y-deteccion)
- [Formatos y Códecs Soportados](#formatos-y-codecs-soportados)
- [Integración y Desarrollo](#integracion-y-desarrollo)
- [Base de Datos y Almacenamiento](#base-de-datos-y-almacenamiento)

## Preguntas sobre Licencias

### P: ¿Cuál es la diferencia entre licencias de Prueba y Comerciales?

**R:** Las diferencias clave son:

| Característica | Prueba | Comercial |
|---------------|--------|-----------|
| Duración | 30 días | Perpetua |
| Marca de agua | Sí (superposición visual) | No |
| Todas las Características | Sí | Sí |
| Uso en Producción | No | Sí |
| Soporte Técnico | Solo foro | Email/Prioridad/Teléfono |
| Actualizaciones | Solo período de prueba | 1 año |

### P: ¿Puedo usar la licencia de Prueba para desarrollo?

**R:** ¡Sí! La licencia de prueba es perfecta para desarrollo y pruebas. Incluye todas las características con una marca de agua. Puedes desarrollar toda tu aplicación con la licencia de prueba y comprar una licencia comercial cuando estés listo para producción.

```csharp
// Entorno de desarrollo
VFPAnalyzer.SetLicenseKey("TRIAL");

// Entorno de producción
VFPAnalyzer.SetLicenseKey(Environment.GetEnvironmentVariable("VFP_LICENSE_KEY"));
```

### P: ¿Qué sucede cuando expira mi prueba?

**R:** Después de 30 días, el SDK dejará de procesar videos y devolverá un error. Tu código permanece intacto - simplemente compra una licencia y actualiza la clave para continuar.

### P: ¿Hay limitaciones de características con diferentes licencias?

**R:** No, la licencia comercial incluye todas las características sin limitaciones. La única diferencia entre licencias de prueba y comerciales es la marca de agua en modo de prueba.

### P: ¿Puedo usar una licencia en múltiples máquinas?

**R:** Los términos de licencia dependen de tu compra:
- **Licencia de Desarrollador Individual**: Un desarrollador, máquinas de desarrollo ilimitadas
- **Licencia de Sitio**: Desarrolladores ilimitados en una ubicación física
- **Licencia Empresarial**: Desarrolladores ilimitados en múltiples ubicaciones

Para despliegue, necesitas una licencia de runtime para cada servidor de producción o aplicación distribuida.

### P: ¿Cómo manejo las licencias en una aplicación distribuida?

**R:** Para aplicaciones distribuidas (instaladas en máquinas de clientes), necesitas:

```csharp
public class LicenseManager
{
    private const string EncryptedLicense = "YOUR_ENCRYPTED_LICENSE";
    
    public static void Initialize()
    {
        // Desencriptar licencia en tiempo de ejecución
        string licenseKey = DecryptLicense(EncryptedLicense);
        VFPAnalyzer.SetLicenseKey(licenseKey);
    }
    
    private static string DecryptLicense(string encrypted)
    {
        // Implementa tu lógica de desencriptación
        // Nunca almacenes licencias de texto plano en aplicaciones distribuidas
        return Decrypt(encrypted);
    }
}
```

## Rendimiento y Optimización

### P: ¿Qué tan rápido puede el SDK procesar videos?

**R:** La velocidad de procesamiento depende de varios factores:

| Factor | Impacto en Velocidad |
|--------|---------------------|
| Resolución de Video | 4K: ~0.5x tiempo real, 1080p: ~2x tiempo real, 480p: ~5x tiempo real |
| Núcleos CPU | Escalado lineal hasta 8 núcleos |
| Aceleración por Hardware | 2-5x más rápido con soporte GPU |
| Tipo de Almacenamiento | SSD proporciona mejora de velocidad del 30-50% |
| Tipo de Huella Digital | Huellas de búsqueda son 2x más lentas que Comparar |

Referencias típicas en hardware moderno (Intel i7, 16GB RAM, SSD):
- **Video 1080p**: 60-120 segundos por hora de contenido
- **Video 720p**: 30-60 segundos por hora de contenido
- **Video 480p**: 15-30 segundos por hora de contenido

### P: ¿Cuánta memoria requiere la generación de huellas digitales?

**R:** El uso de memoria escala con la resolución y duración del video:

```csharp
// Cálculo aproximado de uso de memoria
long EstimateMemoryUsage(int width, int height, int durationSeconds)
{
    // Memoria base para decodificador y búferes
    long baseMemory = 100 * 1024 * 1024; // 100 MB
    
    // Memoria de búfer de fotogramas (3-5 fotogramas típicamente en búfer)
    long frameSize = width * height * 3; // RGB
    long frameBuffers = frameSize * 5;
    
    // Datos de huella digital (aproximadamente 1KB por segundo)
    long fingerprintSize = durationSeconds * 1024;
    
    return baseMemory + frameBuffers + fingerprintSize;
}

// Ejemplo: Video 1080p, 10 minutos
// Memoria ≈ 100MB + (1920*1080*3*5) + (600*1KB) ≈ 131MB
```

### P: ¿Puedo procesar múltiples videos simultáneamente?

**R:** Sí, pero con consideraciones:

```csharp
public class ParallelProcessor
{
    private readonly SemaphoreSlim _semaphore;
    
    public ParallelProcessor(int maxConcurrency = 4)
    {
        // Limitar basado en núcleos CPU y memoria disponible
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
                // Reducir resolución para procesamiento paralelo
                CustomResolution = new Size(640, 480)
            };
            
            var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
            // Procesar huella digital...
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

Concurrencia recomendada:
- **8GB RAM**: 2-3 videos concurrentes
- **16GB RAM**: 4-6 videos concurrentes
- **32GB RAM**: 8-12 videos concurrentes

### P: ¿Cómo puedo optimizar para procesamiento en tiempo real?

**R:** Para procesamiento en tiempo real o casi en tiempo real:

1. **Usar aceleración por hardware**:
```csharp
var source = new VFPFingerprintSource(videoPath)
{
    UseHardwareAcceleration = true,
    HardwareDevice = "cuda" // o "qsv", "d3d11va"
};
```

2. **Procesar en segmentos**:
```csharp
// Procesar segmentos de 30 segundos para streams en vivo
var source = new VFPFingerprintSource(streamUrl)
{
    StartTime = TimeSpan.FromSeconds(segmentIndex * 30),
    StopTime = TimeSpan.FromSeconds((segmentIndex + 1) * 30),
    CustomResolution = new Size(480, 360) // Resolución más baja
};
```

3. **Usar salto de fotogramas**:
```csharp
var source = new VFPFingerprintSource(videoPath)
{
    FrameRate = 5 // Procesar 5 fps en lugar de tasa de fotogramas completa
};
```

## Precisión y Detección

### P: ¿Qué tan precisa es la coincidencia de video?

**R:** La precisión depende del tipo de transformación:

| Transformación | Tasa de Detección | Tasa de Falsos Positivos |
|----------------|-------------------|-------------------------|
| Re-codificación | 99.9% | <0.1% |
| Cambio de resolución | 99.5% | <0.1% |
| Superposición de marca de agua/logo | 98% | <0.5% |
| Recorte (< 20%) | 95% | <1% |
| Ajuste de color | 93% | <2% |
| Compresión pesada | 90% | <3% |
| Transformaciones combinadas | 85-95% | <5% |

### P: ¿Qué umbral de similitud debería usar?

**R:** Recomendaciones de umbral basadas en caso de uso:

```csharp
public enum MatchingThreshold
{
    Identical = 5,        // Mismo archivo, diferente codificación
    NearDuplicate = 15,   // Diferencias menores de calidad
    Similar = 30,         // Mismo contenido, algunas modificaciones
    Related = 50,         // Modificaciones significativas (marcas de agua, etc.)
    PossiblyRelated = 100 // Transformaciones pesadas
}

public bool IsMatch(int difference, MatchingThreshold threshold)
{
    return difference <= (int)threshold;
}
```

Casos de uso:
- **Detección de derechos de autor**: Usar `Similar` (30) o más estricto
- **Búsqueda de duplicados**: Usar `NearDuplicate` (15)
- **Monitoreo de contenido**: Usar `Related` (50) para flexibilidad
- **Detección de escena**: Usar `PossiblyRelated` (100)

### P: ¿Puede el SDK detectar coincidencias parciales?

**R:** ¡Sí! El SDK sobresale en encontrar fragmentos de video:

```csharp
// Buscar un clip de 30 segundos en una película de 2 horas
var searchFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    new VFPFingerprintSource("clip.mp4")
);

var mainFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    new VFPFingerprintSource("movie.mp4")
);

var results = VFPAnalyzer.Search(mainFp, searchFp, searchFp.Duration);

foreach (var result in results)
{
    Console.WriteLine($"Encontrada coincidencia en {result.Position} con puntuación {result.Score}");
}
```

### P: ¿Cómo maneja el SDK diferentes relaciones de aspecto?

**R:** El SDK normaliza las relaciones de aspecto automáticamente, pero puedes mejorar la precisión:

```csharp
// Para videos con letterboxing/pillarboxing
var source = new VFPFingerprintSource(videoPath)
{
    // Ignorar barras negras
    IgnoredAreas = new List<Rectangle>
    {
        new Rectangle(0, 0, 1920, 140),    // Letterbox superior
        new Rectangle(0, 940, 1920, 140)   // Letterbox inferior
    }
};

// O usar recorte inteligente
var source = new VFPFingerprintSource(videoPath)
{
    AutoCropBlackBars = true
};
```

### P: ¿Puede detectar videos reflejados o rotados?

**R:** El SDK puede detectar:
- ✅ Reflejo horizontal (con preprocesamiento)
- ✅ Rotaciones menores (< 5 grados)
- ⚠️ Rotaciones de 90/180/270 grados (requiere preprocesamiento manual)

```csharp
// Para detección de video reflejado
public async Task<bool> CheckMirroredMatch(string video1, string video2)
{
    var fp1 = await GenerateFingerprint(video1);
    var fp2 = await GenerateFingerprint(video2);
    
    // Verificar orientación normal
    int normalDiff = VFPAnalyzer.Compare(fp1, fp2);
    if (normalDiff < 30) return true;
    
    // Verificar versión reflejada
    var fp2Mirrored = await GenerateFingerprint(video2, mirror: true);
    int mirroredDiff = VFPAnalyzer.Compare(fp1, fp2Mirrored);
    
    return mirroredDiff < 30;
}
```

## Formatos y Códecs Soportados

### P: ¿Qué formatos de video están soportados?

**R:** El SDK soporta virtualmente todos los formatos comunes a través de GStreamer:

**Formatos de Contenedor**:
- MP4, M4V, MOV
- AVI, WMV, ASF
- MKV, WebM
- FLV, F4V
- MPEG, MPG, M2TS, TS
- 3GP, 3G2
- OGV, OGG
- Y muchos más...

**Códecs de Video**:
- H.264/AVC
- H.265/HEVC
- VP8, VP9
- MPEG-1, MPEG-2, MPEG-4
- WMV7, WMV8, WMV9
- ProRes, DNxHD
- AV1 (con plugin)

### P: ¿Hay limitaciones de formato?

**R:** Existen algunas limitaciones:

1. **Contenido protegido por DRM**: No puede procesar videos encriptados
2. **Códecs raros**: Puede requerir plugins adicionales de GStreamer
3. **Archivos corruptos**: Procesamiento parcial posible con manejo de errores
4. **Streams en vivo**: Soportados pero requieren procesamiento segmentado

### P: ¿Qué pasa con archivos solo de audio?

**R:** El SDK de Video Fingerprinting requiere contenido de video.

## Compatibilidad de Plataformas

### P: ¿Qué plataformas están soportadas?

**R:** Matriz completa de soporte de plataformas:

| Plataforma | Arquitectura | Versión .NET | Estado |
|------------|-------------|-------------|---------|
| Windows 10/11 | x86, x64, ARM64 | .NET Framework 4.6.1+, .NET 6+ | ✅ Soporte Completo |
| Windows Server 2016+ | x64 | .NET Framework 4.6.1+, .NET 6+ | ✅ Soporte Completo |
| Ubuntu 20.04+ | x64, ARM64 | .NET 6+ | ✅ Soporte Completo |
| Debian 11+ | x64, ARM64 | .NET 6+ | ✅ Soporte Completo |
| RHEL/CentOS 8+ | x64 | .NET 6+ | ✅ Soporte Completo |
| macOS 12+ | x64, ARM64 | .NET 6+ | ✅ Soporte Completo |
| Docker/Kubernetes | Basado en Linux | .NET 6+ | ✅ Soporte Completo |

### P: ¿Puedo usar el SDK en contenedores Docker?

**R:** ¡Sí! Aquí hay un Dockerfile de muestra:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# Instalar GStreamer y dependencias
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

### P: ¿Hay diferencias de rendimiento específicas de plataforma?

**R:** Sí, el rendimiento varía por plataforma:

| Plataforma | Rendimiento Relativo | Aceleración por Hardware |
|------------|---------------------|-------------------------|
| Windows x64 | 100% (línea base) | NVENC, Quick Sync, D3D11 |
| Linux x64 | 95-100% | NVENC, VAAPI |
| macOS Intel | 90-95% | VideoToolbox |
| macOS ARM64 | 85-90% | VideoToolbox |
| Windows ARM64 | 70-80% | Limitado |

### P: ¿Puedo usar el SDK en entornos de nube?

**R:** Sí, el SDK funciona en todas las principales plataformas de nube:

**AWS**:
```csharp
// Usar instancias GPU para mejor rendimiento
// Recomendado: g4dn.xlarge o p3.2xlarge
```

**Azure**:
```csharp
// Usar VMs de serie NCv3 o NVv4
// Habilitar aceleración GPU en instancias de contenedor
```

**Google Cloud**:
```csharp
// Usar N1 con GPUs NVIDIA Tesla
// O serie de máquinas A2 para mejor rendimiento
```

## Integración y Desarrollo

### P: ¿Puedo usar el SDK en una aplicación web?

**R:** Sí, para procesamiento del lado del servidor en ASP.NET Core:

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
            // Almacenar huella digital en base de datos
            
            return Ok(new { Success = true, FingerprintId = fingerprint.Id });
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
```

### P: ¿Cómo implemento una barra de progreso?

**R:** Usar el callback de progreso:

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
                Console.WriteLine($"Error: {error}");
            },
            progressDelegate: (progress) => {
                ProgressChanged?.Invoke(this, progress);
            }
        );
    }
}

// Uso en WPF/WinForms
tracker.ProgressChanged += (s, progress) => {
    Dispatcher.Invoke(() => {
        progressBar.Value = progress;
        labelStatus.Text = $"Procesando: {progress}%";
    });
};
```

## Base de Datos y Almacenamiento

### P: ¿Cómo debería almacenar huellas digitales en una base de datos?

**R:** Mejores prácticas para almacenamiento en base de datos:

```csharp
// Ejemplo de SQL Server
public class FingerprintRepository
{
    public async Task SaveFingerprint(VFPFingerPrint fp, string videoId)
    {
        // Opción 1: Almacenar como binario
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

// Ejemplo de MongoDB
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

### P: ¿Cuánto espacio de almacenamiento requieren las huellas digitales?

**R:** Los tamaños de huella digital son predecibles:

| Tipo de Huella Digital | Fórmula de Tamaño | Ejemplo (video de 10 min) |
|-----------------------|-------------------|---------------------------|
| Comparar | ~100 bytes/segundo | ~60 KB |
| Buscar | ~1 KB/segundo | ~600 KB |
| Con miniaturas | +10 KB/minuto | ~660 KB |

Planificación de almacenamiento:
```csharp
public long EstimateStorageNeeded(int videoCount, double avgDurationMinutes)
{
    // Huellas de búsqueda (peor caso)
    long bytesPerMinute = 60 * 1024; // 60 KB
    long fingerprintSize = (long)(bytesPerMinute * avgDurationMinutes);
    
    // Agregar 20% de sobrecarga para metadatos
    long totalPerVideo = (long)(fingerprintSize * 1.2);
    
    // Almacenamiento total necesario
    return videoCount * totalPerVideo;
}

// Ejemplo: 10,000 videos, promedio de 30 minutos
// Almacenamiento ≈ 10,000 * (60KB * 30 * 1.2) = ~21 GB
```

### P: ¿Debería usar almacenamiento de sistema de archivos o base de datos?

**R:** Depende de tus requerimientos:

| Tipo de Almacenamiento | Pros | Contras | Mejor Para |
|----------------------|------|---------|------------|
| Sistema de Archivos | Rápido, simple, respaldo fácil | Difícil consultar, sin transacciones | < 10,000 videos |
| Base de Datos SQL | ACID, consultable, metadatos | Más lento, límites de tamaño | 10,000 - 100,000 videos |
| Base de Datos NoSQL | Escalable, flexible | Configuración compleja | > 100,000 videos |
| Almacenamiento de Objetos (S3) | Escala ilimitada, barato | Latencia de red | Archivo/respaldo |

Enfoque híbrido (recomendado para gran escala):
```csharp
public class HybridStorage
{
    // Metadatos en base de datos para consultas rápidas
    private readonly SqlConnection _db;
    
    // Datos de huella digital en almacenamiento de objetos
    private readonly IS3Client _s3;
    
    public async Task SaveFingerprint(VFPFingerPrint fp, VideoMetadata metadata)
    {
        // Guardar huella digital en S3
        string s3Key = $"fingerprints/{metadata.VideoId}.vfp";
        await _s3.PutObjectAsync(s3Key, fp.Save());
        
        // Guardar metadatos en base de datos
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

### P: ¿Cómo depuro problemas de rendimiento?

**R:** Usar perfilado y métricas:

```csharp
public class PerformanceMonitor
{
    public async Task<FingerprintMetrics> ProcessWithMetrics(string videoPath)
    {
        var metrics = new FingerprintMetrics();
        var sw = Stopwatch.StartNew();
        
        // Verificar tamaño de archivo
        var fileInfo = new FileInfo(videoPath);
        metrics.FileSize = fileInfo.Length;
        
        // Monitorear memoria antes
        long memoryBefore = GC.GetTotalMemory(false);
        
        var source = new VFPFingerprintSource(videoPath);
        var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
            source,
            progressDelegate: (progress) => {
                if (progress % 10 == 0)
                {
                    long currentMemory = GC.GetTotalMemory(false);
                    Console.WriteLine($"Progreso: {progress}%, Memoria: {currentMemory / 1024 / 1024} MB");
                }
            }
        );
        
        sw.Stop();
        
        metrics.ProcessingTime = sw.Elapsed;
        metrics.MemoryUsed = GC.GetTotalMemory(false) - memoryBefore;
        metrics.ProcessingSpeed = fileInfo.Length / sw.Elapsed.TotalSeconds;
        
        Console.WriteLine($"Reporte de Rendimiento:");
        Console.WriteLine($"  Tamaño de Archivo: {metrics.FileSize / 1024 / 1024} MB");
        Console.WriteLine($"  Tiempo de Procesamiento: {metrics.ProcessingTime}");
        Console.WriteLine($"  Memoria Usada: {metrics.MemoryUsed / 1024 / 1024} MB");
        Console.WriteLine($"  Velocidad: {metrics.ProcessingSpeed / 1024 / 1024} MB/s");
        
        return metrics;
    }
}
```

## Recursos Adicionales

- **[Referencia de API](https://api.visioforge.org/dotnet/)** - Documentación completa de API
- **[Muestras de GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)** - Ejemplos de código
- **[Comunidad de Discord](https://discord.com/invite/yvXUG56WCH)** - Obtén ayuda de la comunidad

## Contactar Soporte

Si no puedes encontrar una respuesta en esta FAQ:

1. **Buscar en el foro**: [https://support.visioforge.com/](https://support.visioforge.com/)
2. **Unirse a Discord**: [https://discord.com/invite/yvXUG56WCH](https://discord.com/invite/yvXUG56WCH)
3. **Email de soporte**: support@visioforge.com (licencias comerciales)
4. **Reportar bugs**: [Issues de GitHub](https://github.com/visioforge/.Net-SDK-s-samples/issues)

Al contactar soporte, por favor incluye:
- Versión del SDK
- Plataforma y versión .NET
- Tipo de licencia
- Mensajes de error y trazas de pila
- Código de muestra reproduciendo el problema
- Registros de depuración si están disponibles