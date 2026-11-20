using System.Text;

namespace GenericGameLauncher.Services;

public interface ISettingsService
{
    bool DarkMode { get; set; }
    string LaunchColor { get; set; }
    void Load();
    void Save();
    string Path { get; }
}

public sealed class SettingsService : ISettingsService
{
    private readonly string root = AppContext.BaseDirectory;
    public string Path => System.IO.Path.Combine(root, "launcher.settings");
    public bool DarkMode { get; set; }
    public string LaunchColor { get; set; } = "#7F8C8D";

    public void Load()
    {
        try
        {
            if (!System.IO.File.Exists(Path)) return;
            foreach (var line in System.IO.File.ReadAllLines(Path))
            {
                if (line.StartsWith("DarkMode=", StringComparison.OrdinalIgnoreCase))
                    DarkMode = line.EndsWith("true", StringComparison.OrdinalIgnoreCase);
                else if (line.StartsWith("LaunchColor=", StringComparison.OrdinalIgnoreCase))
                    LaunchColor = line[(line.IndexOf('=') + 1)..];
            }
        }
        catch { }
    }

    public void Save()
    {
        try
        {
            System.IO.File.WriteAllLines(Path, new[]
            {
                $"DarkMode={DarkMode.ToString().ToLowerInvariant()}",
                $"LaunchColor={LaunchColor}"
            }, Encoding.UTF8);
        }
        catch { }
    }
}
