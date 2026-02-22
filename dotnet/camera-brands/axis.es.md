---
title: Axis - URLs RTSP para Cámaras IP y Conexión con C# .NET
description: Conecta cámaras Axis Communications en C# .NET con patrones de URL RTSP, API VAPIX y ejemplos de código para modelos de las series M, P, Q y F.
---

# Cómo Conectar una Cámara IP Axis en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Axis Communications** es un fabricante sueco ampliamente reconocido como el pionero de las cámaras de red, habiendo creado la primera cámara IP del mundo en 1996. Con sede en Lund, Suecia y ahora subsidiaria de Canon, Axis produce cámaras IP premium, codificadores y productos de audio en red principalmente para el mercado de vigilancia profesional y empresarial.

**Datos clave:**

- **Líneas de producto:** Serie M (compacta/mini), serie P (fija), serie Q (profesional), serie F (modular), serie V (antivandalismo), cámaras PTZ
- **Soporte de protocolos:** ONVIF Profile S/G/T, RTSP, VAPIX (API HTTP propietaria de Axis), HTTP/MJPEG
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** root / (establecido durante la configuración inicial; firmware antiguo: root / pass)
- **Soporte ONVIF:** Completo -- Axis fue miembro fundador de ONVIF
- **Códecs de video:** H.264, H.265 (modelos más recientes), MJPEG
- **Características únicas:** API HTTP VAPIX para control integral de la cámara, ACAP (Axis Camera Application Platform)

## Patrones de URL RTSP

Las cámaras Axis usan la ruta RTSP `axis-media/media.amp` con parámetros opcionales para control de resolución y códec.

### Formato de URL

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:[PUERTO]/axis-media/media.amp
```

### URLs RTSP Principales

| Serie de Modelo | URL RTSP | Códec | Audio |
|-------------|----------|-------|-------|
| Todos los modelos modernos | `rtsp://IP:554/axis-media/media.amp` | H.264 (predeterminado) | Posible |
| Todos los modelos modernos | `rtsp://IP:554/axis-media/media.amp?videocodec=h264` | H.264 (explícito) | Posible |
| Todos los modelos modernos | `rtsp://IP:554/axis-media/media.amp?videocodec=h265` | H.265 | Posible |
| Perfil ONVIF | `rtsp://IP:554/onvif-media/media.amp` | H.264 | Sí |
| Modelos legacy | `rtsp://IP:554/mpeg4/media.amp` | MPEG-4 | Posible |

### Selección de Perfil de Flujo

Las cámaras Axis soportan perfiles de flujo nombrados que se pueden seleccionar mediante parámetro de URL:

| Patrón de URL | Notas |
|-------------|-------|
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Quality` | Perfil de alta calidad |
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Balanced` | Perfil equilibrado |
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Bandwidth` | Perfil de bajo ancho de banda |
| `rtsp://IP:554/axis-media/media.amp?resolution=1920x1080` | Resolución explícita |
| `rtsp://IP:554/axis-media/media.amp?resolution=640x480` | Resolución más baja |
| `rtsp://IP:554/axis-media/media.amp?fps=15` | Límite de tasa de fotogramas |

### Modelos Multicanal (Codificadores, Multisensor)

Para dispositivos multicanal como codificadores de video (M7001, P7214) y cámaras multisensor:

| Dispositivo | URL RTSP | Canal |
|--------|----------|---------|
| Canal 1 | `rtsp://IP:554/axis-media/media.amp?camera=1` | 1 |
| Canal 2 | `rtsp://IP:554/axis-media/media.amp?camera=2` | 2 |
| Canal 3 | `rtsp://IP:554/axis-media/media.amp?camera=3` | 3 |
| Canal 4 | `rtsp://IP:554/axis-media/media.amp?camera=4` | 4 |

### Formatos de URL Legacy

Las cámaras Axis más antiguas (serie 200, primeras series 1000) pueden requerir estos formatos:

