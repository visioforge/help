---
title: Generador de huellas de video - herramienta CLI vfp_gen .NET
description: Genere huellas de video desde línea de comandos usando la herramienta CLI vfp_gen de VisioForge para .NET con modos de búsqueda y comparación.
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

# vfp_gen - Generador de Huellas de Video

📦 **Código Fuente**: [Ver en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/Console/vfp_gen)

## Descripción General

`vfp_gen` es una herramienta de línea de comandos que genera huellas digitales (firmas) de archivos de video. Estas huellas pueden utilizarse para comparación de videos, detección de duplicados o búsqueda de fragmentos.

## Características

- Genera huellas optimizadas para operaciones de comparación o búsqueda
- Procesa videos completos o duraciones específicas
- Soporte para todos los formatos de video principales (MP4, AVI, MKV, MOV, etc.)
- Compatibilidad multiplataforma (Windows x64)

## Uso

```bash
vfp_gen -i "video_entrada.mp4" -o "salida.vsigx" [opciones]
```

### Parámetros Requeridos

- `-i, --input` : Ruta al archivo de video de entrada
- `-o, --output` : Ruta donde se guardará el archivo de huella (típicamente con extensión .vsigx)

### Parámetros Opcionales

- `-t, --type` : Tipo de huella (predeterminado: "search")
  - `search` : Optimizado para encontrar este video como fragmento dentro de otros videos
  - `compare` : Optimizado para comparar videos completos
- `-d, --duration` : Duración a analizar en milisegundos (predeterminado: 0 = archivo completo)
- `-l, --license` : Clave de licencia VisioForge (predeterminado: "TRIAL")

## Ejemplos

### Generar una huella de búsqueda para el video completo
```bash
vfp_gen -i "comercial.mp4" -o "comercial.vsigx"
```

### Generar una huella de comparación
```bash
vfp_gen -i "pelicula.mp4" -o "pelicula_compare.vsigx" -t compare
```

### Generar huella solo para los primeros 30 segundos
```bash
vfp_gen -i "video.mp4" -o "video_30s.vsigx" -d 30000
```

### Usar con clave de licencia
```bash
vfp_gen -i "video.mp4" -o "salida.vsigx" -l "TU-CLAVE-DE-LICENCIA"
```

## Salida

La herramienta genera un archivo de huella binario (`.vsigx`) que contiene:
- Datos de la huella
- Metadatos del video (duración, dimensiones, tasa de fotogramas)
- Referencia al nombre del archivo fuente
- Identificador único

## Casos de Uso

1. **Identificación de Contenido**: Genera huellas para una biblioteca de videos para identificar duplicados
2. **Detección de Publicidad**: Crea huellas de comerciales para encontrarlos en transmisiones
3. **Detección de Escenas**: Genera huellas de escenas específicas para localizarlas en películas completas
4. **Protección de Derechos de Autor**: Crea huellas de contenido con derechos de autor para monitoreo

## Notas de Rendimiento

- La generación de huellas es intensiva en CPU
- El tiempo de procesamiento depende de la duración y resolución del video
- Los archivos de huella generados son típicamente pequeños (rango de KB a MB)
- La herramienta muestra el porcentaje de progreso durante el procesamiento

## Manejo de Errores

La herramienta saldrá con un mensaje de error si:
- El archivo de entrada no existe
- El archivo de salida no puede ser creado/sobrescrito
- El formato de video no es compatible
- Memoria insuficiente para el procesamiento

## Herramientas Relacionadas

- `vfp_compare` : Compara dos archivos de huella
- `vfp_search` : Busca una huella dentro de otra huella
