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

    public TaskService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    public async Task<List<TaskItem>> GetAllAsync()
    {
        if (_context.WeddingId is null)
        {
            return new List<TaskItem>();
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<TaskItemRow>()
            .Where(t => t.WeddingId == _context.WeddingId.Value)
            .Get();

        return result.Models.Select(r => r.ToModel()).ToList();
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
    }

    public async Task UpdateAsync(TaskItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<TaskItemRow>().Update(item.ToRow(WeddingId));
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<TaskItemRow>()
            .Where(t => t.Id == id)
            .Delete();
    }
}
