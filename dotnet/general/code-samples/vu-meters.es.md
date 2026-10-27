---
title: Implementar medidores VU y visualizadores de forma de onda
description: Construya medidores VU y visualizadores de forma de onda en WinForms y WPF para monitoreo de nivel de audio en tiempo real con soporte mono y estéreo en .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - WinForms
  - WPF
  - C#
primary_api_classes:
  - AudioLevelEventArgs
  - VUMeterMaxSampleEventArgs
---

# Visualización de audio: implementación de medidores VU y visualizaciones de forma de onda en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

La visualización de audio es un componente crucial de las aplicaciones de medios modernas, proporcionando a los usuarios retroalimentación visual sobre los niveles de audio y patrones de forma de onda. Esta guía demuestra cómo implementar medidores VU (Unidad de Volumen) y visualizadores de forma de onda tanto en aplicaciones WinForms como WPF.

## Entender los componentes de visualización de audio

Antes de profundizar en la implementación, es importante entender las dos herramientas principales de visualización con las que trabajaremos:

### Medidores VU

Los medidores VU muestran el nivel de audio instantáneo de una señal, típicamente mostrando qué tan fuerte es el audio en cualquier momento dado. Proporcionan retroalimentación en tiempo real sobre los niveles de audio, ayudando a los usuarios a monitorear la intensidad de la señal y prevenir distorsión o recorte.

### Pintores de Forma de Onda

Los visualizadores de forma de onda muestran la señal de audio como una línea continua que representa cambios de amplitud a lo largo del tiempo. Proporcionan una representación más detallada del contenido de audio, mostrando patrones y características que podrían no ser evidentes solo escuchando.

## Implementación en aplicaciones WinForms

WinForms proporciona una manera directa de implementar componentes de visualización de audio con código mínimo. Exploremos la implementación tanto de medidores VU como de pintores de forma de onda.

### Implementación de medidor VU en WinForms

Implementar un medidor VU en WinForms requiere solo unos pocos pasos:

1. **Agregar el Control de Medidor VU**: Primero, agregue el control de medidor VU a su formulario. Para audio estéreo, típicamente agregará dos controles—uno para cada canal.

   ```cs
   // Agregar esto al diseño de su formulario
   VisioForge.Core.UI.WinForms.VolumeMeterPro.VolumeMeter volumeMeter1;
   VisioForge.Core.UI.WinForms.VolumeMeterPro.VolumeMeter volumeMeter2; // Para estéreo
   ```

2. **Habilitar el medidor VU en su control de medios**: Antes de iniciar la reproducción o captura, habilite la funcionalidad del medidor VU en su control de medios.

   ```cs
   // Habilitar medidor VU antes de iniciar reproducción/captura
   mediaPlayer.Audio_VUMeter_Pro_Enabled = true;
   ```

3. **Implementar el Manejador de Eventos**: Agregue un manejador de eventos para procesar los datos de nivel de audio y actualizar la visualización del medidor VU.

   ```cs
   private void VideoCapture1_OnAudioVUMeterProVolume(object sender, AudioLevelEventArgs e)
   {
       volumeMeter1.Amplitude = e.ChannelLevelsDb[0];
       if (e.ChannelLevelsDb.Length > 1)
       {
           volumeMeter2.Amplitude = e.ChannelLevelsDb[1];
       }
   }
   ```

Con estos pasos, su medidor VU se actualizará dinámicamente basándose en los niveles de audio de su reproducción o captura de medios.

### Implementación de pintor de forma de onda en WinForms

La implementación del pintor de forma de onda sigue un patrón similar:

1. **Agregar el Control de Pintor de Forma de Onda**: Agregue el control de pintor de forma de onda a su formulario. Para audio estéreo, agregue dos controles.

   ```cs
   // Agregar esto al diseño de su formulario
   VisioForge.Core.UI.WinForms.VolumeMeterPro.WaveformPainter waveformPainter1;
   VisioForge.Core.UI.WinForms.VolumeMeterPro.WaveformPainter waveformPainter2; // Para estéreo
   ```

