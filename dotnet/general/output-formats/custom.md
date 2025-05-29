---
title: DirectShow Custom Video Format Integration in .NET
description: Learn how to implement custom video output formats using DirectShow filters in .NET applications. Step-by-step guide for developers to create specialized video processing pipelines with codec configuration and format handling.
sidebar_label: Custom Output Formats

---

# Creating Custom Video Output Formats with DirectShow Filters

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net)

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

## Overview

Working with video in .NET applications often requires custom output formats to meet specific project requirements. The VisioForge SDKs provide powerful capabilities to implement custom format outputs using DirectShow filters, giving developers precise control over audio and video processing pipelines.

This guide demonstrates practical techniques for implementing custom output formats that work seamlessly with both the Video Capture SDK .NET and Video Edit SDK .NET, allowing you to tailor your video applications to exact specifications.

## Why Use Custom Output Formats?

Custom output formats offer several advantages for .NET developers:

- Support for specialized video codecs not available in standard formats
- Fine-grained control over video and audio compression settings
- Integration with third-party DirectShow filters
- Ability to create proprietary or industry-specific output formats
- Optimization for specific use cases (streaming, archiving, editing)

## Getting Started with CustomOutput

The `CustomOutput` class is the cornerstone for configuring custom output settings in VisioForge SDKs. This class enables you to define and configure the filters used in your video processing pipeline.

Start by initializing a new instance:

```cs
var customOutput = new CustomOutput();
```

While our examples use the `VideoCaptureCore` class, developers using Video Edit SDK .NET can apply the same techniques with `VideoEditCore`.

## Implementation Strategies

There are two primary approaches to implementing custom format output with DirectShow filters:

### Strategy 1: Three-Component Pipeline

This modular approach divides the processing pipeline into three distinct components:

1. Audio codec
2. Video codec
3. Multiplexer (file format container)

This separation provides maximum flexibility and control over each stage of the process. You can use either standard DirectShow filters or specialized codecs for audio and video components.

#### Retrieving Available Codecs

Begin by populating your UI with available codecs and filters:

```cs
// Populate video codec options
foreach (string codec in VideoCapture1.Video_Codecs)
{
    videoCodecDropdown.Items.Add(codec);
}

// Populate audio codec options
foreach (string codec in VideoCapture1.Audio_Codecs)
{
    audioCodecDropdown.Items.Add(codec);
}

// Get all available DirectShow filters
foreach (string filter in VideoCapture1.DirectShow_Filters)
{
    directShowAudioFilters.Items.Add(filter);
    directShowVideoFilters.Items.Add(filter);
    multiplexerFilters.Items.Add(filter);
    fileWriterFilters.Items.Add(filter);
}
```

#### Configuring the Pipeline Components

Next, set up your video and audio processing components based on user selections:

```cs
// Set up video codec
if (useStandardVideoCodec.Checked)
{
    customOutput.Video_Codec = videoCodecDropdown.Text;
    customOutput.Video_Codec_UseFiltersCategory = false;
}
else
{
    customOutput.Video_Codec = directShowVideoFilters.Text;
    customOutput.Video_Codec_UseFiltersCategory = true;
}

// Set up audio codec
if (useStandardAudioCodec.Checked)
{
    customOutput.Audio_Codec = audioCodecDropdown.Text;
    customOutput.Audio_Codec_UseFiltersCategory = false;
}
else
{
    customOutput.Audio_Codec = directShowAudioFilters.Text;
    customOutput.Audio_Codec_UseFiltersCategory = true;
}

// Configure the multiplexer
customOutput.MuxFilter_Name = multiplexerFilters.Text;
customOutput.MuxFilter_IsEncoder = false;
```

#### Custom File Writer Configuration

For specialized outputs that require a dedicated file writer:

```cs
// Enable special file writer if needed
customOutput.SpecialFileWriter_Needed = useCustomFileWriter.Checked;
customOutput.SpecialFileWriter_FilterName = fileWriterFilters.Text;
```

This approach gives you granular control over each stage of the encoding process, making it ideal for complex output requirements.

### Strategy 2: All-in-One Filter

This streamlined approach uses a single DirectShow filter that combines the functionality of the multiplexer, video codec, and audio codec. The SDK intelligently handles detection of the filter's capabilities, determining whether it:

- Can directly write files without assistance
- Requires the standard DirectShow File Writer filter
- Needs a specialized file writer filter

Implementation is straightforward:

```cs
// Populate filter options from available DirectShow filters
foreach (string filter in VideoCapture1.DirectShow_Filters)
{
    filterDropdown.Items.Add(filter);
}

// Configure the all-in-one filter
customOutput.MuxFilter_Name = selectedFilter.Text;
customOutput.MuxFilter_IsEncoder = true;

// Set up specialized file writer if required
customOutput.SpecialFileWriter_Needed = requiresCustomWriter.Checked;
customOutput.SpecialFileWriter_FilterName = fileWriterFilter.Text;
```

This approach is simpler to implement but offers less granular control over individual components of the encoding process.

## Simplifying Configuration with Dialog UI

For a more user-friendly implementation, VisioForge provides a built-in settings dialog that handles the configuration of custom formats:

```cs
// Create and configure the settings dialog
CustomFormatSettingsDialog settingsDialog = new CustomFormatSettingsDialog(
    VideoCapture1.Video_Codecs.ToArray(),
    VideoCapture1.Audio_Codecs.ToArray(),
    VideoCapture1.DirectShow_Filters.ToArray());

// Apply settings to your CustomOutput instance
settingsDialog.SaveSettings(ref customOutput);
```

This dialog provides a complete UI for configuring all aspects of custom output formats, saving development time while still offering full flexibility.

## Implementing the Output Process

After configuring your custom format settings, you need to apply them to your capture or edit process:

```cs
// Apply the custom format configuration
VideoCapture1.Output_Format = customOutput;

// Set the capture mode
VideoCapture1.Mode = VideoCaptureMode.VideoCapture;

// Specify output file path
VideoCapture1.Output_Filename = "output_video.mp4";

// Start the capture or encoding process
await VideoCapture1.StartAsync();
```

## Performance Considerations

When implementing custom output formats, keep these performance tips in mind:

- DirectShow filters vary in efficiency and resource usage
- Test your filter combinations with typical input media
- Some third-party filters may introduce additional latency
- Consider memory usage when processing high-resolution video
- Filter compatibility may vary across different Windows versions

## Required Packages

To use custom DirectShow filters, ensure you have the appropriate redistributable packages installed:

### Video Capture SDK .Net

- [x86 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- [x64 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Video Edit SDK .Net

- [x86 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
- [x64 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Troubleshooting

Common issues when working with custom DirectShow filters include:

- Filter compatibility conflicts
- Missing codecs or dependencies
- Registration issues with COM components
- Memory leaks in third-party filters
- Performance bottlenecks with complex filter graphs

If you encounter problems, verify that all required filters are properly registered on your system and that you have the latest versions of both the filters and the VisioForge SDK.

## Conclusion

Custom output formats using DirectShow filters provide powerful capabilities for .NET developers working with video applications. Whether you choose the flexibility of a three-component pipeline or the simplicity of an all-in-one filter approach, VisioForge's SDKs give you the tools you need to create exactly the output format your application requires.

---

For more code samples and implementation examples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
