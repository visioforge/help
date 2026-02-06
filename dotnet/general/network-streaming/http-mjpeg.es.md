---
title: Streaming de Video HTTP MJPEG en .NET
description: Streaming HTTP MJPEG en .NET para feeds de video en tiempo real con manejo de conexiones de clientes y tasas de cuadros ajustables.
---

# Streaming HTTP MJPEG

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

La característica del SDK de transmitir video codificado como Motion JPEG (MJPEG) sobre HTTP es ventajosa por su simplicidad y amplia compatibilidad. MJPEG codifica cada cuadro de video individualmente como una imagen JPEG, lo que simplifica la decodificación y es ideal para aplicaciones como streaming web y vigilancia. El uso de HTTP asegura fácil integración y alta compatibilidad entre diferentes plataformas y dispositivos, y es efectivo incluso en redes con configuraciones estrictas. Este método es particularmente adecuado para feeds de video en tiempo real y aplicaciones que requieren análisis directo cuadro por cuadro. Con tasas de cuadros y resoluciones ajustables, el SDK ofrece flexibilidad para varias condiciones de red y requisitos de calidad.

## Salida MJPEG multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La funcionalidad de streaming se implementa a través de dos clases principales:

1. `HTTPMJPEGLiveOutput`: La clase de configuración de alto nivel que configura la salida de streaming
2. `HTTPMJPEGLiveSinkBlock`: El bloque de implementación subyacente que maneja el proceso de streaming real

### Clase HTTPMJPEGLiveOutput

Esta clase sirve como el punto de entrada de configuración para configurar un stream HTTP MJPEG. Implementa la interfaz `IVideoCaptureXBaseOutput`, haciéndola compatible con el sistema de pipeline de captura de video.

#### Propiedades clave

- `Port`: Obtiene el número de puerto de red en el cual se servirá el stream MJPEG

#### Uso

```csharp
// Crear una nueva salida de streaming MJPEG en el puerto 8080
var mjpegOutput = new HTTPMJPEGLiveOutput(8080);

// Agregar la salida MJPEG al motor VideoCaptureCoreX
core.Outputs_Add(mjpegOutput, true);
```

#### Detalles de implementación

- La clase está diseñada para ser inmutable, con el puerto siendo establecido solo a través del constructor
- No soporta codificadores de video o audio, ya que MJPEG usa codificación JPEG directa
- Los métodos relacionados con filename devuelven null o son no-ops, ya que esta es una implementación solo de streaming

### Clase HTTPMJPEGLiveSinkBlock

Esta clase maneja la implementación real de la funcionalidad de streaming MJPEG. Es responsable de:

- Configurar el pipeline para procesamiento de video
- Gestionar el servidor HTTP para streaming
- Manejar datos de video de entrada y convertirlos a formato MJPEG
- Gestionar conexiones de clientes y entrega del stream

#### Características clave

- Implementa múltiples interfaces para integración con el pipeline de medios:
  - `IMediaBlockInternals`: Para integración de pipeline
  - `IMediaBlockDynamicInputs`: Para manejar conexiones de entrada dinámicas
  - `IMediaBlockSink`: Para funcionalidad de sink
  - `IDisposable`: Para limpieza apropiada de recursos

### Notas de implementación

#### Inicialización

```csharp
// El bloque debe ser inicializado con un número de puerto
var mjpegSink = new HTTPMJPEGLiveSinkBlock(8080);
pipeline.Connect(videoSource.Output, mjpegSink.Input);

// "URL de etiqueta IMG es http://127.0.0.1:8090";
```

#### Uso del cliente

Para consumir el stream MJPEG:

1. Inicialice la salida de streaming con el puerto deseado
2. Conéctelo a su pipeline de video
3. Acceda al stream a través de un navegador web o cliente HTTP en:
   ```
   http://[dirección-servidor]:[puerto]
   ```

#### Ejemplo de cliente HTML

```html
<img src="http://localhost:8080" />
```

### Limitaciones y consideraciones

1. Uso de ancho de banda
   - Los streams MJPEG pueden usar ancho de banda significativo ya que cada cuadro es un JPEG completo
   - Considere la configuración de tasa de cuadros y resolución para rendimiento óptimo

2. Soporte de navegador
   - Aunque MJPEG es ampliamente soportado, algunos navegadores modernos pueden tener limitaciones
   - Los dispositivos móviles pueden manejar streams MJPEG de manera diferente

3. Latencia
   - Aunque MJPEG proporciona latencia relativamente baja, no es adecuado para requisitos de ultra baja latencia
   - Las condiciones de red pueden afectar el tiempo de entrega de cuadros

### Mejores prácticas

1. Selección de puerto
   - Elija puertos que no entren en conflicto con otros servicios
   - Considere las implicaciones de firewall al seleccionar puertos

2. Gestión de recursos
   - Siempre disponga del bloque sink apropiadamente
   - Monitoree las conexiones de clientes y uso de recursos

3. Manejo de errores
   - Implemente manejo de errores apropiado para problemas de red y pipeline
   - Monitoree el estado del pipeline para problemas potenciales

### Consideraciones de seguridad

1. Seguridad de red
   - El stream MJPEG no está cifrado por defecto
   - Considere implementar medidas de seguridad adicionales para contenido sensible

2. Control de acceso
   - Sin mecanismo de autenticación integrado
   - Considere implementar control de acceso a nivel de aplicación si es necesario

## Salida MJPEG solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Establezca la propiedad `Network_Streaming_Enabled` a true para habilitar streaming en red.

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

Establezca la salida HTTP MJPEG.

```cs
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.HTTP_MJPEG;
```

Cree el objeto de configuración y establezca el puerto.

```cs
VideoCapture1.Network_Streaming_Output = new MJPEGOutput(8080);
```

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.