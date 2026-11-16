# Changelog

All notable changes to **HV Game Launcher** are documented in this file.

## [1.4.0 Beta] - 2026-11-17

### Added
- Introduced a redesigned user interface inspired by modern game launchers.
- Added support for custom background images.
- Added support for vertical game cover images.
- Added 10 customizable Launch button colors.
- Added white as a Launch button color option.
- Added a new right-side toolbar:
  - Customize
  - Dark / Light Mode
  - Check for Updates
  - Telegram
  - About Me
- Added a Launch menu with separate options for running VBS scripts and launching the game.
- Added background image configuration through `config.ini`.

### Improved
- Redesigned the launcher layout and visual appearance.
- Improved cover image scaling and positioning.
- Removed unnecessary empty space around cover images.
- Improved Launch button positioning and sizing.
- Launcher title color now follows the selected accent color.
- Improved background image rendering to prevent stretching.
- Appearance settings are now stored locally for each launcher instance.

### Configuration
- Added the `Background` option to the `[Appearance]` section.
- Continued support for relative and absolute file paths.

Example:

```ini
[Appearance]
Icon=launcher.png
Cover=cover.jpg
Background=background.jpg
```

### Status
- First beta release of the redesigned interface.
- Further UI refinements and improvements are planned.

## [1.3.0] - 2026-11-16

### Fixed
- Fixed the Dark Mode preference not being saved after closing the launcher.

### Improved
- The launcher now remembers the selected theme and restores it on the next launch.
- Theme preference is stored locally in the user's Windows AppData folder.

## [1.2.0]

### Added
- Added Dark Mode support.
- Added a toggle to switch between Light Mode and Dark Mode.

### Improved
- GitHub and Telegram link colors now adapt to the selected theme:
  - Green in Light Mode.
  - Blue in Dark Mode.
- Removed underlines from promotional links.
- Improved the header layout and Dark Mode toggle positioning to prevent overlapping controls.

## [1.1.2]

### Improved
- Updated promotional link colors for Light Mode and Dark Mode.
- Removed underlines from GitHub and Telegram links.

## [1.1.1]

### Fixed
- Fixed the Dark Mode toggle being overlapped by the header layout.

### Improved
- Adjusted header layout and control positioning.

## [1.0.0] - Initial Release

### Features
- Launch games directly from the launcher.
- Run BAT, CMD, and VBS scripts.
- Customize the application window title.
- Set a custom application icon.
- Set a custom cover image.
- Support for relative and absolute file paths.
- Lightweight Windows Forms application built with .NET 8.
