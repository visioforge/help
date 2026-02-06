---
title: MMT Live: Monitoreo de Medios en Tiempo Real (.NET)
description: Usa MMT Live para monitorear streams en vivo y detectar fragmentos de video en tiempo real para encontrar y gestionar contenido multimedia.
---

# MMT Live - Herramienta de Monitoreo de Medios en Tiempo Real

📦 **Código Fuente**: [Ver en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT%20Live)

## Descripción General

MMT Live es una versión en tiempo real de la Herramienta de Monitoreo de Medios que puede detectar fragmentos de video en streams en vivo o durante la reproducción en vivo. Está diseñada para monitorear transmisiones en tiempo real, detectar anuncios mientras se emiten y activar acciones inmediatas cuando se detecta contenido específico.

## Características Principales

- **Detección en Tiempo Real**: Identifica fragmentos mientras el video se reproduce
- **Soporte de Stream en Vivo**: Monitorea RTSP, HTTP y streams de archivos
- **Notificaciones Instantáneas**: Alertas inmediatas cuando se detecta contenido
- **Monitoreo Continuo**: Capacidad de operación 24/7
- **Detección de Anuncios**: Identificación de comerciales en tiempo real
- **Baja Latencia**: Detección casi instantánea
- **Vista Previa en Vivo**: Monitorea el stream mientras procesa

## Diferencias con MMT Estándar

| Característica | MMT | MMT Live |
|----------------|-----|----------|
| Procesamiento | Post-grabación | Tiempo real |
| Entrada | Solo archivos | Archivos + Streams |
| Detección | Por lotes | Continua |
| Resultados | Después de completar | Inmediatos |
| Caso de Uso | Análisis | Monitoreo |

## Interfaz de Usuario

### Componentes Principales

1. **Reproductor de Medios en Vivo**: Muestra el stream/reproducción actual
2. **Biblioteca de Fragmentos**: Objetivos de detección precargados
3. **Registro de Detección**: Eventos de detección en tiempo real
4. **Indicadores de Estado**: Salud del stream y estado de procesamiento
5. **Panel de Configuración**: Ajuste en vivo de parámetros

## Cómo Usar

### Flujo de Trabajo de Configuración

1. **Preparar Biblioteca de Fragmentos**:
   - Carga comerciales/clips a detectar
   - Genera huellas por adelantado
   - Organiza por prioridad/categoría

2. **Configurar Fuente de Entrada**:
   - Archivo: Selecciona video para monitorear
   - Stream: Ingresa URL RTSP/HTTP
   - Dispositivo: Selecciona dispositivo de captura

3. **Establecer Parámetros de Detección**:
   - Umbral de sensibilidad
   - Duración mínima de coincidencia
   - Preferencias de alerta

4. **Iniciar Monitoreo**:
   - Haz clic en "Start" para comenzar
   - El video se reproduce mientras analiza
   - Las detecciones aparecen inmediatamente

### Operación en Tiempo Real

- **Procesamiento Continuo**: Analiza el video mientras se reproduce
- **Buffer Rotativo**: Mantiene historial de video reciente
- **Coincidencia Instantánea**: Compara contra la biblioteca de fragmentos
- **Registro de Eventos**: Graba todas las detecciones con marcas de tiempo

## Casos de Uso

### 1. Cumplimiento de Transmisión

- Asegura que los anuncios se reproduzcan según lo programado
- Verifica restricciones de contenido
- Monitorea publicidad de competidores
- Rastrea segmentos de programa

### 2. Monitoreo de Stream en Vivo

- Detecta contenido con derechos de autor
- Monitorea múltiples canales
- Rastrea apariciones de marca
- Aseguramiento de calidad

### 3. Acciones Automatizadas

- Activa grabación al detectar
- Envía notificaciones/alertas
- Cambia streams automáticamente
- Genera informes en tiempo real

### 4. Seguimiento de Publicidad

- Cuenta emisiones de comerciales
- Verifica ubicación de anuncios
- Monitorea frecuencia de anuncios
- Análisis competitivo

## Configuración

### Fuentes de Entrada

**Reproducción de Archivo**:

- Simula monitoreo en vivo
- Útil para pruebas
- Soporta todos los formatos de video

