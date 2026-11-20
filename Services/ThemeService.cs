using System.Security.Cryptography;
using System.Text;

namespace GenericGameLauncher.Services;

public interface IThemeService
{
    bool Load();
    void Save(bool dark);
}

public sealed class ThemeService : IThemeService
{
    private readonly string root = AppContext.BaseDirectory;
    private string PathPref()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string identity = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(root)))[..16];
        string directory = Path.Combine(appData, "HVGameLauncher");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"{identity}.settings");
    }

    public bool Load()
    {
        try
        {
            string path = PathPref();
            return File.Exists(path) && File.ReadAllText(path, Encoding.UTF8).Trim().Equals("DarkMode=true", StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    public void Save(bool dark)
    {
        try { File.WriteAllText(PathPref(), dark ? "DarkMode=true" : "DarkMode=false", Encoding.UTF8); }
        catch { }
    }
}
