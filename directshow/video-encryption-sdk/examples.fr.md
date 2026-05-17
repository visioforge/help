---
title: Exemples de chiffrement vidéo DirectShow — C#, C++, Delphi
description: Chiffrez et déchiffrez des fichiers vidéo MP4 via les filtres DirectShow VisioForge. Exemples AES-256 avec mot de passe et clés binaires.
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

# Video Encryption SDK — Exemples de code

## Vue d'ensemble

Cette page fournit des exemples de code complets et opérationnels pour chiffrer et déchiffrer des fichiers vidéo avec le Video Encryption SDK. Les exemples couvrent :

- **Chiffrement de base** — chiffrer des fichiers vidéo avec protection par mot de passe
- **Déchiffrement de fichier** — déchiffrer et lire des fichiers vidéo chiffrés
- **Scénarios avancés** — clés binaires, clés basées sur fichier, paramètres d'encodage personnalisés
- **Gestion d'erreurs** — vérification d'erreurs et récupération robustes

Tous les exemples incluent des implémentations complètes en C++, C# et Delphi.

!!! note "Nommage de l'interface selon les wrappers de langage"
    L'en-tête natif C++ (`encryptor_intf.h`) déclare cette interface sous le nom `ICryptoConfig` avec `IID_ICryptoConfig`. Les wrappers C# et Delphi exposent la même interface sous `IVFCryptoConfig` (et le fournisseur de mot de passe sous `IVFPasswordProvider`). Les deux noms partagent le GUID `{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}` et désignent la même interface COM — les extraits C++ ci-dessous utilisent `ICryptoConfig` tandis que les extraits C#/Delphi utilisent `IVFCryptoConfig`.

---
## Prérequis
### Projets C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "encryptor_intf.h"
#pragma comment(lib, "strmiids.lib")
#pragma comment(lib, "quartz.lib")
```
### Projets C#
```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
```
**Paquets NuGet requis** :
- VisioForge.DirectShowAPI
- VisioForge.DirectShowLib
### Projets Delphi
```delphi
uses
  DirectShow9,
  EncryptorIntf;
```
---

## Exemple 1 : chiffrement vidéo de base

Chiffrez un fichier vidéo avec un mot de passe textuel.

### Implémentation C#

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
            // Creer le graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            mediaEvent = (IMediaEventEx)filterGraph;

            // Ajouter le filtre source
            int hr = filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
            DsError.ThrowExceptionForHR(hr);

            // Ajouter l'encodeur video (H.264)
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFH264Encoder,
                "H.264 Encoder"
            );

            // Ajouter l'encodeur audio (AAC)
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFAACEncoder,
                "AAC Encoder"
            );

            // Ajouter le multiplexeur de chiffrement
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
            );

            hr = filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");
            DsError.ThrowExceptionForHR(hr);

            // Configurer le mot de passe de chiffrement
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Appliquer le mot de passe via la methode utilitaire
                cryptoConfig.ApplyString(password);

                // Verifier que le mot de passe est defini
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

            // Definir le fichier de sortie
            var fileSink = encryptMuxer as IFileSinkFilter;
            if (fileSink != null)
            {
                hr = fileSink.SetFileName(outputFile, null);
                DsError.ThrowExceptionForHR(hr);
            }

            // Construire les connexions du graphe de filtres
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Connecter le chemin video : Source -> Encodeur H.264 -> Multiplexeur de chiffrement
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                sourceFilter,
                videoEncoder,
                encryptMuxer
            );
            DsError.ThrowExceptionForHR(hr);

            // Connecter le chemin audio : Source -> Encodeur AAC -> Multiplexeur de chiffrement
            hr = captureGraph.RenderStream(
                null,
                MediaType.Audio,
                sourceFilter,
                audioEncoder,
                encryptMuxer
            );
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine("Filter graph built successfully");
            Console.WriteLine("Starting encryption...");

            // Demarrer l'encodage
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            // Attendre la fin
            EventCode eventCode;
            do
            {
                hr = mediaEvent.WaitForCompletion(1000, out eventCode);
            }
            while (eventCode == 0);

            Console.WriteLine("Encryption completed successfully!");

            // Nettoyage
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

// Utilisation :
// var encryptor = new BasicVideoEncryption();
// encryptor.EncryptVideo(@"C:\input.mp4", @"C:\output.encrypted.mp4", "MySecurePassword123");
```

