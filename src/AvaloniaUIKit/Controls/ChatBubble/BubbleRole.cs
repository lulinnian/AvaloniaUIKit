namespace AvaloniaUIKit.Controls.ChatBubble;

/// <summary>
/// 聊天气泡角色类型
/// </summary>
public enum BubbleRole
{
    /// <summary>用户消息 — 右对齐，蓝色气泡</summary>
    User,

    /// <summary>AI 助手消息 — 左对齐，灰色气泡，可选头像</summary>
    Assistant,

    /// <summary>系统消息 — 居中，小字灰色，无边框气泡</summary>
    System
}
