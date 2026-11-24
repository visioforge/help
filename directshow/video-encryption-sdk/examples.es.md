---
title: Encriptar video con DirectShow - Ejemplos C#, C++ y Delphi
description: Ejemplos de código para encriptación y desencriptación de video con filtros DirectShow en C++, C# y Delphi usando contraseñas y claves binarias.
tags:
  - Video Encryption SDK
  - DirectShow
  - C++
  - Windows
  - Encoding
  - MP4
  - H.264
  - AAC
  - C#
primary_api_classes:
  - IBaseFilter
  - IVFCryptoConfig
  - IFileSinkFilter
  - SourceFilter
  - VideoEncoder

---

# Video Encryption SDK - Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos de código completos y funcionales para encriptar y desencriptar archivos de video utilizando el Video Encryption SDK. Los ejemplos cubren:

- **Encriptación Básica** - Encriptar archivos de video con protección por contraseña
- **Desencriptación de Archivos** - Desencriptar y reproducir archivos de video encriptados
- **Escenarios Avanzados** - Claves binarias, claves basadas en archivos, configuraciones de codificación personalizadas
- **Manejo de Errores** - Comprobación de errores robusta y recuperación

Todos los ejemplos incluyen implementaciones completas en C++, C# y Delphi.

!!! note "Nombres de la interfaz entre wrappers de lenguaje"
    El header nativo de C++ (`encryptor_intf.h`) declara esta interfaz como `ICryptoConfig` con `IID_ICryptoConfig`. Los wrappers de C# y Delphi exponen la misma interfaz como `IVFCryptoConfig` (y el proveedor de contraseña como `IVFPasswordProvider`). Ambos nombres comparten el GUID `{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}` y se refieren a la misma interfaz COM — los fragmentos en C++ usan `ICryptoConfig` mientras que los de C#/Delphi usan `IVFCryptoConfig`.

---
## Requisitos Previos
### Proyectos C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "encryptor_intf.h"
#pragma comment(lib, "strmiids.lib")
#pragma comment(lib, "quartz.lib")
```
### Proyectos C\#
```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
```
**Paquetes NuGet Requeridos**:
- VisioForge.DirectShowAPI (también contiene el espacio de nombres `VisioForge.DirectShowLib`)
### Proyectos Delphi
```delphi
uses
  ENCDirectShow9,
  encryptor_intf;
```
---

## Ejemplo 1: Encriptación de Video Básica

Encriptar un archivo de video con contraseña de texto.

### Implementación en C\#

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class BasicVideoEncryption
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IMediaEventEx mediaEvent;

    public void EncryptVideo(string inputFile, string outputFile, string password)
    {
        try
        {
            // Crear grafo de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            mediaEvent = (IMediaEventEx)filterGraph;

            // Añadir filtro fuente
            int hr = filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
            DsError.ThrowExceptionForHR(hr);

            // Añadir codificador de video (H.264)
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
                "H.264 Encoder"
            );

            // Añadir codificador de audio (AAC)
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
                "AAC Encoder"
            );

            // Añadir muxer de encriptación
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );

            hr = filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");
            DsError.ThrowExceptionForHR(hr);

            // Configurar contraseña de encriptación
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Aplicar contraseña usando método auxiliar
                cryptoConfig.ApplyString(password);

                // Verificar que la contraseña esté establecida
                hr = cryptoConfig.HavePassword();
                if (hr != 0)
                {
                    throw new Exception("Error al establecer la contraseña de encriptación");
                }

                Console.WriteLine("Contraseña de encriptación configurada exitosamente");
            }
            else
            {
                throw new Exception("Interfaz IVFCryptoConfig no disponible");
            }

            // Establecer archivo de salida
            var fileWriter = (IBaseFilter)new FileWriter();
            hr = filterGraph.AddFilter(fileWriter, "File Writer");
            DsError.ThrowExceptionForHR(hr);
            hr = ((IFileSinkFilter)fileWriter).SetFileName(outputFile, null);
            DsError.ThrowExceptionForHR(hr);

            // Construir conexiones del grafo de filtros
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Conectar ruta de video: Fuente → Codificador H.264 → Muxer de Encriptación
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                sourceFilter,
                videoEncoder,
                encryptMuxer
            );
            DsError.ThrowExceptionForHR(hr);

            // Conectar ruta de audio: Fuente → Codificador AAC → Muxer de Encriptación
            hr = captureGraph.RenderStream(
                null,
                MediaType.Audio,
                sourceFilter,
                audioEncoder,
                encryptMuxer
            );
            // Sin comprobar: un archivo sin audio no tiene nada que conectar aquí

            hr = captureGraph.RenderStream(null, null, encryptMuxer, null, fileWriter);
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine("Grafo de filtros construido exitosamente");
            Console.WriteLine("Iniciando encriptación...");

            // Iniciar codificación
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            // Esperar finalización
            EventCode eventCode;
            do
            {
                hr = mediaEvent.WaitForCompletion(1000, out eventCode);
            }
            while (hr == DSErrorCodes.E_ABORT);
            DsError.ThrowExceptionForHR(hr);
            if (eventCode != EventCode.Complete)
                throw new InvalidOperationException($"Encoding stopped with event {eventCode}");

            Console.WriteLine("¡Encriptación completada exitosamente!");

            // Limpieza
            Marshal.ReleaseComObject(captureGraph);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            throw;
        }
        finally
        {
            Stop();
        }
    }

    public void Stop()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }

        if (filterGraph != null)
        {
            FilterGraphTools.RemoveAllFilters(filterGraph);
        }

        if (mediaEvent != null) Marshal.ReleaseComObject(mediaEvent);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}

// Uso:
// var encryptor = new BasicVideoEncryption();
// encryptor.EncryptVideo(@"C:\input.mp4", @"C:\output.encrypted.mp4", "MiContraseñaSegura123");
```

