namespace WedPlan.Services;

/// <summary>Seiten-Schlüssel für seitenbasierte Bearbeitungsrechte.</summary>
public static class WeddingPages
{
    public const string Guests = "guests";
    public const string Budget = "budget";
    public const string Tasks = "tasks";
    public const string Schedule = "schedule";
    public const string Seating = "seating";
    public const string Settings = "settings";
    public const string Apartment = "apartment";
}

/// <summary>
/// Lädt und cached die Rolle und die seitenbasierten Bearbeitungsrechte des
/// aktuell angemeldeten Benutzers für die aktive Hochzeit. Alle Seiten fragen
/// hier ab, ob der Benutzer bearbeiten darf (CanEdit).
/// </summary>
public class PermissionService
{
    private readonly SupabaseClientProvider _provider;
    private readonly IAuthService _authService;
    private readonly WeddingContext _context;

    private Guid? _loadedWeddingId;
    private bool _isAdmin;
    private bool _canDeleteProject;
    private readonly Dictionary<string, bool> _edit = new();

    public PermissionService(SupabaseClientProvider provider, IAuthService authService, WeddingContext context)
    {
        _provider = provider;
        _authService = authService;
        _context = context;
    }

    /// <summary>Wird ausgelöst, wenn sich die geladenen Rechte ändern.</summary>
    public event Action? OnChange;

    /// <summary>Ist der aktuelle Benutzer Admin der aktiven Hochzeit?</summary>
    public bool IsAdmin => _isAdmin;

    /// <summary>Darf der aktuelle Benutzer das aktive Projekt löschen?</summary>
    public bool CanDeleteProject => _isAdmin || _canDeleteProject;

    /// <summary>Darf der aktuelle Benutzer die angegebene Seite bearbeiten?</summary>
    public bool CanEdit(string page)
    {
        if (_isAdmin)
        {
            return true;
        }
        return _edit.TryGetValue(page, out var allowed) && allowed;
    }

    /// <summary>
    /// Lädt die Rechte des aktuellen Benutzers für die aktive Hochzeit.
    /// Nutzt einen Cache pro Hochzeit; mit force wird neu geladen.
    /// </summary>
    public async Task EnsureLoadedAsync(bool force = false)
    {
        var weddingId = _context.WeddingId;
        if (weddingId is null)
        {
            Reset();
            return;
        }

        if (!force && _loadedWeddingId == weddingId)
        {
            return;
        }

        var userId = await _authService.GetCurrentUserIdAsync();
        if (userId is null)
        {
            Reset();
            return;
        }

        var client = await _provider.GetClientAsync();
        var members = await client
            .From<Models.Supabase.WeddingMemberRow>()
            .Where(m => m.WeddingId == weddingId.Value && m.UserId == userId.Value)
            .Get();

        var me = members.Models.FirstOrDefault();
        if (me is null)
        {
            Reset();
            return;
        }

        _isAdmin = me.Role == "admin";
        _canDeleteProject = me.CanDeleteProject;
        _edit[WeddingPages.Guests] = me.CanEditGuests;
        _edit[WeddingPages.Budget] = me.CanEditBudget;
        _edit[WeddingPages.Tasks] = me.CanEditTasks;
        _edit[WeddingPages.Schedule] = me.CanEditSchedule;
        _edit[WeddingPages.Seating] = me.CanEditSeating;
        _edit[WeddingPages.Settings] = me.CanEditSettings;
        _edit[WeddingPages.Apartment] = me.CanEditApartment;
        _loadedWeddingId = weddingId;

        OnChange?.Invoke();
    }

    private void Reset()
    {
        _loadedWeddingId = null;
        _isAdmin = false;
        _canDeleteProject = false;
        _edit.Clear();
        OnChange?.Invoke();
    }
}
