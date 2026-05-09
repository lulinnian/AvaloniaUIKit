using Avalonia;
using Avalonia.Controls;

namespace AvaloniaUIKit.Extensions;

/// <summary>
/// AvaloniaUIKit 初始化扩展方法。
/// <para>
/// 在 AppBuilder 链中调用 <see cref="UseAvaloniaUIKit"/> 可注册组件库所需的
/// 依赖（将来可在此处添加自定义字体、图标主题等）。
/// </para>
/// </summary>
/// <example>
/// <code>
/// AppBuilder.Configure&lt;App&gt;()
///     .UsePlatformDetect()
///     .UseAvaloniaUIKit()   // ← 注册 UIKit
///     .StartWithClassicDesktopLifetime(args);
/// </code>
/// </example>
public static class UIKitAppBuilderExtensions
{
    /// <summary>
    /// 注册 AvaloniaUIKit 到 AppBuilder。
    /// </summary>
    public static AppBuilder UseAvaloniaUIKit(this AppBuilder builder)
    {
        // 目前仅作占位；后续可在此注册自定义字体、全局资源等
        return builder;
    }
}
