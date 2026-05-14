namespace AvaloniaUIKit.Controls.ChatInput;

/// <summary>
/// UIKitChatInput 的输入状态枚举。
/// <para>通过 PseudoClasses 驱动样式切换。</para>
/// </summary>
public enum ChatInputState
{
    /// <summary>正常状态，输入框为空</summary>
    Normal,

    /// <summary>有文字输入中</summary>
    Typing,

    /// <summary>接近字数上限（超过 MaxLength 的 80%）</summary>
    NearLimit,

    /// <summary>已达到字数上限</summary>
    AtLimit,

    /// <summary>控件被禁用</summary>
    Disabled
}