2. **Habilitar el Procesamiento del Medidor VU**: Habilite la funcionalidad del medidor VU para proporcionar datos al pintor de forma de onda.

   ```cs
   // Habilitar medidor VU antes de iniciar reproducción/captura
   mediaPlayer.Audio_VUMeter_Pro_Enabled = true;
   ```

3. **Implementar el Manejador de Eventos**: Agregue un manejador de eventos para procesar los datos de audio y actualizar la visualización de forma de onda.

   ```cs
   private void VideoCapture1_OnAudioVUMeterProVolume(object sender, AudioLevelEventArgs e)
   {
       waveformPainter1.AddMax(e.ChannelLevelsDb[0]);
       if (e.ChannelLevelsDb.Length > 1)
       {
           waveformPainter2.AddMax(e.ChannelLevelsDb[1]);
       }
   }
   ```

## Implementación en aplicaciones WPF

WPF requiere un enfoque ligeramente diferente debido a su modelo de hilos y framework de UI. Veamos cómo implementar ambos tipos de visualización en WPF.

### Implementación de medidor VU en WPF

1. **Agregar el Control de Medidor VU**: Agregue el control de medidor VU a su diseño XAML. Para audio estéreo, agregue dos controles.

   ```xml
   <VisioForge.Core.UI.WPF.VolumeMeterPro.VolumeMeter x:Name="volumeMeter1" />
   <VisioForge.Core.UI.WPF.VolumeMeterPro.VolumeMeter x:Name="volumeMeter2" /> <!-- Para estéreo -->
   ```

2. **Habilitar el Procesamiento del Medidor VU e Iniciar los Medidores**:

   ```cs
   VideoCapture1.Audio_VUMeter_Pro_Enabled = true;

   volumeMeter1.Start();
   volumeMeter2.Start();
   ```

3. **Implementar el Manejador de Eventos con Dispatcher**: En WPF, necesita usar el Dispatcher para actualizar elementos de UI desde hilos que no son de UI.

   ```cs
   private delegate void AudioVUMeterProVolumeDelegate(AudioLevelEventArgs e);

   private void AudioVUMeterProVolumeDelegateMethod(AudioLevelEventArgs e)
   {
       volumeMeter1.Amplitude = e.ChannelLevelsDb[0];
       volumeMeter1.Update();

       if (e.ChannelLevelsDb.Length > 1)
       {
           volumeMeter2.Amplitude = e.ChannelLevelsDb[1];
           volumeMeter2.Update();
       }
   }

   private void VideoCapture1_OnAudioVUMeterProVolume(object sender, AudioLevelEventArgs e)
   {
       Dispatcher.BeginInvoke(new AudioVUMeterProVolumeDelegate(AudioVUMeterProVolumeDelegateMethod), e);
   }
   ```

4. **Limpiar Después de la Reproducción**: Cuando la reproducción se detiene, limpie los medidores VU para liberar recursos.

   ```cs
   volumeMeter1.Stop();
   volumeMeter1.Clear();

   volumeMeter2.Stop();
   volumeMeter2.Clear();
   ```

### Implementación de pintor de forma de onda en WPF

1. **Agregar el Control de Pintor de Forma de Onda**: Agregue el control de pintor de forma de onda a su diseño XAML.

   ```xml
   <VisioForge.Core.UI.WPF.VolumeMeterPro.WaveformPainter x:Name="waveformPainter" />
   ```

2. **Habilitar el Procesamiento del Medidor VU e Iniciar el Pintor de Forma de Onda**:

   ```cs
   VideoCapture1.Audio_VUMeter_Pro_Enabled = true;
   waveformPainter.Start();
   ```

