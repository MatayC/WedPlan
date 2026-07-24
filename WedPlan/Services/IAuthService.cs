using Supabase.Gotrue;

namespace WedPlan.Services;

/// <summary>
/// Ergebnis eines Auth-Vorgangs (Login/Registrierung).
/// </summary>
/// <param name="Success">Ob der Vorgang erfolgreich war.</param>
/// <param name="ErrorMessage">Fehlermeldung (falls nicht erfolgreich).</param>
/// <param name="NeedsEmailConfirmation">
/// True, wenn die Registrierung erfolgreich war, aber der Benutzer noch seine
/// E-Mail bestätigen muss, bevor er sich anmelden kann.
/// </param>
public record AuthResult(bool Success, string? ErrorMessage = null, bool NeedsEmailConfirmation = false);

/// <summary>
/// Service für Authentifizierung via Supabase (E-Mail + Passwort).
/// </summary>
public interface IAuthService
{
    /// <summary>Wird ausgelöst, wenn sich der Anmeldestatus ändert.</summary>
    event Action? OnAuthStateChanged;

    /// <summary>Gibt an, ob aktuell ein Benutzer angemeldet ist.</summary>
    Task<bool> IsLoggedInAsync();

    /// <summary>Liefert den aktuell angemeldeten Benutzer (oder null).</summary>
    Task<User?> GetCurrentUserAsync();

    /// <summary>Liefert die Id des aktuell angemeldeten Benutzers (oder null).</summary>
    Task<Guid?> GetCurrentUserIdAsync();

    /// <summary>Registriert einen neuen Benutzer mit Nutzername, E-Mail und Passwort.</summary>
    Task<AuthResult> RegisterAsync(string username, string email, string password);

    /// <summary>Meldet einen Benutzer mit E-Mail und Passwort an.</summary>
    Task<AuthResult> LoginAsync(string email, string password);

    /// <summary>Sendet eine E-Mail zum Zurücksetzen des Passworts.</summary>
    Task<AuthResult> SendPasswordResetAsync(string email);

    /// <summary>Setzt das Passwort des aktuell (per Reset-Link) angemeldeten Benutzers neu.</summary>
    Task<AuthResult> UpdatePasswordAsync(string newPassword);

    /// <summary>Meldet den aktuellen Benutzer ab.</summary>
    Task LogoutAsync();
}
