---
title: Procesar Video Delphi - Tutorial Redimensionar & Recortar
description: Redimensionamiento y recorte de video en Delphi: procesamiento en tiempo real, manejo de relación de aspecto y ejemplos de código optimizados.
---

# Redimensionamiento y Recorte de Video en Delphi TVFVideoCapture

La manipulación de video es un componente crítico de muchas aplicaciones modernas. Esta guía proporciona instrucciones detalladas para implementar redimensionamiento y recorte de video en tiempo real en sus aplicaciones Delphi con impacto mínimo en el rendimiento.

## ¿Por Qué Redimensionar o Recortar Video?

El redimensionamiento y recorte de video sirven múltiples propósitos en el desarrollo:

- Optimizar video para diferentes tamaños de pantalla
- Reducir requisitos de ancho de banda para streaming
- Enfocarse en regiones específicas de interés
- Crear dimensiones de video uniformes en toda su aplicación
- Mejorar el rendimiento en dispositivos con recursos limitados

## Habilitando la Funcionalidad de Redimensionar y Recortar

Antes de aplicar cualquier transformación, debe habilitar la funcionalidad de redimensionar/recortar en el componente TVFVideoCapture.

### Paso 1: Habilitar la Característica

```pascal
// Habilitar funcionalidad de redimensionamiento o recorte de video
VideoCapture1.Video_ResizeOrCrop_Enabled := true;
```

```cpp
// C++ MFC - Habilitar capacidades de transformación de video
m_VideoCapture.SetVideo_ResizeOrCrop_Enabled(TRUE);
```

```vb
' VB6 - Activar características de redimensionar/recortar
VideoCapture1.Video_ResizeOrCrop_Enabled = True
```

## Implementación de Redimensionamiento de Video

El redimensionamiento le permite cambiar las dimensiones de su stream de video mientras mantiene la calidad visual.

### Estableciendo Nuevas Dimensiones

```pascal
// Establecer el ancho y alto deseados para la salida de video redimensionado
VideoCapture1.Video_Resize_NewWidth := StrToInt(edResizeWidth.Text);
VideoCapture1.Video_Resize_NewHeight := StrToInt(edResizeHeight.Text);
```

```cpp
// C++ MFC - Configurar dimensiones objetivo para redimensionamiento de video
m_VideoCapture.SetVideo_Resize_NewWidth(_ttoi(m_strResizeWidth));
m_VideoCapture.SetVideo_Resize_NewHeight(_ttoi(m_strResizeHeight));
```

```vb
' VB6 - Definir nuevas dimensiones de video
VideoCapture1.Video_Resize_NewWidth = CInt(txtResizeWidth.Text)
VideoCapture1.Video_Resize_NewHeight = CInt(txtResizeHeight.Text)
```

### Manejando Cambios de Relación de Aspecto

Al redimensionar video, puede elegir entre preservar la relación de aspecto original (letterbox) o estirar el contenido para ajustarse a las nuevas dimensiones.

```pascal
// El modo letterbox añade bordes negros para preservar la relación de aspecto
// Cuando es false, el video se estirará para ajustarse a las nuevas dimensiones
VideoCapture1.Video_Resize_LetterBox := cbResizeLetterbox.Checked;
```

```cpp
// C++ MFC - Configurar método de manejo de relación de aspecto
m_VideoCapture.SetVideo_Resize_LetterBox(m_bResizeLetterbox);
```

```vb
' VB6 - Establecer modo letterbox para preservación de relación de aspecto
VideoCapture1.Video_Resize_LetterBox = chkResizeLetterbox.Value
```

### Seleccionando Algoritmos de Redimensionamiento

Elija entre múltiples algoritmos de redimensionamiento basándose en sus requisitos de calidad y restricciones de rendimiento:

```pascal
// Seleccionar el algoritmo de redimensionamiento apropiado:
// - NearestNeighbor: Más rápido pero menor calidad
// - Bilinear: Buen balance entre velocidad y calidad
// - Bilinear_HQ: Bilinear mejorado con calidad mejorada
// - Bicubic: Mejor calidad con impacto moderado en rendimiento
// - Bicubic_HQ: Máxima calidad con mayor uso de CPU
case cbResizeMode.ItemIndex of
  0: VideoCapture1.Video_Resize_Mode := rm_NearestNeighbor;
  1: VideoCapture1.Video_Resize_Mode := rm_Bilinear;
  2: VideoCapture1.Video_Resize_Mode := rm_Bilinear_HQ;
  3: VideoCapture1.Video_Resize_Mode := rm_Bicubic;
  4: VideoCapture1.Video_Resize_Mode := rm_Bicubic_HQ;
end;
```

