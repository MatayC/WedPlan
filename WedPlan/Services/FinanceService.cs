using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung von <see cref="IFinanceService"/>.
/// Alle Abfragen sind an die aktive Hochzeit (WeddingContext) gebunden;
/// RLS in der Datenbank stellt zusätzlich sicher, dass nur Mitglieder Zugriff haben.
/// </summary>
public class FinanceService : IFinanceService
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private List<FinanceEntry>? _entryCache;
    private List<FinanceBalance>? _balanceCache;
    private Guid? _cacheFor;

    public FinanceService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    private Guid WeddingId => _context.WeddingId
        ?? throw new InvalidOperationException("Keine aktive Hochzeit ausgewählt.");

    public bool HasCache =>
        _entryCache is not null && _balanceCache is not null && _cacheFor == _context.WeddingId;

    public void InvalidateCache()
    {
        _entryCache = null;
        _balanceCache = null;
    }

    // ----- Einträge ---------------------------------------------------------

    public async Task<List<FinanceEntry>> GetEntriesAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<FinanceEntry>();
        }

        if (!forceRefresh && _entryCache is not null && _cacheFor == _context.WeddingId)
        {
            return _entryCache;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<FinanceEntryRow>()
            .Where(e => e.WeddingId == _context.WeddingId.Value)
            .Get();

        _entryCache = result.Models.Select(r => r.ToModel()).ToList();
        _cacheFor = _context.WeddingId;
        return _entryCache;
    }

    public async Task AddEntryAsync(FinanceEntry entry)
    {
        var client = await _provider.GetClientAsync();
        await client.From<FinanceEntryRow>().Insert(entry.ToRow(WeddingId));
        _entryCache = null;
    }

    public async Task UpdateEntryAsync(FinanceEntry entry)
    {
        var client = await _provider.GetClientAsync();
        await client.From<FinanceEntryRow>().Update(entry.ToRow(WeddingId));
        _entryCache = null;
    }

    public async Task DeleteEntryAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<FinanceEntryRow>()
            .Where(e => e.Id == id)
            .Delete();
        _entryCache = null;
    }

    // ----- Kontostände ------------------------------------------------------

    public async Task<List<FinanceBalance>> GetBalancesAsync(bool forceRefresh = false)
    {
        if (_context.WeddingId is null)
        {
            return new List<FinanceBalance>();
        }

        if (!forceRefresh && _balanceCache is not null && _cacheFor == _context.WeddingId)
        {
            return _balanceCache;
        }

        var client = await _provider.GetClientAsync();
        var result = await client
            .From<FinanceBalanceRow>()
            .Where(b => b.WeddingId == _context.WeddingId.Value)
            .Get();

        _balanceCache = result.Models
            .Select(r => r.ToModel())
            .OrderBy(b => b.Month)
            .ToList();
        _cacheFor = _context.WeddingId;
        return _balanceCache;
    }

    public async Task SaveBalanceAsync(FinanceBalance balance)
    {
        var client = await _provider.GetClientAsync();
        var month = new DateTime(balance.Month.Year, balance.Month.Month, 1);

        // Existiert bereits ein Kontostand für diese Person + diesen Monat?
        var existing = await client
            .From<FinanceBalanceRow>()
            .Where(b => b.WeddingId == WeddingId
                     && b.Person == (int)balance.Person
                     && b.Month == month)
            .Get();

        var current = existing.Models.FirstOrDefault();
        if (current is not null)
        {
            balance.Id = current.Id;
            await client.From<FinanceBalanceRow>().Update(balance.ToRow(WeddingId));
        }
        else
        {
            await client.From<FinanceBalanceRow>().Insert(balance.ToRow(WeddingId));
        }
        _balanceCache = null;
    }

    public async Task DeleteBalanceAsync(Guid id)
    {
        var client = await _provider.GetClientAsync();
        await client
            .From<FinanceBalanceRow>()
            .Where(b => b.Id == id)
            .Delete();
        _balanceCache = null;
    }
}
