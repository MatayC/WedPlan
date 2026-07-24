using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Service für die Verwaltung der Gästeliste.
/// Abstrahiert die Datenhaltung, damit später einfach auf eine DB umgestellt werden kann.
/// </summary>
public interface IGuestService
{
    /// <summary>Liefert alle Gäste.</summary>
    Task<List<Guest>> GetAllAsync(bool forceRefresh = false);

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    bool HasCache { get; }

    /// <summary>Liefert einen Gast anhand seiner Id oder null.</summary>
    Task<Guest?> GetByIdAsync(Guid id);

    /// <summary>Fügt einen neuen Gast hinzu.</summary>
    Task AddAsync(Guest guest);

    /// <summary>Aktualisiert einen bestehenden Gast.</summary>
    Task UpdateAsync(Guest guest);

    /// <summary>Löscht einen Gast anhand seiner Id.</summary>
    Task DeleteAsync(Guid id);
}
