---
title: Primeros Pasos con Video Fingerprinting SDK .NET
description: Guía completa de instalación y configuración para VisioForge Video Fingerprinting SDK con configuración, licencias e instrucciones paso a paso.
---

# Primeros Pasos con Video Fingerprinting SDK

¡Bienvenido al VisioForge Video Fingerprinting SDK! Esta guía completa te llevará a través de todo lo que necesitas para comenzar, desde la instalación hasta tu primera aplicación funcional. Al final de esta guía, tendrás una base sólida para construir aplicaciones de huella digital de video.

## Resumen de Inicio Rápido

Si buscas ponerte en marcha rápidamente:

1. Instala el SDK vía NuGet: `Install-Package VisioForge.DotNet.Core`
2. Agrega el paquete de redistribución: `Install-Package VisioForge.DotNet.Core.Redist.VideoFingerprinting`
   - Este paquete único soporta Windows (x86/x64), Linux (x64/ARM64) y macOS
3. Establece tu clave de licencia: `VFPAnalyzer.SetLicenseKey("TRIAL");`
4. Genera tu primera huella digital usando los ejemplos a continuación

## Prerrequisitos y Requisitos del Sistema

Para requisitos detallados del sistema incluyendo plataformas soportadas, especificaciones de hardware y consideraciones de rendimiento, por favor consulta nuestra guía completa de [Requisitos del Sistema](../system-requirements.md).

### Requisitos Específicos de .NET

- **Versión de .NET**: 
  - Windows: .NET Framework 4.6.1+ o .NET 6.0+
  - Linux/macOS: .NET 6.0+
- **IDE**: Visual Studio 2019+ (Windows), Visual Studio Code, o JetBrains Rider
- **Administrador de Paquetes NuGet**: Para instalación y actualizaciones fáciles

## Métodos de Instalación

### Método 1: Administrador de Paquetes NuGet (Recomendado)

La forma más fácil de instalar el SDK es a través del Administrador de Paquetes NuGet en Visual Studio.

#### Vía Interfaz de Usuario del Administrador de Paquetes

1. Haz clic derecho en tu proyecto en el Explorador de Soluciones
2. Selecciona "Administrar Paquetes NuGet"
3. Haz clic en "Examinar" y busca "VisioForge.DotNet.Core"
4. Selecciona el paquete y haz clic en "Instalar"
5. Acepta el acuerdo de licencia

#### Vía Consola del Administrador de Paquetes

```powershell
# Instala el paquete principal del SDK
Install-Package VisioForge.DotNet.Core

# Instala el paquete de redistribución con bibliotecas nativas (requerido)
# Este paquete incluye soporte para Windows (x86/x64), Linux (x64/arm64) y macOS
Install-Package VisioForge.DotNet.Core.Redist.VideoFingerprinting

# Para integración con MongoDB (opcional)
Install-Package VisioForge.DotNet.VideoFingerprinting.MongoDB
```

#### Vía CLI de .NET

```bash
# Agrega el paquete principal del SDK
dotnet add package VisioForge.DotNet.Core

# Agrega el paquete de redistribución con bibliotecas nativas (requerido)
# Este paquete incluye soporte para Windows (x86/x64), Linux (x64/arm64) y macOS
dotnet add package VisioForge.DotNet.Core.Redist.VideoFingerprinting

# Para integración con MongoDB (opcional)
dotnet add package VisioForge.DotNet.VideoFingerprinting.MongoDB

# Restaura paquetes
dotnet restore
```

#### Vía PackageReference (en .csproj)

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core" Version="2025.8.7" />
  <PackageReference Include="VisioForge.DotNet.Core.Redist.VideoFingerprinting" Version="2025.8.7" />
  
  <!-- Opcional: Integración con MongoDB -->
  <PackageReference Include="VisioForge.DotNet.VideoFingerprinting.MongoDB" Version="2025.8.7" />
