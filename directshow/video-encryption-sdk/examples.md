---
title: DirectShow Video Encryption Examples - C#, C++, Delphi
description: Encrypt and decrypt MP4 video files using VisioForge DirectShow filters. AES-256 examples with password protection and binary key modes.
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

# Video Encryption SDK - Code Examples

## Overview

This page provides comprehensive, working code examples for encrypting and decrypting video files using the Video Encryption SDK. Examples cover:

- **Basic Encryption** - Encrypt video files with password protection
- **File Decryption** - Decrypt and play encrypted video files
- **Advanced Scenarios** - Binary keys, file-based keys, custom encoding settings
- **Error Handling** - Robust error checking and recovery

All examples include complete implementations in C++, C#, and Delphi.

!!! note "Interface naming across language wrappers"
    The native C++ header (`encryptor_intf.h`) declares this interface as `ICryptoConfig` with `IID_ICryptoConfig`. The C# and Delphi wrappers expose the same interface as `IVFCryptoConfig` (and the password provider as `IVFPasswordProvider`). Both names share GUID `{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}` and refer to the same COM interface — the C++ snippets below use `ICryptoConfig` while the C#/Delphi snippets use `IVFCryptoConfig`.

---
## Prerequisites
### C++ Projects
```cpp
#include <dshow.h>
#include <streams.h>
#include "encryptor_intf.h"
#pragma comment(lib, "strmiids.lib")
#pragma comment(lib, "quartz.lib")
```
### C# Projects
```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
```
**Required NuGet Packages**:

- VisioForge.DirectShowAPI (it also contains the `VisioForge.DirectShowLib` namespace)
### Delphi Projects
```delphi
uses
  ENCDirectShow9,
  encryptor_intf;
```
---

## Example 1: Basic Video Encryption

Encrypt a video file with string password.

### C# Implementation

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
            // Create filter graph
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            mediaEvent = (IMediaEventEx)filterGraph;

            // Add source filter
            int hr = filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
            DsError.ThrowExceptionForHR(hr);

            // Add video encoder (H.264)
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
                "H.264 Encoder"
            );

            // Add audio encoder (AAC)
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
                "AAC Encoder"
            );

            // Add encrypt muxer filter
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );

            hr = filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");
            DsError.ThrowExceptionForHR(hr);

            // Configure encryption password
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Apply password using helper method
                cryptoConfig.ApplyString(password);

                // Verify password is set
                hr = cryptoConfig.HavePassword();
                if (hr != 0)
                {
                    throw new Exception("Failed to set encryption password");
                }

                Console.WriteLine("Encryption password configured successfully");
            }
            else
            {
                throw new Exception("IVFCryptoConfig interface not available");
            }

            // Set output file
            var fileWriter = (IBaseFilter)new FileWriter();
            hr = filterGraph.AddFilter(fileWriter, "File Writer");
            DsError.ThrowExceptionForHR(hr);
            hr = ((IFileSinkFilter)fileWriter).SetFileName(outputFile, null);
            DsError.ThrowExceptionForHR(hr);

            // Build filter graph connections
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Connect video path: Source → H.264 Encoder → Encrypt Muxer
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                sourceFilter,
                videoEncoder,
                encryptMuxer
            );
            DsError.ThrowExceptionForHR(hr);

            // Connect audio path: Source → AAC Encoder → Encrypt Muxer
            hr = captureGraph.RenderStream(
                null,
                MediaType.Audio,
                sourceFilter,
                audioEncoder,
                encryptMuxer
            );
            // Not checked: a file without audio has nothing to connect here

            hr = captureGraph.RenderStream(null, null, encryptMuxer, null, fileWriter);
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine("Filter graph built successfully");
            Console.WriteLine("Starting encryption...");

            // Start encoding
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            // Wait for completion
            EventCode eventCode;
            do
            {
                hr = mediaEvent.WaitForCompletion(1000, out eventCode);
            }
            while (hr == DSErrorCodes.E_ABORT);
            DsError.ThrowExceptionForHR(hr);
            if (eventCode != EventCode.Complete)
                throw new InvalidOperationException($"Encoding stopped with event {eventCode}");

            Console.WriteLine("Encryption completed successfully!");

            // Cleanup
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

