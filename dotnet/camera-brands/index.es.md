---
title: URLs RTSP de Cámaras IP: Directorio de 62 Marcas en C#
description: Directorio completo de URLs RTSP para 62 marcas de cámaras IP. Conecta Hikvision, Dahua, Axis, Uniview, EZVIZ, Wisenet, Arlo y más usando VisioForge .NET SDK.
---

# Guía de Conexión de Cámaras IP por Marca

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Conectar cámaras IP en C# .NET es sencillo cuando conoces el patrón de URL RTSP correcto para tu marca de cámara. Cada fabricante utiliza formatos de URL, puertos y métodos de autenticación ligeramente diferentes.

Este directorio proporciona **patrones de URL RTSP específicos por marca**, ejemplos de código de conexión usando VisioForge SDK y consejos de solución de problemas para los fabricantes de cámaras IP más populares.

## Cómo Funcionan las Conexiones RTSP de Cámaras

La mayoría de las cámaras IP modernas exponen flujos de video mediante el **protocolo RTSP (Real-Time Streaming Protocol)** en el puerto 554. El flujo general de conexión es:

1. Determinar la dirección IP de tu cámara (mediante descubrimiento ONVIF, tabla de asignaciones DHCP o utilidad del fabricante)
2. Construir la URL RTSP usando el patrón específico de la marca
3. Autenticarse con las credenciales de la cámara
4. Conectar y renderizar el flujo de video

### Código de Inicio Rápido

Conéctate a cualquier cámara RTSP usando uno de los tres enfoques del VisioForge SDK:

=== "VideoCaptureCoreX"

    ```csharp
    // Initialize SDK (call once at app startup)
    await VisioForgeX.InitSDKAsync();

    var videoCapture = new VideoCaptureCoreX(VideoView1);

    // Create RTSP source
    var rtsp = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream1"),
        "admin",
        "password",
        true); // capture audio

    videoCapture.Video_Source = rtsp;

    await videoCapture.StartAsync();
    ```

=== "VideoCaptureCore"

    ```csharp
    var videoCapture = new VideoCaptureCore(VideoView1 as IVideoView);

    videoCapture.IP_Camera_Source = new IPCameraSourceSettings()
    {
        URL = new Uri("rtsp://admin:password@192.168.1.100:554/stream1"),
        Type = IPSourceEngine.Auto_LAV
    };

    videoCapture.Audio_PlayAudio = true;
    videoCapture.Audio_RecordAudio = false;
    videoCapture.Mode = VideoCaptureMode.IPPreview;

    await videoCapture.StartAsync();
    ```

=== "Media Blocks"

    ```csharp
    var pipeline = new MediaBlocksPipeline();

    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream1"),
        "admin",
        "password",
        audioEnabled: true);

    rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;

    var rtspSource = new RTSPSourceBlock(rtspSettings);
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    var audioRenderer = new AudioRendererBlock();

    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);

    await pipeline.StartAsync();
    ```

Reemplaza la URL RTSP con el patrón específico de tu marca de las páginas siguientes.

### ¿Qué SDK Debo Elegir?

| SDK | Ideal Para | Plataformas |
|-----|----------|-----------|
| **VideoCaptureCoreX** | Nuevos proyectos multiplataforma, .NET moderno | Windows, macOS, Linux, Android, iOS |
| **VideoCaptureCore** | Proyectos solo Windows, .NET Framework legacy | Windows |
| **Media Blocks** | Pipelines avanzados, cadenas de procesamiento personalizadas | Windows, macOS, Linux, Android, iOS |

**VideoCaptureCoreX** es recomendado para la mayoría de los proyectos nuevos. Usa **Media Blocks** cuando necesites construir pipelines de procesamiento personalizados con múltiples fuentes, filtros o salidas.

## Marcas de Cámaras

### Marcas Destacadas (Guías Completas)

