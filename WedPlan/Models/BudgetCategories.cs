namespace WedPlan.Models;

/// <summary>
/// Vordefinierte Budget-Kategorien als Konstanten (frei erweiterbar über Freitext).
/// Hält die im Projekt bekannten Standardkategorien zentral vor.
/// </summary>
public static class BudgetCategories
{
    public const string Location = "Location";
    public const string Catering = "Catering";
    public const string Fotograf = "Fotograf";
    public const string Musik = "Musik / DJ";
    public const string Deko = "Dekoration";
    public const string Kleidung = "Kleidung";
    public const string Ringe = "Ringe";
    public const string Einladungen = "Einladungen";
    public const string Blumen = "Blumen";
    public const string Torte = "Hochzeitstorte";
    public const string Sonstiges = "Sonstiges";

    /// <summary>Alle Standardkategorien als geordnete Liste für Autocomplete-Vorschläge.</summary>
    public static readonly IReadOnlyList<string> All = new[]
    {
        Location, Catering, Fotograf, Musik, Deko,
        Kleidung, Ringe, Einladungen, Blumen, Torte, Sonstiges
    };
}
