using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace GenericGameLauncher;

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

    public LauncherForm()
    {
        LoadConfig();
        string gameName = Get("Launcher", "GameName");
        string windowTitle = Get("Launcher", "WindowTitle");
        Text = string.IsNullOrWhiteSpace(windowTitle) ? "HV Game Launcher" : windowTitle;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(900, 540);
        MinimumSize = new Size(760, 460);
        BackColor = Color.FromArgb(238, 238, 238);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        string iconPath = Resolve(Get("Appearance", "Icon"));
        if (File.Exists(iconPath))
        {
            try
            {
                string extension = Path.GetExtension(iconPath);
                if (extension.Equals(".ico", StringComparison.OrdinalIgnoreCase))
                    Icon = new Icon(iconPath);
                else if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                    Icon = CreateIconFromPng(iconPath);
            }
            catch { }
        }

        var outer = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 1, BackColor = BackColor };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Controls.Add(outer);

        var coverPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(21, 21, 21) };
        outer.Controls.Add(coverPanel, 0, 0);
        string coverPath = Resolve(Get("Appearance", "Cover"));
        if (File.Exists(coverPath))
        {
            try
            {
                var picture = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, Image = Image.FromFile(coverPath) };
                coverPanel.Controls.Add(picture);
            }
            catch { AddCoverPlaceholder(coverPanel, gameName); }
        }
        else AddCoverPlaceholder(coverPanel, gameName);

        var right = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(18, 8, 8, 8), ColumnCount = 1, RowCount = 2, BackColor = BackColor };
        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.Controls.Add(right, 1, 0);

        string heading = string.IsNullOrWhiteSpace(gameName) ? "HV Game Launcher" : gameName + " Launcher";
        var header = new Panel { Dock = DockStyle.Fill, BackColor = BackColor };
        right.Controls.Add(header, 0, 0);

        var headingLabel = new Label {
            Text = heading,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(37, 37, 37),
            TextAlign = ContentAlignment.MiddleLeft
        };
        header.Controls.Add(headingLabel);

        bool savedDarkMode = LoadDarkModePreference();
        var darkModeToggle = new CheckBox {
            Text = "Dark mode", AutoSize = true,
            Checked = savedDarkMode,
            Font = new Font("Segoe UI", 9),
            BackColor = BackColor, ForeColor = Color.FromArgb(45, 45, 45),
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        header.Controls.Add(darkModeToggle);

        void LayoutHeader()
        {
            darkModeToggle.Location = new Point(
                Math.Max(0, header.ClientSize.Width - darkModeToggle.Width), 
                Math.Max(0, (header.ClientSize.Height - darkModeToggle.Height) / 2));
            headingLabel.Location = new Point(0, 0);
            headingLabel.Size = new Size(
                Math.Max(0, darkModeToggle.Left - 16),
                header.ClientSize.Height);
            darkModeToggle.BringToFront();
        }
        header.Resize += (_, _) => LayoutHeader();
        LayoutHeader();
        darkModeToggle.CheckedChanged += (_, _) =>
        {
            ApplyTheme(this, darkModeToggle.Checked);
            SaveDarkModePreference(darkModeToggle.Checked);
        };

        // Keep the buttons as a compact group centered in the available area.
        var buttonArea = new Panel { Dock = DockStyle.Fill, BackColor = BackColor };
        right.Controls.Add(buttonArea, 0, 1);

        // Small, clickable developer/community links at the bottom corners.
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 24, BackColor = BackColor };
        buttonArea.Controls.Add(footer);

        var githubLink = new LinkLabel
        {
            Text = "GitHub: @Aydin-2002",
            AutoSize = true,
            Location = new Point(0, 4),
            Font = new Font("Segoe UI", 8.5f),
            LinkColor = Color.FromArgb(34, 139, 34),
            ActiveLinkColor = Color.FromArgb(0, 100, 0),
            VisitedLinkColor = Color.FromArgb(34, 139, 34),
            Cursor = Cursors.Hand,
            LinkBehavior = LinkBehavior.NeverUnderline
        };
        githubLink.LinkClicked += (_, _) => OpenWebLink("https://github.com/Aydin-2002");
        footer.Controls.Add(githubLink);

        var telegramLink = new LinkLabel
        {
            Text = "Telegram: @pcgaminghub",
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Font = new Font("Segoe UI", 8.5f),
            LinkColor = Color.FromArgb(34, 139, 34),
            ActiveLinkColor = Color.FromArgb(0, 100, 0),
            VisitedLinkColor = Color.FromArgb(34, 139, 34),
            Cursor = Cursors.Hand,
            LinkBehavior = LinkBehavior.NeverUnderline
        };
        telegramLink.LinkClicked += (_, _) => OpenWebLink("https://t.me/pcgaminghub");
        footer.Controls.Add(telegramLink);
        void PositionTelegramLink()
        {
            telegramLink.Location = new Point(Math.Max(0, footer.ClientSize.Width - telegramLink.Width), 4);
        }
        footer.Resize += (_, _) => PositionTelegramLink();
        PositionTelegramLink();

        var buttonGroup = new TableLayoutPanel {
            Size = new Size(440, 264), ColumnCount = 1, RowCount = 3,
            BackColor = BackColor, Margin = Padding.Empty, Padding = Padding.Empty
        };
        buttonGroup.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        buttonGroup.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
        buttonGroup.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
        buttonGroup.RowStyles.Add(new RowStyle(SizeType.Absolute, 94));
        buttonArea.Controls.Add(buttonGroup);

        void CenterButtonGroup()
        {
            buttonGroup.Left = Math.Max(0, (buttonArea.ClientSize.Width - buttonGroup.Width) / 2);
            int availableHeight = Math.Max(0, buttonArea.ClientSize.Height - footer.Height);
            buttonGroup.Top = Math.Max(0, (availableHeight - buttonGroup.Height) / 2);
        }
        buttonArea.Resize += (_, _) => CenterButtonGroup();
        CenterButtonGroup();

        AddButton(buttonGroup, "Run VBS", RunScript, 0);
        AddButton(buttonGroup, "Play Game", LaunchGame, 1);
        AddButton(buttonGroup, "Close Launcher", Close, 2);
        ApplyTheme(this, savedDarkMode);
    }

    private string GetPreferencePath()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string identity = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(root))).Substring(0, 16);
        string directory = Path.Combine(appData, "HVGameLauncher");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"{identity}.settings");
    }

    private bool LoadDarkModePreference()
    {
        try
        {
            string path = GetPreferencePath();
            return File.Exists(path) && File.ReadAllText(path, Encoding.UTF8).Trim().Equals("DarkMode=true", StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    private void SaveDarkModePreference(bool dark)
    {
        try
        {
            File.WriteAllText(GetPreferencePath(), dark ? "DarkMode=true" : "DarkMode=false", Encoding.UTF8);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Could not save the theme preference:\r\n{ex.Message}", "Settings save failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static void ApplyTheme(Control parent, bool dark)
    {
        Color background = dark ? Color.FromArgb(30, 30, 30) : Color.FromArgb(238, 238, 238);
        Color foreground = dark ? Color.FromArgb(235, 235, 235) : Color.FromArgb(37, 37, 37);
        Color buttonBackground = dark ? Color.FromArgb(55, 55, 55) : Color.FromArgb(248, 248, 248);
        Color buttonBorder = dark ? Color.FromArgb(85, 85, 85) : Color.FromArgb(190, 190, 190);
        Color linkColor = dark ? Color.FromArgb(100, 181, 246) : Color.FromArgb(34, 139, 34);

        parent.BackColor = background;
        parent.ForeColor = foreground;
        foreach (Control control in parent.Controls)
        {
            if (control is Button button)
            {
                button.BackColor = buttonBackground;
                button.ForeColor = foreground;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = buttonBorder;
            }
            else if (control is LinkLabel link)
            {
                link.LinkColor = linkColor;
                link.ActiveLinkColor = dark ? Color.FromArgb(144, 202, 249) : Color.FromArgb(0, 100, 0);
                link.VisitedLinkColor = linkColor;
                link.LinkBehavior = LinkBehavior.NeverUnderline;
                link.BackColor = background;
                link.ForeColor = foreground;
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.BackColor = background;
                checkBox.ForeColor = foreground;
            }
            else if (control is Label label)
            {
                label.BackColor = background;
                label.ForeColor = foreground;
            }
            else if (control is Panel || control is TableLayoutPanel)
            {
                control.BackColor = background;
                control.ForeColor = foreground;
            }

            if (control.HasChildren && control is not PictureBox)
                ApplyTheme(control, dark);
        }
    }

    private static void OpenWebLink(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open the link:\r\n{ex.Message}", "Open link failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void AddCoverPlaceholder(Panel panel, string gameName)
    {
        panel.Controls.Add(new Label {
            Dock = DockStyle.Fill, ForeColor = Color.Gainsboro, BackColor = panel.BackColor,
            Font = new Font("Segoe UI", 20, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter,
            Text = (string.IsNullOrWhiteSpace(gameName) ? "GAME" : gameName) + "\r\n\r\nCover image"
        });
    }

    private void AddButton(TableLayoutPanel parent, string text, Action action, int row)
    {
        var button = new Button {
            Text = text, Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 5),
            Font = new Font("Segoe UI", 14), BackColor = Color.FromArgb(248, 248, 248),
            ForeColor = Color.FromArgb(34, 34, 34), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(190, 190, 190);
        button.Click += (_, _) => action();
        parent.Controls.Add(button, 0, row);
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
        finally
        {
            DestroyIcon(handle);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

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
            else
            {
                MessageBox.Show(this, "Supported script types: .bat, .cmd, .vbs", "Unsupported file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Script launch failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
}
