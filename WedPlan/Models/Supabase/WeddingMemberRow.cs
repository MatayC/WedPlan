using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Mitgliedschaft eines Benutzers in einer Hochzeit.</summary>
[Table("wedding_members")]
public class WeddingMemberRow : BaseModel
{
    [PrimaryKey("wedding_id", false)]
    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [PrimaryKey("user_id", false)]
    [Column("user_id")]
    public Guid UserId { get; set; }

    /// <summary>Rolle: "admin" (voller Zugriff + Rechteverwaltung) oder "member".</summary>
    [Column("role")]
    public string Role { get; set; } = "member";

    [Column("can_edit_guests")]
    public bool CanEditGuests { get; set; }

    [Column("can_edit_budget")]
    public bool CanEditBudget { get; set; }

    [Column("can_edit_tasks")]
    public bool CanEditTasks { get; set; }

    [Column("can_edit_schedule")]
    public bool CanEditSchedule { get; set; }

    [Column("can_edit_seating")]
    public bool CanEditSeating { get; set; }

    [Column("can_edit_settings")]
    public bool CanEditSettings { get; set; }

    [Column("can_edit_apartment")]
    public bool CanEditApartment { get; set; }

    [Column("can_delete_project")]
    public bool CanDeleteProject { get; set; }
}
