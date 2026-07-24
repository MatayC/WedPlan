using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Service für den Finanzen-Bereich: persönliche Einträge (Einkommen/Ausgaben/Sparen)
/// beider Partner sowie die monatlichen Kontostände zur Verlaufsverfolgung.
/// </summary>
public interface IFinanceService
{
    // ----- Einträge -----

    /// <summary>Liefert alle Finanz-Einträge der aktiven Hochzeit.</summary>
    Task<List<FinanceEntry>> GetEntriesAsync(bool forceRefresh = false);

    /// <summary>Fügt einen neuen Eintrag hinzu.</summary>
    Task AddEntryAsync(FinanceEntry entry);

    /// <summary>Aktualisiert einen bestehenden Eintrag.</summary>
    Task UpdateEntryAsync(FinanceEntry entry);

    /// <summary>Löscht einen Eintrag anhand seiner Id.</summary>
    Task DeleteEntryAsync(Guid id);

    // ----- Kontostände -----

    /// <summary>Liefert alle Kontostand-Einträge (nach Monat sortiert).</summary>
    Task<List<FinanceBalance>> GetBalancesAsync(bool forceRefresh = false);

    /// <summary>Fügt einen neuen Kontostand hinzu oder aktualisiert den Monatswert.</summary>
    Task SaveBalanceAsync(FinanceBalance balance);

    /// <summary>Löscht einen Kontostand-Eintrag anhand seiner Id.</summary>
    Task DeleteBalanceAsync(Guid id);

    // ----- Cache -----

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    bool HasCache { get; }

    /// <summary>Verwirft den Cache, sodass die Daten neu aus der Cloud geladen werden.</summary>
    void InvalidateCache();
}
