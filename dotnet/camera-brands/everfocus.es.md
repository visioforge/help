---
title: Cómo conectar una cámara IP EverFocus en C# .NET
description: Conecte cámaras EverFocus en C# .NET con patrones de URL RTSP y ejemplos de código para modelos de las series EAN, EHN, EMN, EPN, EZN y EQN.
---

# Cómo conectar una cámara IP EverFocus en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción de la Marca

**EverFocus Electronics** es una empresa taiwanesa de vigilancia profesional con sede en New Taipei City, Taiwán, con operaciones en EE.UU. basadas en Duarte, California. Fundada en 1995, EverFocus fabrica cámaras IP, DVRs y soluciones de vigilancia móvil diseñadas para integradores de seguridad profesional. La empresa es bien conocida en el mercado de vigilancia comercial e industrial.

**Datos clave:**

- **Líneas de productos:** EAN (bala), EHN (domo), EMN (mini domo), EPN (PTZ), EZN (compacta), EQN (torreta), ECOR/EPARA (DVRs)
- **Soporte de protocolos:** RTSP, ONVIF, HTTP/CGI
- **Puerto RTSP predeterminado:** 554
- **Credenciales predeterminadas:** admin / admin
- **Soporte ONVIF:** Sí (todas las cámaras IP actuales)
- **Códecs de video:** H.264 (todos los modelos actuales)

!!! info "Formato de URL RTSP de EverFocus"
    Las cámaras EverFocus usan una ruta `rtspStreamOvf` única en sus URLs RTSP. Este formato es específico de EverFocus y no debe confundirse con los patrones de URL de otros fabricantes. Note la doble barra requerida (`//`) antes de `cgi-bin`.

## Patrones de URL RTSP

### Formato de URL Estándar

Las cámaras EverFocus usan la ruta CGI `rtspStreamOvf` para transmisión RTSP:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//cgi-bin/rtspStreamOvf/0
```

| Parámetro | Valor | Descripción |
|-----------|-------|-------------|
| Índice de flujo | `/0` | Flujo principal (resolución más alta) |
| Índice de flujo | `/1` | Subflujo (resolución más baja, menos ancho de banda) |

!!! warning "Doble Barra Requerida"
    La ruta de URL debe incluir una doble barra antes de `cgi-bin` (es decir, `//cgi-bin/rtspStreamOvf/...`). Omitir la barra inicial causará que la conexión falle.

### Formato de URL Principal (rtspStreamOvf)

La mayoría de cámaras IP EverFocus usan el formato `rtspStreamOvf`:

| Modelo | Serie | URL de Flujo Principal | URL de Subflujo |
|--------|-------|------------------------|-----------------|
| EAN3220 | EAN (bala) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EHN3260 | EHN (domo) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN2220 | EMN (mini domo) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN1360 | EMN (mini domo) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN3260 | EMN (mini domo) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EPN4220 | EPN (PTZ) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EZN3160 | EZN (compacta) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |

### Formato de URL Alternativo (streaming/channels)

Algunos modelos EverFocus más nuevos también soportan el formato `streaming/channels`:

| Modelo | Serie | URL de Flujo Principal |
|--------|-------|------------------------|
| EPN4220 | EPN (PTZ) | `rtsp://IP:554/streaming/channels/0` |
| EZN3240 | EZN (compacta) | `rtsp://IP:554/streaming/channels/0` |
| EHN3260 | EHN (domo) | `rtsp://IP:554/streaming/channels/0` |

!!! tip "Qué Formato Usar"
    Pruebe primero el formato `rtspStreamOvf`, ya que es compatible con todas las líneas de productos de cámaras IP EverFocus. El formato `streaming/channels` es una alternativa disponible en modelos más nuevos selectos.

## Conexión con VisioForge SDK

