namespace WedPlan.Models;

// Zentrale Enums für Statuswerte der verschiedenen Bereiche der Hochzeitsplanung.

/// <summary>
/// Zusagestatus eines Gastes (RSVP).
/// </summary>
public enum RsvpStatus
{
    /// <summary>Noch keine Rückmeldung erhalten.</summary>
    Ausstehend = 0,

    /// <summary>Gast hat zugesagt.</summary>
    Zugesagt = 1,

    /// <summary>Gast hat abgesagt.</summary>
    Abgesagt = 2,

    /// <summary>Gast hat unter Vorbehalt zugesagt.</summary>
    Vielleicht = 3
}

/// <summary>
/// Kategorie/Beziehung eines Gastes zum Brautpaar.
/// </summary>
public enum GuestCategory
{
    Familie = 0,
    Freunde = 1,
    Kollegen = 2,
    Sonstige = 3
}

/// <summary>
/// Status eines Budget-Postens.
/// </summary>
public enum BudgetStatus
{
    /// <summary>Nur geplant, noch nichts beauftragt.</summary>
    Geplant = 0,

    /// <summary>Anbieter beauftragt, noch keine Zahlung.</summary>
    Beauftragt = 1,

    /// <summary>Anzahlung geleistet.</summary>
    Angezahlt = 2,

    /// <summary>Vollständig bezahlt.</summary>
    Bezahlt = 3
}

/// <summary>
/// Planungsphase einer Aufgabe (typische Hochzeits-Zeiträume).
/// Bestimmt die Gruppierung in der Checklisten-Ansicht.
/// </summary>
public enum TaskPhase
{
    /// <summary>Aufgaben, die 12 oder mehr Monate vor der Hochzeit erledigt werden sollten.</summary>
    VorherMehr12Monate = 0,

    /// <summary>Aufgaben 6–12 Monate vor der Hochzeit.</summary>
    Vorher6Monate = 1,

    /// <summary>Aufgaben 3–6 Monate vor der Hochzeit.</summary>
    Vorher3Monate = 2,

    /// <summary>Aufgaben 1–3 Monate vor der Hochzeit.</summary>
    Vorher1Monat = 3,

    /// <summary>Aufgaben in der Woche vor der Hochzeit.</summary>
    WocheDavor = 4,

    /// <summary>Aufgaben am Hochzeitstag selbst.</summary>
    AmTag = 5,

    /// <summary>Aufgaben nach der Hochzeit (Dankeskarten, etc.).</summary>
    Danach = 6
}

/// <summary>
/// Status einer Aufgabe in der Checkliste.
/// </summary>
public enum TaskStatus
{
    Offen = 0,
    InArbeit = 1,
    Erledigt = 2
}

/// <summary>
/// Priorität einer Aufgabe.
/// </summary>
public enum TaskPriority
{
    Niedrig = 0,
    Mittel = 1,
    Hoch = 2
}

/// <summary>
/// Form eines Tisches in der Sitzordnung.
/// </summary>
public enum TableShape
{
    Rund = 0,
    Eckig = 1,
    Lang = 2
}

/// <summary>
/// Menü-Wunsch eines Gastes.
/// </summary>
public enum MenuChoice
{
    Normal = 0,
    Vegetarisch = 1,
    Vegan = 2,
    Glutenfrei = 3
}

/// <summary>
/// Kategorie eines Zeitplan-Eintrags – bestimmt Farbe und Icon in der Timeline.
/// </summary>
public enum ScheduleCategory
{
    Vorbereitung = 0,
    Trauung = 1,
    Empfang = 2,
    Dinner = 3,
    Party = 4,
    Sonstiges = 5
}

/// <summary>
/// Ansichtsmodus für den Zeitplan: Hochzeitstag oder Planungs-Meilensteine.
/// </summary>
public enum ScheduleView
{
    /// <summary>Ablauf des Hochzeitstags (nach Uhrzeit).</summary>
    GroszerTag = 0,

    /// <summary>Planungs-Meilensteine im Vorfeld der Hochzeit.</summary>
    Planungsmeilensteine = 1
}

/// <summary>
/// Zuordnung eines Finanz-Eintrags zu einer der beiden Personen bzw. gemeinsam.
/// </summary>
public enum FinancePerson
{
    /// <summary>Partner 1 (Name aus den Hochzeits-Einstellungen).</summary>
    Partner1 = 0,

    /// <summary>Partner 2 (Name aus den Hochzeits-Einstellungen).</summary>
    Partner2 = 1
}

/// <summary>
/// Art eines Finanz-Eintrags – bestimmt, wie er in die Berechnung einfließt.
/// </summary>
public enum FinanceEntryType
{
    /// <summary>Einnahme/Einkommen.</summary>
    Einkommen = 0,

    /// <summary>Feste, regelmäßige Ausgabe (z.B. Miete).</summary>
    Fixkosten = 1,

    /// <summary>Variable/schwankende Ausgabe (z.B. Lebensmittel).</summary>
    VariableKosten = 2,

    /// <summary>Sparen/Rücklage.</summary>
    Sparen = 3
}

/// <summary>
/// Kategorie eines Finanz-Eintrags (erweiterbar).
/// </summary>
public enum FinanceCategory
{
    Miete = 0,
    Versicherung = 1,
    Auto = 2,
    Lebensmittel = 3,
    Abos = 4,
    Freizeit = 5,
    HochzeitRuecklage = 6,
    Gehalt = 7,
    Sonstiges = 8
}

/// <summary>
/// Intervall eines Finanz-Eintrags – für die Umrechnung auf einen Monatswert.
/// </summary>
public enum FinanceInterval
{
    /// <summary>Betrag fällt monatlich an.</summary>
    Monatlich = 0,

    /// <summary>Betrag fällt jährlich an (wird durch 12 geteilt).</summary>
    Jaehrlich = 1,

    /// <summary>Einmaliger Betrag (fließt nicht in die Monatsberechnung ein).</summary>
    Einmalig = 2
}

