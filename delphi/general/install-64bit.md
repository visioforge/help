---
title: Install VisioForge SDK on Delphi 64-bit — Setup Guide
description: Fix 64-bit BPL loading issues in Delphi IDE. Covers design-time vs runtime packages, library path configuration for x86/x64, and common error solutions.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL

---

# Mastering Delphi 64-bit Package Installation

## Introduction to 64-bit Development in Delphi

The evolution to 64-bit computing represents a significant advancement for Delphi developers, opening doors to enhanced performance, expanded memory addressing capabilities, and improved resource utilization. Since the introduction of 64-bit support in Delphi XE2, developers have gained the powerful ability to compile native 64-bit Windows applications. This capability enables software to harness modern hardware architectures, access substantially larger memory spaces, and deliver optimized performance for data-intensive operations.

However, this technological progression introduces a distinctive set of complexities, particularly regarding the installation and management of component packages (`.bpl` files). Many Delphi developers encounter perplexing obstacles when attempting to integrate 64-bit packages into their development workflow, leading to frustration and lost productivity.

This in-depth guide explores these challenges thoroughly and provides meticulously detailed, actionable solutions. The fundamental issue originates from a critical architectural characteristic: **the classic Delphi Integrated Development Environment (IDE), `bin\bds.exe`, is a 32-bit application**. RAD Studio 12.3 Athens added an optional 64-bit IDE that installs side by side with it (the 32-bit IDE remains the default in 12.3), and RAD Studio 13 Florence includes the 64-bit IDE as well. This guide describes the 32-bit IDE; in the 64-bit IDE the package rule is reversed, see [Using the 64-bit IDE](#64-bit-ide). This architectural discrepancy between the 32-bit IDE and the 64-bit compilation target creates numerous misunderstandings and technical difficulties related to package management.

Understanding this architectural limitation constitutes the essential first step toward establishing a seamless development experience. We will thoroughly examine why the 32-bit IDE requires 32-bit design-time packages, explore proper project configuration techniques for both 32-bit and 64-bit targets, clarify the critical function of runtime packages, and outline extensive testing methodologies to ensure your applications perform flawlessly across both architectural environments.

## The Architectural Limitation: Why the 32-bit IDE Requires 32-bit Design-Time Packages

### Understanding the IDE's Architecture

The Delphi IDE serves as the principal environment for visual component design, code editing, debugging operations, and comprehensive project management. When designers place components onto forms using the Form Designer, modify properties through the Object Inspector, or utilize specialized component editors, the IDE must load and execute code contained within the component's design-time package.

Because the classic `bin\bds.exe` (the 32-bit Delphi IDE executable) operates as a 32-bit process, it functions exclusively within the 32-bit memory address space and must adhere to the constraints of 32-bit execution environments. The IDE physically cannot load or execute 64-bit code directly—this represents a hardware and operating system limitation, not merely a software restriction. Any attempt to load a 64-bit DLL (or in Delphi terminology, a 64-bit `.bpl` package) into a 32-bit process will result in immediate failure, typically manifesting as error messages like "Can't load package %s" or obscure operating system error codes.

### Critical Design-Time Requirements

For the IDE to function properly during design activities—enabling visual component manipulation, property configuration, and utilization of design-time features—it *must* load the **32-bit (x86)** version of component packages. This requirement is non-negotiable due to the fundamental architecture of the IDE and operating system memory management principles.

This architectural limitation frequently leads to confusion among developers, creating misconceptions that only 32-bit packages are necessary, or generating questions about why separate 64-bit packages exist if the IDE cannot utilize them. The critical distinction lies in understanding the separation between **design time** operations (occurring within the 32-bit IDE) and **compile/run time** processes (where applications can target either 32-bit or 64-bit architectures).

## Step-by-Step Implementation: Installing 32-bit Design-Time Packages

### Essential First Step: Installing 32-bit Components

Based on the architectural explanation above, the mandatory initial step in the 32-bit IDE always involves installing the 32-bit version of component packages. For the 64-bit IDE, see [Using the 64-bit IDE](#64-bit-ide). This process establishes the foundation for all subsequent development activities.

1. **Acquire Necessary Package Files:** Ensure you possess both 32-bit and 64-bit compiled package files (`.bpl` and `.dcp`). The 32-bit files typically carry identifier suffixes such as `_x86`, `_Win32`, or may lack platform specifiers in older Delphi versions. Conversely, 64-bit packages normally include `_x64` or `_Win64` designations. These files typically generate automatically when building component library projects targeting both Win32 and Win64 platforms. When using third-party components, reputable vendors should supply both architectural versions.

2. **Launch Development Environment:** Start the Delphi IDE with appropriate user permissions.

3. **Access Package Installation Interface:** Navigate through the menu system to `Component > Install Packages...`.

4. **Initiate Package Addition:** Click the "Add..." button to begin the installation process.

5. **Locate 32-bit Package Files:** Browse to the directory containing your **32-bit** compiled package files (`.bpl`). Carefully select the 32-bit `.bpl` file and click "Open" to proceed.

6. **Complete Installation Process:** The package should appear in the "Design packages" list, typically enabled by default. Confirm the installation by clicking "OK".

### Verification and Troubleshooting

The IDE will attempt to load the 32-bit package. When successful, your components should appear in the Tool Palette, enabling immediate use in the Form Designer. If the IDE fails to load the package, verify that you selected the correct 32-bit `.bpl` file and ensure that all dependency packages required by your target package are properly installed and accessible.

**Critical Warning:** In the 32-bit IDE, never attempt to install 64-bit `.bpl` files using the `Component > Install Packages...` menu option. Such attempts will invariably fail because the 32-bit IDE architecture cannot load 64-bit code modules.

## Using the 64-bit IDE (RAD Studio 12.3 and Later) { #64-bit-ide }

RAD Studio 12.3 Athens introduced a 64-bit IDE as an optional "Initial Release": you add it in the Manage Platforms dialog and start it from the "RAD Studio (64-bit Initial Release)" Start menu shortcut. It runs from `bin64\bds.exe` and is installed side by side with the 32-bit IDE, which remains the default in 12.3. RAD Studio 13 Florence includes the 64-bit IDE as well. The 64-bit IDE has the opposite constraint to the one described above:

- **Its design-time packages are 64-bit.** The 64-bit IDE cannot load a 32-bit `.bpl`, so a 32-bit package does not appear on its palette. Install the **64-bit** package there.
- **It targets Win64 only.** It compiles and debugs Delphi Win64 (and C++ Win64 Modern) projects. When you open a project, it adds and activates the Win64 platform if it is missing and hides the other platforms without removing them. Build Win32 targets in the 32-bit IDE.
- **Each IDE keeps its own package list.** Known packages are stored separately for the 32-bit and 64-bit IDE, so a package installed in one does not appear in the other. Install the matching package in each IDE you use.

To install the VisioForge components in the 64-bit IDE:

1. Open the package project (`.dproj` or `.dpk`) from the x64 package folder for your Delphi version, for example `Packages\Delphi 13 x64`.
2. In the Project Manager, right-click the package and select `Compile`.
3. Once it compiles, right-click it again and select `Install`.

Keep 64-bit packages in their own folder, separate from the 32-bit ones, and do not copy them into the IDE's `bin64` folder. Embarcadero's [64-bit IDE](https://docwiki.embarcadero.com/RADStudio/en/64-bit_IDE) page has the full details.

## Advanced Configuration: Setting Project Library Paths for Dual Platform Development

### Configuring Compiler Search Paths

While the IDE utilizes 32-bit packages during design-time operations, the Delphi compiler requires precise information about where to locate appropriate files (`.dcu`, `.dcp`, `.obj`) for your specific target platform during compilation (either 32-bit or 64-bit). These settings are configured through project options, specifically within the library path configuration section. Importantly, these settings must be established separately for each target platform.

1. **Access Project Configuration:** Navigate to `Project > Options...` in the IDE menu.

2. **Select Appropriate Platform:** It is absolutely crucial to configure paths separately for each target platform. Utilize the "Target Platform" dropdown menu located at the top of the Project Options dialog. Begin configuration with the "32-bit Windows" selection.

3. **Navigate to Library Configuration Section:** In the options tree displayed on the left side, select `Delphi Compiler > Library` to access path settings.

4. **Configure 32-bit Library Paths:** Within the "Library path" field, click the ellipsis (...) button to open the path editor. Add the directory containing your compiled **32-bit** units (`.dcu` files) and the **32-bit** package's `.dcp` file for the components you've installed. Ensure this path specifically references the 32-bit output directory of your component library.

5. **Switch to 64-bit Configuration:** Change the "Target Platform" dropdown selection to "64-bit Windows". Notice that the "Library path" field might display different content or appear empty.

6. **Configure 64-bit Library Paths:** Repeat the previous path configuration process, but this time add directories containing your compiled **64-bit** units (`.dcu` files) and the **64-bit** package's `.dcp` file. This path *must* differ from the 32-bit path and correctly reference the 64-bit output directory.

7. **Review Additional Path Settings:** While the Library path configuration is essential for locating `.dcu` and `.dcp` files, also examine the `Browsing path` settings (used by code insight features) and verify the `DCP output directory` location is properly configured if you are building packages yourself. Configure these paths for both 32-bit and 64-bit platforms as well.

8. **Save Configuration Changes:** Click "OK" to preserve the project options settings.

### Avoiding Common Configuration Errors

**Frequent Mistake:** Many developers forget to switch the "Target Platform" dropdown *before* setting the path for that platform. Configuring the 64-bit path while "32-bit Windows" remains selected (or vice-versa) represents a common source of compilation errors later in the development process.

By correctly establishing these platform-specific library paths, you provide the compiler with precise information about where to locate necessary `.dcu` and `.dcp` files for the architecture currently under construction.

## Runtime Package Management Strategies

### Deciding on Linking Approaches

Beyond instructing the compiler where to find units during compilation, you must determine how your final executable will link against component libraries. This critical decision is controlled through the "Runtime Packages" settings section.

You have two principal options:

1. **Static Linking Approach:** If you leave the "Link with runtime packages" option unchecked (or remove all packages from the list), the compiler will directly incorporate necessary code and resources from your components into the final `.exe` file. This approach produces larger executable files but eliminates the requirement to distribute separate `.bpl` files alongside your application.

2. **Dynamic Linking (Runtime Packages) Approach:** If you enable "Link with runtime packages" and specify required packages, the compiler will *not* embed component code into your `.exe`. Instead, your application will dynamically load necessary `.bpl` files during execution. This strategy creates smaller executable files but requires deploying corresponding 32-bit or 64-bit `.bpl` files with your application distribution.

### Detailed Configuration Process

1. **Access Project Options:** Navigate to `Project > Options...` in the IDE menu.

2. **Select Target Platform:** Choose either "32-bit Windows" or "64-bit Windows" from the platform dropdown.

3. **Navigate to Package Settings:** Select `Packages > Runtime Packages` in the options navigation tree.

4. **Configure Linking Method:** Enable or disable the "Link with runtime packages" option based on your preferred linking approach determined earlier.

5. **Specify Required Packages:** When utilizing runtime packages, ensure the list contains the correct base names of packages your application requires (e.g., `MyComponentPackage`). Do *not* include platform suffixes or file extensions in these entries. Delphi automatically appends appropriate platform identifiers and loads the correct `_x86.bpl` or `_x64.bpl` files (or equivalent naming based on Delphi version/settings) during runtime.

6. **Configure Secondary Platform:** Switch the "Target Platform" selection and configure runtime package settings identically for the alternative platform. Typically, the decision to use or not use runtime packages remains consistent across both platforms, but package lists might differ if utilizing platform-specific libraries.

7. **Preserve Configuration:** Click "OK" to save the settings.

### Deployment Considerations

**Critical Deployment Requirement:** If you choose dynamic linking with runtime packages, remember that you *must* distribute the correct architectural version (32-bit or 64-bit) of those `.bpl` files with your application. The 32-bit executable requires 32-bit `.bpl` files, while the 64-bit executable needs 64-bit `.bpl` files. Place these files either in the same directory as the `.exe` or in locations accessible through the system's PATH environment variable.

## Comprehensive Testing and Verification Methodologies

### Multi-platform Verification

Configuration alone cannot guarantee success. Thorough testing becomes essential to confirm that everything functions as expected across both target platforms.

1. **Multi-platform Compilation:** Build your project explicitly for both "32-bit Windows" and "64-bit Windows" target platforms. Address any compiler errors that emerge during this process. Errors occurring during compilation frequently indicate incorrectly configured library paths (detailed in Step 2).

2. **32-bit Execution Testing:** Execute the compiled 32-bit application. Thoroughly test all functionality that depends on the components in question. Specifically look for:
   * Proper visual appearance and interactive behavior of components.
   * Absence of exceptions during component instantiation or method invocation.
   * If using runtime packages, verify the application launches without "Package XYZ not found" error messages.

3. **64-bit Execution Testing:** Execute the compiled 64-bit application. Perform identical tests as conducted with the 32-bit version. Pay particular attention to:
   * Any behavioral differences compared to the 32-bit version.
   * Runtime errors such as Access Violations, which might indicate underlying 64-bit compatibility issues in the component code or application logic (e.g., incorrect pointer arithmetic, integer size assumptions).
   * For runtime packages, check again for missing package errors, ensuring 64-bit `.bpl` files are properly accessible.

4. **Edge Case Evaluation:** Include testing scenarios that explore boundary conditions, particularly regarding memory usage if that represents a motivation for transitioning to 64-bit. Load extensive datasets and perform complex operations involving the components to stress-test the implementation.

### Interpreting Test Results

Any discrepancies or errors encountered during runtime on one platform but not the other strongly suggest either a problem in package configuration (Steps 2 or 3) or potential 64-bit compatibility issues within the component or application code itself. Such issues require careful diagnosis and targeted resolution.

## Advanced Troubleshooting Guide

### Resolving Common Installation Issues

* **"Package XYZ.bpl can't be installed because it is not a design time package."**: This error typically indicates an attempt to install a package via `Component > Install Packages` that lacks necessary design-time registrations or configuration flags. Verify that the package project is correctly configured as a design-time package or combined design-time & runtime package.

* **"Can't load package XYZ.bpl. %1 is not a valid Windows application." / "The specified module could not be found."**: This almost certainly indicates an attempt to install a **64-bit** BPL into the 32-bit IDE via `Component > Install Packages`. Remember to install only 32-bit BPL files through this interface in the 32-bit IDE; the 64-bit IDE needs the 64-bit BPL instead (see [Using the 64-bit IDE](#64-bit-ide)). The "module not found" variant may also occur if the package has dependencies that aren't properly installed or cannot be located.

* **[Compiler Error] F1026 File not found: 'ComponentUnit.dcu'**: This error occurs during compilation (not at design time). It indicates the compiler cannot locate the required `.dcu` file for the currently selected target platform. Carefully review your `Project Options > Delphi Compiler > Library > Library path` settings for the *specific platform* you are currently compiling (Step 2). Ensure the path correctly references the appropriate directory (32-bit or 64-bit) containing the necessary `.dcu` files.

* **[Linker Error] E2202 Required package 'XYZ' not found**: Similar to F1026, but occurring during the linking phase. This frequently indicates the `.dcp` file for the package cannot be found. Verify the Library Path (Step 2) includes the directory containing the correct platform's `.dcp` file. Additionally, ensure the package name appears correctly in `Project Options > Packages > Runtime Packages` if utilizing dynamic linking (Step 3).

* **Runtime Error: "Package XYZ not found"**: This indicates your application was compiled to use runtime packages, but the required `.bpl` file (matching the application's architecture) cannot be located during application startup. Ensure the correct 32-bit or 64-bit `.bpl` files are deployed alongside your `.exe` file (as described in Step 3).

* **Runtime Access Violations (AVs) only in 64-bit:** This typically indicates 64-bit compatibility issues in the code (either in your application or the component implementation). Common sources include:
  * Pointer arithmetic assuming `SizeOf(Pointer)=4` (valid only in 32-bit code).
  * Incorrect use of `Integer` instead of `NativeInt`/`NativeUInt` for handles or pointer-sized values.
  * Direct calls to Windows API functions using incorrect data types for 64-bit environments.
  * Data structure alignment issues.
  
  Debugging the 64-bit application becomes necessary to identify the specific cause of these violations.

## Working with Third-Party Component Packages

### Best Practices for External Components

The principles outlined throughout this guide apply equally to third-party components. Reputable component vendors typically provide:

1. Detailed instructions for proper installation procedures.
2. Separate 32-bit and 64-bit compiled `.bpl`, `.dcp`, and `.dcu` files.
3. An installation utility that handles file placement in appropriate locations and potentially automates the installation of 32-bit design-time packages into the IDE.

If an installer is provided, utilize it as your first approach. However, always validate project options (Library Paths, Runtime Packages) afterward, as installers may not perfectly configure paths for every possible project configuration or Delphi version. If you receive only raw library files without an installer, follow Steps 1-3 manually, carefully identifying and configuring paths for both 32-bit and 64-bit versions supplied by the vendor. When encountering issues, consult the vendor's documentation or contact their technical support team for assistance.

## Summary and Recommendations

### Key Implementation Strategies

Successfully managing Delphi packages for both 32-bit and 64-bit development fundamentally depends on knowing the bitness of the IDE you design in and meticulously configuring project options for each target platform independently. Install the design-time package that matches the IDE (32-bit for the classic IDE, 64-bit for the 64-bit IDE), then carefully establish platform-specific Library Paths and Runtime Package settings to ensure the compiler and your final application can locate and utilize the correct files for the target architecture.

While this approach introduces additional complexity compared to purely 32-bit development, the structured methodology enables you to leverage the substantial benefits of 64-bit compilation while maintaining a fully functional design-time experience within the familiar Delphi IDE environment. Consistent testing across both platforms represents the final, crucial verification step to guarantee robust, reliable applications that perform optimally in both 32-bit and 64-bit environments.

---
Need additional information? Please [contact support](https://helpdesk.visioforge.com/helpdesk/my-tickets/new) for assistance with specific scenarios or component issues.