### Implementación en C++

```cpp
#include <dshow.h>
#include <streams.h>
#include "encryptor_intf.h"

// Version 9 encryptor (AES-256); encryptor_intf.h names only the version 8 one
DEFINE_GUID(CLSID_EncryptMuxer9,
    0x86d83d2e, 0x70d2, 0x4a32, 0xb7, 0xbe, 0xe9, 0x4a, 0x08, 0xaa, 0x7b, 0xaa);

// VisioForge H264 Encoder v10 and AAC Encoder v10
DEFINE_GUID(CLSID_VFH264EncoderV10,
    0xea1fed6b, 0xb876, 0x4db0, 0xb7, 0xb1, 0x77, 0x84, 0x63, 0xe5, 0x99, 0x78);
DEFINE_GUID(CLSID_VFAACEncoderV10,
    0x763cac70, 0x373c, 0x4892, 0x89, 0x8b, 0xac, 0x80, 0x66, 0x1b, 0x15, 0xf3);

class BasicVideoEncryption
{
private:
    IGraphBuilder* pGraph;
    IMediaControl* pControl;
    IMediaEvent* pEvent;

public:
    BasicVideoEncryption() : pGraph(NULL), pControl(NULL), pEvent(NULL) {}

    HRESULT EncryptVideo(LPCWSTR inputFile, LPCWSTR outputFile, LPCWSTR password)
    {
        HRESULT hr;
        IBaseFilter* pSource = NULL;
        IBaseFilter* pVideoEncoder = NULL;
        IBaseFilter* pAudioEncoder = NULL;
        IBaseFilter* pMuxer = NULL;
        ICryptoConfig* pCrypto = NULL;
        IBaseFilter* pFileWriter = NULL;
        IFileSinkFilter* pFileSink = NULL;
        ICaptureGraphBuilder2* pBuilder = NULL;

        // Crear grafo de filtros
        hr = CoCreateInstance(
            CLSID_FilterGraph,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IGraphBuilder,
            (void**)&pGraph
        );
        if (FAILED(hr)) return hr;

        // Obtener interfaces de control y eventos
        pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
        pGraph->QueryInterface(IID_IMediaEvent, (void**)&pEvent);

        // Añadir filtro fuente
        hr = pGraph->AddSourceFilter(inputFile, L"Source", &pSource);
        if (FAILED(hr)) goto cleanup;

        // Añadir codificador de video (H.264)
        hr = CoCreateInstance(
            CLSID_VFH264EncoderV10,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IBaseFilter,
            (void**)&pVideoEncoder
        );
        if (FAILED(hr)) goto cleanup;
        hr = pGraph->AddFilter(pVideoEncoder, L"H.264 Encoder");
        if (FAILED(hr)) goto cleanup;

        // Añadir codificador de audio (AAC)
        hr = CoCreateInstance(
            CLSID_VFAACEncoderV10,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IBaseFilter,
            (void**)&pAudioEncoder
        );
        if (FAILED(hr)) goto cleanup;
        hr = pGraph->AddFilter(pAudioEncoder, L"AAC Encoder");
        if (FAILED(hr)) goto cleanup;

        // Añadir muxer de encriptación
        hr = CoCreateInstance(
            CLSID_EncryptMuxer9,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IBaseFilter,
            (void**)&pMuxer
        );
        if (FAILED(hr)) goto cleanup;
        hr = pGraph->AddFilter(pMuxer, L"Encrypt Muxer");
        if (FAILED(hr)) goto cleanup;

        // Configurar encriptación
        hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
        if (SUCCEEDED(hr))
        {
            // Establecer contraseña
            hr = pCrypto->put_Password(
                (LPBYTE)password,
                wcslen(password) * sizeof(wchar_t)
            );

            if (SUCCEEDED(hr))
            {
                // Verificar que la contraseña esté establecida
                HRESULT hrPassword = pCrypto->HavePassword();
                if (hrPassword == S_OK)
                {
                    wprintf(L"Contraseña de encriptación configurada exitosamente\n");
                }
                else
                {
                    wprintf(L"ADVERTENCIA: Contraseña no establecida\n");
                    hr = E_FAIL;
                }
            }

            pCrypto->Release();
        }
        if (FAILED(hr)) goto cleanup;

        // Establecer archivo de salida
        hr = CoCreateInstance(
            CLSID_FileWriter,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IBaseFilter,
            (void**)&pFileWriter
        );
        if (FAILED(hr)) goto cleanup;
        hr = pGraph->AddFilter(pFileWriter, L"File Writer");
        if (FAILED(hr)) goto cleanup;

        hr = pFileWriter->QueryInterface(IID_IFileSinkFilter, (void**)&pFileSink);
        if (SUCCEEDED(hr))
        {
            hr = pFileSink->SetFileName(outputFile, NULL);
            pFileSink->Release();
        }
        if (FAILED(hr)) goto cleanup;

        // Construir grafo usando Intelligent Connect
        hr = CoCreateInstance(
            CLSID_CaptureGraphBuilder2,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_ICaptureGraphBuilder2,
            (void**)&pBuilder
        );
        if (SUCCEEDED(hr))
        {
            hr = pBuilder->SetFiltergraph(pGraph);

            // Conectar ruta de video
            if (SUCCEEDED(hr)) hr = pBuilder->RenderStream(
                NULL,
                &MEDIATYPE_Video,
                pSource,
                pVideoEncoder,
                pMuxer
            );

            // Conectar ruta de audio
            // Sin comprobar: un archivo sin audio no tiene nada que conectar aquí
            pBuilder->RenderStream(
                NULL,
                &MEDIATYPE_Audio,
                pSource,
                pAudioEncoder,
                pMuxer
            );

            if (SUCCEEDED(hr)) hr = pBuilder->RenderStream(NULL, NULL, pMuxer, NULL, pFileWriter);

            pBuilder->Release();
        }
        if (FAILED(hr)) goto cleanup;

        wprintf(L"Iniciando encriptación...\n");

        // Ejecutar el grafo
        hr = pControl->Run();
        if (FAILED(hr)) goto cleanup;

        // Esperar finalización
        long evCode;
        hr = pEvent->WaitForCompletion(INFINITE, &evCode);
        if (FAILED(hr)) goto cleanup;
        if (evCode != EC_COMPLETE)
        {
            hr = E_FAIL;
            goto cleanup;
        }

        wprintf(L"¡Encriptación completada exitosamente!\n");

cleanup:
        if (pFileWriter) pFileWriter->Release();
        if (pMuxer) pMuxer->Release();
        if (pVideoEncoder) pVideoEncoder->Release();
        if (pAudioEncoder) pAudioEncoder->Release();
        if (pSource) pSource->Release();

        Stop();

        return hr;
    }

    void Stop()
    {
        if (pControl)
        {
            pControl->Stop();
        }

        if (pEvent) pEvent->Release();
        if (pControl) pControl->Release();
        if (pGraph) pGraph->Release();

        pEvent = NULL;
        pControl = NULL;
        pGraph = NULL;
    }
};

// Uso:
// BasicVideoEncryption encryptor;
// encryptor.EncryptVideo(L"C:\\input.mp4", L"C:\\output.encrypted.mp4", L"MiContraseñaSegura123");
```

