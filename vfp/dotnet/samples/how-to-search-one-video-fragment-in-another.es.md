---
title: Buscar fragmentos de video con Fingerprinting en C#
description: Encuentre fragmentos de video dentro de archivos mayores usando el Video Fingerprinting SDK de VisioForge en C# .NET con implementación paso a paso.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - C#
primary_api_classes:
  - VFPFingerprintSource
  - VFSimplePlayerEngine
  - VFPAnalyzer

video:
  id: MM3IJxIxwww
  upload_date: 2026-11-24T09:52:31Z
  duration: PT1M8S
---

# Encontrar fragmentos de video en contenido de video más grande

## Introducción

La tecnología de huellas de video permite a los desarrolladores identificar y localizar segmentos específicos de video dentro de archivos de video más grandes. Esta guía demuestra el proceso de implementación usando un poderoso Video Fingerprinting SDK que funciona con varios formatos de video y niveles de calidad.

Los ejemplos principales en este tutorial usan la implementación de API .NET, pero funcionalidad equivalente está disponible a través de la API C++ para desarrolladores que prefieren soluciones de código nativo.

## Proceso de implementación

### Paso 1: Analizar el archivo de fragmento

Primero, necesitamos extraer una huella del fragmento de video más pequeño que queremos localizar dentro del video más grande. Este proceso implica analizar las características únicas del video y generar una firma digital.

```csharp
// VFPFingerprintSource solo tiene un ctor (string filename) — no existe sobrecarga
// con selección de motor. Limite la ventana de análisis vía StartTime/StopTime.
var fragmentSrc = new VFPFingerprintSource(ShortFile);
fragmentSrc.StopTime = TimeSpan.FromMilliseconds(5000);

// VFPAnalyzer.GetSearchFingerprintForVideoFile solo tiene la variante Async.
var fragment = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(fragmentSrc, ErrorCallback);
```

En este bloque de código:

- Creamos una fuente de huella apuntando al archivo de fragmento corto
- Establecemos un límite de duración de 5 segundos para el análisis
- Generamos una huella buscable usando el analizador

### Paso 2: Analizar el video objetivo

A continuación, necesitamos extraer una huella del video más grande donde buscaremos nuestro fragmento. El proceso es similar, pero sin limitaciones de tiempo.

```csharp
// El mismo ctor de un argumento + getter async para el archivo más largo.
var mainSrc = new VFPFingerprintSource(LongFile);

var main = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(mainSrc, ErrorCallback);
```

### Paso 3: Configurar el manejo de errores

Para mantener un manejo de errores robusto, implementamos una función de callback que captura y muestra cualquier error encontrado durante el proceso de generación de huellas.

```csharp
private static void ErrorCallback(string error)
{
    Console.WriteLine(error);
}
```

### Paso 4: Realizar la operación de búsqueda

Con ambas huellas listas, ahora podemos buscar el fragmento dentro del archivo de video más grande.

```csharp
// establecer el nivel máximo de diferencia
var maxDifference = 500;

// buscar un fragmento de video en otro video usando huellas
var res = VFPSearch.Search(fragment, 0, main, 0, out var difference, maxDifference);

// verificar el resultado
if (res != int.MaxValue)
{
    TimeSpan ts = new TimeSpan(res * TimeSpan.TicksPerSecond);
    Console.WriteLine($"Fragmento detectado en {ts:g}, nivel de diferencia es {difference}");
}
else
{
    Console.WriteLine("Archivo de fragmento no encontrado.");
}
```

En este código:

- Definimos un nivel de tolerancia para diferencias entre huellas
- Realizamos la operación de búsqueda entre nuestro fragmento y el video principal
- Verificamos si se encontró una posición coincidente (`Search` devuelve `int.MaxValue` cuando no encuentra el fragmento; 0 significa una coincidencia al inicio del video)
- Convertimos el resultado a una marca de tiempo y la mostramos junto con el valor de diferencia

### Encontrar cada coincidencia en unas pocas líneas

Una aplicación de consola completa: encuentra cada lugar en el que `clip.mp4` aparece en `movie.mp4` e imprime el inicio de cada coincidencia. `VFPAnalyzer.SearchAsync` recibe las dos huellas, la duración del clip, el límite de diferencia (aquí una coincidencia debe quedar por debajo de 500) y `true` para devolver todas las coincidencias y no solo la mejor, y devuelve el inicio de cada coincidencia como una `List<TimeSpan>`. `GetSearchFingerprintForVideoFileAsync` devuelve `null` cuando no puede analizar un archivo e imprime el motivo a través del callback; si falta un archivo, el constructor de `VFPFingerprintSource` lanza `FileNotFoundException`. Un clip de 10 segundos en 480p se encontró en los cinco lugares de 2 horas y 18 minutos de video 720p, en unos nueve minutos.

```csharp
using System;
using VisioForge.Core;
using VisioForge.Core.VideoFingerPrinting;
using static VisioForge.Core.VideoFingerPrinting.VFPAnalyzer;

await VisioForgeX.InitSDKAsync();
SetLicenseKey("TRIAL");
var clip = await
    GetSearchFingerprintForVideoFileAsync(
        new VFPFingerprintSource("clip.mp4"),
        Console.WriteLine);
var movie = await
    GetSearchFingerprintForVideoFileAsync(
        new VFPFingerprintSource("movie.mp4"),
        Console.WriteLine);
if (clip != null && movie != null)
{
    var found = await SearchAsync(
        clip, movie, clip.Duration, 500, true);
    foreach (var at in found)
        Console.WriteLine(
            $"Found at {at:hh\\:mm\\:ss}");
    if (found.Count == 0)
        Console.WriteLine("Not found");
}
VisioForgeX.DestroySDK();
```

El proyecto completo es [find-clip-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/_VideoTutorials/find-clip-short-console), una aplicación de consola para Windows x64 que referencia `VisioForge.DotNet.Core.Redist.VideoFingerprinting` y los paquetes nativos del SDK para esa plataforma, `VisioForge.CrossPlatform.Core.Windows.x64` y `VisioForge.CrossPlatform.Libav.Windows.x64`. Los nombres de archivo son relativos al directorio de trabajo, así que coloque ambos archivos en la carpeta desde la que inicia la aplicación. Sin clave de licencia, el SDK funciona en modo de prueba y muestra una vez el mensaje «Unregistered version»; haga clic en OK y la búsqueda continúa.

El video de un minuto escribe y ejecuta esta aplicación.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/MM3IJxIxwww?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Consideraciones de rendimiento

La tecnología de huellas usa algoritmos sofisticados que equilibran precisión y rendimiento. Para resultados óptimos:

- Considere ajustar el nivel máximo de diferencia basándose en sus requisitos específicos
- Procese videos en su resolución nativa cuando sea posible
- Para archivos muy grandes, considere dividir la búsqueda en fragmentos manejables

## Recursos adicionales

Para documentación completa, ejemplos de implementación en otros lenguajes e información de licencias, visite la [página del producto](https://www.visioforge.com/video-fingerprinting-sdk).
