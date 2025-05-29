---
title: FM Radio & TV Tuning Integration in .NET SDK
description: Complete guide for C# developers to implement FM radio and TV tuning features in their applications. Learn to scan frequencies, manage channels, and integrate broadcast capabilities in WPF, WinForms, and console applications.
sidebar_label: FM Radio and TV Tuning
order: 2

---
# FM Radio and TV Tuning Integration for .NET Applications

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge variant="dark" size="xl" text="VideoCaptureCore"]

## Introduction to Broadcast Integration

Modern .NET applications can leverage hardware capabilities to provide FM radio and TV tuning functionality. This guide demonstrates how to implement these features in your C# applications, whether you're building WPF, WinForms, or console applications. By following these examples, you'll be able to detect available tuner devices, scan frequencies, manage channels, and deliver a complete broadcast experience to your users.

## Hardware Requirements

Before implementing the code samples below, ensure your development system has:

1. A compatible TV tuner card or USB device
2. Proper driver installation
3. .NET Framework 4.7+ or .NET Core 3.1+/NET 5.0+ for modern applications

## Detecting Available Tuner Devices

The first step in implementing tuner functionality is to detect all available tuner devices on the system. This allows users to select the appropriate hardware for their needs.

```cs
// Populate a combobox with all available TV Tuner devices
foreach (var tunerDevice in VideoCapture1.TVTuner_Devices)
{
  cbTVTuner.Items.Add(tunerDevice);
}
```

You can then let users select their preferred device from the populated list, or automatically select the first available device for a streamlined experience.

## Configuration Basics

### TV Format Selection

Different regions use different broadcasting standards. Your application should detect and allow selection of the appropriate standard:

```cs
// List all supported TV formats (PAL, NTSC, SECAM, etc.)
foreach (var tunerTVFormat in VideoCapture1.TVTuner_TVFormats)
{
  cbTVSystem.Items.Add(tunerTVFormat);
}
```

### Regional Settings

Broadcast frequencies vary by country. Configure your application with the correct regional settings:

```cs
// Populate country selection for region-specific frequencies
foreach (var tunerCountry in VideoCapture1.TVTuner_Countries)
{
  cbTVCountry.Items.Add(tunerCountry);
}
```

## Setting Up the Tuner

After detecting available devices, you need to select and initialize the tuner:

```cs
// Select the TV Tuner device
VideoCapture1.TVTuner_Name = cbTVTuner.Text;

// Initialize the tuner and read its current settings
await VideoCapture1.TVTuner_ReadAsync();
```

This initialization process will prepare the tuner for further operations and read its current configuration.

## Working with Different Signal Sources

Most tuners support multiple input types. You'll need to determine which modes are available:

```cs
// Get all available modes (TV, FM Radio, etc.)
foreach (var tunerMode in VideoCapture1.TVTuner_Modes)
{
  cbTVMode.Items.Add(tunerMode);
}
```

Then select the appropriate input source:

```cs
// Select the signal source (Antenna, Cable, etc.)
cbTVInput.SelectedIndex = cbTVInput.Items.IndexOf(VideoCapture1.TVTuner_InputType);

// Select working mode (TV, FM Radio, etc.)
cbTVMode.SelectedIndex = cbTVMode.Items.IndexOf(VideoCapture1.TVTuner_Mode);
```

## Advanced Frequency Management

For detailed control, you can work directly with the frequency values:

```cs
// Display current frequency settings
edVideoFreq.Text = Convert.ToString(VideoCapture1.TVTuner_VideoFrequency);
edAudioFreq.Text = Convert.ToString(VideoCapture1.TVTuner_AudioFrequency);
```

These values can be useful for debugging or creating custom frequency selection interfaces.

## Setting Broadcasting System Standards

Different regions use different broadcasting standards. Configure your application with the right system:

```cs
// Select the TV system (PAL, NTSC, SECAM, etc.)
cbTVSystem.SelectedIndex = cbTVSystem.Items.IndexOf(VideoCapture1.TVTuner_TVFormat);

// Select country for region-specific frequencies
cbTVCountry.SelectedIndex = cbTVCountry.Items.IndexOf(VideoCapture1.TVTuner_Country);
```

