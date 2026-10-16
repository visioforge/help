---
title: Telemetría y recopilación de datos en los SDK .NET
description: Qué informan los SDK .NET de VisioForge mientras hay un depurador adjunto, qué no se recopila nunca y cómo desactivar la telemetría.
sidebar_label: Telemetría y Privacidad
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
primary_api_classes:
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline

---

# Telemetría y privacidad

Cada motor expone una propiedad `Debug_Telemetry`. Cuando es `true` **y hay un depurador adjunto a su proceso**, el SDK informa de sus propios errores a VisioForge, de modo que los fallos que usted encuentre al integrar el SDK nos lleguen sin que nadie tenga que abrir un ticket, junto con los nombres de las funciones del SDK que usted utiliza, para que sepamos cuáles le importan.

Para los errores de un motor, las tres condiciones son necesarias, y todas se comprueban cada vez que se enviaría un evento; los errores que se producen fuera de un motor solo necesitan las dos últimas (véase más abajo):

- `Debug_Telemetry` es `true` (el valor predeterminado).
- `VisioForgeX.Debug_Telemetry` es `true` (el valor predeterminado) - el interruptor global, véase más abajo.
- `System.Diagnostics.Debugger.IsAttached` es `true`.

Una aplicación que usted compila y distribuye a sus usuarios se ejecuta sin depurador, así que **una aplicación publicada no envía nada**. Desconectar el depurador detiene el envío de inmediato; adjuntar uno a mitad de sesión lo inicia.

## Cómo desactivarlo

Establezca la propiedad en `false` antes de iniciar el motor:

```csharp
// Cualquiera de los motores - VideoCaptureCoreX, VideoCaptureCore, MediaPlayerCoreX,
// MediaPlayerCore, VideoEditCoreX, VideoEditCore, SimplePlayerX,
// LiveVideoCompositor, VideoFrameSource, FrameSource.
core.Debug_Telemetry = false;
```

```csharp
var pipeline = new MediaBlocksPipeline();
pipeline.Debug_Telemetry = false;
```

Para desactivarlo todo a la vez - todos los motores y los errores que se producen fuera de un motor (fábricas de configuración de fuentes como `UniversalSourceSettings.CreateAsync`, enumeración de dispositivos, lectura de información multimedia) junto con las excepciones que lanzan - use el interruptor global. Esos errores fuera de un motor dependen solo de este interruptor, no de la propiedad de un motor:

```csharp
VisioForgeX.Debug_Telemetry = false;
```

La propiedad puede establecerse en cualquier momento; lo que cuenta es su estado en el instante en que se produce el error.

## Qué se envía

- El nombre de la clase y del método y el nivel del evento de registro.
- El mensaje de registro, con las credenciales, rutas de archivo, nombres de host, direcciones IP y MAC, identificadores de dispositivo, GUID y direcciones de correo electrónico eliminados.
- El tipo de excepción, su mensaje depurado de datos sensibles y los marcos de pila. Un marco conserva el nombre del método y el número de línea; la ruta del archivo de origen se elimina, porque refleja la estructura de directorios de su máquina de compilación.
- Hasta 50 líneas de registro anteriores como migas de pan, depuradas de la misma manera, y hasta 30 advertencias y errores de GStreamer anteriores, depurados de la misma manera.
- Cuando un pipeline de GStreamer informa de un error - un `MediaBlocksPipeline`, o aquel sobre el que se ejecuta `VideoCaptureCoreX` o `MediaPlayerCoreX` -, el primero de esos errores de cada pipeline en una ejecución (un motor puede ejecutar varios) lleva el grafo de ese pipeline: el tipo y el nombre de cada elemento, cómo están enlazados los elementos, el formato multimedia de cada enlace (por ejemplo `video/x-raw, format=NV12, width=1920, height=1080`), el estado de los elementos y los valores no predeterminados de los pads que GStreamer escribe en el grafo, como la posición de una entrada de mezclador. Las propiedades de los elementos - donde están los nombres de archivo, las URL y los nombres de dispositivo - quedan excluidas, y el grafo se depura de la misma manera que los mensajes.
- La versión del SDK, el runtime de .NET, la descripción del sistema operativo, la arquitectura del proceso, el framework de destino, el nombre del motor y un identificador aleatorio que se regenera en cada inicio del proceso.
- Al iniciarse un motor o un pipeline, o al intentarlo, los nombres de las funciones del SDK que utiliza: el motor (`VideoCaptureCoreX`, `MediaBlocksPipeline`, ...), los nombres de tipo del SDK de sus bloques, de su formato de salida y de su modo (`MP4OutputBlock`, `VideoCaptureMode.VideoCapture`, ...). Ambas condiciones se comprueban al iniciarse el motor. Cada nombre se envía una sola vez por proceso y por motor (de nuevo en el siguiente inicio si el envío falló), junto con el tipo de compilación del propio SDK (debug o release). Solo se nombran los tipos declarados por el SDK: una clase propia de usted, como un bloque personalizado, nunca se informa.

