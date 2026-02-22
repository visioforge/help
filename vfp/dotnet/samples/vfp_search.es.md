---
title: Búsqueda de Fragmentos de Video en .NET - Guía de vfp_search
description: Usa vfp_search para buscar fragmentos de video dentro de videos más grandes con características para encontrar y administrar tu biblioteca.
---

# vfp_search - Herramienta de Búsqueda de Fragmentos de Video

📦 **Código Fuente**: [Ver en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/Console/vfp_search)

## Descripción General

`vfp_search` es una herramienta de línea de comandos que busca un fragmento de video (como un comercial, intro o escena específica) dentro de un video más grande. Utiliza huellas pregeneradas para localizar rápidamente dónde aparece el fragmento en el video principal.

## Características

- Encuentra fragmentos de video dentro de videos de larga duración
- Detecta comerciales en grabaciones de transmisiones
- Localiza escenas o clips específicos
- Búsqueda rápida sin reprocesar videos
- Devuelve marcas de tiempo exactas de las coincidencias

## Uso

```bash
vfp_search -f "fragmento.vsigx" -m "video_principal.vsigx" [opciones]
```

### Parámetros Requeridos

- `-f, --fragment` : Ruta al archivo de huella del fragmento (el segmento de video a buscar)
- `-m, --main` : Ruta al archivo de huella del video principal (donde buscar)

### Parámetros Opcionales

- `-d, --md` : Diferencia máxima aceptable (predeterminado: 500)
- `-l, --license` : Clave de licencia VisioForge (predeterminado: "TRIAL")

## Ejemplos

### Buscar un comercial en una grabación de TV
```bash
vfp_search -f "comercial.vsigx" -m "grabacion_tv.vsigx"
```

### Búsqueda con coincidencia más estricta
```bash
vfp_search -f "intro.vsigx" -m "pelicula.vsigx" -d 50
```

### Usando clave de licencia
```bash
vfp_search -f "escena.vsigx" -m "pelicula_completa.vsigx" -l "TU-CLAVE-DE-LICENCIA"
```

## Salida

La herramienta muestra:
- Número de coincidencias encontradas
- Marca de tiempo para cada coincidencia (formato: HH:MM:SS)
- Puntuación de diferencia para cada coincidencia
- Tiempo total de procesamiento

Ejemplo de salida:
```
Starting analyze.
Analyze finished. Elapsed time: 0:00:01.234
Search results: 3
00:05:32
01:23:45
02:15:18
```

## Ejemplo de Flujo de Trabajo

1. Genera la huella para el fragmento (ej., comercial de 30 segundos):
```bash
vfp_gen -i "comercial.mp4" -o "comercial.vsigx" -t search
```

2. Genera la huella para el video completo:
```bash
vfp_gen -i "transmision.mp4" -o "transmision.vsigx" -t compare
```

3. Busca el comercial en la transmisión:
```bash
vfp_search -f "comercial.vsigx" -m "transmision.vsigx"
```

## Casos de Uso

1. **Detección de Publicidad**: Encuentra y salta comerciales en programas de TV grabados
2. **Monitoreo de Contenido**: Detecta cuándo aparece contenido específico en transmisiones
3. **Localización de Escenas**: Encuentra escenas específicas en múltiples archivos de video
4. **Detección de Intro/Outro**: Localiza segmentos recurrentes en series
5. **Monitoreo de Derechos de Autor**: Encuentra uso no autorizado de clips de video

## Mejores Prácticas

- Usa huellas tipo "search" para fragmentos (`-t search` en vfp_gen)
- Usa huellas tipo "compare" para videos principales (`-t compare` en vfp_gen)
- Los fragmentos deben tener al menos 5-10 segundos para detección confiable
- Umbrales de diferencia más bajos (< 100) para coincidencias exactas
- Umbrales más altos (100-500) para contenido similar con modificaciones

## Notas de Rendimiento

- La velocidad de búsqueda depende de la duración del video principal
- El uso de memoria es proporcional al tamaño de las huellas
- Típicamente procesa horas de video en segundos

## Manejo de Errores

La herramienta saldrá con un error si:
- Alguno de los archivos de huella no existe
- El fragmento es más largo que el video principal
- Los archivos de huella están corruptos
- Tipos de huella incompatibles

## Limitaciones

- El fragmento debe ser continuo (sin cortes o ediciones)
- Fragmentos muy cortos (< 5 segundos) pueden producir falsos positivos
- Contenido muy modificado puede no ser detectado

## Herramientas Relacionadas

- `vfp_gen` : Genera huellas desde archivos de video
- `vfp_compare` : Compara dos videos completos
- `MMT` : Herramienta GUI para monitoreo de medios con búsqueda de fragmentos