| Marca | Sede | Segmento de Mercado | Guía |
|-------|-------------|----------------|-------|
| **Hikvision** | Hangzhou, China | Empresarial / Consumidor | [Guía de Conexión](hikvision.md) |
| **Dahua** | Hangzhou, China | Empresarial / Consumidor | [Guía de Conexión](dahua.md) |
| **Axis** | Lund, Suecia | Empresarial / Profesional | [Guía de Conexión](axis.md) |
| **Reolink** | Hong Kong | Consumidor / Prosumer | [Guía de Conexión](reolink.md) |
| **Amcrest** | Houston, EE.UU. | Consumidor / PYME | [Guía de Conexión](amcrest.md) |
| **Samsung/Hanwha** | Grasbrunn, Alemania / Seúl, Corea del Sur | Empresarial / Profesional | [Guía de Conexión](samsung.md) |
| **Bosch** | Grasbrunn, Alemania | Empresarial / Infraestructura Crítica | [Guía de Conexión](bosch.md) |
| **Ubiquiti** | Nueva York, EE.UU. | Prosumer / PYME | [Guía de Conexión](ubiquiti.md) |
| **Foscam** | Shenzhen, China | Consumidor / PYME | [Guía de Conexión](foscam.md) |
| **TP-Link** | Shenzhen, China | Consumidor / PYME | [Guía de Conexión](tp-link.md) |
| **Vivotek** | Nueva Taipei, Taiwán | Empresarial / Profesional | [Guía de Conexión](vivotek.md) |
| **Panasonic/i-PRO** | Tokio, Japón | Empresarial / Gobierno | [Guía de Conexión](panasonic.md) |
| **Sony** | Tokio, Japón | Empresarial (descontinuado 2020) | [Guía de Conexión](sony.md) |
| **Lorex** | Markham, Canadá | Consumidor / Prosumer | [Guía de Conexión](lorex.md) |
| **D-Link** | Taipéi, Taiwán | Consumidor / PYME | [Guía de Conexión](dlink.md) |
| **Honeywell** | Charlotte, EE.UU. | Empresarial / Comercial | [Guía de Conexión](honeywell.md) |
| **Pelco** | Fresno, EE.UU. (Motorola Solutions) | Empresarial / Gobierno | [Guía de Conexión](pelco.md) |
| **Cisco** | San José, EE.UU. | Empresarial / Consumidor-PYME (legacy) | [Guía de Conexión](cisco.md) |
| **Grandstream** | Boston, EE.UU. | PYME / Profesional | [Guía de Conexión](grandstream.md) |
| **Swann** | Melbourne, Australia | Consumidor / Prosumer | [Guía de Conexión](swann.md) |
| **GeoVision** | Taipéi, Taiwán | Empresarial / Profesional | [Guía de Conexión](geovision.md) |
| **ACTi** | Taipéi, Taiwán | Profesional / Empresarial | [Guía de Conexión](acti.md) |
| **Canon** | Tokio, Japón | Profesional / Empresarial | [Guía de Conexión](canon.md) |
| **FLIR (Teledyne)** | Wilsonville, EE.UU. | Empresarial / Térmico | [Guía de Conexión](flir.md) |
| **Milesight** | Xiamen, China | Profesional / PYME | [Guía de Conexión](milesight.md) |
| **INSTAR** | Hanau, Alemania | Consumidor / Hogar Inteligente | [Guía de Conexión](instar.md) |
| **Zmodo** | Shenzhen, China | Consumidor / Económico | [Guía de Conexión](zmodo.md) |
| **Arecont Vision** | Glendale, EE.UU. (Costar Group) | Profesional / Empresarial | [Guía de Conexión](arecont.md) |
| **JVC** | Yokohama, Japón | Profesional (descontinuado ~2015) | [Guía de Conexión](jvc.md) |
| **Toshiba** | Tokio, Japón | Empresarial (descontinuado) | [Guía de Conexión](toshiba.md) |
| **LG** | Seúl, Corea del Sur | Empresarial (descontinuado) | [Guía de Conexión](lg.md) |
| **Linksys** | Irvine, EE.UU. | Consumidor (descontinuado ~2014) | [Guía de Conexión](linksys.md) |
| **LTS** | City of Industry, EE.UU. | Profesional (OEM Hikvision) | [Guía de Conexión](lts.md) |
| **Q-See** | Anaheim, EE.UU. | Consumidor (extinto ~2020) | [Guía de Conexión](q-see.md) |
| **Speco Technologies** | Amityville, EE.UU. | Profesional | [Guía de Conexión](speco.md) |
| **EverFocus** | Nueva Taipei, Taiwán | Profesional | [Guía de Conexión](everfocus.md) |
| **ABUS** | Wetter, Alemania | Consumidor / Profesional | [Guía de Conexión](abus.md) |
| **Basler** | Ahrensburg, Alemania | Visión Artificial / Industrial | [Guía de Conexión](basler.md) |
| **Mobotix** | Langmeil, Alemania (Konica Minolta) | Industrial / Infraestructura Crítica | [Guía de Conexión](mobotix.md) |
| **Avigilon** | Vancouver, Canadá (Motorola Solutions) | Empresarial / Infraestructura Crítica | [Guía de Conexión](avigilon.md) |
| **AVTech** | Taipéi, Taiwán | Comercial / Industrial | [Guía de Conexión](avtech.md) |
| **LILIN** | Nueva Taipei, Taiwán | Profesional / Empresarial | [Guía de Conexión](lilin.md) |
| **Zavio** | Hsinchu, Taiwán | Profesional / PYME | [Guía de Conexión](zavio.md) |
| **CP Plus** | Delhi, India | Empresarial / Comercial | [Guía de Conexión](cp-plus.md) |
| **Sanyo** | Osaka, Japón (ahora Panasonic) | Profesional (descontinuado) | [Guía de Conexión](sanyo.md) |
| **BrickCom** | Taipéi, Taiwán | Profesional / Industrial | [Guía de Conexión](brickcom.md) |
| **Edimax** | Taipéi, Taiwán | Consumidor / PYME | [Guía de Conexión](edimax.md) |
| **Uniview (UNV)** | Hangzhou, China | Empresarial / Gobierno | [Guía de Conexión](uniview.md) |
| **Hanwha Vision** | Seúl, Corea del Sur | Empresarial / Profesional | [Guía de Conexión](hanwha.md) |
| **Tiandy** | Tianjin, China | Empresarial / PYME | [Guía de Conexión](tiandy.md) |
| **EZVIZ** | Hangzhou, China (Hikvision) | Consumidor / Hogar Inteligente | [Guía de Conexión](ezviz.md) |
| **Wisenet** | Seúl, Corea del Sur (Hanwha Vision) | Empresarial / Profesional | [Guía de Conexión](wisenet.md) |
| **Annke** | Hong Kong | Consumidor / Prosumer | [Guía de Conexión](annke.md) |
| **Imou** | Hangzhou, China (Dahua) | Consumidor / Hogar Inteligente | [Guía de Conexión](imou.md) |
| **Wyze** | Kirkland, EE.UU. | Consumidor (RTSP limitado) | [Guía de Conexión](wyze.md) |
| **Aqara** | Shenzhen, China | Hogar Inteligente / HomeKit | [Guía de Conexión](aqara.md) |
| **Verkada** | San Mateo, EE.UU. | Empresarial / Gestión en la nube | [Guía de Conexión](verkada.md) |
| **Rhombus** | Sacramento, EE.UU. | Empresarial / Gestión en la nube | [Guía de Conexión](rhombus.md) |
| **Arlo** | Carlsbad, EE.UU. | Consumidor (sin RTSP) | [Guía de Conexión](arlo.md) |
| **Eufy Security** | Changsha, China (Anker) | Consumidor / Hogar Inteligente | [Guía de Conexión](eufy.md) |
| **Tenda** | Shenzhen, China | Consumidor / Económico | [Guía de Conexión](tenda.md) |
| **Mercusys** | Shenzhen, China (TP-Link) | Consumidor / Económico | [Guía de Conexión](mercusys.md) |

