using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Service für die Verwaltung der Sitzordnung (Tische und Plätze).
/// </summary>
public interface ISeatingService
{
    /// <summary>Liefert alle Tische.</summary>
    Task<List<SeatingTable>> GetAllAsync();

    /// <summary>Liefert einen Tisch anhand seiner Id oder null.</summary>
    Task<SeatingTable?> GetByIdAsync(Guid id);

    /// <summary>Fügt einen neuen Tisch hinzu.</summary>
    Task AddAsync(SeatingTable table);

    /// <summary>Aktualisiert einen bestehenden Tisch.</summary>
    Task UpdateAsync(SeatingTable table);

    /// <summary>Löscht einen Tisch anhand seiner Id.</summary>
    Task DeleteAsync(Guid id);
}