</ItemGroup>
```

!!! important "Requisitos de Paquetes NuGet"

    El SDK requiere dos paquetes:

    1. **VisioForge.DotNet.Core** - La biblioteca principal del SDK con API de C#
    2. **VisioForge.DotNet.Core.Redist.VideoFingerprinting** - Bibliotecas nativas para funcionalidad de huella digital de video (soporta Windows x86/x64, Linux x64/arm64 y macOS)

    Ambos paquetes deben instalarse para que el SDK funcione correctamente. El paquete de redistribución contiene bibliotecas nativas específicas de plataforma que se despliegan automáticamente a tu directorio de salida.

    **Paquetes opcionales:**

    - **VisioForge.DotNet.VideoFingerprinting.MongoDB** - Integración con MongoDB para almacenar huellas digitales en una base de datos

    **Soporte de Plataforma:**
    El paquete `VisioForge.DotNet.Core.Redist.VideoFingerprinting` incluye bibliotecas nativas para:

    - Windows (x86 y x64)
    - Linux (x64 y ARM64)
    - macOS (Intel y Apple Silicon)

    Las bibliotecas específicas de plataforma correctas se seleccionan y despliegan automáticamente basadas en tu tiempo de ejecución objetivo.

### Método 2: Instalación Manual

Para entornos donde NuGet no está disponible o para escenarios de despliegue personalizados:

1. **Descarga el SDK**
   - Visita la [página del producto](https://www.visioforge.com/video-fingerprinting-sdk)
   - Elige tu plataforma y arquitectura

2. **Ejecuta el instalador**

### Paquetes Adicionales Específicos de Plataforma

Mientras que el paquete `VisioForge.DotNet.Core.Redist.VideoFingerprinting` incluye todas las bibliotecas nativas necesarias para huella digital de video, puedes necesitar paquetes adicionales para funcionalidad extendida:

#### Para Aplicaciones Windows

```powershell
# Paquetes adicionales específicos de Windows (opcional, basado en tus necesidades)
Install-Package VisioForge.DotNet.Core.Redist.Base.x64  # Soporte extendido de Windows x64
Install-Package VisioForge.DotNet.Core.Redist.Base.x86  # Soporte extendido de Windows x86
```

#### Para Aplicaciones Móviles

```powershell
# Soporte de UI para iOS/macOS/tvOS
Install-Package VisioForge.DotNet.Core.UI.Apple

# Soporte de UI para Android
Install-Package VisioForge.DotNet.Core.UI.Android
```

### Configuración Específica de Plataforma

#### Configuración de Windows

No requerida.

#### Configuración de Linux

1. **Instala Dependencias de GStreamer**

   ```bash
   # Ubuntu/Debian
   sudo apt-get update
   sudo apt-get install -y \
     gstreamer1.0-plugins-base \
     gstreamer1.0-plugins-good \
     gstreamer1.0-plugins-bad \
     gstreamer1.0-plugins-ugly \
     gstreamer1.0-libav
   
   # RHEL/CentOS
   sudo yum install -y \
     gstreamer1.0-plugins-base \
     gstreamer1.0-plugins-good \
     gstreamer1.0-plugins-bad \
     gstreamer1.0-plugins-ugly \
     gstreamer1.0-libav
   ```

#### Configuración de macOS

No requerida.

## Activación de Clave de Licencia

### Obteniendo una Clave de Licencia

1. **Licencia de Prueba**
   - Usa cadena vacía para evaluación
   - Funcionalidad completa con marca de agua
   - Período de evaluación de 30 días

2. **Licencia Comercial**
   - Compra desde [Página del Producto](https://www.visioforge.com/video-fingerprinting-sdk)
   - Recibe clave de licencia vía email
   - Usa clave de licencia en tu aplicación

### Activando Tu Licencia

```csharp
using VisioForge.Core.VideoFingerPrinting;

