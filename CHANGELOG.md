# Changelog

[English](#english) | [فارسی](#فارسی)

---

<a id="english"></a>

# English

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

---

<a id="فارسی"></a>

# فارسی

تمام تغییرات مهم **HV Game Launcher** در این فایل ثبت می‌شوند.

## [1.4.0 Beta] - 2026-11-17

### امکانات جدید
- طراحی مجدد رابط کاربری با الهام از لانچرهای مدرن بازی.
- اضافه شدن امکان استفاده از تصویر پس‌زمینه دلخواه.
- اضافه شدن پشتیبانی از تصاویر کاور عمودی بازی‌ها.
- اضافه شدن ۱۰ رنگ قابل انتخاب برای دکمه Launch.
- اضافه شدن رنگ سفید به گزینه‌های رنگ دکمه Launch.
- اضافه شدن نوار ابزار جدید در سمت راست:
  - شخصی‌سازی (Customize)
  - حالت تاریک / روشن (Dark / Light Mode)
  - بررسی به‌روزرسانی‌ها (Check for Updates)
  - تلگرام (Telegram)
  - درباره من (About Me)
- اضافه شدن منوی Launch با گزینه‌های جداگانه برای اجرای اسکریپت‌های VBS و اجرای بازی.
- اضافه شدن امکان تنظیم مسیر تصویر پس‌زمینه از طریق فایل `config.ini`.

### بهبودها
- طراحی مجدد چیدمان و ظاهر لانچر.
- بهبود نحوه نمایش و موقعیت تصویر کاور.
- حذف فضای خالی غیرضروری اطراف تصاویر کاور.
- بهبود اندازه و موقعیت دکمه Launch.
- هماهنگ شدن رنگ عنوان لانچر با رنگ انتخاب‌شده.
- بهبود نمایش تصویر پس‌زمینه برای جلوگیری از کشیدگی تصویر.
- ذخیره تنظیمات ظاهری به‌صورت محلی برای هر نمونه از لانچر.

### تنظیمات
- اضافه شدن گزینه `Background` به بخش `[Appearance]`.
- ادامه پشتیبانی از مسیرهای نسبی و مطلق فایل‌ها.

نمونه:

```ini
[Appearance]
Icon=launcher.png
Cover=cover.jpg
Background=background.jpg
```

### وضعیت انتشار
- اولین نسخه آزمایشی رابط کاربری بازطراحی‌شده.
- بهبودها و اصلاحات بیشتری برای رابط کاربری در نسخه‌های آینده برنامه‌ریزی شده‌اند.

## [1.3.0] - 2026-11-16

### رفع مشکلات
- رفع مشکل ذخیره نشدن تنظیمات حالت تاریک پس از بستن لانچر.

### بهبودها
- لانچر اکنون تم انتخاب‌شده را به خاطر می‌سپارد و در اجرای بعدی بازیابی می‌کند.
- تنظیمات تم به‌صورت محلی در پوشه AppData ویندوز کاربر ذخیره می‌شوند.

## [1.2.0]

### امکانات جدید
- اضافه شدن پشتیبانی از حالت تاریک (Dark Mode).
- اضافه شدن گزینه تغییر بین حالت روشن و تاریک.

### بهبودها
- رنگ لینک‌های GitHub و Telegram متناسب با تم انتخاب‌شده تغییر می‌کند:
  - سبز در حالت روشن.
  - آبی در حالت تاریک.
- حذف خط زیر لینک‌های تبلیغاتی.
- بهبود چیدمان سربرگ و موقعیت گزینه تغییر حالت تاریک برای جلوگیری از تداخل کنترل‌ها.

## [1.1.2]

### بهبودها
- به‌روزرسانی رنگ لینک‌های تبلیغاتی برای حالت روشن و تاریک.
- حذف خط زیر لینک‌های GitHub و Telegram.

## [1.1.1]

### رفع مشکلات
- رفع مشکل قرار گرفتن کنترل تغییر حالت تاریک در زیر چیدمان سربرگ.

### بهبودها
- اصلاح چیدمان سربرگ و موقعیت کنترل‌ها.

## [1.0.0] - انتشار اولیه

### امکانات
- اجرای مستقیم بازی‌ها از طریق لانچر.
- اجرای اسکریپت‌های BAT، CMD و VBS.
- امکان شخصی‌سازی عنوان پنجره برنامه.
- امکان انتخاب آیکون اختصاصی برنامه.
- امکان انتخاب تصویر کاور دلخواه.
- پشتیبانی از مسیرهای نسبی و مطلق فایل‌ها.
- برنامه‌ای سبک مبتنی بر Windows Forms و .NET 8.
