---
title: Grabación de Pantalla en Aplicaciones Delphi
description: Grabación de pantalla en Delphi con TVFVideoCapture: captura regiones, pantalla completa, tasas de fotogramas y cursor con ejemplos de código.
---

# Implementación de Grabación de Pantalla en Delphi

## Introducción a la Funcionalidad de Captura de Pantalla

TVFVideoCapture proporciona potentes capacidades de grabación de pantalla para desarrolladores Delphi. Esta guía recorre la implementación de características de captura de pantalla en sus aplicaciones, permitiéndole grabar regiones específicas o la pantalla completa con ajustes personalizables.

## Configurando el Área de Captura de Pantalla

Puede controlar precisamente qué porción de la pantalla grabar estableciendo parámetros de coordenadas. Esto es particularmente útil cuando desea enfocarse en ventanas de aplicación específicas o regiones de pantalla.

### Estableciendo Coordenadas de Pantalla Específicas

Use estos parámetros para definir los límites exactos de su área de captura:

```pascal
// Definir la posición del borde superior del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Top := StrToInt(edScreenTop.Text);
// Definir la posición del borde inferior del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Bottom := StrToInt(edScreenBottom.Text);
// Definir la posición del borde izquierdo del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Left := StrToInt(edScreenLeft.Text);
// Definir la posición del borde derecho del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Right := StrToInt(edScreenRight.Text);
```

```cpp
// Definir la posición del borde superior del rectángulo de captura (en píxeles)
m_VideoCapture.SetScreen_Capture_Top(atoi(m_edScreenTop.GetWindowText()));
// Definir la posición del borde inferior del rectángulo de captura (en píxeles)
m_VideoCapture.SetScreen_Capture_Bottom(atoi(m_edScreenBottom.GetWindowText()));
// Definir la posición del borde izquierdo del rectángulo de captura (en píxeles)
m_VideoCapture.SetScreen_Capture_Left(atoi(m_edScreenLeft.GetWindowText()));
// Definir la posición del borde derecho del rectángulo de captura (en píxeles)
m_VideoCapture.SetScreen_Capture_Right(atoi(m_edScreenRight.GetWindowText()));
```

```vb
' Definir la posición del borde superior del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Top = CInt(edScreenTop.Text)
' Definir la posición del borde inferior del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Bottom = CInt(edScreenBottom.Text)
' Definir la posición del borde izquierdo del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Left = CInt(edScreenLeft.Text)
' Definir la posición del borde derecho del rectángulo de captura (en píxeles)
VideoCapture1.Screen_Capture_Right = CInt(edScreenRight.Text)
```

### Capturando la Pantalla Completa

Para grabación de pantalla completa, simplemente habilite la opción de captura de pantalla completa:

```pascal
// Habilitar modo de captura de pantalla completa - grabará toda la pantalla
VideoCapture1.Screen_Capture_FullScreen := true;
```

```cpp
// Habilitar modo de captura de pantalla completa - grabará toda la pantalla
m_VideoCapture.SetScreen_Capture_FullScreen(true);
```

```vb
' Habilitar modo de captura de pantalla completa - grabará toda la pantalla
VideoCapture1.Screen_Capture_FullScreen = True
```

## Optimizando Ajustes de Tasa de Fotogramas

La tasa de fotogramas impacta directamente tanto la calidad como el tamaño del archivo de sus grabaciones de pantalla. Tasas de fotogramas más altas producen video más suave pero generan archivos más grandes.

```pascal
// Establecer tasa de fotogramas de captura a 10 fotogramas por segundo
// Ajuste este valor según sus requisitos de rendimiento
VideoCapture1.Screen_Capture_FrameRate := 10;
```

```cpp
// Establecer tasa de fotogramas de captura a 10 fotogramas por segundo
// Ajuste este valor según sus requisitos de rendimiento
m_VideoCapture.SetScreen_Capture_FrameRate(10);
```

```vb
' Establecer tasa de fotogramas de captura a 10 fotogramas por segundo
' Ajuste este valor según sus requisitos de rendimiento
VideoCapture1.Screen_Capture_FrameRate = 10
```

## Configuración de Rastreo del Cursor

Para videos instructivos o demostraciones, capturar el movimiento del cursor del ratón es esencial:

```pascal
// Habilitar captura del cursor del ratón en la grabación
// Establecer a false para ocultar el cursor en el video de salida
VideoCapture1.Screen_Capture_Grab_Mouse_Cursor := true;
```

```cpp
// Habilitar captura del cursor del ratón en la grabación
// Establecer a false para ocultar el cursor en el video de salida
m_VideoCapture.SetScreen_Capture_Grab_Mouse_Cursor(true);
```

```vb
' Habilitar captura del cursor del ratón en la grabación
' Establecer a false para ocultar el cursor en el video de salida
VideoCapture1.Screen_Capture_Grab_Mouse_Cursor = True
```

## Activando el Modo de Captura de Pantalla

Después de configurar todos los ajustes, establezca el componente en modo de captura de pantalla para comenzar a grabar:

```pascal
// Establecer componente en modo operacional de captura de pantalla
// Esto activa toda la funcionalidad de grabación de pantalla
VideoCapture1.Mode := Mode_Screen_Capture;
```

```cpp
// Establecer componente en modo operacional de captura de pantalla
// Esto activa toda la funcionalidad de grabación de pantalla
m_VideoCapture.SetMode(Mode_Screen_Capture);
```

```vb
' Establecer componente en modo operacional de captura de pantalla
' Esto activa toda la funcionalidad de grabación de pantalla
VideoCapture1.Mode = Mode_Screen_Capture
```

## Consejos Avanzados de Implementación

Para un rendimiento óptimo de grabación de pantalla:

- Considere los recursos del sistema al seleccionar tasas de fotogramas
- Use captura de región cuando sea posible para minimizar la carga de procesamiento
- Pruebe diferentes ajustes de calidad para balancear tamaño de archivo y calidad visual
- Recuerde que la captura del cursor añade una ligera sobrecarga de procesamiento

---
Para ejemplos de código adicionales y ejemplos de implementación, visite nuestro repositorio de [GitHub](https://github.com/visioforge/). Para asistencia técnica con la implementación, por favor contacte a nuestro [equipo de soporte](https://support.visioforge.com/).