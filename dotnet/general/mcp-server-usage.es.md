---
title: Uso del Servidor MCP de VisioForge para Desarrollo con IA
description: Conecte su asistente IA al Servidor MCP de VisioForge para acceso a documentación API, guías de despliegue, ejemplos de código y conocimiento del SDK.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Conversion
  - IP Camera
  - RTSP
  - C#
  - NuGet
primary_api_classes:
  - MediaBlocksPipeline
  - VideoRendererBlock
  - RTSPSourceBlock

---

# Uso del Servidor MCP de VisioForge para Desarrollo Asistido por IA

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción al Servidor MCP de VisioForge

El Servidor MCP (Protocolo de Contexto de Modelo) de VisioForge proporciona a los asistentes de codificación IA acceso directo a documentación completa del SDK de VisioForge, guías de implementación, ejemplos de código y referencias API. Esto permite que su asistente IA brinde ayuda precisa y contextual mientras desarrolla con los SDKs de VisioForge.

### ¿Qué es el Protocolo de Contexto de Modelo (MCP)?

El Protocolo de Contexto de Modelo (MCP) es un estándar abierto desarrollado por Anthropic que permite a los asistentes IA conectarse de forma segura a fuentes de conocimiento y herramientas externas. Piense en él como un puente entre su asistente de codificación IA (como Claude Code, GitHub Copilot o extensiones de VS Code) y servidores de documentación especializados.

Con MCP, su asistente IA puede:

- Consultar documentación API en tiempo real
- Obtener guías de implementación para plataformas específicas
- Recuperar ejemplos de código y fragmentos
- Buscar en la documentación del SDK
- Obtener detalles de configuración específicos de la plataforma

## ¿Por Qué Usar el Servidor MCP de VisioForge?

Al desarrollar con los SDKs de VisioForge, el servidor MCP proporciona varios beneficios clave:

### 1. **Acceso Instantáneo a Documentación API**

Su asistente IA puede consultar la API completa del SDK de VisioForge, incluyendo:

- Todas las clases, métodos, propiedades y eventos
- Descripciones detalladas y notas de uso
- Tipos de parámetros y valores de retorno
- Ejemplos de código y fragmentos
- Referencias cruzadas a APIs relacionadas

### 2. **Orientación de Implementación Específica de Plataforma**

Obtenga instrucciones precisas de implementación para:

- **Escritorio**: Windows, Linux, macOS
- **Móvil**: Android, iOS, Mac Catalyst
- **Frameworks**: MAUI, Uno, Avalonia, WPF, WinForms, Blazor, Console
- **Escenarios**: Grabación RTSP, transcodificación en la nube, streaming HLS

### 3. **Referencias Correctas de Paquetes NuGet**

El servidor MCP genera fragmentos `.csproj` listos para pegar con:

- Paquetes NuGet específicos de plataforma
- Números de versión correctos
- Referencias de paquetes condicionales
- Referencias de proyecto requeridas (como AndroidDependency)

### 4. **Configuración de Compilación Específica de Plataforma**

Recupere targets de MSBuild y fragmentos de configuración para:

- Copia de bibliotecas nativas en Mac Catalyst
- Permisos de Android (manifest + runtime)
- Permisos de Info.plist en iOS
- Configuraciones de compilación específicas de plataforma

## Requisitos Previos

Antes de conectarse al Servidor MCP de VisioForge, asegúrese de tener:

