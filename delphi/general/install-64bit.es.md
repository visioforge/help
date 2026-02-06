---
title: Guía de Instalación de Paquetes Delphi de 64 bits
description: Instala paquetes Delphi de 64 bits: configura rutas de biblioteca, gestiona paquetes en tiempo de ejecución y resuelve problemas de instalación.
---

# Dominar la Instalación de Paquetes Delphi de 64 bits

## Introducción al Desarrollo de 64 bits en Delphi

La evolución hacia la computación de 64 bits representa un avance significativo para los desarrolladores de Delphi, abriendo puertas a un rendimiento mejorado, capacidades expandidas de direccionamiento de memoria y una utilización optimizada de recursos. Desde la introducción del soporte de 64 bits en Delphi XE2, los desarrolladores han ganado la poderosa capacidad de compilar aplicaciones nativas de Windows de 64 bits. Esta capacidad permite que el software aproveche las arquitecturas de hardware modernas, acceda a espacios de memoria sustancialmente más grandes y ofrezca un rendimiento optimizado para operaciones intensivas en datos.

Sin embargo, esta progresión tecnológica introduce un conjunto distintivo de complejidades, particularmente con respecto a la instalación y gestión de paquetes de componentes (archivos `.bpl`). Muchos desarrolladores de Delphi encuentran obstáculos desconcertantes al intentar integrar paquetes de 64 bits en su flujo de trabajo de desarrollo, lo que lleva a la frustración y la pérdida de productividad.

Esta guía detallada explora estos desafíos a fondo y proporciona soluciones meticulosamente detalladas y accionables. El problema fundamental se origina en una característica arquitectónica crítica: **el Entorno de Desarrollo Integrado (IDE) de Delphi sigue siendo una aplicación de 32 bits**, incluso en las versiones más recientes. Esta discrepancia arquitectónica entre el IDE de 32 bits y el objetivo de compilación de 64 bits crea numerosos malentendidos y dificultades técnicas relacionadas con la gestión de paquetes.

Comprender esta limitación arquitectónica constituye el primer paso esencial para establecer una experiencia de desarrollo fluida. Examinaremos a fondo por qué el IDE de 32 bits requiere paquetes de tiempo de diseño de 32 bits, exploraremos las técnicas adecuadas de configuración de proyectos para objetivos tanto de 32 bits como de 64 bits, aclararemos la función crítica de los paquetes en tiempo de ejecución y describiremos metodologías de prueba extensas para asegurar que sus aplicaciones funcionen perfectamente en ambos entornos arquitectónicos.

## La Limitación Arquitectónica: Por Qué el IDE de 32 bits Requiere Paquetes de Tiempo de Diseño de 32 bits

### Comprendiendo la Arquitectura del IDE

El IDE de Delphi sirve como el entorno principal para el diseño visual de componentes, edición de código, operaciones de depuración y gestión integral de proyectos. Cuando los diseñadores colocan componentes en formularios utilizando el Diseñador de Formularios, modifican propiedades a través del Inspector de Objetos o utilizan editores de componentes especializados, el IDE debe cargar y ejecutar el código contenido dentro del paquete de tiempo de diseño del componente.

Debido a que `bds.exe` (el ejecutable del IDE de Delphi) opera como un proceso de 32 bits, funciona exclusivamente dentro del espacio de direcciones de memoria de 32 bits y debe adherirse a las restricciones de los entornos de ejecución de 32 bits. El IDE físicamente no puede cargar ni ejecutar código de 64 bits directamente; esto representa una limitación de hardware y sistema operativo, no simplemente una restricción de software. Cualquier intento de cargar una DLL de 64 bits (o en terminología de Delphi, un paquete `.bpl` de 64 bits) en un proceso de 32 bits resultará en una falla inmediata, manifestándose típicamente como mensajes de error como "Can't load package %s" o códigos de error oscuros del sistema operativo.

### Requisitos Críticos de Tiempo de Diseño

Para que el IDE funcione correctamente durante las actividades de diseño (permitiendo la manipulación visual de componentes, configuración de propiedades y utilización de características de tiempo de diseño), *debe* cargar la versión de **32 bits (x86)** de los paquetes de componentes. Este requisito no es negociable debido a la arquitectura fundamental del IDE y los principios de gestión de memoria del sistema operativo.

Esta limitación arquitectónica frecuentemente lleva a confusión entre los desarrolladores, creando conceptos erróneos de que solo son necesarios los paquetes de 32 bits, o generando preguntas sobre por qué existen paquetes separados de 64 bits si el IDE no puede utilizarlos. La distinción crítica radica en comprender la separación entre las operaciones de **tiempo de diseño** (que ocurren dentro del IDE de 32 bits) y los procesos de **compilación/ejecución** (donde las aplicaciones pueden apuntar a arquitecturas de 32 bits o 64 bits).

