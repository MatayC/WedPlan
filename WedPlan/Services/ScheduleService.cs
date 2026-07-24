using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="IScheduleService"/>.
/// An die aktive Hochzeit (WeddingContext) gebunden.
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private List<ScheduleItem>? _cache;
    private Guid? _cacheFor;

    public ScheduleService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    public bool HasCache => _cache is not null && _cacheFor == _context.WeddingId;

    public void InvalidateCache() => _cache = null;

    public async Task<List<ScheduleItem>> GetAllAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<ScheduleItem>();
        }

        if (!forceRefresh && HasCache)
        {
            return _cache!;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<ScheduleItemRow>()
            .Where(s => s.WeddingId == _context.WeddingId.Value)
            .Get();

        // Programmpunkte chronologisch sortiert zurückgeben.
        _cache = result.Models.Select(r => r.ToModel()).OrderBy(s => s.StartTime).ToList();
        _cacheFor = _context.WeddingId;
        return _cache;
    }

    public async Task<ScheduleItem?> GetByIdAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        var result = await client
            .From<ScheduleItemRow>()
            .Where(s => s.Id == id)
            .Get();

        return result.Models.FirstOrDefault()?.ToModel();
    }

    public async Task AddAsync(ScheduleItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<ScheduleItemRow>().Insert(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task UpdateAsync(ScheduleItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<ScheduleItemRow>().Update(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<ScheduleItemRow>()
            .Where(s => s.Id == id)
            .Delete();
        InvalidateCache();
    }
}
