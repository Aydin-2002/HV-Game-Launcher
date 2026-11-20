using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GenericGameLauncher.Views;

public partial class CustomizeWindow : Window
{
    public CustomizeWindow()
    {
        InitializeComponent();
        Deactivated += (_, _) => Close();
    }

    private void OnApplyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
