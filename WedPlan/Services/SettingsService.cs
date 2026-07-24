using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="ISettingsService"/>.
/// Hält genau eine Einstellungs-Zeile pro Hochzeit (wedding_id).
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;
    private WeddingSettings? _cached;
    private Guid? _cachedFor;

    /// <inheritdoc />
    public event Action? OnChange;

    public SettingsService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    public async Task<WeddingSettings> GetAsync()
    {
        // Ohne aktive Hochzeit: Standard-Einstellungen (z.B. während des Onboardings).
        if (_context.WeddingId is null)
        {
            return _cached ??= new WeddingSettings();
        }

        // Cache nur gültig, solange dieselbe Hochzeit aktiv ist.
        if (_cached is not null && _cachedFor == _context.WeddingId)
        {
            return _cached;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<WeddingSettingsRow>()
            .Where(s => s.WeddingId == _context.WeddingId.Value)
            .Get();

        var row = result.Models.FirstOrDefault();
        _cached = row?.ToModel() ?? new WeddingSettings();
        _cachedFor = _context.WeddingId;
        return _cached;
    }

    public async Task SaveAsync(WeddingSettings settings)
    {
        _cached = settings;
        _cachedFor = _context.WeddingId;

        if (_context.WeddingId is not null)
        {
            var client = await _provider.GetClientAsync();
            // Upsert: legt die Zeile an oder aktualisiert sie (PK = wedding_id).
            await client.From<WeddingSettingsRow>().Upsert(settings.ToRow(_context.WeddingId.Value));
        }

        // Abonnenten (z.B. das MainLayout für den Dark Mode) benachrichtigen.
        OnChange?.Invoke();
    }

    public void InvalidateCache()
    {
        _cached = null;
        _cachedFor = null;
    }
}
