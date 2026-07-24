namespace WedPlan.Models;

/// <summary>
/// Ein einzelner Finanz-Eintrag einer Person (Einkommen, Fixkosten,
/// variable Kosten oder Sparen). Mehrere Einträge ergeben zusammen die
/// persönliche Finanzsituation.
/// </summary>
public class FinanceEntry
{
    /// <summary>Eindeutige Id des Eintrags.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Welcher Person der Eintrag zugeordnet ist.</summary>
    public FinancePerson Person { get; set; } = FinancePerson.Partner1;

    /// <summary>Bezeichnung (z.B. "Gehalt", "Miete", "Netflix").</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Betrag in der Projektwährung.</summary>
    public decimal Amount { get; set; }

    /// <summary>Art des Eintrags (bestimmt die Berechnung).</summary>
    public FinanceEntryType Type { get; set; } = FinanceEntryType.Fixkosten;

    /// <summary>Kategorie des Eintrags.</summary>
    public FinanceCategory Category { get; set; } = FinanceCategory.Sonstiges;

    /// <summary>Intervall (monatlich, jährlich, einmalig).</summary>
    public FinanceInterval Interval { get; set; } = FinanceInterval.Monatlich;

    /// <summary>Optionale Notiz.</summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Der auf einen Monat normalisierte Betrag: jährliche Werte werden durch 12
    /// geteilt, einmalige Werte fließen nicht in die Monatsbetrachtung ein.
    /// </summary>
    public decimal MonthlyAmount => Interval switch
    {
        FinanceInterval.Monatlich => Amount,
        FinanceInterval.Jaehrlich => Amount / 12m,
        _ => 0m
    };
}
