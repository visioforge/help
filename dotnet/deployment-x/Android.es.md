---
title: Desplegar VisioForge .NET SDK en Android: NuGet y ajustes
description: Despliegue sus aplicaciones .NET con VisioForge SDK en Android: paquetes NuGet, integración VideoView, soporte ARM64/x86 y permisos.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Android
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - VideoPlayerActivity

---

# Guía de implementación y despliegue para Android

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a los SDKs de VisioForge para Android

Los desarrolladores Android que trabajan con tecnologías .NET pueden aprovechar las potentes capacidades de los SDKs de VisioForge para integrar funcionalidad multimedia avanzada en sus aplicaciones. Los SDKs proporcionan soluciones robustas para manipulación, reproducción, captura y edición de medios en la plataforma Android usando tecnologías .NET.

El SDK de VisioForge para Android ofrece potentes capacidades para procesamiento, captura, edición y reproducción de video, todo optimizado para la plataforma Android mientras mantiene una experiencia de desarrollo multiplataforma consistente.

El proceso de despliegue en Android requiere consideración especial para gestión de paquetes, compatibilidad de dispositivos, permisos y optimización de rendimiento. Este documento proporciona instrucciones detalladas para asegurar que su aplicación funcione sin problemas en dispositivos Android.

## Requisitos del sistema

Antes de comenzar su proceso de implementación y despliegue en Android, asegúrese de que su entorno de desarrollo cumpla los siguientes requisitos:

### Requisitos del dispositivo

- Dispositivo Android con Android 9.0 (nivel de API 28) o posterior
- Arquitectura de procesador ARM o ARM64
- Espacio de almacenamiento suficiente para activos de aplicación y procesamiento de medios
- Hardware de cámara y micrófono (si se usan características de captura de video/audio)

### Requisitos del entorno de desarrollo

- Computadora con Windows, Linux o macOS
- Visual Studio con cargas de trabajo .NET MAUI o Xamarin instaladas, JetBrains Rider o Visual Studio Code
- .NET 8.0 SDK o posterior (se recomienda la última versión estable)
- Android SDK con niveles de API apropiados instalados
- Java Development Kit (JDK) 11 o posterior
- Conocimiento básico de desarrollo .NET para Android

## Soporte de arquitectura

El SDK de VisioForge para Android proporciona soporte nativo para arquitecturas comunes de dispositivos Android:

### Soporte ARM64

- Optimizado para dispositivos Android modernos
- Procesamiento de video acelerado por hardware
- Rendimiento mejorado para operaciones de medios
- Objetivo principal para la mayoría de las aplicaciones

### Soporte ARM/ARMv7

- Compatibilidad con dispositivos Android más antiguos
- Fallbacks de software para aceleración de hardware cuando sea necesario
- Enfoque equilibrado de rendimiento y compatibilidad

## Proceso de instalación y configuración

Siga estos pasos para configurar e implementar correctamente su aplicación Android con VisioForge:

1. Cree un nuevo proyecto Android en su IDE preferido (se recomienda Visual Studio o JetBrains Rider).
2. Agregue los paquetes NuGet requeridos a su proyecto (detallado en la siguiente sección).
3. Configure los permisos necesarios en su archivo AndroidManifest.xml.
4. Implemente la lógica de su aplicación usando los componentes del SDK de VisioForge.
5. Compile, firme y despliegue su aplicación en dispositivos de prueba.

### Gestión de paquetes NuGet

El SDK de VisioForge para Android se distribuye a través de paquetes NuGet. Agregue los siguientes paquetes a su proyecto Android:

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core) - Paquete administrado principal del SDK (clases core, VideoCaptureCoreX / MediaPlayerCoreX / MediaBlocksPipeline).
- [VisioForge.CrossPlatform.Core.Android](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Android) - Contiene los componentes de redistribución (bibliotecas nativas) requeridos para aplicaciones Android.