## Qué no se envía nunca

- Sus ajustes: aparte del modo del motor y de los formatos multimedia y valores de pads del grafo del pipeline mencionados arriba, no se envía ningún valor de propiedad de un bloque, fuente, salida o motor. La lista de funciones solo nombra tipos del SDK; no describe cómo usted los conectó ni cómo los configuró.
- Nombres o rutas de archivo.
- Identificadores de dispositivo: rutas de instancia PnP, moniker de DirectShow y GUID.
- La identidad del usuario, en ninguna forma.
- Variables de entorno.
- Contenido multimedia - ni fotogramas, ni muestras, ni capturas.
- Ningún identificador que sobreviva a un reinicio. El identificador aleatorio existe solo para agrupar los eventos de una sesión de depuración, y desaparece cuando el proceso termina.

Una cosa que esa lista no promete: un mensaje de error a menudo nombra el elemento o el códec al que se refiere - «Failed to link h264parse to qtmux» es un caso típico -, de modo que los nombres de elementos y de códecs sí aparecen dentro de los mensajes informados. Lo que nunca se envía es una descripción de su configuración más allá del grafo de un pipeline que ha fallado; lo que se envía es el texto del error en sí.

La carga útil no contiene ningún objeto de usuario ni ninguna dirección IP. La propia conexión revela necesariamente su dirección al servidor, que está configurado para descartarla en lugar de almacenarla junto al evento.

## Cuánto se informa

El propio nivel de registro del SDK decide qué puede informarse:

- Con `Debug_Mode` **desactivado**, el SDK registra únicamente errores, de modo que los errores son todo lo que puede enviarse, y las migas de pan contienen solo las advertencias y los errores propios de GStreamer.
- Con `Debug_Mode` **activado**, el SDK registra desde `Debug` hacia arriba. Se informan advertencias y errores; las líneas de depuración e informativas se conservan solo como migas de pan adjuntas al siguiente error.
- En cuanto un motor o un pipeline con `Debug_Telemetry` activado se inicia bajo un depurador, el nivel de registro predeterminado de GStreamer pasa de errores a advertencias durante el resto del proceso, salvo que usted haya asignado a `VisioForgeX.DebugLevel` un valor distinto del predeterminado. El SDK pasa esas advertencias solo a las migas de pan, no a su salida de depuración; la salida de consola propia de GStreamer (stderr) sí las muestra.

## Adónde va

Los informes se envían por HTTPS a `https://telemetry.visioforge.org`, un servidor operado por VisioForge. Las advertencias se ponen en cola y las entrega un hilo en segundo plano, de modo que registrar una no le cuesta nada al hilo que la registra. Un error es distinto: el hilo que lo registró espera hasta 300 milisegundos a que termine el envío, porque «Detener depuración» mata el proceso y el último error es el que vale la pena conservar. Superado ese margen, el hilo continúa y la petición termina en segundo plano. Si la red no está disponible, o se alcanza el límite de frecuencia de envío, el informe se descarta en lugar de reintentarse indefinidamente.

Los nombres de funciones se envían por HTTPS a `https://analytics.visioforge.org`, un servidor de análisis independiente operado por VisioForge, desde una tarea en segundo plano que nunca retrasa al llamador. Un envío que falla no se reintenta en segundo plano; sus nombres se envían de nuevo en el siguiente inicio del motor. Ese servidor no conserva ninguna dirección IP: deriva de la conexión un hash diario anónimo, y la sal de ese hash se elimina a los dos días.

La primera vez que un proceso envía algo, el SDK escribe una línea que indica qué se envía, adónde y cómo desactivarlo, tanto en el registro como en la salida de depuración.

## Véase también

- [Envío de registros](sendlogs.md) - recopilar el registro de depuración completo para soporte
- [Política de privacidad](https://www.visioforge.com/privacy-policy) - la política de empresa bajo la que se encuadra esta función
