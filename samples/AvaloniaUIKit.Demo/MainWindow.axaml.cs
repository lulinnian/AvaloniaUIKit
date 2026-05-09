using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaUIKit;
using AvaloniaUIKit.Controls.Button;

namespace AvaloniaUIKit.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeThemeButtons();
    }

    private void InitializeThemeButtons()
    {
        BtnLightTheme.Click += BtnLightTheme_Click;
        BtnDarkTheme.Click += BtnDarkTheme_Click;
    }

    private void BtnLightTheme_Click(object? sender, RoutedEventArgs e)
    {
        UIKitThemeService.SetLightTheme();
    }

    private void BtnDarkTheme_Click(object? sender, RoutedEventArgs e)
    {
        UIKitThemeService.SetDarkTheme();
    }
}
