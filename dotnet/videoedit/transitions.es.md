---
title: Transiciones de Video y Efectos SMPTE en .NET
description: Explora más de 100 efectos de transición de video en Video Edit SDK para .NET con barridos estándar SMPTE y bordes, suavidad y posicionamiento personalizables.
---

# Transiciones

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

La transición de Barrido SMPTE produce cualquiera de los barridos estándar definidos por la Sociedad de Ingenieros de Cine y Televisión (SMPTE) en el documento SMPTE 258M-1993, excepto el quad split.

## Propiedades

| Propiedad       | Tipo          | Predeterminado | Descripción |
|-----------------|---------------|----------------|-------------|
| Border Color    | Color/TColor  | Negro          | Color del borde alrededor de los bordes del patrón de barrido. |
| BorderSoftness  | long          | 0              | Ancho de la región borrosa alrededor de los bordes del patrón de barrido. Especifica cero para ninguna región borrosa. |
| BorderWidth     | long          | 0              | Ancho del borde sólido a lo largo de los bordes del patrón de barrido. Especifica cero para ningún borde. |
| ID              | long          | 1              | Código de barrido estándar SMPTE especificando el estilo de barrido a usar. Para una lista de códigos de barrido y sus esquemas asociados, consulta el documento SMPTE 258M-1993. |
| OffsetX         | long          | 0              | Desplazamiento horizontal del origen del barrido desde el centro de la imagen. Válido solo para valores de **ID** de 101 a 131. |
| OffsetY         | long          | 0              | Desplazamiento vertical del origen del barrido desde el centro de la imagen. Válido solo para valores de **ID** de 101 a 131. |
| ReplicateX      | long          | 0              | Número de veces para replicar el patrón de barrido horizontalmente. Válido solo para valores de **ID** de 101 a 131. |
| ReplicateY      | long          | 0              | Número de veces para replicar el patrón de barrido verticalmente. Válido solo para valores de **ID** de 101 a 131. |
| ScaleX          | double        | 1.0            | Cantidad para estirar el barrido horizontalmente, como porcentaje de la definición original del barrido. Válido solo para valores de **ID** de 101 a 131. |
| ScaleY          | double        | 1.0            | Cantidad para estirar el barrido verticalmente, como porcentaje de la definición original del barrido. Válido solo para valores de **ID** de 101 a 131. |

## SMPTE

Esta transición soporta los siguientes barridos estándar SMPTE:

