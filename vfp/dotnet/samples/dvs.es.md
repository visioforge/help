---
title: DVS: Escáner de Videos Duplicados para .NET
description: Usa DVS para escanear carpetas en busca de videos duplicados o similares con características para encontrar y gestionar tu biblioteca de medios.
---

# DVS - Escáner de Videos Duplicados

📦 **Código Fuente**:

- [DVS Windows Forms en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/DVS)
- [DVS MAUI (Multiplataforma) en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/DVS%20MAUI)

## Descripción General

DVS (Duplicate Video Scanner / Escáner de Videos Duplicados) es una aplicación de escritorio para Windows que escanea carpetas para encontrar videos duplicados o similares. Utiliza tecnología de huellas de video para comparar videos basándose en su contenido en lugar de las propiedades del archivo, haciéndolo efectivo para encontrar duplicados incluso cuando los videos tienen diferentes formatos, resoluciones o tasas de bits.

## Características

- **Procesamiento por Lotes**: Escanea múltiples carpetas simultáneamente
- **Comparación Inteligente**: Encuentra duplicados incluso con diferentes codificaciones
- **Soporte de Formatos**: Funciona con AVI, WMV, MP4, MPG, MOV, TS, FLV, MKV y más
- **Vista Previa Visual**: Reproductor de medios integrado para revisar duplicados detectados
- **Configuraciones Flexibles**: Umbrales de similitud y opciones de escaneo configurables
- **Seguimiento de Progreso**: Barras de progreso y actualizaciones de estado en tiempo real
- **Exportar Resultados**: Guarda resultados del escaneo para revisión posterior

## Interfaz de Usuario

### Componentes de la Ventana Principal

1. **Panel de Carpetas Fuente**: Agregar/eliminar carpetas a escanear
2. **Panel de Configuración**: Configurar parámetros de escaneo
3. **Panel de Resultados**: Ver y administrar duplicados encontrados
4. **Reproductor de Medios**: Vista previa de videos antes de tomar acción
5. **Barra de Estado**: Monitorear el estado de la operación actual

## Cómo Usar

### Flujo de Trabajo Básico

1. **Agregar Carpetas**:
   - Haz clic en el botón "Add" para seleccionar carpetas que contienen videos
   - Agrega múltiples carpetas para un escaneo completo
   - Usa "Remove" para eliminar carpetas de la lista

2. **Configurar Ajustes**:
   - Selecciona formatos de video a incluir en el escaneo
   - Establece la sensibilidad de comparación (1-100%)
   - Elige opciones de procesamiento

3. **Iniciar Escaneo**:
   - Haz clic en "Process" para comenzar el escaneo
   - DVS genera huellas para todos los videos
   - Los videos se comparan por pares para encontrar duplicados

4. **Revisar Resultados**:
   - Los grupos de duplicados aparecen en el panel de resultados
   - Haz clic en videos para previsualizarlos
   - Usa el menú contextual para operaciones de archivos

### Opciones de Configuración

- **Formatos Soportados**: Marca/desmarca formatos de video a incluir
- **Tamaño Mínimo de Archivo**: Omite archivos de video pequeños
- **Incluir Subcarpetas**: Escanea subdirectorios recursivamente
- **Umbral de Comparación**: Ajusta la sensibilidad (menor = coincidencia más estricta)

## Entendiendo los Resultados

### Puntuaciones de Similitud

- **95-100%**: Casi idénticos (mismo video, diferente codificación)
- **85-95%**: Muy similar (ediciones menores, logos agregados)
- **70-85%**: Contenido similar (ediciones significativas)
- **Menor de 70%**: Videos diferentes

### Agrupación de Resultados

DVS agrupa duplicados por similitud:

- Cada grupo contiene videos que coinciden entre sí
- El primer video en un grupo es la "referencia"
- Los detalles del archivo muestran tamaño, duración y ruta

## Casos de Uso

1. **Limpieza de Almacenamiento**: Encuentra y elimina videos duplicados para liberar espacio
2. **Organización de Medios**: Identifica múltiples copias en diferentes carpetas
3. **Gestión de Calidad**: Mantén la versión de mayor calidad de los duplicados
4. **Mantenimiento de Archivo**: Asegura que no haya copias duplicadas en respaldos
5. **Verificación de Contenido**: Comprueba si los videos son realmente diferentes

## Características Avanzadas

### Caché de Huellas

- DVS puede guardar huellas para escaneos posteriores más rápidos
- Habilita la opción "Save signatures"
- Las huellas en caché se almacenan con los videos

### Operaciones por Lotes

- Selecciona múltiples videos para acciones masivas
- Elimina duplicados manteniendo una copia
- Mueve duplicados a una carpeta separada
- Exporta listas de archivos para procesamiento externo

### Áreas de Ignorar Personalizadas

- Define regiones a ignorar (logos, marcas de tiempo)
- Útil para grabaciones de transmisiones
- Mejora la precisión para contenido con marcas de agua

## Consejos de Rendimiento

1. **Escaneo Inicial**: El primer escaneo es el más lento (genera todas las huellas)
2. **Escaneos Posteriores**: Mucho más rápidos con huellas en caché
3. **Bibliotecas Grandes**: Procesa en lotes para mejor uso de memoria
4. **Unidades de Red**: Copia a unidad local para procesamiento más rápido

## Solución de Problemas

### Problemas Comunes

1. **No Se Encuentran Duplicados**:
   - Verifica la configuración del umbral (intenta aumentarlo)
   - Verifica que los formatos de video estén seleccionados
   - Asegúrate de que las carpetas contengan archivos de video

2. **Demasiados Falsos Positivos**:
   - Disminuye el umbral de comparación
   - Verifica si los videos tienen intros/outros comunes
   - Usa áreas de ignorar para logos

3. **Rendimiento Lento**:
   - Procesa menos archivos a la vez
   - Verifica el espacio disponible en disco
   - Cierra otras aplicaciones

## Requisitos del Sistema

- Windows 7 o posterior (64-bit)
- .NET Framework 8.0 o posterior
- 4GB RAM mínimo (8GB recomendado)
- Almacenamiento adecuado para caché de huellas

## Gestión de Archivos

### Eliminación Segura de Duplicados

1. Siempre previsualiza antes de eliminar
2. Mantén la versión de mayor calidad
3. Considera mantener diferentes formatos
4. Usa "Move to folder" en lugar de eliminar

### Organizando Resultados

- Ordena por tamaño de archivo para encontrar ahorros de espacio
- Ordena por similitud para revisar las coincidencias más cercanas
- Agrupa por carpeta para ver la distribución

## Mejores Prácticas

1. **Prueba Primero**: Ejecuta en una carpeta pequeña para verificar configuraciones
2. **Respalda Archivos Importantes**: Antes de eliminaciones masivas
3. **Revisa Cuidadosamente**: Algunos "duplicados" pueden ser intencionales
4. **Usa Umbrales Apropiados**: Ajusta según el tipo de contenido
5. **Escaneos Regulares**: Escaneos periódicos previenen acumulación de duplicados

## Herramientas Relacionadas

- `vfp_compare`: Herramienta de línea de comandos para comparar dos videos
- `Image Comparer`: Herramienta similar para encontrar imágenes duplicadas
- `MMT`: Herramienta de monitoreo de medios para análisis de transmisiones
