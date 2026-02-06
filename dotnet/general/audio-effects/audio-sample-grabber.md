---
title: Working with Audio Sample Grabber in .NET SDKs
description: Capture and process real-time audio frames using Audio Sample Grabber with X-engines and Classic engines in .NET SDK applications.
---

# Working with Audio Sample Grabber in .NET SDKs

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Audio Sample Grabber

The Audio Sample Grabber is a powerful feature available across our .NET SDKs that enables developers to access raw audio frames directly from both live sources and media files. This capability opens up a wide range of possibilities for audio processing, analysis, and manipulation in your applications.

When working with audio processing, gaining access to individual audio frames is essential for tasks such as:

- Real-time audio visualization
- Custom audio effects processing
- Speech recognition integration
- Audio analysis and metrics
- Custom audio format conversion
- Sound detection algorithms

The `OnAudioFrameBuffer` event is the core mechanism that provides access to these raw audio frames. This event fires each time a new audio frame is available, giving you direct access to unmanaged memory containing the decoded audio data.

## How Audio Sample Grabber Works

The Audio Sample Grabber intercepts the audio pipeline during playback or capture, providing you with the raw audio data before it's rendered to the output device. This data is typically in PCM (Pulse Code Modulation) format, which is the standard format for uncompressed digital audio, but can occasionally be in IEEE floating-point format depending on the audio source.

Each time the `OnAudioFrameBuffer` event fires, it provides an `AudioFrameBufferEventArgs` object containing critical information about the audio frame:

- `Frame.Data`: An `IntPtr` pointing to the unmanaged memory block containing the raw audio data
- `Frame.DataSize`: The size of the audio data in bytes
- `Frame.Info`: A structure containing detailed information about the audio format, including:
  - Channel count (mono, stereo, etc.)
  - Sample rate (typically 44.1kHz, 48kHz, etc.)
  - Bits per sample (16-bit, 24-bit, etc.)
  - Audio format type (PCM, IEEE, etc.)
  - Timestamp information
  - Block alignment and other format-specific details

## Setting Up Audio Sample Grabber

The setup process varies slightly depending on whether you're using our newer X-engines or the Classic engines. Let's explore both approaches:

=== "X-engines"

    
    For X-engines, setting up the Audio Sample Grabber is straightforward. You simply need to create an event handler for the `OnAudioFrameBuffer` event:
    
    ```csharp
    VideoCapture1.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    
    The X-engines architecture automatically enables audio sample grabbing when you subscribe to this event, with no additional configuration required.
    

=== "Classic engines"

    
    When using Classic engines, you need to explicitly enable the Audio Sample Grabber functionality before creating the event handler:
    
    ```csharp
    VideoCapture1.Audio_Sample_Grabber_Enabled = true;
    ```
    
    Then, as with X-engines, create your event handler:
    
    ```csharp
    VideoCapture1.OnAudioFrameBuffer += OnAudioFrameBuffer;
    ```
    
    **Note**: The `Audio_Sample_Grabber_Enabled` property is not required for the VideoEditCore component, which has audio sample grabbing enabled by default.
    

=== "Media Blocks SDK"

    
    The Media Blocks SDK also supports audio sample grabbing. Use the `AudioSampleGrabberBlock` component to capture audio frames.
    
    ```csharp
    private AudioSampleGrabberBlock _audioSampleGrabberSink;
    ```
    
    Then, as with X-engines, create your event handler, and specify the audio format:
    
    ```csharp
    _audioSampleGrabberBlock = new AudioSampleGrabberBlock(VisioForge.Core.Types.X.AudioFormatX.S16);
    _audioSampleGrabberBlock.OnAudioSampleGrabber += OnAudioFrameBuffer;
    ```
    


## Processing Audio Frames

Once you've set up the event handler, you can process the audio frames as they arrive. Here's a basic example of how to handle the `OnAudioFrameBuffer` event:

```csharp
using VisioForge.Types;
using System.Diagnostics;