### Implementación en Delphi

```delphi
uses
  Windows,
  SysUtils,
  ENCDirectShow9,
  ActiveX,
  encryptor_intf;

type
  TBasicVideoEncryption = class
  private
    FFilterGraph: IGraphBuilder;
    FMediaControl: IMediaControl;
    FMediaEvent: IMediaEvent;
  public
    function EncryptVideo(const InputFile, OutputFile, Password: WideString): HRESULT;
    procedure Stop;
  end;

const
  CLSID_VFEncryptor9: TGUID = '{86D83D2E-70D2-4A32-B7BE-E94A08AA7BAA}';

function TBasicVideoEncryption.EncryptVideo(const InputFile, OutputFile, Password: WideString): HRESULT;
var
  SourceFilter: IBaseFilter;
  VideoEncoder: IBaseFilter;
  AudioEncoder: IBaseFilter;
  EncryptMuxer: IBaseFilter;
  FileWriter: IBaseFilter;
  CryptoConfig: IVFCryptoConfig;
  FileSink: IFileSinkFilter;
  CaptureGraph: ICaptureGraphBuilder2;
  EventCode: Integer;
begin
  Result := E_FAIL;

  try
    // Crear grafo de filtros
    Result := CoCreateInstance(CLSID_FilterGraph, nil, CLSCTX_INPROC_SERVER,
                               IID_IGraphBuilder, FFilterGraph);
    if Failed(Result) then Exit;

    // Obtener interfaces de control y eventos
    FFilterGraph.QueryInterface(IID_IMediaControl, FMediaControl);
    FFilterGraph.QueryInterface(IID_IMediaEvent, FMediaEvent);

    // Añadir filtro fuente
    Result := FFilterGraph.AddSourceFilter(PWideChar(InputFile), 'Source', SourceFilter);
    if Failed(Result) then Exit;

    // Añadir codificador de video (H.264)
    Result := CoCreateInstance(CLSID_VFH264EncoderV10, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, VideoEncoder);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(VideoEncoder, 'H.264 Encoder');
    if Failed(Result) then Exit;

    // Añadir codificador de audio (AAC)
    Result := CoCreateInstance(CLSID_VFAACEncoderV10, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, AudioEncoder);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(AudioEncoder, 'AAC Encoder');
    if Failed(Result) then Exit;

    // Añadir muxer de encriptación
    Result := CoCreateInstance(CLSID_VFEncryptor9, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, EncryptMuxer);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(EncryptMuxer, 'Encrypt Muxer');
    if Failed(Result) then Exit;

    // Configurar encriptación
    Result := E_NOINTERFACE;
    if Supports(EncryptMuxer, IVFCryptoConfig, CryptoConfig) then
    begin
      // Establecer contraseña
      // pBuffer es un buffer binario opaco (PByte); convierta el puntero
      // a través de PWideChar para reutilizar el buffer de cadena ancha;
      // la longitud en bytes = cantidad de caracteres * 2.
      Result := CryptoConfig.put_Password(PByte(PWideChar(Password)), Length(Password) * 2);

      if Succeeded(Result) then
      begin
        // Verificar que la contraseña esté establecida
        if CryptoConfig.HavePassword = S_OK then
          WriteLn('Contraseña de encriptación configurada exitosamente')
        else
        begin
          WriteLn('ADVERTENCIA: Contraseña no establecida');
          Result := E_FAIL;
        end;
      end;
    end;
    if Failed(Result) then Exit;

    // Establecer archivo de salida
    Result := CoCreateInstance(CLSID_FileWriter, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, FileWriter);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(FileWriter, 'File Writer');
    if Failed(Result) then Exit;
    Result := E_NOINTERFACE;
    if not Supports(FileWriter, IFileSinkFilter, FileSink) then Exit;
    Result := FileSink.SetFileName(PWideChar(OutputFile), nil);
    if Failed(Result) then Exit;

    // Construir conexiones del grafo
    Result := CoCreateInstance(CLSID_CaptureGraphBuilder2, nil, CLSCTX_INPROC_SERVER,
                               IID_ICaptureGraphBuilder2, CaptureGraph);
    if Succeeded(Result) then
    begin
      Result := CaptureGraph.SetFiltergraph(FFilterGraph);

      // Conectar ruta de video
      if Succeeded(Result) then
        Result := CaptureGraph.RenderStream(nil, @MEDIATYPE_Video, SourceFilter,
                                            VideoEncoder, EncryptMuxer);

      // Conectar ruta de audio
      // Sin comprobar: un archivo sin audio no tiene nada que conectar aquí
      CaptureGraph.RenderStream(nil, @MEDIATYPE_Audio, SourceFilter,
                                AudioEncoder, EncryptMuxer);

      if Succeeded(Result) then
        Result := CaptureGraph.RenderStream(nil, nil, EncryptMuxer, nil, FileWriter);
    end;
    if Failed(Result) then Exit;

    WriteLn('Iniciando encriptación...');

    // Ejecutar el grafo
    Result := FMediaControl.Run;
    if Failed(Result) then Exit;

    // Esperar finalización
    repeat
      Result := FMediaEvent.WaitForCompletion(1000, EventCode);
    until Result <> E_ABORT;
    if Failed(Result) then Exit;
    if EventCode <> EC_COMPLETE then
    begin
      Result := E_FAIL;
      Exit;
    end;

    WriteLn('¡Encriptación completada exitosamente!');

  finally
    Stop;
  end;
end;

procedure TBasicVideoEncryption.Stop;
begin
  if Assigned(FMediaControl) then
    FMediaControl.Stop;

  FMediaEvent := nil;
  FMediaControl := nil;
  FFilterGraph := nil;
end;

// Uso:
// var
//   Encryptor: TBasicVideoEncryption;
// begin
//   Encryptor := TBasicVideoEncryption.Create;
//   try
//     Encryptor.EncryptVideo('C:\input.mp4', 'C:\output.encrypted.mp4', 'MiContraseñaSegura123');
//   finally
//     Encryptor.Free;
//   end;
// end;
```

