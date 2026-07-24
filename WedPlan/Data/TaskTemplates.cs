using WedPlan.Models;

namespace WedPlan.Data;

/// <summary>
/// Vorgefertigte Standard-Checkliste mit typischen Hochzeitsaufgaben.
/// Wird auf Wunsch beim ersten Start angeboten, um die App schnell zu befüllen.
/// </summary>
public static class TaskTemplates
{
    /// <summary>
    /// Gibt eine vollständige Standard-Hochzeits-Checkliste zurück.
    /// Jede Aufgabe erhält eine neue Guid und ist als "Offen" markiert.
    /// </summary>
    public static List<TaskItem> GetDefaultChecklist() => new()
    {
        // ── 12+ Monate vorher ─────────────────────────────────────────────────
        Task("Hochzeitsdatum festlegen",
            "Datum abstimmen und ggf. mit Standesamt koordinieren.",
            TaskPhase.VorherMehr12Monate, TaskPriority.Hoch),
        Task("Budget festlegen",
            "Gesamtbudget definieren und auf Kategorien aufteilen.",
            TaskPhase.VorherMehr12Monate, TaskPriority.Hoch),
        Task("Gästeliste erstellen",
            "Erste Entwurfsliste aller einzuladenden Personen.",
            TaskPhase.VorherMehr12Monate, TaskPriority.Hoch),
        Task("Location besichtigen & buchen",
            "Mehrere Locations vergleichen und buchen.",
            TaskPhase.VorherMehr12Monate, TaskPriority.Hoch),
        Task("Standesamt-Termin vereinbaren",
            "Frühzeitig Termin beim Standesamt sichern.",
            TaskPhase.VorherMehr12Monate, TaskPriority.Hoch),
        Task("Trauredner / Pfarrer kontaktieren",
            "Freie Trauung oder kirchliche Trauung organisieren.",
            TaskPhase.VorherMehr12Monate, TaskPriority.Mittel),
        Task("Hochzeitsplanung starten (WedPlan nutzen 😊)",
            "App einrichten, Partner-Namen und Datum in den Einstellungen hinterlegen.",
            TaskPhase.VorherMehr12Monate, TaskPriority.Niedrig),

        // ── 6 Monate vorher ──────────────────────────────────────────────────
        Task("Fotograf buchen",
            "Portfolios vergleichen, Erstgespräch führen, Vertrag unterzeichnen.",
            TaskPhase.Vorher6Monate, TaskPriority.Hoch),
        Task("Videograf buchen", null,
            TaskPhase.Vorher6Monate, TaskPriority.Mittel),
        Task("Catering / Menü festlegen",
            "Menü-Optionen und Unverträglichkeiten klären.",
            TaskPhase.Vorher6Monate, TaskPriority.Hoch),
        Task("Musik / DJ / Band buchen", null,
            TaskPhase.Vorher6Monate, TaskPriority.Hoch),
        Task("Einladungen gestalten & drucken lassen", null,
            TaskPhase.Vorher6Monate, TaskPriority.Mittel),
        Task("Brautkleid / Anzug auswählen",
            "Anproben einplanen – Änderungen benötigen Zeit.",
            TaskPhase.Vorher6Monate, TaskPriority.Hoch),
        Task("Ringe aussuchen & bestellen", null,
            TaskPhase.Vorher6Monate, TaskPriority.Hoch),
        Task("Flitterwochen planen & buchen", null,
            TaskPhase.Vorher6Monate, TaskPriority.Mittel),
        Task("Sitzordnung planen (grob)",
            "Erste Version der Tisch-Aufteilung erstellen.",
            TaskPhase.Vorher6Monate, TaskPriority.Mittel),

        // ── 3 Monate vorher ──────────────────────────────────────────────────
        Task("Einladungen versenden",
            "Mit RSVP-Datum und allen wichtigen Infos.",
            TaskPhase.Vorher3Monate, TaskPriority.Hoch),
        Task("Friseur & Make-up buchen", null,
            TaskPhase.Vorher3Monate, TaskPriority.Hoch),
        Task("Blumen & Dekoration bestellen",
            "Blumenladen kontaktieren, Konzept festlegen.",
            TaskPhase.Vorher3Monate, TaskPriority.Mittel),
        Task("Hochzeitstorte bestellen", null,
            TaskPhase.Vorher3Monate, TaskPriority.Mittel),
        Task("Trauzeugen briefen",
            "Rollen, Aufgaben und Rede klären.",
            TaskPhase.Vorher3Monate, TaskPriority.Mittel),
        Task("Ablaufplan für den Hochzeitstag erstellen",
            "Zeitplan mit allen Programmpunkten finalisieren.",
            TaskPhase.Vorher3Monate, TaskPriority.Hoch),
        Task("Transport organisieren",
            "Brautauto, Shuttle für Gäste etc.",
            TaskPhase.Vorher3Monate, TaskPriority.Mittel),

        // ── 1 Monat vorher ───────────────────────────────────────────────────
        Task("Zusagen / Absagen final auswerten",
            "Finale Gästezahl an Catering und Location melden.",
            TaskPhase.Vorher1Monat, TaskPriority.Hoch),
        Task("Sitzordnung finalisieren", null,
            TaskPhase.Vorher1Monat, TaskPriority.Hoch),
        Task("Standesamtliche Dokumente vorbereiten",
            "Geburtsurkunden, Ausweise etc. bereithalten.",
            TaskPhase.Vorher1Monat, TaskPriority.Hoch),
        Task("Letzte Anprobe Brautkleid / Anzug", null,
            TaskPhase.Vorher1Monat, TaskPriority.Hoch),
        Task("Ringe abholen", null,
            TaskPhase.Vorher1Monat, TaskPriority.Hoch),
        Task("Bezahlungen ausstehender Rechnungen erledigen", null,
            TaskPhase.Vorher1Monat, TaskPriority.Mittel),
        Task("Notfallkoffer packen (Pflaster, Sicherheitsnadeln, …)", null,
            TaskPhase.Vorher1Monat, TaskPriority.Niedrig),

        // ── Woche davor ──────────────────────────────────────────────────────
        Task("Dienstleister final bestätigen",
            "Alle Anbieter noch einmal telefonisch bestätigen.",
            TaskPhase.WocheDavor, TaskPriority.Hoch),
        Task("Ablaufplan an alle Beteiligten verteilen", null,
            TaskPhase.WocheDavor, TaskPriority.Hoch),
        Task("Umschläge mit Honoraren vorbereiten",
            "Bargeld für Fotograf, DJ etc. bereithalten.",
            TaskPhase.WocheDavor, TaskPriority.Mittel),
        Task("Flitterwochen-Gepäck packen", null,
            TaskPhase.WocheDavor, TaskPriority.Niedrig),
        Task("Ausruhen & genießen 💆", null,
            TaskPhase.WocheDavor, TaskPriority.Niedrig),

        // ── Am Tag ───────────────────────────────────────────────────────────
        Task("Frühstück nicht vergessen 😊", null,
            TaskPhase.AmTag, TaskPriority.Niedrig),
        Task("Friseur & Make-up-Termin", null,
            TaskPhase.AmTag, TaskPriority.Hoch),
        Task("Ringe einpacken", null,
            TaskPhase.AmTag, TaskPriority.Hoch),
        Task("Pünktlich zur standesamtlichen Trauung erscheinen", null,
            TaskPhase.AmTag, TaskPriority.Hoch),
        Task("Brautstrauß übergeben lassen", null,
            TaskPhase.AmTag, TaskPriority.Mittel),

        // ── Danach ───────────────────────────────────────────────────────────
        Task("Dankeskarten verschicken",
            "Innerhalb von 4–6 Wochen nach der Hochzeit.",
            TaskPhase.Danach, TaskPriority.Mittel),
        Task("Hochzeitsfotos / -video erhalten & sichern", null,
            TaskPhase.Danach, TaskPriority.Hoch),
        Task("Namen ändern (Personalausweis, Pass, …)", null,
            TaskPhase.Danach, TaskPriority.Mittel),
        Task("Hochzeitsalbum gestalten lassen", null,
            TaskPhase.Danach, TaskPriority.Niedrig),
    };

    private static TaskItem Task(
        string title,
        string? description,
        TaskPhase phase,
        TaskPriority priority,
        string responsible = "Beide") => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        Description = description,
        Phase = phase,
        Priority = priority,
        Responsible = responsible,
        Status = WedPlan.Models.TaskStatus.Offen
    };
}
