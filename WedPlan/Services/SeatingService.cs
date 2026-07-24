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

    public SeatingService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    public async Task<List<SeatingTable>> GetAllAsync()
    {
        if (_context.WeddingId is null)
        {
            return new List<SeatingTable>();
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<SeatingTableRow>()
            .Where(t => t.WeddingId == _context.WeddingId.Value)
            .Get();

        return result.Models.Select(r => r.ToModel()).ToList();
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
    }

    public async Task UpdateAsync(SeatingTable table)
    {
        var client = await _provider.GetClientAsync();
        await client.From<SeatingTableRow>().Update(table.ToRow(WeddingId));
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<SeatingTableRow>()
            .Where(t => t.Id == id)
            .Delete();
    }
}
