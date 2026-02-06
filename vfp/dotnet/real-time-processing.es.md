---
title: Guía de Huellas de Video en Tiempo Real
description: Procesa streams de video en vivo y genera huellas en tiempo real usando procesamiento basado en fragmentos con el SDK de Huellas de Video de VisioForge.
---

# Guía de Huellas de Video en Tiempo Real

Esta guía demuestra cómo procesar streams de video en vivo y generar huellas en tiempo real usando el SDK de Huellas de Video de VisioForge. El enfoque usa procesamiento basado en fragmentos para construir huellas de streams de video continuos.

## Descripción General

Las huellas en tiempo real funcionan mediante:
1. Captura de fotogramas de una fuente en vivo (cámara, stream IP, etc.)
2. Acumulación de fotogramas en fragmentos basados en tiempo
3. Construcción de huellas a partir de fragmentos completados
4. Búsqueda de coincidencias contra huellas de referencia
5. Uso de fragmentos superpuestos para mejor precisión de detección

## Componentes Principales

### Clase VFPSearch

La clase `VFPSearch` proporciona métodos estáticos para procesamiento de fotogramas en tiempo real:

- `Process()` - Procesa fotogramas individuales y los agrega a los datos de búsqueda
- `Build()` - Construye una huella a partir de datos de búsqueda acumulados

### Clase VFPSearchData

Contenedor para acumular datos de fotogramas durante el procesamiento en tiempo real:

```csharp
// Crear contenedor de datos de búsqueda para una duración específica
var searchData = new VFPSearchData(TimeSpan.FromSeconds(10));
```

## Ejemplo Completo: Procesamiento de Huellas en Tiempo Real

