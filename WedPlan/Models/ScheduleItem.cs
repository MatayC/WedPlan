namespace WedPlan.Models;

/// <summary>
/// Repräsentiert einen Programmpunkt im Zeitplan/der Timeline des Hochzeitstags.
/// </summary>
public class ScheduleItem
{
    /// <summary>Eindeutige Id des Programmpunkts.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Titel des Programmpunkts (z.B. "Standesamtliche Trauung").</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optionale Beschreibung/Details.</summary>
    public string? Description { get; set; }

    /// <summary>Startzeitpunkt des Programmpunkts.</summary>
    public DateTime StartTime { get; set; } = DateTime.Today;

    /// <summary>Optionaler Endzeitpunkt.</summary>
    public DateTime? EndTime { get; set; }

    /// <summary>Ort/Location des Programmpunkts.</summary>
    public string? Location { get; set; }

    /// <summary>Verantwortliche Person/Ansprechpartner.</summary>
    public string? ResponsiblePerson { get; set; }

    /// <summary>Kategorie des Eintrags – bestimmt Farbe und Icon in der Timeline.</summary>
    public ScheduleCategory Category { get; set; } = ScheduleCategory.Sonstiges;

    /// <summary>
    /// Gibt an, ob dieser Eintrag ein Planungs-Meilenstein im Vorfeld ist (true)
    /// oder zum Ablauf des Hochzeitstags gehört (false).
    /// </summary>
    public bool IsMilestone { get; set; }
}
