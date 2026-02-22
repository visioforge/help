---
title: Cámaras Zavio: URLs RTSP y guía de conexión en C# .NET
description: Conecte cámaras Zavio en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series B, D, F, M y P.
---

# Cómo conectar una cámara IP Zavio en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Zavio** (Zavio Inc.) es un fabricante taiwanés de cámaras IP con sede en Hsinchu, Taiwán. Zavio es conocido por cámaras de red de grado profesional con patrones de URL distintivos que incluyen tanto rutas de flujo directo como rutas basadas en perfil. La empresa se dirige a los mercados PYME y de seguridad profesional con una gama de modelos de cámaras bala, domo, fija, mini y PTZ.

**Datos clave:**

- **Líneas de productos:** B (bala), D (domo), F (fija/caja), M (mini), P (PTZ/pan-tilt), V (antivandálica)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI, MJPEG
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (mayoría de modelos)
- **Códecs de video:** H.264, MPEG-4, MJPEG
- **Patrones de URL duales:** Algunos modelos usan `/video.mp4`, otros usan `/video.proN` (basado en perfil)

!!! tip "URLs Basadas en Perfil"
    Las cámaras Zavio soportan URLs basadas en perfil. Use `/video.pro1` para el perfil principal y `/video.pro2` para el perfil secundario. Los perfiles disponibles dependen de la configuración de su cámara.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Zavio soportan dos patrones de URL RTSP principales:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video.mp4
```

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//video.pro1
```

