---
title: Cómo conectar una cámara IP Milesight en C# .NET
description: Conecte cámaras IP Milesight en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series MS-C, MS-A, MS-V, MS-F y MS-B.
---

# Cómo conectar una cámara IP Milesight en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**Milesight Technology** es un fabricante chino de cámaras IP y dispositivos IoT, con sede en Xiamen, China. Milesight se dirige a los mercados profesional y PYME con una línea en rápido crecimiento de cámaras con IA a precios competitivos. La marca es conocida por su fuerte cumplimiento con ONVIF, analíticas de IA integradas (detección facial, LPR, conteo de personas) e integración sencilla con plataformas VMS de terceros.

**Datos clave:**

- **Líneas de productos:** MS-C (mini domo/bala), MS-A (PTZ/domo de velocidad), MS-V (domo antivandálico), MS-F (ojo de pez), MS-B (caja), MS-N (NVR)
- **Soporte de protocolos:** RTSP, ONVIF (Profile S/G/T en todos los modelos actuales), HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / ms1234 (debe cambiarse en el primer inicio de sesión)
- **Soporte ONVIF:** Sí (todos los modelos actuales, Profile S/G/T)
- **Códecs de video:** H.264, H.265, MJPEG

!!! info "Doble barra en URLs RTSP"
    Las cámaras Milesight usan una **doble barra diagonal** antes de `main` y `sub` en sus URLs RTSP: `rtsp://IP:554//main`. Esto es intencional y requerido para todos los modelos actuales de Milesight.

## Patrones de URL RTSP

### Modelos Actuales (Todas las Series)

| Flujo | URL RTSP | Notas |
|-------|----------|-------|
| Flujo principal | `rtsp://IP:554//main` | Resolución completa (note la doble barra) |
| Subflujo | `rtsp://IP:554//sub` | Resolución más baja |
| Flujo raíz | `rtsp://IP:554/` | Alternativa |

### URLs Específicas por Modelo

Todos los modelos actuales de cámaras Milesight usan el mismo patrón de URL RTSP:

| Serie de Modelo | URL RTSP | Tipo | Notas |
|-----------------|----------|------|-------|
| MS-C2672-P | `rtsp://IP:554//main` | Mini domo | 2MP |
| MS-C3366-FP | `rtsp://IP:554//main` | Bala | 3MP, IA |
| MS-C3366-FPH | `rtsp://IP:554//main` | Bala | 3MP, IA, calefactor |
| MS-C2363 | `rtsp://IP:554//main` | Mini domo | 2MP |
| MS-2681 | `rtsp://IP:554//main` | Domo | 8MP |
| MS-3672 | `rtsp://IP:554//main` | Bala | 3MP |
| MS-A series (PTZ) | `rtsp://IP:554//main` | PTZ | Domo de velocidad |
| MS-V series (vandal) | `rtsp://IP:554//main` | Domo antivandálico | Clasificación IK10 |
| MS-F series (fisheye) | `rtsp://IP:554//main` | Ojo de pez | 360 grados |
| MS-B series (box) | `rtsp://IP:554//main` | Caja | Profesional |

### Flujos de Canal NVR

Para los NVR Milesight de la serie MS-N, use el parámetro `channel` para seleccionar flujos de cámaras individuales:

| Flujo | URL RTSP | Notas |
|-------|----------|-------|
| Canal 1, principal | `rtsp://IP:554//main?channel=1` | Canal 1 del NVR |
| Canal 2, principal | `rtsp://IP:554//main?channel=2` | Canal 2 del NVR |
| Canal 1, sub | `rtsp://IP:554//sub?channel=1` | Canal 1 del NVR, baja resolución |
| Canal 2, sub | `rtsp://IP:554//sub?channel=2` | Canal 2 del NVR, baja resolución |

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara Milesight con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// Milesight camera, main stream -- note the double slash before "main"
var uri = new Uri("rtsp://192.168.1.90:554//main");
var username = "admin";
var password = "ms1234";
```

Para acceder al subflujo, use `//sub` en lugar de `//main`. Para selección de canal del NVR, agregue `?channel=N` a la URL.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Notas |
|------|---------------|-------|
| Captura CGI | `http://IP/cgi-bin/snapshot.cgi` | Requiere autenticación básica |

