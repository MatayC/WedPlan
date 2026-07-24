using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="IGuestService"/>.
/// Alle Abfragen sind an die aktive Hochzeit (WeddingContext) gebunden;
/// RLS in der Datenbank stellt zusätzlich sicher, dass nur eigene Daten sichtbar sind.
/// </summary>
public class GuestService : IGuestService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    public GuestService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    public async Task<List<Guest>> GetAllAsync()
    {
        if (_context.WeddingId is null)
        {
            return new List<Guest>();
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<GuestRow>()
            .Where(g => g.WeddingId == _context.WeddingId.Value)
            .Get();

        return result.Models.Select(r => r.ToModel()).ToList();
    }

    public async Task<Guest?> GetByIdAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        var result = await client
            .From<GuestRow>()
            .Where(g => g.Id == id)
            .Get();

        return result.Models.FirstOrDefault()?.ToModel();
    }

    public async Task AddAsync(Guest guest)
    {
        var client = await _provider.GetClientAsync();
        await client.From<GuestRow>().Insert(guest.ToRow(WeddingId));
    }

    public async Task UpdateAsync(Guest guest)
    {
        var client = await _provider.GetClientAsync();
        await client.From<GuestRow>().Update(guest.ToRow(WeddingId));
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<GuestRow>()
            .Where(g => g.Id == id)
            .Delete();
    }
}
