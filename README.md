[English](README.md) | [فارسی](README.fa.md)

# HV Game Launcher

A customizable Windows game launcher built with C#, Avalonia, and .NET 8.

## Features

- Launch the configured game executable.
- Run the configured VBS script from the Launch menu.
- Customize the launcher accent color and switch between Light and Dark themes.
- Use custom cover, logo, application icon, and separate Light and Dark background images.
- Configure game and image paths in `config.ini`.
- Use absolute paths or paths relative to the launcher folder.

## Requirements

### To build from source

- Windows x64
- .NET 8 SDK

### To run the published application

- Windows x64
- .NET 8 Runtime (x64)

The published application is framework-dependent and does not include the .NET runtime. Install the runtime separately. The source project currently targets `net8.0-windows` and `win-x64`.

## Build

1. Install the .NET 8 SDK.
2. Open a terminal in the folder containing `HV.GameLauncher.csproj`.
3. Run:

   ```powershell
   dotnet publish HV.GameLauncher.csproj -c Release
   ```

4. The published files are written to `Publish/HV Game Launcher/`.
5. Keep `config.ini` and any image files referenced by it beside `Launcher.exe`, or set absolute paths in the configuration.

## Configuration

Edit `config.ini` before launching the application:

```ini
[Paths]
GameEXE=
VBSFile=

[Appearance]
Icon=
Logo=
Cover=
BackgroundLight=
BackgroundDark=
```

Set `GameEXE` to the game's executable and `VBSFile` to the VBS script path. Appearance entries are optional. Use paths relative to the launcher folder or absolute paths. `BackgroundLight` and `BackgroundDark` let you specify separate background images for the two themes.

## About this project

HV Game Launcher is a personal hobby project created with assistance from the free version of ChatGPT. The author has no formal programming background and developed the application out of personal interest and curiosity, with help from AI.

## License

No open-source license is currently included. Unless a license is added, reuse, modification, and redistribution are not granted by this repository.
