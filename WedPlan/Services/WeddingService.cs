using Supabase.Postgrest;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Supabase-Implementierung von <see cref="IWeddingService"/>.
/// Nutzt die serverseitigen SQL-Funktionen create_wedding und join_wedding_by_code.
/// </summary>
public class WeddingService : IWeddingService
{
    private readonly SupabaseClientProvider _provider;
    private readonly IAuthService _authService;
    private readonly WeddingContext _context;

    public WeddingService(SupabaseClientProvider provider, IAuthService authService, WeddingContext context)
    {
        _provider = provider;
        _authService = authService;
        _context = context;
    }

    public async Task<bool> LoadActiveWeddingAsync()
    {
        var userId = await _authService.GetCurrentUserIdAsync();
        if (userId is null)
        {
            return false;
        }

        var client = await _provider.GetClientAsync();

        // Mitgliedschaft(en) des Benutzers laden.
        var members = await client
            .From<WeddingMemberRow>()
            .Where(m => m.UserId == userId.Value)
            .Get();

        if (members.Models.Count == 0)
        {
            _context.Clear();
            await _context.PersistActiveAsync();
            return false;
        }

        // Zuletzt gemerkte Hochzeit bevorzugen, sonst die erste Mitgliedschaft.
        var storedId = await _context.GetStoredWeddingIdAsync();
        var membership = members.Models.FirstOrDefault(m => m.WeddingId == storedId)
            ?? members.Models.First();

        var weddings = await client
            .From<WeddingRow>()
            .Where(w => w.Id == membership.WeddingId)
            .Get();

        var wedding = weddings.Models.FirstOrDefault();
        if (wedding is null)
        {
            _context.Clear();
            await _context.PersistActiveAsync();
            return false;
        }

        _context.Set(wedding.Id, wedding.Name, wedding.InviteCode);
        await _context.PersistActiveAsync();
        return true;
    }

    public async Task<(bool Success, string? Error)> CreateWeddingAsync(string name)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var response = await client.Rpc("create_wedding", new Dictionary<string, object> { ["wedding_name"] = name });

            // Neu angelegte Hochzeit direkt als aktive Auswahl merken.
            var newId = response.Content?.Trim().Trim('"');
            if (Guid.TryParse(newId, out var createdId))
            {
                await _context.PersistActiveAsync(createdId);
            }

            var loaded = await LoadActiveWeddingAsync();
            return loaded
                ? (true, null)
                : (false, "Hochzeit wurde angelegt, konnte aber nicht geladen werden.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> JoinWeddingAsync(string inviteCode)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var response = await client.Rpc("join_wedding_by_code",
                new Dictionary<string, object> { ["code"] = inviteCode });

            // Die Funktion gibt die wedding_id zurück – oder null bei ungültigem Code.
            var content = response.Content?.Trim().Trim('"');
            if (string.IsNullOrEmpty(content) || content == "null")
            {
                return (false, "Der Einladungs-Code ist ungültig. Bitte prüfe die Eingabe.");
            }

            // Beigetretene Hochzeit als aktive Auswahl merken.
            if (Guid.TryParse(content, out var joinedId))
            {
                await _context.PersistActiveAsync(joinedId);
            }