// Usage:
// var encryptor = new BasicVideoEncryption();
// encryptor.EncryptVideo(@"C:\input.mp4", @"C:\output.encrypted.mp4", "MySecurePassword123");
```

### C++ Implementation

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

        // Create filter graph
        hr = CoCreateInstance(
            CLSID_FilterGraph,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IGraphBuilder,
            (void**)&pGraph
        );
        if (FAILED(hr)) return hr;

        // Get control and event interfaces
        pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
        pGraph->QueryInterface(IID_IMediaEvent, (void**)&pEvent);

        // Add source filter
        hr = pGraph->AddSourceFilter(inputFile, L"Source", &pSource);
        if (FAILED(hr)) goto cleanup;

        // Add video encoder (H.264)
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

        // Add audio encoder (AAC)
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

        // Add encrypt muxer
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

        // Configure encryption
        hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
        if (SUCCEEDED(hr))
        {
            // Set password
            hr = pCrypto->put_Password(
                (LPBYTE)password,
                wcslen(password) * sizeof(wchar_t)
            );

            if (SUCCEEDED(hr))
            {
                // Verify password is set
                HRESULT hrPassword = pCrypto->HavePassword();
                if (hrPassword == S_OK)
                {
                    wprintf(L"Encryption password configured successfully\n");
                }
                else
                {
                    wprintf(L"WARNING: Password not set\n");
                    hr = E_FAIL;
                }
            }

            pCrypto->Release();
        }
        if (FAILED(hr)) goto cleanup;

        // Set output file
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

        // Build graph using Intelligent Connect
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

            // Connect video path
            if (SUCCEEDED(hr)) hr = pBuilder->RenderStream(
                NULL,
                &MEDIATYPE_Video,
                pSource,
                pVideoEncoder,
                pMuxer
            );

            // Connect audio path
            // Not checked: a file without audio has nothing to connect here
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

        wprintf(L"Starting encryption...\n");

        // Run the graph
        hr = pControl->Run();
        if (FAILED(hr)) goto cleanup;

        // Wait for completion
        long evCode;
        hr = pEvent->WaitForCompletion(INFINITE, &evCode);
        if (FAILED(hr)) goto cleanup;
        if (evCode != EC_COMPLETE)
        {
            hr = E_FAIL;
            goto cleanup;
        }

        wprintf(L"Encryption completed successfully!\n");

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

// Usage:
// BasicVideoEncryption encryptor;
// encryptor.EncryptVideo(L"C:\\input.mp4", L"C:\\output.encrypted.mp4", L"MySecurePassword123");
```

