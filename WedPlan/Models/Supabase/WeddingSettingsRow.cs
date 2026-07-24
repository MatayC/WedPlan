using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WedPlan.Models.Supabase;

/// <summary>Einstellungen-Zeile (genau eine pro Hochzeit) in der Datenbank.</summary>
[Table("wedding_settings")]
public class WeddingSettingsRow : BaseModel
{
    [PrimaryKey("wedding_id", false)]
    [Column("wedding_id")]
    public Guid WeddingId { get; set; }

    [Column("partner1_name")]
    public string Partner1Name { get; set; } = string.Empty;

    [Column("partner2_name")]
    public string Partner2Name { get; set; } = string.Empty;

    [Column("wedding_date")]
    public DateTime? WeddingDate { get; set; }

    [Column("location")]
    public string? Location { get; set; }

    [Column("total_budget")]
    public decimal TotalBudget { get; set; }

    [Column("currency")]
    public string Currency { get; set; } = "EUR";

    [Column("is_dark_mode")]
    public bool IsDarkMode { get; set; }

    [Column("cover_image_url")]
    public string? CoverImageUrl { get; set; }

    [Column("gallery_image1_url")]
    public string? GalleryImage1Url { get; set; }

    [Column("gallery_image2_url")]
    public string? GalleryImage2Url { get; set; }

    [Column("savings_partner1")]
    public decimal SavingsPartner1 { get; set; }

    [Column("savings_partner2")]
    public decimal SavingsPartner2 { get; set; }

    [Column("monthly_saving_partner1")]
    public decimal MonthlySavingPartner1 { get; set; }

    [Column("monthly_saving_partner2")]
    public decimal MonthlySavingPartner2 { get; set; }
}