---
## Ejemplo 2: Desencriptación y Reproducción de Video
Desencriptar un archivo de video encriptado y reproducirlo.
### Implementación en C\#
```csharp
public class VideoDecryption
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IMediaEventEx mediaEvent;
    private IVideoWindow videoWindow;
    public void DecryptAndPlay(string encryptedFile, string password, IntPtr windowHandle)
    {
        try
        {
            // Crear grafo de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            mediaEvent = (IMediaEventEx)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;
            // Añadir filtro demuxer de desencriptación
            var decryptDemuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoDecryptor9))
            );
            int hr = filterGraph.AddFilter(decryptDemuxer, "Decrypt Demuxer");
            DsError.ThrowExceptionForHR(hr);
            // Configurar desencriptación (DEBE usar la misma contraseña que la encriptación)
            var cryptoConfig = decryptDemuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Aplicar contraseña
                cryptoConfig.ApplyString(password);
                // Verificar que la contraseña esté establecida
                hr = cryptoConfig.HavePassword();
                if (hr != 0)
                {
                    throw new Exception("Error al establecer la contraseña de desencriptación");
                }
                Console.WriteLine("Contraseña de desencriptación configurada exitosamente");
            }
            else
            {
                throw new Exception("Interfaz IVFCryptoConfig no disponible");
            }
            // Cargar archivo encriptado
            var fileSourceFilter = (IBaseFilter)new AsyncReader();
            hr = filterGraph.AddFilter(fileSourceFilter, "File Source");
            DsError.ThrowExceptionForHR(hr);
            hr = ((IFileSourceFilter)fileSourceFilter).Load(encryptedFile, null);
            DsError.ThrowExceptionForHR(hr);
            // Construir grafo - renderizar salidas de video y audio
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);
            hr = captureGraph.RenderStream(null, null, fileSourceFilter, null, decryptDemuxer);
            DsError.ThrowExceptionForHR(hr);
            // Renderizar salida de video
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                decryptDemuxer,
                null,
                null
            );
            DsError.ThrowExceptionForHR(hr);
            // Renderizar salida de audio
            hr = captureGraph.RenderStream(
                null,
                MediaType.Audio,
                decryptDemuxer,
                null,
                null
            );
            // El audio es opcional, no fallar si no está presente
            // Configurar ventana de video
            hr = videoWindow.put_Owner(windowHandle);
            hr = videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            hr = videoWindow.put_MessageDrain(windowHandle);
            Console.WriteLine("Iniciando reproducción...");
            // Iniciar reproducción
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
            Console.WriteLine("¡Desencriptación y reproducción iniciadas exitosamente!");
            Marshal.ReleaseComObject(captureGraph);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            throw;
        }
    }
    public void Stop()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }
        if (videoWindow != null)
        {
            videoWindow.put_Visible(OABool.False);
            videoWindow.put_Owner(IntPtr.Zero);
        }
        if (filterGraph != null)
        {
            FilterGraphTools.RemoveAllFilters(filterGraph);
        }
        if (videoWindow != null) Marshal.ReleaseComObject(videoWindow);
        if (mediaEvent != null) Marshal.ReleaseComObject(mediaEvent);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
    public void SetVideoWindowSize(int width, int height)
    {
        if (videoWindow != null)
        {
            videoWindow.SetWindowPosition(0, 0, width, height);
        }
    }
}
// Uso:
// var decryptor = new VideoDecryption();
// decryptor.DecryptAndPlay(@"C:\output.encrypted.mp4", "MiContraseñaSegura123", this.Handle);
```
---

