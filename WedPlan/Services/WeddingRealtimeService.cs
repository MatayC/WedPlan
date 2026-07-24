using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Abonniert per Supabase Realtime Änderungen an der Einstellungs-Zeile der
/// aktiven Hochzeit (z.B. neues Titelbild) und meldet sie an die UI, damit
/// alle Mitglieder Änderungen sofort ohne manuelles Neuladen sehen.
/// </summary>
public class WeddingRealtimeService : IAsyncDisposable
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private Supabase.Realtime.RealtimeChannel? _channel;
    private Guid? _subscribedWedding;

    public WeddingRealtimeService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    /// <summary>Wird ausgelöst, wenn sich die Einstellungen der aktiven Hochzeit geändert haben.</summary>
    public event Action? OnSettingsChanged;

    /// <summary>
    /// Startet (oder erneuert) das Abonnement für die aktuell aktive Hochzeit.
    /// Mehrfachaufrufe sind sicher – ein bestehendes Abo wird nur bei Wechsel erneuert.
    /// </summary>
    public async Task StartAsync()
    {
        var weddingId = _context.WeddingId;
        if (weddingId is null)
        {
            return;
        }

        if (_subscribedWedding == weddingId && _channel is not null)
        {
            return;
        }

        await StopAsync();

        try
        {
            var client = await _provider.GetClientAsync();
            await client.Realtime.ConnectAsync();

            var channel = client.Realtime.Channel("realtime", "public", "wedding_settings");

            channel.AddPostgresChangeHandler(
                Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.All,
                (_, change) =>
                {
                    // Nur auf Änderungen der aktiven Hochzeit reagieren.
                    var model = change.Model<WeddingSettingsRow>();
                    if (model is null || model.WeddingId == _context.WeddingId)
                    {
                        OnSettingsChanged?.Invoke();
                    }
                });

            await channel.Subscribe();
            _channel = channel;
            _subscribedWedding = weddingId;
        }
        catch
        {
            // Realtime ist optional – bei Verbindungsproblemen bleibt der Reload-Fallback.
        }
    }

    /// <summary>Beendet das aktuelle Abonnement.</summary>
    public async Task StopAsync()
    {
        if (_channel is not null)
        {
            try
            {
                _channel.Unsubscribe();
            }
            catch
            {
                // ignorieren
            }

            _channel = null;
            _subscribedWedding = null;
        }

        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}
