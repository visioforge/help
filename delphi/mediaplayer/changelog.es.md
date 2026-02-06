---
title: Actualizaciones y Funcionalidades de la Biblioteca Media Player
description: Registro de cambios de TVFMediaPlayer desde 3.0 hasta 10.0: soporte 4K, cifrado, efectos, streaming y actualizaciones de rendimiento.
---

# Registro de Cambios de la Biblioteca TVFMediaPlayer

Este documento detalla la evolución de la biblioteca TVFMediaPlayer, documentando las funcionalidades significativas, mejoras, optimizaciones y correcciones de errores introducidas en las diversas versiones. Sirve como referencia completa para desarrolladores que rastrean el progreso de la biblioteca y comprenden las capacidades añadidas con el tiempo.

## Versión 10.0: Manejo de Medios Mejorado y Personalización

La versión 10.0 representa un avance significativo, enfocándose en introspección de medios mejorada, registro, personalización y compatibilidad.

### Mejoras Principales

* **Lector de Información de Medios Mejorado:** Esta versión potencia significativamente las capacidades del lector de información de medios. Permite una extracción más rápida y precisa de metadatos de una amplia gama de tipos de archivos multimedia. Los desarrolladores obtienen acceso confiable a detalles críticos como duración, resolución, especificaciones de códec, tasas de bits y etiquetas incrustadas, lo que simplifica la gestión de medios y mejora las capacidades de visualización dentro de las aplicaciones.
* **Capacidades de Registro Mejoradas:** El registro ha sido sustancialmente refinado, ofreciendo a los desarrolladores un control más granular. Las opciones de configuración ahora incluyen niveles de registro distintos (Depuración, Info, Advertencia, Error) y destinos de salida flexibles como archivos, consola o endpoints personalizados. Esto facilita un diagnóstico más efectivo de problemas durante el desarrollo y un monitoreo robusto del comportamiento de la aplicación en producción, lo que resulta en una solución de problemas más rápida y mayor estabilidad de la aplicación.
* **Soporte de Etiquetas de Metadatos Estándar:** Una piedra angular de esta versión es la introducción de soporte completo para leer etiquetas de metadatos estándar incrustadas en contenedores populares de video y audio. Esto incluye formatos como MP4, WMV, MP3, AAC, M4A y Ogg Vorbis. Las aplicaciones que utilizan TVFMediaPlayer ahora pueden extraer y aprovechar sin problemas etiquetas comunes como título, artista, álbum, género, año y arte de portada, enriqueciendo así la experiencia del usuario al proporcionar contexto valioso para el medio que se reproduce.

### Mejoras de Captura y Efectos

* **Nombres de Archivo de División Automática Configurables:** La nueva propiedad `SeparateCapture_Filename_Mask` proporciona control detallado sobre los nombres de archivo al usar la función de captura de división automática basada en duración o tamaño. Esto permite convenciones de nomenclatura personalizadas, mejorando la organización y el flujo de trabajo para grabaciones segmentadas.
* **Serialización de Configuraciones JSON:** Las configuraciones del reproductor multimedia ahora pueden ser fácilmente serializadas y deserializadas desde el formato JSON ampliamente utilizado. Esto simplifica el guardado y carga de configuraciones del reproductor, habilitando configuraciones persistentes e integración más fácil con sistemas de gestión de configuración.
* **Pipeline de Efectos de Video Personalizados:** La flexibilidad en el procesamiento de video se mejora con la capacidad de insertar efectos de video personalizados usando filtros de terceros identificados por su CLSID. Estos filtros pueden colocarse estratégicamente antes o después del filtro de efectos principal o del sample grabber, permitiendo pipelines de manipulación de video sofisticados y personalizados.
* **Efectos de Video Optimizados:** El procesamiento de efectos de video ha sido optimizado para aprovechar al máximo las últimas generaciones de CPUs Intel, resultando en reproducción más fluida y menor consumo de recursos al aplicar efectos.

### Correcciones de Fuente y Compatibilidad