## Ejemplo 3: Uso de Archivo como Clave de Encriptación

Usar el contenido de un archivo como clave de encriptación en lugar de una contraseña.

### Implementación en C\#

```csharp
public class FileKeyEncryption
{
    public void EncryptWithFileKey(string inputFile, string outputFile, string keyFile)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var mediaControl = (IMediaControl)filterGraph;

        try
        {
            // Configurar grafo de filtros (fuente, codificadores, muxer)
            // ... (igual que Ejemplo 1)

            // Añadir muxer de encriptación
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer"));

            // Configurar encriptación usando archivo como clave
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            // Usar contenido del archivo como clave de encriptación (hash SHA-256 del archivo)
            cryptoConfig.ApplyFile(keyFile);

            Console.WriteLine($"Usando archivo de clave: {keyFile}");
            Console.WriteLine("Contenido del archivo hasheado con SHA-256 para encriptación");

            // Verificar
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");
            Console.WriteLine("Clave de encriptación configurada exitosamente");

            // Continuar con la configuración del grafo de filtros y codificación...
        }
        finally
        {
            // Limpieza
        }
    }

    public void DecryptWithFileKey(string encryptedFile, string keyFile)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();

        try
        {
            // Añadir demuxer de desencriptación
            var decryptDemuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoDecryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(decryptDemuxer, "Decrypt Demuxer"));

            // Configurar desencriptación usando el mismo archivo de clave
            var cryptoConfig = decryptDemuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            // DEBE usar el mismo archivo de clave que la encriptación
            cryptoConfig.ApplyFile(keyFile);
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");

            Console.WriteLine($"Usando archivo de clave para desencriptación: {keyFile}");

            // Continuar con la configuración de reproducción...
        }
        finally
        {
            // Limpieza
        }
    }
}

// Uso:
// var encryptor = new FileKeyEncryption();
// encryptor.EncryptWithFileKey(@"C:\input.mp4", @"C:\output.encrypted.mp4", @"C:\keys\encryption.key");
// encryptor.DecryptWithFileKey(@"C:\output.encrypted.mp4", @"C:\keys\encryption.key");
```

---
## Ejemplo 4: Uso de Datos de Clave Binaria
Generar y usar datos de clave binaria para encriptación.
### Implementación en C\#
```csharp
using System.Security.Cryptography;
using System.IO;
public class BinaryKeyEncryption
{
    public byte[] GenerateRandomKey(int keySize = 32)
    {
        // Generar clave aleatoria criptográficamente segura
        byte[] key = new byte[keySize];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(key);
        }
        return key;
    }
    public void SaveKeyToFile(byte[] key, string keyFile)
    {
        File.WriteAllBytes(keyFile, key);
        Console.WriteLine($"Clave guardada en: {keyFile}");
    }
    public byte[] LoadKeyFromFile(string keyFile)
    {
        return File.ReadAllBytes(keyFile);
    }
    public void EncryptWithBinaryKey(string inputFile, string outputFile, byte[] keyData)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        try
        {
            // Configurar grafo de filtros y añadir muxer de encriptación
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer"));
            // Configurar encriptación con clave binaria
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            // Aplicar clave binaria (será hasheada con SHA-256)
            cryptoConfig.ApplyBinary(keyData);
            Console.WriteLine($"Clave binaria aplicada (longitud: {keyData.Length} bytes)");
            // Verificar
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");
            Console.WriteLine("Clave de encriptación configurada exitosamente");
            // Continuar con la codificación...
        }
        finally
        {
            // Limpieza
        }
    }
    public void CompleteWorkflow()
    {
        // Generar clave de encriptación aleatoria
        byte[] encryptionKey = GenerateRandomKey(32);
        Console.WriteLine($"Generada clave de encriptación de {encryptionKey.Length * 8} bits");
        // Guardar clave de forma segura
        string keyFile = @"C:\keys\video_encryption_key.bin";
        SaveKeyToFile(encryptionKey, keyFile);
        // Encriptar video con clave binaria
        EncryptWithBinaryKey(
            @"C:\input.mp4",
            @"C:\output.encrypted.mp4",
            encryptionKey
        );
        Console.WriteLine("¡Video encriptado exitosamente!");
        Console.WriteLine($"Archivo de clave: {keyFile}");
        Console.WriteLine("¡Mantenga este archivo de clave seguro - es necesario para la desencriptación!");
        // Luego, para desencriptación:
        // byte[] key = LoadKeyFromFile(keyFile);
        // DecryptWithBinaryKey(@"C:\output.encrypted.mp4", key);
    }
}
// Uso:
// var encryptor = new BinaryKeyEncryption();
// encryptor.CompleteWorkflow();
```
---

## Ejemplo 5: Encriptación con Configuración de Codificador Personalizada

Encriptar video con parámetros de codificación H.264 específicos.

