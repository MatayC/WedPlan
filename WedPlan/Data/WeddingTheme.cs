using MudBlazor;

namespace WedPlan.Data;

/// <summary>
/// Zentrale Definition des Hochzeits-Themes (Light + Dark) für MudBlazor.
/// Farbpalette: Rosé/Blush (#E8B4B8), Creme (#F5EFE6), Gold (#C9A227),
/// Anthrazit für Text. Elegante Serif-Font für Überschriften, Sans-Serif für Text.
/// </summary>
public static class WeddingTheme
{
    /// <summary>Das fertig konfigurierte MudTheme für die App.</summary>
    public static MudTheme Theme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            // Primärfarbe: warmes Gold für Akzente/Buttons.
            Primary = "#C9A227",
            // Sekundärfarbe: sanftes Rosé/Blush.
            Secondary = "#E8B4B8",
            Tertiary = "#B98A8D",
            // Hintergründe in Creme-Tönen.
            Background = "#F5EFE6",
            BackgroundGray = "#EFE7DA",
            Surface = "#FFFDF9",
            // Anthrazit für Text.
            TextPrimary = "#2E2A28",
            TextSecondary = "#6B615C",
            AppbarBackground = "#E8B4B8",
            AppbarText = "#2E2A28",
            DrawerBackground = "#FFFDF9",
            DrawerText = "#2E2A28",
            DrawerIcon = "#C9A227",
            Success = "#7BA17D",
            Error = "#C15B5B",
            Warning = "#D9A441",
            Info = "#8AA0B5"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#D9B94A",
            Secondary = "#E8B4B8",
            Tertiary = "#C99FA2",
            Background = "#1F1B1A",
            BackgroundGray = "#171312",
            Surface = "#2A2523",
            TextPrimary = "#F5EFE6",
            TextSecondary = "#C9BFB8",
            AppbarBackground = "#2A2523",
            AppbarText = "#F5EFE6",
            DrawerBackground = "#2A2523",
            DrawerText = "#F5EFE6",
            DrawerIcon = "#D9B94A",
            Success = "#7BA17D",
            Error = "#C15B5B",
            Warning = "#D9A441",
            Info = "#8AA0B5"
        },
        // Sanft abgerundete Ecken für ein elegantes, weiches Erscheinungsbild.
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "16px"
        },
        Typography = new Typography
        {
            // Standard-Text in einer klaren, modernen Sans-Serif-Font.
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "Helvetica", "Arial", "sans-serif" },
                FontSize = "0.95rem",
                LineHeight = "1.6",
                LetterSpacing = "normal"
            },
            // Überschriften ebenfalls klar und deutlich (Sans-Serif), mit kräftigem Gewicht.
            H1 = new H1Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "800", LineHeight = "1.2", LetterSpacing = "-0.5px" },
            H2 = new H2Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "800", LineHeight = "1.2", LetterSpacing = "-0.5px" },
            H3 = new H3Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "700", LineHeight = "1.25", LetterSpacing = "-0.3px" },
            H4 = new H4Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "700", LineHeight = "1.3" },
            H5 = new H5Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "700", LineHeight = "1.35" },
            H6 = new H6Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "600", LineHeight = "1.4" },
            Subtitle1 = new Subtitle1Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "500", LineHeight = "1.5" },
            Subtitle2 = new Subtitle2Typography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "500", LineHeight = "1.5" },
            Button = new ButtonTypography { FontFamily = new[] { "Plus Jakarta Sans", "Segoe UI", "sans-serif" }, FontWeight = "600", LetterSpacing = "0.2px", TextTransform = "none" }
        }
    };
}
