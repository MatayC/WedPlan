using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Budget-Posten-Zeile in der Datenbank.</summary>
[Table("budget_items")]
public class BudgetItemRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("category")]
    public string Category { get; set; } = "Allgemein";

    [Column("estimated_cost")]
    public decimal EstimatedCost { get; set; }

    [Column("actual_cost")]
    public decimal ActualCost { get; set; }

    [Column("paid_amount")]
    public decimal PaidAmount { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("vendor")]
    public string? Vendor { get; set; }

    [Column("due_date")]
    public DateTime? DueDate { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }
}