* **Divisor MP3 para Problemas de Reproducción:** Se ha integrado un divisor MP3 para abordar y resolver específicamente inconsistencias de reproducción encontradas con ciertos archivos MP3 no estándar o problemáticos, asegurando mayor compatibilidad.
* **Filtro de Fuente VLC Actualizado:** El filtro de fuente VLC subyacente ha sido actualizado a libVLC versión 2.2.2.0. Esta actualización trae mejoras notables, particularmente en el manejo de streams RTMP y HTTPS, y resuelve fugas de memoria previamente identificadas, contribuyendo a mayor estabilidad y soporte más amplio de protocolos de streaming.
* **Correcciones de Efectos Pan y Blur:** Se han abordado y resuelto problemas específicos relacionados con el efecto Pan en compilaciones x64 y el efecto Blur, asegurando un comportamiento consistente de efectos visuales en diferentes arquitecturas.
* **Fuga de Memoria de Fuente FFMPEG Resuelta:** Se ha identificado y corregido una fuga de memoria asociada con el componente de fuente FFMPEG, mejorando la estabilidad a largo plazo y la gestión de recursos durante la reproducción.

## Versión 9.2: Actualizaciones de Motor y Mejoras del Lector

Esta versión intermedia se enfocó en actualizar componentes centrales y refinar aún más las capacidades de información de medios.

* **Motor VLC Actualizado:** El motor VLC integrado fue actualizado a libVLC versión 2.2.1.0, incorporando correcciones y mejoras upstream del proyecto VLC para mejor estabilidad y compatibilidad de formatos.
* **Lector de Información de Medios Mejorado:** Construyendo sobre mejoras previas, el lector de información de medios recibió más mejoras para soporte de archivos más amplio y extracción de metadatos más precisa.
* **Motor FFMPEG Actualizado:** Los componentes del motor FFMPEG fueron actualizados, asegurando compatibilidad con códecs y formatos más nuevos mientras incorporan optimizaciones de rendimiento.

## Versión 9.1: Integración de Seguridad Avanzada

La versión 9.1 introdujo funcionalidades de seguridad robustas a través de la integración con el SDK de Cifrado de Video.

* **Soporte de SDK de Cifrado de Video v9:** Esta versión añadió compatibilidad con el SDK de Cifrado de Video v9. Esto permite a los desarrolladores implementar cifrado AES-256 fuerte para su contenido de video, usando archivos de clave separados o datos binarios incrustados como claves, mejorando significativamente las capacidades de protección de contenido.

## Versión 9.0: Mejoras de Audio y Flexibilidad de Logo

La versión 9.0 trajo mejoras significativas al manejo de audio y opciones de marca visual.

* **Soporte de Logo GIF Animado:** La capacidad de usar logos de imagen se expandió para incluir soporte para GIFs animados, permitiendo marcas visuales más dinámicas y atractivas dentro de la interfaz de reproducción de video.
* **Mejoras de Audio:** Se introdujo un conjunto de funciones de mejora de audio, incluyendo normalización de audio para asegurar niveles de volumen consistentes, control automático de ganancia (AGC) para ajustar dinámicamente el volumen, y controles de ganancia manual para ajustes precisos de niveles de audio.
* **Volumen de Audio Basado en Porcentaje:** La API para controlar el volumen de audio fue modernizada para usar un sistema basado en porcentaje (0-100%), proporcionando una manera más intuitiva y estandarizada de gestionar niveles de audio comparada con métodos anteriores.

## Versión 8.6: Expansión de Decodificador y Adiciones de API

Esta versión se enfocó en expandir el soporte de códecs, añadir flexibilidad a través de filtros personalizados y refinar la API.

* **Decodificador H264 CPU/Intel QuickSync:** Se añadió un decodificador de video H264 altamente optimizado, aprovechando tanto recursos de CPU como aceleración de hardware Intel QuickSync cuando está disponible. Esto mejora significativamente el rendimiento para decodificar uno de los códecs de video más comunes.
* **Soporte de Filtro de Video DirectShow Personalizado:** Los desarrolladores ganaron la capacidad de integrar sus propios filtros de video DirectShow personalizados en el grafo de reproducción, permitiendo tareas de procesamiento de video altamente especializadas.
* **Evento `OnNewFilePlaybackStarted`:** Se introdujo un nuevo evento, `OnNewFilePlaybackStarted`. Este evento se dispara específicamente cuando un nuevo archivo comienza a reproducirse dentro de un contexto de lista de reproducción, permitiendo a las aplicaciones reaccionar precisamente a las transiciones entre elementos multimedia.
* **Decodificadores Actualizados:** Los decodificadores de audio Ogg Vorbis y video WebM fueron actualizados a sus últimas versiones, asegurando compatibilidad y mejoras de rendimiento.
* **Actualización de API de Captura de Fotogramas:** La API para capturar fotogramas de video individuales fue actualizada, potencialmente ofreciendo rendimiento o flexibilidad mejorados.
* **Correcciones de Errores:** Se implementaron varias correcciones de errores no especificadas para mejorar la estabilidad y confiabilidad general.

