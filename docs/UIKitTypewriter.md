# UIKitTypewriter — AI 打字机控件

## 一、控件简介

`UIKitTypewriter` 是专为 **AI Chat 桌面客户端**场景设计的打字机效果控件，基于 Avalonia `TemplatedControl` 实现。

核心能力：

| 能力 | 说明 |
|------|------|
| 流式文字追加 | `AppendText(string)` 线程安全，可在后台线程（如 HttpClient 读流）直接调用 |
| 打字动画 | `DispatcherTimer` 逐字出现，间隔可配置 |
| 思考中动画 | 三点错开延迟跳动，模拟 AI 处理中的等待感 |
| 光标闪烁 | 0.8s 无限闪烁，流式输出时显示 |
| 跳过动画 | `SkipAnimation()` 立即显示全文，适合用户主动加速 |
| 主题响应 | 光标色 / 思考点颜色均走 `DynamicResource`，切换主题实时响应 |

---

## 二、属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `FullText` | `string?` | `null` | 当前已显示的完整文本（含已出队字符） |
| `CharIntervalMs` | `int` | `30` | 每个字符打出的时间间隔（毫秒），越小越快 |
| `State` | `TypewriterState` | `Idle` | 控件状态，驱动动画显示逻辑 |

---

## 三、方法

| 方法 | 线程安全 | 说明 |
|------|---------|------|
| `AppendText(string chunk)` | ✅ 是 | 向打字队列追加文字片段，自动启动计时器 |
| `SkipAnimation()` | ✅ 是 | 立即将队列中所有字符全部显示并触发 `TypewritingCompleted` |
| `Reset()` | ✅ 是 | 清空文字、清空队列、停止计时器，回到空白初始态 |

---

## 四、事件

| 事件 | 参数 | 触发时机 |
|------|------|---------|
| `TypewritingCompleted` | `EventArgs` | 字符队列清空时（正常打完 / 跳过动画后） |

---

## 五、状态枚举（TypewriterState）

| 枚举值 | PseudoClass | 视觉效果 |
|--------|-------------|---------|
| `Idle` | 无 | 仅显示文本，无任何动画 |
| `Thinking` | `:thinking` | 显示三点跳动动画，文本区为空 |
| `Streaming` | `:streaming` | 显示光标闪烁 + 逐字打出效果 |
| `Done` | `:done` | 光标隐藏，全文静态显示 |

> **注意**：`State` 属性只控制动画显示，不自动触发 `AppendText`。业务代码需手动在合适时机切换状态。

---

## 六、AXAML 用法

### 6.1 引入命名空间

```xml
xmlns:kitTw="clr-namespace:AvaloniaUIKit.Controls.Typewriter;assembly=AvaloniaUIKit"
```

### 6.2 基础用法

```xml
<kitTw:UIKitTypewriter x:Name="Typewriter1"
                       FontSize="14"
                       CharIntervalMs="30" />
```

### 6.3 嵌入卡片（推荐）

```xml
<kitCard:UIKitCard Header="AI 回答">
  <kitTw:UIKitTypewriter x:Name="AiReplyTypewriter"
                         FontSize="14"
                         CharIntervalMs="25" />
</kitCard:UIKitCard>
```

---

## 七、流式追加示例（模拟 LLM API）

以下代码展示如何接入流式 HTTP 响应（SSE / chunked），在后台线程安全追加文字：

```csharp
// 开始请求前 → 思考中
AiReplyTypewriter.Reset();
AiReplyTypewriter.State = TypewriterState.Thinking;

// 发起流式请求
using var response = await _httpClient.SendAsync(request,
    HttpCompletionOption.ResponseHeadersRead, cancellationToken);

var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
using var reader = new StreamReader(stream);

// 切换为流式输出状态
AiReplyTypewriter.State = TypewriterState.Streaming;

string? line;
while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
{
    if (line.StartsWith("data: "))
    {
        var chunk = ParseChunk(line); // 解析 SSE data
        if (!string.IsNullOrEmpty(chunk))
            AiReplyTypewriter.AppendText(chunk); // 线程安全，直接调用
    }
}
// TypewritingCompleted 事件会在队列打完后自动触发，在事件里切换状态：
// AiReplyTypewriter.State = TypewriterState.Done;
```

---

## 八、Token 参考