- **Un asistente IA compatible con MCP**:
  - [Claude Code](https://claude.ai/code) (recomendado)
  - VS Code con extensión MCP
  - GitHub Copilot con soporte MCP
  - Otras herramientas compatibles con MCP

- **Conectividad a Internet** para acceder a `https://mcp.visioforge.com`

## Conexión al Servidor MCP

### Claude Code (Recomendado)

Claude Code tiene soporte MCP integrado. Conéctese con un solo comando:

```bash
claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp
```

**Verificar la conexión:**

```bash
claude mcp list
```

Debería ver `visioforge-sdk` en la lista de servidores conectados.

### VS Code con Extensión MCP

Agregue el Servidor MCP de VisioForge a la configuración de su espacio de trabajo o usuario:

1. Abra VS Code
2. Instale la extensión MCP (si aún no está instalada)
3. Cree o edite `.vscode/mcp.json` en su proyecto:

```json
{
  "servers": {
    "visioforge-sdk": {
      "type": "http",
      "url": "https://mcp.visioforge.com/mcp"
    }
  }
}
```

### Configuración a Nivel de Proyecto (Cualquier Cliente MCP)

Para configuración MCP específica del proyecto, cree `.mcp.json` en la raíz de su repositorio:

```json
{
  "servers": {
    "visioforge-sdk": {
      "type": "http",
      "url": "https://mcp.visioforge.com/mcp",
      "description": "Documentación del SDK de VisioForge y guías de implementación"
    }
  }
}
```

## Herramientas MCP Disponibles

El Servidor MCP de VisioForge expone 14 herramientas especializadas que su asistente IA puede usar. Los nombres y descripciones coinciden exactamente con la respuesta `tools/list` en vivo de `https://mcp.visioforge.com/mcp`.

### 1. **Herramientas de Media Blocks**

#### `list_media_blocks`
Listar los media blocks disponibles de VisioForge, opcionalmente filtrados por categoría. Los media blocks son los bloques de construcción de los pipelines de procesamiento multimedia. Las categorías incluyen: Sources, Sinks, VideoEncoders, AudioEncoders, VideoDecoders, VideoProcessing, AudioProcessing, AudioRendering, VideoRendering, Demuxers, Parsers, OpenGL, OpenCV, Nvidia, Decklink, AWS, RTSPServer, Bridge, Special, Outputs.

**Consultas de ejemplo:**
- "Listar todos los bloques codificadores de video"
- "Mostrar las fuentes de MediaBlocks"
- "¿Qué bloques hay en la categoría OpenCV?"

#### `get_media_block_info`
Obtener información detallada sobre un media block específico incluyendo sus propiedades, métodos, eventos, pads de entrada/salida, parámetros del constructor y documentación. Use esto para entender cómo configurar y usar un media block específico en un pipeline.

**Consultas de ejemplo:**
- "Obtener información sobre RTSPSourceBlock"
- "Mostrar pads y propiedades de H264EncoderBlock"
- "¿Qué parámetros del constructor toma VideoRendererBlock?"

#### `get_pipeline_template`
Obtener una plantilla de pipeline de media blocks para un caso de uso específico. Devuelve la lista de bloques requeridos y cómo conectarlos, junto con código C# para construir el pipeline.

**Consultas de ejemplo:**
- "Plantilla de pipeline para grabación RTSP a MP4"
- "Plantilla para captura de pantalla con audio"
- "Pipeline para streaming HLS"

### 2. **Herramientas de Clases SDK y API**

#### `list_sdk_classes`
Listar las clases principales del SDK de VisioForge. Estas son las clases principales de punto de entrada para construir aplicaciones multimedia: VideoCaptureCoreX (captura/grabación de video), VideoEditCoreX (edición de video), MediaPlayerCoreX (reproducción multimedia), MediaInfoReaderCoreX (análisis multimedia), SimplePlayerCoreX (reproducción simple) y más.

**Consultas de ejemplo:**
- "Listar todas las clases principales del SDK"
- "Mostrar clases de punto de entrada de nivel superior"

#### `get_class_info`
Obtener información detallada sobre cualquier clase del SDK de VisioForge, incluyendo su lista completa de propiedades, métodos, eventos, constructores, clase base, interfaces y documentación. Funciona tanto para las clases principales del SDK como para los media blocks.

**Consultas de ejemplo:**
- "Mostrar documentación de la clase MediaBlocksPipeline"
- "Obtener detalles sobre VideoCaptureCoreX"
- "¿Qué eventos expone MediaPlayerCoreX?"

#### `get_method_signature`
Obtener la firma detallada y la documentación de un método específico de una clase. Útil cuando necesita entender los parámetros, tipo de retorno y comportamiento de un método.

**Consultas de ejemplo:**
- "Firma de StartAsync en MediaBlocksPipeline"
- "¿Qué parámetros toma Connect?"

#### `search_api`
Buscar en toda la API del SDK de VisioForge — nombres de clases, nombres de métodos, nombres de propiedades, nombres de eventos y su texto de documentación. Devuelve resultados clasificados. Use esto cuando no conoce el nombre exacto de la clase, o para encontrar todas las clases relacionadas con un concepto (por ejemplo, "streaming RTSP", "superposición de video", "captura de audio").

**Consultas de ejemplo:**
- "Buscar clases de captura de video"
- "Encontrar métodos relacionados con streaming RTSP"
- "Mostrar todos los codificadores de audio de MediaBlocks"

#### `get_enum_values`
Obtener todos los valores de un tipo enum del SDK de VisioForge con descripciones. Útil para entender las opciones disponibles para las propiedades de configuración (por ejemplo, MediaBlockType, códecs de video, formatos de audio, formatos de píxeles).

**Consultas de ejemplo:**
- "Listar valores del enum VideoCodec"
- "Mostrar valores del enum MediaBlockType"

#### `list_namespaces`
Explorar los namespaces del SDK de VisioForge jerárquicamente. Muestra los namespaces hijos y las clases dentro de un namespace dado. Comience con `VisioForge.Core` o déjelo vacío para ver los namespaces de nivel superior.

**Consultas de ejemplo:**
- "Listar los namespaces de nivel superior"
- "Mostrar clases en VisioForge.Core.MediaBlocks"

#### `get_code_example`
Obtener un ejemplo de código para un escenario común del SDK de VisioForge. Devuelve fragmentos de código C# completos y funcionales que demuestran cómo usar el SDK para tareas como captura de video, streaming RTSP, reproducción multimedia y más.

**Consultas de ejemplo:**
- "Ejemplo de código para captura de cámara RTSP"
- "Mostrar fragmento de grabación MP4"
- "Ejemplo de aplicación de efectos de video"

### 3. **Herramientas de Guías de Implementación**

#### `list_deployment_guides`
Listar las guías de implementación disponibles para el SDK de VisioForge. Filtrar por plataforma, tipo de proyecto, tipo de SDK o escenario. Devuelve una lista de guías con títulos, resúmenes y etiquetas.

**Consultas de ejemplo:**
- "Listar guías de implementación de Android"
- "Mostrar guías de implementación de MAUI"
- "Encontrar guías de implementación para Linux"

#### `get_deployment_guide`
Obtener la guía de implementación completa para un escenario específico. Devuelve instrucciones detalladas, fragmentos de código, paquetes NuGet y notas específicas de la plataforma.

**Consultas de ejemplo:**
- "Obtener la guía de implementación de Android"
- "Mostrar pasos de implementación para plataforma Uno"
- "Cómo implementar en macOS"

#### `get_nuget_packages_snippet`
Obtener un fragmento .csproj con los paquetes NuGet requeridos para un escenario de implementación específico. Devuelve un fragmento XML listo para copiar y pegar en su archivo de proyecto.

**Consultas de ejemplo:**
- "Generar paquetes NuGet para proyecto MAUI Android"
- "Obtener referencias de paquetes para Avalonia en Windows"
- "Mostrar paquetes requeridos para iOS"

#### `get_platform_specific_config`
Obtener código de configuración específico de plataforma para copia de archivos/compilación. Devuelve targets de MSBuild o scripts post-compilación para requisitos especiales de implementación.

**Consultas de ejemplo:**
- "Mostrar target de copia de archivos de Mac Catalyst"
- "Obtener permisos de manifest de Android"
- "Claves de permisos de Info.plist de iOS"

## Ejemplos de Uso

### Ejemplo 1: Configuración de un Proyecto MAUI Android

**Usted pregunta a su asistente IA:**
> "Estoy creando una aplicación de captura de video con MAUI para Android. ¿Qué paquetes NuGet necesito?"

**Su asistente IA usa el servidor MCP para:**
1. Llamar a `get_nuget_packages_snippet` con `platform: Android, projectType: MAUI, sdkType: MediaBlocks`
2. Recuperar las referencias de paquetes correctas
3. Proporcionarle XML listo para pegar:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.2.4" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />
  <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X9.csproj" />
</ItemGroup>
```

### Ejemplo 2: Encontrar Cómo Usar Streaming RTSP

**Usted pregunta a su asistente IA:**
> "Muéstrame cómo capturar desde una cámara RTSP usando Media Blocks SDK"

**Su asistente IA usa el servidor MCP para:**
1. Llamar a `search_api` con consulta "captura de cámara RTSP"
2. Identificar la clase `RTSPSourceBlock`
3. Llamar a `get_code_example` para escenarios RTSP
4. Proporcionarle código funcional:

```csharp
var pipeline = new MediaBlocksPipeline();

// RTSPSourceBlock toma RTSPSourceSettings (no un Uri directamente).
// Construya los settings con la factory async — el ctor es privado.
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://camera.example.com:554/stream"),
    login: null,
    password: null,
    audioEnabled: false);

