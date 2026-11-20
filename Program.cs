using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using GenericGameLauncher.Services;
using GenericGameLauncher.ViewModels;

namespace GenericGameLauncher;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