// Al inicio de la aplicación
public static void InitializeSDK()
{
    // Para evaluación de prueba - no hagas nada
    
    // Para licencia comercial
    VFPAnalyzer.SetLicenseKey("YOUR-LICENSE-KEY-HERE");    
}
```

### Validación de Licencia

```csharp
// Verifica el estado de la licencia
public static bool ValidateLicense()
{
    try
    {
        // Intenta una operación simple para verificar la licencia
        var testSource = new VFPFingerprintSource("test.mp4");
        testSource.StopTime = TimeSpan.FromSeconds(1);
        
        // Esto fallará si la licencia es inválida
        var fp = VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(testSource).Result;
        
        return fp != null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Validación de licencia fallida: {ex.Message}");
        return false;
    }
}
```

### Tipos de Licencia

| Característica | Prueba | Comercial |
|---------|-------|------------|
| Huella Digital Básica | ✅ | ✅ |
| Comparación de Video | ✅ | ✅ |
| Búsqueda de Fragmento | ✅ | ✅ |
| Soporte de Base de Datos | ✅ | ✅ |
| Soporte Multiplataforma | ✅ | ✅ |
| Marca de Agua | Sí | No |
| Soporte Técnico | Foro | Email/Prioridad |
| Actualizaciones | 30 días | 1 año |

## Tu Primera Generación de Huella Digital

Vamos a crear una aplicación de consola simple que genera una huella digital de video:

### Paso 1: Crear un Nuevo Proyecto

```bash
# Crea una nueva aplicación de consola
dotnet new console -n VideoFingerprintingDemo
cd VideoFingerprintingDemo

# Agrega los paquetes del SDK
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoFingerprinting
```

### Paso 2: Implementación Básica

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types.X.Sources;

namespace VideoFingerprintingDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Inicializa el SDK con tu licencia
            VFPAnalyzer.SetLicenseKey("TRIAL");
            
            // Especifica el archivo de video a procesar
            string videoPath = @"C:\Videos\sample.mp4";
            
            if (!File.Exists(videoPath))
            {
                Console.WriteLine($"Error: Archivo de video no encontrado en {videoPath}");
                return;
            }
            
            try
            {
                // Genera la huella digital
                await GenerateFingerprint(videoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        static async Task GenerateFingerprint(string videoPath)
        {
            Console.WriteLine($"Procesando: {Path.GetFileName(videoPath)}");
            Console.WriteLine("----------------------------------------");
            
            // Crea configuración de fuente
            var source = new VFPFingerprintSource(videoPath);
            
            // Opcional: Procesa solo los primeros 30 segundos para pruebas
            source.StopTime = TimeSpan.FromSeconds(30);
            
            // Opcional: Reduce resolución para procesamiento más rápido
            source.CustomResolution = new VisioForge.Core.Types.Size(640, 480);
            
            // Genera huella digital con seguimiento de progreso
            var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                source,
                errorDelegate: (error) => {
                    Console.WriteLine($"Error: {error}");
                },
                progressDelegate: (progress) => {
                    Console.Write($"\rProgreso: {progress}%");
                }
            );
            
            Console.WriteLine(); // Nueva línea después del progreso
            
            if (fingerprint != null)
            {
                // Muestra información de la huella digital
                Console.WriteLine("\n¡Huella Digital Generada Exitosamente!");
                Console.WriteLine($"  Duración: {fingerprint.Duration}");
                Console.WriteLine($"  Resolución: {fingerprint.Width}x{fingerprint.Height}");
                Console.WriteLine($"  Tasa de Cuadros: {fingerprint.FrameRate:F2} fps");
                Console.WriteLine($"  Tamaño de Datos: {fingerprint.Data?.Length ?? 0} bytes");
                
                // Guarda huella digital en archivo
                string outputPath = Path.ChangeExtension(videoPath, ".vfp");
                fingerprint.Save(outputPath);
                
                Console.WriteLine($"\nHuella digital guardada en: {outputPath}");
                Console.WriteLine($"Tamaño del archivo: {new FileInfo(outputPath).Length / 1024} KB");
            }
            else
            {
                Console.WriteLine("Falló al generar la huella digital.");
            }
        }
    }
}
```

### Paso 3: Ejecutar la Aplicación

```bash
# Construye y ejecuta
dotnet build
dotnet run

# Salida esperada:
# Procesando: sample.mp4
# ----------------------------------------
# Progreso: 100%
# 
# ¡Huella Digital Generada Exitosamente!
#   Duración: 00:00:30
#   Resolución: 1920x1080
#   Tasa de Cuadros: 29.97 fps
#   Tamaño de Datos: 125440 bytes
# 
# Huella digital guardada en: C:\Videos\sample.vfp
# Tamaño del archivo: 122 KB
```