## Implementación Paso a Paso: Instalación de Paquetes de Tiempo de Diseño de 32 bits

### Primer Paso Esencial: Instalación de Componentes de 32 bits

Basado en la explicación arquitectónica anterior, el paso inicial obligatorio siempre implica instalar la versión de 32 bits de los paquetes de componentes en el IDE de Delphi. Este proceso establece la base para todas las actividades de desarrollo posteriores.

1. **Adquirir Archivos de Paquete Necesarios:** Asegúrese de poseer archivos de paquete compilados tanto de 32 bits como de 64 bits (`.bpl` y `.dcp`). Los archivos de 32 bits típicamente llevan sufijos identificadores como `_x86`, `_Win32`, o pueden carecer de especificadores de plataforma en versiones anteriores de Delphi. Por el contrario, los paquetes de 64 bits normalmente incluyen designaciones `_x64` o `_Win64`. Estos archivos generalmente se generan automáticamente al construir proyectos de bibliotecas de componentes dirigidos a plataformas Win32 y Win64. Al usar componentes de terceros, los proveedores de confianza deben suministrar ambas versiones arquitectónicas.

2. **Iniciar Entorno de Desarrollo:** Inicie el IDE de Delphi con los permisos de usuario apropiados.

3. **Acceder a la Interfaz de Instalación de Paquetes:** Navegue a través del sistema de menús a `Component > Install Packages...`.

4. **Iniciar Adición de Paquete:** Haga clic en el botón "Add..." para comenzar el proceso de instalación.

5. **Localizar Archivos de Paquete de 32 bits:** Navegue al directorio que contiene sus archivos de paquete compilados de **32 bits** (`.bpl`). Seleccione cuidadosamente el archivo `.bpl` de 32 bits y haga clic en "Open" para proceder.

6. **Completar Proceso de Instalación:** El paquete debería aparecer en la lista "Design packages", típicamente habilitado por defecto. Confirme la instalación haciendo clic en "OK".

### Verificación y Solución de Problemas

El IDE intentará cargar el paquete de 32 bits. Cuando tenga éxito, sus componentes deberían aparecer en la Paleta de Herramientas, permitiendo su uso inmediato en el Diseñador de Formularios. Si el IDE no logra cargar el paquete, verifique que seleccionó el archivo `.bpl` de 32 bits correcto y asegúrese de que todos los paquetes de dependencia requeridos por su paquete objetivo estén correctamente instalados y accesibles.

**Advertencia Crítica:** Nunca intente instalar archivos `.bpl` de 64 bits utilizando la opción de menú `Component > Install Packages...`. Tales intentos fallarán invariablemente porque la arquitectura del IDE de 32 bits no puede cargar módulos de código de 64 bits.

## Configuración Avanzada: Estableciendo Rutas de Biblioteca del Proyecto para Desarrollo de Doble Plataforma

### Configurando Rutas de Búsqueda del Compilador

Mientras que el IDE utiliza paquetes de 32 bits durante las operaciones de tiempo de diseño, el compilador de Delphi requiere información precisa sobre dónde localizar los archivos apropiados (`.dcu`, `.dcp`, `.obj`) para su plataforma objetivo específica durante la compilación (ya sea 32 bits o 64 bits). Estas configuraciones se establecen a través de las opciones del proyecto, específicamente dentro de la sección de configuración de ruta de biblioteca. Es importante destacar que estas configuraciones deben establecerse por separado para cada plataforma objetivo.

1. **Acceder a Configuración del Proyecto:** Navegue a `Project > Options...` en el menú del IDE.

2. **Seleccionar Plataforma Apropiada:** Es absolutamente crucial configurar las rutas por separado para cada plataforma objetivo. Utilice el menú desplegable "Target Platform" ubicado en la parte superior del diálogo de Opciones del Proyecto. Comience la configuración con la selección "32-bit Windows".

3. **Navegar a Sección de Configuración de Biblioteca:** En el árbol de opciones mostrado en el lado izquierdo, seleccione `Delphi Compiler > Library` para acceder a la configuración de rutas.

4. **Configurar Rutas de Biblioteca de 32 bits:** Dentro del campo "Library path", haga clic en el botón de puntos suspensivos (...) para abrir el editor de rutas. Agregue el directorio que contiene sus unidades compiladas de **32 bits** (archivos `.dcu`) y el archivo `.dcp` del paquete de **32 bits** para los componentes que ha instalado. Asegúrese de que esta ruta haga referencia específicamente al directorio de salida de 32 bits de su biblioteca de componentes.

