---
title: Cómo Conectar una Cámara IP Wisenet en C# .NET
description: Conecte cámaras Wisenet en C# .NET con patrones de URL RTSP para las series Wisenet X, P, Q, L de Hanwha Vision. Ejemplos de código completos y guía de NVR.
---

# Cómo Conectar una Cámara IP Wisenet en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Wisenet** es el nombre de marca de producto utilizado por **Hanwha Vision** (anteriormente Hanwha Techwin / Samsung Techwin) para todas las cámaras IP, NVRs y sistemas de gestión de vídeo. Wisenet no es una empresa separada sino el nombre de la familia de productos utilizado en toda la línea de vigilancia de Hanwha Vision.

**Datos clave:**

- **Fabricante:** Hanwha Vision (Corea del Sur)
- **Niveles de producto:** X (premium), P (IA), Q (mainstream), Q mini (compacto), L (valor), T (térmico)
- **Soporte de protocolos:** RTSP, ONVIF Profile S/G/T, SUNAPI (propietario)
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / (se establece durante la activación)
- **Soporte ONVIF:** Sí (todos los modelos actuales)
- **Códecs de vídeo:** H.264, H.265, WiseStream III, MJPEG

!!! info "Wisenet = Productos Hanwha Vision"
    Wisenet es la **marca de producto**, Hanwha Vision es la **empresa**. Todas las cámaras Wisenet utilizan los mismos patrones de URL RTSP. Para instrucciones detalladas de conexión incluyendo acceso a canales del NVR y solución de problemas, consulte nuestra [guía de conexión Hanwha Vision](hanwha.md). Para cámaras heredadas de marca Samsung, consulte la [guía Samsung/Hanwha](samsung.md).

## Patrones de URL RTSP

### Formato de URL Estándar

Todas las cámaras Wisenet comparten la misma estructura de URL basada en perfiles:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/profile[N]/media.smp
```

### Por Nivel de Producto

| Nivel Wisenet | Modelos de Ejemplo | URL de Flujo Principal | Característica Clave |
|--------------|-------------------|----------------------|---------------------|
| **Serie X** (premium) | XNO-6080R, XND-8080RV, XNP-6120H | `rtsp://IP:554/profile2/media.smp` | Mejor WDR, poca luz |
| **Serie P** (IA) | PNO-A9081R, PND-A9081RV | `rtsp://IP:554/profile2/media.smp` | Analítica de aprendizaje profundo |
| **Serie Q** (mainstream) | QNO-8080R, QND-8080R, QNE-8021R | `rtsp://IP:554/profile2/media.smp` | Equilibrio características/precio |
| **Q mini** (compacto) | QND-8021 | `rtsp://IP:554/profile2/media.smp` | Factor de forma discreto |
| **Serie L** (valor) | LNO-6032R, LND-6032R | `rtsp://IP:554/profile2/media.smp` | Nivel de entrada |
| **Serie T** (térmico) | TNO-4030T, TNO-4050T | `rtsp://IP:554/profile2/media.smp` | Térmico + visible |

### Modelos Multi-Sensor y Multidireccionales

| Tipo de Modelo | URL de Flujo | Notas |
|---------------|-------------|-------|
| Sensor único | `rtsp://IP:554/profile2/media.smp` | Estándar |
| Multi-sensor canal 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | Primer sensor |
| Multi-sensor canal 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | Segundo sensor |
| Panorámico combinado | `rtsp://IP:554/profile2/media.smp/trackID=channel5` | Vista combinada |

### Acceso NVR / WAVE VMS

Para NVRs Wisenet (series XRN, QRN, LRN):

| Canal | Flujo Principal | Subflujo |
|-------|----------------|----------|
| Cámara 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | `rtsp://IP:554/profile3/media.smp/trackID=channel1` |
| Cámara 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | `rtsp://IP:554/profile3/media.smp/trackID=channel2` |
| Cámara N | `rtsp://IP:554/profile2/media.smp/trackID=channelN` | `rtsp://IP:554/profile3/media.smp/trackID=channelN` |

## Conexión con VisioForge SDK

Utilice la URL RTSP de su cámara Wisenet con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Wisenet QNO-8080R (Q series 5MP), main stream
var uri = new Uri("rtsp://192.168.1.90:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

Para acceso al subflujo, use `/profile3/media.smp` en lugar de `/profile2/media.smp`.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura JPEG | `http://IP/cgi-bin/video.cgi?msubmenu=jpg&action=view&Resolution=1920x1080&Quality=5&Channel=0` | Requiere autenticación digest |
| Flujo MJPEG | `http://IP/cgi-bin/video.cgi?msubmenu=mjpeg&action=view&Channel=0&Stream=0` | MJPEG continuo |

## Solución de Problemas

### ¿Qué número de perfil es el flujo principal?

Las cámaras Wisenet normalmente usan `profile2` como el flujo principal (mayor calidad). Esto es diferente de la mayoría de otras marcas. Si obtiene resultados inesperados, verifique la configuración de perfil en la interfaz web de la cámara (**Setup > Video/Audio > Video Profile**).

### Ahorro de ancho de banda WiseStream III

WiseStream III ajusta dinámicamente la codificación por región en el fotograma. La salida es H.265 o H.264 estándar -- no se necesita ningún decodificador especial. La configuración de WiseStream se puede ajustar en la interfaz web de la cámara.

### Activación de la cámara

Las nuevas cámaras Wisenet requieren activación (establecer una contraseña) antes de su uso. Use la utilidad Wisenet Installation Wizard, un navegador web o la aplicación móvil Wisenet para la configuración inicial.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para cámaras Wisenet?**

Todas las cámaras Wisenet usan `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp` para el flujo principal. Use `profile3` para el subflujo.

**¿Cuál es la diferencia entre Wisenet X, P, Q y L?**

**X** = premium empresarial. **P** = analítica con inteligencia artificial. **Q** = negocio mainstream. **L** = valor/nivel de entrada. **T** = térmico. Todos los niveles usan el mismo formato de URL RTSP.

**¿Wisenet es lo mismo que las cámaras de seguridad Samsung?**

Wisenet es la marca de producto actual de Hanwha Vision, que adquirió la división de seguridad de Samsung en 2015. Las cámaras heredadas de Samsung Techwin pueden usar formatos de URL diferentes. Consulte nuestra [guía Samsung/Hanwha](samsung.md) para modelos más antiguos.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión Hanwha Vision](hanwha.md) — Integración detallada de Hanwha Vision
- [Guía Heredada Samsung/Hanwha](samsung.md) — Modelos más antiguos de Samsung Techwin
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