Use la URL RTSP de su cámara EverFocus con cualquiera de los tres enfoques del SDK mostrados en la [Guía de Inicio Rápido](index.md#codigo-de-inicio-rapido):

```csharp
// EverFocus EHN3260, main stream
var uri = new Uri("rtsp://192.168.1.90:554//cgi-bin/rtspStreamOvf/0");
var username = "admin";
var password = "admin";
```

Para acceder al subflujo, use `/1` en lugar de `/0` al final de la URL.

## URLs de Captura y MJPEG

| Tipo | Patrón de URL | Modelos Compatibles |
|------|---------------|---------------------|
| Captura (con auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS&strm=CHANNEL` | EQN2101 |
| Captura (simple) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Cámaras IP generales |
| Captura móvil | `http://IP/m/camera[CHANNEL].jpg` | DVRs ECOR/EPARA |

!!! note "Capturas de DVR"
    Para modelos DVR ECOR y EPARA, reemplace `[CHANNEL]` con el número de canal de la cámara (por ejemplo, `camera1.jpg` para el canal 1).

## Solución de Problemas

### Conexión rechazada o timeout

Las cámaras EverFocus usan la ruta CGI `rtspStreamOvf` que es única de esta marca. Asegúrese de no estar usando accidentalmente un formato de URL de otro fabricante:

1. Verifique que la URL incluya la doble barra: `//cgi-bin/rtspStreamOvf/0`
2. Confirme que el puerto RTSP es 554 (o verifique la configuración de red de la cámara para un puerto personalizado)
3. Asegúrese de que la cámara sea accesible en la red haciendo ping a su dirección IP

### El índice de flujo comienza en 0

A diferencia de algunas otras marcas de cámaras donde los canales comienzan en 1, los índices de flujo de EverFocus comienzan en **0**:

- `/0` = Flujo principal (resolución completa)
- `/1` = Subflujo (resolución reducida)

Usar `/1` esperando el flujo principal devolverá el subflujo en su lugar.

### El formato de URL alternativo no funciona

El formato de URL `streaming/channels/0` solo está disponible en ciertos modelos más nuevos (EPN4220, EZN3240, EHN3260). Si este formato no funciona, recurra al formato estándar `//cgi-bin/rtspStreamOvf/0`.

### Problemas de autenticación

Las cámaras EverFocus tienen como predeterminado `admin` / `admin`. Si ha cambiado la contraseña a través de la interfaz web y la ha olvidado, un botón de restablecimiento de hardware en la cámara restaurará los valores predeterminados de fábrica.

## Preguntas Frecuentes

**¿Cuál es la URL RTSP predeterminada para las cámaras EverFocus?**

La URL predeterminada es `rtsp://admin:admin@CAMERA_IP:554//cgi-bin/rtspStreamOvf/0` para el flujo principal. Use `/1` al final para el subflujo. Note la doble barra antes de `cgi-bin` que es requerida.

**¿Por qué la URL RTSP de EverFocus se ve diferente a la de otras cámaras?**

EverFocus usa una ruta CGI propietaria `rtspStreamOvf` que es única de su firmware de cámara. Esto es diferente de los formatos más comunes usados por Hikvision, Dahua o rutas genéricas ONVIF. La doble barra (`//cgi-bin/...`) es intencional y requerida.

**¿Las cámaras EverFocus soportan ONVIF?**

Sí. Todas las cámaras IP EverFocus actuales soportan ONVIF, lo que proporciona una forma estandarizada de descubrir y conectarse a la cámara. Puede usar ONVIF como alternativa al formato de URL RTSP propietario.

**¿Puedo conectarme a DVRs EverFocus (ECOR/EPARA) vía RTSP?**

Los DVRs EverFocus principalmente exponen URLs de captura basadas en HTTP para canales individuales (`http://IP/m/camera[CHANNEL].jpg`). Para transmisión RTSP desde canales DVR, consulte la documentación de su modelo específico de DVR o use el descubrimiento ONVIF.

## Recursos Relacionados

- [Todas las Marcas de Cámaras — Directorio de URLs RTSP](index.md)
- [Guía de Conexión de Speco](speco.md) — Cámaras de vigilancia profesional
- [Guía de Transmisión de Video RTSP](../general/network-streaming/rtsp.md) — Configuración de transmisión RTSP EverFocus
- [Tutorial de Vista Previa de Cámara IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Instalación del SDK y Ejemplos](index.md#comenzar)
