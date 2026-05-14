# UIKitChatBubble — 聊天气泡控件

## 一、控件简介

`UIKitChatBubble` 是专为 **AI Chat 桌面客户端**场景设计的聊天气泡控件，继承 Avalonia `ContentControl`，支持通过 `Content` 属性放置任意子控件。

核心能力：

| 能力 | 说明 |
|------|------|
| 三种角色 | `User`（用户）/ `Assistant`（AI）/ `System`（系统），自动切换对齐、颜色、圆角 |
| 头像插槽 | `Avatar` 属性支持任意内容（Image / Border / PathIcon），仅 Assistant 角色显示 |
| 主题响应 | 气泡背景色走 `DynamicResource`，切换主题实时变化 |
| 内容灵活 | 继承 `ContentControl`，可包裹 `UIKitTypewriter` 实现流式回答效果 |
| 时间戳 | 可选 `Timestamp`，非空时显示，为空时自动隐藏 |

---

## 二、属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Role` | `BubbleRole` | `User` | 气泡角色，决定对齐方向和外观 |
| `Avatar` | `object?` | `null` | 头像内容（任意控件），仅 Assistant 角色且有值时显示 |
| `Message` | `string?` | `null` | 消息文本（简化用法） |
| `Timestamp` | `string?` | `null` | 时间戳文本，非空时在气泡下方显示 |
| `Content` | `object?` | `null` | 气泡内容（继承自 ContentControl，支持任意子控件） |

---

## 三、角色枚举（BubbleRole）

| 枚举值 | 对齐 | 气泡背景 | 文字颜色 | 圆角 | 头像 |
|--------|------|---------|---------|------|------|
| `User` | 右对齐 | Primary 蓝 | 白色 | 12,12,2,12（右下尖角） | 无 |
| `Assistant` | 左对齐 | Surface 灰 | TextPrimary | 12,12,12,2（左下尖角） | 可选 |
| `System` | 居中 | 透明 + 灰边框 | TextSecondary 灰 | 100（胶囊） | 无 |

---

## 四、Template Parts

| Part Name | 控件类型 | 说明 |
|-----------|---------|------|
| `PART_Bubble` | `Border` | 气泡背景容器，不同角色有不同的 Background / CornerRadius |
| `PART_Avatar` | `Border` | 头像容器（32×32 圆形），默认隐藏 |
| `PART_ContentArea` | `StackPanel` | 气泡 + 时间戳的容器，控制整体对齐 |
| `PART_Timestamp` | `TextBlock` | 时间戳文字，默认隐藏 |

---

## 五、AXAML 用法

### 5.1 引入命名空间

```xml
xmlns:kitBubble="clr-namespace:AvaloniaUIKit.Controls.ChatBubble;assembly=AvaloniaUIKit"
```

### 5.2 简化用法（纯文本）

```xml
<!-- 用户消息 -->
<kitBubble:UIKitChatBubble Role="User"
                            Message="你好！"
                            Timestamp="14:30" />

<!-- AI 消息 -->
<kitBubble:UIKitChatBubble Role="Assistant"
                            Message="你好！有什么可以帮你的？"
                            Timestamp="14:31" />

<!-- 系统消息 -->
<kitBubble:UIKitChatBubble Role="System"
                            Message="已连接到 AI 助手" />
```

### 5.3 含头像的 AI 消息

```xml
<kitBubble:UIKitChatBubble Role="Assistant" Timestamp="14:31">
  <kitBubble:UIKitChatBubble.Avatar>
    <Border Width="32" Height="32" CornerRadius="16"
            Background="{DynamicResource UIKit.Primary}">
      <TextBlock Text="AI" Foreground="White" FontWeight="Bold"
                 HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
  </kitBubble:UIKitChatBubble.Avatar>
  你好！有什么可以帮你的？
</kitBubble:UIKitChatBubble>
```

### 5.4 组合 UIKitTypewriter（流式回答）

```xml
<kitBubble:UIKitChatBubble Role="Assistant" Timestamp="14:31">
  <kitBubble:UIKitChatBubble.Avatar>
    <Border Width="32" Height="32" CornerRadius="16" Background="Gold" />
  </kitBubble:UIKitChatBubble.Avatar>
  <kitTw:UIKitTypewriter x:Name="AiReplyTypewriter" CharIntervalMs="25" />
</kitBubble:UIKitChatBubble>
```