## Automated Channel Scanning

One of the most important features is the ability to automatically scan and detect available channels. This requires implementing an event handler to receive scan results:

```cs
private void VideoCapture1_OnTVTunerTuneChannels(object sender, TVTunerTuneChannelsEventArgs e)
{
  // Update progress bar
  pbChannels.Value = e.Progress;

  // If a signal is detected, add the channel to the list
  if (e.SignalPresent)
  {
    cbTVChannel.Items.Add(e.Channel.ToString());
  }

  // Check if scanning is complete
  if (e.Channel == -1)
  {
    pbChannels.Value = 0;
    MessageBox.Show("Channel scanning complete");
  }

  // Keep UI responsive during scanning
  Application.DoEvents();
}
```

This event handler will be called for each frequency as it's scanned, allowing you to update your UI and collect found channels.

## Initiating the Channel Scan Process

Once the event handler is in place, you can start the scanning process:

```cs
const int KHz = 1000;
const int MHz = 1000000; 

// Initialize tuner and clear previous channel list
await VideoCapture1.TVTuner_ReadAsync(); 
cbTVChannel.Items.Clear();

// For FM Radio mode, configure scanning parameters
if ((cbTVMode.SelectedIndex != -1) && (cbTVMode.Text == "FM Radio")) 
{
  // Set FM scanning range from 100 MHz to 110 MHz
  VideoCapture1.TVTuner_FM_Tuning_StartFrequency = 100 * MHz; 
  VideoCapture1.TVTuner_FM_Tuning_StopFrequency = 110 * MHz; 
  
  // Scan in 100 KHz increments
  VideoCapture1.TVTuner_FM_Tuning_Step = 100 * KHz;
}

// Begin the scanning process
VideoCapture1.TVTuner_TuneChannels_Start();
```

This code prepares the tuner and begins scanning. For FM radio mode, it sets specific frequency ranges and steps.

## Manual Channel Management

In addition to automatic scanning, your application should allow manual channel selection:

### Setting Channel by Number

```cs
// Set to a specific channel number
VideoCapture1.TVTuner_Channel = Convert.ToInt32(edChannel.Text); 
await VideoCapture1.TVTuner_ApplyAsync();
```

### Setting Channel by Frequency

```cs
// Set channel to -1 to allow direct frequency setting
VideoCapture1.TVTuner_Channel = -1; 

// Set the specific frequency in Hz
VideoCapture1.TVTuner_Frequency = Convert.ToInt32(edChannel.Text); 
await VideoCapture1.TVTuner_ApplyAsync();
```

This approach gives advanced users more control over their tuning experience.

## Optimizing User Experience

For the best user experience, consider implementing these additional features:

1. **Favorite channels**: Allow users to save and quickly access their preferred channels
2. **Signal strength indicator**: Display the current signal quality
3. **Channel information**: Show program information when available
4. **Auto-tuning scheduled task**: Periodically scan for new channels

## Error Handling Best Practices

Robust error handling is essential for tuner applications:

1. Check if hardware is present before attempting operations
2. Handle cases where no signal is detected
3. Provide clear error messages when tuning fails
4. Implement timeouts for scanning operations

## Required Dependencies

To use the FM radio and TV tuning features, include these packages:

- Video capture redistributables:
  - [x86 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

You can add these packages via NuGet Package Manager or by editing your .csproj file directly.

## Performance Considerations

When implementing tuner functionality:

1. Run scanning operations in a background thread to keep UI responsive
2. Cache channel information to avoid repeated scans
3. Implement efficient channel switching to minimize delay
4. Consider resource usage, especially for embedded or mobile applications

## Conclusion

By following this guide, you can implement full FM radio and TV tuning capabilities in your .NET applications. These features can enhance media applications, home automation systems, or specialized broadcast software. The SDK provides a clean, consistent API that handles the complexities of different tuner hardware.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
