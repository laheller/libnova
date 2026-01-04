# LibNova

**LibNova** is a cross-language & cross-platform .NET wrapper library for the original [libnova](https://libnova.sourceforge.net/index.html), a general purpose, double precision, Celestial Mechanics, Astrometry and Astrodynamics library.

This project provides:

- A slightly modified/fixed version **0.16.0** of the original libnova to be able to build on Windows using Visual Studio 2026 C/C++
- A C# (P/Invoke) wrapper for seamless integration with .NET applications
- A test program to show some of the wrapper's functionaliy

## Features
- The original libnova built as a shared library provides a clean C-language interface.
- P/Invoke-based C# wrapper for .NET applications.
- Self documentation for the C# wrapper.
- Access to all off the original C-language libnova API.
- Cross-platform support: Once the original library is built for Windows & Linux, the C# wrapper should work on both platform.

## Repository structure

LibNova/<br/>
├── clibnova/ -> the original libnova 0.16.0 as a Visual Studio 2026 project<br/>
├── LibNova/ -> .NET P/Invoke C# bindings as a .NET netstandard library project<br/>
├── Test/ -> Example C# console application<br/>
├── README.md -> this file<br/>
└── clibnova.slnx -> Visual Studio 2026 solution file to easily open/build everything<br/>

## Getting started

### Clone repository
```bash
git clone https://github.com/laheller/libnova.git
```

### Build
* Windows<br/>
&#9;Simply open `clibnova.slnx` in Visual Studio 2026 (Community Edition) that has both C++ and C# workloads installed & build. It should create the `clibnova.dll` shared library, build the .NET wrapper and the test console application.
* Linux<br/>
&#9;Look for the original libnova build instructions to build on Linux

### Use in a C# Project:
Add the compiled native library (.dll, .so) to your project output directory, where the main output .NET binary is located.

Reference the `LibNova` project or simply include the `LibNova.cs` source file in your project.

### Check the `Test` project for examples, how to use LibNova library.

## License
This project is licensed under the same terms as the original libnova library. Please refer to the original [license](https://libnova.sourceforge.net/index.html).

## Credits
**libnova** library by **Liam Girdwood** and **Petr Kubanek**.

C# wrapper by Ladislav Heller
