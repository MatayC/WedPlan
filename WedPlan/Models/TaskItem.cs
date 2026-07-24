namespace WedPlan.Models;

/// <summary>
/// Repräsentiert eine Aufgabe in der To-Do-/Checkliste.
/// </summary>
public class TaskItem
{
    /// <summary>Eindeutige Id der Aufgabe.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Titel der Aufgabe.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optionale detaillierte Beschreibung.</summary>
    public string? Description { get; set; }

    /// <summary>Fälligkeitsdatum der Aufgabe.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>Aktueller Status der Aufgabe.</summary>
    public TaskStatus Status { get; set; } = TaskStatus.Offen;

    /// <summary>Priorität der Aufgabe.</summary>
    public TaskPriority Priority { get; set; } = TaskPriority.Mittel;

    /// <summary>Planungsphase – bestimmt die Gruppierung in der Checklisten-Ansicht.</summary>
    public TaskPhase Phase { get; set; } = TaskPhase.Vorher6Monate;

    /// <summary>Verantwortliche Person (z.B. "Partner 1", "Partner 2", "Beide").</summary>
    public string Responsible { get; set; } = "Beide";

    /// <summary>Freie Kategorie zur zusätzlichen Gruppierung (optional).</summary>
    public string Category { get; set; } = "Allgemein";

    /// <summary>Gibt an, ob die Aufgabe erledigt ist (Kurzform für Status == Erledigt).</summary>
    public bool IsDone
    {
        get => Status == WedPlan.Models.TaskStatus.Erledigt;
        set => Status = value ? WedPlan.Models.TaskStatus.Erledigt : WedPlan.Models.TaskStatus.Offen;
    }
}
