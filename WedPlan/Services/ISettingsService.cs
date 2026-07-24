using WedPlan.Models;

namespace WedPlan.Services;

/// <summary>
/// Zentraler Service für die Hochzeits-Einstellungen (Brautpaar, Datum, Location,
/// Budget, Währung, Theme). Wird von allen Seiten genutzt.
/// </summary>
public interface ISettingsService
{
    /// <summary>Wird ausgelöst, wenn sich die Einstellungen ändern (z.B. Dark Mode).</summary>
    event Action? OnChange;

    /// <summary>Liefert die aktuellen Einstellungen (lädt sie bei Bedarf).</summary>
    Task<WeddingSettings> GetAsync();

    /// <summary>Speichert die Einstellungen und benachrichtigt Abonnenten.</summary>
    Task SaveAsync(WeddingSettings settings);

    /// <summary>Verwirft den Cache, sodass die Einstellungen neu aus der Cloud geladen werden.</summary>
    void InvalidateCache();
}