Aquí hay un ejemplo completo basado en código de producción real que procesa video en vivo y detecta coincidencias de contenido:

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
    // Contenedor de datos en vivo basado en fragmentos
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

    // Clase procesadora principal
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
        // Inicializar captura de video
        _videoCapture = new VideoCaptureCoreX();
        _videoCapture.OnVideoFrameBuffer += OnVideoFrameReceived;
        
        // Inicializar cola de procesamiento
        _fingerprintQueue = new ConcurrentQueue<FingerprintLiveData>();
        
        // Cargar huellas de referencia (anuncios, contenido con derechos, etc.)
        _referenceFingerprints = await LoadReferenceFingerprintsAsync();
        
        // Calcular duración óptima de fragmento
        // Debe ser al menos 2x la duración del contenido de referencia más largo
        var maxReferenceDuration = GetMaxReferenceDuration();
        _fragmentDuration = ((maxReferenceDuration + 1000) / 1000 + 1) * 1000 * 2;
    }

    private async Task<List<VFPFingerPrint>> LoadReferenceFingerprintsAsync()
    {
        var fingerprints = new List<VFPFingerPrint>();
        
        // Cargar o generar huellas de referencia
        foreach (var videoFile in GetReferenceVideos())
        {
            VFPFingerPrint fp;
            
            // Verificar si la huella ya existe
            var fpFile = videoFile + ".vfsigx";
            if (File.Exists(fpFile))
            {
                fp = VFPFingerPrint.Load(fpFile);
            }
            else
            {
                // Generar y guardar huella
                var source = new VFPFingerprintSource(videoFile);
                fp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
                    source, 
                    progressCallback: null,
                    cancellationToken: default);
                
                fp.Save(fpFile);
            }
            
            fingerprints.Add(fp);
        }
        
        return fingerprints;
    }

    private void OnVideoFrameReceived(object sender, VideoFrameXBufferEventArgs e)
    {
        // Asignar buffer temporal si es necesario
        if (_tempBuffer == IntPtr.Zero)
        {
            var stride = ((e.Frame.Width * 3 + 3) / 4) * 4; // stride RGB24
            _tempBuffer = Marshal.AllocCoTaskMem(stride * e.Frame.Height);
        }

        // Procesar fragmento principal
        ProcessMainFragment(e);
        
        // Procesar fragmento superpuesto para mejor detección
        ProcessOverlappingFragment(e);
    }

    private void ProcessMainFragment(VideoFrameXBufferEventArgs e)
    {
        // Inicializar nuevo fragmento si es necesario
        if (_searchLiveData == null)
        {
            _searchLiveData = new FingerprintLiveData(
                TimeSpan.FromMilliseconds(_fragmentDuration), 
                DateTime.Now);
            _fragmentCount++;
        }

        // Calcular marca de tiempo relativa al inicio del fragmento
        long timestamp = (long)(e.Frame.Timestamp.TotalMilliseconds * 1000);
        
        // Verificar si todavía estamos dentro de la duración del fragmento actual
        if (timestamp < _fragmentDuration * _fragmentCount)
        {
            // Copiar datos del fotograma al buffer temporal
            CopyMemory(_tempBuffer, e.Frame.Data, e.Frame.DataSize);
            
            // Calcular marca de tiempo corregida (relativa al inicio del fragmento)
            var correctedTimestamp = timestamp - _fragmentDuration * (_fragmentCount - 1);
            
            // Procesar fotograma y agregar a datos de búsqueda
            VFPSearch.Process(
                _tempBuffer,                                    // Datos del fotograma
                e.Frame.Width,                                   // Ancho
                e.Frame.Height,                                  // Alto
                ((e.Frame.Width * 3 + 3) / 4) * 4,             // Stride
                TimeSpan.FromMilliseconds(correctedTimestamp),  // Marca de tiempo
                ref _searchLiveData.Data);                      // Datos de búsqueda
        }
        else
        {
            // Fragmento completo, encolar para procesamiento
            _fingerprintQueue.Enqueue(_searchLiveData);
            _searchLiveData = null;
            
            // Procesar fragmentos en cola
            ProcessQueuedFragments();
        }
    }

    private void ProcessOverlappingFragment(VideoFrameXBufferEventArgs e)
    {
        long timestamp = (long)(e.Frame.Timestamp.TotalMilliseconds * 1000);
        
        // Iniciar procesamiento de superposición después de la mitad de la duración del fragmento
        if (timestamp < _fragmentDuration / 2)
            return;

        // Inicializar fragmento superpuesto si es necesario
        if (_searchLiveOverlapData == null)
        {
            _searchLiveOverlapData = new FingerprintLiveData(
                TimeSpan.FromMilliseconds(_fragmentDuration),
                DateTime.Now);
            _overlapFragmentCount++;
        }

        // Verificar si estamos dentro de la duración del fragmento superpuesto
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
            // Fragmento superpuesto completo
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
                // Construir huella a partir de datos del fragmento
                long dataSize;
                IntPtr fingerprintPtr = VFPSearch.Build(out dataSize, ref fragmentData.Data);
                
                // Crear objeto de huella
                var fingerprint = new VFPFingerPrint
                {
                    Data = new byte[dataSize],
                    OriginalFilename = string.Empty
                };
                
                Marshal.Copy(fingerprintPtr, fingerprint.Data, 0, (int)dataSize);
                
                // Buscar coincidencias contra huellas de referencia
                SearchForMatches(fingerprint, fragmentData.StartTime);
                
                // Limpiar
                fragmentData.Data.Free();
                fragmentData.Dispose();
            }
        }
    }

    private void SearchForMatches(VFPFingerPrint liveFingerprint, DateTime fragmentStartTime)
    {
        foreach (var referenceFingerprint in _referenceFingerprints)
        {
            // Buscar coincidencias con umbral de diferencia configurable
            var matches = VFPAnalyzer.Search(
                referenceFingerprint,           // Huella de referencia
                liveFingerprint,                // Huella en vivo a buscar
                referenceFingerprint.Duration,  // Duración a buscar
                differenceLevel: 10,             // Umbral de similitud (0-100)
                multipleSearch: true);           // Encontrar todas las coincidencias
            
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    // Calcular marca de tiempo real de la coincidencia
                    var matchTime = fragmentStartTime.AddMilliseconds(match.TotalMilliseconds);
                    
                    OnMatchFound(new MatchResult
                    {
                        ReferenceFile = referenceFingerprint.OriginalFilename,
                        Timestamp = matchTime,
                        Position = match,
                        Confidence = CalculateConfidence(differenceLevel: 10)
                    });
                }
            }
        }
    }

    public async Task StartCameraStreamAsync(string deviceName, string format, double frameRate)
    {
        // Configurar fuente de cámara
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
        // Configurar fuente de stream de red
        var sourceSettings = await UniversalSourceSettings.CreateAsync(streamUrl);
        
        // Para cámaras IP con autenticación
        if (requiresAuth)
        {
            sourceSettings.Login = "usuario";
            sourceSettings.Password = "contraseña";
        }
        
        await _videoCapture.StartAsync(sourceSettings);
    }

    public void Stop()
    {
        _videoCapture?.Stop();
        
        // Procesar fragmentos restantes
        while (_fingerprintQueue.TryDequeue(out var fragment))
        {
            ProcessQueuedFragments();
        }
        
        // Limpiar
        if (_tempBuffer != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(_tempBuffer);
            _tempBuffer = IntPtr.Zero;
        }
        
        _searchLiveData?.Dispose();
        _searchLiveOverlapData?.Dispose();
    }

    // Métodos auxiliares
    private long GetMaxReferenceDuration()
    {
        return _referenceFingerprints.Max(fp => (long)fp.Duration.TotalMilliseconds);
    }

    private List<string> GetReferenceVideos()
    {
        // Retornar lista de archivos de video de referencia
        return new List<string> { "anuncio1.mp4", "anuncio2.mp4", "contenido_protegido.mp4" };
    }

    private double CalculateConfidence(int differenceLevel)
    {
        return (100.0 - differenceLevel) / 100.0;
    }

    private void OnMatchFound(MatchResult result)
    {
        Console.WriteLine($"Coincidencia encontrada: {result.ReferenceFile} en {result.Timestamp:HH:mm:ss.fff}");
    }

    [DllImport("msvcrt.dll", EntryPoint = "memcpy")]
    private static extern void CopyMemory(IntPtr dest, IntPtr src, int length);
}

