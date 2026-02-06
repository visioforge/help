---
title: Integración de SDKs con Uno Platform
description: Desarrolle aplicaciones multiplataforma Uno Platform con capacidades multimedia para Windows, Android, iOS, macOS y Linux usando los SDKs de VisioForge.
---

# Integración de SDKs de VisioForge con Aplicaciones Uno Platform

## Descripción General

Uno Platform es un potente framework de UI multiplataforma que permite a los desarrolladores crear aplicaciones nativas para móviles, escritorio y embebidas desde una única base de código C# y XAML. VisioForge proporciona soporte completo para aplicaciones Uno Platform a través del paquete `VisioForge.DotNet.Core.UI.Uno`, que contiene controles de UI especializados diseñados específicamente para Uno Platform.

Nuestros SDKs permiten potentes capacidades multimedia en todas las plataformas soportadas por Uno Platform:

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Plataformas Soportadas

Uno Platform con SDKs de VisioForge soporta:

- **Windows Desktop** - Soporte nativo completo de WinUI 3 con aceleración por hardware
- **Android** - Vistas nativas de Android con aceleración por hardware MediaCodec
- **iOS** - UIKit nativo con aceleración por hardware VideoToolbox
- **macOS** - Soporte Mac Catalyst para Apple Silicon e Intel
- **Linux Desktop** - Renderizado basado en Skia con GStreamer

## Inicio Rápido

### 1. Instalar Paquetes NuGet

Agregue los paquetes principales de VisioForge a su proyecto Uno Platform:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2025.12.9" />
  <PackageReference Include="VisioForge.DotNet.Core" Version="2025.4.10" />
</ItemGroup>
```

### 2. Inicializar el SDK

Inicialice el SDK al iniciar la aplicación. Puede usar la versión síncrona o asíncrona:

```csharp
using VisioForge.Core;

// Inicialización síncrona
VisioForgeX.InitSDK();

// O inicialización asíncrona (recomendada)
await VisioForgeX.InitSDKAsync();
```

Limpie cuando la aplicación termine:

```csharp
VisioForgeX.DestroySDK();
```

### 3. Agregar VideoView al XAML

Agregue el namespace de VisioForge y el control VideoView a su XAML:

```xml
<Page x:Class="YourApp.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:vf="using:VisioForge.Core.UI.Uno">
    
    <Grid>
        <vf:VideoView x:Name="videoView"               
                      HorizontalAlignment="Stretch"
                      VerticalAlignment="Stretch"
                      Background="Black"/>
    </Grid>
</Page>
```

El control VideoView se adapta a las capacidades de renderizado nativas de cada plataforma mientras proporciona una API consistente para el código de su aplicación.

## Implementación de VideoView en Código

Aquí hay un ejemplo completo de uso de VideoView con el pipeline de Media Blocks:

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

public sealed partial class MainPage : Page
{
    private MediaBlocksPipeline? _pipeline;
    private UniversalSourceBlock? _source;
    private VideoRendererBlock? _renderer;
    
    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
        this.Unloaded += MainPage_Unloaded;
    }
    
    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Inicializar SDK (una vez al iniciar la aplicación)
        await VisioForgeX.InitSDKAsync();
    }
    
    private async Task PlayMediaAsync(string mediaPath)
    {
        // Limpiar pipeline anterior si existe
        await CleanupPipelineAsync();
        
        // Crear pipeline
        _pipeline = new MediaBlocksPipeline();
        
        // Crear configuración de fuente - usar método factory para URLs o rutas de archivo
        // Reproducción solo de video: deshabilitar audio ya que no conectamos la salida de audio
        UniversalSourceSettings sourceSettings;
        if (Uri.TryCreate(mediaPath, UriKind.Absolute, out var uri) && !uri.IsFile)
        {
            // Fuente URL
            sourceSettings = await UniversalSourceSettings.CreateAsync(uri, renderAudio: false);
        }
        else
        {
            // Fuente de archivo
            sourceSettings = await UniversalSourceSettings.CreateAsync(mediaPath, renderAudio: false);
        }
        
        // Crear bloques de fuente y renderizador
        _source = new UniversalSourceBlock(sourceSettings);
        _renderer = new VideoRendererBlock(_pipeline, videoView);
        
        // Conectar bloques
        _pipeline.Connect(_source.VideoOutput, _renderer.Input);
        
        // Iniciar reproducción
        await _pipeline.StartAsync();
    }
    
    private async Task CleanupPipelineAsync()
    {
        if (_pipeline != null)
        {
            await _pipeline.StopAsync();
            await _pipeline.DisposeAsync();
            _pipeline = null;
        }
        
        _source?.Dispose();
        _source = null;
        
        _renderer?.Dispose();
        _renderer = null;
    }
    
    private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
    {
        await CleanupPipelineAsync();
        
        // Destruir SDK cuando la aplicación cierra
        VisioForgeX.DestroySDK();
    }
}
```

## Aplicaciones de Ejemplo

Para ejemplos completos y código de muestra, visite:

- Aplicaciones de ejemplo en la carpeta `_DEMOS/Media Blocks SDK/Uno/`
- Aplicaciones de ejemplo en la carpeta `_DEMOS/Media Player SDK X/Uno/`
- Aplicaciones de ejemplo en la carpeta `_DEMOS/Video Capture SDK X/Uno/`
- Nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples)

Los ejemplos disponibles incluyen:

- **Simple Player** - Reproductor de medios básico con controles de reproducción
- **RTSP Viewer** - Visor de transmisión de cámaras de red
- **Video Capture** - Captura de cámara con funcionalidad de grabación

## Próximos Pasos

Para instrucciones completas de despliegue incluyendo:

- Paquetes redistributables específicos de plataforma
- Requisitos del sistema
- Comandos de compilación
- Configuraciones de plataforma (permisos, capacidades, configuraciones de Info.plist)
- Archivo de proyecto de ejemplo completo
- Solución de problemas

Consulte la [Guía de Despliegue de Uno Platform](../deployment-x/uno.es.md).

## Recursos Adicionales

- [Despliegue en Windows](../deployment-x/Windows.es.md)
- [Despliegue en Android](../deployment-x/Android.es.md)
- [Despliegue en iOS](../deployment-x/iOS.es.md)
- [Despliegue en macOS](../deployment-x/macOS.es.md)