3. **Implementar el Manejador de Eventos de Máximo Calculado**: Para pintores de forma de onda en WPF, usamos un evento diferente.

   ```cs
   private delegate void AudioVUMeterProMaximumCalculatedDelegate(VUMeterMaxSampleEventArgs e);

   private void AudioVUMeterProMaximumCalculatedelegateMethod(VUMeterMaxSampleEventArgs e)
   {
       waveformPainter.AddValue(e.MaxSample, e.MinSample);
   }

   private void VideoCapture1_OnAudioVUMeterProMaximumCalculated(object sender, VUMeterMaxSampleEventArgs e)
   {
       Dispatcher.BeginInvoke(new AudioVUMeterProMaximumCalculatedDelegate(AudioVUMeterProMaximumCalculatedelegateMethod), e);
   }
   ```

4. **Limpiar Después de la Reproducción**: Cuando la reproducción se detiene, limpie el pintor de forma de onda.

   ```cs
   waveformPainter.Stop();
   waveformPainter.Clear();
   ```

## Opciones de personalización avanzada

Tanto el medidor VU como los controles de pintor de forma de onda ofrecen extensas opciones de personalización para coincidir con el diseño y los requisitos de experiencia de usuario de su aplicación.

### Personalización de medidores VU

El control `VolumeMeter` expone las siguientes propiedades reales:

- **`MinDb` / `MaxDb`**: rango de decibelios mostrado por el medidor
- **`Boost`**: multiplicador de ganancia aplicado antes del renderizado
- **`Orientation`**: dirección horizontal o vertical de la barra
- **`ForeColor`**: color de la barra (heredado de `Control`)
- **`MinimalUpdateInterval`** (solo WPF): limita la frecuencia de redibujado

Ejemplo de personalización de un medidor VU:

```cs
volumeMeter1.MinDb = -60;
volumeMeter1.MaxDb = 6;
volumeMeter1.Boost = 1.0f;
volumeMeter1.ForeColor = System.Drawing.Color.Green;  // WinForms
volumeMeter1.Orientation = System.Windows.Forms.Orientation.Vertical;
```

### Personalización de pintores de forma de onda

El control `WaveformPainter` tiene una superficie real pequeña:

- **`Boost`** (WinForms): multiplicador de ganancia previo al renderizado
- **`ForeColor` / `BackColor`**: colores de línea y fondo (heredados de `Control`)
- **`Clear()`**: reinicia el historial pintado
- **`AddMax(float)`** (WinForms) / **`AddValue(float, float)`** (WPF): añade una nueva muestra

Ejemplo de personalización de un pintor de forma de onda:

```cs
waveformPainter.ForeColor = System.Drawing.Color.SkyBlue;
waveformPainter.BackColor = System.Drawing.Color.Black;
waveformPainter.Boost = 1.5f;
```

## Consideraciones de rendimiento

Al implementar visualización de audio, considere estos consejos de rendimiento:

1. **Frecuencia de Actualización**: Equilibre la capacidad de respuesta visual con el uso de CPU ajustando la frecuencia con que actualice los visuales
2. **Gestión de Hilos de UI**: Siempre actualice los elementos de UI en el hilo apropiado (especialmente importante en WPF)
3. **Limpieza de Recursos**: Detenga y limpie correctamente los controles de visualización cuando no estén en uso
4. **Almacenamiento en Búfer**: Considere implementar almacenamiento en búfer para una visualización más suave durante el alto uso de CPU

## Conclusión

Implementar medidores VU y pintores de forma de onda agrega valiosa retroalimentación visual a las aplicaciones de medios. Ya sea que desarrolle en WinForms o WPF, estos componentes de visualización de audio ayudan a los usuarios a monitorear y entender los niveles y patrones de audio de manera más intuitiva.

Siguiendo los pasos de implementación descritos en esta guía, puede mejorar sus aplicaciones de medios .NET con características de visualización de audio de calidad profesional que mejoran la experiencia general del usuario.

---
Para más ejemplos de código y SDKs relacionados, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).