| Resource Key | 类型 | 浅色值 | 深色值 | 用途 |
|---|---|---|---|---|
| `UIKit.TypewriterCursorColor` | `Color` | `#FF1677FF` | `#FF4096FF` | 光标颜色 |
| `UIKit.TypewriterThinkingColor` | `Color` | `#FF8C8C8C` | `#FF595959` | 三点颜色 |
| `UIKit.TypewriterCursor` | `SolidColorBrush` | 引用上方 Color | 同左 | 光标 Brush |
| `UIKit.TypewriterThinking` | `SolidColorBrush` | 引用上方 Color | 同左 | 三点 Brush |

自定义颜色示例（在 `App.axaml` 中覆盖）：

```xml
<Application.Resources>
  <!-- 将光标改为绿色 -->
  <Color x:Key="UIKit.TypewriterCursorColor">#FF52C41A</Color>
</Application.Resources>
```

---

## 九、Template Parts

| Part Name | 控件类型 | 说明 |
|-----------|---------|------|
| `PART_Text` | `TextBlock` | 显示已打出的文字 |
| `PART_Cursor` | `Border` | 光标竖线（`:streaming` 时可见） |
| `PART_ThinkingDots` | `StackPanel` | 三点容器（`:thinking` 时可见） |
| `Dot1` / `Dot2` / `Dot3` | `Ellipse` | 三个跳动圆点 |

---

## 十、面试技术要点

以下是面试时可能被深问的关键实现细节，务必能流畅解释：

### 10.1 线程安全追加的实现

`AppendText` 用 `ConcurrentQueue<char>` 作为线程安全的缓冲区，字符入队后通过
`Dispatcher.UIThread.Post(EnsureTimerRunning)` 切回 UI 线程启动 `DispatcherTimer`。
这样即使在 `Task.Run` 或 `HttpClient` 回调线程调用 `AppendText`，也不会产生跨线程异常。

> **考点**：为什么不直接用 `lock`？因为 Avalonia 控件属性必须在 UI 线程访问，`ConcurrentQueue` + `Dispatcher.Post` 是更符合 Avalonia 编程模型的方案。

### 10.2 PseudoClasses 驱动动画

状态切换通过 `PseudoClasses.Set(":thinking", ...)` 驱动样式选择器（如 `controls|UIKitTypewriter:thinking`），
不需要在 C# 代码里操作任何 `IsVisible` 或 `Animation`，**完全由样式层控制视觉**，做到关注点分离。

> **考点**：这与 WPF 的 `VisualStateManager` 类似，但 Avalonia 的 PseudoClass 更轻量，直接复用 CSS 伪类的概念。

### 10.3 动画延迟错开的实现

三个 `Ellipse` 用相同的 `KeyFrame` 曲线（0%→25%→50% 跳），但分别设置 `Animation.Delay="0:0:0"` / `0:0:0.2` / `0:0:0.4`，
形成波浪感。这是纯 XAML 实现，无需任何 C# 代码。

> **踩坑**：Avalonia 11.2.3 的 `Animation.KeyFrame.Setter` 不支持直接 animate `RenderTransform` 子属性
> （`Property="RenderTransform"` 会报 `No animator registered` 运行时异常）。
> 解决方案是改用 **`Margin` 动画**模拟 Y 方向位移（`Margin="0,6,0,0"` = 向下偏移 6px）。
> 面试时可以展示你对 Avalonia 动画限制的理解和变通思路。

### 10.4 SkipAnimation 的设计

直接在 UI 线程停止 Timer、清空队列、同步文本，保证动画中断后状态一致。
同样触发 `TypewritingCompleted` 事件，调用方无需区分"打完"还是"跳过"两种结束路径。

### 10.5 为什么用 TemplatedControl 而非 UserControl？

`TemplatedControl` 允许外部通过样式完全替换模板（`Template` Setter），符合组件库可复用可定制的设计原则。
`UserControl` 的模板是固化的，不利于让使用方自定义外观。

### 10.6 CharIntervalMs 设计思路

通过 `StyledProperty` 暴露，可以在 XAML 直接绑定或在运行时动态调整：
```csharp
// 低端设备加速打字
typewriter.CharIntervalMs = 10;
// 戏剧效果放慢
typewriter.CharIntervalMs = 120;
```
计时器在 `EnsureTimerRunning` 时读取最新值，下次启动时生效。
