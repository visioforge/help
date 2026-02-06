---
title: Selección de Renderizador de Video en Delphi
description: Seleccionar renderizadores de video óptimos en Delphi - Video Renderer, VMR9, EVR con ejemplos de código para rendimiento y aceleración de hardware en Windows.
---

# Guía de Selección de Renderizador de Video para TVFVideoCapture

## Resumen de Renderizadores Disponibles

Al desarrollar aplicaciones de captura de video con TVFVideoCapture, seleccionar el renderizador de video apropiado impacta significativamente el rendimiento y la compatibilidad. Esta guía proporciona ejemplos detallados de implementación para las tres opciones de renderizador disponibles en entornos Delphi, C++ y VB6.

## Video Renderer Estándar

El Video Renderer estándar utiliza GDI para operaciones de dibujo. Esta opción de renderizador se recomienda principalmente para:

- Sistemas heredados
- Entornos donde la aceleración Direct3D no está disponible
- Máxima compatibilidad con hardware antiguo

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_VideoRenderer;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_VideoRenderer);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_VideoRenderer
```

## Video Mixing Renderer 9 (VMR9)

VMR9 representa una solución de filtrado moderna capaz de aprovechar las capacidades de GPU para renderizado mejorado. Las ventajas clave incluyen:

- Procesamiento de video acelerado por hardware
- Opciones avanzadas de desentrelazado
- Rendimiento mejorado para contenido de alta resolución

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_VMR9;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_VMR9);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_VMR9
```

### Accediendo a Modos de Desentrelazado

VMR9 soporta múltiples técnicas de desentrelazado. El siguiente código demuestra cómo recuperar las opciones de desentrelazado disponibles:

```pascal
// Delphi
VideoCapture1.Video_Renderer_Deinterlace_Modes_Fill;
for I := 0 to VideoCapture1.Video_Renderer_Deinterlace_Modes_GetCount - 1 do
  cbDeinterlaceModes.Items.Add(VideoCapture1.Video_Renderer_Deinterlace_Modes_GetItem(i));
```

```cpp
// C++ MFC
m_VideoCapture.Video_Renderer_Deinterlace_Modes_Fill();
for (int i = 0; i < m_VideoCapture.GetVideo_Renderer_Deinterlace_Modes_GetCount(); i++) {
    m_DeinterlaceCombo.AddString(m_VideoCapture.GetVideo_Renderer_Deinterlace_Modes_GetItem(i));
}
```

```vb
' VB6
VideoCapture1.Video_Renderer_Deinterlace_Modes_Fill
For i = 0 To VideoCapture1.Video_Renderer_Deinterlace_Modes_GetCount - 1
    cboDeinterlaceModes.AddItem VideoCapture1.Video_Renderer_Deinterlace_Modes_GetItem(i)
Next i
```

## Enhanced Video Renderer (EVR)

EVR es el renderizador recomendado para entornos Windows modernos (Vista y posterior). Este renderizador avanzado proporciona:

- Capacidades superiores de aceleración de video
- Rendimiento óptimo en Windows 7/10/11
- Mejor utilización de recursos

```pascal
// Delphi
VideoCapture1.Video_Renderer := VR_EVR;
```

```cpp
// C++ MFC
m_VideoCapture.SetVideo_Renderer(VR_EVR);
```

```vb
' VB6
VideoCapture1.Video_Renderer = VR_EVR
```

## Gestionando Relación de Aspecto y Opciones de Visualización

Al mostrar contenido de video, a menudo necesitará manejar diferencias de relación de aspecto entre el video fuente y el área de visualización.

### Estirando la Imagen de Video

Para estirar el video para llenar toda el área de visualización:

```pascal
// Delphi
VideoCapture1.Screen_Stretch := true;
VideoCapture1.Screen_Update;
```

```cpp
// C++ MFC
m_VideoCapture.SetScreen_Stretch(true);
m_VideoCapture.Screen_Update();
```

```vb
' VB6
VideoCapture1.Screen_Stretch = True
VideoCapture1.Screen_Update
```

### Usando Modo Letterbox (Bordes Negros)

Para preservar la relación de aspecto original con bordes negros:

```pascal
// Delphi
VideoCapture1.Screen_Stretch := false;
VideoCapture1.Screen_Update;
```

```cpp
// C++ MFC
m_VideoCapture.SetScreen_Stretch(false);
m_VideoCapture.Screen_Update();
```

```vb
' VB6
VideoCapture1.Screen_Stretch = False
VideoCapture1.Screen_Update
```

## Consideraciones de Rendimiento

Al seleccionar un renderizador para su aplicación, considere estos factores:

1. Versión del sistema operativo objetivo
2. Capacidades de hardware de los sistemas de usuarios finales
3. Resolución de video y requisitos de procesamiento
4. Necesidades de compatibilidad para su entorno de despliegue

---
Por favor contacte con [soporte](https://support.visioforge.com/) si necesita asistencia técnica con esta implementación. Visite nuestro repositorio de [GitHub](https://github.com/visioforge/) para ejemplos de código adicionales y recursos.