### Implémentation C++

```cpp
#include <dshow.h>
#include <streams.h>
#include "encryptor_intf.h"

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

        // Creer le graphe de filtres
        hr = CoCreateInstance(
            CLSID_FilterGraph,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IGraphBuilder,
            (void**)&pGraph
        );
        if (FAILED(hr)) return hr;

        // Recuperer les interfaces de controle et d'evenements
        pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
        pGraph->QueryInterface(IID_IMediaEvent, (void**)&pEvent);

        // Ajouter le filtre source
        IBaseFilter* pSource = NULL;
        hr = pGraph->AddSourceFilter(inputFile, L"Source", &pSource);
        if (FAILED(hr)) goto cleanup;

        // Ajouter l'encodeur video (H.264)
        IBaseFilter* pVideoEncoder = NULL;
        hr = CoCreateInstance(
            CLSID_VFH264Encoder,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IBaseFilter,
            (void**)&pVideoEncoder
        );
        if (FAILED(hr)) goto cleanup;
        hr = pGraph->AddFilter(pVideoEncoder, L"H.264 Encoder");

        // Ajouter l'encodeur audio (AAC)
        IBaseFilter* pAudioEncoder = NULL;
        hr = CoCreateInstance(
            CLSID_VFAACEncoder,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IBaseFilter,
            (void**)&pAudioEncoder
        );
        if (FAILED(hr)) goto cleanup;
        hr = pGraph->AddFilter(pAudioEncoder, L"AAC Encoder");

        // Ajouter le multiplexeur de chiffrement
        IBaseFilter* pMuxer = NULL;
        hr = CoCreateInstance(
            CLSID_EncryptMuxer,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_IBaseFilter,
            (void**)&pMuxer
        );
        if (FAILED(hr)) goto cleanup;
        hr = pGraph->AddFilter(pMuxer, L"Encrypt Muxer");

        // Configurer le chiffrement
        ICryptoConfig* pCrypto = NULL;
        hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
        if (SUCCEEDED(hr))
        {
            // Definir le mot de passe
            hr = pCrypto->put_Password(
                (LPBYTE)password,
                wcslen(password) * sizeof(wchar_t)
            );

            if (SUCCEEDED(hr))
            {
                // Verifier que le mot de passe est defini
                HRESULT hrPassword = pCrypto->HavePassword();
                if (hrPassword == S_OK)
                {
                    wprintf(L"Encryption password configured successfully\n");
                }
                else
                {
                    wprintf(L"WARNING: Password not set\n");
                }
            }

            pCrypto->Release();
        }

        // Definir le fichier de sortie
        IFileSinkFilter* pFileSink = NULL;
        hr = pMuxer->QueryInterface(IID_IFileSinkFilter, (void**)&pFileSink);
        if (SUCCEEDED(hr))
        {
            hr = pFileSink->SetFileName(outputFile, NULL);
            pFileSink->Release();
        }

        // Construire le graphe via Intelligent Connect
        ICaptureGraphBuilder2* pBuilder = NULL;
        hr = CoCreateInstance(
            CLSID_CaptureGraphBuilder2,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_ICaptureGraphBuilder2,
            (void**)&pBuilder
        );
        if (SUCCEEDED(hr))
        {
            pBuilder->SetFiltergraph(pGraph);

            // Connecter le chemin video
            pBuilder->RenderStream(
                NULL,
                &MEDIATYPE_Video,
                pSource,
                pVideoEncoder,
                pMuxer
            );

            // Connecter le chemin audio
            pBuilder->RenderStream(
                NULL,
                &MEDIATYPE_Audio,
                pSource,
                pAudioEncoder,
                pMuxer
            );

            pBuilder->Release();
        }

        wprintf(L"Starting encryption...\n");

        // Demarrer le graphe
        hr = pControl->Run();
        if (FAILED(hr)) goto cleanup;

        // Attendre la fin
        long evCode;
        pEvent->WaitForCompletion(INFINITE, &evCode);

        wprintf(L"Encryption completed successfully!\n");

cleanup:
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

// Utilisation :
// BasicVideoEncryption encryptor;
// encryptor.EncryptVideo(L"C:\\input.mp4", L"C:\\output.encrypted.mp4", L"MySecurePassword123");
```

