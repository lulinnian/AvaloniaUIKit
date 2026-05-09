using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaUIKit.Controls.Badge;

/// <summary>
/// UIKitBadge — 状态徽标控件
/// <para>
/// 常用于展示状态标签、计数气泡等场景。
/// </para>
/// <para>
/// Classes:
/// <list type="bullet">
///   <item><description>(无) — Primary 蓝色背景</description></item>
///   <item><description>secondary — 描边风格</description></item>
///   <item><description>success   — 绿色</description></item>
///   <item><description>warning   — 橙色</description></item>
///   <item><description>danger    — 红色</description></item>
///   <item><description>info      — 青色</description></item>
///   <item><description>pill      — 胶囊圆角</description></item>
/// </list>
/// </para>
/// </summary>
/// <example>
/// <code>
/// &lt;kit:UIKitBadge Text="成功" Classes="success" /&gt;
/// &lt;kit:UIKitBadge Text="99+" Classes="danger pill" /&gt;
/// </code>
/// </example>
public class UIKitBadge : TemplatedControl
{
    /// <summary>徽标显示文字</summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<UIKitBadge, string?>(nameof(Text));

    /// <summary>徽标显示文字</summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    static UIKitBadge()
    {
        AffectsRender<UIKitBadge>(TextProperty);
    }
}