## Solución de Problemas

### Se requiere doble barra

Las cámaras Milesight requieren una **doble barra diagonal** antes de la ruta del flujo:

- Correcto: `rtsp://IP:554//main`
- Puede no funcionar: `rtsp://IP:554/main`

Si su conexión falla, verifique que está usando el formato de URL con doble barra. Este patrón es similar a las cámaras Pelco y ACTi.

### La contraseña predeterminada debe cambiarse

La contraseña predeterminada de fábrica es `ms1234`, pero las cámaras Milesight requieren que esta contraseña se cambie durante el primer inicio de sesión a través de la interfaz web o Milesight CMS. Si la contraseña predeterminada no funciona, es probable que la cámara haya sido configurada con una nueva contraseña.

### Las funciones de IA son independientes de RTSP

Las funciones de IA de Milesight (detección facial, reconocimiento de matrículas, conteo de personas, detección de intrusión) se ejecutan en el procesador integrado de la cámara y no afectan la transmisión RTSP. Los metadatos y eventos de IA se entregan a través de eventos ONVIF o la API de Milesight, no a través del flujo RTSP en sí.

### Milesight CMS es opcional

Milesight CMS (Central Management Software) es el VMS propietario de Milesight. No es necesario para la transmisión RTSP. Las cámaras Milesight funcionan con cualquier VMS compatible con ONVIF o cualquier aplicación que soporte conexiones RTSP estándar.

### Numeración de canales del NVR

Al conectarse a través de un NVR Milesight MS-N, los números de canal comienzan en 1 y corresponden a la entrada física de la cámara o al orden de cámaras de red configurado en el NVR. Use `?channel=1` para la primera cámara, `?channel=2` para la segunda, y así sucesivamente.

### Descubrimiento ONVIF

Todas las cámaras Milesight actuales soportan ONVIF Profile S, G y T. Si prefiere el descubrimiento automático sobre la configuración manual de URL RTSP, use el descubrimiento de dispositivos ONVIF para encontrar cámaras en su red y recuperar sus URLs de transmisión automáticamente.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras Milesight?**

Para todas las cámaras Milesight actuales, use `rtsp://admin:ms1234@CAMERA_IP:554//main` (note la doble barra). Para el subflujo, use `//sub`. Para acceso al NVR, agregue `?channel=N` para seleccionar el canal de cámara deseado.

**¿Todos los modelos Milesight usan la misma URL RTSP?**

Sí. Todos los modelos actuales de cámaras Milesight (series MS-C, MS-A, MS-V, MS-F, MS-B) usan el mismo patrón de URL `//main` y `//sub`. Esto hace de Milesight una de las marcas más consistentes para integración RTSP.

**¿Milesight soporta H.265?**

Sí. Todas las cámaras Milesight actuales soportan codificación H.264, H.265 y MJPEG. H.265 puede habilitarse a través de la interfaz web de la cámara o Milesight CMS para reducir los requisitos de ancho de banda y almacenamiento.

**¿Por qué importa la doble barra en las URLs de Milesight?**

La doble barra (`//main` en lugar de `/main`) es parte de la especificación de URL RTSP de Milesight. Omitir la barra adicional puede causar fallos de conexión. Esta convención es compartida con algunas otras marcas de cámaras (Pelco, ACTi) pero no es universal en la industria.

**¿Puedo acceder a las analíticas de IA de Milesight a través de RTSP?**

No. RTSP entrega únicamente el flujo de video. Los resultados de analíticas de IA (eventos de detección facial, datos de matrículas, estadísticas de conteo de personas) son accesibles a través de eventos ONVIF, la API HTTP de Milesight o Milesight CMS. El flujo de video en sí no contiene metadatos de IA incrustados.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Grandstream](grandstream.md) — Segmento de cámaras PYME / profesional
- [Captura de Cámara IP a MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md) — Grabar flujos Milesight a archivo
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