### Implémentation Delphi

```delphi
uses
  DirectShow9,
  ActiveX,
  EncryptorIntf;

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

function TBasicVideoEncryption.EncryptVideo(const InputFile, OutputFile, Password: WideString): HRESULT;
var
  SourceFilter: IBaseFilter;
  VideoEncoder: IBaseFilter;
  AudioEncoder: IBaseFilter;
  EncryptMuxer: IBaseFilter;
  CryptoConfig: IVFCryptoConfig;
  FileSink: IFileSinkFilter;
  CaptureGraph: ICaptureGraphBuilder2;
  EventCode: Integer;
begin
  Result := E_FAIL;

  try
    // Creer le graphe de filtres
    Result := CoCreateInstance(CLSID_FilterGraph, nil, CLSCTX_INPROC_SERVER,
                               IID_IGraphBuilder, FFilterGraph);
    if Failed(Result) then Exit;

    // Recuperer les interfaces de controle et d'evenements
    FFilterGraph.QueryInterface(IID_IMediaControl, FMediaControl);
    FFilterGraph.QueryInterface(IID_IMediaEvent, FMediaEvent);

    // Ajouter le filtre source
    Result := FFilterGraph.AddSourceFilter(PWideChar(InputFile), 'Source', SourceFilter);
    if Failed(Result) then Exit;

    // Ajouter l'encodeur video (H.264)
    Result := CoCreateInstance(CLSID_VFH264Encoder, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, VideoEncoder);
    if Succeeded(Result) then
      FFilterGraph.AddFilter(VideoEncoder, 'H.264 Encoder');

    // Ajouter l'encodeur audio (AAC)
    Result := CoCreateInstance(CLSID_VFAACEncoder, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, AudioEncoder);
    if Succeeded(Result) then
      FFilterGraph.AddFilter(AudioEncoder, 'AAC Encoder');

    // Ajouter le multiplexeur de chiffrement
    Result := CoCreateInstance(CLSID_EncryptMuxer, nil, CLSCTX_INPROC_SERVER,
                               IID_IBaseFilter, EncryptMuxer);
    if Failed(Result) then Exit;
    FFilterGraph.AddFilter(EncryptMuxer, 'Encrypt Muxer');

    // Configurer le chiffrement
    if Supports(EncryptMuxer, IVFCryptoConfig, CryptoConfig) then
    begin
      // Definir le mot de passe
      // pBuffer est un tampon binaire opaque (PByte) ; on caste via PWideChar pour reutiliser
      // le tampon de chaine large, longueur en octets = nombre de caracteres * 2.
      Result := CryptoConfig.put_Password(PByte(PWideChar(Password)), Length(Password) * 2);

      if Succeeded(Result) then
      begin
        // Verifier que le mot de passe est defini
        if CryptoConfig.HavePassword = S_OK then
          WriteLn('Encryption password configured successfully')
        else
          WriteLn('WARNING: Password not set');
      end;
    end;

    // Definir le fichier de sortie
    if Supports(EncryptMuxer, IFileSinkFilter, FileSink) then
      FileSink.SetFileName(PWideChar(OutputFile), nil);

    // Construire les connexions du graphe
    Result := CoCreateInstance(CLSID_CaptureGraphBuilder2, nil, CLSCTX_INPROC_SERVER,
                               IID_ICaptureGraphBuilder2, CaptureGraph);
    if Succeeded(Result) then
    begin
      CaptureGraph.SetFiltergraph(FFilterGraph);

      // Connecter le chemin video
      CaptureGraph.RenderStream(nil, @MEDIATYPE_Video, SourceFilter,
                                VideoEncoder, EncryptMuxer);

      // Connecter le chemin audio
      CaptureGraph.RenderStream(nil, @MEDIATYPE_Audio, SourceFilter,
                                AudioEncoder, EncryptMuxer);
    end;

    WriteLn('Starting encryption...');

    // Demarrer le graphe
    Result := FMediaControl.Run;
    if Failed(Result) then Exit;

    // Attendre la fin
    repeat
      FMediaEvent.WaitForCompletion(1000, EventCode);
    until EventCode <> 0;

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

// Utilisation :
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
## Exemple 2 : déchiffrement et lecture vidéo {#example-2-decryption}

Déchiffrez un fichier vidéo chiffré et lisez-le.

### Implémentation C# {#example-2-csharp}

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
            // Creer le graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            mediaEvent = (IMediaEventEx)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;
            // Ajouter le demultiplexeur de dechiffrement
            var decryptDemuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("D2C761F0-9988-4f79-9B0E-FB2B79C65851"))
            );
            int hr = filterGraph.AddFilter(decryptDemuxer, "Decrypt Demuxer");
            DsError.ThrowExceptionForHR(hr);
            // Configurer le dechiffrement (DOIT utiliser le meme mot de passe que pour le chiffrement)
            var cryptoConfig = decryptDemuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Appliquer le mot de passe
                cryptoConfig.ApplyString(password);
                // Verifier que le mot de passe est defini
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
            // Charger le fichier chiffre
            var fileSource = decryptDemuxer as IFileSourceFilter;
            if (fileSource != null)
            {
                hr = fileSource.Load(encryptedFile, null);
                DsError.ThrowExceptionForHR(hr);
            }
            // Construire le graphe - rendre les sorties video et audio
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);
            // Rendre la sortie video
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                decryptDemuxer,
                null,
                null
            );
            DsError.ThrowExceptionForHR(hr);
            // Rendre la sortie audio
            hr = captureGraph.RenderStream(
                null,
                MediaType.Audio,
                decryptDemuxer,
                null,
                null
            );
            // L'audio est optionnel, ne pas echouer s'il est absent
            // Configurer la fenetre video
            hr = videoWindow.put_Owner(windowHandle);
            hr = videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            hr = videoWindow.put_MessageDrain(windowHandle);
            Console.WriteLine("Starting playback...");
            // Demarrer la lecture
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
// Utilisation :
// var decryptor = new VideoDecryption();
// decryptor.DecryptAndPlay(@"C:\output.encrypted.mp4", "MySecurePassword123", this.Handle);
```
---

