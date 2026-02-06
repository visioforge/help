---
title: Habilitar Luz de Cámara en Tablets Windows 10+
description: Controla la luz de cámara en tablets Windows 10+ en aplicaciones .NET con ejemplos de código C#, paquetes requeridos e implementación de API TorchControl.
---

# Habilitar Luz de Cámara en Tablets Windows 10+

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introducción

Las tablets modernas con Windows 10+ vienen equipadas con funcionalidad de luz de cámara que los desarrolladores pueden controlar programáticamente. Esta guía explica cómo implementar controles de luz de cámara en tus aplicaciones .NET usando la API TorchControl.

## Implementación con la API TorchControl

La API TorchControl proporciona una forma completa de gestionar luces de cámara en tablets Windows 10+. Esta API ofrece:

- Descubrimiento de dispositivos para cámaras compatibles con antorcha
- Control granular para habilitar y deshabilitar luces de cámara
- Compatibilidad entre dispositivos

### Pasos Básicos de Implementación

1. Inicializar el componente VideoCaptureCore
2. Obtener dispositivos disponibles con capacidades de antorcha
3. Habilitar o deshabilitar funcionalidad de antorcha para dispositivos específicos

## Ejemplo de Código Funcional

```cs
// Inicializar VideoCaptureCore
VideoCaptureCore videoCapture = await VideoCaptureCore.CreateAsync();

// Obtener dispositivos disponibles con capacidad de antorcha
string[] devices = await videoCapture.TorchControl_GetDevicesAsync();

// Habilitar antorcha para el primer dispositivo disponible
if (devices.Length > 0)
{
    await videoCapture.TorchControl_EnableAsync(devices[0], true);
}

// Deshabilitar antorcha cuando sea necesario
await videoCapture.TorchControl_EnableAsync(devices[0], false);
```

## Ejemplo de Implementación Completa

```cs
using System;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.WindowsExtensions;

namespace Demo_Luz_Camara
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore VideoCapture1;
        private string[] _devices;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (VideoCapture1 != null)
            {
                VideoCapture1.Dispose();
                VideoCapture1 = null;
            }
        }

        private async void btEncender_Click(object sender, EventArgs e)
        {
            if (_devices.Length > 0)
            {
                await VideoCapture1.TorchControl_EnableAsync(_devices[0], true);
            }
        }

        private async void btApagar_Click(object sender, EventArgs e)
        {
            if (_devices.Length > 0)
            {
                await VideoCapture1.TorchControl_EnableAsync(_devices[0], false);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            VideoCapture1 = await VideoCaptureCore.CreateAsync();

            _devices = await VideoCapture1.TorchControl_GetDevicesAsync();
            lbCuentaDispositivos.Text = $"Dispositivos encontrados: {_devices.Length}.";
        }
    }
}
```

## Dependencias Requeridas

Para implementar funcionalidad de luz de cámara en tu aplicación, necesitarás:

1. **Paquete NuGet**: Instala el paquete [VisioForge.DotNet.Core.WindowsExtensions](https://www.nuget.org/packages/VisioForge.DotNet.Core.WindowsExtensions).

2. **Redistribuibles de Video Capture**:
   - [Paquete Redist x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
   - [Paquete Redist x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Aplicación de Ejemplo Completa

Para una implementación completamente funcional, explora nuestra [aplicación Demo de Luz de Cámara](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Camera%20Light%20Demo) disponible en nuestro repositorio de GitHub.

## Notas de Compatibilidad

- Esta funcionalidad está diseñada principalmente para dispositivos tablet con Windows 10 y versiones más nuevas
- El hardware del dispositivo debe soportar control programable de luz de cámara
- Algunos fabricantes de dispositivos pueden implementar APIs propietarias que requieren configuración adicional

## Consejos de Solución de Problemas

Si encuentras problemas al habilitar la luz de cámara:

- Verifica que tu dispositivo tenga hardware compatible
- Asegúrate de que todos los paquetes requeridos estén instalados apropiadamente
- Revisa los permisos del dispositivo en el manifiesto de tu aplicación
- Asegúrate de que el dispositivo se reporte como compatible con antorcha usando TorchControl_GetDevicesAsync()

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para acceder a más muestras de código y ejemplos.