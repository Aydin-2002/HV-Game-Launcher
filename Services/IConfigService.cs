using GenericGameLauncher.Models;

namespace GenericGameLauncher.Services;

public interface IConfigService
{
    LauncherConfig Load();
    string ResolvePath(string value);
    string Root { get; }
}
