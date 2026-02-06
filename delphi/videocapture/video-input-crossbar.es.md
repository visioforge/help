---
title: Selección de Entrada de Video con Crossbar en Delphi
description: Seleccionar fuentes de entrada de video en Delphi con crossbar - configurar entradas compuesto, S-Video, HDMI con ejemplos de código paso a paso para Delphi.
---

# Seleccionando Fuentes de Entrada de Video con Tecnología Crossbar

## Introducción a la Selección de Entrada de Video

Al desarrollar aplicaciones que capturan video de dispositivos externos, a menudo necesitará manejar múltiples fuentes de entrada. El crossbar es un componente crucial en sistemas de captura de video que le permite enrutar diferentes entradas físicas (como compuesto, S-Video, HDMI) a su aplicación. Esta guía lo lleva a través del proceso de detectar, configurar y seleccionar entradas de video usando la interfaz crossbar en aplicaciones Delphi, C++ MFC y Visual Basic 6.

## Entendiendo la Tecnología Crossbar

La tecnología crossbar funciona como una matriz de enrutamiento en dispositivos de captura de video, habilitando la conexión entre varias entradas y salidas. Las tarjetas de captura modernas y los sintonizadores de TV frecuentemente incorporan funcionalidad crossbar para facilitar el cambio entre diferentes fuentes de video como:

- Entradas de video compuesto
- Conexiones S-Video
- Video por componentes
- Entradas HDMI
- Entradas de sintonizador de TV
- Interfaces de video digital

Configurar correctamente estas conexiones programáticamente es esencial para aplicaciones que necesitan cambiar dinámicamente entre diferentes fuentes de video.

## Resumen de Pasos de Implementación

El proceso de implementación para configurar conexiones crossbar en su aplicación involucra tres pasos principales:

1. Inicializar la interfaz crossbar y verificar su disponibilidad
2. Enumerar entradas de video disponibles para selección
3. Conectar la entrada seleccionada a la salida del decodificador de video

Examinemos cada paso en detalle con código de ejemplo para entornos Delphi, C++ MFC y VB6.

## Guía de Implementación Detallada

### Paso 1: Inicializar la Interfaz Crossbar

Antes de que pueda trabajar con la selección de entrada, necesita inicializar la interfaz crossbar y verificar que esté disponible en el dispositivo de captura actual.

#### Implementación en Delphi

```pascal
// Inicializar la interfaz crossbar
CrossBarFound := VideoCapture1.Video_CaptureDevice_CrossBar_Init;

// Verificar si la funcionalidad crossbar está disponible
if CrossBarFound then
  ShowMessage('Funcionalidad crossbar detectada e inicializada')
else
  ShowMessage('No hay crossbar disponible en este dispositivo de captura');
```

#### Implementación en C++ MFC

```cpp
// Inicializar la interfaz crossbar
BOOL bCrossBarFound = m_videoCapture.Video_CaptureDevice_CrossBar_Init();

// Verificar si la funcionalidad crossbar está disponible
if (bCrossBarFound) {
    AfxMessageBox(_T("Funcionalidad crossbar detectada e inicializada"));
} else {
    AfxMessageBox(_T("No hay crossbar disponible en este dispositivo de captura"));
}
```

#### Implementación en VB6

```vb
' Inicializar la interfaz crossbar
Dim CrossBarFound As Boolean
CrossBarFound = VideoCapture1.Video_CaptureDevice_CrossBar_Init()

' Verificar si la funcionalidad crossbar está disponible
If CrossBarFound Then
    MsgBox "Funcionalidad crossbar detectada e inicializada"
Else
    MsgBox "No hay crossbar disponible en este dispositivo de captura"
End If
```

La función de inicialización devuelve un valor booleano indicando si la funcionalidad crossbar está disponible en el dispositivo de captura actual. No todos los dispositivos de captura soportan funcionalidad crossbar, por lo que esta verificación es crucial.

### Paso 2: Enumerar Entradas de Video Disponibles

Una vez que haya confirmado que el crossbar está disponible, el siguiente paso es recuperar una lista de entradas disponibles para la salida "Video Decoder". Esto permite a los usuarios seleccionar de las conexiones físicas disponibles.

#### Implementación en Delphi

```pascal
// Limpiar cualquier conexión existente y elementos de UI
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections;
cbCrossbarVideoInput.Clear;

// Obtener conteo de entradas disponibles para la salida "Video Decoder"
var inputCount: Integer := VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount('Video Decoder');

// Poblar UI con entradas disponibles
for i := 0 to inputCount - 1 do begin
  var inputName: String := VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem('Video Decoder', i);
  cbCrossbarVideoInput.Items.Add(inputName);
end;

// Seleccionar el primer elemento por defecto si está disponible
if cbCrossbarVideoInput.Items.Count > 0 then
  cbCrossbarVideoInput.ItemIndex := 0;
```

#### Implementación en C++ MFC

```cpp
// Limpiar cualquier conexión existente y elementos de UI
m_videoCapture.Video_CaptureDevice_CrossBar_ClearConnections();
m_comboVideoInputs.ResetContent();

// Obtener conteo de entradas disponibles para la salida "Video Decoder"
int inputCount = m_videoCapture.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount(_T("Video Decoder"));

// Poblar UI con entradas disponibles
for (int i = 0; i < inputCount; i++) {
    CString inputName = m_videoCapture.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem(_T("Video Decoder"), i);
    m_comboVideoInputs.AddString(inputName);
}

// Seleccionar el primer elemento por defecto si está disponible
if (m_comboVideoInputs.GetCount() > 0) {
    m_comboVideoInputs.SetCurSel(0);
}
```

#### Implementación en VB6