### 5.5 在 ItemsControl 中构建消息列表

```xml
<ItemsControl ItemsSource="{Binding Messages}">
  <ItemsControl.ItemTemplate>
    <DataTemplate>
      <kitBubble:UIKitChatBubble Role="{Binding Role}"
                                  Timestamp="{Binding Time}">
        <TextBlock Text="{Binding Text}" TextWrapping="Wrap" />
      </kitBubble:UIKitChatBubble>
    </DataTemplate>
  </ItemsControl.ItemTemplate>
</ItemsControl>
```

---

## 六、Token 参考

| Resource Key | 类型 | 浅色值 | 深色值 | 用途 |
|---|---|---|---|---|
| `UIKit.ChatBubbleUserBgColor` | `Color` | `#FF1677FF` | `#FF1677FF` | 用户气泡背景 |
| `UIKit.ChatBubbleAssistantBgColor` | `Color` | `#FFF5F5F5` | `#FF2A2A2A` | AI 气泡背景 |
| `UIKit.ChatBubbleUserBg` | `SolidColorBrush` | 引用上方 Color | 同左 | 用户气泡 Brush |
| `UIKit.ChatBubbleAssistantBg` | `SolidColorBrush` | 引用上方 Color | 同左 | AI 气泡 Brush |

> **注意**：用户气泡深浅色都保持 `#FF1677FF`，确保品牌色一致。AI 气泡深色模式用 `#FF2A2A2A`（比 Surface 更深），与背景形成层次。

自定义颜色示例：

```xml
<Application.Resources>
  <!-- 将用户气泡改为绿色 -->
  <Color x:Key="UIKit.ChatBubbleUserBgColor">#FF52C41A</Color>
</Application.Resources>
```

---

## 七、面试技术要点

### 7.1 为什么继承 ContentControl 而非 TemplatedControl？

`ContentControl` 自带 `Content` 和 `ContentTemplate` 属性，支持两种用法：
- **直接设置内容**：`<kitBubble:UIKitChatBubble>任意内容</kitBubble:UIKitChatBubble>`
- **数据模板**：通过 `ContentTemplate` 绑定 ViewModel

这让它既能用于纯 code-behind 场景，也能无缝接入 MVVM，灵活性更好。
`TemplatedControl` 需要手动定义 Content 属性，且不支持 DataTemplate 模式。

### 7.2 PseudoClasses 驱动角色切换

三种角色（User / Assistant / System）通过 `PseudoClasses.Set(":user", ...)` 驱动不同的样式选择器，
包括对齐方向、背景色、圆角形状。**C# 代码只管状态，XAML 样式只管视觉**，完全分离。

> **考点**：与 WPF 的 `VisualStateManager` 对比。Avalonia 的 PseudoClass 更接近 CSS 概念，
> 可以被样式选择器直接匹配（如 `controls|UIKitChatBubble:user /template/ Border#PART_Bubble`），
> 不需要 `VisualStateGroup` / `Storyboard` 那套重量级机制。

### 7.3 非对称圆角设计

气泡圆角故意不对称：
- User：`12,12,2,12`（右下角尖锐，像聊天气泡的"尾巴"）
- Assistant：`12,12,12,2`（左下角尖锐）

这是现代 Chat UI 的常见设计模式，体现对细节的关注。

### 7.4 Avatar 的条件显示

头像不是通过样式选择器控制（Avalonia 不支持属性非空判断 Selector），
而是在 `OnApplyTemplate` + `AvatarProperty.Changed` 中用 C# 代码控制 `PART_Avatar.IsVisible`。
这种 **样式管不了的逻辑交给 code-behind** 是正确的分层方式。

### 7.5 与 UIKitTypewriter 的组合能力

在 ChatBubble 内包裹 UIKitTypewriter，可以实现：
1. 用户发送消息 → 添加 User 气泡
2. 切换到 Thinking → 添加 Assistant 气泡，内部 Typewriter 状态设为 Thinking
3. 流式返回 → Typewriter.AppendText 逐字显示
4. 完成 → Typewriter 状态设为 Done

这是 AI Chat 客户端的标准数据流，组件设计完全匹配。