## Ejemplo Básico de Comparación

Ahora vamos a comparar dos videos para determinar su similitud:

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types.X.Sources;

class VideoComparisonDemo
{
    static async Task Main(string[] args)
    {
        VFPAnalyzer.SetLicenseKey("TRIAL");
        
        string video1 = @"C:\Videos\original.mp4";
        string video2 = @"C:\Videos\copy.mp4";
        
        await CompareVideos(video1, video2);
    }
    
    static async Task CompareVideos(string path1, string path2)
    {
        Console.WriteLine("Comparando videos...");
        Console.WriteLine($"Video 1: {Path.GetFileName(path1)}");
        Console.WriteLine($"Video 2: {Path.GetFileName(path2)}");
        Console.WriteLine("----------------------------------------");
        
        // Crea fuentes con límites de tiempo para comparación rápida
        var source1 = new VFPFingerprintSource(path1)
        {
            StopTime = TimeSpan.FromSeconds(30),
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        var source2 = new VFPFingerprintSource(path2)
        {
            StopTime = TimeSpan.FromSeconds(30),
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        // Genera huellas digitales
        Console.Write("Generando huella digital 1...");
        var fp1 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source1);
        Console.WriteLine(" Hecho");
        
        Console.Write("Generando huella digital 2...");
        var fp2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source2);
        Console.WriteLine(" Hecho");
        
        if (fp1 != null && fp2 != null)
        {
            // Compara huellas digitales
            int difference = VFPAnalyzer.Compare(
                fp1, 
                fp2, 
                TimeSpan.FromMilliseconds(500)
            );
            
            Console.WriteLine($"\nResultados de Comparación:");
            Console.WriteLine($"  Puntaje de Diferencia: {difference}");
            
            // Interpreta los resultados
            string interpretation = GetInterpretation(difference);
            Console.WriteLine($"  Interpretación: {interpretation}");
            
            // Proporciona análisis detallado
            if (difference < 100)
            {
                double similarity = Math.Max(0, 100 - (difference / 3.0));
                Console.WriteLine($"  Similitud: {similarity:F1}%");
            }
        }
        else
        {
            Console.WriteLine("Error: Falló al generar una o ambas huellas digitales");
        }
    }
    
    static string GetInterpretation(int difference)
    {
        if (difference < 5)
            return "IDÉNTICOS - Mismo video, posiblemente diferente codificación";
        else if (difference < 15)
            return "CASI IDÉNTICOS - Mismo video con diferencias menores de calidad";
        else if (difference < 30)
            return "MUY SIMILARES - Mismo contenido con modificaciones leves";
        else if (difference < 50)
            return "SIMILARES - Mismo contenido con cambios notables (marca de agua, logo, etc.)";
        else if (difference < 100)
            return "RELACIONADOS - Similitudes significativas, probablemente mismo material fuente";
        else if (difference < 300)
            return "ALGO RELACIONADOS - Algunas escenas o contenido común";
        else
            return "DIFERENTES - Videos completamente diferentes";
    }
}
```

## Errores Comunes y Soluciones

### Problema 1: DllNotFoundException

**Problema**: La aplicación se bloquea con "Unable to load DLL 'VisioForge_VideoFingerprinting'"

**Solución**:

Agrega el paquete NuGet `VisioForge.DotNet.Core.Redist.VideoFingerprinting` a tu proyecto.

### Problema 2: Excepción de Memoria Insuficiente

**Problema**: "System.OutOfMemoryException" al procesar videos grandes

**Soluciones**:

```csharp
// Solución 1: Usa proceso de 64 bits y aumenta memoria
// Agrega a .csproj:
<PropertyGroup>
  <PlatformTarget>x64</PlatformTarget>
  <LargeAddressAware>true</LargeAddressAware>
</PropertyGroup>

