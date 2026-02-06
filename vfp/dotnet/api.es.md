---
title: API .NET del SDK de Video Fingerprinting
description: API completa del SDK de Video Fingerprinting de VisioForge para generar, comparar y buscar huellas de video con ejemplos de código.
---

# Documentación de la API .NET del SDK de Video Fingerprinting

## Descripción General

El espacio de nombres VisioForge Video Fingerprinting proporciona una funcionalidad potente para la identificación de contenido de video, comparación y operaciones de búsqueda. Permite a las aplicaciones:

- Generar huellas digitales únicas a partir de archivos de video para la identificación de contenido
- Comparar videos para determinar similitud y detectar duplicados
- Buscar fragmentos de video dentro de videos más grandes (por ejemplo, encontrar comerciales, introducciones o escenas específicas)
- Comparar imágenes individuales para la detección de similitud (solo Windows)
- Procesar cuadros de video directamente para generar huellas digitales a partir de transmisiones o contenido generado

## Tabla de Contenidos

- [Clase VFPAnalyzer](#clase-vfpanalyzer)
- [Clase VFPFingerPrint](#clase-vfpfingerprint)
- [Clase VFPFingerprintSource](#clase-vfpfingerprintsource)
- [Clase VFPCompare](#clase-vfpcompare)
- [Clase VFPSearch](#clase-vfpsearch)
- [Clase VFPFingerPrintDB](#clase-vfpfingerprintdb)
- [Clase VFPFingerprintFromFrames](#clase-vfpfingerprintfromframes)
- [Tipos de Soporte](#tipos-de-soporte)
- [Delegados](#delegados)

## Clase VFPAnalyzer

El punto de entrada principal para las operaciones de huellas digitales de video, proporcionando métodos estáticos de alto nivel para análisis, comparación y búsqueda.

### Propiedades

#### DebugDir

```csharp
public static string DebugDir { get; set; }
```

**Descripción:** Ruta del directorio para la salida de depuración. Cuando se establece, los resultados intermedios del procesamiento pueden guardarse para la resolución de problemas.

**Predeterminado:** `null` (salida de depuración deshabilitada)

**Ejemplo:**

```csharp
// Habilitar salida de depuración
VFPAnalyzer.DebugDir = @"C:\Temp\VFP_Debug";

// Deshabilitar salida de depuración
VFPAnalyzer.DebugDir = null;
```

### Métodos

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string vfpLicense)
```

**Descripción:** Establece la clave de licencia para el SDK de Video Fingerprinting. Debe llamarse antes de usar cualquier función de huellas digitales.

**Parámetros:**

- `vfpLicense` (string): Su clave de licencia de VisioForge

**Ejemplo:**

```csharp
// Establecer clave de licencia al inicio de la aplicación
VFPAnalyzer.SetLicenseKey("YOUR-LICENSE-KEY-HERE");
```

#### GetComparingFingerprintForVideoFileAsync

```csharp
public static async Task<VFPFingerPrint> GetComparingFingerprintForVideoFileAsync(
    VFPFingerprintSource source,
    VFPErrorCallback errorDelegate = null,
    VFPProgressCallback progressDelegate = null)
```

**Descripción:** Genera una huella digital optimizada para operaciones de comparación de video completo.

**Parámetros:**

- `source` (VFPFingerprintSource): Configuración de la fuente de video que incluye ruta de archivo, rango de tiempo y opciones de procesamiento
- `errorDelegate` (VFPErrorCallback): Callback opcional para mensajes de error
- `progressDelegate` (VFPProgressCallback): Callback opcional para actualizaciones de progreso (0-100)

**Devuelve:** `Task<VFPFingerPrint>` - Huella digital generada o `null` si ocurrió un error

**Caso de Uso:** Comparar videos completos o segmentos grandes para determinar la similitud general

**Ejemplo:**

```csharp
// Uso básico
var source = new VFPFingerprintSource(@"C:\Videos\movie.mp4");
var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);

if (fingerprint != null)
{
    fingerprint.Save(@"C:\Fingerprints\movie.vsigx");
    Console.WriteLine($"Fingerprint created for {fingerprint.Duration} duration");
}

// Con manejo de errores y reporte de progreso
var source2 = new VFPFingerprintSource(@"C:\Videos\video.mp4")
{
    StartTime = TimeSpan.FromMinutes(5),
    StopTime = TimeSpan.FromMinutes(10)
};

var fingerprint2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
    source2,
    error => Console.WriteLine($"Error: {error}"),
    progress => Console.WriteLine($"Progress: {progress}%"));

// Con áreas ignoradas (por ejemplo, logotipos, marcas de agua)
var source3 = new VFPFingerprintSource(@"C:\Videos\broadcast.mp4");
source3.IgnoredAreas.Add(new Rect(1700, 50, 1870, 150)); // Logotipo superior derecho
source3.IgnoredAreas.Add(new Rect(100, 950, 300, 1000)); // Marca de tiempo inferior

var fingerprint3 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source3);
```

#### GetSearchFingerprintForVideoFileAsync

```csharp
public static async Task<VFPFingerPrint> GetSearchFingerprintForVideoFileAsync(
    VFPFingerprintSource source,
    VFPErrorCallback errorDelegate = null,
    VFPProgressCallback progressDelegate = null)
```

**Descripción:** Genera una huella digital optimizada para operaciones de búsqueda de fragmentos.

**Parámetros:**

- `source` (VFPFingerprintSource): Configuración de la fuente de video
- `errorDelegate` (VFPErrorCallback): Callback de error opcional
- `progressDelegate` (VFPProgressCallback): Callback de progreso opcional

**Devuelve:** `Task<VFPFingerPrint>` - Huella digital generada o `null` si ocurrió un error

**Caso de Uso:** Crear huellas digitales de clips cortos para localizar dentro de videos de larga duración

**Ejemplo:**

```csharp
// Crear huella digital para un comercial
var commercialSource = new VFPFingerprintSource(@"C:\Videos\commercial.mp4");
var commercialFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    commercialSource,
    error => Console.WriteLine($"Error: {error}"),
    progress => Console.WriteLine($"Processing: {progress}%"));

// Crear huella digital para una escena específica
var sceneSource = new VFPFingerprintSource(@"C:\Videos\movie.mp4")
{
    StartTime = TimeSpan.FromMinutes(42),
    StopTime = TimeSpan.FromMinutes(43)
};

var sceneFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(sceneSource);

if (sceneFp != null)
{
    sceneFp.Tag = "Action scene at bridge";
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

**Descripción:** Compara dos huellas digitales de video para determinar la similitud.

**Parámetros:**

- `fp1` (VFPFingerPrint): Primera huella digital
- `fp2` (VFPFingerPrint): Segunda huella digital
- `shift` (TimeSpan): Desplazamiento de tiempo máximo permitido durante la comparación

**Devuelve:** `int` - Puntuación de diferencia (menor = más similar), o `Int32.MaxValue` si alguna huella digital es nula

**Ejemplo:**

```csharp
// Cargar dos huellas digitales
var fp1 = VFPFingerPrint.Load(@"C:\Fingerprints\video1.vsigx");
var fp2 = VFPFingerPrint.Load(@"C:\Fingerprints\video2.vsigx");

// Comparar con tolerancia de desplazamiento de 5 segundos
int difference = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromSeconds(5));

// Interpretar resultados
if (difference < 5)
{
    Console.WriteLine("Videos are nearly identical");
}
else if (difference < 15)
{
    Console.WriteLine("Videos are very similar");
}
else if (difference < 30)
{
    Console.WriteLine("Videos have similar content with differences");
}
else if (difference < 100)
{
    Console.WriteLine("Videos are related but significantly different");
}
else
{
    Console.WriteLine("Videos are completely different");
}

// Comparación por lotes
var fingerprints = new List<VFPFingerPrint>();
foreach (var file in Directory.GetFiles(@"C:\Fingerprints", "*.vsigx"))
{
    fingerprints.Add(VFPFingerPrint.Load(file));
}

var referenceFp = fingerprints[0];
foreach (var fp in fingerprints.Skip(1))
{
    int diff = VFPAnalyzer.Compare(referenceFp, fp, TimeSpan.FromSeconds(3));
    Console.WriteLine($"{fp.OriginalFilename}: Difference = {diff}");
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

**Descripción:** Busca ocurrencias de un fragmento de video dentro de un video más grande.

**Parámetros:**

- `fp1` (VFPFingerPrint): Huella digital del fragmento (aguja)
- `fp2` (VFPFingerPrint): Video en el que buscar (pajar)
- `duration` (TimeSpan): Duración del fragmento (evita coincidencias superpuestas)
- `maxDifference` (int): Diferencia máxima permitida (típico: 5-20)
- `allowMultipleFragments` (bool): Encontrar todas las ocurrencias vs. solo la primera coincidencia

**Devuelve:** `List<TimeSpan>` - Marcas de tiempo donde se encontraron coincidencias

**Ejemplo:**

```csharp
// Buscar un comercial en una grabación
var commercialFp = VFPFingerPrint.Load(@"C:\Fingerprints\commercial.vsigx");
var recordingFp = VFPFingerPrint.Load(@"C:\Fingerprints\tv_recording.vsigx");

// Encontrar todas las ocurrencias
var matches = await VFPAnalyzer.SearchAsync(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30), // Duración del comercial
    maxDifference: 10,
    allowMultipleFragments: true);

foreach (var timestamp in matches)
{
    Console.WriteLine($"Commercial found at: {timestamp:hh\\:mm\\:ss}");
}

// Encontrar solo la primera ocurrencia
var firstMatch = VFPAnalyzer.Search(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30),
    maxDifference: 15,
    allowMultipleFragments: false);

if (firstMatch.Any())
{
    Console.WriteLine($"First occurrence at: {firstMatch[0]}");
}

// Buscar con coincidencia más estricta
var exactMatches = VFPAnalyzer.Search(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30),
    maxDifference: 5, // Muy estricto
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

**Descripción:** Método de conveniencia que genera huellas digitales y compara dos archivos de video en una sola operación.

**Parámetros:**

- `file1` (VFPFingerprintSource): Configuración del primer video
- `file2` (VFPFingerprintSource): Configuración del segundo video
- `shift` (TimeSpan): Desplazamiento de tiempo máximo permitido
- `errorCallback` (VFPErrorCallback): Callback de error
- `threshold` (int): Diferencia máxima para considerar como coincidencia (predeterminado: 500)

**Devuelve:** `Task<bool>` - `true` si los videos coinciden (diferencia < umbral), de lo contrario `false`

**Ejemplo:**

```csharp
// Comparar dos archivos de video directamente
var file1 = new VFPFingerprintSource(@"C:\Videos\original.mp4");
var file2 = new VFPFingerprintSource(@"C:\Videos\copy.mp4");

bool areIdentical = await VFPAnalyzer.CompareVideoFilesAsync(
    file1,
    file2,
    TimeSpan.FromSeconds(5),
    error => Console.WriteLine($"Error: {error}"),
    threshold: 20);

if (areIdentical)
{
    Console.WriteLine("Videos are identical or very similar");
}

// Comparar con procesamiento personalizado
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

## Clase VFPFingerPrint

Representa una huella digital de video con metadatos y soporte de serialización.

### Propiedades

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

### Métodos

#### Load (Estático)

```csharp
public static VFPFingerPrint Load(string filename)
public static VFPFingerPrint Load(byte[] data)
```

**Descripción:** Carga una huella digital desde archivo o memoria.

**Parámetros:**

- `filename` (string): Ruta al archivo de huella digital
- `data` (byte[]): Datos de huella digital en memoria

**Devuelve:** `VFPFingerPrint` - Objeto de huella digital cargado

**Ejemplo:**

```csharp
// Cargar desde archivo
var fingerprint = VFPFingerPrint.Load(@"C:\Fingerprints\video.vsigx");
Console.WriteLine($"Loaded: {fingerprint.OriginalFilename}");
Console.WriteLine($"Duration: {fingerprint.Duration}");
Console.WriteLine($"ID: {fingerprint.ID}");

// Cargar desde memoria
byte[] fpData = File.ReadAllBytes(@"C:\Fingerprints\video.vsigx");
var fingerprint2 = VFPFingerPrint.Load(fpData);

// Cargar múltiples huellas digitales
var fingerprints = new List<VFPFingerPrint>();
foreach (var file in Directory.GetFiles(@"C:\Fingerprints", "*.vsigx"))
{
    try
    {
        var fp = VFPFingerPrint.Load(file);
        fingerprints.Add(fp);
        Console.WriteLine($"Loaded: {fp.OriginalFilename} ({fp.Duration})");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to load {file}: {ex.Message}");
    }
}
```

#### Save

```csharp
public void Save(string filename)
public byte[] Save()
```

**Descripción:** Guarda la huella digital en archivo o memoria. Extensión predeterminada: `.vsigx`

**Parámetros:**

- `filename` (string): Ruta del archivo de salida

**Devuelve:** `byte[]` - Datos de huella digital serializados (versión de memoria)

**Ejemplo:**

```csharp
// Guardar en archivo
fingerprint.Save(@"C:\Fingerprints\output.vsigx");

// Guardar en memoria
byte[] data = fingerprint.Save();
File.WriteAllBytes(@"C:\Backup\fingerprint.vsigx", data);

// Guardar con metadatos
fingerprint.Tag = "Important scene at 00:42:00";
fingerprint.Save(@"C:\Fingerprints\tagged.vsigx");

// Guardado por lotes con nombres organizados
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

## Clase VFPFingerprintSource

Configuración para operaciones de huellas digitales de video.

### Constructor

```csharp
public VFPFingerprintSource(string filename)
```

**Descripción:** Crea una nueva configuración de fuente.

**Parámetros:**

- `filename` (string): Ruta al archivo de video

**Lanza:** `FileNotFoundException` si el archivo no existe

### Propiedades

```csharp
public string Filename { get; }
public TimeSpan StartTime { get; set; }
public TimeSpan StopTime { get; set; }
public Rect CustomCropSize { get; set; }
public Size CustomResolution { get; set; }
public List<Rect> IgnoredAreas { get; }
public TimeSpan OriginalDuration { get; set; }
```

- **`Filename`** (`string`): Ruta al archivo de video fuente
- **`StartTime`** (`TimeSpan`): Hora de inicio para la huella digital (predeterminado: 0)
- **`StopTime`** (`TimeSpan`): Hora de finalización para la huella digital (predeterminado: duración del video)
- **`CustomCropSize`** (`Rect`): Rectángulo de recorte (distancias Izquierda, Arriba, Derecha, Abajo)
- **`CustomResolution`** (`Size`): Resolución objetivo (Vacío = sin redimensionar)
- **`IgnoredAreas`** (`List<Rect>`): Regiones a excluir (por ejemplo, logotipos, marcas de tiempo)
- **`OriginalDuration`** (`TimeSpan`): Duración total del video (poblado automáticamente)

### Ejemplos

```csharp
// Configuración básica
var source = new VFPFingerprintSource(@"C:\Videos\movie.mp4");

// Procesar rango de tiempo específico
var source2 = new VFPFingerprintSource(@"C:\Videos\long_video.mp4")
{
    StartTime = TimeSpan.FromMinutes(10),
    StopTime = TimeSpan.FromMinutes(20)
};

// Recortar a región de interés
var source3 = new VFPFingerprintSource(@"C:\Videos\video.mp4")
{
    CustomCropSize = new Rect(100, 100, 1820, 980) // Eliminar bordes
};

// Redimensionar para procesamiento más rápido
var source4 = new VFPFingerprintSource(@"C:\Videos\4k_video.mp4")
{
    CustomResolution = new Size(1280, 720) // Escalar desde 4K
};

// Ignorar superposiciones y logotipos
var source5 = new VFPFingerprintSource(@"C:\Videos\broadcast.mp4");
source5.IgnoredAreas.Add(new Rect(1700, 50, 1870, 150)); // Logotipo del canal
source5.IgnoredAreas.Add(new Rect(50, 50, 250, 100)); // Error de red
source5.IgnoredAreas.Add(new Rect(100, 950, 400, 1000)); // Ticker

// Opciones de procesamiento combinadas
var source6 = new VFPFingerprintSource(@"C:\Videos\tv_show.mp4")
{
    StartTime = TimeSpan.FromSeconds(90), // Saltar introducción
    StopTime = TimeSpan.FromMinutes(42), // Antes de los créditos
    CustomResolution = new Size(640, 480),
    CustomCropSize = new Rect(60, 0, 1860, 1080) // Eliminar pillarboxing
};
source6.IgnoredAreas.Add(new Rect(1600, 100, 1800, 200));
```

## Clase VFPCompare

Funcionalidad de comparación de huellas digitales de bajo nivel.

### Métodos

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string licenseKey)
```

**Descripción:** Establece la clave de licencia del SDK.

**Ejemplo:**

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

**Descripción:** Procesa el cuadro RGB24 para la huella digital de comparación.

**Parámetros:**

- `ptr` (IntPtr): Puntero a los datos del cuadro RGB24
- `w` (int): Ancho del cuadro
- `h` (int): Altura del cuadro
- `s` (int): Stride (bytes por fila)
- `dTime` (TimeSpan): Marca de tiempo del cuadro
- `data` (ref VFPCompareData): Estructura de datos de comparación

**Devuelve:** `int` - Código de estado (0 = éxito)

**Ejemplo:**

```csharp
// Inicializar datos de comparación
var compareData = new VFPCompareData(durationInSeconds: 120);

// Procesar cuadros (generalmente hecho internamente por VFPAnalyzer)
IntPtr frameData = GetRGB24Frame(); // Su fuente de cuadros
int result = VFPCompare.Process(
    frameData,
    1920, // ancho
    1080, // altura
    5760, // stride para 1920x3 RGB24
    TimeSpan.FromSeconds(1.5),
    ref compareData);

if (result == 0)
{
    Console.WriteLine("Frame processed successfully");
}

// Construir huella digital después de procesar todos los cuadros
IntPtr fpData = VFPCompare.Build(out long length, ref compareData);

// Limpiar
compareData.Free();
```

#### Build

```csharp
public static IntPtr Build(
    out long length,
    ref VFPCompareData video)
```

**Descripción:** Construye la huella digital a partir de los cuadros procesados.

**Parámetros:**

- `length` (out long): Tamaño de los datos de la huella digital
- `video` (ref VFPCompareData): Datos de cuadros procesados

**Devuelve:** `IntPtr` - Puntero a los datos de la huella digital

#### Compare

```csharp
public static double Compare(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    int maxDifference)
```

**Descripción:** Compara dos huellas digitales.

**Parámetros:**

- `fp1` (VFPFingerPrint): Primera huella digital
- `fp2` (VFPFingerPrint): Segunda huella digital
- `maxDifference` (int): Diferencia máxima permitida

**Devuelve:** `double` - Puntuación de similitud (0-100, mayor = más similar)

**Ejemplo:**

```csharp
var fp1 = VFPFingerPrint.Load(@"C:\Fingerprints\video1.vsigx");
var fp2 = VFPFingerPrint.Load(@"C:\Fingerprints\video2.vsigx");

double similarity = VFPCompare.Compare(fp1, fp2, maxDifference: 50);
Console.WriteLine($"Similarity: {similarity:F2}%");

if (similarity > 90)
{
    Console.WriteLine("Videos are very similar");
}
else if (similarity > 70)
{
    Console.WriteLine("Videos have significant similarities");
}
else
{
    Console.WriteLine("Videos are different");
}
```

## Clase VFPSearch

Funcionalidad de búsqueda de huellas digitales de bajo nivel.

### Métodos

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string licenseKey)
```

**Descripción:** Establece la clave de licencia del SDK.

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

**Descripción:** Procesa el cuadro de video para la huella digital de búsqueda.

**Parámetros:**

- `ptr` (IntPtr): Puntero de datos del cuadro RGB24
- `w` (int): Ancho del cuadro
- `h` (int): Altura del cuadro
- `s` (int): Stride
- `dTime` (TimeSpan): Marca de tiempo del cuadro
- `data` (ref VFPSearchData): Estructura de datos de búsqueda

**Devuelve:** `int` - Código de estado

#### Build

```csharp
public static IntPtr Build(
    out long length,
    ref VFPSearchData data)
```

**Descripción:** Construye la huella digital de búsqueda.

**Parámetros:**

- `length` (out long): Tamaño de datos de la huella digital
- `data` (ref VFPSearchData): Cuadros procesados

**Devuelve:** `IntPtr` - Puntero de datos de la huella digital

#### Search

```csharp
public static int Search(
    VFPFingerPrint fp1,
    int startPos1,
    VFPFingerPrint fp2,
    int startPos2,
    out int difference,
    int maxDifference)
```

**Descripción:** Busca fragmento en video.

**Parámetros:**

- `fp1` (VFPFingerPrint): Huella digital del fragmento
- `startPos1` (int): Posición de inicio en el fragmento (segundos)
- `fp2` (VFPFingerPrint): Huella digital del video principal
- `startPos2` (int): Posición de inicio de búsqueda (segundos)
- `difference` (out int): Puntuación de diferencia de coincidencia
- `maxDifference` (int): Diferencia máxima permitida

**Devuelve:** `int` - Posición donde se encontró (segundos) o Int32.MaxValue si no se encontró

**Ejemplo:**

```csharp
// Buscar fragmento
var fragmentFp = VFPFingerPrint.Load(@"C:\Fingerprints\fragment.vsigx");
var videoFp = VFPFingerPrint.Load(@"C:\Fingerprints\full_video.vsigx");

int position = VFPSearch.Search(
    fragmentFp,
    startPos1: 0,
    videoFp,
    startPos2: 0,
    out int matchDifference,
    maxDifference: 20);

if (position != Int32.MaxValue)
{
    Console.WriteLine($"Fragment found at {position} seconds");
    Console.WriteLine($"Match quality: {matchDifference}");
}
else
{
    Console.WriteLine("Fragment not found");
}

// Buscar desde una posición específica
int nextPosition = VFPSearch.Search(
    fragmentFp,
    0,
    videoFp,
    position + 30, // Saltar más allá de la primera coincidencia
    out matchDifference,
    maxDifference: 20);
```

## Clase VFPFingerPrintDB

Base de datos para gestionar colecciones de huellas digitales.

### Propiedades

```csharp
public List<VFPFingerPrint> Items { get; }
```

### Métodos

#### Save

```csharp
public void Save(string filename)
```

**Descripción:** Guarda la base de datos en un archivo.

**Ejemplo:**

```csharp
var db = new VFPFingerPrintDB();

// Agregar huellas digitales
foreach (var videoFile in Directory.GetFiles(@"C:\Videos", "*.mp4"))
{
    var source = new VFPFingerprintSource(videoFile);
    var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    if (fp != null)
    {
        db.Items.Add(fp);
    }
}

// Guardar base de datos
db.Save(@"C:\Database\fingerprints.db");
Console.WriteLine($"Saved {db.Items.Count} fingerprints to database");
```

#### Load (Estático)

```csharp
public static VFPFingerPrintDB Load(string filename)
```

**Descripción:** Carga la base de datos desde un archivo.

**Ejemplo:**

```csharp
// Cargar base de datos existente
var db = VFPFingerPrintDB.Load(@"C:\Database\fingerprints.db");
Console.WriteLine($"Loaded {db.Items.Count} fingerprints");

// Consultar base de datos
var recentVideos = db.Items
    .Where(fp => fp.OriginalDuration > TimeSpan.FromMinutes(30))
    .OrderBy(fp => fp.OriginalFilename)
    .ToList();

foreach (var fp in recentVideos)
{
    Console.WriteLine($"{fp.OriginalFilename}: {fp.Duration}");
}
```

#### ContainsFile

```csharp
public bool ContainsFile(VFPFingerprintSource source)
```

**Descripción:** Comprueba si la base de datos contiene una huella digital para la fuente.

**Ejemplo:**

```csharp
var source = new VFPFingerprintSource(@"C:\Videos\new_video.mp4");

if (!db.ContainsFile(source))
{
    // Generar y agregar nueva huella digital
    var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    db.Items.Add(fp);
    db.Save(@"C:\Database\fingerprints.db");
}
else
{
    Console.WriteLine("Fingerprint already exists in database");
}
```

#### GetFingerprint

```csharp
public VFPFingerPrint GetFingerprint(VFPFingerprintSource source)
```

**Descripción:** Recupera la huella digital que coincide con la fuente.

**Ejemplo:**

```csharp
var source = new VFPFingerprintSource(@"C:\Videos\video.mp4");
var fp = db.GetFingerprint(source);

if (fp != null)
{
    Console.WriteLine($"Found fingerprint: {fp.ID}");
    Console.WriteLine($"Duration: {fp.Duration}");
    Console.WriteLine($"Tag: {fp.Tag}");
}
```

## Clase VFPFingerprintFromFrames

Crea huellas digitales a partir de cuadros de imagen individuales. Disponible en todas las plataformas.

### Constructor

```csharp
public VFPFingerprintFromFrames(
    double frameRate,
    int width,
    int height,
    TimeSpan totalDuration)
```

**Descripción:** Inicializa el constructor de huellas digitales basado en cuadros.

**Parámetros:**

- `frameRate` (double): Tasa de cuadros de video
- `width` (int): Ancho del cuadro
- `height` (int): Altura del cuadro
- `totalDuration` (TimeSpan): Duración total del video

### Métodos

#### Push

```csharp
public void Push(byte[] rgb24frame)           // All platforms
public void Push(Bitmap frame)                // Windows only
public void Push(SKBitmap frame)              // All platforms (new)
public void Push(IntPtr rgb24frame, int rgb24frameSize)  // All platforms
```

**Descripción:** Agrega cuadros al proceso de generación de huellas digitales. Los cuadros deben coincidir con las dimensiones configuradas.

- **byte[]**: Datos de cuadro RGB24 sin procesar (multiplataforma)
- **Bitmap**: System.Drawing.Bitmap (solo Windows)
- **SKBitmap**: Mapa de bits SkiaSharp para soporte multiplataforma
- **IntPtr**: Puntero a datos de cuadro RGB24 (multiplataforma)

**Ejemplo:**

```csharp
// Crear constructor
var builder = new VFPFingerprintFromFrames(
    frameRate: 30.0,
    width: 1920,
    height: 1080,
    totalDuration: TimeSpan.FromMinutes(5));

// Multiplataforma: Agregar cuadros como matrices de bytes
foreach (var frameData in videoStream.GetFrames())
{
    builder.Push(frameData); // byte[] RGB24
}

// Multiplataforma: Agregar mapas de bits SkiaSharp
using (var skBitmap = SKBitmap.Decode(imageData))
{
    builder.Push(skBitmap);
}

// Solo Windows: Agregar cuadros System.Drawing.Bitmap
#if NET_WINDOWS
for (int i = 0; i < frameCount; i++)
{
    Bitmap frame = GetFrameAsBitmap(i);
    builder.Push(frame);
}
#endif

// Multiplataforma: Agregar cuadros vía IntPtr
unsafe
{
    fixed (byte* ptr = frameData)
    {
        builder.Push(new IntPtr(ptr), frameData.Length);
    }
}

// Construir huella digital final
var fingerprint = builder.Build();
fingerprint.OriginalFilename = "stream_capture.mp4";
fingerprint.Save(@"C:\Fingerprints\stream.vsigx");
```

#### Build

```csharp
public VFPFingerPrint Build()
```

**Descripción:** Genera huella digital a partir de cuadros procesados.

**Devuelve:** `VFPFingerPrint` - Huella digital generada

## Tipos de Soporte

### VFPCompareData

```csharp
public struct VFPCompareData
{
    public IntPtr Data { get; set; }
    public VFPCompareData(int duration)
    public void Free()
}
```

**Descripción:** Gestiona datos de comparación nativos.

**Ejemplo:**

```csharp
// Crear y usar datos de comparación
var data = new VFPCompareData(durationInSeconds: 60);
try
{
    // Process frames...
    // Build fingerprint...
}
finally
{
    data.Free(); // Siempre liberar memoria nativa
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

**Descripción:** Gestiona datos de búsqueda nativos con eliminación automática.

**Ejemplo:**

```csharp
// La declaración using asegura una limpieza adecuada
using (var searchData = new VFPSearchData(TimeSpan.FromMinutes(2)))
{
    // Process frames for search fingerprint
    // Build fingerprint
} // Automatically disposed
```

## Delegados

### VFPProgressCallback

```csharp
public delegate void VFPProgressCallback(int percent)
```

**Descripción:** Informa el progreso durante las operaciones de huellas digitales (0-100).

**Ejemplo:**

```csharp
// Visualización de progreso simple
VFPProgressCallback progressCallback = (percent) =>
{
    Console.Write($"\rProgress: {percent}%");
    if (percent == 100) Console.WriteLine();
};

// Progreso con actualización de UI
VFPProgressCallback uiProgress = (percent) =>
{
    progressBar.Value = percent;
    labelStatus.Text = $"Processing: {percent}%";
    Application.DoEvents();
};

// Progreso con verificación de cancelación
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

**Descripción:** Informa errores durante las operaciones de huellas digitales.

**Ejemplo:**

```csharp
// Registrar errores
VFPErrorCallback errorCallback = (error) =>
{
    logger.Error($"VFP Error: {error}");
    File.AppendAllText(@"C:\Logs\vfp_errors.log", 
        $"{DateTime.Now}: {error}{Environment.NewLine}");
};

// Mostrar errores al usuario
VFPErrorCallback userErrorCallback = (error) =>
{
    MessageBox.Show($"Error processing video: {error}", 
        "Fingerprinting Error", 
        MessageBoxButtons.OK, 
        MessageBoxIcon.Error);
};

// Recopilar errores para procesamiento por lotes
var errors = new List<string>();
VFPErrorCallback collectErrors = (error) => errors.Add(error);
```
