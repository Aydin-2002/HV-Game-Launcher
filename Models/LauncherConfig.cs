namespace GenericGameLauncher.Models;

public sealed class LauncherConfig
{
    public string WindowTitle { get; set; } = "";
    public string VBSFile { get; set; } = "";
    public string GameEXE { get; set; } = "";
    public string Cover { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Logo { get; set; } = "";
    public string Background { get; set; } = "";
    public string BackgroundLight { get; set; } = "";
    public string BackgroundDark { get; set; } = "";
}
