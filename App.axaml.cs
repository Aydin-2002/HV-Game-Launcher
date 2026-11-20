using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using GenericGameLauncher.Services;
using GenericGameLauncher.ViewModels;
using GenericGameLauncher.Views;

namespace GenericGameLauncher;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        var sc = new ServiceCollection();
        sc.AddTransient<IConfigService, ConfigService>();
        sc.AddSingleton<IProcessLauncher, ProcessLauncher>();
        sc.AddSingleton<IThemeService, ThemeService>();
        sc.AddSingleton<ISettingsService, SettingsService>();
        sc.AddTransient<MainWindowViewModel>();
        Services = sc.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = vm };
        }
        base.OnFrameworkInitializationCompleted();
    }
}