**Streams RTSP**:

```txt
rtsp://camara.ejemplo.com:554/stream
rtsp://usuario:contraseña@servidor/ruta
```

**Streams HTTP**:

```txt
http://servidor.com/stream.m3u8
http://servidor.com/live.mjpeg
```

### Configuración de Detección

- **Tamaño de Buffer**: Historial de video (5-60 segundos)
- **Intervalo de Verificación**: Con qué frecuencia analizar (1-5 segundos)
- **Umbral de Confianza**: Calidad de coincidencia (70-95%)
- **Prioridad de Fragmento**: Qué fragmentos verificar primero

## Optimización de Rendimiento

### Requisitos del Sistema

- **CPU**: Multinúcleo recomendado
- **RAM**: 8-16GB para operación fluida
- **Red**: Conexión estable para streams
- **Almacenamiento**: SSD rápido para biblioteca de fragmentos

### Consejos de Optimización

1. **Biblioteca de Fragmentos**:
   - Mantén menos de 100 fragmentos activos
   - Pre-genera todas las huellas
   - Elimina fragmentos no utilizados

2. **Calidad del Stream**:
   - Usa tasa de bits consistente
   - Evita resoluciones muy altas
   - Asegura conexión estable

3. **Procesamiento**:
   - Ajusta el intervalo de verificación según la CPU
   - Usa tamaño de buffer apropiado
   - Habilita aceleración GPU si está disponible

## Características Avanzadas

### Monitoreo Multi-Stream

- Monitorea múltiples streams simultáneamente
- Hilos de detección separados por stream
- Informes consolidados
- Gestión de recursos

### Acciones Personalizadas

Configura acciones para detecciones:

- Notificaciones por email
- Webhooks HTTP
- Registro en archivos
- Grabación en base de datos
- Disparadores de grabación de stream

### Zonas de Detección

- Define ventanas de tiempo para detección
- Programa diferentes conjuntos de fragmentos
- Ignora ciertos períodos de tiempo
- Programación de prioridad

## Solución de Problemas

### No Hay Detecciones

- Verifica que los fragmentos estén cargados
- Comprueba que el stream esté reproduciéndose
- Confirma que las huellas estén generadas
- Ajusta la sensibilidad más baja

### Alto Uso de CPU

- Reduce la frecuencia de verificación
- Baja la resolución del stream
- Disminuye el tamaño del buffer
- Limita fragmentos activos

### Problemas de Stream

- Verifica la conectividad de red
- Verifica la URL del stream
- Prueba en reproductor de medios primero
- Monitorea el uso de ancho de banda

### Detecciones Retrasadas

- Aumenta la prioridad de procesamiento
- Reduce el tamaño del buffer
- Verifica los recursos del sistema
- Optimiza la cantidad de fragmentos

## Mejores Prácticas

### Preparación

1. Prueba fragmentos en MMT estándar primero
2. Optimiza la calidad y longitud de fragmentos
3. Construye biblioteca de fragmentos completa
4. Documenta las detecciones esperadas

### Operación

1. Monitorea los recursos del sistema
2. Actualizaciones regulares de biblioteca de fragmentos
3. Verificaciones periódicas de precisión de detección
4. Mantén registros de detección

### Mantenimiento

1. Limpia registros de detección antiguos
2. Actualiza huellas de fragmentos
3. Revisa falsos positivos/negativos
4. Optimiza basándote en resultados

## Opciones de Integración

### Integración API

- API REST para eventos de detección
- WebSocket para actualizaciones en tiempo real
- Opciones de registro en base de datos
- Integraciones de terceros

### Automatización

- Monitoreo programado
- Generación automática de informes
- Escalación de alertas
- Cambio de streams

## Comparación con Alternativas

**vs MMT Estándar**:

- Tiempo real vs post-procesamiento
- Operación continua vs por lotes
- Resultados inmediatos vs diferidos

**vs Monitoreo Manual**:

- Automatizado vs observación humana
- 24/7 vs horas limitadas
- Precisión consistente vs variable

## Herramientas Relacionadas

- `MMT`: Versión de análisis post-grabación
- `vfp_search`: Búsqueda de fragmentos por línea de comandos
- `DVS`: Detección de videos duplicados