Puede agregar estos paquetes usando el NuGet Package Manager en su IDE o agregando lo siguiente a su archivo de proyecto (use las últimas versiones):

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.*" />
</ItemGroup>
```

Nota: Reemplace los números de versión con las últimas versiones disponibles en NuGet.org.

El paquete `VisioForge.CrossPlatform.Core.Android` incluye las bibliotecas nativas para las cuatro ABI de Android (`armeabi-v7a`, `arm64-v8a`, `x86`, `x86_64`) y las integra automáticamente en la compilación: no se requiere ningún paso de copia manual. Dos de ellas solo se usan para la captura desde cámara USB:

- `libVisioForge_UVC.so` - el puente USB Video Class, con libuvc (BSD 3-Clause) compilado dentro.
- `libusb1.0.so` - libusb 1.0.30, enlazado dinámicamente y con licencia LGPL v2.1 o posterior.

Los avisos completos, incluidos ambos textos de licencia, se distribuyen con el paquete en `THIRD-PARTY-NOTICES.txt`.

### Permisos para la cámara USB

La captura desde una cámara USB (UVC) conectada por OTG necesita sus propias entradas en el manifiesto, además de los paquetes anteriores:

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
<uses-feature android:name="android.hardware.camera.autofocus" android:required="false" />
<uses-feature android:name="android.hardware.usb.host" android:required="true" />
```

Android se niega a entregar un dispositivo de video USB a una aplicación que no posea `android.permission.CAMERA`, incluso cuando nunca se usa la API de cámara de Android. Declarar ese permiso también convierte la cámara integrada en un *requisito* implícito, lo que ocultaría la aplicación en Google Play a los dispositivos cuya única cámara llega por USB; de ahí el `required="false"` explícito.

La captura desde cámara USB requiere **Android 9 (nivel de API 28) o posterior**. Para el recorrido completo, consulte [Captura desde cámara USB en Android](../general/guides/android-usb-camera.md).

### Fuentes para las superposiciones de texto

Las superposiciones de texto — `TextOverlayBlock`, el renderizado de subtítulos y la marca de agua de la versión de prueba — resuelven las familias de fuentes mediante fontconfig, que el SDK configura durante la inicialización. Enumera las fuentes del sistema Android y, además de ellas, las que incluye su propia aplicación, de modo que una superposición pueda nombrar una tipografía que el dispositivo no tiene instalada.

Una fuente se detecta cuando llega al APK como **asset**. Una aplicación .NET MAUI lo obtiene sin trabajo adicional:

```xml
<ItemGroup>
  <MauiFont Include="Resources\Fonts\*" />
</ItemGroup>
```

Una aplicación .NET para Android convencional declara la fuente por su cuenta, ya sea en la raíz de la carpeta de assets o en una subcarpeta `assets/fonts`:

```xml
<ItemGroup>
  <AndroidAsset Include="Assets\OpenSans-Regular.ttf" />
</ItemGroup>
```

Se reconocen `.ttf`, `.ttc`, `.otf` y `.otc`. El SDK las extrae al almacenamiento privado de la aplicación en el primer arranque tras una instalación o actualización, por lo que un inicio normal no copia nada, y una fuente eliminada en una versión posterior deja de resolverse.

> **Nota:** en su código, indique el nombre de la **familia** tal como está registrado dentro del archivo: `Open Sans`, no `OpenSans-Regular.ttf`. Las fuentes declaradas como *recursos* de Android en `Resources/font/` no son assets y las superposiciones de texto no las ven; distribuya el archivo como asset en su lugar.

## Integración de biblioteca de bindings de Java

Las aplicaciones Android que usan el SDK de VisioForge requieren una biblioteca de bindings de Java personalizada para la funcionalidad adecuada. Este paso esencial asegura la comunicación adecuada entre el framework .NET y el entorno basado en Java de Android.

Siga estos pasos detallados para integrarla:

1. Clone el repositorio de la biblioteca de binding desde [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency).
2. Elija el proyecto de binding que coincida con su versión objetivo de .NET — la carpeta incluye un `VisioForge.Core.Android.X{N}.csproj` por cada versión de .NET soportada (ej. `VisioForge.Core.Android.X9.csproj` para .NET 9, `VisioForge.Core.Android.X10.csproj` para .NET 10). Si su versión de .NET no aparece listada, elija la más cercana soportada.
3. Agregue una referencia al proyecto de binding desde el .csproj de su app:

```xml
<ItemGroup>
  <ProjectReference Include="..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
```

> **Nota:** Asegúrese de ajustar la ruta relativa para que coincida con la estructura de su proyecto

## Implementar VideoView en su aplicación

### Agregar VideoView a su diseño

El control `VideoView` es la interfaz principal para mostrar contenido de video en su aplicación Android. Para integrarlo en su app, siga estos pasos:

1. Abra el archivo de diseño de su Activity o Fragment (típicamente un archivo `.axml` o `.xml`)
2. Agregue el elemento VideoView como se muestra en el ejemplo a continuación:

```xml
<VisioForge.Core.UI.Android.VideoView
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:minWidth="25px"
    android:minHeight="25px"
    android:id="@+id/videoView" />
```

### Inicializar VideoView en código

Después de agregar el VideoView a su diseño, necesitará inicializarlo en el código de su Activity o Fragment:

```csharp
using VisioForge.Core.UI.Android;

namespace TuApp
{
    [Activity(Label = "VideoPlayerActivity")]
    public class VideoPlayerActivity : Activity
    {
        private VideoView _videoView;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.your_layout);
            
            // Inicializar la vista de video
            _videoView = FindViewById<VideoView>(Resource.Id.videoView);
        }
    }
}
```

## Consideraciones de rendimiento

Use dispositivos Android físicos para pruebas siempre que sea posible. Los simuladores pueden no representar con precisión el rendimiento del mundo real, especialmente para operaciones de video aceleradas por hardware.

## Firma y publicación de la aplicación

### Firma de la aplicación

Para distribuir su aplicación Android, necesita firmarla con un certificado digital:

1. Cree un archivo keystore si aún no tiene uno:

```bash
keytool -genkey -v -keystore your-app-key.keystore -alias your-app-alias -keyalg RSA -keysize 2048 -validity 10000
```

2. Configure la firma en su proyecto:

Agregue lo siguiente a su archivo `android/app/build.gradle`:

```text
android {
    ...
    
    signingConfigs {
        release {
            storeFile file("your-app-key.keystore")
            storePassword "your-store-password"
            keyAlias "your-app-alias"
            keyPassword "your-key-password"
        }
    }
    
    buildTypes {
        release {
            signingConfig signingConfigs.release
            ...
        }
    }
}
```

Para proyectos .NET MAUI o Xamarin.Android, configure la firma en su archivo .csproj:

```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
    <AndroidKeyStore>True</AndroidKeyStore>
    <AndroidSigningKeyStore>your-app-key.keystore</AndroidSigningKeyStore>
    <AndroidSigningStorePass>your-store-password</AndroidSigningStorePass>
    <AndroidSigningKeyAlias>your-app-alias</AndroidSigningKeyAlias>
    <AndroidSigningKeyPass>your-key-password</AndroidSigningKeyPass>
</PropertyGroup>
```

### Publicación en Google Play Store

1. Genere un AAB (Android App Bundle) para distribución:

```bash
dotnet build -f net8.0-android -c Release /p:AndroidPackageFormat=aab
```

2. Cree una cuenta de desarrollador en Google Play Console si aún no tiene una.

3. Cree una nueva aplicación en Google Play Console.

4. Suba su archivo AAB a la pista de producción.

5. Complete la información del listado de la tienda.

6. Envíe la aplicación para revisión.

## Solución de problemas

### Problemas comunes

1. **Permisos faltantes**: Asegúrese de que todos los permisos requeridos estén declarados en AndroidManifest.xml y solicitados en tiempo de ejecución.
2. **Compatibilidad de arquitectura**: Verifique que su aplicación soporte la arquitectura del dispositivo objetivo (ARM/ARM64).
3. **Restricciones de memoria**: Monitoree el uso de memoria e implemente gestión adecuada de recursos.
4. **Problemas de rendimiento**: Use aceleración de hardware y optimice las operaciones de medios para dispositivos móviles.
5. **Errores de bindings de Java**: Cuando enfrente problemas con bindings de Java:
   - Confirme que está usando la versión correcta de la biblioteca de binding
   - Verifique desajustes de versión entre .NET y la biblioteca de binding
   - Verifique que todas las dependencias estén correctamente referenciadas

### Obtener ayuda

Si encuentra problemas con su despliegue del SDK de VisioForge en Android, por favor consulte:

- [Portal de Soporte](https://support.visioforge.com)
- [Ejemplos en GitHub](https://github.com/visioforge/.Net-SDK-s-samples)

## Conclusión

Implementar y desplegar aplicaciones del SDK de VisioForge en dispositivos Android requiere atención cuidadosa a consideraciones específicas de la plataforma. Siguiendo las guías en este documento, puede asegurar un proceso de desarrollo y despliegue fluido y entregar aplicaciones de video de alta calidad a sus usuarios de Android.

Recuerde probar exhaustivamente en dispositivos objetivo, especialmente para operaciones intensivas de rendimiento como captura y procesamiento de video. Con la implementación adecuada, el SDK de VisioForge habilita potentes aplicaciones de medios en todo el ecosistema Android.
