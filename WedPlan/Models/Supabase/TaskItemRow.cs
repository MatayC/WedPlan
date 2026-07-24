using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Aufgaben-/Checklisten-Zeile in der Datenbank.</summary>
[Table("tasks")]
public class TaskItemRow : BaseModel
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

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("priority")]
    public int Priority { get; set; }

    [Column("phase")]
    public int Phase { get; set; }

    [Column("responsible")]
    public string? Responsible { get; set; }

    [Column("category")]
    public string? Category { get; set; }
}
