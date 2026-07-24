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

    public ScheduleService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    public async Task<List<ScheduleItem>> GetAllAsync()
    {
        if (_context.WeddingId is null)
        {
            return new List<ScheduleItem>();
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<ScheduleItemRow>()
            .Where(s => s.WeddingId == _context.WeddingId.Value)
            .Get();

        // Programmpunkte chronologisch sortiert zurückgeben.
        return result.Models.Select(r => r.ToModel()).OrderBy(s => s.StartTime).ToList();
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
    }

    public async Task UpdateAsync(ScheduleItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<ScheduleItemRow>().Update(item.ToRow(WeddingId));
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<ScheduleItemRow>()
            .Where(s => s.Id == id)
            .Delete();
    }
}
