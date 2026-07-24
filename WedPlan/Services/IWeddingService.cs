namespace WedPlan.Services;

/// <summary>Berechtigungen eines Mitglieds je Seite plus Rolle/Löschrecht.</summary>
public class MemberPermissions
{
    public bool IsAdmin { get; set; }
    public bool CanEditGuests { get; set; }
    public bool CanEditBudget { get; set; }
    public bool CanEditTasks { get; set; }
    public bool CanEditSchedule { get; set; }
    public bool CanEditSeating { get; set; }
    public bool CanEditSettings { get; set; }
    public bool CanEditApartment { get; set; }
    public bool CanDeleteProject { get; set; }
}

/// <summary>Ein Mitglied einer Hochzeits-Gruppe (für die Mitgliederliste).</summary>
public record WeddingMember(Guid UserId, string Username, string? Email)
{
    /// <summary>Rechte des Mitglieds in der aktiven Hochzeit.</summary>
    public MemberPermissions Permissions { get; init; } = new();
}

/// <summary>Eine Hochzeit des Benutzers (für die Projektauswahl).</summary>
public record WeddingSummary(Guid Id, string Name, string? InviteCode);

/// <summary>
/// Service für die Verwaltung von Hochzeits-Gruppen: Anlegen, Beitreten per Code,
/// aktive Hochzeit laden und Mitglieder auflisten.
/// </summary>
public interface IWeddingService
{
    /// <summary>Lädt die aktive Hochzeit des angemeldeten Benutzers in den WeddingContext.</summary>
    Task<bool> LoadActiveWeddingAsync();

    /// <summary>Legt eine neue Hochzeit an und macht den Benutzer zum Mitglied.</summary>
    Task<(bool Success, string? Error)> CreateWeddingAsync(string name);

    /// <summary>Tritt einer Hochzeit per Einladungs-Code bei.</summary>
    Task<(bool Success, string? Error)> JoinWeddingAsync(string inviteCode);

    /// <summary>Liefert die Mitglieder der aktiven Hochzeit inkl. ihrer Rechte.</summary>
    Task<List<WeddingMember>> GetMembersAsync();

    /// <summary>Liefert alle Hochzeiten (Projekte), denen der Benutzer angehört.</summary>
    Task<List<WeddingSummary>> GetMyWeddingsAsync();

    /// <summary>Wechselt die aktive Hochzeit auf die angegebene Id.</summary>
    Task<bool> SwitchWeddingAsync(Guid weddingId);

    /// <summary>Löscht eine Hochzeit (Admins oder Berechtigte) inkl. aller zugehörigen Daten.</summary>
    Task<(bool Success, string? Error)> DeleteWeddingAsync(Guid weddingId);

    /// <summary>Setzt die Rechte eines Mitglieds (nur Admins). Speichert sofort.</summary>
    Task<(bool Success, string? Error)> UpdateMemberPermissionsAsync(Guid userId, MemberPermissions permissions);

    /// <summary>Entfernt ein Mitglied aus der aktiven Hochzeit (nur Admins).</summary>
    Task<(bool Success, string? Error)> RemoveMemberAsync(Guid userId);

    /// <summary>Verlässt die angegebene Hochzeit (das aktuelle Mitglied tritt aus).</summary>
    Task<(bool Success, string? Error)> LeaveWeddingAsync(Guid weddingId);
}
