using System.Diagnostics;

namespace GenericGameLauncher.Services;

public interface IProcessLauncher
{
    void RunScript(string path);
    void LaunchGame(string path);
    void OpenLink(string url);
}

public sealed class ProcessLauncher : IProcessLauncher
{
    public void RunScript(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        ProcessStartInfo startInfo;
        if (extension is ".bat" or ".cmd")
        {
            startInfo = new ProcessStartInfo
            {
                FileName = Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe",
                Arguments = $"/c \"\"{path}\"\"",
                WorkingDirectory = Path.GetDirectoryName(path)!,
                UseShellExecute = true
            };
        }
        else if (extension == ".vbs")
        {
            startInfo = new ProcessStartInfo
            {
                FileName = "wscript.exe",
                Arguments = $"\"{path}\"",
                WorkingDirectory = Path.GetDirectoryName(path)!,
                UseShellExecute = true
            };
        }
        else throw new NotSupportedException("Supported script types: .bat, .cmd, .vbs");
        Process.Start(startInfo);
    }

    public void LaunchGame(string path)
    {
        Process.Start(new ProcessStartInfo(path) { WorkingDirectory = Path.GetDirectoryName(path)!, UseShellExecute = true });
    }

    public void OpenLink(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