## Versión 8.5: Rotación, Preparación para 4K y Opciones de Renderizado

La versión 8.5 introdujo funcionalidades innovadoras de manipulación de video y preparó el motor para contenido de ultra alta definición.

* **Rotación de Video en Tiempo Real:** Se añadió un nuevo efecto de video, habilitando la rotación en tiempo real del stream de video durante la reproducción (ej., 90, 180, 270 grados).
* **Fuente FFMPEG Actualizada:** El componente de fuente FFMPEG fue actualizado, probablemente incorporando soporte para formatos más nuevos o mejorando el rendimiento.
* **Efectos de Video Listos para 4K:** Los efectos de video existentes fueron optimizados y probados para asegurar que funcionan eficientemente con contenido de video de resolución 4K.
* **Corrección de Error de Desplazamiento de Zoom VMR-9/EVR:** Se corrigió un error específico relacionado con desplazamiento inesperado de imagen al usar zoom con los renderizadores de video VMR-9 o EVR.
* **Renderizador de Video Direct2D (Beta):** Se introdujo un nuevo renderizador de video basado en Direct2D como función beta. Este renderizador incluía soporte para rotación de video en vivo y buscaba aprovechar APIs de gráficos modernas para rendimiento y calidad potencialmente mejorados.
* **Correcciones de Errores:** Incluyó varias correcciones generales de errores para mejorar la estabilidad.

## Versión 8.4: Actualizaciones de Decodificador y Estabilidad

Esta fue principalmente una versión de mantenimiento enfocada en actualizar componentes centrales.

* **Decodificador FFMPEG Actualizado:** Los componentes del decodificador FFMPEG fueron actualizados, probablemente incorporando correcciones y mejoras del proyecto FFMPEG.
* **Correcciones de Errores:** Se abordaron varios errores no especificados para mejorar la estabilidad.

## Versión 8.3: Versión de Estabilidad

Esta versión se enfocó únicamente en abordar errores identificados en versiones anteriores.

* **Correcciones de Errores:** Se implementaron varias correcciones para mejorar la confiabilidad y estabilidad general de la biblioteca.

## Versión 8.0: Introduciendo el Motor VLC

La versión 8.0 marcó una adición arquitectónica significativa al integrar el potente motor VLC.

* **Integración del Motor VLC:** El reconocido motor VLC fue integrado como un backend de reproducción alternativo para archivos de video y audio. Esto trajo el extenso soporte de formatos y capacidades robustas de streaming de VLC a las aplicaciones TVFMediaPlayer.
* **Correcciones de Errores:** Incluyó varias correcciones generales de errores.

## Serie de Versiones 7.x: Efectos, Cifrado y Listas de Reproducción

La serie de versiones 7 introdujo funcionalidades clave relacionadas con control de reproducción, seguridad y efectos visuales.

### Versión 7.20

* **Reproducción Inversa:** Se añadió la capacidad de reproducir archivos de video en reversa, abriendo posibilidades creativas y casos de uso de aplicaciones especializadas.
* **Correcciones de Errores:** Se abordaron varios errores.

### Versión 7.12

* **Soporte de Cifrado de Video:** Se añadió soporte inicial para cifrado de video, proporcionando mecanismos básicos de protección de contenido.
* **Correcciones de Errores:** Incluyó mejoras generales de estabilidad.

### Versión 7.7

* **Efecto Fade-In/Fade-Out:** Se añadió un efecto de transición de video común y útil, fade-in/fade-out, a los efectos de video disponibles.
* **Soporte de Lista de Reproducción:** Se introdujo funcionalidad para crear y gestionar listas de reproducción, permitiendo que secuencias de archivos multimedia se reproduzcan automáticamente.
* **Correcciones de Errores:** Se abordaron varios problemas.

