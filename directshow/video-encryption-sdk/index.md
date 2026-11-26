---
title: AES-256 Video Encryption Using DirectShow COM Filters
description: Encrypt H.264/AAC MP4 video with AES-256 using DirectShow filters. VisioForge SDK with password and binary key modes for C++, C#, and Delphi.
sidebar_label: Video Encryption SDK
order: 5
tags:
  - Video Encryption SDK
  - DirectShow
  - C++
  - Windows
primary_api_classes:
  - IVFCryptoConfig
  - IVFPasswordProvider
video:
  id: N71nzfSry-g
  upload_date: 2026-11-26T14:36:42Z
  duration: PT6M6S

---

# Video Encryption SDK

## Introduction to Video Encryption

The [Video Encryption SDK](https://www.visioforge.com/video-encryption-sdk) provides robust tools for encoding video files into MP4 H264/AAC format with advanced encryption capabilities. Developers can secure their media content using custom passwords or binary data encryption methods.

The SDK integrates seamlessly with any DirectShow application through a complete set of filters. These filters come with extensive interfaces allowing developers to fine-tune settings according to specific security requirements and implementation needs.


## Video tutorial

The video builds the C# demos in Visual Studio, walks through their filter graphs, encrypts an MP4 file with AES-256 and plays it back with the same password:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/N71nzfSry-g?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

---

## Installation

Before using the code samples and integrating the SDK into your application, you must first install the Video Encryption SDK from the [product page](https://www.visioforge.com/video-encryption-sdk).

**Installation Steps**:

1. Download the SDK installer from the product page
2. Run the installer with administrative privileges
3. The installer will register all necessary DirectShow filters
4. Sample applications and source code will be available in the installation directory

**Note**: The SDK filters must be properly registered on the system before they can be used in your applications. The installer handles this automatically.

---

## Integration Flexibility

You can implement the SDK in various DirectShow applications as filters for both encryption and decryption processes. The system works effectively with:

- Live video sources
- File-based video sources
- Software video encoders
- GPU-accelerated video encoders from the [DirectShow Encoding Filters pack](https://www.visioforge.com/encoding-filters-pack) (available separately)
- Third-party DirectShow filters for additional video encoding options

## Key Features and Capabilities

### Core Functionality

- **Secure Encryption/Decryption**: Process video files or capture streams with robust security algorithms
- **Format Support**: Full H264 encoder support for video content
- **Audio Handling**: Complete AAC encoder support for audio streams
- **Flexible Security Options**: Implement encryption using either binary data or string passwords

### Performance Optimization

- AES-256 encryption engine for maximum security
- CPU hardware acceleration support
- GPU acceleration compatibility
- Optimized for high-speed encryption processes

## Development Resources

### Code Samples and Documentation

The SDK includes comprehensive code samples for multiple programming languages:

- C# implementation examples
- C++ reference code
- Delphi sample projects

These samples provide practical implementation guidance for developers building secure video applications.

### Demo Application

Explore the included Video Encryptor application for a hands-on demonstration of the SDK's capabilities in a working environment.

---

## API Reference

### Core Interfaces

The SDK provides comprehensive COM interfaces for encryption and decryption:

#### IVFCryptoConfig

Primary interface for configuring encryption settings on the muxer filter (encryption) and demuxer filter (decryption).

**GUID**: `{BAA5BD1E-3B30-425e-AB3B-CC20764AC253}`

**Methods**:
- `put_Provider` - Set password provider for advanced scenarios (binary keys, dynamic passwords)
- `get_Provider` - Get password provider interface
- `put_Password` - Set encryption password or key directly (binary data)
- `HavePassword` - Check if password is set

#### IVFPasswordProvider

Callback interface for advanced password provision scenarios such as binary data keys, dynamic password generation, or custom key derivation.

**GUID**: `{6F8162B5-778D-42b5-9242-1BBABB24FFC4}`

**Methods**:
- `QueryPassword` - Query password for specific file

**Use Cases**:
- Binary key data provision
- Dynamic password generation
- File-specific encryption keys
- Custom key derivation functions

#### Helper Classes

The SDK includes helper extension methods for .NET developers:
- `ApplyString` - Apply string password (hashed with SHA-256)
- `ApplyFile` - Use file as encryption key (SHA-256 hash of file content)
- `ApplyBinary` - Apply binary key data (hashed with SHA-256)

### Filter CLSIDs

| Filter | CLSID | Purpose |
|--------|-------|---------|
| **VisioForge Encryptor v9** | `{86D83D2E-70D2-4A32-B7BE-E94A08AA7BAA}` | Muxer with AES-256 encryption (version 9 files) |
| **VisioForge Decryptor v9** | `{4B89F5E2-2BA9-4C8F-A78A-EA2701AB582C}` | Demuxer with AES-256 decryption (version 9 files) |
| **VisioForge Encryptor v8** | `{F1D3727A-88DE-49ab-A635-280BEFEFF902}` | Muxer with AES-128 encryption (version 8 files) |
| **VisioForge Decryptor v8** | `{D2C761F0-9988-4f79-9B0E-FB2B79C65851}` | Demuxer with AES-128 decryption (version 8 files) |

For detailed interface documentation and code examples, see the [Interface Reference](interface-reference.md).

---

## Code Examples

### Quick Start - Encryption

#### C# Example

```csharp
using VisioForge.DirectShowAPI;

// Get crypto config interface from encrypt muxer
var cryptoConfig = muxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Apply string password
    cryptoConfig.ApplyString("MySecurePassword123");

    // Or use file as key
    // cryptoConfig.ApplyFile(@"C:\keys\mykey.bin");

    // Or use binary data
    // byte[] keyData = new byte[] { 0x01, 0x02, 0x03, ... };
    // cryptoConfig.ApplyBinary(keyData);
}
```

#### C++ Example

```cpp
#include "encryptor_intf.h"

ICryptoConfig* pCrypto = nullptr;
hr = pMuxer->QueryInterface(IID_ICryptoConfig, (void**)&pCrypto);
if (SUCCEEDED(hr))
{
    // Set password
    const wchar_t* password = L"MySecurePassword123";
    hr = pCrypto->put_Password(
        (LPBYTE)password,
        wcslen(password) * sizeof(wchar_t)
    );

    pCrypto->Release();
}
```

### Decryption

#### C# Example

```csharp
// Get crypto config interface from decrypt demuxer
var cryptoConfig = demuxerFilter as IVFCryptoConfig;
if (cryptoConfig != null)
{
    // Must use same password/key as encryption
    cryptoConfig.ApplyString("MySecurePassword123");
}
```

For complete examples including filter graph setup, see the [Examples Page](examples.md).


## Encrypt and play a file in C\#

The [C# demos](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK/dotnet) build two DirectShow graphs: the Encryption Demo writes an encrypted file, and the Player Demo plays it back. Both are WinForms apps on .NET 8 for x64, and they reach DirectShow through the [VisioForge.DirectShowAPI](https://www.nuget.org/packages/VisioForge.DirectShowAPI) NuGet package. They read MP4 files through LAV Filters, which the Video Encryption SDK installer includes. The code below is based on the demos.

### Add the filters

The encryptor filter takes the H.264 video and the AAC audio and writes them into one encrypted file through the File Writer. The version 9 encryptor writes AES-256 files, the version 8 encryptor AES-128 files.

```csharp
// AddEncryptor() in the demo
// Encryptor v9 writes AES-256 files; Consts.CLSID_VFVideoEncryptor8 writes AES-128 files
muxFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    new Guid(Consts.CLSID_VFVideoEncryptor9),
    "VisioForge Encryptor v9");

videoEncFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    DSFilterInitInfoConsts.VFH264EncoderV10.CLSID,
    "VisioForge H264 Encoder");

audioEncFilter = FilterGraphTools.AddFilterFromClsid(
    filterGraphSource,
    DSFilterInitInfoConsts.VFAACEncoderV10.CLSID,
    "VisioForge AAC Encoder v10");

Guid CLSID_FileWriter = new Guid("8596E5F0-0DA5-11d0-BD21-00A0C911CE86");
fileWriterFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_FileWriter, "Filewriter");
IFileSinkFilter fileSink = fileWriterFilter as IFileSinkFilter;
fileSink?.SetFileName(edOutput.Text, null);
```

### Set the key

`IVFCryptoConfig` takes the key: `ApplyString` a password, `ApplyFile` any file, `ApplyBinary` a byte array. Set it in `AddEncryptor`, right after the filters above are added and before `RenderStream` connects the encryptor. With a license key, `IVFRegister.SetLicenseKey` registers the filter and the trial notices go away.

```csharp
IVFRegister reg = muxFilter as IVFRegister;
if (reg != null && !string.IsNullOrEmpty(SDK_LICENSE_KEY))
{
    reg.SetLicenseKey(SDK_LICENSE_KEY);
}

IVFCryptoConfig cryptoConfig = muxFilter as IVFCryptoConfig;
if (cryptoConfig == null)
{
    throw new InvalidOperationException("The encryptor filter is not registered.");
}

cryptoConfig.ApplyString(password);
// or: cryptoConfig.ApplyFile(keyFileName);
// or: cryptoConfig.ApplyBinary(keyBytes);
```

### Connect the graph

LAV Splitter Source splits the MP4 into its video and audio streams. The video is decoded into the RGB2YUV filter first, and only then do the encoders join the graph. A file without an audio stream is encrypted video-only.

```csharp
// LAV Splitter Source splits the file into video and audio streams
Guid CLSID_LAVSplitterSource = new Guid("B98D13E7-55DB-4385-A33D-09FD1BA26338");
sourceVideoFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_LAVSplitterSource, "LAV Splitter Source");
hr = ((IFileSourceFilter)sourceVideoFilter).Load(edSourceFile.Text, null);
DsError.ThrowExceptionForHR(hr);

Guid CLSID_VFRGB2YUV = new Guid("3BDA461E-12DB-4C24-9815-B68D1AA4D34A");
rgb2yuvFilter = FilterGraphTools.AddFilterFromClsid(filterGraphSource, CLSID_VFRGB2YUV, "VisioForge RGB2YUV");

// Decode the video into RGB2YUV before the encoders join the graph
hr = captureGraphSource.RenderStream(null, MediaType.Video, sourceVideoFilter, null, rgb2yuvFilter);
DsError.ThrowExceptionForHR(hr);

AddEncryptor();

hr = captureGraphSource.RenderStream(null, null, rgb2yuvFilter, videoEncFilter, muxFilter);
DsError.ThrowExceptionForHR(hr);

// A file without audio is encrypted video-only
IPin audioPin = FilterHelper.GetFreePinWithMediaType(sourceVideoFilter, PinDirection.Output, MediaType.Audio);
if (audioPin != null)
{
    Marshal.ReleaseComObject(audioPin);
    hr = captureGraphSource.RenderStream(null, MediaType.Audio, sourceVideoFilter, audioEncFilter, muxFilter);
    DsError.ThrowExceptionForHR(hr);
}

hr = captureGraphSource.RenderStream(null, null, muxFilter, null, fileWriterFilter);
DsError.ThrowExceptionForHR(hr);

// No reference clock: the file is encrypted as fast as the encoders run
IMediaFilter mediaFilter = filterGraphSource as IMediaFilter;
mediaFilter?.SetSyncSource(null);

hr = mediaControlSource.Run();
DsError.ThrowExceptionForHR(hr);
```

### Play the file back

A plain File Source reads the encrypted file, and the decryptor filter decrypts it with the same key. The decryptor must match the file: version 9 for AES-256, version 8 for AES-128. With a wrong key, the graph cannot connect the decryptor's output.

```csharp
var CLSID_FileSourceAsync = new Guid("E436EBB5-524F-11CE-9F53-0020AF0BA770");
var source = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_FileSourceAsync, "File source (async.)");
int hr = ((IFileSourceFilter)source).Load(edSourceFile.Text, null);
DsError.ThrowExceptionForHR(hr);

// Decryptor v9 reads AES-256 files; Consts.CLSID_VFVideoDecryptor8 reads AES-128 files
decryptor = FilterGraphTools.AddFilterFromClsid(
    filterGraph,
    new Guid(Consts.CLSID_VFVideoDecryptor9),
    "VisioForge Decryptor v9");

IVFRegister reg = decryptor as IVFRegister;
if (reg != null && !string.IsNullOrEmpty(SDK_LICENSE_KEY))
{
    reg.SetLicenseKey(SDK_LICENSE_KEY);
}

IVFCryptoConfig cryptoConfig = decryptor as IVFCryptoConfig;
if (cryptoConfig == null)
{
    throw new InvalidOperationException("The decryptor filter is not registered.");
}

cryptoConfig.ApplyString(password);

Guid CLSID_VideoMixingRenderer9 = new Guid("51b4abf3-748f-4e3b-a276-c828330e926a");
videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_VideoMixingRenderer9, "VMR-9");

// Draw the video into a panel of the form
IVMRFilterConfig9 vmrFilterConfig = videoRenderer as IVMRFilterConfig9;
if (vmrFilterConfig == null)
{
    throw new InvalidOperationException("Unable to create the VMR-9 renderer.");
}

vmrFilterConfig.SetRenderingMode(VMR9Mode.Windowless);
IVMRWindowlessControl9 vmrWindowlessControl = videoRenderer as IVMRWindowlessControl9;
vmrWindowlessControl.SetVideoClippingWindow(pnScreen.Handle);

hr = captureGraph.RenderStream(null, null, source, null, decryptor);
DsError.ThrowExceptionForHR(hr);

hr = captureGraph.RenderStream(null, MediaType.Video, decryptor, null, videoRenderer);
DsError.ThrowExceptionForHR(hr);

// Audio goes to the default sound device; a file without audio plays video-only
IPin audioPin = FilterHelper.GetFreePinWithMediaType(decryptor, PinDirection.Output, MediaType.Audio);
if (audioPin != null)
{
    Marshal.ReleaseComObject(audioPin);
    hr = captureGraph.RenderStream(null, MediaType.Audio, decryptor, null, null);
    if (hr < 0)
    {
        MessageBox.Show(this, $"Unable to play the audio (0x{hr:X8}), playing video only.");
    }
}

vmrWindowlessControl.SetVideoPosition(null, new DsRect(0, 0, pnScreen.Width, pnScreen.Height));

hr = mediaControl.Run();
DsError.ThrowExceptionForHR(hr);
```

---

## Sample Applications

The SDK includes working sample applications demonstrating encryption and decryption workflows:

### Included Samples

- **Encryption Demo** - Demonstrates video file encryption with H.264/AAC encoding
- **Player Demo** - Shows decryption and playback of encrypted video files

### GitHub Repository

Complete source code for all samples is available:
- [Video Encryption SDK Samples](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) - C#, C++, and Delphi examples

These samples include:
- Complete filter graph construction
- Encryption configuration
- Decryption and playback
- Error handling
- Best practices implementation

---

## Licensing Information

- [End User License Agreement](../../eula.md)

## Version History

### Version 11.4

- Full compatibility with VisioForge .NET SDKs 11.4
- Enhanced Nvidia NVENC support for H264 and H265 video encoders
- Improved Intel QuickSync support for H264 video encoder
- Added NV12 colorspace support for enhanced performance

### Version 11.0

- Complete compatibility with VisioForge .NET SDKs 11.0
- Enhanced GPU encoders support
- Upgraded AAC encoder functionality
  
### Version 10.0

- Full compatibility with VisioForge .NET SDKs 10.0
- Enhanced compatibility with H264 and H265 video formats
- Integrated AMD AMF acceleration support
- Added Intel QuickSync technology support

### Version 9.0

- Significantly improved encryption processing speed
- Added CPU hardware acceleration capabilities
- Implemented new engine based on AES-256 encryption
- Added file usage as a key (with binary array support)
- Integrated NVENC support for GPU acceleration
- Enhanced AAC HE encoder support

### Version 8.0

- Updated video and audio encoders
- Improved filter encryption performance

### Version 7.0

- Initial release as a standalone product
- Previously integrated within Video Capture SDK, Video Edit SDK, and Media Player SDK
- Compatible with any DirectShow application without requiring additional VisioForge SDKs

---

## Resources

- [Product Page](https://www.visioforge.com/video-encryption-sdk) - Purchase, licensing, and product information
- [Sample Applications](https://github.com/visioforge/directshow-samples/tree/main/Video%20Encryption%20SDK) - Complete source code examples


## Video Transcript

### What we cover

Let's encrypt video files with a password on Windows. First with the ready-made VisioForge Video Encryptor app, then in your own C# code with the Video Encryption SDK. Here's the plan. Video Encryptor encrypts a file with a password, with no code at all. Its Advanced view adds a list of files, the output folder and the codec settings. Then we build the SDK demos in Visual Studio, encrypt an MP4 with AES-256 from C#, and play the encrypted file back with the same password.

### How it works

How does it work? The video is encoded to H.264 and the audio to AAC. The encryptor filter encrypts both and writes them into one file. Only the decryptor filter, with the same key, can read that file back. Version 9 files use AES-256, version 8 files use AES-128. The key is a string, and version 9 also takes a file or binary data as the key. All of these are DirectShow filters, so they work in any DirectShow graph. You need Windows 10 or 11. For the app part, install Video Encryptor. For the code, install the Video Encryption SDK: it registers the filters, and it includes LAV Filters, which the demo uses to read MP4 files. Then Visual Studio 2022 or 2026 with the .NET desktop development workload, and the demos from the directshow-samples repository on GitHub. Both installers are on visioforge.com.

### Video Encryptor: Simple view

Video Encryptor opens in Simple view. Type a password first. Then click the lock, or drop a file on it, and pick the video. The unregistered version shows a few trial notices before it starts; they're cut here. The encrypted file lands next to the source, with the .evf extension. Encryption completed.

### Video Encryptor: Advanced view

Advanced view takes a list of files. Click Add files and pick one or more videos. On the Settings tab, choose the output folder: the source folder, or a folder of your own. Output format holds the H.264 settings for the video, with the bit rate, and the AAC settings for the audio. Type the password, and click Start. The preview shows the video while it's encrypted, and the progress bar runs to the end. Done: a second .evf file, next to its source.

### The SDK demos

The SDK gives your own app the same filters. Its Encryption Demo encrypts a video file, or a camera, into an encrypted file. Its Player Demo plays an encrypted file back with its key. Both are C# WinForms apps on .NET 8, for x64, and they reach DirectShow through the VisioForge.DirectShowAPI NuGet package. Open VideoEncryptionSDK.sln from the repository, and build the solution. NuGet restores the package, and both demos build.

### Encrypt: the filters

The encryption graph starts in AddEncryptor. It adds the encryptor filter by its class ID: the version 9 encryptor for AES-256, or the version 8 one for AES-128. Then the H.264 encoder. Then the AAC encoder for the audio, and the File Writer, with the output file name.

### Encrypt: the key

ApplyEncryptorSettings configures the encryptor. With a license key, IVFRegister.SetLicenseKey registers it, and the trial notices go away. IVFCryptoConfig takes the key. ApplyString takes a password, ApplyFile takes any file as the key, and ApplyBinary takes a byte array.

### Encrypt: the graph

SourceInitFile opens the source with LAV Splitter Source, which splits an MP4 into its video and audio streams. When LAV Filters are missing, the demo says so and stops. The video is decoded into the RGB2YUV filter first, and only then do the encoders join the graph. Then RenderStream connects RGB2YUV through the H.264 encoder to the encryptor. If the file has an audio stream, it goes through the AAC encoder to the encryptor too. A file without audio is encrypted video-only. Last, the encryptor connects to the File Writer.

### Encrypt a file

Run the Encryption Demo. Set the source file, the output file, and the password. AES-256 is the default. Click Start. Without a license key, the SDK shows a trial notice first; it's cut here. Thirty seconds of video take about ten seconds, and the demo reports Completed.

### Play it back

Now the Player Demo. A plain File Source reads the encrypted file: it's just bytes until the decryptor sees them. The decryptor filter matches the encryptor: version 9 for AES-256. It takes the key through the same IVFCryptoConfig interface. The source connects to the decryptor. The decryptor's video goes to the VMR-9 renderer, and its audio, if the file has any, to the default sound device. Set Player Demo as the startup project and run it. Pick the encrypted file and type the same password. Start, and it plays. The same player opens the file Video Encryptor made. That one is a version 8 file, so choose AES-128. In the trial, this one shows a small Unregistered mark in its corner.

### Good to know

A few things to know. The password is the key: there's no way back without it. The decryptor must match the file, version 9 or version 8. A wrong key fails: the graph cannot connect the decryptor's output. With a license, SetLicenseKey registers the filters, and the trial notices go away. And the Encryption Demo also encrypts a camera: choose Device instead of Video file.

### Next steps

The demos are on GitHub, and the docs are at visioforge.com/help, with a reference of every filter interface. Thanks for watching.

---

## See Also

- [Interface Reference](interface-reference.md) - Complete API documentation
- [Examples](examples.md) - Comprehensive code examples for encryption and decryption
- [DirectShow Encoding Filters Pack](../filters-enc/index.md) - Compatible video encoders (H.264, H.265, AAC)
