---
title: Agent Skills pour les SDK .NET VisioForge — agents IA et LLM
description: Skills par plateforme pour les agents IA de codage (Claude Code, Cursor, Copilot, Codex, Gemini CLI) couvrant NuGet, licence et pièges de déploiement.
---

# Agent Skills pour les SDK .NET VisioForge

VisioForge publie des **Agent Skills** par plateforme à l'adresse `https://www.visioforge.com/.well-known/agent-skills/index.json` conformément au [RFC v0.2.0 de découverte d'Agent Skills](https://github.com/cloudflare/agent-skills-discovery-rfc).

## De quoi s'agit-il ?

Un **Skill** est un petit paquet autonome de connaissances procédurales qu'un agent IA de codage peut charger à la demande : les paquets NuGet à ajouter, le code d'enregistrement de la licence, les particularités du fichier de projet et les modes de défaillance de déploiement qu'un développeur rencontre en pratique. Les Skills sont chargés automatiquement par les agents qui les prennent en charge — aucune étape d'installation de votre côté.

Lorsqu'un développeur s'adresse à un agent (Claude Code, Cursor, GitHub Copilot, OpenAI Codex, Gemini CLI, OpenCode, Goose, Junie, …) avec une demande du type *« ajoute la capture vidéo VisioForge à cette application WPF »*, l'agent inspecte l'index de découverte, trouve le skill correspondant, télécharge l'archive et suit les instructions fournies. Le résultat est une configuration qui utilise les versions *actuelles* des paquets et évite les pièges de déploiement signalés dans la documentation.

## Surface de découverte

Les points d'accès well-known et les signaux HTML/HTTP sont émis par le site marketing à `www.visioforge.com` (l'hôte qui sert `/.well-known/agent-skills/`) ; le site d'aide sur lequel vous lisez ceci (`help.visioforge.com`) n'injecte actuellement pas le `<link>` ni l'en-tête `Link:` de découverte. Les agents qui nous découvrent depuis une page du site d'aide doivent suivre directement le lien intégré vers `https://www.visioforge.com/.well-known/agent-skills/index.json`.

| Point d'accès (servi par `www.visioforge.com`) | Rôle |
|---|---|
| `/.well-known/agent-skills/index.json` | Index de tous les skills publiés, avec nom, description, URL de l'archive et empreinte SHA-256. |
| `/.well-known/agent-skills/<skill-name>.zip` | Archive contenant `SKILL.md` et le dossier `references/` (csproj d'exemple, code d'initialisation). |
| `<link rel="agent-skills">` dans le `<head>` de chaque page `www.visioforge.com` | Signal HTML pointant vers l'index. |
| En-tête HTTP `Link: <…>; rel="agent-skills"` sur `www.visioforge.com` | Même signal pour les réponses non HTML (Markdown, etc.). |

Les Skills sont **découvrables depuis toute page de `www.visioforge.com`** — les agents n'ont pas besoin de connaître l'URL well-known à l'avance.

## Skills disponibles

