---
title: Superposiciones de Texto Dinámico en Video con .NET y C#
description: Cree múltiples superposiciones de texto en video con OnVideoFrameBuffer, propiedades personalizables y actualizaciones dinámicas en .NET.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - C#
primary_api_classes:
  - VideoFrameBufferEventArgs
---

# Implementación de superposiciones de texto dinámicas en frames de video en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Agregar superposiciones de texto al contenido de video se ha vuelto esencial para varias aplicaciones, desde agregar marcas de agua y marcas de tiempo hasta crear anotaciones informativas y subtítulos. Mientras que muchos SDKs ofrecen capacidades de superposición de texto incorporadas, estas funciones podrían no siempre proporcionar el nivel de personalización o flexibilidad requerido para proyectos avanzados.

Esta guía demuestra cómo implementar superposiciones de texto personalizadas usando el evento `OnVideoFrameBuffer`. Este enfoque le da control total sobre la apariencia, posición y comportamiento del texto, permitiendo implementaciones de superposición más sofisticadas de lo que es posible con métodos API estándar.

## ¿Por qué usar superposiciones de texto personalizadas?

Las APIs de superposición de texto estándar a menudo tienen limitaciones en áreas como:

- Número de elementos de texto concurrentes
- Opciones de personalización de fuente
- Actualizaciones de texto dinámicas
- Capacidades de animación
- Control de posicionamiento preciso
- Gestión de canal alfa

Al aprovechar el evento `OnVideoFrameBuffer` y trabajar directamente con datos de bitmap, puede superar estas limitaciones e implementar exactamente lo que su aplicación necesita.

## Entender el enfoque

La técnica demostrada en este artículo involucra:

1. Crear un bitmap transparente con las mismas dimensiones que el frame de video
2. Dibujar elementos de texto en este bitmap usando GDI+ (System.Drawing)
3. Convertir el bitmap a un búfer de memoria
4. Superponer este búfer en los datos del frame de video
5. Opcionalmente actualizar elementos de texto dinámicamente

Esto proporciona un método poderoso para la creación de superposiciones de texto mientras mantiene buen rendimiento.

## Implementación básica

El siguiente ejemplo de código muestra una implementación directa para dibujar múltiples superposiciones de texto en frames de video:

```cs
        // Imagen
        private Bitmap logoImage = null;

        // Búfer RGB32 de imagen
        private IntPtr logoImageBuffer = IntPtr.Zero;
        private int logoImageBufferSize = 0;

        private string text1 = "Hola Mundo";
        private string text2 = "Hey-hey";
        private string text3 = "Océano de panqueques";

        private void SDK_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
        {
            // dibujar texto en imagen
            if (logoImage == null)
            {
                logoImage = new Bitmap(e.Frame.Info.Width, e.Frame.Info.Height, PixelFormat.Format32bppArgb);

                using (var grf = Graphics.FromImage(logoImage))
                {
                    // modo de antialiasing
                    grf.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // modo de dibujo
                    grf.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // modo de suavizado
                    grf.SmoothingMode = SmoothingMode.HighQuality;

                    // texto 1
                    var brush1 = new SolidBrush(Color.Blue);
                    var font1 = new Font("Arial", 30, FontStyle.Regular);
                    grf.DrawString(text1, font1, brush1, 100, 100);

                    // texto 2
                    var brush2 = new SolidBrush(Color.Red);
                    var font2 = new Font("Times New Roman", 35, FontStyle.Strikeout);
                    grf.DrawString(text2, font2, brush2, e.Frame.Info.Width / 2, e.Frame.Info.Height / 2);

                    // texto 3
                    var brush3 = new SolidBrush(Color.Green);
                    var font3 = new Font("Verdana", 40, FontStyle.Italic);
                    grf.DrawString(text3, font3, brush3, 200, 200);
                }
            }

            // crear búfer de imagen si no está asignado o tiene tamaño cero
            if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
            {
                if (logoImageBuffer == IntPtr.Zero)
                {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                }
                else
                {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;

                        Marshal.FreeCoTaskMem(logoImageBuffer);
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                }

                BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
            }

            // Dibujar imagen
            FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width, e.Frame.Info.Height, 0, 0);

            e.UpdateData = true;
        }
```

### Componentes clave explicados

