using Supabase;

namespace WedPlan.Services;

/// <summary>
/// Stellt einen pro Benutzer-Circuit (Scoped) initialisierten Supabase-Client bereit.
/// Der Client wird beim ersten Zugriff einmalig initialisiert und stellt dabei
/// eine ggf. im Browser gespeicherte Anmelde-Session wieder her.
/// </summary>
public class SupabaseClientProvider
{
    private readonly string _url;
    private readonly string _key;
    private readonly SupabaseSessionPersistence _persistence;
    private Client? _client;
    private bool _initialized;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public SupabaseClientProvider(IConfiguration configuration, SupabaseSessionPersistence persistence)
    {
        _url = configuration["Supabase:Url"]
            ?? Environment.GetEnvironmentVariable("SUPABASE_URL")
            ?? throw new InvalidOperationException("Supabase:Url ist nicht konfiguriert (User Secrets/appsettings/Umgebungsvariable SUPABASE_URL).");
        _key = configuration["Supabase:Key"]
            ?? Environment.GetEnvironmentVariable("SUPABASE_KEY")
            ?? throw new InvalidOperationException("Supabase:Key ist nicht konfiguriert (User Secrets/appsettings/Umgebungsvariable SUPABASE_KEY).");
        _persistence = persistence;
    }

    /// <summary>
    /// Liefert den initialisierten Supabase-Client. Beim ersten Aufruf wird
    /// die Verbindung hergestellt und eine ggf. gespeicherte Session geladen.
    /// </summary>
    public async Task<Client> GetClientAsync()
    {
        if (_initialized && _client is not null)
        {
            return _client;
        }

        await _gate.WaitAsync();
        try
        {
            if (_initialized && _client is not null)
            {
                return _client;
            }

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false,
                AutoRefreshToken = true
            };

            _client = new Client(_url, _key, options);
            await _client.InitializeAsync();

            // Gespeicherte Anmelde-Session wiederherstellen (falls vorhanden).
            var tokens = await _persistence.LoadAsync();
            if (tokens is not null)
            {
                try
                {
                    await _client.Auth.SetSession(tokens.Value.AccessToken, tokens.Value.RefreshToken);
                }
                catch
                {
                    // Ungültige/abgelaufene Tokens: verwerfen, Benutzer muss sich neu anmelden.
                    await _persistence.ClearAsync();
                }
            }

            _initialized = true;
            return _client;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Speichert die aktuelle Session nach erfolgreichem Login/Registrierung.</summary>
    public async Task PersistCurrentSessionAsync()
    {
        if (_client?.Auth.CurrentSession is { } session
            && !string.IsNullOrEmpty(session.AccessToken)
            && !string.IsNullOrEmpty(session.RefreshToken))
        {
            await _persistence.SaveAsync(session.AccessToken, session.RefreshToken);
        }
    }

    /// <summary>Entfernt die gespeicherte Session (beim Logout).</summary>
    public async Task ClearPersistedSessionAsync()
    {
        await _persistence.ClearAsync();
    }
}
