using System;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using GenericGameLauncher.ViewModels;

namespace GenericGameLauncher.Views;

public partial class MainWindow : Window
{
    private readonly List<(Vector2 Position, Vector2 Velocity, int Life)> sparks = new();
    private readonly Random random = new();
    private readonly System.Timers.Timer sparkTimer;
    private Point pointerPosition;
    private Canvas? sparkCanvas;

    public MainWindow()
    {
        InitializeComponent();
        sparkTimer = new System.Timers.Timer(25) { AutoReset = true };
        sparkTimer.Elapsed += OnSparkElapsed;
        SetRoundedClip(this.FindControl<Border>("MainSurface"), 16);
        SetRoundedClip(this.FindControl<Image>("BackgroundImageView"), 16);
        SetRoundedClip(this.FindControl<Border>("CoverBorder"), 10);
        DataContextChanged += (_, _) =>
        {
            if (DataContext is not MainWindowViewModel vm) return;
            if (!string.IsNullOrWhiteSpace(vm.IconPath) && System.IO.File.Exists(vm.IconPath))
            {
                try { Icon = new WindowIcon(vm.IconPath); } catch { }
            }
            var coverBorder = this.FindControl<Border>("CoverBorder");
            if (coverBorder is not null && vm.CoverImage is null)
            {
                var tb = new TextBlock
                {
                    Text = "NO COVER",
                    Foreground = Brushes.Gainsboro,
                    FontSize = 20,
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                };
                coverBorder.Child = tb;
            }
            this.FindControl<TextBlock>("GithubLink")?.AddHandler(PointerPressedEvent, (s, e) => vm.OpenLinkCommand.Execute("https://github.com/Aydin-2002"));
            this.FindControl<TextBlock>("TelegramLink")?.AddHandler(PointerPressedEvent, (s, e) => vm.OpenLinkCommand.Execute("https://t.me/pcgaminghub"));
        };
        sparkCanvas = this.FindControl<Canvas>("SparkCanvas");
    }

    private static void SetRoundedClip(Control? control, double radius)
    {
        if (control is null) return;

        void UpdateClip(object? sender, SizeChangedEventArgs e)
        {
            var bounds = control.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            control.Clip = new RectangleGeometry(new Rect(0, 0, bounds.Width, bounds.Height), radius, radius);
        }

        control.SizeChanged += UpdateClip;
        control.AttachedToVisualTree += (_, _) =>
        {
            var bounds = control.Bounds;
            if (bounds.Width > 0 && bounds.Height > 0)
                control.Clip = new RectangleGeometry(new Rect(0, 0, bounds.Width, bounds.Height), radius, radius);
        };
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        pointerPosition = e.GetPosition(this);
        bool shouldShowSparks = DataContext is MainWindowViewModel vm &&
                                vm.IsDarkMode && !vm.HasDarkBackgroundImage;
        if (shouldShowSparks)
        {
            EmitSparks(pointerPosition);
            if (!sparkTimer.Enabled) sparkTimer.Start();
        }
        else
        {
            sparkTimer.Stop();
            sparks.Clear();
            sparkCanvas?.Children.Clear();
        }
    }

    private void EmitSparks(Point p)
    {
        for (int i = 0; i < 2; i++)
        {
            double a = random.NextDouble() * Math.PI * 2;
            float speed = (float)(0.7 + random.NextDouble() * 2.2);
            sparks.Add((new Vector2((float)p.X, (float)p.Y),
                        new Vector2((float)Math.Cos(a) * speed, (float)Math.Sin(a) * speed),
                        18 + random.Next(12)));
        }
    }

    private void OnSparkElapsed(object? sender, ElapsedEventArgs e) => AdvanceSparks();

    private void AdvanceSparks()
    {
        for (int i = sparks.Count - 1; i >= 0; i--)
        {
            var s = sparks[i];
            var pos = s.Position + s.Velocity;
            var vel = s.Velocity * 0.94f;
            var life = s.Life - 1;
            if (life <= 1) sparks.RemoveAt(i);
            else sparks[i] = (pos, vel, life);
        }
        if (sparks.Count == 0) sparkTimer.Stop();

        Dispatcher.UIThread.Post(() => UpdateSparkVisuals());
    }

    private void UpdateSparkVisuals()
    {
        if (sparkCanvas is null) return;
        sparkCanvas.Children.Clear();
        if (Application.Current?.RequestedThemeVariant != ThemeVariant.Dark) return;
        foreach (var s in sparks)
        {
            int alpha = Math.Clamp(s.Life * 15, 40, 220);
            var spark = new Border
            {
                Width = 3,
                Height = 3,
                CornerRadius = new CornerRadius(1.5),
                Background = new SolidColorBrush(Color.FromArgb((byte)alpha, 255, 255, 255)),
            };
            Canvas.SetLeft(spark, s.Position.X - 1.5);
            Canvas.SetTop(spark, s.Position.Y - 1.5);
            sparkCanvas.Children.Add(spark);
        }
    }
}
