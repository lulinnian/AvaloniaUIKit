# UIKitButton

封装自 Avalonia 标准 `Button`，通过 `Classes` 切换视觉变体。

---

## 引用命名空间

```xml
xmlns:kit="clr-namespace:AvaloniaUIKit.Controls.Button;assembly=AvaloniaUIKit"
```

---

## 变体

| Classes | 说明 |
|---------|------|
| (无) | Primary — 填充蓝色背景 |
| `secondary` | 描边按钮，蓝色文字 |
| `danger` | 危险操作，红色背景 |
| `ghost` | 透明背景 + 灰色边框 |
| `link` | 纯文字链接样式，无边框 |

## 尺寸修饰

| Classes | 说明 |
|---------|------|
| `small` | 小号（高度 24px） |
| (无) | 默认（高度 32px） |
| `large` | 大号（高度 40px） |

---

## 用法示例

```xml
<!-- 基本用法 -->
<kit:UIKitButton>确认</kit:UIKitButton>

<!-- 组合变体 + 尺寸 -->
<kit:UIKitButton Classes="danger large">删除账号</kit:UIKitButton>

<!-- 禁用 -->
<kit:UIKitButton IsEnabled="False">提交中...</kit:UIKitButton>

<!-- 绑定命令 -->
<kit:UIKitButton Command="{Binding SaveCommand}">保存</kit:UIKitButton>
```

---

## 主题色联动

按钮颜色绑定以下 Token，覆盖即可换色：

- `UIKit.Primary` / `UIKit.PrimaryHover` / `UIKit.PrimaryActive`
- `UIKit.Danger` / `UIKit.DangerHover` / `UIKit.DangerActive`
