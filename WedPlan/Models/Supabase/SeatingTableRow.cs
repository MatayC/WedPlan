using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Sitzordnungs-Tisch-Zeile in der Datenbank.</summary>
[Table("seating_tables")]
public class SeatingTableRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("shape")]
    public int Shape { get; set; }

    [Column("capacity")]
    public int Capacity { get; set; } = 8;

    [Column("assigned_guest_ids")]
    public List<Guid> AssignedGuestIds { get; set; } = new();
}