```cpp
// C++ MFC - Establecer el algoritmo de redimensionamiento basado en necesidades de calidad/rendimiento
switch(m_nResizeMode)
{
  case 0: m_VideoCapture.SetVideo_Resize_Mode(rm_NearestNeighbor); break; // Más rápido
  case 1: m_VideoCapture.SetVideo_Resize_Mode(rm_Bilinear); break;        // Estándar
  case 2: m_VideoCapture.SetVideo_Resize_Mode(rm_Bilinear_HQ); break;     // Mejorado
  case 3: m_VideoCapture.SetVideo_Resize_Mode(rm_Bicubic); break;         // Alta calidad
  case 4: m_VideoCapture.SetVideo_Resize_Mode(rm_Bicubic_HQ); break;      // Máxima calidad
}
```

```vb
' VB6 - Elegir algoritmo de redimensionamiento basado en necesidades de calidad y rendimiento
Select Case cboResizeMode.ListIndex
  Case 0: VideoCapture1.Video_Resize_Mode = rm_NearestNeighbor  ' Más rápido, menor calidad
  Case 1: VideoCapture1.Video_Resize_Mode = rm_Bilinear         ' Opción balanceada
  Case 2: VideoCapture1.Video_Resize_Mode = rm_Bilinear_HQ      ' Bilinear mejorado
  Case 3: VideoCapture1.Video_Resize_Mode = rm_Bicubic          ' Mejor calidad
  Case 4: VideoCapture1.Video_Resize_Mode = rm_Bicubic_HQ       ' Máxima calidad
End Select
```

## Implementación de Recorte de Video

El recorte le permite seleccionar una región específica de interés de su stream de video.

### Paso 1: Habilitar Recorte

Al igual que con el redimensionamiento, primero debe habilitar la característica:

```pascal
// Habilitar capacidades de transformación de video antes de aplicar recorte
VideoCapture1.Video_ResizeOrCrop_Enabled := true;
```

```cpp
// C++ MFC - Activar características de manipulación de video
m_VideoCapture.SetVideo_ResizeOrCrop_Enabled(TRUE);
```

```vb
' VB6 - Habilitar funcionalidad de transformación de video
VideoCapture1.Video_ResizeOrCrop_Enabled = True
```

### Paso 2: Definir Región de Recorte

Especifique los límites de su región de recorte definiendo las coordenadas izquierda, superior, derecha e inferior:

```pascal
// Definir las coordenadas de la región de recorte en píxeles
// Estos valores representan la distancia desde cada borde del video original
VideoCapture1.Video_Crop_Left := StrToInt(edCropLeft.Text);
VideoCapture1.Video_Crop_Top := StrToInt(edCropTop.Text);
VideoCapture1.Video_Crop_Right := StrToInt(edCropRight.Text);
VideoCapture1.Video_Crop_Bottom := StrToInt(edCropBottom.Text);
```

```cpp
// C++ MFC - Establecer los límites de recorte en píxeles
// Cada valor define cuántos píxeles recortar del borde respectivo
m_VideoCapture.SetVideo_Crop_Left(_ttoi(m_strCropLeft));
m_VideoCapture.SetVideo_Crop_Top(_ttoi(m_strCropTop));
m_VideoCapture.SetVideo_Crop_Right(_ttoi(m_strCropRight));
m_VideoCapture.SetVideo_Crop_Bottom(_ttoi(m_strCropBottom));
```

```vb
' VB6 - Configurar límites de región de recorte
' Los valores representan conteos de píxeles desde cada borde a excluir
VideoCapture1.Video_Crop_Left = CInt(txtCropLeft.Text)
VideoCapture1.Video_Crop_Top = CInt(txtCropTop.Text)
VideoCapture1.Video_Crop_Right = CInt(txtCropRight.Text)
VideoCapture1.Video_Crop_Bottom = CInt(txtCropBottom.Text)
```

## Mejores Prácticas para Manipulación de Video

Para resultados óptimos al implementar redimensionamiento y recorte de video:

1. **Probar en hardware objetivo** - Diferentes algoritmos de redimensionamiento tienen diferentes requisitos de CPU
2. **Considere su caso de uso** - Para aplicaciones en tiempo real, favorezca el rendimiento sobre la calidad
3. **Mantener relaciones de aspecto** - A menos que sea específicamente necesario, preserve las proporciones originales
4. **Combinar operaciones con prudencia** - Aplicar tanto redimensionamiento como recorte aumenta la sobrecarga de procesamiento
5. **Cachear ajustes** - Evite cambiar parámetros frecuentemente durante la captura

## Solución de Problemas Comunes

- Si el rendimiento es pobre, pruebe un algoritmo de redimensionamiento más rápido
- Asegúrese de que los valores de recorte no excedan las dimensiones de su stream de video
- Al usar modo letterbox, tenga en cuenta los bordes negros en el diseño de su UI
- Para mejores resultados, redimensione a dimensiones que sean múltiplos de 8 o 16

---
Para ejemplos de código adicionales y ejemplos de implementación, visite nuestro repositorio de [GitHub](https://github.com/visioforge/). ¿Necesita asistencia técnica? Contacte a nuestro equipo de soporte para orientación personalizada.