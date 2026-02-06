---
title: Herramienta de Monitoreo de Medios para .Net
description: Usa MMT para encontrar fragmentos de video dentro de videos más grandes con características para gestionar tu biblioteca de medios.
---

# Herramienta de Monitoreo de Medios

📦 **Código Fuente**:
- [MMT Windows Forms en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT)
- [MMT MAUI (Multiplataforma) en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT%20MAUI)

## Descripción General

MMT (Media Monitoring Tool / Herramienta de Monitoreo de Medios) es una aplicación de escritorio para Windows diseñada para encontrar fragmentos de video dentro de videos más grandes. Se utiliza principalmente para detectar comerciales, anuncios, intros, outros o cualquier segmento de video específico dentro de grabaciones de transmisiones o colecciones de video. A diferencia de DVS que compara videos completos, MMT se especializa en localizar dónde aparecen clips específicos dentro de contenido más largo.

## Características Principales

- **Detección de Fragmentos**: Encuentra clips de video cortos dentro de grabaciones largas
- **Procesamiento por Lotes**: Busca múltiples fragmentos en múltiples videos
- **Monitoreo de Publicidad**: Detecta y rastrea apariciones de comerciales
- **Visualización de Línea de Tiempo**: Ve exactamente cuándo aparecen los fragmentos
- **Vista Previa de Medios**: Reproductor integrado para revisar detecciones
- **Capacidades de Exportación**: Guarda resultados en formato CSV
- **Soporte de Base de Datos**: Construye bibliotecas de fragmentos para búsquedas repetidas

## Interfaz de Usuario

### Componentes Principales

1. **Reproductor de Medios**: Vista previa de videos y detecciones
2. **Pestaña Broadcast Dump**: Administra videos donde buscar
3. **Pestaña Ads/Fragments**: Administra fragmentos de video a buscar
4. **Pestaña Results**: Ve resultados de detección y estadísticas
5. **Pestaña Settings**: Configura parámetros de búsqueda
6. **Barra de Estado**: Monitorea el progreso del procesamiento

## Cómo Usar

### Flujo de Trabajo Básico

1. **Agregar Contenido de Transmisión**:
   - Usa la pestaña "Broadcast dump"
   - Agrega archivos o carpetas que contengan videos de larga duración
   - Estos son los videos dentro de los cuales buscarás

2. **Agregar Fragmentos**:
   - Cambia a la pestaña "Ads/fragments"
   - Agrega comerciales, intros o clips a encontrar
   - Puedes agregar archivos individuales o carpetas

3. **Configurar Ajustes**:
   - Establece la sensibilidad de detección
   - Elige opciones de procesamiento
   - Configura preferencias de salida

4. **Procesar**:
   - Haz clic en "Process" para iniciar el análisis
   - MMT genera huellas para todo el contenido
   - Busca cada fragmento en cada transmisión

5. **Revisar Resultados**:
   - Ve las detecciones en la pestaña Results
   - Observa marcas de tiempo y puntuaciones de confianza
   - Previsualiza coincidencias en el reproductor de medios

## Casos de Uso

### 1. Detección de Comerciales

- Monitorea grabaciones de TV para anuncios específicos
- Rastrea frecuencia y ubicación de comerciales
- Genera informes para análisis publicitario

### 2. Monitoreo de Contenido

- Encuentra contenido con derechos de autor en subidas
- Detecta uso no autorizado de clips de video
- Monitorea apariciones de marca

### 3. Análisis de Transmisiones

- Localiza segmentos de programa (intros, outros)
- Encuentra segmentos de noticias en múltiples transmisiones
- Rastrea contenido recurrente

### 4. Control de Calidad

- Verifica inserción de anuncios en transmisiones
- Comprueba segmentos faltantes
- Asegura cumplimiento de contenido

## Configuración y Opciones

### Parámetros de Detección

- **Sensibilidad**: Ajusta el umbral de coincidencia (1-100)
  - Mayor = Coincidencia más estricta (menos falsos positivos)
  - Menor = Coincidencia más flexible (puede omitir variaciones)

