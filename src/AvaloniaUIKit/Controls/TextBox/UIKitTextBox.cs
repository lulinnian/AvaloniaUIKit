using Avalonia;
using Avalonia.Controls;

namespace AvaloniaUIKit.Controls.TextBox;

/// <summary>
/// UIKitTextBox — 文本输入框控件
/// <para>
/// 在 Avalonia 标准 TextBox 基础上，增加前缀/后缀内容插槽，
/// 并自动绑定 UIKit Token 主题色（悬停 / 聚焦边框颜色）。
/// </para>
/// </summary>
/// <example>
/// <code>
/// &lt;!-- 普通输入框 --&gt;
/// &lt;kit:UIKitTextBox Watermark="请输入用户名" /&gt;
///
/// &lt;!-- 带前缀图标 --&gt;
/// &lt;kit:UIKitTextBox Watermark="搜索"&gt;
///   &lt;kit:UIKitTextBox.PrefixContent&gt;
///     &lt;PathIcon Data="{StaticResource SearchIcon}" /&gt;
///   &lt;/kit:UIKitTextBox.PrefixContent&gt;
/// &lt;/kit:UIKitTextBox&gt;
/// </code>
/// </example>
public class UIKitTextBox : Avalonia.Controls.TextBox
{
    /// <summary>输入框前缀内容（图标等）</summary>
    public static readonly StyledProperty<object?> PrefixContentProperty =
        AvaloniaProperty.Register<UIKitTextBox, object?>(nameof(PrefixContent));

    /// <summary>输入框后缀内容（清除按钮 / 单位等）</summary>
    public static readonly StyledProperty<object?> SuffixContentProperty =
        AvaloniaProperty.Register<UIKitTextBox, object?>(nameof(SuffixContent));

    /// <summary>输入框前缀内容</summary>
    public object? PrefixContent
    {
        get => GetValue(PrefixContentProperty);
        set => SetValue(PrefixContentProperty, value);
    }

    /// <summary>输入框后缀内容</summary>
    public object? SuffixContent
    {
        get => GetValue(SuffixContentProperty);
        set => SetValue(SuffixContentProperty, value);
    }

    static UIKitTextBox()
    {
        AffectsRender<UIKitTextBox>(PrefixContentProperty, SuffixContentProperty);
    }
}
