using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Zeitplan-/Programmpunkt-Zeile in der Datenbank.</summary>
[Table("schedule_items")]
public class ScheduleItemRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("start_time")]
    public DateTime? StartTime { get; set; }

    [Column("end_time")]
    public DateTime? EndTime { get; set; }

    [Column("location")]
    public string? Location { get; set; }

    [Column("responsible_person")]
    public string? ResponsiblePerson { get; set; }

    [Column("category")]
    public int Category { get; set; }

    [Column("is_milestone")]
    public bool IsMilestone { get; set; }
}