## Exemple 3 : utiliser un fichier comme clé de chiffrement

Utilisez le contenu d'un fichier comme clé de chiffrement au lieu d'un mot de passe.

### Implémentation C#

```csharp
public class FileKeyEncryption
{
    public void EncryptWithFileKey(string inputFile, string outputFile, string keyFile)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var mediaControl = (IMediaControl)filterGraph;

        try
        {
            // Configurer le graphe de filtres (source, encodeurs, multiplexeur)
            // ... (identique a l'exemple 1)

            // Ajouter le multiplexeur de chiffrement
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
            );
            filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");

            // Configurer le chiffrement en utilisant le fichier comme cle
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Utiliser le contenu du fichier comme cle de chiffrement (hash SHA-256 du fichier)
                cryptoConfig.ApplyFile(keyFile);

                Console.WriteLine($"Using key file: {keyFile}");
                Console.WriteLine("File content hashed with SHA-256 for encryption");

                // Verifier
                int hr = cryptoConfig.HavePassword();
                if (hr == 0)
                {
                    Console.WriteLine("Encryption key configured successfully");
                }
            }

            // Poursuivre la configuration et l'encodage du graphe de filtres...
        }
        finally
        {
            // Nettoyage
        }
    }

    public void DecryptWithFileKey(string encryptedFile, string keyFile)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();

        try
        {
            // Ajouter le demultiplexeur de dechiffrement
            var decryptDemuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("D2C761F0-9988-4f79-9B0E-FB2B79C65851"))
            );
            filterGraph.AddFilter(decryptDemuxer, "Decrypt Demuxer");

            // Configurer le dechiffrement en utilisant le meme fichier de cle
            var cryptoConfig = decryptDemuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // DOIT utiliser le meme fichier de cle que pour le chiffrement
                cryptoConfig.ApplyFile(keyFile);

                Console.WriteLine($"Using key file for decryption: {keyFile}");
            }

            // Poursuivre la configuration de la lecture...
        }
        finally
        {
            // Nettoyage
        }
    }
}

// Utilisation :
// var encryptor = new FileKeyEncryption();
// encryptor.EncryptWithFileKey(@"C:\input.mp4", @"C:\output.encrypted.mp4", @"C:\keys\encryption.key");
// encryptor.DecryptWithFileKey(@"C:\output.encrypted.mp4", @"C:\keys\encryption.key");
```

