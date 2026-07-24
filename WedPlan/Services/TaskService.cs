using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="ITaskService"/>.
/// An die aktive Hochzeit (WeddingContext) gebunden.
/// </summary>
public class TaskService : ITaskService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private List<TaskItem>? _cache;
    private Guid? _cacheFor;

    public TaskService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    /// <summary>Sind bereits Daten für die aktive Hochzeit im Cache?</summary>
    public bool HasCache => _cache is not null && _cacheFor == _context.WeddingId;

    private void InvalidateCache() => _cache = null;

    public async Task<List<TaskItem>> GetAllAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<TaskItem>();
        }

        if (!forceRefresh && HasCache)
        {
            return _cache!;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<TaskItemRow>()
            .Where(t => t.WeddingId == _context.WeddingId.Value)
            .Get();

        _cache = result.Models.Select(r => r.ToModel()).ToList();
        _cacheFor = _context.WeddingId;
        return _cache;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        var result = await client
            .From<TaskItemRow>()
            .Where(t => t.Id == id)
            .Get();

        return result.Models.FirstOrDefault()?.ToModel();
    }

    public async Task AddAsync(TaskItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<TaskItemRow>().Insert(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task UpdateAsync(TaskItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<TaskItemRow>().Update(item.ToRow(WeddingId));
        InvalidateCache();
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<TaskItemRow>()
            .Where(t => t.Id == id)
            .Delete();
        InvalidateCache();
    }
}
