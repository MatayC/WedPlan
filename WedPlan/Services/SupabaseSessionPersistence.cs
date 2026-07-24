using Blazored.LocalStorage;

namespace WedPlan.Services;

/// <summary>
/// Speichert die Supabase-Anmelde-Session (Access-/Refresh-Token) im Browser-LocalStorage,
/// damit der Benutzer beim erneuten Öffnen der App angemeldet bleibt.
/// Zugriff nur nach dem ersten interaktiven Render (JS-Interop).
/// </summary>
public class SupabaseSessionPersistence
{
    private const string AccessTokenKey = "wedplan_access_token";
    private const string RefreshTokenKey = "wedplan_refresh_token";

    private readonly ILocalStorageService _localStorage;

    public SupabaseSessionPersistence(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>Gespeicherte Tokens laden (oder null, falls keine vorhanden).</summary>
    public async Task<(string AccessToken, string RefreshToken)?> LoadAsync()
    {
        var access = await _localStorage.GetItemAsStringAsync(AccessTokenKey);
        var refresh = await _localStorage.GetItemAsStringAsync(RefreshTokenKey);

        if (string.IsNullOrEmpty(access) || string.IsNullOrEmpty(refresh))
        {
            return null;
        }

        return (access, refresh);
    }

    /// <summary>Tokens speichern.</summary>
    public async Task SaveAsync(string accessToken, string refreshToken)
    {
        await _localStorage.SetItemAsStringAsync(AccessTokenKey, accessToken);
        await _localStorage.SetItemAsStringAsync(RefreshTokenKey, refreshToken);
    }

    /// <summary>Gespeicherte Tokens löschen (z.B. beim Logout).</summary>
    public async Task ClearAsync()
    {
        await _localStorage.RemoveItemAsync(AccessTokenKey);
        await _localStorage.RemoveItemAsync(RefreshTokenKey);
    }
}
