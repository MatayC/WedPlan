using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-basierte Implementierung der Authentifizierung (E-Mail + Passwort).
/// </summary>
public class AuthService : IAuthService
{
    private readonly SupabaseClientProvider _provider;

    public AuthService(SupabaseClientProvider provider)
    {
        _provider = provider;
    }

    public event Action? OnAuthStateChanged;

    public async Task<bool> IsLoggedInAsync()
    {
        var client = await _provider.GetClientAsync();
        return client.Auth.CurrentUser is not null;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var client = await _provider.GetClientAsync();
        return client.Auth.CurrentUser;
    }

    public async Task<Guid?> GetCurrentUserIdAsync()
    {
        var client = await _provider.GetClientAsync();
        var id = client.Auth.CurrentUser?.Id;
        return Guid.TryParse(id, out var guid) ? guid : null;
    }

    public async Task<AuthResult> RegisterAsync(string username, string email, string password)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var options = new SignUpOptions
            {
                Data = new Dictionary<string, object> { ["username"] = username }
            };

            var session = await client.Auth.SignUp(email, password, options);

            // Wenn E-Mail-Bestätigung aktiv ist, gibt es einen User, aber keine Session/Token.
            if (session?.User is not null && string.IsNullOrEmpty(session.AccessToken))
            {
                return new AuthResult(true, NeedsEmailConfirmation: true);
            }

            if (session?.User is not null)
            {
                await _provider.PersistCurrentSessionAsync();
                await EnsureProfileAsync(session.User);
                OnAuthStateChanged?.Invoke();
                return new AuthResult(true);
            }

            return new AuthResult(false, "Registrierung fehlgeschlagen. Bitte erneut versuchen.");
        }
        catch (GotrueException ex)
        {
            return new AuthResult(false, TranslateError(ex.Message));
        }
        catch (Exception ex)
        {
            return new AuthResult(false, ex.Message);
        }
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var session = await client.Auth.SignInWithPassword(email, password);
            if (session?.User is not null)
            {
                await _provider.PersistCurrentSessionAsync();
                await EnsureProfileAsync(session.User);
                OnAuthStateChanged?.Invoke();
                return new AuthResult(true);
            }

            return new AuthResult(false, "Anmeldung fehlgeschlagen. Bitte E-Mail und Passwort prüfen.");
        }
        catch (GotrueException ex)
        {
            return new AuthResult(false, TranslateError(ex.Message));
        }
        catch (Exception ex)
        {
            return new AuthResult(false, ex.Message);
        }
    }

    public async Task<AuthResult> SendPasswordResetAsync(string email)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            await client.Auth.ResetPasswordForEmail(email);
            return new AuthResult(true);
        }
        catch (GotrueException ex)
        {
            return new AuthResult(false, TranslateError(ex.Message));
        }
        catch (Exception ex)
        {
            return new AuthResult(false, ex.Message);
        }
    }

    public async Task<AuthResult> UpdatePasswordAsync(string newPassword)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var attrs = new UserAttributes { Password = newPassword };
            var user = await client.Auth.Update(attrs);
            return user is not null
                ? new AuthResult(true)
                : new AuthResult(false, "Passwort konnte nicht geändert werden.");
        }
        catch (GotrueException ex)
        {
            return new AuthResult(false, TranslateError(ex.Message));
        }
        catch (Exception ex)
        {
            return new AuthResult(false, ex.Message);
        }
    }

    public async Task LogoutAsync()
    {
        var client = await _provider.GetClientAsync();
        await client.Auth.SignOut();
        await _provider.ClearPersistedSessionAsync();
        OnAuthStateChanged?.Invoke();
    }

    /// <summary>
    /// Stellt sicher, dass für den angemeldeten Benutzer ein Profil mit Nutzername
    /// existiert. Der Datenbank-Trigger legt das Profil normalerweise beim SignUp an;
    /// dieser Upsert ist ein Sicherheitsnetz, damit niemand als „Unbekannt" erscheint
    /// (z.B. wenn der Trigger fehlt oder der Nutzername leer geblieben ist).
    /// </summary>
    private async Task EnsureProfileAsync(User user)
    {
        try
        {
            if (!Guid.TryParse(user.Id, out var userId))
            {
                return;
            }

            // Nutzername bevorzugt aus den user_metadata, sonst aus dem E-Mail-Präfix.
            var username = user.UserMetadata is not null
                && user.UserMetadata.TryGetValue("username", out var raw)
                && raw is not null
                && !string.IsNullOrWhiteSpace(raw.ToString())
                    ? raw.ToString()!
                    : (user.Email?.Split('@').FirstOrDefault() ?? "Gast");

            var client = await _provider.GetClientAsync();

            // Existiert bereits ein gültiges Profil? Dann nichts überschreiben.
            var existing = await client
                .From<ProfileRow>()
                .Where(p => p.Id == userId)
                .Get();

            var current = existing.Models.FirstOrDefault();
            if (current is not null && !string.IsNullOrWhiteSpace(current.Username)
                && current.Username != "Unbekannt")
            {
                return;
            }

            await client.From<ProfileRow>().Upsert(new ProfileRow
            {
                Id = userId,
                Username = username,
                Email = user.Email
            });
        }
        catch
        {
            // Profil-Absicherung ist optional; schlägt sie fehl, bleibt der Login gültig.
        }
    }

    private static string TranslateError(string message)
    {
        if (message.Contains("Invalid login credentials", StringComparison.OrdinalIgnoreCase))
        {
            return "E-Mail oder Passwort ist falsch.";
        }
        if (message.Contains("Email not confirmed", StringComparison.OrdinalIgnoreCase))
        {
            return "Bitte bestätige zuerst deine E-Mail-Adresse (Link im Postfach).";
        }
        if (message.Contains("User already registered", StringComparison.OrdinalIgnoreCase))
        {
            return "Diese E-Mail ist bereits registriert. Bitte melde dich an.";
        }
        if (message.Contains("Password should be at least", StringComparison.OrdinalIgnoreCase))
        {
            return "Das Passwort ist zu kurz (mindestens 6 Zeichen).";
        }
        return message;
    }
}
