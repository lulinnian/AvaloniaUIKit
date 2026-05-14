using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;

namespace AvaloniaUIKit.Controls.ChatInput;

/// <summary>
/// UIKitChatInput — AI 聊天输入区控件。
/// <para>
/// 支持多行编辑、Enter / Ctrl+Enter 发送、字数统计与限制、
/// 拖拽文件/图片、粘贴图片、附件预览、草稿持久化。
/// </para>
/// </summary>
/// <example>
/// <code>
/// &lt;!-- 基础用法：Enter 发送 --&gt;
/// &lt;kitInput:UIKitChatInput Watermark="输入消息..." /&gt;
///
/// &lt;!-- Ctrl+Enter 发送 --&gt;
/// &lt;kitInput:UIKitChatInput Watermark="输入消息..." SendOnCtrlEnter="True" /&gt;
///
/// &lt;!-- 带字数限制 --&gt;
/// &lt;kitInput:UIKitChatInput MaxLength="500" ShowCharCount="True" /&gt;
/// </code>
/// </example>
public class UIKitChatInput : TemplatedControl
{
    #region 字段

    private Avalonia.Controls.TextBox? _editBox;
    private Avalonia.Controls.Button? _sendButton;
    private ItemsControl? _attachmentsPanel;
    private Border? _dragOverlay;
    private TextBlock? _charCount;

    private const double LineHeight = 22.0;
    private const double MinInputHeight = 40.0;
    private const double MaxInputHeight = 200.0;

    #endregion

    #region PseudoClasses

    private static readonly string[] StatePseudoClasses = [":normal", ":typing", ":nearlimit", ":atlimit", ":disabled"];

    #endregion

    #region StyledProperties

    /// <summary>占位提示文字</summary>
    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<UIKitChatInput, string?>(nameof(Watermark), "输入消息...");

    /// <summary>输入文本（双向绑定）</summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<UIKitChatInput, string?>(nameof(Text), defaultValue: "", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    /// <summary>最大字符数（默认 4000）</summary>
    public static readonly StyledProperty<int> MaxLengthProperty =
        AvaloniaProperty.Register<UIKitChatInput, int>(nameof(MaxLength), 4000);

    /// <summary>发送按钮是否可用（只读，自动计算）</summary>
    public static readonly StyledProperty<bool> IsSendEnabledProperty =
        AvaloniaProperty.Register<UIKitChatInput, bool>(nameof(IsSendEnabled), false);

    /// <summary>草稿文本（用于持久化恢复）</summary>
    public static readonly StyledProperty<string?> DraftProperty =
        AvaloniaProperty.Register<UIKitChatInput, string?>(nameof(Draft));

    /// <summary>是否显示字数统计（默认 true）</summary>
    public static readonly StyledProperty<bool> ShowCharCountProperty =
        AvaloniaProperty.Register<UIKitChatInput, bool>(nameof(ShowCharCount), true);

    /// <summary>是否显示附件区域（默认 true）</summary>
    public static readonly StyledProperty<bool> ShowAttachmentsProperty =
        AvaloniaProperty.Register<UIKitChatInput, bool>(nameof(ShowAttachments), true);

    /// <summary>当前附件列表</summary>
    public static readonly StyledProperty<IList?> AttachmentsProperty =
        AvaloniaProperty.Register<UIKitChatInput, IList?>(nameof(Attachments));

    /// <summary>使用 Ctrl+Enter 发送而非 Enter（默认 false = Enter 发送）</summary>
    public static readonly StyledProperty<bool> SendOnCtrlEnterProperty =
        AvaloniaProperty.Register<UIKitChatInput, bool>(nameof(SendOnCtrlEnter), false);

    #endregion

    #region RoutedEvents

    /// <summary>用户触发发送（快捷键 / 点击按钮）</summary>
    public static readonly RoutedEvent<ChatInputSendEventArgs> SendEvent =
        RoutedEvent.Register<UIKitChatInput, ChatInputSendEventArgs>(nameof(Send), RoutingStrategies.Bubble);

    /// <summary>添加了新附件（拖拽 / 粘贴图片）</summary>
    public static readonly RoutedEvent<ChatInputAttachmentEventArgs> AttachmentAddedEvent =
        RoutedEvent.Register<UIKitChatInput, ChatInputAttachmentEventArgs>(nameof(AttachmentAdded), RoutingStrategies.Bubble);