// Solución 2: Reduce resolución de video
var source = new VFPFingerprintSource(videoPath)
{
    CustomResolution = new VisioForge.Core.Types.Size(320, 240), // Resolución muy baja
    FrameRate = 5 // Procesa menos cuadros por segundo
};
```

### Problema 3: Velocidad de Procesamiento Lenta

**Problema**: La generación de huella digital toma demasiado tiempo

**Soluciones**:

```csharp
// Solución 1: Usa procesamiento paralelo para múltiples videos
static async Task ProcessMultipleVideos(string[] videoPaths)
{
    var tasks = videoPaths.Select(path => Task.Run(async () =>
    {
        var source = new VFPFingerprintSource(path)
        {
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        return await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    }));
    
    var fingerprints = await Task.WhenAll(tasks);
}

// Solución 3: Cachea huellas digitales
class FingerprintCache
{
    private static Dictionary<string, VFPFingerPrint> cache = new();
    
    public static async Task<VFPFingerPrint> GetOrGenerate(string videoPath)
    {
        string cacheKey = GetCacheKey(videoPath);
        
        if (cache.ContainsKey(cacheKey))
            return cache[cacheKey];
        
        string cachePath = $"{videoPath}.vfp";
        
        if (File.Exists(cachePath))
        {
            var fp = VFPFingerPrint.Load(cachePath);
            cache[cacheKey] = fp;
            return fp;
        }
        
        // Genera nueva huella digital
        var source = new VFPFingerprintSource(videoPath);
        var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
        
        if (fingerprint != null)
        {
            fingerprint.Save(cachePath);
            cache[cacheKey] = fingerprint;
        }
        
        return fingerprint;
    }
    
    private static string GetCacheKey(string path)
    {
        var info = new FileInfo(path);
        return $"{path}_{info.Length}_{info.LastWriteTimeUtc.Ticks}";
    }
}
```

### Problema 4: Resultados de Similitud Incorrectos

**Problema**: Videos que deberían coincidir se muestran como diferentes

**Soluciones**:

```csharp
// Solución 1: Ajusta parámetros de comparación
static int CompareWithTolerance(VFPFingerPrint fp1, VFPFingerPrint fp2)
{
    // Prueba diferentes desplazamientos de tiempo
    int[] shifts = { 100, 500, 1000, 2000 }; // milisegundos
    int minDifference = int.MaxValue;
    
    foreach (int shift in shifts)
    {
        int diff = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromMilliseconds(shift));
        minDifference = Math.Min(minDifference, diff);
    }
    
    return minDifference;
}

