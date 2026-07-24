using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="IApartmentService"/>.
/// An die aktive Hochzeit (WeddingContext) gebunden.
/// </summary>
public class ApartmentService : IApartmentService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private List<ApartmentItem>? _cache;
    private Guid? _cacheFor;

    public ApartmentService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    public bool HasCache => _cache is not null && _cacheFor == _context.WeddingId;

    private void InvalidateCache() => _cache = null;

    public async Task<List<ApartmentItem>> GetAllAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<ApartmentItem>();
        }

        if (!forceRefresh && HasCache)
        {
            return _cache!;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<ApartmentItemRow>()
            .Where(a => a.WeddingId == _context.WeddingId.Value)
            .Get();

        _cache = result.Models.Select(r => r.ToModel()).ToList();
        _cacheFor = _context.WeddingId;
        return _cache;
    }

    public async Task<ApartmentItem?> GetByIdAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        var result = await client
            .From<ApartmentItemRow>()
            .Where(a => a.Id == id)
            .Get();

        return result.Models.FirstOrDefault()?.ToModel();
    }

    public async Task AddAsync(ApartmentItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<ApartmentItemRow>().Insert(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task UpdateAsync(ApartmentItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<ApartmentItemRow>().Update(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<ApartmentItemRow>()
            .Where(a => a.Id == id)
            .Delete();
        InvalidateCache();
    }
}
