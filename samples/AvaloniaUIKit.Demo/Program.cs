using Avalonia;
using AvaloniaUIKit.Extensions;

namespace AvaloniaUIKit.Demo;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .UseAvaloniaUIKit()   // 注册 UIKit 扩展
                     .WithInterFont()
                     .LogToTrace();
}
