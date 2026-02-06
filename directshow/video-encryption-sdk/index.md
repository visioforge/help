---
title: Advanced Video Encryption SDK for Developers
description: DirectShow video encryption SDK with AES-256 for H.264/AAC MP4 files, GPU acceleration, and complete API for C++, C#, and Delphi.
sidebar_label: Video Encryption SDK
order: 5
---

# Video Encryption SDK

## Introduction to Video Encryption

The [Video Encryption SDK](https://www.visioforge.com/video-encryption-sdk) provides robust tools for encoding video files into MP4 H264/AAC format with advanced encryption capabilities. Developers can secure their media content using custom passwords or binary data encryption methods.

The SDK integrates seamlessly with any DirectShow application through a complete set of filters. These filters come with extensive interfaces allowing developers to fine-tune settings according to specific security requirements and implementation needs.

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
| **Encrypt Muxer** | `{F1D3727A-88DE-49ab-A635-280BEFEFF902}` | Muxer with encryption |
| **Decrypt Demuxer** | `{D2C761F0-9988-4f79-9B0E-FB2B79C65851}` | Demuxer with decryption |

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

- Full compatibility with VisioForge .Net SDKs 11.4
- Enhanced Nvidia NVENC support for H264 and H265 video encoders
- Improved Intel QuickSync support for H264 video encoder
- Added NV12 colorspace support for enhanced performance

### Version 11.0

- Complete compatibility with VisioForge .Net SDKs 11.0
- Enhanced GPU encoders support
- Upgraded AAC encoder functionality
  
### Version 10.0

- Full compatibility with VisioForge .Net SDKs 10.0
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

---

## See Also

- [Interface Reference](interface-reference.md) - Complete API documentation
- [Examples](examples.md) - Comprehensive code examples for encryption and decryption
- [DirectShow Encoding Filters Pack](../filters-enc/index.md) - Compatible video encoders (H.264, H.265, AAC)
