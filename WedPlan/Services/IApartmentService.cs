using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Service für die Verwaltung der Wohnungsplanungs-Posten.
/// </summary>
public interface IApartmentService
{
    /// <summary>Liefert alle Wohnungs-Posten.</summary>
    Task<List<ApartmentItem>> GetAllAsync(bool forceRefresh = false);

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    bool HasCache { get; }

    /// <summary>Verwirft den Cache, sodass die Daten neu aus der Cloud geladen werden.</summary>
    void InvalidateCache();

    /// <summary>Liefert einen Wohnungs-Posten anhand seiner Id oder null.</summary>
    Task<ApartmentItem?> GetByIdAsync(Guid id);

    /// <summary>Fügt einen neuen Wohnungs-Posten hinzu.</summary>
    Task AddAsync(ApartmentItem item);

    /// <summary>Aktualisiert einen bestehenden Wohnungs-Posten.</summary>
    Task UpdateAsync(ApartmentItem item);

    /// <summary>Löscht einen Wohnungs-Posten anhand seiner Id.</summary>
    Task DeleteAsync(Guid id);
}