5. **Cambiar a Configuración de 64 bits:** Cambie la selección del menú desplegable "Target Platform" a "64-bit Windows". Note que el campo "Library path" podría mostrar contenido diferente o aparecer vacío.

6. **Configurar Rutas de Biblioteca de 64 bits:** Repita el proceso de configuración de ruta anterior, pero esta vez agregue directorios que contengan sus unidades compiladas de **64 bits** (archivos `.dcu`) y el archivo `.dcp` del paquete de **64 bits**. Esta ruta *debe* diferir de la ruta de 32 bits y hacer referencia correctamente al directorio de salida de 64 bits.

7. **Revisar Configuraciones de Ruta Adicionales:** Mientras que la configuración de la ruta de biblioteca es esencial para localizar archivos `.dcu` y `.dcp`, también examine las configuraciones de `Browsing path` (usadas por las características de code insight) y verifique que la ubicación del `DCP output directory` esté configurada correctamente si está construyendo paquetes usted mismo. Configure estas rutas para plataformas de 32 bits y 64 bits también.

8. **Guardar Cambios de Configuración:** Haga clic en "OK" para preservar las configuraciones de opciones del proyecto.

### Evitando Errores Comunes de Configuración

**Error Frecuente:** Muchos desarrolladores olvidan cambiar el menú desplegable "Target Platform" *antes* de establecer la ruta para esa plataforma. Configurar la ruta de 64 bits mientras "32-bit Windows" permanece seleccionado (o viceversa) representa una fuente común de errores de compilación más adelante en el proceso de desarrollo.

Al establecer correctamente estas rutas de biblioteca específicas de la plataforma, proporciona al compilador información precisa sobre dónde localizar los archivos `.dcu` y `.dcp` necesarios para la arquitectura actualmente en construcción.

## Estrategias de Gestión de Paquetes en Tiempo de Ejecución

### Decidiendo Enfoques de Enlace

Más allá de instruir al compilador dónde encontrar unidades durante la compilación, debe determinar cómo se enlazará su ejecutable final con las bibliotecas de componentes. Esta decisión crítica se controla a través de la sección de configuración "Runtime Packages".

Tiene dos opciones principales:

1. **Enfoque de Enlace Estático:** Si deja la opción "Link with runtime packages" desmarcada (o elimina todos los paquetes de la lista), el compilador incorporará directamente el código y los recursos necesarios de sus componentes en el archivo `.exe` final. Este enfoque produce archivos ejecutables más grandes pero elimina el requisito de distribuir archivos `.bpl` separados junto con su aplicación.

2. **Enfoque de Enlace Dinámico (Paquetes en Tiempo de Ejecución):** Si habilita "Link with runtime packages" y especifica los paquetes requeridos, el compilador *no* incrustará el código del componente en su `.exe`. En su lugar, su aplicación cargará dinámicamente los archivos `.bpl` necesarios durante la ejecución. Esta estrategia crea archivos ejecutables más pequeños pero requiere desplegar los archivos `.bpl` correspondientes de 32 bits o 64 bits con la distribución de su aplicación.

### Proceso de Configuración Detallado

1. **Acceder a Opciones del Proyecto:** Navegue a `Project > Options...` en el menú del IDE.

2. **Seleccionar Plataforma Objetivo:** Elija "32-bit Windows" o "64-bit Windows" del menú desplegable de plataforma.

3. **Navegar a Configuración de Paquetes:** Seleccione `Packages > Runtime Packages` en el árbol de navegación de opciones.

4. **Configurar Método de Enlace:** Habilite o deshabilite la opción "Link with runtime packages" según su enfoque de enlace preferido determinado anteriormente.

5. **Especificar Paquetes Requeridos:** Al utilizar paquetes en tiempo de ejecución, asegúrese de que la lista contenga los nombres base correctos de los paquetes que su aplicación requiere (por ejemplo, `MyComponentPackage`). *No* incluya sufijos de plataforma o extensiones de archivo en estas entradas. Delphi agrega automáticamente los identificadores de plataforma apropiados y carga los archivos `_x86.bpl` o `_x64.bpl` correctos (o nomenclatura equivalente basada en la versión/configuración de Delphi) durante el tiempo de ejecución.

6. **Configurar Plataforma Secundaria:** Cambie la selección "Target Platform" y configure los ajustes de paquetes en tiempo de ejecución de manera idéntica para la plataforma alternativa. Típicamente, la decisión de usar o no usar paquetes en tiempo de ejecución permanece consistente en ambas plataformas, pero las listas de paquetes podrían diferir si se utilizan bibliotecas específicas de la plataforma.

