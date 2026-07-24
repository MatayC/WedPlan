namespace WedPlan.Models;

/// <summary>
/// Repräsentiert einen Posten im Budget-Planer.
/// </summary>
public class BudgetItem
{
    /// <summary>Eindeutige Id des Budget-Postens.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Bezeichnung des Postens (z.B. "Location", "Fotograf").</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Kategorie zur Gruppierung (z.B. "Deko", "Catering").</summary>
    public string Category { get; set; } = "Allgemein";

    /// <summary>Geschätzte/geplante Kosten.</summary>
    public decimal EstimatedCost { get; set; }

    /// <summary>Tatsächlich angefallene Kosten.</summary>
    public decimal ActualCost { get; set; }

    /// <summary>Bereits bezahlter Betrag.</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Zahlungsstatus des Postens.</summary>
    public BudgetStatus Status { get; set; } = BudgetStatus.Geplant;

    /// <summary>Optionaler Lieferant/Anbieter (z.B. Name der Firma oder Person).</summary>
    public string? Vendor { get; set; }

    /// <summary>Optionales Fälligkeitsdatum der Zahlung.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Optionale Notiz.</summary>
    public string? Notes { get; set; }
}