### Delphi Implementation

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
    // Create filter graph
    Result := CoCreateInstance(CLSID_FilterGraph, nil, CLSCTX_INPROC_SERVER,
                               IID_IGraphBuilder, FFilterGraph);
    if Failed(Result) then Exit;

    // Get control and event interfaces
    FFilterGraph.QueryInterface(IID_IMediaControl, FMediaControl);
    FFilterGraph.QueryInterface(IID_IMediaEvent, FMediaEvent);

    // Add source filter
    Result := FFilterGraph.AddSourceFilter(PWideChar(InputFile), 'Source', SourceFilter);
    if Failed(Result) then Exit;

    // Add video encoder (H.264)
    Result := CoCreateInstance(CLSID_VFH264EncoderV10, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, VideoEncoder);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(VideoEncoder, 'H.264 Encoder');
    if Failed(Result) then Exit;

    // Add audio encoder (AAC)
    Result := CoCreateInstance(CLSID_VFAACEncoderV10, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, AudioEncoder);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(AudioEncoder, 'AAC Encoder');
    if Failed(Result) then Exit;

    // Add encrypt muxer
    Result := CoCreateInstance(CLSID_VFEncryptor9, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, EncryptMuxer);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(EncryptMuxer, 'Encrypt Muxer');
    if Failed(Result) then Exit;

    // Configure encryption
    Result := E_NOINTERFACE;
    if Supports(EncryptMuxer, IVFCryptoConfig, CryptoConfig) then
    begin
      // Set password
      // pBuffer is opaque binary data (PByte); cast through PWideChar to reuse
      // the wide-string buffer, length in bytes = char count * 2.
      Result := CryptoConfig.put_Password(PByte(PWideChar(Password)), Length(Password) * 2);

      if Succeeded(Result) then
      begin
        // Verify password is set
        if CryptoConfig.HavePassword = S_OK then
          WriteLn('Encryption password configured successfully')
        else
        begin
          WriteLn('WARNING: Password not set');
          Result := E_FAIL;
        end;
      end;
    end;
    if Failed(Result) then Exit;

    // Set output file
    Result := CoCreateInstance(CLSID_FileWriter, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, FileWriter);
    if Failed(Result) then Exit;
    Result := FFilterGraph.AddFilter(FileWriter, 'File Writer');
    if Failed(Result) then Exit;
    Result := E_NOINTERFACE;
    if not Supports(FileWriter, IFileSinkFilter, FileSink) then Exit;
    Result := FileSink.SetFileName(PWideChar(OutputFile), nil);
    if Failed(Result) then Exit;

    // Build graph connections
    Result := CoCreateInstance(CLSID_CaptureGraphBuilder2, nil, CLSCTX_INPROC_SERVER,
                               IID_ICaptureGraphBuilder2, CaptureGraph);
    if Succeeded(Result) then
    begin
      Result := CaptureGraph.SetFiltergraph(FFilterGraph);

      // Connect video path
      if Succeeded(Result) then
        Result := CaptureGraph.RenderStream(nil, @MEDIATYPE_Video, SourceFilter,
                                            VideoEncoder, EncryptMuxer);

      // Connect audio path
      // Not checked: a file without audio has nothing to connect here
      CaptureGraph.RenderStream(nil, @MEDIATYPE_Audio, SourceFilter,
                                AudioEncoder, EncryptMuxer);

      if Succeeded(Result) then
        Result := CaptureGraph.RenderStream(nil, nil, EncryptMuxer, nil, FileWriter);
    end;
    if Failed(Result) then Exit;

    WriteLn('Starting encryption...');

    // Run the graph
    Result := FMediaControl.Run;
    if Failed(Result) then Exit;

    // Wait for completion
    repeat
      Result := FMediaEvent.WaitForCompletion(1000, EventCode);
    until Result <> E_ABORT;
    if Failed(Result) then Exit;
    if EventCode <> EC_COMPLETE then
    begin
      Result := E_FAIL;
      Exit;
    end;

    WriteLn('Encryption completed successfully!');

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

// Usage:
// var
//   Encryptor: TBasicVideoEncryption;
// begin
//   Encryptor := TBasicVideoEncryption.Create;
//   try
//     Encryptor.EncryptVideo('C:\input.mp4', 'C:\output.encrypted.mp4', 'MySecurePassword123');
//   finally
//     Encryptor.Free;
//   end;
// end;
```

---
## Example 2: Video Decryption and Playback {#example-2-decryption}

Decrypt an encrypted video file and play it.

### C# Implementation {#example-2-csharp}

<!-- code block -->
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
            // Create filter graph
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            mediaEvent = (IMediaEventEx)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;
            // Add decrypt demuxer filter
            var decryptDemuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoDecryptor9))
            );
            int hr = filterGraph.AddFilter(decryptDemuxer, "Decrypt Demuxer");
            DsError.ThrowExceptionForHR(hr);
            // Configure decryption (MUST use same password as encryption)
            var cryptoConfig = decryptDemuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Apply password
                cryptoConfig.ApplyString(password);
                // Verify password is set
                hr = cryptoConfig.HavePassword();
                if (hr != 0)
                {
                    throw new Exception("Failed to set decryption password");
                }
                Console.WriteLine("Decryption password configured successfully");
            }
            else
            {
                throw new Exception("IVFCryptoConfig interface not available");
            }
            // Load encrypted file
            var fileSourceFilter = (IBaseFilter)new AsyncReader();
            hr = filterGraph.AddFilter(fileSourceFilter, "File Source");
            DsError.ThrowExceptionForHR(hr);
            hr = ((IFileSourceFilter)fileSourceFilter).Load(encryptedFile, null);
            DsError.ThrowExceptionForHR(hr);
            // Build graph - render video and audio outputs
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);
            hr = captureGraph.RenderStream(null, null, fileSourceFilter, null, decryptDemuxer);
            DsError.ThrowExceptionForHR(hr);
            // Render video output
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                decryptDemuxer,
                null,
                null
            );
            DsError.ThrowExceptionForHR(hr);
            // Render audio output
            hr = captureGraph.RenderStream(
                null,
                MediaType.Audio,
                decryptDemuxer,
                null,
                null
            );
            // Audio is optional, don't fail if not present
            // Configure video window
            hr = videoWindow.put_Owner(windowHandle);
            hr = videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            hr = videoWindow.put_MessageDrain(windowHandle);
            Console.WriteLine("Starting playback...");
            // Start playback
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
            Console.WriteLine("Decryption and playback started successfully!");
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
// Usage:
// var decryptor = new VideoDecryption();
// decryptor.DecryptAndPlay(@"C:\output.encrypted.mp4", "MySecurePassword123", this.Handle);
```
---

