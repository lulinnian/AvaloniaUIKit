# UIKitBadge

轻量状态徽标控件，用于展示状态标签、计数气泡、分类标签等。

---

## 引用命名空间

```xml
xmlns:kitBadge="clr-namespace:AvaloniaUIKit.Controls.Badge;assembly=AvaloniaUIKit"
```

---

## 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Text` | `string?` | 徽标显示文字 |

## Classes 变体

| Classes | 说明 |
|---------|------|
| (无) | Primary 蓝色背景 |
| `secondary` | 描边风格，蓝色文字 |
| `success` | 绿色背景 |
| `warning` | 橙色背景 |
| `danger` | 红色背景 |
| `info` | 青色背景 |

## 形状修饰

| Classes | 说明 |
|---------|------|
| (无) | 圆角矩形（默认） |
| `pill` | 胶囊形（圆角 = 100） |

---

## 用法示例

```xml
<!-- 状态标签 -->
<kitBadge:UIKitBadge Classes="success" Text="已完成" />
<kitBadge:UIKitBadge Classes="warning" Text="审核中" />
<kitBadge:UIKitBadge Classes="danger" Text="已拒绝" />

<!-- 计数气泡 -->
<kitBadge:UIKitBadge Classes="danger pill" Text="99+" />

<!-- 描边风格 -->
<kitBadge:UIKitBadge Classes="secondary" Text="标签" />
```
