using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Gast-Zeile in der Datenbank.</summary>
[Table("guests")]
public class GuestRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("email")]
    public string? Email { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("category")]
    public int Category { get; set; }

    [Column("group_name")]
    public string GroupName { get; set; } = "Sonstige";

    [Column("rsvp")]
    public int Rsvp { get; set; }

    [Column("plus_ones")]
    public int PlusOnes { get; set; }

    [Column("table_id")]
    public Guid? TableId { get; set; }

    [Column("menu")]
    public int Menu { get; set; }

    [Column("allergies")]
    public string? Allergies { get; set; }

    [Column("attends_reception")]
    public bool AttendsReception { get; set; } = true;

    [Column("notes")]
    public string? Notes { get; set; }
}
