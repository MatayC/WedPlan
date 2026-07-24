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

    public BudgetService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    public async Task<List<BudgetItem>> GetAllAsync()
    {
        if (_context.WeddingId is null)
        {
            return new List<BudgetItem>();
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<BudgetItemRow>()
            .Where(b => b.WeddingId == _context.WeddingId.Value)
            .Get();

        return result.Models.Select(r => r.ToModel()).ToList();
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
    }

    public async Task UpdateAsync(BudgetItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<BudgetItemRow>().Update(item.ToRow(WeddingId));
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<BudgetItemRow>()
            .Where(b => b.Id == id)
            .Delete();
    }
}