| Número | Descripción                       | Número | Descripción                                    |
|--------|-----------------------------------|--------|------------------------------------------------|
| 1      | Horizontal                        | 211    | Radial, izquierda-derecha, arriba              |
| 2      | Vertical                          | 212    | Radial, arriba-abajo, derecha                  |
| 3      | Superior izquierdo                | 213    | Radial, izquierda-derecha, arriba-abajo        |
| 4      | Superior derecho                  | 214    | Radial, arriba-abajo, izquierda-derecha        |
| 5      | Inferior derecho                  | 221    | Radial, arriba                                 |
| 6      | Inferior izquierdo                | 222    | Radial, derecha                                |
| 7      | Cuatro esquinas                   | 223    | Radial, abajo                                  |
| 8      | Cuatro cuadrados                  | 224    | Radial, izquierda                              |
| 21     | Puertas de granero, vertical      | 225    | Radial, arriba horario, abajo horario          |
| 22     | Puertas de granero, horizontal    | 226    | Radial, izquierda horario, derecha horario     |
| 23     | Centro superior                   | 227    | Radial, arriba horario, abajo antihorario      |
| 24     | Centro derecho                    | 228    | Radial, izquierda horario, derecha antihorario |
| 25     | Centro inferior                   | 231    | Radial, división arriba                        |
| 26     | Centro izquierdo                  | 232    | Radial, división derecha                       |
| 41     | Diagonal, NO a SE                 | 233    | Radial, división abajo                         |
| 42     | Diagonal, NE a SO                 | 234    | Radial, división izquierda                     |
| 43     | Triángulos, arriba/abajo          | 235    | Radial, división arriba-abajo                  |
| 44     | Triángulos, izquierda/derecha     | 236    | Radial, división izquierda-derecha             |
| 45     | Franja diagonal, SO a NE          | 241    | Radial, esquina superior-izquierda             |
| 46     | Franja diagonal, NO a SE          | 242    | Radial, esquina inferior-izquierda             |
| 47     | Cruz                              | 243    | Radial, esquina inferior-derecha               |
| 48     | Caja de diamante                  | 244    | Radial, esquina superior-derecha               |
| 61     | Cuña, arriba                      | 245    | Radial, superior-izq, inferior-der             |
| 62     | Cuña, derecha                     | 246    | Radial, inferior-izq, superior-der             |
| 63     | Cuña, abajo                       | 251    | Radial centro, arriba                          |
| 64     | Cuña, izquierda                   | 252    | Radial centro, izquierda                       |
| 65     | V                                 | 253    | Radial centro, abajo                           |
| 66     | V, derecha                        | 254    | Radial centro, derecha                         |
| 67     | V, invertida                      | 261    | Caja radial, derecha                           |
| 68     | V, izquierda                      | 262    | Caja radial, arriba                            |
| 71     | Diente de sierra, izquierda       | 263    | Radial centro, arriba, abajo                   |
| 72     | Diente de sierra, arriba          | 264    | Radial centro, izquierda, derecha              |
| 73     | Diente de sierra, vertical        | 301    | Matriz, horizontal                             |
| 74     | Diente de sierra, horizontal      | 302    | Matriz, vertical                               |
| 101    | Caja                              | 303    | Matriz, diagonal, superior-izquierda           |
| 102    | Diamante                          | 304    | Matriz, diagonal, superior-derecha             |
| 103    | Triángulo, arriba                 | 305    | Matriz, diagonal, inferior-derecha             |
| 104    | Triángulo, derecha                | 306    | Matriz, diagonal, inferior-izquierda           |
| 105    | Triángulo, abajo                  | 310    | Matriz, horario superior-izquierda             |
| 106    | Triángulo, izquierda              | 311    | Matriz, horario superior-derecha               |
| 107    | Punta de flecha, arriba           | 312    | Matriz, horario inferior-derecha               |
| 108    | Punta de flecha, derecha          | 313    | Matriz, horario inferior-izquierda             |
| 109    | Punta de flecha, abajo            | 314    | Matriz, antihorario superior-izquierda         |
| 110    | Punta de flecha, izquierda        | 315    | Matriz, antihorario superior-derecha           |
| 111    | Pentágono, arriba                 | 316    | Matriz, antihorario inferior-derecha           |
| 112    | Pentágono, abajo                  | 317    | Matriz, antihorario inferior-izquierda         |
| 113    | Hexágono                          | 320    | Matriz, vertical sup-izq, sup-der              |
| 114    | Hexágono, rotado                  | 321    | Matriz, vertical inf-izq, inf-der              |
| 119    | Círculo                           | 322    | Matriz, vertical sup-izq, inf-der              |
| 120    | Óvalo, horizontal                 | 323    | Matriz, vertical inf-izq, sup-der              |
| 121    | Óvalo, vertical                   | 324    | Matriz, horizontal sup-izq, inf-izq            |
| 122    | Ojo, horizontal                   | 325    | Matriz, horizontal sup-der, inf-der            |
| 123    | Ojo, vertical                     | 326    | Matriz, horizontal sup-izq, inf-der            |
| 124    | Rectángulo redondeado, horizontal | 327    | Matriz, horizontal sup-der, inf-izq            |
| 125    | Rectángulo redondeado, vertical   | 328    | Matriz, diagonal inf-izq, sup-der              |
| 127    | Estrella de 4 puntas              | 329    | Matriz, diagonal sup-izq, inf-der              |
| 128    | Estrella de 4 puntas              | 340    | Matriz, espiral doble arriba                   |
| 129    | Estrella de 6 puntas              | 341    | Matriz, espiral doble abajo                    |
| 130    | Corazón                           | 342    | Matriz, espiral doble izquierda                |
| 131    | Cerradura                         | 343    | Matriz, espiral doble derecha                  |
| 201    | Radial, 12 en punto               | 344    | Matriz, cuádruple espiral, arriba-abajo        |
| 202    | Radial, 3 en punto                | 345    | Matriz, cuádruple espiral, izquierda-derecha   |
| 203    | Radial, 6 en punto                | 350    | Cascada, izquierda                             |
| 204    | Radial, 9 en punto                | 351    | Cascada, derecha                               |
| 205    | Radial, 12 + 6 en punto           | 352    | Cascada, horizontal, izquierda                 |
| 206    | Radial, 3 + 9 en punto            | 353    | Cascada, horizontal, derecha                   |
| 207    | Radial, 4 vías                    | 409    | Máscara aleatoria                              |
  
---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.