using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaUIKit.Controls.Card;

/// <summary>
/// UIKitCard — 卡片容器控件
/// <para>
/// 提供统一的圆角边框卡片外观，支持可选的 Header 区域。
/// </para>
/// <para>
/// Classes:
/// <list type="bullet">
///   <item><description>hoverable — 鼠标悬停时显示主色调边框</description></item>
/// </list>
/// </para>
/// </summary>
/// <example>
/// <code>
/// &lt;kit:UIKitCard Header="标题"&gt;
///   &lt;TextBlock Text="卡片内容" /&gt;
/// &lt;/kit:UIKitCard&gt;
/// </code>
/// </example>
public class UIKitCard : ContentControl
{
    /// <summary>卡片头部标题内容（可选）</summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<UIKitCard, object?>(nameof(Header));

    /// <summary>卡片头部标题内容（可选）</summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    static UIKitCard()
    {
        AffectsRender<UIKitCard>(HeaderProperty);
    }
}