var rtspSource = new RTSPSourceBlock(rtspSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
await pipeline.StartAsync();
```

### Ejemplo 3: Implementación en Mac Catalyst

**Usted pregunta a su asistente IA:**
> "¿Cómo implemento mi aplicación Uno en Mac Catalyst?"

**Su asistente IA usa el servidor MCP para:**
1. Llamar a `get_deployment_guide` con `guideId: "uno-platform"`
2. Extraer la sección de Mac Catalyst
3. Llamar a `get_platform_specific_config` con `platform: "maccatalyst"`
4. Proporcionarle:
   - Comandos de compilación
   - Target de MSBuild para copia de archivos
   - Pasos de implementación

### Ejemplo 4: Entender una API Específica

**Usted pregunta a su asistente IA:**
> "¿Qué parámetros acepta UniversalSourceBlock?"

**Su asistente IA usa el servidor MCP para:**
1. Llamar a `search_api` con consulta "UniversalSourceBlock"
2. Encontrar la clase en los resultados
3. Llamar a `get_class_info` con el nombre de la clase
4. Analizar la documentación y explicar:
   - Parámetros del constructor
   - Formatos de archivo soportados
   - Opciones de configuración
   - Ejemplos de uso

## Mejores Prácticas

### 1. **Sea Específico en Sus Preguntas**

En lugar de preguntas genéricas, proporcione contexto:

- ❌ "¿Cómo capturo video?"
- ✅ "¿Cómo capturo video de una cámara RTSP usando Media Blocks SDK en Android?"

### 2. **Especifique Su Plataforma y Framework**

Siempre mencione su plataforma objetivo y framework de UI:

- "Estoy usando MAUI en iOS..."
- "Mi aplicación Avalonia se dirige a Windows y Linux..."
- "Para mi aplicación de Plataforma Uno en Android..."

### 3. **Pregunte sobre Implementación Temprano**

Antes de profundizar en el código, pregunte sobre requisitos de implementación:

- "¿Qué paquetes NuGet necesito para Mac Catalyst?"
- "Muéstrame la guía de implementación para Avalonia en Linux"
- "¿Qué permisos se requieren para acceso a cámara en iOS?"

### 4. **Solicite Ejemplos de Código**

No dude en pedir código funcional:

- "Muéstrame un ejemplo completo de..."
- "Genera código para..."
- "Implementación de ejemplo de..."

## Solución de Problemas

### Problemas de Conexión

Si su asistente IA no puede conectarse al servidor MCP:

1. **Verifique su conexión a Internet** - El servidor MCP está alojado en `https://mcp.visioforge.com`
2. **Verifique la URL** - Asegúrese de estar usando el endpoint correcto: `https://mcp.visioforge.com/mcp`
3. **Reinicie su asistente IA** - A veces un reinicio resuelve problemas de conexión
4. **Verifique los logs del cliente MCP** - Busque errores de conexión en los logs de su cliente

### Información Incorrecta o Desactualizada

El servidor MCP se actualiza regularmente, pero si nota información incorrecta:

1. **Verifique la versión del SDK** - Asegúrese de estar usando la última versión del SDK
2. **Verifique versiones de paquetes** - Compare con [NuGet.org](https://www.nuget.org/packages?q=VisioForge)
3. **Reporte problemas** - Contacte a nuestro equipo de soporte (vea Recursos Adicionales abajo)

### Asistente IA No Usa el Servidor MCP

Si su asistente IA no parece usar el servidor MCP:

1. **Menciónelo explícitamente** - Diga "Usa el servidor MCP de VisioForge para encontrar..."
2. **Verifique la conexión** - Ejecute `claude mcp list` o verifique su configuración MCP
3. **Reinicie la sesión** - Comience una nueva conversación con su asistente IA

## Seguridad y Privacidad

### Transmisión de Datos

- Toda comunicación con el servidor MCP usa **cifrado HTTPS**
- El servidor es de solo lectura - solo proporciona documentación, sin recopilación de datos
- No se envía información personal ni código al servidor
- Las consultas API se procesan en tiempo real y no se almacenan

### Autenticación

- El Servidor MCP de VisioForge es **accesible públicamente** - no se requiere autenticación
- Su asistente IA se conecta directamente a `https://mcp.visioforge.com/mcp`
- No se necesitan claves API ni credenciales

## Detalles Técnicos

### Endpoint del Servidor MCP

```
https://mcp.visioforge.com/mcp
```

### Capacidades del Servidor

- **Protocolo**: MCP (Protocolo de Contexto de Modelo)
- **Transporte**: HTTP/HTTPS
- **Herramientas**: 14 herramientas especializadas de documentación e implementación
- **Cobertura API**: API completa del SDK .NET de VisioForge (todas las clases, métodos, propiedades)
- **Guías de Implementación**: Más de 15 guías de plataforma y tipo de proyecto
- **Ejemplos de Código**: Cientos de fragmentos de código funcionales
- **Frecuencia de Actualización**: Actualizado con cada lanzamiento del SDK

### Arquitectura del Servidor

El servidor MCP ofrece:

- **Alta disponibilidad**: 99.9% de tiempo de actividad
- **Tiempos de respuesta rápidos**: < 200ms promedio
- **Cifrado SSL/TLS**: Todo el tráfico cifrado
- **Actualizaciones automáticas**: Sincronizado con lanzamientos del SDK
- **Limitación de tasa**: Política de uso justo (sin límites estrictos para desarrolladores)

## Recursos Adicionales

### Documentación

- [Documentación del SDK de VisioForge](https://www.visioforge.com/help/)
- [Especificación del Protocolo MCP](https://modelcontextprotocol.io/specification)
- [Documentación de Claude Code](https://claude.ai/code)

### Soporte y Comunidad

¿Necesita ayuda? Póngase en contacto:

- **[Portal de Soporte](https://support.visioforge.com/)** - Soporte técnico y reporte de problemas
- **[Comunidad Discord](https://discord.com/invite/yvXUG56WCH)** - Chatea con desarrolladores y obtén respuestas rápidas
- **[Muestras en GitHub](https://github.com/visioforge/.Net-SDK-s-samples)** - Proyectos de ejemplo completos
- **Correo electrónico:** <support@visioforge.com>

### Guías Relacionadas

- [Guía de Instalación](../install/index.md)
- [Requisitos del Sistema](../system-requirements.md)
- [Guías de Implementación](../deployment-x/index.md)
- [Referencia API](https://api.visioforge.org/dotnet/api/index.html)

## Conclusión

El Servidor MCP de VisioForge transforma el desarrollo asistido por IA al proporcionar a su asistente de codificación acceso directo a documentación completa y actualizada del SDK. Ya sea que esté construyendo una aplicación de captura de video en Android, un reproductor multimedia en Windows o una herramienta de edición multiplataforma con Avalonia, el servidor MCP asegura que su asistente IA tenga el conocimiento para ayudarlo a tener éxito.

¡Conéctese hoy y experimente el futuro del desarrollo de SDK con IA!
