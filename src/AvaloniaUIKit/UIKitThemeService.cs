using Avalonia.Media;

namespace AvaloniaUIKit;

/// <summary>
/// UIKit 主题服务，负责在运行时动态切换浅色/深色主题。
/// </summary>
public static class UIKitThemeService
{
    /// <summary>浅色模式</summary>
    public static void SetLightTheme() => ApplyColors(LightColors);

    /// <summary>深色模式</summary>
    public static void SetDarkTheme() => ApplyColors(DarkColors);

    private static void ApplyColors(ColorSet colors)
    {
        var app = Avalonia.Application.Current;
        if (app == null) return;

        // 一次性覆盖所有颜色 Token，DynamicResource 会自动响应变化
        var r = app.Resources;

        // Primary
        r["UIKit.PrimaryColor"] = colors.PrimaryColor;
        r["UIKit.PrimaryHoverColor"] = colors.PrimaryHoverColor;
        r["UIKit.PrimaryActiveColor"] = colors.PrimaryActiveColor;
        r["UIKit.PrimaryForegroundColor"] = colors.PrimaryForegroundColor;

        // Danger
        r["UIKit.DangerColor"] = colors.DangerColor;
        r["UIKit.DangerHoverColor"] = colors.DangerHoverColor;
        r["UIKit.DangerActiveColor"] = colors.DangerActiveColor;

        // Success
        r["UIKit.SuccessColor"] = colors.SuccessColor;

        // Warning
        r["UIKit.WarningColor"] = colors.WarningColor;

        // Neutrals
        r["UIKit.BackgroundColor"] = colors.BackgroundColor;
        r["UIKit.SurfaceColor"] = colors.SurfaceColor;
        r["UIKit.BorderColor"] = colors.BorderColor;
        r["UIKit.BorderFocusColor"] = colors.BorderFocusColor;

        // Text
        r["UIKit.TextPrimaryColor"] = colors.TextPrimaryColor;
        r["UIKit.TextSecondaryColor"] = colors.TextSecondaryColor;
        r["UIKit.TextDisabledColor"] = colors.TextDisabledColor;

        // Typewriter
        r["UIKit.TypewriterCursorColor"] = colors.TypewriterCursorColor;
        r["UIKit.TypewriterThinkingColor"] = colors.TypewriterThinkingColor;
    }

    private record ColorSet(
        Color PrimaryColor, Color PrimaryHoverColor, Color PrimaryActiveColor, Color PrimaryForegroundColor,
        Color DangerColor, Color DangerHoverColor, Color DangerActiveColor,
        Color SuccessColor, Color WarningColor,
        Color BackgroundColor, Color SurfaceColor, Color BorderColor, Color BorderFocusColor,
        Color TextPrimaryColor, Color TextSecondaryColor, Color TextDisabledColor,
        // Typewriter
        Color TypewriterCursorColor, Color TypewriterThinkingColor);

    private static readonly ColorSet LightColors = new(
        /* Primary       */  PrimaryColor: Color.Parse("#FF1677FF"),
        /* PrimaryHover  */  PrimaryHoverColor: Color.Parse("#FF4096FF"),
        /* PrimaryActive */  PrimaryActiveColor: Color.Parse("#FF0958D9"),
        /* PrimaryFg     */  PrimaryForegroundColor: Color.Parse("#FFFFFFFF"),
        /* Danger        */  DangerColor: Color.Parse("#FFFF4D4F"),
        /* DangerHover   */  DangerHoverColor: Color.Parse("#FFFF7875"),
        /* DangerActive  */  DangerActiveColor: Color.Parse("#FFCF1322"),
        /* Success       */  SuccessColor: Color.Parse("#FF52C41A"),
        /* Warning       */  WarningColor: Color.Parse("#FFFAAD14"),
        /* Background    */  BackgroundColor: Color.Parse("#FFFFFFFF"),
        /* Surface       */  SurfaceColor: Color.Parse("#FFF5F5F5"),
        /* Border        */  BorderColor: Color.Parse("#FFD9D9D9"),
        /* BorderFocus   */  BorderFocusColor: Color.Parse("#FF1677FF"),
        /* TextPrimary   */  TextPrimaryColor: Color.Parse("#FF000000"),
        /* TextSecondary */  TextSecondaryColor: Color.Parse("#FF8C8C8C"),
        /* TextDisabled  */  TextDisabledColor: Color.Parse("#FFBFBFBF"),
        // Typewriter
        /* TwCursor      */  TypewriterCursorColor: Color.Parse("#FF1677FF"),
        /* TwThinking    */  TypewriterThinkingColor: Color.Parse("#FF8C8C8C")
    );

    private static readonly ColorSet DarkColors = new(
        /* Primary       */  PrimaryColor: Color.Parse("#FF4096FF"),
        /* PrimaryHover  */  PrimaryHoverColor: Color.Parse("#FF69B1FF"),
        /* PrimaryActive */  PrimaryActiveColor: Color.Parse("#FF1677FF"),
        /* PrimaryFg     */  PrimaryForegroundColor: Color.Parse("#FFFFFFFF"),
        /* Danger        */  DangerColor: Color.Parse("#FFFF7875"),
        /* DangerHover   */  DangerHoverColor: Color.Parse("#FFFFB8B8"),
        /* DangerActive  */  DangerActiveColor: Color.Parse("#FFFF4D4F"),
        /* Success       */  SuccessColor: Color.Parse("#FF95DE64"),
        /* Warning       */  WarningColor: Color.Parse("#FFFFD666"),
        /* Background    */  BackgroundColor: Color.Parse("#FF141414"),
        /* Surface       */  SurfaceColor: Color.Parse("#FF1F1F1F"),
        /* Border        */  BorderColor: Color.Parse("#FF424242"),
        /* BorderFocus   */  BorderFocusColor: Color.Parse("#FF4096FF"),
        /* TextPrimary   */  TextPrimaryColor: Color.Parse("#FFE8E8E8"),
        /* TextSecondary */  TextSecondaryColor: Color.Parse("#FF8C8C8C"),
        /* TextDisabled  */  TextDisabledColor: Color.Parse("#FF595959"),
        // Typewriter
        /* TwCursor      */  TypewriterCursorColor: Color.Parse("#FF4096FF"),
        /* TwThinking    */  TypewriterThinkingColor: Color.Parse("#FF595959")
    );
}
