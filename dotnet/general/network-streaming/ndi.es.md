---
title: Integración de Transmisión de Video NDI en .NET
description: Implemente transmisión NDI de alto rendimiento en .NET para transmisión de video y audio de baja latencia sobre redes IP con flujos de trabajo profesionales.
---

# Integración de Transmisión Network Device Interface (NDI)

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## ¿Qué es NDI y Por Qué Usarlo?

La integración de la tecnología Network Device Interface (NDI) en el SDK de VisioForge proporciona una solución transformadora para flujos de trabajo profesionales de producción y transmisión de video. NDI ha emergido como un estándar líder de la industria para producción en vivo, permitiendo transmisión de video de alta calidad y ultra baja latencia sobre redes Ethernet convencionales.

NDI simplifica significativamente el proceso de compartir y gestionar múltiples flujos de video a través de diversos dispositivos y plataformas. Cuando se implementa dentro del SDK de VisioForge, facilita la transmisión perfecta de contenido de video y audio de alta definición desde servidores a clientes con características de rendimiento excepcionales. Esto hace que la tecnología sea particularmente valiosa para aplicaciones incluyendo:

- Transmisión y streaming en vivo
- Videoconferencia profesional
- Configuraciones de producción multi-cámara
- Flujos de trabajo de producción remota
- Entornos de presentación educativa y corporativa

La flexibilidad inherente y eficiencia de la tecnología de transmisión NDI reduce sustancialmente la dependencia de configuraciones de hardware especializado, entregando una alternativa rentable a sistemas tradicionales basados en SDI para producción de video profesional en vivo.

## Requisitos de Instalación

### Prerrequisitos para Implementación NDI

Para implementar exitosamente la funcionalidad de transmisión NDI dentro de su aplicación, debe instalar uno de los siguientes paquetes de software NDI oficiales:

