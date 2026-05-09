# UIKitTextBox

在 Avalonia 标准 `TextBox` 基础上扩展，新增前缀/后缀内容插槽，并自动应用 UIKit 边框主题色。

---

## 引用命名空间

```xml
xmlns:kitBox="clr-namespace:AvaloniaUIKit.Controls.TextBox;assembly=AvaloniaUIKit"
```

---

## 扩展属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `PrefixContent` | `object?` | 输入框左侧内容（图标等） |
| `SuffixContent` | `object?` | 输入框右侧内容（清除按钮、单位等） |

其余属性继承自 `Avalonia.Controls.TextBox`（`Text`、`Watermark`、`PasswordChar`、`AcceptsReturn` 等）。

---

## 状态样式

| 状态 | 效果 |
|------|------|
| 默认 | 灰色边框 |
| 悬停 | 主色调边框 |
| 聚焦 | 主色调边框（`UIKit.BorderFocus`） |
| 禁用 | 灰色背景 + 低透明度 |

---

## 用法示例

```xml
<!-- 普通输入框 -->
<kitBox:UIKitTextBox Watermark="请输入关键词" />

<!-- 密码框 -->
<kitBox:UIKitTextBox Watermark="密码" PasswordChar="●" />

<!-- 带前缀图标 -->
<kitBox:UIKitTextBox Watermark="搜索">
  <kitBox:UIKitTextBox.PrefixContent>
    <PathIcon Data="{StaticResource SearchIcon}" Width="14" />
  </kitBox:UIKitTextBox.PrefixContent>
</kitBox:UIKitTextBox>

<!-- 多行文本域 -->
<kitBox:UIKitTextBox Watermark="备注"
                     AcceptsReturn="True"
                     MinHeight="100"
                     TextWrapping="Wrap" />
```