```vb
' Limpiar cualquier conexión existente y elementos de UI
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections
cboVideoInputs.Clear

' Obtener conteo de entradas disponibles para la salida "Video Decoder"
Dim inputCount As Integer
inputCount = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetCount("Video Decoder")

' Poblar UI con entradas disponibles
Dim i As Integer
Dim inputName As String
For i = 0 To inputCount - 1
    inputName = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput_GetItem("Video Decoder", i)
    cboVideoInputs.AddItem inputName
Next i

' Seleccionar el primer elemento por defecto si está disponible
If cboVideoInputs.ListCount > 0 Then
    cboVideoInputs.ListIndex = 0
End If
```

Los tipos de entrada comunes que podría encontrar incluyen:

- Compuesto
- S-Video
- HDMI
- Componentes
- Sintonizador de TV

La lista exacta depende de las capacidades de su hardware de captura específico.

### Paso 3: Aplicar la Entrada Seleccionada

Después de que el usuario seleccione su fuente de entrada deseada, necesita aplicar esta selección estableciendo una conexión entre la entrada seleccionada y la salida del decodificador de video.

#### Implementación en Delphi

```pascal
// Primero limpiar cualquier conexión existente
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections;

// Conectar la entrada seleccionada a la salida "Video Decoder"
// Parámetros: nombre de entrada, nombre de salida, enrutamiento automático de señal
if cbCrossbarVideoInput.ItemIndex >= 0 then begin
  var selectedInput: String := cbCrossbarVideoInput.Items[cbCrossbarVideoInput.ItemIndex];
  var success: Boolean := VideoCapture1.Video_CaptureDevice_CrossBar_Connect(selectedInput, 'Video Decoder', true);
  
  if success then
    ShowMessage('Conectado exitosamente ' + selectedInput + ' a Video Decoder')
  else
    ShowMessage('Error al establecer conexión');
end;
```

#### Implementación en C++ MFC

```cpp
// Primero limpiar cualquier conexión existente
m_videoCapture.Video_CaptureDevice_CrossBar_ClearConnections();

// Conectar la entrada seleccionada a la salida "Video Decoder"
// Parámetros: nombre de entrada, nombre de salida, enrutamiento automático de señal
int selectedIndex = m_comboVideoInputs.GetCurSel();
if (selectedIndex >= 0) {
    CString selectedInput;
    m_comboVideoInputs.GetLBText(selectedIndex, selectedInput);
    
    BOOL success = m_videoCapture.Video_CaptureDevice_CrossBar_Connect(
        selectedInput, _T("Video Decoder"), TRUE);
    
    if (success) {
        CString msg;
        msg.Format(_T("Conectado exitosamente %s a Video Decoder"), selectedInput);
        AfxMessageBox(msg);
    } else {
        AfxMessageBox(_T("Error al establecer conexión"));
    }
}
```

#### Implementación en VB6

```vb
' Primero limpiar cualquier conexión existente
VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections

' Conectar la entrada seleccionada a la salida "Video Decoder"
' Parámetros: nombre de entrada, nombre de salida, enrutamiento automático de señal
If cboVideoInputs.ListIndex >= 0 Then
    Dim selectedInput As String
    selectedInput = cboVideoInputs.Text
    
    Dim success As Boolean
    success = VideoCapture1.Video_CaptureDevice_CrossBar_Connect(selectedInput, "Video Decoder", True)
    
    If success Then
        MsgBox "Conectado exitosamente " & selectedInput & " a Video Decoder"
    Else
        MsgBox "Error al establecer conexión"
    End If
End If
```

El tercer parámetro (`true`) habilita el enrutamiento automático de señal, lo que ayuda a manejar escenarios de conexión complejos donde podría requerirse enrutamiento intermedio.

## Mejores Prácticas para Implementación de Crossbar

Para una selección robusta de entrada de video en sus aplicaciones:

1. **Siempre inicialice el crossbar primero**: Verifique la disponibilidad antes de intentar operaciones
2. **Limpie conexiones existentes**: Antes de establecer una nueva conexión, limpie cualquier existente
3. **Maneje la ausencia de crossbar graciosamente**: Proporcione opciones alternativas cuando la funcionalidad crossbar no esté disponible
4. **Valide selecciones**: Asegure que una entrada válida esté seleccionada antes de intentar establecer conexiones
5. **Proporcione retroalimentación al usuario**: Informe a los usuarios sobre intentos de conexión exitosos o fallidos

## Solución de Problemas Comunes

Si encuentra problemas con conexiones crossbar:

- Verifique que su dispositivo de captura soporte funcionalidad crossbar
- Verifique que los nombres de entrada y salida coincidan exactamente con lo que el dispositivo reporta
- Asegure la instalación correcta del controlador del dispositivo
- Use registro de depuración para rastrear intentos de conexión
- Pruebe con diferentes fuentes de entrada para aislar problemas específicos de hardware

## Conclusión

La implementación adecuada de tecnología crossbar en sus aplicaciones de captura de video da a los usuarios la flexibilidad de trabajar con múltiples fuentes de entrada. Siguiendo los pasos descritos en esta guía, puede crear un sistema de selección de entrada de video robusto y fácil de usar para sus aplicaciones, independientemente de si está desarrollando en Delphi, C++ MFC o Visual Basic 6.

Los ejemplos de código proporcionados demuestran cómo inicializar el crossbar, enumerar entradas disponibles y conectar entradas seleccionadas a la salida del decodificador de video. Con estos fundamentos en su lugar, puede construir aplicaciones de captura de video sofisticadas que soporten una amplia gama de dispositivos de entrada y tipos de conexión.

---
Para asistencia adicional con la implementación de esta funcionalidad, explore nuestras otras páginas de documentación y repositorio de ejemplos de código para técnicas más avanzadas y soluciones.