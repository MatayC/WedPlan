using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Abonniert per Supabase Realtime Änderungen an allen Tabellen der aktiven
/// Hochzeit (Einstellungen, Gäste, Budget, Aufgaben, Zeitplan, Sitzordnung,
/// Wohnung) und meldet sie an die UI, damit alle Mitglieder Änderungen sofort
/// ohne manuelles Neuladen sehen.
/// </summary>
public class WeddingRealtimeService : IAsyncDisposable
{
    private readonly SupabaseClientProvider _provider;
    private readonly WeddingContext _context;

    private readonly List<Supabase.Realtime.RealtimeChannel> _channels = new();
    private Guid? _subscribedWedding;

    // Tabellen, deren Änderungen live überwacht werden. Der Schlüssel ist zugleich
    // der Bezeichner, der über OnDataChanged an die UI gemeldet wird.
    private static readonly string[] DataTables =
    {
        "guests",
        "budget_items",
        "apartment_items",
        "tasks",
        "schedule_items",
        "seating_tables"
    };

    public WeddingRealtimeService(SupabaseClientProvider provider, WeddingContext context)
    {
        _provider = provider;
        _context = context;
    }

    /// <summary>Wird ausgelöst, wenn sich die Einstellungen der aktiven Hochzeit geändert haben.</summary>
    public event Action? OnSettingsChanged;

    /// <summary>
    /// Wird ausgelöst, wenn sich Daten einer überwachten Tabelle der aktiven
    /// Hochzeit geändert haben. Parameter ist der Tabellenname (z.B. "guests").
    /// </summary>
    public event Action<string>? OnDataChanged;

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

        if (_subscribedWedding == weddingId && _channels.Count > 0)
        {
            return;
        }

        await StopAsync();

        try
        {
            var client = await _provider.GetClientAsync();
            await client.Realtime.ConnectAsync();

            // Einstellungen (Titelbild, Dark Mode, Sparplan …)
            var settingsChannel = client.Realtime.Channel("realtime", "public", "wedding_settings");
            settingsChannel.AddPostgresChangeHandler(
                Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.All,
                (_, change) =>
                {
                    var model = change.Model<WeddingSettingsRow>();
                    if (model is null || model.WeddingId == _context.WeddingId)
                    {
                        OnSettingsChanged?.Invoke();
                    }
                });
            await settingsChannel.Subscribe();
            _channels.Add(settingsChannel);

            // Alle übrigen Datentabellen – bei Änderung wird der Tabellenname gemeldet,
            // damit die UI gezielt den passenden Cache invalidiert und neu lädt.
            foreach (var table in DataTables)
            {
                var channel = client.Realtime.Channel("realtime", "public", table);
                var tableName = table;
                channel.AddPostgresChangeHandler(
                    Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.All,
                    (_, _) => OnDataChanged?.Invoke(tableName));
                await channel.Subscribe();
                _channels.Add(channel);
            }

            _subscribedWedding = weddingId;
        }
        catch
        {
            // Realtime ist optional – bei Verbindungsproblemen bleibt der Reload-Fallback.
        }
    }

    /// <summary>Beendet alle aktuellen Abonnements.</summary>
    public async Task StopAsync()
    {
        foreach (var channel in _channels)
        {
            try
            {
                channel.Unsubscribe();
            }
            catch
            {
                // ignorieren
            }
        }

        _channels.Clear();
        _subscribedWedding = null;

        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}
