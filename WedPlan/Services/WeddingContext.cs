using Blazored.LocalStorage;

namespace WedPlan.Services;

/// <summary>
/// Hält die aktuell aktive Hochzeit (wedding_id) des eingeloggten Benutzers
/// für die Dauer des Blazor-Circuits (Scoped). Alle Daten-Services fragen hier
/// die aktive Hochzeit ab, um die richtigen Cloud-Daten zu laden/speichern.
/// Die zuletzt gewählte Hochzeit wird im Browser gemerkt (LocalStorage).
/// </summary>
public class WeddingContext
{
    private const string ActiveWeddingKey = "wedplan_active_wedding";

    private readonly ILocalStorageService _localStorage;

    public WeddingContext(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>Die aktive Hochzeit oder null, wenn der Benutzer (noch) keiner angehört.</summary>
    public Guid? WeddingId { get; private set; }

    /// <summary>Anzeigename der aktiven Hochzeit.</summary>
    public string? WeddingName { get; private set; }

    /// <summary>Einladungs-Code der aktiven Hochzeit (zum Teilen).</summary>
    public string? InviteCode { get; private set; }

    /// <summary>Gibt an, ob eine aktive Hochzeit gesetzt ist.</summary>
    public bool HasWedding => WeddingId is not null;

    /// <summary>Wird ausgelöst, wenn sich die aktive Hochzeit ändert.</summary>
    public event Action? OnChange;

    /// <summary>Setzt die aktive Hochzeit.</summary>
    public void Set(Guid weddingId, string? name, string? inviteCode)
    {
        WeddingId = weddingId;
        WeddingName = name;
        InviteCode = inviteCode;
        OnChange?.Invoke();
    }

    /// <summary>Löscht die aktive Hochzeit (z.B. beim Logout).</summary>
    public void Clear()
    {
        WeddingId = null;
        WeddingName = null;
        InviteCode = null;
        OnChange?.Invoke();
    }

    /// <summary>Merkt sich die gewählte Hochzeit im Browser (LocalStorage).</summary>
    public async Task PersistActiveAsync()
    {
        await PersistActiveAsync(WeddingId);
    }

    /// <summary>Merkt sich eine bestimmte Hochzeits-Id im Browser (LocalStorage).</summary>
    public async Task PersistActiveAsync(Guid? weddingId)
    {
        if (weddingId is null)
        {
            await _localStorage.RemoveItemAsync(ActiveWeddingKey);
        }
        else
        {
            await _localStorage.SetItemAsStringAsync(ActiveWeddingKey, weddingId.Value.ToString());
        }
    }

    /// <summary>Liest die zuletzt gemerkte Hochzeits-Id aus dem Browser (oder null).</summary>
    public async Task<Guid?> GetStoredWeddingIdAsync()
    {
        var value = await _localStorage.GetItemAsStringAsync(ActiveWeddingKey);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
