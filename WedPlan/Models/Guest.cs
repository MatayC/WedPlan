namespace WedPlan.Models;

/// <summary>
/// Repräsentiert einen Gast auf der Gästeliste.
/// </summary>
public class Guest
{
    /// <summary>Eindeutige Id des Gastes.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Vorname des Gastes.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Nachname des Gastes.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Vollständiger Name (Vor- und Nachname zusammengesetzt).
    /// Berechnet – wird nicht separat gespeichert.
    /// </summary>
    public string Name => $"{FirstName} {LastName}".Trim();

    /// <summary>Optionale E-Mail-Adresse.</summary>
    public string? Email { get; set; }

    /// <summary>Optionale Telefonnummer.</summary>
    public string? Phone { get; set; }

    /// <summary>Beziehung/Kategorie des Gastes (Enum, für Statistik/Filter).</summary>
    public GuestCategory Category { get; set; } = GuestCategory.Sonstige;

    /// <summary>
    /// Frei wählbare Gruppe/Kategorie (z.B. "Familie Braut", "Freunde").
    /// Frei erweiterbar für die Gruppierung in der Gästeliste.
    /// </summary>
    public string Group { get; set; } = "Sonstige";

    /// <summary>Zusagestatus (RSVP).</summary>
    public RsvpStatus Rsvp { get; set; } = RsvpStatus.Ausstehend;

    /// <summary>Anzahl der Begleitpersonen (Plus One / Kinder).</summary>
    public int PlusOnes { get; set; }

    /// <summary>Optionale Zuordnung zu einem Tisch (Verknüpfung mit der Sitzordnung).</summary>
    public Guid? TableId { get; set; }

    /// <summary>Menü-Wunsch des Gastes.</summary>
    public MenuChoice Menu { get; set; } = MenuChoice.Normal;

    /// <summary>Allergien/Unverträglichkeiten (Freitext).</summary>
    public string? Allergies { get; set; }

    /// <summary>Gibt an, ob der Gast zur Feier eingeladen ist (nicht nur zur Trauung).</summary>
    public bool AttendsReception { get; set; } = true;

    /// <summary>Besondere Hinweise/Notiz (Freitext).</summary>
    public string? Notes { get; set; }
}
