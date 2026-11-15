# HV Game Launcher — Source Code

A lightweight Windows game launcher built with C# and Windows Forms on .NET 8.

## Features

- Launches the configured game executable directly.
- Runs a configured `.bat`, `.cmd`, or `.vbs` script.
- Supports a custom window icon in `.ico` or `.png` format.
- Supports a custom cover image.
- Accepts absolute paths or paths relative to the launcher folder.
- Builds as a self-contained, single-file Windows x64 application.

## Build requirements

- Windows 10 or Windows 11, 64-bit.
- .NET 8 SDK.

## Build

1. Install the .NET 8 SDK.
2. Keep `Program.cs`, `Launcher.csproj`, `Build.bat`, and `config.ini` together.
3. Run `Build.bat`.
4. The published application will be created at `Publish/Launcher.exe`.
5. `config.ini` is copied beside the published executable automatically.

## Configuration

Edit `config.ini` before running the launcher:

```ini
[Launcher]
GameName=Example Game
WindowTitle=HV Game Launcher

[Paths]
GameEXE=Game.exe
VBSFile=VBS.cmd

[Appearance]
Icon=launcher.png
Cover=cover.jpg
```

Relative paths are resolved from the folder containing `Launcher.exe`. Absolute paths are also supported. The icon and cover are optional. Keep any referenced image files beside the launcher or provide an absolute path.

`Play Game` starts the configured executable directly. `Run VBS` starts `.bat` and `.cmd` files through `cmd.exe`, or `.vbs` files through `wscript.exe`.

## Transparency

The source code is included so users can inspect how the launcher works and build it themselves. The launcher reads its settings from `config.ini` and starts only the executable or script path configured there. The source does not include any game files.

No open-source license is included in this package. If you want to grant others permission to modify or redistribute the code, add a license before publishing.