---
## Exemple 4 : utilisation de données de clé binaires
Générez et utilisez des données de clé binaires pour le chiffrement.
### Implémentation C# {#example-4-csharp}

<!-- code block -->
```csharp
using System.Security.Cryptography;
using System.IO;
public class BinaryKeyEncryption
{
    public byte[] GenerateRandomKey(int keySize = 32)
    {
        // Generer une cle aleatoire cryptographiquement sure
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
            // Configurer le graphe de filtres et ajouter le multiplexeur de chiffrement
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
            );
            filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");
            // Configurer le chiffrement avec une cle binaire
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                // Appliquer la cle binaire (sera hachee en SHA-256)
                cryptoConfig.ApplyBinary(keyData);
                Console.WriteLine($"Binary key applied (length: {keyData.Length} bytes)");
                // Verifier
                int hr = cryptoConfig.HavePassword();
                if (hr == 0)
                {
                    Console.WriteLine("Encryption key configured successfully");
                }
            }
            // Poursuivre l'encodage...
        }
        finally
        {
            // Nettoyage
        }
    }
    public void CompleteWorkflow()
    {
        // Generer une cle de chiffrement aleatoire
        byte[] encryptionKey = GenerateRandomKey(32);
        Console.WriteLine($"Generated {encryptionKey.Length * 8}-bit encryption key");
        // Sauvegarder la cle de maniere securisee
        string keyFile = @"C:\keys\video_encryption_key.bin";
        SaveKeyToFile(encryptionKey, keyFile);
        // Chiffrer la video avec la cle binaire
        EncryptWithBinaryKey(
            @"C:\input.mp4",
            @"C:\output.encrypted.mp4",
            encryptionKey
        );
        Console.WriteLine("Video encrypted successfully!");
        Console.WriteLine($"Key file: {keyFile}");
        Console.WriteLine("Keep this key file secure - it's required for decryption!");
        // Plus tard, pour le dechiffrement :
        // byte[] key = LoadKeyFromFile(keyFile);
        // DecryptWithBinaryKey(@"C:\output.encrypted.mp4", key);
    }
}
// Utilisation :
// var encryptor = new BinaryKeyEncryption();
// encryptor.CompleteWorkflow();
```
---

## Exemple 5 : chiffrement avec paramètres d'encodage personnalisés

Chiffrez la vidéo avec des paramètres d'encodage H.264 spécifiques.