7. **Preservar Configuración:** Haga clic en "OK" para guardar los ajustes.

### Consideraciones de Despliegue

**Requisito Crítico de Despliegue:** Si elige el enlace dinámico con paquetes en tiempo de ejecución, recuerde que *debe* distribuir la versión arquitectónica correcta (32 bits o 64 bits) de esos archivos `.bpl` con su aplicación. El ejecutable de 32 bits requiere archivos `.bpl` de 32 bits, mientras que el ejecutable de 64 bits necesita archivos `.bpl` de 64 bits. Coloque estos archivos en el mismo directorio que el `.exe` o en ubicaciones accesibles a través de la variable de entorno PATH del sistema.

## Metodologías Integrales de Prueba y Verificación

### Verificación Multiplataforma

La configuración por sí sola no puede garantizar el éxito. Las pruebas exhaustivas se vuelven esenciales para confirmar que todo funcione como se espera en ambas plataformas objetivo.

1. **Compilación Multiplataforma:** Construya su proyecto explícitamente para las plataformas objetivo "32-bit Windows" y "64-bit Windows". Aborde cualquier error del compilador que surja durante este proceso. Los errores que ocurren durante la compilación frecuentemente indican rutas de biblioteca configuradas incorrectamente (detallado en el Paso 2).

2. **Pruebas de Ejecución de 32 bits:** Ejecute la aplicación compilada de 32 bits. Pruebe a fondo toda la funcionalidad que depende de los componentes en cuestión. Busque específicamente:
   * Apariencia visual adecuada y comportamiento interactivo de los componentes.
   * Ausencia de excepciones durante la instanciación de componentes o invocación de métodos.
   * Si usa paquetes en tiempo de ejecución, verifique que la aplicación se inicie sin mensajes de error "Package XYZ not found".

3. **Pruebas de Ejecución de 64 bits:** Ejecute la aplicación compilada de 64 bits. Realice pruebas idénticas a las realizadas con la versión de 32 bits. Preste especial atención a:
   * Cualquier diferencia de comportamiento en comparación con la versión de 32 bits.
   * Errores de tiempo de ejecución como Violaciones de Acceso, que podrían indicar problemas subyacentes de compatibilidad de 64 bits en el código del componente o la lógica de la aplicación (por ejemplo, aritmética de punteros incorrecta, suposiciones de tamaño de enteros).
   * Para paquetes en tiempo de ejecución, verifique nuevamente si hay errores de paquetes faltantes, asegurando que los archivos `.bpl` de 64 bits sean correctamente accesibles.

4. **Evaluación de Casos Límite:** Incluya escenarios de prueba que exploren condiciones límite, particularmente con respecto al uso de memoria si eso representa una motivación para la transición a 64 bits. Cargue conjuntos de datos extensos y realice operaciones complejas que involucren a los componentes para probar la implementación bajo estrés.

### Interpretando Resultados de Pruebas

Cualquier discrepancia o error encontrado durante el tiempo de ejecución en una plataforma pero no en la otra sugiere fuertemente un problema en la configuración del paquete (Pasos 2 o 3) o posibles problemas de compatibilidad de 64 bits dentro del componente o el código de la aplicación en sí. Tales problemas requieren un diagnóstico cuidadoso y una resolución dirigida.

## Guía Avanzada de Solución de Problemas

### Resolviendo Problemas Comunes de Instalación

* **"Package XYZ.bpl can't be installed because it is not a design time package."**: Este error típicamente indica un intento de instalar un paquete a través de `Component > Install Packages` que carece de los registros de tiempo de diseño o banderas de configuración necesarios. Verifique que el proyecto del paquete esté configurado correctamente como un paquete de tiempo de diseño o un paquete combinado de tiempo de diseño y ejecución.

* **"Can't load package XYZ.bpl. %1 is not a valid Windows application." / "The specified module could not be found."**: Esto casi con certeza indica un intento de instalar un BPL de **64 bits** en el IDE de 32 bits a través de `Component > Install Packages`. Recuerde instalar solo archivos BPL de 32 bits a través de esta interfaz. La variante "module not found" también puede ocurrir si el paquete tiene dependencias que no están instaladas correctamente o no se pueden localizar.

* **[Compiler Error] F1026 File not found: 'ComponentUnit.dcu'**: Este error ocurre durante la compilación (no en tiempo de diseño). Indica que el compilador no puede localizar el archivo `.dcu` requerido para la plataforma objetivo seleccionada actualmente. Revise cuidadosamente sus configuraciones de `Project Options > Delphi Compiler > Library > Library path` para la *plataforma específica* que está compilando actualmente (Paso 2). Asegúrese de que la ruta haga referencia correctamente al directorio apropiado (32 bits o 64 bits) que contiene los archivos `.dcu` necesarios.

