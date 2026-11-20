using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GenericGameLauncher.Views;

public partial class LaunchMenuWindow : Window
{
    public LaunchMenuWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
