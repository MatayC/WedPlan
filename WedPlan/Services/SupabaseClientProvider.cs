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
        var url = configuration["Supabase:Url"]
            ?? Environment.GetEnvironmentVariable("SUPABASE_URL")
            ?? throw new InvalidOperationException("Supabase:Url ist nicht konfiguriert (User Secrets/appsettings/Umgebungsvariable SUPABASE_URL).");
        var key = configuration["Supabase:Key"]
            ?? Environment.GetEnvironmentVariable("SUPABASE_KEY")
            ?? throw new InvalidOperationException("Supabase:Key ist nicht konfiguriert (User Secrets/appsettings/Umgebungsvariable SUPABASE_KEY).");

        _url = NormalizeUrl(url);
        _key = key.Trim();
        _persistence = persistence;
    }

    /// <summary>
    /// Bereinigt die Supabase-URL: entfernt Leerzeichen/Zeilenumbrüche, ergänzt bei
    /// Bedarf das https-Schema und entfernt einen abschließenden Schrägstrich.
    /// So werden typische Konfigurationsfehler (Invalid URI) abgefangen.
    /// </summary>
    private static string NormalizeUrl(string raw)
    {
        var url = raw.Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException("Supabase:Url ist leer. Bitte in Render unter Environment 'SUPABASE_URL' setzen (Format: https://xxxxx.supabase.co).");
        }

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            url = "https://" + url;
        }

        url = url.TrimEnd('/');

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException($"Supabase:Url '{raw}' ist keine gültige URL. Erwartet wird z.B. https://xxxxx.supabase.co.");
        }

        return url;
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
