---
title: Comparar huellas de video - CLI vfp_compare en .NET
description: Compare huellas de video por similitud usando la herramienta CLI vfp_compare de VisioForge con umbrales configurables y detección de duplicados.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - MP4
  - C#

---

# vfp_compare - herramienta de comparación de huellas de video

📦 **Código Fuente**: [Ver en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/Console/vfp_compare)

## Descripción general

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

### Parámetros requeridos

- `-f, --f1` : Ruta al primer archivo de huella
- `-s, --f2` : Ruta al segundo archivo de huella

### Parámetros opcionales

- `-d, --md` : Desplazamiento temporal máximo en segundos entre los videos en el que se busca la mejor alineación (predeterminado: 500)
- `--threshold` : Diferencia por debajo de la cual los archivos se consideran similares (predeterminado: 300)
- `-l, --license` : Clave de licencia VisioForge (predeterminado: "TRIAL")

## Ejemplos

### Comparación básica
```bash
vfp_compare -f "video1.vsigx" -s "video2.vsigx"
```

### Comparación con umbral personalizado
```bash
vfp_compare -f "original.vsigx" -s "copia.vsigx" --threshold 100
```

### Usar clave de licencia
```bash
vfp_compare -f "archivo1.vsigx" -s "archivo2.vsigx" -l "TU-CLAVE-DE-LICENCIA"
```

## Salida

La herramienta muestra:
- Puntuación de diferencia (menor = más similar)
- Resultado de comparación basado en el umbral
- Tiempo de procesamiento

### Entender las puntuaciones de diferencia

- **0-5**: Videos casi idénticos (mismo contenido, diferencias menores de codificación)
- **5-15**: Videos muy similares (mismo contenido, diferente calidad/compresión)
- **15-30**: Videos similares (mismo contenido con ediciones, logos o marcas de agua)
- **30-100**: Contenido relacionado con diferencias significativas
- **100-300**: Videos diferentes con algunas escenas similares
- **300+**: Videos completamente diferentes

## Casos de uso

1. **Detección de Duplicados**: Encuentra copias exactas de videos en diferentes formatos
2. **Comparación de Calidad**: Compara diferentes codificaciones del mismo video
3. **Detección de Ediciones**: Identifica si un video ha sido modificado
4. **Verificación de Derechos de Autor**: Comprueba si el contenido coincide con la fuente original

## Ejemplo de flujo de trabajo

1. Genere huellas para los videos:
```bash
vfp_gen -i "original.mp4" -o "original.vsigx" -t compare
vfp_gen -i "sospechoso.mp4" -o "sospechoso.vsigx" -t compare
```

2. Compara las huellas:
```bash
vfp_compare -f "original.vsigx" -s "sospechoso.vsigx"
```

## Notas de rendimiento

- La comparación es casi instantánea
- El uso de memoria es mínimo (solo carga las huellas)
- No requiere decodificación de video

## Manejo de errores

La herramienta saldrá con un error si:
- Alguno de los archivos de huella no existe
- Los archivos de huella están corruptos
- Las huellas fueron generadas con configuraciones incompatibles

## Mejores prácticas

- Use huellas tipo "compare" (generadas con `-t compare`) para mejores resultados
- Mantenga las huellas con sus videos fuente como referencia
- Documente el umbral de diferencia usado para su caso de uso

## Herramientas relacionadas

- `vfp_gen` : Genera huellas desde archivos de video
- `vfp_search` : Busca fragmentos dentro de videos
- `DVS` : Herramienta GUI para encontrar videos duplicados en carpetas
