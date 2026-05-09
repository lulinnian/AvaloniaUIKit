# AvaloniaUIKit 开发踩坑记录

本文档记录了开发 AvaloniaUIKit 控件库过程中遇到的所有问题及解决方案。

---

## 目录

1. [XML 注释中包含 `--` 导致编译失败](#1-xml-注释中包含----导致编译失败)
2. [.NET 9 SDK NuGet 还原失败 (path1 null)](#2-net-9-sdk-nuget-还原失败-path1-null)
3. [命名空间与类型名冲突 (CS0118)](#3-命名空间与类型名冲突-cs0118)
4. [Avalonia WrapPanel 不支持 Spacing 属性 (AVLN2000)](#4-avalonia-wrappanel-不支持-spacing-属性-avln2000)
5. [PowerShell `;` 分隔符导致命令解析失败](#5-powershell--分隔符导致命令解析失败)
6. [PATH 环境变量损坏导致 dotnet 命令失败](#6-path-环境变量损坏导致-dotnet-命令失败)
7. [Demo 浅色/深色模式切换无响应](#7-demo-浅色深色模式切换无响应)

---

## 1. XML 注释中包含 `--` 导致编译失败

### 问题

在 `.axaml` 文件的 `<!-- -->` 注释中使用了装饰性分隔线：

```xml
<!-- ── Button 样式 ── -->
```

MSBuild 报错：`XML comment comment started with <!-- ── ...` 导致编译失败。

### 原因

XML 规范禁止在注释内容中出现 `--`（即使是 Unicode 字符 `─` 也不行，MSBuild 的 XML 解析器会将其处理为 `--`）。

### 解决方案

移除注释中的装饰性符号，改用纯文本：

```xml
<!-- Button 样式 -->
```

**经验教训**：`.axaml` 文件中的 `<!-- -->` 注释需严格遵守 XML 规范，不得包含 `--` 序列。

---

## 2. .NET 9 SDK NuGet 还原失败 (path1 null)

### 问题

执行 `dotnet restore` 时报错：

```
C:\Program Files\dotnet\sdk\9.0.301\NuGet.targets(789,5): error : Value cannot be null. (Parameter 'path1')
```

### 原因

.NET 9 SDK 的 NuGet 工具链存在 bug，在处理某些项目配置时 `Path.Combine()` 收到 null 参数。

### 解决方案

使用 `nuget.exe` CLI 进行还原，绕过 .NET 9 SDK 的 bug：

```powershell
# 下载 nuget.exe（如果没有）
Invoke-WebRequest -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe

# 使用 nuget.exe 还原
.\nuget.exe restore AvaloniaUIKit.sln -NonInteractive

# 然后使用 dotnet build --no-restore 构建
dotnet build AvaloniaUIKit.sln -c Debug --no-restore
```

**临时解决方案**：在修复 SDK bug 之前，始终使用 `nuget.exe restore` + `dotnet build --no-restore` 的组合。

---

## 3. 命名空间与类型名冲突 (CS0118)

### 问题

创建 `AvaloniaUIKit.Controls.Button` 命名空间下的 `UIKitButton` 类时，编译器报错：

```
CS0118: 'Button' is a namespace but is used like a type
```

### 原因

命名空间 `AvaloniaUIKit.Controls.Button` 包含了类型名 `Button`（来自 `Avalonia.Controls.Button`），导致完全限定名冲突。

### 解决方案

在类声明中显式指定父类型，而非使用 `Button` 作为基类简写：

```csharp
// ❌ 错误 - 编译器认为 "Button" 是命名空间
namespace AvaloniaUIKit.Controls.Button
{
    public class UIKitButton : Button { }
}

// ✅ 正确 - 显式使用完全限定名
namespace AvaloniaUIKit.Controls.Button
{
    public class UIKitButton : Avalonia.Controls.Button { }
}
```

**命名约定建议**：考虑将命名空间改为复数形式（如 `Buttons`）来避免与类型名冲突。

---

## 4. Avalonia WrapPanel 不支持 Spacing 属性 (AVLN2000)

### 问题

在 `.axaml` 中使用 `WrapPanel` 并设置 `Spacing` 属性：

```xml
<WrapPanel Spacing="8">
```

编译器报错：

```
AVLN2000: Unable to find best match for StyleProperty for property 'Spacing'
```

### 原因

Avalonia 的 `WrapPanel` 控件没有 `Spacing` 属性（与 WPF 的 `WrapPanel` 不同）。

### 解决方案

使用子元素 `Margin` + 父容器负 `Margin` 的技巧实现间距：

```xml
<!-- ✅ 正确方式：父容器负 Margin 抵消子元素外边距 -->
<WrapPanel Margin="-4">
    <kit:UIKitButton Margin="4">Button 1</kit:UIKitButton>
    <kit:UIKitButton Margin="4">Button 2</kit:UIKitButton>
</WrapPanel>
```

---

## 5. PowerShell `;` 分隔符导致命令解析失败

### 问题

在 PowerShell 中使用 `;` 分隔多个命令时，如果命令中包含括号或特殊字符，会导致解析错误。

### 原因

PowerShell 将 `;` 视为命令分隔符，且在解析复杂表达式时可能出现问题。

### 解决方案

使用 `ProcessStartInfo` 直接启动进程，而非通过 PowerShell 命令字符串：

```csharp
// 使用 ProcessStartInfo 直接调用 MSBuild
var psi = new ProcessStartInfo {
    FileName = msbuildPath,
    Arguments = $"\"{slnPath}\" /p:Configuration=Debug",
    UseShellExecute = false,
    RedirectStandardOutput = true
};
```

或者在 PowerShell 中使用脚本块（`{ }`）或 `Start-Process`。

---

## 6. PATH 环境变量损坏导致 dotnet 命令失败

### 问题

执行 `dotnet` 命令时报错：

```
The term 'C:\Program' is not recognized as the name of a cmdlet...
```

### 原因

PATH 环境变量中某个条目缺少 `C:` 前缀，导致路径被错误解析（如 `\Program Files\...` 而非 `C:\Program Files\...`）。

### 解决方案

通过注册表修复 PATH 环境变量：

```powershell
# 读取当前 PATH
$path = (Get-ItemProperty -Path 'HKCU:\Environment' -Name PATH).PATH

# 修复缺少盘符的路径
$path = $path -replace '(?<=;)\\', ';C:\'

# 写回注册表
Set-ItemProperty -Path 'HKCU:\Environment' -Name PATH -Value $path
```

**预防措施**：修改 PATH 时始终使用绝对路径（包含盘符）。

---

## 7. Demo 浅色/深色模式切换无响应

### 问题

点击 Demo 主窗口的「浅色模式」/「深色模式」按钮没有任何反应。

### 原因

`MainWindow.axaml` 中虽然定义了按钮的 `x:Name`（如 `BtnLightTheme`、`BtnDarkTheme`），但 `MainWindow.axaml.cs` 中没有：
1. 获取按钮引用
2. 绑定 `Click` 事件处理函数
3. 实现主题切换逻辑

### 解决方案

在 `MainWindow.axaml.cs` 中添加主题切换逻辑：

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaUIKit.Controls.Button;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeThemeButtons();
    }

    private void InitializeThemeButtons()
    {
        var btnLight = this.FindControl<UIKitButton>("BtnLightTheme");
        if (btnLight != null)
        {
            btnLight.Click += (_, _) =>
            {
                if (Application.Current != null)
                {
                    Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                }
            };
        }

        var btnDark = this.FindControl<UIKitButton>("BtnDarkTheme");
        if (btnDark != null)
        {
            btnDark.Click += (_, _) =>
            {
                if (Application.Current != null)
                {
                    Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                }
            };
        }
    }
}
```

### 关键要点

- 必须使用 `Application.Current.RequestedThemeVariant` 来切换主题（Avalonia 11 方式）
- `FindControl<T>` 需要在 `InitializeComponent()` 之后调用
- 需要添加 `using Avalonia.Styling` 和 `using Avalonia` 命名空间

---

## 总结

| 问题类型 | 数量 | 关键教训 |
|---------|------|----------|
| XML/Avalonia 语法 | 2 | 严格遵守 XML 规范；注意 Avalonia 与 WPF 的差异 |
| .NET SDK 工具链 | 1 | .NET 9 存在 NuGet bug，可用 nuget.exe 绕过 |
| C# 编译 | 1 | 命名空间与类型名冲突时显式使用完全限定名 |
| PowerShell 脚本 | 1 | 复杂命令使用 `ProcessStartInfo` 而非字符串拼接 |
| 环境变量 | 1 | PATH 修改后需验证格式正确性 |
| Avalonia 主题 API | 1 | 使用 `RequestedThemeVariant` 切换主题 |

---

*最后更新：2026-05-09*
