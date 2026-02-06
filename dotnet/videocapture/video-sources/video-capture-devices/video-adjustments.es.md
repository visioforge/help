---
title: Dominar Controles de Imagen de Cámara en .NET
description: Controla configuraciones de cámara incluyendo brillo, contraste, tono y saturación en .NET con ejemplos de código del Video Capture SDK.
---

# Dominar Controles de Imagen de Cámara en Aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introducción a los Ajustes de Hardware de Cámara

Al desarrollar aplicaciones que utilizan webcams u otros dispositivos de captura de video, tener control preciso sobre los parámetros de calidad de imagen es esencial para crear software de grado profesional. La clase `VideoHardwareAdjustment` proporciona a los desarrolladores herramientas poderosas para ajustar programáticamente configuraciones de cámara como brillo, contraste, tono, saturación, nitidez, gamma y más.

## Propiedades de Ajuste de Hardware Soportadas

El SDK soporta numerosas propiedades de ajuste incluyendo:

- Brillo (Brightness)
- Contraste (Contrast)
- Tono (Hue)
- Saturación (Saturation)
- Nitidez (Sharpness)
- Gamma
- Balance de Blancos (White Balance)
- Compensación de Contraluz (Backlight Compensation)
- Ganancia (Gain)
- Exposición (Exposure)

Nota que no todas las cámaras soportan cada propiedad. El SDK ignorará elegantemente cualquier propiedad no soportada por el hardware de cámara específico que estés usando.

## Trabajar con Ajustes de Cámara

### Obtener Rangos de Valores de Propiedades

Antes de establecer valores de ajuste, es importante entender el rango válido para cada propiedad. Usa el método `Video_CaptureDevice_VideoAdjust_GetRangesAsync` para recuperar los valores mínimo, máximo, predeterminado y tamaño de paso para cualquier propiedad de ajuste:

```cs
// Recuperar el rango válido de valores para la propiedad de brillo
// Esto ayuda a entender los límites dentro de los cuales puedes ajustar configuraciones
var brightnessRange = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRangesAsync(VideoHardwareAdjustment.Brightness);

// Verificar los valores mínimo y máximo permitidos
int minValue = brightnessRange.Min;
int maxValue = brightnessRange.Max;
int defaultValue = brightnessRange.Default;
int step = brightnessRange.SteppingDelta;
```

### Establecer Valores de Ajuste

Una vez que conoces el rango válido, puedes establecer un valor específico usando el método `Video_CaptureDevice_VideoAdjust_SetValueAsync`:

```cs
// Crear un objeto de valor de ajuste con tus configuraciones deseadas
// Puedes especificar un valor personalizado y si debe usarse ajuste automático
var adjustmentValue = new VideoCaptureDeviceAdjustValue(75, false); // Valor: 75, Auto: false

// Aplicar el ajuste de brillo a la cámara
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Brightness, adjustmentValue);
```

### Recuperar Valores Actuales

Para leer el valor actual de cualquier propiedad de ajuste, usa el método `Video_CaptureDevice_VideoAdjust_GetValueAsync`:

```cs
// Obtener la configuración actual de brillo de la cámara
// Esto devuelve tanto el valor actual como si el auto-ajuste está habilitado
var currentBrightness = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetValueAsync(VideoHardwareAdjustment.Brightness);

// Acceder al valor actual y bandera de auto
int value = currentBrightness.Value;
bool isAuto = currentBrightness.Auto;
```

## Ejemplos Prácticos

### Ajustar Brillo y Contraste

```cs
// Establecer brillo
var brightness = new VideoCaptureDeviceAdjustValue(80, false);
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Brightness, brightness);

// Establecer contraste
var contrast = new VideoCaptureDeviceAdjustValue(60, false);
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Contrast, contrast);
```

### Usar Ajuste Automático

```cs
// Habilitar exposición automática
var autoExposure = new VideoCaptureDeviceAdjustValue(0, true); // Valor ignorado cuando auto = true
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Exposure, autoExposure);

// Habilitar balance de blancos automático
var autoWhiteBalance = new VideoCaptureDeviceAdjustValue(0, true);
await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.WhiteBalance, autoWhiteBalance);
```

### Crear Control Deslizante de UI

```cs
// Inicializar control deslizante con rango de cámara
var brightnessRange = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRangesAsync(VideoHardwareAdjustment.Brightness);

sliderBrightness.Minimum = brightnessRange.Min;
sliderBrightness.Maximum = brightnessRange.Max;
sliderBrightness.Value = brightnessRange.Default;

// Manejar cambios del control deslizante
private async void sliderBrightness_ValueChanged(object sender, EventArgs e)
{
    var value = new VideoCaptureDeviceAdjustValue((int)sliderBrightness.Value, false);
    await VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValueAsync(VideoHardwareAdjustment.Brightness, value);
}
```

## Mejores Prácticas para Ajustes de Cámara

1. **Siempre verificar rangos primero**: Diferentes modelos de cámara tienen diferentes rangos válidos para cada propiedad
2. **Respetar el tamaño de paso**: Algunos ajustes solo pueden cambiarse en incrementos específicos
3. **Considerar ajuste automático**: Muchas cámaras proporcionan excelentes resultados con configuraciones automáticas
4. **Guardar preferencias del usuario**: Permite a los usuarios guardar sus configuraciones preferidas
5. **Proporcionar valores predeterminados**: Ofrece forma de restablecer a configuraciones de fábrica

## Manejo de Errores

```cs
try
{
    var range = await VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRangesAsync(VideoHardwareAdjustment.Brightness);
    // Usar el rango
}
catch (NotSupportedException)
{
    // La cámara no soporta esta propiedad
    MessageBox.Show("El ajuste de brillo no está soportado por esta cámara.");
}
```

## Aplicaciones de Ejemplo

Explora estas aplicaciones de ejemplo para ver ajustes de cámara en acción:

- [Demo Principal de Video Capture (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
- [Demo de Webcam (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a muestras de código adicionales y recursos de implementación.