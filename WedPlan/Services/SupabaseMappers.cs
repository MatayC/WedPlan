using WedPlan.Models;
using WedPlan.Models.Supabase;

namespace WedPlan.Services;

/// <summary>
/// Wandelt zwischen den Supabase-Datenbank-Zeilen (Row) und den bestehenden
/// App-/Domain-Modellen um. So bleiben Seiten und Dialoge unverändert.
/// Enums werden als int gespeichert (Reihenfolge = Wert).
/// </summary>
internal static class SupabaseMappers
{
    // ----- Guest -----
    public static Guest ToModel(this GuestRow r) => new()
    {
        Id = r.Id,
        FirstName = r.FirstName,
        LastName = r.LastName,
        Email = r.Email,
        Phone = r.Phone,
        Category = (GuestCategory)r.Category,
        Group = r.GroupName,
        Rsvp = (RsvpStatus)r.Rsvp,
        PlusOnes = r.PlusOnes,
        TableId = r.TableId,
        Menu = (MenuChoice)r.Menu,
        Allergies = r.Allergies,
        AttendsReception = r.AttendsReception,
        Notes = r.Notes
    };

    public static GuestRow ToRow(this Guest g, Guid weddingId) => new()
    {
        Id = g.Id,
        WeddingId = weddingId,
        FirstName = g.FirstName,
        LastName = g.LastName,
        Email = g.Email,
        Phone = g.Phone,
        Category = (int)g.Category,
        GroupName = g.Group,
        Rsvp = (int)g.Rsvp,
        PlusOnes = g.PlusOnes,
        TableId = g.TableId,
        Menu = (int)g.Menu,
        Allergies = g.Allergies,
        AttendsReception = g.AttendsReception,
        Notes = g.Notes
    };

    // ----- BudgetItem -----
    public static BudgetItem ToModel(this BudgetItemRow r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Category = r.Category,
        EstimatedCost = r.EstimatedCost,
        ActualCost = r.ActualCost,
        PaidAmount = r.PaidAmount,
        Status = (BudgetStatus)r.Status,
        Vendor = r.Vendor,
        DueDate = r.DueDate,
        Notes = r.Notes
    };

    public static BudgetItemRow ToRow(this BudgetItem b, Guid weddingId) => new()
    {
        Id = b.Id,
        WeddingId = weddingId,
        Title = b.Title,
        Category = b.Category,
        EstimatedCost = b.EstimatedCost,
        ActualCost = b.ActualCost,
        PaidAmount = b.PaidAmount,
        Status = (int)b.Status,
        Vendor = b.Vendor,
        DueDate = b.DueDate,
        Notes = b.Notes
    };

    // ----- ApartmentItem -----
    public static ApartmentItem ToModel(this ApartmentItemRow r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Category = r.Category,
        EstimatedCost = r.EstimatedCost,
        ActualCost = r.ActualCost,
        PaidAmount = r.PaidAmount,
        Status = (BudgetStatus)r.Status,
        Vendor = r.Vendor,
        DueDate = r.DueDate,
        Notes = r.Notes
    };

    public static ApartmentItemRow ToRow(this ApartmentItem a, Guid weddingId) => new()
    {
        Id = a.Id,
        WeddingId = weddingId,
        Title = a.Title,
        Category = a.Category,
        EstimatedCost = a.EstimatedCost,
        ActualCost = a.ActualCost,
        PaidAmount = a.PaidAmount,
        Status = (int)a.Status,
        Vendor = a.Vendor,
        DueDate = a.DueDate,
        Notes = a.Notes
    };

    // ----- TaskItem -----
    public static TaskItem ToModel(this TaskItemRow r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Description = r.Description,
        DueDate = r.DueDate,
        Status = (WedPlan.Models.TaskStatus)r.Status,
        Priority = (TaskPriority)r.Priority,
        Phase = (TaskPhase)r.Phase,
        Responsible = r.Responsible ?? "Beide",
        Category = r.Category ?? "Allgemein"
    };

    public static TaskItemRow ToRow(this TaskItem t, Guid weddingId) => new()
    {
        Id = t.Id,
        WeddingId = weddingId,
        Title = t.Title,
        Description = t.Description,
        DueDate = t.DueDate,
        Status = (int)t.Status,
        Priority = (int)t.Priority,
        Phase = (int)t.Phase,
        Responsible = t.Responsible,
        Category = t.Category
    };

    // ----- ScheduleItem -----
    public static ScheduleItem ToModel(this ScheduleItemRow r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Description = r.Description,
        StartTime = r.StartTime ?? DateTime.Today,
        EndTime = r.EndTime,
        Location = r.Location,
        ResponsiblePerson = r.ResponsiblePerson,
        Category = (ScheduleCategory)r.Category,
        IsMilestone = r.IsMilestone
    };

    public static ScheduleItemRow ToRow(this ScheduleItem s, Guid weddingId) => new()
    {
        Id = s.Id,
        WeddingId = weddingId,
        Title = s.Title,
        Description = s.Description,
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        Location = s.Location,
        ResponsiblePerson = s.ResponsiblePerson,
        Category = (int)s.Category,
        IsMilestone = s.IsMilestone
    };

    // ----- SeatingTable -----
    public static SeatingTable ToModel(this SeatingTableRow r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Shape = (TableShape)r.Shape,
        Capacity = r.Capacity,
        AssignedGuestIds = r.AssignedGuestIds?.ToList() ?? new List<Guid>()
    };

    public static SeatingTableRow ToRow(this SeatingTable t, Guid weddingId) => new()
    {
        Id = t.Id,
        WeddingId = weddingId,
        Name = t.Name,
        Shape = (int)t.Shape,
        Capacity = t.Capacity,
        AssignedGuestIds = t.AssignedGuestIds?.ToList() ?? new List<Guid>()
    };

    // ----- WeddingSettings -----
    public static WeddingSettings ToModel(this WeddingSettingsRow r) => new()
    {
        Partner1Name = r.Partner1Name,
        Partner2Name = r.Partner2Name,
        WeddingDate = r.WeddingDate,
        Location = r.Location,
        TotalBudget = r.TotalBudget,
        Currency = r.Currency,
        IsDarkMode = r.IsDarkMode,
        CoverImageUrl = r.CoverImageUrl,
        GalleryImage1Url = r.GalleryImage1Url,
        GalleryImage2Url = r.GalleryImage2Url,
        SavingsPartner1 = r.SavingsPartner1,
        SavingsPartner2 = r.SavingsPartner2,
        MonthlySavingPartner1 = r.MonthlySavingPartner1,
        MonthlySavingPartner2 = r.MonthlySavingPartner2
    };

    public static WeddingSettingsRow ToRow(this WeddingSettings s, Guid weddingId) => new()
    {
        WeddingId = weddingId,
        Partner1Name = s.Partner1Name,
        Partner2Name = s.Partner2Name,
        WeddingDate = s.WeddingDate,
        Location = s.Location,
        TotalBudget = s.TotalBudget,
        Currency = s.Currency,
        IsDarkMode = s.IsDarkMode,
        CoverImageUrl = s.CoverImageUrl,
        GalleryImage1Url = s.GalleryImage1Url,
        GalleryImage2Url = s.GalleryImage2Url,
        SavingsPartner1 = s.SavingsPartner1,
        SavingsPartner2 = s.SavingsPartner2,
        MonthlySavingPartner1 = s.MonthlySavingPartner1,
        MonthlySavingPartner2 = s.MonthlySavingPartner2
    };
}
