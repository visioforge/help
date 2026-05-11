---
title: Using the VisioForge MCP Server for AI-Assisted Development
description: Connect your AI coding assistant to the VisioForge MCP Server for instant access to API documentation, deployment guides, code examples, and SDK knowledge.
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

The VisioForge MCP Server exposes 14 specialized tools your AI assistant can use. Names and descriptions match the live `tools/list` response from `https://mcp.visioforge.com/mcp` exactly.

### 1. **Media Blocks Tools**

#### `list_media_blocks`
List available VisioForge media blocks, optionally filtered by category. Media blocks are the building blocks of media processing pipelines. Categories include: Sources, Sinks, VideoEncoders, AudioEncoders, VideoDecoders, VideoProcessing, AudioProcessing, AudioRendering, VideoRendering, Demuxers, Parsers, OpenGL, OpenCV, Nvidia, Decklink, AWS, RTSPServer, Bridge, Special, Outputs.

**Example queries:**
- "List all video encoder blocks"
- "Show MediaBlocks sources"
- "What blocks are in the OpenCV category?"

#### `get_media_block_info`
Get detailed information about a specific media block including its properties, methods, events, input/output pads, constructor parameters, and documentation. Use this to understand how to configure and use a specific media block in a pipeline.

**Example queries:**
- "Get info on RTSPSourceBlock"
- "Show pads and properties of H264EncoderBlock"
- "What constructor parameters does VideoRendererBlock take?"

#### `get_pipeline_template`
Get a media block pipeline template for a specific use case. Returns the list of required blocks and how to connect them, along with C# code to build the pipeline.

**Example queries:**
- "Pipeline template for RTSP-to-MP4 recording"
- "Template for screen capture with audio"
- "Pipeline for HLS streaming"

### 2. **SDK Class & API Tools**

#### `list_sdk_classes`
List core VisioForge SDK classes. These are the main entry-point classes for building media applications: VideoCaptureCoreX (video capture/recording), VideoEditCoreX (video editing), MediaPlayerCoreX (media playback), MediaInfoReaderCoreX (media analysis), SimplePlayerCoreX (simple playback), and more.

**Example queries:**
- "List all core SDK classes"
- "Show top-level entry-point classes"

#### `get_class_info`
Get detailed information about any VisioForge SDK class, including its full list of properties, methods, events, constructors, base class, interfaces, and documentation. Works for both core SDK classes and media blocks.

**Example queries:**
- "Show documentation for MediaBlocksPipeline class"
- "Get details about VideoCaptureCoreX"
- "What events does MediaPlayerCoreX expose?"

#### `get_method_signature`
Get the detailed signature and documentation for a specific method on a class. Useful when you need to understand a method's parameters, return type, and behavior.

**Example queries:**
- "Signature of StartAsync on MediaBlocksPipeline"
- "What parameters does Connect take?"

#### `search_api`
Search across the entire VisioForge SDK API — class names, method names, property names, event names, and their documentation text. Returns ranked results. Use this when you don't know the exact class name, or to find all classes related to a concept (e.g., "RTSP streaming", "video overlay", "audio capture").

**Example queries:**
- "Search for video capture classes"
- "Find methods related to RTSP streaming"
- "Show all MediaBlocks audio encoders"

#### `get_enum_values`
Get all values of a VisioForge SDK enum type with descriptions. Useful for understanding available options for configuration properties (e.g., MediaBlockType, video codecs, audio formats, pixel formats).

**Example queries:**
- "List values of VideoCodec enum"
- "Show MediaBlockType enum values"

#### `list_namespaces`
Browse VisioForge SDK namespaces hierarchically. Shows child namespaces and classes within a given namespace. Start with `VisioForge.Core` or leave empty to see top-level namespaces.

**Example queries:**
- "List top-level namespaces"
- "Show classes in VisioForge.Core.MediaBlocks"

#### `get_code_example`
Get a code example for a common VisioForge SDK scenario. Returns complete, working C# code snippets that demonstrate how to use the SDK for tasks like video capture, RTSP streaming, media playback, and more.

**Example queries:**
- "Code example for RTSP camera capture"
- "Show MP4 recording snippet"
- "Example of applying video effects"

### 3. **Deployment Guide Tools**

#### `list_deployment_guides`
List available deployment guides for VisioForge SDK. Filter by platform, project type, SDK type, or scenario. Returns a list of guides with titles, summaries, and tags.

**Example queries:**
- "List Android deployment guides"
- "Show MAUI deployment guides"
- "Find deployment guides for Linux"

#### `get_deployment_guide`
Get the complete deployment guide for a specific scenario. Returns detailed instructions, code snippets, NuGet packages, and platform-specific notes.

**Example queries:**
- "Get the Android deployment guide"
- "Show deployment steps for Uno platform"
- "How to deploy on macOS"

#### `get_nuget_packages_snippet`
Get .csproj snippet with required NuGet packages for a specific deployment scenario. Returns XML snippet ready to copy-paste into your project file.

**Example queries:**
- "Generate NuGet packages for Android MAUI project"
- "Get package references for Avalonia on Windows"
- "Show required packages for iOS"

#### `get_platform_specific_config`
Get platform-specific file copying/build configuration code. Returns MSBuild targets or post-build scripts for special deployment requirements.

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
3. Call `get_code_example` for RTSP scenarios
4. Provide you with working code:

```csharp
var pipeline = new MediaBlocksPipeline();

// RTSPSourceBlock takes RTSPSourceSettings (not a Uri directly).
// Build the settings via the async factory — the ctor is private.
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
3. Call `get_class_info` with the class name
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
- **Tools**: 14 specialized documentation and deployment tools
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

- [VisioForge SDK Documentation](https://www.visioforge.com/help/)
- [MCP Protocol Specification](https://modelcontextprotocol.io/specification)
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
