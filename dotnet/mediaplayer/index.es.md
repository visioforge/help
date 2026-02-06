---
title: Media Player SDK .Net (MediaPlayerCore)
description: Implementa reproducción de video, streaming y capacidades de reproducción de medios en aplicaciones .NET con tutoriales de uso de Media Player SDK.

---

# Media Player SDK .Net

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Media Player SDK .Net es un SDK de reproductor de video con una amplia gama de características.

El SDK puede usar varios motores de decodificación para reproducir archivos de video y audio, como FFMPEG, VLC y DirectShow. La mayoría de los formatos de video y audio son soportados por el motor FFMPEG.

Puedes reproducir archivos, streams de red, videos de 360 grados y, opcionalmente, discos DVD y Blu-Ray.

## Características

- Reproducción de video y audio
- Efectos de video
- Efectos de audio
- Superposiciones de texto
- Superposiciones de imagen
- Superposiciones SVG
- Brillo, contraste, saturación, tono y otros ajustes de video
- Sepia, pixelado, escala de grises y otros filtros de video

Puedes consultar la lista completa de características en la [página del producto](https://www.visioforge.com/media-player-sdk-net).

## Aplicaciones de ejemplo

Puedes usar código WPF en aplicaciones WinForms y viceversa. La mayoría del código es el mismo para todos los frameworks de UI, incluyendo Avalonia y MAUI. La principal diferencia es el control VideoView disponible para cada framework de UI.

### Motor MediaPlayerCoreX (multiplataforma)

MAUI

Avalonia

- [Reproductor de Medios Simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia/Simple%20Media%20Player) muestra funcionalidad básica de reproducción en Avalonia

iOS

Android

- [Reproductor de Medios Simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Android/MediaPlayer) muestra funcionalidad básica de reproducción en Android

macOS

WPF

WinForms

### Motor MediaPlayerCore (solo Windows)

#### WPF

- [Reproductor Simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Simple%20Player%20Demo) muestra funcionalidad básica de reproducción
- [Demo Principal](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Main%20Demo) muestra todas las características del SDK
- [Reproductor Nvidia Maxine](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Nvidia%20Maxine%20Player) usa el motor Nvidia Maxine
- [Reproductor con Skin](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Skinned%20Player) muestra cómo usar skins personalizados
- [Demo madVR](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/madVR%20Demo) usa el renderizador de video madVR

#### WinForms

- [Reproductor de Audio](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Audio%20Player) muestra cómo reproducir archivos de audio
- [Reproductor de DVD](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/DVD%20Player) muestra cómo reproducir DVDs
- [Demo de Reproducción de Memoria Encriptada](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Encrypted%20Memory%20Playback%20Demo) muestra cómo reproducir archivos encriptados desde memoria
- [Demo de Karaoke](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Karaoke%20Demo) muestra cómo reproducir archivos de audio karaoke
- [Demo Principal](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Main%20Demo) muestra todas las características del SDK
- [Stream de Memoria](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Memory%20Stream) muestra cómo reproducir archivos desde memoria
- [Múltiples Streams de Video](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Multiple%20Video%20Streams) muestra cómo reproducir archivos con múltiples streams de video
- [Reproducción Sin Interrupciones](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Seamless%20Playback) muestra cómo reproducir archivos sin retrasos
- [Reproductor de Video Simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Simple%20Video%20Player) muestra funcionalidad básica de reproducción
- [Dos Ventanas](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Two%20Windows) muestra cómo reproducir archivos en dos ventanas
- [Demo VR 360](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/VR%20360%20Demo) muestra cómo reproducir videos de 360 grados
- [Demo de Mezcla de Video](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Video%20Mixing%20Demo) muestra cómo mezclar archivos de video
- [Reproductor de YouTube](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/YouTube%20Player%20Demo) muestra cómo reproducir videos de YouTube (con licencia abierta)
- [Demo madVR](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/madVR%20Demo) usa madVR para renderizar video

#### WinUI

- [Reproductor de Medios Simple](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinUI/CSharp/Simple%20Media%20Player%20WinUI) muestra funcionalidad básica de reproducción

#### Fragmentos de código

- [Reproducción desde Memoria](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_CodeSnippets/memory-playback) muestra cómo reproducir archivos desde memoria
- [Leer Información del Archivo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_CodeSnippets/read-file-info) muestra cómo leer información del archivo

## Documentación  

- [Guías Adicionales](guides/index.md)
- [Ejemplos de código](code-samples/index.md)
- [Despliegue](deployment.md)
- [API](https://api.visioforge.org/dotnet/api/index.html)

## Enlaces

- [Registro de Cambios](../changelog.md)
- [Contrato de Licencia de Usuario Final](../../eula.md)
