using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaUIKit.Controls.ChatBubble;
using AvaloniaUIKit.Controls.ChatInput;
using AvaloniaUIKit.Controls.Typewriter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaUIKit.Demo.Controls;

public partial class AIChatDemo : UserControl
{
    // ─── 模拟回复数据 ─────────────────────────────────────────────────────

    private static readonly Dictionary<string, string> SimulatedResponses = new()
    {
        ["你好"] = "你好！很高兴见到你 👋\n\n我是 UIKit AI 助手，基于 AvaloniaUIKit 控件库构建。我可以回答关于 Avalonia UI 开发、C# 编程、桌面应用设计等方面的问题。",
        ["你是谁"] = "我是 AvaloniaUIKit AI 助手，一个使用 AvaloniaUIKit 控件库构建的桌面 AI 聊天演示。\n\n我使用了以下 UIKit 控件：\n• UIKitChatBubble — 聊天气泡展示\n• UIKitTypewriter — AI 流式打字效果\n• UIKitChatInput — 消息输入框\n\n这一切都在 Avalonia 11 框架上运行！",
        ["推荐"] = "推荐你试试以下 Avalonia 开发资源：\n\n1. 官方文档 — docs.avaloniaui.com\n2. AvaloniaUI GitHub — 最新的示例和源码\n3. Avalonia 论坛 — 社区问答\n4. AvaloniaUIKit — 本项目控件库，提供开箱即用的现代化控件",
    };

    private const string DefaultAIResponse =
        "感谢你的消息！这是一个模拟回复。\n\n这是使用 Avalonia 框架实现的简易的 AI 聊天界面。\n\n当前演示使用了 UIKitTypewriter 控件的流式打字动画效果，模拟真实 AI 的输出体验。";

    // ─── 状态 ─────────────────────────────────────────────────────────────

    private bool _isAIResponding;

    // ─── 构造 ─────────────────────────────────────────────────────────────

    public AIChatDemo()
    {
        InitializeComponent();
        AIChatInput.Send += AIChatInput_Send;
        BtnClearChat.Click += BtnClearChat_Click;
    }

    // ─── 事件处理 ─────────────────────────────────────────────────────────

    /// <summary>用户发送消息 → 添加用户气泡 → 模拟 AI 回复</summary>
    private async void AIChatInput_Send(object? sender, ChatInputSendEventArgs e)
    {
        if (_isAIResponding) return;

        var userMessage = e.Message.Trim();
        if (string.IsNullOrEmpty(userMessage)) return;

        // 1. 添加用户消息气泡
        AddUserBubble(userMessage);

        // 2. 禁用输入，更新状态
        _isAIResponding = true;
        AIChatInput.IsEnabled = false;
        AIChatStatusText.Text = "正在回复...";

        // 3. 模拟 AI 思考 + 流式输出
        await SimulateAIResponse(userMessage);
    }

    /// <summary>清空聊天记录</summary>
    private void BtnClearChat_Click(object? sender, RoutedEventArgs e)
    {
        if (_isAIResponding) return;
        AIChatMessageList.Children.Clear();

        var welcome = new UIKitChatBubble
        {
            Role = BubbleRole.System,
            Message = "聊天记录已清空。输入消息开始新的对话。"
        };
        AIChatMessageList.Children.Add(welcome);
    }

    // ─── 私有方法 ─────────────────────────────────────────────────────────

    private void AddUserBubble(string message)
    {
        var now = DateTime.Now;
        var timeStr = $"{now.Hour:D2}:{now.Minute:D2}";

        var bubble = new UIKitChatBubble
        {
            Role = BubbleRole.User,
            Message = message,
            Timestamp = timeStr
        };
        AIChatMessageList.Children.Add(bubble);
        ScrollToBottom();
    }

    private async Task SimulateAIResponse(string userMessage)
    {
        var response = GetSimulatedResponse(userMessage);

        var typewriter = new UIKitTypewriter
        {
            FontSize = 14,
            CharIntervalMs = 25
        };

        var aiBubble = new UIKitChatBubble
        {
            Role = BubbleRole.Assistant,
            Timestamp = $"{DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}"
        };

        // 设置 AI 头像（可爱机器人）
        var avatar = new Border
        {
            Width = 36,
            Height = 36,
            CornerRadius = new CornerRadius(18),
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#6C5CE7")),
            Child = new TextBlock
            {
                Text = "\U0001F916",
                FontSize = 18,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };
        aiBubble.Avatar = avatar;
        aiBubble.Content = typewriter;

        AIChatMessageList.Children.Add(aiBubble);
        ScrollToBottom();

        // 阶段 1：思考中（1~2 秒）
        typewriter.State = TypewriterState.Thinking;
        await Task.Delay(new Random().Next(1000, 2000));

        // 阶段 2：流式输出
        typewriter.State = TypewriterState.Streaming;

        var random = new Random();
        var chunkSize = 2;
        for (var i = 0; i < response.Length; i += chunkSize)
        {
            var end = Math.Min(i + chunkSize, response.Length);
            var chunk = response[i..end];
            typewriter.AppendText(chunk);
            await Task.Delay(random.Next(20, 60));
            ScrollToBottom();
        }

        // 阶段 3：完成
        typewriter.State = TypewriterState.Done;
        ScrollToBottom();

        // 恢复输入
        _isAIResponding = false;
        AIChatInput.IsEnabled = true;
        AIChatStatusText.Text = "在线";
    }

    private static string GetSimulatedResponse(string userMessage)
    {
        var lower = userMessage.ToLowerInvariant();

        foreach (var (keyword, reply) in SimulatedResponses)
        {
            if (lower.Contains(keyword))
                return reply;
        }

        return DefaultAIResponse;
    }

    private void ScrollToBottom()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (AIChatMessageList.Children.Count > 0)
            {
                var lastChild = AIChatMessageList.Children[^1];
                lastChild.BringIntoView();
            }
        }, DispatcherPriority.Background);
    }
}
