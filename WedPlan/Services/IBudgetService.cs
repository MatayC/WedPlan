using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Service für die Verwaltung der Budget-Posten.
/// </summary>
public interface IBudgetService
{
    /// <summary>Liefert alle Budget-Posten.</summary>
    Task<List<BudgetItem>> GetAllAsync(bool forceRefresh = false);

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    bool HasCache { get; }

    /// <summary>Liefert einen Budget-Posten anhand seiner Id oder null.</summary>
    Task<BudgetItem?> GetByIdAsync(Guid id);

    /// <summary>Fügt einen neuen Budget-Posten hinzu.</summary>
    Task AddAsync(BudgetItem item);

    /// <summary>Aktualisiert einen bestehenden Budget-Posten.</summary>
    Task UpdateAsync(BudgetItem item);

    /// <summary>Löscht einen Budget-Posten anhand seiner Id.</summary>
    Task DeleteAsync(Guid id);
}
