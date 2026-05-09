# AvaloniaUIKit

> 一个基于 **Avalonia 11+** 构建的可复用 UI 组件库，提供统一的主题色体系、控件样式系统和常用控件，帮助多个 Avalonia 客户端项目消除重复开发。

---

## 特性

- **主题色 Token 系统** — 所有颜色通过语义化资源键（`UIKit.Primary` 等）定义，客户端只需覆盖键值即可切换主题色。
- **深色 / 浅色模式** — 内置两套 Token 字典，合并对应字典即可切换。
- **样式系统** — 通过 `UIKitTheme.axaml` 一行代码注册所有控件样式。
- **扩展控件** — 提供 UIKitButton、UIKitCard、UIKitTextBox、UIKitBadge 等常用控件。
- **Demo 项目** — `samples/AvaloniaUIKit.Demo` 提供所有控件的可运行预览。
- **Visual Studio / Rider 友好** — 标准 `.sln` 解决方案结构。

---

## 项目结构

```
AvaloniaUIKit/
├── src/
│   └── AvaloniaUIKit/              # 核心组件库
│       ├── Themes/
│       │   ├── ColorTokens.axaml       # 浅色模式 Token
│       │   └── ColorTokensDark.axaml   # 深色模式 Token
│       ├── Styles/
│       │   ├── UIKitButtonStyles.axaml
│       │   ├── UIKitCardStyles.axaml
│       │   ├── UIKitTextBoxStyles.axaml
│       │   └── UIKitBadgeStyles.axaml
│       ├── Controls/
│       │   ├── Button/UIKitButton.cs
│       │   ├── Card/UIKitCard.cs
│       │   ├── TextBox/UIKitTextBox.cs
│       │   └── Badge/UIKitBadge.cs
│       ├── Extensions/
│       │   └── UIKitAppBuilderExtensions.cs
│       └── UIKitTheme.axaml            # 统一注册入口
├── samples/
│   └── AvaloniaUIKit.Demo/         # 控件预览 Demo 项目
├── docs/
│   ├── UIKitButton.md
│   ├── UIKitCard.md
│   ├── UIKitTextBox.md
│   ├── UIKitBadge.md
│   └── ThemeSystem.md
├── .gitignore
├── NuGet.Config
└── AvaloniaUIKit.sln
```

---

## 快速开始

### 1. 安装

方式 A — 项目引用（同一解决方案）：

```xml
<ProjectReference Include="..\..\src\AvaloniaUIKit\AvaloniaUIKit.csproj" />
```

方式 B — NuGet 包（发布后）：

```
dotnet add package AvaloniaUIKit
```

### 2. 注册主题

在客户端 `App.axaml` 中：

```xml
<Application.Styles>
  <FluentTheme />
  <!-- 注册 AvaloniaUIKit -->
  <StyleInclude Source="avares://AvaloniaUIKit/UIKitTheme.axaml" />
</Application.Styles>
```

在 `Program.cs` 中：

```csharp
AppBuilder.Configure<App>()
    .UsePlatformDetect()
    .UseAvaloniaUIKit()   // ← 添加这一行
    .WithInterFont()
    .LogToTrace();
```

### 3. 覆盖主题色

```xml
<Application.Resources>
  <!-- 改为紫色主题 -->
  <Color x:Key="UIKit.PrimaryColor">#FF722ED1</Color>
  <Color x:Key="UIKit.PrimaryHoverColor">#FF9254DE</Color>
  <Color x:Key="UIKit.PrimaryActiveColor">#FF531DAB</Color>
</Application.Resources>
```

### 4. 使用控件

```xml
xmlns:kit="clr-namespace:AvaloniaUIKit.Controls.Button;assembly=AvaloniaUIKit"
xmlns:kitBadge="clr-namespace:AvaloniaUIKit.Controls.Badge;assembly=AvaloniaUIKit"

<!-- 按钮 -->
<kit:UIKitButton>主要按钮</kit:UIKitButton>
<kit:UIKitButton Classes="secondary">次要按钮</kit:UIKitButton>
<kit:UIKitButton Classes="danger large">危险操作</kit:UIKitButton>

<!-- 徽标 -->
<kitBadge:UIKitBadge Classes="success pill" Text="已通过" />
```

---

## 运行 Demo

```bash
cd samples/AvaloniaUIKit.Demo
dotnet run
```

---

## 控件文档

| 控件 | 文档 |
|------|------|
| UIKitButton | [docs/UIKitButton.md](docs/UIKitButton.md) |
| UIKitCard | [docs/UIKitCard.md](docs/UIKitCard.md) |
| UIKitTextBox | [docs/UIKitTextBox.md](docs/UIKitTextBox.md) |
| UIKitBadge | [docs/UIKitBadge.md](docs/UIKitBadge.md) |
| 主题色系统 | [docs/ThemeSystem.md](docs/ThemeSystem.md) |

---

## Git 初始化

```bash
git init
git add .
git commit -m "feat: init AvaloniaUIKit component library"
git remote add origin https://github.com/yourname/AvaloniaUIKit.git
git push -u origin main
```

---

## 贡献

1. Fork 仓库
2. 创建特性分支 `feat/my-control`
3. 提交 PR，在描述中附上控件截图和用法示例

---

## 许可证

MIT