    #endregion

    #region CLR 属性

    /// <summary>占位提示文字</summary>
    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    /// <summary>输入文本（双向绑定）</summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>最大字符数</summary>
    public int MaxLength
    {
        get => GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    /// <summary>发送按钮是否可用（只读，自动计算）</summary>
    public bool IsSendEnabled
    {
        get => GetValue(IsSendEnabledProperty);
        private set => SetValue(IsSendEnabledProperty, value);
    }

    /// <summary>草稿文本</summary>
    public string? Draft
    {
        get => GetValue(DraftProperty);
        set => SetValue(DraftProperty, value);
    }

    /// <summary>是否显示字数统计</summary>
    public bool ShowCharCount
    {
        get => GetValue(ShowCharCountProperty);
        set => SetValue(ShowCharCountProperty, value);
    }

    /// <summary>是否显示附件区域</summary>
    public bool ShowAttachments
    {
        get => GetValue(ShowAttachmentsProperty);
        set => SetValue(ShowAttachmentsProperty, value);
    }

    /// <summary>当前附件列表</summary>
    public IList? Attachments
    {
        get => GetValue(AttachmentsProperty);
        set => SetValue(AttachmentsProperty, value);
    }

    /// <summary>使用 Ctrl+Enter 发送（默认 false = Enter 发送）</summary>
    public bool SendOnCtrlEnter
    {
        get => GetValue(SendOnCtrlEnterProperty);
        set => SetValue(SendOnCtrlEnterProperty, value);
    }

    #endregion

    #region 事件

    /// <summary>用户触发发送事件</summary>
    public event EventHandler<ChatInputSendEventArgs> Send
    {
        add => AddHandler(SendEvent, value);
        remove => RemoveHandler(SendEvent, value);
    }

    /// <summary>附件添加事件</summary>
    public event EventHandler<ChatInputAttachmentEventArgs> AttachmentAdded
    {
        add => AddHandler(AttachmentAddedEvent, value);
        remove => RemoveHandler(AttachmentAddedEvent, value);
    }

    #endregion

    #region 静态构造函数

    static UIKitChatInput()
    {
        AffectsRender<UIKitChatInput>(
            WatermarkProperty, TextProperty, MaxLengthProperty,
            IsSendEnabledProperty, ShowCharCountProperty, ShowAttachmentsProperty,
            SendOnCtrlEnterProperty);

        TextProperty.Changed.AddClassHandler<UIKitChatInput>(OnTextChanged);
        MaxLengthProperty.Changed.AddClassHandler<UIKitChatInput>(OnMaxLengthChanged);
        IsEnabledProperty.Changed.AddClassHandler<UIKitChatInput>(OnIsEnabledChanged);
        ShowCharCountProperty.Changed.AddClassHandler<UIKitChatInput>(OnShowCharCountChanged);
        ShowAttachmentsProperty.Changed.AddClassHandler<UIKitChatInput>(OnShowAttachmentsChanged);
        SendOnCtrlEnterProperty.Changed.AddClassHandler<UIKitChatInput>(OnSendOnCtrlEnterChanged);
        IsSendEnabledProperty.Changed.AddClassHandler<UIKitChatInput>(OnIsSendEnabledChanged);
    }

    #endregion

    #region Template 方法

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // 解绑旧模板事件
        UnhookEditBoxEvents();
        UnhookDragDropEvents();

        // 获取模板部件
        _editBox = e.NameScope.Find<AvaloniaUIKit.Controls.TextBox.UIKitTextBox>("PART_EditBox");
        _sendButton = e.NameScope.Find<Avalonia.Controls.Button>("PART_SendButton");
        _attachmentsPanel = e.NameScope.Find<ItemsControl>("PART_AttachmentsPanel");
        _dragOverlay = e.NameScope.Find<Border>("PART_DragOverlay");
        _charCount = e.NameScope.Find<TextBlock>("PART_CharCount");

        // 绑定内部 TextBox
        if (_editBox != null)
        {
            _editBox.AcceptsReturn = true;
            _editBox.TextWrapping = TextWrapping.Wrap;
            _editBox.Watermark = Watermark;
            _editBox.Text = Text ?? string.Empty;
            _editBox.MaxLength = MaxLength > 0 ? MaxLength : int.MaxValue;

            HookEditBoxEvents();
            UpdateEditBoxHeight();
        }

        if (_sendButton != null)
        {
            _sendButton.Click += SendButton_Click;
            _sendButton.IsEnabled = IsSendEnabled;
        }

        // 初始化附件面板可见性
        UpdateAttachmentsPanelVisibility();

        // 初始化字数统计可见性
        UpdateCharCountVisibility();

        // 更新快捷键提示
        UpdateSendButtonContent();

        // 初始状态
        UpdateState();
        HookDragDropEvents();
    }

