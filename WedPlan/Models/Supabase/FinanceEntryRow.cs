using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Finanz-Eintrag-Zeile in der Datenbank.</summary>
[Table("finance_entries")]
public class FinanceEntryRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [Column("person")]
    public int Person { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("type")]
    public int Type { get; set; }

    [Column("category")]
    public int Category { get; set; }

    [Column("interval")]
    public int Interval { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }
}