### Implementación en C\#

```csharp
public class CustomEncodingEncryption
{
    public void EncryptWithCustomSettings(
        string inputFile,
        string outputFile,
        string password,
        int videoBitrate = 5000000,
        int audioBitrate = 192000)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var mediaControl = (IMediaControl)filterGraph;

        try
        {
            // Añadir fuente
            DsError.ThrowExceptionForHR(filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter));

            // Añadir y configurar codificador H.264
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
                "H.264 Encoder"
            );

            // Configurar codificador H.264 (si la interfaz está disponible)
            var h264Config = videoEncoder as IH264Encoder;
            if (h264Config != null)
            {
                h264Config.put_Bitrate(videoBitrate);
                h264Config.put_Profile(77); // Perfil Main
                h264Config.put_Level(41);   // Nivel 4.1 (1080p)
                h264Config.put_RateControl(1); // CBR

                Console.WriteLine($"H.264 configurado: {videoBitrate / 1000000.0} Mbps, Perfil Main, Nivel 4.1");
            }

            // Añadir y configurar codificador AAC
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
                "AAC Encoder"
            );

            // Configurar codificador AAC (si la interfaz está disponible)
            var aacConfig = audioEncoder as IVFAACEncoder;
            if (aacConfig != null)
            {
                aacConfig.SetBitRate((uint)audioBitrate);
                aacConfig.SetProfile(2); // AAC-LC
                aacConfig.SetOutputFormat(0); // Raw AAC para MP4

                Console.WriteLine($"AAC configurado: {audioBitrate / 1000} kbps, perfil AAC-LC");
            }

            // Añadir muxer de encriptación
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer"));

            // Configurar encriptación
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            cryptoConfig.ApplyString(password);
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");
            Console.WriteLine("Encriptación configurada");

            // Establecer archivo de salida
            var fileWriter = (IBaseFilter)new FileWriter();
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(fileWriter, "File Writer"));
            DsError.ThrowExceptionForHR(((IFileSinkFilter)fileWriter).SetFileName(outputFile, null));

            // Construir conexiones
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            DsError.ThrowExceptionForHR(captureGraph.SetFiltergraph(filterGraph));

            DsError.ThrowExceptionForHR(captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoder, encryptMuxer));
            // Sin comprobar: un archivo sin audio no tiene nada que conectar aquí
            captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoder, encryptMuxer);
            DsError.ThrowExceptionForHR(captureGraph.RenderStream(null, null, encryptMuxer, null, fileWriter));

            Console.WriteLine("Iniciando codificación con configuración personalizada...");

            // Ejecutar codificación
            int hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            // Esperar finalización
            var mediaEvent = (IMediaEventEx)filterGraph;
            EventCode eventCode;
            do
            {
                hr = mediaEvent.WaitForCompletion(1000, out eventCode);
            }
            while (hr == DSErrorCodes.E_ABORT);
            DsError.ThrowExceptionForHR(hr);
            if (eventCode != EventCode.Complete)
                throw new InvalidOperationException($"Encoding stopped with event {eventCode}");

            Console.WriteLine("¡Codificación y encriptación completadas!");

            Marshal.ReleaseComObject(captureGraph);
        }
        finally
        {
            // Limpieza
            mediaControl?.Stop();
            FilterGraphTools.RemoveAllFilters(filterGraph);
        }
    }
}

// Uso:
// var encryptor = new CustomEncodingEncryption();
// encryptor.EncryptWithCustomSettings(
//     @"C:\input.mp4",
//     @"C:\output.encrypted.mp4",
//     "MiContraseñaSegura123",
//     videoBitrate: 8000000,  // 8 Mbps
//     audioBitrate: 256000    // 256 kbps
// );
```

