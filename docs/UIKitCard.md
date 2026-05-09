# UIKitCard

带圆角边框的通用卡片容器，支持可选的 `Header` 标题区域。

---

## 引用命名空间

```xml
xmlns:kitCard="clr-namespace:AvaloniaUIKit.Controls.Card;assembly=AvaloniaUIKit"
```

---

## 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Header` | `object?` | 卡片标题（可以是字符串或任意控件） |
| `Content` | `object?` | 卡片主体内容（继承自 ContentControl） |

## Classes 修饰

| Classes | 说明 |
|---------|------|
| (无) | 普通卡片，边框颜色固定 |
| `hoverable` | 鼠标悬停时边框变为主色调 |

---

## 用法示例

```xml
<!-- 带标题的卡片 -->
<kitCard:UIKitCard Header="用户信息">
  <StackPanel>
    <TextBlock Text="张三" />
    <TextBlock Text="admin@example.com" />
  </StackPanel>
</kitCard:UIKitCard>

<!-- Hoverable 卡片 -->
<kitCard:UIKitCard Classes="hoverable" Header="点击查看详情">
  <TextBlock Text="鼠标悬停时边框高亮" />
</kitCard:UIKitCard>

<!-- 纯内容卡片（无标题） -->
<kitCard:UIKitCard>
  <Image Source="..." />
</kitCard:UIKitCard>
```
