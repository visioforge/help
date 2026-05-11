---
title: Agent Skills para los SDKs de VisioForge .NET — agentes IA
description: Skills por plataforma para agentes de IA (Claude Code, Cursor, Copilot, Codex, Gemini CLI) — paquetes NuGet, licencia y particularidades de despliegue.
---

# Agent Skills para los SDKs de VisioForge .NET

VisioForge publica **Agent Skills** por plataforma en `https://www.visioforge.com/.well-known/agent-skills/index.json` según el [Agent Skills Discovery RFC v0.2.0](https://github.com/cloudflare/agent-skills-discovery-rfc).

## ¿Qué es esto?

Un **Skill** es un paquete pequeño y autocontenido de conocimiento procedimental que un agente de IA puede cargar bajo demanda: los paquetes NuGet a añadir, el código de registro de licencia, las particularidades del archivo de proyecto y los modos de fallo de despliegue que un desarrollador encuentra en la práctica. Los skills se cargan automáticamente por agentes compatibles — no requiere instalación de tu parte.

Cuando un desarrollador le pide al agente (Claude Code, Cursor, GitHub Copilot, OpenAI Codex, Gemini CLI, OpenCode, Goose, Junie, …) algo como *"añade captura de video VisioForge a esta app WPF"*, el agente inspecciona el índice de descubrimiento, encuentra el skill correspondiente, descarga el archivo y sigue las instrucciones empaquetadas. El resultado es una configuración que usa las versiones *actuales* de los paquetes y evita las trampas de despliegue que documentamos.

## Superficie de descubrimiento

Los endpoints bien-conocidos y las señales HTML/HTTP los emite el sitio de marketing `www.visioforge.com` (el host que sirve `/.well-known/agent-skills/`); el sitio de ayuda donde estás leyendo esto (`help.visioforge.com`) actualmente no inyecta el `<link>` ni la cabecera `Link:` de descubrimiento. Los agentes que nos descubran desde una página del sitio de ayuda deberían seguir directamente el enlace inline a `https://www.visioforge.com/.well-known/agent-skills/index.json`.

| Endpoint (servido por `www.visioforge.com`) | Propósito |
|---|---|
| `/.well-known/agent-skills/index.json` | Índice de cada skill publicado con nombre, descripción, URL del archivo y digest SHA-256. |
| `/.well-known/agent-skills/<skill-name>.zip` | Archivo con `SKILL.md` + `references/` (csproj de muestra, código init). |
| `<link rel="agent-skills">` en `<head>` de cada página de `www.visioforge.com` | Señal HTML al índice. |
| `Link: <…>; rel="agent-skills"` cabecera HTTP en `www.visioforge.com` | Misma señal para respuestas no-HTML (Markdown, etc.). |

Los skills son **descubribles desde cualquier página de `www.visioforge.com`** — los agentes no necesitan conocer la URL bien-conocida previamente.

## Skills disponibles

42 skills cubren cada intersección (SDK, plataforma) que tiene un sample funcional en [github.com/visioforge/.Net-SDK-s-samples](https://github.com/visioforge/.Net-SDK-s-samples). La lista autoritativa — con nombre, descripción, URL del archivo y digest SHA-256 de cada skill — vive en el índice de descubrimiento `https://www.visioforge.com/.well-known/agent-skills/index.json`. Recupéralo una vez y elige las entradas cuya `description` coincide con tu tarea; deliberadamente no duplicamos la lista aquí para evitar el desfase.

Las familias de skills publicadas actualmente:

- **Video Capture SDK .NET** (`video-capture-sdk-net-*`) — stack legacy Windows-only basado en DirectShow. Hosts: WPF, WinForms, WinUI 3, consola.
- **Video Capture SDK X** (`video-capture-sdk-x-*`) — edición "X" multiplataforma. Hosts: WPF, WinForms, WinUI 3, MAUI, Avalonia, Uno, Android nativo, iOS nativo, macOS nativo.
- **Media Player SDK .NET** (`media-player-sdk-net-*`) — reproductor legacy Windows-only. Hosts: WPF, WinForms, WinUI 3.
- **Media Player SDK X** (`media-player-sdk-x-*`) — reproductor multiplataforma. Hosts: WPF, WinForms, WinUI 3, MAUI, Avalonia, Uno, Android nativo, iOS nativo, macOS nativo.
- **Video Edit SDK .NET** (`video-edit-sdk-net-*`) — editor no-lineal legacy Windows-only. Hosts: WPF, WinForms, consola.
- **Video Edit SDK X** (`video-edit-sdk-x-*`) — editor multiplataforma. Hosts: WPF, WinForms, Avalonia, consola.
- **Media Blocks SDK .NET** (`media-blocks-sdk-net-*`) — SDK de pipelines basados en grafos. Hosts: WPF, WinForms, MAUI, Avalonia, Blazor Server, Uno, consola, Android nativo, iOS nativo, macOS nativo.

Convención de nombres: `<sdk-family>-<host>`, donde `<host>` es el shell UI por plataforma o el modelo de hosting. Consulta el índice de descubrimiento para el conjunto exacto y vigente.

## Cómo recoge un agente un skill

1. El agente rastrea una página en `www.visioforge.com`, ve `<link rel="agent-skills" href="/.well-known/agent-skills/index.json">` (o la misma URL en la cabecera HTTP `Link` para respuestas Markdown). Los agentes que llegan vía `help.visioforge.com` deberían seguir directamente el enlace inline a `https://www.visioforge.com/.well-known/agent-skills/index.json` — el sitio de ayuda no emite esas señales de descubrimiento.
2. El agente recupera el índice, lee el array `skills[]`, escoge las entradas cuya `description` coincide con la tarea del usuario.
3. El agente descarga el `.zip` correspondiente, verifica el digest `sha256:`, descomprime `SKILL.md` y `references/` y sigue las instrucciones procedimentales.

## Leer un skill tú mismo

Un archivo skill es un zip plano. Descomprímelo y encontrarás:

- `SKILL.md` — frontmatter (`name`, `description`) seguido de prosa procedimental: cuándo usar el skill, paquetes NuGet, configuración del proyecto, registro de licencia, errores comunes.
- `references/Sample.csproj` — csproj mínimo y funcional para el host objetivo del skill (WPF, WinForms, MAUI, Console, …) — cada skill por plataforma incluye un csproj específico para ese host (p. ej. el csproj del skill WPF establece `Microsoft.NET.Sdk.WindowsDesktop` + `<UseWPF>true</UseWPF>`).
- `references/<init-source>.cs` — el patrón de inicialización de un sample oficial real. Los skills WPF también incluyen el `.xaml` correspondiente, `App.xaml(.cs)` y cualquier recurso necesario para que `dotnet build` tenga éxito sobre el bundle tal cual.

El mismo `SKILL.md` también se renderiza como página normal de documentación en esta sección — ver las subsecciones por skill arriba.

## Mantenimiento

Los skills siguen la release pública actual de NuGet. La versión fijada en cada `SKILL.md` y `references/Sample.csproj` coincide con la versión de los samples oficiales en [github.com/visioforge/.Net-SDK-s-samples](https://github.com/visioforge/.Net-SDK-s-samples). Cuando se libera una versión mayor del SDK, cada skill se actualiza y los digests sha256 del índice de descubrimiento se recomputan durante el build de visioforge.com.

## Superficies de descubrimiento relacionadas

VisioForge ya expone tres superficies para agentes de IA — Agent Skills es la cuarta y complementa las demás:

| Superficie | Forma | Mejor para |
|---|---|---|
| `mcp.visioforge.com/mcp` (vía `/.well-known/mcp.json` + `/.well-known/mcp-server-card`) | Servidor MCP en vivo con 14 herramientas read-only | Consultar la API del SDK, buscar media blocks, obtener ejemplos de código |
| `/llms.txt`, `/llms-full.txt` | Índice de docs amigable a LLMs | Ingesta masiva del corpus de documentación |
| `<meta name="x-webmcp-tools">` + runtime WebMCP | 10 herramientas in-page registradas vía `navigator.modelContext.provideContext()` | Agente actuando en la pestaña actual del usuario (cambio de idioma, esquema de página, copiar código, …) |
| **`/.well-known/agent-skills/index.json`** | **Skills procedimentales por (SDK, plataforma) en archivos** | **Bootstrap de un proyecto nuevo que usa uno de nuestros SDKs** |