    #endregion

    #region 内部事件处理

    // ─── TextBox 事件 ────────────────────────────────────────────────────────

    private void HookEditBoxEvents()
    {
        if (_editBox == null) return;
        _editBox.AddHandler(KeyDownEvent, EditBox_KeyDown, RoutingStrategies.Tunnel);
        _editBox.TextChanged += EditBox_TextChanged;
        _editBox.GotFocus += EditBox_GotFocus;
        _editBox.LostFocus += EditBox_LostFocus;
    }

    private void UnhookEditBoxEvents()
    {
        if (_editBox == null) return;
        _editBox.RemoveHandler(KeyDownEvent, EditBox_KeyDown);
        _editBox.TextChanged -= EditBox_TextChanged;
        _editBox.GotFocus -= EditBox_GotFocus;
        _editBox.LostFocus -= EditBox_LostFocus;
    }

    private void EditBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (_editBox == null) return;

        bool isEnter = e.Key == Key.Enter;
        bool hasCtrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        bool isCtrlV = e.Key == Key.V && hasCtrl;

        // ─── Ctrl+V 粘贴：提前检查剪贴板图片 ───────────────────────────
        if (isCtrlV)
        {
            HandlePasteFromClipboard(e);
            // 如果已处理，HandlePasteFromClipboard 内部会设置 e.Handled = true
            return;
        }

