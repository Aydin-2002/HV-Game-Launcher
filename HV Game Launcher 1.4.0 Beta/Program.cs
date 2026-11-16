using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace GenericGameLauncher;

internal sealed class ImagePanel : Panel
{
    private Image? fillImage;
    public bool FitContain { get; set; }
    public Image? FillImage
    {
        get => fillImage;
        set { fillImage = value; Invalidate(); }
    }

    public ImagePanel() => DoubleBuffered = true;

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (fillImage is null)
        {
            base.OnPaintBackground(e);
            return;
        }

        e.Graphics.Clear(BackColor == Color.Transparent ? Parent?.BackColor ?? Color.Black : BackColor);
        float scale = FitContain
            ? Math.Min((float)ClientSize.Width / fillImage.Width, (float)ClientSize.Height / fillImage.Height)
            : Math.Max((float)ClientSize.Width / fillImage.Width, (float)ClientSize.Height / fillImage.Height);
        float width = fillImage.Width * scale;
        float height = fillImage.Height * scale;
        var destination = new RectangleF((ClientSize.Width - width) / 2f,
                                         (ClientSize.Height - height) / 2f,
                                         width, height);
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        e.Graphics.DrawImage(fillImage, destination);
    }
}

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LauncherForm());
    }
}

internal sealed class LauncherForm : Form
{
    private readonly string root = AppContext.BaseDirectory;
    private readonly Dictionary<string, string> config = new(StringComparer.OrdinalIgnoreCase);
    private readonly string[] palette =
    {
        "#FFFFFF", "#E74C3C", "#E67E22", "#F1C40F", "#2ECC71",
        "#1ABC9C", "#3498DB", "#9B59B6", "#E84393", "#7F8C8D"
    };
    private readonly Color[] paletteColors;
    private ImagePanel mainPanel = null!;
    private Panel contentPanel = null!;
    private Panel rail = null!;
    private Panel coverHost = null!;
    private ImagePanel coverPanel = null!;
    private Button launchButton = null!;
    private Label titleLabel = null!;
    private bool dark;
    private Color launchColor = Color.White;
    private Image? backgroundImage;
    private Image? coverImage;

    public LauncherForm()
    {
        paletteColors = palette.Select(ColorTranslator.FromHtml).ToArray();
        LoadConfig();
        LoadLocalSettings();

        Text = string.IsNullOrWhiteSpace(Get("Launcher", "WindowTitle"))
            ? "HV Game Launcher" : Get("Launcher", "WindowTitle");
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1040, 620);
        MinimumSize = new Size(900, 540);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        DoubleBuffered = true;