### Implémentation C#

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
            // Ajouter la source
            filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

            // Ajouter et configurer l'encodeur H.264
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFH264Encoder,
                "H.264 Encoder"
            );

            // Configurer l'encodeur H.264 (si l'interface est disponible)
            var h264Config = videoEncoder as IH264Encoder;
            if (h264Config != null)
            {
                h264Config.put_Bitrate(videoBitrate);
                h264Config.put_Profile(77); // Profil Main
                h264Config.put_Level(41);   // Niveau 4.1 (1080p)
                h264Config.put_RateControl(1); // CBR
                h264Config.put_GOP(60);     // 2 secondes a 30 fps

                Console.WriteLine($"H.264 configured: {videoBitrate / 1000000.0} Mbps, Main profile, Level 4.1");
            }

            // Ajouter et configurer l'encodeur AAC
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFAACEncoder,
                "AAC Encoder"
            );

            // Configurer l'encodeur AAC (si l'interface est disponible)
            var aacConfig = audioEncoder as IVFAACEncoder;
            if (aacConfig != null)
            {
                aacConfig.SetBitrate((uint)audioBitrate);
                aacConfig.SetProfile(2); // AAC-LC
                aacConfig.SetOutputFormat(0); // AAC brut pour MP4

                Console.WriteLine($"AAC configured: {audioBitrate / 1000} kbps, AAC-LC profile");
            }

            // Ajouter le multiplexeur de chiffrement
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
            );
            filterGraph.AddFilter(encryptMuxer, "Encrypt Muxer");

            // Configurer le chiffrement
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig != null)
            {
                cryptoConfig.ApplyString(password);
                Console.WriteLine("Encryption configured");
            }

            // Definir le fichier de sortie
            var fileSink = encryptMuxer as IFileSinkFilter;
            fileSink?.SetFileName(outputFile, null);

            // Construire les connexions
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            captureGraph.SetFiltergraph(filterGraph);

            captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoder, encryptMuxer);
            captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoder, encryptMuxer);

            Console.WriteLine("Starting encoding with custom settings...");

            // Lancer l'encodage
            int hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            // Attendre la fin
            var mediaEvent = (IMediaEventEx)filterGraph;
            EventCode eventCode;
            do
            {
                hr = mediaEvent.WaitForCompletion(1000, out eventCode);
            }
            while (eventCode == 0);

            Console.WriteLine("Encoding and encryption completed!");

            Marshal.ReleaseComObject(captureGraph);
        }
        finally
        {
            // Nettoyage
            mediaControl?.Stop();
            FilterGraphTools.RemoveAllFilters(filterGraph);
        }
    }
}

// Utilisation :
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

## Exemple 6 : gestion d'erreurs et validation {#example-6-error-handling}

Gestion d'erreurs complète pour le chiffrement/déchiffrement.