| Ruta URL | Descripción |
|----------|-------------|
| `/video.mp4` | Flujo principal MP4 (formato más común) |
| `//video.pro1` | Perfil 1 / flujo principal (prefijo de doble barra) |
| `//video.pro2` | Perfil 2 / subflujo (prefijo de doble barra) |
| `//video.h264` | Flujo H.264 directo (algunos modelos) |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Notas |
|--------|------|------------------------|-------|
| B5110 (bala) | Bala | `rtsp://IP//video.pro1` | Basado en perfil, también soporta `//video.h264` |
| B5210 (bala) | Bala | `rtsp://IP//video.pro1` | Basado en perfil |
| B7110 (bala) | Bala | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| B7210 (bala) | Bala | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| D3100 (domo) | Domo | `rtsp://IP//video.pro1` | Basado en perfil |
| D3200 (domo) | Domo | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| D4210 (domo) | Domo | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| D50E (domo) | Domo | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| D510E (domo) | Domo | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| D520E (domo) | Domo | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| D7111 (domo) | Domo | `rtsp://IP:554//video.pro2` | Subflujo perfil 2 |
| D7210 (domo) | Domo | `rtsp://IP:554//video.pro2` | Subflujo perfil 2 |
| F1100 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F1105 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F1150 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F210A (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F3100 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F3102 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F3110 (fija) | Fija | `rtsp://IP:554//video.pro2` | Subflujo perfil 2 |
| F3115 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F312A (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F3201 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F3206 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F3210 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F3215 (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F511E (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F520IE (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F521E (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| F731E (fija) | Fija | `rtsp://IP:554/video.mp4` | Flujo principal MP4 |
| M510W (mini) | Mini | `rtsp://IP:554/video.mp4` | Cámara mini inalámbrica |
| M511E (mini) | Mini | `rtsp://IP:554/video.mp4` | Cámara mini |
| P5110 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |
| P5115 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |
| P5210 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |

### Formatos de URL Alternativos

Algunos modelos Zavio soportan estas URLs alternativas:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/video.mp4` | Flujo MP4 (recomendado para la mayoría de modelos) |
| `rtsp://IP//video.pro1` | Perfil 1, flujo principal |
| `rtsp://IP:554//video.pro2` | Perfil 2, subflujo |
| `rtsp://IP//video.h264` | Flujo H.264 directo (B5110 y similares) |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Zavio con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Zavio B7110, MP4 main stream
var uri = new Uri("rtsp://192.168.1.90:554/video.mp4");
var username = "admin";
var password = "admin";
```

Para cámaras basadas en perfil, use `//video.pro1` para el flujo principal o `//video.pro2` para el subflujo.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura por Perfil | `http://IP/cgi-bin/view/image?pro_CHANNEL` | Captura por número de perfil |
| Captura JPEG | `http://IP/jpg/image.jpg` | Captura JPEG estándar |
| JPEG con Tamaño | `http://IP/jpg/image.jpg?size=3` | JPEG con parámetro de tamaño |
| JPEG CGI | `http://IP/cgi-bin/jpg/image` | Captura JPEG basada en CGI |
| Flujo MJPEG | `http://IP/video.mjpg` | Flujo MJPEG continuo |
| MJPEG (calidad/FPS) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | MJPEG con control de calidad y FPS |
| Flujo HTTP por Perfil | `http://IP/stream?uri=video.proN` | Flujo HTTP basado en perfil |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras Zavio se envían con credenciales predeterminadas de `admin` / `admin`. Si la cámara ha sido configurada con credenciales diferentes:

1. Acceda a la cámara en `http://CAMERA_IP` en un navegador
2. Inicie sesión y verifique la configuración en **Network > RTSP**
3. Verifique que la autenticación RTSP esté habilitada y sus credenciales sean correctas

### Elegir entre /video.mp4 y /video.proN

Las cámaras Zavio tienen dos familias de URL. La elección correcta depende de su modelo:

- **Mayoría de modelos** (B7110, F210A, F312A, F520IE, F521E, F731E, etc.): Use `/video.mp4`
- **Modelos más antiguos o basados en perfil** (B5110, B5210, D3100): Use `//video.pro1`
- Si un formato falla, pruebe el otro

### Doble barra en URLs de perfil

Las URLs basadas en perfil de Zavio requieren una doble barra (`//`) antes de `video.proN`. Esto es intencional:

- Correcto: `rtsp://IP//video.pro1`
- Incorrecto: `rtsp://IP/video.pro1`

Si omite la doble barra en un modelo basado en perfil, la conexión puede fallar.

### Sin video con códec MPEG-4

Algunos modelos Zavio más antiguos tienen como predeterminado la codificación MPEG-4. Si experimenta problemas de códec:

- Inicie sesión en la interfaz web de la cámara
- Cambie el códec de video a **H.264** en la configuración del flujo
- Use la URL `/video.mp4` o `//video.pro1` después de cambiar la configuración

### Conexión rechazada en puerto 554

Verifique que RTSP esté habilitado en la cámara:

- Interfaz web: Verifique configuración en **Network > RTSP**
- Confirme que el puerto 554 no esté bloqueado por un firewall
- El puerto RTSP predeterminado es 554

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras Zavio?**

La URL más común es `rtsp://admin:admin@CAMERA_IP:554/video.mp4` para el flujo principal MP4. Para modelos basados en perfil, use `rtsp://admin:admin@CAMERA_IP//video.pro1` en su lugar.

**¿Las cámaras Zavio soportan ONVIF?**

Sí. La mayoría de modelos Zavio soportan ONVIF, lo que proporciona un método estandarizado para descubrimiento y transmisión de cámaras sin necesitar patrones de URL específicos de la marca.

**¿Cuál es la diferencia entre /video.mp4 y /video.pro1?**

`/video.mp4` es una ruta de flujo directo usada por la mayoría de modelos Zavio más nuevos. `//video.pro1` y `//video.pro2` son rutas basadas en perfil que hacen referencia a perfiles de flujo configurados en la interfaz web de la cámara. El Perfil 1 es típicamente el flujo principal (alta resolución) y el Perfil 2 es típicamente el subflujo (menor resolución).

**¿Cuáles son las credenciales de inicio de sesión predeterminadas para las cámaras Zavio?**

El nombre de usuario predeterminado es `admin` y la contraseña predeterminada es `admin`. Se recomienda encarecidamente cambiar estas credenciales después de la configuración inicial.

**¿Puedo controlar la calidad y tasa de cuadros MJPEG?**

Sí. Las cámaras Zavio soportan parámetros MJPEG en la URL. Use `http://IP/video.mjpg?q=30&fps=33&id=0.5` para especificar calidad (`q`), cuadros por segundo (`fps`) e identificador de flujo (`id`).

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Edimax](edimax.md) — Cámaras PYME taiwanesas
- [Captura ONVIF con Postprocesamiento](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Pipeline de captura ONVIF para Zavio
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
