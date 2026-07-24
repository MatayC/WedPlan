using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="IBudgetService"/>.
/// An die aktive Hochzeit (WeddingContext) gebunden.
/// </summary>
public class BudgetService : IBudgetService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private List<BudgetItem>? _cache;
    private Guid? _cacheFor;

    public BudgetService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    public bool HasCache => _cache is not null && _cacheFor == _context.WeddingId;

    public void InvalidateCache() => _cache = null;

    public async Task<List<BudgetItem>> GetAllAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<BudgetItem>();
        }

        // Cache liefert sofort – kein sichtbarer Ladevorgang bei erneutem Besuch.
        if (!forceRefresh && HasCache)
        {
            return _cache!;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<BudgetItemRow>()
            .Where(b => b.WeddingId == _context.WeddingId.Value)
            .Get();

        _cache = result.Models.Select(r => r.ToModel()).ToList();
        _cacheFor = _context.WeddingId;
        return _cache;
    }

    public async Task<BudgetItem?> GetByIdAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        var result = await client
            .From<BudgetItemRow>()
            .Where(b => b.Id == id)
            .Get();

        return result.Models.FirstOrDefault()?.ToModel();
    }

    public async Task AddAsync(BudgetItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<BudgetItemRow>().Insert(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task UpdateAsync(BudgetItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<BudgetItemRow>().Update(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<BudgetItemRow>()
            .Where(b => b.Id == id)
            .Delete();
        InvalidateCache();
    }
}