// Solución 2: Maneja videos con diferentes relaciones de aspecto
var source = new VFPFingerprintSource(videoFilePath);
{
    // Ignora áreas de letterbox/pillarbox
    source.IgnoredAreas.AddRange(new[]
    {
        new Rect(0, 0, 1920, 140),      // Letterbox superior
        new Rect(0, 940, 1920, 140)     // Letterbox inferior
    });
};
```

## Resumen de Mejores Prácticas

### Cosas que Hacer

- ✅ Siempre establece clave de licencia antes de cualquier operación del SDK
- ✅ Usa bloques try-catch alrededor de llamadas del SDK
- ✅ Procesa videos a resolución más baja para análisis más rápido
- ✅ Cachea huellas digitales para evitar reprocesamiento
- ✅ Usa tipo apropiado de huella digital (Búsqueda vs Comparación)
- ✅ Prueba con segmentos de video pequeños primero
- ✅ Implementa callbacks de progreso para retroalimentación del usuario
- ✅ Desecha objetos de huella digital cuando termines

### Cosas que No Hacer

- ❌ No ignores callbacks de error
- ❌ No compares huellas digitales de diferentes tipos
- ❌ No proceses múltiples videos grandes simultáneamente sin gestión de memoria

## Próximos Pasos

Ahora que tienes el SDK instalado y funcionando, explora estos recursos:

1. **[Documentación de API](api.md)** - Referencia completa para todas las clases y métodos
2. **[Casos de Uso y Aplicaciones](../use-cases.md)** - Escenarios de implementación del mundo real
3. **[Entendiendo la Tecnología](../understanding-video-fingerprinting.md)** - Inmersión técnica profunda

## Obteniendo Ayuda

### Recursos

- **Referencia de API**: [https://api.visioforge.org/dotnet/](https://api.visioforge.org/dotnet/)
- **Muestras de GitHub**: [https://github.com/visioforge/.Net-SDK-s-samples/](https://github.com/visioforge/.Net-SDK-s-samples/)
- **Foro de Soporte**: [https://support.visioforge.com/](https://support.visioforge.com/)
- **Comunidad de Discord**: [https://discord.com/invite/yvXUG56WCH](https://discord.com/invite/yvXUG56WCH)

### Preguntas Comunes

- **P: ¿Puedo usar el SDK en una aplicación web?**
  R: Sí, el SDK puede usarse en aplicaciones ASP.NET Core para procesamiento del lado del servidor.

- **P: ¿Qué formatos de video están soportados?**
  R: MP4, AVI, MKV, MOV, WMV, FLV, WebM y muchos más a través de GStreamer.

- **P: ¿Qué tan precisa es la huella digital?**
  R: Típicamente 95-99% precisa para identificación de contenido, dependiendo de transformaciones.

- **P: ¿Puede detectar videos con marcas de agua agregadas?**
  R: Sí, el SDK puede identificar videos incluso con marcas de agua, logos o subtítulos agregados.

## Ejemplo Completo Funcional

Aquí hay una aplicación de consola completa que demuestra todas las operaciones básicas:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types.X.Sources;

namespace VideoFingerprintingDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Inicializa SDK
            VFPAnalyzer.SetLicenseKey("TRIAL");
            
            // Configura rutas
            string videosDir = @"C:\Videos";
            string dbDir = @"C:\FingerprintDB";
            Directory.CreateDirectory(dbDir);
            
            var app = new FingerprintingApp(videosDir, dbDir);
            
            while (true)
            {
                Console.WriteLine("\n=== Demo de Huella Digital de Video ===");
                Console.WriteLine("1. Generar huella digital para un video");
                Console.WriteLine("2. Comparar dos videos");
                Console.WriteLine("3. Encontrar fragmento en video");
                Console.WriteLine("4. Construir base de datos de huellas digitales");
                Console.WriteLine("5. Buscar en base de datos videos similares");
                Console.WriteLine("0. Salir");
                Console.Write("\nSelecciona opción: ");
                
                var choice = Console.ReadLine();
                Console.WriteLine();
                
                try
                {
                    switch (choice)
                    {
                        case "1":
                            await app.GenerateFingerprint();
                            break;
                        case "2":
                            await app.CompareTwoVideos();
                            break;
                        case "3":
                            await app.FindFragment();
                            break;
                        case "4":
                            await app.BuildDatabase();
                            break;
                        case "5":
                            await app.SearchDatabase();
                            break;
                        case "0":
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                Console.WriteLine("\nPresiona cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
    
    class FingerprintingApp
    {
        private string videosDir;
        private string dbDir;
        private Dictionary<string, VFPFingerPrint> database = new Dictionary<string, VFPFingerPrint>();
        
        public FingerprintingApp(string videosDir, string dbDir)
        {
            this.videosDir = videosDir;
            this.dbDir = dbDir;
            LoadDatabase();
        }
        
        public async Task GenerateFingerprint()
        {
            Console.Write("Ingresa nombre de archivo de video: ");
            string filename = Console.ReadLine();
            string videoPath = Path.Combine(videosDir, filename);
            
            if (!File.Exists(videoPath))
            {
                Console.WriteLine("¡Archivo no encontrado!");
                return;
            }
            
            var source = new VFPFingerprintSource(videoPath);
            
            Console.Write("¿Procesar video completo? (y/n): ");
            if (Console.ReadLine().ToLower() != "y")
            {
                Console.Write("Ingresa duración en segundos: ");
                if (int.TryParse(Console.ReadLine(), out int seconds))
                {
                    source.StopTime = TimeSpan.FromSeconds(seconds);
                }
            }
            
            Console.WriteLine("Generando huella digital...");
            var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                source,
                errorDelegate: (msg) => Console.WriteLine($"Error: {msg}"),
                progressDelegate: (p) => Console.Write($"\rProgreso: {p}%")
            );
            
            if (fp != null)
            {
                string outputPath = Path.ChangeExtension(videoPath, ".vfp");
                fp.Save(outputPath);
                Console.WriteLine($"\n✓ Huella digital guardada en: {outputPath}");
                Console.WriteLine($"  Duración: {fp.Duration}");
                Console.WriteLine($"  Resolución: {fp.Width}x{fp.Height}");
                Console.WriteLine($"  Tasa de Cuadros: {fp.FrameRate:F2} fps");
            }
        }
        
        public async Task CompareTwoVideos()
        {
            Console.Write("Ingresa nombre de archivo del primer video: ");
            string file1 = Path.Combine(videosDir, Console.ReadLine());
            
            Console.Write("Ingresa nombre de archivo del segundo video: ");
            string file2 = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(file1) || !File.Exists(file2))
            {
                Console.WriteLine("¡Uno o ambos archivos no encontrados!");
                return;
            }
            
            Console.WriteLine("Generando huellas digitales...");
            
            var source1 = new VFPFingerprintSource(file1);
            source1.StopTime = TimeSpan.FromSeconds(30); // Comparación rápida
            
            var source2 = new VFPFingerprintSource(file2);
            source2.StopTime = TimeSpan.FromSeconds(30);
            
            var fp1 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source1);
            var fp2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source2);
            
            if (fp1 != null && fp2 != null)
            {
                int difference = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromMilliseconds(500));
                
                Console.WriteLine($"\nPuntaje de Diferencia: {difference}");
                
                if (difference < 5)
                    Console.WriteLine("✓ Videos son IDÉNTICOS");
                else if (difference < 30)
                    Console.WriteLine("✓ Videos son MUY SIMILARES");
                else if (difference < 100)
                    Console.WriteLine("✓ Videos son SIMILARES");
                else if (difference < 300)
                    Console.WriteLine("⚠ Videos tienen ALGUNAS SIMILITUDES");
                else
                    Console.WriteLine("✗ Videos son DIFERENTES");
            }
        }
        
        public async Task FindFragment()
        {
            Console.Write("Ingresa nombre de archivo de video de fragmento: ");
            string fragmentFile = Path.Combine(videosDir, Console.ReadLine());
            
            Console.Write("Ingresa nombre de archivo de video completo: ");
            string fullFile = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(fragmentFile) || !File.Exists(fullFile))
            {
                Console.WriteLine("¡Uno o ambos archivos no encontrados!");
                return;
            }
            
            Console.WriteLine("Procesando fragmento...");
            var fragmentFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
                new VFPFingerprintSource(fragmentFile),
                progressDelegate: (p) => Console.Write($"\rFragmento: {p}%")
            );
            
            Console.WriteLine("\nProcesando video completo...");
            var fullFp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                new VFPFingerprintSource(fullFile),
                progressDelegate: (p) => Console.Write($"\rVideo completo: {p}%")
            );
            
            if (fragmentFp != null && fullFp != null)
            {
                Console.WriteLine("\n\nBuscando...");
                var positions = await VFPAnalyzer.SearchAsync(
                    fragmentFp, fullFp, fragmentFp.Duration, 50, true
                );
                
                if (positions.Count > 0)
                {
                    Console.WriteLine($"✓ Encontradas {positions.Count} ocurrencia(s):");
                    foreach (var pos in positions)
                    {
                        Console.WriteLine($"  - En {pos:hh\\:mm\\:ss}");
                    }
                }
                else
                {
                    Console.WriteLine("✗ Fragmento no encontrado");
                }
            }
        }
        
        public async Task BuildDatabase()
        {
            var videoFiles = Directory.GetFiles(videosDir, "*.mp4")
                .Concat(Directory.GetFiles(videosDir, "*.avi"))
                .Concat(Directory.GetFiles(videosDir, "*.mkv"))
                .ToList();
            
            Console.WriteLine($"Encontrados {videoFiles.Count} archivos de video");
            
            int processed = 0;
            foreach (var videoFile in videoFiles)
            {
                string id = Path.GetFileNameWithoutExtension(videoFile);
                string fpPath = Path.Combine(dbDir, $"{id}.vfp");
                
                if (File.Exists(fpPath))
                {
                    Console.WriteLine($"Saltando {id} (ya existe)");
                    continue;
                }
                
                Console.WriteLine($"Procesando {id}...");
                
                var source = new VFPFingerprintSource(videoFile);
                source.StopTime = TimeSpan.FromSeconds(60); // Solo primer minuto
                
                var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                    source,
                    progressDelegate: (p) => Console.Write($"\r  Progreso: {p}%")
                );
                
                if (fp != null)
                {
                    fp.ID = Guid.NewGuid();
                    fp.OriginalFilename = Path.GetFileName(videoFile);
                    fp.Save(fpPath);
                    processed++;
                    Console.WriteLine($"\r  ✓ Huella digital guardada para {id}");
                }
            }
            
            Console.WriteLine($"\n✓ Procesados {processed} videos");
            LoadDatabase();
        }
        
        public async Task SearchDatabase()
        {
            Console.Write("Ingresa nombre de archivo de video de consulta: ");
            string queryFile = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(queryFile))
            {
                Console.WriteLine("¡Archivo no encontrado!");
                return;
            }
            
            Console.Write("Ingresa umbral de similitud (predeterminado 30): ");
            if (!int.TryParse(Console.ReadLine(), out int threshold))
                threshold = 30;
            
            Console.WriteLine("Generando huella digital de consulta...");
            var queryFp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                new VFPFingerprintSource(queryFile) { StopTime = TimeSpan.FromSeconds(60) }
            );
            
            if (queryFp == null) return;
            
            Console.WriteLine($"Buscando {database.Count} huellas digitales...");
            
            var matches = new List<(string id, int score)>();
            
            foreach (var entry in database)
            {
                int score = VFPAnalyzer.Compare(queryFp, entry.Value, TimeSpan.FromMilliseconds(500));
                if (score < threshold)
                {
                    matches.Add((entry.Key, score));
                }
            }
            
            if (matches.Count > 0)
            {
                Console.WriteLine($"\n✓ Encontrados {matches.Count} video(s) similar(es):");
                foreach (var match in matches.OrderBy(m => m.score))
                {
                    var fp = database[match.id];
                    Console.WriteLine($"  - {fp.OriginalFilename} (puntaje: {match.score})");
                }
            }
            else
            {
                Console.WriteLine("\n✗ No se encontraron videos similares");
            }
        }
        
        private void LoadDatabase()
        {
            database.Clear();
            
            if (!Directory.Exists(dbDir))
                return;
            
            var files = Directory.GetFiles(dbDir, "*.vfp");
            foreach (var file in files)
            {
                try
                {
                    var fp = VFPFingerPrint.Load(file);
                    string id = Path.GetFileNameWithoutExtension(file);
                    database[id] = fp;
                }
                catch { }
            }
            
            Console.WriteLine($"Cargadas {database.Count} huellas digitales de la base de datos");
        }
    }
}
```

