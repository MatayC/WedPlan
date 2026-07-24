namespace WedPlan.Models;

/// <summary>
/// Repräsentiert einen individuellen Kostenposten der Wohnungsplanung
/// (z.B. Miete/Kaution, Möbel, Renovierung, Umzug).
/// </summary>
public class ApartmentItem
{
    /// <summary>Eindeutige Id des Postens.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Bezeichnung des Postens (z.B. "Sofa", "Kaution").</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Kategorie/Raum zur Gruppierung (z.B. "Wohnzimmer", "Umzug").</summary>
    public string Category { get; set; } = "Allgemein";

    /// <summary>Geschätzte/geplante Kosten.</summary>
    public decimal EstimatedCost { get; set; }

    /// <summary>Tatsächlich angefallene Kosten.</summary>
    public decimal ActualCost { get; set; }

    /// <summary>Bereits bezahlter Betrag.</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Zahlungsstatus des Postens.</summary>
    public BudgetStatus Status { get; set; } = BudgetStatus.Geplant;

    /// <summary>Optionaler Lieferant/Anbieter (z.B. Möbelhaus).</summary>
    public string? Vendor { get; set; }

    /// <summary>Optionales Fälligkeitsdatum der Zahlung.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Optionale Notiz.</summary>
    public string? Notes { get; set; }
}
