using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaUIKit.Controls.ChatBubble;

/// <summary>
/// UIKitChatBubble — 聊天气泡控件
/// <para>
/// 专为 AI Chat 场景设计，支持用户/AI 助手/系统三种角色，
/// 自动切换对齐方向、气泡颜色和布局。
/// </para>
/// <para>
/// 角色（通过 <see cref="Role"/> 属性控制）：
/// <list type="bullet">
///   <item><description>User      — 右对齐，蓝色背景，白色文字</description></item>
///   <item><description>Assistant — 左对齐，灰色背景，支持头像</description></item>
///   <item><description>System    — 居中，小字灰色</description></item>
/// </list>
/// </para>
/// <para>
/// 可通过 <see cref="Content"/> 属性设置任意子内容，
/// 包括包裹 <c>UIKitTypewriter</c> 实现流式回答效果。
/// </para>
/// </summary>
/// <example>
/// <code>
/// &lt;!-- 用户消息 --&gt;
/// &lt;kitBubble:UIKitChatBubble Role="User" Message="你好！" Timestamp="14:30" /&gt;
///
/// &lt;!-- AI 消息（含头像） --&gt;
/// &lt;kitBubble:UIKitChatBubble Role="Assistant" Timestamp="14:30"&gt;
///   &lt;kitBubble:UIKitChatBubble.Avatar&gt;
///     &lt;Border Width="32" Height="32" CornerRadius="16" Background="Gold" /&gt;
///   &lt;/kitBubble:UIKitChatBubble.Avatar&gt;
///   &lt;kitBubble:UIKitChatBubble&gt;
///     你好！有什么可以帮你的？
///   &lt;/kitBubble:UIKitChatBubble&gt;
/// &lt;/kitBubble:UIKitChatBubble&gt;
///
/// &lt;!-- 系统消息 --&gt;
/// &lt;kitBubble:UIKitChatBubble Role="System" Message="已切换到 GPT-4 模型" /&gt;
/// </code>
/// </example>
public class UIKitChatBubble : ContentControl
{
    // ─── Styled Properties ─────────────────────────────────────────────────

    /// <summary>气泡角色（User / Assistant / System）</summary>
    public static readonly StyledProperty<BubbleRole> RoleProperty =
        AvaloniaProperty.Register<UIKitChatBubble, BubbleRole>(nameof(Role), defaultValue: BubbleRole.User);

    /// <summary>头像内容（Image / PathIcon / Border 等），仅 Assistant 角色显示</summary>
    public static readonly StyledProperty<object?> AvatarProperty =
        AvaloniaProperty.Register<UIKitChatBubble, object?>(nameof(Avatar));

    /// <summary>消息文本（简化用法，设置后内部自动创建 TextBlock）</summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<UIKitChatBubble, string?>(nameof(Message));

    /// <summary>时间戳文本（显示在气泡下方）</summary>
    public static readonly StyledProperty<string?> TimestampProperty =
        AvaloniaProperty.Register<UIKitChatBubble, string?>(nameof(Timestamp));

    // ─── Public Properties ─────────────────────────────────────────────────

    /// <summary>气泡角色（User / Assistant / System）</summary>
    public BubbleRole Role
    {
        get => GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    /// <summary>头像内容，仅 Assistant 角色显示</summary>
    public object? Avatar
    {
        get => GetValue(AvatarProperty);
        set => SetValue(AvatarProperty, value);
    }

    /// <summary>消息文本（简化用法）</summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>时间戳文本</summary>
    public string? Timestamp
    {
        get => GetValue(TimestampProperty);
        set => SetValue(TimestampProperty, value);
    }

    // ─── Internal Fields ────────────────────────────────────────────────────
    private Border? _avatarBorder;
    private TextBlock? _timestampBlock;
    private TextBlock? _messageBlock;

    // ─── Static Constructor ────────────────────────────────────────────────
    static UIKitChatBubble()
    {
        AffectsRender<UIKitChatBubble>(RoleProperty, AvatarProperty, MessageProperty, TimestampProperty);
        RoleProperty.Changed.AddClassHandler<UIKitChatBubble>(OnRolePropertyChanged);
        AvatarProperty.Changed.AddClassHandler<UIKitChatBubble>(OnAvatarPropertyChanged);
        TimestampProperty.Changed.AddClassHandler<UIKitChatBubble>(OnTimestampPropertyChanged);
        MessageProperty.Changed.AddClassHandler<UIKitChatBubble>(OnMessagePropertyChanged);
    }

    // ─── Template ──────────────────────────────────────────────────────────

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _avatarBorder    = e.NameScope.Find<Border>("PART_Avatar");
        _timestampBlock  = e.NameScope.Find<TextBlock>("PART_Timestamp");
        _messageBlock    = e.NameScope.Find<TextBlock>("PART_Message");

        UpdatePseudoClasses(Role);
        UpdateAvatarVisibility();
        UpdateTimestampVisibility();
        UpdateMessageVisibility();
    }

    // ─── Property Change Handlers ───────────────────────────────────────────

    private static void OnRolePropertyChanged(UIKitChatBubble sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdatePseudoClasses((BubbleRole)e.NewValue!);
    }

    private static void OnAvatarPropertyChanged(UIKitChatBubble sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdateAvatarVisibility();
    }

    private static void OnTimestampPropertyChanged(UIKitChatBubble sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdateTimestampVisibility();
    }

    private static void OnMessagePropertyChanged(UIKitChatBubble sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdateMessageVisibility();
    }

    private void UpdatePseudoClasses(BubbleRole role)
    {
        PseudoClasses.Set(":user",      role == BubbleRole.User);
        PseudoClasses.Set(":assistant", role == BubbleRole.Assistant);
        PseudoClasses.Set(":system",    role == BubbleRole.System);
    }

    private void UpdateAvatarVisibility()
    {
        if (_avatarBorder == null) return;
        _avatarBorder.IsVisible = Avatar != null && Role == BubbleRole.Assistant;
    }

    private void UpdateTimestampVisibility()
    {
        if (_timestampBlock == null) return;
        _timestampBlock.IsVisible = !string.IsNullOrEmpty(Timestamp);
    }

    private void UpdateMessageVisibility()
    {
        if (_messageBlock == null) return;
        _messageBlock.IsVisible = !string.IsNullOrEmpty(Message);
    }
}
