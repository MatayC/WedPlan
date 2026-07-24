using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Profil-Zeile (verknüpft mit auth.users). Enthält den Nutzernamen.</summary>
[Table("profiles")]
public class ProfileRow : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("email")]
    public string? Email { get; set; }
}
