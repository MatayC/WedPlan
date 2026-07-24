namespace WedPlan.Models;

/// <summary>
/// Repräsentiert einen Tisch in der Sitzordnung.
/// Die Gastliste wird als Liste von Guest-Ids gehalten und ist
/// mit dem Guest.TableId-Feld synchronisiert.
/// </summary>
public class SeatingTable
{
    /// <summary>Eindeutige Id des Tisches.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Bezeichnung des Tisches (z.B. "Tisch 1", "Familientisch").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Form des Tisches.</summary>
    public TableShape Shape { get; set; } = TableShape.Rund;

    /// <summary>Maximale Anzahl an Plätzen.</summary>
    public int Capacity { get; set; } = 8;

    /// <summary>
    /// Ids der zugeordneten Gäste (inklusive Begleitpersonen werden separat berechnet).
    /// Canonical source of truth für die Tischzuordnung – Guest.TableId ist die
    /// denormalisierte Kopie für schnelle Abfragen in der Gästeliste.
    /// </summary>
    public List<Guid> AssignedGuestIds { get; set; } = new();

    /// <summary>Anzahl belegter Plätze (Gäste + ihre Begleitpersonen).</summary>
    public int OccupiedSeats(IEnumerable<Models.Guest> allGuests)
        => allGuests
            .Where(g => AssignedGuestIds.Contains(g.Id))
            .Sum(g => 1 + g.PlusOnes);

    /// <summary>Gibt an, ob der Tisch überbelegt ist.</summary>
    public bool IsOverbooked(IEnumerable<Models.Guest> allGuests)
        => OccupiedSeats(allGuests) > Capacity;
}

