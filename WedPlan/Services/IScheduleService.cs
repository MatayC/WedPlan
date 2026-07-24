using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Service für die Verwaltung des Zeitplans/der Timeline.
/// </summary>
public interface IScheduleService
{
    /// <summary>Liefert alle Programmpunkte (nach Startzeit sortiert).</summary>
    Task<List<ScheduleItem>> GetAllAsync(bool forceRefresh = false);

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    bool HasCache { get; }

    /// <summary>Verwirft den Cache, sodass die Daten neu aus der Cloud geladen werden.</summary>
    void InvalidateCache();

    /// <summary>Liefert einen Programmpunkt anhand seiner Id oder null.</summary>
    Task<ScheduleItem?> GetByIdAsync(Guid id);

    /// <summary>Fügt einen neuen Programmpunkt hinzu.</summary>
    Task AddAsync(ScheduleItem item);

    /// <summary>Aktualisiert einen bestehenden Programmpunkt.</summary>
    Task UpdateAsync(ScheduleItem item);

    /// <summary>Löscht einen Programmpunkt anhand seiner Id.</summary>
    Task DeleteAsync(Guid id);
}