## Example 3: Using File as Encryption Key

Use a file's content as the encryption key instead of a password.

### C# Implementation

```csharp
public class FileKeyEncryption
{
    public void EncryptWithFileKey(string inputFile, string outputFile, string keyFile)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var mediaControl = (IMediaControl)filterGraph;

        try
        {
            // Setup filter graph (source, encoders, muxer)
            // ... (same as Example 1)

            // Add encrypt muxer
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer"));

            // Configure encryption using file as key
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            // Use file content as encryption key (SHA-256 hash of file)
            cryptoConfig.ApplyFile(keyFile);

            Console.WriteLine($"Using key file: {keyFile}");
            Console.WriteLine("File content hashed with SHA-256 for encryption");

            // Verify
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");
            Console.WriteLine("Encryption key configured successfully");

            // Continue with filter graph setup and encoding...
        }
        finally
        {
            // Cleanup
        }
    }

    public void DecryptWithFileKey(string encryptedFile, string keyFile)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();

        try
        {
            // Add decrypt demuxer
            var decryptDemuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoDecryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(decryptDemuxer, "Decrypt Demuxer"));

            // Configure decryption using same key file
            var cryptoConfig = decryptDemuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            // MUST use same key file as encryption
            cryptoConfig.ApplyFile(keyFile);
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");

            Console.WriteLine($"Using key file for decryption: {keyFile}");

            // Continue with playback setup...
        }
        finally
        {
            // Cleanup
        }
    }
}

// Usage:
// var encryptor = new FileKeyEncryption();
// encryptor.EncryptWithFileKey(@"C:\input.mp4", @"C:\output.encrypted.mp4", @"C:\keys\encryption.key");
// encryptor.DecryptWithFileKey(@"C:\output.encrypted.mp4", @"C:\keys\encryption.key");
```

---
## Example 4: Using Binary Key Data
Generate and use binary key data for encryption.
### C# Implementation {#example-4-csharp}

<!-- code block -->
```csharp
using System.Security.Cryptography;
using System.IO;
public class BinaryKeyEncryption
{
    public byte[] GenerateRandomKey(int keySize = 32)
    {
        // Generate cryptographically secure random key
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
        Console.WriteLine($"Key saved to: {keyFile}");
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
            // Setup filter graph and add encrypt muxer
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer"));
            // Configure encryption with binary key
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            // Apply binary key (will be hashed with SHA-256)
            cryptoConfig.ApplyBinary(keyData);
            Console.WriteLine($"Binary key applied (length: {keyData.Length} bytes)");
            // Verify
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");
            Console.WriteLine("Encryption key configured successfully");
            // Continue with encoding...
        }
        finally
        {
            // Cleanup
        }
    }
    public void CompleteWorkflow()
    {
        // Generate random encryption key
        byte[] encryptionKey = GenerateRandomKey(32);
        Console.WriteLine($"Generated {encryptionKey.Length * 8}-bit encryption key");
        // Save key securely
        string keyFile = @"C:\keys\video_encryption_key.bin";
        SaveKeyToFile(encryptionKey, keyFile);
        // Encrypt video with binary key
        EncryptWithBinaryKey(
            @"C:\input.mp4",
            @"C:\output.encrypted.mp4",
            encryptionKey
        );
        Console.WriteLine("Video encrypted successfully!");
        Console.WriteLine($"Key file: {keyFile}");
        Console.WriteLine("Keep this key file secure - it's required for decryption!");
        // Later, for decryption:
        // byte[] key = LoadKeyFromFile(keyFile);
        // DecryptWithBinaryKey(@"C:\output.encrypted.mp4", key);
    }
}
// Usage:
// var encryptor = new BinaryKeyEncryption();
// encryptor.CompleteWorkflow();
```
---

