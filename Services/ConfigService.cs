using System.Text;
using GenericGameLauncher.Models;

namespace GenericGameLauncher.Services;

public sealed class ConfigService : IConfigService
{
    private readonly string root = AppContext.BaseDirectory;
    private readonly Dictionary<string, string> config = new(StringComparer.OrdinalIgnoreCase);

    public string Root => root;

    public ConfigService()
    {
        string path = Path.Combine(root, "config.ini");
        if (!File.Exists(path)) return;
        string section = "";
        foreach (string raw in File.ReadAllLines(path, Encoding.UTF8))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#")) continue;
            if (line.StartsWith("[") && line.EndsWith("]")) { section = line[1..^1].Trim(); continue; }
            int equals = line.IndexOf('=');
            if (equals > 0) config[$"{section}.{line[..equals].Trim()}"] = line[(equals + 1)..].Trim();
        }
    }

    public LauncherConfig Load()
    {
        return new LauncherConfig
        {
            WindowTitle = Get("Launcher", "WindowTitle"),
            VBSFile = Get("Paths", "VBSFile"),
            GameEXE = Get("Paths", "GameEXE"),
            Cover = Get("Appearance", "Cover"),
            Icon = Get("Appearance", "Icon"),
            Logo = Get("Appearance", "Logo"),
            Background = Get("Appearance", "Background"),
            BackgroundLight = Get("Appearance", "BackgroundLight"),
            BackgroundDark = Get("Appearance", "BackgroundDark"),
        };
    }

    private string Get(string section, string key) =>
        config.TryGetValue($"{section}.{key}", out string? value) ? value : "";

    public string ResolvePath(string value) =>
        string.IsNullOrWhiteSpace(value) ? "" : Path.IsPathRooted(value) ? value : Path.GetFullPath(Path.Combine(root, value));
}
