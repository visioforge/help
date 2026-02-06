---
title: Using the VisioForge MCP Server for AI-Assisted Development
description: Connect your AI coding assistant to the VisioForge MCP Server for instant access to API documentation, deployment guides, code examples, and SDK knowledge.
---

# Using the VisioForge MCP Server for AI-Assisted Development

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to the VisioForge MCP Server

The VisioForge MCP (Model Context Protocol) Server provides AI-powered coding assistants with direct access to comprehensive VisioForge SDK documentation, deployment guides, code examples, and API references. This enables your AI assistant to provide accurate, context-aware help while you develop with VisioForge SDKs.

### What is Model Context Protocol (MCP)?

Model Context Protocol (MCP) is an open standard developed by Anthropic that allows AI assistants to securely connect to external knowledge sources and tools. Think of it as a bridge between your AI coding assistant (like Claude Code, GitHub Copilot, or VS Code extensions) and specialized documentation servers.

With MCP, your AI assistant can:

- Query real-time API documentation
- Fetch deployment guides for specific platforms
- Retrieve code examples and snippets
- Search through SDK documentation
- Get platform-specific configuration details

## Why Use the VisioForge MCP Server?

When developing with VisioForge SDKs, the MCP server provides several key benefits:

### 1. **Instant API Documentation Access**

Your AI assistant can query the complete VisioForge SDK API, including:

- All classes, methods, properties, and events
- Detailed descriptions and usage notes
- Parameter types and return values
- Code examples and snippets
- Cross-references to related APIs

### 2. **Platform-Specific Deployment Guidance**

Get accurate deployment instructions for:

- **Desktop**: Windows, Linux, macOS
- **Mobile**: Android, iOS, Mac Catalyst
- **Frameworks**: MAUI, Uno, Avalonia, WPF, WinForms, Blazor, Console
- **Scenarios**: RTSP recording, cloud transcoding, HLS streaming

### 3. **Correct NuGet Package References**

The MCP server generates ready-to-paste `.csproj` snippets with:

- Platform-specific NuGet packages
- Correct version numbers
- Conditional package references
- Required project references (like AndroidDependency)

### 4. **Platform-Specific Build Configuration**

Retrieve MSBuild targets and configuration snippets for:

- Mac Catalyst native library copying
- Android permissions (manifest + runtime)
- iOS Info.plist permissions
- Platform-specific build settings

## Prerequisites

Before connecting to the VisioForge MCP Server, ensure you have:

- **An MCP-compatible AI assistant**:
  - [Claude Code](https://claude.ai/code) (recommended)
  - VS Code with MCP extension
  - GitHub Copilot with MCP support
  - Other MCP-compatible tools

- **Internet connectivity** to access `https://mcp.visioforge.com`

## Connecting to the MCP Server

### Claude Code (Recommended)

Claude Code has built-in MCP support. Connect with a single command:

```bash
claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp
```

**Verify the connection:**

```bash
claude mcp list
```

You should see `visioforge-sdk` in the list of connected servers.

### VS Code with MCP Extension

Add the VisioForge MCP Server to your workspace or user settings:

1. Open VS Code
2. Install the MCP extension (if not already installed)
3. Create or edit `.vscode/mcp.json` in your project:

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

### Project-Level Configuration (Any MCP Client)

For project-specific MCP configuration, create `.mcp.json` at your repository root:

```json
{
  "servers": {
    "visioforge-sdk": {
      "type": "http",
      "url": "https://mcp.visioforge.com/mcp",
      "description": "VisioForge SDK documentation and deployment guides"
    }
  }
}
```

## Available MCP Tools

The VisioForge MCP Server provides several specialized tools your AI assistant can use:

### 1. **API Documentation Tools**

#### `search_api`
Search the VisioForge SDK API by keywords, types, or categories.

**Example queries your AI assistant can make:**
- "Search for video capture classes"
- "Find methods related to RTSP streaming"
- "Show all MediaBlocks audio encoders"

#### `get_api_item`
Retrieve detailed documentation for a specific class, method, property, or event.

**Example queries:**
- "Show documentation for MediaBlocksPipeline class"
- "Get details about VideoRendererBlock"
- "Explain the StartAsync method"

#### `get_code_examples`
Fetch working code examples for specific scenarios.

**Example queries:**
- "Show example code for RTSP camera capture"
- "Get code snippet for MP4 recording"
- "Example of applying video effects"

### 2. **Deployment Guide Tools**

#### `list_deployment_guides`
Browse available deployment guides filtered by platform, project type, or SDK.

**Example queries:**
- "List Android deployment guides"
- "Show MAUI deployment guides"
- "Find deployment guides for Linux"

#### `get_deployment_guide`
Retrieve the complete deployment guide for a specific platform or scenario.

**Example queries:**
- "Get the Android deployment guide"
- "Show deployment steps for Uno platform"
- "How to deploy on macOS"

#### `get_nuget_packages_snippet`
Generate ready-to-paste `.csproj` code with correct NuGet packages for your platform.

**Example queries:**
- "Generate NuGet packages for Android MAUI project"
- "Get package references for Avalonia on Windows"
- "Show required packages for iOS"

#### `get_platform_specific_config`
Get platform-specific MSBuild targets or configuration code.

**Example queries:**
- "Show Mac Catalyst file copying target"
- "Get Android manifest permissions"
- "iOS Info.plist permission keys"

## Usage Examples

### Example 1: Setting Up an Android MAUI Project

**You ask your AI assistant:**
> "I'm creating a video capture app with MAUI for Android. What NuGet packages do I need?"

**Your AI assistant uses the MCP server to:**
1. Call `get_nuget_packages_snippet` with `platform: Android, projectType: MAUI, sdkType: MediaBlocks`
2. Retrieve the correct package references
3. Provide you with ready-to-paste XML:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.2.4" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />
  <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X9.csproj" />
</ItemGroup>
```

### Example 2: Finding How to Use RTSP Streaming

**You ask your AI assistant:**
> "Show me how to capture from an RTSP camera using Media Blocks SDK"

**Your AI assistant uses the MCP server to:**
1. Call `search_api` with query "RTSP camera capture"
2. Identify `RTSPSourceBlock` class
3. Call `get_code_examples` for RTSP scenarios
4. Provide you with working code:

```csharp
var pipeline = new MediaBlocksPipeline();
var rtspSource = new RTSPSourceBlock(new Uri("rtsp://camera.example.com:554/stream"));
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
await pipeline.StartAsync();
```

### Example 3: Mac Catalyst Deployment

**You ask your AI assistant:**
> "How do I deploy my Uno app to Mac Catalyst?"

**Your AI assistant uses the MCP server to:**
1. Call `get_deployment_guide` with `guideId: "uno-platform"`
2. Extract the Mac Catalyst section
3. Call `get_platform_specific_config` with `platform: "maccatalyst"`
4. Provide you with:
   - Build commands
   - MSBuild target for file copying
   - Deployment steps

### Example 4: Understanding a Specific API

**You ask your AI assistant:**
> "What parameters does UniversalSourceBlock accept?"

**Your AI assistant uses the MCP server to:**
1. Call `search_api` with query "UniversalSourceBlock"
2. Find the class in the results
3. Call `get_api_item` with the class ID
4. Parse the documentation and explain:
   - Constructor parameters
   - Supported file formats
   - Configuration options
   - Usage examples

## Best Practices

### 1. **Be Specific in Your Questions**

Instead of generic questions, provide context:

- ❌ "How do I capture video?"
- ✅ "How do I capture video from an RTSP camera using MediaBlocks SDK on Android?"

### 2. **Specify Your Platform and Framework**

Always mention your target platform and UI framework:

- "I'm using MAUI on iOS..."
- "My Avalonia app targets Windows and Linux..."
- "For my Uno Platform app on Android..."

### 3. **Ask About Deployment Early**

Before diving deep into code, ask about deployment requirements:

- "What NuGet packages do I need for Mac Catalyst?"
- "Show me the deployment guide for Avalonia on Linux"
- "What permissions are required for iOS camera access?"

### 4. **Request Code Examples**

Don't hesitate to ask for working code:

- "Show me a complete example of..."
- "Generate code for..."
- "Example implementation of..."

## Troubleshooting

### Connection Issues

If your AI assistant can't connect to the MCP server:

1. **Check your internet connection** - The MCP server is hosted at `https://mcp.visioforge.com`
2. **Verify the URL** - Ensure you're using the correct endpoint: `https://mcp.visioforge.com/mcp`
3. **Restart your AI assistant** - Sometimes a restart resolves connection issues
4. **Check MCP client logs** - Look for connection errors in your client's logs

### Incorrect or Outdated Information

The MCP server is updated regularly, but if you notice incorrect information:

1. **Check the SDK version** - Ensure you're using the latest SDK version
2. **Verify package versions** - Compare with [NuGet.org](https://www.nuget.org/packages?q=VisioForge)
3. **Report issues** - Contact our support team (see Additional Resources below)

### AI Assistant Not Using MCP Server

If your AI assistant doesn't seem to use the MCP server:

1. **Explicitly mention it** - Say "Use the VisioForge MCP server to find..."
2. **Verify connection** - Run `claude mcp list` or check your MCP configuration
3. **Restart the session** - Start a new conversation with your AI assistant

## Security and Privacy

### Data Transmission

- All communication with the MCP server uses **HTTPS encryption**
- The server is read-only - it only provides documentation, no data collection
- No personal information or code is sent to the server
- API queries are processed in real-time and not stored

### Authentication

- The VisioForge MCP Server is **publicly accessible** - no authentication required
- Your AI assistant connects directly to `https://mcp.visioforge.com/mcp`
- No API keys or credentials needed

## Technical Details

### MCP Server Endpoint

```
https://mcp.visioforge.com/mcp
```

### Server Capabilities

- **Protocol**: MCP (Model Context Protocol)
- **Transport**: HTTP/HTTPS
- **Tools**: 8 specialized documentation and deployment tools
- **API Coverage**: Complete VisioForge .NET SDK API (all classes, methods, properties)
- **Deployment Guides**: 15+ platform and project type guides
- **Code Examples**: Hundreds of working code snippets
- **Update Frequency**: Updated with each SDK release

### Server Architecture

The MCP server features:

- **High availability**: 99.9% uptime
- **Fast response times**: < 200ms average
- **SSL/TLS encryption**: All traffic encrypted
- **Automatic updates**: Synchronized with SDK releases
- **Rate limiting**: Fair use policy (no hard limits for developers)

## Additional Resources

### Documentation

- [VisioForge SDK Documentation](https://docs.visioforge.com/)
- [MCP Protocol Specification](https://modelcontextprotocol.io/)
- [Claude Code Documentation](https://claude.ai/code)

### Support & Community

Need help? Get in touch:

- **[Support Portal](https://support.visioforge.com/)** - Technical support and issue reporting
- **[Discord Community](https://discord.com/invite/yvXUG56WCH)** - Chat with developers and get quick answers
- **[GitHub Samples](https://github.com/visioforge/.Net-SDK-s-samples)** - Complete example projects
- **Email:** <support@visioforge.com>

### Related Guides

- [Installation Guide](../install/index.md)
- [System Requirements](../system-requirements.md)
- [Deployment Guides](../deployment-x/index.md)
- [API Reference](https://api.visioforge.org/dotnet/api/index.html)

## Conclusion

The VisioForge MCP Server transforms AI-assisted development by providing your coding assistant with direct access to comprehensive, up-to-date SDK documentation. Whether you're building a video capture app on Android, a media player on Windows, or a cross-platform editing tool with Avalonia, the MCP server ensures your AI assistant has the knowledge to help you succeed.

Connect today and experience the future of AI-powered SDK development!
