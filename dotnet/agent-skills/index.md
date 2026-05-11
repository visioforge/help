---
title: Agent Skills for VisioForge .NET SDKs — AI coding agents
description: Per-platform skills for AI coding agents (Claude Code, Cursor, Copilot, Codex, Gemini CLI) covering NuGet, licence setup, and deployment caveats.
---

# Agent Skills for VisioForge .NET SDKs

VisioForge publishes per-platform **Agent Skills** at `https://www.visioforge.com/.well-known/agent-skills/index.json` per the [Agent Skills Discovery RFC v0.2.0](https://github.com/cloudflare/agent-skills-discovery-rfc).

## What is this?

A **Skill** is a small, self-contained package of procedural knowledge an AI coding agent can load on demand: the NuGet packages to add, the license registration code, the project-file caveats, and the deployment failure modes a developer hits in practice. Skills are loaded automatically by skills-aware agents — no installation step on your side.

When a developer prompts an agent (Claude Code, Cursor, GitHub Copilot, OpenAI Codex, Gemini CLI, OpenCode, Goose, Junie, …) with something like *"add VisioForge video capture to this WPF app"*, the agent inspects the discovery index, finds the matching skill, downloads the archive, and follows the bundled instructions. The result is a setup that uses the *current* package versions and avoids the deployment gotchas the docs flag.

## Discovery surface

The well-known endpoints and HTML/HTTP signals are emitted by the marketing site at `www.visioforge.com` (the host that serves `/.well-known/agent-skills/`); the help site you're reading this on (`help.visioforge.com`) doesn't currently inject the discovery `<link>` or `Link:` header. Agents discovering us from a help-site page should follow the inline link to `https://www.visioforge.com/.well-known/agent-skills/index.json` directly.

| Endpoint (served by `www.visioforge.com`) | Purpose |
|---|---|
| `/.well-known/agent-skills/index.json` | Index of every published skill with name, description, archive URL, and SHA-256 digest. |
| `/.well-known/agent-skills/<skill-name>.zip` | Archive containing `SKILL.md` + bundled `references/` (sample `.csproj`, init code). |
| `<link rel="agent-skills">` in every `www.visioforge.com` page `<head>` | HTML signal pointing at the index. |
| `Link: <…>; rel="agent-skills"` HTTP header on `www.visioforge.com` | Same signal for non-HTML responses (Markdown, etc.). |

Skills are **discoverable from any page of `www.visioforge.com`** — agents don't need to know the well-known URL up front.

## Available skills

42 skills cover every (SDK, platform) intersection that has a working sample in [github.com/visioforge/.Net-SDK-s-samples](https://github.com/visioforge/.Net-SDK-s-samples). The authoritative list — including each skill's name, description, archive URL, and SHA-256 digest — lives in the discovery index at `https://www.visioforge.com/.well-known/agent-skills/index.json`. Fetch it once and pick the entries whose `description` matches your task; the discovery index is intentionally not duplicated here to avoid drift.

The skill families currently published:

- **Video Capture SDK .NET** (`video-capture-sdk-net-*`) — legacy Windows-only DirectShow stack. Hosts: WPF, WinForms, WinUI 3, console.
- **Video Capture SDK X** (`video-capture-sdk-x-*`) — cross-platform "X" edition. Hosts: WPF, WinForms, WinUI 3, MAUI, Avalonia, Uno, native Android, native iOS, native macOS.
- **Media Player SDK .NET** (`media-player-sdk-net-*`) — legacy Windows-only player. Hosts: WPF, WinForms, WinUI 3.
- **Media Player SDK X** (`media-player-sdk-x-*`) — cross-platform player. Hosts: WPF, WinForms, WinUI 3, MAUI, Avalonia, Uno, native Android, native iOS, native macOS.
- **Video Edit SDK .NET** (`video-edit-sdk-net-*`) — legacy Windows-only non-linear editor. Hosts: WPF, WinForms, console.
- **Video Edit SDK X** (`video-edit-sdk-x-*`) — cross-platform editor. Hosts: WPF, WinForms, Avalonia, console.
- **Media Blocks SDK .NET** (`media-blocks-sdk-net-*`) — graph-based pipeline SDK. Hosts: WPF, WinForms, MAUI, Avalonia, Blazor Server, Uno, console, native Android, native iOS, native macOS.

Naming convention: `<sdk-family>-<host>` where `<host>` is the per-platform UI shell or hosting model. See the discovery index for the live, exact set.

## How agents pick up a skill

1. Agent crawls a page on `www.visioforge.com`, sees `<link rel="agent-skills" href="/.well-known/agent-skills/index.json">` (or the same URL in the `Link` HTTP header for Markdown responses). Agents arriving via `help.visioforge.com` should follow the inline `https://www.visioforge.com/.well-known/agent-skills/index.json` link directly — the help site doesn't emit those discovery signals.
2. Agent fetches the index, reads the `skills[]` array, picks the entries whose `description` matches the user's task.
3. Agent downloads the matching `.zip`, verifies the `sha256:` digest, unpacks `SKILL.md` and `references/`, and follows the procedural instructions.

## Reading a skill yourself

A skill archive is a plain zip. Unpack it and you'll find:

- `SKILL.md` — frontmatter (`name`, `description`) followed by procedural prose: when to use the skill, NuGet packages, project setup, license registration, common pitfalls.
- `references/Sample.csproj` — minimal, working csproj for the skill's target host (WPF, WinForms, MAUI, Console, …) — each per-platform skill ships a csproj specific to that host (e.g. the WPF skill csproj sets `Microsoft.NET.Sdk.WindowsDesktop` + `<UseWPF>true</UseWPF>`).
- `references/<init-source>.cs` — the initialization pattern from a real official sample. WPF skills also bundle the matching `.xaml`, `App.xaml(.cs)`, and any required resources so `dotnet build` succeeds against the bundle as-is.

The same `SKILL.md` is also rendered as a normal documentation page in this section — see the per-skill subsections above.

## Maintenance

Skills track the current public NuGet release. The version pinned in each `SKILL.md` and `references/Sample.csproj` matches the version in the official samples on [github.com/visioforge/.Net-SDK-s-samples](https://github.com/visioforge/.Net-SDK-s-samples). When a major SDK version ships, every skill is updated and the discovery index sha256 digests are recomputed during the visioforge.com build.

## Related discovery surfaces

VisioForge already exposes three other AI-agent surfaces — Agent Skills is the fourth and complements them:

| Surface | Shape | Best for |
|---|---|---|
| `mcp.visioforge.com/mcp` (via `/.well-known/mcp.json` + `/.well-known/mcp-server-card`) | Live MCP server with 14 read-only doc tools | Querying the SDK API surface, looking up media blocks, fetching code examples |
| `/llms.txt`, `/llms-full.txt` | LLM-friendly docs index | Bulk ingestion of the docs corpus |
| `<meta name="x-webmcp-tools">` + WebMCP runtime | 10 in-page tools registered via `navigator.modelContext.provideContext()` | Agent acting in the user's current browser tab (locale switch, page outline, copy code, …) |
| **`/.well-known/agent-skills/index.json`** | **Per-(SDK, platform) procedural skill archives** | **Bootstrapping a new project that uses one of our SDKs** |
