using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="IGuestService"/>.
/// Alle Abfragen sind an die aktive Hochzeit (WeddingContext) gebunden;
/// RLS in der Datenbank stellt zusätzlich sicher, dass nur eigene Daten sichtbar sind.
/// </summary>
public class GuestService : IGuestService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private List<Guest>? _cache;
    private Guid? _cacheFor;

    public GuestService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    public bool HasCache => _cache is not null && _cacheFor == _context.WeddingId;

    public void InvalidateCache() => _cache = null;

    public async Task<List<Guest>> GetAllAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<Guest>();
        }

        if (!forceRefresh && HasCache)
        {
            return _cache!;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<GuestRow>()
            .Where(g => g.WeddingId == _context.WeddingId.Value)
            .Get();

        _cache = result.Models.Select(r => r.ToModel()).ToList();
        _cacheFor = _context.WeddingId;
        return _cache;
    }

    public async Task<Guest?> GetByIdAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        var result = await client
            .From<GuestRow>()
            .Where(g => g.Id == id)
            .Get();

        return result.Models.FirstOrDefault()?.ToModel();
    }

    public async Task AddAsync(Guest guest)
    {
        var client = await _provider.GetClientAsync();
        await client.From<GuestRow>().Insert(guest.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task UpdateAsync(Guest guest)
    {
        var client = await _provider.GetClientAsync();
        await client.From<GuestRow>().Update(guest.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<GuestRow>()
            .Where(g => g.Id == id)
            .Delete();
        InvalidateCache();
    }
}
