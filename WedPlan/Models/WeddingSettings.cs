namespace WedPlan.Models;

/// <summary>
/// Zentrale Einstellungen der Hochzeit, die von allen Bereichen genutzt werden.
/// </summary>
public class WeddingSettings
{
    /// <summary>Eindeutige Id (für spätere Mehrfach-Hochzeiten / DB-Anbindung).</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Name von Partner 1.</summary>
    public string Partner1Name { get; set; } = string.Empty;

    /// <summary>Name von Partner 2.</summary>
    public string Partner2Name { get; set; } = string.Empty;

    /// <summary>Datum der Hochzeit.</summary>
    public DateTime? WeddingDate { get; set; }

    /// <summary>Location/Veranstaltungsort.</summary>
    public string? Location { get; set; }

    /// <summary>Gesamtbudget für die Hochzeit.</summary>
    public decimal TotalBudget { get; set; }

    /// <summary>Währung (ISO-Code oder Symbol, z.B. "EUR", "€").</summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>Gibt an, ob der Dark Mode aktiviert ist.</summary>
    public bool IsDarkMode { get; set; }

    /// <summary>Öffentliche URL des hochgeladenen Titelbilds (Supabase Storage) oder null.</summary>
    public string? CoverImageUrl { get; set; }

    /// <summary>Öffentliche URL des ersten dezenten Dashboard-Bilds oder null.</summary>
    public string? GalleryImage1Url { get; set; }

    /// <summary>Öffentliche URL des zweiten dezenten Dashboard-Bilds oder null.</summary>
    public string? GalleryImage2Url { get; set; }

    /// <summary>Aktuell Erspartes von Partner 1 (Konto, separat vom Budget).</summary>
    public decimal SavingsPartner1 { get; set; }

    /// <summary>Aktuell Erspartes von Partner 2 (Konto, separat vom Budget).</summary>
    public decimal SavingsPartner2 { get; set; }

    /// <summary>Geschätzter monatlicher Sparbetrag von Partner 1 bis zur Hochzeit.</summary>
    public decimal MonthlySavingPartner1 { get; set; }

    /// <summary>Geschätzter monatlicher Sparbetrag von Partner 2 bis zur Hochzeit.</summary>
    public decimal MonthlySavingPartner2 { get; set; }
}