- **Desplazamiento de Tiempo**: Máximo desplazamiento temporal permitido
- **Áreas de Ignorar**: Define regiones a excluir (logos, tickers)

### Opciones de Procesamiento

- **Multi-threading**: Usa múltiples núcleos de CPU
- **Límite de Memoria**: Controla el uso de RAM
- **Caché de Huellas**: Guarda para reprocesamiento más rápido

## Entendiendo los Resultados

### Información del Resultado

Cada detección muestra:

- **Nombre del Fragmento**: Qué clip fue encontrado
- **Archivo de Transmisión**: Dónde fue encontrado
- **Marca de Tiempo**: Posición exacta (HH:MM:SS)
- **Duración**: Longitud de la coincidencia
- **Confianza**: Calidad de la coincidencia (porcentaje)

### Puntuaciones de Confianza

- **95-100%**: Coincidencia exacta
- **85-95%**: Alta confianza (diferencias menores)
- **70-85%**: Coincidencia probable (algunas modificaciones)
- **Menor de 70%**: Posible coincidencia (requiere revisión)

## Características Avanzadas

### Base de Datos de Fragmentos

- Construye bibliotecas de fragmentos comunes
- Guarda huellas para reutilización
- Organiza por categorías (comerciales, intros, etc.)

### Análisis por Lotes

- Procesa múltiples transmisiones durante la noche
- Encola grandes conjuntos de fragmentos
- Genera informes completos

### Exportación CSV

Los resultados pueden exportarse con:

- Marcas de tiempo de detección
- Rutas de archivos
- Puntuaciones de confianza
- Detalles del fragmento

## Mejores Prácticas

### Preparación de Fragmentos

1. **Longitud Óptima**: 10-60 segundos funciona mejor
2. **Cortes Limpios**: Evita fotogramas parciales al inicio/fin
3. **Calidad Completa**: Usa la fuente de mayor calidad disponible
4. **Sin Modificaciones**: No recortes ni edites fragmentos

### Archivos de Transmisión

1. **Formato Consistente**: Se prefiere codificación similar
2. **Archivos Completos**: Evita grabaciones corruptas
3. **Longitud Razonable**: Divide grabaciones muy largas

### Optimización de Rendimiento

1. **Pre-genera Huellas**: Ahorra tiempo de procesamiento
2. **Procesa en Lotes**: No sobrecargues la memoria
3. **Usa Almacenamiento SSD**: Acceso más rápido a archivos
4. **Cierra Otras Apps**: Maximiza recursos disponibles

## Solución de Problemas

### No Se Encuentran Detecciones

- Verifica la calidad y longitud del fragmento
- Verifica que la transmisión contenga el fragmento
- Ajusta la configuración de sensibilidad
- Asegura el rango de tiempo correcto

### Demasiados Falsos Positivos

- Aumenta el umbral de sensibilidad
- Verifica elementos comunes (fotogramas negros)
- Usa fragmentos más largos
- Define áreas de ignorar

### Problemas de Rendimiento

- Reduce el número de archivos simultáneos
- Habilita caché de huellas
- Verifica espacio disponible en disco
- Monitorea el uso de RAM

## Flujos de Trabajo Típicos

### Monitoreo de Comerciales

1. Graba streams de transmisión
2. Crea biblioteca de fragmentos de comerciales
3. Ejecuta MMT diaria/semanalmente
4. Exporta informes para clientes

### Verificación de Contenido

1. Prepara clips de contenido autorizado
2. Monitorea plataformas de video
3. Detecta uso no autorizado
4. Documenta violaciones

## Requisitos del Sistema

- Windows 7 o posterior (64-bit)
- .NET Framework 8.0
- 8GB RAM recomendado
- Almacenamiento rápido para archivos de video
- CPU multinúcleo beneficioso

## Herramientas Relacionadas

- `vfp_search`: Búsqueda de fragmentos por línea de comandos
- `MMT Live`: Versión de monitoreo en tiempo real
- `DVS`: Encuentra videos completos duplicados