## Example 5: Encryption with Custom Encoder Settings

Encrypt video with specific H.264 encoding parameters.

### C# Implementation

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
            // Add source
            DsError.ThrowExceptionForHR(filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter));

            // Add and configure H.264 encoder
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
                "H.264 Encoder"
            );

            // Configure H.264 encoder (if interface available)
            var h264Config = videoEncoder as IH264Encoder;
            if (h264Config != null)
            {
                h264Config.put_Bitrate(videoBitrate);
                h264Config.put_Profile(77); // Main profile
                h264Config.put_Level(41);   // Level 4.1 (1080p)
                h264Config.put_RateControl(1); // CBR

                Console.WriteLine($"H.264 configured: {videoBitrate / 1000000.0} Mbps, Main profile, Level 4.1");
            }

            // Add and configure AAC encoder
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
                "AAC Encoder"
            );

            // Configure AAC encoder (if interface available)
            var aacConfig = audioEncoder as IVFAACEncoder;
            if (aacConfig != null)
            {
                aacConfig.SetBitRate((uint)audioBitrate);
                aacConfig.SetProfile(2); // AAC-LC
                aacConfig.SetOutputFormat(0); // Raw AAC for MP4

                Console.WriteLine($"AAC configured: {audioBitrate / 1000} kbps, AAC-LC profile");
            }

            // Add encrypt muxer
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer"));

            // Configure encryption
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
                throw new InvalidOperationException("IVFCryptoConfig is not available");
            cryptoConfig.ApplyString(password);
            if (cryptoConfig.HavePassword() != 0)
                throw new InvalidOperationException("Password not set");
            Console.WriteLine("Encryption configured");

            // Set output file
            var fileWriter = (IBaseFilter)new FileWriter();
            DsError.ThrowExceptionForHR(filterGraph.AddFilter(fileWriter, "File Writer"));
            DsError.ThrowExceptionForHR(((IFileSinkFilter)fileWriter).SetFileName(outputFile, null));

            // Build connections
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            DsError.ThrowExceptionForHR(captureGraph.SetFiltergraph(filterGraph));

            DsError.ThrowExceptionForHR(captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoder, encryptMuxer));
            // Not checked: a file without audio has nothing to connect here
            captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoder, encryptMuxer);
            DsError.ThrowExceptionForHR(captureGraph.RenderStream(null, null, encryptMuxer, null, fileWriter));

            Console.WriteLine("Starting encoding with custom settings...");

            // Run encoding
            int hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            // Wait for completion
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

            Console.WriteLine("Encoding and encryption completed!");

            Marshal.ReleaseComObject(captureGraph);
        }
        finally
        {
            // Cleanup
            mediaControl?.Stop();
            FilterGraphTools.RemoveAllFilters(filterGraph);
        }
    }
}