private void OnAudioFrameBuffer(object sender, AudioFrameBufferEventArgs e)
{
    // Log audio frame information
    Debug.WriteLine($"Audio frame: {e.Frame.DataSize} bytes; Format: {e.Frame.Info}");
    
    // Access to raw audio data through the unmanaged pointer
    IntPtr rawAudioData = e.Frame.Data;
    
    // Get audio format details
    int channelCount = e.Frame.Info.ChannelCount;
    int sampleRate = e.Frame.Info.SampleRate;
    int bitsPerSample = e.Frame.Info.BitsPerSample;
    
    // Your custom audio processing code here
    // ...
}
```

## Working with Audio Data

### Converting Unmanaged Memory to Managed Arrays

While the `e.Frame.Data` provides a pointer to unmanaged memory, you often need to work with the data in a more convenient form. The `AudioFrame` class provides a helpful `GetDataArray()` method that returns a copy of the audio data as a byte array:

```csharp
private void VideoCapture1_OnAudioFrameBuffer(object sender, AudioFrameBufferEventArgs e)
{
    // Get a managed copy of the audio data
    byte[] audioData = e.Frame.GetDataArray();
    
    // Now you can work with the data using standard C# array operations
    // ...
}
```

### Converting PCM Data to Samples

For many audio processing tasks, you'll want to convert the raw PCM bytes into actual audio sample values. Here's a helper method to convert a PCM byte array to an array of audio samples (assuming 16-bit samples):

```csharp
private short[] ConvertBytesToSamples(byte[] audioData)
{
    short[] samples = new short[audioData.Length / 2];
    
    for (int i = 0; i < samples.Length; i++)
    {
        // Combine two bytes into one 16-bit sample
        samples[i] = (short)(audioData[i * 2] | (audioData[i * 2 + 1] << 8));
    }
    
    return samples;
}
```

### Handling Multi-Channel Audio

When working with stereo or multi-channel audio, the samples are typically interleaved. For a stereo stream, the data is arranged as: [Left0, Right0, Left1, Right1, ...]. You may want to separate these channels for processing:

```csharp
private void ProcessStereoAudio(short[] samples, int channelCount)
{
    if (channelCount != 2) return;
    
    // Create arrays for each channel
    int samplesPerChannel = samples.Length / 2;
    short[] leftChannel = new short[samplesPerChannel];
    short[] rightChannel = new short[samplesPerChannel];
    
    // Separate the channels
    for (int i = 0; i < samplesPerChannel; i++)
    {
        leftChannel[i] = samples[i * 2];
        rightChannel[i] = samples[i * 2 + 1];
    }
    
    // Process each channel separately
    // ...
}
```

## Common Audio Processing Scenarios

### Audio Level Metering

A common use case for the Audio Sample Grabber is to implement audio level metering:

```csharp
private void CalculateAudioLevel(short[] samples)
{
    double sum = 0;
    
    // Calculate RMS (Root Mean Square) value
    foreach (short sample in samples)
    {
        sum += sample * sample;
    }
    
    double rms = Math.Sqrt(sum / samples.Length);
    
    // Convert to decibels
    double db = 20 * Math.Log10(rms / 32768);
    
    // Update UI with the level (you'll need to invoke if on a different thread)
    Debug.WriteLine($"Audio level: {db} dB");
}
```

### Real-time FFT for Spectrum Analysis

For frequency spectrum analysis, you might want to perform an FFT (Fast Fourier Transform) on the audio data:

```csharp
// Note: You'll need a library for FFT calculation
// This is a simplified example
private void PerformFFTAnalysis(short[] samples)
{
    // Typically you would use a library like Math.NET Numerics
    // Convert samples to complex numbers
    Complex[] complex = samples.Select(s => new Complex(s, 0)).ToArray();
    
    // Perform FFT (pseudocode)
    // Complex[] fftResult = FFT.Forward(complex);
    
    // Process FFT results
    // ...
}
```

## Performance Considerations

When working with the Audio Sample Grabber, keep these performance considerations in mind:

1. **Minimize Processing Time**: The `OnAudioFrameBuffer` event is called on the audio processing thread. Long-running operations can cause audio glitches.

2. **Consider Thread Safety**: If you need to update UI elements or interact with other components, use proper thread synchronization methods.

3. **Avoid Memory Allocations**: Frequent memory allocations in the event handler can lead to garbage collection pauses. Reuse arrays where possible.

4. **Buffer Copying**: The `GetDataArray()` method creates a copy of the audio data. For very high-performance scenarios, consider working directly with the unmanaged pointer.

## Conclusion

The Audio Sample Grabber provides a powerful way to access and process raw audio data in real-time from both live sources and media files. By leveraging this functionality, you can implement sophisticated audio processing features in your applications, from simple level metering to complex audio analysis and effects processing.

Whether you're building a professional audio application, implementing audio visualization, or integrating with speech recognition services, the Audio Sample Grabber gives you the raw data you need to bring your audio processing ideas to life.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.