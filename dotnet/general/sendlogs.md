---
title: Troubleshooting with Logs for .NET SDK Products
description: Enable and capture debug logs for .NET SDK troubleshooting with step-by-step instructions for demo and production environments.
---

# Troubleshooting with Logs for .NET SDK Products

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Why Logs Matter in SDK Troubleshooting

When developing applications that utilize media SDKs, you may encounter technical issues that require detailed investigation. Debug logs provide critical information that helps identify the root cause of problems quickly and efficiently. These logs capture everything from initialization sequences to detailed operation steps, error conditions, and system information.

Properly collected logs offer several key benefits:

- **Faster Issue Resolution**: Technical support can quickly identify the source of problems
- **Complete Context**: Logs provide a full picture of what happened before, during, and after an issue
- **System Information**: Details about your environment help reproduce and solve problems
- **Development Insights**: Understanding logs can help you optimize your implementation

## Log Collection in Demo Applications

Our demo applications include built-in debugging capabilities that make it easy to collect logs for troubleshooting. Follow these steps to enable and share logs:

### Step-by-Step Guide for Demo Application Logging

1. **Launch the Demo Application**
   - Open the relevant demo application for your SDK
   - Locate the main interface where settings can be configured

2. **Enable Debug Mode**
   - Find and check the "Debug" checkbox in the application interface
   - This activates detailed logging of all SDK operations

3. **Reproduce the Issue**
   - Configure any other required settings for your specific scenario
   - Press the Start or Play button (depending on which SDK you're using)
   - Allow the application to run until the issue occurs
   - After sufficient time to capture the problem, press the Stop button

4. **Collect Log Files**
   - Navigate to "My Documents\VisioForge" on your system
   - This folder contains all generated log files
   - **Important**: Exclude any audio/video recordings from your collection to reduce file size

5. **Share Logs Securely**
   - Compress the log files into a ZIP archive
   - Upload to a secure file sharing service like Dropbox, Google Drive, or OneDrive
   - Share the access link with technical support

## Implementing Logging in Your Custom Applications

When you're developing your own applications with our SDKs, you'll need to explicitly enable and configure logging. This section explains how to implement logging with different SDK components.

### Enabling Debug Logs in Your Code

Regardless of which SDK you're using, the basic approach to enabling logs follows a similar pattern:

```csharp
// Example for MediaPlayer SDK
mediaPlayer.Debug_Mode = true;
mediaPlayer.Debug_Dir = "C:\\Logs\\MyApplication";

// Example for Video Capture SDK
videoCapture.Debug_Mode = true;
videoCapture.Debug_Dir = "C:\\Logs\\MyApplication";

// Example for Video Edit SDK
videoEdit.Debug_Mode = true;
videoEdit.Debug_Dir = "C:\\Logs\\MyApplication";
```

### Detailed Implementation Guide

1. **Set Debug Mode Property**
   - For any SDK component you're using, set the `Debug_Mode` property to `true`
   - This must be done before calling initialization or playback methods
   - Example: `MediaPlayer1.Debug_Mode = true;`

2. **Specify Log Directory**
   - Set the `Debug_Dir` property to a valid directory path
   - Ensure the specified directory exists and your application has write permissions
   - Example: `MediaPlayer1.Debug_Dir = "C:\\LogFiles\\MyApp";`

3. **Configure Additional Parameters**
   - Set up any other required parameters for your specific use case
   - These could include video sources, codecs, output settings, etc.

4. **Initialize and Run the Component**
   - Call the appropriate method to start the component (e.g., `Start()` or `Play()`)
   - Let the application run until you've reproduced the issue you're troubleshooting

5. **Collect and Share Logs**
   - Locate the log files in both your specified directory and "My Documents\VisioForge"
   - Compress all log files into a ZIP archive
   - Share via secure file sharing service

## Advanced Logging Techniques

For more complex applications or difficult-to-reproduce issues, consider these advanced logging approaches:

### Conditional Debug Activation

You might want to enable debug logging only in certain scenarios or based on user actions:

```csharp
// Enable debug mode only when troubleshooting
if (troubleshootingMode)
{
    mediaPlayer.Debug_Mode = true;
    mediaPlayer.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "AppLogs"
    );
}
```

### Environment-Specific Logging

Different deployment environments may require different logging approaches:

```csharp
#if DEBUG
    // Development environment logging
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "DevLogs"
    );
#else
    // Production environment logging (if permitted by your privacy policy)
    string appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "YourCompany",
        "YourApp",
        "Logs"
    );
    Directory.CreateDirectory(appDataPath);
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = appDataPath;
#endif
```

## Best Practices for Effective Logging

To ensure you get the most valuable information from your logs, follow these best practices:

### 1. Clear Initial State

Before starting a logging session, consider resetting your application state:

- Close and restart the application
- Clear any cached data if relevant
- Ensure you're capturing from a known starting point

### 2. Capture Complete Sessions

When possible, capture the entire session from start to finish:

- Enable logging before initializing SDK components
- Let logging run through the entire operation
- Continue logging until after the issue occurs

### 3. Document Reproduction Steps

Along with your logs, provide clear steps to reproduce the issue:

- Note specific settings used
- Document the exact sequence of operations
- Include timing information if relevant (e.g., "crash occurs after 30 seconds of playback")

### 4. Manage Log Size

Debug logs can grow large, especially for long sessions:

- For extended tests, consider breaking logging into multiple sessions
- Focus on capturing just the problematic scenario
- Always exclude large media files when sharing logs

### 5. Secure Sensitive Information

Before sharing logs, be aware of potential sensitive data:

- Review logs for any personal or sensitive information
- Consider using sanitized test content when possible
- Use secure methods to transfer log files

## Interpreting Common Log Messages

While advanced log analysis is best left to technical support, understanding some common log patterns can help you identify issues:

- **Initialization Errors**: Look for messages containing "Init" or "Initialize"
- **Format Issues**: Watch for "format" or "codec" related messages
- **Resource Problems**: Messages about "memory", "handles", or "resources"
- **Performance Warnings**: Notes about "frame drops", "processing time", or "buffers"

## Conclusion

Proper logging is essential for efficient troubleshooting of SDK-based applications. By following the guidelines in this document, you can provide the detailed information needed to quickly resolve any issues you encounter. Remember that detailed logs significantly reduce resolution time and help improve the quality of both your application and our SDKs.

For additional code samples and implementation guides, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
