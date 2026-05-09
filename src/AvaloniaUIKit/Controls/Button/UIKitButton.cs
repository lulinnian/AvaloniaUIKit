using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaUIKit.Controls.Button;

/// <summary>
/// UIKitButton — AvaloniaUIKit 按钮控件
/// <para>
/// 通过 Classes 切换变体：
/// <list type="bullet">
///   <item><description>(无 class) — Primary 填充按钮（默认）</description></item>
///   <item><description>secondary  — 描边按钮</description></item>
///   <item><description>danger     — 危险操作红色按钮</description></item>
///   <item><description>ghost      — 透明背景、灰色边框</description></item>
///   <item><description>link       — 纯文字链接样式</description></item>
///   <item><description>small      — 小号尺寸修饰</description></item>
///   <item><description>large      — 大号尺寸修饰</description></item>
/// </list>
/// </para>
/// </summary>
/// <example>
/// <code>
/// &lt;kit:UIKitButton&gt;确认&lt;/kit:UIKitButton&gt;
/// &lt;kit:UIKitButton Classes="secondary"&gt;取消&lt;/kit:UIKitButton&gt;
/// &lt;kit:UIKitButton Classes="danger large"&gt;删除&lt;/kit:UIKitButton&gt;
/// </code>
/// </example>
public class UIKitButton : Avalonia.Controls.Button
{
    static UIKitButton()
    {
        // 使样式系统自动应用 UIKitButtonStyles.axaml 里 Selector 对应的样式
        AffectsRender<UIKitButton>();
    }
}