## Benchmarks de Rendimiento

| Operación | Duración | Tamaño de Archivo | Tiempo de Procesamiento | Uso de Memoria |
|-----------|----------|-----------|----------------|------------|
| Generar huella digital | 1 minuto | 100 MB | ~5 segundos | 200 MB |
| Generar huella digital | 10 minutos | 1 GB | ~45 segundos | 400 MB |
| Comparar huellas digitales | N/A | N/A | <1 ms | Mínimo |
| Buscar fragmento | 30 seg en 1 hora | N/A | ~100 ms | 100 MB |
| Consulta de base de datos | N/A | 1000 videos | ~50 ms | 250 MB |

## Resumen

Ahora has aprendido cómo:

- ✅ Instalar y configurar el SDK de Huella Digital de Video con paquetes NuGet apropiados
- ✅ Generar huellas digitales desde archivos de video
- ✅ Comparar videos para similitud
- ✅ Buscar fragmentos dentro de videos
- ✅ Construir y consultar una base de datos de huellas digitales
- ✅ Manejar problemas comunes y optimizar rendimiento

El SDK de Huella Digital de Video proporciona una base poderosa para aplicaciones de identificación de contenido, detección de duplicados y monitoreo de medios. Comienza con los ejemplos simples e incorpora gradualmente características más avanzadas a medida que crezcan tus necesidades.

¡Felicitaciones! Ahora estás listo para construir poderosas aplicaciones de huella digital de video con el SDK de VisioForge.