1. **Creación de Bitmap**: Creamos un bitmap de 32 bits (con canal alfa) que coincide con las dimensiones del frame de video
2. **Configuración de Gráficos**: Configuramos anti-aliasing, interpolación y suavizado para renderizado de texto de alta calidad
3. **Configuración de Texto**: Cada elemento de texto obtiene su propia fuente, color y posición
4. **Gestión de Memoria**: Asignamos memoria no administrada para el búfer de bitmap
5. **Conversión de Bitmap a Búfer**: Convertimos el bitmap a un búfer de memoria usando `BitmapHelper.BitmapToIntPtr`
6. **Superposición de Búfer**: Dibujamos el búfer RGBA en el frame de video usando `FastImageProcessing.Draw_RGB32OnRGB24`
7. **Bandera de Actualización de Frame**: Establecemos `e.UpdateData = true` para informar al SDK que los datos del frame han sido modificados

## Implementación avanzada con actualizaciones dinámicas

Para aplicaciones más interactivas, podría necesitar actualizar las superposiciones de texto dinámicamente. La siguiente implementación soporta actualizaciones en tiempo real del contenido de texto, fuentes y colores:

```cs
        // Imagen
        Bitmap logoImage = null;

        // Búfer RGB32 de imagen
        IntPtr logoImageBuffer = IntPtr.Zero;
        int logoImageBufferSize = 0;

        // configuración de texto
        string text1 = "Hola Mundo";
        Font font1 = new Font("Arial", 30, FontStyle.Regular);
        SolidBrush brush1 = new SolidBrush(Color.Blue);

        string text2 = "Hey-hey";
        Font font2 = new Font("Times New Roman", 35, FontStyle.Strikeout);
        SolidBrush brush2 = new SolidBrush(Color.Red);

        string text3 = "Océano de panqueques";
        Font font3 = new Font("Verdana", 40, FontStyle.Italic);
        SolidBrush brush3 = new SolidBrush(Color.Green);

        // bandera de actualización
        bool textUpdate = false;
        object textLock = new object();

        // Actualizar superposición de texto, índice es [1..3]
        void UpdateText(int index, string text, Font font, SolidBrush brush)
        {
            lock (textLock)
            {
                textUpdate = true;
            }

            switch (index)
            {
                case 1:
                    text1 = text;
                    font1 = font;
                    brush1 = brush;
                    break;
                case 2:
                    text2 = text;
                    font2 = font;
                    brush2 = brush;
                    break;
                case 3:
                    text3 = text;
                    font3 = font;
                    brush3 = brush;
                    break;
                default:
                    return;
            }
        }

        private void SDK_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
        {
            lock (textLock)
            {
                if (textUpdate)
                {
                    logoImage.Dispose();
                    logoImage = null;
                }

                // dibujar texto en imagen
                if (logoImage == null)
                {
                    logoImage = new Bitmap(e.Frame.Info.Width, e.Frame.Info.Height, PixelFormat.Format32bppArgb);

                    using (var grf = Graphics.FromImage(logoImage))
                    {
                        // modo de antialiasing
                        grf.TextRenderingHint = TextRenderingHint.AntiAlias;

                        // modo de dibujo
                        grf.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        // modo de suavizado
                        grf.SmoothingMode = SmoothingMode.HighQuality;

                        // texto 1
                        grf.DrawString(text1, font1, brush1, 100, 100);

                        // texto 2
                        grf.DrawString(text2, font2, brush2, e.Frame.Info.Width / 2, e.Frame.Info.Height / 2);

                        // texto 3
                        grf.DrawString(text3, font3, brush3, 200, 200);
                    }
                }

                // crear búfer de imagen si no está asignado o tiene tamaño cero
                if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
                {
                    if (logoImageBuffer == IntPtr.Zero)
                    {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(e.Frame.Info.Width) * e.Frame.Info.Height;
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                    }
                    else
                    {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(e.Frame.Info.Width) * e.Frame.Info.Height;

                        Marshal.FreeCoTaskMem(logoImageBuffer);
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                    }

                    BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
                }

                if (textUpdate)
                {
                    textUpdate = false;
                    BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
                }

                // Dibujar imagen
                FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
                e.Frame.Info.Height, 0, 0);

                e.UpdateData = true;
            }
        }

        private void btUpdateText1_Click(object sender, EventArgs e)
        {
            UpdateText(1, "Hola mundo", new Font("Arial", 48, FontStyle.Underline),
                new SolidBrush(Color.Aquamarine));
        }
```

