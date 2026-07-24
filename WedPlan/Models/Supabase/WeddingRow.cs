using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Hochzeits-/Gruppen-Zeile mit Einladungs-Code.</summary>
[Table("weddings")]
public class WeddingRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("invite_code")]
    public string InviteCode { get; set; } = string.Empty;

    [Column("created_by")]
    public Guid? CreatedBy { get; set; }
}
