---
title: Amcrest: URLs RTSP y conexión de cámaras IP en C# .NET
description: Conecta cámaras Amcrest en C# .NET con patrones de URL RTSP y ejemplos de código para modelos IP2M, IP4M, IP5M, IP8M y NVR.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - IP Camera
  - RTSP
  - ONVIF
  - MJPEG
  - C#

---

# Cómo Conectar una Cámara IP Amcrest en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Amcrest** (Amcrest Technologies LLC) es una marca americana de cámaras de seguridad para consumidores con sede en Houston, Texas. Las cámaras Amcrest son fabricadas por **Dahua Technology** y usan firmware y protocolos de Dahua. Esto significa que las cámaras Amcrest comparten patrones de URL RTSP, interfaces web y endpoints de API idénticos con las cámaras Dahua. Amcrest se ha convertido en una de las marcas de cámaras IP más vendidas en Amazon en Norteamérica.

**Datos clave:**

- **Líneas de producto:** IP2M (1080p), IP4M (4MP), IP5M (5MP), IP8M (4K/8MP), ASH (hogar inteligente), NV (NVRs)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI, Amcrest Cloud, RTMP
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (debe cambiarse en el primer inicio de sesión con firmware más reciente)
- **Soporte ONVIF:** Sí (todos los modelos actuales)
- **Códecs de video:** H.264 (todos los modelos), H.265 (IP4M y posteriores)
- **Base OEM:** Dahua (formato de URL RTSP idéntico)

!!! info "Amcrest = Dahua"
    Las cámaras Amcrest usan firmware de Dahua y exactamente el mismo formato de URL RTSP que las cámaras Dahua. Si estás familiarizado con la integración de Dahua, Amcrest funciona de manera idéntica. Consulta nuestra [guía de conexión Dahua](dahua.md) para detalles adicionales.

## Patrones de URL RTSP

### Formato de URL Estándar

Amcrest usa el patrón de URL `cam/realmonitor` de Dahua:

```
rtsp://[USUARIO]:[CONTRASEÑA]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de la cámara (1 para cámaras independientes) |
| `subtype` | 0 | Flujo principal (resolución más alta) |
| `subtype` | 1 | Subflujo (resolución más baja, menos ancho de banda) |
| `subtype` | 2 | Tercer flujo (si es compatible, optimizado para móvil) |

### Modelos de Cámaras

| Modelo | Resolución | URL Flujo Principal | Audio |
|-------|-----------|----------------|-------|
| IP2M-841 (bala 1080p) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| IP2M-844 (domo 1080p) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| IP4M-1051 (bala 4MP) | 2688x1520 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| IP5M-T1179E (torreta 5MP) | 2592x1944 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| IP8M-2493E (bala 4K) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| IP8M-T2599E (torreta 4K) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| ASH-41 (pan/tilt) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| ASH-42 (interior) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |

### URLs de Canales NVR

Para NVRs Amcrest (NV4108E, NV4216E, NV5216E, etc.):

| Canal | Flujo Principal | Subflujo |
|---------|-------------|------------|
| Cámara 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Cámara 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Cámara N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

### Formatos de URL Alternativos

Algunos modelos Amcrest más antiguos o versiones de firmware soportan estas URLs alternativas:

| Patrón de URL | Notas |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Estándar (recomendado) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&unicast=true` | Forzar unicast |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&proto=Onvif` | Compatible con ONVIF |

## Conexión con VisioForge SDK

Usa la URL RTSP de tu cámara Amcrest con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Amcrest IP4M-1051, main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Para acceder al subflujo, usa `subtype=1` en lugar de `subtype=0`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|-------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requiere autenticación básica |
| Captura JPEG (legacy) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Autenticación por URL |
| Flujo MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | MJPEG continuo |
| Imagen actual | `http://IP/onvif-http/snapshot?channel=1` | Captura HTTP ONVIF |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras Amcrest con firmware más reciente requieren que la contraseña se cambie del valor predeterminado en el primer inicio de sesión. Si no has configurado la cámara a través de la interfaz web o la app Amcrest aún:

1. Accede a la cámara en `http://IP_CAMARA` desde un navegador
2. Completa el asistente de configuración inicial
3. Establece una contraseña segura
4. Usa esas credenciales en tu URL RTSP

### Puerto 554 vs puerto personalizado

Algunas versiones de firmware Amcrest permiten cambiar el puerto RTSP. Verifica la configuración del puerto en:

- Interfaz web: **Configuración > Red > Puerto > Puerto RTSP**
- El predeterminado es 554

### Confusión de tipo de flujo

- `subtype=0` = Flujo principal (resolución completa, mayor ancho de banda)
- `subtype=1` = Subflujo (resolución reducida, menor ancho de banda)
- `subtype=2` = Tercer flujo (si está disponible, típicamente para móvil)

### Cámaras Amcrest SmartHome (ASH)

Las cámaras de la serie ASH (como ASH-41, ASH-42) usan el mismo formato de URL RTSP, pero algunos modelos requieren habilitar RTSP en la app Amcrest Smart Home primero.

## Preguntas Frecuentes

**¿Son las cámaras Amcrest y Dahua lo mismo?**

Las cámaras Amcrest son fabricadas por Dahua y usan firmware de Dahua. El formato de URL RTSP (`cam/realmonitor?channel=1&subtype=0`) es idéntico. Cualquier código escrito para cámaras Dahua funciona con Amcrest y viceversa. Las principales diferencias son la marca, la garantía y el soporte en Norteamérica.

**¿Cuál es la URL RTSP predeterminada para cámaras Amcrest?**

La URL es `rtsp://admin:password@IP_CAMARA:554/cam/realmonitor?channel=1&subtype=0` para el flujo principal. Reemplaza `channel=1` con el canal apropiado para configuraciones NVR y `subtype=0` con `subtype=1` para el subflujo.

**¿Las cámaras Amcrest soportan ONVIF?**

Sí. Todas las cámaras Amcrest actuales soportan ONVIF Profile S y Profile T. ONVIF está habilitado por defecto en la mayoría de los modelos.

**¿Puedo usar cámaras Amcrest sin la nube de Amcrest?**

Sí. RTSP, ONVIF y la interfaz web funcionan localmente sin ninguna dependencia de la nube. El servicio de nube de Amcrest es opcional y solo es necesario para la visualización remota a través de las apps de Amcrest.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Dahua](dahua.md) — Mismo formato de URL (base OEM)
- [Guía de Conexión Lorex](lorex.md) — También usa formato de URL Dahua
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