---
## Ejemplo 6: Manejo de Errores y Validación
Manejo de errores completo para encriptación/desencriptación.
### Implementación en C\#
```csharp
public class RobustEncryption
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    public enum EncryptionResult
    {
        Success,
        FileNotFound,
        InvalidPassword,
        InterfaceNotAvailable,
        FilterGraphError,
        EncodingError,
        Unknown
    }
    public EncryptionResult EncryptVideoSafely(
        string inputFile,
        string outputFile,
        string password)
    {
        try
        {
            // Validar entradas
            if (string.IsNullOrEmpty(inputFile))
            {
                Console.WriteLine("ERROR: Ruta de archivo de entrada vacía");
                return EncryptionResult.FileNotFound;
            }
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"ERROR: Archivo de entrada no encontrado: {inputFile}");
                return EncryptionResult.FileNotFound;
            }
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("ERROR: La contraseña está vacía");
                return EncryptionResult.InvalidPassword;
            }
            if (password.Length < 8)
            {
                Console.WriteLine("ADVERTENCIA: La contraseña tiene menos de 8 caracteres (no recomendado)");
            }
            Console.WriteLine("Validación de entrada aprobada");
            // Crear grafo de filtros
            filterGraph = (IFilterGraph2)new FilterGraph();
            if (filterGraph == null)
            {
                Console.WriteLine("ERROR: Fallo al crear grafo de filtros");
                return EncryptionResult.FilterGraphError;
            }
            mediaControl = (IMediaControl)filterGraph;
            // Añadir filtro fuente
            int hr = filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
            if (hr != 0 || sourceFilter == null)
            {
                Console.WriteLine($"ERROR: Fallo al añadir filtro fuente (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            // Añadir codificadores
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
                "H.264 Encoder"
            );
            if (videoEncoder == null)
            {
                Console.WriteLine("ERROR: Fallo al crear codificador de video");
                return EncryptionResult.FilterGraphError;
            }
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
                "AAC Encoder"
            );
            // Añadir muxer de encriptación
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            if (encryptMuxer == null)
            {
                Console.WriteLine("ERROR: Fallo al crear muxer de encriptación");
                return EncryptionResult.FilterGraphError;
            }
            hr = filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");
            if (hr != 0)
            {
                Console.WriteLine($"ERROR: Fallo al añadir muxer de encriptación (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            // Configurar encriptación
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
            {
                Console.WriteLine("ERROR: Interfaz IVFCryptoConfig no disponible");
                return EncryptionResult.InterfaceNotAvailable;
            }
            cryptoConfig.ApplyString(password);
            // Verificar que la contraseña esté establecida
            hr = cryptoConfig.HavePassword();
            if (hr != 0)
            {
                Console.WriteLine("ERROR: Verificación de contraseña fallida");
                return EncryptionResult.InvalidPassword;
            }
            Console.WriteLine("Contraseña de encriptación configurada exitosamente");
            // Establecer archivo de salida
            var fileWriter = (IBaseFilter)new FileWriter();
            hr = filterGraph.AddFilter(fileWriter, "File Writer");
            if (hr >= 0)
            {
                hr = ((IFileSinkFilter)fileWriter).SetFileName(outputFile, null);
            }
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: Fallo al establecer archivo de salida (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            // Construir grafo de filtros
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: SetFiltergraph failed (HRESULT: 0x{hr:X8})");
                Marshal.ReleaseComObject(captureGraph);
                return EncryptionResult.FilterGraphError;
            }
            hr = captureGraph.RenderStream(
                null, MediaType.Video,
                sourceFilter, videoEncoder, encryptMuxer
            );
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: Video connection failed (HRESULT: 0x{hr:X8})");
                Marshal.ReleaseComObject(captureGraph);
                return EncryptionResult.FilterGraphError;
            }
            hr = captureGraph.RenderStream(
                null, MediaType.Audio,
                sourceFilter, audioEncoder, encryptMuxer
            );
            if (hr != 0)
            {
                Console.WriteLine($"ADVERTENCIA: Conexión de audio fallida (HRESULT: 0x{hr:X8}) - continuando sin audio");
            }
            hr = captureGraph.RenderStream(null, null, encryptMuxer, null, fileWriter);
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: Fallo al conectar File Writer (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            Console.WriteLine("Grafo de filtros construido exitosamente");
            Console.WriteLine("Iniciando encriptación...");
            // Iniciar codificación
            hr = mediaControl.Run();
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: Fallo al iniciar codificación (HRESULT: 0x{hr:X8})");
                return EncryptionResult.EncodingError;
            }
            // Monitorizar progreso
            var mediaEvent = (IMediaEventEx)filterGraph;
            EventCode eventCode;
            IntPtr param1, param2;
            do
            {
                hr = mediaEvent.GetEvent(out eventCode, out param1, out param2, 1000);
                if (hr == 0)
                {
                    mediaEvent.FreeEventParams(eventCode, param1, param2);
                    if (eventCode == EventCode.Complete)
                    {
                        break;
                    }
                    else if (eventCode == EventCode.ErrorAbort || eventCode == EventCode.UserAbort || eventCode == EventCode.ErrorAbortEx)
                    {
                        Console.WriteLine($"ERROR: Codificación abortada (código de evento: {eventCode})");
                        return EncryptionResult.EncodingError;
                    }
                }
                else if (hr != DSErrorCodes.E_ABORT)
                {
                    Console.WriteLine($"ERROR: GetEvent failed (HRESULT: 0x{hr:X8})");
                    return EncryptionResult.EncodingError;
                }
            }
            while (true);
            Console.WriteLine("¡Encriptación completada exitosamente!");
            // Verificar que el archivo de salida existe
            if (File.Exists(outputFile))
            {
                FileInfo fi = new FileInfo(outputFile);
                Console.WriteLine($"Archivo de salida creado: {outputFile}");
                Console.WriteLine($"Tamaño de archivo: {fi.Length / (1024.0 * 1024.0):F2} MB");
            }
            else
            {
                Console.WriteLine("ADVERTENCIA: Archivo de salida no encontrado después de la codificación");
            }
            Marshal.ReleaseComObject(captureGraph);
            return EncryptionResult.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPCIÓN: {ex.Message}");
            Console.WriteLine($"Traza de pila: {ex.StackTrace}");
            return EncryptionResult.Unknown;
        }
        finally
        {
            Cleanup();
        }
    }
    private void Cleanup()
    {
        try
        {
            if (mediaControl != null)
            {
                mediaControl.Stop();
                Marshal.ReleaseComObject(mediaControl);
            }
            if (filterGraph != null)
            {
                FilterGraphTools.RemoveAllFilters(filterGraph);
                Marshal.ReleaseComObject(filterGraph);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error de limpieza: {ex.Message}");
        }
    }
}
// Uso:
// var encryptor = new RobustEncryption();
// var result = encryptor.EncryptVideoSafely(
//     @"C:\input.mp4",
//     @"C:\output.encrypted.mp4",
//     "MiContraseñaSegura123"
// );
//
// if (result == RobustEncryption.EncryptionResult.Success)
// {
//     Console.WriteLine("¡ÉXITO!");
// }
// else
// {
//     Console.WriteLine($"FALLO: {result}");
// }
```
---