42 skills couvrent chaque intersection (SDK, plateforme) pour laquelle il existe un exemple fonctionnel dans [github.com/visioforge/.Net-SDK-s-samples](https://github.com/visioforge/.Net-SDK-s-samples). La liste de référence — comprenant le nom, la description, l'URL d'archive et l'empreinte SHA-256 de chaque skill — se trouve dans l'index de découverte à `https://www.visioforge.com/.well-known/agent-skills/index.json`. Récupérez-le une fois et sélectionnez les entrées dont la `description` correspond à votre tâche ; l'index de découverte n'est intentionnellement pas dupliqué ici pour éviter toute dérive.

Les familles de skills actuellement publiées :

- **Video Capture SDK .NET** (`video-capture-sdk-net-*`) — pile DirectShow Windows uniquement (héritée). Hôtes : WPF, WinForms, WinUI 3, console.
- **Video Capture SDK X** (`video-capture-sdk-x-*`) — édition « X » multiplateforme. Hôtes : WPF, WinForms, WinUI 3, MAUI, Avalonia, Uno, Android natif, iOS natif, macOS natif.
- **Media Player SDK .NET** (`media-player-sdk-net-*`) — lecteur Windows uniquement (hérité). Hôtes : WPF, WinForms, WinUI 3.
- **Media Player SDK X** (`media-player-sdk-x-*`) — lecteur multiplateforme. Hôtes : WPF, WinForms, WinUI 3, MAUI, Avalonia, Uno, Android natif, iOS natif, macOS natif.
- **Video Edit SDK .NET** (`video-edit-sdk-net-*`) — éditeur non linéaire Windows uniquement (hérité). Hôtes : WPF, WinForms, console.
- **Video Edit SDK X** (`video-edit-sdk-x-*`) — éditeur multiplateforme. Hôtes : WPF, WinForms, Avalonia, console.
- **Media Blocks SDK .NET** (`media-blocks-sdk-net-*`) — SDK de pipeline basé sur un graphe. Hôtes : WPF, WinForms, MAUI, Avalonia, Blazor Server, Uno, console, Android natif, iOS natif, macOS natif.

Convention de nommage : `<sdk-family>-<host>` où `<host>` est la coque d'interface utilisateur par plateforme ou le modèle d'hébergement. Consultez l'index de découverte pour l'ensemble exact à jour.

## Comment les agents adoptent un skill

1. L'agent explore une page sur `www.visioforge.com`, voit `<link rel="agent-skills" href="/.well-known/agent-skills/index.json">` (ou la même URL dans l'en-tête HTTP `Link` pour les réponses Markdown). Les agents arrivant via `help.visioforge.com` doivent suivre directement le lien intégré `https://www.visioforge.com/.well-known/agent-skills/index.json` — le site d'aide n'émet pas ces signaux de découverte.
2. L'agent récupère l'index, lit le tableau `skills[]` et sélectionne les entrées dont la `description` correspond à la tâche de l'utilisateur.
3. L'agent télécharge l'archive `.zip` correspondante, vérifie l'empreinte `sha256:`, décompresse `SKILL.md` et `references/`, puis suit les instructions procédurales.

## Lire un skill vous-même

Une archive de skill est un simple zip. Décompressez-la et vous trouverez :

- `SKILL.md` — frontmatter (`name`, `description`) suivi de prose procédurale : quand utiliser le skill, paquets NuGet, configuration du projet, enregistrement de la licence, pièges courants.
- `references/Sample.csproj` — csproj minimal et fonctionnel pour l'hôte cible du skill (WPF, WinForms, MAUI, Console, …) — chaque skill par plateforme livre un csproj spécifique à cet hôte (par exemple, le csproj du skill WPF définit `Microsoft.NET.Sdk.WindowsDesktop` + `<UseWPF>true</UseWPF>`).
- `references/<init-source>.cs` — le schéma d'initialisation tiré d'un véritable exemple officiel. Les skills WPF intègrent également le fichier `.xaml` correspondant, `App.xaml(.cs)` et toutes les ressources nécessaires pour que `dotnet build` réussisse contre le paquet tel quel.

Le même `SKILL.md` est également rendu sous forme de page de documentation normale dans cette section — voir les sous-sections par skill ci-dessus.

## Maintenance

Les skills suivent la version NuGet publique courante. La version épinglée dans chaque `SKILL.md` et `references/Sample.csproj` correspond à la version des exemples officiels sur [github.com/visioforge/.Net-SDK-s-samples](https://github.com/visioforge/.Net-SDK-s-samples). Quand une version majeure du SDK est publiée, tous les skills sont mis à jour et les empreintes sha256 de l'index de découverte sont recalculées lors de la build de visioforge.com.

## Surfaces de découverte associées

VisioForge expose déjà trois autres surfaces destinées aux agents IA — Agent Skills est la quatrième et les complète :

| Surface | Forme | Idéal pour |
|---|---|---|
| `mcp.visioforge.com/mcp` (via `/.well-known/mcp.json` + `/.well-known/mcp-server-card`) | Serveur MCP en direct avec 14 outils de documentation en lecture seule | Interroger la surface API du SDK, rechercher des media blocks, récupérer des exemples de code |
| `/llms.txt`, `/llms-full.txt` | Index de documentation adapté aux LLM | Ingestion en masse du corpus documentaire |
| `<meta name="x-webmcp-tools">` + runtime WebMCP | 10 outils intégrés à la page, enregistrés via `navigator.modelContext.provideContext()` | Agent agissant dans l'onglet de navigateur actuel de l'utilisateur (changement de langue, plan de la page, copie de code, …) |
| **`/.well-known/agent-skills/index.json`** | **Archives de skills procéduraux par (SDK, plateforme)** | **Démarrer un nouveau projet utilisant l'un de nos SDK** |
