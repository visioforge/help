---
title: Imou RTSP en C# .NET - Guía de Conexión de Cámaras IP
description: Conecte cámaras Imou en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series Cruiser, Ranger, Bullet, Cell y Versa.
---

# Cómo Conectar una Cámara IP Imou en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Imou** (pronunciado "ii-mu") es una marca de cámaras de seguridad y hogar inteligente para consumidores propiedad de **Dahua Technology**. Lanzada en 2019, Imou se dirige al mercado de consumidores y pequeñas empresas con cámaras WiFi, cámaras con batería, timbres y kits de seguridad para el hogar. Las cámaras Imou utilizan firmware de Dahua y patrones de URL RTSP de Dahua.

**Datos clave:**

- **Líneas de productos:** Cruiser (PT exterior), Ranger (PT interior), Bullet (exterior fija), Cell (batería), Versa (versátil), Rex (interior)
- **Soporte de protocolos:** RTSP (debe habilitarse en algunos modelos), ONVIF (modelos selectos), nube Imou Life
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin (o admin / imou + sufijo de número de serie)
- **Soporte ONVIF:** Sí (la mayoría de modelos con cable)
- **Códecs de vídeo:** H.264, H.265 (modelos selectos)
- **Empresa matriz:** Dahua Technology

!!! info "Imou = Marca de Consumo de Dahua"
    Las cámaras Imou utilizan firmware de Dahua y el mismo formato de URL RTSP `cam/realmonitor` que las cámaras Dahua. Consulte nuestra [guía de conexión Dahua](dahua.md) para detalles adicionales.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras Imou utilizan el patrón de URL `cam/realmonitor` de Dahua:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| `channel` | 1 | Canal de cámara (siempre 1 para cámaras independientes) |
| `subtype` | 0 | Flujo principal (mayor resolución) |
| `subtype` | 1 | Subflujo (menor resolución, menos ancho de banda) |

### Modelos de Cámaras

| Modelo | Tipo | URL de Flujo Principal | Audio |
|--------|------|----------------------|-------|
| Cruiser SE+ 4MP | PTZ Exterior | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Cruiser 2E 4MP | PTZ Exterior | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Ranger 2 (IPC-A22EP) | PTZ Interior | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Ranger SE 4MP | PTZ Interior | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Rex 3D (IPC-GS7EP) | PTZ Interior | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Bullet 2E (IPC-F22FP) | Exterior fija | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Bullet 2S (IPC-F26FP) | Exterior fija | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Versa 4MP | Interior/exterior | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Sí |
| Cell 2 | Batería exterior | Limitado — ver nota | Sí |
| Cell Go | Batería mini | Sin RTSP | No |

!!! warning "Modelos con Batería"
    Las cámaras Imou con batería (serie Cell) tienen soporte RTSP limitado o nulo. La Cell 2 puede soportar RTSP cuando está conectada a la Estación Base Imou, pero la Cell Go y otras cámaras mini con batería son dispositivos solo de nube.

### Formatos de URL Alternativos

| Patrón de URL | Notas |
|---------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Estándar (recomendado) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&unicast=true` | Forzar unicast |
| `rtsp://IP:554/live` | Ruta simple (algunos modelos) |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Imou con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Imou Cruiser SE+ 4MP, main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo, use `subtype=1` en lugar de `subtype=0`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requiere autenticación básica |
| Flujo MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | MJPEG continuo |

## Solución de Problemas

### RTSP no accesible

Algunas cámaras Imou requieren que RTSP se habilite a través de la aplicación Imou Life:

1. Abra la aplicación **Imou Life** → seleccione su cámara
2. Vaya a **Configuración > Configuración avanzada > RTSP**
3. Habilite RTSP y anote la contraseña (puede diferir de la contraseña de la aplicación)

### Credenciales predeterminadas

Los valores predeterminados de contraseña de Imou varían según el modelo y firmware:

- `admin` / `admin` (común en modelos antiguos)
- `admin` / código específico (verifique la etiqueta de la cámara)
- Contraseña personalizada establecida durante la configuración de la aplicación Imou Life

Si el inicio de sesión RTSP falla, verifique la contraseña RTSP en la configuración de la aplicación Imou Life.

### Dirección IP de cámara WiFi

Las cámaras WiFi Imou obtienen su IP de su router vía DHCP. Encuentre la IP local de la cámara en:

1. La aplicación Imou Life → Información del dispositivo
2. La lista de clientes DHCP de su router
3. Descubrimiento ONVIF (si es compatible)

### Interfaz web de Dahua

Algunas cámaras Imou exponen la interfaz web de Dahua en `http://CAMERA_IP`. Esto proporciona opciones de configuración adicionales más allá de la aplicación Imou Life, incluyendo configuración RTSP, codificación de vídeo y configuración de red.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Imou?**

La URL estándar es `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` para el flujo principal. Este es el mismo formato que las cámaras Dahua. Es posible que RTSP deba habilitarse en la aplicación Imou Life primero.

**¿Imou es lo mismo que Dahua?**

Imou es una marca de consumo propiedad de Dahua Technology. Las cámaras Imou usan firmware de Dahua y el mismo formato de URL RTSP (`cam/realmonitor`). Las principales diferencias son la marca, las funciones orientadas al consumidor y la integración del servicio en la nube.

**¿Puedo usar cámaras Imou sin la nube?**

Parcialmente. Puede acceder a flujos RTSP localmente sin la nube Imou para visualización en vivo y grabación. Sin embargo, la configuración inicial de la cámara requiere la aplicación Imou Life. Las funciones dependientes de la nube como alertas inteligentes, almacenamiento en la nube y acceso remoto requieren una suscripción a Imou.

**¿Las cámaras Imou soportan ONVIF?**

La mayoría de las cámaras Imou con cable y conectadas por WiFi soportan ONVIF. Los modelos con batería generalmente no lo soportan. Verifique las especificaciones de su cámara en la aplicación Imou Life.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Dahua](dahua.md) — Empresa matriz, formato de URL idéntico
- [Guía de Conexión Amcrest](amcrest.md) — Otra marca OEM de Dahua
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