## Mejores Prácticas

### Seguridad de Contraseñas

1. **Use Contraseñas Fuertes**
   - Mínimo 16 caracteres
   - Mezcla de mayúsculas, minúsculas, números, símbolos
   - Evite palabras del diccionario
   - Use gestores de contraseñas para generar/almacenar

2. **Almacenamiento de Claves**
   - Nunca codifique contraseñas en el código fuente
   - Use almacenamiento seguro de claves (Windows DPAPI, KeyVault, etc.)
   - Implemente controles de acceso adecuados

3. **Distribución de Claves**
   - Use canales seguros para la distribución de claves
   - Considere encriptación de clave pública para el intercambio de claves
   - Implemente políticas de rotación de claves

### Seguridad de Implementación

**BIEN: Manejo seguro de contraseñas**

```csharp
public void SetEncryptionPassword(IVFCryptoConfig config)
{
    // Obtener contraseña de almacenamiento seguro
    string password = SecureStorage.GetPassword();

    try
    {
        config.ApplyString(password);
    }
    finally
    {
        // Limpiar contraseña de la memoria
        password = null;
        GC.Collect();
    }
}
```

**MAL: Contraseña codificada**

```csharp
public void SetEncryptionPassword(IVFCryptoConfig config)
{
    config.ApplyString("MiContraseña123"); // ¡NO HAGA ESTO!
}
```

### Seguridad de Archivos

- Use permisos de archivo apropiados para archivos encriptados
- Implemente borrado seguro para archivos temporales
- Considere encriptar archivos de claves con encriptación adicional
- Registre el acceso a contenido encriptado para pistas de auditoría

### Pautas de Implementación

1. **Siempre Valide Entradas**
   - Compruebe la existencia del archivo antes de procesar
   - Valide que la contraseña no esté vacía
   - Verifique que las interfaces de filtro estén disponibles

2. **Manejo de Errores**
   - Compruebe los valores HRESULT de las llamadas COM
   - Use bloques try-catch para excepciones
   - Proporcione mensajes de error significativos

3. **Gestión de Recursos**
   - Siempre libere objetos COM
   - Use bloques `finally` para limpieza
   - Detenga el grafo de filtros antes de liberar

4. **Pruebas**
   - Pruebe con varios formatos de entrada
   - Verifique que los archivos encriptados se puedan desencriptar
   - Pruebe condiciones de error (contraseña incorrecta, etc.)

---
## Solución de Problemas
### Códigos de Error Comunes
| HRESULT | Descripción | Solución |
|---------|-------------|----------|
| `E_INVALIDARG` | Búfer o tamaño inválido | Compruebe los parámetros de puntero y tamaño del búfer |
| `E_POINTER` | Puntero nulo | Asegúrese de que los punteros sean válidos antes de llamar |
| `E_FAIL` | Fallo general | Compruebe el estado y la configuración del filtro |
| `S_FALSE` | Contraseña no establecida | Llame a `put_Password` antes de `HavePassword` |
### Ejemplo de Manejo de Errores
```csharp
public bool ConfigureEncryption(IBaseFilter muxer, string password)
{
    var cryptoConfig = muxer as IVFCryptoConfig;
    if (cryptoConfig == null)
    {
        Console.WriteLine("ERROR: El filtro no soporta IVFCryptoConfig");
        return false;
    }
    try
    {
        // Establecer contraseña
        cryptoConfig.ApplyString(password);
        // Verificar
        int hr = cryptoConfig.HavePassword();
        if (hr != 0)
        {
            Console.WriteLine("ERROR: Contraseña no establecida correctamente");
            return false;
        }
        Console.WriteLine("Encriptación configurada exitosamente");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: Fallo al configurar encriptación: {ex.Message}");
        return false;
    }
}
```
---

### Problemas Comunes

#### Error "Filter not registered"

**Problema**: CLSID no encontrado

**Solución**:
```bash
# Registrar filtros (ejecutar como Administrador)
regsvr32 "C:\Path\To\EncryptMuxer.ax"
regsvr32 "C:\Path\To\DecryptDemuxer.ax"
```

#### Error "Interface not available"

**Problema**: No se puede consultar IVFCryptoConfig

**Solución**:
- Verifique que la versión del filtro sea correcta
- Compruebe que el filtro esté registrado correctamente
- Asegúrese de que las interfaces COM sean compatibles

#### La Desencriptación Falla con Video Distorsionado

**Problema**: Contraseña incorrecta usada para desencriptación

**Solución**:
- Verifique que se use la misma contraseña para encriptación y desencriptación
- Compruebe la codificación de la contraseña (Unicode vs. ANSI)
- Asegúrese de que no haya errores tipográficos en la contraseña

#### El Archivo de Salida está Vacío o Corrupto

**Problema**: La codificación falló silenciosamente

**Solución**:
- Conecte la salida del cifrador a un File Writer y establezca el nombre de archivo en el File Writer: el cifrador no implementa `IFileSinkFilter`
- Compruebe que todos los filtros codificadores estén conectados correctamente
- Verifique que las configuraciones del codificador sean válidas
- Monitorice IMediaEventEx para eventos de error
- Compruebe que haya espacio en disco disponible

---
## Ver También
- [Descripción General del Video Encryption SDK](index.md) - Características del producto
- [Referencia de Interfaz](interface-reference.md) - Documentación completa de la API
- [DirectShow Encoding Filters Pack](../filters-enc/index.md) - Codificadores compatibles
- [Ejemplos en GitHub](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) - Código fuente completo
