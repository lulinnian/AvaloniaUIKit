using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace AvaloniaUIKit.Controls.Typewriter;

/// <summary>
/// UIKitTypewriter — AI 打字机控件
/// <para>
/// 专为 AI Chat 场景设计，支持流式文字追加、打字动画、思考中动画等状态。
/// </para>
/// <para>
/// 状态（通过 <see cref="State"/> 属性控制）：
/// <list type="bullet">
///   <item><description>Idle      — 初始/空闲，无动画</description></item>
///   <item><description>Thinking  — 思考中，显示三点跳动动画</description></item>
///   <item><description>Streaming — 流式输出中，显示光标闪烁</description></item>
///   <item><description>Done      — 输出完毕，光标消失</description></item>
/// </list>
/// </para>
/// <para>
/// 流式追加：调用 <see cref="AppendText(string)"/> 即可线程安全地追加文字并驱动打字动画。
/// </para>
/// </summary>
/// <example>
/// <code>
/// // 开始思考
/// Typewriter1.State = TypewriterState.Thinking;
///
/// // 流式追加（可在后台线程调用）
/// Typewriter1.AppendText("Hello, ");
/// Typewriter1.AppendText("World!");
///
/// // 跳过动画立即显示全文
/// Typewriter1.SkipAnimation();
/// </code>
/// </example>
public class UIKitTypewriter : TemplatedControl
{
    // ─── Template Part Names ───────────────────────────────────────────────
    private const string PartTextBlock = "PART_Text";

    // ─── Styled Properties ─────────────────────────────────────────────────

    /// <summary>完整文本（已显示 + 待显示队列之和的最终结果）</summary>
    public static readonly StyledProperty<string?> FullTextProperty =
        AvaloniaProperty.Register<UIKitTypewriter, string?>(nameof(FullText));

    /// <summary>每个字符的打字间隔（毫秒），默认 30ms</summary>
    public static readonly StyledProperty<int> CharIntervalMsProperty =
        AvaloniaProperty.Register<UIKitTypewriter, int>(nameof(CharIntervalMs), defaultValue: 30);

    /// <summary>控件当前状态</summary>
    public static readonly StyledProperty<TypewriterState> StateProperty =
        AvaloniaProperty.Register<UIKitTypewriter, TypewriterState>(nameof(State), defaultValue: TypewriterState.Idle);

    // ─── Public Properties ─────────────────────────────────────────────────

    /// <summary>完整文本（已显示 + 待显示队列之和的最终结果）</summary>
    public string? FullText
    {
        get => GetValue(FullTextProperty);
        set => SetValue(FullTextProperty, value);
    }

    /// <summary>每个字符的打字间隔（毫秒），默认 30ms</summary>
    public int CharIntervalMs
    {
        get => GetValue(CharIntervalMsProperty);
        set => SetValue(CharIntervalMsProperty, value);
    }

    /// <summary>控件当前状态</summary>
    public TypewriterState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    // ─── Events ────────────────────────────────────────────────────────────

    /// <summary>打字动画结束（队列清空）时触发</summary>
    public event EventHandler? TypewritingCompleted;

    // ─── Internal Fields ───────────────────────────────────────────────────
    private readonly ConcurrentQueue<char> _queue = new();
    private readonly StringBuilder _displayed = new();
    private DispatcherTimer? _timer;
    private TextBlock? _textBlock;

    // ─── Static Constructor ────────────────────────────────────────────────
    static UIKitTypewriter()
    {
        AffectsRender<UIKitTypewriter>(FullTextProperty, StateProperty);
        StateProperty.Changed.AddClassHandler<UIKitTypewriter>(OnStatePropertyChanged);
    }

    // ─── Template ──────────────────────────────────────────────────────────

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _textBlock = e.NameScope.Find<TextBlock>(PartTextBlock);
        // 若已有内容（比如 XAML 绑定了 FullText）则直接显示
        if (!string.IsNullOrEmpty(FullText))
        {
            _displayed.Clear();
            _displayed.Append(FullText);
            SyncTextBlock();
        }
    }

    // ─── Public API ────────────────────────────────────────────────────────

    /// <summary>
    /// 线程安全地追加文字到打字队列。
    /// 可在后台线程（如 HttpClient 流式读取）直接调用。
    /// </summary>
    /// <param name="chunk">待追加的文字片段</param>
    public void AppendText(string chunk)
    {
        if (string.IsNullOrEmpty(chunk)) return;

        // 将每个字符入队
        foreach (var ch in chunk)
            _queue.Enqueue(ch);

        // 确保 UI 线程上的计时器已启动
        Dispatcher.UIThread.Post(EnsureTimerRunning);
    }

    /// <summary>
    /// 跳过打字动画，立即将队列内所有字符全部显示并停止计时器。
    /// </summary>
    public void SkipAnimation()
    {
        Dispatcher.UIThread.Post(SkipAnimationCore);
    }

    /// <summary>
    /// 重置控件到初始状态：清空已显示文本、清空队列、停止计时器。
    /// </summary>
    public void Reset()
    {
        Dispatcher.UIThread.Post(ResetCore);
    }

    // ─── Private Helpers ───────────────────────────────────────────────────

    private void EnsureTimerRunning()
    {
        if (_timer != null && _timer.IsEnabled) return;
        if (_queue.IsEmpty) return;

        _timer ??= new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(1, CharIntervalMs));
        _timer.Tick -= Timer_Tick;   // 防止重复订阅
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_queue.TryDequeue(out var ch))
        {
            _displayed.Append(ch);
            SyncTextBlock();
            // 同步更新 FullText（只增量，不重建队列）
            SetCurrentValue(FullTextProperty, _displayed.ToString());
        }
        else
        {
            // 队列已空 → 停止计时器，触发完成事件
            _timer?.Stop();
            TypewritingCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SkipAnimationCore()
    {
        _timer?.Stop();
        while (_queue.TryDequeue(out var ch))
            _displayed.Append(ch);
        SyncTextBlock();
        SetCurrentValue(FullTextProperty, _displayed.ToString());
        TypewritingCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void ResetCore()
    {
        _timer?.Stop();
        // 清空队列
        while (_queue.TryDequeue(out _)) { }
        _displayed.Clear();
        SyncTextBlock();
        SetCurrentValue(FullTextProperty, string.Empty);
    }

    private void SyncTextBlock()
    {
        if (_textBlock != null)
            _textBlock.Text = _displayed.ToString();
    }

    // ─── State → PseudoClasses ─────────────────────────────────────────────

    private static void OnStatePropertyChanged(UIKitTypewriter sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdatePseudoClasses((TypewriterState)e.NewValue!);
    }

    private void UpdatePseudoClasses(TypewriterState state)
    {
        PseudoClasses.Set(":thinking",  state == TypewriterState.Thinking);
        PseudoClasses.Set(":streaming", state == TypewriterState.Streaming);
        PseudoClasses.Set(":done",      state == TypewriterState.Done);
    }
}
