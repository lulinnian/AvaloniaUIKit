using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaUIKit;
using AvaloniaUIKit.Controls.Button;
using AvaloniaUIKit.Controls.ChatInput;
using AvaloniaUIKit.Controls.Typewriter;
using System;
using System.IO;
using System.Linq;

namespace AvaloniaUIKit.Demo;

public partial class MainWindow : Window
{
    // 演示用文本，模拟 AI 流式回答
    private const string DemoText =
        "你好！我是 AvaloniaUIKit 打字机控件。\n\n" +
        "我专为 AI Chat 场景设计，支持：\n" +
        "• 流式文字追加（AppendText）\n" +
        "• 思考中三点跳动动画\n" +
        "• 光标闪烁动画\n" +
        "• 跳过动画立即显示\n\n" +
        "适合接入 LLM 流式 API，打造原生桌面 AI 应用。";

    public MainWindow()
    {
        InitializeComponent();
        InitializeThemeButtons();
        InitializeNavButtons();
        InitializeTypewriterDemo();
        InitializeChatInputDemo();
    }

    // ─── 主题切换 ───────────────────────────────────────────────────────────

    private void InitializeThemeButtons()
    {
        BtnLightTheme.Click += BtnLightTheme_Click;
        BtnDarkTheme.Click  += BtnDarkTheme_Click;
    }

    private void BtnLightTheme_Click(object? sender, RoutedEventArgs e)
    {
        UIKitThemeService.SetLightTheme();
    }

    private void BtnDarkTheme_Click(object? sender, RoutedEventArgs e)
    {
        UIKitThemeService.SetDarkTheme();
    }

    // ─── 侧边导航 ──────────────────────────────────────────────────────────

    private void InitializeNavButtons()
    {
        NavButton.Click     += NavButton_Click;
        NavBadge.Click      += NavButton_Click;
        NavTextBox.Click    += NavButton_Click;
        NavCard.Click       += NavButton_Click;
        NavTypewriter.Click += NavButton_Click;
        NavChatBubble.Click += NavButton_Click;
        NavChatInput.Click  += NavButton_Click;
        NavAIChat.Click     += NavButton_Click;
    }

    private void NavButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        var tag = btn.Tag?.ToString();
        if (string.IsNullOrEmpty(tag)) return;

        // 根据 Tag 名找到对应的内容控件，滚动到可见区域
        var target = this.FindControl<Control>(tag);
        if (target != null)
        {
            target.BringIntoView();
        }
    }

    // ─── Typewriter Demo ────────────────────────────────────────────────────

    private void InitializeTypewriterDemo()
    {
        BtnTwThinking.Click += BtnTwThinking_Click;
        BtnTwPlay.Click     += BtnTwPlay_Click;
        BtnTwSkip.Click     += BtnTwSkip_Click;
        BtnTwReset.Click    += BtnTwReset_Click;

        Typewriter1.TypewritingCompleted += Typewriter1_TypewritingCompleted;
    }

    /// <summary>进入思考中状态，展示三点跳动动画</summary>
    private void BtnTwThinking_Click(object? sender, RoutedEventArgs e)
    {
        Typewriter1.Reset();
        Typewriter1.State = TypewriterState.Thinking;
        TwStatusText.Text = "状态：Thinking（思考中）";
    }

    /// <summary>模拟 AI 流式输出：先思考 1.5s，再逐字打印演示文本</summary>
    private async void BtnTwPlay_Click(object? sender, RoutedEventArgs e)
    {
        // 禁用播放按钮，防止重复点击
        BtnTwPlay.IsEnabled = false;

        Typewriter1.Reset();
        Typewriter1.State = TypewriterState.Thinking;
        TwStatusText.Text = "状态：Thinking（思考中）";

        // 模拟网络延迟
        await Task.Delay(1500);

        // 切换到流式输出状态
        Typewriter1.State = TypewriterState.Streaming;
        TwStatusText.Text = "状态：Streaming（输出中）";

        // 模拟流式 chunk 追加（每次追加一小段，间隔随机）
        var random = new Random();
        var chunkSize = 3;
        for (var i = 0; i < DemoText.Length; i += chunkSize)
        {
            var end   = Math.Min(i + chunkSize, DemoText.Length);
            var chunk = DemoText[i..end];
            Typewriter1.AppendText(chunk);
            await Task.Delay(random.Next(30, 80));
        }
    }

    /// <summary>跳过动画，立即显示全文</summary>
    private void BtnTwSkip_Click(object? sender, RoutedEventArgs e)
    {
        Typewriter1.SkipAnimation();
    }

    /// <summary>重置控件到初始状态</summary>
    private void BtnTwReset_Click(object? sender, RoutedEventArgs e)
    {
        Typewriter1.Reset();
        Typewriter1.State = TypewriterState.Idle;
        TwStatusText.Text = "状态：Idle";
        BtnTwPlay.IsEnabled = true;
    }

    /// <summary>打字完成回调：切换为 Done 状态并恢复播放按钮</summary>
    private void Typewriter1_TypewritingCompleted(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Typewriter1.State   = TypewriterState.Done;
            TwStatusText.Text   = "状态：Done（输出完毕）";
            BtnTwPlay.IsEnabled = true;
        });
    }

    // ─── ChatInput Demo ─────────────────────────────────────────────────────

    private void InitializeChatInputDemo()
    {
        ChatInput1.Send += ChatInput1_Send;
        ChatInput2.Send += ChatInput2_Send;
        ChatInput1.AttachmentAdded += ChatInput_AttachmentAdded;
        ChatInput2.AttachmentAdded += ChatInput_AttachmentAdded;
    }

    /// <summary>Enter 模式发送回调</summary>
    private void ChatInput1_Send(object? sender, ChatInputSendEventArgs e)
    {
        ChatInput1Result.Text = $"已发送（Enter 模式）：{e.Message}";
    }

    /// <summary>Ctrl+Enter 模式发送回调</summary>
    private void ChatInput2_Send(object? sender, ChatInputSendEventArgs e)
    {
        ChatInput2Result.Text = $"已发送（Ctrl+Enter 模式）：{e.Message}";
    }

    /// <summary>附件添加回调（拖拽 / 粘贴）</summary>
    private void ChatInput_AttachmentAdded(object? sender, ChatInputAttachmentEventArgs e)
    {
        var files = string.Join(", ", e.FilePaths.Select(Path.GetFileName));
        ChatInput1Result.Text = $"添加附件：{files}";
    }
}