### Implémentation C# {#example-6-csharp}

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
            // Valider les entrees
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
            // Creer le graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            if (filterGraph == null)
            {
                Console.WriteLine("ERROR: Failed to create filter graph");
                return EncryptionResult.FilterGraphError;
            }
            mediaControl = (IMediaControl)filterGraph;
            // Ajouter le filtre source
            int hr = filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
            if (hr != 0 || sourceFilter == null)
            {
                Console.WriteLine($"ERROR: Failed to add source filter (HRESULT: 0x{hr:X8})");
                return EncryptionResult.FilterGraphError;
            }
            // Ajouter les encodeurs
            var videoEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFH264Encoder,
                "H.264 Encoder"
            );
            if (videoEncoder == null)
            {
                Console.WriteLine("ERROR: Failed to create video encoder");
                return EncryptionResult.FilterGraphError;
            }
            var audioEncoder = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFAACEncoder,
                "AAC Encoder"
            );
            // Ajouter le multiplexeur de chiffrement
            var encryptMuxer = (IBaseFilter)Activator.CreateInstance(
                Type.GetTypeFromCLSID(new Guid("F1D3727A-88DE-49ab-A635-280BEFEFF902"))
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
            // Configurer le chiffrement
            var cryptoConfig = encryptMuxer as IVFCryptoConfig;
            if (cryptoConfig == null)
            {
                Console.WriteLine("ERROR: IVFCryptoConfig interface not available");
                return EncryptionResult.InterfaceNotAvailable;
            }
            try
            {
                cryptoConfig.ApplyString(password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to apply password: {ex.Message}");
                return EncryptionResult.InvalidPassword;
            }
            // Verifier que le mot de passe est defini
            hr = cryptoConfig.HavePassword();
            if (hr != 0)
            {
                Console.WriteLine("ERROR: Password verification failed");
                return EncryptionResult.InvalidPassword;
            }
            Console.WriteLine("Encryption password configured successfully");
            // Definir le fichier de sortie
            var fileSink = encryptMuxer as IFileSinkFilter;
            if (fileSink != null)
            {
                hr = fileSink.SetFileName(outputFile, null);
                if (hr != 0)
                {
                    Console.WriteLine($"ERROR: Failed to set output file (HRESULT: 0x{hr:X8})");
                    return EncryptionResult.FilterGraphError;
                }
            }
            // Construire le graphe de filtres
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            hr = captureGraph.RenderStream(
                null, MediaType.Video,
                sourceFilter, videoEncoder, encryptMuxer
            );
            if (hr != 0)
            {
                Console.WriteLine($"WARNING: Video connection failed (HRESULT: 0x{hr:X8})");
            }
            hr = captureGraph.RenderStream(
                null, MediaType.Audio,
                sourceFilter, audioEncoder, encryptMuxer
            );
            if (hr != 0)
            {
                Console.WriteLine($"WARNING: Audio connection failed (HRESULT: 0x{hr:X8}) - continuing without audio");
            }
            Console.WriteLine("Filter graph built successfully");
            Console.WriteLine("Starting encryption...");
            // Demarrer l'encodage
            hr = mediaControl.Run();
            if (hr != 0)
            {
                Console.WriteLine($"ERROR: Failed to start encoding (HRESULT: 0x{hr:X8})");
                return EncryptionResult.EncodingError;
            }
            // Suivre la progression
            var mediaEvent = (IMediaEventEx)filterGraph;
            EventCode eventCode;
            long param1, param2;
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
                    else if (eventCode == EventCode.ErrorAbort)
                    {
                        Console.WriteLine($"ERROR: Encoding aborted (event code: {eventCode})");
                        return EncryptionResult.EncodingError;
                    }
                }
            }
            while (true);
            Console.WriteLine("Encryption completed successfully!");
            // Verifier que le fichier de sortie existe
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
// Utilisation :
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

## Bonnes pratiques

### Sécurité des mots de passe

1. **Utiliser des mots de passe forts**
   - Minimum 16 caractères
   - Mélange de majuscules, minuscules, chiffres, symboles
   - Éviter les mots du dictionnaire
   - Utiliser un gestionnaire de mots de passe pour générer/stocker

2. **Stockage des clés**
   - Ne jamais coder les mots de passe en dur dans le code source
   - Utiliser un stockage sécurisé (Windows DPAPI, KeyVault, etc.)
   - Mettre en place des contrôles d'accès appropriés

3. **Distribution des clés**
   - Utiliser des canaux sécurisés pour la distribution des clés
   - Envisager le chiffrement à clé publique pour l'échange de clés
   - Mettre en place des politiques de rotation des clés

### Sécurité de l'implémentation

**BON : gestion sécurisée du mot de passe**

```csharp
public void SetEncryptionPassword(IVFCryptoConfig config)
{
    // Recuperer le mot de passe depuis le stockage securise
    string password = SecureStorage.GetPassword();

    try
    {
        config.ApplyString(password);
    }
    finally
    {
        // Effacer le mot de passe de la memoire
        password = null;
        GC.Collect();
    }
}
```

