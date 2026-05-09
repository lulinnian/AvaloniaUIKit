# 主题色系统

AvaloniaUIKit 使用 **Token（资源键）** 驱动颜色，所有控件只绑定 Token，而不直接写死颜色值。

这样客户端只需覆盖 Token，就能同时改变所有控件的主题色——包括客户端自己的私有控件（只要它们也绑定同一套 Token）。

---

## Token 列表

### 主色调

| Token 键 | 说明 | 默认值（浅色） |
|-----------|------|---------------|
| `UIKit.PrimaryColor` | 主色 | `#1677FF` |
| `UIKit.PrimaryHoverColor` | 主色悬停 | `#4096FF` |
| `UIKit.PrimaryActiveColor` | 主色按下 | `#0958D9` |
| `UIKit.PrimaryForegroundColor` | 主色背景上的文字 | `#FFFFFF` |
| `UIKit.DangerColor` | 危险色 | `#FF4D4F` |
| `UIKit.SuccessColor` | 成功色 | `#52C41A` |
| `UIKit.WarningColor` | 警告色 | `#FAAD14` |

### 背景 / 边框

| Token 键 | 说明 | 默认值 |
|-----------|------|--------|
| `UIKit.BackgroundColor` | 页面背景 | `#FFFFFF` |
| `UIKit.SurfaceColor` | 表面（卡片内次级背景）| `#F5F5F5` |
| `UIKit.BorderColor` | 默认边框 | `#D9D9D9` |
| `UIKit.BorderFocusColor` | 聚焦边框 | `#1677FF` |

### 文字

| Token 键 | 说明 | 默认值 |
|-----------|------|--------|
| `UIKit.TextPrimaryColor` | 主文字 | `#000000` |
| `UIKit.TextSecondaryColor` | 次要文字 | `#8C8C8C` |
| `UIKit.TextDisabledColor` | 禁用文字 | `#BFBFBF` |

### 圆角 / 字号 / 间距

| Token 键 | 类型 | 默认值 |
|-----------|------|--------|
| `UIKit.RadiusSmall` | CornerRadius | `4` |
| `UIKit.RadiusMedium` | CornerRadius | `6` |
| `UIKit.RadiusLarge` | CornerRadius | `8` |
| `UIKit.FontSizeSmall` | Double | `12` |
| `UIKit.FontSizeBase` | Double | `14` |
| `UIKit.FontSizeLarge` | Double | `16` |
| `UIKit.FontSizeTitle` | Double | `20` |

---

## 浅色 / 深色切换

**方式一：在 App.axaml 中合并深色 Token 字典**

```xml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <ResourceInclude Source="avares://AvaloniaUIKit/Themes/ColorTokensDark.axaml" />
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

**方式二：代码动态切换（运行时）**

```csharp
// 切换深色
var dark = new ResourceDictionary();
dark.MergedDictionaries.Add(
    (IResourceDictionary)AvaloniaXamlLoader.Load(
        new Uri("avares://AvaloniaUIKit/Themes/ColorTokensDark.axaml")));
Application.Current!.Resources.MergedDictionaries.Add(dark);
```

---

## 自定义主题色

在注册 `UIKitTheme.axaml` 之后，覆盖 Color Token：

```xml
<Application.Resources>
  <!-- 自定义主色为紫色 -->
  <Color x:Key="UIKit.PrimaryColor">#FF722ED1</Color>
  <Color x:Key="UIKit.PrimaryHoverColor">#FF9254DE</Color>
  <Color x:Key="UIKit.PrimaryActiveColor">#FF531DAB</Color>
</Application.Resources>
```

> **注意**：覆盖 `Color` 类型的键（而不是 `SolidColorBrush`），组件库内的 Brush Token 会通过 `DynamicResource` 自动引用最新的 Color 值。

---

## 私有控件绑定 Token

在客户端自己的控件中绑定同一套 Token，主题切换时也会自动更新：

```xml
<Border Background="{DynamicResource UIKit.Surface}"
        BorderBrush="{DynamicResource UIKit.Border}">
  <TextBlock Text="我的私有控件"
             Foreground="{DynamicResource UIKit.TextPrimary}" />
</Border>
```
