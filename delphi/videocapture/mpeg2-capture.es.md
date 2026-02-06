---
title: Captura de Video MPEG-2 en Delphi con Hardware de Sintonizador de TV
description: Captura MPEG-2 en Delphi usando codificadores de hardware de sintonizador de TV: enumeración de dispositivos, configuración y ejemplos optimizados.
---

# Captura de Video MPEG-2 en Delphi Usando Codificadores de Hardware de Sintonizador de TV

Este tutorial completo demuestra cómo implementar funcionalidad de captura de video MPEG-2 de alta calidad en sus aplicaciones Delphi aprovechando sintonizadores de TV con capacidades de codificación de hardware integradas. La codificación por hardware reduce significativamente el uso de CPU mientras mantiene excelente calidad de video.

## Resumen de la Codificación MPEG-2 por Hardware

Los codificadores MPEG-2 por hardware proporcionan un rendimiento superior en comparación con soluciones de codificación basadas en software. Son particularmente útiles para desarrollar aplicaciones profesionales de captura de video que requieren procesamiento eficiente y salida de alta calidad.

## Enumerando Codificadores MPEG-2 de Hardware Disponibles

El primer paso es identificar todos los codificadores MPEG-2 de hardware disponibles en el sistema. Este código demuestra cómo poblar un desplegable con los dispositivos detectados:

```pascal
// Listar todos los codificadores MPEG-2 de hardware disponibles en el sistema
// Esto ayuda a los usuarios a seleccionar el dispositivo de codificación apropiado
VideoCapture1.Special_Filters_Fill;
for I := 0 to VideoCapture1.Special_Filters_GetCount(SF_Hardware_Video_Encoder) - 1 do
  cbMPEGEncoder.Items.Add(VideoCapture1.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i));
```

```cpp
// C++ MFC implementación para enumeración de codificadores MPEG-2
// Pobla un combobox con todos los codificadores de hardware detectados
m_VideoCapture.Special_Filters_Fill();
for (int i = 0; i < m_VideoCapture.Special_Filters_GetCount(SF_Hardware_Video_Encoder); i++)
{
  CString encoderName = m_VideoCapture.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i);
  m_cbMPEGEncoder.AddString(encoderName);
}
```

```vb
' VB6 implementación para encontrar codificadores MPEG-2 de hardware
' Lista todos los codificadores disponibles en un control combobox
VideoCapture1.Special_Filters_Fill
For i = 0 To VideoCapture1.Special_Filters_GetCount(SF_Hardware_Video_Encoder) - 1
  cbMPEGEncoder.AddItem VideoCapture1.Special_Filters_GetItem(SF_Hardware_Video_Encoder, i)
Next i
```

## Seleccionando un Codificador MPEG-2 Específico

Después de enumerar los codificadores disponibles, el siguiente paso es seleccionar un codificador específico para usar:

```pascal
// Configurar el componente para usar el codificador MPEG-2 de hardware seleccionado
// Esto debe hacerse antes de iniciar el proceso de captura
VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name := cbMPEGEncoder.Items[cbMPEGEncoder.ItemIndex];
```

```cpp
// C++ MFC: Seleccionar y configurar el codificador MPEG-2 de hardware elegido
// Recupera el nombre del codificador seleccionado del combobox
int nIndex = m_cbMPEGEncoder.GetCurSel();
CString encoderName;
m_cbMPEGEncoder.GetLBText(nIndex, encoderName);
m_VideoCapture.Video_CaptureDevice_InternalMPEGEncoder_Name = encoderName;
```

```vb
' VB6: Establecer el codificador seleccionado como el codificador MPEG-2 de hardware activo
' Debe llamarse antes de inicializar el grafo de captura
VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name = cbMPEGEncoder.List(cbMPEGEncoder.ListIndex)
```

## Configurando el Formato DirectStream MPEG para Salida

Para capturar correctamente video codificado en MPEG-2, necesita establecer el formato de salida apropiado:

```pascal
// Establecer el formato de salida a DirectStream MPEG
// Esto habilita el manejo adecuado de streams MPEG-2 codificados por hardware
VideoCapture1.OutputFormat := Format_DirectStream_MPEG;
```

```cpp
// C++ MFC: Configurar el formato de salida para contenido codificado MPEG-2
// El formato DirectStream MPEG preserva el stream codificado por hardware
m_VideoCapture.OutputFormat = Format_DirectStream_MPEG;
```

```vb
' VB6: Establecer el formato de salida apropiado para codificación MPEG-2 por hardware
' El formato DirectStream asegura que los datos codificados se manejen correctamente
VideoCapture1.OutputFormat = Format_DirectStream_MPEG
```

## Estableciendo el Modo de Captura de Video

Antes de iniciar el proceso de captura, establezca el componente en modo de captura de video:

```pascal
// Configurar el componente para operación de captura de video
// Esto prepara el grafo DirectShow interno para grabación
VideoCapture1.Mode := Mode_Video_Capture;
```

```cpp
// C++ MFC: Establecer el componente en modo de captura de video
// Requerido antes de iniciar el proceso de captura MPEG-2
m_VideoCapture.Mode = Mode_Video_Capture;
```

```vb
' VB6: Establecer modo de captura de video antes de iniciar grabación
' Esto inicializa los filtros DirectShow apropiados
VideoCapture1.Mode = Mode_Video_Capture
```

## Iniciando el Proceso de Captura MPEG-2

Finalmente, inicie el proceso de captura para comenzar a grabar video MPEG-2:

```pascal
// Iniciar el proceso de captura de video con los ajustes configurados
// El componente ahora comenzará a grabar a la salida especificada
VideoCapture1.Start;
```

```cpp
// C++ MFC: Iniciar el proceso de captura de video MPEG-2
// La grabación comienza con los ajustes configurados previamente
m_VideoCapture.Start();
```

```vb
' VB6: Iniciar la captura de video con la configuración actual
' El codificador de hardware ahora comenzará a procesar datos de video
VideoCapture1.Start
```

## Consideraciones Avanzadas de Captura MPEG-2

Al implementar captura MPEG-2 con codificadores de hardware, considere estos factores adicionales:

1. Los codificadores de hardware típicamente ofrecen mejor rendimiento que las soluciones basadas en software
2. Algunos sintonizadores de TV proporcionan parámetros de codificación adicionales que pueden personalizarse
3. Los tamaños de buffer pueden necesitar ajuste para capturas de mayor calidad
4. Los codificadores de hardware a menudo manejan el escalado de video y conversión de tasa de fotogramas internamente

## Solución de Problemas Comunes

Si encuentra problemas con la codificación MPEG-2 por hardware:

1. Verifique que su dispositivo sintonizador de TV soporte codificación MPEG-2 por hardware
2. Asegure la instalación correcta de controladores para el dispositivo de captura
3. Verifique que DirectX esté correctamente instalado y actualizado
4. Considere la disponibilidad de recursos del sistema, ya que algunos codificadores requieren recursos específicos

Por favor contacte a nuestro equipo de soporte dedicado para asistencia con la implementación de este tutorial en su aplicación específica. Visite nuestro repositorio de GitHub para ejemplos de código adicionales y ejemplos de implementación.