* **[Linker Error] E2202 Required package 'XYZ' not found**: Similar a F1026, pero ocurriendo durante la fase de enlace. Esto frecuentemente indica que el archivo `.dcp` para el paquete no se puede encontrar. Verifique que la Ruta de Biblioteca (Paso 2) incluya el directorio que contiene el archivo `.dcp` de la plataforma correcta. Además, asegúrese de que el nombre del paquete aparezca correctamente en `Project Options > Packages > Runtime Packages` si utiliza enlace dinámico (Paso 3).

* **Runtime Error: "Package XYZ not found"**: Esto indica que su aplicación fue compilada para usar paquetes en tiempo de ejecución, pero el archivo `.bpl` requerido (que coincide con la arquitectura de la aplicación) no se puede localizar durante el inicio de la aplicación. Asegúrese de que los archivos `.bpl` correctos de 32 bits o 64 bits estén desplegados junto con su archivo `.exe` (como se describe en el Paso 3).

* **Runtime Access Violations (AVs) only in 64-bit:** Esto típicamente indica problemas de compatibilidad de 64 bits en el código (ya sea en su aplicación o en la implementación del componente). Las fuentes comunes incluyen:
  * Aritmética de punteros asumiendo `SizeOf(Pointer)=4` (válido solo en código de 32 bits).
  * Uso incorrecto de `Integer` en lugar de `NativeInt`/`NativeUInt` para identificadores o valores del tamaño de un puntero.
  * Llamadas directas a funciones de la API de Windows utilizando tipos de datos incorrectos para entornos de 64 bits.
  * Problemas de alineación de estructuras de datos.
  
  La depuración de la aplicación de 64 bits se vuelve necesaria para identificar la causa específica de estas violaciones.

## Trabajando con Paquetes de Componentes de Terceros

### Mejores Prácticas para Componentes Externos

Los principios descritos a lo largo de esta guía se aplican igualmente a los componentes de terceros. Los proveedores de componentes de confianza típicamente proporcionan:

1. Instrucciones detalladas para procedimientos de instalación adecuados.
2. Archivos `.bpl`, `.dcp` y `.dcu` compilados por separado para 32 bits y 64 bits.
3. Una utilidad de instalación que maneja la colocación de archivos en ubicaciones apropiadas y potencialmente automatiza la instalación de paquetes de tiempo de diseño de 32 bits en el IDE.

Si se proporciona un instalador, utilícelo como su primer enfoque. Sin embargo, siempre valide las opciones del proyecto (Rutas de Biblioteca, Paquetes en Tiempo de Ejecución) después, ya que los instaladores pueden no configurar perfectamente las rutas para cada configuración de proyecto posible o versión de Delphi. Si recibe solo archivos de biblioteca sin procesar sin un instalador, siga los Pasos 1-3 manualmente, identificando y configurando cuidadosamente las rutas para las versiones de 32 bits y 64 bits suministradas por el proveedor. Al encontrar problemas, consulte la documentación del proveedor o contacte a su equipo de soporte técnico para obtener asistencia.

## Resumen y Recomendaciones

### Estrategias Clave de Implementación

Gestionar con éxito los paquetes de Delphi para el desarrollo tanto de 32 bits como de 64 bits depende fundamentalmente de comprender la naturaleza de 32 bits del IDE y configurar meticulosamente las opciones del proyecto para cada plataforma objetivo de forma independiente. Siempre instale el paquete de 32 bits para uso en tiempo de diseño, luego establezca cuidadosamente las Rutas de Biblioteca específicas de la plataforma y la configuración de Paquetes en Tiempo de Ejecución para asegurar que el compilador y su aplicación final puedan localizar y utilizar los archivos correctos para la arquitectura objetivo.

Si bien este enfoque introduce una complejidad adicional en comparación con el desarrollo puramente de 32 bits, la metodología estructurada le permite aprovechar los beneficios sustanciales de la compilación de 64 bits mientras mantiene una experiencia de tiempo de diseño completamente funcional dentro del entorno familiar del IDE de Delphi. Las pruebas consistentes en ambas plataformas representan el paso de verificación final y crucial para garantizar aplicaciones robustas y confiables que funcionen de manera óptima en entornos tanto de 32 bits como de 64 bits.

---
¿Necesita información adicional? Por favor [contacte a soporte](https://support.visioforge.com/) para asistencia con escenarios específicos o problemas de componentes.
