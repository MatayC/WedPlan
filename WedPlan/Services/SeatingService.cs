using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="ISeatingService"/>.
/// An die aktive Hochzeit (WeddingContext) gebunden.
/// </summary>
public class SeatingService : ISeatingService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private List<SeatingTable>? _cache;
    private Guid? _cacheFor;

    public SeatingService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    public bool HasCache => _cache is not null && _cacheFor == _context.WeddingId;

    public void InvalidateCache() => _cache = null;

    public async Task<List<SeatingTable>> GetAllAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<SeatingTable>();
        }

        if (!forceRefresh && HasCache)
        {
            return _cache!;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<SeatingTableRow>()
            .Where(t => t.WeddingId == _context.WeddingId.Value)
            .Get();

        _cache = result.Models.Select(r => r.ToModel()).ToList();
        _cacheFor = _context.WeddingId;
        return _cache;
    }

    public async Task<SeatingTable?> GetByIdAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        var result = await client
            .From<SeatingTableRow>()
            .Where(t => t.Id == id)
            .Get();

        return result.Models.FirstOrDefault()?.ToModel();
    }

    public async Task AddAsync(SeatingTable table)
    {
        var client = await _provider.GetClientAsync();
        await client.From<SeatingTableRow>().Insert(table.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task UpdateAsync(SeatingTable table)
    {
        var client = await _provider.GetClientAsync();
        await client.From<SeatingTableRow>().Update(table.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<SeatingTableRow>()
            .Where(t => t.Id == id)
            .Delete();
        InvalidateCache();
    }
}
