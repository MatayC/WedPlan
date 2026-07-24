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

    public ApartmentService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    public async Task<List<ApartmentItem>> GetAllAsync()
    {
        if (_context.WeddingId is null)
        {
            return new List<ApartmentItem>();
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<ApartmentItemRow>()
            .Where(a => a.WeddingId == _context.WeddingId.Value)
            .Get();

        return result.Models.Select(r => r.ToModel()).ToList();
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
    }

    public async Task UpdateAsync(ApartmentItem item)
    {
        var client = await _provider.GetClientAsync();
        await client.From<ApartmentItemRow>().Update(item.ToRow(WeddingId));
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<ApartmentItemRow>()
            .Where(a => a.Id == id)
            .Delete();
    }
}
