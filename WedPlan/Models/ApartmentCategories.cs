namespace WedPlan.Models;

/// <summary>
/// Vordefinierte Kategorien/Räume für die Wohnungsplanung (frei erweiterbar über Freitext).
/// </summary>
public static class ApartmentCategories
{
    public const string MieteKaution = "Miete / Kaution";
    public const string Umzug = "Umzug";
    public const string Renovierung = "Renovierung";
    public const string Kueche = "Küche";
    public const string Wohnzimmer = "Wohnzimmer";
    public const string Schlafzimmer = "Schlafzimmer";
    public const string Bad = "Badezimmer";
    public const string Buero = "Büro / Arbeitszimmer";
    public const string Elektro = "Elektrogeräte";
    public const string Deko = "Dekoration";
    public const string Garten = "Garten / Balkon";
    public const string Sonstiges = "Sonstiges";

    /// <summary>Alle Standardkategorien als geordnete Liste für Autocomplete-Vorschläge.</summary>
    public static readonly IReadOnlyList<string> All = new[]
    {
        MieteKaution, Umzug, Renovierung, Kueche, Wohnzimmer, Schlafzimmer,
        Bad, Buero, Elektro, Deko, Garten, Sonstiges
    };
}
