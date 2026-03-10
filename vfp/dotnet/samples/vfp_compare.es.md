---
title: Comparar Huellas de Video - CLI vfp_compare en .NET
description: Compare huellas de video por similitud usando la herramienta CLI vfp_compare de VisioForge con umbrales configurables y detección de duplicados.
---

# vfp_compare - Herramienta de Comparación de Huellas de Video

📦 **Código Fuente**: [Ver en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/Console/vfp_compare)

## Descripción General

`vfp_compare` es una herramienta de línea de comandos que compara dos archivos de huellas de video para determinar si los videos son similares o idénticos. Es útil para detectar videos duplicados, encontrar contenido similar o verificar la integridad del video.

## Características

- Compara huellas de video pregeneradas
- Comparación rápida sin reprocesar videos
- Umbral de similitud configurable
- Devuelve puntuación numérica de diferencia

## Uso

```bash
vfp_compare -f "huella1.vsigx" -s "huella2.vsigx" [opciones]
```

### Parámetros Requeridos

- `-f, --f1` : Ruta al primer archivo de huella
- `-s, --f2` : Ruta al segundo archivo de huella

### Parámetros Opcionales

- `-d, --md` : Diferencia máxima aceptable (predeterminado: 500)
- `-l, --license` : Clave de licencia VisioForge (predeterminado: "TRIAL")

## Ejemplos

### Comparación básica
```bash
vfp_compare -f "video1.vsigx" -s "video2.vsigx"
```

### Comparación con umbral personalizado
```bash
vfp_compare -f "original.vsigx" -s "copia.vsigx" -d 100
```

### Usando clave de licencia
```bash
vfp_compare -f "archivo1.vsigx" -s "archivo2.vsigx" -l "TU-CLAVE-DE-LICENCIA"
```

## Salida

La herramienta muestra:
- Puntuación de diferencia (menor = más similar)
- Resultado de comparación basado en el umbral
- Tiempo de procesamiento

### Entendiendo las Puntuaciones de Diferencia

- **0-5**: Videos casi idénticos (mismo contenido, diferencias menores de codificación)
- **5-15**: Videos muy similares (mismo contenido, diferente calidad/compresión)
- **15-30**: Videos similares (mismo contenido con ediciones, logos o marcas de agua)
- **30-100**: Contenido relacionado con diferencias significativas
- **100-300**: Videos diferentes con algunas escenas similares
- **300+**: Videos completamente diferentes

## Casos de Uso

1. **Detección de Duplicados**: Encuentra copias exactas de videos en diferentes formatos
2. **Comparación de Calidad**: Compara diferentes codificaciones del mismo video
3. **Detección de Ediciones**: Identifica si un video ha sido modificado
4. **Verificación de Derechos de Autor**: Comprueba si el contenido coincide con la fuente original

## Ejemplo de Flujo de Trabajo

1. Genera huellas para los videos:
```bash
vfp_gen -i "original.mp4" -o "original.vsigx" -t compare
vfp_gen -i "sospechoso.mp4" -o "sospechoso.vsigx" -t compare
```

2. Compara las huellas:
```bash
vfp_compare -f "original.vsigx" -s "sospechoso.vsigx"
```

## Notas de Rendimiento

- La comparación es casi instantánea
- El uso de memoria es mínimo (solo carga las huellas)
- No requiere decodificación de video

## Manejo de Errores

La herramienta saldrá con un error si:
- Alguno de los archivos de huella no existe
- Los archivos de huella están corruptos
- Las huellas fueron generadas con configuraciones incompatibles

## Mejores Prácticas

- Usa huellas tipo "compare" (generadas con `-t compare`) para mejores resultados
- Mantén las huellas con sus videos fuente como referencia
- Documenta el umbral de diferencia usado para tu caso de uso

## Herramientas Relacionadas

- `vfp_gen` : Genera huellas desde archivos de video
- `vfp_search` : Busca fragmentos dentro de videos
- `DVS` : Herramienta GUI para encontrar videos duplicados en carpetas