1. **[NDI SDK](https://ndi.video/for-developers/ndi-sdk/download/)** - Recomendado para desarrolladores que necesitan acceso completo a la funcionalidad NDI
2. **[NDI Tools](https://ndi.video/tools/)** - Adecuado para escenarios básicos de implementación y pruebas

Estos paquetes proporcionan los componentes de runtime necesarios que habilitan la comunicación NDI a través de su infraestructura de red.

## Implementación de Salida NDI Multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Entendiendo la Arquitectura de Clase NDIOutput

La clase `NDIOutput` sirve como el marco de implementación central para funcionalidad NDI dentro del ecosistema del SDK de VisioForge. Esta clase encapsula propiedades de configuración y lógica de procesamiento requerida para transmisión de video sobre IP de alto rendimiento usando el protocolo NDI. La arquitectura habilita transmisión de video y audio de calidad broadcast a través de infraestructura de red estándar sin requisitos de hardware especializado.

#### Definición de Clase e Implementación de Interfaz

```csharp
public class NDIOutput : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

La clase implementa varias interfaces que proporcionan funcionalidad completa para diferentes escenarios de salida:

- `IVideoEditXBaseOutput` - Proporciona integración con flujos de trabajo de edición de video
- `IVideoCaptureXBaseOutput` - Habilita capacidades de streaming directo de captura a NDI
- `IOutputVideoProcessor` - Permite procesamiento avanzado de video durante la salida
- `IOutputAudioProcessor` - Facilita procesamiento y manipulación de audio en el pipeline NDI

### Propiedades de Configuración

#### Pipeline de Procesamiento de Video

```csharp
public MediaBlock CustomVideoProcessor { get; set; }
```

Esta propiedad permite a los desarrolladores extender el pipeline de transmisión NDI con funcionalidad de procesamiento de video personalizado. Al asignar una implementación `MediaBlock` personalizada, puede integrar filtros de video especializados, transformaciones o algoritmos de análisis antes de que el contenido sea transmitido vía NDI.

#### Pipeline de Procesamiento de Audio

```csharp
public MediaBlock CustomAudioProcessor { get; set; }
```

Similar a la propiedad del procesador de video, esto permite la inserción de lógica de procesamiento de audio personalizado. Aplicaciones comunes incluyen ajuste dinámico de nivel de audio, reducción de ruido o efectos de audio especializados que mejoran la experiencia de transmisión.

#### Configuración de Sumidero NDI

```csharp
public NDISinkSettings Sink { get; set; }
```

Esta propiedad contiene los parámetros de configuración completos para el sumidero de salida NDI, incluyendo configuraciones esenciales como identificación de flujo, opciones de compresión y parámetros de transmisión de red.

### Sobrecargas de Constructor

#### Constructor Básico con Nombre de Flujo

```csharp
public NDIOutput(string name)
```

Crea una nueva instancia de salida NDI con el nombre de flujo especificado, que identificará esta fuente NDI en la red.

**Parámetros:**

- `name`: Identificador de cadena para el flujo NDI visible para receptores en la red

#### Constructor Avanzado con Configuraciones Pre-configuradas

```csharp
public NDIOutput(NDISinkSettings settings)
```

Crea una nueva instancia de salida NDI con configuraciones de sumidero completamente pre-configuradas para escenarios de implementación avanzada.

**Parámetros:**

- `settings`: Un objeto `NDISinkSettings` completamente configurado que contiene todos los parámetros de configuración NDI requeridos

### Métodos Centrales

#### Identificación de Flujo

```csharp
public string GetFilename()
```

Devuelve el nombre configurado del flujo NDI. Este método mantiene compatibilidad con interfaces de salida basadas en archivos en la arquitectura del SDK.

**Devuelve:** El identificador de flujo NDI actual

```csharp
public void SetFilename(string filename)
```

Actualiza el identificador de flujo NDI. Este método se usa principalmente para compatibilidad con otros tipos de salida que usan identificación basada en nombre de archivo.

**Parámetros:**

- `filename`: El nombre actualizado para el flujo NDI

#### Gestión de Codificador

```csharp
public Tuple<string, Type>[] GetVideoEncoders()
```

Devuelve un arreglo vacío ya que NDI maneja la codificación de video internamente a través de su tecnología propietaria.

**Devuelve:** Arreglo vacío de tuplas de codificador

```csharp
public Tuple<string, Type>[] GetAudioEncoders()
```

Devuelve un arreglo vacío ya que NDI maneja la codificación de audio internamente a través de su tecnología propietaria.

**Devuelve:** Arreglo vacío de tuplas de codificador

## Ejemplos de Implementación

### Implementación de Media Blocks SDK

El siguiente ejemplo demuestra cómo configurar una salida NDI usando la arquitectura Media Blocks SDK:

```cs
// Crear un bloque de salida NDI con un nombre de flujo descriptivo
var ndiSink = new NDISinkBlock("VisioForge Production Stream");

// Conectar fuente de video a la salida NDI
// El método CreateNewInput establece un canal de entrada de video para el sumidero NDI
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Conectar fuente de audio a la salida NDI
// El método CreateNewInput establece un canal de entrada de audio para el sumidero NDI
pipeline.Connect(audioSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

### Implementación de Video Capture SDK

Este ejemplo muestra cómo integrar transmisión NDI dentro del marco Video Capture SDK:

```cs
// Inicializar salida NDI con un nombre de flujo amigable para red
var ndiOutput = new NDIOutput("VisioForge_Studio_Output");

// Agregar la salida NDI configurada al pipeline de captura de video
core.Outputs_Add(ndiOutput); // core representa la instancia VideoCaptureCoreX
```

## Implementación NDI Específica de Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Para implementaciones específicas de Windows, el SDK proporciona opciones de configuración adicionales a través de los componentes VideoCaptureCore o VideoEditCore.

### Guía de Implementación Paso a Paso

#### 1. Habilitar Transmisión de Red

Primero, active la funcionalidad de transmisión de red:

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

#### 2. Configurar Transmisión de Audio

Habilite transmisión de audio junto con contenido de video:

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

#### 3. Seleccionar Protocolo NDI

Especifique NDI como el formato de transmisión preferido:

```csharp
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.NDI;
```

#### 4. Crear y Configurar Salida NDI

Inicialice la salida NDI con un nombre descriptivo:

```cs
var streamName = "VisioForge NDI Streamer";
var ndiOutput = new NDIOutput(streamName); 
```

#### 5. Asignar la Salida

Conecte la salida NDI configurada al pipeline de captura de video:

```cs
VideoCapture1.Network_Streaming_Output = ndiOutput;
```

#### 6. Generar la URL NDI (Opcional)

Para depuración o compartir, puede generar la URL de protocolo NDI estándar:

```cs
string ndiUrl = $"ndi://{System.Net.Dns.GetHostName()}/{streamName}";
Debug.WriteLine(ndiUrl);
```

## Consideraciones de Integración Avanzada

Cuando implemente transmisión NDI en entornos de producción, considere los siguientes factores:

- **Requisitos de ancho de banda de red** - Los flujos NDI pueden consumir ancho de banda significativo dependiendo de resolución y framerate
- **Compensaciones de calidad vs. latencia** - Configure configuraciones de compresión apropiadas basadas en su caso de uso específico
- **Distribución multicast vs. unicast** - Determine el método óptimo de transmisión de red basado en su infraestructura
- **Opciones de aceleración por hardware** - Aproveche aceleración GPU donde esté disponible para rendimiento mejorado
- **Mecanismo de descubrimiento** - Considere cómo las fuentes NDI serán descubiertas a través de segmentos de red

## Componentes Relacionados

- **NDISinkSettings** - Proporciona opciones de configuración detalladas para el sumidero de salida NDI
- **NDISinkBlock** - Implementa la funcionalidad central de salida NDI referenciada en NDISinkSettings
- **MediaBlockPadMediaType** - Enum usado para especificar el tipo de medio (video o audio) para conexiones de entrada

---
Visite nuestro [repositorio GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para muestras de código adicionales y ejemplos de implementación.