### Nuevas características en la implementación avanzada

1. **Seguridad de Hilos**: Usamos un objeto lock para prevenir acceso concurrente a recursos compartidos
2. **Mecanismo de Actualización**: El método `UpdateText` proporciona una interfaz limpia para cambiar propiedades de texto
3. **Almacenamiento de Propiedades de Texto**: Cada elemento de texto tiene sus propias variables para contenido, fuente y color
4. **Detección de Cambios**: Usamos una bandera (`textUpdate`) para indicar cuando las propiedades del texto han cambiado
5. **Gestión de Recursos**: Liberamos el bitmap antiguo cuando las propiedades del texto cambian
6. **Actualización de Búfer**: Actualizamos el búfer de memoria cuando las propiedades del texto cambian
7. **Integración de UI**: Un manejador de clic de botón de ejemplo demuestra cómo activar actualizaciones de texto

## Consejos de optimización de rendimiento

Al implementar superposiciones de texto con este método, considere estas optimizaciones de rendimiento:

1. **Minimizar Recreaciones de Bitmap**: Solo recree el bitmap cuando sea necesario (cambios de texto, cambios de resolución)
2. **Cachear Objetos Font**: La creación de fuentes es costosa; cree fuentes una vez y reutilícelas
3. **Usar Memoria Eficientemente**: Libere memoria no administrada cuando ya no se necesite
4. **Optimizar Operaciones de Dibujo**: Use aceleración de hardware cuando esté disponible
5. **Considere la Frecuencia de Actualización**: Para actualizaciones frecuentes, considere técnicas de doble búfer
6. **Perfile su código**: Use herramientas de perfilado de rendimiento para identificar cuellos de botella

## Características avanzadas a considerar

Esta implementación básica puede extenderse con características adicionales:

1. **Animación de Texto**: Implemente movimiento de texto, desvanecimiento u otras animaciones
2. **Formato de Texto**: Agregue soporte para formato de texto enriquecido (negrita, cursiva, etc.)
3. **Efectos de Texto**: Implemente sombras, contornos o efectos de brillo
4. **Alineación de Texto**: Agregue soporte para diferentes opciones de alineación de texto
5. **Texto Multi-línea**: Implemente manejo apropiado de texto multi-línea con ajuste
6. **Localización**: Agregue soporte para diferentes idiomas y direcciones de texto
7. **Monitoreo de Rendimiento**: Agregue diagnósticos para monitorear el rendimiento de renderizado

## Consideraciones de gestión de memoria

Al trabajar con memoria no administrada, es crucial manejar la limpieza de recursos correctamente:

1. Implemente el patrón `IDisposable` en su clase
2. Libere memoria no administrada en el método `Dispose`
3. Considere usar `SafeHandle` o construcciones similares para gestión de recursos más segura
4. Establezca punteros de búfer a `IntPtr.Zero` después de liberarlos
5. Use manejo de excepciones estructurado alrededor de operaciones de memoria

## Ejemplo de limpieza

```cs
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Liberar recursos administrados
        if (logoImage != null)
        {
            logoImage.Dispose();
            logoImage = null;
        }
    }
    
    // Liberar recursos no administrados
    if (logoImageBuffer != IntPtr.Zero)
    {
        Marshal.FreeCoTaskMem(logoImageBuffer);
        logoImageBuffer = IntPtr.Zero;
        logoImageBufferSize = 0;
    }
    
    base.Dispose(disposing);
}
```

## Dependencias requeridas

- Componentes redistribuibles del SDK

## Conclusión

Implementar superposiciones de texto personalizadas usando el evento `OnVideoFrameBuffer` proporciona una solución poderosa y flexible para aplicaciones que requieren capacidades avanzadas de visualización de texto. Aunque requiere más código que usar métodos API incorporados, la flexibilidad y control adicionales lo hacen valioso para aplicaciones de video sofisticadas.

Siguiendo los patrones demostrados en esta guía, puede crear superposiciones de texto dinámicas y de alta calidad que pueden actualizarse en tiempo real, proporcionando una experiencia de usuario rica en sus aplicaciones de video.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.