### Versión 7.5

* **Chroma Key Mejorado:** El efecto chroma key (pantalla verde) fue mejorado para mejor calidad y control más preciso.
* **Logo de Texto Mejorado:** Se mejoró la función para superponer logos de texto sobre el video.
* **API de Efectos de Video Modificada:** La API para aplicar efectos de video sufrió modificaciones, potencialmente para usabilidad mejorada o para acomodar nuevas funcionalidades.
* **Correcciones de Errores:** Incluyó varias correcciones de estabilidad.

### Versión 7.0

* **Soporte para Windows 8 RTM:** Se aseguró compatibilidad con la versión final de Windows 8.
* **Efectos de Video Mejorados:** Se hicieron más mejoras a la calidad y rendimiento de los efectos de video existentes.
* **Nuevo Motor de Reproducción FFMPEG:** Se introdujo un nuevo motor de reproducción basado en componentes FFMPEG, ofreciendo una alternativa a la reproducción basada en DirectShow por defecto y expandiendo la compatibilidad de formatos.

## Serie de Versiones 6.x: Compatibilidad con Windows 8 y Optimizaciones

La serie de versiones 6 se enfocó en adaptarse al entonces nuevo sistema operativo Windows 8 y mejorar el rendimiento.

### Versión 6.3

* **Soporte para Windows 8 Customer Preview:** Se añadió compatibilidad para la versión Customer Preview de Windows 8.
* **Efectos de Video Mejorados:** Continuó el refinamiento del rendimiento y calidad de los efectos de video.

### Versión 6.0

* **Soporte OpenCL Mejorado:** Se mejoró la utilización de OpenCL para tareas de aceleración GPU, potencialmente aumentando el rendimiento para efectos o decodificación en hardware compatible.
* **Soporte para Windows 8 Developer Preview:** Se añadió soporte temprano para la versión Developer Preview de Windows 8.
* **Efectos de Video Mejorados:** Mejoras generales al subsistema de efectos de video.

## Serie de Versiones 3.x: Funcionalidades Tempranas y Optimizaciones

La serie de versiones 3 sentó las bases de funcionalidades y se enfocó en optimizaciones específicas de CPU.

### Versión 3.9

* **Nuevos Instaladores:** Se introdujo un nuevo instalador principal e instaladores redistribuibles separados para despliegue más fácil.
* **Correcciones Menores de Errores:** Se abordaron problemas menores pendientes.

### Versión 3.7

* **Efectos de Video Mejorados:** Se hicieron mejoras a las funcionalidades de efectos de video.
* **Nuevas Aplicaciones Demo:** Se añadieron nuevas aplicaciones demo para demostrar las capacidades de la biblioteca.
* **Optimizaciones de CPU para Netbooks:** Se incluyeron optimizaciones de rendimiento específicas adaptadas para procesadores Intel Core II/Atom y AMD de netbooks.
* **Correcciones Menores de Errores:** Mejoras generales de estabilidad.

### Versión 3.5

* **Efectos de Video Mejorados:** Continuó el trabajo en mejorar los efectos de video.
* **Optimizaciones para Intel Core i7:** Se añadieron nuevas optimizaciones de rendimiento específicamente para la entonces nueva arquitectura de CPU Intel Core i7.

### Versión 3.0

* **Detección de Movimiento:** Se introdujo una función de detección de movimiento, habilitando a las aplicaciones reaccionar a cambios dentro del stream de video.
* **Chroma Key:** Se añadió funcionalidad inicial de chroma key (pantalla verde).
* **Soporte de Fuente MMS/WMV:** Se incluyó soporte para streaming usando el protocolo MMS y reproducción de archivos WMV (Windows Media Video).
* **Optimizaciones de CPU:** Se añadieron optimizaciones de rendimiento dirigidas a procesadores Intel Atom y Core i3/i5/i7.
* **Procesamiento Directo de Stream:** Se habilitó la capacidad de acceder y procesar directamente datos de stream de video y audio decodificados, ofreciendo posibilidades avanzadas de manipulación.
