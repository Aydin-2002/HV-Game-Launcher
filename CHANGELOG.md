# Changelog

[English](#english) | [فارسی](#فارسی)

---

<a id="english"></a>

# English

All notable changes to **HV Game Launcher** are documented in this file.

[2.0.0] - Final Release - 2026-11-20

English

This is the final release of HV Game Launcher. The application was rewritten from the ground up, replacing the original Windows Forms implementation with a modern Avalonia-based launcher while preserving and expanding its core functionality.

###Added

- Added a redesigned launcher interface with a cover image, custom logo, background image, and circular toolbar.

- Added support for 10 customizable accent colors.

- Added Light and Dark themes.

- Added a Launch menu with separate options to run the game or its VBS script.

- Added customizable cover, logo, application icon, and background image support.

- Added configuration options in config.ini for game paths and appearance assets.

- Added support for relative and absolute file paths.

###Improved

- Rebuilt the application using Avalonia and .NET 8.

- Improved image scaling and clipping so cover and background images stay within their display areas.

- Refined the launcher layout, panels, buttons, and icons.

- Improved theme switching and saved appearance preferences locally.

###Requirements

Windows x64

.NET 8 Desktop Runtime


## [1.4.0] - 2026-11-17

### Added
- Added support for a custom transparent logo above the game cover, including PNG and ICO image files.
- Added support for custom launcher background images.
- Added support for vertical game cover images.
- Added 10 customizable Launch button colors, including white.
- Added a five-button circular toolbar for customization, theme switching, update checking, Telegram, and About.
- Added a Launch menu with separate options for launching the game and running a VBS script.
- Added current-version information alongside the latest GitHub release version.
- Added `Logo` and `Background` options to the `[Appearance]` section of `config.ini`.

### Improved
- Redesigned the launcher interface with a modern game-launcher appearance.
- Removed the centered title and placed the custom logo above the cover.
- Improved cover image scaling, positioning, and spacing.
- Refined the Launch button size and moved it to the bottom-right area of the launcher.
- Improved the right-side toolbar layout and circular icon presentation.
- Made the launcher title color follow the selected accent color.
- Improved background image rendering to avoid unwanted stretching.
- Added smooth animated transitions between Light and Dark themes.
- Optimized theme transitions and repainting to reduce flickering, stuttering, and unnecessary redraws.
- Saved appearance preferences locally for each launcher instance.
- Preserved support for relative and absolute file paths.

### Fixed
- Fixed visual flickering and background glitches during theme changes.
- Improved the stability and smoothness of switching between Light and Dark themes.

### Release
- First stable release of the redesigned HV Game Launcher.

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
[2.0.0] - Final Release - 2026-11-20

این نسخه، انتشار نهایی HV Game Launcher است. برنامه از پایه بازنویسی شده و پیاده‌سازی اولیهٔ Windows Forms با لانچری مدرن مبتنی بر Avalonia جایگزین شده است؛ قابلیت‌های اصلی حفظ و امکانات ظاهری آن گسترش یافته‌اند.

###امکانات جدید

- اضافه شدن رابط کاربری بازطراحی‌شده با تصویر کاور، لوگوی اختصاصی، تصویر پس‌زمینه و نوار ابزار دایره‌ای.

- اضافه شدن ۱۰ رنگ قابل انتخاب برای شخصی‌سازی رنگ اصلی برنامه.

- اضافه شدن تم‌های روشن و تاریک.

- اضافه شدن منوی Launch با گزینه‌های جداگانه برای اجرای بازی و اجرای فایل VBS.

- اضافه شدن امکان استفاده از تصویر کاور، لوگو، آیکون برنامه و تصویر پس‌زمینهٔ دلخواه.

- اضافه شدن گزینه‌های تنظیم مسیر بازی و تصاویر ظاهری به فایل config.ini.

- پشتیبانی از مسیرهای نسبی و مطلق فایل‌ها.

###بهبودها

- بازنویسی برنامه با استفاده از Avalonia و .NET 8.

- بهبود مقیاس‌بندی و برش تصاویر تا کاور و پس‌زمینه از محدودهٔ نمایش خود بیرون نزنند.

- اصلاح چیدمان لانچر، پنل‌ها، دکمه‌ها و آیکون‌ها.

- بهبود تغییر تم و ذخیرهٔ محلی تنظیمات ظاهری.

###پیش‌نیازها

ویندوز ۶۴ بیتی

.NET 8 Desktop Runtime


## [1.4.0] - 2026-11-17

### امکانات جدید
- اضافه شدن امکان نمایش لوگوی اختصاصی و شفاف در بالای تصویر کاور، با پشتیبانی از فایل‌های PNG و ICO.
- اضافه شدن امکان استفاده از تصویر پس‌زمینه دلخواه برای لانچر.
- اضافه شدن پشتیبانی از تصاویر کاور عمودی بازی‌ها.
- اضافه شدن ۱۰ رنگ قابل انتخاب برای دکمه Launch، شامل رنگ سفید.
- اضافه شدن نوار ابزار دایره‌ای شامل پنج دکمه برای شخصی‌سازی، تغییر تم، بررسی به‌روزرسانی، تلگرام و درباره برنامه.
- اضافه شدن منوی Launch با گزینه‌های جداگانه برای اجرای بازی و اجرای اسکریپت VBS.
- اضافه شدن نمایش نسخه فعلی لانچر در کنار آخرین نسخه منتشرشده در گیت‌هاب.
- اضافه شدن گزینه‌های `Logo` و `Background` به بخش `[Appearance]` در فایل `config.ini`.

### بهبودها
- بازطراحی رابط کاربری با ظاهری مدرن و الهام‌گرفته از لانچرهای بازی.
- حذف عنوان وسط صفحه و قرارگیری لوگوی اختصاصی در بالای کاور.
- بهبود مقیاس‌بندی، موقعیت و فاصله تصویر کاور.
- اصلاح اندازه دکمه Launch و انتقال آن به قسمت پایین سمت راست لانچر.
- بهبود چیدمان نوار ابزار سمت راست و ظاهر دایره‌ای آیکون‌ها.
- هماهنگ شدن رنگ عنوان لانچر با رنگ انتخاب‌شده.
- بهبود نمایش تصویر پس‌زمینه برای جلوگیری از کشیدگی نامناسب تصویر.
- اضافه شدن انتقال انیمیشنی و روان بین تم‌های روشن و تاریک.
- بهینه‌سازی تغییر تم و بازترسیم اجزای رابط کاربری برای کاهش پرش تصویر، لگ و بازترسیم‌های غیرضروری.
- ذخیره تنظیمات ظاهری به‌صورت محلی و مستقل برای هر نمونه از لانچر.
- حفظ پشتیبانی از مسیرهای نسبی و مطلق فایل‌ها.

### رفع مشکلات
- رفع پرش تصویر و مشکلات پس‌زمینه هنگام تغییر تم.
- بهبود پایداری و روانی جابه‌جایی بین حالت روشن و تاریک.

### وضعیت انتشار
- انتشار اولین نسخه پایدار از رابط کاربری بازطراحی‌شده HV Game Launcher.

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
