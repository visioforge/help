---
title: FFMPEG Source DirectShow Filter for Multimedia Apps
description: Integrate powerful media playback into your applications with our FFMPEG Source DirectShow Filter. Full support for MP4, MKV, AVI, network streams, and seamless integration with Delphi, C++, and .NET projects.
sidebar_label: FFMPEG Source DirectShow Filter

---

# FFMPEG Source DirectShow Filter

## Introduction

The FFMPEG Source DirectShow filter enables developers to seamlessly integrate advanced media decoding and playback capabilities into any DirectShow-compatible application. This powerful component bridges the gap between complex multimedia formats and your software development needs, providing a robust foundation for building media-rich applications.

## Key Features and Capabilities

Our filter comes bundled with all necessary FFMPEG DLLs and provides a feature-rich DirectShow filter interface that supports:

- **Extensive Format Compatibility**: Handle a wide range of video and audio formats including MP4, MKV, AVI, MOV, WMV, FLV, and many others without additional codec installations
- **Network Stream Support**: Connect to RTSP, RTMP, HTTP, UDP, and TCP streams for live media integration
- **Multiple Stream Management**: Select between video and audio streams in multi-stream media files
- **Advanced Seeking Capabilities**: Implement precise seeking functionality in your applications
- **GPU Acceleration**: Utilize hardware acceleration for optimal performance

## Implementation Examples

The SDK includes comprehensive sample applications for multiple development environments:

### Delphi Integration (Primary)

```delphi
// Initialize the FFMPEG Source filter in Delphi using DSPack
procedure TMainForm.InitializeFFMPEGSource;
var
  FFMPEGFilter: IBaseFilter;
  FileSource: IFileSourceFilter;
begin
  // Create FFMPEG Source filter instance
  // IMPORTANT: Ensure proper COM initialization before this call
  CoCreateInstance(CLSID_FFMPEGSource, nil, CLSCTX_INPROC_SERVER, 
                  IID_IBaseFilter, FFMPEGFilter);
  
  // Query for file source interface
  FFMPEGFilter.QueryInterface(IID_IFileSourceFilter, FileSource);
  
  // Load media file - can be local or network URL
  FileSource.Load('C:\media\sample.mp4', nil);
  
  // Add to filter graph for rendering
  FilterGraph.AddFilter(FFMPEGFilter, 'FFMPEG Source');
  
  // Connect to appropriate renderers or processing filters
  // FilterGraph.RenderStream(...);
end;
```

### .NET and C++ Options

The SDK also supports .NET applications (using DirectShowNet library) and C++ development environments with equivalent functionality and similar implementation patterns.

### C++ Integration Example

```cpp
// Initialize the FFMPEG Source filter in C++ using DirectShow
HRESULT InitializeFFMPEGSource()
{
    HRESULT hr = S_OK;
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pFFMPEGSource = NULL;
    IFileSourceFilter* pFileSource = NULL;
    
    // Initialize COM
    CoInitialize(NULL);
    
    // Create the filter graph manager
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, 
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr))
        return hr;
    
    // Create the FFMPEG Source filter
    hr = CoCreateInstance(CLSID_FFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pFFMPEGSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Add the filter to the graph
    hr = pGraph->AddFilter(pFFMPEGSource, L"FFMPEG Source");
    if (FAILED(hr))
        goto cleanup;
    
    // Get the IFileSourceFilter interface
    hr = pFFMPEGSource->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Load the media file
    hr = pFileSource->Load(L"C:\\media\\sample.mp4", NULL);
    if (FAILED(hr))
        goto cleanup;
    
    // Render the output pins of the FFMPEG Source filter
    hr = pGraph->Render(GetPin(pFFMPEGSource, PINDIR_OUTPUT, 0));
    
    // Get the media control interface for playback control
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Start playback
        hr = pControl->Run();
        // ... handle playback as needed
    }
    
cleanup:
    // Release interfaces
    if (pControl) pControl->Release();
    if (pFileSource) pFileSource->Release();
    if (pFFMPEGSource) pFFMPEGSource->Release();
    if (pGraph) pGraph->Release();
    
    return hr;
}

// Helper function to get pins from a filter
IPin* GetPin(IBaseFilter* pFilter, PIN_DIRECTION PinDir, int nPin)
{
    IEnumPins* pEnum = NULL;
    IPin* pPin = NULL;
    
    if (pFilter)
    {
        pFilter->EnumPins(&pEnum);
        if (pEnum)
        {
            while (pEnum->Next(1, &pPin, NULL) == S_OK)
            {
                PIN_DIRECTION PinDirThis;
                pPin->QueryDirection(&PinDirThis);
                if (PinDir == PinDirThis)
                {
                    if (nPin == 0)
                        break;
                    nPin--;
                }
                pPin->Release();
                pPin = NULL;
            }
            pEnum->Release();
        }
    }
    
    return pPin;
}
```

## Integration with Processing Filters

Enhance your media pipeline by connecting the FFMPEG Source filter with additional processing components:

- Apply real-time video effects and transformations
- Process audio streams for custom sound manipulation
- Implement specialized media analysis features

Our [Processing Filters pack](https://www.visioforge.com/processing-filters-pack) offers additional capabilities, or you can integrate with any standard DirectShow-compatible filters.

## Technical Specifications

### Supported DirectShow Interfaces

The filter implements these standard DirectShow interfaces for maximum compatibility:

- **IAMStreamSelect**: Select between multiple video and audio streams
- **IAMStreamConfig**: Configure video and audio settings
- **IFileSourceFilter**: Set filename or streaming URL
- **IMediaSeeking**: Implement precise seeking functionality
- **ISpecifyPropertyPages**: Access configuration through property pages

## Version History and Updates

### Version 15.0

- Enhanced FFMPEG libraries with latest codecs
- Added GPU decoding support for improved performance
- Optimized memory management for large files

### Version 12.0

- Updated FFMPEG libraries
- Improved compatibility with Windows 10/11

### Version 11.0

- Updated FFMPEG libraries
- Fixed seeking issues with certain file formats

### Version 10.0

- Updated FFMPEG libraries
- Added support for additional container formats

### Version 9.0

- Updated FFMPEG libraries
- Performance optimizations

### Version 8.0

- Updated FFMPEG libraries
- Improved error handling

### Version 7.0

- Initial release as an independent product
- Core functionality established

## Additional Resources

- Explore our [product page](https://www.visioforge.com/ffmpeg-source-directshow-filter) for detailed specifications
- View our [End User License Agreement](../../eula.md) for licensing details
- Check our developer documentation for advanced implementation scenarios