// Clases de soporte
public class MatchResult
{
    public string ReferenceFile { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Position { get; set; }
    public double Confidence { get; set; }
}
```

## Conceptos Clave

### Duración del Fragmento

La duración del fragmento determina cuántos datos de video se acumulan antes de construir una huella:

```csharp
// Calcular duración óptima del fragmento
// Debe ser al menos 2x el contenido más largo que quieras detectar
var maxContentDuration = 30000; // 30 segundos en milisegundos
var fragmentDuration = ((maxContentDuration + 1000) / 1000 + 1) * 1000 * 2;
// Resultado: fragmentos de 62000ms (62 segundos)
```

### Fragmentos Superpuestos

Usar fragmentos superpuestos mejora la precisión de detección al asegurar que el contenido no se pierda en los límites de fragmentos:

```csharp
// Fragmento principal: 0-60 segundos
// Fragmento superpuesto 1: 30-90 segundos (inicia al 50% del principal)
// Fragmento superpuesto 2: 60-120 segundos
// Esto asegura que cualquier contenido sea capturado completamente en al menos un fragmento
```

### Procesamiento de Fotogramas

Cada fotograma se procesa individualmente y se agrega a los datos de búsqueda del fragmento actual:

```csharp
VFPSearch.Process(
    frameData,      // Puntero a datos del fotograma RGB24
    width,          // Ancho del fotograma
    height,         // Alto del fotograma  
    stride,         // Stride de fila en bytes
    timestamp,      // Marca de tiempo del fotograma
    ref searchData  // Acumulador para este fragmento
);
```

### Construcción de Huellas

Una vez que un fragmento está completo, construye la huella:

```csharp
long dataSize;
IntPtr fingerprintPtr = VFPSearch.Build(out dataSize, ref searchData);

// Copiar a array administrado
var fingerprintData = new byte[dataSize];
Marshal.Copy(fingerprintPtr, fingerprintData, 0, (int)dataSize);
```

## Consideraciones de Rendimiento

### Gestión de Memoria

- Pre-asignar buffers para evitar asignaciones repetidas
- Usar `Marshal.AllocCoTaskMem` para buffers no administrados
- Disponer correctamente de objetos `VFPSearchData` después de usar
- Liberar datos de búsqueda con `searchData.Free()` después de construir

### Estrategia de Procesamiento

- Usar una cola para desacoplar captura de fotogramas del procesamiento de huellas
- Procesar fragmentos en un hilo separado para evitar bloquear la captura
- Ajustar la duración del fragmento basándose en la longitud del contenido de referencia
- Usar fragmentos superpuestos para mejor precisión de detección

### Consejos de Optimización

1. **Duración del Fragmento**: Fragmentos más largos = mejor precisión pero mayor latencia
2. **Porcentaje de Superposición**: 50% de superposición es típico, ajustar según necesidades
3. **Umbral de Diferencia**: Valores más bajos = coincidencia más estricta (escala 0-100)
4. **Tamaño del Buffer**: Pre-asignar basándose en el tamaño máximo esperado del fotograma

## Casos de Uso Comunes

### Monitoreo de Transmisiones en Vivo

Monitorear transmisiones de TV en vivo para contenido con derechos de autor o anuncios:

```csharp
// Inicializar con biblioteca de contenido de transmisión
var processor = new RealTimeFingerprintProcessor();
await processor.LoadReferenceFingerprintsAsync("biblioteca_anuncios/");

// Iniciar procesamiento del stream de transmisión
await processor.StartNetworkStreamAsync("rtsp://servidor-transmision/stream1");
```

### Análisis de Cámaras de Seguridad

Detectar eventos específicos u objetos en feeds de cámaras de seguridad:

```csharp
// Procesar múltiples streams de cámara
var processors = new List<RealTimeFingerprintProcessor>();

foreach (var camera in GetSecurityCameras())
{
    var processor = new RealTimeFingerprintProcessor();
    await processor.StartNetworkStreamAsync(camera.StreamUrl);
    processors.Add(processor);
}
```

### Cumplimiento de Contenido

Asegurar que las plataformas de streaming cumplan con restricciones de contenido:

```csharp
// Monitorear streams generados por usuarios
var processor = new RealTimeFingerprintProcessor();
await processor.LoadReferenceFingerprintsAsync("contenido_prohibido/");

// Procesar stream entrante
await processor.StartNetworkStreamAsync(userStreamUrl);
```

## Solución de Problemas

### No Se Encuentran Coincidencias

- Verificar que la duración del fragmento sea apropiada para la longitud del contenido de referencia
- Verificar que el umbral de diferencia no sea demasiado estricto
- Asegurar que los fragmentos superpuestos estén habilitados
- Verificar que las huellas de referencia se generaron correctamente

### Alto Uso de Memoria

- Reducir la duración del fragmento si es posible
- Procesar y disponer fragmentos más frecuentemente
- Usar menor porcentaje de superposición
- Liberar buffers no administrados correctamente

### Retraso en el Procesamiento

- Usar hilos separados para captura y procesamiento
- Implementar descarte de fotogramas si el procesamiento no puede mantener el ritmo
- Optimizar búsqueda de huellas de referencia (indexar, búsqueda paralela)
- Considerar aceleración GPU si está disponible

## Resumen

Las huellas en tiempo real con el SDK de VisioForge usan un enfoque basado en fragmentos que:
- Acumula fotogramas en fragmentos basados en tiempo usando `VFPSearchData`
- Procesa fotogramas en tiempo real con `VFPSearch.Process()`
- Construye huellas a partir de fragmentos con `VFPSearch.Build()`
- Usa fragmentos superpuestos para detección robusta
- Permite coincidencia de contenido en vivo contra huellas de referencia

Este enfoque está probado en entornos de producción para monitoreo de transmisiones, cumplimiento de contenido y aplicaciones de seguridad.

## Aplicación de Ejemplo Completa

Para un ejemplo completo funcional de huellas de video en tiempo real, consulta la aplicación de ejemplo **MMT Live**:
- [Ejemplo MMT Live en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT%20Live)

Este ejemplo demuestra todos los conceptos cubiertos en esta guía con una aplicación Windows Forms completa para detección de comerciales en vivo.