// Usage:
// var encryptor = new CustomEncodingEncryption();
// encryptor.EncryptWithCustomSettings(
//     @"C:\input.mp4",
//     @"C:\output.encrypted.mp4",
//     "MySecurePassword123",
//     videoBitrate: 8000000,  // 8 Mbps
//     audioBitrate: 256000    // 256 kbps
// );
```

---

## Example 6: Error Handling and Validation {#example-6-error-handling}

Comprehensive error handling for encryption/decryption.

### C# Implementation {#example-6-csharp}

<!-- code block -->
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
            // Validate inputs
            if (string.IsNullOrEmpty(inputFile))
            {
                Console.WriteLine("ERROR: Input file path is empty");
                return EncryptionResult.FileNotFound;
            }
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"ERROR: Input file not found: {inputFile}");
                return EncryptionResult.FileNotFound;
            }
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("ERROR: Password is empty");
                return EncryptionResult.InvalidPassword;
            }
            if (password.Length < 8)
            {
                Console.WriteLine("WARNING: Password is less than 8 characters (not recommended)");
            }
            Console.WriteLine("Input validation passed");
            // Create filter graph
            filterGraph = (IFilterGraph2)new FilterGraph();
            if (filterGraph == null)
            {
                Console.WriteLine("ERROR: Failed to create filter graph");
                return EncryptionResult.FilterGraphError;
            }
            mediaControl = (IMediaControl)filterGraph;
            // Add source filter
            int hr = filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
            if (hr != 0 || sourceFilter == null)
            {
                Console.WriteLine($"ERROR: Failed to add source filter (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            // Add encoders
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
                "H.264 Encoder"
            );
            if (videoEncoder == null)
            {
                Console.WriteLine("ERROR: Failed to create video encoder");
                return EncryptionResult.FilterGraphError;
            }
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
                "AAC Encoder"
            );
            // Add encrypt muxer
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid(Consts.CLSID_VFVideoEncryptor9))
            );
            if (encryptMuxer == null)
            {
                Console.WriteLine("ERROR: Failed to create encrypt muxer");
                return EncryptionResult.FilterGraphError;
            }
            hr = filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");
            if (hr != 0)
            {
                Console.WriteLine($"ERROR: Failed to add encrypt muxer (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            // Configure encryption
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
            {
                Console.WriteLine("ERROR: IVFCryptoConfig interface not available");
                return EncryptionResult.InterfaceNotAvailable;
            }
            cryptoConfig.ApplyString(password);
            // Verify password is set
            hr = cryptoConfig.HavePassword();
            if (hr != 0)
            {
                Console.WriteLine("ERROR: Password verification failed");
                return EncryptionResult.InvalidPassword;
            }
            Console.WriteLine("Encryption password configured successfully");
            // Set output file
            var fileWriter = (IBaseFilter)new FileWriter();
            hr = filterGraph.AddFilter(fileWriter, "File Writer");
            if (hr >= 0)
            {
                hr = ((IFileSinkFilter)fileWriter).SetFileName(outputFile, null);
            }
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: Failed to set output file (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            // Build filter graph
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
                Console.WriteLine($"WARNING: Audio connection failed (HRESULT: 0x{hr:X8}) - continuing without audio");
            }
            hr = captureGraph.RenderStream(null, null, encryptMuxer, null, fileWriter);
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: File Writer connection failed (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            Console.WriteLine("Filter graph built successfully");
            Console.WriteLine("Starting encryption...");
            // Start encoding
            hr = mediaControl.Run();
            if (hr < 0)
            {
                Console.WriteLine($"ERROR: Failed to start encoding (HRESULT: 0x{hr:X8})");
                return EncryptionResult.EncodingError;
            }
            // Monitor progress
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
                        Console.WriteLine($"ERROR: Encoding aborted (event code: {eventCode})");
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
            Console.WriteLine("Encryption completed successfully!");
            // Verify output file exists
            if (File.Exists(outputFile))
            {
                FileInfo fi = new FileInfo(outputFile);
                Console.WriteLine($"Output file created: {outputFile}");
                Console.WriteLine($"File size: {fi.Length / (1024.0 * 1024.0):F2} MB");
            }
            else
            {
                Console.WriteLine("WARNING: Output file not found after encoding");
            }
            Marshal.ReleaseComObject(captureGraph);
            return EncryptionResult.Success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXCEPTION: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
            Console.WriteLine($"Cleanup error: {ex.Message}");
        }
    }
}
// Usage:
// var encryptor = new RobustEncryption();
// var result = encryptor.EncryptVideoSafely(
//     @"C:\input.mp4",
//     @"C:\output.encrypted.mp4",
//     "MySecurePassword123"
// );
//
// if (result == RobustEncryption.EncryptionResult.Success)
// {
//     Console.WriteLine("SUCCESS!");
// }
// else
// {
//     Console.WriteLine($"FAILED: {result}");
// }
```
---

## Best Practices

### Password Security

1. **Use Strong Passwords**
   - Minimum 16 characters
   - Mix of uppercase, lowercase, numbers, symbols
   - Avoid dictionary words
   - Use password managers to generate/store

2. **Key Storage**
   - Never hardcode passwords in source code
   - Use secure key storage (Windows DPAPI, KeyVault, etc.)
   - Implement proper access controls

3. **Key Distribution**
   - Use secure channels for key distribution
   - Consider public-key encryption for key exchange
   - Implement key rotation policies

### Implementation Security