### Patrones Comunes de URL RTSP por Marca

Para referencia rápida, estos son los patrones de URL RTSP principales para marcas populares de cámaras:

| Marca | Patrón de URL RTSP Principal | Puerto Predeterminado |
|-------|--------------------------|-------------|
| Hikvision | `rtsp://IP:554/Streaming/Channels/101` | 554 |
| Dahua | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Axis | `rtsp://IP:554/axis-media/media.amp` | 554 |
| Foscam | `rtsp://IP:88/videoMain` | 88 |
| TP-Link (Tapo) | `rtsp://IP:554/stream1` | 554 |
| Amcrest | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Reolink | `rtsp://IP:554/h264Preview_01_main` | 554 |
| Ubiquiti | `rtsp://IP:7447/STREAM_TOKEN` | 7447 |
| Samsung/Hanwha | `rtsp://IP:554/profile2/media.smp` | 554 |
| Bosch | `rtsp://IP:554/video?inst=1` | 554 |
| Vivotek | `rtsp://IP:554/live.sdp` | 554 |
| Panasonic/i-PRO | `rtsp://IP:554/MediaInput/h264` | 554 |
| Sony | `rtsp://IP:554/media/video1` | 554 |
| Lorex | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| D-Link | `rtsp://IP:554/live1.sdp` | 554 |
| Honeywell | `rtsp://IP:554/h264` | 554 |
| Pelco | `rtsp://IP:554//stream1` | 554 |
| Cisco | `rtsp://IP:554/img/media.sav` | 554 |
| Grandstream | `rtsp://IP:554/live/ch00_0` | 554 |
| Swann | `rtsp://IP:554/live/h264` | 554 |
| GeoVision | `rtsp://IP:8554//CH001.sdp` | 8554 |
| ACTi | `rtsp://IP:7070//stream1` | 7070 |
| Canon | `rtsp://IP:554/cam1/h264` | 554 |
| FLIR (Teledyne) | `rtsp://IP:554/ch0` | 554 |
| Milesight | `rtsp://IP:554//main` | 554 |
| INSTAR | `rtsp://IP:554//11` | 554 |
| Zmodo | `rtsp://IP:10554//tcp/av0_0` | 10554 |
| Arecont Vision | `rtsp://IP:554/h264.sdp` | 554 |
| JVC | `rtsp://IP:554/PSIA/Streaming/channels/0` | 554 |
| Toshiba | `rtsp://IP:554/live.sdp` | 554 |
| LG | `rtsp://IP:554/video1+audio1` | 554 |
| Linksys | `rtsp://IP:554/img/media.sav` | 554 |
| LTS | `rtsp://IP:554//Streaming/Channels/1` | 554 |
| Q-See | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | 554 |
| Speco | `rtsp://IP:554/1/stream1` | 554 |
| EverFocus | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | 554 |
| ABUS | `rtsp://IP:554/video.mp4` | 554 |
| Basler | `rtsp://IP:554/h264` | 554 |
| Mobotix | `rtsp://IP:554/mobotix.h264` | 554 |
| Avigilon | `rtsp://IP:554/defaultPrimary?streamType=u` | 554 |
| AVTech | `rtsp://IP:554/live/h264` | 554 |
| LILIN | `rtsp://IP:554/rtsph2641080p` | 554 |
| Zavio | `rtsp://IP:554/video.mp4` | 554 |
| CP Plus | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | 554 |
| Sanyo | `rtsp://IP:554/VideoInput/1/h264/1` | 554 |
| BrickCom | `rtsp://IP:554/channel1` | 554 |
| Edimax | `rtsp://IP:554/ipcam_h264.sdp` | 554 |
| Uniview (UNV) | `rtsp://IP:554/media/video1` | 554 |
| Hanwha Vision | `rtsp://IP:554/profile2/media.smp` | 554 |
| Tiandy | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| EZVIZ | `rtsp://IP:554/h264/ch1/main/av_stream` | 554 |
| Wisenet | `rtsp://IP:554/profile2/media.smp` | 554 |
| Annke | `rtsp://IP:554/Streaming/Channels/101` | 554 |
| Imou | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | 554 |
| Wyze | `rtsp://IP:8554/live` | 8554 |
| Aqara | `rtsp://IP:554/live/ch00_1` | 554 |
| Verkada | N/A (solo nube) | N/A |
| Rhombus | `rtsp://IP:554/live` (si está habilitado) | 554 |
| Arlo | N/A (sin RTSP) | N/A |
| Eufy Security | `rtsp://IP:554/live0` | 554 |
| Tenda | `rtsp://IP:554/stream1` | 554 |
| Mercusys | `rtsp://IP:554/stream1` | 554 |