| Patrón de URL | Modelos | Notas |
|-------------|--------|-------|
| `rtsp://IP:554/mpeg4/media.amp` | 200, 205, 206, 207 | Flujo MPEG-4 |
| `http://IP/axis-cgi/mjpg/video.cgi` | Todos los modelos | MJPEG sobre HTTP |
| `http://IP/mjpg/video.mjpg` | Serie 200 | Flujo MJPEG directo |
| `http://IP/axis-cgi/mjpg/video.cgi?camera=1` | Multicanal | Canal específico |
| `http://IP/axis-cgi/mjpg/video.cgi?resolution=640x480` | Todos los modelos | Resolución específica |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Axis con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Axis camera, H.264 main stream
var uri = new Uri("rtsp://192.168.1.50:554/axis-media/media.amp");
var username = "root";
var password = "YourPassword";
```

Para acceder al subflujo, agrega el parámetro `?resolution=640x480`.

### Descubrimiento ONVIF

Axis fue miembro fundador de ONVIF y tiene cumplimiento ONVIF líder en la industria. Consulta la [guía de integración ONVIF](../mediablocks/Sources/index.md) para ejemplos de código de descubrimiento.

## URLs de Captura y MJPEG (API VAPIX)

Las cámaras Axis proporcionan la API HTTP VAPIX, que es más rica en funciones que la mayoría de las otras marcas:

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/axis-cgi/jpg/image.cgi` | Fotograma actual |
| Captura (dimensionada) | `http://IP/axis-cgi/jpg/image.cgi?resolution=1920x1080` | Resolución específica |
| Captura (con superposición) | `http://IP/axis-cgi/jpg/image.cgi?date=1&clock=1` | Superposición de fecha/hora |
| Captura (selección de cámara) | `http://IP/axis-cgi/jpg/image.cgi?camera=1` | Dispositivo multicanal |
| Captura simple | `http://IP/jpg/image.jpg` | Captura JPEG básica |
| Captura dimensionada | `http://IP/jpg/image.jpg?size=3` | Tamaño predefinido (1-5) |
| Flujo MJPEG | `http://IP/axis-cgi/mjpg/video.cgi` | MJPEG continuo |
| MJPEG (resolución) | `http://IP/axis-cgi/mjpg/video.cgi?resolution=640x480` | MJPEG dimensionado |
| MJPEG (directo) | `http://IP/mjpg/video.mjpg` | MJPEG directo (legacy) |

## Solución de Problemas

### Audio "Posible" vs "Sí"

Axis marca el soporte de audio como "Posible" en muchos flujos RTSP porque la disponibilidad de audio depende de si el modelo de cámara tiene micrófono integrado o entrada de audio externa. La URL RTSP es la misma tenga o no audio -- el SDK detectará y usará automáticamente el audio si está disponible.

### Errores "401 Unauthorized"

- Las cámaras Axis usan autenticación digest por defecto para RTSP
- Asegúrate de usar las credenciales correctas (el nombre de usuario predeterminado es `root`, no `admin`)
- En firmware más reciente, la contraseña debe cumplir requisitos de complejidad (mínimo 8 caracteres)

### Flujo MPEG-4 no disponible en modelos más recientes

Las cámaras Axis modernas (firmware 5.x+) han eliminado el soporte MPEG-4. Usa `/axis-media/media.amp` (H.264) en lugar de `/mpeg4/media.amp`.

### La resolución no coincide con la salida esperada

Las cámaras Axis negocian la resolución dinámicamente. Para forzar una resolución específica, agrega el parámetro `resolution`:
`rtsp://IP:554/axis-media/media.amp?resolution=1920x1080`

### Conexiones de codificador multicanal

Al conectarte a un codificador Axis (M7001, P7214, etc.), debes especificar el parámetro de cámara/canal. Sin él, obtienes el canal 1 por defecto.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Axis?**

La URL estándar es `rtsp://root:password@IP_CAMARA:554/axis-media/media.amp`. Esto funciona para prácticamente todas las cámaras Axis modernas (series M, P, Q, F, V). El nombre de usuario predeterminado es `root` (no `admin` como en la mayoría de las otras marcas).

**¿Cómo cambio entre H.264 y H.265 en cámaras Axis?**

Agrega el parámetro `videocodec` a la URL RTSP: `rtsp://IP:554/axis-media/media.amp?videocodec=h265` para H.265, o `videocodec=h264` para H.264. Ten en cuenta que H.265 solo está disponible en modelos Axis más recientes con chipsets Artpec-7 o posteriores.

**¿Puedo controlar la calidad del flujo mediante la URL RTSP?**

Sí. Axis soporta varios parámetros de URL: `resolution` (ej., `1920x1080`), `fps` (tasa de fotogramas), `compression` (0-100) y `streamprofile` (perfiles nombrados configurados en la cámara). Ejemplo: `rtsp://IP:554/axis-media/media.amp?resolution=1280x720&fps=15`.

**¿Por qué Axis usa "root" como nombre de usuario predeterminado en lugar de "admin"?**

Las cámaras Axis ejecutan Linux embebido y, siguiendo las convenciones de Unix, el usuario administrativo se llama `root`. Esto es diferente de la mayoría de las otras marcas de cámaras que usan `admin`.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Bosch](bosch.md) — Par de vigilancia empresarial
- [Guía de Conexión Hanwha Vision](hanwha.md) — Par de vigilancia empresarial
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
