using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaUIKit.Extensions;

namespace AvaloniaUIKit.Demo;

public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