## Descubrimiento ONVIF

La mayoría de las cámaras IP modernas soportan **ONVIF (Open Network Video Interface Forum)**, que permite el descubrimiento automático de cámaras en tu red. VisioForge SDK soporta descubrimiento ONVIF -- consulta nuestra [guía de integración ONVIF](../mediablocks/Sources/index.md) para más detalles.

## Comenzar

### Instalar via NuGet

=== "Multiplataforma (recomendado)"

    ```bash
    dotnet add package VisioForge.CrossPlatform.Core
    ```

=== "Solo Windows"

    ```bash
    dotnet add package VisioForge.DotNet.Core
    dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
    ```

### Proyectos de Ejemplo

Ejemplos funcionales completos para integración de cámaras IP:

- [Vista Previa de Cámara IP (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-preview) — Vista en vivo de la cámara
- [Grabación de Cámara IP a MP4](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-capture-mp4) — Grabar flujos a archivo
- [Todos los Ejemplos del SDK .NET](https://github.com/visioforge/.Net-SDK-s-samples) — Repositorio completo de ejemplos

## Recursos Relacionados

- [Documentación del Bloque Fuente RTSP](../mediablocks/Sources/index.md)
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Grabación de Cámara IP a MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md)
- [Construcción de Aplicaciones de Cámara con Media Blocks](../mediablocks/GettingStarted/camera.md)
- [Guía de Enumeración de Dispositivos](../mediablocks/GettingStarted/device-enum.md)