            var loaded = await LoadActiveWeddingAsync();
            return loaded
                ? (true, null)
                : (false, "Beitritt erfolgreich, aber die Hochzeit konnte nicht geladen werden.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<List<WeddingSummary>> GetMyWeddingsAsync()
    {
        var userId = await _authService.GetCurrentUserIdAsync();
        if (userId is null)
        {
            return new List<WeddingSummary>();
        }

        var client = await _provider.GetClientAsync();

        var members = await client
            .From<WeddingMemberRow>()
            .Where(m => m.UserId == userId.Value)
            .Get();

        var weddingIds = members.Models.Select(m => m.WeddingId).ToList();
        if (weddingIds.Count == 0)
        {
            return new List<WeddingSummary>();
        }

        var weddings = await client
            .From<WeddingRow>()
            .Filter("id", Constants.Operator.In, weddingIds.Select(id => id.ToString()).ToList())
            .Get();

        return weddings.Models
            .Select(w => new WeddingSummary(w.Id, w.Name, w.InviteCode))
            .ToList();
    }

    public async Task<bool> SwitchWeddingAsync(Guid weddingId)
    {
        var userId = await _authService.GetCurrentUserIdAsync();
        if (userId is null)
        {
            return false;
        }

        var client = await _provider.GetClientAsync();

        // Sicherstellen, dass der Benutzer Mitglied der Ziel-Hochzeit ist.
        var members = await client
            .From<WeddingMemberRow>()
            .Where(m => m.UserId == userId.Value && m.WeddingId == weddingId)
            .Get();

        if (members.Models.Count == 0)
        {
            return false;
        }

        var weddings = await client
            .From<WeddingRow>()
            .Where(w => w.Id == weddingId)
            .Get();

        var wedding = weddings.Models.FirstOrDefault();
        if (wedding is null)
        {
            return false;
        }

        _context.Set(wedding.Id, wedding.Name, wedding.InviteCode);
        await _context.PersistActiveAsync();
        return true;
    }

    public async Task<(bool Success, string? Error)> DeleteWeddingAsync(Guid weddingId)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var response = await client.Rpc("delete_wedding",
                new Dictionary<string, object> { ["target_wedding"] = weddingId.ToString() });

            var content = response.Content?.Trim().Trim('"');
            if (!string.Equals(content, "true", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Die Hochzeit konnte nicht gelöscht werden.");
            }

            // War es die aktive Hochzeit? Dann auf eine andere umschalten oder leeren.
            if (_context.WeddingId == weddingId)
            {
                await _context.PersistActiveAsync(null);
                _context.Clear();
                await LoadActiveWeddingAsync();
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<List<WeddingMember>> GetMembersAsync()
    {
        if (_context.WeddingId is null)
        {
            return new List<WeddingMember>();
        }

        var client = await _provider.GetClientAsync();

        var members = await client
            .From<WeddingMemberRow>()
            .Where(m => m.WeddingId == _context.WeddingId.Value)
            .Get();

        var userIds = members.Models.Select(m => m.UserId).ToList();
        if (userIds.Count == 0)
        {
            return new List<WeddingMember>();
        }

        var profiles = await client
            .From<ProfileRow>()
            .Filter("id", Constants.Operator.In, userIds.Select(id => id.ToString()).ToList())
            .Get();

        return members.Models.Select(m =>
        {
            var profile = profiles.Models.FirstOrDefault(p => p.Id == m.UserId);
            return new WeddingMember(m.UserId, profile?.Username ?? "Unbekannt", profile?.Email)
            {
                Permissions = new MemberPermissions
                {
                    IsAdmin = m.Role == "admin",
                    CanEditGuests = m.CanEditGuests,
                    CanEditBudget = m.CanEditBudget,
                    CanEditTasks = m.CanEditTasks,
                    CanEditSchedule = m.CanEditSchedule,
                    CanEditSeating = m.CanEditSeating,
                    CanEditSettings = m.CanEditSettings,
                    CanEditApartment = m.CanEditApartment,
                    CanDeleteProject = m.CanDeleteProject
                }
            };
        }).ToList();
    }

    public async Task<(bool Success, string? Error)> UpdateMemberPermissionsAsync(Guid userId, MemberPermissions permissions)
    {
        if (_context.WeddingId is null)
        {
            return (false, "Keine aktive Hochzeit.");
        }

        try
        {
            var client = await _provider.GetClientAsync();
            var response = await client.Rpc("update_member_permissions", new Dictionary<string, object>
            {
                ["target_wedding"] = _context.WeddingId.Value.ToString(),
                ["target_user"] = userId.ToString(),
                ["new_role"] = permissions.IsAdmin ? "admin" : "member",
                ["edit_guests"] = permissions.CanEditGuests,
                ["edit_budget"] = permissions.CanEditBudget,
                ["edit_tasks"] = permissions.CanEditTasks,
                ["edit_schedule"] = permissions.CanEditSchedule,
                ["edit_seating"] = permissions.CanEditSeating,
                ["edit_settings"] = permissions.CanEditSettings,
                ["edit_apartment"] = permissions.CanEditApartment,
                ["delete_project"] = permissions.CanDeleteProject
            });

            var content = response.Content?.Trim().Trim('"');
            if (!string.Equals(content, "true", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Keine Berechtigung, diese Rechte zu ändern.");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> RemoveMemberAsync(Guid userId)
    {
        if (_context.WeddingId is null)
        {
            return (false, "Keine aktive Hochzeit.");
        }

        try
        {
            var client = await _provider.GetClientAsync();
            var response = await client.Rpc("remove_wedding_member", new Dictionary<string, object>
            {
                ["target_wedding"] = _context.WeddingId.Value.ToString(),
                ["target_user"] = userId.ToString()
            });

            var content = response.Content?.Trim().Trim('"');
            if (!string.Equals(content, "true", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Mitglied konnte nicht entfernt werden (nur Admins, nicht sich selbst).");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> LeaveWeddingAsync(Guid weddingId)
    {
        try
        {
            var client = await _provider.GetClientAsync();
            var response = await client.Rpc("leave_wedding", new Dictionary<string, object>
            {
                ["target_wedding"] = weddingId.ToString()
            });

            var content = response.Content?.Trim().Trim('"');
            if (!string.Equals(content, "true", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Verlassen nicht möglich. Der letzte Admin muss das Projekt stattdessen löschen.");
            }

            // War es die aktive Hochzeit? Dann auf eine andere umschalten oder leeren.
            if (_context.WeddingId == weddingId)
            {
                await _context.PersistActiveAsync(null);
                _context.Clear();
                await LoadActiveWeddingAsync();
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
