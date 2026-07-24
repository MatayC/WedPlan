namespace WedPlan.Models;

/// <summary>
/// Monatlicher Kontostand einer Person. Idealerweise trägt jede Person
/// jeweils zum 15. eines Monats ihren aktuellen Kontostand ein, sodass sich
/// die Entwicklung über die Monate nachverfolgen lässt.
/// </summary>
public class FinanceBalance
{
    /// <summary>Eindeutige Id des Kontostand-Eintrags.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Welcher Person der Kontostand gehört.</summary>
    public FinancePerson Person { get; set; } = FinancePerson.Partner1;

    /// <summary>Monat/Jahr des Eintrags (immer auf den 1. des Monats normalisiert).</summary>
    public DateTime Month { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);

    /// <summary>Kontostand zum Zeitpunkt der Erfassung.</summary>
    public decimal Amount { get; set; }

    /// <summary>Optionale Notiz.</summary>
    public string? Notes { get; set; }
}