        LoadWindowIcon();
        BuildInterface();
        ApplyTheme();
    }

    private void BuildInterface()
    {
        mainPanel = new ImagePanel { Dock = DockStyle.Fill, Padding = new Padding(14) };
        Controls.Add(mainPanel);

        rail = new Panel { Dock = DockStyle.Right, Width = 62, Padding = new Padding(5) };
        mainPanel.Controls.Add(rail);

        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
            Padding = new Padding(0), Margin = Padding.Empty
        };
        outer.BackColor = Color.Transparent;
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        mainPanel.Controls.Add(outer);

        coverHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        outer.Controls.Add(coverHost, 0, 0);
        coverPanel = new ImagePanel { BackColor = Color.Transparent, FitContain = true };
        coverHost.Controls.Add(coverPanel);
        coverHost.Resize += (_, _) => PositionCover();
        LoadCover();

        contentPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28, 18, 24, 18) };
        outer.Controls.Add(contentPanel, 1, 0);

        string gameName = Get("Launcher", "GameName");
        string heading = string.IsNullOrWhiteSpace(gameName) ? "HV Game Launcher" : gameName;

        var center = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        contentPanel.Controls.Add(center);

        launchButton = new Button
        {
            Text = "LAUNCH", Size = new Size(230, 60),
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            Anchor = AnchorStyles.None
        };
        launchButton.FlatAppearance.BorderSize = 0;
        launchButton.Click += (_, _) => ShowLaunchMenu();
        center.Controls.Add(launchButton);

        void PositionLaunchButton()
        {
            launchButton.Location = new Point(
                Math.Max(0, center.ClientSize.Width - launchButton.Width - 24),
                Math.Max(0, center.ClientSize.Height - launchButton.Height - 20));
        }
        center.Resize += (_, _) => PositionLaunchButton();
        Shown += (_, _) => PositionLaunchButton();

        titleLabel = new Label
        {
            Text = heading, Dock = DockStyle.Top, Height = 72, Width = 300,
            Font = new Font("Segoe UI", 23, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        contentPanel.Controls.Add(titleLabel);
        contentPanel.Controls.SetChildIndex(titleLabel, 0);

        AddRailButton("⚙", "Customize", Customize, 0);
        AddRailButton("◐", "Dark / Light Mode", ToggleTheme, 1);
        AddRailButton("↻", "Update Launcher", CheckForUpdates, 2);
        AddRailButton("✈", "Telegram", () => OpenLink("https://t.me/pcgaminghub"), 3);
        AddRailButton("ⓘ", "About Me", () => OpenLink("https://github.com/Aydin-2002"), 4);

        if (!string.IsNullOrWhiteSpace(Get("Appearance", "Background")))
            LoadBackground();
    }

    private void AddRailButton(string glyph, string tip, Action action, int index)
    {
        var button = new Button
        {
            Text = glyph, Size = new Size(46, 46),
            Location = new Point(3, 8 + index * 58),
            Font = new Font("Segoe UI Symbol", 17),
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            TabStop = false
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => action();
        var toolTip = new ToolTip();
        toolTip.SetToolTip(button, tip);
        rail.Controls.Add(button);
    }

    private void LoadCover()
    {
        string path = Resolve(Get("Appearance", "Cover"));
        if (File.Exists(path))
        {
            try
            {
                using var source = Image.FromFile(path);
                coverImage = new Bitmap(source);
                coverPanel.FillImage = coverImage;
                PositionCover();
                return;
            }
            catch { }
        }

        coverPanel.Size = new Size(250, 150);
        coverPanel.Controls.Add(new Label
        {
            Dock = DockStyle.Fill, Text = "COVER IMAGE",
            ForeColor = Color.Gainsboro, BackColor = Color.FromArgb(24, 24, 24),
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        });
        PositionCover();
    }

    private void PositionCover()
    {
        if (coverHost is null || coverPanel is null) return;

        int width = Math.Min(250, Math.Max(1, coverHost.ClientSize.Width));
        int height = coverImage is null
            ? 150
            : Math.Max(1, (int)Math.Round(width * (double)coverImage.Height / coverImage.Width));
        height = Math.Min(height, Math.Max(1, coverHost.ClientSize.Height));
        coverPanel.Size = new Size(width, height);
        coverPanel.Location = new Point(
            (coverHost.ClientSize.Width - width) / 2,
            (coverHost.ClientSize.Height - height) / 2);
    }

    private void LoadBackground()
    {
        string path = Resolve(Get("Appearance", "Background"));
        if (!File.Exists(path)) return;
        try
        {
            using var source = Image.FromFile(path);
            backgroundImage = new Bitmap(source);
            mainPanel.FillImage = backgroundImage;
            contentPanel.BackColor = Color.Transparent;
            rail.BackColor = Color.Transparent;
        }
        catch
        {
            MessageBox.Show(this, $"Background image could not be loaded:\r\n{path}",
                "Background image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void LoadWindowIcon()
    {
        string path = Resolve(Get("Appearance", "Icon"));
        if (!File.Exists(path)) return;
        try
        {
            if (Path.GetExtension(path).Equals(".ico", StringComparison.OrdinalIgnoreCase))
                Icon = new Icon(path);
            else if (Path.GetExtension(path).Equals(".png", StringComparison.OrdinalIgnoreCase))
                Icon = CreateIconFromPng(path);
        }
        catch { }
    }

    private void ShowLaunchMenu()
    {
        using var menu = new Form
        {
            Text = "Launch", StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false,
            MinimizeBox = false, ShowInTaskbar = false, ClientSize = new Size(300, 164)
        };
        var vbs = new Button { Text = "Run VBS", Bounds = new Rectangle(22, 22, 256, 48), Font = new Font("Segoe UI", 11) };
        var game = new Button { Text = "Play Game", Bounds = new Rectangle(22, 86, 256, 48), Font = new Font("Segoe UI", 11) };
        vbs.Click += (_, _) => { menu.DialogResult = DialogResult.OK; menu.Close(); RunScript(); };
        game.Click += (_, _) => { menu.DialogResult = DialogResult.OK; menu.Close(); LaunchGame(); };
        menu.Controls.AddRange(new Control[] { vbs, game });
        menu.ShowDialog(this);
    }

    private void Customize()
    {
        using var picker = new Form
        {
            Text = "Launch Button Color", StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false,
            MinimizeBox = false, ShowInTaskbar = false, ClientSize = new Size(310, 190)
        };
        var grid = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), WrapContents = true };
        picker.Controls.Add(grid);
        foreach (Color color in paletteColors)
        {
            var swatch = new Button
            {
                Size = new Size(46, 46), BackColor = color,
                FlatStyle = FlatStyle.Flat, Margin = new Padding(5),
                Cursor = Cursors.Hand
            };
            swatch.FlatAppearance.BorderSize = 1;
            swatch.Click += (_, _) =>
            {
                launchColor = color;
                SaveLocalSettings();
                ApplyTheme();
                picker.Close();
            };
            grid.Controls.Add(swatch);
        }
        picker.ShowDialog(this);
    }

    private void ToggleTheme()
    {
        dark = !dark;
        SaveLocalSettings();
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        Color background = dark ? Color.FromArgb(28, 30, 36) : Color.FromArgb(242, 244, 248);
        Color foreground = dark ? Color.FromArgb(238, 240, 245) : Color.FromArgb(35, 39, 47);
        mainPanel.BackColor = background;
        contentPanel.BackColor = backgroundImage is null ? background : Color.Transparent;
        rail.BackColor = backgroundImage is null ? background : Color.Transparent;
        // The title follows the selected Launch button color.
        titleLabel.ForeColor = launchColor;
        foreach (Control control in rail.Controls)
        {
            if (control is Button button)
            {
                button.BackColor = dark ? Color.FromArgb(45, 48, 56) : Color.FromArgb(228, 232, 239);
                button.ForeColor = foreground;
            }
        }
        launchButton.BackColor = launchColor;
        launchButton.ForeColor = launchColor.GetBrightness() > 0.78f ? Color.FromArgb(30, 34, 42) : Color.White;
        launchButton.FlatAppearance.MouseOverBackColor = ControlPaint.Light(launchColor, .12f);
        launchButton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(launchColor, .12f);
        launchButton.FlatAppearance.BorderSize = 0;
    }

    private void CheckForUpdates()
    {
        using var dialog = new Form
        {
            Text = "Launcher Update", StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false,
            MinimizeBox = false, ShowInTaskbar = false, ClientSize = new Size(390, 150)
        };
        var status = new Label
        {
            Dock = DockStyle.Top, Height = 72, TextAlign = ContentAlignment.MiddleCenter,
            Text = "Checking GitHub Releases..."
        };
        var open = new Button { Text = "Open Release Page", Dock = DockStyle.Bottom, Height = 38, Enabled = false };
        open.Click += (_, _) => OpenLink("https://github.com/Aydin-2002/HV-Game-Launcher/releases/latest");
        dialog.Controls.Add(status);
        dialog.Controls.Add(open);
        dialog.Shown += async (_, _) =>
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("HV-Game-Launcher");
                client.Timeout = TimeSpan.FromSeconds(8);
                string json = await client.GetStringAsync("https://api.github.com/repos/Aydin-2002/HV-Game-Launcher/releases/latest");
                using var document = JsonDocument.Parse(json);
                string tag = document.RootElement.TryGetProperty("tag_name", out var tagNode) ? tagNode.GetString() ?? "Unknown" : "Unknown";
                status.Text = $"Latest GitHub release: {tag}";
                open.Enabled = true;
            }
            catch
            {
                status.Text = "Could not check for updates.\r\nCheck your internet connection.";
                open.Enabled = true;
            }
        };
        dialog.ShowDialog(this);
    }

    private void RunScript()
    {
        string configured = Get("Paths", "VBSFile");
        if (string.IsNullOrWhiteSpace(configured))
        {
            MessageBox.Show(this, "Set Paths > VBSFile in config.ini first.", "Path not set", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        string path = Resolve(configured);
        if (!File.Exists(path))
        {
            MessageBox.Show(this, $"Script not found:\r\n{path}", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        try
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();
            ProcessStartInfo info;
            if (extension is ".bat" or ".cmd")
                info = new ProcessStartInfo(Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe", $"/c \"\"{path}\"\"")
                { WorkingDirectory = Path.GetDirectoryName(path)!, UseShellExecute = true };
            else if (extension == ".vbs")
                info = new ProcessStartInfo("wscript.exe", $"\"{path}\"")
                { WorkingDirectory = Path.GetDirectoryName(path)!, UseShellExecute = true };
            else
            {
                MessageBox.Show(this, "Supported script types: .bat, .cmd, .vbs", "Unsupported file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Process.Start(info);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Script launch failed", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void LaunchGame()
    {
        string configured = Get("Paths", "GameEXE");
        if (string.IsNullOrWhiteSpace(configured))
        {
            MessageBox.Show(this, "Set Paths > GameEXE in config.ini first.", "Path not set", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        string path = Resolve(configured);
        if (!File.Exists(path))
        {
            MessageBox.Show(this, $"Executable not found:\r\n{path}", "Game not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        try { Process.Start(new ProcessStartInfo(path) { WorkingDirectory = Path.GetDirectoryName(path)!, UseShellExecute = true }); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Launch failed", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void LoadConfig()
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

    private string Get(string section, string key) =>
        config.TryGetValue($"{section}.{key}", out string? value) ? value : "";

    private string Resolve(string value) =>
        string.IsNullOrWhiteSpace(value) ? "" : Path.IsPathRooted(value) ? value : Path.GetFullPath(Path.Combine(root, value));

    // Preferences live beside this launcher, so separate game folders keep independent settings.
    private string SettingsPath => Path.Combine(root, "launcher.settings");

    private void LoadLocalSettings()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return;
            foreach (string line in File.ReadAllLines(SettingsPath))
            {
                if (line.StartsWith("DarkMode=", StringComparison.OrdinalIgnoreCase))
                    dark = line.EndsWith("true", StringComparison.OrdinalIgnoreCase);
                else if (line.StartsWith("LaunchColor=", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        launchColor = ColorTranslator.FromHtml(line[(line.IndexOf('=') + 1)..]);
                        // Replace the previous default indigo with the new white palette entry.
                        if (launchColor.ToArgb() == ColorTranslator.FromHtml("#5865F2").ToArgb())
                        {
                            launchColor = Color.White;
                            SaveLocalSettings();
                        }
                    }
                    catch { }
                }
            }
        }
        catch { }
    }

    private void SaveLocalSettings()
    {
        try
        {
            File.WriteAllLines(SettingsPath, new[]
            {
                $"DarkMode={dark.ToString().ToLowerInvariant()}",
                $"LaunchColor={ColorTranslator.ToHtml(launchColor)}"
            }, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Could not save launcher settings:\r\n{ex.Message}",
                "Settings save failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static void OpenLink(string url)
    {
        try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
        catch (Exception ex) { MessageBox.Show($"Could not open the link:\r\n{ex.Message}", "Open link failed", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private static Icon CreateIconFromPng(string path)
    {
        using var source = new Bitmap(path);
        using var canvas = new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(canvas))
        {
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(source, new Rectangle(0, 0, 32, 32));
        }
        IntPtr handle = canvas.GetHicon();
        try
        {
            using var temporary = Icon.FromHandle(handle);
            return (Icon)temporary.Clone();
        }
        finally { DestroyIcon(handle); }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            backgroundImage?.Dispose();
            coverImage?.Dispose();
        }
        base.Dispose(disposing);
    }
}
