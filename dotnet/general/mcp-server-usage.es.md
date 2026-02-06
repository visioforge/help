---
title: Uso del Servidor MCP de VisioForge para Desarrollo Asistido por IA
description: Conecte su asistente de codificación IA al Servidor MCP de VisioForge para acceso instantáneo a documentación API, guías de implementación, ejemplos de código y conocimiento del SDK.
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

El Servidor MCP de VisioForge proporciona varias herramientas especializadas que su asistente IA puede usar:

### 1. **Herramientas de Documentación API**

#### `search_api`
Buscar en la API del SDK de VisioForge por palabras clave, tipos o categorías.

**Consultas de ejemplo que su asistente IA puede hacer:**
- "Buscar clases de captura de video"
- "Encontrar métodos relacionados con streaming RTSP"
- "Mostrar todos los codificadores de audio de MediaBlocks"

#### `get_api_item`
Recuperar documentación detallada para una clase, método, propiedad o evento específico.

**Consultas de ejemplo:**
- "Mostrar documentación de la clase MediaBlocksPipeline"
- "Obtener detalles sobre VideoRendererBlock"
- "Explicar el método StartAsync"

#### `get_code_examples`
Obtener ejemplos de código funcionales para escenarios específicos.

**Consultas de ejemplo:**
- "Mostrar código de ejemplo para captura de cámara RTSP"
- "Obtener fragmento de código para grabación MP4"
- "Ejemplo de aplicación de efectos de video"

### 2. **Herramientas de Guías de Implementación**

#### `list_deployment_guides`
Explorar guías de implementación disponibles filtradas por plataforma, tipo de proyecto o SDK.

**Consultas de ejemplo:**
- "Listar guías de implementación de Android"
- "Mostrar guías de implementación de MAUI"
- "Encontrar guías de implementación para Linux"

#### `get_deployment_guide`
Recuperar la guía de implementación completa para una plataforma o escenario específico.

**Consultas de ejemplo:**
- "Obtener la guía de implementación de Android"
- "Mostrar pasos de implementación para plataforma Uno"
- "Cómo implementar en macOS"

#### `get_nuget_packages_snippet`
Generar código `.csproj` listo para pegar con paquetes NuGet correctos para su plataforma.

**Consultas de ejemplo:**
- "Generar paquetes NuGet para proyecto MAUI Android"
- "Obtener referencias de paquetes para Avalonia en Windows"
- "Mostrar paquetes requeridos para iOS"

#### `get_platform_specific_config`
Obtener targets de MSBuild o código de configuración específico de plataforma.

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
3. Llamar a `get_code_examples` para escenarios RTSP
4. Proporcionarle código funcional:

```csharp
var pipeline = new MediaBlocksPipeline();
var rtspSource = new RTSPSourceBlock(new Uri("rtsp://camera.example.com:554/stream"));
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
3. Llamar a `get_api_item` con el ID de la clase
4. Analizar la documentación y explicar:
   - Parámetros del constructor
   - Formatos de archivo soportados
   - Opciones de configuración
   - Ejemplos de uso

## Mejores Prácticas

### 1. **Sea Específico en Sus Preguntas**

En lugar de preguntas genéricas, proporcione contexto:

- ❌ "¿Cómo capturo video?"
- ✅ "¿Cómo capturo video de una cámara RTSP usando MediaBlocks SDK en Android?"

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
3. **Reporte problemas** - Contacte al [soporte de VisioForge](https://support.visioforge.com/)

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
- **Herramientas**: 8 herramientas especializadas de documentación e implementación
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

- [Documentación del SDK de VisioForge](https://docs.visioforge.com/)
- [Especificación del Protocolo MCP](https://modelcontextprotocol.io/)
- [Documentación de Claude Code](https://claude.ai/code)

### Soporte

- [Portal de Soporte de VisioForge](https://support.visioforge.com/)
- [Comunidad Discord](https://discord.com/invite/yvXUG56WCH)
- [Muestras en GitHub](https://github.com/visioforge/.Net-SDK-s-samples)

### Guías Relacionadas

- [Guía de Instalación](../install/index.md)
- [Requisitos del Sistema](../system-requirements.md)
- [Guías de Implementación](../deployment-x/index.md)
- [Referencia API](https://api.visioforge.org/dotnet/api/index.html)

## Conclusión

El Servidor MCP de VisioForge transforma el desarrollo asistido por IA al proporcionar a su asistente de codificación acceso directo a documentación completa y actualizada del SDK. Ya sea que esté construyendo una aplicación de captura de video en Android, un reproductor multimedia en Windows o una herramienta de edición multiplataforma con Avalonia, el servidor MCP asegura que su asistente IA tenga el conocimiento para ayudarlo a tener éxito.

¡Conéctese hoy y experimente el futuro del desarrollo de SDK con IA!

---

**¿Necesita Ayuda?**

Si tiene preguntas sobre el uso del servidor MCP o encuentra algún problema, comuníquese con nuestro equipo de soporte:

- [Portal de Soporte](https://support.visioforge.com/)
- [Comunidad Discord](https://discord.com/invite/yvXUG56WCH)
- Correo electrónico: support@visioforge.com
