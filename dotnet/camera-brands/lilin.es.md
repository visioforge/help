---
title: LILIN - URLs RTSP para cámaras IP y conexión en C# .NET
description: Conecte cámaras LILIN en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series LR, IPR, Z, D, S y P.
---

# Cómo conectar una cámara IP LILIN en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**LILIN** (Merit LILIN Co., Ltd.) es un fabricante taiwanés profesional de cámaras de seguridad con sede en New Taipei City, Taiwán. Fundada en 1980, LILIN es uno de los fabricantes de cámaras IP más antiguos del mundo. La empresa es conocida por cámaras de vigilancia de grado profesional con patrones de URL RTSP distintivos que codifican la resolución directamente en la ruta de la URL.

**Datos clave:**

- **Líneas de productos:** Z Series (bala), S Series (domo de velocidad), D Series (domo), LR Series (IR), P Series (panorámica)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / pass
- **Soporte ONVIF:** Sí (mayoría de modelos actuales)
- **Códecs de video:** H.264, MJPEG
- **Patrón de URL único:** Resolución codificada en la ruta RTSP (ej., `rtsph264720p`, `rtsph2641080p`)

!!! info "Rutas RTSP Basadas en Resolución"
    LILIN usa un patrón de URL único donde la resolución está codificada directamente en la ruta RTSP (ej., `rtsph264720p` para 720p, `rtsph2641080p` para 1080p). Asegúrese de usar el sufijo de resolución correcto para su modelo de cámara.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras LILIN usan un formato de ruta RTSP basado en resolución:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/rtsph2641080p
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `rtsph264720p` | Flujo 720p | H.264 a resolución 1280x720 |
| `rtsph2641080p` | Flujo 1080p | H.264 a resolución 1920x1080 |
| `rtsph2641024p` | Flujo 1024p | H.264 a resolución 1280x1024 (nota: doble barra en algunos modelos) |

### Modelos de Cámaras

| Modelo | Resolución | URL de Flujo Principal | Notas |
|--------|------------|------------------------|-------|
| LR7022E4 (bala IR) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Serie LR, 1080p |
| LR7722X (bala IR) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Serie LR, 1080p |
| IPR712M4.3 (PTZ) | 1280x1024 | `rtsp://IP:554//rtsph2641024p` | Serie IPR, ruta con doble barra |
| Z Series (bala) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Cámaras bala exterior |
| D Series (domo) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Domo interior/exterior |
| S Series (domo de velocidad) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Domo de velocidad PTZ |

### Formatos de URL Alternativos

Algunos modelos LILIN o versiones de firmware soportan estas URLs alternativas:

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/rtsph2641080p` | 1080p estándar (recomendado) |
| `rtsp://IP:554/rtsph264720p` | Flujo 720p |
| `rtsp://IP:554//rtsph2641024p` | Flujo 1024p (doble barra, algunos modelos PTZ) |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara LILIN con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// LILIN LR7022E4, 1080p main stream
var uri = new Uri("rtsp://192.168.1.90:554/rtsph2641080p");
var username = "admin";
var password = "pass";
```

Para acceso a 720p, use `rtsph264720p` en lugar de `rtsph2641080p`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG (VGA) | `http://IP/getimage?camera=CHANNEL&fmt=vga` | Captura resolución VGA |
| Captura por Canal | `http://IP/getimage[CHANNEL]` | Reemplace CHANNEL con número de cámara |
| Captura Rápida | `http://IP/snap` | URL de captura simple |
| Captura CGI | `http://IP/cgi-bin/net_jpeg.cgi?ch=CHANNEL` | Captura basada en CGI |
| Captura con Auth | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Autenticación basada en URL |
| Imagen Directa | `http://IP/image/CHANNEL.jpg` | Imagen JPEG directa por canal |

## Solución de Problemas

### Error "401 Unauthorized"

Las cámaras LILIN se envían con credenciales predeterminadas de `admin` / `pass`. Si ha cambiado la contraseña a través de la interfaz web, asegúrese de actualizar las credenciales en su URL RTSP.

1. Acceda a la cámara en `http://CAMERA_IP` en un navegador
2. Inicie sesión con sus credenciales
3. Verifique la configuración RTSP en la sección de configuración de red

### Doble barra en la ruta RTSP

Algunos modelos LILIN, particularmente la serie IPR PTZ, requieren una doble barra (`//`) antes de la ruta de resolución. Si una URL con una sola barra falla:

- Pruebe `rtsp://IP:554//rtsph2641024p` en lugar de `rtsp://IP:554/rtsph2641024p`
- Esto se observa comúnmente con modelos de resolución 1024p

### Elegir el sufijo de resolución correcto

Las cámaras LILIN no usan `subtype=0/1` como muchas otras marcas. En su lugar, la resolución del flujo se selecciona cambiando la ruta de la URL:

- `rtsph264720p` para 720p (1280x720)
- `rtsph2641080p` para 1080p (1920x1080)
- `rtsph2641024p` para 1024p (1280x1024)

Si especifica una resolución que su cámara no soporta, la conexión fallará.

### Conexión rechazada en puerto 554

Verifique que RTSP esté habilitado en la cámara:

- Interfaz web: Verifique configuración en **Network > RTSP**
- Confirme que el puerto 554 no esté bloqueado por un firewall
- El puerto RTSP predeterminado es 554

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras LILIN?**

La URL más común es `rtsp://admin:pass@CAMERA_IP:554/rtsph2641080p` para el flujo principal de 1080p. Reemplace el sufijo de resolución (`rtsph2641080p`) con el valor apropiado para la resolución de su cámara.

**¿Las cámaras LILIN soportan ONVIF?**

Sí. La mayoría de modelos LILIN actuales soportan ONVIF, lo que proporciona un método alternativo para descubrir y conectarse a la cámara sin necesitar patrones de URL específicos de la marca.

**¿Por qué LILIN usa un formato de URL RTSP diferente?**

LILIN codifica la resolución directamente en la ruta RTSP en lugar de usar parámetros de canal/subtipo como Dahua o Hikvision. Esta es una decisión de diseño propietaria. El formato es directo una vez que conoce qué sufijo de resolución soporta su modelo de cámara.

**¿Cuáles son las credenciales de inicio de sesión predeterminadas para las cámaras LILIN?**

El nombre de usuario predeterminado es `admin` y la contraseña predeterminada es `pass`. Se recomienda cambiar estas credenciales después de la configuración inicial por razones de seguridad.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de AVTech](avtech.md) — Cámaras industriales taiwanesas
- [Guía de Conexión de BrickCom](brickcom.md) — Cámaras industriales taiwanesas
- [Guía de Integración de Cámara RTSP](../videocapture/video-sources/ip-cameras/rtsp.md) — Configuración de flujo RTSP LILIN
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
