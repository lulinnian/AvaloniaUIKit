namespace AvaloniaUIKit.Controls.Typewriter;

/// <summary>
/// UIKitTypewriter 控件的显示状态
/// </summary>
public enum TypewriterState
{
    /// <summary>空闲 — 控件初始态，无动画</summary>
    Idle,

    /// <summary>思考中 — 显示三点跳动动画，适用于 AI 正在处理请求时</summary>
    Thinking,

    /// <summary>流式输出中 — 显示光标闪烁，文字逐字打出</summary>
    Streaming,

    /// <summary>输出完毕 — 光标消失，文字完全呈现</summary>
    Done
}
