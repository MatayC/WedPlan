using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Monatlicher Kontostand-Zeile in der Datenbank.</summary>
[Table("finance_balances")]
public class FinanceBalanceRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [Column("person")]
    public int Person { get; set; }

    [Column("month")]
    public DateTime Month { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }
}
