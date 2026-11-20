using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenericGameLauncher.Models;
using GenericGameLauncher.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace GenericGameLauncher.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IConfigService configService;
    private readonly IProcessLauncher processLauncher;
    private readonly ISettingsService settingsService;

    public LauncherConfig Config { get; }

    [ObservableProperty] private string windowTitle = "";
    [ObservableProperty] private bool isDarkMode;
    [ObservableProperty] private Bitmap? coverImage;
    [ObservableProperty] private Bitmap? logoImage;
    [ObservableProperty] private Bitmap? backgroundImage;
    [ObservableProperty] private string launchColor = "#7F8C8D";
    [ObservableProperty] private string launchButtonBackground = "#FFFFFF";
    [ObservableProperty] private string launchButtonForeground = "#000000";
    [ObservableProperty] private IBrush neonBrush = new SolidColorBrush(Color.Parse("#FFFFFF"));
    [ObservableProperty] private string iconPath = "";

    public bool HasDarkBackgroundImage => IsDarkMode &&
        !string.IsNullOrWhiteSpace(Config.BackgroundDark) &&
        File.Exists(configService.ResolvePath(Config.BackgroundDark));

    public IReadOnlyList<string> Palette { get; } = new[]
    {
        "#FFFFFF", "#E74C3C", "#E67E22", "#F1C40F", "#2ECC71",
        "#8D6E63", "#3498DB", "#9B59B6", "#E84393", "#7F8C8D"
    };

    public MainWindowViewModel(IConfigService configService, IProcessLauncher processLauncher, ISettingsService settingsService)
    {
        this.configService = configService;
        this.processLauncher = processLauncher;
        this.settingsService = settingsService;
        settingsService.Load();
        Config = configService.Load();
        WindowTitle = string.IsNullOrWhiteSpace(Config.WindowTitle) ? "HV Game Launcher" : Config.WindowTitle;

        LoadCoverImage();
        LoadLogoImage();
        LoadBackgroundImage();
        IconPath = configService.ResolvePath(Config.Icon);
        IsDarkMode = settingsService.DarkMode;
        LaunchColor = settingsService.LaunchColor;
        ApplyLaunchColorVisuals(LaunchColor);
        ApplyTheme(IsDarkMode);
    }

    private void LoadCoverImage()
    {
        var path = configService.ResolvePath(Config.Cover);
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            try { CoverImage = new Bitmap(path); } catch { }
    }

    private void LoadLogoImage()
    {
        var path = configService.ResolvePath(Config.Logo);
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            try { LogoImage = new Bitmap(path); } catch { }
    }

    private void LoadBackgroundImage()
    {
        var light = configService.ResolvePath(Config.BackgroundLight);
        var dark = configService.ResolvePath(Config.BackgroundDark);
        var legacy = configService.ResolvePath(Config.Background);
        string chosen = IsDarkMode ? dark : light;
        if (string.IsNullOrWhiteSpace(chosen)) chosen = legacy;
        BackgroundImage = null;
        if (!string.IsNullOrWhiteSpace(chosen) && File.Exists(chosen))
            try { BackgroundImage = new Bitmap(chosen); } catch { }
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        settingsService.DarkMode = value;
        settingsService.Save();
        LoadBackgroundImage();
        ApplyTheme(value);
        UpdateLaunchButtonAppearance();
        OnPropertyChanged(nameof(HasDarkBackgroundImage));
    }

    partial void OnLaunchColorChanged(string value)
    {
        ApplyLaunchColorVisuals(value);
        settingsService.LaunchColor = value;
        settingsService.Save();
    }

    private void ApplyLaunchColorVisuals(string value)
    {
        var accent = Color.Parse(value);
        LaunchButtonBackground = $"#55{accent.R:X2}{accent.G:X2}{accent.B:X2}";
        NeonBrush = new SolidColorBrush(accent);
        UpdateLaunchButtonAppearance();
    }

    private void UpdateLaunchButtonAppearance()
    {
        try
        {
            bool whiteAccent = Color.Parse(LaunchColor) == Color.Parse("#FFFFFF");
            LaunchButtonForeground = !IsDarkMode || whiteAccent ? "#000000" : "#FFFFFF";
        }
        catch { LaunchButtonForeground = "#FFFFFF"; }
    }

    private static void ApplyTheme(bool dark)
    {
        if (Application.Current is null) return;
        Application.Current.RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    [RelayCommand]
    private void RunScript() => RunScriptImpl();

    private void RunScriptImpl()
    {
        string configured = Config.VBSFile;
        if (string.IsNullOrWhiteSpace(configured)) { Show("Set Paths > VBSFile in config.ini first.", "Path not set"); return; }
        string path = configService.ResolvePath(configured);
        if (!File.Exists(path)) { Show($"Script not found:\r\n{path}", "File not found", Icon.Error); return; }
        try { processLauncher.RunScript(path); }
        catch (Exception ex) { Show(ex.Message, "Script launch failed", Icon.Error); }
    }

    [RelayCommand]
    private void LaunchGame()
    {
        string configured = Config.GameEXE;
        if (string.IsNullOrWhiteSpace(configured)) { Show("Set Paths > GameEXE in config.ini first.", "Path not set"); return; }
        string path = configService.ResolvePath(configured);
        if (!File.Exists(path)) { Show($"Executable not found:\r\n{path}", "Game not found", Icon.Error); return; }
        try { processLauncher.LaunchGame(path); }
        catch (Exception ex) { Show(ex.Message, "Launch failed", Icon.Error); }
    }

    [RelayCommand]
    private void ShowLaunchMenu()
    {
        var menu = new Views.LaunchMenuWindow { DataContext = this };
        menu.Deactivated += (_, _) => menu.Close();
        menu.Show();
    }

    [RelayCommand]
    private void CloseApp() => Environment.Exit(0);

    [RelayCommand]
    private void OpenLink(string url)
    {
        try { processLauncher.OpenLink(url); }
        catch (Exception ex) { Show($"Could not open the link:\r\n{ex.Message}", "Open link failed", Icon.Error); }
    }

    [RelayCommand]
    private void ToggleTheme() { IsDarkMode = !IsDarkMode; }

    [RelayCommand]
    private void Customize()
    {
        var window = new Views.CustomizeWindow { DataContext = this };
        window.Show();
    }

    [RelayCommand]
    private void SelectColor(string color)
    {
        LaunchColor = color;
    }

    [RelayCommand]
    private void ShowAboutMe() => OpenLink("https://github.com/Aydin-2002");

    private static void Show(string msg, string title, Icon icon = Icon.Info)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, msg, ButtonEnum.Ok, icon);
        box.ShowAsync();
    }
}

public sealed class BoolInverterConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