        // ─── Enter 发送 ─────────────────────────────────────────────────
        if (SendOnCtrlEnter)
        {
            // Ctrl+Enter 模式：只有 Ctrl+Enter 才发送
            if (isEnter && hasCtrl)
            {
                TrySend();
                e.Handled = true;
            }
        }
        else
        {
            // Enter 模式：Enter 发送，Shift+Enter 换行
            if (isEnter && !e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                TrySend();
                e.Handled = true;
            }
        }
    }

    private void EditBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_editBox == null) return;

        // 同步文本到外部 TextProperty（避免循环）
        var newText = _editBox.Text ?? string.Empty;
        if (Text != newText)
        {
            Text = newText;
        }

        // 更新自动增高
        UpdateEditBoxHeight();

        // 更新状态
        UpdateState();
    }

    private void EditBox_GotFocus(object? sender, GotFocusEventArgs e)
    {
        PseudoClasses.Set(":focused", true);
    }

    private void EditBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        PseudoClasses.Set(":focused", false);

        // 失焦时保存草稿
        if (!string.IsNullOrEmpty(Text))
        {
            Draft = Text;
        }
    }

    // ─── 发送 ────────────────────────────────────────────────────────────────

    private void SendButton_Click(object? sender, RoutedEventArgs e)
    {
        TrySend();
    }

    private void TrySend()
    {
        if (!IsSendEnabled || _editBox == null) return;

        var message = _editBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(message)) return;

        var args = new ChatInputSendEventArgs(SendEvent, message);
        RaiseEvent(args);

        // 发送后清空输入
        _editBox.Clear();
        Text = string.Empty;
        UpdateEditBoxHeight();
        UpdateState();

        // 清空草稿
        Draft = null;
    }

    // ─── 拖拽 ────────────────────────────────────────────────────────────────

    private void HookDragDropEvents()
    {
        AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void UnhookDragDropEvents()
    {
        RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
        RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (IsEnabled && e.Data.Contains(DataFormats.Files))
        {
            e.DragEffects = DragDropEffects.Copy;
            PseudoClasses.Set(":dragover", true);
            if (_dragOverlay != null)
            {
                _dragOverlay.IsVisible = true;
            }
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void OnDragLeave(object? sender, RoutedEventArgs e)
    {
        PseudoClasses.Set(":dragover", false);
        if (_dragOverlay != null)
        {
            _dragOverlay.IsVisible = false;
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        PseudoClasses.Set(":dragover", false);
        if (_dragOverlay != null)
        {
            _dragOverlay.IsVisible = false;
        }

        if (!IsEnabled) return;

        var files = e.Data.GetFiles();
        if (files == null || !files.Any()) return;

        AddAttachments(files.Select(f => f.Path.LocalPath).ToArray());
        e.Handled = true;
    }

    // ─── 粘贴图片 ──────────────────────────────────────────────────────────
    //
    // 在 KeyDown Tunnel 中拦截 Ctrl+V，异步检查剪贴板是否包含图片。
    // 策略：
    //   1. 先同步设置 e.Handled = true 阻止 TextBox 默认粘贴行为
    //   2. 异步检查剪贴板内容
    //   3. 如果有图片 → 保存为临时文件作为附件
    //   4. 如果只有文本 → 手动将文本插入到 TextBox（模拟正常粘贴）
    //
    // 图片获取优先级：
    //   - 复制的文件（FileDrop）：Avalonia IClipboard.GetDataAsync("FileDrop") → string[]
    //   - 截图/复制的图片（Bitmap）：先尝试 Avalonia IClipboard，失败则 P/Invoke Win32 API
    //
    // 注意：不能使用 PastingFromClipboard 事件，因为它是同步检查 e.Handled，
    //       而 async handler 中的 await 会导致 e.Handled 设置太晚，无法阻止默认行为。

    private async void HandlePasteFromClipboard(KeyEventArgs e)
    {
        // 立即阻止默认粘贴行为
        e.Handled = true;

        if (_editBox == null || !IsEnabled) return;

        try
        {
            var topLevel = TopLevel.GetTopLevel(_editBox);
            var clipboard = topLevel?.Clipboard;
            if (clipboard == null) return;

            // 获取剪贴板可用格式
            var formats = await clipboard.GetFormatsAsync();

            // ─── 尝试处理文件粘贴（复制的图片文件） ─────────────────────
            if (formats != null && formats.Any(f => f.Equals("FileDrop", StringComparison.OrdinalIgnoreCase)))
            {
                var data = await clipboard.GetDataAsync("FileDrop");
                if (data is string[] paths && paths.Length > 0)
                {
                    var imagePaths = paths.Where(p =>
                    {
                        var ext = Path.GetExtension(p).ToLowerInvariant();
                        return ext is ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp" or ".svg";
                    }).ToArray();

                    if (imagePaths.Length > 0)
                    {
                        AddAttachments(imagePaths);
                        return;
                    }
                }
            }

            // ─── 尝试处理位图粘贴（截图 / Alt+PrtSc） ──────────────────
            bool hasImage = formats != null && formats.Any(f =>
                f.Equals("Bitmap", StringComparison.OrdinalIgnoreCase) ||
                f.Equals("PNG", StringComparison.OrdinalIgnoreCase) ||
                f.IndexOf("image/", StringComparison.OrdinalIgnoreCase) >= 0);

            string? savedFilePath = null;

            if (hasImage)
            {
                // 方式 1：尝试 Avalonia IClipboard.GetDataAsync("Bitmap")
                try
                {
                    var bitmapData = await clipboard.GetDataAsync("Bitmap");
                    if (bitmapData is Bitmap avaloniaBitmap)
                    {
                        savedFilePath = await SaveBitmapToTempFile(avaloniaBitmap);
                    }
                    else if (bitmapData is byte[] rawBytes && rawBytes.Length > 0)
                    {
                        savedFilePath = await SaveBytesToTempFile(rawBytes, ".png");
                    }
                }
                catch
                {
                    // Avalonia IClipboard 可能不支持此格式，回退到 Win32
                }

                // 方式 2：尝试 Avalonia IClipboard.GetDataAsync("PNG")
                if (savedFilePath == null)
                {
                    try
                    {
                        var pngData = await clipboard.GetDataAsync("PNG");
                        if (pngData is byte[] pngBytes && pngBytes.Length > 0)
                        {
                            savedFilePath = await SaveBytesToTempFile(pngBytes, ".png");
                        }
                    }
                    catch { }
                }

                // 方式 3：P/Invoke Win32 Clipboard API 直接获取位图数据
                if (savedFilePath == null && OperatingSystem.IsWindows())
                {
#pragma warning disable CA1416 // 调用点已在 OperatingSystem.IsWindows() 守卫下
                    savedFilePath = await Task.Run(() => TrySaveClipboardImageViaWin32());
#pragma warning restore CA1416
                }
            }

            if (savedFilePath != null)
            {
                AddAttachments([savedFilePath]);
                return;
            }

            // ─── 没有图片，回退到正常文本粘贴 ─────────────────────────
            var text = await clipboard.GetTextAsync();
            if (!string.IsNullOrEmpty(text))
            {
                _editBox.Text = _editBox.Text ?? string.Empty;
                var caretIndex = _editBox.CaretIndex;
                var currentText = _editBox.Text;
                _editBox.Text = currentText.Insert(caretIndex, text);
                _editBox.CaretIndex = caretIndex + text.Length;
            }
        }
        catch
        {
            // 剪贴板操作可能因权限或其他原因失败，静默忽略
        }
    }

    // ─── Win32 P/Invoke：直接从剪贴板获取位图数据 ──────────────────────────

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetClipboardData(uint uFormat);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool IsClipboardFormatAvailable(uint format);

    private const uint CF_DIB = 8;
    private const uint CF_BITMAP = 2;
    private const uint CF_PNG = 15;

    /// <summary>
    /// 通过 Win32 Clipboard API 获取剪贴板中的位图，保存为 PNG 文件。
    /// 优先使用 CF_PNG，回退到 CF_DIB。
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static string? TrySaveClipboardImageViaWin32()
    {
        if (!OpenClipboard(IntPtr.Zero)) return null;

        try
        {
            // 优先尝试 CF_PNG (0x0C031)
            IntPtr hGlobal;
            byte[]? pngBytes = null;

            // 尝试 PNG 格式
            hGlobal = GetClipboardData(CF_PNG);
            if (hGlobal != IntPtr.Zero)
            {
                pngBytes = GlobalLockToBytes(hGlobal);
            }

            // 回退到 DIB 格式
            if (pngBytes == null || pngBytes.Length == 0)
            {
                hGlobal = GetClipboardData(CF_DIB);
                if (hGlobal != IntPtr.Zero)
                {
                    pngBytes = ConvertDibToPng(hGlobal);
                }
            }

            if (pngBytes == null || pngBytes.Length == 0) return null;

            var tempDir = Path.Combine(Path.GetTempPath(), "AvaloniaUIKit", "PasteImages");
            Directory.CreateDirectory(tempDir);

            var fileName = $"paste_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.png";
            var filePath = Path.Combine(tempDir, fileName);

            File.WriteAllBytes(filePath, pngBytes);
            return filePath;
        }
        catch
        {
            return null;
        }
        finally
        {
            CloseClipboard();
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalUnlock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint GlobalSize(IntPtr hMem);

    private static byte[]? GlobalLockToBytes(IntPtr hGlobal)
    {
        var ptr = GlobalLock(hGlobal);
        if (ptr == IntPtr.Zero) return null;

        try
        {
            var size = GlobalSize(hGlobal);
            if (size == 0) return null;

            var bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, (int)size);
            return bytes;
        }
        finally
        {
            GlobalUnlock(hGlobal);
        }
    }

    /// <summary>
    /// 将 DIB (Device Independent Bitmap) 数据转换为 PNG 字节流。
    /// 使用 System.Drawing 在 .NET 8 上转换（Windows only）。
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static byte[]? ConvertDibToPng(IntPtr hGlobal)
    {
        var dibBytes = GlobalLockToBytes(hGlobal);
        if (dibBytes == null || dibBytes.Length < 40) return null;

        try
        {
            // DIB 头是 BITMAPINFOHEADER (40 bytes)，后面跟像素数据
            // 偏移量 14 是 BITMAPINFOHEADER.bfSize 所在的位置不对——
            // DIB 格式直接就是 BITMAPINFOHEADER + 像素数据（无 BITMAPFILEHEADER）

            // 用 System.Drawing.Bitmap 从 MemoryStream 加载需要 BMP 文件格式，
            // 所以需要先构造 BMP 头部
            var bmpBytes = CreateBmpFromDib(dibBytes);
            if (bmpBytes == null) return null;

            // 使用 System.Drawing 转换为 PNG
            using var ms = new MemoryStream(bmpBytes);
            using var bitmap = new System.Drawing.Bitmap(ms);
            using var pngStream = new MemoryStream();
            bitmap.Save(pngStream, System.Drawing.Imaging.ImageFormat.Png);
            return pngStream.ToArray();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 将 DIB 字节流（BITMAPINFOHEADER + 像素数据）转换为标准 BMP 字节流。
    /// BMP 格式 = 14 字节 BITMAPFILEHEADER + BITMAPINFOHEADER + 像素数据
    /// </summary>
    private static byte[]? CreateBmpFromDib(byte[] dibData)
    {
        if (dibData.Length < 40) return null;

        var headerSize = 14; // BITMAPFILEHEADER
        var bmp = new byte[headerSize + dibData.Length];

        // BITMAPFILEHEADER
        bmp[0] = (byte)'B';
        bmp[1] = (byte)'M';
        // bfSize = total file size
        var totalSize = (uint)(headerSize + dibData.Length);
        bmp[2] = (byte)(totalSize & 0xFF);
        bmp[3] = (byte)((totalSize >> 8) & 0xFF);
        bmp[4] = (byte)((totalSize >> 16) & 0xFF);
        bmp[5] = (byte)((totalSize >> 24) & 0xFF);
        // bfReserved1, bfReserved2 = 0
        // bfOffBits = header size (14)
        bmp[10] = (byte)(headerSize & 0xFF);
        bmp[11] = (byte)((headerSize >> 8) & 0xFF);

        // Copy DIB data (BITMAPINFOHEADER + pixel data)
        Array.Copy(dibData, 0, bmp, headerSize, dibData.Length);

        return bmp;
    }

    private static async Task<string?> SaveBitmapToTempFile(Bitmap bitmap)
    {
        try
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "AvaloniaUIKit", "PasteImages");
            Directory.CreateDirectory(tempDir);

            var fileName = $"paste_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.png";
            var filePath = Path.Combine(tempDir, fileName);

            // Bitmap.Encode 需要在 UI 线程或正确的上下文中执行
            await Task.Run(() =>
            {
                using var stream = File.Create(filePath);
                bitmap.Save(stream);
            });

            return filePath;
        }
        catch
        {
            return null;
        }
    }

    private static Task<string?> SaveBytesToTempFile(byte[] data, string extension)
    {
        try
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "AvaloniaUIKit", "PasteImages");
            Directory.CreateDirectory(tempDir);

            var fileName = $"paste_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(tempDir, fileName);

            return Task.Run(() =>
            {
                File.WriteAllBytes(filePath, data);
                return (string?)filePath;
            });
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }

    // ─── 状态更新 ─────────────────────────────────────────────────────────────

    private void UpdateState()
    {
        if (!IsEnabled)
        {
            SetStatePseudoClass(ChatInputState.Disabled);
            IsSendEnabled = false;
            UpdateCharCountText();
            return;
        }

        var text = Text ?? string.Empty;
        var len = text.Length;
        var max = MaxLength > 0 ? MaxLength : int.MaxValue;

        ChatInputState state;
        bool canSend;

        if (string.IsNullOrWhiteSpace(text))
        {
            state = ChatInputState.Normal;
            canSend = false;
        }
        else if (len >= max)
        {
            state = ChatInputState.AtLimit;
            canSend = false;
        }
        else if (len >= max * 0.8)
        {
            state = ChatInputState.NearLimit;
            canSend = true;
        }
        else
        {
            state = ChatInputState.Typing;
            canSend = true;
        }

        SetStatePseudoClass(state);
        IsSendEnabled = canSend;
        UpdateCharCountText();
    }

    private void SetStatePseudoClass(ChatInputState state)
    {
        PseudoClasses.Set(":normal", state == ChatInputState.Normal);
        PseudoClasses.Set(":typing", state == ChatInputState.Typing);
        PseudoClasses.Set(":nearlimit", state == ChatInputState.NearLimit);
        PseudoClasses.Set(":atlimit", state == ChatInputState.AtLimit);
        PseudoClasses.Set(":disabled", state == ChatInputState.Disabled);
    }

    private void UpdateCharCountText()
    {
        if (_charCount == null) return;
        var len = (Text ?? string.Empty).Length;
        var max = MaxLength > 0 ? MaxLength : int.MaxValue;
        _charCount.Text = max < int.MaxValue ? $"{len}/{max}" : string.Empty;
    }

    private void UpdateEditBoxHeight()
    {
        if (_editBox == null) return;
        // Avalonia 11 的 TextBox 没有 LineCount 属性，通过换行符计算行数
        var text = _editBox.Text ?? string.Empty;
        var lineCount = string.IsNullOrEmpty(text) ? 1 : text.Count(c => c == '\n') + 1;
        var desiredHeight = Math.Max(MinInputHeight, lineCount * LineHeight);
        desiredHeight = Math.Min(desiredHeight, MaxInputHeight);
        _editBox.MinHeight = desiredHeight;
    }

    private void UpdateAttachmentsPanelVisibility()
    {
        if (_attachmentsPanel == null) return;
        _attachmentsPanel.IsVisible = ShowAttachments;
    }

    private void UpdateCharCountVisibility()
    {
        if (_charCount == null) return;
        _charCount.IsVisible = ShowCharCount;
    }

    private void UpdateSendButtonContent()
    {
        if (_sendButton == null) return;
        _sendButton.Content = SendOnCtrlEnter ? "发送 ↵" : "发送";
    }

    // ─── 附件管理 ─────────────────────────────────────────────────────────────

    private void AddAttachments(string[] filePaths)
    {
        var list = Attachments as IList;
        if (list == null)
        {
            list = new ObservableCollection<object>();
            Attachments = list;
        }

        foreach (var path in filePaths)
        {
            list.Add(path);
        }

        // 触发事件
        var args = new ChatInputAttachmentEventArgs(AttachmentAddedEvent, filePaths);
        RaiseEvent(args);
    }

    // ─── Property Changed Handlers ────────────────────────────────────────────

    private static void OnTextChanged(UIKitChatInput sender, AvaloniaPropertyChangedEventArgs e)
    {
        // 从外部设置 Text 时，同步到内部 TextBox
        if (sender._editBox != null && sender._editBox.Text != (e.NewValue as string ?? string.Empty))
        {
            sender._editBox.Text = e.NewValue as string ?? string.Empty;
        }

        sender.UpdateState();
        sender.UpdateEditBoxHeight();
    }

    private static void OnMaxLengthChanged(UIKitChatInput sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender._editBox != null)
        {
            var max = (int)e.NewValue!;
            sender._editBox.MaxLength = max > 0 ? max : int.MaxValue;
        }
        sender.UpdateState();
    }

    private static void OnIsEnabledChanged(UIKitChatInput sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender._editBox != null)
        {
            sender._editBox.IsEnabled = (bool)e.NewValue!;
        }
        if (sender._sendButton != null)
        {
            sender._sendButton.IsEnabled = sender.IsEnabled && sender.IsSendEnabled;
        }
        sender.UpdateState();
    }

    private static void OnShowCharCountChanged(UIKitChatInput sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdateCharCountVisibility();
    }

    private static void OnShowAttachmentsChanged(UIKitChatInput sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdateAttachmentsPanelVisibility();
    }

    private static void OnSendOnCtrlEnterChanged(UIKitChatInput sender, AvaloniaPropertyChangedEventArgs e)
    {
        sender.UpdateSendButtonContent();
    }

    private static void OnIsSendEnabledChanged(UIKitChatInput sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender._sendButton != null)
        {
            sender._sendButton.IsEnabled = (bool)e.NewValue!;
        }
    }

    #endregion

    #region 公共方法

    /// <summary>聚焦到输入框</summary>
    public void FocusEditBox()
    {
        _editBox?.Focus();
    }

    /// <summary>清空所有输入和附件</summary>
    public void ClearAll()
    {
        Text = string.Empty;
        Draft = null;
        if (Attachments is IList list)
        {
            list.Clear();
        }
        UpdateEditBoxHeight();
        UpdateState();
    }

    /// <summary>加载草稿</summary>
    public void RestoreDraft(string draftText)
    {
        Draft = draftText;
        Text = draftText;
    }

    #endregion
}

/// <summary>发送事件参数，携带发送的文本内容</summary>
public class ChatInputSendEventArgs : RoutedEventArgs
{
    /// <summary>发送的文本消息</summary>
    public string Message { get; }

    public ChatInputSendEventArgs(RoutedEvent routedEvent, string message)
        : base(routedEvent)
    {
        Message = message;
    }
}

/// <summary>附件添加事件参数，携带添加的文件路径</summary>
public class ChatInputAttachmentEventArgs : RoutedEventArgs
{
    /// <summary>添加的文件路径列表</summary>
    public string[] FilePaths { get; }

    public ChatInputAttachmentEventArgs(RoutedEvent routedEvent, string[] filePaths)
        : base(routedEvent)
    {
        FilePaths = filePaths;
    }
}