**GOOD: Secure password handling**

```csharp
public void SetEncryptionPassword(IVFCryptoConfig config)
{
    // Get password from secure storage
    string password = SecureStorage.GetPassword();

    try
    {
        config.ApplyString(password);
    }
    finally
    {
        // Clear password from memory
        password = null;
        GC.Collect();
    }
}
```

**BAD: Hardcoded password**

```csharp
public void SetEncryptionPassword(IVFCryptoConfig config)
{
    config.ApplyString("MyPassword123"); // DON'T DO THIS!
}
```

### File Security

- Use appropriate file permissions for encrypted files
- Implement secure deletion for temporary files
- Consider encrypting key files with additional encryption
- Log access to encrypted content for audit trails

### Implementation Guidelines

1. **Always Validate Inputs**
   - Check file existence before processing
   - Validate password is not empty
   - Verify filter interfaces are available

2. **Error Handling**
   - Check HRESULT values from COM calls
   - Use try-catch blocks for exceptions
   - Provide meaningful error messages

3. **Resource Management**
   - Always release COM objects
   - Use `finally` blocks for cleanup
   - Stop filter graph before releasing

4. **Testing**
   - Test with various input formats
   - Verify encrypted files can be decrypted
   - Test error conditions (wrong password, etc.)

---
## Troubleshooting
### Common Error Codes
| HRESULT | Description | Solution |
|---------|-------------|----------|
| `E_INVALIDARG` | Invalid buffer or size | Check buffer pointer and size parameters |
| `E_POINTER` | Null pointer | Ensure pointers are valid before calling |
| `E_FAIL` | General failure | Check filter state and configuration |
| `S_FALSE` | No password set | Call `put_Password` before `HavePassword` |
### Error Handling Example
```csharp
public bool ConfigureEncryption(IBaseFilter muxer, string password)
{
    var cryptoConfig = muxer as IVFCryptoConfig;
    if (cryptoConfig == null)
    {
        Console.WriteLine("ERROR: Filter does not support IVFCryptoConfig");
        return false;
    }
    try
    {
        // Set password
        cryptoConfig.ApplyString(password);
        // Verify
        int hr = cryptoConfig.HavePassword();
        if (hr != 0)
        {
            Console.WriteLine("ERROR: Password not set correctly");
            return false;
        }
        Console.WriteLine("Encryption configured successfully");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: Failed to configure encryption: {ex.Message}");
        return false;
    }
}
```
---

### Common Issues

#### "Filter not registered" Error

**Problem**: CLSID not found

**Solution**:
```batch
REM Register filters (run as Administrator)
REM VisioForge_Encryptor_v9 holds both the v9 encryptor and the v9 decryptor the samples use
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Video Encryption SDK\Redist\VisioForge_Encryptor_v9_x64.ax"
REM The H.264 and AAC encoders the samples use (VFH264EncoderV10, VFAACEncoderV10)
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Video Encryption SDK\Redist\VisioForge_H264_Encoder_x64.ax"
regsvr32 "C:\Program Files (x86)\VisioForge\VisioForge Video Encryption SDK\Redist\VisioForge_AAC_Encoder_v10_x64.ax"
REM 32-bit applications: the same files without _x64
```

#### "Interface not available" Error

**Problem**: Cannot query IVFCryptoConfig

**Solution**:

- Verify filter version is correct
- Check filter is properly registered
- Ensure COM interfaces are compatible

#### Decryption Fails with Garbled Video

**Problem**: Wrong password used for decryption

**Solution**:

- Verify same password is used for encryption and decryption
- Check password encoding (Unicode vs. ANSI)
- Ensure no typos in password

#### Output File is Empty or Corrupt

**Problem**: Encoding failed silently

**Solution**:

- Connect the encryptor's output to a File Writer and set the file name on the File Writer: the encryptor itself has no `IFileSinkFilter`
- Check all encoder filters are properly connected
- Verify encoder configurations are valid
- Monitor IMediaEventEx for error events
- Check disk space is available

---
## See Also

- [Video Encryption SDK Overview](index.md) - Product features
- [Interface Reference](interface-reference.md) - Complete API documentation
- [DirectShow Encoding Filters Pack](../filters-enc/index.md) - Compatible encoders
- [GitHub Samples](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) - Complete source code