**MAUVAIS : mot de passe codé en dur**

```csharp
public void SetEncryptionPassword(IVFCryptoConfig config)
{
    config.ApplyString("MyPassword123"); // A NE PAS FAIRE !
}
```

### Sécurité des fichiers

- Utilisez des permissions de fichier appropriées pour les fichiers chiffrés
- Mettez en place une suppression sécurisée des fichiers temporaires
- Envisagez de chiffrer les fichiers de clés avec un chiffrement supplémentaire
- Journalisez les accès au contenu chiffré pour la traçabilité

### Lignes directrices d'implémentation

1. **Toujours valider les entrées**
   - Vérifier l'existence du fichier avant traitement
   - Valider que le mot de passe n'est pas vide
   - Vérifier la disponibilité des interfaces de filtre

2. **Gestion d'erreurs**
   - Vérifier les valeurs HRESULT des appels COM
   - Utiliser des blocs try-catch pour les exceptions
   - Fournir des messages d'erreur significatifs

3. **Gestion des ressources**
   - Toujours libérer les objets COM
   - Utiliser des blocs `finally` pour le nettoyage
   - Arrêter le graphe de filtres avant la libération

4. **Tests**
   - Tester avec divers formats d'entrée
   - Vérifier que les fichiers chiffrés peuvent être déchiffrés
   - Tester les conditions d'erreur (mauvais mot de passe, etc.)

---
## Dépannage
### Codes d'erreur courants
| HRESULT | Description | Solution |
|---------|-------------|----------|
| `E_INVALIDARG` | Tampon ou taille invalide | Vérifier le pointeur de tampon et les paramètres de taille |
| `E_POINTER` | Pointeur null | S'assurer que les pointeurs sont valides avant l'appel |
| `E_FAIL` | Échec général | Vérifier l'état et la configuration du filtre |
| `S_FALSE` | Aucun mot de passe défini | Appeler `put_Password` avant `HavePassword` |
### Exemple de gestion d'erreurs
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
        // Definir le mot de passe
        cryptoConfig.ApplyString(password);
        // Verifier
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

### Problèmes courants

#### Erreur « Filter not registered »

**Problème** : CLSID introuvable

**Solution** :
```bash
# Enregistrer les filtres (executer en tant qu'administrateur)
regsvr32 "C:\Path\To\EncryptMuxer.ax"
regsvr32 "C:\Path\To\DecryptDemuxer.ax"
```

#### Erreur « Interface not available »

**Problème** : impossible d'interroger IVFCryptoConfig

**Solution** :
- Vérifier que la version du filtre est correcte
- Vérifier que le filtre est correctement enregistré
- S'assurer que les interfaces COM sont compatibles

#### Le déchiffrement échoue avec une vidéo brouillée

**Problème** : mauvais mot de passe utilisé pour le déchiffrement

**Solution** :
- Vérifier que le même mot de passe est utilisé pour le chiffrement et le déchiffrement
- Vérifier l'encodage du mot de passe (Unicode vs ANSI)
- S'assurer qu'il n'y a pas de fautes de frappe dans le mot de passe

#### Le fichier de sortie est vide ou corrompu

**Problème** : l'encodage a échoué silencieusement

**Solution** :
- Vérifier que tous les filtres encodeurs sont correctement connectés
- Vérifier la validité des configurations d'encodeur
- Surveiller IMediaEventEx pour les événements d'erreur
- Vérifier la disponibilité d'espace disque

---
## Voir aussi
- [Vue d'ensemble du Video Encryption SDK](index.md) — fonctionnalités du produit
- [Référence des interfaces](interface-reference.md) — documentation API complète
- [Pack de filtres d'encodage DirectShow](../filters-enc/index.md) — encodeurs compatibles
- [Exemples GitHub](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) — code source complet
