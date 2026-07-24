using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Service für die Verwaltung der Aufgaben/Checkliste.
/// </summary>
public interface ITaskService
{
    /// <summary>Liefert alle Aufgaben.</summary>
    Task<List<TaskItem>> GetAllAsync();

    /// <summary>Liefert eine Aufgabe anhand ihrer Id oder null.</summary>
    Task<TaskItem?> GetByIdAsync(Guid id);

    /// <summary>Fügt eine neue Aufgabe hinzu.</summary>
    Task AddAsync(TaskItem item);

    /// <summary>Aktualisiert eine bestehende Aufgabe.</summary>
    Task UpdateAsync(TaskItem item);

    /// <summary>Löscht eine Aufgabe anhand ihrer Id.</summary>
    Task DeleteAsync(Guid id);
}
