---
title: Desinstalar Filtros DirectShow en Windows con regsvr32
description: Desinstale correctamente filtros DirectShow con técnicas manuales, pasos de solución de problemas y mejores prácticas para aplicaciones multimedia .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - Playback

---

# Eliminación de filtros DirectShow en Windows

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Los filtros DirectShow son componentes esenciales para aplicaciones multimedia en entornos Windows. Permiten que el software procese datos de audio y video de manera eficiente. Sin embargo, puede haber situaciones donde necesite desinstalar estos filtros, como al actualizar su aplicación, resolver conflictos o eliminar completamente un paquete de software. Esta guía proporciona instrucciones detalladas sobre cómo desinstalar correctamente filtros DirectShow de su sistema.

## Entender los filtros DirectShow

DirectShow es un framework multimedia y API diseñado por Microsoft para desarrolladores de software para realizar varias operaciones con archivos de medios. Está construido sobre la arquitectura Component Object Model (COM) y usa un enfoque modular donde cada paso de procesamiento es manejado por un componente separado llamado filtro.

Los filtros se categorizan en tres tipos principales:

- **Filtros fuente**: Leen datos de archivos, dispositivos de captura o flujos de red
- **Filtros de transformación**: Procesan o modifican los datos (compresión, descompresión, efectos)
- **Filtros renderizadores**: Muestran video o reproducen audio

Cuando los componentes del SDK se instalan, registran filtros DirectShow en el Registro de Windows, haciéndolos disponibles para cualquier aplicación que use el framework DirectShow.

## ¿Por qué desinstalar filtros DirectShow?

Hay varias razones por las que podría necesitar desinstalar filtros DirectShow:

1. **Conflictos de versión**: Versiones más nuevas del SDK podrían requerir eliminar filtros antiguos
2. **Limpieza del sistema**: Eliminar componentes no usados para mantener la eficiencia del sistema
3. **Solución de problemas**: Resolver problemas con aplicaciones multimedia
4. **Eliminación completa de software**: Asegurar que no queden componentes después de desinstalar la aplicación principal
5. **Re-registro**: A veces desinstalar y reinstalar filtros puede resolver problemas de registro

## Métodos para desinstalar filtros DirectShow

### Método 1: Usar el instalador del SDK (recomendado)

La manera más directa de desinstalar filtros DirectShow es a través del instalador del SDK (o redist) mismo. Los paquetes SDK incluyen rutinas de desinstalación que eliminan correctamente todos los componentes, incluyendo filtros DirectShow.

### Método 2: Desregistro Manual con regsvr32

Si la desinstalación automática no es posible o necesita desregistrar filtros específicos, puede usar la herramienta de línea de comandos `regsvr32`:

1. Abra el Símbolo del sistema como Administrador (haga clic derecho en Símbolo del sistema y seleccione "Ejecutar como administrador")
2. Use la siguiente sintaxis de comando para desregistrar un filtro:

   ```cmd
   regsvr32 /u "C:\ruta\al\filtro.dll"
   ```

3. Reemplace `C:\ruta\al\filtro.dll` con la ruta real al archivo del filtro DirectShow
4. Presione Enter para ejecutar el comando

Por ejemplo, para desregistrar un filtro ubicado en `C:\Program Files\Common Files\CarpetaFiltro\filtro_ejemplo.dll`, usaría:

```cmd
regsvr32 /u "C:\Program Files\Common Files\CarpetaFiltro\filtro_ejemplo.dll"
```

Debería ver un diálogo de confirmación indicando desregistro exitoso.

## Encontrar ubicaciones de filtros DirectShow

Antes de poder desregistrar filtros manualmente, necesita conocer sus ubicaciones. Aquí hay varios métodos para encontrar filtros DirectShow instalados:

### Usar GraphStudio

[GraphStudio](https://github.com/cplussharp/graph-studio-next) es una herramienta de código abierto poderosa para trabajar con filtros DirectShow. Para encontrar ubicaciones de filtros:

1. Descargue e instale GraphStudio
2. Inicie la aplicación con privilegios de administrador
3. Vaya a "Graph > Insert Filters"
4. Navegue a través de la lista de filtros instalados
5. Haga clic derecho en un filtro y seleccione "Properties"
6. Note la ruta "File:" mostrada en el diálogo de propiedades

Este método proporciona la ruta exacta del archivo necesaria para el desregistro manual.

### Usar el registro del sistema

También puede encontrar filtros DirectShow a través del Registro de Windows:

1. Presione `Win + R` para abrir el diálogo Ejecutar
2. Escriba `regedit` y presione Enter para abrir el Editor del Registro
3. Navegue a `HKEY_CLASSES_ROOT\CLSID`
4. Use la función de búsqueda (Ctrl+F) para encontrar nombres de filtros
5. Busque la clave "InprocServer32" bajo el CLSID del filtro, que contiene la ruta del archivo

## Consideraciones de plataforma (x86 vs x64)

Los filtros DirectShow son específicos de plataforma, lo que significa que las versiones de 32 bits (x86) y 64 bits (x64) son componentes separados. Si ha instalado ambas versiones, necesita desregistrar cada una por separado.

Para sistemas x64:

- Los filtros de 64 bits típicamente se instalan en `C:\Windows\System32`
- Los filtros de 32 bits típicamente se instalan en `C:\Windows\SysWOW64`

Use la versión apropiada de `regsvr32` para cada plataforma:

- Para filtros de 64 bits: `C:\Windows\System32\regsvr32.exe`
- Para filtros de 32 bits: `C:\Windows\SysWOW64\regsvr32.exe`

## Solución de problemas de desinstalación de filtros

Si encuentra problemas durante la desinstalación de filtros, intente estos pasos de solución de problemas:

### Incapaz de desregistrar el filtro

Si recibe un error como "DllUnregisterServer falló con código de error 0x80004005":

1. Asegúrese de estar ejecutando el Símbolo del sistema como Administrador
2. Verifique que la ruta al filtro sea correcta
3. Verifique si el archivo del filtro existe y no está en uso por ninguna aplicación
4. Cierre cualquier aplicación que pueda estar usando filtros DirectShow
5. En algunos casos, puede ser necesario reiniciar el sistema antes del desregistro

### El filtro sigue presente después del desregistro

Si un filtro parece seguir registrado después de intentar desregistrarlo:

1. Use GraphStudio para verificar si el filtro todavía aparece en la lista de filtros disponibles
2. Busque múltiples instancias del filtro en diferentes ubicaciones
3. Verifique tanto las ubicaciones de registro de 32 bits como de 64 bits
4. Intente usar la herramienta "OleView" proporcionada por Microsoft para inspeccionar registros COM

## Verificar la desinstalación exitosa

Después de desinstalar filtros DirectShow, verifique que la eliminación fue exitosa:

1. Use GraphStudio para verificar si los filtros ya no aparecen en la lista de filtros disponibles
2. Verifique el registro para cualquier entrada restante relacionada con los filtros
3. Pruebe cualquier aplicación que previamente usaba los filtros para asegurar que manejen la ausencia correctamente

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y ejemplos de implementación para trabajar con DirectShow y aplicaciones multimedia en .NET.