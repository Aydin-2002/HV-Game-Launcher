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

    public ImagePanel()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw, true);
        UpdateStyles();
    }

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



internal sealed class RailIconButton : Control
{
    private readonly int iconIndex;
    private readonly Func<float> themeBlend;
    private bool hovered;

    public RailIconButton(int index, Func<float> blend)
    {
        iconIndex = index;
        themeBlend = blend;
        Size = new Size(46, 46);
        Cursor = Cursors.Hand;
        TabStop = false;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        hovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        hovered = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    private static Color BlendColor(Color from, Color to, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return Color.FromArgb(
            (int)(from.A + (to.A - from.A) * amount),
            (int)(from.R + (to.R - from.R) * amount),
            (int)(from.G + (to.G - from.G) * amount),
            (int)(from.B + (to.B - from.B) * amount));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

        float blend = Math.Clamp(themeBlend(), 0f, 1f);
        bool dark = blend >= 0.5f;
        Color fill = BlendColor(Color.White, Color.Black, blend);
        Color ink = BlendColor(Color.Black, Color.White, blend);
        if (hovered)
            fill = BlendColor(Color.FromArgb(238, 238, 238), Color.FromArgb(28, 28, 28), blend);

        using (var background = new SolidBrush(fill))
            g.FillEllipse(background, 1.4f, 1.4f, 43.2f, 43.2f);

        // Light mode has a black outline. Dark mode stays a clean solid circle
        // without the unwanted white inset border.
        if (!dark)
        {
            using var border = new Pen(ink, 2.0f);
            g.DrawEllipse(border, 1.6f, 1.6f, 42.8f, 42.8f);
        }

        using var iconPen = new Pen(ink, 2.55f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round,
            LineJoin = System.Drawing.Drawing2D.LineJoin.Round
        };
        using var detailPen = new Pen(ink, 2.1f)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round,
            LineJoin = System.Drawing.Drawing2D.LineJoin.Round
        };
        using var iconBrush = new SolidBrush(ink);

        switch (iconIndex)
        {
            case 0: // Customize: settings gear.
                using (var gear = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    // Eight-tooth gear silhouette centered in the circular button.
                    const int teeth = 8;
                    const float cx = 23f, cy = 23f;
                    const float innerR = 10.2f, outerR = 15.0f;
                    var points = new List<PointF>();
                    for (int i = 0; i < teeth * 4; i++)
                    {
                        double angle = -Math.PI / 2 + i * (Math.PI * 2 / (teeth * 4));
                        float radius = (i % 4 == 1 || i % 4 == 2) ? outerR : innerR;
                        points.Add(new PointF(
                            cx + (float)Math.Cos(angle) * radius,
                            cy + (float)Math.Sin(angle) * radius));
                    }
                    gear.AddPolygon(points.ToArray());
                    g.FillPath(iconBrush, gear);
                }
                // Hollow center, matching the button's current foreground/background colors.
                using (var center = new SolidBrush(fill))
                    g.FillEllipse(center, 18.2f, 18.2f, 9.6f, 9.6f);
                break;

            case 1: // Theme: sun with crescent for dark mode; hollow sun for light mode.
                for (int i = 0; i < 8; i++)
                {
                    double angle = i * Math.PI / 4.0;
                    float cx = 23f, cy = 23f;
                    float r1 = 11.0f, r2 = 15.0f;
                    g.DrawLine(iconPen,
                        cx + (float)Math.Cos(angle) * r1,
                        cy + (float)Math.Sin(angle) * r1,
                        cx + (float)Math.Cos(angle) * r2,
                        cy + (float)Math.Sin(angle) * r2);
                }

                if (dark)
                {
                    g.FillEllipse(iconBrush, 16.0f, 16.0f, 14.0f, 14.0f);
                    using var cutout = new SolidBrush(fill);
                    g.FillEllipse(cutout, 20.5f, 13.8f, 14.0f, 14.0f);
                }
                else
                {
                    g.DrawEllipse(iconPen, 16.0f, 16.0f, 14.0f, 14.0f);
                }
                break;

            case 2: // Update: retain the existing download symbol.
                g.DrawLine(iconPen, 23f, 10.5f, 23f, 27.0f);
                g.DrawLine(iconPen, 16.2f, 20.5f, 23f, 27.4f);
                g.DrawLine(iconPen, 29.8f, 20.5f, 23f, 27.4f);
                g.DrawLine(iconPen, 13.0f, 34.0f, 33.0f, 34.0f);
                break;

            case 3: // Telegram: centered paper plane, no extra inner circle.
                using (var plane = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    plane.AddPolygon(new[]
                    {
                        new PointF(9.0f, 21.0f), new PointF(35.5f, 10.0f),
                        new PointF(29.6f, 35.0f), new PointF(22.4f, 28.0f),
                        new PointF(17.8f, 31.8f), new PointF(18.4f, 24.8f)
                    });
                    g.FillPath(iconBrush, plane);
                    using var foldPen = new Pen(fill, 1.45f)
                    {
                        StartCap = System.Drawing.Drawing2D.LineCap.Round,
                        EndCap = System.Drawing.Drawing2D.LineCap.Round
                    };
                    g.DrawLine(foldPen, 18.4f, 24.8f, 35.5f, 10.0f);
                    g.DrawLine(foldPen, 22.4f, 28.0f, 27.2f, 22.8f);
                }
                break;

            default: // About Me: centered user silhouette.
                g.FillEllipse(iconBrush, 18.5f, 10.0f, 9.0f, 9.0f);
                using (var shoulders = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    shoulders.AddArc(14.0f, 20.0f, 18.0f, 18.0f, 180.0f, 180.0f);
                    shoulders.AddLine(32.0f, 29.0f, 32.0f, 34.0f);
                    shoulders.AddArc(14.0f, 29.0f, 18.0f, 9.0f, 0.0f, 180.0f);
                    shoulders.CloseFigure();
                    g.FillPath(iconBrush, shoulders);
                }
                break;
        }
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
    private PictureBox logoBox = null!;
    private ImagePanel coverPanel = null!;
    private Button launchButton = null!;
    private bool dark;
    private float themeBlend;
    private readonly System.Windows.Forms.Timer themeTimer = new() { Interval = 15 };
    private DateTime themeAnimationStarted;
    private float themeAnimationFrom;
    private float themeAnimationTo;
    private Color launchColor = Color.White;
    private Image? backgroundImage;
    private Image? coverImage;
    private Image? logoImage;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED: paint child controls as one buffered frame.
            return cp;
        }
    }

    public LauncherForm()
    {
        paletteColors = palette.Select(ColorTranslator.FromHtml).ToArray();
        LoadConfig();
        LoadLocalSettings();
        themeBlend = dark ? 1f : 0f;
        themeTimer.Tick += (_, _) => AdvanceThemeAnimation();

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
        ApplyTheme(themeBlend);
    }

    private void BuildInterface()
    {
        mainPanel = new ImagePanel { Dock = DockStyle.Fill, Padding = new Padding(14) };
        Controls.Add(mainPanel);

        rail = new Panel { Dock = DockStyle.Right, Width = 62, Padding = new Padding(5, 5, 0, 5) };
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

        // Optional game logo. PictureBox preserves PNG alpha transparency.
        logoBox = new PictureBox
        {
            Dock = DockStyle.Top, Height = 110,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Visible = false
        };
        coverHost.Controls.Add(logoBox);
        LoadLogo();

        coverPanel = new ImagePanel { BackColor = Color.Transparent, FitContain = true };
        coverHost.Controls.Add(coverPanel);
        coverHost.Resize += (_, _) => PositionCover();
        LoadCover();

        contentPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28, 18, 24, 18) };
        outer.Controls.Add(contentPanel, 1, 0);

        launchButton = new Button
        {
            Text = "LAUNCH", Size = new Size(170, 45),
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        launchButton.FlatAppearance.BorderSize = 0;
        launchButton.Click += (_, _) => ShowLaunchMenu();
        mainPanel.Controls.Add(launchButton);

        void PositionLaunchButton()
        {
            launchButton.Location = new Point(
                Math.Max(0, mainPanel.ClientSize.Width - launchButton.Width - 10),
                Math.Max(0, mainPanel.ClientSize.Height - launchButton.Height - 17));
        }
        mainPanel.Resize += (_, _) => PositionLaunchButton();
        Shown += (_, _) => PositionLaunchButton();

        AddRailButton("⚙", "Customize", Customize, 0);
        AddRailButton("◐", "Dark / Light Mode", ToggleTheme, 1);
        AddRailButton("↻", "Update Launcher", CheckForUpdates, 2);
        AddRailButton("✈", "Telegram", () => OpenLink("https://t.me/pcgaminghub"), 3);
        AddRailButton("ⓘ", "About Me", () => OpenLink("https://github.com/Aydin-2002"), 4);

        if (!string.IsNullOrWhiteSpace(Get("Appearance", "Background")))
            LoadBackground();

        // Keep the Launch button visible above the background and content.
        launchButton.BringToFront();
        PositionLaunchButton();
    }

    private void AddRailButton(string glyph, string tip, Action action, int index)
    {
        var button = new RailIconButton(index, () => themeBlend)
        {
            Location = new Point(11, 8 + index * 58)
        };
        button.Click += (_, _) => action();
        var toolTip = new ToolTip();
        toolTip.SetToolTip(button, tip);
        rail.Controls.Add(button);
    }

    private void LoadLogo()
    {
        string path = Resolve(Get("Appearance", "Logo"));
        if (!File.Exists(path)) return;
        try
        {
            using var source = Image.FromFile(path);
            logoImage = new Bitmap(source);
            logoBox.Image = logoImage;
            logoBox.Visible = true;
        }
        catch
        {
            logoBox.Visible = false;
        }
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
        int top;
        if (logoImage is not null && logoBox.Visible)
            top = logoBox.Bottom + 6;
        else
            top = (coverHost.ClientSize.Height - height) / 2;
        top = Math.Max(0, Math.Min(top, Math.Max(0, coverHost.ClientSize.Height - height)));
        coverPanel.Location = new Point((coverHost.ClientSize.Width - width) / 2, top);
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
                ApplyTheme(themeBlend);
                picker.Close();
            };
            grid.Controls.Add(swatch);
        }
        picker.ShowDialog(this);
    }

    private void ToggleTheme()
    {
        themeAnimationFrom = themeBlend;
        dark = !dark;
        themeAnimationTo = dark ? 1f : 0f;
        themeAnimationStarted = DateTime.UtcNow;
        SaveLocalSettings();

        if (themeTimer.Enabled)
            themeTimer.Stop();

        themeTimer.Start();
    }

    private void AdvanceThemeAnimation()
    {
        const double durationMilliseconds = 360.0;
        double elapsed = (DateTime.UtcNow - themeAnimationStarted).TotalMilliseconds;
        float t = (float)Math.Clamp(elapsed / durationMilliseconds, 0.0, 1.0);

        // Smoothstep easing: gradual start and finish, with no abrupt color jump.
        float eased = t * t * (3f - 2f * t);
        themeBlend = themeAnimationFrom + (themeAnimationTo - themeAnimationFrom) * eased;
        ApplyTheme(themeBlend);

        if (t >= 1f)
        {
            themeBlend = themeAnimationTo;
            themeTimer.Stop();
            ApplyTheme(themeBlend);
        }
    }

    private static Color BlendThemeColor(Color light, Color darkColor, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return Color.FromArgb(
            (int)(light.A + (darkColor.A - light.A) * amount),
            (int)(light.R + (darkColor.R - light.R) * amount),
            (int)(light.G + (darkColor.G - light.G) * amount),
            (int)(light.B + (darkColor.B - light.B) * amount));
    }

    private void ApplyTheme(float blend)
    {
        Color lightBackground = Color.FromArgb(242, 244, 248);
        Color darkBackground = Color.FromArgb(28, 30, 36);
        Color background = BlendThemeColor(lightBackground, darkBackground, blend);

        // The background image does not change with the theme. Avoid touching
        // its parent panel during every animation frame; doing so forces a costly
        // recursive repaint of the full image and all transparent child controls.
        if (backgroundImage is null)
        {
            mainPanel.BackColor = background;
            contentPanel.BackColor = background;
            rail.BackColor = background;
        }

        // Only repaint the five animated icons. WS_EX_COMPOSITED remains enabled
        // to preserve the flicker-free background behavior from Stage 14.
        foreach (Control control in rail.Controls)
            control.Invalidate();

        launchButton.BackColor = launchColor;
        launchButton.ForeColor = launchColor.GetBrightness() > 0.78f
            ? Color.FromArgb(30, 34, 42) : Color.White;
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
            MinimizeBox = false, ShowInTaskbar = false, ClientSize = new Size(390, 178)
        };
        const string currentVersion = "1.4.0";
        var current = new Label
        {
            Dock = DockStyle.Top, Height = 42, TextAlign = ContentAlignment.MiddleCenter,
            Text = $"Current version: {currentVersion}"
        };
        var status = new Label
        {
            Dock = DockStyle.Top, Height = 82, TextAlign = ContentAlignment.MiddleCenter,
            Text = "Checking GitHub Releases..."
        };
        var open = new Button { Text = "Open Release Page", Dock = DockStyle.Bottom, Height = 38, Enabled = false };
        open.Click += (_, _) => OpenLink("https://github.com/Aydin-2002/HV-Game-Launcher/releases/latest");
        dialog.Controls.Add(open);
        dialog.Controls.Add(status);
        dialog.Controls.Add(current);
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
            logoImage?.Dispose();
            themeTimer.Stop();
            themeTimer.Dispose();
        }
        base.Dispose(disposing);
    }
}
